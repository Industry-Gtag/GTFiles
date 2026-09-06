using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Audio;
using Photon.Realtime;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200043F RID: 1087
public class NetworkSystemFusion : NetworkSystem
{
	// Token: 0x17000294 RID: 660
	// (get) Token: 0x060019EC RID: 6636 RVA: 0x0009088A File Offset: 0x0008EA8A
	// (set) Token: 0x060019ED RID: 6637 RVA: 0x00090892 File Offset: 0x0008EA92
	public NetworkRunner runner { get; private set; }

	// Token: 0x17000295 RID: 661
	// (get) Token: 0x060019EE RID: 6638 RVA: 0x0009089B File Offset: 0x0008EA9B
	public override bool IsOnline
	{
		get
		{
			return this.runner != null && !this.runner.IsSinglePlayer;
		}
	}

	// Token: 0x17000296 RID: 662
	// (get) Token: 0x060019EF RID: 6639 RVA: 0x000908BB File Offset: 0x0008EABB
	public override bool InRoom
	{
		get
		{
			return this.runner != null && this.runner.State != NetworkRunner.States.Shutdown && !this.runner.IsSinglePlayer && this.runner.IsConnectedToServer;
		}
	}

	// Token: 0x17000297 RID: 663
	// (get) Token: 0x060019F0 RID: 6640 RVA: 0x000908F3 File Offset: 0x0008EAF3
	public override string RoomName
	{
		get
		{
			SessionInfo sessionInfo = this.runner.SessionInfo;
			if (sessionInfo == null)
			{
				return null;
			}
			return sessionInfo.Name;
		}
	}

	// Token: 0x060019F1 RID: 6641 RVA: 0x0009090C File Offset: 0x0008EB0C
	public override string RoomStringStripped()
	{
		SessionInfo sessionInfo = this.runner.SessionInfo;
		NetworkSystem.reusableSB.Clear();
		NetworkSystem.reusableSB.AppendFormat("Room: '{0}' ", (sessionInfo.Name.Length < 20) ? sessionInfo.Name : sessionInfo.Name.Remove(20));
		NetworkSystem.reusableSB.AppendFormat("{0},{1} {3}/{2} players.", new object[]
		{
			sessionInfo.IsVisible ? "visible" : "hidden",
			sessionInfo.IsOpen ? "open" : "closed",
			sessionInfo.MaxPlayers,
			sessionInfo.PlayerCount
		});
		NetworkSystem.reusableSB.Append("\ncustomProps: {");
		NetworkSystem.reusableSB.AppendFormat("joinedGameMode={0}, ", (RoomSystem.RoomGameMode.Length < 50) ? RoomSystem.RoomGameMode : RoomSystem.RoomGameMode.Remove(50));
		IDictionary properties = sessionInfo.Properties;
		Debug.Log(RoomSystem.RoomGameMode.ToString());
		if (properties.Contains("gameMode"))
		{
			object obj = properties["gameMode"];
			if (obj == null)
			{
				NetworkSystem.reusableSB.AppendFormat("gameMode=null}", Array.Empty<object>());
			}
			else
			{
				string text = obj as string;
				if (text != null)
				{
					NetworkSystem.reusableSB.AppendFormat("gameMode={0}", (text.Length < 50) ? text : text.Remove(50));
				}
			}
		}
		NetworkSystem.reusableSB.Append("}");
		Debug.Log(NetworkSystem.reusableSB.ToString());
		return NetworkSystem.reusableSB.ToString();
	}

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x060019F2 RID: 6642 RVA: 0x00090AA8 File Offset: 0x0008ECA8
	public override string GameModeString
	{
		get
		{
			SessionProperty sessionProperty;
			this.runner.SessionInfo.Properties.TryGetValue("gameMode", out sessionProperty);
			if (sessionProperty != null)
			{
				return (string)sessionProperty.PropertyValue;
			}
			return null;
		}
	}

	// Token: 0x17000299 RID: 665
	// (get) Token: 0x060019F3 RID: 6643 RVA: 0x00090AE2 File Offset: 0x0008ECE2
	public override string CurrentRegion
	{
		get
		{
			SessionInfo sessionInfo = this.runner.SessionInfo;
			if (sessionInfo == null)
			{
				return null;
			}
			return sessionInfo.Region;
		}
	}

	// Token: 0x1700029A RID: 666
	// (get) Token: 0x060019F4 RID: 6644 RVA: 0x00090AFC File Offset: 0x0008ECFC
	public override bool SessionIsPrivate
	{
		get
		{
			NetworkRunner runner = this.runner;
			bool? flag;
			if (runner == null)
			{
				flag = null;
			}
			else
			{
				SessionInfo sessionInfo = runner.SessionInfo;
				flag = ((sessionInfo != null) ? new bool?(!sessionInfo.IsVisible) : null);
			}
			bool? flag2 = flag;
			return flag2.GetValueOrDefault();
		}
	}

	// Token: 0x1700029B RID: 667
	// (get) Token: 0x060019F5 RID: 6645 RVA: 0x00090B48 File Offset: 0x0008ED48
	public override bool SessionIsSubscription
	{
		get
		{
			NetworkRunner runner = this.runner;
			if (runner == null)
			{
				return false;
			}
			SessionInfo sessionInfo = runner.SessionInfo;
			int? num = ((sessionInfo != null) ? new int?(sessionInfo.MaxPlayers) : null);
			int num2 = 10;
			return (num.GetValueOrDefault() > num2) & (num != null);
		}
	}

	// Token: 0x1700029C RID: 668
	// (get) Token: 0x060019F6 RID: 6646 RVA: 0x00090B98 File Offset: 0x0008ED98
	public override int LocalPlayerID
	{
		get
		{
			return this.runner.LocalPlayer.PlayerId;
		}
	}

	// Token: 0x1700029D RID: 669
	// (get) Token: 0x060019F7 RID: 6647 RVA: 0x00090BB8 File Offset: 0x0008EDB8
	public override string CurrentPhotonBackend
	{
		get
		{
			return "Fusion";
		}
	}

	// Token: 0x1700029E RID: 670
	// (get) Token: 0x060019F8 RID: 6648 RVA: 0x00090BBF File Offset: 0x0008EDBF
	public override double SimTime
	{
		get
		{
			return (double)this.runner.SimulationTime;
		}
	}

