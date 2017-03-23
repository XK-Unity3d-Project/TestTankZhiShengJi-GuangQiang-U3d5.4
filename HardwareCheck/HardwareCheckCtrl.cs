using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class HardwareCheckCtrl : MonoBehaviour
{
	public PcvrComState PcvrComSt;
	public UILabel[] BiZhiLable;
	public UILabel[] QiangPosLable;
	public UILabel[] QiangZDLable;
	public UILabel AnJianLable;
	public UILabel StartLedP1;
	public UILabel StartLedP2;
	public UILabel[] QiNangLabel;
	public GameObject JiaMiCeShiObj;
	public bool IsJiaMiCeShi;
	int StartLedNumP1 = 1;
	int StartLedNumP2 = 1;
	public static bool IsTestHardWare;
	public static HardwareCheckCtrl Instance;
	void Awake()
	{
		if (PcvrComSt == PcvrComState.TanKeFangXiangZhenDong) {
			MyCOMDevice.PcvrComSt = PcvrComSt;
		}
		else {
			MyCOMDevice.PcvrComSt = PcvrComState.TanKeGunZhenDong;
		}
	}

	// Use this for initialization
	void Start()
	{
		Screen.SetResolution(1360, 768, false);
		SetZuoYiActiveShangP1(false);
		SetZuoYiActiveXiaP1(false);
		SetZuoYiActiveShangP2(false);
		SetZuoYiActiveXiaP2(false);
		Instance = this;
		IsTestHardWare = true;
		JiaMiCeShiObj.SetActive(IsJiaMiCeShi);
		BiZhiLable[0].text = "0";
		BiZhiLable[1].text = "0";
		AnJianLable.text = "...";

		HardwareBtCtrl.StartLedP1 = StartLedP1;
		HardwareBtCtrl.StartLedP2 = StartLedP2;

		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;
		InputEventCtrl.GetInstance().ClickFireBtOneEvent += ClickFireBtOneEvent;
		InputEventCtrl.GetInstance().ClickFireBtTwoEvent += ClickFireBtTwoEvent;
		InputEventCtrl.GetInstance().ClickDaoDanBtOneEvent += ClickDaoDanBtOneEvent;
		InputEventCtrl.GetInstance().ClickDaoDanBtTwoEvent += ClickDaoDanBtTwoEvent;
		InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartP1BtEvent;
		InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickStartP2BtEvent;
		InputEventCtrl.GetInstance().ClickStopDongGanBtOneEvent += ClickStopDongGanBtOneEvent;
	}
	
	// Update is called once per frame
	void Update()
	{
		BiZhiLable[0].text = XKGlobalData.CoinPlayerOne.ToString();
		BiZhiLable[1].text = XKGlobalData.CoinPlayerTwo.ToString();
		QiangPosLable[0].text = "X-> "+pcvr.MousePositionP1.x.ToString();
		QiangPosLable[1].text = "Y-> "+pcvr.MousePositionP1.y.ToString();
		QiangPosLable[2].text = "X-> "+pcvr.MousePositionP2.x.ToString();
		QiangPosLable[3].text = "Y-> "+pcvr.MousePositionP2.y.ToString();
	}

	void ClickSetEnterBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "设置 Down";
		}
		else {
			AnJianLable.text = "设置 Up";
		}
	}

	void ClickSetMoveBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "移动 Down";
		}
		else {
			AnJianLable.text = "移动 Up";
		}
	}

	void ClickStartP1BtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "1P开始 Down";
		}
		else {
			AnJianLable.text = "1P开始 Up";
		}
	}

	void ClickStartP2BtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "2P开始 Down";
		}
		else {
			AnJianLable.text = "2P开始 Up";
		}
	}
	
	void ClickStopDongGanBtOneEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "紧急 Down";
		}
		else {
			AnJianLable.text = "紧急 Up";
		}
	}

	void ClickFireBtOneEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "1P射击 Down";
		}
		else {
			AnJianLable.text = "1P射击 Up";
		}
	}

	void ClickFireBtTwoEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "2P射击 Down";
		}
		else {
			AnJianLable.text = "2P射击 Up";
		}
	}
	
	void ClickDaoDanBtOneEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "1P导弹 Down";
		}
		else {
			AnJianLable.text = "1P导弹 Up";
		}
	}
	
	void ClickDaoDanBtTwoEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "2P导弹 Down";
		}
		else {
			AnJianLable.text = "2P导弹 Up";
		}
	}

	public UILabel JiaMiJYLabel;
	public UILabel JiaMiJYMsg;
	public static bool IsOpenJiaMiJiaoYan;
	void CloseJiaMiJiaoYanFailed()
	{
		if (!IsInvoking("JiaMiJiaoYanFailed")) {
			return;
		}
		CancelInvoke("JiaMiJiaoYanFailed");
	}

	public void JiaMiJiaoYanFailed()
	{
		SetJiaMiJYMsg("", JiaMiJiaoYanEnum.Failed);
	}
	
	public void JiaMiJiaoYanSucceed()
	{
		SetJiaMiJYMsg("", JiaMiJiaoYanEnum.Succeed);
	}

	public void SetJiaMiJYMsg(string msg, JiaMiJiaoYanEnum key)
	{
		switch (key) {
		case JiaMiJiaoYanEnum.Succeed:
			CloseJiaMiJiaoYanFailed();
			JiaMiJYMsg.text = "校验成功";
			ResetJiaMiJYLabelInfo();
			ScreenLog.Log("校验成功");
			break;
			
		case JiaMiJiaoYanEnum.Failed:
			CloseJiaMiJiaoYanFailed();
			JiaMiJYMsg.text = "校验失败";
			ResetJiaMiJYLabelInfo();
			ScreenLog.Log("校验失败");
			break;
			
		default:
			JiaMiJYMsg.text = msg;
			ScreenLog.Log(msg);
			break;
		}
	}
	
	public static void CloseJiaMiJiaoYan()
	{
		if (!IsOpenJiaMiJiaoYan) {
			return;
		}
		IsOpenJiaMiJiaoYan = false;
	}

	void ResetJiaMiJYLabelInfo()
	{
		CloseJiaMiJiaoYan();
		JiaMiJYLabel.text = "加密校验";
	}

	public void SubCoinPOne()
	{
		if (XKGlobalData.CoinPlayerOne < 1) {
			return;
		}
		XKGlobalData.CoinPlayerOne--;
		pcvr.GetInstance().SubPlayerCoin(1, PlayerEnum.PlayerOne);
	}
	
	public void SubCoinPTwo()
	{
		if (XKGlobalData.CoinPlayerTwo < 1) {
			return;
		}
		XKGlobalData.CoinPlayerTwo--;
		pcvr.GetInstance().SubPlayerCoin(1, PlayerEnum.PlayerTwo);
	}

	public void StartLedCheckP1()
	{
		StartLedNumP1++;
		switch (StartLedNumP1) {
		case 1:
			StartLedP1.text = "1P开始灯亮";
			pcvr.StartLightStateP1 = LedState.Liang;
			break;
			
		case 2:
			StartLedP1.text = "1P开始灯闪";
			pcvr.StartLightStateP1 = LedState.Shan;
			break;
			
		case 3:
			StartLedP1.text = "1P开始灯灭";
			pcvr.StartLightStateP1 = LedState.Mie;
			StartLedNumP1 = 1;
			break;
		}
	}
	
	public void StartLedCheckP2()
	{
		StartLedNumP2++;
		switch (StartLedNumP2) {
		case 1:
			StartLedP2.text = "2P开始灯亮";
			pcvr.StartLightStateP2 = LedState.Liang;
			break;
			
		case 2:
			StartLedP2.text = "2P开始灯闪";
			pcvr.StartLightStateP2 = LedState.Shan;
			break;
			
		case 3:
			StartLedP2.text = "2P开始灯灭";
			pcvr.StartLightStateP2 = LedState.Mie;
			StartLedNumP2 = 1;
			break;
		}
	}

	public void OnClickCloseAppBt()
	{
		Application.Quit();
	}
	
	public void OnClickRestartAppBt()
	{
		Application.Quit();
		RunCmd("start ComTest.exe");
	}

	public static void OnRestartGame()
	{
		string gameName = "";
		switch (GameTypeCtrl.AppTypeStatic) {
		case AppGameType.DanJiTanKe:
			gameName = "Tank.exe";
			break;
		case AppGameType.DanJiFeiJi:
			gameName = "Helicopter.exe";
			break;
		case AppGameType.LianJiTanKe:
			gameName = "TankClient.exe";
			break;
		case AppGameType.LianJiFeiJi:
			gameName = "HelicopterClient.exe";
			break;
		case AppGameType.LianJiServer:
			gameName = "GameServer.exe";
			break;
		default:
			return;
		}
		Application.Quit();
		RunCmd("start "+gameName);
	}

	static void RunCmd(string command)
	{
		//實例一個Process類，啟動一個獨立進程    
		Process p = new Process();    //Process類有一個StartInfo屬性，這個是ProcessStartInfo類，    
		//包括了一些屬性和方法，下面我們用到了他的幾個屬性：   
		p.StartInfo.FileName = "cmd.exe";           //設定程序名   
		p.StartInfo.Arguments = "/c " + command;    //設定程式執行參數   
		p.StartInfo.UseShellExecute = false;        //關閉Shell的使用    p.StartInfo.RedirectStandardInput = true;   //重定向標準輸入    p.StartInfo.RedirectStandardOutput = true;  //重定向標準輸出   
		p.StartInfo.RedirectStandardError = true;   //重定向錯誤輸出    
		p.StartInfo.CreateNoWindow = true;          //設置不顯示窗口    
		p.Start();   //啟動
	}

	public void StartJiaoYanIO()
	{
		pcvr.GetInstance().StartJiaoYanIO();
	}

	public void OnClickQiNangBt_1()
	{
		QiNangLabel[0].text = QiNangLabel[0].text != "1气囊充气" ? "1气囊充气" : "1气囊放气";
		pcvr.QiNangArray[0] = (byte)(pcvr.QiNangArray[0] != 1 ? 1 : 0);
	}

	public void OnClickQiNangBt_2()
	{
		QiNangLabel[1].text = QiNangLabel[1].text != "2气囊充气" ? "2气囊充气" : "2气囊放气";
		pcvr.QiNangArray[1] = (byte)(pcvr.QiNangArray[1] != 1 ? 1 : 0);
	}

	public void OnClickQiNangBt_3()
	{
		QiNangLabel[2].text = QiNangLabel[2].text != "3气囊充气" ? "3气囊充气" : "3气囊放气";
		pcvr.QiNangArray[2] = (byte)(pcvr.QiNangArray[2] != 1 ? 1 : 0);
	}

	public void OnClickQiNangBt_4()
	{
		QiNangLabel[3].text = QiNangLabel[3].text != "4气囊充气" ? "4气囊充气" : "4气囊放气";
		pcvr.QiNangArray[3] = (byte)(pcvr.QiNangArray[3] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBt_5()
	{
		QiNangLabel[4].text = QiNangLabel[4].text != "5气囊充气" ? "5气囊充气" : "5气囊放气";
		pcvr.QiNangArray[4] = (byte)(pcvr.QiNangArray[4] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBt_6()
	{
		QiNangLabel[5].text = QiNangLabel[5].text != "6气囊充气" ? "6气囊充气" : "6气囊放气";
		pcvr.QiNangArray[5] = (byte)(pcvr.QiNangArray[5] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBt_7()
	{
		QiNangLabel[6].text = QiNangLabel[6].text != "7气囊充气" ? "7气囊充气" : "7气囊放气";
		pcvr.QiNangArray[6] = (byte)(pcvr.QiNangArray[6] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBt_8()
	{
		QiNangLabel[7].text = QiNangLabel[7].text != "8气囊充气" ? "8气囊充气" : "8气囊放气";
		pcvr.QiNangArray[7] = (byte)(pcvr.QiNangArray[7] != 1 ? 1 : 0);
	}

	bool IsNewGunZD = false;
	int GetGunZhenDongVal(float gunZDProVal)
	{
		int maxGunZD = 255;
		int minGunZD = 185;
		int maxDJ = 15;
		maxDJ = maxGunZD - minGunZD + 1;
		int dengJiVal = (int)(gunZDProVal * (maxDJ - 1)) % maxDJ;
		dengJiVal += 1;
		float dengJiKey = (float)dengJiVal / maxDJ;

		int zdVal = (int)(dengJiKey * (maxGunZD - minGunZD) + minGunZD);
		return zdVal;
	}

	public void SetQiangZDValue_1()
	{
		if (UIProgressBar.current == null) {
			return;
		}
		int valZD = Mathf.RoundToInt(UIProgressBar.current.value * 255f);
		if (IsNewGunZD) {
			valZD = GetGunZhenDongVal(UIProgressBar.current.value);
		}
		QiangZDLable[0].text = valZD.ToString();
		pcvr.QiangZhenDongP1 = valZD;
	}

	public void SetQiangZDValue_2()
	{
		if (UIProgressBar.current == null) {
			return;
		}
		int valZD = Mathf.RoundToInt(UIProgressBar.current.value * 255f);
		if (IsNewGunZD) {
			valZD = GetGunZhenDongVal(UIProgressBar.current.value);
		}
		QiangZDLable[1].text = valZD.ToString();
		pcvr.QiangZhenDongP2 = valZD;
	}

	public UILabel DianJiSpeedLB;
	public void SetDianJiSpeed()
	{
		if (UIProgressBar.current == null) {
			return;
		}
		int valZD = Mathf.RoundToInt((UIProgressBar.current.value * 14f) + 1);
		DianJiSpeedLB.text = valZD.ToString("X2");
		pcvr.DianJiSpeedP1 = valZD;
		pcvr.DianJiSpeedP2 = valZD;

		for (int i = 0; i < pcvr.ZuoYiDianJiSpeedVal.Length; i++) {
			if (pcvr.ZuoYiDianJiSpeedVal[i] != 0x00) {
				pcvr.ZuoYiDianJiSpeedVal[i] = (byte)((0xf0 & pcvr.ZuoYiDianJiSpeedVal[i]) + (0x0f & valZD));
			}
		}
	}

	public GameObject[] ZuoYiDuiGouP1;
	public void SetZuoYiActiveShangP1(bool isActive)
	{
		ZuoYiDuiGouP1[0].SetActive(isActive);
	}

	public void SetZuoYiActiveXiaP1(bool isActive)
	{
		ZuoYiDuiGouP1[1].SetActive(isActive);
	}
	
	public void OnClickZuoYiShangP1()
	{
		UnityEngine.Debug.Log("OnClickZuoYiShangP1...");
		pcvr.GetInstance().SetZuoYiDianJiSpeed(PlayerEnum.PlayerOne, 1);
	}

	public void OnClickZuoYiXiaP1()
	{
		UnityEngine.Debug.Log("OnClickZuoYiXiaP1...");
		pcvr.GetInstance().SetZuoYiDianJiSpeed(PlayerEnum.PlayerOne, -1);
	}
	
	public void OnClickZuoYiTingP1()
	{
		UnityEngine.Debug.Log("OnClickZuoYiTingP1...");
		pcvr.GetInstance().SetZuoYiDianJiSpeed(PlayerEnum.PlayerOne, 0);
	}

	public GameObject[] ZuoYiDuiGouP2;
	public void SetZuoYiActiveShangP2(bool isActive)
	{
		ZuoYiDuiGouP2[0].SetActive(isActive);
	}
	
	public void SetZuoYiActiveXiaP2(bool isActive)
	{
		ZuoYiDuiGouP2[1].SetActive(isActive);
	}
	
	public void OnClickZuoYiShangP2()
	{
		UnityEngine.Debug.Log("OnClickZuoYiShangP2...");
		pcvr.GetInstance().SetZuoYiDianJiSpeed(PlayerEnum.PlayerTwo, 1);
	}
	
	public void OnClickZuoYiXiaP2()
	{
		UnityEngine.Debug.Log("OnClickZuoYiXiaP2...");
		pcvr.GetInstance().SetZuoYiDianJiSpeed(PlayerEnum.PlayerTwo, -1);
	}
	
	public void OnClickZuoYiTingP2()
	{
		UnityEngine.Debug.Log("OnClickZuoYiTingP2...");
		pcvr.GetInstance().SetZuoYiDianJiSpeed(PlayerEnum.PlayerTwo, 0);
	}
	
	int[] FangXiangDouDongSt = {0, 0, 0, 0};
	public UILabel[] FangXiangDouDongLB;
	public void SetFangXiangDouDongP1()
	{
		int playerIndex = 0;
		UnityEngine.Debug.Log("SetFangXiangDouDong -> p"+playerIndex
		                      +": FangXiangDouDongSt "+FangXiangDouDongSt[playerIndex]);
		switch (FangXiangDouDongSt[playerIndex]) {
		case 0:
			FangXiangDouDongSt[playerIndex] = 1;
			FangXiangDouDongLB[0].text = "1P方向关闭";
			pcvr.GetInstance().ActiveFangXiangDouDong(PlayerEnum.PlayerOne, IsLoopDouDongFX);
			break;
		case 1:
			FangXiangDouDongSt[playerIndex] = 0;
			FangXiangDouDongLB[0].text = "1P方向抖动";
			pcvr.GetInstance().CloseFangXiangPanPower(PlayerEnum.PlayerOne);
			break;
		}
	}
	
	public void SetFangXiangDouDongP2()
	{
		int playerIndex = 1;
		UnityEngine.Debug.Log("SetFangXiangDouDong -> p"+playerIndex
		                      +": FangXiangDouDongSt "+FangXiangDouDongSt[playerIndex]);
		switch (FangXiangDouDongSt[playerIndex]) {
		case 0:
			FangXiangDouDongSt[playerIndex] = 1;
			FangXiangDouDongLB[1].text = "2P方向关闭";
			pcvr.GetInstance().ActiveFangXiangDouDong(PlayerEnum.PlayerTwo, IsLoopDouDongFX);
			break;
		case 1:
			FangXiangDouDongSt[playerIndex] = 0;
			FangXiangDouDongLB[1].text = "2P方向抖动";
			pcvr.GetInstance().CloseFangXiangPanPower(PlayerEnum.PlayerTwo);
			break;
		}
	}
	
	bool IsLoopDouDongFX = true;
	public void OnClickLoopFangXiangDouDong()
	{
		IsLoopDouDongFX = !IsLoopDouDongFX;
		UnityEngine.Debug.Log("OnClickLoopFangXiangDouDong -> IsLoopDouDongFX "+IsLoopDouDongFX);
	}
}

public enum JiaMiJiaoYanEnum
{
	Null,
	Succeed,
	Failed,
}