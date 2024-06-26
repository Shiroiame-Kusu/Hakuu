using Hakuu.Base;
using Hakuu.Base.Motd;
using Hakuu.Core.Generic;
using Hakuu.Extensions;
using Hakuu.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using RegExp = System.Text.RegularExpressions;

namespace Hakuu.Core.Server
{
    internal static class ServerManager
    {
        /// <summary>
        /// 启动文件名称
        /// </summary>
        public static string StartFileName { get; private set; } = string.Empty;

        /// <summary>
        /// 存档名称
        /// </summary>
        public static string LevelName { get; private set; } = string.Empty;

        /// <summary>
        /// 难度
        /// </summary>
        public static string Difficulty { get; private set; } = string.Empty;

        /// <summary>
        /// 临时行储存
        /// </summary>
        private static string _tempLine = string.Empty;

        /// <summary>
        /// 重启
        /// </summary>
        private static bool _restart;

        /// <summary>
        /// 由用户关闭服务器
        /// </summary>
        private static bool _isStoppedByUser;

        /// <summary>
        /// 服务器状态
        /// </summary>
        public static bool Status => !_serverProcess?.HasExited ?? false;

        /// <summary>
        /// CPU使用率
        /// </summary>
        public static double CPUUsage { get; private set; }

        /// <summary>
        /// Motd对象
        /// </summary>
        public static Motd? Motd { get; private set; }
        public static bool isJEServer { get; private set; }
        /// <summary>
        /// 更新计时器
        /// </summary>
        private static Timer? _updateTimer;

        /// <summary>
        /// 当前CPU时间
        /// </summary>
        private static TimeSpan _prevProcessCpuTime = TimeSpan.Zero;

        /// <summary>
        /// 服务器进程
        /// </summary>
        private static Process? _serverProcess;

        /// <summary>
        /// 输入流写入
        /// </summary>
        private static StreamWriter? _inputWriter;

        /// <summary>
        /// 命令历史记录列表下标
        /// </summary>
        public static int CommandHistoryIndex;

        /// <summary>
        /// 命令历史记录
        /// </summary>
        public static readonly List<string> CommandHistory = new();

        public static string? JEOptimizationArguments { get; set; }
        public static string? JEStartMaxRam { get; set; }
        public static string? JavaVersion { get; set; }
        public static int? JavaVersionNumber { get; set; }
        public static bool AbleToUse_incubator_vector { get; set; }
        public static string? Use_incubator_vector { get; set; }
        public static bool? StartResult;

