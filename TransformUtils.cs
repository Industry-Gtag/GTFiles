using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000DEF RID: 3567
public static class TransformUtils
{
	// Token: 0x06005776 RID: 22390 RVA: 0x001C8D78 File Offset: 0x001C6F78
	public static int ComputePathHashByInstance(Transform t)
	{
		if (t == null)
		{
			return 0;
		}
		int num = 0;
		Transform transform = t;
		while (transform != null)
		{
			num = StaticHash.Compute(num, transform.GetHashCode());
			transform = transform.parent;
		}
		return num;
	}

	// Token: 0x06005777 RID: 22391 RVA: 0x001C8DB4 File Offset: 0x001C6FB4
	public static Hash128 ComputePathHash(Transform t)
	{
		if (t == null)
		{
			return default(Hash128);
		}
		Hash128 hash = default(Hash128);
		Transform transform = t;
		while (transform != null)
		{
			Hash128 hash2 = Hash128.Compute(transform.name);
			HashUtilities.AppendHash(ref hash2, ref hash);
			transform = transform.parent;
		}
		return hash;
	}

	// Token: 0x06005778 RID: 22392 RVA: 0x001C8E08 File Offset: 0x001C7008
	public static string GetScenePath(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		string text = t.name;
		Transform transform = t.parent;
		while (transform != null)
		{
			text = transform.name + "/" + text;
			transform = transform.parent;
		}
		return text;
	}

	// Token: 0x06005779 RID: 22393 RVA: 0x001C8E54 File Offset: 0x001C7054
	public static string GetScenePathReverse(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		string text = t.name;
		Transform transform = t.parent;
		Queue<string> queue = new Queue<string>(16);
		while (transform != null)
		{
			queue.Enqueue(transform.name);
			transform = transform.parent;
		}
		while (queue.Count > 0)
		{
			text = text + "/" + queue.Dequeue();
		}
		return text;
	}

	// Token: 0x0400680F RID: 26639
	private const string kFwdSlash = "/";
}
