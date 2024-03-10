using Wpf.Ui.Controls;

namespace Hakuu.Windows.Pages.Settings
{
    public partial class Container : UiPage
    {
        public Container()
        {
            InitializeComponent();
            Catalog.Settings.Container = this;
        }
    }
}
