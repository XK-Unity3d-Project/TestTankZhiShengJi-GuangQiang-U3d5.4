using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Runtime.InteropServices.ComTypes;

public enum MODE
{
		MODE_SET_CALIBRATION,
		MODE_MOTION,
};

class CSampleGrabberCB
{
		#region Member variables
		private int m_videoWidth;
		private int m_videoHeight;
		private int m_stride;
		private int m_CamID;
		static CSampleGrabberCB _Instance;
		public static CSampleGrabberCB GetInstance()
		{
				if (_Instance == null) {
						_Instance = new CSampleGrabberCB(0);
						ScreenLog.Log("create CSampleGrabberCB!");
				}
				return _Instance;
		}
		#endregion

		/// zero based device index, and some device parms, plus the file name to save to
		public CSampleGrabberCB(int iDeviceNum)
		{
				m_CamID = iDeviceNum;
				InitFindPlayerPoint();

				#if CHECK_CAMERA_ID
				if (!CheckCameraIdInfo())
				{
					return;
				}
				#endif
		}

		public static void Msg(string str)
		{
				MessageBox.Show(str);
		}

		bool CheckCameraIdInfo()
		{
				bool isFindCamera = false;
				string pDisplayName = "";
				//dev.Mon.GetDisplayName(null, null, out pDisplayName);
				if (pDisplayName.Contains("vid_04fc") &&
						(pDisplayName.Contains("pid_fa02") || pDisplayName.Contains("pid_fa09")))
				{
						isFindCamera = true;
				}
				else
				{
						Msg("Camera vid or pid error!");
				}
				return isFindCamera;
		}

		//pBuffer -> 图像的灰度值.
		/// <summary> buffer callback, COULD BE FROM FOREIGN THREAD. </summary>
		public void BufferCB(long sampleTime, byte[] pBuffer, int bufferLen)
		{
				UpdateWindowRect(sampleTime);
				CheckBufferCB(pBuffer, bufferLen);
				return;
		}

		#region Find Player Cross Point
		const int m_nMoveRadius = 0;
		//采集器输出图像的大小信息.
		int Width;
		int Height;

		//显示器的大小信息.
		int lClientWidth;
		int lClientHeight;
		int m_nSmoothingCount;

		float m_fMark;
		float m_fExsmothX, m_fExsmothY;

		public static MODE m_mode;
		//激光器枪在显示器上投射的坐标信息.
		public static Point m_curMousePoint;
		//存储显示器四个角在采集器图像里的坐标信息.
		Point[] m_p4 = new Point[4];

		Warper m_warp;
		bool g_bBeginDrawRectangle;
		bool m_bFirstInst;

		//校准光枪第几个点信息.
		byte m_nLed;
		//用于校准光枪坐标.
		bool m_bRectifyState;
		bool m_bCurPointModified;

		//76800 = 320 * 240; -> 采集器像素的宽*高.
		Point[] unwantedPoint; //干扰光源坐标信息.
		long unwantedPointNum;
		int getFrameNum;

		const int SM_CXSCREEN = 0;
		const int SM_CYSCREEN = 1;
		Rectangle m_Rect;
		double TimeUpdateLast;

		#region API函数声明开始.
		[DllImport("kernel32")]//返回0表示失败，非0为成功
		private static extern int WritePrivateProfileStruct(string lpszSections,
				string lpszKey, byte[] lpStruct, int uSizeStruct, string szFile);

		[DllImport("kernel32")]//返回取得字符串缓冲区的长度
		private static extern int GetPrivateProfileStruct(string lpszSections,
				string lpszKey, byte[] lpStruct, int uSizeStruct, string szFile);

		[DllImport("kernel32")]
		private static extern int GetPrivateProfileInt(string lpAppName,
				string lpKeyName, int nDefault, string lpFileName);
		[DllImport("kernel32")]
		private static extern bool WritePrivateProfileString(string lpAppName,
				string lpKeyName, string lpString, string lpFileName);

