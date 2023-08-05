using Newtonsoft.Json.Linq;
using Serein.Utils;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;
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
using System.Windows.Threading;
using System.Timers;
using System.Collections.Generic;

namespace Serein.Windows.Pages.Server
{
    /// <summary>
    /// Interaction logic for Download.xaml
    /// </summary>
    
    public partial class Download : UiPage
    {
        public static string? API = "https://download.fastmirror.net/api/v3/";
        public static string? VersionAPI;
        public static string? VersionAPIStatus;
        public JObject VersionAPIDataPrase;
        public static int DownloadableServerNumber {get; set; }
        public static int DownloadableServerVersion { get; set; }
        public static string DetailedAPIStatusCode { get; private set; }
        public static string DetailedAPIResult { get; private set; }
        public static string ServerPath = AppDomain.CurrentDomain.BaseDirectory + "Serein-Server";
        private double b;

        public string DownloadUnit { get; private set; }
        public static int a { get; set;}
        public string CurrentServerPath { get; private set; }
        public string DownloadFileURL { get; private set; }
        public static string ResponseStatus { get; private set; }
        public static string ResponseData { get; private set; }

        public static bool isDownloadFinished = true;
        

        public Download()
        {
            InitializeComponent();
            LoadAPIInfo();
            

        }
        private async void ServerDownload(object sender, RoutedEventArgs e)
        {   

            /*DownloadButton.IsEnabled = false;
            ServerDownloadLogTextBox.Clear();
            var ServerName = ServerDownloadName.SelectedItem.ToString();
                var ServerVersion = ServerDownloadVersion.SelectedItem.ToString();
                ServerDownloadLogTextBox.AppendText($"正在下载服务端\n名称: {ServerName}\n版本: {ServerVersion}\n");
            CurrentServerPath = ServerPath + "\\" + ServerName + "\\" + ServerVersion;
            if (File.Exists(CurrentServerPath + "\\server.jar"))
            {
                File.Delete(CurrentServerPath + "\\server.jar");
            }
            ServerDownloadLogTextBox.AppendText($"下载目录为:{CurrentServerPath}\n");
            
            JObject DetailedAPIPrase = JObject.Parse(DetailedAPIResult);
            Directory.CreateDirectory(CurrentServerPath + "\\");
            try
            {
                if (!string.IsNullOrEmpty(ServerDownloadCoreVersion.SelectedItem.ToString()))
                {
                    DownloadFileURL = "https://download.fastmirror.net/download/" + ServerName + "/" + ServerVersion + "/" + ServerDownloadCoreVersion.SelectedItem;
                }
                else
                {
                    DownloadFileURL = "https://download.fastmirror.net/download/" + ServerName + "/" + ServerVersion + "/" + DetailedAPIPrase["data"]["builds"][0]["core_version"];
                }
            }catch(Exception ex)
            {

            }
            

            var DownloadStatus = DownloadFile(DownloadFileURL, CurrentServerPath + "\\server.jar");
            if(DownloadStatus == true)
            {
                ServerDownloadLogTextBox.AppendText("下载完成\n");
            }
            else
            {
                ServerDownloadLogTextBox.AppendText("下载失败\n");
            }
            try
            {
                if(!string.IsNullOrEmpty(ServerDownloadName.SelectedItem.ToString()) && !string.IsNullOrEmpty(ServerDownloadVersion.SelectedItem.ToString()))
                    {
                        DownloadButton.IsEnabled = true;
                    }
            }catch
            {

            }
            
            
            if (AutoSetupPath.IsChecked == true)
            {
                Global.Settings.Server.Path = CurrentServerPath + "\\server.jar";
            }*/
            

        }
        private async void LoadAPIInfo()
        {   
            ServerDownloadLogTextBox.AppendText("正在获取API信息......\n");
            await RequestAPI(API);
            VersionAPI = ResponseData;
            VersionAPIStatus = ResponseStatus;
            try
            {
                
                if (VersionAPIStatus != "OK")
                {
                    Logger.MsgBox("无法连接至服务器，请检查您的网络连接", "Serein", 0, 48);

                }
                else
                {
                    ServerDownloadLogTextBox.AppendText("获取API信息成功\n");
                
                }
                
                VersionAPIDataPrase = JObject.Parse(VersionAPI);
                DownloadableServerNumber = VersionAPIDataPrase["data"].Count();
                for(int i = 0; i < DownloadableServerNumber; i++)
                {
                    ServerDownloadName.Items.Add(VersionAPIDataPrase["data"][i]["name"]);
                }
            }
            catch(Exception ex){
                
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
            var APIResponse =  Response.Content.ReadAsStringAsync();

            ResponseData = APIResponse.Result.ToString();
            ResponseStatus = StatusCode.ToString();
            
        }
        /*private static async Task RequestDetailedAPI(string url)
        {
            var HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36 Edg/115.0.1901.188");
            HttpClient.DefaultRequestHeaders.Add("Referer", "https://www.fastmirror.net/");
            HttpClient.DefaultRequestHeaders.Add("Host", "download.fastmirror.net");
            HttpClient.DefaultRequestHeaders.Add("Origin", "https://www.fastmirror.net");
            var Response = await HttpClient.GetAsync(url);
            var StatusCode = Response.StatusCode;
            var Header = Response.Headers;
            var APIResponse = Response.Content.ReadAsStringAsync();
            DetailedAPIStatusCode = StatusCode.ToString();
            DetailedAPIResult = APIResponse.Result.ToString();

        }*/

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
            DownloadableServerVersion = VersionAPIDataPrase["data"][i]["mc_versions"].Count();
            for (int i2 = 0; i2 < DownloadableServerVersion; i2++)
            {
                ServerDownloadVersion.Items.Add(VersionAPIDataPrase["data"][i]["mc_versions"][i2]);
            }
            /*DownloadButton.IsEnabled = false;
            var i = ServerDownloadName.SelectedIndex;
            JObject APIDataPrase = JObject.Parse(APIResult);
            DownloadableServerVersion = APIDataPrase["data"][i]["mc_versions"].Count();
            ServerDownloadVersion.Items.Clear();
            ServerDownloadCoreVersion.Items.Clear();
            for (int i2 = 0;i2 < DownloadableServerVersion; i2++)
            {
                ServerDownloadVersion.Items.Add(APIDataPrase["data"][i]["mc_versions"][i2]);
            }

            DownloadButton.IsEnabled = false;
            CoreVersion.Visibility = Visibility.Collapsed;
            ServerDownloadVersion.IsEnabled = true;*/
        }

