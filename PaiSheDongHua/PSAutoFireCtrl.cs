using UnityEngine;
using System.Collections;

public class PSAutoFireCtrl : MonoBehaviour {
	public GameObject AmmoNpcPrefab;
	public GameObject AmmoLiZi;
	public Transform AmmoSpawnPoint;
	public bool IsAimPlayer;
	[Range(0.001f, 500f)] public float Frequency = 10f;
	[Range(1f, 500f)] public float TimeFireVal = 3f;
	float LastFireTime;
	float TimeStartFire;
	// Use this for initialization
	void Start()
	{
		if (AmmoNpcPrefab == null) {
			Debug.LogWarning("AmmoNpcPrefab is null");
			AmmoNpcPrefab.name = "null";
		}
		else {
			NpcAmmoCtrl ammoScript = AmmoNpcPrefab.GetComponent<NpcAmmoCtrl>();
			if (ammoScript == null) {
				Debug.LogWarning("AmmoNpcPrefab cannot find NpcAmmoCtrl");
				GameObject obj = null;
				obj.name = "null";
			}
		}

		if (AmmoSpawnPoint == null) {
			AmmoSpawnPoint = transform;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.L)) {
			StartAutoFire();
		}

		if (Time.realtimeSinceStartup < LastFireTime + 1f / Frequency) {
			return;
		}
		LastFireTime = Time.realtimeSinceStartup;
		
		if (!IsAutoFire) {
			return;
		}
		
		if (Time.realtimeSinceStartup - TimeStartFire > TimeFireVal) {
			return;
		}

		GameObject obj = (GameObject)Instantiate(AmmoNpcPrefab, AmmoSpawnPoint.position, AmmoSpawnPoint.rotation);
		NpcAmmoCtrl AmmoScript = obj.GetComponent<NpcAmmoCtrl>();
		AmmoScript.SetIsAimPlayer(IsAimPlayer);
		AmmoScript.SetIsAimFeiJiPlayer(IsAimPlayer);
		Transform tran = obj.transform;
		tran.parent = XkGameCtrl.MissionCleanup;

		if (AmmoLiZi != null) {
			obj = (GameObject)Instantiate(AmmoLiZi, AmmoSpawnPoint.position, AmmoSpawnPoint.rotation);
			tran = obj.transform;
			tran.parent = XkGameCtrl.MissionCleanup;
			XkGameCtrl.CheckObjDestroyThisTimed(obj);
		}
	}

	bool IsAutoFire;
	public void StartAutoFire()
	{
		if (IsAutoFire) {
			return;
		}
		IsAutoFire = true;
		TimeStartFire = Time.realtimeSinceStartup;
	}
}