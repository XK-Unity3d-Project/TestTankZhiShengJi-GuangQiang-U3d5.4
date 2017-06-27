//#define TEST_SHUIQIANG_ZUOBIAO
//#define COM_TANK_TEST
/**
 * 当该属性COM_TANK_TEST打开后,用来在台儿庄机台上测试坦克游戏.
 */

#define COM_FEIJI_TX
/**
 * COM_FEIJI_TX该属性用来打开飞机机台的硬件通讯逻辑.
 */

using UnityEngine;
using System.Collections;
using System;
using System.Drawing;


public class pcvr : MonoBehaviour {
	public static bool bIsHardWare = true;
	public static bool IsTestHardWareError = false;
	public static Vector3 CrossPositionOne;
	public static Vector3 CrossPositionTwo;
	public static Vector3 CrossPositionThree;
	public static Vector3 CrossPositionFour;
	public static bool IsJiaoYanHid;
	int HID_BUF_LEN_WRITE = 0;
	
	private static int openPCVRFlag = 1;
	private System.IntPtr gTestHidPtr;
	float lastUpTime = 0.0f;
	public static float mGetSteer = 0f;
	public static Vector3 MousePositionP1;
	public static Vector3 MousePositionP2;
	public static bool IsClickFireBtDown;

	public int CoinNumCurrentP1 = 0;
	public int CoinNumCurrentP2 = 0;
	public int CoinNumCurrentP3 = 0;
	public int CoinNumCurrentP4 = 0;
	
	public static bool bPlayerHitTaBan_P1 = false;
	public static bool bPlayerHitTaBan_P2 = false;
	
	public static float VerticalVal;
	public static float TanBanDownCount_P1 = 0;
	public static float TanBanDownCount_P2 = 0;
	
	public static LedState StartLightStateP1 = LedState.Mie;
	public static LedState StartLightStateP2 = LedState.Mie;
	public static bool IsOpenStartLightP1 = false;
	public static bool IsOpenStartLightP2 = false;
	int subCoinNum12 = 0;
	int subCoinNum34 = 0;
	
	static string FileName;
	static HandleJson HandleJsonObj;
	
	public static uint SteerValMax = 999999;
	public static uint SteerValCen = 1765;
	public static uint SteerValMin = 0;
	public static uint SteerValCur;
	public static uint SteerDisVal;
	
	public static uint TaBanValMax;
	public static uint TaBanValMin;
	public static uint TaBanValCur = 30000;
	public static uint TaBanDisVal;
	
	public static uint CrossPosXMaxP1;
	public static uint CrossPosXMinP1;
	public static uint CrossPosXCurP1;
	
	public static uint CrossPosYMaxP1;
	public static uint CrossPosYMinP1;
	public static uint CrossPosYCurP1;
	
	public static uint CrossPosDisXP1;
	public static uint CrossPosDisYP1;

	/**
	 * px1	px2
	 * px4	px3
	 */
	static uint CrossPxP1_1;
	static uint CrossPxP1_2;
	static uint CrossPxP1_3;
	static uint CrossPxP1_4;
	
	/**
	 * py1	py2
	 * py4	py3
	 */
	static uint CrossPyP1_1;
	static uint CrossPyP1_2;
	static uint CrossPyP1_3;
	static uint CrossPyP1_4;

	public static uint CrossPosXMaxP2;
	public static uint CrossPosXMinP2;
	public static uint CrossPosXCurP2;
	
	public static uint CrossPosYMaxP2;
	public static uint CrossPosYMinP2;
	public static uint CrossPosYCurP2;
	
	public static uint CrossPosDisXP2;
	public static uint CrossPosDisYP2;
	
	static uint CrossPxP2_1;
	static uint CrossPxP2_2;
	static uint CrossPxP2_3;
	static uint CrossPxP2_4;
	
	static uint CrossPyP2_1;
	static uint CrossPyP2_2;
	static uint CrossPyP2_3;
	static uint CrossPyP2_4;

	public static bool IsOpenShuiBeng = false;
	public static PcvrShuiBengState ShuiBengState = PcvrShuiBengState.Level_1;
	static uint TanBanCenterNum = 30000;
	static float SubTaBanCount = 2.0f;
	
	bool IsSubPlayerCoin = false;
	bool IsSubCoinP1 = false;
	bool IsSubCoinP2 = false;
	/*bool IsSubCoinP3 = false;
	bool IsSubCoinP4 = false;*/
	static bool IsFireBtDownP1 = false;
	static bool IsFireBtDownP2 = false;
	static bool IsDaoDanBtDownP1 = false;
	static bool IsDaoDanBtDownP2 = false;
	static public bool bPlayerStartKeyDownP1 = false;
	static public bool bPlayerStartKeyDownP2 = false;
	static public bool IsClickDongGanBtOne = false;
	static public bool IsClickDongGanBtTwo = false;
	private bool bSetEnterKeyDown = false;
	static public bool bSetMoveKeyDown = false;
//	static public bool bIsTouBiBtDown = false;
	static bool IsFanZhuangTaBan = false;
	public static uint CoinCurPcvr12;
	public static uint CoinCurPcvr34;
	//9500.0f -> maxSpeed(60km/h)
	public static float PcvrTanBanValTmp = 1.8f * 9500.0f;
	public static int QiangZhenDongP1;
	public static int QiangZhenDongP2;
	public static int DianJiSpeedP1 = 0x01;
	public static int DianJiSpeedP2 = 0x01;
	static pcvr Instance;
	public static pcvr GetInstance()
	{
		if (Instance == null) {
			GameObject obj = new GameObject("_PCVR");
			DontDestroyOnLoad(obj);
			Instance = obj.AddComponent<pcvr>();
			if (bIsHardWare) {
				obj.AddComponent<MyCOMDevice>();
			}

			if (IsTestHardWareError) {
				TestComPort.GetInstance();
			}
			NetworkServerNet.GetInstance();
			XKOpenCGCamera.GetInstance();
			XKLaserPosCtrl.GetInstance();
		}
		return Instance;
	}

	public static bool IsComTankTest;
	// Use this for initialization
	void Awake()
	{
		#if COM_TANK_TEST
		IsComTankTest = true;
		MyCOMDevice.PcvrComSt = PcvrComState.TanKeGunZhenDong; //test.
		#endif

		Debug.Log("MyCOMDevice.PcvrComSt "+MyCOMDevice.PcvrComSt);
//		switch (MyCOMDevice.PcvrComSt) {
//		case PcvrComState.TanKeFangXiangZhenDong:
//			MyCOMDevice.ComThreadClass.BufLenRead = 39;
//			MyCOMDevice.ComThreadClass.BufLenWrite = 32;
//			break;
//		case PcvrComState.TanKeGunZhenDong:
//			MyCOMDevice.ComThreadClass.BufLenRead = 27;
//			MyCOMDevice.ComThreadClass.BufLenWrite = 23;
//			break;
//		}

		if (Application.loadedLevel == (int)GameLevel.Movie) {
			AudioManager.Instance.SetParentTran(transform);
		}
		
		#if TEST_SHUIQIANG_ZUOBIAO
		GunPosOffsetPY = 0f; //test.
		XkGameCtrl.ScreenWidth = 226.6f;
		XkGameCtrl.ScreenHeight = 128f;
		#endif
	}

	void Start()
	{
		HID_BUF_LEN_WRITE = MyCOMDevice.ComThreadClass.BufLenWrite;
		InitJiaoYanMiMa();
		lastUpTime = Time.realtimeSinceStartup;
		InitHandleJsonInfo();
		InitSteerInfo();
		InitTaBanInfo();
		InitCrossPosInfoPOne();
		InitCrossPosInfoPTwo();
	}

//	void Update()
//	{
//		#if TEST_SHUIQIANG_ZUOBIAO
//		if (Input.GetKeyUp(KeyCode.X)) {
//			if (ShuiQiangX >= 5f) {
//				ShuiQiangX = 0f;
//			}
//			else {
//				ShuiQiangX += 1f;
//			}
//		}
//
//		if (Input.GetKeyUp(KeyCode.Y)) {
//			if (ShuiQiangY >= 5f) {
//				ShuiQiangY = 0f;
//			}
//			else {
//				ShuiQiangY += 1f;
//			}
//		}
//		#endif
//
//		if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiServer) {
//			return;
//		}
//		CheckCrossPositionPOne();
//		CheckCrossPositionPTwo();
//		CheckIsPlayerActivePcvr();
//	}

	// Update is called once per frame
//	void FixedUpdate()
//	{
//		if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiServer) {
//			return;
//		}
//		CheckCrossPositionPOne();
//		CheckCrossPositionPTwo();
//
//		if (!bIsHardWare || XkGameCtrl.IsLoadingLevel) {
//			return;
//		}
//		
//		float dTime = Time.realtimeSinceStartup - lastUpTime;
//		if (IsJiaoYanHid) {
//			if (dTime < 0.1f) {
//				return;
//			}
//		}
//		lastUpTime = Time.realtimeSinceStartup;
		
//		GetMessage();
//		SendMessage();
//	}

//	static byte ReadHead_1 = 0x01;
//	static byte ReadHead_2 = 0x55;
	static byte WriteHead_1 = 0x02;
	static byte WriteHead_2 = 0x55;
	static byte WriteEnd_1 = 0x0d;
	static byte WriteEnd_2 = 0x0a;
	byte EndRead_1 = 0x41;
	byte EndRead_2 = 0x42;
	byte EndRead_3 = 0x43;
	byte EndRead_4 = 0x44;

	public static void CheckMovePlayerZuoYi()
	{
		bool isMoveZuoYiP1 = false;
		bool isMoveZuoYiP2 = false;
		
		switch (GameTypeCtrl.AppTypeStatic) {
		case AppGameType.DanJiTanKe:
			if (QiNangArray[0] == 1 || QiNangArray[1] == 1) {
				isMoveZuoYiP1 = true;
			}
			break;
		case AppGameType.LianJiTanKe:
		case AppGameType.LianJiFeiJi:
			if (QiNangArray[0] == 1 || QiNangArray[1] == 1 || QiNangArray[4] == 1) {
				isMoveZuoYiP1 = true;
			}
			break;
		}

		if (isMoveZuoYiP1) {
			if (ZuoYiDianJiSpeedVal[0] == 0x00) {
				Instance.SetZuoYiDianJiSpeed(PlayerEnum.PlayerOne, 1);
			}
		}
		else {
			if (RunZuoYiState[0] == 0x00) {
				if (Instance != null) {
					Instance.SetZuoYiDianJiSpeed(PlayerEnum.PlayerOne, 0);
				}
			}
		}
		
		switch (GameTypeCtrl.AppTypeStatic) {
		case AppGameType.DanJiTanKe:
			if (QiNangArray[4] == 1 || QiNangArray[5] == 1) {
				isMoveZuoYiP2 = true;
			}
			break;
		case AppGameType.LianJiTanKe:
		case AppGameType.LianJiFeiJi:
			if (QiNangArray[0] == 1 || QiNangArray[1] == 1 || QiNangArray[4] == 1) {
				isMoveZuoYiP2 = true;
			}
			break;
		}
		
		if (isMoveZuoYiP2) {
			if (ZuoYiDianJiSpeedVal[1] == 0x00) {
				Instance.SetZuoYiDianJiSpeed(PlayerEnum.PlayerTwo, 1);
			}
		}
		else {
			if (RunZuoYiState[1] == 0x00) {
				if (Instance != null) {
					Instance.SetZuoYiDianJiSpeed(PlayerEnum.PlayerTwo, 0);
				}
			}
		}
	}

	/**
单机版坦克8气囊.
****************.显示器.****************
QiNangArray[0]			QiNangArray[1]
QiNangArray[3]			QiNangArray[2]
****************.显示器.****************
QiNangArray[4]			QiNangArray[5]
QiNangArray[7]			QiNangArray[6]

联机版坦克8气囊.
******************.显示器.******************
QiNangArray[0]		QiNangArray[4]		QiNangArray[1]

QiNangArray[7]					        QiNangArray[5]

QiNangArray[3]		QiNangArray[6]		QiNangArray[2]
	 */
	public static byte[] QiNangArray = {0, 0, 0, 0, 0, 0, 0, 0};
	/**
	 * key == 0 -> 关闭动感,气囊.
	 * key == 1 -> 关闭气囊,停止座椅电机的运动.
	 */
	public static void CloseAllQiNangArray(int key = 0)
	{
		for (int i = 0; i < QiNangArray.Length;  i++) {
			QiNangArray[i] = 0;
		}

		switch(key) {
		case 0:
			DongGanState = 0;
			CheckMovePlayerZuoYi();
			break;
		case 1:
			if (Instance != null) {
				Instance.SetZuoYiDianJiSpeed(PlayerEnum.PlayerOne, 0);
				Instance.SetZuoYiDianJiSpeed(PlayerEnum.PlayerTwo, 0);
			}
			break;
		}
	}

	public static void OpenDongGanState()
	{
		DongGanState = 1;
	}
	
	static bool IsOpenQiNangQian;
	static bool IsOpenQiNangHou;
	static bool IsOpenQiNangZuo;
	static bool IsOpenQiNangYou;
	/**
	 * 单机版坦克8气囊.
	 * 		P1
	 * qn1		qn2
	 * qn4		qn3
	 * 
	 * 		P2
	 * qn5		qn6
	 * qn8		qn7
	 * 
	 * 联机版坦克8气囊.
	 * qn1		qn5		qn2
	 * 
	 * qn8				qn6
	 * 
	 * qn4		qn7		qn3
	 */
	public static void OpenQiNangQian()
	{
		if (IsOpenQiNangQian) {
			return;
		}
		IsOpenQiNangQian = true;

		switch (GameTypeCtrl.AppTypeStatic) {
		case AppGameType.LianJiTanKe:
			if (XkGameCtrl.IsActivePlayerOne || XkGameCtrl.IsActivePlayerTwo) {
				QiNangArray[0] = 1;
				QiNangArray[4] = 1;
				QiNangArray[1] = 1;
			}
			else {
				QiNangArray[0] = 0;
				QiNangArray[4] = 0;
				QiNangArray[1] = 0;
			}
			break;

		default:
			if (XkGameCtrl.IsActivePlayerOne) {
				QiNangArray[0] = 1;
				QiNangArray[1] = 1;
			}
			else {
				QiNangArray[0] = 0;
				QiNangArray[1] = 0;
			}
			
			if (XkGameCtrl.IsActivePlayerTwo) {
				QiNangArray[4] = 1;
				QiNangArray[5] = 1;
			}
			else {
				QiNangArray[4] = 0;
				QiNangArray[5] = 0;
			}
			break;
		}
		CheckMovePlayerZuoYi();
	}
	
	/**
	 * key == 0 -> 关闭前边全部气囊.
	 * key == 1 -> 关闭前边的左气囊.
	 * key == 2 -> 关闭前边的右气囊.
	 */
	public static void CloseQiNangQian(int key = 0)
	{
		if (!IsOpenQiNangQian) {
			return;
		}
		IsOpenQiNangQian = false;
		
		switch (GameTypeCtrl.AppTypeStatic) {
		case AppGameType.LianJiTanKe:
			switch (key) {
			case 1:
				QiNangArray[0] = 0;
				QiNangArray[4] = 0;
				break;
			case 2:
				QiNangArray[1] = 0;
				QiNangArray[4] = 0;
				break;
			default:
				QiNangArray[0] = 0;
				QiNangArray[4] = 0;
				QiNangArray[1] = 0;
				break;
			}
			break;
		
		default:
			switch (key) {
			case 1:
				QiNangArray[0] = 0;
				QiNangArray[4] = 0;
				break;
			case 2:
				QiNangArray[1] = 0;
				QiNangArray[5] = 0;
				break;
			default:
				QiNangArray[0] = 0;
				QiNangArray[1] = 0;
				QiNangArray[4] = 0;
				QiNangArray[5] = 0;
				break;
			}
			break;
		}
		CheckMovePlayerZuoYi();
	}
	
