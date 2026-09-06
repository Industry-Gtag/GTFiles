using System;

// Token: 0x0200029F RID: 671
public interface IGameStateReceiver
{
	// Token: 0x060011B2 RID: 4530
	void GameStateReceiverOnStateChanged(long oldState, long newState);
}
