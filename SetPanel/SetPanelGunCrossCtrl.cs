using UnityEngine;
using System.Collections;

public class SetPanelGunCrossCtrl : MonoBehaviour {
	public PlayerEnum PlayerSt = PlayerEnum.PlayerOne;
	public GameObject [] AimObjArray;
	Transform ObjTran;
	GameObject CrossObj;

//	static SetPanelGunCrossCtrl _Instance;
//	public static SetPanelGunCrossCtrl GetInstance()
//	{
//		return _Instance;
//	}

	static SetPanelGunCrossCtrl _InstanceOne;
	public static SetPanelGunCrossCtrl GetInstanceOne()
	{
		return _InstanceOne;
	}
	
	static SetPanelGunCrossCtrl _InstanceTwo;
	public static SetPanelGunCrossCtrl GetInstanceTwo()
	{
		return _InstanceTwo;
	}

	// Use this for initialization
	void Awake()
	{
		//_Instance = this;
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			_InstanceOne = this;
			break;

		case PlayerEnum.PlayerTwo:
			_InstanceTwo = this;
			break;
		}
		CrossObj = transform.gameObject;
		ObjTran = transform;
		SetPlayerGunCrossActive(false);
		SetPlayerAimObjArrayActive(false);
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			ObjTran.localPosition = pcvr.CrossPositionOne;
			break;

		case PlayerEnum.PlayerTwo:
			ObjTran.localPosition = pcvr.CrossPositionTwo;
			break;
		}
	}

	public static void SetGunCrossActive(bool isActive)
	{
		_InstanceOne.SetPlayerGunCrossActive(isActive);
		_InstanceTwo.SetPlayerGunCrossActive(isActive);
	}
	
	public void SetPlayerGunCrossActive(bool isActive)
	{
		if (isActive == CrossObj.activeSelf) {
			return;
		}
		CrossObj.SetActive(isActive);
	}

	public static void SetAimObjArrayActive(bool isActive)
	{
		_InstanceOne.SetPlayerAimObjArrayActive(isActive);
		_InstanceTwo.SetPlayerAimObjArrayActive(isActive);
	}

	public void SetPlayerAimObjArrayActive(bool isActive)
	{
		int max = AimObjArray.Length;
		for (int i = 0; i < max; i++) {
			if (AimObjArray[i] != null && AimObjArray[i].activeSelf != isActive) {
				AimObjArray[i].SetActive(isActive);
			}
		}
	}
}
