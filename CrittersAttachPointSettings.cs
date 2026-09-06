using System;

// Token: 0x02000055 RID: 85
public class CrittersAttachPointSettings : CrittersActorSettings
{
	// Token: 0x060001A7 RID: 423 RVA: 0x0000A3F9 File Offset: 0x000085F9
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersAttachPoint crittersAttachPoint = (CrittersAttachPoint)this.parentActor;
		crittersAttachPoint.anchorLocation = this.anchoredLocation;
		crittersAttachPoint.rb.isKinematic = true;
		crittersAttachPoint.isLeft = this.isLeft;
	}

	// Token: 0x040001CA RID: 458
	public bool isLeft;

	// Token: 0x040001CB RID: 459
	public CrittersAttachPoint.AnchoredLocationTypes anchoredLocation;
}
