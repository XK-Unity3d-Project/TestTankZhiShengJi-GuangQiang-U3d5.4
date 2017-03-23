using UnityEngine;
using System.Collections;
using System;

public class SetPanelUiRoot : MonoBehaviour
{
	public UILabel CoinStartLabel;
	public UILabel CoinCurLabel;
	public UISprite DuiGouDiffLow;
	public UISprite DuiGouDiffMiddle;
	public UISprite DuiGouDiffHigh;
	
	public UISprite DuiGouYunYingMode;
	public UISprite DuiGouFreeMode;
	
	public UILabel GunZhenDongLbP1;
	public UILabel GunZhenDongLbP2;

	public UILabel[] DianJiSpeedLb;
	
	public UISprite DuiGouLanguageCh;
	public UISprite DuiGouLanguageEn;

	public GameObject SetPanelObj;
	public GameObject JiaoZhunPanelObj;
	public GameObject CeShiPanelObj;
	public Transform StarTran;
	public GameObject QiNangTestPanelObj;
	public UITexture QiNangUITexture;
	/**
	 * QiNangUI[0] -> 1, QiNangUI[1] -> 2, 
	 * QiNangUI[3] -> 4, QiNangUI[2] -> 3, 
	 */
	public Texture[] QiNangUI;
	public Transform GunCrossTran;
	public GameObject GunAdjustObj;
	public UISprite SpriteAdjustGunCross;
	public GameJiTaiType GameJiTai = GameJiTaiType.TanKeJiTai;

	GameObject StarObj;
	public UILabel GameAudioVolumeLB;
	int GameAudioVolume;

	public enum PanelState
	{
		SetPanel,
		JiaoYanPanel,
		CeShiPanel
	}
	public PanelState PanelStVal = PanelState.SetPanel;
	
	public int StarMoveCount;
	int GameDiffState;
	bool IsFreeGameMode;
	
	string FileName = "BikeConfig.xml";
	public static HandleJson HandleJsonObj;

