using UnityEngine;
using System.Collections;

public class PlayerZhiShengJiCtrl : MonoBehaviour {
	public Vector3 MessCenterVal;
	public bool IsTestRigMessCenter;
	public Transform TestRigMessCenter;
	PlayerTypeEnum PlayerType;
	Rigidbody RigidbodyCom;
	float TimeCheckPlayerActive;
	Transform TranParent;
	XkPlayerCtrl PlayerScript;
	// Use this for initialization
	void Awake()
	{
		PlayerScript = GetComponentInParent<XkPlayerCtrl>();
		PlayerScript.SetPlayerZhiShengJiScript(this);
		PlayerType = PlayerScript.PlayerSt;
		RigidbodyCom = rigidbody;
		switch (PlayerType) {
		case PlayerTypeEnum.FeiJi:
			break;
			
		case PlayerTypeEnum.TanKe:
			RigidbodyCom.useGravity = false;
			RigidbodyCom.centerOfMass = MessCenterVal;
			RigidbodyCom.maxAngularVelocity = 2f;
			if (XkGameCtrl.GameJiTaiSt != GameJiTaiType.TanKeJiTai) {
				if (Network.peerType != NetworkPeerType.Disconnected) {
					RigidbodyCom.isKinematic = true;
				}
				//RigidbodyCom.isKinematic = true; //test
			}
			Invoke("DelaySetTanKeParent", 2f);
			break;
		}
		IsTestRigMessCenter = false;
	}

	void DelaySetTanKeParent()
	{
		TranParent = transform.parent;
		transform.parent = null;
	}

	void Update()
	{
		if (!ScreenDanHeiCtrl.IsStartGame) {
			return;
		}

		if (PlayerScript.PlayerSt != PlayerTypeEnum.TanKe) {
			return;
		}
		CheckPlayerTanKeAngle();

		if (XkGameCtrl.GameJiTaiSt != GameJiTaiType.TanKeJiTai) {
			return;
		}

		CheckPlayerIsActive();
		CheckRigidbodyMassCenter();
	}

	public void MakePlayerTanKeMoveToLand()
	{
		if (PlayerScript.PlayerSt != PlayerTypeEnum.TanKe) {
			return;
		}

		if (TranParent == null) {
			return;
		}

		if (XKTriggerClosePlayerUI.IsActiveHeTiCloseUI) {
			if (transform.position != TranParent.position) {
				transform.position = TranParent.position;
			}
			return;
		}

		RaycastHit hitInfo;
		Vector3 offsetPos = new Vector3(0f, 0.5f, 0f);
		Vector3 startPos = TranParent.position + (Vector3.up * 2f);
		Vector3 forwardVal = Vector3.down;
		Physics.Raycast(startPos, forwardVal, out hitInfo, 20f, XkGameCtrl.GetInstance().LandLayer.value);
		if (hitInfo.collider != null){
			if (Vector3.Distance(transform.position, hitInfo.point) > 20f) {
				transform.position = TranParent.position;
			}
			else {
				Vector3 posTmp = hitInfo.point + offsetPos;
				transform.position = Vector3.Lerp(transform.position, posTmp, 0.05f);
			}
		}
		else {
			if (Mathf.Abs(transform.position.y - TranParent.position.y) > 1f) {
				transform.position = TranParent.position;
			}
		}
	}

	void CheckPlayerTanKeAngle()
	{
		if (TranParent == null) {
			return;
		}

		Vector3 eulerAngles = transform.localEulerAngles;
		if (XKTriggerClosePlayerUI.IsActiveHeTiCloseUI) {
			if (eulerAngles != TranParent.localEulerAngles) {
				transform.localEulerAngles = TranParent.localEulerAngles;
			}
			return;
		}

		float minAnglex = 15f;
		bool isChangeAngle = false;
		if (eulerAngles.x > minAnglex && eulerAngles.x <= 180f) {
			eulerAngles.x = minAnglex;
			isChangeAngle = true;
		}
		else if (eulerAngles.x < (360f - minAnglex) && eulerAngles.x >= 180f) {
			eulerAngles.x = (360f - minAnglex);
			isChangeAngle = true;
		}

		float minAngleZ = 35f;
		if (eulerAngles.z > minAngleZ && eulerAngles.z <= 180f) {
			eulerAngles.z = minAngleZ;
			isChangeAngle = true;
		}
		else if (eulerAngles.z < (360f - minAngleZ) && eulerAngles.z >= 180f) {
			eulerAngles.z = (360f - minAngleZ);
			isChangeAngle = true;
		}
		eulerAngles.y = TranParent.localEulerAngles.y;

		if (RigidbodyCom.isKinematic) {
			transform.localEulerAngles = TranParent.localEulerAngles;
			return;
		}

		RigidbodyCom.velocity = Vector3.zero;
		if (isChangeAngle) {
			RigidbodyCom.angularVelocity = Vector3.zero;
		}
		//Debug.Log("localEulerAngles ** "+transform.localEulerAngles);
		transform.localEulerAngles = eulerAngles;
	}

	void CheckTanKeLocalPostion()
	{
		transform.position = TranParent.position;

//		if (posLoc.x != 0f) {
//			posLoc.x = 0f;
//			isChangePos = true;
//		}
//		
//		if (posLoc.z != 0f) {
//			posLoc.z = 0f;
//			isChangePos = true;
//		}
//		
//		if (isChangePos) {
//			transform.localPosition = posLoc;
//		}

		if (ServerPortCameraCtrl.GetInstanceTK() != null) {
			ServerPortCameraCtrl.GetInstanceTK().CheckCameraFollowTran();
		}
	}

	void CheckPlayerIsActive()
	{
		float dTime = Time.realtimeSinceStartup - TimeCheckPlayerActive;
		if (dTime < 0.2f) {
			return;
		}
		TimeCheckPlayerActive = Time.realtimeSinceStartup;

		if (!XkGameCtrl.IsMoveOnPlayerDeath) {
			if (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo) {
				if (RigidbodyCom.useGravity) {
					RigidbodyCom.useGravity = false;
				}
				return;
			}
		}
		
		if (!RigidbodyCom.useGravity) {
			RigidbodyCom.useGravity = true;
		}
	}

	void CheckRigidbodyMassCenter()
	{
		if (!IsTestRigMessCenter) {
			return;
		}
		RigidbodyCom.centerOfMass = MessCenterVal;
		if (TestRigMessCenter != null) {
			TestRigMessCenter.position = RigidbodyCom.centerOfMass + transform.position;
		}
	}

	public PlayerTypeEnum GetPlayerType()
	{
		return PlayerType;
	}

	public void ClosePlayerRigidbody()
	{
		if (PlayerScript.PlayerSt != PlayerTypeEnum.TanKe) {
			return;
		}
		RigidbodyCom.isKinematic = true;
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
	}
}

public enum ZhiShengJiAction
{
	Null,
	Root1,
	Root2,
}