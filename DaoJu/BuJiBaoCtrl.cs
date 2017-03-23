using UnityEngine;
using System.Collections;

public enum BuJiBaoType
{
	Null,
	DaoDan,
	GaoBaoDan,
	YouLiang,
}

public enum PlayerEnum
{
	Null,
	PlayerOne,
	PlayerTwo,
	PlayerThree,
	PlayerFour,
}

public class BuJiBaoCtrl : MonoBehaviour {
	public BuJiBaoType BuJiBao;
	public Animator AniCom;
	public GameObject ExplodeObj;
	[Range(0.1f, 30f)]public float DestroyTime = 2f;
	bool IsDeath;
	bool IsDelayDestroy;
	NetworkView NetworkViewCom;
	void Start()
	{
		NetworkViewCom = GetComponent<NetworkView>();
		if (transform.parent != XkGameCtrl.MissionCleanup) {
			transform.parent = XkGameCtrl.MissionCleanup;
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		string layerName = LayerMask.LayerToName(collision.gameObject.layer);
		if (layerName == XkGameCtrl.TerrainLayer && !IsDelayDestroy) {
			InitDelayDestroyBuJiBao();
		}
		
		if (Network.peerType == NetworkPeerType.Client) {
			return;
		}

		PlayerZhiShengJiCtrl script = collision.transform.root.GetComponent<PlayerZhiShengJiCtrl>();
		if (script == null) {
			return;
		}
		RemoveBuJiBao(PlayerEnum.Null, 1);
	}

	void InitDelayDestroyBuJiBao()
	{	
		if (IsDelayDestroy) {
			return;
		}
		IsDelayDestroy = true;
		if (AniCom != null) {
			AniCom.SetBool("LuoDi", true);
		}
		
		if (Network.peerType != NetworkPeerType.Disconnected) {
			if (Network.peerType == NetworkPeerType.Client) {
				return;
			}
		}
		Invoke("DelayDestroyBuJiBao", DestroyTime);
	}

	void DelayDestroyBuJiBao()
	{
		RemoveBuJiBao(PlayerEnum.Null);
	}

	/// <summary>
	/// Removes the bu ji bao. key == 0 -> hit TerrainLayer, key == 1 -> PlayerOne, key == 2 -> PlayerTwo.
	/// </summary>
	/// <param name="key">Key.</param>
	public void RemoveBuJiBao(PlayerEnum key, int keyHit = 0)
	{
		if (IsDeath) {
			return;
		}
		IsDeath = true;
		CancelInvoke("DelayDestroyBuJiBao");
		if (key != PlayerEnum.Null || keyHit == 1) {
			XKGlobalData.GetInstance().PlayAudioHitBuJiBao();
			if (ExplodeObj != null) {
				GameObject obj = (GameObject)Instantiate(ExplodeObj, transform.position, transform.rotation);
				XkGameCtrl.CheckObjDestroyThisTimed(obj);
			}
			
			if (Network.peerType != NetworkPeerType.Server) {
				//Add BuJiBao
				switch (BuJiBao) {
				case BuJiBaoType.DaoDan:
					if (keyHit == 1) {
						XkGameCtrl.GetInstance().AddDaoDanNum(PlayerEnum.PlayerOne);
						XkGameCtrl.GetInstance().AddDaoDanNum(PlayerEnum.PlayerTwo);
					}
					else {
						XkGameCtrl.GetInstance().AddDaoDanNum(key);
					}
					break;
					
				case BuJiBaoType.GaoBaoDan:
					if (keyHit == 1) {
						XkGameCtrl.GetInstance().AddGaoBaoDanNum(PlayerEnum.PlayerOne);
						XkGameCtrl.GetInstance().AddGaoBaoDanNum(PlayerEnum.PlayerTwo);
					}
					else {
						XkGameCtrl.GetInstance().AddGaoBaoDanNum(key);
					}
					break;
					
				case BuJiBaoType.YouLiang:
					XkGameCtrl.GetInstance().AddPlayerYouLiang(XkGameCtrl.YouLiangBuJiNum, key);
					break;
				}
			}
		}

		if (Network.peerType != NetworkPeerType.Disconnected) {
			if (Network.peerType == NetworkPeerType.Client) {
				NetworkViewCom.RPC("BuJiBaoSendRemoveObj", RPCMode.OthersBuffered);
				return;
			}
		}
		DestroyNetObj(gameObject);
	}

	[RPC] void BuJiBaoSendRemoveObj()
	{
		if (IsDeath) {
			return;
		}
		IsDeath = true;

		if (ExplodeObj != null) {
			GameObject obj = (GameObject)Instantiate(ExplodeObj, transform.position, transform.rotation);
			XkGameCtrl.CheckObjDestroyThisTimed(obj);
		}
		DestroyNetObj(gameObject);
	}

	void DestroyNetObj(GameObject obj)
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			Destroy(obj);
		}
		else {
			if (Network.peerType == NetworkPeerType.Server) {
				if (NetworkServerNet.GetInstance() != null) {
					NetworkServerNet.GetInstance().RemoveNetworkObj(obj);
				}
			}
		}
	}
}