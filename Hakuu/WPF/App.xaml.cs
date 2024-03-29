using Hakuu.Utils;
using System.Runtime.InteropServices;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

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

    public partial class App : Application
    {
        public static string? Username = null;
        public static string? Password = null;
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeConsole();
        protected override void OnStartup(StartupEventArgs e)
        {   
            base.OnStartup(e);
            Runtime.Init();
            DispatcherUnhandledException += (_, e) => CrashInterception.ShowException(e.Exception);
            if (Global.BRANCH != "Release" || Global.BRANCH != "ReleaseCandidate")
            {
                AllocConsole(); // 打开控制台
            }
            else
            {
                FreeConsole(); // 关闭控制台
            }
        }
        /*public App()
        {
            Runtime.Init();
            DispatcherUnhandledException += (_, e) => CrashInterception.ShowException(e.Exception);

        }*/
    }
}
