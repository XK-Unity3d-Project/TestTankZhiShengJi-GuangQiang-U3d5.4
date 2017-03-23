using UnityEngine;
using System.Collections;
using System;
using System.IO;

public enum GameTextType
{
	Chinese,
	English,
}

public class XKGlobalData {
	public static GameTextType GameTextVal = GameTextType.Chinese;
	public static int CoinPlayerOne = 0;
	public static int CoinPlayerTwo = 0;
	public static int CoinPlayerThree = 0;
	public static int CoinPlayerFour = 0;
	public static int GunZhenDongP1 = 0;
	public static int GunZhenDongP2 = 0;
	public static int DianJiSpeedP1 = 0;
	public static int DianJiSpeedP2 = 0;
	public static int GameNeedCoin;
	public static int GameAudioVolume;
	public static bool IsFreeMode;
	public static string GameDiff = "1";
	static string FilePath = "";
	static public string FileName = "../config/XKGameConfig.xml";
	static public HandleJson HandleJsonObj = null;
	float TimeValDaoDanJingGao;
	static XKGlobalData Instance;
	public static XKGlobalData GetInstance()
	{
		if (Instance == null) {
			Instance = new XKGlobalData();
			Instance.InitInfo();
			if (!Directory.Exists(FilePath)) {
				Directory.CreateDirectory(FilePath);
			}

			if(HandleJsonObj == null) {
				HandleJsonObj = HandleJson.GetInstance();
			}

			string startCoinInfo = HandleJsonObj.ReadFromFileXml(FileName, "START_COIN");
			if (startCoinInfo == null || startCoinInfo == "") {
				startCoinInfo = "1";
				HandleJsonObj.WriteToFileXml(FileName, "START_COIN", startCoinInfo);
			}
			XKGlobalData.GameNeedCoin = Convert.ToInt32( startCoinInfo );
//			GameNeedCoin = 1;

			string modeGame = HandleJsonObj.ReadFromFileXml(FileName, "GAME_MODE");
			if(modeGame == null || modeGame == "") {
				modeGame = "1";
				HandleJsonObj.WriteToFileXml(FileName, "GAME_MODE", modeGame);
			}
			
			if(modeGame == "0") {
				IsFreeMode = true;
			}

			string gmText = HandleJsonObj.ReadFromFileXml(FileName, "GameTextVal");
			if (gmText == null || gmText == "") {
				gmText = "0"; //中文版.
				SetGameTextMode(GameTextType.Chinese);
			}
			GameTextVal = gmText == "0" ? GameTextType.Chinese : GameTextType.English;
			GameTextVal = GameTextType.English; //test.

			GetGameDiffVal();

			string gunZhenDongStr = HandleJsonObj.ReadFromFileXml(FileName, "GunZDP1");
			if(gunZhenDongStr == null || gunZhenDongStr == "") {
				gunZhenDongStr = "5";
			}
			GunZhenDongP1 = Convert.ToInt32( gunZhenDongStr );
			pcvr.SetGunZhenDongDengJi(GunZhenDongP1, PlayerEnum.PlayerOne);

			gunZhenDongStr = HandleJsonObj.ReadFromFileXml(FileName, "GunZDP2");
			if(gunZhenDongStr == null || gunZhenDongStr == "") {
				gunZhenDongStr = "5";
			}
			GunZhenDongP2 = Convert.ToInt32( gunZhenDongStr );
			pcvr.SetGunZhenDongDengJi(GunZhenDongP2, PlayerEnum.PlayerTwo);

			string dianJiSpeedStr = HandleJsonObj.ReadFromFileXml(FileName, "DianJiSpeedP1");
			if(dianJiSpeedStr == null || dianJiSpeedStr == "") {
				dianJiSpeedStr = "5";
			}
			DianJiSpeedP1 = Convert.ToInt32( dianJiSpeedStr );
			pcvr.SetPlayerZuoYiDianJiSpeed(DianJiSpeedP1, PlayerEnum.PlayerOne);
			
			dianJiSpeedStr = HandleJsonObj.ReadFromFileXml(FileName, "DianJiSpeedP2");
			if(dianJiSpeedStr == null || dianJiSpeedStr == "") {
				dianJiSpeedStr = "5";
			}
			DianJiSpeedP2 = Convert.ToInt32( dianJiSpeedStr );
			pcvr.SetPlayerZuoYiDianJiSpeed(DianJiSpeedP2, PlayerEnum.PlayerTwo);

			string val = HandleJsonObj.ReadFromFileXml(FileName, "GameAudioVolume");
			if (val == null || val == "") {
				val = "7";
				HandleJsonObj.WriteToFileXml(FileName, "GameAudioVolume", val);
			}
			GameAudioVolume = Convert.ToInt32(val);
		}
		return Instance;
	}
	
	void InitInfo()
	{
		FilePath = Application.dataPath + "/../config";
	}
	
	public static void GetGameDiffVal()
	{
		string diffStr = HandleJsonObj.ReadFromFileXml(FileName, "GAME_DIFFICULTY");
		if(diffStr == null || diffStr == "") {
			diffStr = "1";
			HandleJsonObj.WriteToFileXml(FileName, "GAME_DIFFICULTY", diffStr);
		}
		GameDiff = diffStr;
	}

