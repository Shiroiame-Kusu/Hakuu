﻿using Hakuu.Utils;
using Hakuu.Core.Generic;
using Hakuu.Core.Server;
using System.Timers;
using System.Windows;
using Wpf.Ui.Controls;
using Hakuu.Windows.Pages.Server;

namespace Hakuu.Windows.Pages
{
    public partial class Home : UiPage
    {
        private readonly Timer _timer = new(5000)
        {
            AutoReset = true
        };

        public Home()
        {
            InitializeComponent();
            _timer.Elapsed += (_, _) => Update();
            _timer.Start();
        }

        private void Update()
        {
            Dispatcher.Invoke(() =>
            {
                RAM_Percent.Text = $"{(double)SystemInfo.UsedRAM / 1024:N1} / {(double)SystemInfo.TotalRAM / 1024:N1} GB   {SystemInfo.RAMUsage:N1}%";
                RAM_Percent_Ring.Progress = SystemInfo.RAMUsage;
                Server_Status.Text = ServerManager.Status ? "已启动" : "未启动";
                Server_Time.Text = ServerManager.Status ? ServerManager.Time : "-";
                Server_Occupancy.Text = ServerManager.Status ? ServerManager.CPUUsage.ToString("N1") + "%" : "-";
                if (ServerManager.Status)
                {
                    Server_Online.Text = ServerManager.Motd != null && ServerManager.Motd.IsSuccessful ? $"{ServerManager.Motd.OnlinePlayer}/{ServerManager.Motd.MaxPlayer}" : "获取失败";
                }
                else
                {
                    Server_Online.Text = "-";
                }
                string CPUPercentage = SystemInfo.CPUUsage.ToString("N1");
                CPU_Percent.Text = CPUPercentage + "%";
                CPU_Percent_Bar.Value = double.TryParse(CPUPercentage, out double _Result1) ? _Result1 : 0;
                CPU_Percent_Bar.IsIndeterminate = false;
                CPU_Name.Text = SystemInfo.CPUName;
            });
        }


        private void CardAction_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as CardAction)?.Tag)
            {
                case "Server":
                    if (Global.Settings.Hakuu.PagesDisplayed.ServerPanel)
                    {

                        Catalog.MainWindow?.Navigation.Navigate(1);
                        Catalog.Server.Container?.Navigation?.Navigate(0);
                        ServerManager.Start();
                    }
                    break;
                case "Console":
                    if (Global.Settings.Hakuu.PagesDisplayed.ServerPanel)
                    {
                        Catalog.MainWindow?.Navigation.Navigate(1);
                        Catalog.Server.Container?.Navigation?.Navigate(0);
                    }
                    break;
                case "Schedule":
                    if (Global.Settings.Hakuu.PagesDisplayed.Schedule)
                    {
                        Catalog.MainWindow?.Navigation.Navigate(2);
                        Catalog.Function.Container?.Navigation?.Navigate(0);
                    }
                    break;
                case "Download":
                    if (Global.Settings.Hakuu.PagesDisplayed.ServerPanel)
                    {
                        Catalog.MainWindow?.Navigation.Navigate(1);
                        Catalog.Server.Container?.Navigation?.Navigate(2);
                    }
                    break;
                case "ServerSettings":
                    if (Global.Settings.Hakuu.PagesDisplayed.Settings)
                    {
                        Catalog.MainWindow?.Navigation.Navigate(3);
                        Catalog.Settings.Container?.Navigation?.Navigate(0);
                    }
                    break;
                case "Settings":
                    if (Global.Settings.Hakuu.PagesDisplayed.Settings)
                    {
                        Catalog.MainWindow?.Navigation.Navigate(3);
                        Catalog.Settings.Container?.Navigation?.Navigate(1);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
