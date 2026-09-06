using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x020012BD RID: 4797
	public class PFXExtraAnimControls : MonoBehaviour
	{
		// Token: 0x0600785A RID: 30810 RVA: 0x0027043C File Offset: 0x0026E63C
		protected void Awake()
		{
			this.emissionModules = new ParticleSystem.EmissionModule[this.particleSystems.Length];
			this.cachedEmitBursts = new ParticleSystem.Burst[this.particleSystems.Length][];
			this.adjustedEmitBursts = new ParticleSystem.Burst[this.particleSystems.Length][];
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				ParticleSystem.EmissionModule emission = this.particleSystems[i].emission;
				this.cachedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				this.adjustedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				for (int j = 0; j < emission.burstCount; j++)
				{
					this.cachedEmitBursts[i][j] = emission.GetBurst(j);
					this.adjustedEmitBursts[i][j] = emission.GetBurst(j);
				}
				this.emissionModules[i] = emission;
			}
		}

		// Token: 0x0600785B RID: 30811 RVA: 0x0027051C File Offset: 0x0026E71C
		protected void LateUpdate()
		{
			for (int i = 0; i < this.emissionModules.Length; i++)
			{
				this.emissionModules[i].rateOverTimeMultiplier = this.emitRateMult;
				Mathf.Min(this.emissionModules[i].burstCount, this.cachedEmitBursts[i].Length);
				for (int j = 0; j < this.cachedEmitBursts[i].Length; j++)
				{
					this.adjustedEmitBursts[i][j].probability = this.cachedEmitBursts[i][j].probability * this.emitBurstProbabilityMult;
				}
				this.emissionModules[i].SetBursts(this.adjustedEmitBursts[i]);
			}
		}

		// Token: 0x04008895 RID: 34965
		public float emitRateMult = 1f;

		// Token: 0x04008896 RID: 34966
		public float emitBurstProbabilityMult = 1f;

		// Token: 0x04008897 RID: 34967
		[SerializeField]
		private ParticleSystem[] particleSystems;

		// Token: 0x04008898 RID: 34968
		private ParticleSystem.EmissionModule[] emissionModules;

		// Token: 0x04008899 RID: 34969
		private ParticleSystem.Burst[][] cachedEmitBursts;

		// Token: 0x0400889A RID: 34970
		private ParticleSystem.Burst[][] adjustedEmitBursts;
	}
}
