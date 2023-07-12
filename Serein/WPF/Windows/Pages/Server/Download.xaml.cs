using Newtonsoft.Json.Linq;
using Serein.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Serein.Windows.Pages.Server
{
    /// <summary>
    /// Interaction logic for Download.xaml
    /// </summary>
    public partial class Download : UiPage
    {
        public Download()
        {
            string API = "https://download.fastmirror.net/api/v3";
            InitializeComponent();
            //var APIResponse = RequestAPI(API).ToString();
            //JObject APIDataPrase = JObject.Parse(APIResponse);
            /*for (int i = 0; i < APIDataPrase.Count; i++)
            {
                
            }*/
        }

        private void ServerDownload(object sender, RoutedEventArgs e)
        {
            
        }
        private static void Downloader()
        {
            
        } 
        private static void RequestAPI(string url)
        {
            var HttpClient = new HttpClient();
            var Response =  HttpClient.GetAsync(url);
            var StatusCode = Response.StatusCode;
            var Header = Response.Headers;
            var APIResponse =  Response.Content.ReadAsStringAsync();
            return APIResponse;
        }
    }
}
