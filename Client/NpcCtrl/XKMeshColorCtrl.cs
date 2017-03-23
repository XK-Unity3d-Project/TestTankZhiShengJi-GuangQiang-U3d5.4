using UnityEngine;
using System.Collections;

public class XKMeshColorCtrl : MonoBehaviour
{
	/**
	 * SkinMaterialArray[0] -> oldMaterial.
	 * SkinMaterialArray[1] -> newMaterial.
	 */
	public Material[] SkinMaterialArray;
	public SkinnedMeshRenderer SkinMesh;
	//[Range(0, 10)]public int SkinMeshIndex = 0;
	/**
	 * MaterialArray[0] -> oldMaterial.
	 * MaterialArray[1] -> newMaterial.
	 */
	public Material[] MaterialArray;
	public MeshRenderer RenderMesh;
//	[Range(0, 10)]public int MeshIndex = 0;
	void Start()
	{
		if (SkinMesh != null) {
			if (SkinMaterialArray != null
			    && SkinMaterialArray.Length > 0
			    && SkinMaterialArray[0] == null) {
				SkinMaterialArray[0] = SkinMesh.material;
			}
		}

		if (RenderMesh != null) {
			if (MaterialArray != null
			    && MaterialArray.Length > 0
			    &&  MaterialArray[0] == null) {
				MaterialArray[0] = RenderMesh.material;
			}
		}
	}

	public void MakeMeshToNewColor(float timeVal)
	{
		//Debug.Log("MakeMeshToNewColor...");
		if (SkinMesh != null) {
			SkinMesh.material = SkinMaterialArray[1];
		}
		
		if (RenderMesh != null) {
			RenderMesh.material = MaterialArray[1];
		}
		CancelInvoke("OnCompelteMakeMeshToNewColor");
		Invoke("OnCompelteMakeMeshToNewColor", timeVal);
	}

	void OnCompelteMakeMeshToNewColor()
	{
		MakeMeshToOldColor();
	}

	void MakeMeshToOldColor()
	{
		if (SkinMesh != null) {
			SkinMesh.material = SkinMaterialArray[0];
		}
		
		if (RenderMesh != null) {
			RenderMesh.material = MaterialArray[0];
		}
	}
}