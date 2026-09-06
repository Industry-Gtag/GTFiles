using System;
using UnityEngine;

// Token: 0x02000E22 RID: 3618
public class SurfaceImpactFX : MonoBehaviour
{
	// Token: 0x06005894 RID: 22676 RVA: 0x001CC0AC File Offset: 0x001CA2AC
	public void Awake()
	{
		if (this.particleFX == null)
		{
			this.particleFX = base.GetComponent<ParticleSystem>();
		}
		if (this.particleFX == null)
		{
			Debug.LogError("SurfaceImpactFX: No ParticleSystem found! Disabling component.", this);
			base.enabled = false;
			return;
		}
		this.fxMainModule = this.particleFX.main;
	}

	// Token: 0x06005895 RID: 22677 RVA: 0x001CC105 File Offset: 0x001CA305
	public void SetScale(float scale)
	{
		this.fxMainModule.gravityModifierMultiplier = this.startingGravityModifier * scale;
		base.transform.localScale = this.startingScale * scale;
	}

	// Token: 0x040068C0 RID: 26816
	public ParticleSystem particleFX;

	// Token: 0x040068C1 RID: 26817
	public float startingGravityModifier;

	// Token: 0x040068C2 RID: 26818
	public Vector3 startingScale = Vector3.one;

	// Token: 0x040068C3 RID: 26819
	private ParticleSystem.MainModule fxMainModule;
}
