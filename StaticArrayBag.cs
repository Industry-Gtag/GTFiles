using System;
using System.Collections.Generic;

// Token: 0x02000DE5 RID: 3557
public class StaticArrayBag<T>
{
	// Token: 0x06005722 RID: 22306 RVA: 0x001C7BDC File Offset: 0x001C5DDC
	public T[] GetStaticArray(int size)
	{
		T[] array;
		if (!this.m_bag.ContainsKey(size))
		{
			array = new T[size];
			this.m_bag[size] = array;
		}
		else
		{
			array = this.m_bag[size];
		}
		return array;
	}

	// Token: 0x040067F4 RID: 26612
	private Dictionary<int, T[]> m_bag = new Dictionary<int, T[]>(1);
}
