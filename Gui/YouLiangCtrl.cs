using UnityEngine;
using System.Collections;

public class YouLiangCtrl : MonoBehaviour {
	public GameObject YouLiangFlashObj;
	public UISprite YouLiangJDSprite;
	/**
	 * YouLiangJDSpriteArray[0] -> 玩家1油量.
	 * YouLiangJDSpriteArray[1] -> 玩家2油量.
	 */
	public UISprite[] YouLiangJDSpriteArray;
	/**
	 * YouLiangBanYuanP1[0] -> 彩色半圆.
	 * YouLiangBanYuanP1[1] -> 灰色半圆.
	 */
	public GameObject[] YouLiangBanYuanP1;
	/**
	 * YouLiangBanYuanP1[0] -> 彩色半圆.
	 * YouLiangBanYuanP1[1] -> 灰色半圆.
	 */
	public GameObject[] YouLiangBanYuanP2;
	/**
	 * 双油量遮挡.
	 */
	public GameObject YouLiangZheDangObj;
	public GameObject YouLiangBuZuObj;
	/**
	 * YouLiangBuZuObjArray[0] -> 玩家1.
	 * YouLiangBuZuObjArray[1] -> 玩家2.
	 */
	public GameObject[] YouLiangBuZuObjArray;
	public static bool IsActiveYouLiangFlash;
	public static bool IsChangeYouLiangFillAmout;
	float StartYouLiang;
	/**
	 * StartYouLiangArray[0] -> 玩家1.
	 * StartYouLiangArray[1] -> 玩家2.
	 */
	float[] StartYouLiangArray = {0f, 0f, 0f, 0f};
	float EndYouLiang;
	/**
	 * EndYouLiangArray[0] -> 玩家1.
	 * EndYouLiangArray[1] -> 玩家2.
	 */
	float[] EndYouLiangArray = {0f, 0f, 0f, 0f};
	static YouLiangCtrl Instance;
	public static YouLiangCtrl GetInstance()
	{
		return Instance;
	}

	// Use this for initialization
	void Start()
	{
		Instance = this;
		PlayerYouLiangCount = 0;
		bool isSelectYouLiang2 = false;
		if (XkGameCtrl.SelectYouLiangUI == 2) {
			isSelectYouLiang2 = true;
		}

		YouLiangBuZuObj.SetActive(!isSelectYouLiang2);
		YouLiangJDSprite.gameObject.SetActive(!isSelectYouLiang2);
		YouLiangZheDangObj.SetActive(isSelectYouLiang2);
		for (int i = 0; i < YouLiangJDSpriteArray.Length; i++) {
			YouLiangJDSpriteArray[i].gameObject.SetActive(isSelectYouLiang2);
			YouLiangBuZuObjArray[i].SetActive(false);
			if (i == 0){
				YouLiangBanYuanP1[i].SetActive(false);
				YouLiangBanYuanP2[i].SetActive(false);
			}
			else {
				YouLiangBanYuanP1[i].SetActive(isSelectYouLiang2);
				YouLiangBanYuanP2[i].SetActive(isSelectYouLiang2);
			}
		}

		SetActiveYouLiangFlash(false);
	}
	
	void Update()
	{
		CheckPlayerYouLiangBuZu();
		if (IsChangeYouLiangFillAmout) {
			ChangePlayerYouLiangFillAmout();
			return;
		}

		switch (XkGameCtrl.SelectYouLiangUI) {
		case 1:
			SetPlayerYouLiangAmout(XkGameCtrl.PlayerYouLiangCur);
			break;
		case 2:
			SetPlayerYouLiangAmout(XkGameCtrl.PlayerYouLiangCurP1, PlayerEnum.PlayerOne);
			SetPlayerYouLiangAmout(XkGameCtrl.PlayerYouLiangCurP2, PlayerEnum.PlayerTwo);
			break;
		}
	}

	public void InitChangePlayerYouLiangFillAmout(float startVal, float endVal, PlayerEnum indexPlayer = PlayerEnum.Null)
	{
		if (IsChangeYouLiangFillAmout) {
			return;
		}
		IsChangeYouLiangFillAmout = true;

		switch (XkGameCtrl.SelectYouLiangUI) {
		case 1:
			StartYouLiang = startVal;
			EndYouLiang = endVal;
			break;
		case 2:
			PlayerYouLiangCount++;
			int indexVal = (int)indexPlayer - 1;
			StartYouLiangArray[indexVal] = startVal;
			EndYouLiangArray[indexVal] = endVal;
			break;
		}
	}

	void ResetChangePlayerYouLiangFillAmout()
	{
		if (!IsChangeYouLiangFillAmout) {
			return;
		}
		IsChangeYouLiangFillAmout = false;
		Debug.Log("ResetChangePlayerYouLiangFillAmout...");

		if (XkGameCtrl.SelectYouLiangUI == 2) {
			for (int i = 0; i < YouLiangJDSpriteArray.Length; i++) {
				StartYouLiangArray[i] = 0f;
				EndYouLiangArray[i] = 0f;
			}
		}
	}

