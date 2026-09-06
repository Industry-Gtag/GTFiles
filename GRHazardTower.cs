using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020007BD RID: 1981
public class GRHazardTower : MonoBehaviour, IGameEntityComponent, IGameProjectileLauncher
{
	// Token: 0x0600329F RID: 12959 RVA: 0x00115A90 File Offset: 0x00113C90
	public void OnEntityInit()
	{
		this.gameEntity.MinTimeBetweenTicks = 0.5f;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.OnThink));
		this.senseNearby.Setup(this.fireFrom, this.gameEntity);
	}

	// Token: 0x060032A0 RID: 12960 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060032A1 RID: 12961 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060032A2 RID: 12962 RVA: 0x00115AEC File Offset: 0x00113CEC
	public void OnThink()
	{
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		if (timeAsDouble < this.nextFireTime)
		{
			return;
		}
		GRHazardTower.tempRigs.Clear();
		GRHazardTower.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GRHazardTower.tempRigs);
		this.senseNearby.UpdateNearby(GRHazardTower.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		if (vrrig == null)
		{
			return;
		}
		Vector3 vector = vrrig.transform.position;
		Vector3 vector2 = Vector3.up * 0.1f;
		vector += vector2;
		GhostReactorManager.Get(this.gameEntity).RequestFireProjectile(this.gameEntity.id, this.fireFrom.position, vector, PhotonNetwork.Time + 0.0);
		this.nextFireTime = timeAsDouble + (double)this.fireCooldownTime;
	}

	// Token: 0x060032A3 RID: 12963 RVA: 0x00115BD8 File Offset: 0x00113DD8
	public void OnFire(Vector3 fireFromPos, Vector3 fireAtPos, double fireAtTime)
	{
		Vector3 vector;
		if (this.gameEntity.IsAuthority() && GREnemyRanged.CalculateLaunchDirection(fireFromPos, fireAtPos, this.projectileSpeed, out vector))
		{
			this.gameEntity.manager.RequestCreateItem(this.projectilePrefab.name.GetStaticHash(), fireFromPos, Quaternion.LookRotation(vector, Vector3.up), (long)this.gameEntity.GetNetId());
		}
		double timeAsDouble = Time.timeAsDouble;
		this.nextFireTime = timeAsDouble + (double)this.fireCooldownTime;
	}

	// Token: 0x060032A4 RID: 12964 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnProjectileInit(GRRangedEnemyProjectile projectile)
	{
	}

	// Token: 0x060032A5 RID: 12965 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnProjectileHit(GRRangedEnemyProjectile projectile, Collision collision)
	{
	}

	// Token: 0x040041A1 RID: 16801
	public GameEntity gameEntity;

	// Token: 0x040041A2 RID: 16802
	public GRSenseNearby senseNearby;

	// Token: 0x040041A3 RID: 16803
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x040041A4 RID: 16804
	public float projectileSpeed;

	// Token: 0x040041A5 RID: 16805
	public GameEntity projectilePrefab;

	// Token: 0x040041A6 RID: 16806
	public Transform fireFrom;

	// Token: 0x040041A7 RID: 16807
	public float fireChargeTime;

	// Token: 0x040041A8 RID: 16808
	public float fireCooldownTime;

	// Token: 0x040041A9 RID: 16809
	private double nextFireTime;

	// Token: 0x040041AA RID: 16810
	private static List<VRRig> tempRigs = new List<VRRig>(16);
}
