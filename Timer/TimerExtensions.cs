using System;
using UnityEngine;

namespace Timer
{

    /// <summary>
    /// 计时器扩展方法
    /// </summary>
    public static class TimerExtensions
    {

        private const float FrameInterval = 0.016f;
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
            return timersystem.CreateTimer(TimeSpan.Zero, 1, callback, userData, false);
        }

        /// <summary>
        /// 添加延迟执行计时器(指定帧数)
        /// </summary>
        public static ITimerEntity AddFrames(this ITimerSystem timersystem, int frames, TimerCallback callback, object userData = null, bool useUnscaledTime = false)
        {
            float seconds = frames / 60f;
            return timersystem.CreateTimer(TimeSpan.FromSeconds(seconds), 1, callback, userData, useUnscaledTime);
        }

        /// <summary>
        /// 添加倒计时计时器
        /// </summary>
        public static ITimerEntity AddCountdown(this ITimerSystem timersystem, float totalSeconds, Action<float> onTick, Action onComplete, bool useUnscaledTime = false)
        {
            float remainingTime = totalSeconds;

            // 先声明委托变量
            TimerCallback countdownCallback = null;
            ITimerEntity timerEntity = null;

            // 然后给委托变量赋值
            countdownCallback = (userData) =>
            {
                remainingTime -= useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                if (remainingTime < 0)
                    remainingTime = 0;

                onTick?.Invoke(remainingTime);

                if (remainingTime <= 0)
                {
                    onComplete?.Invoke();
                    if (timerEntity != null)
                    {
                        timersystem.RemoveTimer(timerEntity);
                    }
                }
            };

            // 每帧更新
            timerEntity = timersystem.CreateTimer(TimeSpan.FromSeconds(FrameInterval), -1, countdownCallback, null, useUnscaledTime);
            return timerEntity;
        }
    }
}