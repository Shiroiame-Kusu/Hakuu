using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;

namespace Hakuu.Windows.Pages.Server
{
    public partial class Container : UiPage
    {
        public Container()
        {
            InitializeComponent();
            PanelNavigationItem.Visibility = Global.Settings.Hakuu.PagesDisplayed.ServerPanel ? Visibility.Visible : Visibility.Hidden;
            PluginManagerNavigationItem.Visibility = Global.Settings.Hakuu.PagesDisplayed.ServerPluginManager ? Visibility.Visible : Visibility.Hidden;
            DownloadServerNavigationItem.Visibility = Global.Settings.Hakuu.PagesDisplayed.DownloadServer ? Visibility.Visible : Visibility.Hidden;
            if (Global.Settings.Hakuu.PagesDisplayed.ServerPanel)
            {
                Navigation.Navigate(0);
            }
            else if (Global.Settings.Hakuu.PagesDisplayed.ServerPluginManager)
            {
                Navigation.Navigate(1);
            }
            else if (Global.Settings.Hakuu.PagesDisplayed.DownloadServer)
            {
                Navigation.Navigate(2);
            }
            else
            {
                Navigation.Frame = null;
            }
            Catalog.Server.Container = this;
            if (Catalog.WaitForUpdate.Server_Container) {
                ContainerUpdater(Global.Settings.Server.Path.Substring(Global.Settings.Server.Path.Length - 3));
            }
        }
        public void ContainerUpdater(string ServerType)
        {
            if (!string.IsNullOrEmpty(ServerType))
            {

                switch (ServerType)
                {
                    case "jar":
                        Dispatcher.Invoke(() => { this.MinecraftProperties.Visibility = Visibility.Visible; });
                        Dispatcher.Invoke(() => { this.PlayerList.Visibility = Visibility.Visible; });
                        break;
                    default:
                        Dispatcher.Invoke(() => { this.MinecraftProperties.Visibility = Visibility.Collapsed; });
                        Dispatcher.Invoke(() => { this.PlayerList.Visibility = Visibility.Collapsed; });
                        break;
                }


            }
            Catalog.WaitForUpdate.Server_Container = false;
        }
    }
}
