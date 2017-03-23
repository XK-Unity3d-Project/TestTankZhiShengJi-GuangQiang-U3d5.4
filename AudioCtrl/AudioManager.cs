using UnityEngine;

/// <summary>
/// 声音管理器。
/// </summary>
public class AudioManager : MonoBehaviour
{
	private static AudioManager mInstance;
	/// <summary>
	/// 获取声音管理器的唯一实例。
	/// </summary>
	public static AudioManager Instance
	{
	    get
	    {
	        if (mInstance == null) {
				GameObject root = new GameObject("_AudioManager");
				mInstance = root.AddComponent<AudioManager>();
				root.AddComponent<AudioListener>();
	        }
	        return mInstance;
	    }
	}

	public void SetParentTran(Transform tran)
	{
		if (GameOverCtrl.IsShowGameOver
		    || (JiFenJieMianCtrl.GetInstance() != null && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask())) {
			return;
		}
		XkGameCtrl.SetParentTran(transform, tran);
	}

	public void MoveAudioManagerObj()
	{
		Vector3[] posArray = new Vector3[2];
		posArray[0] = transform.position;
		posArray[1] = transform.position + Vector3.up * 2000f;
		iTween.MoveTo(gameObject, iTween.Hash("path", posArray,
		                                  "time", 3f,
		                                  "orienttopath", true,
		                                  "easeType", iTween.EaseType.linear));
	}
}