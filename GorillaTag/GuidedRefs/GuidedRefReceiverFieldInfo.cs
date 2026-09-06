using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001268 RID: 4712
	[Serializable]
	public struct GuidedRefReceiverFieldInfo
	{
		// Token: 0x06007734 RID: 30516 RVA: 0x0026AB9C File Offset: 0x00268D9C
		public GuidedRefReceiverFieldInfo(bool useRecommendedDefaults)
		{
			this.resolveModes = (useRecommendedDefaults ? (GRef.EResolveModes.Runtime | GRef.EResolveModes.SceneProcessing) : GRef.EResolveModes.None);
			this.targetId = null;
			this.hubId = null;
			this.fieldId = 0;
		}

		// Token: 0x040086C9 RID: 34505
		[SerializeField]
		public GRef.EResolveModes resolveModes;

		// Token: 0x040086CA RID: 34506
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x040086CB RID: 34507
		[Tooltip("(Required) Used to filter down which relay the target can belong to. Only one GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO hubId;

		// Token: 0x040086CC RID: 34508
		[NonSerialized]
		public int fieldId;
	}
}
