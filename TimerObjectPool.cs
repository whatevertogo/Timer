using System;
using System.Collections.Generic;

namespace Timer
{
    /// <summary>
    /// 简单对象池，用于重用计时器监听器对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public class TimerObjectPool<T> where T : class, new()
    {
        private readonly Stack<T> _pool = new Stack<T>();
        private readonly Action<T> _resetAction;
        private int _capacity;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="initialCapacity">初始容量</param>
        /// <param name="resetAction">对象重置方法</param>
        public TimerObjectPool(int initialCapacity = 10, Action<T> resetAction = null)
        {
            _capacity = initialCapacity;
            _resetAction = resetAction;

            // 预创建对象
            for (int i = 0; i < initialCapacity; i++)
            {
                _pool.Push(new T());
            }
        }

        /// <summary>
        /// 从池中获取对象
        /// </summary>
        /// <returns>对象实例</returns>
        public T Get()
        {
            if (_pool.Count > 0)
            {
                return _pool.Pop();
            }
            return new T();
        }

        /// <summary>
        /// 归还对象到池中
        /// </summary>
        /// <param name="obj">要归还的对象</param>
        public void Release(T obj)
        {
            if (obj == null) return;

            _resetAction?.Invoke(obj);
            _pool.Push(obj);
        }

        /// <summary>
        /// 清空对象池
        /// </summary>
        public void Clear()
        {
            _pool.Clear();
        }
    }
}
