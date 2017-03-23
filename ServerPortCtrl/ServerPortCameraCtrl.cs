using UnityEngine;
using System.Collections;

public class ServerPortCameraCtrl : MonoBehaviour {
	public PlayerTypeEnum CameraType = PlayerTypeEnum.FeiJi;
	[Range(0.001f, 1f)]public float FeiJiFollowSpeed = 0.015f;
	float MinRandTime;
	float MaxRandTime;
	static bool IsFollowTest = true;
	static int CountFJ;
	static int CountTK;
	int CameraIndex = 0;
	Transform FollowTran;
	Transform CameraTran;
	GameObject CameraObj;
	static ServerPortCameraCtrl _InstanceFJ;
	public static ServerPortCameraCtrl GetInstanceFJ()
	{
		return _InstanceFJ;
	}
	static ServerPortCameraCtrl _InstanceTK;
	public static ServerPortCameraCtrl GetInstanceTK()
	{
		return _InstanceTK;
	}

	// Use this for initialization
	void Awake()
	{
		Debug.Log("Init serverPortCamera -> CameraType "+CameraType);
		switch (CameraType) {
		case PlayerTypeEnum.FeiJi:
			_InstanceFJ = this;
			break;
			
		case PlayerTypeEnum.TanKe:
			_InstanceTK = this;
			break;
		}
		CameraTran = transform;
		CameraObj = gameObject;

		MinRandTime = XkGameCtrl.GetInstance().MinRandTimeServer;
		MaxRandTime = XkGameCtrl.GetInstance().MaxRandTimeServer;
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		CheckServerPortCamera();
		CheckCameraFollowTran();
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.F)) {
			IsFollowTest = !IsFollowTest;
		}
		CheckCameraFollowTran();
	}

	void LateUpdate()
	{
		CheckCameraFollowTran();
	}

	public void CheckCameraFollowTran()
	{
		if (!IsFollowTest) {
			return;
		}

		if (FollowTran == null) {
			return;
		}
		
		if (Vector3.Distance(CameraTran.position, FollowTran.position) > 50f) {
			CameraTran.position = FollowTran.position;
			CameraTran.rotation = FollowTran.rotation;
			return;
		}

		switch (CameraType) {
		case PlayerTypeEnum.FeiJi:
			CameraTran.position = Vector3.Lerp(CameraTran.position, FollowTran.position, FeiJiFollowSpeed);
			break;

		case PlayerTypeEnum.TanKe:
			CameraTran.position = FollowTran.position;
			break;
		}
		CameraTran.rotation = Quaternion.Slerp(CameraTran.rotation, FollowTran.rotation, 0.015f);
	}

	void GetCameraFollowTran()
	{
		CameraIndex = (CameraIndex + 1 + (Random.Range(0, 100) % 3)) % 4;
//		Debug.Log("GetCameraFollowTran -> indexCam "+CameraIndex+", CameraType "+CameraType);
//		Debug.Log("CountFJ "+CountFJ+", CountTK "+CountTK);
		Transform tran = null;
		switch (CameraType) {
		case PlayerTypeEnum.FeiJi:
			_InstanceTK.CloseCameraServer();
			tran = XkPlayerCtrl.GetInstanceFeiJi().PlayerCamPoint[CameraIndex];
			break;
			
		case PlayerTypeEnum.TanKe:
			_InstanceFJ.CloseCameraServer();
			tran = XkPlayerCtrl.GetInstanceTanKe().PlayerCamPoint[CameraIndex];
			break;
		}
		FollowTran = tran;
	}

	void CloseCameraServer()
	{
		if (IsInvoking("DelayRandOpenServerPortCamera")) {
			CancelInvoke("DelayRandOpenServerPortCamera");
		}
		CameraObj.SetActive(false);
	}

	public void ActiveServerPortCamera()
	{
		if (XkPlayerCtrl.GetInstanceFeiJi() == null && XkPlayerCtrl.GetInstanceTanKe() == null) {
			return;
		}

		switch (CameraType) {
		case PlayerTypeEnum.FeiJi:
			if (XkPlayerCtrl.GetInstanceFeiJi() == null) {
				_InstanceTK.ActiveServerPortCamera();
				return;
			}
			break;

		case PlayerTypeEnum.TanKe:
			if (XkPlayerCtrl.GetInstanceTanKe() == null) {
				_InstanceFJ.ActiveServerPortCamera();
				return;
			}
			break;
		}

		GetCameraFollowTran();
		if (FollowTran != null) {
			if (IsFollowTest) {
				CameraTran.position = FollowTran.position;
				CameraTran.rotation = FollowTran.rotation;
			}
			else {
				CameraTran.parent = FollowTran;
				CameraTran.localPosition = Vector3.zero;
				CameraTran.localEulerAngles = Vector3.zero;
			}
			CameraObj.SetActive(true);

			switch (CameraType) {
			case PlayerTypeEnum.FeiJi:
				if (LookCamera.GetInstanceFJ() != null) {
					LookCamera.GetInstanceFJ().SetLookCameraActive(false);
				}

				if (LookCamera.GetInstanceTK() != null) {
					LookCamera.GetInstanceTK().SetLookCameraActive(true);
				}
				break;

			case PlayerTypeEnum.TanKe:
				if (LookCamera.GetInstanceFJ() != null) {
					LookCamera.GetInstanceFJ().SetLookCameraActive(true);
				}

				if (LookCamera.GetInstanceTK() != null) {
					LookCamera.GetInstanceTK().SetLookCameraActive(false);
				}
				break;
			}
		}

		float randTime = Random.Range(MinRandTime, MaxRandTime);
		Invoke("DelayRandOpenServerPortCamera", randTime);
	}

	void CheckServerPortCamera()
	{
		switch (CameraType) {
		case PlayerTypeEnum.FeiJi:
			if (XkPlayerCtrl.GetInstanceFeiJi() == null) {
				if (XkPlayerCtrl.GetInstanceTanKe() != null) {
					CloseCameraServer();
					ServerPortCameraCtrl.GetInstanceTK().ActiveServerPortCamera();
				}
				return;
			}
			break;
			
		case PlayerTypeEnum.TanKe:
			if (XkPlayerCtrl.GetInstanceTanKe() == null) {
				if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
					CloseCameraServer();
					ServerPortCameraCtrl.GetInstanceFJ().ActiveServerPortCamera();
				}
				return;
			}
			break;
		}
	}

	void DelayRandOpenServerPortCamera()
	{
		RandOpenServerPortCamera();
	}

	public static void RandOpenServerPortCamera()
	{
//		bool isTestTK = false;
//		if (isTestTK) {
//			ServerPortCameraCtrl.GetInstanceTK().ActiveServerPortCamera();
//			return;
//		}

		int randVal = Random.Range(0, 100) % 2;
		if (randVal == 0) {
			CountTK = 0;
			CountFJ++;
			if (CountFJ > 2) {
				CountFJ = 0;
				CountTK++;
				ServerPortCameraCtrl.GetInstanceTK().ActiveServerPortCamera();
				return;
			}
			ServerPortCameraCtrl.GetInstanceFJ().ActiveServerPortCamera();
		}
		else {
			CountFJ = 0;
			CountTK++;
			if (CountTK > 2) {
				CountTK = 0;
				CountFJ++;
				ServerPortCameraCtrl.GetInstanceFJ().ActiveServerPortCamera();
				return;
			}
			ServerPortCameraCtrl.GetInstanceTK().ActiveServerPortCamera();
		}
	}

	public static void CloseAllServerPortCamera()
	{
		Debug.Log("CloseAllServerPortCamera...");
		_InstanceFJ.CloseCameraServer();
		_InstanceTK.CloseCameraServer();
		
		if (XkPlayerCtrl.GetInstanceFeiJi() == null && XkPlayerCtrl.GetInstanceTanKe() == null) {
			return;
		}

		if (XkPlayerCtrl.GetInstanceFeiJi() != null && XkPlayerCtrl.GetInstanceTanKe() != null) {
			XKPlayerCamera.GetInstanceFeiJi().SetEnableCamera(true);
			return;
		}

		if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
			XKPlayerCamera.GetInstanceFeiJi().SetEnableCamera(true);
		}

		if (XkPlayerCtrl.GetInstanceTanKe() != null) {
			XKPlayerCamera.GetInstanceTanKe().SetEnableCamera(true);
		}
	}
}