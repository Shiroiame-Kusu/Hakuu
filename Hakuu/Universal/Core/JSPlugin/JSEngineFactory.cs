using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jint;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Interop;
using Newtonsoft.Json;
using Hakuu.Base;
using Hakuu.Base.Motd;
using Hakuu.Core.Generic;
using Hakuu.Core.JSPlugin.Native;
using Hakuu.Core.JSPlugin.Permission;
using Hakuu.Core.Server;
using Hakuu.Extensions;
using Hakuu.Utils;
using System;
using System.Reflection;
using System.Threading;
using SystemInfoLibrary.OperatingSystem;

namespace Hakuu.Core.JSPlugin
{
    internal static class JSEngineFactory
    {
        /// <summary>
        /// 初始化JS引擎
        /// </summary>
        /// <returns>JS引擎</returns>
        public static Engine Create() => Create(true, null, null, new());

        /// <summary>
        /// 初始化JS引擎
        /// </summary>
        /// <param name="isExecuteByCommand">被命令执行</param>
        /// <param name="namespace">命名空间</param>
        /// <param name="cancellationTokenSource">取消Token</param>
        /// <returns>JS引擎</returns>
        public static Engine Create(bool isExecuteByCommand, string? @namespace, CancellationTokenSource? cancellationTokenSource, PreLoadConfig preLoadConfig)
        {
            Engine engine = new(
                new Action<Options>((cfg) =>
                {
                    cfg.CatchClrExceptions();

                    if (cancellationTokenSource is not null)
                    {
                        cfg.CancellationToken(cancellationTokenSource.Token);
                    }
                    if (!isExecuteByCommand)
                    {
                        List<Assembly> assemblies = new();
                        IEnumerable<string> assemblyNames = (preLoadConfig.Assemblies ?? Array.Empty<string>()).Concat(Global.Settings.Hakuu.Function.JSGlobalAssemblies);
                        foreach (string assemblyString in assemblyNames)
                        {
                            try
                            {
                                assemblies.Add(Assembly.Load(assemblyString));
                            }
                            catch (Exception e)
                            {
                                Utils.Logger.Output(LogType.Plugin_Warn, $"加载程序集“{assemblyString}”时出现异常：", e.Message);
                            }
                        }

                        cfg.AllowClr(assemblies.ToArray());
                        cfg.EnableModules(Path.Combine(Global.PATH, JSPluginManager.PluginPath));

                        cfg.Modules.RegisterRequire = true;
                        cfg.Interop.AllowGetType = preLoadConfig.AllowGetType;
                        cfg.Interop.AllowOperatorOverloading = preLoadConfig.AllowOperatorOverloading;
                        cfg.Interop.AllowSystemReflection = preLoadConfig.AllowSystemReflection;
                        cfg.Interop.AllowWrite = preLoadConfig.AllowWrite;
                        // cfg.StringCompilationAllowed = preLoadConfig.StringCompilationAllowed;
                        cfg.Strict = preLoadConfig.Strict;
                        cfg.Interop.ExceptionHandler = (_) => true;
                    }
                    else
                    {
                        cfg.TimeoutInterval(TimeSpan.FromMinutes(1));
                    }
                }
                ));

            engine.SetValue("Hakuu_path",
                Global.PATH);
            engine.SetValue("Hakuu_version",
                Global.VERSION);
            engine.SetValue("Hakuu_namespace",
                string.IsNullOrEmpty(@namespace) ? JsValue.Null : @namespace);
            engine.SetValue("Hakuu_debugLog",
                new Action<JsValue>((content) => Utils.Logger.Output(LogType.Debug, $"[{@namespace ?? "unknown"}]", content)));
            if (!string.IsNullOrEmpty(@namespace))
            {
                engine.SetValue("Hakuu_log",
                    new Action<JsValue>((content) => Utils.Logger.Output(LogType.Plugin_Info, $"[{@namespace}]", content)));
                engine.SetValue("Hakuu_registerPlugin",
                    new Func<string, string, string, string, string>((name, version, author, description) => JSFunc.Register(@namespace, name, version, author, description)));
                engine.SetValue("Hakuu_setListener",
                    new Func<string, JsValue, bool>((eventName, callback) => JSFunc.SetListener(@namespace, eventName, callback)));
                engine.SetValue("Hakuu_setVariable",
                    new Func<string, JsValue, bool>(JSFunc.SetVariable));
                engine.SetValue("Hakuu_export",
                    new Action<string, JsValue>(JSFunc.Export));
                engine.SetValue("Hakuu_setPreLoadConfig",
                    new Action<JsValue, JsValue, JsValue, JsValue, JsValue, JsValue, JsValue>((assemblies, allowGetType, allowOperatorOverloading, allowSystemReflection, allowWrite, strict, stringCompilationAllowed) => JSFunc.SetPreLoadConfig(@namespace, assemblies, allowGetType, allowOperatorOverloading, allowSystemReflection, allowWrite, strict, stringCompilationAllowed)));
                engine.SetValue("Hakuu_safeCall",
                    JSFunc.SafeCall);
                engine.SetValue("Hakuu_reloadFiles",
                    new Func<string, bool>((type) => JSFunc.ReloadFiles(@namespace, type)));
                engine.SetValue("setTimeout",
                    new Func<JsValue, JsValue, JsValue>((callback, interval) => JSFunc.SetTimer(@namespace, callback, interval, false)));
                engine.SetValue("setInterval",
                    new Func<JsValue, JsValue, JsValue>((callback, interval) => JSFunc.SetTimer(@namespace, callback, interval, true)));
                engine.SetValue("clearTimeout",
                    new Func<JsValue, bool>(JSFunc.ClearTimer));
                engine.SetValue("clearInterval",
                    new Func<JsValue, bool>(JSFunc.ClearTimer));
                engine.SetValue("WSClient",
                    TypeReference.CreateTypeReference(engine, typeof(WSClient)));
                engine.SetValue("MessageBus",
                    TypeReference.CreateTypeReference(engine, typeof(MessageBus)));
                engine.SetValue("Logger",
                    TypeReference.CreateTypeReference(engine, typeof(Native.Logger)));
            }
            else
            {
                engine.SetValue("Hakuu_log", JsValue.Undefined);
                engine.SetValue("Hakuu_registerPlugin", JsValue.Undefined);
                engine.SetValue("Hakuu_setListener", JsValue.Undefined);
                engine.SetValue("Hakuu_setVariable", JsValue.Undefined);
                engine.SetValue("Hakuu_export", JsValue.Undefined);
                engine.SetValue("Hakuu_setPreLoadConfig", JsValue.Undefined);
                engine.SetValue("Hakuu_safeCall", JsValue.Undefined);
                engine.SetValue("Hakuu_reloadFiles", JsValue.Undefined);
                engine.SetValue("setTimeout", JsValue.Undefined);
                engine.SetValue("setInterval", JsValue.Undefined);
                engine.SetValue("clearTimeout", JsValue.Undefined);
                engine.SetValue("clearInterval", JsValue.Undefined);
                engine.SetValue("WSClient", JsValue.Undefined);
                engine.SetValue("MessageBus", JsValue.Undefined);
                engine.SetValue("Logger", JsValue.Undefined);
                engine.SetValue("require", JsValue.Undefined);
            }
            engine.SetValue("Hakuu_getSysinfo",
                new Func<object>(() => SystemInfo.Info ?? OperatingSystemInfo.GetOperatingSystemInfo()));
            engine.SetValue("Hakuu_getCPUUsage",
                new Func<float>(() => SystemInfo.CPUUsage));
#if CONSOLE
            engine.SetValue("Hakuu_type", 0);
#elif WINFORM
            engine.SetValue("Hakuu_type", 1);
#elif WPF
            engine.SetValue("Hakuu_type", 2);
#else
            engine.SetValue("Hakuu_type", -1);
#endif
            engine.SetValue("Hakuu_typeName", Global.TYPE);
            engine.SetValue("Hakuu_startTime", Global.StartTime);
            engine.SetValue("Hakuu_getNetSpeed",
                new Func<Array>(() => new[] { SystemInfo.UploadSpeed, SystemInfo.DownloadSpeed }));
            engine.SetValue("Hakuu_runCommand",
                new Action<string>((command) => Command.Run(CommandOrigin.Javascript, command)));
            engine.SetValue("Hakuu_getSettings",
                new Func<string>(() => JsonConvert.SerializeObject(Global.Settings)));
            engine.SetValue("Hakuu_getSettingsObject",
                new Func<JsValue>(() => JsValue.FromObject(engine, Global.Settings)));
            engine.SetValue("Hakuu_getMotdpe",
                new Func<string, string?>((addr) => new Motdpe(addr).Origin));
            engine.SetValue("Hakuu_getMotdje",
                new Func<string, string?>((addr) => new Motdje(addr).Origin));
            engine.SetValue("Hakuu_startServer",
                new Func<bool>(() => ServerManager.Start(true)));
            engine.SetValue("Hakuu_stopServer",
                new Action(() => ServerManager.Stop(true)));
            engine.SetValue("Hakuu_killServer",
                new Func<bool>(() => ServerManager.Kill(true)));
            engine.SetValue("Hakuu_getServerStatus",
                new Func<bool>(() => ServerManager.Status));
            engine.SetValue("Hakuu_getServerMotd",
                new Func<JsValue>(() => JsValue.FromObject(engine, ServerManager.Motd)));
            engine.SetValue("Hakuu_sendCmd",
                new Action<string, bool>((commnad, usingUnicode) => ServerManager.InputCommand(commnad, usingUnicode, false)));
            engine.SetValue("Hakuu_getServerTime",
                new Func<string>(() => ServerManager.Time));
            engine.SetValue("Hakuu_getServerCPUUsage",
                new Func<double>(() => ServerManager.CPUUsage));
            engine.SetValue("Hakuu_getServerFile",
                new Func<string>(() => ServerManager.StartFileName));
            engine.SetValue("Hakuu_sendGroup",
                new Func<long, string, bool>((target, message) => Websocket.Send(false, message, target)));
            engine.SetValue("Hakuu_sendPrivate",
                new Func<long, string, bool>((target, message) => Websocket.Send(true, message, target)));
            engine.SetValue("Hakuu_sendTemp",
                new Func<long, long, string, bool>((groupID, userID, message) => Websocket.Send(groupID, userID, message)));
            engine.SetValue("Hakuu_sendPacket",
                new Func<string, bool>((message) => Websocket.Send(message)));
            engine.SetValue("Hakuu_getWsStatus",
                new Func<bool>(() => Websocket.Status));
            engine.SetValue("Hakuu_getSelfId",
                new Func<long?>(() => PacketHandler.SelfIdInt64));
            engine.SetValue("Hakuu_getWsStat",
                new Func<long?[]>(() => new[] { PacketHandler.MessageSentInt64, PacketHandler.MessageReceivedInt64 }));
            engine.SetValue("Hakuu_bindMember",
                new Func<long, string, bool>(Generic.Binder.Bind));
            engine.SetValue("Hakuu_unbindMember",
                new Func<long, bool>(Generic.Binder.UnBind));
            engine.SetValue("Hakuu_getID",
                new Func<string, long>(Generic.Binder.GetID));
            engine.SetValue("Hakuu_getGameID",
                new Func<long, string>(Generic.Binder.GetGameID));
            engine.SetValue("Hakuu_getGroupCache",
                new Func<JsValue>(() => JsValue.FromObject(engine, JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Member>>>(Global.GroupCache.ToJson()))));
            engine.SetValue("Hakuu_getUserInfo",
                new Func<long, long, JsValue>((groupID, userID) => Global.GroupCache.TryGetValue(groupID, out Dictionary<long, Member>? groupinfo) && groupinfo.TryGetValue(userID, out Member? member) ? JsValue.FromObject(engine, member) : JsValue.Null));
            engine.SetValue("Hakuu_getPluginList",
                new Func<JsValue>(() => JsValue.FromObject(engine, JSPluginManager.PluginDict.Values.Select(plugin => new PluginStruct(plugin)).ToArray())));
            engine.SetValue("Hakuu_getRegexes",
                new Func<JsValue>(() => JsValue.FromObject(engine, Global.RegexList.ToArray())));
            engine.SetValue("Hakuu_addRegex",
                new Func<string, int?, bool?, string, string, long[], bool>(JSFunc.AddRegex));
            engine.SetValue("Hakuu_editRegex",
                new Func<int?, string, int?, bool?, string, string, long[], bool>(JSFunc.EditRegex));
            engine.SetValue("Hakuu_removeRegex",
                new Func<int?, bool>(JSFunc.RemoveRegex));
            engine.SetValue("Hakuu_import",
                new Func<string, JsValue>((key) => JSPluginManager.VariablesExportedDict.TryGetValue(key, out JsValue? jsValue) ? jsValue : JsValue.Undefined));
            engine.SetValue("Hakuu_getPermissionGroups",
                new Func<JsValue>(() => JsValue.FromObject(engine, Global.PermissionGroups)));
            engine.SetValue("Hakuu_addPermissionGroup",
                new Func<string, PermissionGroup, bool, bool>(PermissionManager.Add));
            engine.SetValue("Hakuu_removePermissionGroup",
                new Func<string, bool>(PermissionManager.Remove));
            engine.SetValue("Hakuu_calculatePermission",
                new Func<string, long, long?, JsValue>((type, userId, groupId) => JsValue.FromObject(engine, PermissionManager.Calculate(type, userId, groupId ?? 0))));
            engine.SetValue("Hakuu_existPermissionGroup",
                new Func<string, bool>(Global.PermissionGroups.ContainsKey));
            engine.SetValue("Hakuu_setPermission",
                new Func<string, string, JsValue, bool>(PermissionManager.SetPermission));
            engine.SetValue("Motdpe", TypeReference.CreateTypeReference(engine, typeof(Motdpe)));
            engine.SetValue("Motdje", TypeReference.CreateTypeReference(engine, typeof(Motdje)));
            engine.Execute(
                @"const Hakuu = {
                    path:               Hakuu_path,
                    type:               Hakuu_type,
                    typeName:           Hakuu_typeName,
                    version:            Hakuu_version,
                    startTime:          Hakuu_startTime,
                    namespace:          Hakuu_namespace,

                    getSettings:        Hakuu_getSettings,
                    getSettingsObject:  Hakuu_getSettingsObject,
                    log:                Hakuu_log,
                    debugLog:           Hakuu_debugLog,
                    runCommand:         Hakuu_runCommand,
                    registerPlugin:     Hakuu_registerPlugin,
                    setListener:        Hakuu_setListener,
                    getPluginList:      Hakuu_getPluginList,
                    setVariable:        Hakuu_setVariable,
                    setPreLoadConfig:   Hakuu_setPreLoadConfig,
                    reloadFiles:        Hakuu_reloadFiles,
                    safeCall:           Hakuu_safeCall,

                    getRegexes:         Hakuu_getRegexes,
                    addRegex:           Hakuu_addRegex,
                    editRegex:          Hakuu_editRegex,
                    removeRegex:        Hakuu_removeRegex,

                    import:             Hakuu_import,
                    imports:            Hakuu_import,
                    export:             Hakuu_export,
                    exports:            Hakuu_export,

                    getCPUUsage:        Hakuu_getCPUUsage,
                    getNetSpeed:        Hakuu_getNetSpeed,
                    getSysInfo:         Hakuu_getSysinfo,

                    startServer:        Hakuu_startServer,
                    stopServer:         Hakuu_stopServer,
                    sendCmd:            Hakuu_sendCmd,
                    killServer:         Hakuu_killServer,
                    getServerStatus:    Hakuu_getServerStatus,
                    getServerTime:      Hakuu_getServerTime,
                    getServerCPUUsage:  Hakuu_getServerCPUUsage,
                    getServerFile:      Hakuu_getServerFile,
                    getServerMotd:      Hakuu_getServerMotd,
                    getMotdpe:          Hakuu_getMotdpe,
                    getMotdje:          Hakuu_getMotdje,

                    sendGroup:          Hakuu_sendGroup,
                    sendPrivate:        Hakuu_sendPrivate,
                    sendTemp:           Hakuu_sendTemp,
                    sendPacket:         Hakuu_sendPacket,
                    getWsStatus:        Hakuu_getWsStatus,
                    getWsStat:          Hakuu_getWsStat,
                    getSelfId:          Hakuu_getSelfId,
                    getGroupCache:      Hakuu_getGroupCache,
                    getUserInfo:        Hakuu_getUserInfo,

                    bindMember:         Hakuu_bindMember,
                    unbindMember:       Hakuu_unbindMember,
                    getID:              Hakuu_getID,
                    getGameID:          Hakuu_getGameID,

                    getPermissionGroups:    Hakuu_getPermissionGroups,
                    addPermissionGroup:     Hakuu_addPermissionGroup,
                    removePermissionGroup:  Hakuu_removePermissionGroup,
                    calculatePermission:    Hakuu_calculatePermission,
                    existPermissionGroup:   Hakuu_existPermissionGroup,
                    setPermission:          Hakuu_setPermission,
                };"
            );

            if (!string.IsNullOrEmpty(Global.Settings.Hakuu.Function.JSScriptToPreExecute))
            {
                engine.Execute(Global.Settings.Hakuu.Function.JSScriptToPreExecute!);
            }
            return engine;
        }

        /// <summary>
        /// 运行代码
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns>错误信息</returns>
        public static void Run(this Engine engine, string code, out string exceptionMessage)
        {
            try
            {
                engine.Execute(code);
                exceptionMessage = string.Empty;
            }
            catch (JavaScriptException e)
            {
                Utils.Logger.Output(LogType.Debug, e);
                exceptionMessage = $"{e.Message}\n{e.JavaScriptStackTrace}";
            }
            catch (Exception e)
            {
                Utils.Logger.Output(LogType.Debug, e);
                exceptionMessage = e.Message;
            }
        }
    }
}
