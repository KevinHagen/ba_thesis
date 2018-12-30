using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace PerlinNoise
{
	/// <summary>
	///     Generates Terrain based on 2D Perlin Noise
	/// </summary>
	[RequireComponent(typeof(MeshFilter))]
	public class TerrainGenerator : BaseGenerator
	{
		#region Serialize Fields

		[Header("Noise Parameters")] [SerializeField]
		private float _noiseScale;
		[SerializeField] private Vector2 _offset;
		[SerializeField] private float _scrollSpeed;
		[Header("For Octaves")] [SerializeField]
		private int _octaves;
		[SerializeField] [Range(0f, 1f)]
		private float _persistance;
		[SerializeField] private float _lacunarity;
		[Header("Noise Setup")] [SerializeField] [Tooltip("Custom Noise or Unitys Impl?")]
		private bool _useUnityStandardNoise;
		[SerializeField] [Tooltip("Use random G or predefined 16 Vectors?")]
		private bool _useRandomVectorDistribution;
		[SerializeField] [Tooltip("Cubic or Quintic Interpolant?")]
		private bool _useCubicInterpolant;
		[SerializeField] private float _maxHeight;
		[SerializeField] [FormerlySerializedAs("_heightSmoothingCurve")]
		private AnimationCurve _terrainSmoothingCurve;
		[Header("Debug")] [SerializeField]
		private NoiseTextureRenderer _noiseDisplay;
		[SerializeField] private MeshFilter _filter;
		[SerializeField] private bool _drawGizmos;
		[SerializeField] private bool _autoUpdate;

		#endregion

		#region Private Fields

		private Mesh _mesh;
		private float _offsetX;
		//private static float[][,] _generationSteps;
		private float[,] _noiseMap;
		private int[] _triangles;
		private Vector3[] _vertices;
		private bool _enableScrolling;
		private bool _applyTerrainSmooting = true;
		private float _offsetY;

		#endregion

		#region Properties

		public bool AutoUpdate => _autoUpdate;

		#endregion

		#region Unity methods

		protected override void Awake()
		{
			base.Awake();
			_filter = GetComponent<MeshFilter>();

			PNMenu menu = UI as PNMenu;
			menu.NoiseScaleSlider.onValueChanged.AddListener(value => Generate());
			menu.MaxHeightSlider.onValueChanged.AddListener(value => Generate());
			menu.LacunaritySlider.onValueChanged.AddListener(value => Generate());
			menu.OctavesSlider.onValueChanged.AddListener(value => Generate());
			menu.PersistanceSlider.onValueChanged.AddListener(value => Generate());
			menu.OffsetXSlider.onValueChanged.AddListener(value => Generate());
			menu.OffsetYSlider.onValueChanged.AddListener(value => Generate());

			//menu.ShowStepSlider.onValueChanged.AddListener(value => ShowStep((int) value));
			menu.ScrollingToggle.onValueChanged.AddListener(b => _enableScrolling = b);
			menu.SmoothTerrainToggle.onValueChanged.AddListener(b =>
			                                                    {
				                                                    _applyTerrainSmooting = b;
				                                                    Generate();
			                                                    });
		}

		private void Update()
		{
			if (_enableScrolling)
			{
				_offset = new Vector2(_offset.x + Time.deltaTime * _scrollSpeed, _offset.y);
				Generate();
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		///     Generate the map and its data
		/// </summary>
		public override void Generate()
		{
			base.Generate();

			//if we may not visualize or dont want to, do the generation instantly
			if (!_useVisualization || !_allowVisualisation)
			{
				_noiseMap = GenerateNoiseMap();
				_noiseDisplay.DrawNoiseMap(_noiseMap);
				ApplyNoiseMap();
			}
		}

		/// <summary>
		///     Clears the generated data
		/// </summary>
		public override void Clear()
		{
			if (_mesh)
				_mesh.Clear();
		}

		#endregion

		#region Protected methods

		/// <summary>
		///     Retrieves the generator specific data from the ui
		/// </summary>
		protected override void GetGeneratorSpecificData()
		{
			PNMenu menu = UI as PNMenu;
			if (!_enableScrolling)
				_offset = menu.OffsetVector;
			_noiseScale = menu.NoiseScale;
			_useCubicInterpolant = menu.UseCubicInterpolant;
			_useRandomVectorDistribution = menu.UseRandomVectorDistribution;
			_useUnityStandardNoise = menu.UseUnityNoise;
			_octaves = menu.Octaves;
			_persistance = menu.Persistance;
			_lacunarity = menu.Lacunarity;
			_maxHeight = menu.MaxHeight;
		}

		/// <summary>
		///     Instantiate map from the generated data
		/// </summary>
		protected override void InstantiateMap()
		{
			UpdateMesh();
		}

		//Moving the "Octaves"-Slider is essentially the step by step generation so this is not necessary
		protected override IEnumerator StepByStepGeneration()
		{
			yield return null;
			////steps are the octaves applied one after another
			//for (int i = 0; i < _octaves; i++)
			//{
			//	yield return new WaitForSeconds(_timeBetweenSteps);
			//	ShowStep(_currentOctave);
			//	_currentOctave++;
			//}
		}

		/// <summary>
		///     Sets up all data prior to the actual generation
		/// </summary>
		protected override void PrepareInitialState()
		{
			//init mesh data
			_mesh = new Mesh();
			_filter.mesh = _mesh;
			_offsetX = (_width - 1) / -2f;
			_offsetY = (_height - 1) / 2f;

			////Save octaves as different generation steps to display them later
			//_generationSteps = new float[_octaves + 1][,];
			//for (int i = 0; i < _generationSteps.Length; i++)
			//{
			//	_generationSteps[i] = new float[_width, _height];
			//}

			//generate the basic ground plane
			GenerateGroundPlane();
			UpdateMesh();
		}

		//Grid is centered in generation phase, so do nothing for centering here
		protected override void CenterGrid()
		{
		}

		#endregion

		#region Private methods

		///// <summary>
		/////     Show terrain at a specific step from the generation process
		///// </summary>
		///// <param name="step">Display generated data at timestep step</param>
		//private void ShowStep(int step)
		//{
		//	if ((_noiseMap == null) || (_noiseMap[0, 0] == 0f) || (step > _octaves))
		//		return;

		//	for (int x = 0; x < _width; x++)
		//	{
		//		for (int y = 0; y < _height; y++)
		//		{
		//			_noiseMap[x, y] = _generationSteps[step][x, y];
		//		}
		//	}

		//	_currentOctave = step;
		//	ApplyNoiseMap();
		//}

		/// <summary>
		///     Changes the z-coordinate of each vertex in the ground plane to use a value from a previously generated noisemap
		/// </summary>
		private void ApplyNoiseMap()
		{
			int index = 0;
			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					float xVal = _vertices[index].x;
					float yVal = _vertices[index].z;
					float zVal = (_applyTerrainSmooting ? _terrainSmoothingCurve.Evaluate(_noiseMap[x, y]) : _noiseMap[x, y]) * _maxHeight;
					_vertices[index] = new Vector3(xVal, zVal, yVal);
					index++;
				}
			}

			UpdateMesh();
		}

		//TODO Global Normalization
		/// <summary>
		///     Generates a perlin noise-based two dimensional noise map.
		/// </summary>
		/// <returns>float[,] - noiseMap</returns>
		private float[,] GenerateNoiseMap()
		{
			float[,] noiseMap = new float[_width, _height];

			//float maxPossibleHeight = 0f;
			//float amplitude = 1f;
			Vector2[] octaveOffsets = new Vector2[_octaves];
			for (int i = 0; i < _octaves; i++)
			{
				//Pick a random area on the noisemap for generation so it is everchanging
				float offsetX = Random.Range(-100000, 100000) + _offset.x;
				float offsetY = Random.Range(-100000, 100000) + _offset.y;
				octaveOffsets[i] = new Vector2(offsetX, offsetY);

				//maxPossibleHeight += amplitude;
				//amplitude *= _persistance;
			}

			//for local normalizing
			float maxNoiseHeight = float.MinValue;
			float minNoiseHeight = float.MaxValue;

			if (_noiseScale <= 0)
				_noiseScale = 0.001f;

			//zoom into center
			float halfWidth = _width / 2f;
			float halfHeight = _height / 2f;

			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					float noiseHeight;
					float sampleX = (x - halfWidth) / _noiseScale;
					float sampleY = (y - halfHeight) / _noiseScale;

					//Unity/Custom Implementation?
					if (_useUnityStandardNoise)
						noiseHeight = ApplyOctavesUnityPN(sampleX, sampleY, octaveOffsets);
					else if (!_useUnityStandardNoise && (_octaves > 1))
						noiseHeight = PerlinNoise.WithOctaves2D(sampleX, sampleY, _octaves, _lacunarity, _persistance, octaveOffsets, _useCubicInterpolant, _useRandomVectorDistribution);
					else
						noiseHeight = PerlinNoise.In2D(sampleX, sampleY, _useCubicInterpolant, _useRandomVectorDistribution);

					if (noiseHeight > maxNoiseHeight)
						maxNoiseHeight = noiseHeight;
					else if (noiseHeight < minNoiseHeight)
						minNoiseHeight = noiseHeight;

					noiseMap[x, y] = noiseHeight;

					//float normalizedHeight = (noiseMap[x, y] + 1) / 2 / (maxPossibleHeight / 0.9f);
					//noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
				}
			}

			//Normalize 
			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
				}
			}

			return noiseMap;
		}

		/// <summary>
		///     Returns a noise value processed through at least one octave.
		///     Using only one octave effectively does not differ from not using octaves.
		/// </summary>
		/// <param name="sampleX">X coordinate of the sample point</param>
		/// <param name="sampleY">Y coordinate of the sample point</param>
		/// <param name="octaveOffsets">Offsetvector to get a random area from the map</param>
		/// <returns>A noise value between -1 and 1</returns>
		private float ApplyOctavesUnityPN(float sampleX, float sampleY, Vector2[] octaveOffsets)
		{
			float frequency = 1f;
			float amplitude = 1f;
			float noiseHeight = 0f;

			for (int i = 0; i < _octaves; i++)
			{
				float x = (sampleX + octaveOffsets[i].x) * frequency;
				float y = (sampleY + octaveOffsets[i].y) * frequency;

				float noiseValue = Mathf.PerlinNoise(x, y) * 2 - 1; //Map from -1 to 1 so the terrain can rise and fall
				noiseHeight += noiseValue * amplitude;

				amplitude *= _persistance;
				frequency *= _lacunarity;
			}

			return noiseHeight;
		}

		#endregion

		//private int _currentOctave;

		#region Mesh Generation

		/// <summary>
		///     Generates a flat plane object in dimensions _width * _height
		/// </summary>
		private void GenerateGroundPlane()
		{
			_vertices = new Vector3[_width * _height];
			_triangles = new int[(_width - 1) * (_height - 1) * 6];

			int vertIndex = 0;
			int triangleIndex = 0;
			for (int y = 0; y < _height; y++)
			{
				for (int x = 0; x < _width; x++)
				{
					_vertices[vertIndex] = new Vector3(_offsetX + x, 0, _offsetY - y);

					if ((x < _width - 1) && (y < _height - 1))
					{
						_triangles[triangleIndex + 0] = vertIndex + 0;
						_triangles[triangleIndex + 1] = vertIndex + 1 + _width;
						_triangles[triangleIndex + 2] = vertIndex + 0 + _width;
						_triangles[triangleIndex + 3] = vertIndex + 1 + _width;
						_triangles[triangleIndex + 4] = vertIndex + 0;
						_triangles[triangleIndex + 5] = vertIndex + 1;
						triangleIndex += 6;
					}

					vertIndex++;
				}
			}
		}

		/// <summary>
		///     Update the Mesh data
		/// </summary>
		private void UpdateMesh()
		{
			_mesh.Clear();
			_mesh.vertices = _vertices;
			_mesh.triangles = _triangles;
			_mesh.RecalculateNormals();
		}

		private void OnDrawGizmos()
		{
			if ((_vertices == null) || !_drawGizmos)
				return;

			foreach (Vector3 v in _vertices)
			{
				Gizmos.DrawSphere(v, .1f);
			}
		}

		#endregion
	}
}