using System;
using UnityEngine;

// Token: 0x020000E8 RID: 232
[RequireComponent(typeof(GameTriggerInteractable))]
public class SIGadgetPumpBlaster : MonoBehaviour, SIGadgetBlasterType
{
	// Token: 0x0600056C RID: 1388 RVA: 0x0001E9FF File Offset: 0x0001CBFF
	private bool CheckInput()
	{
		return this.blaster.CheckInput();
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x0001EA0C File Offset: 0x0001CC0C
	private void OnEnable()
	{
		this.blaster = base.GetComponent<SIGadgetBlaster>();
		this.triggerInteractable = base.GetComponent<GameTriggerInteractable>();
		this.strokeLength = (this.pumpFullyClosed.position - this.pumpFullyOpen.position).magnitude;
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x0001EA5C File Offset: 0x0001CC5C
	public void OnUpdateAuthority(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle)
		{
			if (currentState != SIGadgetBlasterState.Pumping)
			{
				return;
			}
			if (!this.triggerInteractable.triggerInteractionActive)
			{
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			}
			Vector3 vector = this.pumpFullyOpen.position - this.pumpFullyClosed.position;
			Vector3 vector2 = this.pumpingTransform.position - this.pumpFullyClosed.position;
			if (Vector3.Dot(vector, vector2) < 0f)
			{
				vector2 = Vector3.zero;
			}
			Vector3 vector3 = Vector3.Project(vector2, vector);
			this.pumpHandlePosition.position = this.pumpFullyClosed.position + vector.normalized * Mathf.Clamp(vector3.magnitude, 0f, vector.magnitude);
			if (!this.pumpFullyOpened && vector3.magnitude > (1f - this.pumpThresholdPercent) * this.strokeLength)
			{
				this.pumpFullyOpened = true;
			}
			else if (this.pumpFullyOpen && vector3.magnitude < this.pumpThresholdPercent * this.strokeLength)
			{
				this.pumpFullyOpened = false;
				this.currentPumpChargeAmount = Mathf.Min(this.currentPumpChargeAmount + this.chargePerPump, this.maxPumpCharge);
			}
			if (this.CheckInput() && this.currentPumpChargeAmount > 0f)
			{
				this.AttemptFireProjectile(this.blaster.NextFireId(), this.currentPumpChargeAmount, this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
			}
		}
		else
		{
			if (this.triggerInteractable.triggerInteractionActive)
			{
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Pumping);
				return;
			}
			if (this.CheckInput() && this.currentPumpChargeAmount > 0f)
			{
				this.AttemptFireProjectile(this.blaster.NextFireId(), this.currentPumpChargeAmount, this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				return;
			}
		}
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x0001EC58 File Offset: 0x0001CE58
	public void OnUpdateRemote(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle && currentState == SIGadgetBlasterState.Pumping)
		{
			Vector3 vector = this.pumpFullyOpen.position - this.pumpFullyClosed.position;
			Vector3 vector2 = this.pumpingTransform.position - this.pumpFullyClosed.position;
			if (Vector3.Dot(vector, vector2) < 0f)
			{
				vector2 = Vector3.zero;
			}
			Vector3 vector3 = Vector3.Project(vector2, vector);
			this.pumpHandlePosition.position = this.pumpFullyClosed.position + vector.normalized * Mathf.Clamp(vector3.magnitude, 0f, vector.magnitude);
			this.currentPumpChargeAmount = Mathf.Min(this.maxPumpCharge, this.currentPumpChargeAmount + Time.deltaTime * this.remotePumpChargePerSecond);
		}
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x0001ED38 File Offset: 0x0001CF38
	public void SetStateShared()
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState == SIGadgetBlasterState.Idle)
		{
			this.blaster.blasterSource.clip = this.idleClip;
			this.blaster.blasterSource.volume = this.idleVolume;
			this.pumpingTransform = null;
			return;
		}
		if (currentState != SIGadgetBlasterState.Pumping)
		{
			return;
		}
		GameEntity gameEntity = this.blaster.gameEntity;
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(gameEntity.AttachedPlayerActorNr, out gamePlayer))
		{
			EHandedness equippedHandedness = gameEntity.EquippedHandedness;
			Transform transform;
			if (equippedHandedness != EHandedness.Left)
			{
				if (equippedHandedness != EHandedness.Right)
				{
					transform = this.pumpingTransform;
				}
				else
				{
					transform = gamePlayer.leftHand;
				}
			}
			else
			{
				transform = gamePlayer.rightHand;
			}
			this.pumpingTransform = transform;
		}
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x0001EDDC File Offset: 0x0001CFDC
	public void AttemptFireProjectile(int fireId, float pumpChargeAmount, Vector3 position, Quaternion rotation)
	{
		if (pumpChargeAmount <= 0f)
		{
			return;
		}
		if (pumpChargeAmount - this.maxPumpDiff > this.currentPumpChargeAmount)
		{
			return;
		}
		if (this.blaster.projectileCount > this.blaster.maxProjectileCount)
		{
			return;
		}
		if (this.blaster.LocalEquippedOrActivated)
		{
			this.blaster.SendClientToClientRPC(0, new object[] { fireId, position, rotation });
		}
		this.currentPumpChargeAmount = Mathf.Min(this.maxPumpCharge, pumpChargeAmount);
		this.blaster.firingSource.time = 0f;
		this.blaster.firingSource.Play();
		this.blaster.firingSource.loop = false;
		this.blaster.InstantiateProjectile(this.projectilePrefab, position, rotation, fireId);
		this.currentPumpChargeAmount = 0f;
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x0001EEC0 File Offset: 0x0001D0C0
	public void NetworkFireProjectile(object[] data)
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
		float num2;
		if (!GameEntityManager.ValidateDataType<float>(data[1], out num2))
		{
			return;
		}
		Vector3 vector;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[2], out vector))
		{
			return;
		}
		Quaternion quaternion;
		if (!GameEntityManager.ValidateDataType<Quaternion>(data[3], out quaternion))
		{
			return;
		}
		this.AttemptFireProjectile(num, num2, vector, quaternion);
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	// Token: 0x04000650 RID: 1616
	public SIGadgetBlasterProjectile projectilePrefab;

