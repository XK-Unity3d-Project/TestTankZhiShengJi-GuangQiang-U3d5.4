using UnityEngine;
using System.Collections;

public class DongGanUICtrl : MonoBehaviour {
	/**
	 * DongGanUI[0] -> 关闭动感.
	 * DongGanUI[1] -> 打开动感.
	 */
	public Texture[] DongGanUI;
	public Texture[] DongGanUI_Ch;
	public Texture[] DongGanUI_En;
	UITexture DongGanTexture;
	public static int DongGanCount;
	public static DongGanUICtrl Instance;
	// Use this for initialization
	void Start()
	{
		Instance = this;
		GameTextType gameTextVal = XKGlobalData.GetGameTextMode();
		//gameTextVal = GameTextType.English; //test.
		switch (gameTextVal) {
		case GameTextType.Chinese:
			DongGanUI = DongGanUI_Ch;
			break;
			
		case GameTextType.English:
			DongGanUI = DongGanUI_En;
			break;
		}

		DongGanCount = 1;
		DongGanTexture = GetComponent<UITexture>();
		DongGanTexture.mainTexture = DongGanUI[0];
		gameObject.SetActive(false);
	}

	public static void ShowDongGanInfo()
	{
		if (!ScreenDanHeiCtrl.IsStartGame) {
			return;
		}

		if (JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
			return;
		}

		if (Instance == null) {
			return;
		}

		DongGanCount++;
		if (DongGanCount > 1) {
			DongGanCount = 0;
		}
		pcvr.DongGanState = (byte)DongGanCount;
		Instance.ShowDongGanUI(DongGanCount);
	}

	void ShowDongGanUI(int index)
	{
		DongGanTexture.mainTexture = DongGanUI[index];
		gameObject.SetActive(true);

		if (index == 1) {
			Invoke("HiddenDongGanUI", 3f);
		}
	}

	void HiddenDongGanUI()
	{
		gameObject.SetActive(false);
	}
}