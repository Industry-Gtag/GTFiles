using System;
using System.Collections.Generic;

namespace GorillaExtensions
{
	// Token: 0x020011BF RID: 4543
	public static class DictionaryExtensions
	{
		// Token: 0x06007291 RID: 29329 RVA: 0x00255590 File Offset: 0x00253790
		public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new()
		{
			TValue tvalue;
			if (dict.TryGetValue(key, out tvalue))
			{
				return tvalue;
			}
			dict[key] = new TValue();
			return dict[key];
		}
	}
}