	public static void OpenQiNangHou()
	{
		if (IsOpenQiNangHou) {
			return;
		}
		IsOpenQiNangHou = true;
		
		switch (GameTypeCtrl.AppTypeStatic) {
		case AppGameType.LianJiTanKe:
			if (XkGameCtrl.IsActivePlayerOne || XkGameCtrl.IsActivePlayerTwo) {
				QiNangArray[3] = 1;
				QiNangArray[6] = 1;
				QiNangArray[2] = 1;
			}
			else {
				QiNangArray[3] = 0;
				QiNangArray[6] = 0;
				QiNangArray[2] = 0;
			}
			break;
			
		default:
			if (XkGameCtrl.IsActivePlayerOne) {
				QiNangArray[2] = 1;
				QiNangArray[3] = 1;
			}
			else {
				QiNangArray[2] = 0;
				QiNangArray[3] = 0;
			}

			if (XkGameCtrl.IsActivePlayerTwo) {
				QiNangArray[6] = 1;
				QiNangArray[7] = 1;
			}
			else {
				QiNangArray[6] = 0;
				QiNangArray[7] = 0;
			}
			break;
		}
		CheckMovePlayerZuoYi();
	}
	
	/**
	 * key == 0 -> 关闭前边全部气囊.
	 * key == 1 -> 关闭前边的左气囊.
	 * key == 2 -> 关闭前边的右气囊.
	 */
	public static void CloseQiNangHou(int key = 0)
	{
		if (!IsOpenQiNangHou) {
			return;
		}
		IsOpenQiNangHou = false;
		
		switch (GameTypeCtrl.AppTypeStatic) {
		case AppGameType.LianJiTanKe:
			switch (key) {
			case 1:
				QiNangArray[3] = 0;
				QiNangArray[6] = 0;
				break;
			case 2:
				QiNangArray[2] = 0;
				QiNangArray[6] = 0;
				break;
			default:
				QiNangArray[3] = 0;
				QiNangArray[6] = 0;
				QiNangArray[2] = 0;
				break;
			}
			break;
			
		default:
			switch (key) {
			case 1:
				QiNangArray[3] = 0;
				QiNangArray[7] = 0;
				break;
			case 2:
				QiNangArray[2] = 0;
				QiNangArray[6] = 0;
				break;
			default:
				QiNangArray[2] = 0;
				QiNangArray[3] = 0;
				QiNangArray[6] = 0;
				QiNangArray[7] = 0;
				break;
			}
			break;
		}
		CheckMovePlayerZuoYi();
	}
	
	public static void OpenQiNangZuo()
	{
		if (IsOpenQiNangZuo) {
			return;
		}
		IsOpenQiNangZuo = true;
		
		switch (GameTypeCtrl.AppTypeStatic) {
		case AppGameType.LianJiTanKe:
			if (XkGameCtrl.IsActivePlayerOne || XkGameCtrl.IsActivePlayerTwo) {
				QiNangArray[0] = 1;
				QiNangArray[7] = 1;
				QiNangArray[3] = 1;
			}
			else {
				QiNangArray[0] = 0;
				QiNangArray[7] = 0;
				QiNangArray[3] = 0;
			}
			break;
			
		default:
			if (XkGameCtrl.IsActivePlayerOne) {
				QiNangArray[0] = 1;
				QiNangArray[3] = 1;
			}
			else {
				QiNangArray[0] = 0;
				QiNangArray[3] = 0;
			}

			if (XkGameCtrl.IsActivePlayerTwo) {
				QiNangArray[4] = 1;
				QiNangArray[7] = 1;
			}
			else {
				QiNangArray[4] = 0;
				QiNangArray[7] = 0;
			}
			break;
		}
		CheckMovePlayerZuoYi();
	}

	/**
	 * key == 0 -> 关闭左边全部气囊.
	 * key == 1 -> 关闭左边的前气囊.
	 * key == 2 -> 关闭左边的后气囊.
	 */
	public static void CloseQiNangZuo(int key = 0)
	{
		if (!IsOpenQiNangZuo) {
			return;
		}
		IsOpenQiNangZuo = false;
		
		switch (GameTypeCtrl.AppTypeStatic) {
		case AppGameType.LianJiTanKe:
			switch (key) {
			case 1:
				QiNangArray[0] = 0;
				QiNangArray[7] = 0;
				break;
			case 2:
				QiNangArray[3] = 0;
				QiNangArray[7] = 0;
				break;
			default:
				QiNangArray[0] = 0;
				QiNangArray[7] = 0;
				QiNangArray[3] = 0;
				break;
			}
			break;
			
		default:
			switch (key) {
			case 1:
				QiNangArray[0] = 0;
				QiNangArray[4] = 0;
				break;
			case 2:
				QiNangArray[3] = 0;
				QiNangArray[7] = 0;
				break;
			default:
				QiNangArray[0] = 0;
				QiNangArray[3] = 0;
				QiNangArray[4] = 0;
				QiNangArray[7] = 0;
				break;
			}
			break;
		}
		CheckMovePlayerZuoYi();
	}
	
	public static void OpenQiNangYou()
	{
		if (IsOpenQiNangYou) {
			return;
		}
		IsOpenQiNangYou = true;
		
		switch (GameTypeCtrl.AppTypeStatic) {
		case AppGameType.LianJiTanKe:
			if (XkGameCtrl.IsActivePlayerOne || XkGameCtrl.IsActivePlayerTwo) {
				QiNangArray[1] = 1;
				QiNangArray[5] = 1;
				QiNangArray[2] = 1;
			}
			else {
				QiNangArray[1] = 0;
				QiNangArray[5] = 0;
				QiNangArray[2] = 0;
			}
			break;
			
		default:
			if (XkGameCtrl.IsActivePlayerOne) {
				QiNangArray[1] = 1;
				QiNangArray[2] = 1;
			}
			else {
				QiNangArray[1] = 0;
				QiNangArray[2] = 0;
			}
			
			if (XkGameCtrl.IsActivePlayerTwo) {
				QiNangArray[5] = 1;
				QiNangArray[6] = 1;
			}
			else {
				QiNangArray[5] = 0;
				QiNangArray[6] = 0;
			}
			break;
		}
		CheckMovePlayerZuoYi();
	}
	
	/**
	 * key == 0 -> 关闭右边全部气囊.
	 * key == 1 -> 关闭右边的前气囊.
	 * key == 2 -> 关闭右边的后气囊.
	 */
	public static void CloseQiNangYou(int key = 0)
	{
		if (!IsOpenQiNangYou) {
			return;
		}
		IsOpenQiNangYou = false;
		
		switch (GameTypeCtrl.AppTypeStatic) {
		case AppGameType.LianJiTanKe:
			switch (key) {
			case 1:
				QiNangArray[1] = 0;
				QiNangArray[5] = 0;
				break;
			case 2:
				QiNangArray[2] = 0;
				QiNangArray[5] = 0;
				break;
			default:
				QiNangArray[1] = 0;
				QiNangArray[5] = 0;
				QiNangArray[2] = 0;
				break;
			}
			break;
			
		default:
			switch (key) {
			case 1:
				QiNangArray[1] = 0;
				QiNangArray[5] = 0;
				break;
			case 2:
				QiNangArray[2] = 0;
				QiNangArray[6] = 0;
				break;
			default:
				QiNangArray[1] = 0;
				QiNangArray[2] = 0;
				QiNangArray[5] = 0;
				QiNangArray[6] = 0;
				break;
			}
			break;
		}
		CheckMovePlayerZuoYi();
	}
	
	public static bool IsPlayerHitShake;
	public void OnPlayerHitShake()
	{
		if (IsPlayerHitShake) {
			return;
		}
		IsPlayerHitShake = true;
		//Debug.Log("OnPlayerHitShake...");
		StartCoroutine(PcvrQiNangHitShake());
	}
	
	void ClosePlayerHitShake()
	{
		if (!IsPlayerHitShake) {
			return;
		}
		IsPlayerHitShake = false;
		//Debug.Log("ClosePlayerHitShake...");
	}
	
	IEnumerator PcvrQiNangHitShake()
	{
		bool isHitShake = true;
		int count = 0;
		do {
			if (count % 2 == 0) {
				OpenQiNangZuo();
				CloseQiNangYou();
			}
			else {
				OpenQiNangYou();
				CloseQiNangZuo();
			}
			yield return new WaitForSeconds(0.25f);
			
			if (count >= 4) {
				isHitShake = false;
				ClosePlayerHitShake();
				yield break;
			}
			count++;
		} while (isHitShake);
	}

	const float TimeDisJiHuoGunDouDong = 0.1f;
	const float TimeDisJiQiangZiDan = 0.01f;
	const float TimeDisGaoBaoDan = 0.08f;
	float[] LastFireTimeArray = {0f, 0f};
	float[] LastFireActiveTimeArray = {0f, 0f};
	void SendMessage()
	{
		if (!MyCOMDevice.IsFindDeviceDt) {
			return;
		}
		
		byte []buffer;
		buffer = new byte[HID_BUF_LEN_WRITE];
		buffer[0] = WriteHead_1;
		buffer[1] = WriteHead_2;
		buffer[HID_BUF_LEN_WRITE - 2] = WriteEnd_1;
		buffer[HID_BUF_LEN_WRITE - 1] = WriteEnd_2;
		switch (MyCOMDevice.PcvrComSt) {
		case PcvrComState.TanKeFangXiangZhenDong:
		{
			#if !COM_TANK_TEST
			for (int i = 5; i < HID_BUF_LEN_WRITE - 2; i++) {
				buffer[i] = (byte)(UnityEngine.Random.Range(0, 10000) % 256);
			}
			
			if (IsSubPlayerCoin) {
				buffer[2] = 0xaa;
				buffer[3] = (byte)subCoinNum12;
				//buffer[4] = (byte)subCoinNum34;
				//ScreenLog.Log("sub coinP12 "+subCoinNum12.ToString("X2")+", coinP34 "+subCoinNum34.ToString("X2"));
			}
			
			switch (StartLightStateP1) {
			case LedState.Liang:
				buffer[5] |= 0x01;
				break;
				
			case LedState.Shan:
				buffer[5] |= 0x01;
				break;
				
			case LedState.Mie:
				buffer[5] &= 0xfe;
				break;
			}
			
			switch (StartLightStateP2) {
			case LedState.Liang:
				buffer[5] |= 0x02;
				break;
				
			case LedState.Shan:
				buffer[5] |= 0x02;
				break;
				
			case LedState.Mie:
				buffer[5] &= 0xfd;
				break;
			}
			buffer[5] &= 0xe3; //close led3-5.
			
			if (DongGanState == 1 || HardwareCheckCtrl.IsTestHardWare) {
				buffer[6] = (byte)(QiNangArray[0]
				                   + (QiNangArray[1] << 1)
				                   + (QiNangArray[2] << 2)
				                   + (QiNangArray[3] << 3)
				                   + (QiNangArray[4] << 4)
				                   + (QiNangArray[5] << 5)
				                   + (QiNangArray[6] << 6)
				                   + (QiNangArray[7] << 7));
			}
			else {
				buffer[6] = 0x00;
				
				if (RunZuoYiState[0] == 0x00) {
					SetRunZuoYiState(PlayerEnum.PlayerOne, 1);
				}
				
				if (RunZuoYiState[1] == 0x00) {
					SetRunZuoYiState(PlayerEnum.PlayerTwo, 1);
				}
			}
			
			if (IsJiaoYanHid) {
				for (int i = 0; i < 4; i++) {
					buffer[i + 21] = JiaoYanMiMa[i];
				}
				
				for (int i = 0; i < 4; i++) {
					buffer[i + 25] = JiaoYanDt[i];
				}
			}
			else {
				RandomJiaoYanMiMaVal();
				for (int i = 0; i < 4; i++) {
					buffer[i + 21] = JiaoYanMiMaRand[i];
				}
				
				//0x41 0x42 0x43 0x44
				for (int i = 26; i < 29; i++) {
					buffer[i] = (byte)UnityEngine.Random.Range(0x00, 0x40);
				}
				buffer[25] = 0x00;
				
				for (int i = 26; i < 29; i++) {
					buffer[25] ^= buffer[i];
				}
			}

			if (!HardwareCheckCtrl.IsTestHardWare) {
				if (SetPanelUiRoot.IsOpenSetPanel) {
					buffer[8] = GetPlayerGunZhenDongVal(QiangZhenDongP1);
					buffer[9] = GetPlayerGunZhenDongVal(QiangZhenDongP2);
				}
				else {
					if (IsCloseGunZhenDong) {
						buffer[8] = 0x00;
						buffer[9] = 0x00;
					}
					else {
						float timeDisVal = TimeDisJiQiangZiDan;
						float timeActiveDisVal = TimeDisJiHuoGunDouDong;
						if (XkGameCtrl.IsActivePlayerOne) {
							if (XkGameCtrl.GaoBaoDanNumPOne > 0) {
								timeDisVal = TimeDisGaoBaoDan;
							}
							
							if (Time.realtimeSinceStartup - LastFireTimeArray[0] < timeDisVal) {
								LastFireActiveTimeArray[0] = Time.realtimeSinceStartup;
								buffer[8] = 0x00;
							}
							else {
								if (Time.realtimeSinceStartup - LastFireActiveTimeArray[0] >= timeActiveDisVal) {
									LastFireTimeArray[0] = Time.realtimeSinceStartup;
								}
								buffer[8] = GetPlayerGunZhenDongVal(QiangZhenDongP1);
							}
						}
						else {
							buffer[8] = 0x00;
						}
						
						if (XkGameCtrl.IsActivePlayerTwo) {
							timeDisVal = TimeDisJiQiangZiDan;
							if (XkGameCtrl.GaoBaoDanNumPTwo > 0) {
								timeDisVal = TimeDisGaoBaoDan;
							}
							
							if (Time.realtimeSinceStartup - LastFireTimeArray[1] < timeDisVal) {
								LastFireActiveTimeArray[1] = Time.realtimeSinceStartup;
								buffer[9] = 0x00;
							}
							else {
								if (Time.realtimeSinceStartup - LastFireActiveTimeArray[1] >= timeActiveDisVal) {
									LastFireTimeArray[1] = Time.realtimeSinceStartup;
								}
								buffer[9] = GetPlayerGunZhenDongVal(QiangZhenDongP2);
							}
						}
						else {
							buffer[9] = 0x00;
						}
					}
				}
			}
			else {
				buffer[8] = (byte)QiangZhenDongP1;
				buffer[9] = (byte)QiangZhenDongP2;
			}

			if (HardwareCheckCtrl.IsTestHardWare) {
				buffer[10] = (byte)QiangZhenDongP1;
				buffer[11] = (byte)QiangZhenDongP2;
			}
			else {
				buffer[10] = 0x00;
				buffer[11] = 0x00;
			}

			if (JiFenJieMianCtrl.GetInstance() != null && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
				buffer[12] = 0x00;
				buffer[13] = 0x00;
				buffer[14] = 0x00;
				buffer[15] = 0x00;
			}
			else {
				if (!HardwareCheckCtrl.IsTestHardWare) {
					buffer[12] = XkGameCtrl.IsActivePlayerOne == true ? ZuoYiDianJiSpeedVal[0] : (byte)0x00;
					buffer[13] = XkGameCtrl.IsActivePlayerTwo == true ? ZuoYiDianJiSpeedVal[1] : (byte)0x00;
					buffer[14] = XkGameCtrl.IsActivePlayerThree == true ? ZuoYiDianJiSpeedVal[2] : (byte)0x00;
					buffer[15] = XkGameCtrl.IsActivePlayerFour == true ? ZuoYiDianJiSpeedVal[3] : (byte)0x00;
				}
				else {
					buffer[12] = ZuoYiDianJiSpeedVal[0];
					buffer[13] = ZuoYiDianJiSpeedVal[1];
					buffer[14] = ZuoYiDianJiSpeedVal[2];
					buffer[15] = ZuoYiDianJiSpeedVal[3];
				}
			}

			if (!IsRecordTimeFangXiangPan) {
				IsRecordTimeFangXiangPan = true;
				TimeFangXiangPan = Time.realtimeSinceStartup;
				buffer[16] = 0xaa;
				buffer[17] = 0xaa;
			}
			else {
				if (Time.realtimeSinceStartup - TimeFangXiangPan < 60f) {
					buffer[16] = 0xaa;
					buffer[17] = 0xaa;
				}
				else {
					if (HardwareCheckCtrl.IsTestHardWare) {
						buffer[16] = FangXiangPanDouDongVal[0];
						buffer[17] = FangXiangPanDouDongVal[1];
					}
					else {
						buffer[16] = XkGameCtrl.IsActivePlayerOne == true ? FangXiangPanDouDongVal[0] : (byte)0x00;
						buffer[17] = XkGameCtrl.IsActivePlayerTwo == true ? FangXiangPanDouDongVal[1] : (byte)0x00;
					}
				}
			}
			buffer[18] = 0x00;
			buffer[19] = 0x00;
			buffer[20] = 0x00;
			for (int i = 2; i <= 11; i++) {
				buffer[20] ^= buffer[i];
			}
			
			buffer[29] = 0x00;
			for (int i = 0; i < HID_BUF_LEN_WRITE; i++) {
				if (i == 29) {
					continue;
				}
				buffer[29] ^= buffer[i];
			}
			#endif
		}
		break;
		case PcvrComState.TanKeGunZhenDong:
		{
			#if COM_TANK_TEST || COM_FEIJI_TX
			for (int i = 4; i < HID_BUF_LEN_WRITE - 2; i++) {
				buffer[i] = (byte)(UnityEngine.Random.Range(0, 10000) % 256);
			}
			
			if (IsSubPlayerCoin) {
				buffer[2] = 0xaa;
				buffer[3] = (byte)subCoinNum12;
			}
			
			switch (StartLightStateP1) {
			case LedState.Liang:
				buffer[4] |= 0x01;
				break;
				
			case LedState.Shan:
				buffer[4] |= 0x01;
				break;
				
			case LedState.Mie:
				buffer[4] &= 0xfe;
				break;
			}
			
			switch (StartLightStateP2) {
			case LedState.Liang:
				buffer[4] |= 0x02;
				break;
				
			case LedState.Shan:
				buffer[4] |= 0x02;
				break;
				
			case LedState.Mie:
				buffer[4] &= 0xfd;
				break;
			}
			
			if (DongGanState == 1 || HardwareCheckCtrl.IsTestHardWare) {
				buffer[5] = (byte)(QiNangArray[0]
				                   + (QiNangArray[1] << 1)
				                   + (QiNangArray[2] << 2)
				                   + (QiNangArray[3] << 3)
				                   + (QiNangArray[4] << 4)
				                   + (QiNangArray[5] << 5)
				                   + (QiNangArray[6] << 6)
				                   + (QiNangArray[7] << 7));
			}
			else {
				buffer[5] = 0x00;
				
				if (RunZuoYiState[0] == 0x00) {
					SetRunZuoYiState(PlayerEnum.PlayerOne, 1);
				}

				if (RunZuoYiState[1] == 0x00) {
					SetRunZuoYiState(PlayerEnum.PlayerTwo, 1);
				}
			}
			
			if (IsJiaoYanHid) {
				for (int i = 0; i < 4; i++) {
					buffer[i + 10] = JiaoYanMiMa[i];
				}
				
				for (int i = 0; i < 4; i++) {
					buffer[i + 14] = JiaoYanDt[i];
				}
			}
			else {
				RandomJiaoYanMiMaVal();
				for (int i = 0; i < 4; i++) {
					buffer[i + 10] = JiaoYanMiMaRand[i];
				}
				
				//0x41 0x42 0x43 0x44
				for (int i = 15; i < 18; i++) {
					buffer[i] = (byte)UnityEngine.Random.Range(0x00, 0x40);
				}
				buffer[14] = 0x00;
				
				for (int i = 15; i < 18; i++) {
					buffer[14] ^= buffer[i];
				}
			}

			if (!HardwareCheckCtrl.IsTestHardWare) {
				if (SetPanelUiRoot.IsOpenSetPanel) {
					buffer[8] = GetPlayerGunZhenDongVal(QiangZhenDongP1);
					buffer[9] = GetPlayerGunZhenDongVal(QiangZhenDongP2);
				}
				else {
					if (IsCloseGunZhenDong) {
						buffer[8] = 0x00;
						buffer[9] = 0x00;
					}
					else {
						float timeDisVal = TimeDisJiQiangZiDan;
						float timeActiveDisVal = TimeDisJiHuoGunDouDong;
						if (XkGameCtrl.IsActivePlayerOne) {
							if (XkGameCtrl.GaoBaoDanNumPOne > 0) {
								timeDisVal = TimeDisGaoBaoDan;
							}
							
							if (Time.realtimeSinceStartup - LastFireTimeArray[0] < timeDisVal) {
								LastFireActiveTimeArray[0] = Time.realtimeSinceStartup;
								buffer[8] = 0x00;
							}
							else {
								if (Time.realtimeSinceStartup - LastFireActiveTimeArray[0] >= timeActiveDisVal) {
									LastFireTimeArray[0] = Time.realtimeSinceStartup;
								}
								buffer[8] = GetPlayerGunZhenDongVal(QiangZhenDongP1);
							}
						}
						else {
							buffer[8] = 0x00;
						}
						
						if (XkGameCtrl.IsActivePlayerTwo) {
							timeDisVal = TimeDisJiQiangZiDan;
							if (XkGameCtrl.GaoBaoDanNumPTwo > 0) {
								timeDisVal = TimeDisGaoBaoDan;
							}
							
							if (Time.realtimeSinceStartup - LastFireTimeArray[1] < timeDisVal) {
								LastFireActiveTimeArray[1] = Time.realtimeSinceStartup;
								buffer[9] = 0x00;
							}
							else {
								if (Time.realtimeSinceStartup - LastFireActiveTimeArray[1] >= timeActiveDisVal) {
									LastFireTimeArray[1] = Time.realtimeSinceStartup;
								}
								buffer[9] = GetPlayerGunZhenDongVal(QiangZhenDongP2);
							}
						}
						else {
							buffer[9] = 0x00;
						}
					}
				}
			}
			else {
				buffer[8] = (byte)QiangZhenDongP1;
				buffer[9] = (byte)QiangZhenDongP2;
			}
			
			buffer[6] = 0x00;
			for (int i = 2; i <= 11; i++) {
				if (i == 6) {
					continue;
				}
				buffer[6] ^= buffer[i];
			}
			
			buffer[19] = 0x00;
			for (int i = 0; i < HID_BUF_LEN_WRITE; i++) {
				if (i == 19) {
					continue;
				}
				buffer[19] ^= buffer[i];
			}
			#endif
		}
		break;
		}
		MyCOMDevice.ComThreadClass.WriteByteMsg = buffer;
	}
	public static float TimeFangXiangPan;
	bool IsRecordTimeFangXiangPan;
	
