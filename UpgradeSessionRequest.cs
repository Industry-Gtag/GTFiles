using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x02000B45 RID: 2885
[Serializable]
public class UpgradeSessionRequest : KIDRequestData
{
	// Token: 0x04005BF3 RID: 23539
	public List<RequestedPermission> Permissions;
}