		[DllImport("user32")]
		static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle rect);
		[DllImport("user32")]
		static extern IntPtr GetDesktopWindow();
		[DllImport("user32")]
		static extern int GetSystemMetrics(int nIndex);
		#endregion API函数声明结束.

		string FFileName = "test.ini";
		/// <summary>
		/// 读出INI文件(int)
		/// </summary>
		/// <param name="Section">项目名称(如 [TypeName] )</param>
		/// <param name="Key">键</param>
		public int ReadInt(string section, string key, int def)
		{
				return GetPrivateProfileInt(section, key, def, FFileName);
		}

		/// <summary>
		/// 写入INI文件(int)
		/// </summary>
		/// <param name="Section">项目名称(如 [TypeName] )</param>
		/// <param name="Key">键</param>
		/// <param name="Value">值</param>
		public void WriteInt(string section, string key, int iVal)
		{
				WritePrivateProfileString(section, key, iVal.ToString(), FFileName);
		}

		void InitFindPlayerPoint()
		{
				FFileName = Application.StartupPath + "\\test.ini";
				int rv = ReadInt("TestSec", "GrayVal", -1);
				if (rv < 0 || rv > 255)
				{
						GrayThreshold = 125;
						WriteInt("TestSec", "GrayVal", GrayThreshold);
				}
				else {
						GrayThreshold = (byte)rv;
				}

				Width = XKOpenCGCamera.GetInstance().GetCGCameraWith();
				Height = XKOpenCGCamera.GetInstance().GetCGCameraHeight();
				lClientWidth = GetSystemMetrics(SM_CXSCREEN);
				lClientHeight = GetSystemMetrics(SM_CYSCREEN);
				m_curMousePoint = Point.Empty;
				m_curMousePoint.X = -1;
				m_curMousePoint.Y = -1;
				MinPoint = Point.Empty;
				MaxPoint = Point.Empty;
				unwantedPoint = new Point[XKOpenCGCamera.GetInstance().CGCameraBufLen];

				m_warp = new Warper();
				m_mode = MODE.MODE_MOTION;
				m_bCurPointModified = false;
				m_Rect = Rectangle.Empty;

				ResetRectify();
				ResetSmoothing();
				InitRectifyCfg();

				m_bFirstInst = false;
				unwantedPointNum = 0;
				getFrameNum = 0;
		}

		void InitRectifyCfg()
		{
				int rv = -1;
				GetWindowRect(GetDesktopWindow(), ref m_Rect);
				/************************************************************************/
				/* cvSrcBt[0] -> x高字节,cvSrcBt[1] -> x低字节.                         */
				/* cvSrcBt[2] -> y高字节,cvSrcBt[3] -> y低字节.                         */
				/************************************************************************/
				byte[] cvSrcBt = new byte[4];
				string szwcKey = "";
				string strTitle = "Camera" + m_CamID;
				bool isCheckMP4Info = true;

				for (int i = 0; i < 4; i++)
				{
						szwcKey = "DataSrc" + i;
						rv = GetPrivateProfileStruct(strTitle, szwcKey, cvSrcBt, cvSrcBt.Length, ".//Rectangle.vro");
						if (rv == 1)
						{
								m_p4[i].X = (cvSrcBt[0] << 8) + cvSrcBt[1];
								m_p4[i].Y = (cvSrcBt[2] << 8) + cvSrcBt[3];
						}

						if (rv != 1 || m_p4[i].X > m_Rect.Right - m_Rect.Left || m_p4[i].Y > m_Rect.Bottom - m_Rect.Top)
						{
								isCheckMP4Info = false;
								ResetMP4Info();
								break;
						}
				}

				if (isCheckMP4Info) {
						CheckMP4Info();
				}
		}

		void ResetMP4Info()
		{
				m_p4[0].X = 20;
				m_p4[0].Y = 20;
				m_p4[1].X = 60;
				m_p4[1].Y = 20;
				m_p4[2].X = 60;
				m_p4[2].Y = 60;
				m_p4[3].X = 20;
				m_p4[3].Y = 60;
				CheckMP4Info();
		}

		void CheckMP4Info()
		{
				MinPoint.X = int.MaxValue;
				MinPoint.Y = int.MaxValue;
				MaxPoint.X = int.MinValue;
				MaxPoint.Y = int.MinValue;
				for (int i = 0; i < 4; i++) {
						if (MinPoint.X > m_p4[i].X) {
								MinPoint.X = m_p4[i].X;
						}

						if (MinPoint.Y > m_p4[i].Y) {
								MinPoint.Y = m_p4[i].Y;
						}

						if (MaxPoint.X < m_p4[i].X) {
								MaxPoint.X = m_p4[i].X;
						}

						if (MaxPoint.Y < m_p4[i].Y) {
								MaxPoint.Y = m_p4[i].Y;
						}
				}
				//ScreenLog.Log("MaxPoint "+MaxPoint+", MinPoint "+MinPoint);
		}

		void ResetRectify()
		{
				g_bBeginDrawRectangle = false;
				m_bRectifyState = false;
				m_nLed = 0;
		}

		//pBuffer -> 图像灰度值.
		void getUnwantedPoint(byte[] pBuffer)
		{
				unwantedPointNum = 0;
				for (int y = 0; y < Height; y++)
				{
						for (int x = 0; x < Width; x++)
						{
								if (pBuffer[x + (y * Width)] > 200) 
								{
										//记录干扰光源坐标信息.
										unwantedPoint[unwantedPointNum].X = x;
										unwantedPoint[unwantedPointNum].Y = y;
										unwantedPointNum++;
								}
						}
				}
		}

		void subUnWantedPoint(byte[] pBuffer, int buferSize)
		{
				for (int index = 0; index < unwantedPointNum; index++)
				{
						int x = unwantedPoint[index].X;
						int y = unwantedPoint[index].Y;
						if (x + (Width * y) >= buferSize) {
								return;
						}
						//将RGB设置为零,起到屏蔽干扰光源的目的.
						pBuffer[x + (Width * y)] = 0;
				}
		}

		//初始化校准显示器四个角在采集器输出图像的坐标.
		//校准之前务必关闭一下所有激光器,便于准确抓拍干扰光源.
		public void InitJiaoZhunZuoBiao()
		{
				if (m_mode == MODE.MODE_SET_CALIBRATION)
				{
						return;
				}
				m_nLed = 0;
				g_bBeginDrawRectangle = true;
				m_mode = MODE.MODE_SET_CALIBRATION;
				getFrameNum = 0;
		}

		//接收校准按键消息,打开查找光点开关.
		public void ActiveJiaoZhunZuoBiao()
		{
				if (m_mode != MODE.MODE_SET_CALIBRATION || m_bRectifyState)
				{
						return;
				}
				m_bRectifyState = true;
				ScreenLog.Log("ActiveJiaoZhunZuoBiao -> m_nLed " + m_nLed);
		}

		void CallGameChangeJiaoZhunPic()
		{
				//通知游戏更新校准图片信息.
				if (SetPanelUiRoot.GetInstance() != null) {
						SetPanelUiRoot.HandleJiaoZhunLaser();
				}
		}

		void OpenPlayerJiGuangQi()
		{
				//通知硬件IO打开玩家激光器.
		}

		void CallGameUpdateZhunXingZuoBiao(Point pointVal)
		{
				//通知游戏更新准星坐标信息.
				//ScreenLog.Log("crossPos " + pointVal);
				byte indexVal = IndexMousePoint;
				if (indexVal % JiGuangLQ != 0) {
						return;
				}

				//错位处理玩家准星,可能是因为摄像头图像采集延迟.
				//或激光器延时或串口通信延时导致.
				if (indexVal == 0) {
						indexVal = (byte)(MaxMousePointNum - 1);
				}
				else {
						indexVal = (byte)((indexVal / JiGuangLQ) - 1);
				}
				//ScreenLog.Log(indexVal+" <- indexVal ********** IndexMousePoint -> " + IndexMousePoint);

				pcvr.UpdatePcvrCrossPos(indexVal, pointVal);
//				XKPlayerCrossCtrl playerCross = XKPlayerCrossCtrl.GetInstance(indexVal);
//				if (playerCross != null) {
//						playerCross.UpdateZhunXingZuoBiao(pointVal);
//				}
		}

		void UpdateWindowRect(long timeVal)
		{
				//每隔一定时间更新一次数据信息.
				if (timeVal - TimeUpdateLast < 3000) {
						return;
				}
				TimeUpdateLast = timeVal;
				GetWindowRect(GetDesktopWindow(), ref m_Rect);
		}

		void CheckBufferCB(byte[] pBuffer, int bufferLen)
		{
				//CallGameUpdateZhunXingZuoBiao(new Point(getFrameNum, getFrameNum + 2)); //test.
				if (m_mode == MODE.MODE_MOTION)
				{
						if (getFrameNum >= 3333)
						{
								getFrameNum = 0;
						}

						//3333 = 10000 / (1000 / 320); -> 每隔10秒获取一次干扰光源信息,320是采集器的刷新帧率.
						if (getFrameNum % 3333 == 0)
						{
								//ScreenLog.Log("getUnwantedPoint...");
								getUnwantedPoint(pBuffer);
						}
						getFrameNum++;
				}
				else {
						if (getFrameNum == 0)
						{
								getFrameNum = 1;
								getUnwantedPoint(pBuffer);
								OpenPlayerJiGuangQi();
								return;
						}
				}

				if (unwantedPointNum > 0)
				{
						//ScreenLog.Log("subUnWantedPoint -> pointNum "+unwantedPointNum);
						subUnWantedPoint(pBuffer, bufferLen);
				}

				switch (m_mode)
				{
				case MODE.MODE_SET_CALIBRATION:
						if (g_bBeginDrawRectangle)
						{
								if (m_bRectifyState)
								{
										int nled = m_nLed;
										Point pointVal =  GetPointToConvert(pBuffer);
										//Con::printf("nled %d, m_nled %d",nled, m_nLed);

										//find pointJiGuangQi
										if (nled != m_nLed && 4 >= m_nLed)
										{
												//m_p4的坐标顺序与光枪校准顺序相反.
												m_p4[4 - m_nLed].X = pointVal.X;
												m_p4[4 - m_nLed].Y = pointVal.Y;
												if (4 - m_nLed == 0) {
														CheckMP4Info();
												}
												m_bRectifyState = false;
												//添加代码,改变校准图片信息.
												CallGameChangeJiaoZhunPic();
										}
								}

								//光枪校准已经完成.
								if (4 == m_nLed)
								{
										SetCalibrationInfo();
										g_bBeginDrawRectangle = false;
										m_mode = MODE.MODE_MOTION;
								}
						}
						break;

				case MODE.MODE_MOTION:
						if (!m_bFirstInst)
						{
								SetCalibrationInfo();
						}
						ConvertGrayBitmapFindPoint(pBuffer);

						if (m_bCurPointModified)
						{
								//找到激光器亮点坐标信息.
								m_bCurPointModified = false;
								//改变准星坐标.
								CallGameUpdateZhunXingZuoBiao(m_curMousePoint);
						}

						if (IsCGCameraQuickCOM) {
								AddIndexMousePoint();
						}
						else {
								//通知XKLaserPosCtrl下一帧数据关闭所有激光器.
								XKLaserPosCtrl.SetIsCloseAllJiGuangQi(true);
						}
						break;

				default:
						break;
				}
				return;
		}

		/**
		 * IsCGCameraQuickCOM == true  -> 摄像机刷新速度快于串口通信速度.
		 * IsCGCameraQuickCOM == false -> 摄像机刷新速度慢于串口通信速度.
		 */
		public static bool IsCGCameraQuickCOM = true;
		/**
		 * 改变IndexMousePoint数值,控制激光器状态信息.
		 * 1.当串口通信速度大于摄像头刷新速度时,在串口通信里控制激光器切换状态.
		 * 2.当串口通信速度小于摄像头刷新速度时,在摄像机刷新图像里控制激光器切换状态.
		 */
		public void AddIndexMousePoint()
		{
				//ScreenLog.Log("IndexMousePoint ----- " + IndexMousePoint);
				IndexMousePoint++;
				if (IndexMousePoint >= MaxMousePointLQ) {
						IndexMousePoint = 0;
				}
		}

		//有效图像范围的信息.
		Point MinPoint;
		Point MaxPoint;

		//设置透视变换类信息.
		void SetCalibrationInfo()
		{
				string szwcKey = "";
				string strTitle = "Camera"+m_CamID;
				/************************************************************************/
				/* cvSrcBt[0] -> x高字节,cvSrcBt[1] -> x低字节.                         */
				/* cvSrcBt[2] -> y高字节,cvSrcBt[3] -> y低字节.                         */
				/************************************************************************/
				byte[] cvSrcBt = new byte[4];
				Point[] cvsrc = new Point[4]; //采集器点信息.
				Point[] cvdst = new Point[4]; //显示器屏幕信息.
				m_bFirstInst = true; //设置透视变换类信息状态.

				cvdst[0].X = 0;
				cvdst[0].Y = 0;
				cvdst[1].X = lClientWidth;
				cvdst[1].Y = 0;
				cvdst[2].X = lClientWidth;
				cvdst[2].Y = lClientHeight;
				cvdst[3].X = 0;
				cvdst[3].Y = lClientHeight;

				//test.
				//m_p4[0].X = 0;
				//m_p4[0].Y = 62;
				//m_p4[1].X = 285;
				//m_p4[1].Y = 56;
				//m_p4[2].X = 266;
				//m_p4[2].Y = 165;
				//m_p4[3].X = 33;
				//m_p4[3].Y = 180;
				//test.
				for( int i = 0; i < 4; i++ )
				{
						cvsrc[i].X = m_p4[i].X;
						cvsrc[i].Y = m_p4[i].Y;

						cvdst[i].X = (int)(cvdst[i].X * ((float)Width / lClientWidth));
						cvdst[i].Y = (int)(cvdst[i].Y * ((float)Height / lClientHeight));

						if( m_mode == MODE.MODE_SET_CALIBRATION )
						{
								szwcKey = "DataSrc" + i;
								cvSrcBt[0] = (byte)((m_p4[i].X & 0xff00) >> 8);
								cvSrcBt[1] = (byte)((m_p4[i].X & 0x00ff) >> 0);
								cvSrcBt[2] = (byte)((m_p4[i].Y & 0xff00) >> 8);
								cvSrcBt[3] = (byte)((m_p4[i].Y & 0x00ff) >> 0);

								szwcKey = "DataSrc" + i;
								WritePrivateProfileStruct(strTitle, szwcKey, cvSrcBt, cvSrcBt.Length, ".//Rectangle.vro");
						}
				}

				//设置透视变换类信息.
				m_warp.setSource(cvsrc[0].X, cvsrc[0].Y,
						cvsrc[1].X, cvsrc[1].Y,
						cvsrc[2].X, cvsrc[2].Y,
						cvsrc[3].X, cvsrc[3].Y);

				m_warp.setDestination(cvdst[0].X, cvdst[0].Y,
						cvdst[1].X, cvdst[1].Y,
						cvdst[2].X, cvdst[2].Y,
						cvdst[3].X, cvdst[3].Y);
		}

		void ConvertGrayBitmapFindPoint(byte[] pBuffer)
		{
				int nMax_x = 0;
				int nMax_y = 0;
				float nMaxx1 = 0.0f;
				float nMaxy1 = 0.0f;
				float ax = 0.0f;
				float ay = 0.0f;
				float b = 0.0f;
				bool bIsMouseInClient = false;
				float maxGray = 255;
				//优化算法->在有效图像范围内查找光点.
				for (int j = MinPoint.Y; j < MaxPoint.Y; j++)
				{
						for (int i = MinPoint.X; i < MaxPoint.X; i++)
						{
								if (pBuffer[i + (j * Width)] > GrayThreshold) {
										//这里是重点,暂时不知道为什么要这样计算.
										ax += maxGray * i;
										ay += maxGray * j;
										b += maxGray;
										bIsMouseInClient = true;
								}
						}
				}

				if (!bIsMouseInClient)
				{
						return;
				}

				if (b != 0)
				{
						ax = ax / b;
						ay = ay / b;
				}
				nMaxx1 = ax;
				nMaxy1 = ay;

				//重点,透视变换类.
				PointF pointNV = m_warp.warp(nMaxx1, nMaxy1);
				nMaxx1 = pointNV.X;
				nMaxy1 = pointNV.Y;
				//坐标平滑处理.
				//PointF pointVal = Exponentialsmoothing(nMaxx1, nMaxy1);
				//nMaxx1 = pointVal.X;
				//nMaxy1 = pointVal.Y;

				if (bIsMouseInClient)
				{
//						ScreenLog.Log("rectR "+m_Rect.Right
//										+", rectL "+m_Rect.Left
//										+", rectB "+m_Rect.Bottom
//										+", rectT "+m_Rect.Top);
						nMax_x = (int)(((float)Math.Abs(m_Rect.Right - m_Rect.Left) / (float)Width) * nMaxx1);
						nMax_y = (int)(((float)Math.Abs(m_Rect.Bottom - m_Rect.Top) / (float)Height) * nMaxy1);

						nMax_x = nMax_x > m_Rect.Right ? m_Rect.Right : nMax_x;
						nMax_y = nMax_y > m_Rect.Bottom ? m_Rect.Bottom : nMax_y;

						int d1 = (int)Math.Abs(m_curMousePoint.X - nMax_x);
						if (d1 > m_nMoveRadius)
						{
								m_curMousePoint.X = nMax_x;
								m_bCurPointModified = true;
						}

						int d2 = (int)Math.Abs(m_curMousePoint.Y - nMax_y);
						if (d2 > m_nMoveRadius)
						{
								m_curMousePoint.Y = nMax_y;
								m_bCurPointModified = true;
						}
				}

				//if (!bIsMouseInClient)
				//{
				//    m_curMousePoint.X = -1;
				//    m_curMousePoint.Y = -1;
				//}
		}

		//4->激光器数量.
		const byte MaxMousePointNum = 4;
		//2->冷却激光器使用,确保同一时刻只有一个激光点在画面里.
		public static byte JiGuangLQ = 2;
		byte MaxMousePointLQ = (byte)(MaxMousePointNum * JiGuangLQ);
		public static byte IndexMousePoint;
		//灰度图的阀值.
		byte GrayThreshold = 120;
		Point GetPointToConvert(byte[] pBuffer)
		{
				bool isStopCheck = false;
				Point pointVal = Point.Empty;
				for (int y = 0; y < Height; y++)
				{
						for (int x = 0; x < Width; x++)
						{
								//找到激光器亮点.
								if (pBuffer[x + (y * Width)] > GrayThreshold)
								{
										isStopCheck = true;
										pointVal.X = x;
										pointVal.Y = y;
										m_nLed++;
										break;
								}
						}

						if (isStopCheck)
						{
								break;
						}
				}
				return pointVal;
		}

		void ResetSmoothing()
		{
				m_nSmoothingCount = 0;
				m_fMark = 0.05f;
				m_fExsmothX = 0.0f;
				m_fExsmothY = 0.0f;
		}

		PointF Exponentialsmoothing(float warpedX, float warpedY)
		{
				PointF pointVal = PointF.Empty;
				if (m_nSmoothingCount == 0)
				{
						m_fExsmothX = warpedX;
						m_fExsmothY = warpedY;
						m_nSmoothingCount = 1;
				}
				else
				{
						m_fExsmothX = m_fMark * warpedX + (1 - m_fMark) * m_fExsmothX;
						m_fExsmothY = m_fMark * warpedY + (1 - m_fMark) * m_fExsmothY;
				}
				warpedX = m_fExsmothX;
				warpedY = m_fExsmothY;

				pointVal.X = warpedX;
				pointVal.Y = warpedY;
				return pointVal;
		}
		#endregion
}