using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XKSpawnNpcPoint : MonoBehaviour {
	public SpawnPointType PointType = SpawnPointType.KongZhong;
	public bool IsAimFeiJiPlayer;
	public Transform HuoCheNpcTran;
	public GameObject NpcObj;
	public Transform NpcPath;
	public FirePointCtrl[] FirePointGroup;
	public bool IsLoopFirePoint;
	bool IsHuoCheNpc;
	public bool IsAimPlayer; //fire
	public bool IsDoublePlayer;
	public Transform[] ChildSpawnPoint;
	public GameObject NpcFangZhen; //用于方阵npc的攻击点逻辑.
	[Range(0.1f, 100f)] public float MvSpeed = 1f;
	[Range(0.1f, 1000f)] public float FireDistance = 1f;
	public AnimatorNameNPC AniFangZhenFireRun; //方阵npc进入攻击点时的run动画.
	[Range(0.1f, 100f)] public float SpeedFangZhenFireRun; //方阵npc进入攻击点时移动的速度.
	public AnimatorNameNPC AniName; //Run animation
	public AnimatorNameNPC AniRootName;
	[Range(0f, 30f)] public float TimeRootAni = 0f;
	float LoopSpawnTime = 0.1f; //miao
	public GameObject[] NpcSpawnLoop; //循环产生npc的数组.
	[Range(1, 30)] public int SpawnMaxNpc = 1;
	[Range(0f, 30f)] public float TimeMinFire = 0.1f;
	[Range(0f, 30f)] public float TimeMaxFire = 2f;
	public RuntimeAnimatorController AniController; //FlyNpc动画控制运动的Controller.
	public List<XKTriggerSpawnNpc> TestTriggerSpawnNpc;
	public List<XKTriggerRemoveNpc> TestTriggerRemoveNpc;
	int SpawnNpcCount;
	XKNpcMoveCtrl[] NpcScript;
	XKNpcFangZhenCtrl NpcFangZhenScript;
	bool IsRemoveSpawnPointNpc;
	XKHuoCheCtrl HuoCheScript;
	XKCannonCtrl CannonScript;
	XKDaPaoCtrl DaPaoScript;
	bool IsSpawnPointNpc;
	GameObject NpcLoopObj;
	bool IsRemoveTrigger;
	bool IsSpawnTrigger;
	bool IsPlayerAimTrigger;
	bool IsPlayerLeaveTrigger;
	// Use this for initialization
	void Awake()
	{
		if (HuoCheNpcTran != null) {
			HuoCheNpcTran.gameObject.SetActive(false);
		}

		if (NpcObj == null) {
			if (!XkGameCtrl.GetInstance().IsCartoonShootTest) {
				Debug.LogWarning("NpcObj was null");
				GameObject obj = null;
				obj.name = "null";
			}
			return;
		}
		else {
			NetworkView netView = NpcObj.GetComponent<NetworkView>();
			if (netView == null) {
				Debug.LogWarning("npc cannot find NetworkView");
				GameObject obj = null;
				obj.name = "null";
				return;
			}
		}

		XKNpcMoveCtrl npcScript = NpcObj.GetComponent<XKNpcMoveCtrl>();
		if (npcScript != null) {
			if (npcScript.IsFireMove && IsAimPlayer) {
				Debug.LogWarning("Npc.IsFireMove and SpawnPoint.IsAimPlayer is true!");
				GameObject obj = null;
				obj.name = "null";
			}

			if (npcScript.IsAniMove) {
				SetFeiJiSpawnPointInfo();
			}
			
			if (npcScript.NpcState == NpcType.FlyNpc) {
				if (NpcPath != null && NpcPath.childCount < 2) {
					Debug.LogWarning("NpcPath.childCount was wrong!");
					GameObject obj = null;
					obj.name = "null";
				}
			}
		}

		if (NpcPath != null) {
		    if (NpcPath.childCount < 1) {
				Debug.LogWarning("NpcPath.childCount was wrong! childCount = "+NpcPath.childCount);
				GameObject obj = null;
				obj.name = "null";
			}

			if (NpcPath.GetComponent<NpcPathCtrl>() == null) {
				Debug.LogWarning("NpcPath was wrong! cannot find NpcPathCtrl script");
				GameObject obj = null;
				obj.name = "null";
			}
		}

		if (FirePointGroup.Length > 0) {
			for (int i = 0; i < FirePointGroup.Length; i++) {
				if (FirePointGroup[i] == null) {
					Debug.LogWarning("FirePointGroup was wrong! index "+(i+1));
					GameObject obj = null;
					obj.name = "null";
					break;
				}
			}

			if (SpeedFangZhenFireRun <= 0f) {
				Debug.LogWarning("SpeedFangZhenFireRun was wrong! SpeedFangZhenFireRun "+SpeedFangZhenFireRun);
				GameObject obj = null;
				obj.name = "null";
			}
		}

		if (NpcSpawnLoop.Length > 0) {
			for (int i = 0; i < NpcSpawnLoop.Length; i++) {
				if (NpcSpawnLoop[i] == null) {
					Debug.LogWarning("NpcSpawnLoop was wrong! index "+(i+1));
					GameObject obj = null;
					obj.name = "null";
					break;
				}
			}
		}

		if (ChildSpawnPoint.Length > 0) {
			for (int i = 0; i < ChildSpawnPoint.Length; i++) {
				if (ChildSpawnPoint[i] == null) {
					Debug.LogWarning("ChildSpawnPoint was wrong! index "+(i+1));
					GameObject obj = null;
					obj.name = "null";
					break;
				}
			}
		}

		if (ChildSpawnPoint.Length > 0 && NpcFangZhen == null) {
			Debug.LogWarning("NpcFangZhen is null! fangZhenLength "+ChildSpawnPoint.Length);
			GameObject obj = null;
			obj.name = "null";
		}
		Invoke("CheckIsRemoveTrigger", 1f);
	}

	int IndexFeiJiPoint;
	public int GetIndexFeiJiPoint()
	{
		return IndexFeiJiPoint;
	}

	public static void HandleFeiJiNpcSpawnInfo(XKNpcMoveCtrl npcScript, int indexVal)
	{
		if (FiJiNpcPointList == null) {
			return;
		}

		if (indexVal >= FiJiNpcPointList.Count) {
			return;
		}

		//Debug.Log("HandleFeiJiNpcSpawnInfo -> indexVal "+indexVal);
		XKSpawnNpcPoint spawnScript = FiJiNpcPointList[indexVal];
		spawnScript.SaveFeiJiNpcSpawnInfo(npcScript);
	}

	public void SaveFeiJiNpcSpawnInfo(XKNpcMoveCtrl npcScript)
	{
		NpcLoopObj = npcScript.gameObject;
		npcScript.TestSpawnPoint = gameObject;
	}

	public static List<XKSpawnNpcPoint> FiJiNpcPointList;
	void SetFeiJiSpawnPointInfo()
	{
		if (FiJiNpcPointList == null) {
			FiJiNpcPointList = new List<XKSpawnNpcPoint>();
		}

		if (FiJiNpcPointList.Contains(this)) {
			return;
		}
		IndexFeiJiPoint = FiJiNpcPointList.Count;
		FiJiNpcPointList.Add(this);
	}

	public static void ClearFiJiNpcPointList()
	{
		if (FiJiNpcPointList == null) {
			return;
		}
		FiJiNpcPointList.Clear();
	}

	void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}
		CheckTestTriggerSpawnNpc();
		CheckTestTriggerRemoveNpc();

		/*if (NpcObj != null) {
			XKNpcMoveCtrl npcScript = NpcObj.GetComponent<XKNpcMoveCtrl>();
			if (npcScript != null && npcScript.NpcState == NpcType.FlyNpc && npcScript.NpcJiFen != NpcJiFenEnum.FeiJi) {
				npcScript.NpcJiFen = NpcJiFenEnum.FeiJi;
			}
		}*/

		if ((int)AniFangZhenFireRun < (int)AnimatorNameNPC.Run1
		    || (int)AniFangZhenFireRun > (int)AnimatorNameNPC.Run3) {
			AniFangZhenFireRun = AnimatorNameNPC.Run1;
		}

		if ((int)AniRootName < (int)AnimatorNameNPC.Root1
		    || (int)AniRootName > (int)AnimatorNameNPC.Root4) {
			AniRootName = AnimatorNameNPC.Root1;
		}

		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, FireDistance);

		if (NpcPath == null) {
			return;
		}

		if (NpcPath.childCount > 1) {
			Transform[] tranArray =  new Transform[2];
			tranArray[0] = transform;
			tranArray[1] = NpcPath.GetChild(0);
			iTween.DrawPath(tranArray, Color.blue);
			return;
		}
	}

	public void SpawnPointAllNpc()
	{
		if (XkGameCtrl.GetInstance().IsCartoonShootTest && NpcObj == null) {
			return;
		}
		
		if (!enabled || !gameObject.activeSelf) {
			return;
		}

		if (IsSpawnPointNpc) {
			return;
		}
		IsSpawnPointNpc = true;
		InitSpawnPointInfo();
		StartSpawnNpc();
	}

	//获取当前产生点产生的Npc.
	public GameObject GetNpcLoopObj()
	{
		return NpcLoopObj;
	}

	void StartSpawnNpc()
	{
		if (!enabled || !gameObject.activeSelf) {
			return;
		}

		if (ScreenDanHeiCtrl.IsStartGame) {
			if ((XkGameCtrl.GameModeVal == GameMode.DanJiFeiJi && PointType == SpawnPointType.DiMian)
			    || (XkGameCtrl.GameModeVal == GameMode.DanJiTanKe && PointType == SpawnPointType.KongZhong)) {
				return;
			}
			
			if (IsDoublePlayer && (!XkGameCtrl.IsActivePlayerOne || !XkGameCtrl.IsActivePlayerTwo)) {
				return;
			}
		}
		
		GameObject obj = null;
		XKHuoCheCtrl hcScript = NpcObj.GetComponent<XKHuoCheCtrl>();
		if (hcScript != null) {
			obj = SpawnPointNpc(NpcObj, transform.position, transform.rotation);
			if (obj == null) {
				//Debug.Log("StartSpawnNpc -> Cannot spawn HuoCheNpc!");
				return;
			}

			obj.transform.parent = XkGameCtrl.MissionCleanup;
			HuoCheScript = obj.GetComponent<XKHuoCheCtrl>();
			HuoCheScript.SetHuoCheInfo(this);
			//HuoCheScript.StartMoveHuoChe(NpcPath);
			NpcLoopObj = obj;
			return;
		}

		DaPaoScript = NpcObj.GetComponent<XKDaPaoCtrl>();
		if (DaPaoScript != null) {
//			Debug.Log("Spawn Cannon... ");
			obj = SpawnPointNpc(NpcObj, transform.position, transform.rotation);
			if (obj == null) {
				//Debug.Log("StartSpawnNpc -> Cannot spawn DaPaoNpc!");
				return;
			}

			if (!IsHuoCheNpc) {
				obj.transform.parent = XkGameCtrl.MissionCleanup;
			}
			else {
				obj.transform.parent = transform.parent;
			}

			//XkGameCtrl.GetInstance().AddNpcTranToList(obj.transform);
			DaPaoScript = obj.GetComponent<XKDaPaoCtrl>();
			DaPaoScript.SetSpawnPointScript(this);
			NpcLoopObj = obj;
			return;
		}

		if (IsRemoveSpawnPointNpc) {
			return;
		}

		//Debug.Log("SpawnPointAllNpc...NpcObj is "+NpcObj.name+", SpawnNpcCount "+SpawnNpcCount);
		if (ChildSpawnPoint.Length > 0) {
			//spawn fangZhenNpc
			GameObject fangZhenObj = SpawnPointNpc(NpcFangZhen, transform.position, transform.rotation);
			if (fangZhenObj == null) {
				//Debug.LogError("StartSpawnNpc -> Cannot spawn FangZhenNpc! NpcFangZhen "+NpcFangZhen.name);
				//fangZhenObj.name = "null";
				return;
			}
			NpcFangZhenScript = fangZhenObj.GetComponent<XKNpcFangZhenCtrl>();
			//NpcFangZhenScript.ActiveFangZhenNpc();
			
			XKNpcMoveCtrl npcScript = null;
			Transform fangZhenTran = fangZhenObj.transform;
			fangZhenTran.parent = XkGameCtrl.MissionCleanup;
			obj = SpawnPointNpc(NpcObj, transform.position, transform.rotation);
			if (obj == null) {
				//Debug.Log("StartSpawnNpc -> Cannot spawn FangZhenChildNpc --- 1");
				return;
			}

			npcScript = obj.GetComponent<XKNpcMoveCtrl>();
			if (npcScript != null) {
				npcScript.ActiveIsFangZhenNpc();
				npcScript.SetNpcSpawnScriptInfo(this);
			}
			obj.transform.parent = fangZhenTran;

			for (int i = 0; i < ChildSpawnPoint.Length; i++) {
				obj = SpawnPointNpc(NpcObj, ChildSpawnPoint[i].position, ChildSpawnPoint[i].rotation);
				if (obj == null) {
					//Debug.LogWarning("StartSpawnNpc -> Cannot spawn FangZhenChildNpc --- index "+i);
					break;
				}

				npcScript = obj.GetComponent<XKNpcMoveCtrl>();
				if (npcScript != null) {
					npcScript.ActiveIsFangZhenNpc();
					npcScript.SetNpcSpawnScriptInfo(this);
				}
				obj.transform.parent = fangZhenTran;
			}

			NpcFangZhenScript.SetSpawnNpcInfo(this);
			NpcLoopObj = fangZhenObj;
			return;
		}

		if (NpcLoopObj != null && SpawnMaxNpc > 1) {
			if (IsRemoveSpawnPointNpc) {
				return;
			}

			XKNpcMoveCtrl npcScript = NpcLoopObj.GetComponent<XKNpcMoveCtrl>();
			if (npcScript != null && !npcScript.GetIsDeathNPC()) {
				Invoke("StartSpawnNpc", LoopSpawnTime);
				return;
			}
		}

		if (SpawnNpcCount > 0) {
			int maxVal = NpcSpawnLoop.Length;
			int randVal = Random.Range(0, (maxVal+1));
			if (randVal == 0) {
				obj = SpawnPointNpc(NpcObj, transform.position, transform.rotation);
			}
			else {
				randVal = randVal > maxVal ? maxVal : randVal;
				obj = SpawnPointNpc(NpcSpawnLoop[randVal - 1], transform.position, transform.rotation);
			}
		}
		else {
			obj = SpawnPointNpc(NpcObj, transform.position, transform.rotation);
		}

		if (obj == null) {
			//Debug.Log("StartSpawnNpc -> Cannot spawn PuTongNpc!");
			return;
		}

		if (!IsHuoCheNpc) {
			obj.transform.parent = XkGameCtrl.MissionCleanup;
		}
		else {
			obj.transform.parent = transform.parent;
		}

		NpcScript[SpawnNpcCount] = obj.GetComponent<XKNpcMoveCtrl>();
		if (NpcScript[SpawnNpcCount] != null) {
			NpcScript[SpawnNpcCount].SetSpawnNpcInfo(this);
		}
		NpcLoopObj = obj;

		SpawnNpcCount++;
		if (SpawnNpcCount >= SpawnMaxNpc) {
			return;
		}
		
		Invoke("StartSpawnNpc", LoopSpawnTime);
	}

	public void SetIsHuoCheNpc()
	{
		IsHuoCheNpc = true;
	}

	public bool GetIsHuoCheNpc()
	{
		return IsHuoCheNpc;
	}

	GameObject SpawnPointNpc(GameObject objPrefab, Vector3 pos, Quaternion rot)
	{
		GameObject obj = null;
		obj = XKNpcSpawnListCtrl.GetInstance().GetNpcObjFromNpcDtList(objPrefab, pos, rot);
//		Debug.Log("SpawnPointNpc -> objPrefab "+objPrefab.name);
//		if (obj == null) {
//			Debug.Log("SpawnPointNpc -> obj is null");
//		}
//		else {
//			Debug.Log("SpawnPointNpc -> obj is "+obj.name);
//		}

//		if (Network.peerType == NetworkPeerType.Disconnected) {
//			obj = (GameObject)Instantiate(objPrefab, pos, rot);
//		}
//		else {
//			int playerID = int.Parse(Network.player.ToString());
//			obj = (GameObject)Network.Instantiate(objPrefab, pos, rot, playerID);
//			if (NetworkServerNet.GetInstance() != null) {
//				NetworkServerNet.GetInstance().AddNpcObjList(obj);
//			}
//		}
		return obj;
	}

	public void RemovePointAllNpc()
	{
		if (!XkGameCtrl.GetMissionCleanupIsActive()) {
			return;
		}

		if (NpcObj == null) {
			Debug.LogError("NpcObj is null");
			return;
		}

		if (ScreenDanHeiCtrl.IsStartGame) {
			if (!gameObject.activeSelf) {
				return;
			}
			gameObject.SetActive(false);
			
			if (IsRemoveSpawnPointNpc) {
				return;
			}
			IsRemoveSpawnPointNpc = true;
		}
		else {
			//reset spawnPoint info
			IsSpawnPointNpc = false;
		}

		if (IsInvoking("StartSpawnNpc")){
			CancelInvoke("StartSpawnNpc");
		}

		if (NpcFangZhenScript != null) {
			if (NpcFangZhenScript.TestSpawnPoint != gameObject) {
				//Debug.LogWarning("RemovePointAllNpc -> Cannot remove fangZhenNpc");
				return;
			}
			NpcFangZhenScript.TriggerRemovePointNpc(0);
		}
		else if (DaPaoScript != null) {
			if (DaPaoScript.TestSpawnPoint != gameObject) {
				//Debug.LogWarning("RemovePointAllNpc -> Cannot remove daPaoNpc");
				return;
			}
			DaPaoScript.OnRemoveCannon(PlayerEnum.Null, 0);
		}
		else if (HuoCheScript != null) {
			HuoCheScript.OnRemoveHuoCheObj();
		}
		else if (NpcScript != null) {
			int max = NpcScript.Length;
			for (int i = 0; i < max; i++) {
				if(NpcScript[i] != null) {
					if (NpcScript[i].TestSpawnPoint != gameObject) {
						//Debug.LogWarning("RemovePointAllNpc -> Cannot remove puTongNpc");
						continue;
					}
					NpcScript[i].TriggerRemovePointNpc(0);
				}
			}
		}
	}
	
	void InitSpawnPointInfo()
	{
//		if (NpcScript != null) {
//			return;
//		}

		SpawnNpcCount = 0;
		NpcObj.SetActive(true);
		NpcLoopObj = null;
		NpcScript = new XKNpcMoveCtrl[SpawnMaxNpc];
	}

	public void SetIsRemoveTrigger()
	{
		IsRemoveTrigger = true;
	}

	public void SetIsSpawnTrigger()
	{
		IsSpawnTrigger = true;
	}
	
	void DelayCheckAimLeaveTrigger()
	{
		if (IsPlayerAimTrigger && !IsPlayerLeaveTrigger) {
			//该产生点有瞄准触发器，但是它没有加离开触发器.
			Debug.LogWarning("The SpawnPoint has not XKTriggerPlayerAimRemove!");
			GameObject obj = null;
			obj.name = "null";
		}
	}

	void CheckIsRemoveTrigger()
	{
		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			return;
		}

		if (XkGameCtrl.IsDonotCheckError) {
			return; //test
		}

		DelayCheckAimLeaveTrigger();
		if (!IsSpawnTrigger) {
			return; //没有使用该产生点.
		}

		if (IsRemoveTrigger && IsSpawnTrigger) {
			return;
		}

		//使用了该产生点，但是没有在删除触发器中调用.
		Debug.LogWarning("This spawnPoint has no removeTrigger!");
		GameObject obj = null;
		obj.name = "null";
	}

	public void SetIsPlayerAimTrigger()
	{
		IsPlayerAimTrigger = true;
	}

	public void SetIsPlayerLeaveTrigger()
	{
		IsPlayerLeaveTrigger = true;
	}

	public void AddTestTriggerSpawnNpc(XKTriggerSpawnNpc script)
	{
		bool isFind = TestTriggerSpawnNpc.Contains(script);
		if (isFind) {
			return;
		}
		TestTriggerSpawnNpc.Add(script);
	}

	void CheckTestTriggerSpawnNpc()
	{
		bool isFind = false;
		int max = TestTriggerSpawnNpc.Count;
		for (int i = 0; i < max; i++) {
			if (TestTriggerSpawnNpc[i] != null) {
				int maxPoint = TestTriggerSpawnNpc[i].SpawnPointArray.Length;
				for (int j = 0; j < maxPoint; j++) {
					if (TestTriggerSpawnNpc[i].SpawnPointArray[j] == this) {
						isFind = true;
					}
				}

				if (!isFind) {
					TestTriggerSpawnNpc.Remove(TestTriggerSpawnNpc[i]);
					break;
				}
				isFind = false;
			}
		}
	}

	
	public void AddTestTriggerRemoveNpc(XKTriggerRemoveNpc script)
	{
		bool isFind = TestTriggerRemoveNpc.Contains(script);
		if (isFind) {
			return;
		}
		TestTriggerRemoveNpc.Add(script);
	}
	
	void CheckTestTriggerRemoveNpc()
	{
		bool isFind = false;
		int max = TestTriggerRemoveNpc.Count;
		for (int i = 0; i < max; i++) {
			if (TestTriggerRemoveNpc[i] != null) {
				int maxPoint = TestTriggerRemoveNpc[i].SpawnPointArray.Length;
				for (int j = 0; j < maxPoint; j++) {
					if (TestTriggerRemoveNpc[i].SpawnPointArray[j] == this) {
						isFind = true;
					}
				}
				
				if (!isFind) {
					TestTriggerRemoveNpc.Remove(TestTriggerRemoveNpc[i]);
					break;
				}
				isFind = false;
			}
		}
	}
}

public enum SpawnPointType
{
	Null,
	KongZhong,
	DiMian,
}
