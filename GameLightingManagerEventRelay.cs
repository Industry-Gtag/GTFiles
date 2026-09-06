using System;
using UnityEngine;

// Token: 0x020000A4 RID: 164
public class GameLightingManagerEventRelay : MonoBehaviour
{
	// Token: 0x06000408 RID: 1032 RVA: 0x00017F0B File Offset: 0x0001610B
	public void SetCustomDynamicLightingEnabled(bool value)
	{
		if (GameLightingManager.instance == null)
		{
			Debug.LogError("GameLightingManagerEventRelay :: GameLightingManager has not been instanced!");
			return;
		}
		GameLightingManager.instance.ZoneEnableCustomDynamicLighting(value);
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00017F34 File Offset: 0x00016134
	public void SetNearsightedDimLightIntensity(float value)
	{
		if (GameLightingManager.instance == null)
		{
			Debug.LogError("GameLightingManagerEventRelay :: GameLightingManager has not been instanced!");
			return;
		}
		GameLightingManager.instance.GR_NearsightedDimLight.intensity = value;
	}
}
