using System;
using UnityEngine;

// Token: 0x02000D85 RID: 3461
public class BetterBaker : MonoBehaviour
{
	// Token: 0x040066D3 RID: 26323
	public string bakeryLightmapDirectory;

	// Token: 0x040066D4 RID: 26324
	public string dayNightLightmapsDirectory;

	// Token: 0x040066D5 RID: 26325
	public GameObject[] allLights;

	// Token: 0x02000D86 RID: 3462
	public struct LightMapMap
	{
		// Token: 0x040066D6 RID: 26326
		public string timeOfDayName;

		// Token: 0x040066D7 RID: 26327
		public GameObject lightObject;
	}
}
