using UnityEngine;
using System.Collections;

public class JiFenJieMianCtrl : MonoBehaviour {
	public GameObject GameOverObj; //服务器的GameOver.
	public GameObject FinishTaskObj;
	public GameObject JiFenJieMianObj;
	public GameObject ScreenDanHeiObj;
	GameObject JiFenZongJieMianObj;
	bool IsShowFinishTask;
	bool IsMakeJiFenStop;
	float TimeStartVal;
	static JiFenJieMianCtrl Instance;
	public static JiFenJieMianCtrl GetInstance()
	{
		return Instance;
	}

	// Use this for initialization
	void Start()
	{
		Instance = this;
		TimeStartVal = Time.realtimeSinceStartup;
		JiFenZongJieMianObj = gameObject;
		UITexture screenTexture = ScreenDanHeiObj.GetComponent<UITexture>();
		screenTexture.alpha = 0f;
		ScreenDanHeiObj.SetActive(false);
		JiFenZongJieMianObj.SetActive(false);
		GameOverObj.SetActive(false);
		FinishTaskObj.SetActive(false);
		JiFenJieMianObj.SetActive(false);
	}

	public bool GetIsShowFinishTask()
	{
		return IsShowFinishTask;
	}

	public void ShowFinishTaskInfo()
	{
		if (IsShowFinishTask) {
			return;
		}
		IsShowFinishTask = true;
		DanYaoInfoCtrl.GetInstanceOne().HiddenPlayerDanYaoInfo();
		DanYaoInfoCtrl.GetInstanceTwo().HiddenPlayerDanYaoInfo();
		ZhunXingCtrl.GetInstanceOne().SetActiveZhunXingObj(false);
		ZhunXingCtrl.GetInstanceTwo().SetActiveZhunXingObj(false);
		CoinPlayerCtrl.GetInstanceOne().HiddenPlayerCoin();
		CoinPlayerCtrl.GetInstanceTwo().HiddenPlayerCoin();
		YouLiangAddCtrl.GetInstance().HiddenYouLiangAdd();
		YouLiangCtrl.GetInstance().HiddenYouLiang();
		XKTriggerClosePlayerUI.ResetIsClosePlayerUI(1);
		JiFenZongJieMianObj.SetActive(true);
		
		if (Network.peerType == NetworkPeerType.Server) {
			ScreenDanHeiCtrl.GetInstance().OpenPlayerUI();
			if (GameOverCtrl.IsShowGameOver) {
				GameOverObj.SetActive(true);
			}
			else {
				FinishTaskObj.SetActive(true);
			}
			return;
		}
		FinishTaskObj.SetActive(true);
		XKGlobalData.GetInstance().PlayAudioRenWuOver();
	}

	public void DelayActiveJiFenJieMian()
	{
		ScreenDanHeiObj.SetActive(true);
//		CancelInvoke("ActiveJiFenJieMian");
//		Invoke("ActiveJiFenJieMian", 2f);
	}

	public void ActiveJiFenJieMian()
	{
		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}

		if (JiFenJieMianObj.activeSelf) {
			return;
		}
		FinishTaskObj.SetActive(false);

		if (!JiFenZongJieMianObj.activeSelf) {
			DanYaoInfoCtrl.GetInstanceOne().HiddenPlayerDanYaoInfo();
			DanYaoInfoCtrl.GetInstanceTwo().HiddenPlayerDanYaoInfo();
			ZhunXingCtrl.GetInstanceOne().SetActiveZhunXingObj(false);
			ZhunXingCtrl.GetInstanceTwo().SetActiveZhunXingObj(false);
			CoinPlayerCtrl.GetInstanceOne().HiddenPlayerCoin();
			CoinPlayerCtrl.GetInstanceTwo().HiddenPlayerCoin();
			YouLiangAddCtrl.GetInstance().HiddenYouLiangAdd();
			YouLiangCtrl.GetInstance().HiddenYouLiang();
			JiFenZongJieMianObj.SetActive(true);
		}
		else {
			XkGameCtrl.HiddenMissionCleanup();

			if (Network.peerType == NetworkPeerType.Client) {
				if (NetCtrl.GetInstance() != null) {
					NetCtrl.GetInstance().TryCloseServerPort();
				}
			}
		}

		System.GC.Collect();
		if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
			XkPlayerCtrl.GetInstanceFeiJi().StopPlayerMoveAudio();
		}
		
		if (XkPlayerCtrl.GetInstanceTanKe() != null) {
			XkPlayerCtrl.GetInstanceTanKe().StopPlayerMoveAudio();
		}
		JiFenJieMianObj.SetActive(true);
		
		if (XkGameCtrl.IsPlayGamePOne) {
			XunZhangJBCtrl.GetInstanceOne().ShowPlayerXunZhangJB();
		}
		
		if (XkGameCtrl.IsPlayGamePTwo) {
			XunZhangJBCtrl.GetInstanceTwo().ShowPlayerXunZhangJB();
		}
		
		pcvr.CloseGunZhenDongDengJi();
		pcvr.CloseAllQiNangArray();
		if (pcvr.GetInstance() != null) {
			pcvr.GetInstance().CloseFangXiangPanPower(PlayerEnum.Null);
		}

		Invoke("DelayForceRestartComPort", 0.5f);
	}

	void DelayForceRestartComPort()
	{
		if (pcvr.bIsHardWare && MyCOMDevice.GetInstance() != null) {
			MyCOMDevice.GetInstance().ForceRestartComPort();
		}
	}

	public void StopJiFenTime()
	{
		if (IsInvoking("DelayStopJiFenPanel")) {
			CancelInvoke("DelayStopJiFenPanel");
		}

		if (Time.realtimeSinceStartup - TimeStartVal < 8f) {
			Debug.Log("StopJiFenTime -> TimeStartVal was wrong!");
			return;
		}

		if (IsMakeJiFenStop) {
			return;
		}
		IsMakeJiFenStop = true;

		//GameOverCtrl.IsShowGameOver = true; //test
		if (Application.loadedLevel < (int)GameLevel.Scene_4
		//if (Application.loadedLevel < XkGameCtrl.TestGameEndLv //test
		    && Application.loadedLevel < (Application.levelCount - 1)
		    && !GameOverCtrl.IsShowGameOver) {
			MakeOtherPortStopJiFenTime();
			int loadLevel = Application.loadedLevel + 1;
			Debug.Log("loadLevel *** "+loadLevel);
			XkGameCtrl.IsLoadingLevel = true;
			if (NetCtrl.GetInstance() != null) {
				NetCtrl.GetInstance().ResetGameInfo();
			}
			LoadingGameCtrl.ResetLoadingInfo();

			if (!XkGameCtrl.IsGameOnQuit) {
				System.GC.Collect();
				Application.LoadLevel(loadLevel);
			}
		}
		else {
			XkGameCtrl.LoadingGameMovie();
		}
	}

	void MakeOtherPortStopJiFenTime()
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			return;
		}

		if (NetCtrl.GetInstance() != null) {
			NetCtrl.GetInstance().MakeOtherPortStopJiFenTime();
		}
	}
}