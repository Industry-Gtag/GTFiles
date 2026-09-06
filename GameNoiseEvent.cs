using System;
using UnityEngine;

// Token: 0x020007C8 RID: 1992
public struct GameNoiseEvent
{
	// Token: 0x060032C4 RID: 12996 RVA: 0x00116332 File Offset: 0x00114532
	public bool IsValid()
	{
		return (float)(Time.timeAsDouble - this.eventTime) <= this.duration;
	}

	// Token: 0x040041D4 RID: 16852
	public Vector3 position;

	// Token: 0x040041D5 RID: 16853
	public double eventTime;

	// Token: 0x040041D6 RID: 16854
	public float duration;

	// Token: 0x040041D7 RID: 16855
	public float magnitude;
}
