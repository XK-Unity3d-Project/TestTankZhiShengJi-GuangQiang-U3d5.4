using UnityEngine;
using System.Collections;

public class XKPlayerFirePointCtrl : MonoBehaviour
{
	/**
	 * 坦克机台双屏版本时,2个玩家的子弹发射点重合.
	 * AmmoStartPos[0] -> 用于玩家自身子弹发射点.
	 * AmmoStartPos[1] -> 用于其他端口子弹发射点(其他端口应该还是各自的发射点).
	 */
	public Transform[] AmmoStartPos;
	public Transform[] DaoDanAmmoPos;
	// Use this for initialization
	void Start ()
	{
		PcvrComState tankPMState = XkGameCtrl.GetInstance().GameTKPMState;
		if (tankPMState != PcvrComState.TanKeGame2Ping) {
			return;
		}

		XKPlayerAutoFire fireScript = GetComponent<XKPlayerAutoFire>();
		fireScript.AmmoStartPosOne[0] = AmmoStartPos[0];
		fireScript.AmmoStartPosTwo[0] = AmmoStartPos[0];
		fireScript.DaoDanAmmoPosOne = DaoDanAmmoPos;
		fireScript.DaoDanAmmoPosTwo = DaoDanAmmoPos;
	}
}