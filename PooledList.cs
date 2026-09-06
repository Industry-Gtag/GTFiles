using System;
using System.Collections.Generic;
using GorillaTag;

// Token: 0x02000DFC RID: 3580
public class PooledList<T> : ObjectPoolEvents
{
	// Token: 0x060057AC RID: 22444 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ObjectPoolEvents.OnTaken()
	{
	}

	// Token: 0x060057AD RID: 22445 RVA: 0x001C9593 File Offset: 0x001C7793
	void ObjectPoolEvents.OnReturned()
	{
		this.List.Clear();
	}

	// Token: 0x0400682D RID: 26669
	public List<T> List = new List<T>();
}