        private async void ServerDownloadVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*CoreVersion.Visibility = Visibility.Collapsed;
            ServerDownloadCoreVersion.IsEnabled = false;
            DownloadButton.IsEnabled = false;
            ServerDownloadCoreVersion.Items.Clear();
            
            try
            {   var ServerName = ServerDownloadName.SelectedItem.ToString();
                var ServerVersion = ServerDownloadVersion.SelectedItem.ToString();
                await RequestDetailedAPI(API + ServerName + "/" + ServerVersion + "?limit=10");
                if (DetailedAPIStatusCode != "OK")
                {
                    Logger.MsgBox("无法连接至服务器，请检查您的网络连接", "Serein", 0, 48);

                }
                else if (DetailedAPIStatusCode == "NotFound")
                {
                    Logger.MsgBox("出现连接错误，请报告给开发者", "Serein", 0, 48);
                }
            }
            catch (Exception ex)
            {


            }
            
            JObject DetailedAPIPrase = JObject.Parse(DetailedAPIResult);
            try
            {   

                if(!string.IsNullOrEmpty(DetailedAPIPrase["data"]["builds"][0]["core_version"].ToString())) {
                    CoreVersion.Visibility = Visibility.Visible;
                    for (int i = 0;i < DetailedAPIPrase["data"]["builds"].Count(); i++)
                    {   
                        ServerDownloadCoreVersion.Items.Add(DetailedAPIPrase["data"]["builds"][i]["core_version"]);
                    }
                    ServerDownloadCoreVersion.IsEnabled = true;
                    DownloadButton.IsEnabled = false;
                }
                else
                {
                    CoreVersion.Visibility = Visibility.Collapsed;
                    if (!string.IsNullOrEmpty(ServerDownloadName.SelectedItem.ToString()) && !string.IsNullOrEmpty(ServerDownloadVersion.SelectedItem.ToString()) && isDownloadFinished == true)
                    {
                        DownloadButton.IsEnabled = true;

                    }
                }
            }catch(Exception ex)
            {
                DownloadButton.IsEnabled = false;
            }
            */
        }
        private void ServerDownloadCoreVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            
        }
        public bool DownloadFile(string URL, string filename)
        {
            try
            {
                isDownloadFinished = false;
                HttpWebRequest Myrq = (HttpWebRequest)HttpWebRequest.Create(URL);
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
                        if(totalDownloadedByte == totalBytes)
                        {
                            break;
                        }
                        Thread.Sleep(1000);
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
        private static object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAPIInfo();
        }

        private async void FetchDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException();
            }catch(Exception) {
                Logger.MsgBox("唔......看起来这个功能还没做好呢", "Serein", 0, 48);
            }
            
            //TODO
            //await RequestAPI(API);
        }
    }
}
