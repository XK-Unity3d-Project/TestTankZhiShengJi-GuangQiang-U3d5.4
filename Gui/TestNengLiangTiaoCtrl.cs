using UnityEngine;
using System.Collections;

public class TestNengLiangTiaoCtrl : MonoBehaviour {
	public Renderer NengLiangRenderer;
	[Range(0f, 1f)]public float NengLiangVal;
	public bool IsAimPlayer;
	Transform CameraTran;
	Transform NengLianCtrlTran;
	// Use this for initialization
	void Start()
	{
		NengLianCtrlTran = transform;
		CameraTran = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (!IsAimPlayer) {
			Vector3 posA = CameraTran.position;
			Vector3 posB = NengLianCtrlTran.position;
			//posA.y = posB.y = 0f;
			Vector3 forwardVal = posB - posA;
			NengLianCtrlTran.forward = Vector3.Lerp(NengLianCtrlTran.forward, forwardVal.normalized, Time.deltaTime * 10f);
		}
		NengLiangRenderer.materials[0].SetTextureOffset("_MainTex", new Vector2(NengLiangVal, 0f));
	}
}