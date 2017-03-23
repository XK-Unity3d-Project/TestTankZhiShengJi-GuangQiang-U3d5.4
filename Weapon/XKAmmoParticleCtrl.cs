using UnityEngine;
using System.Collections;

public class XKAmmoParticleCtrl : MonoBehaviour {
	public GameObject PuTongAmmoLZ; //普通子弹爆炸粒子.
	// Use this for initialization
	void Start()
	{
		if (PuTongAmmoLZ == null) {
			Debug.LogWarning("PuTongAmmoLZ is null");
			GameObject obj = null;
			obj.name = "null";
		}
	}
}