using UnityEngine;

/// <summary>
/// Move 2D Camera around the scene
/// </summary>
public class CameraMovement2D : MonoBehaviour
{
	#region Serialize Fields

	[SerializeField] private float _zoomFactor;
	[SerializeField] private float _moveSpeed;

	#endregion

	#region Private Fields

	private Camera _cam;

	#endregion

	#region Unity methods

	private void Awake()
	{
		_cam = GetComponent<Camera>();
	}

	private void LateUpdate()
	{
		float zoom = Input.GetAxisRaw("Zoom") * _zoomFactor;
		float moveX = Input.GetAxisRaw("Horizontal") * _moveSpeed;
		float moveY = Input.GetAxisRaw("Vertical") * _moveSpeed;

		if (Input.GetButtonDown("CenterCam"))
			transform.position = new Vector3(0, 0, -10);

		if (zoom != 0)
		{
			_cam.orthographicSize += zoom;
			_cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, 5, 120);
		}

		if ((moveX != 0) || (moveY != 0))
			transform.Translate(moveX, moveY, 0);
	}

	#endregion
}