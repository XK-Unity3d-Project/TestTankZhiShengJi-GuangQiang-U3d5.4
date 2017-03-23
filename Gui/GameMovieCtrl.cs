using UnityEngine;
using System.Collections;

public class GameMovieCtrl : MonoBehaviour {
	public MovieTexture Movie;
	public GameObject MovieBJObj;
	public Rect[] RectArray;
	public Rect RectMv;
	public Texture[] TextureMv;
	public Texture[] TextureMv_Ch; //中文版UI.
	public Texture[] TextureMv_En; //英文版UI.
	/**
	 * TextureMvEnd[0] ->“坦克大战”.
	 * TextureMvEnd[1] ->“直升机大战”.
	 * TextureMvEnd[2] ->“联合战斗”.
	 */
	public Texture[] TextureMvEnd;
	public Texture TextureMvLG;
	public static bool IsTestLJGame; //测试联机小窗口游戏.
	AudioSource AudioSourceObj;
	bool IsStopMovie;
	/**
	 * 控制游戏为3屏或1屏输出.
	 * 1.单机和联机坦克为1屏输出.
	 * 2.直升机机台为3屏输出.
	 */
	public static bool IsThreeScreenGame = false;
	public static bool IsActivePlayer;
	float TimeVal;
	public static bool IsTestXiaoScreen = false;
	enum QualityLevelEnum
	{
		Fastest,
		Fast,
		Simple,
		Good,
		Beautiful,
		Fantastic
	}
	static GameMovieCtrl _instance;
	public static GameMovieCtrl GetInstance()
	{
		return _instance;
	}

	//public static string TestGameFile = "TestGame.info";
	// Use this for initialization
	void Start()
	{
		XkGameCtrl.IsLoadingLevel = false;
		string threeScreen = HandleJson.GetInstance().ReadFromFilePathXml(GameTypeCtrl.TestGameFile, "threeScreen");
		if (threeScreen == "") {
			threeScreen = "0";
			HandleJson.GetInstance().WriteToFilePathXml(GameTypeCtrl.TestGameFile, "threeScreen", threeScreen);
		}
		IsTestThreeScreen = threeScreen == "0" ? false : true;

		switch (GameTypeCtrl.AppTypeStatic) {
		case AppGameType.DanJiFeiJi:
		case AppGameType.LianJiFeiJi:
			IsThreeScreenGame = true;
			if (!IsTestThreeScreen) {
				IsThreeScreenGame = false;
			}
			break;
		default:
			IsThreeScreenGame = false;
			break;
		}

		if (XKGlobalData.GetInstance() != null) {
			if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiServer) {
				AudioListener.volume = 0f;
			}
			else {
				AudioListener.volume = (float)XKGlobalData.GameAudioVolume / 10f;
			}
		}

		_instance = this;
		XkGameCtrl.ResetIsLoadingLevel();
		if (pcvr.GetInstance() != null) {
			pcvr.GetInstance().CloseFangXiangPanPower(PlayerEnum.Null);
		}
		pcvr.CloseAllQiNangArray(1);
		pcvr.CloseGunZhenDongDengJi();
		PlayerIPInfo = Network.player.ipAddress;
		TimeLast = Time.realtimeSinceStartup;
		GameTextType gameTextVal = XKGlobalData.GetGameTextMode();
		//gameTextVal = GameTextType.English; //test.
		switch (gameTextVal) {
		case GameTextType.Chinese:
			TextureMv = TextureMv_Ch;
			break;
			
		case GameTextType.English:
			TextureMv = TextureMv_En;
			break;
		}

