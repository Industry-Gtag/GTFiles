using System;

// Token: 0x0200029E RID: 670
public interface IGameStateProvider
{
	// Token: 0x060011B0 RID: 4528
	void GameStateReceiverRegister(IGameStateReceiver receiver);

	// Token: 0x060011B1 RID: 4529
	void GameStateReceiverUnregister(IGameStateReceiver receiver);
}
