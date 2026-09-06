using System;
using UnityEngine;

// Token: 0x0200004F RID: 79
public class CrittersActorSettings : MonoBehaviour
{
	// Token: 0x06000190 RID: 400 RVA: 0x00009FD2 File Offset: 0x000081D2
	public virtual void OnEnable()
	{
		this.UpdateActorSettings();
	}

	// Token: 0x06000191 RID: 401 RVA: 0x00009FDC File Offset: 0x000081DC
	public virtual void UpdateActorSettings()
	{
		this.parentActor.usesRB = this.usesRB;
		this.parentActor.rb.isKinematic = !this.usesRB;
		this.parentActor.equipmentStorable = this.canBeStored;
		this.parentActor.storeCollider = this.storeCollider;
		this.parentActor.equipmentStoreTriggerCollider = this.equipmentStoreTriggerCollider;
	}

	// Token: 0x040001AE RID: 430
	public CrittersActor parentActor;

	// Token: 0x040001AF RID: 431
	public bool usesRB;

	// Token: 0x040001B0 RID: 432
	public bool canBeStored;

	// Token: 0x040001B1 RID: 433
	public CapsuleCollider storeCollider;

	// Token: 0x040001B2 RID: 434
	public CapsuleCollider equipmentStoreTriggerCollider;
}