		if (AudioListCtrl.GetInstance() != null) {
			AudioListCtrl.GetInstance().CloseGameAudioBJ();
		}
		Screen.showCursor = false;
		LoadingGameCtrl.ResetLoadingInfo();
		Time.timeScale = 1.0f;
		RectMv.width = Screen.width;
		RectMv.height = Screen.height * 0.93f;
		float perY = 620f / 768f;
		float perXA = 180f / 1360f;
		float perXB = 955f / 1360f;
		float perXC = 100f / 1360f;
		float perXD = 875f / 1360f;
		RectArray[0].y = Screen.height * perY;
		RectArray[1].y = Screen.height * perY;
		RectArray[2].y = Screen.height * perY;
		RectArray[3].y = Screen.height * perY;

		RectArray[0].x = Screen.width * perXA;
		RectArray[1].x = Screen.width * perXB;
		RectArray[2].x = Screen.width * perXC;
		RectArray[3].x = Screen.width * perXD;
		AudioManager.Instance.SetParentTran(null);
		GameOverCtrl.IsShowGameOver = false;
		//IsTestLJGame = true; //test
		//IsTestXiaoScreen = true; //test
		if (!XkGameCtrl.IsGameOnQuit) {
			if (!IsThreeScreenGame) {
				if (!Screen.fullScreen
					|| Screen.currentResolution.width != (int)XkGameCtrl.ScreenWidth
					|| Screen.currentResolution.height != (int)XkGameCtrl.ScreenHeight) {
					if (!IsTestLJGame && !IsTestXiaoScreen) {
						if (XkGameCtrl.ScreenWidth != 1360f) {
							Screen.SetResolution(1360, 768, true);
						}
						else {
							Screen.SetResolution((int)XkGameCtrl.ScreenWidth,
							                     (int)XkGameCtrl.ScreenHeight,
							                     true);
						}
					}
				}
			}
			else {
				if (!Screen.fullScreen
					|| Screen.currentResolution.width != (int)XkGameCtrl.ScreenWidth3
				    || Screen.currentResolution.height != (int)XkGameCtrl.ScreenHeight3) {
					if (!IsTestLJGame && !IsTestXiaoScreen) {
						Screen.SetResolution((int)XkGameCtrl.ScreenWidth3,
						                     (int)XkGameCtrl.ScreenHeight3,
						                     true);
					}
				}
			}
		}

		if (!IsTestLJGame) {
			IsActivePlayer = true;
			if (IsTestXiaoScreen) {
				Screen.SetResolution(680, 384, false); //test
			}
		}

