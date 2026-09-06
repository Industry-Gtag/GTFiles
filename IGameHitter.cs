using System;
using UnityEngine;

// Token: 0x020006E4 RID: 1764
public interface IGameHitter
{
	// Token: 0x06002C6B RID: 11371
	void OnSuccessfulHit(GameHitData hit);

	// Token: 0x06002C6C RID: 11372 RVA: 0x00002C2D File Offset: 0x00000E2D
	void OnSuccessfulHitPlayer(GRPlayer player, Vector3 hitPosition)
	{
	}
}
