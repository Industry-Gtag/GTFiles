using System;
using UnityEngine;

namespace GorillaTag.Reactions
{
	// Token: 0x02001257 RID: 4695
	public class ShakeReaction : MonoBehaviour, ITickSystemPost
	{
		// Token: 0x17000B8F RID: 2959
		// (get) Token: 0x060076E2 RID: 30434 RVA: 0x00268D9E File Offset: 0x00266F9E
		private float loopSoundTotalDuration
		{
			get
			{
				return this.loopSoundFadeInDuration + this.loopSoundSustainDuration + this.loopSoundFadeOutDuration;
			}
		}

		// Token: 0x17000B90 RID: 2960
		// (get) Token: 0x060076E3 RID: 30435 RVA: 0x00268DB4 File Offset: 0x00266FB4
		// (set) Token: 0x060076E4 RID: 30436 RVA: 0x00268DBC File Offset: 0x00266FBC
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x060076E5 RID: 30437 RVA: 0x00268DC8 File Offset: 0x00266FC8
		protected void Awake()
		{
			this.sampleHistoryPos = new Vector3[256];
			this.sampleHistoryTime = new float[256];
			this.sampleHistoryVel = new Vector3[256];
			if (this.particles != null)
			{
				this.maxEmissionRate = this.particles.emission.rateOverTime.constant;
			}
			Application.quitting += this.HandleApplicationQuitting;
		}

