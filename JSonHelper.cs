using System;
using UnityEngine;

// Token: 0x02000507 RID: 1287
public static class JSonHelper
{
	// Token: 0x06002048 RID: 8264 RVA: 0x000ADB64 File Offset: 0x000ABD64
	public static T[] FromJson<T>(string json)
	{
		return JsonUtility.FromJson<JSonHelper.Wrapper<T>>(json).Items;
	}

	// Token: 0x06002049 RID: 8265 RVA: 0x000ADB71 File Offset: 0x000ABD71
	public static string ToJson<T>(T[] array)
	{
		return JsonUtility.ToJson(new JSonHelper.Wrapper<T>
		{
			Items = array
		});
	}

	// Token: 0x0600204A RID: 8266 RVA: 0x000ADB84 File Offset: 0x000ABD84
	public static string ToJson<T>(T[] array, bool prettyPrint)
	{
		return JsonUtility.ToJson(new JSonHelper.Wrapper<T>
		{
			Items = array
		}, prettyPrint);
	}

	// Token: 0x02000508 RID: 1288
	[Serializable]
	private class Wrapper<T>
	{
		// Token: 0x04002B12 RID: 11026
		public T[] Items;
	}
}
