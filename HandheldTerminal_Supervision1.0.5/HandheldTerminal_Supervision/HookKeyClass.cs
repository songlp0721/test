using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HandheldTerminal_Supervision
{
    public class HookKeyClass
    {
        public delegate int HookKeyProc(int code, IntPtr wParam, IntPtr lParam);
        public delegate void KeyEventHandler(int KeyValue);
        public event KeyEventHandler KeyEvent;
        private HookKeyProc hookKeyDeleg;
        private int hHookKey = 0;
        public HookKeyClass()
        { Start(); }

        ~HookKeyClass()
        { Stop(); }


        #region public methods
        //安装钩子
        private void Start()
        {
            if (hHookKey != 0)
            {
                //Unhook the previouse one
                this.Stop();
            }

            hookKeyDeleg = new HookKeyProc(HookKeyProcedure);
            hHookKey = SetWindowsHookEx(WH_KEYBOARD_LL, hookKeyDeleg, GetModuleHandle(null), 0);
            if (hHookKey == 0)
            {
                throw new SystemException("Failed acquiring of the hook.");
            }
        }

        //拆除钩子
        public void Stop()
        {
            UnhookWindowsHookEx(hHookKey);
        }

        #endregion


        #region protected and private methods
        private int HookKeyProcedure(int code, IntPtr wParam, IntPtr lParam)
        {
            switch ((int)wParam)
            {
                case 257:         //按下响应
                    KBDLLHOOKSTRUCT hookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                    if (code < 0)
                    { return CallNextHookEx(hookKeyDeleg, code, wParam, lParam); }
                    //if(hookStruct.vkCode==201 | 
                    KeyEvent(hookStruct.vkCode);
                    break;
                case 256:

                    break;

                default: break;
            }

            return CallNextHookEx(hookKeyDeleg, code, wParam, lParam);
        }
        #endregion


        #region P/Invoke declarations

        [DllImport("coredll.dll")]
        private static extern int SetWindowsHookEx(int type, HookKeyProc HookKeyProc, IntPtr hInstance, int m);
        //private static extern int SetWindowsHookEx(int type, HookMouseProc HookMouseProc, IntPtr hInstance, int m);

        [DllImport("coredll.dll")]
        private static extern IntPtr GetModuleHandle(string mod);


        [DllImport("coredll.dll")]
        private static extern int CallNextHookEx(HookKeyProc hhk, int nCode, IntPtr wParam, IntPtr lParam);


        [DllImport("coredll.dll")]
        private static extern int GetCurrentThreadId();

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);
        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        const int WH_KEYBOARD_LL = 20;
        public class KeyBoardInfo
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
        }
        #endregion
    }
}
