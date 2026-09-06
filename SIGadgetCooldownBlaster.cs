using System;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class SIGadgetCooldownBlaster : MonoBehaviour, SIGadgetBlasterType
{
	// Token: 0x06000560 RID: 1376 RVA: 0x0001E496 File Offset: 0x0001C696
	private bool CheckInput()
	{
		return this.blaster.CheckInput();
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x0001E4A4 File Offset: 0x0001C6A4
	private void OnEnable()
	{
		this.blaster = base.GetComponent<SIGadgetBlaster>();
		this.blaster.firingSource.clip = this.firingClip;
		this.blaster.firingSource.volume = this.firingVolume;
		this.blaster.firingSource.loop = false;
		this.blaster.blasterSource.clip = this.cooldownClip;
		this.blaster.blasterSource.volume = this.cooldownVolume;
		this.blaster.blasterSource.loop = false;
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x0001E538 File Offset: 0x0001C738
	public void OnUpdateAuthority(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle)
		{
			if (currentState != SIGadgetBlasterState.Cooldown)
			{
				return;
			}
			if (Time.time >= this.blaster.lastFired + this.fireCooldown)
			{
				this.blaster.FireProjectileHaptics(this.availableToFireHapticStrength, 0.02f);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			}
		}
		else
		{
			if (!this.CheckInput())
			{
				this.triggerHeldDown = false;
				return;
			}
			if (!this.triggerHeldDown)
			{
				this.triggerHeldDown = true;
				this.FireProjectile(this.blaster.NextFireId(), this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Cooldown);
				return;
			}
		}
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x0001E5F4 File Offset: 0x0001C7F4
	public void OnUpdateRemote(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle)
		{
		}
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x0001E614 File Offset: 0x0001C814
	public void SetStateShared()
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState == SIGadgetBlasterState.Idle)
		{
			this.cooldownIndicator.sharedMaterial = this.readyToFireMaterial;
			return;
		}
		if (currentState != SIGadgetBlasterState.Cooldown)
		{
			return;
		}
		this.blaster.lastFired = Time.time;
		this.cooldownIndicator.sharedMaterial = this.onCooldownMaterial;
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x0001E668 File Offset: 0x0001C868
	public void FireProjectile(int fireId, Vector3 position, Quaternion rotation)
	{
		if (this.blaster.projectileCount > this.blaster.maxProjectileCount)
		{
			return;
		}
		if (this.blaster.LocalEquippedOrActivated)
		{
			if (Time.time < this.blaster.lastFired + this.fireCooldown)
			{
				return;
			}
			this.blaster.FireProjectileHaptics(this.firingHapticStrength, this.firingHapticDuration);
			this.blaster.SendClientToClientRPC(0, new object[] { fireId, position, rotation });
		}
		this.blaster.firingSource.time = 0f;
		this.blaster.firingSource.Play();
		this.blaster.blasterSource.time = 0f;
		this.blaster.blasterSource.Play();
		this.blaster.InstantiateProjectile(this.projectilePrefab, position, rotation, fireId);
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x0001E758 File Offset: 0x0001C958
	public void NetworkFireProjectile(object[] data)
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
		Quaternion quaternion;
		if (!GameEntityManager.ValidateDataType<Quaternion>(data[2], out quaternion))
		{
			return;
		}
		if ((vector - this.blaster.firingPosition.position).magnitude > this.blaster.maxLagDistance)
		{
			return;
		}
		if (this.blaster.CurrentFireRate() > 1f / this.fireCooldown * (1f + this.fireRateGracePercentage))
		{
			return;
		}
		this.FireProjectile(num, vector, quaternion);
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	// Token: 0x04000634 RID: 1588
	public SIGadgetBlasterProjectile projectilePrefab;

	// Token: 0x04000635 RID: 1589
	public float fireCooldown = 0.5f;

	// Token: 0x04000636 RID: 1590
	public float fireRateGracePercentage = 0.25f;

	// Token: 0x04000637 RID: 1591
	public float availableToFireHapticStrength = 0.1f;

	// Token: 0x04000638 RID: 1592
	public float availableToFireHapticDuration = 0.01f;

	// Token: 0x04000639 RID: 1593
	public float firingHapticStrength = 0.25f;

	// Token: 0x0400063A RID: 1594
	public float firingHapticDuration = 0.01f;

	// Token: 0x0400063B RID: 1595
	public AudioClip firingClip;

	// Token: 0x0400063C RID: 1596
	public AudioClip cooldownClip;

	// Token: 0x0400063D RID: 1597
	public float firingVolume;

	// Token: 0x0400063E RID: 1598
	public float cooldownVolume;

	// Token: 0x0400063F RID: 1599
	public ParticleSystem fireFX;

	// Token: 0x04000640 RID: 1600
	public MeshRenderer cooldownIndicator;

	// Token: 0x04000641 RID: 1601
	public Material readyToFireMaterial;

	// Token: 0x04000642 RID: 1602
	public Material onCooldownMaterial;

	// Token: 0x04000643 RID: 1603
	private bool triggerHeldDown;

	// Token: 0x04000644 RID: 1604
	private SIGadgetBlaster blaster;
}
