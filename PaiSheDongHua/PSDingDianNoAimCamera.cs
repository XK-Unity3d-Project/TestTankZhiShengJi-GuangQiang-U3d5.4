using UnityEngine;
using System.Collections;

public class PSDingDianNoAimCamera : MonoBehaviour {
	Camera CameraCom;
	float TimeLast;
	public static GameObject DingDianNoAimCamera;
	void Start()
	{
		CameraCom = GetComponent<Camera>();
		if (!XkGameCtrl.GetInstance().IsCartoonShootTest) {
			if (CameraCom != null) {
				CameraCom.enabled = false;
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		CheckCameraCom();
	}

	void CheckCameraCom()
	{
		if (Time.realtimeSinceStartup - TimeLast < 0.1f) {
			return;
		}
		TimeLast = Time.realtimeSinceStartup;

		if (!XkGameCtrl.GetInstance().IsCartoonShootTest) {
			if (DingDianNoAimCamera == null || DingDianNoAimCamera != gameObject) {
				gameObject.SetActive(false);
			}
			return;
		}
		else {
			if (CameraCom == null) {
				return;
			}
			
			if (!CameraCom.enabled || Camera.main != CameraCom) {
				gameObject.SetActive(false);
			}
		}
	}
}