using UnityEngine;
using System.Collections;

public class CoinPlayerCtrl : MonoBehaviour {
	public PlayerEnum PlayerSt = PlayerEnum.Null;
	public UISprite CoinSpriteA; //ShiWei
	public UISprite CoinSpriteB; //GeWei
	public UISprite NeedCoinSpriteA; //ShiWei
	public UISprite NeedCoinSpriteB; //GeWei
	public GameObject InsertCoinObj;
	public GameObject StartBtObj;
	public GameObject CoinGroup;
	public GameObject FreeMode;
	static CoinPlayerCtrl _InstanceOne;
	public static CoinPlayerCtrl GetInstanceOne()
	{
		return _InstanceOne;
	}

	static CoinPlayerCtrl _InstanceTwo;
	public static CoinPlayerCtrl GetInstanceTwo()
	{
		return _InstanceTwo;
	}

	// Use this for initialization
	void Start()
	{
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			_InstanceOne = this;
			XKGlobalData.SetCoinPlayerOne(XKGlobalData.CoinPlayerOne);
			InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartBtOneEvent;
			break;
			
		case PlayerEnum.PlayerTwo:
			_InstanceTwo = this;
			XKGlobalData.SetCoinPlayerTwo(XKGlobalData.CoinPlayerTwo);
			InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickStartBtTwoEvent;
			break;
		}
		SetGameNeedCoin(XKGlobalData.GameNeedCoin);
		SetActiveFreeMode(XKGlobalData.IsFreeMode);
		InsertCoinObj.SetActive(false);
		StartBtObj.SetActive(false);
		
