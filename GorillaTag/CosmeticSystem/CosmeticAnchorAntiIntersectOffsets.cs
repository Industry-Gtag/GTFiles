using System;
using UnityEngine.Serialization;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02001279 RID: 4729
	[Serializable]
	public struct CosmeticAnchorAntiIntersectOffsets
	{
		// Token: 0x040086F4 RID: 34548
		public CosmeticAnchorAntiClipEntry nameTag;

		// Token: 0x040086F5 RID: 34549
		public CosmeticAnchorAntiClipEntry leftArm;

		// Token: 0x040086F6 RID: 34550
		public CosmeticAnchorAntiClipEntry rightArm;

		// Token: 0x040086F7 RID: 34551
		public CosmeticAnchorAntiClipEntry chest;

		// Token: 0x040086F8 RID: 34552
		public CosmeticAnchorAntiClipEntry huntComputer;

		// Token: 0x040086F9 RID: 34553
		public CosmeticAnchorAntiClipEntry badge;

		// Token: 0x040086FA RID: 34554
		public CosmeticAnchorAntiClipEntry builderWatch;

		// Token: 0x040086FB RID: 34555
		public CosmeticAnchorAntiClipEntry friendshipBraceletLeft;

		// Token: 0x040086FC RID: 34556
		[FormerlySerializedAs("friendshipBradceletRight")]
		public CosmeticAnchorAntiClipEntry friendshipBraceletRight;

		// Token: 0x040086FD RID: 34557
		public static readonly CosmeticAnchorAntiIntersectOffsets Identity = new CosmeticAnchorAntiIntersectOffsets
		{
			nameTag = CosmeticAnchorAntiClipEntry.Identity,
			leftArm = CosmeticAnchorAntiClipEntry.Identity,
			rightArm = CosmeticAnchorAntiClipEntry.Identity,
			chest = CosmeticAnchorAntiClipEntry.Identity,
			huntComputer = CosmeticAnchorAntiClipEntry.Identity,
			badge = CosmeticAnchorAntiClipEntry.Identity,
			builderWatch = CosmeticAnchorAntiClipEntry.Identity
		};
	}
}
