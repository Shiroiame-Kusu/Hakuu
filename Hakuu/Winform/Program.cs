using Hakuu.Utils;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Hakuu
{
    /*
     *  ____ 
     * /\  _`\                        __            
     * \ \,\L\_\     __   _ __    __ /\_\    ___    
     *  \/_\__ \   /'__`\/\`'__\/'__`\/\ \ /' _ `\  
     *    /\ \L\ \/\  __/\ \ \//\  __/\ \ \/\ \/\ \ 
     *    \ `\____\ \____\\ \_\\ \____\\ \_\ \_\ \_\
     *     \/_____/\/____/ \/_/ \/____/ \/_/\/_/\/_/
     *     
     *     https://github.com/Zaitonn/Hakuu
     *  Copyright © 2022 Zaitonn. All Rights Reserved.
     *     
     */

    static class Program
    {
        public static Ui.Ui? Ui;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Runtime.Init();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (_, e) => CrashInterception.ShowException(e.Exception);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Global.Settings.Hakuu.DPIAware && Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }
            Ui = new();
            Application.Run(Ui);
        }

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
