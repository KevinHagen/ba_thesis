using UnityEngine;

/// <summary>
/// Move 3D Camera around the scene
/// </summary>
public class CameraMovement3D : MonoBehaviour
{
	#region Static Stuff

	private static Vector3 ClampMagnitude(Vector3 v, float max, float min)
	{
		double sm = v.sqrMagnitude;
		if (sm > max * (double) max) return v.normalized * max;
		if (sm < min * (double) min) return v.normalized * min;
		return v;
	}

	#endregion

	#region Serialize Fields

	[SerializeField] private float _rotationSpeed;
	[SerializeField] private int _degreePerStep;
	[SerializeField] private float _zoomFactor;
	[SerializeField] private float _minDistance;
	[SerializeField] private float _maxDistance;

	#endregion

	#region Unity methods

	private void LateUpdate()
	{
		if (Input.GetKey(KeyCode.A))
		{
			RotateCameraAroundWorldCenter(Vector3.up, true);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			RotateCameraAroundWorldCenter(Vector3.up, false);
		}
		else if (Input.GetKey(KeyCode.W))
		{
			RotateCameraAroundWorldCenter(Vector3.right, true);
		}
		else if (Input.GetKey(KeyCode.S))
		{
			RotateCameraAroundWorldCenter(Vector3.right, false);
		}

		float zoom = Input.GetAxisRaw("Zoom");
		if (zoom != 0)
		{
			Vector3 movementDir = transform.position.normalized;
			transform.position += -1 * movementDir * zoom * _zoomFactor * Time.deltaTime;
			transform.position = ClampMagnitude(transform.position, _maxDistance, _minDistance);
		}

		transform.LookAt(Vector3.zero);
	}

	#endregion

	#region Private methods

	private void RotateCameraAroundWorldCenter(Vector3 axis, bool positive)
	{
		transform.RotateAround(Vector3.zero, positive ? axis : -1 * axis, _degreePerStep * _rotationSpeed * Time.deltaTime);
	}

	#endregion
}