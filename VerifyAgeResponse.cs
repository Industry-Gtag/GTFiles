using System;
using System.Runtime.CompilerServices;
using KID.Model;

// Token: 0x02000B48 RID: 2888
public class VerifyAgeResponse
{
	// Token: 0x1700070C RID: 1804
	// (get) Token: 0x0600499A RID: 18842 RVA: 0x001886F3 File Offset: 0x001868F3
	// (set) Token: 0x0600499B RID: 18843 RVA: 0x001886FB File Offset: 0x001868FB
	public SessionStatus Status { get; set; }

	// Token: 0x1700070D RID: 1805
	// (get) Token: 0x0600499C RID: 18844 RVA: 0x00188704 File Offset: 0x00186904
	// (set) Token: 0x0600499D RID: 18845 RVA: 0x0018870C File Offset: 0x0018690C
	[Nullable(2)]
	public Session Session
	{
		[NullableContext(2)]
		get;
		[NullableContext(2)]
		set;
	}

	// Token: 0x1700070E RID: 1806
	// (get) Token: 0x0600499E RID: 18846 RVA: 0x00188715 File Offset: 0x00186915
	// (set) Token: 0x0600499F RID: 18847 RVA: 0x0018871D File Offset: 0x0018691D
	public KIDDefaultSession DefaultSession { get; set; }
}
