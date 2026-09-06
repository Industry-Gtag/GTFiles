using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000DC RID: 220
public class SIBlasterSprayProjectile : MonoBehaviour
{
	// Token: 0x06000528 RID: 1320 RVA: 0x0001D171 File Offset: 0x0001B371
	private void OnEnable()
	{
		this.projectile = base.GetComponent<SIGadgetBlasterProjectile>();
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x0001D180 File Offset: 0x0001B380
	public void LocalProjectileHit(SIPlayer player = null)
	{
		if (player != null && this.projectile.hitEffectPlayer != null)
		{
			Object.Instantiate<GameObject>(this.projectile.hitEffectPlayer, this.projectile.transform.position, this.projectile.transform.rotation);
		}
		if (player == null && this.projectile.hitEffect != null)
		{
			Object.Instantiate<GameObject>(this.projectile.hitEffect, this.projectile.transform.position, this.projectile.transform.rotation);
		}
		if (player != null)
		{
			this.TriggerBlastDirectHitPlayer(player);
		}
		this.projectile.DespawnProjectile();
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x0001D244 File Offset: 0x0001B444
	public void TriggerBlastDirectHitPlayer(SIPlayer playerHit)
	{
		if (playerHit == SIPlayer.LocalPlayer)
		{
			return;
		}
		float num = Vector3.Angle(base.transform.forward, Vector3.up);
		Vector3 vector = Vector3.RotateTowards(base.transform.forward.normalized, Vector3.up, Mathf.Clamp(num - this.upwardsAngle, 0f, this.upwardsAngle) * 0.017453292f, 0f);
		this.projectile.parentBlaster.SendClientToClientRPC(1, new object[]
		{
			this.projectile.projectileId,
			base.transform.position,
			vector,
			playerHit.ActorNr
		});
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x0001D30C File Offset: 0x0001B50C
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
		Vector3 vector2;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[2], out vector2))
		{
			return;
		}
		int num2;
		if (!GameEntityManager.ValidateDataType<int>(data[3], out num2))
		{
			return;
		}
		if ((base.transform.position - vector).magnitude > this.projectile.parentBlaster.maxLagDistance)
		{
			return;
		}
		this.projectile.DespawnProjectile();
		SIPlayer siplayer = SIPlayer.Get(num2);
		if (siplayer == null)
		{
			return;
		}
		if (siplayer != SIPlayer.LocalPlayer)
		{
			Object.Instantiate<GameObject>(this.projectile.hitEffect, vector, this.projectile.transform.rotation);
			return;
		}
		Object.Instantiate<GameObject>(this.projectile.hitEffectPlayer, vector, this.projectile.transform.rotation);
		GTPlayer.Instance.ApplyKnockback(vector2.normalized, this.knockbackSpeed, true);
	}

	// Token: 0x040005EE RID: 1518
	private SIGadgetBlasterProjectile projectile;

	// Token: 0x040005EF RID: 1519
	public float knockbackSpeed;

	// Token: 0x040005F0 RID: 1520
	public float verticalOffset = -0.133f;

	// Token: 0x040005F1 RID: 1521
	public float upwardsAngle = 30f;
}
