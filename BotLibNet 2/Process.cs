using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static bool processExist(string processName)
        {
            Process[] processesList = Process.GetProcessesByName(processName);
            if (processesList.Length > 0)
                return true;
            else
                return false;
        }

    }
}