using Newtonsoft.Json.Linq;
using Hakuu.Base.Packets;
using Hakuu.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hakuu.Core.Generic
{
    internal static class Matcher
    {


        /// <summary>
        /// 处理来自控制台的消息
        /// </summary>
        /// <param name="line">控制台的消息</param>
        public static void Process(string line)
        {
            lock (Global.RegexList)
            {
                foreach (Base.Regex regex in Global.RegexList)
                {
                    if (string.IsNullOrEmpty(regex.Expression) || regex.Area != 1 || !System.Text.RegularExpressions.Regex.IsMatch(line, regex.Expression))
                    {
                        continue;
                    }
                    Task.Run(() => Command.Run(Base.CommandOrigin.Console, regex.Command, System.Text.RegularExpressions.Regex.Match(line, regex.Expression)));
                }
            }
        }

        /// <summary>
        /// 匹配消息
        /// </summary>
        /// <param name="messagePacket">数据包</param>
        /// <param name="isSelfMessage">是否为自身消息</param>

        /// <summary>
        /// 判断是否为管理
        /// </summary>
        /// <param name="messagePacket">数据包</param>
        /// <returns>是否为管理</returns>
        /// <summary>
        /// 更新群组缓存
        /// </summary>
        /// <param name="messagePacket">数据包</param>
        private static void UpdateGroupCache(Message messagePacket)
        {
            lock (Global.GroupCache)
            {
                if (!Global.GroupCache.ContainsKey(messagePacket.GroupId))
                {
                    Global.GroupCache.Add(messagePacket.GroupId, new Dictionary<long, Base.Member>());
                }
                if (!Global.GroupCache[messagePacket.GroupId].ContainsKey(messagePacket.UserId))
                {
                    Global.GroupCache[messagePacket.GroupId].Add(messagePacket.UserId, new Base.Member());
                }
                Base.Member member = Global.GroupCache[messagePacket.GroupId][messagePacket.UserId];
                member.ID = messagePacket.UserId;
                member.Nickname = messagePacket.Sender!.Nickname ?? string.Empty;
                member.Card = messagePacket.Sender.Card ?? string.Empty;
                member.Role = messagePacket.Sender.RoleIndex;
                member.GameID = Binder.GetGameID(messagePacket.UserId);

                Global.GroupCache[messagePacket.GroupId][messagePacket.UserId] = member;
            }
        }
    }
}
