using System;

// Token: 0x020006BF RID: 1727
public interface IGameEntityCustomStateChange
{
	// Token: 0x06002B1B RID: 11035
	bool CanChangeState(long newState, int playerId);
}
