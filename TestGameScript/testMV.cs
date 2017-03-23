using UnityEngine;
using System.Collections;

public class testMV : MonoBehaviour
{
	public MovieTexture mvObj;
	// Use this for initialization
	void Start()
	{
		mvObj.loop = true;
	}

	void OnGUI()
	{
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), mvObj, ScaleMode.StretchToFill);
		if (!mvObj.isPlaying) {
			mvObj.Play();
		}
	}
}
