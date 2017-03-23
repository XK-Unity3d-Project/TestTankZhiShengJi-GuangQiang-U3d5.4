using UnityEngine;
using System.Collections;

public class XKTriggerBuJiBaoOpen : MonoBehaviour {

	public GameObject BuJiBaoA;
	public Transform BuJiBaoPointA;
	[Range(0.1f, 10f)] public float TimeBuJiA = 1f;
	public GameObject BuJiBaoB;
	public Transform BuJiBaoPointB;
	[Range(0.1f, 10f)] public float TimeBuJiB = 1f;
	bool IsActiveTrigger;
	public AiPathCtrl TestPlayerPath;
	void Start()
	{
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
	}

	void OnTriggerEnter(Collider other)
	{
		if (Network.peerType != NetworkPeerType.Disconnected) {
			if (Network.peerType == NetworkPeerType.Client) {
				return;
			}
		}

		if (other.GetComponent<XkPlayerCtrl>() == null) {
			return;
		}

		if (IsActiveTrigger) {
			return;
		}
		IsActiveTrigger = true;
		StartCoroutine(SpawnBuJiBaoToPlayer());
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

	IEnumerator SpawnBuJiBaoToPlayer()
	{
		if (BuJiBaoA == null && BuJiBaoB == null) {
			yield break;
		}

		do {
			if (BuJiBaoA != null) {
				SpawnPointDaoJu(BuJiBaoA, BuJiBaoPointA.position, BuJiBaoPointA.rotation); //Spawn BuJiA
				yield return new WaitForSeconds(TimeBuJiA);
			}
			
			if (BuJiBaoB != null) {
				SpawnPointDaoJu(BuJiBaoB, BuJiBaoPointB.position, BuJiBaoPointB.rotation); //Spawn BuJiB
				yield return new WaitForSeconds(TimeBuJiB);
			}
		} while (true);
	}

	void SpawnPointDaoJu(GameObject objPrefab, Vector3 pos, Quaternion rot)
	{
		GameObject obj = null;
		if (Network.peerType == NetworkPeerType.Disconnected) {
			obj = (GameObject)Instantiate(objPrefab, pos, rot);
		}
		else {
			int playerID = int.Parse(Network.player.ToString());
			obj = (GameObject)Network.Instantiate(objPrefab, pos, rot, playerID);
			if (NetworkServerNet.GetInstance() != null) {
				NetworkServerNet.GetInstance().AddNpcObjList(obj);
			}
		}
		obj.transform.parent = XkGameCtrl.MissionCleanup;
	}

	public bool CloseSpawnBuJiBaoToPlayer()
	{
		if (!IsActiveTrigger) {
			return false;
		}
		StopCoroutine(SpawnBuJiBaoToPlayer());
		gameObject.SetActive(false);
		return true;
	}
}