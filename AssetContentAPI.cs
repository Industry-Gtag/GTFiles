using System;
using UnityEngine;

// Token: 0x02000E10 RID: 3600
public class AssetContentAPI : ScriptableObject
{
	// Token: 0x04006874 RID: 26740
	public string bundleName;

	// Token: 0x04006875 RID: 26741
	public LazyLoadReference<TextAsset> bundleFile;

	// Token: 0x04006876 RID: 26742
	public Object[] assets = new Object[0];
}
