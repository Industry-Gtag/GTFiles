using System;

// Token: 0x02000327 RID: 807
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DevInspectorCyan : DevInspectorColor
{
	// Token: 0x0600140F RID: 5135 RVA: 0x0006CBA8 File Offset: 0x0006ADA8
	public DevInspectorCyan()
		: base("#5ff")
	{
	}
}
