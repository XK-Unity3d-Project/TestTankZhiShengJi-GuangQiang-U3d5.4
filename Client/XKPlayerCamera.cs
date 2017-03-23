using UnityEngine;
using System.Collections;

public class XKPlayerCamera : MonoBehaviour {
	Transform CameraTran;
	Transform AimTran;
	Transform CameraParent;
	float SpeedIntoAim = 0.1f;
	float SpeedOutAim = 1f;
	bool IsOutAim;
	float AimNpcSpeed = 0.1f;
	float LeaveNpcSpeed = 0.1f;
	float GenZongTmpVal = 0.0001f;
	float GenZongCamRotVal = 0.2f;
	bool IsChangeSpeedOutAim;
	public static Transform FeiJiCameraTan;
	public static Transform TanKeCameraTan;
	GameObject CameraObj;
	XkPlayerCtrl PlayerScript;
	GameObject AimNpcObj;
	float TimeCheckAimNpcLast;
	Camera PlayerCamera;
	PlayerTypeEnum PlayerSt = PlayerTypeEnum.FeiJi;
	static XKPlayerCamera _InstanceFeiJi;
	public static XKPlayerCamera GetInstanceFeiJi()
	{
		return _InstanceFeiJi;
	}
	static XKPlayerCamera _InstanceTanKe;
	public static XKPlayerCamera GetInstanceTanKe()
	{
		return _InstanceTanKe;
	}
	static XKPlayerCamera _InstanceCartoon;
	public static XKPlayerCamera GetInstanceCartoon()
	{
		return _InstanceCartoon;
	}

	// Use this for initialization
	void Start()
	{
		PlayerCamera = camera;
		camera.targetTexture = null;
		CameraTran = transform;
		XkPlayerCtrl script = GetComponentInParent<XkPlayerCtrl>();
		switch (script.PlayerSt) {
		case PlayerTypeEnum.FeiJi:
			_InstanceFeiJi = this;
			PlayerSt = PlayerTypeEnum.FeiJi;
			FeiJiCameraTan = transform;
			gameObject.SetActive(false);
			break;

		case PlayerTypeEnum.TanKe:
			_InstanceTanKe = this;
			PlayerSt = PlayerTypeEnum.TanKe;
			TanKeCameraTan = transform;
			gameObject.SetActive(false);
			break;

		case PlayerTypeEnum.CartoonCamera:
			_InstanceCartoon = this;
			PlayerSt = PlayerTypeEnum.CartoonCamera;
			break;
		}

		CameraObj = gameObject;
		PlayerScript = GetComponentInParent<XkPlayerCtrl>();
		if (PlayerScript != null) {
			PlayerScript.SetPlayerCamera(this);
		}
		
		GameObject obj = new GameObject();
		obj.name = "CameraParent";
		CameraParent = obj.transform;
		CameraParent.parent = CameraTran.parent;
		CameraParent.localPosition = CameraTran.localPosition;
		CameraParent.rotation = CameraTran.rotation;
		CameraTran.parent = null;

		if (PlayerSt != PlayerTypeEnum.CartoonCamera) {
			SetEnableCamera(false);
		}
	}

	public void SetActiveCamera(bool isActive)
	{
		CameraObj.SetActive(isActive);
		if (XkGameCtrl.GameModeVal == GameMode.LianJi && !XKCameraMapCtrl.GetInstance().GetActiveCameraMap()) {
			GameJiTaiType jiTai = XkGameCtrl.GameJiTaiSt;
			switch (jiTai) {
			case GameJiTaiType.FeiJiJiTai:
				if (PlayerSt == PlayerTypeEnum.TanKe) {
					isActive = false;
				}
				break;

			case GameJiTaiType.TanKeJiTai:
				if (PlayerSt == PlayerTypeEnum.FeiJi) {
					isActive = false;
				}
				break;
			}
		}

		if (isActive && !ScreenDanHeiCtrl.IsStartGame && PlayerSt != PlayerTypeEnum.CartoonCamera) {
			isActive = false;
		}
		//Debug.Log("SetActiveCamera -> player "+PlayerSt+", isEnable "+isActive);
		PlayerCamera.enabled = isActive;
	}

	public void SetEnableCamera(bool isEnable)
	{
		if (XkGameCtrl.GameModeVal == GameMode.LianJi && Network.peerType != NetworkPeerType.Disconnected) {
			GameJiTaiType jiTai = XkGameCtrl.GameJiTaiSt;
			switch (jiTai) {
			case GameJiTaiType.FeiJiJiTai:
				if (PlayerSt == PlayerTypeEnum.TanKe) {
					isEnable = false;
				}
				break;
				
			case GameJiTaiType.TanKeJiTai:
				if (PlayerSt == PlayerTypeEnum.FeiJi) {
					isEnable = false;
				}
				break;
			}
		}
		//Debug.Log("SetEnableCamera -> player "+PlayerSt+", isEnable "+isEnable);
		PlayerCamera.enabled = isEnable;
	}

