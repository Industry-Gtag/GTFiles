using System;
using UnityEngine;

// Token: 0x020001FF RID: 511
public class CornOnCobCosmetic : MonoBehaviour
{
	// Token: 0x06000D6F RID: 3439 RVA: 0x00049A58 File Offset: 0x00047C58
	protected void Awake()
	{
		this.emissionModule = this.particleSys.emission;
		this.maxBurstProbability = ((this.emissionModule.burstCount > 0) ? this.emissionModule.GetBurst(0).probability : 0.2f);
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x00049AA8 File Offset: 0x00047CA8
	protected void LateUpdate()
	{
		for (int i = 0; i < this.emissionModule.burstCount; i++)
		{
			ParticleSystem.Burst burst = this.emissionModule.GetBurst(i);
			burst.probability = this.maxBurstProbability * this.particleEmissionCurve.Evaluate(this.thermalReceiver.celsius);
			this.emissionModule.SetBurst(i, burst);
		}
		int particleCount = this.particleSys.particleCount;
		if (particleCount > this.previousParticleCount)
		{
			this.soundBankPlayer.Play();
		}
		this.previousParticleCount = particleCount;
	}

	// Token: 0x0400101A RID: 4122
	[Tooltip("The corn will start popping based on the temperature from this ThermalReceiver.")]
	public ThermalReceiver thermalReceiver;

	// Token: 0x0400101B RID: 4123
	[Tooltip("The particle system that will be emitted when the heat source is hot enough.")]
	public ParticleSystem particleSys;

	// Token: 0x0400101C RID: 4124
	[Tooltip("The curve that determines how many particles will be emitted based on the heat source's temperature.\n\nThe x-axis is the heat source's temperature and the y-axis is the number of particles to emit.")]
	public AnimationCurve particleEmissionCurve;

	// Token: 0x0400101D RID: 4125
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x0400101E RID: 4126
	private ParticleSystem.EmissionModule emissionModule;

	// Token: 0x0400101F RID: 4127
	private float maxBurstProbability;

	// Token: 0x04001020 RID: 4128
	private int previousParticleCount;
}
