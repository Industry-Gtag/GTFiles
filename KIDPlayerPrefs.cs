using System;

// Token: 0x02000B8C RID: 2956
public class KIDPlayerPrefs
{
	// Token: 0x04005D76 RID: 23926
	public const string SESSION_ID_PREFIX_PLAYER_PREF = "kIDSessionID-";

	// Token: 0x04005D77 RID: 23927
	public const string SESSION_ETAG_PLAYER_PREF = "kIDSessionETAG-";

	// Token: 0x04005D78 RID: 23928
	public const string SESSION_CHANGED_PLAYER_PREF = "kIDSessionUpdated-";

	// Token: 0x04005D79 RID: 23929
	private const string KID_PERMISSIONS_CSV = "kid-permission-csv";

	// Token: 0x04005D7A RID: 23930
	private const string KID_DEFAULT_PERMISSIONS_CSV = "kid-default-permission-csv";

	// Token: 0x04005D7B RID: 23931
	private const string KID_PERMISSIONS_ENABLED_KEY = "-enabled";

	// Token: 0x04005D7C RID: 23932
	private const string KID_PERMISSIONS_MANAGED_BY_KEY = "-managed-by";

	// Token: 0x04005D7D RID: 23933
	private const string KID_EMAIL_KEY = "k-id_EmailAddress";
}
