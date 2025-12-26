using System;
using UnityEngine;

namespace Timer
{

    /// <summary>
    /// 计时器扩展方法
    /// </summary>
    public static class TimerExtensions
    {

        /// <summary>
        /// 添加一次性计时器
        /// </summary>
        public static ITimerEntity AddOnce(this ITimerSystem timersystem, float seconds, TimerCallback callback, object userData = null, bool useUnscaledTime = false)
        {
            return timersystem.CreateTimer(TimeSpan.FromSeconds(seconds), 1, callback, userData, useUnscaledTime);
        }

        /// <summary>
        /// 添加无限重复计时器
        /// </summary>
        public static ITimerEntity AddRepeat(this ITimerSystem timersystem, float seconds, TimerCallback callback, object userData = null, bool useUnscaledTime = false)
        {
            return timersystem.CreateTimer(TimeSpan.FromSeconds(seconds), -1, callback, userData, useUnscaledTime);
        }

        /// <summary>
        /// 添加有限次数重复计时器
        /// </summary>
        public static ITimerEntity AddRepeat(this ITimerSystem timersystem, float seconds, int repeatCount, TimerCallback callback, object userData = null, bool useUnscaledTime = false)
        {
            return timersystem.CreateTimer(TimeSpan.FromSeconds(seconds), repeatCount, callback, userData, useUnscaledTime);
        }

        /// <summary>
        /// 添加延迟执行计时器(下一帧)
        /// </summary>
        public static ITimerEntity AddNextFrame(this ITimerSystem timersystem, TimerCallback callback, object userData = null)
        {
            return timersystem.CreateTimer(TimeSpan.FromTicks(1), 1, callback, userData, false);
        }
    }
}
