using UnityEngine;
using System.Collections;

public class DirectionalLightCtrl : MonoBehaviour {
	// Use this for initialization
	void Start()
	{
		Light lightCom = GetComponent<Light>();
		if (lightCom != null) {
			if (Network.peerType != NetworkPeerType.Disconnected) {
				lightCom.shadows = LightShadows.None;
			}
			else {
				lightCom.shadows = LightShadows.Hard;
				//lightCom.shadows = LightShadows.None; //test
			}
		}
	}
}