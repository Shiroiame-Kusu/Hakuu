using Newtonsoft.Json.Linq;
using Hakuu.Utils;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using System.Threading;
using System.Windows.Threading;

namespace Hakuu.Windows.Pages.Server
{
    /// <summary>
    /// Interaction logic for Download.xaml
    /// </summary>
    
    public partial class Download : UiPage
    {
        public static string? API = "https://download.fastmirror.net/api/v3/";
        public static string? DownloadAPI = "https://download.fastmirror.net/download/";
        public static string? VersionAPI;
        public static string? VersionAPIStatus;
        public JObject? VersionAPIDataPrase;
        public static string? DetailedAPI;
        public static string? DetailedAPIStatus;
        public JObject? DetailedAPIDataPrase;
        public static int? DownloadableServerNumber {get; set; }
        public static int? DownloadableServerVersion { get; set; }
        public static string? ServerPath;
        private double b;

        public string? DownloadUnit { get; private set; }
        public static int a { get; set;}
        public string? CurrentServerPath { get; private set; }
        public string? DownloadFileURL { get; private set; }
        public static string? ResponseStatus { get; private set; }
        public static string? ResponseData { get; private set; }

        public static bool isDownloadFinished = true;
        

        public Download()
        {
            InitializeComponent();
            LoadAPIInfo();
            

        }
        private async void ServerDownload(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Global.Settings.Hakuu.HakuuDownloadPath))
            {
                ServerPath = Global.Settings.Hakuu.HakuuDownloadPath;
            }
            else
            {
                ServerPath = AppDomain.CurrentDomain.BaseDirectory + "Hakuu-Server";
            }
            DownloadButton.IsEnabled = false;
            Refresh.IsEnabled = false;
            ServerDownloadName.IsEnabled = false;
            ServerDownloadVersion.IsEnabled = false;
            var ServerName = ServerDownloadName.SelectedItem.ToString();
            var ServerVersion = ServerDownloadVersion.SelectedItem.ToString();
            CurrentServerPath = ServerPath + "\\" + ServerName + "\\" + ServerVersion;

            Directory.CreateDirectory(CurrentServerPath + "\\");
            if (File.Exists(CurrentServerPath + "\\server.jar"))
            {
                File.Delete(CurrentServerPath + "\\server.jar");
            }
            ServerDownloadLogTextBox.AppendText($"下载目录为:{CurrentServerPath}\n");
            try
            {
                if (!string.IsNullOrEmpty(ServerDownloadCoreVersion.SelectedItem.ToString()))
                {
                    DownloadFileURL = DownloadAPI + ServerName + "/" + ServerVersion + "/" + ServerDownloadCoreVersion.SelectedItem;
                }
            }
            catch
            {
                DownloadFileURL = DownloadAPI + ServerName + "/" + ServerVersion + "/" + DetailedAPIDataPrase?["data"]?["builds"]?[0]?["core_version"];
            }
            var DownloadStatus = DownloadFile(DownloadFileURL, CurrentServerPath + "\\server.jar");
            if (DownloadStatus == true)
            {
                ServerDownloadLogTextBox.AppendText("下载完成\n");

            }
            else
            {
                ServerDownloadLogTextBox.AppendText("下载失败\n");
                Logger.MsgBox("下载失败", "Hakuu", 0, 48);
            }

