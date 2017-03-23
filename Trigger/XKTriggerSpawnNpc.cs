using UnityEngine;
using System.Collections;

public class XKTriggerSpawnNpc : MonoBehaviour {

	public XKSpawnNpcPoint[] SpawnPointArray;
	public AiPathCtrl TestPlayerPath;
	static bool IsDonnotSpawnNpcTest = false;
	void Start()
	{
		for (int i = 0; i < SpawnPointArray.Length; i++) {
			if (SpawnPointArray[i] == null) {
				Debug.LogWarning("SpawnPointArray was wrong! index "+(i+1));
				GameObject obj = null;
				obj.name = "null";
				break;
			}
			SpawnPointArray[i].SetIsSpawnTrigger();
		}
		Invoke("DelayChangeBoxColliderSize", 0.2f);
	}

	void DelayChangeBoxColliderSize()
	{
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
	}

	void OnTriggerEnter(Collider other)
	{
		if (IsDonnotSpawnNpcTest) {
			return; //test;
		}

		if (JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
			return;
		}

		if (!XkGameCtrl.IsMoveOnPlayerDeath) {
			if (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo) {
				return;
			}
		}
		
		if (Network.peerType == NetworkPeerType.Client) {
			return;
		}

		XkPlayerCtrl ScriptPlayer = other.GetComponent<XkPlayerCtrl>();
		if (ScriptPlayer == null) {
			return;
		}

		//test
//		if (ScriptPlayer.PlayerSt == PlayerTypeEnum.FeiJi) {
//			IsDonnotSpawnNpcTest = true;
//		}
//		else {
//			return;
//		}

		//Debug.Log("XKTriggerSpawnNpc::OnTriggerEnter -> hit "+other.name);
		for (int i = 0; i < SpawnPointArray.Length; i++) {
			SpawnPointArray[i].SpawnPointAllNpc();
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

		for (int i = 0; i < SpawnPointArray.Length; i++) {
			if (SpawnPointArray[i] == null) {
				Debug.LogWarning("SpawnPointArray was wrong! index "+(i+1));
				GameObject obj = null;
				obj.name = "null";
				break;
			}
			SpawnPointArray[i].AddTestTriggerSpawnNpc(this);
		}

		if (TestPlayerPath != null) {
			TestPlayerPath.DrawPath();
		}
	}

	public void RemoveCartoonSpawnNpc()
	{
		int max = SpawnPointArray.Length;
		for (int i = 0; i < max; i++) {
			SpawnPointArray[i].RemovePointAllNpc();
		}
	}
}