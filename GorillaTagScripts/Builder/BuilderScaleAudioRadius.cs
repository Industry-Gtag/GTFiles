using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001047 RID: 4167
	public class BuilderScaleAudioRadius : MonoBehaviour
	{
		// Token: 0x060067EC RID: 26604 RVA: 0x00216DD1 File Offset: 0x00214FD1
		private void OnEnable()
		{
			if (this.useLossyScaleOnEnable)
			{
				this.setScaleNextFrame = true;
				this.enableFrame = Time.frameCount;
			}
		}

		// Token: 0x060067ED RID: 26605 RVA: 0x00216DED File Offset: 0x00214FED
		private void OnDisable()
		{
			if (this.useLossyScaleOnEnable)
			{
				this.RevertScale();
			}
		}

		// Token: 0x060067EE RID: 26606 RVA: 0x00216DFD File Offset: 0x00214FFD
		private void LateUpdate()
		{
			if (this.setScaleNextFrame && Time.frameCount > this.enableFrame)
			{
				if (this.useLossyScaleOnEnable)
				{
					this.SetScale(base.transform.lossyScale.x);
				}
				this.setScaleNextFrame = false;
			}
		}

		// Token: 0x060067EF RID: 26607 RVA: 0x00216E39 File Offset: 0x00215039
		private void PlaySound()
		{
			if (this.autoPlaySoundBank != null)
			{
				this.autoPlaySoundBank.Play();
				return;
			}
			if (this.audioSource.clip != null)
			{
				this.audioSource.Play();
			}
		}

		// Token: 0x060067F0 RID: 26608 RVA: 0x00216E74 File Offset: 0x00215074
		public void SetScale(float inScale)
		{
			if (Mathf.Approximately(inScale, this.scale))
			{
				if (this.autoPlay)
				{
					this.PlaySound();
				}
				return;
			}
			this.scale = inScale;
			this.RevertScale();
			if (Mathf.Approximately(this.scale, 1f))
			{
				if (this.autoPlay)
				{
					this.PlaySound();
				}
				return;
			}
			AudioRolloffMode rolloffMode = this.audioSource.rolloffMode;
			if (rolloffMode > AudioRolloffMode.Linear)
			{
				if (rolloffMode == AudioRolloffMode.Custom)
				{
					this.maxDist = this.audioSource.maxDistance;
					this.audioSource.maxDistance *= this.scale;
				}
			}
			else
			{
				this.minDist = this.audioSource.minDistance;
				this.maxDist = this.audioSource.maxDistance;
				this.audioSource.maxDistance *= this.scale;
				this.audioSource.minDistance *= this.scale;
			}
			if (this.autoPlay)
			{
				this.PlaySound();
			}
			this.shouldRevert = true;
		}

		// Token: 0x060067F1 RID: 26609 RVA: 0x00216F74 File Offset: 0x00215174
		public void RevertScale()
		{
			if (!this.shouldRevert)
			{
				return;
			}
			AudioRolloffMode rolloffMode = this.audioSource.rolloffMode;
			if (rolloffMode > AudioRolloffMode.Linear)
			{
				if (rolloffMode == AudioRolloffMode.Custom)
				{
					this.audioSource.maxDistance = this.maxDist;
				}
			}
			else
			{
				this.audioSource.minDistance = this.minDist;
				this.audioSource.maxDistance = this.maxDist;
			}
			this.scale = 1f;
			this.shouldRevert = false;
		}

		// Token: 0x040076F4 RID: 30452
		[Tooltip("Scale particles on enable using lossy scale")]
		[SerializeField]
		private bool useLossyScaleOnEnable;

		// Token: 0x040076F5 RID: 30453
		[Tooltip("Play sound after scaling")]
		[SerializeField]
		private bool autoPlay;

		// Token: 0x040076F6 RID: 30454
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040076F7 RID: 30455
		[FormerlySerializedAs("soundBankToPlay")]
		[SerializeField]
		private SoundBankPlayer autoPlaySoundBank;

		// Token: 0x040076F8 RID: 30456
		private float minDist;

		// Token: 0x040076F9 RID: 30457
		private float maxDist = 1f;

		// Token: 0x040076FA RID: 30458
		private AnimationCurve customCurve;

		// Token: 0x040076FB RID: 30459
		private AnimationCurve scaledCurve = new AnimationCurve();

		// Token: 0x040076FC RID: 30460
		private float scale = 1f;

		// Token: 0x040076FD RID: 30461
		private bool shouldRevert;

		// Token: 0x040076FE RID: 30462
		private bool setScaleNextFrame;

		// Token: 0x040076FF RID: 30463
		private int enableFrame;
	}
}
