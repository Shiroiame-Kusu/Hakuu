using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace Serein.Windows.Pages.Function.Frp
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UiPage
    {
        public Login()
        {
            InitializeComponent();

        }

        private void Register_Navigate(object sender, RequestNavigateEventArgs e)
        {
            var url = e.Uri.ToString();
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
            e.Handled = true;
        }
        private void ForgetPassword_Navigate(object sender, RequestNavigateEventArgs e)
        {
            var url = e.Uri.ToString();
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
            e.Handled = true;
        }
    }
}
