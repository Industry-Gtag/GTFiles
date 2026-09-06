using System;
using System.Collections.Generic;
using GorillaTagScripts.GhostReactor;
using UnityEngine;

// Token: 0x0200078A RID: 1930
public class GREnemy : MonoBehaviour, IGameEntityComponent, IGameHittable
{
	// Token: 0x060030E0 RID: 12512 RVA: 0x00109143 File Offset: 0x00107343
	private void Awake()
	{
		this.damageFlash.Setup();
	}

	// Token: 0x060030E1 RID: 12513 RVA: 0x00109150 File Offset: 0x00107350
	public void OnEntityInit()
	{
		if (this.gameEntity != null)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.OnUpdate));
		}
	}

	// Token: 0x060030E2 RID: 12514 RVA: 0x00109150 File Offset: 0x00107350
	public void OnEntityDestroy()
	{
		if (this.gameEntity != null)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.OnUpdate));
		}
	}

	// Token: 0x060030E3 RID: 12515 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060030E4 RID: 12516 RVA: 0x00109188 File Offset: 0x00107388
	public static void HideRenderers(List<Renderer> renderers, bool hide)
	{
		if (renderers == null)
		{
			return;
		}
		for (int i = 0; i < renderers.Count; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x060030E5 RID: 12517 RVA: 0x001091CC File Offset: 0x001073CC
	public static void HideObjects(List<GameObject> objects, bool hide)
	{
		if (objects == null)
		{
			return;
		}
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i] != null)
			{
				objects[i].SetActive(!hide);
			}
		}
	}

	// Token: 0x060030E6 RID: 12518 RVA: 0x0010920D File Offset: 0x0010740D
	public void OnUpdate()
	{
		this.damageFlash.Update();
	}

	// Token: 0x060030E7 RID: 12519 RVA: 0x0010921A File Offset: 0x0010741A
	public void SetMaxHP(int maxHp)
	{
		if (this.healthMeter != null)
		{
			this.healthMeter.Setup(maxHp);
		}
	}

	// Token: 0x060030E8 RID: 12520 RVA: 0x00109236 File Offset: 0x00107436
	public void SetHP(int newHp)
	{
		if (this.healthMeter != null)
		{
			this.healthMeter.SetHP(newHp);
		}
	}

	// Token: 0x060030E9 RID: 12521 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x060030EA RID: 12522 RVA: 0x00109252 File Offset: 0x00107452
	public void OnHit(GameHitData hit)
	{
		if (hit.hitAmount > 0)
		{
			this.damageFlash.Play();
		}
	}

	// Token: 0x04003E84 RID: 16004
	public GRHealthMeter healthMeter;

	// Token: 0x04003E85 RID: 16005
	public GREnemyType enemyType;

	// Token: 0x04003E86 RID: 16006
	public GameEntity gameEntity;

	// Token: 0x04003E87 RID: 16007
	public GRDamageFlash damageFlash;
}
