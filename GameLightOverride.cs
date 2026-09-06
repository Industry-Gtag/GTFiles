using System;
using UnityEngine;

// Token: 0x020006EF RID: 1775
public class GameLightOverride : MonoBehaviour
{
	// Token: 0x06002CB1 RID: 11441 RVA: 0x000F166F File Offset: 0x000EF86F
	public void MaxGameLightOverride(int newMaxLights)
	{
		GameLightingManager.instance.SetMaxLights(newMaxLights);
	}

	// Token: 0x06002CB2 RID: 11442 RVA: 0x000F167E File Offset: 0x000EF87E
	private void OnDisable()
	{
		GameLightingManager.instance.SetMaxLights(20);
	}
}
