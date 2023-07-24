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
        private double b;

        public string DownloadUnit { get; private set; }
        public static long TotalByteToReceive {get; set; }
        public static long ByteReceived {get; set; }
        public static string ReceivedPrecent {get; set; }
        public static int a { get; set;}
        public Download()
        {
            InitializeComponent();
            LoadAPIInfo();
            
        }
        public static void Timer_Tick(object sender, EventArgs e)
        {
            a++;
        }
        private async void ServerDownload(object sender, RoutedEventArgs e)
        {
            DownloadButton.IsEnabled = false;
            ServerDownloadLogTextBox.Clear();
            var ServerName = ServerDownloadName.SelectedItem.ToString();
                var ServerVersion = ServerDownloadVersion.SelectedItem.ToString();
                ServerDownloadLogTextBox.AppendText($"正在下载服务端\n名称: {ServerName}\n版本: {ServerVersion}\n");
            ServerPath = ServerPath + "\\" + ServerName + "\\" + ServerVersion;
            if (File.Exists(ServerPath + "\\server.jar"))
            {
                File.Delete(ServerPath + "\\server.jar");
            }
            ServerDownloadLogTextBox.AppendText($"下载目录为:{ServerPath}\n");
            try{
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
                
                }catch (Exception ex){
                

                }
            JObject DetailedAPIPrase = JObject.Parse(DetailedAPIResult);
            Directory.CreateDirectory(ServerPath + "\\");
            
            
            var url = "https://download.fastmirror.net/download/" + ServerName + "/" + ServerVersion + "/" + DetailedAPIPrase["data"]["builds"][0]["core_version"];
            var DownloadStatus = DownloadFile(url, ServerPath + "\\server.jar");
            if(DownloadStatus == true)
            {
                ServerDownloadLogTextBox.AppendText("下载完成\n");
            }
            else
            {
                ServerDownloadLogTextBox.AppendText("下载失败\n");
            }
            DownloadButton.IsEnabled = true;
            Global.Settings.Server.Path = ServerPath + "\\server.jar";

        }
        private async void LoadAPIInfo()
        {   ServerDownloadLogTextBox.AppendText("正在获取API信息......\n");
            await RequestAPI(API); 
            try{
                
                if (APIStatusCode != "OK")
                {
                    Logger.MsgBox("无法连接至服务器，请检查您的网络连接", "Serein", 0, 48);

                }
                else
                {
                    ServerDownloadLogTextBox.AppendText("获取API信息成功\n");
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
        public bool DownloadFile(string URL, string filename)
        {
            try
            {   
                HttpWebRequest Myrq = (HttpWebRequest)HttpWebRequest.Create(URL);
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
                        
                        Thread.Sleep(1000);
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
                
                return true;
            }
            catch (Exception)
            {
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

    }
}
