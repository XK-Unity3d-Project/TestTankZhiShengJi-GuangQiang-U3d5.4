using UnityEngine;
using System.Collections;

public class PlayerKillNumCtrl : MonoBehaviour {
	public PlayerEnum PlayerSt = PlayerEnum.Null;
	public UISprite[] KillNpcNum; //人型npc.
	public UISprite[] KillTKNum; //坦克.
	public UISprite[] KillFJNum; //飞机.
	public UISprite[] KillCBNum; //船舶.
	GameObject KillNpcObj;
	GameObject KillTKObj;
	GameObject KillFJObj;
	GameObject KillCBObj;
	bool IsShowKillNum;
	float TimeDelayVal = 1f;
	static bool IsEndPlayerKillNumCartoon;
	static PlayerKillNumCtrl InstanceOne;
	public static PlayerKillNumCtrl GetInstanceOne()
	{
		return InstanceOne;
	}

	static PlayerKillNumCtrl InstanceTwo;
	public static PlayerKillNumCtrl GetInstanceTwo()
	{
		return InstanceTwo;
	}

	// Use this for initialization
	void Start()
	{
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			InstanceOne = this;
			break;

		case PlayerEnum.PlayerTwo:
			InstanceTwo = this;
			break;
		}
		IsEndPlayerKillNumCartoon = false;
		KillNpcObj = KillNpcNum[0].transform.parent.gameObject;
		KillTKObj = KillTKNum[0].transform.parent.gameObject;
		KillFJObj = KillFJNum[0].transform.parent.gameObject;
		KillCBObj = KillCBNum[0].transform.parent.gameObject;
		KillNpcObj.SetActive(false);
		KillTKObj.SetActive(false);
		KillFJObj.SetActive(false);
		KillCBObj.SetActive(false);
	}

	public void ShowPlayerKillNum()
	{
		if (IsShowKillNum) {
			return;
		}
		IsShowKillNum = true;
		ShowPlayerKillNpcNum();
	}

	void ShowPlayerKillNpcNum()
	{
		if (KillNpcObj.activeSelf) {
			return;
		}
		KillNpcObj.SetActive(true);
		XKGlobalData.GetInstance().PlayAudioJiFenGunDong();
		Invoke("StopKillNpcNumCartoon", TimeDelayVal);
	}
	
	/*ShiBingNumPOne = 0;
	CheLiangNumPOne = 0;
	ChuanBoNumPOne = 0;
	FeiJiNumPOne = 0;
	ShiBingNumPTwo = 0;
	CheLiangNumPTwo = 0;
	ChuanBoNumPTwo = 0;
	FeiJiNumPTwo = 0;*/
	void StopKillNpcNumCartoon()
	{
		int max = KillNpcNum.Length;
		UISpriteAnimation AniCom = null;

		int numVal = 0;
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			numVal = XkGameCtrl.ShiBingNumPOne;
			break;

		case PlayerEnum.PlayerTwo:
			numVal = XkGameCtrl.ShiBingNumPTwo;
			break;
		}

		//bool isShowZero = false;
		int valTmp = 0;
		int powVal = 0;
		for (int i = 0; i < max; i++) {
			AniCom = KillNpcNum[i].GetComponent<UISpriteAnimation>();
			AniCom.enabled = false;

			powVal = (int)Mathf.Pow(10, max - i - 1);
			valTmp = numVal / powVal;
			//Debug.Log("valTmp *** "+valTmp);
			/*if (!isShowZero) {
			    if (valTmp > 0) {
					isShowZero = true;
				}
				else {
					if (i < (max - 1)) {
						KillNpcNum[i].enabled = false;
						continue;
					}
				}
			}*/
			KillNpcNum[i].spriteName = "KillNum_" + valTmp;
			numVal -= valTmp * powVal;
		}
		ShowPlayerKillTKNum();
	}

	void ShowPlayerKillTKNum()
	{
		if (KillTKObj.activeSelf) {
			return;
		}
		KillTKObj.SetActive(true);
		Invoke("StopKillTKNumCartoon", TimeDelayVal);
	}

	void StopKillTKNumCartoon()
	{
		int max = KillTKNum.Length;
		UISpriteAnimation AniCom = null;
		
		int numVal = 0;
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			numVal = XkGameCtrl.CheLiangNumPOne;
			break;
			
		case PlayerEnum.PlayerTwo:
			numVal = XkGameCtrl.CheLiangNumPTwo;
			break;
		}
		
		//bool isShowZero = false;
		int valTmp = 0;
		int powVal = 0;
		for (int i = 0; i < max; i++) {
			AniCom = KillTKNum[i].GetComponent<UISpriteAnimation>();
			AniCom.enabled = false;
			
			powVal = (int)Mathf.Pow(10, max - i - 1);
			valTmp = numVal / powVal;
			//Debug.Log("valTmp *** "+valTmp);
			/*if (!isShowZero) {
				if (valTmp > 0) {
					isShowZero = true;
				}
				else {
					if (i < (max - 1)) {
						KillTKNum[i].enabled = false;
						continue;
					}
				}
			}*/
			KillTKNum[i].spriteName = "KillNum_" + valTmp;
			numVal -= valTmp * powVal;
		}
		ShowPlayerKillFJNum();
	}

	void ShowPlayerKillFJNum()
	{
		if (KillFJObj.activeSelf) {
			return;
		}
		KillFJObj.SetActive(true);
		Invoke("StopKillFJNumCartoon", TimeDelayVal);
	}
	
	void StopKillFJNumCartoon()
	{
		int max = KillFJNum.Length;
		UISpriteAnimation AniCom = null;
		
		int numVal = 0;
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			numVal = XkGameCtrl.FeiJiNumPOne;
			break;
			
		case PlayerEnum.PlayerTwo:
			numVal = XkGameCtrl.FeiJiNumPTwo;
			break;
		}
		
		//bool isShowZero = false;
		int valTmp = 0;
		int powVal = 0;
		for (int i = 0; i < max; i++) {
			AniCom = KillFJNum[i].GetComponent<UISpriteAnimation>();
			AniCom.enabled = false;
			
			powVal = (int)Mathf.Pow(10, max - i - 1);
			valTmp = numVal / powVal;
			//Debug.Log("valTmp *** "+valTmp);
			/*if (!isShowZero) {
				if (valTmp > 0) {
					isShowZero = true;
				}
				else {
					if (i < (max - 1)) {
						KillFJNum[i].enabled = false;
						continue;
					}
				}
			}*/
			KillFJNum[i].spriteName = "KillNum_" + valTmp;
			numVal -= valTmp * powVal;
		}
		ShowPlayerKillCBNum();
	}

	void ShowPlayerKillCBNum()
	{
		if (KillCBObj.activeSelf) {
			return;
		}
		KillCBObj.SetActive(true);
		Invoke("StopKillCBNumCartoon", TimeDelayVal);
	}
	
	void StopKillCBNumCartoon()
	{
		int max = KillCBNum.Length;
		UISpriteAnimation AniCom = null;
		
		int numVal = 0;
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			numVal = XkGameCtrl.ChuanBoNumPOne;
			break;
			
		case PlayerEnum.PlayerTwo:
			numVal = XkGameCtrl.ChuanBoNumPTwo;
			break;
		}
		
		//bool isShowZero = false;
		int valTmp = 0;
		int powVal = 0;
		for (int i = 0; i < max; i++) {
			AniCom = KillCBNum[i].GetComponent<UISpriteAnimation>();
			AniCom.enabled = false;
			
			powVal = (int)Mathf.Pow(10, max - i - 1);
			valTmp = numVal / powVal;
			//Debug.Log("valTmp *** "+valTmp);
			/*if (!isShowZero) {
				if (valTmp > 0) {
					isShowZero = true;
				}
				else {
					if (i < (max - 1)) {
						KillCBNum[i].enabled = false;
						continue;
					}
				}
			}*/
			KillCBNum[i].spriteName = "KillNum_" + valTmp;
			numVal -= valTmp * powVal;
		}
		EndPlayerKillNumCartoon();
	}

	void EndPlayerKillNumCartoon()
	{
		if (IsEndPlayerKillNumCartoon) {
			return;
		}
		IsEndPlayerKillNumCartoon = true;
		Debug.Log("EndPlayerKillNumCartoon...");
		
		XKGlobalData.GetInstance().StopAudioJiFenGunDong();
		if (XkGameCtrl.IsPlayGamePOne) {
			XunZhangZPCtrl.GetInstanceOne().ShowPlayerXunZhang();
		}

		if (XkGameCtrl.IsPlayGamePTwo) {
			XunZhangZPCtrl.GetInstanceTwo().ShowPlayerXunZhang();
		}
	}
}