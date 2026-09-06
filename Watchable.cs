using System;
using System.Collections.Generic;

// Token: 0x020003B2 RID: 946
public class Watchable<T>
{
	// Token: 0x17000237 RID: 567
	// (get) Token: 0x060016D1 RID: 5841 RVA: 0x00085088 File Offset: 0x00083288
	// (set) Token: 0x060016D2 RID: 5842 RVA: 0x00085090 File Offset: 0x00083290
	public T value
	{
		get
		{
			return this._value;
		}
		set
		{
			T value2 = this._value;
			this._value = value;
			foreach (Action<T> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x060016D3 RID: 5843 RVA: 0x000850F0 File Offset: 0x000832F0
	public Watchable()
	{
	}

	// Token: 0x060016D4 RID: 5844 RVA: 0x00085103 File Offset: 0x00083303
	public Watchable(T initial)
	{
		this._value = initial;
	}

	// Token: 0x060016D5 RID: 5845 RVA: 0x00085120 File Offset: 0x00083320
	public void AddCallback(Action<T> callback, bool shouldCallbackNow = false)
	{
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			foreach (Action<T> action in this.callbacks)
			{
				action(this._value);
			}
		}
	}

	// Token: 0x060016D6 RID: 5846 RVA: 0x00085188 File Offset: 0x00083388
	public void RemoveCallback(Action<T> callback)
	{
		this.callbacks.Remove(callback);
	}

	// Token: 0x040021DE RID: 8670
	private T _value;

	// Token: 0x040021DF RID: 8671
	private List<Action<T>> callbacks = new List<Action<T>>();
}
