using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200076F RID: 1903
public class GRDebugUpgradeKiosk : MonoBehaviour
{
	// Token: 0x06003029 RID: 12329 RVA: 0x00105A27 File Offset: 0x00103C27
	public void Init(GhostReactorManager grManager, GhostReactor reactor)
	{
		this.grManager = grManager;
		this.reactor = reactor;
	}

	// Token: 0x0600302A RID: 12330 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Start()
	{
	}

	// Token: 0x0600302B RID: 12331 RVA: 0x00105A37 File Offset: 0x00103C37
	public void OnButtonSpawnClub()
	{
		this.OnButtonSpawnEntity("GhostReactorToolClub", this.toolSpawnNode);
	}

	// Token: 0x0600302C RID: 12332 RVA: 0x00105A4A File Offset: 0x00103C4A
	public void OnButtonSpawnCollector()
	{
		this.OnButtonSpawnEntity("GhostReactorToolCollector", this.toolSpawnNode);
	}

	// Token: 0x0600302D RID: 12333 RVA: 0x00105A5D File Offset: 0x00103C5D
	public void OnButtonSpawnLantern()
	{
		this.OnButtonSpawnEntity("GhostReactorToolLantern", this.toolSpawnNode);
	}

	// Token: 0x0600302E RID: 12334 RVA: 0x00105A70 File Offset: 0x00103C70
	public void OnButtonSpawnFlash()
	{
		this.OnButtonSpawnEntity("GhostReactorToolFlash", this.toolSpawnNode);
	}

	// Token: 0x0600302F RID: 12335 RVA: 0x00105A83 File Offset: 0x00103C83
	public void OnButtonSpawnShieldGun()
	{
		this.OnButtonSpawnEntity("GhostReactorToolShieldGun", this.toolSpawnNode);
	}

	// Token: 0x06003030 RID: 12336 RVA: 0x00105A96 File Offset: 0x00103C96
	public void OnButtonSpawnRevive()
	{
		this.OnButtonSpawnEntity("GhostReactorToolRevive", this.toolSpawnNode);
	}

	// Token: 0x06003031 RID: 12337 RVA: 0x00105AA9 File Offset: 0x00103CA9
	public void OnButtonSpawnDirectionalShield()
	{
		this.OnButtonSpawnEntity("GhostReactorToolDirectionalShield", this.toolSpawnNode);
	}

	// Token: 0x06003032 RID: 12338 RVA: 0x00105ABC File Offset: 0x00103CBC
	public void OnButtonSpawnStatusWatch()
	{
		this.OnButtonSpawnEntity("GhostReactorToolStatusWatch", this.toolSpawnNode);
	}

	// Token: 0x06003033 RID: 12339 RVA: 0x00105ACF File Offset: 0x00103CCF
	public void OnButtonSpawnDockWrist()
	{
		this.OnButtonSpawnEntity("GhostReactorToolDockWrist", this.toolSpawnNode);
	}

	// Token: 0x06003034 RID: 12340 RVA: 0x00105AE2 File Offset: 0x00103CE2
	public void OnButtonSpawnSmallBackpack()
	{
		this.OnButtonSpawnEntity("GhostReactorToolSmallBackpack", this.toolSpawnNode);
	}

	// Token: 0x06003035 RID: 12341 RVA: 0x00105AF5 File Offset: 0x00103CF5
	public void OnButtonKillAllEnemies()
	{
		this.KillAllEnemies();
	}

