﻿using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Serein
{
    public partial class Ui : Form
    {
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
            new Task(() =>
            {
                {
                    Server.Start();
                }
            }).Start();
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
            PanelConsoleInput.Clear();
            PanelConsoleInput.Focus();
        }
        private void PanelConsoleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                Server.InputCommand(PanelConsoleInput.Text);
                PanelConsoleInput.Clear();
            }
        }

        private void PanelTableLayout_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (!PanelConsoleInput.Focused)
            {
                PanelConsoleInput.Focus();
            }
        }

        private void PanelConsoleInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(13))
            {
                e.Handled = true;
            }
        }
    }
}