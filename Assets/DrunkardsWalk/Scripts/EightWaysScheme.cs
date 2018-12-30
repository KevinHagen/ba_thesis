using UnityEngine;

namespace DrunkardsWalk
{
	/// <summary>
	/// Implementation of IWalkScheme. Allows navigation into the following eight cardinal directions: north, south, east, west, north-east, south-east, north-west, south-west
	/// </summary>
	public class EightWaysScheme : MonoBehaviour, IWalkScheme
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
			//pseudo-random value to achieve an even distribution of all directions (p = 0.125)
			int rand = Random.Range(0, 8);

			Vector3 walkDirection;
			if (rand == 0)
			{
				walkDirection = Vector3.right;
			}
			else if (rand == 1)
			{
				walkDirection = Vector3.left;
			}
			else if (rand == 2)
			{
				walkDirection = Vector3.up;
			}
			else if (rand == 3)
			{
				walkDirection = Vector3.down;
			}
			else if (rand == 4)
			{
				walkDirection = Vector3.right + Vector3.up;
			}
			else if (rand == 5)
			{
				walkDirection = Vector3.right + Vector3.down;
			}
			else if (rand == 6)
			{
				walkDirection = Vector3.left + Vector3.up;
			}
			else if (rand == 7)
			{
				walkDirection = Vector3.left + Vector3.down;
			}
			else
			{
				walkDirection = Vector3.zero;
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
			return previousDirection;	//no special handling necessary, previousDirection simply is the previousDirection.
		}

		#endregion
	}
}