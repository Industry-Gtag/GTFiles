using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

// Token: 0x020001EA RID: 490
public static class NativeArrayPool<T> where T : struct
{
	// Token: 0x06000CD3 RID: 3283 RVA: 0x00046C3F File Offset: 0x00044E3F
	static NativeArrayPool()
	{
		Application.quitting += NativeArrayPool<T>.OnQuit;
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x00046C5C File Offset: 0x00044E5C
	private static void OnQuit()
	{
		NativeArrayPool<T>.Dispose();
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x00046C64 File Offset: 0x00044E64
	[OnEnterPlay_Run]
	public static void Dispose()
	{
		if (NativeArrayPool<T>._lookup == null)
		{
			return;
		}
		foreach (Stack<NativeArray<T>> stack in NativeArrayPool<T>._lookup.Values)
		{
			foreach (NativeArray<T> nativeArray in stack)
			{
				nativeArray.Dispose();
			}
		}
		NativeArrayPool<T>._lookup.Clear();
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x00046D04 File Offset: 0x00044F04
	public static NativeArray<T> Get(int length)
	{
		NativeArray<T> nativeArray;
		if (!NativeArrayPool<T>.GetCollectionForLength(length).TryPop(out nativeArray))
		{
			nativeArray = new NativeArray<T>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		}
		return nativeArray;
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x00046D2B File Offset: 0x00044F2B
	public static void Return(NativeArray<T> item)
	{
		NativeArrayPool<T>.GetCollectionForLength(item.Length).Push(item);
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x00046D40 File Offset: 0x00044F40
	private static Stack<NativeArray<T>> GetCollectionForLength(int length)
	{
		Stack<NativeArray<T>> stack;
		if (!NativeArrayPool<T>._lookup.TryGetValue(length, out stack))
		{
			stack = new Stack<NativeArray<T>>();
			NativeArrayPool<T>._lookup.Add(length, stack);
		}
		return stack;
	}

	// Token: 0x04000F8A RID: 3978
	private static Dictionary<int, Stack<NativeArray<T>>> _lookup = new Dictionary<int, Stack<NativeArray<T>>>();
}