	public static void SetCoinPlayerOne(int coin)
	{
		if (coin > 0 && CoinPlayerOne != coin) {
			PlayTouBiAudio();
		}
		CoinPlayerOne = coin;
		if (CoinPlayerCtrl.GetInstanceOne() != null) {
			CoinPlayerCtrl.GetInstanceOne().SetPlayerCoin(coin);
		}
		
		if (SetPanelUiRoot.GetInstance() != null) {
			SetPanelUiRoot.GetInstance().SetCoinStartLabelInfo();
		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public static void SetCoinPlayerTwo(int coin)
	{
		if (coin > 0 && CoinPlayerTwo != coin) {
			PlayTouBiAudio();
		}
		CoinPlayerTwo = coin;
		if (CoinPlayerCtrl.GetInstanceTwo()) {
			CoinPlayerCtrl.GetInstanceTwo().SetPlayerCoin(coin);
		}

		if (SetPanelUiRoot.GetInstance() != null) {
			SetPanelUiRoot.GetInstance().SetCoinStartLabelInfo();
		}
		pcvr.SetIsPlayerActivePcvr();
	}

	public static void SetCoinPlayerThree(int coin)
	{
//		if (coin > 0 && CoinPlayerThree != coin) {
//			PlayTouBiAudio();
//		}
		CoinPlayerThree = coin;
//		if (CoinPlayerCtrl.GetInstanceTwo()) {
//			CoinPlayerCtrl.GetInstanceTwo().SetPlayerCoin(coin);
//		}
//		
//		if (SetPanelUiRoot.GetInstance() != null) {
//			SetPanelUiRoot.GetInstance().SetCoinStartLabelInfo();
//		}
	}

	public static void SetCoinPlayerFour(int coin)
	{
//		if (coin > 0 && CoinPlayerFour != coin) {
//			PlayTouBiAudio();
//		}
		CoinPlayerFour = coin;
//		if (CoinPlayerCtrl.GetInstanceTwo()) {
//			CoinPlayerCtrl.GetInstanceTwo().SetPlayerCoin(coin);
//		}
//		
//		if (SetPanelUiRoot.GetInstance() != null) {
//			SetPanelUiRoot.GetInstance().SetCoinStartLabelInfo();
//		}
	}

	public static void SetGameNeedCoin(int coin)
	{
		GameNeedCoin = coin;
		CoinPlayerCtrl.GetInstanceOne().SetGameNeedCoin(coin);
		CoinPlayerCtrl.GetInstanceTwo().SetGameNeedCoin(coin);
	}

	public static void SetGameTextMode(GameTextType modeVal)
	{
		string gmText = modeVal == GameTextType.Chinese ? "0" : "1";
		//gmText == "0" -> 中文版,  gmText == "1" -> 英文版.
		HandleJsonObj.WriteToFileXml(FileName, "GameTextVal", gmText);
		GameTextVal = modeVal;
	}

	public static GameTextType GetGameTextMode()
	{
		GetInstance();
		return GameTextVal;
	}
	
	public static void PlayAudioSetMove()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASSetMove);
	}

	public static void PlayAudioSetEnter()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASSetEnter);
	}

	static void PlayTouBiAudio()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASTouBi);
	}

	public void PlayStartBtAudio()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASStartBt);
	}
	
	public void PlayModeBeiJingAudio()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASModeBeiJing, 2);
	}
	
	public void StopModeBeiJingAudio()
	{
		AudioListCtrl.StopLoopAudio(AudioListCtrl.GetInstance().ASModeBeiJing);
	}

	public void PlayModeXuanZeAudio()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASModeXuanZe);
	}
	
	public void PlayModeQueRenAudio()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASModeQueRen);
	}

	public void PlayGuanKaBeiJingAudio()
	{
		int audioIndex = Application.loadedLevel - 1;
		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			audioIndex = 1; //test
		}

		if (AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex] != null) {
			AudioSource audioVal = AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex].gameObject.AddComponent<AudioSource>();
			audioVal.clip = AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex].clip;
			audioVal.volume = AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex].volume;

			AudioListCtrl.GetInstance().RemoveAudioSource(AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex]);
			AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex] = audioVal;
		}
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex], 2);
	}

	public void PlayDaoDanJingGaoAudio()
	{
		if (Time.realtimeSinceStartup - TimeValDaoDanJingGao < 0.5f) {
			return;
		}
		TimeValDaoDanJingGao = Time.realtimeSinceStartup;
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASDaoDanJingGao, 1);
	}

	public void PlayJiaYouBaoZhaAudio()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASJiaYouBaoZha);
	}

	public void PlayAudioRanLiaoJingGao()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASRanLiaoJingGao, 2);
	}

	public void StopAudioRanLiaoJingGao()
	{
		AudioListCtrl.StopLoopAudio(AudioListCtrl.GetInstance().ASRanLiaoJingGao);
	}
	
	public void PlayAudioHitBuJiBao()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASHitBuJiBao);
	}
	
	public void PlayAudioRenWuOver()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASRenWuOver);
		MakeAudioVolumeDown();
	}
	
	public void PlayAudioGameOver()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASGameOver);
		MakeAudioVolumeDown();
	}

	void MakeAudioVolumeDown()
	{
		int loadLevelNum = Application.loadedLevel - 1;
		if (loadLevelNum < 0 || loadLevelNum > 3) {
			loadLevelNum = 0;
		}
		AudioListCtrl.StopLoopAudio(AudioListCtrl.GetInstance().ASGuanKaBJ[loadLevelNum], 1);
	}
	
	public void PlayAudioXuBiDaoJiShi()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASXuBiDaoJiShi);
	}
	
	public void PlayAudioXunZhangJB()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASXunZhangJB);
	}

	public void PlayAudioXunZhangZP()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASXunZhangZP);
	}

	public void PlayAudioJiFenGunDong()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASJiFenGunDong, 2);
	}
	
	public void StopAudioJiFenGunDong()
	{
		AudioListCtrl.StopLoopAudio(AudioListCtrl.GetInstance().ASJiFenGunDong);
	}

	public void PlayAudioZhunXingTX()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASZhunXingTX);
	}
}