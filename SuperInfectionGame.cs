using System;
using GorillaGameModes;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000181 RID: 385
public sealed class SuperInfectionGame : GorillaTagManager
{
	// Token: 0x170000EA RID: 234
	// (get) Token: 0x06000A3B RID: 2619 RVA: 0x00037354 File Offset: 0x00035554
	// (set) Token: 0x06000A3C RID: 2620 RVA: 0x0003735B File Offset: 0x0003555B
	public new static SuperInfectionGame instance { get; private set; }

	// Token: 0x06000A3D RID: 2621 RVA: 0x00037363 File Offset: 0x00035563
	public override GameModeType GameType()
	{
		return GameModeType.SuperInfect;
	}

	// Token: 0x170000EB RID: 235
	// (get) Token: 0x06000A3E RID: 2622 RVA: 0x00037367 File Offset: 0x00035567
	// (set) Token: 0x06000A3F RID: 2623 RVA: 0x0003736F File Offset: 0x0003556F
	[DebugReadout]
	public ESuperInfectionGameState gameState { get; private set; }

	// Token: 0x06000A40 RID: 2624 RVA: 0x00037378 File Offset: 0x00035578
	public override void Awake()
	{
		SuperInfectionGame.instance = this;
		this.gameState = ESuperInfectionGameState.Stopped;
		base.Awake();
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x0003738D File Offset: 0x0003558D
	public override void OnEnable()
	{
		base.OnEnable();
		SIProgression instance = SIProgression.Instance;
		if (instance == null)
		{
			return;
		}
		instance.ResetTelemetryIntervalData();
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x000373A4 File Offset: 0x000355A4
	public override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x000373AC File Offset: 0x000355AC
	public override void Tick()
	{
		base.Tick();
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x000373B4 File Offset: 0x000355B4
	public override void StartPlaying()
	{
		this.gameState = ESuperInfectionGameState.Starting;
		base.StartPlaying();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			SIProgression.Instance.AddRoundTelemetry();
		}
		VRRig.LocalRig.EnableSuperInfectionHands(true);
		for (int i = 0; i < this.currentNetPlayerArray.Length; i++)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(this.currentNetPlayerArray[i], out rigContainer))
			{
				rigContainer.Rig.EnableSuperInfectionHands(true);
			}
		}
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x00037424 File Offset: 0x00035624
	public override void StopPlaying()
	{
		base.StopPlaying();
		this.gameState = ESuperInfectionGameState.Stopped;
		VRRig.LocalRig.EnableSuperInfectionHands(false);
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x00037440 File Offset: 0x00035640
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(newPlayer, out rigContainer))
		{
			rigContainer.Rig.EnableSuperInfectionHands(true);
		}
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x0003746F File Offset: 0x0003566F
	public override string GameModeName()
	{
		return "SUPER INFECTION";
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x00037478 File Offset: 0x00035678
	public override string GameModeNameRoomLabel()
	{
		string text;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_SUPER_INFECTION_ROOM_LABEL", out text, "(SUPER INFECTION GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_SUPER_INFECTION_ROOM_LABEL]");
		}
		return text;
	}

	// Token: 0x06000A49 RID: 2633 RVA: 0x000374A3 File Offset: 0x000356A3
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x000374AB File Offset: 0x000356AB
	protected override void InfectionRoundStart()
	{
		base.InfectionRoundStart();
		this.gameState = ESuperInfectionGameState.Playing;
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x000374BA File Offset: 0x000356BA
	protected override void InfectionRoundEnd()
	{
		base.InfectionRoundEnd();
		this.gameState = ESuperInfectionGameState.RoundRestarting;
		SuperInfectionManager.activeSuperInfectionManager.zoneSuperInfection.ResetPerRoundResources();
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x000374D8 File Offset: 0x000356D8
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return base.LocalCanTag(myPlayer, otherPlayer);
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x000374E2 File Offset: 0x000356E2
	public override void UpdatePlayerAppearance(VRRig rig)
	{
		base.UpdatePlayerAppearance(rig);
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x000374EB File Offset: 0x000356EB
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		return base.MyMatIndex(forPlayer);
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x000374F4 File Offset: 0x000356F4
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeWrite(stream, info);
		stream.SendNext(this.gameState);
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x00037510 File Offset: 0x00035710
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeRead(stream, info);
		ESuperInfectionGameState esuperInfectionGameState = (ESuperInfectionGameState)stream.ReceiveNext();
		if (!Enum.IsDefined(typeof(ESuperInfectionGameState), this.gameState))
		{
			return;
		}
		this.gameState = esuperInfectionGameState;
		if (this.gameState != this._gameState_previous)
		{
			this._OnGameStateChanged();
			this._gameState_previous = this.gameState;
		}
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x00037575 File Offset: 0x00035775
	public void _OnGameStateChanged()
	{
		if (this.gameState == ESuperInfectionGameState.Starting)
		{
			SIProgression.Instance.AddRoundTelemetry();
		}
		GTDev.Log<string>(string.Format("Game state changed to {0} ...\n(was {1}).", this.gameState, this._gameState_previous), null);
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x000375B0 File Offset: 0x000357B0
	public override void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		try
		{
			SIProgression.Instance.HandleTagTelemetry(taggedPlayer, taggingPlayer);
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
		}
		RigContainer rigContainer;
		RigContainer rigContainer2;
		if (!VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer) || !VRRigCache.Instance.TryGetVrrig(taggingPlayer, out rigContainer2))
		{
			return;
		}
		if (taggingPlayer.ActorNumber != SIPlayer.LocalPlayer.ActorNr)
		{
			return;
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Dash] > 0)
		{
			PlayerGameEvents.MiscEvent("SIDashTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Thruster] > 0)
		{
			PlayerGameEvents.MiscEvent("SIThrusterTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Stilt] > 0)
		{
			PlayerGameEvents.MiscEvent("SIStiltTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Platform] > 0)
		{
			PlayerGameEvents.MiscEvent("SIPlatformTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Blaster] > 0)
		{
			PlayerGameEvents.MiscEvent("SIBlasterTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedOthersGadgets > 0)
		{
			PlayerGameEvents.MiscEvent("SIBorrowedGadgetTag", 1);
		}
		PlayerGameEvents.MiscEvent("SIGameModeTag", 1);
	}

	// Token: 0x04000C7B RID: 3195
	[SerializeField]
	private int _mySuperExampleSerializedField = 123;

	// Token: 0x04000C7D RID: 3197
	private ESuperInfectionGameState _gameState_previous;
}
