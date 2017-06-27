using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class RunSystemCmd
{
		public static void RunCmd(string command)
		{
				//實例一個Process類,啟動一個獨立進程.
				Process p = new Process();    //Process類有一個StartInfo屬性，這個是ProcessStartInfo類.
				//包括了一些屬性和方法，下面我們用到了他的幾個屬性.
				p.StartInfo.FileName = "cmd.exe";           //設定程序名
				p.StartInfo.Arguments = "/c " + command;    //設定程式執行參數
				p.StartInfo.UseShellExecute = false;        //關閉Shell的使用
				//p.StartInfo.RedirectStandardInput = true;   //重定向標準輸入
				//p.StartInfo.RedirectStandardOutput = true;  //重定向標準輸出
				p.StartInfo.RedirectStandardError = true;   //重定向錯誤輸出
				p.StartInfo.CreateNoWindow = true;          //設置不顯示窗口
				p.Start();   //啟動
		}
}