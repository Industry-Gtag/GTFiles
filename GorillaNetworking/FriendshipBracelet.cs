using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020010CE RID: 4302
	public class FriendshipBracelet : MonoBehaviour
	{
		// Token: 0x06006B7E RID: 27518 RVA: 0x0022A862 File Offset: 0x00228A62
		protected void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
		}

		// Token: 0x06006B7F RID: 27519 RVA: 0x0022A870 File Offset: 0x00228A70
		private AudioSource GetAudioSource()
		{
			if (!this.isLeftHand)
			{
				return this.ownerRig.rightHandPlayer;
			}
			return this.ownerRig.leftHandPlayer;
		}

		// Token: 0x06006B80 RID: 27520 RVA: 0x0022A891 File Offset: 0x00228A91
		private void OnEnable()
		{
			this.PlayAppearEffects();
		}

		// Token: 0x06006B81 RID: 27521 RVA: 0x0022A899 File Offset: 0x00228A99
		public void PlayAppearEffects()
		{
			this.GetAudioSource().GTPlayOneShot(this.braceletFormedSound, 1f);
			if (this.braceletFormedParticle)
			{
				this.braceletFormedParticle.Play();
			}
		}

		// Token: 0x06006B82 RID: 27522 RVA: 0x0022A8CC File Offset: 0x00228ACC
		private void OnDisable()
		{
			if (!this.ownerRig.gameObject.activeInHierarchy)
			{
				return;
			}
			this.GetAudioSource().GTPlayOneShot(this.braceletBrokenSound, 1f);
			if (this.braceletBrokenParticle)
			{
				this.braceletBrokenParticle.Play();
			}
		}

		// Token: 0x06006B83 RID: 27523 RVA: 0x0022A91C File Offset: 0x00228B1C
		public void UpdateBeads(List<Color> colors, int selfIndex)
		{
			int num = colors.Count - 1;
			int num2 = (this.braceletBeads.Length - num) / 2;
			for (int i = 0; i < this.braceletBeads.Length; i++)
			{
				int num3 = i - num2;
				if (num3 >= 0 && num3 < num)
				{
					this.braceletBeads[i].enabled = true;
					this.braceletBeads[i].material.color = colors[num3];
					this.braceletBananas[i].gameObject.SetActive(num3 == selfIndex);
				}
				else
				{
					this.braceletBeads[i].enabled = false;
					this.braceletBananas[i].gameObject.SetActive(false);
				}
			}
			SkinnedMeshRenderer[] array = this.braceletStrings;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].material.color = colors[colors.Count - 1];
			}
		}

		// Token: 0x04007B00 RID: 31488
		[SerializeField]
		private SkinnedMeshRenderer[] braceletStrings;

		// Token: 0x04007B01 RID: 31489
		[SerializeField]
		private MeshRenderer[] braceletBeads;

		// Token: 0x04007B02 RID: 31490
		[SerializeField]
		private MeshRenderer[] braceletBananas;

		// Token: 0x04007B03 RID: 31491
		[SerializeField]
		private bool isLeftHand;

		// Token: 0x04007B04 RID: 31492
		[SerializeField]
		private AudioClip braceletFormedSound;

		// Token: 0x04007B05 RID: 31493
		[SerializeField]
		private AudioClip braceletBrokenSound;

		// Token: 0x04007B06 RID: 31494
		[SerializeField]
		private ParticleSystem braceletFormedParticle;

		// Token: 0x04007B07 RID: 31495
		[SerializeField]
		private ParticleSystem braceletBrokenParticle;

		// Token: 0x04007B08 RID: 31496
		private VRRig ownerRig;
	}
}
