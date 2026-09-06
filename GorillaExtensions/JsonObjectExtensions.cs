using System;
using System.Runtime.CompilerServices;
using PlayFab.Json;

namespace GorillaExtensions
{
	// Token: 0x020011C5 RID: 4549
	[NullableContext(1)]
	[Nullable(0)]
	public static class JsonObjectExtensions
	{
		// Token: 0x060072AD RID: 29357 RVA: 0x00255B3C File Offset: 0x00253D3C
		[return: Nullable(2)]
		public static T GetValue<[Nullable(2)] T>(this JsonObject obj, string key)
		{
			object obj2;
			if (!obj.TryGetValue(key, out obj2))
			{
				return default(T);
			}
			if (obj2 is T)
			{
				return (T)((object)obj2);
			}
			return default(T);
		}

		// Token: 0x060072AE RID: 29358 RVA: 0x00255B7A File Offset: 0x00253D7A
		public static bool TryGetValue<[Nullable(2)] T>(this JsonObject obj, string key, [Nullable(2)] out T t)
		{
			t = obj.GetValue(key);
			return t != null;
		}
	}
}
