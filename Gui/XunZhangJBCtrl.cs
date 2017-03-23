using UnityEngine;
using System.Collections;

public class XunZhangJBCtrl : MonoBehaviour {
	public PlayerEnum PlayerSt = PlayerEnum.Null;
	public UITexture[] XunZhangUITexture;
	public Texture[] XunZhangJBTexture;
	int XunZhangNum = 0;
	GameObject[] XunZhangObjArray = new GameObject[4];
	int[] ShiBingXunZhangJB = {10, 20};
	int[] TanKeXunZhangJB = {10, 20};
	int[] ChuanBoXunZhangJB = {10, 20};
	int[] FeiJiXunZhangJB = {10, 20};
	Vector3[] XunZhangLocalPos = {
		new Vector3(0f, 0f, 0f),
		new Vector3(108f, 0f, 0f),
		new Vector3(210f, 0f, 0f),
		new Vector3(315f, 0f, 0f),
	};
	int XunZhangIndex;
	static int XunZhangPlayerCount;
	static XunZhangJBCtrl InstanceOne;
	public static XunZhangJBCtrl GetInstanceOne()
	{
		return InstanceOne;
	}

	static XunZhangJBCtrl InstanceTwo;
	public static XunZhangJBCtrl GetInstanceTwo()
	{
		return InstanceTwo;
	}

	// Use this for initialization
	void Awake()
	{
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			InstanceOne = this;
			break;

		case PlayerEnum.PlayerTwo:
			InstanceTwo = this;
			break;
		}

