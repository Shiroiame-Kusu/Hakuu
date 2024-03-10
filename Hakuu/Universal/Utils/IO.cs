using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Hakuu.Base;
using Hakuu.Core.Generic;
using Hakuu.Extensions;
using Hakuu.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

#if WPF
using Hakuu.Windows;
#endif

namespace Hakuu.Utils
{
    internal static class IO
    {
        /// <summary>
        /// 旧文本
        /// </summary>
        private static string _oldSettings = string.Empty, _oldMembers = string.Empty;

#if !CONSOLE
        /// <summary>
        /// 保存更新设置计时器
        /// </summary>
        public static readonly Timer Timer = new(2000) { AutoReset = true };
#endif

        /// <summary>
        /// 懒惰计时器
        /// </summary>
        public static readonly Timer LazyTimer = new(60000) { AutoReset = true };

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="path">路径</param>
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 启动保存和更新设置定时器
        /// </summary>
        public static void StartSaving()
        {
#if !CONSOLE
            Timer.Elapsed += (_, _) => SaveSettings();
            Timer.Start();
#endif
            LazyTimer.Start();
        }

        /// <summary>
        /// 读取所有文件
        /// </summary>
        public static void ReadAll()
        {
            
            if (File.Exists(Path.Combine("data", "task.json")) && !File.Exists(Path.Combine("data", "schedule.json")))
            {
                ReadSchedule(Path.Combine("data", "task.json"));
                File.Delete(Path.Combine("data", "task.json"));
            }
            else
            {
                ReadSchedule();
            }
            ReadSettings();
            SaveSettings();
        }

        
        /// <summary>
        /// 读取任务文件
        /// </summary>
        /// <param name="filename">路径</param>
        public static void ReadSchedule(string? filename = null)
        {
            CreateDirectory("data");
            filename ??= Path.Combine("data", "schedule.json");
            if (!File.Exists(filename)) { return; }

            using (StreamReader streamReader = new(filename, Encoding.UTF8))
            {
                if (filename.ToLowerInvariant().EndsWith(".tsv"))
                {
                    string? line;
                    List<Schedule> list = new();
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        Schedule? schedule = Schedule.FromText(line);
                        if (schedule is not null && schedule.Check())
                        {
                            list.Add(schedule);
                        }
                    }
                    Global.Schedules = list;
                }
                else if (filename.ToLowerInvariant().EndsWith(".json"))
                {
                    string text = streamReader.ReadToEnd();
                    if (!string.IsNullOrEmpty(text))
                    {
                        JObject? jsonObject = JsonConvert.DeserializeObject<JObject>(text);
                        if ((jsonObject?["type"]?.ToString().ToUpperInvariant() == "SCHEDULE" ||
                            jsonObject?["type"]?.ToString().ToUpperInvariant() == "TASK") &&
                            jsonObject["data"] != null)
                        {
                            Global.Schedules = ((JArray)jsonObject["data"]!).ToObject<List<Schedule>>()!;
                        }
                        else if (!string.IsNullOrEmpty(filename))
                        {
                            Logger.MsgBox("不支持导入此文件", "Hakuu", 0, 48);
                        }
                    }
                }
            }
            SaveSchedule();
        }

        /// <summary>
        /// 保存任务
        /// </summary>
        public static void SaveSchedule()
        {
            CreateDirectory("data");
            JObject jsonObject = new()
            {
                { "type", "SCHEDULE" },
                { "comment", "非必要请不要直接修改文件，语法错误可能导致数据丢失" },
                { "data", JArray.FromObject(Global.Schedules) }
            };
            lock (FileLock.Schedule)
            {
                File.WriteAllText(Path.Combine("data", "schedule.json"), jsonObject.ToString());
            }
        }

