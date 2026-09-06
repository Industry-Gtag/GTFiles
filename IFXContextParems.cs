using System;

// Token: 0x02000D22 RID: 3362
public interface IFXContextParems<T> where T : FXSArgs
{
	// Token: 0x170007DB RID: 2011
	// (get) Token: 0x06005354 RID: 21332
	FXSystemSettings settings { get; }

	// Token: 0x06005355 RID: 21333
	void OnPlayFX(T parems);
}
