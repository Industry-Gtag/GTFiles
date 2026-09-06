using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200076B RID: 1899
public class GRCurrencyDepositor : MonoBehaviour
{
	// Token: 0x06003017 RID: 12311 RVA: 0x00105864 File Offset: 0x00103A64
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06003018 RID: 12312 RVA: 0x00105870 File Offset: 0x00103A70
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody != null)
		{
			GRCollectible component = other.attachedRigidbody.GetComponent<GRCollectible>();
			if (component != null)
			{
				if ((component.type == ProgressionManager.CoreType.ChaosSeed && !this.collectSentientCores) || (component.type != ProgressionManager.CoreType.ChaosSeed && this.collectSentientCores))
				{
					return;
				}
				if (this.reactor.grManager.IsAuthority())
				{
					this.reactor.grManager.RequestDepositCollectible(component.entity.id);
				}
				this.collectibleDepositedEffect.Play();
				this.audioSource.volume = this.collectibleDepositedClipVolume;
				this.audioSource.PlayOneShot(this.collectibleDepositedClip);
				if (component.entity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					if (GamePlayerLocal.instance.gamePlayer.GetGrabbedGameEntityId(0) == component.entity.id)
					{
						GorillaTagger.Instance.StartVibration(true, 0.5f, 0.15f);
						return;
					}
					if (GamePlayerLocal.instance.gamePlayer.GetGrabbedGameEntityId(1) == component.entity.id)
					{
						GorillaTagger.Instance.StartVibration(false, 0.5f, 0.15f);
					}
				}
			}
		}
	}

	// Token: 0x04003DA9 RID: 15785
	public Transform depositingChargePoint;

	// Token: 0x04003DAA RID: 15786
	[SerializeField]
	private ParticleSystem collectibleDepositedEffect;

	// Token: 0x04003DAB RID: 15787
	[SerializeField]
	private AudioClip collectibleDepositedClip;

	// Token: 0x04003DAC RID: 15788
	[SerializeField]
	private float collectibleDepositedClipVolume;

	// Token: 0x04003DAD RID: 15789
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003DAE RID: 15790
	[SerializeField]
	private bool collectSentientCores;

	// Token: 0x04003DAF RID: 15791
	private const float hapticStrength = 0.5f;

	// Token: 0x04003DB0 RID: 15792
	private const float hapticDuration = 0.15f;

	// Token: 0x04003DB1 RID: 15793
	[NonSerialized]
	public GhostReactor reactor;
}
