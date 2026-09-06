using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006E5 RID: 1765
public class GameHittable : MonoBehaviour
{
	// Token: 0x06002C6D RID: 11373 RVA: 0x000F0054 File Offset: 0x000EE254
	private void Awake()
	{
		this.components = new List<IGameHittable>(1);
		base.GetComponentsInChildren<IGameHittable>(this.components);
		for (int i = 0; i < this.hittablePoints.Count; i++)
		{
			this.hittablePoints[i].damageFlash.Setup();
		}
	}

	// Token: 0x06002C6E RID: 11374 RVA: 0x000F00A5 File Offset: 0x000EE2A5
	private void OnEnable()
	{
		if (this.gameEntity != null)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.OnUpdate));
		}
	}

	// Token: 0x06002C6F RID: 11375 RVA: 0x000F00DC File Offset: 0x000EE2DC
	private void OnDisable()
	{
		if (this.gameEntity != null)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnTick = (Action)Delegate.Remove(gameEntity.OnTick, new Action(this.OnUpdate));
		}
	}

	// Token: 0x06002C70 RID: 11376 RVA: 0x000F0114 File Offset: 0x000EE314
	public void OnUpdate()
	{
		for (int i = 0; i < this.hittablePoints.Count; i++)
		{
			this.hittablePoints[i].damageFlash.Update();
		}
	}

	// Token: 0x06002C71 RID: 11377 RVA: 0x000F014D File Offset: 0x000EE34D
	public void RequestHit(GameHitData hitData)
	{
		hitData.hitEntityId = this.gameEntity.id;
		this.gameEntity.manager.RequestHit(hitData);
	}

	// Token: 0x06002C72 RID: 11378 RVA: 0x000F0174 File Offset: 0x000EE374
	public void ApplyHit(GameHitData hitData)
	{
		for (int i = 0; i < this.components.Count; i++)
		{
			this.components[i].OnHit(hitData);
		}
		GameHitter component = this.gameEntity.manager.GetGameEntity(hitData.hitByEntityId).GetComponent<GameHitter>();
		if (component != null)
		{
			component.ApplyHit(hitData);
		}
		GameHittable.HittablePoint hittablePoint = this.GetHittablePoint(hitData.hittablePoint);
		if (hittablePoint != null)
		{
			hittablePoint.damageFlash.Play();
		}
	}

	// Token: 0x06002C73 RID: 11379 RVA: 0x000F01F0 File Offset: 0x000EE3F0
	private GameHittable.HittablePoint GetHittablePoint(int hittablePoint)
	{
		if (hittablePoint < 0 || hittablePoint >= this.hittablePoints.Count)
		{
			return null;
		}
		return this.hittablePoints[hittablePoint];
	}

	// Token: 0x06002C74 RID: 11380 RVA: 0x000F0214 File Offset: 0x000EE414
	public bool IsHitValid(GameHitData hitData)
	{
		for (int i = 0; i < this.components.Count; i++)
		{
			if (!this.components[i].IsHitValid(hitData))
			{
				return false;
			}
		}
		return this.hittablePoints.Count <= 0 || (hitData.hittablePoint >= 0 && hitData.hittablePoint < this.hittablePoints.Count);
	}

	// Token: 0x06002C75 RID: 11381 RVA: 0x000F027C File Offset: 0x000EE47C
	public int FindHittablePoint(Collider collider)
	{
		if (this.hittablePoints == null || this.hittablePoints.Count == 0)
		{
			return 0;
		}
		for (int i = 0; i < this.hittablePoints.Count; i++)
		{
			if (this.hittablePoints[i].colliders.Contains(collider))
			{
				return i;
			}
		}
		return 0;
	}

	// Token: 0x06002C76 RID: 11382 RVA: 0x000F02D4 File Offset: 0x000EE4D4
	public bool IsColliderValid(Collider collider)
	{
		if (this.hittablePoints == null || this.hittablePoints.Count == 0)
		{
			return true;
		}
		for (int i = 0; i < this.hittablePoints.Count; i++)
		{
			if (this.hittablePoints[i].colliders.Contains(collider))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040038EE RID: 14574
	public GameEntity gameEntity;

	// Token: 0x040038EF RID: 14575
	public List<GameHittable.HittablePoint> hittablePoints;

	// Token: 0x040038F0 RID: 14576
	private List<IGameHittable> components;

	// Token: 0x020006E6 RID: 1766
	[Serializable]
	public class HittablePoint
	{
		// Token: 0x040038F1 RID: 14577
		public List<Collider> colliders;

		// Token: 0x040038F2 RID: 14578
		public GRDamageFlash damageFlash;
	}
}
