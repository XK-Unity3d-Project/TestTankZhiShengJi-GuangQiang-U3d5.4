using UnityEngine;
using System.Collections;

public class XKTriggerHuoJianDan : MonoBehaviour {

//	[Range(1, 100)] public int AmmoNum = 1;
//	public Transform AimPointTran;
//	[Range(0.1f, 50f)] public float TimeUnit = 0.3f;
//	bool IsPanXuanPath = false;
//	[Range(1, 100)] public int KillNpcMin = 1;
//	public AiPathCtrl TestPlayerPath;
//	int KillCount;
//	bool IsActiveTrigger;
//	public static bool IsActivePanXuanPath;
//	int PanXuanCount;
//	static XKTriggerHuoJianDan Instance;
//	public static XKTriggerHuoJianDan GetInstance()
//	{
//		return Instance;
//	}
//	
//	void Start()
//	{
//		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
//	}
//
//	XkPlayerCtrl ScriptPlayer;
//	void OnTriggerEnter(Collider other)
//	{
//		ScriptPlayer = other.GetComponent<XkPlayerCtrl>();
//		if (ScriptPlayer == null) {
//			return;
//		}
//		//Debug.Log("XKTriggerHuoJianDan::OnTriggerEnter -> hit "+other.name);
//		if (!IsPanXuanPath) {
//			StartCoroutine(SpawnHuoJianDan(ScriptPlayer));
//		}
//		else {
//			ActivePanXuanPathInfo();
//			PanXuanCount++;
//		}
//	}
//	
//	void OnDrawGizmosSelected()
//	{
//		if (!enabled) {
//			return;
//		}
//		
//		if (TestPlayerPath != null) {
//			TestPlayerPath.DrawPath();
//		}
//	}
//
//	IEnumerator SpawnHuoJianDan(XkPlayerCtrl script)
//	{
//		int count = 0;
//		bool IsSpawnObj = true;
//		do {
//			script.SpawnPlayerHuoJianDan(AimPointTran);
//			count++;
//			//Debug.Log("SpawnHuoJianDan -> count "+count);
//			if (count >= AmmoNum) {
//				IsSpawnObj = false;
//				break;
//			}
//			yield return new WaitForSeconds(TimeUnit);
//		} while (IsSpawnObj);
//
//		if (IsPanXuanPath) {
//			gameObject.SetActive(false);
//		}
//		yield break;
//	}
//	
//	void ActivePanXuanPathInfo()
//	{
//		if (!IsPanXuanPath || IsActivePanXuanPath || IsActiveTrigger) {
//			return;
//		}
//		//Debug.Log("ActivePanXuanPathInfo...");
//		IsActivePanXuanPath = true;
//		IsActiveTrigger = true;
//		Instance = this;
//	}
//
//	public void AddKillNpcNum()
//	{
//		KillCount++;
//		if (KillCount >= KillNpcMin) {
//			//Debug.Log("AddKillNpcNum -> KillCount "+KillCount);
//			StartCoroutine(SpawnHuoJianDan(ScriptPlayer));
//			HuoLiZhiYuanCtrl.GetInstance().SetIsActive(true);
//			ResetPanXuanPathInfo();
//		}
//	}
//
//	void ResetPanXuanPathInfo()
//	{
//		if (!IsPanXuanPath) {
//			return;
//		}
//		IsActivePanXuanPath = false;
//		Instance = null;
//	}
}