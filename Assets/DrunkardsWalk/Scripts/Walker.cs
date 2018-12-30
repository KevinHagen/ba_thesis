using UnityEngine;

namespace DrunkardsWalk
{
	/// <summary>
	/// Walker object that executes one digging step, whenever Walk() or RepeatWalk() are called
	/// </summary>
	public class Walker : MonoBehaviour
	{
		#region Private Fields

		private float _width;
		private float _height;
		private IWalkScheme _walkScheme;
		private bool _biasTowardsPreviousDirection;
		private float _previousDirectionChance;
		private float _previousDirectionChanceDecayRate;
		private float _currentPreviousDirectionChance;
		private Vector3 _previousDirection;

		#endregion

		#region Properties

		public bool IsRepeatingDirection { get; private set; }
		public bool IsRoomie { get; private set; }
		public int XMapPos { get; private set; }
		public int YMapPos { get; private set; }

		#endregion

		#region Public methods

		/// <summary>
		///     Init a new Walker
		/// </summary>
		/// <param name="xMapPos">Start x-Pos</param>
		/// <param name="yMapPos">Start y-Pos</param>
		/// <param name="width">Map width</param>
		/// <param name="height">Map height</param>
		/// <param name="stepScheme">StepScheme for the walkers movement</param>
		/// <param name="isRoomie">Can this walker spawn rooms?</param>
		public void Init(int xMapPos, int yMapPos, int width, int height, DrunkardsWalk.StepScheme stepScheme, bool isRoomie)
		{
			_width = width;
			_height = height;
			XMapPos = xMapPos;
			YMapPos = yMapPos;
			IsRoomie = isRoomie;

			//Add the corresponding component
			switch (stepScheme)
			{
				case DrunkardsWalk.StepScheme.Traditional:
					_walkScheme = gameObject.AddComponent<TraditionalScheme>();
					break;
				case DrunkardsWalk.StepScheme.EightWays:
					_walkScheme = gameObject.AddComponent<EightWaysScheme>();
					break;
				case DrunkardsWalk.StepScheme.Hexagonal:
					_walkScheme = gameObject.AddComponent<HexagonalScheme>();
					break;
			}
		}

		/// <summary>
		///     Execute the Walk
		/// </summary>
		public void Walk()
		{
			Vector3 walkDirection;

			//If biasing is enabled, use the last direction eventually. If so, decay the chance by the given rate
			if (_biasTowardsPreviousDirection && (Random.Range(0f, 1f) < _currentPreviousDirectionChance))
			{
				RepeatWalk();
				_currentPreviousDirectionChance *= 1 - _previousDirectionChanceDecayRate;
				IsRepeatingDirection = true;
				return;
			}

			//Pick a new movement direction until it won't go out of bounds (map)
			do
			{
				walkDirection = _walkScheme.Walk(XMapPos, YMapPos);
			} while (!IsInBounds(walkDirection));

			//Save as previousDirection, reset biasing chance and update position
			_previousDirection = walkDirection;
			_currentPreviousDirectionChance = _previousDirectionChance;
			IsRepeatingDirection = false;
			XMapPos += (int) walkDirection.x;
			YMapPos += (int) walkDirection.y;
		}

		/// <summary>
		///     Walk towards the same direction as the previousDirection
		/// </summary>
		public void RepeatWalk()
		{
			Vector3 walkDirection = _walkScheme.RepeatWalk(_previousDirection, XMapPos, YMapPos);

			if (!IsInBounds(walkDirection)) return;

			XMapPos += (int) walkDirection.x;
			YMapPos += (int) walkDirection.y;
			_previousDirection = walkDirection;
		}

		/// <summary>
		///     Should the walker bias its movement direction towards the previous one?
		/// </summary>
		/// <param name="on">Enable/Disable biasing</param>
		/// <param name="previousDirectionChance">Chance to continue towards previousDirection</param>
		/// <param name="decayRate">The rate at which the previousDirectionChance decays</param>
		public void ToggleBiasTowardsPreviousDirection(bool on, float previousDirectionChance, float decayRate)
		{
			_biasTowardsPreviousDirection = on;
			_previousDirectionChance = previousDirectionChance;
			_previousDirectionChanceDecayRate = decayRate;
		}

		/// <summary>
		///     Is moving into the given direction safe or will it go out of bounds?
		/// </summary>
		/// <param name="xDir">Walk Direction X</param>
		/// <param name="yDir">Walk Direction Y</param>
		/// <returns>bool - movement safe?</returns>
		public bool IsInBounds(float xDir, float yDir)
		{
			return (XMapPos + xDir >= 0) && (XMapPos + xDir < _width) && (YMapPos + yDir >= 0) && (YMapPos + yDir < _height);
		}

		#endregion

		#region Private methods

		/// <summary>
		///     Is moving into the given direction safe or will it go out of bounds?
		/// </summary>
		/// <param name="dir">Walk Direction</param>
		/// <returns>bool - movement safe?</returns>
		private bool IsInBounds(Vector3 dir)
		{
			return (XMapPos + dir.x >= 0) && (XMapPos + dir.x < _width) && (YMapPos + dir.y >= 0) && (YMapPos + dir.y < _height);
		}

		#endregion
	}
}