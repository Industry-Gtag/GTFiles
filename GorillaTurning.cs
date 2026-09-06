using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020008E0 RID: 2272
public class GorillaTurning : GorillaTriggerBox
{
	// Token: 0x06003B92 RID: 15250 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Awake()
	{
	}

	// Token: 0x04004C24 RID: 19492
	public Material redMaterial;

	// Token: 0x04004C25 RID: 19493
	public Material blueMaterial;

	// Token: 0x04004C26 RID: 19494
	public Material greenMaterial;

	// Token: 0x04004C27 RID: 19495
	public Material transparentBlueMaterial;

	// Token: 0x04004C28 RID: 19496
	public Material transparentRedMaterial;

	// Token: 0x04004C29 RID: 19497
	public Material transparentGreenMaterial;

	// Token: 0x04004C2A RID: 19498
	public MeshRenderer smoothTurnBox;

	// Token: 0x04004C2B RID: 19499
	public MeshRenderer snapTurnBox;

	// Token: 0x04004C2C RID: 19500
	public MeshRenderer noTurnBox;

	// Token: 0x04004C2D RID: 19501
	public GorillaSnapTurn snapTurn;

	// Token: 0x04004C2E RID: 19502
	public string currentChoice;

	// Token: 0x04004C2F RID: 19503
	public float currentSpeed;
}
