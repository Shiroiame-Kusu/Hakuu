using Serein.Base.Motd;
using Serein.Base.Packets;
using Serein.Core.Server;
using Serein.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Serein.Core.Generic
{
    internal static class Command
    {
        /// <summary>
        /// 启动cmd.exe
        /// </summary>
        /// <param name="command">执行的命令</param>
        private static void StartShell(string command)
        {
            Process process = new()
            {
                StartInfo = new()
                {
                    FileName = Environment.OSVersion.Platform == PlatformID.Win32NT ? "cmd.exe" : "/bin/bash",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Global.PATH
                }
            };
            process.Start();
            StreamWriter commandWriter = new(process.StandardInput.BaseStream, Encoding.Default)
            {
                AutoFlush = true
            };
            commandWriter.WriteLine(command.TrimEnd('\r', '\n'));
            commandWriter.Close();
            Task.Run(() =>
            {
                process.WaitForExit(600000);
                if (!process.HasExited)
                {
#if NET
                    process.Kill(true);
#else
                    process.Kill();
#endif
                }
                process.Dispose();
            });
        }

        /// <summary>
        /// 处理Serein命令
        /// </summary>
        /// <param name="originType">输入类型</param>
        /// <param name="command">命令</param>
        public static void Run(Base.CommandOrigin originType, string command) => Run(originType, command, null, false);

        /// <summary>
        /// 处理Serein命令
        /// </summary>
        /// <param name="originType">输入类型</param>
        /// <param name="command">命令</param>
        /// <param name="message">数据包</param>
        /// <param name="disableMotd">禁用Motd获取</param>
        public static void Run(Base.CommandOrigin originType, string command, Message? message, bool disableMotd = false) => Run(originType, command, null, message, disableMotd);

        /// <summary>
        /// 处理Serein命令
        /// </summary>
        /// <param name="originType">输入类型</param>
        /// <param name="command">命令</param>
        /// <param name="msgMatch">消息匹配对象</param>
        public static void Run(Base.CommandOrigin originType, string command, Match msgMatch) => Run(originType, command, msgMatch, null, false);

        /// <summary>
        /// 处理Serein命令
        /// </summary>
        /// <param name="originType">输入类型</param>
        /// <param name="command">命令</param>
        /// <param name="msgMatch">消息匹配对象</param>
        /// <param name="message">数据包</param>
        /// <param name="disableMotd">禁用Motd获取</param>
        public static void Run(
            Base.CommandOrigin originType,
            string command,
            Match? msgMatch,
            Message? message,
            bool disableMotd,
            long groupId = 0
            )
        {
            Logger.Output(
                Base.LogType.Debug,
                    "命令运行",
                    $"originType:{originType} ",
                    $"command:{command}");
        }
        /// <summary>
        /// 获取命令类型
        /// </summary>
        /// <param name="command">命令</param>
        /// <returns>类型</returns>
        public static Base.CommandType GetType(string command)
        {
            if (string.IsNullOrEmpty(command) ||
                !command.Contains("|") ||
                !Regex.IsMatch(command, @"^.+?\|[\s\S]+$", RegexOptions.IgnoreCase))
            {
                return Base.CommandType.Invalid;
            }
            switch (Regex.Match(command, @"^([^\|]+?)\|").Groups[1].Value.ToLowerInvariant())
            {
                case "cmd":
                    return Base.CommandType.ExecuteShellCmd;
                case "s":
                case "server":
                    return Base.CommandType.ServerInput;
                case "s:unicode":
                case "server:unicode":
                case "s:u":
                case "server:u":
                    return Base.CommandType.ServerInputWithUnicode;
                case "g":
                case "group":
                    return Base.CommandType.SendGroupMsg;
                case "p":
                case "private":
                    return Base.CommandType.SendPrivateMsg;
                case "t":
                case "temp":
                    return Base.CommandType.SendTempMsg;
                case "b":
                case "bind":
                    return Base.CommandType.Bind;
                case "ub":
                case "unbind":
                    return Base.CommandType.Unbind;
                case "motdpe":
                    return Base.CommandType.RequestMotdpe;
                case "motdje":
                    return Base.CommandType.RequestMotdje;
                case "js":
                case "javascript":
                    return Base.CommandType.ExecuteJavascriptCodes;
                case "reload":
                    return Base.CommandType.Reload;
                case "debug":
                    return Base.CommandType.DebugOutput;
                default:
                    if (Regex.IsMatch(command, @"^(g|group):\d+\|", RegexOptions.IgnoreCase))
                    {
                        return Base.CommandType.SendGivenGroupMsg;
                    }
                    if (Regex.IsMatch(command, @"^(p|private):\d+\|", RegexOptions.IgnoreCase))
                    {
                        return Base.CommandType.SendGivenPrivateMsg;
                    }
                    if (Regex.IsMatch(command, @"^(js|javascript):[^\|]+\|", RegexOptions.IgnoreCase))
                    {
                        return Base.CommandType.ExecuteJavascriptCodesWithNamespace;
                    }
                    if (Regex.IsMatch(command, @"^(reload)\|(all|regex|schedule|member|groupcache)", RegexOptions.IgnoreCase))
                    {
                        return Base.CommandType.Reload;
                    }
                    return Base.CommandType.Invalid;
            }
        }

        /// <summary>
        /// 获取命令的值
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="match">消息匹配对象</param>
        /// <returns>值</returns>
        public static string GetValue(string command, Match? match = null)
        {
            string value = command.Substring(command.IndexOf('|') + 1);
            if (match != null)
            {
                for (int i = match.Groups.Count; i >= 0; i--)
                {
                    value = System.Text.RegularExpressions.Regex.Replace(value, $"\\${i}(?!\\d)", match.Groups[i].Value);
                }
            }
            Logger.Output(Base.LogType.Debug, value);
            return value;
        }

        public static class Patterns
        {
            public static readonly Regex CQAt = new(@"\[CQ:at,qq=(\d+)\]", RegexOptions.Compiled);

            /// <summary>
            /// 变量正则
            /// </summary>
            public static readonly Regex Variable = new(@"%(\w+)%", RegexOptions.Compiled);

            /// <summary>
            /// 游戏ID正则
            /// </summary>
            public static readonly Regex GameID = new(@"%GameID:(\d+)%", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            /// <summary>
            /// ID正则
            /// </summary>
            public static readonly Regex ID = new(@"%ID:([^%]+?)%", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
    }
}
