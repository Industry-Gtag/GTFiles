using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02001283 RID: 4739
	[Serializable]
	public struct CosmeticPlacementInfo
	{
		// Token: 0x04008743 RID: 34627
		[Tooltip("The bone to attach the cosmetic to.")]
		public GTHardCodedBones.SturdyEBone parentBone;

		// Token: 0x04008744 RID: 34628
		public XformOffset offset;
	}
}
