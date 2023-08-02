using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Forms;
using System.Diagnostics;
using Esprima.Ast;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections;

namespace Serein.Windows.Pages.Server
{
    /// <summary>
    /// Interaction logic for Usage.xaml
    /// </summary>
    public partial class Usage : UiPage
    {
        public int i = 0;
        public List<double> CPUMonitor = new List<double> { };
        public ISeries[] LineSeriesCollection_CPU { get; set; }
        public Usage()
        {
            InitializeComponent();
            CPUMonitor.Add(1);
            LineSeriesCollection_CPU = new ISeries[]
                {
                new LineSeries<double>
                    {
                        Values = CPUMonitor,
                        Fill = null,
                        Stroke = null
                }


            };
            
            
       }
    }




}
    


