using System;
using UnityEngine;

// Token: 0x02000E85 RID: 3717
[Serializable]
public class VoiceLoudnessReactorParticleSystemTarget
{
	// Token: 0x1700089A RID: 2202
	// (get) Token: 0x06005A53 RID: 23123 RVA: 0x001D560B File Offset: 0x001D380B
	// (set) Token: 0x06005A54 RID: 23124 RVA: 0x001D5613 File Offset: 0x001D3813
	public float InitialSpeed
	{
		get
		{
			return this.initialSpeed;
		}
		set
		{
			this.initialSpeed = value;
		}
	}

	// Token: 0x1700089B RID: 2203
	// (get) Token: 0x06005A55 RID: 23125 RVA: 0x001D561C File Offset: 0x001D381C
	// (set) Token: 0x06005A56 RID: 23126 RVA: 0x001D5624 File Offset: 0x001D3824
	public float InitialRate
	{
		get
		{
			return this.initialRate;
		}
		set
		{
			this.initialRate = value;
		}
	}

	// Token: 0x1700089C RID: 2204
	// (get) Token: 0x06005A57 RID: 23127 RVA: 0x001D562D File Offset: 0x001D382D
	// (set) Token: 0x06005A58 RID: 23128 RVA: 0x001D5635 File Offset: 0x001D3835
	public float InitialSize
	{
		get
		{
			return this.initialSize;
		}
		set
		{
			this.initialSize = value;
		}
	}

	// Token: 0x04006B6A RID: 27498
	public ParticleSystem particleSystem;

	// Token: 0x04006B6B RID: 27499
	public bool UseSmoothedLoudness;

	// Token: 0x04006B6C RID: 27500
	public float Scale = 1f;

	// Token: 0x04006B6D RID: 27501
	private float initialSpeed;

	// Token: 0x04006B6E RID: 27502
	private float initialRate;

	// Token: 0x04006B6F RID: 27503
	private float initialSize;

	// Token: 0x04006B70 RID: 27504
	public AnimationCurve speed;

	// Token: 0x04006B71 RID: 27505
	public AnimationCurve rate;

	// Token: 0x04006B72 RID: 27506
	public AnimationCurve size;

	// Token: 0x04006B73 RID: 27507
	[HideInInspector]
	public ParticleSystem.MainModule Main;

	// Token: 0x04006B74 RID: 27508
	[HideInInspector]
	public ParticleSystem.EmissionModule Emission;
}