	public void ActivePlayerCamera()
	{
		bool isEnable = true;
		GameJiTaiType jiTai = XkGameCtrl.GameJiTaiSt;
		if (XkGameCtrl.GameModeVal == GameMode.LianJi && Network.peerType != NetworkPeerType.Disconnected) {
			switch (jiTai) {
			case GameJiTaiType.FeiJiJiTai:
				if (PlayerSt == PlayerTypeEnum.TanKe) {
					isEnable = false;
				}
				break;
				
			case GameJiTaiType.TanKeJiTai:
				if (PlayerSt == PlayerTypeEnum.FeiJi) {
					isEnable = false;
				}
				break;
			}
		}
		//Debug.Log("ActivePlayerCamera -> player "+PlayerSt+", isEnable "+isEnable+", jiTai "+jiTai);
		PlayerCamera.enabled = isEnable;
	}

	public bool GetActiveCamera()
	{
		return CameraObj.activeSelf;
	}

	void Update()
	{
		SmothMoveCamera();
		CheckMainCamera();
		//CheckStopCameraAimTranArray();
	}

	void FixedUpdate()
	{
		SmothMoveCamera();
	}

	void LateUpdate()
	{
		SmothMoveCamera();
	}

	void CheckMainCamera()
	{
		if (Camera.main == null || !Camera.main.enabled) {
			if (_InstanceFeiJi != null) {
				_InstanceFeiJi.ActivePlayerCamera();
			}
			else if (_InstanceTanKe != null) {
				_InstanceTanKe.ActivePlayerCamera();
			}
		}
	}

	public void SmothMoveCamera()
	{
		if (XKPlayerHeTiData.IsActiveHeTiPlayer) {
			if (PlayerSt == PlayerTypeEnum.FeiJi || PlayerSt == PlayerTypeEnum.TanKe) {
				this.enabled = false;
				return;
			}
		}

		if (CameraParent == null) {
			return;
		}

		if (PlayerScript.PlayerSt == PlayerTypeEnum.FeiJi
		    || PlayerScript.PlayerSt == PlayerTypeEnum.CartoonCamera) {
			if (Vector3.Distance(CameraTran.position, CameraParent.position) > 30f) {
				CameraTran.position = CameraParent.position;
				CameraTran.rotation = CameraParent.rotation;
			}
			else {
				CameraTran.position = Vector3.Lerp(CameraTran.position, CameraParent.position, Time.deltaTime);
			}
		}
		else {
			if (!CameraShake.IsCameraShake) {
				//CameraTran.position = CameraParent.position;
				if (Vector3.Distance(CameraTran.position, CameraParent.position) > 30f) {
					CameraTran.position = CameraParent.position;
					CameraTran.rotation = CameraParent.rotation;
				}
				else {
					CameraTran.position = Vector3.Lerp(CameraTran.position, CameraParent.position, Time.deltaTime);
				}
			}
		}
		SmothChangeCameraRot();
		
		if (PlayerScript.PlayerSt == PlayerTypeEnum.FeiJi) {
			if (ServerPortCameraCtrl.GetInstanceFJ() != null) {
				ServerPortCameraCtrl.GetInstanceFJ().CheckCameraFollowTran();
			}
		}
		else if (PlayerScript.PlayerSt == PlayerTypeEnum.TanKe) {
			if (ServerPortCameraCtrl.GetInstanceTK() != null) {
				ServerPortCameraCtrl.GetInstanceTK().CheckCameraFollowTran();
			}
		}
	}

