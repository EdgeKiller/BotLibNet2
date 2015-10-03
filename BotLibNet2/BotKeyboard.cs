/*
 * Created by SharpDevelop.
 * User: EdgeKiller
 * Date: 03/10/2015
 */
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace BotLibNet2
{
	public class BotKeyboard
	{
		private IntPtr process;

        public BotKeyboard(IntPtr proc)
        {
            this.process = proc;
        }

        #region Keys
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        public void SendKey(Keys key)
        {
            const uint WM_KEYDOWN = 0x100;
            const uint WM_KEYUP = 0x101;
            int k = (int)key;
            SendMessage(process, WM_KEYDOWN, ((IntPtr)k), (IntPtr)0);
            SendMessage(process, WM_KEYUP, ((IntPtr)k), (IntPtr)0);
        }

        public void SendMultiKey(Keys key, int pressNumber, int interval = 1)
        {
            const uint WM_KEYDOWN = 0x100;
            const uint WM_KEYUP = 0x101;
            int k = (int)key;
            for (int i = 0; i < pressNumber; i++)
            {
                SendMessage(process, WM_KEYDOWN, ((IntPtr)k), (IntPtr)0);
                SendMessage(process, WM_KEYUP, ((IntPtr)k), (IntPtr)0);
                if (pressNumber > 1)
                    Thread.Sleep(interval);
            }
        }

        public void SendKey(int key)
        {
            const uint WM_KEYDOWN = 0x100;
            const uint WM_KEYUP = 0x101;
            SendMessage(process, WM_KEYDOWN, ((IntPtr)key), (IntPtr)0);
            SendMessage(process, WM_KEYUP, ((IntPtr)key), (IntPtr)0);
        }

        public void SendMultiKey(int key, int pressNumber, int interval = 1)
        {
            const uint WM_KEYDOWN = 0x100;
            const uint WM_KEYUP = 0x101;
            for (int i = 0; i < pressNumber; i++)
            {
                SendMessage(process, WM_KEYDOWN, ((IntPtr)key), (IntPtr)0);
                SendMessage(process, WM_KEYUP, ((IntPtr)key), (IntPtr)0);
                if (pressNumber > 1)
                    Thread.Sleep(interval);
            }
        }
        #endregion
	}
}
