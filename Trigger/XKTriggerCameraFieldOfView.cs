using UnityEngine;
using System.Collections;

public class XKTriggerCameraFieldOfView : MonoBehaviour {
	[Range(1f, 179f)]public float CameraFieldView = 10f;
	[Range(0.01f, 10f)]public float WorldTimeScale = 0.5f;
	[Range(0.01f, 100f)]public float WorldTimeVal = 5f;
	[Range(0.01f, 10f)]public float TimeChangeCamFOV = 1.5f;
	float CameraFieldViewStart;
	TweenFOV TweenCamFOV;
	bool IsChangeWorldTime;
	GameObject CameraObj;
	Camera PlayerCamera;
	public AiPathCtrl TestPlayerPath;
	public static XKTriggerCameraFieldOfView Instance;
	void Start()
	{
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
		CheckIsHiddenObj();
	}
	
	void CheckIsHiddenObj()
	{
		bool isHiddenObj = false;
		if (Network.peerType == NetworkPeerType.Disconnected) {
			if (XkGameCtrl.GameModeVal != GameMode.LianJi
			    && XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
				isHiddenObj = true;
			}
		}
		else {
			if (Network.peerType == NetworkPeerType.Server
			    || XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
				isHiddenObj = true;
			}
		}
		
		if (isHiddenObj) {
			gameObject.SetActive(false);
		}
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

	void OnTriggerEnter(Collider other)
	{
		if (XKTriggerKaQiuShaFire.IsCloseKaQiuShaTest) {
			return; //test;
		}

		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}

		XkPlayerCtrl playerScript = other.GetComponent<XkPlayerCtrl>();
		if (playerScript == null) {
			return;
		}
		
		if (playerScript.PlayerSt == PlayerTypeEnum.FeiJi && XkGameCtrl.GameJiTaiSt != GameJiTaiType.FeiJiJiTai) {
			return;
		}
		
		if (playerScript.PlayerSt == PlayerTypeEnum.TanKe && XkGameCtrl.GameJiTaiSt != GameJiTaiType.TanKeJiTai) {
			return;
		}

		Instance = this;
		Debug.Log("XKTriggerCameraFieldOfView -> OnTriggerEnter...");
		XKPlayerCamera cameraScript = playerScript.GetPlayerCameraScript();
		CameraObj = cameraScript.gameObject;
		TweenCamFOV = CameraObj.GetComponent<TweenFOV>();
		if (TweenCamFOV != null) {
			DestroyObject(TweenCamFOV);
		}
		PlayerCamera = CameraObj.GetComponent<Camera>();
		CameraFieldViewStart = PlayerCamera.fieldOfView;

		TweenCamFOV = CameraObj.AddComponent<TweenFOV>();
		TweenCamFOV.enabled = false;
		TweenCamFOV.from = CameraFieldViewStart;
		TweenCamFOV.to = CameraFieldView;
		TweenCamFOV.duration = TimeChangeCamFOV;
		EventDelegate.Add(TweenCamFOV.onFinished, delegate{
			ChangeWorldTimeVal(0);
		});
		TweenCamFOV.enabled = true;
		TweenCamFOV.PlayForward();
	}

	public void ChangeWorldTimeVal(int key = 0)
	{
		if (IsChangeWorldTime) {
			return;
		}
		Debug.Log("XKTriggerCameraFieldOfView::ChangeWorldTimeVal -> key "+key
		          +", fieldOfView "+PlayerCamera.fieldOfView);
		IsChangeWorldTime = true;
		Time.timeScale = WorldTimeScale;
		if (key == 1 && Network.peerType != NetworkPeerType.Server) {
			if (TweenCamFOV != null) {
				TweenCamFOV.enabled = false;
				DestroyObject(TweenCamFOV);
			}
			PlayerCamera.fieldOfView = CameraFieldView;
			Debug.Log("XKTriggerCameraFieldOfView::ChangeWorldTimeVal -> fieldOfView "+PlayerCamera.fieldOfView);
		}
	}

	public void ResetWorldTimeVal()
	{
		Debug.Log("XKTriggerCameraFieldOfView -> ResetWorldTimeVal...");
		if (!IsChangeWorldTime) {
			return;
		}
		IsChangeWorldTime = false;
		Time.timeScale = 1f;
		
		TweenCamFOV = CameraObj.GetComponent<TweenFOV>();
		if (TweenCamFOV != null) {
			DestroyObject(TweenCamFOV);
		}

		TweenCamFOV = CameraObj.AddComponent<TweenFOV>();
		TweenCamFOV.enabled = false;
		TweenCamFOV.from = CameraFieldView;
		TweenCamFOV.to = CameraFieldViewStart;
		TweenCamFOV.duration = TimeChangeCamFOV;
		EventDelegate.Add(TweenCamFOV.onFinished, delegate{
			EndPlayerCameraFOV();
		});
		TweenCamFOV.enabled = true;
		TweenCamFOV.PlayForward();
	}

	void EndPlayerCameraFOV()
	{
		XKTriggerClosePlayerUI.ResetIsClosePlayerUI();
	}
}