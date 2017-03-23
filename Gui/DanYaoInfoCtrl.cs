using UnityEngine;
using System.Collections;

public class DanYaoInfoCtrl : MonoBehaviour {
	public PlayerEnum PlayerSt = PlayerEnum.Null;
	public UITexture GaoBaoTextureCom; //gaoBao or jiQiang
	public UITexture DaoDanTextureCom;
	public UISprite HuoLiJQSprite;
	public UISprite DaoDanSprite;
	/**
	 * DanYaoTK[0] -> jiQiang, DanYaoTK[1] -> gaoBao, DanYaoTK[2] -> daoDan
	 */
	public Texture[] DanYaoTK;
	/**
	 * DanYaoFJ[0] -> jiQiang, DanYaoFJ[1] -> gaoBao, DanYaoFJ[2] -> daoDan
	 */
	public Texture[] DanYaoFJ;
	public UISprite[] AmmoDaoDan;
	/**
	 * AmmoGaoBao[0-1] -> gaoBaoAmmoNum, AmmoGaoBao[2] -> jiQiangAmmoNum
	 */
	public UISprite[] AmmoGaoBao;
	int GaoBaoAmmoNum = -1;
	int DaoDanAmmoNum = -1;
	static DanYaoInfoCtrl InstanceOne;
	public static DanYaoInfoCtrl GetInstanceOne()
	{
		return InstanceOne;
	}

	static DanYaoInfoCtrl InstanceTwo;
	public static DanYaoInfoCtrl GetInstanceTwo()
	{
		return InstanceTwo;
	}

