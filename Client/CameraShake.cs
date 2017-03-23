using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
	float TimeShake = 4f; //控制镜头抖动的时长.
	private Transform mCamTran;	//Main Camera transform
	private float fCamShakeImpulse = 0.0f;	//Camera Shake Impulse
	float minShakeVal = 0.001f;
	public static bool IsCameraShake;
	// Use this for initialization
	void Awake () {
		mCamTran = transform;
		//InvokeRepeating("TestCamShake", 3f, 3f);
	}
	
	void TestCamShake()
	{
		SetCameraShakeImpulseValue(0.5f);
	}
	
	void FixedUpdate()
	{
		CameraMain();
	}

	public void SetCameraTimeShake(float val)
	{
		float minTimeVal = 0.01f;
		TimeShake = val > minTimeVal ? val : minTimeVal;
	}

	/*
	*	FUNCTION: Controls camera movements
	*	CALLED BY: FixedUpdate()
	*/
	private void CameraMain()
	{
		//make the camera shake if the fCamShakeImpulse is not zero
		if (fCamShakeImpulse > 0.0f) {
			shakeCamera();
		}
	}
	
	/*
	*	FUNCTION: Make the camera vibrate. Used for visual effects
	*/
	void shakeCamera()
	{
		Vector3 pos = mCamTran.position;
		pos.x += Random.Range(0, 100) % 2 == 0 ? Random.Range(-fCamShakeImpulse, -minShakeVal) : Random.Range(minShakeVal, fCamShakeImpulse);
		pos.y += Random.Range(0, 100) % 2 == 0 ? Random.Range(-fCamShakeImpulse, -minShakeVal) : Random.Range(minShakeVal, fCamShakeImpulse);
		//pos.z += Random.Range(0, 100) % 2 == 0 ? Random.Range(-fCamShakeImpulse, -minShakeVal) : Random.Range(minShakeVal, fCamShakeImpulse);
		mCamTran.position = pos;
		
		fCamShakeImpulse -= Time.deltaTime * fCamShakeImpulse * TimeShake;
		if (fCamShakeImpulse < minShakeVal) {
			fCamShakeImpulse = 0.0f;
			IsCameraShake = false;
		}
	}

	/*
	*	FUNCTION: Set the intensity of camera vibration
	*	PARAMETER 1: Intensity value of the vibration
	*/
	public void SetCameraShakeImpulseValue(float zhenFuVal)
	{
		if (fCamShakeImpulse > 0.0f) {
			return;
		}
		IsCameraShake = true;
		fCamShakeImpulse = zhenFuVal;
	}
}