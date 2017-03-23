using UnityEngine;
using System.Collections;

public class GameModeCtrl : MonoBehaviour {
	public static GameMode ModeVal = GameMode.Null;
	public GameObject ModeObj;
	public GameObject StartBtObj;
	public GameObject LoadingObj;
	public GameObject WaitingObj;
	public static bool IsSelectDanJiMode;
	/**
	 * GameModeState == 0 -> 玩家还没有进入游戏.
	 * GameModeState == 1 -> 玩家已经开始进入游戏.
	 */
	public static byte GameModeState = 0;
	bool IsShowGameMode;
	bool IsLoadingGame;
	static GameModeCtrl _Instance;
	public static GameModeCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiServer) {
			ModeVal = GameMode.LianJi;
		}
		else {
			ModeVal = GameMode.Null;
		}

		IsSelectDanJiMode = true;
		GameModeState = 0;
		SetActiveLoading(false);
		HiddenGameMode();
		SetActiveStartBt(false);
		SetActiveWaitingObj(false);
		InputEventCtrl.GetInstance().ClickFireBtOneEvent += ClickFireBtOneEvent;
		InputEventCtrl.GetInstance().ClickFireBtTwoEvent += ClickFireBtTwoEvent;
	}

	void SetActiveWaitingObj(bool isActive)
	{
		if (WaitingObj.activeSelf == isActive) {
			return;
		}
		WaitingObj.SetActive(isActive);
	}

	public void ShowGameMode()
	{
		if (IsShowGameMode) {
			return;
		}
		IsShowGameMode = true;
//		Debug.Log("show game mode selectPanel...");
		XKGlobalData.GetInstance().PlayModeBeiJingAudio();
		InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickFireBtOneEvent;
		InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickFireBtTwoEvent;
		Invoke("DelayShowGameMode", 0.3f);
	}
	
	void DelayShowGameMode()
	{
		ModeObj.SetActive(true);
	}

	void HiddenGameMode()
	{
		if (!ModeObj.activeSelf) {
			return;
		}
		ModeObj.SetActive(false);
	}

	public void SetActiveStartBt(bool IsActive)
	{
		if (StartBtObj.activeSelf == IsActive) {
			return;
		}

		if (IsActive && ModeVal == GameMode.Null) {
			return;
		}

		if (!IsActive) {
			if (ModeVal == GameMode.LianJi) {
				SetActiveWaitingObj(true);
			}
		}
		else {
			SetActiveWaitingObj(false);
		}
		StartBtObj.SetActive(IsActive);
	}

	public void SetActiveLoading(bool isActive)
	{
		if (LoadingObj.activeSelf == isActive) {
			return;
		}
		LoadingObj.SetActive(isActive);

		if (isActive) {
			GameModeState = 1;
			//DanKengCtrl.GetInstance().ShowDanKengObj();
			ZhunXingCtrl.GetInstanceOne().SetActiveZhunXingObj(false);
			ZhunXingCtrl.GetInstanceTwo().SetActiveZhunXingObj(false);
			if (!IsLoadingGame) {
				IsLoadingGame = true;
				Invoke("DelayLoadingGame", 0.2f);
			}
		}
	}

	void DelayLoadingGame()
	{
		XkGameCtrl.LoadingGameScene_1();
	}

	public void ServerCallClientLoadingGame()
	{
		if (!ModeObj.activeSelf) {
			return;
		}
		HiddenGameMode();
		SetActiveLoading(true);
	}
	
	void ClickFireBtOneEvent(ButtonState state)
	{
		if (!XkGameCtrl.IsActivePlayerOne) {
			return;
		}
		CheckGameBtEvent();
	}

	void ClickFireBtTwoEvent(ButtonState state)
	{
		if (!XkGameCtrl.IsActivePlayerTwo) {
			return;
		}
		CheckGameBtEvent();
	}

	void CheckGameBtEvent()
	{
		if (!ModeObj.activeSelf) {
			return;
		}
		
		if (ModeVal == GameMode.Null) {
			return;
		}
		
		if (!StartBtObj.activeSelf) {
			return;
		}

		XKGlobalData.GetInstance().PlayModeQueRenAudio();
		if (ModeVal != GameMode.LianJi) {
			IsSelectDanJiMode = true;
			HiddenGameMode();
			if (NetCtrl.GetInstance() != null) {
				NetCtrl.GetInstance().SendSubLinkCount();
			}
			NetworkServerNet.GetInstance().MakeClientDisconnect(); //Close ClientNet
			SetActiveLoading(true);
		}
		else {
			IsSelectDanJiMode = false;
			if (NetworkServerNet.GetInstance() != null) {
				NetworkServerNet.GetInstance().TryToLinkServer();
			}

			if (NetCtrl.GetInstance() != null) {
				NetCtrl.GetInstance().SendAddLinkCount();
			}
		}
		SetActiveStartBt(false);
	}
}