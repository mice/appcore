
using System.Collections.Generic;
using System;

/// <summary>
/// Abstract class for Signals, provides hash by type functionality
/// </summary>
public abstract class ABaseSignal : ISignal
{
    protected string _hash;
    protected System.Action<ISignal, object, string> onAdd;
    protected System.Action<ISignal, object> onRemove;

    /// <summary>
    /// Unique id for this signal
    /// </summary>
    public string Hash()
    {
        if (string.IsNullOrEmpty(_hash))
        {
            _hash = this.GetType().ToString();
        }
        return _hash;
    }

    public void SetUpdateHandler(System.Action<ISignal, object,string> onAdd, System.Action<ISignal, object> onRemove)
    {
        this.onAdd = onAdd;
        this.onRemove = onRemove;
    }
}

/// <summary>
/// Strongly typed messages with no parameters
/// </summary>
public abstract class ASignal : ABaseSignal, IRemoveable
{
    private List<System.Action> callback = new List<System.Action>(4);

    public ASignal()
    {

    }

    /// <summary>
    /// Adds a listener to this Signal
    /// </summary>
    /// <param name="handler">Method to be called when signal is fired</param>
    public System.Action AddListener(System.Action handler, bool useOnce)
    {
        if (!callback.Contains(handler))
        {
            callback.Add(handler);
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
        if (callback.Remove(handler))
        {
            onRemove(this, handler);
        }
    }

    void IRemoveable.RemoveHandler(Delegate handler)
    {
        RemoveListener(handler as System.Action);
    }

    /// <summary>
    /// Dispatch this signal
    /// </summary>
    public void Dispatch()
    {
#if UNITY_EDITOR
        UnityEngine.Profiling.Profiler.BeginSample("Signals Dispatch");
#endif
        if (callback != null)
        {
            for (int i = callback.Count - 1; i >= 0; i--)
            {
                callback[i]?.Invoke();
            }
        }
#if UNITY_EDITOR
        UnityEngine.Profiling.Profiler.EndSample();
#endif
    }
}

/// <summary>
/// Strongly typed messages with 1 parameter
/// </summary>
/// <typeparam name="T">Parameter type</typeparam>
public abstract class ASignal<T> : ABaseSignal, IRemoveable
{
    private List<System.Action<T>> callback = new List<System.Action<T>>(4);

    /// <summary>
    /// Adds a listener to this Signal
    /// </summary>
    /// <param name="handler">Method to be called when signal is fired</param>
    public System.Action<T> AddListener(System.Action<T> handler)
    {
        if (!callback.Contains(handler))
        {
            callback.Add(handler);
            onAdd(this, handler, string.Empty);
        }
        return handler;
    }

    /// <summary>
    /// Removes a listener from this Signal
    /// </summary>
    /// <param name="handler">Method to be unregistered from signal</param>
    public void RemoveListener(System.Action<T> handler)
    {
        if (callback.Remove(handler))
        {
            onRemove(this, handler);
        }
    }

    void IRemoveable.RemoveHandler(Delegate handler)
    {
        RemoveListener(handler as System.Action<T>);
    }

    /// <summary>
    /// Dispatch this signal with 1 parameter
    /// </summary>
    public void Dispatch(T arg1)
    {
#if UNITY_EDITOR
        UnityEngine.Profiling.Profiler.BeginSample("Signals Dispatch");
#endif
        if (callback != null)
        {
            for (int i = callback.Count - 1; i >= 0; i--)
            {
                callback[i]?.Invoke(arg1);
            }
        }
#if UNITY_EDITOR
        UnityEngine.Profiling.Profiler.EndSample();
#endif
    }
}

/// <summary>
/// Strongly typed messages with 2 parameters
/// </summary>
/// <typeparam name="T">First parameter type</typeparam>
/// <typeparam name="U">Second parameter type</typeparam>
public abstract class ASignal<T, U> : ABaseSignal, IRemoveable
{
    private List<System.Action<T, U>> callback = new List<System.Action<T, U>>(4);

    /// <summary>
    /// Adds a listener to this Signal
    /// </summary>
    /// <param name="handler">Method to be called when signal is fired</param>
    public System.Action<T, U> AddListener(System.Action<T, U> handler)
    {
        if (!callback.Contains(handler))
        {
            callback.Add(handler);
            onAdd(this, handler, string.Empty);
        }
        return handler;
    }

    /// <summary>
    /// Removes a listener from this Signal
    /// </summary>
    /// <param name="handler">Method to be unregistered from signal</param>
    public void RemoveListener(System.Action<T, U> handler)
    {
        if (callback.Remove(handler))
        {
            onRemove(this, handler);
        }
    }

    void IRemoveable.RemoveHandler(Delegate handler)
    {
        RemoveListener(handler as System.Action<T, U>);
    }

    /// <summary>
    /// Dispatch this signal
    /// </summary>
    public void Dispatch(T arg1, U arg2)
    {
#if UNITY_EDITOR
        UnityEngine.Profiling.Profiler.BeginSample("Signals Dispatch");
#endif
        if (callback != null)
        {
            for (int i = callback.Count - 1; i >= 0; i--)
            {
                callback[i]?.Invoke(arg1, arg2);
            }
        }
#if UNITY_EDITOR
        UnityEngine.Profiling.Profiler.EndSample();
#endif
    }
}

/// <summary>
/// Strongly typed messages with 3 parameter
/// </summary>
/// <typeparam name="T">First parameter type</typeparam>
/// <typeparam name="U">Second parameter type</typeparam>
/// <typeparam name="V">Third parameter type</typeparam>
public abstract class ASignal<T, U, V> : ABaseSignal, IRemoveable
{
    private List<System.Action<T, U, V>> callback = new List<System.Action<T, U, V>>(4);

    /// <summary>
    /// Adds a listener to this Signal
    /// </summary>
    /// <param name="handler">Method to be called when signal is fired</param>
    public System.Action<T, U, V> AddListener(System.Action<T, U, V> handler)
    {
        if (!callback.Contains(handler))
        {
            callback.Add(handler);
            onAdd(this, handler,string.Empty);
        }
        return handler;
    }

    /// <summary>
    /// Removes a listener from this Signal
    /// </summary>
    /// <param name="handler">Method to be unregistered from signal</param>
    public void RemoveListener(System.Action<T, U, V> handler)
    {
        if (callback.Remove(handler))
        {
            onRemove(this, handler);
        }
    }

    void IRemoveable.RemoveHandler(Delegate handler)
    {
        RemoveListener(handler as System.Action<T, U, V>);
    }

    /// <summary>
    /// Dispatch this signal
    /// </summary>
    public void Dispatch(T arg1, U arg2, V arg3)
    {
#if UNITY_EDITOR
        UnityEngine.Profiling.Profiler.BeginSample("Signals Dispatch");
#endif
        if (callback != null)
        {
            for (int i = callback.Count - 1; i >= 0; i--)
            {
                callback[i]?.Invoke(arg1, arg2, arg3);
            }
        }
#if UNITY_EDITOR
        UnityEngine.Profiling.Profiler.EndSample();
#endif
    }
}

public class EventSignal : AbstractDataSignal<String>
{

}