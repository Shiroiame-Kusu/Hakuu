using Serein.Base;
using Serein.Settings;
using System;
using System.Collections.Generic;

namespace Serein
{
    internal static class Global
    {
        /// <summary>
        /// 程序路径
        /// </summary>
        public static readonly string PATH = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 启动时间
        /// </summary>
        public static readonly DateTime StartTime = DateTime.Now;

        /// <summary>
        /// 版本号
        /// </summary>
        public const string VERSION = "v0.4.0-A2";

        /// <summary>
        /// 类型
        /// </summary>
        public const string TYPE =
#if CONSOLE
            "console";
#elif WINFORM
            "winform";
#elif WPF
            "wpf";
#else
            "unknown";
#endif
        /// <summary>
        /// LOGO
        /// </summary>
        public const string LOGO = @"
  ██████ ▓█████  ██▀███  ▓█████  ██▓ ███▄    █ 
▒██    ▒ ▓█   ▀ ▓██ ▒ ██▒▓█   ▀ ▓██▒ ██ ▀█   █ 
░ ▓██▄   ▒███   ▓██ ░▄█ ▒▒███   ▒██▒▒██  ▀█ ██▒
  ▒   ██▒▒██  ▄ ▒██▀▀█▄  ▒██  ▄ ░██░░██▒  ▐▌██▒
▒██████▒▒░▒████▒░██▓ ▒██▒░▒████▒░██░▒██░   ▓██░
▒ ▒▓▒ ▒ ░░░ ▒░ ░░ ▒▓ ░▒▓░░░ ▒░ ░░▓  ░ ▒░   ▒ ▒ 
░ ░▒  ░ ░ ░ ░  ░  ░▒ ░ ▒░ ░ ░  ░ ▒ ░░ ░░   ░ ▒░
░  ░  ░     ░     ░░   ░    ░    ▒ ░   ░   ░ ░ 
      ░     ░  ░   ░        ░  ░ ░           ░ ";

        /// <summary>
        /// 正则项列表
        /// </summary>
        


        /// <summary>
        /// 任务项列表
        /// </summary>
        public static List<Schedule> Schedules
        {
            get => _schedules;
            set
            {
                lock (_schedules)
                {
                    _schedules = value;
                    _schedules.ForEach((schedule) => schedule.Check());
                }
            }
        }

        private static List<Schedule> _schedules = new();

        /// <summary>
        /// 成员项字典
        /// </summary>

        /// <summary>
        /// 设置项
        /// </summary>
        public static Category Settings = new();

        /// <summary>
        /// 首次开启
        /// </summary>
        public static bool FirstOpen;

        /// <summary>
        /// 编译信息
        /// </summary>
        public static readonly BuildInfo BuildInfo = new();
        public static Array? ServerFileAPI;
    }
}