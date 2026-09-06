using System;
using UnityEngine;

namespace DefaultNamespace
{
	// Token: 0x020013CE RID: 5070
	[RequireComponent(typeof(SoundBankPlayer))]
	public class SoundBankPlayerCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000C57 RID: 3159
		// (get) Token: 0x06007EE2 RID: 32482 RVA: 0x00298F86 File Offset: 0x00297186
		// (set) Token: 0x06007EE3 RID: 32483 RVA: 0x00298F8E File Offset: 0x0029718E
		public bool TickRunning { get; set; }

		// Token: 0x06007EE4 RID: 32484 RVA: 0x00298F97 File Offset: 0x00297197
		private void Awake()
		{
			this.playAudioLoop = false;
		}

		// Token: 0x06007EE5 RID: 32485 RVA: 0x0001A297 File Offset: 0x00018497
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06007EE6 RID: 32486 RVA: 0x0001A29F File Offset: 0x0001849F
		private void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06007EE7 RID: 32487 RVA: 0x00298FA0 File Offset: 0x002971A0
		public void Tick()
		{
			if (!this.playAudioLoop)
			{
				return;
			}
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null && !this.soundBankPlayer.audioSource.isPlaying)
			{
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x06007EE8 RID: 32488 RVA: 0x00299008 File Offset: 0x00297208
		public void PlayAudio()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x06007EE9 RID: 32489 RVA: 0x00299054 File Offset: 0x00297254
		public void PlayAudioLoop()
		{
			this.playAudioLoop = true;
		}

		// Token: 0x06007EEA RID: 32490 RVA: 0x00299060 File Offset: 0x00297260
		public void PlayAudioNonInterrupting()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				if (this.soundBankPlayer.audioSource.isPlaying)
				{
					return;
				}
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x06007EEB RID: 32491 RVA: 0x002990C0 File Offset: 0x002972C0
		public void PlayAudioWithTunableVolume(bool leftHand, float fingerValue)
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				float num = Mathf.Clamp01(fingerValue);
				this.soundBankPlayer.audioSource.volume = num;
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x06007EEC RID: 32492 RVA: 0x00299124 File Offset: 0x00297324
		public void StopAudio()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				this.soundBankPlayer.audioSource.Stop();
			}
			this.playAudioLoop = false;
		}

		// Token: 0x0400914C RID: 37196
		[SerializeField]
		private SoundBankPlayer soundBankPlayer;

		// Token: 0x0400914D RID: 37197
		private bool playAudioLoop;
	}
}