		// Token: 0x060076E6 RID: 30438 RVA: 0x00268E48 File Offset: 0x00267048
		protected void OnEnable()
		{
			float unscaledTime = Time.unscaledTime;
			Vector3 position = this.shakeXform.position;
			for (int i = 0; i < 256; i++)
			{
				this.sampleHistoryTime[i] = unscaledTime;
				this.sampleHistoryPos[i] = position;
				this.sampleHistoryVel[i] = Vector3.zero;
			}
			if (this.loopSoundAudioSource != null)
			{
				this.loopSoundAudioSource.loop = true;
				this.loopSoundAudioSource.GTPlay();
			}
			this.hasLoopSound = this.loopSoundAudioSource != null;
			this.hasShakeSound = this.shakeSoundBankPlayer != null;
			this.hasParticleSystem = this.particles != null;
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x060076E7 RID: 30439 RVA: 0x00268EFF File Offset: 0x002670FF
		protected void OnDisable()
		{
			if (this.loopSoundAudioSource != null)
			{
				this.loopSoundAudioSource.GTStop();
			}
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x060076E8 RID: 30440 RVA: 0x0015AB83 File Offset: 0x00158D83
		private void HandleApplicationQuitting()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x060076E9 RID: 30441 RVA: 0x00268F20 File Offset: 0x00267120
		void ITickSystemPost.PostTick()
		{
			float unscaledTime = Time.unscaledTime;
			Vector3 position = this.shakeXform.position;
			int num = (this.currentIndex - 1 + 256) % 256;
			this.currentIndex = (this.currentIndex + 1) % 256;
			this.sampleHistoryTime[this.currentIndex] = unscaledTime;
			float num2 = unscaledTime - this.sampleHistoryTime[num];
			this.sampleHistoryPos[this.currentIndex] = position;
			if (num2 > 0f)
			{
				Vector3 vector = position - this.sampleHistoryPos[num];
				this.sampleHistoryVel[this.currentIndex] = vector / num2;
			}
			else
			{
				this.sampleHistoryVel[this.currentIndex] = Vector3.zero;
			}
			float sqrMagnitude = (this.sampleHistoryVel[num] - this.sampleHistoryVel[this.currentIndex]).sqrMagnitude;
			this.poopVelocity = Mathf.Round(Mathf.Sqrt(sqrMagnitude) * 1000f) / 1000f;
			float num3 = this.shakeXform.lossyScale.x * this.velocityThreshold * this.velocityThreshold;
			if (sqrMagnitude >= num3)
			{
				this.lastShakeTime = unscaledTime;
			}
			float num4 = unscaledTime - this.lastShakeTime;
			float num5 = Mathf.Clamp01(num4 / this.particleDuration);
			if (this.hasParticleSystem)
			{
				this.particles.emission.rateOverTime = this.emissionCurve.Evaluate(num5) * this.maxEmissionRate;
			}
			if (this.hasShakeSound && this.lastShakeTime - this.lastShakeSoundTime > this.shakeSoundCooldown)
			{
				this.shakeSoundBankPlayer.Play();
				this.lastShakeSoundTime = unscaledTime;
			}
			if (this.hasLoopSound)
			{
				if (num4 < this.loopSoundFadeInDuration)
				{
					this.loopSoundAudioSource.volume = this.loopSoundBaseVolume * this.loopSoundFadeInCurve.Evaluate(Mathf.Clamp01(num4 / this.loopSoundFadeInDuration));
					return;
				}
				if (num4 < this.loopSoundFadeInDuration + this.loopSoundSustainDuration)
				{
					this.loopSoundAudioSource.volume = this.loopSoundBaseVolume;
					return;
				}
				this.loopSoundAudioSource.volume = this.loopSoundBaseVolume * this.loopSoundFadeOutCurve.Evaluate(Mathf.Clamp01((num4 - this.loopSoundFadeInDuration - this.loopSoundSustainDuration) / this.loopSoundFadeOutDuration));
			}
		}

		// Token: 0x0400866E RID: 34414
		[SerializeField]
		private Transform shakeXform;

		// Token: 0x0400866F RID: 34415
		[SerializeField]
		private float velocityThreshold = 5f;

		// Token: 0x04008670 RID: 34416
		[SerializeField]
		private SoundBankPlayer shakeSoundBankPlayer;

		// Token: 0x04008671 RID: 34417
		[SerializeField]
		private float shakeSoundCooldown = 1f;

		// Token: 0x04008672 RID: 34418
		[SerializeField]
		private AudioSource loopSoundAudioSource;

		// Token: 0x04008673 RID: 34419
		[SerializeField]
		private float loopSoundBaseVolume = 1f;

		// Token: 0x04008674 RID: 34420
		[SerializeField]
		private float loopSoundSustainDuration = 1f;

		// Token: 0x04008675 RID: 34421
		[SerializeField]
		private float loopSoundFadeInDuration = 1f;

		// Token: 0x04008676 RID: 34422
		[SerializeField]
		private AnimationCurve loopSoundFadeInCurve;

		// Token: 0x04008677 RID: 34423
		[SerializeField]
		private float loopSoundFadeOutDuration = 1f;

		// Token: 0x04008678 RID: 34424
		[SerializeField]
		private AnimationCurve loopSoundFadeOutCurve;

		// Token: 0x04008679 RID: 34425
		[SerializeField]
		private ParticleSystem particles;

		// Token: 0x0400867A RID: 34426
		[SerializeField]
		private AnimationCurve emissionCurve;

		// Token: 0x0400867B RID: 34427
		[SerializeField]
		private float particleDuration = 5f;

		// Token: 0x0400867D RID: 34429
		private const int sampleHistorySize = 256;

		// Token: 0x0400867E RID: 34430
		private float[] sampleHistoryTime;

		// Token: 0x0400867F RID: 34431
		private Vector3[] sampleHistoryPos;

		// Token: 0x04008680 RID: 34432
		private Vector3[] sampleHistoryVel;

		// Token: 0x04008681 RID: 34433
		private int currentIndex;

		// Token: 0x04008682 RID: 34434
		private float lastShakeSoundTime = float.MinValue;

		// Token: 0x04008683 RID: 34435
		private float lastShakeTime = float.MinValue;

		// Token: 0x04008684 RID: 34436
		private float maxEmissionRate;

		// Token: 0x04008685 RID: 34437
		private bool hasLoopSound;

		// Token: 0x04008686 RID: 34438
		private bool hasShakeSound;

		// Token: 0x04008687 RID: 34439
		private bool hasParticleSystem;

		// Token: 0x04008688 RID: 34440
		[DebugReadout]
		private float poopVelocity;
	}
}
