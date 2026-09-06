using System;
using Newtonsoft.Json;

// Token: 0x02000B37 RID: 2871
[Serializable]
public class TMPPermission
{
	// Token: 0x170006FD RID: 1789
	// (get) Token: 0x0600496A RID: 18794 RVA: 0x001885C7 File Offset: 0x001867C7
	// (set) Token: 0x0600496B RID: 18795 RVA: 0x001885CF File Offset: 0x001867CF
	[JsonProperty("name")]
	public string Name { get; set; }

	// Token: 0x170006FE RID: 1790
	// (get) Token: 0x0600496C RID: 18796 RVA: 0x001885D8 File Offset: 0x001867D8
	// (set) Token: 0x0600496D RID: 18797 RVA: 0x001885E0 File Offset: 0x001867E0
	[JsonProperty("enabled")]
	public bool Enabled { get; set; }

	// Token: 0x170006FF RID: 1791
	// (get) Token: 0x0600496E RID: 18798 RVA: 0x001885E9 File Offset: 0x001867E9
	// (set) Token: 0x0600496F RID: 18799 RVA: 0x001885F1 File Offset: 0x001867F1
	[JsonProperty("managedBy")]
	public ManagedBy ManagedBy { get; set; }
}
