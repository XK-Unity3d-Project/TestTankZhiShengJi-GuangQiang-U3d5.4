using UnityEngine;
using System.Collections;

public class XKPlayerGunCtrl : MonoBehaviour {
	public PlayerEnum PlayerSt = PlayerEnum.Null;
	public LayerMask FireLayer;
	public Transform PaoGuan;
	[Range(-90f, 90f)]public float UpPaoGuanJDVal = -45f;
	[Range(-90f, 90f)]public float DownPaoGuanJDVal = 45f;
	[Range(0f, 180f)]public float MaxPaoShenJDVal = 10f;
	[Range(0f, -180f)]public float MinPaoShenJDVal = -10f;
	Transform CannonTran;
	Transform AmmoStartPos;
	float FireRayDirLen = 500f;
	const float AngleMin = 1f;
	const float AngleMax = 89f;
	float CosAngleMin = Mathf.Cos((AngleMin / 180f) * Mathf.PI);
	float CosAngleDP = Mathf.Cos((45 / 180f) * Mathf.PI);
	float CosAngleUp = 0f;
	float CosAngleDown = 0f;
	float OffsetForward = 30f;
	float FirePosValTmp = 1000f;
	// Use this for initialization
	void Start()
	{
		if (UpPaoGuanJDVal >= DownPaoGuanJDVal) {
			Debug.LogError("XKCannonCtrl-> MaxPaoGuanJDVal was wrong!");
			GameObject obj = null;
			obj.name = "null";
		}
		
		if (MaxPaoShenJDVal < MinPaoShenJDVal) {
			Debug.LogError("XKCannonCtrl-> MaxPaoShenJDVal was wrong!");
			GameObject obj = null;
			obj.name = "null";
		}
		
		CosAngleUp = Mathf.Cos((UpPaoGuanJDVal / 180f) * Mathf.PI);
		CosAngleDown = Mathf.Cos((DownPaoGuanJDVal / 180f) * Mathf.PI);
		
		CannonTran = transform;
		AmmoStartPos = PaoGuan;
	}

