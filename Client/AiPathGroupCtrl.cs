using UnityEngine;
using System.Collections;

public class AiPathGroupCtrl : MonoBehaviour {
	public PlayerTypeEnum PathState = PlayerTypeEnum.FeiJi;
	void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}

		AiPathCtrl[] PathArray = transform.GetComponentsInChildren<AiPathCtrl>();
		for (int i = 0; i < PathArray.Length; i++) {
			PathArray[i].name = PathState + "AiPath_" + (i+1);
		}
	}
}
