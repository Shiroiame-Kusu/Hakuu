using System.Windows;
using Wpf.Ui.Controls;

namespace Hakuu.Windows.Pages.Function
{
    public partial class Container : UiPage
    {
        public Container()
        {
            InitializeComponent();
            ScheduleNavigationItem.Visibility = Global.Settings.Hakuu.PagesDisplayed.Schedule ? Visibility.Visible : Visibility.Hidden;
            
            if (Global.Settings.Hakuu.PagesDisplayed.Schedule)
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
