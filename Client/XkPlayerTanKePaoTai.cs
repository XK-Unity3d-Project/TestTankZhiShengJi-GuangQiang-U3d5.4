using UnityEngine;
using System.Collections;

public class XkPlayerTanKePaoTai : MonoBehaviour {
	public GameObject PaoTaiRealObj;
	public GameObject TKLvDai;
	Transform AimPoint;
	Transform PaoTaiTran;
	Transform PlayerCameraTran;
	float PaoTaiSpeed = 0.8f;
	// Use this for initialization
	void Start()
	{
		if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
			PaoTaiRealObj.SetActive(false);
		}
		PaoTaiTran = transform;

		if (TKLvDai != null) {
			if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
				UVA scriptUV = TKLvDai.GetComponent<UVA>();
				if (scriptUV != null) {
					scriptUV.enabled = false;
				}

				MeshRenderer renderLvDai = TKLvDai.GetComponent<MeshRenderer>();
				if (renderLvDai != null) {
					renderLvDai.enabled = false;
				}
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		CheckTanKePaoTaiAimObj();
	}

	void FixedUpdate()
	{
		CheckTanKePaoTaiAimObj();
	}

	void CheckTanKePaoTaiAimObj()
	{
		if (PlayerCameraTran == null) {
			return;
		}

		if (AimPoint != null) {
			Vector3 endPos = AimPoint.position;
			Vector3 startPos = PaoTaiTran.position;
			endPos.y = startPos.y = 0f;
			if (Vector3.Distance(endPos, startPos) >= 2f) {
				Vector3 forwardVal = Vector3.Normalize(endPos - startPos);
				forwardVal.y = PaoTaiTran.forward.y;
				PaoTaiTran.forward = Vector3.Lerp(PaoTaiTran.forward, forwardVal, PaoTaiSpeed * Time.deltaTime);
			}
		}
		else {
			Vector3 forwardVal = PlayerCameraTran.forward;
			forwardVal.y = PaoTaiTran.forward.y;
			PaoTaiTran.forward = Vector3.Lerp(PaoTaiTran.forward, forwardVal, PaoTaiSpeed * Time.deltaTime);
		}
		
		Vector3 eulerAngles = PaoTaiTran.localEulerAngles;
		if (eulerAngles.x != 0f || eulerAngles.z != 0f) {
			eulerAngles.x = eulerAngles.z = 0f;
			PaoTaiTran.localEulerAngles = eulerAngles;
		}
	}

	public void SetAimPoint(Transform tran)
	{
		if (tran == AimPoint) {
			return;
		}
		AimPoint = tran;
	}

	public void SetPlayerCameraInfo(Transform tranCam)
	{
		PlayerCameraTran = tranCam;
	}

	public void ResetTransformRotation()
	{
		PaoTaiTran.localEulerAngles = Vector3.zero;
	}
}