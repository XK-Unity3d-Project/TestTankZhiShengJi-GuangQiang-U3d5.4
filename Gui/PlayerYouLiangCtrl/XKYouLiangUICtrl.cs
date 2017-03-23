using UnityEngine;
using System.Collections;

public class XKYouLiangUICtrl : MonoBehaviour
{
	public UITexture UITextureCom;
	public Texture TexturePXueLiang1;
	public Texture TexturePXueLiang2;
	// Use this for initialization
	void Start()
	{
		switch(XkGameCtrl.SelectYouLiangUI)
		{
		case 1:
			if (UITextureCom != null) {
				UITextureCom.mainTexture = TexturePXueLiang1;
			}
			break;
		case 2:
			if (UITextureCom != null) {
				UITextureCom.mainTexture = TexturePXueLiang2;
			}
			break;
		}
	}
}