	// Update is called once per frame
	void Update()
	{
		if (!XkGameCtrl.IsActivePlayerOne && PlayerSt == PlayerEnum.PlayerOne) {
			return;
		}

		if (!XkGameCtrl.IsActivePlayerTwo && PlayerSt == PlayerEnum.PlayerTwo) {
			return;
		}

		if (Camera.main == null) {
			return;
		}
		
		Vector3 posA = Vector3.zero;
		Vector3 mousePosInput = Input.mousePosition;
		if (pcvr.bIsHardWare) {
			if (PlayerSt == PlayerEnum.PlayerOne) {
				mousePosInput = pcvr.CrossPositionOne;
			}
			else {
				mousePosInput = pcvr.CrossPositionTwo;
			}
		}
		
		Vector3 ammoSpawnPos = AmmoStartPos.position;
		Vector3 firePos = Vector3.zero;
		Vector3 mousePos = mousePosInput + Vector3.forward * OffsetForward;
		Vector3 posTmp = Camera.main.ScreenToWorldPoint(mousePos);
		Vector3 ammoForward = Vector3.Normalize( posTmp - ammoSpawnPos );
		firePos = FirePosValTmp * ammoForward + ammoSpawnPos;
		Ray ray = Camera.main.ScreenPointToRay(mousePosInput);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, FireRayDirLen, FireLayer.value)) {
			posA = hit.point;
		}
		else {
			posA = firePos;
		}
		
		Vector3 posASave = posA;
		Vector3 posB = CannonTran.position;
		posA.y = posB.y = 0f;
		Vector3 forwardVal = posA - posB;
		forwardVal.y = CannonTran.forward.y;
		CannonTran.forward = Vector3.Lerp(CannonTran.forward, forwardVal.normalized, Time.deltaTime);
		
		Vector3 eulerAnglesPS = CannonTran.localEulerAngles;
		eulerAnglesPS.x = eulerAnglesPS.z = 0f;
		float angleY = eulerAnglesPS.y > 180 ? -(360 - eulerAnglesPS.y) : eulerAnglesPS.y;
		if (angleY > MaxPaoShenJDVal || angleY < MinPaoShenJDVal) {
			angleY = angleY > MaxPaoShenJDVal ? MaxPaoShenJDVal : angleY;
			eulerAnglesPS.y = angleY > MinPaoShenJDVal ? angleY : MinPaoShenJDVal;
		}
		CannonTran.localEulerAngles = eulerAnglesPS;
		
		MakePaoGuanAimPlayer(posASave);
	}

	void MakePaoGuanAimPlayer(Vector3 playerPos)
	{
		Vector3 posA = playerPos;
		Vector3 posB = PaoGuan.position;
		Vector3 posBTmp = PaoGuan.position;
		Vector3 vecA = Vector3.Normalize(posA - posB);
		posBTmp.y = posA.y;
		
		Vector3 vecC = CannonTran.forward;
		float cosAC = Vector3.Dot(vecA, vecC);
		if (cosAC < CosAngleDP) {
			return;
		}
		
		Vector3 vecB = Vector3.Normalize(posA - posBTmp);
		float cosAB = Vector3.Dot(vecA, vecB);
		if (cosAB > CosAngleMin) {
			Vector3 eulerAnglesTmp = Vector3.zero;
			eulerAnglesTmp.x = posA.y >= posB.y ? (-AngleMin) : AngleMin;
			if (Mathf.Abs(PaoGuan.localEulerAngles.x) > AngleMin) {
				Vector3 eulerAnglesTmpVal = PaoGuan.localEulerAngles;
				if (eulerAnglesTmpVal.x > 90f) {
					eulerAnglesTmpVal.x = eulerAnglesTmpVal.x - 360f;
				}
				eulerAnglesTmpVal = Vector3.Lerp(eulerAnglesTmpVal, eulerAnglesTmp, Time.deltaTime);
				eulerAnglesTmpVal.x = eulerAnglesTmpVal.x != 0f ? eulerAnglesTmpVal.x : eulerAnglesTmp.x;
				PaoGuan.localEulerAngles = eulerAnglesTmpVal;
			}
			return;
		}
		
		if (posA.y > posB.y && cosAB < CosAngleUp) {
			Vector3 eulerAnglesTmp = Vector3.zero;
			eulerAnglesTmp.x = UpPaoGuanJDVal;
			Vector3 eulerAnglesTmpVal = PaoGuan.localEulerAngles;
			if (eulerAnglesTmpVal.x > 90f) {
				eulerAnglesTmpVal.x = eulerAnglesTmpVal.x - 360f;
			}
			eulerAnglesTmpVal = Vector3.Lerp(eulerAnglesTmpVal, eulerAnglesTmp, Time.deltaTime);
			PaoGuan.localEulerAngles = eulerAnglesTmpVal;
			return;
		}
		
		if (posA.y < posB.y && cosAB < CosAngleDown) {
			Vector3 eulerAnglesTmp = Vector3.zero;
			eulerAnglesTmp.x = DownPaoGuanJDVal;
			Vector3 eulerAnglesTmpVal = PaoGuan.localEulerAngles;
			if (eulerAnglesTmpVal.x > 90f) {
				eulerAnglesTmpVal.x = eulerAnglesTmpVal.x - 360f;
			}
			eulerAnglesTmpVal = Vector3.Lerp(eulerAnglesTmpVal, eulerAnglesTmp, Time.deltaTime);
			PaoGuan.localEulerAngles = eulerAnglesTmpVal;
			return;
		}
		
		PaoGuan.forward = Vector3.Lerp(PaoGuan.forward, vecA, Time.deltaTime);
		Vector3 eulerAngles = PaoGuan.localEulerAngles;
		eulerAngles.y = eulerAngles.z = 0f;
		PaoGuan.localEulerAngles = eulerAngles;
	}
}