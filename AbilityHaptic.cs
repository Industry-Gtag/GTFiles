using System;

// Token: 0x02000731 RID: 1841
[Serializable]
public class AbilityHaptic
{
	// Token: 0x06002EBC RID: 11964 RVA: 0x000FFAB4 File Offset: 0x000FDCB4
	public void PlayIfHeldLocal(GameEntity gameEntity)
	{
		if (gameEntity == null || !gameEntity.IsHeldByLocalPlayer())
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), this.strength, this.duration);
	}

	// Token: 0x06002EBD RID: 11965 RVA: 0x000FFB18 File Offset: 0x000FDD18
	public void PlayIfSnappedLocal(GameEntity gameEntity)
	{
		if (gameEntity == null || !gameEntity.IsSnappedByLocalPlayer())
		{
			return;
		}
		GameSnappable component = gameEntity.GetComponent<GameSnappable>();
		if (component == null)
		{
			return;
		}
		if (component.IsSnappedToLeftArm())
		{
			GorillaTagger.Instance.StartVibration(true, this.strength, this.duration);
		}
		if (component.IsSnappedToRightArm())
		{
			GorillaTagger.Instance.StartVibration(false, this.strength, this.duration);
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), this.strength, this.duration);
	}

	// Token: 0x04003BEB RID: 15339
	public float strength = 0.2f;

	// Token: 0x04003BEC RID: 15340
	public float duration = 0.1f;
}
