# Timer 计时器系统

一个高效、灵活的计时器系统实现，专为 Unity 项目设计。该系统提供了简单易用的 API，用于管理和控制各种计时任务。

## 特性

- 🚀 **高性能对象池实现**，减少 GC 压力
- 💡 **支持多种计时器类型**：一次性、有限重复、无限重复、帧计时器、倒计时
- 🔄 **灵活的回调机制**，支持传递用户数据
- ⏱ **时间控制**：支持 TimeScale 时间缩放和不受缩放影响的 UnscaledTime
- 🎮 **计时器控制**：支持暂停、恢复、重置、手动移除
- 🛠 **简单易用的 API 接口**
- 🎯 **完整的接口抽象**，易于扩展和测试
- 📊 **调试友好**：编辑器下支持查看活跃计时器信息

## 安装

将 `Timer` 文件夹复制到你的 Unity 项目中即可使用。

## 快速开始

### 基础用法

```csharp
using Timer;

// 一次性计时器：3秒后执行
TimerSystemManager.Instance.AddOnce(3f, (data) => {
    Debug.Log("3秒后执行！");
});

// 重复计时器：每1秒执行一次，共5次
TimerSystemManager.Instance.AddRepeat(1f, 5, (data) => {
    Debug.Log("重复执行！");
});

// 无限重复计时器：每0.5秒执行一次
var infiniteTimer = TimerSystemManager.Instance.AddRepeat(0.5f, (data) => {
    Debug.Log("无限重复！");
});
```

### 高级用法

#### 倒计时

```csharp
TimerSystemManager.Instance.AddCountdown(
    10f,                                    // 总时长10秒
    (remainingTime) => {                    // 每帧回调
        Debug.Log($"剩余时间：{remainingTime:F1}秒");
    },
    () => {                                 // 完成回调
        Debug.Log("倒计时完成！");
    }
);
```

#### 带控制的计时器

```csharp
// 创建可控制的计时器
var timer = TimerSystemManager.Instance.AddOnce(5f, (data) => {
    Debug.Log("5秒后执行");
});

// 暂停计时器
timer.Pause();

// 恢复计时器
timer.Resume();

// 手动移除计时器
TimerSystemManager.Instance.RemoveTimer(timer);
```

#### 不受时间缩放影响的计时器

```csharp
// 即使修改 Time.timeScale，计时器仍按真实时间运行
TimerSystemManager.Instance.AddOnce(
    3f,
    (data) => Debug.Log("不受时间缩放影响"),
    useUnscaledTime: true
);
```

#### 帧计时器

```csharp
// 10帧后执行
TimerSystemManager.Instance.AddFrames(10, (data) => {
    Debug.Log("10帧后执行");
});

// 下一帧执行
TimerSystemManager.Instance.AddNextFrame((data) => {
    Debug.Log("下一帧执行");
});
```

#### 时间缩放

```csharp
// 设置时间缩放为0.5（时间流速减半）
TimerSystemManager.Instance.TimeScale = 0.5f;

// 暂停所有受时间缩放影响的计时器
TimerSystemManager.Instance.TimeScale = 0f;

// 恢复正常时间流速
TimerSystemManager.Instance.TimeScale = 1f;
```

## API 参考

### ITimerSystem 接口

| 方法/属性 | 说明 |
|----------|------|
| `CreateTimer(interval, repeat, callback, userData, useUnscaledTime)` | 创建计时器 |
| `RemoveTimer(timer)` | 移除计时器 |
| `FindTimer(callback)` | 根据回调查找计时器 |
| `PauseTimer(callback)` | 暂停指定回调的计时器 |
| `ResumeTimer(callback)` | 恢复指定回调的计时器 |
| `ClearAllTimers()` | 清理所有计时器 |
| `ActiveTimerCount` | 当前活跃计时器数量 |
| `TimeScale` | 时间缩放系数 |
| `Initialize()` | 初始化计时器系统 |
| `Shutdown()` | 关闭计时器系统 |

