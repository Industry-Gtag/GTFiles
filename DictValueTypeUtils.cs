using System;
using System.Collections.Generic;

// Token: 0x02000DA2 RID: 3490
public static class DictValueTypeUtils
{
	// Token: 0x060055DC RID: 21980 RVA: 0x001C1ADC File Offset: 0x001BFCDC
	public static void TryGetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value) where TValue : struct
	{
		if (dict.TryGetValue(key, out value))
		{
			return;
		}
		value = default(TValue);
		dict.Add(key, value);
	}
}
