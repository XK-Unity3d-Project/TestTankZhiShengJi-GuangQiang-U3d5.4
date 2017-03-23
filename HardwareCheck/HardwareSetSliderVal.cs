using UnityEngine;
using System.Collections;

public class HardwareSetSliderVal : MonoBehaviour {
	UILabel SliderLabel;
	// Use this for initialization
	void Start()
	{
		SliderLabel = GetComponent<UILabel>();
	}
	
	public void SetCurrentPercent()
	{
		if (UIProgressBar.current == null) {
			return;
		}
		int val = Mathf.RoundToInt(UIProgressBar.current.value * 15f);
		string strInfo = "0x" + val.ToString("X2");
		SliderLabel.text = strInfo;
	}
}