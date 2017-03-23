using UnityEngine;
using System.Collections;

public class DanKengCtrl : MonoBehaviour {
	public GameObject[] DanKengArray;
	bool IsShowDanKeng;
	int DanKengIndex;
	static DanKengCtrl Instance;
	public static DanKengCtrl GetInstance()
	{
		return Instance;
	}

	// Use this for initialization
	void Awake()
	{
		Instance = this;
		gameObject.SetActive(false);
		int max = DanKengArray.Length;
		for (int i = 0; i < max; i++) {
			DanKengArray[i].SetActive(false);
		}
	}

	public void ShowDanKengObj()
	{
		if (IsShowDanKeng) {
			return;
		}
		IsShowDanKeng = true;
		gameObject.SetActive(true);
		ShowNextDanKeng();
	}

	public void ShowNextDanKeng()
	{
		if (DanKengIndex >= DanKengArray.Length) {
			//Start loading level_1
			//XkGameCtrl.LoadingGameScene_1();
			return;
		}

		if (DanKengIndex == 0) {
			DelayShowNextDanKeng();
		}
		else {
			Invoke("DelayShowNextDanKeng", 0.2f);
		}
	}

	void DelayShowNextDanKeng()
	{
		DanKengArray[DanKengIndex].SetActive(true);
		DanKengIndex++;
	}
}