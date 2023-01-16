﻿using Newtonsoft.Json.Linq;
using Serein.Extensions;
using Serein.JSPlugin;

namespace Serein.Base
{
    public static class Matcher
    {
        /// <summary>
        /// 统计信息
        /// </summary>
        public static string MessageReceived, MessageSent, SelfId;

        /// <summary>
        /// 处理来自控制台的消息
        /// </summary>
        /// <param name="line">控制台的消息</param>
        public static void Process(string line)
        {
            foreach (Items.Regex regex in Global.RegexItems)
            {
                if (string.IsNullOrEmpty(regex.Expression) || regex.Area != 1)
                {
                    continue;
                }
                if (System.Text.RegularExpressions.Regex.IsMatch(line, regex.Expression))
                {
                    Command.Run(2, regex.Command, msgMatch: System.Text.RegularExpressions.Regex.Match(line, regex.Expression));
                }
            }
        }

        /// <summary>
        /// 处理来自机器人的消息
        /// </summary>
        /// <param name="packet">数据包</param>
        public static void Process(JObject packet)
        {
            if (string.IsNullOrEmpty(packet.TryGetString("post_type")))
            {
                return;
            }
            string postType = packet.TryGetString("post_type").ToString();
            long result, userId, groupId;
            switch (postType)
            {
                case "message":
                case "message_sent":
                    bool isSelfMessage = postType == "message_sent";
                    string messageType = packet.TryGetString("$1").ToString();
                    string rawMessage = packet.TryGetString("raw_message").ToString();
                    userId = long.TryParse(packet.TryGetString("sender", "user_id").ToString(), out result) ? result : -1;
                    groupId = messageType == "group" && long.TryParse(packet.TryGetString("group_id").ToString(), out result) ? result : -1;
                    Logger.Out(Items.LogType.Bot_Receive, $"{packet.TryGetString("sender", "nickname")}({packet.TryGetString("sender", "user_id")})" + ":" + rawMessage);
                    foreach (Items.Regex regex in Global.RegexItems)
                    {
                        if (
                            string.IsNullOrEmpty(regex.Expression) ||
                            regex.Area <= 1 ||
                            !(
                                isSelfMessage && regex.Area == 4 ||
                                !isSelfMessage && regex.Area != 4
                            ) ||
                            messageType == "group" && !Global.Settings.Bot.GroupList.Contains(groupId) ||
                            !System.Text.RegularExpressions.Regex.IsMatch(rawMessage, regex.Expression)
                            )
                        {
                            continue;
                        }
                        if (
                            !(
                            Global.Settings.Bot.PermissionList.Contains(userId) ||
                            Global.Settings.Bot.GivePermissionToAllAdmin &&
                            messageType == "group" && (
                                packet.TryGetString("sender", "role") == "admin" ||
                                packet.TryGetString("sender", "role") == "owner")
                            ) &&
                            regex.IsAdmin &&
                            !isSelfMessage
                            )
                        {
                            switch (regex.Area)
                            {
                                case 2:
                                    EventTrigger.Trigger(Items.EventType.PermissionDeniedFromGroupMsg, groupId, userId);
                                    break;
                                case 3:
                                    EventTrigger.Trigger(Items.EventType.PermissionDeniedFromPrivateMsg, -1, userId);
                                    break;
                            }
                            continue;
                        }
                        if (System.Text.RegularExpressions.Regex.IsMatch(rawMessage, regex.Expression))
                        {
                            if ((regex.Area == 4 || regex.Area == 2) && messageType == "group")
                            {
                                Command.Run(
                                    1,
                                    regex.Command,
                                    packet,
                                    System.Text.RegularExpressions.Regex.Match(
                                        rawMessage,
                                        regex.Expression
                                    ),
                                    userId,
                                    groupId
                                );
                            }
                            else if ((regex.Area == 4 || regex.Area == 3) && messageType == "private")
                            {
                                Command.Run(
                                    1,
                                    regex.Command,
                                    packet,
                                    System.Text.RegularExpressions.Regex.Match(
                                        rawMessage,
                                        regex.Expression
                                        ),
                                    userId
                                );
                            }
                        }
                    }
                    if (!isSelfMessage)
                    {
                        if (messageType == "private")
                        {
                            JSFunc.Trigger(Items.EventType.ReceivePrivateMessage, userId, rawMessage, packet.TryGetString("sender", "nickname").ToString());
                        }
                        else if (messageType == "group")
                        {
                            JSFunc.Trigger(Items.EventType.ReceiveGroupMessage, groupId, userId, rawMessage,
                                string.IsNullOrEmpty(packet.TryGetString("sender", "card").ToString()) ? packet.TryGetString("sender", "nickname").ToString() : packet.TryGetString("sender", "card").ToString());
                        }
                    }
                    break;
                case "meta_event":
                    if (packet.TryGetString("meta_event_type").ToString() == "heartbeat")
                    {
                        SelfId = packet.TryGetString("self_id").ToString();
                        MessageReceived = (
                            string.IsNullOrEmpty(packet.TryGetString("status", "stat", "message_received")) ?
                            packet.TryGetString("status", "stat", "MessageReceived") : packet.TryGetString("status", "stat", "message_received"));
                        MessageReceived = (
                            string.IsNullOrEmpty(packet.TryGetString("status", "stat", "message_sent")) ?
                            packet.TryGetString("status", "stat", "MessageSent") : packet.TryGetString("status", "stat", "message_sent"));
                        if ((long.TryParse(MessageReceived, out long TempNumber) ? TempNumber : 0) > 10000000)
                        {
                            MessageReceived = (TempNumber / 10000).ToString("N1") + "w";
                        }
                        if ((long.TryParse(MessageSent, out TempNumber) ? TempNumber : 0) > 10000000)
                        {
                            MessageSent = (TempNumber / 10000).ToString("N1") + "w";
                        }
                    }
                    break;
                case "notice":
                    userId = long.TryParse(packet.TryGetString("user_id").ToString(), out result) ? result : -1;
                    groupId = long.TryParse(packet.TryGetString("group_id").ToString(), out result) ? result : -1;
                    if (Global.Settings.Bot.GroupList.Contains(groupId))
                    {
                        switch (packet.TryGetString("notice_type").ToString())
                        {
                            case "GroupDecrease":
                            case "group_decrease":
                                EventTrigger.Trigger(Items.EventType.GroupDecrease, groupId, userId);
                                JSFunc.Trigger(Items.EventType.GroupDecrease, groupId, userId);
                                break;
                            case "GroupIncrease":
                            case "group_increase":
                                EventTrigger.Trigger(Items.EventType.GroupIncrease, groupId, userId);
                                JSFunc.Trigger(Items.EventType.GroupIncrease, groupId, userId);
                                break;
                            case "notify":
                                if (packet.TryGetString("sub_type").ToString() == "poke" &&
                                    packet.TryGetString("target_id").ToString() == SelfId)
                                {
                                    EventTrigger.Trigger(Items.EventType.GroupPoke, groupId, userId);
                                    JSFunc.Trigger(Items.EventType.GroupPoke, groupId, userId);
                                }
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