        /// <summary>
        /// 读取设置
        /// </summary>
        public static void ReadSettings()
        {
            if (!Directory.Exists("settings"))
            {
                Global.FirstOpen = true;
                Directory.CreateDirectory("settings");
                File.WriteAllText(Path.Combine("settings", "Matches.json"), JsonConvert.SerializeObject(new Matches(), Formatting.Indented));
                File.WriteAllText(Path.Combine("settings", "Event.json"), JsonConvert.SerializeObject(new Event(), Formatting.Indented));
                return;
            }
            if (File.Exists(Path.Combine("settings", "Server.json")))
            {
                Global.Settings.Server = JsonConvert.DeserializeObject<Settings.Server>(File.ReadAllText(Path.Combine("settings", "Server.json"), Encoding.UTF8)) ?? new();
            }
            if (File.Exists(Path.Combine("settings", "Hakuu.json")))
            {
                Global.Settings.Hakuu = JsonConvert.DeserializeObject<Settings.Hakuu>(File.ReadAllText(Path.Combine("settings", "Hakuu.json"), Encoding.UTF8)) ?? new();
                if (!Global.Settings.Hakuu.Function.RegexForCheckingGameID.TestRegex())
                {
                    throw new NotSupportedException("“Hakuu.Function.RegexForCheckingGameID”不合法，请修改“settings/Hakuu.json”后重试");
                }
            }
            if (File.Exists(Path.Combine("settings", "Matches.json")))
            {
                Global.Settings.Matches = JsonConvert.DeserializeObject<Matches>(File.ReadAllText(Path.Combine("settings", "Matches.json"), Encoding.UTF8)) ?? new();
                File.WriteAllText(Path.Combine("settings", "Matches.json"), JsonConvert.SerializeObject(Global.Settings.Matches, Formatting.Indented));
            }
            if (File.Exists(Path.Combine("settings", "Event.json")))
            {
                Global.Settings.Event = JsonConvert.DeserializeObject<Settings.Event>(File.ReadAllText(Path.Combine("settings", "Event.json"), Encoding.UTF8)) ?? new();
                File.WriteAllText(Path.Combine("settings", "Event.json"), JsonConvert.SerializeObject(Global.Settings.Event, Formatting.Indented));
            }
        }

        /// <summary>
        /// 更新设置
        /// </summary>
        public static void UpdateSettings()
        {
            CreateDirectory("settings");
            try
            {
                if (File.Exists(Path.Combine("settings", "Matches.json")))
                {
                    Global.Settings.Matches = JsonConvert.DeserializeObject<Matches>(File.ReadAllText(Path.Combine("settings", "Matches.json"), Encoding.UTF8))!;
                }
            }
            catch (Exception e)
            {
                Logger.Output(LogType.Debug, "Fail to update Matches.json:", e);
            }
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        public static void SaveSettings()
        {
            string newSettings = JsonConvert.SerializeObject(Global.Settings);
            if (newSettings != _oldSettings)
            {
                CreateDirectory("settings");
                _oldSettings = newSettings;
                lock (FileLock.Settings)
                {
                    File.WriteAllText(Path.Combine("settings", "Server.json"), JsonConvert.SerializeObject(Global.Settings.Server, Formatting.Indented));
                    
                    File.WriteAllText(Path.Combine("settings", "Hakuu.json"), JsonConvert.SerializeObject(Global.Settings.Hakuu, Formatting.Indented));
                }
            }
        }

        /// <summary>
        /// 保存事件设置
        /// </summary>
        public static void SaveEventSetting()
        {
            CreateDirectory("settings");
            lock (Global.Settings.Event)
            {
                File.WriteAllText(Path.Combine("settings", "Event.json"), JsonConvert.SerializeObject(Global.Settings.Event, Formatting.Indented));
            }
        }

        /// <summary>
        /// 保存群组缓存
        /// </summary>
        /// <summary>
        /// 控制台日志
        /// </summary>
        /// <param name="line">行文本</param>
        public static void ConsoleLog(string line)
        {
            if (Global.Settings.Server.EnableLog)
            {
                CreateDirectory(Path.Combine("logs", "console"));
                try
                {
                    lock (FileLock.Console)
                    {
                        File.AppendAllText(
                            Path.Combine("logs", "console", $"{DateTime.Now:yyyy-MM-dd}.log"),
                            LogPreProcessing.Filter(line.TrimEnd('\n', '\r')) + Environment.NewLine,
                            Encoding.UTF8
                        );
                    }
                }
                catch (Exception e)
                {
                    Logger.Output(LogType.Debug, e);
                }
            }
        }

        
        public static class FileLock
        {
            public static readonly object
                Console = new(),
                Msg = new(),
                Crash = new(),
                Debug = new(),
                Regex = new(),
                Schedule = new(),
                GroupCache = new(),
                Member = new(),
                Settings = new(),
                PermissionGroups = new();
        }
    }
}