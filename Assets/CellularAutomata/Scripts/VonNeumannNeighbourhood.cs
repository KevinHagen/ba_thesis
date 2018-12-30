using UnityEngine;

namespace CellularAutomata
{
	/*
	 * Spreads like this: stepRange =	1,	  2,	   3
	 *												   3
	 *										  2		  323
	 *									 1	 212	 32123
	 *									101	21012	3210123
	 *									 1	 212	 32123
	 *										  2		  323
	 *												   3
	 *
	 * NeighbourCount = 4 * s_0 + 4 * s_1 + ... + 4 * s_n
	 * => all neighbours except those where the absolute distance of neighbour(x,y) to (x,y) is != stepRange
	 */
	public class VonNeumannNeighbourhood : AbstractNeighbourhood
	{
		#region Properties

		//Equal to the sum of 4 * s_0 + 4 * s_1 + ... + 4 * s_n, where n = _stepRange
		/// <summary>
		/// Amount of all neighbours (no matter which state)
		/// </summary>
		public override int NeighbourCount
		{
			get
			{
				int neighbourSum = 0;
				for (int i = 1; i <= _stepRange; i++)
				{
					neighbourSum += 4 * i;
				}

				return neighbourSum;
			}
		}

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

			//go through all adjacent cells, within x - _steprange to x + _stepRange and y - _stepRange to y + _stepRange 
			//effectively go through a _steprange * _steprange field patch where point (x, y) is centered
			for (int neighbourX = xPos - _stepRange; neighbourX <= xPos + _stepRange; neighbourX++)
			{
				for (int neighbourY = yPos - _stepRange; neighbourY <= yPos + _stepRange; neighbourY++)
				{
					//Do not consider cells if their distance is greater than stepRange (because they're out of the neighbourhood's range)
					int absXPosDiff = Mathf.Abs(xPos - neighbourX);
					int absYPosDiff = Mathf.Abs(yPos - neighbourY);
					int distance = absYPosDiff + absXPosDiff;
					if (distance > _stepRange)
						continue;

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
					int distance = absYPosDiff + absXPosDiff;
					if (distance > _stepRange)
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