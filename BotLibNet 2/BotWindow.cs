using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace BotLibNet2
{
    public struct Rect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }

    public class BotWindow
    {
        private IntPtr process;

        public BotWindow(IntPtr proc)
        {
            this.process = proc;
        }

        #region GetPosition
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);
        public Point GetPosition()
        {
            Rect WinRect = new Rect();
            GetWindowRect(process, ref WinRect);
            Point position = new Point(WinRect.Left, WinRect.Top);
            return position;
        }

        public int GetX()
        {
            return GetPosition().X;
        }

        public int GetY()
        {
            return GetPosition().Y;
        }
        #endregion

        #region SetPosition
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        public bool SetPosition(int x, int y)
        {
            return SetWindowPos(process, IntPtr.Zero, x, y, 0, 0, 5);
        }

        public bool SetPosition(Point pos)
        {
            return SetWindowPos(process, IntPtr.Zero, pos.X, pos.Y, 0, 0, 5);
        }
        #endregion

        #region GetSize
        public Size GetSize()
        {
            Rect WinRect = new Rect();
            GetWindowRect(process, ref WinRect);
            int height = WinRect.Bottom - WinRect.Top;
            int width = WinRect.Right - WinRect.Left;
            Size size = new Size(width, height);
            return size;
        }

        public int GetWidth()
        {
            return GetSize().Width;
        }

        public int GetHeight()
        {
            return GetSize().Height;
        }
        #endregion

        #region SetSize
        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        public bool SetSize(int width, int height)
        {
            return MoveWindow(process, GetX(), GetY(), width, height, true);
        }

        public bool SetSize(Size size)
        {
            return MoveWindow(process, GetX(), GetY(), size.Width, size.Height, true);
        }
        #endregion

        #region RemoveBar
        [DllImport("USER32.DLL")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public bool RemoveWindowBorder(int width, int height)
        {
            const int GWL_STYLE = (-16);
            const Int32 WS_VISIBLE = 0x10000000;
            SetWindowLong(process, GWL_STYLE, (WS_VISIBLE));
            SetPosition(0, 0);
            SetSize(width, height);
            return true;
        }
        #endregion
    }
}
