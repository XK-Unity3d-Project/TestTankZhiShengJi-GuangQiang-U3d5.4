using UnityEngine;
using System.Collections;

public class XKFireParticleCtrl : MonoBehaviour {
	public ParticleSystem GunParticles;                    // Reference to the particle system.
	public DestroyThisTimed DestroyScript;
	public float TimeDestroy;
	GameObject ParticleObj;
	float TimeLastVal;
	// Use this for initialization
	void Awake () {
		ParticleObj = gameObject;
		GunParticles = GetComponentInChildren<ParticleSystem> ();
		DestroyScript = GetComponent<DestroyThisTimed>();
		if (DestroyScript != null) {
			TimeDestroy = DestroyScript.TimeRemove;
		}
		else {
			TimeDestroy = 3f;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if (!ParticleObj.activeSelf) {
			return;
		}

		if (Time.realtimeSinceStartup - TimeLastVal < TimeDestroy) {
			return;
		}
		ParticleObj.SetActive(false);
//		if (!pcvr.bIsHardWare && Input.GetKeyUp( KeyCode.F )) {
//			OpenGunParticle();
//		}
	}

	public void OpenGunParticle()
	{
		if (ParticleObj.activeSelf) {
			return;
		}
		TimeLastVal = Time.realtimeSinceStartup;
		ParticleObj.SetActive(true);
	}
}