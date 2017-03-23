using UnityEngine;
using System.Collections;

public class XKTriggerOpenPlayerUI : MonoBehaviour
{
	public AiMark AiMarkCom;
	static AiMark AiMarkComStatic;
	public static bool IsActiveOpenPlayerUI;
	public AiPathCtrl TestPlayerPath;
	void Start()
	{
		IsActiveOpenPlayerUI = false;
		AiMarkComStatic = AiMarkCom;
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

	void OnTriggerEnter(Collider other)
	{
		if (Network.peerType != NetworkPeerType.Disconnected) {
			if (Network.peerType == NetworkPeerType.Server) {
				return;
			}
		}
		
		XkPlayerCtrl playerScript = other.GetComponent<XkPlayerCtrl>();
		if (playerScript == null) {
			return;
		}

		HandleHeTiPlayerEvent();
		if (Network.peerType != NetworkPeerType.Disconnected) {
			NetCtrl.GetInstance().HandleHeTiPlayerEvent();
		}
		gameObject.SetActive(false);
	}

	public static void HandleHeTiPlayerEvent()
	{
		if (IsActiveOpenPlayerUI) {
			return;
		}
		IsActiveOpenPlayerUI = true;

		if (Network.peerType == NetworkPeerType.Disconnected
		    || Network.peerType == NetworkPeerType.Client) {
			ScreenDanHeiCtrl.GetInstance().OpenScreenDanHui(2);
			XKTriggerClosePlayerUI.ResetIsClosePlayerUI();
		}

		XKPlayerHeTiData.GetInstance().ShowHeTiPlayerObj();
		XkPlayerCtrl.GetInstanceCartoon().RestartMovePlayer(1);
		XkPlayerCtrl.GetInstanceCartoon().MakePlayerMoveToAiMark(AiMarkComStatic);
		XkPlayerCtrl.GetInstanceCartoon().SetPlayerCameraTran();
	}
}