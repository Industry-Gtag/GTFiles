using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000D7E RID: 3454
public static class AssetUtils
{
	// Token: 0x06005529 RID: 21801 RVA: 0x00002C2D File Offset: 0x00000E2D
	[Conditional("UNITY_EDITOR")]
	public static void ExecAndUnloadUnused(Action action)
	{
	}

	// Token: 0x0600552A RID: 21802 RVA: 0x001BE556 File Offset: 0x001BC756
	[Conditional("UNITY_EDITOR")]
	public static void LoadAssetOfType<T>(ref T result, ref string resultPath) where T : Object
	{
		result = default(T);
		resultPath = null;
	}

	// Token: 0x0600552B RID: 21803 RVA: 0x001BE562 File Offset: 0x001BC762
	[Conditional("UNITY_EDITOR")]
	public static void FindAllAssetsOfType<T>(ref T[] results, ref string[] assetPaths) where T : Object
	{
		results = Array.Empty<T>();
	}

	// Token: 0x0600552C RID: 21804 RVA: 0x001BE56B File Offset: 0x001BC76B
	public static T[] FindAllAssetsOfType<T>() where T : Object
	{
		return Array.Empty<T>();
	}

	// Token: 0x0600552D RID: 21805 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void ForceSave<T>(this IList<T> assets, Action<T> onPreSave = null, bool unloadUnusedAfter = false) where T : Object
	{
	}

	// Token: 0x0600552E RID: 21806 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void ForceSave(this Object asset)
	{
	}

	// Token: 0x0600552F RID: 21807 RVA: 0x001BE572 File Offset: 0x001BC772
	public static long ComputeAssetId(this Object asset, bool unsigned = false)
	{
		return 0L;
	}

	// Token: 0x06005530 RID: 21808 RVA: 0x001BE578 File Offset: 0x001BC778
	public static string GetGameObjectPath(GameObject obj)
	{
		string text = "/" + obj.name;
		while (obj.transform.parent != null)
		{
			obj = obj.transform.parent.gameObject;
			text = "/" + obj.name + text;
		}
		return text;
	}
}
