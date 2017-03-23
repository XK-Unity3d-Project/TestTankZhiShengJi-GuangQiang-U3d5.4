using UnityEngine;
using System.Collections;

public class XKTriggerPlayerAim : MonoBehaviour {
	public XKSpawnNpcPoint[] SpawnPoint; //主角瞄准的对象.
	[Range(0.001f, 100f)]public float AimSpeed = 0.1f;
	[Range(0.001f, 100f)]public float LeaveSpeed = 0.1f;
	public AiPathCtrl TestPlayerPath;
	// Use this for initialization
	void Start()
	{
		int max = SpawnPoint.Length;
		if (max <= 0) {
			Debug.LogWarning("SpawnPoint.len is wrong!");
			GameObject obj = null;
			obj.name = "null";
			return;
		}

		XKSpawnNpcPoint script = null;
		for (int i = 0; i < max; i++) {
			if (SpawnPoint[i] == null) {
				Debug.LogWarning("SpawnPoint is wrong! index is " + i);
				GameObject obj = null;
				obj.name = "null";
				break;
			}

			script = SpawnPoint[i].GetComponent<XKSpawnNpcPoint>();
			script.SetIsPlayerAimTrigger();
		}
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
	}

	void OnTriggerEnter(Collider other)
	{
		XkPlayerCtrl script = other.GetComponent<XkPlayerCtrl>();
		if (script == null) {
			return;
		}
		script.SetXKTriggerPlayerAim(this);
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