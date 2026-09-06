using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000A00 RID: 2560
public class TakeMyHand_HandLink : HoldableObject, IGorillaSliceableSimple
{
	// Token: 0x17000630 RID: 1584
	// (get) Token: 0x060041A5 RID: 16805 RVA: 0x0015D9DF File Offset: 0x0015BBDF
	// (set) Token: 0x060041A6 RID: 16806 RVA: 0x0015D9E7 File Offset: 0x0015BBE7
	public bool IsTentacleGrab { get; private set; }

	// Token: 0x17000631 RID: 1585
	// (get) Token: 0x060041A7 RID: 16807 RVA: 0x0015D9F0 File Offset: 0x0015BBF0
	// (set) Token: 0x060041A8 RID: 16808 RVA: 0x0015D9F8 File Offset: 0x0015BBF8
	public bool IsLocal { get; private set; }

	// Token: 0x060041A9 RID: 16809 RVA: 0x0015DA04 File Offset: 0x0015BC04
	private void Start()
	{
		this.myOtherHandLink = (this.isLeftHand ? this.myRig.rightHandLink : this.myRig.leftHandLink);
		if (this.myRig.isOfflineVRRig)
		{
			base.gameObject.SetActive(false);
			this.IsLocal = true;
		}
		if (this.interactionPoint == null)
		{
			this.interactionPoint = base.GetComponent<InteractionPoint>();
		}
	}

	// Token: 0x060041AA RID: 16810 RVA: 0x0001212B File Offset: 0x0001032B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060041AB RID: 16811 RVA: 0x00012134 File Offset: 0x00010334
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060041AC RID: 16812 RVA: 0x0015DA74 File Offset: 0x0015BC74
	public void SliceUpdate()
	{
		this.interactionPoint.enabled = this.isReadyForGrabbing && (this.myRig.transform.position - VRRig.LocalRig.transform.position).sqrMagnitude < 9f;
	}

