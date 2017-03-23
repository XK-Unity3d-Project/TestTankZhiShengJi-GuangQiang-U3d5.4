using UnityEngine;
using System.Collections;

public class XKSetRigidbodyMassCenter : MonoBehaviour
{
	public Vector3 MessCenterVal;
	public bool IsTestRigMessCenter;
	public Transform TestRigMessCenter;
	Rigidbody RigidbodyCom;
	// Use this for initialization
	void Start ()
	{
		RigidbodyCom = GetComponent<Rigidbody>();
	}

	void Update()
	{
		CheckRigidbodyMassCenter();
	}

	void CheckRigidbodyMassCenter()
	{
		if (!IsTestRigMessCenter) {
			return;
		}
		RigidbodyCom.centerOfMass = MessCenterVal;
		if (TestRigMessCenter != null) {
			TestRigMessCenter.position = RigidbodyCom.centerOfMass + transform.position;
		}
	}
}