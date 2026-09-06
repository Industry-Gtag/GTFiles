using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x0200111F RID: 4383
	public static class ExtensionMethods
	{
		// Token: 0x06006DE7 RID: 28135 RVA: 0x00237A04 File Offset: 0x00235C04
		public static void SafeInvoke<T>(this Action<T> action, T data)
		{
			try
			{
				if (action != null)
				{
					action(data);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[PlayFabTitleDataCache::SafeInvoke] Failure invoking action: {0}", ex));
			}
		}

		// Token: 0x06006DE8 RID: 28136 RVA: 0x00237A40 File Offset: 0x00235C40
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			dict[key] = value;
		}
	}
}
