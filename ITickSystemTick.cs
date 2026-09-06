using System;

// Token: 0x02000D50 RID: 3408
internal interface ITickSystemTick
{
	// Token: 0x1700080D RID: 2061
	// (get) Token: 0x06005466 RID: 21606
	// (set) Token: 0x06005467 RID: 21607
	bool TickRunning { get; set; }

	// Token: 0x06005468 RID: 21608
	void Tick();
}
