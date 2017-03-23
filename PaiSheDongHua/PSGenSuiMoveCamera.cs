using UnityEngine;
using System.Collections;

public class PSGenSuiMoveCamera : MonoBehaviour {
	public PlayerTypeEnum PlayerSt = PlayerTypeEnum.FeiJi;
	public GameObject[] GenSuiMoveCam;
	public GameObject MainCamera; //游戏启动时使用的主摄像机.
	static PSGenSuiMoveCamera _InstanceFeiJi;
	public static PSGenSuiMoveCamera GetInstanceFeiJi()
	{
		return _InstanceFeiJi;
	}
	static PSGenSuiMoveCamera _InstanceTanKe;
	public static PSGenSuiMoveCamera GetInstanceTanKe()
	{
		return _InstanceTanKe;
	}

	// Use this for initialization
	void Awake()
	{
		switch (PlayerSt) {
		case PlayerTypeEnum.FeiJi:
			_InstanceFeiJi = this;
			break;
			
		case PlayerTypeEnum.TanKe:
			_InstanceTanKe = this;
			break;
		}

		for (int i = 0; i < GenSuiMoveCam.Length; i++) {
			if (MainCamera == GenSuiMoveCam[i]) {
				continue;
			}

			if (!XkGameCtrl.GetInstance().IsCartoonShootTest) {
				if (camera != null) {
					camera.enabled = false;
				}
			}
			GenSuiMoveCam[i].SetActive(false);
			GenSuiMoveCam[i].tag = "MainCamera";
		}

		/*if (MainCamera != null) {
			Camera.main.enabled = false;
			MainCamera.SetActive(true);
		}*/
	}

	public void ActiveGenSuiMoveCam(int indexVal)
	{
		if (indexVal < 0 || indexVal >= GenSuiMoveCam.Length || GenSuiMoveCam[indexVal] == null) {
			Debug.LogWarning("indexVal or GenSuiMoveCam[i] was wrong!");
			GameObject obj = null;
			obj.name = "null";
			return;
		}

		if (Network.peerType == NetworkPeerType.Server || XkGameCtrl.GetInstance().IsServerCameraTest) {
			if (Camera.main.gameObject != XkGameCtrl.ServerCameraObj) {
				if (Camera.main != null) {
					Camera.main.enabled = false;
				}
			}
			XkGameCtrl.SetServerCameraTran(GenSuiMoveCam[indexVal].transform);
		}
		else {
			if (Camera.main != camera) {
				if (Camera.main != null) {
					Camera.main.enabled = false;
				}
			}
			GenSuiMoveCam[indexVal].SetActive(true);
		}
	}
}
