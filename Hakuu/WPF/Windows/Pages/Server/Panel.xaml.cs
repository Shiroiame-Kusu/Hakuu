﻿using Hakuu.Utils;
using Hakuu.Core.Server;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using Wpf.Ui.Controls;
using System.Diagnostics;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using Esprima.Ast;

namespace Hakuu.Windows.Pages.Server
{
    
    public partial class Panel : UiPage
    {
        private readonly Timer _updateInfoTimer = new Timer(2000) { AutoReset = true };
        public static string ServerLog {get; set;}
        private string CommandHeaderSelected = "";
        private static bool IsBackgroundTaskRunning = false;
        private string? OldServerType = null;
        public Panel()
        {
            InitializeComponent();
            if (!IsBackgroundTaskRunning)
            {
                IsBackgroundTaskRunning = true;
                Task.Run(() => {
                    
                    while (true)
                    {
                        
                        string? ServerType = null;
                        if (!ServerManager.Status)
                        {
                            try
                            {
                                ServerType = Global.Settings.Server.Path.Substring(Global.Settings.Server.Path.Length - 3);
                            }
                            catch
                            {

                            }
                            if(ServerType != OldServerType)
                            {
                                switch (ServerType)
                                {
                                    case "jar":
                                        Dispatcher.InvokeAsync(() => { MEMSettings.Visibility = Visibility.Visible; }, System.Windows.Threading.DispatcherPriority.Background);

                                        break;
                                    default:
                                        Dispatcher.InvokeAsync(() => { MEMSettings.Visibility = Visibility.Collapsed; }, System.Windows.Threading.DispatcherPriority.Background);
                                        break;
                                }
                                OldServerType = ServerType;
                            }
                            
                        }
                        
                        System.Threading.Thread.Sleep(500);
                    }
                });
            }
            

            
            
            AutoJVMOptimization.IsChecked = Global.Settings.Server.AutoJVMOptimization;
            _updateInfoTimer.Elapsed += (_, _) => UpdateInfos();
            _updateInfoTimer.Start();
            PanelRichTextBox.Document.Blocks.Clear();
            MaxRAM.Text = Global.Settings.Server.MaxRAM;
            lock (Catalog.Server.Cache)
            {
                Catalog.Server.Cache.ForEach(
                    (line) => Dispatcher.Invoke(() => Append(LogPreProcessing.Color(line)))
                );
            }
            Catalog.Server.Panel = this;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            
            ServerManager.Start();
            UpdateInfos();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
            => ServerManager.Stop();

        private void Restart_Click(object sender, RoutedEventArgs e)
            => ServerManager.RequestRestart();

        private void Kill_Click(object sender, RoutedEventArgs e)
        {
            ServerManager.Kill();
            UpdateInfos();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            ServerManager.InputCommand(CommandHeaderSelected + InputBox.Text);
            InputBox.Text = "";
        }

        private void InputBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    ServerManager.InputCommand(CommandHeaderSelected + InputBox.Text);
                    InputBox.Text = "";
                    e.Handled = true;
                    break;
                case Key.Up:
                case Key.PageUp:
                    if (ServerManager.CommandHistoryIndex > 0)
                    {
                        ServerManager.CommandHistoryIndex--;
                    }
                    if (ServerManager.CommandHistoryIndex >= 0 && ServerManager.CommandHistoryIndex < ServerManager.CommandHistory.Count)
                    {
                        InputBox.Text = ServerManager.CommandHistory[ServerManager.CommandHistoryIndex];
                    }
                    InputBox.SelectionStart = InputBox.Text.Length;
                    e.Handled = true;
                    break;
                case Key.Down:
                case Key.PageDown:
                    if (ServerManager.CommandHistoryIndex < ServerManager.CommandHistory.Count)
                    {
                        ServerManager.CommandHistoryIndex++;
                    }
                    if (ServerManager.CommandHistoryIndex >= 0 && ServerManager.CommandHistoryIndex < ServerManager.CommandHistory.Count)
                    {
                        InputBox.Text = ServerManager.CommandHistory[ServerManager.CommandHistoryIndex];
                    }
                    else if (ServerManager.CommandHistoryIndex == ServerManager.CommandHistory.Count && ServerManager.CommandHistory.Count != 0)
                    {
                        InputBox.Text = string.Empty;
                    }
                    InputBox.SelectionStart = InputBox.Text.Length;
                    e.Handled = true;
                    break;
            }
        }

