using System;
using System.Collections.Generic;

/// <summary>
/// Signals main facade class
/// inner+class:A+B;
/// </summary>
public static partial class Signals
{
    private static Dictionary<Type, ISignal> signals = new Dictionary<Type, ISignal>();
    private static Dictionary<object, string> handlerDict = new Dictionary<object, string>();

    public static SType Get<SType>() where SType : ISignal, new()
    {
        Type signalType = typeof(SType);
        return (SType)_internal_Get(signalType);
    }

    public static ISignal Get(string typeName)
    {
        Type signalType = Type.GetType(typeName);
        if (signalType == null)
            throw new Exception($"{typeName} is Not a Valid Type Name");
        return _internal_Get(signalType) as ISignal;
    }

    /// <summary>只Get不创建,无法知道创建</summary>
    public static ISignal GetByHash(string hashName)
    {
        foreach (var t in signals.Values)
        {
            if (t.Hash() == hashName)
            {
                return t;
            }
        }
        return null;
    }

    private static ISignal _internal_Get(Type signalType)
    {
        ISignal signal;
        if (signals.TryGetValue(signalType, out signal))
        {
            return signal as ISignal;
        }
        return Bind(signalType) as ISignal;
    }

    public static void RemoveHandler(System.Delegate handler)
    {
        if (handler == null)
            return;
        if (handlerDict.ContainsKey(handler))
        {
            var signal = GetSignalByHash(handlerDict[handler]);
            (signal as IRemoveable)?.RemoveHandler(handler);
        }
    }

    public static void RemoveHandlers(string group)
    {
        throw new NotImplementedException();
    }

    private static ISignal Bind(Type signalType)
    {
        ISignal signal;
        if (signals.TryGetValue(signalType, out signal))
        {
            LogError(string.Format("Signal already registered for type {0}", signalType.ToString()));
            return signal;
        }

        signal = (ISignal)Activator.CreateInstance(signalType);
        signals.Add(signalType, signal);
        signal.SetUpdateHandler(_OnAddHandler, _RemoveHandler);
        return signal;
    }

    /// <summary>
    /// 需要一个链表,否则重复添加的情况,无法处理.
    /// </summary>
    /// <param name="signal"></param>
    /// <param name="handler"></param>
    private static void _OnAddHandler(ISignal signal, System.Object handler,string group)
    {
        //UnityEngine.Debug.Assert(!handlerDict.ContainsKey(handler)|| handlerDict[handler] == signal.Hash,"duplicate add");
        handlerDict[handler] = signal.Hash();
        if (!string.IsNullOrEmpty(group))
        {

        }
    }

    private static void _RemoveHandler(ISignal signal, System.Object handler)
    {
        handlerDict.Remove(signal);
    }

    private static ISignal Bind<T>() where T : ISignal, new()
    {
        return Bind(typeof(T));
    }

    private static ISignal GetSignalByHash(string signalHash)
    {
        foreach (ISignal signal in signals.Values)
        {
            if (signal.Hash() == signalHash)
            {
                return signal;
            }
        }
        return null;
    }

    private static void LogError(string content)
    {

    }
}
