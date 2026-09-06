using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x0200126E RID: 4718
	public interface IGuidedRefTargetMono : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000B9E RID: 2974
		// (get) Token: 0x06007756 RID: 30550
		// (set) Token: 0x06007757 RID: 30551
		GuidedRefBasicTargetInfo GRefTargetInfo { get; set; }

		// Token: 0x17000B9F RID: 2975
		// (get) Token: 0x06007758 RID: 30552
		Object GuidedRefTargetObject { get; }
	}
}