	void GetMessage()
	{
		if (!MyCOMDevice.ComThreadClass.IsReadComMsg) {
			return;
		}
		
		if (MyCOMDevice.ComThreadClass.IsReadMsgComTimeOut) {
			return;
		}
		
		if (MyCOMDevice.ComThreadClass.ReadByteMsg.Length < (MyCOMDevice.ComThreadClass.BufLenRead - MyCOMDevice.ComThreadClass.BufLenReadEnd)) {
			//Debug.Log("ReadBufLen was wrong! len is "+MyCOMDevice.ComThreadClass.ReadByteMsg.Length);
			return;
		}
		
		if (IsJiOuJiaoYanFailed) {
			return;
		}

		switch (MyCOMDevice.PcvrComSt) {
		case PcvrComState.TanKeFangXiangZhenDong:
		{
			#if !COM_TANK_TEST
			if ((MyCOMDevice.ComThreadClass.ReadByteMsg[34]&0x01) == 0x01) {
				JiOuJiaoYanCount++;
				if (JiOuJiaoYanCount >= JiOuJiaoYanMax && !IsJiOuJiaoYanFailed) {
					IsJiOuJiaoYanFailed = true;
					//JiOuJiaoYanFailed
				}
			}
			//IsJiOuJiaoYanFailed = true; //test
			byte tmpVal = 0x00;
			string testA = "";
			for (int i = 0; i < (MyCOMDevice.ComThreadClass.BufLenRead - 4); i++) {
				if (i == 18 || i == 19 || i == 33) {
					continue;
				}
				testA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
				tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
			}
			tmpVal ^= EndRead_1;
			tmpVal ^= EndRead_2;
			tmpVal ^= EndRead_3;
			tmpVal ^= EndRead_4;
			testA += EndRead_1 + " ";
			testA += EndRead_2 + " ";
			testA += EndRead_3 + " ";
			testA += EndRead_4 + " ";
			
			if (tmpVal != MyCOMDevice.ComThreadClass.ReadByteMsg[33]) {
				if (MyCOMDevice.ComThreadClass.IsStopComTX) {
					return;
				}
				MyCOMDevice.ComThreadClass.IsStopComTX = true;
				ScreenLog.Log("testA: "+testA);
				ScreenLog.LogError("tmpVal: "+tmpVal.ToString("X2")+", byte[33] "+MyCOMDevice.ComThreadClass.ReadByteMsg[33].ToString("X2"));
				ScreenLog.Log("byte[33] was wrong!");
				return;
			}
			
			if (IsJiaoYanHid) {
				tmpVal = 0x00;
				//string testStrA = MyCOMDevice.ComThreadClass.ReadByteMsg[30].ToString("X2") + " ";
				string testStrB = "";
				string testStrA = "";
				//			for (int i = 0; i < MyCOMDevice.ComThreadClass.ReadByteMsg.Length; i++) {
				//				testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
				//			}
				//			ScreenLog.Log("readStr: "+testStrA);
				
				//			for (int i = 0; i < JiaoYanDt.Length; i++) {
				//				testStrB += JiaoYanDt[i].ToString("X2") + " ";
				//			}
				//			ScreenLog.Log("GameSendDt: "+testStrB);
				
				//			string testStrC = "";
				//			for (int i = 0; i < JiaoYanDt.Length; i++) {
				//				testStrC += JiaoYanMiMa[i].ToString("X2") + " ";
				//			}
				//			ScreenLog.Log("GameSendMiMa: "+testStrC);
				
				for (int i = 26; i < 29; i++) {
					tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
					testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
				}
				
				if (tmpVal == MyCOMDevice.ComThreadClass.ReadByteMsg[25]) {
					bool isJiaoYanDtSucceed = false;
					tmpVal = 0x00;
					for (int i = 30; i < 33; i++) {
						tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
					}
					
					//校验2...
					if ( tmpVal == MyCOMDevice.ComThreadClass.ReadByteMsg[29]
					    && (JiaoYanDt[1]&0xef) == MyCOMDevice.ComThreadClass.ReadByteMsg[30]
					    && (JiaoYanDt[2]&0xfe) == MyCOMDevice.ComThreadClass.ReadByteMsg[31]
					    && (JiaoYanDt[3]|0x28) == MyCOMDevice.ComThreadClass.ReadByteMsg[32] ) {
						isJiaoYanDtSucceed = true;
					}
					
					JiaoYanCheckCount++;
					if (isJiaoYanDtSucceed) {
						//JiaMiJiaoYanSucceed
						OnEndJiaoYanIO(JIAOYANENUM.SUCCEED);
						//ScreenLog.Log("JMJYCG...");
					}
					else {
						if (JiaoYanCheckCount > 0) {
							OnEndJiaoYanIO(JIAOYANENUM.FAILED);
						}
						testStrA = "";
						for (int i = 0; i < 35; i++) {
							testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
						}
						
						testStrB = "";
						for (int i = 29; i < 33; i++) {
							testStrB += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
						}
						
						string testStrD = "";
						for (int i = 0; i < 4; i++) {
							testStrD += JiaoYanDt[i].ToString("X2") + " ";
						}
						ScreenLog.Log("JiaoYan2 ShiBai...");
						ScreenLog.Log("ReadByte[0 - 35] "+testStrA);
						ScreenLog.Log("ReadByte[29 - 32] "+testStrB);
						ScreenLog.Log("SendByte[25 - 28] "+testStrD);
						ScreenLog.LogError("校验数据错误!");
					}
				}
				else {
					ScreenLog.Log("JiaoYan1 ShiBai...");
					testStrB = "byte[25] "+MyCOMDevice.ComThreadClass.ReadByteMsg[25].ToString("X2")+" "
						+", tmpVal "+tmpVal.ToString("X2");
					ScreenLog.Log("ReadByte[25 - 28] "+testStrA);
					ScreenLog.Log(testStrB);
					ScreenLog.LogError("ReadByte[25] was wrong!");
				}
			}
			#endif
		}
		break;

		case PcvrComState.TanKeGunZhenDong:
		{
			#if COM_TANK_TEST || COM_FEIJI_TX
			if ((MyCOMDevice.ComThreadClass.ReadByteMsg[22]&0x01) == 0x01) {
				JiOuJiaoYanCount++;
				if (JiOuJiaoYanCount >= JiOuJiaoYanMax && !IsJiOuJiaoYanFailed) {
					IsJiOuJiaoYanFailed = true;
					//JiOuJiaoYanFailed
				}
			}
			//IsJiOuJiaoYanFailed = true; //test

			byte tmpVal = 0x00;
			string testA = "";
			for (int i = 2; i < (MyCOMDevice.ComThreadClass.BufLenRead - 4); i++) {
				if (i == 18 || i == 21) {
					continue;
				}
				testA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
				tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
			}
			tmpVal ^= EndRead_1;
			tmpVal ^= EndRead_2;
			testA += EndRead_1 + " ";
			testA += EndRead_2 + " ";
			testA += EndRead_3 + " ";
			testA += EndRead_4 + " ";
			
			if (tmpVal != MyCOMDevice.ComThreadClass.ReadByteMsg[21]) {
				if (MyCOMDevice.ComThreadClass.IsStopComTX) {
					return;
				}
				MyCOMDevice.ComThreadClass.IsStopComTX = true;
	//			ScreenLog.Log("testA: "+testA);
	//			ScreenLog.LogError("tmpVal: "+tmpVal.ToString("X2")+", byte[21] "+MyCOMDevice.ComThreadClass.ReadByteMsg[21].ToString("X2"));
	//			ScreenLog.Log("byte21 was wrong!");
				return;
			}
			
			if (IsJiaoYanHid) {
				tmpVal = 0x00;
				//string testStrA = MyCOMDevice.ComThreadClass.ReadByteMsg[30].ToString("X2") + " ";
				//			string testStrB = "";
				//			string testStrA = "";
				//			for (int i = 0; i < MyCOMDevice.ComThreadClass.ReadByteMsg.Length; i++) {
				//				testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
				//			}
				//			ScreenLog.Log("readStr: "+testStrA);
				//
				//			for (int i = 0; i < JiaoYanDt.Length; i++) {
				//				testStrB += JiaoYanDt[i].ToString("X2") + " ";
				//			}
				//			ScreenLog.Log("GameSendDt: "+testStrB);
				//
				//			string testStrC = "";
				//			for (int i = 0; i < JiaoYanDt.Length; i++) {
				//				testStrC += JiaoYanMiMa[i].ToString("X2") + " ";
				//			}
				//			ScreenLog.Log("GameSendMiMa: "+testStrC);
				
				for (int i = 11; i < 14; i++) {
					tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
					//testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
				}
				
				if (tmpVal == MyCOMDevice.ComThreadClass.ReadByteMsg[10]) {
					bool isJiaoYanDtSucceed = false;
					tmpVal = 0x00;
					for (int i = 15; i < 18; i++) {
						tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
					}
					
					//Ð£Ñé2...
					if ( tmpVal == MyCOMDevice.ComThreadClass.ReadByteMsg[14]
					    && (JiaoYanDt[1]&0xef) == MyCOMDevice.ComThreadClass.ReadByteMsg[15]
					    && (JiaoYanDt[2]&0xfe) == MyCOMDevice.ComThreadClass.ReadByteMsg[16]
					    && (JiaoYanDt[3]|0x28) == MyCOMDevice.ComThreadClass.ReadByteMsg[17] ) {
						isJiaoYanDtSucceed = true;
					}
					
					JiaoYanCheckCount++;
					if (isJiaoYanDtSucceed) {
						//JiaMiJiaoYanSucceed
						OnEndJiaoYanIO(JIAOYANENUM.SUCCEED);
						//ScreenLog.Log("JMJYCG...");
					}
					else {
						if (JiaoYanCheckCount > 0) {
							OnEndJiaoYanIO(JIAOYANENUM.FAILED);
						}
	//					testStrA = "";
	//					for (int i = 0; i < 23; i++) {
	//						testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
	//					}
	//
	//					testStrB = "";
	//					for (int i = 14; i < 18; i++) {
	//						testStrB += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
	//					}
	//					
	//					string testStrD = "";
	//					for (int i = 0; i < 4; i++) {
	//						testStrD += JiaoYanDt[i].ToString("X2") + " ";
	//					}
	//					ScreenLog.Log("ReadByte[0 - 22] "+testStrA);
	//					ScreenLog.Log("ReadByte[14 - 17] "+testStrB);
	//					ScreenLog.Log("SendByte[14 - 17] "+testStrD);
	//					ScreenLog.LogError("Ð£ÑéÊý¾Ý´íÎó!");
					}
				}
	//			else {
	//				testStrB = "byte[10] "+MyCOMDevice.ComThreadClass.ReadByteMsg[10].ToString("X2")+" "
	//					+", tmpVal "+tmpVal.ToString("X2");
	//				ScreenLog.Log("ReadByte[10 - 13] "+testStrA);
	//				ScreenLog.Log(testStrB);
	//				ScreenLog.LogError("ReadByte[10] was wrong!");
	//			}
			}
			#endif
		}
		break;
		}
		
		int len = MyCOMDevice.ComThreadClass.ReadByteMsg.Length;
		uint[] readMsg = new uint[len];
		for (int i = 0; i < len; i++) {
			readMsg[i] = MyCOMDevice.ComThreadClass.ReadByteMsg[i];
		}
		keyProcess(readMsg);
	}
	
