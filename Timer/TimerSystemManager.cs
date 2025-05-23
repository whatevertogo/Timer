using System;
using System.Collections.Generic;
using CDTU.Utils;
using UnityEngine;

namespace Timer
{
    /// <summary>
    /// 计时器管理器
    /// </summary>
    public partial class TimerSystemManager : SingletonDD<TimerSystemManager>, ITimerSystem
    {
        private TimerObjectPool<TimerEntity> _timerPool;
        private List<TimerEntity> _activeTimers = new List<TimerEntity>();
        private Dictionary<TimerCallback, TimerEntity> _callbackToTimer = new Dictionary<TimerCallback, TimerEntity>();

        public TimeSpan DeltaTime { get; private set; }
        public TimeSpan UnscaledDeltaTime { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }
        public TimeSpan UnscaledElapsedTime { get; private set; }

        private float _timeScale = 1.0f;
        public float TimeScale
        {
            get => _timeScale;
            set => _timeScale = Mathf.Max(0, value);
        }

        protected override void Awake()
        {
            base.Awake();
            _timerPool = new TimerObjectPool<TimerEntity>(20, timer => timer.Reset());
            Initialize();
        }

        public void Initialize()
        {
            Shutdown();
            ElapsedTime = TimeSpan.Zero;
            UnscaledElapsedTime = TimeSpan.Zero;
        }

        public void Shutdown()
        {
            foreach (var timer in _activeTimers)
            {
                _timerPool.Release(timer);
            }
            _activeTimers.Clear();
            _callbackToTimer.Clear();
        }

        private void Update()
        {
            UnscaledDeltaTime = TimeSpan.FromSeconds(Time.unscaledDeltaTime);
            DeltaTime = TimeSpan.FromSeconds(Time.unscaledDeltaTime * TimeScale);
            
            UnscaledElapsedTime += UnscaledDeltaTime;
            ElapsedTime += DeltaTime;
            
            UpdateTimers();
        }

        private void UpdateTimers()
        {
            for (int i = _activeTimers.Count - 1; i >= 0; i--)
            {
                var timer = _activeTimers[i];
                
                if (!timer.IsRunning)
                {
                    _activeTimers.RemoveAt(i);
                    _callbackToTimer.Remove(timer.GetCallback());
                    _timerPool.Release(timer);
                    continue;
                }

                timer.Update(timer.UseUnscaledTime ? UnscaledDeltaTime : DeltaTime);
            }
        }

        public ITimerEntity CreateTimer(TimeSpan interval, int repeat, TimerCallback callback, object userData = null, bool useUnscaledTime = false)
        {
            if (_callbackToTimer.TryGetValue(callback, out var existingTimer))
            {
                existingTimer.Pause();
                _timerPool.Release(existingTimer);
                _activeTimers.Remove(existingTimer);
            }

            var timer = _timerPool.Get();
            timer.Initialize(interval, repeat, callback, userData, useUnscaledTime);
            _activeTimers.Add(timer);
            _callbackToTimer[callback] = timer;
            return timer;
        }

        public void RemoveTimer(ITimerEntity timer)
        {
            if (timer is TimerEntity entity)
            {
                entity.Pause();
            }
        }

        public ITimerEntity FindTimer(TimerCallback callback)
        {
            return _callbackToTimer.TryGetValue(callback, out var timer) && timer.IsRunning ? timer : null;
        }

        private void OnDestroy()
        {
            Shutdown();
        }
    }
}