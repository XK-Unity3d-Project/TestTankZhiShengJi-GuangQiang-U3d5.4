using System.Diagnostics;
using UnityEngine;

public class XKMasterServerCtrl
{
	public static void CheckMasterServerIP()
	{
		KillSystemProcess("MasterServer");
		KillSystemProcess("Facilitator");
		OpenGameProcess("MasterServer/MasterServer.exe");
		OpenGameProcess("Facilitator/Facilitator.exe");
	}

	public static void CloseMasterServer()
	{
		KillSystemProcess("MasterServer");
		KillSystemProcess("Facilitator");
	}
	
	static void KillSystemProcess(string processName)
	{
		if (processName == "") {
			return;
		}

		Process[] myProcesses = Process.GetProcesses();
		//UnityEngine.Debug.Log("Length ** "+myProcesses.Length);
		foreach (Process process in myProcesses) {
			try
			{
				if (!process.HasExited)
				{
					//UnityEngine.Debug.Log("name --- "+process.ProcessName);
					if (process.ProcessName == processName) {
						process.Kill();
					}
				}
			}
			catch (System.InvalidOperationException)
			{
				//UnityEngine.Debug.Log("Holy batman we've got an exception!");
			}
		}
	}
	
	
	static void OpenGameProcess(string processName)
	{
		if (processName == "") {
			return;
		}

		Process p = new Process();
		p.StartInfo.FileName = processName;
		p.StartInfo.UseShellExecute = false;
		p.StartInfo.RedirectStandardInput = true;
		p.StartInfo.RedirectStandardOutput = true;
		p.StartInfo.RedirectStandardError = true;
		p.StartInfo.CreateNoWindow = true;//true表示不显示黑框，false表示显示dos界面
		
		p.Start();
		//要执行的dos命令  p.StandardInput.WriteLine("");
		//p.StandardInput.WriteLine("exit");
		
		//p.Close();
	}
}