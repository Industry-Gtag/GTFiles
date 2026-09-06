using System;
using System.Collections.Generic;

// Token: 0x02000DB0 RID: 3504
public static class LinqUtils
{
	// Token: 0x060055F8 RID: 22008 RVA: 0x001C1EAB File Offset: 0x001C00AB
	public static IEnumerable<TResult> SelectManyNullSafe<TSource, TResult>(this IEnumerable<TSource> sources, Func<TSource, IEnumerable<TResult>> selector)
	{
		if (sources == null)
		{
			yield break;
		}
		if (selector == null)
		{
			yield break;
		}
		foreach (TSource tsource in sources)
		{
			if (tsource != null)
			{
				IEnumerable<TResult> enumerable = selector(tsource);
				foreach (TResult tresult in enumerable)
				{
					if (tresult != null)
					{
						yield return tresult;
					}
				}
				IEnumerator<TResult> enumerator2 = null;
			}
		}
		IEnumerator<TSource> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x060055F9 RID: 22009 RVA: 0x001C1EC2 File Offset: 0x001C00C2
	public static IEnumerable<TSource> DistinctBy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		HashSet<TResult> set = new HashSet<TResult>();
		foreach (TSource tsource in source)
		{
			TResult tresult = selector(tsource);
			if (set.Add(tresult))
			{
				yield return tsource;
			}
		}
		IEnumerator<TSource> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x060055FA RID: 22010 RVA: 0x001C1EDC File Offset: 0x001C00DC
	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (T t in source)
		{
			action(t);
		}
		return source;
	}

	// Token: 0x060055FB RID: 22011 RVA: 0x001C1F28 File Offset: 0x001C0128
	public static T[] AsArray<T>(this IEnumerable<T> source)
	{
		return (T[])source;
	}

	// Token: 0x060055FC RID: 22012 RVA: 0x001C1F30 File Offset: 0x001C0130
	public static List<T> AsList<T>(this IEnumerable<T> source)
	{
		return (List<T>)source;
	}

	// Token: 0x060055FD RID: 22013 RVA: 0x001C1F38 File Offset: 0x001C0138
	public static IList<T> Transform<T>(this IList<T> list, Func<T, T> action)
	{
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = action(list[i]);
		}
		return list;
	}

	// Token: 0x060055FE RID: 22014 RVA: 0x001C1F6B File Offset: 0x001C016B
	public static IEnumerable<T> Self<T>(this T value)
	{
		yield return value;
		yield break;
	}
}