	void SmothChangeCameraRot()
	{
		CheckAimNpcObj();
		if (AimTran == null) {
			if (IsOutAim) {
				float angle = Quaternion.Angle(CameraTran.rotation, CameraParent.rotation);
				//Debug.Log("angle ****** "+angle);
				if (angle <= 0.001f) {
					IsChangeSpeedOutAim = true;
				}

				if (IsChangeSpeedOutAim) {
					if (SpeedOutAim > GenZongCamRotVal) {
						SpeedOutAim -= GenZongTmpVal;
					}
					else {
						SpeedOutAim += GenZongTmpVal;
					}

					if (Mathf.Abs(SpeedOutAim - GenZongCamRotVal) <= (GenZongTmpVal * 1.5f)) {
						SpeedOutAim = GenZongCamRotVal;
					}
					IsOutAim = false;
				}
				CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation, CameraParent.rotation, SpeedOutAim * Time.deltaTime);
			}
			else {
				IsChangeSpeedOutAim = false;
				if (PlayerScript.PlayerSt == PlayerTypeEnum.FeiJi
				    || PlayerScript.PlayerSt == PlayerTypeEnum.CartoonCamera) {
					CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation, CameraParent.rotation, GenZongCamRotVal * Time.deltaTime);
				}
				else {
					if (Quaternion.Angle(CameraTran.rotation, CameraParent.rotation) > 30f) {
						CameraTran.rotation = CameraParent.rotation;
					}
					else {
						CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation,
						                                      CameraParent.rotation,
						                                      GenZongCamRotVal * Time.deltaTime);
					}
				}
			}
		}
		else {
			CheckAimTranObj();
		}
	}

	void CheckAimTranObj()
	{
		if (AimTran == null) {
			return;
		}

		Vector3 endPos = AimTran.position;
		Vector3 startPos = CameraTran.position;
		if (PlayerScript.PlayerSt == PlayerTypeEnum.TanKe) {
			endPos.y = startPos.y = 0f;
		}

		Vector3 forwardVal = endPos - startPos;
		if (forwardVal != Vector3.zero && forwardVal.magnitude > 0.001f) {
			Quaternion rotTmp = Quaternion.LookRotation(forwardVal);
			CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation, rotTmp, SpeedIntoAim * Time.deltaTime);
//			if (Quaternion.Angle(CameraTran.rotation, rotTmp) <= 5f
//			    && IsHandleCameraAimArray
//			    && CamAimArrayIndex < CameraAimArray.Length) {
//				AimTran = CameraAimArray[CamAimArrayIndex];
//				CamAimArrayIndex++;
//			}
		}
	}

	void ChangeAimTran(Transform aimVal)
	{
//		Debug.Log("ChangeAimTran...");
		if (aimVal == null) {
			if (AimTran != null) {
				IsOutAim = true;
			}
			else {
				IsOutAim = false;
			}
		}
		else {
			SpeedIntoAim = AimNpcSpeed;
			SpeedOutAim = LeaveNpcSpeed;
		}		
		AimTran = aimVal;
	}

	public void SetAimTranInfo(AiMark markScript)
	{
		if (AimNpcObj != null) {
//			Debug.Log("SetAimTranInfo -> AimNpcObj should be null");
			return;
		}

		Transform aimVal = markScript.PlayerCamAimTran;
//		PlayerStopTimeVal = markScript.TimePlayerAni;
//		if (PlayerStopTimeVal > 0f && markScript.PlayerAni == ZhiShengJiAction.Null) {
//			IsHandleCameraAimArray = true;
//			CamAimArrayIndex = 0;
			//CameraAimArray = markScript.CameraAimArray;
			//Invoke("StopCameraAimTranArray", PlayerStopTimeVal);
//		}

		if (aimVal == null) {
			if (AimTran != null) {
				IsOutAim = true;
			}
			else {
				IsOutAim = false;
			}
		}
		else {
			SpeedIntoAim = markScript.SpeedIntoAim;
			SpeedOutAim = markScript.SpeedOutAim;
			//Debug.Log("111*************SpeedOutAim "+SpeedOutAim);
		}

		AimTran = aimVal;
	}

//	void CheckStopCameraAimTranArray()
//	{
//		if (!IsHandleCameraAimArray) {
//			return;
//		}
//
//		if (Time.realtimeSinceStartup - StartCameraAimArrayTime < PlayerStopTimeVal) {
//			return;
//		}
//		StopCameraAimTranArray();
//	}

//	float StartCameraAimArrayTime;
//	void StopCameraAimTranArray()
//	{
//		IsOutAim = true;
//		IsHandleCameraAimArray = false;
//		CamAimArrayIndex = 0;
//		AimTran = null;
//	}

	public Transform GetAimTram()
	{
		return AimTran;
	}

	void CheckAimNpcObj()
	{
		float dTime = Time.realtimeSinceStartup - TimeCheckAimNpcLast;
		if (dTime < 0.05f) {
			return;
		}
		TimeCheckAimNpcLast = Time.realtimeSinceStartup;
		
		if (PlayerScript == null) {
			return;
		}
		
		if (PlayerScript.GetAimNpcObj() == null) {
			if (AimNpcObj != null) {
				AimNpcObj = null;
				ChangeAimTran(null);
			}
			return;
		}
		
		if (PlayerScript.GetAimNpcObj() != AimNpcObj) {
			AimNpcObj = PlayerScript.GetAimNpcObj(); //改变距离主角最近的npc.
			ChangeAimTran(AimNpcObj.transform);
		}
	}

	public void SetCameraAimNpcSpeed(float aimSpeed, float leaveSpeed)
	{
		AimNpcSpeed = aimSpeed;
		LeaveNpcSpeed = leaveSpeed;
	}
	
	/**
	 * key == 1 -> 使主角摄像机依附于父级摄像机并且停止跟踪.
	 */
	public void SetPlayerCameraTran(int key)
	{
		switch(key) {
		case 1:
			CameraTran.parent = CameraParent;
			CameraTran.localPosition = Vector3.zero;
			CameraTran.localEulerAngles = Vector3.zero;
			CameraParent = null; //stop move player camera pos.
			break;
		
		default:
			CameraTran.position = CameraParent.position;
			CameraTran.rotation = CameraParent.rotation;
			break;
		}
	}
}