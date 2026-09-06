using System;
using UnityEngine;

// Token: 0x020000FC RID: 252
public class SIChargeDisplay : MonoBehaviour
{
	// Token: 0x060005F2 RID: 1522 RVA: 0x000226CC File Offset: 0x000208CC
	public void UpdateDisplay(int chargeCount)
	{
		for (int i = 0; i < this.chargeDisplay.Length; i++)
		{
			this.chargeDisplay[i].material = ((i < chargeCount) ? this.chargedMat : this.unchargedMat);
		}
	}

	// Token: 0x0400077C RID: 1916
	[SerializeField]
	private MeshRenderer[] chargeDisplay;

	// Token: 0x0400077D RID: 1917
	[SerializeField]
	private Material chargedMat;

	// Token: 0x0400077E RID: 1918
	[SerializeField]
	private Material unchargedMat;
}
