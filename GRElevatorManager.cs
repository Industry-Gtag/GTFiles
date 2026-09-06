using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x0200077C RID: 1916
[NetworkBehaviourWeaved(0)]
public class GRElevatorManager : NetworkComponent, ITickSystemTick
{
	// Token: 0x17000488 RID: 1160
	// (get) Token: 0x06003088 RID: 12424 RVA: 0x00106E46 File Offset: 0x00105046
	public bool InPrivateRoom
	{
		get
		{
			return NetworkSystem.Instance.SessionIsPrivate;
		}
	}

	// Token: 0x17000489 RID: 1161
	// (get) Token: 0x06003089 RID: 12425 RVA: 0x00106E52 File Offset: 0x00105052
	// (set) Token: 0x0600308A RID: 12426 RVA: 0x00106E5A File Offset: 0x0010505A
	public bool TickRunning { get; set; }

	// Token: 0x0600308B RID: 12427 RVA: 0x00106E64 File Offset: 0x00105064
	protected override void Awake()
	{
		base.Awake();
		if (GRElevatorManager._instance != null)
		{
			Debug.LogError("Multiple elevator managers! This should never happen!");
			return;
		}
		GRElevatorManager._instance = this;
		this.currentState = GRElevatorManager.ElevatorSystemState.InLocation;
		this.currentLocation = GRElevatorManager.ElevatorLocation.Mall;
		this.destination = GRElevatorManager.ElevatorLocation.Mall;
		this.elevatorByLocation = new Dictionary<GRElevatorManager.ElevatorLocation, GRElevator>();
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.elevatorByLocation[this.allElevators[i].location] = this.allElevators[i];
		}
		this.actorIds = new List<int>();
		this.mainStagingShuttle.specificFloor = -1;
		this.mainDrillShuttle.specificFloor = 0;
		this.allShuttles = new List<GRShuttle>(64);
		for (int j = 0; j < this.shuttleGroups.Count; j++)
		{
			GRElevatorManager.GRShuttleGroup grshuttleGroup = this.shuttleGroups[j];
			for (int k = 0; k < grshuttleGroup.ghostReactorStagingShuttles.Count; k++)
			{
				this.allShuttles.Add(grshuttleGroup.ghostReactorStagingShuttles[k]);
				grshuttleGroup.ghostReactorStagingShuttles[k].SetLocation(grshuttleGroup.location);
			}
		}
		this.allShuttles.Add(this.mainStagingShuttle);
		this.allShuttles.Add(this.mainDrillShuttle);
		for (int l = 0; l < this.allShuttles.Count; l++)
		{
			this.allShuttles[l].Init(l);
		}
	}

	// Token: 0x0600308C RID: 12428 RVA: 0x00106FDC File Offset: 0x001051DC
	protected override void Start()
	{
		base.Start();
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnLeftRoom;
		NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerAdded;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerRemoved;
	}

	// Token: 0x0600308D RID: 12429 RVA: 0x00107054 File Offset: 0x00105254
	protected void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnLeftRoom;
		NetworkSystem.Instance.OnPlayerJoined -= this.OnPlayerAdded;
		NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerRemoved;
	}

	// Token: 0x0600308E RID: 12430 RVA: 0x001070CA File Offset: 0x001052CA
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		TickSystem<object>.AddTickCallback(this);
		this.DestinationVideoPlayer.loopPointReached += this.DisableVideoScreens;
	}

	// Token: 0x0600308F RID: 12431 RVA: 0x001070F5 File Offset: 0x001052F5
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
		this.DestinationVideoPlayer.loopPointReached -= this.DisableVideoScreens;
	}

	// Token: 0x06003090 RID: 12432 RVA: 0x00107120 File Offset: 0x00105320
	private void DisableVideoScreens(VideoPlayer source)
	{
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.allElevators[i].videoDisplay.SetActive(false);
		}
	}

	// Token: 0x06003091 RID: 12433 RVA: 0x0010715C File Offset: 0x0010535C
	public void Tick()
	{
		if (!this.cosmeticsInitialized)
		{
			this.CheckInitializationState();
			return;
		}
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.allElevators[i].PhysicalElevatorUpdate();
		}
		this.ProcessElevatorSystemState();
		if (this.justTeleported)
		{
			this.justTeleported = false;
			GTPlayer.Instance.disableMovement = false;
		}
	}

	// Token: 0x06003092 RID: 12434 RVA: 0x001071BF File Offset: 0x001053BF
	private void CheckInitializationState()
	{
		this.cosmeticsInitialized = true;
		if (GRElevatorManager.InControlOfElevator())
		{
			this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.InLocation, GRElevatorManager.ElevatorLocation.Mall);
		}
	}

	// Token: 0x06003093 RID: 12435 RVA: 0x001071D8 File Offset: 0x001053D8
	public void ProcessElevatorSystemState()
	{
		switch (this.currentState)
		{
		case GRElevatorManager.ElevatorSystemState.Dormant:
			break;
		case GRElevatorManager.ElevatorSystemState.InLocation:
			if (this.currentLocation == this.destination && this.waitForZoneLoadFallbackTimer >= 0f && this.elevatorByLocation[this.currentLocation].DoorIsClosing())
			{
				this.waitForZoneLoadFallbackTimer += Time.deltaTime;
				if (this.waitForZoneLoadFallbackTimer >= this.waitForZoneLoadFallbackMaxTime)
				{
					this.OnReachedDestination();
				}
			}
			break;
		case GRElevatorManager.ElevatorSystemState.DestinationPressed:
		{
			if (!GRElevatorManager.InControlOfElevator())
			{
				return;
			}
			double time = this.GetTime();
			if (this.elevatorByLocation[this.currentLocation].DoorsFullyClosed() && time >= this.doorsFullyClosedTime + (double)this.doorsFullyClosedDelay)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.WaitingToTeleport, GRElevatorManager.ElevatorLocation.None);
				return;
			}
			if (time >= this.destinationButtonLastPressedTime + (double)this.destinationButtonlastPressedDelay && !this.elevatorByLocation[this.currentLocation].DoorIsClosing())
			{
				this.destinationButtonLastPressedTime = time;
				this.CloseAllElevators();
				return;
			}
			break;
		}
		case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
			if (!GRElevatorManager.InControlOfElevator())
			{
				return;
			}
			if (this.GetTime() >= this.doorsFullyClosedTime + (double)this.doorsFullyClosedDelay && !this.waitingForRemoteTeleport)
			{
				this.ActivateElevating();
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06003094 RID: 12436 RVA: 0x00107308 File Offset: 0x00105508
	public void ActivateElevating()
	{
		if (PhotonNetwork.InRoom)
		{
			this.photonView.RPC("RemoteActivateTeleport", RpcTarget.All, new object[]
			{
				(int)this.currentLocation,
				(int)this.destination,
				GRElevatorManager.LowestActorNumberInElevator()
			});
			return;
		}
		this.ActivateTeleport(this.currentLocation, this.destination, -1, this.GetTime());
	}

	// Token: 0x06003095 RID: 12437 RVA: 0x00107378 File Offset: 0x00105578
	public void LeadElevatorJoin()
	{
		GRElevatorManager.LeadElevatorJoin(this.elevatorByLocation[this.currentLocation].friendCollider, this.elevatorByLocation[this.destination].friendCollider, this.elevatorByLocation[this.destination].joinTrigger);
	}

	// Token: 0x06003096 RID: 12438 RVA: 0x001073CC File Offset: 0x001055CC
	public static void SetupFriendGroup(GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider destinationFriendCollider)
	{
		PhotonNetworkController.Instance.FriendIDList = new List<string>(sourceFriendCollider.playerIDsCurrentlyTouching);
		PhotonNetworkController.Instance.FriendIDList.AddRange(destinationFriendCollider.playerIDsCurrentlyTouching);
		PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
		PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
	}

	// Token: 0x06003097 RID: 12439 RVA: 0x00107474 File Offset: 0x00105674
	public static void LeadElevatorJoin(GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider destinationFriendCollider, GorillaNetworkJoinTrigger destinationJoinTrigger)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			sourceFriendCollider.RefreshPlayersWithinBounds();
			destinationFriendCollider.RefreshPlayersWithinBounds();
			GRElevatorManager.SetupFriendGroup(sourceFriendCollider, destinationFriendCollider);
			RoomSystem.SendElevatorFollowCommand(PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr, sourceFriendCollider, destinationFriendCollider);
			PhotonNetwork.SendAllOutgoingCommands();
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(destinationJoinTrigger, JoinType.JoinWithElevator, null, false);
		}
		GRElevatorManager.JoinPublicRoom();
	}

	// Token: 0x06003098 RID: 12440 RVA: 0x001074DC File Offset: 0x001056DC
	public static void LeadShuttleJoin(GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider destinationFriendCollider, GorillaNetworkJoinTrigger destinationJoinTrigger, int targetLevel)
	{
		sourceFriendCollider.RefreshPlayersWithinBounds();
		destinationFriendCollider.RefreshPlayersWithinBounds();
		GorillaComputer.instance.friendJoinCollider = destinationFriendCollider;
		GorillaComputer.instance.UpdateScreen();
		if (NetworkSystem.Instance.InRoom)
		{
			GRElevatorManager.SetupFriendGroup(sourceFriendCollider, destinationFriendCollider);
			RoomSystem.SendShuttleFollowCommand(PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr, sourceFriendCollider, destinationFriendCollider);
			PhotonNetwork.SendAllOutgoingCommands();
			List<ValueTuple<string, string>> list = null;
			if (targetLevel >= 0)
			{
				int joinDepthSectionFromLevel = GhostReactor.GetJoinDepthSectionFromLevel(targetLevel);
				list = new List<ValueTuple<string, string>>
				{
					new ValueTuple<string, string>("ghostReactorDepth", joinDepthSectionFromLevel.ToString())
				};
			}
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(destinationJoinTrigger, JoinType.JoinWithElevator, list, false);
		}
		PhotonNetworkController.Instance.AttemptToJoinPublicRoom(destinationJoinTrigger, JoinType.Solo, null, false);
	}

	// Token: 0x06003099 RID: 12441 RVA: 0x00107590 File Offset: 0x00105790
	public void UpdateElevatorState(GRElevatorManager.ElevatorSystemState newState, GRElevatorManager.ElevatorLocation location = GRElevatorManager.ElevatorLocation.None)
	{
		switch (this.currentState)
		{
		case GRElevatorManager.ElevatorSystemState.Dormant:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.InLocation:
				this.elevatorByLocation[this.currentLocation].PlayDing();
				this.OpenElevator(this.destination);
				break;
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				this.maxDoorClosingTime = this.GetTime();
				this.destinationButtonLastPressedTime = this.GetTime();
				this.doorsFullyClosedTime = this.GetTime();
				if (this.destination != this.currentLocation)
				{
					this.destination = location;
					this.PlayDestinationVideo(this.destination);
				}
				this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			}
			break;
		case GRElevatorManager.ElevatorSystemState.InLocation:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.Dormant:
				this.CloseAllElevators();
				break;
			case GRElevatorManager.ElevatorSystemState.InLocation:
				if (location == this.currentLocation)
				{
					this.OpenElevator(this.currentLocation);
				}
				else
				{
					this.CloseAllElevators();
				}
				break;
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
				if (location != this.currentLocation)
				{
					this.destination = location;
					this.destinationButtonLastPressedTime = this.GetTime();
					this.maxDoorClosingTime = this.GetTime();
					this.PlayDestinationVideo(this.destination);
				}
				else
				{
					if (this.elevatorByLocation[this.destination].DoorIsClosing())
					{
						this.OpenElevator(this.currentLocation);
					}
					newState = this.currentState;
				}
				if (this.currentLocation != GRElevatorManager.ElevatorLocation.None)
				{
					this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
				}
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				if (this.currentLocation != GRElevatorManager.ElevatorLocation.None)
				{
					this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
				}
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			}
			break;
		case GRElevatorManager.ElevatorSystemState.DestinationPressed:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.Dormant:
				this.CloseAllElevators();
				break;
			case GRElevatorManager.ElevatorSystemState.InLocation:
				this.OpenElevator(location);
				this.elevatorByLocation[this.currentLocation].PlayDing();
				break;
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
				if (location != this.currentLocation)
				{
					this.destination = location;
					this.PlayDestinationVideo(this.destination);
				}
				break;
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				this.doorsFullyClosedTime = this.GetTime();
				if (this.currentLocation != GRElevatorManager.ElevatorLocation.None)
				{
					this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
					this.elevatorByLocation[this.currentLocation].PlayElevatorMusic(0f);
				}
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			}
			break;
		case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.Dormant:
				this.CloseAllElevators();
				this.elevatorByLocation[this.currentLocation].PlayElevatorStopped();
				this.elevatorByLocation[this.destination].PlayElevatorStopped();
				break;
			case GRElevatorManager.ElevatorSystemState.InLocation:
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.OnSceneLoadsCompleted = (Action)Delegate.Combine(instance.OnSceneLoadsCompleted, new Action(this.OnReachedDestination));
				this.waitForZoneLoadFallbackTimer = 0.01f;
				this.elevatorByLocation[this.currentLocation].PlayElevatorStopped();
				this.currentLocation = location;
				break;
			}
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				if (location != this.currentLocation)
				{
					this.destination = location;
					this.PlayDestinationVideo(this.destination);
				}
				else
				{
					this.OpenElevator(location);
					newState = GRElevatorManager.ElevatorSystemState.InLocation;
				}
				break;
			}
			break;
		}
		this.currentState = newState;
		this.UpdateUI();
	}

	// Token: 0x0600309A RID: 12442 RVA: 0x0010793C File Offset: 0x00105B3C
	private void PlayDestinationVideo(GRElevatorManager.ElevatorLocation destination)
	{
		VideoClip clipForDestination = this.getClipForDestination(destination);
		if (this.DestinationVideoPlayer.isPlaying && this.DestinationVideoPlayer.clip != clipForDestination)
		{
			this.DestinationVideoPlayer.Stop();
			this.DisableVideoScreens(this.DestinationVideoPlayer);
		}
		if (clipForDestination != null && this.currentLocation != GRElevatorManager.ElevatorLocation.None)
		{
			this.DestinationVideoPlayer.clip = clipForDestination;
			this.DestinationVideoPlayer.SetTargetAudioSource(0, this.DestinationVideoPlayerAudioSource);
			this.DestinationVideoPlayer.Play();
			this.DestinationVideoPlayerAudioSource.transform.position = this.elevatorByLocation[this.currentLocation].videoAudio.transform.position;
			this.elevatorByLocation[this.currentLocation].videoDisplay.SetActive(true);
		}
	}

	// Token: 0x0600309B RID: 12443 RVA: 0x00107A10 File Offset: 0x00105C10
	private VideoClip getClipForDestination(GRElevatorManager.ElevatorLocation destination)
	{
		for (int i = 0; i < this.DestinationVideos.Length; i++)
		{
			if (this.DestinationVideos[i].Destination == destination)
			{
				return this.DestinationVideos[i].VideoClip;
			}
		}
		return null;
	}

	// Token: 0x0600309C RID: 12444 RVA: 0x00107A58 File Offset: 0x00105C58
	public void UpdateUI()
	{
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.allElevators[i].outerText.text = "ELEVATOR LOCATION:\n" + this.currentLocation.ToString().ToUpper();
			GRElevatorManager.ElevatorSystemState elevatorSystemState = this.currentState;
			if (elevatorSystemState > GRElevatorManager.ElevatorSystemState.InLocation)
			{
				if (elevatorSystemState - GRElevatorManager.ElevatorSystemState.DestinationPressed <= 1)
				{
					if (this.destination != this.currentLocation)
					{
						this.allElevators[i].innerText.text = "NEXT STOP:\n" + this.destination.ToString().ToUpper();
					}
					else
					{
						this.allElevators[i].innerText.text = "CHOOSE DESTINATION";
					}
				}
			}
			else
			{
				this.allElevators[i].innerText.text = "CHOOSE DESTINATION";
			}
		}
	}

	// Token: 0x0600309D RID: 12445 RVA: 0x00107B48 File Offset: 0x00105D48
	public static void RegisterElevator(GRElevator elevator)
	{
		if (GRElevatorManager._instance == null)
		{
			return;
		}
		GRElevatorManager._instance.elevatorByLocation[elevator.location] = elevator;
	}

	// Token: 0x0600309E RID: 12446 RVA: 0x00107B6E File Offset: 0x00105D6E
	public static void DeregisterElevator(GRElevator elevator)
	{
		if (GRElevatorManager._instance == null)
		{
			return;
		}
		GRElevatorManager._instance.elevatorByLocation[elevator.location] = null;
	}

	// Token: 0x0600309F RID: 12447 RVA: 0x00107B94 File Offset: 0x00105D94
	public static void ElevatorButtonPressed(GRElevator.ButtonType type, GRElevatorManager.ElevatorLocation location)
	{
		if (GRElevatorManager._instance != null)
		{
			GRElevatorManager._instance.ElevatorButtonPressedInternal(type, location);
			if (!GRElevatorManager._instance.IsMine && NetworkSystem.Instance.InRoom)
			{
				GRElevatorManager._instance.photonView.RPC("RemoteElevatorButtonPress", RpcTarget.MasterClient, new object[]
				{
					(int)type,
					(int)location
				});
			}
		}
	}

	// Token: 0x060030A0 RID: 12448 RVA: 0x00107C00 File Offset: 0x00105E00
	private void ElevatorButtonPressedInternal(GRElevator.ButtonType type, GRElevatorManager.ElevatorLocation location)
	{
		GRElevator grelevator;
		if (!this.elevatorByLocation.TryGetValue(location, out grelevator) || grelevator == null)
		{
			Debug.LogWarning(string.Format("[GRElevatorManager] No elevator registered for location '{0}'. Elevator may not be enabled yet or is missing from allElevators.", location), this);
			return;
		}
		grelevator.PressButtonVisuals(type);
		grelevator.PlayButtonPress();
		if (base.IsMine)
		{
			this.ProcessElevatorButtonPress(type, location);
		}
	}

	// Token: 0x060030A1 RID: 12449 RVA: 0x00107C5C File Offset: 0x00105E5C
	public void ProcessElevatorButtonPress(GRElevator.ButtonType type, GRElevatorManager.ElevatorLocation location)
	{
		switch (type)
		{
		case GRElevator.ButtonType.Mall:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.Mall);
				return;
			}
			break;
		case GRElevator.ButtonType.City:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.City);
				return;
			}
			break;
		case GRElevator.ButtonType.GhostReactor:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.GhostReactor);
				return;
			}
			break;
		case GRElevator.ButtonType.Open:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				if (this.currentState == GRElevatorManager.ElevatorSystemState.DestinationPressed)
				{
					if (this.GetTime() >= this.maxDoorClosingTime + (double)this.doorMaxClosingDelay)
					{
						break;
					}
					this.destinationButtonLastPressedTime = this.GetTime();
					this.doorsFullyClosedTime = this.GetTime();
				}
				this.OpenElevator(location);
				return;
			}
			break;
		case GRElevator.ButtonType.Close:
			this.CloseAllElevators();
			break;
		case GRElevator.ButtonType.Summon:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport && this.currentState != GRElevatorManager.ElevatorSystemState.DestinationPressed)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, location);
				return;
			}
			break;
		case GRElevator.ButtonType.MonkeBlocks:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.MonkeBlocks);
				return;
			}
			break;
		case GRElevator.ButtonType.VIMExperience1:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.VIMExperience1);
				return;
			}
			break;
		case GRElevator.ButtonType.VIMExperience2:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.VIMExperience2);
				return;
			}
			break;
		case GRElevator.ButtonType.VIMExperience3:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.VIMExperience3);
				return;
			}
			break;
		case GRElevator.ButtonType.VIMExperience4:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.VIMExperience4);
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060030A2 RID: 12450 RVA: 0x00107DAC File Offset: 0x00105FAC
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.doorsFullyClosedTime);
		stream.SendNext(this.destinationButtonLastPressedTime);
		stream.SendNext(this.maxDoorClosingTime);
		stream.SendNext((int)this.currentLocation);
		stream.SendNext((int)this.destination);
		stream.SendNext((int)this.currentState);
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			stream.SendNext((int)this.allElevators[i].state);
		}
		for (int j = 0; j < this.allShuttles.Count; j++)
		{
			stream.SendNext((byte)this.allShuttles[j].GetState());
			bool flag = this.allShuttles[j].specificDestinationShuttle == null;
			NetPlayer owner = this.allShuttles[j].GetOwner();
			int num = ((!flag || owner == null) ? (-1) : owner.ActorNumber);
			stream.SendNext(num);
		}
	}

	// Token: 0x060030A3 RID: 12451 RVA: 0x00107EC8 File Offset: 0x001060C8
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		double num = (double)stream.ReceiveNext();
		if (!double.IsNaN(num) && !double.IsInfinity(num))
		{
			this.doorsFullyClosedTime = num;
		}
		num = (double)stream.ReceiveNext();
		if (!double.IsNaN(num) && !double.IsInfinity(num))
		{
			this.destinationButtonLastPressedTime = num;
		}
		num = (double)stream.ReceiveNext();
		if (!double.IsNaN(num) && !double.IsInfinity(num))
		{
			this.maxDoorClosingTime = num;
		}
		GRElevatorManager.ElevatorLocation elevatorLocation = this.currentLocation;
		int num2 = (int)stream.ReceiveNext();
		if (num2 >= 0 && num2 <= 9)
		{
			this.currentLocation = (GRElevatorManager.ElevatorLocation)num2;
		}
		GRElevatorManager.ElevatorLocation elevatorLocation2 = this.destination;
		num2 = (int)stream.ReceiveNext();
		if (num2 >= 0 && num2 <= 9)
		{
			this.destination = (GRElevatorManager.ElevatorLocation)num2;
		}
		num2 = (int)stream.ReceiveNext();
		if (num2 >= 0 && num2 < 5)
		{
			GRElevatorManager.ElevatorSystemState elevatorSystemState = (GRElevatorManager.ElevatorSystemState)num2;
			if (elevatorSystemState != this.currentState && elevatorSystemState == GRElevatorManager.ElevatorSystemState.DestinationPressed)
			{
				this.PlayDestinationVideo(this.destination);
			}
			this.currentState = (GRElevatorManager.ElevatorSystemState)num2;
		}
		this.UpdateUI();
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			num2 = (int)stream.ReceiveNext();
			if (num2 >= 0 && num2 < 8)
			{
				this.allElevators[i].UpdateRemoteState((GRElevator.ElevatorState)num2);
			}
		}
		for (int j = 0; j < this.allShuttles.Count; j++)
		{
			byte b = (byte)stream.ReceiveNext();
			int num3 = (int)stream.ReceiveNext();
			if (b >= 0 && b < 7)
			{
				this.allShuttles[j].SetState((GRShuttleState)b, false);
			}
			if (this.allShuttles[j].specificDestinationShuttle == null && num3 != -1)
			{
				NetPlayer netPlayer = NetPlayer.Get(num3);
				this.allShuttles[j].SetOwner(netPlayer);
			}
		}
	}

	// Token: 0x060030A4 RID: 12452 RVA: 0x0010809C File Offset: 0x0010629C
	[PunRPC]
	public void RemoteElevatorButtonPress(int elevatorButtonPressed, int elevatorLocation, PhotonMessageInfo info)
	{
		if (!base.IsMine || this.m_RpcSpamChecks.IsSpamming(GRElevatorManager.RPC.RemoteElevatorButtonPress))
		{
			return;
		}
		if (elevatorLocation < 0 || elevatorLocation >= 9)
		{
			return;
		}
		if (elevatorButtonPressed < 0 || elevatorButtonPressed >= 13)
		{
			return;
		}
		this.ElevatorButtonPressedInternal((GRElevator.ButtonType)elevatorButtonPressed, (GRElevatorManager.ElevatorLocation)elevatorLocation);
	}

	// Token: 0x060030A5 RID: 12453 RVA: 0x001080D4 File Offset: 0x001062D4
	[PunRPC]
	public void RemoteActivateTeleport(int elevatorStartLocation, int elevatorDestinationLocation, int lowestActorNumber, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient || this.m_RpcSpamChecks.IsSpamming(GRElevatorManager.RPC.RemoteActivateTeleport))
		{
			return;
		}
		if (elevatorStartLocation < 0 || elevatorStartLocation >= 9 || elevatorDestinationLocation < 0 || elevatorDestinationLocation >= 9)
		{
			return;
		}
		if (!this.waitingForRemoteTeleport)
		{
			base.StartCoroutine(this.TeleportDelay((GRElevatorManager.ElevatorLocation)elevatorStartLocation, (GRElevatorManager.ElevatorLocation)elevatorDestinationLocation, lowestActorNumber, info.SentServerTime));
		}
	}

	// Token: 0x060030A6 RID: 12454 RVA: 0x00108130 File Offset: 0x00106330
	private IEnumerator TeleportDelay(GRElevatorManager.ElevatorLocation start, GRElevatorManager.ElevatorLocation destination, int lowestActorNumber, double sentServerTime)
	{
		this.timeLastTeleported = (double)Time.time;
		this.waitingForRemoteTeleport = true;
		this.lastTeleportSource = start;
		yield return new WaitForSeconds((float)(PhotonNetwork.Time - (sentServerTime + 0.75)));
		this.RefreshTeleportingPlayersJoinTime();
		yield return new WaitForSeconds(0.25f);
		this.waitingForRemoteTeleport = false;
		this.ActivateTeleport(start, destination, lowestActorNumber, sentServerTime);
		yield break;
	}

	// Token: 0x060030A7 RID: 12455 RVA: 0x0010815C File Offset: 0x0010635C
	public void ActivateTeleport(GRElevatorManager.ElevatorLocation start, GRElevatorManager.ElevatorLocation destination, int lowestActorNumber, double photonServerTime)
	{
		GRElevator grelevator = this.elevatorByLocation[start];
		GRElevator grelevator2 = this.elevatorByLocation[destination];
		if (grelevator == null || grelevator2 == null)
		{
			return;
		}
		grelevator.friendCollider.RefreshPlayersWithinBounds();
		if (!PhotonNetwork.InRoom)
		{
			this.RefreshTeleportingPlayersJoinTime();
		}
		if (!grelevator.friendCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId))
		{
			this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.InLocation, destination);
			return;
		}
		this.elevatorByLocation[destination].collidersAndVisuals.SetActive(true);
		if (this.DestinationVideoPlayer.isPlaying)
		{
			this.elevatorByLocation[destination].videoDisplay.SetActive(true);
			this.DestinationVideoPlayerAudioSource.transform.position = this.elevatorByLocation[destination].videoAudio.transform.position;
		}
		GTPlayer instance = GTPlayer.Instance;
		Vector3 vector = grelevator.transform.InverseTransformPoint(instance.transform.position);
		Vector3 vector2 = grelevator2.transform.TransformPoint(vector);
		Quaternion quaternion = Quaternion.Inverse(grelevator.transform.rotation) * instance.transform.rotation;
		Quaternion quaternion2 = grelevator2.transform.rotation * quaternion;
		if (vector.sqrMagnitude > 1E-08f)
		{
			vector2 -= grelevator2.transform.TransformDirection(vector.normalized) * 0.001f;
		}
		instance.TeleportTo(vector2, quaternion2, false, false);
		this.justTeleported = true;
		instance.disableMovement = true;
		GorillaComputer.instance.allowedMapsToJoin = this.elevatorByLocation[destination].joinTrigger.myCollider.myAllowedMapsToJoin;
		this.lastTeleportSource = start;
		this.lastLowestActorNr = lowestActorNumber;
		if (!this.InPrivateRoom && lowestActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.LeadElevatorJoin();
		}
		this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.InLocation, destination);
		grelevator2.PlayElevatorMusic(grelevator.musicAudio.time);
	}

	// Token: 0x060030A8 RID: 12456 RVA: 0x0010835C File Offset: 0x0010655C
	public void CloseAllElevators()
	{
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			if (!this.allElevators[i].DoorIsClosing())
			{
				this.allElevators[i].UpdateLocalState(GRElevator.ElevatorState.DoorBeginClosing);
			}
		}
	}

	// Token: 0x060030A9 RID: 12457 RVA: 0x001083A4 File Offset: 0x001065A4
	public void OpenElevator(GRElevatorManager.ElevatorLocation location)
	{
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.allElevators[i].UpdateLocalState((this.allElevators[i].location == location) ? GRElevator.ElevatorState.DoorBeginOpening : GRElevator.ElevatorState.DoorBeginClosing);
		}
	}

	// Token: 0x060030AA RID: 12458 RVA: 0x001083F0 File Offset: 0x001065F0
	public double GetTime()
	{
		double num = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		if (this.doorsFullyClosedTime > num || this.destinationButtonLastPressedTime > num || this.maxDoorClosingTime > num || num - this.doorsFullyClosedTime > 10.0 || num - this.destinationButtonLastPressedTime > 10.0 || num - this.maxDoorClosingTime > 20.0)
		{
			this.doorsFullyClosedTime = num;
			this.destinationButtonLastPressedTime = num;
			this.maxDoorClosingTime = num;
		}
		return num;
	}

	// Token: 0x060030AB RID: 12459 RVA: 0x0010847C File Offset: 0x0010667C
	public static bool ValidElevatorNetworking(int actorNr)
	{
		if (GRElevatorManager._instance == null)
		{
			return false;
		}
		if (RoomSystem.WasRoomPrivate)
		{
			return false;
		}
		if (actorNr == GRElevatorManager._instance.lastLowestActorNr)
		{
			return true;
		}
		if (GRElevatorManager._instance.lastTeleportSource == GRElevatorManager.ElevatorLocation.None)
		{
			return false;
		}
		GorillaFriendCollider friendCollider = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.destination].friendCollider;
		GorillaFriendCollider friendCollider2 = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.lastTeleportSource].friendCollider;
		if ((double)Time.time < GRElevatorManager._instance.timeLastTeleported + 3.0)
		{
			friendCollider.RefreshPlayersWithinBounds();
			friendCollider2.RefreshPlayersWithinBounds();
		}
		NetPlayer netPlayer = NetPlayer.Get(actorNr);
		return netPlayer != null && (friendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) || friendCollider2.playerIDsCurrentlyTouching.Contains(netPlayer.UserId));
	}

	// Token: 0x060030AC RID: 12460 RVA: 0x00108558 File Offset: 0x00106758
	public static bool ValidShuttleNetworking(int actorNr)
	{
		if (GRElevatorManager._instance == null)
		{
			return false;
		}
		if (RoomSystem.WasRoomPrivate)
		{
			return false;
		}
		GRPlayer grplayer = GRPlayer.Get(actorNr);
		if (grplayer == null)
		{
			return false;
		}
		GRShuttle shuttle = GRElevatorManager.GetShuttle(grplayer.shuttleData.currShuttleId);
		GRShuttle grshuttle = GRElevatorManager.GetShuttle(grplayer.shuttleData.targetShuttleId);
		if (shuttle == null)
		{
			return false;
		}
		if (grshuttle == null)
		{
			grshuttle = GRElevatorManager.GetShuttle(GRShuttle.CalcTargetShuttleId(grplayer.shuttleData.currShuttleId, grplayer.shuttleData.ownerUserId));
			if (grshuttle == null)
			{
				return false;
			}
		}
		NetPlayer netPlayer = NetPlayer.Get(actorNr);
		if (netPlayer == null)
		{
			return false;
		}
		if (netPlayer == shuttle.GetOwner())
		{
			return true;
		}
		GorillaFriendCollider friendCollider = grshuttle.friendCollider;
		GorillaFriendCollider friendCollider2 = shuttle.friendCollider;
		friendCollider.RefreshPlayersWithinBounds();
		friendCollider2.RefreshPlayersWithinBounds();
		return friendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) || friendCollider2.playerIDsCurrentlyTouching.Contains(netPlayer.UserId);
	}

	// Token: 0x060030AD RID: 12461 RVA: 0x0010864C File Offset: 0x0010684C
	public static bool IsPlayerInShuttle(int actorNr, GRShuttle currShuttle, GRShuttle targetShuttle)
	{
		if (GRElevatorManager._instance == null)
		{
			return false;
		}
		NetPlayer netPlayer = NetPlayer.Get(actorNr);
		if (netPlayer == null)
		{
			return false;
		}
		bool flag = false;
		if (currShuttle != null)
		{
			GorillaFriendCollider friendCollider = currShuttle.friendCollider;
			if (friendCollider != null)
			{
				friendCollider.RefreshPlayersWithinBounds();
			}
			flag = friendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId);
		}
		bool flag2 = false;
		if (targetShuttle != null)
		{
			GorillaFriendCollider friendCollider2 = targetShuttle.friendCollider;
			if (friendCollider2 != null)
			{
				friendCollider2.RefreshPlayersWithinBounds();
			}
			friendCollider2.playerIDsCurrentlyTouching.Contains(netPlayer.UserId);
		}
		return flag || flag2;
	}

	// Token: 0x060030AE RID: 12462 RVA: 0x001086E4 File Offset: 0x001068E4
	public static int LowestActorNumberInElevator()
	{
		GorillaFriendCollider friendCollider = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.currentLocation].friendCollider;
		GorillaFriendCollider friendCollider2 = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.destination].friendCollider;
		friendCollider.RefreshPlayersWithinBounds();
		friendCollider2.RefreshPlayersWithinBounds();
		int num = int.MaxValue;
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			if (num > allNetPlayers[i].ActorNumber && (friendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId) || friendCollider2.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId)))
			{
				num = allNetPlayers[i].ActorNumber;
			}
		}
		return num;
	}

	// Token: 0x060030AF RID: 12463 RVA: 0x001087A4 File Offset: 0x001069A4
	public static int LowestActorNumberInElevator(GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider destinationFriendCollider)
	{
		sourceFriendCollider.RefreshPlayersWithinBounds();
		destinationFriendCollider.RefreshPlayersWithinBounds();
		int num = int.MaxValue;
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			if (num > allNetPlayers[i].ActorNumber && (sourceFriendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId) || destinationFriendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId)))
			{
				num = allNetPlayers[i].ActorNumber;
			}
		}
		return num;
	}

	// Token: 0x060030B0 RID: 12464 RVA: 0x0010881C File Offset: 0x00106A1C
	private void RefreshTeleportingPlayersJoinTime()
	{
		GorillaFriendCollider friendCollider = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.currentLocation].friendCollider;
		this.actorIds.Clear();
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			RigContainer rigContainer;
			if (friendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId) && VRRigCache.Instance.TryGetVrrig(allNetPlayers[i], out rigContainer))
			{
				rigContainer.Rig.ResetTimeSpawned();
			}
		}
	}

	// Token: 0x060030B1 RID: 12465 RVA: 0x0010889D File Offset: 0x00106A9D
	public static bool InControlOfElevator()
	{
		return !NetworkSystem.Instance.InRoom || GRElevatorManager._instance.IsMine;
	}

	// Token: 0x060030B2 RID: 12466 RVA: 0x001088B7 File Offset: 0x00106AB7
	public static void JoinPublicRoom()
	{
		PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.destination].joinTrigger, JoinType.Solo, null, false);
	}

	// Token: 0x060030B3 RID: 12467 RVA: 0x001088E8 File Offset: 0x00106AE8
	public void OnReachedDestination()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.OnSceneLoadsCompleted = (Action)Delegate.Remove(instance.OnSceneLoadsCompleted, new Action(this.OnReachedDestination));
		this.elevatorByLocation[this.destination].PlayElevatorStopped();
		if (this.currentLocation == this.destination)
		{
			this.OpenElevator(this.currentLocation);
			this.elevatorByLocation[this.currentLocation].PlayDing();
		}
		this.waitForZoneLoadFallbackTimer = -1f;
	}

	// Token: 0x060030B4 RID: 12468 RVA: 0x0010896C File Offset: 0x00106B6C
	public static GRShuttle GetShuttle(int shuttleId)
	{
		if (GRElevatorManager._instance == null)
		{
			return null;
		}
		return GRElevatorManager._instance.GetShuttleById(shuttleId);
	}

	// Token: 0x060030B5 RID: 12469 RVA: 0x00108988 File Offset: 0x00106B88
	public void InitShuttles(GhostReactor reactor)
	{
		for (int i = 0; i < this.allShuttles.Count; i++)
		{
			this.allShuttles[i].SetReactor(reactor);
		}
	}

	// Token: 0x060030B6 RID: 12470 RVA: 0x001089C0 File Offset: 0x00106BC0
	public GRShuttle GetPlayerShuttle(GRShuttleGroupLoc shuttleGroupLoc, int shuttleIndex)
	{
		int i = 0;
		while (i < this.shuttleGroups.Count)
		{
			if (this.shuttleGroups[i].location == shuttleGroupLoc)
			{
				if (shuttleIndex < 0 || shuttleIndex >= this.shuttleGroups[i].ghostReactorStagingShuttles.Count)
				{
					Debug.LogErrorFormat("Invalid Shuttle Index {0} of {1}", new object[]
					{
						shuttleIndex,
						this.shuttleGroups[i].ghostReactorStagingShuttles.Count
					});
					return null;
				}
				return this.shuttleGroups[i].ghostReactorStagingShuttles[shuttleIndex];
			}
			else
			{
				i++;
			}
		}
		return null;
	}

	// Token: 0x060030B7 RID: 12471 RVA: 0x00108A6C File Offset: 0x00106C6C
	public GRShuttle GetDrillShuttleForPlayer(int actorNumber)
	{
		return this.GetShuttleForPlayer(actorNumber, GRShuttleGroupLoc.Drill);
	}

	// Token: 0x060030B8 RID: 12472 RVA: 0x00108A76 File Offset: 0x00106C76
	public GRShuttle GetStagingShuttleForPlayer(int actorNumber)
	{
		return this.GetShuttleForPlayer(actorNumber, GRShuttleGroupLoc.Staging);
	}

	// Token: 0x060030B9 RID: 12473 RVA: 0x00108A80 File Offset: 0x00106C80
	public GRShuttle GetShuttleForPlayer(int actorNumber, GRShuttleGroupLoc shuttleGroupLoc)
	{
		for (int i = 0; i < this.shuttleGroups.Count; i++)
		{
			if (this.shuttleGroups[i].location == shuttleGroupLoc)
			{
				for (int j = 0; j < this.shuttleGroups[i].ghostReactorStagingShuttles.Count; j++)
				{
					GRShuttle grshuttle = this.shuttleGroups[i].ghostReactorStagingShuttles[j];
					if (!(grshuttle == null))
					{
						NetPlayer owner = grshuttle.GetOwner();
						if (owner != null && owner.ActorNumber == actorNumber)
						{
							return grshuttle;
						}
					}
				}
			}
		}
		return null;
	}

	// Token: 0x060030BA RID: 12474 RVA: 0x00108B10 File Offset: 0x00106D10
	public GRShuttle GetShuttleById(int shuttleId)
	{
		for (int i = 0; i < this.allShuttles.Count; i++)
		{
			if (this.allShuttles[i].shuttleId == shuttleId)
			{
				return this.allShuttles[i];
			}
		}
		return null;
	}

	// Token: 0x060030BB RID: 12475 RVA: 0x00108B58 File Offset: 0x00106D58
	private int AddPlayer(NetPlayer netPlayer)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return -1;
		}
		int num = -1;
		List<GRShuttle> ghostReactorStagingShuttles = this.shuttleGroups[0].ghostReactorStagingShuttles;
		for (int i = 0; i < ghostReactorStagingShuttles.Count; i++)
		{
			if (ghostReactorStagingShuttles[i].GetOwner() == null)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return -1;
		}
		for (int j = 0; j < this.shuttleGroups.Count; j++)
		{
			this.shuttleGroups[j].ghostReactorStagingShuttles[num].SetOwner(netPlayer);
		}
		return num;
	}

	// Token: 0x060030BC RID: 12476 RVA: 0x00108BE0 File Offset: 0x00106DE0
	private void RemovePlayer(NetPlayer netPlayer)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		int num = -1;
		List<GRShuttle> ghostReactorStagingShuttles = this.shuttleGroups[0].ghostReactorStagingShuttles;
		for (int i = 0; i < ghostReactorStagingShuttles.Count; i++)
		{
			if (ghostReactorStagingShuttles[i].GetOwner() == netPlayer)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return;
		}
		for (int j = 0; j < this.shuttleGroups.Count; j++)
		{
			this.shuttleGroups[j].ghostReactorStagingShuttles[num].SetOwner(null);
		}
	}

	// Token: 0x060030BD RID: 12477 RVA: 0x00108C68 File Offset: 0x00106E68
	public void OnLeftRoom()
	{
		for (int i = 0; i < this.shuttleGroups.Count; i++)
		{
			for (int j = 0; j < this.shuttleGroups[i].ghostReactorStagingShuttles.Count; j++)
			{
				GRShuttle grshuttle = this.shuttleGroups[i].ghostReactorStagingShuttles[j];
				if (!(grshuttle == null))
				{
					grshuttle.SetOwner(null);
				}
			}
		}
	}

	// Token: 0x060030BE RID: 12478 RVA: 0x00108CD4 File Offset: 0x00106ED4
	public void OnPlayerAdded(NetPlayer player)
	{
		if (!PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
		{
			return;
		}
		this.AddPlayer(player);
	}

	// Token: 0x060030BF RID: 12479 RVA: 0x00108CF3 File Offset: 0x00106EF3
	public void OnPlayerRemoved(NetPlayer player)
	{
		if (!PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
		{
			return;
		}
		this.RemovePlayer(player);
	}

	// Token: 0x060030C0 RID: 12480 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void WriteDataFusion()
	{
	}

	// Token: 0x060030C1 RID: 12481 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void ReadDataFusion()
	{
	}

	// Token: 0x060030C3 RID: 12483 RVA: 0x00002E6F File Offset: 0x0000106F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060030C4 RID: 12484 RVA: 0x00002E7B File Offset: 0x0000107B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04003E33 RID: 15923
	public PhotonView photonView;

	// Token: 0x04003E34 RID: 15924
	public static GRElevatorManager _instance;

	// Token: 0x04003E35 RID: 15925
	public Dictionary<GRElevatorManager.ElevatorLocation, GRElevator> elevatorByLocation;

	// Token: 0x04003E36 RID: 15926
	public List<GRElevator> allElevators;

	// Token: 0x04003E37 RID: 15927
	[SerializeField]
	private GRElevatorManager.ElevatorLocation destination;

	// Token: 0x04003E38 RID: 15928
	[SerializeField]
	private GRElevatorManager.ElevatorLocation currentLocation;

	// Token: 0x04003E39 RID: 15929
	private GRElevatorManager.ElevatorLocation lastTeleportSource = GRElevatorManager.ElevatorLocation.None;

	// Token: 0x04003E3A RID: 15930
	public GRElevatorManager.ElevatorSystemState currentState;

	// Token: 0x04003E3B RID: 15931
	private double timeLastTeleported;

	// Token: 0x04003E3C RID: 15932
	private bool cosmeticsInitialized;

	// Token: 0x04003E3D RID: 15933
	[SerializeField]
	private List<GRElevatorManager.GRShuttleGroup> shuttleGroups;

	// Token: 0x04003E3E RID: 15934
	public GRShuttle mainStagingShuttle;

	// Token: 0x04003E3F RID: 15935
	public GRShuttle mainDrillShuttle;

	// Token: 0x04003E40 RID: 15936
	private List<GRShuttle> allShuttles;

	// Token: 0x04003E41 RID: 15937
	public float destinationButtonlastPressedDelay = 3f;

	// Token: 0x04003E42 RID: 15938
	public float doorsFullyClosedDelay = 3f;

	// Token: 0x04003E43 RID: 15939
	public float doorMaxClosingDelay = 12f;

	// Token: 0x04003E44 RID: 15940
	public double destinationButtonLastPressedTime;

	// Token: 0x04003E45 RID: 15941
	public double doorsFullyClosedTime;

	// Token: 0x04003E46 RID: 15942
	public double maxDoorClosingTime;

	// Token: 0x04003E47 RID: 15943
	private List<int> actorIds;

	// Token: 0x04003E48 RID: 15944
	public CallLimitersList<CallLimiter, GRElevatorManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GRElevatorManager.RPC>();

	// Token: 0x04003E49 RID: 15945
	private bool justTeleported;

	// Token: 0x04003E4A RID: 15946
	private bool waitingForRemoteTeleport;

	// Token: 0x04003E4B RID: 15947
	private int lastLowestActorNr;

	// Token: 0x04003E4C RID: 15948
	private float waitForZoneLoadFallbackTimer;

	// Token: 0x04003E4D RID: 15949
	public float waitForZoneLoadFallbackMaxTime = 5f;

	// Token: 0x04003E4F RID: 15951
	[SerializeField]
	private GRElevatorManager.DestinationVideo[] DestinationVideos;

	// Token: 0x04003E50 RID: 15952
	[SerializeField]
	private VideoPlayer DestinationVideoPlayer;

	// Token: 0x04003E51 RID: 15953
	[SerializeField]
	private AudioSource DestinationVideoPlayerAudioSource;

	// Token: 0x0200077D RID: 1917
	[Serializable]
	public class GRShuttleGroup
	{
		// Token: 0x04003E52 RID: 15954
		public GRShuttleGroupLoc location;

		// Token: 0x04003E53 RID: 15955
		public List<GRShuttle> ghostReactorStagingShuttles;
	}

	// Token: 0x0200077E RID: 1918
	public enum ElevatorSystemState
	{
		// Token: 0x04003E55 RID: 15957
		Dormant,
		// Token: 0x04003E56 RID: 15958
		InLocation,
		// Token: 0x04003E57 RID: 15959
		DestinationPressed,
		// Token: 0x04003E58 RID: 15960
		WaitingToTeleport,
		// Token: 0x04003E59 RID: 15961
		Teleporting,
		// Token: 0x04003E5A RID: 15962
		None
	}

	// Token: 0x0200077F RID: 1919
	public enum RPC
	{
		// Token: 0x04003E5C RID: 15964
		RemoteElevatorButtonPress,
		// Token: 0x04003E5D RID: 15965
		RemoteActivateTeleport
	}

	// Token: 0x02000780 RID: 1920
	public enum ElevatorLocation
	{
		// Token: 0x04003E5F RID: 15967
		Mall,
		// Token: 0x04003E60 RID: 15968
		City,
		// Token: 0x04003E61 RID: 15969
		GhostReactor,
		// Token: 0x04003E62 RID: 15970
		MonkeBlocks,
		// Token: 0x04003E63 RID: 15971
		VIMExperience1,
		// Token: 0x04003E64 RID: 15972
		VIMExperience2,
		// Token: 0x04003E65 RID: 15973
		VIMExperience3,
		// Token: 0x04003E66 RID: 15974
		VIMExperience4,
		// Token: 0x04003E67 RID: 15975
		GhostEntrance,
		// Token: 0x04003E68 RID: 15976
		None
	}

	// Token: 0x02000781 RID: 1921
	[Serializable]
	public struct DestinationVideo
	{
		// Token: 0x04003E69 RID: 15977
		public GRElevatorManager.ElevatorLocation Destination;

		// Token: 0x04003E6A RID: 15978
		public VideoClip VideoClip;
	}
}
