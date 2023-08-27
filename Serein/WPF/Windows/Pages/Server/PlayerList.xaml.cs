using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json.Linq;
using Serein.Base.Motd;
using Serein.Core.Server;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace Serein.Windows.Pages.Server
{
    /// <summary>
    /// Interaction logic for PlayerList.xaml
    /// </summary>
    public partial class PlayerList : UiPage
    {
        public PlayerList()
        {
            
            InitializeComponent();
            
            Task.Run(() =>
            {
                while (true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        //if(ServerManager.Start())
                        try
                        {
                            JObject? PlayerListItems = Motd.PlayerListData;
                            Console.WriteLine(PlayerListItems.ToString());
                            int x = -1;
                        
                            x = PlayerListItems["sample"].Count() - 1;
                        
                        List<Player> items = new List<Player>();

                        for (int i = 0;i <= x;i++)
                        {
                        //ListColumnUsername. PlayerListItems["sample"][i];
                        var item = PlayerListItems["sample"][i]["name"];
                            Console.WriteLine(item);
                            items.Add(new Player() { Username = PlayerListItems["sample"][i]["name"].ToString(), UUID = PlayerListItems["sample"][i]["id"].ToString()});
                        }
                    
                        PlayerListView.ItemsSource = items;
                        }
                        catch
                        {

                        }
                    },System.Windows.Threading.DispatcherPriority.Background);
                        
                        Thread.Sleep(500);
                }
            });
        }

        private void ListView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }
    }
    public class Player
    {
        public string Username { get; set; }
        public string UUID { get; set; }
    }
}
