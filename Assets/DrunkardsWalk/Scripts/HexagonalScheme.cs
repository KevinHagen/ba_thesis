using UnityEngine;

namespace DrunkardsWalk
{
	/// <summary>
	/// Implementation of IWalkScheme. Allows navigation into the following six cardinal directions: north, south, north-east, south-east, north-west, south-west 
	/// </summary>
	public class HexagonalScheme : MonoBehaviour, IWalkScheme
	{
		#region ${0} Members

		/// <summary>
		///     Execute the walk, starting at position mapX, mapY. Suggests to Walk into direction of return value.
		/// </summary>
		/// <param name="mapX">StartPos X</param>
		/// <param name="mapY">StartPos Y</param>
		/// <returns>Vector3 walkDirection</returns>
		public Vector3 Walk(int mapX, int mapY)
		{
			//pseudo-random value to achieve an even distribution of all directions (p = 1/6)
			int rand = Random.Range(0, 6);
			bool isOnEvenColumn = mapX % 2 == 0;
			Vector3 walkDirection;
			//in order to walk correctly on a hexagonal map, we need to accomodate the circumstances of offset columns with an odd index. 
			//to do so, the diagonal walk directions are adjusted accordingly for even/odd columns
			switch (rand)
			{
				case 0:
					walkDirection = isOnEvenColumn ? Vector3.left + Vector3.down : Vector3.left; //Lower Left
					break;
				case 1:
					walkDirection = isOnEvenColumn ? Vector3.left : Vector3.left + Vector3.up; //Upper Left
					break;
				case 2:
					walkDirection = isOnEvenColumn ? Vector3.right + Vector3.down : Vector3.right; //Lower right
					break;
				case 3:
					walkDirection = isOnEvenColumn ? Vector3.right : Vector3.right + Vector3.up; //Upper right
					break;
				case 4:
					walkDirection = Vector3.down; //down
					break;
				case 5:
					walkDirection = Vector3.up; //up
					break;
				default:
					walkDirection = Vector3.zero;
					break;
			}

			return walkDirection;
		}

		/// <summary>
		///     Walks towards the previously taken direction again in a 'logical' way. Usually just returning previousDirection
		///     again is sufficient
		///     but for some walkschemes (like hexagonal) a special handling is necessary.
		/// </summary>
		/// <param name="previousDirection">Previously taken walking direction</param>
		/// <param name="mapX">StartPos X</param>
		/// <param name="mapY">StartPos Y</param>
		/// <returns>Vector3 walkDirection</returns>
		public Vector3 RepeatWalk(Vector3 previousDirection, int mapX, int mapY)
		{
			bool isOnEvenColumn = mapX % 2 == 0;

			//walking into the same (diagonal) direction on a hexagonal map needs to adjust the previousDirection.y value
			//since odd/even columns on the hex-grid have different directional vectors foreach diagonal direction (e.g. upper left)
			if (previousDirection.x == -1 && previousDirection.y == 1 && isOnEvenColumn)
				previousDirection.y = 0;
			else if (previousDirection.x == -1 && previousDirection.y == -1 && !isOnEvenColumn)
				previousDirection.y = 0;
			else if (previousDirection.x == 1 && previousDirection.y == 1 && isOnEvenColumn)
				previousDirection.y = 0;
			else if (previousDirection.x == 1 && previousDirection.y == -1 && !isOnEvenColumn)
				previousDirection.y = 0;

			return previousDirection;
		}

		#endregion
	}
}