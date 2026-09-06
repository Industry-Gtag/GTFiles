using System;
using UnityEngine;

// Token: 0x0200092F RID: 2351
public class TapInnerGlow : MonoBehaviour
{
	// Token: 0x170005AC RID: 1452
	// (get) Token: 0x06003D96 RID: 15766 RVA: 0x0014E240 File Offset: 0x0014C440
	private Material targetMaterial
	{
		get
		{
			if (this._instance.AsNull<Material>() == null)
			{
				return this._instance = this._renderer.material;
			}
			return this._instance;
		}
	}

	// Token: 0x06003D97 RID: 15767 RVA: 0x0014E27C File Offset: 0x0014C47C
	public void Tap()
	{
		if (!this._renderer)
		{
			return;
		}
		Material targetMaterial = this.targetMaterial;
		float num = this.tapLength;
		float time = GTShaderGlobals.Time;
		UberShader.InnerGlowSinePeriod.SetValue<float>(targetMaterial, num);
		UberShader.InnerGlowSinePhaseShift.SetValue<float>(targetMaterial, time);
	}

	// Token: 0x04004E5D RID: 20061
	public Renderer _renderer;

	// Token: 0x04004E5E RID: 20062
	public float tapLength = 1f;

	// Token: 0x04004E5F RID: 20063
	[Space]
	private Material _instance;
}
