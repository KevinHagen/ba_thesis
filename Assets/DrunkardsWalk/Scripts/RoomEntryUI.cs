using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DrunkardsWalk
{
	/// <summary>
	/// UI Panel with necessary information to create, edit and remove rooms for DrunkardsWalk.cs
	/// </summary>
	public class RoomEntryUI : MonoBehaviour
	{
		#region Static Stuff

		public static int UIElementHeight = 167;

		#endregion

		#region Serialize Fields

		[SerializeField] public Button RemoveButton;
		[SerializeField] private TextMeshProUGUI _roomText;
		[SerializeField] private Slider _widthSlider;
		[SerializeField] private Slider _heightSlider;
		[SerializeField] [FormerlySerializedAs("_probabilitySlider")]
		public Slider ProbabilitySlider;

		#endregion

		#region Private Fields

		private TextMeshProUGUI _widthOutputText;
		private TextMeshProUGUI _heightOutputText;
		private TextMeshProUGUI _probabilityOutputText;

		#endregion

		#region Properties

		public int Height { get; private set; } = 3;
		public int Index { get; private set; }
		public float Probability { get; private set; }
		public int Width { get; private set; } = 3;

		#endregion

		#region Unity methods

		private void Awake()
		{
			_widthSlider.onValueChanged.AddListener(v =>
			                                        {
				                                        Width = (int) v;
				                                        _widthOutputText.text = v.ToString();
			                                        });
			_heightSlider.onValueChanged.AddListener(v =>
			                                         {
				                                         Height = (int) v;
				                                         _heightOutputText.text = v.ToString();
			                                         });
			ProbabilitySlider.onValueChanged.AddListener(v =>
			                                             {
				                                             Probability = v;
				                                             _probabilityOutputText.text = v.ToString();
			                                             });
			RemoveButton.onClick.AddListener(() => Destroy(gameObject, 0.25f));

			_widthOutputText = BaseMenu.FindOutputTextForSlider(_widthSlider);
			_heightOutputText = BaseMenu.FindOutputTextForSlider(_heightSlider);
			_probabilityOutputText = BaseMenu.FindOutputTextForSlider(ProbabilitySlider);
		}

		#endregion

		#region Public methods

		public void Setup(int roomNumber, int width, int height, float prob)
		{
			_roomText.text = "Room " + roomNumber;
			Index = roomNumber;

			_widthSlider.value = width;
			_heightSlider.value = height;
			ProbabilitySlider.value = prob;
			_widthOutputText.text = width.ToString();
			_heightOutputText.text = height.ToString();
			_probabilityOutputText.text = prob.ToString();
			Width = width;
			Height = height;
			Probability = prob;
		}

		#endregion
	}
}