	// Token: 0x1700029F RID: 671
	// (get) Token: 0x060019F9 RID: 6649 RVA: 0x00090BCD File Offset: 0x0008EDCD
	public override float SimDeltaTime
	{
		get
		{
			return this.runner.DeltaTime;
		}
	}

	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x060019FA RID: 6650 RVA: 0x00090BDA File Offset: 0x0008EDDA
	public override int SimTick
	{
		get
		{
			return this.runner.Tick.Raw;
		}
	}

	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x060019FB RID: 6651 RVA: 0x00090BEC File Offset: 0x0008EDEC
	public override int TickRate
	{
		get
		{
			return this.runner.TickRate;
		}
	}

	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x060019FC RID: 6652 RVA: 0x00090BDA File Offset: 0x0008EDDA
	public override int ServerTimestamp
	{
		get
		{
			return this.runner.Tick.Raw;
		}
	}

	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x060019FD RID: 6653 RVA: 0x00090BF9 File Offset: 0x0008EDF9
	public override int RoomPlayerCount
	{
		get
		{
			return this.runner.SessionInfo.PlayerCount;
		}
	}

	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x060019FE RID: 6654 RVA: 0x00090C0B File Offset: 0x0008EE0B
	public override VoiceConnection VoiceConnection
	{
		get
		{
			return this.FusionVoice;
		}
	}

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x060019FF RID: 6655 RVA: 0x00090C13 File Offset: 0x0008EE13
	public override bool IsMasterClient
	{
		get
		{
			NetworkRunner runner = this.runner;
			return runner == null || runner.IsSharedModeMasterClient;
		}
	}

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x06001A00 RID: 6656 RVA: 0x00090C28 File Offset: 0x0008EE28
	public override NetPlayer MasterClient
	{
		get
		{
			if (this.runner != null && this.runner.IsSharedModeMasterClient)
			{
				return base.GetPlayer(this.runner.LocalPlayer);
			}
			if (!(global::GorillaGameModes.GameMode.ActiveNetworkHandler != null))
			{
				return null;
			}
			return base.GetPlayer(global::GorillaGameModes.GameMode.ActiveNetworkHandler.Object.StateAuthority);
		}
	}

	// Token: 0x06001A01 RID: 6657 RVA: 0x00090C88 File Offset: 0x0008EE88
	public override async void Initialise()
	{
		base.Initialise();
		this.myObjectProvider = new CustomObjectProvider();
		base.netState = NetSystemState.Initialization;
		this.internalState = NetworkSystemFusion.InternalState.Idle;
		await this.ReturnToSinglePlayer();
		this.AwaitAuth();
		this.CreateRegionCrawler();
		GameModeSerializer.FusionGameModeOwnerChanged = (Action<NetPlayer>)Delegate.Combine(GameModeSerializer.FusionGameModeOwnerChanged, new Action<NetPlayer>(base.OnMasterClientSwitchedCallback));
		this.OnMasterClientSwitchedEvent += new Action<NetPlayer>(this.OnMasterSwitch);
		base.netState = NetSystemState.Idle;
		this.playerPool = new ObjectPool<FusionNetPlayer>(20);
		base.UpdatePlayers();
	}

	// Token: 0x06001A02 RID: 6658 RVA: 0x00090CC0 File Offset: 0x0008EEC0
	private void CreateRegionCrawler()
	{
		GameObject gameObject = new GameObject("[Network Crawler]");
		gameObject.transform.SetParent(base.transform);
		this.regionCrawler = gameObject.AddComponent<FusionRegionCrawler>();
	}

	// Token: 0x06001A03 RID: 6659 RVA: 0x00090CF8 File Offset: 0x0008EEF8
	private async Task AwaitAuth()
	{
		this.internalState = NetworkSystemFusion.InternalState.AwaitingAuth;
		while (this.cachedPlayfabAuth == null)
		{
			await Task.Yield();
		}
		this.internalState = NetworkSystemFusion.InternalState.Idle;
		base.netState = NetSystemState.Idle;
	}

	// Token: 0x06001A04 RID: 6660 RVA: 0x00090D3B File Offset: 0x0008EF3B
	public override void FinishAuthenticating()
	{
		if (this.cachedPlayfabAuth != null)
		{
			Debug.Log("AUTHED");
			return;
		}
		Debug.LogError("Authentication Failed");
	}

