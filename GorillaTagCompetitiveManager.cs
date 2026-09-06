using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020008A9 RID: 2217
public class GorillaTagCompetitiveManager : GorillaTagManager
{
	// Token: 0x06003A05 RID: 14853 RVA: 0x0013C318 File Offset: 0x0013A518
	public float GetRoundDuration()
	{
		return this.roundDuration;
	}

	// Token: 0x06003A06 RID: 14854 RVA: 0x0013C320 File Offset: 0x0013A520
	public GorillaTagCompetitiveManager.GameState GetCurrentGameState()
	{
		return this.gameState;
	}

	// Token: 0x06003A07 RID: 14855 RVA: 0x0013C328 File Offset: 0x0013A528
	public bool IsMatchActive()
	{
		return this.gameState == GorillaTagCompetitiveManager.GameState.Playing;
	}

	// Token: 0x14000067 RID: 103
	// (add) Token: 0x06003A08 RID: 14856 RVA: 0x0013C334 File Offset: 0x0013A534
	// (remove) Token: 0x06003A09 RID: 14857 RVA: 0x0013C368 File Offset: 0x0013A568
	public static event Action<GorillaTagCompetitiveManager.GameState> onStateChanged;

	// Token: 0x14000068 RID: 104
	// (add) Token: 0x06003A0A RID: 14858 RVA: 0x0013C39C File Offset: 0x0013A59C
	// (remove) Token: 0x06003A0B RID: 14859 RVA: 0x0013C3D0 File Offset: 0x0013A5D0
	public static event Action<float> onUpdateRemainingTime;

	// Token: 0x14000069 RID: 105
	// (add) Token: 0x06003A0C RID: 14860 RVA: 0x0013C404 File Offset: 0x0013A604
	// (remove) Token: 0x06003A0D RID: 14861 RVA: 0x0013C438 File Offset: 0x0013A638
	public static event Action<NetPlayer> onPlayerJoined;

	// Token: 0x1400006A RID: 106
	// (add) Token: 0x06003A0E RID: 14862 RVA: 0x0013C46C File Offset: 0x0013A66C
	// (remove) Token: 0x06003A0F RID: 14863 RVA: 0x0013C4A0 File Offset: 0x0013A6A0
	public static event Action<NetPlayer> onPlayerLeft;

	// Token: 0x1400006B RID: 107
	// (add) Token: 0x06003A10 RID: 14864 RVA: 0x0013C4D4 File Offset: 0x0013A6D4
	// (remove) Token: 0x06003A11 RID: 14865 RVA: 0x0013C508 File Offset: 0x0013A708
	public static event Action onRoundStart;

	// Token: 0x1400006C RID: 108
	// (add) Token: 0x06003A12 RID: 14866 RVA: 0x0013C53C File Offset: 0x0013A73C
	// (remove) Token: 0x06003A13 RID: 14867 RVA: 0x0013C570 File Offset: 0x0013A770
	public static event Action onRoundEnd;

	// Token: 0x1400006D RID: 109
	// (add) Token: 0x06003A14 RID: 14868 RVA: 0x0013C5A4 File Offset: 0x0013A7A4
	// (remove) Token: 0x06003A15 RID: 14869 RVA: 0x0013C5D8 File Offset: 0x0013A7D8
	public static event Action<NetPlayer, NetPlayer> onTagOccurred;

	// Token: 0x06003A16 RID: 14870 RVA: 0x0013C60B File Offset: 0x0013A80B
	public static void RegisterScoreboard(GorillaTagCompetitiveScoreboard scoreboard)
	{
		GorillaTagCompetitiveManager.scoreboards.Add(scoreboard);
	}

	// Token: 0x06003A17 RID: 14871 RVA: 0x0013C618 File Offset: 0x0013A818
	public static void DeregisterScoreboard(GorillaTagCompetitiveScoreboard scoreboard)
	{
		GorillaTagCompetitiveManager.scoreboards.Remove(scoreboard);
	}

