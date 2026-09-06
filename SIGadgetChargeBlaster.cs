using System;
using UnityEngine;

// Token: 0x020000E4 RID: 228
public class SIGadgetChargeBlaster : MonoBehaviour, SIGadgetBlasterType
{
	// Token: 0x06000555 RID: 1365 RVA: 0x0001DE1D File Offset: 0x0001C01D
	private bool CheckInput()
	{
		return this.blaster.CheckInput();
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x0001DE2A File Offset: 0x0001C02A
	private void OnEnable()
	{
		this.blaster = base.GetComponent<SIGadgetBlaster>();
		this.currentCharge = 0f;
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x0001DE44 File Offset: 0x0001C044
	public void OnUpdateAuthority(float dt)
	{
		switch (this.blaster.currentState)
		{
		case SIGadgetBlasterState.Idle:
			if (this.CheckInput())
			{
				this.FireProjectile(0f, this.blaster.NextFireId(), this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Charging);
				return;
			}
			break;
		case SIGadgetBlasterState.Charging:
			this.currentCharge += this.chargeRatePerSecond * Time.deltaTime;
			this.UpdateChargingVisuals();
			if (this.CheckInput())
			{
				this.blaster.FireProjectileHaptics(this.chargeLevels[this.CurrentBlasterChargeLevel()].chargingHapticStrength, Time.fixedDeltaTime);
				return;
			}
			if (this.CurrentBlasterChargeLevel() > 0)
			{
				this.FireProjectile(this.currentCharge, this.blaster.NextFireId(), this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Cooldown);
				return;
			}
			this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			return;
		case SIGadgetBlasterState.Cooldown:
			if (Time.time >= this.blaster.lastFired + this.fireCooldown)
			{
				if (this.CheckInput())
				{
					this.blaster.SetStateAuthority(SIGadgetBlasterState.Charging);
					return;
				}
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x0001DFA0 File Offset: 0x0001C1A0
	public void OnUpdateRemote(float dt)
	{
		switch (this.blaster.currentState)
		{
		case SIGadgetBlasterState.Idle:
		case SIGadgetBlasterState.Cooldown:
			break;
		case SIGadgetBlasterState.Charging:
			this.currentCharge += this.chargeRatePerSecond * Time.deltaTime;
			this.UpdateChargingVisuals();
			break;
		default:
			return;
		}
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0001DFEC File Offset: 0x0001C1EC
	public void SetStateShared()
	{
		switch (this.blaster.currentState)
		{
		case SIGadgetBlasterState.Idle:
			this.currentCharge = 0f;
			break;
		case SIGadgetBlasterState.Charging:
			this.currentCharge = 0f;
			this.blaster.blasterSource.clip = this.chargingClip;
			this.blaster.blasterSource.volume = this.chargeLevels[0].chargingVolume;
			this.blaster.blasterSource.loop = true;
			this.blaster.blasterSource.Play();
			break;
		case SIGadgetBlasterState.Cooldown:
			this.blaster.blasterSource.Stop();
			if (Time.time > this.blaster.lastFired + this.fireCooldown)
			{
				this.blaster.lastFired = Time.time;
			}
			break;
		}
		this.UpdateChargingVisuals();
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x0001E0D0 File Offset: 0x0001C2D0
	public void FireProjectile(float firedAtChargeLevel, int fireId, Vector3 position, Quaternion rotation)
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
			this.blaster.SendClientToClientRPC(0, new object[] { firedAtChargeLevel, fireId, position, rotation });
		}
		if (Mathf.Abs(this.currentCharge - firedAtChargeLevel) <= this.maxChargeDiff)
		{
			this.currentCharge = firedAtChargeLevel;
		}
		int num = this.CurrentBlasterChargeLevel();
		this.blaster.firingSource.clip = this.chargeLevels[num].firingClip;
		this.blaster.firingSource.volume = this.chargeLevels[num].firingVolume;
		this.chargeLevels[num].fireFX.Play();
		SIGadgetBlasterProjectile projectilePrefab = this.chargeLevels[num].projectilePrefab;
		this.blaster.firingSource.time = 0f;
		this.blaster.firingSource.Play();
		this.blaster.firingSource.loop = false;
		if (this.blaster.LocalEquippedOrActivated)
		{
			this.blaster.FireProjectileHaptics(this.chargeLevels[num].firingHapticStrength, this.chargeLevels[num].firingHapticDuration);
		}
		this.currentCharge = 0f;
		this.blaster.InstantiateProjectile(projectilePrefab, position, rotation, fireId);
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x0001E270 File Offset: 0x0001C470
	private void UpdateChargingVisuals()
	{
		bool flag = this.blaster.currentState == SIGadgetBlasterState.Charging;
		int num = this.CurrentBlasterChargeLevel();
		for (int i = 0; i < this.chargeLevels.Length; i++)
		{
			bool flag2 = flag && i == num;
			if (this.chargeLevels[i].chargingFX.activeSelf != flag2)
			{
				this.chargeLevels[i].chargingFX.SetActive(flag2);
			}
		}
		if (this.blaster.blasterSource.clip != this.chargingClip)
		{
			this.blaster.blasterSource.clip = this.chargingClip;
		}
		this.blaster.blasterSource.volume = this.chargeLevels[num].chargingVolume;
		if (!flag && this.blaster.blasterSource.isPlaying)
		{
			this.blaster.blasterSource.Stop();
		}
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x0001E35C File Offset: 0x0001C55C
	public void NetworkFireProjectile(object[] data)
	{
		if (data == null || data.Length != 4)
		{
			return;
		}
		float num;
		if (!GameEntityManager.ValidateDataType<float>(data[0], out num))
		{
			return;
		}
		if (float.IsNaN(num) || float.IsInfinity(num))
		{
			return;
		}
		int num2;
		if (!GameEntityManager.ValidateDataType<int>(data[1], out num2))
		{
			return;
		}
		Vector3 vector;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[2], out vector))
		{
			return;
		}
		if (!vector.IsFinite())
		{
			return;
		}
		Quaternion quaternion;
		if (!GameEntityManager.ValidateDataType<Quaternion>(data[3], out quaternion))
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
		this.FireProjectile(num, num2, vector, quaternion);
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x0001E420 File Offset: 0x0001C620
	public int CurrentBlasterChargeLevel()
	{
		int num = -1;
		for (int i = 0; i < this.chargeLevels.Length; i++)
		{
			if (this.currentCharge < this.chargeLevels[i].chargeThreshold)
			{
				return num;
			}
			num = i;
		}
		return num;
	}

	// Token: 0x04000622 RID: 1570
	[SerializeField]
	private float fireCooldown = 0.2f;

	// Token: 0x04000623 RID: 1571
	[SerializeField]
	private float chargeRatePerSecond = 20f;

	// Token: 0x04000624 RID: 1572
	public float fireRateGracePercentage = 0.25f;

	// Token: 0x04000625 RID: 1573
	public float maxChargeDiff = 5f;

	// Token: 0x04000626 RID: 1574
	private float currentCharge;

	// Token: 0x04000627 RID: 1575
	public AudioClip chargingClip;

	// Token: 0x04000628 RID: 1576
	public SIGadgetChargeBlaster.BlasterChargeLevel[] chargeLevels;

	// Token: 0x04000629 RID: 1577
	private SIGadgetBlaster blaster;

	// Token: 0x020000E5 RID: 229
	[Serializable]
	public struct BlasterChargeLevel
	{
		// Token: 0x0400062A RID: 1578
		public float chargeThreshold;

		// Token: 0x0400062B RID: 1579
		public float chargingVolume;

		// Token: 0x0400062C RID: 1580
		public float firingVolume;

		// Token: 0x0400062D RID: 1581
		public float chargingHapticStrength;

		// Token: 0x0400062E RID: 1582
		public float firingHapticStrength;

		// Token: 0x0400062F RID: 1583
		public float firingHapticDuration;

		// Token: 0x04000630 RID: 1584
		public AudioClip firingClip;

		// Token: 0x04000631 RID: 1585
		public ParticleSystem fireFX;

		// Token: 0x04000632 RID: 1586
		public GameObject chargingFX;

		// Token: 0x04000633 RID: 1587
		public SIGadgetBlasterProjectile projectilePrefab;
	}
}
