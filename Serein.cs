﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace Serein
{
    public partial class Serein : Form
    {
        public Serein()
        {
            InitializeComponent();
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "console.html"))
            {
                MessageBox.Show($"文件  {AppDomain.CurrentDomain.BaseDirectory}console.html  已丢失");
                System.Environment.Exit(0);
            }
            string VERSION = "Testing 2022";
            PanelConsoleWebBrowser.Navigate(@"file:\\\" + AppDomain.CurrentDomain.BaseDirectory + "console.html?from=panel");
            BotWebBrowser.Navigate(@"file:\\\" + AppDomain.CurrentDomain.BaseDirectory + "console.html?from=bot");
            Global.PanelConsoleWebBrowser = PanelConsoleWebBrowser;
            Global.BotWebBrowser = BotWebBrowser;
            SettingSereinVersion.Text = $"Version:{VERSION}";
        }
        public object[] UpdateSettings()
        {
            Settings.Server.Path = SettingServerPath.Text;
            Settings.Server.EnableRestart = SettingServerEnableRestart.Checked;
            Settings.Server.EnableOutputCommand = SettingServerEnableOutputCommand.Checked;
            Settings.Server.EnableLog = SettingServerEnableLog.Checked;
            Settings.Server.OutputStyle = SettingServerOutputStyle.SelectedIndex;
            Settings.Bot.Path = SettingBotPath.Text;
            Settings.Bot.EnableLog = SettingBotEnableLog.Checked;
            Settings.Bot.GivePermissionToAllAdmin = SettingBotGivePermissionToAllAdmin.Checked;
            Settings.Bot.ListenPort = SettingBotListenPort.DecimalPlaces;
            Settings.Bot.SendPort = SettingBotSendPort.DecimalPlaces;
            Settings.Serein.EnableGetAnnouncement = SettingSereinEnableGetAnnouncement.Checked;
            Settings.Serein.EnableGetUpdate = SettingSereinEnableGetUpdate.Checked;
            object[] Setting = new object[0];
            return Setting;
        }
        private void SettingBotSupportedLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Mrs4s/go-cqhttp");
        }
        private void SettingServerPathSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "支持的文件(*.exe *.bat)|*.exe;*.bat"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SettingServerPath.Text = dialog.FileName;
                Server.Path = dialog.FileName;
            }
        }
        private void SettingBotPathSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "go-cqhttp可执行文件|go-cqhttp.exe;go-cqhttp_windows_armv7.exe;go-cqhttp_windows_386.exe;go-cqhttp_windows_arm64.exe;go-cqhttp_windows_amd64.exe"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SettingBotPath.Text = dialog.FileName;
            }
        }
        delegate void PanelConsoleWebBrowser_Delegate(object[] objects);
        private void PanelConsoleWebBrowser_AppendText(object[] objects)
        {
            PanelConsoleWebBrowser.Document.InvokeScript("AppendText", objects);
            
        }
        public void PanelConsoleWebBrowser_Invoke(string str)
        {
            object[] objects1 = { str };
            object[] objects2 = { objects1 };
            Invoke((PanelConsoleWebBrowser_Delegate)PanelConsoleWebBrowser_AppendText, objects2);
        }
        private void PanelControlStart_Click(object sender, EventArgs e)
        {
            Server.Start();
        }
        private void PanelControlStop_Click(object sender, EventArgs e)
        {
            Server.Stop();
        }
        private void PanelControlRestart_Click(object sender, EventArgs e)
        {
            Server.RestartRequest();
        }
        private void PanelControlKill_Click(object sender, EventArgs e)
        {
            Server.Kill();
        }
        private void PanelConsoleEnter_Click(object sender, EventArgs e)
        {
            Server.InputCommand(PanelConsoleInput.Text);
            PanelConsoleInput.Text = "";
        }
        private void PanelConsoleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Server.InputCommand(PanelConsoleInput.Text);
                PanelConsoleInput.Text = "";
            }
        }
    }
}
