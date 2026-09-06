using System;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000E9 RID: 233
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetDashYoyo : SIGadget
{
	// Token: 0x17000062 RID: 98
	// (get) Token: 0x06000575 RID: 1397 RVA: 0x0001EF54 File Offset: 0x0001D154
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

	// Token: 0x06000576 RID: 1398 RVA: 0x0001EFD0 File Offset: 0x0001D1D0
	private void Start()
	{
		this._stateMaterials = this.m_baseStateMats;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this._HandleStartInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this._HandleStartInteraction));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this._HandleStopInteraction));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this._HandleStopInteraction));
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x0001F088 File Offset: 0x0001D288
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
		if (this._attachedVRRig != null)
		{
			VRRig attachedVRRig = this._attachedVRRig;
			attachedVRRig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(attachedVRRig.OnMaterialIndexChanged, new Action<int, int>(this._HandleVRRigMaterialIndexChanged));
		}
		this._ResetYoYo();
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x0001F174 File Offset: 0x0001D374
	private void LateUpdate()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		SIGadgetDashYoyo.EState state = this._state;
		if (state - SIGadgetDashYoyo.EState.Thrown <= 2)
		{
			this.m_tetherLineRenderer.SetPosition(1, this.m_tetherLineRenderer.transform.InverseTransformPoint(this.m_yoyoTarget.position));
		}
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x0001F1C0 File Offset: 0x0001D3C0
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
		if (this._attachedVRRig != null)
		{
			VRRig attachedVRRig = this._attachedVRRig;
			attachedVRRig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(attachedVRRig.OnMaterialIndexChanged, new Action<int, int>(this._HandleVRRigMaterialIndexChanged));
		}
		this._attachedVRRig = gamePlayer.rig;
		VRRig attachedVRRig2 = this._attachedVRRig;
		attachedVRRig2.OnMaterialIndexChanged = (Action<int, int>)Delegate.Combine(attachedVRRig2.OnMaterialIndexChanged, new Action<int, int>(this._HandleVRRigMaterialIndexChanged));
		int num = (this._isTagged ? 2 : 0);
		if (num != this._attachedVRRig.setMatIndex)
		{
			this._HandleVRRigMaterialIndexChanged(num, this._attachedVRRig.setMatIndex);
		}
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x0001F2A4 File Offset: 0x0001D4A4
	private void _HandleStopInteraction()
	{
		this._attachedPlayerActorNr = -1;
		this._attachedNetPlayer = null;
		if (this._attachedVRRig != null)
		{
			VRRig attachedVRRig = this._attachedVRRig;
			attachedVRRig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(attachedVRRig.OnMaterialIndexChanged, new Action<int, int>(this._HandleVRRigMaterialIndexChanged));
		}
		this._attachedVRRig = null;
		if (this._isTagged)
		{
			this._HandleVRRigMaterialIndexChanged(2, 0);
		}
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		if (this._state == SIGadgetDashYoyo.EState.DashUsed || this._state == SIGadgetDashYoyo.EState.OnCooldown)
		{
			this.SetStateAuthority(SIGadgetDashYoyo.EState.OnCooldown);
		}
		else
		{
			this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
		}
		GTPlayer.Instance.ResetRigidbodyInterpolation();
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x0001F348 File Offset: 0x0001D548
	private void _HandleVRRigMaterialIndexChanged(int oldMatIndex, int newMatIndex)
	{
		if (this._attachedPlayerActorNr != -1 && (newMatIndex == 2 || newMatIndex == 1) && this._hasTagUpgrade)
		{
			SuperInfectionGame superInfectionGame = GorillaGameManager.instance as SuperInfectionGame;
			if (superInfectionGame != null)
			{
				this._isTagged = this._attachedNetPlayer != null && superInfectionGame.IsInfected(this._attachedNetPlayer);
				this._OnTagStateOrUpgradesChanged();
				return;
			}
		}
		this._isTagged = false;
		this._OnTagStateOrUpgradesChanged();
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x0001F3B0 File Offset: 0x0001D5B0
	protected override void OnUpdateAuthority(float dt)
	{
		base.OnUpdateAuthority(dt);
		this._wasActivated = this._isActivated;
		this._isActivated = this._CheckInput();
		if (Time.unscaledTime < this._successfulYankTime + this.m_slipperySurfacesTime)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
		}
		switch (this._state)
		{
		case SIGadgetDashYoyo.EState.Idle:
			if (this._isActivated)
			{
				this._PlayHaptic(0.1f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.PreparedToThrow);
				return;
			}
			break;
		case SIGadgetDashYoyo.EState.OnCooldown:
			if (Time.unscaledTime > this._successfulYankTime + this._cooldownDuration)
			{
				this._PlayHaptic(0.5f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
				return;
			}
			break;
		case SIGadgetDashYoyo.EState.PreparedToThrow:
			if (!this._isActivated)
			{
				if (this._ThrowYoYoTarget())
				{
					this._PlayHaptic(0.5f);
					GTPlayer.Instance.RigidbodyInterpolation = RigidbodyInterpolation.None;
					this.SetStateAuthority(SIGadgetDashYoyo.EState.Thrown);
					return;
				}
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
				return;
			}
			break;
		case SIGadgetDashYoyo.EState.Thrown:
			if (Time.unscaledTime > this._timeLastThrown + this.m_waitBeforeAutoReturn)
			{
				this._PlayHaptic(0.75f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
				GTPlayer.Instance.ResetRigidbodyInterpolation();
				return;
			}
			if (GTPlayer.Instance.RigidbodyInterpolation != RigidbodyInterpolation.None)
			{
				GTPlayer.Instance.RigidbodyInterpolation = RigidbodyInterpolation.None;
			}
			if (this._isActivated)
			{
				this.SetStateAuthority(SIGadgetDashYoyo.EState.PreparedToDash);
				return;
			}
			break;
		case SIGadgetDashYoyo.EState.PreparedToDash:
			if (Time.unscaledTime > this._timeLastThrown + this.m_waitBeforeAutoReturn)
			{
				this._PlayHaptic(0.75f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
				return;
			}
			if (!this._isActivated)
			{
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Thrown);
				return;
			}
			this._CheckYankProgression();
			return;
		case SIGadgetDashYoyo.EState.DashUsed:
			if (Time.unscaledTime > this._successfulYankTime + this.m_postYankCooldown)
			{
				this._PlayHaptic(0.1f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.OnCooldown);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x0001F560 File Offset: 0x0001D760
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetDashYoyo.EState estate = (SIGadgetDashYoyo.EState)this.gameEntity.GetState();
		if (estate != this._state)
		{
			this._SetStateShared(estate);
		}
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x0001F591 File Offset: 0x0001D791
	private static bool _CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 6L;
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x0001F59F File Offset: 0x0001D79F
	private void SetStateAuthority(SIGadgetDashYoyo.EState newState)
	{
		this._SetStateShared(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x0001F5C0 File Offset: 0x0001D7C0
	private void _SetStateShared(SIGadgetDashYoyo.EState newState)
	{
		if (newState == this._state || !SIGadgetDashYoyo._CanChangeState((long)newState))
		{
			return;
		}
		SIGadgetDashYoyo.EState state = this._state;
		this._state = newState;
		switch (this._state)
		{
		case SIGadgetDashYoyo.EState.Idle:
			if (state == SIGadgetDashYoyo.EState.OnCooldown)
			{
				this._PlayAudio(4);
			}
			else if (state == SIGadgetDashYoyo.EState.PreparedToThrow)
			{
				this._PlayAudio(5);
			}
			this._ResetYoYo();
			this._SetMaterials(this._stateMaterials.idle);
			return;
		case SIGadgetDashYoyo.EState.OnCooldown:
			this._PlayAudio(3);
			this._ResetYoYo();
			this._SetMaterials(this._stateMaterials.cooldown);
			return;
		case SIGadgetDashYoyo.EState.PreparedToThrow:
			this._PlayAudio(0);
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.Thrown:
			if (state != SIGadgetDashYoyo.EState.PreparedToDash)
			{
				this._PlayAudio(1);
			}
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.PreparedToDash:
			this._yankBeginPos = this.m_yoyoDefaultPosXform.position;
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.DashUsed:
			this._PlayAudio(2);
			this._FreezeYoYo();
			this._SetMaterials(this._stateMaterials.cooldown);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x0001F6DC File Offset: 0x0001D8DC
	private bool _CheckInput()
	{
		float num = (this._wasActivated ? this.m_inputDeactivateThreshold : this.m_inputActivateThreshold);
		return this.m_buttonActivatable.CheckInput(num);
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x0001F70C File Offset: 0x0001D90C
	private bool _ThrowYoYoTarget()
	{
		Vector3 vector = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex);
		if (vector.magnitude < this.m_minThrowSpeed)
		{
			return false;
		}
		Vector3 handAngularVelocity = GamePlayerLocal.instance.GetHandAngularVelocity(this._HandIndex);
		GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
		vector *= this._throwMultiplier;
		vector += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
		this._LaunchYoYoShared(vector, handAngularVelocity, this.m_yoyoTargetRB.transform.position, this.m_yoyoTargetRB.transform.rotation);
		this._timeLastThrown = Time.unscaledTime;
		if (!NetworkSystem.Instance.InRoom)
		{
			return true;
		}
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone == null)
		{
			return true;
		}
		this._launchYoyoRPCArgs[0] = this.gameEntity.GetNetId();
		this._launchYoyoRPCArgs[1] = vector;
		this._launchYoyoRPCArgs[2] = handAngularVelocity;
		this._launchYoyoRPCArgs[3] = this.m_yoyoTargetRB.transform.position;
		this._launchYoyoRPCArgs[4] = this.m_yoyoTargetRB.transform.rotation;
		simanagerForZone.CallRPC(SuperInfectionManager.ClientToClientRPC.LaunchDashYoyo, this._launchYoyoRPCArgs);
		return true;
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x0001F85A File Offset: 0x0001DA5A
	internal void RemoteThrowYoYoTarget(Vector3 velocity, Vector3 angVelocity, Vector3 targetPosition, Quaternion targetRotation)
	{
		this._LaunchYoYoShared(velocity, angVelocity, targetPosition, targetRotation);
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x0001F868 File Offset: 0x0001DA68
	private void _LaunchYoYoShared(Vector3 velocity, Vector3 angVelocity, Vector3 targetPosition, Quaternion targetRotation)
	{
		this.m_yoyoTargetRB.transform.parent = null;
		float x = base.transform.lossyScale.x;
		this.m_yoyoTargetRB.transform.localScale = new Vector3(x, x, x);
		this.m_yoyoTargetRB.transform.position = targetPosition;
		this.m_yoyoTargetRB.transform.rotation = targetRotation;
		this.m_yoyoTargetRB.gameObject.SetActive(true);
		this.m_yoyoTarget.parent = this.m_yoyoTargetRB.transform;
		this.m_yoyoTargetRB.isKinematic = false;
		this.m_yoyoTargetRB.linearVelocity = velocity;
		this.m_yoyoTargetRB.angularVelocity = angVelocity;
		this.m_tetherLineRenderer.gameObject.SetActive(true);
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x0001F92E File Offset: 0x0001DB2E
	private void _FreezeYoYo()
	{
		this.m_yoyoTargetRB.gameObject.SetActive(false);
		this.m_yoyoTarget.parent = null;
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x0001F950 File Offset: 0x0001DB50
	internal void OnHitPlayer_Authority(SuperInfectionGame siTagGameManager, NetPlayer victimNetPlayer)
	{
		bool flag = siTagGameManager.IsInfected(this._attachedNetPlayer);
		bool flag2 = siTagGameManager.IsInfected(victimNetPlayer);
		if (flag == flag2)
		{
			return;
		}
		if (this._hasTagUpgrade && !flag2)
		{
			siTagGameManager.ReportTag(victimNetPlayer, this._attachedNetPlayer);
			return;
		}
		RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.SetSlowedTime, victimNetPlayer);
		RoomSystem.SendSoundEffectOnOther(5, 0.125f, victimNetPlayer, false);
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x0001F9A4 File Offset: 0x0001DBA4
	private void _ResetYoYo()
	{
		this.m_tetherLineRenderer.gameObject.SetActive(false);
		this.m_yoyoTargetRB.gameObject.SetActive(false);
		this.m_yoyoTarget.SetParent(this.m_yoyoDefaultPosXform, false);
		this.m_yoyoTarget.transform.localPosition = Vector3.zero;
		this.m_yoyoTarget.transform.localRotation = Quaternion.identity;
		this.m_yoyoTargetRB.transform.localScale = Vector3.one;
		this.m_yoyoTargetRB.transform.SetParent(this.m_yoyoDefaultPosXform, false);
		this.m_yoyoTargetRB.transform.localPosition = Vector3.zero;
		this.m_yoyoTargetRB.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x0001FA65 File Offset: 0x0001DC65
	private void _SetMaterials(Material mat)
	{
		this.m_yoyoRenderer.sharedMaterial = mat;
		this.m_tetherLineRenderer.sharedMaterial = mat;
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x0001FA80 File Offset: 0x0001DC80
	private void _CheckYankProgression()
	{
		Vector3 handVelocity = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex);
		this._maxEncounteredYankSpeed = Mathf.Max(this._maxEncounteredYankSpeed, handVelocity.magnitude);
		Vector3 vector = this._yankBeginPos - this.m_yoyoDefaultPosXform.position;
		Vector3 normalized = (-handVelocity.normalized + vector.normalized).normalized;
		Vector3 vector2 = this.m_yoyoTarget.position - this.m_yoyoDefaultPosXform.position;
		if (vector.magnitude < this.m_yankMinDistance || this._maxEncounteredYankSpeed < this.m_yankMinSpeed || Vector3.Angle(vector2, normalized) > this.m_yankMaxAngle)
		{
			return;
		}
		if (base.IsBlocked(SIExclusionType.AffectsLocalMovement))
		{
			return;
		}
		this._successfulYankTime = Time.unscaledTime;
		float num = this._CalculateDashSpeed(handVelocity.magnitude);
		GTPlayer instance = GTPlayer.Instance;
		instance.SetMaximumSlipThisFrame();
		instance.SetVelocity(Vector3.RotateTowards(vector2.normalized, normalized, this._maxInfluenceAngle * 0.017453292f, 0f) * num);
		this._PlayHaptic(2f);
		this.SetStateAuthority(SIGadgetDashYoyo.EState.DashUsed);
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x0001FBA8 File Offset: 0x0001DDA8
	private float _CalculateDashSpeed(float currentYankSpeed)
	{
		float num = Mathf.InverseLerp(this.m_yankMinSpeed, this.m_yankMaxSpeed, currentYankSpeed);
		float num2 = this.m_speedMappingCurve.Evaluate(num);
		return Mathf.Lerp(this.m_minDashSpeed, this._maxDashSpeed, num2);
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x0001FBE8 File Offset: 0x0001DDE8
	private void _PlayHaptic(float strengthMultiplier)
	{
		bool flag;
		if (base.FindAttachedHand(out flag))
		{
			GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength * strengthMultiplier, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x0001FC20 File Offset: 0x0001DE20
	private void _PlayAudio(int index)
	{
		this.m_audioSource.clip = this.m_clips[index];
		this.m_audioSource.volume = this.m_clipVolumes[index];
		this.m_audioSource.GTPlay();
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x0001FC54 File Offset: 0x0001DE54
	private void _OnTagStateOrUpgradesChanged()
	{
		this._stateMaterials = (this._hasTagUpgrade ? (this._isTagged ? this.m_tagUpgradeStateMatsWhileTagged : this.m_tagUpgradeStateMatsWhileUntagged) : this.m_baseStateMats);
		switch (this._state)
		{
		case SIGadgetDashYoyo.EState.Idle:
			this._SetMaterials(this._stateMaterials.idle);
			return;
		case SIGadgetDashYoyo.EState.OnCooldown:
			this._SetMaterials(this._stateMaterials.cooldown);
			return;
		case SIGadgetDashYoyo.EState.PreparedToThrow:
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.Thrown:
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.PreparedToDash:
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.DashUsed:
			this._SetMaterials(this._stateMaterials.cooldown);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x0001FD20 File Offset: 0x0001DF20
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._cooldownDuration = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Cooldown) ? this.m_cooldownDurationUpgrade : this.m_cooldownDurationDefault);
		this._throwMultiplier = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Range) ? this.m_throwMultiplierUpgrade : this.m_throwMultiplierDefault);
		this._maxDashSpeed = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Speed) ? this.m_maxDashSpeedUpgraded : this.m_maxDashSpeedDefault);
		this._maxInfluenceAngle = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Dynamic) ? this.m_maxInfluenceAngleUpgrade : this.m_maxInfluenceAngleDefault);
		this._hasStunUpgrade = withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Stun);
		this._hasTagUpgrade = withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Tag);
		this._OnTagStateOrUpgradesChanged();
	}

	// Token: 0x04000666 RID: 1638
	private const string preLog = "[SIGadgetDashYoyo]  ";

	// Token: 0x04000667 RID: 1639
	private const string preErr = "[SIGadgetDashYoyo]  ERROR!!!  ";

	// Token: 0x04000668 RID: 1640
	[SerializeField]
	private GameSnappable m_snappable;

	// Token: 0x04000669 RID: 1641
	[SerializeField]
	private Transform m_yoyoDefaultPosXform;

	// Token: 0x0400066A RID: 1642
	[SerializeField]
	private Transform m_yoyoTarget;

	// Token: 0x0400066B RID: 1643
	[SerializeField]
	private Rigidbody m_yoyoTargetRB;

	// Token: 0x0400066C RID: 1644
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x0400066D RID: 1645
	[SerializeField]
	private float m_inputActivateThreshold = 0.35f;

	// Token: 0x0400066E RID: 1646
	[SerializeField]
	private float m_inputDeactivateThreshold = 0.25f;

	// Token: 0x0400066F RID: 1647
	private SIGadgetDashYoyo.StateMaterialsInfo _stateMaterials;

	// Token: 0x04000670 RID: 1648
	[SerializeField]
	private SIGadgetDashYoyo.StateMaterialsInfo m_baseStateMats;

	// Token: 0x04000671 RID: 1649
	[SerializeField]
	private SIGadgetDashYoyo.StateMaterialsInfo m_tagUpgradeStateMatsWhileTagged;

	// Token: 0x04000672 RID: 1650
	[SerializeField]
	private SIGadgetDashYoyo.StateMaterialsInfo m_tagUpgradeStateMatsWhileUntagged;

	// Token: 0x04000673 RID: 1651
	[SerializeField]
	private MeshRenderer m_yoyoRenderer;

	// Token: 0x04000674 RID: 1652
	[SerializeField]
	private AudioSource m_audioSource;

	// Token: 0x04000675 RID: 1653
	[SerializeField]
	public AudioClip[] m_clips;

	// Token: 0x04000676 RID: 1654
	[SerializeField]
	public float[] m_clipVolumes;

	// Token: 0x04000677 RID: 1655
	private float _throwMultiplier;

	// Token: 0x04000678 RID: 1656
	[SerializeField]
	private float m_throwMultiplierDefault = 1.5f;

	// Token: 0x04000679 RID: 1657
	[SerializeField]
	private float m_throwMultiplierUpgrade = 2f;

	// Token: 0x0400067A RID: 1658
	[FormerlySerializedAs("m_tether")]
	[SerializeField]
	private LineRenderer m_tetherLineRenderer;

	// Token: 0x0400067B RID: 1659
	[SerializeField]
	private float m_minThrowSpeed = 2f;

	// Token: 0x0400067C RID: 1660
	[SerializeField]
	private float m_waitBeforeAutoReturn = 3f;

	// Token: 0x0400067D RID: 1661
	[SerializeField]
	private float m_postYankCooldown = 2f;

	// Token: 0x0400067E RID: 1662
	[SerializeField]
	private float m_maxYankRecheckTime = 0.2f;

	// Token: 0x0400067F RID: 1663
	[SerializeField]
	private float m_yankMinDistance = 0.5f;

	// Token: 0x04000680 RID: 1664
	[SerializeField]
	private float m_yankMaxAngle = 60f;

	// Token: 0x04000681 RID: 1665
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMinSpeed = 2f;

	// Token: 0x04000682 RID: 1666
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMaxSpeed = 8f;

	// Token: 0x04000683 RID: 1667
	[Tooltip("Dash min/max speed: The fastest speed the player will move")]
	[SerializeField]
	private float m_minDashSpeed = 4f;

	// Token: 0x04000684 RID: 1668
	private float _maxDashSpeed;

	// Token: 0x04000685 RID: 1669
	[SerializeField]
	private float m_maxDashSpeedDefault = 11f;

	// Token: 0x04000686 RID: 1670
	[SerializeField]
	private float m_maxDashSpeedUpgraded = 13f;

	// Token: 0x04000687 RID: 1671
	[Tooltip("Maps yank speed to dash speed.\nX = Yank Speed (min to max)\nY = Dash Speed (min to max).")]
	[SerializeField]
	private AnimationCurve m_speedMappingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000688 RID: 1672
	[SerializeField]
	private float m_slipperySurfacesTime = 0.25f;

	// Token: 0x04000689 RID: 1673
	private float _maxInfluenceAngle;

	// Token: 0x0400068A RID: 1674
	[SerializeField]
	private float m_maxInfluenceAngleDefault = 10f;

	// Token: 0x0400068B RID: 1675
	[SerializeField]
	private float m_maxInfluenceAngleUpgrade = 15f;

	// Token: 0x0400068C RID: 1676
	private float _cooldownDuration;

	// Token: 0x0400068D RID: 1677
	[SerializeField]
	private float m_cooldownDurationDefault = 6f;

	// Token: 0x0400068E RID: 1678
	[SerializeField]
	private float m_cooldownDurationUpgrade = 5f;

	// Token: 0x0400068F RID: 1679
	private bool _hasStunUpgrade;

	// Token: 0x04000690 RID: 1680
	private bool _hasTagUpgrade;

	// Token: 0x04000691 RID: 1681
	private bool _isActivated;

	// Token: 0x04000692 RID: 1682
	private bool _wasActivated;

	// Token: 0x04000693 RID: 1683
	private float _timeLastThrown;

	// Token: 0x04000694 RID: 1684
	private float _successfulYankTime;

	// Token: 0x04000695 RID: 1685
	private float _maxEncounteredYankSpeed;

	// Token: 0x04000696 RID: 1686
	private Vector3 _yankBeginPos;

	// Token: 0x04000697 RID: 1687
	private bool _isRecheckingYank;

	// Token: 0x04000698 RID: 1688
	private VRRig _attachedVRRig;

	// Token: 0x04000699 RID: 1689
	private int _lastAttachedPlayerActorNr;

	// Token: 0x0400069A RID: 1690
	private int _attachedPlayerActorNr = int.MinValue;

	// Token: 0x0400069B RID: 1691
	private NetPlayer _attachedNetPlayer;

	// Token: 0x0400069C RID: 1692
	private bool _isTagged;

	// Token: 0x0400069D RID: 1693
	private readonly object[] _launchYoyoRPCArgs = new object[5];

	// Token: 0x0400069E RID: 1694
	private SIGadgetDashYoyo.EState _state;

	// Token: 0x020000EA RID: 234
	[Serializable]
	public struct StateMaterialsInfo
	{
		// Token: 0x0400069F RID: 1695
		public Material idle;

		// Token: 0x040006A0 RID: 1696
		public Material ready;

		// Token: 0x040006A1 RID: 1697
		public Material cooldown;
	}

	// Token: 0x020000EB RID: 235
	private enum EState
	{
		// Token: 0x040006A3 RID: 1699
		Idle,
		// Token: 0x040006A4 RID: 1700
		OnCooldown,
		// Token: 0x040006A5 RID: 1701
		PreparedToThrow,
		// Token: 0x040006A6 RID: 1702
		Thrown,
		// Token: 0x040006A7 RID: 1703
		PreparedToDash,
		// Token: 0x040006A8 RID: 1704
		DashUsed,
		// Token: 0x040006A9 RID: 1705
		Count
	}
}
