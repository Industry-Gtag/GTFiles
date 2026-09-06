using System;

// Token: 0x02000D51 RID: 3409
internal interface ITickSystemPost
{
	// Token: 0x1700080E RID: 2062
	// (get) Token: 0x06005469 RID: 21609
	// (set) Token: 0x0600546A RID: 21610
	bool PostTickRunning { get; set; }

	// Token: 0x0600546B RID: 21611
	void PostTick();
}
