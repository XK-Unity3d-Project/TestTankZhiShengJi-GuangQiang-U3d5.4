using UnityEngine;
using System.Collections;

public class CrossInfoCtrl : MonoBehaviour {
	public PlayerEnum PlayerSt = PlayerEnum.Null;
	public UILabel CrossPxLable;
	public UILabel CrossPyLable;
	// Update is called once per frame
	void Update()
	{
		switch(PlayerSt) {
		case PlayerEnum.PlayerOne:
			CrossPxLable.text = "px: "+pcvr.CrossPositionOne.x;
			CrossPyLable.text = "py: "+pcvr.CrossPositionOne.y;
			break;
			
		case PlayerEnum.PlayerTwo:
			CrossPxLable.text = "px: "+pcvr.CrossPositionTwo.x;
			CrossPyLable.text = "py: "+pcvr.CrossPositionTwo.y;
			break;
		}
	}
}