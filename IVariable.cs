using System;

// Token: 0x0200090F RID: 2319
public interface IVariable<T> : IVariable
{
	// Token: 0x1700058D RID: 1421
	// (get) Token: 0x06003CC9 RID: 15561 RVA: 0x0014B49D File Offset: 0x0014969D
	// (set) Token: 0x06003CCA RID: 15562 RVA: 0x0014B4A5 File Offset: 0x001496A5
	T Value
	{
		get
		{
			return this.Get();
		}
		set
		{
			this.Set(value);
		}
	}

	// Token: 0x06003CCB RID: 15563
	T Get();

	// Token: 0x06003CCC RID: 15564
	void Set(T value);

	// Token: 0x1700058E RID: 1422
	// (get) Token: 0x06003CCD RID: 15565 RVA: 0x0014B4AE File Offset: 0x001496AE
	Type IVariable.ValueType
	{
		get
		{
			return typeof(T);
		}
	}
}