	// Token: 0x04000651 RID: 1617
	public AudioClip idleClip;

	// Token: 0x04000652 RID: 1618
	public AudioClip cooldownClip;

	// Token: 0x04000653 RID: 1619
	public float idleVolume;

	// Token: 0x04000654 RID: 1620
	public float cooldownVolume;

	// Token: 0x04000655 RID: 1621
	public AudioClip firingClip;

	// Token: 0x04000656 RID: 1622
	public float firingVolume;

	// Token: 0x04000657 RID: 1623
	public ParticleSystem fireFX;

	// Token: 0x04000658 RID: 1624
	public Transform pumpHandlePosition;

	// Token: 0x04000659 RID: 1625
	public Transform pumpFullyClosed;

	// Token: 0x0400065A RID: 1626
	public Transform pumpFullyOpen;

	// Token: 0x0400065B RID: 1627
	private GameTriggerInteractable triggerInteractable;

	// Token: 0x0400065C RID: 1628
	private SIGadgetBlaster blaster;

	// Token: 0x0400065D RID: 1629
	private Transform pumpingTransform;

	// Token: 0x0400065E RID: 1630
	public float currentPumpChargeAmount;

	// Token: 0x0400065F RID: 1631
	public float maxPumpCharge = 1f;

	// Token: 0x04000660 RID: 1632
	public float remotePumpChargePerSecond = 2f;

	// Token: 0x04000661 RID: 1633
	public float maxPumpDiff = 0.5f;

	// Token: 0x04000662 RID: 1634
	private float chargePerPump = 1f;

	// Token: 0x04000663 RID: 1635
	private bool pumpFullyOpened;

	// Token: 0x04000664 RID: 1636
	private float pumpThresholdPercent = 0.1f;

	// Token: 0x04000665 RID: 1637
	private float strokeLength;
}
