using UnityEngine;
using System.Collections;

public class FirePointCtrl : MonoBehaviour {

	void Start()
	{
		enabled = false;
		FirePoint[] tranArray = transform.GetComponentsInChildren<FirePoint>();
		if (tranArray.Length != transform.childCount) {
			Debug.LogWarning("FirePoint was wrong!");
			GameObject obj = null;
			obj.name = "null";
		}
	}

	void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}
		SetFirePointName();
	}

	public void SetFirePointName()
	{
		FirePoint[] tranArray = transform.GetComponentsInChildren<FirePoint>();
		for (int i = 0; i < tranArray.Length; i++) {
			tranArray[i].name = "Point_" + i;
		}
	}
}