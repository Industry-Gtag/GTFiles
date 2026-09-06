using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020007BC RID: 1980
public class GRHazardousMaterial : MonoBehaviour
{
	// Token: 0x0600329A RID: 12954 RVA: 0x001159E7 File Offset: 0x00113BE7
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x0600329B RID: 12955 RVA: 0x001159F0 File Offset: 0x00113BF0
	public void OnLocalPlayerOverlap()
	{
		GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
		if (component != null && component.State == GRPlayer.GRPlayerState.Alive)
		{
			this.reactor.grManager.RequestPlayerStateChange(component, GRPlayer.GRPlayerState.Ghost);
		}
	}

	// Token: 0x0600329C RID: 12956 RVA: 0x00115A2B File Offset: 0x00113C2B
	private void OnTriggerEnter(Collider collider)
	{
		if (collider == GTPlayer.Instance.headCollider || collider == GTPlayer.Instance.bodyCollider)
		{
			this.OnLocalPlayerOverlap();
		}
	}

	// Token: 0x0600329D RID: 12957 RVA: 0x00115A57 File Offset: 0x00113C57
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider == GTPlayer.Instance.headCollider || collision.collider == GTPlayer.Instance.bodyCollider)
		{
			this.OnLocalPlayerOverlap();
		}
	}

	// Token: 0x040041A0 RID: 16800
	private GhostReactor reactor;
}
