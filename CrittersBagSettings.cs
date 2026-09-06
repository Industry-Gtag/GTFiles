using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000057 RID: 87
public class CrittersBagSettings : CrittersActorSettings
{
	// Token: 0x060001B3 RID: 435 RVA: 0x0000A884 File Offset: 0x00008A84
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersBag crittersBag = (CrittersBag)this.parentActor;
		crittersBag.attachableCollider = this.attachableCollider;
		crittersBag.dropCube = this.dropCube;
		crittersBag.anchorLocation = this.anchorLocation;
		crittersBag.attachDisableColliders = this.attachDisableColliders;
		crittersBag.attachSound = this.attachSound;
		crittersBag.detachSound = this.detachSound;
		crittersBag.blockAttachTypes = this.blockAttachTypes;
	}

	// Token: 0x040001DA RID: 474
	public Collider attachableCollider;

	// Token: 0x040001DB RID: 475
	public BoxCollider dropCube;

	// Token: 0x040001DC RID: 476
	public CrittersAttachPoint.AnchoredLocationTypes anchorLocation;

	// Token: 0x040001DD RID: 477
	public List<Collider> attachDisableColliders;

	// Token: 0x040001DE RID: 478
	public AudioClip attachSound;

	// Token: 0x040001DF RID: 479
	public AudioClip detachSound;

	// Token: 0x040001E0 RID: 480
	public List<CrittersActor.CrittersActorType> blockAttachTypes;
}
