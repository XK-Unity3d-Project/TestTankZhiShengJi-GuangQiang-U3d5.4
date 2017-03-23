using UnityEngine;
using System.Collections;

public class YouLiangTiShiCtrl : MonoBehaviour {
	public GameObject YouLiangBiaoObj;
	static YouLiangTiShiCtrl _Instance;
	public static YouLiangTiShiCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		YouLiangBiaoObj.SetActive(false);
		gameObject.SetActive(false);
	}

	public void ShowYouLiangTiShi()
	{
		if (gameObject.activeSelf) {
			return;
		}
		gameObject.SetActive(true);
		YouLiangBiaoObj.SetActive(true);

		Invoke("HiddenGameObj", 7f);
	}

	public void HiddenGameObj()
	{
		CancelInvoke("HiddenGameObj");
		YouLiangBiaoObj.SetActive(false);
		gameObject.SetActive(false);
		if (!YouLiangDianTiShiCtrl.IsTiShiOver) {
			YouLiangDianTiShiCtrl.GetInstance().ShowYouLiangDianTiShi();
		}
	}
}