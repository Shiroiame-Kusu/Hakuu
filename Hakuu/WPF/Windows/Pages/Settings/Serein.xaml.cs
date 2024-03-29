using Hakuu.Utils;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Hakuu.Windows.Pages.Settings
{
    public partial class Hakuu : UiPage
    {
        private readonly bool _loaded;

        public Hakuu()
        {
            InitializeComponent();
            Load();
            _loaded = true;
            Catalog.Settings.Hakuu = this;
            
        }

        private void Load()
        {
            UseDarkTheme.IsChecked = Global.Settings.Hakuu.UseDarkTheme;
            EnableGetUpdate.IsChecked = Global.Settings.Hakuu.EnableGetUpdate;
            AutoUpdate.IsChecked = Global.Settings.Hakuu.AutoUpdate && Global.Settings.Hakuu.EnableGetUpdate;
            AutoUpdate.IsEnabled = EnableGetUpdate.IsChecked ?? false;
            ThemeFollowSystem.IsChecked = Global.Settings.Hakuu.ThemeFollowSystem;
            MaxCacheLines.Value = Global.Settings.Hakuu.MaxCacheLines;
            Version.Text = "当前版本：" + Global.VERSION;
            BuildInfo.Text = Global.BuildInfo.ToString();
            if (string.IsNullOrEmpty(Global.Settings.Hakuu.HakuuDownloadPath) || Global.Settings.Hakuu.HakuuDownloadPath == AppDomain.CurrentDomain.BaseDirectory + "Hakuu-Server")
            {
                HakuuDownloadPath.Text = "\\\\Hakuu-Server";
            }
            else
            {
                HakuuDownloadPath.Text = Global.Settings.Hakuu.HakuuDownloadPath;
            }
            CustomizeTitle.IsChecked = Global.Settings.Server.isCustomizedTitleEnabled;
            switch (CustomizeTitle.IsChecked)
            {
                case true:
                    CustomizedTitle.Visibility = Visibility.Visible;
                    break;
                case false:
                    CustomizedTitle.Visibility = Visibility.Collapsed;
                    break;
            }
            
            //CustomizedTitle.IsEnabled = Global.Settings.Server.isCustomizedTitleEnabled;
            
        }

        private void EnableGetUpdate_Click(object sender, RoutedEventArgs e)
        {
            AutoUpdate.IsEnabled = EnableGetUpdate.IsChecked ?? false;
            AutoUpdate.IsChecked = (AutoUpdate.IsChecked ?? false) ? EnableGetUpdate.IsChecked ?? false : false;
            Global.Settings.Hakuu.EnableGetUpdate = EnableGetUpdate.IsChecked ?? false;
            Global.Settings.Hakuu.AutoUpdate = AutoUpdate.IsChecked ?? false;
        }

        private void AutoUpdate_Click(object sender, RoutedEventArgs e)
            => Global.Settings.Hakuu.AutoUpdate = AutoUpdate.IsChecked ?? false;

        private void ThemeFollowSystem_Click(object sender, RoutedEventArgs e)
        {
            UseDarkTheme.IsChecked = (UseDarkTheme.IsChecked ?? false) && (ThemeFollowSystem.IsChecked ?? false) ? false : UseDarkTheme.IsChecked;
            Global.Settings.Hakuu.UseDarkTheme = UseDarkTheme.IsChecked ?? false;
            Global.Settings.Hakuu.ThemeFollowSystem = ThemeFollowSystem.IsChecked ?? false;
            Theme.Apply(Global.Settings.Hakuu.UseDarkTheme ? ThemeType.Dark : ThemeType.Light);
        }

        private void UseDarkTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeFollowSystem.IsChecked = (ThemeFollowSystem.IsChecked ?? false) && (UseDarkTheme.IsChecked ?? false) ? false : ThemeFollowSystem.IsChecked;
            Global.Settings.Hakuu.UseDarkTheme = UseDarkTheme.IsChecked ?? false;
            Global.Settings.Hakuu.ThemeFollowSystem = ThemeFollowSystem.IsChecked ?? false;
            Theme.Apply(Global.Settings.Hakuu.UseDarkTheme ? ThemeType.Dark : ThemeType.Light);
        }

        public void UpdateVersion(string text)
            => Dispatcher.Invoke(() => Version.Text = "当前版本：" + Global.VERSION + text);

        private void MaxCacheLines_TextChanged(object sender, TextChangedEventArgs e)
            => Global.Settings.Hakuu.MaxCacheLines = _loaded ? (int)MaxCacheLines.Value : Global.Settings.Hakuu.MaxCacheLines;

        private void WelPage_Click(object sender, RoutedEventArgs e)
            => Runtime.ShowWelcomePage();

        private void SetHakuuDownloadPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = true;
            dialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            dialog.Description = "请选择Hakuu下载路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                HakuuDownloadPath.Text = dialog.SelectedPath;
                Global.Settings.Hakuu.HakuuDownloadPath = dialog.SelectedPath;
            }
            
        }
        private void CustomizeTitle_Click(object sender, RoutedEventArgs e)
        {
            Global.Settings.Server.isCustomizedTitleEnabled = (bool)CustomizeTitle.IsChecked;
            switch (CustomizeTitle.IsChecked)
            {
                case true:
                    CustomizedTitle.Visibility = Visibility.Visible;
                    break;
                case false:
                    CustomizedTitle.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void CustomizedTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            Global.Settings.Server.CustomizedTitle = CustomizedTitle.Text;
            Catalog.MainWindow?.UpdateTitle(Global.Settings.Server.CustomizedTitle);
        }
    }
}
