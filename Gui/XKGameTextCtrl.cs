using UnityEngine;
using System.Collections;

public class XKGameTextCtrl : MonoBehaviour {
	public Texture TextureCH;
	public Texture TextureEN;
	public UISpriteAnimation UISpAniCom;
	public string ChSpAni; //中文前缀.
	public string EnSpAni; //英文前缀.
	public UISprite UISpCom;
	public string ChSpName; //中文图名称.
	public string EnSpName; //英文图名称.
	public MeshRenderer MeshRenderCom;
	public Material Material_Ch;
	public Material Material_En;
	GameTextType GameTextVal = XKGlobalData.GameTextVal;
	// Use this for initialization
	void Start()
	{
		GameTextVal = XKGlobalData.GetGameTextMode();
		//GameTextVal = GameTextType.English; //test.
		//Debug.Log("GameTextVal "+GameTextVal);
		CheckGameUITexture();
		CheckUISpAniCom();
		CheckGameUISpCom();
		CheckMeshRenderCom();
	}

	void CheckGameUITexture()
	{
		if (TextureCH != null && TextureEN != null) {
			//改变UITexture的图片.
			UITexture uiTextureCom = GetComponent<UITexture>();
			switch (GameTextVal) {
			case GameTextType.Chinese:
				if (uiTextureCom != null) {
					uiTextureCom.mainTexture = TextureCH;
				}
				break;
				
			case GameTextType.English:
				if (uiTextureCom != null) {
					uiTextureCom.mainTexture = TextureEN;
				}
				break;
			}
		}
	}
	
	void CheckUISpAniCom()
	{
		if (UISpAniCom == null) {
			return;
		}
		
		switch (GameTextVal) {
		case GameTextType.Chinese:
			UISpAniCom.namePrefix = ChSpAni;
			break;
			
		case GameTextType.English:
			UISpAniCom.namePrefix = EnSpAni;
			break;
		}
	}
	
	void CheckGameUISpCom()
	{
		if (UISpCom == null) {
			return;
		}

		switch (GameTextVal) {
		case GameTextType.Chinese:
			UISpCom.spriteName = ChSpName;
			break;
			
		case GameTextType.English:
			UISpCom.spriteName = EnSpName;
			break;
		}
	}
	
	void CheckMeshRenderCom()
	{
		if (MeshRenderCom == null) {
			return;
		}
		
		switch (GameTextVal) {
		case GameTextType.Chinese:
			MeshRenderCom.material = Material_Ch;
			break;
			
		case GameTextType.English:
			MeshRenderCom.material = Material_En;
			break;
		}
	}
}