using System;

// Token: 0x020006E3 RID: 1763
public interface IGameHittable
{
	// Token: 0x06002C69 RID: 11369
	bool IsHitValid(GameHitData hit);

	// Token: 0x06002C6A RID: 11370
	void OnHit(GameHitData hit);
}
