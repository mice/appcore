using System;
using System.Collections.Generic;
using System.Reflection;

public class NativeInvokeContainer : IInvokeContainer
{
    public string TypeName { get; private set; }
    private Type Type;
    private object instance;
    private Dictionary<string,MethodInfo> methodDict = new Dictionary<string, MethodInfo>();

    public NativeInvokeContainer(string typeName)
    {
        this.TypeName = typeName;
        this.Type = Type.GetType(typeName);
    }

    public void Dispose()
    {
        methodDict.Clear();
        if (instance != null)
        {
            instance = null;
        }
    }

    public void Init()
    {
        instance = System.Activator.CreateInstance(Type);
    }

    public void Invoke(string methodName)
    {
        MethodInfo method = null;
        if (Type != null && !methodDict.TryGetValue(methodName, out method))
        {
            method = Type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            methodDict[methodName] = method;
        }

        if (method != null)
        {
            method.Invoke(instance, System.Array.Empty<object>());
        }
    }

    public void Invoke<T>(string methodName, T p0)
    {
        MethodInfo method = null;
        if (Type != null && !methodDict.TryGetValue(methodName, out method))
        {
            method = Type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            methodDict[methodName] = method;
        }

        if (method != null)
        {
            method.Invoke(instance, new object[1] { p0 });
        }
    }

    public void Invoke<T1, T2>(string methodName, T1 p0, T2 p1)
    {
        MethodInfo method = null;
        if (Type != null && !methodDict.TryGetValue(methodName, out method))
        {
            method = Type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            methodDict[methodName] = method;
        }

        if (method != null)
        {
            method.Invoke(instance, new object[2] { p0, p1 });
        }
    }

    public void Invoke<T1, T2, T3>(string methodName, T1 p0, T2 p1, T3 p2)
    {
        MethodInfo method = null;
        if (Type != null && !methodDict.TryGetValue(methodName, out method))
        {
            method = Type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            methodDict[methodName] = method;
        }

        if (method != null)
        {
            method.Invoke(instance, new object[3] { p0, p1 ,p2});
        }
    }

    public RET Invoke2<RET>(string methodName)
    {
        MethodInfo method = null;
        if (Type != null && !methodDict.TryGetValue(methodName, out method))
        {
            method = Type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            methodDict[methodName] = method;
        }

        if (method != null)
        {
            return (RET)method.Invoke(instance,System.Array.Empty<object>());
        }
        return default;
    }

    public RET Invoke2<T1, RET>(string methodName, T1 p0)
    {
        MethodInfo method = null;
        if (Type != null && !methodDict.TryGetValue(methodName, out method))
        {
            method = Type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            methodDict[methodName] = method;
        }

        if (method != null)
        {
            return (RET)method.Invoke(instance, new object[1] { p0 });
        }
        return default;
    }
}
