using System;
using System.Diagnostics;

namespace BotLibNet2
{
	public class BotProcess
	{
		public BotKeyboard keyboard;
        public BotMouse mouse;
        public BotWindow window;
        public BotImage image;
        public IntPtr process;

        public BotProcess(string processName) 
        {
            Process[] processesList = Process.GetProcessesByName(processName);
            if (processesList.Length > 0)
            {
                process = processesList[0].MainWindowHandle;
                keyboard = new BotKeyboard(process);
                mouse = new BotMouse(process);
                window = new BotWindow(process);
                image = new BotImage(process);
            }
        }

        public static bool ProcessExist(string processName)
        {
            return (Process.GetProcessesByName(processName).Length > 0) ? true : false;
        }
	}
}
