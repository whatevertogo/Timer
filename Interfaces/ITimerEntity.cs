using System;

namespace Timer
{
    /// <summary>
    /// 单个计时器实体的接口
    /// </summary>
    public interface ITimerEntity
    {
        /// <summary>
        /// 是否正在运行
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// 是否使用非缩放时间
        /// </summary>
        bool UseUnscaledTime { get; }

        /// <summary>
        /// 间隔时间
        /// </summary>
        TimeSpan Interval { get; }

        /// <summary>
        /// 剩余重复次数，-1表示无限重复
        /// </summary>
        int RemainingRepeatCount { get; }

        /// <summary>
        /// 暂停计时器
        /// </summary>
        void Pause();

        /// <summary>
        /// 恢复计时器
        /// </summary>
        void Resume();

        /// <summary>
        /// 重置计时器
        /// </summary>
        void Reset();
    }
}
