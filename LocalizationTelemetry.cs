using System;
using UnityEngine;

// Token: 0x02000C09 RID: 3081
public static class LocalizationTelemetry
{
	// Token: 0x17000766 RID: 1894
	// (get) Token: 0x06004D3B RID: 19771 RVA: 0x00039037 File Offset: 0x00037237
	public static string GameVersionCustomTag
	{
		get
		{
			return "game_version_" + Application.version;
		}
	}

	// Token: 0x04006093 RID: 24723
	public const string LANGUAGE_CHANGED_EVENT_NAME = "language_changed";

	// Token: 0x04006094 RID: 24724
	private const string GAME_VERSION_CUSTOM_TAG_PREFIX = "game_version_";

	// Token: 0x04006095 RID: 24725
	public const string STARTING_LANGUAGE_BODY_DATA = "starting_language";

	// Token: 0x04006096 RID: 24726
	public const string NEW_LANGUAGE_BODY_DATA = "new_language";
}
