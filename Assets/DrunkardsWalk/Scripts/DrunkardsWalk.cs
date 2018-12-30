using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DrunkardsWalk
{
	/// <summary>
	///     Implementation of a CellularAutomaton. It is designed with a set of parameters to allow customization and
	///     experimentation with the algorithm.
	/// </summary>
	public class DrunkardsWalk : BaseGenerator
	{
		#region StepScheme enum

		/// <summary>
		///     StepScheme to use for moving around the grid
		/// </summary>
		public enum StepScheme
		{
			Traditional,
			EightWays,
			Hexagonal
		}

		#endregion

		#region Static Stuff

		public const float HexGridYOffset = 0.435f;
		public const float HexGridXOffset = 0.755f;

		#endregion

		#region Serialize Fields

		[Header("Setup")] [SerializeField]
		private Walker _walkerPrefab;
		[SerializeField] private Tile _squareTilePrefab;
		[SerializeField] private Tile _hexagonalTilePrefab;
		[Header("Drunkards Walk")] [SerializeField]
		private StepScheme _stepScheme;
		[SerializeField] [Range(0, 1)] private float _targetCarvedRate;
		[SerializeField] private bool _beginInCenter;
		[SerializeField] private bool _biasTowardsPreviousDirection;
		[SerializeField] [Range(0, 1)] private float _previousDirectionChance;
		[SerializeField] [Range(0, 1)] private float _previousDirectionChanceDecayRate;
		[Header("Walkers")] [SerializeField]
		private float _walkerSpawnChance;
		[SerializeField] private float _isRoomieChance;
		[SerializeField] private int _maxWalkerCount;
		[Header("Rooms")] [SerializeField]
		private List<Room> _rooms = new List<Room>();
		[SerializeField] [Range(0, 1)] private float _spawnRoomChance;
		[Header("Levy Flight")] [SerializeField]
		private bool _levyFlightEnabled;
		[SerializeField] [Range(0, 1)] private float _levyFlightChance;
		[SerializeField] [Range(1, 16)] private int _maxStepLength;
		[Header("Debug")] [SerializeField]
		private bool _showDebugColors;
		[SerializeField] private bool _showWhileGeneration;
		[SerializeField] private Color _defaultWalkColor;
		[SerializeField] private Color _roomColor;
		[SerializeField] private Color _corridorColor;
		[SerializeField] private Color _levyFlightColor;
		[SerializeField] private Color _newWalkerColor;
		[SerializeField] private Color _deadWalkerColor;
		[SerializeField] private DebugColorPanel _colorPanel;

		#endregion

		#region Private Fields

		private List<Walker> _walkers = new List<Walker>();
		private float _tilesCarved;
		private Color[,] _debugColors;

		#endregion

		#region Unity methods

		protected override void Awake()
		{
			base.Awake();
			DrunkardsMenu menu = UI as DrunkardsMenu;
			menu.EnableLevyFlightToggle.onValueChanged.AddListener(b => _levyFlightEnabled = b);
			menu.LevyFlightChanceSlider.onValueChanged.AddListener(v => _levyFlightChance = v);
			menu.MaxStepLengthSlider.onValueChanged.AddListener(v => _maxStepLength = (int) v);

			menu.BiasTowardsPreviousToggle.onValueChanged.AddListener(b => _biasTowardsPreviousDirection = b);
			menu.PreviousDirectionBiasSlider.onValueChanged.AddListener(v => _previousDirectionChance = v);
			menu.DecayRateSlider.onValueChanged.AddListener(v => _previousDirectionChanceDecayRate = v);

			menu.SpawnRoomChanceSlider.onValueChanged.AddListener(v => _spawnRoomChance = v);

			menu.WalkerSpawnRateSlider.onValueChanged.AddListener(v => _walkerSpawnChance = v);
			menu.RoomieChanceSlider.onValueChanged.AddListener(v => _isRoomieChance = v);
			menu.MaxWalkerSlider.onValueChanged.AddListener(v => _maxWalkerCount = (int) v);

			_colorPanel.ShowLiveToggle.onValueChanged.AddListener(b => _showWhileGeneration = b);
			_colorPanel.ShowToggle.onValueChanged.AddListener(b =>
			                                                  {
				                                                  _showDebugColors = b;
				                                                  ColorizeMap();
			                                                  });
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
				InstantiateMap();
				SpawnWalker(_beginInCenter ? _width / 2 : Random.Range(0, _width), _beginInCenter ? _height / 2 : Random.Range(0, _height), Random.Range(0f, 1f) < _isRoomieChance);
				while (_tilesCarved / (_width * _height) < _targetCarvedRate)
				{
					WalkTheWalkers(false);
				}

				ColorizeMap();
				foreach (Walker walker in _walkers)
				{
					walker.transform.localPosition = _tiles[walker.XMapPos, walker.YMapPos].transform.localPosition;
				}
			}
		}

		/// <summary>
		///     Clears the generated data
		/// </summary>
		public override void Clear()
		{
			base.Clear();

			foreach (Walker walker in _walkers)
			{
#if UNITY_EDITOR
				if (EditorApplication.isPlaying)
					Destroy(walker, 0.2f);
				else
					DestroyImmediate(walker);
#else
				Destroy(walker, 0.2f);
#endif
			}

			_walkers.Clear();
			_tilesCarved = 0;
		}

		#endregion

		#region Protected methods

		/// <summary>
		///     Retrieves the generator specific data from the ui
		/// </summary>
		protected override void GetGeneratorSpecificData()
		{
			DrunkardsMenu menu = UI as DrunkardsMenu;
			_targetCarvedRate = menu.CarveRate;
			_beginInCenter = menu.StartinCenter;
			_stepScheme = menu.StepScheme;
			_maxStepLength = menu.MaxStepLength;
			_levyFlightChance = menu.LevyFlightChance;
			_levyFlightEnabled = menu.EnableLevyFlight;
			_biasTowardsPreviousDirection = menu.BiasTowardsPrevious;
			_previousDirectionChance = menu.PreviousDirectionBias;
			_previousDirectionChanceDecayRate = menu.DecayRate;
			_spawnRoomChance = menu.SpawnRoomChance;
			_walkerSpawnChance = menu.WalkerSpawnRate;
			_isRoomieChance = menu.IsRoomieChance;
			_maxWalkerCount = menu.MaxWalkerCount;

			_showWhileGeneration = _colorPanel.ShowLiveToggle.isOn;
			_showDebugColors = _colorPanel.ShowToggle.isOn;

			_rooms.Clear();
			foreach (RoomEntryUI menuRoomEntry in menu.RoomEntries)
			{
				_rooms.Add(new Room(menuRoomEntry.Width, menuRoomEntry.Height, menuRoomEntry.Probability));
			}

			_rooms.Sort();
		}

		/// <summary>
		///     Instantiate map from the generated data
		/// </summary>
		protected override void InstantiateMap()
		{
			switch (_stepScheme)
			{
				case StepScheme.Traditional:
				case StepScheme.EightWays:
					//Square-board
					for (int i = 0; i < _width; i++)
					{
						for (int j = 0; j < _height; j++)
						{
							Tile newTile = Instantiate(_squareTilePrefab, transform);
							newTile.transform.localPosition = new Vector2(i, j);
							_tiles[i, j] = newTile;
						}
					}

					break;
				case StepScheme.Hexagonal:
					//hex-board
					for (int i = 0; i < _width; i++)
					{
						for (int j = 0; j < _height; j++)
						{
							Tile newTile = Instantiate(_hexagonalTilePrefab, transform);
							bool oddRow = i % 2 != 0;
							newTile.transform.localPosition = new Vector2(i * HexGridXOffset, oddRow ? 2 * (HexGridYOffset * j) + HexGridYOffset : j * HexGridYOffset * 2);
							_tiles[i, j] = newTile;
						}
					}

					break;
			}
		}

		/// <summary>
		///     Generates a map in steps executed after a given time interval
		/// </summary>
		/// <returns></returns>
		protected override IEnumerator StepByStepGeneration()
		{
			//Colorize initial map state (all black/walls), then spawn a walker on the map
			ColorizeMap();
			SpawnWalker(_beginInCenter ? _width / 2 : Random.Range(0, _width), _beginInCenter ? _height / 2 : Random.Range(0, _height), Random.Range(0f, 1f) < _isRoomieChance);
			//carve until the desired fillrate is reached
			while (_tilesCarved / (_width * _height) < _targetCarvedRate)
			{
				yield return new WaitForSeconds(_timeBetweenSteps);
				WalkTheWalkers();
			}
			//Colorize again, so the debug colors are potentially drawn
			ColorizeMap();
		}

		/// <summary>
		///     Sets up all data prior to the actual generation
		/// </summary>
		protected override void PrepareInitialState()
		{
			base.PrepareInitialState();
			_debugColors = new Color[_width, _height];
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					_debugColors[x, y] = Color.black;
				}
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		///     Colorizes the map either in the stored debug colors or white and black cells
		/// </summary>
		private void ColorizeMap()
		{
			if ((_tiles == null) || (_tiles[0, 0] == null)) return;

			if (!_showDebugColors)
			{
				for (int x = 0; x < _width; x++)
				{
					for (int y = 0; y < _height; y++)
					{
						_tiles[x, y].SetTileColor(_tileData[x, y] ? Color.white : Color.black);
					}
				}
			}
			else
			{
				for (int x = 0; x < _width; x++)
				{
					for (int y = 0; y < _height; y++)
					{
						_tiles[x, y].SetTileColor(_debugColors[x, y]);
					}
				}
			}
		}

		/// <summary>
		///     Walks each walker one step and checks their special actions (LF, Rooms, ...)
		/// </summary>
		/// <param name="showMovement">instantaneously visualize?</param>
		private void WalkTheWalkers(bool showMovement = true)
		{
			//Need a new list to avoid concurrentModificationException
			List<Walker> breedList = new List<Walker>();
			foreach (Walker walker in _walkers)
			{
				walker.Walk();
				MoveWalker(showMovement, walker, walker.IsRepeatingDirection ? _corridorColor : _defaultWalkColor);

				//Possibly execute a levy flight if it is enabled
				if (_levyFlightEnabled)
					LevyFlight(showMovement, walker);
				//Possibly spawn a room if the walker is allowed to
				if (walker.IsRoomie)
					SpawnRoom(walker, showMovement);
				if ((_walkers.Count < _maxWalkerCount) && (Random.Range(0f, 1f) < _walkerSpawnChance))
					breedList.Add(walker);
			}

			foreach (Walker walker in breedList)
			{
				SpawnWalker(walker.XMapPos, walker.YMapPos, Random.Range(0f, 1f) < _isRoomieChance);
			}
		}

		/// <summary>
		///     Move the given walker around the map and carves its underlying tile
		/// </summary>
		/// <param name="showMovement">Visualize or not</param>
		/// <param name="walker">Walker to move</param>
		/// <param name="debugColor">Debug color that should be used to visualize this specific step</param>
		private void MoveWalker(bool showMovement, Walker walker, Color debugColor = default(Color))
		{
			// Move the walker to its new position and carve the underlying tile
			if (showMovement)
			{
				//update position and instantaneously visualize
				if (_tiles[walker.XMapPos, walker.YMapPos].VisitTile())
					_tilesCarved++;

				walker.transform.localPosition = _tiles[walker.XMapPos, walker.YMapPos].transform.localPosition;
			}
			else
			{
				//tile was a new one if the underlying data was not true
				if (!_tileData[walker.XMapPos, walker.YMapPos])
					_tilesCarved++;
			}

			if (!_tileData[walker.XMapPos, walker.YMapPos])
			{
				if (_showWhileGeneration)
					_tiles[walker.XMapPos, walker.YMapPos].SetTileColor(debugColor);
				_debugColors[walker.XMapPos, walker.YMapPos] = debugColor;
			}

			_tileData[walker.XMapPos, walker.YMapPos] = true;
		}

		/// <summary>
		///     Attempts to execute a Levy Flight for given walker
		/// </summary>
		/// <param name="showMovement">Visualize instantaneously or not</param>
		/// <param name="walker">Walker that attempts a levy flight</param>
		private void LevyFlight(bool showMovement, Walker walker)
		{
			//Only execute LF if were below the given chance
			if (!(Random.Range(0f, 1f) < _levyFlightChance)) return;

			//Randomly select a steplength up to the upper border, then repeat the last walk and move the walker accordingly
			int steps = Random.Range(1, _maxStepLength + 1);
			for (int i = 1; i < steps; i++)
			{
				walker.RepeatWalk();
				MoveWalker(showMovement, walker, _levyFlightColor);
			}
		}

		/// <summary>
		///     Let a walker attempt to spawn a room centered at its position
		/// </summary>
		/// <param name="walker">Walker that will spawn the room</param>
		/// <param name="showMovement">Whether or not to visualize the creation instantaneously</param>
		private void SpawnRoom(Walker walker, bool showMovement)
		{
			//Only spawn a room if were below the given chance
			if (!(Random.Range(0f, 1f) < _spawnRoomChance)) return;

			float diceRoll = Random.Range(0f, 1f);
			foreach (Room room in _rooms)
			{
				//if the diceRoll is higher than room probability, it is not the one to spawn (expecting a sorted list where index 0 goes from 0-x%)
				if (!(diceRoll < room.Probability)) continue;

				//Mark the tiles surrounding the walker as visited (room is centered around the walker)
				//Halved width/height are used for correct centering
				int halvedRoomWidth = room.Width / 2;
				int halvedRoomHeight = room.Height / 2;
				for (int x = -halvedRoomWidth; x < (room.Width / 2f % 2 == 0 ? halvedRoomWidth : halvedRoomWidth + 1); x++)
				{
					for (int y = -halvedRoomHeight; y < (room.Height / 2f % 2 == 0 ? halvedRoomHeight : halvedRoomHeight + 1); y++)
					{
						if (!walker.IsInBounds(x, y)) continue;

						if (showMovement)
						{
							if (_tiles[walker.XMapPos + x, walker.YMapPos + y].VisitTile())
								_tilesCarved++;
						}
						else
						{
							if (!_tileData[walker.XMapPos + x, walker.YMapPos + y])
								_tilesCarved++;
						}

						if (_showWhileGeneration)
							_tiles[walker.XMapPos + x, walker.YMapPos + y].SetTileColor(_roomColor);
						_debugColors[walker.XMapPos + x, walker.YMapPos + y] = _roomColor;

						_tileData[walker.XMapPos + x, walker.YMapPos + y] = true;
					}
				}
			}
		}

		/// <summary>
		///     Spawn a walker at a given position
		/// </summary>
		/// <param name="startX">X coordinate</param>
		/// <param name="startY">Y coordinate</param>
		/// <param name="isRoomie">Whether or not this walker may spawn rooms</param>
		private void SpawnWalker(int startX, int startY, bool isRoomie)
		{
			//Create new walker object and add it to list
			Walker walker = Instantiate(_walkerPrefab, transform);
			_walkers.Add(walker);
			_walkers[_walkers.Count - 1].Init(startX, startY, _width, _height, _stepScheme, isRoomie);

			//carve the tile underneath the new walker
			_tileData[startX, startY] = true;
			_tilesCarved++;

			//show at its position when visualizing
			if (_useVisualization && _allowVisualisation)
			{
				_tiles[startX, startY].VisitTile();
				walker.transform.localPosition = _tiles[startX, startY].transform.localPosition;
			}

			//toggle biasing towards previous direction
			if (_biasTowardsPreviousDirection)
				walker.ToggleBiasTowardsPreviousDirection(true, _previousDirectionChance, _previousDirectionChanceDecayRate);

			//debug colors
			if (_showWhileGeneration)
				_tiles[walker.XMapPos, walker.YMapPos].SetTileColor(_newWalkerColor);
			_debugColors[walker.XMapPos, walker.YMapPos] = _newWalkerColor;
		}

		#endregion

		#region Nested type: Room

		/// <summary>
		///     Data class to describe rooms on a 2D map. Implements IComparable: Sorts by probability (highest first)
		/// </summary>
		[Serializable]
		private class Room : IComparable
		{
			#region Public Fields

			public int Height;
			public float Probability;
			public int Width;

			#endregion

			#region Constructors

			public Room(int width, int height, float probability)
			{
				Width = width;
				Height = height;
				Probability = probability;
			}

			#endregion

			#region ${0} Members

			/// <summary>
			///     Compares two room objects and sorts them by their probability (highest first)
			/// </summary>
			/// <param name="obj">Room object to compare to</param>
			/// <returns>int - order</returns>
			public int CompareTo(object obj)
			{
				Room other = (Room) obj;
				if (other.Probability > Probability)
				{
					return 1;
				}

				if (other.Probability < Probability)
				{
					return -1;
				}

				return 0;
			}

			#endregion
		}

		#endregion
	}
}