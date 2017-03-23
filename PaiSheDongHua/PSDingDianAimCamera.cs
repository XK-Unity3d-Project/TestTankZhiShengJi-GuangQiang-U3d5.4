using UnityEngine;
using System.Collections;

public class PSDingDianAimCamera : MonoBehaviour {
	[Range(0.001f, 100f)]public float AimSpeed = 0.3f;
	Transform CameraTran;
	Camera CameraCom;
	Transform AimPlayerTran;
	public static GameObject DingDianAimCamera;
	// Use this for initialization
	void Awake()
	{
		if (XkGameCtrl.GetInstance ().IsCartoonShootTest) {
			camera.tag = "MainCamera";
			CameraCom = camera;
		}
		CameraTran = transform;
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update()
	{
		CheckCameraCom();
		if (!CameraCom.enabled) {
			gameObject.SetActive(false);
			return;
		}

		if (AimPlayerTran == null) {
			return;
		}
//		CameraTran.LookAt(AimPlayerTran);
		Vector3 forwardVal = AimPlayerTran.position - CameraTran.position;
		if (forwardVal != Vector3.zero) {
			Quaternion rotTmp = Quaternion.LookRotation(forwardVal);
			CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation, rotTmp, AimSpeed * Time.deltaTime);
		}
	}

	void CheckCameraCom()
	{
		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			return;
		}

		if (DingDianAimCamera == null || DingDianAimCamera != gameObject) {
			gameObject.SetActive(false);
		}
	}

	public void ActiveCamera(Transform playerTran)
	{
		AimPlayerTran = playerTran;
		if (Network.peerType == NetworkPeerType.Server || XkGameCtrl.GetInstance().IsServerCameraTest) {
			if (Camera.main.gameObject != XkGameCtrl.ServerCameraObj) {
				if (Camera.main != null) {
					Camera.main.enabled = false;
				}
			}

			if (CameraCom != null) {
				CameraCom.enabled = false;
			}
			DingDianAimCamera = gameObject;
			CameraCom = XkGameCtrl.ServerCameraObj.camera;
			XkGameCtrl.SetServerCameraTran(transform);
		}
		else {
			if (Camera.main != null) {
				Camera.main.enabled = false;
			}
			gameObject.SetActive(true);
		}
	}
}