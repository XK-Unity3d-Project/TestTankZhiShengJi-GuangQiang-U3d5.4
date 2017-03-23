using UnityEngine;
using System.Collections;

public class XKTriggerGameOver : MonoBehaviour {
	public GameObject PlayerDaoDan;
	public Transform[] AmmoPointTran;
	[Range(0.1f, 30f)]public float TimeGameOver = 2f;
	public AiPathCtrl TestPlayerPath;
	public static bool IsActiveGameOver;
	static XKTriggerGameOver _Instance;
	public static XKTriggerGameOver GetInstance()
	{
		return _Instance;
	}

	void Start()
	{
		_Instance = this;
		if (Network.peerType == NetworkPeerType.Client) {
			gameObject.SetActive(false);
			return;
		}
		IsActiveGameOver = false;
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
		
		int max = AmmoPointTran.Length;
		for (int i = 0; i < max; i++) {
			if (AmmoPointTran[i] == null) {
				Debug.LogWarning("AmmoPointTran was wrong! index "+i);
				GameObject obj = null;
				obj.name = "null";
				break;
			}
			AmmoPointTran[i].gameObject.SetActive(true);
		}
	}

	void Update()
	{
		if (!pcvr.bIsHardWare && ScreenDanHeiCtrl.IsStartGame) {
			if (Input.GetKeyUp(KeyCode.M)) {
				CheckIsActiveGameOver(null); //test
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		XkPlayerCtrl script = other.GetComponent<XkPlayerCtrl>();
		if (script == null) {
			return;
		}
		CheckIsActiveGameOver(script);
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

	void CheckIsActiveGameOver(XkPlayerCtrl script)
	{
		if (DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()) {
			XkGameCtrl.IsActivePlayerOne = DaoJiShiCtrl.IsActivePlayerOne;
			XkGameCtrl.IsActivePlayerTwo = DaoJiShiCtrl.IsActivePlayerTwo;
			DaoJiShiCtrl.GetInstance().StopDaoJiShi();
		}

		if (Network.peerType != NetworkPeerType.Disconnected) {
			if (Network.peerType == NetworkPeerType.Client) {
				return;
			}

			if (NetCtrl.GetInstance() != null) {
				NetCtrl.GetInstance().TryActiveGameOverEvent();
			}
		}
		else {
			if (GameOverCtrl.IsShowGameOver) {
				return;
			}
		}

		if (IsActiveGameOver) {
			return;
		}
		Debug.Log("CheckIsActiveGameOver...");
		IsActiveGameOver = true;
		XKGlobalData.GetInstance().StopAudioRanLiaoJingGao();
		SpawnPlayerDaoDan(script);
	}

	public void SpawnPlayerDaoDan(XkPlayerCtrl script)
	{
		if (DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()) {
			XkGameCtrl.IsActivePlayerOne = DaoJiShiCtrl.IsActivePlayerOne;
			XkGameCtrl.IsActivePlayerTwo = DaoJiShiCtrl.IsActivePlayerTwo;
			DaoJiShiCtrl.GetInstance().StopDaoJiShi();
		}

		if (PlayerDaoDan != null && script != null) {
			int max = AmmoPointTran.Length;
			for (int i = 0; i < max; i++) {
				if (!AmmoPointTran [i].gameObject.activeSelf) {
					continue;
				}
				AmmoPointTran [i].gameObject.SetActive (false);
				script.SpawnPlayerDaoDan (AmmoPointTran [i], PlayerDaoDan);
			}
		}

		if (Network.peerType == NetworkPeerType.Disconnected || Network.peerType == NetworkPeerType.Server) {
			Invoke("DelayShowPlayerFinishTask", TimeGameOver);
		}
	}

	void DelayShowPlayerFinishTask()
	{
		XkGameCtrl.OnPlayerFinishTask(); //goto jiFenPanel.
		MakeOtherClientShowFinishTask();
	}

	void MakeOtherClientShowFinishTask()
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			return;
		}

		if (NetCtrl.GetInstance() != null) {
			NetCtrl.GetInstance().MakeOtherClientShowFinishTask();
		}
	}
}