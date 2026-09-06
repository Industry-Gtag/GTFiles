using System;
using UnityEngine;

// Token: 0x020003BE RID: 958
[Serializable]
public class ZoneData
{
	// Token: 0x04002251 RID: 8785
	public GTZone zone;

	// Token: 0x04002252 RID: 8786
	public string sceneName;

	// Token: 0x04002253 RID: 8787
	public float CameraFarClipPlane = 500f;

	// Token: 0x04002254 RID: 8788
	public GameObject[] rootGameObjects;

	// Token: 0x04002255 RID: 8789
	[NonSerialized]
	public bool active;
}
