using UnityEngine;
using System.Collections;

public class YouLiangAddCtrl : MonoBehaviour {
	public GameObject YouLiangGuang;
	public UISprite YouLiangSprite;
	/**
	 * YouLiangSpriteArray[0] -> 玩家1油量.
	 * YouLiangSpriteArray[0] -> 玩家2油量.
	 */
	public UISprite[] YouLiangSpriteArray;
	/**
	 * 油量增加灰色底图.
	 */
	public GameObject YouLiangDiTuObj;
	[Range(1f, 100f)] public float YouLiangDianAddYL = 40f;
	static YouLiangAddCtrl _Instance;
	public static YouLiangAddCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		YouLiangGuang.SetActive(false);
		
		bool isSelectYouLiang2 = false;
		if (XkGameCtrl.SelectYouLiangUI == 2) {
			isSelectYouLiang2 = true;
		}
		YouLiangSprite.gameObject.SetActive(!isSelectYouLiang2);
		YouLiangDiTuObj.SetActive(isSelectYouLiang2);
		for (int i = 0; i < YouLiangSpriteArray.Length; i++) {
			YouLiangSpriteArray[i].gameObject.SetActive(isSelectYouLiang2);
		}

		SetYouLiangSpriteAmount(0f);
	}

	public void SetYouLiangSpriteAmount(float val, PlayerEnum indexPlayer = PlayerEnum.Null)
	{
		switch (XkGameCtrl.SelectYouLiangUI) {
		case 1:
			YouLiangSprite.fillAmount = val;
			break;
		case 2:
			if (indexPlayer == PlayerEnum.Null) {
				for (int i = 0; i < YouLiangSpriteArray.Length; i++) {
					YouLiangSpriteArray[i].fillAmount = val;
				}
			}
			else {
				int indexVal = (int)indexPlayer - 1;
				//Debug.Log("indexVal "+indexVal+", indexPlayer "+indexPlayer);
				YouLiangSpriteArray[indexVal].fillAmount = val;
			}
			break;
		}
	}

	public void AddPlayerYouLiangDian(PlayerEnum indexPlayer)
	{
		XkGameCtrl.GetInstance().AddPlayerYouLiang(YouLiangDianAddYL, indexPlayer); //Add Player YouLiang
	}

	public void HiddenYouLiangAdd()
	{
		gameObject.SetActive(false);
	}

	public void ShowYouLiangGuangObj()
	{
		if (YouLiangGuang.activeSelf) {
			YouLiangGuang.SetActive(false);
		}
		YouLiangGuang.SetActive(true);
		if (IsInvoking("HiddenYouLiangGuangObj")) {
			CancelInvoke("HiddenYouLiangGuangObj");
		}
		Invoke("HiddenYouLiangGuangObj", 0.1f);
	}

	void HiddenYouLiangGuangObj()
	{
		YouLiangGuang.SetActive(false);
	}
}