using UnityEngine;
using System.Collections;

public class AudioListCtrl : MonoBehaviour {
	public AudioSource ASSetMove;				//设置移动提示音.
	public AudioSource ASSetEnter;				//设置确定提示音.
	public AudioSource ASTouBi;					//投币提示音.
	public AudioSource ASStartBt;				//按确认按键提示音.
	public AudioSource ASModeBeiJing;			//模式选择背景音效.
	public AudioSource ASModeXuanZe;			//模式选择音效.
	public AudioSource ASModeQueRen;			//模式选择确认音效.
	public AudioSource[] ASGuanKaBJ;			//每关背景音效.
	/**
	 * 主角飞机或坦克的炮弹为空或在导弹的间隔时间内玩家继续按下发射键的提示音效.
	 */
	public AudioSource ASDaoDanJingGao;
	/**
	 * 主角飞机或坦克在游戏途中加油音效，也就是当产生加油爆炸特效时再播放此声音.
	 */
	public AudioSource ASJiaYouBaoZha;
	public AudioSource ASRanLiaoJingGao;			//主角飞机或坦克在燃料不足时的提示音效.
	public AudioSource ASHitBuJiBao;				//主角击中补给包时的音效.
	public AudioSource ASRenWuOver;					//主角任务完成提示音效.
	public AudioSource ASGameOver;					//主角Game Over时的失败音效.
	public AudioSource ASXuBiDaoJiShi;				//续币倒计时提示音效（每过一秒响一次）.
	public AudioSource ASXunZhangJB;				//基本勋章贴在积分界面上的声音.
	public AudioSource ASXunZhangZP;				//总评勋章贴在积分界面上的声音.
	public AudioSource ASJiFenGunDong;				//积分时数字的滚动声音.
	public AudioSource ASZhunXingTX;				//准星特效声音.
	static AudioListCtrl _Instance;
	public static AudioListCtrl GetInstance()
	{
		if (_Instance == null) {
			if (XkGameCtrl.GetInstance() != null) {
				GameObject obj = (GameObject)Instantiate(XkGameCtrl.GetInstance().AudioListPrefab);
				_Instance = obj.GetComponent<AudioListCtrl>();
			}
		}
		return _Instance;
	}

	void Awake()
	{
		if (_Instance != null) {
			Destroy(gameObject);
			return;
		}

		_Instance = this;
		XkGameCtrl.SetParentTran(transform, AudioManager.Instance.transform);
		ResetGameAudioSource();
		DontDestroyOnLoad(gameObject);
	}
	
	public static void StopLoopAudio(AudioSource asVal, int key = 0)
	{
		if (asVal == null) {
			return;
		}

		if (key == 0) {
			asVal.Stop();
		}
		else {
			TweenVolume tVol = asVal.GetComponent<TweenVolume>();
			if (tVol != null) {
				tVol.enabled = true;
			}
			AudioManager.Instance.MoveAudioManagerObj();
		}
	}

	void ResetGameAudioSource()
	{
		CloseAudioSourceOnAwake(ASSetMove);
		CloseAudioSourceOnAwake(ASSetEnter);
		CloseAudioSourceOnAwake(ASTouBi);
		CloseAudioSourceOnAwake(ASStartBt);
		CloseAudioSourceOnAwake(ASModeBeiJing);
		CloseAudioSourceOnAwake(ASModeXuanZe);
		CloseAudioSourceOnAwake(ASModeQueRen);
		
		int max = ASGuanKaBJ.Length;
		for (int i = 0; i < max; i++) {
			CloseAudioSourceOnAwake(ASGuanKaBJ[i]);
		}
		
		CloseAudioSourceOnAwake(ASDaoDanJingGao);
		CloseAudioSourceOnAwake(ASJiaYouBaoZha);
		CloseAudioSourceOnAwake(ASRanLiaoJingGao);
		CloseAudioSourceOnAwake(ASHitBuJiBao);
		CloseAudioSourceOnAwake(ASRenWuOver);
		CloseAudioSourceOnAwake(ASGameOver);
		CloseAudioSourceOnAwake(ASXuBiDaoJiShi);
		CloseAudioSourceOnAwake(ASXunZhangJB);
		CloseAudioSourceOnAwake(ASXunZhangZP);
		CloseAudioSourceOnAwake(ASJiFenGunDong);
	}
	
	void CloseAudioSourceOnAwake(AudioSource asVal)
	{
		if (asVal == null) {
			return;
		}
		asVal.playOnAwake = false;
	}
	
	/**
	 * key == 0 -> 强制停止音效.
	 * key == 1 -> 音效停止时播放音效.
	 * key == 2 -> 循环播放音效.
	 */
	public static void PlayAudioSource(AudioSource asVal, int key = 0)
	{
		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}

		if (asVal == null) {
			return;
		}
		
		switch (key) {
		case 0:
			asVal.Stop();
			break;
			
		case 1:
			if (asVal.isPlaying) {
				return;
			}
			break;
			
		case 2:
			if (!asVal.loop) {
				asVal.loop = true;
			}
			
			if (asVal.isPlaying) {
				asVal.Stop();
			}
			break;
		}
		
		asVal.Play();
	}

	public void RemoveAudioSource(AudioSource val)
	{
		DestroyImmediate(val);
	}

	public void CloseGameAudioBJ()
	{
		int max = ASGuanKaBJ.Length;
		for (int i = 0; i < max; i++) {
			ASGuanKaBJ[i].Stop();
		}
	}
}