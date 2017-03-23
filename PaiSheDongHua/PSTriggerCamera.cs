using UnityEngine;
using System.Collections;

public enum PSCameraState
{
	Null,
	GenSuiMoveCam,
	DingDianAimCam,
	DingDianNoAimCam,
	ZiYouMoveCam,
}

public class PSTriggerCamera : MonoBehaviour {
	public PSCameraState CamState = PSCameraState.Null;
	public int IndexGenSuiMove; //跟随移动摄像机.
	public PSDingDianAimCamera DingDianAimCam; //定点瞄准摄像机.
	public GameObject DingDianNoAimObj; //定点不瞄准摄像机.
	public PSZiYouMoveCamera ZiYouMoveCam; //自由移动摄像机.
	public XKTriggerPlayAnimation TriggerPlayAni; //播放npc动画触发器.
	[Range(0f, 100f)]public float WorldTimeScale = 1f; //世界时间轴控制.
	[Range(0f, 100f)]public float WorldTimeVal;
	[Range(0f, 100f)]public float TimeShake = 0f; //镜头抖动时间.
	[Range(0.05f, 10f)]public float CamShakeZFVal = 0.5f; //镜头抖动振幅.
	CameraShake CamShakeScript;
	public PSAutoFireCtrl[] AutoFireCom; //自动发射子弹.
	public GameObject[] AutoFireNpc; //自动发射子弹.
	public PSPlayerAutoFireCtrl PSPlayerAutoFire;
	public static PlayerAmmoType AutoFirePlayerAmmoTypeVal = PlayerAmmoType.Null;
	public Transform TanKePlayerAimPoint; //主角坦克玩家的炮塔瞄准点.
	public Transform[] ExplodePoint; //爆炸激活点.
	public GameObject[] ExplodePrefab;
	public GameObject[] HiddenExplodeObj; //爆炸隐藏的对象.
	public AiPathCtrl TestPlayerPath;
	// Use this for initialization
	void Start()
	{
		if (Network.peerType != NetworkPeerType.Server) {
			if (GameMovieCtrl.IsActivePlayer
			    || (!XkGameCtrl.GetInstance().IsCartoonShootTest && !XkGameCtrl.GetInstance().IsServerCameraTest)) {
				gameObject.SetActive(false);
			}
			return;
		}

		if (!XkGameCtrl.GetInstance().IsCartoonShootTest && CamState == PSCameraState.Null) {
			gameObject.SetActive(false);
			return;
		}
		Invoke("CheckTriggerCameraInfo", 1f);
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
	}

	void CheckTriggerCameraInfo()
	{
		if (XkGameCtrl.IsLoadingLevel) {
			return;
		}

		bool isOutPrintError = false;
		switch (CamState) {
		case PSCameraState.DingDianAimCam:
			if (DingDianAimCam == null) {
				Debug.LogWarning("DingDianAimCam is null");
				isOutPrintError = true;
			}
			break;
			
		case PSCameraState.DingDianNoAimCam:
			if (DingDianNoAimObj == null) {
				Debug.LogWarning("DingDianNoAimObj is null");
				isOutPrintError = true;
			}
			else if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
				Camera cameraCom = DingDianNoAimObj.GetComponent<Camera>();
				if (cameraCom != null) {
					PSDingDianNoAimCamera noAimCamera = DingDianNoAimObj.GetComponent<PSDingDianNoAimCamera>();
					if (noAimCamera == null) {
						Debug.LogWarning("DingDianNoAimObj cannot find PSDingDianNoAimCamera!");
						isOutPrintError = true;
					}
					else {
						cameraCom.tag = "MainCamera";
						DingDianNoAimObj.SetActive(false);
					}
				}
				else {
					Debug.LogWarning("DingDianNoAimObj cannot find camera component!");
					isOutPrintError = true;
				}
			}
			else {
				Camera cameraCom = DingDianNoAimObj.GetComponent<Camera>();
				if (cameraCom != null) {
					cameraCom.enabled = false;
				}
			}
			break;
			
		case PSCameraState.GenSuiMoveCam:
			if (TestPlayerPath != null) {
				AiPathGroupCtrl pathCtrl = TestPlayerPath.GetComponentInParent<AiPathGroupCtrl>();
				if (pathCtrl != null) {
					if (pathCtrl.PathState == PlayerTypeEnum.FeiJi) {
						if (XkGameCtrl.GameModeVal != GameMode.DanJiTanKe && Network.peerType == NetworkPeerType.Disconnected) {
							if (PSGenSuiMoveCamera.GetInstanceFeiJi() == null) {
								Debug.LogWarning("PSGenSuiMoveCameraFeiJi is null");
								isOutPrintError = true;
							}
							else {
								if (IndexGenSuiMove < 0
								    || IndexGenSuiMove >= PSGenSuiMoveCamera.GetInstanceFeiJi().GenSuiMoveCam.Length
								    || PSGenSuiMoveCamera.GetInstanceFeiJi().GenSuiMoveCam[IndexGenSuiMove] == null) {
									Debug.LogWarning("IndexGenSuiMoveFeiJi was wrong!");
									isOutPrintError = true;
								}
							}
						}
					}
					else {
						if (XkGameCtrl.GameModeVal != GameMode.DanJiFeiJi && Network.peerType == NetworkPeerType.Disconnected) {
							if (PSGenSuiMoveCamera.GetInstanceTanKe() == null) {
								Debug.LogWarning("PSGenSuiMoveCameraTanKe is null");
								isOutPrintError = true;
							}
							else {
								if (IndexGenSuiMove < 0
								    || IndexGenSuiMove >= PSGenSuiMoveCamera.GetInstanceTanKe().GenSuiMoveCam.Length
								    || PSGenSuiMoveCamera.GetInstanceTanKe().GenSuiMoveCam[IndexGenSuiMove] == null) {
									Debug.LogWarning("IndexGenSuiMoveTanKe was wrong!");
									isOutPrintError = true;
								}
							}
						}
					}
				}
			}
			break;
			
		case PSCameraState.ZiYouMoveCam:
			if (ZiYouMoveCam == null) {
				Debug.LogWarning("ZiYouMoveCam is null");
				isOutPrintError = true;
			}
			break;
			
		/*case PSCameraState.Null:
			Debug.LogWarning("CamState is null");
			isOutPrintError = true;
			break;*/
		}
		
