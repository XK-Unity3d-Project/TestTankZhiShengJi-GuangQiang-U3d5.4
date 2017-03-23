using UnityEngine;
using System.Collections;

public class XKTriggerPlayerAimRemove : MonoBehaviour {
	public XKSpawnNpcPoint[] SpawnPoint; //主角瞄准的对象.
	public AiPathCtrl TestPlayerPath;
	public NpcPathCtrl TestNpcPath;
	// Use this for initialization
	void Start()
	{
		int max = SpawnPoint.Length;
		if (max <= 0) {
			Debug.LogWarning("SpawnPoint.len is wrong!");
			GameObject obj = null;
			obj.name = "null";
			return;
		}
		
		XKSpawnNpcPoint script = null;
		for (int i = 0; i < max; i++) {
			if (SpawnPoint[i] == null) {
				Debug.LogWarning("SpawnPoint is wrong! index is " + i);
				GameObject obj = null;
				obj.name = "null";
				break;
			}
			
			script = SpawnPoint[i].GetComponent<XKSpawnNpcPoint>();
			script.SetIsPlayerLeaveTrigger();
		}
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
	}

	void OnTriggerEnter(Collider other)
	{
		XkPlayerCtrl playerScript = other.GetComponent<XkPlayerCtrl>();
		XKNpcMoveCtrl npcScript = other.GetComponent<XKNpcMoveCtrl>();
		if (npcScript != null) {
			/*if (!npcScript.IsAniMove) {
				return;
			}*/

			if (npcScript.TestSpawnPoint == null) {
				return;
			}

			XKSpawnNpcPoint spawnNpcPoint = npcScript.TestSpawnPoint.GetComponent<XKSpawnNpcPoint>();
			if (spawnNpcPoint == null) {
				return;
			}

			if (SpawnPoint[0] != spawnNpcPoint) {
				return;
			}
			playerScript = XkPlayerCtrl.GetInstanceFeiJi();
		}

		if (playerScript == null) {
			return;
		}

		int max = SpawnPoint.Length;
		for (int i = 0; i < max; i++) {
			if (SpawnPoint[i] != null) {
				playerScript.RemoveAimSpawnPoint(SpawnPoint[i]);
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}
		
		if (TestPlayerPath != null) {
			TestPlayerPath.DrawPath();
		}

		if (TestNpcPath != null) {
			TestNpcPath.DrawPath();
		}
	}
}