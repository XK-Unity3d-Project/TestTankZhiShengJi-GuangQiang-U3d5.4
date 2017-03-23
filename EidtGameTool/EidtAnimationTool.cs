using UnityEngine;
using System.Collections;

public class EidtAnimationTool : MonoBehaviour {
	public NpcPathCtrl NpcPathScript;
	[Range(1, 100)]public int MarkIndex = 1;
	public Transform MoveToPoint;
	// Use this for initialization
	void Start()
	{
		enabled = false;
	}
	
	void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}

		if (NpcPathScript == null) {
			return;
		}

		if (MarkIndex < 0 || MarkIndex > NpcPathScript.transform.childCount) {
			MarkIndex = 1;
		}

		MoveToPoint = NpcPathScript.transform.GetChild(MarkIndex-1);
		if (MoveToPoint == null) {
			return;
		}
		transform.position = MoveToPoint.position;
		transform.rotation = MoveToPoint.rotation;
	}
}