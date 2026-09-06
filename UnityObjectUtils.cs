using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000DF6 RID: 3574
public static class UnityObjectUtils
{
	// Token: 0x060057A3 RID: 22435 RVA: 0x001C9500 File Offset: 0x001C7700
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T AsNull<T>(this T obj) where T : Object
	{
		if (obj == null)
		{
			return default(T);
		}
		if (!(obj == null))
		{
			return obj;
		}
		return default(T);
	}

	// Token: 0x060057A4 RID: 22436 RVA: 0x0004693E File Offset: 0x00044B3E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SafeDestroy(this Object obj)
	{
		Object.Destroy(obj);
	}
}
