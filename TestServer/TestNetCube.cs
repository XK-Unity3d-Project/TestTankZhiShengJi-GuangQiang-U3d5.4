using UnityEngine;
using System.Collections;
using System;

public class TestNetCube : MonoBehaviour {
	NetworkView NetworkViewCom;
	void Start()
	{
		NetworkViewCom = GetComponent<NetworkView>();
	}

	// Update is called once per frame
	void Update()
	{
		if(NetworkViewCom.isMine && Network.isServer) {
			//ScreenLog.Log("SendServerLevelCur -> level " + Application.loadedLevel);
			if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0) {
				return;
			}

			NetworkViewCom.RPC("TestNetCubeSendServerLevelCur", RPCMode.OthersBuffered, Application.loadedLevel);
		}
		else {
			if (Network.peerType == NetworkPeerType.Disconnected) {
				Network.Destroy(gameObject);
			}
		}
	}

	[RPC] void TestNetCubeSendServerLevelCur(int level)
	{
	}
}
