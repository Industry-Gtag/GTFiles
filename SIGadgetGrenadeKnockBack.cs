using System;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class SIGadgetGrenadeKnockBack : SIGadgetGrenade
{
	// Token: 0x0600064D RID: 1613 RVA: 0x00023837 File Offset: 0x00021A37
	protected override void OnEnable()
	{
		base.OnEnable();
		this.state = SIGadgetGrenadeKnockBack.State.Idle;
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void HandleActivated()
	{
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x00023846 File Offset: 0x00021A46
	protected override void HandleHitSurface()
	{
		if (this.state == SIGadgetGrenadeKnockBack.State.Thrown)
		{
			this.SetStateAuthority(SIGadgetGrenadeKnockBack.State.Triggered);
		}
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x00023858 File Offset: 0x00021A58
	protected override void HandleThrown()
	{
		if (this.state == SIGadgetGrenadeKnockBack.State.Idle)
		{
			this.SetStateAuthority(SIGadgetGrenadeKnockBack.State.Thrown);
		}
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnUpdateAuthority(float dt)
	{
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x0002386C File Offset: 0x00021A6C
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeKnockBack.State state = (SIGadgetGrenadeKnockBack.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x00023896 File Offset: 0x00021A96
	private void SetStateAuthority(SIGadgetGrenadeKnockBack.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x000238B8 File Offset: 0x00021AB8
	private void SetState(SIGadgetGrenadeKnockBack.State newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeKnockBack.State.Idle:
		case SIGadgetGrenadeKnockBack.State.Thrown:
			break;
		case SIGadgetGrenadeKnockBack.State.Triggered:
			this.TriggerExplosion();
			break;
		default:
			return;
		}
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x000238F8 File Offset: 0x00021AF8
	private void TriggerExplosion()
	{
		Vector3 vector = GTPlayer.Instance.transform.position - base.transform.position;
		float sqrMagnitude = vector.sqrMagnitude;
		if (this.explosionRadius * this.explosionRadius > sqrMagnitude)
		{
			float num = Mathf.Sqrt(sqrMagnitude);
			float num2 = 1f - num / this.explosionRadius;
			float num3 = this.knockbackStrength * num2;
			GTPlayer.Instance.ApplyKnockback(vector.normalized, num3, false);
		}
		if (this.gameEntity.lastHeldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.SetStateAuthority(SIGadgetGrenadeKnockBack.State.Idle);
		}
		Action grenadeFinished = this.GrenadeFinished;
		if (grenadeFinished == null)
		{
			return;
		}
		grenadeFinished();
	}

	// Token: 0x040007B9 RID: 1977
	[SerializeField]
	private float knockbackStrength;

	// Token: 0x040007BA RID: 1978
	[SerializeField]
	private float explosionRadius;

	// Token: 0x040007BB RID: 1979
	private SIGadgetGrenadeKnockBack.State state;

	// Token: 0x0200010A RID: 266
	private enum State
	{
		// Token: 0x040007BD RID: 1981
		Idle,
		// Token: 0x040007BE RID: 1982
		Thrown,
		// Token: 0x040007BF RID: 1983
		Triggered
	}
}
