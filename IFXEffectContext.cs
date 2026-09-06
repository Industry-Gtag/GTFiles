using System;

// Token: 0x02000D24 RID: 3364
public interface IFXEffectContext<T> where T : IFXEffectContextObject
{
	// Token: 0x170007E3 RID: 2019
	// (get) Token: 0x06005360 RID: 21344
	T effectContext { get; }

	// Token: 0x170007E4 RID: 2020
	// (get) Token: 0x06005361 RID: 21345
	FXSystemSettings settings { get; }
}
