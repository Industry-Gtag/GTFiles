using System;

// Token: 0x02000A1A RID: 2586
public interface ITimeOfDaySystem
{
	// Token: 0x1700064B RID: 1611
	// (get) Token: 0x0600425F RID: 16991
	double currentTimeInSeconds { get; }

	// Token: 0x1700064C RID: 1612
	// (get) Token: 0x06004260 RID: 16992
	double totalTimeInSeconds { get; }
}
