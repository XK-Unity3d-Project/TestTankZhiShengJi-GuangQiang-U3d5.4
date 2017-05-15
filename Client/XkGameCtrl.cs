using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NpcJiFenEnum
{
	Null = -1,
	ShiBing,
	CheLiang,
	ChuanBo,
	FeiJi,
}

public enum GameMode
{
	Null,
	LianJi,
	DanJiFeiJi,
	DanJiTanKe,
}

public enum GameJiTaiType
{
	Null,
	FeiJiJiTai,
	TanKeJiTai,
}

public class XkGameCtrl : MonoBehaviour {
	public GameObject GameWaterObj;
	public PcvrComState GameTKPMState = PcvrComState.TanKeGunZhenDong;
	/**
	 * YouLiangDianMin -> 飞机主角单人油量点.
	 */
	[Range(1, 1000)] public int YouLiangDianMin = 50;
	/**
	 * YouLiangDianMinSR -> 飞机主角双人油量点.
	 */
	[Range(1, 1000)] public int YouLiangDianMinSR = 50;
	/**
	 * YouLiangDianMinLJ -> 飞机主角联机单人油量点.
	 */
	[Range(1, 1000)] public int YouLiangDianMinLJ = 50;
	/**
	 * YouLiangDianMinLJSR -> 飞机主角联机双人油量点.
	 */
	[Range(1, 1000)] public int YouLiangDianMinLJSR = 50;
	/**
	 * YouLiangDianMin -> 坦克主角单人油量点.
	 */
	[Range(1, 1000)] public int TKYouLiangDianMin = 50;
	/**
	 * YouLiangDianMinSR -> 坦克主角双人油量点.
	 */
	[Range(1, 1000)] public int SRTKYouLiangDianMin = 50;
	/**
	 * YouLiangDianMinLJ -> 坦克主角联机单人油量点.
	 */
	[Range(1, 1000)] public int LJTKYouLiangDianMin = 50;
	/**
	 * YouLiangDianMinLJSR -> 坦克主角联机双人油量点.
	 */
	[Range(1, 1000)] public int LJSRTKYouLiangDianMin = 50;
	int YouLiangDianVal;
	int YouLiangDianValP1;
	int YouLiangDianValP2;
	[Range(1, 99)] public int DaoDanNum = 50;
	public GameObject FeiJiPlayer;
	public AiMark FeiJiPlayerMark;
	Transform FeiJiPlayerTran;
	int FeiJiMarkIndex = 1;
	AiPathCtrl FeiJiPlayerPath;
	public GameObject TanKePlayer;
	public AiMark TanKePlayerMark;
	Transform TanKePlayerTran;
	int TanKeMarkIndex = 1;
	AiPathCtrl TanKePlayerPath;
	public GameObject CartoonCamPlayer;
	public AiMark CartoonCamPlayerMark;
	public GameObject ServerCamera; //服务器飞机摄像机.
	public static GameObject ServerCameraObj;
	public GameObject ServerCameraTK; //服务器坦克摄像机.
	public static GameObject ServerCameraObjTK;
	Transform CartoonCamPlayerTran;
	int CartoonCamMarkIndex = 1;
	AiPathCtrl CartoonCamPlayerPath;
	public LayerMask PlayerLayer;
	public LayerMask LandLayer;
	public LayerMask NpcAmmoHitLayer;
	public LayerMask PlayerAmmoHitLayer;
	public static Transform MissionCleanup;
	public static Transform NpcAmmoArray;
	public static Transform PlayerAmmoArray;
	List<Transform> NpcTranList = new List<Transform>(20);
	static List<YouLiangDianMoveCtrl> YLDLvA = new List<YouLiangDianMoveCtrl>(20);
	static List<YouLiangDianMoveCtrl> YLDLvB = new List<YouLiangDianMoveCtrl>(20);
	static List<YouLiangDianMoveCtrl> YLDLvC = new List<YouLiangDianMoveCtrl>(20);
	public static float ScreenWidth = 1360f;
	public static float ScreenHeight = 768f;
	public static float ScreenWidth3 = 4080f;
	public static float ScreenHeight3 = 768f;
	public static string TerrainLayer = "Terrain";
	public int[] ShiBingXunZhangJB = {10, 20};
	public int[] TanKeXunZhangJB = {10, 20};
	public int[] ChuanBoXunZhangJB = {10, 20};
	public int[] FeiJiXunZhangJB = {10, 20};
	public int[] XunZhangZP = {100, 200, 300};
	[Range(0f, 100f)]public float MinRandTimeServer = 5f;
	[Range(0f, 100f)]public float MaxRandTimeServer = 15f;
	public GameObject GameFpsPrefab;
	public GameObject AudioListPrefab;
	public GameMode TestGameModeVal = GameMode.Null;
	public static GameMode GameModeVal = GameMode.DanJiFeiJi;
	public static GameJiTaiType GameJiTaiSt = GameJiTaiType.FeiJiJiTai;
	public RenderTexture TestCameraRender;
	public bool IsCartoonShootTest;
	public bool IsActiveAllPlayerTest = true;
	public bool IsServerCameraTest;
	public static string TagNull = "Untagged";
	public static string TagMainCamera = "MainCamera";
	public static int ShiBingNumPOne;
	public static int CheLiangNumPOne;
	public static int ChuanBoNumPOne;
	public static int FeiJiNumPOne;
	public static int ShiBingNumPTwo;
	public static int CheLiangNumPTwo;
	public static int ChuanBoNumPTwo;
	public static int FeiJiNumPTwo;
	int AllPlayerKillNpc;
	const int GaoBaoDanBuJiNum = 99;
	int DaoDanBuJiNum = 3;
	public static int DaoDanNumPOne;
	public static int DaoDanNumPTwo;
	public static int GaoBaoDanNumPOne;
	public static int GaoBaoDanNumPTwo;
	public static bool IsMoveOnPlayerDeath = true;
	public static float YouLiangBuJiNum = 30f;
	public static float PlayerYouLiangMax = 60f;
	public static float PlayerYouLiangCur = 60f;
	public static float PlayerYouLiangCurP1 = 60f;
	public static float PlayerYouLiangCurP2 = 60f;
	public static bool IsActivePlayerOne;
	public static bool IsActivePlayerTwo;
	public static bool IsActivePlayerThree;
	public static bool IsActivePlayerFour;
	/// <summary>
	/// set IsLoadingLevel is true when loading level, otherwise set IsLoadingLevel is false.
	/// </summary>
	public static bool IsLoadingLevel;
//	public static Transform PlayerTranCurrent;
	float TimeCheckNpcAmmo;
	float TimeCheckNpcTranList;
	public static float TriggerBoxSize_Z = 1.5f;
	static List<XKTriggerRemoveNpc> CartoonTriggerSpawnList;
	public static int CountNpcAmmo;
	static List<GameObject> NpcAmmoList;
	public static bool IsDonotCheckError = false;
	public static bool IsShowDebugInfoBox = false;
	static bool IsActiveFinishTask;
	public static bool IsPlayGamePOne;
	public static bool IsPlayGamePTwo;
	public static int YouLiangDianAddPOne;
	public static int YouLiangDianAddPTwo;
	public static GameObject ServerCameraPar;
	public static int YouLiangJingGaoVal = 10;
	public static bool IsAddPlayerYouLiang;
	public static bool IsGameOnQuit;
	public static bool IsDrawGizmosObj = true;
	public static int AmmoNumMaxNpc = 30;
	/**
	 * SelectYouLiangUI == 1 -> 两个玩家使用1个油量.
	 * SelectYouLiangUI == 2 -> 两个玩家使用2个油量.
	 */
	public static int SelectYouLiangUI = 2;
	public static int TestGameEndLv = (int)GameLevel.Scene_2;
	static XkGameCtrl _Instance;
	public static XkGameCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake()
	{
		_Instance = this;
		if (pcvr.GetInstance() != null) {
			pcvr.GetInstance().OpenFangXiangPanPower(PlayerEnum.Null);
		}
		pcvr.CloseAllQiNangArray();
		pcvr.OpenPlayerGunZhenDong();
		XKSpawnNpcPoint.ClearFiJiNpcPointList();
		XKPlayerHeTiData.IsActiveHeTiPlayer = false;
		XKTriggerClosePlayerUI.IsActiveHeTiCloseUI = false;
		XKTriggerClosePlayerUI.IsClosePlayerUI = false;
		XKTriggerKaQiuShaFire.IsFireKaQiuSha = false;
		XKTriggerOpenPlayerUI.IsActiveOpenPlayerUI = false;
		XKGlobalData.GetInstance().StopModeBeiJingAudio();
		SetActiveGameWaterObj(false);
		int indexGuanKa = Application.loadedLevel - 1;
		switch (XKGlobalData.GameDiff) {
		case "0":
			GameDiffVal = GameDiffDiArray[indexGuanKa];
			break;
		case "1":
			GameDiffVal = GameDiffZhongArray[indexGuanKa];
			break;
		case "2":
			GameDiffVal = GameDiffGaoArray[indexGuanKa];
			break;
		}

		/*if (!pcvr.bIsHardWare && !GameMovieCtrl.IsActivePlayer) {
			GameDiffVal = GameDiffTest;
		}*/
		CountNpcAmmo = 0;
		ShiBingNumPOne = 0;
		CheLiangNumPOne = 0;
		ChuanBoNumPOne = 0;
		FeiJiNumPOne = 0;
		ShiBingNumPTwo = 0;
		CheLiangNumPTwo = 0;
		ChuanBoNumPTwo = 0;
		GaoBaoDanNumPOne = 0;
		GaoBaoDanNumPTwo = 0;
		FeiJiNumPTwo = 0;
		YouLiangDianAddPOne = 0;
		YouLiangDianAddPTwo = 0;
		IsActiveFinishTask = false;
		IsAddPlayerYouLiang = false;
		ScreenDanHeiCtrl.IsStartGame = false;

		if (GameFpsPrefab != null) {
			Instantiate(GameFpsPrefab);
		}

		PlayerAmmoCtrl.PlayerAmmoHitLayer = PlayerAmmoHitLayer;
		NpcAmmoList = new List<GameObject>();
		CartoonTriggerSpawnList = new List<XKTriggerRemoveNpc>();
		if (Network.peerType == NetworkPeerType.Disconnected) {
			if (!GameMovieCtrl.IsActivePlayer) {
				if (IsServerCameraTest) {
					TestGameModeVal = GameMode.LianJi;
				}
				GameModeVal = TestGameModeVal != GameMode.Null ? TestGameModeVal : GameModeVal; //TestGame
			}
			else {
				if (GameTypeCtrl.AppTypeStatic == AppGameType.DanJiFeiJi
				    || GameTypeCtrl.AppTypeStatic == AppGameType.LianJiFeiJi) {
					GameModeVal = GameMode.DanJiFeiJi;
				}
				else if (GameTypeCtrl.AppTypeStatic == AppGameType.DanJiTanKe
				         || GameTypeCtrl.AppTypeStatic == AppGameType.LianJiTanKe) {
					GameModeVal = GameMode.DanJiTanKe;
				}
			}
			GameModeCtrl.IsSelectDanJiMode = true;
		}
		else {
			GameModeVal = GameMode.LianJi;
			if (Network.peerType == NetworkPeerType.Server) {
				GameJiTaiSt = GameJiTaiType.Null;
			}
			else if (Network.peerType == NetworkPeerType.Client) {
				if (GameTypeCtrl.AppTypeStatic == AppGameType.DanJiFeiJi
				    || GameTypeCtrl.AppTypeStatic == AppGameType.LianJiFeiJi) {
					GameJiTaiSt = GameJiTaiType.FeiJiJiTai;
				}
				else if (GameTypeCtrl.AppTypeStatic == AppGameType.DanJiTanKe
				         || GameTypeCtrl.AppTypeStatic == AppGameType.LianJiTanKe) {
					GameJiTaiSt = GameJiTaiType.TanKeJiTai;
				}
			}
			GameModeCtrl.IsSelectDanJiMode = false;
		}

		if (GameMovieCtrl.IsActivePlayer) {
			IsCartoonShootTest = false;
			IsServerCameraTest = false;
			Screen.showCursor = false;
			GameTKPMState = GameTypeCtrl.GameTKPMStatic;
		}

		if (IsServerCameraTest) {
			IsCartoonShootTest = false;
		}

		if (IsCartoonShootTest) {
			Screen.SetResolution(1360, 768, true);
		}
		else if (!XkGameCtrl.IsGameOnQuit) {
			if (!GameMovieCtrl.IsThreeScreenGame) {
				if (!Screen.fullScreen
					|| Screen.currentResolution.width != (int)ScreenWidth
				    || Screen.currentResolution.height != (int)ScreenHeight) {
					if (GameMovieCtrl.IsActivePlayer
					    && !GameMovieCtrl.IsTestXiaoScreen
					    && !GameMovieCtrl.IsTestThreeScreen) {
						Screen.SetResolution((int)XkGameCtrl.ScreenWidth,
						                     (int)XkGameCtrl.ScreenHeight,
						                     true);
					}
				}
			}
			else {
				if (!Screen.fullScreen
					|| Screen.currentResolution.width != (int)XkGameCtrl.ScreenWidth3
				    || Screen.currentResolution.height != (int)XkGameCtrl.ScreenHeight3) {
					if (GameMovieCtrl.IsActivePlayer
					    && !GameMovieCtrl.IsTestXiaoScreen
					    && !GameMovieCtrl.IsTestThreeScreen) {
						Screen.SetResolution((int)XkGameCtrl.ScreenWidth3,
						                     (int)XkGameCtrl.ScreenHeight3,
						                     true);
					}
				}
			}
		}

		NpcAmmoCtrl.NpcAmmoHitLayer = NpcAmmoHitLayer;
		GameObject obj = null;
		XkPlayerCtrl playerScript = null;
		Transform pathTran = null;
		if (FeiJiPlayerMark != null) {
			FeiJiPlayerTran = FeiJiPlayerMark.transform;
			FeiJiPlayerPath = FeiJiPlayerTran.parent.GetComponent<AiPathCtrl>();
			pathTran = FeiJiPlayerPath.transform;

			for (int i = 0; i < pathTran.childCount; i++) {
				if (FeiJiPlayerTran == pathTran.GetChild(i)) {
					FeiJiMarkIndex = i + 1;
					break;
				}
			}
		}
		else {
			Debug.LogWarning("FeiJiPlayerMark was wrong!");
			obj.name = "null";
			return;
		}

		if (TanKePlayerMark != null) {
			TanKePlayerTran = TanKePlayerMark.transform;
			TanKePlayerPath = TanKePlayerTran.parent.GetComponent<AiPathCtrl>();
			pathTran = TanKePlayerPath.transform;
			
			for (int i = 0; i < pathTran.childCount; i++) {
				if (TanKePlayerTran == pathTran.GetChild(i)) {
					TanKeMarkIndex = i + 1;
					break;
				}
			}
		}
		else {
			Debug.LogWarning("TanKePlayerMark was wrong!");
			obj.name = "null";
			return;
		}
		
		if (CartoonCamPlayerMark != null) {
			CartoonCamPlayerTran = CartoonCamPlayerMark.transform;
			CartoonCamPlayerPath = CartoonCamPlayerTran.parent.GetComponent<AiPathCtrl>();
			pathTran = CartoonCamPlayerPath.transform;
			
			for (int i = 0; i < pathTran.childCount; i++) {
				if (CartoonCamPlayerTran == pathTran.GetChild(i)) {
					CartoonCamMarkIndex = i + 1;
					break;
				}
			}
		}
		else {
			Debug.LogWarning("CartoonCamPlayerMark was wrong!");
			obj.name = "null";
			return;
		}

		Vector3 posPlayerFJ = new Vector3(0f, -1700f, 0f);
		Vector3 posPlayerTK = new Vector3(0f, -1500f, 0f);
		switch (GameModeVal) {
		case GameMode.DanJiFeiJi:
			GameJiTaiSt = GameJiTaiType.FeiJiJiTai; //test
			obj = (GameObject)Instantiate(FeiJiPlayer, posPlayerFJ, FeiJiPlayerTran.rotation);
			playerScript = obj.GetComponent<XkPlayerCtrl>();
			playerScript.SetAiPathScript(FeiJiPlayerPath);
//			PlayerTranCurrent = obj.transform;
			break;

		case GameMode.DanJiTanKe:
			GameJiTaiSt = GameJiTaiType.TanKeJiTai; //test
			obj = (GameObject)Instantiate(TanKePlayer, posPlayerTK, TanKePlayerTran.rotation);
			playerScript = obj.GetComponent<XkPlayerCtrl>();
			playerScript.SetAiPathScript(TanKePlayerPath);
//			PlayerTranCurrent = obj.transform;
			break;

		case GameMode.LianJi:
			//Debug.Log("peerType "+Network.peerType);
			if (Network.peerType == NetworkPeerType.Disconnected) {
				obj = (GameObject)Instantiate(FeiJiPlayer, posPlayerFJ, FeiJiPlayerTran.rotation);
				playerScript = obj.GetComponent<XkPlayerCtrl>();
				playerScript.SetAiPathScript(FeiJiPlayerPath);

				obj = (GameObject)Instantiate(TanKePlayer, posPlayerTK, TanKePlayerTran.rotation);
				playerScript = obj.GetComponent<XkPlayerCtrl>();
				playerScript.SetAiPathScript(TanKePlayerPath);
			}
			else {
				if (Network.peerType == NetworkPeerType.Client) {
					Invoke("DelaySpawnClientPlayer", 6f);
				}
				else {
					AmmoNumMaxNpc = 25;
				}
			}
			break;
		}

		//CartoonCamPlayer
		if (GameModeVal != GameMode.LianJi || Network.peerType == NetworkPeerType.Disconnected) {
			obj = (GameObject)Instantiate(CartoonCamPlayer, CartoonCamPlayerTran.position, CartoonCamPlayerTran.rotation);
			playerScript = obj.GetComponent<XkPlayerCtrl>();
			playerScript.SetAiPathScript(CartoonCamPlayerPath);
		}
		else {
			if (Network.peerType == NetworkPeerType.Server) {
				NetworkServerNet.GetInstance().SpawnNetPlayerObj(CartoonCamPlayer,
				                                                      CartoonCamPlayerPath,
				                                                      CartoonCamPlayerTran.position,
				                                                      CartoonCamPlayerTran.rotation);
			}
		}

		GameObject objMiss = new GameObject();
		objMiss.name = "MissionCleanup";
		objMiss.transform.parent = transform;
		MissionCleanup = objMiss.transform;

		objMiss = new GameObject();
		objMiss.name = "NpcAmmoArray";
		objMiss.transform.parent = MissionCleanup;
		NpcAmmoArray = objMiss.transform;

		objMiss = new GameObject();
		objMiss.name = "PlayerAmmoArray";
		objMiss.transform.parent = MissionCleanup;
		PlayerAmmoArray = objMiss.transform;
		XKNpcSpawnListCtrl.GetInstance();

		DaoDanNum = DaoDanNum > 99 ? 99 : DaoDanNum;
		DaoDanNumPOne = DaoDanNum;
		DaoDanNumPTwo = DaoDanNum;
		PlayerYouLiangCur = 0f;
		PlayerYouLiangCurP1 = 0f;
		PlayerYouLiangCurP2 = 0f;
		if (Network.peerType != NetworkPeerType.Server) {
			StartCoroutine(SubPlayerYouLiang());
		}
		Invoke("DelayResetIsLoadingLevel", 2f);
		Invoke("TestInitCameraRender", 0.5f);

		if (Network.peerType == NetworkPeerType.Server || IsServerCameraTest) {
			ServerCameraObj = (GameObject)Instantiate(ServerCamera);
			ServerCameraObj.SetActive(false);
			ServerCameraObj.transform.parent = MissionCleanup;

			ServerCameraObjTK = (GameObject)Instantiate(ServerCameraTK);
			ServerCameraObjTK.SetActive(false);
			ServerCameraObjTK.transform.parent = MissionCleanup;
		}

		if (!GameMovieCtrl.IsActivePlayer) {
			SetActivePlayerOne(true);
			if (IsActiveAllPlayerTest || IsCartoonShootTest) {
				SetActivePlayerTwo(true);
			}
		}
		IsPlayGamePOne = IsActivePlayerOne;
		IsPlayGamePTwo = IsActivePlayerTwo;

		if (MinRandTimeServer >= MaxRandTimeServer) {
			Debug.LogWarning("MinRandTimeServer was wrong!");
		}
	}

