using UnityEngine;
using System.Collections;

public class XKTriggerMoveToAiMark : MonoBehaviour {
	public AiMark MarkCom;
	public AiPathCtrl SelectAiPath;
	public AiPathCtrl TestPlayerPath;
	// Use this for initialization
	void Start()
	{
		if (MarkCom == null) {
			Debug.LogWarning("MarkCom was null");
			GameObject obj = null;
			obj.name = "null";
		}
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
	}

	void OnTriggerEnter(Collider other)
	{
		if (ScreenDanHeiCtrl.IsStartGame) {
			return;
		}

		XkPlayerCtrl playerScript = other.GetComponent<XkPlayerCtrl>();
		if (playerScript == null) {
			return;
		}
		//Debug.Log("XKTriggerSpawnNpc::OnTriggerEnter -> hit "+other.name);
		ScreenDanHeiCtrl.GetInstance().OpenScreenDanHui();
		playerScript.MakePlayerMoveToAiMark(MarkCom);
		gameObject.SetActive(false);
	}

	void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}
		
		if (TestPlayerPath != null) {
			TestPlayerPath.DrawPath();
		}
	}
}
