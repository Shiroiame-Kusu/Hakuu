﻿using Serein.Base;
using System;
using System.IO;
using System.Windows;

namespace Serein
{
    public partial class App : Application
    {
        public App()
        {
            CrashInterception.Init();
            DispatcherUnhandledException += (sender, e) => CrashInterception.ShowException(e.Exception);
            if (!Directory.Exists("console") || !File.Exists("console/console.html"))
            {
                ResourcesManager.InitConsole();
                Global.FirstOpen = true;
            }
            Global.Args = Environment.GetCommandLineArgs();
            if (Global.Args.Contains("debug"))
                Global.Settings.Serein.DevelopmentTool.EnableDebug = true;
            IO.ReadAll();
            IO.StartSavingAndUpdating();
            TaskRunner.Start();
            Net.StartChecking();
        }
    }
}
