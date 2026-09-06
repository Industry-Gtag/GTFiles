using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000DB RID: 219
public class SIBlasterSplashProjectile : MonoBehaviour, SIGadgetProjectileType
{
	// Token: 0x06000522 RID: 1314 RVA: 0x0001CB9F File Offset: 0x0001AD9F
	private void OnEnable()
	{
		this.projectile = base.GetComponent<SIGadgetBlasterProjectile>();
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x0001CBB0 File Offset: 0x0001ADB0
	public void LocalProjectileHit(SIPlayer player = null)
	{
		if (!this.projectile.firedByPlayer == SIPlayer.LocalPlayer)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, this.projectile.transform.position, this.projectile.transform.rotation);
			this.projectile.DespawnProjectile();
			return;
		}
		this.rigList.Clear();
		VRRigCache.Instance.GetActiveRigs(this.rigList);
		Vector3 position = this.projectile.transform.position;
		for (int i = this.rigList.Count - 1; i >= 0; i--)
		{
			if ((this.rigList[i].transform.position - position).magnitude < this.splashHitDistance)
			{
				Vector3 position2 = this.rigList[i].head.rigTarget.position;
				Vector3 position3 = this.rigList[i].bodyTransform.position;
				if (Physics.RaycastNonAlloc(position, position2 - position, this.hits, this.splashHitDistance, this.projectile.parentBlaster.environmentLayerMask, QueryTriggerInteraction.Ignore) != 0 && Physics.RaycastNonAlloc(position, position3 - position, this.hits, this.splashHitDistance, this.projectile.parentBlaster.environmentLayerMask, QueryTriggerInteraction.Ignore) != 0)
				{
					this.rigList.RemoveAt(i);
				}
			}
			else
			{
				this.rigList.RemoveAt(i);
			}
		}
		if (this.rigList.Count <= 0)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, this.projectile.transform.position, this.projectile.transform.rotation);
			this.projectile.DespawnProjectile();
			return;
		}
		SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffectPlayer, this.projectile.transform.position, this.projectile.transform.rotation);
		this.TriggerSplashHitPlayers(this.rigList);
		this.projectile.DespawnProjectile();
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x0001CDD8 File Offset: 0x0001AFD8
	public void TriggerSplashHitPlayers(List<VRRig> hitPlayers)
	{
		int[] array = new int[hitPlayers.Count];
		Vector3[] array2 = new Vector3[hitPlayers.Count];
		for (int i = 0; i < hitPlayers.Count; i++)
		{
			array[i] = ((hitPlayers[i] != null && hitPlayers[i].OwningNetPlayer != null) ? hitPlayers[i].OwningNetPlayer.ActorNumber : (-1));
			Vector3 vector = hitPlayers[i].transform.position - this.projectile.transform.position;
			float num = Mathf.Max(0f, 1f - Mathf.Max(0f, vector.magnitude - this.fullSplashRadius) / (this.splashHitDistance - this.fullSplashRadius));
			array2[i] = vector.normalized * this.knockbackSpeed * num;
			if (hitPlayers[i] != null && hitPlayers[i].isLocal && num > 0f)
			{
				this.SplashHitLocalPlayer(array2[i]);
			}
		}
		this.projectile.parentBlaster.SendClientToClientRPC(1, new object[]
		{
			this.projectile.projectileId,
			base.transform.position,
			array,
			array2
		});
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x0001CF40 File Offset: 0x0001B140
	public void NetworkedProjectileHit(object[] data)
	{
		if (data == null || data.Length != 4)
		{
			return;
		}
		int num;
		if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
		{
			return;
		}
		Vector3 vector;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[1], out vector))
		{
			return;
		}
		if (!vector.IsFinite())
		{
			return;
		}
		int[] array;
		if (!GameEntityManager.ValidateDataType<int[]>(data[2], out array))
		{
			return;
		}
		Vector3[] array2;
		if (!GameEntityManager.ValidateDataType<Vector3[]>(data[3], out array2))
		{
			return;
		}
		for (int i = 0; i < array2.Length; i++)
		{
			if (!array2[i].IsFinite())
			{
				return;
			}
		}
		if (array.Length > VRRigCache.Instance.GetAllRigs().Length)
		{
			return;
		}
		if (array.Length != array2.Length)
		{
			return;
		}
		if ((base.transform.position - vector).magnitude > this.projectile.parentBlaster.maxLagDistance)
		{
			return;
		}
		this.projectile.DespawnProjectile();
		bool flag = false;
		for (int j = 0; j < array.Length; j++)
		{
			SIPlayer siplayer = SIPlayer.Get(array[j]);
			if (siplayer != null && siplayer == SIPlayer.LocalPlayer)
			{
				flag = true;
				this.SplashHitLocalPlayer(array2[j]);
			}
		}
		if (flag)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffectPlayer, vector, base.transform.rotation);
			return;
		}
		SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, vector, base.transform.rotation);
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x0001D094 File Offset: 0x0001B294
	public void SplashHitLocalPlayer(Vector3 directionAndMagnitude)
	{
		if (directionAndMagnitude.magnitude > this.knockbackSpeed * 1.05f)
		{
			return;
		}
		SIPlayer.LocalPlayer.NotifyBlasterSplashHit();
		float num = Vector3.Angle(directionAndMagnitude.normalized, Vector3.up);
		Vector3 vector = Vector3.RotateTowards(directionAndMagnitude.normalized, Vector3.up, Mathf.Clamp(num - this.upwardsAngle, 0f, this.upwardsAngle) * 0.017453292f, 0f);
		this.projectile.KnockbackWithHaptics(vector * directionAndMagnitude.magnitude, directionAndMagnitude.magnitude / this.knockbackSpeed * this.projectile.hapticHitStrength, this.projectile.hapticHitDuration, true);
	}

	// Token: 0x040005E7 RID: 1511
	public float knockbackSpeed;

	// Token: 0x040005E8 RID: 1512
	public float fullSplashRadius;

	// Token: 0x040005E9 RID: 1513
	public float splashHitDistance;

	// Token: 0x040005EA RID: 1514
	public float upwardsAngle = 30f;

	// Token: 0x040005EB RID: 1515
	private SIGadgetBlasterProjectile projectile;

	// Token: 0x040005EC RID: 1516
	private List<VRRig> rigList = new List<VRRig>();

	// Token: 0x040005ED RID: 1517
	private RaycastHit[] hits = new RaycastHit[20];
}
