using System;
using System.Collections.Generic;
using System.Text;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200036E RID: 878
public class RacingManager : NetworkSceneObject, ITickSystemTick
{
	// Token: 0x17000222 RID: 546
	// (get) Token: 0x06001582 RID: 5506 RVA: 0x00071ED1 File Offset: 0x000700D1
	// (set) Token: 0x06001583 RID: 5507 RVA: 0x00071ED8 File Offset: 0x000700D8
	public static RacingManager instance { get; private set; }

	// Token: 0x17000223 RID: 547
	// (get) Token: 0x06001584 RID: 5508 RVA: 0x00071EE0 File Offset: 0x000700E0
	// (set) Token: 0x06001585 RID: 5509 RVA: 0x00071EE8 File Offset: 0x000700E8
	public bool TickRunning { get; set; }

	// Token: 0x06001586 RID: 5510 RVA: 0x00071EF4 File Offset: 0x000700F4
	private void Awake()
	{
		RacingManager.instance = this;
		HashSet<int> hashSet = new HashSet<int>();
		this.races = new RacingManager.Race[this.raceSetups.Length];
		for (int i = 0; i < this.raceSetups.Length; i++)
		{
			this.races[i] = new RacingManager.Race(i, this.raceSetups[i], hashSet, this.photonView);
		}
		RoomSystem.JoinedRoomEvent += new Action(this.OnRoomJoin);
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoined);
	}

	// Token: 0x06001587 RID: 5511 RVA: 0x00071F8A File Offset: 0x0007018A
	protected override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		TickSystem<object>.AddTickCallback(this);
		base.OnEnable();
	}

	// Token: 0x06001588 RID: 5512 RVA: 0x00071F9E File Offset: 0x0007019E
	protected override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		TickSystem<object>.RemoveTickCallback(this);
		base.OnDisable();
	}

	// Token: 0x06001589 RID: 5513 RVA: 0x00071FB4 File Offset: 0x000701B4
	private void OnRoomJoin()
	{
		for (int i = 0; i < this.races.Length; i++)
		{
			this.races[i].Clear();
		}
	}

	// Token: 0x0600158A RID: 5514 RVA: 0x00071FE4 File Offset: 0x000701E4
	private void OnPlayerJoined(NetPlayer player)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		for (int i = 0; i < this.races.Length; i++)
		{
			this.races[i].SendStateToNewPlayer(player);
		}
	}

	// Token: 0x0600158B RID: 5515 RVA: 0x00072020 File Offset: 0x00070220
	public void RegisterVisual(RaceVisual visual)
	{
		int raceId = visual.raceId;
		if (raceId >= 0 && raceId < this.races.Length)
		{
			this.races[raceId].RegisterVisual(visual);
		}
	}

	// Token: 0x0600158C RID: 5516 RVA: 0x00072051 File Offset: 0x00070251
	public void Button_StartRace(int raceId, int laps)
	{
		if (raceId >= 0 && raceId < this.races.Length)
		{
			this.races[raceId].Button_StartRace(laps);
		}
	}

	// Token: 0x0600158D RID: 5517 RVA: 0x00072070 File Offset: 0x00070270
	[PunRPC]
	private void RequestRaceStart_RPC(int raceId, int laps, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RequestRaceStart_RPC");
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (laps != 1 && laps != 3 && laps != 5)
		{
			return;
		}
		if (raceId >= 0 && raceId < this.races.Length)
		{
			this.races[raceId].Host_RequestRaceStart(laps, info.Sender.ActorNumber);
		}
	}

	// Token: 0x0600158E RID: 5518 RVA: 0x000720C8 File Offset: 0x000702C8
	[PunRPC]
	private void RaceBeginCountdown_RPC(byte raceId, byte laps, double startTime, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RaceBeginCountdown_RPC");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (laps != 1 && laps != 3 && laps != 5)
		{
			return;
		}
		if (!double.IsFinite(startTime))
		{
			return;
		}
		if (startTime < PhotonNetwork.Time || startTime > PhotonNetwork.Time + 4.0)
		{
			return;
		}
		if (raceId >= 0 && (int)raceId < this.races.Length)
		{
			this.races[(int)raceId].BeginCountdown(startTime, (int)laps);
		}
	}

	// Token: 0x0600158F RID: 5519 RVA: 0x00072140 File Offset: 0x00070340
	[PunRPC]
	private void RaceLockInParticipants_RPC(byte raceId, int[] participantActorNumbers, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RaceLockInParticipants_RPC");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (participantActorNumbers.Length > 20)
		{
			return;
		}
		for (int i = 1; i < participantActorNumbers.Length; i++)
		{
			if (participantActorNumbers[i] <= participantActorNumbers[i - 1])
			{
				return;
			}
		}
		if (raceId >= 0 && (int)raceId < this.races.Length)
		{
			this.races[(int)raceId].LockInParticipants(participantActorNumbers, false);
		}
	}

	// Token: 0x06001590 RID: 5520 RVA: 0x000721A5 File Offset: 0x000703A5
	public void OnCheckpointPassed(int raceId, int checkpointIndex)
	{
		this.photonView.RPC("PassCheckpoint_RPC", RpcTarget.All, new object[]
		{
			(byte)raceId,
			(byte)checkpointIndex
		});
	}

	// Token: 0x06001591 RID: 5521 RVA: 0x000721D2 File Offset: 0x000703D2
	[PunRPC]
	private void PassCheckpoint_RPC(byte raceId, byte checkpointIndex, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "PassCheckpoint_RPC");
		if (raceId >= 0 && (int)raceId < this.races.Length)
		{
			this.races[(int)raceId].PassCheckpoint(info.Sender, (int)checkpointIndex, info.SentServerTime);
		}
	}

	// Token: 0x06001592 RID: 5522 RVA: 0x00072209 File Offset: 0x00070409
	[PunRPC]
	private void RaceEnded_RPC(byte raceId, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RaceEnded_RPC");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (raceId >= 0 && (int)raceId < this.races.Length)
		{
			this.races[(int)raceId].RaceEnded();
		}
	}

	// Token: 0x06001593 RID: 5523 RVA: 0x00072240 File Offset: 0x00070440
	void ITickSystemTick.Tick()
	{
		for (int i = 0; i < this.races.Length; i++)
		{
			this.races[i].Tick();
		}
	}

	// Token: 0x06001594 RID: 5524 RVA: 0x00072270 File Offset: 0x00070470
	public bool IsActorLockedIntoAnyRace(int actorNumber)
	{
		for (int i = 0; i < this.races.Length; i++)
		{
			if (this.races[i].IsActorLockedIntoRace(actorNumber))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04001A5A RID: 6746
	[SerializeField]
	private RacingManager.RaceSetup[] raceSetups;

	// Token: 0x04001A5B RID: 6747
	private const int MinPlayersInRace = 1;

	// Token: 0x04001A5C RID: 6748
	private const float ResultsDuration = 10f;

	// Token: 0x04001A5D RID: 6749
	private RacingManager.Race[] races;

	// Token: 0x0200036F RID: 879
	[Serializable]
	private struct RaceSetup
	{
		// Token: 0x04001A5E RID: 6750
		public BoxCollider startVolume;

		// Token: 0x04001A5F RID: 6751
		public int numCheckpoints;

		// Token: 0x04001A60 RID: 6752
		public float dqBaseDuration;

		// Token: 0x04001A61 RID: 6753
		public float dqInterval;
	}

	// Token: 0x02000370 RID: 880
	private struct RacerData
	{
		// Token: 0x04001A62 RID: 6754
		public int actorNumber;

		// Token: 0x04001A63 RID: 6755
		public string playerName;

		// Token: 0x04001A64 RID: 6756
		public int numCheckpointsPassed;

		// Token: 0x04001A65 RID: 6757
		public double latestCheckpointTime;

		// Token: 0x04001A66 RID: 6758
		public bool isDisqualified;
	}

	// Token: 0x02000371 RID: 881
	private class RacerComparer : IComparer<RacingManager.RacerData>
	{
		// Token: 0x06001596 RID: 5526 RVA: 0x000722AC File Offset: 0x000704AC
		public int Compare(RacingManager.RacerData a, RacingManager.RacerData b)
		{
			int num = a.isDisqualified.CompareTo(b.isDisqualified);
			if (num != 0)
			{
				return num;
			}
			int num2 = a.numCheckpointsPassed.CompareTo(b.numCheckpointsPassed);
			if (num2 != 0)
			{
				return -num2;
			}
			if (a.numCheckpointsPassed > 0)
			{
				return a.latestCheckpointTime.CompareTo(b.latestCheckpointTime);
			}
			return a.actorNumber.CompareTo(b.actorNumber);
		}

		// Token: 0x04001A67 RID: 6759
		public static RacingManager.RacerComparer instance = new RacingManager.RacerComparer();
	}

	// Token: 0x02000372 RID: 882
	public enum RacingState
	{
		// Token: 0x04001A69 RID: 6761
		Inactive,
		// Token: 0x04001A6A RID: 6762
		Countdown,
		// Token: 0x04001A6B RID: 6763
		InProgress,
		// Token: 0x04001A6C RID: 6764
		Results
	}

	// Token: 0x02000373 RID: 883
	private class Race
	{
		// Token: 0x06001599 RID: 5529 RVA: 0x00072324 File Offset: 0x00070524
		public Race(int raceIndex, RacingManager.RaceSetup setup, HashSet<int> actorsInAnyRace, PhotonView photonView)
		{
			this.raceIndex = raceIndex;
			this.numCheckpoints = setup.numCheckpoints;
			this.raceStartZone = setup.startVolume;
			this.dqBaseDuration = setup.dqBaseDuration;
			this.dqInterval = setup.dqInterval;
			this.photonView = photonView;
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x0600159A RID: 5530 RVA: 0x000723B6 File Offset: 0x000705B6
		// (set) Token: 0x0600159B RID: 5531 RVA: 0x000723BE File Offset: 0x000705BE
		public RacingManager.RacingState racingState { get; private set; }

		// Token: 0x0600159C RID: 5532 RVA: 0x000723C7 File Offset: 0x000705C7
		public void RegisterVisual(RaceVisual visual)
		{
			this.raceVisual = visual;
		}

		// Token: 0x0600159D RID: 5533 RVA: 0x000723D0 File Offset: 0x000705D0
		public void Clear()
		{
			this.hasLockedInParticipants = false;
			this.racers.Clear();
			this.playerLookup.Clear();
			this.racingState = RacingManager.RacingState.Inactive;
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x000723F8 File Offset: 0x000705F8
		public bool IsActorLockedIntoRace(int actorNumber)
		{
			if (this.racingState != RacingManager.RacingState.InProgress || !this.hasLockedInParticipants)
			{
				return false;
			}
			for (int i = 0; i < this.racers.Count; i++)
			{
				if (this.racers[i].actorNumber == actorNumber)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600159F RID: 5535 RVA: 0x00072448 File Offset: 0x00070648
		public void SendStateToNewPlayer(NetPlayer newPlayer)
		{
			switch (this.racingState)
			{
			case RacingManager.RacingState.Inactive:
			case RacingManager.RacingState.Results:
				return;
			case RacingManager.RacingState.Countdown:
				this.photonView.RPC("RaceBeginCountdown_RPC", RpcTarget.All, new object[]
				{
					(byte)this.raceIndex,
					(byte)this.numLapsSelected,
					this.raceStartTime
				});
				return;
			case RacingManager.RacingState.InProgress:
				return;
			default:
				return;
			}
		}

		// Token: 0x060015A0 RID: 5536 RVA: 0x000724B8 File Offset: 0x000706B8
		public void Tick()
		{
			if (Time.time >= this.nextTickTimestamp)
			{
				this.nextTickTimestamp = Time.time + this.TickWithNextDelay();
			}
		}

		// Token: 0x060015A1 RID: 5537 RVA: 0x000724DC File Offset: 0x000706DC
		public float TickWithNextDelay()
		{
			bool flag = this.raceVisual != null;
			if (flag)
			{
				this.raceVisual.ActivateStartingWall(this.racingState == RacingManager.RacingState.Countdown);
			}
			switch (this.racingState)
			{
			case RacingManager.RacingState.Inactive:
				if (flag)
				{
					this.RefreshStartingPlayerList();
				}
				return 1f;
			case RacingManager.RacingState.Countdown:
				if (this.raceStartTime > PhotonNetwork.Time)
				{
					if (flag)
					{
						this.RefreshStartingPlayerList();
						this.raceVisual.UpdateCountdown(Mathf.CeilToInt((float)(this.raceStartTime - PhotonNetwork.Time)));
					}
				}
				else
				{
					this.RaceCountdownEnds();
				}
				return 0.1f;
			case RacingManager.RacingState.InProgress:
				if (PhotonNetwork.IsMasterClient)
				{
					if (PhotonNetwork.Time > this.abortRaceAtTimestamp)
					{
						this.photonView.RPC("RaceEnded_RPC", RpcTarget.All, new object[] { (byte)this.raceIndex });
					}
					else
					{
						int num = 0;
						for (int i = 0; i < this.racers.Count; i++)
						{
							if (this.racers[i].numCheckpointsPassed < this.numCheckpointsToWin)
							{
								num++;
							}
						}
						if (num == 0)
						{
							this.photonView.RPC("RaceEnded_RPC", RpcTarget.All, new object[] { (byte)this.raceIndex });
						}
					}
				}
				return 1f;
			case RacingManager.RacingState.Results:
				if (Time.time >= this.resultsEndTimestamp)
				{
					if (flag)
					{
						this.raceVisual.OnRaceReset();
					}
					this.racingState = RacingManager.RacingState.Inactive;
				}
				return 1f;
			default:
				return 1f;
			}
		}

		// Token: 0x060015A2 RID: 5538 RVA: 0x00072650 File Offset: 0x00070850
		public void RaceEnded()
		{
			if (this.racingState != RacingManager.RacingState.InProgress)
			{
				return;
			}
			this.racingState = RacingManager.RacingState.Results;
			this.resultsEndTimestamp = Time.time + 10f;
			if (this.raceVisual != null)
			{
				this.raceVisual.OnRaceEnded();
			}
			for (int i = 0; i < this.racers.Count; i++)
			{
				RacingManager.RacerData racerData = this.racers[i];
				if (racerData.numCheckpointsPassed < this.numCheckpointsToWin)
				{
					racerData.isDisqualified = true;
					this.racers[i] = racerData;
				}
			}
			this.racers.Sort(RacingManager.RacerComparer.instance);
			this.OnRacerOrderChanged();
			for (int j = 0; j < this.racers.Count; j++)
			{
				if (this.racers[j].actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					VRRig.LocalRig.hoverboardVisual.SetRaceDisplay("");
					VRRig.LocalRig.hoverboardVisual.SetRaceLapsDisplay("");
					return;
				}
			}
		}

		// Token: 0x060015A3 RID: 5539 RVA: 0x00072750 File Offset: 0x00070950
		private void RefreshStartingPlayerList()
		{
			if (this.raceVisual != null && this.UpdateActorsInStartZone())
			{
				RacingManager.Race.stringBuilder.Clear();
				RacingManager.Race.stringBuilder.AppendLine("NEXT RACE LINEUP");
				for (int i = 0; i < this.actorsInStartZone.Count; i++)
				{
					RacingManager.Race.stringBuilder.Append("    ");
					RacingManager.Race.stringBuilder.AppendLine(this.playerNamesInStartZone[this.actorsInStartZone[i]]);
				}
				this.raceVisual.SetRaceStartScoreboardText(RacingManager.Race.stringBuilder.ToString(), "");
			}
		}

		// Token: 0x060015A4 RID: 5540 RVA: 0x000727F3 File Offset: 0x000709F3
		public void Button_StartRace(int laps)
		{
			if (this.racingState != RacingManager.RacingState.Inactive)
			{
				return;
			}
			this.photonView.RPC("RequestRaceStart_RPC", RpcTarget.MasterClient, new object[] { this.raceIndex, laps });
		}

		// Token: 0x060015A5 RID: 5541 RVA: 0x0007282C File Offset: 0x00070A2C
		public void Host_RequestRaceStart(int laps, int requestedByActorNumber)
		{
			if (this.racingState != RacingManager.RacingState.Inactive)
			{
				return;
			}
			this.UpdateActorsInStartZone();
			if (this.actorsInStartZone.Contains(requestedByActorNumber))
			{
				this.photonView.RPC("RaceBeginCountdown_RPC", RpcTarget.All, new object[]
				{
					(byte)this.raceIndex,
					(byte)laps,
					PhotonNetwork.Time + 4.0
				});
			}
		}

		// Token: 0x060015A6 RID: 5542 RVA: 0x000728A0 File Offset: 0x00070AA0
		public void BeginCountdown(double startTime, int laps)
		{
			if (this.racingState != RacingManager.RacingState.Inactive)
			{
				return;
			}
			this.racingState = RacingManager.RacingState.Countdown;
			this.raceStartTime = startTime;
			this.abortRaceAtTimestamp = startTime + (double)this.dqBaseDuration;
			this.numLapsSelected = laps;
			this.numCheckpointsToWin = this.numCheckpoints * laps + 1;
			this.hasLockedInParticipants = false;
			if (this.raceVisual != null)
			{
				this.raceVisual.OnCountdownStart(laps, (float)(startTime - PhotonNetwork.Time));
			}
		}

		// Token: 0x060015A7 RID: 5543 RVA: 0x00072914 File Offset: 0x00070B14
		public void RaceCountdownEnds()
		{
			if (this.racingState != RacingManager.RacingState.Countdown)
			{
				return;
			}
			this.racingState = RacingManager.RacingState.InProgress;
			if (this.raceVisual != null)
			{
				this.raceVisual.OnRaceStart();
			}
			this.UpdateActorsInStartZone();
			if (PhotonNetwork.IsMasterClient)
			{
				this.photonView.RPC("RaceLockInParticipants_RPC", RpcTarget.All, new object[]
				{
					(byte)this.raceIndex,
					this.actorsInStartZone.ToArray()
				});
				return;
			}
			if (this.actorsInStartZone.Count >= 1)
			{
				this.LockInParticipants(this.actorsInStartZone.ToArray(), true);
			}
		}

		// Token: 0x060015A8 RID: 5544 RVA: 0x000729B0 File Offset: 0x00070BB0
		public void LockInParticipants(int[] participantActorNumbers, bool isProvisional = false)
		{
			if (this.hasLockedInParticipants)
			{
				return;
			}
			if (!isProvisional && participantActorNumbers.Length < 1)
			{
				this.racingState = RacingManager.RacingState.Inactive;
				return;
			}
			this.racers.Clear();
			if (participantActorNumbers.Length != 0)
			{
				foreach (VRRig vrrig in VRRigCache.ActiveRigs)
				{
					int actorNumber = vrrig.OwningNetPlayer.ActorNumber;
					if (participantActorNumbers.BinarySearch(actorNumber) >= 0 && !RacingManager.instance.IsActorLockedIntoAnyRace(actorNumber))
					{
						this.racers.Add(new RacingManager.RacerData
						{
							actorNumber = actorNumber,
							playerName = vrrig.OwningNetPlayer.SanitizedNickName,
							latestCheckpointTime = this.raceStartTime
						});
					}
				}
			}
			if (!isProvisional)
			{
				if (this.racers.Count < 1)
				{
					this.racingState = RacingManager.RacingState.Inactive;
					return;
				}
				this.hasLockedInParticipants = true;
			}
			this.racers.Sort(RacingManager.RacerComparer.instance);
			this.OnRacerOrderChanged();
		}

		// Token: 0x060015A9 RID: 5545 RVA: 0x00072AB8 File Offset: 0x00070CB8
		public void PassCheckpoint(Player player, int checkpointIndex, double time)
		{
			if (this.racingState == RacingManager.RacingState.Inactive)
			{
				return;
			}
			if (time < this.raceStartTime || time < PhotonNetwork.Time - 5.0 || time > PhotonNetwork.Time + 0.10000000149011612)
			{
				return;
			}
			if (this.abortRaceAtTimestamp < time + (double)this.dqInterval)
			{
				this.abortRaceAtTimestamp = time + (double)this.dqInterval;
			}
			RacingManager.RacerData racerData = default(RacingManager.RacerData);
			int i = 0;
			while (i < this.racers.Count)
			{
				racerData = this.racers[i];
				if (racerData.actorNumber == player.ActorNumber)
				{
					if (racerData.numCheckpointsPassed >= this.numCheckpointsToWin || racerData.isDisqualified)
					{
						return;
					}
					if (checkpointIndex != racerData.numCheckpointsPassed % this.numCheckpoints)
					{
						return;
					}
					RigContainer rigContainer;
					if (this.raceVisual != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer) && !this.raceVisual.IsPlayerNearCheckpoint(rigContainer.Rig, checkpointIndex))
					{
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			if (racerData.actorNumber != player.ActorNumber)
			{
				return;
			}
			racerData.numCheckpointsPassed++;
			racerData.latestCheckpointTime = time;
			this.racers[i] = racerData;
			if (racerData.numCheckpointsPassed >= this.numCheckpointsToWin || (i > 0 && RacingManager.RacerComparer.instance.Compare(this.racers[i - 1], racerData) > 0))
			{
				this.racers.Sort(RacingManager.RacerComparer.instance);
				this.OnRacerOrderChanged();
			}
			if (player.IsLocal)
			{
				if (checkpointIndex == this.numCheckpoints - 1)
				{
					int num = racerData.numCheckpointsPassed / this.numCheckpoints + 1;
					if (num > this.numLapsSelected)
					{
						this.raceVisual.ShowFinishLineText("FINISH");
						this.raceVisual.EnableRaceEndSound();
						return;
					}
					if (num == this.numLapsSelected)
					{
						this.raceVisual.ShowFinishLineText("FINAL LAP");
						return;
					}
					this.raceVisual.ShowFinishLineText("NEXT LAP");
					return;
				}
				else if (checkpointIndex == 0)
				{
					int num2 = racerData.numCheckpointsPassed / this.numCheckpoints + 1;
					if (num2 > this.numLapsSelected)
					{
						VRRig.LocalRig.hoverboardVisual.SetRaceLapsDisplay("");
						return;
					}
					VRRig.LocalRig.hoverboardVisual.SetRaceLapsDisplay(string.Format("LAP {0}/{1}", num2, this.numLapsSelected));
				}
			}
		}

		// Token: 0x060015AA RID: 5546 RVA: 0x00072CFC File Offset: 0x00070EFC
		private void OnRacerOrderChanged()
		{
			if (this.raceVisual != null)
			{
				RacingManager.Race.stringBuilder.Clear();
				RacingManager.Race.timesStringBuilder.Clear();
				RacingManager.Race.timesStringBuilder.AppendLine("");
				bool flag = false;
				switch (this.racingState)
				{
				case RacingManager.RacingState.Inactive:
					return;
				case RacingManager.RacingState.Countdown:
					RacingManager.Race.stringBuilder.AppendLine("STARTING LINEUP");
					flag = true;
					break;
				case RacingManager.RacingState.InProgress:
					RacingManager.Race.stringBuilder.AppendLine("RACE LEADERBOARD");
					break;
				case RacingManager.RacingState.Results:
					RacingManager.Race.stringBuilder.AppendLine("RACE RESULTS");
					break;
				}
				for (int i = 0; i < this.racers.Count; i++)
				{
					RacingManager.RacerData racerData = this.racers[i];
					if (racerData.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
					{
						VRRig.LocalRig.hoverboardVisual.SetRaceDisplay(racerData.isDisqualified ? "DQ" : (i + 1).ToString());
					}
					string text = (racerData.isDisqualified ? "DQ. " : (flag ? "    " : ((i + 1).ToString() + ". ")));
					RacingManager.Race.stringBuilder.Append(text);
					if (text.Length <= 3)
					{
						RacingManager.Race.stringBuilder.Append(" ");
					}
					RacingManager.Race.stringBuilder.AppendLine(racerData.playerName);
					if (racerData.isDisqualified)
					{
						RacingManager.Race.timesStringBuilder.AppendLine("--.--");
					}
					else if (racerData.numCheckpointsPassed < this.numCheckpointsToWin)
					{
						RacingManager.Race.timesStringBuilder.AppendLine("");
					}
					else
					{
						RacingManager.Race.timesStringBuilder.AppendLine(string.Format("{0:0.00}", racerData.latestCheckpointTime - this.raceStartTime));
					}
				}
				string text2 = RacingManager.Race.stringBuilder.ToString();
				string text3 = RacingManager.Race.timesStringBuilder.ToString();
				this.raceVisual.SetScoreboardText(text2, text3);
				this.raceVisual.SetRaceStartScoreboardText(text2, text3);
			}
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x00072F08 File Offset: 0x00071108
		private bool UpdateActorsInStartZone()
		{
			if (Time.time < this.nextStartZoneUpdateTimestamp)
			{
				return false;
			}
			this.nextStartZoneUpdateTimestamp = Time.time + 0.1f;
			List<int> list = this.actorsInStartZone2;
			List<int> list2 = this.actorsInStartZone;
			this.actorsInStartZone = list;
			this.actorsInStartZone2 = list2;
			this.actorsInStartZone.Clear();
			this.playerNamesInStartZone.Clear();
			int num = Physics.OverlapBoxNonAlloc(this.raceStartZone.transform.position, this.raceStartZone.size / 2f, RacingManager.Race.overlapColliders, this.raceStartZone.transform.rotation, RacingManager.Race.playerLayerMask);
			num = Mathf.Min(num, RacingManager.Race.overlapColliders.Length);
			for (int i = 0; i < num; i++)
			{
				Collider collider = RacingManager.Race.overlapColliders[i];
				if (!(collider == null))
				{
					VRRig component = collider.attachedRigidbody.gameObject.GetComponent<VRRig>();
					int count = this.actorsInStartZone.Count;
					if (!(component == null))
					{
						if (component.isLocal)
						{
							if (NetworkSystem.Instance.LocalPlayer == null)
							{
								RacingManager.Race.overlapColliders[i] = null;
								goto IL_01E2;
							}
							if (RacingManager.instance.IsActorLockedIntoAnyRace(NetworkSystem.Instance.LocalPlayer.ActorNumber))
							{
								goto IL_01E2;
							}
							this.actorsInStartZone.AddSortedUnique(NetworkSystem.Instance.LocalPlayer.ActorNumber);
							if (this.actorsInStartZone.Count > count)
							{
								this.playerNamesInStartZone.Add(NetworkSystem.Instance.LocalPlayer.ActorNumber, component.playerNameVisible);
							}
						}
						else
						{
							if (RacingManager.instance.IsActorLockedIntoAnyRace(component.OwningNetPlayer.ActorNumber))
							{
								goto IL_01E2;
							}
							this.actorsInStartZone.AddSortedUnique(component.OwningNetPlayer.ActorNumber);
							if (this.actorsInStartZone.Count > count)
							{
								this.playerNamesInStartZone.Add(component.OwningNetPlayer.ActorNumber, component.playerNameVisible);
							}
						}
						RacingManager.Race.overlapColliders[i] = null;
					}
				}
				IL_01E2:;
			}
			if (this.actorsInStartZone2.Count != this.actorsInStartZone.Count)
			{
				return true;
			}
			for (int j = 0; j < this.actorsInStartZone.Count; j++)
			{
				if (this.actorsInStartZone[j] != this.actorsInStartZone2[j])
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04001A6D RID: 6765
		private int raceIndex;

		// Token: 0x04001A6E RID: 6766
		private int numCheckpoints;

		// Token: 0x04001A6F RID: 6767
		private float dqBaseDuration;

		// Token: 0x04001A70 RID: 6768
		private float dqInterval;

		// Token: 0x04001A71 RID: 6769
		private BoxCollider raceStartZone;

		// Token: 0x04001A72 RID: 6770
		private PhotonView photonView;

		// Token: 0x04001A73 RID: 6771
		private List<RacingManager.RacerData> racers = new List<RacingManager.RacerData>(20);

		// Token: 0x04001A74 RID: 6772
		private Dictionary<NetPlayer, int> playerLookup = new Dictionary<NetPlayer, int>();

		// Token: 0x04001A75 RID: 6773
		private List<int> actorsInStartZone = new List<int>();

		// Token: 0x04001A76 RID: 6774
		private List<int> actorsInStartZone2 = new List<int>();

		// Token: 0x04001A77 RID: 6775
		private Dictionary<int, string> playerNamesInStartZone = new Dictionary<int, string>();

		// Token: 0x04001A78 RID: 6776
		private int numLapsSelected = 1;

		// Token: 0x04001A7A RID: 6778
		private double raceStartTime;

		// Token: 0x04001A7B RID: 6779
		private double abortRaceAtTimestamp;

		// Token: 0x04001A7C RID: 6780
		private float resultsEndTimestamp;

		// Token: 0x04001A7D RID: 6781
		private bool isInstanceLoaded;

		// Token: 0x04001A7E RID: 6782
		private int numCheckpointsToWin;

		// Token: 0x04001A7F RID: 6783
		private RaceVisual raceVisual;

		// Token: 0x04001A80 RID: 6784
		private bool hasLockedInParticipants;

		// Token: 0x04001A81 RID: 6785
		private float nextTickTimestamp;

		// Token: 0x04001A82 RID: 6786
		private static StringBuilder stringBuilder = new StringBuilder();

		// Token: 0x04001A83 RID: 6787
		private static StringBuilder timesStringBuilder = new StringBuilder();

		// Token: 0x04001A84 RID: 6788
		private static Collider[] overlapColliders = new Collider[20];

		// Token: 0x04001A85 RID: 6789
		private static int playerLayerMask = UnityLayer.GorillaBodyCollider.ToLayerMask() | UnityLayer.GorillaTagCollider.ToLayerMask();

		// Token: 0x04001A86 RID: 6790
		private float nextStartZoneUpdateTimestamp;
	}
}
