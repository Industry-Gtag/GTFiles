using System;
using UnityEngine.Serialization;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001270 RID: 4720
	public struct RegisteredReceiverFieldInfo
	{
		// Token: 0x040086D6 RID: 34518
		[FormerlySerializedAs("receiver")]
		public IGuidedRefReceiverMono receiverMono;

		// Token: 0x040086D7 RID: 34519
		public int fieldId;

		// Token: 0x040086D8 RID: 34520
		public int index;
	}
}
