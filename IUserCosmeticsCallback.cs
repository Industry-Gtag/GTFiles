using System;

// Token: 0x02000D32 RID: 3378
internal interface IUserCosmeticsCallback
{
	// Token: 0x0600538A RID: 21386
	bool OnGetUserCosmetics(string cosmetics);

	// Token: 0x170007E8 RID: 2024
	// (get) Token: 0x0600538B RID: 21387
	// (set) Token: 0x0600538C RID: 21388
	bool PendingUpdate { get; set; }
}
