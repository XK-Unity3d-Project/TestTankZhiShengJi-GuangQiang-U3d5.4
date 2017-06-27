using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using CGSDK;
using CGAPI = CGSDK.CGAPI;

using FactoryHandle = System.IntPtr;
using DeviceHandle = System.IntPtr;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
//using UnityEngine.SceneManagement;

public class XKOpenCGCamera : MonoBehaviour
{
		string FilePath;
		string ImgPath = "";
		DeviceHandle mDeviceHandle = IntPtr.Zero;
		long LastTimeVal;
		int CameraFrameVal = 60;
		int CGCamFrameCount;
		public bool IsPlayCGCam = false;
		public bool IsShowCGCamFrame = false;
		Texture2D mImg = null;
		const int CGCameraWith = 320;
		const int CGCameraHeight = 240;
		public int CGCameraBufLen = CGCameraWith * CGCameraHeight;
		byte[] mBufHandle;

		static XKOpenCGCamera _Instance;
		public static XKOpenCGCamera GetInstance()
		{
				if (_Instance == null) {
						GameObject obj = new GameObject();
						DontDestroyOnLoad(obj);
						obj.name = "XKCGCameraCtrl";
						_Instance = obj.AddComponent<XKOpenCGCamera>();
						CSampleGrabberCB.GetInstance();
				}
				return _Instance;
		}

		// Use this for initialization
		void Start ()
		{
				#if !UNITY_EDITOR
				//此处代码用于发布版里,动态复制dll到指定目录,方便游戏的发布维护.
				string sourePath = UnityEngine.Application.dataPath+"/Plugins/CGDEVSDK.dll";
				string destPath = UnityEngine.Application.dataPath + "/Mono/CGDEVSDK.dll";
				try
				{
						bool isExitDll = File.Exists(destPath);
						//Debug.Log("isExitDll "+isExitDll);
						if (!isExitDll) {
							// Copy the file.
							File.Copy(sourePath, destPath);
							Debug.Log("{"+sourePath+"} copied to {"+destPath+"}");
							
							//Restart app.
							UnityEngine.Application.Quit();
							RunSystemCmd.RunCmd("start Tank.exe");
							return;
						}
				} 
				catch 
				{
						Debug.Log("Double copy is not allowed, which was not expected.");
				}
				#endif

				//_Instance = this;
				CGCameraBufLen = CGCameraWith * CGCameraHeight;
				mImg = new Texture2D(CGCameraWith, CGCameraHeight, TextureFormat.ARGB32, false);
				mBufHandle = new byte[CGCameraBufLen];

				FilePath = UnityEngine.Application.dataPath + "/../CGCamera";
				if (!Directory.Exists(FilePath)) {
						Directory.CreateDirectory(FilePath);
				}
				ImgPath = FilePath+"/CGCmaraTmp.png";

				uint[] adwVersion = new uint[4];
				CGAPI.DeviceGetSDKVersion(adwVersion);
				SearchDevices();
				SelectedCGCameraDevice();
				if (mDeviceHandle != IntPtr.Zero)
				{
						Debug.Log("Device start...");
						CGAPI.DeviceStart(mDeviceHandle);
						SetCGCameraInfo();
				}
		}

		//拍照并且保存.
		private void Snapshot()
		{
				if (mDeviceHandle != DeviceHandle.Zero)
				{
						DeviceStatus devStatus = CGAPI.CaptureFile(mDeviceHandle, ImgPath, emDSFileType.FILE_PNG);
						if (DeviceStatus.STATUS_OK == devStatus){
								//Debug.Log("保存成功! "+ImgPath);
								//MessageBox.Show(ImgPath, "保存成功");
						} else{
								MessageBox.Show(ImgPath, "保存失败");
						}
				}
		}

