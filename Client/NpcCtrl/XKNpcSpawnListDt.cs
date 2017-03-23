using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NpcScriptState
{
	Null,
	XKNpcMove,
	XKDaPao,
	XKHuoChe,
	XKFangZhen,
}

public class XKNpcSpawnListDt : MonoBehaviour {
	public string NpcPrefabName;
	public NpcScriptState NpcScriptSt = NpcScriptState.Null;
	public List<GameObject> NpcList = new List<GameObject>(10);
	public GameObject FindNpcObjFromNpcList(GameObject npcPrefab)
	{
		if (npcPrefab == null) {
			return null;
		}

		GameObject npcObj = null;
		int max = NpcList.Count;
		XKNpcMoveCtrl npcMoveScript = null;
		XKDaPaoCtrl npcDaPaoScript = null;
		XKHuoCheCtrl npcHuoCheScript = null;
		XKNpcFangZhenCtrl npcFangZhenScript = null;
		if (max > 0 && NpcScriptSt != NpcScriptState.XKHuoChe) {
			for (int i = 0; i < max; i++) {
				if (NpcList[i] != null) {
					switch (NpcScriptSt) {
					case NpcScriptState.XKNpcMove:
						npcMoveScript = NpcList[i].GetComponent<XKNpcMoveCtrl>();
						if (npcMoveScript != null
						    && npcMoveScript.GetIsDeathNPC()) {
							GameObject realNpcObj = npcMoveScript.RealNpcTran.gameObject;
							if (!realNpcObj.activeSelf) {
								npcObj = NpcList[i];
							}
						}
						break;

					case NpcScriptState.XKDaPao:
						npcDaPaoScript = NpcList[i].GetComponent<XKDaPaoCtrl>();
						if (npcDaPaoScript != null
						    && npcDaPaoScript.GetIsDeathNpc()) {
							npcDaPaoScript.ResetNpcDaPaoInfo();
							npcObj = NpcList[i];
						}
						break;
						
					case NpcScriptState.XKHuoChe:
						break;
						
					case NpcScriptState.XKFangZhen:
						npcFangZhenScript = NpcList[i].GetComponent<XKNpcFangZhenCtrl>();
						if (npcFangZhenScript != null
						    && npcFangZhenScript.GetIsHiddenNpcObj()
						    && !npcFangZhenScript.GetIsActiveFangZhen()) {
							npcObj = NpcList[i];
							npcFangZhenScript.ActiveFangZhenNpc();
						}
						break;
					}
				}

				if (npcObj != null) {
					break;
				}
			}
		}

		if (npcObj == null) {
			if (NpcScriptSt == NpcScriptState.Null) {
				npcMoveScript = npcPrefab.GetComponent<XKNpcMoveCtrl>();
				if (npcMoveScript != null) {
					NpcScriptSt = NpcScriptState.XKNpcMove;
				}

				if (NpcScriptSt == NpcScriptState.Null) {
					npcDaPaoScript = npcPrefab.GetComponent<XKDaPaoCtrl>();
					if (npcDaPaoScript != null) {
						NpcScriptSt = NpcScriptState.XKDaPao;
					}
				}

				if (NpcScriptSt == NpcScriptState.Null) {
					npcHuoCheScript = npcPrefab.GetComponent<XKHuoCheCtrl>();
					if (npcHuoCheScript != null) {
						NpcScriptSt = NpcScriptState.XKHuoChe;
					}
				}

				if (NpcScriptSt == NpcScriptState.Null) {
					npcFangZhenScript = npcPrefab.GetComponent<XKNpcFangZhenCtrl>();
					if (npcFangZhenScript != null) {
						NpcScriptSt = NpcScriptState.XKFangZhen;
					}
				}
			}

			switch (NpcScriptSt) {
			case NpcScriptState.XKNpcMove:
				npcObj = SpawnNpcByNpcPrefab(npcPrefab);
				break;

			case NpcScriptState.XKDaPao:
				npcObj = SpawnNpcByNpcPrefab(npcPrefab);
				break;
				
			case NpcScriptState.XKHuoChe:
				npcObj = SpawnNpcByNpcPrefab(npcPrefab);
				break;
				
			case NpcScriptState.XKFangZhen:
				npcObj = SpawnNpcByNpcPrefab(npcPrefab);
				break;
			}
		}
		return npcObj;
	}

	GameObject SpawnNpcByNpcPrefab(GameObject npcPrefab)
	{
		if (NpcPrefabName == "" || NpcPrefabName == null) {
			NpcPrefabName = npcPrefab.name;
		}

		GameObject obj = null;
		Vector3 posVal = new Vector3(-18000f, -18000f, 0f);
		Quaternion rotVal = Quaternion.Euler(Vector3.zero);
		if (Network.peerType == NetworkPeerType.Disconnected) {
			obj = (GameObject)Instantiate(npcPrefab, posVal, rotVal);
		}
		else {
			int playerID = int.Parse(Network.player.ToString());
			obj = (GameObject)Network.Instantiate(npcPrefab, posVal, rotVal, playerID);
			if (NetworkServerNet.GetInstance() != null) {
				NetworkServerNet.GetInstance().AddNpcObjList(obj);
			}
		}
		HandleNpcList(obj);
		return obj;
	}

	void HandleNpcList(GameObject npcObj)
	{
		if (npcObj == null || NpcList.Contains(npcObj)) {
			return;
		}
		NpcList.Add(npcObj);
	}

	public void CheckRemoveNpcSpawnListDt(GameObject npcObj)
	{
		if (npcObj == null) {
			return;
		}

		int max = NpcList.Count;
		for (int i = 0; i < max; i++) {
			if (NpcList[i] == npcObj) {
				//Debug.Log("CheckRemoveNpcSpawnListDt -> npcObj "+npcObj.name);
				NpcList.RemoveAt(i);
				break;
			}
		}
	}
}