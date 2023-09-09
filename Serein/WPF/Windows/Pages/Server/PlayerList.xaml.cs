using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json.Linq;
using Serein.Base.Motd;
using Serein.Core.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace Serein.Windows.Pages.Server
{
    /// <summary>
    /// Interaction logic for PlayerList.xaml
    /// </summary>
    public partial class PlayerList : UiPage
    {
        public static JObject? PlayerListData = Motd.PlayerListData;
        public static int CurrentSelectedIndex;
        //public bool isPlayerListNotNull = true;
        public List<Player> items { get; set; }
        public PlayerList()
        {
            
            InitializeComponent();

            Task.Run(() =>
            {
                
                while (true)
                {
                    refresh();
                    Thread.Sleep(500);
                }
            });
            

        }
        
        private void refresh()
        {
            string? PlayerListItems = null;
            PlayerListData = Motd.PlayerListData;
            try
            {
                PlayerListItems = PlayerListData["sample"].ToString();
            }
            catch
            {

            }
            items = new List<Player>();
            if (ServerManager.Status)
            {
                if (!string.IsNullOrEmpty(PlayerListItems))
                {
                    for (int a = 0; a < PlayerListData["sample"].Count(); a++)
                    {
                        string? Username = PlayerListData["sample"][a]["name"].ToString();
                        string? UUID = PlayerListData["sample"][a]["id"].ToString();

                        string UsernameFilter = Username + @"\[\/(.*?)\]";
                        string IPFilter = @"\[\/(.*?)\]";
                        string? IP = null;
                        try
                        {
                            MatchCollection match = Regex.Matches(Panel.ServerLog, UsernameFilter);
                            Console.WriteLine(match.Count());
                            string? str = Regex.Match(match[match.Count() - 1].Value, IPFilter).ToString();
                            str = str.Remove(0, 2);
                            IP = str.Substring(0, str.Length - 1);
                        }
                        catch
                        {

                        }
                        
                        items.Add(new Player() { Username = Username, UUID = UUID, IP = IP });
                    }
                }
            }
            else
            {

            }

            Dispatcher.Invoke(() =>
            {
                PlayerListView.ItemsSource = items;
            });
        }
        private void ListView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if(PlayerListView.SelectedIndex != -1)
            {   
                GamemodeSurvival.IsEnabled = true;
                GamemodeCreative.IsEnabled = true;
                GamemodeAdventure.IsEnabled = true;
                GamemodeSpectator.IsEnabled = true;
                SetOperator.IsEnabled = true;
                DeleteOperator.IsEnabled = true;
                KickPlayer.IsEnabled = true;
                BanPlayer.IsEnabled = true;
                BanPlayerIP.IsEnabled = true;
            }
            else
            {
                GamemodeSurvival.IsEnabled = false;
                GamemodeCreative.IsEnabled = false;
                GamemodeAdventure.IsEnabled = false;
                GamemodeSpectator.IsEnabled = false;
                SetOperator.IsEnabled = false;
                DeleteOperator.IsEnabled = false;
                KickPlayer.IsEnabled = false;
                BanPlayer.IsEnabled = false;
                BanPlayerIP.IsEnabled = false;
            }
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            refresh();
        }
        private void GamemodeSurvival_Click(object sender, RoutedEventArgs e)
        {
            string? username = null;
            try
            {
                username = items[CurrentSelectedIndex].Username;
                //username = items[-1].Username;
            }
            catch
            {
                return;
            }
            ServerManager.InputCommand("minecraft:gamemode survival " + username);
        }

        private void GamemodeCreative_Click(object sender, RoutedEventArgs e)
        {
            string? username = null;
            try
            {
                username = items[CurrentSelectedIndex].Username;
                //username = items[-1].Username;
            }
            catch
            {
                return;
            }
            ServerManager.InputCommand("minecraft:gamemode creative " + username);
        }

        private void GamemodeAdventure_Click(object sender, RoutedEventArgs e)
        {
            string? username = null;
            try
            {
                username = items[CurrentSelectedIndex].Username;
                //username = items[-1].Username;
            }
            catch
            {
                return;
            }
            ServerManager.InputCommand("minecraft:gamemode adventure " + username);
        }

        private void GamemodeSpectator_Click(object sender, RoutedEventArgs e)
        {
            string? username = null;
            try
            {
                username = items[CurrentSelectedIndex].Username;
                //username = items[-1].Username;
            }
            catch
            {
                return;
            }
            ServerManager.InputCommand("minecraft:gamemode spectator " + username);
        }

        private void SetOperator_Click(object sender, RoutedEventArgs e)
        {
            string? username = null;
            try
            {
                username = items[CurrentSelectedIndex].Username;
                //username = items[-1].Username;
            }
            catch
            {
                return;
            }
            ServerManager.InputCommand("minecraft:op " + username);
        }

        private void DeleteOperator_Click(object sender, RoutedEventArgs e)
        {
            string? username = null;
            try
            {
                username = items[CurrentSelectedIndex].Username;
                //username = items[-1].Username;
            }
            catch
            {
                return;
            }
            ServerManager.InputCommand("minecraft:deop " + username);
        }

        private void KickPlayer_Click(object sender, RoutedEventArgs e)
        {
            string? username = null;
            try
            {
                username = items[CurrentSelectedIndex].Username;
                //username = items[-1].Username;
            }
            catch
            {
                return;
            }
            ServerManager.InputCommand("minecraft:kick " + username);
        }

        private void BanPlayer_Click(object sender, RoutedEventArgs e)
        {
            string? username = null;
            try
            {
                username = items[CurrentSelectedIndex].Username;
                //username = items[-1].Username;
            }
            catch
            {
                return;
            }
            ServerManager.InputCommand("ban " + username);
        }

        private void BanPlayerIP_Click(object sender, RoutedEventArgs e)
        {
            string? username = null;
            try
            {
                username = items[CurrentSelectedIndex].Username;
                //username = items[-1].Username;
            }
            catch
            {
                return;
            }
            ServerManager.InputCommand("ipban " + username);
        }

        private void PlayerListView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(PlayerListView.SelectedIndex != -1)
            {
                CurrentSelectedIndex = PlayerListView.SelectedIndex;
            }
        }
    }
    public class Player
    {
        public string? Username { get; set; }
        public string? UUID { get; set; }
        public string? IP { get; set; }
        public override string ToString()
        {
            return this.Username + " (" + this.UUID + ") IP:" + this.IP;
        }
    }
}
