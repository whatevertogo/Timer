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

        public ITimerEntity CreateTimer(TimeSpan interval,//间隔事件
         int repeat,   //重复次数 
         TimerCallback callback,//回调函数
          object userData = null,//传递参数
           bool useUnscaledTime = false//是否使用不受时间缩放影响的时间
           )
        {
            // 参数校验
            if (callback == null)
            {
                Debug.LogError("[TimerSystem] Callback cannot be null");
                return null;
            }
            if (interval.TotalSeconds < 0)
            {
                Debug.LogError($"[TimerSystem] Interval cannot be negative: {interval.TotalSeconds}");
                return null;
            }
            if (repeat == 0 || repeat < -1)
            {
                Debug.LogError($"[TimerSystem] Repeat must be -1 (infinite) or > 0, got: {repeat}");
                return null;
            }

            var timer = _timerPool.Get();
            timer.Initialize(interval, repeat, callback, userData, useUnscaledTime);
            _activeTimers.Add(timer);
            
            // 注意：不再强制移除旧的同名回调，允许多个计时器使用相同回调
            // 但 FindTimer 会返回最后一个创建的
            _callbackToTimer[callback] = timer;
            
            return timer;
        }

        public void RemoveTimer(ITimerEntity timer)
        {
            if (timer == null) return;
            
            if (timer is TimerEntity entity)
            {
                entity.Pause();
                _activeTimers.Remove(entity);
                
                // 从字典中移除（只移除当前计时器的引用）
                var callback = entity.GetCallback();
                if (_callbackToTimer.TryGetValue(callback, out var cachedTimer) && cachedTimer == entity)
                {
                    _callbackToTimer.Remove(callback);
                }
                
                _timerPool.Release(entity);
            }
        }

        public ITimerEntity FindTimer(TimerCallback callback)
        {
            return _callbackToTimer.TryGetValue(callback, out var timer) && timer.IsRunning ? timer : null;
        }

        /// <summary>
        /// 获取当前活跃计时器数量
        /// </summary>
        public int ActiveTimerCount => _activeTimers.Count;

        /// <summary>
        /// 清理所有计时器
        /// </summary>
        public void ClearAllTimers()
        {
            Shutdown();
            Initialize();
        }

        /// <summary>
        /// 暂停指定回调的计时器
        /// </summary>
        public void PauseTimer(TimerCallback callback)
        {
            var timer = FindTimer(callback);
            timer?.Pause();
        }

        /// <summary>
        /// 恢复指定回调的计时器
        /// </summary>
        public void ResumeTimer(TimerCallback callback)
        {
            if (_callbackToTimer.TryGetValue(callback, out var timer))
            {
                timer.Resume();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 调试：输出所有活跃计时器
        /// </summary>
        public void LogActiveTimers()
        {
            Debug.Log($"[TimerSystem] Active Timers: {_activeTimers.Count}");
            foreach (var timer in _activeTimers)
            {
                var callback = timer.GetCallback();
                var methodName = callback?.Method.Name ?? "<null>";
                Debug.Log($"  - {methodName}, Remaining: {timer.RemainingRepeatCount}, Interval: {timer.Interval.TotalSeconds:F2}s");
            }
        }
#endif

        private void OnDestroy()
        {
            Shutdown();
        }
    }
}