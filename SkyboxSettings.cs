using System;
using UnityEngine;

// Token: 0x0200039D RID: 925
[ExecuteInEditMode]
public class SkyboxSettings : MonoBehaviour
{
	// Token: 0x0600167E RID: 5758 RVA: 0x0008297E File Offset: 0x00080B7E
	private void OnEnable()
	{
		if (this._skyMaterial)
		{
			RenderSettings.skybox = this._skyMaterial;
		}
	}

	// Token: 0x0400209D RID: 8349
	[SerializeField]
	private Material _skyMaterial;
}