		XunZhangPlayerCount = 0;
		int max = XunZhangUITexture.Length;
		for (int i = 0; i < max; i++) {
			XunZhangUITexture[i].gameObject.SetActive(false);
		}
		gameObject.SetActive(false);
	}

	public void ShowPlayerXunZhangJB()
	{
//		XkGameCtrl.ShiBingNumPOne = 5; //test
//		XkGameCtrl.CheLiangNumPOne = 5; //test
//		XkGameCtrl.FeiJiNumPOne = 5; //test
//		XkGameCtrl.ChuanBoNumPOne = 5; //test		
//		XkGameCtrl.ShiBingNumPTwo = 5; //test
//		XkGameCtrl.CheLiangNumPTwo = 0; //test
//		XkGameCtrl.FeiJiNumPTwo = 0; //test
//		XkGameCtrl.ChuanBoNumPTwo = 5; //test

		ShiBingXunZhangJB = XkGameCtrl.GetInstance().ShiBingXunZhangJB;
		TanKeXunZhangJB = XkGameCtrl.GetInstance().TanKeXunZhangJB;
		ChuanBoXunZhangJB = XkGameCtrl.GetInstance().ChuanBoXunZhangJB;
		FeiJiXunZhangJB = XkGameCtrl.GetInstance().FeiJiXunZhangJB;

		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			if (XkGameCtrl.ShiBingNumPOne > 0) {
				if (XkGameCtrl.ShiBingNumPOne > ShiBingXunZhangJB[1]) {
						XunZhangUITexture[0].mainTexture = XunZhangJBTexture[8];
				}
				else if (XkGameCtrl.ShiBingNumPOne > ShiBingXunZhangJB[0]) {
					XunZhangUITexture[0].mainTexture = XunZhangJBTexture[4];
				}
				else {
					XunZhangUITexture[0].mainTexture = XunZhangJBTexture[0];
				}
				XunZhangObjArray[XunZhangNum] = XunZhangUITexture[0].gameObject;
				XunZhangNum++;
			}

			if (XkGameCtrl.CheLiangNumPOne > 0) {
				if (XkGameCtrl.CheLiangNumPOne > TanKeXunZhangJB[1]) {
					XunZhangUITexture[1].mainTexture = XunZhangJBTexture[9];
				}
				else if (XkGameCtrl.CheLiangNumPOne > TanKeXunZhangJB[0]) {
					XunZhangUITexture[1].mainTexture = XunZhangJBTexture[5];
				}
				else {
					XunZhangUITexture[1].mainTexture = XunZhangJBTexture[1];
				}
				XunZhangObjArray[XunZhangNum] = XunZhangUITexture[1].gameObject;
				XunZhangNum++;
			}

			if (XkGameCtrl.FeiJiNumPOne > 0) {
				if (XkGameCtrl.FeiJiNumPOne > FeiJiXunZhangJB[1]) {
					XunZhangUITexture[2].mainTexture = XunZhangJBTexture[10];
				}
				else if (XkGameCtrl.FeiJiNumPOne > FeiJiXunZhangJB[0]) {
					XunZhangUITexture[2].mainTexture = XunZhangJBTexture[6];
				}
				else {
					XunZhangUITexture[2].mainTexture = XunZhangJBTexture[2];
				}
				XunZhangObjArray[XunZhangNum] = XunZhangUITexture[2].gameObject;
				XunZhangNum++;
			}

			if (XkGameCtrl.ChuanBoNumPOne > 0) {
				if (XkGameCtrl.ChuanBoNumPOne > ChuanBoXunZhangJB[1]) {
					XunZhangUITexture[3].mainTexture = XunZhangJBTexture[11];
				}
				else if (XkGameCtrl.ChuanBoNumPOne > ChuanBoXunZhangJB[0]) {
					XunZhangUITexture[3].mainTexture = XunZhangJBTexture[7];
				}
				else {
					XunZhangUITexture[3].mainTexture = XunZhangJBTexture[3];
				}
				XunZhangObjArray[XunZhangNum] = XunZhangUITexture[3].gameObject;
				XunZhangNum++;
			}
			break;
			
		case PlayerEnum.PlayerTwo:
			if (XkGameCtrl.ShiBingNumPTwo > 0) {
				if (XkGameCtrl.ShiBingNumPTwo > ShiBingXunZhangJB[1]) {
					XunZhangUITexture[0].mainTexture = XunZhangJBTexture[8];
				}
				else if (XkGameCtrl.ShiBingNumPTwo > ShiBingXunZhangJB[0]) {
					XunZhangUITexture[0].mainTexture = XunZhangJBTexture[4];
				}
				else {
					XunZhangUITexture[0].mainTexture = XunZhangJBTexture[0];
				}
				XunZhangObjArray[XunZhangNum] = XunZhangUITexture[0].gameObject;
				XunZhangNum++;
			}
			
			if (XkGameCtrl.CheLiangNumPTwo > 0) {
				if (XkGameCtrl.CheLiangNumPTwo > TanKeXunZhangJB[1]) {
					XunZhangUITexture[1].mainTexture = XunZhangJBTexture[9];
				}
				else if (XkGameCtrl.CheLiangNumPTwo > TanKeXunZhangJB[0]) {
					XunZhangUITexture[1].mainTexture = XunZhangJBTexture[5];
				}
				else {
					XunZhangUITexture[1].mainTexture = XunZhangJBTexture[1];
				}
				XunZhangObjArray[XunZhangNum] = XunZhangUITexture[1].gameObject;
				XunZhangNum++;
			}
			
			if (XkGameCtrl.FeiJiNumPTwo > 0) {
				if (XkGameCtrl.FeiJiNumPTwo > FeiJiXunZhangJB[1]) {
					XunZhangUITexture[2].mainTexture = XunZhangJBTexture[10];
				}
				else if (XkGameCtrl.FeiJiNumPTwo > FeiJiXunZhangJB[0]) {
					XunZhangUITexture[2].mainTexture = XunZhangJBTexture[6];
				}
				else {
					XunZhangUITexture[2].mainTexture = XunZhangJBTexture[2];
				}
				XunZhangObjArray[XunZhangNum] = XunZhangUITexture[2].gameObject;
				XunZhangNum++;
			}
			
			if (XkGameCtrl.ChuanBoNumPTwo > 0) {
				if (XkGameCtrl.ChuanBoNumPTwo > ChuanBoXunZhangJB[1]) {
					XunZhangUITexture[3].mainTexture = XunZhangJBTexture[11];
				}
				else if (XkGameCtrl.ChuanBoNumPTwo > ChuanBoXunZhangJB[0]) {
					XunZhangUITexture[3].mainTexture = XunZhangJBTexture[7];
				}
				else {
					XunZhangUITexture[3].mainTexture = XunZhangJBTexture[3];
				}
				XunZhangObjArray[XunZhangNum] = XunZhangUITexture[3].gameObject;
				XunZhangNum++;
			}
			break;
		}

		//Debug.Log("XunZhangNum "+XunZhangNum);
		if (XunZhangNum > 0) {
			gameObject.SetActive(true);
		}
		ShowNextPlayerXunZhang();
	}

	public void ShowNextPlayerXunZhang()
	{
		XKGlobalData.GetInstance().PlayAudioXunZhangJB();
		if (XunZhangIndex >= XunZhangNum) {
			//Start show player killNpcNum info
			if (XkGameCtrl.IsPlayGamePOne && XkGameCtrl.IsPlayGamePTwo) {
				XunZhangPlayerCount++;
				if (XunZhangPlayerCount >= 2) {
					PlayerKillNumCtrl.GetInstanceOne().ShowPlayerKillNum();
					PlayerKillNumCtrl.GetInstanceTwo().ShowPlayerKillNum();
				}
			}
			else {
				switch (PlayerSt) {
				case PlayerEnum.PlayerOne:
					PlayerKillNumCtrl.GetInstanceOne().ShowPlayerKillNum();
					break;
					
				case PlayerEnum.PlayerTwo:
					PlayerKillNumCtrl.GetInstanceTwo().ShowPlayerKillNum();
					break;
				}
			}
			return;
		}

		TweenPosition tweenPos = XunZhangObjArray[XunZhangIndex].GetComponent<TweenPosition>();
		tweenPos.to = XunZhangLocalPos[XunZhangIndex];
		XunZhangObjArray[XunZhangIndex].transform.localPosition = tweenPos.from;
		XunZhangObjArray[XunZhangIndex].SetActive(true);
		XunZhangIndex++;
	}
}