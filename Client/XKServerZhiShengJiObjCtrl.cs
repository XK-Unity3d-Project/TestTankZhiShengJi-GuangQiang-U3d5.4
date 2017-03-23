using UnityEngine;
using System.Collections;

public class XKServerZhiShengJiObjCtrl : MonoBehaviour
{
	public GameObject HiddenZhiShengJiObj;
	// Use this for initialization
	void Awake()
	{
		if (Network.peerType == NetworkPeerType.Server || XkGameCtrl.GetInstance().IsServerCameraTest) {
			HiddenZhiShengJiObj.SetActive(false);
		}
	}
}