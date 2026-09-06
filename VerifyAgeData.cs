using System;

// Token: 0x02000B4D RID: 2893
public class VerifyAgeData
{
	// Token: 0x060049B2 RID: 18866 RVA: 0x00188DA8 File Offset: 0x00186FA8
	public VerifyAgeData(VerifyAgeResponse response)
	{
		if (response == null)
		{
			return;
		}
		this.Status = response.Status;
		if (response.Session == null && response.DefaultSession == null)
		{
			return;
		}
		this.Session = new TMPSession(response.Session, response.DefaultSession, this.Status);
	}

	// Token: 0x04005C0F RID: 23567
	public readonly SessionStatus Status;

	// Token: 0x04005C10 RID: 23568
	public readonly TMPSession Session;
}
