using UnityEngine;
using System.Collections;

using System;
using System.Runtime.InteropServices;

public class NetCtrl : MonoBehaviour {
	public GameObject TestPlayer;
	public GameObject TestNpc;
	public static int SelectLinkCount;
	bool IsSendAddLinkCount;
	public static int MaxPlayerCount = 2;
	bool IsSendLoadLevel;
	bool IsSendSubLinkCount;
	NetworkView NetworkViewCom;
	bool IsMakeGameCloseCartoon;
	bool IsMakeGameStopJiFenTime;
	bool IsMakeClientShowFinishTask;
	int CountGameOver;
	private static NetCtrl _Instance;
	public static NetCtrl GetInstance()
	{
		return _Instance;
	}

	void Awake()
	{
		_Instance = this;
		if (Application.loadedLevel == (int)GameLevel.Movie) {
			MaxPlayerCount = 2;
			//MaxPlayerCount = 1; //test
		}

		NetworkViewCom = GetComponent<NetworkView>();
		gameObject.name = "_NetCtrl";
		DontDestroyOnLoad(gameObject);

		NetworkServerNet.NetCtrlScript = _Instance;
		if (Network.peerType == NetworkPeerType.Client) {
			SendAddLinkCount();
		}
	}

	public void MakeClientPlayerMove()
	{
		NetworkViewCom.RPC("NetCtrlMakeClientPlayerMove", RPCMode.OthersBuffered);
	}
	
