using UnityEngine;
using System.Collections;

public class XKPlayerDongGanCtrl : MonoBehaviour {
	public PlayerTypeEnum PlayerSt = PlayerTypeEnum.TanKe;
	/**
QiNangStateTK[0] -> 前气囊
QiNangStateTK[1] -> 后气囊
QiNangStateTK[2] -> 左气囊
QiNangStateTK[3] -> 右气囊
	 */
	public static int[] QiNangStateTK = {0, 0, 0, 0};
	/**
QiNangStateFJ[0] -> 前气囊
QiNangStateFJ[1] -> 后气囊
QiNangStateFJ[2] -> 左气囊
QiNangStateFJ[3] -> 右气囊
	 */
	public static int[] QiNangStateFJ = {0, 0, 0, 0};
	Vector3 EulerAngle;
	// Use this for initialization
	void Start()
	{
		//GameTypeCtrl.AppTypeStatic = AppGameType.LianJiTanKe; //test.
		QiNangStateTK = new int[]{0, 0, 0, 0};
		QiNangStateFJ = new int[]{0, 0, 0, 0};
	}

	/**
	 * KeyZYQiNangState = 0 -> 左右气囊关闭.
	 * KeyZYQiNangState = 1 -> 左气囊关闭.
	 * KeyZYQiNangState = 2 -> 右气囊关闭.
	 */
	int KeyZYQiNangState;
	/**
	 * KeyQHQiNangState = 0 -> 前后气囊关闭.
	 * KeyQHQiNangState = 1 -> 前气囊关闭.
	 * KeyQHQiNangState = 2 -> 后气囊关闭.
	 */
	int KeyQHQiNangState;
	// Update is called once per frame
	void Update()
	{
//		if (pcvr.DongGanState == 0) {
//			return;
//		}
		
		if (DaoJiShiCtrl.GetInstance().GetIsPlayDaoJishi()
		    || (!XkGameCtrl.IsActivePlayerOne && !XkGameCtrl.IsActivePlayerTwo)) {
//			pcvr.CloseQiNangQian();
//			pcvr.CloseQiNangHou();
			return;
		}

		PlayerSt = PlayerTypeEnum.TanKe; //test.
		float eulerAngleX = 0f;
		float eulerAngleZ = 0f;
		float offsetAngle = 0f;
		switch (PlayerSt) {
		case PlayerTypeEnum.TanKe:
			EulerAngle = transform.eulerAngles;
			if (EulerAngle.x > 180f) {
				EulerAngle.x -= 360f;
			}
			
			if (EulerAngle.z > 180f) {
				EulerAngle.z -= 360f;
			}
			
			eulerAngleX = EulerAngle.x;
			eulerAngleZ = EulerAngle.z;
			offsetAngle = 0f;
			if (Mathf.Abs(eulerAngleX) <= offsetAngle) {
				//前后气囊放气.
				if (KeyQHQiNangState != 0) {
					QiNangStateTK[0] = 0;
					QiNangStateTK[1] = 0;
					KeyQHQiNangState = 0;
					if (KeyZYQiNangState == 0) {
						pcvr.CloseQiNangQian();
						pcvr.CloseQiNangHou();
					}
				}
			}
			else if  (eulerAngleX < 0f) {
				//前气囊充气,后气囊放气.
				if (KeyQHQiNangState != 1) {
					QiNangStateTK[0] = 1;
					QiNangStateTK[1] = 0;
					KeyQHQiNangState = 1;
					pcvr.OpenQiNangQian();
					pcvr.CloseQiNangHou(KeyZYQiNangState);
				}
			}
			else if (eulerAngleX > 0f) {
				//后气囊充气,前气囊放气.
				if (KeyQHQiNangState != 2) {
					QiNangStateTK[0] = 0;
					QiNangStateTK[1] = 1;
					KeyQHQiNangState = 2;
					pcvr.OpenQiNangHou();
					pcvr.CloseQiNangQian(KeyZYQiNangState);
				}
			}
			
			if (Mathf.Abs(eulerAngleZ) <= offsetAngle) {
				//左右气囊放气.
				if (KeyZYQiNangState != 0) {
					QiNangStateTK[2] = 0;
					QiNangStateTK[3] = 0;
					KeyZYQiNangState = 0;
					if (KeyQHQiNangState == 0) {
						pcvr.CloseQiNangZuo();
						pcvr.CloseQiNangYou();
					}
				}
			}
			else if (eulerAngleZ < 0f) {
				//左气囊充气,右气囊放气.
				if (KeyZYQiNangState != 1) {
					QiNangStateTK[2] = 1;
					QiNangStateTK[3] = 0;
					KeyZYQiNangState = 1;
					pcvr.OpenQiNangZuo();
					pcvr.CloseQiNangYou(KeyQHQiNangState);
				}
			}
			else if  (eulerAngleZ > 0f) {
				//右气囊充气,左气囊放气.
				if (KeyZYQiNangState != 2) {
					QiNangStateTK[2] = 0;
					QiNangStateTK[3] = 1;
					KeyZYQiNangState = 2;
					pcvr.OpenQiNangYou();
					pcvr.CloseQiNangZuo(KeyQHQiNangState);
				}
			}
			break;

		case PlayerTypeEnum.FeiJi:
			EulerAngle = transform.eulerAngles;
			if (EulerAngle.x > 180f) {
				EulerAngle.x -= 360f;
			}
			
			if (EulerAngle.z > 180f) {
				EulerAngle.z -= 360f;
			}
			eulerAngleX = EulerAngle.x;
			eulerAngleZ = EulerAngle.z;
			offsetAngle = 1f;

			if (Mathf.Abs(eulerAngleX) <= offsetAngle) {
				//前后气囊放气.
				QiNangStateFJ[0] = 0;
				QiNangStateFJ[1] = 0;
			}
			else if  (eulerAngleX < 0f) {
				//前气囊充气,后气囊放气.
				QiNangStateFJ[0] = 1;
				QiNangStateFJ[1] = 0;
			}
			else if (eulerAngleX > 0f) {
				//后气囊充气,前气囊放气.
				QiNangStateFJ[0] = 0;
				QiNangStateFJ[1] = 1;
			}
			
			if (Mathf.Abs(eulerAngleZ) <= offsetAngle) {
				//左右气囊放气.
				QiNangStateFJ[2] = 0;
				QiNangStateFJ[3] = 0;
			}
			else if  (eulerAngleZ > 0f) {
				//右气囊充气,左气囊放气.
				QiNangStateFJ[2] = 0;
				QiNangStateFJ[3] = 1;
			}
			else if (eulerAngleZ < 0f) {
				//左气囊充气,右气囊放气.
				QiNangStateFJ[2] = 1;
				QiNangStateFJ[3] = 0;
			}
			break;
		}
	}
}