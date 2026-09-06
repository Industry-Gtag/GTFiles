using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using UnityEngine;

// Token: 0x02000117 RID: 279
public class SIGadgetTentacleArm : SIGadget, ICallBack, IEnergyGadget
{
	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060006E1 RID: 1761 RVA: 0x00026849 File Offset: 0x00024A49
	// (set) Token: 0x060006E2 RID: 1762 RVA: 0x00026851 File Offset: 0x00024A51
	public bool isAnchored { get; private set; }

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x060006E3 RID: 1763 RVA: 0x0002685A File Offset: 0x00024A5A
	// (set) Token: 0x060006E4 RID: 1764 RVA: 0x00026862 File Offset: 0x00024A62
	public bool isHoldingHand { get; private set; }

	// Token: 0x060006E5 RID: 1765 RVA: 0x0002686C File Offset: 0x00024A6C
	private void Awake()
	{
		this._fps_holding_base = this.FuelPerSecond_Holding;
		this._fps_recharging_base = this.FuelPerSecond_Recharging;
		this._grabCost_base = this.FuelCost_Grab;
		this._jumpCost_base = this.FuelCost_JumpSpeed;
		this._jumpSpeed_base = this.MaxTentacleJumpSpeed;
		this._grabAngle_base = this.MaxGrabAngle;
		this._wall_angle_dot = Mathf.Cos(0.017453292f * this.WallAngle);
		this.tentacleMat = new Material(this.tentacleRenderer.sharedMaterial);
		this.tentacleRenderer.sharedMaterial = this.tentacleMat;
		if (this.tentacleRenderer2 != null)
		{
			this.hasTentacle2 = true;
			this.tentacleMat2 = new Material(this.tentacleRenderer2.sharedMaterial);
			this.tentacleRenderer2.sharedMaterial = this.tentacleMat2;
		}
		this._gaugeMatPropBlock = new MaterialPropertyBlock();
		if (this.m_gaugeMatSlots == null)
		{
			this.m_gaugeMatSlots = Array.Empty<GTRendererMatSlot>();
		}
		int num = 0;
		for (int i = 0; i < this.m_gaugeMatSlots.Length; i++)
		{
			if (this.m_gaugeMatSlots[i].TryInitialize())
			{
				this.m_gaugeMatSlots[num] = this.m_gaugeMatSlots[i];
				num++;
			}
		}
		if (num != this.m_gaugeMatSlots.Length)
		{
			Array.Resize<GTRendererMatSlot>(ref this.m_gaugeMatSlots, num);
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.OnSnapped));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.OnReleased));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.OnUnsnapped));
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
		this.heldPlayerCallback = new SIGadgetTentacleArm.HeldPlayerCallback(this);
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x00026A73 File Offset: 0x00024C73
	private void Start()
	{
		this.clawVisualPos = this.claw.transform.position;
		this.clawVisualRot = this.claw.transform.rotation;
		this.clawReleasedVisual.SetActive(false);
		this.CallBack();
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x00026AB3 File Offset: 0x00024CB3
	private void OnDestroy()
	{
		if (this.hasRigCallback)
		{
			this.hasRigCallback = false;
			this.rigForCallback.RemoveLateUpdateCallback(this);
		}
		if (this.hasGravityOverride)
		{
			this.RemoveGravityOverride();
		}
		this.heldPlayerCallback.Unregister();
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x00026AEC File Offset: 0x00024CEC
	private void OnGrabbed()
	{
		this.isLeftHanded = this.gameEntity.heldByHandIndex == 0;
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			this.hasRigCallback = true;
			this.rigForCallback = gamePlayer.rig;
			this.rigForCallback.AddLateUpdateCallback(this);
		}
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x00026B40 File Offset: 0x00024D40
	private void OnSnapped()
	{
		this.isLeftHanded = this.gameEntity.snappedJoint == SnapJointType.HandL;
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out gamePlayer))
		{
			this.hasRigCallback = true;
			this.rigForCallback = gamePlayer.rig;
			this.rigForCallback.AddLateUpdateCallback(this);
		}
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x00026B94 File Offset: 0x00024D94
	private void OnReleased()
	{
		this.ClearClawAnchor();
		if (this.hasRigCallback)
		{
			this.hasRigCallback = false;
			this.rigForCallback.RemoveLateUpdateCallback(this);
		}
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x00026BB7 File Offset: 0x00024DB7
	private void OnUnsnapped()
	{
		if (this.hasRigCallback)
		{
			this.hasRigCallback = false;
			this.rigForCallback.RemoveLateUpdateCallback(this);
		}
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x00026BD4 File Offset: 0x00024DD4
	private bool CheckInput()
	{
		return this.buttonActivatable.CheckInput(0.25f);
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x00026BE8 File Offset: 0x00024DE8
	private Vector3 GetIdealClawPosition(VRRig rig)
	{
		Vector3 position = rig.bodyTransform.position;
		position.y += 0.05f;
		Vector3 position2 = base.transform.position;
		Vector3 vector = position2 - position;
		return position2 + vector * this.LengthFactor + base.transform.forward * this.tentacleForwardAdjustment;
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x00026C50 File Offset: 0x00024E50
	protected override void OnUpdateAuthority(float dt)
	{
		bool flag = this.CheckInput();
		if (this.isGripBroken)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				this.isGripBroken = false;
			}
		}
		Vector3 position = base.transform.position;
		Vector3 idealClawPosition = this.GetIdealClawPosition(VRRig.LocalRig);
		Quaternion rotation = base.transform.rotation;
		float num = 0.15f;
		bool flag2 = this.isLowFuel;
		if ((this.knownSafePosition - idealClawPosition).IsLongerThan(1f))
		{
			Vector3 position2 = GTPlayer.Instance.headCollider.transform.position;
			Ray ray = new Ray(position2, idealClawPosition - position2);
			RaycastHit raycastHit;
			if (Physics.SphereCast(ray, num, out raycastHit, (idealClawPosition - position2).magnitude, this.worldCollisionLayers))
			{
				this.knownSafePosition = ray.origin + ray.direction * (raycastHit.distance - num * 2.01f);
			}
			else
			{
				this.knownSafePosition = position;
			}
		}
		if ((this.isAnchored || this.isHoldingHand) && !flag)
		{
			GorillaTagger.Instance.StartVibration(this.isLeftHanded, this.hapticStrengthOnRelease, this.hapticDurationOnRelease);
			this.ClearClawAnchor();
		}
		else
		{
			if (this.isAnchored)
			{
				this.currentFuel = Mathf.Max(0f, this.currentFuel - dt * this._current_grab_fps);
				this.isLowFuel = this.currentFuel < this._lowFuelThreshold;
				if (this.isLowFuel && !flag2)
				{
					this.lowFuelSound.Play();
				}
				this.UpdateFuelGauge();
				if (this.currentFuel == 0f)
				{
					this.isGripBroken = true;
					flag = false;
					this.ClearClawAnchor();
					this.detachFailSound.Play();
				}
				else
				{
					Vector3 position3 = GTPlayer.Instance.transform.position;
					this.clawHoldAdjustment -= position3 - this.lastRequestedPlayerPosition;
					Vector3 vector = this.clawAnchorPosition - (idealClawPosition + this.clawHoldAdjustment);
					(ref vector).ClampThisMagnitudeSafe(this.MaxTentacleJumpSpeed * dt);
					GTPlayer.Instance.RequestTentacleMove(this.isLeftHanded, vector);
					GTPlayer.Instance.TentacleActiveAtFrame = Time.frameCount + 1;
					this.lastRequestedPlayerPosition = position3 + vector;
					if ((this.clawAnchorPosition - base.transform.position).IsLongerThan(this.maxTentacleLength))
					{
						this.isGripBroken = true;
						this.ClearClawAnchor();
						this.detachFailSound.Play();
					}
					else
					{
						this.clawVisualPos = this.clawAnchorPosition;
						this.clawVisualRot = this.clawAnchorRotation;
					}
				}
				this.wasGrabPressed = flag;
				return;
			}
			if (this.isHoldingHand)
			{
				TakeMyHand_HandLink takeMyHand_HandLink = (this.isLeftHanded ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink);
				if (takeMyHand_HandLink.IsLinkActive())
				{
					Vector3 position4 = (this.isLeftHanded ? VRRig.LocalRig.leftHand : VRRig.LocalRig.rightHand).overrideTarget.position;
					takeMyHand_HandLink.TentacleOffset = idealClawPosition - position4;
				}
				else
				{
					this.isGripBroken = true;
					this.ClearClawAnchor();
					this.detachFailSound.Play();
				}
				this.wasGrabPressed = flag;
				return;
			}
		}
		RaycastHit raycastHit2;
		bool flag3 = Physics.SphereCast(new Ray(this.knownSafePosition, idealClawPosition - this.knownSafePosition), num, out raycastHit2, (idealClawPosition - this.knownSafePosition).magnitude, this.worldCollisionLayers);
		Vector3 vector2 = idealClawPosition;
		Quaternion quaternion = rotation;
		bool flag4 = false;
		bool flag5 = this.currentFuel < this.FuelCost_Grab + this.FuelPerSecond_Holding;
		if (flag5 && flag)
		{
			this.isGripBroken = true;
			flag = false;
			this.attachFailSound.Play();
		}
		if (flag3)
		{
			if (!flag5)
			{
				float num2 = Vector3.Dot(raycastHit2.normal, GTPlayerTransform.Instance.GravityUp);
				if (num2 >= this._min_grab_dot)
				{
					this._current_grab_fps = ((num2 >= this._wall_angle_dot) ? this.FuelPerSecond_Holding : (this.FuelPerSecond_Holding * this.FuelCost_Wall_Multiplier));
					flag4 = true;
					if (GTPlayer.Instance.GetSlidePercentage(raycastHit2) > 0.5f)
					{
						if (!this.canHoldSlipperyWalls)
						{
							flag4 = false;
							if (flag && !this.hasFailedToGrab)
							{
								this.attachFailSound.Play();
								this.hasFailedToGrab = true;
							}
						}
						else
						{
							this._current_grab_fps *= this.FuelCost_Slippery_Multiplier;
						}
					}
				}
				else if (flag && !this.hasFailedToGrab)
				{
					this.attachFailSound.Play();
					this.hasFailedToGrab = true;
				}
			}
			this.knownSafePosition += (idealClawPosition - this.knownSafePosition).normalized * (raycastHit2.distance - num * 2.01f);
			vector2 = raycastHit2.point + raycastHit2.normal * 0.1f;
		}
		else
		{
			this.knownSafePosition = idealClawPosition;
		}
		if (flag && flag4)
		{
			vector2 = raycastHit2.point + raycastHit2.normal * 0.01f;
			quaternion = Quaternion.LookRotation(-raycastHit2.normal, rotation * Vector3.up);
			this.SetClawAnchor(vector2, quaternion, vector2 - idealClawPosition);
			GorillaTagger.Instance.StartVibration(this.isLeftHanded, this.hapticStrengthOnGrab, this.hapticDurationOnGrab);
			this.currentFuel -= this.FuelCost_Grab;
		}
		else
		{
			if (flag && !this.wasGrabPressed && (!GorillaComputer.instance.IsPlayerInVirtualStump() || !CustomMapManager.WantsHoldingHandsDisabled()))
			{
				TakeMyHand_HandLink takeMyHand_HandLink2 = (this.isLeftHanded ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink);
				Vector3 position5 = (this.isLeftHanded ? VRRig.LocalRig.leftHand : VRRig.LocalRig.rightHand).overrideTarget.position;
				foreach (VRRig vrrig in VRRigCache.ActiveRigs)
				{
					if (!vrrig.isLocal)
					{
						if (vrrig.leftHandLink.interactionPoint.OverlapCheck(vector2) && vrrig.leftHandLink.CanBeGrabbed())
						{
							if (takeMyHand_HandLink2.TentacleTryCreateLink(vrrig.leftHandLink))
							{
								this.isHoldingHand = true;
								this.clawHoldingVisual.SetActive(true);
								this.clawReleasedVisual.SetActive(false);
								takeMyHand_HandLink2.TentacleOffset = idealClawPosition - position5;
								this.heldPlayerCallback.Register(vrrig, vrrig.leftHandLink);
								this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
								break;
							}
						}
						else if (vrrig.rightHandLink.interactionPoint.OverlapCheck(vector2) && vrrig.rightHandLink.CanBeGrabbed() && takeMyHand_HandLink2.TentacleTryCreateLink(vrrig.rightHandLink))
						{
							this.isHoldingHand = true;
							this.clawHoldingVisual.SetActive(true);
							this.clawReleasedVisual.SetActive(false);
							takeMyHand_HandLink2.TentacleOffset = idealClawPosition - position5;
							this.heldPlayerCallback.Register(vrrig, vrrig.rightHandLink);
							this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
							break;
						}
					}
				}
			}
			Vector3 vector3 = Quaternion.AngleAxis(Time.time * 180f, Vector3.forward) * Vector3.up;
			if (flag4)
			{
				quaternion = Quaternion.Lerp(rotation, Quaternion.LookRotation(-raycastHit2.normal, rotation * Vector3.up), 0.75f);
				quaternion *= Quaternion.AngleAxis(5f, vector3);
			}
			else
			{
				quaternion *= Quaternion.AngleAxis(20f, vector3);
				vector2.y += 0.05f * Mathf.Cos(Time.time * 2f);
			}
		}
		this.clawVisualPos = vector2;
		this.clawVisualRot = quaternion;
		if (!this.isAnchored)
		{
			this.isLowFuel = this.currentFuel < this.FuelCost_Grab;
		}
		this.wasGrabPressed = flag;
		this.UpdateFuelGauge();
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x0002748C File Offset: 0x0002568C
	private void UpdateFuelGauge()
	{
		float num = this.currentFuel / this.fuelSize;
		for (int i = 0; i < this.m_gaugeMatSlots.Length; i++)
		{
			this._gaugeMatPropBlock.SetFloat(ShaderProps._EmissionDissolveProgress, num);
			this.m_gaugeMatSlots[i].renderer.SetPropertyBlock(this._gaugeMatPropBlock, this.m_gaugeMatSlots[i].slot);
		}
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x000274F8 File Offset: 0x000256F8
	protected override void OnUpdateRemote(float dt)
	{
		if (this.isAnchored)
		{
			return;
		}
		VRRig attachedPlayerRig = base.GetAttachedPlayerRig();
		if (attachedPlayerRig == null)
		{
			return;
		}
		Vector3 idealClawPosition = this.GetIdealClawPosition(attachedPlayerRig);
		Quaternion rotation = base.transform.rotation;
		Vector3 position = base.transform.position;
		if ((this.knownSafePosition - idealClawPosition).IsLongerThan(1f))
		{
			this.knownSafePosition = position;
		}
		if (this.isHoldingHand)
		{
			TakeMyHand_HandLink takeMyHand_HandLink = (this.isLeftHanded ? attachedPlayerRig.leftHandLink : attachedPlayerRig.rightHandLink);
			Vector3 position2 = (this.isLeftHanded ? attachedPlayerRig.leftHand : attachedPlayerRig.rightHand).rigTarget.position;
			takeMyHand_HandLink.TentacleOffset = idealClawPosition - position2;
			return;
		}
		float num = 0.15f;
		RaycastHit raycastHit;
		bool flag = Physics.SphereCast(new Ray(this.knownSafePosition, idealClawPosition - this.knownSafePosition), num, out raycastHit, (idealClawPosition - this.knownSafePosition).magnitude, this.worldCollisionLayers);
		Vector3 vector = Quaternion.AngleAxis(Time.time * 180f, Vector3.forward) * Vector3.up;
		Vector3 vector2 = idealClawPosition;
		Quaternion quaternion = rotation;
		if (flag)
		{
			this.knownSafePosition += (idealClawPosition - this.knownSafePosition).normalized * (raycastHit.distance - num * 2.01f);
			vector2 = raycastHit.point + raycastHit.normal * 0.1f;
			quaternion *= Quaternion.AngleAxis(5f, vector);
		}
		else
		{
			this.knownSafePosition = idealClawPosition;
			quaternion *= Quaternion.AngleAxis(20f, vector);
			vector2.y += 0.05f * Mathf.Cos(Time.time * 2f);
		}
		this.clawVisualPos = vector2;
		this.clawVisualRot = quaternion;
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x000276DC File Offset: 0x000258DC
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this.FuelPerSecond_Holding = this._fps_holding_base * (withUpgrades.Contains(SIUpgradeType.Tentacle_Efficiency) ? 0.8f : 1f);
		this.FuelPerSecond_Recharging = this._fps_recharging_base * (withUpgrades.Contains(SIUpgradeType.Tentacle_Charge_Rate) ? 1.2f : 1f);
		this.FuelCost_Grab = this._grabCost_base * (withUpgrades.Contains(SIUpgradeType.Tentacle_Efficiency) ? 0.8f : 1f);
		this.FuelCost_JumpSpeed = this._jumpCost_base * (withUpgrades.Contains(SIUpgradeType.Tentacle_Efficiency) ? 0.8f : 1f);
		this.MaxGrabAngle = (withUpgrades.Contains(SIUpgradeType.Tentacle_Power_Claw) ? 180f : this._grabAngle_base);
		this.MaxTentacleJumpSpeed = this._jumpSpeed_base;
		this._min_grab_dot = Mathf.Cos(0.017453292f * this.MaxGrabAngle);
		this._lowFuelThreshold = this.FuelCost_Grab + this.FuelPerSecond_Holding;
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x000277DC File Offset: 0x000259DC
	private long GetStateLong()
	{
		if (this.isAnchored)
		{
			return 4611686018427387904L | BitPackUtils.PackAnchoredPosRotForNetwork(this.clawAnchorPosition, this.clawAnchorRotation);
		}
		if (this.isHoldingHand)
		{
			TakeMyHand_HandLink takeMyHand_HandLink = (this.isLeftHanded ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink);
			NetPlayer grabbedPlayer = takeMyHand_HandLink.grabbedPlayer;
			int num = ((grabbedPlayer != null) ? grabbedPlayer.ActorNumber : 0);
			return long.MinValue | (takeMyHand_HandLink.grabbedHandIsLeft ? 2305843009213693952L : 0L) | (long)num;
		}
		return 0L;
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x0002786C File Offset: 0x00025A6C
	private void SetClawAnchor(Vector3 clawPosition, Quaternion clawRotation, Vector3 adjustment)
	{
		if (!this.isAnchored)
		{
			this.attachSound.Play();
		}
		this.hasFailedToGrab = false;
		this.isAnchored = true;
		this.clawHoldAdjustment = adjustment;
		this.clawAnchorPosition = clawPosition;
		this.clawAnchorRotation = clawRotation;
		this.clawHoldingVisual.SetActive(true);
		this.clawReleasedVisual.SetActive(false);
		if (this.IsEquippedLocal())
		{
			this.lastRequestedPlayerPosition = GTPlayer.Instance.transform.position;
			this.SetGravityOverride();
			SIPlayer.LocalPlayer.OnKnockback += this.OnKnockback;
			this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
		}
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x0002791C File Offset: 0x00025B1C
	private void ClearClawAnchor()
	{
		if (this.isAnchored || this.isHoldingHand)
		{
			this.detachSound.Play();
		}
		this.hasFailedToGrab = false;
		this.isAnchored = false;
		this.clawHoldingVisual.SetActive(false);
		this.clawReleasedVisual.SetActive(true);
		if (this.isHoldingHand && this.IsEquippedLocal())
		{
			(this.isLeftHanded ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).BreakLink();
		}
		this.isHoldingHand = false;
		if (this.hasGravityOverride)
		{
			this.RemoveGravityOverride();
		}
		if (this.IsEquippedLocal() && !base.IsBlocked(SIExclusionType.AffectsLocalMovement))
		{
			Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
			float num = averagedVelocity.magnitude;
			if (this.FuelCost_JumpSpeed > 0f)
			{
				num = Mathf.Min(num, this.currentFuel / this.FuelCost_JumpSpeed * this.MaxTentacleJumpSpeed);
			}
			num = Mathf.Min(num, this.MaxTentacleJumpSpeed);
			this.currentFuel -= num / this.MaxTentacleJumpSpeed * this.FuelCost_JumpSpeed;
			if (averagedVelocity.IsLongerThan(num))
			{
				GTPlayer.Instance.SetVelocity(averagedVelocity.normalized * num);
			}
			else
			{
				GTPlayer.Instance.SetVelocity(averagedVelocity);
			}
			SIPlayer.LocalPlayer.OnKnockback -= this.OnKnockback;
			this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
		}
	}

	// Token: 0x060006F5 RID: 1781 RVA: 0x00027A8C File Offset: 0x00025C8C
	private void OnKnockback(Vector3 knockbackVector)
	{
		if (this.isAnchored)
		{
			this.isGripBroken = true;
			this.ClearClawAnchor();
		}
	}

	// Token: 0x060006F6 RID: 1782 RVA: 0x00027AA3 File Offset: 0x00025CA3
	private void SetGravityOverride()
	{
		GTPlayer.Instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
		this.hasGravityOverride = true;
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x00027AC3 File Offset: 0x00025CC3
	private void RemoveGravityOverride()
	{
		GTPlayer.Instance.UnsetGravityOverride(this);
		this.hasGravityOverride = false;
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void GravityOverrideFunction(GTPlayer player)
	{
	}

	// Token: 0x060006F9 RID: 1785 RVA: 0x00027AD8 File Offset: 0x00025CD8
	private void OnEntityStateChanged(long oldState, long newState)
	{
		if (this.IsEquippedLocal() || this.activatedLocally)
		{
			return;
		}
		if ((newState & -9223372036854775808L) != 0L)
		{
			this.isHoldingHand = true;
			this.clawHoldingVisual.SetActive(true);
			this.clawReleasedVisual.SetActive(false);
			GamePlayer gamePlayer;
			if (GamePlayer.TryGetGamePlayer((int)newState, out gamePlayer))
			{
				this.heldPlayerCallback.Register(gamePlayer.rig, ((newState & 2305843009213693952L) != 0L) ? gamePlayer.rig.leftHandLink : gamePlayer.rig.rightHandLink);
				return;
			}
		}
		else if (newState != 0L)
		{
			int attachedPlayerActorNr = this.gameEntity.AttachedPlayerActorNr;
			GamePlayer gamePlayer2;
			if (attachedPlayerActorNr >= 1 && GamePlayer.TryGetGamePlayer(attachedPlayerActorNr, out gamePlayer2))
			{
				Vector3 vector;
				Quaternion quaternion;
				BitPackUtils.UnpackAnchoredPosRotForNetwork(newState, gamePlayer2.rig.transform.position, out vector, out quaternion);
				this.SetClawAnchor(vector, quaternion, Vector3.zero);
				this.clawVisualPos = this.clawAnchorPosition;
				this.clawVisualRot = this.clawAnchorRotation;
				return;
			}
		}
		else
		{
			this.ClearClawAnchor();
		}
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x00027BCB File Offset: 0x00025DCB
	public override void OnEntityInit()
	{
		this.currentFuel = 10f;
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x00027BD8 File Offset: 0x00025DD8
	public static Vector3 GetPlaneIntersection(Vector3 p1Pos, Vector3 p1Norm, Vector3 p2Pos, Vector3 p2Norm, Vector3 refPoint)
	{
		Vector3 normalized = Vector3.Cross(p1Norm, p2Norm).normalized;
		float num = Vector3.Dot(p1Pos, p1Norm);
		float num2 = Vector3.Dot(p2Pos, p2Norm);
		float num3 = Vector3.Dot(p1Norm, p2Norm);
		float num4 = 1f - num3 * num3;
		if (Mathf.Abs(num4) < 0.001f)
		{
			return refPoint;
		}
		float num5 = (num - num2 * num3) / num4;
		float num6 = (num2 - num * num3) / num4;
		Vector3 vector = num5 * p1Norm + num6 * p2Norm;
		return vector + Vector3.Project(refPoint - vector, normalized);
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x00027C68 File Offset: 0x00025E68
	public static Vector3 SplineSample(float theta, Vector3 startDir, Vector3 endPos, Vector3 endDir)
	{
		float num = 1f - theta;
		float num2 = Mathf.Lerp(theta * theta, 1f - num * num, theta);
		Vector3 vector = startDir * theta;
		Vector3 vector2 = endPos + endDir * num;
		return Vector3.Lerp(vector, vector2, num2);
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x00027CAC File Offset: 0x00025EAC
	private void UpdateTentacle(Material material, Transform tentacle, Transform anchor)
	{
		Vector3 vector = Vector3.forward * this.LengthFactor;
		material.SetVector(this.tentacleStartDir_HASH, vector);
		Vector3 vector2 = tentacle.InverseTransformPoint(anchor.position);
		material.SetVector(this.tentacleEnd_HASH, vector2);
		Vector3 vector3 = -tentacle.InverseTransformDirection(anchor.forward) * this.LengthFactor;
		material.SetVector(this.tentacleEndDir_HASH, vector3);
		Vector3 vector4 = SIGadgetTentacleArm.SplineSample(0.25f, vector, vector2, vector3);
		Vector3 vector5 = SIGadgetTentacleArm.SplineSample(0.26f, vector, vector2, vector3);
		Vector3 vector6 = SIGadgetTentacleArm.SplineSample(0.75f, vector, vector2, vector3);
		Vector3 vector7 = SIGadgetTentacleArm.SplineSample(0.76f, vector, vector2, vector3);
		Vector3 planeIntersection = SIGadgetTentacleArm.GetPlaneIntersection(vector4, (vector5 - vector4).normalized, vector6, (vector7 - vector6).normalized, Quaternion.AngleAxis(90f, Vector3.forward) * vector2.WithZ(0f).normalized);
		material.SetVector(this.tentacleRingOrigin_HASH, planeIntersection);
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00027DE4 File Offset: 0x00025FE4
	public void CallBack()
	{
		this.lastCallbackFrame = Time.frameCount;
		if (this.isHoldingHand && this.lastHeldCallbackFrame != this.lastCallbackFrame)
		{
			return;
		}
		this.claw.transform.localPosition = Vector3.MoveTowards(this.claw.transform.localPosition, this.claw.transform.parent.InverseTransformPoint(this.clawVisualPos), this.ClawMaxBlendSpeed * Time.deltaTime);
		this.claw.transform.localRotation = Quaternion.RotateTowards(this.claw.transform.localRotation, this.claw.transform.parent.InverseTransformRotation(this.clawVisualRot), this.ClawMaxRotBlendSpeed * Time.deltaTime);
		this.UpdateTentacle(this.tentacleMat, this.tentacleRenderer.transform, this.tentacleAnchor);
		if (this.hasTentacle2)
		{
			this.UpdateTentacle(this.tentacleMat2, this.tentacleRenderer2.transform, this.tentacleAnchor2);
		}
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x00027EF0 File Offset: 0x000260F0
	private void UpdateTentacleHoldingHandPos(TakeMyHand_HandLink heldHandLink)
	{
		if (!this.isHoldingHand)
		{
			this.heldPlayerCallback.Unregister();
			return;
		}
		this.lastHeldCallbackFrame = Time.frameCount;
		this.clawVisualPos = heldHandLink.LinkPosition;
		this.clawVisualRot = heldHandLink.transform.rotation * Quaternion.AngleAxis(90f, Vector3.right);
		if (this.lastHeldCallbackFrame == this.lastCallbackFrame)
		{
			this.CallBack();
		}
	}

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x06000700 RID: 1792 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool UsesEnergy
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x06000701 RID: 1793 RVA: 0x00027F61 File Offset: 0x00026161
	public bool IsFull
	{
		get
		{
			return this.currentFuel >= this.fuelSize;
		}
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x00027F74 File Offset: 0x00026174
	public void UpdateRecharge(float dt)
	{
		if (!this.isAnchored)
		{
			this.currentFuel = Mathf.Clamp(this.currentFuel + dt * this.FuelPerSecond_Recharging, 0f, this.fuelSize);
		}
	}

	// Token: 0x04000870 RID: 2160
	private const string preLog = "[SIGadgetWristJet]  ";

	// Token: 0x04000871 RID: 2161
	private const string preErr = "[SIGadgetWristJet]  ERROR!!!  ";

	// Token: 0x04000872 RID: 2162
	private const string preErrBeta = "[SIGadgetWristJet]  ERROR!!!  (beta only log)  ";

	// Token: 0x04000873 RID: 2163
	[SerializeField]
	private GameObject claw;

	// Token: 0x04000874 RID: 2164
	[SerializeField]
	private GameObject clawHoldingVisual;

	// Token: 0x04000875 RID: 2165
	[SerializeField]
	private GameObject clawReleasedVisual;

	// Token: 0x04000876 RID: 2166
	[SerializeField]
	private LayerMask worldCollisionLayers;

	// Token: 0x04000877 RID: 2167
	[SerializeField]
	private Transform marker;

	// Token: 0x04000878 RID: 2168
	[SerializeField]
	private float maxTentacleLength;

	// Token: 0x04000879 RID: 2169
	[SerializeField]
	private float tentacleForwardAdjustment;

	// Token: 0x0400087A RID: 2170
	[SerializeField]
	private MeshRenderer tentacleRenderer;

	// Token: 0x0400087B RID: 2171
	[SerializeField]
	private Transform tentacleAnchor;

	// Token: 0x0400087C RID: 2172
	[SerializeField]
	private MeshRenderer tentacleRenderer2;

	// Token: 0x0400087D RID: 2173
	[SerializeField]
	private Transform tentacleAnchor2;

	// Token: 0x0400087E RID: 2174
	[SerializeField]
	private SoundBankPlayer attachSound;

	// Token: 0x0400087F RID: 2175
	[SerializeField]
	private SoundBankPlayer detachSound;

	// Token: 0x04000880 RID: 2176
	[SerializeField]
	private SoundBankPlayer attachFailSound;

	// Token: 0x04000881 RID: 2177
	[SerializeField]
	private SoundBankPlayer detachFailSound;

	// Token: 0x04000882 RID: 2178
	[SerializeField]
	private SoundBankPlayer lowFuelSound;

	// Token: 0x04000883 RID: 2179
	[SerializeField]
	private float hapticStrengthOnGrab = 0.5f;

	// Token: 0x04000884 RID: 2180
	[SerializeField]
	private float hapticDurationOnGrab = 0.2f;

	// Token: 0x04000885 RID: 2181
	[SerializeField]
	private float hapticStrengthOnRelease = 0.5f;

	// Token: 0x04000886 RID: 2182
	[SerializeField]
	private float hapticDurationOnRelease = 0.2f;

	// Token: 0x04000887 RID: 2183
	[SerializeField]
	private GameButtonActivatable buttonActivatable;

	// Token: 0x04000888 RID: 2184
	[SerializeField]
	private float ClawMaxBlendSpeed = 10f;

	// Token: 0x04000889 RID: 2185
	[SerializeField]
	private float ClawMaxRotBlendSpeed = 1000f;

	// Token: 0x0400088A RID: 2186
	private MaterialPropertyBlock _gaugeMatPropBlock;

	// Token: 0x0400088B RID: 2187
	[SerializeField]
	private GTRendererMatSlot[] m_gaugeMatSlots;

	// Token: 0x0400088C RID: 2188
	private const float kFUEL_CAPACITY = 10f;

	// Token: 0x0400088D RID: 2189
	private float fuelSize = 10f;

	// Token: 0x0400088E RID: 2190
	private float currentFuel;

	// Token: 0x0400088F RID: 2191
	public float FuelPerSecond_Holding = 1f;

	// Token: 0x04000890 RID: 2192
	public float FuelCost_Wall_Multiplier = 2f;

	// Token: 0x04000891 RID: 2193
	public float FuelCost_Slippery_Multiplier = 2f;

	// Token: 0x04000892 RID: 2194
	public float FuelPerSecond_Recharging = 1f;

	// Token: 0x04000893 RID: 2195
	public float FuelCost_Grab = 1f;

	// Token: 0x04000894 RID: 2196
	public float FuelCost_JumpSpeed = 1f;

	// Token: 0x04000895 RID: 2197
	public float MaxTentacleJumpSpeed = 8f;

	// Token: 0x04000896 RID: 2198
	public float LengthFactor = 1.5f;

	// Token: 0x04000897 RID: 2199
	public float MaxGrabAngle = 60f;

	// Token: 0x04000898 RID: 2200
	public float WallAngle = 60f;

	// Token: 0x04000899 RID: 2201
	public bool canHoldSlipperyWalls;

	// Token: 0x0400089A RID: 2202
	private bool hasTentacle2;

	// Token: 0x0400089B RID: 2203
	private Material tentacleMat;

	// Token: 0x0400089C RID: 2204
	private Material tentacleMat2;

	// Token: 0x0400089D RID: 2205
	private ShaderHashId tentacleStartDir_HASH = "_TentacleStartDir";

	// Token: 0x0400089E RID: 2206
	private ShaderHashId tentacleEnd_HASH = "_TentacleEndPos";

	// Token: 0x0400089F RID: 2207
	private ShaderHashId tentacleEndDir_HASH = "_TentacleEndDir";

	// Token: 0x040008A0 RID: 2208
	private ShaderHashId tentacleRingOrigin_HASH = "_TentacleRingOrigin";

	// Token: 0x040008A1 RID: 2209
	private bool isLeftHanded;

	// Token: 0x040008A2 RID: 2210
	private Vector3 knownSafePosition;

	// Token: 0x040008A3 RID: 2211
	private Vector3 clawHoldAdjustment;

	// Token: 0x040008A4 RID: 2212
	private Vector3 clawAnchorPosition;

	// Token: 0x040008A5 RID: 2213
	private Vector3 lastRequestedPlayerPosition;

	// Token: 0x040008A6 RID: 2214
	private Quaternion clawAnchorRotation;

	// Token: 0x040008A7 RID: 2215
	private bool isGripBroken;

	// Token: 0x040008A8 RID: 2216
	private bool hasGravityOverride;

	// Token: 0x040008A9 RID: 2217
	private bool isLowFuel;

	// Token: 0x040008AA RID: 2218
	private bool hasFailedToGrab;

	// Token: 0x040008AB RID: 2219
	private float _fps_holding_base;

	// Token: 0x040008AC RID: 2220
	private float _fps_recharging_base;

	// Token: 0x040008AD RID: 2221
	private float _grabCost_base;

	// Token: 0x040008AE RID: 2222
	private float _jumpCost_base;

	// Token: 0x040008AF RID: 2223
	private float _jumpSpeed_base;

	// Token: 0x040008B0 RID: 2224
	private float _grabAngle_base;

	// Token: 0x040008B1 RID: 2225
	private float _min_grab_dot;

	// Token: 0x040008B2 RID: 2226
	private float _wall_angle_dot;

	// Token: 0x040008B3 RID: 2227
	private float _current_grab_fps;

	// Token: 0x040008B4 RID: 2228
	private float _lowFuelThreshold;

	// Token: 0x040008B7 RID: 2231
	private SIGadgetTentacleArm.HeldPlayerCallback heldPlayerCallback;

	// Token: 0x040008B8 RID: 2232
	private bool hasRigCallback;

	// Token: 0x040008B9 RID: 2233
	private VRRig rigForCallback;

	// Token: 0x040008BA RID: 2234
	private Vector3 clawVisualPos;

	// Token: 0x040008BB RID: 2235
	private Quaternion clawVisualRot;

	// Token: 0x040008BC RID: 2236
	private bool wasGrabPressed;

	// Token: 0x040008BD RID: 2237
	private const long HoldingLeftHand_Bit = 2305843009213693952L;

	// Token: 0x040008BE RID: 2238
	private const long Anchored_Bit = 4611686018427387904L;

	// Token: 0x040008BF RID: 2239
	private const long HoldingHand_Bit = -9223372036854775808L;

	// Token: 0x040008C0 RID: 2240
	private int lastCallbackFrame;

	// Token: 0x040008C1 RID: 2241
	private int lastHeldCallbackFrame;

	// Token: 0x02000118 RID: 280
	private class HeldPlayerCallback : ICallBack
	{
		// Token: 0x06000704 RID: 1796 RVA: 0x000280B2 File Offset: 0x000262B2
		public HeldPlayerCallback(SIGadgetTentacleArm parent)
		{
			this.parent = parent;
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x000280C1 File Offset: 0x000262C1
		public void Register(VRRig heldPlayer, TakeMyHand_HandLink heldHandLink)
		{
			this.Unregister();
			this.heldRig = heldPlayer;
			this.heldHandLink = heldHandLink;
			heldPlayer.AddLateUpdateCallback(this);
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x000280DE File Offset: 0x000262DE
		public void Unregister()
		{
			if (this.heldRig != null)
			{
				this.heldRig.RemoveLateUpdateCallback(this);
			}
			this.heldRig = null;
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x00028101 File Offset: 0x00026301
		public void CallBack()
		{
			this.parent.UpdateTentacleHoldingHandPos(this.heldHandLink);
		}

		// Token: 0x040008C2 RID: 2242
		private SIGadgetTentacleArm parent;

		// Token: 0x040008C3 RID: 2243
		private VRRig heldRig;

		// Token: 0x040008C4 RID: 2244
		private TakeMyHand_HandLink heldHandLink;
	}
}
