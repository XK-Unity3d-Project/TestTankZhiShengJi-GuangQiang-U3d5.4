using UnityEngine;
using System.Collections;

public class TestComTongXin : MonoBehaviour
{
	// Use this for initialization
	void Awake()
	{
		MyCOMDevice.PcvrComSt = PcvrComState.TanKeGunZhenDong; //test.
		switch (MyCOMDevice.PcvrComSt) {
		case PcvrComState.TanKeFangXiangZhenDong:
			MyCOMDevice.ComThreadClass.BufLenRead = 39;
			MyCOMDevice.ComThreadClass.BufLenWrite = 32;
			break;
		case PcvrComState.TanKeGunZhenDong:
			MyCOMDevice.ComThreadClass.BufLenRead = 27;
			MyCOMDevice.ComThreadClass.BufLenWrite = 23;
			break;
		}
		HID_BUF_LEN_WRITE = MyCOMDevice.ComThreadClass.BufLenWrite;
	}

	void FixedUpdate()
	{
		GetMessage();
		SendMessage();
	}
	
	void GetMessage()
	{
	}
	
	int HID_BUF_LEN_WRITE = 0;
	static byte WriteHead_1 = 0x02;
	static byte WriteHead_2 = 0x55;
	static byte WriteEnd_1 = 0x0d;
	static byte WriteEnd_2 = 0x0a;
	static byte[] JiaoYanMiMa = new byte[4];
	static byte[] JiaoYanMiMaRand = new byte[4];
	public static byte[] QiNangArray = {0, 0, 0, 0, 0, 0, 0, 0};
	void RandomJiaoYanMiMaVal()
	{
		for (int i = 0; i < 4; i++) {
			JiaoYanMiMaRand[i] = (byte)UnityEngine.Random.Range(0x00, (JiaoYanMiMa[i] - 1));
		}
		
		byte TmpVal = 0x00;
		for (int i = 1; i < 4; i++) {
			TmpVal ^= JiaoYanMiMaRand[i];
		}
		
		if (TmpVal == JiaoYanMiMaRand[0]) {
			JiaoYanMiMaRand[0] ^= 0x01; //fix JiaoYanMiMaRand[0].
		}
	}
	
	float TimeFire;
	float TimeCloseFire;
	int QNCount;
	float TimeQN;
	public byte QiNangState;
	void SendMessage()
	{
		if (Time.realtimeSinceStartup - TimeQN > 10f) {
			TimeQN = Time.realtimeSinceStartup;
			if (QNCount >= 3) {
				QNCount = -1;
			}
			QNCount++;
			
			for (int i = 0; i < 4; i++) {
				QiNangArray[i] = (byte)((i == QNCount) ? 1 : 0);
			}
		}

		byte []buffer;
		buffer = new byte[HID_BUF_LEN_WRITE];
		buffer[0] = WriteHead_1;
		buffer[1] = WriteHead_2;
		buffer[HID_BUF_LEN_WRITE - 2] = WriteEnd_1;
		buffer[HID_BUF_LEN_WRITE - 1] = WriteEnd_2;

		for (int i = 4; i < HID_BUF_LEN_WRITE - 2; i++) {
			buffer[i] = (byte)(UnityEngine.Random.Range(0, 10000) % 256);
		}
		buffer[4] &= 0xfe;
		buffer[4] &= 0xfd;

		buffer[5] = (byte)(QiNangArray[0]
		                   + (QiNangArray[1] << 1)
		                   + (QiNangArray[2] << 2)
		                   + (QiNangArray[3] << 3)
		                   + (QiNangArray[4] << 4)
		                   + (QiNangArray[5] << 5)
		                   + (QiNangArray[6] << 6)
		                   + (QiNangArray[7] << 7));
		QiNangState = buffer[5];

		RandomJiaoYanMiMaVal();
		for (int i = 0; i < 4; i++) {
			buffer[i + 10] = JiaoYanMiMaRand[i];
		}
		
		//0x41 0x42 0x43 0x44
		for (int i = 15; i < 18; i++) {
			buffer[i] = (byte)UnityEngine.Random.Range(0x00, 0x40);
		}
		buffer[14] = 0x00;
		
		for (int i = 15; i < 18; i++) {
			buffer[14] ^= buffer[i];
		}

		if (Time.realtimeSinceStartup - TimeFire < 10f) {
			TimeCloseFire = Time.realtimeSinceStartup;
			buffer[8] = 0x08;
			buffer[9] = 0x08;
		}
		else {
			if (Time.realtimeSinceStartup - TimeCloseFire < 3f) {
				buffer[8] = 0x00;
				buffer[9] = 0x00;
			}
			else {
				TimeFire = Time.realtimeSinceStartup;
			}
		}

		switch (ComTongXunState) {
		case KuaiTingCom:
			buffer[5] = 0x00;
			for (int i = 2; i <= 11; i++) {
				if (i == 5) {
					continue;
				}
				buffer[5] ^= buffer[i];
			}
			
			buffer[19] = 0x00;
			for (int i = 2; i < (HID_BUF_LEN_WRITE - 2); i++) {
				if (i == 19) {
					continue;
				}
				buffer[19] ^= buffer[i];
			}
			break;
			
		case TanKeCom:
			buffer[6] = 0x00;
			for (int i = 2; i <= 11; i++) {
				if (i == 6) {
					continue;
				}
				buffer[6] ^= buffer[i];
			}
			
			buffer[19] = 0x00;
			for (int i = 0; i < HID_BUF_LEN_WRITE; i++) {
				if (i == 19) {
					continue;
				}
				buffer[19] ^= buffer[i];
			}
			break;
		}
		MyCOMDevice.ComThreadClass.WriteByteMsg = buffer;
	}
	const byte KuaiTingCom = 0;
	const byte TanKeCom = 1;
	/**
	 * ComTongXunState == 0 -> KuaiTingCom.
	 * ComTongXunState == 1 -> TanKeCom.
	 */
	byte ComTongXunState = KuaiTingCom;
}