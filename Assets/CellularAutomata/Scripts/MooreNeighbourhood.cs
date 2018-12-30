using UnityEngine;

namespace CellularAutomata
{
	/*
	 * Spreads like this: stepRange =	1,	  2,	   3
	 *												3333333
	 *										22222	3222223
	 *									111	21112	3211123
	 *									101	21012	3210123
	 *									111	21112	3211123
	 *										22222	3222223
	 *												3333333
	 *
	 * NeighbourCount = 2^(2*s+1) - 1
	 * => the neighbours are ALL fields within (x,y) +/- stepRange
	 */
	public class MooreNeighbourhood : AbstractNeighbourhood
	{
		#region Properties

		/// <summary>
		/// Amount of all neighbours (no matter which state)
		/// </summary>
		//Count is equal to _stepRange * 2 + 1 (or in other words any odd number >= 3)
		//Subtract 1 afterwards because the centered cell is not considered a neighbour
		public override int NeighbourCount => (int) Mathf.Pow(_stepRange * 2 + 1, 2) - 1;
	

		#endregion

		#region Public methods

		/// <summary>
		/// Counts all living neighbours for a point (x, y) within the given neighbourhood with a radius set by SetRange(int range)
		/// </summary>
		/// <param name="xPos">x coordinate of the point</param>
		/// <param name="yPos">y coordinate of the point</param>
		/// <returns>int - livingNeighbourCount</returns>
		public override int GetLivingNeighboursCount(int xPos, int yPos)
		{
			int livingNeighbourCount = 0;

			//Moore neighbourhood counts all adjacent cells within an eightway pattern for radius _stepRange
			for (int neighbourX = xPos - _stepRange; neighbourX <= xPos + _stepRange; neighbourX++)
			{
				for (int neighbourY = yPos - _stepRange; neighbourY <= yPos + _stepRange; neighbourY++)
				{
					//Count all neighbours that are alive (true) and in bounds. Dont't count yourself. If a neighbour is out of bounds, pretend its alive and count it.
					if ((IsInBounds(neighbourX, neighbourY) && ((neighbourX != xPos) || (neighbourY != yPos)) && TileData[neighbourX, neighbourY]) || !IsInBounds(neighbourX, neighbourY))
					{
						livingNeighbourCount++;
					}
				}
			}

			return livingNeighbourCount;
		}

		/// <summary>
		/// Returns a Vector2-Array with x,y position of all neighbours for a point (xPos, yPos)
		/// </summary>
		/// <param name="xPos">x coordinate of the point</param>
		/// <param name="yPos">x coordinate of the point</param>
		/// <returns>Vector2[] - neighbourPositions</returns>
		public override Vector2[] GetNeighboursForPos(int xPos, int yPos)
		{
			//one vector2 foreach neighbour
			Vector2[] indices = new Vector2[NeighbourCount];
			int currentIndex = 0;
			//go through the whole map to get the real indices
			for (int x = 0; x < _widthBoundary; x++)
			{
				for (int y = 0; y < _heightBoundary; y++)
				{
					//Do not consider cells if their distance is greater than stepRange (because they're out of the neighbourhood's range)
					int absXPosDiff = Mathf.Abs(xPos - x);
					int absYPosDiff = Mathf.Abs(yPos - y);
					if ((absXPosDiff > _stepRange) || (absYPosDiff > _stepRange))
						continue;

					//add the found neighbour to the array
					if (IsInBounds(x, y) && ((x != xPos) || (y != yPos)))
					{
						indices[currentIndex] = new Vector2(x, y);
						currentIndex++;
					}
				}
			}

			return indices;
		}

		#endregion
	}
}