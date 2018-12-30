using System.Collections.Generic;
using UnityEngine;

namespace CellularAutomata
{
	/// <summary>
	///     Visualizes a given 2D neighbouhood on a 11x11 field of cells. (meaning it displays a maximum radius of 5)
	/// </summary>
	public class NeighbourhoodVisualizationUI : MonoBehaviour
	{
		#region Serialize Fields

		[SerializeField] private Color _defaultColor;
		[SerializeField] private Color _centerColor;
		[SerializeField] private Color _neighbourColor;

		#endregion

		#region Private Fields

		private Tile[] _neighbourhoodVisualisationTiles;
		private int _centerX;
		private int _centerY;

		#endregion

		#region Properties

		public List<AbstractNeighbourhood> Neighbourhoods { get; private set; }

		#endregion

		#region Unity methods

		private void Awake()
		{
			//Given neighbourhoods in the example. Feel free to add any. 
			Neighbourhoods = new List<AbstractNeighbourhood>
			                 {
				                 new MooreNeighbourhood(),
				                 new VonNeumannNeighbourhood()
			                 };
			//Set boundaries foreach neighbourhood to visualize correctly
			foreach (AbstractNeighbourhood neighbourhood in Neighbourhoods)
			{
				neighbourhood.UpdateBoundaries(11, 11);
			}

			_neighbourhoodVisualisationTiles = GetComponentsInChildren<Tile>();
			_centerX = 5;
			_centerY = 5;
		}

		private void Start()
		{
			//show moore neighbourhood with range 1 by default
			UpdateVis(1, 0);
		}

		#endregion

		#region Public methods

		/// <summary>
		///     Updates the cell field when radius/neighbourhood changes
		/// </summary>
		/// <param name="neighbourhoodRange">New Radius for the neighbourhood</param>
		/// <param name="neighbourhoodIndex">New neighbourhood</param>
		public void UpdateVis(int neighbourhoodRange, int neighbourhoodIndex)
		{
			foreach (Tile neighbourTile in _neighbourhoodVisualisationTiles)
			{
				neighbourTile.SetTileColor(_defaultColor);
			}

			//center tile has a special color
			_neighbourhoodVisualisationTiles[60].SetTileColor(_centerColor);

			//update range and neighbourhood, then paint the neighbours
			Neighbourhoods[neighbourhoodIndex].SetRange(neighbourhoodRange);
			Vector2[] indices = Neighbourhoods[neighbourhoodIndex].GetNeighboursForPos(_centerX, _centerY);
			foreach (Vector2 indexVector in indices)
			{
				int x = (int) indexVector.x;
				int y = (int) indexVector.y;
				_neighbourhoodVisualisationTiles[y * 11 + x].SetTileColor(_neighbourColor);
			}
		}

		#endregion
	}
}