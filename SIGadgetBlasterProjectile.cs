using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E1 RID: 225
[RequireComponent(typeof(SIGadgetProjectileType))]
[RequireComponent(typeof(Rigidbody))]
public class SIGadgetBlasterProjectile : MonoBehaviourTick
{
	// Token: 0x06000548 RID: 1352 RVA: 0x0001D9DD File Offset: 0x0001BBDD
	public override void Tick()
	{
		if (Time.time > this.timeSpawned + this.maxLifetime)
		{
			this.parentBlaster.DespawnProjectile(this);
		}
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x0001DA00 File Offset: 0x0001BC00
	public void InitializeProjectile()
	{
		this.rb.angularVelocity = Vector3.zero;
		this.rb.linearVelocity = base.transform.forward * this.startingVelocity;
		this.timeSpawned = Time.realtimeSinceStartup;
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponentInChildren<AudioSource>();
		}
		this.audioSource.time = 0f;
		this.projectileType = base.GetComponent<SIGadgetProjectileType>();
		SIGadgetProjectileModifier[] components = base.GetComponents<SIGadgetProjectileModifier>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].ModifyProjectile(this);
		}
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x0001DAA0 File Offset: 0x0001BCA0
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<SIExclusionZone>() != null && Time.realtimeSinceStartup > this.timeSpawned + 0.02f)
		{
			if (this.exclusionZoneDespawnEffect != null)
			{
				SIGadgetBlasterProjectile.SpawnExplosion(this.exclusionZoneDespawnEffect, base.transform.position, base.transform.rotation);
			}
			this.DespawnProjectile();
			return;
		}
		SIPlayer componentInParent = other.GetComponentInParent<SIPlayer>();
		if (componentInParent == null)
		{
			return;
		}
		if (componentInParent == this.firedByPlayer)
		{
			return;
		}
		if (this.firedByPlayer != SIPlayer.LocalPlayer || componentInParent == SIPlayer.LocalPlayer)
		{
			return;
		}
		this.projectileType.LocalProjectileHit(componentInParent);
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x0001DB54 File Offset: 0x0001BD54
	private void OnCollisionEnter(Collision collision)
	{
		this.projectileType.LocalProjectileHit(null);
		HitTargetNetworkState hitTargetNetworkState;
		if (collision.collider.gameObject.TryGetComponent<HitTargetNetworkState>(out hitTargetNetworkState))
		{
			hitTargetNetworkState.TargetHit((Time.time - this.timeSpawned) * this.startingVelocity * -base.transform.forward + base.transform.position, base.transform.position);
		}
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x0001DBCA File Offset: 0x0001BDCA
	public void DespawnProjectile()
	{
		this.parentBlaster.DespawnProjectile(this);
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x0001DBD8 File Offset: 0x0001BDD8
	public void KnockbackWithHaptics(Vector3 directionAndMagnitude, bool adjustForDirection = true)
	{
		this.KnockbackWithHaptics(directionAndMagnitude, this.hapticHitStrength, this.hapticHitDuration, adjustForDirection);
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x0001DBF0 File Offset: 0x0001BDF0
	public void KnockbackWithHaptics(Vector3 directionAndMagnitude, float hapticStrength, float hapticDuration, bool adjustForDirection = true)
	{
		SIPlayer.LocalPlayer.PlayerKnockback(directionAndMagnitude, true, true);
		SIPlayer.LocalPlayer.NotifyBlasterHit();
		if (adjustForDirection)
		{
			Vector3 vector = GorillaTagger.Instance.leftHandTransform.position - GorillaTagger.Instance.bodyCollider.transform.position;
			Vector3 vector2 = GorillaTagger.Instance.rightHandTransform.position - GorillaTagger.Instance.bodyCollider.transform.position;
			float num = 0.5f;
			float num2 = 45f;
			float num3 = Vector3.Angle(vector, directionAndMagnitude);
			float num4 = Vector3.Angle(vector2, directionAndMagnitude);
			float num5 = (1f - Mathf.Max(num3 - num2, 0f) / (180f - num2)) * num + (1f - num);
			float num6 = (1f - Mathf.Max(num4 - num2, 0f) / (180f - num2)) * num + (1f - num);
			SIPlayer.LocalPlayer.PlayerHandHaptic(true, num5, hapticDuration, true);
			SIPlayer.LocalPlayer.PlayerHandHaptic(false, num6, hapticDuration, true);
			return;
		}
		SIPlayer.LocalPlayer.PlayerHandHaptic(true, hapticStrength, hapticDuration, true);
		SIPlayer.LocalPlayer.PlayerHandHaptic(false, hapticStrength, hapticDuration, true);
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0001DD14 File Offset: 0x0001BF14
	public static GameObject SpawnExplosion(GameObject explosionPrefab, Vector3 position, Quaternion rotation)
	{
		if (SIGadgetBlasterProjectile.blasterProjectileExplosionPools == null)
		{
			SIGadgetBlasterProjectile.blasterProjectileExplosionPools = new Dictionary<int, List<GameObject>>();
		}
		if (SIGadgetBlasterProjectile.explosionTypeKey == null)
		{
			SIGadgetBlasterProjectile.explosionTypeKey = new Dictionary<GameObject, int>();
		}
		int instanceID = explosionPrefab.GetInstanceID();
		if (!SIGadgetBlasterProjectile.blasterProjectileExplosionPools.ContainsKey(instanceID))
		{
			SIGadgetBlasterProjectile.blasterProjectileExplosionPools.Add(instanceID, new List<GameObject>());
		}
		List<GameObject> list = SIGadgetBlasterProjectile.blasterProjectileExplosionPools[instanceID];
		GameObject gameObject;
		if (list.Count <= 0)
		{
			gameObject = Object.Instantiate<GameObject>(explosionPrefab, position, rotation);
			SIGadgetBlasterProjectile.explosionTypeKey.Add(gameObject, instanceID);
		}
		else
		{
			gameObject = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			gameObject.SetActive(true);
		}
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		return gameObject;
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x0001DDD0 File Offset: 0x0001BFD0
	public static void DespawnExplosion(GameObject explosion)
	{
		SIGadgetBlasterProjectile.blasterProjectileExplosionPools[SIGadgetBlasterProjectile.explosionTypeKey[explosion]].Add(explosion);
		explosion.SetActive(false);
	}

	// Token: 0x04000610 RID: 1552
	[OnEnterPlay_SetNull]
	public static Dictionary<int, List<GameObject>> blasterProjectileExplosionPools;

	// Token: 0x04000611 RID: 1553
	[OnEnterPlay_SetNull]
	public static Dictionary<GameObject, int> explosionTypeKey;

	// Token: 0x04000612 RID: 1554
	[NonSerialized]
	public int poolId;

	// Token: 0x04000613 RID: 1555
	public SIGadgetProjectileType projectileType;

	// Token: 0x04000614 RID: 1556
	public Rigidbody rb;

	// Token: 0x04000615 RID: 1557
	public GameObject hitEffect;

	// Token: 0x04000616 RID: 1558
	public GameObject hitEffectPlayer;

	// Token: 0x04000617 RID: 1559
	public float maxLifetime = 10f;

	// Token: 0x04000618 RID: 1560
	[NonSerialized]
	public float timeSpawned;

	// Token: 0x04000619 RID: 1561
	public float hapticHitStrength = 0.75f;

	// Token: 0x0400061A RID: 1562
	public float hapticHitDuration = 0.1f;

	// Token: 0x0400061B RID: 1563
	[NonSerialized]
	public SIGadgetBlaster parentBlaster;

	// Token: 0x0400061C RID: 1564
	[NonSerialized]
	public int projectileId;

	// Token: 0x0400061D RID: 1565
	[NonSerialized]
	public SIPlayer firedByPlayer;

	// Token: 0x0400061E RID: 1566
	public float startingVelocity;

	// Token: 0x0400061F RID: 1567
	public const float EXCLUSION_ZONE_MINIMUM_LIFETIME = 0.02f;

	// Token: 0x04000620 RID: 1568
	public GameObject exclusionZoneDespawnEffect;

	// Token: 0x04000621 RID: 1569
	private AudioSource audioSource;
}
