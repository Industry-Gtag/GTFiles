using System;
using GorillaLocomotion.Climbing;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001368 RID: 4968
	public class VelocityBasedAudioTriggerCosmetic : MonoBehaviour
	{
		// Token: 0x06007C7B RID: 31867 RVA: 0x0028A8C0 File Offset: 0x00288AC0
		private void Awake()
		{
			if (this.audioClip != null)
			{
				this.audioSource.clip = this.audioClip;
			}
			if (this.soundBank != null && this.audioSource != null)
			{
				this.soundBank.audioSource = this.audioSource;
			}
		}

		// Token: 0x06007C7C RID: 31868 RVA: 0x0028A91C File Offset: 0x00288B1C
		private void Update()
		{
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			if (averageVelocity.magnitude < this.minVelocityThreshold)
			{
				return;
			}
			float num = Mathf.InverseLerp(this.minVelocityThreshold, this.maxVelocity, averageVelocity.magnitude);
			float num2 = Mathf.Lerp(this.minOutputVolume, this.maxOutputVolume, num);
			this.audioSource.volume = num2;
			if (this.audioSource != null && !this.audioSource.isPlaying && this.audioClip != null)
			{
				this.audioSource.clip = this.audioClip;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			else if (this.soundBank != null && this.soundBank.soundBank != null && !this.soundBank.isPlaying)
			{
				this.soundBank.Play(new float?(num2), null);
			}
		}

		// Token: 0x04008EEC RID: 36588
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04008EED RID: 36589
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04008EEE RID: 36590
		[SerializeField]
		private AudioClip audioClip;

		// Token: 0x04008EEF RID: 36591
		[SerializeField]
		private SoundBankPlayer soundBank;

		// Token: 0x04008EF0 RID: 36592
		[Tooltip(" Minimum velocity to trigger audio")]
		[SerializeField]
		private float minVelocityThreshold = 0.5f;

		// Token: 0x04008EF1 RID: 36593
		[SerializeField]
		private float maxVelocity = 2f;

		// Token: 0x04008EF2 RID: 36594
		[SerializeField]
		private float minOutputVolume;

		// Token: 0x04008EF3 RID: 36595
		[SerializeField]
		private float maxOutputVolume = 1f;
	}
}
