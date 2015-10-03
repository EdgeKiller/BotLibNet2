/*
 * Created by SharpDevelop.
 * User: EdgeKiller
 * Date: 03/10/2015
 */
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

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

        public int GetX()
        {
            return Cursor.Position.X;
        }

        public int GetY()
        {
            return Cursor.Position.Y;
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

        public bool SetPosition(Point pos)
        {
            try
            {
                Cursor.Position = pos;
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

        public void SendClick(WButton button, Point pos)
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
            _SendMessage(process, btnDown, 0, LParam);
            _SendMessage(process, btnUp, 0, LParam);
        }

        public void SendClick(int button, int x, int y)
        {
            int LParam = MakeLParam(x, y), btnDown = 0, btnUp = 0;
            switch ((WButton)button)
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
            _SendMessage(process, btnDown, 0, LParam);
            _SendMessage(process, btnUp, 0, LParam);
        }

        public void SendMultiClick(WButton button, Point pos, int numberClick, int interval = 1)
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

        public void SendMultiClick(int button, int x, int y, int numberClick, int interval = 1)
        {
            int LParam = MakeLParam(x, y), btnDown = 0, btnUp = 0;
            switch ((WButton)button)
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
