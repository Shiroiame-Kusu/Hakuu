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
using System.Threading;

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
        public static DownloadConfiguration downloadOpt = new DownloadConfiguration()
        {
            MaxTryAgainOnFailover = 5, // 失败的最大次数
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
                Task.Run(() =>
                {
                    while (true)
                    { 
                        Thread.Sleep(1000);

                    }
                });
                    var url = "https://download.fastmirror.net/download/" + ServerName + "/" + ServerVersion + "/" + DetailedAPIPrase["data"]["builds"]["core_version"];
                await DownloadFileAsync(url,ServerPath + "\\server.jar");
                }
            catch (Exception ex){
                

                }

            
            
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
        public static async Task<bool> DownloadFileAsync(string url, string fileName, Action<double> progress = default, CancellationToken cancelationToken = default)
        {
            try
            {
                // 使用HttpClient类创建一个HTTP客户端，指定不使用代理,并设置一个 CookieContainer
                using (var httpClient = new HttpClient(new HttpClientHandler() { CookieContainer = new CookieContainer(), UseProxy = false }))
                //using (var httpClient = new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler() { Proxy = null, CookieContainer = new System.Net.CookieContainer() }))
                {
                    // 发送GET请求，并等待响应
                    var response = await httpClient.GetAsync(new Uri(url), HttpCompletionOption.ResponseHeadersRead);
                    // 判断请求是否成功,如果失败则返回 false
                    if (!response.IsSuccessStatusCode)
                    {
                        return (false);
                    }
                    // 获取响应内容长度
                    long contentLength = response.Content.Headers.ContentLength ?? 0;
                    // 创建一个文件流，并将响应内容写入文件流
                    using (var fs = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    {
                        // 创建一个缓冲区，大小为64KB
                        byte[] buffer = new byte[65536];
                        // 获取响应流
                        var httpStream = await response.Content.ReadAsStreamAsync();
                        // 定义变量，用于记录每次读取的字节数
                        int readLength = 0;
                        // 循环异步读取响应流的内容，直到读取完毕
                        while ((readLength = await httpStream.ReadAsync(buffer, 0, buffer.Length, cancelationToken)) > 0)
                        {
                            // 检查是否已经取消了任务
                            if (cancelationToken.IsCancellationRequested)
                            {
                                // 如果任务已经取消，关闭文件流，并删除已经下载的文件
                                fs.Close();
                                File.Delete(fileName);
                                return (false);
                            }
                            await fs.WriteAsync(buffer, 0, readLength, cancelationToken);
                            progress?.Invoke(Math.Round((double)fs.Length / contentLength * 100, 2));
                        }
                    }
                }
                // 返回true表示文件下载成功
                return (true);
            }
            catch (Exception)
            {
                // 返回false表示文件下载失败
                return (false);
            }
        }
    }
}
