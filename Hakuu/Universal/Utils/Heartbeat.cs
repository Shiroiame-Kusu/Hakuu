using Hakuu.Base;
using Hakuu.Core.Server;
using Hakuu.Extensions;
using System;
using System.Timers;

namespace Hakuu.Utils
{
    internal static class Heartbeat
    {
        /// <summary>
        /// GUID
        /// </summary>
        private readonly static string _guid = Guid.NewGuid().ToString("N");

        /// <summary>
        /// 启动时刻时间戳
        /// </summary>
        private readonly static long _startTimeStamp = (long)(TimeZoneInfo.ConvertTimeToUtc(Global.StartTime) - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;

        /// <summary>
        /// 在线统计计时器
        /// </summary>
        private static readonly Timer _heartbeatTimer = new(600_000) { AutoReset = true }; // 十分钟一次心跳事件

        /// <summary>
        /// 开始心跳事件
        /// </summary>
        public static void Start()
        {
            _heartbeatTimer.Elapsed += (_, _) => Request();
            _heartbeatTimer.Start();
        }

        /// <summary>
        /// 心跳请求
        /// </summary>
        private static void Request()
        {
                if (Global.Settings.Hakuu.Function.NoHeartbeat)
            {
                return;
            }
            try
            {
                Logger.Output(LogType.Debug,
                    Net.Get($"https://api.online-count.Hakuu.cc/heartbeat?guid={_guid}&type={Global.TYPE}&version={Global.VERSION}&start_time={_startTimeStamp}&server_status={ServerManager.Status}")
                        .Await().Content.ReadAsStringAsync().Await());
            }
            catch (Exception e)
            {
                Logger.Output(LogType.Debug, e);
            }
        }
    }
}