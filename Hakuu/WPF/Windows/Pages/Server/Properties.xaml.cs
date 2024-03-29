using Hakuu;
using Hakuu.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using Wpf.Ui.Controls;
using System.Security.Cryptography;
using Hakuu.Core.Server;
using System.Collections.Specialized;

namespace Hakuu.Windows.Pages.Server
{
    /// <summary>
    /// Interaction logic for Properties.xaml
    /// </summary>
    public partial class Properties : UiPage
    {

        public PropertyOperation PropertiesOperation ;
        public static int i = 0;

        private static bool IsBackgroundTaskRunning = false;
        private static bool PageIsEnabled;
        private static string OldMD5 = "";
        
        public Properties()
        {
            InitializeComponent();

            PropertiesPage.IsEnabled = false;
            try
            {
                
                if(File.Exists(Path.GetDirectoryName(Global.Settings.Server.Path) + "\\server.properties"))
                {
                    PropertiesOperation = new PropertyOperation(Path.GetDirectoryName(Global.Settings.Server.Path) + "\\server.properties");
                    LoadProperties();
                    ReloadinBackground();
                }
                else
                {
                    ReloadinBackground();
                }
            }
            catch(Exception ex) 
            {
                CrashInterception.ShowException(ex);
            }
        }
        private void ReloadinBackground()
        {
            i++;
            if(i <= 1)
            {
                Task.Run(() => {
                while (true)
                {
                    if (!ServerManager.Status)
                    {
                        string MD5 = null;
                            if(File.Exists(Path.GetDirectoryName(Global.Settings.Server.Path) + "\\server.properties"))
                            {
                                MD5 = GetMD5WithFilePath(Path.GetDirectoryName(Global.Settings.Server.Path) + "\\server.properties");
                            }
                            if (!string.IsNullOrEmpty(MD5) && MD5 != OldMD5)
                            {   
                                OldMD5 = MD5;
                                if (File.Exists(Path.GetDirectoryName(Global.Settings.Server.Path) + "\\server.properties"))
                                {
                                    PropertiesOperation = new PropertyOperation(Path.GetDirectoryName(Global.Settings.Server.Path) + "\\server.properties");
                                    Dispatcher.InvokeAsync(() =>
                                    {

                                        PropertiesPage.IsEnabled = true;
                                        LoadProperties();
                                    }, System.Windows.Threading.DispatcherPriority.Background);

                                }
                                else
                                {
                                    if (PageIsEnabled)
                                    {
                                        Dispatcher.InvokeAsync(() =>
                                        {
                                            PropertiesPage.IsEnabled = false;
                                        }, System.Windows.Threading.DispatcherPriority.Background);
                                    }

                                }

                            }
                        }else{
                            if (PageIsEnabled)
                            {
                                Dispatcher.InvokeAsync(() =>
                                {
                                    PropertiesPage.IsEnabled = false;
                                }, System.Windows.Threading.DispatcherPriority.Background);
                            }
                        }
                        Thread.Sleep(1000);
                    }

                    

                });
            }
        }
        public static string GetMD5WithFilePath(string filePath)
        {
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash_byte = md5.ComputeHash(file);
            string str = System.BitConverter.ToString(hash_byte);
            str = str.Replace("-", "");
            file.Close();
            return str;
        }
        private void LoadProperties()
        {   
            
            switch (PropertiesOperation["gamemode"])
            {
                case "survival":
                    SERVER_MODE.SelectedIndex = 0;
                    break;
                case "creative":
                    SERVER_MODE.SelectedIndex = 1;
                    break;
                case "spectator":
                    SERVER_MODE.SelectedIndex = 2;
                    break;
                case "adventure":
                    SERVER_MODE.SelectedIndex = 3;
                    break;
                default: throw new Exception();
            }
            switch (PropertiesOperation["difficulty"])
            {
                case "peaceful":
                    SERVER_DIFFICULTY.SelectedIndex = 0;
                    break;
                case "easy":
                    SERVER_DIFFICULTY.SelectedIndex = 1;
                    break;
                case "normal":
                    SERVER_DIFFICULTY.SelectedIndex = 2;
                    break;
                case "hard":
                    SERVER_DIFFICULTY.SelectedIndex = 3;
                    break;
                default: throw new Exception();
            }
            switch (PropertiesOperation["allow-flight"])
            {
                case "true":
                    ALLOW_FLIGHT.SelectedIndex = 0;
                    break;
                case "false":
                    ALLOW_FLIGHT.SelectedIndex = 1;
                    break;
                default: throw new Exception();
            }
            switch (PropertiesOperation["spawn-animals"])
            {
                case "true":
                    ANIMAL_SPAWN.SelectedIndex = 0;
                    break;
                case "false":
                    ANIMAL_SPAWN.SelectedIndex = 1;
                    break;
                default: throw new Exception();
            }
            switch (PropertiesOperation["spawn-monsters"])
            {
                case "true":
                    MOBS_SPAWN.SelectedIndex = 0;
                    break;
                case "false":
                    MOBS_SPAWN.SelectedIndex = 1;
                    break;
                default:
                    throw new Exception();
            }
            switch (PropertiesOperation["spawn-npcs"])
            {
                case "true":
                    NPC_SPAWN.SelectedIndex = 0;
                    break;
                case "false":
                    NPC_SPAWN.SelectedIndex = 1;
                    break;
                default:
                    throw new Exception();
            }
            switch (PropertiesOperation["generate-structures"])
            {
                case "true":
                    BUILD_GENERATE.SelectedIndex = 0;
                    break;
                case "false":
                    BUILD_GENERATE.SelectedIndex = 1;
                    break;
                default:
                    throw new Exception();
            }
            switch (PropertiesOperation["allow-nether"])
            {
                case "true":
                    NETHER_ISENABLED.SelectedIndex = 0;
                    break;
                case "false":
                    NETHER_ISENABLED.SelectedIndex = 1;
                    break;
                default:
                    throw new Exception();
            }
            switch (PropertiesOperation["pvp"])
            {
                case "true":
                    PVP_ISENABLED.SelectedIndex = 0;
                    break;
                case "false":
                    PVP_ISENABLED.SelectedIndex = 1;
                    break;
                default:
                    throw new Exception();
            }
            switch (PropertiesOperation["hardcore"])
            {
                case "true":
                    HARDCORE_ISENABLED.SelectedIndex = 0;
                    break;
                case "false":
                    HARDCORE_ISENABLED.SelectedIndex = 1;
                    break;
                default:
                    throw new Exception();
            }
            switch (PropertiesOperation["white-list"])
            {
                case "true":
                    WHITELIST_ISENABLED.SelectedIndex = 0;
                    break;
                case "false":
                    WHITELIST_ISENABLED.SelectedIndex = 1;
                    break;
                default:
                    throw new Exception();
            }
            switch (PropertiesOperation["enable-command-block"])
            {
                case "true":
                    COMMANDBLOCK_ISENABLED.SelectedIndex = 0;
                    break;
                case "false":
                    COMMANDBLOCK_ISENABLED.SelectedIndex = 1;
                    break;
                default:
                    throw new Exception();
            }
            switch (PropertiesOperation["online-mode"])
            {
                case "true":
                    ONLINEMODE_ISENABLED.SelectedIndex = 0;
                    break;
                case "false":
                    ONLINEMODE_ISENABLED.SelectedIndex = 1;
                    break;
                default:
                    throw new Exception();
            }
            SERVER_SEED.Text = PropertiesOperation["level-seed"]?.ToString();
            SERVER_PORT.Text = PropertiesOperation["server-port"]?.ToString();
            try
            {
                if(int.Parse(PropertiesOperation["server-port"]?.ToString()) != 0)
                    {
                        Global.Settings.Server.Port = int.Parse(PropertiesOperation["server-port"]?.ToString());
                    }
            }
            catch
            {

            }
            SPAWNPOINT_PROTECT.Text = PropertiesOperation["spawn-protection"]?.ToString();
            MAINWORLD_NAME.Text = PropertiesOperation["level-name"]?.ToString();
            WORLD_BORDER.Text = PropertiesOperation["max-world-size"]?.ToString();
            SERVER_MAXPLAYER.Text = PropertiesOperation["max-players"]?.ToString();
            SERVER_MOTD.Text = PropertiesOperation["motd"]?.ToString();
            SERVER_VIEWDISTANCE.Text = PropertiesOperation["view-distance"]?.ToString();
            SERVER_SIMULATEDISTANCE.Text = PropertiesOperation["simulation-distance"]?.ToString();
            SERVER_MODE.SelectionChanged += SERVER_MODE_SelectionChanged;
            SERVER_DIFFICULTY.SelectionChanged += SERVER_DIFFICULTY_SelectionChanged;
            ALLOW_FLIGHT.SelectionChanged += ALLOW_FLIGHT_SelectionChanged;
            ANIMAL_SPAWN.SelectionChanged += ANIMAL_SPAWN_SelectionChanged;
            MOBS_SPAWN.SelectionChanged += MOBS_SPAWN_SelectionChanged;
            NPC_SPAWN.SelectionChanged += NPC_SPAWN_SelectionChanged;
            BUILD_GENERATE.SelectionChanged += BUILD_GENERATE_SelectionChanged;
            NETHER_ISENABLED.SelectionChanged += NETHER_ISENABLED_SelectionChanged;
            PVP_ISENABLED.SelectionChanged += PVP_ISENABLED_SelectionChanged;
            HARDCORE_ISENABLED.SelectionChanged += HARDCORE_ISENABLED_SelectionChanged;
            WHITELIST_ISENABLED.SelectionChanged += WHITELIST_ISENABLED_SelectionChanged;
            COMMANDBLOCK_ISENABLED.SelectionChanged += COMMANDBLOCK_ISENABLED_SelectionChanged;
            ONLINEMODE_ISENABLED.SelectionChanged += ONLINEMODE_ISENABLED_SelectionChanged;
            SERVER_SEED.TextChanged += SERVER_SEED_TextChanged;
            SERVER_PORT.TextChanged += SERVER_PORT_TextChanged;
            SPAWNPOINT_PROTECT.TextChanged += SPAWNPOINT_PROTECT_TextChanged;
            MAINWORLD_NAME.TextChanged += MAINWORLD_NAME_TextChanged;
            WORLD_BORDER.TextChanged += WORLD_BORDER_TextChanged;
            SERVER_MAXPLAYER.TextChanged += SERVER_MAXPLAYER_TextChanged;
            SERVER_MOTD.TextChanged += SERVER_MOTD_TextChanged;
            SERVER_VIEWDISTANCE.TextChanged += SERVER_VIEWDISTANCE_TextChanged;
            SERVER_SIMULATEDISTANCE.TextChanged += SERVER_SIMULATEDISTANCE_TextChanged;



            GC.Collect();

        }
        private async void SaveProperties()
        {
            
            if (!PropertiesOperation.Save())
            {
                Dispatcher.Invoke(() => {
                    Logger.MsgBox($"无法自动保存配置文件, 文件被{PropertyOperation.FileOccupiedProcessName} PID:{PropertyOperation.FileOccupiedProcessPID}占用","Hakuu",0,48);
                },System.Windows.Threading.DispatcherPriority.Background);

            }
        }
        private void SERVER_MODE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (SERVER_MODE.SelectedIndex) {
                case 0:
                    PropertiesOperation["gamemode"] = "survival";
                    break;
                case 1:
                    PropertiesOperation["gamemode"] = "creative";
                    break;
                case 2:
                    PropertiesOperation["gamemode"] = "spectator";
                    break;
                case 3:
                    PropertiesOperation["gamemode"] = "adventure";
                    break;

            }
            SaveProperties();
        }

