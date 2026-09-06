using System;
using UnityEngine;

// Token: 0x0200092B RID: 2347
[Serializable]
public class SizeLayerMask
{
	// Token: 0x170005A9 RID: 1449
	// (get) Token: 0x06003D7E RID: 15742 RVA: 0x0014DA68 File Offset: 0x0014BC68
	public int Mask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x04004E42 RID: 20034
	[SerializeField]
	private bool affectLayerA = true;

	// Token: 0x04004E43 RID: 20035
	[SerializeField]
	private bool affectLayerB = true;

	// Token: 0x04004E44 RID: 20036
	[SerializeField]
	private bool affectLayerC = true;

	// Token: 0x04004E45 RID: 20037
	[SerializeField]
	private bool affectLayerD = true;
}