	// Token: 0x06001A05 RID: 6661 RVA: 0x00090D5C File Offset: 0x0008EF5C
	public override async Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		NetJoinResult netJoinResult;
		if (this.isWrongVersion)
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else if (base.netState != NetSystemState.Idle && base.netState != NetSystemState.InGame)
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else if (this.InRoom && roomName == this.RoomName)
		{
			netJoinResult = NetJoinResult.AlreadyInRoom;
		}
		else
		{
			base.netState = NetSystemState.Connecting;
			Utils.Log("Connecting to:" + (string.IsNullOrEmpty(roomName) ? "random room" : roomName));
			NetJoinResult netJoinResult2;
			if (!string.IsNullOrEmpty(roomName))
			{
				Task<NetJoinResult> makeOrJoinTask = this.MakeOrJoinRoom(roomName, opts);
				await makeOrJoinTask;
				netJoinResult2 = makeOrJoinTask.Result;
				makeOrJoinTask = null;
			}
			else
			{
				Task<NetJoinResult> makeOrJoinTask = this.JoinRandomPublicRoom(opts);
				await makeOrJoinTask;
				netJoinResult2 = makeOrJoinTask.Result;
				makeOrJoinTask = null;
			}
			if (netJoinResult2 == NetJoinResult.Failed_Full || netJoinResult2 == NetJoinResult.Failed_Other)
			{
				this.ResetSystem();
				netJoinResult = netJoinResult2;
			}
			else if (netJoinResult2 == NetJoinResult.AlreadyInRoom)
			{
				base.netState = NetSystemState.InGame;
				netJoinResult = netJoinResult2;
			}
			else
			{
				base.UpdatePlayers();
				base.netState = NetSystemState.InGame;
				Utils.Log("Connect to room result: " + netJoinResult2.ToString());
				netJoinResult = netJoinResult2;
			}
		}
		return netJoinResult;
	}

	// Token: 0x06001A06 RID: 6662 RVA: 0x00090DB0 File Offset: 0x0008EFB0
	private async Task<bool> Connect(global::Fusion.GameMode mode, string targetSessionName, RoomConfig opts)
	{
		if (this.runner != null)
		{
			bool goingBetweenRooms = this.InRoom && mode != global::Fusion.GameMode.Single;
			await this.CloseRunner(ShutdownReason.Ok);
			await Task.Yield();
			if (goingBetweenRooms)
			{
				base.SinglePlayerStarted();
				await Task.Yield();
			}
		}
		if (this.volatileNetObj)
		{
			Debug.LogError("Volatile net obj should not exist - destroying and recreating");
			Object.Destroy(this.volatileNetObj);
		}
		this.volatileNetObj = new GameObject("VolatileFusionObj");
		this.volatileNetObj.transform.parent = base.transform;
		this.runner = this.volatileNetObj.AddComponent<NetworkRunner>();
		this.internalRPCProvider = this.runner.AddBehaviour<FusionInternalRPCs>();
		this.callbackHandler = this.volatileNetObj.AddComponent<FusionCallbackHandler>();
		this.callbackHandler.Setup(this);
		this.AttachCallbackTargets();
		this.lastConnectAttempt_WasFull = false;
		this.internalState = NetworkSystemFusion.InternalState.ConnectingToRoom;
		Hashtable customProps = opts.CustomProps;
		Dictionary<string, SessionProperty> dictionary = ((customProps != null) ? customProps.ToPropDict() : null);
		this.myObjectProvider.SceneObjects = this.SceneObjectsToAttach;
		NetworkSceneManagerDefault networkSceneManagerDefault = this.volatileNetObj.AddComponent<NetworkSceneManagerDefault>();
		Task<global::Fusion.StartGameResult> startupTask = this.runner.StartGame(new StartGameArgs
		{
			IsVisible = new bool?(opts.isPublic),
			IsOpen = new bool?(opts.isJoinable),
			GameMode = mode,
			SessionName = targetSessionName,
			PlayerCount = new int?((int)opts.MaxPlayers),
			SceneManager = networkSceneManagerDefault,
			AuthValues = this.cachedPlayfabAuth,
			SessionProperties = dictionary,
			EnableClientSessionCreation = new bool?(opts.createIfMissing),
			ObjectProvider = this.myObjectProvider
		});
		await startupTask;
		Utils.Log("Startuptask finished : " + startupTask.Result.ToString());
		bool flag;
		if (!startupTask.Result.Ok)
		{
			base.CurrentRoom = null;
			flag = startupTask.Result.Ok;
		}
		else
		{
			if (this.cachedNetSceneObjects.Count > 0)
			{
				foreach (NetworkObject networkObject in this.cachedNetSceneObjects)
				{
					this.registrationQueue.Enqueue(networkObject);
				}
			}
			this.AttachSceneObjects(false);
			this.AddVoice();
			base.CurrentRoom = opts;
			if (this.IsTotalAuthority() || this.runner.IsSharedModeMasterClient)
			{
				opts.SetFusionOpts(this.runner);
			}
			this.SetMyNickName(GorillaComputer.instance.savedName);
			flag = startupTask.Result.Ok;
		}
		return flag;
	}

	// Token: 0x06001A07 RID: 6663 RVA: 0x00090E0C File Offset: 0x0008F00C
	private async Task<NetJoinResult> MakeOrJoinRoom(string roomName, RoomConfig opts)
	{
		int currentRegionIndex = 0;
		bool flag = false;
		opts.createIfMissing = false;
		Task<bool> connectTask;
		while (currentRegionIndex < this.regionNames.Length && !flag)
		{
			try
			{
				PhotonAppSettings.Global.AppSettings.FixedRegion = this.regionNames[currentRegionIndex];
				this.internalState = NetworkSystemFusion.InternalState.Searching_Joining;
				connectTask = this.Connect(global::Fusion.GameMode.Shared, roomName, opts);
				await connectTask;
				flag = connectTask.Result;
				if (!flag)
				{
					if (this.lastConnectAttempt_WasFull)
					{
						Utils.Log("Found room but it was full");
						break;
					}
					Utils.Log("Region incrimenting");
					currentRegionIndex++;
				}
				connectTask = null;
			}
			catch (Exception ex)
			{
				Debug.LogError("MakeOrJoinRoom - message: " + ex.Message + "\nStacktrace : " + ex.StackTrace);
				return NetJoinResult.Failed_Other;
			}
		}
		if (this.lastConnectAttempt_WasFull)
		{
			PhotonAppSettings.Global.AppSettings.FixedRegion = "";
			return NetJoinResult.Failed_Full;
		}
		if (flag)
		{
			return NetJoinResult.Success;
		}
		PhotonAppSettings.Global.AppSettings.FixedRegion = "";
		opts.createIfMissing = true;
		connectTask = this.Connect(global::Fusion.GameMode.Shared, roomName, opts);
		await connectTask;
		Utils.Log("made room?");
		if (!connectTask.Result)
		{
			Debug.LogError("NS-FUS] Failed to create private room");
			return NetJoinResult.Failed_Other;
		}
		while (!this.runner.SessionInfo.IsValid)
		{
			await Task.Yield();
		}
		return NetJoinResult.FallbackCreated;
	}

	// Token: 0x06001A08 RID: 6664 RVA: 0x00090E60 File Offset: 0x0008F060
	private async Task<NetJoinResult> JoinRandomPublicRoom(RoomConfig opts)
	{
		bool shouldCreateIfNone = opts.createIfMissing;
		PhotonAppSettings.Global.AppSettings.FixedRegion = "";
		this.internalState = NetworkSystemFusion.InternalState.Searching_Joining;
		opts.createIfMissing = false;
		Task<bool> connectTask = this.Connect(global::Fusion.GameMode.Shared, null, opts);
		await connectTask;
		NetJoinResult netJoinResult;
		if (!connectTask.Result && shouldCreateIfNone)
		{
			opts.createIfMissing = shouldCreateIfNone;
			Task<bool> createTask = this.Connect(global::Fusion.GameMode.Shared, NetworkSystem.GetRandomRoomName(), opts);
			await createTask;
			if (!createTask.Result)
			{
				Debug.LogError("NS-FUS] Failed to create public room");
				netJoinResult = NetJoinResult.Failed_Other;
			}
			else
			{
				opts.SetFusionOpts(this.runner);
				netJoinResult = NetJoinResult.FallbackCreated;
			}
		}
		else
		{
			netJoinResult = NetJoinResult.Success;
		}
		return netJoinResult;
	}

	// Token: 0x06001A09 RID: 6665 RVA: 0x00090EAC File Offset: 0x0008F0AC
	public override async Task JoinFriendsRoom(string userID, int actorIDToFollow, string keyToFollow, string shufflerToFollow)
	{
		bool foundFriend = false;
		float searchStartTime = Time.realtimeSinceStartup;
		float timeToSpendSearching = 15f;
		Dictionary<string, global::PlayFab.ClientModels.SharedGroupDataRecord> dummyData = new Dictionary<string, global::PlayFab.ClientModels.SharedGroupDataRecord>();
		try
		{
			base.groupJoinInProgress = true;
			while (!foundFriend && searchStartTime + timeToSpendSearching > Time.realtimeSinceStartup)
			{
				NetworkSystemFusion.<>c__DisplayClass63_0 CS$<>8__locals1 = new NetworkSystemFusion.<>c__DisplayClass63_0();
				CS$<>8__locals1.data = dummyData;
				CS$<>8__locals1.callbackFinished = false;
				PlayFabClientAPI.GetSharedGroupData(new global::PlayFab.ClientModels.GetSharedGroupDataRequest
				{
					Keys = new List<string> { keyToFollow },
					SharedGroupId = userID
				}, delegate(GetSharedGroupDataResult result)
				{
					CS$<>8__locals1.data = result.Data;
					Debug.Log(string.Format("Got friend follow data, {0} entries", CS$<>8__locals1.data.Count));
					CS$<>8__locals1.callbackFinished = true;
				}, delegate(PlayFabError error)
				{
					Debug.Log(string.Format("GetSharedGroupData returns error: {0}", error));
					CS$<>8__locals1.callbackFinished = true;
				}, null, null);
				while (!CS$<>8__locals1.callbackFinished)
				{
					await Task.Yield();
				}
				foreach (KeyValuePair<string, global::PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in CS$<>8__locals1.data)
				{
					if (keyValuePair.Key == keyToFollow)
					{
						string[] array = keyValuePair.Value.Value.Split("|", StringSplitOptions.None);
						if (array.Length == 2)
						{
							string roomID = NetworkSystem.ShuffleRoomName(array[0], shufflerToFollow.Substring(2, 8), false);
							int regionIndex = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".IndexOf(NetworkSystem.ShuffleRoomName(array[1], shufflerToFollow.Substring(0, 2), false));
							if (regionIndex >= 0 && regionIndex < NetworkSystem.Instance.regionNames.Length)
							{
								foundFriend = true;
								NetPlayer player = this.GetPlayer(actorIDToFollow);
								if (this.InRoom && this.GetPlayer(actorIDToFollow) != null)
								{
									MonkeAgent.instance.SendReport("possible kick attempt", player.UserId, player.NickName);
								}
								else if (this.RoomName != roomID)
								{
									await this.ReturnToSinglePlayer();
									Task<NetJoinResult> ConnectToRoomTask = this.ConnectToRoom(roomID, new RoomConfig
									{
										createIfMissing = false,
										isPublic = true,
										isJoinable = true
									}, regionIndex);
									await ConnectToRoomTask;
									NetJoinResult result2 = ConnectToRoomTask.Result;
									ConnectToRoomTask = null;
								}
								roomID = null;
							}
						}
					}
				}
				Dictionary<string, global::PlayFab.ClientModels.SharedGroupDataRecord>.Enumerator enumerator = default(Dictionary<string, global::PlayFab.ClientModels.SharedGroupDataRecord>.Enumerator);
				await Task.Delay(500);
				CS$<>8__locals1 = null;
			}
		}
		finally
		{
			base.groupJoinInProgress = false;
		}
	}

	// Token: 0x06001A0A RID: 6666 RVA: 0x00002E60 File Offset: 0x00001060
	public override void JoinPubWithFriends()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001A0B RID: 6667 RVA: 0x00090F10 File Offset: 0x0008F110
	public override async Task ReturnToSinglePlayer()
	{
		if (base.netState == NetSystemState.InGame || base.netState == NetSystemState.Initialization)
		{
			base.netState = NetSystemState.Disconnecting;
			Utils.Log("Returning to single player");
			if (this.runner)
			{
				await this.CloseRunner(ShutdownReason.Ok);
				await Task.Yield();
				Utils.Log("Connect in return to single player");
			}
			base.netState = NetSystemState.Idle;
			this.internalState = NetworkSystemFusion.InternalState.Idle;
			base.SinglePlayerStarted();
		}
	}

	// Token: 0x06001A0C RID: 6668 RVA: 0x00090F54 File Offset: 0x0008F154
	private async Task CloseRunner(ShutdownReason reason = ShutdownReason.Ok)
	{
		this.internalState = NetworkSystemFusion.InternalState.Disconnecting;
		try
		{
			await this.runner.Shutdown(true, reason, false);
		}
		catch (Exception ex)
		{
			StackFrame frame = new StackTrace(ex, true).GetFrame(0);
			int fileLineNumber = frame.GetFileLineNumber();
			Debug.LogError(string.Concat(new string[]
			{
				ex.Message,
				" File:",
				frame.GetFileName(),
				" line: ",
				fileLineNumber.ToString()
			}));
		}
		if (Application.isPlaying)
		{
			Object.Destroy(this.volatileNetObj);
		}
		else
		{
			Object.DestroyImmediate(this.volatileNetObj);
		}
		this.internalState = NetworkSystemFusion.InternalState.Disconnected;
	}

	// Token: 0x06001A0D RID: 6669 RVA: 0x00090FA0 File Offset: 0x0008F1A0
	public async void MigrateHost(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		Utils.Log("HOSTTEST : MigrateHostTriggered, returning to single player!");
		await this.ReturnToSinglePlayer();
	}

	// Token: 0x06001A0E RID: 6670 RVA: 0x00090FD8 File Offset: 0x0008F1D8
	public async void ResetSystem()
	{
		if (Application.isPlaying)
		{
			base.StopAllCoroutines();
			await this.Connect(global::Fusion.GameMode.Single, "--", RoomConfig.SPConfig());
			Utils.Log("Connect in return to single player");
			base.netState = NetSystemState.Idle;
			this.internalState = NetworkSystemFusion.InternalState.Idle;
		}
	}

	// Token: 0x06001A0F RID: 6671 RVA: 0x0009100F File Offset: 0x0008F20F
	private void AddVoice()
	{
		this.SetupVoice();
	}

	// Token: 0x06001A10 RID: 6672 RVA: 0x00091018 File Offset: 0x0008F218
	private void SetupVoice()
	{
		Utils.Log("<color=orange>Adding Voice Stuff</color>");
		this.FusionVoice = this.volatileNetObj.AddComponent<VoiceConnection>();
		this.FusionVoice.LogLevel = this.VoiceSettings.LogLevel;
		this.FusionVoice.GlobalRecordersLogLevel = this.VoiceSettings.GlobalRecordersLogLevel;
		this.FusionVoice.GlobalSpeakersLogLevel = this.VoiceSettings.GlobalSpeakersLogLevel;
		this.FusionVoice.AutoCreateSpeakerIfNotFound = this.VoiceSettings.CreateSpeakerIfNotFound;
		Photon.Realtime.AppSettings appSettings = new Photon.Realtime.AppSettings();
		appSettings.AppIdFusion = PhotonAppSettings.Global.AppSettings.AppIdFusion;
		appSettings.AppIdVoice = PhotonAppSettings.Global.AppSettings.AppIdVoice;
		this.FusionVoice.Settings = appSettings;
		this.remoteVoiceAddedCallbacks.ForEach(delegate(Action<RemoteVoiceLink> callback)
		{
			this.FusionVoice.RemoteVoiceAdded += callback;
		});
		this.localRecorder = this.volatileNetObj.AddComponent<Recorder>();
		this.localRecorder.LogLevel = this.VoiceSettings.LogLevel;
		this.localRecorder.RecordOnlyWhenEnabled = this.VoiceSettings.RecordOnlyWhenEnabled;
		this.localRecorder.RecordOnlyWhenJoined = this.VoiceSettings.RecordOnlyWhenJoined;
		this.localRecorder.StopRecordingWhenPaused = this.VoiceSettings.StopRecordingWhenPaused;
		this.localRecorder.TransmitEnabled = this.VoiceSettings.TransmitEnabled;
		this.localRecorder.AutoStart = this.VoiceSettings.AutoStart;
		this.localRecorder.Encrypt = this.VoiceSettings.Encrypt;
		this.localRecorder.FrameDuration = this.VoiceSettings.FrameDuration;
		this.localRecorder.SamplingRate = this.VoiceSettings.SamplingRate;
		this.localRecorder.InterestGroup = this.VoiceSettings.InterestGroup;
		this.localRecorder.SourceType = this.VoiceSettings.InputSourceType;
		this.localRecorder.MicrophoneType = this.VoiceSettings.MicrophoneType;
		this.localRecorder.UseMicrophoneTypeFallback = this.VoiceSettings.UseFallback;
		this.localRecorder.VoiceDetection = this.VoiceSettings.Detect;
		this.localRecorder.VoiceDetectionThreshold = this.VoiceSettings.Threshold;
		this.localRecorder.Bitrate = this.VoiceSettings.Bitrate;
		this.localRecorder.VoiceDetectionDelayMs = this.VoiceSettings.Delay;
		this.localRecorder.DebugEchoMode = this.VoiceSettings.DebugEcho;
		this.localRecorder.UserData = this.runner.UserId;
		this.FusionVoice.PrimaryRecorder = this.localRecorder;
		this.volatileNetObj.AddComponent<VoiceToLoudness>();
	}

	// Token: 0x06001A11 RID: 6673 RVA: 0x000912BB File Offset: 0x0008F4BB
	public override void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback)
	{
		this.remoteVoiceAddedCallbacks.Add(callback);
	}

	// Token: 0x06001A12 RID: 6674 RVA: 0x000912C9 File Offset: 0x0008F4C9
	private void AttachCallbackTargets()
	{
		this.runner.AddCallbacks(this.objectsThatNeedCallbacks.ToArray());
	}

	// Token: 0x06001A13 RID: 6675 RVA: 0x000912E1 File Offset: 0x0008F4E1
	public void RegisterForNetworkCallbacks(INetworkRunnerCallbacks callbacks)
	{
		if (!this.objectsThatNeedCallbacks.Contains(callbacks))
		{
			this.objectsThatNeedCallbacks.Add(callbacks);
		}
		if (this.runner != null)
		{
			this.runner.AddCallbacks(new INetworkRunnerCallbacks[] { callbacks });
		}
	}

	// Token: 0x06001A14 RID: 6676 RVA: 0x00091320 File Offset: 0x0008F520
	private async void AttachSceneObjects(bool onlyCached = false)
	{
		if (!onlyCached)
		{
			this.SceneObjectsToAttach.ForEach(delegate(GameObject obj)
			{
				if (!this.cachedNetSceneObjects.Exists((NetworkObject o) => o.gameObject == obj.gameObject))
				{
					NetworkObject component = obj.GetComponent<NetworkObject>();
					if (component == null)
					{
						Debug.LogWarning("no network object on scene item - " + obj.name);
						return;
					}
					this.cachedNetSceneObjects.Add(component);
					this.registrationQueue.Enqueue(component);
				}
			});
		}
		await Task.Delay(5);
		this.ProcessRegistrationQueue();
	}

	// Token: 0x06001A15 RID: 6677 RVA: 0x00091360 File Offset: 0x0008F560
	public override void AttachObjectInGame(GameObject item)
	{
		base.AttachObjectInGame(item);
		NetworkObject component = item.GetComponent<NetworkObject>();
		if ((component != null && !this.cachedNetSceneObjects.Contains(component)) || !component.IsValid)
		{
			this.cachedNetSceneObjects.AddIfNew(component);
			this.registrationQueue.Enqueue(component);
			this.ProcessRegistrationQueue();
		}
	}

	// Token: 0x06001A16 RID: 6678 RVA: 0x000913B8 File Offset: 0x0008F5B8
	private void ProcessRegistrationQueue()
	{
		if (this.isProcessingQueue)
		{
			Debug.LogError("Queue is still processing");
			return;
		}
		this.isProcessingQueue = true;
		List<NetworkObject> list = new List<NetworkObject>();
		SceneRef sceneRef = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
		while (this.registrationQueue.Count > 0)
		{
			NetworkObject networkObject = this.registrationQueue.Dequeue();
			if (this.InRoom && !networkObject.IsValid && !networkObject.Id.IsValid && networkObject.Runner == null)
			{
				try
				{
					list.Add(networkObject);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					this.isProcessingQueue = false;
					this.runner.RegisterSceneObjects(sceneRef, list.ToArray(), default(NetworkSceneLoadId));
					this.ProcessRegistrationQueue();
					break;
				}
			}
		}
		this.runner.RegisterSceneObjects(sceneRef, list.ToArray(), default(NetworkSceneLoadId));
		this.isProcessingQueue = false;
	}

	// Token: 0x06001A17 RID: 6679 RVA: 0x000914B4 File Offset: 0x0008F6B4
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false)
	{
		Utils.Log("Net instantiate Fusion: " + prefab.name);
		try
		{
			return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(this.runner.LocalPlayer), null, (NetworkSpawnFlags)0).gameObject;
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
		return null;
	}

	// Token: 0x06001A18 RID: 6680 RVA: 0x00091524 File Offset: 0x0008F724
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false)
	{
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (playerRef.PlayerId == playerAuthID)
			{
				Utils.Log("Net instantiate Fusion: " + prefab.name);
				return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(playerRef), null, (NetworkSpawnFlags)0).gameObject;
			}
		}
		Debug.LogError(string.Format("Couldn't find player with ID: {0}, cancelling requested spawn...", playerAuthID));
		return null;
	}

	// Token: 0x06001A19 RID: 6681 RVA: 0x000915D0 File Offset: 0x0008F7D0
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject, byte group = 0, object[] data = null, NetworkRunner.OnBeforeSpawned callback = null)
	{
		Utils.Log("Net instantiate Fusion: " + prefab.name);
		return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(this.runner.LocalPlayer), callback, (NetworkSpawnFlags)0).gameObject;
	}

	// Token: 0x06001A1A RID: 6682 RVA: 0x00091624 File Offset: 0x0008F824
	public override void NetDestroy(GameObject instance)
	{
		NetworkObject networkObject;
		if (instance.TryGetComponent<NetworkObject>(out networkObject))
		{
			this.runner.Despawn(networkObject);
			return;
		}
		Object.Destroy(instance);
	}

	// Token: 0x06001A1B RID: 6683 RVA: 0x00091650 File Offset: 0x0008F850
	public override bool ShouldSpawnLocally(int playerID)
	{
		if (this.runner.GameMode == global::Fusion.GameMode.Shared)
		{
			return this.runner.LocalPlayer.PlayerId == playerID || (playerID == -1 && this.runner.IsSharedModeMasterClient);
		}
		return this.runner.GameMode != global::Fusion.GameMode.Client;
	}

	// Token: 0x06001A1C RID: 6684 RVA: 0x000916A8 File Offset: 0x0008F8A8
	public override void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true)
	{
		Utils.Log(rpcMethod.GetDelegateName() + "RPC called!");
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				playerRef != this.runner.LocalPlayer;
			}
		}
	}

	// Token: 0x06001A1D RID: 6685 RVA: 0x00091720 File Offset: 0x0008F920
	public override void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true)
	{
		Utils.Log(rpcMethod.GetDelegateName() + "RPC called!");
		(ref args).SerializeToRPCData<T>();
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				playerRef != this.runner.LocalPlayer;
			}
		}
	}

	// Token: 0x06001A1E RID: 6686 RVA: 0x000917A0 File Offset: 0x0008F9A0
	public override void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true)
	{
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				playerRef != this.runner.LocalPlayer;
			}
		}
	}

	// Token: 0x06001A1F RID: 6687 RVA: 0x00091804 File Offset: 0x0008FA04
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod)
	{
		this.GetPlayerRef(targetPlayerID);
		Utils.Log(rpcMethod.GetDelegateName() + "RPC called!");
	}

	// Token: 0x06001A20 RID: 6688 RVA: 0x00091823 File Offset: 0x0008FA23
	public override void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args)
	{
		Utils.Log(rpcMethod.GetDelegateName() + "RPC called!");
		this.GetPlayerRef(targetPlayerID);
	}

	// Token: 0x06001A21 RID: 6689 RVA: 0x00091842 File Offset: 0x0008FA42
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message)
	{
		this.GetPlayerRef(targetPlayerID);
	}

	// Token: 0x06001A22 RID: 6690 RVA: 0x0009184C File Offset: 0x0008FA4C
	public override void NetRaiseEventReliable(byte eventCode, object data)
	{
		byte[] array = data.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedReliable(this.runner, eventCode, array, false, null, default(RpcInfo));
	}

	// Token: 0x06001A23 RID: 6691 RVA: 0x00091878 File Offset: 0x0008FA78
	public override void NetRaiseEventUnreliable(byte eventCode, object data)
	{
		byte[] array = data.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedUnreliable(this.runner, eventCode, array, false, null, default(RpcInfo));
	}

	// Token: 0x06001A24 RID: 6692 RVA: 0x000918A4 File Offset: 0x0008FAA4
	public override void NetRaiseEventReliable(byte eventCode, object data, NetEventOptions opts)
	{
		byte[] array = data.ByteSerialize();
		byte[] array2 = opts.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedReliable(this.runner, eventCode, array, true, array2, default(RpcInfo));
	}

	// Token: 0x06001A25 RID: 6693 RVA: 0x000918D8 File Offset: 0x0008FAD8
	public override void NetRaiseEventUnreliable(byte eventCode, object data, NetEventOptions opts)
	{
		byte[] array = data.ByteSerialize();
		byte[] array2 = opts.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedUnreliable(this.runner, eventCode, array, true, array2, default(RpcInfo));
	}

	// Token: 0x06001A26 RID: 6694 RVA: 0x00002E60 File Offset: 0x00001060
	public override string GetRandomWeightedRegion()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001A27 RID: 6695 RVA: 0x0009190C File Offset: 0x0008FB0C
	public override async Task AwaitSceneReady()
	{
		while (this.runner.SceneManager.IsBusy)
		{
			await Task.Yield();
		}
		for (float counter = 0f; counter < 0.5f; counter += Time.deltaTime)
		{
			await Task.Yield();
		}
	}

	// Token: 0x06001A28 RID: 6696 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnJoinedSession()
	{
	}

	// Token: 0x06001A29 RID: 6697 RVA: 0x0009194F File Offset: 0x0008FB4F
	public void OnJoinFailed(NetConnectFailedReason reason)
	{
		switch (reason)
		{
		case NetConnectFailedReason.Timeout:
		case NetConnectFailedReason.ServerRefused:
			break;
		case NetConnectFailedReason.ServerFull:
			this.lastConnectAttempt_WasFull = true;
			break;
		default:
			return;
		}
	}

	// Token: 0x06001A2A RID: 6698 RVA: 0x0009196D File Offset: 0x0008FB6D
	public void OnDisconnectedFromSession()
	{
		Utils.Log("On Disconnected");
		this.internalState = NetworkSystemFusion.InternalState.Disconnected;
		base.UpdatePlayers();
	}

	// Token: 0x06001A2B RID: 6699 RVA: 0x00091987 File Offset: 0x0008FB87
	public void OnRunnerShutDown()
	{
		Utils.Log("Runner shutdown callback");
		if (this.internalState == NetworkSystemFusion.InternalState.Disconnecting)
		{
			this.internalState = NetworkSystemFusion.InternalState.Disconnected;
		}
	}

	// Token: 0x06001A2C RID: 6700 RVA: 0x000919A5 File Offset: 0x0008FBA5
	public void OnFusionPlayerJoined(PlayerRef player)
	{
		this.AwaitJoiningPlayerClientReady(player);
	}

	// Token: 0x06001A2D RID: 6701 RVA: 0x000919B0 File Offset: 0x0008FBB0
	private async Task AwaitJoiningPlayerClientReady(PlayerRef player)
	{
		base.UpdatePlayers();
		if (this.runner != null && player == this.runner.LocalPlayer && !this.runner.IsSinglePlayer)
		{
			Utils.Log("JoinedNetworkRoom");
			await Task.Delay(8);
			base.JoinedNetworkRoom();
		}
		if (this.runner != null && player == this.runner.LocalPlayer && this.runner.IsSinglePlayer)
		{
			base.SinglePlayerStarted();
		}
		await Task.Delay(200);
		NetPlayer joiningPlayer = base.GetPlayer(player);
		if (joiningPlayer == null)
		{
			Debug.LogError("Joining player doesnt have a NetPlayer somehow, this shouldnt happen");
		}
		while (joiningPlayer.NickName.IsNullOrEmpty())
		{
			await Task.Delay(1);
		}
		base.PlayerJoined(joiningPlayer);
	}

	// Token: 0x06001A2E RID: 6702 RVA: 0x000919FC File Offset: 0x0008FBFC
	public void OnFusionPlayerLeft(PlayerRef player)
	{
		if (this.IsTotalAuthority())
		{
			NetworkObject playerObject = this.runner.GetPlayerObject(player);
			if (playerObject != null)
			{
				Utils.Log("Destroying player object for leaving player!");
				this.NetDestroy(playerObject.gameObject);
			}
			else
			{
				Utils.Log("Player left without destroying an avatar for it somehow?");
			}
		}
		NetPlayer player2 = base.GetPlayer(player);
		if (player2 == null)
		{
			Debug.LogError("Joining player doesnt have a NetPlayer somehow, this shouldnt happen");
		}
		base.PlayerLeft(player2);
		base.UpdatePlayers();
	}

	// Token: 0x06001A2F RID: 6703 RVA: 0x00091A6C File Offset: 0x0008FC6C
	protected override void UpdateNetPlayerList()
	{
		if (this.runner == null)
		{
			if (this.netPlayerCache.Count <= 1)
			{
				if (this.netPlayerCache.Exists((NetPlayer p) => p.IsLocal))
				{
					goto IL_0084;
				}
			}
			this.netPlayerCache.ForEach(delegate(NetPlayer p)
			{
				this.playerPool.Return((FusionNetPlayer)p);
			});
			this.netPlayerCache.Clear();
			this.netPlayerCache.Add(new FusionNetPlayer(default(PlayerRef)));
			return;
		}
		IL_0084:
		NetPlayer[] array;
		if (this.runner.IsSinglePlayer)
		{
			if (this.netPlayerCache.Count == 1 && this.netPlayerCache[0].IsLocal)
			{
				return;
			}
			bool flag = false;
			array = this.netPlayerCache.ToArray();
			if (this.netPlayerCache.Count > 0)
			{
				foreach (NetPlayer netPlayer in array)
				{
					if (((FusionNetPlayer)netPlayer).PlayerRef == this.runner.LocalPlayer)
					{
						flag = true;
					}
					else
					{
						this.playerPool.Return((FusionNetPlayer)netPlayer);
						this.netPlayerCache.Remove(netPlayer);
					}
				}
			}
			if (!flag)
			{
				FusionNetPlayer fusionNetPlayer = this.playerPool.Take();
				fusionNetPlayer.InitPlayer(this.runner.LocalPlayer);
				this.netPlayerCache.Add(fusionNetPlayer);
			}
		}
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			bool flag2 = false;
			for (int j = 0; j < this.netPlayerCache.Count; j++)
			{
				if (playerRef == ((FusionNetPlayer)this.netPlayerCache[j]).PlayerRef)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				FusionNetPlayer fusionNetPlayer2 = this.playerPool.Take();
				fusionNetPlayer2.InitPlayer(playerRef);
				this.netPlayerCache.Add(fusionNetPlayer2);
			}
		}
		array = this.netPlayerCache.ToArray();
		foreach (NetPlayer netPlayer2 in array)
		{
			bool flag3 = false;
			using (IEnumerator<PlayerRef> enumerator = this.runner.ActivePlayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == ((FusionNetPlayer)netPlayer2).PlayerRef)
					{
						flag3 = true;
					}
				}
			}
			if (!flag3)
			{
				this.playerPool.Return((FusionNetPlayer)netPlayer2);
				this.netPlayerCache.Remove(netPlayer2);
			}
		}
	}

	// Token: 0x06001A30 RID: 6704 RVA: 0x00091D28 File Offset: 0x0008FF28
	public override void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null)
	{
		PlayerRef playerRef = this.runner.LocalPlayer;
		if (owningPlayerID != null)
		{
			playerRef = this.GetPlayerRef(owningPlayerID.Value);
		}
		this.runner.SetPlayerObject(playerRef, playerInstance.GetComponent<NetworkObject>());
	}

	// Token: 0x06001A31 RID: 6705 RVA: 0x00091D6C File Offset: 0x0008FF6C
	private PlayerRef GetPlayerRef(int playerID)
	{
		if (this.runner == null)
		{
			Debug.LogWarning("There is no runner yet - returning default player ref");
			return default(PlayerRef);
		}
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (playerRef.PlayerId == playerID)
			{
				return playerRef;
			}
		}
		Debug.LogWarning(string.Format("GetPlayerRef - Couldn't find active player with ID #{0}", playerID));
		return default(PlayerRef);
	}

	// Token: 0x06001A32 RID: 6706 RVA: 0x00091E08 File Offset: 0x00090008
	public override NetPlayer GetLocalPlayer()
	{
		if (this.netPlayerCache.Count == 0 || this.netPlayerCache.Count != this.runner.SessionInfo.PlayerCount)
		{
			base.UpdatePlayers();
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.IsLocal)
			{
				return netPlayer;
			}
		}
		Debug.LogError("Somehow there is no local NetPlayer. This shoulnd't happen.");
		return null;
	}

	// Token: 0x06001A33 RID: 6707 RVA: 0x00091EA0 File Offset: 0x000900A0
	public override NetPlayer GetPlayer(int PlayerID)
	{
		if (PlayerID == -1)
		{
			Debug.LogWarning("Attempting to get NetPlayer for local -1 ID.");
			return null;
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.ActorNumber == PlayerID)
			{
				return netPlayer;
			}
		}
		if (this.netPlayerCache.Count == 0 || this.netPlayerCache.Count != this.runner.SessionInfo.PlayerCount)
		{
			base.UpdatePlayers();
			foreach (NetPlayer netPlayer2 in this.netPlayerCache)
			{
				if (netPlayer2.ActorNumber == PlayerID)
				{
					return netPlayer2;
				}
			}
		}
		Debug.LogError("Failed to find the player, before and after resyncing the player cache, this probably shoulnd't happen...");
		return null;
	}

	// Token: 0x06001A34 RID: 6708 RVA: 0x00091F94 File Offset: 0x00090194
	public override void SetMyNickName(string name)
	{
		if (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags) && !name.StartsWith("gorilla"))
		{
			Debug.Log("[KID] Trying to set custom nickname but that permission has been disallowed");
			if (this.InRoom && GorillaTagger.Instance.rigSerializer != null)
			{
				GorillaTagger.Instance.rigSerializer.nickName = "gorilla";
			}
			return;
		}
		PlayerPrefs.SetString("playerName", name);
		if (this.InRoom && GorillaTagger.Instance.rigSerializer != null)
		{
			GorillaTagger.Instance.rigSerializer.nickName = name;
		}
	}

	// Token: 0x06001A35 RID: 6709 RVA: 0x0009202E File Offset: 0x0009022E
	public override string GetMyNickName()
	{
		return PlayerPrefs.GetString("playerName");
	}

	// Token: 0x06001A36 RID: 6710 RVA: 0x0009203C File Offset: 0x0009023C
	public override string GetMyDefaultName()
	{
		return "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
	}

	// Token: 0x06001A37 RID: 6711 RVA: 0x00092070 File Offset: 0x00090270
	public override string GetNickName(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		return this.GetNickName(player);
	}

	// Token: 0x06001A38 RID: 6712 RVA: 0x0009208C File Offset: 0x0009028C
	public override string GetNickName(NetPlayer player)
	{
		if (player == null)
		{
			Debug.LogError("Cant get nick name as playerID doesnt have a NetPlayer...");
			return "";
		}
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
		{
			return rigContainer.Rig.rigSerializer.defaultName.Value ?? "";
		}
		return rigContainer.Rig.rigSerializer.nickName.Value ?? "";
	}

	// Token: 0x06001A39 RID: 6713 RVA: 0x00092105 File Offset: 0x00090305
	public override string GetMyUserID()
	{
		return this.runner.GetPlayerUserId(this.runner.LocalPlayer);
	}

	// Token: 0x06001A3A RID: 6714 RVA: 0x0009211D File Offset: 0x0009031D
	public override string GetUserID(int playerID)
	{
		if (this.runner == null)
		{
			return string.Empty;
		}
		return this.runner.GetPlayerUserId(this.GetPlayerRef(playerID));
	}

	// Token: 0x06001A3B RID: 6715 RVA: 0x00092145 File Offset: 0x00090345
	public override string GetUserID(NetPlayer player)
	{
		if (this.runner == null)
		{
			return string.Empty;
		}
		return this.runner.GetPlayerUserId(((FusionNetPlayer)player).PlayerRef);
	}

	// Token: 0x06001A3C RID: 6716 RVA: 0x00092171 File Offset: 0x00090371
	public override void SetMyTutorialComplete()
	{
		if (!(PlayerPrefs.GetString("didTutorial", "nope") == "done"))
		{
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06001A3D RID: 6717 RVA: 0x000921A2 File Offset: 0x000903A2
	public override bool GetMyTutorialCompletion()
	{
		return PlayerPrefs.GetString("didTutorial", "nope") == "done";
	}

	// Token: 0x06001A3E RID: 6718 RVA: 0x000921C0 File Offset: 0x000903C0
	public override bool GetPlayerTutorialCompletion(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player == null)
		{
			Debug.LogError("Player not found");
			return false;
		}
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (rigContainer == null)
		{
			Debug.LogError("VRRig not found for player");
			return false;
		}
		if (rigContainer.Rig.rigSerializer == null)
		{
			Debug.LogWarning("Vr rig serializer is not set up on the rig yet");
			return false;
		}
		return rigContainer.Rig.rigSerializer.tutorialComplete;
	}

	// Token: 0x06001A3F RID: 6719 RVA: 0x00092236 File Offset: 0x00090436
	public override string GetPlayerPlatform(NetPlayer player)
	{
		return "";
	}

	// Token: 0x06001A40 RID: 6720 RVA: 0x00092236 File Offset: 0x00090436
	public override string GetPlayerMothershipId(NetPlayer player)
	{
		return "";
	}

	// Token: 0x06001A41 RID: 6721 RVA: 0x0009223D File Offset: 0x0009043D
	public override int GlobalPlayerCount()
	{
		if (this.regionCrawler == null)
		{
			return 0;
		}
		return this.regionCrawler.PlayerCountGlobal;
	}

	// Token: 0x06001A42 RID: 6722 RVA: 0x0009225C File Offset: 0x0009045C
	public override int GetOwningPlayerID(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			return -1;
		}
		if (this.runner.GameMode == global::Fusion.GameMode.Shared)
		{
			return networkObject.StateAuthority.PlayerId;
		}
		return networkObject.InputAuthority.PlayerId;
	}

	// Token: 0x06001A43 RID: 6723 RVA: 0x000922A0 File Offset: 0x000904A0
	public override bool IsObjectLocallyOwned(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			return false;
		}
		if (this.runner.GameMode == global::Fusion.GameMode.Shared)
		{
			return networkObject.StateAuthority == this.runner.LocalPlayer;
		}
		return networkObject.InputAuthority == this.runner.LocalPlayer;
	}

	// Token: 0x06001A44 RID: 6724 RVA: 0x000922F4 File Offset: 0x000904F4
	public override bool IsTotalAuthority()
	{
		return this.runner.Mode == SimulationModes.Server || this.runner.Mode == SimulationModes.Host || this.runner.GameMode == global::Fusion.GameMode.Single || this.runner.IsSharedModeMasterClient;
	}

	// Token: 0x06001A45 RID: 6725 RVA: 0x00092330 File Offset: 0x00090530
	public override bool ShouldWriteObjectData(GameObject obj)
	{
		NetworkObject networkObject;
		return obj.TryGetComponent<NetworkObject>(out networkObject) && networkObject.HasStateAuthority;
	}

	// Token: 0x06001A46 RID: 6726 RVA: 0x00092350 File Offset: 0x00090550
	public override bool ShouldUpdateObject(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			return true;
		}
		if (this.IsTotalAuthority())
		{
			return true;
		}
		if (networkObject.InputAuthority.IsRealPlayer && !networkObject.InputAuthority.IsRealPlayer)
		{
			return networkObject.InputAuthority == this.runner.LocalPlayer;
		}
		return this.runner.IsSharedModeMasterClient;
	}

	// Token: 0x06001A47 RID: 6727 RVA: 0x000923B8 File Offset: 0x000905B8
	public override bool IsObjectRoomObject(GameObject obj)
	{
		NetworkObject networkObject;
		if (obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			Debug.LogWarning("Fusion currently automatically passes false for roomobject check.");
			return false;
		}
		return false;
	}

	// Token: 0x06001A48 RID: 6728 RVA: 0x000923DC File Offset: 0x000905DC
	private void OnMasterSwitch(NetPlayer player)
	{
		if (this.runner.IsSharedModeMasterClient)
		{
			Dictionary<string, SessionProperty> dictionary = new Dictionary<string, SessionProperty> { 
			{
				"MasterClient",
				base.LocalPlayer.ActorNumber
			} };
			this.runner.SessionInfo.UpdateCustomProperties(dictionary);
		}
	}

	// Token: 0x040024AB RID: 9387
	private NetworkSystemFusion.InternalState internalState;

	// Token: 0x040024AC RID: 9388
	private FusionInternalRPCs internalRPCProvider;

	// Token: 0x040024AD RID: 9389
	private FusionCallbackHandler callbackHandler;

	// Token: 0x040024AE RID: 9390
	private FusionRegionCrawler regionCrawler;

	// Token: 0x040024AF RID: 9391
	private GameObject volatileNetObj;

	// Token: 0x040024B0 RID: 9392
	private global::Fusion.Photon.Realtime.AuthenticationValues cachedPlayfabAuth;

	// Token: 0x040024B1 RID: 9393
	private const string playerPropertiesPath = "P_FusionProperties";

	// Token: 0x040024B2 RID: 9394
	private bool lastConnectAttempt_WasFull;

	// Token: 0x040024B3 RID: 9395
	private VoiceConnection FusionVoice;

	// Token: 0x040024B4 RID: 9396
	private CustomObjectProvider myObjectProvider;

	// Token: 0x040024B5 RID: 9397
	private ObjectPool<FusionNetPlayer> playerPool;

	// Token: 0x040024B6 RID: 9398
	public List<NetworkObject> cachedNetSceneObjects = new List<NetworkObject>();

	// Token: 0x040024B7 RID: 9399
	private List<INetworkRunnerCallbacks> objectsThatNeedCallbacks = new List<INetworkRunnerCallbacks>();

	// Token: 0x040024B8 RID: 9400
	private Queue<NetworkObject> registrationQueue = new Queue<NetworkObject>();

	// Token: 0x040024B9 RID: 9401
	private bool isProcessingQueue;

	// Token: 0x02000440 RID: 1088
	private enum InternalState
	{
		// Token: 0x040024BB RID: 9403
		AwaitingAuth,
		// Token: 0x040024BC RID: 9404
		Idle,
		// Token: 0x040024BD RID: 9405
		Searching_Joining,
		// Token: 0x040024BE RID: 9406
		Searching_Joined,
		// Token: 0x040024BF RID: 9407
		Searching_JoinFailed,
		// Token: 0x040024C0 RID: 9408
		Searching_Disconnecting,
		// Token: 0x040024C1 RID: 9409
		Searching_Disconnected,
		// Token: 0x040024C2 RID: 9410
		ConnectingToRoom,
		// Token: 0x040024C3 RID: 9411
		ConnectedToRoom,
		// Token: 0x040024C4 RID: 9412
		JoinRoomFailed,
		// Token: 0x040024C5 RID: 9413
		Disconnecting,
		// Token: 0x040024C6 RID: 9414
		Disconnected,
		// Token: 0x040024C7 RID: 9415
		StateCheckFailed
	}
}
