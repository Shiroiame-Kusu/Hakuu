using Hakuu.Core.Generic;
using Hakuu.Core.Server;
using Hakuu.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


#if !CONSOLE
using Ookii.Dialogs.Wpf;
#endif

namespace Hakuu.Utils
{
    internal static class Runtime
    {
        /// <summary>
        /// 命令行参数
        /// </summary>
        private readonly static IList<string> _args = Environment.GetCommandLineArgs();

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            CrashInterception.Init();
            Debug.WriteLine(Global.LOGO);
            ProcessThreadCollection threadCollection = Process.GetCurrentProcess().Threads;

            Directory.SetCurrentDirectory(Global.PATH);
            IO.ReadAll();
            Task.Run(SystemInfo.Init);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#if WINFORM
            ResourcesManager.InitConsole();
#endif

#if !CONSOLE
            AppDomain.CurrentDomain.ProcessExit += (_, _) => IO.Timer.Stop();
#endif
            AppDomain.CurrentDomain.ProcessExit += (_, _) => IO.LazyTimer.Stop();

            if (_args.Contains("debug"))
            {
                Global.Settings.Hakuu.DevelopmentTool.EnableDebug = true;
            }
        }

        /// <summary>
        /// 开始核心功能调用
        /// </summary>
        public static void Start()
        {
            //TaskRunner.Start();
            Heartbeat.Start();
            Update.Init();
            IO.StartSaving();
            if (Global.FirstOpen)
            {
                ShowWelcomePage();
            }
            Task.Run(() =>
            {
                (Global.Settings.Hakuu.AutoRun.Delay > 0 ? Global.Settings.Hakuu.AutoRun.Delay : 0).ToSleep();
                if (Global.Settings.Hakuu.AutoRun.StartServer || _args.Contains("auto_start"))
                {
                    Task.Run(ServerManager.Start);
                }
            });
        }

        /// <summary>
        /// 显示欢迎页面
        /// </summary>
        public static void ShowWelcomePage()
        {
#if !CONSOLE
            TaskDialog taskDialog = new()
            {
                Buttons = {
                        new(ButtonType.Ok)
                    },
                MainInstruction = "欢迎使用Hakuu！！",
                WindowTitle = "Hakuu",
                Content = "" +
                    "如果你是第一次使用Hakuu，那么一定要仔细阅读以下内容，相信这些会对你有所帮助(๑•̀ㅂ•́)و✧\n" +
                    "◦ 官网文档：<a href=\"https://Hakuu.cc\">https://Hakuu.cc</a>\n" +
                    "◦ GitHub仓库：<a href=\"https://github.com/Shiroiame-Kusu/Hakuu\">https://github.com/Shiroiame-Kusu/Hakuu</a>\n",
                Footer = "使用此软件即视为你已阅读并同意了<a href=\"https://Hakuu.cc/docs/more/agreement\">用户协议</a>",
                FooterIcon = TaskDialogIcon.Information,
                EnableHyperlinks = true,
                ExpandedInformation = "此软件与Mojang Studio、网易、Microsoft没有从属关系\n" +
                     "Hakuu is licensed under <a href=\"https://github.com/Zaitonn/Hakuu/blob/main/LICENSE\">GPL-v3.0</a>\n" +
                     "Copyright © 2022-2023 <a href=\"https://github.com/Zaitonn\">Zaitonn</a> && <a href=\"https://github.com/Shiroiame-Kusu\">Shiroiame-Kusu</a>. All Rights Reserved.",
            };
            taskDialog.HyperlinkClicked += (_, e) => Process.Start(new ProcessStartInfo(e.Href) { UseShellExecute = true });
            taskDialog.ShowDialog();
#else       
            Logger.Output(Base.LogType.Info,
                "欢迎使用Hakuu！！\n" +
                "如果你是第一次使用Hakuu，那么一定要仔细阅读以下内容，相信这些会对你有所帮助OwO\n" +
                "◦ 官网文档：https://Hakuu.cc\n" +
                "◦ GitHub仓库：https://github.com/Shiroiame-Kusu/Hakuu\n" +
                "◦ 使用此软件即视为你已阅读并同意了用户协议（https://Hakuu.cc/docs/more/agreement）" +
                "（控制台不支持超链接，你可以复制后到浏览器中打开）");
#endif
        }
    }
}