using UnityEngine;
using System.Collections;

public class RenWuXinXiCtrl : MonoBehaviour {
	public UISprite[] RwSpriteInfo;
	public UISprite[] RedSprite;
	GameObject RenWuXinXiObj;
	int LoadedLevelVal;
	int CountRw;
	float TimeStart;
	int[] CountRenWuInfo = {2, 2, 2, 3, 2, 2};
	int[] CountRenWuRedInfo = {2, 2, 2, 2, 2, 2};
	static RenWuXinXiCtrl Instance;
	public static RenWuXinXiCtrl GetInstance()
	{
		return Instance;
	}

	// Use this for initialization
	void Start()
	{
		Instance = this;
		RenWuXinXiObj = gameObject;
		RenWuXinXiObj.SetActive(false);
	}

	void Update()
	{
		CheckRenWuSpriteInfo();
	}

	public void InitRenWuSprite()
	{
		if (RenWuXinXiObj.activeSelf) {
			return;
		}
		RenWuXinXiObj.SetActive(true);
		LoadedLevelVal = Application.loadedLevel - 1;
		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			LoadedLevelVal = 0; //test
		}
		int max = RwSpriteInfo.Length;
		int levelNum = LoadedLevelVal + 1;
		string spriteNameTmp = "";

		int indexTmp = 0;
		int indexVal = 0;
		string EnInfoStr = "";
		GameTextType gameTextVal = XKGlobalData.GetGameTextMode();
		if (gameTextVal == GameTextType.English) {
			EnInfoStr = "_En";
		}

		for (int i = 0; i < max; i++) {
			if (LoadedLevelVal == 0) {
				RwSpriteInfo[i].enabled = false;
			}
			RwSpriteInfo[i].fillAmount = 0f;
			indexTmp = i % 3;
			if (indexTmp < CountRenWuInfo[LoadedLevelVal]) {
				if (i > 2) {
					indexVal = 2;
				}
				spriteNameTmp = levelNum + "_" + indexVal + "_" + indexTmp + EnInfoStr;
				//Debug.Log("name "+spriteNameTmp);
				RwSpriteInfo[i].spriteName = spriteNameTmp;
			}
		}

		max = RedSprite.Length;
		for (int i = 0; i < max; i++) {
			if (LoadedLevelVal == 0) {
				RedSprite[i].enabled = false;
			}
			RedSprite[i].gameObject.SetActive(false);
			
			indexTmp = i % 3;
			if (indexTmp < CountRenWuRedInfo[LoadedLevelVal]) {
				spriteNameTmp = levelNum + "_1_" + indexTmp + EnInfoStr;
				//Debug.Log("name ** "+spriteNameTmp);
				RedSprite[i].spriteName = spriteNameTmp;
			}
		}
		
		if (LoadedLevelVal == 0) {
			YouLiangTiShiCtrl.GetInstance().ShowYouLiangTiShi();
		}
	}

	void CheckRenWuSpriteInfo()
	{
		if (CountRw >= 6) {
			return; //stop check RenWuInfo
		}

		if (Time.realtimeSinceStartup - TimeStart <= 2f && CountRw == 3) {
			return;
		}

		if (CountRw < 3) {
			RwSpriteInfo[CountRw].fillAmount += (0.2f * Time.deltaTime);
		}
		else {
			RwSpriteInfo[CountRw].fillAmount += Time.deltaTime;
		}

		if (RwSpriteInfo[CountRw].fillAmount >= 1f) {
			RwSpriteInfo[CountRw].fillAmount = 1f;
			int countTmp = (CountRw % 3) + 1;
			if (countTmp >= CountRenWuInfo[LoadedLevelVal]) {
				CountRw += (3 - CountRenWuInfo[LoadedLevelVal]);
			}
			CountRw++;
			//Debug.Log("test **** countRw "+CountRw+", countTmp "+countTmp+", loadedLevel "+LoadedLevelVal);

			if (CountRw == 3) {
				TimeStart = Time.realtimeSinceStartup;
			}

			if (CountRw >= 6) {
				Invoke("ShowRenWuRedInfo", 1f);
			}
		}
	}

	void ShowRenWuRedInfo()
	{
		int max = CountRenWuRedInfo[LoadedLevelVal];
		for (int i = 0; i < max; i++) {
			RedSprite[i].gameObject.SetActive(true);
		}
	}

	public void HiddenRenWuXinXi()
	{
		if (LoadedLevelVal == 0) {
			YouLiangDianTiShiCtrl.GetInstance().HiddenGameObj();
		}
		RenWuXinXiObj.SetActive(false);
		ScreenDanHeiCtrl.GetInstance().InitPlayScreenDanHei();
	}
}