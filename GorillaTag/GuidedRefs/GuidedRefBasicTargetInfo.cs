using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001261 RID: 4705
	[Serializable]
	public struct GuidedRefBasicTargetInfo
	{
		// Token: 0x040086B8 RID: 34488
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x040086B9 RID: 34489
		[Tooltip("Used to filter down which relay the target can belong to. If null or empty then all parents with a GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO[] hubIds;

		// Token: 0x040086BA RID: 34490
		[DebugOption]
		[SerializeField]
		public bool hackIgnoreDuplicateRegistration;
	}
}