        /// <summary>
        /// 编码列表
        /// </summary>
        private static readonly Encoding[] _encodings =
        {
            new UTF8Encoding(false),
            new UTF8Encoding(true),
            new UnicodeEncoding(false, false),
            new UnicodeEncoding(true, false),
            Encoding.UTF32,
            Encoding.ASCII,
            Encoding.GetEncoding("GBK")
        };

#if CONSOLE
        /// <summary>
        /// 上一次执行强制结束时间
        /// </summary>
        private static DateTime _lastKillTime = DateTime.Now;
#endif

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <returns>启动结果</returns>
        public static bool Start() => Start(false);

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="quiet">静处理</param>
        /// <returns>启动结果</returns>
        public static bool Start(bool quiet)
        {

            if (Status)
            {
                if (!quiet)
                {
                    Logger.MsgBox("服务器已在运行中", "Hakuu", 0, 48);
                }
            }
            else if (string.IsNullOrEmpty(Global.Settings.Server.Path) || string.IsNullOrWhiteSpace(Global.Settings.Server.Path))
            {
                if (!quiet)
                {
                    Logger.MsgBox("启动路径为空", "Hakuu", 0, 48);
                }
            }
            else if (!File.Exists(Global.Settings.Server.Path))
            {
                if (!quiet)
                {
                    Logger.MsgBox($"启动文件\"{Global.Settings.Server.Path}\"未找到", "Hakuu", 0, 48);
                }
            }
            else if (!quiet && Path.GetFileName(Global.Settings.Server.Path).Contains("Hakuu") && !Logger.MsgBox("禁止禁止禁止套娃（）", "Hakuu", 1, 48))
            {
                return false;
            }
            else
            {
#if CONSOLE
                Logger.Output(LogType.Server_Notice, "若要执行Hakuu指令，请使用\"Hakuu <你的指令>\"代替原输入方式\n");
#else
                Logger.Output(LogType.Server_Clear);
#endif
                Logger.Output(LogType.Server_Notice, "启动中");

                string ServerType = Global.Settings.Server.Path.Substring(Global.Settings.Server.Path.Length - 3);
                if (string.IsNullOrEmpty(Global.Settings.Server.MaxRAM))
                {
                    PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                    float AvailableRAM = ramCounter.NextValue();
                    double AutoSetRAM = Math.Round(AvailableRAM * 0.8, 0);
                    
                    JEStartMaxRam = AutoSetRAM.ToString();
                    Logger.MsgBox("未设置最大内存\n已自动为您设置启动内存为 " + JEStartMaxRam + "M", "Hakuu", 0, 48);

                }
                else
                {
                    JEStartMaxRam = Global.Settings.Server.MaxRAM;
                }
                string? JavaPathSettings = Global.Settings.Server.JavaPath;
                if (string.IsNullOrEmpty(JavaPathSettings) == true)
                {
                    CurrentJavaPath = "java";
                }
                else
                {
                    CurrentJavaPath = Global.Settings.Server.JavaPath;
                }
                try
                {
                    ProcessStartInfo defaultJava = new ProcessStartInfo();
                    defaultJava.FileName = CurrentJavaPath;
                    defaultJava.Arguments = " -version";
                    defaultJava.RedirectStandardError = true;
                    defaultJava.UseShellExecute = false;
                    defaultJava.CreateNoWindow = true;

                    Process? pr = Process.Start(defaultJava);
                    JavaVersion = pr?.StandardError?.ReadLine()?.Split(' ')[2].Replace("\"", "");
                    JavaVersionNumber = int.Parse(JavaVersion?.Substring(0, 2));

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception is " + ex.Message);
                }
                if (JavaVersionNumber == 16 || JavaVersionNumber == 17 || JavaVersionNumber == 18 || JavaVersionNumber == 19)
                {
                    AbleToUse_incubator_vector = true;
                    Use_incubator_vector = " --add-modules=jdk.incubator.vector";
                }
                else
                {
                    Use_incubator_vector = "";
                }

                #region 主变量初始化
                if (ServerType == "jar")
                {   
                    PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                    float AvailableRAM = ramCounter.NextValue();
                    if (Global.Settings.Server.AutoJVMOptimization)
                    {
                        if (AvailableRAM >= 12288)
                        {
                            JEOptimizationArguments = Use_incubator_vector + " -XX:+UseG1GC " +
                                "-XX:+ParallelRefProcEnabled -XX:MaxGCPauseMillis=200 -XX:+UnlockExperimentalVMOptions " +
                                "-XX:+DisableExplicitGC -XX:+AlwaysPreTouch -XX:G1HeapWastePercent=5 -XX:G1MixedGCCountTarget=4 " +
                                "-XX:InitiatingHeapOccupancyPercent=15 -XX:G1MixedGCLiveThresholdPercent=90 -XX:G1RSetUpdatingPauseTimePercent=5 " +
                                "-XX:SurvivorRatio=32 -XX:+PerfDisableSharedMem -XX:MaxTenuringThreshold=1 -Dusing.aikars.flags=https://mcflags.emc.gs " +
                                "-Daikars.new.flags=true -XX:G1NewSizePercent=40 -XX:G1MaxNewSizePercent=50 -XX:G1HeapRegionSize=16M -XX:G1ReservePercent=15 ";
                        }
                        else
                        {
                            JEOptimizationArguments = Use_incubator_vector + " -XX:+UseG1GC " +
                                "-XX:+ParallelRefProcEnabled -XX:MaxGCPauseMillis=200 -XX:+UnlockExperimentalVMOptions " +
                                "-XX:+DisableExplicitGC -XX:+AlwaysPreTouch -XX:G1HeapWastePercent=5 -XX:G1MixedGCCountTarget=4 " +
                                "-XX:InitiatingHeapOccupancyPercent=15 -XX:G1MixedGCLiveThresholdPercent=90 -XX:G1RSetUpdatingPauseTimePercent=5 " +
                                "-XX:SurvivorRatio=32 -XX:+PerfDisableSharedMem -XX:MaxTenuringThreshold=1 -Dusing.aikars.flags=https://mcflags.emc.gs " +
                                "-Daikars.new.flags=true -XX:G1NewSizePercent=30 -XX:G1MaxNewSizePercent=40 -XX:G1HeapRegionSize=8M -XX:G1ReservePercent=20 ";
                        }
                    }
                    else
                    {
                        JEOptimizationArguments = Use_incubator_vector;
                    }

                    ProcessStartInfo ServerStartInfo = new ProcessStartInfo(Global.Settings.Server.Path)
                    {

                        FileName = CurrentJavaPath,
                        Arguments = " -jar -Xmx" + JEStartMaxRam + "M " + JEOptimizationArguments + Global.Settings.Server.Path + " --nogui",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        StandardOutputEncoding = _encodings[Global.Settings.Server.OutputEncoding],
                        WorkingDirectory = Path.GetDirectoryName(Global.Settings.Server.Path),
                    };
                    Console.WriteLine(JEStartMaxRam + "M " + JEOptimizationArguments);
                    StartInfo = ServerStartInfo;
                    File.WriteAllText(Path.GetDirectoryName(Global.Settings.Server.Path) + "\\eula.txt", "eula=true");
                }
                else
                {
                    ProcessStartInfo ServerStartInfo = new ProcessStartInfo(Global.Settings.Server.Path)
                    {

                        FileName = Global.Settings.Server.Path,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        StandardOutputEncoding = _encodings[Global.Settings.Server.OutputEncoding],
                        WorkingDirectory = Path.GetDirectoryName(Global.Settings.Server.Path)
                    };

                    StartInfo = ServerStartInfo;
                }
                Logger.Output(LogType.Server_Notice, $"使用的jvm启动参数: {JEOptimizationArguments}\n");
                _serverProcess = Process.Start(StartInfo);
                _serverProcess!.EnableRaisingEvents = true;
                _serverProcess.Exited += (_, _) => CloseAll();
                _inputWriter = new(
                    _serverProcess.StandardInput.BaseStream,
                    _encodings[Global.Settings.Server.InputEncoding]
                   )
                {
                    AutoFlush = true,
                    NewLine = string.IsNullOrEmpty(Global.Settings.Server.LineTerminator) ? Environment.NewLine : Global.Settings.Server.LineTerminator.Replace("\\n", "\n").Replace("\\r", "\r")
                };
                _serverProcess.BeginOutputReadLine();
                _serverProcess.OutputDataReceived += SortOutputHandler;


                #region 变量初始化
                _restart = false;
                _isStoppedByUser = false;
                LevelName = string.Empty;
                Difficulty = string.Empty;
                _tempLine = string.Empty;
                CommandHistory.Clear();
                //StartFileName = Path.GetFileName(Global.Settings.Server.Path);
                StartFileName = ServerType == "jar" ? Path.GetFullPath(Global.Settings.Server.Path) : Path.GetFileName(Global.Settings.Server.Path);
                _prevProcessCpuTime = TimeSpan.Zero;
                #endregion

                Task.Run(() =>
                {
                    EventTrigger.Trigger(EventType.ServerStart);
                    _updateTimer = new(2000) { AutoReset = true };
                    _updateTimer.Elapsed += (_, _) => UpdateInfo();
                    _updateTimer.Start();
                });
                #endregion
            }
            return false;
        }
        public static void MainProcess()
        {

        }
        /// <summary>
        /// 关闭服务器
        /// </summary>
        public static void Stop() => Stop(false);

