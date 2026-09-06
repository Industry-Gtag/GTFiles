using System;
using UnityEngine;

// Token: 0x02000352 RID: 850
public class HandFXModifier : FXModifier
{
	// Token: 0x060014F3 RID: 5363 RVA: 0x0006FF5B File Offset: 0x0006E15B
	private void Awake()
	{
		this.originalScale = base.transform.localScale;
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x0006FF6E File Offset: 0x0006E16E
	private void OnDisable()
	{
		base.transform.localScale = this.originalScale;
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x0006FF81 File Offset: 0x0006E181
	public override void UpdateScale(float scale, Color color)
	{
		scale = Mathf.Clamp(scale, this.minScale, this.maxScale);
		base.transform.localScale = this.originalScale * scale;
	}

	// Token: 0x040019AC RID: 6572
	private Vector3 originalScale;

	// Token: 0x040019AD RID: 6573
	[SerializeField]
	private float minScale;

	// Token: 0x040019AE RID: 6574
	[SerializeField]
	private float maxScale;

	// Token: 0x040019AF RID: 6575
	[SerializeField]
	private ParticleSystem dustBurst;

	// Token: 0x040019B0 RID: 6576
	[SerializeField]
	private ParticleSystem dustLinger;
}
