using Serein;
using Serein.Utils;
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

namespace Serein.Windows.Pages.Server
{
    /// <summary>
    /// Interaction logic for Properties.xaml
    /// </summary>
    public partial class Properties : UiPage
    {

        public PropertyOperation PropertiesOperation;
        public static int i = 0;

        private static bool IsBackgroundTaskRunning = false;
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
            catch
            {
                
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
                        GC.Collect();
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
                                Dispatcher.InvokeAsync(() =>
                                {
                                    PropertiesPage.IsEnabled = false;
                                }, System.Windows.Threading.DispatcherPriority.Background);
                            }
                        Thread.Sleep(1000);
                    }

                    

                });
            }
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




            GC.Collect();

        }
        private void SaveProperties()
        {
            PropertiesOperation.Save();
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
            PropertiesOperation.Save();
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
            PropertiesOperation.Save();
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
            PropertiesOperation.Save();
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
            PropertiesOperation.Save();
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
            PropertiesOperation.Save();
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
            PropertiesOperation.Save();
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
            PropertiesOperation.Save();
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
            PropertiesOperation.Save();
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
            PropertiesOperation.Save();
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
            PropertiesOperation.Save();
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
            PropertiesOperation.Save();
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
            PropertiesOperation.Save();
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
            PropertiesOperation.Save();
        }

        private void SERVER_SEED_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["level-seed"] = SERVER_SEED.Text;
            PropertiesOperation.Save();
        }

        private void SERVER_PORT_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["server-port"] = SERVER_PORT.Text;
            PropertiesOperation.Save();
        }

        private void SPAWNPOINT_PROTECT_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["spawn-protection"] = SPAWNPOINT_PROTECT.Text;
            PropertiesOperation.Save();
        }

        private void MAINWORLD_NAME_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["level-name"] = MAINWORLD_NAME.Text;
            PropertiesOperation.Save();
        }

        private void WORLD_BORDER_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["max-world-size"] = WORLD_BORDER.Text;
            PropertiesOperation.Save();
        }

        private void SERVER_MAXPLAYER_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["max-players"] = SERVER_MAXPLAYER.Text;
            PropertiesOperation.Save();
        }

        private void SERVER_MOTD_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["motd"] = SERVER_MOTD.Text;
            PropertiesOperation.Save();
        }

        private void SERVER_VIEWDISTANCE_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["view-distance"] = SERVER_VIEWDISTANCE.Text;
            PropertiesOperation.Save();
        }

        private void SERVER_SIMULATEDISTANCE_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertiesOperation["simulation-distance"] = SERVER_SIMULATEDISTANCE.Text;
            PropertiesOperation.Save();
        }
    }
}
