using System;
using UnityEngine;

// Token: 0x020000D7 RID: 215
public class SIBlasterDirectHitProjectile : MonoBehaviour, SIGadgetProjectileType
{
	// Token: 0x06000517 RID: 1303 RVA: 0x0001C7BD File Offset: 0x0001A9BD
	private void OnEnable()
	{
		this.projectile = base.GetComponent<SIGadgetBlasterProjectile>();
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x0001C7CC File Offset: 0x0001A9CC
	public void LocalProjectileHit(SIPlayer player = null)
	{
		if (player != null && this.projectile.hitEffectPlayer != null)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffectPlayer, this.projectile.transform.position, this.projectile.transform.rotation);
		}
		if (player == null && this.projectile.hitEffect != null)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, this.projectile.transform.position, this.projectile.transform.rotation);
		}
		if (player != null)
		{
			this.TriggerBlastDirectHitPlayer(player);
		}
		this.projectile.DespawnProjectile();
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x0001C890 File Offset: 0x0001AA90
	public void TriggerBlastDirectHitPlayer(SIPlayer playerHit)
	{
		if (playerHit == SIPlayer.LocalPlayer)
		{
			return;
		}
		this.projectile.parentBlaster.SendClientToClientRPC(1, new object[]
		{
			this.projectile.projectileId,
			base.transform.position,
			playerHit.ActorNr
		});
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x0001C8F8 File Offset: 0x0001AAF8
	public void NetworkedProjectileHit(object[] data)
	{
		if (data == null || data.Length != 3)
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
		int num2;
		if (!GameEntityManager.ValidateDataType<int>(data[2], out num2))
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
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, vector, this.projectile.transform.rotation);
			return;
		}
		SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffectPlayer, vector, this.projectile.transform.rotation);
		float num3 = Vector3.Angle(base.transform.forward, Vector3.up);
		Vector3 vector2 = Vector3.RotateTowards(base.transform.forward.normalized, Vector3.up, Mathf.Clamp(num3 - this.upwardsAngle, 0f, this.upwardsAngle) * 0.017453292f, 0f);
		this.projectile.KnockbackWithHaptics(vector2.normalized * this.knockbackSpeed, true);
	}

	// Token: 0x040005DF RID: 1503
	private SIGadgetBlasterProjectile projectile;

	// Token: 0x040005E0 RID: 1504
	public float knockbackSpeed;

	// Token: 0x040005E1 RID: 1505
	public float upwardsAngle = 30f;
}
