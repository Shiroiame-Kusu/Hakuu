using Newtonsoft.Json.Linq;
using Serein.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
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
        private string? token { get; set; }
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

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            Username.IsEnabled = false;
            Password.IsEnabled = false;
            using var client = new HttpClient();
            string username = Username.Text;
            string password = Password.Text;
            var result = await client.GetAsync("https://api.locyanfrp.cn/User/DoLogin?username=" + username + "&password=" + password);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var result2 = result.Content.ReadAsStringAsync().ToString();
                JObject? ParsedResult = JObject.Parse(result2);
                if(result.StatusCode.ToString() == "200")
                {
                    if(ParsedResult["status"].ToString() == "0")
                    {
                        token = ParsedResult["token"].ToString();
                    }
                    else
                    {
                        Logger.MsgBox("Serein", ParsedResult["message"].ToString(), 0, 48);
                    }
                }
                else
                {
                    Logger.MsgBox("Serein", "请检查您的网络连接", 0, 48);
                }
            }
            else
            {
                Username.IsEnabled = true;
                Password.IsEnabled = true;
                Logger.MsgBox("Serein", "你到底打没打用户名和密码", 0, 48);
            }
            
            
        }
    }
}
