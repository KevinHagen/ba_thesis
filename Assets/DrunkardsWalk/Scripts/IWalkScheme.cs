using UnityEngine;

namespace DrunkardsWalk
{
	/// <summary>
	/// Interface used to create new walkschemes
	/// </summary>
	internal interface IWalkScheme
	{
		#region Public methods

		/// <summary>
		///     Execute the walk, starting at position mapX, mapY. Suggests to Walk into direction of return value.
		/// </summary>
		/// <param name="mapX">StartPos X</param>
		/// <param name="mapY">StartPos Y</param>
		/// <returns>Vector3 walkDirection</returns>
		Vector3 Walk(int mapX, int mapY);

		/// <summary>
		///     Walks towards the previously taken direction again in a 'logical' way. Usually just returning previousDirection
		///     again is sufficient
		///     but for some walkschemes (like hexagonal) a special handling is necessary.
		/// </summary>
		/// <param name="previousDirection">Previously taken walking direction</param>
		/// <param name="mapX">StartPos X</param>
		/// <param name="mapY">StartPos Y</param>
		/// <returns>Vector3 walkDirection</returns>
		Vector3 RepeatWalk(Vector3 previousDirection, int mapX, int mapY);

		#endregion
	}
}