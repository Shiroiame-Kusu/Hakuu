﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Serein
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
     *     https://github.com/Zaitonn/Serein
     *  Copyright © 2022 Zaitonn. All Rights Reserved.
     *     
     */

    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!File.Exists(Global.Path + "console\\console.html"))
            {
                ResourcesManager.InitConsole();
                Global.FirstOpen = true;
            }
            Ui ui = new Ui();
            Global.Ui = ui;
            Application.Run(ui);
        }

        private static void ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Abort(e.Exception.StackTrace);
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Abort(e.ExceptionObject.ToString());
        }

        private static void Abort(string Text)
        {
            Global.Crash = true;
            if (Server.Status && Global.Settings_Server.AutoStop)
            {
                foreach (string Command in Global.Settings_Server.StopCommand.Split(';'))
                {
                    Server.InputCommand(Command);
                }
            }
            if (!Directory.Exists(Global.Path + "\\logs\\crash"))
            {
                Directory.CreateDirectory(Global.Path + "\\logs\\crash");
            }
            try
            {
                StreamWriter LogWriter = new StreamWriter(
                    Global.Path + $"\\logs\\crash\\{DateTime.Now:yyyy-MM-dd}.log",
                    true,
                    Encoding.UTF8
                    );
                LogWriter.WriteLine(
                    DateTime.Now + "  |  "
                    + Global.VERSION + "  |  " +
                    "NET" + Environment.Version.ToString() +
                    "\n" +
                    Text +
                    "\n==============================================="
                    );
                LogWriter.Flush();
                LogWriter.Close();
            }
            catch { }
            MessageBox.Show(
                "崩溃啦:(\n\n" +
                $"{Text}\n" +
                $"版本： {Global.VERSION}\n" +
                $"时间：{DateTime.Now}\n" +
                $"NET版本：{Environment.Version}\n\n\n" +
                $"崩溃日志已保存在{Global.Path + $"logs\\crash\\{DateTime.Now:yyyy-MM-dd}.log"}\n" +
                "若有必要，请在GitHub提交Issue反馈此问题",
                "Serein", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Global.Crash = false;
        }
    }
}