using UnityEngine;
using System.Collections;

public class GameMoShiInfo : MonoBehaviour {
	public GameMode AppModeVal = GameMode.Null;
	public static GameMoShiInfo InstanceDanJi;
	public static GameMoShiInfo InstanceLianJi;
	UISprite ModeSprite;
	GameTextType GameTextVal = XKGlobalData.GameTextVal;
	void Start()
	{
		GameTextVal = XKGlobalData.GetGameTextMode();
		//GameTextVal = GameTextType.English; //test
		ModeSprite = GetComponent<UISprite>();
		switch (AppModeVal) {
		case GameMode.LianJi:
			InstanceLianJi = this;
			break;

		default:
			InstanceDanJi = this;
			break;
		}
		InitGameModeImg();
	}

	void InitGameModeImg()
	{
		if (AppModeVal == GameMode.LianJi) {
			switch (GameTextVal) {
			case GameTextType.Chinese:
				InstanceLianJi.ModeSprite.spriteName = "ShuangRen_1";
				break;
				
			case GameTextType.English:
				InstanceLianJi.ModeSprite.spriteName = "ShuangRen_En_1";
				break;
			}
		}
		else {
			switch (GameTextVal) {
			case GameTextType.Chinese:
				InstanceDanJi.ModeSprite.spriteName = "DanRen_1";
				break;
				
			case GameTextType.English:
				InstanceDanJi.ModeSprite.spriteName = "DanRen_En_1";
				break;
			}
		}
	}

	public void SetTransformScale(Vector3 scaleVal)
	{
		transform.localScale = scaleVal;
		if (scaleVal != new Vector3(1f, 1f, 1f)) {
			if (GameTextVal == GameTextType.Chinese) {
				switch (AppModeVal) {
				case GameMode.LianJi:
					InstanceDanJi.ModeSprite.spriteName = "DanRen_1";
					InstanceLianJi.ModeSprite.spriteName = "ShuangRen_2";
					break;
					
				default:
					InstanceDanJi.ModeSprite.spriteName = "DanRen_2";
					InstanceLianJi.ModeSprite.spriteName = "ShuangRen_1";
					break;
				}
			}
			else {
				switch (AppModeVal) {
				case GameMode.LianJi:
					InstanceDanJi.ModeSprite.spriteName = "DanRen_En_1";
					InstanceLianJi.ModeSprite.spriteName = "ShuangRen_En_2";
					break;
					
				default:
					InstanceDanJi.ModeSprite.spriteName = "DanRen_En_2";
					InstanceLianJi.ModeSprite.spriteName = "ShuangRen_En_1";
					break;
				}
			}
		}
	}
}