using UnityEngine;
using System.Diagnostics;

public class XKCheckGameServerIP
{
	public static void CheckServerIP()
	{
		if (Network.player.ipAddress == NetworkServerNet.ServerPortIP) {
			return;
		}
		ChangePcIP(2);
	}

	static void  ChangePcIP(int ip)
	{
		if (ip < 2 || ip > 255) {
			IsCloseCmd = true;
			return;
		}
		Screen.fullScreen = false;
		IsCloseCmd = false;
		IsOpenCmd = true;
		
		string cmd = "start ChangeIp.exe 2";
		RunCmd(cmd);
	}

	public static bool IsOpenCmd;
	public static bool IsCloseCmd;
	public static void CloseCmd()
	{
		if (!IsOpenCmd) {
			return;
		}

		if (IsCloseCmd) {
			return;
		}
		IsCloseCmd = true;
		UnityEngine.Debug.Log("CloseCmd...");

		RunCmd("ntsd -c q -pn ChangeIp.exe");
		
//		if (Network.player.ipAddress == NetworkServerNet.ServerPortIP) {
//			//ResetFullScreen...
//			//GameTypeCtrl.Instance.DelayResetFullScreen();
//			RestartGame();
//			return;
//		}
		//Delay CheckServerIP...
		//GameTypeCtrl.Instance.DelayCheckServerIP();
	}

	public static void RestartGame()
	{
		Application.Quit();
		string cmd = "start testServer.exe";
		RunCmd(cmd);
	}

	static void RunCmd(string command)
	{
		//實例一個Process類，啟動一個獨立進程.
		Process processObj = new Process();
		//設定程序名.  
		processObj.StartInfo.FileName = "cmd.exe";
		//設定程式執行參數.
		processObj.StartInfo.Arguments = "/c " + command;
		//關閉Shell的使用.
		processObj.StartInfo.UseShellExecute = false;
		
		//重定向標準輸入.
		//processObj.StartInfo.RedirectStandardInput = true;
		//重定向標準輸出.
		//processObj.StartInfo.RedirectStandardOutput = true;
		
		//重定向錯誤輸出.
		processObj.StartInfo.RedirectStandardError = true;
		//設置不顯示窗口.    
		processObj.StartInfo.CreateNoWindow = false;
		//啟動.
		processObj.Start();
	}
}