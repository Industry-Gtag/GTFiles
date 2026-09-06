using System;
using UnityEngine;

// Token: 0x02000386 RID: 902
[Serializable]
public struct MaterialCombinerPerRendererInfo
{
	// Token: 0x04001AD5 RID: 6869
	public Renderer renderer;

	// Token: 0x04001AD6 RID: 6870
	public int slotIndex;

	// Token: 0x04001AD7 RID: 6871
	public int sliceIndex;

	// Token: 0x04001AD8 RID: 6872
	public Color baseColor;

	// Token: 0x04001AD9 RID: 6873
	public Material oldMat;

	// Token: 0x04001ADA RID: 6874
	public bool wasMeshCombined;
}
