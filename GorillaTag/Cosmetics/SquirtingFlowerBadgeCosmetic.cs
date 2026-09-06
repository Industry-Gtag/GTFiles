using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012F1 RID: 4849
	public class SquirtingFlowerBadgeCosmetic : MonoBehaviour, ISpawnable, IFingerFlexListener
	{
		// Token: 0x17000BC8 RID: 3016
		// (get) Token: 0x06007982 RID: 31106 RVA: 0x0027A2D2 File Offset: 0x002784D2
		// (set) Token: 0x06007983 RID: 31107 RVA: 0x0027A2DA File Offset: 0x002784DA
		public VRRig MyRig { get; private set; }

		// Token: 0x17000BC9 RID: 3017
		// (get) Token: 0x06007984 RID: 31108 RVA: 0x0027A2E3 File Offset: 0x002784E3
		// (set) Token: 0x06007985 RID: 31109 RVA: 0x0027A2EB File Offset: 0x002784EB
		public bool IsSpawned { get; set; }

		// Token: 0x17000BCA RID: 3018
		// (get) Token: 0x06007986 RID: 31110 RVA: 0x0027A2F4 File Offset: 0x002784F4
		// (set) Token: 0x06007987 RID: 31111 RVA: 0x0027A2FC File Offset: 0x002784FC
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06007988 RID: 31112 RVA: 0x0027A305 File Offset: 0x00278505
		public void OnSpawn(VRRig rig)
		{
			this.MyRig = rig;
		}

		// Token: 0x06007989 RID: 31113 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnDespawn()
		{
		}

		// Token: 0x0600798A RID: 31114 RVA: 0x0027A30E File Offset: 0x0027850E
		private void Update()
		{
			if (!this.restartTimer && Time.time - this.triggeredTime >= this.coolDownTimer)
			{
				this.restartTimer = true;
			}
		}

		// Token: 0x0600798B RID: 31115 RVA: 0x0027A334 File Offset: 0x00278534
		private void OnPlayEffectLocal()
		{
			if (this.particlesToPlay != null)
			{
				this.particlesToPlay.Play();
			}
			if (this.objectToEnable != null)
			{
				this.objectToEnable.SetActive(true);
			}
			if (this.audioSource != null && this.audioToPlay != null)
			{
				this.audioSource.GTPlayOneShot(this.audioToPlay, 1f);
			}
			this.restartTimer = false;
			this.triggeredTime = Time.time;
		}

		// Token: 0x0600798C RID: 31116 RVA: 0x0027A3B8 File Offset: 0x002785B8
		public void OnButtonPressed(bool isLeftHand, float value)
		{
			if (!this.FingerFlexValidation(isLeftHand))
			{
				return;
			}
			if (!this.restartTimer || !this.buttonReleased)
			{
				return;
			}
			this.OnPlayEffectLocal();
			this.buttonReleased = false;
		}

		// Token: 0x0600798D RID: 31117 RVA: 0x0027A3E2 File Offset: 0x002785E2
		public void OnButtonReleased(bool isLeftHand, float value)
		{
			if (!this.FingerFlexValidation(isLeftHand))
			{
				return;
			}
			this.buttonReleased = true;
		}

		// Token: 0x0600798E RID: 31118 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnButtonPressStayed(bool isLeftHand, float value)
		{
		}

		// Token: 0x0600798F RID: 31119 RVA: 0x0027A3F5 File Offset: 0x002785F5
		public bool FingerFlexValidation(bool isLeftHand)
		{
			return (!this.leftHand || isLeftHand) && (this.leftHand || !isLeftHand);
		}

		// Token: 0x04008AC6 RID: 35526
		[SerializeField]
		private ParticleSystem particlesToPlay;

		// Token: 0x04008AC7 RID: 35527
		[SerializeField]
		private GameObject objectToEnable;

		// Token: 0x04008AC8 RID: 35528
		[SerializeField]
		private AudioClip audioToPlay;

		// Token: 0x04008AC9 RID: 35529
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04008ACA RID: 35530
		[SerializeField]
		private float coolDownTimer = 2f;

		// Token: 0x04008ACB RID: 35531
		[SerializeField]
		private bool leftHand;

		// Token: 0x04008ACC RID: 35532
		private float triggeredTime;

		// Token: 0x04008ACD RID: 35533
		private bool restartTimer;

		// Token: 0x04008ACE RID: 35534
		private bool buttonReleased = true;
	}
}
