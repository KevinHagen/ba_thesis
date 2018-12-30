using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Base GUI that is used for all Classes deriving from BaseGenerator
/// </summary>
[RequireComponent(typeof(Animator))]
public class BaseMenu : MonoBehaviour
{
	#region Static Stuff

	/// <summary>
	///     Find the outputText for a given slider.
	/// </summary>
	/// <param name="slider">Slider whose OutputText Element is required</param>
	/// <returns>TextMeshProUGUI - outputText</returns>
	public static TextMeshProUGUI FindOutputTextForSlider(Slider slider)
	{
		return slider.transform.parent.Find("OutputText").GetComponent<TextMeshProUGUI>();
	}

	#endregion

	#region Serialize Fields

	[SerializeField] private Button _toggleShowButton;
	[SerializeField] private Text _toggleShowText;
	[SerializeField] private TMP_InputField _widthInput;
	[SerializeField] private TMP_InputField _heightInput;
	[SerializeField] private TMP_InputField _seedInput;
	[SerializeField] private Toggle _seedToggle;
	[SerializeField] private Toggle _visToggle;
	[SerializeField] public Slider TimeBetweenStepsSlider;

	#endregion

	#region Private Fields

	private bool _isOpen;
	private Animator _animator;
	private TextMeshProUGUI _timeBetweenStepsOutputText;

	#endregion

	#region Properties

	public int Height { get; private set; } = 50;
	public string Seed { get; private set; } = "CustomSeed";
	public float TimeBetweenSteps { get; protected set; } = 0.5f;
	public bool UseCustomSeed { get; private set; } = true;
	public virtual bool UseVisualization { get; private set; } = true;
	public int Width { get; private set; } = 50;

	#endregion

	#region Unity methods

	protected virtual void Awake()
	{
		_toggleShowButton.onClick.AddListener(ToggleShow);
		_widthInput.onEndEdit.AddListener(OnWidthChanged);
		_heightInput.onEndEdit.AddListener(OnHeightChanged);
		_seedInput.onEndEdit.AddListener(OnSeedChanged);
		_seedToggle.onValueChanged.AddListener(UseSeedChanged);
		if (_visToggle != null)
			_visToggle.onValueChanged.AddListener(OnVisChangend);
		if (TimeBetweenStepsSlider != null)
			TimeBetweenStepsSlider.onValueChanged.AddListener(OnStepTimeChanged);
		_animator = GetComponent<Animator>();
		_isOpen = true;

		if (TimeBetweenStepsSlider != null)
			_timeBetweenStepsOutputText = FindOutputTextForSlider(TimeBetweenStepsSlider);
		SetDefaultGUIValues();
	}

	#endregion

	#region Protected methods

	protected virtual void SetDefaultGUIValues()
	{
		_widthInput.text = Width.ToString();
		_heightInput.text = Height.ToString();
		if (TimeBetweenStepsSlider != null)
			_timeBetweenStepsOutputText.text = TimeBetweenSteps.ToString();
		if (TimeBetweenStepsSlider != null)
			TimeBetweenStepsSlider.value = TimeBetweenSteps;
		_seedInput.text = Seed;
		_seedToggle.isOn = UseCustomSeed;
		if (_visToggle != null)
			_visToggle.isOn = UseVisualization;
	}

	#endregion

	#region Private methods

	private void OnStepTimeChanged(float value)
	{
		TimeBetweenSteps = value;
		_timeBetweenStepsOutputText.text = value.ToString();
	}

	private void OnVisChangend(bool arg0)
	{
		UseVisualization = arg0;
	}

	private void UseSeedChanged(bool arg0)
	{
		UseCustomSeed = arg0;
	}

	private void OnSeedChanged(string arg0)
	{
		Seed = arg0;
	}

	private void OnHeightChanged(string newValue)
	{
		int height = -1;
		if (int.TryParse(newValue, out height))
		{
			Height = height;
		}
		else
		{
			Debug.LogWarning("New Height was no Number...");
		}
	}

	private void OnWidthChanged(string newValue)
	{
		int width = -1;
		if (int.TryParse(newValue, out width))
		{
			Width = width;
		}
		else
		{
			Debug.LogWarning("New Width was no Number...");
		}
	}

	private void ToggleShow()
	{
		_isOpen = !_isOpen;
		_animator.SetBool("show", _isOpen);
		_toggleShowText.text = _isOpen ? ">" : "<";
	}

	#endregion
}