using System;

// Token: 0x020004FE RID: 1278
public interface ICosmeticStateSync
{
	// Token: 0x17000373 RID: 883
	// (get) Token: 0x06002009 RID: 8201
	int StateValue { get; }

	// Token: 0x0600200A RID: 8202
	void OnStateUpdate(int state);
}
