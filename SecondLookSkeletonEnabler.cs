using System;
using UnityEngine;

// Token: 0x020001D2 RID: 466
public class SecondLookSkeletonEnabler : Tappable
{
	// Token: 0x06000C65 RID: 3173 RVA: 0x000440AB File Offset: 0x000422AB
	private void Awake()
	{
		this.isTapped = false;
		this.skele = Object.FindFirstObjectByType<SecondLookSkeleton>();
		this.skele.spookyText = this.spookyText;
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x000440D0 File Offset: 0x000422D0
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (!this.isTapped)
		{
			base.OnTapLocal(tapStrength, tapTime, info);
			if (this.skele != null)
			{
				this.skele.tapped = true;
			}
			base.gameObject.SetActive(false);
			this.isTapped = true;
			this.playOnDisappear.GTPlay();
			this.particles.Play();
		}
	}

	// Token: 0x04000F17 RID: 3863
	public bool isTapped;

	// Token: 0x04000F18 RID: 3864
	public AudioSource playOnDisappear;

	// Token: 0x04000F19 RID: 3865
	public ParticleSystem particles;

	// Token: 0x04000F1A RID: 3866
	public GameObject spookyText;

	// Token: 0x04000F1B RID: 3867
	private SecondLookSkeleton skele;
}
