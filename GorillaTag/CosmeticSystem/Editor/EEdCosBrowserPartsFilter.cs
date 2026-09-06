using System;

namespace GorillaTag.CosmeticSystem.Editor
{
	// Token: 0x02001290 RID: 4752
	[Flags]
	public enum EEdCosBrowserPartsFilter
	{
		// Token: 0x040087D1 RID: 34769
		None = 0,
		// Token: 0x040087D2 RID: 34770
		NoParts = 1,
		// Token: 0x040087D3 RID: 34771
		Holdable = 2,
		// Token: 0x040087D4 RID: 34772
		Functional = 4,
		// Token: 0x040087D5 RID: 34773
		Wardrobe = 8,
		// Token: 0x040087D6 RID: 34774
		Store = 16,
		// Token: 0x040087D7 RID: 34775
		FirstPerson = 32,
		// Token: 0x040087D8 RID: 34776
		LocalRig = 64,
		// Token: 0x040087D9 RID: 34777
		All = 127
	}
}
