using System;
using System.Diagnostics;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x0200108F RID: 4239
	public static class AssetDatabase
	{
		// Token: 0x060069D2 RID: 27090 RVA: 0x00220D88 File Offset: 0x0021EF88
		public static T LoadAssetAtPath<T>(string assetPath) where T : Object
		{
			return default(T);
		}

		// Token: 0x060069D3 RID: 27091 RVA: 0x001BE56B File Offset: 0x001BC76B
		public static T[] LoadAssetsOfType<T>() where T : Object
		{
			return Array.Empty<T>();
		}

		// Token: 0x060069D4 RID: 27092 RVA: 0x00220D9E File Offset: 0x0021EF9E
		public static string[] FindAssetsOfType<T>() where T : Object
		{
			return Array.Empty<string>();
		}

		// Token: 0x060069D5 RID: 27093 RVA: 0x00220DA5 File Offset: 0x0021EFA5
		[Conditional("UNITY_EDITOR")]
		public static void SaveToDisk(params Object[] assetsToSave)
		{
			AssetDatabase.SaveAssetsToDisk(assetsToSave, true);
		}

		// Token: 0x060069D6 RID: 27094 RVA: 0x00002C2D File Offset: 0x00000E2D
		public static void SaveAssetsToDisk(Object[] assetsToSave, bool saveProject = true)
		{
		}
	}
}
