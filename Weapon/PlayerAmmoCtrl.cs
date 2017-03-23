using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAmmoCtrl : MonoBehaviour {
	public PlayerAmmoType AmmoType = PlayerAmmoType.PuTongAmmo;
	public GameObject AmmoExplode;
	[Range(1f, 4000f)] public float MvSpeed = 50f;
	[Range(1f, 1000f)] public float AmmoDamageDis = 50f;
	[Range(1f, 100f)] public float LiveTime = 4f;
	public GameObject MetalParticle;		//金属.
	public GameObject ConcreteParticle;		//混凝土.
	public GameObject DirtParticle;			//土地.
	public GameObject WoodParticle;			//树木.
	public GameObject WaterParticle;		//水.
	public GameObject SandParticle;			//沙滩.
	public GameObject GlassParticle;		//玻璃.
	float SpawnAmmoTime;
	GameObject ObjAmmo;
	Transform AmmoTran;
	PlayerEnum PlayerState = PlayerEnum.Null;
	public static LayerMask PlayerAmmoHitLayer;
	Vector3 AmmoStartPos;
	Vector3 AmmoEndPos;
	bool IsHandleRpc;
	GameObject HitNpcObj;
	bool IsDestroyAmmo;
	bool IsDonotHurtNpc;
	TrailRenderer TrailScript;
	float TrailTime = 3f;
	void Awake()
	{
		if (TrailScript == null) {
			TrailScript = GetComponentInChildren<TrailRenderer>();
			TrailScript.castShadows = false;
			TrailScript.receiveShadows = false;
			TrailTime = TrailScript.time;
		}
		AmmoTran = transform;
		ObjAmmo = gameObject;
		AmmoTran.parent = XkGameCtrl.PlayerAmmoArray;
		SpawnAmmoTime = Time.realtimeSinceStartup;
	}
	
	void Update()
	{
		if (Time.frameCount % 4 != 0) {
			return;
		}

		if (!IsHandleRpc) {
			return;
		}
		
		if (IsDestroyAmmo) {
			return;
		}
		
		if (Time.realtimeSinceStartup - SpawnAmmoTime > LiveTime) {
			IsAmmoSpawnParticle = false;
			//Debug.Log("*********************test player ammo -- "+AmmoType);
			MoveAmmoOnCompelteITween();
			return;
		}
	}

	public void StartMoveAmmo(Vector3 firePos, PlayerEnum playerSt,
	                          NpcPathCtrl ammoMovePath = null, GameObject hitObjNpc = null)
	{
		HitNpcObj = hitObjNpc;
		ObjAmmo = gameObject;
		if (!ObjAmmo.activeSelf) {
			ObjAmmo.SetActive(true);
			IsDestroyAmmo = false;
		}
		AmmoTran = transform;
		PlayerState = playerSt;
		MoveAmmoByItween(firePos, ammoMovePath);
		IsHandleRpc = true;
	}

	void ResetTrailScriptInfo()
	{
		gameObject.SetActive(false);
		if (TrailScript == null) {
			return;
		}
		TrailScript.time = TrailTime;
	}
	
	void MoveAmmoByItween(Vector3 firePos, NpcPathCtrl ammoMovePath)
	{
		IsAmmoSpawnParticle = true;
		SpawnAmmoTime = Time.realtimeSinceStartup;
		Vector3[] posArray = new Vector3[2];
		if (ammoMovePath == null) {
			posArray[0] = AmmoTran.position;
			posArray[1] = firePos;
			AmmoStartPos = AmmoTran.position;
			iTween.MoveTo(ObjAmmo, iTween.Hash("path", posArray,
			                                   "speed", MvSpeed,
			                                   "orienttopath", true,
			                                   "easeType", iTween.EaseType.linear,
			                                   "oncomplete", "MoveAmmoOnCompelteITween"));
		}
		else {
			int countMark = ammoMovePath.transform.childCount;
			Transform[] tranArray = ammoMovePath.transform.GetComponentsInChildren<Transform>();
			List<Transform> nodesTran = new List<Transform>(tranArray){};
			nodesTran.Remove(ammoMovePath.transform);
			transform.position = nodesTran[0].position;
			transform.rotation = nodesTran[0].rotation;
			firePos = nodesTran[countMark-1].position;
			AmmoStartPos = nodesTran[countMark-2].position;
			iTween.MoveTo(ObjAmmo, iTween.Hash("path", nodesTran.ToArray(),
			                                   "speed", MvSpeed,
			                                   "orienttopath", true,
			                                   "easeType", iTween.EaseType.linear,
			                                   "oncomplete", "MoveAmmoOnCompelteITween"));
		}
		AmmoEndPos = firePos;
	}

	void SpawnAmmoParticleObj()
	{
		GameObject objParticle = null;
		GameObject obj = null;
		Transform tran = null;
		Vector3 hitPos = transform.position;

		RaycastHit hit;
		if (!IsHandleRpc) {
			AmmoEndPos = transform.position;
			AmmoStartPos = transform.position - transform.forward * 3f;
			Physics.Raycast(AmmoStartPos, transform.forward, out hit, 1000f, PlayerAmmoHitLayer);
			if (hit.collider != null) {
				AmmoEndPos = hit.point;
			}
		}

		Vector3 forwardVal = Vector3.Normalize(AmmoEndPos - AmmoStartPos);
		if (AmmoType == PlayerAmmoType.PuTongAmmo) {
			float disVal = Vector3.Distance(AmmoEndPos, AmmoStartPos) + 10f;
			Physics.Raycast(AmmoStartPos, forwardVal, out hit, disVal, PlayerAmmoHitLayer);
			if (hit.collider != null) {
				hitPos = hit.point;
				XKAmmoParticleCtrl ammoParticleScript = hit.collider.GetComponent<XKAmmoParticleCtrl>();
				if (ammoParticleScript != null && ammoParticleScript.PuTongAmmoLZ != null) {
					objParticle = ammoParticleScript.PuTongAmmoLZ;
				}
				else {
					string tagHitObj = hit.collider.tag;
					switch (tagHitObj) {
					case "metal":
						if (MetalParticle != null) {
							objParticle = MetalParticle;
						}
						break;
						
					case "concrete":
						if (ConcreteParticle != null) {
							objParticle = ConcreteParticle;
						}
						break;
						
					case "dirt":
						if (DirtParticle != null) {
							objParticle = DirtParticle;
						}
						break;
						
					case "wood":
						if (WoodParticle != null) {
							objParticle = WoodParticle;
						}
						break;
						
					case "water":
						if (WaterParticle != null) {
							objParticle = WaterParticle;
						}
						break;
						
					case "sand":
						if (SandParticle != null) {
							objParticle = SandParticle;
						}
						break;
						
					case "glass":
						if (GlassParticle != null) {
							objParticle = GlassParticle;
						}
						break;
					}
					
					if (objParticle == null) {
						objParticle = AmmoExplode;
					}
				}

				if (IsHandleRpc
				    && !IsDonotHurtNpc
				    && Network.peerType != NetworkPeerType.Server
				    && AmmoType == PlayerAmmoType.PuTongAmmo) {
					XKNpcHealthCtrl healthScript = hit.collider.GetComponent<XKNpcHealthCtrl>();
					if (healthScript != null) {
						if (HitNpcObj == null || HitNpcObj != hit.collider.gameObject) {
							//Debug.Log("playerAmmo hit npc, npc is "+hit.collider.name);
							healthScript.OnDamageNpc(AmmoType, PlayerState);
						}
					}
				}
			}
			else {
				objParticle = AmmoExplode;
			}
		}
		else {
			float disVal = Vector3.Distance(AmmoEndPos, AmmoStartPos) + 10f;
			Physics.Raycast(AmmoStartPos, forwardVal, out hit, disVal, PlayerAmmoHitLayer);
			if (hit.collider != null) {
//				if (AmmoType == PlayerAmmoType.GaoBaoAmmo) {
//					Debug.Log("hit.collider "+hit.collider.name);
//				}
				hitPos = hit.point;
				string tagHitObj = hit.collider.tag;
				switch (tagHitObj) {
				case "dirt":
					if (DirtParticle != null) {
						objParticle = DirtParticle;
					}
					break;

				case "water":
					if (WaterParticle != null) {
						objParticle = WaterParticle;
					}
					break;
				}
				
				if (objParticle == null) {
					objParticle = AmmoExplode;
				}
			}
			else {
				objParticle = AmmoExplode;
			}
		}
		
		if (objParticle == null) {
			return;
		}
		
		if (AmmoType == PlayerAmmoType.DaoDanAmmo) {
			Vector3 AmmoPos = transform.position - (transform.forward * 3f);
			Physics.Raycast(AmmoPos, forwardVal, out hit, 13f, XkGameCtrl.GetInstance().LandLayer);
			if (hit.collider != null) {
				hitPos = hit.point;
				Vector3 normalVal = hit.normal;
				Quaternion rotVal = Quaternion.identity;
				if (normalVal.magnitude > 0.001f) {
					rotVal = Quaternion.LookRotation(normalVal);
				}
				obj = (GameObject)Instantiate(objParticle, hitPos, rotVal);
				obj.transform.up = normalVal;
			}
			else {
				obj = (GameObject)Instantiate(objParticle, hitPos, transform.rotation);
			}
		}
		else {
			obj = (GameObject)Instantiate(objParticle, hitPos, transform.rotation);
		}
		tran = obj.transform;
		tran.parent = XkGameCtrl.PlayerAmmoArray;
		XkGameCtrl.CheckObjDestroyThisTimed(obj);

		XkAmmoTieHuaCtrl tieHuaScript = obj.GetComponent<XkAmmoTieHuaCtrl>();
		if (tieHuaScript != null && tieHuaScript.TieHuaTran != null) {
			Transform tieHuaTran = tieHuaScript.TieHuaTran;
			Vector3 AmmoPos = transform.position - (transform.forward * 3f);
			Physics.Raycast(AmmoPos, forwardVal, out hit, 13f, XkGameCtrl.GetInstance().PlayerAmmoHitLayer);
			if (hit.collider != null) {
				tieHuaTran.up = hit.normal;
			}
		}
	}

	bool IsAmmoSpawnParticle = true;
	void MoveAmmoOnCompelteITween()
	{
		if (IsDestroyAmmo) {
			return;
		}
		IsDestroyAmmo = true;
		//Debug.Log("MoveAmmoOnCompelteITween...");
		if (IsAmmoSpawnParticle) {
			SpawnAmmoParticleObj();
		}
		
		iTween itweenScript = GetComponent<iTween>();
		if (itweenScript != null) {
			itweenScript.isRunning = false;
			itweenScript.isPaused = true;
			itweenScript.enabled = false;
		}

		if (!IsDonotHurtNpc) {
			CheckAmmoDamageNpc();
		}

		if (AmmoType == PlayerAmmoType.PuTongAmmo || AmmoType == PlayerAmmoType.GaoBaoAmmo) {
			//Destroy(ObjAmmo, 0.1f); //test
			if (!IsInvoking("DaleyHiddenPlayerAmmo")) {
				Invoke("DaleyHiddenPlayerAmmo", 0.2f);
			}
		}
		else {
			Destroy(ObjAmmo, 0.1f);
		}
	}

	void DaleyHiddenPlayerAmmo()
	{
		if (TrailScript != null) {
			TrailScript.time = 0f;
			Invoke("ResetTrailScriptInfo", 0.1f);
		}
		else {
			gameObject.SetActive(false);
		}
	}

	void CheckAmmoDamageNpc()
	{
		if (AmmoType == PlayerAmmoType.PuTongAmmo && XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
			return;
		}
		
		XKNpcHealthCtrl healthScript = null;
		float disDamage = AmmoDamageDis;
		Collider[] hits = Physics.OverlapSphere(AmmoTran.position, disDamage, PlayerAmmoHitLayer);
		List<XKNpcHealthCtrl> healthList = new List<XKNpcHealthCtrl>();
		foreach (Collider c in hits) {
			// Don't collide with triggers
			if (c.isTrigger) {
				continue;
			}

			healthScript = c.gameObject.GetComponent<XKNpcHealthCtrl>();
			if (healthScript != null) {
				//Debug.Log("test fix nnn *** "+healthScript.name);
				healthScript.OnDamageNpc(AmmoType, PlayerState);
				healthList.Add(healthScript);
			}
		}
		healthList.Clear();
		healthList = null;
	}
	
	public void SetIsDonotHurtNpc(bool isDonotHurt)
	{
		IsDonotHurtNpc = isDonotHurt;
	}
}