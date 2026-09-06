using System;
using UnityEngine.Serialization;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001271 RID: 4721
	public struct GuidedRefTryResolveInfo
	{
		// Token: 0x040086D9 RID: 34521
		public int fieldId;

		// Token: 0x040086DA RID: 34522
		public int index;

		// Token: 0x040086DB RID: 34523
		[FormerlySerializedAs("target")]
		public IGuidedRefTargetMono targetMono;
	}
}
