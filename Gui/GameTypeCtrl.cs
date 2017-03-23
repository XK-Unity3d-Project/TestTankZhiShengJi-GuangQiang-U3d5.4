using UnityEngine;
using System.Collections;

public enum AppGameType
{
	Null,
	DanJiTanKe,
	DanJiFeiJi,
	LianJiTanKe,
	LianJiFeiJi,
	LianJiServer,
}

public class GameTypeCtrl : MonoBehaviour
{
	/**
	 * IsZuoBiaoFanZhuanPY == true -> 准星坐标在Y轴进行反转.
	 * 1.单机坦克机台的准星坐标默认为Y轴反转的.
	 * 2.飞机机台的准星坐标默认为Y轴反转的(u型方向盘).
	 * 3.联机坦克机台的准星坐标默认为Y轴不反转的.
	 */
	bool IsZuoBiaoFanZhuanPY;
	/**
	 * GameTKPMState == TanKeGame2Ping -> 坦克游戏采用双屏(2个屏幕画面一致,2个玩家子弹发射点一致).
	 * GameTKPMState == other -> 坦克游戏采用单屏.
	 * 1.单机坦克可以选择双屏和单屏画面版本.
	 * 2.联机坦克默认为双屏画面版本.
	 * 3.直升机机台默认为单屏画面(主要针对玩家子弹发射点进行控制)版本.
	 */
	public PcvrComState GameTKPMState = PcvrComState.TanKeGunZhenDong;
	public static PcvrComState GameTKPMStatic = PcvrComState.TanKeGunZhenDong;
	/**
	 * PcvrGameSt == TanKeFangXiangZhenDong -> 在设置界面的准星校准时采用方向盘UI校准.
	 * PcvrGameSt == TanKeGunZhenDong -> 在设置界面的准星校准时采用枪震动UI校准.
	 * 1.单机坦克在设置界面默认采用方向盘UI校准.
	 * 2.联机机坦克在设置界面默认采用枪震动UI校准.
	 * 3.飞机机台在设置界面默认采用方向盘UI校准.
	 */
	PcvrComState PcvrGameSt = PcvrComState.TanKeGunZhenDong;
	public AppGameType AppType = AppGameType.Null;
	public GameObject NetCtrlObj;
	public static AppGameType AppTypeStatic = AppGameType.Null;
	public static bool IsServer;
	public static string TestGameFile = "TestGame.info";
	public static GameTypeCtrl Instance;
	void Awake()
	{
		Instance = this;
		switch (AppType) {
		case AppGameType.DanJiFeiJi:
		case AppGameType.LianJiFeiJi:
			MyCOMDevice.PcvrComSt = PcvrComState.TanKeGunZhenDong;
			IsZuoBiaoFanZhuanPY = true;
			PcvrGameSt = PcvrComState.TanKeFangXiangZhenDong;
			GameTKPMState = PcvrComState.TanKeFangXiangZhenDong;
			break;
		case AppGameType.DanJiTanKe:
		case AppGameType.LianJiTanKe:
			if (!pcvr.IsComTankTest) {
				MyCOMDevice.PcvrComSt = PcvrComState.TanKeFangXiangZhenDong;
			}
			IsZuoBiaoFanZhuanPY = AppType == AppGameType.DanJiTanKe ? true : false;
			PcvrGameSt = AppType == AppGameType.DanJiTanKe ?
									PcvrComState.TanKeFangXiangZhenDong : PcvrComState.TanKeGunZhenDong;
			GameTKPMState = AppType == AppGameType.DanJiTanKe ?
									GameTKPMState : PcvrComState.TanKeGame2Ping;
			break;
		}
		GameTKPMStatic = GameTKPMState;
		MyCOMDevice.PcvrGameSt = PcvrGameSt;
		pcvr.IsFanZhuanZuoBiaoPY = IsZuoBiaoFanZhuanPY;

		AppType = AppType == AppGameType.Null ? AppGameType.LianJiServer : AppType;
		if (!pcvr.bIsHardWare && DaoJiShiCtrl.IsTestActivePlayer) {
			string gameType = HandleJson.GetInstance().ReadFromFilePathXml(TestGameFile, "gameType");
			if (gameType == null || gameType == "") {
				gameType = "0";
				HandleJson.GetInstance().WriteToFilePathXml(TestGameFile, "gameType", gameType);
			}

			switch (gameType) {
			case "0":
				AppType = AppGameType.LianJiTanKe;
				break;
			case "1":
				AppType = AppGameType.LianJiFeiJi;
				break;
			case "2":
				AppType = AppGameType.LianJiServer;
				break;
			case "3":
				AppType = AppGameType.DanJiTanKe;
				break;
			case "4":
				AppType = AppGameType.DanJiFeiJi;
				break;
			}
			//Debug.Log("appType "+AppType+", gameType "+gameType);
		}

		AppTypeStatic = AppType;
		if (AppType == AppGameType.LianJiServer) {
			IsServer = true;
			DelayCheckServerIP();
		}
	}

	void Update()
	{
		if (Time.frameCount % 200 == 0) {
			System.GC.Collect();
			if (GameMovieCtrl.PlayerIPInfo == NetworkServerNet.ServerPortIP
			    && XKCheckGameServerIP.IsCloseCmd
			    && !Screen.fullScreen) {
				//ResetFullScreen...
				DelayResetFullScreen();
			}
		}
	}

	public void DelayCheckServerIP()
	{
		if (!pcvr.bIsHardWare) {
			return;
		}
		Invoke("CheckServerPortIP", 5f);
		//Invoke("DelayCloseCmd", 15f);
	}

	void CheckServerPortIP()
	{
		if (Network.player.ipAddress == NetworkServerNet.ServerPortIP) {
			//XKCheckGameServerIP.RestartGame();
			return;
		}

		XKCheckGameServerIP.CheckServerIP();
	}

	public void DelayResetFullScreen()
	{
		CancelInvoke("ResetFullScreen");
		Invoke("ResetFullScreen", 3f);
	}

	void ResetFullScreen()
	{
		Debug.Log("ResetFullScreen...");
		Screen.SetResolution(Screen.width, Screen.height, true);
	}

	void DelayCloseCmd()
	{
		XKCheckGameServerIP.CloseCmd();
	}
}