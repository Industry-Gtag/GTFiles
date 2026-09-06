using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x0200127E RID: 4734
	[Serializable]
	public struct CosmeticCollectionSlotDefinition
	{
		// Token: 0x04008711 RID: 34577
		[Tooltip("Position, rotation and scale of this slot relative to the parent cosmetic's root transform. Edit visually using the Cosmetic Editor Stage.")]
		public XformOffset offset;
	}
}
