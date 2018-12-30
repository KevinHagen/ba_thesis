using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace PerlinNoise
{
	/// <summary>
	///     UI to process user input to the perlin noise implementation in TerrainGenerator.cs
	/// </summary>
	public class PNMenu : BaseMenu
	{
		#region Serialize Fields

		[Header("Noise Parameter")] [SerializeField] [FormerlySerializedAs("_noiseScaleSlider")]
		public Slider NoiseScaleSlider;
		[SerializeField] [FormerlySerializedAs("_maxHeightSlider")]
		public Slider MaxHeightSlider;
		[SerializeField] public Slider OffsetXSlider;
		[SerializeField] public Slider OffsetYSlider;
		[SerializeField] public Toggle ScrollingToggle;
		[SerializeField] public Toggle SmoothTerrainToggle;
		[Header("Custom Noise Parameter")] [SerializeField]
		private Toggle _unityNoiseToggle;
		[SerializeField] private Toggle _customNoiseToggle;
		[SerializeField] private Toggle _cubicInterpolantToggle;
		[SerializeField] private Toggle _quinticInterpolantToggle;
		[SerializeField] private Toggle _randomVectorDistributionToggle;
		[SerializeField] private Toggle _predefinedVectorDistributionToggle;
		[Header("Octaves")] [SerializeField] [FormerlySerializedAs("_octavesSlider")]
		public Slider OctavesSlider;
		[SerializeField] [FormerlySerializedAs("_persistanceSlider")]
		public Slider PersistanceSlider;
		[SerializeField] [FormerlySerializedAs("_lacunaritySlider")]
		public Slider LacunaritySlider;
		[Header("Debug")] [SerializeField]
		private Toggle _showDebugTextureToggle;
		[SerializeField] private RawImage _debugTexture;

		#endregion

		#region Private Fields

		private TextMeshProUGUI _noiseScaleOutputText;
		private TextMeshProUGUI _maxHeightOutputText;
		private TextMeshProUGUI _octavesOutputText;
		private TextMeshProUGUI _persistanceOutputText;
		private TextMeshProUGUI _lacunarityOutputText;
		private TextMeshProUGUI _offsetYOutputText;
		private TextMeshProUGUI _offsetXOutputText;
		private float offsetX;
		private float offsetY;

		#endregion

		#region Properties

		public Vector2 OffsetVector => new Vector2(offsetX, offsetY);
		public float Lacunarity { get; private set; } = 4f;
		public float MaxHeight { get; private set; } = 7.0f;
		public float NoiseScale { get; private set; } = 17.68f;
		public int Octaves { get; private set; } = 1;
		public float Persistance { get; private set; } = 0.5f;
		public bool UseCubicInterpolant { get; private set; } = true;
		public bool UseRandomVectorDistribution { get; private set; }
		public bool UseUnityNoise { get; private set; } = true;
		public override bool UseVisualization => false;

		#endregion

		#region Unity methods

		protected override void Awake()
		{
			//toggle listeners
			_unityNoiseToggle.onValueChanged.AddListener(b =>
			                                             {
				                                             ToggleCustomNoiseParams(false);
				                                             UseUnityNoise = b;
				                                             _customNoiseToggle.isOn = !b;
			                                             });
			_customNoiseToggle.onValueChanged.AddListener(b =>
			                                              {
				                                              ToggleCustomNoiseParams(true);
				                                              UseUnityNoise = !b;
				                                              _unityNoiseToggle.isOn = !b;
			                                              });
			_cubicInterpolantToggle.onValueChanged.AddListener(b =>
			                                                   {
				                                                   UseCubicInterpolant = b;
				                                                   _quinticInterpolantToggle.isOn = !b;
			                                                   });
			_quinticInterpolantToggle.onValueChanged.AddListener(b =>
			                                                     {
				                                                     UseCubicInterpolant = !b;
				                                                     _cubicInterpolantToggle.isOn = !b;
			                                                     });
			_randomVectorDistributionToggle.onValueChanged.AddListener(b =>
			                                                           {
				                                                           UseRandomVectorDistribution = b;
				                                                           _predefinedVectorDistributionToggle.isOn = !b;
			                                                           });
			_predefinedVectorDistributionToggle.onValueChanged.AddListener(b =>
			                                                               {
				                                                               UseRandomVectorDistribution = !b;
				                                                               _randomVectorDistributionToggle.isOn = !b;
			                                                               });
			_showDebugTextureToggle.onValueChanged.AddListener(b => _debugTexture.gameObject.SetActive(b));

			//Slider listeners
			PersistanceSlider.onValueChanged.AddListener(value =>
			                                             {
				                                             Persistance = value;
				                                             _persistanceOutputText.text = value.ToString();
			                                             });
			LacunaritySlider.onValueChanged.AddListener(value =>
			                                            {
				                                            Lacunarity = value;
				                                            _lacunarityOutputText.text = value.ToString();
			                                            });
			NoiseScaleSlider.onValueChanged.AddListener(value =>
			                                            {
				                                            NoiseScale = value;
				                                            _noiseScaleOutputText.text = value.ToString();
			                                            });
			OctavesSlider.onValueChanged.AddListener(value =>
			                                         {
				                                         Octaves = (int) value;
				                                         _octavesOutputText.text = value.ToString();
			                                         });
			MaxHeightSlider.onValueChanged.AddListener(value =>
			                                           {
				                                           MaxHeight = value;
				                                           _maxHeightOutputText.text = value.ToString();
			                                           });
			OffsetXSlider.onValueChanged.AddListener(value =>
			                                         {
				                                         offsetX = value;
				                                         _offsetXOutputText.text = offsetX.ToString();
			                                         });
			OffsetYSlider.onValueChanged.AddListener(value =>
			                                         {
				                                         offsetY = value;
				                                         _offsetYOutputText.text = offsetY.ToString();
			                                         });

			//output texts
			_maxHeightOutputText = FindOutputTextForSlider(MaxHeightSlider);
			_noiseScaleOutputText = FindOutputTextForSlider(NoiseScaleSlider);
			_octavesOutputText = FindOutputTextForSlider(OctavesSlider);
			_persistanceOutputText = FindOutputTextForSlider(PersistanceSlider);
			_lacunarityOutputText = FindOutputTextForSlider(LacunaritySlider);
			_offsetXOutputText = FindOutputTextForSlider(OffsetXSlider);
			_offsetYOutputText = FindOutputTextForSlider(OffsetYSlider);

			base.Awake();
		}

		#endregion

		#region Protected methods

		protected override void SetDefaultGUIValues()
		{
			base.SetDefaultGUIValues();

			_unityNoiseToggle.isOn = UseUnityNoise;
			_customNoiseToggle.isOn = !UseUnityNoise;
			_cubicInterpolantToggle.isOn = UseCubicInterpolant;
			_quinticInterpolantToggle.isOn = !UseCubicInterpolant;
			_randomVectorDistributionToggle.isOn = UseRandomVectorDistribution;
			_predefinedVectorDistributionToggle.isOn = !UseRandomVectorDistribution;
			ToggleCustomNoiseParams(!UseUnityNoise);

			PersistanceSlider.value = Persistance;
			LacunaritySlider.value = Lacunarity;
			NoiseScaleSlider.value = NoiseScale;
			MaxHeightSlider.value = MaxHeight;
			OctavesSlider.value = Octaves;
			OffsetXSlider.value = offsetX;
			OffsetYSlider.value = offsetY;

			_octavesOutputText.text = Octaves.ToString();
			_maxHeightOutputText.text = MaxHeight.ToString();
			_persistanceOutputText.text = Persistance.ToString();
			_lacunarityOutputText.text = Lacunarity.ToString();
			_noiseScaleOutputText.text = NoiseScale.ToString();
			_offsetXOutputText.text = offsetX.ToString();
			_offsetYOutputText.text = offsetY.ToString();
		}

		#endregion

		#region Private methods

		//Toggle show parameters that are only configurable in the custom noise implementation (switchting vector distribution and interpolant)
		private void ToggleCustomNoiseParams(bool on)
		{
			_cubicInterpolantToggle.transform.parent.parent.gameObject.SetActive(on);
			_randomVectorDistributionToggle.transform.parent.parent.gameObject.SetActive(on);
		}

		#endregion
	}
}