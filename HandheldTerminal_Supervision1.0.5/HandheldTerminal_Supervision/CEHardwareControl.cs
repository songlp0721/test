using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace HandheldTerminal_Supervision
{
    /// <summary>
    /// CE设备硬件控制
    /// </summary>
    public static class CEHardwareControl
    {
            #region 设置系统时间
            public struct SystemTime
          {
             public short wYear;
             public short wMonth;
             public short wDayOfWeek;
             public short wDay;
             public short wHour;
             public short wMinute;
             public short wSecond;
             public short wMiliseconds;
          }
            [DllImport("coredll.dll")]
            public static extern bool SetSystemTime(ref SystemTime sysTime); //设置系统时间
            [DllImport("coredll.dll")]
            public static extern bool GetSystemTime(ref SystemTime sysTime); //设置系统时间
            /// <summary>
            /// 设置系统时间
            /// </summary>
            /// <param name="dt"></param>
            /// <returns></returns>
            public static  bool setSystemTime(DateTime dt)
          {
              try
              {
                  dt = dt-new TimeSpan(8,0,0);

                  SystemTime time = new SystemTime();
                  GetSystemTime(ref time);
                  time.wYear = (short)dt.Year;
                  time.wMonth = (short)dt.Month;
                  time.wDay = (short)dt.Day;
                  time.wHour = (short)dt.Hour;
                  time.wMinute = (short)dt.Minute;
                  time.wSecond = (short)dt.Second;
                  SetSystemTime(ref time);
                  return true;
              }
              catch
              {
                  return false;
              }

          }
            #endregion

            #region 设置任务栏开关
            [DllImport("coredll.dll")]
            public static extern int FindWindow(string lpClassName, string lpWindowName);
            [DllImport("coredll.dll")]
            internal extern static int EnableWindow(int hwnd, int fEnable);
            [DllImport("coredll.dll")]
            public static extern int ShowWindow(int hwnd, int nCmdShow);
            /// <summary>
            /// 根据参数控制任务栏的显示与否
            /// </summary>
            /// <param name="showTemp">0：隐藏；1：显示</param>
            public static void ControlShow_Hide(int showTemp)
            {
                int hTaskBarWnd = FindWindow("HHTaskBar", null);
                ShowWindow(hTaskBarWnd, showTemp);
            }
            #endregion

            #region 背光控制
            /// <summary>
            /// 获取当前背景灯亮度 0~10
            /// </summary>
            /// <returns></returns>
            public static int getBacklLight()
            {
                RegistryKey RegKey = Registry.CurrentUser.OpenSubKey(@"ControlPanel\BackLight", true);

                int n = (int)RegKey.GetValue("BattLightLever");
                RegKey.Close();
                return n;

            }



            /// <summary>
            /// 设置屏幕背景灯亮度
            /// </summary>
            /// <param name="n">取值1~10</param>
            public static bool setBackLight(int n)
            {
                if (n > 10 || n < 1)
                {
                    return false;
                }
                RegistryKey RegKey = Registry.CurrentUser.OpenSubKey(@"ControlPanel\BackLight", true);
                RegKey.SetValue("BattLightLever", n, RegistryValueKind.DWord);
                RegKey.Close();
                return true;
            }


            /// <summary>
            /// 关闭背光灯。
            /// </summary>
            /// <returns></returns>
            public static bool CloseBackLight()
            {
                try
                {
                    RegistryKey RegKey = Registry.CurrentUser.OpenSubKey(@"ControlPanel\BackLight", true);
                    RegKey.SetValue("BattLightLever", 0, RegistryValueKind.DWord);
                    RegKey.Close();
                    return true;
                }
                catch (System.Exception)
                {
                    return false;
                }
            }

            #endregion

            #region 加载外部引用
            //电源使用

            [DllImport("AddinionalDLL_Mfc.dll")]
            public static extern int GetPowerState();
            [DllImport("PowerDLL.dll")]
            public static extern int GetBattery();
            #endregion
    }



}
