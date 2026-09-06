using System;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000103 RID: 259
public class SIGadgetGrenadeBlackHole : SIGadgetGrenade
{
	// Token: 0x0600062A RID: 1578 RVA: 0x000231B7 File Offset: 0x000213B7
	protected override void OnEnable()
	{
		base.OnEnable();
		this.state = SIGadgetGrenadeBlackHole.State.Idle;
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void HandleActivated()
	{
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x000231C6 File Offset: 0x000213C6
	protected override void HandleHitSurface()
	{
		if (this.state == SIGadgetGrenadeBlackHole.State.Thrown)
		{
			this.SetStateAuthority(SIGadgetGrenadeBlackHole.State.Triggered);
		}
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x000231D8 File Offset: 0x000213D8
	protected override void HandleThrown()
	{
		if (this.state == SIGadgetGrenadeBlackHole.State.Idle)
		{
			this.SetStateAuthority(SIGadgetGrenadeBlackHole.State.Thrown);
		}
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnUpdateAuthority(float dt)
	{
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x000231EC File Offset: 0x000213EC
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeBlackHole.State state = (SIGadgetGrenadeBlackHole.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x00023216 File Offset: 0x00021416
	private void SetStateAuthority(SIGadgetGrenadeBlackHole.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x00023238 File Offset: 0x00021438
	private void SetState(SIGadgetGrenadeBlackHole.State newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeBlackHole.State.Idle:
		case SIGadgetGrenadeBlackHole.State.Thrown:
			break;
		case SIGadgetGrenadeBlackHole.State.Triggered:
			this.TriggerExplosion();
			break;
		default:
			return;
		}
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x00023278 File Offset: 0x00021478
	private void TriggerExplosion()
	{
		Vector3 vector = base.transform.position - GTPlayer.Instance.transform.position;
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
			this.SetStateAuthority(SIGadgetGrenadeBlackHole.State.Idle);
		}
	}

	// Token: 0x04000799 RID: 1945
	[SerializeField]
	private float knockbackStrength;

	// Token: 0x0400079A RID: 1946
	[SerializeField]
	private float explosionRadius;

	// Token: 0x0400079B RID: 1947
	private SIGadgetGrenadeBlackHole.State state;

	// Token: 0x02000104 RID: 260
	private enum State
	{
		// Token: 0x0400079D RID: 1949
		Idle,
		// Token: 0x0400079E RID: 1950
		Thrown,
		// Token: 0x0400079F RID: 1951
		Triggered
	}
}
