using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000F1 RID: 241
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetAirGrab : SIGadget
{
	// Token: 0x17000064 RID: 100
	// (get) Token: 0x060005B0 RID: 1456 RVA: 0x00020A40 File Offset: 0x0001EC40
	private int _HandIndex
	{
		get
		{
			if ((this.m_snappable.snappedToJoint != null && this.m_snappable.snappedToJoint.jointType == SnapJointType.HandL) || this.gameEntity.heldByHandIndex == 0)
			{
				return 0;
			}
			if ((this.m_snappable.snappedToJoint != null && this.m_snappable.snappedToJoint.jointType == SnapJointType.HandR) || this.gameEntity.heldByHandIndex == 1)
			{
				return 1;
			}
			return -1;
		}
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x00020ABC File Offset: 0x0001ECBC
	private void Awake()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this._HandleStartInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this._HandleStartInteraction));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this._HandleStopInteraction));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this._HandleStopInteraction));
		this._groundedUseCounter = new ResettableUseCounter(1, this.m_maxSuperchargeUses, new Action<bool>(this.OnRecharge));
		foreach (AudioClip audioClip in this.m_clips)
		{
			if (audioClip)
			{
				audioClip.LoadAudioData();
			}
		}
		this._grabXformInitialScale = this.m_airGrabXform.localScale;
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x00020BBC File Offset: 0x0001EDBC
	private void OnRecharge(bool recharged)
	{
		if (recharged)
		{
			this.rechargeSound.Play();
		}
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x00020BCC File Offset: 0x0001EDCC
	private void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this._HandleStartInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Remove(gameEntity2.OnSnapped, new Action(this._HandleStartInteraction));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Remove(gameEntity3.OnReleased, new Action(this._HandleStopInteraction));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Remove(gameEntity4.OnUnsnapped, new Action(this._HandleStopInteraction));
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x00020C7D File Offset: 0x0001EE7D
	private void ClearGravityOverride()
	{
		GTPlayer.Instance.UnsetGravityOverride(this);
		this.hasGravityOverride = false;
	}

	// Token: 0x060005B5 RID: 1461 RVA: 0x00020C94 File Offset: 0x0001EE94
	private new void OnDisable()
	{
		if (this.hasGravityOverride)
		{
			this.ClearGravityOverride();
		}
		if (this.m_airGrabXform != null)
		{
			this.m_airGrabXform.gameObject.SetActive(false);
			this.m_airGrabXform.SetParent(base.transform, false);
		}
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x00020CE0 File Offset: 0x0001EEE0
	private void _HandleStartInteraction()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._attachedPlayerActorNr = this.gameEntity.AttachedPlayerActorNr;
		this._attachedNetPlayer = NetworkSystem.Instance.GetPlayer(this._attachedPlayerActorNr);
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this._attachedPlayerActorNr, out gamePlayer))
		{
			return;
		}
		this._attachedVRRig = gamePlayer.rig;
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x00020D38 File Offset: 0x0001EF38
	private void _HandleStopInteraction()
	{
		if (this.hasGravityOverride)
		{
			this.ClearGravityOverride();
		}
		this._attachedPlayerActorNr = -1;
		this._attachedNetPlayer = null;
		this._attachedVRRig = null;
		this.m_airGrabXform.gameObject.SetActive(false);
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		if (this._state == SIGadgetAirGrab.EState.DashUsed)
		{
			this.SetStateAuthority(SIGadgetAirGrab.EState.DashUsed);
			return;
		}
		this.SetStateAuthority(SIGadgetAirGrab.EState.Idle);
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x00020DA0 File Offset: 0x0001EFA0
	protected void FixedUpdate()
	{
		if ((!this.IsEquippedLocal() && !this.activatedLocally) || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._wasActivated = this._isActivated;
		this._isActivated = this._CheckInput();
		GTPlayer instance = GTPlayer.Instance;
		if (Time.unscaledTime < this._airGrabTime + this.m_slipperySurfacesTime)
		{
			instance.SetMaximumSlipThisFrame();
		}
		switch (this._state)
		{
		case SIGadgetAirGrab.EState.Idle:
			if (this._isActivated && !base.IsBlocked(SIExclusionType.AffectsLocalMovement))
			{
				if (this._groundedUseCounter.TryUse())
				{
					this.UpdateUsageIndicator();
					this._PlayHaptic(2f);
					this.SetStateAuthority(SIGadgetAirGrab.EState.StartAirGrabbing);
					return;
				}
			}
			else if (instance.IsGroundedButt || instance.IsGroundedHand)
			{
				this._groundedUseCounter.Reset();
				this.UpdateUsageIndicator();
				return;
			}
			break;
		case SIGadgetAirGrab.EState.StartAirGrabbing:
			if (this._isActivated)
			{
				this._grabStartTime = Time.unscaledTime;
				this._airReleaseSpeed = 0f;
				this.m_airGrabXform.SetParent(null, false);
				this.m_airGrabXform.position = GTPlayer.Instance.GetControllerTransform(this._HandIndex == 0).position;
				this.m_airGrabXform.gameObject.SetActive(true);
				this.m_airGrabXform.transform.localScale = this._grabXformInitialScale;
				GTPlayer.Instance.SetVelocity(Vector3.zero);
				this.lastRequestedPlayerPos = GTPlayer.Instance.transform.position;
				GTPlayer.Instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
				this.hasGravityOverride = true;
				this.SetStateAuthority(SIGadgetAirGrab.EState.PreparedToDash);
				return;
			}
			this.m_airGrabXform.transform.parent = base.transform;
			this.m_airGrabXform.gameObject.SetActive(false);
			return;
		case SIGadgetAirGrab.EState.PreparedToDash:
		{
			if (!this._isActivated)
			{
				this._DoDash();
				return;
			}
			if (Time.unscaledTime > this._grabStartTime + this._maxHoldTime)
			{
				this._DoDash();
				return;
			}
			float num = (Time.unscaledTime - this._grabStartTime) / this._maxHoldTime;
			this.m_airGrabXform.localScale = this._grabXformInitialScale * (1f - num);
			this._UpdateAirGrab();
			return;
		}
		case SIGadgetAirGrab.EState.DashUsed:
			this.m_airGrabXform.transform.parent = base.transform;
			this.m_airGrabXform.gameObject.SetActive(false);
			this.ClearGravityOverride();
			this.SetStateAuthority(SIGadgetAirGrab.EState.Idle);
			break;
		default:
			return;
		}
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x00020FFA File Offset: 0x0001F1FA
	private void UpdateUsageIndicator()
	{
		GameObject canActivateIndicator = this.m_canActivateIndicator;
		if (canActivateIndicator == null)
		{
			return;
		}
		canActivateIndicator.SetActive(this._groundedUseCounter.IsReady);
	}

	// Token: 0x060005BA RID: 1466 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void GravityOverrideFunction(GTPlayer player)
	{
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x00021018 File Offset: 0x0001F218
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetAirGrab.EState estate = (SIGadgetAirGrab.EState)this.gameEntity.GetState();
		if (estate != this._state)
		{
			this._SetStateShared(estate);
			if (this._state == SIGadgetAirGrab.EState.PreparedToDash)
			{
				this.m_airGrabXform.transform.parent = base.transform;
				this.m_airGrabXform.transform.position = ((this._HandIndex == 0) ? base.GetAttachedPlayerRig().leftHand : base.GetAttachedPlayerRig().rightHand).GetExtrapolatedControllerPosition();
				this.m_airGrabXform.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x0001D66E File Offset: 0x0001B86E
	private static bool _CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L;
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x000210AE File Offset: 0x0001F2AE
	private void SetStateAuthority(SIGadgetAirGrab.EState newState)
	{
		this._SetStateShared(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x000210D0 File Offset: 0x0001F2D0
	private void _SetStateShared(SIGadgetAirGrab.EState newState)
	{
		if (newState == this._state || !SIGadgetAirGrab._CanChangeState((long)newState))
		{
			return;
		}
		SIGadgetAirGrab.EState state = this._state;
		this._state = newState;
		switch (this._state)
		{
		case SIGadgetAirGrab.EState.Idle:
			this.m_airGrabXform.gameObject.SetActive(false);
			return;
		case SIGadgetAirGrab.EState.StartAirGrabbing:
			if (state != SIGadgetAirGrab.EState.PreparedToDash)
			{
				this.onGrabSound.Play();
				return;
			}
			break;
		case SIGadgetAirGrab.EState.PreparedToDash:
			break;
		case SIGadgetAirGrab.EState.DashUsed:
			this._PlayAudio(2);
			break;
		default:
			return;
		}
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x00021148 File Offset: 0x0001F348
	private bool _CheckInput()
	{
		float num = (this._wasActivated ? this.m_inputDeactivateThreshold : this.m_inputActivateThreshold);
		return this.m_buttonActivatable.CheckInput(num);
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x00021178 File Offset: 0x0001F378
	private void _UpdateAirGrab()
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 vector = instance.transform.position - this.lastRequestedPlayerPos;
		this.m_airGrabXform.position += vector;
		Transform controllerTransform = instance.GetControllerTransform(this._HandIndex == 0);
		Vector3 vector2 = this.m_airGrabXform.position - controllerTransform.position;
		instance.SetVelocity(Vector3.zero);
		this.lastRequestedPlayerPos = instance.transform.position + vector2;
		instance.RigidbodyMovePosition(this.lastRequestedPlayerPos);
		float magnitude = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex).magnitude;
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x0002122C File Offset: 0x0001F42C
	private void _DoDash()
	{
		this._airGrabTime = Time.unscaledTime;
		Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
		float num = this._CalculateDashSpeed(averagedVelocity.magnitude);
		GTPlayer instance = GTPlayer.Instance;
		instance.SetMaximumSlipThisFrame();
		instance.SetVelocity(averagedVelocity.normalized * num);
		this._PlayHaptic(2f);
		this.SetStateAuthority(SIGadgetAirGrab.EState.DashUsed);
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x0002128C File Offset: 0x0001F48C
	private float _CalculateDashSpeed(float currentYankSpeed)
	{
		float num = Mathf.InverseLerp(this.m_yankMinSpeed, this.m_yankMaxSpeed, currentYankSpeed);
		float num2 = this.m_speedMappingCurve.Evaluate(num);
		return Mathf.Lerp(this.m_minDashSpeed, this._maxDashSpeed, num2);
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x000212CC File Offset: 0x0001F4CC
	private void _PlayHaptic(float strengthMultiplier)
	{
		bool flag;
		if (base.FindAttachedHand(out flag))
		{
			GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength * strengthMultiplier, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x00021304 File Offset: 0x0001F504
	private void _PlayAudio(int index)
	{
		this.m_audioSource.clip = this.m_clips[index];
		this.m_audioSource.volume = this.m_clipVolumes[index];
		this.m_audioSource.GTPlay();
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x00021338 File Offset: 0x0001F538
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._maxDashSpeed = (withUpgrades.Contains(SIUpgradeType.AirControl_AirGrab_Speed) ? this.m_maxDashSpeedUpgraded : this.m_maxDashSpeedDefault);
		this._maxHoldTime = (withUpgrades.Contains(SIUpgradeType.AirControl_AirGrab_HoldTime) ? this.m_maxHoldTimeUpgraded : this.m_maxHoldTimeDefault);
	}

	// Token: 0x040006E3 RID: 1763
	private const string preLog = "[SIGadgetAirGrab]  ";

	// Token: 0x040006E4 RID: 1764
	private const string preErr = "[SIGadgetAirGrab]  ERROR!!!  ";

	// Token: 0x040006E5 RID: 1765
	[SerializeField]
	private GameSnappable m_snappable;

	// Token: 0x040006E6 RID: 1766
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x040006E7 RID: 1767
	[SerializeField]
	private float m_inputActivateThreshold = 0.35f;

	// Token: 0x040006E8 RID: 1768
	[SerializeField]
	private float m_inputDeactivateThreshold = 0.25f;

	// Token: 0x040006E9 RID: 1769
	[SerializeField]
	private AudioSource m_audioSource;

	// Token: 0x040006EA RID: 1770
	[SerializeField]
	private SoundBankPlayer onGrabSound;

	// Token: 0x040006EB RID: 1771
	[SerializeField]
	private SoundBankPlayer rechargeSound;

	// Token: 0x040006EC RID: 1772
	[SerializeField]
	public AudioClip[] m_clips;

	// Token: 0x040006ED RID: 1773
	[SerializeField]
	public float[] m_clipVolumes;

	// Token: 0x040006EE RID: 1774
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMinSpeed = 2f;

	// Token: 0x040006EF RID: 1775
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMaxSpeed = 8f;

	// Token: 0x040006F0 RID: 1776
	[Tooltip("Dash min/max speed: The fastest speed the player will move")]
	[SerializeField]
	private float m_minDashSpeed = 4f;

	// Token: 0x040006F1 RID: 1777
	private float _maxDashSpeed;

	// Token: 0x040006F2 RID: 1778
	[SerializeField]
	private float m_maxDashSpeedDefault = 7f;

	// Token: 0x040006F3 RID: 1779
	[SerializeField]
	private float m_maxDashSpeedUpgraded = 9f;

	// Token: 0x040006F4 RID: 1780
	private float _maxHoldTime;

	// Token: 0x040006F5 RID: 1781
	[SerializeField]
	private float m_maxHoldTimeDefault = 3f;

	// Token: 0x040006F6 RID: 1782
	[SerializeField]
	private float m_maxHoldTimeUpgraded = 5f;

	// Token: 0x040006F7 RID: 1783
	[Tooltip("Maps yank speed to dash speed.\nX = Yank Speed (min to max)\nY = Dash Speed (min to max).")]
	[SerializeField]
	private AnimationCurve m_speedMappingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040006F8 RID: 1784
	[SerializeField]
	private float m_slipperySurfacesTime = 0.25f;

	// Token: 0x040006F9 RID: 1785
	[SerializeField]
	private float m_maxInfluenceAngleDefault = 10f;

	// Token: 0x040006FA RID: 1786
	[SerializeField]
	private float m_maxInfluenceAngleUpgrade = 15f;

	// Token: 0x040006FB RID: 1787
	[SerializeField]
	private float m_cooldownDurationDefault = 6f;

	// Token: 0x040006FC RID: 1788
	[SerializeField]
	private float m_cooldownDurationUpgrade = 5f;

	// Token: 0x040006FD RID: 1789
	[SerializeField]
	private int m_maxSuperchargeUses = 2;

	// Token: 0x040006FE RID: 1790
	[SerializeField]
	private Transform m_airGrabXform;

	// Token: 0x040006FF RID: 1791
	[SerializeField]
	private GameObject m_canActivateIndicator;

	// Token: 0x04000700 RID: 1792
	private bool _isActivated;

	// Token: 0x04000701 RID: 1793
	private bool _wasActivated;

	// Token: 0x04000702 RID: 1794
	private float _airGrabTime;

	// Token: 0x04000703 RID: 1795
	private float _airReleaseSpeed;

	// Token: 0x04000704 RID: 1796
	private Vector3 _airReleaseVector;

	// Token: 0x04000705 RID: 1797
	private VRRig _attachedVRRig;

	// Token: 0x04000706 RID: 1798
	private int _lastAttachedPlayerActorNr;

	// Token: 0x04000707 RID: 1799
	private int _attachedPlayerActorNr = int.MinValue;

	// Token: 0x04000708 RID: 1800
	private NetPlayer _attachedNetPlayer;

	// Token: 0x04000709 RID: 1801
	private bool _isTagged;

	// Token: 0x0400070A RID: 1802
	private readonly object[] _launchYoyoRPCArgs = new object[5];

	// Token: 0x0400070B RID: 1803
	private SIGadgetAirGrab.EState _state;

	// Token: 0x0400070C RID: 1804
	private ResettableUseCounter _groundedUseCounter;

	// Token: 0x0400070D RID: 1805
	private bool hasGravityOverride;

	// Token: 0x0400070E RID: 1806
	private float _grabStartTime;

	// Token: 0x0400070F RID: 1807
	private Vector3 _grabXformInitialScale;

	// Token: 0x04000710 RID: 1808
	private Vector3 lastRequestedPlayerPos;

	// Token: 0x020000F2 RID: 242
	private enum EState
	{
		// Token: 0x04000712 RID: 1810
		Idle,
		// Token: 0x04000713 RID: 1811
		StartAirGrabbing,
		// Token: 0x04000714 RID: 1812
		PreparedToDash,
		// Token: 0x04000715 RID: 1813
		DashUsed,
		// Token: 0x04000716 RID: 1814
		Count
	}
}
