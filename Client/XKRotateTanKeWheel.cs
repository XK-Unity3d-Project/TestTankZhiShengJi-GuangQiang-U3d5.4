using UnityEngine;
using System.Collections;

public class XKRotateTanKeWheel : MonoBehaviour
{
	public Vector3 SpeedVal;
	void Start()
	{
		if (XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
			gameObject.SetActive(false);
		}
	}

	// Update is called once per frame
	void Update()
	{
		transform.Rotate(SpeedVal);
	}
}