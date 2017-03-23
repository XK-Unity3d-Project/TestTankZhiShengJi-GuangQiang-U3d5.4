using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PSCameraPath : MonoBehaviour {
	[Range(0.1f, 1000f)]public float MvSpeed = 1f;
	// Use this for initialization
	void Start()
	{
		enabled = false;
		int max = transform.childCount - 1;
		PSCameraMark markScript = null;
		for (int i = 0; i < max; i++){
			markScript = transform.GetChild(i).GetComponent<PSCameraMark>();
			if (markScript != null) {
				markScript.SetNextMark(transform.GetChild(i+1));
			}
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

		Transform [] tranArray = transform.GetComponentsInChildren<Transform>();
		for (int i = 1; i < tranArray.Length; i++) {
			tranArray[i].name = "Mark_" + (i-1);
		}

		if (transform.childCount > 1) {
			List<Transform> nodesTran = new List<Transform>(transform.GetComponentsInChildren<Transform>()){};
			nodesTran.Remove(transform);
			iTween.DrawPath(nodesTran.ToArray(), Color.blue);
			return;
		}
	}

	public void DrawPath ()
	{
		OnDrawGizmosSelected();
	}
}
