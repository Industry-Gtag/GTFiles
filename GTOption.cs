using System;
using UnityEngine;

// Token: 0x0200034C RID: 844
[Serializable]
public struct GTOption<T>
{
	// Token: 0x17000212 RID: 530
	// (get) Token: 0x060014CE RID: 5326 RVA: 0x0006F266 File Offset: 0x0006D466
	public T ResolvedValue
	{
		get
		{
			if (!this.enabled)
			{
				return this.defaultValue;
			}
			return this.value;
		}
	}

	// Token: 0x060014CF RID: 5327 RVA: 0x0006F27D File Offset: 0x0006D47D
	public GTOption(T defaultValue)
	{
		this.enabled = false;
		this.value = defaultValue;
		this.defaultValue = defaultValue;
	}

	// Token: 0x060014D0 RID: 5328 RVA: 0x0006F294 File Offset: 0x0006D494
	public void ResetValue()
	{
		this.value = this.defaultValue;
	}

	// Token: 0x04001987 RID: 6535
	[Tooltip("When checked, the filter is applied; when unchecked (default), it is ignored.")]
	[SerializeField]
	public bool enabled;

	// Token: 0x04001988 RID: 6536
	[SerializeField]
	public T value;

	// Token: 0x04001989 RID: 6537
	[NonSerialized]
	public readonly T defaultValue;
}
