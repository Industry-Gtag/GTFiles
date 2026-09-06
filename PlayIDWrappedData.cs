using System;

// Token: 0x02000DCC RID: 3532
internal struct PlayIDWrappedData<T>
{
	// Token: 0x060056A6 RID: 22182 RVA: 0x001C45B9 File Offset: 0x001C27B9
	public PlayIDWrappedData(T initialValue)
	{
		this.currentValue = initialValue;
		this.initialValue = initialValue;
		this.id = EnterPlayID.GetCurrent();
	}

	// Token: 0x17000847 RID: 2119
	// (get) Token: 0x060056A7 RID: 22183 RVA: 0x001C45D4 File Offset: 0x001C27D4
	// (set) Token: 0x060056A8 RID: 22184 RVA: 0x001C45F0 File Offset: 0x001C27F0
	public T Value
	{
		get
		{
			if (!this.id.IsCurrent)
			{
				return this.initialValue;
			}
			return this.currentValue;
		}
		set
		{
			this.currentValue = value;
			this.id = EnterPlayID.GetCurrent();
		}
	}

	// Token: 0x04006798 RID: 26520
	private T currentValue;

	// Token: 0x04006799 RID: 26521
	private T initialValue;

	// Token: 0x0400679A RID: 26522
	private EnterPlayID id;
}
