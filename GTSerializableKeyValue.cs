using System;

// Token: 0x02000383 RID: 899
[Serializable]
public struct GTSerializableKeyValue<T1, T2>
{
	// Token: 0x060015F3 RID: 5619 RVA: 0x0007464D File Offset: 0x0007284D
	public GTSerializableKeyValue(T1 k, T2 v)
	{
		this.k = k;
		this.v = v;
	}

	// Token: 0x04001AC7 RID: 6855
	public T1 k;

	// Token: 0x04001AC8 RID: 6856
	public T2 v;
}
