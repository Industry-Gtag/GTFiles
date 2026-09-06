using System;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000230 RID: 560
public class RCShip : RCHoverboard
{
	// Token: 0x06000ED0 RID: 3792 RVA: 0x00050E36 File Offset: 0x0004F036
	private byte GetDataB()
	{
		if (!this.hasNetworkSync)
		{
			return 0;
		}
		return this.networkSync.syncedState.dataB;
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x00050E52 File Offset: 0x0004F052
	private void SetDataB(byte b)
	{
		if (this.hasNetworkSync)
		{
			this.networkSync.syncedState.dataB = b;
		}
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x00050E70 File Offset: 0x0004F070
	private void WriteCannonBit(bool toLeft)
	{
		if (!this.hasNetworkSync)
		{
			return;
		}
		byte b = this.GetDataB();
		b = (toLeft ? (b | 1) : ((byte)((int)b & -2)));
		this.SetDataB(b);
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x00050EA3 File Offset: 0x0004F0A3
	private bool ReadCannonBit()
	{
		if (!this.hasNetworkSync)
		{
			return this.cannonToLeft;
		}
		return (this.GetDataB() & 1) > 0;
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x00050EBF File Offset: 0x0004F0BF
	private bool ReadFireFlip()
	{
		return (this.GetDataB() & 2) > 0;
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x00050ECC File Offset: 0x0004F0CC
	protected override void AuthorityUpdate(float dt)
	{
		base.AuthorityUpdate(dt);
		float trigger = this.activeInput.trigger;
		float num = (float)this.activeInput.buttons;
		if (this.localState == RCVehicle.State.Mobilized && this.localStatePrev != RCVehicle.State.Mobilized)
		{
			this.armedAfterMobilize = false;
			if (trigger >= this.triggerReleaseThreshold)
			{
				this.triggerIsDown = true;
			}
		}
		if (this.localState == RCVehicle.State.Mobilized)
		{
			if (!this.armedAfterMobilize && trigger <= this.triggerReleaseThreshold)
			{
				this.armedAfterMobilize = true;
				this.triggerIsDown = false;
			}
			if (this.armedAfterMobilize)
			{
				if (!this.triggerIsDown && trigger >= this.triggerPressThreshold)
				{
					this.triggerIsDown = true;
					UnityEvent onFire = this.OnFire;
					if (onFire != null)
					{
						onFire.Invoke();
					}
					if (this.hasNetworkSync)
					{
						byte b = this.GetDataB();
						b ^= 2;
						this.SetDataB(b);
						this.lastFireFlip = (b & 2) > 0;
					}
				}
				else if (this.triggerIsDown && trigger <= this.triggerReleaseThreshold)
				{
					this.triggerIsDown = false;
				}
			}
			if (!this.faceIsDown && num >= this.facePressThreshold)
			{
				this.faceIsDown = true;
				this.cannonToLeft = !this.cannonToLeft;
				this.WriteCannonBit(this.cannonToLeft);
			}
			else if (this.faceIsDown && num <= this.faceReleaseThreshold)
			{
				this.faceIsDown = false;
			}
		}
		else
		{
			if (this.faceIsDown && num <= this.faceReleaseThreshold)
			{
				this.faceIsDown = false;
			}
			this.armedAfterMobilize = false;
			if (this.triggerIsDown && trigger <= this.triggerReleaseThreshold)
			{
				this.triggerIsDown = false;
			}
		}
		if (this.hasNetworkSync)
		{
			byte b2 = this.GetDataB();
			if (this.localState == RCVehicle.State.Mobilized && this.rb != null && this.rb.linearVelocity.sqrMagnitude >= this.movingSpeedThreshold * this.movingSpeedThreshold)
			{
				b2 |= 4;
				this.isMovingShared = true;
			}
			else
			{
				b2 = (byte)((int)b2 & -5);
				this.isMovingShared = false;
			}
			this.SetDataB(b2);
			return;
		}
		this.isMovingShared = this.localState == RCVehicle.State.Mobilized && this.rb != null && this.rb.linearVelocity.sqrMagnitude >= this.movingSpeedThreshold * this.movingSpeedThreshold;
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x00051108 File Offset: 0x0004F308
	protected override void RemoteUpdate(float dt)
	{
		base.RemoteUpdate(dt);
		if (!this.hasNetworkSync)
		{
			return;
		}
		this.cannonToLeft = this.ReadCannonBit();
		bool flag = this.ReadFireFlip();
		if (!base.HasLocalAuthority)
		{
			if (flag != this.lastFireFlip)
			{
				this.lastFireFlip = flag;
				UnityEvent onFire = this.OnFire;
				if (onFire != null)
				{
					onFire.Invoke();
				}
			}
			byte dataB = this.GetDataB();
			this.isMovingShared = (dataB & 4) > 0;
			return;
		}
		this.lastFireFlip = flag;
		this.isMovingShared = this.localState == RCVehicle.State.Mobilized && this.rb != null && this.rb.linearVelocity.sqrMagnitude >= this.movingSpeedThreshold * this.movingSpeedThreshold;
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x000511C4 File Offset: 0x0004F3C4
	protected override void SharedUpdate(float dt)
	{
		base.SharedUpdate(dt);
		if (this.cannonTransform != null)
		{
			float num = (this.cannonToLeft ? this.leftYaw : this.rightYaw);
			Vector3 localEulerAngles = this.cannonTransform.localEulerAngles;
			localEulerAngles.z = Mathf.MoveTowardsAngle(localEulerAngles.z, num, this.cannonYawSpeed * dt);
			this.cannonTransform.localEulerAngles = localEulerAngles;
		}
		if (this.cannonToLeft != this.lastCannonToLeft)
		{
			this.lastCannonToLeft = this.cannonToLeft;
			UnityEvent<bool> onCannonSideChanged = this.OnCannonSideChanged;
			if (onCannonSideChanged != null)
			{
				onCannonSideChanged.Invoke(this.cannonToLeft);
			}
		}
		bool flag = this.localState == RCVehicle.State.Mobilized && this.isMovingShared;
		if (flag != this.lastIsMoving)
		{
			this.lastIsMoving = flag;
			if (flag)
			{
				UnityEvent onMoveStarted = this.OnMoveStarted;
				if (onMoveStarted == null)
				{
					return;
				}
				onMoveStarted.Invoke();
				return;
			}
			else
			{
				UnityEvent onMoveStopped = this.OnMoveStopped;
				if (onMoveStopped == null)
				{
					return;
				}
				onMoveStopped.Invoke();
			}
		}
	}

	// Token: 0x040011CB RID: 4555
	[Header("RCShip - Events")]
	public UnityEvent OnFire;

	// Token: 0x040011CC RID: 4556
	public UnityEvent<bool> OnCannonSideChanged;

	// Token: 0x040011CD RID: 4557
	public UnityEvent OnMoveStarted;

	// Token: 0x040011CE RID: 4558
	public UnityEvent OnMoveStopped;

	// Token: 0x040011CF RID: 4559
	[Header("RCShip - Cannon Rotation")]
	[SerializeField]
	private Transform cannonTransform;

	// Token: 0x040011D0 RID: 4560
	[SerializeField]
	private float leftYaw = -45f;

	// Token: 0x040011D1 RID: 4561
	[SerializeField]
	private float rightYaw = 45f;

	// Token: 0x040011D2 RID: 4562
	[SerializeField]
	private float cannonYawSpeed = 240f;

	// Token: 0x040011D3 RID: 4563
	[Header("RCShip - Input")]
	[Range(0f, 1f)]
	[SerializeField]
	private float triggerPressThreshold = 0.6f;

	// Token: 0x040011D4 RID: 4564
	[Range(0f, 1f)]
	[SerializeField]
	private float triggerReleaseThreshold = 0.1f;

	// Token: 0x040011D5 RID: 4565
	[Range(0f, 1f)]
	[SerializeField]
	private float facePressThreshold = 0.6f;

	// Token: 0x040011D6 RID: 4566
	[Range(0f, 1f)]
	[SerializeField]
	private float faceReleaseThreshold = 0.1f;

	// Token: 0x040011D7 RID: 4567
	[Header("RCShip - Movement Detection")]
	[Tooltip("Minimum speed to consider the ship moving")]
	[SerializeField]
	private float movingSpeedThreshold = 0.05f;

	// Token: 0x040011D8 RID: 4568
	private bool prevTriggerDown;

	// Token: 0x040011D9 RID: 4569
	private bool prevFaceDown;

	// Token: 0x040011DA RID: 4570
	private bool faceIsDown;

	// Token: 0x040011DB RID: 4571
	private bool triggerIsDown;

	// Token: 0x040011DC RID: 4572
	private bool armedAfterMobilize;

	// Token: 0x040011DD RID: 4573
	private bool cannonToLeft;

	// Token: 0x040011DE RID: 4574
	private const byte CannonLeftBit = 1;

	// Token: 0x040011DF RID: 4575
	private const byte FireFlipBit = 2;

	// Token: 0x040011E0 RID: 4576
	private const byte MovingBit = 4;

	// Token: 0x040011E1 RID: 4577
	private bool lastFireFlip;

	// Token: 0x040011E2 RID: 4578
	private bool lastCannonToLeft;

	// Token: 0x040011E3 RID: 4579
	private bool lastIsMoving;

	// Token: 0x040011E4 RID: 4580
	private bool isMovingShared;
}
