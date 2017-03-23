using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PSZiYouMoveCamera : MonoBehaviour {
	public AiPathCtrl AiPathScript;
	public Transform RealCamTran;
	public static float TimeUnitMove = 0.02f;
	Transform AimTran;
	Transform NextMarkTran;
	Camera RealCamera;
	float SpeedA = 0f;
	float SpeedB;
	float JiaSuDuVal;
	bool IsStartMovePlayerByMark;
	Vector3 ForwardMoveVal;
	Vector3 EndPos;
	Quaternion RotationStart;
	Quaternion RotationEnd;
	float TimeRotation;
	Vector3[] PathNodes;
	int MarkCount;
	PSZiYouCameraCtrl ZiYouCamera;
	public static GameObject ZYMoveCamera;
	// Use this for initialization
	void Start()
	{
		if (Network.peerType != NetworkPeerType.Server) {
			if (GameMovieCtrl.IsActivePlayer
			    || (!XkGameCtrl.GetInstance().IsCartoonShootTest && !XkGameCtrl.GetInstance().IsServerCameraTest)) {
				gameObject.SetActive(false);
			}
			return;
		}

		bool isOutputError = false;
		if (AiPathScript == null) {
			isOutputError = true;
			Debug.LogWarning("AiPathScript is null");
		}
		else {
			if (AiPathScript.transform.childCount < 2) {
				isOutputError = true;
				Debug.LogWarning("AiPathScript.childCount was wrong");
			}
			else {
				Transform tranMark = AiPathScript.transform.GetChild(0);
				transform.position = tranMark.position;
				transform.rotation = tranMark.rotation;
			}
		}

		Rigidbody rig = GetComponent<Rigidbody>();
		if (rig == null) {
			isOutputError = true;
			Debug.LogWarning("PSZiYouMoveCamera cannot find Rigidbody");
		}

		BoxCollider box = GetComponent<BoxCollider>();
		if (box == null) {
			isOutputError = true;
			Debug.LogWarning("PSZiYouMoveCamera cannot find BoxCollider");
		}

		if (RealCamTran == null) {
			isOutputError = true;
			Debug.LogWarning("RealCamTran is null");
		}
		else if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			RealCamera = RealCamTran.GetComponent<Camera>();
			if (RealCamera == null) {
				isOutputError = true;
				Debug.LogWarning("PSZiYouMoveCamera cannot find RealCamera");
			}
			else {
				RealCamera.tag = "MainCamera";
				ZiYouCamera = RealCamTran.GetComponentInChildren<PSZiYouCameraCtrl>();
				if (ZiYouCamera == null) {
					isOutputError = true;
					Debug.LogWarning("PSZiYouMoveCamera cannot find PSZiYouCameraCtrl");
				}
			}
			RealCamTran.gameObject.SetActive(false);
		}
		else {
			RealCamera = RealCamTran.GetComponent<Camera>();
			if (RealCamera != null) {
				RealCamera.enabled = false;
			}

			RealCamera = XkGameCtrl.ServerCameraObj.GetComponent<Camera>();
			if (RealCamera == null) {
				isOutputError = true;
				Debug.LogWarning("PSZiYouMoveCamera cannot find RealCamera");
			}
			else {
				RealCamera.tag = "MainCamera";
				ZiYouCamera = RealCamTran.GetComponentInChildren<PSZiYouCameraCtrl>();
				if (ZiYouCamera == null) {
					isOutputError = true;
					Debug.LogWarning("PSZiYouMoveCamera cannot find PSZiYouCameraCtrl");
				}
			}
			RealCamTran.gameObject.SetActive(false);
		}

		if (isOutputError) {
			GameObject obj = null;
			obj.name = "null";
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

		if (AiPathScript != null) {
			AiPathScript.DrawPath();
		}
	}

	bool CheckIsStopMoveCamera()
	{
		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
			if (RealCamera == null) {
				return false;
			}
			
			if (!RealCamera.enabled || Camera.main != RealCamera) {
				RealCamera.gameObject.SetActive(false);
				gameObject.SetActive(false);
				return true;
			}
		}
		else {
			if (ZYMoveCamera == null || ZYMoveCamera != gameObject) {
				gameObject.SetActive(false);
				return true;
			}
		}
		return false;
	}

	public void SetCameraMarkInfo(AiMark script)
	{
		if (script == null) {
			return;
		}

		if (script.IsAimPlayer) {
			ZiYouCamera.ChangeAimTran(AimTran, script);
		}
		else {
			ZiYouCamera.ChangeAimTran(null, null);
		}
	}

	public void MoveCameraByItween(XkPlayerCtrl playerScript)
	{
		ZYMoveCamera = gameObject;
		if (Network.peerType == NetworkPeerType.Server || XkGameCtrl.GetInstance().IsServerCameraTest) {
			if (Camera.main.gameObject != XkGameCtrl.ServerCameraObj) {
				if (Camera.main != null) {
					Camera.main.enabled = false;
				}
			}
			XkGameCtrl.SetServerCameraTran(RealCamTran);
		}
		else {
			if (Camera.main != null) {
				Camera.main.enabled = false;
			}
			RealCamTran.gameObject.SetActive(true);
		}
		AimTran = playerScript.transform;
		InitMovePlayerByMarkSpeed();
	}

	IEnumerator MovePlayerByMarkSpeed()
	{
		bool isStopMove = false;
		int countNode = 0;
		int maxCountNode = PathNodes.Length;
		EndPos = PathNodes[1];
		
//		Debug.Log("MovePlayerByMarkSpeed -> start time "+Time.realtimeSinceStartup);
		float timeLastVal = Time.realtimeSinceStartup;
		float timeRotationVal = 0f;
		do {
			if (CheckIsStopMoveCamera()){
				yield break;
			}

			float ds = 0f;
			float dTime = Time.realtimeSinceStartup - timeLastVal;
			if (dTime > 1f) {
				dTime = TimeUnitMove;
			}
			else {
				float minTimeUnit = 0.03f;
				dTime = dTime > minTimeUnit ? minTimeUnit : dTime;
			}
			//dTime = TimeUnitMove; //test
			timeLastVal = Time.realtimeSinceStartup;
			float disAimNode = Vector3.Distance(transform.position, EndPos);
			if (JiaSuDuVal == 0) {
				ds = SpeedA * dTime;
			}
			else {
				ds = (SpeedA * dTime) + (0.5f * JiaSuDuVal * Mathf.Pow(dTime, 2f));
				if ((SpeedA >= SpeedB && JiaSuDuVal >= 0f)
				    || (SpeedA <= SpeedB && JiaSuDuVal <= 0f)) {
					SpeedA = SpeedB;
				}
				else {
					if (disAimNode >= ds) {
						SpeedA = SpeedA + (JiaSuDuVal * dTime);
					}
					else {
						float speedValTmp = 2f*JiaSuDuVal*disAimNode + Mathf.Pow(SpeedA, 2f);
						if (speedValTmp > 0) {
							SpeedA = Mathf.Sqrt(speedValTmp);
						}
					}
					
					if ((SpeedA >= SpeedB && JiaSuDuVal >= 0f)
					    || (SpeedA <= SpeedB && JiaSuDuVal <= 0f)) {
						SpeedA = SpeedB;
					}
				}
			}

			timeRotationVal += dTime;
			if (timeRotationVal <= TimeRotation) {
				float perRot = timeRotationVal / TimeRotation;
				perRot = perRot > 1f ? 1f : perRot;
				transform.rotation = Quaternion.Lerp(RotationStart, RotationEnd, perRot);
			}
//			Debug.Log("SpeedA "+SpeedA+", ds "+ds+", dTime "+dTime);
			
			if (disAimNode <= ds) {
//				Debug.Log("Over, ds "+ds+", realDis "+Vector3.Distance(transform.position, EndPos)+", time "+dTime);
				countNode++;
				float disVal = ds - disAimNode;
				float disNode = 0f;
				int count = 0;
				for (int i = countNode; i < (maxCountNode - 2); i++) {
					disNode = Vector3.Distance(PathNodes[i], PathNodes[i+1]);
					if (disVal > disNode) {
						disVal = disVal - disNode;
						count++;
						transform.position = PathNodes[countNode];
						SmothMoveplayerCamera();
					}
					else {
						break;
					}
				}
				countNode += count;
				
				if (countNode < (maxCountNode - 1) && (countNode+1) < PathNodes.Length) {
					transform.position = EndPos;
					SmothMoveplayerCamera();
					EndPos = PathNodes[countNode+1]; //更新EndPos.
					ForwardMoveVal = Vector3.Normalize(EndPos - transform.position); //更新ForwardMoveVal.
					
//					Debug.Log("***realDis "+Vector3.Distance(transform.position, EndPos));
//					Debug.Log("***ForwardMoveVal "+Vector3.Distance(ForwardMoveVal, Vector3.zero));
					yield return new WaitForSeconds(TimeUnitMove);
					continue;
				}
				else {
					timeLastVal = Time.realtimeSinceStartup;
					timeRotationVal = 0f;
					SpeedA = SpeedB;
					transform.position = EndPos;
					SmothMoveplayerCamera();
					
					if (!MovePlayerOnCompelteITween()) {
						isStopMove = true;
						break;
					}
					countNode = 0;
					maxCountNode = PathNodes.Length;
					yield return new WaitForSeconds(TimeUnitMove);
					continue;
				}
			}
			
			transform.position += (ForwardMoveVal * ds);
			SmothMoveplayerCamera();
			yield return new WaitForSeconds(TimeUnitMove);
		} while (!isStopMove);
	}

	void SmothMoveplayerCamera()
	{
		if (ZiYouCamera == null) {
			return;
		}
		ZiYouCamera.SmothMoveCamera();
	}

	bool MovePlayerOnCompelteITween()
	{
		MarkCount++;
		Transform pathTran = AiPathScript.transform;
		if (MarkCount >= pathTran.childCount) {
			if (AiPathScript.mNextPath1 == null) {
				Debug.Log("ZiYouMoveCamera move to end position");
				return false;
			}
			MarkCount = 0;
			AiPathScript = AiPathScript.mNextPath1.GetComponent<AiPathCtrl>();
			//Debug.Log("move next path, path is "+AiPathScript.name);
		}
		MovePlayerByItween();
		return true;
	}
	
	public void MovePlayerByItween()
	{
		InitMovePlayerByMarkSpeed();
	}

	void InitMovePlayerByMarkSpeed()
	{
		Transform tran = AiPathScript.transform;
		Transform[] tranArray = new Transform[2];
		tranArray[0] = transform;
		tranArray[1] = tran.GetChild(MarkCount);
		
		EndPos = tranArray[1].position;
		AiMark markScript = tranArray[1].GetComponent<AiMark>();
//		AimMvToMarkTran = markScript.transform;
		if (EndPos == tranArray[0].position) {
			//move to next mark point
			SpeedA = markScript.MvSpeed;
			MovePlayerOnCompelteITween();
			return;
		}
		
		PathNodes = AiPathScript.GetPathNodes(MarkCount);
		if (PathNodes == null) {
			Debug.Log("PathNodes is null! MarkCount "+MarkCount);
			return;
		}
		
		EndPos = PathNodes[1];
		ForwardMoveVal = Vector3.Normalize(EndPos - tranArray[0].position);
		SpeedB = markScript.MvSpeed;
		float disVal = 0f;
		int maxNodes = PathNodes.Length;
		for (int i = 1; i < maxNodes; i++) {
			disVal += Vector3.Distance(PathNodes[i - 1], PathNodes[i]);
		}
		
		float timeVal = (2f * disVal) / (SpeedA + SpeedB);
		TimeRotation = timeVal;
		if (SpeedB != SpeedA) {
			JiaSuDuVal = ((SpeedB - SpeedA) * (SpeedA + SpeedB)) / (2f * disVal);
		}
		else {
			JiaSuDuVal = 0f;
		}
		RotationStart = tranArray[0].rotation;
		RotationEnd = tranArray[1].rotation;
//		Debug.LogError("SpeedA *** "+SpeedA+", MarkCount "+MarkCount+", maxNodes "+maxNodes);
//		Debug.LogWarning("SpeedA *** "+SpeedA+", SpeedB "+SpeedB);
		if (!IsStartMovePlayerByMark) {
			gameObject.SetActive(true);
			StartCoroutine(MovePlayerByMarkSpeed());
			IsStartMovePlayerByMark = true;
		}
	}
}