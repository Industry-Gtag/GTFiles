using System;
using UnityEngine;

// Token: 0x0200062B RID: 1579
public class BuilderBumpGlow : MonoBehaviour
{
	// Token: 0x06002744 RID: 10052 RVA: 0x000CFA7C File Offset: 0x000CDC7C
	public void Awake()
	{
		this.blendIn = 1f;
		this.intensity = 0f;
		this.UpdateRender();
	}

	// Token: 0x06002745 RID: 10053 RVA: 0x000CFA9A File Offset: 0x000CDC9A
	public void SetIntensity(float intensity)
	{
		this.intensity = intensity;
		this.UpdateRender();
	}

	// Token: 0x06002746 RID: 10054 RVA: 0x000CFAA9 File Offset: 0x000CDCA9
	public void SetBlendIn(float blendIn)
	{
		this.blendIn = blendIn;
		this.UpdateRender();
	}

	// Token: 0x06002747 RID: 10055 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void UpdateRender()
	{
	}

	// Token: 0x040032E7 RID: 13031
	public MeshRenderer glowRenderer;

	// Token: 0x040032E8 RID: 13032
	private float blendIn;

	// Token: 0x040032E9 RID: 13033
	private float intensity;
}
