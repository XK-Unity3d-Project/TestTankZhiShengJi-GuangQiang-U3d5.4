using UnityEngine;
using System.Collections;

public class GameOverCtrl : MonoBehaviour {
	GameObject GameOverObj;
	public static bool IsShowGameOver;
	static GameOverCtrl Instance;
	public static GameOverCtrl GetInstance()
	{
		return Instance;
	}

	// Use this for initialization
	void Start()
	{
		Instance = this;
		IsShowGameOver = false;
		GameOverObj = gameObject;
		GameOverObj.SetActive(false);
	}

	public void ShowGameOver()
	{
		if (IsShowGameOver) {
			return;
		}
		IsShowGameOver = true;
		XKGlobalData.GetInstance().PlayAudioGameOver();
		GameOverObj.SetActive(true);
		Invoke("HiddenGameOver", 3f);
		MakeServerShowGameOver();
	}

	void HiddenGameOver()
	{
		GameOverObj.SetActive(false);
		//XkGameCtrl.LoadingGameMovie(); //Back Movie Scene.
		JiFenJieMianCtrl.GetInstance().ActiveJiFenJieMian();
	}

	void MakeServerShowGameOver()
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			return;
		}
		
		if (NetCtrl.GetInstance() != null) {
			NetCtrl.GetInstance().MakeServerShowGameOver();
		}
	}
}

public enum GameLevel
{
	None = -1,
	Movie,
	Scene_1,
	Scene_2,
	Scene_3,
	Scene_4,
	SetPanel,
}