	// Token: 0x06003A18 RID: 14872 RVA: 0x0013C628 File Offset: 0x0013A828
	public override void StartPlaying()
	{
		base.StartPlaying();
		this.scoring = base.GetComponentInChildren<RankedMultiplayerScore>();
		if (this.scoring != null)
		{
			this.scoring.Initialize();
		}
		VRRig.LocalRig.EnableRankedTimerWatch(true);
		for (int i = 0; i < this.currentNetPlayerArray.Length; i++)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(this.currentNetPlayerArray[i], out rigContainer))
			{
				rigContainer.Rig.EnableRankedTimerWatch(true);
			}
		}
	}

	// Token: 0x06003A19 RID: 14873 RVA: 0x0013C6A0 File Offset: 0x0013A8A0
	public override void StopPlaying()
	{
		base.StopPlaying();
		VRRig.LocalRig.EnableRankedTimerWatch(false);
		if (this.scoring != null)
		{
			this.scoring.ResetMatch();
			this.scoring.Unsubscribe();
		}
		for (int i = 0; i < GorillaTagCompetitiveManager.scoreboards.Count; i++)
		{
			GorillaTagCompetitiveManager.scoreboards[i].UpdateScores(this.gameState, this.lastActiveTime, null, this.scoring.PlayerRankedTiers, this.scoring.ProjectedEloDeltas, this.currentInfected, this.scoring.Progression);
		}
	}

	// Token: 0x06003A1A RID: 14874 RVA: 0x0013C73B File Offset: 0x0013A93B
	public override void ResetGame()
	{
		base.ResetGame();
		this.gameState = GorillaTagCompetitiveManager.GameState.None;
	}

	// Token: 0x06003A1B RID: 14875 RVA: 0x0013C74A File Offset: 0x0013A94A
	internal override void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		base.NetworkLinkSetup(netSerializer);
		netSerializer.AddRPCComponent<GorillaTagCompetitiveRPCs>();
	}

	// Token: 0x06003A1C RID: 14876 RVA: 0x0013C75C File Offset: 0x0013A95C
	public override void Tick()
	{
		if (this.stateRemainingTime > 0f)
		{
			this.stateRemainingTime -= Time.deltaTime;
			if (this.stateRemainingTime <= 0f)
			{
				this.UpdateState();
			}
			Action<float> action = GorillaTagCompetitiveManager.onUpdateRemainingTime;
			if (action != null)
			{
				action(this.stateRemainingTime);
			}
		}
		base.Tick();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (Time.time - this.lastWaitingForPlayerPingRoomTime > this.waitingForPlayerPingRoomDuration)
			{
				this.PingRoom();
				this.lastWaitingForPlayerPingRoomTime = Time.time;
			}
			if (Time.time - this.lastWaitingForPlayerPingRoomTime > 3f)
			{
				this.ShowDebugPing = false;
			}
		}
		this.UpdateScoreboards();
	}

	// Token: 0x06003A1D RID: 14877 RVA: 0x0013C808 File Offset: 0x0013AA08
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.PingRoom();
			this.lastWaitingForPlayerPingRoomTime = Time.time;
		}
	}

	// Token: 0x06003A1E RID: 14878 RVA: 0x0013C830 File Offset: 0x0013AA30
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (newPlayer == NetworkSystem.Instance.LocalPlayer)
		{
			using (List<GorillaTagCompetitiveForcedLeaveRoomVolume>.Enumerator enumerator = this.forceLeaveRoomVolumes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ContainsPoint(VRRig.LocalRig.transform.position))
					{
						NetworkSystem.Instance.ReturnToSinglePlayer();
						return;
					}
				}
			}
			object obj;
			if (NetworkSystem.Instance.IsMasterClient)
			{
				GorillaTagCompetitiveServerApi.Instance.RequestCreateMatchId(delegate(string id)
				{
					Hashtable hashtable = new Hashtable();
					hashtable.Add("matchId", id);
					PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
				});
			}
			else if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("matchId", out obj))
			{
				GorillaTagCompetitiveServerApi.Instance.RequestValidateMatchJoin((string)obj, delegate(bool valid)
				{
					if (!valid)
					{
						Debug.LogError("ValidateMatchJoin failed. Leaving room!");
						NetworkSystem.Instance.ReturnToSinglePlayer();
					}
				});
			}
		}
		Action<NetPlayer> action = GorillaTagCompetitiveManager.onPlayerJoined;
		if (action != null)
		{
			action(newPlayer);
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(newPlayer, out rigContainer))
		{
			rigContainer.Rig.EnableRankedTimerWatch(true);
		}
	}

	// Token: 0x06003A1F RID: 14879 RVA: 0x0013C964 File Offset: 0x0013AB64
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		Action<NetPlayer> action = GorillaTagCompetitiveManager.onPlayerLeft;
		if (action != null)
		{
			action(otherPlayer);
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(otherPlayer, out rigContainer))
		{
			rigContainer.Rig.EnableRankedTimerWatch(false);
		}
	}

	// Token: 0x06003A20 RID: 14880 RVA: 0x0013C9A4 File Offset: 0x0013ABA4
	public RankedMultiplayerScore GetScoring()
	{
		return this.scoring;
	}

	// Token: 0x06003A21 RID: 14881 RVA: 0x0013C9AC File Offset: 0x0013ABAC
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return base.LocalCanTag(myPlayer, otherPlayer) && this.gameState != GorillaTagCompetitiveManager.GameState.StartingCountdown && this.gameState != GorillaTagCompetitiveManager.GameState.PostRound;
	}

	// Token: 0x06003A22 RID: 14882 RVA: 0x0013C9CF File Offset: 0x0013ABCF
	public override bool LocalIsTagged(NetPlayer player)
	{
		return this.gameState != GorillaTagCompetitiveManager.GameState.StartingCountdown && this.gameState != GorillaTagCompetitiveManager.GameState.PostRound && base.LocalIsTagged(player);
	}

	// Token: 0x06003A23 RID: 14883 RVA: 0x0013C9EC File Offset: 0x0013ABEC
	public override void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		base.ReportTag(taggedPlayer, taggingPlayer);
	}

	// Token: 0x06003A24 RID: 14884 RVA: 0x0013C9F6 File Offset: 0x0013ABF6
	public override GameModeType GameType()
	{
		return GameModeType.InfectionCompetitive;
	}

	// Token: 0x06003A25 RID: 14885 RVA: 0x0013C9FA File Offset: 0x0013ABFA
	public override string GameModeName()
	{
		return "COMP-INFECT";
	}

	// Token: 0x06003A26 RID: 14886 RVA: 0x0013CA04 File Offset: 0x0013AC04
	public override string GameModeNameRoomLabel()
	{
		string text;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_COMP_INF_ROOM_LABEL", out text, "(COMP-INFECT GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_COMP_INF_ROOM_LABEL]");
		}
		return text;
	}

	// Token: 0x06003A27 RID: 14887 RVA: 0x00002076 File Offset: 0x00000276
	public override bool CanJoinFrienship(NetPlayer player)
	{
		return false;
	}

	// Token: 0x06003A28 RID: 14888 RVA: 0x0013CA2F File Offset: 0x0013AC2F
	public override void UpdateInfectionState()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		if (this.gameState == GorillaTagCompetitiveManager.GameState.Playing && this.IsEveryoneTagged())
		{
			this.HandleInfectionRoundComplete();
		}
	}

	// Token: 0x06003A29 RID: 14889 RVA: 0x0013CA58 File Offset: 0x0013AC58
	public override void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (!this.currentInfected.Contains(taggingPlayer))
		{
			return;
		}
		RigContainer rigContainer;
		RigContainer rigContainer2;
		if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer) && VRRigCache.Instance.TryGetVrrig(taggingPlayer, out rigContainer2))
		{
			VRRig rig = rigContainer2.Rig;
			VRRig rig2 = rigContainer.Rig;
			if (!rig.IsPositionInRange(rig2.transform.position, 6f) && !rig.CheckTagDistanceRollback(rig2, 6f, 0.2f))
			{
				return;
			}
			if (!NetworkSystem.Instance.IsMasterClient && this.gameState == GorillaTagCompetitiveManager.GameState.Playing && !this.currentInfected.Contains(taggedPlayer))
			{
				base.AddLastTagged(taggedPlayer, taggingPlayer);
				this.currentInfected.Add(taggedPlayer);
			}
			Action<NetPlayer, NetPlayer> action = GorillaTagCompetitiveManager.onTagOccurred;
			if (action == null)
			{
				return;
			}
			action(taggedPlayer, taggingPlayer);
		}
	}

	// Token: 0x06003A2A RID: 14890 RVA: 0x0013CB1C File Offset: 0x0013AD1C
	private void SetState(GorillaTagCompetitiveManager.GameState newState)
	{
		if (newState != this.gameState)
		{
			GorillaTagCompetitiveManager.GameState gameState = this.gameState;
			this.gameState = newState;
			switch (this.gameState)
			{
			case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
				this.EnterStateWaitingForPlayers();
				break;
			case GorillaTagCompetitiveManager.GameState.StartingCountdown:
				this.EnterStateStartingCountdown();
				break;
			case GorillaTagCompetitiveManager.GameState.Playing:
				this.EnterStatePlaying();
				break;
			case GorillaTagCompetitiveManager.GameState.PostRound:
				this.EnterStatePostRound();
				break;
			}
			Action<GorillaTagCompetitiveManager.GameState> action = GorillaTagCompetitiveManager.onStateChanged;
			if (action != null)
			{
				action(this.gameState);
			}
			Action<float> action2 = GorillaTagCompetitiveManager.onUpdateRemainingTime;
			if (action2 != null)
			{
				action2(this.stateRemainingTime);
			}
			if (this.gameState == GorillaTagCompetitiveManager.GameState.Playing)
			{
				Action action3 = GorillaTagCompetitiveManager.onRoundStart;
				if (action3 != null)
				{
					action3();
				}
			}
			else if (gameState == GorillaTagCompetitiveManager.GameState.Playing)
			{
				Action action4 = GorillaTagCompetitiveManager.onRoundEnd;
				if (action4 != null)
				{
					action4();
				}
			}
			GTDev.Log<string>(string.Format("!! Competitive SetState: {0} at: {1}", this.gameState, Time.time), null);
		}
	}

	// Token: 0x06003A2B RID: 14891 RVA: 0x0013CC02 File Offset: 0x0013AE02
	private void EnterStateWaitingForPlayers()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			base.SetisCurrentlyTag(true);
			base.ClearInfectionState();
		}
	}

	// Token: 0x06003A2C RID: 14892 RVA: 0x0013CC20 File Offset: 0x0013AE20
	private void EnterStateStartingCountdown()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.isCurrentlyTag)
			{
				base.SetisCurrentlyTag(false);
			}
			this.currentIt = null;
			base.ClearInfectionState();
			GameMode.RefreshPlayers();
			this.CheckForInfected();
			this.stateRemainingTime = this.startCountdownDuration;
		}
	}

	// Token: 0x06003A2D RID: 14893 RVA: 0x0013CC6C File Offset: 0x0013AE6C
	private void EnterStatePlaying()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.isCurrentlyTag)
			{
				base.SetisCurrentlyTag(false);
			}
			this.currentIt = null;
			this.stateRemainingTime = this.roundDuration;
			this.PingRoom();
		}
		this.DisplayScoreboardPredictedResults(false);
	}

	// Token: 0x06003A2E RID: 14894 RVA: 0x0013CCA9 File Offset: 0x0013AEA9
	private void EnterStatePostRound()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.isCurrentlyTag)
			{
				base.SetisCurrentlyTag(false);
			}
			this.currentIt = null;
			this.stateRemainingTime = this.postRoundDuration;
		}
		this.DisplayScoreboardPredictedResults(true);
	}

	// Token: 0x06003A2F RID: 14895 RVA: 0x0013CCE0 File Offset: 0x0013AEE0
	public override void UpdateState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			switch (this.gameState)
			{
			case GorillaTagCompetitiveManager.GameState.None:
				this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
				return;
			case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
				this.UpdateStateWaitingForPlayers();
				return;
			case GorillaTagCompetitiveManager.GameState.StartingCountdown:
				this.UpdateStateStartingCountdown();
				return;
			case GorillaTagCompetitiveManager.GameState.Playing:
				this.UpdateStatePlaying();
				return;
			case GorillaTagCompetitiveManager.GameState.PostRound:
				this.UpdateStatePostRound();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06003A30 RID: 14896 RVA: 0x0013CD40 File Offset: 0x0013AF40
	private void UpdateStateWaitingForPlayers()
	{
		if (this.IsInfectionPossible())
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.StartingCountdown);
			return;
		}
		if (this.isCurrentlyTag && this.currentIt == null)
		{
			int num = Random.Range(0, GameMode.ParticipatingPlayers.Count);
			this.ChangeCurrentIt(GameMode.ParticipatingPlayers[num], false);
		}
	}

	// Token: 0x06003A31 RID: 14897 RVA: 0x0013CD90 File Offset: 0x0013AF90
	private void UpdateStateStartingCountdown()
	{
		if (!this.IsInfectionPossible())
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
			return;
		}
		if (this.stateRemainingTime < 0f)
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.Playing);
			return;
		}
		this.CheckForInfected();
	}

	// Token: 0x06003A32 RID: 14898 RVA: 0x0013CDBD File Offset: 0x0013AFBD
	private void UpdateStatePlaying()
	{
		if (this.IsGameInvalid())
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
			return;
		}
		if (this.stateRemainingTime < 0f)
		{
			this.HandleInfectionRoundComplete();
			return;
		}
		if (this.IsEveryoneTagged())
		{
			this.HandleInfectionRoundComplete();
			return;
		}
		this.CheckForInfected();
	}

	// Token: 0x06003A33 RID: 14899 RVA: 0x0013CDF8 File Offset: 0x0013AFF8
	private void HandleInfectionRoundComplete()
	{
		foreach (NetPlayer netPlayer in GameMode.ParticipatingPlayers)
		{
			RoomSystem.SendSoundEffectToPlayer(2, 0.25f, netPlayer, true);
		}
		PlayerGameEvents.GameModeCompleteRound();
		GameMode.BroadcastRoundComplete();
		this.lastTaggedActorNr.Clear();
		this.waitingToStartNextInfectionGame = true;
		this.timeInfectedGameEnded = (double)Time.time;
		this.SetState(GorillaTagCompetitiveManager.GameState.PostRound);
	}

	// Token: 0x06003A34 RID: 14900 RVA: 0x0013CE80 File Offset: 0x0013B080
	private void UpdateStatePostRound()
	{
		if (this.stateRemainingTime < 0f)
		{
			if (this.IsInfectionPossible())
			{
				this.SetState(GorillaTagCompetitiveManager.GameState.StartingCountdown);
				return;
			}
			this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
		}
	}

	// Token: 0x06003A35 RID: 14901 RVA: 0x0013CEA8 File Offset: 0x0013B0A8
	private void PingRoom()
	{
		object obj;
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("matchId", out obj))
		{
			GorillaTagCompetitiveServerApi.Instance.RequestPingRoom((string)obj, delegate
			{
				this.ShowDebugPing = true;
			});
		}
	}

	// Token: 0x17000533 RID: 1331
	// (get) Token: 0x06003A36 RID: 14902 RVA: 0x0013CEE9 File Offset: 0x0013B0E9
	// (set) Token: 0x06003A37 RID: 14903 RVA: 0x0013CEF1 File Offset: 0x0013B0F1
	public bool ShowDebugPing { get; set; }

	// Token: 0x06003A38 RID: 14904 RVA: 0x0013CEFA File Offset: 0x0013B0FA
	private bool IsGameInvalid()
	{
		return GameMode.ParticipatingPlayers.Count <= 1;
	}

	// Token: 0x06003A39 RID: 14905 RVA: 0x0013CF0C File Offset: 0x0013B10C
	private bool IsInfectionPossible()
	{
		return GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold;
	}

	// Token: 0x06003A3A RID: 14906 RVA: 0x0013CF24 File Offset: 0x0013B124
	private bool IsEveryoneTagged()
	{
		bool flag = true;
		foreach (NetPlayer netPlayer in GameMode.ParticipatingPlayers)
		{
			if (!this.currentInfected.Contains(netPlayer))
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	// Token: 0x06003A3B RID: 14907 RVA: 0x0013CF84 File Offset: 0x0013B184
	private void CheckForInfected()
	{
		if (this.currentInfected.Count == 0)
		{
			int num = Random.Range(0, GameMode.ParticipatingPlayers.Count);
			this.AddInfectedPlayer(GameMode.ParticipatingPlayers[num], true);
		}
	}

	// Token: 0x06003A3C RID: 14908 RVA: 0x0013CFC1 File Offset: 0x0013B1C1
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeWrite(stream, info);
		stream.SendNext(this.gameState);
		stream.SendNext(this.stateRemainingTime);
	}

	// Token: 0x06003A3D RID: 14909 RVA: 0x0013CFF0 File Offset: 0x0013B1F0
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		base.OnSerializeRead(stream, info);
		GorillaTagCompetitiveManager.GameState gameState = (GorillaTagCompetitiveManager.GameState)stream.ReceiveNext();
		this.stateRemainingTime = (float)stream.ReceiveNext();
		this.SetState(gameState);
	}

	// Token: 0x06003A3E RID: 14910 RVA: 0x0013D03C File Offset: 0x0013B23C
	public void UpdateScoreboards()
	{
		List<RankedMultiplayerScore.PlayerScoreInRound> sortedScores = this.scoring.GetSortedScores();
		if (this.gameState == GorillaTagCompetitiveManager.GameState.Playing)
		{
			this.lastActiveTime = Time.time;
		}
		for (int i = 0; i < GorillaTagCompetitiveManager.scoreboards.Count; i++)
		{
			GorillaTagCompetitiveManager.scoreboards[i].UpdateScores(this.gameState, this.lastActiveTime, sortedScores, this.scoring.PlayerRankedTiers, this.scoring.ProjectedEloDeltas, this.currentInfected, this.scoring.Progression);
		}
	}

	// Token: 0x06003A3F RID: 14911 RVA: 0x0013D0C4 File Offset: 0x0013B2C4
	public void DisplayScoreboardPredictedResults(bool bShow)
	{
		for (int i = 0; i < GorillaTagCompetitiveManager.scoreboards.Count; i++)
		{
			GorillaTagCompetitiveManager.scoreboards[i].DisplayPredictedResults(bShow);
		}
	}

	// Token: 0x06003A40 RID: 14912 RVA: 0x0013D0F7 File Offset: 0x0013B2F7
	public void RegisterForcedLeaveVolume(GorillaTagCompetitiveForcedLeaveRoomVolume volume)
	{
		if (!this.forceLeaveRoomVolumes.Contains(volume))
		{
			this.forceLeaveRoomVolumes.Add(volume);
		}
	}

	// Token: 0x06003A41 RID: 14913 RVA: 0x0013D113 File Offset: 0x0013B313
	public void UnregisterForcedLeaveVolume(GorillaTagCompetitiveForcedLeaveRoomVolume volume)
	{
		this.forceLeaveRoomVolumes.Remove(volume);
	}

	// Token: 0x04004A22 RID: 18978
	[SerializeField]
	private float startCountdownDuration = 3f;

	// Token: 0x04004A23 RID: 18979
	[SerializeField]
	private float roundDuration = 300f;

	// Token: 0x04004A24 RID: 18980
	[SerializeField]
	private float postRoundDuration = 15f;

	// Token: 0x04004A25 RID: 18981
	[SerializeField]
	private float waitingForPlayerPingRoomDuration = 60f;

	// Token: 0x04004A26 RID: 18982
	private GorillaTagCompetitiveManager.GameState gameState;

	// Token: 0x04004A27 RID: 18983
	private float stateRemainingTime;

	// Token: 0x04004A28 RID: 18984
	private float lastActiveTime;

	// Token: 0x04004A29 RID: 18985
	private float lastWaitingForPlayerPingRoomTime;

	// Token: 0x04004A31 RID: 18993
	private RankedMultiplayerScore scoring;

	// Token: 0x04004A32 RID: 18994
	private List<GorillaTagCompetitiveForcedLeaveRoomVolume> forceLeaveRoomVolumes = new List<GorillaTagCompetitiveForcedLeaveRoomVolume>();

	// Token: 0x04004A33 RID: 18995
	private static List<GorillaTagCompetitiveScoreboard> scoreboards = new List<GorillaTagCompetitiveScoreboard>();

	// Token: 0x020008AA RID: 2218
	public enum GameState
	{
		// Token: 0x04004A36 RID: 18998
		None,
		// Token: 0x04004A37 RID: 18999
		WaitingForPlayers,
		// Token: 0x04004A38 RID: 19000
		StartingCountdown,
		// Token: 0x04004A39 RID: 19001
		Playing,
		// Token: 0x04004A3A RID: 19002
		PostRound
	}
}
