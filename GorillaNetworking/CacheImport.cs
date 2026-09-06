using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x02001126 RID: 4390
	public class CacheImport
	{
		// Token: 0x17000A7C RID: 2684
		// (get) Token: 0x06006E0E RID: 28174 RVA: 0x00238467 File Offset: 0x00236667
		// (set) Token: 0x06006E0F RID: 28175 RVA: 0x0023846F File Offset: 0x0023666F
		public string DeploymentId { get; set; }

		// Token: 0x17000A7D RID: 2685
		// (get) Token: 0x06006E10 RID: 28176 RVA: 0x00238478 File Offset: 0x00236678
		// (set) Token: 0x06006E11 RID: 28177 RVA: 0x00238480 File Offset: 0x00236680
		public Dictionary<string, Dictionary<string, string>> TitleData { get; set; }
	}
}
