using Notification.Wpf;
using Hakuu.Windows.Pages;
using Hakuu.Windows.Pages.Function;
using Hakuu.Windows.Pages.Server;
using Hakuu.Windows.Pages.Settings;
using System.Collections.Generic;

namespace Hakuu.Windows
{
    internal static class Catalog
    {
        public static NotificationManager? Notification { get; set; }
        public static MainWindow? MainWindow { get; set; }
        public static Home? Home { get; set; }
        public static Debug? Debug { get; set; }

        public static class Server
        {
            public static Panel? Panel { get; set; }
            public static Plugins? Plugins { get; set; }
            public static Pages.Server.Container? Container { get; set; }
            public static List<(Hakuu.Base.LogType, string)> Cache = new();
        }

        public static class Function
        {
            public static Schedule? Schedule { get; set; }
            public static Pages.Function.Container? Container { get; set; }
            public static List<(Hakuu.Base.LogType, string)> BotCache = new();
            public static List<(Hakuu.Base.LogType, string)> PluginCache = new();
        }

        public static class Settings
        {
            public static Pages.Settings.Server? Server { get; set; }
            public static Pages.Settings.Hakuu? Hakuu { get; set; }
            public static Pages.Settings.Container? Container { get; set; }
        }
        public static class WaitForUpdate
        {
            public static bool Server_Panel = false;
            public static bool Server_Container = false;
        }
    }
}