	public void ChangeAudioListParent()
	{
		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}

		if (Application.loadedLevel < (int)GameLevel.Scene_1 || Application.loadedLevel > (int)GameLevel.Scene_4) {
			return;
		}

		Debug.Log("ChangeAudioListParent -> GameJiTaiSt "+GameJiTaiSt);
		switch (GameJiTaiSt) {
		case GameJiTaiType.FeiJiJiTai:
			if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
				AudioManager.Instance.SetParentTran(XkPlayerCtrl.GetInstanceFeiJi().transform);
			}
			break;

		case GameJiTaiType.TanKeJiTai:
			if (XkPlayerCtrl.GetInstanceTanKe() != null) {
				AudioManager.Instance.SetParentTran(XkPlayerCtrl.GetInstanceTanKe().transform);
			}
			break;
		}
	}

	void DelaySpawnClientPlayer()
	{
		Vector3 posPlayerFJ = new Vector3(0f, -1700f, 0f);
		Vector3 posPlayerTK = new Vector3(0f, -1500f, 0f);
		if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiFeiJi) {
			NetworkServerNet.GetInstance().SpawnNetPlayerObj(FeiJiPlayer,
			                                                      FeiJiPlayerPath,
			                                                      posPlayerFJ,
			                                                      FeiJiPlayer.transform.rotation);
		}
		else if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiTanKe) {
			NetworkServerNet.GetInstance().SpawnNetPlayerObj(TanKePlayer,
			                                                      TanKePlayerPath,
			                                                      posPlayerTK,
			                                                      TanKePlayer.transform.rotation);
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.P) && GameMovieCtrl.IsTestThreeScreen) {
			GameMovieCtrl.TestGameThreeScreen();
		}

		if (!pcvr.bIsHardWare) {
			if (IsCartoonShootTest) {
				if (Input.GetKeyUp(KeyCode.N)) {
					if (!XkGameCtrl.IsGameOnQuit && (Application.loadedLevel+1) < Application.levelCount) {
						System.GC.Collect();
						Application.LoadLevel((Application.loadedLevel+1));
					}
				}
			}

			if (GameModeVal == GameMode.LianJi
			    && Network.peerType == NetworkPeerType.Disconnected
			    && ScreenDanHeiCtrl.IsStartGame) {
				if (Input.GetKeyUp(KeyCode.P)) {
					TestChangePlayerCamera(); //test
				}
				
				if (Input.GetKeyUp(KeyCode.O)
				    && XKCameraMapCtrl.GetInstance() != null) {
					XKCameraMapCtrl.GetInstance().SetCameraMapState(); //test
				}
			}
			
			if (Input.GetKeyUp(KeyCode.S)) {
				IsShowDebugInfoBox = !IsShowDebugInfoBox; //test
			}

			if (Input.GetKeyUp(KeyCode.N)) {
				IsSubPlayerYouLiangTest = !IsSubPlayerYouLiangTest;
				//int testYLD = 100;
				if (IsSubPlayerYouLiangTest) {
					//float testYL = PlayerYouLiangMax * 0.3f;
					float testYL = 5f;
					PlayerYouLiangCur = testYL;
					if (IsActivePlayerOne) {
						PlayerYouLiangCurP1 = testYL;
						//AddYouLiangDian(testYLD, PlayerEnum.PlayerOne);
					}
					if (IsActivePlayerTwo) {
						PlayerYouLiangCurP2 = testYL;
						//AddYouLiangDian(testYLD, PlayerEnum.PlayerTwo);
					}
				}
				else {
					PlayerYouLiangCur = PlayerYouLiangMax;
					if (IsActivePlayerOne) {
						//PlayerYouLiangCurP1 = PlayerYouLiangMax;
						//AddYouLiangDian(testYLD, PlayerEnum.PlayerOne);
					}
					if (IsActivePlayerTwo) {
						//PlayerYouLiangCurP2 = PlayerYouLiangMax;
						//AddYouLiangDian(testYLD, PlayerEnum.PlayerTwo);
					}
				}
			}
		}

		CheckNpcAmmoList();
		CheckNpcTranFromList();
	}

	void DelayResetIsLoadingLevel()
	{
		XkGameCtrl.ResetIsLoadingLevel();
	}

	public static void ResetIsLoadingLevel()
	{
		IsLoadingLevel = false;
	}

	void TestInitCameraRender()
	{
		if (GameModeVal != GameMode.LianJi) {
			return;
		}

		if(XKPlayerCamera.GetInstanceCartoon() != null) {
			XKPlayerCamera.GetInstanceCartoon().SetActiveCamera(false);
		}

		if (XKPlayerCamera.GetInstanceTanKe() != null) {
			XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(false);
		}

		if (XKPlayerCamera.GetInstanceFeiJi() != null) {
			XKPlayerCamera.GetInstanceFeiJi().SetActiveCamera(false);
		}
		
		if (XKPlayerCamera.GetInstanceTanKe() != null) {
			XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(true);
		}
		
		if (XKPlayerCamera.GetInstanceFeiJi() != null) {
			XKPlayerCamera.GetInstanceFeiJi().SetActiveCamera(true);
			XKPlayerCamera.GetInstanceFeiJi().camera.targetTexture = null;
		}

		if (XKPlayerCamera.GetInstanceTanKe() != null && Network.peerType == NetworkPeerType.Disconnected) {
			XKPlayerCamera.GetInstanceTanKe().camera.targetTexture = TestCameraRender;
		}

		if(XKPlayerCamera.GetInstanceCartoon() != null) {
			XKPlayerCamera.GetInstanceCartoon().SetActiveCamera(true);
		}

		if (Network.peerType == NetworkPeerType.Disconnected) {
			if (XKPlayerCamera.GetInstanceFeiJi() != null && XKPlayerCamera.GetInstanceTanKe() != null) {
				XKPlayerCamera.GetInstanceFeiJi().gameObject.tag = TagMainCamera;
				XKPlayerCamera.GetInstanceTanKe().gameObject.tag = TagNull;
			}
			else if (XKPlayerCamera.GetInstanceFeiJi() != null) {
				XKPlayerCamera.GetInstanceFeiJi().gameObject.tag = TagMainCamera;
			}
			else if (XKPlayerCamera.GetInstanceTanKe() != null) {
				XKPlayerCamera.GetInstanceTanKe().gameObject.tag = TagMainCamera;
			}
		}
		else {
			if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiTanKe) {
				if (XKPlayerCamera.GetInstanceTanKe() != null) {
					XKPlayerCamera.GetInstanceTanKe().gameObject.tag = TagMainCamera;
				}
			}
			else {
				if (XKPlayerCamera.GetInstanceFeiJi() != null) {
					XKPlayerCamera.GetInstanceFeiJi().gameObject.tag = TagMainCamera;
				}
			}
		}
	}

	public void ChangePlayerCameraTag()
	{
		if (GameModeVal != GameMode.LianJi) {
			return;
		}

		if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiTanKe) {
			if (XKPlayerCamera.GetInstanceTanKe() != null) {
				XKPlayerCamera.GetInstanceTanKe().gameObject.tag = TagMainCamera;
				XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(true);
				XKPlayerCamera.GetInstanceTanKe().ActivePlayerCamera();
			}

			if (XKPlayerCamera.GetInstanceFeiJi() != null) {
				XKPlayerCamera.GetInstanceFeiJi().gameObject.tag = TagNull;
			}
		}
		else {
			if (XKPlayerCamera.GetInstanceTanKe() != null) {
				XKPlayerCamera.GetInstanceTanKe().gameObject.tag = TagNull;
			}

			if (XKPlayerCamera.GetInstanceFeiJi() != null) {
				XKPlayerCamera.GetInstanceFeiJi().gameObject.tag = TagMainCamera;
			}
		}
	}
	
	void TestChangePlayerCamera()
	{
		if (GameModeVal != GameMode.LianJi) {
			return;
		}

		//AudioListener listener = null;
		switch (GameJiTaiSt) {
		case GameJiTaiType.FeiJiJiTai:
			GameJiTaiSt = GameJiTaiType.TanKeJiTai;
			XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(false);
			XKPlayerCamera.GetInstanceFeiJi().SetActiveCamera(false);
			XKPlayerCamera.GetInstanceFeiJi().SetActiveCamera(true);
			if (!XKCameraMapCtrl.GetInstance().GetActiveCameraMap()) {
				XKPlayerCamera.GetInstanceFeiJi().SetEnableCamera(false);
			}
			XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(true);
			XKPlayerCamera.GetInstanceFeiJi().gameObject.tag = TagNull;
			XKPlayerCamera.GetInstanceTanKe().gameObject.tag = TagMainCamera;

			XKPlayerCamera.GetInstanceFeiJi().camera.targetTexture = TestCameraRender;
			XKPlayerCamera.GetInstanceTanKe().camera.targetTexture = null;
//			PlayerTranCurrent = XkPlayerCtrl.GetInstanceTanKe().transform;
			Camera.SetupCurrent(XKPlayerCamera.GetInstanceFeiJi().camera);

			/*listener = XKPlayerCamera.GetInstanceFeiJi().GetComponent<AudioListener>();
			listener.enabled = false;
			listener = XKPlayerCamera.GetInstanceTanKe().GetComponent<AudioListener>();
			if (ScreenDanHeiCtrl.IsStartGame) {
				listener.enabled = true;
			}
			else {
				listener.enabled = false;
			}*/
			break;

		case GameJiTaiType.TanKeJiTai:
			GameJiTaiSt = GameJiTaiType.FeiJiJiTai;
			XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(false);
			XKPlayerCamera.GetInstanceFeiJi().SetActiveCamera(false);
			XKPlayerCamera.GetInstanceTanKe().SetActiveCamera(true);
			if (!XKCameraMapCtrl.GetInstance().GetActiveCameraMap()) {
				XKPlayerCamera.GetInstanceTanKe().SetEnableCamera(false);
			}
			XKPlayerCamera.GetInstanceFeiJi().SetActiveCamera(true);
			XKPlayerCamera.GetInstanceFeiJi().gameObject.tag = TagMainCamera;
			XKPlayerCamera.GetInstanceTanKe().gameObject.tag = TagNull;

			XKPlayerCamera.GetInstanceFeiJi().camera.targetTexture = null;
			XKPlayerCamera.GetInstanceTanKe().camera.targetTexture = TestCameraRender;
//			PlayerTranCurrent = XkPlayerCtrl.GetInstanceFeiJi().transform;
			Camera.SetupCurrent(XKPlayerCamera.GetInstanceTanKe().camera);

			/*listener = XKPlayerCamera.GetInstanceFeiJi().GetComponent<AudioListener>();
			if (ScreenDanHeiCtrl.IsStartGame) {
				listener.enabled = true;
			}
			else {
				listener.enabled = false;
			}
			listener = XKPlayerCamera.GetInstanceTanKe().GetComponent<AudioListener>();
			listener.enabled = false;*/
			break;
		}
		ChangeAudioListParent();
	}

	public List<Transform> GetNpcTranList()
	{
		return NpcTranList;
	}

	public void AddNpcTranToList(Transform tran)
	{
		if (tran == null || NpcTranList.Contains(tran)) {
			return;
		}
		NpcTranList.Add(tran);
	}

	public void RemoveNpcTranFromList(Transform tran)
	{
		if (tran == null || !NpcTranList.Contains(tran)) {
			return;
		}
		NpcTranList.Remove(tran);
	}

	public void CheckNpcTranFromList()
	{
		float dTime = Time.realtimeSinceStartup - TimeCheckNpcTranList;
		if (dTime < 0.1f) {
			return;
		}
		TimeCheckNpcTranList = Time.realtimeSinceStartup;

		int max = NpcTranList.Count;
		int[] countArray = new int[max];
		int indexCount = 0;
		for (int i = 0; i < max; i++) {
			if (NpcTranList[i] == null) {
				countArray[indexCount] = i;
				indexCount++;
			}
		}
		
		for (int i = 0; i < max; i++) {
			if (countArray[i] == 0 && i > 0) {
				break;
			}

			if (countArray[i] >= NpcTranList.Count) {
				break;
			}

			if (NpcTranList[countArray[i]] == null) {
				NpcTranList.RemoveAt(countArray[i]);
			}
		}
	}

	public void AddPlayerKillNpc(PlayerEnum playerSt, NpcJiFenEnum npcSt)
	{
		AllPlayerKillNpc++;
		switch (playerSt) {
		case PlayerEnum.Null:
			if (XkGameCtrl.IsActivePlayerOne) {
				AddKillNpcToPlayerOne(npcSt);
			}

			if (XkGameCtrl.IsActivePlayerTwo) {
				AddKillNpcToPlayerTwo(npcSt);
			}
			break;

		case PlayerEnum.PlayerOne:
			AddKillNpcToPlayerOne(npcSt);
			break;

		case PlayerEnum.PlayerTwo:
			AddKillNpcToPlayerTwo(npcSt);
			break;
		}
	}

	void AddKillNpcToPlayerOne(NpcJiFenEnum npcSt)
	{
		switch (npcSt) {
		case NpcJiFenEnum.ShiBing:
			ShiBingNumPOne++;
			break;

		case NpcJiFenEnum.CheLiang:
			CheLiangNumPOne++;
			break;

		case NpcJiFenEnum.ChuanBo:
			ChuanBoNumPOne++;
			break;

		case NpcJiFenEnum.FeiJi:
			FeiJiNumPOne++;
			break;
		}
	}

	void AddKillNpcToPlayerTwo(NpcJiFenEnum npcSt)
	{
		switch (npcSt) {
		case NpcJiFenEnum.ShiBing:
			ShiBingNumPTwo++;
			break;
			
		case NpcJiFenEnum.CheLiang:
			CheLiangNumPTwo++;
			break;
			
		case NpcJiFenEnum.ChuanBo:
			ChuanBoNumPTwo++;
			break;
			
		case NpcJiFenEnum.FeiJi:
			FeiJiNumPTwo++;
			break;
		}
	}

	public void AddDaoDanNum(PlayerEnum playerSt)
	{
		switch(playerSt) {
		case PlayerEnum.PlayerOne:
			DaoDanNumPOne += DaoDanBuJiNum;
			if (DaoDanNumPOne > 99) {
				DaoDanNumPOne = 99;
			}
			DanYaoInfoCtrl.GetInstanceOne().ShowDaoDanSprite();
			break;
			
		case PlayerEnum.PlayerTwo:
			DaoDanNumPTwo += DaoDanBuJiNum;
			if (DaoDanNumPTwo > 99) {
				DaoDanNumPTwo = 99;
			}
			DanYaoInfoCtrl.GetInstanceTwo().ShowDaoDanSprite();
			break;
		}
	}
	
	public void AddGaoBaoDanNum(PlayerEnum playerSt)
	{
		switch(playerSt) {
		case PlayerEnum.PlayerOne:
			if (!IsActivePlayerOne) {
				return;
			}

			GaoBaoDanNumPOne += GaoBaoDanBuJiNum;
			if (GaoBaoDanNumPOne > 99) {
				GaoBaoDanNumPOne = 99;
			}
			DanYaoInfoCtrl.GetInstanceOne().ShowHuoLiJQSprite();
			XKPlayerAutoFire.AmmoStatePOne = PlayerAmmoType.GaoBaoAmmo;
			ZhunXingCtrl.GetInstanceOne().SetZhunXingSprite();
			break;
			
		case PlayerEnum.PlayerTwo:
			if (!IsActivePlayerTwo) {
				return;
			}

			GaoBaoDanNumPTwo += GaoBaoDanBuJiNum;
			if (GaoBaoDanNumPTwo > 99) {
				GaoBaoDanNumPTwo = 99;
			}
			DanYaoInfoCtrl.GetInstanceTwo().ShowHuoLiJQSprite();
			XKPlayerAutoFire.AmmoStatePTwo = PlayerAmmoType.GaoBaoAmmo;
			ZhunXingCtrl.GetInstanceTwo().SetZhunXingSprite();
			break;
		}
	}
	
	public int GetGaoBaoDanNumPOne()
	{
		return GaoBaoDanNumPOne;
	}

	public int GetGaoBaoDanNumPTwo()
	{
		return GaoBaoDanNumPTwo;
	}
	
	public void SubGaoBaoDanNumPOne()
	{
		//Debug.Log("SubGaoBaoDanNumPOne -> GaoBaoDanNumPOne "+GaoBaoDanNumPOne);
		GaoBaoDanNumPOne--;
		if (GaoBaoDanNumPOne <= 0) {
			XKPlayerAutoFire.AmmoStatePOne = PlayerAmmoType.PuTongAmmo;
			ZhunXingCtrl.GetInstanceOne().SetZhunXingSprite();
		}
	}
	
	public void SubGaoBaoDanNumPTwo()
	{
		GaoBaoDanNumPTwo--;
		if (GaoBaoDanNumPTwo <= 0) {
			XKPlayerAutoFire.AmmoStatePTwo = PlayerAmmoType.PuTongAmmo;
			ZhunXingCtrl.GetInstanceTwo().SetZhunXingSprite();
		}
	}
	
	public int GetDaoDanNumPOne()
	{
		return DaoDanNumPOne;
	}
	
	public int GetDaoDanNumPTwo()
	{
		return DaoDanNumPTwo;
	}

	public void SubDaoDanNumPOne()
	{
		DaoDanNumPOne--;
	}
	
	public void SubDaoDanNumPTwo()
	{
		DaoDanNumPTwo--;
	}

	static bool IsSubPlayerYouLiangTest = true;
	IEnumerator SubPlayerYouLiang()
	{
		do {
			yield return new WaitForSeconds(1.0f);
			if (JiFenJieMianCtrl.GetInstance() != null && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
				yield break;
			}
			
			if (XKTriggerClosePlayerUI.IsClosePlayerUI
			    || YouLiangCtrl.IsChangeYouLiangFillAmout
			    || IsAddPlayerYouLiang
			    || IsCartoonShootTest
			    || IsServerCameraTest) {
				continue;
			}

			if (Network.peerType == NetworkPeerType.Server) {
				continue;
			}

			if (XKTriggerGameOver.IsActiveGameOver) {
				yield break;
			}
			
			System.GC.Collect();
			switch (SelectYouLiangUI) {
			case 1:
				if (PlayerYouLiangCur <= 0f
				    || !ScreenDanHeiCtrl.IsStartGame
				    || !ZhunXingTeXiaoCtrl.IsOverTeXiaoZhunXing) {
					continue;
				}

				if (PlayerYouLiangCur > 0) {
					PlayerYouLiangCur -= 1f;
					//PlayerYouLiangCur += 1f; //test
					if (!pcvr.bIsHardWare && !IsSubPlayerYouLiangTest) {
						PlayerYouLiangCur += 1f; //test
					}
				}
				/*if (PlayerYouLiangCur > 15f) {
					PlayerYouLiangCur -= 14f; //test
				}*/
				
				if (!DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()) {
					if (PlayerYouLiangCur <= YouLiangJingGaoVal && !YouLiangCtrl.IsActiveYouLiangFlash) {
						YouLiangCtrl.GetInstance().SetActiveYouLiangFlash(true);
						XKGlobalData.GetInstance().PlayAudioRanLiaoJingGao();
					}
				}
				
				if (PlayerYouLiangCur <= 0f) {
					PlayerYouLiangCur = 0f;
					if (!JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
						DaoJiShiCtrl.GetInstance().StartPlayDaoJiShi();
					}
					SetActivePlayerOne(false);
					SetActivePlayerTwo(false);
				}
				break;
			case 2:
				if ((PlayerYouLiangCurP1 <= 0f && PlayerYouLiangCurP2 <= 0f)
				    || !ScreenDanHeiCtrl.IsStartGame
				    || !ZhunXingTeXiaoCtrl.IsOverTeXiaoZhunXing) {
					continue;
				}

				if (PlayerYouLiangCurP1 > 0) {
					PlayerYouLiangCurP1 -= 1f;
					//PlayerYouLiangCurP1 += 1f; //test
					if (!pcvr.bIsHardWare && !IsSubPlayerYouLiangTest) {
						PlayerYouLiangCurP1 += 1f; //test
					}
				}

				if (PlayerYouLiangCurP2 > 0) {
					PlayerYouLiangCurP2 -= 1f;
					//PlayerYouLiangCurP2 += 1f; //test
					if (!pcvr.bIsHardWare && !IsSubPlayerYouLiangTest) {
						PlayerYouLiangCurP2 += 1f; //test
					}
				}
				/*if (PlayerYouLiangCurP1 > 15f) {
					PlayerYouLiangCurP1 -= 14f; //test
				}
				if (PlayerYouLiangCurP2 > 15f) {
					PlayerYouLiangCurP2 -= 14f; //test
				}
				 */
				
				if (!DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()) {
					if (PlayerYouLiangCurP1 > 0f
						&& PlayerYouLiangCurP1 <= YouLiangJingGaoVal
					    && !YouLiangCtrl.IsActiveYouLiangFlash) {
						YouLiangCtrl.GetInstance().SetActiveYouLiangFlash(true);
						XKGlobalData.GetInstance().PlayAudioRanLiaoJingGao();
					}

					if (PlayerYouLiangCurP2 > 0f
					    && PlayerYouLiangCurP2 <= YouLiangJingGaoVal
					    && !YouLiangCtrl.IsActiveYouLiangFlash) {
						YouLiangCtrl.GetInstance().SetActiveYouLiangFlash(true);
						XKGlobalData.GetInstance().PlayAudioRanLiaoJingGao();
					}
				}
				
				//show DaoJiShiInfo.
				if (PlayerYouLiangCurP1 <= 0f && PlayerYouLiangCurP2 <= 0f) {
					PlayerYouLiangCurP1 = 0f;
					PlayerYouLiangCurP2 = 0f;
					if (!JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
						DaoJiShiCtrl.GetInstance().StartPlayDaoJiShi();
					}
				}
				
				if (PlayerYouLiangCurP1 <= 0f) {
					PlayerYouLiangCurP1 = 0f;
					SetActivePlayerOne(false);
					if (YouLiangCtrl.IsActiveYouLiangFlash
					    && PlayerYouLiangCurP2 > YouLiangJingGaoVal) {
						YouLiangCtrl.GetInstance().SetActiveYouLiangFlash(false);
					}
				}

				if (PlayerYouLiangCurP2 <= 0f) {
					PlayerYouLiangCurP2 = 0f;
					SetActivePlayerTwo(false);
					if (YouLiangCtrl.IsActiveYouLiangFlash
					    && PlayerYouLiangCurP1 > YouLiangJingGaoVal) {
						YouLiangCtrl.GetInstance().SetActiveYouLiangFlash(false);
					}
				}
				break;
			}
		} while(true);
	}

	public void AddPlayerYouLiang(float val, PlayerEnum indexPlayer = PlayerEnum.Null)
	{
//		PlayerYouLiangCur += YouLiangBuJiNum;
		if (DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()) {
			return;
		}
		IsAddPlayerYouLiang = false;

		switch (SelectYouLiangUI) {
		case 1:
			float startVal = PlayerYouLiangCur;
			PlayerYouLiangCur += val;
			if (PlayerYouLiangCur > PlayerYouLiangMax) {
				PlayerYouLiangCur = PlayerYouLiangMax;
			}
			YouLiangCtrl.GetInstance().InitChangePlayerYouLiangFillAmout(startVal, PlayerYouLiangCur);

			if (PlayerYouLiangCur > YouLiangJingGaoVal) {
				YouLiangCtrl.GetInstance().SetActiveYouLiangFlash(false);
			}
			break;
		case 2:
			if ((indexPlayer == PlayerEnum.Null || indexPlayer == PlayerEnum.PlayerOne)
			    && IsActivePlayerOne) {
				float startValP1 = PlayerYouLiangCurP1;
				PlayerYouLiangCurP1 += val;
				if (PlayerYouLiangCurP1 > PlayerYouLiangMax) {
					PlayerYouLiangCurP1 = PlayerYouLiangMax;
				}
				YouLiangCtrl.GetInstance().InitChangePlayerYouLiangFillAmout(startValP1,
				                                                             PlayerYouLiangCurP1,
				                                                             PlayerEnum.PlayerOne);

				if (PlayerYouLiangCurP1 > YouLiangJingGaoVal) {
					YouLiangCtrl.GetInstance().SetActiveYouLiangFlash(false);
				}
			}

			if ((indexPlayer == PlayerEnum.Null || indexPlayer == PlayerEnum.PlayerTwo)
			    && IsActivePlayerTwo) {
				float startValP2 = PlayerYouLiangCurP2;
				PlayerYouLiangCurP2 += val;
				if (PlayerYouLiangCurP2 > PlayerYouLiangMax) {
					PlayerYouLiangCurP2 = PlayerYouLiangMax;
				}
				YouLiangCtrl.GetInstance().InitChangePlayerYouLiangFillAmout(startValP2,
				                                                             PlayerYouLiangCurP2,
				                                                             PlayerEnum.PlayerTwo);

				if (PlayerYouLiangCurP2 > YouLiangJingGaoVal) {
					YouLiangCtrl.GetInstance().SetActiveYouLiangFlash(false);
				}
			}
			break;
		}
	}

	[Range(0f, 100f)]public float GameDiffTest = 0.8f;
	public static float GameDiffVal = 1f;
	/**
	 * GameDiffDiArray[0] -> 第一关油量点控制(游戏难度-低).
	 * GameDiffDiArray[3] -> 第四关油量点控制(游戏难度-低).
	 */
	static float[] GameDiffDiArray = {1f, 0.8f, 1f, 0.9f};
	/**
	 * GameDiffZhongArray[0] -> 第一关油量点控制(游戏难度-中).
	 * GameDiffZhongArray[3] -> 第四关油量点控制(游戏难度-中).
	 */
	static float[] GameDiffZhongArray = {0.8f, 0.6f, 0.8f, 0.7f};
	/**
	 * GameDiffGaoArray[0] -> 第一关油量点控制(游戏难度-高).
	 * GameDiffGaoArray[3] -> 第四关油量点控制(游戏难度-高).
	 */
	static float[] GameDiffGaoArray = {0.6f, 0.5f, 0.6f, 0.6f};
	public void AddYouLiangDian(int val, PlayerEnum playerSt)
	{
		if (IsCartoonShootTest) {
			return;
		}

		if (JiFenJieMianCtrl.GetInstance() != null) {
			if (JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
				return;
			}
		}

		if (SelectYouLiangUI == 2) {
			if (!IsActivePlayerOne && playerSt == PlayerEnum.PlayerOne) {
				return;
			}

			if (!IsActivePlayerTwo && playerSt == PlayerEnum.PlayerTwo) {
				return;
			}

			if (playerSt == PlayerEnum.Null) {
				if (!IsActivePlayerOne && !IsActivePlayerTwo) {
					return;
				}
			}
		}
		
		if (IsActivePlayerOne && IsActivePlayerTwo) {
			val = (int)(1.5f * val);
		}
		val = (int)(GameDiffVal * val);

		switch (playerSt) {
		case PlayerEnum.PlayerOne:
			YouLiangDianAddPOne += val;
			YouLiangDianValP1 += val;
			break;

		case PlayerEnum.PlayerTwo:
			YouLiangDianAddPTwo += val;
			YouLiangDianValP2 += val;
			break;

		default:
			if (IsActivePlayerOne) {
				YouLiangDianAddPOne += val;
				YouLiangDianValP1 += val;
			}
			if (IsActivePlayerTwo) {
				YouLiangDianAddPTwo += val;
				YouLiangDianValP2 += val;
			}
			break;
		}

		YouLiangDianVal += val;
		int youLiangDianTmp = YouLiangDianMin;
		if (IsActivePlayerOne && IsActivePlayerTwo) {
			if (GameModeVal == GameMode.LianJi) {
				if (GameJiTaiSt == GameJiTaiType.FeiJiJiTai) {
					youLiangDianTmp = YouLiangDianMinLJSR;
				}
				else {
					youLiangDianTmp = LJSRTKYouLiangDianMin;
				}
			}
			else {
				if (GameJiTaiSt == GameJiTaiType.FeiJiJiTai) {
					youLiangDianTmp = YouLiangDianMinSR;
				}
				else {
					youLiangDianTmp = SRTKYouLiangDianMin;
				}
			}
		}
		else {
			if (GameModeVal == GameMode.LianJi) {
				if (GameJiTaiSt == GameJiTaiType.FeiJiJiTai) {
					youLiangDianTmp = YouLiangDianMinLJ;
				}
				else {
					youLiangDianTmp = LJTKYouLiangDianMin;
				}
			}
			else {
				if (GameJiTaiSt != GameJiTaiType.FeiJiJiTai) {
					youLiangDianTmp = TKYouLiangDianMin;
				}
			}
		}

		float valTmp = 0f;
		switch (SelectYouLiangUI) {
		case 1:
			if (YouLiangDianVal >= youLiangDianTmp && !DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()) {
				IsAddPlayerYouLiang = true;
				YouLiangMarkCtrl.GetInstance().MoveYouLiangMark();
				YouLiangDianVal -= youLiangDianTmp;
			}
			
			valTmp = (float)YouLiangDianVal / youLiangDianTmp;
			YouLiangAddCtrl.GetInstance().SetYouLiangSpriteAmount(valTmp);
			break;
		case 2:
			if (YouLiangDianValP1 >= youLiangDianTmp && !DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()) {
				IsAddPlayerYouLiang = true;
				YouLiangMarkCtrl.GetInstanceP1().MoveYouLiangMark();
				YouLiangDianValP1 -= youLiangDianTmp;
			}

			if (YouLiangDianValP2 >= youLiangDianTmp && !DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()) {
				IsAddPlayerYouLiang = true;
				YouLiangMarkCtrl.GetInstanceP2().MoveYouLiangMark();
				YouLiangDianValP2 -= youLiangDianTmp;
			}
			
			valTmp = (float)YouLiangDianValP1 / youLiangDianTmp;
			YouLiangAddCtrl.GetInstance().SetYouLiangSpriteAmount(valTmp, PlayerEnum.PlayerOne);
			valTmp = (float)YouLiangDianValP2 / youLiangDianTmp;
			YouLiangAddCtrl.GetInstance().SetYouLiangSpriteAmount(valTmp, PlayerEnum.PlayerTwo);
			break;
		}
	}

	void ResetPlayerYouLiangVal(PlayerEnum indexPlayer = PlayerEnum.Null)
	{
		YouLiangDianVal = 0;
		switch (indexPlayer) {
		case PlayerEnum.PlayerOne:
			YouLiangDianValP1 = 0;
			break;
		case PlayerEnum.PlayerTwo:
			YouLiangDianValP2 = 0;
			break;
		}

		if (YouLiangAddCtrl.GetInstance() != null) {
			YouLiangAddCtrl.GetInstance().SetYouLiangSpriteAmount(0f, indexPlayer);
		}
	}

	public static bool GetIsActivePlayer(PlayerEnum indexPlayer)
	{
		if (indexPlayer == PlayerEnum.Null) {
			return false;
		}

		bool isActivePlayer = false;
		switch (indexPlayer) {
		case PlayerEnum.PlayerOne:
			isActivePlayer = IsActivePlayerOne;
			break;
		case PlayerEnum.PlayerTwo:
			isActivePlayer = IsActivePlayerTwo;
			break;
		case PlayerEnum.PlayerThree:
			isActivePlayer = IsActivePlayerThree;
			break;
		case PlayerEnum.PlayerFour:
			isActivePlayer = IsActivePlayerFour;
			break;
		}
		return isActivePlayer;
	}

	public static void SetActivePlayerOne(bool isActive)
	{
		IsActivePlayerOne = isActive;
		if (isActive) {
			IsPlayGamePOne = true;
			pcvr.StartLightStateP1 = LedState.Mie;
		}

		if (XkGameCtrl.GetInstance() == null ||
			(XkGameCtrl.GetInstance() != null && !XkGameCtrl.GetInstance().IsCartoonShootTest) ) {
			if (isActive && Application.loadedLevel == (int)GameLevel.Movie) {
				StopMovie();
			}
		}

		if (Application.loadedLevel >= (int)GameLevel.Scene_1 && Application.loadedLevel <= (int)GameLevel.Scene_4) {
			if (isActive) {
				if (!XKTriggerClosePlayerUI.IsClosePlayerUI) {
					DaoDanNumPOne = _Instance.DaoDanNum;
				}

				CheckMovePlayer();
				if (DanYaoInfoCtrl.GetInstanceOne() != null) {
					DanYaoInfoCtrl.GetInstanceOne().ShowPlayerDanYaoInfo();
				}
			}
			else {
				DanYaoInfoCtrl.GetInstanceOne().HiddenPlayerDanYaoInfo();
				if (XkGameCtrl.GetInstance() != null) {
					XkGameCtrl.GetInstance().ResetPlayerYouLiangVal(PlayerEnum.PlayerOne);
				}
			}
		}

		if (ZhunXingCtrl.GetInstanceOne() != null) {
			if (Application.loadedLevel == (int)GameLevel.Movie) {
				if (GameTypeCtrl.AppTypeStatic == AppGameType.DanJiTanKe
				    || GameTypeCtrl.AppTypeStatic == AppGameType.DanJiFeiJi) {
					isActive = false;
				}
			}
			ZhunXingCtrl.GetInstanceOne().SetActiveZhunXingObj(isActive);
		}
		SetPlayerFireMaxAmmoCount();
	}

	public static void SetActivePlayerTwo(bool isActive)
	{
		IsActivePlayerTwo = isActive;
		if (isActive) {
			IsPlayGamePTwo = true;
			pcvr.StartLightStateP2 = LedState.Mie;
		}
		
		if (XkGameCtrl.GetInstance() == null ||
		    (XkGameCtrl.GetInstance() != null && !XkGameCtrl.GetInstance().IsCartoonShootTest) ) {
			if (isActive && Application.loadedLevel == (int)GameLevel.Movie) {
				StopMovie();
			}
		}
		
		if (Application.loadedLevel >= (int)GameLevel.Scene_1 && Application.loadedLevel <= (int)GameLevel.Scene_4) {
			if (isActive) {
				if (!XKTriggerClosePlayerUI.IsClosePlayerUI) {
					DaoDanNumPTwo = _Instance.DaoDanNum;
				}

				CheckMovePlayer();
				if (DanYaoInfoCtrl.GetInstanceTwo() != null) {
					DanYaoInfoCtrl.GetInstanceTwo().ShowPlayerDanYaoInfo();
				}
			}
			else {
				DanYaoInfoCtrl.GetInstanceTwo().HiddenPlayerDanYaoInfo();
				if (XkGameCtrl.GetInstance() != null) {
					XkGameCtrl.GetInstance().ResetPlayerYouLiangVal(PlayerEnum.PlayerTwo);
				}
			}
		}

		if (ZhunXingCtrl.GetInstanceTwo() != null) {
			if (Application.loadedLevel == (int)GameLevel.Movie) {
				if (GameTypeCtrl.AppTypeStatic == AppGameType.DanJiTanKe
				    || GameTypeCtrl.AppTypeStatic == AppGameType.DanJiFeiJi) {
					isActive = false;
				}
			}
			ZhunXingCtrl.GetInstanceTwo().SetActiveZhunXingObj(isActive);
		}
		SetPlayerFireMaxAmmoCount();
	}
	
	static void SetPlayerFireMaxAmmoCount()
	{		
		if (!IsActivePlayerOne || !IsActivePlayerTwo) {
			XKPlayerAutoFire.MaxAmmoCount = 15;
		}
		else {
			XKPlayerAutoFire.MaxAmmoCount = 30;
		}
	}

	static void StopMovie()
	{
		if (Application.loadedLevel != (int)GameLevel.Movie) {
			return;
		}
		GameMovieCtrl.GetInstance().StopPlayMovie();
		switch (GameTypeCtrl.AppTypeStatic) {
		case AppGameType.DanJiFeiJi:
		case AppGameType.DanJiTanKe:
			GameModeCtrl.GetInstance().SetActiveLoading(true);
			break;

		case AppGameType.LianJiFeiJi:
		case AppGameType.LianJiTanKe:
			GameModeCtrl.GetInstance().ShowGameMode();
			break;
		}
	}

