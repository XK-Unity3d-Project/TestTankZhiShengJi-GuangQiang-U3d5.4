using UnityEngine;
using System.Collections;

public class AiMark : MonoBehaviour
{
	public static bool IsMoveSpeedByAiMark = true;
	[Range(0.1f, 200f)]public float MvSpeed = 1f;
	/**************************************************************
	 * PlayerAni是ZhiShengJiAction.null并且TimePlayerAni > 0f时,
	 * 主角会在该产生点停留TimePlayerAni时间之后在运动.
	**************************************************************/
	public ZhiShengJiAction PlayerAni;
	[Range(0f, 100f)]public float TimePlayerAni = 0f;
	public Transform PlayerCamAimTran;
	//public Transform[] CameraAimArray;
	[Range(0.01f, 100f)]public float SpeedIntoAim = 1f;
	[Range(0.01f, 100f)]public float SpeedOutAim = 1f;
	public bool IsAimPlayer;
	public bool IsTestDrawPath;
	private Transform _mNextMark = null;
	public Transform mNextMark
	{
		get
		{
			return _mNextMark;
		}
		set
		{
			_mNextMark = value;
		}
	}
	
	private int mMarkCount = 0;
	bool IsInitMarkInfo;
	void Start()
	{
		bool isOutputError = false;
//		if (CameraAimArray.Length > 0) {
//			if (PlayerAni != ZhiShengJiAction.Null || TimePlayerAni <= 0f) {
//				Debug.Log("CameraAimArray -> PlayerAni or TimePlayerAni was wrong!");
//				isOutputError = true;
//			}
//
//			for (int i = 0; i < CameraAimArray.Length; i++) {
//				if (CameraAimArray[i] == null) {
//					Debug.Log("CameraAimArray was wrong! index "+i);
//					isOutputError = true;
//					break;
//				}
//			}
//		}

		if (PlayerAni == ZhiShengJiAction.Null && TimePlayerAni > 0f && MvSpeed > 1f) {
			Debug.Log("PlayerAni is null, but MvSpeed is greater than 1f");
			isOutputError = true;
		}

		if (isOutputError) {
			GameObject obj = null;
			obj.name = "null";
		}
		IsInitMarkInfo = true;
		IsTestDrawPath = false;
	}
	
	public void setMarkCount( int count )
	{
		mMarkCount = count;
	}
	
	public int getMarkCount()
	{
		return mMarkCount;
	}
	
	// Use this for initialization
//	void Start()
//	{
//		this.enabled = false;
//	}
	
	void OnTriggerEnter(Collider other)
	{
		PSZiYouMoveCamera script = other.GetComponent<PSZiYouMoveCamera>();
		if (script != null) {
			script.SetCameraMarkInfo(this);
			return;
		}

		XkPlayerCtrl playerScript = other.GetComponent<XkPlayerCtrl>();
		if (playerScript == null) {
			return;
		}
//		Debug.Log("AiMark::OnTriggerEnter -> AniName "+PlayerAni);
//		Debug.Log("AiMark::OnTriggerEnter -> MarkName "+gameObject.name);
		playerScript.PlayZhuJiaoMarkAction(this);
	}

	void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}
		CheckBoxCollider();
		CheckPathMarkScale();

		Transform parTran = transform.parent;
		if (parTran == null) {
			return;
		}

		AiPathCtrl pathScript = parTran.GetComponent<AiPathCtrl>();
		if (!pathScript.enabled) {
			if (!IsTestDrawPath) {
				if (!IsInitMarkInfo) {
					pathScript.enabled = true;
				}
				else {
					return;
				}
			}
			else {
				pathScript.enabled = true;
			}
		}
		else {
			if (!IsTestDrawPath) {
				if (IsInitMarkInfo) {
					pathScript.enabled = false;
				}
			}
		}
		pathScript.DrawPath();
	}
	
	void CheckPathMarkScale()
	{
		Vector3 scale = new Vector3(1f, 1f, 1f);
		if (transform.localScale != scale) {
			transform.localScale = scale;
		}
		transform.localScale = scale;
	}

	void CheckBoxCollider()
	{
		BoxCollider boxCol = GetComponent<BoxCollider>();
		if (boxCol == null) {
			boxCol = gameObject.AddComponent<BoxCollider>();
		}

		boxCol.isTrigger = true;
		Vector3 boxSize = new Vector3(1f, 1f, 1f);
		if (boxCol.size != boxSize) {
			boxCol.size = boxSize;
		}

		//"Ignore Raycast"
		gameObject.layer = LayerMask.NameToLayer("TransparentFX");
	}
}