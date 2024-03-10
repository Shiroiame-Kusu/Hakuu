using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Hakuu.Settings
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    internal class Category
    {
        public Server Server = new();
        
        public Matches Matches = new();
        
        public Hakuu Hakuu = new();
        
        public Event Event = new();
    }
}
