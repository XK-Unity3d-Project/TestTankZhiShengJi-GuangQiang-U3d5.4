using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AiPathCtrl : MonoBehaviour
{
	//int PathIndex;
	int KeyState;
	public Transform mNextPath1 = null;
	Transform mNextPath2 = null;
	Transform mNextPath3 = null;
//	float MvSpeed = 1f;
	bool IsOrienttopath;
//	public ZhiShengJiAction PlayerAni;
//	[Range(0f, 100f)]public float TimePlayerAni = 0f;
	Transform PlayerCamAimTran;
	iTween.LoopType PathLoopType = iTween.LoopType.none;
	int NextPathNum;
	int PrePathNum;
	Transform PrePathTran1;
	Transform PrePathTran2;
	Transform PrePathTran3;

	/****************************************************
	 * 路径点个数为[2, 20]个.
	 ***************************************************/
	void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}

		AiPathCtrl pathScript;
		if (mNextPath1 != null) {
			pathScript = mNextPath1.GetComponent<AiPathCtrl>();
			pathScript.SetPrePathTran(transform);
		}

		if (transform.childCount > 20) {
			int maxCount = transform.childCount;
			for (int i = maxCount-1; i > 19; i--) {
				DestroyImmediate(transform.GetChild(i).gameObject);
			}
		}

		AiMark [] tranArray = transform.GetComponentsInChildren<AiMark>();
		for (int i = 0; i < tranArray.Length; i++) {
			tranArray[i].name = "Mark_" + i;
		}
		
		if (transform.childCount <= 1 && mNextPath1 == null) {
			return;
		}

		List<Transform> nodes = new List<Transform>(transform.GetComponentsInChildren<Transform>()){};
		nodes.Remove(transform);
		if (mNextPath1 != null && mNextPath1.childCount > 1) {
			nodes.Add(mNextPath1.GetChild(0));
			nodes.Add(mNextPath1.GetChild(1));
		}

		if (PrePathTran1 != null && PrePathTran1.childCount > 1) {
			nodes.Insert(0, PrePathTran1.GetChild(PrePathTran1.childCount-1));
			nodes.Insert(0, PrePathTran1.GetChild(PrePathTran1.childCount-2));
		}
		DrawAiPath(nodes.ToArray());

		//GetPathNodes(TestNodesNum); //test
	}

	void CheckNpcPathScript()
	{
		AiMark[] markScript = GetComponentsInChildren<AiMark>();
		if (markScript.Length != transform.childCount) {
			Debug.LogWarning("PlayerPath was wrong! markLen "+markScript.Length);
			GameObject obj = null;
			obj.name = "null";
		}
	}

	//public int TestNodesNum = 1;
	public Vector3[] GetPathNodes(int markCount)
	{
		if (markCount > (transform.childCount - 1) || markCount < 0) {
			//markCount > (transform.childCount - 1)该路径已经走完.
			Debug.LogWarning("markCount was wrong! markCount "+markCount);
			return null;
		}

		List<Transform> nodes = new List<Transform>(transform.GetComponentsInChildren<Transform>()){};
		nodes.Remove(transform);
		if (mNextPath1 != null && mNextPath1.childCount > 1) {
			nodes.Add(mNextPath1.GetChild(0));
			nodes.Add(mNextPath1.GetChild(1));
		}

		bool isPrePath = false;
		if (PrePathTran1 != null && PrePathTran1.childCount > 1) {
			nodes.Insert(0, PrePathTran1.GetChild(PrePathTran1.childCount-1));
			nodes.Insert(0, PrePathTran1.GetChild(PrePathTran1.childCount-2));
			isPrePath = true;
		}
		else {
			if (markCount == 0) {
				//start move no.1 path
				return null;
			}
		}

		Transform[]tranArray = nodes.ToArray();
		Vector3[] path = new Vector3[tranArray.Length];
		for (int i = 0; i < path.Length; i++) {
			path[i] = tranArray[i].position;
		}
		Vector3[] vector3s = PathControlPointGenerator(path);
		
		//Line Draw:
//		int SmoothAmount = path.Length*20;
		int SmoothAmount = path.Length*(path.Length - 1);
		if (path.Length < 21) {
			SmoothAmount = (1+(20/(path.Length-1)))*path.Length*(path.Length-1);
		}
		
		int markPointNum = (int)(SmoothAmount/(path.Length - 1));
		int pathLength = markPointNum+1;
//		int countMark = markPointNum;
		int startCount = 0;
		int endCount = 0;
		if (!isPrePath) {
			startCount = markPointNum*(markCount-1);
			endCount = startCount + pathLength;
		}
		else {
			startCount = markPointNum*(markCount+1);
			endCount = startCount + pathLength;
		}
		Vector3[] pathNode = new Vector3[pathLength];
		
		//Gizmos.color=Color.blue;
//		float radius = 0.05f;
//		Vector3 size = new Vector3(radius, radius, radius);
		int indexNode = 0;
		float pm = 0f;
		for (int i = startCount; i < endCount; i++) {
			pm = (float) i / SmoothAmount;
			indexNode = i - startCount;
			//Debug.Log("indexNode "+indexNode+", i "+i+", len "+pathNode.Length);
			pathNode[indexNode] = Interp(vector3s,pm);
			//Gizmos.DrawCube(pathNode[indexNode], size);
		}
		return pathNode;
	}

	public void DrawPath()
	{
		OnDrawGizmosSelected();
	}

	public void SetPrePathTran(Transform tranVal)
	{
		PrePathNum++;
		switch(PrePathNum)
		{
		case 1:
			PrePathTran1 = tranVal;
			break;

		case 2:
			PrePathTran2 = tranVal;
			break;

		case 3:
			PrePathTran3 = tranVal;
			break;
		}
	}
	
	public Transform GetPrePathTran(int val)
	{
		Transform tranVal = PrePathTran1;
		switch(val)
		{
		case 1:
			tranVal = PrePathTran1;
			break;

		case 2:
			tranVal = PrePathTran2;
			break;

		case 3:
			tranVal = PrePathTran3;
			break;
		}
		return tranVal;
	}
	
	public int GetPrePathNum()
	{
		return PrePathNum;
	}

	public int GetNextPathNum()
	{
		return NextPathNum;
	}

	// Use this for initialization
	void Start()
	{
		CheckNpcPathScript();
		if (transform.childCount < 2) {
			Debug.LogWarning("AiPath node count was wrong!");
			GameObject obj = null;
			obj.name = "null";
		}

		AiPathCtrl pathScript;
		if(mNextPath1 != null)
		{
			NextPathNum++;
			pathScript = mNextPath1.GetComponent<AiPathCtrl>();
			pathScript.SetPrePathTran(transform);
		}
		
		if(mNextPath2 != null)
		{
			NextPathNum++;
			pathScript = mNextPath2.GetComponent<AiPathCtrl>();
			pathScript.SetPrePathTran(transform);
		}

		if(mNextPath3 != null)
		{
			NextPathNum++;
			pathScript = mNextPath3.GetComponent<AiPathCtrl>();
			pathScript.SetPrePathTran(transform);
		}

		AiMark markScript;
		int count = transform.childCount;
		for(int i = 0; i < count; i++)
		{
			Transform mark = transform.GetChild(i);
			markScript = mark.GetComponent<AiMark>();
			markScript.setMarkCount( i );
			if(i < (count - 1))
			{
				markScript.mNextMark = transform.GetChild(i + 1);
			}
			else
			{
				if(mNextPath1 != null && mNextPath1.childCount > 0)
				{
					markScript.mNextMark = mNextPath1.GetChild(0);
				}
				else if(mNextPath2 != null && mNextPath2.childCount > 0)
				{
					markScript.mNextMark = mNextPath2.GetChild(0);
				}
				else if(mNextPath3 != null && mNextPath3.childCount > 0)
				{
					markScript.mNextMark = mNextPath3.GetChild(0);
				}
			}
		}
		this.enabled = false;
	}

	public iTween.LoopType GetPathLoopType()
	{
		return PathLoopType;
	}

	/******************drawPath**************************/
	public static void DrawAiPath(Transform[] tranArray)
	{
		Vector3[] path = new Vector3[tranArray.Length];
		for (int i = 0; i < path.Length; i++) {
			path[i] = tranArray[i].position;
		}
		DrawPathHelper(path, Color.green, "gizmos");
	}
	
