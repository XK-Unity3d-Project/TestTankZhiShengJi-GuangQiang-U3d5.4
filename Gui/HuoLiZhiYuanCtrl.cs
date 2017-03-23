using UnityEngine;
using System.Collections;

public class HuoLiZhiYuanCtrl : MonoBehaviour {

	static HuoLiZhiYuanCtrl Instance;
	public static HuoLiZhiYuanCtrl GetInstance()
	{
		return Instance;
	}

	// Use this for initialization
	void Start()
	{
		Instance = this;
		SetIsActive(false);
	}

	public void SetIsActive(bool isActive)
	{
		gameObject.SetActive(isActive);
		if (isActive) {
			Invoke("HiddenHuoLiZhiYuan", 3f);
		}
	}

	void HiddenHuoLiZhiYuan()
	{
		gameObject.SetActive(false);
		XkPlayerCtrl.GetInstanceFeiJi().ExitPlayerLoopPath();
	}
}
