using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;

namespace Serein.Windows.Pages.Server
{
    public partial class Container : UiPage
    {
        public Container()
        {
            InitializeComponent();
            PanelNavigationItem.Visibility = Global.Settings.Serein.PagesDisplayed.ServerPanel ? Visibility.Visible : Visibility.Hidden;
            PluginManagerNavigationItem.Visibility = Global.Settings.Serein.PagesDisplayed.ServerPluginManager ? Visibility.Visible : Visibility.Hidden;
            DownloadServerNavigationItem.Visibility = Global.Settings.Serein.PagesDisplayed.DownloadServer ? Visibility.Visible : Visibility.Hidden;
            if (Global.Settings.Serein.PagesDisplayed.ServerPanel)
            {
                Navigation.Navigate(0);
            }
            else if (Global.Settings.Serein.PagesDisplayed.ServerPluginManager)
            {
                Navigation.Navigate(1);
            }
            else if (Global.Settings.Serein.PagesDisplayed.DownloadServer)
            {
                Navigation.Navigate(2);
            }
            else
            {
                Navigation.Frame = null;
            }
            Catalog.Server.Container = this;
            Task.Run(() => {
                while (true)
                {
                    string? ServerType = null;
                    try
                    {
                        ServerType = Global.Settings.Server.Path.Substring(Global.Settings.Server.Path.Length - 3);
                    }
                    catch
                    {

                    }
                    switch (ServerType)
                    {
                        case "jar":
                            Dispatcher.InvokeAsync(() => { MinecraftProperties.Visibility = Visibility.Visible; }, System.Windows.Threading.DispatcherPriority.Background);
                            Dispatcher.InvokeAsync(() => { PlayerList.Visibility = Visibility.Visible; }, System.Windows.Threading.DispatcherPriority.Background);
                            break;
                        default:
                            Dispatcher.InvokeAsync(() => { MinecraftProperties.Visibility = Visibility.Collapsed; }, System.Windows.Threading.DispatcherPriority.Background);
                            Dispatcher.InvokeAsync(() => { PlayerList.Visibility = Visibility.Collapsed; }, System.Windows.Threading.DispatcherPriority.Background);
                            break;
                    }
                    System.Threading.Thread.Sleep(500);
                }
            });
        }
    }
}
