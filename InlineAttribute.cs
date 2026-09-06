using System;
using System.Diagnostics;

// Token: 0x020002AB RID: 683
[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.All)]
public class InlineAttribute : Attribute
{
	// Token: 0x060011DC RID: 4572 RVA: 0x0005FD90 File Offset: 0x0005DF90
	public InlineAttribute(bool keepLabel = false, bool asGroup = false)
	{
		this.keepLabel = keepLabel;
		this.asGroup = asGroup;
	}

	// Token: 0x04001569 RID: 5481
	public readonly bool keepLabel;

	// Token: 0x0400156A RID: 5482
	public readonly bool asGroup;
}
