using System;

// Token: 0x02000D4F RID: 3407
internal interface ITickSystemPre
{
	// Token: 0x1700080C RID: 2060
	// (get) Token: 0x06005463 RID: 21603
	// (set) Token: 0x06005464 RID: 21604
	bool PreTickRunning { get; set; }

	// Token: 0x06005465 RID: 21605
	void PreTick();
}