        /// <summary>
        /// 关闭服务器
        /// </summary>
        /// <param name="quiet">静处理</param>
        public static void Stop(bool quiet)
        {
            if (Status)
            {
                foreach (string command in Global.Settings.Server.StopCommands)
                {
                    if (!string.IsNullOrEmpty(command))
                    {
                        InputCommand(command);
                    }
                }
            }
            else if (_restart)
            {
                _restart = false;
            }
            else if (!quiet)
            {
                Logger.MsgBox("服务器不在运行中", "Hakuu", 0, 48);
            }
        }

        /// <summary>
        /// 强制结束服务器
        /// </summary>
        /// <returns>强制结束结果</returns>
        public static bool Kill() => Kill(false);

        /// <summary>
        /// 强制结束服务器
        /// </summary>
        /// <param name="quiet">静处理</param>
        /// <returns>强制结束结果</returns>
        public static bool Kill(bool quiet)
        {
            if (quiet)
            {
                if (Status)
                {
                    try
                    {
#if NET6_0
                        _serverProcess?.Kill(true);
#else
                        _serverProcess?.Kill();
#endif
                        _isStoppedByUser = true;
                        _restart = false;
                        return true;
                    }
                    catch (Exception e)
                    {
                        Logger.Output(LogType.Debug, e);
                    }
                }
            }
            else if (!Status)
            {
                Logger.MsgBox("服务器不在运行中", "Hakuu", 0, 48);
            }
#if CONSOLE
            else
            {
                DateTime nowTime = DateTime.Now;
                if ((nowTime - _lastKillTime).TotalSeconds < 2)
                {
                    _lastKillTime = nowTime;
                    try
                    {
#if NET6_0
                        _serverProcess?.Kill(true);
#else
                        _serverProcess?.Kill();
#endif
                        _isStoppedByUser = true;
                        _restart = false;
                        return true;
                    }
                    catch (Exception e)
                    {
                        Logger.Output(LogType.Warn, "强制结束失败：\n", e.Message);
                        Logger.Output(LogType.Debug, e);
                    }
                }
                else
                {
                    _lastKillTime = nowTime;
                    Logger.Output(LogType.Warn, "请在2s内再次执行强制结束服务器（Ctrl+C 或输入“Hakuu s k”）以确认此操作");
                }
            }
#else
            else if (Logger.MsgBox("确定结束进程吗？\n此操作可能导致存档损坏等问题", "Hakuu", 1, 48)
#if !NET6_0
                 && (
                    !StartFileName.ToLowerInvariant().EndsWith(".bat") || (
                    StartFileName.ToLowerInvariant().EndsWith(".bat") &&
                    Logger.MsgBox("由于启动文件为批处理文件（*.bat），\n强制结束进程功能可能不一定有效\n是否继续？", "Hakuu", 1, 48)))
#endif
                )
            {
                try
                {
#if NET6_0
                    _serverProcess?.Kill(true);
#else
                    _serverProcess?.Kill();
#endif
                    _isStoppedByUser = true;
                    _restart = false;
                    return true;
                }
                catch (Exception e)
                {
                    Logger.MsgBox("强制结束失败\n" + e.Message, "Hakuu", 0, 16);
                    return false;
                }
            }
#endif
            return false;
        }