	int PlayerYouLiangCount = 0;
	void ChangePlayerYouLiangFillAmout()
	{
		if (!IsChangeYouLiangFillAmout) {
			return;
		}

		switch (XkGameCtrl.SelectYouLiangUI) {
		case 1:
			StartYouLiang += 2f;
			if (EndYouLiang < StartYouLiang) {
				StartYouLiang = EndYouLiang;
			}
			
			SetPlayerYouLiangAmout(StartYouLiang);
			if (EndYouLiang <= StartYouLiang) {
				ResetChangePlayerYouLiangFillAmout();
			}
			break;
		case 2:
			bool isReset = true;
			PlayerEnum indexPlayer = PlayerEnum.Null;
			for (int i = 0; i < YouLiangJDSpriteArray.Length; i++) {
				if (StartYouLiangArray[i] == EndYouLiangArray[i] && EndYouLiangArray[i] == 0) {
					continue;
				}

				StartYouLiangArray[i] += 2f;
				if (EndYouLiangArray[i] < StartYouLiangArray[i]) {
					StartYouLiangArray[i] = EndYouLiangArray[i];
				}

				indexPlayer = (PlayerEnum)(i+1);
				SetPlayerYouLiangAmout(StartYouLiangArray[i], indexPlayer);
				if (EndYouLiangArray[i] <= StartYouLiangArray[i]) {
					PlayerYouLiangCount--;
					if (PlayerYouLiangCount <= 0) {
						PlayerYouLiangCount = 0;
						isReset = false;
					}
				}
			}

			if (isReset) {
				ResetChangePlayerYouLiangFillAmout();
			}
			break;
		}
	}

	float MaxYouLiangAmount = 1f;
	float KeyPlayerYouLiang = 1f;
	float PlayerYouLiangMaxVal = -1f;
	void SetPlayerYouLiangAmout(float youLiangCur, PlayerEnum indexPlayer = PlayerEnum.Null)
	{
		if (PlayerYouLiangMaxVal != XkGameCtrl.PlayerYouLiangMax) {
			PlayerYouLiangMaxVal = XkGameCtrl.PlayerYouLiangMax;
			MaxYouLiangAmount = XkGameCtrl.SelectYouLiangUI == 1 ? 1f : 0.78f;
			KeyPlayerYouLiang = MaxYouLiangAmount / XkGameCtrl.PlayerYouLiangMax;
		}

		float valYL = KeyPlayerYouLiang * youLiangCur;
		if (valYL < 0f) {
			valYL = 0f;
		}

		switch (XkGameCtrl.SelectYouLiangUI) {
		case 1:
			if (YouLiangJDSprite.fillAmount != valYL) {
				YouLiangJDSprite.fillAmount = valYL;
			}
			break;
		case 2:
			if (indexPlayer == PlayerEnum.Null) {
				return;
			}

			int indexVal = (int)indexPlayer - 1;
			if (YouLiangJDSpriteArray[indexVal].fillAmount == valYL) {
				return;
			}

			if (valYL > 0f) {
				if (!YouLiangBanYuanP1[0].activeSelf && indexPlayer == PlayerEnum.PlayerOne) {
					YouLiangBanYuanP1[0].SetActive(true);
				}
				if (!YouLiangBanYuanP2[0].activeSelf && indexPlayer == PlayerEnum.PlayerTwo) {
					YouLiangBanYuanP2[0].SetActive(true);
				}
			}

			if (valYL <= 0f) {
				if (YouLiangBanYuanP1[0].activeSelf && indexPlayer == PlayerEnum.PlayerOne) {
					YouLiangBanYuanP1[0].SetActive(false);
				}
				if (YouLiangBanYuanP2[0].activeSelf && indexPlayer == PlayerEnum.PlayerTwo) {
					YouLiangBanYuanP2[0].SetActive(false);
				}
			}
			YouLiangJDSpriteArray[indexVal].fillAmount = valYL;
			break;
		}
	}

	float TimeYouLiangBZ;
	void CheckPlayerYouLiangBuZu()
	{
		if (Time.realtimeSinceStartup - TimeYouLiangBZ < 1f) {
			return;
		}
		TimeYouLiangBZ = Time.realtimeSinceStartup;

		if (XkGameCtrl.SelectYouLiangUI == 1) {
			return;
		}

		bool isActiveBZP1 = XkGameCtrl.PlayerYouLiangCurP1 > XkGameCtrl.YouLiangJingGaoVal ? false : true;
		if (!XkGameCtrl.IsActivePlayerOne) {
			isActiveBZP1 = false;
		}
		if (isActiveBZP1 != YouLiangBuZuObjArray[0].activeSelf) {
			YouLiangBuZuObjArray[0].SetActive(isActiveBZP1);
		}

		bool isActiveBZP2 = XkGameCtrl.PlayerYouLiangCurP2 > XkGameCtrl.YouLiangJingGaoVal ? false : true;
		if (!XkGameCtrl.IsActivePlayerTwo) {
			isActiveBZP2 = false;
		}
		if (isActiveBZP2 != YouLiangBuZuObjArray[1].activeSelf) {
			YouLiangBuZuObjArray[1].SetActive(isActiveBZP2);
		}
		//Debug.Log("isActiveBZP1 "+isActiveBZP1+", isActiveBZP2 "+isActiveBZP2);
	}

	public void SetActiveYouLiangFlash(bool isActive)
	{
		IsActiveYouLiangFlash = isActive;
		YouLiangFlashObj.SetActive(isActive);
		if (!isActive) {
			XKGlobalData.GetInstance().StopAudioRanLiaoJingGao();
		}
	}

	public void HiddenYouLiang()
	{
		gameObject.SetActive(false);
		XKGlobalData.GetInstance().StopAudioRanLiaoJingGao();
	}
}