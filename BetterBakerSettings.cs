using System;
using UnityEngine;

// Token: 0x02000D8A RID: 3466
public class BetterBakerSettings : MonoBehaviour
{
	// Token: 0x040066DF RID: 26335
	[SerializeField]
	public GameObject[] lightMapMaps = new GameObject[9];

	// Token: 0x02000D8B RID: 3467
	[Serializable]
	public struct LightMapMap
	{
		// Token: 0x040066E0 RID: 26336
		[SerializeField]
		public string timeOfDayName;

		// Token: 0x040066E1 RID: 26337
		[SerializeField]
		public GameObject sceneLightObject;
	}
}
