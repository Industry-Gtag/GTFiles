using System;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000874 RID: 2164
public sealed class GorillaGuardianManager : GorillaGameManager
{
	// Token: 0x17000502 RID: 1282
	// (get) Token: 0x06003837 RID: 14391 RVA: 0x0013293F File Offset: 0x00130B3F
	// (set) Token: 0x06003838 RID: 14392 RVA: 0x00132947 File Offset: 0x00130B47
	public bool isPlaying { get; private set; }

	// Token: 0x06003839 RID: 14393 RVA: 0x00132950 File Offset: 0x00130B50
	public override void StartPlaying()
	{
		base.StartPlaying();
		this.isPlaying = true;
		if (PhotonNetwork.IsMasterClient)
		{
			foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
			{
				gorillaGuardianZoneManager.StartPlaying();
			}
		}
	}

	// Token: 0x0600383A RID: 14394 RVA: 0x001329B4 File Offset: 0x00130BB4
	public override void StopPlaying()
	{
		base.StopPlaying();
		this.isPlaying = false;
		if (PhotonNetwork.IsMasterClient)
		{
			foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
			{
				gorillaGuardianZoneManager.StopPlaying();
			}
		}
	}

	// Token: 0x0600383B RID: 14395 RVA: 0x00132A18 File Offset: 0x00130C18
	public override void ResetGame()
	{
		base.ResetGame();
	}

