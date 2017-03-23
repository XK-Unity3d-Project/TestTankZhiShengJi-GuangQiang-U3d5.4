using UnityEngine;
using System.Collections;

public class XKNpcDamageCtrl : MonoBehaviour
{
	[Range(0.01f, 3f)]public float DamageTime = 0.1f;
	public XKMeshColorCtrl[] MeshColorArray;
	public void PlayNpcDamageEvent()
	{
		//Debug.Log("PlayNpcDamageEvent...");
		for (int i = 0; i < MeshColorArray.Length; i++) {
			if (MeshColorArray[i] != null) {
				MeshColorArray[i].MakeMeshToNewColor(DamageTime);
			}
		}
	}
}