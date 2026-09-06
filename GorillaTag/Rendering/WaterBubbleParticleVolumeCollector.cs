using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaTag.Rendering
{
	// Token: 0x020012B1 RID: 4785
	public class WaterBubbleParticleVolumeCollector : MonoBehaviour
	{
		// Token: 0x0600783A RID: 30778 RVA: 0x0026E9DC File Offset: 0x0026CBDC
		protected void Awake()
		{
			List<WaterVolume> componentsInHierarchy = SceneManager.GetActiveScene().GetComponentsInHierarchy(true, 64);
			List<Collider> list = new List<Collider>(componentsInHierarchy.Count * 4);
			foreach (WaterVolume waterVolume in componentsInHierarchy)
			{
				if (!(waterVolume.Parameters != null) || waterVolume.Parameters.allowBubblesInVolume)
				{
					foreach (Collider collider in waterVolume.volumeColliders)
					{
						if (!(collider == null))
						{
							list.Add(collider);
						}
					}
				}
			}
			this.bubbleableVolumeColliders = list.ToArray();
			this.particleTriggerModules = new ParticleSystem.TriggerModule[this.particleSystems.Length];
			this.particleEmissionModules = new ParticleSystem.EmissionModule[this.particleSystems.Length];
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				this.particleTriggerModules[i] = this.particleSystems[i].trigger;
				this.particleEmissionModules[i] = this.particleSystems[i].emission;
			}
			for (int j = 0; j < this.particleSystems.Length; j++)
			{
				ParticleSystem.TriggerModule triggerModule = this.particleTriggerModules[j];
				for (int k = 0; k < list.Count; k++)
				{
					triggerModule.SetCollider(k, this.bubbleableVolumeColliders[k]);
				}
			}
			this.SetEmissionState(false);
		}

		// Token: 0x0600783B RID: 30779 RVA: 0x0026EB7C File Offset: 0x0026CD7C
		protected void LateUpdate()
		{
			bool headInWater = GTPlayer.Instance.HeadInWater;
			if (headInWater && !this.emissionEnabled)
			{
				this.SetEmissionState(true);
				return;
			}
			if (!headInWater && this.emissionEnabled)
			{
				this.SetEmissionState(false);
			}
		}

		// Token: 0x0600783C RID: 30780 RVA: 0x0026EBBC File Offset: 0x0026CDBC
		private void SetEmissionState(bool setEnabled)
		{
			float num = (setEnabled ? 1f : 0f);
			for (int i = 0; i < this.particleEmissionModules.Length; i++)
			{
				this.particleEmissionModules[i].rateOverTimeMultiplier = num;
			}
			this.emissionEnabled = setEnabled;
		}

		// Token: 0x04008860 RID: 34912
		public ParticleSystem[] particleSystems;

		// Token: 0x04008861 RID: 34913
		private ParticleSystem.TriggerModule[] particleTriggerModules;

		// Token: 0x04008862 RID: 34914
		private ParticleSystem.EmissionModule[] particleEmissionModules;

		// Token: 0x04008863 RID: 34915
		private Collider[] bubbleableVolumeColliders;

		// Token: 0x04008864 RID: 34916
		private bool emissionEnabled;
	}
}
