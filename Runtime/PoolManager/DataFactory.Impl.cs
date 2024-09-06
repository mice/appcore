using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    public class DataFactoryImpl: global::DataFactory.IDataFactory
    {
        private object _mutex = new object();
        //类对象的常驻数量
        public Dictionary<int, byte> classObjectCount = new Dictionary<int, byte>();

        private Dictionary<int, Queue<object>> m_ClassObjectDic = new Dictionary<int, Queue<object>>();

#if UNITY_EDITOR
        /// <summary> for check double release; </summary>
        private HashSet<object> releasedObjects = new HashSet<object>();
#endif
        public static void Init()
        {
            global::DataFactory.SetDataFactory(new DataFactoryImpl());
        }
        /// <summary>
        /// 获取数据对象，如果没缓存，则创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : new()
        {
#if UNITY_EDITOR
            if (typeof(T).IsValueType)
            {
                UnityEngine.Debug.LogError($"Excludes Value Type"); //ValueType不适合Pool
            }else if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
            {
                UnityEngine.Debug.LogError($"Excludes UnityEngine.Object"); //ValueType不适合Pool
            }
#endif
            lock (_mutex)
            {
                //先找到类的哈希
                int key = typeof(T).GetHashCode();
                Queue<object> queue = null;
                m_ClassObjectDic.TryGetValue(key, out queue);
                if (queue == null)
                {
                    queue = new Queue<object>();
                    m_ClassObjectDic[key] = queue;
                }
                //开始获取对象
                if (queue.Count > 0)
                {
                    object obj = queue.Dequeue();
#if UNITY_EDITOR
                    releasedObjects.Remove(obj);
#endif
                    return (T)obj;
                }
                else
                {
                    return new T();
                }
            }
        }

        System.Collections.Generic.List<T> global::DataFactory.IDataFactory.GetList<T>()
        {
            return Get<List<T>>();
        }

        public void ForceClear<T>()
        {
            ForceClearBy(typeof(T));
        }

        /// <summary>
        /// 强制清除某一类对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ForceClearBy(System.Type type)
        {
            ClearByKey(type.GetHashCode());
        }

        /// <summary>
        /// 设置类常驻数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        public void SetResideCount<T>(byte count) where T : class
        {
            int key = typeof(T).GetHashCode();
            classObjectCount[key] = count;
        }

        private static void Reset(object obj)
        {
            if (obj is MemoryStream)
            {
                var ms = obj as MemoryStream;
                ms.Seek(0, SeekOrigin.Begin);
                ms.SetLength(0);
            }
            else if (obj is System.Text.StringBuilder)
            {
                (obj as System.Text.StringBuilder).Length = 0;
            }else if(obj is System.Collections.IList tmpList)
            {
                tmpList.Clear();
            }
        }

        public void Dispose()
        {
            Clear();
        }

        // 说明：除了指定的协议数据，其它数据暂时不走缓存
        public void Release(object obj)
        {
            lock (_mutex)
            {
                if (obj == null) return;
                Reset(obj);
                int key = obj.GetType().GetHashCode();

                if (m_ClassObjectDic.TryGetValue(key, out var queue))
                {
                    queue.Enqueue(obj);
                }
#if UNITY_EDITOR
                if (releasedObjects.Contains(obj))
                {
                    UnityEngine.Debug.LogError($"Double release {obj.GetType()}");
                }
                releasedObjects.Add(obj);
#endif
            }
        }

        private void ClearByKey(int key)
        {
            Queue<object> queue = m_ClassObjectDic[key];
            int queueCount = queue.Count;
            //释放时候判断，释放数量不能低于常驻数量
            byte resideCount;
            classObjectCount.TryGetValue(key, out resideCount);

            object obj = null;
            while (queueCount > resideCount)
            {
                queueCount--;
                obj = queue.Dequeue();
#if UNITY_EDITOR
                releasedObjects.Remove(obj);
#endif
            }
        }

        //释放对象池
        public void Clear()
        {
#if UNITY_EDITOR
            releasedObjects.Clear();
#endif
            lock (m_ClassObjectDic)
            {
                var lst = new System.Collections.Generic.List<int>(m_ClassObjectDic.Keys);
                int lstCount = lst.Count;

                for (int i = 0; i < lstCount; i++)
                {
                    int key = lst[i];
                    ClearByKey(key);
                }
            }
        }
    }
}