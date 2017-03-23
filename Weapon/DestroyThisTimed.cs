using UnityEngine;
using System.Collections;

public class DestroyThisTimed : MonoBehaviour {
	[Range(0f, 100f)] public float TimeRemove = 5f;
	// Use this for initialization
	void Start()
	{
		Destroy(gameObject, TimeRemove);
	}
}
