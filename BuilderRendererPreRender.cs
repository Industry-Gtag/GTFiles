using System;

// Token: 0x02000620 RID: 1568
public class BuilderRendererPreRender : MonoBehaviourPostTick
{
	// Token: 0x0600271F RID: 10015 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Awake()
	{
	}

	// Token: 0x06002720 RID: 10016 RVA: 0x000CF035 File Offset: 0x000CD235
	public override void PostTick()
	{
		if (this.builderRenderer != null)
		{
			this.builderRenderer.PreRenderIndirect();
		}
	}

	// Token: 0x040032A6 RID: 12966
	public BuilderRenderer builderRenderer;
}
