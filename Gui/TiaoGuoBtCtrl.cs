using UnityEngine;
using System.Collections;

public enum TiaoGuoBtState
{
	Cartoon,
	JiFenPanel
}

public class TiaoGuoBtCtrl : MonoBehaviour {
	public TiaoGuoBtState BtState = TiaoGuoBtState.JiFenPanel;
	GameObject TiaoGuoBtObj;
	static TiaoGuoBtCtrl InstanceCartoon;
	public static TiaoGuoBtCtrl GetInstanceCartoon()
	{
		return InstanceCartoon;
	}
	
	static TiaoGuoBtCtrl InstanceJiFen;
	public static TiaoGuoBtCtrl GetInstanceJiFen()
	{
		return InstanceJiFen;
	}

	// Use this for initialization
	void Start()
	{
		TiaoGuoBtObj = gameObject;
		InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartBtOneEvent;
		InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickStartBtTwoEvent;
		if (BtState != TiaoGuoBtState.Cartoon) {
			InstanceJiFen = this;
		}
		else {
			InstanceCartoon = this;
		}
		TiaoGuoBtObj.SetActive(false);
	}

	public void ShowTiaoGuoBt()
	{
		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			return;
		}

		if (TiaoGuoBtObj.activeSelf) {
			return;
		}
		TiaoGuoBtObj.SetActive(true);
		
		pcvr.StartLightStateP1 = LedState.Shan;
		pcvr.StartLightStateP2 = LedState.Shan;
	}
	
	public void HiddenTiaoGuoBt()
	{
		if (!TiaoGuoBtObj.activeSelf) {
			return;
		}
		TiaoGuoBtObj.SetActive(false);
		
		pcvr.StartLightStateP1 = LedState.Mie;
		pcvr.StartLightStateP2 = LedState.Mie;
	}

	void ClickStartBtOneEvent(ButtonState state)
	{
		if (!TiaoGuoBtObj.activeSelf) {
			return;
		}

		if (state == ButtonState.DOWN) {
			return;
		}
		OnClickTiaoGuoBt();
	}
	
	void ClickStartBtTwoEvent(ButtonState state)
	{
		if (!TiaoGuoBtObj.activeSelf) {
			return;
		}
		
		if (state == ButtonState.DOWN) {
			return;
		}
		OnClickTiaoGuoBt();
	}

	void OnClickTiaoGuoBt()
	{
		XKGlobalData.GetInstance().PlayStartBtAudio();
		HiddenTiaoGuoBt();
		if (!JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
			if (Network.peerType != NetworkPeerType.Server) {
				XKTriggerEndCartoon.GetInstance().CloseStartCartoon();
			}
		}
		/*else {
			JiFenJieMianCtrl.GetInstance().StopJiFenTime();
		}*/
	}
}