	Vector3 [] SetPanelStarPos;
	public Vector3 [] TanKeStarPos;
	public Vector3 [] FeiJiStarPos;
	Vector3[] TankStarPosTmp = {
		new Vector3(-560f, 230f, 0f),
		new Vector3(-560f, 180f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(-560f, 120f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(-508f, 40f, 0f),
		new Vector3(-307f, 40f, 0f),
		new Vector3(-560f, 10f, 0f),
		new Vector3(-560f, -40f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(-505f, -130f, 0f),
		new Vector3(-300f, -130f, 0f),
		new Vector3(-535f, -185f, 0f),
		new Vector3(-483f, -185f, 0f),
		new Vector3(-300f, -150f, 0f),
		new Vector3(35f, 230f, 0f),
		new Vector3(35f, 200f, 0f),
		new Vector3(35f, 164f, 0f),
		new Vector3(35f, 132f, 0f),
		new Vector3(35f, 100f, 0f),
		new Vector3(35f, 70f, 0f),
		new Vector3(35f, 40f, 0f),
		new Vector3(35f, 5f, 0f),
		new Vector3(35f, -25f, 0f),
		new Vector3(35f, -55f, 0f),
		new Vector3(35f, -85f, 0f),
		new Vector3(35f, -120f, 0f)
	};

	enum SelectSetGameDt
	{
		Null,
		CoinStart = 1,
		GameModeYunYing,
		GameModeMianFei,
		GameDiffDi,
		GameDiffZhong,
		GameDiffGao,
		GunShakeP1,
		GunShakeP2,
		ResetFactory,
		GameLanguageCh,
		GameLanguageEn,
		GameAudioSet,
		GameAudioReset,
		DianJiSpeedP1,
		DianJiSpeedP2,
		Exit,
		GunAdjustP1,
		GunAdjustP2,
		ButtonTest,
		StartLedTest,
		QiNangTest1,
		QiNangTest2,
		QiNangTest3,
		QiNangTest4,
		QiNangTest5,
		QiNangTest6,
		QiNangTest7,
		QiNangTest8,
	}
	
	enum SelectJiaoZhunDate
	{
		GunAdjustP1,
		GunAdjustP2,
	}
	
	enum SelectCeShiDate
	{
		AnJianTest,
		StartLedTest,
		QiNangTest1,
		QiNangTest2,
		QiNangTest3,
		QiNangTest4,
		QiNangTest5,
		QiNangTest6,
		QiNangTest7,
		QiNangTest8,
	}
	SelectCeShiDate GameCeShiSt = SelectCeShiDate.AnJianTest;
	string startCoinInfo = "";

	enum AdjustDirState
	{
		DirectionRight = 0,
		DirectionCenter,
		DirectionLeft
	}
//	AdjustDirState AdjustDirSt = AdjustDirState.DirectionRight;

	AdjustGunDrossState AdjustGunDrossSt = AdjustGunDrossState.GunCrossLU;
	
	public bool IsMoveStar = true;
	enum QiNangTestEnum
	{
		Null = -1,
		QNLF_CQ, //左前气囊充气.
		QNLF_FQ, //左前气囊放气.
		QNRF_CQ, //右前气囊充气.
		QNRF_FQ, //右前气囊放气.
		QNRB_CQ, //右后气囊充气.
		QNRB_FQ, //右后气囊放气.
		QNLB_CQ, //左后气囊充气.
		QNLB_FQ, //左后气囊放气.
		QNEixt, //退出气囊测试.
	}
	QiNangTestEnum QiNangTestState = QiNangTestEnum.Null;

	public static bool IsOpenSetPanel;
	public static SetPanelUiRoot _Instance;
	public static SetPanelUiRoot GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		Time.timeScale = 1.0f;
		IsOpenSetPanel = true;
		XkGameCtrl.IsLoadingLevel = false;

		XkGameCtrl.ResetIsLoadingLevel();
		if (pcvr.GetInstance() != null) {
			pcvr.GetInstance().CloseFangXiangPanPower(PlayerEnum.Null);
		}
		pcvr.CloseAllQiNangArray(1);
		pcvr.OpenPlayerGunZhenDong();
		if (GameMovieCtrl.IsActivePlayer) {
			if (GameTypeCtrl.AppTypeStatic == AppGameType.DanJiFeiJi
			    || GameTypeCtrl.AppTypeStatic == AppGameType.LianJiFeiJi) {
				GameJiTai = GameJiTaiType.FeiJiJiTai;
			}

			if (GameTypeCtrl.AppTypeStatic == AppGameType.DanJiTanKe
			         || GameTypeCtrl.AppTypeStatic == AppGameType.LianJiTanKe) {
				GameJiTai = GameJiTaiType.TanKeJiTai;
			}

			if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiServer
			    || GameTypeCtrl.AppTypeStatic == AppGameType.Null) {
				GameJiTai = GameJiTaiType.Null;
			}
		}
		GameOverCtrl.IsShowGameOver = false;
		pcvr.OpenDongGanState();
		pcvr.StartLightStateP1 = LedState.Mie;
		pcvr.StartLightStateP2 = LedState.Mie;
		XkGameCtrl.SetActivePlayerOne(false);
		XkGameCtrl.SetActivePlayerTwo(false);

		switch (GameJiTai) {
		case GameJiTaiType.TanKeJiTai:
			TanKeStarPos = TankStarPosTmp;
			SetPanelStarPos = TanKeStarPos;
			break;
		case GameJiTaiType.FeiJiJiTai:
			FeiJiStarPos = TankStarPosTmp;
			SetPanelStarPos = FeiJiStarPos;
			break;
		}
		StarObj = StarTran.gameObject;
		SetStarObjActive(true);

		InitHandleJson();
		InitStarImgPos();
		InitGameAudioValue();
		InitCoinStartLabel();
		InitGameDiffDuiGou();
		InitGameModeDuiGou();
		InitGunZDInfo();
		InitDianJiSpeedInfo();
		SetGameLanguageVal();

		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;
		InputEventCtrl.GetInstance().ClickFireBtOneEvent += ClickFireBtOneEvent;
		InputEventCtrl.GetInstance().ClickFireBtTwoEvent += ClickFireBtTwoEvent;
		InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartBtEventP1;
		InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickStartBtEventP2;
		InputEventCtrl.GetInstance().ClickDaoDanBtOneEvent += ClickDaoDanBtOneEvent;
		InputEventCtrl.GetInstance().ClickDaoDanBtTwoEvent += ClickDaoDanBtTwoEvent;
		InputEventCtrl.GetInstance().ClickStopDongGanBtOneEvent += ClickStopDongGanBtOneEvent;
	}

	void Update()
	{
		if (SetBtSt == ButtonState.DOWN && Time.time - TimeSetMoveBt > 1f && Time.frameCount % 200 == 0) {
			MoveStarImg();
		}
	}

	void ClickSetEnterBtEvent(ButtonState val)
	{
		if(val == ButtonState.DOWN)
		{
			return;
		}
		//BackMovieScene(); //test
		HanldeClickEnterBtEvent();
	}
	
	float TimeSetMoveBt;
	ButtonState SetBtSt = ButtonState.UP;
	void ClickSetMoveBtEvent(ButtonState val)
	{
		SetBtSt = val;
		if (val == ButtonState.DOWN) {
			TimeSetMoveBt = Time.time;
			return;
		}
		
		if (Time.time - TimeSetMoveBt > 1f) {
			return;
		}
		MoveStarImg();
	}

	void ClickStopDongGanBtOneEvent(ButtonState val)
	{
		CheckPlayerOnClickBt(PlayerEnum.PlayerOne, 3);
	}

	void ClickDaoDanBtOneEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			return;
		}
		CheckPlayerOnClickBt(PlayerEnum.PlayerOne, 0);
	}

	void ClickDaoDanBtTwoEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			return;
		}
		CheckPlayerOnClickBt(PlayerEnum.PlayerTwo, 0);
	}

	void ClickFireBtOneEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			return;
		}
		CheckPlayerOnClickBt(PlayerEnum.PlayerOne, 2);

		SelectSetGameDt dtEnum = (SelectSetGameDt) StarMoveCount;
		if (dtEnum != SelectSetGameDt.GunAdjustP1) {
			return;
		}
		HanldeClickFireBtEvent();
	}

	void ClickFireBtTwoEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			return;
		}
		CheckPlayerOnClickBt(PlayerEnum.PlayerTwo, 2);

		SelectSetGameDt dtEnum = (SelectSetGameDt) StarMoveCount;
		if (dtEnum != SelectSetGameDt.GunAdjustP2) {
			return;
		}
		HanldeClickFireBtEvent();
	}

	void ClickStartBtEventP1(ButtonState val)
	{
		if(val == ButtonState.DOWN)
		{
			return;
		}
		HandleClickStartBtEventP1();
		CheckPlayerOnClickBt(PlayerEnum.PlayerOne, 1);
	}

	void HandleClickStartBtEventP1()
	{
		SelectSetGameDt dtEnum = (SelectSetGameDt) StarMoveCount;
		if (dtEnum == SelectSetGameDt.GunAdjustP1) {
			HanldeClickFireBtEvent();
		}
	}

	void ClickStartBtEventP2(ButtonState val)
	{
		if(val == ButtonState.DOWN)
		{
			return;
		}
		HandleClickStartBtEventP2();
		CheckPlayerOnClickBt(PlayerEnum.PlayerTwo, 1);
	}
	
	void HandleClickStartBtEventP2()
	{
		SelectSetGameDt dtEnum = (SelectSetGameDt) StarMoveCount;
		if (dtEnum == SelectSetGameDt.GunAdjustP2) {
			HanldeClickFireBtEvent();
		}
	}

	void SetStarObjActive(bool isActive)
	{
		StarObj.SetActive(isActive);
	}

	void InitGameAudioValue()
	{
		string val = HandleJsonObj.ReadFromFileXml(FileName, "GameAudioVolume");
		if (val == null || val == "") {
			val = "7";
			HandleJsonObj.WriteToFileXml(FileName, "GameAudioVolume", val);
		}
		GameAudioVolume = Convert.ToInt32(val);
		GameAudioVolumeLB.text = GameAudioVolume.ToString();
	}

	void InitCoinStartLabel()
	{
		startCoinInfo = HandleJsonObj.ReadFromFileXml(FileName, "START_COIN");
		if (startCoinInfo == null || startCoinInfo == "") {
			startCoinInfo = "1";
			HandleJsonObj.WriteToFileXml(FileName, "START_COIN", startCoinInfo);
		}
		XKGlobalData.GameNeedCoin = Convert.ToInt32( startCoinInfo );
		SetCoinStartLabelInfo();
	}

	public void SetCoinStartLabelInfo()
	{
		//Debug.Log("CoinPlayerOne "+XKGlobalData.CoinPlayerOne+", CoinPlayerTwo "+XKGlobalData.CoinPlayerTwo);
		HandleJsonObj.WriteToFileXml(FileName, "START_COIN", XKGlobalData.GameNeedCoin.ToString());
		CoinStartLabel.text = XKGlobalData.GameNeedCoin.ToString();
		CoinCurLabel.text = "1P Coin " + XKGlobalData.CoinPlayerOne
								+ ", 2P Coin " + XKGlobalData.CoinPlayerTwo;
	}

	void InitHandleJson()
	{
		XKGlobalData.GetInstance();
		FileName = XKGlobalData.FileName;
		HandleJsonObj = XKGlobalData.HandleJsonObj;
	}

	void InitGameDiffDuiGou()
	{
		XKGlobalData.GetGameDiffVal();
		SetGameDiffState();
	}

	void SetGameDiffState()
	{
		switch (XKGlobalData.GameDiff)
		{
		case "0":
			DuiGouDiffLow.enabled = true;
			DuiGouDiffMiddle.enabled = false;
			DuiGouDiffHigh.enabled = false;
			GameDiffState = 0;
			break;
			
		case "1":
			DuiGouDiffLow.enabled = false;
			DuiGouDiffMiddle.enabled = true;
			DuiGouDiffHigh.enabled = false;
			GameDiffState = 1;
			break;
			
		case "2":
			DuiGouDiffLow.enabled = false;
			DuiGouDiffMiddle.enabled = false;
			DuiGouDiffHigh.enabled = true;
			GameDiffState = 2;
			break;

		default:
			XKGlobalData.GameDiff = "1";
			DuiGouDiffLow.enabled = false;
			DuiGouDiffMiddle.enabled = true;
			DuiGouDiffHigh.enabled = false;
			GameDiffState = 1;
			break;
		}
		HandleJsonObj.WriteToFileXml(FileName, "GAME_DIFFICULTY", XKGlobalData.GameDiff);
		GameDiffState++;
	}

	void InitGameModeDuiGou()
	{
		SetGameModeState();
	}

	void SetGameModeState()
	{
		string modeGame = "";
		if (XKGlobalData.IsFreeMode) {
			modeGame = "0";
		}
		else {
			modeGame = "1";
		}

		DuiGouYunYingMode.enabled = !XKGlobalData.IsFreeMode;
		DuiGouFreeMode.enabled = XKGlobalData.IsFreeMode;
		IsFreeGameMode = XKGlobalData.IsFreeMode;
		HandleJsonObj.WriteToFileXml(FileName, "GAME_MODE", modeGame);
	}

	public int GunZDP1 = 0;
	public int GunZDP2 = 0;
	void InitGunZDInfo()
	{
		GunZDP1 = XKGlobalData.GunZhenDongP1;
		SetGunZhenDongP1();

		GunZDP2 = XKGlobalData.GunZhenDongP2;
		SetGunZhenDongP2();
	}

	void SetGunZhenDongP1()
	{
		XKGlobalData.GunZhenDongP1 = GunZDP1;
		pcvr.SetGunZhenDongDengJi(GunZDP1, PlayerEnum.PlayerOne);
		GunZhenDongLbP1.text = GunZDP1.ToString();
		HandleJsonObj.WriteToFileXml(FileName, "GunZDP1", GunZDP1.ToString());
	}
	
	void SetGunZhenDongP2()
	{
		XKGlobalData.GunZhenDongP2 = GunZDP2;
		pcvr.SetGunZhenDongDengJi(GunZDP2, PlayerEnum.PlayerTwo);
		GunZhenDongLbP2.text = GunZDP2.ToString();
		HandleJsonObj.WriteToFileXml(FileName, "GunZDP2", GunZDP2.ToString());
	}

	public int DianJiSpeedP1 = 0;
	public int DianJiSpeedP2 = 0;
	void InitDianJiSpeedInfo()
	{
		DianJiSpeedP1 = XKGlobalData.DianJiSpeedP1;
		SetDianJiSpeedP1();
		
		DianJiSpeedP2 = XKGlobalData.DianJiSpeedP2;
		SetDianJiSpeedP2();
	}
	
	void SetDianJiSpeedP1()
	{
		XKGlobalData.DianJiSpeedP1 = DianJiSpeedP1;
		pcvr.SetPlayerZuoYiDianJiSpeed(DianJiSpeedP1, PlayerEnum.PlayerOne);
		DianJiSpeedLb[0].text = DianJiSpeedP1.ToString();
		HandleJsonObj.WriteToFileXml(FileName, "DianJiSpeedP1", DianJiSpeedP1.ToString());
	}
	
	void SetDianJiSpeedP2()
	{
		XKGlobalData.DianJiSpeedP2 = DianJiSpeedP2;
		pcvr.SetPlayerZuoYiDianJiSpeed(DianJiSpeedP2, PlayerEnum.PlayerTwo);
		DianJiSpeedLb[1].text = DianJiSpeedP2.ToString();
		HandleJsonObj.WriteToFileXml(FileName, "DianJiSpeedP2", DianJiSpeedP2.ToString());
	}

	void SetGameLanguageVal()
	{
		DuiGouLanguageCh.enabled = true;
		DuiGouLanguageEn.enabled = false;
	}

	void HanldeClickEnterBtEvent()
	{
		if (PanelStVal == PanelState.SetPanel) {
			SelectSetGameDt DtEnum = (SelectSetGameDt) StarMoveCount;
			switch (DtEnum) {
			case SelectSetGameDt.CoinStart:
				if (XKGlobalData.GameNeedCoin >= 9) {
					XKGlobalData.GameNeedCoin = 0;
				}
				XKGlobalData.GameNeedCoin++;

				SetCoinStartLabelInfo();
				break;

			case SelectSetGameDt.GameModeYunYing:
			case SelectSetGameDt.GameModeMianFei:
				IsFreeGameMode = !IsFreeGameMode;
				XKGlobalData.IsFreeMode = IsFreeGameMode;
				SetGameModeState();
				break;

			case SelectSetGameDt.GameDiffGao:
			case SelectSetGameDt.GameDiffZhong:
			case SelectSetGameDt.GameDiffDi:
				if (GameDiffState >= 3) {
					GameDiffState = 0;
				}
				XKGlobalData.GameDiff = GameDiffState.ToString();
				SetGameDiffState();
				break;

			case SelectSetGameDt.GunShakeP1:
				if (GunZDP1 >= 15) {
					GunZDP1 = -1;
				}
				GunZDP1++;
				SetGunZhenDongP1();
				break;
				
			case SelectSetGameDt.GunShakeP2:
				if (GunZDP2 >= 15) {
					GunZDP2 = -1;
				}
				GunZDP2++;
				SetGunZhenDongP2();
				break;
				
			case SelectSetGameDt.DianJiSpeedP1:
				if (DianJiSpeedP1 >= 15) {
					DianJiSpeedP1 = 0;
				}
				DianJiSpeedP1++;
				SetDianJiSpeedP1();
				break;
				
			case SelectSetGameDt.DianJiSpeedP2:
				if (DianJiSpeedP2 >= 15) {
					DianJiSpeedP2 = 0;
				}
				DianJiSpeedP2++;
				SetDianJiSpeedP2();
				break;

			case SelectSetGameDt.ResetFactory:
				ResetFactoryInfo();
				break;
				
			case SelectSetGameDt.GameLanguageCh:
				break;

			case SelectSetGameDt.GameLanguageEn:
				break;
				
			case SelectSetGameDt.GameAudioSet:
				GameAudioVolume++;
				if (GameAudioVolume > 10) {
					GameAudioVolume = 0;
				}
				GameAudioVolumeLB.text = GameAudioVolume.ToString();
				HandleJsonObj.WriteToFileXml(FileName, "GameAudioVolume", GameAudioVolume.ToString());
				XKGlobalData.GameAudioVolume = GameAudioVolume;
				break;

			case SelectSetGameDt.GameAudioReset:
				GameAudioVolume = 7;
				GameAudioVolumeLB.text = GameAudioVolume.ToString();
				HandleJsonObj.WriteToFileXml(FileName, "GameAudioVolume", "7");
				XKGlobalData.GameAudioVolume = GameAudioVolume;
				break;

			case SelectSetGameDt.Exit:
				ExitSetPanle();
				break;

			case SelectSetGameDt.GunAdjustP1:
			case SelectSetGameDt.GunAdjustP2:
				ChangeGuiPanel();
				if (SelectSetGameDt.GunAdjustP1 == DtEnum) {
					OpenJiaoYanPanelObj( SelectJiaoZhunDate.GunAdjustP1 );
				}

				if (SelectSetGameDt.GunAdjustP2 == DtEnum) {
					OpenJiaoYanPanelObj( SelectJiaoZhunDate.GunAdjustP2 );
				}
				break;
				
			case SelectSetGameDt.ButtonTest:
				GameCeShiSt = SelectCeShiDate.AnJianTest;
				OpenCeShiPanel();
				break;
				
			case SelectSetGameDt.StartLedTest:
				GameCeShiSt = SelectCeShiDate.StartLedTest;
				OpenCeShiPanel();
				break;
				
			case SelectSetGameDt.QiNangTest1:
				GameCeShiSt = SelectCeShiDate.QiNangTest1;
				OpenCeShiPanel();
				break;
				
			case SelectSetGameDt.QiNangTest2:
				GameCeShiSt = SelectCeShiDate.QiNangTest2;
				OpenCeShiPanel();
				break;
				
			case SelectSetGameDt.QiNangTest3:
				GameCeShiSt = SelectCeShiDate.QiNangTest3;
				OpenCeShiPanel();
				break;
				
			case SelectSetGameDt.QiNangTest4:
				GameCeShiSt = SelectCeShiDate.QiNangTest4;
				OpenCeShiPanel();
				break;
				
			case SelectSetGameDt.QiNangTest5:
				GameCeShiSt = SelectCeShiDate.QiNangTest5;
				OpenCeShiPanel();
				break;
				
			case SelectSetGameDt.QiNangTest6:
				GameCeShiSt = SelectCeShiDate.QiNangTest6;
				OpenCeShiPanel();
				break;
				
			case SelectSetGameDt.QiNangTest7:
				GameCeShiSt = SelectCeShiDate.QiNangTest7;
				OpenCeShiPanel();
				break;
				
			case SelectSetGameDt.QiNangTest8:
				GameCeShiSt = SelectCeShiDate.QiNangTest8;
				OpenCeShiPanel();
				break;
			}
		}
		else if (PanelStVal == PanelState.CeShiPanel) {
			SelectCeShiDate DtEnum = GameCeShiSt;
			switch (DtEnum) {
			case SelectCeShiDate.AnJianTest:
			case SelectCeShiDate.StartLedTest:
				CloseCeShiPanel();
				break;

			case SelectCeShiDate.QiNangTest1:
			case SelectCeShiDate.QiNangTest2:
			case SelectCeShiDate.QiNangTest3:
			case SelectCeShiDate.QiNangTest4:
			case SelectCeShiDate.QiNangTest5:
			case SelectCeShiDate.QiNangTest6:
			case SelectCeShiDate.QiNangTest7:
			case SelectCeShiDate.QiNangTest8:
				CloseCeShiPanel();
				break;
			}
		}
	}

	void HanldeClickFireBtEvent()
	{
		if (GunAdjustObj.activeSelf) {
			CloseAllJiaoYanPanel();
		}
	}

	void InitAdjustGunCross()
	{
		AdjustGunDrossSt = AdjustGunDrossState.GunCrossLU;
		ChangeAdjustGunCrossImg();
	}

	void ChangeAdjustGunCrossImg()
	{
		string jiaoZhunCross = "GunJY_";
		int index = (int)AdjustGunDrossSt;
		//jiaoZhunCross = "FangXiangJY_";
		switch (MyCOMDevice.PcvrGameSt) {
		case PcvrComState.TanKeFangXiangZhenDong:
			jiaoZhunCross = "FangXiangJY_";
			break;
		case PcvrComState.TanKeGunZhenDong:
			jiaoZhunCross = "GunJY_";
			break;
		}
		SpriteAdjustGunCross.spriteName = jiaoZhunCross + index.ToString();
	}

	void CloseAllJiaoYanPanel()
	{
		if (GunAdjustObj.activeSelf) {
			SelectSetGameDt dtEnum = (SelectSetGameDt) StarMoveCount;
//			Debug.Log("dtEnum *** "+dtEnum);
			switch (dtEnum) {
			case SelectSetGameDt.GunAdjustP1:
				pcvr.SaveCrossPosInfoPOne(AdjustGunDrossSt);
				break;
				
			case SelectSetGameDt.GunAdjustP2:
				pcvr.SaveCrossPosInfoPTwo(AdjustGunDrossSt);
				break;
			}

			switch (AdjustGunDrossSt) {
			case AdjustGunDrossState.GunCrossLU:
				AdjustGunDrossSt = AdjustGunDrossState.GunCrossRU;
				ChangeAdjustGunCrossImg();
				return;
				
			case AdjustGunDrossState.GunCrossRU:
				AdjustGunDrossSt = AdjustGunDrossState.GunCrossRD;
				ChangeAdjustGunCrossImg();
				return;
				
			case AdjustGunDrossState.GunCrossRD:
				AdjustGunDrossSt = AdjustGunDrossState.GunCrossLD;
				ChangeAdjustGunCrossImg();
				return;
				
			case AdjustGunDrossState.GunCrossLD:
				AdjustGunDrossSt = AdjustGunDrossState.GunCrossOver;
				SetPanelGunCrossCtrl.SetAimObjArrayActive(false);
				SetPanelGunCrossCtrl.SetGunCrossActive(true);
				break;
			}
		}

		GunAdjustObj.SetActive(false);
		if (!pcvr.bIsHardWare) {
			Screen.showCursor = false;
		}
		
		IsMoveStar = true;
		PanelStVal = PanelState.SetPanel;
		StarObj.SetActive(true);
		JiaoZhunPanelObj.SetActive(false);
		SetPanelObj.SetActive(true);
	}

	void OpenJiaoYanPanelObj(SelectJiaoZhunDate selectVal)
	{
		if (GunAdjustObj.activeSelf) {
			if (!GunAdjustObj.activeSelf) {
				CloseAllJiaoYanPanel();
			}
			return;
		}

		IsMoveStar = false;
		StarObj.SetActive(false);
		switch (selectVal) {
		case SelectJiaoZhunDate.GunAdjustP1:
		case SelectJiaoZhunDate.GunAdjustP2:
			InitAdjustGunCross();
			GunAdjustObj.SetActive(true);
			if (!pcvr.bIsHardWare) {
				Screen.showCursor = true;
			}
			break;
		}
	}

	public void SetHitAimObjInfoActive(bool isActive)
	{
	}

	void CloseQiNangTestPanelObj()
	{
		QiNangTestPanelObj.SetActive(false);
		pcvr.CloseAllQiNangArray(1);
	}

	void OpenQiNangTestPanelObj(SelectCeShiDate SelCeShiDt)
	{
		switch (SelCeShiDt) {
		case SelectCeShiDate.QiNangTest1:
			QiNangUITexture.mainTexture = QiNangUI[0];
			pcvr.QiNangArray[0] = 1;
			break;
		case SelectCeShiDate.QiNangTest2:
			QiNangUITexture.mainTexture = QiNangUI[1];
			pcvr.QiNangArray[1] = 1;
			break;
		case SelectCeShiDate.QiNangTest3:
			QiNangUITexture.mainTexture = QiNangUI[2];
			pcvr.QiNangArray[2] = 1;
			break;
		case SelectCeShiDate.QiNangTest4:
			QiNangUITexture.mainTexture = QiNangUI[3];
			pcvr.QiNangArray[3] = 1;
			break;
		case SelectCeShiDate.QiNangTest5:
			QiNangUITexture.mainTexture = QiNangUI[4];
			pcvr.QiNangArray[4] = 1;
			break;
		case SelectCeShiDate.QiNangTest6:
			QiNangUITexture.mainTexture = QiNangUI[5];
			pcvr.QiNangArray[5] = 1;
			break;
		case SelectCeShiDate.QiNangTest7:
			QiNangUITexture.mainTexture = QiNangUI[6];
			pcvr.QiNangArray[6] = 1;
			break;
		case SelectCeShiDate.QiNangTest8:
			QiNangUITexture.mainTexture = QiNangUI[7];
			pcvr.QiNangArray[7] = 1;
			break;
		}
		QiNangTestPanelObj.SetActive(true);
		CeShiPanelObj.SetActive(true);
	}

	void OnClickQiNangTestEnvent()
	{
		Debug.Log("OnClickQiNangTestEnvent "+QiNangTestState);
		switch (QiNangTestState) {
		case QiNangTestEnum.QNLF_CQ:
			pcvr.QiNangArray[0] = 1;
			break;
			
		case QiNangTestEnum.QNLF_FQ:
			pcvr.QiNangArray[0] = 0;
			break;
			
		case QiNangTestEnum.QNRF_CQ:
			pcvr.QiNangArray[1] = 1;
			break;
			
		case QiNangTestEnum.QNRF_FQ:
			pcvr.QiNangArray[1] = 0;
			break;
			
		case QiNangTestEnum.QNRB_CQ:
			pcvr.QiNangArray[2] = 1;
			break;
			
		case QiNangTestEnum.QNRB_FQ:
			pcvr.QiNangArray[2] = 0;
			break;
			
		case QiNangTestEnum.QNLB_CQ:
			pcvr.QiNangArray[3] = 1;
			break;
			
		case QiNangTestEnum.QNLB_FQ:
			pcvr.QiNangArray[3] = 0;
			break;
		}
	}

	void CloseAllTestPanel()
	{
		Debug.Log("CloseAllTestPanel...");
		SetPanelGunCrossCtrl.SetGunCrossActive(false);
		StarObj.SetActive(true);
		StarMoveCount = StarMoveCount > 0 ? (StarMoveCount - 1) : 0;
		MoveStarImg();
	}

	void ChangeGuiPanel()
	{
		PanelState stValTmp = PanelStVal;
		switch (PanelStVal) {
		case PanelState.SetPanel:
			SelectSetGameDt SetPanelDt = (SelectSetGameDt) StarMoveCount;
			if (SetPanelDt == SelectSetGameDt.GunAdjustP1
			    || SetPanelDt == SelectSetGameDt.GunAdjustP2) {
				JiaoZhunPanelObj.SetActive(true);
				CeShiPanelObj.SetActive(false);
				PanelStVal = PanelState.JiaoYanPanel;
			}
			SetPanelObj.SetActive(false);
			break;
			
		case PanelState.JiaoYanPanel:
		case PanelState.CeShiPanel:
			JiaoZhunPanelObj.SetActive(false);
			CeShiPanelObj.SetActive(false);
			SetPanelObj.SetActive(true);
			PanelStVal = PanelState.SetPanel;
			break;
		}

		if (stValTmp == PanelState.CeShiPanel) {
			ResetStarImgPos(false);
		}
		else {
			ResetStarImgPos(true);
		}
	}
	
	public GameObject AnJianTestObj;
	public UITexture AnJianUITexture;
	/**
	 * AnJianUI[0] -> player1 daoDanBt.
	 * AnJianUI[1] -> player1 startBt.
	 * AnJianUI[2] -> player1 fireBt.
	 * AnJianUI[3] -> player2 daoDanBt.
	 * AnJianUI[4] -> player2 startBt.
	 * AnJianUI[5] -> player2 fireBt.
	 * AnJianUI[6] -> Player jinJiBt.
	 */
	public Texture[] AnJianUI;
	/**
	 * btKey == 0 -> daoDanBt.
	 * btKey == 1 -> startBt.
	 * btKey == 2 -> fireBt.
	 * btKey == 3 -> jinJiBt.
	 */
	void CheckPlayerOnClickBt(PlayerEnum playerVal, int btKey)
	{
		if (!AnJianTestObj.activeSelf) {
			return;
		}

		int indexVal = btKey;
		if (btKey == 3) {
			indexVal = 6;
		}
		else {
			if (playerVal == PlayerEnum.PlayerTwo) {
				indexVal += 3;
			}
		}
		Debug.Log("indexVal "+indexVal);
		AnJianUITexture.mainTexture = AnJianUI[indexVal];
	}

	public GameObject StartLedObj;
	public UITexture StartLedUITexture;
	/**
	 * StartLedUI[0] -> Shan.
	 * StartLedUI[1] -> Mie.
	 */
	public Texture[] StartLedUI;
	LedState StartLedSt = LedState.Shan;
	void CloseStartLedCheck()
	{
		StartLedObj.SetActive(false);
		CancelInvoke("CheckStartLedState");
	}

	void OpenStartLedCheck()
	{
		StartLedObj.SetActive(true);
		InvokeRepeating("CheckStartLedState", 0f, 3f);
	}

	void CheckStartLedState()
	{
		if (StartLedSt == LedState.Shan) {
			pcvr.StartLightStateP1 = LedState.Shan;
			pcvr.StartLightStateP2 = LedState.Shan;
			StartLedUITexture.mainTexture = StartLedUI[0];
			StartLedSt = LedState.Mie;
			return;
		}
		
		if (StartLedSt == LedState.Mie) {
			pcvr.StartLightStateP1 = LedState.Mie;
			pcvr.StartLightStateP2 = LedState.Mie;
			StartLedUITexture.mainTexture = StartLedUI[1];
			StartLedSt = LedState.Shan;
			return;
		}
	}

	void CloseCeShiPanel()
	{
		Debug.Log("CloseCeShiPanel...");
		IsMoveStar = true;
		PanelStVal = PanelState.SetPanel;
		CeShiPanelObj.SetActive(false);
		AnJianTestObj.SetActive(false);
		CloseStartLedCheck();
		CloseQiNangTestPanelObj();
	}

	void OpenCeShiPanel()
	{
		Debug.Log("OpenCeShiPanel...");
		JiaoZhunPanelObj.SetActive(false);
		CeShiPanelObj.SetActive(true);
		PanelStVal = PanelState.CeShiPanel;
		CheckOpenCeShiPanel();
	}

	void CheckOpenCeShiPanel()
	{
		bool isReturn = false;
		switch (GameCeShiSt) {
		case SelectCeShiDate.AnJianTest:
			AnJianUITexture.mainTexture = null;
			AnJianTestObj.SetActive(true);
			isReturn = true;
			break;
		case SelectCeShiDate.StartLedTest:
			OpenStartLedCheck();
			isReturn = true;
			break;
		}

		if (isReturn) {
			return;
		}

		switch(GameJiTai) {
		case GameJiTaiType.TanKeJiTai:
			CheckOpenTanKeCeShiInfo();
			break;

		case GameJiTaiType.FeiJiJiTai:
			break;
		}
	}

	void CheckOpenTanKeCeShiInfo()
	{
		switch (GameCeShiSt) {
		case SelectCeShiDate.QiNangTest1:
		case SelectCeShiDate.QiNangTest2:
		case SelectCeShiDate.QiNangTest3:
		case SelectCeShiDate.QiNangTest4:
		case SelectCeShiDate.QiNangTest5:
		case SelectCeShiDate.QiNangTest6:
		case SelectCeShiDate.QiNangTest7:
		case SelectCeShiDate.QiNangTest8:
			OpenQiNangTestPanelObj(GameCeShiSt);
			break;
		}
	}

	void OpenSetPanel()
	{
		ChangeGuiPanel();
	}

	void ExitSetPanle()
	{
		IsOpenSetPanel = false;
		BackMovieScene();
	}

	void ResetFactoryInfo()
	{
		ResetPlayerCoinCur();
		XKGlobalData.GameNeedCoin = 1;
		XKGlobalData.GameDiff = "1";
		XKGlobalData.IsFreeMode = false;

		HandleJsonObj.WriteToFileXml(FileName, "START_COIN", XKGlobalData.GameNeedCoin.ToString());
		HandleJsonObj.WriteToFileXml(FileName, "GAME_DIFFICULTY", "1");
		HandleJsonObj.WriteToFileXml(FileName, "GAME_MODE", "1");
		
		GameAudioVolume = 7;
		GameAudioVolumeLB.text = GameAudioVolume.ToString();
		HandleJsonObj.WriteToFileXml(FileName, "GameAudioVolume", "7");
		XKGlobalData.GameAudioVolume = GameAudioVolume;

		GunZDP1 = 5;
		GunZDP2 = 5;
		DianJiSpeedP1 = 5;
		DianJiSpeedP2 = 5;
		SetGunZhenDongP1();
		SetGunZhenDongP2();
		SetDianJiSpeedP1();
		SetDianJiSpeedP2();
		SetGameLanguageVal();
		InitCoinStartLabel();
		InitGameDiffDuiGou();
		InitGameModeDuiGou();
	}

	void InitStarImgPos()
	{
		MoveStarImg();
	}

	void ResetStarImgPos(bool isReset)
	{
		InitStarImgPos();
	}

	void MoveStarImg()
	{
		if (!StarObj.activeSelf) {
			return;
		}

		Vector3 pos = Vector3.zero;
		switch(PanelStVal)
		{
		case PanelState.SetPanel:
			if (StarMoveCount >= SetPanelStarPos.Length) {
				StarMoveCount = 0;
			}
			
			SelectSetGameDt selDtState = (SelectSetGameDt)StarMoveCount;
			if (GameJiTai == GameJiTaiType.TanKeJiTai) {
				switch (selDtState) {
				case SelectSetGameDt.GameDiffDi:
					StarMoveCount = (int)SelectSetGameDt.GameDiffGao;
					break;

				case SelectSetGameDt.GameModeYunYing:
					StarMoveCount = (int)SelectSetGameDt.GameModeMianFei;
					break;
					
				case SelectSetGameDt.GameLanguageCh:
					StarMoveCount = (int)SelectSetGameDt.GameLanguageEn;
					break;
				}
			}

			if (GameJiTai == GameJiTaiType.FeiJiJiTai) {
				switch (selDtState) {
				case SelectSetGameDt.GameDiffDi:
					StarMoveCount = (int)SelectSetGameDt.GameDiffGao;
					break;
					
				case SelectSetGameDt.GameModeYunYing:
					StarMoveCount = (int)SelectSetGameDt.GameModeMianFei;
					break;
					
				case SelectSetGameDt.GameLanguageCh:
					StarMoveCount = (int)SelectSetGameDt.GameLanguageEn;
					break;
					
				case SelectSetGameDt.GameAudioReset:
					StarMoveCount = (int)SelectSetGameDt.DianJiSpeedP2;
					break;
					
				case SelectSetGameDt.StartLedTest:
					StarMoveCount = (int)SelectSetGameDt.Null;
					break;
				}
			}
			pos = SetPanelStarPos[StarMoveCount];
			break;

		case PanelState.JiaoYanPanel:
		case PanelState.CeShiPanel:
			IsMoveStar = false;
			break;
		}

		if (IsMoveStar) {
			StarTran.localPosition = pos;
			StarMoveCount++;
		}
	}

	void ResetPlayerCoinCur()
	{
		XKGlobalData.CoinPlayerOne = 0;
		XKGlobalData.CoinPlayerTwo = 0;
		pcvr.GetInstance().CoinNumCurrentP1 = 0;
		pcvr.GetInstance().CoinNumCurrentP2 = 0;
	}

	void BackMovieScene()
	{
		if(Application.loadedLevel != (int)GameLevel.Movie)
		{
			//ResetPlayerCoinCur(); //test
			//XKGlobalData.GetInstance().gameLeve = GameLevel.Movie;
			
			XkGameCtrl.IsLoadingLevel = true;
			XkGameCtrl.ResetGameInfo();
			if (!XkGameCtrl.IsGameOnQuit) {
				System.GC.Collect();
				Application.LoadLevel((int)GameLevel.Movie);
			}
		}
	}
}