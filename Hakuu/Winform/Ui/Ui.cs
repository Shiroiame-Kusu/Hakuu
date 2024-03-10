using System.Windows.Forms;

namespace Hakuu.Ui
{
    public partial class Ui : Form
    {
        public Ui()
        {
            InitializeComponent();
            Initialize();
            UpdateVersion();
        }
    }
}