            if ((bool)AutoSetupPath.IsChecked)
            {
                Global.Settings.Server.Path = CurrentServerPath + "\\server.jar";
                if (Catalog.Server.Plugins != null) { Catalog.Server.Plugins.Load(); }
                var Loader = new Settings.Server();
                Loader.Path.Text = Global.Settings.Server.Path;
            }
            try
            {
                if (isDownloadFinished)
                {
                    Refresh.IsEnabled = true;
                    DownloadButton.IsEnabled = true;

                    ServerDownloadVersion.IsEnabled = true;
                    ServerDownloadName.IsEnabled = true;
                }
            }catch
            {

            }
        }
        private async void LoadAPIInfo()
        {   
            ServerDownloadLogTextBox.AppendText("正在获取API信息......\n");
            
            try
            {
                await RequestAPI(API);
            }
            catch
            {
                throw new HttpRequestException();
            }
            
            VersionAPI = ResponseData;
            VersionAPIStatus = ResponseStatus;
            try
            {
                
                if (VersionAPIStatus != "OK")
                {
                    Logger.MsgBox("无法连接至服务器，请检查您的网络连接", "Hakuu", 0, 48);

                }
                else
                {
                    ServerDownloadLogTextBox.AppendText("获取API信息成功\n");

                }
                
                VersionAPIDataPrase = JObject.Parse(VersionAPI);
                DownloadableServerNumber = VersionAPIDataPrase?["data"]?.Count();
                for(int i = 0; i < DownloadableServerNumber; i++)
                {
                    ServerDownloadName.Items.Add(VersionAPIDataPrase?["data"]?[i]?["name"]);
                }
            }
            catch{
                
            }

            
            
        }

        private static async Task RequestAPI(string url)
        {
            var HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36 Edg/115.0.1901.188");
            HttpClient.DefaultRequestHeaders.Add("Referer", "https://www.fastmirror.net/");
            HttpClient.DefaultRequestHeaders.Add("Host", "download.fastmirror.net");
            HttpClient.DefaultRequestHeaders.Add("Origin", "https://www.fastmirror.net");
            //Myrq.Referer = "https://www.fastmirror.net/";
            //Myrq.Host = "download.fastmirror.net";
            //Myrq.Headers.Add("Origin", "https://www.fastmirror.net");
            var Response = await HttpClient.GetAsync(url);
            var StatusCode = Response.StatusCode;
            var APIResponse = Response.Content.ReadAsStringAsync();

            ResponseData = APIResponse.Result.ToString();
            ResponseStatus = StatusCode.ToString();
        }

        private void ServerDownloadName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ServerDownloadVersion.Items.Clear();
            try
            {
                if (!string.IsNullOrEmpty(ServerDownloadName.SelectedItem.ToString()))
                {
                    ServerDownloadVersion.IsEnabled = true;
                }
                DownloadButton.IsEnabled = false;
            }
            catch
            {
                DownloadButton.IsEnabled = false;
            }
            var i = ServerDownloadName.SelectedIndex;
            if (i >= 0)
            {

                DownloadableServerVersion = VersionAPIDataPrase?["data"]?[i]?["mc_versions"]?.Count();
            }
            else
            {
                i = 0;
                DownloadableServerVersion = VersionAPIDataPrase?["data"]?[i]?["mc_versions"]?.Count();
            }
            for (int i2 = 0; i2 < DownloadableServerVersion; i2++)
            {
                ServerDownloadVersion.Items.Add(VersionAPIDataPrase?["data"]?[i]?["mc_versions"]?[i2]);
            }
        }

        private async void ServerDownloadVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(!string.IsNullOrEmpty(ServerDownloadVersion.SelectedItem.ToString()))
                {
                    var ServerName = ServerDownloadName.SelectedItem.ToString();
                    var ServerVersion = ServerDownloadVersion.SelectedItem.ToString();
                    await RequestAPI(API + ServerName + "/" + ServerVersion + "?limit=10");
                    DetailedAPI = ResponseData;
                    DetailedAPIStatus = ResponseStatus;
                    if (DetailedAPIStatus != "OK")
                    {
                        Logger.MsgBox("无法连接至服务器，请检查您的网络连接", "Hakuu", 0, 48);

                    }
                    else if (DetailedAPIStatus == "NotFound")
                    {
                        Logger.MsgBox("出现连接错误，请报告给开发者", "Hakuu", 0, 48);
                    }
                    try
                    {
                        if(!string.IsNullOrEmpty(DetailedAPI)){
                            DetailedAPIDataPrase = JObject.Parse(DetailedAPI);
                            DownloadButton.IsEnabled = true;
                        }
                    }catch{

                    }
                }
            }
            catch
            {
                DownloadButton.IsEnabled = false;
            }
        }
        private void ServerDownloadCoreVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ServerDownloadName.Items.Clear();
            ServerDownloadVersion.Items.Clear();
            ServerDownloadCoreVersion.Items.Clear();
            ServerDownloadLogTextBox.Clear();
            LoadAPIInfo();
        }

        private void FetchDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {
                Logger.MsgBox("唔......看起来这个功能还没做好呢", "Hakuu", 0, 48);
            }

            //TODO
            //await RequestAPI(API);
        }
        public bool DownloadFile(string URL, string filename)
        {
            try
            {
                isDownloadFinished = false;
                HttpWebRequest Myrq = (HttpWebRequest)WebRequest.Create(URL);
                Myrq.Referer = "https://www.fastmirror.net/";
                Myrq.Host = "download.fastmirror.net";
                Myrq.Headers.Add("Origin", "https://www.fastmirror.net");

                HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                if (DownloadProgress != null)
                {
                    DownloadProgress.Maximum = (int)totalBytes;
                }
                Stream st = myrp.GetResponseStream();
                Stream so = new FileStream(filename, FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                DownloadProgressText.Text = "";
                Task.Run(() =>
                {
                    a = 0;
                    

                    while (true)
                    {
                        
                        
                        a++;
                        if (Math.Round((double)totalDownloadedByte / 1024 / a, 2) >= 1000)
                        {
                            b = 1024 * 1024;
                            DownloadUnit = " MB/s  ";
                        }
                        else
                        {
                            b = 1024;
                            DownloadUnit = " KB/s  ";
                        }
                        DownloadProgressText.Dispatcher.Invoke(() =>
                        {
                            DownloadProgressText.Text = Math.Round((double)totalDownloadedByte / b / a, 2) + DownloadUnit + Math.Round((double)totalDownloadedByte / totalBytes * 100, 2).ToString() + "%";
                        });
                        if (totalDownloadedByte == totalBytes)
                        {
                            break;
                        }

                        Thread.Sleep(500);
                    }
                    
                    Task.FromResult(0);
                
                });
                
                while (osize > 0)
                {   
                    
                    totalDownloadedByte = osize + totalDownloadedByte;
                    DoEvents();
                    so.Write(by, 0, osize);
                    if (DownloadProgress != null)
                    {
                        DownloadProgress.Value = (int)totalDownloadedByte;
                    }
                    
                    osize = st.Read(by, 0, (int)by.Length);
                }
                so.Close();
                st.Close();
                isDownloadFinished = true;
                return true;

            }
            catch (Exception)
            {
                isDownloadFinished = true;
                return false;
                throw;
                
            }
        }
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrames), frame);
            try { Dispatcher.PushFrame(frame); }
            catch (InvalidOperationException) { }
        }
        private static object? ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }

        
    }
}