	public static byte DongGanState = 0;
	void keyProcess(uint []buffer)
	{
		switch (MyCOMDevice.PcvrComSt) {
		case PcvrComState.TanKeFangXiangZhenDong:
		{
			#if !COM_TANK_TEST
			MousePositionP1.x = ((buffer[2] & 0x0f) << 8) + buffer[3];
			MousePositionP1.y = ((buffer[4] & 0x0f) << 8) + buffer[5];
			MousePositionP2.x = ((buffer[6] & 0x0f) << 8) + buffer[7];
			MousePositionP2.y = ((buffer[8] & 0x0f) << 8) + buffer[9];                                                           
			
			//game coinInfo
			CoinCurPcvr12 = buffer[18];
			uint coinP1 = CoinCurPcvr12 & 0x0f;
			uint coinP2 = (CoinCurPcvr12 & 0xf0) >> 4;
			//		CoinCurPcvr34 = buffer[19];
			//		uint coinP3 = CoinCurPcvr34 & 0x0f;
			//		uint coinP4 = (CoinCurPcvr34 & 0xf0) >> 4;
			//coinP2 = coinP1; //test
			if (IsSubPlayerCoin) {
				if (coinP1 == 0 && IsSubCoinP1) {
					IsSubCoinP1 = false;
					IsSubPlayerCoin = false;
					subCoinNum12 = 0;
				}
				
				if (coinP2 == 0 && IsSubCoinP2) {
					IsSubCoinP2 = false;
					IsSubPlayerCoin = false;
					subCoinNum12 = 0;
				}
				
	//			if (coinP3 == 0 && IsSubCoinP3) {
	//				ScreenLog.Log("sub coinP3 "+coinP3);
	//				IsSubCoinP3 = false;
	//				IsSubPlayerCoin = false;
	//				subCoinNum34 = 0;
	//			}
	//			
	//			if (coinP4 == 0 && IsSubCoinP4) {
	//				IsSubCoinP4 = false;
	//				IsSubPlayerCoin = false;
	//				subCoinNum34 = 0;
	//			}
			}
			else {
				if (coinP1 > 0 && coinP1 < 256) {
					IsSubCoinP1 = true;
					CoinNumCurrentP1 += (int)coinP1;
					XKGlobalData.SetCoinPlayerOne(CoinNumCurrentP1);
					SubPcvrCoin(PlayerEnum.PlayerOne, (int)(CoinCurPcvr12 & 0x0f));
				}
				
				if (coinP2 > 0 && coinP2 < 256) {
					IsSubCoinP2 = true;
					CoinNumCurrentP2 += (int)coinP2;
					XKGlobalData.SetCoinPlayerTwo(CoinNumCurrentP2);
					SubPcvrCoin(PlayerEnum.PlayerTwo, (int)(CoinCurPcvr12 & 0xf0));
				}
				
//				if (coinP3 > 0 && coinP3 < 256) {
//					ScreenLog.Log("coinP3 "+coinP3);
//					IsSubCoinP3 = true;
//					CoinNumCurrentP3 += (int)coinP3;
//					XKGlobalData.SetCoinPlayerThree(CoinNumCurrentP3);
//					SubPcvrCoin(PlayerEnum.PlayerTwo, (int)(CoinCurPcvr34 & 0x0f));
//				}
//				
//				if (coinP4 > 0 && coinP4 < 256) {
//					IsSubCoinP4 = true;
//					CoinNumCurrentP4 += (int)coinP4;
//					XKGlobalData.SetCoinPlayerFour(CoinNumCurrentP4);
//					SubPcvrCoin(PlayerEnum.PlayerFour, (int)(CoinCurPcvr34 & 0xf0));
//				}
			}
			
			//test
			//buffer[23] = (byte)(UnityEngine.Random.Range(0, 100) % 16);
			//buffer[24] = (byte)(UnityEngine.Random.Range(0, 100) % 16);
			if ((buffer[23]&0x01) == 0x01 && ZuoYiTrigger[0] == 0) {
				ZuoYiTrigger[0] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerOne, 1, ButtonState.DOWN);
			}
			else if ((buffer[23]&0x01) == 0x00 && ZuoYiTrigger[0] == 1) {
				ZuoYiTrigger[0] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerOne, 1, ButtonState.UP);
			}
			
			if ((buffer[23]&0x02) == 0x02 && ZuoYiTrigger[1] == 0) {
				ZuoYiTrigger[1] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerOne, 0, ButtonState.DOWN);
			}
			else if ((buffer[23]&0x02) == 0x00 && ZuoYiTrigger[1] == 1) {
				ZuoYiTrigger[1] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerOne, 0, ButtonState.UP);
			}
			
			if ((buffer[23]&0x04) == 0x04 && ZuoYiTrigger[2] == 0) {
				ZuoYiTrigger[2] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerOne, -1, ButtonState.DOWN);
			}
			else if ((buffer[23]&0x04) == 0x00 && ZuoYiTrigger[2] == 1) {
				ZuoYiTrigger[2] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerOne, -1, ButtonState.UP);
			}
			
			if ((buffer[23]&0x08) == 0x08 && ZuoYiTrigger[3] == 0) {
				ZuoYiTrigger[3] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerTwo, 1, ButtonState.DOWN);
			}
			else if ((buffer[23]&0x08) == 0x00 && ZuoYiTrigger[3] == 1) {
				ZuoYiTrigger[3] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerTwo, 1, ButtonState.UP);
			}
			
			if ((buffer[23]&0x10) == 0x10 && ZuoYiTrigger[4] == 0) {
				ZuoYiTrigger[4] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerTwo, 0, ButtonState.DOWN);
			}
			else if ((buffer[23]&0x10) == 0x00 && ZuoYiTrigger[4] == 1) {
				ZuoYiTrigger[4] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerTwo, 0, ButtonState.UP);
			}
			
			if ((buffer[23]&0x20) == 0x20 && ZuoYiTrigger[5] == 0) {
				ZuoYiTrigger[5] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerTwo, -1, ButtonState.DOWN);
			}
			else if ((buffer[23]&0x20) == 0x00 && ZuoYiTrigger[5] == 1) {
				ZuoYiTrigger[5] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerTwo, -1, ButtonState.UP);
			}
			
			//game startBt, hitNpcBt or jiaoZhunBt
			if( !bPlayerStartKeyDownP1 && (buffer[20]&0x01) == 0x01 )
			{
				//ScreenLog.Log("gameP1 startBt down!");
				bPlayerStartKeyDownP1 = true;
				InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.DOWN );
			}
			else if ( bPlayerStartKeyDownP1 && (buffer[20]&0x01) == 0x00 )
			{
				//ScreenLog.Log("gameP1 startBt up!");
				bPlayerStartKeyDownP1 = false;
				InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.UP );
			}
			
			if( !IsFireBtDownP1 && (buffer[20]&0x02) == 0x02 )
			{
				IsFireBtDownP1 = true;
				InputEventCtrl.IsClickFireBtOneDown = true;
				//ScreenLog.Log("game fireBtP1 down!");
				InputEventCtrl.GetInstance().ClickFireBtOne( ButtonState.DOWN );
			}
			else if( IsFireBtDownP1 && (buffer[20]&0x02) == 0x00 )
			{
				IsFireBtDownP1 = false;
				InputEventCtrl.IsClickFireBtOneDown = false;
				//ScreenLog.Log("game fireBtP1 up!");
				InputEventCtrl.GetInstance().ClickFireBtOne( ButtonState.UP );
			}
			
			if( !IsDaoDanBtDownP1 && (buffer[20]&0x04) == 0x04 )
			{
				IsDaoDanBtDownP1 = true;
				InputEventCtrl.GetInstance().ClickDaoDanBtOne( ButtonState.DOWN );
			}
			else if( IsDaoDanBtDownP1 && (buffer[20]&0x04) == 0x00 )
			{
				IsDaoDanBtDownP1 = false;
				InputEventCtrl.GetInstance().ClickDaoDanBtOne( ButtonState.UP );
			}
			
			//game startBt, hitNpcBt or jiaoZhunBt
			if( !bPlayerStartKeyDownP2 && (buffer[20]&0x08) == 0x08 )
			{
				//ScreenLog.Log("gameP2 startBt down!");
				bPlayerStartKeyDownP2 = true;
				InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.DOWN );
			}
			else if ( bPlayerStartKeyDownP2 && (buffer[20]&0x08) == 0x00 )
			{
				//ScreenLog.Log("gameP2 startBt up!");
				bPlayerStartKeyDownP2 = false;
				InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.UP );
			}
			
			if( !IsFireBtDownP2 && (buffer[20]&0x10) == 0x10 )
			{
				IsFireBtDownP2 = true;
				InputEventCtrl.IsClickFireBtTwoDown = true;
				//ScreenLog.Log("game fireBtP2 down!");
				InputEventCtrl.GetInstance().ClickFireBtTwo( ButtonState.DOWN );
			}
			else if( IsFireBtDownP2 && (buffer[20]&0x10) == 0x00 )
			{
				IsFireBtDownP2 = false;
				InputEventCtrl.IsClickFireBtTwoDown = false;
				//ScreenLog.Log("game fireBtP2 up!");
				InputEventCtrl.GetInstance().ClickFireBtTwo( ButtonState.UP );
			}
			
			if( !IsDaoDanBtDownP2 && (buffer[20]&0x20) == 0x20 )
			{
				IsDaoDanBtDownP2 = true;
				InputEventCtrl.GetInstance().ClickDaoDanBtTwo( ButtonState.DOWN );
			}
			else if( IsDaoDanBtDownP2 && (buffer[20]&0x20) == 0x00 )
			{
				IsDaoDanBtDownP2 = false;
				InputEventCtrl.GetInstance().ClickDaoDanBtTwo( ButtonState.UP );
			}
			
			//DongGanBt
			if( !IsClickDongGanBtOne && (buffer[21]&0x10) == 0x10 )
			{
				IsClickDongGanBtOne = true;
				InputEventCtrl.GetInstance().ClickStopDongGanBtOne( ButtonState.DOWN );
			}
			else if ( IsClickDongGanBtOne && (buffer[21]&0x10) == 0x00 )
			{
				IsClickDongGanBtOne = false;
				InputEventCtrl.GetInstance().ClickStopDongGanBtOne( ButtonState.UP );
			}
			
			//setPanel selectBt
			if( !bSetEnterKeyDown && (buffer[21]&0x40) == 0x40 )
			{
				bSetEnterKeyDown = true;
				//ScreenLog.Log("game setEnterBt down!");
				InputEventCtrl.GetInstance().ClickSetEnterBt( ButtonState.DOWN );
			}
			else if ( bSetEnterKeyDown && (buffer[21]&0x40) == 0x00 )
			{
				bSetEnterKeyDown = false;
				//ScreenLog.Log("game setEnterBt up!");
				InputEventCtrl.GetInstance().ClickSetEnterBt( ButtonState.UP );
			}
			
			//setPanel moveBt
			if ( !bSetMoveKeyDown && (buffer[21]&0x80) == 0x80 )
			{
				bSetMoveKeyDown = true;
				//ScreenLog.Log("game setMoveBt down!");
				//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.DOWN );
				InputEventCtrl.GetInstance().ClickSetMoveBt( ButtonState.DOWN );
			}
			else if( bSetMoveKeyDown && (buffer[21]&0x80) == 0x00 )
			{
				bSetMoveKeyDown = false;
				//ScreenLog.Log("game setMoveBt up!");
				//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.UP );
				InputEventCtrl.GetInstance().ClickSetMoveBt( ButtonState.UP );
			}
			#endif
		}
		break;

		case PcvrComState.TanKeGunZhenDong:
		{
			#if COM_TANK_TEST || COM_FEIJI_TX
			MousePositionP1.x = ((buffer[2] & 0x0f) << 8) + buffer[3];
			MousePositionP1.y = ((buffer[4] & 0x0f) << 8) + buffer[5];
			MousePositionP2.x = ((buffer[6] & 0x0f) << 8) + buffer[7];
			MousePositionP2.y = ((buffer[8] & 0x0f) << 8) + buffer[9];                                                           
			
			//game coinInfo
			CoinCurPcvr12 = buffer[18];
			uint coinP1 = CoinCurPcvr12 & 0x0f;
			uint coinP2 = (CoinCurPcvr12 & 0xf0) >> 4;//coinP2 = coinP1; //test
			if (IsSubPlayerCoin) {
				if (coinP1 == 0 && IsSubCoinP1) {
					IsSubCoinP1 = false;
					IsSubPlayerCoin = false;
					subCoinNum12 = 0;
				}

				if (coinP2 == 0 && IsSubCoinP2) {
					IsSubCoinP2 = false;
					IsSubPlayerCoin = false;
					subCoinNum12 = 0;
				}
			}
			else {
				if (coinP1 > 0 && coinP1 < 256) {
					IsSubCoinP1 = true;
					CoinNumCurrentP1 += (int)coinP1;
					XKGlobalData.SetCoinPlayerOne(CoinNumCurrentP1);
					SubPcvrCoin(PlayerEnum.PlayerOne, (int)(CoinCurPcvr12 & 0x0f));
				}
				
				if (coinP2 > 0 && coinP2 < 256) {
					IsSubCoinP2 = true;
					CoinNumCurrentP2 += (int)coinP2;
					XKGlobalData.SetCoinPlayerTwo(CoinNumCurrentP2);
					SubPcvrCoin(PlayerEnum.PlayerTwo, (int)(CoinCurPcvr12 & 0xf0));
				}
			}
			
			//game startBt, hitNpcBt or jiaoZhunBt
			if( !bPlayerStartKeyDownP1 && (buffer[19]&0x04) == 0x04 )
			{
				//ScreenLog.Log("gameP1 startBt down!");
				bPlayerStartKeyDownP1 = true;
				InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.DOWN );
			}
			else if ( bPlayerStartKeyDownP1 && (buffer[19]&0x04) == 0x00 )
			{
				//ScreenLog.Log("gameP1 startBt up!");
				bPlayerStartKeyDownP1 = false;
				InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.UP );
			}
			
			//game startBt, hitNpcBt or jiaoZhunBt
			if( !bPlayerStartKeyDownP2 && (buffer[19]&0x08) == 0x08 )
			{
				//ScreenLog.Log("gameP2 startBt down!");
				bPlayerStartKeyDownP2 = true;
				InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.DOWN );
			}
			else if ( bPlayerStartKeyDownP2 && (buffer[19]&0x08) == 0x00 )
			{
				//ScreenLog.Log("gameP2 startBt up!");
				bPlayerStartKeyDownP2 = false;
				InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.UP );
			}
			
			//DongGanBt
			if( !IsClickDongGanBtOne && (buffer[19]&0x10) == 0x10 )
			{
				IsClickDongGanBtOne = true;
				InputEventCtrl.GetInstance().ClickStopDongGanBtOne( ButtonState.DOWN );
			}
			else if ( IsClickDongGanBtOne && (buffer[19]&0x10) == 0x00 )
			{
				IsClickDongGanBtOne = false;
				InputEventCtrl.GetInstance().ClickStopDongGanBtOne( ButtonState.UP );
			}
			
			if( !IsFireBtDownP1 && (buffer[19]&0x40) == 0x40 )
			{
				IsFireBtDownP1 = true;
				InputEventCtrl.IsClickFireBtOneDown = true;
				//ScreenLog.Log("game fireBtP1 down!");
				InputEventCtrl.GetInstance().ClickFireBtOne( ButtonState.DOWN );
			}
			else if( IsFireBtDownP1 && (buffer[19]&0x40) == 0x00 )
			{
				IsFireBtDownP1 = false;
				InputEventCtrl.IsClickFireBtOneDown = false;
				//ScreenLog.Log("game fireBtP1 up!");
				InputEventCtrl.GetInstance().ClickFireBtOne( ButtonState.UP );
			}
			
			if( !IsDaoDanBtDownP1 && (buffer[19]&0x80) == 0x80 )
			{
				IsDaoDanBtDownP1 = true;
				InputEventCtrl.GetInstance().ClickDaoDanBtOne( ButtonState.DOWN );
			}
			else if( IsDaoDanBtDownP1 && (buffer[19]&0x80) == 0x00 )
			{
				IsDaoDanBtDownP1 = false;
				InputEventCtrl.GetInstance().ClickDaoDanBtOne( ButtonState.UP );
			}
			
			if( !IsFireBtDownP2 && (buffer[20]&0x01) == 0x01 )
			{
				IsFireBtDownP2 = true;
				InputEventCtrl.IsClickFireBtTwoDown = true;
				//ScreenLog.Log("game fireBtP2 down!");
				InputEventCtrl.GetInstance().ClickFireBtTwo( ButtonState.DOWN );
			}
			else if( IsFireBtDownP2 && (buffer[20]&0x01) == 0x00 )
			{
				IsFireBtDownP2 = false;
				InputEventCtrl.IsClickFireBtTwoDown = false;
				//ScreenLog.Log("game fireBtP2 up!");
				InputEventCtrl.GetInstance().ClickFireBtTwo( ButtonState.UP );
			}
			
			if( !IsDaoDanBtDownP2 && (buffer[20]&0x02) == 0x02 )
			{
				IsDaoDanBtDownP2 = true;
				InputEventCtrl.GetInstance().ClickDaoDanBtTwo( ButtonState.DOWN );
			}
			else if( IsDaoDanBtDownP2 && (buffer[20]&0x02) == 0x00 )
			{
				IsDaoDanBtDownP2 = false;
				InputEventCtrl.GetInstance().ClickDaoDanBtTwo( ButtonState.UP );
			}
			
			//setPanel selectBt
			if( !bSetEnterKeyDown && (buffer[19]&0x01) == 0x01 )
			{
				bSetEnterKeyDown = true;
				//ScreenLog.Log("game setEnterBt down!");
				InputEventCtrl.GetInstance().ClickSetEnterBt( ButtonState.DOWN );
			}
			else if ( bSetEnterKeyDown && (buffer[19]&0x01) == 0x00 )
			{
				bSetEnterKeyDown = false;
				//ScreenLog.Log("game setEnterBt up!");
				InputEventCtrl.GetInstance().ClickSetEnterBt( ButtonState.UP );
			}
			
			//setPanel moveBt
			if ( !bSetMoveKeyDown && (buffer[19]&0x02) == 0x02 )
			{
				bSetMoveKeyDown = true;
				//ScreenLog.Log("game setMoveBt down!");
				//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.DOWN );
				InputEventCtrl.GetInstance().ClickSetMoveBt( ButtonState.DOWN );
			}
			else if( bSetMoveKeyDown && (buffer[19]&0x02) == 0x00 )
			{
				bSetMoveKeyDown = false;
				//ScreenLog.Log("game setMoveBt up!");
				//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.UP );
				InputEventCtrl.GetInstance().ClickSetMoveBt( ButtonState.UP );
			}
			#endif
		}
		break;
		}
	}
	
