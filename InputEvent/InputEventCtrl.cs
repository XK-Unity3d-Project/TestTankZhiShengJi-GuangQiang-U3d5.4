#define FIREBT_STARTBT
/**
 * FIREBT_STARTBT -> 发射按键带有开始按键功能.
 */
using UnityEngine;
using System.Collections;

public class InputEventCtrl : MonoBehaviour {
	public static bool IsClickFireBtOneDown;
	public static bool IsClickFireBtTwoDown;
	public static bool IsClickDaoDanBtOneDown;
	public static bool IsClickDaoDanBtTwoDown;
	static private InputEventCtrl Instance = null;
	static public InputEventCtrl GetInstance()
	{
		if(Instance == null)
		{
			GameObject obj = new GameObject("_InputEventCtrl");
			Instance = obj.AddComponent<InputEventCtrl>();
			pcvr.GetInstance();
			XKGlobalData.GetInstance();
			SetPanelCtrl.GetInstance();
		}
		return Instance;
	}

	#region Click Button Envent
	public delegate void EventHandel(ButtonState val);
	public event EventHandel ClickStartBtOneEvent;
	public void ClickStartBtOne(ButtonState val)
	{
		if(ClickStartBtOneEvent != null)
		{
			ClickStartBtOneEvent( val );
			//pcvr.StartLightStateP1 = LedState.Mie;
			/*if (val == ButtonState.UP) {
				XKGlobalData.GetInstance().PlayStartBtAudio();
			}*/
		}
		pcvr.SetIsPlayerActivePcvr();
	}
	