        /// <summary>
        /// 输入行
        /// </summary>
        /// <param name="line">行</param>
        public static void InputCommand(string command) => InputCommand(command, false, false);

        /// <summary>
        /// 输入行
        /// </summary>
        /// <param name="command">行</param>
        /// <param name="usingUnicode">使用Unicode</param>
        /// <param name="isFromCommand">来自命令</param>
        public static void InputCommand(string command, bool usingUnicode, bool isFromCommand)
        {
            if (Status)
            {
                command = command.TrimEnd('\n').Replace("\n", "\\n").Replace("\r", "\\n");
                string line_copy = command;
                if (CommandHistory.Count > 50)
                {
                    CommandHistory.RemoveRange(0, CommandHistory.Count - 50);
                }
                if (
                    (
                        CommandHistory.Count > 0 &&
                        CommandHistory[CommandHistory.Count - 1] != command || // 与最后一项重复
                        CommandHistory.Count == 0) &&
                        !isFromCommand && // 通过Hakuu命令执行的不计入
                        !string.IsNullOrEmpty(command)) // 为空不计入
                { 
                    CommandHistory.Add(command);
                }
                CommandHistoryIndex = CommandHistory.Count;
#if !CONSOLE
                if (Global.Settings.Server.EnableOutputCommand)
                {
                    Logger.Output(LogType.Server_Input, $">{command}");
                }
#endif
                if (usingUnicode || Global.Settings.Server.EnableUnicode)
                {
                    line_copy = ConvertToUnicode(line_copy);
                }
                _inputWriter?.WriteLine(line_copy);
                IO.ConsoleLog(">" + command);
            }
            else if (isFromCommand)
            {
                if (command.Trim().ToLowerInvariant() == "start")
                {
                    Start(false);
                }
                else if (command.Trim().ToLowerInvariant() == "stop")
                {
                    _restart = false;
                }
            }
        }

