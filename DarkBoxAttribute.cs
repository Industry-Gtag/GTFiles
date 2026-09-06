using System;
using System.Diagnostics;

// Token: 0x020002A9 RID: 681
[Conditional("UNITY_EDITOR")]
public class DarkBoxAttribute : Attribute
{
	// Token: 0x060011D9 RID: 4569 RVA: 0x0000256A File Offset: 0x0000076A
	public DarkBoxAttribute()
	{
	}

	// Token: 0x060011DA RID: 4570 RVA: 0x0005FD81 File Offset: 0x0005DF81
	public DarkBoxAttribute(bool withBorders)
	{
		this.withBorders = withBorders;
	}

	// Token: 0x04001568 RID: 5480
	public readonly bool withBorders;
}
