using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003B4 RID: 948
[CreateAssetMenu(fileName = "WatchableStringSO", menuName = "ScriptableObjects/WatchableStringSO")]
public class WatchableStringSO : ScriptableObject
{
	// Token: 0x1700023A RID: 570
	// (get) Token: 0x060016DF RID: 5855 RVA: 0x000852D1 File Offset: 0x000834D1
	// (set) Token: 0x060016E0 RID: 5856 RVA: 0x000852D9 File Offset: 0x000834D9
	private string _value { get; set; }

	// Token: 0x1700023B RID: 571
	// (get) Token: 0x060016E1 RID: 5857 RVA: 0x000852E2 File Offset: 0x000834E2
	// (set) Token: 0x060016E2 RID: 5858 RVA: 0x000852F0 File Offset: 0x000834F0
	public string Value
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
			foreach (Action<string> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x060016E3 RID: 5859 RVA: 0x00085350 File Offset: 0x00083550
	private void EnsureInitialized()
	{
		if (!this.enterPlayID.IsCurrent)
		{
			this._value = this.InitialValue;
			this.callbacks = new List<Action<string>>();
			this.enterPlayID = EnterPlayID.GetCurrent();
		}
	}

	// Token: 0x060016E4 RID: 5860 RVA: 0x00085384 File Offset: 0x00083584
	public void AddCallback(Action<string> callback, bool shouldCallbackNow = false)
	{
		this.EnsureInitialized();
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			string value = this._value;
			foreach (Action<string> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x060016E5 RID: 5861 RVA: 0x000853F4 File Offset: 0x000835F4
	public void RemoveCallback(Action<string> callback)
	{
		this.EnsureInitialized();
		this.callbacks.Remove(callback);
	}

	// Token: 0x060016E6 RID: 5862 RVA: 0x00085409 File Offset: 0x00083609
	public override string ToString()
	{
		return this.Value;
	}

	// Token: 0x040021E4 RID: 8676
	[TextArea]
	public string InitialValue;

	// Token: 0x040021E6 RID: 8678
	private EnterPlayID enterPlayID;

	// Token: 0x040021E7 RID: 8679
	private List<Action<string>> callbacks;
}
