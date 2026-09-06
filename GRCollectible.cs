using System;
using UnityEngine;

// Token: 0x02000768 RID: 1896
public class GRCollectible : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06003005 RID: 12293 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Awake()
	{
	}

	// Token: 0x06003006 RID: 12294 RVA: 0x001053A8 File Offset: 0x001035A8
	public void OnEntityInit()
	{
		GameEntityManager manager = this.entity.manager;
		GameEntity gameEntity = manager.GetGameEntity(manager.GetEntityIdFromNetId((int)this.entity.createData));
		if (gameEntity != null)
		{
			GRCollectibleDispenser component = gameEntity.GetComponent<GRCollectibleDispenser>();
			if (component != null)
			{
				component.GetSpawnedCollectible(this);
			}
		}
	}

	// Token: 0x06003007 RID: 12295 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06003008 RID: 12296 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06003009 RID: 12297 RVA: 0x001053F8 File Offset: 0x001035F8
	public void InvokeOnCollected()
	{
		Action onCollected = this.OnCollected;
		if (onCollected == null)
		{
			return;
		}
		onCollected();
	}

	// Token: 0x04003D8E RID: 15758
	public GameEntity entity;

	// Token: 0x04003D8F RID: 15759
	public int energyValue = 100;

	// Token: 0x04003D90 RID: 15760
	public ProgressionManager.CoreType type;

	// Token: 0x04003D91 RID: 15761
	public Action OnCollected;
}