//	void ResetIsTouBiBtDown()
//	{
//		if(!bIsTouBiBtDown)
//		{
//			return;
//		}
//		bIsTouBiBtDown = false;
//	}
	
	static void SubTaBanCountInfo()
	{
		if(TanBanDownCount_P1 > 0)
		{
			TanBanDownCount_P1 -= SubTaBanCount;
			if(TanBanDownCount_P1 < 0)
			{
				TanBanDownCount_P1 = 0;
			}
		}
	}
	
	void closeDevice()
	{
		if (openPCVRFlag == 1)
		{
			openPCVRFlag = 2;
		}
	}
	
	void SubPcvrCoin(PlayerEnum indexPlayer, int subNum)
	{
		if (!bIsHardWare) {
			return;
		}
		IsSubPlayerCoin = true;
		switch (indexPlayer) {
		case PlayerEnum.PlayerOne:
		case PlayerEnum.PlayerTwo:
			subCoinNum12 += subNum;
			break;
		case PlayerEnum.PlayerThree:
		case PlayerEnum.PlayerFour:
			subCoinNum34 += subNum;
			break;
		}
	}
	
	public void SubPlayerCoin(int subNum, PlayerEnum playerIndex)
	{
		if (!bIsHardWare) {
			return;
		}

		switch (playerIndex) {
		case PlayerEnum.PlayerOne:
			if (CoinNumCurrentP1 < subNum) {
				return;
			}
			CoinNumCurrentP1 -= subNum;
			break;

		case PlayerEnum.PlayerTwo:
			if (CoinNumCurrentP2 < subNum) {
				return;
			}
			CoinNumCurrentP2 -= subNum;
			break;
		}

//		if (gOldCoinNum >= subNum) {
//			gOldCoinNum = (uint)(gOldCoinNum - subNum);
//		}
//		else {
//			if (mOldCoinNum == 0) {
//				return;
//			}
//			int subTmpVal = (int)(subNum - gOldCoinNum);
//			mOldCoinNum -= (uint)subTmpVal;
//			gOldCoinNum = 0;
//		}
	}
	
	public static void InitHandleJsonInfo()
	{
		FileName = XKGlobalData.FileName;
		HandleJsonObj = XKGlobalData.HandleJsonObj;
	}
	
	public static void InitSteerInfo()
	{
		string strValMax = HandleJsonObj.ReadFromFileXml(FileName, "SteerValMax");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "1";
			HandleJsonObj.WriteToFileXml(FileName, "SteerValMax", strValMax);
		}
		SteerValMax = Convert.ToUInt32( strValMax );
		
		string strValMin = HandleJsonObj.ReadFromFileXml(FileName, "SteerValMin");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			HandleJsonObj.WriteToFileXml(FileName, "SteerValMin", strValMin);
		}
		SteerValMin = Convert.ToUInt32( strValMin );
		
		string strValCen = HandleJsonObj.ReadFromFileXml(FileName, "SteerValCen");
		if(strValCen == null || strValCen == "")
		{
			strValCen = "1";
			HandleJsonObj.WriteToFileXml(FileName, "SteerValCen", strValCen);
		}
		SteerValCen = Convert.ToUInt32( strValCen );
		
		SteerDisVal = (uint)Mathf.Abs((float)SteerValMax - SteerValMin);
		//Debug.Log("SteerDisVal " + SteerDisVal + ", SteerValMax " + SteerValMax + ", SteerValMin " + SteerValMin);
	}
	
	public static void SaveSteerVal(uint steerVal, PcvrValState key)
	{
		switch (key) {
		case PcvrValState.ValMin:
			SteerValMin = steerVal;
			HandleJsonObj.WriteToFileXml(FileName, "SteerValMin", steerVal.ToString());
			break;
			
		case PcvrValState.ValCenter:
			SteerValCen = steerVal;
			HandleJsonObj.WriteToFileXml(FileName, "SteerValCen", steerVal.ToString());
			break;
			
		case PcvrValState.ValMax:
			SteerValMax = steerVal;
			HandleJsonObj.WriteToFileXml(FileName, "SteerValMax", steerVal.ToString());
			break;
		}
		SteerDisVal = (uint)Mathf.Abs((float)SteerValMax - SteerValMin);
	}
	
	public static float GetPcvrSteerVal()
	{
		if (bIsHardWare && openPCVRFlag == 0) {
			mGetSteer = 0f;
			return mGetSteer;
		}
		
		if (SteerDisVal == 0) {
			mGetSteer = 0f;
			return mGetSteer;
		}
		
		float tmpVal = 0f;
		float steerCur = 0;
		if (bIsHardWare && !IsGetValByKey) {
			//steerCur = (float)SteerValCur;
			uint bikeDir = SteerValCur;
			uint bikeDirLen = SteerValMax - SteerValMin + 1;
			if(SteerValMax < SteerValMin)
			{
				if(bikeDir > SteerValCen)
				{
					bikeDirLen = SteerValMin - SteerValCen + 1;
				}
				else
				{
					bikeDirLen = SteerValCen - SteerValMax + 1;
				}
				
				//check bikeDir
				if(bikeDir < SteerValMax)
				{
					bikeDir = SteerValMax;
				}
				else if(bikeDir > SteerValMin)
				{
					bikeDir = SteerValMin;
				}
			}
			else
			{
				if(bikeDir > SteerValCen)
				{
					bikeDirLen = SteerValMax - SteerValCen + 1;
				}
				else
				{
					bikeDirLen = SteerValCen - SteerValMin + 1;
				}
				
				//check bikeDir
				if(bikeDir < SteerValMin)
				{
					bikeDir = SteerValMin;
				}
				else if(bikeDir > SteerValMax)
				{
					bikeDir = SteerValMax;
				}
			}
			////ScreenLog.Log("bikeDirLen = " + bikeDirLen);
			
			if(bikeDirLen == 0)
			{
				bikeDirLen = 1;
			}
			
			uint bikeDirCur = SteerValMax - bikeDir;
			float bikeDirPer = (float)bikeDirCur / bikeDirLen;
			if(SteerValMax > SteerValMin)
			{
				//ZhengJie FangXiangDianWeiQi
				if(bikeDir > SteerValCen)
				{
					bikeDirCur = bikeDir - SteerValCen;
					bikeDirPer = (float)bikeDirCur / bikeDirLen;
				}
				else
				{
					bikeDirCur = SteerValCen - bikeDir;
					bikeDirPer = - (float)bikeDirCur / bikeDirLen;
				}
			}
			else
			{
				//FanJie DianWeiQi
				if(bikeDir > SteerValCen)
				{
					bikeDirCur = bikeDir - SteerValCen;
					bikeDirPer = - (float)bikeDirCur / bikeDirLen;
				}
				else
				{
					bikeDirCur = SteerValCen - bikeDir;
					bikeDirPer = (float)bikeDirCur / bikeDirLen;
				}
			}
			mGetSteer = bikeDirPer;
		}
		else {
			steerCur = Input.GetAxis("Horizontal") + 1f;
			tmpVal = Mathf.Abs(steerCur - SteerValMin) / SteerDisVal;
			mGetSteer = (tmpVal - 0.5f) * 2f;
		}
		
		/*TestValStr = tmpVal.ToString() + " *** " + steerCur.ToString() + " * " + Input.GetAxis("Horizontal") + " ** " + mGetSteer;
		Debug.Log(TestValStr);*/
		return mGetSteer;
	}
	
	public static void InitTaBanInfo()
	{
		string strValMax = HandleJsonObj.ReadFromFileXml(FileName, "TaBanValMax");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "1";
			HandleJsonObj.WriteToFileXml(FileName, "TaBanValMax", strValMax);
		}
		TaBanValMax = Convert.ToUInt32( strValMax );
		
		string strValMin = HandleJsonObj.ReadFromFileXml(FileName, "TaBanValMin");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			HandleJsonObj.WriteToFileXml(FileName, "TaBanValMin", strValMin);
		}
		TaBanValMin = Convert.ToUInt32( strValMin );
		
		string strIsFanZhuangTaBan = HandleJsonObj.ReadFromFileXml(FileName, "IsFanZhuangTaBan");
		if(strIsFanZhuangTaBan == null || strIsFanZhuangTaBan == "")
		{
			strIsFanZhuangTaBan = "0";
			HandleJsonObj.WriteToFileXml(FileName, "IsFanZhuangTaBan", strIsFanZhuangTaBan);
		}
		IsFanZhuangTaBan = strIsFanZhuangTaBan == "0" ? false : true;
		TaBanDisVal = (uint)Mathf.Abs((float)TaBanValMax - TaBanValMin);
		//Debug.Log("TaBanDisVal " + TaBanDisVal + ", TaBanValMax " + TaBanValMax + ", TaBanValMin " + TaBanValMin);
	}
	
	public static void SaveTaBanVal(uint TaBanVal, PcvrValState key)
	{
		switch (key) {
		case PcvrValState.ValMin:
			TaBanValMin = TaBanVal;
			HandleJsonObj.WriteToFileXml(FileName, "TaBanValMin", TaBanVal.ToString());
			break;
			
		case PcvrValState.ValMax:
			if (!bIsHardWare) {
				SaveTaBanVal(1, PcvrValState.ValMin);
			}
			else {
				SaveTaBanVal(TanBanCenterNum, PcvrValState.ValMin);
				
				uint bikeTaBanNum = TaBanVal;
				if(bikeTaBanNum >= TanBanCenterNum)
				{
					IsFanZhuangTaBan = false;
					HandleJsonObj.WriteToFileXml(FileName, "IsFanZhuangTaBan", "0");
				}
				else if(bikeTaBanNum < TanBanCenterNum)
				{
					IsFanZhuangTaBan = true;
					HandleJsonObj.WriteToFileXml(FileName, "IsFanZhuangTaBan", "1");
				}
			}
			TaBanValMax = TaBanVal;
			HandleJsonObj.WriteToFileXml(FileName, "TaBanValMax", TaBanVal.ToString());
			TaBanDisVal = (uint)Mathf.Abs((float)TaBanValMax - TaBanValMin);
			break;
		}
	}
	
	public static bool IsGetValByKey = true;
	public static float GetPcvrTaBanVal()
	{
		if (bIsHardWare && openPCVRFlag == 0) {
			TanBanDownCount_P1 = 0f;
			return TanBanDownCount_P1;
		}
		
		if (TaBanDisVal == 0) {
			TanBanDownCount_P1 = 0f;
			return TanBanDownCount_P1;
		}
		
		float tmpVal = 0f;
		float taBanCur = 0;
		if (bIsHardWare && !IsGetValByKey) {
			uint bikeTaBanNum = TaBanValCur;
			//player click jiaoTaBanBt
			if(!IsFanZhuangTaBan)
			{
				if(bikeTaBanNum > TanBanCenterNum)
				{
					bPlayerHitTaBan_P1 = true;
					if(TaBanValMax < bikeTaBanNum)
					{
						TaBanValMax = bikeTaBanNum;
						HandleJsonObj.WriteToFileXml(FileName, "TaBanValMax", TaBanValMax.ToString());
					}
					TanBanDownCount_P1 = ((float)(bikeTaBanNum - TanBanCenterNum) / (TaBanValMax - TanBanCenterNum)) * PcvrTanBanValTmp;
				}
				else if(bikeTaBanNum < TanBanCenterNum)
				{
					SubTaBanCountInfo();
					bPlayerHitTaBan_P1 = false;
				}
				else
				{
					SubTaBanCountInfo();
				}
			}
			else
			{
				if(bikeTaBanNum < TanBanCenterNum)
				{
					bPlayerHitTaBan_P1 = true;
					if(TaBanValMax > bikeTaBanNum)
					{
						TaBanValMax = bikeTaBanNum;
						HandleJsonObj.WriteToFileXml(FileName, "TaBanValMax", TaBanValMax.ToString());
					}
					TanBanDownCount_P1 = ((float)(TanBanCenterNum - bikeTaBanNum) / (TanBanCenterNum - TaBanValMax)) * PcvrTanBanValTmp;
				}
				else if(bikeTaBanNum > TanBanCenterNum)
				{
					SubTaBanCountInfo();
					bPlayerHitTaBan_P1 = false;
				}
				else
				{
					SubTaBanCountInfo();
				}
			}
		}
		else {
			taBanCur = Input.GetAxis("Vertical") + 1f;
			if ( (TaBanValMin < TaBanValMax && taBanCur >= TaBanValMin)
			    || (TaBanValMin > TaBanValMax && taBanCur <= TaBanValMin) ) {
				//tmpVal = Mathf.Abs((float)taBanCur - TaBanValMin) / TaBanDisVal;
				tmpVal = Mathf.Abs(taBanCur - TaBanValMin) / TaBanDisVal;
				//TanBanDownCount_P1 = (tmpVal - 0.5f) * 2f;
				TanBanDownCount_P1 = tmpVal;
			}
		}
		return TanBanDownCount_P1;
	}
	
	void InitCrossPosInfoPOne()
	{
		string strValMax = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosXMaxP1");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP1", strValMax);
		}
		CrossPosXMaxP1 = Convert.ToUInt32( strValMax );
		
		string strValMin = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosXMinP1");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP1", strValMax);
		}
		CrossPosXMinP1 = Convert.ToUInt32( strValMin );
		
		strValMax = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosYMaxP1");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP1", strValMax);
		}
		CrossPosYMaxP1 = Convert.ToUInt32( strValMax );
		
		strValMin = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosYMinP1");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP1", strValMax);
		}
		CrossPosYMinP1 = Convert.ToUInt32( strValMin );
		
		CrossPosDisXP1 = (uint)Mathf.Abs((float)CrossPosXMaxP1 - CrossPosXMinP1) + 1;
		CrossPosDisYP1 = (uint)Mathf.Abs((float)CrossPosYMaxP1 - CrossPosYMinP1) + 1;
		/*Debug.Log("CrossPosDisXP1 " + CrossPosDisXP1 + ", CrossPosDisYP1 " + CrossPosDisYP1);
		Debug.Log("CrossPosXMaxP1 " + CrossPosXMaxP1 + ", CrossPosXMinP1 " + CrossPosXMinP1);
		Debug.Log("CrossPosYMaxP1 " + CrossPosYMaxP1 + ", CrossPosYMinP1 " + CrossPosYMinP1);*/
	}
	
	public static void SaveCrossPosInfoPOne(AdjustGunDrossState val)
	{
		Vector3 crossPos = Input.mousePosition;
		if (bIsHardWare) {
			crossPos = MousePositionP1;
		}
		
		if (crossPos.x  < 0f) {
			crossPos.x = 0f;
		}
		
		if (crossPos.y  < 0f) {
			crossPos.y = 0f;
		}
		
		uint pxCenterMin = 0;
		uint pyCenterMin = 0;
		uint pxCenterMax = 0;
		uint pyCenterMax = 0;
		
		uint px = (uint)crossPos.x;
		uint py = (uint)crossPos.y;
		//Debug.Log("px " + px + ", py " + py);

		switch (MyCOMDevice.PcvrGameSt) {
		case PcvrComState.TanKeGunZhenDong:
		{
			switch (val) {
			case AdjustGunDrossState.GunCrossLU:
				if (IsFanZhuanZuoBiaoPY) {
					CrossPxP1_1 = px;
					CrossPyP1_4 = py;
				}
				else {
					CrossPxP1_1 = px;
					CrossPyP1_1 = py;
				}
				break;
				
			case AdjustGunDrossState.GunCrossRU:
				if (IsFanZhuanZuoBiaoPY) {
					CrossPxP1_2 = px;
					CrossPyP1_3 = py;
				}
				else {
					CrossPxP1_2 = px;
					CrossPyP1_2 = py;
				}
				break;
				
			case AdjustGunDrossState.GunCrossRD:
				if (IsFanZhuanZuoBiaoPY) {
					CrossPxP1_3 = px;
					CrossPyP1_2 = py;
				}
				else {
					CrossPxP1_3 = px;
					CrossPyP1_3 = py;
				}
				break;
				
			case AdjustGunDrossState.GunCrossLD:
				if (IsFanZhuanZuoBiaoPY) {
					CrossPxP1_4 = px;
					CrossPyP1_1 = py;
				}
				else {
					CrossPxP1_4 = px;
					CrossPyP1_4 = py;
				}
				
				pxCenterMin = (uint)(0.5f * (CrossPxP1_1 + CrossPxP1_4));
				pxCenterMax = (uint)(0.5f * (CrossPxP1_2 + CrossPxP1_3));
				pyCenterMin = (uint)(0.5f * (CrossPyP1_3 + CrossPyP1_4));
				pyCenterMax = (uint)(0.5f * (CrossPyP1_1 + CrossPyP1_2));
				
				if (pxCenterMin < pxCenterMax) {
					CrossPosXMinP1 = CrossPxP1_1 >= CrossPxP1_4 ? CrossPxP1_4 : CrossPxP1_1;
					px = CrossPosXMinP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP1", px.ToString());
					
					CrossPosXMaxP1 = CrossPxP1_2 >= CrossPxP1_3 ? CrossPxP1_2 : CrossPxP1_3;
					px = CrossPosXMaxP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP1", px.ToString());
				}
				else {
					CrossPosXMinP1 = CrossPxP1_1 <= CrossPxP1_4 ? CrossPxP1_4 : CrossPxP1_1;
					px = CrossPosXMinP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP1", px.ToString());
					
					CrossPosXMaxP1 = CrossPxP1_2 <= CrossPxP1_3 ? CrossPxP1_2 : CrossPxP1_3;
					px = CrossPosXMaxP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP1", px.ToString());
				}
				
				if (pyCenterMin < pyCenterMax) {
					CrossPosYMinP1 = CrossPyP1_3 >= CrossPyP1_4 ? CrossPyP1_4 : CrossPyP1_3;
					py = CrossPosYMinP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP1", py.ToString());
					
					CrossPosYMaxP1 = CrossPyP1_1 >= CrossPyP1_2 ? CrossPyP1_1 : CrossPyP1_2;
					py = CrossPosYMaxP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP1", py.ToString());
				}
				else {
					CrossPosYMinP1 = CrossPyP1_3 <= CrossPyP1_4 ? CrossPyP1_4 : CrossPyP1_3;
					py = CrossPosYMinP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP1", py.ToString());
					
					CrossPosYMaxP1 = CrossPyP1_1 <= CrossPyP1_2 ? CrossPyP1_1 : CrossPyP1_2;
					py = CrossPosYMaxP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP1", py.ToString());
				}
				break;
			}
		}
		break;
			
		case PcvrComState.TanKeFangXiangZhenDong:
		{
			switch (val) {
			case AdjustGunDrossState.GunCrossLU:
				CrossPxP1_1 = px;
				CrossPxP1_4 = px;
				break;
				
			case AdjustGunDrossState.GunCrossRU:
				CrossPxP1_2 = px;
				CrossPxP1_3 = px;
				break;
				
			case AdjustGunDrossState.GunCrossRD:
				if (IsFanZhuanZuoBiaoPY)
				{
					CrossPyP1_3 = py;
					CrossPyP1_4 = py;
				}
				else
				{
					CrossPyP1_1 = py;
					CrossPyP1_2 = py;
				}
				break;
				
			case AdjustGunDrossState.GunCrossLD:
				if (IsFanZhuanZuoBiaoPY)
				{
					CrossPyP1_1 = py;
					CrossPyP1_2 = py;
				}
				else
				{
					CrossPyP1_3 = py;
					CrossPyP1_4 = py;
				}
				
				pxCenterMin = (uint)(0.5f * (CrossPxP1_1 + CrossPxP1_4));
				pxCenterMax = (uint)(0.5f * (CrossPxP1_2 + CrossPxP1_3));
				pyCenterMin = (uint)(0.5f * (CrossPyP1_3 + CrossPyP1_4));
				pyCenterMax = (uint)(0.5f * (CrossPyP1_1 + CrossPyP1_2));
				
				if (pxCenterMin < pxCenterMax) {
					CrossPosXMinP1 = CrossPxP1_1 >= CrossPxP1_4 ? CrossPxP1_4 : CrossPxP1_1;
					px = CrossPosXMinP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP1", px.ToString());
					
					CrossPosXMaxP1 = CrossPxP1_2 >= CrossPxP1_3 ? CrossPxP1_2 : CrossPxP1_3;
					px = CrossPosXMaxP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP1", px.ToString());
				}
				else {
					CrossPosXMinP1 = CrossPxP1_1 <= CrossPxP1_4 ? CrossPxP1_4 : CrossPxP1_1;
					px = CrossPosXMinP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP1", px.ToString());
					
					CrossPosXMaxP1 = CrossPxP1_2 <= CrossPxP1_3 ? CrossPxP1_2 : CrossPxP1_3;
					px = CrossPosXMaxP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP1", px.ToString());
				}
				
				if (pyCenterMin < pyCenterMax) {
					CrossPosYMinP1 = CrossPyP1_3 >= CrossPyP1_4 ? CrossPyP1_4 : CrossPyP1_3;
					py = CrossPosYMinP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP1", py.ToString());
					
					CrossPosYMaxP1 = CrossPyP1_1 >= CrossPyP1_2 ? CrossPyP1_1 : CrossPyP1_2;
					py = CrossPosYMaxP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP1", py.ToString());
				}
				else {
					CrossPosYMinP1 = CrossPyP1_3 <= CrossPyP1_4 ? CrossPyP1_4 : CrossPyP1_3;
					py = CrossPosYMinP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP1", py.ToString());
					
					CrossPosYMaxP1 = CrossPyP1_1 <= CrossPyP1_2 ? CrossPyP1_1 : CrossPyP1_2;
					py = CrossPosYMaxP1;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP1", py.ToString());
				}
				break;
			}
		}
		break;
		}
		
		CrossPosDisXP1 = (uint)Mathf.Abs((float)CrossPosXMaxP1 - CrossPosXMinP1) + 1;
		CrossPosDisYP1 = (uint)Mathf.Abs((float)CrossPosYMaxP1 - CrossPosYMinP1) + 1;
		
		/*Debug.Log("CrossPosDisXP1 " + CrossPosDisXP1
		          + ", CrossPosDisYP1 " + CrossPosDisYP1
		          + ", CrossPosXMaxP1 " + CrossPosXMaxP1
		          + ", CrossPosXMinP1 " + CrossPosXMinP1
		          + ", CrossPosYMaxP1 " + CrossPosYMaxP1
		          + ", CrossPosYMinP1 " + CrossPosYMinP1);*/
	}
	
	public static void CheckCrossPositionPOne()
	{
		if (CrossPosDisXP1 <= 1 || CrossPosDisYP1 <= 1) {
			return;
		}
		
		Vector3 pos = Vector3.zero;
		Vector3 mousePosCur = Input.mousePosition;
		if (bIsHardWare) {
			mousePosCur = MousePositionP1;
		}
		
		if (CrossPosXMinP1 < CrossPosXMaxP1) {
			mousePosCur.x = mousePosCur.x < CrossPosXMinP1 ? CrossPosXMinP1 : mousePosCur.x;
			mousePosCur.x = mousePosCur.x < CrossPosXMaxP1 ? mousePosCur.x : CrossPosXMaxP1;
		}
		else {
			mousePosCur.x = mousePosCur.x > CrossPosXMinP1 ? CrossPosXMinP1 : mousePosCur.x;
			mousePosCur.x = mousePosCur.x > CrossPosXMaxP1 ? mousePosCur.x : CrossPosXMaxP1;
		}
		
		if (CrossPosYMinP1 < CrossPosYMaxP1) {
			mousePosCur.y = mousePosCur.y < CrossPosYMinP1 ? CrossPosYMinP1 : mousePosCur.y;
			mousePosCur.y = mousePosCur.y < CrossPosYMaxP1 ? mousePosCur.y : CrossPosYMaxP1;
		}
		else {
			mousePosCur.y = mousePosCur.y > CrossPosYMinP1 ? CrossPosYMinP1 : mousePosCur.y;
			mousePosCur.y = mousePosCur.y > CrossPosYMaxP1 ? mousePosCur.y : CrossPosYMaxP1;
		}

		if (!GameMovieCtrl.IsThreeScreenGame) {
			pos.x = (int)(Mathf.Abs(mousePosCur.x - CrossPosXMinP1 + 1) * XkGameCtrl.ScreenWidth) / CrossPosDisXP1;
			pos.y = (int)(Mathf.Abs(mousePosCur.y - CrossPosYMinP1 + 1) * XkGameCtrl.ScreenHeight) / CrossPosDisYP1;
		}
		else {
			pos.x = (int)(Mathf.Abs(mousePosCur.x - CrossPosXMinP1 + 1) * XkGameCtrl.ScreenWidth3) / CrossPosDisXP1;
			pos.y = (int)(Mathf.Abs(mousePosCur.y - CrossPosYMinP1 + 1) * XkGameCtrl.ScreenHeight3) / CrossPosDisYP1;
		}

		if (GunPosOffsetPY > 0f) {
			float offsetPY = Mathf.Abs(pos.y - CrossPositionOne.y);
			if (XkGameCtrl.GaoBaoDanNumPOne <= 0) {
				if (offsetPY < GunPosOffsetPY) {
					pos.y = CrossPositionOne.y;
				}
			}
			else {
				if (offsetPY < GunPosOffsetGaoBaoDanPY) {
					pos.y = CrossPositionOne.y;
				}
			}
		}
		CrossPositionOne = pos;
		
		if (bIsHardWare) {
			if (ZhunXingCtrl.GetInstanceOne() != null && ZhunXingCtrl.GetInstanceOne().GetActiveZhunXing()) {
				ZhunXingCtrl.GetInstanceOne().FixedUpdateGunCross();
			}
		}
	}
	static float GunPosOffsetPY = 18f;
	const float GunPosOffsetGaoBaoDanPY = 28f;

	void InitCrossPosInfoPTwo()
	{
		string strValMax = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosXMaxP2");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP2", strValMax);
		}
		CrossPosXMaxP2 = Convert.ToUInt32( strValMax );
		
		string strValMin = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosXMinP2");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP2", strValMax);
		}
		CrossPosXMinP2 = Convert.ToUInt32( strValMin );
		
		strValMax = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosYMaxP2");
		if(strValMax == null || strValMax == "")
		{
			strValMax = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP2", strValMax);
		}
		CrossPosYMaxP2 = Convert.ToUInt32( strValMax );
		
		strValMin = HandleJsonObj.ReadFromFileXml(FileName, "CrossPosYMinP2");
		if(strValMin == null || strValMin == "")
		{
			strValMin = "1";
			HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP2", strValMax);
		}
		CrossPosYMinP2 = Convert.ToUInt32( strValMin );
		
		CrossPosDisXP2 = (uint)Mathf.Abs((float)CrossPosXMaxP2 - CrossPosXMinP2) + 1;
		CrossPosDisYP2 = (uint)Mathf.Abs((float)CrossPosYMaxP2 - CrossPosYMinP2) + 1;
		/*Debug.Log("CrossPosDisXP2 " + CrossPosDisXP2 + ", CrossPosDisYP2 " + CrossPosDisYP2);
		Debug.Log("CrossPosXMaxP2 " + CrossPosXMaxP2 + ", CrossPosXMinP2 " + CrossPosXMinP2);
		Debug.Log("CrossPosYMaxP2 " + CrossPosYMaxP2 + ", CrossPosYMinP2 " + CrossPosYMinP2);*/
	}

	/**
	 * IsFanZhuanZuoBiaoPY == true -> 准星坐标在Y轴进行反转.
	 */
	public static bool IsFanZhuanZuoBiaoPY;
	public static void SaveCrossPosInfoPTwo(AdjustGunDrossState val)
	{
		Vector3 crossPos = Input.mousePosition;
		if (bIsHardWare) {
			crossPos = MousePositionP2;
		}
		
		if (crossPos.x  < 0f) {
			crossPos.x = 0f;
		}
		
		if (crossPos.y  < 0f) {
			crossPos.y = 0f;
		}
		
		uint pxCenterMin = 0;
		uint pyCenterMin = 0;
		uint pxCenterMax = 0;
		uint pyCenterMax = 0;
		
		uint px = (uint)crossPos.x;
		uint py = (uint)crossPos.y;
		//Debug.Log("px " + px + ", py " + py);

		switch (MyCOMDevice.PcvrGameSt) {
		case PcvrComState.TanKeGunZhenDong:
		{
			switch (val) {
			case AdjustGunDrossState.GunCrossLU:
				if (IsFanZhuanZuoBiaoPY) {
					CrossPxP2_1 = px;
					CrossPyP2_4 = py;
				}
				else {
					CrossPxP2_1 = px;
					CrossPyP2_1 = py;
				}
				break;
				
			case AdjustGunDrossState.GunCrossRU:
				if (IsFanZhuanZuoBiaoPY) {
					CrossPxP2_2 = px;
					CrossPyP2_3 = py;
				}
				else {
					CrossPxP2_2 = px;
					CrossPyP2_2 = py;
				}
				break;
				
			case AdjustGunDrossState.GunCrossRD:
				if (IsFanZhuanZuoBiaoPY) {
					CrossPxP2_3 = px;
					CrossPyP2_2 = py;
				}
				else {
					CrossPxP2_3 = px;
					CrossPyP2_3 = py;
				}
				break;
				
			case AdjustGunDrossState.GunCrossLD:
				if (IsFanZhuanZuoBiaoPY) {
					CrossPxP2_4 = px;
					CrossPyP2_1 = py;
				}
				else {
					CrossPxP2_4 = px;
					CrossPyP2_4 = py;
				}
				
				pxCenterMin = (uint)(0.5f * (CrossPxP2_1 + CrossPxP2_4));
				pxCenterMax = (uint)(0.5f * (CrossPxP2_2 + CrossPxP2_3));
				pyCenterMin = (uint)(0.5f * (CrossPyP2_3 + CrossPyP2_4));
				pyCenterMax = (uint)(0.5f * (CrossPyP2_1 + CrossPyP2_2));
				
				if (pxCenterMin < pxCenterMax) {
					CrossPosXMinP2 = CrossPxP2_1 >= CrossPxP2_4 ? CrossPxP2_4 : CrossPxP2_1;
					px = CrossPosXMinP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP2", px.ToString());
					
					CrossPosXMaxP2 = CrossPxP2_2 >= CrossPxP2_3 ? CrossPxP2_2 : CrossPxP2_3;
					px = CrossPosXMaxP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP2", px.ToString());
				}
				else {
					CrossPosXMinP2 = CrossPxP2_1 <= CrossPxP2_4 ? CrossPxP2_4 : CrossPxP2_1;
					px = CrossPosXMinP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP2", px.ToString());
					
					CrossPosXMaxP2 = CrossPxP2_2 <= CrossPxP2_3 ? CrossPxP2_2 : CrossPxP2_3;
					px = CrossPosXMaxP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP2", px.ToString());
				}
				
				if (pyCenterMin < pyCenterMax) {
					CrossPosYMinP2 = CrossPyP2_3 >= CrossPyP2_4 ? CrossPyP2_4 : CrossPyP2_3;
					py = CrossPosYMinP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP2", py.ToString());
					
					CrossPosYMaxP2 = CrossPyP2_1 >= CrossPyP2_2 ? CrossPyP2_1 : CrossPyP2_2;
					py = CrossPosYMaxP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP2", py.ToString());
				}
				else {
					CrossPosYMinP2 = CrossPyP2_3 <= CrossPyP2_4 ? CrossPyP2_4 : CrossPyP2_3;
					py = CrossPosYMinP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP2", py.ToString());
					
					CrossPosYMaxP2 = CrossPyP2_1 <= CrossPyP2_2 ? CrossPyP2_1 : CrossPyP2_2;
					py = CrossPosYMaxP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP2", py.ToString());
				}
				break;
			}
		}
		break;
		case PcvrComState.TanKeFangXiangZhenDong:
		{
			switch (val) {
			case AdjustGunDrossState.GunCrossLU:
				CrossPxP2_1 = px;
				CrossPxP2_4 = px;
				break;
				
			case AdjustGunDrossState.GunCrossRU:
				CrossPxP2_2 = px;
				CrossPxP2_3 = px;
				break;
				
			case AdjustGunDrossState.GunCrossRD:
				if (IsFanZhuanZuoBiaoPY)
				{
					CrossPyP2_3 = py;
					CrossPyP2_4 = py;
				}
				else
				{
					CrossPyP2_1 = py;
					CrossPyP2_2 = py;
				}
				break;
				
			case AdjustGunDrossState.GunCrossLD:
				if (IsFanZhuanZuoBiaoPY)
				{
					CrossPyP2_1 = py;
					CrossPyP2_2 = py;
				}
				else
				{
					CrossPyP2_3 = py;
					CrossPyP2_4 = py;
				}
				
				pxCenterMin = (uint)(0.5f * (CrossPxP2_1 + CrossPxP2_4));
				pxCenterMax = (uint)(0.5f * (CrossPxP2_2 + CrossPxP2_3));
				pyCenterMin = (uint)(0.5f * (CrossPyP2_3 + CrossPyP2_4));
				pyCenterMax = (uint)(0.5f * (CrossPyP2_1 + CrossPyP2_2));
				
				if (pxCenterMin < pxCenterMax) {
					CrossPosXMinP2 = CrossPxP2_1 >= CrossPxP2_4 ? CrossPxP2_4 : CrossPxP2_1;
					px = CrossPosXMinP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP2", px.ToString());
					
					CrossPosXMaxP2 = CrossPxP2_2 >= CrossPxP2_3 ? CrossPxP2_2 : CrossPxP2_3;
					px = CrossPosXMaxP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP2", px.ToString());
				}
				else {
					CrossPosXMinP2 = CrossPxP2_1 <= CrossPxP2_4 ? CrossPxP2_4 : CrossPxP2_1;
					px = CrossPosXMinP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMinP2", px.ToString());
					
					CrossPosXMaxP2 = CrossPxP2_2 <= CrossPxP2_3 ? CrossPxP2_2 : CrossPxP2_3;
					px = CrossPosXMaxP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosXMaxP2", px.ToString());
				}
				
				if (pyCenterMin < pyCenterMax) {
					CrossPosYMinP2 = CrossPyP2_3 >= CrossPyP2_4 ? CrossPyP2_4 : CrossPyP2_3;
					py = CrossPosYMinP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP2", py.ToString());
					
					CrossPosYMaxP2 = CrossPyP2_1 >= CrossPyP2_2 ? CrossPyP2_1 : CrossPyP2_2;
					py = CrossPosYMaxP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP2", py.ToString());
				}
				else {
					CrossPosYMinP2 = CrossPyP2_3 <= CrossPyP2_4 ? CrossPyP2_4 : CrossPyP2_3;
					py = CrossPosYMinP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMinP2", py.ToString());
					
					CrossPosYMaxP2 = CrossPyP2_1 <= CrossPyP2_2 ? CrossPyP2_1 : CrossPyP2_2;
					py = CrossPosYMaxP2;
					HandleJsonObj.WriteToFileXml(FileName, "CrossPosYMaxP2", py.ToString());
				}
				break;
			}
		}
		break;
		}
		
		CrossPosDisXP2 = (uint)Mathf.Abs((float)CrossPosXMaxP2 - CrossPosXMinP2) + 1;
		CrossPosDisYP2 = (uint)Mathf.Abs((float)CrossPosYMaxP2 - CrossPosYMinP2) + 1;
		
		/*Debug.Log("CrossPosDisXP2 " + CrossPosDisXP2
		          + ", CrossPosDisYP2 " + CrossPosDisYP2
		          + ", CrossPosXMaxP2 " + CrossPosXMaxP2
		          + ", CrossPosXMinP2 " + CrossPosXMinP2
		          + ", CrossPosYMaxP2 " + CrossPosYMaxP2
		          + ", CrossPosYMinP2 " + CrossPosYMinP2);*/
	}
	
	public static void CheckCrossPositionPTwo()
	{
		if (CrossPosDisXP2 <= 1 || CrossPosDisYP2 <= 1) {
			return;
		}
		
		Vector3 pos = Vector3.zero;
		Vector3 mousePosCur = Input.mousePosition;
		if (bIsHardWare) {
			mousePosCur = MousePositionP2;
		}
		
		if (CrossPosXMinP2 < CrossPosXMaxP2) {
			mousePosCur.x = mousePosCur.x < CrossPosXMinP2 ? CrossPosXMinP2 : mousePosCur.x;
			mousePosCur.x = mousePosCur.x < CrossPosXMaxP2 ? mousePosCur.x : CrossPosXMaxP2;
		}
		else {
			mousePosCur.x = mousePosCur.x > CrossPosXMinP2 ? CrossPosXMinP2 : mousePosCur.x;
			mousePosCur.x = mousePosCur.x > CrossPosXMaxP2 ? mousePosCur.x : CrossPosXMaxP2;
		}
		
		if (CrossPosYMinP2 < CrossPosYMaxP2) {
			mousePosCur.y = mousePosCur.y < CrossPosYMinP2 ? CrossPosYMinP2 : mousePosCur.y;
			mousePosCur.y = mousePosCur.y < CrossPosYMaxP2 ? mousePosCur.y : CrossPosYMaxP2;
		}
		else {
			mousePosCur.y = mousePosCur.y > CrossPosYMinP2 ? CrossPosYMinP2 : mousePosCur.y;
			mousePosCur.y = mousePosCur.y > CrossPosYMaxP2 ? mousePosCur.y : CrossPosYMaxP2;
		}

		if (!GameMovieCtrl.IsThreeScreenGame) {
			pos.x = (int)(Mathf.Abs(mousePosCur.x - CrossPosXMinP2 + 1) * XkGameCtrl.ScreenWidth) / CrossPosDisXP2;
			pos.y = (int)(Mathf.Abs(mousePosCur.y - CrossPosYMinP2 + 1) * XkGameCtrl.ScreenHeight) / CrossPosDisYP2;
		}
		else {
			pos.x = (int)(Mathf.Abs(mousePosCur.x - CrossPosXMinP2 + 1) * XkGameCtrl.ScreenWidth3) / CrossPosDisXP2;
			pos.y = (int)(Mathf.Abs(mousePosCur.y - CrossPosYMinP2 + 1) * XkGameCtrl.ScreenHeight3) / CrossPosDisYP2;
		}

		if (GunPosOffsetPY > 0f) {
			float offsetPY = Mathf.Abs(pos.y - CrossPositionTwo.y);
			if (XkGameCtrl.GaoBaoDanNumPTwo <= 0) {
				if (offsetPY < GunPosOffsetPY) {
					pos.y = CrossPositionTwo.y;
				}
			}
			else {
				if (offsetPY < GunPosOffsetGaoBaoDanPY) {
					pos.y = CrossPositionTwo.y;
				}
			}
		}
		#if TEST_SHUIQIANG_ZUOBIAO
		Screen.showCursor = !bIsHardWare;
		pos.x += ShuiQiangX * XkGameCtrl.ScreenWidth;
		pos.y += ShuiQiangY * XkGameCtrl.ScreenHeight;
		#endif

		CrossPositionTwo = pos;

		if (bIsHardWare) {
			if (ZhunXingCtrl.GetInstanceTwo() != null && ZhunXingCtrl.GetInstanceTwo().GetActiveZhunXing()) {
				ZhunXingCtrl.GetInstanceTwo().FixedUpdateGunCross();
			}
		}
	}

	#if TEST_SHUIQIANG_ZUOBIAO
	void OnGUI()
	{
		string testA = "djsp1 "+ZuoYiDianJiSpeedVal[0]+", djsp2 "+ZuoYiDianJiSpeedVal[1]
		+", djstp1 "+XKPlayerAutoFire.DianJiState[0]+", djstp2 "+XKPlayerAutoFire.DianJiState[1];
		GUI.Label(new Rect(10f, 55f, Screen.width, 30f), testA);
	}
	#endif

	static void RandomJiaoYanDt()
	{	
		for (int i = 1; i < 4; i++) {
			JiaoYanDt[i] = (byte)UnityEngine.Random.Range(0x00, 0x7b);
		}
		JiaoYanDt[0] = 0x00;
		for (int i = 1; i < 4; i++) {
			JiaoYanDt[0] ^= JiaoYanDt[i];
		}
	}
	
	public void StartJiaoYanIO()
	{
		if (IsJiaoYanHid) {
			return;
		}
		
		if (!HardwareCheckCtrl.IsTestHardWare) {
			if (JiaoYanSucceedCount >= JiaoYanFailedMax) {
				return;
			}
			
			if (JiaoYanState == JIAOYANENUM.FAILED && JiaoYanFailedCount >= JiaoYanFailedMax) {
				return;
			}
		}
		else {
			HardwareCheckCtrl.Instance.SetJiaMiJYMsg("校验中...", JiaMiJiaoYanEnum.Null);
		}
		RandomJiaoYanDt();
		JiaoYanCheckCount = 0;
		IsJiaoYanHid = true;
		CancelInvoke("CloseJiaoYanIO");
		Invoke("CloseJiaoYanIO", 3f);
		//ScreenLog.Log("开始校验...");
	}
	
	void CloseJiaoYanIO()
	{
		if (!IsJiaoYanHid) {
			return;
		}
		IsJiaoYanHid = false;
		OnEndJiaoYanIO(JIAOYANENUM.FAILED);
	}
	
	void OnEndJiaoYanIO(JIAOYANENUM val)
	{
		IsJiaoYanHid = false;
		if (IsInvoking("CloseJiaoYanIO")) {
			CancelInvoke("CloseJiaoYanIO");
		}
		
		switch (val) {
		case JIAOYANENUM.FAILED:
			JiaoYanFailedCount++;
			if (HardwareCheckCtrl.IsTestHardWare) {
				HardwareCheckCtrl.Instance.JiaMiJiaoYanFailed();
			}
			break;
			
		case JIAOYANENUM.SUCCEED:
			JiaoYanSucceedCount++;
			JiaoYanFailedCount = 0;
			if (HardwareCheckCtrl.IsTestHardWare) {
				HardwareCheckCtrl.Instance.JiaMiJiaoYanSucceed();
			}
			break;
		}
		JiaoYanState = val;
		//Debug.Log("*****JiaoYanState "+JiaoYanState);
		
		if (JiaoYanFailedCount >= JiaoYanFailedMax || IsJiOuJiaoYanFailed) {
			//JiaoYanFailed
			if (IsJiOuJiaoYanFailed) {
				//JiOuJiaoYanFailed
				//Debug.Log("JOJYSB...");
			}
			else {
				//JiaMiXinPianJiaoYanFailed
				//Debug.Log("JMXPJYSB...");
				IsJiaMiJiaoYanFailed = true;
			}
		}
	}
	public static bool IsJiaMiJiaoYanFailed;
	
	enum JIAOYANENUM
	{
		NULL,
		SUCCEED,
		FAILED,
	}
	static int JiaoYanCheckCount;
	static JIAOYANENUM JiaoYanState = JIAOYANENUM.NULL;
	static byte JiaoYanFailedMax = 0x01;
	static byte JiaoYanSucceedCount;
	static byte JiaoYanFailedCount;
	static byte[] JiaoYanDt = new byte[4];
	static byte[] JiaoYanMiMa = new byte[4];
	static byte[] JiaoYanMiMaRand = new byte[4];
	byte JiOuJiaoYanCount;
	byte JiOuJiaoYanMax = 5;
	public static bool IsJiOuJiaoYanFailed;
	
	void InitJiaoYanMiMa()
	{
		JiaoYanMiMa[1] = 0x8e; //0x8e
		JiaoYanMiMa[2] = 0xc3; //0xc3
		JiaoYanMiMa[3] = 0xd7; //0xd7
		JiaoYanMiMa[0] = 0x00;
		for (int i = 1; i < 4; i++) {
			JiaoYanMiMa[0] ^= JiaoYanMiMa[i];
		}
	}
	
	void RandomJiaoYanMiMaVal()
	{
		for (int i = 0; i < 4; i++) {
			JiaoYanMiMaRand[i] = (byte)UnityEngine.Random.Range(0x00, (JiaoYanMiMa[i] - 1));
		}
		
		byte TmpVal = 0x00;
		for (int i = 1; i < 4; i++) {
			TmpVal ^= JiaoYanMiMaRand[i];
		}
		
		if (TmpVal == JiaoYanMiMaRand[0]) {
			JiaoYanMiMaRand[0] ^= 0x01; //fix JiaoYanMiMaRand[0].
		}
	}

	public static void SetGunZhenDongDengJi(int val, PlayerEnum playerVal)
	{
		switch (playerVal) {
		case PlayerEnum.PlayerOne:
			QiangZhenDongP1 = val;
			break;
		case PlayerEnum.PlayerTwo:
			QiangZhenDongP2 = val;
			break;
		}
	}

	static bool IsCloseGunZhenDong;
	public static void CloseGunZhenDongDengJi()
	{
		IsCloseGunZhenDong = true;
	}

	public static void OpenPlayerGunZhenDong()
	{
		IsCloseGunZhenDong = false;
	}
	
	public static void SetPlayerZuoYiDianJiSpeed(int val, PlayerEnum playerVal)
	{
		switch (playerVal) {
		case PlayerEnum.PlayerOne:
			DianJiSpeedP1 = val;
			break;
		case PlayerEnum.PlayerTwo:
			DianJiSpeedP2 = val;
			break;
		}
	}

	/**
	 * 座椅电机速度设置.
	 * ZuoYiDianJiSpeedVal == 0x1x -> 电机向下运动.
	 * ZuoYiDianJiSpeedVal == 0x0x -> 电机向上运动.
	 */
	public static byte[] ZuoYiDianJiSpeedVal = {0, 0, 0, 0};
	/**
	 * moveState == 1 -> 向上.
	 * moveState == 0 -> 停止.
	 * moveState == -1 -> 向下.
	 */
	public void SetZuoYiDianJiSpeed(PlayerEnum indexPlayer, int moveState)
	{
		int indexVal = (int)indexPlayer - 1;
		byte speedTmp = 0x00;
		byte speed = 0x00;
		switch (indexPlayer) {
		case PlayerEnum.PlayerOne:
			speed = (byte)DianJiSpeedP1;
			break;
		case PlayerEnum.PlayerTwo:
			speed = (byte)DianJiSpeedP2;
			break;
		}

		switch (XKPlayerAutoFire.DianJiState[indexVal]) {
		case 0:
			speed = (XkGameCtrl.GetIsActivePlayer(indexPlayer) == true && IsOpenQiNangQian == true) ? speed : (byte)0x00;
			break;
		case 1:
			if (speed + 5 > 15) {
				speed = 15;
			}
			else {
				speed = (byte)(speed + 5);
			}
			speed = XkGameCtrl.GetIsActivePlayer(indexPlayer) == true ? speed : (byte)0x00;
			break;
		case 2:
			speed = XkGameCtrl.GetIsActivePlayer(indexPlayer) == true ? (byte)15 : (byte)0x00;
			break;
		}

		switch (moveState) {
		case 1:
			speedTmp = speed;
			SetRunZuoYiState(indexPlayer, 0);
			break;
		case 0:
			speedTmp = 0x00;
			SetRunZuoYiState(indexPlayer, 1);
			break;
		case -1:
			speedTmp = (byte)(0x10 + speed);
			SetRunZuoYiState(indexPlayer, 0);
			break;
		}
		ZuoYiDianJiSpeedVal[indexVal] = speedTmp;
	}

	/**
	 * RunZuoYiState[0] == 0 -> run zuoYi.
	 * RunZuoYiState[0] == 1 -> stop zuoYi.
	 * RunZuoYiState[0] -> player1.
	 * RunZuoYiState[1] -> player2.
	 */
	static byte[] RunZuoYiState = {0, 0, 0, 0};
	/**
	 * runVal == 0 -> run zuoYi.
	 * runVal == 1 -> stop zuoYi.
	 */
	public static void SetRunZuoYiState(PlayerEnum indexPlayer, byte runVal)
	{
		int indexVal = (int)indexPlayer - 1;
		RunZuoYiState[indexVal] = runVal;
	}

	/**
	 * ZuoYiTrigger[0] -> 玩家1座椅传感器1.
	 * ZuoYiTrigger[1] -> 玩家1座椅传感器2.
	 * ZuoYiTrigger[2] -> 玩家1座椅传感器3.
	 */
