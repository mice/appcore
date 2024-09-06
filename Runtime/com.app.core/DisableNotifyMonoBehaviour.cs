using System;
using System.Collections.Generic;
using UnityEngine;

public class DisableNotifyMonoBehaviour : MonoBehaviour
{
	public Transform _transform
	{
		get;
		private set;
	}

	public DisableNotifyMonoBehaviour notifyMaster
	{
		get;
		private set;
	}

	public List<DisableNotifyMonoBehaviour> notifyServants
	{
		get;
		private set;
	}

	protected virtual void Awake()
	{
		this._transform = base.transform;
	}

	protected virtual void OnDisable()
	{
		if (this.notifyServants != null)
		{
			this.notifyServants.ForEach(delegate(DisableNotifyMonoBehaviour o)
			{
				o.OnDisableMaster();
			});
			this.notifyServants.Clear();
			this.notifyServants = null;
		}
		this.ResetNotifyMaster();
	}

	private void OnApplicationQuit()
	{
		this.notifyMaster = null;
		if (this.notifyServants != null)
		{
			this.notifyServants.Clear();
			this.notifyServants = null;
		}
	}

	public virtual void SetNotifyMaster(DisableNotifyMonoBehaviour master)
	{
		this.ResetNotifyMaster();
		this.notifyMaster = master;
		if (master.notifyServants == null)
		{
			master.notifyServants = new List<DisableNotifyMonoBehaviour>();
		}
		master.notifyServants.Add(this);
		master.OnAttachServant(this);
	}

	public void ResetNotifyMaster()
	{
		if (this.notifyMaster != null)
		{
			if (this.notifyMaster.notifyServants != null)
			{
				this.notifyMaster.notifyServants.Remove(this);
			}
			this.notifyMaster.OnDetachServant(this);
			this.notifyMaster = null;
		}
	}

	protected virtual void OnDisableMaster()
	{
	}

	protected virtual void OnAttachServant(DisableNotifyMonoBehaviour servant)
	{
	}

	protected virtual void OnDetachServant(DisableNotifyMonoBehaviour servant)
	{
	}
}
