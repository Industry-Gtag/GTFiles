using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02001281 RID: 4737
	[Serializable]
	public struct CosmeticPart
	{
		// Token: 0x0400873E RID: 34622
		public GTAssetRef<GameObject> prefabAssetRef;

		// Token: 0x0400873F RID: 34623
		[Tooltip("Determines how the cosmetic part will be attached to the player.")]
		public CosmeticAttachInfo[] attachAnchors;

		// Token: 0x04008740 RID: 34624
		[NonSerialized]
		public ECosmeticPartType partType;
	}
}
