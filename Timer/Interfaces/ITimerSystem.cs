using System;

namespace Timer
{
    /// <summary>
    /// 计时器系统接口
    /// </summary>
    public interface ITimerSystem
    {
        /// <summary>
        /// 初始化计时器系统
        /// </summary>
        void Initialize();

        /// <summary>
        /// 关闭计时器系统
        /// </summary>
        void Shutdown();

        // /// <summary>
        // /// 当前帧时间增量
        // /// </summary>
        // TimeSpan DeltaTime { get; }

        // /// <summary>
        // /// 当前帧时间增量(不受TimeScale影响)
        // /// </summary>
        // TimeSpan UnscaledDeltaTime { get; }

        // /// <summary>
        // /// 计时器累计运行时间
        // /// </summary>
        // TimeSpan ElapsedTime { get; }

        // /// <summary>
        // /// 计时器累计运行时间(不受TimeScale影响)
        // /// </summary>
        // TimeSpan UnscaledElapsedTime { get; }

        /// <summary>
        /// 时间缩放系数
        /// </summary>
        float TimeScale { get; set; }

        /// <summary>
        /// 创建一个新的计时器
        /// </summary>
        ITimerEntity CreateTimer(TimeSpan interval, int repeat, TimerCallback callback, object userData = null, bool useUnscaledTime = false);

        /// <summary>
        /// 移除一个计时器
        /// </summary>
        void RemoveTimer(ITimerEntity timer);

        /// <summary>
        /// 根据回调查找计时器
        /// </summary>
        ITimerEntity FindTimer(TimerCallback callback);
    }
}
