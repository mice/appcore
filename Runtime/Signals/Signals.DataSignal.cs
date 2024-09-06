
using System.Collections.Generic;
using System;

public abstract class AbstractDataSignal<T> : ABaseSignal, IRemoveable where T : IEquatable<T>
{
    private Dictionary<T, List<System.Action>> protoHandlerDict = new Dictionary<T, List<System.Action>>();
    private Dictionary<object, T> handlerDict = new Dictionary<object, T>();

    public System.Action AddListener(T proto_id, System.Action handler)
    {
        if (!protoHandlerDict.ContainsKey(proto_id))
        {
            protoHandlerDict[proto_id] = new List<System.Action>(4);
        }
        if (!protoHandlerDict[proto_id].Contains(handler))
        {
            //UnityEngine.Debug.Assert(!handlerDict.ContainsKey(handler) || (handlerDict[handler].GetHashCode() == proto_id.GetHashCode()), "one handler multi key");
            handlerDict[handler] = proto_id;
            protoHandlerDict[proto_id].Add(handler);
            onAdd(this, handler, string.Empty);
        }
        return handler;
    }


    /// <summary>
    /// Removes a listener from this Signal
    /// </summary>
    /// <param name="handler">Method to be unregistered from signal</param>
    public void RemoveListener(System.Action handler)
    {
        if (handlerDict.ContainsKey(handler))
        {
            var proto_id = handlerDict[handler];
            RemoveListener(proto_id, handler);
        }
    }

    void IRemoveable.RemoveHandler(Delegate handler)
    {
        RemoveListener(handler as System.Action);
    }

    private void RemoveListener(T proto_id, System.Action handler)
    {
        handlerDict.Remove(handler);
        if (protoHandlerDict.ContainsKey(proto_id) && protoHandlerDict[proto_id].Remove(handler))
        {
            onRemove(this, handler);
        }
    }

    /// <summary>
    /// Dispatch this signal
    /// </summary>
    public void Dispatch(T proto_id)
    {
#if UNITY_EDITOR
        UnityEngine.Profiling.Profiler.BeginSample("Signals Dispatch"); 
#endif

        if (protoHandlerDict.ContainsKey(proto_id))
        {
            foreach (var handler in protoHandlerDict[proto_id])
                handler?.Invoke();
        }
#if UNITY_EDITOR
        UnityEngine.Profiling.Profiler.EndSample();
#endif
    }
}

public abstract class AbstractDataSignal<T, U> : ABaseSignal, IRemoveable
{
    private Dictionary<T, List<System.Action<U>>> protoHandlerDict = new Dictionary<T, List<System.Action<U>>>();
    private Dictionary<object, T> handlerDict = new Dictionary<object, T>();

    public System.Action<U> AddListener(T proto_id, System.Action<U> handler)
    {
        if (!protoHandlerDict.ContainsKey(proto_id))
        {
            protoHandlerDict[proto_id] = new List<System.Action<U>>();
        }
        if (!protoHandlerDict[proto_id].Contains(handler))
        {
            //UnityEngine.Debug.Assert(!handlerDict.ContainsKey(handler)|| (handlerDict[handler].GetHashCode() == proto_id.GetHashCode()),"one handler multi key");
            handlerDict[handler] = proto_id;
            protoHandlerDict[proto_id].Add(handler);
            onAdd(this, handler, string.Empty);
        }
        return handler;
    }

    /// <summary>
    /// Removes a listener from this Signal
    /// </summary>
    /// <param name="handler">Method to be unregistered from signal</param>
    public void RemoveListener(System.Action<U> handler)
    {
        if (handlerDict.ContainsKey(handler))
        {
            var proto_id = handlerDict[handler];
            RemoveListener(proto_id, handler);
        }
    }

    void IRemoveable.RemoveHandler(Delegate handler)
    {
        RemoveListener(handler as System.Action<U>);
    }

    private void RemoveListener(T proto_id, System.Action<U> handler)
    {
        handlerDict.Remove(handler);
        if (protoHandlerDict.ContainsKey(proto_id) && protoHandlerDict[proto_id].Remove(handler))
        {
            onRemove(this, handler);
        }
    }

    /// <summary>
    /// Dispatch this signal
    /// </summary>
    public void Dispatch(T proto_id, U data)
    {
#if UNITY_EDITOR
        UnityEngine.Profiling.Profiler.BeginSample("Signals Dispatch");
#endif
        if (protoHandlerDict.ContainsKey(proto_id))
        {
            for (int i = protoHandlerDict[proto_id].Count - 1; i >= 0; i--)
            {
                protoHandlerDict[proto_id][i]?.Invoke(data);
            }
        }
#if UNITY_EDITOR
        UnityEngine.Profiling.Profiler.EndSample();
#endif
    }
}

public abstract class AbstractDataSignal<T, U, V> : ABaseSignal, IRemoveable
{
    private Dictionary<T, List<System.Action<U, V>>> protoHandlerDict = new Dictionary<T, List<System.Action<U, V>>>();
    private Dictionary<object, T> handlerDict = new Dictionary<object, T>();

    public System.Action<U, V> AddListener(T proto_id, System.Action<U, V> handler)
    {
        if (!protoHandlerDict.ContainsKey(proto_id))
        {
            protoHandlerDict[proto_id] = new List<System.Action<U, V>>(4);
        }
        if (!protoHandlerDict[proto_id].Contains(handler))
        {
            //UnityEngine.Debug.Assert(!handlerDict.ContainsKey(handler) || (handlerDict[handler].GetHashCode() == proto_id.GetHashCode()), "one handler multi key");
            handlerDict[handler] = proto_id;
            protoHandlerDict[proto_id].Add(handler);
            onAdd(this, handler, string.Empty);
        }
        return handler;
    }

    /// <summary>
    /// Removes a listener from this Signal
    /// </summary>
    /// <param name="handler">Method to be unregistered from signal</param>
    public void RemoveListener(System.Action<U, V> handler)
    {
        if (handlerDict.ContainsKey(handler))
        {
            var proto_id = handlerDict[handler];
            RemoveListener(proto_id, handler);
        }
    }

    void IRemoveable.RemoveHandler(Delegate handler)
    {
        RemoveListener(handler as System.Action<U, V>);
    }

    private void RemoveListener(T proto_id, System.Action<U, V> handler)
    {
        handlerDict.Remove(handler);
        if (protoHandlerDict.ContainsKey(proto_id) && protoHandlerDict[proto_id].Remove(handler))
        {
            onRemove(this, handler);
        }
    }

    /// <summary>
    /// Dispatch this signal
    /// </summary>
    public void Dispatch(T proto_id, U data, V param)
    {
#if UNITY_EDITOR
        UnityEngine.Profiling.Profiler.BeginSample("Signals Dispatch");
#endif
        if (protoHandlerDict.ContainsKey(proto_id))
        {
            for (int i = protoHandlerDict[proto_id].Count - 1; i >= 0; i--)
            {
                protoHandlerDict[proto_id][i]?.Invoke(data, param);
            }
        }
#if UNITY_EDITOR
        UnityEngine.Profiling.Profiler.EndSample();
#endif
    }
}

