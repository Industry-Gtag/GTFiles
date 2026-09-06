using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000D7D RID: 3453
public static class ArrayUtils
{
	// Token: 0x0600551F RID: 21791 RVA: 0x001BE372 File Offset: 0x001BC572
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int BinarySearch<T>(this T[] array, T value) where T : IComparable<T>
	{
		return Array.BinarySearch<T>(array, 0, array.Length, value);
	}

	// Token: 0x06005520 RID: 21792 RVA: 0x001BE37F File Offset: 0x001BC57F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrEmpty<T>(this T[] array)
	{
		return array == null || array.Length == 0;
	}

	// Token: 0x06005521 RID: 21793 RVA: 0x001BE38B File Offset: 0x001BC58B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullOrEmpty<T>(this List<T> list)
	{
		return list == null || list.Count == 0;
	}

	// Token: 0x06005522 RID: 21794 RVA: 0x001BE39C File Offset: 0x001BC59C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(this T[] array, int from, int to)
	{
		T t = array[from];
		T t2 = array[to];
		array[to] = t;
		array[from] = t2;
	}

	// Token: 0x06005523 RID: 21795 RVA: 0x001BE3D4 File Offset: 0x001BC5D4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(this List<T> list, int from, int to)
	{
		T t = list[from];
		T t2 = list[to];
		list[to] = t;
		list[from] = t2;
	}

	// Token: 0x06005524 RID: 21796 RVA: 0x001BE410 File Offset: 0x001BC610
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] Clone<T>(T[] source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.Length == 0)
		{
			return Array.Empty<T>();
		}
		T[] array = new T[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			array[i] = source[i];
		}
		return array;
	}

	// Token: 0x06005525 RID: 21797 RVA: 0x001BE452 File Offset: 0x001BC652
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static List<T> Clone<T>(List<T> source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.Count == 0)
		{
			return new List<T>();
		}
		return new List<T>(source);
	}

	// Token: 0x06005526 RID: 21798 RVA: 0x001BE470 File Offset: 0x001BC670
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfRef<T>(this T[] array, T value) where T : class
	{
		if (array == null || array.Length == 0)
		{
			return -1;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06005527 RID: 21799 RVA: 0x001BE4AC File Offset: 0x001BC6AC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfRef<T>(this List<T> list, T value) where T : class
	{
		if (list == null || list.Count == 0)
		{
			return -1;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06005528 RID: 21800 RVA: 0x001BE4F0 File Offset: 0x001BC6F0
	public static bool GTEnsureNoNulls<T>(ref T[] unityObjs) where T : Object
	{
		if (unityObjs == null)
		{
			unityObjs = Array.Empty<T>();
		}
		int num = 0;
		for (int i = 0; i < unityObjs.Length; i++)
		{
			if (!(unityObjs[i] == null))
			{
				unityObjs[num] = unityObjs[i];
				num++;
			}
		}
		bool flag = num != unityObjs.Length;
		if (flag)
		{
			Array.Resize<T>(ref unityObjs, num);
		}
		return flag;
	}
}