        public void Append(Paragraph paragraph)
        {
            Dispatcher.Invoke(() =>
                    {
                        PanelRichTextBox.Document = PanelRichTextBox.Document ?? new();
                        PanelRichTextBox.Document.Blocks.Add(paragraph);
                        while (PanelRichTextBox.Document.Blocks.Count > Global.Settings.Hakuu.MaxCacheLines)
                        {
                            PanelRichTextBox.Document.Blocks.Remove(PanelRichTextBox.Document.Blocks.FirstBlock);
                        }
                        PanelRichTextBox.ScrollToEnd();
                    }, System.Windows.Threading.DispatcherPriority.Background); 
            
        }

        private void UpdateInfos()
            => Dispatcher.Invoke(() =>
            {
                
                Status.Content = ServerManager.Status ? "已启动" : "未启动";
                if (ServerManager.Status)
                {
                    Version.Content = ServerManager.Motd != null && !string.IsNullOrEmpty(ServerManager.Motd.Version) ? ServerManager.Motd.Version : "-";
                    PlayCount.Content = ServerManager.Motd != null ? $"{ServerManager.Motd.OnlinePlayer}/{ServerManager.Motd.MaxPlayer}" : "-";
                }
                else
                {
                    PlayCount.Content = "-";
                    Version.Content = "-";
                }
                Difficulity.Content = ServerManager.Status && !string.IsNullOrEmpty(ServerManager.Difficulty) ? ServerManager.Difficulty : "-";
                Time.Content = ServerManager.Status ? ServerManager.Time : "-";
                CPUPerc.Content = ServerManager.Status ? "%" + ServerManager.CPUUsage.ToString("N1") : "-";
                //if (string.IsNullOrEmpty(Global.Settings.Server.CustomizedTitle))
                switch (string.IsNullOrEmpty(Global.Settings.Server.CustomizedTitle)){
                    case true:
                        Catalog.MainWindow?.UpdateTitle(ServerManager.Status ? ServerManager.StartFileName : null);
                        break;
                    case false:
                        Catalog.MainWindow?.UpdateTitle(Global.Settings.Server.CustomizedTitle);
                        break;
                }
                
                JavaVersion.Content = ServerManager.JavaVersion;
                
            });

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {   
                if(string.IsNullOrEmpty(Global.Settings.Server.Path) != true)
                {
                    
                    Process.Start("Explorer.exe", Path.GetDirectoryName(Global.Settings.Server.Path));
                }
                else
                {
                    Logger.MsgBox("打开文件夹失败，请检查启动路径是否为空", "Hakuu", 0, 48);
                }
                
            }catch(Exception ex)
            {
                Logger.MsgBox("打开文件夹失败，请检查启动路径是否为空", "Hakuu", 0 , 48);
            }
            
        }

        private void AutoJVMOptimization_Click(object sender, RoutedEventArgs e)
        {
            Global.Settings.Server.AutoJVMOptimization = (bool)AutoJVMOptimization.IsChecked;
        }

        private void PanelRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
                TextRange textRange = new TextRange(
                    // TextPointer to the start of content in the RichTextBox.
                    PanelRichTextBox.Document.ContentStart,
                    // TextPointer to the end of content in the RichTextBox.
                    PanelRichTextBox.Document.ContentEnd
                );

                // The Text property on a TextRange object returns a string
                // representing the plain text content of the TextRange.
                ServerLog = textRange.Text;
        }

        private void CommandHeaderSelected_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CommandHeader.SelectedIndex)
            {
                case 0:
                    CommandHeaderSelected = "";
                    break;
                case 1:
                    CommandHeaderSelected = "say ";
                    break;
                case 2:
                    CommandHeaderSelected = "kill ";
                    break;
                case 3:
                    CommandHeaderSelected = "effect ";
                    break;
                case 4:
                    CommandHeaderSelected = "whitelist ";
                    break;
                case 5:
                    CommandHeaderSelected = "broadcast ";
                    break;
            }
        }

        private void MaxRAM_TextChanged(object sender, TextChangedEventArgs e)
        {
            Global.Settings.Server.MaxRAM = MaxRAM.Text;
        }
    }
}
