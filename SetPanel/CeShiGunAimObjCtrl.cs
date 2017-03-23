using UnityEngine;
using System.Collections;

public class CeShiGunAimObjCtrl : MonoBehaviour {
	
	public int AimObjCount;
	public static int [] AimObjKeyArray = new int[3];

	Transform GunCrossTran;
	Transform ObjTran;

	// Use this for initialization
	void Start () {
		ObjTran = transform;
		GunCrossTran = SetPanelUiRoot.GetInstance().GunCrossTran;
	}

	// Update is called once per frame
	void Update () {

		bool isClickFireBtDown = false;
		if (InputEventCtrl.IsClickFireBtOneDown || InputEventCtrl.IsClickFireBtTwoDown) {
			isClickFireBtDown = true;
		}

		if (!isClickFireBtDown) {
			SetPanelUiRoot.GetInstance().SetHitAimObjInfoActive(false);
			return;
		}

		Vector3 vecA = GunCrossTran.localPosition;
		Vector3 vecB = ObjTran.localPosition;
		vecA.z = vecB.z = 0f;

		float dis = Vector3.Distance(vecA, vecB);
		if (dis <= 99f) {
			SetPanelUiRoot.GetInstance().SetHitAimObjInfoActive(true);
			AimObjKeyArray[AimObjCount] = 1;
		}
		else {
			bool isHitObj = false;
			AimObjKeyArray[AimObjCount] = 0;
			for (int i = 0; i < AimObjKeyArray.Length; i++)
			{
				if (AimObjKeyArray[i] == 1) {
					isHitObj = true;
					break;
				}
			}

			if (!isHitObj) {
				SetPanelUiRoot.GetInstance().SetHitAimObjInfoActive(false);
			}
		}
	}
}