	// Use this for initialization
	void Start()
	{
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			InstanceOne = this;
			if (!XkGameCtrl.IsActivePlayerOne) {
				HiddenPlayerDanYaoInfo();
			}
			break;

		case PlayerEnum.PlayerTwo:
			InstanceTwo = this;
			if (!XkGameCtrl.IsActivePlayerTwo) {
				HiddenPlayerDanYaoInfo();
			}
			break;
		}

//		XkGameCtrl.GaoBaoDanNumPOne = 5; //test
//		XkGameCtrl.GaoBaoDanNumPTwo = 5; //test
		InitPlayerDanYaoInfo();
	}

	void Update()
	{
		if (!pcvr.bIsHardWare && Input.GetKeyUp(KeyCode.L)) {
			//test
			ShowHuoLiJQSprite();
			ShowDaoDanSprite();
		}
		CheckPlayerGaoBaoAmmoNum();
	}

	public void ShowPlayerDanYaoInfo()
	{
		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}

		if (gameObject.activeSelf) {
			return;
		}
		gameObject.SetActive(true);
	}
	
	public void HiddenPlayerDanYaoInfo()
	{
		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}

		if (!gameObject.activeSelf) {
			return;
		}
		gameObject.SetActive(false);
		HiddenDaoDanSprite();
		HiddenHuoLiJQSprite();
	}

	void InitPlayerDanYaoInfo()
	{
		HuoLiJQSprite.gameObject.SetActive(false);
		DaoDanSprite.gameObject.SetActive(false);

		GameJiTaiType jiTaiType = XkGameCtrl.GameJiTaiSt;
		switch (jiTaiType) {
		case GameJiTaiType.FeiJiJiTai:
			GaoBaoTextureCom.mainTexture = DanYaoFJ[0];
			DaoDanTextureCom.mainTexture = DanYaoFJ[2];
			break;

		case GameJiTaiType.TanKeJiTai:
			GaoBaoTextureCom.mainTexture = DanYaoTK[0];
			DaoDanTextureCom.mainTexture = DanYaoTK[2];
			break;
		}
		ShowJiQiangAmmoNum();

		if (Network.peerType == NetworkPeerType.Server) {
			gameObject.SetActive(false);
		}
	}

	void ShowJiQiangAmmoNum()
	{
		if (!AmmoGaoBao[1].enabled && AmmoGaoBao[2].enabled) {
			return;
		}
		AmmoGaoBao[0].enabled = false;
		AmmoGaoBao[1].enabled = false;
		AmmoGaoBao[2].enabled = true;
		
		GameJiTaiType jiTaiType = XkGameCtrl.GameJiTaiSt;
		switch (jiTaiType) {
		case GameJiTaiType.FeiJiJiTai:
			GaoBaoTextureCom.mainTexture = DanYaoFJ[0];
			break;
			
		case GameJiTaiType.TanKeJiTai:
			GaoBaoTextureCom.mainTexture = DanYaoTK[0];
			break;
		}
	}

	void ShowGaoBaoAmmoNum()
	{
		if (AmmoGaoBao[1].enabled && !AmmoGaoBao[2].enabled) {
			return;
		}
		AmmoGaoBao[0].enabled = true;
		AmmoGaoBao[1].enabled = true;
		AmmoGaoBao[2].enabled = false;

		GameJiTaiType jiTaiType = XkGameCtrl.GameJiTaiSt;
		switch (jiTaiType) {
		case GameJiTaiType.FeiJiJiTai:
			GaoBaoTextureCom.mainTexture = DanYaoFJ[1];
			break;

		case GameJiTaiType.TanKeJiTai:
			GaoBaoTextureCom.mainTexture = DanYaoTK[1];
			break;
		}
	}

	void CheckGaoBaoAmmoNum(int ammoNum)
	{
		if (GaoBaoAmmoNum == ammoNum) {
			return;
		}
		GaoBaoAmmoNum = ammoNum;

		int max = 2;
		int numVal = ammoNum;
		int valTmp = 0;
		int powVal = 0;
		bool isShowZero = false;
		for (int i = 0; i < 2; i++) {
			powVal = (int)Mathf.Pow(10, max - i - 1);
			valTmp = numVal / powVal;
			//Debug.Log("valTmp *** "+valTmp);
			if (!isShowZero) {
				if (valTmp > 0) {
					isShowZero = true;
				}
				else {
					if (i < (max - 1)) {
						AmmoGaoBao[i].enabled = false;
						continue;
					}
				}
			}
			
			if (!AmmoGaoBao[i].enabled) {
				AmmoGaoBao[i].enabled = true;
			}
			AmmoGaoBao[i].spriteName = "GBNum_" + valTmp;
			numVal -= valTmp * powVal;
		}
	}

	void CheckDaoDanAmmoNum(int ammoNum)
	{
		if (DaoDanAmmoNum == ammoNum) {
			return;
		}
		DaoDanAmmoNum = ammoNum;
		
		int max = 2;
		int numVal = ammoNum;
		int valTmp = 0;
		int powVal = 0;
		bool isShowZero = false;
		for (int i = 0; i < 2; i++) {
			powVal = (int)Mathf.Pow(10, max - i - 1);
			valTmp = numVal / powVal;
			//Debug.Log("valTmp *** "+valTmp+", numVal *** "+numVal);
			if (!isShowZero) {
				if (valTmp > 0) {
					isShowZero = true;
				}
				else {
					if (i < (max - 1)) {
						AmmoDaoDan[i].enabled = false;
						continue;
					}
				}
			}

			if (!AmmoDaoDan[i].enabled) {
				AmmoDaoDan[i].enabled = true;
			}
			AmmoDaoDan[i].spriteName = "DaoDanNum_" + valTmp;
			numVal -= valTmp * powVal;
		}
	}

	void CheckPlayerGaoBaoAmmoNum()
	{
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			if (XkGameCtrl.GaoBaoDanNumPOne > 0) {
				ShowGaoBaoAmmoNum();
				if (XkGameCtrl.GaoBaoDanNumPOne != GaoBaoAmmoNum) {
					CheckGaoBaoAmmoNum(XkGameCtrl.GaoBaoDanNumPOne);
				}
			}
			else {
				ShowJiQiangAmmoNum();
			}

			if (XkGameCtrl.DaoDanNumPOne != DaoDanAmmoNum) {
				CheckDaoDanAmmoNum(XkGameCtrl.DaoDanNumPOne);
			}
			break;

		case PlayerEnum.PlayerTwo:
			if (XkGameCtrl.GaoBaoDanNumPTwo > 0) {
				ShowGaoBaoAmmoNum();
				if (XkGameCtrl.GaoBaoDanNumPTwo != GaoBaoAmmoNum) {
					CheckGaoBaoAmmoNum(XkGameCtrl.GaoBaoDanNumPTwo);
				}
			}
			else {
				ShowJiQiangAmmoNum();
			}
			
			if (XkGameCtrl.DaoDanNumPTwo != DaoDanAmmoNum) {
				CheckDaoDanAmmoNum(XkGameCtrl.DaoDanNumPTwo);
			}
			break;
		}
	}
	
	public void ShowHuoLiJQSprite()
	{
		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}

		if (HuoLiJQSprite.gameObject.activeSelf) {
			return;
		}
		HuoLiJQSprite.fillAmount = 0f;
		HuoLiJQSprite.gameObject.SetActive(true);
		StartCoroutine(ChangeHuoLiJQSpriteAmount());
	}
	
	void HiddenHuoLiJQSprite()
	{
		//Debug.Log("HiddenHuoLiJQSprite...");
		if (!HuoLiJQSprite.gameObject.activeSelf) {
			return;
		}
		HuoLiJQSprite.gameObject.SetActive(false);
	}

	IEnumerator ChangeHuoLiJQSpriteAmount()
	{
		bool isStopChange = false;
		do {
			HuoLiJQSprite.fillAmount += 0.2f;
			if (HuoLiJQSprite.fillAmount >= 1f) {
				HuoLiJQSprite.fillAmount = 1f;
				isStopChange = true;
				Invoke("HiddenHuoLiJQSprite", 3f);
				yield break;
			}
			else {
				yield return new WaitForSeconds(0.05f);
			}
		} while (!isStopChange);
	}
	
	public void ShowDaoDanSprite()
	{
		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}

		if (!gameObject.activeSelf) {
			return;
		}

		if (DaoDanSprite.gameObject.activeSelf) {
			return;
		}
		
		GameJiTaiType jiTaiType = XkGameCtrl.GameJiTaiSt;
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			if (jiTaiType == GameJiTaiType.FeiJiJiTai) {
				DaoDanSprite.spriteName = "1PDaoDan";
			}
			else if (jiTaiType == GameJiTaiType.TanKeJiTai) {
				DaoDanSprite.spriteName = "1PPaoDan";
			}
			break;
			
		case PlayerEnum.PlayerTwo:
			if (jiTaiType == GameJiTaiType.FeiJiJiTai) {
				DaoDanSprite.spriteName = "2PDaoDan";
			}
			else if (jiTaiType == GameJiTaiType.TanKeJiTai) {
				DaoDanSprite.spriteName = "2PPaoDan";
			}
			break;
		}
		DaoDanSprite.fillAmount = 0f;
		DaoDanSprite.gameObject.SetActive(true);
		StartCoroutine(ChangeDaoDanSpriteAmount());
	}
	
	void HiddenDaoDanSprite()
	{
		//Debug.Log("HiddenDaoDanSprite...");
		if (!DaoDanSprite.gameObject.activeSelf) {
			return;
		}
		DaoDanSprite.gameObject.SetActive(false);
	}
	
	IEnumerator ChangeDaoDanSpriteAmount()
	{
		bool isStopChange = false;
		do {
			DaoDanSprite.fillAmount += 0.2f;
			if (DaoDanSprite.fillAmount >= 1f) {
				DaoDanSprite.fillAmount = 1f;
				isStopChange = true;
				Invoke("HiddenDaoDanSprite", 3f);
				yield break;
			}
			else {
				yield return new WaitForSeconds(0.05f);
			}
		} while (!isStopChange);
	}
}