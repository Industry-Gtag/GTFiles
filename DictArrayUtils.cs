using System;
using System.Collections.Generic;

// Token: 0x02000DA3 RID: 3491
public static class DictArrayUtils
{
	// Token: 0x060055DD RID: 21981 RVA: 0x001C1AFD File Offset: 0x001BFCFD
	public static void TryGetOrAddList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key, out List<TValue> list, int capacity)
	{
		if (dict.TryGetValue(key, out list) && list != null)
		{
			return;
		}
		list = new List<TValue>(capacity);
		dict.Add(key, list);
	}

	// Token: 0x060055DE RID: 21982 RVA: 0x001C1B1F File Offset: 0x001BFD1F
	public static void TryGetOrAddArray<TKey, TValue>(this Dictionary<TKey, TValue[]> dict, TKey key, out TValue[] array, int size)
	{
		if (dict.TryGetValue(key, out array) && array != null)
		{
			return;
		}
		array = new TValue[size];
		dict.Add(key, array);
	}
}
