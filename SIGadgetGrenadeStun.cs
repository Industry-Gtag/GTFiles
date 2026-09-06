using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class SIGadgetGrenadeStun : SIGadgetGrenade
{
	// Token: 0x06000657 RID: 1623 RVA: 0x0002399F File Offset: 0x00021B9F
	protected override void OnEnable()
	{
		base.OnEnable();
		this.state = SIGadgetGrenadeStun.State.Idle;
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void HandleActivated()
	{
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x000239AE File Offset: 0x00021BAE
	protected override void HandleHitSurface()
	{
		if (this.state == SIGadgetGrenadeStun.State.Thrown)
		{
			this.SetStateAuthority(SIGadgetGrenadeStun.State.Triggered);
		}
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x000239C0 File Offset: 0x00021BC0
	protected override void HandleThrown()
	{
		if (this.state == SIGadgetGrenadeStun.State.Idle)
		{
			this.SetStateAuthority(SIGadgetGrenadeStun.State.Thrown);
		}
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnUpdateAuthority(float dt)
	{
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x000239D4 File Offset: 0x00021BD4
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeStun.State state = (SIGadgetGrenadeStun.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x000239FE File Offset: 0x00021BFE
	private void SetStateAuthority(SIGadgetGrenadeStun.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x00023A20 File Offset: 0x00021C20
	private void SetState(SIGadgetGrenadeStun.State newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeStun.State.Idle:
		case SIGadgetGrenadeStun.State.Thrown:
			break;
		case SIGadgetGrenadeStun.State.Triggered:
			this.TriggerExplosion();
			break;
		default:
			return;
		}
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x00023A60 File Offset: 0x00021C60
	private void TriggerExplosion()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.explosionRadius, UnityLayer.GorillaTagCollider.ToLayerMask());
		for (int i = 0; i < array.Length; i++)
		{
			VRRig componentInParent = array[i].GetComponentInParent<VRRig>();
			if (componentInParent != null)
			{
				Vector3 vector = componentInParent.transform.position - base.transform.position;
				float magnitude = vector.magnitude;
				float num = 1f - magnitude / this.explosionRadius;
				float num2 = this.knockbackStrength * num;
				RoomSystem.LaunchPlayer(componentInParent.OwningNetPlayer, num2 * vector / magnitude);
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, componentInParent.OwningNetPlayer);
			}
		}
		if (this.gameEntity.lastHeldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.SetStateAuthority(SIGadgetGrenadeStun.State.Idle);
		}
	}

	// Token: 0x040007C0 RID: 1984
	[SerializeField]
	private float knockbackStrength;

	// Token: 0x040007C1 RID: 1985
	[SerializeField]
	private float explosionRadius;

	// Token: 0x040007C2 RID: 1986
	private SIGadgetGrenadeStun.State state;

	// Token: 0x0200010C RID: 268
	private enum State
	{
		// Token: 0x040007C4 RID: 1988
		Idle,
		// Token: 0x040007C5 RID: 1989
		Thrown,
		// Token: 0x040007C6 RID: 1990
		Triggered
	}
}
