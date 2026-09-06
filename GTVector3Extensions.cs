using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000336 RID: 822
public static class GTVector3Extensions
{
	// Token: 0x06001455 RID: 5205 RVA: 0x0006DAA2 File Offset: 0x0006BCA2
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 X_Z(this Vector3 vector)
	{
		return new Vector3(vector.x, 0f, vector.z);
	}

	// Token: 0x06001456 RID: 5206 RVA: 0x0006DABC File Offset: 0x0006BCBC
	public static Vector3 Sum(this IList<Vector3> vecs)
	{
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < vecs.Count; i++)
		{
			vector += vecs[i];
		}
		return vector;
	}

	// Token: 0x06001457 RID: 5207 RVA: 0x0006DAF0 File Offset: 0x0006BCF0
	public static Vector3 Average(this IList<Vector3> vecs)
	{
		int count = vecs.Count;
		if (count == 0)
		{
			return Vector3.zero;
		}
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < count; i++)
		{
			vector += vecs[i];
		}
		return vector / (float)count;
	}

	// Token: 0x06001458 RID: 5208 RVA: 0x0006DB38 File Offset: 0x0006BD38
	public static Vector3 Sum(this IEnumerable<Vector3> vecs)
	{
		Vector3 vector = Vector3.zero;
		foreach (Vector3 vector2 in vecs)
		{
			vector += vector2;
		}
		return vector;
	}

	// Token: 0x06001459 RID: 5209 RVA: 0x0006DB88 File Offset: 0x0006BD88
	public static Vector3 Average(this IEnumerable<Vector3> vecs)
	{
		Vector3 vector = Vector3.zero;
		int num = 0;
		foreach (Vector3 vector2 in vecs)
		{
			vector += vector2;
			num++;
		}
		if (num == 0)
		{
			return Vector3.zero;
		}
		return vector / (float)num;
	}
}