		if (isOutPrintError) {
			GameObject obj = null;
			obj.name = "null";
		}
		else {
			CheckCamShakeScript();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (GameMovieCtrl.IsActivePlayer) {
			if (Network.peerType == NetworkPeerType.Server) {
			}
			else {
				return;
			}
		}

		XkPlayerCtrl script = other.GetComponent<XkPlayerCtrl>();
		if (script == null) {
			return;
		}
		//Debug.Log("*********************name "+gameObject.name);

		PSDingDianAimCamera.DingDianAimCamera = null;
		PSDingDianNoAimCamera.DingDianNoAimCamera = null;
		PSZiYouMoveCamera.ZYMoveCamera = null;
		if (CamState == PSCameraState.GenSuiMoveCam) {
			PlayerTypeEnum playerType = script.PlayerSt;
			switch (playerType) {
			case PlayerTypeEnum.FeiJi:
				PSGenSuiMoveCamera.GetInstanceFeiJi().ActiveGenSuiMoveCam(IndexGenSuiMove);
				break;

			case PlayerTypeEnum.TanKe:
				PSGenSuiMoveCamera.GetInstanceTanKe().ActiveGenSuiMoveCam(IndexGenSuiMove);
				break;
			}
		}

		if (DingDianAimCam != null && CamState == PSCameraState.DingDianAimCam) {
			DingDianAimCam.ActiveCamera(script.transform);
		}
		CheckZiYouMoveCam(script);
		CheckDingDianNoAimCam();

		if (GameMovieCtrl.IsActivePlayer || XkGameCtrl.GetInstance().IsServerCameraTest) {
			return;
		}
		CheckCameraTimeShake();
		CheckExplodePoint();
		CheckTanKePlayerAimPoint();
		CheckAutoFireCom();
		CheckTriggerPlayAni();
		CheckWorldTimeScale();
		CheckAutoFirePlayerAmmoType();
	}
	
