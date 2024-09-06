using System;

namespace Common.Core
{
    /// <summary>
    /// 单例方法的基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : IHotFix where T : class, new()
    {
        private static T _inst;

        public static T I
        {
            get
            {
                if (_inst == null)
                {
                    _inst = new T();
                    if (_inst != null)
                    {
                        (_inst as Singleton<T>).Init();
                    }
                }

                return _inst;
            }
        }
        /// <summary>
        /// 释放单例，引用置为null
        /// </summary>
        public static void Release()
        {
            if (_inst != null)
            {
                (_inst as Singleton<T>).Dispose();
                _inst = null;
            }
        }
        /// <summary>
        /// 单例第一次实例化时调用
        /// </summary>
        protected virtual void Init()
        {
        }
        /// <summary>
        /// 单例被释放时调用。
        /// </summary>
        public virtual void Dispose()
        {
            _inst = null;
        }
        /// <summary>
        /// 单例是否有效
        /// </summary>
        /// <returns></returns>
        public static bool IsValid()
        {
            return _inst != null;
        }
    }
}