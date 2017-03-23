using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XKNpcMoveCtrl : MonoBehaviour {
	float SpawnTimeVal;
	public NpcType NpcState = NpcType.LandNpc;
	public NpcJiFenEnum NpcJiFen = NpcJiFenEnum.ShiBing; //控制npc的运动.
	public NpcJiFenEnum NpcMoveType = NpcJiFenEnum.ShiBing; //控制npc的运动.
	public Transform RealNpcTran;
	public GameObject DeathExplode;
	[Range(0f, 1f)]public float DoFire1 = 0.1f;
	[Range(0f, 1f)]public float DoFire2 = 0.2f;
	public bool IsZaiTiNpc;
	public bool IsFireMove;
	public bool IsAniMove; //用动画来控制运动.
	//CountHuanDan小于1时，不做换弹动画. 否则做换弹动画.
	[Range(-1, 100)] public int CountHuanDan = 10;
	[Range(0f, 100f)]public float RemoveNpcTime = 2f;
	public GameObject TestSpawnPoint;
	Rigidbody BuWaWaRigidbody;
	Animator AnimatorCom;
	GameObject NpcObj;
	Transform NpcTran;
	Transform NpcPathTran;
	float MvSpeed;
	int MarkCount;
	XKNpcAnimatorCtrl NpcAniScript;
	string CurrentRunAnimation;
	bool IsDeathNPC;
	bool IsChangeNpcForward;
	Vector3 ForwardValNpc;
	float FireDistance = 0f;
	bool IsDoFireAnimation;
	bool IsAimPlayer;
	XKSpawnNpcPoint SpawnPointScript;
	bool IsFangZhenNpc;
	XKNpcFangZhenCtrl NpcFangZhenScript;
	bool IsDoFireAniZaiTi;
	Animator[] ZaiTiNpcAni;
	XKNpcAnimatorCtrl[] ZaiTiNpcAniScript;
	Rigidbody[] ZaiTiNpcBuWaWa;
	iTween ITweenScriptNpc;
	bool IsWuDi;
	bool IsHuoCheNpc;
	float TimeMinFire = 0.1f;
	float TimeMaxFire = 2f;
	int ZaiTiNpcFireCount;
	bool IsAimPlayerByFire;
	bool IsDelayFireAction;
	Transform MarkTranAim;
	Vector3 MarkTranAimForward;
	NpcPathCtrl NpcPathScript;
	int IndexNpc;
	int IndexFirePointGroup;
//	bool IsIntoJingJieState;
	FirePoint FirePointScript;
	bool IsMoveFirePoint;
	NpcFireAction FireAnimation;
	Transform MarkNpcMove;
	bool IsMoveToMarkPoint;
	NetworkView NetViewCom;
	int RecordAimPlayerState = -1;
	bool IsHandleRpc;
	void Awake()
	{
		if (IsAniMove) {
			SetIsRemoveNpcObj();
		}

		SpawnTimeVal = Time.realtimeSinceStartup;
		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			if (TestSpawnPoint != null) {
				XKSpawnNpcPoint spawnPoint = TestSpawnPoint.GetComponent<XKSpawnNpcPoint>();
				if (spawnPoint != null) {
					SetSpawnNpcInfo(spawnPoint);
				}
			}
		}

		NpcMoveType = NpcJiFen;
		NetViewCom = GetComponent<NetworkView>();
		if (Network.peerType == NetworkPeerType.Disconnected) {
			NetViewCom.enabled = false;
		}
		InitNpcInfo();
		//XkGameCtrl.GetInstance().AddNpcTranToList(NpcTran);
		MakeLandNpcMoveToLand();
		Invoke("DelayChangeNpcParent", 0.2f);
		
		/*if (NpcState == NpcType.FlyNpc) {
			NpcJiFen = NpcJiFenEnum.FeiJi;
		}*/
	}

	void DelayChangeNpcParent()
	{
		if (!IsHuoCheNpc) {
			if (transform.parent == null) {
				transform.parent = XkGameCtrl.MissionCleanup;
			}
		}
	}

	void Update()
	{
		CheckNpcIsRemove();
		if (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo) {
			if (!XkGameCtrl.IsMoveOnPlayerDeath) {
				if (ITweenScriptNpc == null) {
					ITweenScriptNpc = GetComponent<iTween>();
				}
				
				if (ITweenScriptNpc != null && ITweenScriptNpc.isRunning) {
					ITweenScriptNpc.isRunning = false;
				}
				return;
			}
		}
		else {
			if (ITweenScriptNpc != null && !ITweenScriptNpc.isRunning) {
				ITweenScriptNpc.isRunning = true;
			}
		}

		if (IsDeathNPC) {
			return;
		}
		
		MakeLandNpcMoveToLand();
		if (!IsHandleRpc) {
			UpdateClientNpcTransformAimPlayer();
			return;
		}

		SendNpcTransformInfo();
		CheckMoveNpcOnCompelteITween();

		if (IsChangeNpcForward) {
			NpcTran.forward = Vector3.Lerp(NpcTran.forward, ForwardValNpc, 0.05f);
			if (Vector3.Distance(NpcTran.forward, ForwardValNpc) <= 0.02f) {
				NpcTran.forward = ForwardValNpc;
				IsChangeNpcForward = false;
			}
		}

		if (MarkTranAim != null) {
			if (NpcMoveType == NpcJiFenEnum.CheLiang || NpcMoveType == NpcJiFenEnum.ChuanBo) {
				RealNpcTran.forward = Vector3.Slerp(RealNpcTran.forward, MarkTranAimForward, 0.9f * Time.deltaTime);
			}
		}

		if (!IsDoFireAnimation && !IsFangZhenNpc && !IsDoFireAniZaiTi && NpcState != NpcType.FlyNpc) {
			Vector3 posA = NpcTran.position;
			Vector3 posB = Vector3.zero;
			switch (XkGameCtrl.GameModeVal) {
			case GameMode.DanJiFeiJi:
				posB = XkPlayerCtrl.PlayerTranFeiJi.position;
				break;

			case GameMode.DanJiTanKe:
				posB = XkPlayerCtrl.PlayerTranTanKe.position;
				break;

			case GameMode.LianJi:
				if (SpawnPointScript == null) {
					return;
				}

				if (XkPlayerCtrl.PlayerTranFeiJi != null || XkPlayerCtrl.PlayerTranTanKe != null) {
					if (SpawnPointScript.PointType == SpawnPointType.KongZhong) {
						if (XkPlayerCtrl.PlayerTranFeiJi != null) {
							posB = XkPlayerCtrl.PlayerTranFeiJi.position;
						}
						else {
							posB = XkPlayerCtrl.PlayerTranTanKe.position;
						}
					}
					else if (SpawnPointScript.PointType == SpawnPointType.DiMian) {
						if (XkPlayerCtrl.PlayerTranTanKe != null) {
							posB = XkPlayerCtrl.PlayerTranTanKe.position;
						}
						else {
							posB = XkPlayerCtrl.PlayerTranFeiJi.position;
						}
					}
					else if (SpawnPointScript.PointType == SpawnPointType.Null) {
						if (SpawnPointScript.IsAimFeiJiPlayer) {
							if (XkPlayerCtrl.PlayerTranFeiJi != null) {
								posB = XkPlayerCtrl.PlayerTranFeiJi.position;
							}
							else {
								posB = XkPlayerCtrl.PlayerTranTanKe.position;
							}
						}
						else {
							if (XkPlayerCtrl.PlayerTranTanKe != null) {
								posB = XkPlayerCtrl.PlayerTranTanKe.position;
							}
							else {
								posB = XkPlayerCtrl.PlayerTranFeiJi.position;
							}
						}
					}
				}
				else {
					return;
				}
				break;
			}

			posA.y = posB.y = 0f;
			if (Vector3.Distance(posA, posB) <= FireDistance) {
				MakeNpcDoFireAnimation();
			}
		}

		if (IsAimPlayer && (IsDoFireAnimation && !IsFireMove && !IsMoveFirePoint)) {
			CheckNpcAimPlayer();
			SetNpcIsAimPlayer(1);
		}
		else {
			SetNpcIsAimPlayer(0);
		}
	}

	void UpdateClientNpcTransformAimPlayer()
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			return;
		}
		
		if (IsHandleRpc) {
			return;
		}

		if (!IsAimPlayer) {
			return;
		}

		CheckNpcAimPlayer();
	}

	void CheckNpcAimPlayer()
	{
		Vector3 pos = Vector3.zero;
		if (XkPlayerCtrl.PlayerTranFeiJi != null || XkPlayerCtrl.PlayerTranTanKe != null) {
			switch (NpcAimPlayerState) {
			case 0:
			case 2:
				if (XkPlayerCtrl.PlayerTranFeiJi != null) {
					pos = XkPlayerCtrl.PlayerTranFeiJi.position;
				}
				else {
					pos = XkPlayerCtrl.PlayerTranTanKe.position;
				}
				break;
				
			default:
				if (XkPlayerCtrl.PlayerTranTanKe != null) {
					pos = XkPlayerCtrl.PlayerTranTanKe.position;
				}
				else {
					pos = XkPlayerCtrl.PlayerTranFeiJi.position;
				}
				break;
			}
		}
		else {
			return;
		}
		
		pos.y = RealNpcTran.position.y;
		RealNpcTran.LookAt(pos);
	}

	/********************************************************************
	 * NpcAimPlayerState == 0; -> SpawnPointScript.PointType == SpawnPointType.KongZhong
	 * NpcAimPlayerState == 1; -> SpawnPointScript.PointType == SpawnPointType.DiMian
	 * NpcAimPlayerState == 2; -> SpawnPointScript.PointType == SpawnPointType.Null && SpawnPointScript.IsAimFeiJiPlayer == true
	 * NpcAimPlayerState == -1; -> SpawnPointScript.PointType == SpawnPointType.Null && SpawnPointScript.IsAimFeiJiPlayer == false
	 ********************************************************************/
	int NpcAimPlayerState = -1;
	void SetNpcAimPlayerState()
	{
		if (NetViewCom == null) {
			NetViewCom = GetComponent<NetworkView>();
		}

		if (SpawnPointScript.PointType == SpawnPointType.KongZhong) {
			NpcAimPlayerState = 0;
		}
		else if (SpawnPointScript.PointType == SpawnPointType.DiMian) {
			NpcAimPlayerState = 1;
		}
		else {
			if (SpawnPointScript.IsAimFeiJiPlayer) {
				NpcAimPlayerState = 2;
			}
			else {
				return;
			}
		}
		
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		
		if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0) {
			return;
		}
		
		if (!IsHandleRpc) {
			return;
		}
		NetViewCom.RPC("XKNpcSendSetNpcAimPlayerState", RPCMode.OthersBuffered, NpcAimPlayerState);
	}

	[RPC] void XKNpcSendSetNpcAimPlayerState(int val)
	{
		if (val > 2 || val < 0) {
			return;
		}
		NpcAimPlayerState = val;
	}

	void SetNpcIsAimPlayer(int val)
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		
		if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0) {
			return;
		}
		
		if (!IsHandleRpc || !ScreenDanHeiCtrl.IsStartGame) {
			return;
		}

		if (RecordAimPlayerState == val) {
			return;
		}
		RecordAimPlayerState = val;
		NetViewCom.RPC("XKNpcSendSetNpcIsAimPlayer", RPCMode.OthersBuffered, val);
	}

	[RPC] void XKNpcSendSetNpcIsAimPlayer(int val)
	{
		bool isAim = val == 1 ? true : false;
		if (isAim == false && isAim != IsAimPlayer) {
			if (RealNpcTran.localEulerAngles != Vector3.zero) {
				RealNpcTran.localEulerAngles = Vector3.zero;
			}
		}
		IsAimPlayer = isAim;
	}

	void SendNpcTransformInfo()
	{
		if (XkGameCtrl.GameModeVal != GameMode.LianJi || !GameMovieCtrl.IsActivePlayer) {
			return;
		}

		if (Network.peerType != NetworkPeerType.Server) {
			IsHandleRpc = false;
			return;
		}
		
		if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0) {
			return;
		}

		if (!IsHandleRpc) {
			return;
		}

		if (IsHuoCheNpc) {
			return;
		}
		NetViewCom.RPC("XKNpcSendOtherTranformInfo", RPCMode.OthersBuffered, transform.position, transform.rotation);

		if (IsZaiTiNpc) {
			NetViewCom.RPC("XKRealNpcSendOtherTranformInfo", RPCMode.OthersBuffered, RealNpcTran.rotation);
		}
	}

	[RPC] void XKNpcSendOtherTranformInfo(Vector3 pos, Quaternion rot)
	{
		if (IsHuoCheNpc) {
			return;
		}
		transform.position = pos;
		transform.rotation = rot;
		MakeLandNpcMoveToLand();
	}
	
	[RPC] void XKRealNpcSendOtherTranformInfo(Quaternion rot)
	{
		RealNpcTran.rotation = rot;
	}

	public FirePoint GetFirePointScript()
	{
		return FirePointScript;
	}
	
	public void SetIndexNpc(int val)
	{
		IndexNpc = val;
		SetFirePointScript();
	}

	void SetFirePointScript()
	{
		if (SpawnPointScript.FirePointGroup.Length < 1) {
			FirePointScript = null;
			IndexFirePointGroup = 0;
			return;
		}

		if (SpawnPointScript.FirePointGroup.Length <= IndexFirePointGroup) {
			if (!SpawnPointScript.IsLoopFirePoint || SpawnPointScript.FirePointGroup.Length == 1) {
				FirePointScript = null;
				return;
			}
			else {
				IndexFirePointGroup = 0;
			}
		}
//		Debug.Log("SetFirePointScript...");

		FirePointCtrl pointCtrl = SpawnPointScript.FirePointGroup[IndexFirePointGroup];
		Transform pointCtrlTran = pointCtrl.transform;
//		Debug.Log("IndexNpc "+IndexNpc+", childCount "+pointCtrlTran.childCount);
		if (IndexNpc < pointCtrlTran.childCount) {
			FirePointScript = pointCtrlTran.GetChild(IndexNpc).GetComponent<FirePoint>();
		}
		else {
			int rv = Random.Range(0, pointCtrlTran.childCount);
			FirePointScript = pointCtrlTran.GetChild(rv).GetComponent<FirePoint>();
		}
	}

	public void MakeNpcMoveFirePoint()
	{
		if (IsDeathNPC || FirePointScript == null) {
			return;
		}

		if (IsMoveFirePoint) {
			return;
		}
		IsMoveFirePoint = true;
		IsChangeNpcForward = false;
		RealNpcTran.localEulerAngles = Vector3.zero;

		FireAnimation = FirePointScript.AniFireName;
		float mvSpeed = SpawnPointScript.SpeedFangZhenFireRun > 0f ? SpawnPointScript.SpeedFangZhenFireRun : 1f;
		Vector3 firePos = FirePointScript.transform.position;
		Vector3[] posArray = new Vector3[2];
		posArray[0] = NpcTran.position;
		posArray[1] = firePos;
		iTween itweenScript = GetComponent<iTween>();
		if (itweenScript != null) {
			Destroy(itweenScript);
		}
		
		iTween.MoveTo(NpcObj, iTween.Hash("path", posArray,
		                                  "speed", mvSpeed,
		                                  "orienttopath", true,
		                                  "easeType", iTween.EaseType.linear,
		                                  "oncomplete", "OnCompelteMakeNpcMoveFirePoint"));

		IndexFirePointGroup++;
		SetFirePointScript();
	}

	void OnCompelteMakeNpcMoveFirePoint()
	{
		IsMoveFirePoint = false;
		SetIsDoFireAnimation(true);
		RandomPlayFireAction(NpcAniScript, false); //play fire action
	}

	public void ActiveIsFangZhenNpc()
	{
		IsFangZhenNpc = true;
	}

	public void SetIsFangZhenNpc(XKNpcFangZhenCtrl fangZhenScript, int index)
	{
		IsFangZhenNpc = true;
		NpcFangZhenScript = fangZhenScript;
		XkNpcZaiTiCtrl script = GetComponentInChildren<XkNpcZaiTiCtrl>();
		if (script != null) {
			script.SetFangZhenZaiTiNpcAni(index);
		}
	}

	public void SetNpcForwardVal(Vector3 val)
	{
		if (IsDeathNPC) {
			return;
		}
		IsChangeNpcForward = true;
		ForwardValNpc = val;
	}

	bool IsInitNpcInfo;
	void InitNpcInfo()
	{
		if (IsInitNpcInfo) {
			return;
		}
		IsInitNpcInfo = true;

		if (IsDeathNPC) {
			NpcObj = null;
			IsDeathNPC = false;
			RealNpcTran.gameObject.SetActive(true);
		}

		if (NpcObj != null) {
			return;
		}
		NpcObj = gameObject;
		NpcTran = transform;

		NpcAniScript = RealNpcTran.gameObject.GetComponent<XKNpcAnimatorCtrl>();
		if (NpcAniScript == null) {
			NpcAniScript = RealNpcTran.gameObject.AddComponent<XKNpcAnimatorCtrl>();
		}
		NpcAniScript.SetAmmoPrefabVal(this);

		AnimatorCom = RealNpcTran.GetComponent<Animator>();
		if (!IsAniMove) {
			AnimatorCom.enabled = true;
		}
		BoxCollider boxCol = NpcObj.GetComponent<BoxCollider>();
		if (boxCol == null) {
			boxCol = NpcObj.AddComponent<BoxCollider>();
			boxCol.center = new Vector3(0f, 0.5f, 0f);
			boxCol.size = new Vector3(0.2f, 0.2f, 0.2f);
		}

		if (IsZaiTiNpc) {
			InitZaiTiNpcInfo();
			int max = ZaiTiNpcAni.Length;
			ZaiTiNpcAniScript = new XKNpcAnimatorCtrl[max];
			for (int i = 0; i < max; i++) {
				if (XkGameCtrl.GetInstance().IsCartoonShootTest && ZaiTiNpcAni[i] == null) {
					continue;
				}

				if (ZaiTiNpcAni[i].gameObject.GetComponent<XKNpcAnimatorCtrl>() != null) {
					//Debug.LogWarning("XKNpcAnimatorCtrl is not null, name "+gameObject.name);
					continue;
				}
				ZaiTiNpcAniScript[i] = ZaiTiNpcAni[i].gameObject.AddComponent<XKNpcAnimatorCtrl>();
				ZaiTiNpcAniScript[i].SetAmmoPrefabVal(this);
			}

			XKNpcHealthCtrl healthScript = AnimatorCom.GetComponent<XKNpcHealthCtrl>();
			if (healthScript != null) {
				healthScript.SetNpcMoveScript(this);
			}
			else {
				healthScript = GetComponentInChildren<XKNpcHealthCtrl>();
				if (healthScript != null) {
					healthScript.SetNpcMoveScript(this);
				}
			}
		}
		else {
			XKNpcHealthCtrl healthScript = GetComponentInChildren<XKNpcHealthCtrl>();
			if (healthScript != null) {
				healthScript.SetNpcMoveScript(this);
			}
			
			XkNpcZaiTiCtrl zaiTiScript = GetComponentInChildren<XkNpcZaiTiCtrl>();
			BuWaWaRigidbody = zaiTiScript.ZaiTiNpcBuWaWa;
		}
	}

	public void SetNpcSpawnScriptInfo(XKSpawnNpcPoint spawnScript)
	{
		if (IsDeathNPC) {
			IsDeathNPC = false;
			if (AnimatorCom == null) {
				AnimatorCom = RealNpcTran.GetComponent<Animator>();
			}
			AnimatorCom.enabled = true;
			RealNpcTran.gameObject.SetActive(true);

			XKNpcHealthCtrl healthScript = AnimatorCom.GetComponent<XKNpcHealthCtrl>();
			if (healthScript != null) {
				healthScript.SetNpcMoveScript(this);
			}
		}

		IsHandleRpc = true;
		SpawnPointScript = spawnScript;
		SetNpcAimPlayerState();

		IsAimPlayer = SpawnPointScript.IsAimPlayer;
		if (!IsDoFireAnimation) {
			if ( (NpcPathTran == null && (int)SpawnPointScript.AniName < (int)AnimatorNameNPC.Run1
			      && (int)SpawnPointScript.AniName > (int)AnimatorNameNPC.Run3)
			    || NpcPathTran != null )
			{
				PlayNpcAnimation(SpawnPointScript.AniName);
			}
		}
		NpcPathTran = SpawnPointScript.NpcPath;
		MvSpeed = SpawnPointScript.MvSpeed;
		FireDistance = SpawnPointScript.FireDistance;
		
		TimeMinFire = SpawnPointScript.TimeMinFire;
		TimeMaxFire = SpawnPointScript.TimeMaxFire;
		SetNpcFireAnimationIsAimFeiJiPlayer(spawnScript.IsAimFeiJiPlayer);

		SetFirePointScript();
	}
	
	void SetNpcFireAnimationIsAimFeiJiPlayer(bool isAim)
	{
		SetNpcIsAimFeiJiPlayer(isAim);
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		NetViewCom.RPC("XKNpcSendFireAnimationIsAimFeiJiPlayer", RPCMode.OthersBuffered,
		               isAim == true ? 1 : 0);
	}
	
	void SetNpcIsAimFeiJiPlayer(bool isAim)
	{
		XKNpcAnimatorCtrl[] npcAniScript = GetComponentsInChildren<XKNpcAnimatorCtrl>();
		for (int i = 0; i < npcAniScript.Length; i++) {
			npcAniScript[i].SetIsAimFeiJiPlayer(isAim);
		}

		XkNpcZaiTiCtrl[] zaiTiScript = GetComponentsInChildren<XkNpcZaiTiCtrl>();
		for (int i = 0; i < zaiTiScript.Length; i++) {
			zaiTiScript[i].SetIsAimFeiJiPlayer(isAim);
		}
	}
	
	[RPC] void XKNpcSendFireAnimationIsAimFeiJiPlayer(int val)
	{
		if (IsDeathNPC) {
			IsDeathNPC = false;
			XKNpcHealthCtrl healthScript = AnimatorCom.GetComponent<XKNpcHealthCtrl>();
			if (healthScript != null) {
				healthScript.SetNpcMoveScript(this);
			}
		}

		if (RealNpcTran != null) {
			RealNpcTran.gameObject.SetActive(true);
		}

		if (AnimatorCom == null) {
			AnimatorCom = RealNpcTran.GetComponent<Animator>();
		}

		if (AnimatorCom != null) {
			AnimatorCom.enabled = true;
		}

		bool isAim = val == 1 ? true : false;
		SetNpcIsAimFeiJiPlayer(isAim);
	}

	XKCannonCtrl[] CannonScript;
	public void SetSpawnNpcInfo(XKSpawnNpcPoint spawnScript)
	{
		SendNpcTransformInfo();
		SetNpcSpawnScriptInfo(spawnScript);
		IsHuoCheNpc = spawnScript.GetIsHuoCheNpc();
		TestSpawnPoint = spawnScript.gameObject;
		if (spawnScript.NpcPath != null) {
			NpcPathScript = spawnScript.NpcPath.GetComponent<NpcPathCtrl>();
		}

		if (CannonScript == null || CannonScript.Length < 1) {
			CannonScript = GetComponentsInChildren<XKCannonCtrl>();
		}
		if (CannonScript.Length > 0) {
			int max = CannonScript.Length;
			//Debug.Log("SetSpawnNpcInfo -> max "+max);
			for (int i = 0; i < max; i++) {
				CannonScript[i].SetSpawnPointScript(null);
			}
			SetCannonAimPlayerState();
		}

		InitNpcInfo();
		if (SpawnPointScript.TimeRootAni > 0f) {
			if (!IsDoFireAnimation) {
					PlayNpcAnimation(SpawnPointScript.AniRootName);
			}
		}

		if (NpcPathTran != null && !IsAniMove) {
			StartCoroutine(StartMoveNpcByItween(SpawnPointScript.AniName, SpawnPointScript.TimeRootAni));
		}

		if (IsAniMove) {
			Animator aniCom = GetComponent<Animator>();
			if (aniCom == null) {
				gameObject.AddComponent<Animator>();
				CancelInvoke("DelayCheckNpcAniController");
				Invoke("DelayCheckNpcAniController", 0.1f);
			}
			else {
				DelayCheckNpcAniController();
			}

			SendFeiJiNpcPointIndex(spawnScript);
		}
	}

	void SendFeiJiNpcPointIndex(XKSpawnNpcPoint spawnScript)
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		
		if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0) {
			return;
		}
		int indexVal = spawnScript.GetIndexFeiJiPoint();
		NetViewCom.RPC("XKNpcSendFeiJiNpcPointIndex", RPCMode.OthersBuffered, indexVal);
	}

	[RPC] void XKNpcSendFeiJiNpcPointIndex(int indexVal)
	{
		//Debug.Log("XKNpcSendFeiJiNpcPointIndex -> indexVal "+indexVal);
		XKSpawnNpcPoint.HandleFeiJiNpcSpawnInfo(this, indexVal);
	}
	
	void SetCannonAimPlayerState()
	{
		int aimState = -1;
		if (SpawnPointScript.PointType == SpawnPointType.KongZhong) {
			aimState = 0;
		}
		else if (SpawnPointScript.PointType == SpawnPointType.DiMian) {
			aimState = 1;
		}
		else {
			if (SpawnPointScript.IsAimFeiJiPlayer) {
				aimState = 2;
			}
		}
		SetCannonNpcInfo(aimState, SpawnPointScript.FireDistance);
		
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		
		if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0) {
			return;
		}
		
		if (NetViewCom == null) {
			NetViewCom = GetComponent<NetworkView>();
		}
		NetViewCom.RPC("XKCannonSendSetNpcAimPlayerState", RPCMode.OthersBuffered, aimState, SpawnPointScript.FireDistance);
	}
	
	[RPC] void XKCannonSendSetNpcAimPlayerState(int valAim, float valFireDis)
	{
		//Debug.Log("XKCannonSendSetNpcAimPlayerState.............");
		SetCannonNpcInfo(valAim, valFireDis);
	}
	
	void SetCannonNpcInfo(int valAim, float valFireDis)
	{
		if (CannonScript == null || CannonScript.Length < 1) {
			CannonScript = GetComponentsInChildren<XKCannonCtrl>();
		}

		int max = CannonScript.Length;
		for (int i = 0; i < max; i++) {
			CannonScript[i].SetCannonSpawnPointInfo(valAim, valFireDis);
		}
	}

	void DelayCheckNpcAniController()
	{
		Animator aniCom = GetComponent<Animator>();
		aniCom.runtimeAnimatorController = SpawnPointScript.AniController;
		aniCom.enabled = true;
	}
	
	IEnumerator StartMoveNpcByItween(AnimatorNameNPC aniVal, float rootTime)
	{
		yield return new WaitForSeconds(rootTime);
		if (IsDeathNPC || (IsDoFireAnimation && !IsFireMove)) {
			yield break;
		}
		
		AnimatorNameNPC runAni = AnimatorNameNPC.Run1;
		if (aniVal == AnimatorNameNPC.Run1 || aniVal == AnimatorNameNPC.Run2 || aniVal == AnimatorNameNPC.Run3) {
			runAni = aniVal;
		}
		else {
			runAni = XKNpcFangZhenCtrl.GetRandRunAniName();
		}

		if (!IsDoFireAnimation) {
			PlayNpcAnimation(runAni);
		}
		MoveNpcByItween();
		yield break;
	}

	public void PlayNpcAnimation(AnimatorNameNPC aniVal)
	{
		if (IsZaiTiNpc || IsDeathNPC) {
			return;
		}

		if (aniVal != AnimatorNameNPC.Null) {
			if (aniVal == AnimatorNameNPC.Run1 || aniVal == AnimatorNameNPC.Run2
			    || aniVal == AnimatorNameNPC.Run3) {
				CurrentRunAnimation = aniVal.ToString();
			}
			NetNpcPlayAnimation(NpcAniScript, aniVal.ToString());
		}
	}

	void MoveNpcByItween()
	{
		if (IsDeathNPC) {
			return;
		}

		if (NpcPathTran == null) {
			//Debug.Log("The npc has no path! name "+gameObject.name);
			return;
		}

		if (NpcState == NpcType.FlyNpc) {
			List<Transform> tranList = new List<Transform>(NpcPathTran.GetComponentsInChildren<Transform>()){};
			tranList.Remove(NpcPathTran);

			NpcMark markScript = tranList[0].GetComponent<NpcMark>();
			if (markScript != null) {
				SetIsDoFireAnimation(markScript.IsFireFeiJiNpc);
			}
			MarkTranAim = tranList[0];
			MarkNpcMove = MarkTranAim;
			MarkCount++;
			iTween.MoveTo(NpcObj, iTween.Hash("path", tranList.ToArray(),
			                                  "speed", MvSpeed,
			                                  "orienttopath", true,
			                                  "easeType", iTween.EaseType.linear));
			return;
		}

		Transform[] tranArray = new Transform[2];
		tranArray[0] = NpcTran;
		if (MarkCount >= NpcPathTran.childCount || MarkCount < 0) {
			MarkCount = 0; //fixed MarkCount
		}
		tranArray[1] = NpcPathTran.GetChild(MarkCount);

		MarkCount++;
		if (IsHuoCheNpc) {
			StartCoroutine(MoveNpcByLocalPos(tranArray[1], MvSpeed));
		}
		else {
			MarkNpcMove = tranArray[1];
			IsMoveToMarkPoint = false;

			bool isOrienttopath = true;
//			if (NpcJiFen == NpcJiFenEnum.FeiJi  || NpcJiFen == NpcJiFenEnum.ChuanBo
			if (NpcMoveType == NpcJiFenEnum.ChuanBo || NpcMoveType == NpcJiFenEnum.CheLiang) {
				isOrienttopath = false;
				MarkTranAim = tranArray[1];
				MarkTranAimForward = MarkTranAim.position - NpcTran.position;
			}

			iTween.MoveTo(NpcObj, iTween.Hash("path", tranArray,
			                                  "speed", MvSpeed,
			                                  "orienttopath", isOrienttopath,
			                                  "easeType", iTween.EaseType.linear));
//			                                  "oncomplete", "MoveNpcOnCompelteITween"));

//			if (NpcJiFen == NpcJiFenEnum.FeiJi) {
//				IsSetFeiJiMoveInfo = false;
//				Invoke("SetFeiJiMoveInfo", 0.1f);
//			}
		}
	}

	void CheckMoveNpcOnCompelteITween()
	{
		if (IsAniMove) {
			return;
		}

		if (IsMoveToMarkPoint) {
			return;
		}

		if (MarkNpcMove == null) {
			return;
		}

		float dis = Vector3.Distance(NpcObj.transform.position, MarkNpcMove.position);
		if (dis > (MvSpeed * Time.deltaTime)) {
			return;
		}
//		Debug.Log("CheckMoveNpcOnCompelteITween -> npc has moved to markPoint");
		IsMoveToMarkPoint = true;

		if (NpcState == NpcType.FlyNpc) {
			if (MarkCount >= NpcPathTran.childCount) {
				return;
			}
//			Debug.Log("MarkCount***"+MarkCount);

			MarkTranAim = NpcPathTran.GetChild(MarkCount);
			NpcMark markScript = MarkTranAim.GetComponent<NpcMark>();
			if (markScript != null) {
				SetIsDoFireAnimation(markScript.IsFireFeiJiNpc);
			}
			MarkNpcMove = MarkTranAim;
			MarkCount++;
			IsMoveToMarkPoint = false;
			return;
		}
		MoveNpcOnCompelteITween();
	}

	IEnumerator MoveNpcByLocalPos(Transform endTran, float speed)
	{
		bool isMoveNpc = true;
		float timeVal = 0.03f;
		float speedVal = speed * timeVal;
		Vector3 endPos = NpcTran.position;
		Vector3 startPos = endTran.position;
		endPos.y = startPos.y = 0f;
		Vector3 forwardVal = Vector3.Normalize(endPos - startPos);
		Vector3 vecSpeed = forwardVal * speedVal;
		do {
			if (!IsHuoCheNpc || IsDeathNPC) {
//				Debug.LogWarning("MoveNpcByLocalPos -> IsHuoCheNpc "+IsHuoCheNpc
//				          +", IsDeathNPC "+IsDeathNPC+", npcName "+NpcObj.name);
				yield break;
			}

			endPos = NpcTran.position;
			startPos = endTran.position;
			if (Vector3.Distance(endPos, startPos) > speedVal) {
				endPos.y = startPos.y = 0f;
				forwardVal = Vector3.Normalize(endPos - startPos);
				vecSpeed = forwardVal * speedVal;
			}

			float spLX = vecSpeed.z;
			float spLY = vecSpeed.x;
			float spLZ = vecSpeed.y;
			vecSpeed.x = spLY;
			vecSpeed.y = spLZ;
			vecSpeed.z = spLX;
			NpcTran.localPosition += vecSpeed;
			NpcTran.right = Vector3.Lerp(NpcTran.forward, forwardVal, 0.5f);
			if (Vector3.Distance(endPos, startPos) < 2f*speedVal) {
				isMoveNpc = false;
				break;
			}
			yield return new WaitForSeconds(0.03f);
		} while (isMoveNpc);

		OnFinishedMoveNpcByLocalPos();
	}

	void OnFinishedMoveNpcByLocalPos()
	{
		if (MarkCount >= NpcPathTran.childCount) {
			if (NpcPathScript != null && NpcPathScript.IsMoveEndFire) {
				MakeNpcDoFireAnimation(); //play fire animation
			}
			else {
				MakeNpcPlayRootAnimation();
			}
			return;
		}

		if (IsDoFireAnimation && !IsFireMove) {
			return;
		}
		Transform markTran = NpcPathTran.GetChild(MarkCount - 1);
		NpcMark markScript = markTran.GetComponent<NpcMark>();
		MvSpeed = markScript.MvSpeed;
		IsWuDi = markScript.IsWuDi;
		/*if (!IsDoFireAnimation) {
			PlayNpcAnimation(markScript.AniName);
		}*/
		MoveNpcByItween();
		//Debug.Log("MoveNpcOnCompelteITween...npc is "+NpcObj.name);
//		if (markScript.AnimatorTime > 0f && markScript.AniName != AnimatorNameNPC.Null) {
//			Invoke("DelayMoveNpcWaitAnimationEnd", markScript.AnimatorTime);
//		}
//		else {
//			MoveNpcByItween();
//		}
	}

	void InitZaiTiNpcInfo()
	{
		XkNpcZaiTiCtrl[] zaiTiScript = GetComponentsInChildren<XkNpcZaiTiCtrl>();
		int max = zaiTiScript.Length;
		List<Animator> zaiTiAniList = new List<Animator>(max);
		List<Rigidbody> zaiTiRigList = new List<Rigidbody>(max);
		for (int i = 0; i < zaiTiScript.Length; i++) {
			zaiTiScript[i].SetIsZaiNpc();
			zaiTiAniList.Add(zaiTiScript[i].ZaiTiNpcAni);
			zaiTiRigList.Add(zaiTiScript[i].ZaiTiNpcBuWaWa);
		}
		ZaiTiNpcAni = zaiTiAniList.ToArray();
		ZaiTiNpcBuWaWa = zaiTiRigList.ToArray();
	}

	public bool GetIsAimPlayerByFire()
	{
		return IsAimPlayerByFire;
	}

	void SetIsAimPlayerByFire(bool isAim)
	{
		IsAimPlayerByFire = isAim;
		if (!isAim) {
			//Play npc fire action
			
			if (!IsZaiTiNpc) {
				NpcAniScript.ResetFireAnimationSpeed();
			}
			else {
//				Debug.Log("ZaiTiNpcFireCount *** "+ZaiTiNpcFireCount);
				if (ZaiTiNpcAniScript[ZaiTiNpcFireCount] == null) {
					return;
				}
				ZaiTiNpcAniScript[ZaiTiNpcFireCount].ResetFireAnimationSpeed();
				ZaiTiNpcFireCount++;
			}
		}
	}

	IEnumerator DelayResetIsAimPlayerByFire(float timeVal)
	{
		yield return new WaitForSeconds(timeVal);
		SetIsAimPlayerByFire(false);
//		Debug.Log("DelayResetIsAimPlayerByFire***name "+gameObject.name);
	}

	void DelayPlayFireAction()
	{
		if (IsDelayFireAction) {
			return;
		}

		if (!IsZaiTiNpc) {
			IsDelayFireAction = true;
		}
//		Debug.Log("DelayPlayFireAction***name "+gameObject.name);
		float rv = Random.Range(TimeMinFire, TimeMaxFire);
		SetIsAimPlayerByFire(true);
		StartCoroutine(DelayResetIsAimPlayerByFire(rv));
	}

	public void MakeNpcDoFireAnimation()
	{
		//Debug.Log("MakeNpcDoFireAnimation****");
		if (IsDeathNPC) {
			return;
		}

		if (!IsZaiTiNpc) {
			if (!IsFireMove) {
				iTween itweenScript = GetComponent<iTween>();
				if (itweenScript != null) {
					itweenScript.isRunning = false;
					itweenScript.isPaused = true;
					itweenScript.enabled = false;
					Destroy(itweenScript);
				}
			}
		}
		else {
			if (IsDoFireAniZaiTi) {
				return;
			}
			//Debug.Log("ZaiTiNpc do fire...");
			IsDoFireAniZaiTi = true;
			ZaiTiNpcFireCount = 0;
			int max = ZaiTiNpcAni.Length;
			for (int i = 0; i < max; i++) {
				if (ZaiTiNpcAniScript[i] == null) {
					continue;
				}

				RandomPlayFireAction(ZaiTiNpcAniScript[i]);
				DelayPlayFireAction();
				ZaiTiNpcAniScript[i].SetCountHuanDan(CountHuanDan);
			}
			return;
		}
		
		SetIsDoFireAnimation(true);
		RandomPlayFireAction(NpcAniScript);
		DelayPlayFireAction();
		NpcAniScript.SetCountHuanDan(CountHuanDan);
	}

	public void SetIsDoFireAnimation(bool isDoFire)
	{
		if (isDoFire == IsDoFireAnimation) {
			return;
		}
		IsDoFireAnimation = isDoFire;
		SetClientNpcIsDoFireAnimation(isDoFire);
	}

	void RandomPlayFireAction(XKNpcAnimatorCtrl aniScript, bool isRandom = true)
	{
		float randVal = Random.Range(0f, 1f);
		SpawnPointType type = SpawnPointScript.PointType;
		bool isAimTanKeFire = false;
		bool isAimFeiJiFire = false;
		switch (type) {
		case SpawnPointType.DiMian:
			isAimTanKeFire = true;
			isAimFeiJiFire = false;
			break;

		case SpawnPointType.KongZhong:
			isAimTanKeFire = false;
			isAimFeiJiFire = true;
			break;

		case SpawnPointType.Null:
			if (XkGameCtrl.GameModeVal == GameMode.LianJi) {
				if (SpawnPointScript.IsAimFeiJiPlayer) {
					isAimTanKeFire = false;
					isAimFeiJiFire = true;
				}
				else {
					isAimTanKeFire = true;
					isAimFeiJiFire = false;
				}
			}
			else {
				if (XkGameCtrl.GameModeVal == GameMode.DanJiFeiJi) {
					isAimTanKeFire = false;
					isAimFeiJiFire = true;
				}
				else if (XkGameCtrl.GameModeVal == GameMode.DanJiTanKe) {
					isAimTanKeFire = true;
					isAimFeiJiFire = false;
				}
			}
			break;
		}

		AnimatorNameNPC aniName = AnimatorNameNPC.Null;
		aniScript.ResetIsDoRunFireAction();
		if (isAimFeiJiFire) {
			if (!isRandom) {
				switch (FireAnimation) {
				case NpcFireAction.Fire1_4:
					aniName = AnimatorNameNPC.Fire1;
					break;

				case NpcFireAction.Fire2_5:
					aniName = AnimatorNameNPC.Fire2;
					break;

				case NpcFireAction.Fire3_6:
					aniName = AnimatorNameNPC.Fire3;
					break;
				}
			}
			else {
				if (randVal < DoFire1) {
					aniName = AnimatorNameNPC.Fire1;
				}
				else if (randVal >= DoFire1 && randVal < (DoFire1 + DoFire2)) {
					aniName = AnimatorNameNPC.Fire2;
				}
				else {
					aniName = AnimatorNameNPC.Fire3;
				}
			}
		}

		if (isAimTanKeFire) {
			if (!isRandom) {
				switch (FireAnimation) {
				case NpcFireAction.Fire1_4:
					aniName = AnimatorNameNPC.Fire4;
					break;
					
				case NpcFireAction.Fire2_5:
					aniName = AnimatorNameNPC.Fire5;
					break;
					
				case NpcFireAction.Fire3_6:
					aniName = AnimatorNameNPC.Fire6;
					break;
				}
			}
			else {
				if (randVal < DoFire1) {
					aniName = AnimatorNameNPC.Fire4;
				}
				else if (randVal >= DoFire1 && randVal < (DoFire1 + DoFire2)) {
					aniName = AnimatorNameNPC.Fire5;
				}
				else {
					aniName = AnimatorNameNPC.Fire6;
				}
			}
		}
		NetNpcPlayAnimation(aniScript, aniName.ToString());
	}

	public bool GetIsDoFireAnimation()
	{
		return IsDoFireAnimation;
	}

	void SetClientNpcIsDoFireAnimation(bool isDoFire)
	{
		if (Network.peerType != NetworkPeerType.Server
		    || Network.connections.Length <= 0
		    || NetworkServerNet.ServerSendState != 0) {
			return;
		}
		NetViewCom.RPC("XKNpcSetClientNpcIsDoFireAnimation", RPCMode.OthersBuffered,
		               isDoFire == true ? 1 : 0);
	}

	[RPC] void XKNpcSetClientNpcIsDoFireAnimation(int val)
	{
		IsDoFireAnimation = (val == 1 ? true : false);
	}

	public bool GetIsWuDi()
	{
		return IsWuDi;
	}
	
	void MakeNpcPlayRootAnimation()
	{
		PlayNpcAnimation(AnimatorNameNPC.Root1);
	}

	void MoveNpcOnCompelteITween()
	{
		if (MarkCount <= 0) {
			return;
		}

		if (MarkCount >= NpcPathTran.childCount) {
			if (NpcPathScript != null && NpcPathScript.IsMoveEndFire) {
				MakeNpcDoFireAnimation(); //play fire animation
			}
			else {
				MakeNpcPlayRootAnimation();
			}
			return;
		}

		if (IsDoFireAnimation && !IsFireMove) {
			return;
		}

		Transform markTran = NpcPathTran.GetChild(MarkCount - 1);
		NpcMark markScript = markTran.GetComponent<NpcMark>();
		MvSpeed = markScript.MvSpeed;
		IsWuDi = markScript.IsWuDi;
		if (!IsDoFireAnimation) {
			PlayNpcAnimation(markScript.AniName);
		}

		//Debug.Log("MoveNpcOnCompelteITween...npc is "+NpcObj.name);
		if (markScript.AnimatorTime > 0f && markScript.AniName != AnimatorNameNPC.Null) {
			Invoke("DelayMoveNpcWaitAnimationEnd", markScript.AnimatorTime);
		}
		else {
			MoveNpcByItween();
		}
	}

	public void SetFeiJiMarkInfo(NpcMark script)
	{
		IsWuDi = script.IsWuDi;
//		if (IsWuDi) {
//			Debug.Log("SetFeiJiMarkInfo ********* IsWuDi "+IsWuDi);
//		}
	}

	void DelayMoveNpcWaitAnimationEnd()
	{
		if (IsDoFireAnimation && !IsFireMove) {
			return;
		}

		if (!IsZaiTiNpc && !IsDoFireAnimation) {
			NetNpcPlayAnimation(NpcAniScript, CurrentRunAnimation);
		}
		MoveNpcByItween();
	}

	public void NetNpcPlayAnimation(XKNpcAnimatorCtrl aniScript, string aniName)
	{
		aniScript.PlayNpcAnimatoin(aniName);

		if (Network.peerType == NetworkPeerType.Server && NetViewCom != null) {
			if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0 || !IsHandleRpc) {
				return;
			}
			NetViewCom.RPC("XKNpcSendPlayAnimation", RPCMode.OthersBuffered, aniName);
		}
	}

	[RPC] void XKNpcSendPlayAnimation(string aniName)
	{
		if (NpcAniScript == null) {
			return;
		}
		NpcAniScript.PlayNpcAnimatoin(aniName);
	}

	public bool GetIsDeathNPC()
	{
		return IsDeathNPC;
	}

	void DestroyNpc()
	{
		if (IsAniMove) {
			StartCoroutine(DestroyNetNpcObj(NpcObj));
		}
		else {
			StartCoroutine(DestroyNetNpcObj(NpcObj, RemoveNpcTime));
		}
	}

	void CallServerRemoveNpc()
	{
		if (Network.peerType != NetworkPeerType.Client) {
			return;
		}

		if (IsHandleRpc) {
			return;
		}
		NetViewCom.RPC("XKNpcSendServerRemoveNpc", RPCMode.OthersBuffered);
	}

	[RPC] void XKNpcSendServerRemoveNpc()
	{
		TriggerRemovePointNpc(1);
	}

	void CallServerRemoveCannon(int cannonIndex)
	{
		if (Network.peerType != NetworkPeerType.Client) {
			return;
		}
		
		if (IsHandleRpc) {
			return;
		}
		NetViewCom.RPC("XKNpcSendServerRemoveCannon", RPCMode.OthersBuffered, cannonIndex);
	}
	
	[RPC] void XKNpcSendServerRemoveCannon(int cannonIndex)
	{
		if (CannonScript == null) {
			return;
		}

		int max = CannonScript.Length;
		if (max > 1) {
			for (int i = 0; i < max; i++) {
				if (CannonScript[i] != null) {
					CannonScript[i].OnRemoveCannon(PlayerEnum.Null, 1);
					break;
				}
			}
		}
	}

	public void TriggerRemovePointNpc(int key, XKCannonCtrl cannonScriptVal = null)
	{
		if (cannonScriptVal != null && CannonScript != null) {
			int max = CannonScript.Length;
			if (max > 1) {
				for (int i = 0; i < max; i++) {
					if (CannonScript[i] != null && CannonScript[i] == cannonScriptVal) {
						CallServerRemoveCannon(i);
						break;
					}
				}
			}
		}

		if (IsDeathNPC) {
			return;
		}
		IsDeathNPC = true;
		HandleNpcDeathInfo();

		//XkGameCtrl.ClearNpcSpawnAllAmmo(gameObject);
		CallServerRemoveNpc();
		CancelInvoke("DelayMoveNpcWaitAnimationEnd");
		iTween itweenScript = GetComponent<iTween>();
		if (itweenScript != null) {
			itweenScript.isRunning = false;
			itweenScript.isPaused = true;
			Destroy(itweenScript);
		}
		AnimatorCom.enabled = false;

		if (key == 0) {
			StartCoroutine(DestroyNetNpcObj(NpcObj));
		}
		else {
			if (NpcTran.parent != XkGameCtrl.MissionCleanup) {
				NpcTran.parent = XkGameCtrl.MissionCleanup;
			}

			if (Vector3.Distance(Vector3.zero, RealNpcTran.localPosition) > 0.1f
			    && Network.peerType == NetworkPeerType.Disconnected) {
				//Debug.Log("fix realnpc localPosition, name "+NpcObj.name);
				Vector3 posTmpVal = RealNpcTran.position;
				RealNpcTran.parent = XkGameCtrl.MissionCleanup;
				NpcObj.transform.position = posTmpVal;
				RealNpcTran.parent = NpcObj.transform;
				RealNpcTran.localPosition = Vector3.zero;
			}

			if (DeathExplode != null) {
				if (!DeathExplode.activeSelf) {
					DeathExplode.SetActive(true);
				}

				GameObject objExplode = null;
				if (NpcState == NpcType.LandNpc) {
					objExplode = (GameObject)Instantiate(DeathExplode, RealNpcTran.position, RealNpcTran.rotation);
				}
				else {
					objExplode = (GameObject)Instantiate(DeathExplode, NpcTran.position, NpcTran.rotation);
				}
				objExplode.transform.parent = XkGameCtrl.NpcAmmoArray;
				XkGameCtrl.CheckObjDestroyThisTimed(objExplode);
			}
			
			if (BuWaWaRigidbody != null && !IsZaiTiNpc) {
				Rigidbody rigCom = NpcObj.GetComponent<Rigidbody>();
				if (rigCom == null) {
					rigCom = NpcObj.AddComponent<Rigidbody>();
				}
				rigCom.isKinematic = false;
				rigCom.AddForce(NpcTran.forward * 5f, ForceMode.Impulse);
				BuWaWaRigidbody.gameObject.SetActive(true);
				BuWaWaRigidbody.AddForce(NpcTran.forward * 95f, ForceMode.Impulse);
				DestroyNpc();
			}
			else if (IsZaiTiNpc) {
				Rigidbody rigCom = NpcObj.GetComponent<Rigidbody>();
				if (rigCom == null) {
					rigCom = NpcObj.AddComponent<Rigidbody>();
				}
				rigCom.isKinematic = false;
				rigCom.AddForce(NpcTran.forward * 5f, ForceMode.Impulse);

				XkNpcZaiTiCtrl zaiTiScript = null;
				int max = ZaiTiNpcAni.Length;
				for (int i = 0; i < max; i++) {
					zaiTiScript = ZaiTiNpcAni[i].GetComponent<XkNpcZaiTiCtrl>();
					if (ZaiTiNpcAni[i] != null && !zaiTiScript.GetIsDeathNPC()) {
						//ZaiTiNpcAni[i].transform.parent = XkGameCtrl.MissionCleanup;
//						if (ZaiTiNpcAni[i].gameObject.GetComponent<Rigidbody>() == null) {
//							ZaiTiNpcAni[i].gameObject.AddComponent<Rigidbody>();
//						}
						ZaiTiNpcAni[i].speed = 0f;
						ZaiTiNpcAni[i].enabled = false;
						//zaiTiScript.SetIsDeathNPC();
						zaiTiScript.SetIsZaiNpc();
						zaiTiScript.RemoveNpcObj();
						//StartCoroutine(DestroyNetNpcObj(ZaiTiNpcAni[i].gameObject, 2f));
					}
				}

				max = ZaiTiNpcBuWaWa.Length;
				for (int i = 0; i < max; i++) {
					if (ZaiTiNpcBuWaWa[i] != null) {
						ZaiTiNpcBuWaWa[i].gameObject.SetActive(true);
						ZaiTiNpcBuWaWa[i].AddForce(NpcTran.forward * 95f, ForceMode.Impulse);
					}
				}
				DestroyNpc();
			}
			else {
				StartCoroutine(DestroyNetNpcObj(NpcObj));
			}

			if (IsFangZhenNpc) {
				NpcFangZhenScript.SubFangZhenNpcCount();
			}

//			if (XKTriggerHuoJianDan.IsActivePanXuanPath) {
//				XKTriggerHuoJianDan.GetInstance().AddKillNpcNum();
//			}
		}
		XkGameCtrl.GetInstance().RemoveNpcTranFromList(NpcTran);
	}

	bool IsRemoveNpcObj;
	public void SetIsRemoveNpcObj()
	{
		//Debug.Log("SetIsRemoveNpcObj -> npcName "+NpcObj.name);
		IsRemoveNpcObj = true;
	}

	IEnumerator DestroyNetNpcObj(GameObject obj, float timeVal = 0f)
	{
		if (timeVal > 0f) {
			yield return new WaitForSeconds(timeVal);
		}

		bool isHiddenNpcTest = true;
		if (IsRemoveNpcObj) {
			isHiddenNpcTest = false;
		}

		if (!isHiddenNpcTest) {
			if (Network.peerType == NetworkPeerType.Disconnected) {
				XKNpcSpawnListCtrl.GetInstance().CheckNpcObjByNpcSpawnListDt(NpcObj);
				Destroy(obj);
			}
			else {
				if (Network.peerType == NetworkPeerType.Server) {
					XKNpcSpawnListCtrl.GetInstance().CheckNpcObjByNpcSpawnListDt(NpcObj);
					if (NetworkServerNet.GetInstance() != null) {
						NetworkServerNet.GetInstance().RemoveNetworkObj(obj);
					}
				}
			}
		}
		else {
			ResetNpcInfo();
		}
	}

	public void MoveFZNpcToFirePoint(Vector3 firePos, float mvSpeed)
	{
		if (IsDeathNPC) {
			return;
		}

		IsChangeNpcForward = false;
		Vector3[] posArray = new Vector3[2];
		posArray[0] = NpcTran.position;
		posArray[1] = firePos;
		
		iTween.MoveTo(NpcObj, iTween.Hash("path", posArray,
		                                  "speed", mvSpeed,
		                                  "orienttopath", true,
		                                  "easeType", iTween.EaseType.linear,
		                                  "oncomplete", "MoveFZNpcOnCompelteITween"));
	}

	void MoveFZNpcOnCompelteITween()
	{
		MakeNpcDoFireAnimation();
	}

	public bool GetIsAimPlayer()
	{
		return IsAimPlayer;
	}

	public XKSpawnNpcPoint GetSpawnPointScript()
	{
		return SpawnPointScript;
	}

	public void SetSpawnPointScript(XKSpawnNpcPoint script)
	{
		IsHandleRpc = true;
		SpawnPointScript = script;
		SetNpcAimPlayerState();
	}
	
	void MakeLandNpcMoveToLand()
	{
		if (NpcState != NpcType.LandNpc) {
			return;
		}
		RaycastHit hitInfo;
		Vector3 startPos = RealNpcTran.position + Vector3.up;
		Vector3 forwardVal = Vector3.down;
		Physics.Raycast(startPos, forwardVal, out hitInfo, 30f, XkGameCtrl.GetInstance().LandLayer.value);
		if (hitInfo.collider != null){
			RealNpcTran.position = hitInfo.point;
		}
	}

	public void SetFireDistance(float val)
	{
		//Debug.Log("valDis "+val);
		FireDistance = val;
		IsDoFireAnimation = true;
	}

	public void SetHuoCheNpcInfo(int indexPoint)
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		
		if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0) {
			return;
		}
		NetViewCom.RPC("XKNpcSendSetHuoCheNpcInfo", RPCMode.OthersBuffered, indexPoint);
	}
	
	[RPC] void XKNpcSendSetHuoCheNpcInfo(int indexPoint)
	{
		StartCoroutine(DelaySetHuoCheNpcInfo(indexPoint));
	}
	
	IEnumerator DelaySetHuoCheNpcInfo(int indexPoint)
	{
		yield return new WaitForSeconds(0.5f);
//		Debug.Log("XKNpcSendSetHuoCheNpcInfo -> indexPoint "+indexPoint+", name "+transform.name);
		if (Network.peerType == NetworkPeerType.Server) {
			yield break;
		}
		
		XKHuoCheCtrl huoCheScript = XKHuoCheCtrl.Instance;
		if (huoCheScript == null) {
			yield break;
		}
		
		XKSpawnNpcPoint[] pointScript = huoCheScript.gameObject.GetComponentsInChildren<XKSpawnNpcPoint>();
//		Debug.Log("XKNpcSendSetHuoCheNpcInfo -> pointScript.Len "+pointScript.Length);
		if (pointScript.Length > 0) {
			for (int i = 0; i < pointScript.Length; i++) {
				if (i == indexPoint) {
					IsHuoCheNpc = true;
//					Debug.Log("XKNpcSendSetHuoCheNpcInfo... IsHuoCheNpc "+IsHuoCheNpc+", parentName "
//					          +pointScript[i].transform.parent.name);
					CancelInvoke("DelayChangeNpcParent");
					transform.parent = pointScript[i].transform.parent;
					transform.localPosition = pointScript[i].transform.localPosition;
					transform.localEulerAngles = pointScript[i].transform.localEulerAngles;
					yield break;
				}
			}
		}
	}

	public void SetFeiJiNpcInfo(int indexPoint)
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		
		if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0) {
			return;
		}
		NetViewCom.RPC("XKNpcSendSetFeiJiNpcInfo", RPCMode.OthersBuffered, indexPoint);
	}
	
	[RPC] void XKNpcSendSetFeiJiNpcInfo(int indexPoint)
	{
		StartCoroutine(DelaySetFeiJiNpcInfo(indexPoint));
	}
	
	IEnumerator DelaySetFeiJiNpcInfo(int indexPoint)
	{
		yield return new WaitForSeconds(0.5f);
//		Debug.Log("XKNpcSendSetHuoCheNpcInfo -> indexPoint "+indexPoint+", name "+transform.name);
		if (Network.peerType == NetworkPeerType.Server) {
			yield break;
		}
		
		XKHuoCheCtrl huoCheScript = XKHuoCheCtrl.Instance;
		if (huoCheScript == null) {
			yield break;
		}
		
		XKSpawnNpcPoint[] pointScript = huoCheScript.gameObject.GetComponentsInChildren<XKSpawnNpcPoint>();
//		Debug.Log("XKNpcSendSetHuoCheNpcInfo -> pointScript.Len "+pointScript.Length);
		if (pointScript.Length > 0) {
			for (int i = 0; i < pointScript.Length; i++) {
				if (i == indexPoint) {
					IsHuoCheNpc = true;
//					Debug.Log("XKNpcSendSetHuoCheNpcInfo... IsHuoCheNpc "+IsHuoCheNpc+", parentName "
//					          +pointScript[i].transform.parent.name);
					CancelInvoke("DelayChangeNpcParent");
					transform.parent = pointScript[i].transform.parent;
					transform.localPosition = pointScript[i].transform.localPosition;
					transform.localEulerAngles = pointScript[i].transform.localEulerAngles;
					yield break;
				}
			}
		}
	}

	void CheckNpcIsRemove()
	{
		if (Time.realtimeSinceStartup - SpawnTimeVal < 180f) {
			return;
		}
		
		if (Time.frameCount % 100 != 0) {
			return;
		}
		
		if (Network.peerType == NetworkPeerType.Client) {
			return;
		}
		
		XkPlayerCtrl scriptFJ = XkPlayerCtrl.GetInstanceFeiJi();
		XkPlayerCtrl scriptTK = XkPlayerCtrl.GetInstanceTanKe();
		if (scriptFJ == null && scriptTK == null) {
			return;
		}
		
		float disFJ = 0f;
		float disTK = 0f;
		float minDis = 400f;
		Vector3 posA = Vector3.zero;
		Vector3 posB = Vector3.zero;
		Vector3 posC = Vector3.zero;
		if (scriptFJ != null && scriptTK != null) {
			posA = NpcTran.position;
			posB = scriptFJ.transform.position;
			posC = scriptTK.transform.position;
			posA.y = posB.y = posC.y = 0f;
			disFJ = Vector3.Distance(posB, posA);
			disTK = Vector3.Distance(posC, posA);
			if (disFJ > minDis && disTK > minDis) {
				//Debug.Log("npcMoveTest*******************111");
				TriggerRemovePointNpc(0);
			}
			return;
		}
		
		if (scriptFJ != null) {
			posA = NpcTran.position;
			posB = scriptFJ.transform.position;
			posA.y = posB.y = posC.y = 0f;
			disFJ = Vector3.Distance(posB, posA);
			if (disFJ > minDis) {
				//Debug.Log("npcMoveTest*******************222");
				TriggerRemovePointNpc(0);
			}
			return;
		}
		
		if (scriptTK != null) {
			posA = NpcTran.position;
			posC = scriptTK.transform.position;
			posA.y = posB.y = posC.y = 0f;
			disTK = Vector3.Distance(posC, posA);
			if (disTK > minDis) {
				//Debug.Log("npcMoveTest*******************333");
				TriggerRemovePointNpc(0);
			}
			return;
		}
	}

	/// <summary>
	/// Resets the npc info.
	/// </summary>
	void ResetNpcInfo()
	{
		IsInitNpcInfo = false;
		IsHandleRpc = false;
		IsMoveToMarkPoint = false;
		
		IsMoveFirePoint = false;
//		IsIntoJingJieState = false;
		IsDelayFireAction = false;
		
		IsAimPlayerByFire = false;
		IsHuoCheNpc = false;
		IsWuDi = false;
		
		IsDoFireAniZaiTi = false;
		IsFangZhenNpc = false;
		IsAimPlayer = false;

		IsDoFireAnimation = false;
		IsChangeNpcForward = false;
		//IsDeathNPC = false;

		IndexFirePointGroup = 0;
		MarkCount = 0;

		XkNpcZaiTiCtrl zaiTiScript = GetComponentInChildren<XkNpcZaiTiCtrl>();
		if (zaiTiScript != null && zaiTiScript.ZaiTiNpcBuWaWa != null) {
			zaiTiScript.ResetNpcZaiTiSomeInfo();
		}

		RealNpcTran.gameObject.SetActive(false);
		RealNpcTran.localPosition = Vector3.zero;
		RealNpcTran.localEulerAngles = Vector3.zero;
		if (BuWaWaRigidbody != null) {
			Transform buWaWaTran = BuWaWaRigidbody.transform;
			buWaWaTran.localPosition = Vector3.zero;
			buWaWaTran.localEulerAngles = Vector3.zero;
		}

		Rigidbody rigCom = NpcObj.GetComponent<Rigidbody>();
		if (rigCom != null && !rigCom.isKinematic) {
			Destroy(rigCom);
		}

		rigCom = RealNpcTran.GetComponent<Rigidbody>();
		if (rigCom != null && !rigCom.isKinematic) {
			Destroy(rigCom);
		}
		NpcTran.position = new Vector3(-18000f, -18000f, 0f);

		if (Network.peerType == NetworkPeerType.Server) {
			NetViewCom.RPC("NpcSendResetNpcTransformInfo", RPCMode.OthersBuffered);
		}
	}

	[RPC] void NpcSendResetNpcTransformInfo()
	{
		IsDeathNPC = true;
		AnimatorCom.enabled = false;
		XkNpcZaiTiCtrl zaiTiScript = GetComponentInChildren<XkNpcZaiTiCtrl>();
		if (zaiTiScript != null && zaiTiScript.ZaiTiNpcBuWaWa != null) {
			zaiTiScript.ResetNpcZaiTiSomeInfo();
		}

		RealNpcTran.gameObject.SetActive(false);
		RealNpcTran.localPosition = Vector3.zero;
		RealNpcTran.localEulerAngles = Vector3.zero;

		if (BuWaWaRigidbody != null) {
			Transform buWaWaTran = BuWaWaRigidbody.transform;
			buWaWaTran.localPosition = Vector3.zero;
			buWaWaTran.localEulerAngles = Vector3.zero;
		}
		
		Rigidbody rigCom = NpcObj.GetComponent<Rigidbody>();
		if (rigCom != null && !rigCom.isKinematic) {
			Destroy(rigCom);
		}
		
		rigCom = RealNpcTran.GetComponent<Rigidbody>();
		if (rigCom != null && !rigCom.isKinematic) {
			Destroy(rigCom);
		}
		NpcTran.position = new Vector3(-18000f, -18000f, 0f);
	}

	void HandleNpcDeathInfo()
	{
		if (IsZaiTiNpc || IsAniMove) {
			return;
		}

		if (NpcAniScript != null) {
			BoxCollider boxCol = NpcAniScript.GetComponent<BoxCollider>();
			if (boxCol == null || !boxCol.enabled) {
				return;
			}
			NpcAniScript.ResetNpcAnimation();
		}
	}
}

public enum NpcType
{
	WaterNpc,
	LandNpc,
	FlyNpc,
}