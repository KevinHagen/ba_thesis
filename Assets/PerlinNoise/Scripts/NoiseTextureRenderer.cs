using UnityEngine;
using UnityEngine.UI;

namespace PerlinNoise
{
	/// <summary>
	/// Renders a given noise map to a texture
	/// </summary>
	public class NoiseTextureRenderer : MonoBehaviour
	{
		#region Serialize Fields

		[SerializeField] private Renderer _renderer;
		[SerializeField] private RawImage _image;

		#endregion

		#region Unity methods

		private void Awake()
		{
			_renderer = GetComponent<Renderer>();
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Draw a noise map onto a texture
		/// </summary>
		/// <param name="noiseMap">2D noise map to be drawn</param>
		public void DrawNoiseMap(float[,] noiseMap)
		{
			int width = noiseMap.GetLength(0);
			int height = noiseMap.GetLength(1);

			Texture2D texture = new Texture2D(width, height);

			Color[] pixelColors = new Color[width * height];
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					pixelColors[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
				}
			}

			texture.SetPixels(pixelColors);
			texture.Apply();

			if (_renderer)
				_renderer.sharedMaterial.mainTexture = texture;
			if (_image)
				_image.texture = texture;
		}

		#endregion
	}
}