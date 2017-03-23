using UnityEngine;
using System.Collections;

public class TestComPort : MonoBehaviour {
	static int TimeVal;
	static string TestReadMsgA = "";	
	static string TestReadMsgB = "";	
	static string TestReadMsgC = "";	
	static string TestReadMsgD = "";	
	static string TestReadMsgE = "";
	static TestComPort _Instance;
	public static TestComPort GetInstance()
	{
		if (_Instance == null) {
			GameObject obj = new GameObject("_TestComPort");
			DontDestroyOnLoad(obj);
			_Instance = obj.AddComponent<TestComPort>();
		}
		return _Instance;
	}

	void Awake()
	{
		if (_Instance != null) {
			Destroy(gameObject);
			return;
		}
		_Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void OnGUI()
	{
		if (MyCOMDevice.ComThreadClass.ReadByteMsg.Length < (MyCOMDevice.ComThreadClass.BufLenRead - MyCOMDevice.ComThreadClass.BufLenReadEnd)) {
//			Debug.Log("ReadBufLen: "+MyCOMDevice.ComThreadClass.ReadByteMsg.Length);
//			Debug.LogError("ReadMsgError: msg -> "+TestReadMsg);
			return;
		}

		bool isTestLoadingInfo = false;
		float hVal = 25f;
		float wVal = 300f;
		if (isTestLoadingInfo) {
			TestReadMsgA = "IsLoadingLevel: "+XkGameCtrl.IsLoadingLevel
				+", ReadTimeOutCount: "+MyCOMDevice.ComThreadClass.ReadTimeOutCount;

			TimeVal = (int)Time.realtimeSinceStartup;
			TestReadMsgB = MyCOMDevice.ComThreadClass.ComPortName+" -> time: "+TimeVal.ToString("d10");
			
			GUI.Box(new Rect(0f, 0f, wVal, hVal), TestReadMsgA);
			GUI.Box(new Rect(0f, hVal, wVal, hVal), TestReadMsgB);
		}
		else {
			if (MyCOMDevice.ComThreadClass.ReadCount > 0) {
				TestReadMsgA = "Read: ";
				for (int i = 0; i < MyCOMDevice.ComThreadClass.ReadByteMsg.Length; i++) {
					TestReadMsgA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
				}
				GUI.Label(new Rect(0f, 10f, Screen.width, 30f), TestReadMsgA);
			}
			
			TestReadMsgB = "WriteCount: " + MyCOMDevice.ComThreadClass.WriteCount;
			TestReadMsgC = "ReadCount: " + MyCOMDevice.ComThreadClass.ReadCount;
			TestReadMsgD = "IsLoadingLevel: "+XkGameCtrl.IsLoadingLevel
				+", ReadTimeOutCount: "+MyCOMDevice.ComThreadClass.ReadTimeOutCount;
			
			TimeVal = (int)Time.realtimeSinceStartup;
			TestReadMsgE = MyCOMDevice.ComThreadClass.ComPortName + " -> time: "+TimeVal.ToString();
			
			GUI.Box(new Rect(0f, 70f, Screen.width, 30f), TestReadMsgB);
			GUI.Box(new Rect(0f, 100f, Screen.width, 30f), TestReadMsgC);
			GUI.Box(new Rect(0f, 130f, Screen.width, 30f), TestReadMsgD);
			GUI.Box(new Rect(0f, 160f, Screen.width, 30f), TestReadMsgE);
		}
	}
}