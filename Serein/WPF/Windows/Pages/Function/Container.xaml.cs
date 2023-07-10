using System.Windows;
using Wpf.Ui.Controls;

namespace Serein.Windows.Pages.Function
{
    public partial class Container : UiPage
    {
        public Container()
        {
            InitializeComponent();
            ScheduleNavigationItem.Visibility = Global.Settings.Serein.PagesDisplayed.Schedule ? Visibility.Visible : Visibility.Hidden;
            
            if (Global.Settings.Serein.PagesDisplayed.Schedule)
            {
                Navigation.Navigate(4);
            }
            else
            {
                Navigation.Frame = null;
            }
            Catalog.Function.Container = this;
        }
    }
}
