using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x0200127F RID: 4735
	[Serializable]
	public struct CosmeticHoldableSlotAttachInfo
	{
		// Token: 0x04008712 RID: 34578
		[Tooltip("The anchor that this holdable cosmetic can attach to.")]
		public GTSturdyEnum<GTHardCodedBones.EHandAndStowSlots> stowSlot;

		// Token: 0x04008713 RID: 34579
		public XformOffset offset;
	}
}
