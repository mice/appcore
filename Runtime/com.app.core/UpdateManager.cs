using UnityEngine.Events;

public class UpdateManager : MonoBehaviourSingleton<UpdateManager>
{
    private UpdateEventContainer eventContainer = new UpdateEventContainer();

    public void AddUpdate(UnityAction action)
    {
        eventContainer.AddUpdateEvent(action, 0);
    }

    public void UpdateLateUpdate(UnityAction action)
    {
        eventContainer.AddUpdateEvent(action, 1);
    }

    public void RemoveUpdate(UnityAction action)
    {
        eventContainer.RemoveUpdateEvent(action);
    }

    private void Update()
    {
        var e = eventContainer.GetUpdateEvent(0);
        e.Invoke();
    }

    private void LateUpdate()
    {
        var e = eventContainer.GetUpdateEvent(1);
        e.Invoke();
    }

    public void Clear()
    {
        eventContainer.Clear();
    }
}