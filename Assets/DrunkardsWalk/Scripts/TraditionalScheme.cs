using UnityEngine;

namespace DrunkardsWalk
{
	/// <summary>
	/// Implementation of IWalkScheme. Allows navigation into the four main cardinal directions: north, south, east, west
	/// </summary>
	public class TraditionalScheme : MonoBehaviour, IWalkScheme
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
			//pseudo-random value to achieve an even distribution of all directions (p = 0.25)
			int rand = Random.Range(0, 4);

			Vector3 walkDirection;
			switch (rand)
			{
				case 0:
					walkDirection = Vector3.right;
					break;
				case 1:
					walkDirection = Vector3.left;
					break;
				case 2:
					walkDirection = Vector3.up;
					break;
				case 3:
					walkDirection = Vector3.down;
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
			return previousDirection;
		}

		#endregion
	}
}