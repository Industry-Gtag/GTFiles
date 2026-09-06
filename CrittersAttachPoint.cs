using System;

// Token: 0x02000053 RID: 83
public class CrittersAttachPoint : CrittersActor
{
	// Token: 0x060001A5 RID: 421 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void ProcessRemote()
	{
	}

	// Token: 0x040001C3 RID: 451
	public bool fixedOrientation = true;

	// Token: 0x040001C4 RID: 452
	public CrittersAttachPoint.AnchoredLocationTypes anchorLocation;

	// Token: 0x040001C5 RID: 453
	public bool isLeft;

	// Token: 0x02000054 RID: 84
	public enum AnchoredLocationTypes
	{
		// Token: 0x040001C7 RID: 455
		Arm,
		// Token: 0x040001C8 RID: 456
		Chest,
		// Token: 0x040001C9 RID: 457
		Back
	}
}
