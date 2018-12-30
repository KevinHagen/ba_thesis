using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CellularAutomata
{
	/// <summary>
	/// UI for cellular automaton
	/// </summary>
	public class CAMenu : BaseMenu
	{
		#region Serialize Fields

		[Header("Params")] [SerializeField]
		private Slider _generationSlider;
		[SerializeField] private Slider _startAliveSlider;
		[SerializeField] public Slider ShowStepSlider;
		[Header("Rules")] [SerializeField]
		private Slider _lowerBirthLimitSlider;
		[SerializeField] private Slider _upperBirthLimitSlider;
		[SerializeField] private Slider _starvationLimitSlider;
		[SerializeField] private Slider _overPopulationLimitSlider;
		[Header("Neighbourhood")] [SerializeField]
		private TMP_Dropdown _neighbourhoodDropdown;
		[SerializeField] private Slider _neighbourhoodRangeSlider;
		[SerializeField] private NeighbourhoodVisualizationUI _neighbourhoodVisPanel;
		[SerializeField] private TextMeshProUGUI _neighbourhoodCount;

		#endregion

		#region Private Fields

		private TextMeshProUGUI _generationOutputText;
		private TextMeshProUGUI _startAliveOutputText;
		private TextMeshProUGUI _showStepOutputText;
		private TextMeshProUGUI _neighbourhoodRangeOutputText;
		private TextMeshProUGUI _lowerBirthLimitOutputText;
		private TextMeshProUGUI _upperBirthLimitOutputText;
		private TextMeshProUGUI _starvationLimitOutputText;
		private TextMeshProUGUI _overPopulationLimitOutputText;

		#endregion

		#region Properties

		public int Generations { get; private set; } = 5;
		public int LowerBirthLimit { get; private set; } = 5;
		public int NeighbourhoodIndex { get; private set; }
		public int NeighbourhoodRange { get; private set; } = 1;
		public int OverPopulationLimit { get; private set; } = 8;
		public float StartAliveChance { get; private set; } = 0.5f;
		public int StarvationLimit { get; private set; } = 4;
		public int UpperBirthLimit { get; private set; } = 8;

		#endregion

		#region Unity methods

		protected override void Awake()
		{
			//Listener for parameter changes
			_generationSlider.onValueChanged.AddListener(v =>
			                                             {
				                                             Generations = (int) v;
				                                             _generationOutputText.text = v.ToString();
				                                             ShowStepSlider.maxValue = Generations;
			                                             });
			_startAliveSlider.onValueChanged.AddListener(v =>
			                                             {
				                                             StartAliveChance = v;
				                                             _startAliveOutputText.text = v.ToString();
			                                             });
			ShowStepSlider.onValueChanged.AddListener(v => _showStepOutputText.text = v.ToString());

			//Listener for all rule related parameters
			_lowerBirthLimitSlider.onValueChanged.AddListener(v =>
			                                                  {
				                                                  LowerBirthLimit = (int) v;
				                                                  _lowerBirthLimitOutputText.text = v.ToString();
			                                                  });
			_upperBirthLimitSlider.onValueChanged.AddListener(v =>
			                                                  {
				                                                  UpperBirthLimit = (int) v;
				                                                  _upperBirthLimitOutputText.text = v.ToString();
			                                                  });
			_starvationLimitSlider.onValueChanged.AddListener(v =>
			                                                  {
				                                                  StarvationLimit = (int) v;
				                                                  _starvationLimitOutputText.text = v.ToString();
			                                                  });
			_overPopulationLimitSlider.onValueChanged.AddListener(v =>
			                                                      {
				                                                      OverPopulationLimit = (int) v;
				                                                      _overPopulationLimitOutputText.text = v.ToString();
			                                                      });

			//Listener for neighbourhood related parameters
			_neighbourhoodDropdown.onValueChanged.AddListener(v =>
			                                                  {
				                                                  NeighbourhoodIndex = v;
				                                                  _neighbourhoodVisPanel.UpdateVis(NeighbourhoodRange, v);
				                                                  _neighbourhoodCount.text = _neighbourhoodVisPanel.Neighbourhoods[NeighbourhoodIndex].NeighbourCount.ToString();
				                                                  UpdateRuleSliders();
			                                                  });
			_neighbourhoodRangeSlider.onValueChanged.AddListener(v =>
			                                                     {
				                                                     NeighbourhoodRange = (int) v;
				                                                     _neighbourhoodVisPanel.UpdateVis((int) v, NeighbourhoodIndex);
				                                                     _neighbourhoodRangeOutputText.text = v.ToString();
				                                                     _neighbourhoodCount.text = _neighbourhoodVisPanel.Neighbourhoods[NeighbourhoodIndex].NeighbourCount.ToString();
				                                                     UpdateRuleSliders();
			                                                     });

			//OutputTexts for all sliders
			_generationOutputText = FindOutputTextForSlider(_generationSlider);
			_startAliveOutputText = FindOutputTextForSlider(_startAliveSlider);
			_showStepOutputText = FindOutputTextForSlider(ShowStepSlider);
			_neighbourhoodRangeOutputText = FindOutputTextForSlider(_neighbourhoodRangeSlider);
			_lowerBirthLimitOutputText = FindOutputTextForSlider(_lowerBirthLimitSlider);
			_upperBirthLimitOutputText = FindOutputTextForSlider(_upperBirthLimitSlider);
			_starvationLimitOutputText = FindOutputTextForSlider(_starvationLimitSlider);
			_overPopulationLimitOutputText = FindOutputTextForSlider(_overPopulationLimitSlider);

			//override timeBetweenSteps so user can look longer at each step by default (bcs. usually a lot of details change per step)
			TimeBetweenSteps = 2.0f;

			base.Awake();
		}

		#endregion

		#region Protected methods

		protected override void SetDefaultGUIValues()
		{
			base.SetDefaultGUIValues();
			_generationSlider.value = Generations;
			_startAliveSlider.value = StartAliveChance;
			TimeBetweenStepsSlider.value = TimeBetweenSteps;
			_neighbourhoodRangeSlider.value = NeighbourhoodRange;
			_lowerBirthLimitSlider.value = LowerBirthLimit;
			_upperBirthLimitSlider.value = UpperBirthLimit;
			_starvationLimitSlider.value = StarvationLimit;
			_overPopulationLimitSlider.value = OverPopulationLimit;
			_neighbourhoodDropdown.value = NeighbourhoodIndex;

			_generationOutputText.text = Generations.ToString();
			_startAliveOutputText.text = StartAliveChance.ToString();
			_neighbourhoodRangeOutputText.text = NeighbourhoodRange.ToString();
			_lowerBirthLimitOutputText.text = LowerBirthLimit.ToString();
			_upperBirthLimitOutputText.text = UpperBirthLimit.ToString();
			_starvationLimitOutputText.text = StarvationLimit.ToString();
			_overPopulationLimitOutputText.text = OverPopulationLimit.ToString();
			_neighbourhoodCount.text = _neighbourhoodVisPanel.Neighbourhoods[NeighbourhoodIndex].NeighbourCount.ToString();
		}

		#endregion

		#region Private methods

		private void UpdateRuleSliders()
		{
			int newMaximum = _neighbourhoodVisPanel.Neighbourhoods[NeighbourhoodIndex].NeighbourCount;
			_lowerBirthLimitSlider.maxValue = newMaximum;
			_upperBirthLimitSlider.maxValue = newMaximum;
			_starvationLimitSlider.maxValue = newMaximum;
			_overPopulationLimitSlider.maxValue = newMaximum;
		}

		#endregion
	}
}