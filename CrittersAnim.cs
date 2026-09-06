using System;
using UnityEngine;

// Token: 0x02000052 RID: 82
[Serializable]
public class CrittersAnim
{
	// Token: 0x060001A2 RID: 418 RVA: 0x0000A374 File Offset: 0x00008574
	public bool IsModified()
	{
		return (this.squashAmount != null && this.squashAmount.length > 1) || (this.forwardOffset != null && this.forwardOffset.length > 1) || (this.horizontalOffset != null && this.horizontalOffset.length > 1) || (this.verticalOffset != null && this.verticalOffset.length > 1);
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x0000A3DD File Offset: 0x000085DD
	public static bool IsModified(CrittersAnim anim)
	{
		return anim != null && anim.IsModified();
	}

	// Token: 0x040001BE RID: 446
	public AnimationCurve squashAmount;

	// Token: 0x040001BF RID: 447
	public AnimationCurve forwardOffset;

	// Token: 0x040001C0 RID: 448
	public AnimationCurve horizontalOffset;

	// Token: 0x040001C1 RID: 449
	public AnimationCurve verticalOffset;

	// Token: 0x040001C2 RID: 450
	public float playSpeed;
}
