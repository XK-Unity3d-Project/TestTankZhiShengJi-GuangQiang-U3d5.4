using UnityEngine;
using System.Collections;

public class XKPlayerHeTiData : MonoBehaviour
{
	public GameObject HeTiPlayerObj;
	public Transform FeiJiPlayerPoint;
	public Transform TanKePlayerPoint;
	public static bool IsActiveHeTiPlayer;
	static XKPlayerHeTiData _Instance;
	public static XKPlayerHeTiData GetInstance()
	{
		return _Instance;
	}

	void Start()
	{
		_Instance = this;
		IsActiveHeTiPlayer = false;
		HeTiPlayerObj.SetActive(false);
	}

	public void ShowHeTiPlayerObj()
	{
		IsActiveHeTiPlayer = true;
		Transform playerTran = null;
		if (XkPlayerCtrl.GetInstanceFeiJi() != null) {
			XkPlayerCtrl.GetInstanceFeiJi().StopMovePlayer();
			playerTran = XkPlayerCtrl.GetInstanceFeiJi().transform;
			playerTran.parent = FeiJiPlayerPoint;
			playerTran.localPosition = Vector3.zero;
			playerTran.localEulerAngles = Vector3.zero;
			XkPlayerCtrl.GetInstanceFeiJi().SetPlayerCameraTran(1);
		}

		if (XkPlayerCtrl.GetInstanceTanKe() != null) {
			XkPlayerCtrl.GetInstanceTanKe().StopMovePlayer();
			XkPlayerCtrl.GetInstanceTanKe().ClosePlayerRigidbody();
			playerTran = XkPlayerCtrl.GetInstanceTanKe().transform;
			playerTran.parent = TanKePlayerPoint;
			playerTran.localPosition = Vector3.zero;
			playerTran.localEulerAngles = Vector3.zero;
			XkPlayerCtrl.GetInstanceTanKe().SetPlayerCameraTran(1);
		}
		//HeTiPlayerObj.SetActive(true);
	}
}