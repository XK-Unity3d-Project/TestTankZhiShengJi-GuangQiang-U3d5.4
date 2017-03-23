using UnityEngine;
using System.Collections;

public class XKDaPaoCtrl : MonoBehaviour {
	public GameObject TestSpawnPoint;
	NetworkView NetViewCom;
	XKSpawnNpcPoint SpawnPointScript;
	XKNpcMoveCtrl NpcMoveScript;
	XKCannonCtrl[] CannonScript;
	bool IsDeathNpc;
	bool IsHuoCheNpc;
	bool IsHandleRpc;
	void Awake()
	{
		NpcMoveScript = GetComponentInParent<XKNpcMoveCtrl>();
		NetViewCom = GetComponent<NetworkView>();
		if (Network.peerType == NetworkPeerType.Disconnected) {
			if (NetViewCom != null) {
				NetViewCom.enabled = false;
			}
		}
	}

	void Start()
	{
		if (CannonScript == null || CannonScript.Length < 1) {
			CannonScript = gameObject.GetComponentsInChildren<XKCannonCtrl>();
		}

		//Debug.Log("name "+gameObject.name+", IsHuoCheNpc "+IsHuoCheNpc);
		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			if (TestSpawnPoint != null) {
				XKSpawnNpcPoint spawnPoint = TestSpawnPoint.GetComponent<XKSpawnNpcPoint>();
				if (spawnPoint != null) {
					SetSpawnPointScript(spawnPoint);
				}
			}
		}
		Invoke("DelayChangeNpcParent", 0.2f);
	}

	void DelayChangeNpcParent()
	{
//		Debug.Log("DelayChangeNpcParent -> IsHuoCheNpc "+IsHuoCheNpc+", name "+gameObject.name);
		if (!IsHuoCheNpc) {
			if (transform.parent == null) {
				transform.parent = XkGameCtrl.MissionCleanup;
			}
		}
	}

	public void SetSpawnPointScript(XKSpawnNpcPoint script)
	{
		IsHandleRpc = true;
		SpawnPointScript = script;
		IsHuoCheNpc = SpawnPointScript.GetIsHuoCheNpc();
//		Debug.Log("SetSpawnPointScript -> IsHuoCheNpc "+IsHuoCheNpc);
		TestSpawnPoint = script.gameObject;
		
		if (CannonScript == null || CannonScript.Length < 1) {
			CannonScript = gameObject.GetComponentsInChildren<XKCannonCtrl>();
		}

		if (CannonScript.Length > 0) {
			int max = CannonScript.Length;
//			Debug.Log("SetSpawnPointScript -> max "+max);
			for (int i = 0; i < max; i++) {
				CannonScript[i].SetSpawnPointScript(this);
			}
			SetCannonAimPlayerState();
		}
		SendNpcTransformInfo();
	}
	
	void SetCannonAimPlayerState()
	{
		int aimState = -1;
		if (SpawnPointScript.PointType == SpawnPointType.KongZhong) {
			aimState = 0;
		}
		else if (SpawnPointScript.PointType == SpawnPointType.DiMian) {
			aimState = 1;
		}
		else {
			if (SpawnPointScript.IsAimFeiJiPlayer) {
				aimState = 2;
			}
		}
		SetCannonNpcInfo(aimState, SpawnPointScript.FireDistance);
		
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		
		if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0) {
			return;
		}
		
		if (NetViewCom == null) {
			NetViewCom = GetComponent<NetworkView>();
		}

		if (NetViewCom != null) {
			NetViewCom.RPC("XKCannonSendSetNpcAimPlayerState", RPCMode.OthersBuffered, aimState, SpawnPointScript.FireDistance);
		}
	}
	
	[RPC] void XKCannonSendSetNpcAimPlayerState(int valAim, float valFireDis)
	{
		//Debug.Log("XKCannonSendSetNpcAimPlayerState.............");
		SetCannonNpcInfo(valAim, valFireDis);
	}
	
	void SetCannonNpcInfo(int valAim, float valFireDis)
	{
		if (CannonScript == null || CannonScript.Length < 1) {
			CannonScript = gameObject.GetComponentsInChildren<XKCannonCtrl>();
		}

		if (CannonScript.Length > 0) {
			int max = CannonScript.Length;
			for (int i = 0; i < max; i++) {
				CannonScript[i].SetCannonSpawnPointInfo(valAim, valFireDis);
			}
		}
	}

	public void OnRemoveCannon(PlayerEnum playerSt, int key, float timeVal = 0f)
	{
		if (IsDeathNpc) {
			return;
		}
		IsDeathNpc = true;
		
		//XkGameCtrl.ClearNpcSpawnAllAmmo(gameObject);
		XkGameCtrl.GetInstance().RemoveNpcTranFromList(transform);
		if (Network.peerType != NetworkPeerType.Disconnected) {
			HandleNetDaoPaoRemove(key);
		}
	}

	void HandleNetDaoPaoRemove(int key)
	{
		if (key == 1) {
			if (CannonScript == null || CannonScript.Length < 1) {
				CannonScript = gameObject.GetComponentsInChildren<XKCannonCtrl>();
			}
			
			if (CannonScript.Length > 0) {
				int max = CannonScript.Length;
				for (int i = 0; i < max; i++) {
					CannonScript[i].CallOtherPortDeath();
				}
			}
		}

		if (NpcMoveScript == null) {
			transform.position = new Vector3(-18000f, -18000f, 0f);
			transform.eulerAngles = Vector3.zero;
		}

		if (NetViewCom != null) {
			NetViewCom.RPC("XKDaPaoSendRemoveObj", RPCMode.OthersBuffered, key);
		}
	}

	[RPC] void XKDaPaoSendRemoveObj(int key)
	{
		if (key == 1) {
			if (CannonScript == null || CannonScript.Length < 1) {
				CannonScript = gameObject.GetComponentsInChildren<XKCannonCtrl>();
			}

			if (CannonScript.Length > 0) {
				int max = CannonScript.Length;
				for (int i = 0; i < max; i++) {
					CannonScript[i].CallOtherPortDeath();
				}
			}
		}

		if (NpcMoveScript == null) {
			transform.position = new Vector3(-18000f, -18000f, 0f);
			transform.eulerAngles = Vector3.zero;
		}
		IsDeathNpc = true;
	}

	void SendNpcTransformInfo()
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		
		if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0) {
			return;
		}
		
		if (!IsHandleRpc) {
			return;
		}

		if (NetViewCom != null) {
			NetViewCom.RPC("XKDaPaoSendOtherTranformInfo", RPCMode.OthersBuffered, transform.position, transform.rotation);
		}
	}
	
	[RPC] void XKDaPaoSendOtherTranformInfo(Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;
		ResetNpcDaPaoInfo();
	}

	public void SetHuoCheNpcInfo(int indexPoint)
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}
		
		if (Network.connections.Length <= 0 || NetworkServerNet.ServerSendState != 0) {
			return;
		}

		if (NetViewCom != null) {
			NetViewCom.RPC("XKDaPaoSendSetHuoCheNpcInfo", RPCMode.OthersBuffered, indexPoint);
		}
	}

	[RPC] void XKDaPaoSendSetHuoCheNpcInfo(int indexPoint)
	{
		StartCoroutine(DelaySetHuoCheNpcInfo(indexPoint));
	}

	IEnumerator DelaySetHuoCheNpcInfo(int indexPoint)
	{
		yield return new WaitForSeconds(0.5f);
//		Debug.Log("XKDaPaoSendSetHuoCheNpcInfo -> indexPoint "+indexPoint+", name "+transform.name);
		if (Network.peerType == NetworkPeerType.Server) {
			yield break;
		}
		
		XKHuoCheCtrl huoCheScript = XKHuoCheCtrl.Instance;
		if (huoCheScript == null) {
			yield break;
		}
		
		XKSpawnNpcPoint[] pointScript = huoCheScript.gameObject.GetComponentsInChildren<XKSpawnNpcPoint>();
//		Debug.Log("XKDaPaoSendSetHuoCheNpcInfo -> pointScript.Len "+pointScript.Length);
		if (pointScript.Length > 0) {
			for (int i = 0; i < pointScript.Length; i++) {
				if (i == indexPoint) {
					IsHuoCheNpc = true;
//					Debug.Log("XKDaPaoSendSetHuoCheNpcInfo... IsHuoCheNpc "+IsHuoCheNpc+", parentName "
//					          +pointScript[i].transform.parent.name);
					CancelInvoke("DelayChangeNpcParent");
					transform.parent = pointScript[i].transform.parent;
					transform.localPosition = pointScript[i].transform.localPosition;
					transform.localEulerAngles = pointScript[i].transform.localEulerAngles;
					yield break;
				}
			}
		}
	}

	public bool GetIsDeathNpc()
	{
		return IsDeathNpc;
	}

	public void ResetNpcDaPaoInfo()
	{
		//Debug.Log("ResetNpcDaPaoInfo -> npcObj "+gameObject.name+", npcId "+NpcId);
		IsDeathNpc = false;
	}
}