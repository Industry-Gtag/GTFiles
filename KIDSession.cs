using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x02000B39 RID: 2873
[Serializable]
public class KIDSession
{
	// Token: 0x17000700 RID: 1792
	// (get) Token: 0x06004972 RID: 18802 RVA: 0x001885FA File Offset: 0x001867FA
	// (set) Token: 0x06004973 RID: 18803 RVA: 0x00188602 File Offset: 0x00186802
	public SessionStatus SessionStatus { get; set; }

	// Token: 0x17000701 RID: 1793
	// (get) Token: 0x06004974 RID: 18804 RVA: 0x0018860B File Offset: 0x0018680B
	// (set) Token: 0x06004975 RID: 18805 RVA: 0x00188613 File Offset: 0x00186813
	public GTAgeStatusType AgeStatus { get; set; }

	// Token: 0x17000702 RID: 1794
	// (get) Token: 0x06004976 RID: 18806 RVA: 0x0018861C File Offset: 0x0018681C
	// (set) Token: 0x06004977 RID: 18807 RVA: 0x00188624 File Offset: 0x00186824
	public Guid SessionId { get; set; }

	// Token: 0x17000703 RID: 1795
	// (get) Token: 0x06004978 RID: 18808 RVA: 0x0018862D File Offset: 0x0018682D
	// (set) Token: 0x06004979 RID: 18809 RVA: 0x00188635 File Offset: 0x00186835
	public string KUID { get; set; }

	// Token: 0x17000704 RID: 1796
	// (get) Token: 0x0600497A RID: 18810 RVA: 0x0018863E File Offset: 0x0018683E
	// (set) Token: 0x0600497B RID: 18811 RVA: 0x00188646 File Offset: 0x00186846
	public string etag { get; set; }

	// Token: 0x17000705 RID: 1797
	// (get) Token: 0x0600497C RID: 18812 RVA: 0x0018864F File Offset: 0x0018684F
	// (set) Token: 0x0600497D RID: 18813 RVA: 0x00188657 File Offset: 0x00186857
	public List<Permission> Permissions { get; set; }

	// Token: 0x17000706 RID: 1798
	// (get) Token: 0x0600497E RID: 18814 RVA: 0x00188660 File Offset: 0x00186860
	// (set) Token: 0x0600497F RID: 18815 RVA: 0x00188668 File Offset: 0x00186868
	public DateTime DateOfBirth { get; set; }

	// Token: 0x17000707 RID: 1799
	// (get) Token: 0x06004980 RID: 18816 RVA: 0x00188671 File Offset: 0x00186871
	// (set) Token: 0x06004981 RID: 18817 RVA: 0x00188679 File Offset: 0x00186879
	public string Jurisdiction { get; set; }
}
