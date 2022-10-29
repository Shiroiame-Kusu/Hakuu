﻿using Serein.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Serein.Server
{
    internal static class PluginManager
    {
        public static string BasePath = string.Empty;

        public static bool Available
        {
            get
            {
                if (ServerManager.Status)
                {
                    Logger.MsgBox("服务器仍在运行中", "Serein", 0, 48);
                    return false;
                }
                else if (string.IsNullOrEmpty(BasePath) && Directory.Exists(BasePath))
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// 获取插件列表
        /// </summary>
        /// <returns>插件列表</returns>
        public static string[] Get()
        {
            if (File.Exists(Global.Settings.Server.Path))
            {
                if (Directory.Exists(Path.GetDirectoryName(Global.Settings.Server.Path) + "\\plugin"))
                    BasePath = Path.GetDirectoryName(Global.Settings.Server.Path) + "\\plugin";
                else if (Directory.Exists(Path.GetDirectoryName(Global.Settings.Server.Path) + "\\plugins"))
                    BasePath = Path.GetDirectoryName(Global.Settings.Server.Path) + "\\plugins";
                else if (Directory.Exists(Path.GetDirectoryName(Global.Settings.Server.Path) + "\\mod"))
                    BasePath = Path.GetDirectoryName(Global.Settings.Server.Path) + "\\mod";
                else if (Directory.Exists(Path.GetDirectoryName(Global.Settings.Server.Path) + "\\mods"))
                    BasePath = Path.GetDirectoryName(Global.Settings.Server.Path) + "\\mods";
                else
                {
                    BasePath = string.Empty;
                    return null;
                }
                Logger.Out(LogType.Debug, "[PluginManager:Get()]", BasePath);
                if (!string.IsNullOrWhiteSpace(BasePath))
                {
                    string[] Files = Directory.GetFiles(BasePath, "*", SearchOption.TopDirectoryOnly);
                    return Files;
                }
            }
            return null;
        }

        /// <summary>
        /// 导入插件
        /// </summary>
        /// <param name="Files">文件列表</param>
        public static void Add(IList<string> Files)
        {
            foreach (string FileName in Files)
            {
                try
                {
                    File.Copy(FileName, BasePath + "\\" + Path.GetFileName(FileName));
                }
                catch (Exception e)
                {
                    Logger.MsgBox($"文件\"{FileName}\"删除失败\n{e.Message}", "Serein", 0, 48);
                    break;
                }
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="Files">文件列表</param>
        public static void Remove(List<string> Files)
        {
            if (Available)
            {
                if (Files.Count <= 0)
                    Logger.Out(LogType.Debug, "[PluginManager:Remove()]", "数据不合法");
                else if (Logger.MsgBox($"确定删除\"{Files[0]}\"{(Files.Count > 1 ? $"等{Files.Count}个文件" : string.Empty)}？\n它将会永远失去！（真的很久！）", "Serein", 1, 48))
                {
                    foreach (string FileName in Files)
                    {
                        try
                        {
                            File.Delete(FileName);
                        }
                        catch (Exception e)
                        {
                            Logger.Out(LogType.Debug, "[PluginManager:Remove()]", e);
                            Logger.MsgBox(
                                $"文件\"{FileName}\"删除失败\n{e.Message}", "Serein",
                                0, 48
                            );
                            break;
                        }
                    }
                }
            }
        }

        public static void Disable(List<string> Files)
        {
            foreach (string FileName in Files)
            {
                try
                {
                    if (FileName.ToLower().EndsWith(".lock"))
                        continue;
                    File.Move(FileName, FileName + ".lock");
                }
                catch (Exception Exp)
                {
                    Logger.Out(LogType.Debug, "[PluginManager:Disable()]", FileName);
                    Logger.MsgBox(
                        $"文件\"{FileName}\"禁用失败\n" +
                        $"{Exp.Message}", "Serein",
                        0, 48
                        );
                    break;
                }
            }
        }

        public static void Enable(List<string> Files)
        {
            foreach (string FileName in Files)
            {
                try
                {
                    File.Move(FileName, System.Text.RegularExpressions.Regex.Replace(FileName, @"\.lock", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase));
                }
                catch (Exception Exp)
                {
                    Logger.MsgBox(
                        $"文件\"{FileName}\"启用失败\n" +
                        $"{Exp.Message}", "Serein",
                        0, 48
                        );
                    break;
                }
            }
        }

        public static string GetRelativeUri(string File)
        {
            return WebUtility.UrlDecode(new Uri(BasePath).MakeRelativeUri(new Uri(File)).OriginalString);
        }

        public static string GetAbsoluteUri(string File)
        {
            return Path.Combine(Directory.GetParent(BasePath).FullName, File).Replace('/', '\\').TrimStart('\u202a');
        }

        public static void OpenFolder(string Path = null)
            => Process.Start(new ProcessStartInfo("Explorer.exe")
            {
                Arguments = !string.IsNullOrEmpty(Path)
                    ? $"/e,/select,\"{Path}\""
                    : $"/e,\"{BasePath}\""
            });
    }
}
