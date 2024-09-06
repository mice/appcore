
public interface IInvokeContainer
{
    string TypeName { get; }
    void Init();
    void Dispose();
    void Invoke(string methodName);
    void Invoke<T>(string methodName, T p0);
    void Invoke<T1, T2>(string methodName, T1 p0, T2 p1);
    void Invoke<T1, T2, T3>(string methodName, T1 p0, T2 p1, T3 p2);
    RET Invoke2<RET>(string methodName);
    RET Invoke2<T1, RET>(string methodName, T1 p0);
}