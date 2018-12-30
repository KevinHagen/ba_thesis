using UnityEngine;

namespace CellularAutomata
{
	/// <summary>
	/// Base class to construct different neighbourhood rules used in a cellular automaton. It is designed to work with any kind of 2D CA
	/// </summary>
	public abstract class AbstractNeighbourhood
	{
		#region Protected Fields

		protected int _heightBoundary;
		protected int _stepRange;
		protected int _widthBoundary;

		#endregion

		#region Properties

		/// <summary>
		/// Amount of all neighbours (no matter which state)
		/// </summary>
		public abstract int NeighbourCount { get; }
		/// <summary>
		/// Holds the current state for point (x, y)
		/// </summary>
		public bool[,] TileData { get; set; }

		#endregion

		#region Public methods

		/// <summary>
		/// Update map boundaries
		/// </summary>
		/// <param name="width">New width</param>
		/// <param name="height">New height</param>
		public void UpdateBoundaries(int width, int height)
		{
			_widthBoundary = width;
			_heightBoundary = height;
		}

		/// <summary>
		/// Set the radius of the neighbourhood
		/// </summary>
		/// <param name="range">New radius</param>
		public void SetRange(int range)
		{
			_stepRange = range;
		}

		/// <summary>
		/// Counts all living neighbours for a point (x, y) within the given neighbourhood with a radius set by SetRange(int range)
		/// </summary>
		/// <param name="xPos">x coordinate of the point</param>
		/// <param name="yPos">y coordinate of the point</param>
		/// <returns>int - livingNeighbourCount</returns>
		public abstract int GetLivingNeighboursCount(int xPos, int yPos);
		/// <summary>
		/// Returns a Vector2-Array with x,y position of all neighbours for a point (xPos, yPos)
		/// </summary>
		/// <param name="xPos">x coordinate of the point</param>
		/// <param name="yPos">x coordinate of the point</param>
		/// <returns>Vector2[] - neighbourPositions</returns>
		public abstract Vector2[] GetNeighboursForPos(int xPos, int yPos);

		#endregion

		#region Protected methods

		/// <summary>
		/// Checks whether a neighbour with given X and Y pos is in bounds of the underlying 2d array
		/// </summary>
		/// <param name="neighbourX">Neighbours X Position</param>
		/// <param name="neighbourY">Neighbours Y Position</param>
		/// <returns>bool isInBounds</returns>
		protected bool IsInBounds(int neighbourX, int neighbourY)
		{
			return (neighbourX >= 0) && (neighbourX < _widthBoundary) && (neighbourY >= 0) && (neighbourY < _heightBoundary);
		}

		#endregion
	}
}