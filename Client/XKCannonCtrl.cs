using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XKCannonCtrl : MonoBehaviour {
	public GameObject DeathExplode;
	public Transform DeathExplodePoint;
	public GameObject DaPaoAmmo;
	public AudioSource AudioCannonFire;
	public GameObject DaPaoAmmoLiZi;
	GameObject[] DaPaoAmmoLiZiObj;
	public Transform PaoGuan;
	public Transform[] SpawnAmmoPoint;
	[Range(1f, 1000f)] public float FireDis = 50f;
	//炮管角度控制.
	/*[Range(275f, 355f)]public float MaxPaoGuanJDVal = 355f;
	[Range(275f, 355f)]public float MinPaoGuanJDVal = 275f;*/
	[Range(-90f, 90f)]public float UpPaoGuanJDVal = -45f;
	[Range(-90f, 90f)]public float DownPaoGuanJDVal = 45f;
	//炮管旋转速度.
	[Range(0.000001f, 100f)]public float PaoGuanSDVal = 2f;
	//炮管伸缩速度.
	[Range(0.03f, 1f)]public float PaoGuanShenSuoSD = 0.03f;
	//炮身角度控制.
	[Range(0f, 180f)]public float MaxPaoShenJDVal = 10f;
	[Range(0f, -180f)]public float MinPaoShenJDVal = -10f;
	//炮身旋转速度.
	[Range(0.000001f, 100f)]public float PaoShenSDVal = 2f;
	[Range(0f, 100f)]public float PaoGuanZhenFu = 0.9f;
	//单管大炮做fire的间隔时间.
	[Range(0.06f, 100f)]public float TimeDanGuanFire = 1f;
	//多管大炮各个炮管产生子弹的间隔时间.
	[Range(0.03f, 10f)] public float TimeAmmoUnit = 0.1f;
	//多管炮弹每次发射炮弹的间隔时间.
	[Range(0.1f, 50f)] public float TimeAmmoWait = 1f;
	public GameObject[] DaPaoHiddenArray;
	public bool IsHiddenAmmoSpawnPoint; //卡丘沙隐藏子弹发射点逻辑控制.
	public GameObject KaQiuShaTSLiZi; //卡丘沙开火前的特殊粒子.
	public NpcPathCtrl AmmoMovePath; //拍摄循环动画时，使子弹可以做抛物线运动.
	Transform CannonTran;
	bool IsDoFireAnimation;
	bool IsDeathNpc;
	bool IsStopAnimation;
	bool IsDouGuanDaPao;
	bool IsPlayPaoGuanAnimation;
	bool IsActiveTrigger;
	bool IsActiveFireTrigger;
	bool IsOutputError = false;
	// Use this for initialization
	void Start()
	{
		if (DeathExplodePoint == null) {
			DeathExplodePoint = transform;
		}

		if (AudioCannonFire != null) {
			AudioCannonFire.Stop();
		}

		if (SpawnAmmoPoint.Length <= 0) {
			Debug.LogWarning("XKCannonCtrl -> SpawnAmmoPoint was wrong!");
			IsOutputError = true;
		}
		else {
			for (int i = 0; i < SpawnAmmoPoint.Length; i ++) {
				if (SpawnAmmoPoint[i] == null) {
					Debug.LogWarning("XKCannonCtrl -> SpawnAmmoPoint was wrong! index = "+i);
					IsOutputError = true;
					break;
				}
			}
		}

		if (DaPaoHiddenArray.Length > 0) {
			int max = DaPaoHiddenArray.Length;
			for (int i = 0; i < max; i++) {
				if (DaPaoHiddenArray[i] == null) {
					Debug.LogWarning("DaPaoHiddenArray was wrong! index is "+i);
					IsOutputError = true;
					break;
				}

				XKDaPaoCtrl daPaoScript = DaPaoHiddenArray[i].GetComponent<XKDaPaoCtrl>();
				if (daPaoScript != null) {
					Debug.LogWarning("DaPaoHiddenArray was wrong! name is "+daPaoScript.gameObject.name);
				}
			}
		}

		CosAngleUp = Mathf.Cos((UpPaoGuanJDVal / 180f) * Mathf.PI);
		CosAngleDown = Mathf.Cos((DownPaoGuanJDVal / 180f) * Mathf.PI);

		CannonTran = transform;
		if (HealthScript == null) {
			HealthScript = GetComponent<XKNpcHealthCtrl>();
		}
		if (HealthScript != null) {
			HealthScript.SetCannonScript(this);
			IsYouTongNpc = HealthScript.IsYouTongNpc;
		}

		if (SpawnAmmoPoint.Length > 1) {
			IsDouGuanDaPao = true;
		}
		DaPaoAmmoLiZiObj = new GameObject[SpawnAmmoPoint.Length];
		InitNpcAmmoList();

		NpcMoveScript = GetComponentInParent<XKNpcMoveCtrl>();
		if (NpcMoveScript != null && IsHiddenAmmoSpawnPoint) {
			NpcMoveScript.SetIsRemoveNpcObj();
		}

		if (IsOutputError) {
			GameObject obj = null;
			obj.name = "null";		
		}
	}

	XKNpcMoveCtrl NpcMoveScript;
	XKNpcHealthCtrl HealthScript;
	bool IsYouTongNpc;
	public Transform TestPlayerTran;
	const float AngleMin = 1f;
	const float AngleMax = 89f;
	float CosAngleMin = Mathf.Cos((AngleMin / 180f) * Mathf.PI);
	float CosAngleDP = Mathf.Cos((45 / 180f) * Mathf.PI);
	float CosAngleUp = 0f;
	float CosAngleDown = 0f;
	bool IsDonnotSpawnAmmo;
	Transform NpcFirePlayer;
	XKDaPaoCtrl DaPaoCtrlScript;
	/*void OnDrawGizmosSelected()
	{
		if (!enabled) {
			return;
		}
		
		UpPaoGuanJDVal = MinPaoGuanJDVal - 360f;
		DownPaoGuanJDVal = MaxPaoGuanJDVal - 360f;
	}*/

	void MakePaoGuanAimPlayer(Vector3 playerPos)
	{
//		Vector3 posA = TestPlayerTran.position; //test
		Vector3 posA = playerPos;
		Vector3 posB = PaoGuan.position;
		Vector3 posBTmp = PaoGuan.position;
		Vector3 vecA = Vector3.Normalize(posA - posB);
		posBTmp.y = posA.y;
		
		Vector3 vecC = CannonTran.forward;
		float cosAC = Vector3.Dot(vecA, vecC);
		if (cosAC < CosAngleDP) {
			return;
		}

		Vector3 vecB = Vector3.Normalize(posA - posBTmp);
		float cosAB = Vector3.Dot(vecA, vecB);

		if (cosAB > CosAngleMin /*|| cosAB < CosAngleMax*/) {
//			if (cosAB > CosAngleMin) {
				Vector3 eulerAnglesTmp = Vector3.zero;
//				Vector3 eulerAnglesTmpVal = Vector3.zero;
				eulerAnglesTmp.x = posA.y >= posB.y ? (-AngleMin) : AngleMin;
				if (Mathf.Abs(PaoGuan.localEulerAngles.x) > AngleMin) {
					Vector3 eulerAnglesTmpVal = PaoGuan.localEulerAngles;
					if (eulerAnglesTmpVal.x > 90f) {
//						Debug.Log("A "+PaoGuan.localEulerAngles+", B "+AngleMin);
						eulerAnglesTmpVal.x = eulerAnglesTmpVal.x - 360f;
					}
					eulerAnglesTmpVal = Vector3.Lerp(eulerAnglesTmpVal, eulerAnglesTmp, Time.deltaTime * PaoGuanSDVal);
					eulerAnglesTmpVal.x = eulerAnglesTmpVal.x != 0f ? eulerAnglesTmpVal.x : eulerAnglesTmp.x;
				
					if (UpPaoGuanJDVal != DownPaoGuanJDVal) {	
						PaoGuan.localEulerAngles = eulerAnglesTmpVal;
					}
				}
//			}
//			else if (cosAB < CosAngleMax) {
//				Vector3 eulerAnglesTmp = Vector3.zero;
//				eulerAnglesTmp.x = posA.y >= posB.y ? (-AngleMax) : AngleMax;
//				PaoGuan.localEulerAngles = Vector3.Lerp(PaoGuan.localEulerAngles, eulerAnglesTmp,
//				                                        Time.deltaTime * PaoGuanSDVal);
//				PaoGuan.localEulerAngles = eulerAnglesTmp;
//			}
			return;
		}

		if (posA.y > posB.y && cosAB < CosAngleUp) {
			Vector3 eulerAnglesTmp = Vector3.zero;
			eulerAnglesTmp.x = UpPaoGuanJDVal;
			Vector3 eulerAnglesTmpVal = PaoGuan.localEulerAngles;
			if (eulerAnglesTmpVal.x > 90f) {
//				Debug.Log("A "+PaoGuan.localEulerAngles+", B "+AngleMin);
				eulerAnglesTmpVal.x = eulerAnglesTmpVal.x - 360f;
			}
			eulerAnglesTmpVal = Vector3.Lerp(eulerAnglesTmpVal, eulerAnglesTmp, Time.deltaTime * PaoGuanSDVal);
			
			if (UpPaoGuanJDVal != DownPaoGuanJDVal) {
				PaoGuan.localEulerAngles = eulerAnglesTmpVal;
			}
			return;
		}
		
		if (posA.y < posB.y && cosAB < CosAngleDown) {
			Vector3 eulerAnglesTmp = Vector3.zero;
			eulerAnglesTmp.x = DownPaoGuanJDVal;
			Vector3 eulerAnglesTmpVal = PaoGuan.localEulerAngles;
			if (eulerAnglesTmpVal.x > 90f) {
				//Debug.Log("A "+PaoGuan.localEulerAngles+", B "+AngleMin);
				eulerAnglesTmpVal.x = eulerAnglesTmpVal.x - 360f;
			}
			eulerAnglesTmpVal = Vector3.Lerp(eulerAnglesTmpVal, eulerAnglesTmp, Time.deltaTime * PaoGuanSDVal);
			
			if (UpPaoGuanJDVal != DownPaoGuanJDVal) {
				PaoGuan.localEulerAngles = eulerAnglesTmpVal;
			}
			return;
		}
		
		if (UpPaoGuanJDVal != DownPaoGuanJDVal) {
			PaoGuan.forward = Vector3.Lerp(PaoGuan.forward, vecA, Time.deltaTime * PaoGuanSDVal);
		}
		Vector3 eulerAngles = PaoGuan.localEulerAngles;
		eulerAngles.y = eulerAngles.z = 0f;
		if (UpPaoGuanJDVal != DownPaoGuanJDVal) {
			PaoGuan.localEulerAngles = eulerAngles;
		}
	}

	public void SetIsActiveTrigger()
	{
		IsActiveTrigger = true;
		if (Network.peerType == NetworkPeerType.Client) {
			IsDoFireAnimation = true;
			IsStopAnimation = false;
			Debug.Log("KaQiuSha -> SetIsActiveTrigger...");
		}
	}

	int FireCountCannon;
	// Update is called once per frame
	void Update()
	{
		if (IsOutputError) {
			return;		
		}

		if (IsYouTongNpc) {
			return;
		}
		
		if (JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()
		    || GameOverCtrl.IsShowGameOver) {
			return;
		}

		if (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo) {
			if (!XkGameCtrl.IsMoveOnPlayerDeath) {
				if (!IsStopAnimation) {
					IsStopAnimation = true;
				}
				return;
			}
		}
		else {
			if (IsStopAnimation) {
				IsStopAnimation = false;
			}
		}

		if (IsDonnotSpawnAmmo) {
			return;
		}

		if (IsHiddenAmmoSpawnPoint && IsActiveFireTrigger) {
			return;
		}

		Vector3 posA = Vector3.zero;
		switch (XkGameCtrl.GameModeVal) {
		case GameMode.DanJiFeiJi:
			posA = XkPlayerCtrl.PlayerTranFeiJi.position;
			break;

		case GameMode.DanJiTanKe:
			posA = XkPlayerCtrl.PlayerTranTanKe.position;
			break;

		case GameMode.LianJi:
			if (XkPlayerCtrl.PlayerTranFeiJi == null && XkPlayerCtrl.PlayerTranTanKe == null) {
				return;
			}

			if (FireCountCannon < 2) {
				if (NpcAimPlayerState == 0 || NpcAimPlayerState == 2) {
					if (XkPlayerCtrl.PlayerTranFeiJi != null) {
						posA = XkPlayerCtrl.PlayerTranFeiJi.position;
					}
					else {
						posA = XkPlayerCtrl.PlayerTranTanKe.position;
					}
				}
				else {
					if (XkPlayerCtrl.PlayerTranTanKe != null) {
						posA = XkPlayerCtrl.PlayerTranTanKe.position;
					}
					else {
						posA = XkPlayerCtrl.PlayerTranFeiJi.position;
					}
				}
			}
			else {
				if (!IsDoFireAnimation) {
					if (XkPlayerCtrl.PlayerTranFeiJi != null && XkPlayerCtrl.PlayerTranTanKe != null) {
						Vector3 posTmpA = XkPlayerCtrl.PlayerTranFeiJi.position;
						Vector3 posTmpB = XkPlayerCtrl.PlayerTranTanKe.position;
						Vector3 posTmpC = CannonTran.position;
						posTmpA.y = posTmpB.y = posTmpC.y = 0f;
						if (Vector3.Distance(posTmpA, posTmpC) < Vector3.Distance(posTmpB, posTmpC)) {
							if (NpcFirePlayer != XkPlayerCtrl.PlayerTranFeiJi) {
								NpcFirePlayer = XkPlayerCtrl.PlayerTranFeiJi;
							}
							posA = XkPlayerCtrl.PlayerTranFeiJi.position;
						}
						else {
							if (NpcFirePlayer != XkPlayerCtrl.PlayerTranTanKe) {
								NpcFirePlayer = XkPlayerCtrl.PlayerTranTanKe;
							}
							posA = XkPlayerCtrl.PlayerTranTanKe.position;
						}
					}
					else if (XkPlayerCtrl.PlayerTranFeiJi != null) {
						if (NpcFirePlayer != XkPlayerCtrl.PlayerTranFeiJi) {
							NpcFirePlayer = XkPlayerCtrl.PlayerTranFeiJi;
						}
						posA = XkPlayerCtrl.PlayerTranFeiJi.position;
					}
					else {
						if (NpcFirePlayer != XkPlayerCtrl.PlayerTranTanKe) {
							NpcFirePlayer = XkPlayerCtrl.PlayerTranTanKe;
						}
						posA = XkPlayerCtrl.PlayerTranTanKe.position;
					}
				}
				else {
					if (NpcFirePlayer == null) {
						IsDoFireAnimation = false;
					}
					else {
						posA = NpcFirePlayer.position;
					}
				}
			}
			break;
		}

		Vector3 posASave = posA;
		Vector3 posB = CannonTran.position;
		if (IsDoFireAnimation) {
			posA.y = posB.y = 0f;
			Vector3 forwardVal = posA - posB;
			if (MaxPaoShenJDVal != MinPaoShenJDVal) {
				CannonTran.forward = Vector3.Lerp(CannonTran.forward, forwardVal.normalized, Time.deltaTime * PaoShenSDVal);
			}

			Vector3 eulerAnglesPS = CannonTran.localEulerAngles;
			eulerAnglesPS.x = eulerAnglesPS.z = 0f;
			float angleY = eulerAnglesPS.y > 180 ? -(360 - eulerAnglesPS.y) : eulerAnglesPS.y;
			if (angleY > MaxPaoShenJDVal || angleY < MinPaoShenJDVal) {
				angleY = angleY > MaxPaoShenJDVal ? MaxPaoShenJDVal : angleY;
				eulerAnglesPS.y = angleY > MinPaoShenJDVal ? angleY : MinPaoShenJDVal;
			}

			if (MaxPaoShenJDVal != MinPaoShenJDVal) {
				CannonTran.localEulerAngles = eulerAnglesPS;
			}

			if (!IsPlayPaoGuanAnimation) {
				if (IsHiddenAmmoSpawnPoint) {
					switch (XkGameCtrl.GameModeVal) {
					case GameMode.DanJiFeiJi:
						posA = XkPlayerCtrl.KaQiuShaAimPlayerTranFJ.position;
						break;
						
					case GameMode.DanJiTanKe:
						posA = XkPlayerCtrl.KaQiuShaAimPlayerTranTK.position;
						break;
						
					case GameMode.LianJi:
						if (XkPlayerCtrl.KaQiuShaAimPlayerTranFJ == null && XkPlayerCtrl.KaQiuShaAimPlayerTranTK == null) {
							return;
						}
						
						if (NpcAimPlayerState == 0 || NpcAimPlayerState == 2) {
							if (XkPlayerCtrl.KaQiuShaAimPlayerTranFJ != null) {
								posA = XkPlayerCtrl.KaQiuShaAimPlayerTranFJ.position;
							}
							else {
								posA = XkPlayerCtrl.KaQiuShaAimPlayerTranTK.position;
							}
						}
						else {
							if (XkPlayerCtrl.KaQiuShaAimPlayerTranTK != null) {
								posA = XkPlayerCtrl.KaQiuShaAimPlayerTranTK.position;
							}
							else {
								posA = XkPlayerCtrl.KaQiuShaAimPlayerTranFJ.position;
							}
						}
						break;
					}
					posASave = posA;
				}
				MakePaoGuanAimPlayer(posASave);
			}
		}
		
		if (Network.peerType == NetworkPeerType.Server) {
			if (XkGameCtrl.CountNpcAmmo >= XkGameCtrl.AmmoNumMaxNpc) {
				return;
			}
		}

		if (IsHiddenAmmoSpawnPoint) {
			if (Network.peerType != NetworkPeerType.Disconnected) {
				if (XKTriggerKaQiuShaFire.IsFireKaQiuSha) {
					XKTriggerKaQiuShaFire.IsFireKaQiuSha = false;
					SetIsActiveTrigger();
				}
			}

			posA.y = posB.y = 0f;
			if (Vector3.Distance(posA, posB) <= FireDis && !IsDoFireAnimation) {
				IsDoFireAnimation = true;
				if (KaQiuShaTSLiZi != null && !IsActiveFireTrigger) {
					KaQiuShaTSLiZi.SetActive(true);
				}
			}

			if (IsActiveTrigger && !IsDonnotSpawnAmmo) {
				IsActiveTrigger = false;
				IsActiveFireTrigger = true;
				if (KaQiuShaTSLiZi != null) {
					KaQiuShaTSLiZi.SetActive(false);
				}
				StartCoroutine(SpawnDuoPaoAmmo());
			}
			return;
		}

		posA.y = posB.y = 0f;
		if (Vector3.Distance(posA, posB) <= FireDis && !IsDoFireAnimation) {
			FireCountCannon++;
			IsDoFireAnimation = true;
			if (!IsDouGuanDaPao) {
				StartCoroutine(PlayPaoGuanAnimation());
			}
			else {
				if (!IsDonnotSpawnAmmo && !IsHiddenAmmoSpawnPoint) {
					StartCoroutine(SpawnDuoPaoAmmo());
				}
			}
		}
		else if (Vector3.Distance(posA, posB) > FireDis && IsDoFireAnimation) {
			FireCountCannon++;
			IsDoFireAnimation = false;
			if (!IsDouGuanDaPao) {
				StopCoroutine(PlayPaoGuanAnimation());
			}
			else {
				StopCoroutine(SpawnDuoPaoAmmo());
				StopAudioCannonFire();
			}
		}
	}

	/********************************************************************
	 * NpcAimPlayerState == 0; -> SpawnPointScript.PointType == SpawnPointType.KongZhong
	 * NpcAimPlayerState == 1; -> SpawnPointScript.PointType == SpawnPointType.DiMian
	 * NpcAimPlayerState == 2; -> SpawnPointScript.PointType == SpawnPointType.Null && SpawnPointScript.IsAimFeiJiPlayer == true
	 * NpcAimPlayerState == -1; -> SpawnPointScript.PointType == SpawnPointType.Null && SpawnPointScript.IsAimFeiJiPlayer == false
	 ********************************************************************/
	int NpcAimPlayerState = -1;
	public void SetCannonSpawnPointInfo(int aimState, float fireDisVal)
	{
		//Debug.Log("SetCannonSpawnPointInfo -> aimState "+aimState+", fireDisVal "+fireDisVal);
		ResetCannonInfo();
		NpcAimPlayerState = aimState;
		FireDis = fireDisVal;
		if (!gameObject.activeSelf || IsDeathNpc) {
//			Debug.Log("test*************SetSpawnPointScript");
			gameObject.SetActive(true);
			IsDeathNpc = false;
			InitCannonInfo();
		}
	}

	void PlayAudioCannonFire()
	{
		if (AudioCannonFire == null) {
			return;
		}

		if (AudioCannonFire.isPlaying && AudioCannonFire.loop) {
			return;
		}

		if (!AudioCannonFire.enabled) {
			return;
		}

		if (AudioCannonFire.isPlaying) {
			AudioCannonFire.Stop();
		}
		AudioCannonFire.Play();
	}

	void StopAudioCannonFire()
	{
		if (AudioCannonFire == null) {
			return;
		}
		AudioCannonFire.Stop();
	}

	IEnumerator PlayPaoGuanAnimation()
	{
		int count = 0;
		int maxCount = 1;
		float speed = PaoGuanZhenFu / maxCount;
		bool isBackPaoGuan = false;
		bool isFireAmmo = false;
		IsPlayPaoGuanAnimation = true;
		do {
			if (JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()
			    || IsDeathNpc
			    || GameOverCtrl.IsShowGameOver) {
				yield break;
			}

			if (IsStopAnimation) {
				yield return new WaitForSeconds(0.3f);
				continue;
			}

			if (!isBackPaoGuan) {
				PaoGuan.position -= PaoGuan.forward * speed;
				count++;
				if (count >= maxCount) {
					isBackPaoGuan = true;
				}

				if (count == 1 && !isFireAmmo) {
					isFireAmmo = true;
					PlayAudioCannonFire();
					
					PlayerAmmoCtrl ammoPlayerScript = DaPaoAmmo.GetComponent<PlayerAmmoCtrl>();
					if (ammoPlayerScript != null && !XkGameCtrl.GetInstance().IsCartoonShootTest) {
						yield break;
					}

					GameObject obj = GetNpcAmmoFromList(SpawnAmmoPoint[0]);
					if (obj == null) {
						yield break;
					}

					NpcAmmoCtrl AmmoScript = obj.GetComponent<NpcAmmoCtrl>();
					Transform tran = obj.transform;
					tran.parent = XkGameCtrl.NpcAmmoArray;
					if (AmmoScript != null) {
						AmmoScript.SetIsAimPlayer(false);
						AmmoScript.SetIsAimFeiJiPlayer(false);
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

					if (DaPaoAmmoLiZiObj[0] == null) {
						obj = (GameObject)Instantiate(DaPaoAmmoLiZi, SpawnAmmoPoint[0].position, SpawnAmmoPoint[0].rotation);
						tran = obj.transform;
						DaPaoAmmoLiZiObj[0] = obj;
						XkGameCtrl.CheckObjDestroyThisTimed(obj);
					}

					if (!SpawnAmmoPoint[0].gameObject.activeSelf) {
						SpawnAmmoPoint[0].gameObject.SetActive(true);
					}
					tran.parent = SpawnAmmoPoint[0];
				}
			}
			else {
				PaoGuan.position += PaoGuan.forward * speed;
				count--;
				if (count <= 0) {
					//break;
					IsPlayPaoGuanAnimation = false;
					isBackPaoGuan = false;
					isFireAmmo = false;
					count = 0;
					if (IsDoFireAnimation) {
						yield return new WaitForSeconds(TimeDanGuanFire);

						IsPlayPaoGuanAnimation = true;
						continue;
					}
				}
			}
			yield return new WaitForSeconds(PaoGuanShenSuoSD);
//			Debug.Log("dp****************"+Time.deltaTime);
		} while (IsDoFireAnimation);
	}

	IEnumerator SpawnDuoPaoAmmo()
	{
		if (!IsDouGuanDaPao) {
			yield break;
		}

		int countMax = SpawnAmmoPoint.Length;
		int count = countMax;
		do {
			if (JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()
			    || IsDeathNpc
			    || GameOverCtrl.IsShowGameOver) {
				yield break;
			}

			count--;
			PlayAudioCannonFire();

			GameObject obj = GetNpcAmmoFromList(SpawnAmmoPoint[count]);
			if (obj == null) {
				yield break;
			}

			NpcAmmoCtrl AmmoScript = obj.GetComponent<NpcAmmoCtrl>();
			if (IsHiddenAmmoSpawnPoint) {
				if ((count + 1) == countMax) {
					XKTriggerStopMovePlayer.SetKaQiuShaAmmoTrInfo(AmmoScript);
				}
				AmmoScript.SetIsCannotAddNpcAmmoList();
			}
			AmmoScript.SetIsAimPlayer(false);
			AmmoScript.SetIsAimFeiJiPlayer(false);
			Transform tran = obj.transform;
			tran.parent = XkGameCtrl.NpcAmmoArray;
			if (IsHiddenAmmoSpawnPoint) {
				SpawnAmmoPoint[count].gameObject.SetActive(false);
			}
			else {
				if (!SpawnAmmoPoint[count].gameObject.activeSelf) {
					SpawnAmmoPoint[count].gameObject.SetActive(true);
				}
			}

			if (DaPaoAmmoLiZiObj[count] == null) {
				obj = (GameObject)Instantiate(DaPaoAmmoLiZi, SpawnAmmoPoint[count].position, SpawnAmmoPoint[count].rotation);
				tran = obj.transform;
				DaPaoAmmoLiZiObj[count] = obj;
				XkGameCtrl.CheckObjDestroyThisTimed(obj);
			}

			if (IsHiddenAmmoSpawnPoint) {
				tran.parent = XkGameCtrl.NpcAmmoArray;
			}
			else {
				tran.parent = SpawnAmmoPoint[count];
			}

			yield return new WaitForSeconds(TimeAmmoUnit);
			if (!IsDoFireAnimation || IsStopAnimation) {
				yield break;
			}

			if (count <= 0) {
				yield return new WaitForSeconds(TimeAmmoWait);
				
				if (!IsDoFireAnimation || IsStopAnimation || IsHiddenAmmoSpawnPoint) {
					if (IsHiddenAmmoSpawnPoint) {
						IsDonnotSpawnAmmo = true;
					}
					yield break;
				}
				else {
					count = countMax;
					continue;
				}
			}
		} while (count > 0);
	}

	public void CallOtherPortDeath()
	{
		if (IsDeathNpc) {
			return;
		}
		IsDeathNpc = true;
		
		//ClearNpcAmmoList();
		if (DeathExplode != null) {
			if (!DeathExplode.activeSelf) {
				DeathExplode.SetActive(true);
			}
			GameObject objExplode = null;
			objExplode = (GameObject)Instantiate(DeathExplode, DeathExplodePoint.position, DeathExplodePoint.rotation);
			objExplode.transform.parent = XkGameCtrl.NpcAmmoArray;
			XkGameCtrl.CheckObjDestroyThisTimed(objExplode);
		}

		int max = DaPaoHiddenArray.Length;
		for (int i = 0; i < max; i++) {
			if (DaPaoHiddenArray[i] != null) {
				DaPaoHiddenArray[i].SetActive(false);
			}
		}
		ResetCannonInfo();
	}

	public void OnRemoveCannon(PlayerEnum playerSt, int key)
	{
		if (IsDeathNpc) {
			return;
		}
		IsDeathNpc = true;

		//ClearNpcAmmoList();
		if (key == 1) {
			//XkGameCtrl.GetInstance().AddPlayerKillNpc(playerSt, NpcJiFenEnum.CheLiang);
			if (DeathExplode != null) {
				if (!DeathExplode.activeSelf) {
					DeathExplode.SetActive(true);
				}
				GameObject objExplode = null;
				objExplode = (GameObject)Instantiate(DeathExplode, DeathExplodePoint.position, DeathExplodePoint.rotation);
				objExplode.transform.parent = XkGameCtrl.NpcAmmoArray;
				XkGameCtrl.CheckObjDestroyThisTimed(objExplode);
			}
		}
		XkGameCtrl.GetInstance().RemoveNpcTranFromList(CannonTran);

		float timeVal = 0f;
		if (DaPaoHiddenArray.Length <= 0) {
			gameObject.SetActive(false);
			ResetCannonInfo();
			//Destroy(gameObject);
		}
		else {
			int max = DaPaoHiddenArray.Length;
			for (int i = 0; i < max; i++) {
				if (DaPaoHiddenArray[i] != null) {
					DaPaoHiddenArray[i].SetActive(false);
				}
			}
			timeVal = 3f;
		}

		if (IsHiddenAmmoSpawnPoint) {
			ResetCannonInfo();
		}

		if (DaPaoCtrlScript == null) {
			DaPaoCtrlScript = GetComponentInParent<XKDaPaoCtrl>();
			//if (DaPaoCtrlScript != null) {
				//Debug.Log("find XKDaPaoCtrl script...");
			//}
		}

		if (key == 1 && DaPaoCtrlScript != null && NpcMoveScript == null) {
			//Debug.Log("XKDaPaoCtrl -> OnRemoveCannon...");
			DaPaoCtrlScript.OnRemoveCannon(PlayerEnum.Null, 1, timeVal);
		}
	}

	public void SetSpawnPointScript(XKDaPaoCtrl daPaoScript)
	{
		DaPaoCtrlScript = daPaoScript;
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

//		Debug.Log("XKCannonCtrl::ClearNpcAmmoList -> NpcAmmoCount "+AmmoList.Count);
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
		}
		return objAmmo;
	}
	
	GameObject SpawnNpcAmmo(Transform spawnPoint)
	{
		return (GameObject)Instantiate(DaPaoAmmo, spawnPoint.position, spawnPoint.rotation);
	}

	/// <summary>
	/// Resets the cannon info.
	/// </summary>
	void ResetCannonInfo()
	{
		IsClearNpcAmmo = false;
		IsDonnotSpawnAmmo = false;
//		IsYouTongNpc = false;

//		IsOutputError = false;
		IsActiveFireTrigger = false;
		IsActiveTrigger = false;
		
		IsPlayPaoGuanAnimation = false;
//		IsDouGuanDaPao = false;
		IsStopAnimation = false;

		IsDoFireAnimation = false;
		//IsDeathNpc = false;

		int max = 0;
		if (IsHiddenAmmoSpawnPoint) {
			max = SpawnAmmoPoint.Length;
			for (int i = 0; i < max; i++) {
				SpawnAmmoPoint[i].gameObject.SetActive(true);
			}
		}

		max = DaPaoHiddenArray.Length;
		for (int i = 0; i < max; i++) {
			if (DaPaoHiddenArray[i] != null) {
				DaPaoHiddenArray[i].SetActive(true);
			}
		}
	}

	void InitCannonInfo()
	{
		if (HealthScript != null) {
			HealthScript.SetCannonScript(this);
		}

//		int max = DaPaoHiddenArray.Length;
//		for (int i = 0; i < max; i++) {
//			if (DaPaoHiddenArray[i] != null) {
//				DaPaoHiddenArray[i].SetActive(true);
//			}
//		}

		if (AudioCannonFire != null) {
			AudioCannonFire.Stop();
		}
		InitNpcAmmoList();
	}
}