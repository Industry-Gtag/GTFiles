using System;
using UnityEngine;

// Token: 0x020005E3 RID: 1507
public class GorillaSurfaceOverride : MonoBehaviour
{
	// Token: 0x04003152 RID: 12626
	[GorillaSoundLookup]
	public int overrideIndex;

	// Token: 0x04003153 RID: 12627
	public float extraVelMultiplier = 1f;

	// Token: 0x04003154 RID: 12628
	public float extraVelMaxMultiplier = 1f;

	// Token: 0x04003155 RID: 12629
	[HideInInspector]
	[NonSerialized]
	public float slidePercentageOverride = -1f;

	// Token: 0x04003156 RID: 12630
	public bool sendOnTapEvent;

	// Token: 0x04003157 RID: 12631
	public bool disablePushBackEffect;
}
