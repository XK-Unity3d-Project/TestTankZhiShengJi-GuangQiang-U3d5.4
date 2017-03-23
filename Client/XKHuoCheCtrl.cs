using UnityEngine;
using System.Collections;

public class XKHuoCheCtrl : MonoBehaviour {
	public Animator AnimatorCom;
	Transform AnimatorTran;
	public Transform RealHuoCheTran;
	public GameObject TestSpawnPoint;
	Transform NpcPathTran;
	GameObject NpcObj;
	Transform NpcTran;
	int MarkCount;
	float MvSpeed = 1f;
	iTween ITweenScriptNpc;
	bool IsStopAnimation;
	XKSpawnNpcPoint SpawnPointScript;
	NetworkView NetViewCom;
	bool IsHandleRpc;
	bool IsPlayTurnAnimation;
	string AnimationCurName;
	Vector3 SpawnPointHuoChePos;
	Quaternion SpawnPointHuoCheRot;
	bool IsDeathNpc;
	public static XKHuoCheCtrl Instance;
	void Start()
	{
		Instance = this;
		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			if (TestSpawnPoint != null) {
				XKSpawnNpcPoint spawnCom = TestSpawnPoint.GetComponent<XKSpawnNpcPoint>();
				if (spawnCom != null) {
					SetHuoCheInfo(spawnCom);
				}
			}
		}

		if (transform.parent == null) {
			transform.parent = XkGameCtrl.MissionCleanup;
			NpcObj = gameObject;
			AnimatorTran = AnimatorCom.transform;
			NpcTran = transform;
		}

