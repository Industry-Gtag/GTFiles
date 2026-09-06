using System;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class SIGadgetHolsterDisk : SIGadget, I_SIDisruptable
{
	// Token: 0x06000664 RID: 1636 RVA: 0x00023B40 File Offset: 0x00021D40
	private void Awake()
	{
		this.SetState(SIGadgetHolsterDisk.State.Unequipped);
		this.referenceGadget.gameObject.SetActive(false);
		this.referenceTransform = this.referenceGadget.transform;
		this.cooldownTimer = 0f;
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x00023B76 File Offset: 0x00021D76
	private void Start()
	{
		this.CreateGadget();
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x00023B80 File Offset: 0x00021D80
	private void CreateGadget()
	{
		this.gameEntity.manager.RequestCreateItem(this.referenceGadget.gameObject.name.GetStaticHash(), this.referenceGadget.transform.position, this.referenceGadget.transform.rotation, (long)this.gameEntity.GetNetId());
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x00023BE0 File Offset: 0x00021DE0
	public void RegisterGadget(SIGadget gadget)
	{
		this.cachedGadget = gadget;
		this.grenadeGadget = this.cachedGadget.GetComponent<SIGadgetGrenade>();
		this.gadgetRB = this.cachedGadget.GetComponent<Rigidbody>();
		SIGadgetGrenade sigadgetGrenade = this.grenadeGadget;
		sigadgetGrenade.GrenadeFinished = (Action)Delegate.Combine(sigadgetGrenade.GrenadeFinished, new Action(this.GadgetRespawn));
		this.cachedGadget.gameObject.SetActive(false);
		this.GadgetRespawn();
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x00023C54 File Offset: 0x00021E54
	private new void OnDisable()
	{
		if (this.grenadeGadget != null)
		{
			SIGadgetGrenade sigadgetGrenade = this.grenadeGadget;
			sigadgetGrenade.GrenadeFinished = (Action)Delegate.Remove(sigadgetGrenade.GrenadeFinished, new Action(this.GadgetRespawn));
		}
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x00023C8C File Offset: 0x00021E8C
	protected override void OnUpdateAuthority(float dt)
	{
		base.OnUpdateAuthority(dt);
		switch (this.state)
		{
		case SIGadgetHolsterDisk.State.Unequipped:
		case SIGadgetHolsterDisk.State.Ready:
			break;
		case SIGadgetHolsterDisk.State.OnCooldown:
			this.cooldownTimer += dt;
			this.grenadeGadget.grenadeRenderer.material.SetFloat("_RespawnAmount", this.cooldownTimer / this.cooldownTime);
			if (this.cooldownTimer > this.cooldownTime)
			{
				this.SetState(SIGadgetHolsterDisk.State.Ready);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x00023D04 File Offset: 0x00021F04
	private void SetState(SIGadgetHolsterDisk.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetHolsterDisk.State.Unequipped:
			this.cooldownTimer = 0f;
			return;
		case SIGadgetHolsterDisk.State.OnCooldown:
			break;
		case SIGadgetHolsterDisk.State.Ready:
			this.cachedGadget.gameEntity.pickupable = true;
			break;
		default:
			return;
		}
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x00023D59 File Offset: 0x00021F59
	public void DiskSnappedToHolster()
	{
		this.cachedGadget.gameObject.SetActive(true);
		this.gameEntity.pickupable = false;
		this.GadgetRespawn();
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x00023D7E File Offset: 0x00021F7E
	public void DiskRemovedFromHolster()
	{
		this.SetState(SIGadgetHolsterDisk.State.Unequipped);
		this.gameEntity.pickupable = true;
		this.cachedGadget.gameObject.SetActive(false);
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x00023DA4 File Offset: 0x00021FA4
	public void GadgetRespawn()
	{
		this.cachedGadget.transform.parent = base.transform;
		this.cachedGadget.transform.localPosition = this.referenceTransform.localPosition;
		this.cachedGadget.transform.localRotation = this.referenceTransform.localRotation;
		this.cachedGadget.gameEntity.pickupable = false;
		this.gadgetRB.isKinematic = true;
		this.SetState(SIGadgetHolsterDisk.State.OnCooldown);
		this.cooldownTimer = 0f;
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x00023E2C File Offset: 0x0002202C
	public void Disrupt(float disruptTime)
	{
		this.SetState(SIGadgetHolsterDisk.State.OnCooldown);
		this.cooldownTimer = -disruptTime;
	}

	// Token: 0x040007CE RID: 1998
	public SIGadget referenceGadget;

	// Token: 0x040007CF RID: 1999
	public float cooldownTime;

	// Token: 0x040007D0 RID: 2000
	private SIGadgetHolsterDisk.State state;

	// Token: 0x040007D1 RID: 2001
	private float cooldownTimer;

	// Token: 0x040007D2 RID: 2002
	private SIGadgetGrenade grenadeGadget;

	// Token: 0x040007D3 RID: 2003
	private Rigidbody gadgetRB;

	// Token: 0x040007D4 RID: 2004
	private SIGadget cachedGadget;

	// Token: 0x040007D5 RID: 2005
	private Transform referenceTransform;

	// Token: 0x02000110 RID: 272
	private enum State
	{
		// Token: 0x040007D7 RID: 2007
		Unequipped,
		// Token: 0x040007D8 RID: 2008
		OnCooldown,
		// Token: 0x040007D9 RID: 2009
		Ready
	}
}
