using System;
using UnityEngine;

// Token: 0x020001BF RID: 447
public class MetroManager : MonoBehaviour
{
	// Token: 0x06000BF9 RID: 3065 RVA: 0x00041804 File Offset: 0x0003FA04
	private void Update()
	{
		for (int i = 0; i < this._blimps.Length; i++)
		{
			this._blimps[i].Tick();
		}
		for (int j = 0; j < this._spotlights.Length; j++)
		{
			this._spotlights[j].Tick();
		}
	}

	// Token: 0x04000E94 RID: 3732
	[SerializeField]
	private MetroBlimp[] _blimps = new MetroBlimp[0];

	// Token: 0x04000E95 RID: 3733
	[SerializeField]
	private MetroSpotlight[] _spotlights = new MetroSpotlight[0];

	// Token: 0x04000E96 RID: 3734
	[Space]
	[SerializeField]
	private Transform _blimpsRotationAnchor;
}
