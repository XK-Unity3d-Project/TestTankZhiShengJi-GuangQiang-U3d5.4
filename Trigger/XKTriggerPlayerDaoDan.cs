using UnityEngine;
using System.Collections;

public class XKTriggerPlayerDaoDan : MonoBehaviour
{
	public GameObject PlayerDaoDan;
	public Transform[] AmmoPointTran;
	public AiPathCtrl TestPlayerPath;
	void Start()
	{
//		if (Network.peerType == NetworkPeerType.Client) {
//			gameObject.SetActive(false);
//			return;
//		}
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);

		bool isOutputError = false;
		if (PlayerDaoDan == null) {
			Debug.LogWarning("PlayerDaoDan was null");
			isOutputError = true;
		}

		int max = AmmoPointTran.Length;
		for (int i = 0; i < max; i++) {
			if (AmmoPointTran[i] == null) {
				Debug.LogWarning("AmmoPointTran was wrong! index "+i);
				isOutputError = true;
				break;
			}
			AmmoPointTran[i].gameObject.SetActive(true);
		}

		if (isOutputError) {
			GameObject obj = null;
			obj.name = "null";
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
//		if (Network.peerType != NetworkPeerType.Disconnected) {
//			if (Network.peerType == NetworkPeerType.Client) {
//				return;
//			}
//		}

		XkPlayerCtrl script = other.GetComponent<XkPlayerCtrl>();
		if (script == null) {
			return;
		}

		if (PlayerDaoDan != null) {
			SpawnPlayerDaoDan(script, PlayerDaoDan);
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
	}
	
	void SpawnPlayerDaoDan(XkPlayerCtrl script, GameObject playerDaoDan)
	{
		int max = AmmoPointTran.Length;
		for (int i = 0; i < max; i++) {
			if (!AmmoPointTran[i].gameObject.activeSelf) {
				continue;
			}
			AmmoPointTran[i].gameObject.SetActive(false);
			script.SpawnPlayerDaoDan(AmmoPointTran[i], playerDaoDan);
		}
	}
}