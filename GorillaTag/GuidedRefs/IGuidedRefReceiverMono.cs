using System;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x0200126D RID: 4717
	public interface IGuidedRefReceiverMono : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x06007751 RID: 30545
		bool GuidedRefTryResolveReference(GuidedRefTryResolveInfo target);

		// Token: 0x17000B9D RID: 2973
		// (get) Token: 0x06007752 RID: 30546
		// (set) Token: 0x06007753 RID: 30547
		int GuidedRefsWaitingToResolveCount { get; set; }

		// Token: 0x06007754 RID: 30548
		void OnAllGuidedRefsResolved();

		// Token: 0x06007755 RID: 30549
		void OnGuidedRefTargetDestroyed(int fieldId);
	}
}
