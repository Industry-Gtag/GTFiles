using System;
using UnityEngine;

// Token: 0x020007FE RID: 2046
public class GRSummonedEntity : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06003450 RID: 13392 RVA: 0x0011F789 File Offset: 0x0011D989
	private void Awake()
	{
		this.entity = base.GetComponent<GameEntity>();
	}

	// Token: 0x06003451 RID: 13393 RVA: 0x0011F798 File Offset: 0x0011D998
	public void OnEntityInit()
	{
		this.summonerEntityId = this.entity.createdByEntityId;
		if (this.summonerEntityId.IsValid())
		{
			this.summoner = this.FindSummoner();
			if (this.summoner != null)
			{
				this.summoner.OnSummonedEntityInit(this.entity);
			}
		}
	}

	// Token: 0x06003452 RID: 13394 RVA: 0x0011F7E8 File Offset: 0x0011D9E8
	public GameEntityId GetSummonerID()
	{
		return this.summonerEntityId;
	}

	// Token: 0x06003453 RID: 13395 RVA: 0x0011F7F0 File Offset: 0x0011D9F0
	public void OnEntityDestroy()
	{
		if (this.summoner != null)
		{
			this.summoner.OnSummonedEntityDestroy(this.entity);
		}
	}

	// Token: 0x06003454 RID: 13396 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06003455 RID: 13397 RVA: 0x0011F80C File Offset: 0x0011DA0C
	private IGRSummoningEntity FindSummoner()
	{
		if (this.summonerEntityId.IsValid())
		{
			GameEntity gameEntity = GhostReactorManager.Get(this.entity).gameEntityManager.GetGameEntity(this.summonerEntityId);
			if (gameEntity != null)
			{
				return gameEntity.GetComponent<IGRSummoningEntity>();
			}
		}
		return null;
	}

	// Token: 0x04004419 RID: 17433
	private GameEntityId summonerEntityId = GameEntityId.Invalid;

	// Token: 0x0400441A RID: 17434
	private GameEntity entity;

	// Token: 0x0400441B RID: 17435
	private IGRSummoningEntity summoner;
}
