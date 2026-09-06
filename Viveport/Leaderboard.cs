using System;

namespace Viveport
{
	// Token: 0x02000EA9 RID: 3753
	public class Leaderboard
	{
		// Token: 0x170008B8 RID: 2232
		// (get) Token: 0x06005B19 RID: 23321 RVA: 0x001D96A7 File Offset: 0x001D78A7
		// (set) Token: 0x06005B1A RID: 23322 RVA: 0x001D96AF File Offset: 0x001D78AF
		public int Rank { get; set; }

		// Token: 0x170008B9 RID: 2233
		// (get) Token: 0x06005B1B RID: 23323 RVA: 0x001D96B8 File Offset: 0x001D78B8
		// (set) Token: 0x06005B1C RID: 23324 RVA: 0x001D96C0 File Offset: 0x001D78C0
		public int Score { get; set; }

		// Token: 0x170008BA RID: 2234
		// (get) Token: 0x06005B1D RID: 23325 RVA: 0x001D96C9 File Offset: 0x001D78C9
		// (set) Token: 0x06005B1E RID: 23326 RVA: 0x001D96D1 File Offset: 0x001D78D1
		public string UserName { get; set; }
	}
}
