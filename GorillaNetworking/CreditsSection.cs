using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x020010D6 RID: 4310
	[Serializable]
	internal class CreditsSection
	{
		// Token: 0x17000A45 RID: 2629
		// (get) Token: 0x06006BB2 RID: 27570 RVA: 0x0022B79C File Offset: 0x0022999C
		// (set) Token: 0x06006BB3 RID: 27571 RVA: 0x0022B7A4 File Offset: 0x002299A4
		public string Title { get; set; }

		// Token: 0x17000A46 RID: 2630
		// (get) Token: 0x06006BB4 RID: 27572 RVA: 0x0022B7AD File Offset: 0x002299AD
		// (set) Token: 0x06006BB5 RID: 27573 RVA: 0x0022B7B5 File Offset: 0x002299B5
		public List<string> Entries { get; set; }
	}
}
