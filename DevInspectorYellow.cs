using System;

// Token: 0x02000326 RID: 806
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DevInspectorYellow : DevInspectorColor
{
	// Token: 0x0600140E RID: 5134 RVA: 0x0006CB9B File Offset: 0x0006AD9B
	public DevInspectorYellow()
		: base("#ff5")
	{
	}
}
