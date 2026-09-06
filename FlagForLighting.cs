using System;
using UnityEngine;

// Token: 0x02000DA8 RID: 3496
public class FlagForLighting : MonoBehaviour
{
	// Token: 0x04006746 RID: 26438
	public FlagForLighting.TimeOfDay myTimeOfDay;

	// Token: 0x02000DA9 RID: 3497
	public enum TimeOfDay
	{
		// Token: 0x04006748 RID: 26440
		Sunrise,
		// Token: 0x04006749 RID: 26441
		TenAM,
		// Token: 0x0400674A RID: 26442
		Noon,
		// Token: 0x0400674B RID: 26443
		ThreePM,
		// Token: 0x0400674C RID: 26444
		Sunset,
		// Token: 0x0400674D RID: 26445
		Night,
		// Token: 0x0400674E RID: 26446
		RainingDay,
		// Token: 0x0400674F RID: 26447
		RainingNight,
		// Token: 0x04006750 RID: 26448
		None
	}
}
