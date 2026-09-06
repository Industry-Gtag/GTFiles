using System;
using UnityEngine;

// Token: 0x020001A4 RID: 420
[ExecuteAlways]
public class WaterSurfaceMaterialController : MonoBehaviour
{
	// Token: 0x06000B4E RID: 2894 RVA: 0x0003C779 File Offset: 0x0003A979
	protected void OnEnable()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.matPropBlock = new MaterialPropertyBlock();
		this.ApplyProperties();
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x0003C798 File Offset: 0x0003A998
	private void ApplyProperties()
	{
		this.matPropBlock.SetVector(ShaderProps._ScrollSpeedAndScale, new Vector4(this.ScrollX, this.ScrollY, this.Scale, 0f));
		if (this.renderer)
		{
			this.renderer.SetPropertyBlock(this.matPropBlock);
		}
	}

	// Token: 0x04000D96 RID: 3478
	public float ScrollX = 0.6f;

	// Token: 0x04000D97 RID: 3479
	public float ScrollY = 0.6f;

	// Token: 0x04000D98 RID: 3480
	public float Scale = 1f;

	// Token: 0x04000D99 RID: 3481
	private Renderer renderer;

	// Token: 0x04000D9A RID: 3482
	private MaterialPropertyBlock matPropBlock;
}
