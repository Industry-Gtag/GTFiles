using System;
using UnityEngine;

// Token: 0x02000310 RID: 784
public static class GlobalDeactivatedSpawnRoot
{
	// Token: 0x060013E2 RID: 5090 RVA: 0x0006C3C8 File Offset: 0x0006A5C8
	public static Transform GetOrCreate()
	{
		if (!GlobalDeactivatedSpawnRoot._xform)
		{
			GlobalDeactivatedSpawnRoot._xform = new GameObject("GlobalDeactivatedSpawnRoot").transform;
			GlobalDeactivatedSpawnRoot._xform.gameObject.SetActive(false);
			Object.DontDestroyOnLoad(GlobalDeactivatedSpawnRoot._xform.gameObject);
		}
		GlobalDeactivatedSpawnRoot._xform.gameObject.SetActive(false);
		return GlobalDeactivatedSpawnRoot._xform;
	}

	// Token: 0x04001882 RID: 6274
	private static Transform _xform;
}
