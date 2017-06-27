using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class ZhunXingCtrl : MonoBehaviour {
	public PlayerEnum PlayerSt;
	public UISprite ZhunXingSprite;
	GameObject ZhunXingObj;
	Transform ZhunXingTran;
	bool IsFixZhunXing;
	GameMode ZXModeVal = GameMode.Null;
	static ZhunXingCtrl _InstanceOne;
	static ZhunXingCtrl _InstanceTwo;
	public static int SM_CXSCREEN = 0;
	public static int SM_CYSCREEN = 1;
	[DllImport("user32")]
	public static extern int GetSystemMetrics(int nIndex);
	public static ZhunXingCtrl GetInstanceOne()
	{
		return _InstanceOne;
	}

	public static ZhunXingCtrl GetInstanceTwo()
	{
		return _InstanceTwo;
	}

	// Use this for initialization
	void Start()
	{
		ZhunXingTran = transform;
		ZhunXingObj = gameObject;
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			_InstanceOne = this;
			if (XkGameCtrl.IsActivePlayerOne) {
				SetActiveZhunXingObj(true);
			}
			else {
				SetActiveZhunXingObj(false);
			}
			break;

		case PlayerEnum.PlayerTwo:
			_InstanceTwo = this;
			if (XkGameCtrl.IsActivePlayerTwo) {
				SetActiveZhunXingObj(true);
			}
			else {
				SetActiveZhunXingObj(false);
			}
			break;
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		FixedUpdateGunCross();
	}

	public void FixedUpdateGunCross()
	{
		Vector3 mousePosInput = Input.mousePosition;
		if (pcvr.bIsHardWare) {
			switch (PlayerSt) {
			case PlayerEnum.PlayerOne:
				mousePosInput = pcvr.CrossPositionOne;
				break;
			
			case PlayerEnum.PlayerTwo:
				mousePosInput = pcvr.CrossPositionTwo;
				break;
			
			case PlayerEnum.PlayerThree:
				mousePosInput = pcvr.CrossPositionThree;
				break;
			
			case PlayerEnum.PlayerFour:
				mousePosInput = pcvr.CrossPositionFour;
				break;
			}
		}
		else {
			if (!GameMovieCtrl.IsThreeScreenGame) {
				mousePosInput.x *= (XkGameCtrl.ScreenWidth / Screen.width);
				mousePosInput.y *= (XkGameCtrl.ScreenHeight / Screen.height);
			}
			else {
				mousePosInput.x *= (XkGameCtrl.ScreenWidth3 / Screen.width);
				mousePosInput.y *= (XkGameCtrl.ScreenHeight3 / Screen.height);
			}
		}

		if(IsFixZhunXing != Screen.fullScreen)
		{
			IsFixZhunXing = Screen.fullScreen;
			int iScreenW = GetSystemMetrics(SM_CXSCREEN);
			int iScreenH = GetSystemMetrics(SM_CYSCREEN);
			if(!Screen.fullScreen)
			{
				iScreenW = Screen.width;
				iScreenH = Screen.height;
			}
			
			float sx = (1360f *(float)iScreenH) / (768f * (float)iScreenW);
			if (GameMovieCtrl.IsThreeScreenGame) {
				sx = (1360f *(float)iScreenH) / (768f * (float)iScreenW / 3f);
			}
			//ScreenLog.Log("sx **** " + sx + ", Screen: " + iScreenW + ", " + iScreenH );
			transform.localScale = new Vector3(sx, transform.localScale.y, transform.localScale.z);
		}
		ZhunXingTran.localPosition = mousePosInput;
	}
	
	void OnTriggerEnter(Collider other)
	{
//		Debug.Log("test---OnTriggerEnter -> Collider "+other.gameObject.name
//		          +", IsSelectDanJiMode "+GameModeCtrl.IsSelectDanJiMode);
		if (Application.loadedLevel != (int)GameLevel.Movie) {
			return;
		}
		
//		if (GameModeCtrl.IsSelectDanJiMode) {
//			return;
//		}
		if (GameModeCtrl.GameModeState != 0) {
			return;
		}

		GameMoShiInfo script = other.GetComponent<GameMoShiInfo>();
		if (script == null) {
			return;
		}
//		Debug.Log("OnTriggerEnter -> Collider "+other.gameObject.name);

		switch (script.AppModeVal) {
		case GameMode.LianJi:
			if (ZXModeVal != GameMode.LianJi) {
				GameMoShiInfo.InstanceLianJi.SetTransformScale(new Vector3(1.1f, 1.1f, 1f));
				GameMoShiInfo.InstanceDanJi.SetTransformScale(new Vector3(1f, 1f, 1f));
				GameModeCtrl.ModeVal = GameMode.LianJi;
				GameModeCtrl.GetInstance().SetActiveStartBt(true);
				XKGlobalData.GetInstance().PlayModeXuanZeAudio();
			}
			break;

		default:
			if (ZXModeVal == GameMode.Null || ZXModeVal == GameMode.LianJi) {
				GameMoShiInfo.InstanceLianJi.SetTransformScale(new Vector3(1f, 1f, 1f));
				GameMoShiInfo.InstanceDanJi.SetTransformScale(new Vector3(1.1f, 1.1f, 1f));
				GameModeCtrl.ModeVal = GameMode.DanJiFeiJi;
				GameModeCtrl.GetInstance().SetActiveStartBt(true);
				XKGlobalData.GetInstance().PlayModeXuanZeAudio();
			}
			break;
		}
		ZXModeVal = GameModeCtrl.ModeVal;
	}

	public void SetActiveZhunXingObj(bool isActive)
	{
		if (isActive) {
			SetZhunXingSprite();
		}

		if (Application.loadedLevel == (int)GameLevel.Movie && isActive) {
			Invoke("DelayActivePlayerZhunXing", 0.5f);
		}
		else {
			ZhunXingObj.SetActive(isActive);
		}
	}

	void DelayActivePlayerZhunXing()
	{
		ZhunXingObj.SetActive(true);
	}

	public bool GetActiveZhunXing()
	{
		return ZhunXingObj.activeSelf;
	}

	public void SetZhunXingSprite()
	{
		Vector2[] zhunXingScale = new Vector2[3]{
			new Vector2(94, 146),
			new Vector2(166, 166),
			new Vector2(281, 201)
		};

		int indexVal = 0;
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			if (XKPlayerAutoFire.IsAimPlayerPOne) {
				ZhunXingSprite.spriteName = "ZhunXingP1_2";
			}
			else {
				if (XkGameCtrl.GaoBaoDanNumPOne <= 0) {
					ZhunXingSprite.spriteName = "ZhunXingP1_0";
					indexVal = 1;
				}
				else {
					ZhunXingSprite.spriteName = "ZhunXingP1_1";
					indexVal = 2;
				}
			}
			break;
			
		case PlayerEnum.PlayerTwo:
			if (XKPlayerAutoFire.IsAimPlayerPTwo) {
				ZhunXingSprite.spriteName = "ZhunXingP2_2";
			}
			else {
				if (XkGameCtrl.GaoBaoDanNumPTwo <= 0) {
					ZhunXingSprite.spriteName = "ZhunXingP2_0";
					indexVal = 1;
				}
				else {
					ZhunXingSprite.spriteName = "ZhunXingP2_1";
					indexVal = 2;
				}
			}
			break;
		}
		ZhunXingSprite.width = (int)zhunXingScale[indexVal].x;
		ZhunXingSprite.height = (int)zhunXingScale[indexVal].y;
	}
}