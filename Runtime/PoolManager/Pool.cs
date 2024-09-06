using System;
using System.Collections.Generic;
using UnityEngine;


public class Pool<ABS_T> where ABS_T : Poolable
{
	private Dictionary<string,Queue<ABS_T>> poolablesOfType = new Dictionary<string, Queue<ABS_T>>();

	private string GetKey<T>()
	{
		return typeof(T).ToString();
	}

	private string GetKey(Type type)
	{
		return type.ToString();
	}

	public T Alloc<T>() where T : Poolable, ABS_T, new()
	{
		if (poolablesOfType.TryGetValue(this.GetKey<T>(),out var queue))
		{
			queue = new Queue<ABS_T>();
			this.poolablesOfType.Add(this.GetKey<T>(), queue);
		}
		T result = (T)((object)null);
		if (queue.Count > 0)
		{
			result = (T)((object)queue.Dequeue());
		}
		else
		{
			result = Activator.CreateInstance<T>();
			result.OnAwake();
		}
		result.OnInit();
		return result;
	}

	public void Free(ABS_T poolable)
	{
		if (!this.poolablesOfType.TryGetValue(this.GetKey(poolable.GetType()), out var queue))
		{
			Debug.LogError("Pool: not alloc poolable. poolable=" + poolable);
			return;
		}
		poolable.OnFinal();
		queue.Enqueue(poolable);
	}

	public void Clear()
	{
		foreach(var item in poolablesOfType)
        {
			item.Value.Clear();
        }
	}
}
