using System;

// Token: 0x02000B4C RID: 2892
public class UpgradeSessionData
{
	// Token: 0x060049B1 RID: 18865 RVA: 0x00188D7C File Offset: 0x00186F7C
	public UpgradeSessionData(UpgradeSessionResponse response)
	{
		this.status = response.status;
		this.session = new TMPSession(response.session, null, this.status);
	}

	// Token: 0x04005C0D RID: 23565
	public readonly SessionStatus status;

	// Token: 0x04005C0E RID: 23566
	public readonly TMPSession session;
}
