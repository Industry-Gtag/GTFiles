using System;
using System.Collections.Generic;

// Token: 0x02000DA1 RID: 3489
public static class DictRefTypeUtils
{
	// Token: 0x060055DB RID: 21979 RVA: 0x001C1AAA File Offset: 0x001BFCAA
	public static void TryGetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value) where TValue : class, new()
	{
		if (dict.TryGetValue(key, out value) && value != null)
		{
			return;
		}
		value = new TValue();
		dict.Add(key, value);
	}
}
