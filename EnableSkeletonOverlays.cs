using System;
using UnityEngine;

// Token: 0x02000332 RID: 818
public class EnableSkeletonOverlays : MonoBehaviour
{
	// Token: 0x06001442 RID: 5186 RVA: 0x0006D896 File Offset: 0x0006BA96
	private void OnEnable()
	{
		Shader.SetGlobalFloat(this._BlackAndWhite, 1f);
		GorillaBodyRenderer.EnableSkeletonOverlays(this.bodyMaterial, this.skeletonMaterial);
	}

	// Token: 0x06001443 RID: 5187 RVA: 0x0006D8BE File Offset: 0x0006BABE
	private void OnDisable()
	{
		Shader.SetGlobalFloat(this._BlackAndWhite, 0f);
		GorillaBodyRenderer.DisableSkeletonOverlays();
	}

	// Token: 0x0400191B RID: 6427
	[SerializeField]
	private Material bodyMaterial;

	// Token: 0x0400191C RID: 6428
	[SerializeField]
	private Material skeletonMaterial;

	// Token: 0x0400191D RID: 6429
	private ShaderHashId _BlackAndWhite = "_GreyZoneActive";
}
