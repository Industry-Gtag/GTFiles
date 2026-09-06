using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

// Token: 0x020001E6 RID: 486
public static class NativeCollectionExtensions
{
	// Token: 0x06000CC4 RID: 3268 RVA: 0x000468E0 File Offset: 0x00044AE0
	public static T[] ToArray<[IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType
	{
		return list.AsArray().ToArray();
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x000468FC File Offset: 0x00044AFC
	public static List<T> ToList<[IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType
	{
		List<T> list2 = new List<T>(list.Length);
		for (int i = 0; i < list.Length; i++)
		{
			list2.Add(list[i]);
		}
		return list2;
	}
}
