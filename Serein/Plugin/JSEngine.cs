﻿using Jint;
using Serein.Base;
using Serein.Items.Motd;
using Serein.Server;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Serein.Plugin
{
    internal class JSEngine
    {
        public static Engine engine = new Engine();

        public static void Setup()
        {
            engine = Init(
            new Engine(cfg => cfg.AllowClr(
                typeof(File).Assembly,
                typeof(Path).Assembly,
                typeof(Directory).Assembly,
                typeof(DirectoryInfo).Assembly,
                typeof(StreamReader).Assembly,
                typeof(StreamWriter).Assembly,
                typeof(Encoding).Assembly,
                typeof(Process).Assembly,
                typeof(ProcessStartInfo).Assembly
                )
            .CatchClrExceptions()
            ));
        }
        /// <summary>
        /// 初始化JS引擎
        /// </summary>
        /// <returns>JS引擎</returns>
        public static Engine Init(Engine engine)
        {
            engine.SetValue("Serein_SystemInfo", new Func<string, string>((Type) =>
            {
                switch (Type.ToLower())
                {
                    case "os":
                        return SystemInfo.OS;
                    case "net":
                        return SystemInfo.NET;
                    case "cpuname":
                        return SystemInfo.CPUName;
                    case "cpupercentage":
                        return SystemInfo.CPUPercentage;
                    case "usedram":
                        return SystemInfo.UsedRAM;
                    case "totalram":
                        return SystemInfo.TotalRAM;
                    case "rampercentage":
                        return SystemInfo.RAMPercentage;
                    default:
                        return string.Empty;
                }
            }));
            engine.SetValue("Serein_Log", new Action<object>((Content) => { Global.Ui.SereinPluginsWebBrowser_Invoke(Log.EscapeLog((Content ?? "").ToString())); }));
            engine.SetValue("Serein_Command_Run", new Action<string>((command) => Command.Run(5, command)));
            engine.SetValue("Serein_Global_Path", Global.Path);
            engine.SetValue("Serein_Global_Version", Global.VERSION);
            engine.SetValue("Serein_Global_Debug", new Action<object>(Global.Debug));
            engine.SetValue("Serein_Global_Settings", new Func<string>(() => JsonConvert.SerializeObject(Global.Settings)));
            engine.SetValue("Serein_Plugin_JSFunc_Register", new Func<string, string, string, string, bool>(JSFunc.Register));
            engine.SetValue("Serein_Plugin_JSFunc_RegisterCommand", new Func<string, Delegate, bool>(JSFunc.RegisterCommand));
            engine.SetValue("Serein_Plugin_JSFunc_SetListener", new Func<string, Delegate, bool>(JSFunc.SetListener));
            engine.SetValue("Serein_Motdpe", new Func<string, string>((IP) => { return new Motdpe(IP).Original; }));
            engine.SetValue("Serein_Motdje", new Func<string, string>((IP) => { return new Motdje(IP).Original; }));
            engine.SetValue("Serein_ServerManager_Start", new Func<bool>(() => ServerManager.Start(true)));
            engine.SetValue("Serein_ServerManager_Stop", new Action(ServerManager.Stop));
            engine.SetValue("Serein_ServerManager_Kill", new Func<bool>(() => ServerManager.Kill(true)));
            engine.SetValue("Serein_ServerManager_Status", new Func<bool>(() => { return ServerManager.Status; }));
            engine.SetValue("Serein_ServerManager_Stop", new Action<string, bool>((Commnad, Unicode) => ServerManager.InputCommand(Commnad, Unicode: Unicode)));
            engine.SetValue("Serein_ServerManager_GetTime", new Func<string>(() => ServerManager.GetTime()));
            engine.SetValue("Serein_ServerManager_GetCPUPersent", new Func<string>(() => { return ServerManager.CPUPersent.ToString("N2"); }));
            engine.SetValue("Serein_Websocket_SendGroup", new Func<long, string, bool>((Target, Message) => { return (Websocket.Send(false, Message, Target)); }));
            engine.SetValue("Serein_Websocket_SendPrivate", new Func<long, string, bool>((Target, Message) => { return (Websocket.Send(true, Message, Target)); }));
            engine.SetValue("Serein_Websocket_SendPackage", new Func<string, bool>((Json) => { if (Websocket.Status) { Websocket.webSocket.Send(Json); } return Websocket.Status; }));
            engine.SetValue("Serein_Websocket_Status", new Func<bool>(() => { return Websocket.Status; }));
            engine.SetValue("Serein_Member_Bind", new Func<long, string, bool>(Members.Bind));
            engine.SetValue("Serein_Member_UnBind", new Func<long, bool>(Members.UnBind));
            engine.SetValue("Serein_Member_GetID", new Func<string, long>(Members.GetID));
            engine.SetValue("Serein_Member_GetGameID", new Func<long, string>(Members.GetGameID));
            engine.Execute("var serein={" +
                "log:Serein_Log," +
                "path:Serein_Global_Path," +
                "versions:Serein_Global_Version," +
                "getSettings:Serein_Global_Settings," +
                "debugLog:Serein_Global_Debug," +
                "runCommand:Serein_Command_Run," +
                "registerPlugin:Serein_Plugin_JSFunc_Register," +
                "registerCommand:Serein_Plugin_JSFunc_RegisterCommand," +
                "setListener:Serein_Plugin_JSFunc_SetListener," +
                "getSysInfo:Serein_SystemInfo," +
                "getMotdpe:Serein_Motdpe," +
                "getMotdje:Serein_Motdje," +
                "startServer:Serein_ServerManager_Start," +
                "stopServer:Serein_ServerManager_Stop," +
                "killServer:Serein_ServerManager_Kill," +
                "getServerStatus:Serein_ServerManager_Status," +
                "getServerTime:Serein_ServerManager_GetTime," +
                "getServerCPUPersent:Serein_ServerManager_GetCPUPersent," +
                "sendGroup:Serein_Websocket_SendGroup," +
                "sendPrivate:Serein_Websocket_SendPrivate," +
                "sendPackage:Serein_Websocket_SendPackage," +
                "getWsStatus:Serein_Websocket_Status," +
                "bindMember:Serein_Member_Bind," +
                "unbindMember:Serein_Member_UnBind," +
                "getID:Serein_Member_GetID," +
                "getGameID:Serein_Member_GetGameID" +
                "};");
            return engine;
        }
        public static string Run(string Code)
        {
            try
            {
                engine.Execute(Code);
                return string.Empty;
            }
            catch (Exception e)
            {
                Global.Debug("[JSEngine:Run()] " + e.Message);
                return e.Message;
            }
        }
        public static void Invoke(string FuncitonName, params object[] Args)
        {
            try
            {
                engine.Invoke(FuncitonName, Args);
            }
            catch (Exception e)
            {
                Global.Debug("[JSEngine:Invoke()] " + e.Message);
            }
        }
    }
}