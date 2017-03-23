using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcPathCtrl : MonoBehaviour {
	public bool IsMoveEndFire;
	public bool IsAutoMarkName;
	public bool IsDrawLine;
	void Start()
	{
		CheckNpcPathScript();
		IsAutoMarkName = false;
		this.enabled = false;
		NpcMark[] markScript = GetComponentsInChildren<NpcMark>();
		if (markScript.Length != transform.childCount) {
			Debug.LogWarning("NpcPathScript was wrong!");
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
		ChangeMarkName();

		Transform parTran = transform;
		if(parTran.childCount > 1)
		{
			List<Transform> nodesTran = new List<Transform>(parTran.GetComponentsInChildren<Transform>()){};
			nodesTran.Remove(parTran);
			if (IsDrawLine) {
				iTween.DrawLine(nodesTran.ToArray(), Color.blue);
			}
			else {
				iTween.DrawPath(nodesTran.ToArray(), Color.blue);
			}
		}
	}
	
	public void DrawPath ()
	{
		OnDrawGizmosSelected();
	}

	void CheckNpcPathScript()
	{
		NpcMark[] markScript = GetComponentsInChildren<NpcMark>();
		if (markScript.Length != transform.childCount) {
			Debug.LogWarning("NpcPath was wrong! markLen "+markScript.Length);
			GameObject obj = null;
			obj.name = "null";
		}
	}

	void ChangeMarkName()
	{
		if (!IsAutoMarkName) {
			return;
		}
		
		NpcMark [] tranArray = transform.GetComponentsInChildren<NpcMark>();
		for (int i = 0; i < tranArray.Length; i++) {
			tranArray[i].name = "Mark_" + i;
		}
	}
}