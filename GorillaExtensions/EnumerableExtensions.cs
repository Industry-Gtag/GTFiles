using System;
using System.Collections.Generic;

namespace GorillaExtensions
{
	// Token: 0x020011C0 RID: 4544
	public static class EnumerableExtensions
	{
		// Token: 0x06007292 RID: 29330 RVA: 0x002555C0 File Offset: 0x002537C0
		public static TValue MinBy<TValue, TKey>(this IEnumerable<TValue> ts, Func<TValue, TKey> keyGetter) where TKey : struct, IComparable<TKey>
		{
			TValue tvalue = default(TValue);
			TKey? tkey = null;
			foreach (TValue tvalue2 in ts)
			{
				TKey tkey2 = keyGetter(tvalue2);
				if (tkey == null || tkey2.CompareTo(tkey.Value) < 0)
				{
					tvalue = tvalue2;
					tkey = new TKey?(tkey2);
				}
			}
			if (tkey == null)
			{
				throw new ArgumentException("Cannot calculate MinBy on an empty IEnumerable.");
			}
			return tvalue;
		}

		// Token: 0x06007293 RID: 29331 RVA: 0x0025565C File Offset: 0x0025385C
		public static IEnumerable<T> Peek<T>(this IEnumerable<T> ts, Action<T> action)
		{
			foreach (T t in ts)
			{
				action(t);
				yield return t;
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}
	}
}
