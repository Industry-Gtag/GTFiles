using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x02000B2E RID: 2862
[Serializable]
public class KIDDefaultSession
{
	// Token: 0x170006FA RID: 1786
	// (get) Token: 0x06004963 RID: 18787 RVA: 0x00188594 File Offset: 0x00186794
	// (set) Token: 0x06004964 RID: 18788 RVA: 0x0018859C File Offset: 0x0018679C
	public List<Permission> Permissions { get; set; }

	// Token: 0x170006FB RID: 1787
	// (get) Token: 0x06004965 RID: 18789 RVA: 0x001885A5 File Offset: 0x001867A5
	// (set) Token: 0x06004966 RID: 18790 RVA: 0x001885AD File Offset: 0x001867AD
	public AgeStatusType AgeStatus { get; set; }

	// Token: 0x170006FC RID: 1788
	// (get) Token: 0x06004967 RID: 18791 RVA: 0x001885B6 File Offset: 0x001867B6
	// (set) Token: 0x06004968 RID: 18792 RVA: 0x001885BE File Offset: 0x001867BE
	public int Age { get; set; }
}
