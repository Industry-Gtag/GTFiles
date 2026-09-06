using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000107 RID: 263
public class SIGadgetGrenadeGravity : SIGadgetGrenade
{
	// Token: 0x0600063D RID: 1597 RVA: 0x0002344F File Offset: 0x0002164F
	protected override void OnEnable()
	{
		base.OnEnable();
		this.gravityField.SetActive(false);
		this.state = SIGadgetGrenadeGravity.State.Idle;
		this.stateRemainingDuration = -1f;
		this.isLocalPlayerInEffect = false;
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x0002347C File Offset: 0x0002167C
	protected override void HandleActivated()
	{
		if (this.state == SIGadgetGrenadeGravity.State.Idle)
		{
			this.activatedLocally = true;
			this.SetStateAuthority(SIGadgetGrenadeGravity.State.Activated);
			return;
		}
		this.SetStateAuthority(SIGadgetGrenadeGravity.State.Idle);
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void HandleThrown()
	{
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void HandleHitSurface()
	{
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x0002349C File Offset: 0x0002169C
	protected override void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case SIGadgetGrenadeGravity.State.Idle:
			break;
		case SIGadgetGrenadeGravity.State.Activated:
			this.stateRemainingDuration -= dt;
			if (this.stateRemainingDuration <= 0f)
			{
				this.SetStateAuthority(SIGadgetGrenadeGravity.State.Triggered);
				return;
			}
			break;
		case SIGadgetGrenadeGravity.State.Triggered:
			this.stateRemainingDuration -= dt;
			if (this.stateRemainingDuration <= 0f)
			{
				this.SetStateAuthority(SIGadgetGrenadeGravity.State.Idle);
				return;
			}
			if (this.freezePositionOnTrigger)
			{
				this.CheckReenabledFreezePosition();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x00023518 File Offset: 0x00021718
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeGravity.State state = (SIGadgetGrenadeGravity.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
		if (this.freezePositionOnTrigger)
		{
			this.CheckReenabledFreezePosition();
		}
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x00023550 File Offset: 0x00021750
	private void SetStateAuthority(SIGadgetGrenadeGravity.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x00023574 File Offset: 0x00021774
	private void SetState(SIGadgetGrenadeGravity.State newState)
	{
		if (newState == this.state || !this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeGravity.State.Idle:
			this.activatedLocally = false;
			this.stateRemainingDuration = -1f;
			this.mesh.material = this.idleMat;
			this.DeactivateGravityEffect();
			return;
		case SIGadgetGrenadeGravity.State.Activated:
			this.stateRemainingDuration = this.counterDuration;
			this.mesh.material = this.activatedMat;
			this.DeactivateGravityEffect();
			return;
		case SIGadgetGrenadeGravity.State.Triggered:
			this.stateRemainingDuration = this.triggerDuration;
			this.mesh.material = this.triggeredMat;
			this.ActivateGravityEffect();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x00023627 File Offset: 0x00021827
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 3L;
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x00023636 File Offset: 0x00021836
	private void ActivateGravityEffect()
	{
		this.gravityField.SetActive(true);
		if (this.freezePositionOnTrigger)
		{
			this.rb.isKinematic = true;
			this.rb.linearVelocity = Vector3.zero;
		}
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x00023668 File Offset: 0x00021868
	private void DeactivateGravityEffect()
	{
		this.gravityField.SetActive(false);
		if (this.isLocalPlayerInEffect)
		{
			this.isLocalPlayerInEffect = false;
			GTPlayer instance = GTPlayer.Instance;
			if (instance != null)
			{
				instance.UnsetGravityOverride(this);
			}
		}
		if (this.freezePositionOnTrigger && !this.thrownGadget.IsHeld())
		{
			this.rb.isKinematic = false;
		}
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x000236C8 File Offset: 0x000218C8
	private void CheckReenabledFreezePosition()
	{
		if (this.state == SIGadgetGrenadeGravity.State.Triggered && !this.thrownGadget.IsHeld() && !this.rb.isKinematic)
		{
			this.rb.isKinematic = true;
			this.rb.linearVelocity = Vector3.zero;
		}
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x00023714 File Offset: 0x00021914
	private void OnTriggerEnter(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			this.isLocalPlayerInEffect = true;
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
		}
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x00023758 File Offset: 0x00021958
	private void OnTriggerExit(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			this.isLocalPlayerInEffect = false;
			instance.UnsetGravityOverride(this);
		}
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x00023790 File Offset: 0x00021990
	public void GravityOverrideFunction(GTPlayer player)
	{
		Vector3 vector = Physics.gravity * this.standardGravityMultiplier;
		Vector3 vector2 = Vector3.zero;
		if (!this.thrownGadget.IsHeldLocal())
		{
			vector2 = (base.transform.position - player.headCollider.transform.position).normalized * this.attractorStrength;
		}
		player.AddForce((vector + vector2) * player.scale, ForceMode.Acceleration);
	}

	// Token: 0x040007A7 RID: 1959
	[Header("Activation")]
	[SerializeField]
	private float counterDuration = 1f;

	// Token: 0x040007A8 RID: 1960
	[Header("Gravity Effect")]
	[SerializeField]
	private GameObject gravityField;

	// Token: 0x040007A9 RID: 1961
	[SerializeField]
	private bool freezePositionOnTrigger;

	// Token: 0x040007AA RID: 1962
	[SerializeField]
	private float triggerDuration = 5f;

	// Token: 0x040007AB RID: 1963
	[SerializeField]
	private float standardGravityMultiplier = 1f;

	// Token: 0x040007AC RID: 1964
	[SerializeField]
	private float attractorStrength;

	// Token: 0x040007AD RID: 1965
	[Header("FX")]
	[SerializeField]
	private MeshRenderer mesh;

	// Token: 0x040007AE RID: 1966
	[SerializeField]
	private Material idleMat;

	// Token: 0x040007AF RID: 1967
	[SerializeField]
	private Material activatedMat;

	// Token: 0x040007B0 RID: 1968
	[SerializeField]
	private Material triggeredMat;

	// Token: 0x040007B1 RID: 1969
	private SIGadgetGrenadeGravity.State state;

	// Token: 0x040007B2 RID: 1970
	private float stateRemainingDuration;

	// Token: 0x040007B3 RID: 1971
	private bool isLocalPlayerInEffect;

	// Token: 0x02000108 RID: 264
	private enum State
	{
		// Token: 0x040007B5 RID: 1973
		Idle,
		// Token: 0x040007B6 RID: 1974
		Activated,
		// Token: 0x040007B7 RID: 1975
		Triggered,
		// Token: 0x040007B8 RID: 1976
		Count
	}
}
