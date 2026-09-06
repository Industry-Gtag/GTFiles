using System;
using KID.Model;

// Token: 0x02000B42 RID: 2882
[Serializable]
public class GetRequirementsResponse : GetAgeGateRequirementsResponse
{
	// Token: 0x1700070B RID: 1803
	// (get) Token: 0x06004992 RID: 18834 RVA: 0x001886DA File Offset: 0x001868DA
	// (set) Token: 0x06004993 RID: 18835 RVA: 0x001886E2 File Offset: 0x001868E2
	public int PlatformMinimumAge { get; set; }
}