	// Token: 0x060041AD RID: 16813 RVA: 0x0015DACC File Offset: 0x0015BCCC
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.CanBeGrabbed())
		{
			return;
		}
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
		{
			(this.isLeftHand ? this.myRig.leftHolds : this.myRig.rightHolds).OnGrab(pointGrabbed, grabbingHand);
			return;
		}
		TakeMyHand_HandLink takeMyHand_HandLink = ((grabbingHand == EquipmentInteractor.instance.leftHand) ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink);
		if (takeMyHand_HandLink.isReadyForGrabbing && Time.time - takeMyHand_HandLink.gripPressedAtTimestamp < 0.1f)
		{
			takeMyHand_HandLink.LocalCreateLink(this);
		}
	}

	// Token: 0x060041AE RID: 16814 RVA: 0x0015DB78 File Offset: 0x0015BD78
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.myRig.isOfflineVRRig)
		{
			TakeMyHand_HandLink takeMyHand_HandLink = ((releasingHand == EquipmentInteractor.instance.leftHand) ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink);
			bool flag = false;
			HandLinkAuthorityStatus handLinkAuthorityStatus = GTPlayer.Instance.TakeMyHand_GetSelfHandLinkAuthority();
			int num;
			HandLinkAuthorityStatus chainAuthority = takeMyHand_HandLink.GetChainAuthority(out num);
			if (handLinkAuthorityStatus.type >= HandLinkAuthorityType.ButtGrounded && chainAuthority.type < handLinkAuthorityStatus.type)
			{
				flag = true;
			}
			else if (takeMyHand_HandLink.myOtherHandLink.grabbedLink != null)
			{
				int num2;
				HandLinkAuthorityStatus chainAuthority2 = takeMyHand_HandLink.myOtherHandLink.GetChainAuthority(out num2);
				if (chainAuthority2.type >= HandLinkAuthorityType.ButtGrounded && chainAuthority.type < chainAuthority2.type)
				{
					flag = true;
				}
			}
			if (flag)
			{
				Vector3 averageVelocity = GTPlayer.Instance.GetHandVelocityTracker(takeMyHand_HandLink.isLeftHand).GetAverageVelocity(true, 0.15f, false);
				this.myRig.netView.SendRPC("DroppedByPlayer", this.myRig.OwningNetPlayer, new object[] { averageVelocity });
				this.myRig.ApplyLocalTrajectoryOverride(averageVelocity);
			}
			takeMyHand_HandLink.BreakLink();
		}
		return true;
	}

	// Token: 0x060041AF RID: 16815 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x060041B0 RID: 16816 RVA: 0x0015DC9F File Offset: 0x0015BE9F
	public override void DropItemCleanup()
	{
		if (this.grabbedLink != null)
		{
			this.grabbedLink.BreakLink();
		}
	}

	// Token: 0x060041B1 RID: 16817 RVA: 0x0015DCBA File Offset: 0x0015BEBA
	public bool CanBeGrabbed()
	{
		return (!GorillaComputer.instance.IsPlayerInVirtualStump() || !CustomMapManager.WantsHoldingHandsDisabled()) && Time.time >= this.rejectGrabsUntilTimestamp && this.isReadyForGrabbing && this.grabbedPlayer == null;
	}

	// Token: 0x060041B2 RID: 16818 RVA: 0x0015DCF5 File Offset: 0x0015BEF5
	public bool IsLinkActive()
	{
		return this.grabbedLink != null;
	}

	// Token: 0x060041B3 RID: 16819 RVA: 0x0015DD04 File Offset: 0x0015BF04
	public bool TentacleTryCreateLink(TakeMyHand_HandLink remoteLink)
	{
		if (!this.myRig.isLocal || this.grabbedPlayer != null)
		{
			return false;
		}
		if (GorillaComputer.instance.IsPlayerInVirtualStump() && CustomMapManager.WantsHoldingHandsDisabled())
		{
			return false;
		}
		if (Time.time < this.rejectGrabsUntilTimestamp)
		{
			return false;
		}
		if (!remoteLink.CanBeGrabbed())
		{
			return false;
		}
		GRPlayer grplayer = GRPlayer.Get(remoteLink.myRig);
		GRPlayer grplayer2 = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer);
		if (grplayer2 != null && grplayer != null && grplayer2.State == GRPlayer.GRPlayerState.Ghost != (grplayer.State == GRPlayer.GRPlayerState.Ghost))
		{
			return false;
		}
		this.IsTentacleGrab = true;
		this.grabbedLink = remoteLink;
		this.grabbedLink.TentacleOffset = Vector3.zero;
		this.grabbedPlayer = remoteLink.myRig.OwningNetPlayer;
		this.grabbedHandIsLeft = remoteLink.isLeftHand;
		Action onHandLinkChanged = TakeMyHand_HandLink.OnHandLinkChanged;
		if (onHandLinkChanged != null)
		{
			onHandLinkChanged();
		}
		return true;
	}

	// Token: 0x17000632 RID: 1586
	// (get) Token: 0x060041B4 RID: 16820 RVA: 0x0015DDE8 File Offset: 0x0015BFE8
	// (set) Token: 0x060041B5 RID: 16821 RVA: 0x0015DDF0 File Offset: 0x0015BFF0
	public Vector3 TentacleOffset { get; set; }

	// Token: 0x17000633 RID: 1587
	// (get) Token: 0x060041B6 RID: 16822 RVA: 0x0015DDF9 File Offset: 0x0015BFF9
	public Vector3 LinkPosition
	{
		get
		{
			return base.transform.position + this.TentacleOffset;
		}
	}

	// Token: 0x060041B7 RID: 16823 RVA: 0x0015DE14 File Offset: 0x0015C014
	private void LocalCreateLink(TakeMyHand_HandLink remoteLink)
	{
		if (this.grabbedPlayer != null || !this.myRig.isLocal)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(remoteLink.myRig);
		GRPlayer grplayer2 = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer);
		if (grplayer2 != null && grplayer != null && grplayer2.State == GRPlayer.GRPlayerState.Ghost != (grplayer.State == GRPlayer.GRPlayerState.Ghost))
		{
			return;
		}
		EquipmentInteractor.instance.UpdateHandEquipment(remoteLink, this.isLeftHand);
		this.grabbedLink = remoteLink;
		this.grabbedPlayer = remoteLink.myRig.OwningNetPlayer;
		this.grabbedHandIsLeft = remoteLink.isLeftHand;
		this.TentacleOffset = Vector3.zero;
		if (remoteLink.IsTentacleGrab)
		{
			remoteLink.TentacleOffset = base.transform.position - remoteLink.transform.position;
		}
		else
		{
			remoteLink.TentacleOffset = Vector3.zero;
		}
		GorillaTagger.Instance.StartVibration(this.isLeftHand, this.hapticStrengthOnGrab, this.hapticDurationOnGrab);
		(this.isLeftHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer).GTPlayOneShot(this.audioOnGrab, 1f);
		Action onHandLinkChanged = TakeMyHand_HandLink.OnHandLinkChanged;
		if (onHandLinkChanged == null)
		{
			return;
		}
		onHandLinkChanged();
	}

	// Token: 0x060041B8 RID: 16824 RVA: 0x0015DF4B File Offset: 0x0015C14B
	public void BreakLinkTo(TakeMyHand_HandLink targetLink)
	{
		if (this.grabbedLink == targetLink)
		{
			this.BreakLink();
		}
	}

	// Token: 0x060041B9 RID: 16825 RVA: 0x0015DF64 File Offset: 0x0015C164
	public void BreakLink()
	{
		if (this.grabbedPlayer == null || this.grabbedLink == null)
		{
			return;
		}
		Vector3 vector = this.myRig.LatestVelocity();
		GTPlayer.Instance.SetVelocity(vector);
		this.IsTentacleGrab = false;
		this.TentacleOffset = Vector3.zero;
		this.grabbedLink = null;
		this.grabbedPlayer = null;
		this.grabbedHandIsLeft = false;
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isLeftHand);
		Action onHandLinkChanged = TakeMyHand_HandLink.OnHandLinkChanged;
		if (onHandLinkChanged == null)
		{
			return;
		}
		onHandLinkChanged();
	}

	// Token: 0x060041BA RID: 16826 RVA: 0x0015DFE8 File Offset: 0x0015C1E8
	public static bool IsHandInChainWithOtherPlayer(TakeMyHand_HandLink startingLink, int targetPlayer)
	{
		TakeMyHand_HandLink takeMyHand_HandLink = startingLink;
		int num = 0;
		int roomPlayerCount = NetworkSystem.Instance.RoomPlayerCount;
		while (takeMyHand_HandLink != null && num < roomPlayerCount)
		{
			if (takeMyHand_HandLink.myRig == null || takeMyHand_HandLink.myRig.creator == null)
			{
				return false;
			}
			if (takeMyHand_HandLink.myRig.creator.ActorNumber == targetPlayer)
			{
				return true;
			}
			TakeMyHand_HandLink takeMyHand_HandLink2 = null;
			RigContainer rigContainer;
			if (takeMyHand_HandLink.grabbedLink != null && takeMyHand_HandLink.grabbedLink.myOtherHandLink != null)
			{
				takeMyHand_HandLink2 = takeMyHand_HandLink.grabbedLink.myOtherHandLink;
			}
			else if (takeMyHand_HandLink.grabbedPlayer != null && VRRigCache.Instance.TryGetVrrig(takeMyHand_HandLink.grabbedPlayer, out rigContainer))
			{
				TakeMyHand_HandLink takeMyHand_HandLink3 = (takeMyHand_HandLink.grabbedHandIsLeft ? rigContainer.Rig.leftHandLink : rigContainer.Rig.rightHandLink);
				if (takeMyHand_HandLink3 != null && takeMyHand_HandLink3.myOtherHandLink != null)
				{
					takeMyHand_HandLink2 = takeMyHand_HandLink3.myOtherHandLink;
				}
			}
			takeMyHand_HandLink = takeMyHand_HandLink2;
			num++;
		}
		return false;
	}

	// Token: 0x060041BB RID: 16827 RVA: 0x0015E0E4 File Offset: 0x0015C2E4
	public void LocalUpdate(bool isGroundedHand, bool isGroundedButt, bool isGripPressed, bool isReadyForGrabbing)
	{
		if (isGripPressed && !this.wasGripPressed)
		{
			this.gripPressedAtTimestamp = Time.time;
		}
		this.wasGripPressed = isGripPressed;
		this.isReadyForGrabbing = isReadyForGrabbing && Time.time >= this.rejectGrabsUntilTimestamp;
		this.isGroundedHand = isGroundedHand;
		this.isGroundedButt = isGroundedButt;
		if (this.grabbedLink != null)
		{
			if (!this.grabbedLink.isReadyForGrabbing && this.grabbedLink.grabbedPlayer != NetworkSystem.Instance.LocalPlayer)
			{
				this.BreakLink();
				return;
			}
			if ((!this.IsTentacleGrab && !isGripPressed) || !this.grabbedLink.myRig.gameObject.activeSelf)
			{
				this.BreakLink();
				return;
			}
			GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
			if (gorillaGuardianManager != null && gorillaGuardianManager.IsPlayerGuardian(this.grabbedPlayer))
			{
				this.BreakLink();
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.grabbedLink.myRig);
			GRPlayer grplayer2 = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer);
			if (grplayer2 != null && grplayer != null && grplayer2.State == GRPlayer.GRPlayerState.Ghost != (grplayer.State == GRPlayer.GRPlayerState.Ghost))
			{
				this.BreakLink();
				return;
			}
			if (GorillaComputer.instance.IsPlayerInVirtualStump() && CustomMapManager.WantsHoldingHandsDisabled())
			{
				this.BreakLink();
				return;
			}
		}
	}

	// Token: 0x060041BC RID: 16828 RVA: 0x0015E22B File Offset: 0x0015C42B
	public void RejectGrabsFor(float duration)
	{
		this.rejectGrabsUntilTimestamp = Mathf.Max(this.rejectGrabsUntilTimestamp, Time.time + duration);
	}

	// Token: 0x060041BD RID: 16829 RVA: 0x0015E245 File Offset: 0x0015C445
	public void Write(out bool isGroundedHand, out bool isGroundedButt, out int grabbedPlayerActorNumber, out bool grabbedHandIsLeft)
	{
		isGroundedHand = this.isGroundedHand;
		isGroundedButt = this.isGroundedButt;
		if (this.grabbedPlayer != null)
		{
			grabbedPlayerActorNumber = this.grabbedPlayer.ActorNumber;
			grabbedHandIsLeft = this.grabbedHandIsLeft;
			return;
		}
		grabbedPlayerActorNumber = 0;
		grabbedHandIsLeft = false;
	}

	// Token: 0x060041BE RID: 16830 RVA: 0x0015E280 File Offset: 0x0015C480
	public void Read(Vector3 remoteHandLocalPos, Quaternion remoteBodyWorldRot, Vector3 remoteBodyWorldPos, bool isGroundedHand, bool isGroundedButt, bool isReadyForGrabbing, bool isTentacleGrab, int grabbedPlayerActorNumber, bool grabbedHandIsLeft)
	{
		this.isGroundedHand = isGroundedHand;
		this.isGroundedButt = isGroundedButt;
		this.isReadyForGrabbing = isReadyForGrabbing;
		if (grabbedPlayerActorNumber == 0)
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsLocal)
			{
				(this.grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).BreakLink();
			}
			bool flag = this.grabbedPlayer != null;
			this.grabbedPlayer = null;
			this.grabbedLink = null;
			if (flag)
			{
				Action onHandLinkChanged = TakeMyHand_HandLink.OnHandLinkChanged;
				if (onHandLinkChanged != null)
				{
					onHandLinkChanged();
				}
			}
		}
		else if (this.lastReadGrabbedPlayerActorNumber == grabbedPlayerActorNumber)
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsValid && this.grabbedPlayer.ActorNumber == grabbedPlayerActorNumber && this.grabbedPlayer.IsLocal && !this.IsLocalGrabInRange(grabbedHandIsLeft, remoteHandLocalPos, remoteBodyWorldRot, remoteBodyWorldPos, 7f))
			{
				if (this.grabbedHandIsLeft)
				{
					VRRig.LocalRig.leftHandLink.BreakLink();
				}
				else
				{
					VRRig.LocalRig.rightHandLink.BreakLink();
				}
			}
		}
		else
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsLocal)
			{
				VRRig.LocalRig.leftHandLink.BreakLinkTo(this);
				VRRig.LocalRig.rightHandLink.BreakLinkTo(this);
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(grabbedPlayerActorNumber);
			if (player != null)
			{
				bool flag2 = true;
				if (player.IsLocal && !isTentacleGrab && !(grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).IsTentacleGrab)
				{
					flag2 = this.IsLocalGrabInRange(grabbedHandIsLeft, remoteHandLocalPos, remoteBodyWorldRot, remoteBodyWorldPos, 0.25f);
				}
				if (!flag2)
				{
					(grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).RejectGrabsFor(0.5f);
					bool flag3 = this.grabbedPlayer != null;
					this.grabbedPlayer = null;
					this.grabbedLink = null;
					if (flag3)
					{
						Action onHandLinkChanged2 = TakeMyHand_HandLink.OnHandLinkChanged;
						if (onHandLinkChanged2 != null)
						{
							onHandLinkChanged2();
						}
					}
				}
				else if (player == this.myRig.OwningNetPlayer)
				{
					bool flag4 = this.grabbedPlayer != null;
					this.grabbedPlayer = null;
					this.grabbedLink = null;
					if (flag4)
					{
						Action onHandLinkChanged3 = TakeMyHand_HandLink.OnHandLinkChanged;
						if (onHandLinkChanged3 != null)
						{
							onHandLinkChanged3();
						}
					}
				}
				else
				{
					this.grabbedPlayer = player;
					this.grabbedHandIsLeft = grabbedHandIsLeft;
					this.IsTentacleGrab = isTentacleGrab;
					this.CheckFormLinkWithRemoteGrab();
					Action onHandLinkChanged4 = TakeMyHand_HandLink.OnHandLinkChanged;
					if (onHandLinkChanged4 != null)
					{
						onHandLinkChanged4();
					}
				}
			}
			else
			{
				bool flag5 = this.grabbedPlayer != null;
				this.grabbedPlayer = null;
				this.grabbedLink = null;
				if (flag5)
				{
					Action onHandLinkChanged5 = TakeMyHand_HandLink.OnHandLinkChanged;
					if (onHandLinkChanged5 != null)
					{
						onHandLinkChanged5();
					}
				}
			}
		}
		this.lastReadGrabbedPlayerActorNumber = grabbedPlayerActorNumber;
	}

	// Token: 0x060041BF RID: 16831 RVA: 0x0015E520 File Offset: 0x0015C720
	private bool IsLocalGrabInRange(bool grabbedLeftHand, Vector3 handLocalPos, Quaternion bodyWorldRot, Vector3 bodyWorldPos, float tolerance)
	{
		return ((grabbedLeftHand ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).transform.position - (bodyWorldPos + bodyWorldRot * handLocalPos)).IsShorterThan(tolerance);
	}

	// Token: 0x060041C0 RID: 16832 RVA: 0x0015E560 File Offset: 0x0015C760
	private void CheckFormLinkWithRemoteGrab()
	{
		if (this.grabbedPlayer != NetworkSystem.Instance.LocalPlayer)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(this.grabbedPlayer, out rigContainer))
			{
				TakeMyHand_HandLink takeMyHand_HandLink = (this.grabbedHandIsLeft ? rigContainer.Rig.leftHandLink : rigContainer.Rig.rightHandLink);
				if (takeMyHand_HandLink.grabbedPlayer == this.myRig.creator)
				{
					this.grabbedLink = takeMyHand_HandLink;
					this.grabbedLink.grabbedLink = this;
				}
			}
			return;
		}
		TakeMyHand_HandLink takeMyHand_HandLink2 = (this.grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink);
		if (takeMyHand_HandLink2.isReadyForGrabbing)
		{
			takeMyHand_HandLink2.LocalCreateLink(this);
			return;
		}
		takeMyHand_HandLink2.RejectGrabsFor(0.5f);
	}

	// Token: 0x060041C1 RID: 16833 RVA: 0x0015E618 File Offset: 0x0015C818
	public HandLinkAuthorityStatus GetChainAuthority(out int stepsToAuth)
	{
		TakeMyHand_HandLink takeMyHand_HandLink = this.grabbedLink;
		int num = 1;
		HandLinkAuthorityStatus handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.None, -1f, -1);
		stepsToAuth = -1;
		while (takeMyHand_HandLink != null && num < 10 && !takeMyHand_HandLink.IsLocal)
		{
			if (takeMyHand_HandLink.isGroundedHand)
			{
				stepsToAuth = num;
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.HandGrounded, -1f, -1);
			}
			if (handLinkAuthorityStatus.type < HandLinkAuthorityType.ResidualHandGrounded && (double)(takeMyHand_HandLink.myRig.LastHandTouchedGroundAtNetworkTime + 1f) > PhotonNetwork.Time)
			{
				stepsToAuth = num;
				handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.ResidualHandGrounded, takeMyHand_HandLink.myRig.LastHandTouchedGroundAtNetworkTime, takeMyHand_HandLink.myRig.OwningNetPlayer.ActorNumber);
			}
			else if (handLinkAuthorityStatus.type < HandLinkAuthorityType.ButtGrounded && takeMyHand_HandLink.isGroundedButt)
			{
				stepsToAuth = num;
				handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.ButtGrounded, -1f, -1);
			}
			else if (handLinkAuthorityStatus.type == HandLinkAuthorityType.None)
			{
				HandLinkAuthorityStatus handLinkAuthorityStatus2 = new HandLinkAuthorityStatus(HandLinkAuthorityType.None, takeMyHand_HandLink.myRig.LastTouchedGroundAtNetworkTime, takeMyHand_HandLink.myRig.OwningNetPlayer.ActorNumber);
				if (handLinkAuthorityStatus2 > handLinkAuthorityStatus)
				{
					stepsToAuth = num;
					handLinkAuthorityStatus = handLinkAuthorityStatus2;
				}
			}
			num++;
			takeMyHand_HandLink = takeMyHand_HandLink.myOtherHandLink.grabbedLink;
		}
		return handLinkAuthorityStatus;
	}

	// Token: 0x060041C2 RID: 16834 RVA: 0x0015E730 File Offset: 0x0015C930
	public void VisuallySnapHandsTogether()
	{
		if (this.grabbedLink == null)
		{
			return;
		}
		if (this.IsTentacleGrab || this.grabbedLink.IsTentacleGrab)
		{
			return;
		}
		if (this.grabbedLink.snapPositionCalculatedAtFrame == Time.frameCount)
		{
			this.snapPositionCalculatedAtFrame = Time.frameCount;
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = this.grabbedLink.transform.position;
		Vector3 vector = (position + position2) / 2f;
		Vector3 vector2 = (this.isLeftHand ? this.myRig.leftHand.rigTarget : this.myRig.rightHand.rigTarget).position - position;
		Vector3 vector3 = (this.grabbedLink.isLeftHand ? this.grabbedLink.myRig.leftHand.rigTarget : this.grabbedLink.myRig.rightHand.rigTarget).position - position2;
		Vector3 vector4 = vector + vector2;
		Vector3 vector5 = vector + vector3;
		this.myIK.OverrideTargetPos(this.isLeftHand, vector4);
		this.grabbedLink.myIK.OverrideTargetPos(this.grabbedLink.isLeftHand, vector5);
	}

	// Token: 0x060041C3 RID: 16835 RVA: 0x0015E86A File Offset: 0x0015CA6A
	public void PlayVicariousTapHaptic()
	{
		GorillaTagger.Instance.StartVibration(this.isLeftHand, this.hapticStrengthOnVicariousTap, this.hapticDurationOnVicariousTap);
	}

	// Token: 0x04005250 RID: 21072
	[FormerlySerializedAs("myPlayer")]
	[SerializeField]
	public VRRig myRig;

	// Token: 0x04005251 RID: 21073
	[FormerlySerializedAs("leftHand")]
	[SerializeField]
	private bool isLeftHand;

	// Token: 0x04005252 RID: 21074
	[SerializeField]
	public GorillaIK myIK;

	// Token: 0x04005253 RID: 21075
	private TakeMyHand_HandLink myOtherHandLink;

	// Token: 0x04005254 RID: 21076
	private bool isReadyForGrabbing;

	// Token: 0x04005255 RID: 21077
	public bool isGroundedHand;

	// Token: 0x04005256 RID: 21078
	public bool isGroundedButt;

	// Token: 0x04005257 RID: 21079
	private bool wasGripPressed;

	// Token: 0x04005258 RID: 21080
	private float gripPressedAtTimestamp;

	// Token: 0x04005259 RID: 21081
	private float rejectGrabsUntilTimestamp;

	// Token: 0x0400525A RID: 21082
	public TakeMyHand_HandLink grabbedLink;

	// Token: 0x0400525B RID: 21083
	public NetPlayer grabbedPlayer;

	// Token: 0x0400525C RID: 21084
	public bool grabbedHandIsLeft;

	// Token: 0x0400525F RID: 21087
	private const bool DEBUG_GRAB_ANYONE = false;

	// Token: 0x04005260 RID: 21088
	[SerializeField]
	private float hapticStrengthOnGrab;

	// Token: 0x04005261 RID: 21089
	[SerializeField]
	private float hapticDurationOnGrab;

	// Token: 0x04005262 RID: 21090
	[SerializeField]
	private float hapticStrengthOnVicariousTap;

	// Token: 0x04005263 RID: 21091
	[SerializeField]
	private float hapticDurationOnVicariousTap;

	// Token: 0x04005264 RID: 21092
	[SerializeField]
	private AudioClip audioOnGrab;

	// Token: 0x04005265 RID: 21093
	public InteractionPoint interactionPoint;

	// Token: 0x04005266 RID: 21094
	public static Action OnHandLinkChanged;

	// Token: 0x04005268 RID: 21096
	private int lastReadGrabbedPlayerActorNumber;

	// Token: 0x04005269 RID: 21097
	private int snapPositionCalculatedAtFrame = -1;
}