//	private static Vector3[] GetPathNodes(Vector3[] path){
//		Vector3[] vector3s = PathControlPointGenerator(path);
//		
//		//Line Draw:
//		Vector3 prevPt = Interp(vector3s,0);
////		Gizmos.color=color;
//		int SmoothAmount = path.Length*20;
//		Vector3[] pathNode = new Vector3[SmoothAmount+1];
//		pathNode[0] = prevPt;
//		for (int i = 1; i <= SmoothAmount; i++) {
//			float pm = (float) i / SmoothAmount;
//			Vector3 currPt = Interp(vector3s,pm);
//			pathNode[i] = currPt;
////			if(method == "gizmos"){
////				Gizmos.DrawWireSphere(prevPt, 0.01f);
////				Gizmos.DrawLine(currPt, prevPt);
////			}else if(method == "handles"){
////				Debug.LogError("iTween Error: Drawing a path with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
////				//UnityEditor.Handles.DrawLine(currPt, prevPt);
////			}
////			prevPt = currPt;
//		}
//		return pathNode;
//	}

	private static void DrawPathHelper(Vector3[] path, Color color, string method){
		if (path.Length <= 1) {
			return;
		}

		Vector3[] vector3s = PathControlPointGenerator(path);
		//Line Draw:
		Vector3 prevPt = Interp(vector3s,0);
		Gizmos.color=color;
//		float radius = 0.05f;
//		Vector3 size = new Vector3(radius, radius, radius);
		//int SmoothAmount = path.Length*20;
		int SmoothAmount = path.Length*(path.Length - 1);
		if (path.Length < 21) {
			SmoothAmount = (1+(20/(path.Length-1)))*path.Length*(path.Length-1);
		}

		int markPointNum = (int)(SmoothAmount/(path.Length - 1));
		int countMark = markPointNum;
		for (int i = 1; i <= SmoothAmount; i++) {
			float pm = (float) i / SmoothAmount;
			Vector3 currPt = Interp(vector3s,pm);
			if(method == "gizmos"){
				if (countMark == markPointNum) {
//					Gizmos.color=Color.red;
//					Gizmos.DrawCube(prevPt, size);
					countMark = 0;
				}
				else {
//					Gizmos.color=color;
//					Gizmos.DrawWireCube(prevPt, size);
				}
				Gizmos.DrawLine(currPt, prevPt);
			}else if(method == "handles"){
				Debug.LogError("iTween Error: Drawing a path with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
				//UnityEditor.Handles.DrawLine(currPt, prevPt);
			}
			prevPt = currPt;
			countMark++;
		}
//		Gizmos.color=Color.red;
//		Gizmos.DrawCube(prevPt, size);
	}	
	
	private static Vector3[] PathControlPointGenerator(Vector3[] path){
		Vector3[] suppliedPath;
		Vector3[] vector3s;
		
		//create and store path points:
		suppliedPath = path;
		
		//populate calculate path;
		int offset = 2;
		vector3s = new Vector3[suppliedPath.Length+offset];
		Array.Copy(suppliedPath,0,vector3s,1,suppliedPath.Length);
		
		//populate start and end control points:
		//vector3s[0] = vector3s[1] - vector3s[2];
		vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
		vector3s[vector3s.Length-1] = vector3s[vector3s.Length-2] + (vector3s[vector3s.Length-2] - vector3s[vector3s.Length-3]);
		
		//is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
		if(vector3s[1] == vector3s[vector3s.Length-2]){
			Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
			Array.Copy(vector3s,tmpLoopSpline,vector3s.Length);
			tmpLoopSpline[0]=tmpLoopSpline[tmpLoopSpline.Length-3];
			tmpLoopSpline[tmpLoopSpline.Length-1]=tmpLoopSpline[2];
			vector3s=new Vector3[tmpLoopSpline.Length];
			Array.Copy(tmpLoopSpline,vector3s,tmpLoopSpline.Length);
		}	
		
		return(vector3s);
	}
	
	//andeeee from the Unity forum's steller Catmull-Rom class ( http://forum.unity3d.com/viewtopic.php?p=218400#218400 ):
	private static Vector3 Interp(Vector3[] pts, float t){
		int numSections = pts.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
		float u = t * (float) numSections - (float) currPt;
		
		Vector3 a = pts[currPt];
		Vector3 b = pts[currPt + 1];
		Vector3 c = pts[currPt + 2];
		Vector3 d = pts[currPt + 3];
		
		return .5f * (
			(-a + 3f * b - 3f * c + d) * (u * u * u)
			+ (2f * a - 5f * b + 4f * c - d) * (u * u)
			+ (-a + c) * u
			+ 2f * b
			);
	}	
}