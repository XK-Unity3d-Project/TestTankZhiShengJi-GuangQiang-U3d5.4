using UnityEngine;
using System.Collections;

public class ScreenDanHeiCtrl : MonoBehaviour {
	public GameObject StartCameraObj;
	public Camera GameUiCamera;
	GameObject ScreenDanHeiObj;
	TweenAlpha DanHeiTweenAlpha;
	public static bool IsStartGame;
	int StartMovePlayerCount;
	UITexture TextureUI;
	static ScreenDanHeiCtrl Instance;
	public static ScreenDanHeiCtrl GetInstance()
	{
		return Instance;
	}

	// Use this for initialization
	void Start()
	{
		Instance = this;
		Time.timeScale = 1.0f;
		IsStartGame = false;
		ScreenDanHeiObj = gameObject;
		GameUiCamera.enabled = false;
		StartCameraObj.SetActive(true);
		TextureUI = GetComponent<UITexture>();
//		if (Screen.width > 1360) {
//			TextureUI.width = Screen.width * 3;
//			TextureUI.height = Screen.height * 3;
//		}
		DanHeiTweenAlpha = ScreenDanHeiObj.GetComponent<TweenAlpha>();
		DanHeiTweenAlpha.enabled = false;
	}

	public void StartPlayDanHei()
	{
		TextureUI.alpha = 255f;
		DanHeiTweenAlpha.enabled = true;
		Invoke("HiddeScreenDanHeiObj", (DanHeiTweenAlpha.duration + DanHeiTweenAlpha.delay));
	}

	void HiddeScreenDanHeiObj()
	{
		ScreenDanHeiObj.SetActive(false);
		XkGameCtrl.GetInstance().CheckIsCartoonShootTest();
	}

	public void CloseStartCartoon()
	{
		CancelInvoke("HiddeScreenDanHeiObj");
		TiaoGuoBtCtrl.GetInstanceCartoon().HiddenTiaoGuoBt();
		ScreenDanHeiObj.SetActive(false);
		if (DanHeiTweenAlpha != null) {
			DestroyObject(DanHeiTweenAlpha);
		}
	}

