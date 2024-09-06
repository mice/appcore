using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpdateEventContainer
{
    private UnityEngine.Events.UnityEvent _UpdateEvent = new UnityEngine.Events.UnityEvent();
    private UnityEngine.Events.UnityEvent _LateUpdateEvent = new UnityEngine.Events.UnityEvent();
    private UnityEngine.Events.UnityEvent _FixedUpdateEvent = new UnityEngine.Events.UnityEvent();
    private Dictionary<UnityEngine.Events.UnityAction, int> eventTypeDict = new Dictionary<UnityEngine.Events.UnityAction, int>();

    /// <summary>
    /// 添加外部接口,防止意外
    /// </summary>
    /// <param name="updateType"></param>
    /// <returns></returns>
    public UnityEngine.Events.UnityEvent GetUpdateEvent(int updateType)
    {
        return updateType == 1 ? _LateUpdateEvent :
            updateType == 2 ? _FixedUpdateEvent :
            _UpdateEvent;
    }

    /// <summary>
    /// 1:lateUpdate 
    /// 2:fixedUpdate
    /// 其他为update
    /// </summary>
    /// <param name="action"></param>
    /// <param name="updateType">1,为lateUpdate,2为fixedUpdate,其他为update</param>
    public void AddUpdateEvent(UnityEngine.Events.UnityAction action, int updateType)
    {

        if (eventTypeDict.TryGetValue(action, out var _updateType))
        {
            if (_updateType != updateType)
            {
                DLog.LogError("Error:");
            }
        }
        else
        {
            eventTypeDict[action] = updateType;
        }
        var evt = GetUpdateEvent(updateType);
        evt.RemoveListener(action);
        evt.AddListener(action);
    }

    public void RemoveUpdateEvent(UnityEngine.Events.UnityAction action)
    {
        if (eventTypeDict.TryGetValue(action, out var updateType))
        {
            var evt = GetUpdateEvent(updateType);
            evt.RemoveListener(action);
        }
        else
        {
            _UpdateEvent.RemoveListener(action);
            _LateUpdateEvent.RemoveListener(action);
            _FixedUpdateEvent.RemoveListener(action);
        }
    }

    public void Clear()
    {
        _UpdateEvent.RemoveAllListeners();
        _LateUpdateEvent.RemoveAllListeners();
        _FixedUpdateEvent.RemoveAllListeners();
        eventTypeDict.Clear();
    }
}