	// Token: 0x0600383C RID: 14396 RVA: 0x00132A20 File Offset: 0x00130C20
	internal override void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		base.NetworkLinkSetup(netSerializer);
		netSerializer.AddRPCComponent<GuardianRPCs>();
	}

	// Token: 0x0600383D RID: 14397 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
	}

	// Token: 0x0600383E RID: 14398 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnSerializeRead(object newData)
	{
	}

	// Token: 0x0600383F RID: 14399 RVA: 0x00036275 File Offset: 0x00034475
	public override object OnSerializeWrite()
	{
		return null;
	}

	// Token: 0x06003840 RID: 14400 RVA: 0x00132A30 File Offset: 0x00130C30
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return this.IsPlayerGuardian(myPlayer) && !this.IsHoldingPlayer();
	}

	// Token: 0x06003841 RID: 14401 RVA: 0x00002076 File Offset: 0x00000276
	public override bool LocalIsTagged(NetPlayer player)
	{
		return false;
	}

	// Token: 0x06003842 RID: 14402 RVA: 0x00132A46 File Offset: 0x00130C46
	public override bool CanJoinFrienship(NetPlayer player)
	{
		return player != null && !this.IsPlayerGuardian(player);
	}

	// Token: 0x06003843 RID: 14403 RVA: 0x00132A58 File Offset: 0x00130C58
	public bool IsPlayerGuardian(NetPlayer player)
	{
		using (List<GorillaGuardianZoneManager>.Enumerator enumerator = GorillaGuardianZoneManager.zoneManagers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsPlayerGuardian(player))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06003844 RID: 14404 RVA: 0x00132AB4 File Offset: 0x00130CB4
	public void RequestEjectGuardian(NetPlayer player)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.EjectGuardian(player);
			return;
		}
		global::GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("GuardianRequestEject", false, Array.Empty<object>());
	}

	// Token: 0x06003845 RID: 14405 RVA: 0x00132ADC File Offset: 0x00130CDC
	public void EjectGuardian(NetPlayer player)
	{
		foreach (GorillaGuardianZoneManager gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers)
		{
			if (gorillaGuardianZoneManager.IsPlayerGuardian(player))
			{
				gorillaGuardianZoneManager.SetGuardian(null);
			}
		}
	}

	// Token: 0x06003846 RID: 14406 RVA: 0x00132B38 File Offset: 0x00130D38
	public void LaunchPlayer(NetPlayer launcher, Vector3 velocity)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(launcher, out rigContainer))
		{
			return;
		}
		if (Vector3.Magnitude(VRRigCache.Instance.localRig.Rig.transform.position - rigContainer.Rig.transform.position) > this.requiredGuardianDistance + Mathf.Epsilon)
		{
			return;
		}
		if (velocity.sqrMagnitude > this.maxLaunchVelocity * this.maxLaunchVelocity)
		{
			return;
		}
		GTPlayer.Instance.DoLaunch(velocity);
	}

	// Token: 0x06003847 RID: 14407 RVA: 0x00132BBC File Offset: 0x00130DBC
	public override void LocalTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool bodyHit, bool leftHand)
	{
		base.LocalTag(taggedPlayer, taggingPlayer, bodyHit, leftHand);
		if (bodyHit)
		{
			return;
		}
		RigContainer rigContainer;
		Vector3 vector;
		if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer) && this.CheckSlap(taggingPlayer, taggedPlayer, leftHand, out vector))
		{
			global::GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("GuardianLaunchPlayer", taggedPlayer, new object[] { vector });
			rigContainer.Rig.ApplyLocalTrajectoryOverride(vector);
			global::GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("ShowSlapEffects", true, new object[]
			{
				rigContainer.Rig.transform.position,
				vector.normalized
			});
			this.LocalPlaySlapEffect(rigContainer.Rig.transform.position, vector.normalized);
		}
	}

	// Token: 0x06003848 RID: 14408 RVA: 0x00132C80 File Offset: 0x00130E80
	private bool CheckSlap(NetPlayer slapper, NetPlayer target, bool leftHand, out Vector3 velocity)
	{
		velocity = Vector3.zero;
		if (this.IsHoldingPlayer(leftHand))
		{
			return false;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(slapper, out rigContainer))
		{
			return false;
		}
		Vector3 vector = GTPlayer.Instance.GetHandVelocityTracker(leftHand).GetAverageVelocity(true, 0.15f, false);
		Vector3 vector2 = (leftHand ? rigContainer.Rig.leftHandHoldsPlayer.transform.right : rigContainer.Rig.rightHandHoldsPlayer.transform.right);
		if (Vector3.Dot(vector.normalized, vector2) < this.slapFrontAlignmentThreshold && Vector3.Dot(vector.normalized, vector2) > this.slapBackAlignmentThreshold)
		{
			return false;
		}
		if (vector.magnitude < this.launchMinimumStrength)
		{
			return false;
		}
		vector = Vector3.ClampMagnitude(vector, this.maxLaunchVelocity);
		RigContainer rigContainer2;
		if (!VRRigCache.Instance.TryGetVrrig(target, out rigContainer2))
		{
			return false;
		}
		if (this.IsRigBeingHeld(rigContainer2.Rig) || rigContainer2.Rig.IsLocalTrajectoryOverrideActive())
		{
			return false;
		}
		if (!this.CheckLaunchRetriggerDelay(rigContainer2.Rig))
		{
			return false;
		}
		vector *= this.launchStrengthMultiplier;
		Vector3 vector3;
		if (rigContainer2.Rig.IsOnGround(this.launchGroundHeadCheckDist, this.launchGroundHandCheckDist, out vector3))
		{
			vector += vector3 * this.launchGroundKickup * Mathf.Clamp01(1f - Vector3.Dot(vector3, vector.normalized));
		}
		velocity = vector;
		return true;
	}

	// Token: 0x06003849 RID: 14409 RVA: 0x00132DEC File Offset: 0x00130FEC
	public override void HandleHandTap(NetPlayer tappingPlayer, Tappable hitTappable, bool leftHand, Vector3 handVelocity, Vector3 tapSurfaceNormal)
	{
		base.HandleHandTap(tappingPlayer, hitTappable, leftHand, handVelocity, tapSurfaceNormal);
		if (hitTappable != null)
		{
			TappableGuardianIdol tappableGuardianIdol = hitTappable as TappableGuardianIdol;
			if (tappableGuardianIdol != null && tappableGuardianIdol.isActivationReady)
			{
				tappableGuardianIdol.isActivationReady = false;
				GorillaTagger.Instance.StartVibration(leftHand, GorillaTagger.Instance.tapHapticStrength * this.hapticStrength, GorillaTagger.Instance.tapHapticDuration * this.hapticDuration);
			}
		}
		if (!this.IsPlayerGuardian(tappingPlayer))
		{
			return;
		}
		if (this.IsHoldingPlayer(leftHand))
		{
			return;
		}
		float num = Vector3.Dot(Vector3.down, handVelocity);
		if (num < this.slamTriggerTapSpeed || Vector3.Dot(Vector3.down, handVelocity.normalized) < this.slamTriggerAngle)
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(tappingPlayer, out rigContainer))
		{
			return;
		}
		VRMap vrmap = (leftHand ? rigContainer.Rig.leftHand : rigContainer.Rig.rightHand);
		Vector3 vector = vrmap.rigTarget.rotation * vrmap.trackingPositionOffset * rigContainer.Rig.scaleFactor;
		Vector3 vector2 = vrmap.rigTarget.position - vector;
		float num2 = Mathf.Clamp01((num - this.slamTriggerTapSpeed) / (this.slamMaxTapSpeed - this.slamTriggerTapSpeed));
		num2 = Mathf.Lerp(this.slamMinStrengthMultiplier, this.slamMaxStrengthMultiplier, num2);
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			RigContainer rigContainer2;
			if (RoomSystem.PlayersInRoom[i] != tappingPlayer && VRRigCache.Instance.TryGetVrrig(RoomSystem.PlayersInRoom[i], out rigContainer2))
			{
				VRRig rig = rigContainer2.Rig;
				if (!this.IsRigBeingHeld(rig) && this.CheckLaunchRetriggerDelay(rig))
				{
					Vector3 position = rig.transform.position;
					if (Vector3.SqrMagnitude(position - vector2) < this.slamRadius * this.slamRadius)
					{
						Vector3 vector3 = (position - vector2).normalized * num2;
						vector3 = Vector3.ClampMagnitude(vector3, this.maxLaunchVelocity);
						global::GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("GuardianLaunchPlayer", RoomSystem.PlayersInRoom[i], new object[] { vector3 });
					}
				}
			}
		}
		this.LocalPlaySlamEffect(vector2, Vector3.up);
		global::GorillaGameModes.GameMode.ActiveNetworkHandler.SendRPC("ShowSlamEffect", true, new object[]
		{
			vector2,
			Vector3.up
		});
	}

	// Token: 0x0600384A RID: 14410 RVA: 0x0013305E File Offset: 0x0013125E
	private bool CheckLaunchRetriggerDelay(VRRig launchedRig)
	{
		return launchedRig.fxSettings.callSettings[7].CallLimitSettings.CheckCallTime(Time.time);
	}

	// Token: 0x0600384B RID: 14411 RVA: 0x0013307C File Offset: 0x0013127C
	private bool IsHoldingPlayer()
	{
		return this.IsHoldingPlayer(true) || this.IsHoldingPlayer(false);
	}

	// Token: 0x0600384C RID: 14412 RVA: 0x00133090 File Offset: 0x00131290
	private bool IsHoldingPlayer(bool leftHand)
	{
		return (leftHand && EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment is HoldableHand) || (!leftHand && EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment is HoldableHand);
	}

	// Token: 0x0600384D RID: 14413 RVA: 0x001330EC File Offset: 0x001312EC
	private bool IsRigBeingHeld(VRRig rig)
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment != null)
		{
			HoldableHand holdableHand = EquipmentInteractor.instance.leftHandHeldEquipment as HoldableHand;
			if (holdableHand != null && holdableHand.Rig == rig)
			{
				return true;
			}
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment != null)
		{
			HoldableHand holdableHand2 = EquipmentInteractor.instance.rightHandHeldEquipment as HoldableHand;
			if (holdableHand2 != null && holdableHand2.Rig == rig)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600384E RID: 14414 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600384F RID: 14415 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06003850 RID: 14416 RVA: 0x00133160 File Offset: 0x00131360
	public override GameModeType GameType()
	{
		return GameModeType.Guardian;
	}

	// Token: 0x06003851 RID: 14417 RVA: 0x00133163 File Offset: 0x00131363
	public override string GameModeName()
	{
		return "GUARDIAN";
	}

	// Token: 0x06003852 RID: 14418 RVA: 0x0013316C File Offset: 0x0013136C
	public override string GameModeNameRoomLabel()
	{
		string text;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_GUARDIAN_ROOM_LABEL", out text, "(GUARDIAN GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_GUARDIAN_ROOM_LABEL]");
		}
		return text;
	}

	// Token: 0x06003853 RID: 14419 RVA: 0x00133197 File Offset: 0x00131397
	public void PlaySlapEffect(Vector3 location, Vector3 direction)
	{
		this.LocalPlaySlapEffect(location, direction);
	}

	// Token: 0x06003854 RID: 14420 RVA: 0x001331A1 File Offset: 0x001313A1
	private void LocalPlaySlapEffect(Vector3 location, Vector3 direction)
	{
		ObjectPools.instance.Instantiate(this.slapImpactPrefab, location, Quaternion.LookRotation(direction), true);
	}

	// Token: 0x06003855 RID: 14421 RVA: 0x001331BC File Offset: 0x001313BC
	public void PlaySlamEffect(Vector3 location, Vector3 direction)
	{
		this.LocalPlaySlamEffect(location, direction);
	}

	// Token: 0x06003856 RID: 14422 RVA: 0x001331C6 File Offset: 0x001313C6
	private void LocalPlaySlamEffect(Vector3 location, Vector3 direction)
	{
		ObjectPools.instance.Instantiate(this.slamImpactPrefab, location, Quaternion.LookRotation(direction), true);
	}

	// Token: 0x0400484F RID: 18511
	[Space]
	[SerializeField]
	private float slapFrontAlignmentThreshold = 0.7f;

	// Token: 0x04004850 RID: 18512
	[SerializeField]
	private float slapBackAlignmentThreshold = 0.7f;

	// Token: 0x04004851 RID: 18513
	[SerializeField]
	private float launchMinimumStrength = 6f;

	// Token: 0x04004852 RID: 18514
	[SerializeField]
	private float launchStrengthMultiplier = 1f;

	// Token: 0x04004853 RID: 18515
	[SerializeField]
	private float launchGroundHeadCheckDist = 1.2f;

	// Token: 0x04004854 RID: 18516
	[SerializeField]
	private float launchGroundHandCheckDist = 0.4f;

	// Token: 0x04004855 RID: 18517
	[SerializeField]
	private float launchGroundKickup = 3f;

	// Token: 0x04004856 RID: 18518
	[Space]
	[SerializeField]
	private float slamTriggerTapSpeed = 7f;

	// Token: 0x04004857 RID: 18519
	[SerializeField]
	private float slamMaxTapSpeed = 16f;

	// Token: 0x04004858 RID: 18520
	[SerializeField]
	private float slamTriggerAngle = 0.7f;

	// Token: 0x04004859 RID: 18521
	[SerializeField]
	private float slamRadius = 2.4f;

	// Token: 0x0400485A RID: 18522
	[SerializeField]
	private float slamMinStrengthMultiplier = 3f;

	// Token: 0x0400485B RID: 18523
	[SerializeField]
	private float slamMaxStrengthMultiplier = 10f;

	// Token: 0x0400485C RID: 18524
	[Space]
	[SerializeField]
	private GameObject slapImpactPrefab;

	// Token: 0x0400485D RID: 18525
	[SerializeField]
	private GameObject slamImpactPrefab;

	// Token: 0x0400485E RID: 18526
	[Space]
	[SerializeField]
	private float hapticStrength = 1f;

	// Token: 0x0400485F RID: 18527
	[SerializeField]
	private float hapticDuration = 1f;

	// Token: 0x04004861 RID: 18529
	private float requiredGuardianDistance = 10f;

	// Token: 0x04004862 RID: 18530
	private float maxLaunchVelocity = 20f;
}
