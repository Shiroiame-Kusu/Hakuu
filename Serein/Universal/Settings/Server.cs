﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Serein.Settings
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    internal class Server
    {
        public bool AutoStop = true;
        
        public bool EnableRestart;
        
        public bool EnableOutputCommand = true;
        
        public bool EnableLog;
        
        public bool EnableUnicode;
        
        public string[] ExcludedOutputs = new string[] { };
        
        public int InputEncoding;
        
        public string LineTerminator = Environment.NewLine;
        
        public int OutputEncoding;
        
        public int OutputStyle = 1;
        
        public string Path = string.Empty;
        
        public int Port = 25565;
        
        public string[] StopCommands = { "stop" };
        
        public int Type = 1;
    }
}
