using UnityEngine;
using System.Collections;

public enum TriggerMode
{
	DanJi,
	LianJi,
}

public class XKTriggerRemoveNpc : MonoBehaviour {
	public TriggerMode Mode = TriggerMode.DanJi;
	public XKSpawnNpcPoint[] SpawnPointArray;
	public AiPathCtrl TestPlayerPath;

	void Start()
	{
		/*switch (Mode) {
		case TriggerMode.DanJi:
			if (XkGameCtrl.GameModeVal == GameMode.LianJi) {
				return;
			}
			break;
			
		case TriggerMode.LianJi:
			if (XkGameCtrl.GameModeVal != GameMode.LianJi) {
				return;
			}
			break;
		}*/
		
		for (int i = 0; i < SpawnPointArray.Length; i++) {
			if (SpawnPointArray[i] == null) {
				Debug.LogWarning("SpawnPointArray was wrong! index "+(i+1));
				GameObject obj = null;
				obj.name = "null";
				break;
			}
			SpawnPointArray[i].SetIsRemoveTrigger();
		}
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
	}

	void OnTriggerEnter(Collider other)
	{
		if (Network.peerType == NetworkPeerType.Client && ScreenDanHeiCtrl.IsStartGame) {
			return;
		}

		if (other.GetComponent<XkPlayerCtrl>() == null) {
			return;
		}

		switch (Mode) {
		case TriggerMode.DanJi:
			if (XkGameCtrl.GameModeVal == GameMode.LianJi) {
				if (XkPlayerCtrl.PlayerTranFeiJi != null && XkPlayerCtrl.PlayerTranTanKe != null) {
					return;
				}
			}
			break;

		case TriggerMode.LianJi:
			if (XkGameCtrl.GameModeVal != GameMode.LianJi) {
				return;
			}
			break;
		}

		RemoveSpawnPointNpc();
	}

	public void RemoveSpawnPointNpc()
	{
		//Debug.Log("XKTriggerRemoveNpc::OnTriggerEnter -> hit "+other.name);
		for (int i = 0; i < SpawnPointArray.Length; i++) {
			SpawnPointArray[i].RemovePointAllNpc();
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

		if (SpawnPointArray.Length > 0) {
			for (int i = 0; i < SpawnPointArray.Length; i++) {
				if (SpawnPointArray[i] == null) {
					Debug.LogWarning("SpawnPointArray was wrong! SpawnPoint is null, index "+i);
					GameObject obj = null;
					obj.name = "null";
					break;
				}
				SpawnPointArray[i].AddTestTriggerRemoveNpc(this);
			}
		}

		if (TestPlayerPath != null) {
			TestPlayerPath.DrawPath();
		}
	}
}