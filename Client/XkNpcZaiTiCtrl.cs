using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XkNpcZaiTiCtrl : MonoBehaviour {
//	public NpcJiFenEnum NpcJiFen = NpcJiFenEnum.ShiBing;
	public Animator ZaiTiNpcAni;
	public RuntimeAnimatorController[] NpcAniController;
	public Rigidbody ZaiTiNpcBuWaWa;
	public GameObject AmmoPrefab;
	public AudioSource AudioNpcFire;
	public GameObject AmmoLiZiPrefab;
	public Transform AmmoSpawnTran;
	/**************************************************
	 * 该NPC为特殊NPC，在做Fire动作时，需要有两个点同时发射子弹，且每个点的子弹发射频率分别可调.
	 * 只要当该NPC做Fire动作时，即发射子弹，也就是说在一次Fire动作中可能会发射多次子弹.
	 **************************************************/
	public bool IsTeShuFireNpc;
	public float[] TimeFireAmmo;//发射子弹间隔时间.
	public GameObject[] AmmoPrefabTeShu;
	public GameObject[] AmmoLZPrefabTeShu;
	GameObject[] AmmoLZObjTeShu;
	public AudioSource[] AudioTeShuNpcFire;
	public Transform[] AmmoSpawnTranTeShu;
	public GameObject DeathExplode;
	public Transform DeathExplodePoint;
	public NpcPathCtrl AmmoMovePath; //拍摄循环动画时，使子弹可以做抛物线运动.
	bool IsZaiTiNpc;
	bool IsDeathNPC;
	GameObject NpcObj;
	Transform NpcTran;
	XKNpcMoveCtrl NpcScript;
	float[] TimeTeShuFire;
	XKSpawnNpcPoint SpawnPointScript;
	bool IsAimFeiJiPlayer;
	void Awake()
	{
		if (NpcAniController.Length > 0) {
			for (int i = 0; i < NpcAniController.Length; i++) {
				if (NpcAniController[i] == null) {
					Debug.LogWarning("NpcAniController was wrong!");
					GameObject obj = null;
					obj.name = "null";
					break;
				}
			}

			int randVal = Random.Range(0, (1+NpcAniController.Length));
			if (randVal < NpcAniController.Length && ZaiTiNpcAni != null) {
				ZaiTiNpcAni.runtimeAnimatorController = NpcAniController[randVal];
			}
			//ZaiTiNpcAni.runtimeAnimatorController = NpcAniController[0]; //test
		}

		if (IsTeShuFireNpc) {
			if (TimeFireAmmo.Length != AmmoPrefabTeShu.Length || TimeFireAmmo.Length != AmmoSpawnTranTeShu.Length) {
				Debug.LogWarning("IsTeShuFireNpc was true, but TimeFireAmmo or AmmoPrefabTeShu or AmmoSpawnTranTeShu length was wrong!");
				GameObject obj = null;
				obj.name = "null";
			}

			for (int i = 0; i < TimeFireAmmo.Length; i++) {
				if (TimeFireAmmo[i] < 0.005f) {
					TimeFireAmmo[i] = 0.005f;
				}
			}
			AmmoLZObjTeShu = new GameObject[AmmoLZPrefabTeShu.Length];
			InitNpcAmmoList();
		}

		if (ZaiTiNpcBuWaWa != null) {
			ZaiTiNpcBuWaWa.gameObject.SetActive (false);
		}
		NpcObj = gameObject;
		NpcTran = transform;
		
		if (DeathExplodePoint == null) {
			DeathExplodePoint = NpcTran;
		}
//		XKNpcHealthCtrl healthScript = GetComponent<XKNpcHealthCtrl>();
//		healthScript.SetNpcJiFen(NpcJiFen);

		NpcScript = GetComponentInParent<XKNpcMoveCtrl>();
		if (NpcScript != null) {
			if (NpcScript.IsAniMove) {
				Rigidbody rig = GetComponent<Rigidbody>();
				if (rig == null) {
					gameObject.AddComponent<Rigidbody>();
				}
				Invoke("DelaySetRigidbodyInfo", 0.2f);
			}
		}
		TimeTeShuFire = new float[TimeFireAmmo.Length];
	}

	void DelaySetRigidbodyInfo()
	{
		Rigidbody rig = GetComponent<Rigidbody>();
		if (rig != null) {
			rig.isKinematic = true;
			rig.useGravity = true;
		}
		
		if (SpawnPointScript == null) {
			SpawnPointScript = NpcScript.GetSpawnPointScript();
		}
	}

	public void SetIsZaiNpc()
	{
		IsZaiTiNpc = true;
	}
	
	public void SetFangZhenZaiTiNpcAni(int index)
	{
		if (NpcAniController.Length > 0) {
			int randVal = index % (NpcAniController.Length + 1);
			if (randVal < NpcAniController.Length && ZaiTiNpcAni != null) {
				ZaiTiNpcAni.runtimeAnimatorController = NpcAniController[randVal];
			}
		}
	}

//	public void SetIsDeathNPC()
//	{
//		IsDeathNPC = true;
//	}

	public bool GetIsDeathNPC()
	{
		return IsDeathNPC;
	}

	public void RemoveNpcObj()
	{
		if (!IsZaiTiNpc) {
			return;
		}

		if (IsDeathNPC) {
			return;
		}
		IsDeathNPC = true;

		ZaiTiNpcAni.enabled = false;
		if (DeathExplode != null) {
			GameObject objExplode = null;
			if (DeathExplodePoint != null) {
				objExplode = (GameObject)Instantiate(DeathExplode, DeathExplodePoint.position, DeathExplodePoint.rotation);
			}
			else {
				objExplode = (GameObject)Instantiate(DeathExplode, transform.position, transform.rotation);
			}
			objExplode.transform.parent = XkGameCtrl.NpcAmmoArray;
			//XkGameCtrl.CheckObjDestroyThisTimed(objExplode);

			if (Network.peerType != NetworkPeerType.Disconnected) {
				XKNpcHealthCtrl healthScript = GetComponent<XKNpcHealthCtrl>();
				if (healthScript != null) {
					healthScript.CheckHiddenNpcObjArray();
				}
			}
		}

		if (NpcObj != null) {
			if (ZaiTiNpcBuWaWa != null && !IsZaiTiNpc) {
				Rigidbody rigCom = NpcObj.GetComponent<Rigidbody>();
				if (rigCom == null) {
					rigCom = NpcObj.AddComponent<Rigidbody>();
				}
				rigCom.isKinematic = false;
				rigCom.AddForce(transform.forward * 5f, ForceMode.Impulse);
				ZaiTiNpcBuWaWa.gameObject.SetActive(true);
				ZaiTiNpcBuWaWa.AddForce(transform.forward * 95f, ForceMode.Impulse);
				//Destroy(NpcObj, 2f);
			}
			else {
				Rigidbody rigCom = NpcObj.GetComponent<Rigidbody>();
				if (rigCom == null) {
					rigCom = NpcObj.AddComponent<Rigidbody>();
				}
				rigCom.isKinematic = false;
				rigCom.AddForce(transform.forward * 5f, ForceMode.Impulse);
				//Destroy(NpcObj, 2f);
			}
		}
		ResetZaiTiNpcInfo();
	}

	public void SetNpcIsDoFire(NpcMark script)
	{
		//Debug.Log("SetNpcIsDoFire -> IsFireFeiJiNpc "+script.IsFireFeiJiNpc);
		if (!NpcScript.IsAniMove) {
			return;
		}

		if (SpawnPointScript == null) {
			return;
		}

		Transform npcPath = SpawnPointScript.NpcPath;
		Transform markPar = script.transform.parent;
		if (npcPath != markPar) {
			return;
		}
		//Debug.Log("***********SetNpcIsDoFire -> IsFireFeiJiNpc "+script.IsFireFeiJiNpc);
		NpcScript.SetIsDoFireAnimation(script.IsFireFeiJiNpc);
		NpcScript.SetFeiJiMarkInfo(script);
	}

	void Update()
	{
		if (!XkGameCtrl.IsMoveOnPlayerDeath) {
			if (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo) {
				return;
			}
		}

		if (IsDeathNPC) {
			return;
		}

		if (!IsTeShuFireNpc) {
			return;
		}

		if (TimeTeShuFire.Length < 1) {
			return;
		}

		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}

		if (JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()
		    || GameOverCtrl.IsShowGameOver) {
			return;
		}

		if (!NpcScript.GetIsDoFireAnimation()) {
			return;
		}

		GameObject obj = null;
		Transform tran = null;
		for (int i = 0; i < TimeTeShuFire.Length; i++) {
			TimeTeShuFire[i] += Time.deltaTime;
			if (TimeTeShuFire[i] >= TimeFireAmmo[i]) {
				TimeTeShuFire[i] = 0f; //fire ammo
//				Debug.Log("teShuFireNpc -> i = "+i);

				if (i < AudioTeShuNpcFire.Length && AudioTeShuNpcFire[i] != null) {
					if (AudioTeShuNpcFire[i].isPlaying) {
						AudioTeShuNpcFire[i].Stop();
					}
					AudioTeShuNpcFire[i].Play();
				}

				if (AmmoLZPrefabTeShu != null && AmmoLZPrefabTeShu[i] != null && AmmoLZObjTeShu[i] == null) {
					obj = (GameObject)Instantiate(AmmoLZPrefabTeShu[i],
					                              AmmoSpawnTranTeShu[i].position, AmmoSpawnTranTeShu[i].rotation);
					
					tran = obj.transform;
					AmmoLZObjTeShu[i] = obj;
					XkGameCtrl.CheckObjDestroyThisTimed(obj);
					tran.parent = AmmoSpawnTranTeShu[i];
				}
				
				PlayerAmmoCtrl ammoPlayerScript = AmmoPrefabTeShu[i].GetComponent<PlayerAmmoCtrl>();
				if (ammoPlayerScript != null && !XkGameCtrl.GetInstance().IsCartoonShootTest) {
					continue;
				}

				obj = GetNpcAmmoFromList(AmmoSpawnTranTeShu[i], AmmoPrefabTeShu[i]);
				if (obj == null) {
					return;
				}
				tran = obj.transform;
				tran.parent = XkGameCtrl.NpcAmmoArray;

				NpcAmmoCtrl ammoNpcScript = obj.GetComponent<NpcAmmoCtrl>();
				if (ammoNpcScript != null) {
					ammoNpcScript.SetNpcScriptInfo(NpcScript);
					ammoNpcScript.SetIsAimFeiJiPlayer(IsAimFeiJiPlayer);
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
						ammoScript.StartMoveAmmo(firePos, PlayerEnum.Null, AmmoMovePath);
					}
				}

//				if (AmmoLiZiPrefab != null) {
//					obj = (GameObject)Instantiate(AmmoLiZiPrefab, AmmoSpawnTran.position, AmmoSpawnTran.rotation);
//					tran = obj.transform;
//					tran.parent = XkGameCtrl.MissionCleanup;
//				}
			}
		}
	}

	public void SetIsAimFeiJiPlayer(bool isAim)
	{
		IsAimFeiJiPlayer = isAim;
	}
	
	/// <summary>
	/// The ammo list.
	/// </summary>
	public List<NpcAmmoCtrl> AmmoList;
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
//		Debug.Log("XkNpcZaiTiCtrl::ClearNpcAmmoList -> NpcAmmoCount "+AmmoList.Count);
		
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
	
	GameObject GetNpcAmmoFromList(Transform spawnPoint, GameObject ammoPrefabObj)
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
			objAmmo = SpawnNpcAmmo(spawnPoint, ammoPrefabObj);
			HandleAmmoList( objAmmo.GetComponent<NpcAmmoCtrl>() );
		}
		
		if (objAmmo != null) {
			Transform tranAmmo = objAmmo.transform;
			tranAmmo.position = spawnPoint.position;
			tranAmmo.rotation = spawnPoint.rotation;
		}
		return objAmmo;
	}
	
	GameObject SpawnNpcAmmo(Transform spawnPoint, GameObject ammoPrefabObj)
	{
		return (GameObject)Instantiate(ammoPrefabObj, spawnPoint.position, spawnPoint.rotation);
	}

	public void ResetNpcZaiTiSomeInfo()
	{
		if (ZaiTiNpcBuWaWa != null) {
			GameObject objBuWaWa = ZaiTiNpcBuWaWa.gameObject;
			objBuWaWa.SetActive(false);
			Transform tranBuWaWa = ZaiTiNpcBuWaWa.transform;
			tranBuWaWa.localPosition = Vector3.zero;
			tranBuWaWa.localEulerAngles = Vector3.zero;
		}
	}

	void ResetZaiTiNpcInfo()
	{
		IsClearNpcAmmo = false;
		IsAimFeiJiPlayer = false;
		IsDeathNPC = false;
		IsZaiTiNpc = false;
	}
}