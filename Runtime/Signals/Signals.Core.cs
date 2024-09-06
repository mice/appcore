using System;


/// <summary>
/// Base interface for Signals
/// </summary>
public interface ISignal
{
    string Hash();

    void SetUpdateHandler(System.Action<ISignal, object, string> onAdd, System.Action<ISignal, object> onRemove);
}

public interface IRemoveable
{
    void RemoveHandler(Delegate handler);
}