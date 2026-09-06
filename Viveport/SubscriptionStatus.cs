using System;
using System.Collections.Generic;

namespace Viveport
{
	// Token: 0x02000EAA RID: 3754
	public class SubscriptionStatus
	{
		// Token: 0x170008BB RID: 2235
		// (get) Token: 0x06005B20 RID: 23328 RVA: 0x001D96DA File Offset: 0x001D78DA
		// (set) Token: 0x06005B21 RID: 23329 RVA: 0x001D96E2 File Offset: 0x001D78E2
		public List<SubscriptionStatus.Platform> Platforms { get; set; }

		// Token: 0x170008BC RID: 2236
		// (get) Token: 0x06005B22 RID: 23330 RVA: 0x001D96EB File Offset: 0x001D78EB
		// (set) Token: 0x06005B23 RID: 23331 RVA: 0x001D96F3 File Offset: 0x001D78F3
		public SubscriptionStatus.TransactionType Type { get; set; }

		// Token: 0x06005B24 RID: 23332 RVA: 0x001D96FC File Offset: 0x001D78FC
		public SubscriptionStatus()
		{
			this.Platforms = new List<SubscriptionStatus.Platform>();
			this.Type = SubscriptionStatus.TransactionType.Unknown;
		}

		// Token: 0x02000EAB RID: 3755
		public enum Platform
		{
			// Token: 0x04006BFC RID: 27644
			Windows,
			// Token: 0x04006BFD RID: 27645
			Android
		}

		// Token: 0x02000EAC RID: 3756
		public enum TransactionType
		{
			// Token: 0x04006BFF RID: 27647
			Unknown,
			// Token: 0x04006C00 RID: 27648
			Paid,
			// Token: 0x04006C01 RID: 27649
			Redeem,
			// Token: 0x04006C02 RID: 27650
			FreeTrial
		}
	}
}
