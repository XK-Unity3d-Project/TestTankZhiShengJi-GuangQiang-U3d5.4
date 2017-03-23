using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class YouLiangMarkCtrl : MonoBehaviour {
	public PlayerEnum PlayerSt = PlayerEnum.Null;
	public Transform MarkPath;
	public GameObject ExplodeObj;
	GameObject YouLiangMark;
	static YouLiangMarkCtrl _Instance;
	public static YouLiangMarkCtrl GetInstance()
	{
		return _Instance;
	}
	
	static YouLiangMarkCtrl _InstanceP1;
	public static YouLiangMarkCtrl GetInstanceP1()
	{
		return _InstanceP1;
	}

	static YouLiangMarkCtrl _InstanceP2;
	public static YouLiangMarkCtrl GetInstanceP2()
	{
		return _InstanceP2;
	}
	// Use this for initialization
	void Start()
	{
		switch (PlayerSt) {
		case PlayerEnum.Null:
			_Instance = this;
			break;
		case PlayerEnum.PlayerOne:
			_InstanceP1 = this;
			break;
		case PlayerEnum.PlayerTwo:
			_InstanceP2 = this;
			break;
		}
		YouLiangMark = gameObject;
		YouLiangMark.SetActive(false);
	}

	public void MoveYouLiangMark()
	{
		if (YouLiangMark.activeSelf) {
			return;
		}
		YouLiangMark.transform.position = MarkPath.GetChild(0).position;
		YouLiangMark.SetActive(true);
		List<Transform> nodes = new List<Transform>(MarkPath.GetComponentsInChildren<Transform>()){};
		nodes.Remove(MarkPath);
		iTween.MoveTo(YouLiangMark, iTween.Hash("path", nodes.ToArray(),
		                                        "time", 1.5f,
		                                        "orienttopath", false,
		                                        "looptype", iTween.LoopType.none,
		                                        "easeType", iTween.EaseType.linear,
		                                        "oncomplete", "MoveYouLiangMarkOnCompelteITween"));
	}
	
	void MoveYouLiangMarkOnCompelteITween()
	{
//		Debug.Log("MoveYouLiangMarkOnCompelteITween...");
		YouLiangMark.SetActive(false);
		XKGlobalData.GetInstance().PlayJiaYouBaoZhaAudio();
		GameObject obj = (GameObject)Instantiate(ExplodeObj, YouLiangMark.transform.position, YouLiangMark.transform.rotation);
		XkGameCtrl.CheckObjDestroyThisTimed(obj);
		YouLiangAddCtrl.GetInstance().AddPlayerYouLiangDian(PlayerSt);
	}
}