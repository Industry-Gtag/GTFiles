using System;

// Token: 0x0200063C RID: 1596
public interface IBuilderPieceFunctional
{
	// Token: 0x060027C4 RID: 10180
	void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp);

	// Token: 0x060027C5 RID: 10181
	void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp);

	// Token: 0x060027C6 RID: 10182
	bool IsStateValid(byte state);

	// Token: 0x060027C7 RID: 10183
	void FunctionalPieceUpdate();

	// Token: 0x060027C8 RID: 10184 RVA: 0x00002E60 File Offset: 0x00001060
	void FunctionalPieceFixedUpdate()
	{
		throw new NotImplementedException();
	}

	// Token: 0x060027C9 RID: 10185 RVA: 0x000D2D8F File Offset: 0x000D0F8F
	float GetInteractionDistace()
	{
		return 2.5f;
	}
}
