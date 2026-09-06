using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000843 RID: 2115
public class GiantSnowflakeAudio : MonoBehaviour
{
	// Token: 0x06003655 RID: 13909 RVA: 0x0012C5AC File Offset: 0x0012A7AC
	private void Start()
	{
		foreach (GiantSnowflakeAudio.SnowflakeScaleOverride snowflakeScaleOverride in this.audioOverrides)
		{
			if (base.transform.lossyScale.x < snowflakeScaleOverride.scaleMax)
			{
				base.GetComponent<GorillaSurfaceOverride>().overrideIndex = snowflakeScaleOverride.newOverrideIndex;
			}
		}
	}

	// Token: 0x0400471A RID: 18202
	public List<GiantSnowflakeAudio.SnowflakeScaleOverride> audioOverrides;

	// Token: 0x02000844 RID: 2116
	[Serializable]
	public struct SnowflakeScaleOverride
	{
		// Token: 0x0400471B RID: 18203
		public float scaleMax;

		// Token: 0x0400471C RID: 18204
		[GorillaSoundLookup]
		public int newOverrideIndex;
	}
}
