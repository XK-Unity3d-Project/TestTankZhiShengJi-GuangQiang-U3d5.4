using UnityEngine;
using System.Collections;

public class XKNpcFangZhenCtrl : MonoBehaviour {

	public Transform NpcFirePointGroup;
	public GameObject TestSpawnPoint;
	GameObject NpcObj;
	Transform NpcTran;
	NetworkView NetViewCom;
	Transform NpcPathTran;
	float MvSpeed;
	int MarkCount;
	AnimatorNameNPC CurrentRunAnimation = AnimatorNameNPC.Null;
	bool IsDeathNPC;
	XKNpcMoveCtrl[] NpcMoveScript;
	bool IsMoveToFirePoint;
	float FireDistance = 0f;
	int FangZhenNpcCount = 1;
	bool IsMoveEndPoint;
	iTween ITweenScriptNpc;
	XKSpawnNpcPoint SpawnPointScript;
	NpcPathCtrl NpcPathScript;
	//int NpcId;
	void Awake()
	{
		//NpcId = GetInstanceID();
		if (NetViewCom == null) {
			NetViewCom = GetComponent<NetworkView>();
		}

		if (Network.peerType == NetworkPeerType.Disconnected) {
			NetViewCom.enabled = false;
		}
		transform.parent = XkGameCtrl.MissionCleanup;
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
				return;
			}
		}
		else {
			if (ITweenScriptNpc != null && !ITweenScriptNpc.isRunning) {
				ITweenScriptNpc.isRunning = true;
			}
		}

		if (!ScreenDanHeiCtrl.IsStartGame) {
			return;
		}

		if (IsDeathNPC) {
			return;
		}

		if (IsMoveEndPoint && NpcPathScript.IsMoveEndFire) {
			return;
		}

		if (NpcTran == null) {
			return;
		}

		if (!IsMoveToFirePoint) {
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
				if (XkPlayerCtrl.PlayerTranFeiJi != null || XkPlayerCtrl.PlayerTranTanKe != null) {
					if (AimStateVal == 0 || AimStateVal == 2) {
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
				else {
					return;
				}
				break;
			}

			posA.y = posB.y = 0f;
			if (Vector3.Distance(posA, posB) <= FireDistance) {
				MoveFangZhenNpcToFirePoint();
			}
		}
	}

	/*********************************************************************
	 * (AimStateVal == 0 || AimStateVal == 2) ---> AimPlayerFeiJi,
	 * otherwise AimPlayerTanKe.
	 ********************************************************************/
	int AimStateVal = -1;
	void SetAimState()
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
		AimStateVal = aimState;
	}

	public static AnimatorNameNPC GetRandRunAniName()
	{
		AnimatorNameNPC runAni = AnimatorNameNPC.Run1;
		int randVal = Random.Range(0, 50) % 3;
		if (randVal == 0) {
			runAni = AnimatorNameNPC.Run1;
		}
		else if (randVal == 1) {
			runAni = AnimatorNameNPC.Run2;
		}
		else {
			runAni = AnimatorNameNPC.Run3;
		}
		return runAni;
	}

	void MoveFangZhenNpcToFirePoint()
	{
		if (IsMoveToFirePoint) {
			return;
		}
		IsMoveToFirePoint = true;
		bool isBreak = false;
		int count = 0;
		int index = 0;

		iTween itweenScript = GetComponent<iTween>();
		if (itweenScript != null) {
			itweenScript.isRunning = false;
			itweenScript.isPaused = true;
			itweenScript.enabled = false;
		}

		do {
			if (IsDeathNPC) {
				isBreak = true;
				break;
			}

			if (NpcMoveScript[index] != null) {
				if (SpawnPointScript.AniFangZhenFireRun != AnimatorNameNPC.Null) {
					PlayNpcAnimation(SpawnPointScript.AniFangZhenFireRun);
				}
				else if (CurrentRunAnimation == AnimatorNameNPC.Null) {
					AnimatorNameNPC runAni = GetRandRunAniName();
					PlayNpcAnimation(runAni);
				}

				float speed = SpawnPointScript.SpeedFangZhenFireRun;
				if (NpcPathScript != null && NpcMoveScript[index] != null) {
					if (SpawnPointScript.FirePointGroup.Length < 1) {
						NpcMoveScript[index].MoveFZNpcToFirePoint(NpcFirePointGroup.GetChild(count).position, speed);
					}
					else {
						if (!NpcPathScript.IsMoveEndFire) {
							NpcMoveScript[index].MoveFZNpcToFirePoint(NpcFirePointGroup.GetChild(count).position, speed);
						}
						else {
							//移动到开火点组.
							NpcMoveScript[index].MakeNpcMoveFirePoint();
						}
					}
				}
			}

			index++;
			if (index >= NpcMoveScript.Length) {
				isBreak = true;
				break;
			}

			count++;
			if (count >= NpcFirePointGroup.childCount) {
				count = 0;
			}
		} while(!isBreak);
		//Debug.Log("MoveFangZhenNpcToFirePoint...");
	}

	public void ActiveFangZhenNpc()
	{
		if (IsHiddenNpcObj) {
			IsHiddenNpcObj = false;
			IsActiveFangZhen = true;
			//Debug.Log("ActiveFangZhenNpc -> objName "+NpcObj.name+", npcId "+NpcId);
		}
	}
	bool IsActiveFangZhen = true;
	public bool GetIsActiveFangZhen()
	{
		return IsActiveFangZhen;
	}

	public void SetSpawnNpcInfo(XKSpawnNpcPoint spawnScript)
	{
		NetViewCom = GetComponent<NetworkView>();
		SpawnPointScript = spawnScript;
		SetAimState();
		TestSpawnPoint = spawnScript.gameObject;
		if (spawnScript.NpcPath != null) {
			NpcPathScript = spawnScript.NpcPath.GetComponent<NpcPathCtrl>();
		}

		NpcObj = gameObject;
		NpcTran = transform;
		NpcPathTran = spawnScript.NpcPath;
		MvSpeed = spawnScript.MvSpeed;
		FireDistance = spawnScript.FireDistance;
		
		if (IsDeathNPC) {
			IsDeathNPC = false;
			IsHiddenNpcObj = false;
		}

		SetNpcFireAnimationIsAimFeiJiPlayer(spawnScript.IsAimFeiJiPlayer);

		NpcMoveScript = NpcTran.GetComponentsInChildren<XKNpcMoveCtrl>();
		for (int i = 0; i < NpcMoveScript.Length; i++) {
			NpcMoveScript[i].SetIsFangZhenNpc(this, i);
			NpcMoveScript[i].SetNpcSpawnScriptInfo(spawnScript);
			NpcMoveScript[i].SetIndexNpc(i);
		}
		FangZhenNpcCount = NpcMoveScript.Length;
		//Debug.Log("SetSpawnNpcInfo -> FangZhenNpcCount "+FangZhenNpcCount+", fangZhenObj "+NpcObj.name);

		if (spawnScript.TimeRootAni > 0f) {
			PlayNpcAnimation(spawnScript.AniRootName);
		}
		StartCoroutine(StartMoveNpcByItween(spawnScript.AniName, spawnScript.TimeRootAni));
	}

	void SetNpcFireAnimationIsAimFeiJiPlayer(bool isAim)
	{
		SetNpcIsAimFeiJiPlayer(isAim);
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		NetViewCom.RPC("XKNpcFZSendFireAnimationIsAimFeiJiPlayer", RPCMode.OthersBuffered,
		               isAim == true ? 1 : 0);
	}

	void SetNpcIsAimFeiJiPlayer(bool isAim)
	{
		XKNpcAnimatorCtrl[] npcAniScript = GetComponentsInChildren<XKNpcAnimatorCtrl>();
		for (int i = 0; i < npcAniScript.Length; i++) {
			npcAniScript[i].SetIsAimFeiJiPlayer(isAim);
		}
	}

	[RPC] void XKNpcFZSendFireAnimationIsAimFeiJiPlayer(int val)
	{
		bool isAim = val == 1 ? true : false;
		SetNpcIsAimFeiJiPlayer(isAim);
	}

	IEnumerator StartMoveNpcByItween(AnimatorNameNPC aniVal, float rootTime)
	{
		yield return new WaitForSeconds(rootTime);
		if (IsDeathNPC || IsMoveToFirePoint) {
			yield break;
		}

		AnimatorNameNPC runAni = AnimatorNameNPC.Run1;
		if (aniVal == AnimatorNameNPC.Run1 || aniVal == AnimatorNameNPC.Run2 || aniVal == AnimatorNameNPC.Run3) {
			runAni = aniVal;
		}
		else {
			runAni = GetRandRunAniName();
		}
		PlayNpcAnimation(runAni);
		MoveNpcByItween();
		yield break;
	}

	void MakeNpcForwardMarkPos(Transform mark)
	{
		Vector3 vecA = Vector3.zero;
		Vector3 posA = Vector3.zero;
		Vector3 posB = Vector3.zero;

		posA = mark.position;
		posB = NpcTran.position;
		vecA = posA - posB;
		for (int i = 0; i < NpcMoveScript.Length; i++) {
			if (NpcMoveScript[i] != null) {
				NpcMoveScript[i].SetNpcForwardVal(vecA);
			}
		}
	}
	
	void PlayNpcAnimation(AnimatorNameNPC aniVal)
	{
		if (aniVal != AnimatorNameNPC.Null) {
			if (aniVal == AnimatorNameNPC.Run1 || aniVal == AnimatorNameNPC.Run2
			    || aniVal == AnimatorNameNPC.Run3) {
				CurrentRunAnimation = aniVal;
			}

			for (int i = 0; i < NpcMoveScript.Length; i++) {
				if (NpcMoveScript[i] != null) {
					NpcMoveScript[i].PlayNpcAnimation(aniVal);
				}
			}
		}
	}

	void MoveNpcByItween()
	{
		Transform[] tranArray = new Transform[2];
		tranArray[0] = NpcTran;
		tranArray[1] = NpcPathTran.GetChild(MarkCount);
		MakeNpcForwardMarkPos(tranArray[1]);
		MarkCount++;
		
		iTween.MoveTo(NpcObj, iTween.Hash("path", tranArray,
		                                  "speed", MvSpeed,
		                                  "orienttopath", false,
		                                  "easeType", iTween.EaseType.linear,
		                                  "oncomplete", "MoveNpcOnCompelteITween"));
	}

	void MakeNpcPlayFireAnimation()
	{
		for (int i = 0; i < NpcMoveScript.Length; i++) {
			if (NpcMoveScript[i] != null) {
				NpcMoveScript[i].MakeNpcDoFireAnimation();
			}
		}
	}

	void MakeNpcPlayRootAnimation()
	{
		for (int i = 0; i < NpcMoveScript.Length; i++) {
			if (NpcMoveScript[i] != null) {
				NpcMoveScript[i].PlayNpcAnimation(AnimatorNameNPC.Root1);
			}
		}
	}

	void MoveNpcOnCompelteITween()
	{
		if (MarkCount >= NpcPathTran.childCount) {
			IsMoveEndPoint = true;
			if (NpcPathScript != null && NpcPathScript.IsMoveEndFire) {
				MakeNpcPlayFireAnimation();
			}
			else {
				MakeNpcPlayRootAnimation();
			}
			return;
		}

		Transform markTran = NpcPathTran.GetChild(MarkCount - 1);
		NpcMark markScript = markTran.GetComponent<NpcMark>();
		MvSpeed = markScript.MvSpeed;
		PlayNpcAnimation(markScript.AniName);
		
		//Debug.Log("MoveNpcOnCompelteITween...npc is "+NpcObj.name);
		if (markScript.AnimatorTime > 0f && markScript.AniName != AnimatorNameNPC.Null) {
			Invoke("DelayMoveNpcWaitAnimationEnd", markScript.AnimatorTime);
		}
		else {
			MoveNpcByItween();
		}
	}
	
	void DelayMoveNpcWaitAnimationEnd()
	{
		if (IsDeathNPC) {
			return;
		}
		PlayNpcAnimation(CurrentRunAnimation);
		MoveNpcByItween();
	}

	public void TriggerRemovePointNpc(int key)
	{
		if (IsDeathNPC) {
			return;
		}
		IsDeathNPC = true;
		CancelInvoke("DelayMoveNpcWaitAnimationEnd");
		iTween itweenScript = GetComponent<iTween>();
		if (itweenScript != null) {
			itweenScript.isRunning = false;
			itweenScript.isPaused = true;
			Destroy(itweenScript);
		}
		
		if (key == 0) {
			StartCoroutine(DestroyNetNpcObj(NpcObj));
		}
		else {
			StartCoroutine(DestroyNetNpcObj(NpcObj, 3f));
		}
	}

	IEnumerator DestroyNetNpcObj(GameObject obj, float timeVal = 0f)
	{
		if (timeVal > 0f) {
			yield return new WaitForSeconds(timeVal);
		}

		bool isTestHiddenObj = true;
		if (!isTestHiddenObj) {
			if (Network.peerType == NetworkPeerType.Disconnected) {
				Destroy(obj);
			}
			else {
				if (Network.peerType == NetworkPeerType.Server) {
					if (NetworkServerNet.GetInstance() != null) {
						int max = NpcMoveScript.Length;
						for (int i = 0; i < max; i++) {
							if (NpcMoveScript[i] != null) {
								NetworkServerNet.GetInstance().RemoveNetworkObj(NpcMoveScript[i].gameObject);
							}
						}
						NetworkServerNet.GetInstance().RemoveNetworkObj(obj);
					}
				}
			}
		}
		else {
			Transform npcChildTran = null;
			NpcMoveScript = NpcTran.GetComponentsInChildren<XKNpcMoveCtrl>();
			int max = NpcMoveScript.Length;
			for (int i = 0; i < max; i++) {
				if (NpcMoveScript[i] != null) {
					NpcMoveScript[i].TriggerRemovePointNpc(0);
					npcChildTran = NpcMoveScript[i].transform;
					npcChildTran.parent = XkGameCtrl.MissionCleanup;
				}
			}
			//Debug.Log("****max "+max+", fangZhenObj "+NpcObj.name);
			NpcTran.position = new Vector3(-18000f, -18000f, 0f);
			ResetFangZhenObjInfo();
			IsHiddenNpcObj = true;
			IsActiveFangZhen = false;
		}
	}

	bool IsHiddenNpcObj;
	public bool GetIsHiddenNpcObj()
	{
		return IsHiddenNpcObj;
	}

	public void SubFangZhenNpcCount()
	{
		FangZhenNpcCount--;
		if (FangZhenNpcCount <= 0) {
			TriggerRemovePointNpc(1);
		}
	}

	public bool GetIsDeathNPC()
	{
		return IsDeathNPC;
	}

	void ResetFangZhenObjInfo()
	{
		IsMoveEndPoint = false;
		IsMoveToFirePoint = false;

		NpcTran = null;
		MarkCount = 0;
		FireDistance = 0f;
		FangZhenNpcCount = 0;
		CurrentRunAnimation = AnimatorNameNPC.Null;
		NpcMoveScript = null;
	}
}