        private void SERVER_DIFFICULTY_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (SERVER_DIFFICULTY.SelectedIndex)
            {
                case 0:
                    PropertiesOperation["difficulty"] = "peaceful";
                    break; 
                case 1:
                    PropertiesOperation["difficulty"] = "easy";
                    break; 
                case 2:
                    PropertiesOperation["difficulty"] = "normal";
                    break;
                case 3:
                    PropertiesOperation["difficulty"] = "hard";
                    break;
            }
            SaveProperties();
        }

        private void ALLOW_FLIGHT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ALLOW_FLIGHT.SelectedIndex)
            {
                case 0:
                    PropertiesOperation["allow-flight"] = "true"; 
                    break;
                case 1:
                    PropertiesOperation["allow-flight"] = "false";
                    break;
            }
            SaveProperties();
        }

        private void ANIMAL_SPAWN_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ANIMAL_SPAWN.SelectedIndex)
            {
                case 0:
                    PropertiesOperation["spawn-animals"] = "true";
                    break;
                case 1:
                    PropertiesOperation["spawn-animals"] = "false";
                    break;
            }
            SaveProperties();
        }

        private void MOBS_SPAWN_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (MOBS_SPAWN.SelectedIndex)
            {
                case 0:
                    PropertiesOperation["spawn-monsters"] = "true";
                    break;
                case 1:
                    PropertiesOperation["spawn-monsters"] = "false";
                    break;
            }
            SaveProperties();
        }

        private void NPC_SPAWN_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (NPC_SPAWN.SelectedIndex)
            {
                case 0:
                    PropertiesOperation["spawn-npcs"] = "true";
                    break;
                case 1:
                    PropertiesOperation["spawn-npcs"] = "false";
                    break;
            }
            SaveProperties();
        }

        private void BUILD_GENERATE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(BUILD_GENERATE.SelectedIndex)
            {
                case 0:
                    PropertiesOperation["generate-structures"] = "true";
                    break;
                case 1:
                    PropertiesOperation["generate-structures"] = "false";
                    break;
            }
            SaveProperties();
        }

        private void NETHER_ISENABLED_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(NETHER_ISENABLED.SelectedIndex) {
                case 0:
                    PropertiesOperation["allow-nether"] = "true";
                    break;
                case 1:
                    PropertiesOperation["allow-nether"] = "false";
                    break;
            }
            SaveProperties();
        }

        private void PVP_ISENABLED_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (PVP_ISENABLED.SelectedIndex)
            {
                case 0:
                    PropertiesOperation["pvp"] = "true";
                    break;
                case 1:
                    PropertiesOperation["pvp"] = "false";
                    break;
            }
            SaveProperties();
        }

        private void HARDCORE_ISENABLED_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (HARDCORE_ISENABLED.SelectedIndex)
            {
                case 0:
                    PropertiesOperation["hardcore"] = "true";
                    break;
                case 1:
                    PropertiesOperation["hardcore"] = "false";
                    break;
            }
            SaveProperties();
        }

        private void WHITELIST_ISENABLED_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (WHITELIST_ISENABLED.SelectedIndex)
            {
                case 0:
                    PropertiesOperation["white-list"] = "true";
                    break;
                case 1:
                    PropertiesOperation["white-list"] = "false";
                    break;
            }
            SaveProperties();
        }

        private void COMMANDBLOCK_ISENABLED_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (COMMANDBLOCK_ISENABLED.SelectedIndex)
            {
                case 0:
                    PropertiesOperation["enable-command-block"] = "true";
                    break;
                case 1:
                    PropertiesOperation["enable-command-block"] = "false";
                    break;
            }
            SaveProperties();
        }

        private void ONLINEMODE_ISENABLED_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ONLINEMODE_ISENABLED.SelectedIndex)
            {
                case 0:
                    PropertiesOperation["online-mode"] = "true";
                    break;
                case 1:
                    PropertiesOperation["online-mode"] = "false";
                    break;
            }
            SaveProperties();
        }

        private void SERVER_SEED_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["level-seed"] = SERVER_SEED.Text;
            SaveProperties();
        }

        private void SERVER_PORT_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["server-port"] = SERVER_PORT.Text;
            SaveProperties();
        }

        private void SPAWNPOINT_PROTECT_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["spawn-protection"] = SPAWNPOINT_PROTECT.Text;
            SaveProperties();
        }

        private void MAINWORLD_NAME_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["level-name"] = MAINWORLD_NAME.Text;
            SaveProperties();
        }

        private void WORLD_BORDER_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["max-world-size"] = WORLD_BORDER.Text;
            SaveProperties();
        }

        private void SERVER_MAXPLAYER_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["max-players"] = SERVER_MAXPLAYER.Text;
            SaveProperties();
        }

        private void SERVER_MOTD_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["motd"] = SERVER_MOTD.Text;
            SaveProperties();
        }

        private void SERVER_VIEWDISTANCE_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["view-distance"] = SERVER_VIEWDISTANCE.Text;
            SaveProperties();
        }

        private void SERVER_SIMULATEDISTANCE_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["simulation-distance"] = SERVER_SIMULATEDISTANCE.Text;
            SaveProperties();
        }

        private void PropertiesPage_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PageIsEnabled = PropertiesPage.IsEnabled;
        }
    }
}