		switch(GameTypeCtrl.AppTypeStatic) {
		case AppGameType.LianJiServer:
			gameObject.SetActive(false);
			break;
		}
	}

	void Update()
	{
		if ((JiFenJieMianCtrl.GetInstance() != null && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask())
		    || (PlayerSt == PlayerEnum.PlayerOne && XkGameCtrl.IsActivePlayerOne)
		    || (PlayerSt == PlayerEnum.PlayerTwo && XkGameCtrl.IsActivePlayerTwo)) {
			if (InsertCoinObj.activeSelf) {
				InsertCoinObj.SetActive(false);
			}

			if (StartBtObj.activeSelf) {
				StartBtObj.SetActive(false);
			}
			return;
		}

		CheckPlayerOneCoinCur();
		CheckPlayerTwoCoinCur();
	}

	public void HiddenPlayerCoin()
	{
		gameObject.SetActive(false);
	}

	public void SetActiveFreeMode(bool isActive)
	{
		if (isActive && InsertCoinObj.activeSelf) {
			InsertCoinObj.SetActive(false);
		}
		FreeMode.SetActive(isActive);
		CoinGroup.SetActive(!isActive);
	}

	void ClickStartBtOneEvent(ButtonState state)
	{
		if (XkGameCtrl.IsActivePlayerOne) {
			return;
		}

		if (!StartBtObj.activeSelf) {
			return;
		}

		if (GameOverCtrl.IsShowGameOver) {
			return;
		}
		XKGlobalData.GetInstance().PlayStartBtAudio();
		SubCoinPlayerOne();
		StartBtObj.SetActive(false);
		XkGameCtrl.SetActivePlayerOne(true);
	}

	void ClickStartBtTwoEvent(ButtonState state)
	{
		if (XkGameCtrl.IsActivePlayerTwo) {
			return;
		}
		
		if (!StartBtObj.activeSelf) {
			return;
		}
		
		if (GameOverCtrl.IsShowGameOver) {
			return;
		}
		XKGlobalData.GetInstance().PlayStartBtAudio();
		SubCoinPlayerTwo();
		StartBtObj.SetActive(false);
		XkGameCtrl.SetActivePlayerTwo(true);
	}

	void SubCoinPlayerOne()
	{
		pcvr.GetInstance().SubPlayerCoin(XKGlobalData.GameNeedCoin, PlayerEnum.PlayerOne);
		XKGlobalData.CoinPlayerOne -= XKGlobalData.GameNeedCoin;
		SetPlayerCoin(XKGlobalData.CoinPlayerOne);
	}
	
	void SubCoinPlayerTwo()
	{
		pcvr.GetInstance().SubPlayerCoin(XKGlobalData.GameNeedCoin, PlayerEnum.PlayerTwo);
		XKGlobalData.CoinPlayerTwo -= XKGlobalData.GameNeedCoin;
		SetPlayerCoin(XKGlobalData.CoinPlayerTwo);
	}

	void CheckPlayerOneCoinCur()
	{
		if (PlayerSt != PlayerEnum.PlayerOne) {
			return;
		}

		if (XkGameCtrl.IsActivePlayerOne) {
			return;
		}

		if (!XKGlobalData.IsFreeMode) {
			if (XKGlobalData.CoinPlayerOne < XKGlobalData.GameNeedCoin && !InsertCoinObj.activeSelf) {
				InsertCoinObj.SetActive(true); //Active Insert Coin
				StartBtObj.SetActive(false);
			}
			else if (XKGlobalData.CoinPlayerOne >= XKGlobalData.GameNeedCoin && (InsertCoinObj.activeSelf || !StartBtObj.activeSelf)) {
				InsertCoinObj.SetActive(false); //Hidden Insert Coin
				StartBtObj.SetActive(true);
			}
		}
		else {
			if (!StartBtObj.activeSelf) {
				StartBtObj.SetActive(true);
			}
		}

		if (StartBtObj.activeSelf) {
			pcvr.StartLightStateP1 = LedState.Shan;
		}
	}

	void CheckPlayerTwoCoinCur()
	{
		if (PlayerSt != PlayerEnum.PlayerTwo) {
			return;
		}
		
		if (XkGameCtrl.IsActivePlayerTwo) {
			return;
		}
		
		if (!XKGlobalData.IsFreeMode) {
			if (XKGlobalData.CoinPlayerTwo < XKGlobalData.GameNeedCoin && !InsertCoinObj.activeSelf) {
				InsertCoinObj.SetActive(true); //Active Insert Coin
				StartBtObj.SetActive(false);
			}
			else if (XKGlobalData.CoinPlayerTwo >= XKGlobalData.GameNeedCoin && (InsertCoinObj.activeSelf || !StartBtObj.activeSelf)) {
				InsertCoinObj.SetActive(false); //Hidden Insert Coin
				StartBtObj.SetActive(true);
			}
		}
		else {
			if (!StartBtObj.activeSelf) {
				StartBtObj.SetActive(true);
			}
		}
		
		if (StartBtObj.activeSelf) {
			pcvr.StartLightStateP2 = LedState.Shan;
		}
	}

	public void SetPlayerCoin(int coin)
	{
		SetPlayerCoinSprite(coin);
	}

	void SetPlayerCoinSprite(int num)
	{
		if(num > 99)
		{
			switch (PlayerSt) {
			case PlayerEnum.PlayerOne:
				CoinSpriteA.spriteName = "p1_9";
				CoinSpriteB.spriteName = "p1_9";
				break;

			case PlayerEnum.PlayerTwo:
				CoinSpriteA.spriteName = "p2_9";
				CoinSpriteB.spriteName = "p2_9";
				break;
			}
		}
		else
		{
			string playerCoinStr = "";
			switch (PlayerSt) {
			case PlayerEnum.PlayerOne:
				playerCoinStr = "p1_";
				break;
				
			case PlayerEnum.PlayerTwo:
				playerCoinStr = "p2_";
				break;
			}

			int coinShiWei = (int)((float)num/10.0f);
			CoinSpriteA.spriteName = playerCoinStr + coinShiWei.ToString();
			CoinSpriteB.spriteName = playerCoinStr + (num%10).ToString();
		}
	}

	public void SetGameNeedCoin(int coin)
	{
		SetGameNeedCoinSprite(coin);
	}

	void SetGameNeedCoinSprite(int num)
	{
		string playerCoinStr = "";
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			playerCoinStr = "p1_";
			break;
			
		case PlayerEnum.PlayerTwo:
			playerCoinStr = "p2_";
			break;
		}
		NeedCoinSpriteA.spriteName = playerCoinStr + (num/10).ToString();
		NeedCoinSpriteB.spriteName = playerCoinStr + (num%10).ToString();
	}
}