#if !COM_TANK_TEST
	byte[] ZuoYiTrigger = { 0, 0, 0,
		0, 0, 0,
		0, 0, 0,
		0, 0, 0};
#endif
	/**
	 * indexTrigger ==  1 -> 上.
	 * indexTrigger ==  0 -> 中.
	 * indexTrigger == -1 -> 下.
	 */
	void OnZuoYiDianJiMoveOver(PlayerEnum indexPlayer, int indexTrigger, ButtonState btState)
	{
		/*Debug.Log("OnZuoYiDianJiMoveOver -> indexPlayer "+indexPlayer+", indexTrigger "+indexTrigger
		          +", btState "+btState);*/
		int indexVal = (int)indexPlayer - 1;
		if (btState == ButtonState.DOWN) {
			switch (indexTrigger) {
			case 1:
				SetZuoYiDianJiSpeed(indexPlayer, -1);
				break;
			case -1:
				if (RunZuoYiState[indexVal] == 0) {
					SetZuoYiDianJiSpeed(indexPlayer, 1);
				}
				else {
					SetZuoYiDianJiSpeed(indexPlayer, 0);
				}
				break;
			}
			//SetZuoYiDianJiSpeed(indexPlayer, 0);
		}
		
		if (HardwareCheckCtrl.IsTestHardWare) {
			bool isActiveTrigger = btState == ButtonState.DOWN ? true : false;
			switch(indexPlayer) {
			case PlayerEnum.PlayerOne:
				switch(indexTrigger) {
				case 1:
					HardwareCheckCtrl.Instance.SetZuoYiActiveShangP1(isActiveTrigger);
					break;
				case -1:
					HardwareCheckCtrl.Instance.SetZuoYiActiveXiaP1(isActiveTrigger);
					break;
				}
				break;
			case PlayerEnum.PlayerTwo:
				switch(indexTrigger) {
				case 1:
					HardwareCheckCtrl.Instance.SetZuoYiActiveShangP2(isActiveTrigger);
					break;
				case -1:
					HardwareCheckCtrl.Instance.SetZuoYiActiveXiaP2(isActiveTrigger);
					break;
				}
				break;
			}
		}
	}

	/**
	 * FangXiangPanDouDongVal == 0x00 -> 关闭力反馈.
	 * FangXiangPanDouDongVal == 0x55 -> 方向抖动.
	 * FangXiangPanDouDongVal == 0xaa -> 打开力反馈.
	 * FangXiangPanDouDongVal == 0x11 -> 停止方向抖动.
	 */
	public static byte[] FangXiangPanDouDongVal = {0, 0, 0, 0};
	/**
	 * FangXiangPanDouDong == 0 -> 抖动关闭.
	 * FangXiangPanDouDong == 1 -> 抖动打开.
	 */
	public static byte[] FangXiangPanDouDong = {0, 0, 0, 0};
	/**
	 * 方向盘是否循环抖动控制.
	 */
	public static byte[] FangXiangPanDouDongLPVal = {0, 0, 0, 0};
	public void ActiveFangXiangDouDong(PlayerEnum playerVal, bool isLoopDouDong)
	{
		int indexVal = (int)playerVal - 1;
		FangXiangPanDouDong[indexVal] = 1;
		FangXiangPanDouDongVal[indexVal] = 0xaa;
		FangXiangPanDouDongLPVal[indexVal] = (byte)(isLoopDouDong == true ? 1 : 0);
		StopCoroutine(PlayFangXiangPanDouDong(playerVal));
		StartCoroutine(PlayFangXiangPanDouDong(playerVal));
	}
	
	IEnumerator PlayFangXiangPanDouDong(PlayerEnum playerVal)
	{
		bool isPlayDouDong = true;
		int count = 0;
		int douDongCiShu = 1;
		int douDongMax = douDongCiShu * 2;
		int indexVal = (int)playerVal - 1;
		do {
			if (HardwareCheckCtrl.IsTestHardWare) {
				Debug.Log("PlayFangXiangPanDouDong -> playerVal "+playerVal+", count "+count);
			}

			if ((count >= douDongMax && FangXiangPanDouDongLPVal[indexVal] == 0)
			    || FangXiangPanDouDong[indexVal] == 0) {
				FangXiangPanDouDongVal[indexVal] = 0xaa;
				isPlayDouDong = false;
				yield break;
			}
			FangXiangPanDouDongVal[indexVal] = (byte)(count % 2 == 0 ? 0x55 : 0x00);
			count++;
			yield return new WaitForSeconds(0.3f);
		} while (isPlayDouDong);
	}
	
	public void CloseFangXiangPanPower(PlayerEnum playerVal)
	{
		if (playerVal != PlayerEnum.Null) {
			int indexVal = (int)playerVal - 1;
			FangXiangPanDouDongVal[indexVal] = 0x00;
			FangXiangPanDouDong[indexVal] = 0;
			StopCoroutine(PlayFangXiangPanDouDong(playerVal));
		}
		else {
			StopCoroutine(PlayFangXiangPanDouDong(PlayerEnum.PlayerOne));
			StopCoroutine(PlayFangXiangPanDouDong(PlayerEnum.PlayerTwo));
			StopCoroutine(PlayFangXiangPanDouDong(PlayerEnum.PlayerThree));
			StopCoroutine(PlayFangXiangPanDouDong(PlayerEnum.PlayerFour));
			for (int i = 0; i < FangXiangPanDouDong.Length; i++) {
				FangXiangPanDouDong[i] = 0;
				FangXiangPanDouDongVal[i] = 0x00;
			}
		}
	}

	public void OpenFangXiangPanPower(PlayerEnum playerVal)
	{
		if (playerVal != PlayerEnum.Null) {
			int indexVal = (int)playerVal - 1;
			FangXiangPanDouDong[indexVal] = 0;
			FangXiangPanDouDongVal[indexVal] = 0xaa;
			StopCoroutine(PlayFangXiangPanDouDong(playerVal));
		}
		else {
			StopCoroutine(PlayFangXiangPanDouDong(PlayerEnum.PlayerOne));
			StopCoroutine(PlayFangXiangPanDouDong(PlayerEnum.PlayerTwo));
			StopCoroutine(PlayFangXiangPanDouDong(PlayerEnum.PlayerThree));
			StopCoroutine(PlayFangXiangPanDouDong(PlayerEnum.PlayerFour));
			for (int i = 0; i < FangXiangPanDouDong.Length; i++) {
				FangXiangPanDouDong[i] = 0;
				FangXiangPanDouDongVal[i] = 0xaa;
			}
		}
	}

	/**
	 * 185 - 255新版本枪震动控制数据,以10为单位递增.
	 */
	byte[] GunZhenDongArray = {	0x00,
								0x01,
								0x02,
								0x03,
								0x04,
								0x05,
								0x06,
								0x07,
								0x08,
								0x09,
								0x0A,
								0x0B,
								0x0C,
								0x0D,
								0x0E,
								0x0F	};
	byte GetPlayerGunZhenDongVal(int gunZDVal)
	{
		if (gunZDVal < 0 || gunZDVal >= GunZhenDongArray.Length) {
			gunZDVal = 0;
		}

		if (MyCOMDevice.PcvrComSt == PcvrComState.TanKeGunZhenDong) {
			return GunZhenDongArray[gunZDVal];
		}

		byte gunZDTmp = 0;
		if (gunZDVal > 1) {
			gunZDTmp = (byte)(190 + (gunZDVal - 2) * 5);
		}
		return gunZDTmp;
	}

	public static bool IsPlayerActivePcvr = true;
	public static float TimeLastActivePcvr;
	void CheckIsPlayerActivePcvr()
	{
		if (!IsPlayerActivePcvr) {
			return;
		}

		if (Time.realtimeSinceStartup - TimeLastActivePcvr > 60f) {
			IsPlayerActivePcvr = false;
		}
	}

	public static void SetIsPlayerActivePcvr()
	{
		if (!bIsHardWare) {
			return;
		}
		IsPlayerActivePcvr = true;
		TimeLastActivePcvr = Time.realtimeSinceStartup;
	}

	public static void UpdatePcvrCrossPos(byte indexVal, Point pos)
	{
			PlayerEnum indexPlayer = PlayerEnum.Null;
			indexPlayer = (PlayerEnum)(indexVal + 1);
			Vector3 posCross = new Vector3(pos.X, pos.Y, 0f);
			switch (indexPlayer) {
			case PlayerEnum.PlayerOne:
					CrossPositionOne = posCross;
					break;

			case PlayerEnum.PlayerTwo:
					CrossPositionTwo = posCross;
					break;

			case PlayerEnum.PlayerThree:
					CrossPositionThree = posCross;
					break;

			case PlayerEnum.PlayerFour:
					CrossPositionFour = posCross;
					break;
			}
	}
}

public enum PcvrValState
{
	ValMax,
	ValMin,
	ValCenter
}

public enum PcvrShuiBengState
{
	Level_1,
	Level_2
}

public enum LedState
{
	Liang,
	Shan,
	Mie
}

public enum AdjustGunDrossState
{
	GunCrossLU = 0,
	GunCrossRU,
	GunCrossRD,
	GunCrossLD,
	GunCrossOver
}