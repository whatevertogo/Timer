using UnityEngine;
using Timer;

public class TimerExample : MonoBehaviour
{
    // 存储需要手动管理的计时器引用
    private ITimerEntity _infiniteTimer;
    private ITimerEntity _countdownTimer;

    private void Start()
    {
        // 添加一次性计时器：2秒后执行
        TimerSystemManager.Instance.AddOnce(2f, OnTimerComplete, "一次性计时器数据");

        // 添加重复计时器：每1秒执行一次，共5次
        TimerSystemManager.Instance.AddRepeat(1f, 5, OnRepeatTimer, "有限重复计时器数据");

        // 添加无限重复计时器：每0.5秒执行一次
        _infiniteTimer = TimerSystemManager.Instance.AddRepeat(0.5f, OnInfiniteTimer);

        // 添加帧计时器：10帧后执行
        TimerSystemManager.Instance.AddFrames(10, OnFrameTimer);

        // 添加倒计时：5秒倒计时
        _countdownTimer = TimerSystemManager.Instance.AddCountdown(5f, OnCountdownTick, OnCountdownComplete);

        // 不受时间缩放影响的计时器
        TimerSystemManager.Instance.AddOnce(3f, OnUnscaledTimer, "不受缩放影响的计时器", true);

        // 演示计时器控制
        // 3秒后暂停无限计时器
        TimerSystemManager.Instance.AddOnce(3f, _ => 
        {
            Debug.Log("暂停无限计时器");
            _infiniteTimer?.Pause();

            // 2秒后恢复
            TimerSystemManager.Instance.AddOnce(2f, __ => 
            {
                Debug.Log("恢复无限计时器");
                _infiniteTimer?.Resume();
            });
        });
    }

    private void OnTimerComplete(object userData)
    {
        Debug.Log($"一次性计时器完成：{userData}");
    }

    private void OnRepeatTimer(object userData)
    {
        Debug.Log($"重复计时器触发：{userData}");
    }

    private void OnInfiniteTimer(object userData)
    {
        Debug.Log("无限重复计时器触发");
    }

    private void OnFrameTimer(object userData)
    {
        Debug.Log("帧计时器触发");
    }

    private void OnCountdownTick(float remainingTime)
    {
        Debug.Log($"倒计时：{remainingTime:F1}秒");
    }

    private void OnCountdownComplete()
    {
        Debug.Log("倒计时完成！");
    }

    private void OnUnscaledTimer(object userData)
    {
        Debug.Log($"不受缩放影响的计时器触发：{userData}");
    }

    private void OnDestroy()
    {
        // 清理特定的计时器
        if (_infiniteTimer != null)
        {
            TimerSystemManager.Instance.RemoveTimer(_infiniteTimer);
            _infiniteTimer = null;
        }

        if (_countdownTimer != null)
        {
            TimerSystemManager.Instance.RemoveTimer(_countdownTimer);
            _countdownTimer = null;
        }

        // 清理所有计时器
        TimerSystemManager.Instance.Shutdown();
    }
}