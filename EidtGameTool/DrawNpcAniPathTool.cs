using UnityEngine;
using System.Collections;

public class DrawNpcAniPathTool : MonoBehaviour {
	public NpcMark NpcMarkScript;
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

		if (NpcMarkScript == null) {
			return;
		}
		NpcMarkScript.DrawPath();
	}
}