using System;
using UnityEngine;

// Token: 0x02000869 RID: 2153
public class GorillaColorizableParticle : GorillaColorizableBase
{
	// Token: 0x060037CB RID: 14283 RVA: 0x00131834 File Offset: 0x0012FA34
	public override void SetColor(Color color)
	{
		ParticleSystem.MainModule main = this.particleSystem.main;
		Color color2 = new Color(Mathf.Pow(color.r, this.gradientColorPower), Mathf.Pow(color.g, this.gradientColorPower), Mathf.Pow(color.b, this.gradientColorPower), color.a);
		main.startColor = new ParticleSystem.MinMaxGradient(this.useLinearColor ? color.linear : color, this.useLinearColor ? color2.linear : color2);
	}

	// Token: 0x040047F6 RID: 18422
	public ParticleSystem particleSystem;

	// Token: 0x040047F7 RID: 18423
	public float gradientColorPower = 2f;

	// Token: 0x040047F8 RID: 18424
	public bool useLinearColor = true;
}
