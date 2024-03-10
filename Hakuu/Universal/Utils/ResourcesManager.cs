﻿#if WINFORM
using Hakuu.Properties;
using System.IO;
using System.Text;

namespace Hakuu.Utils
{
    internal static class ResourcesManager
    {
        /// <summary>
        /// 初始化控制台
        /// </summary>
        public static void InitConsole()
        {
            if (!Directory.Exists("console") || !File.Exists(Path.Combine("console", "console.html")))
            {
                ExtractConsoleFile(Resources.console_html, "console.html");
                ExtractConsoleFile(Resources.preset_css, "preset.css");
            }
        }

        /// <summary>
        /// 解压控制台文件
        /// </summary>
        /// <param name="resource">资源</param>
        /// <param name="name">文件名</param>
        private static void ExtractConsoleFile(string resource, string name)
        {
            IO.CreateDirectory("console");
            File.WriteAllText(Path.Combine("console", name), resource, Encoding.UTF8);
        }
    }
}
#endif
