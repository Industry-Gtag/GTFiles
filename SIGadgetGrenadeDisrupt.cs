using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000105 RID: 261
public class SIGadgetGrenadeDisrupt : SIGadgetGrenade
{
	// Token: 0x06000634 RID: 1588 RVA: 0x00023317 File Offset: 0x00021517
	protected override void OnEnable()
	{
		base.OnEnable();
		this.state = SIGadgetGrenadeDisrupt.State.Idle;
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x00023328 File Offset: 0x00021528
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeDisrupt.State state = (SIGadgetGrenadeDisrupt.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x00023352 File Offset: 0x00021552
	private void SetStateAuthority(SIGadgetGrenadeDisrupt.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x00023374 File Offset: 0x00021574
	private void SetState(SIGadgetGrenadeDisrupt.State newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeDisrupt.State.Idle:
		case SIGadgetGrenadeDisrupt.State.Thrown:
			break;
		case SIGadgetGrenadeDisrupt.State.Triggered:
			this.TriggerExplosion();
			break;
		default:
			return;
		}
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x000233B4 File Offset: 0x000215B4
	private void TriggerExplosion()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.explosionRadius);
		for (int i = 0; i < array.Length; i++)
		{
			I_SIDisruptable componentInParent = array[i].GetComponentInParent<I_SIDisruptable>();
			if (componentInParent != null)
			{
				componentInParent.Disrupt(this.disruptTime);
			}
		}
		if (this.gameEntity.lastHeldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.SetStateAuthority(SIGadgetGrenadeDisrupt.State.Idle);
		}
		Action grenadeFinished = this.GrenadeFinished;
		if (grenadeFinished == null)
		{
			return;
		}
		grenadeFinished();
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void HandleActivated()
	{
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x0002342C File Offset: 0x0002162C
	protected override void HandleHitSurface()
	{
		if (this.state == SIGadgetGrenadeDisrupt.State.Thrown)
		{
			this.SetStateAuthority(SIGadgetGrenadeDisrupt.State.Triggered);
		}
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x0002343E File Offset: 0x0002163E
	protected override void HandleThrown()
	{
		if (this.state == SIGadgetGrenadeDisrupt.State.Idle)
		{
			this.SetStateAuthority(SIGadgetGrenadeDisrupt.State.Thrown);
		}
	}

	// Token: 0x040007A0 RID: 1952
	public float disruptTime;

	// Token: 0x040007A1 RID: 1953
	[SerializeField]
	private float explosionRadius;

	// Token: 0x040007A2 RID: 1954
	private SIGadgetGrenadeDisrupt.State state;

	// Token: 0x02000106 RID: 262
	private enum State
	{
		// Token: 0x040007A4 RID: 1956
		Idle,
		// Token: 0x040007A5 RID: 1957
		Thrown,
		// Token: 0x040007A6 RID: 1958
		Triggered
	}
}
