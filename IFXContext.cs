using System;

// Token: 0x02000D20 RID: 3360
public interface IFXContext
{
	// Token: 0x170007DA RID: 2010
	// (get) Token: 0x06005351 RID: 21329
	FXSystemSettings settings { get; }

	// Token: 0x06005352 RID: 21330
	void OnPlayFX();
}