//	static int LevelCountValTest = 0;
	public static void LoadingGameScene_1()
	{
		//Debug.Log("LoadingGameScene_1...");
		GameMovieCtrl.GetInstance().StopPlayMovie();
		XkGameCtrl.IsLoadingLevel = true;
		//return; //test
		if (!XkGameCtrl.IsGameOnQuit) {
			System.GC.Collect();
			Application.LoadLevel((int)GameLevel.Scene_1);
			//Application.LoadLevel((int)GameLevel.Scene_4);//test
//			LevelCountValTest++;
//			Application.LoadLevel(LevelCountValTest);
//			if (LevelCountValTest >= (int)GameLevel.Scene_3) {
//				LevelCountValTest = 0;
//			}
		}
	}

	public static void LoadingGameMovie(int key = 0)
	{
		if (XkGameCtrl.IsLoadingLevel) {
			return;
		}

		if (NetworkServerNet.GetInstance() != null && NetCtrl.GetInstance() != null && key == 0) {
			NetworkServerNet.GetInstance().MakeClientDisconnect(); //Close ClientNet
			NetworkServerNet.GetInstance().MakeServerDisconnect(); //Close ServerNet
		}
		
		ResetGameInfo();
		SetActivePlayerOne(false);
		SetActivePlayerTwo(false);
		IsLoadingLevel = true;
		if (!XkGameCtrl.IsGameOnQuit) {
			System.GC.Collect();
			Application.LoadLevel((int)GameLevel.Movie);
		}
	}

	public static void CheckMovePlayer()
	{
		if (!ScreenDanHeiCtrl.IsStartGame) {
			return;
		}

		switch (XkGameCtrl.GameModeVal) {
		case GameMode.DanJiFeiJi:
			if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
				if (!IsActivePlayerOne && !IsActivePlayerTwo) {
					XkPlayerCtrl.GetInstanceFeiJi().StopMovePlayer();
				}
				else {
//					AddPlayerYouLiangToMax();
					if (DaoJiShiCtrl.GetInstance() != null) {
						DaoJiShiCtrl.GetInstance().StopDaoJiShi();
					}
					XkPlayerCtrl.GetInstanceFeiJi().RestartMovePlayer();
				}
			}
			break;

		case GameMode.DanJiTanKe:
			if (XkPlayerCtrl.GetInstanceTanKe() != null) {
				if (!IsActivePlayerOne && !IsActivePlayerTwo) {
					XkPlayerCtrl.GetInstanceTanKe().StopMovePlayer();
				}
				else {
//					AddPlayerYouLiangToMax();
					if (DaoJiShiCtrl.GetInstance() != null) {
						DaoJiShiCtrl.GetInstance().StopDaoJiShi();
					}
					XkPlayerCtrl.GetInstanceTanKe().RestartMovePlayer();
				}
			}
			break;

		case GameMode.LianJi:
			if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
				if (!IsActivePlayerOne && !IsActivePlayerTwo) {
					XkPlayerCtrl.GetInstanceFeiJi().StopMovePlayer();
				}
				else {
//					AddPlayerYouLiangToMax();
					if (DaoJiShiCtrl.GetInstance() != null) {
						DaoJiShiCtrl.GetInstance().StopDaoJiShi();
					}
					XkPlayerCtrl.GetInstanceFeiJi().RestartMovePlayer();
				}
			}

			if (XkPlayerCtrl.GetInstanceTanKe() != null) {
				if (!IsActivePlayerOne && !IsActivePlayerTwo) {
					XkPlayerCtrl.GetInstanceTanKe().StopMovePlayer();
				}
				else {
//					AddPlayerYouLiangToMax();
					if (DaoJiShiCtrl.GetInstance() != null) {
						DaoJiShiCtrl.GetInstance().StopDaoJiShi();
					}
					XkPlayerCtrl.GetInstanceTanKe().RestartMovePlayer();
				}
			}
			break;
		}
	}

	public static void AddPlayerYouLiangToMax()
	{
		GameLevel levelVal = (GameLevel)Application.loadedLevel;
		switch (levelVal) {
		case GameLevel.Scene_1:
			PlayerYouLiangMax = 90f;
			break;
		case GameLevel.Scene_2:
			PlayerYouLiangMax = 60f;
			break;
		case GameLevel.Scene_3:
			PlayerYouLiangMax = 60f;
			break;
		case GameLevel.Scene_4:
			PlayerYouLiangMax = 60f;
			break;
		}
		//Debug.Log("AddPlayerYouLiangToMax -> PlayerYouLiangMax "+PlayerYouLiangMax);

		switch (SelectYouLiangUI) {
		case 1:
			PlayerYouLiangCur = PlayerYouLiangMax;
			break;
		case 2:
			if (IsActivePlayerOne) {
				PlayerYouLiangCurP1 = PlayerYouLiangCurP1 > 0f ? PlayerYouLiangCurP1 : PlayerYouLiangMax;
			}
			if (IsActivePlayerTwo) {
				PlayerYouLiangCurP2 = PlayerYouLiangCurP2 > 0f ? PlayerYouLiangCurP2 : PlayerYouLiangMax;
			}
			break;
		}
//		PlayerYouLiangCur = 10f; //test
		if (YouLiangCtrl.GetInstance() != null) {
			YouLiangCtrl.GetInstance().SetActiveYouLiangFlash(false);
		}
	}

	public static void OnPlayerFinishTask()
	{
		if (IsActiveFinishTask) {
			return;
		}
		IsActiveFinishTask = true;
		JiFenJieMianCtrl.GetInstance().ShowFinishTaskInfo();
	}
	
	public int GetFeiJiMarkIndex()
	{
		return FeiJiMarkIndex;
	}

	public int GetTanKeMarkIndex()
	{
		return TanKeMarkIndex;
	}

	public int GetCartoonCamMarkIndex()
	{
		return CartoonCamMarkIndex;
	}

	public static void AddCartoonTriggerSpawnList(XKTriggerRemoveNpc script)
	{
		if (script == null) {
			return;
		}

		if (CartoonTriggerSpawnList.Contains(script)) {
			return;
		}
		CartoonTriggerSpawnList.Add(script);
	}

	public static void ClearCartoonSpawnNpc()
	{
		Debug.Log("ClearCartoonSpawnNpc...");
		int max = CartoonTriggerSpawnList.Count;
		for (int i = 0; i < max; i++) {
			CartoonTriggerSpawnList[i].RemoveSpawnPointNpc();
		}
		CartoonTriggerSpawnList.Clear();
	}
	
	public static void AddNpcAmmoList(GameObject obj)
	{
		if (NpcAmmoList.Contains(obj)) {
			return;
		}
		CountNpcAmmo++;
		NpcAmmoList.Add(obj);
	}
	
	public static void RemoveNpcAmmoList(GameObject obj)
	{
		if (!NpcAmmoList.Contains(obj)) {
			return;
		}
		CountNpcAmmo--;
		NpcAmmoList.Remove(obj);
	}

	void CheckNpcAmmoList()
	{
		float dTime = Time.realtimeSinceStartup - TimeCheckNpcAmmo;
		if (dTime < 0.1f) {
			return;
		}
		TimeCheckNpcAmmo = Time.realtimeSinceStartup;
		
		int maxAmmo = AmmoNumMaxNpc;
		if (NpcAmmoList.Count <= maxAmmo) {
			return;
		}
		
		int num = NpcAmmoList.Count - maxAmmo;
		GameObject[] ammoArray = new GameObject[num];
		for (int i = 0; i < num; i++) {
			ammoArray[i] = NpcAmmoList[i];
		}
		
		NpcAmmoCtrl script = null;
		for (int i = 0; i < num; i++) {
			if (ammoArray[i] == null) {
				continue;
			}
			
			script = ammoArray[i].GetComponent<NpcAmmoCtrl>();
			if (script == null) {
				NpcAmmoList.Remove(ammoArray[i]);
				continue;
			}
			script.GameNeedRemoveAmmo();
		}
	}
	
	public void CheckIsCartoonShootTest()
	{
		if (!IsCartoonShootTest) {
			return;
		}
		
		XKTriggerEndCartoon.GetInstance().CloseStartCartoon(); //test
	}
	
	public void ChangeBoxColliderSize(Transform tran)
	{
		Vector3 scaleVal = tran.localScale;
		scaleVal.z = 1f;
		tran.localScale = scaleVal;

		BoxCollider box = tran.GetComponent<BoxCollider>();
		Vector3 sizeBox = box.size;
		sizeBox.z = TriggerBoxSize_Z;
		box.size = sizeBox;
	}

	public static void SetServerCameraTran(Transform tran)
	{
		if (ServerCameraPar != null) {
			ServerCameraPar.SetActive(false);
		}
		ServerCameraPar = tran.gameObject;

		if (!tran.gameObject.activeSelf) {
			tran.gameObject.SetActive(true);
		}

		if (tran.camera != null && tran.camera.enabled) {
			tran.camera.enabled = false;
		}

		Transform serverCamTran = ServerCameraObj.transform;
		serverCamTran.parent = tran;
		serverCamTran.localPosition = Vector3.zero;
		serverCamTran.localEulerAngles = Vector3.zero;
		if (!ServerCameraObj.activeSelf) {
			ServerCameraObj.SetActive(true);
		}
	}

	public static void CheckObjDestroyThisTimed(GameObject obj)
	{
		if (GameMovieCtrl.IsActivePlayer) {
			return;
		}

		if (obj == null) {
			return;
		}

		DestroyThisTimed script = obj.GetComponent<DestroyThisTimed>();
		if (script == null) {
			script = obj.AddComponent<DestroyThisTimed>();
			script.TimeRemove = 1f;
			Debug.LogWarning("obj is not find DestroyThisTimed! name is "+obj.name);
		}
	}

	public static void ResetGameInfo()
	{
		DaoDanNumPOne = 0;
		DaoDanNumPTwo = 0;
		GaoBaoDanNumPOne = 0;
		GaoBaoDanNumPTwo = 0;
	}
	
	public static void SetParentTran(Transform tran, Transform parTran)
	{
		tran.parent = parTran;
		tran.localPosition = Vector3.zero;
		tran.localEulerAngles = Vector3.zero;
	}

	public static void HiddenMissionCleanup()
	{
		if (MissionCleanup == null || !MissionCleanup.gameObject.activeSelf) {
			return;
		}

//		if (Network.peerType == NetworkPeerType.Client) {
//			return;
//		}
		if (GameModeVal == GameMode.LianJi) {
			return;
		}
		MissionCleanup.gameObject.SetActive(false);
	}

	public static bool GetMissionCleanupIsActive()
	{
		return MissionCleanup.gameObject.activeSelf;
	}
	
	public static void SetActiveGameWaterObj(bool isActive)
	{
		if (_Instance == null) {
			return;
		}
		
		if (_Instance.GameWaterObj == null) {
			return;
		}
		
		if (isActive == _Instance.GameWaterObj.activeSelf) {
			return;
		}
		_Instance.GameWaterObj.SetActive(isActive);
	}
	
	public static void ActiveServerCameraTran()
	{
		Debug.Log("ActiveServerCameraTran...");
		ServerPortCameraCtrl.RandOpenServerPortCamera();
	}
	
	public static void AddYLDLv(YouLiangDianMoveCtrl script)
	{
		YouLiangDengJi levelValTmp = script.LevelVal;
		switch (levelValTmp) {
		case YouLiangDengJi.Level_1:
			if (YLDLvA.Contains(script)) {
				return;
			}
			YLDLvA.Add(script);
			break;
			
		case YouLiangDengJi.Level_2:
			if (YLDLvB.Contains(script)) {
				return;
			}
			YLDLvB.Add(script);
			break;
			
		case YouLiangDengJi.Level_3:
			if (YLDLvC.Contains(script)) {
				return;
			}
			YLDLvC.Add(script);
			break;
		}
	}

	public static YouLiangDianMoveCtrl GetYLDMoveScript(YouLiangDengJi levelValTmp)
	{
		int maxNum = 0;
		YouLiangDianMoveCtrl yldScript = null;
		switch (levelValTmp) {
		case YouLiangDengJi.Level_1:
			maxNum = YLDLvA.Count;
			for (int i = 0; i < maxNum; i++) {
				if (YLDLvA[i] != null && !YLDLvA[i].gameObject.activeSelf) {
					yldScript = YLDLvA[i];
					break;
				}
			}
			break;
			
		case YouLiangDengJi.Level_2:
			maxNum = YLDLvB.Count;
			for (int i = 0; i < maxNum; i++) {
				if (YLDLvB[i] != null && !YLDLvB[i].gameObject.activeSelf) {
					yldScript = YLDLvB[i];
					break;
				}
			}
			break;
			
		case YouLiangDengJi.Level_3:
			maxNum = YLDLvC.Count;
			for (int i = 0; i < maxNum; i++) {
				if (YLDLvC[i] != null && !YLDLvC[i].gameObject.activeSelf) {
					yldScript = YLDLvC[i];
					break;
				}
			}
			break;
		}
		
		if (yldScript == null) {
			yldScript = YouLiangDianUICtrl.GetInstance().SpawnYouLiangDianUI(levelValTmp);
			AddYLDLv(yldScript);
		}
		return yldScript;
	}

	public static void ClearNpcSpawnAllAmmo(GameObject npcObj)
	{
		bool isClearNpcAmmoTest = false;
		if (!isClearNpcAmmoTest) {
			return;
		}

		if (npcObj == null) {
			return;
		}
		
		XKCannonCtrl[] npcCannonScript = npcObj.GetComponentsInChildren<XKCannonCtrl>();
		int max = npcCannonScript.Length;
		if (max > 0) {
			for (int i = 0; i < max; i++) {
				if (npcCannonScript[i] != null) {
					npcCannonScript[i].ClearNpcAmmoList();
				}
			}
		}

		XkNpcZaiTiCtrl[] zaiTiScriptArray = npcObj.GetComponentsInChildren<XkNpcZaiTiCtrl>();
		max = zaiTiScriptArray.Length;
		if (max > 0) {
			for (int i = 0; i < max; i++) {
				if (zaiTiScriptArray[i] != null
				    && zaiTiScriptArray[i].IsTeShuFireNpc
				    && zaiTiScriptArray[i].TimeFireAmmo.Length > 0) {
					zaiTiScriptArray[i].ClearNpcAmmoList();
				}
			}
		}
		
		XKNpcAnimatorCtrl[] npcAniScript = npcObj.GetComponentsInChildren<XKNpcAnimatorCtrl>();
		max = npcAniScript.Length;
		if (max > 0) {
			for (int i = 0; i < max; i++) {
				if (npcAniScript[i] != null) {
					npcAniScript[i].ClearNpcAmmoList();
				}
			}
		}
	}

	void OnGUI()
	{
		if (IsCartoonShootTest || !IsShowDebugInfoBox) {
			return;
		}

		float hight = 25f;
		float width = 600;
		string infoA = "P1: ShiBing "+ShiBingNumPOne+" CheLiang "+CheLiangNumPOne+" ChuanBo "+ChuanBoNumPOne+" FeiJi "+FeiJiNumPOne;
		GUI.Box(new Rect(0f, 0f, width, hight), infoA);

		string infoB = "P2: ShiBing "+ShiBingNumPTwo+" CheLiang "+CheLiangNumPTwo+" ChuanBo "+ChuanBoNumPTwo+" FeiJi "+FeiJiNumPTwo;
		GUI.Box(new Rect(0f, hight, width, hight), infoB);

		string infoD = "P1: YouLiangAdd "+YouLiangDianAddPOne+" P2: YouLiangAdd "+YouLiangDianAddPTwo
						+", GameDiffVal "+GameDiffVal;
		GUI.Box(new Rect(0f, hight*2f, width, hight), infoD);

		string infoE = "PlayerYouLiangMax "+PlayerYouLiangMax+", PlayerYouLiang "+PlayerYouLiangCur
			+", PlayerYouLiangCurP1 "+PlayerYouLiangCurP1
			+", PlayerYouLiangCurP2 "+PlayerYouLiangCurP2;
		GUI.Box(new Rect(0f, hight*3f, width, hight), infoE);
		
		string infoF = "CountNpcAmmo "+CountNpcAmmo
				+", CountPlayerAmmo "+XKPlayerAutoFire.PlayerAmmoNumTest
				+", NpcNum "+NpcTranList.Count;
		GUI.Box(new Rect(0f, hight*4f, width, hight), infoF);

		XkPlayerCtrl playerScript = null;
		switch (GameModeVal) {
		case GameMode.DanJiFeiJi:
			playerScript = XkPlayerCtrl.GetInstanceFeiJi();
			break;
			
		case GameMode.DanJiTanKe:
			playerScript = XkPlayerCtrl.GetInstanceTanKe();
			break;
			
		case GameMode.LianJi:
			if (GameJiTaiSt == GameJiTaiType.FeiJiJiTai) {
				playerScript = XkPlayerCtrl.GetInstanceFeiJi();
			}
			else if (GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
				playerScript = XkPlayerCtrl.GetInstanceTanKe();
			}
			break;
		}

		if (playerScript != null && playerScript.GetAiPathScript() != null
		    && playerScript.GetAimMvToMarkTran() != null) {
			string infoH = "Path "+playerScript.GetAiPathScript().name + ", Mark "+playerScript.GetAimMvToMarkTran().name;
			XKPlayerCamera cameraScript = playerScript.GetPlayerCameraScript();
			if (cameraScript != null) {
				if (cameraScript.GetAimTram() != null) {
					infoH += ", CameraAim is "+cameraScript.GetAimTram().name;
				}
				else {
					infoH += ", CameraAim is null";
				}
			}
			infoH += ", GameModeVal "+GameModeVal;
			GUI.Box(new Rect(0f, hight*5f, width, hight), infoH);
		}

		string infoI = "TestDTimeVal "+XkPlayerCtrl.TestDTimeVal.ToString("f3");
		if (GameModeVal == GameMode.DanJiFeiJi || GameModeVal == GameMode.DanJiTanKe) {
			infoI += ", TestSpeed " + XkPlayerCtrl.TestSpeed.ToString("f3");
		}
		else {
			
			if (XkGameCtrl.GameModeVal == GameMode.LianJi) {
				if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.FeiJiJiTai) {
					infoI += ", TestSpeed " + XkPlayerCtrl.TestSpeed.ToString("f3");
				}
				else if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
					infoI += ", TestSpeed " + XkPlayerCtrl.TestSpeed.ToString("f3");
				}
			}
		}
		infoI += ", AP1 "+IsActivePlayerOne+", AP2 "+IsActivePlayerTwo
					+", DP1 "+DaoJiShiCtrl.IsActivePlayerOne
					+", DP2 "+DaoJiShiCtrl.IsActivePlayerTwo;
		GUI.Box(new Rect(0f, hight*6f, width, hight), infoI);

		string infoJ = "QiNangTK: Q "+XKPlayerDongGanCtrl.QiNangStateTK[0]
		+", H "+XKPlayerDongGanCtrl.QiNangStateTK[1]
		+", Z "+XKPlayerDongGanCtrl.QiNangStateTK[2]
		+", Y "+XKPlayerDongGanCtrl.QiNangStateTK[3]
		+", q0 "+pcvr.QiNangArray[0]
		+", q1 "+pcvr.QiNangArray[1]
		+", q2 "+pcvr.QiNangArray[2]
		+", q3 "+pcvr.QiNangArray[3]
		+", q4 "+pcvr.QiNangArray[4]
		+", q5 "+pcvr.QiNangArray[5]
		+", q6 "+pcvr.QiNangArray[6]
		+", q7 "+pcvr.QiNangArray[7]
		+", AppType "+GameTypeCtrl.AppTypeStatic;
		GUI.Box(new Rect(0f, hight*7f, width, hight), infoJ);

		string infoK = "QiNangFJ: Q "+XKPlayerDongGanCtrl.QiNangStateFJ[0]
		+", H "+XKPlayerDongGanCtrl.QiNangStateFJ[1]
		+", Z "+XKPlayerDongGanCtrl.QiNangStateFJ[2]
		+", Y "+XKPlayerDongGanCtrl.QiNangStateFJ[3];
		GUI.Box(new Rect(0f, hight*8f, width, hight), infoK);
		
		string infoL = "IsStartGame "+ScreenDanHeiCtrl.IsStartGame;
		if (XkPlayerCtrl.PlayerTranFeiJi == null) {
			infoL += ", PlayerFJ == null";
		}
		else {
			infoL += ", PlayerFJ != null";
		}

		if (XkPlayerCtrl.PlayerTranTanKe == null) {
			infoL += ", PlayerTK == null";
		}
		else {
			infoL += ", PlayerTK != null";
		}
		GUI.Box(new Rect(0f, hight*9f, width, hight), infoL);
	}
}