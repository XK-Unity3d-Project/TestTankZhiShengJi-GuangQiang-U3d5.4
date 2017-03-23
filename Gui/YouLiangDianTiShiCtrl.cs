using UnityEngine;
using System.Collections;

public class YouLiangDianTiShiCtrl : MonoBehaviour {
	public GameObject YouXiangObj;
	public static bool IsTiShiOver;
	static YouLiangDianTiShiCtrl _Instance;
	public static YouLiangDianTiShiCtrl GetInstance()
	{
		return _Instance;
	}
	
	// Use this for initialization
	void Start()
	{
		_Instance = this;
		IsTiShiOver = false;
		gameObject.SetActive(false);
		YouXiangObj.SetActive(false);
	}
	
	public void ShowYouLiangDianTiShi()
	{
		if (gameObject.activeSelf) {
			return;
		}
		gameObject.SetActive(true);
		YouXiangObj.SetActive(true);
	}
	
	public void HiddenGameObj()
	{
		IsTiShiOver = true;
		YouXiangObj.SetActive(false);
		gameObject.SetActive(false);
		YouLiangTiShiCtrl.GetInstance().HiddenGameObj();
	}
}