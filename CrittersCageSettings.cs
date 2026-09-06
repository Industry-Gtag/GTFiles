using System;
using UnityEngine;

// Token: 0x0200005E RID: 94
public class CrittersCageSettings : CrittersActorSettings
{
	// Token: 0x060001D9 RID: 473 RVA: 0x0000B214 File Offset: 0x00009414
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersCage crittersCage = (CrittersCage)this.parentActor;
		crittersCage.cagePosition = this.cagePoint;
		crittersCage.grabPosition = this.grabPoint;
	}

	// Token: 0x0400021C RID: 540
	public Transform cagePoint;

	// Token: 0x0400021D RID: 541
	public Transform grabPoint;
}