		void SearchDevices()
		{
				DeviceStatus devSatus = CGAPI.DeviceInitialSDK(IntPtr.Zero, false);
				if (DeviceStatus.STATUS_OK == devSatus)
				{
						int iCameraCounts = 0;
						devSatus = CGAPI.EnumDevice(IntPtr.Zero, ref iCameraCounts);
						if (DeviceStatus.STATUS_OK == devSatus) {
								IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(new EnumDeviceParam()) * iCameraCounts);
								devSatus = CGAPI.EnumDevice(ptr, ref iCameraCounts);
								if (DeviceStatus.STATUS_OK == devSatus)
								{
										for (int i = 0; i < iCameraCounts; i++)
										{
												EnumDeviceParam edp = (EnumDeviceParam)Marshal.PtrToStructure((IntPtr)((int)ptr + i * Marshal.SizeOf(new EnumDeviceParam())),
																												typeof(EnumDeviceParam));
												string strDevice = String.Format("{0} : {1}", edp.lpDeviceDesc, edp.devIndex);
												Debug.Log("DeviceInfo "+strDevice);
										}
								}
								Marshal.FreeHGlobal(ptr);
						}
				}
		}

		public void OnRecvFrame(IntPtr pDevice, IntPtr pImageBuffer, ref DeviceFrameInfo pFrInfo, IntPtr lParam)
		{
				IntPtr pRGB24Buff = CGAPI.DeviceISP(mDeviceHandle, pImageBuffer, ref pFrInfo);
				if (pRGB24Buff == IntPtr.Zero) {
						return;
				}
				//CGAPI.DeviceDisplayRGB24(mDeviceHandle, pRGB24Buff, ref pFrInfo); //显示图像有关.

				//Debug.Log("OnRecvFrame...");
				//重点->这里是将IntPtr转换为byte[]数组.
				int countBuf = (int)(pFrInfo.uiWidth * pFrInfo.uiHeight);
				//mBufHandle = new byte[countBuf];
				Marshal.Copy(pImageBuffer, mBufHandle, 0, countBuf);
				for (int i = 0; i < mBufHandle.Length; i++)
				{
						if (mBufHandle[i] == 0)
						{
								continue;
						}
						else
						{
								//Debug.Log("i " + i + ", val " + mBufHandle[i]);
						}
				}
				
				DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
				DateTime nowTime = DateTime.Now;
				long unixTime = (long)Math.Round((nowTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);
				if (IsShowCGCamFrame) {
						if (LastTimeVal == 0) {
								LastTimeVal = unixTime;
						}
						else {
								CGCamFrameCount++;
								long dTimeVal = unixTime - LastTimeVal;
								if (dTimeVal >= 1000) {
										CameraFrameVal = CGCamFrameCount;
										CGCamFrameCount = 0;
										LastTimeVal = unixTime;
										//Console.WriteLine("dTime " + unixTime + ", camZhenLv " + CamZhenLvVal);
								}
						}
				}

				if (CSampleGrabberCB.GetInstance() != null) {
						CSampleGrabberCB.GetInstance().BufferCB(unixTime, mBufHandle, CGCameraBufLen);
				}
		} 

		public static void OnReceiveFrame(IntPtr pDevice, IntPtr pImageBuffer, ref DeviceFrameInfo pFrInfo, IntPtr lParam)
		{
				if (XKOpenCGCamera.GetInstance() == null) {
						return;
				}
				XKOpenCGCamera.GetInstance().OnRecvFrame(pDevice, pImageBuffer, ref pFrInfo, lParam);
		}

		void OnApplicationQuit()
		{
				Debug.Log("OnApplicationQuit...CGCamera");
				CloseCGCamera();
		}

		public void CloseCGCamera()
		{
				if (mDeviceHandle != IntPtr.Zero)
				{
						Debug.Log("CloseCGCamera...");
						CGAPI.DeviceStop(mDeviceHandle);
						CGAPI.CloseDevice(mDeviceHandle);
						CGAPI.DeviceUnInit(mDeviceHandle);
						CGAPI.DeviceRelease(mDeviceHandle);
						mDeviceHandle = IntPtr.Zero;
						CGAPI.DeviceUnInitialSDK();
				}
		}

		private void SelectedCGCameraDevice()
		{
				if (IntPtr.Zero != mDeviceHandle)
				{
						CGAPI.DeviceStop(mDeviceHandle);
						CGAPI.SyncCloseDevice(mDeviceHandle);
						CGAPI.DeviceUnInit(mDeviceHandle);
						CGAPI.DeviceRelease(mDeviceHandle);
						mDeviceHandle = IntPtr.Zero;
				}
				else
				{
						byte byDev = 1;
						DeviceStatus devStatus = CGAPI.OpenDeviceByUSBAddress(byDev, ref mDeviceHandle);
						if (DeviceStatus.STATUS_OK == devStatus)
						{
								ReceiveFrameProc rfCallBack = new ReceiveFrameProc(OnReceiveFrame);
								//不添加回调函数.
								//devStatus = CGAPI.DeviceInit(mDeviceHandle, panelVideo.Handle, false, true);
								//添加回调函数.
								//devStatus = CGAPI.DeviceInitEx(mDeviceHandle, rfCallBack, IntPtr.Zero, panelVideo.Handle, true);

								devStatus = CGAPI.DeviceInitEx(mDeviceHandle, rfCallBack, IntPtr.Zero, IntPtr.Zero, true);
								if (DeviceStatus.STATUS_OK == devStatus)
								{
										Debug.Log("Open CGCamera...");
								}
						}
				}
		}

		void OnGUI()
		{
				if (IsPlayCGCam) {
						if (mBufHandle != null &&
								Time.frameCount % 5 == 0) {
								int x = 0;
								int y = 0;
								int indexVal = 0;
								float colorTmp = 0;
								System.Drawing.Color colorValTmp = System.Drawing.Color.Black;
								UnityEngine.Color colorVal = UnityEngine.Color.black;
								while(y < CGCameraHeight) {
										x = 0;
										while(x < CGCameraWith) {
												/**
												 *	Unity图片像素顺序.
												 *	2		3
												 *	0		1 
												 *
												 *	CGCamera图片像素顺序.
												 *	0		1
												 *	2		3
												 */
												//indexVal = x + (y * CGCameraWith);
												//篡改图片像素顺序.
												indexVal = (CGCameraWith - x - 1) + (y * CGCameraWith);
												indexVal = CGCameraBufLen - indexVal - 1;
												colorTmp = (float)mBufHandle[indexVal] / 255;
												colorVal = new UnityEngine.Color(colorTmp, colorTmp, colorTmp);
												mImg.SetPixel(x, y, colorVal);
												++x;
										}
										++y;
								}
								mImg.Apply();
						}

						if (mImg != null) {
								GUI.DrawTexture(new Rect(10, 10, CGCameraWith, CGCameraHeight), mImg);
						}
				}

				if (IsShowCGCamFrame) {
						GUI.color = UnityEngine.Color.green;
						GUI.Label(new Rect(10f, 270f, 200f, 30), "CameraFps :: "+CameraFrameVal);
				}
		}

		public int GetCGCameraWith()
		{
				return CGCameraWith;
		}

		public int GetCGCameraHeight()
		{
				return CGCameraHeight;
		}

		//设置CGCamera的通用参数.
		void SetCGCameraInfo()
		{
				if (mDeviceHandle == IntPtr.Zero) {
						return;
				}
				Debug.Log("SetCGCameraInfo...");
				CGAPI.SetMirror(mDeviceHandle, emMirrorDirection.MD_HORIZONTAL, false);
				CGAPI.SetMirror(mDeviceHandle, emMirrorDirection.MD_VERTICAL, false);
				CGAPI.SetAnalogGain(mDeviceHandle, 63);
				CGAPI.SetFrameSpeed(mDeviceHandle, emDeviceFrameSpeed.HIGH_SPEED, false);
				CGAPI.SetFrameSpeedTune(mDeviceHandle, 0);
				CGAPI.SaveParameter(mDeviceHandle, emParameterTeam.PARAMETER_TEAM_A);

				//在这里设置摄像头的分辨率.
//				ResolutionParam param = new ResolutionParam();
//				CGAPI.GetResolution(mDeviceHandle, ref param);
//				param.type = 1; //摄像机输出图像的旋转有关.
//				param.dri.devROISize.iHOffset = 0;
//				param.dri.devROISize.iVOffset = 0;
//				param.dri.devROISize.iWidth = 320;
//				param.dri.devROISize.iHeight = 240;
//				CGAPI.SetResolution(mDeviceHandle, param);

				SetFindPointCGCameraInfo();
		}

		//设置CGCamera的参数,用于调整摄像头位置.
		void SetPlayCGCameraInfo()
        {
                SetCGCameraIsOpen(true);
				if (mDeviceHandle == IntPtr.Zero) {
						return;
				}
				CGAPI.SetExposureTime(mDeviceHandle, 16383);
		}

		//设置CGCamera的参数,
		void SetFindPointCGCameraInfo()
        {
                SetCGCameraIsOpen(false);
				if (mDeviceHandle == IntPtr.Zero) {
						return;
				}
				CGAPI.SetExposureTime(mDeviceHandle, 239);
		}

		void Update()
		{
				//test start
				if (Input.GetKeyUp(KeyCode.K)) {
						Snapshot();
						SetCGCameraIsOpen(!IsPlayCGCam);
				}

				if (Input.GetKeyUp(KeyCode.P)) {
						SetPlayCGCameraInfo();
				}

				if (Input.GetKeyUp(KeyCode.L)) {
						SetFindPointCGCameraInfo();
				}

				if (Input.GetKeyUp(KeyCode.F)) {
						IsShowCGCamFrame = !IsShowCGCamFrame;
//						pcvr.SetIsCloseAllJiGuangQi(true);
				}
				//test end
		}

		public void SetCGCameraIsOpen(bool isOpen)
		{
				IsPlayCGCam = isOpen;
				IsShowCGCamFrame = isOpen;
		}
}