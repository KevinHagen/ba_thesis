using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DrunkardsWalk
{
	/// <summary>
	///     UI used to allow user input for DrunkardsWalk-Algorithm
	/// </summary>
	public class DrunkardsMenu : BaseMenu
	{
		#region Serialize Fields

		[Header("Drunkards Walk")] [SerializeField]
		private TMP_Dropdown _stepSchemeDropdown;
		[SerializeField] private Slider _carveRateSlider;
		[SerializeField] private Toggle _startInCenterToggle;
		[SerializeField] public Toggle BiasTowardsPreviousToggle;
		[SerializeField] public Slider PreviousDirectionBiasSlider;
		[SerializeField] public Slider DecayRateSlider;
		[Header("Walker Settings")] [SerializeField]
		public Slider WalkerSpawnRateSlider;
		[SerializeField] public Slider MaxWalkerSlider;
		[SerializeField] public Slider RoomieChanceSlider;
		[Header("Levy Flight")] [SerializeField]
		public Toggle EnableLevyFlightToggle;
		[SerializeField] public Slider LevyFlightChanceSlider;
		[SerializeField] public Slider MaxStepLengthSlider;
		[Header("Rooms")] [SerializeField]
		private Button _createRoomButton;
		[SerializeField] private RoomEntryUI _roomEntryPrefab;
		[SerializeField] private RectTransform _roomConfigHolder;
		[SerializeField] private SliderGroup _sliderGroup;
		[SerializeField] public Slider SpawnRoomChanceSlider;

		#endregion

		#region Private Fields

		private int _roomCount;
		private RectTransform _content;
		private TextMeshProUGUI _carveRateOutputText;
		private TextMeshProUGUI _maxStepOutputText;
		private TextMeshProUGUI _previousDirectionOutputText;
		private TextMeshProUGUI _previousDirectionDecayOutputText;
		private TextMeshProUGUI _levyFlightChanceOutputText;
		private TextMeshProUGUI _spawnRoomChanceOutputText;
		private TextMeshProUGUI _walkerSpawnRateOutputText;
		private TextMeshProUGUI _roomieChanceOutputText;
		private TextMeshProUGUI _maxWalkerOutputText;

		#endregion

		#region Properties

		public bool BiasTowardsPrevious { get; private set; }
		public float CarveRate { get; private set; } = 0.456f;
		public float DecayRate { get; private set; } = 0.05f;
		public bool EnableLevyFlight { get; private set; }
		public float IsRoomieChance { get; private set; } = 0.2f;
		public float LevyFlightChance { get; private set; }
		public int MaxStepLength { get; private set; } = 1;
		public int MaxWalkerCount { get; private set; } = 10;
		public float PreviousDirectionBias { get; private set; } = 0.42f;
		public List<RoomEntryUI> RoomEntries { get; private set; }
		public float SpawnRoomChance { get; private set; } = 0.01f;
		public bool StartinCenter { get; private set; } = true;
		public DrunkardsWalk.StepScheme StepScheme { get; private set; } = DrunkardsWalk.StepScheme.Traditional;
		public float WalkerSpawnRate { get; private set; } = 0.05f;

		#endregion

		#region Unity methods

		protected override void Awake()
		{
			_stepSchemeDropdown.onValueChanged.AddListener(OnStepSchemeChanged);
			_carveRateSlider.onValueChanged.AddListener(OnCarveRateChanged);
			MaxStepLengthSlider.onValueChanged.AddListener(OnStepLengthChanged);
			LevyFlightChanceSlider.onValueChanged.AddListener(OnLevyChanceChanged);
			DecayRateSlider.onValueChanged.AddListener(OnDecayRateChanged);
			PreviousDirectionBiasSlider.onValueChanged.AddListener(OnPreviousDirectionBiasChanged);
			SpawnRoomChanceSlider.onValueChanged.AddListener(OnSpawnRoomChanceChanged);
			WalkerSpawnRateSlider.onValueChanged.AddListener(OnWalkerSpawnRateChanged);
			RoomieChanceSlider.onValueChanged.AddListener(OnRoomieChanceChanged);
			MaxWalkerSlider.onValueChanged.AddListener(OnMaxWalkersChanged);
			_startInCenterToggle.onValueChanged.AddListener(OnStartInCenterChanged);
			BiasTowardsPreviousToggle.onValueChanged.AddListener(OnBiasTowardsPreviousChanged);
			EnableLevyFlightToggle.onValueChanged.AddListener(OnToggleLevyFlight);
			_createRoomButton.onClick.AddListener(() => CreateNewRoom());

			_carveRateOutputText = FindOutputTextForSlider(_carveRateSlider);
			_maxStepOutputText = FindOutputTextForSlider(MaxStepLengthSlider);
			_previousDirectionOutputText = FindOutputTextForSlider(PreviousDirectionBiasSlider);
			_previousDirectionDecayOutputText = FindOutputTextForSlider(DecayRateSlider);
			_levyFlightChanceOutputText = FindOutputTextForSlider(LevyFlightChanceSlider);
			_spawnRoomChanceOutputText = FindOutputTextForSlider(SpawnRoomChanceSlider);
			_walkerSpawnRateOutputText = FindOutputTextForSlider(WalkerSpawnRateSlider);
			_roomieChanceOutputText = FindOutputTextForSlider(RoomieChanceSlider);
			_maxWalkerOutputText = FindOutputTextForSlider(MaxWalkerSlider);

			_content = GetComponent<ScrollRect>().content;
			RoomEntries = new List<RoomEntryUI>();
			CreateNewRoom(3, 3, 0.5f);
			CreateNewRoom(5, 5, 0.3f);
			CreateNewRoom(7, 7, 0.2f);

			base.Awake();
		}

		#endregion

		#region Protected methods

		protected override void SetDefaultGUIValues()
		{
			base.SetDefaultGUIValues();

			_stepSchemeDropdown.value = 0;
			_carveRateSlider.value = CarveRate;
			MaxStepLengthSlider.value = MaxStepLength;
			LevyFlightChanceSlider.value = LevyFlightChance;
			PreviousDirectionBiasSlider.value = PreviousDirectionBias;
			DecayRateSlider.value = DecayRate;
			SpawnRoomChanceSlider.value = SpawnRoomChance;
			WalkerSpawnRateSlider.value = WalkerSpawnRate;
			RoomieChanceSlider.value = IsRoomieChance;
			MaxWalkerSlider.value = MaxWalkerCount;
			_startInCenterToggle.isOn = StartinCenter;
			EnableLevyFlightToggle.isOn = EnableLevyFlight;
			BiasTowardsPreviousToggle.isOn = BiasTowardsPrevious;

			_carveRateOutputText.text = CarveRate.ToString();
			_maxStepOutputText.text = MaxStepLength.ToString();
			_levyFlightChanceOutputText.text = LevyFlightChance.ToString();
			_previousDirectionOutputText.text = PreviousDirectionBias.ToString();
			_previousDirectionDecayOutputText.text = DecayRate.ToString();
			_spawnRoomChanceOutputText.text = SpawnRoomChance.ToString();
			_walkerSpawnRateOutputText.text = WalkerSpawnRate.ToString();
			_roomieChanceOutputText.text = IsRoomieChance.ToString();
			_maxWalkerOutputText.text = MaxWalkerCount.ToString();
		}

		#endregion

		#region Private methods

		private void OnMaxWalkersChanged(float value)
		{
			MaxWalkerCount = (int) value;
			_maxWalkerOutputText.text = MaxWalkerCount.ToString();
		}

		private void OnRoomieChanceChanged(float value)
		{
			IsRoomieChance = value;
			_roomieChanceOutputText.text = IsRoomieChance.ToString();
		}

		private void OnWalkerSpawnRateChanged(float value)
		{
			WalkerSpawnRate = value;
			_walkerSpawnRateOutputText.text = WalkerSpawnRate.ToString();
		}

		private void OnToggleLevyFlight(bool value)
		{
			EnableLevyFlight = value;
		}

		private void OnSpawnRoomChanceChanged(float value)
		{
			_spawnRoomChanceOutputText.text = value.ToString();
			SpawnRoomChance = value;
		}

		private void CreateNewRoom(int width = 3, int height = 3, float prob = 0.0f)
		{
			RoomEntryUI newEntry = Instantiate(_roomEntryPrefab, _roomConfigHolder);
			newEntry.transform.position = new Vector3(newEntry.transform.position.x, newEntry.transform.position.y - RoomEntryUI.UIElementHeight * _roomCount, newEntry.transform.position.z);
			newEntry.RemoveButton.onClick.AddListener(() =>
			                                          {
				                                          int deletedRoomIndex = newEntry.Index;
				                                          foreach (RoomEntryUI entry in RoomEntries.Where(r => r.Index > deletedRoomIndex))
				                                          {
					                                          entry.Setup(entry.Index - 1, entry.Width, entry.Height, entry.Probability);
					                                          entry.transform.position = new Vector3(entry.transform.position.x, entry.transform.position.y + RoomEntryUI.UIElementHeight, entry.transform.position.z);
				                                          }

				                                          _sliderGroup.RemoveSlider(newEntry.ProbabilitySlider);
				                                          RoomEntries.Remove(newEntry);
				                                          _roomCount--;
				                                          _content.sizeDelta = new Vector2(_content.sizeDelta.x, _content.sizeDelta.y - RoomEntryUI.UIElementHeight);
			                                          });
			_roomCount++;
			newEntry.Setup(_roomCount, width, height, prob);
			RoomEntries.Add(newEntry);
			_sliderGroup.AddSlider(newEntry.ProbabilitySlider);
			_content.sizeDelta = new Vector2(_content.sizeDelta.x, _content.sizeDelta.y + RoomEntryUI.UIElementHeight);
		}

		private void OnDecayRateChanged(float value)
		{
			DecayRate = value;
			_previousDirectionDecayOutputText.text = value.ToString();
		}

		private void OnPreviousDirectionBiasChanged(float value)
		{
			PreviousDirectionBias = value;
			_previousDirectionOutputText.text = value.ToString();
		}

		private void OnBiasTowardsPreviousChanged(bool isOn)
		{
			BiasTowardsPrevious = isOn;
		}

		private void OnStepLengthChanged(float value)
		{
			MaxStepLength = (int) value;
			_maxStepOutputText.text = value.ToString();
		}

		private void OnLevyChanceChanged(float value)
		{
			LevyFlightChance = value;
			_levyFlightChanceOutputText.text = value.ToString();
		}

		private void OnStartInCenterChanged(bool isOn)
		{
			StartinCenter = isOn;
		}

		private void OnCarveRateChanged(float value)
		{
			CarveRate = value;
			_carveRateOutputText.text = value.ToString();
		}

		private void OnStepSchemeChanged(int index)
		{
			switch (index)
			{
				case 0:
					StepScheme = DrunkardsWalk.StepScheme.Traditional;
					break;
				case 1:
					StepScheme = DrunkardsWalk.StepScheme.EightWays;
					break;
				case 2:
					StepScheme = DrunkardsWalk.StepScheme.Hexagonal;
					break;
				default:
					Debug.LogWarning("Unexpected StepScheme");
					break;
			}
		}

		#endregion
	}
}