	[RPC] void NetCtrlMakeClientPlayerMove()
	{
		Debug.Log("NetCtrlMakeClientPlayerMove......");
		if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiFeiJi) {
			if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
				XkPlayerCtrl.GetInstanceFeiJi().RestartMovePlayer();
				XkGameCtrl.GetInstance().ChangePlayerCameraTag();
			}
			else {
				Debug.Log("NetCtrlMakeClientPlayerMove -> FeiJiPlayer is null");
				StartCoroutine(LoopCheckPlayerRestartMove());
			}
		}

		if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiTanKe) {
			if (XkPlayerCtrl.GetInstanceTanKe() != null) {
				XkPlayerCtrl.GetInstanceTanKe().RestartMovePlayer();
				XkGameCtrl.GetInstance().ChangePlayerCameraTag();
			}
			else {
				Debug.Log("NetCtrlMakeClientPlayerMove -> TanKePlayer is null");
				StartCoroutine(LoopCheckPlayerRestartMove());
			}
		}
	}

	IEnumerator LoopCheckPlayerRestartMove()
	{
		bool isLoopCheck = true;
		do {
			yield return new WaitForSeconds(0.1f);

			if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiFeiJi) {
				if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
					XkPlayerCtrl.GetInstanceFeiJi().RestartMovePlayer();
					XkGameCtrl.GetInstance().ChangePlayerCameraTag();
					isLoopCheck = false;
					yield break;
				}
			}
			
			if (GameTypeCtrl.AppTypeStatic == AppGameType.LianJiTanKe) {
				if (XkPlayerCtrl.GetInstanceTanKe() != null) {
					XkPlayerCtrl.GetInstanceTanKe().RestartMovePlayer();
					XkGameCtrl.GetInstance().ChangePlayerCameraTag();
					isLoopCheck = false;
					yield break;
				}
			}
		} while (isLoopCheck);
	}

	public void SetScreenDanHieStartMovePlayer()
	{
		NetworkViewCom.RPC("NetCtrlSetScreenDanHieStartMovePlayer", RPCMode.Server);
	}

	[RPC] void NetCtrlSetScreenDanHieStartMovePlayer()
	{
		ScreenDanHeiCtrl.GetInstance().AddStartMovePlayerCount();
	}

	public void SendAddLinkCount()
	{
		if (Network.peerType != NetworkPeerType.Client) {
			return;
		}

		if (IsSendAddLinkCount) {
			return;
		}
		Debug.Log("SendAddLinkCount...");
		IsSendAddLinkCount = true;
		IsSendSubLinkCount = false;
		NetworkViewCom.RPC("NetCtrlSendAddLinkCount", RPCMode.Server);
	}

	[RPC] void NetCtrlSendAddLinkCount()
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		SelectLinkCount++;

		Debug.Log("NetCtrlSendAddLinkCount -> SelectLinkCount "+SelectLinkCount+", MaxPlayerCount "+MaxPlayerCount);
		if (SelectLinkCount >= MaxPlayerCount) {
			//Loading game, start to link game.
			SelectLinkCount = 0;
			Network.incomingPassword = ServerPassword.XKGame.ToString();
			SendClientLoadGameLevel();
			if (NetworkServerNet.GetInstance() != null) {
				NetworkServerNet.GetInstance().SetIsCheckServerPortPlayerNum();
			}
		}
		else {
			IsSendLoadLevel = false;
		}
	}

	void SendClientLoadGameLevel()
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}

		if (IsSendLoadLevel) {
			return;
		}
		IsSendLoadLevel = true;

		XkGameCtrl.IsLoadingLevel = true;
		if (Application.loadedLevel == (int)GameLevel.Movie) {
			//Invoke("DelayLoadingGameScene_1", 6f);
			StartCoroutine(DelayLoadingGameScene_1());
		}
		NetworkViewCom.RPC("NetCtrlSendClientLoadGameLevel", RPCMode.Others);
	}

	IEnumerator DelayLoadingGameScene_1()
	{
		yield return new WaitForSeconds(6f);
		Debug.Log("DelayLoadingGameScene...");
		if (Application.loadedLevel == (int)GameLevel.Movie) {
			XkGameCtrl.LoadingGameScene_1();
		}
	}

	[RPC] void NetCtrlSendClientLoadGameLevel()
	{
		if (Network.peerType != NetworkPeerType.Client) {
			return;
		}

		if (Application.loadedLevel == (int)GameLevel.Movie) {
			GameModeCtrl.GetInstance().ServerCallClientLoadingGame();
		}
		else {
			XunZhangZPCtrl.IsShouldStopJiFenPanel = true;
			if (XunZhangZPCtrl.IsOverPlayerZPXunZhang) {
				XunZhangZPCtrl.GetInstanceOne().CheckLianJiIsShouldStopJiFenPanel();
			}
		}
	}

	public void SendSubLinkCount()
	{
		if (Network.peerType != NetworkPeerType.Client) {
			return;
		}
		
		if (IsSendSubLinkCount) {
			return;
		}
		IsSendAddLinkCount = false;
		IsSendSubLinkCount = true;
		NetworkViewCom.RPC("NetCtrlSendSubLinkCount", RPCMode.Server);
	}

	[RPC] void NetCtrlSendSubLinkCount()
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}

		if (SelectLinkCount <= 0) {
			SelectLinkCount = 0;
			return;
		}
		SelectLinkCount--;
	}

	void Update()
	{
		if(NetworkViewCom.isMine && Network.isServer) {
			if (!pcvr.bIsHardWare && Input.GetKeyUp(KeyCode.M)) {
				NetCtrlMakeOtherClientShowFinishTask(1);
			}
		}
		else {
			if (Network.peerType == NetworkPeerType.Disconnected) {
				Network.Destroy(gameObject);
			}
		}
	}

	public void CloseServerPortSendRPC()
	{
		if (!Network.isClient) {
			return;
		}

		if (Network.peerType == NetworkPeerType.Disconnected) {
			return;
		}
		NetworkViewCom.RPC("NetCtrlRPCSendCloseServerPortSend", RPCMode.Others);
	}

	[RPC] void NetCtrlRPCSendCloseServerPortSend()
	{
		if (!Network.isServer) {
			return;
		}
		NetworkServerNet.SetServerSendState(1);
	}
	
	public void CloseClientPortSendRPC()
	{
		if (!Network.isServer) {
			return;
		}
		NetworkViewCom.RPC("NetCtrlRPCSendCloseClientPortSend", RPCMode.Others);
	}
	
	[RPC] void NetCtrlRPCSendCloseClientPortSend()
	{
		if (!Network.isClient) {
			return;
		}
		NetworkServerNet.SetServerSendState(1);
	}

	public void SendSetScreenDanHeiIsStartGame()
	{
		NetworkViewCom.RPC("NetCtrlSendSetScreenDanHeiIsStartGame", RPCMode.Server);
	}
	
	[RPC] void NetCtrlSendSetScreenDanHeiIsStartGame()
	{
		if (ScreenDanHeiCtrl.IsStartGame) {
			return;
		}
		Debug.Log("NetCtrlSendSetScreenDanHeiIsStartGame...");
		XkGameCtrl.ClearCartoonSpawnNpc();
		ScreenDanHeiCtrl.IsStartGame = true;
		Time.timeScale = 1.0f;
	}

	public void MakeOtherPortCloseCartoon()
	{
		if (IsMakeGameCloseCartoon) {
			return;
		}
		IsMakeGameCloseCartoon = true;
		NetworkViewCom.RPC("NetCtrlMakeOtherPortCloseCartoon", RPCMode.OthersBuffered);
	}
	
	[RPC] void NetCtrlMakeOtherPortCloseCartoon()
	{
		IsMakeGameCloseCartoon = true;
		XKTriggerEndCartoon.GetInstance().CloseStartCartoon();
	}

	public void MakeOtherPortStopJiFenTime()
	{
		if (IsMakeGameStopJiFenTime) {
			return;
		}
		IsMakeGameStopJiFenTime = true;
		Debug.Log("MakeOtherPortStopJiFenTime...");
		NetworkViewCom.RPC("NetCtrlMakeOtherPortStopJiFenTime", RPCMode.OthersBuffered);
	}
	
	[RPC] void NetCtrlMakeOtherPortStopJiFenTime()
	{
		if (IsMakeGameStopJiFenTime) {
			return;
		}
		Debug.Log("NetCtrlMakeOtherPortStopJiFenTime...");
		IsMakeGameStopJiFenTime = true;
		JiFenJieMianCtrl.GetInstance().StopJiFenTime();
	}

	public void MakeServerShowGameOver()
	{
		NetworkViewCom.RPC("NetCtrlMakeOtherClientShowFinishTask", RPCMode.Server, 1); //show gameover.
	}

	public void MakeOtherClientShowFinishTask(int key = 0)
	{
		if (IsMakeClientShowFinishTask) {
			return;
		}
		IsMakeClientShowFinishTask = true;
		NetworkViewCom.RPC("NetCtrlMakeOtherClientShowFinishTask", RPCMode.OthersBuffered, key);
	}
	
	[RPC] void NetCtrlMakeOtherClientShowFinishTask(int key = 0)
	{
		if (key == 1) {
			if (Network.peerType != NetworkPeerType.Server) {
				return;
			}
			else {
				CountGameOver++;
				if (CountGameOver < Network.connections.Length) {
					return;
				}
			}
		}

		IsMakeClientShowFinishTask = true;
		if (GameOverCtrl.IsShowGameOver) {
			return;
		}

		if (key == 1) {
			GameOverCtrl.IsShowGameOver = true;
		}
		XkGameCtrl.OnPlayerFinishTask();
	}

	public void ResetGameInfo()
	{
		IsMakeGameStopJiFenTime = false;
		IsMakeGameCloseCartoon = false;
		IsMakeClientShowFinishTask = false;
	}
	
	public void HandleHeTiPlayerEvent()
	{
		NetworkViewCom.RPC("NetCtrlSendHandleHeTiPlayerEvent", RPCMode.OthersBuffered);
	}

	[RPC] void NetCtrlSendHandleHeTiPlayerEvent()
	{
		XKTriggerOpenPlayerUI.HandleHeTiPlayerEvent();
	}

	public void HandleLoadingGamePlayerCount()
	{
		NetworkViewCom.RPC("NetCtrlSendHandleLoadingGamePlayerCount", RPCMode.Server);
	}
	
	[RPC] void NetCtrlSendHandleLoadingGamePlayerCount()
	{
		LoadingGameCtrl.AddLoadingPlayerCount();
	}

	public void HandleLoadingGameHiddenLoadingGame()
	{
		NetworkViewCom.RPC("NetCtrlSendHandleLoadingGameHiddenLoadingGame", RPCMode.OthersBuffered);
	}

	[RPC] void NetCtrlSendHandleLoadingGameHiddenLoadingGame()
	{
		LoadingGameCtrl.SetIsHiddenLoadingGame();
	}

	public void TryCloseServerPort()
	{
		NetworkViewCom.RPC("NetCtrlSendTryCloseServerPort", RPCMode.Server);
	}

	[RPC] void NetCtrlSendTryCloseServerPort()
	{
		if (NetworkServerNet.GetInstance() != null) {
			NetworkServerNet.GetInstance().TryToCloseServerPort(); //Close ServerNet
		}
	}

	public void TryActiveHeTiPlayerEvent()
	{
		NetworkViewCom.RPC("NetCtrlSendTryActiveHeTiPlayerEvent", RPCMode.OthersBuffered);
	}
	
	[RPC] void NetCtrlSendTryActiveHeTiPlayerEvent()
	{
		if (XKTriggerClosePlayerUI.GetInstance() != null) {
			XkPlayerCtrl playerScript = null;
			if (Network.peerType == NetworkPeerType.Server) {
				if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
					playerScript = XkPlayerCtrl.GetInstanceFeiJi();
				}
				else if (XkPlayerCtrl.GetInstanceTanKe() != null) {
					playerScript = XkPlayerCtrl.GetInstanceTanKe();
				}
			}
			else if (Network.peerType == NetworkPeerType.Client) {
				if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.FeiJiJiTai) {
					playerScript = XkPlayerCtrl.GetInstanceFeiJi();
				}
				else if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
					playerScript = XkPlayerCtrl.GetInstanceTanKe();
				}
			}
			XKTriggerClosePlayerUI.GetInstance().HandlePlayerOnTriggerEnter(playerScript);
		}
	}

	public void TryActiveKaQiuShaFire()
	{
		NetworkViewCom.RPC("NetCtrlSendTryActiveKaQiuShaFire", RPCMode.Server);
	}
	
	[RPC] void NetCtrlSendTryActiveKaQiuShaFire()
	{
		XKTriggerKaQiuShaFire.SetIsFireKaQiuSha();
	}

	public void TryActiveGameOverEvent()
	{
		NetworkViewCom.RPC("NetCtrlSendTryActiveGameOverEvent", RPCMode.OthersBuffered);
	}
	
	[RPC] void NetCtrlSendTryActiveGameOverEvent()
	{
		XkPlayerCtrl playerScript = null;
		if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.FeiJiJiTai) {
			playerScript = XkPlayerCtrl.GetInstanceFeiJi();
			if (playerScript == null) {
				playerScript = XkPlayerCtrl.GetInstanceTanKe();
			}
		}
		else if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
			playerScript = XkPlayerCtrl.GetInstanceTanKe();
			if (playerScript == null) {
				playerScript = XkPlayerCtrl.GetInstanceFeiJi();
			}
		}
		XKTriggerGameOver.GetInstance().SpawnPlayerDaoDan(playerScript);
	}
}