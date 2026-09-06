using System;
using UnityEngine;

// Token: 0x02000041 RID: 65
[Serializable]
internal class SoundIdRemapping
{
	// Token: 0x1700001B RID: 27
	// (get) Token: 0x0600010E RID: 270 RVA: 0x00006639 File Offset: 0x00004839
	public int SoundIn
	{
		get
		{
			return this.soundIn;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x0600010F RID: 271 RVA: 0x00006641 File Offset: 0x00004841
	public int SoundOut
	{
		get
		{
			return this.soundOut;
		}
	}

	// Token: 0x0400011C RID: 284
	[GorillaSoundLookup]
	[SerializeField]
	private int soundIn = 1;

	// Token: 0x0400011D RID: 285
	[GorillaSoundLookup]
	[SerializeField]
	private int soundOut = 2;
}
