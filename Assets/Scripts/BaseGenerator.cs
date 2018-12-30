using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class BaseGenerator : MonoBehaviour
{
	#region Serialize Fields

	[SerializeField] protected BaseMenu UI;
	[Header("BaseGenerator")] [SerializeField]
	protected bool _useCustomSeed;
	[SerializeField] protected string _seed;
	[SerializeField] protected int _width;
	[SerializeField] protected int _height;
	[SerializeField] protected bool _useVisualization;
	[SerializeField] protected float _timeBetweenSteps;

	#endregion

	#region Protected Fields
	/// <summary>
	/// Determines whether StepByStepGeneration may be executed or not (only relevant in-editor)
	/// </summary>
	protected bool _allowVisualisation;
	/// <summary>
	/// Underlying data for the 2D map. Used to build the actual geometry later
	/// </summary>
	protected bool[,] _tileData;
	/// <summary>
	/// Two dimensional array of tiles of level geometry
	/// </summary>
	protected Tile[,] _tiles;

	#endregion

	#region Unity methods

	protected virtual void Awake()
	{
		if (UI.TimeBetweenStepsSlider != null)
			UI.TimeBetweenStepsSlider.onValueChanged.AddListener(value => _timeBetweenSteps = value);
	}

	#endregion

	#region Public methods

	/// <summary>
	/// Generate the map and its data
	/// </summary>
	public virtual void Generate()
	{
		StopAllCoroutines();
		Clear();
#if UNITY_EDITOR
		if (EditorApplication.isPlaying)
		{
#endif
			GetBaseValues();
			GetGeneratorSpecificData();
#if UNITY_EDITOR
		}
#endif
		CenterGrid();
		InitPRNG();
		PrepareInitialState();
#if UNITY_EDITOR
		_allowVisualisation = EditorApplication.isPlaying;
		if (_allowVisualisation && _useVisualization)
		{
			InstantiateMap();
			StartCoroutine(StepByStepGeneration());
		}
#elif UNITY_STANDALONE || UNITY_WEBGL
		_allowVisualisation = true;
		if (_useVisualization)
		{
			InstantiateMap();
			StartCoroutine(StepByStepGeneration());
		}
#endif
	}

	/// <summary>
	/// Clears the generated data
	/// </summary>
	public virtual void Clear()
	{
		_tiles = null;
		_tileData = null;

		while (transform.childCount > 0)
		{
			DestroyImmediate(transform.GetChild(0).gameObject);
		}
	}

	#endregion

	#region Protected methods

	/// <summary>
	/// Retrieves the generator specific data from the ui
	/// </summary>
	protected abstract void GetGeneratorSpecificData();
	/// <summary>
	/// Instantiate map from the generated data
	/// </summary>
	protected abstract void InstantiateMap();
	/// <summary>
	/// Generates a map in steps executed after a given time interval
	/// </summary>
	/// <returns></returns>
	protected abstract IEnumerator StepByStepGeneration();

	/// <summary>
	/// Sets up all data prior to the actual generation
	/// </summary>
	protected virtual void PrepareInitialState()
	{
		_tileData = new bool[_width, _height];
		_tiles = new Tile[_width, _height];
	}

	/// <summary>
	/// Centers grid on the screen
	/// </summary>
	protected virtual void CenterGrid()
	{
		transform.position = new Vector3(-_width / 2, -_height / 2, 0);
	}

	#endregion

	#region Private methods

	/// <summary>
	/// Retrieves user input from the UI for the Base Generator
	/// </summary>
	private void GetBaseValues()
	{
		_width = UI.Width;
		_height = UI.Height;
		_seed = UI.Seed;
		_useCustomSeed = UI.UseCustomSeed;
		_useVisualization = UI.UseVisualization;
		_timeBetweenSteps = UI.TimeBetweenSteps;
	}

	/// <summary>
	/// Initializie a pseudo-random number generator with the current seed or a generated one if none is given.
	/// </summary>
	private void InitPRNG()
	{
		if (!_useCustomSeed || string.IsNullOrEmpty(_seed))
		{
			_seed = DateTime.Now.ToString();
		}

		Random.InitState(_seed.GetHashCode());
	}

	#endregion
}