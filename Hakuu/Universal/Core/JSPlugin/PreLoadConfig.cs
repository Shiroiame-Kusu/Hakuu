using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Hakuu.Core.JSPlugin
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    internal struct PreLoadConfig
    {
        public string[] Assemblies = { };

        /// <remarks>
        /// https://github.com/sebastienros/jint/blob/main/Jint/Options.cs
        /// </remarks>
        public bool
            AllowGetType = false,
            AllowOperatorOverloading = true,
            AllowSystemReflection = false,
            AllowWrite = true,
            Strict = false,
            StringCompilationAllowed = true;

        public PreLoadConfig() { }
    }
}
