using System;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x0200127B RID: 4731
	[Serializable]
	public struct CosmeticAnchorAntiClipEntry
	{
		// Token: 0x04008708 RID: 34568
		public bool enabled;

		// Token: 0x04008709 RID: 34569
		public XformOffset offset;

		// Token: 0x0400870A RID: 34570
		public static readonly CosmeticAnchorAntiClipEntry Identity = new CosmeticAnchorAntiClipEntry
		{
			offset = XformOffset.Identity
		};
	}
}
