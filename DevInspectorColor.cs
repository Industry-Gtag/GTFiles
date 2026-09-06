using System;

// Token: 0x02000323 RID: 803
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DevInspectorColor : Attribute
{
	// Token: 0x17000200 RID: 512
	// (get) Token: 0x0600140A RID: 5130 RVA: 0x0006CB84 File Offset: 0x0006AD84
	public string Color { get; }

	// Token: 0x0600140B RID: 5131 RVA: 0x0006CB8C File Offset: 0x0006AD8C
	public DevInspectorColor(string color)
	{
		this.Color = color;
	}
}
