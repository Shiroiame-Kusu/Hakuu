using Serein.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
using Wpf.Ui.Controls;

namespace Serein.Windows.Pages.Server
{
    /// <summary>
    /// Interaction logic for Properties.xaml
    /// </summary>
    public partial class Properties : UiPage
    {
        public static string PropertiesPath = Global.Settings.Server.Path + "\\server.properties";
        PropertyOperation PropertiesOperation = new PropertyOperation(PropertiesPath);
        public Properties()
        {
            InitializeComponent();
            
            
            LoadProperties();

        }

        private void LoadProperties()
        {
            
            
        }
        private void SaveProperties()
        {
            PropertiesOperation.Save();
        }
    }
}
