using Hakuu.Core.Generic;
using Hakuu.Core.Server;
using Hakuu.Extensions;
using System;
using System.Windows.Forms;

namespace Hakuu.Ui
{
    public partial class Ui : Form
    {
        /// <summary>
        /// 更新计时器
        /// </summary>
        private readonly System.Timers.Timer UpdateInfoTimer = new(5000) { AutoReset = true };

        private void UpdateInfo()
        {
            if (!Visible)
            {
                return;
            }
            else if (ServerManager.Status)
            {
                Text = $"Hakuu | {(string.IsNullOrEmpty(ServerManager.StartFileName) ? "unknown" : ServerManager.StartFileName)}";
                ServerPanelInfoTime2.Text = ServerManager.Time;
                ServerPanelInfoCPU2.Text = $"{ServerManager.CPUUsage:N1}%";
                ServerPanelInfoStatus2.Text = "已启动";
                ServerPanelInfoVersion2.Text = ServerManager.Motd != null && !string.IsNullOrEmpty(ServerManager.Motd.Version) ? ServerManager.Motd.Version : "-";
                ServerPanelInfoPlayerCount2.Text = ServerManager.Motd != null ? $"{ServerManager.Motd.OnlinePlayer}/{ServerManager.Motd.MaxPlayer}" : "-";
                ServerPanelInfoDifficulty2.Text = ServerManager.Status && !string.IsNullOrEmpty(ServerManager.Difficulty) ? ServerManager.Difficulty : "-";
            }
            else
            {
                Text = "Hakuu";
                ServerPanelInfoStatus2.Text = "未启动";
                ServerPanelInfoVersion2.Text = "-";
                ServerPanelInfoDifficulty2.Text = "-";
                ServerPanelInfoPlayerCount2.Text = "-";
                ServerPanelInfoTime2.Text = "-";
                ServerPanelInfoCPU2.Text = "-";
            }
            if (Websocket.Status)
            {
                BotInfoStatus2.Text = "已连接";
                BotInfoTime2.Text = (DateTime.Now - Websocket.StartTime).ToCustomString();
            }
            else
            {
                BotInfoStatus2.Text = "未连接";
                BotInfoTime2.Text = "-";
            }

            BotInfoQQ2.Text = PacketHandler.SelfId;
            BotInfoMessageReceived2.Text = PacketHandler.MessageReceived;
            BotInfoMessageSent2.Text = PacketHandler.MessageSent;
        }

        public void SettingHakuuVersion_Update(string NewText)
        {
            if (SettingHakuuVersion.InvokeRequired)
            {
                Action<string> actionDelegate = (Text) => { SettingHakuuVersion.Text = Text; };
                SettingHakuuVersion.Invoke(actionDelegate, NewText);
            }
        }
    }
}
