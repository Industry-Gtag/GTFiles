using System;
using System.Collections.Generic;

namespace GorillaExtensions
{
	// Token: 0x020011BE RID: 4542
	public static class CollectionExtensions
	{
		// Token: 0x0600728E RID: 29326 RVA: 0x002554A8 File Offset: 0x002536A8
		public static void AddAll<T>(this ICollection<T> collection, IEnumerable<T> ts)
		{
			foreach (T t in ts)
			{
				collection.Add(t);
			}
		}

		// Token: 0x0600728F RID: 29327 RVA: 0x002554F0 File Offset: 0x002536F0
		public static void CopyStringKeepDelimiterAtEnd(this HashSet<string> hash, string str, char delimiter)
		{
			if (string.IsNullOrEmpty(str))
			{
				return;
			}
			int i = 0;
			int num = 0;
			int length = str.Length;
			while (i < length)
			{
				if (str[i] == delimiter)
				{
					hash.Add(str.Substring(num, i - num));
					num = i + 1;
				}
				i++;
			}
		}

		// Token: 0x06007290 RID: 29328 RVA: 0x0025553C File Offset: 0x0025373C
		public static bool ContainsAll<T>(this ICollection<T> collection, IEnumerable<T> ts)
		{
			foreach (T t in ts)
			{
				if (!collection.Contains(t))
				{
					return false;
				}
			}
			return true;
		}
	}
}
