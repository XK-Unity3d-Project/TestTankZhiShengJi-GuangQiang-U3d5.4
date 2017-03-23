using UnityEngine;
using System.Collections;

public class ZhunXingTeXiaoCtrl : MonoBehaviour {
	public GameObject ZhunXingParticle;
	public GameObject[] ZhunXingParticleArray;
	public GameObject[] ZhunXingArray;
	public static bool IsOverTeXiaoZhunXing;
	bool IsShowZhunXingTX;
	static ZhunXingTeXiaoCtrl Instance;
	public static ZhunXingTeXiaoCtrl GetInstance()
	{
		return Instance;
	}

	// Use this for initialization
	void Awake()
	{
		Instance = this;
		IsOverTeXiaoZhunXing = false;
		ZhunXingParticleArray[0].SetActive(false);
		ZhunXingParticleArray[1].SetActive(false);
		gameObject.SetActive(false);
		ZhunXingParticle.SetActive(false);
	}

	public void ShowZhunXingTeXiao()
	{
		if (gameObject.activeSelf || IsShowZhunXingTX) {
			return;
		}
		IsShowZhunXingTX = true;

		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			IsOverTeXiaoZhunXing = true;
			return;
		}

		Debug.Log("ShowZhunXingTeXiao...");
		if (!XkGameCtrl.IsActivePlayerOne) {
			ZhunXingArray[0].SetActive(false);
			ZhunXingArray[1].SetActive(false);
		}
		
		if (!XkGameCtrl.IsActivePlayerTwo) {
			ZhunXingArray[2].SetActive(false);
			ZhunXingArray[3].SetActive(false);
		}
		gameObject.SetActive(true);
		XKGlobalData.GetInstance().PlayAudioZhunXingTX();
	}

	public void ShowZhunXingParticle()
	{
		if (ZhunXingParticle.activeSelf) {
			return;
		}
		ZhunXingParticle.SetActive(true);

		if (XkGameCtrl.IsActivePlayerOne) {
			ZhunXingParticleArray[0].SetActive(true);
		}

		if (XkGameCtrl.IsActivePlayerTwo) {
			ZhunXingParticleArray[1].SetActive(true);
		}
		Invoke("DelayActiveGameUiCamera", 1f);
	}

	void DelayActiveGameUiCamera()
	{
		IsOverTeXiaoZhunXing = true;
		gameObject.SetActive(false);
		ScreenDanHeiCtrl.GetInstance().ActiveGameUiCamera();
		pcvr.OpenDongGanState();
	}
}