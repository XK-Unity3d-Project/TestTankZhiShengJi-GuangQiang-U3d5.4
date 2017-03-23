using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum YouLiangDengJi
{
	Level_1,
	Level_2,
	Level_3,
}

public class YouLiangDianMoveCtrl : MonoBehaviour {
	public YouLiangDengJi LevelVal = YouLiangDengJi.Level_1;
	void Awake()
	{
		UITexture textureUICom = GetComponent<UITexture>();
		if (textureUICom != null) {
			textureUICom.depth = -2;
		}
	}

	public void StartMoveYouLiangDian(Vector3 startPos, Vector3 endPos)
	{
		if (!gameObject.activeSelf) {
			gameObject.SetActive(true);
		}
		Vector3[] nodes = new Vector3[2];
		transform.localScale = new Vector3(1f, 1f, 1f);
		transform.localPosition = startPos;
		nodes[0] = transform.position;
		nodes[1] = endPos;
		float disVal = Vector3.Distance(nodes[0], nodes[1]);
		float speedVal = disVal / 0.2f;
		iTween.MoveTo(gameObject, iTween.Hash("path", nodes,
		                                      "speed", speedVal,
		                                        "orienttopath", false,
		                                        "looptype", iTween.LoopType.none,
		                                        "easeType", iTween.EaseType.linear,
		                                      	"oncomplete", "MoveYouLiangDianOnCompelte"));
	}

	void MoveYouLiangDianOnCompelte()
	{
		//Debug.Log("MoveYouLiangDianOnCompelte...");
		YouLiangAddCtrl.GetInstance().ShowYouLiangGuangObj();
		gameObject.SetActive(false);
	}
}