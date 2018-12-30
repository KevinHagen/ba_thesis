using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CellularAutomata
{
	/// <summary>
	/// Implementation of a CellularAutomaton. It is designed with a set of parameters to allow customization and experimentation with the algorithm.
	/// The default configuartion utilizes a Moore neighbourhood (radius 1) with a 5-4 rule and a 45% chance for a tile to be a wall.
	/// </summary>
	public class CellularAutomaton : BaseGenerator
	{
		#region Serialize Fields

		[Header("Cellular Automaton")] [SerializeField]
		private int _generations;
		[SerializeField] [Range(0, 1)] private float _startAliveChance;
		[SerializeField] private Tile _groundPrefab;
		/// <summary>
		///     Minimum neighbours necessary to give birth to a cell
		/// </summary>
		[Header("Rules")] [SerializeField]
		private int _lowerBirthLimit;
		/// <summary>
		///     Maximum neighbours allowed to give birth to a cell
		/// </summary>
		[SerializeField] private int _upperBirthLimit;
		/// <summary>
		///     Maximum cells which can 'live' in a neighbourhood before die of overpopulation
		/// </summary>
		[SerializeField] private int _overPopulationLimit;
		/// <summary>
		///     Minimum neighbours necessary to prevent starvation of cells in a neighbourhood
		/// </summary>
		[SerializeField] private int _starvationLimit;
		[Header("Neighbourhood")] [SerializeField]
		private int _stepRange;

		#endregion

		#region Private Fields

		/// <summary>
		///     All available neighbourhoods
		/// </summary>
		private List<AbstractNeighbourhood> _neighbourhoods;
		/// <summary>
		///     Currently used neighbourhood
		/// </summary>
		private AbstractNeighbourhood _currentNeighbourhood;
		/// <summary>
		///     Stores data foreach generation generated in the process
		/// </summary>
		private bool[][,] _generationSteps;
		/// <summary>
		///     Currently displayed generation
		/// </summary>
		private int _currentGeneration;
		private Slider _showStepSlider;

		#endregion

		#region Unity methods

		protected override void Awake()
		{
			base.Awake();
			CAMenu menu = UI as CAMenu;
			menu.ShowStepSlider.onValueChanged.AddListener(ShowStep);
			_showStepSlider = menu.ShowStepSlider;

			//Given neighbourhoods for the example. Feel free to add any.
			_neighbourhoods = new List<AbstractNeighbourhood>
			                  {
				                  new MooreNeighbourhood(),
				                  new VonNeumannNeighbourhood()
			                  };
		}

		#endregion

		#region Public methods

		/// <summary>
		///     Generate the map and its data
		/// </summary>
		public override void Generate()
		{
			base.Generate();

			//if we may not visualize or dont want to, do the generation instantly
			if (!_useVisualization || !_allowVisualisation)
			{
				for (int i = 0; i < _generations; i++)
				{
					ApplyRules();
				}

				InstantiateMap();
			}
		}

		/// <summary>
		///     Clears the generated data
		/// </summary>
		public override void Clear()
		{
			base.Clear();
			_currentGeneration = 0;
		}

		#endregion

		#region Protected methods

		/// <summary>
		///     Sets up all data prior to the actual generation
		/// </summary>
		protected override void PrepareInitialState()
		{
			base.PrepareInitialState();

			_generationSteps = new bool[_generations + 1][,];
			for (int i = 0; i < _generationSteps.Length; i++)
			{
				_generationSteps[i] = new bool[_width, _height];
			}

			//default to MooreNeighbourhood
			if (_currentNeighbourhood == null)
				_currentNeighbourhood = new MooreNeighbourhood();

			//Update neighbourhood with current values
			_currentNeighbourhood.UpdateBoundaries(_width, _height);
			_currentNeighbourhood.SetRange(_stepRange);

			InitMapWithRandomDistribution();
		}

		/// <summary>
		///     Instantiate map from the generated data
		/// </summary>
		protected override void InstantiateMap()
		{
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					Tile tile = Instantiate(_groundPrefab, transform);
					tile.transform.localPosition = new Vector3(x, y);
					//Set each tile to a color representing its state (white = dead, black = alive)
					tile.SetTileColor(_tileData[x, y] ? Color.black : Color.white);
					_tiles[x, y] = tile;
				}
			}
		}

		/// <summary>
		///     Retrieves the generator specific data from the ui
		/// </summary>
		protected override void GetGeneratorSpecificData()
		{
			CAMenu menu = UI as CAMenu;
			_generations = menu.Generations;
			_startAliveChance = menu.StartAliveChance;
			_currentNeighbourhood = _neighbourhoods[menu.NeighbourhoodIndex] ?? new MooreNeighbourhood();
			_stepRange = menu.NeighbourhoodRange;
			_lowerBirthLimit = menu.LowerBirthLimit;
			_upperBirthLimit = menu.UpperBirthLimit;
			_starvationLimit = menu.StarvationLimit;
			_overPopulationLimit = menu.OverPopulationLimit;
		}

		/// <summary>
		///     Generates a map in steps executed after a given time interval
		/// </summary>
		/// <returns></returns>
		protected override IEnumerator StepByStepGeneration()
		{
			for (int i = 0; i < _generations; i++)
			{
				yield return new WaitForSeconds(_timeBetweenSteps);
				ApplyRules();
				ShowStep(_currentGeneration);
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		///     Show level at a specific step from the generation process
		/// </summary>
		/// <param name="arg0"></param>
		private void ShowStep(float arg0)
		{
			int step = (int) arg0;
			if ((_tiles == null) || (_tiles[0, 0] == null) || (step > _generations))
				return;

			//Set each tile to its state at generation step
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					_tiles[x, y].SetTileColor(_generationSteps[step][x, y] ? Color.black : Color.white);
				}
			}

			_currentGeneration = step;
		}

		/// <summary>
		///     Create a random initial distribution of dead/alive tiles
		/// </summary>
		private void InitMapWithRandomDistribution()
		{
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					//Set the border tiles to alive by default to create a frame of walls surrounding the level
					if ((x == 0) || (x == _width - 1) || (y == 0) || (y == _height - 1))
					{
						_tileData[x, y] = true;
					}
					//Set other tiles to either dead/alive based on a pseudo-random value
					else
					{
						_tileData[x, y] = Random.Range(0f, 1f) < _startAliveChance;
					}
				}
			}

			//Store as generation0
			_generationSteps[_currentGeneration] = _tileData;
			if (_showStepSlider)
				_showStepSlider.value = _currentGeneration;
		}

		/// <summary>
		///     Applies the current rule-set configuration
		/// </summary>
		private void ApplyRules()
		{
			//Create a new array to store the state of cells in the next generation
			bool[,] newData = new bool[_width, _height];
			_currentNeighbourhood.TileData = _tileData;

			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					int livingNeighbourCount = _currentNeighbourhood.GetLivingNeighboursCount(x, y);

					//give birth to a cell if livingNeighbourCount > birthLimit and cell is not alive
					if (!_tileData[x, y] && (livingNeighbourCount >= _lowerBirthLimit) && (livingNeighbourCount <= _upperBirthLimit))
						newData[x, y] = true;
					//let a cell survive if livingNeighbourCount is inbetween the starvation and overpopulation Limits
					else if (_tileData[x, y] && (livingNeighbourCount >= _starvationLimit) && (livingNeighbourCount <= _overPopulationLimit))
						newData[x, y] = true;
					//if none of the above is true, kill it.
					else
						newData[x, y] = false;
				}
			}

			//after generation process we can safely override the cell states
			_tileData = newData;
			_currentGeneration++;

			//store as generationX
			_generationSteps[_currentGeneration] = _tileData;
			if (_showStepSlider)
				_showStepSlider.value = _currentGeneration;
		}

		#endregion
	}
}