        /// <summary>
        /// 输出处理
        /// /// </summary>
        private static void SortOutputHandler(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {   

                string lineFiltered = LogPreProcessing.Filter(e.Data);
                Console.WriteLine(lineFiltered);
                if (string.IsNullOrEmpty(LevelName) && RegExp.Regex.IsMatch(lineFiltered, Global.Settings.Matches.LevelName, RegExp.RegexOptions.IgnoreCase))
                {
                    LevelName = RegExp.Regex.Match(lineFiltered, Global.Settings.Matches.LevelName, RegExp.RegexOptions.IgnoreCase).Groups[1].Value.Trim();
                }
                else if (string.IsNullOrEmpty(Difficulty) && RegExp.Regex.IsMatch(lineFiltered, Global.Settings.Matches.Difficulty, RegExp.RegexOptions.IgnoreCase))
                {
                    Difficulty = RegExp.Regex.Match(lineFiltered, Global.Settings.Matches.Difficulty, RegExp.RegexOptions.IgnoreCase).Groups[1].Value.Trim();
                }
                bool excluded = false;
                foreach (string exp1 in Global.Settings.Server.ExcludedOutputs)
                {
                    if (exp1.TryParse(RegExp.RegexOptions.IgnoreCase, out RegExp.Regex? regex) && regex!.IsMatch(lineFiltered))
                    {
                        excluded = true;
                        break;
                    }
                }
                bool interdicted = false;
                if (!excluded && !interdicted)
                {
                    Logger.Output(LogType.Server_Output, e.Data);
                    bool isMuiltLinesMode = false;
                    foreach (string exp2 in Global.Settings.Matches.MuiltLines)
                    {
                        if (exp2.TryParse(RegExp.RegexOptions.IgnoreCase, out RegExp.Regex? regex) && regex!.IsMatch(lineFiltered))
                        {
                            _tempLine = lineFiltered.Trim('\r', '\n');
                            isMuiltLinesMode = true;
                            break;
                        }
                    }
                    if (!isMuiltLinesMode)
                    {
                        if (!string.IsNullOrEmpty(_tempLine))
                        {
                            string tempLine = _tempLine + "\n" + lineFiltered;
                            _tempLine = string.Empty;
                        }
                        else
                        {

                        }
                    }
                }
                IO.ConsoleLog(lineFiltered);
            }
        }

