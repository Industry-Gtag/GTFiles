using System;

namespace GorillaTag.CosmeticSystem.Editor
{
	// Token: 0x0200128E RID: 4750
	[Flags]
	public enum EEdCosBrowserAntiClippingFilter
	{
		// Token: 0x040087B6 RID: 34742
		None = 0,
		// Token: 0x040087B7 RID: 34743
		NameTag = 1,
		// Token: 0x040087B8 RID: 34744
		LeftArm = 2,
		// Token: 0x040087B9 RID: 34745
		RightArm = 4,
		// Token: 0x040087BA RID: 34746
		Chest = 8,
		// Token: 0x040087BB RID: 34747
		HuntComputer = 16,
		// Token: 0x040087BC RID: 34748
		Badge = 32,
		// Token: 0x040087BD RID: 34749
		BuilderWatch = 64,
		// Token: 0x040087BE RID: 34750
		FriendshipBraceletLeft = 128,
		// Token: 0x040087BF RID: 34751
		FriendshipBraceletRight = 256,
		// Token: 0x040087C0 RID: 34752
		All = 511
	}
}
