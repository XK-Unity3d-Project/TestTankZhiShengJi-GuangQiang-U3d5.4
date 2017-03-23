using UnityEngine;
using System.Collections;

public class DaoJiShiCtrl : MonoBehaviour {
	public GameObject ContinueGameObj;
	GameObject DaoJiShiObj;
	UISprite DaoJiShiSprite;
	bool IsPlayDaoJishi;
	int DaoJiShiCount = 9;
	public static bool IsActivePlayerOne;
	public static bool IsActivePlayerTwo;
	public static bool IsTestActivePlayer = false;
	static DaoJiShiCtrl Instance;
	public static DaoJiShiCtrl GetInstance()
	{
		return Instance;
	}

	// Use this for initialization
	void Start()
	{
		IsActivePlayerOne = false;
		IsActivePlayerTwo = false;
		DaoJiShiObj = gameObject;
		DaoJiShiSprite = GetComponent<UISprite>();
		Instance = this;
		DaoJiShiObj.SetActive(false);
		ContinueGameObj.SetActive(false);
	}

	public void StartPlayDaoJiShi()
	{
		if (IsPlayDaoJishi) {
			return;
		}
		IsPlayDaoJishi = true;
		DaoJiShiCount = 9;
		DaoJiShiSprite.spriteName = "daoJiShi9";
		DaoJiShiObj.SetActive(true);
		ContinueGameObj.SetActive(true);
		ShowDaoJiShiInfo();
		XKGlobalData.GetInstance().StopAudioRanLiaoJingGao();
		YouLiangCtrl.GetInstance().SetActiveYouLiangFlash(false);

		IsActivePlayerOne = XkGameCtrl.IsActivePlayerOne;
		IsActivePlayerTwo = XkGameCtrl.IsActivePlayerTwo;
	}

	public void StopDaoJiShi()
	{
		if (!IsPlayDaoJishi) {
			return;
		}
		IsPlayDaoJishi = false;

		TweenScale tweenScaleCom = GetComponent<TweenScale>();
		if (tweenScaleCom != null) {
			tweenScaleCom.enabled = false;
			DestroyObject(tweenScaleCom);
		}
		ContinueGameObj.SetActive(false);
		DaoJiShiObj.SetActive(false);
	}

	void ShowDaoJiShiInfo()
	{
		XKGlobalData.GetInstance().PlayAudioXuBiDaoJiShi();
		TweenScale tweenScaleCom = GetComponent<TweenScale>();
		if (tweenScaleCom != null) {
			DestroyObject(tweenScaleCom);
		}

		tweenScaleCom = DaoJiShiObj.AddComponent<TweenScale>();
		tweenScaleCom.enabled = false;
		tweenScaleCom.duration = 1.2f;
		tweenScaleCom.from = new Vector3(3f, 3f, 1f);
		tweenScaleCom.to = new Vector3(1f, 1f, 1f);
		EventDelegate.Add(tweenScaleCom.onFinished, delegate{
			ChangeDaoJiShiVal();
		});
		tweenScaleCom.enabled = true;
		tweenScaleCom.PlayForward();
	}

	void ChangeDaoJiShiVal()
	{
		if (JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
			StopDaoJiShi();
			return;
		}

		if (DaoJiShiCount <= 1) {
			StopDaoJiShi();
			GameOverCtrl.GetInstance().ShowGameOver();
			return;
		}

		DaoJiShiCount--;
		DaoJiShiSprite.spriteName = "daoJiShi" + DaoJiShiCount;
		ShowDaoJiShiInfo();

		if (!pcvr.bIsHardWare && DaoJiShiCount == 1 && IsTestActivePlayer) {
			XkGameCtrl.SetActivePlayerOne(true);
		}
	}

	public bool GetIsPlayDaoJishi()
	{
		return IsPlayDaoJishi;
	}
}