using Newtonsoft.Json.Linq;
using Serein.Utils;
using System;
using System.IO;
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
using Downloader;
using System.Reflection;
using System.ComponentModel;

namespace Serein.Windows.Pages.Server
{
    /// <summary>
    /// Interaction logic for Download.xaml
    /// </summary>
    
    public partial class Download : UiPage
    {
        public static string? API = "https://download.fastmirror.net/api/v3/";
        public static string APIResult {  get; set; }
        public static string APIStatusCode { get; set; }
        public static int DownloadableServerNumber {get; set; }
        public static int DownloadableServerVersion { get; set; }
        public static string DetailedAPIStatusCode { get; private set; }
        public static string DetailedAPIResult { get; private set; }
        public static string ServerPath = AppDomain.CurrentDomain.BaseDirectory + "Serein-Server";
        public static long TotalByteToReceive {get; set; }
        public static long ByteReceived {get; set; }
        public static string ReceivedPrecent {get; set; }

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
                ServerDownloadLogTextBox.AppendText($"正在下载服务端\n名称: {ServerName}\n版本: {ServerVersion}\n");

            ServerDownloadLogTextBox.AppendText($"下载目录为:{ServerPath}\n");
            try
                {
                    await RequestDetailedAPI(API + ServerName + "/" + ServerVersion);
                    if (DetailedAPIStatusCode != "OK")
                    {
                        Logger.MsgBox("无法连接至服务器，请检查您的网络连接", "Serein", 0, 48);

                    }else if(DetailedAPIStatusCode == "NotFound") {
                    Logger.MsgBox("出现连接错误，请报告给开发者", "Serein", 0, 48);
                }
                else
                {
                    
                }
                JObject DetailedAPIPrase = JObject.Parse(DetailedAPIResult);
                Directory.CreateDirectory(ServerPath + "\\");
                await Downloader("https://download.fastmirror.net/download/" + ServerName + "/" + ServerVersion + "/" + DetailedAPIPrase["data"]["builds"]["core_version"]);
                }catch (Exception ex){
                

                }

            
            
        }
        private static async Task Downloader(string url)
        {   
            
            var downloadOpt = new DownloadConfiguration()
            {
                BufferBlockSize = 10240, // 通常，主机最大支持8000字节，默认值为8000。
                ChunkCount = 8, // 要下载的文件分片数量，默认值为1
                MaximumBytesPerSecond = 0, // 下载速度限制为1MB/s，默认值为零或无限制
                MaxTryAgainOnFailover = 5, // 失败的最大次数
                ParallelDownload = true, // 下载文件是否为并行的。默认值为false
                MaximumMemoryBufferBytes = 50,
                ReserveStorageSpaceBeforeStartingDownload = true,

                Timeout = 1000, // 每个 stream reader  的超时（毫秒），默认值是1000
                RequestConfiguration = // 定制请求头文件
                {        
                    Accept = "*/*",        
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,        
                    CookieContainer =  new CookieContainer(), // Add your cookies
                    Headers = new WebHeaderCollection(), // Add your custom headers
                    KeepAlive = true,        
                    ProtocolVersion = HttpVersion.Version11, // Default value is HTTP 1.1
                    UseDefaultCredentials = false,        
                    UserAgent = $"DownloaderSample/{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}"    
                }
            };
            var downloader = new DownloadService(downloadOpt);
            downloader.DownloadStarted += OnDownloadStarted;
            //downloader.ChunkDownloadProgressChanged += OnChunkDownloadProgressChanged;
            downloader.DownloadProgressChanged += OnDownloadProgressChanged;
            downloader.DownloadFileCompleted += OnDownloadFileCompleted;

            await downloader.DownloadFileTaskAsync(url, ServerPath + "\\server.jar");
        }

        private static void OnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            
        }

        private static void OnDownloadProgressChanged(object? sender, Downloader.DownloadProgressChangedEventArgs e)
        {
            Download download = new Download();
            ByteReceived = e.ReceivedBytesSize;
            ReceivedPrecent = (ByteReceived / TotalByteToReceive).ToString();
            download.ServerDownloadLogTextBox.AppendText($"已下载{ReceivedPrecent}");
        }

        private static void OnDownloadStarted(object? sender, DownloadStartedEventArgs e)
        {
            TotalByteToReceive = e.TotalBytesToReceive;
            

        }

        private async void LoadAPIInfo()
        {
            await RequestAPI(API); 
            try{
                ServerDownloadLogTextBox.AppendText(APIStatusCode+"\n");
                if (APIStatusCode != "OK")
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

        private void ServerDownloadVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ServerDownloadName.SelectedItem.ToString()))
            {
                DownloadButton.IsEnabled = true;
            }
            
        }
    }
}
