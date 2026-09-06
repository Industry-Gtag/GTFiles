using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003B3 RID: 947
public class WatchableGenericSO<T> : ScriptableObject
{
	// Token: 0x17000238 RID: 568
	// (get) Token: 0x060016D7 RID: 5847 RVA: 0x00085197 File Offset: 0x00083397
	// (set) Token: 0x060016D8 RID: 5848 RVA: 0x0008519F File Offset: 0x0008339F
	private T _value { get; set; }

	// Token: 0x17000239 RID: 569
	// (get) Token: 0x060016D9 RID: 5849 RVA: 0x000851A8 File Offset: 0x000833A8
	// (set) Token: 0x060016DA RID: 5850 RVA: 0x000851B8 File Offset: 0x000833B8
	public T Value
	{
		get
		{
			this.EnsureInitialized();
			return this._value;
		}
		set
		{
			this.EnsureInitialized();
			this._value = value;
			foreach (Action<T> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x00085218 File Offset: 0x00083418
	private void EnsureInitialized()
	{
		if (!this.enterPlayID.IsCurrent)
		{
			this._value = this.InitialValue;
			this.callbacks = new List<Action<T>>();
			this.enterPlayID = EnterPlayID.GetCurrent();
		}
	}

	// Token: 0x060016DC RID: 5852 RVA: 0x0008524C File Offset: 0x0008344C
	public void AddCallback(Action<T> callback, bool shouldCallbackNow = false)
	{
		this.EnsureInitialized();
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			T value = this._value;
			foreach (Action<T> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x060016DD RID: 5853 RVA: 0x000852BC File Offset: 0x000834BC
	public void RemoveCallback(Action<T> callback)
	{
		this.EnsureInitialized();
		this.callbacks.Remove(callback);
	}

	// Token: 0x040021E0 RID: 8672
	public T InitialValue;

	// Token: 0x040021E2 RID: 8674
	private EnterPlayID enterPlayID;

	// Token: 0x040021E3 RID: 8675
	private List<Action<T>> callbacks;
}
