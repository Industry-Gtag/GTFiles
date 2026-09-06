using System;
using System.Collections.Generic;

namespace GorillaTag.GuidedRefs.Internal
{
	// Token: 0x02001272 RID: 4722
	public class RelayInfo
	{
		// Token: 0x040086DC RID: 34524
		[NonSerialized]
		public IGuidedRefTargetMono targetMono;

		// Token: 0x040086DD RID: 34525
		[NonSerialized]
		public List<RegisteredReceiverFieldInfo> registeredFields;

		// Token: 0x040086DE RID: 34526
		[NonSerialized]
		public List<RegisteredReceiverFieldInfo> resolvedFields;
	}
}
