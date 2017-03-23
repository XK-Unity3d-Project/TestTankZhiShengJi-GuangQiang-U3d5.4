using UnityEngine;
using System.Collections;

public class LookCamera : MonoBehaviour 
{
	public PlayerTypeEnum CameraType = PlayerTypeEnum.FeiJi;
	public bool IsNpcObj = false;
	Transform Tran;
	Transform MyCamera;
	static LookCamera _InstanceFJ;
	public static LookCamera GetInstanceFJ()
	{
		return _InstanceFJ;
	}
	static LookCamera _InstanceTK;
	public static LookCamera GetInstanceTK()
	{
		return _InstanceTK;
	}

	void Start() 
	{
		if (XkGameCtrl.GameModeVal != GameMode.LianJi && !IsNpcObj) {
			gameObject.SetActive(false);
			return;
		}

		Tran = transform;
		Invoke("DelaySetCameraInfo", 4f);
	}

	void DelaySetCameraInfo()
	{
		switch (CameraType) {
		case PlayerTypeEnum.FeiJi:
			if (!IsNpcObj) {
			    if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.FeiJiJiTai) {
					gameObject.SetActive(false);
				}
				_InstanceFJ = this;
			}

			if (XKPlayerCamera.GetInstanceTanKe() != null) {
				MyCamera = XKPlayerCamera.GetInstanceTanKe().transform;
			}
			break;

		case PlayerTypeEnum.TanKe:
			if (!IsNpcObj) {				
				if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
					gameObject.SetActive(false);
				}
				_InstanceTK = this;
			}

			if (XKPlayerCamera.GetInstanceFeiJi() != null) {
				MyCamera = XKPlayerCamera.GetInstanceFeiJi().transform;
			}
			break;
		}
	}

	void Update() 
	{
		if (Time.frameCount % 5 != 0) {
			return;
		}

		if (Tran == null || MyCamera == null) {
			if (MyCamera == null) {
				switch (CameraType) {
				case PlayerTypeEnum.FeiJi:
					if (XKPlayerCamera.GetInstanceTanKe() != null) {
						MyCamera = XKPlayerCamera.GetInstanceTanKe().transform;
					}
					break;
					
				case PlayerTypeEnum.TanKe:
					if (XKPlayerCamera.GetInstanceFeiJi() != null) {
						MyCamera = XKPlayerCamera.GetInstanceFeiJi().transform;
					}
					break;
				}
			}
			return;
		}
		Tran.LookAt(MyCamera.position);
	}

	public void SetLookCameraActive(bool isActive)
	{
		if (isActive == gameObject.activeSelf) {
			return;
		}
		gameObject.SetActive(isActive);
	}
}