using Serein.Utils;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Serein.Windows.Pages.Settings
{
    public partial class Serein : UiPage
    {
        private readonly bool _loaded;

        public Serein()
        {
            InitializeComponent();
            Load();
            _loaded = true;
            Catalog.Settings.Serein = this;
            
        }

        private void Load()
        {
            UseDarkTheme.IsChecked = Global.Settings.Serein.UseDarkTheme;
            EnableGetUpdate.IsChecked = Global.Settings.Serein.EnableGetUpdate;
            AutoUpdate.IsChecked = Global.Settings.Serein.AutoUpdate && Global.Settings.Serein.EnableGetUpdate;
            AutoUpdate.IsEnabled = EnableGetUpdate.IsChecked ?? false;
            ThemeFollowSystem.IsChecked = Global.Settings.Serein.ThemeFollowSystem;
            MaxCacheLines.Value = Global.Settings.Serein.MaxCacheLines;
            Version.Text = "当前版本：" + Global.VERSION;
            BuildInfo.Text = Global.BuildInfo.ToString();
            if (string.IsNullOrEmpty(Global.Settings.Serein.SereinDownloadPath) || Global.Settings.Serein.SereinDownloadPath == AppDomain.CurrentDomain.BaseDirectory + "Serein-Server")
            {
                SereinDownloadPath.Text = "\\\\Serein-Server";
            }
        }

        private void EnableGetUpdate_Click(object sender, RoutedEventArgs e)
        {
            AutoUpdate.IsEnabled = EnableGetUpdate.IsChecked ?? false;
            AutoUpdate.IsChecked = (AutoUpdate.IsChecked ?? false) ? EnableGetUpdate.IsChecked ?? false : false;
            Global.Settings.Serein.EnableGetUpdate = EnableGetUpdate.IsChecked ?? false;
            Global.Settings.Serein.AutoUpdate = AutoUpdate.IsChecked ?? false;
        }

        private void AutoUpdate_Click(object sender, RoutedEventArgs e)
            => Global.Settings.Serein.AutoUpdate = AutoUpdate.IsChecked ?? false;

        private void ThemeFollowSystem_Click(object sender, RoutedEventArgs e)
        {
            UseDarkTheme.IsChecked = (UseDarkTheme.IsChecked ?? false) && (ThemeFollowSystem.IsChecked ?? false) ? false : UseDarkTheme.IsChecked;
            Global.Settings.Serein.UseDarkTheme = UseDarkTheme.IsChecked ?? false;
            Global.Settings.Serein.ThemeFollowSystem = ThemeFollowSystem.IsChecked ?? false;
            Theme.Apply(Global.Settings.Serein.UseDarkTheme ? ThemeType.Dark : ThemeType.Light);
        }

        private void UseDarkTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeFollowSystem.IsChecked = (ThemeFollowSystem.IsChecked ?? false) && (UseDarkTheme.IsChecked ?? false) ? false : ThemeFollowSystem.IsChecked;
            Global.Settings.Serein.UseDarkTheme = UseDarkTheme.IsChecked ?? false;
            Global.Settings.Serein.ThemeFollowSystem = ThemeFollowSystem.IsChecked ?? false;
            Theme.Apply(Global.Settings.Serein.UseDarkTheme ? ThemeType.Dark : ThemeType.Light);
        }

        public void UpdateVersion(string text)
            => Dispatcher.Invoke(() => Version.Text = "当前版本：" + Global.VERSION + text);

        private void MaxCacheLines_TextChanged(object sender, TextChangedEventArgs e)
            => Global.Settings.Serein.MaxCacheLines = _loaded ? (int)MaxCacheLines.Value : Global.Settings.Serein.MaxCacheLines;

        private void WelPage_Click(object sender, RoutedEventArgs e)
            => Runtime.ShowWelcomePage();

        private void SetSereinDownloadPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = true;
            dialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            dialog.Description = "请选择Serein下载路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SereinDownloadPath.Text = dialog.SelectedPath;
                Global.Settings.Serein.SereinDownloadPath = dialog.SelectedPath;
            }
            
        }
    }
}
