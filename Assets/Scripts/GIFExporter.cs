using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GIFExporter : MonoBehaviour
{
	#region Static Stuff

	private const string ScreenshotFilename = "Screenshot";

	#endregion

	#region Serialize Fields

	[SerializeField] private bool _export;
	[SerializeField] private int _targetFrameRate;
	//[SerializeField] private string _path;

	#endregion

	#region Private Fields

	private string _sceneName;
	private string _fullFilePath;
	private int _currentScreenshotCount;
	private bool _isSetup;

	#endregion

	#region Unity methods

	private void Start()
	{
		if (!_export) return;

		Setup();
	}

	private void Setup()
	{
		_isSetup = false;

		_sceneName = SceneManager.GetActiveScene().name;
		string directoryForScreenshots = Application.dataPath + "/GIFs" + "/" + _sceneName;

		if (!Directory.Exists(directoryForScreenshots))
			Directory.CreateDirectory(directoryForScreenshots);

		_fullFilePath = directoryForScreenshots + "/" + ScreenshotFilename;
		Time.captureFramerate = _targetFrameRate;


		_isSetup = true;
	}

	private void Update()
	{
		if (!_isSetup && _export)
			Setup();

		if (_export)
			DoExportJob();
	}

	#endregion

	#region Private methods

	private void DoExportJob()
	{
		ScreenCapture.CaptureScreenshot(_fullFilePath + _currentScreenshotCount + ".png");
		_currentScreenshotCount++;
	}

	#endregion
}