		if (NetViewCom == null) {
			NetViewCom = GetComponent<NetworkView>();
		}
	}

	void Update()
	{
		if (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo) {
			if (!XkGameCtrl.IsMoveOnPlayerDeath) {
				if (ITweenScriptNpc == null) {
					ITweenScriptNpc = GetComponent<iTween>();
				}
				
				if (ITweenScriptNpc != null && ITweenScriptNpc.isRunning) {
					ITweenScriptNpc.isRunning = false;
				}
				
				if (!IsStopAnimation) {
					IsStopAnimation = true;
					AnimatorCom.speed = 0f;
				}
				return;
			}
		}
		else {
			if (ITweenScriptNpc != null && !ITweenScriptNpc.isRunning) {
				ITweenScriptNpc.isRunning = true;
			}

			if (IsStopAnimation) {
				IsStopAnimation = false;
				AnimatorCom.speed = 1f;
			}
		}

		if (IsHandleRpc && !IsPlayTurnAnimation) {
			SendNpcTransformInfo();
		}
	}

	public void StartMoveHuoChe(Transform tranPath)
	{
		NpcObj = gameObject;
		AnimatorTran = AnimatorCom.transform;
		NpcTran = transform;
		NpcPathTran = tranPath;
		Transform markTran = NpcPathTran.GetChild(MarkCount);
		NpcMark markScript = markTran.GetComponent<NpcMark>();
		MvSpeed = markScript.MvSpeed;
		Invoke("MoveNpcByItween", SpawnPointScript.TimeRootAni);
		//MoveNpcByItween();
	}

	public void OnNpcTriggerAnimation()
	{
		if (NpcTran == null) {
			Destroy(AnimatorTran.gameObject);
			return;
		}

		if (AnimationCurName == AnimatorNameNPC.TurnLeft.ToString()
		    || AnimationCurName == AnimatorNameNPC.TurnRight.ToString()) {
			IsPlayTurnAnimation = false;
			NpcTran.position = RealHuoCheTran.position;
			AnimatorTran.parent = NpcTran;
			if (IsHandleRpc) {
				SendNpcTransformInfo();
			}
		}

		if (IsHandleRpc) {
			//ResetNpcAnimation();
			MoveNpcByItween();
		}
	}

	void MoveNpcByItween()
	{
		Transform[] tranArray = new Transform[2];
		tranArray[0] = NpcTran;
		tranArray[1] = NpcPathTran.GetChild(MarkCount);
		MarkCount++;
		iTween.MoveTo(NpcObj, iTween.Hash("path", tranArray,
		                                  "speed", MvSpeed,
		                                  "orienttopath", false,
		                                  "easeType", iTween.EaseType.linear,
		                                  "oncomplete", "MoveNpcOnCompelteITween"));
	}

	void MoveNpcOnCompelteITween()
	{
		if (MarkCount >= NpcPathTran.childCount) {
			//goto end point
			return;
		}

		Transform markTran = NpcPathTran.GetChild(MarkCount - 1);
		NpcMark markScript = markTran.GetComponent<NpcMark>();
		MvSpeed = markScript.MvSpeed;
		if (markScript.AniName != AnimatorNameNPC.Null) {
//			if (markScript.AniName == AnimatorNameNPC.TurnLeft || markScript.AniName == AnimatorNameNPC.TurnRight) {
//				AnimatorTran.parent = XkGameCtrl.MissionCleanup;
//				AnimatorTran.position = SpawnPointScript.HuoCheNpcTran.position;
//				AnimatorTran.rotation = SpawnPointScript.HuoCheNpcTran.rotation;
//			}
			PlayNpcAnimation(markScript.AniName.ToString());
			SendNpcPlayAnimation(markScript.AniName.ToString());
		}
		else {
			MoveNpcByItween();
		}
		//Debug.Log("MoveNpcOnCompelteITween...npc is "+NpcObj.name);
	}

	void ResetNpcAnimation()
	{
		AnimatorCom.SetBool("TurnLeft", false);
		AnimatorCom.SetBool("TurnRight", false);
	}

	public void PlayNpcAnimation(string aniVal)
	{
		if (aniVal == AnimatorNameNPC.TurnLeft.ToString() || aniVal == AnimatorNameNPC.TurnRight.ToString()) {
			AnimatorTran.parent = XkGameCtrl.MissionCleanup;
			AnimatorTran.position = SpawnPointHuoChePos;
			AnimatorTran.rotation = SpawnPointHuoCheRot;
			AnimationCurName = aniVal;
			IsPlayTurnAnimation = true;
		}
		//ResetNpcAnimation();
		AnimatorCom.SetBool(aniVal.ToString(), true);
	}

	public void SetHuoCheInfo(XKSpawnNpcPoint script)
	{
		IsHandleRpc = true;
		NetViewCom = GetComponent<NetworkView>();
		SpawnPointScript = script;
		transform.position = SpawnPointScript.transform.position;
		transform.rotation = SpawnPointScript.transform.rotation;
		TestSpawnPoint = SpawnPointScript.gameObject;
		SendHuoCheTranInfo(SpawnPointScript.HuoCheNpcTran.position, SpawnPointScript.HuoCheNpcTran.rotation);
		
		XKNpcMoveCtrl[] npcMoveScript = GetComponentsInChildren<XKNpcMoveCtrl>();
		int max = npcMoveScript.Length;
		if (max > 0) {
			for (int i = 0; i < max; i++) {
				npcMoveScript[i].SetSpawnPointScript(script);
			}
		}

		XKCannonCtrl[] npcCannonScript = GetComponentsInChildren<XKCannonCtrl>();
		max = npcCannonScript.Length;
		if (max > 0) {
			for (int i = 0; i < max; i++) {
				npcCannonScript[i].SetSpawnPointScript(null);
			}
			SetCannonAimPlayerState();
		}
		
		XKSpawnNpcPoint[] pointScript = GetComponentsInChildren<XKSpawnNpcPoint>();
		//Debug.Log("pointScript.Len "+pointScript.Length);
		max = pointScript.Length;
		if (max > 0) {
			//for (int i = 0; i < 1; i++) { //test
			for (int i = 0; i < max; i++) {
				pointScript[i].SetIsHuoCheNpc();
				pointScript[i].SpawnPointAllNpc();
				SetOtherPortHuoCheNpcInfo(pointScript[i], i);
			}
		}

		StartMoveHuoChe(script.NpcPath);
	}

	void SetOtherPortHuoCheNpcInfo(XKSpawnNpcPoint pointScript, int indexPoint)
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}

		GameObject npcObj = pointScript.GetNpcLoopObj();
		if (pointScript == null || npcObj == null) {
			return;
		}

		XKDaPaoCtrl daPaoScript = npcObj.GetComponent<XKDaPaoCtrl>();
		if (daPaoScript != null) {
			daPaoScript.SetHuoCheNpcInfo(indexPoint);
		}
		else {
			XKNpcMoveCtrl npcScript = npcObj.GetComponent<XKNpcMoveCtrl>();
			if (npcScript != null) {
				npcScript.SetHuoCheNpcInfo(indexPoint);
			}
		}
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
		NetViewCom.RPC("XKHuoCheSendSetNpcAimPlayerState", RPCMode.OthersBuffered, aimState, SpawnPointScript.FireDistance);
	}
	
	[RPC] void XKHuoCheSendSetNpcAimPlayerState(int valAim, float valFireDis)
	{
		//Debug.Log("XKCannonSendSetNpcAimPlayerState.............");
		SetCannonNpcInfo(valAim, valFireDis);
	}
	
	void SetCannonNpcInfo(int valAim, float valFireDis)
	{
		XKCannonCtrl[] cannoScript = GetComponentsInChildren<XKCannonCtrl>();
		int max = cannoScript.Length;
		for (int i = 0; i < max; i++) {
			cannoScript[i].SetCannonSpawnPointInfo(valAim, valFireDis);
		}
	}

	void SendNpcTransformInfo()
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		
		if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0) {
			return;
		}
		
		if (!IsHandleRpc) {
			return;
		}
		NetViewCom.RPC("XKHuoCheSendOtherTranformInfo", RPCMode.OthersBuffered, transform.position, transform.rotation);
	}
	
	[RPC] void XKHuoCheSendOtherTranformInfo(Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;
	}
	
	void SendNpcPlayAnimation(string ani)
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		
		if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0) {
			return;
		}
		
		if (!IsHandleRpc) {
			return;
		}
		NetViewCom.RPC("XKHuoCheSendPlayAnimation", RPCMode.OthersBuffered, ani);
	}
	
	[RPC] void XKHuoCheSendPlayAnimation(string ani)
	{
		PlayNpcAnimation(ani);
	}

	void SendHuoCheTranInfo(Vector3 pos, Quaternion rot)
	{
		SpawnPointHuoChePos = pos;
		SpawnPointHuoCheRot = rot;
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		
		if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0) {
			return;
		}
		
		if (!IsHandleRpc) {
			return;
		}
		NetViewCom.RPC("XKHuoCheSendHuoCheTranInfo", RPCMode.OthersBuffered, pos, rot);
	}
	
	[RPC] void XKHuoCheSendHuoCheTranInfo(Vector3 pos, Quaternion rot)
	{
		SpawnPointHuoChePos = pos;
		SpawnPointHuoCheRot = rot;
	}

	public void OnRemoveHuoCheObj(float timeVal = 0f)
	{
		if (IsDeathNpc) {
			return;
		}
		IsDeathNpc = true;

		//XkGameCtrl.ClearNpcSpawnAllAmmo(gameObject);
		XkGameCtrl.GetInstance().RemoveNpcTranFromList(transform);
		if (Network.peerType == NetworkPeerType.Disconnected) {
			XKNpcSpawnListCtrl.GetInstance().CheckNpcObjByNpcSpawnListDt(gameObject);
			Destroy(gameObject, timeVal);
		}
		else {
			if (NetworkServerNet.GetInstance() != null) {
				XKNpcSpawnListCtrl.GetInstance().CheckNpcObjByNpcSpawnListDt(gameObject);
				NetworkServerNet.GetInstance().RemoveNetworkObj(gameObject);
			}
		}
	}
}