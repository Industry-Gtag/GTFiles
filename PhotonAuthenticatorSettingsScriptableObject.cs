using System;
using UnityEngine;

// Token: 0x02000CDA RID: 3290
[CreateAssetMenu(fileName = "PhotonAuthenticatorSettings", menuName = "ScriptableObjects/PhotonAuthenticatorSettings")]
public class PhotonAuthenticatorSettingsScriptableObject : ScriptableObject
{
	// Token: 0x0400637B RID: 25467
	public string PunAppId;

	// Token: 0x0400637C RID: 25468
	public string FusionAppId;

	// Token: 0x0400637D RID: 25469
	public string VoiceAppId;
}
