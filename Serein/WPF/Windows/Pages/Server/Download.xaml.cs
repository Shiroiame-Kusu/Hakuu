using Newtonsoft.Json.Linq;
using Serein.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
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
using Wpf.Ui.Dpi;

namespace Serein.Windows.Pages.Server
{
    /// <summary>
    /// Interaction logic for Download.xaml
    /// </summary>
    
    public partial class Download : UiPage
    {
        public static string? API = "https://download.fastmirror.net/api/v3";
        public static string APIResult {  get; set; }
        public static string APIStatusCode { get; set; }
        public static int DownloadableServerNumber {get; set; }
        public static int DownloadableServerVersion { get; set; }
        public static string DetailedAPIStatusCode { get; private set; }
        public static string DetailedAPIResult { get; private set; }

        public Download()
        {
            InitializeComponent();
            LoadAPIInfo();
            
        }

        private async void ServerDownload(object sender, RoutedEventArgs e)
        {
            ServerDownloadLogTextBox.Clear();
            var ServerName = ServerDownloadName.SelectedItem.ToString();
            var ServerVersion = ServerDownloadVersion.SelectedItem.ToString();
            ServerDownloadLogTextBox.AppendText($"正在下载服务端\n名称：{ServerName}\n版本：{ServerVersion}\n");
            try
            {
                await RequestDetailedAPI(API + ServerDownloadName.SelectedItem + "/" + ServerDownloadVersion.SelectedItem);
                if (DetailedAPIStatusCode != "200" || DetailedAPIStatusCode != "OK")
                {
                    Logger.MsgBox("无法连接至服务器，请检查您的网络连接", "Serein", 0, 48);

                }
                JObject DetailedAPIPrase = JObject.Parse(DetailedAPIResult);
                Downloader("https://download.fastmirror.net/download/" + ServerDownloadName.SelectedItem + "/" + ServerDownloadVersion.SelectedItem + "/" + DetailedAPIPrase["data"]["builds"]["core_version"]);
            }
            catch (Exception ex)
            {
                
            }



        }
        private static void Downloader(string url)
        {
            
        } 
        private async void LoadAPIInfo()
        {
            await RequestAPI(API); 
            try{
                if (APIStatusCode != "200" || APIStatusCode != "OK")
                {
                    Logger.MsgBox("无法连接至服务器，请检查您的网络连接", "Serein", 0, 48);

                }

            }
            catch(Exception ex){
                
            }

            JObject APIDataPrase = JObject.Parse(APIResult);
            DownloadableServerNumber = APIDataPrase["data"].Count();
            for(int i = 0; i < DownloadableServerNumber; i++)
            {
                ServerDownloadName.Items.Add(APIDataPrase["data"][i]["name"]);
            }
            
        }
       
        private static async Task RequestAPI(string url)
        {
            var HttpClient = new HttpClient();
            var Response = await HttpClient.GetAsync(url);
            var StatusCode = Response.StatusCode;
            var Header = Response.Headers;
            var APIResponse =  Response.Content.ReadAsStringAsync();
            APIStatusCode = StatusCode.ToString();
            APIResult = APIResponse.Result.ToString();
           
        }
        private static async Task RequestDetailedAPI(string url)
        {
            var HttpClient = new HttpClient();
            var Response = await HttpClient.GetAsync(url);
            var StatusCode = Response.StatusCode;
            var Header = Response.Headers;
            var APIResponse = Response.Content.ReadAsStringAsync();
            DetailedAPIStatusCode = StatusCode.ToString();
            DetailedAPIResult = APIResponse.Result.ToString();

        }

        private void ServerDownloadName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var i = ServerDownloadName.SelectedIndex;
            JObject APIDataPrase = JObject.Parse(APIResult);
            DownloadableServerVersion = APIDataPrase["data"][i]["mc_versions"].Count();
            ServerDownloadVersion.Items.Clear();
            for (int i2 = 0;i2 < DownloadableServerVersion; i2++)
            {
                ServerDownloadVersion.Items.Add(APIDataPrase["data"][i]["mc_versions"][i2]);
            }
        }
    }
}
