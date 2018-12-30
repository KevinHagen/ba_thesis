using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Groups a collection of UnityEngine.UI.Slider and manages their total min/maxValues to match given parameters.
/// </summary>
public class SliderGroup : MonoBehaviour
{
	[SerializeField] private List<Slider> _sliders;
	[SerializeField] private float _totalMinValue;
	[SerializeField] private float _totalMaxValue;

	private void Awake()
	{
		_sliders = new List<Slider>();
	}

	public void AddSlider(Slider slider)
	{
		_sliders.Add(slider);
		slider.onValueChanged.AddListener(v => ClipToBounds());
		ClipToBounds();
	}

	private void ClipToBounds()
	{
		float total = 0f;
		foreach (Slider slider in _sliders)
		{
			total += slider.value;
		}

		int sliderIndex = 0;
		while (total > _totalMaxValue)
		{
			_sliders[sliderIndex].value -= 0.01f;
			total -= 0.01f;
			sliderIndex++;
			sliderIndex %= _sliders.Count;
		}

		while (total < _totalMinValue)
		{
			_sliders[sliderIndex].value += 0.01f;
			total += 0.01f;
			sliderIndex++;
			sliderIndex %= _sliders.Count;
		}
	}

	public void RemoveSlider(Slider slider)
	{
		_sliders.Remove(slider);
		ClipToBounds();
	}
}
