using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ScreenLogType : int
{
	Normal  = 0,
	Warning = 1,
	Error   = 2
}

public class ScreenLogMsg
{
 	public string logInfo;
	public ScreenLogType logType;
}

public class ScreenLog : MonoBehaviour
{	
	static ScreenLog instance_;

	static List<ScreenLogMsg> logInfoList_ = new List<ScreenLogMsg>();

	float buttonX = 100;
	float buttonY = 10;
	
	bool touchMoveEnable = false;
	Vector2 touchPosition_ = new Vector2(0, 0);
	
	bool isShow = false;
	bool isLock = true;
	Vector2 scrollPosition = Vector2.zero;
	int recodeOffset = 50;

	GUISkin skin = null;
	GUIStyle styleError = new GUIStyle();
	GUIStyle styleWarning = new GUIStyle();
	
	public static void init()
	{
		if (!instance_)
		{
			GameObject go = new GameObject("ScreenLog");
			go.AddComponent<ScreenLog>();
			DontDestroyOnLoad(go);
		}
	}
	
	private void Awake()
	{
		instance_ = this;

		skin = ScriptableObject.CreateInstance<GUISkin>();
		skin.label.normal.textColor = new UnityEngine.Color(1, 1, 1);
		skin.label.wordWrap = true;

		styleError.normal.textColor = new UnityEngine.Color(1.0f, 0.0f, 0.0f);
		styleWarning.normal.textColor = new UnityEngine.Color(248.0f / 255.0f, 253.0f / 255.0f, 43.0f / 255.0f);
	}
	
	void Destroy()
	{
		UnityEngine.Object.Destroy(this.gameObject);
		instance_ = null;
	}

	int logCount = 0;
	void Update()
	{
		if(Input.GetKeyUp("`") && !pcvr.bIsHardWare)
		{
			isShow = !isShow;
			if(isShow)
			{
				Screen.lockCursor = false;
				Log("show Screen Log...");
			}
			else
			{
				Screen.lockCursor = true;
				Log("hidden Screen Log...");
			}
		}

		if(Input.GetKeyUp(KeyCode.F9) && isShow && !pcvr.bIsHardWare)
		{
			ClearScreenLog();
		}

		if (isShow)
		{
//			int testNum = Time.frameCount % 200;
//			if(testNum == 0)
//			{
//				Log("test ******************* logInfoList_.Count " + logInfoList_.Count);
//			}
//			else if(testNum == 1)
//			{
//				LogWarning("test 1111111111111111111");
//			}
//			else if(testNum == 2)
//			{
//				LogError("test 2222222222222222222");
//			}

			if (!isLock)
			{
				if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
				{
					Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
					scrollPosition.y += touchDeltaPosition.y;
					
					if (scrollPosition.y < 0)
					{
						scrollPosition.y = 0;
					}
					
					if (scrollPosition.y > recodeOffset * logInfoList_.Count - Screen.height)
					{
						scrollPosition.y = recodeOffset * logInfoList_.Count - Screen.height;
					}
				}
			}
		}
		else
		{
			if (Input.touchCount > 0)
			{
				if (Input.GetTouch(0).phase == TouchPhase.Moved && touchMoveEnable)
				{
					Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
					buttonX += touchDeltaPosition.x * 3;
					buttonY -= touchDeltaPosition.y * 3;
				}
				else if (Input.GetTouch(0).phase == TouchPhase.Began)
				{
					touchPosition_ = Input.GetTouch(0).position;
					Vector2 touchPosition = touchPosition_;
					touchPosition.y = Screen.height - touchPosition.y;
					if (touchPosition.x > buttonX && touchPosition.x < buttonX + 100 && 
					    touchPosition.y > buttonY && touchPosition.y < buttonY + 50)
					{
						touchMoveEnable = true;
					}
				}
				else if (Input.GetTouch(0).phase == TouchPhase.Ended)
				{
					touchMoveEnable = false;
				}
			}
		}
	}
	
	void OnGUI()
	{
		if (isShow)
		{
			GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
			
			if (isLock)
			{
				if (recodeOffset * logInfoList_.Count > Screen.height && logCount != logInfoList_.Count)
				{
					//Log("logCount " + logCount + ", logInfoList_.Count " + logInfoList_.Count);
					logCount = logInfoList_.Count;
					scrollPosition.y = recodeOffset * logInfoList_.Count - Screen.height;
				}
			}
			
			scrollPosition = GUI.BeginScrollView(new Rect(0, 0, Screen.width, Screen.height), scrollPosition, new Rect(0, 0, Screen.width, recodeOffset * logInfoList_.Count));
			GUI.skin = skin;

			lock (((ICollection)logInfoList_).SyncRoot)
			{
				for (int i = 0; i < logInfoList_.Count; i++)
				{
					switch( (int)logInfoList_[i].logType )
					{
					case (int)ScreenLogType.Normal:
						GUI.Label(new Rect(0, recodeOffset * i, Screen.width, recodeOffset), logInfoList_[i].logInfo);
						break;

					case (int)ScreenLogType.Warning:
						GUI.Label(new Rect(0, recodeOffset * i, Screen.width, recodeOffset), logInfoList_[i].logInfo, styleWarning);
						break;

					case (int)ScreenLogType.Error:
						GUI.Label(new Rect(0, recodeOffset * i, Screen.width, recodeOffset), logInfoList_[i].logInfo, styleError);
						break;
					}
				}
			}
			GUI.skin = null;
			GUI.EndScrollView();
		}
	}

	public static void Log(string info)
	{
		if(instance_ == null)
		{
			init();
			//return;
		}

		ScreenLogMsg msg = new ScreenLogMsg();
		msg.logInfo = info;
		msg.logType = ScreenLogType.Normal;

		lock (((ICollection)logInfoList_).SyncRoot)
		{
			logInfoList_.Add(msg);
		}

		Debug.Log(info);
	}
	
	public static void LogWarning(string info)
	{
		if(instance_ == null)
		{
			init();
			//return;
		}
		
		ScreenLogMsg msg = new ScreenLogMsg();
		msg.logInfo = info;
		msg.logType = ScreenLogType.Warning;

		lock (((ICollection)logInfoList_).SyncRoot)
		{
			logInfoList_.Add(msg);
		}
		
		Debug.LogWarning(info);
	}

	public static void LogError(string info)
	{
		if(instance_ == null)
		{
			init();
			//return;
		}
		
		ScreenLogMsg msg = new ScreenLogMsg();
		msg.logInfo = info;
		msg.logType = ScreenLogType.Error;
		
		lock (((ICollection)logInfoList_).SyncRoot)
		{
			logInfoList_.Add(msg);
		}
		
		Debug.LogError(info);
	}

	public static void ClearScreenLog()
	{
		if(instance_ == null)
		{
			return;
		}

		lock (((ICollection)logInfoList_).SyncRoot)
		{
			logInfoList_.Clear();
		}
	}
}
