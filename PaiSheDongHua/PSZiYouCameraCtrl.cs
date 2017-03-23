using UnityEngine;
using System.Collections;

public class PSZiYouCameraCtrl : MonoBehaviour {
	Transform CameraTran;
	Transform AimTran;
	Transform CameraParent;
	float SpeedIntoAim = 0.2f;
	float SpeedOutAim = 1f;
	bool IsOutAim;
	float GenZongTmpVal = 0.0001f;
	float GenZongCamRotVal = 0.2f;
	bool IsChangeSpeedOutAim;
	// Use this for initialization
	void Start()
	{
		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			camera.targetTexture = null;
		}
		CameraTran = transform;
		
		GameObject obj = new GameObject();
		obj.name = "CameraParent";
		CameraParent = obj.transform;
		CameraParent.parent = CameraTran.parent;
		CameraParent.localPosition = CameraTran.localPosition;
		CameraParent.rotation = CameraTran.rotation;
		CameraTran.parent = null;
	}

	void Update()
	{
		SmothMoveCamera();
	}
	
	void FixedUpdate()
	{
		SmothMoveCamera();
	}
	
	void LateUpdate()
	{
		SmothMoveCamera();
	}
	
	public void SmothMoveCamera()
	{
		if (CameraParent == null) {
			return;
		}

		if (Vector3.Distance(CameraTran.position, CameraParent.position) > 30f) {
			CameraTran.position = CameraParent.position;
			CameraTran.rotation = CameraParent.rotation;
		}
		else {
			CameraTran.position = Vector3.Lerp(CameraTran.position, CameraParent.position, Time.deltaTime);
		}
		SmothChangeCameraRot();
	}
	
	void SmothChangeCameraRot()
	{
		if (AimTran == null) {
			if (IsOutAim) {
				float angle = Quaternion.Angle(CameraTran.rotation, CameraParent.rotation);
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
				CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation, CameraParent.rotation, GenZongCamRotVal * Time.deltaTime);
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
		
		Vector3 forwardVal = AimTran.position - CameraTran.position;
		if (forwardVal != Vector3.zero) {
			Quaternion rotTmp = Quaternion.LookRotation(forwardVal);
			CameraTran.rotation = Quaternion.Lerp(CameraTran.rotation, rotTmp, SpeedIntoAim * Time.deltaTime);
		}
	}
	
	public void ChangeAimTran(Transform aimVal, AiMark markScript)
	{
//		string info = "ChangeAimTran...";
//		info += aimVal != null ? "aim "+aimVal.name : "";
//		Debug.Log(info);
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
		}
		AimTran = aimVal;
	}
}