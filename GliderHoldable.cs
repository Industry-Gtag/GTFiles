using System;
using System.Collections;
using System.Runtime.InteropServices;
using AA;
using Fusion;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000CE3 RID: 3299
[RequireComponent(typeof(Rigidbody))]
[NetworkBehaviourWeaved(11)]
public class GliderHoldable : NetworkHoldableObject, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x170007BD RID: 1981
	// (get) Token: 0x0600519C RID: 20892 RVA: 0x001B00E8 File Offset: 0x001AE2E8
	private bool OutOfBounds
	{
		get
		{
			return this.maxDistanceRespawnOrigin != null && (this.maxDistanceRespawnOrigin.position - base.transform.position).sqrMagnitude > this.maxDistanceBeforeRespawn * this.maxDistanceBeforeRespawn;
		}
	}

	// Token: 0x0600519D RID: 20893 RVA: 0x001B0138 File Offset: 0x001AE338
	protected override void Awake()
	{
		base.Awake();
		base.transform.parent = null;
		this.defaultMaxDistanceBeforeRespawn = this.maxDistanceBeforeRespawn;
		this.spawnPosition = (this.skyJungleSpawnPostion = base.transform.position);
		this.spawnRotation = (this.skyJungleSpawnRotation = base.transform.rotation);
		this.skyJungleRespawnOrigin = this.maxDistanceRespawnOrigin;
		this.syncedState.Init(this.spawnPosition, this.spawnRotation);
		this.rb = base.GetComponent<Rigidbody>();
		this.yaw = base.transform.rotation.eulerAngles.y;
		this.oneHandRotationRateExp = Mathf.Exp(this.oneHandHoldRotationRate);
		this.twoHandRotationRateExp = Mathf.Exp(this.twoHandHoldRotationRate);
		this.subtlePlayerPitchRateExp = Mathf.Exp(this.subtlePlayerPitchRate);
		this.subtlePlayerRollRateExp = Mathf.Exp(this.subtlePlayerRollRate);
		this.accelSmoothingFollowRateExp = Mathf.Exp(this.accelSmoothingFollowRate);
		this.networkSyncFollowRateExp = Mathf.Exp(this.networkSyncFollowRate);
		this.ownershipGuard.AddCallbackTarget(this);
		this.calmAudio.volume = 0f;
		this.activeAudio.volume = 0f;
		this.whistlingAudio.volume = 0f;
	}

	// Token: 0x0600519E RID: 20894 RVA: 0x001B0286 File Offset: 0x001AE486
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		if (this.ownershipGuard != null)
		{
			this.ownershipGuard.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x0600519F RID: 20895 RVA: 0x000F8BDC File Offset: 0x000F6DDC
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
	}

	// Token: 0x060051A0 RID: 20896 RVA: 0x001B02A8 File Offset: 0x001AE4A8
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		this.Respawn();
		base.OnDisable();
	}

	// Token: 0x060051A1 RID: 20897 RVA: 0x001B02BC File Offset: 0x001AE4BC
	public void Respawn()
	{
		if ((base.IsValid && base.IsMine) || !NetworkSystem.Instance.InRoom)
		{
			if (EquipmentInteractor.instance != null)
			{
				if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
				{
					this.OnRelease(null, EquipmentInteractor.instance.leftHand);
				}
				if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
				{
					this.OnRelease(null, EquipmentInteractor.instance.rightHand);
				}
			}
			this.rb.isKinematic = true;
			base.transform.position = this.spawnPosition;
			base.transform.rotation = this.spawnRotation;
			this.lastHeldTime = -1f;
			this.syncedState.Init(this.spawnPosition, this.spawnRotation);
		}
	}

	// Token: 0x060051A2 RID: 20898 RVA: 0x001B038D File Offset: 0x001AE58D
	public void CustomMapLoad(Transform placeholderTransform, float respawnDistance)
	{
		this.maxDistanceRespawnOrigin = placeholderTransform;
		this.spawnPosition = placeholderTransform.position;
		this.spawnRotation = placeholderTransform.rotation;
		this.maxDistanceBeforeRespawn = respawnDistance;
		this.Respawn();
	}

	// Token: 0x060051A3 RID: 20899 RVA: 0x001B03BB File Offset: 0x001AE5BB
	public void CustomMapUnload()
	{
		this.maxDistanceRespawnOrigin = this.skyJungleRespawnOrigin;
		this.spawnPosition = this.skyJungleSpawnPostion;
		this.spawnRotation = this.skyJungleSpawnRotation;
		this.maxDistanceBeforeRespawn = this.defaultMaxDistanceBeforeRespawn;
		this.Respawn();
	}

	// Token: 0x170007BE RID: 1982
	// (get) Token: 0x060051A4 RID: 20900 RVA: 0x00023F0C File Offset: 0x0002210C
	public override bool TwoHanded
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060051A5 RID: 20901 RVA: 0x001B03F4 File Offset: 0x001AE5F4
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest && this.syncedState.riderId == -1)
		{
			this.ownershipGuard.RequestOwnershipImmediately(delegate
			{
				this.pendingOwnershipRequest = false;
			});
			this.pendingOwnershipRequest = true;
			if (this.reenableOwnershipRequestCoroutine != null)
			{
				base.StopCoroutine(this.reenableOwnershipRequestCoroutine);
			}
			this.reenableOwnershipRequestCoroutine = base.StartCoroutine(this.ReenableOwnershipRequest());
		}
	}

	// Token: 0x060051A6 RID: 20902 RVA: 0x001B0470 File Offset: 0x001AE670
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (base.IsMine || !NetworkSystem.Instance.InRoom || this.pendingOwnershipRequest)
		{
			this.OnGrabAuthority(pointGrabbed, grabbingHand);
			return;
		}
		if (NetworkSystem.Instance.InRoom && !base.IsMine && !this.pendingOwnershipRequest && this.syncedState.riderId == -1)
		{
			this.ownershipGuard.RequestOwnershipImmediately(delegate
			{
				this.pendingOwnershipRequest = false;
			});
			this.pendingOwnershipRequest = true;
			if (this.reenableOwnershipRequestCoroutine != null)
			{
				base.StopCoroutine(this.reenableOwnershipRequestCoroutine);
			}
			this.reenableOwnershipRequestCoroutine = base.StartCoroutine(this.ReenableOwnershipRequest());
			this.OnGrabAuthority(pointGrabbed, grabbingHand);
		}
	}

	// Token: 0x060051A7 RID: 20903 RVA: 0x001B0518 File Offset: 0x001AE718
	public void OnGrabAuthority(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		if ((flag && !EquipmentInteractor.instance.isLeftGrabbing) || (!flag && !EquipmentInteractor.instance.isRightGrabbing))
		{
			return;
		}
		if (this.riderId != NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.riderId = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		Vector3 vector = this.ClosestPointInHandle(grabbingHand.transform.position, pointGrabbed);
		if (flag)
		{
			this.leftHold.Activate(grabbingHand.transform, base.transform, vector);
		}
		else
		{
			this.rightHold.Activate(grabbingHand.transform, base.transform, vector);
		}
		if (this.leftHold.active && this.rightHold.active)
		{
			Vector3 handsVector = this.GetHandsVector(this.leftHold.transform.position, this.rightHold.transform.position, GTPlayer.Instance.headCollider.transform.position, true);
			this.twoHandRotationOffsetAxis = Vector3.Cross(handsVector, base.transform.right).normalized;
			if ((double)this.twoHandRotationOffsetAxis.sqrMagnitude < 0.001)
			{
				this.twoHandRotationOffsetAxis = base.transform.right;
				this.twoHandRotationOffsetAngle = 0f;
			}
			else
			{
				this.twoHandRotationOffsetAngle = Vector3.SignedAngle(handsVector, base.transform.right, this.twoHandRotationOffsetAxis);
			}
		}
		this.rb.isKinematic = true;
		this.rb.useGravity = false;
		this.ridersMaterialOverideIndex = 0;
		if (this.cosmeticMaterialOverrides.Length != 0)
		{
			VRRig offlineVRRig = this.cachedRig;
			if (offlineVRRig == null)
			{
				offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			}
			if (offlineVRRig != null)
			{
				for (int i = 0; i < this.cosmeticMaterialOverrides.Length; i++)
				{
					if (this.cosmeticMaterialOverrides[i].cosmeticName != null && offlineVRRig.cosmeticSet != null && offlineVRRig.cosmeticSet.HasItem(this.cosmeticMaterialOverrides[i].cosmeticName))
					{
						this.ridersMaterialOverideIndex = i + 1;
						break;
					}
				}
			}
		}
		this.infectedState = false;
		if (GorillaGameManager.instance as GorillaTagManager != null)
		{
			this.infectedState = this.syncedState.tagged;
		}
		if (this.infectedState)
		{
			this.leafMesh.material = this.GetInfectedMaterial();
		}
		else
		{
			this.leafMesh.material = this.GetMaterialFromIndex((byte)this.ridersMaterialOverideIndex);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment.GetType() == typeof(GliderHoldable) && EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment.GetType() == typeof(GliderHoldable) && EquipmentInteractor.instance.leftHandHeldEquipment != EquipmentInteractor.instance.rightHandHeldEquipment)
		{
			this.holdingTwoGliders = true;
		}
	}

	// Token: 0x060051A8 RID: 20904 RVA: 0x001B08A4 File Offset: 0x001AEAA4
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		this.holdingTwoGliders = false;
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		if (this.leftHold.active && this.rightHold.active)
		{
			if (flag)
			{
				this.rightHold.Activate(this.rightHold.transform, base.transform, this.ClosestPointInHandle(this.rightHold.transform.position, this.handle));
			}
			else
			{
				this.leftHold.Activate(this.leftHold.transform, base.transform, this.ClosestPointInHandle(this.leftHold.transform.position, this.handle));
			}
		}
		Vector3 averageVelocity = GTPlayer.Instance.GetHandVelocityTracker(flag).GetAverageVelocity(true, 0.15f, false);
		(flag ? this.leftHold : this.rightHold).Deactivate();
		EquipmentInteractor.instance.UpdateHandEquipment(null, flag);
		if (!this.leftHold.active && !this.rightHold.active)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyDropped;
			this.audioLevel = 0f;
			this.riderId = -1;
			this.cachedRig = null;
			this.subtlePlayerPitch = 0f;
			this.subtlePlayerRoll = 0f;
			this.leftHoldPositionLocal = null;
			this.rightHoldPositionLocal = null;
			this.ridersMaterialOverideIndex = 0;
			if (base.IsMine || !NetworkSystem.Instance.InRoom)
			{
				this.rb.isKinematic = false;
				this.rb.useGravity = true;
				this.rb.linearVelocity = averageVelocity;
				this.syncedState.riderId = -1;
				this.syncedState.tagged = false;
				this.syncedState.materialIndex = 0;
				this.syncedState.position = base.transform.position;
				this.syncedState.rotation = base.transform.rotation;
				this.syncedState.audioLevel = 0;
			}
			this.leafMesh.material = this.baseLeafMaterial;
		}
		return true;
	}

	// Token: 0x060051A9 RID: 20905 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void DropItemCleanup()
	{
	}

	// Token: 0x060051AA RID: 20906 RVA: 0x001B0ABC File Offset: 0x001AECBC
	public void FixedUpdate()
	{
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		GTPlayer instance = GTPlayer.Instance;
		if (this.holdingTwoGliders)
		{
			instance.AddForce(Physics.gravity, ForceMode.Acceleration);
			return;
		}
		if (this.leftHold.active || this.rightHold.active)
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.previousVelocity = this.currentVelocity;
			this.currentVelocity = instance.RigidbodyVelocity;
			float magnitude = this.currentVelocity.magnitude;
			this.accelerationAverage.AddSample((this.currentVelocity - this.previousVelocity) / Time.fixedDeltaTime, Time.fixedTime);
			float rollAngle180Wrapping = this.GetRollAngle180Wrapping();
			float num = this.liftIncreaseVsRoll.Evaluate(Mathf.Clamp01(Mathf.Abs(rollAngle180Wrapping / 180f))) * this.liftIncreaseVsRollMaxAngle;
			Vector3 vector = Vector3.RotateTowards(this.currentVelocity, Quaternion.AngleAxis(num, -base.transform.right) * base.transform.forward * magnitude, this.pitchVelocityFollowRateAngle * 0.017453292f * fixedDeltaTime, this.pitchVelocityFollowRateMagnitude * fixedDeltaTime);
			Vector3 vector2 = vector - this.currentVelocity;
			float num2 = this.NormalizeAngle180(Vector3.SignedAngle(Vector3.ProjectOnPlane(this.currentVelocity, base.transform.right), base.transform.forward, base.transform.right));
			if (num2 > 90f)
			{
				num2 = Mathf.Lerp(0f, 90f, Mathf.InverseLerp(180f, 90f, num2));
			}
			else if (num2 < -90f)
			{
				num2 = Mathf.Lerp(0f, -90f, Mathf.InverseLerp(-180f, -90f, num2));
			}
			float num3 = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-90f, 90f, num2));
			Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-90f, 90f, this.pitch));
			float num4 = this.liftVsAttack.Evaluate(num3);
			instance.AddForce(vector2 * num4, ForceMode.VelocityChange);
			float num5 = this.dragVsAttack.Evaluate(num3);
			float num6 = ((this.syncedState.riderId != -1 && this.syncedState.materialIndex == 1) ? (this.dragVsSpeedMaxSpeed + this.infectedSpeedIncrease) : this.dragVsSpeedMaxSpeed);
			float num7 = this.dragVsSpeed.Evaluate(Mathf.Clamp01(magnitude / num6));
			float num8 = Mathf.Clamp01(num5 * this.attackDragFactor + num7 * this.dragVsSpeedDragFactor);
			instance.AddForce(-this.currentVelocity * num8, ForceMode.Acceleration);
			if (this.pitch > 0f && this.currentVelocity.y > 0f && (this.currentVelocity - this.previousVelocity).y > 0f)
			{
				float num9 = Mathf.InverseLerp(0f, this.pullUpLiftActivationVelocity, this.currentVelocity.y);
				float num10 = Mathf.InverseLerp(0f, this.pullUpLiftActivationAcceleration, (this.currentVelocity - this.previousVelocity).y / fixedDeltaTime);
				float num11 = Mathf.Min(num9, num10);
				instance.AddForce(-Physics.gravity * this.pullUpLiftBonus * num11, ForceMode.Acceleration);
			}
			if (Vector3.Dot(vector, Physics.gravity) > 0f)
			{
				instance.AddForce(-Physics.gravity * this.gravityCompensation, ForceMode.Acceleration);
				return;
			}
		}
		else
		{
			Vector3 vector3 = this.WindResistanceForceOffset(base.transform.up, Vector3.down);
			Vector3 vector4 = base.transform.position - vector3 * this.gravityUprightTorqueMultiplier;
			this.rb.AddForceAtPosition(-this.fallingGravityReduction * Physics.gravity * this.rb.mass, vector4, ForceMode.Force);
		}
	}

	// Token: 0x060051AB RID: 20907 RVA: 0x001B0EC4 File Offset: 0x001AF0C4
	public void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		if (base.IsMine || !NetworkSystem.Instance.InRoom || this.pendingOwnershipRequest)
		{
			this.AuthorityUpdate(deltaTime);
			return;
		}
		this.RemoteSyncUpdate(deltaTime);
	}

	// Token: 0x060051AC RID: 20908 RVA: 0x001B0F04 File Offset: 0x001AF104
	private void AuthorityUpdate(float dt)
	{
		if (!this.leftHold.active && !this.rightHold.active)
		{
			this.AuthorityUpdateUnheld(dt);
		}
		else if (this.leftHold.active || this.rightHold.active)
		{
			this.AuthorityUpdateHeld(dt);
		}
		this.syncedState.audioLevel = (byte)Mathf.FloorToInt(255f * this.audioLevel);
	}

	// Token: 0x060051AD RID: 20909 RVA: 0x001B0F74 File Offset: 0x001AF174
	private void AuthorityUpdateHeld(float dt)
	{
		if (this.gliderState != GliderHoldable.GliderState.LocallyHeld)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyHeld;
		}
		this.rb.isKinematic = true;
		this.lastHeldTime = Time.time;
		if (this.leftHold.active)
		{
			this.leftHold.holdLocalPos = Vector3.Lerp(Vector3.zero, this.leftHold.holdLocalPos, Mathf.Exp(-5f * dt));
		}
		if (this.rightHold.active)
		{
			this.rightHold.holdLocalPos = Vector3.Lerp(Vector3.zero, this.rightHold.holdLocalPos, Mathf.Exp(-5f * dt));
		}
		Vector3 vector = Vector3.zero;
		if (this.leftHold.active && this.rightHold.active)
		{
			vector = (this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos)) * 0.5f;
		}
		else if (this.leftHold.active)
		{
			vector = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos);
		}
		else if (this.rightHold.active)
		{
			vector = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos);
		}
		this.UpdateGliderPosition();
		float magnitude = this.currentVelocity.magnitude;
		if (this.setMaxHandSlipDuringFlight && magnitude > this.maxSlipOverrideSpeedThreshold)
		{
			if (this.leftHold.active)
			{
				GTPlayer.Instance.SetLeftMaximumSlipThisFrame();
			}
			if (this.rightHold.active)
			{
				GTPlayer.Instance.SetRightMaximumSlipThisFrame();
			}
		}
		bool flag = false;
		GorillaTagManager gorillaTagManager = GorillaGameManager.instance as GorillaTagManager;
		if (gorillaTagManager != null)
		{
			flag = gorillaTagManager.IsInfected(NetworkSystem.Instance.LocalPlayer);
		}
		bool flag2 = flag != this.infectedState;
		this.infectedState = flag;
		if (flag2)
		{
			if (this.infectedState)
			{
				this.leafMesh.material = this.GetInfectedMaterial();
			}
			else
			{
				this.leafMesh.material = this.GetMaterialFromIndex(this.syncedState.materialIndex);
			}
		}
		Vector3 average = this.accelerationAverage.GetAverage();
		this.accelerationSmoothed = Mathf.Lerp(average.magnitude, this.accelerationSmoothed, Mathf.Exp(-this.accelSmoothingFollowRateExp * dt));
		float num = Mathf.InverseLerp(this.hapticMaxSpeedInputRange.x, this.hapticMaxSpeedInputRange.y, magnitude);
		float num2 = Mathf.InverseLerp(this.hapticAccelInputRange.x, this.hapticAccelInputRange.y, this.accelerationSmoothed);
		float num3 = Mathf.InverseLerp(this.hapticSpeedInputRange.x, this.hapticSpeedInputRange.y, magnitude);
		this.UpdateAudioSource(this.calmAudio, num * this.audioVolumeMultiplier);
		this.UpdateAudioSource(this.activeAudio, num2 * num * this.audioVolumeMultiplier);
		if (this.infectedState)
		{
			this.UpdateAudioSource(this.whistlingAudio, Mathf.InverseLerp(this.whistlingAudioSpeedInputRange.x, this.whistlingAudioSpeedInputRange.y, magnitude) * num2 * num * this.audioVolumeMultiplier);
		}
		else
		{
			this.UpdateAudioSource(this.whistlingAudio, 0f);
		}
		float num4 = Mathf.Max(num2 * this.hapticAccelOutputMax * num, num3 * this.hapticSpeedOutputMax);
		if (this.rightHold.active)
		{
			GorillaTagger.Instance.DoVibration(XRNode.RightHand, num4, dt);
		}
		if (this.leftHold.active)
		{
			GorillaTagger.Instance.DoVibration(XRNode.LeftHand, num4, dt);
		}
		Vector3 vector2 = this.handle.transform.position + this.handle.transform.rotation * new Vector3(0f, 0f, 1f);
		if (Time.frameCount % 2 == 0)
		{
			Vector3 vector3 = this.handle.transform.rotation * new Vector3(-0.707f, 0f, 0.707f);
			RaycastHit raycastHit;
			if (this.leftWhooshStartTime < Time.time - this.whooshSoundRetriggerThreshold && magnitude > this.whooshSpeedThresholdInput.x && Physics.Raycast(new Ray(vector2, vector3), out raycastHit, this.whooshCheckDistance, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				this.leftWhooshStartTime = Time.time;
				this.leftWhooshHitPoint = raycastHit.point;
				this.leftWhooshAudio.GTStop();
				this.leftWhooshAudio.volume = Mathf.Lerp(this.whooshVolumeOutput.x, this.whooshVolumeOutput.y, Mathf.InverseLerp(this.whooshSpeedThresholdInput.x, this.whooshSpeedThresholdInput.y, magnitude));
				this.leftWhooshAudio.GTPlay();
			}
		}
		else
		{
			Vector3 vector4 = this.handle.transform.rotation * new Vector3(0.707f, 0f, 0.707f);
			RaycastHit raycastHit2;
			if (this.rightWhooshStartTime < Time.time - this.whooshSoundRetriggerThreshold && magnitude > this.whooshSpeedThresholdInput.x && Physics.Raycast(new Ray(vector2, vector4), out raycastHit2, this.whooshCheckDistance, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				this.rightWhooshStartTime = Time.time;
				this.rightWhooshHitPoint = raycastHit2.point;
				this.rightWhooshAudio.GTStop();
				this.rightWhooshAudio.volume = Mathf.Lerp(this.whooshVolumeOutput.x, this.whooshVolumeOutput.y, Mathf.InverseLerp(this.whooshSpeedThresholdInput.x, this.whooshSpeedThresholdInput.y, magnitude));
				this.rightWhooshAudio.GTPlay();
			}
		}
		Vector3 headCenterPosition = GTPlayer.Instance.HeadCenterPosition;
		if (this.leftWhooshStartTime > Time.time - this.whooshSoundDuration)
		{
			this.leftWhooshAudio.transform.position = this.leftWhooshHitPoint;
		}
		else
		{
			this.leftWhooshAudio.transform.localPosition = new Vector3(-this.whooshAudioPositionOffset.x, this.whooshAudioPositionOffset.y, this.whooshAudioPositionOffset.z);
		}
		if (this.rightWhooshStartTime > Time.time - this.whooshSoundDuration)
		{
			this.rightWhooshAudio.transform.position = this.rightWhooshHitPoint;
		}
		else
		{
			this.rightWhooshAudio.transform.localPosition = new Vector3(this.whooshAudioPositionOffset.x, this.whooshAudioPositionOffset.y, this.whooshAudioPositionOffset.z);
		}
		if (this.extendTagRangeInFlight)
		{
			float num5 = Mathf.Lerp(this.tagRangeOutput.x, this.tagRangeOutput.y, Mathf.InverseLerp(this.tagRangeSpeedInput.x, this.tagRangeSpeedInput.y, magnitude));
			GorillaTagger.Instance.SetTagRadiusOverrideThisFrame(num5);
			if (this.debugDrawTagRange)
			{
				GorillaTagger.Instance.DebugDrawTagCasts(Color.yellow);
			}
		}
		Vector3 normalized = Vector3.ProjectOnPlane(base.transform.right, Vector3.up).normalized;
		Vector3 normalized2 = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
		float num6 = -Vector3.Dot(vector - this.handle.transform.position, normalized2);
		Vector3 vector5 = this.handle.transform.position - normalized2 * (this.riderPosRange.y * 0.5f + this.riderPosRangeOffset + num6);
		float num7 = Vector3.Dot(headCenterPosition - vector5, normalized);
		float num8 = Vector3.Dot(headCenterPosition - vector5, normalized2);
		num7 /= this.riderPosRange.x * 0.5f;
		num8 /= this.riderPosRange.y * 0.5f;
		this.riderPosition.x = Mathf.Sign(num7) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.riderPosRangeNormalizedDeadzone.x, 1f, Mathf.Abs(num7)));
		this.riderPosition.y = Mathf.Sign(num8) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.riderPosRangeNormalizedDeadzone.y, 1f, Mathf.Abs(num8)));
		Vector3 vector6;
		Vector3 vector7;
		if (this.leftHold.active && this.rightHold.active)
		{
			vector6 = this.leftHold.transform.position;
			this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector6));
			vector7 = this.rightHold.transform.position;
			this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector7));
		}
		else if (this.leftHold.active)
		{
			vector6 = this.leftHold.transform.position;
			this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector6));
			Vector3 vector8 = vector6 + this.leftHold.transform.forward * this.oneHandSimulatedHoldOffset.x;
			if (this.rightHoldPositionLocal != null)
			{
				this.rightHoldPositionLocal = new Vector3?(Vector3.Lerp(GTPlayer.Instance.transform.InverseTransformPoint(vector8), this.rightHoldPositionLocal.Value, Mathf.Exp(-5f * dt)));
				vector7 = GTPlayer.Instance.transform.TransformPoint(this.rightHoldPositionLocal.Value);
			}
			else
			{
				vector7 = vector8;
				this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector7));
			}
		}
		else
		{
			vector7 = this.rightHold.transform.position;
			this.rightHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector7));
			Vector3 vector9 = vector7 + this.rightHold.transform.forward * this.oneHandSimulatedHoldOffset.x;
			if (this.leftHoldPositionLocal != null)
			{
				this.leftHoldPositionLocal = new Vector3?(Vector3.Lerp(GTPlayer.Instance.transform.InverseTransformPoint(vector9), this.leftHoldPositionLocal.Value, Mathf.Exp(-5f * dt)));
				vector6 = GTPlayer.Instance.transform.TransformPoint(this.leftHoldPositionLocal.Value);
			}
			else
			{
				vector6 = vector9;
				this.leftHoldPositionLocal = new Vector3?(GTPlayer.Instance.transform.InverseTransformPoint(vector6));
			}
		}
		Vector3 vector10;
		Vector3 vector11;
		this.GetHandsOrientationVectors(vector6, vector7, GTPlayer.Instance.headCollider.transform, false, out vector10, out vector11);
		float num9 = this.riderPosition.y * this.riderPosDirectPitchMax;
		if (!this.leftHold.active || !this.rightHold.active)
		{
			num9 *= this.oneHandPitchMultiplier;
		}
		Spring.CriticalSpringDamperExact(ref this.pitch, ref this.pitchVel, num9, 0f, this.pitchHalfLife, dt);
		this.pitch = Mathf.Clamp(this.pitch, this.pitchMinMax.x, this.pitchMinMax.y);
		Quaternion quaternion = Quaternion.AngleAxis(this.pitch, Vector3.right);
		this.twoHandRotationOffsetAngle = Mathf.Lerp(0f, this.twoHandRotationOffsetAngle, Mathf.Exp(-8f * dt));
		Vector3 vector12 = (this.twoHandGliderInversionOnYawInsteadOfRoll ? vector11 : Vector3.up);
		Quaternion quaternion2 = Quaternion.AngleAxis(this.twoHandRotationOffsetAngle, this.twoHandRotationOffsetAxis) * Quaternion.LookRotation(vector10, vector12) * Quaternion.AngleAxis(-90f, Vector3.up);
		float num10 = ((this.leftHold.active && this.rightHold.active) ? this.twoHandRotationRateExp : this.oneHandRotationRateExp);
		base.transform.rotation = Quaternion.Slerp(quaternion2 * quaternion, base.transform.rotation, Mathf.Exp(-num10 * dt));
		if (this.subtlePlayerPitchActive || this.subtlePlayerRollActive)
		{
			float num11 = Mathf.InverseLerp(this.subtlePlayerRotationSpeedRampMinMax.x, this.subtlePlayerRotationSpeedRampMinMax.y, this.currentVelocity.magnitude);
			Quaternion quaternion3 = Quaternion.identity;
			if (this.subtlePlayerRollActive)
			{
				float num12 = this.GetRollAngle180Wrapping();
				if (num12 > 90f)
				{
					num12 = Mathf.Lerp(0f, 90f, Mathf.InverseLerp(180f, 90f, num12));
				}
				else if (num12 < -90f)
				{
					num12 = Mathf.Lerp(0f, -90f, Mathf.InverseLerp(-180f, -90f, num12));
				}
				Vector3 normalized3 = new Vector3(this.currentVelocity.x, 0f, this.currentVelocity.z).normalized;
				Vector3 vector13 = new Vector3(average.x, 0f, average.z);
				float num13 = Vector3.Dot(vector13 - Vector3.Dot(vector13, normalized3) * normalized3, Vector3.Cross(normalized3, Vector3.up));
				this.turnAccelerationSmoothed = Mathf.Lerp(num13, this.turnAccelerationSmoothed, Mathf.Exp(-this.accelSmoothingFollowRateExp * dt));
				float num14 = 0f;
				if (num13 * num12 > 0f)
				{
					num14 = Mathf.InverseLerp(this.subtlePlayerRollAccelMinMax.x, this.subtlePlayerRollAccelMinMax.y, Mathf.Abs(this.turnAccelerationSmoothed));
				}
				float num15 = num12 * this.subtlePlayerRollFactor * Mathf.Min(num11, num14);
				this.subtlePlayerRoll = Mathf.Lerp(num15, this.subtlePlayerRoll, Mathf.Exp(-this.subtlePlayerRollRateExp * dt));
				quaternion3 = Quaternion.AngleAxis(this.subtlePlayerRoll, base.transform.forward);
			}
			Quaternion quaternion4 = Quaternion.identity;
			if (this.subtlePlayerPitchActive)
			{
				float num16 = this.pitch * this.subtlePlayerPitchFactor * Mathf.Min(num11, 1f);
				this.subtlePlayerPitch = Mathf.Lerp(num16, this.subtlePlayerPitch, Mathf.Exp(-this.subtlePlayerPitchRateExp * dt));
				quaternion4 = Quaternion.AngleAxis(this.subtlePlayerPitch, -base.transform.right);
			}
			Quaternion quaternion5 = quaternion4 * quaternion3;
			GTPlayerTransform.ApplyRotationOverride(in quaternion5, Time.frameCount);
		}
		this.UpdateGliderPosition();
		if (this.syncedState.riderId != NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.riderId = (this.syncedState.riderId = NetworkSystem.Instance.LocalPlayer.ActorNumber);
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		this.syncedState.tagged = this.infectedState;
		this.syncedState.materialIndex = (byte)this.ridersMaterialOverideIndex;
		if (this.cachedRig != null)
		{
			this.syncedState.position = this.cachedRig.transform.InverseTransformPoint(base.transform.position);
			this.syncedState.rotation = Quaternion.Inverse(this.cachedRig.transform.rotation) * base.transform.rotation;
		}
		else
		{
			Debug.LogError("Glider failed to get a reference to the local player's VRRig while the player was flying", this);
		}
		this.audioLevel = num2 * num;
		if (this.OutOfBounds)
		{
			this.Respawn();
		}
		if (this.leftHold.active && EquipmentInteractor.instance.leftHandHeldEquipment != this)
		{
			this.OnRelease(null, EquipmentInteractor.instance.leftHand);
		}
		if (this.rightHold.active && EquipmentInteractor.instance.rightHandHeldEquipment != this)
		{
			this.OnRelease(null, EquipmentInteractor.instance.rightHand);
		}
	}

	// Token: 0x060051AE RID: 20910 RVA: 0x001B1F20 File Offset: 0x001B0120
	private void AuthorityUpdateUnheld(float dt)
	{
		this.syncedState.position = base.transform.position;
		this.syncedState.rotation = base.transform.rotation;
		if (this.gliderState != GliderHoldable.GliderState.LocallyDropped)
		{
			this.gliderState = GliderHoldable.GliderState.LocallyDropped;
			this.syncedState.riderId = -1;
			this.syncedState.materialIndex = 0;
			this.syncedState.tagged = false;
			this.leafMesh.material = this.baseLeafMaterial;
		}
		if (this.audioLevel * this.audioVolumeMultiplier > 0.001f)
		{
			this.audioLevel = Mathf.Lerp(0f, this.audioLevel, Mathf.Exp(-2f * dt));
			this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.whistlingAudio, this.audioLevel * this.audioVolumeMultiplier);
		}
		if (this.OutOfBounds || (this.lastHeldTime > 0f && this.lastHeldTime < Time.time - this.maxDroppedTimeToRespawn))
		{
			this.Respawn();
		}
	}

	// Token: 0x060051AF RID: 20911 RVA: 0x001B2054 File Offset: 0x001B0254
	private void RemoteSyncUpdate(float dt)
	{
		this.rb.isKinematic = true;
		int num = this.syncedState.riderId;
		bool flag = this.riderId != num;
		if (flag)
		{
			this.riderId = num;
			this.cachedRig = this.getNewHolderRig(this.riderId);
		}
		if (this.riderId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.cachedRig = null;
			this.syncedState.riderId = -1;
			this.syncedState.materialIndex = 0;
			this.syncedState.audioLevel = 0;
		}
		if (this.syncedState.riderId == -1)
		{
			base.transform.position = Vector3.Lerp(this.syncedState.position, base.transform.position, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			base.transform.rotation = Quaternion.Slerp(this.syncedState.rotation, base.transform.rotation, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
		}
		else if (this.cachedRig != null)
		{
			this.positionLocalToVRRig = Vector3.Lerp(this.syncedState.position, this.positionLocalToVRRig, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			this.rotationLocalToVRRig = Quaternion.Slerp(this.syncedState.rotation, this.rotationLocalToVRRig, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
			base.transform.position = this.cachedRig.transform.TransformPoint(this.positionLocalToVRRig);
			base.transform.rotation = this.cachedRig.transform.rotation * this.rotationLocalToVRRig;
		}
		bool flag2 = false;
		if (GorillaGameManager.instance as GorillaTagManager != null)
		{
			flag2 = this.syncedState.tagged;
		}
		bool flag3 = flag2 != this.infectedState;
		this.infectedState = flag2;
		if (flag3 || flag)
		{
			if (this.infectedState)
			{
				this.leafMesh.material = this.GetInfectedMaterial();
			}
			else
			{
				this.leafMesh.material = this.GetMaterialFromIndex(this.syncedState.materialIndex);
			}
		}
		float num2 = Mathf.Clamp01((float)this.syncedState.audioLevel / 255f);
		if (this.audioLevel != num2)
		{
			this.audioLevel = num2;
			if (this.syncedState.riderId != -1 && this.syncedState.tagged)
			{
				this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				this.UpdateAudioSource(this.whistlingAudio, this.audioLevel * this.infectedAudioVolumeMultiplier);
				return;
			}
			this.UpdateAudioSource(this.calmAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.activeAudio, this.audioLevel * this.audioVolumeMultiplier);
			this.UpdateAudioSource(this.whistlingAudio, 0f);
		}
	}

	// Token: 0x060051B0 RID: 20912 RVA: 0x001B2358 File Offset: 0x001B0558
	private VRRig getNewHolderRig(int riderId)
	{
		if (riderId >= 0)
		{
			NetPlayer netPlayer;
			if (riderId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				netPlayer = NetworkSystem.Instance.LocalPlayer;
			}
			else
			{
				netPlayer = NetworkSystem.Instance.GetPlayer(riderId);
			}
			RigContainer rigContainer;
			if (netPlayer != null && VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
			{
				return rigContainer.Rig;
			}
		}
		return null;
	}

	// Token: 0x060051B1 RID: 20913 RVA: 0x001B23B0 File Offset: 0x001B05B0
	private Vector3 ClosestPointInHandle(Vector3 startingPoint, InteractionPoint interactionPoint)
	{
		CapsuleCollider component = interactionPoint.GetComponent<CapsuleCollider>();
		Vector3 vector = startingPoint;
		if (component != null)
		{
			Vector3 vector2 = ((component.direction == 0) ? Vector3.right : ((component.direction == 1) ? Vector3.up : Vector3.forward));
			Vector3 vector3 = component.transform.rotation * vector2;
			Vector3 vector4 = component.transform.position + component.transform.rotation * component.center;
			float num = Mathf.Clamp(Vector3.Dot(vector - vector4, vector3), -component.height * 0.5f, component.height * 0.5f);
			vector = vector4 + vector3 * num;
		}
		return vector;
	}

	// Token: 0x060051B2 RID: 20914 RVA: 0x001B2470 File Offset: 0x001B0670
	private void UpdateGliderPosition()
	{
		if (this.leftHold.active && this.rightHold.active)
		{
			Vector3 vector = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + base.transform.TransformVector(this.leftHold.handleLocalPos);
			Vector3 vector2 = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos) + base.transform.TransformVector(this.rightHold.handleLocalPos);
			base.transform.position = (vector + vector2) * 0.5f;
			return;
		}
		if (this.leftHold.active)
		{
			base.transform.position = this.leftHold.transform.TransformPoint(this.leftHold.holdLocalPos) + base.transform.TransformVector(this.leftHold.handleLocalPos);
			return;
		}
		if (this.rightHold.active)
		{
			base.transform.position = this.rightHold.transform.TransformPoint(this.rightHold.holdLocalPos) + base.transform.TransformVector(this.rightHold.handleLocalPos);
		}
	}

	// Token: 0x060051B3 RID: 20915 RVA: 0x001B25C8 File Offset: 0x001B07C8
	private Vector3 GetHandsVector(Vector3 leftHandPos, Vector3 rightHandPos, Vector3 headPos, bool flipBasedOnFacingDir)
	{
		Vector3 vector = rightHandPos - leftHandPos;
		Vector3 vector2 = (rightHandPos + leftHandPos) * 0.5f - headPos;
		Vector3 normalized = Vector3.Cross(Vector3.up, vector2).normalized;
		if (flipBasedOnFacingDir && Vector3.Dot(vector, normalized) < 0f)
		{
			vector = -vector;
		}
		return vector;
	}

	// Token: 0x060051B4 RID: 20916 RVA: 0x001B2624 File Offset: 0x001B0824
	private void GetHandsOrientationVectors(Vector3 leftHandPos, Vector3 rightHandPos, Transform head, bool flipBasedOnFacingDir, out Vector3 handsVector, out Vector3 handsUpVector)
	{
		handsVector = rightHandPos - leftHandPos;
		float magnitude = handsVector.magnitude;
		handsVector /= Mathf.Max(magnitude, 0.001f);
		Vector3 position = head.position;
		float num = 1f;
		Vector3 vector = ((Vector3.Dot(head.right, handsVector) < 0f) ? handsVector : (-handsVector));
		Vector3 normalized = Vector3.ProjectOnPlane(-head.forward, vector).normalized;
		Vector3 vector2 = normalized * num + position;
		Vector3 vector3 = (leftHandPos + rightHandPos) * 0.5f;
		Vector3 vector4 = Vector3.ProjectOnPlane(vector3 - head.position, Vector3.up);
		float magnitude2 = vector4.magnitude;
		vector4 /= Mathf.Max(magnitude2, 0.001f);
		Vector3 normalized2 = Vector3.ProjectOnPlane(-base.transform.forward, Vector3.up).normalized;
		Vector3 vector5 = -vector4 * num + position;
		float num2 = Vector3.Dot(normalized2, -vector4);
		float num3 = Vector3.Dot(normalized2, normalized);
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0f)
		{
			num2 = Mathf.Abs(num2);
			num3 = Mathf.Abs(num3);
		}
		num2 = Mathf.Max(num2, 0f);
		num3 = Mathf.Max(num3, 0f);
		Vector3 vector6 = (vector5 * num2 + vector2 * num3) / Mathf.Max(num2 + num3, 0.001f);
		Vector3 vector7 = vector3 - vector6;
		Vector3 normalized3 = Vector3.Cross(Vector3.up, vector7).normalized;
		if (flipBasedOnFacingDir && Vector3.Dot(handsVector, normalized3) < 0f)
		{
			handsVector = -handsVector;
		}
		handsUpVector = Vector3.Cross(Vector3.ProjectOnPlane(vector7, Vector3.up), handsVector).normalized;
	}

	// Token: 0x060051B5 RID: 20917 RVA: 0x001B284F File Offset: 0x001B0A4F
	private Material GetMaterialFromIndex(byte materialIndex)
	{
		if (materialIndex < 1 || (int)materialIndex > this.cosmeticMaterialOverrides.Length)
		{
			return this.baseLeafMaterial;
		}
		return this.cosmeticMaterialOverrides[(int)(materialIndex - 1)].material;
	}

	// Token: 0x060051B6 RID: 20918 RVA: 0x001B287C File Offset: 0x001B0A7C
	private float GetRollAngle180Wrapping()
	{
		Vector3 normalized = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
		float num = Vector3.SignedAngle(Vector3.Cross(Vector3.up, normalized).normalized, base.transform.right, base.transform.forward);
		return this.NormalizeAngle180(num);
	}

	// Token: 0x060051B7 RID: 20919 RVA: 0x001B28DD File Offset: 0x001B0ADD
	private float SignedAngleInPlane(Vector3 from, Vector3 to, Vector3 normal)
	{
		from = Vector3.ProjectOnPlane(from, normal);
		to = Vector3.ProjectOnPlane(to, normal);
		return Vector3.SignedAngle(from, to, normal);
	}

	// Token: 0x060051B8 RID: 20920 RVA: 0x001B28F9 File Offset: 0x001B0AF9
	private float NormalizeAngle180(float angle)
	{
		angle = (angle + 180f) % 360f;
		if (angle < 0f)
		{
			angle += 360f;
		}
		return angle - 180f;
	}

	// Token: 0x060051B9 RID: 20921 RVA: 0x001B2924 File Offset: 0x001B0B24
	private void UpdateAudioSource(AudioSource source, float level)
	{
		source.volume = level;
		if (!source.isPlaying && level > 0.01f)
		{
			source.GTPlay();
			return;
		}
		if (source.isPlaying && level < 0.01f && this.syncedState.riderId == -1)
		{
			source.GTStop();
		}
	}

	// Token: 0x060051BA RID: 20922 RVA: 0x001B2973 File Offset: 0x001B0B73
	private Material GetInfectedMaterial()
	{
		if (GorillaGameManager.instance is GorillaFreezeTagManager)
		{
			return this.frozenLeafMaterial;
		}
		return this.infectedLeafMaterial;
	}

	// Token: 0x060051BB RID: 20923 RVA: 0x001B2990 File Offset: 0x001B0B90
	public void OnTriggerStay(Collider other)
	{
		GliderWindVolume component = other.GetComponent<GliderWindVolume>();
		if (component == null)
		{
			return;
		}
		if (!base.IsMine && NetworkSystem.Instance.InRoom && !this.pendingOwnershipRequest)
		{
			return;
		}
		if (Time.frameCount == this.windVolumeForceAppliedFrame)
		{
			return;
		}
		if (this.leftHold.active || this.rightHold.active)
		{
			Vector3 accelFromVelocity = component.GetAccelFromVelocity(GTPlayer.Instance.RigidbodyVelocity);
			GTPlayer.Instance.AddForce(accelFromVelocity, ForceMode.Acceleration);
			this.windVolumeForceAppliedFrame = Time.frameCount;
			return;
		}
		Vector3 accelFromVelocity2 = component.GetAccelFromVelocity(this.rb.linearVelocity);
		Vector3 vector = this.WindResistanceForceOffset(base.transform.up, component.WindDirection);
		Vector3 vector2 = base.transform.position + vector * this.windUprightTorqueMultiplier;
		this.rb.AddForceAtPosition(accelFromVelocity2 * this.rb.mass, vector2, ForceMode.Force);
		this.windVolumeForceAppliedFrame = Time.frameCount;
	}

	// Token: 0x060051BC RID: 20924 RVA: 0x001B2A8E File Offset: 0x001B0C8E
	private Vector3 WindResistanceForceOffset(Vector3 upDir, Vector3 windDir)
	{
		if (Vector3.Dot(upDir, windDir) < 0f)
		{
			upDir *= -1f;
		}
		return Vector3.ProjectOnPlane(upDir - windDir, upDir);
	}

	// Token: 0x170007BF RID: 1983
	// (get) Token: 0x060051BD RID: 20925 RVA: 0x001B2AB8 File Offset: 0x001B0CB8
	// (set) Token: 0x060051BE RID: 20926 RVA: 0x001B2AE2 File Offset: 0x001B0CE2
	[Networked]
	[NetworkedWeaved(0, 11)]
	internal unsafe GliderHoldable.SyncedState Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GliderHoldable.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(GliderHoldable.SyncedState*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GliderHoldable.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(GliderHoldable.SyncedState*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060051BF RID: 20927 RVA: 0x001B2B10 File Offset: 0x001B0D10
	public override void ReadDataFusion()
	{
		int num = this.syncedState.riderId;
		this.syncedState = this.Data;
		if (num != this.syncedState.riderId)
		{
			this.positionLocalToVRRig = this.syncedState.position;
			this.rotationLocalToVRRig = this.syncedState.rotation;
		}
	}

	// Token: 0x060051C0 RID: 20928 RVA: 0x001B2B63 File Offset: 0x001B0D63
	public override void WriteDataFusion()
	{
		this.Data = this.syncedState;
	}

	// Token: 0x060051C1 RID: 20929 RVA: 0x001B2B74 File Offset: 0x001B0D74
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		Player sender = info.Sender;
		PunNetPlayer punNetPlayer = (PunNetPlayer)this.ownershipGuard.actualOwner;
		if (sender != ((punNetPlayer != null) ? punNetPlayer.PlayerRef : null))
		{
			return;
		}
		int num = this.syncedState.riderId;
		this.syncedState.riderId = (int)stream.ReceiveNext();
		this.syncedState.tagged = (bool)stream.ReceiveNext();
		this.syncedState.materialIndex = (byte)stream.ReceiveNext();
		this.syncedState.audioLevel = (byte)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		(ref this.syncedState.position).SetValueSafe(in vector);
		Quaternion quaternion = (Quaternion)stream.ReceiveNext();
		(ref this.syncedState.rotation).SetValueSafe(in quaternion);
		if (num != this.syncedState.riderId)
		{
			this.positionLocalToVRRig = this.syncedState.position;
			this.rotationLocalToVRRig = this.syncedState.rotation;
		}
	}

	// Token: 0x060051C2 RID: 20930 RVA: 0x001B2C7C File Offset: 0x001B0E7C
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		object sender = info.Sender;
		NetPlayer actualOwner = this.ownershipGuard.actualOwner;
		if (!sender.Equals((actualOwner != null) ? actualOwner.GetPlayerRef() : null))
		{
			return;
		}
		stream.SendNext(this.syncedState.riderId);
		stream.SendNext(this.syncedState.tagged);
		stream.SendNext(this.syncedState.materialIndex);
		stream.SendNext(this.syncedState.audioLevel);
		stream.SendNext(this.syncedState.position);
		stream.SendNext(this.syncedState.rotation);
	}

	// Token: 0x060051C3 RID: 20931 RVA: 0x001B2D37 File Offset: 0x001B0F37
	private IEnumerator ReenableOwnershipRequest()
	{
		yield return new WaitForSeconds(3f);
		this.pendingOwnershipRequest = false;
		yield break;
	}

	// Token: 0x060051C4 RID: 20932 RVA: 0x001B2D48 File Offset: 0x001B0F48
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer == NetworkSystem.Instance.LocalPlayer)
		{
			this.pendingOwnershipRequest = false;
			if (!this.leftHold.active && !this.rightHold.active && (this.spawnPosition - base.transform.position).sqrMagnitude > 1f)
			{
				this.rb.isKinematic = false;
				this.rb.WakeUp();
				this.lastHeldTime = Time.time;
			}
		}
	}

	// Token: 0x060051C5 RID: 20933 RVA: 0x001B2DCA File Offset: 0x001B0FCA
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return !base.IsMine || !NetworkSystem.Instance.InRoom || (!this.leftHold.active && !this.rightHold.active);
	}

	// Token: 0x060051C6 RID: 20934 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x060051C7 RID: 20935 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x060051C8 RID: 20936 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x060051CC RID: 20940 RVA: 0x001B32B9 File Offset: 0x001B14B9
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060051CD RID: 20941 RVA: 0x001B32D1 File Offset: 0x001B14D1
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x040063A5 RID: 25509
	[Header("Flight Settings")]
	[SerializeField]
	private Vector2 pitchMinMax = new Vector2(-80f, 80f);

	// Token: 0x040063A6 RID: 25510
	[SerializeField]
	private Vector2 rollMinMax = new Vector2(-70f, 70f);

	// Token: 0x040063A7 RID: 25511
	[SerializeField]
	private float pitchHalfLife = 0.2f;

	// Token: 0x040063A8 RID: 25512
	public Vector2 pitchVelocityTargetMinMax = new Vector2(-60f, 60f);

	// Token: 0x040063A9 RID: 25513
	public Vector2 pitchVelocityRampTimeMinMax = new Vector2(-1f, 1f);

	// Token: 0x040063AA RID: 25514
	[SerializeField]
	private float pitchVelocityFollowRateAngle = 60f;

	// Token: 0x040063AB RID: 25515
	[SerializeField]
	private float pitchVelocityFollowRateMagnitude = 5f;

	// Token: 0x040063AC RID: 25516
	[SerializeField]
	private AnimationCurve liftVsAttack = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040063AD RID: 25517
	[SerializeField]
	private AnimationCurve dragVsAttack = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040063AE RID: 25518
	[SerializeField]
	[Range(0f, 1f)]
	public float attackDragFactor = 0.1f;

	// Token: 0x040063AF RID: 25519
	[SerializeField]
	private AnimationCurve dragVsSpeed = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040063B0 RID: 25520
	[SerializeField]
	public float dragVsSpeedMaxSpeed = 30f;

	// Token: 0x040063B1 RID: 25521
	[SerializeField]
	[Range(0f, 1f)]
	public float dragVsSpeedDragFactor = 0.2f;

	// Token: 0x040063B2 RID: 25522
	[SerializeField]
	private AnimationCurve liftIncreaseVsRoll = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040063B3 RID: 25523
	[SerializeField]
	private float liftIncreaseVsRollMaxAngle = 20f;

	// Token: 0x040063B4 RID: 25524
	[SerializeField]
	[Range(0f, 1f)]
	private float gravityCompensation = 0.8f;

	// Token: 0x040063B5 RID: 25525
	[Range(0f, 1f)]
	public float pullUpLiftBonus = 0.1f;

	// Token: 0x040063B6 RID: 25526
	public float pullUpLiftActivationVelocity = 1f;

	// Token: 0x040063B7 RID: 25527
	public float pullUpLiftActivationAcceleration = 3f;

	// Token: 0x040063B8 RID: 25528
	[Header("Body Positioning Control")]
	[SerializeField]
	private float riderPosDirectPitchMax = 70f;

	// Token: 0x040063B9 RID: 25529
	[SerializeField]
	private Vector2 riderPosRange = new Vector2(2.2f, 0.75f);

	// Token: 0x040063BA RID: 25530
	[SerializeField]
	private float riderPosRangeOffset = 0.15f;

	// Token: 0x040063BB RID: 25531
	[SerializeField]
	private Vector2 riderPosRangeNormalizedDeadzone = new Vector2(0.15f, 0.05f);

	// Token: 0x040063BC RID: 25532
	[Header("Direct Handle Control")]
	[SerializeField]
	private float oneHandHoldRotationRate = 2f;

	// Token: 0x040063BD RID: 25533
	private Vector3 oneHandSimulatedHoldOffset = new Vector3(0.5f, -0.35f, 0.25f);

	// Token: 0x040063BE RID: 25534
	private float oneHandPitchMultiplier = 0.8f;

	// Token: 0x040063BF RID: 25535
	[SerializeField]
	private float twoHandHoldRotationRate = 4f;

	// Token: 0x040063C0 RID: 25536
	[SerializeField]
	private bool twoHandGliderInversionOnYawInsteadOfRoll;

	// Token: 0x040063C1 RID: 25537
	[Header("Player Settings")]
	[SerializeField]
	private bool setMaxHandSlipDuringFlight = true;

	// Token: 0x040063C2 RID: 25538
	[SerializeField]
	private float maxSlipOverrideSpeedThreshold = 5f;

	// Token: 0x040063C3 RID: 25539
	[Header("Player Camera Rotation")]
	[SerializeField]
	private float subtlePlayerPitchFactor = 0.2f;

	// Token: 0x040063C4 RID: 25540
	[SerializeField]
	private float subtlePlayerPitchRate = 2f;

	// Token: 0x040063C5 RID: 25541
	[SerializeField]
	private float subtlePlayerRollFactor = 0.2f;

	// Token: 0x040063C6 RID: 25542
	[SerializeField]
	private float subtlePlayerRollRate = 2f;

	// Token: 0x040063C7 RID: 25543
	[SerializeField]
	private Vector2 subtlePlayerRotationSpeedRampMinMax = new Vector2(2f, 8f);

	// Token: 0x040063C8 RID: 25544
	[SerializeField]
	private Vector2 subtlePlayerRollAccelMinMax = new Vector2(0f, 30f);

	// Token: 0x040063C9 RID: 25545
	[SerializeField]
	private Vector2 subtlePlayerPitchAccelMinMax = new Vector2(0f, 10f);

	// Token: 0x040063CA RID: 25546
	[SerializeField]
	private float accelSmoothingFollowRate = 2f;

	// Token: 0x040063CB RID: 25547
	[Header("Haptics")]
	[SerializeField]
	private Vector2 hapticAccelInputRange = new Vector2(5f, 20f);

	// Token: 0x040063CC RID: 25548
	[SerializeField]
	private float hapticAccelOutputMax = 0.35f;

	// Token: 0x040063CD RID: 25549
	[SerializeField]
	private Vector2 hapticMaxSpeedInputRange = new Vector2(5f, 10f);

	// Token: 0x040063CE RID: 25550
	[SerializeField]
	private Vector2 hapticSpeedInputRange = new Vector2(3f, 30f);

	// Token: 0x040063CF RID: 25551
	[SerializeField]
	private float hapticSpeedOutputMax = 0.15f;

	// Token: 0x040063D0 RID: 25552
	[SerializeField]
	private Vector2 whistlingAudioSpeedInputRange = new Vector2(15f, 30f);

	// Token: 0x040063D1 RID: 25553
	[Header("Audio")]
	[SerializeField]
	private float audioVolumeMultiplier = 0.25f;

	// Token: 0x040063D2 RID: 25554
	[SerializeField]
	private float infectedAudioVolumeMultiplier = 0.5f;

	// Token: 0x040063D3 RID: 25555
	[SerializeField]
	private Vector2 whooshSpeedThresholdInput = new Vector2(10f, 25f);

	// Token: 0x040063D4 RID: 25556
	[SerializeField]
	private Vector2 whooshVolumeOutput = new Vector2(0.2f, 0.75f);

	// Token: 0x040063D5 RID: 25557
	[SerializeField]
	private float whooshCheckDistance = 2f;

	// Token: 0x040063D6 RID: 25558
	[Header("Tag Adjustment")]
	[SerializeField]
	private bool extendTagRangeInFlight = true;

	// Token: 0x040063D7 RID: 25559
	[SerializeField]
	private Vector2 tagRangeSpeedInput = new Vector2(5f, 20f);

	// Token: 0x040063D8 RID: 25560
	[SerializeField]
	private Vector2 tagRangeOutput = new Vector2(0.03f, 3f);

	// Token: 0x040063D9 RID: 25561
	[SerializeField]
	private bool debugDrawTagRange = true;

	// Token: 0x040063DA RID: 25562
	[Header("Infected State")]
	[SerializeField]
	private float infectedSpeedIncrease = 5f;

	// Token: 0x040063DB RID: 25563
	[Header("Glider Materials")]
	[SerializeField]
	private MeshRenderer leafMesh;

	// Token: 0x040063DC RID: 25564
	[SerializeField]
	private Material baseLeafMaterial;

	// Token: 0x040063DD RID: 25565
	[SerializeField]
	private Material infectedLeafMaterial;

	// Token: 0x040063DE RID: 25566
	[SerializeField]
	private Material frozenLeafMaterial;

	// Token: 0x040063DF RID: 25567
	[SerializeField]
	private GliderHoldable.CosmeticMaterialOverride[] cosmeticMaterialOverrides;

	// Token: 0x040063E0 RID: 25568
	[Header("Network Syncing")]
	[SerializeField]
	private float networkSyncFollowRate = 2f;

	// Token: 0x040063E1 RID: 25569
	[Header("Life Cycle")]
	[SerializeField]
	private Transform maxDistanceRespawnOrigin;

	// Token: 0x040063E2 RID: 25570
	[SerializeField]
	private float maxDistanceBeforeRespawn = 180f;

	// Token: 0x040063E3 RID: 25571
	[SerializeField]
	private float maxDroppedTimeToRespawn = 120f;

	// Token: 0x040063E4 RID: 25572
	[Header("Rigidbody")]
	[SerializeField]
	private float windUprightTorqueMultiplier = 1f;

	// Token: 0x040063E5 RID: 25573
	[SerializeField]
	private float gravityUprightTorqueMultiplier = 0.5f;

	// Token: 0x040063E6 RID: 25574
	[SerializeField]
	private float fallingGravityReduction = 0.1f;

	// Token: 0x040063E7 RID: 25575
	[Header("References")]
	[SerializeField]
	private AudioSource calmAudio;

	// Token: 0x040063E8 RID: 25576
	[SerializeField]
	private AudioSource activeAudio;

	// Token: 0x040063E9 RID: 25577
	[SerializeField]
	private AudioSource whistlingAudio;

	// Token: 0x040063EA RID: 25578
	[SerializeField]
	private AudioSource leftWhooshAudio;

	// Token: 0x040063EB RID: 25579
	[SerializeField]
	private AudioSource rightWhooshAudio;

	// Token: 0x040063EC RID: 25580
	[SerializeField]
	private InteractionPoint handle;

	// Token: 0x040063ED RID: 25581
	[SerializeField]
	private RequestableOwnershipGuard ownershipGuard;

	// Token: 0x040063EE RID: 25582
	private bool subtlePlayerPitchActive = true;

	// Token: 0x040063EF RID: 25583
	private bool subtlePlayerRollActive = true;

	// Token: 0x040063F0 RID: 25584
	private float subtlePlayerPitch;

	// Token: 0x040063F1 RID: 25585
	private float subtlePlayerRoll;

	// Token: 0x040063F2 RID: 25586
	private float subtlePlayerPitchRateExp = 0.75f;

	// Token: 0x040063F3 RID: 25587
	private float subtlePlayerRollRateExp = 0.025f;

	// Token: 0x040063F4 RID: 25588
	private float defaultMaxDistanceBeforeRespawn = 180f;

	// Token: 0x040063F5 RID: 25589
	private GliderHoldable.HoldingHand leftHold = new GliderHoldable.HoldingHand();

	// Token: 0x040063F6 RID: 25590
	private GliderHoldable.HoldingHand rightHold = new GliderHoldable.HoldingHand();

	// Token: 0x040063F7 RID: 25591
	private GliderHoldable.SyncedState syncedState;

	// Token: 0x040063F8 RID: 25592
	private Vector3 twoHandRotationOffsetAxis = Vector3.forward;

	// Token: 0x040063F9 RID: 25593
	private float twoHandRotationOffsetAngle;

	// Token: 0x040063FA RID: 25594
	private Rigidbody rb;

	// Token: 0x040063FB RID: 25595
	private Vector2 riderPosition = Vector2.zero;

	// Token: 0x040063FC RID: 25596
	private Vector3 previousVelocity;

	// Token: 0x040063FD RID: 25597
	private Vector3 currentVelocity;

	// Token: 0x040063FE RID: 25598
	private float pitch;

	// Token: 0x040063FF RID: 25599
	private float yaw;

	// Token: 0x04006400 RID: 25600
	private float roll;

	// Token: 0x04006401 RID: 25601
	private float pitchVel;

	// Token: 0x04006402 RID: 25602
	private float yawVel;

	// Token: 0x04006403 RID: 25603
	private float rollVel;

	// Token: 0x04006404 RID: 25604
	private float oneHandRotationRateExp;

	// Token: 0x04006405 RID: 25605
	private float twoHandRotationRateExp;

	// Token: 0x04006406 RID: 25606
	private Quaternion playerFacingRotationOffset = Quaternion.identity;

	// Token: 0x04006407 RID: 25607
	private const float accelAveragingWindow = 0.1f;

	// Token: 0x04006408 RID: 25608
	private AverageVector3 accelerationAverage = new AverageVector3(0.1f);

	// Token: 0x04006409 RID: 25609
	private float accelerationSmoothed;

	// Token: 0x0400640A RID: 25610
	private float turnAccelerationSmoothed;

	// Token: 0x0400640B RID: 25611
	private float accelSmoothingFollowRateExp = 1f;

	// Token: 0x0400640C RID: 25612
	private float networkSyncFollowRateExp = 2f;

	// Token: 0x0400640D RID: 25613
	private bool pendingOwnershipRequest;

	// Token: 0x0400640E RID: 25614
	private Vector3 positionLocalToVRRig = Vector3.zero;

	// Token: 0x0400640F RID: 25615
	private Quaternion rotationLocalToVRRig = Quaternion.identity;

	// Token: 0x04006410 RID: 25616
	private Coroutine reenableOwnershipRequestCoroutine;

	// Token: 0x04006411 RID: 25617
	private Vector3 spawnPosition;

	// Token: 0x04006412 RID: 25618
	private Quaternion spawnRotation;

	// Token: 0x04006413 RID: 25619
	private Vector3 skyJungleSpawnPostion;

	// Token: 0x04006414 RID: 25620
	private Quaternion skyJungleSpawnRotation;

	// Token: 0x04006415 RID: 25621
	private Transform skyJungleRespawnOrigin;

	// Token: 0x04006416 RID: 25622
	private float lastHeldTime = -1f;

	// Token: 0x04006417 RID: 25623
	private Vector3? leftHoldPositionLocal;

	// Token: 0x04006418 RID: 25624
	private Vector3? rightHoldPositionLocal;

	// Token: 0x04006419 RID: 25625
	private float whooshSoundDuration = 1f;

	// Token: 0x0400641A RID: 25626
	private float whooshSoundRetriggerThreshold = 0.5f;

	// Token: 0x0400641B RID: 25627
	private float leftWhooshStartTime = -1f;

	// Token: 0x0400641C RID: 25628
	private Vector3 leftWhooshHitPoint = Vector3.zero;

	// Token: 0x0400641D RID: 25629
	private Vector3 whooshAudioPositionOffset = new Vector3(0.5f, -0.25f, 0.5f);

	// Token: 0x0400641E RID: 25630
	private float rightWhooshStartTime = -1f;

	// Token: 0x0400641F RID: 25631
	private Vector3 rightWhooshHitPoint = Vector3.zero;

	// Token: 0x04006420 RID: 25632
	private int ridersMaterialOverideIndex;

	// Token: 0x04006421 RID: 25633
	private int windVolumeForceAppliedFrame = -1;

	// Token: 0x04006422 RID: 25634
	private bool holdingTwoGliders;

	// Token: 0x04006423 RID: 25635
	private GliderHoldable.GliderState gliderState;

	// Token: 0x04006424 RID: 25636
	private float audioLevel;

	// Token: 0x04006425 RID: 25637
	private int riderId = -1;

	// Token: 0x04006426 RID: 25638
	[SerializeField]
	private VRRig cachedRig;

	// Token: 0x04006427 RID: 25639
	private bool infectedState;

	// Token: 0x04006428 RID: 25640
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 11)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private GliderHoldable.SyncedState _Data;

	// Token: 0x02000CE4 RID: 3300
	private enum GliderState
	{
		// Token: 0x0400642A RID: 25642
		LocallyHeld,
		// Token: 0x0400642B RID: 25643
		LocallyDropped,
		// Token: 0x0400642C RID: 25644
		RemoteSyncing
	}

	// Token: 0x02000CE5 RID: 3301
	private class HoldingHand
	{
		// Token: 0x060051CE RID: 20942 RVA: 0x001B32E8 File Offset: 0x001B14E8
		public void Activate(Transform handTransform, Transform gliderTransform, Vector3 worldGrabPoint)
		{
			this.active = true;
			this.transform = handTransform.transform;
			this.holdLocalPos = handTransform.InverseTransformPoint(worldGrabPoint);
			this.handleLocalPos = gliderTransform.InverseTransformVector(gliderTransform.position - worldGrabPoint);
			this.localHoldRotation = Quaternion.Inverse(handTransform.rotation) * gliderTransform.rotation;
		}

		// Token: 0x060051CF RID: 20943 RVA: 0x001B3349 File Offset: 0x001B1549
		public void Deactivate()
		{
			this.active = false;
			this.transform = null;
			this.holdLocalPos = Vector3.zero;
			this.handleLocalPos = Vector3.zero;
			this.localHoldRotation = Quaternion.identity;
		}

		// Token: 0x0400642D RID: 25645
		public bool active;

		// Token: 0x0400642E RID: 25646
		public Transform transform;

		// Token: 0x0400642F RID: 25647
		public Vector3 holdLocalPos;

		// Token: 0x04006430 RID: 25648
		public Vector3 handleLocalPos;

		// Token: 0x04006431 RID: 25649
		public Quaternion localHoldRotation;
	}

	// Token: 0x02000CE6 RID: 3302
	[NetworkStructWeaved(11)]
	[StructLayout(LayoutKind.Explicit, Size = 44)]
	internal struct SyncedState : INetworkStruct
	{
		// Token: 0x060051D1 RID: 20945 RVA: 0x001B337A File Offset: 0x001B157A
		public void Init(Vector3 defaultPosition, Quaternion defaultRotation)
		{
			this.riderId = -1;
			this.materialIndex = 0;
			this.audioLevel = 0;
			this.position = defaultPosition;
			this.rotation = defaultRotation;
		}

		// Token: 0x060051D2 RID: 20946 RVA: 0x001B339F File Offset: 0x001B159F
		public SyncedState(int id = -1)
		{
			this.riderId = id;
			this.materialIndex = 0;
			this.audioLevel = 0;
			this.tagged = default(NetworkBool);
			this.position = default(Vector3);
			this.rotation = default(Quaternion);
		}

		// Token: 0x04006432 RID: 25650
		[FieldOffset(0)]
		public int riderId;

		// Token: 0x04006433 RID: 25651
		[FieldOffset(4)]
		public byte materialIndex;

		// Token: 0x04006434 RID: 25652
		[FieldOffset(8)]
		public byte audioLevel;

		// Token: 0x04006435 RID: 25653
		[FieldOffset(12)]
		public NetworkBool tagged;

		// Token: 0x04006436 RID: 25654
		[FieldOffset(16)]
		public Vector3 position;

		// Token: 0x04006437 RID: 25655
		[FieldOffset(28)]
		public Quaternion rotation;
	}

	// Token: 0x02000CE7 RID: 3303
	[Serializable]
	private struct CosmeticMaterialOverride
	{
		// Token: 0x04006438 RID: 25656
		public string cosmeticName;

		// Token: 0x04006439 RID: 25657
		public Material material;
	}
}
