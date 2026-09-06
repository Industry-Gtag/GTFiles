using System;
using UnityEngine;

// Token: 0x02000CDB RID: 3291
public class PlayFabAuthenticatorSettings
{
	// Token: 0x0600517D RID: 20861 RVA: 0x001AFB74 File Offset: 0x001ADD74
	static PlayFabAuthenticatorSettings()
	{
		PlayFabAuthenticatorSettings.Load("PlayFabAuthenticatorSettings");
	}

	// Token: 0x0600517E RID: 20862 RVA: 0x001AFB80 File Offset: 0x001ADD80
	public static void Load(string path)
	{
		PlayFabAuthenticatorSettingsScriptableObject playFabAuthenticatorSettingsScriptableObject = Resources.Load<PlayFabAuthenticatorSettingsScriptableObject>(path);
		PlayFabAuthenticatorSettings.TitleId = playFabAuthenticatorSettingsScriptableObject.TitleId;
		PlayFabAuthenticatorSettings.AuthApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.AuthApiBaseUrl;
		PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.DailyQuestsApiBaseUrl;
		PlayFabAuthenticatorSettings.FriendApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.FriendApiBaseUrl;
		PlayFabAuthenticatorSettings.HpPromoApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.HpPromoApiBaseUrl;
		PlayFabAuthenticatorSettings.IapApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.IapApiBaseUrl;
		PlayFabAuthenticatorSettings.KidApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.KidApiBaseUrl;
		PlayFabAuthenticatorSettings.MmrApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.MmrApiBaseUrl;
		PlayFabAuthenticatorSettings.ModerationApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.ModerationApiBaseUrl;
		PlayFabAuthenticatorSettings.ProgressionApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.ProgressionApiBaseUrl;
		PlayFabAuthenticatorSettings.TitleDataApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.TitleDataApiBaseUrl;
		PlayFabAuthenticatorSettings.VotingApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.VotingApiBaseUrl;
	}

	// Token: 0x0400637E RID: 25470
	public static string TitleId;

	// Token: 0x0400637F RID: 25471
	public static string AuthApiBaseUrl;

	// Token: 0x04006380 RID: 25472
	public static string DailyQuestsApiBaseUrl;

	// Token: 0x04006381 RID: 25473
	public static string FriendApiBaseUrl;

	// Token: 0x04006382 RID: 25474
	public static string HpPromoApiBaseUrl;

	// Token: 0x04006383 RID: 25475
	public static string IapApiBaseUrl;

	// Token: 0x04006384 RID: 25476
	public static string KidApiBaseUrl;

	// Token: 0x04006385 RID: 25477
	public static string MmrApiBaseUrl;

	// Token: 0x04006386 RID: 25478
	public static string ModerationApiBaseUrl;

	// Token: 0x04006387 RID: 25479
	public static string ProgressionApiBaseUrl;

	// Token: 0x04006388 RID: 25480
	public static string TitleDataApiBaseUrl;

	// Token: 0x04006389 RID: 25481
	public static string VotingApiBaseUrl;
}