	public event EventHandel ClickStartBtTwoEvent;
	public void ClickStartBtTwo(ButtonState val)
	{
		if(ClickStartBtTwoEvent != null)
		{
			ClickStartBtTwoEvent( val );
			//pcvr.StartLightStateP2 = LedState.Mie;
			/*if (val == ButtonState.UP) {
				XKGlobalData.GetInstance().PlayStartBtAudio();
			}*/
		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public event EventHandel ClickSetEnterBtEvent;
	public void ClickSetEnterBt(ButtonState val)
	{
//		SetEnterBtSt = val;
		if(ClickSetEnterBtEvent != null)
		{
			ClickSetEnterBtEvent( val );
		}

		if (val == ButtonState.DOWN) {
			XKGlobalData.PlayAudioSetEnter();
//			TimeSetEnterMoveBt = Time.time;
		}
		pcvr.SetIsPlayerActivePcvr();
	}
	
	public event EventHandel ClickSetMoveBtEvent;
	public void ClickSetMoveBt(ButtonState val)
	{
		if(ClickSetMoveBtEvent != null)
		{
			ClickSetMoveBtEvent( val );
		}

		if (val == ButtonState.DOWN) {
			XKGlobalData.PlayAudioSetMove();
		}
		pcvr.SetIsPlayerActivePcvr();
	}
	
	public event EventHandel ClickFireBtOneEvent;
	public void ClickFireBtOne(ButtonState val)
	{
		#if FIREBT_STARTBT
		{
			if (SetPanelUiRoot.GetInstance() == null || HardwareCheckCtrl.IsTestHardWare) {
				if(ClickStartBtOneEvent != null)
				{
					ClickStartBtOneEvent( val );
				}
			}
		}
		#endif
		if(ClickFireBtOneEvent != null)
		{
			ClickFireBtOneEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public event EventHandel ClickFireBtTwoEvent;
	public void ClickFireBtTwo(ButtonState val)
	{
		#if FIREBT_STARTBT
		{
			if (SetPanelUiRoot.GetInstance() == null || HardwareCheckCtrl.IsTestHardWare) {
				if(ClickStartBtTwoEvent != null)
				{
					ClickStartBtTwoEvent( val );
				}
			}
		}
		#endif
		if(ClickFireBtTwoEvent != null)
		{
			ClickFireBtTwoEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public event EventHandel ClickDaoDanBtOneEvent;
	public void ClickDaoDanBtOne(ButtonState val)
	{
		if(ClickDaoDanBtOneEvent != null)
		{
			ClickDaoDanBtOneEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}
	
	public event EventHandel ClickDaoDanBtTwoEvent;
	public void ClickDaoDanBtTwo(ButtonState val)
	{
		if(ClickDaoDanBtTwoEvent != null)
		{
			ClickDaoDanBtTwoEvent( val );
		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public event EventHandel ClickStopDongGanBtOneEvent;
	public void ClickStopDongGanBtOne(ButtonState val)
	{
		if(ClickStopDongGanBtOneEvent != null)
		{
			ClickStopDongGanBtOneEvent( val );
		}

		if (val == ButtonState.DOWN) {
			DongGanUICtrl.ShowDongGanInfo();
		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public event EventHandel ClickStopDongGanBtTwoEvent;
	public void ClickStopDongGanBtTwo(ButtonState val)
	{
		if(ClickStopDongGanBtTwoEvent != null)
		{
			ClickStopDongGanBtTwoEvent( val );
		}
		
		if (val == ButtonState.DOWN) {
			DongGanUICtrl.ShowDongGanInfo();
		}
		pcvr.SetIsPlayerActivePcvr();
	}
	#endregion
	
	float TimeFireBt;
//	float TimeSetEnterMoveBt;
//	ButtonState SetEnterBtSt = ButtonState.UP;
	void Update()
	{
//		if (SetEnterBtSt == ButtonState.DOWN && Time.time - TimeSetEnterMoveBt > 2f) {
//			HardwareCheckCtrl.OnRestartGame();
//		}

		if (pcvr.bIsHardWare && !pcvr.IsGetValByKey) {
			return;
		}

		if (Input.GetKeyUp(KeyCode.T)) {
			int coinVal = XKGlobalData.CoinPlayerOne + 1;
			XKGlobalData.SetCoinPlayerOne(coinVal);
		}

		if (Input.GetKeyUp(KeyCode.I)) {
			int coinVal = XKGlobalData.CoinPlayerTwo + 1;
			XKGlobalData.SetCoinPlayerTwo(coinVal);
		}

		//StartBt PlayerOne
		if(Input.GetKeyUp(KeyCode.G))
		{
			ClickStartBtOne( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.G))
		{
			ClickStartBtOne( ButtonState.DOWN );
		}
		
		//StartBt PlayerTwo
		if(Input.GetKeyUp(KeyCode.K))
		{
			ClickStartBtTwo( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.K))
		{
			ClickStartBtTwo( ButtonState.DOWN );
		}
		
		//setPanel enter button
		if(Input.GetKeyUp(KeyCode.F4))
		{
			ClickSetEnterBt( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.F4))
		{
			ClickSetEnterBt( ButtonState.DOWN );
		}
		
		//setPanel move button
		if(Input.GetKeyUp(KeyCode.F5))
		{
			ClickSetMoveBt( ButtonState.UP );
			//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.F5))
		{
			ClickSetMoveBt( ButtonState.DOWN );
			//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.DOWN );
		}

		//Fire button
//		if(Input.GetKeyUp(KeyCode.Mouse0))
//		{
//			IsClickFireBtOneDown = false;
//			ClickFireBtOne( ButtonState.UP );
//
//			IsClickFireBtTwoDown = false;
//			ClickFireBtTwo( ButtonState.UP );
//		}
		
		if(Input.GetKeyDown(KeyCode.Mouse0))
		{
			IsClickFireBtOneDown = true;
			ClickFireBtOne( ButtonState.DOWN );

			IsClickFireBtTwoDown = true;
			ClickFireBtTwo( ButtonState.DOWN );
		}

		if(Input.GetKeyUp(KeyCode.Mouse1))
		{
			IsClickDaoDanBtOneDown = false;
			ClickDaoDanBtOne( ButtonState.UP );

			IsClickDaoDanBtTwoDown = false;
			ClickDaoDanBtTwo( ButtonState.UP );
		}
		
		if(Input.GetKeyDown(KeyCode.Mouse1))
		{
			IsClickDaoDanBtOneDown = true;
			ClickDaoDanBtOne( ButtonState.DOWN );

			IsClickDaoDanBtTwoDown = true;
			ClickDaoDanBtTwo( ButtonState.DOWN );
		}

		if(Input.GetKeyDown(KeyCode.C))
		{
			ClickStopDongGanBtOne(ButtonState.DOWN);
		}
	}
}

public enum ButtonState : int
{
	UP = 1,
	DOWN = -1
}