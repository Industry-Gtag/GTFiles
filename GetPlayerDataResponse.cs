using System;
using KID.Model;

// Token: 0x02000B41 RID: 2881
[Serializable]
public class GetPlayerDataResponse
{
	// Token: 0x04005BEA RID: 23530
	public SessionStatus? Status;

	// Token: 0x04005BEB RID: 23531
	public Session Session;

	// Token: 0x04005BEC RID: 23532
	public KIDDefaultSession DefaultSession;

	// Token: 0x04005BED RID: 23533
	public string[] Permissions;

	// Token: 0x04005BEE RID: 23534
	public bool HasConfirmedSetup;
}
