using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DrunkardsWalk
{
	/// <summary>
	/// UI Panel that displays debug colors used in DrunkardsWalk.cs
	/// </summary>
	public class DebugColorPanel : MonoBehaviour
	{
		public Toggle ShowToggle;
		public Toggle ShowLiveToggle;

		[SerializeField] private Button _toggleOpenButton;
		[SerializeField] private TextMeshProUGUI _toggleShowText;

		private Animator _animator;
		private bool _isOpen;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_toggleOpenButton.onClick.AddListener(ToggleOpen);
			_isOpen = true;
		}

		private void ToggleOpen()
		{
			_isOpen = !_isOpen;
			_animator.SetBool("show", _isOpen);
			_toggleShowText.text = _isOpen ? "X" : "V";
		}
	}
}
