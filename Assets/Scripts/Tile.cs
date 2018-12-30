using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 2D Map-Tile with a SpriteRenderer attached
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
	#region Serialize Fields

	[SerializeField] private SpriteRenderer _renderer;
	[SerializeField] private bool _isInUI;

	#endregion

	private Image _image;

	private void Awake()
	{
		//Fallback if a sprite renderer is not already set @ compiletime
		if (_renderer == null)
			_renderer = GetComponent<SpriteRenderer>();

		//For UI visualizations use Image-Component instead of SpriteRenderer
		if (_isInUI)
			_image = GetComponent<Image>();
	}

	#region Public methods

	/// <summary>
	/// Visits a specific tile (used for RandomWalks)
	/// </summary>
	/// <returns>bool - whether or not the tile was visited before</returns>
	public bool VisitTile()
	{
		if (_renderer.color.Equals(Color.black))
		{
			_renderer.color = Color.white;
			return true;
		}

		return false;
	}

	/// <summary>
	/// Set the SpriteRenderers Color to color
	/// </summary>
	/// <param name="color">Is the new Color of the attached SR</param>
	public void SetTileColor(Color color)
	{
		if (!_isInUI)
			_renderer.color = color;
		else
			_image.color = color;
	}

	#endregion
}