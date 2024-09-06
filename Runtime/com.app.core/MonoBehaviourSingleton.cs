using System;
using UnityEngine;

public class MonoBehaviourSingleton<T> : DisableNotifyMonoBehaviour where T : MonoBehaviour
{
	private static T _inst;

	public static T I
	{
		get
		{
			if (_inst == null)
			{
				_inst = FindObjectOfType<T>();
				if (_inst == null)
				{
					//Log.Error(LOG.SYSTEM, typeof(T) + " is nothing", new object[0]);
				}
			}
			return _inst;
		}
	}

	private void OnDestroy()
	{
        if (!AppStatus.isApplicationQuit)
        {
            _OnDestroy();
        }
        if (_inst == this)
		{
			OnDestroySingleton();
			_inst = null;
		}
	}

	protected virtual void _OnDestroy()
	{
	}

	protected virtual void OnDestroySingleton()
	{
	}

	protected override void Awake()
	{
        base.Awake();
		Check_inst();
        OnAwake();
    }

    protected virtual void OnAwake()
    {
       
    }

    protected bool Check_inst()
	{
		if (this == I)
		{
			return true;
		}
#if UNITY_EDITOR
        if( !Application.isPlaying && string.IsNullOrEmpty(gameObject.scene.name))
        {
            return false;
        }
#endif
        if (this != null)
        {
            Destroy(this);
        }
		return false;
	}

    public static T GetOrCreate(Transform parent)
    {
        if (MonoBehaviourSingleton<T>.I == null) //同步第一次.
        {
            Transform tfm = null;
            if (parent == null)
            {
                tfm = new GameObject(typeof(T).Name).transform;
                GameObject.DontDestroyOnLoad(tfm);
            }
            else
            {
                tfm = _CreateOrGetInChild(parent, typeof(T).Name, null);
            }

            {
                tfm.gameObject.GetOrAddComponent<T>();
            }
        }

        return MonoBehaviourSingleton<T>.I;//同步第二次
    }

    public static bool IsValid()
	{
		return _inst != null;
	}

    //同步方法:Utils.Utility
    private static Transform _CreateOrGetInChild(Transform container, string name, System.Action<GameObject> onCreated)
    {
        if (container == null)
        {
            UnityEngine.Debug.LogWarning("container is Null");
            return null;
        }
        var fx_building_container = container.Find(name);
        if (fx_building_container == null)
        {
            fx_building_container = new GameObject(name).transform;
            fx_building_container.SetParent(container);
            onCreated?.Invoke(fx_building_container?.gameObject);
        }
        return fx_building_container;
    }
}
