using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02001282 RID: 4738
	[Serializable]
	public struct CosmeticPartMirrorOption
	{
		// Token: 0x04008741 RID: 34625
		public ECosmeticPartMirrorAxis axis;

		// Token: 0x04008742 RID: 34626
		[Tooltip("This will multiply the local scale for the selected axis by -1.")]
		public bool negativeScale;
	}
}
