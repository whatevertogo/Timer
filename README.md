# Timer 计时器系统

一个高效、灵活的计时器系统实现，专为Unity项目设计。该系统提供了简单易用的API，用于管理和控制各种计时任务。

## 特性

- 🚀 高性能对象池实现，减少GC压力
- 💡 支持多种计时器类型（一次性、循环）
- 🔄 灵活的回调机制
- 🛠 简单易用的API接口
- 🎯 完整的接口抽象，易于扩展


## 使用方法

TimeExample有示例

### 高级用法

```csharp
// 创建可控制的计时器实体
var timerEntity = TimerSystemManager.Instance.CreateTimerEntity();
timerEntity.Init(5.0f, () => {
    Debug.Log("5秒后执行，且可以提前终止!");
});

// 随时终止计时器
timerEntity.Stop();
```

## 主要组件

- `TimerSystemManager`: 计时器系统的核心管理类
- `TimerEntity`: 计时器实体，代表单个计时任务
- `TimerObjectPool`: 计时器对象池，用于优化性能
- `TimerExtensions`:计时器扩展工具方法
- `ITimerSystem`: 计时器系统接口
- `ITimerEntity`: 计时器实体接口

## 示例

查看 `TimeExample` 文件夹中的示例代码，了解更多使用方法。

## 性能优化

系统使用对象池来管理计时器实体，大大减少了垃圾回收的压力。即使在需要频繁创建和销毁计时器的场景下，也能保持稳定的性能。