### ITimerEntity 接口

| 属性/方法 | 说明 |
|----------|------|
| `IsRunning` | 是否正在运行 |
| `UseUnscaledTime` | 是否使用非缩放时间 |
| `Interval` | 间隔时间 |
| `RemainingRepeatCount` | 剩余重复次数（-1表示无限） |
| `Pause()` | 暂停计时器 |
| `Resume()` | 恢复计时器 |
| `Reset()` | 重置计时器 |

### TimerExtensions 扩展方法

| 方法 | 说明 |
|------|------|
| `AddOnce(seconds, callback, userData, useUnscaledTime)` | 添加一次性计时器 |
| `AddRepeat(seconds, callback, userData, useUnscaledTime)` | 添加无限重复计时器 |
| `AddRepeat(seconds, repeatCount, callback, userData, useUnscaledTime)` | 添加有限次数重复计时器 |
| `AddNextFrame(callback, userData)` | 添加延迟执行计时器（下一帧） |
| `AddFrames(frames, callback, userData, useUnscaledTime)` | 添加延迟执行计时器（指定帧数） |
| `AddCountdown(totalSeconds, onTick, onComplete, useUnscaledTime)` | 添加倒计时计时器 |

## 架构说明

```
Timer/
├── Interfaces/
│   ├── ITimerSystem.cs          # 计时器系统接口
│   └── ITimerEntity.cs          # 计时器实体接口
├── TimerSystemManager.cs        # 核心管理器（单例）
├── TimerEntity.cs               # 计时器实体实现
├── TimerObjectPool.cs           # 对象池实现
├── TimerExtensions.cs           # 扩展方法
├── TimerCallback.cs             # 回调委托定义
└── TimeExample/
    └── TimeExample.cs           # 使用示例
```

### 设计模式

- **单例模式**：TimerSystemManager 使用单例确保全局唯一
- **对象池模式**：TimerObjectPool 重用 TimerEntity 对象，减少 GC
- **接口隔离**：ITimerSystem 和 ITimerEntity 提供清晰的抽象
- **扩展方法模式**：TimerExtensions 提供便捷的链式 API

## 性能优化

### 对象池机制

系统使用对象池来管理 `TimerEntity` 实例：

```csharp
// 初始化时预创建20个对象
_timerPool = new TimerObjectPool<TimerEntity>(20, timer => timer.Reset());
```

**优势**：
- 减少内存分配
- 降低垃圾回收（GC）压力
- 提升频繁创建销毁场景下的性能

### 最佳实践

1. **及时清理**：在 `OnDestroy` 中移除不再需要的计时器
2. **保存引用**：对于需要手动控制的计时器，保存 `ITimerEntity` 引用
3. **合理使用对象池**：系统默认预创建20个对象，根据需求调整
4. **选择合适的时间模式**：UI动画使用 `UnscaledTime`，游戏逻辑使用受 `TimeScale` 影响的时间

## 示例

完整示例请查看 `TimeExample/TimeExample.cs` 文件，包含：
- 一次性计时器
- 有限/无限重复计时器
- 帧计时器
- 倒计时
- 不受时间缩放影响的计时器
- 计时器的暂停和恢复

## 常见问题

### Q: 计时器没有执行？
A: 确保 `TimerSystemManager` 已添加到场景中，且 `Initialize()` 已被调用。

### Q: 如何取消一个计时器？
A: 保存 `ITimerEntity` 引用，调用 `Pause()` 或通过 `RemoveTimer()` 移除。

### Q: TimeScale 为 0 时，某些计时器仍在运行？
A: 这些计时器使用了 `useUnscaledTime: true` 参数，不受 TimeScale 影响。

### Q: 修改 TimeScale 后，已有的计时器会受影响吗？
A: 只有创建时 `useUnscaledTime` 为 `false` 的计时器会受影响。

## 系统要求

- Unity 2020.3 或更高版本
- .NET Framework 4.x 或 .NET Standard 2.0+

## 许可证

本项目遵循 MIT 许可证。