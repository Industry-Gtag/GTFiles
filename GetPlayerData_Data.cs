using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000B2C RID: 2860
public class GetPlayerData_Data
{
	// Token: 0x06004961 RID: 18785 RVA: 0x001884DC File Offset: 0x001866DC
	public GetPlayerData_Data(GetSessionResponseType type, GetPlayerDataResponse response)
	{
		this.responseType = type;
		if (response == null)
		{
			if (this.responseType == GetSessionResponseType.OK)
			{
				this.responseType = GetSessionResponseType.ERROR;
				Debug.LogError("[KID::GET_PLAYER_DATA_DATA] Incoming [GetPlayerDataResponse] is NULL");
			}
			return;
		}
		this.status = response.Status;
		if (this.status != null)
		{
			this.session = new TMPSession(response.Session, response.DefaultSession, this.status.Value);
			this.session.SetOptInPermissions(response.Permissions);
			Debug.Log("[KID::GET_PLAYER_DATA_DATA::OptInRefactor] Setting Opt-in Permissions: " + string.Join(", ", this.session.GetOptedInPermissions()));
		}
		this.HasConfirmedSetup = response.HasConfirmedSetup;
	}

	// Token: 0x04005BA5 RID: 23461
	public readonly GetSessionResponseType responseType;

	// Token: 0x04005BA6 RID: 23462
	public readonly SessionStatus? status;

	// Token: 0x04005BA7 RID: 23463
	public readonly TMPSession session;

	// Token: 0x04005BA8 RID: 23464
	[Nullable(new byte[] { 2, 0 })]
	public readonly string[] OptInPermissions;

	// Token: 0x04005BA9 RID: 23465
	public readonly bool HasConfirmedSetup;
}
