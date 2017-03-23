using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XKNpcAnimatorCtrl : MonoBehaviour {

	Animator AnimatorCom;
	public GameObject AmmoPrefab;
	public GameObject AmmoLiZiPrefab;
	public Transform AmmoSpawnTran;
	AudioSource AudioNpcFire;
	XKNpcMoveCtrl NpcScript;
	XkNpcZaiTiCtrl ZaiTiScriptVal;
	bool IsStopAnimation;
	int CountFireAction;
	public int CountHuanDan = 10;
	string AnimationNameCur = "";
	bool IsDoHuanDanAction;
	bool IsDoRunFireAction;
	int CountFireRunVal;
	int CountFireRun = 5;
	bool IsAimFeiJiPlayer;
	GameObject AmmoLiZiObj;
	// Use this for initialization
	void Awake()
	{
		InitNpcAmmoList();
		CountHuanDan = Random.Range(3, 8);
		AnimatorCom = GetComponent<Animator>();
		NpcScript = GetComponentInParent<XKNpcMoveCtrl>();
	}

	void Update()
	{
		if (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo) {
			if (!XkGameCtrl.IsMoveOnPlayerDeath) {
				if (!IsStopAnimation) {
					IsStopAnimation = true;
					AnimatorCom.speed = 0f;
				}
			}
		}
		else {
			if (IsStopAnimation) {
				IsStopAnimation = false;
				AnimatorCom.speed = 1f;
			}
		}

		if (AnimatorCom != null
		    && !AnimatorCom.enabled
		    && AnimatorCom.runtimeAnimatorController != null
		    && NpcScript != null
		    && !NpcScript.GetIsDeathNPC()) {
			if (!NpcScript.IsAniMove) {
				AnimatorCom.enabled = true;
			}
		}
	}

	public void PlayNpcAnimatoin(string aniName)
	{
		//Debug.Log("aniName "+aniName);
		if (aniName == "") {
			return;
		}

		if (AnimatorCom == null || AnimatorCom.runtimeAnimatorController == null) {
			//Debug.LogWarning("AnimatorCom or runtimeAnimatorController is null, name "+gameObject.name);
			return;
		}

		if (!gameObject.activeSelf) {
			gameObject.SetActive(true);
		}

		if (!AnimatorCom.enabled) {
			AnimatorCom.enabled = true;
		}

		bool isHuanDan = aniName.StartsWith("HuanDan");
		if (!isHuanDan) {
			ResetNpcAnimation();
			AnimationNameCur = aniName;
		}
		AnimatorCom.SetBool(aniName, true);
	}

	public void ResetIsDoRunFireAction()
	{
		IsDoRunFireAction = false;
	}

	public void ResetNpcAnimation()
	{
		if (!gameObject.activeSelf) {
			return;
		}

		if (AnimatorCom == null || AnimatorCom.runtimeAnimatorController == null) {
			Debug.LogWarning("AnimatorCom is null, name " + gameObject.name);
			AnimatorCom.name = "null";
			return;
		}

		AnimatorCom.SetBool("Root1", false);
		AnimatorCom.SetBool("Root2", false);
		AnimatorCom.SetBool("Root3", false);
		AnimatorCom.SetBool("Root4", false);
		AnimatorCom.SetBool("Run1", false);
		AnimatorCom.SetBool("Run2", false);
		AnimatorCom.SetBool("Run3", false);
		AnimatorCom.SetBool("Fire1", false);
		AnimatorCom.SetBool("Fire2", false);
		AnimatorCom.SetBool("Fire3", false);
		AnimatorCom.SetBool("Fire4", false);
		AnimatorCom.SetBool("Fire5", false);
		AnimatorCom.SetBool("Fire6", false);
		AnimatorCom.SetBool("HuanDan1", false);
		AnimatorCom.SetBool("HuanDan2", false);
		AnimatorCom.SetBool("HuanDan3", false);
		AnimatorCom.SetBool("HuanDan4", false);
		AnimatorCom.SetBool("HuanDan5", false);
		AnimatorCom.SetBool("HuanDan6", false);
	}

	public void SetAmmoPrefabVal(XKNpcMoveCtrl scriptVal)
	{
		if (ZaiTiScriptVal == null) {
			ZaiTiScriptVal = GetComponent<XkNpcZaiTiCtrl>();
		}

		if (ZaiTiScriptVal != null) {
			AmmoPrefab = ZaiTiScriptVal.AmmoPrefab;
			AmmoLiZiPrefab = ZaiTiScriptVal.AmmoLiZiPrefab;
			AmmoSpawnTran = ZaiTiScriptVal.AmmoSpawnTran;
			AudioNpcFire = ZaiTiScriptVal.AudioNpcFire;
		}
		NpcScript = scriptVal;
		SetCountHuanDan(scriptVal.CountHuanDan);

		//SetSpawnPointScript( NpcScript.GetSpawnPointScript() );
	}

	public void ResetFireAnimationSpeed()
	{
		if (!XkGameCtrl.IsMoveOnPlayerDeath) {
			if (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo) {
				return;
			}
		}

		if (AnimatorCom.speed == 1f) {
			return;
		}
		AnimatorCom.speed = 1f;
	}

	void OnTriggerFireAimAnimation()
	{
		if (!NpcScript.GetIsAimPlayerByFire()) {
			return;
		}
		//Debug.Log("OnTriggerFireAimAnimation -> name "+gameObject.name);
		AnimatorCom.speed = 0f;
	}

	public void SetIsAimFeiJiPlayer(bool isAim)
	{
		IsDoHuanDanAction = false;
		IsAimFeiJiPlayer = isAim;
	}

	void OnTriggerFireAnimation()
	{
		//return; //test
		//Debug.Log("OnTriggerFireAnimation**NpcName "+AnimatorCom.name);
		if (!XkGameCtrl.IsMoveOnPlayerDeath) {
			if (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo) {
				return;
			}
		}
				
		if (JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()
		    || GameOverCtrl.IsShowGameOver) {
			return;
		}

		if (Network.peerType == NetworkPeerType.Client) {
			if (!IsDoHuanDanAction) {
				StartSpawnNpcAmmo();
				if (CountHuanDan > 0) {
					CountFireAction++;
					if (CountHuanDan <= CountFireAction) {
						CountFireAction = 0;
						PlayNPCHuanDanAction(); //Play huanDan action}
					}
				}
			}
			return;
		}

		int rv = AddCountFireAction();
		if (Network.peerType == NetworkPeerType.Server) {
			if (XkGameCtrl.CountNpcAmmo >= XkGameCtrl.AmmoNumMaxNpc) {
				return;
			}
		}

		if (rv != -1) {
			StartSpawnNpcAmmo();
		}
	}

	void StartSpawnNpcAmmo()
	{
		if (AudioNpcFire != null) {
			if (AudioNpcFire.isPlaying) {
				AudioNpcFire.Stop();
			}
			AudioNpcFire.Play();
		}

		GameObject obj = null;
		Transform tran = null;
		if (AmmoLiZiPrefab != null && AmmoLiZiObj == null) {
			obj = (GameObject)Instantiate(AmmoLiZiPrefab, AmmoSpawnTran.position, AmmoSpawnTran.rotation);
			tran = obj.transform;
			tran.parent = XkGameCtrl.NpcAmmoArray;
			AmmoLiZiObj = obj;
			XkGameCtrl.CheckObjDestroyThisTimed(obj);
		}

		PlayerAmmoCtrl ammoPlayerScript = AmmoPrefab.GetComponent<PlayerAmmoCtrl>();
		if (ammoPlayerScript != null && !XkGameCtrl.GetInstance().IsCartoonShootTest) {
			return;
		}

		obj = GetNpcAmmoFromList(AmmoSpawnTran);
		if (obj == null) {
			return;
		}

		tran = obj.transform;
		tran.parent = XkGameCtrl.NpcAmmoArray;
		NpcAmmoCtrl AmmoScript = obj.GetComponent<NpcAmmoCtrl>();
		if (AmmoScript != null) {
			AmmoScript.SetNpcScriptInfo(NpcScript);
			AmmoScript.SetIsAimFeiJiPlayer(IsAimFeiJiPlayer);
		}
		else {
			PlayerAmmoCtrl ammoScript = obj.GetComponent<PlayerAmmoCtrl>();
			if (ammoScript != null) {
				Vector3 startPos = tran.position;
				Vector3 firePos = tran.position;
				Vector3 ammoForward = tran.forward;
				firePos = Random.Range(300f, 400f) * ammoForward + startPos;
				float fireDisVal = Vector3.Distance(firePos, startPos);
				RaycastHit hit;
				LayerMask FireLayer = XkGameCtrl.GetInstance().PlayerAmmoHitLayer;
				if (Physics.Raycast(startPos, ammoForward, out hit, fireDisVal, FireLayer.value)) {
					//Debug.Log("npc fire PlayerAmmo, fire obj -> "+hit.collider.name);
					firePos = hit.point;
					XKNpcHealthCtrl healthScript = hit.collider.GetComponent<XKNpcHealthCtrl>();
					if (healthScript != null) {
						healthScript.OnDamageNpc(ammoScript.AmmoType, PlayerEnum.Null);
					}
					
					BuJiBaoCtrl buJiBaoScript = hit.collider.GetComponent<BuJiBaoCtrl>();
					if (buJiBaoScript != null) {
						buJiBaoScript.RemoveBuJiBao(PlayerEnum.Null); //buJiBaoScript
					}
				}
				ammoScript.StartMoveAmmo(firePos, PlayerEnum.Null);
			}
		}
	}
	
	void OnTriggerHuanDanAnimation()
	{
//		Debug.Log("OnTriggerHuanDanAnimation...");
		RandomPlayNpcFireAction();
		IsDoHuanDanAction = false;
	}

	void RandomPlayNpcFireAction()
	{
		ResetHuanDanAnimation();
//		if (ZaiTiScriptVal != null) {
//			ZaiTiScriptVal.MakeNpcDoFireAnimation();
//		}
//		else {
//			NpcScript.MakeNpcDoFireAnimation();
//		}
	}

	void ResetHuanDanAnimation()
	{
		AnimatorCom.SetBool("HuanDan1", false);
		AnimatorCom.SetBool("HuanDan2", false);
		AnimatorCom.SetBool("HuanDan3", false);
		AnimatorCom.SetBool("HuanDan4", false);
		AnimatorCom.SetBool("HuanDan5", false);
		AnimatorCom.SetBool("HuanDan6", false);
	}

//	static int TestNumVal;
	//int TestHuanDanNpc;
	public void SetCountHuanDan(int val)
	{
		val = val > 1 ? Random.Range(val, (val + 5)) : val;
		CountHuanDan = val;

//		if (CountHuanDan == 1) {
//			TestNumVal++;
			//TestHuanDanNpc = TestNumVal;
//		}
	}

	int AddCountFireAction()
	{
		if (IsDoHuanDanAction) {
//			Debug.LogWarning("IsDoHuanDanAction is true");
			return -1;
		}
		
		if (IsDoRunFireAction) {
//			Debug.LogWarning("IsDoRunFireAction is true");
			return -1;
		}
		
		CountFireAction++;
		FirePoint firePointScript = NpcScript.GetFirePointScript();
		if (firePointScript != null) {
			CountFireRun = firePointScript.CountFire;
			CountFireRunVal++;
//			Debug.Log("CountFireAction "+CountFireAction+", CountFireRunVal "+CountFireRunVal);
			if (CountFireRun <= CountFireRunVal) {
				//Play Run_Fire Action
				CountFireRunVal = 0;
				MakeNpcDoActionRun3();
				NpcScript.MakeNpcMoveFirePoint();
				return 0;
			}
		}

		if (CountHuanDan <= 0) {
			return 0;
		}

		if (CountHuanDan <= CountFireAction) {
			//Play huanDan action
			CountFireAction = 0;
			PlayNPCHuanDanAction();
		}
		return 0;
	}

	void MakeNpcDoActionRun3()
	{
		IsDoHuanDanAction = false;
		IsDoRunFireAction = true;
		ResetNpcAnimation();
		NpcScript.NetNpcPlayAnimation(this, AnimatorNameNPC.Run3.ToString());
	}

	void PlayNPCHuanDanAction()
	{
		IsDoHuanDanAction = true;
		string aniName = "";
		if (AnimationNameCur == AnimatorNameNPC.Fire1.ToString()) {
			aniName = AnimatorNameNPC.HuanDan1.ToString();
		}
		else if (AnimationNameCur == AnimatorNameNPC.Fire2.ToString()) {
			aniName = AnimatorNameNPC.HuanDan2.ToString();
		}
		else if (AnimationNameCur == AnimatorNameNPC.Fire3.ToString()) {
			aniName = AnimatorNameNPC.HuanDan3.ToString();
		}
		else if (AnimationNameCur == AnimatorNameNPC.Fire4.ToString()) {
			aniName = AnimatorNameNPC.HuanDan4.ToString();
		}
		else if (AnimationNameCur == AnimatorNameNPC.Fire5.ToString()) {
			aniName = AnimatorNameNPC.HuanDan5.ToString();
		}
		else if (AnimationNameCur == AnimatorNameNPC.Fire6.ToString()) {
			aniName = AnimatorNameNPC.HuanDan6.ToString();
		}

		NpcScript.NetNpcPlayAnimation(this, aniName);
	}

	/// <summary>
	/// The ammo list.
	/// </summary>
	List<NpcAmmoCtrl> AmmoList;
	bool IsClearNpcAmmo;
	void InitNpcAmmoList()
	{
		if (AmmoList != null) {
			AmmoList.Clear();
		}
		AmmoList = new List<NpcAmmoCtrl>(5);
	}
	
	void HandleAmmoList(NpcAmmoCtrl scriptAmmo)
	{
		if (AmmoList.Contains(scriptAmmo)) {
			return;
		}
		AmmoList.Add(scriptAmmo);
	}
	
	public void ClearNpcAmmoList()
	{
		if (IsClearNpcAmmo) {
			return;
		}
		IsClearNpcAmmo = true;
//		Debug.Log("XKNpcAnimatorCtrl::ClearNpcAmmoList -> NpcAmmoCount "+AmmoList.Count);

		if (AmmoList == null || AmmoList.Count < 1) {
			return;
		}
		
		NpcAmmoCtrl[] ammoListArray = AmmoList.ToArray();
		int max = ammoListArray.Length;
		for (int i = 0; i < max; i++) {
			if (ammoListArray[i] != null) {
				ammoListArray[i].MakeNpcAmmoDestory();
			}
		}
		AmmoList.Clear();
	}
	
	GameObject GetNpcAmmoFromList(Transform spawnPoint)
	{
		if (IsClearNpcAmmo) {
			return null;
		}

		GameObject objAmmo = null;
		int max = AmmoList.Count;
		for (int i = 0; i < max; i++) {
			if (AmmoList[i] != null && !AmmoList[i].gameObject.activeSelf) {
				objAmmo = AmmoList[i].gameObject;
				break;
			}
		}
		
		if (objAmmo == null) {
			objAmmo = SpawnNpcAmmo(spawnPoint);
			HandleAmmoList( objAmmo.GetComponent<NpcAmmoCtrl>() );
		}
		
		if (objAmmo != null) {
			Transform tranAmmo = objAmmo.transform;
			tranAmmo.position = spawnPoint.position;
			tranAmmo.rotation = spawnPoint.rotation;
//			if (!objAmmo.activeSelf) {
//				objAmmo.SetActive(true);
//			}
		}
		return objAmmo;
	}
	
	GameObject SpawnNpcAmmo(Transform spawnPoint)
	{
		return (GameObject)Instantiate(AmmoPrefab, spawnPoint.position, spawnPoint.rotation);
	}
}