using UnityEngine;
using System.Collections;

public class XKTriggerBuJiBaoClose : MonoBehaviour {
	public XKTriggerBuJiBaoOpen TriggerBuJiBaoOpen;
	public AiPathCtrl TestPlayerPath;
	void Start()
	{
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<XkPlayerCtrl>() == null) {
			return;
		}

		bool isClose = TriggerBuJiBaoOpen.CloseSpawnBuJiBaoToPlayer();
		if (!isClose) {
			return;
		}
		gameObject.SetActive(false);
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