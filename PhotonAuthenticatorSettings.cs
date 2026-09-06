using System;
using UnityEngine;

// Token: 0x02000CD9 RID: 3289
public class PhotonAuthenticatorSettings
{
	// Token: 0x06005179 RID: 20857 RVA: 0x001AFB40 File Offset: 0x001ADD40
	static PhotonAuthenticatorSettings()
	{
		PhotonAuthenticatorSettings.Load("PhotonAuthenticatorSettings");
	}

	// Token: 0x0600517A RID: 20858 RVA: 0x001AFB4C File Offset: 0x001ADD4C
	public static void Load(string path)
	{
		PhotonAuthenticatorSettingsScriptableObject photonAuthenticatorSettingsScriptableObject = Resources.Load<PhotonAuthenticatorSettingsScriptableObject>(path);
		PhotonAuthenticatorSettings.PunAppId = photonAuthenticatorSettingsScriptableObject.PunAppId;
		PhotonAuthenticatorSettings.FusionAppId = photonAuthenticatorSettingsScriptableObject.FusionAppId;
		PhotonAuthenticatorSettings.VoiceAppId = photonAuthenticatorSettingsScriptableObject.VoiceAppId;
	}

	// Token: 0x04006378 RID: 25464
	public static string PunAppId;

	// Token: 0x04006379 RID: 25465
	public static string FusionAppId;

	// Token: 0x0400637A RID: 25466
	public static string VoiceAppId;
}