	// Token: 0x06003036 RID: 12342 RVA: 0x00105AFD File Offset: 0x00103CFD
	public void OnButtonSpawnPest()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyPest", this.enemySpawnNode);
	}

	// Token: 0x06003037 RID: 12343 RVA: 0x00105B10 File Offset: 0x00103D10
	public void OnButtonSpawnChaser()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyChaser", this.enemySpawnNode);
	}

	// Token: 0x06003038 RID: 12344 RVA: 0x00105B23 File Offset: 0x00103D23
	public void OnButtonSpawnPhantom()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyPhantom", this.enemySpawnNode);
	}

	// Token: 0x06003039 RID: 12345 RVA: 0x00105B36 File Offset: 0x00103D36
	public void OnButtonSpawnRanged()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyRanged", this.enemySpawnNode);
	}

	// Token: 0x0600303A RID: 12346 RVA: 0x00105B49 File Offset: 0x00103D49
	public void OnButtonSpawnSummoner()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemySummoner", this.enemySpawnNode);
	}

	// Token: 0x0600303B RID: 12347 RVA: 0x00105B5C File Offset: 0x00103D5C
	public void OnButtonSpawnIceRanged()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyRangedIce", this.enemySpawnNode);
	}

	// Token: 0x0600303C RID: 12348 RVA: 0x00105B6F File Offset: 0x00103D6F
	public void OnButtonSpawnUpgEff1()
	{
		this.OnButtonSpawnEntity("GRUPowerEff1", this.upgradeSpawnNode);
	}

	// Token: 0x0600303D RID: 12349 RVA: 0x00105B82 File Offset: 0x00103D82
	public void OnButtonSpawnUpgEff2()
	{
		this.OnButtonSpawnEntity("GRUPowerEff2", this.upgradeSpawnNode);
	}

	// Token: 0x0600303E RID: 12350 RVA: 0x00105B95 File Offset: 0x00103D95
	public void OnButtonSpawnUpgEff3()
	{
		this.OnButtonSpawnEntity("GRUPowerEff3", this.upgradeSpawnNode);
	}

	// Token: 0x0600303F RID: 12351 RVA: 0x00105BA8 File Offset: 0x00103DA8
	public void OnButtonSpawnUpgBatonDmg1()
	{
		this.OnButtonSpawnEntity("GRUBatonDamage1", this.upgradeSpawnNode);
	}

	// Token: 0x06003040 RID: 12352 RVA: 0x00105BBB File Offset: 0x00103DBB
	public void OnButtonSpawnUpgBatonDmg2()
	{
		this.OnButtonSpawnEntity("GRUBatonDamage2", this.upgradeSpawnNode);
	}

	// Token: 0x06003041 RID: 12353 RVA: 0x00105BCE File Offset: 0x00103DCE
	public void OnButtonSpawnUpgBatonDmg3()
	{
		this.OnButtonSpawnEntity("GRUBatonDamage3", this.upgradeSpawnNode);
	}

	// Token: 0x06003042 RID: 12354 RVA: 0x00105B6F File Offset: 0x00103D6F
	public void OnButtonSpawnUpgEfficiency1()
	{
		this.OnButtonSpawnEntity("GRUPowerEff1", this.upgradeSpawnNode);
	}

	// Token: 0x06003043 RID: 12355 RVA: 0x00105B82 File Offset: 0x00103D82
	public void OnButtonSpawnUpgEfficiency2()
	{
		this.OnButtonSpawnEntity("GRUPowerEff2", this.upgradeSpawnNode);
	}

	// Token: 0x06003044 RID: 12356 RVA: 0x00105B95 File Offset: 0x00103D95
	public void OnButtonSpawnUpgEfficiency3()
	{
		this.OnButtonSpawnEntity("GRUPowerEff3", this.upgradeSpawnNode);
	}

	// Token: 0x06003045 RID: 12357 RVA: 0x00105BE1 File Offset: 0x00103DE1
	public void OnButtonSpawnChaosSeed()
	{
		this.OnButtonSpawnEntity("GhostReactorCollectibleSentientCore", this.enemySpawnNode);
	}

	// Token: 0x06003046 RID: 12358 RVA: 0x00105BF4 File Offset: 0x00103DF4
	public void OnButtonSpawnEntity(string entityName, Transform location)
	{
		if (location == null)
		{
			return;
		}
		Debug.Log("GRDebugUpgradeKiosk attempting to spawn " + entityName);
		int staticHash = entityName.GetStaticHash();
		GameEntityId gameEntityId = this.grManager.gameEntityManager.RequestCreateItem(staticHash, location.position, Quaternion.identity, 0L);
		GameAgent component = this.grManager.gameEntityManager.GetGameEntity(gameEntityId).gameObject.GetComponent<GameAgent>();
		if (component != null)
		{
			if (entityName.Contains("enemy", StringComparison.OrdinalIgnoreCase))
			{
				GhostReactorManager.entityDebugEnabled = true;
			}
			this.spawnedEntities.Add(gameEntityId);
			component.ApplyDestination(location.position);
			return;
		}
		Debug.Log("GRDebugUpgradeKiosk failed to spawn " + entityName);
	}

	// Token: 0x06003047 RID: 12359 RVA: 0x00105CA4 File Offset: 0x00103EA4
	public void KillAllEnemies()
	{
		foreach (GameEntityId gameEntityId in this.spawnedEntities)
		{
			this.grManager.gameEntityManager.RequestDestroyItem(gameEntityId);
		}
		this.spawnedEntities.Clear();
	}

	// Token: 0x04003DB3 RID: 15795
	public Transform upgradeSpawnNode;

	// Token: 0x04003DB4 RID: 15796
	public Transform toolSpawnNode;

	// Token: 0x04003DB5 RID: 15797
	public Transform enemySpawnNode;

	// Token: 0x04003DB6 RID: 15798
	private GhostReactorManager grManager;

	// Token: 0x04003DB7 RID: 15799
	private GhostReactor reactor;

	// Token: 0x04003DB8 RID: 15800
	private List<GameEntityId> spawnedEntities = new List<GameEntityId>();
}
