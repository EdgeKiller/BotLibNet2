/*
 * Created by SharpDevelop.
 * User: EdgeKiller
 * Date: 03/10/2015
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

namespace BotLibNet2
{
	public class BotImage
	{
		
		IntPtr process;

		public BotImage(IntPtr proc)
		{
			this.process = proc;
		}

		public Color GetPixelColor(Point pos, bool foregroundMode = false)
		{
			return GetWindowImage(foregroundMode).GetPixel(pos.X, pos.Y);
		}

		public Bitmap CaptureRegion(Rectangle region, bool foregroundMode = false)
		{
			return GetWindowImage(foregroundMode).Clone(new Rectangle(region.X, region.Y, region.Width, region.Height), PixelFormat.Format24bppRgb);
		}

		#region GetScreenImage

		[StructLayout(LayoutKind.Sequential)]
		struct RECT
		{
			int _Left;
			int _Top;
			int _Right;
			int _Bottom;

			public RECT(RECT Rectangle)
				: this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
			{
			}
			public RECT(int Left, int Top, int Right, int Bottom)
			{
				_Left = Left;
				_Top = Top;
				_Right = Right;
				_Bottom = Bottom;
			}

			public int X
			{
				get { return _Left; }
				set { _Left = value; }
			}
			public int Y
			{
				get { return _Top; }
				set { _Top = value; }
			}
			public int Left
			{
				get { return _Left; }
				set { _Left = value; }
			}
			public int Top
			{
				get { return _Top; }
				set { _Top = value; }
			}
			public int Right
			{
				get { return _Right; }
				set { _Right = value; }
			}
			public int Bottom
			{
				get { return _Bottom; }
				set { _Bottom = value; }
			}
			public int Height
			{
				get { return _Bottom - _Top; }
				set { _Bottom = value + _Top; }
			}
			public int Width
			{
				get { return _Right - _Left; }
				set { _Right = value + _Left; }
			}
			public Point Location
			{
				get { return new Point(Left, Top); }
				set
				{
					_Left = value.X;
					_Top = value.Y;
				}
			}
			public Size Size
			{
				get { return new Size(Width, Height); }
				set
				{
					_Right = value.Width + _Left;
					_Bottom = value.Height + _Top;
				}
			}

			public static implicit operator Rectangle(RECT Rectangle)
			{
				return new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
			}
			public static implicit operator RECT(Rectangle Rectangle)
			{
				return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
			}
			public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
			{
				return Rectangle1.Equals(Rectangle2);
			}
			public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
			{
				return !Rectangle1.Equals(Rectangle2);
			}

			public override string ToString()
			{
				return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
			}

			public override int GetHashCode()
			{
				return ToString().GetHashCode();
			}

			public bool Equals(RECT Rectangle)
			{
				return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
			}

			public override bool Equals(object Object)
			{
				if (Object is RECT)
				{
					return Equals((RECT)Object);
				}
				if (Object is Rectangle)
				{
					return Equals(new RECT((Rectangle)Object));
				}
				return false;
			}
		}

		[DllImport("user32.dll")]
		private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
		[DllImport("user32.dll")]
		static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);
		[DllImport("user32.dll")]
		private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
		[DllImport("user32.dll")]
		private static extern int SetForegroundWindow(IntPtr hWnd);
		private const int SW_RESTORE = 9;
		[DllImport("user32.dll")]
		private static extern IntPtr ShowWindow(IntPtr hWnd, int nCmdShow);
		public Bitmap GetWindowImage(bool foregroundMode = false)
		{
			if (!foregroundMode)
			{
				RECT rc;
				GetWindowRect(process, out rc);
				Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format24bppRgb);
				Graphics gfxBmp = Graphics.FromImage(bmp);
				IntPtr hdcBitmap = gfxBmp.GetHdc();
				PrintWindow(process, hdcBitmap, 0);
				gfxBmp.ReleaseHdc(hdcBitmap);
				gfxBmp.Dispose();
				return bmp;
			}
			else
			{
				SetForegroundWindow(process);
				ShowWindow(process, SW_RESTORE);
				Thread.Sleep(500);
				RECT rect = new RECT();
				GetWindowRect(process, out rect);
				int width = rect.Right - rect.Left;
				int height = rect.Bottom - rect.Top;
				Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
				Graphics.FromImage(bmp).CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
				return bmp;
			}
		}

		#endregion


		#region Static

		public static Rectangle SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, int tol)
		{		
			BitmapData smallData =
				smallBmp.LockBits(new Rectangle(0, 0, smallBmp.Width, smallBmp.Height),
				                  ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
			BitmapData bigData =
				bigBmp.LockBits(new Rectangle(0, 0, bigBmp.Width, bigBmp.Height),
				                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

			int smallStride = smallData.Stride;
			int bigStride = bigData.Stride;

			int bigWidth = bigBmp.Width;
			int bigHeight = bigBmp.Height - smallBmp.Height + 1;
			int smallWidth = smallBmp.Width * 3;
			int smallHeight = smallBmp.Height;

			Rectangle location = Rectangle.Empty;
			int margin = tol;
				
			unsafe
			{
				byte* pSmall = (byte*)(void*)smallData.Scan0;
				byte* pBig = (byte*)(void*)bigData.Scan0;

				int smallOffset = smallStride - smallBmp.Width * 3;
				int bigOffset = bigStride - bigBmp.Width * 3;

				bool matchFound = true;

				for (int y = 0; y < bigHeight; y++)
				{
					for (int x = 0; x < bigWidth; x++)
					{
						byte* pBigBackup = pBig;
						byte* pSmallBackup = pSmall;

						//Look for the small picture.
						for (int i = 0; i < smallHeight; i++)
						{
							int j = 0;
							matchFound = true;
							for (j = 0; j < smallWidth; j++)
							{
								//With tolerance: pSmall value should be between margins.
								int inf = pBig[0] - margin;
								int sup = pBig[0] + margin;
								if (sup < pSmall[0] || inf > pSmall[0])
								{
									matchFound = false;
									break;
								}

								pBig++;
								pSmall++;
							}

							if (!matchFound) break;

							//We restore the pointers.
							pSmall = pSmallBackup;
							pBig = pBigBackup;

							//Next rows of the small and big pictures.
							pSmall += smallStride * (1 + i);
							pBig += bigStride * (1 + i);
						}

						//If match found, we return.
						if (matchFound)
						{
							location.X = x;
							location.Y = y;
							location.Width = smallBmp.Width;
							location.Height = smallBmp.Height;
							break;
						}
						//If no match found, we restore the pointers and continue.
						else
						{
							pBig = pBigBackup;
							pSmall = pSmallBackup;
							pBig += 3;
						}
					}

					if (matchFound) break;

					pBig += bigOffset;
				}
			}

			bigBmp.UnlockBits(bigData);
			smallBmp.UnlockBits(smallData);

			return location;
		}
		
		#endregion
		
	}
}