using UnityEngine;
using System.Collections;

public class XKHuoCheAnimatorCtrl : MonoBehaviour {
	XKHuoCheCtrl HuoCheScript;
	// Use this for initialization
	void Start()
	{
		HuoCheScript = GetComponentInParent<XKHuoCheCtrl>();
	}
	
	void OnTriggerAnimationEvent()
	{
		HuoCheScript.OnNpcTriggerAnimation();
	}
}