        /// <summary>
        /// 关闭相关服务
        /// </summary>
        private static void CloseAll()
        {
            if (_inputWriter is null || _serverProcess is null || _updateTimer is null)
            {
                return;
            }
            _inputWriter.Close();
            _inputWriter.Dispose();
            Logger.Output(LogType.Server_Output, "");
            _updateTimer.Stop();
            if (!_isStoppedByUser && _serverProcess.ExitCode != 0)
            {
                Logger.Output(LogType.Server_Notice, $"进程疑似非正常退出（返回：{_serverProcess.ExitCode}）");
                _restart = Global.Settings.Server.EnableRestart;
                EventTrigger.Trigger(EventType.ServerExitUnexpectedly);
            }
            else
            {
                Logger.Output(LogType.Server_Notice, $"进程已退出（返回：{_serverProcess.ExitCode}）");
                EventTrigger.Trigger(EventType.ServerStop);
            }
            if (_restart)
            {
                Task.Factory.StartNew(RestartTimer);
            }
            LevelName = string.Empty;
            Difficulty = string.Empty;
        }

        /// <summary>
        /// 重启请求
        /// </summary>
        public static void RequestRestart()
        {
            _restart = true;
            Stop();
        }

        /// <summary>
        /// 重启计时器
        /// </summary>
        private static void RestartTimer()
        {
            Logger.Output(LogType.Server_Notice,
                $"服务器将在5s后（{DateTime.Now.AddSeconds(5):T}）重新启动"
                );
#if CONSOLE
            Logger.Output(LogType.Server_Notice, "你可以输入\"stop\"来取消这次重启");
#else
            Logger.Output(LogType.Server_Notice, "你可以按下停止按钮来取消这次重启");
#endif
            for (int i = 0; i < 10; i++)
            {
                500.ToSleep();
                if (!_restart)
                {
                    break;
                }
            }
            if (_restart)
            {
                Start(true);
            }
            else
            {
                Logger.Output(LogType.Server_Notice, "重启已取消");
            }
        }

        /// <summary>
        /// 获取CPU占用
        /// </summary>
        public static void UpdateInfo()
        {
            if (Status && _serverProcess is not null)
            {
                CPUUsage = (_serverProcess.TotalProcessorTime - _prevProcessCpuTime).TotalMilliseconds / 2000 / Environment.ProcessorCount * 100;
                _prevProcessCpuTime = _serverProcess.TotalProcessorTime;
                if (CPUUsage > 100)
                {
                    CPUUsage = 100;
                }
                if (Global.Settings.Server.Type == 1)
                {
                    Motd = new Motdpe(Global.Settings.Server.Port);
                }
                else if (Global.Settings.Server.Type == 2)
                {
                    Motd = new Motdje(Global.Settings.Server.Port);
                }
            }
            else
            {
                Motd = null;
            }
        }

        /// <summary>
        /// 获取运行时间
        /// </summary>
        /// <returns>运行时间</returns>
        public static string Time => Status && _serverProcess is not null ? (DateTime.Now - _serverProcess.StartTime).ToCustomString() : string.Empty;

        public static ProcessStartInfo? StartInfo { get; private set; }
        public static string? CurrentJavaPath { get; private set; }

        /// <summary>
        /// Unicode转换
        /// </summary>
        /// <param name="text">输入</param>
        /// <returns>输出字符串</returns>
        private static string ConvertToUnicode(string text)
        {
            StringBuilder stringBuilder = new();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] < 127)
                {
                    stringBuilder.Append(text[i].ToString());
                }
                else
                {
                    stringBuilder.Append(string.Format("\\u{0:x4}", (int)text[i]));
                }
            }
            return stringBuilder.ToString();
        }
    }
}
