using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x0200126F RID: 4719
	[Serializable]
	public struct GuidedRefReceiverArrayInfo
	{
		// Token: 0x06007759 RID: 30553 RVA: 0x0026AC52 File Offset: 0x00268E52
		public GuidedRefReceiverArrayInfo(bool useRecommendedDefaults)
		{
			this.resolveModes = (useRecommendedDefaults ? (GRef.EResolveModes.Runtime | GRef.EResolveModes.SceneProcessing) : GRef.EResolveModes.None);
			this.targets = Array.Empty<GuidedRefTargetIdSO>();
			this.hubId = null;
			this.fieldId = 0;
			this.resolveCount = 0;
		}

		// Token: 0x040086D1 RID: 34513
		[Tooltip("Controls whether the array should be overridden by the guided refs.")]
		[SerializeField]
		public GRef.EResolveModes resolveModes;

		// Token: 0x040086D2 RID: 34514
		[Tooltip("(Required) Used to filter down which relay the target can belong to. Only one GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO hubId;

		// Token: 0x040086D3 RID: 34515
		[SerializeField]
		public GuidedRefTargetIdSO[] targets;

		// Token: 0x040086D4 RID: 34516
		[NonSerialized]
		public int fieldId;

		// Token: 0x040086D5 RID: 34517
		[NonSerialized]
		public int resolveCount;
	}
}