	void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}
		
		if (TestPlayerPath != null) {
			TestPlayerPath.DrawPath();
		}
	}

	void CheckZiYouMoveCam(XkPlayerCtrl playerScript)
	{
		if (CamState != PSCameraState.ZiYouMoveCam) {
			return;
		}

		if (ZiYouMoveCam == null) {
			return;
		}
		ZiYouMoveCam.MoveCameraByItween(playerScript);
	}

	void CheckWorldTimeScale()
	{
		if (WorldTimeVal <= 0f) {
			return;
		}

		/*if (Time.timeScale == WorldTimeScale) {
			return;
		}*/
		Time.timeScale = WorldTimeScale;
		CancelInvoke("ResetWorldTimeScale");
		Invoke("ResetWorldTimeScale", (WorldTimeVal * WorldTimeScale));
	}

	void ResetWorldTimeScale()
	{
		Time.timeScale = 1f;
	}

	void CheckDingDianNoAimCam()
	{
		if (CamState != PSCameraState.DingDianNoAimCam) {
			return;
		}

		if (DingDianNoAimObj == null) {
			return;
		}
		
		if (Network.peerType == NetworkPeerType.Server || XkGameCtrl.GetInstance().IsServerCameraTest) {
			if (Camera.main.gameObject != XkGameCtrl.ServerCameraObj) {
				if (Camera.main != null) {
					Camera.main.enabled = false;
				}
			}
			PSDingDianNoAimCamera.DingDianNoAimCamera = DingDianNoAimObj;
			XkGameCtrl.SetServerCameraTran(DingDianNoAimObj.transform);
		}
		else {
			if (Camera.main != null) {
				Camera.main.enabled = false;
			}
			DingDianNoAimObj.SetActive(true);
		}
	}

	void CheckCamShakeScript()
	{
		if (TimeShake <= 0f) {
			return;
		}
		
		CameraShake shakeScript;
		bool isOutPrintError = false;
		switch (CamState) {
		case PSCameraState.DingDianAimCam:
			if (DingDianAimCam == null) {
				Debug.LogWarning("DingDianAimCam is null");
				isOutPrintError = true;
			}
			
			if (!isOutPrintError) {
				shakeScript = DingDianAimCam.gameObject.GetComponent<CameraShake>();
				if (shakeScript == null) {
					shakeScript = DingDianAimCam.gameObject.AddComponent<CameraShake>();
				}
				CamShakeScript = shakeScript;
			}
			break;
			
		case PSCameraState.DingDianNoAimCam:
			if (DingDianNoAimObj == null) {
				Debug.LogWarning("DingDianNoAimObj is null");
				isOutPrintError = true;
			}
			
			if (!isOutPrintError) {
				shakeScript = DingDianNoAimObj.GetComponent<CameraShake>();
				if (shakeScript == null) {
					shakeScript = DingDianNoAimObj.AddComponent<CameraShake>();
				}
				CamShakeScript = shakeScript;
			}
			break;
			
		case PSCameraState.GenSuiMoveCam:
			if (TestPlayerPath != null) {
				AiPathGroupCtrl pathCtrl = TestPlayerPath.GetComponentInParent<AiPathGroupCtrl>();
				if (pathCtrl != null) {
					if (pathCtrl.PathState == PlayerTypeEnum.FeiJi) {
						if (PSGenSuiMoveCamera.GetInstanceFeiJi() == null) {
							Debug.LogWarning("PSGenSuiMoveCameraFeiJi is null");
							isOutPrintError = true;
						}
						else {
							if (IndexGenSuiMove < 0
							    || IndexGenSuiMove >= PSGenSuiMoveCamera.GetInstanceFeiJi().GenSuiMoveCam.Length
							    || PSGenSuiMoveCamera.GetInstanceFeiJi().GenSuiMoveCam[IndexGenSuiMove] == null) {
								Debug.LogWarning("IndexGenSuiMoveFeiJi was wrong!");
								isOutPrintError = true;
							}
						}
					}
					else {
						if (PSGenSuiMoveCamera.GetInstanceTanKe() == null) {
							Debug.LogWarning("PSGenSuiMoveCameraTanKe is null");
							isOutPrintError = true;
						}
						else {
							if (IndexGenSuiMove < 0
							    || IndexGenSuiMove >= PSGenSuiMoveCamera.GetInstanceTanKe().GenSuiMoveCam.Length
							    || PSGenSuiMoveCamera.GetInstanceTanKe().GenSuiMoveCam[IndexGenSuiMove] == null) {
								Debug.LogWarning("IndexGenSuiMoveTanKe was wrong!");
								isOutPrintError = true;
							}
						}
					}
					
					if (!isOutPrintError) {
						shakeScript = null;
						if (pathCtrl.PathState == PlayerTypeEnum.FeiJi) {
							shakeScript = PSGenSuiMoveCamera.GetInstanceFeiJi().GenSuiMoveCam[IndexGenSuiMove].GetComponent<CameraShake>();
							if (shakeScript == null) {
								shakeScript = PSGenSuiMoveCamera.GetInstanceFeiJi().GenSuiMoveCam[IndexGenSuiMove].AddComponent<CameraShake>();
							}
						}
						else {
							shakeScript = PSGenSuiMoveCamera.GetInstanceTanKe().GenSuiMoveCam[IndexGenSuiMove].GetComponent<CameraShake>();
							if (shakeScript == null) {
								shakeScript = PSGenSuiMoveCamera.GetInstanceTanKe().GenSuiMoveCam[IndexGenSuiMove].AddComponent<CameraShake>();
							}
						}
						CamShakeScript = shakeScript;
					}
				}
			}

			break;
			
		case PSCameraState.ZiYouMoveCam:
			if (ZiYouMoveCam == null) {
				Debug.LogWarning("ZiYouMoveCam is null");
				isOutPrintError = true;
			}
			
			if (!isOutPrintError) {
				shakeScript = ZiYouMoveCam.gameObject.GetComponent<CameraShake>();
				if (shakeScript == null) {
					shakeScript = ZiYouMoveCam.gameObject.AddComponent<CameraShake>();
				}
				CamShakeScript = shakeScript;
			}
			break;
		}
		
		/*if (CamShakeScript == null) {
			Debug.LogWarning("CamShakeScript is null");
			GameObject obj = null;
			obj.name = "null";
		}*/
	}
	
	void CheckCameraTimeShake()
	{
		if (TimeShake <= 0f) {
			return;
		}
		
		if (CamShakeScript == null) {
			if (Camera.main != null) {
				CamShakeScript = Camera.main.GetComponent<CameraShake>();
				if (CamShakeScript == null) {
					CamShakeScript = Camera.main.gameObject.AddComponent<CameraShake>();
				}
			}
			else {
				Debug.LogWarning("Cannot find CameraMain");
				GameObject obj = null;
				obj.name = "null";
				return;
			}
		}
		CamShakeScript.SetCameraTimeShake(TimeShake);
		CamShakeScript.SetCameraShakeImpulseValue(CamShakeZFVal);
	}
	
	void CheckExplodePoint()
	{
		if (ExplodePoint.Length <= 0) {
			return;
		}
		
		GameObject obj = null;
		Transform tran = null;
		int max = ExplodePoint.Length;
		for (int i = 0; i < max; i++) {
			if (ExplodePrefab[i] != null && ExplodePoint[i] != null) {
				obj = (GameObject)Instantiate(ExplodePrefab[i], ExplodePoint[i].position, ExplodePoint[i].rotation);
				tran = obj.transform;
				tran.parent = XkGameCtrl.MissionCleanup;
				XkGameCtrl.CheckObjDestroyThisTimed(obj);
			}
		}

		max = HiddenExplodeObj.Length;
		for (int i = 0; i < max; i++) {
			if (HiddenExplodeObj[i] != null) {
				HiddenExplodeObj[i].SetActive(false);
			}
		}
	}

	void CheckTanKePlayerAimPoint()
	{
		if (TanKePlayerAimPoint == null) {
			return;
		}
		
		if (XkPlayerCtrl.GetInstanceTanKe() == null) {
			return;
		}
	}

	void CheckAutoFireCom()
	{
		if (AutoFireCom.Length > 0) {
			int max = AutoFireCom.Length;
			for (int i = 0; i < max; i++) {
				if (AutoFireCom[i] != null) {
					AutoFireCom[i].StartAutoFire();
				}
			}
		}
		
		if (AutoFireNpc.Length > 0) {
			XKNpcMoveCtrl npcScript = null;
			XKCannonCtrl[] cannonScript;
			float fireDisVal = 99999f;
			int max = AutoFireNpc.Length;
			for (int i = 0; i < max; i++) {
				if (AutoFireNpc[i] != null) {
					npcScript = AutoFireNpc[i].GetComponent<XKNpcMoveCtrl>();
					if (npcScript != null) {
						npcScript.SetFireDistance(fireDisVal);
					}

					cannonScript = AutoFireNpc[i].GetComponentsInChildren<XKCannonCtrl>();
					if (cannonScript != null && cannonScript.Length > 0) {
						int maxCannon = cannonScript.Length;
						for (int j = 0; j < maxCannon; j++) {
							cannonScript[j].FireDis = fireDisVal;
						}
					}
				}
			}
		}
	}

	void CheckTriggerPlayAni()
	{
		if (TriggerPlayAni == null) {
			return;
		}
		TriggerPlayAni.PlayAnimation();
	}

	void CheckAutoFirePlayerAmmoType()
	{
		if (PSPlayerAutoFire == null) {
			return;
		}

		PlayerAmmoType autoFirePlayerAmmoType = PSPlayerAutoFire.AutoFirePlayerAmmoType;
		int maxAmmoNum = 999999;
		switch (autoFirePlayerAmmoType) {
		case PlayerAmmoType.DaoDanAmmo:
			XkGameCtrl.DaoDanNumPOne = maxAmmoNum;
			XkGameCtrl.DaoDanNumPTwo = maxAmmoNum;
			break;
			
		case PlayerAmmoType.GaoBaoAmmo:
			XkGameCtrl.GaoBaoDanNumPOne = maxAmmoNum;
			XkGameCtrl.GaoBaoDanNumPTwo = maxAmmoNum;
			break;
			
		case PlayerAmmoType.PuTongAmmo:
			XkGameCtrl.GaoBaoDanNumPOne = 0;
			XkGameCtrl.GaoBaoDanNumPTwo = 0;
			break;
		}
		AutoFirePlayerAmmoTypeVal = autoFirePlayerAmmoType;
	}
}