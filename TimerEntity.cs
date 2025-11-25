using System;
using UnityEngine;

namespace Timer
{
    /// <summary>
    /// 计时器实体实现
    /// </summary>
    public class TimerEntity : ITimerEntity
    {
        public bool IsRunning { get; private set; }
        public bool UseUnscaledTime { get; private set; }
        public TimeSpan Interval { get; private set; }
        public int RemainingRepeatCount { get; private set; }

        private TimeSpan _elapsed;
        private TimerCallback _callback;
        private object _userData;

        public TimerEntity()
        {
            Reset();
        }

        public void Initialize(TimeSpan interval, int repeat, TimerCallback callback, object userData, bool useUnscaledTime)
        {
            Interval = interval;
            RemainingRepeatCount = repeat;
            _callback = callback;
            _userData = userData;
            UseUnscaledTime = useUnscaledTime;
            _elapsed = TimeSpan.Zero;
            IsRunning = true;
        }

        public void Update(TimeSpan deltaTime)
        {
            if (!IsRunning) return;

            _elapsed += deltaTime;

            if (_elapsed < Interval) return;

            _elapsed = TimeSpan.Zero;

            _callback?.Invoke(_userData);

            if (RemainingRepeatCount > 0)
            {
                RemainingRepeatCount--;
                if (RemainingRepeatCount == 0)
                {
                    IsRunning = false;
                }
            }
        }

        public void Pause()
        {
            IsRunning = false;
        }

        public void Resume()
        {
            if (RemainingRepeatCount != 0 || RemainingRepeatCount == -1)
            {
                IsRunning = true;
            }
        }

        public void Reset()
        {
            Interval = TimeSpan.Zero;
            RemainingRepeatCount = 0;
            _callback = null;
            _userData = null;
            UseUnscaledTime = false;
            _elapsed = TimeSpan.Zero;
            IsRunning = false;
        }

        // 内部辅助方法，供TimerSystemManager使用
        internal TimerCallback GetCallback() => _callback;
    }
}
