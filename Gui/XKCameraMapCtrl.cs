using UnityEngine;
using System.Collections;

public class XKCameraMapCtrl : MonoBehaviour {
	Camera CameraCom;
	static XKCameraMapCtrl _Instance;
	public static XKCameraMapCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		CameraCom = camera;
		gameObject.tag = XkGameCtrl.TagNull;
		if (XkGameCtrl.GameModeVal != GameMode.LianJi || GameMovieCtrl.IsActivePlayer) {
			gameObject.SetActive(false);
			return;
		}
		SetCameraMapState();
	}

	public void SetCameraMapState()
	{
		CameraCom.enabled = !CameraCom.enabled;
		if (!CameraCom.enabled) {
			GameJiTaiType jiTai = XkGameCtrl.GameJiTaiSt;
			switch (jiTai) {
			case GameJiTaiType.FeiJiJiTai:
				XKPlayerCamera.GetInstanceTanKe().SetEnableCamera(false);
				break;

			case GameJiTaiType.TanKeJiTai:
				XKPlayerCamera.GetInstanceFeiJi().SetEnableCamera(false);
				break;
			}
		}
		else {
			GameJiTaiType jiTai = XkGameCtrl.GameJiTaiSt;
			switch (jiTai) {
			case GameJiTaiType.FeiJiJiTai:
				XKPlayerCamera.GetInstanceTanKe().SetEnableCamera(true);
				break;
				
			case GameJiTaiType.TanKeJiTai:
				XKPlayerCamera.GetInstanceFeiJi().SetEnableCamera(true);
				break;
			}
		}
	}

	public bool GetActiveCameraMap()
	{
		return CameraCom.enabled;
	}
}