	/**
	 * key == 1 -> 隐藏主角.
	 */
	public void OpenScreenDanHui(int key = 0)
	{
		if (ScreenDanHeiObj.activeSelf) {
			return;
		}
		ScreenDanHeiObj.SetActive(true);

		if (DanHeiTweenAlpha != null) {
			DestroyObject(DanHeiTweenAlpha);
		}
		DanHeiTweenAlpha = ScreenDanHeiObj.AddComponent<TweenAlpha>();
		DanHeiTweenAlpha.enabled = false;
		DanHeiTweenAlpha.from = 1f;
		DanHeiTweenAlpha.to = 0f;
		EventDelegate.Add(DanHeiTweenAlpha.onFinished, delegate{
			Invoke("HiddeScreenDanHeiObj", 0.1f);
		});
		DanHeiTweenAlpha.enabled = true;
		DanHeiTweenAlpha.PlayForward();
		switch (key) {
		case 1:
			if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
				XkPlayerCtrl.GetInstanceFeiJi().HandlePlayerHiddenArray();
			}

			if (XkPlayerCtrl.GetInstanceTanKe() != null) {
				XkPlayerCtrl.GetInstanceTanKe().HandlePlayerHiddenArray();
			}
			break;
		}
	}

	public bool GetScreenDanHeiObjActive()
	{
		return ScreenDanHeiObj.activeSelf;
	}

	public void InitPlayScreenDanHei()
	{
		if (ScreenDanHeiObj.activeSelf) {
			return;
		}

		ScreenDanHeiObj.SetActive(true);
		if (DanHeiTweenAlpha != null) {
			DestroyObject(DanHeiTweenAlpha);
		}
		DanHeiTweenAlpha = ScreenDanHeiObj.AddComponent<TweenAlpha>();
		DanHeiTweenAlpha.enabled = false;
		DanHeiTweenAlpha.from = 0f;
		DanHeiTweenAlpha.to = 1f;
		EventDelegate.Add(DanHeiTweenAlpha.onFinished, delegate{
			Invoke("OnSreenAlphaToMax", 0.5f);
		});
		DanHeiTweenAlpha.enabled = true;
		DanHeiTweenAlpha.PlayForward();
	}

	void OnSreenAlphaToMax()
	{
		GameMode modeVal = XkGameCtrl.GameModeVal;
		Debug.Log("OnSreenAlphaToMax -> GameMode "+modeVal);

		bool isClearCartoonNpc = true;
		if (!XKTriggerOpenPlayerUI.IsActiveOpenPlayerUI) {
			XKPlayerCamera.GetInstanceCartoon().SetActiveCamera(false);
		}

		switch (modeVal) {
		case GameMode.DanJiFeiJi:
			XkPlayerCtrl.GetInstanceFeiJi().MakePlayerFlyToPathMark();
			break;

		case GameMode.DanJiTanKe:
			XkPlayerCtrl.GetInstanceTanKe().MakePlayerFlyToPathMark();
			break;

		case GameMode.LianJi:
			if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
				XkPlayerCtrl.GetInstanceFeiJi().MakePlayerFlyToPathMark();
			}

			if (XkPlayerCtrl.GetInstanceTanKe() != null) {
				XkPlayerCtrl.GetInstanceTanKe().MakePlayerFlyToPathMark();
			}

			if (Network.peerType != NetworkPeerType.Disconnected) {
				isClearCartoonNpc = false;
			}

			if (Network.peerType == NetworkPeerType.Client) {
				NetCtrl.GetInstance().SendSetScreenDanHeiIsStartGame();
			}
			break;
		}
		DestroyObject(DanHeiTweenAlpha);
		DanHeiTweenAlpha = ScreenDanHeiObj.AddComponent<TweenAlpha>();
		DanHeiTweenAlpha.enabled = false;
		DanHeiTweenAlpha.from = 1f;
		DanHeiTweenAlpha.to = 0f;
		EventDelegate.Add(DanHeiTweenAlpha.onFinished, delegate{
			Invoke("OnSreenAlphaToMin", 0.2f);
		});
		DanHeiTweenAlpha.enabled = true;
		DanHeiTweenAlpha.PlayForward();

		if (isClearCartoonNpc) {
			XkGameCtrl.ClearCartoonSpawnNpc();
		}
		
		if (Network.peerType != NetworkPeerType.Server) {
			IsStartGame = true;
		}
		Time.timeScale = 1.0f;
		switch (modeVal) {
		case GameMode.DanJiFeiJi:
			XkPlayerCtrl.GetInstanceFeiJi().RestartMovePlayer();
			break;
			
		case GameMode.DanJiTanKe:
			XkPlayerCtrl.GetInstanceTanKe().RestartMovePlayer();
			break;
			
		case GameMode.LianJi:
			if (Network.peerType != NetworkPeerType.Server) {
				if (Network.peerType != NetworkPeerType.Client) {
					if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
						XkPlayerCtrl.GetInstanceFeiJi().RestartMovePlayer();
					}
					
					if (XkPlayerCtrl.GetInstanceTanKe() != null) {
						XkPlayerCtrl.GetInstanceTanKe().RestartMovePlayer();
					}
					XKCameraMapCtrl.GetInstance().SetCameraMapState(); //test
				}
				else {
					//SendServerMovePlayer
					NetCtrl.GetInstance().SetScreenDanHieStartMovePlayer();
				}
			}
			else {
				AddStartMovePlayerCount();
			}
			break;
		}
	}

	public void AddStartMovePlayerCount()
	{
		StartMovePlayerCount++;
//		Debug.Log("AddStartMovePlayerCount -> StartMovePlayerCount "+StartMovePlayerCount
//		          +", netLen"+Network.connections.Length);
		if (Network.peerType == NetworkPeerType.Server) {
			if (StartMovePlayerCount > Network.connections.Length) {
				NetCtrl.GetInstance().MakeClientPlayerMove(); //MakeClientPlayerMove
			}
		}
	}

	void OnSreenAlphaToMin()
	{
		if (Network.peerType != NetworkPeerType.Server) {
			ZhunXingTeXiaoCtrl.GetInstance().ShowZhunXingTeXiao();
		}
		else {
			StartCameraObj.SetActive(false);
			ScreenDanHeiObj.SetActive(false);
		}

		if (XkGameCtrl.GetInstance().IsServerCameraTest || Network.peerType == NetworkPeerType.Server) {
			XkGameCtrl.ActiveServerCameraTran();
			if (XKPlayerCamera.GetInstanceFeiJi() != null) {
				XKPlayerCamera.GetInstanceFeiJi().SetEnableCamera(false);
			}

			if (XKPlayerCamera.GetInstanceTanKe() != null) {
				XKPlayerCamera.GetInstanceTanKe().SetEnableCamera(false);
			}
		}
	}

	public void ActiveGameUiCamera()
	{
		if (!XkGameCtrl.GetInstance().IsCartoonShootTest
		    && !XkGameCtrl.GetInstance().IsServerCameraTest) {
			if (Network.peerType != NetworkPeerType.Server) {
				GameUiCamera.enabled = true;
			}
		}
		StartCameraObj.SetActive(false);
		ScreenDanHeiObj.SetActive(false);

		if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
			XkPlayerCtrl.GetInstanceFeiJi().OpenPlayerMoveAudio();
		}

		if (XkPlayerCtrl.GetInstanceTanKe() != null) {
			XkPlayerCtrl.GetInstanceTanKe().OpenPlayerMoveAudio();
		}
	}

	public void ClosePlayerUI()
	{
		GameUiCamera.enabled = false;
	}

	public void OpenPlayerUI()
	{
		if (XkGameCtrl.GetInstance().IsCartoonShootTest
		    || XkGameCtrl.GetInstance().IsServerCameraTest) {
			return;
		}
		GameUiCamera.enabled = true;
	}

	public void CloseStartCamera()
	{
		StartCameraObj.SetActive(false);
	}
	
	public void OpenStartCamera()
	{
		StartCameraObj.SetActive(true);
	}
}