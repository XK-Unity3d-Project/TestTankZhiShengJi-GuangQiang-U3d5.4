using UnityEngine;
using System.Collections;

public class YouLiangDianUICtrl : MonoBehaviour {
	public GameObject YLDUIA;
	public GameObject YLDUIB;
	public GameObject YLDUIC;
	public Transform YLDEndPosTran;
	static YouLiangDianUICtrl _Instance;
	public static YouLiangDianUICtrl GetInstance()
	{
		return _Instance;
	}
	// Use this for initialization
	void Start()
	{
		_Instance = this;
		
		YouLiangDianMoveCtrl yldScript = null;
		for (int i = 0; i < 20; i++) {
			yldScript = YouLiangDianUICtrl.GetInstance().SpawnYouLiangDianUI( YouLiangDengJi.Level_1 );
			XkGameCtrl.AddYLDLv(yldScript);
			yldScript.gameObject.SetActive(false);
			if (i < 5) {
				yldScript = YouLiangDianUICtrl.GetInstance().SpawnYouLiangDianUI( YouLiangDengJi.Level_2 );
				XkGameCtrl.AddYLDLv(yldScript);
				yldScript.gameObject.SetActive(false);
				
				yldScript = YouLiangDianUICtrl.GetInstance().SpawnYouLiangDianUI( YouLiangDengJi.Level_3 );
				XkGameCtrl.AddYLDLv(yldScript);
				yldScript.gameObject.SetActive(false);
			}
		}
	}

	public void HandleNpcYouLiangDian(int youLiangDianVal, Transform tranNpc)
	{
		if (JiFenJieMianCtrl.GetInstance() != null) {
			if (JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
				return;
			}
		}

		int randVal = Random.Range(0, 100) % 4;
		int yldLvA = 4; //min
		int yldLvB = 9; //center
		if (youLiangDianVal <= yldLvA) {
			randVal = 0;
			//激活两个玩家时使小油桶不向上飞.
			/*if (XkGameCtrl.IsActivePlayerOne && XkGameCtrl.IsActivePlayerTwo) {
				return;
			}*/
			return;
		}
		else if (youLiangDianVal > yldLvA && youLiangDianVal <= yldLvB) {
			randVal = 1;
		}
		else {
			randVal = 2;
		}
		//randVal = 1; //test

		YouLiangDianMoveCtrl scriptYLD = null;
		switch (randVal) {
		case 0:
			scriptYLD = XkGameCtrl.GetYLDMoveScript(YouLiangDengJi.Level_1);
			break;
		case 1:
			scriptYLD = XkGameCtrl.GetYLDMoveScript(YouLiangDengJi.Level_2);
			break;
		default:
			scriptYLD = XkGameCtrl.GetYLDMoveScript(YouLiangDengJi.Level_3);
			break;
		}
		
		Vector3 startPos = Vector3.zero;
		startPos = Camera.main.WorldToScreenPoint(tranNpc.position);
		startPos.z = 0f;
		if (!GameMovieCtrl.IsThreeScreenGame) {
			startPos.x = (XkGameCtrl.ScreenWidth * startPos.x) / Screen.width;
			startPos.y = (XkGameCtrl.ScreenHeight * startPos.y) / Screen.height;
		}
		else {
			startPos.x = (XkGameCtrl.ScreenWidth3 * startPos.x) / Screen.width;
			startPos.y = (XkGameCtrl.ScreenHeight3 * startPos.y) / Screen.height;
		}
		//Debug.Log("startPos **** "+startPos);
		scriptYLD.StartMoveYouLiangDian(startPos, YLDEndPosTran.position);
	}

	public YouLiangDianMoveCtrl SpawnYouLiangDianUI(YouLiangDengJi levelVal)
	{
		GameObject obj = null;
		Transform tran = null;
		switch (levelVal) {
		case YouLiangDengJi.Level_1:
			obj = (GameObject)Instantiate(YLDUIA);
			tran = obj.transform;
			break;

		case YouLiangDengJi.Level_2:
			obj = (GameObject)Instantiate(YLDUIB);
			tran = obj.transform;
			break;

		case YouLiangDengJi.Level_3:
			obj = (GameObject)Instantiate(YLDUIC);
			tran = obj.transform;
			break;
		}

		tran.parent = transform;
		tran.localPosition = Vector3.zero;
		YouLiangDianMoveCtrl script = obj.GetComponent<YouLiangDianMoveCtrl>();
		return script;
	}
}