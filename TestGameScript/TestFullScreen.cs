using UnityEngine;
using System.Collections;

using System;
using System.Runtime.InteropServices;

public enum DMDO 
{
	DEFAULT = 0,
	D90 = 1,
	D180 = 2,
	D270 = 3,
}

public class TestFullScreen : MonoBehaviour
{
	public GameObject NetworkRpcObj;
	public GameObject RootObj;
	public GameObject StartPageObj;
	
	public UISprite UiSpriteObj;
	public GameObject CointInfo;
	public GameObject StartBtObj;
	
	public bool IsServerPort = false;
	public static bool IsServer = false;
	public static bool IsRecordServerInfo = false;
	
	public Rect screenPosition;

	[DllImport("user32")]
	static extern IntPtr SetWindowLong (IntPtr hwnd, int _nIndex , int dwNewLong);  
	[DllImport("user32")]  
	static extern bool SetWindowPos (IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);  
	[DllImport("user32")]  
	static extern IntPtr GetForegroundWindow ();
	[DllImport("user32")]
	static extern bool GetWindowRect(IntPtr hWnd, ref Rect rect);
	[DllImport("user32")]
	static extern int GetWindowLong(IntPtr hWnd, int nIndex);
	[DllImport("user32")]
	static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);
	[DllImport("user32")]
	public static extern int GetSystemMetrics(int nIndex);
	[DllImport("user32")]
	static extern int SetForegroundWindow(IntPtr hwnd);
	[DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
	static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
	[DllImport("user32")]
	static extern bool SetMenu(IntPtr hWnd, IntPtr hMenu);
	
	const int GWL_STYLE = -16;  
	const int WS_BORDER = 1;  
	const int WS_POPUP = 0x800000;
	const int WS_SYSMENU = 0x80000;
	
	
	const int SWP_NOSIZE = 0x0001;
	const int SWP_NOMOVE = 0x0002;
	const uint SW_SHOWNORMAL = 1;
	const int HWND_NOTOPMOST = 0xffffffe;
	
	const int WS_CAPTION = (int)0x00C00000; 
	const int WS_CHILD = (int)0x40000000; 
	
	const uint SWP_SHOWWINDOW = 0x0040;  
	const uint SWP_DRAWFRAME = 0x0020;
	const uint SWP_DEFERERASE = 0x2000;
	const uint SWP_FRAMECHANGED = 0x0020;
	//int HWND_TOP = 0;
	public static int SM_CXSCREEN = 0;
	public static int SM_CYSCREEN = 1;
	
	bool IsChangePos = false;
	
	void ChangeWindowPos()  
	{	
		if(IsChangePos)
		{
			return;
		}
		IsChangePos = true;
		
		Screen.fullScreen = false;
		Invoke("fixWindowPos", 3.0f); //move the game to child screen
		Invoke("fixWindowPos", 5.0f); //make the game full screen
	}
	
	void FullScreenViewCtrl()
	{
		if(Network.isClient)
		{
			//Screen.fullScreen = true;
			return;
		}
		
		IntPtr m_hWnd = GetForegroundWindow();
		
		//IntPtr pParentWndSave = new IntPtr(); //父窗口句柄.
		//IntPtr pParentWndSaveTmp = new IntPtr(); //父窗口句柄.
		int dwWindowStyleSave = 0; //窗口风格.
		//Rect rcWndRectSave = new Rect(0, 0, 0, 0); //窗口位置.
		
		int iScreenW = GetSystemMetrics(SM_CXSCREEN);
		//int iScreenH = GetSystemMetrics(SM_CYSCREEN);
		
		dwWindowStyleSave = GetWindowLong(m_hWnd, GWL_STYLE); //保存窗口风格.
		
		//GetWindowRect(m_hWnd, &rcWndRectSave); //保存窗口位置.
		
		//pParentWndSave = SetParent(m_hWnd, pParentWndSaveTmp); //保存父窗口句柄/设置父窗口.
		
		SetWindowLong(m_hWnd, GWL_STYLE,
		              dwWindowStyleSave & (~WS_CHILD) & (~WS_CAPTION) & (~WS_BORDER));//使窗口不具有CAPTION风格.
		
		uint SWP = SWP_DRAWFRAME | SWP_DEFERERASE | SWP_FRAMECHANGED | SWP_SHOWWINDOW;
		SWP = SWP_SHOWWINDOW;
		//SetWindowPos(m_hWnd, 0, iScreenW, 0, iScreenW, iScreenH, SWP); //修改窗口置全屏.
		SetWindowPos(m_hWnd, 0, iScreenW, 0, 800, 600, SWP); //修改窗口置全屏.
		
		IntPtr ptr = new IntPtr();
		m_hWnd = new IntPtr();
		SetMenu(m_hWnd, ptr); //取消边框.
	}
	
	void SetFullScreenTest()
	{
		//IntPtr ptr = new IntPtr();
		IntPtr m_hWnd = GetForegroundWindow();
		int iScreenW = GetSystemMetrics(SM_CXSCREEN);
		int iScreenH = GetSystemMetrics(SM_CYSCREEN);
		
		SetWindowLong( m_hWnd, GWL_STYLE, WS_POPUP|WS_SYSMENU );
		//SetWindowPos( m_hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED );
		
		uint SWP = SWP_DRAWFRAME | SWP_DEFERERASE | SWP_FRAMECHANGED | SWP_SHOWWINDOW;
		SWP = SWP_SHOWWINDOW;
		SetWindowPos(m_hWnd, 0, iScreenW, 0, iScreenW, iScreenH, SWP); //修改窗口置全屏.
		
		ShowWindow( m_hWnd, SW_SHOWNORMAL );
		
		//SetMenu(m_hWnd, ptr);
	}
	
	void fixWindowPos()
	{
		FullScreenViewCtrl();
		//ScreenLog.Log("ChangeWindowPos..." + Screen.width + " " + Screen.height);
	}
	
	void makeGameFullScreen()
	{
		Screen.fullScreen = true;
	}
	
	void CheckIsFullScreen()
	{
		if(Time.frameCount % 300 == 0)
		{
			if(!IsServer)
			{
				bool IsFullScreen = true;
				if(IsFullScreen)
				{
					if(Screen.width != 1360 || !Screen.fullScreen)
					{
						////ScreenLog.Log("test******************** Screen.width " + Screen.width);
						Screen.SetResolution(1360, 768, true);
						//Invoke("makeGameFullScreen", 2.0f);
					}
				}
				else
				{
					if(Screen.width != 400)
					{
						////ScreenLog.Log("test******************** Screen.width " + Screen.width);
						Screen.SetResolution(400, 300, false);
					}
				}
			}
		}
	}


	// Use this for initialization
	void Awake () {
		
//		　DEVMODE dm = new DEVMODE(); 
//		　dm.dmSize= (short)Marshal.SizeOf(typeof(DEVMODE)); 
//		　dm.dmPelsWidth = 1024; 
//		　dm.dmPelsHeight= 768; 
//		　dm.dmDisplayFrequency=85; 
//		　dm.dmFields = DEVMODE.DM_PELSWIDTH | DEVMODE.DM_PELSHEIGHT|DEVMODE.DM_DISPLAYFREQUENCY; 
//		ChangeDisplaySettings(ref dm, 0); 
		
		//		Screen.showCursor = false;
		//		if(!IsRecordServerInfo)
		//		{
		//			IsRecordServerInfo = true;
		//			IsServer = IsServerPort;
		//
		//			if(!IsServer)
		//			{
		//				Screen.SetResolution(1360, 768, false);
		//				Invoke("makeGameFullScreen", 2.0f);
		//			}
		//			else
		//			{
		//				//Screen.fullScreen = false;
		//				if(RootObj != null)
//				{
//					RootObj.SetActive(false);
//				}
//				ChangeWindowPos();
//			}
//		}
	}
}