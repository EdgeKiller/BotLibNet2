using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace BotLibNet2
{

    public enum WButton
    {
        Left,
        Right,
        Middle
    }

    public class BotMouse
    {
        private IntPtr process;

        public BotMouse(IntPtr proc)
        {
            this.process = proc;
        }

        #region GetPosition
        public Point GetPosition()
        {
            return Cursor.Position;
        }
        #endregion

        #region SetPosition
        public bool SetPosition(int x, int y)
        {
            try
            {
                Cursor.Position = new Point(x, y);
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region SendClick
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private enum WMessages : int
        {
            WM_LBUTTONDOWN = 0x201, //Left mousebutton down
            WM_LBUTTONUP = 0x202,  //Left mousebutton up
            WM_LBUTTONDBLCLK = 0x203, //Left mousebutton doubleclick
            WM_RBUTTONDOWN = 0x204, //Right mousebutton down
            WM_RBUTTONUP = 0x205,   //Right mousebutton up
            WM_RBUTTONDBLCLK = 0x206, //Right mousebutton doubleclick
            WM_MBUTTONDOWN = 0x207, //Middle mousebutton down
            WM_MBUTTONUP = 0x208, //Middle mousebutton up
            WM_MBUTTONDBLCLK = 0x209, //Middle mousebutton doubleclick
        }

        private void _SendMessage(IntPtr handle, int Msg, int wParam, int lParam)
        {
            SendMessage(handle, Msg, wParam, lParam);
        }

        private int MakeLParam(int LoWord, int HiWord)
        {
            return ((HiWord << 16) | (LoWord & 0xffff));
        }

        public void SendClick(WButton button, Point pos, int numberClick = 1, int interval = 1)
        {
            int LParam = MakeLParam(pos.X, pos.Y), btnDown = 0, btnUp = 0;
            switch (button)
            {
                case WButton.Left:
                    btnDown = (int)WMessages.WM_LBUTTONDOWN;
                    btnUp = (int)WMessages.WM_LBUTTONUP;
                    break;
                case WButton.Right:
                    btnDown = (int)WMessages.WM_RBUTTONDOWN;
                    btnUp = (int)WMessages.WM_RBUTTONUP;
                    break;
                case WButton.Middle:
                    btnDown = (int)WMessages.WM_MBUTTONDOWN;
                    btnUp = (int)WMessages.WM_MBUTTONUP;
                    break;
            }
            for (int i = 0; i < numberClick; i++)
            {
                _SendMessage(process, btnDown, 0, LParam);
                _SendMessage(process, btnUp, 0, LParam);
                if (numberClick > 1)
                    Thread.Sleep(interval);
            }


        }
        #endregion

    }

}