		QualitySettings.SetQualityLevel((int)QualityLevelEnum.Fast);
		AudioSourceObj = transform.GetComponent<AudioSource>();
		Invoke("DelayResetIsLoadingLevel", 4f);
		PlayMovie();
	}

	void CheckClientPortMovieInfo()
	{
		if (Time.frameCount % 500 != 0) {
			return;
		}

		if (RectMv.width == Screen.width && RectMv.height == (Screen.height * 0.93f)) {
			return;
		}

		RectMv.width = Screen.width;
		RectMv.height = Screen.height * 0.93f;
		if (!IsThreeScreenGame) {
			float perY = 620f / 768f;
			float perXA = 180f / 1360f;
			float perXB = 955f / 1360f;
			float perXC = 100f / 1360f;
			float perXD = 875f / 1360f;
			RectArray[0].y = Screen.height * perY;
			RectArray[1].y = Screen.height * perY;
			RectArray[2].y = Screen.height * perY;
			RectArray[3].y = Screen.height * perY;
			
			RectArray[0].x = Screen.width * perXA;
			RectArray[1].x = Screen.width * perXB;
			RectArray[2].x = Screen.width * perXC;
			RectArray[3].x = Screen.width * perXD;
		}
	}

	void DelayResetIsLoadingLevel()
	{
		XkGameCtrl.ResetIsLoadingLevel();
		if (NetworkServerNet.GetInstance() != null) {
			NetworkServerNet.GetInstance().TryToCreateServer();
		}
	}
	
	void PlayMovie()
	{
		if (renderer != null) {
			renderer.enabled = true;
			renderer.material.mainTexture = Movie;
		}
		Movie.loop = false;
		Movie.Play();
		TimeStartMV = Time.realtimeSinceStartup;
		
		if (AudioSourceObj != null) {
			AudioSourceObj.clip = Movie.audioClip;
			AudioSourceObj.enabled = true;
			AudioSourceObj.Play();
		}
	}

	public void StopPlayMovie()
	{
		if (IsStopMovie) {
			return;
		}
		IsStopMovie = true;
		Movie.Stop();
		if (AudioSourceObj != null) {
			AudioSourceObj.Stop();
			AudioSourceObj.enabled = false;
		}

		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}
		gameObject.SetActive(false);
		//MovieBJObj.SetActive(false);
	}

	void ShowGameMvLG()
	{
		if (TimeMovieVal < 80f || IsPlayMvLogo) {
			return;
		}

		float swTmp = (float)Screen.width / 3f;
		if (!IsThreeScreenGame) {
			GUI.DrawTexture(new Rect(0f,0f, Screen.width, Screen.height), TextureMvLG);
		}
		else {
			if (Screen.width > 1360) {
				GUI.DrawTexture(new Rect(0f, 0f, swTmp, Screen.height), TextureMvLG);
				GUI.DrawTexture(new Rect(swTmp, 0f, swTmp, Screen.height), TextureMvLG);
				GUI.DrawTexture(new Rect(swTmp * 2f, 0f, swTmp, Screen.height), TextureMvLG);
			}
			else {
				GUI.DrawTexture(new Rect(0f,0f, Screen.width, Screen.height), TextureMvLG);
			}
		}
	}

	float TimeMovieVal;
	float TimeLast;
	public static string PlayerIPInfo = "192.168.0.2";
	bool IsFixServerPortIP;
	bool IsRestartGame;
	float TimeStartMV;
	int CountMV;
	bool IsPlayMvLogo;
	float TimeLogo;
	void ShowGameMovieLogo()
	{
		int indexMVLG = 0;
		float swTmp = (float)Screen.width / 3f;
		AppGameType gameType = GameTypeCtrl.AppTypeStatic;
		switch (gameType) {
		case AppGameType.LianJiFeiJi:
		case AppGameType.LianJiTanKe:
		case AppGameType.LianJiServer:
			indexMVLG = 2;
			break;
		case AppGameType.DanJiFeiJi:
			indexMVLG = 1;
			break;
		case AppGameType.DanJiTanKe:
			indexMVLG = 0;
			break;
		}
		if (!IsThreeScreenGame) {
			GUI.DrawTexture(new Rect(0f,0f, Screen.width, Screen.height), TextureMvEnd[indexMVLG]);
		}
		else {
			if (Screen.width > 1360) {
				GUI.DrawTexture(new Rect(0f, 0f, swTmp, Screen.height), TextureMvEnd[indexMVLG]);
				GUI.DrawTexture(new Rect(swTmp, 0f, swTmp, Screen.height), TextureMvEnd[indexMVLG]);
				GUI.DrawTexture(new Rect(swTmp * 2f, 0f, swTmp, Screen.height), TextureMvEnd[indexMVLG]);
			}
			else {
				GUI.DrawTexture(new Rect(0f,0f, Screen.width, Screen.height), TextureMvEnd[indexMVLG]);
			}
		}
	}

	float TimeDelayHiddenMvLogo = -10f;
	void OnGUI()
	{
		if (IsPlayMvLogo) {
			ShowGameMovieLogo();
			if (Time.realtimeSinceStartup - TimeLogo >= 3f) {
				IsPlayMvLogo = false;
				TimeStartMV = Time.realtimeSinceStartup;
				TimeDelayHiddenMvLogo = Time.realtimeSinceStartup;
				Movie.Play();
				AudioSourceObj.Play();
				return;
			}
			return;
		}

		if (Time.realtimeSinceStartup - TimeDelayHiddenMvLogo < 0.2f) {
			ShowGameMovieLogo();
			return;
		}

		if (renderer != null) {
			return;
		}
		//GUI.Box(new Rect(0f, 0f, 200f, 30f), "dtime "+Time.deltaTime.ToString("f3"));

		if (Network.peerType == NetworkPeerType.Server
		    || GameTypeCtrl.AppTypeStatic == AppGameType.LianJiServer) {
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Movie, ScaleMode.StretchToFill);
			
			TimeMovieVal = Time.realtimeSinceStartup - TimeStartMV;
			if (TimeMovieVal >= Movie.duration + 3f) {
				TimeLogo = Time.realtimeSinceStartup;
				IsPlayMvLogo = true;
				Movie.Stop();
				AudioSourceObj.Stop();
				
				TimeStartMV = Time.realtimeSinceStartup;
				CountMV++;
			}
			
			ShowGameMvLG();

			if (!pcvr.bIsHardWare) {
				string mvInfo = "mvTime "+Movie.duration+", CountMV "+CountMV+", timeVal "+TimeMovieVal.ToString("f2");
				GUI.Box(new Rect(0f, 0f, Screen.width * 0.5f, 30f), mvInfo);
				return;
			}

			if (Time.frameCount % 200 == 0) {
				PlayerIPInfo = Network.player.ipAddress;
				/*if (Network.player != null) {
					PlayerIPInfo = Network.player.ipAddress;
				}*/
			}

			if (Time.realtimeSinceStartup - TimeLast < 20f) {
				if (PlayerIPInfo == NetworkServerNet.ServerPortIP) {
					return;
				}
				IsFixServerPortIP = true;
				string infoA = "The pc IP is "+PlayerIPInfo+", ServerPortIP was wrong! Fixing IP...";
				GUI.Box(new Rect(0, 0, Screen.width, Screen.height), infoA);
				return;
			}
			
			if (PlayerIPInfo == NetworkServerNet.ServerPortIP) {
				if (IsFixServerPortIP) {
					string infoA = "ServerPortIP is fixed, restart the game...";
					GUI.Box(new Rect(0, 0, Screen.width, Screen.height), infoA);
					if (!IsRestartGame) {
						IsRestartGame = true;
						//XKCheckGameServerIP.CloseCmd();
						XKCheckGameServerIP.RestartGame();
					}
				}
				return;
			}

			string infoB = "Set ServerPortIp(192.168.0.2) is failed! The ip(192.168.0.2) has been used!";
			GUI.Box(new Rect(0, 0, Screen.width, 30), infoB);
			string infoC = "Please restart the pc after change the pcIP(192.168.0.2) to otherIP!";
			GUI.Box(new Rect(0, 30, Screen.width, 30), infoC);
			return;
		}

		if (IsStopMovie) {
			return;
		}
		CheckClientPortMovieInfo();
		
		if (!IsThreeScreenGame) {
			GUI.DrawTexture(RectMv, Movie, ScaleMode.StretchToFill);
		}
		else {
			if (Screen.width > 1360) {
				float swTmp = (float)Screen.width / 3f;
				GUI.DrawTexture(new Rect(0f, 0f, swTmp, RectMv.height),
				                Movie, ScaleMode.StretchToFill);
				GUI.DrawTexture(new Rect(swTmp, 0f, swTmp, RectMv.height),
				                Movie, ScaleMode.StretchToFill);
				GUI.DrawTexture(new Rect(swTmp * 2f, 0f, swTmp, RectMv.height),
				                Movie, ScaleMode.StretchToFill);
			}
			else {
				GUI.DrawTexture(RectMv, Movie, ScaleMode.StretchToFill);
			}
			
//			string testStr = "sw "+Screen.width+", sw/3 "+swTmp;
//			GUI.Box(new Rect(0f, 0f, 500f, 25f), testStr);
		}

		TimeVal += Time.deltaTime;
		int timeTmp = (int)TimeVal;
		if (!XKGlobalData.IsFreeMode) {
			if (timeTmp % 2 == 0) {
				if (XKGlobalData.CoinPlayerOne < XKGlobalData.GameNeedCoin) {
					GUI.DrawTexture(RectArray[0], TextureMv[0]);
				}
				else {
					GUI.DrawTexture(RectArray[2], TextureMv[1]);
				}
				
				if (XKGlobalData.CoinPlayerTwo < XKGlobalData.GameNeedCoin) {
					GUI.DrawTexture(RectArray[1], TextureMv[0]);
				}
				else {
					GUI.DrawTexture(RectArray[3], TextureMv[1]);
				}
			}
			else {
				if (XKGlobalData.CoinPlayerOne >= XKGlobalData.GameNeedCoin) {
					GUI.DrawTexture(RectArray[2], TextureMv[2]);
				}
				
				if (XKGlobalData.CoinPlayerTwo >= XKGlobalData.GameNeedCoin) {
					GUI.DrawTexture(RectArray[3], TextureMv[2]);
				}
			}
		}
		else {
			if (timeTmp % 2 == 0) {
				GUI.DrawTexture(RectArray[2], TextureMv[1]);
				GUI.DrawTexture(RectArray[3], TextureMv[1]);
			}
			else {
				GUI.DrawTexture(RectArray[2], TextureMv[2]);
				GUI.DrawTexture(RectArray[3], TextureMv[2]);
			}
		}
		
		if (Camera.main != null && IsThreeScreenGame) {
			Vector3 posTmp = Camera.main.WorldToScreenPoint(InsertCoinTr[0].position);
			float testPX = posTmp.x - (RectArray[0].width / 2f);
			float testPY = posTmp.y + (RectArray[0].height * 0.7f);
			testPY = Screen.height - testPY;
			RectArray[0].x = testPX;
			RectArray[0].y = testPY;

			posTmp = Camera.main.WorldToScreenPoint(InsertCoinTr[1].position);
			testPX = posTmp.x - (RectArray[1].width / 2f);
			RectArray[1].x = testPX;
			RectArray[1].y = testPY;

			posTmp = Camera.main.WorldToScreenPoint(StartBtTr[0].position);
			testPX = posTmp.x - (RectArray[2].width / 2f);
			testPY = posTmp.y + (RectArray[2].height * 0.7f);
			testPY = Screen.height - testPY;
			RectArray[2].x = testPX;
			RectArray[2].y = testPY;
			
			posTmp = Camera.main.WorldToScreenPoint(StartBtTr[1].position);
			testPX = posTmp.x - (RectArray[3].width / 2f);
			RectArray[3].x = testPX;
			RectArray[3].y = testPY;
		}

		TimeMovieVal = Time.realtimeSinceStartup - TimeStartMV;
		if (TimeMovieVal >= Movie.duration + 3f) {
			TimeLogo = Time.realtimeSinceStartup;
			IsPlayMvLogo = true;
			Movie.Stop();
			AudioSourceObj.Stop();

			TimeStartMV = Time.realtimeSinceStartup;
			CountMV++;
		}
		
		ShowGameMvLG();
		if (!pcvr.bIsHardWare) {
			string mvInfo = "mvTime "+Movie.duration+", CountMV "+CountMV+", timeVal "+TimeMovieVal.ToString("f2");
			GUI.Box(new Rect(0f, 0f, Screen.width * 0.5f, 30f), mvInfo);
		}
		XKGameFPSCtrl.DrawGameFPS();
	}
	public Transform[] InsertCoinTr;
	public Transform[] StartBtTr;
	public static bool IsTestThreeScreen = true;
	void Update()
	{
		if (!IsThreeScreenGame) {
			return;
		}

		if (Input.GetKeyUp(KeyCode.P) && IsTestThreeScreen) {
			TestGameThreeScreen();
		}
	}

	public static void TestGameThreeScreen()
	{
		Screen.SetResolution(1440, 271, false);
	}
}