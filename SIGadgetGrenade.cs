using System;
using UnityEngine;

// Token: 0x02000102 RID: 258
public abstract class SIGadgetGrenade : SIGadget
{
	// Token: 0x06000623 RID: 1571 RVA: 0x00023094 File Offset: 0x00021294
	protected new virtual void OnEnable()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.activatedLocally = false;
		this.thrownGadget.OnActivated += this.HandleActivated;
		this.thrownGadget.OnThrown += this.HandleThrown;
		this.thrownGadget.OnHitSurface += this.HandleHitSurface;
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x000230FC File Offset: 0x000212FC
	protected new virtual void OnDisable()
	{
		this.thrownGadget.OnActivated -= this.HandleActivated;
		this.thrownGadget.OnThrown -= this.HandleThrown;
		this.thrownGadget.OnHitSurface -= this.HandleHitSurface;
	}

	// Token: 0x06000625 RID: 1573
	protected abstract void HandleActivated();

	// Token: 0x06000626 RID: 1574
	protected abstract void HandleThrown();

	// Token: 0x06000627 RID: 1575
	protected abstract void HandleHitSurface();

	// Token: 0x06000628 RID: 1576 RVA: 0x00023154 File Offset: 0x00021354
	public override void OnEntityInit()
	{
		base.OnEntityInit();
		GameEntityId entityIdFromNetId = this.gameEntity.manager.GetEntityIdFromNetId((int)this.gameEntity.createData);
		this.parentEntity = this.gameEntity.manager.GetGameEntity(entityIdFromNetId);
		SIGadgetHolsterDisk component = this.parentEntity.GetComponent<SIGadgetHolsterDisk>();
		if (component != null)
		{
			component.RegisterGadget(this);
		}
	}

	// Token: 0x04000794 RID: 1940
	public Action GrenadeFinished;

	// Token: 0x04000795 RID: 1941
	public Renderer grenadeRenderer;

	// Token: 0x04000796 RID: 1942
	[SerializeField]
	protected ThrownGadget thrownGadget;

	// Token: 0x04000797 RID: 1943
	protected Rigidbody rb;

	// Token: 0x04000798 RID: 1944
	protected GameEntity parentEntity;
}
