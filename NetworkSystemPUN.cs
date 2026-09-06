using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Fusion;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Audio;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

// Token: 0x02000472 RID: 1138
[RequireComponent(typeof(PUNCallbackNotifier))]
public class NetworkSystemPUN : NetworkSystem
{
	// Token: 0x170002EB RID: 747
	// (get) Token: 0x06001BAA RID: 7082 RVA: 0x00095EE9 File Offset: 0x000940E9
	public override NetPlayer[] AllNetPlayers
	{
		get
		{
			return this.m_allNetPlayers;
		}
	}

	// Token: 0x170002EC RID: 748
	// (get) Token: 0x06001BAB RID: 7083 RVA: 0x00095EF1 File Offset: 0x000940F1
	public override NetPlayer[] PlayerListOthers
	{
		get
		{
			return this.m_otherNetPlayers;
		}
	}

	// Token: 0x170002ED RID: 749
	// (get) Token: 0x06001BAC RID: 7084 RVA: 0x00095EF9 File Offset: 0x000940F9
	public override VoiceConnection VoiceConnection
	{
		get
		{
			return this.punVoice;
		}
	}

	// Token: 0x170002EE RID: 750
	// (get) Token: 0x06001BAD RID: 7085 RVA: 0x00095F04 File Offset: 0x00094104
	private int lowestPingRegionIndex
	{
		get
		{
			int num = 9999;
			int num2 = -1;
			for (int i = 0; i < this.regionData.Length; i++)
			{
				if (this.regionData[i].pingToRegion < num)
				{
					num = this.regionData[i].pingToRegion;
					num2 = i;
				}
			}
			return num2;
		}
	}

	// Token: 0x170002EF RID: 751
	// (get) Token: 0x06001BAE RID: 7086 RVA: 0x00095F4D File Offset: 0x0009414D
	// (set) Token: 0x06001BAF RID: 7087 RVA: 0x00095F55 File Offset: 0x00094155
	private NetworkSystemPUN.InternalState internalState
	{
		get
		{
			return this.currentState;
		}
		set
		{
			this.currentState = value;
		}
	}

	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x06001BB0 RID: 7088 RVA: 0x00095F5E File Offset: 0x0009415E
	public override string CurrentPhotonBackend
	{
		get
		{
			return "PUN";
		}
	}

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x06001BB1 RID: 7089 RVA: 0x00095F65 File Offset: 0x00094165
	public override bool IsOnline
	{
		get
		{
			return this.InRoom;
		}
	}

	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x06001BB2 RID: 7090 RVA: 0x00095F6D File Offset: 0x0009416D
	public override bool InRoom
	{
		get
		{
			return PhotonNetwork.InRoom;
		}
	}

	// Token: 0x170002F3 RID: 755
	// (get) Token: 0x06001BB3 RID: 7091 RVA: 0x00095F74 File Offset: 0x00094174
	public override string RoomName
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return ((currentRoom != null) ? currentRoom.Name : null) ?? string.Empty;
		}
	}

	// Token: 0x06001BB4 RID: 7092 RVA: 0x00095F90 File Offset: 0x00094190
	public override string RoomStringStripped()
	{
		Room currentRoom = PhotonNetwork.CurrentRoom;
		NetworkSystem.reusableSB.Clear();
		NetworkSystem.reusableSB.AppendFormat("Room: '{0}' ", (currentRoom.Name.Length < 20) ? currentRoom.Name : currentRoom.Name.Remove(20));
		NetworkSystem.reusableSB.AppendFormat("{0},{1} {3}/{2} players.", new object[]
		{
			currentRoom.IsVisible ? "visible" : "hidden",
			currentRoom.IsOpen ? "open" : "closed",
			currentRoom.MaxPlayers,
			currentRoom.PlayerCount
		});
		NetworkSystem.reusableSB.Append("\ncustomProps: {");
		NetworkSystem.reusableSB.AppendFormat("joinedGameMode={0}, ", (RoomSystem.RoomGameMode.Length < 50) ? RoomSystem.RoomGameMode : RoomSystem.RoomGameMode.Remove(50));
		IDictionary customProperties = currentRoom.CustomProperties;
		this.AppendStringFromDict(customProperties, "gameMode", 50, NetworkSystem.reusableSB);
		NetworkSystem.reusableSB.Append(", ");
		this.AppendStringFromDict(customProperties, "platform", 10, NetworkSystem.reusableSB);
		NetworkSystem.reusableSB.Append(", ");
		this.AppendStringFromDict(customProperties, "queueName", 15, NetworkSystem.reusableSB);
		NetworkSystem.reusableSB.Append(", ");
		this.AppendStringFromDict(customProperties, "language", 15, NetworkSystem.reusableSB);
		NetworkSystem.reusableSB.Append(", ");
		this.AppendStringFromDict(customProperties, "fan_club", 6, NetworkSystem.reusableSB);
		NetworkSystem.reusableSB.Append(", ");
		this.AppendStringFromDict(customProperties, "mmrTier", 8, NetworkSystem.reusableSB);
		NetworkSystem.reusableSB.Append("}");
		return NetworkSystem.reusableSB.ToString();
	}

	// Token: 0x06001BB5 RID: 7093 RVA: 0x00096164 File Offset: 0x00094364
	private void AppendStringFromDict(IDictionary dict, string key, int maxStrLen, StringBuilder sb)
	{
		sb.AppendFormat("{0}=", key);
		if (dict.Contains(key))
		{
			string text = dict[key] as string;
			if (text != null)
			{
				sb.Append((text.Length < maxStrLen) ? text : text.Remove(maxStrLen));
				return;
			}
		}
		sb.Append("null");
	}

	// Token: 0x170002F4 RID: 756
	// (get) Token: 0x06001BB6 RID: 7094 RVA: 0x000961C4 File Offset: 0x000943C4
	public override string GameModeString
	{
		get
		{
			object obj;
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj);
			if (obj != null)
			{
				return obj.ToString();
			}
			return null;
		}
	}

	// Token: 0x170002F5 RID: 757
	// (get) Token: 0x06001BB7 RID: 7095 RVA: 0x000961F3 File Offset: 0x000943F3
	public override string CurrentRegion
	{
		get
		{
			return PhotonNetwork.CloudRegion;
		}
	}

	// Token: 0x170002F6 RID: 758
	// (get) Token: 0x06001BB8 RID: 7096 RVA: 0x000961FA File Offset: 0x000943FA
	public override bool SessionIsPrivate
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return currentRoom != null && !currentRoom.IsVisible;
		}
	}

	// Token: 0x170002F7 RID: 759
	// (get) Token: 0x06001BB9 RID: 7097 RVA: 0x00096210 File Offset: 0x00094410
	public override bool SessionIsSubscription
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			byte? b = ((currentRoom != null) ? new byte?(currentRoom.MaxPlayers) : null);
			int? num = ((b != null) ? new int?((int)b.GetValueOrDefault()) : null);
			int num2 = 10;
			return (num.GetValueOrDefault() > num2) & (num != null);
		}
	}

	// Token: 0x170002F8 RID: 760
	// (get) Token: 0x06001BBA RID: 7098 RVA: 0x00096273 File Offset: 0x00094473
	public override int LocalPlayerID
	{
		get
		{
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x06001BBB RID: 7099 RVA: 0x0009627F File Offset: 0x0009447F
	public override int ServerTimestamp
	{
		get
		{
			return PhotonNetwork.ServerTimestamp;
		}
	}

	// Token: 0x170002FA RID: 762
	// (get) Token: 0x06001BBC RID: 7100 RVA: 0x00096286 File Offset: 0x00094486
	public override double SimTime
	{
		get
		{
			return PhotonNetwork.Time;
		}
	}

	// Token: 0x170002FB RID: 763
	// (get) Token: 0x06001BBD RID: 7101 RVA: 0x0009628D File Offset: 0x0009448D
	public override float SimDeltaTime
	{
		get
		{
			return Time.deltaTime;
		}
	}

	// Token: 0x170002FC RID: 764
	// (get) Token: 0x06001BBE RID: 7102 RVA: 0x0009627F File Offset: 0x0009447F
	public override int SimTick
	{
		get
		{
			return PhotonNetwork.ServerTimestamp;
		}
	}

	// Token: 0x170002FD RID: 765
	// (get) Token: 0x06001BBF RID: 7103 RVA: 0x00096294 File Offset: 0x00094494
	public override int TickRate
	{
		get
		{
			return PhotonNetwork.SerializationRate;
		}
	}

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x06001BC0 RID: 7104 RVA: 0x0009629B File Offset: 0x0009449B
	public override int RoomPlayerCount
	{
		get
		{
			return (int)PhotonNetwork.CurrentRoom.PlayerCount;
		}
	}

	// Token: 0x170002FF RID: 767
	// (get) Token: 0x06001BC1 RID: 7105 RVA: 0x000962A7 File Offset: 0x000944A7
	public override bool IsMasterClient
	{
		get
		{
			return PhotonNetwork.IsMasterClient;
		}
	}

	// Token: 0x06001BC2 RID: 7106 RVA: 0x000962B0 File Offset: 0x000944B0
	public override async void Initialise()
	{
		base.Initialise();
		base.netState = NetSystemState.Initialization;
		PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = NetworkSystemConfig.AppVersion;
		PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
		PhotonNetwork.EnableCloseConnection = false;
		PhotonNetwork.AutomaticallySyncScene = false;
		string playerName = PlayerPrefs.GetString("playerName", "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0'));
		this.playerPool = new ObjectPool<PunNetPlayer>(20);
		base.UpdatePlayers();
		await this.CacheRegionInfo();
		base.UpdatePlayers();
		this.SetMyNickName(playerName);
	}

	// Token: 0x06001BC3 RID: 7107 RVA: 0x000962E8 File Offset: 0x000944E8
	private async Task CacheRegionInfo()
	{
		if (!this.isWrongVersion)
		{
			this.regionData = new NetworkRegionInfo[this.regionNames.Length];
			for (int i = 0; i < this.regionData.Length; i++)
			{
				this.regionData[i] = new NetworkRegionInfo();
			}
			TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(NetworkSystemPUN.InternalState.Authenticated, float.PositiveInfinity).GetAwaiter();
			TaskAwaiter<bool> taskAwaiter2;
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (taskAwaiter.GetResult())
			{
				base.netState = NetSystemState.PingRecon;
				for (int tryingRegionIndex = 0; tryingRegionIndex < this.regionNames.Length; tryingRegionIndex++)
				{
					this.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
					PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[tryingRegionIndex];
					this.currentRegionIndex = tryingRegionIndex;
					PhotonNetwork.ConnectUsingSettings();
					taskAwaiter = this.WaitForStateCheck(NetworkSystemPUN.InternalState.ConnectedToMaster, 10f).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						base.netState = NetSystemState.PingRecon;
					}
					else
					{
						this.regionData[this.currentRegionIndex].playersInRegion = PhotonNetwork.CountOfPlayers;
						this.regionData[this.currentRegionIndex].pingToRegion = PhotonNetwork.GetPing();
						Utils.Log("Ping for " + PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion.ToString() + " is " + PhotonNetwork.GetPing().ToString());
						this.internalState = NetworkSystemPUN.InternalState.PingGathering;
						PhotonNetwork.Disconnect();
						taskAwaiter = this.WaitForStateCheck(NetworkSystemPUN.InternalState.Internal_Disconnected, 10f).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<bool>);
						}
						if (!taskAwaiter.GetResult())
						{
							return;
						}
					}
				}
				this.internalState = NetworkSystemPUN.InternalState.Idle;
				base.netState = NetSystemState.Idle;
			}
		}
	}

	// Token: 0x06001BC4 RID: 7108 RVA: 0x0009632B File Offset: 0x0009452B
	public override AuthenticationValues GetAuthenticationValues()
	{
		return PhotonNetwork.AuthValues;
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x00096332 File Offset: 0x00094532
	public override void SetAuthenticationValues(AuthenticationValues authValues)
	{
		PhotonNetwork.AuthValues = authValues;
	}

	// Token: 0x06001BC6 RID: 7110 RVA: 0x0009633C File Offset: 0x0009453C
	public override void FinishAuthenticating()
	{
		if (PhotonNetwork.AuthValues == null)
		{
			this._taskCancelTokens.ForEach(delegate(CancellationTokenSource cts)
			{
				cts.Cancel();
				cts.Dispose();
			});
			this._taskCancelTokens.Clear();
			return;
		}
		this.internalState = NetworkSystemPUN.InternalState.Authenticated;
	}

	// Token: 0x06001BC7 RID: 7111 RVA: 0x00096390 File Offset: 0x00094590
	private async Task WaitForState(CancellationToken ct, NetworkSystemPUN.InternalState[] desiredStates, float timeout)
	{
		float timeoutTime = Time.realtimeSinceStartup + timeout;
		while (!desiredStates.Contains(this.internalState))
		{
			if (ct.IsCancellationRequested)
			{
				string text = "";
				foreach (NetworkSystemPUN.InternalState internalState in desiredStates)
				{
					text += string.Format("- {0}", internalState);
				}
				Debug.LogError("Got cancelation token while waiting for states " + text);
				this.internalState = NetworkSystemPUN.InternalState.StateCheckFailed;
				break;
			}
			if (timeoutTime < Time.realtimeSinceStartup)
			{
				string text2 = "";
				foreach (NetworkSystemPUN.InternalState internalState2 in desiredStates)
				{
					text2 += string.Format("- {0}", internalState2);
				}
				Debug.LogError("Got stuck waiting for states " + text2);
				this.internalState = NetworkSystemPUN.InternalState.StateCheckFailed;
				break;
			}
			await Task.Yield();
		}
	}

	// Token: 0x06001BC8 RID: 7112 RVA: 0x000963EC File Offset: 0x000945EC
	private Task<bool> WaitForStateCheck(NetworkSystemPUN.InternalState[] desiredStates, float timeout = 10f)
	{
		NetworkSystemPUN.<WaitForStateCheck>d__62 <WaitForStateCheck>d__;
		<WaitForStateCheck>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<WaitForStateCheck>d__.<>4__this = this;
		<WaitForStateCheck>d__.desiredStates = desiredStates;
		<WaitForStateCheck>d__.timeout = timeout;
		<WaitForStateCheck>d__.<>1__state = -1;
		<WaitForStateCheck>d__.<>t__builder.Start<NetworkSystemPUN.<WaitForStateCheck>d__62>(ref <WaitForStateCheck>d__);
		return <WaitForStateCheck>d__.<>t__builder.Task;
	}

	// Token: 0x06001BC9 RID: 7113 RVA: 0x0009643F File Offset: 0x0009463F
	private Task<bool> WaitForStateCheck(NetworkSystemPUN.InternalState desiredState, float timeout = 10f)
	{
		return this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { desiredState }, timeout);
	}

	// Token: 0x06001BCA RID: 7114 RVA: 0x00096454 File Offset: 0x00094654
	private async Task<NetJoinResult> MakeOrFindRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		if (this.InRoom)
		{
			await this.InternalDisconnect();
		}
		this.currentRegionIndex = 0;
		bool flag = ((regionIndex >= 0) ? (await this.TryJoinRoomInRegion(roomName, opts, regionIndex)) : (await this.TryJoinRoom(roomName, opts)));
		NetJoinResult netJoinResult;
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_JoinFailed_Full)
		{
			netJoinResult = NetJoinResult.Failed_Full;
		}
		else if (!flag)
		{
			netJoinResult = await this.TryCreateRoom(roomName, opts);
		}
		else if (this.internalState != NetworkSystemPUN.InternalState.Searching_Joined)
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else
		{
			netJoinResult = NetJoinResult.Success;
		}
		return netJoinResult;
	}

	// Token: 0x06001BCB RID: 7115 RVA: 0x000964B0 File Offset: 0x000946B0
	private async Task<bool> TryJoinRoom(string roomName, RoomConfig opts)
	{
		while (this.currentRegionIndex < this.regionNames.Length)
		{
			TaskAwaiter<bool> taskAwaiter = this.TryJoinRoomInRegion(roomName, opts, this.currentRegionIndex).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (taskAwaiter.GetResult())
			{
				return true;
			}
			this.currentRegionIndex++;
		}
		return false;
	}

	// Token: 0x06001BCC RID: 7116 RVA: 0x00096504 File Offset: 0x00094704
	private async Task<bool> TryJoinRoomInRegion(string roomName, RoomConfig opts, int regionIndex)
	{
		this.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
		string text = this.regionNames[regionIndex];
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = text;
		this.currentRegionIndex = regionIndex;
		this.UpdateZoneInfo(opts.isPublic, null);
		PhotonNetwork.ConnectUsingSettings();
		TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(NetworkSystemPUN.InternalState.ConnectedToMaster, 10f).GetAwaiter();
		TaskAwaiter<bool> taskAwaiter2;
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		bool flag;
		if (!taskAwaiter.GetResult())
		{
			flag = false;
		}
		else
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Joining;
			PhotonNetwork.JoinRoom(roomName, null);
			taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
			{
				NetworkSystemPUN.InternalState.Searching_Joined,
				NetworkSystemPUN.InternalState.Searching_JoinFailed_NotFound,
				NetworkSystemPUN.InternalState.Searching_JoinFailed_Full,
				NetworkSystemPUN.InternalState.Searching_JoinFailed_Other
			}, 10f).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				flag = false;
			}
			else
			{
				bool foundRoom = this.internalState != NetworkSystemPUN.InternalState.Searching_JoinFailed_NotFound;
				if (!foundRoom)
				{
					PhotonNetwork.Disconnect();
					this.internalState = NetworkSystemPUN.InternalState.Searching_Disconnecting;
					taskAwaiter = this.WaitForStateCheck(NetworkSystemPUN.InternalState.Searching_Disconnected, 10f).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						return false;
					}
				}
				flag = foundRoom;
			}
		}
		return flag;
	}

	// Token: 0x06001BCD RID: 7117 RVA: 0x00096560 File Offset: 0x00094760
	private async Task<NetJoinResult> TryCreateRoom(string roomName, RoomConfig opts)
	{
		Debug.Log("returning to best region to create room");
		this.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[this.lowestPingRegionIndex];
		this.currentRegionIndex = this.lowestPingRegionIndex;
		this.UpdateZoneInfo(opts.isPublic, null);
		PhotonNetwork.ConnectUsingSettings();
		TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(NetworkSystemPUN.InternalState.ConnectedToMaster, 10f).GetAwaiter();
		TaskAwaiter<bool> taskAwaiter2;
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		NetJoinResult netJoinResult;
		if (!taskAwaiter.GetResult())
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Creating;
			PhotonNetwork.CreateRoom(roomName, opts.ToPUNOpts(), null, null);
			taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
			{
				NetworkSystemPUN.InternalState.Searching_Created,
				NetworkSystemPUN.InternalState.Searching_CreateFailed
			}, 10f).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				netJoinResult = NetJoinResult.Failed_Other;
			}
			else if (this.internalState == NetworkSystemPUN.InternalState.Searching_CreateFailed)
			{
				netJoinResult = NetJoinResult.Failed_Other;
			}
			else
			{
				netJoinResult = NetJoinResult.FallbackCreated;
			}
		}
		return netJoinResult;
	}

	// Token: 0x06001BCE RID: 7118 RVA: 0x000965B4 File Offset: 0x000947B4
	private async Task<NetJoinResult> JoinRandomPublicRoom(RoomConfig opts)
	{
		if (this.InRoom)
		{
			await this.InternalDisconnect();
		}
		this.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
		object obj;
		if (!this.firstRoomJoin && opts.CustomProps.TryGetValue("gameMode", out obj) && !obj.ToString().StartsWith("city"))
		{
			this.firstRoomJoin = true;
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[this.lowestPingRegionIndex];
			this.currentRegionIndex = this.lowestPingRegionIndex;
		}
		else if (!opts.IsJoiningWithFriends)
		{
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.GetRandomWeightedRegion();
			this.currentRegionIndex = Array.IndexOf<string>(this.regionNames, PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion);
		}
		this.UpdateZoneInfo(true, PhotonNetworkController.Instance.currentJoinTrigger.zone.GetName<GTZone>());
		PhotonNetwork.ConnectUsingSettings();
		TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(NetworkSystemPUN.InternalState.ConnectedToMaster, 10f).GetAwaiter();
		TaskAwaiter<bool> taskAwaiter2;
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		NetJoinResult netJoinResult;
		if (!taskAwaiter.GetResult())
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Joining;
			if (opts.IsJoiningWithFriends)
			{
				PhotonNetwork.JoinRandomRoom(opts.EffectiveSearchFilter, opts.MaxPlayers, MatchmakingMode.RandomMatching, null, null, opts.joinFriendIDs.ToArray<string>());
			}
			else
			{
				PhotonNetwork.JoinRandomRoom(opts.EffectiveSearchFilter, opts.MaxPlayers, MatchmakingMode.FillRoom, null, null, null);
			}
			taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
			{
				NetworkSystemPUN.InternalState.Searching_Joined,
				NetworkSystemPUN.InternalState.Searching_JoinFailed_NotFound,
				NetworkSystemPUN.InternalState.Searching_JoinFailed_Full,
				NetworkSystemPUN.InternalState.Searching_JoinFailed_Other
			}, 10f).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				netJoinResult = NetJoinResult.Failed_Other;
			}
			else if (this.internalState != NetworkSystemPUN.InternalState.Searching_Joined)
			{
				this.internalState = NetworkSystemPUN.InternalState.Searching_Creating;
				string text = "";
				if (opts.MaxPlayers == 20 && opts.isPublic)
				{
					text = ":GTFC";
				}
				if (opts.IsJoiningWithFriends)
				{
					PhotonNetwork.CreateRoom(NetworkSystem.GetRandomRoomName() + text, opts.ToPUNOpts(), null, opts.joinFriendIDs);
				}
				else
				{
					PhotonNetwork.CreateRoom(NetworkSystem.GetRandomRoomName() + text, opts.ToPUNOpts(), null, null);
				}
				taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
				{
					NetworkSystemPUN.InternalState.Searching_Created,
					NetworkSystemPUN.InternalState.Searching_CreateFailed
				}, 10f).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (!taskAwaiter.GetResult())
				{
					netJoinResult = NetJoinResult.Failed_Other;
				}
				else if (this.internalState == NetworkSystemPUN.InternalState.Searching_CreateFailed)
				{
					netJoinResult = NetJoinResult.Failed_Other;
				}
				else
				{
					netJoinResult = NetJoinResult.FallbackCreated;
				}
			}
			else
			{
				netJoinResult = NetJoinResult.Success;
			}
		}
		return netJoinResult;
	}

	// Token: 0x06001BCF RID: 7119 RVA: 0x00096600 File Offset: 0x00094800
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
		else if (this.roomTask != null && !this.roomTask.IsCompleted)
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else
		{
			base.netState = NetSystemState.Connecting;
			NetJoinResult netJoinResult2;
			if (roomName != null)
			{
				this.roomTask = this.MakeOrFindRoom(roomName, opts, regionIndex);
				netJoinResult2 = await this.roomTask;
				this.roomTask = null;
			}
			else
			{
				this.roomTask = this.JoinRandomPublicRoom(opts);
				netJoinResult2 = await this.roomTask;
				this.roomTask = null;
			}
			if (netJoinResult2 == NetJoinResult.Failed_Full)
			{
				GorillaComputer.instance.roomFull = true;
				GorillaComputer.instance.UpdateScreen();
				this.ResetSystem();
				this.roomTask = null;
				netJoinResult = netJoinResult2;
			}
			else if (netJoinResult2 == NetJoinResult.Failed_Other)
			{
				this.ResetSystem();
				this.roomTask = null;
				netJoinResult = netJoinResult2;
			}
			else if (netJoinResult2 == NetJoinResult.AlreadyInRoom)
			{
				base.netState = NetSystemState.InGame;
				this.roomTask = null;
				netJoinResult = netJoinResult2;
			}
			else if (!this.InRoom)
			{
				GTDev.LogError<string>("NetworkSystem: room joined success but we have disconnected", null);
				netJoinResult = NetJoinResult.Failed_Other;
			}
			else
			{
				base.netState = NetSystemState.InGame;
				base.PlayerJoined(base.LocalPlayer);
				this.localRecorder.StartRecording();
				netJoinResult = netJoinResult2;
			}
		}
		return netJoinResult;
	}

	// Token: 0x06001BD0 RID: 7120 RVA: 0x0009665C File Offset: 0x0009485C
	public override async Task JoinFriendsRoom(string userID, int actorIDToFollow, string keyToFollow, string shufflerToFollow)
	{
		bool foundFriend = false;
		float searchStartTime = Time.realtimeSinceStartup;
		float timeToSpendSearching = 15f;
		Dictionary<string, global::PlayFab.ClientModels.SharedGroupDataRecord> dummyData = new Dictionary<string, global::PlayFab.ClientModels.SharedGroupDataRecord>();
		bool failedToJoinFriend = false;
		try
		{
			base.groupJoinInProgress = true;
			while (!foundFriend && searchStartTime + timeToSpendSearching > Time.realtimeSinceStartup)
			{
				NetworkSystemPUN.<>c__DisplayClass70_0 CS$<>8__locals1 = new NetworkSystemPUN.<>c__DisplayClass70_0();
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
								Player player;
								if (this.InRoom && PhotonNetwork.CurrentRoom.Players.TryGetValue(actorIDToFollow, out player) && player != null)
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
									failedToJoinFriend = result2 > NetJoinResult.Success;
									if (result2 == NetJoinResult.Success)
									{
										this.groupJoinOverrideGameMode = NetworkSystem.Instance.GameModeString;
									}
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
			if (failedToJoinFriend)
			{
				FriendshipGroupDetection.Instance.OnFailedToFollowParty();
			}
		}
	}

	// Token: 0x06001BD1 RID: 7121 RVA: 0x00002E60 File Offset: 0x00001060
	public override void JoinPubWithFriends()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001BD2 RID: 7122 RVA: 0x000966C0 File Offset: 0x000948C0
	public override string GetRandomWeightedRegion()
	{
		float value = Random.value;
		int num = 0;
		for (int i = 0; i < this.regionData.Length; i++)
		{
			num += this.regionData[i].playersInRegion;
		}
		float num2 = 0f;
		int num3 = -1;
		while (num2 < value && num3 < this.regionData.Length - 1)
		{
			num3++;
			num2 += (float)this.regionData[num3].playersInRegion / (float)num;
		}
		return this.regionNames[num3];
	}

	// Token: 0x06001BD3 RID: 7123 RVA: 0x00096738 File Offset: 0x00094938
	public override async Task ReturnToSinglePlayer()
	{
		if (base.netState == NetSystemState.InGame || base.netState == NetSystemState.Connecting)
		{
			base.netState = NetSystemState.Disconnecting;
			this._taskCancelTokens.ForEach(delegate(CancellationTokenSource cts)
			{
				cts.Cancel();
				cts.Dispose();
			});
			this._taskCancelTokens.Clear();
			await this.InternalDisconnect();
			base.netState = NetSystemState.Idle;
		}
	}

	// Token: 0x06001BD4 RID: 7124 RVA: 0x0009677C File Offset: 0x0009497C
	private async Task InternalDisconnect()
	{
		this.internalState = NetworkSystemPUN.InternalState.Internal_Disconnecting;
		PhotonNetwork.Disconnect();
		TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(NetworkSystemPUN.InternalState.Internal_Disconnected, 10f).GetAwaiter();
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			TaskAwaiter<bool> taskAwaiter2;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		if (!taskAwaiter.GetResult())
		{
			Debug.LogError("Failed to achieve internal disconnected state");
		}
		Object.Destroy(this.VoiceNetworkObject);
		base.UpdatePlayers();
		base.SinglePlayerStarted();
	}

	// Token: 0x06001BD5 RID: 7125 RVA: 0x000967BF File Offset: 0x000949BF
	private void AddVoice()
	{
		this.SetupVoice();
	}

	// Token: 0x06001BD6 RID: 7126 RVA: 0x000967C8 File Offset: 0x000949C8
	private void SetupVoice()
	{
		try
		{
			this.punVoice = PhotonVoiceNetwork.Instance;
			this.VoiceNetworkObject = this.punVoice.gameObject;
			this.VoiceNetworkObject.name = "VoiceNetworkObject";
			this.VoiceNetworkObject.transform.parent = base.transform;
			this.VoiceNetworkObject.transform.localPosition = Vector3.zero;
			this.punVoice.LogLevel = this.VoiceSettings.LogLevel;
			this.punVoice.GlobalRecordersLogLevel = this.VoiceSettings.GlobalRecordersLogLevel;
			this.punVoice.GlobalSpeakersLogLevel = this.VoiceSettings.GlobalSpeakersLogLevel;
			this.punVoice.AutoConnectAndJoin = this.VoiceSettings.AutoConnectAndJoin;
			this.punVoice.AutoLeaveAndDisconnect = this.VoiceSettings.AutoLeaveAndDisconnect;
			this.punVoice.WorkInOfflineMode = this.VoiceSettings.WorkInOfflineMode;
			this.punVoice.AutoCreateSpeakerIfNotFound = this.VoiceSettings.CreateSpeakerIfNotFound;
			AppSettings appSettings = new AppSettings();
			appSettings.AppIdRealtime = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
			appSettings.AppIdVoice = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice;
			this.punVoice.Settings = appSettings;
			this.remoteVoiceAddedCallbacks.ForEach(delegate(Action<RemoteVoiceLink> callback)
			{
				this.punVoice.RemoteVoiceAdded += callback;
			});
			this.localRecorder = this.VoiceNetworkObject.GetComponent<GTRecorder>();
			if (this.localRecorder == null)
			{
				this.localRecorder = this.VoiceNetworkObject.AddComponent<GTRecorder>();
				if (VRRigCache.Instance != null && VRRigCache.Instance.localRig != null)
				{
					LoudSpeakerActivator[] componentsInChildren = VRRigCache.Instance.localRig.GetComponentsInChildren<LoudSpeakerActivator>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].SetRecorder((GTRecorder)this.localRecorder);
					}
				}
			}
			this.localRecorder.LogLevel = this.VoiceSettings.LogLevel;
			this.localRecorder.RecordOnlyWhenEnabled = this.VoiceSettings.RecordOnlyWhenEnabled;
			this.localRecorder.RecordOnlyWhenJoined = this.VoiceSettings.RecordOnlyWhenJoined;
			this.localRecorder.StopRecordingWhenPaused = this.VoiceSettings.StopRecordingWhenPaused;
			this.localRecorder.TransmitEnabled = this.VoiceSettings.TransmitEnabled;
			this.localRecorder.AutoStart = this.VoiceSettings.AutoStart;
			this.localRecorder.Encrypt = this.VoiceSettings.Encrypt;
			this.localRecorder.FrameDuration = this.VoiceSettings.FrameDuration;
			this.localRecorder.InterestGroup = this.VoiceSettings.InterestGroup;
			this.localRecorder.SourceType = this.VoiceSettings.InputSourceType;
			this.localRecorder.MicrophoneType = this.VoiceSettings.MicrophoneType;
			this.localRecorder.UseMicrophoneTypeFallback = this.VoiceSettings.UseFallback;
			this.localRecorder.VoiceDetection = this.VoiceSettings.Detect;
			this.localRecorder.VoiceDetectionThreshold = this.VoiceSettings.Threshold;
			this.localRecorder.VoiceDetectionDelayMs = this.VoiceSettings.Delay;
			this.localRecorder.DebugEchoMode = this.VoiceSettings.DebugEcho;
			bool flag = SubscriptionManager.IsLocalSubscribed();
			bool flag2 = NetworkSystem.Instance.SessionIsSubscription || PhotonNetwork.CurrentRoom.Name.EndsWith(":GTFC");
			if (flag || flag2)
			{
				this.localRecorder.SamplingRate = this.VoiceSettings.SubsSamplingRate;
				this.localRecorder.Bitrate = this.VoiceSettings.SubsBitrate;
			}
			else
			{
				this.localRecorder.SamplingRate = this.VoiceSettings.SamplingRate;
				this.localRecorder.Bitrate = this.VoiceSettings.Bitrate;
			}
			this.VoiceNetworkObject.AddComponent<VoiceToLoudness>();
			this.punVoice.PrimaryRecorder = this.localRecorder;
		}
		catch (Exception ex)
		{
			Debug.LogError("An exception was thrown when trying to setup photon voice, please check microphone permissions:\n" + ex.ToString());
		}
	}

	// Token: 0x06001BD7 RID: 7127 RVA: 0x000912BB File Offset: 0x0008F4BB
	public override void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback)
	{
		this.remoteVoiceAddedCallbacks.Add(callback);
	}

	// Token: 0x06001BD8 RID: 7128 RVA: 0x00096BD8 File Offset: 0x00094DD8
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			return null;
		}
		if (isRoomObject)
		{
			return PhotonNetwork.InstantiateRoomObject(prefab.name, position, rotation, 0, null);
		}
		return PhotonNetwork.Instantiate(prefab.name, position, rotation, 0, null);
	}

	// Token: 0x06001BD9 RID: 7129 RVA: 0x00096C06 File Offset: 0x00094E06
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, position, rotation, isRoomObject);
	}

	// Token: 0x06001BDA RID: 7130 RVA: 0x00096C13 File Offset: 0x00094E13
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject, byte group = 0, object[] data = null, NetworkRunner.OnBeforeSpawned callback = null)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			return null;
		}
		if (isRoomObject)
		{
			return PhotonNetwork.InstantiateRoomObject(prefab.name, position, rotation, group, data);
		}
		return PhotonNetwork.Instantiate(prefab.name, position, rotation, group, data);
	}

	// Token: 0x06001BDB RID: 7131 RVA: 0x00096C48 File Offset: 0x00094E48
	public override void NetDestroy(GameObject instance)
	{
		PhotonView photonView;
		if (instance.TryGetComponent<PhotonView>(out photonView) && photonView.AmOwner)
		{
			PhotonNetwork.Destroy(instance);
			return;
		}
		Object.Destroy(instance);
	}

	// Token: 0x06001BDC RID: 7132 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null)
	{
	}

	// Token: 0x06001BDD RID: 7133 RVA: 0x00096C74 File Offset: 0x00094E74
	public override void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true)
	{
		RpcTarget rpcTarget = (sendToSelf ? RpcTarget.All : RpcTarget.Others);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, rpcTarget, new object[] { NetworkSystem.EmptyArgs });
	}

	// Token: 0x06001BDE RID: 7134 RVA: 0x00096CB0 File Offset: 0x00094EB0
	public override void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true)
	{
		RpcTarget rpcTarget = (sendToSelf ? RpcTarget.All : RpcTarget.Others);
		(ref args).SerializeToRPCData<T>();
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, rpcTarget, new object[] { args.Data });
	}

	// Token: 0x06001BDF RID: 7135 RVA: 0x00096CF4 File Offset: 0x00094EF4
	public override void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true)
	{
		RpcTarget rpcTarget = (sendToSelf ? RpcTarget.All : RpcTarget.Others);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, rpcTarget, new object[] { message });
	}

	// Token: 0x06001BE0 RID: 7136 RVA: 0x00096D2C File Offset: 0x00094F2C
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[] { NetworkSystem.EmptyArgs });
	}

	// Token: 0x06001BE1 RID: 7137 RVA: 0x00096D6C File Offset: 0x00094F6C
	public override void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		(ref args).SerializeToRPCData<T>();
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[] { args.Data });
	}

	// Token: 0x06001BE2 RID: 7138 RVA: 0x00096DB4 File Offset: 0x00094FB4
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[] { message });
	}

	// Token: 0x06001BE3 RID: 7139 RVA: 0x00096DF0 File Offset: 0x00094FF0
	public override async Task AwaitSceneReady()
	{
		while (PhotonNetwork.LevelLoadingProgress < 1f)
		{
			await Task.Yield();
		}
	}

	// Token: 0x06001BE4 RID: 7140 RVA: 0x00096E2C File Offset: 0x0009502C
	public override NetPlayer GetLocalPlayer()
	{
		if (this.netPlayerCache.Count == 0)
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
		Debug.LogError("Somehow no local net players found. This shouldn't happen");
		return null;
	}

	// Token: 0x06001BE5 RID: 7141 RVA: 0x00096EA4 File Offset: 0x000950A4
	public override NetPlayer GetPlayer(int PlayerID)
	{
		if (this.InRoom && !PhotonNetwork.CurrentRoom.Players.ContainsKey(PlayerID))
		{
			return null;
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.ActorNumber == PlayerID)
			{
				return netPlayer;
			}
		}
		base.UpdatePlayers();
		foreach (NetPlayer netPlayer2 in this.netPlayerCache)
		{
			if (netPlayer2.ActorNumber == PlayerID)
			{
				return netPlayer2;
			}
		}
		GTDev.LogWarning<string>("There is no NetPlayer with this ID currently in game. Passed ID: " + PlayerID.ToString(), null);
		return null;
	}

	// Token: 0x06001BE6 RID: 7142 RVA: 0x00096F84 File Offset: 0x00095184
	public override void SetMyNickName(string id)
	{
		if (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags) && !id.StartsWith("gorilla"))
		{
			Debug.Log("[KID] Trying to set custom nickname but that permission has been disallowed");
			PhotonNetwork.LocalPlayer.NickName = "gorilla";
			return;
		}
		PlayerPrefs.SetString("playerName", id);
		string nickName = PhotonNetwork.LocalPlayer.NickName;
		PhotonNetwork.LocalPlayer.NickName = id;
	}

	// Token: 0x06001BE7 RID: 7143 RVA: 0x00096FE1 File Offset: 0x000951E1
	public override string GetMyNickName()
	{
		return PhotonNetwork.LocalPlayer.NickName;
	}

	// Token: 0x06001BE8 RID: 7144 RVA: 0x00096FED File Offset: 0x000951ED
	public override string GetMyDefaultName()
	{
		return PhotonNetwork.LocalPlayer.DefaultName;
	}

	// Token: 0x06001BE9 RID: 7145 RVA: 0x00096FFC File Offset: 0x000951FC
	public override string GetNickName(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player != null)
		{
			return player.NickName;
		}
		return null;
	}

	// Token: 0x06001BEA RID: 7146 RVA: 0x0009701C File Offset: 0x0009521C
	public override string GetNickName(NetPlayer player)
	{
		return player.NickName;
	}

	// Token: 0x06001BEB RID: 7147 RVA: 0x00097024 File Offset: 0x00095224
	public override void SetMyTutorialComplete()
	{
		bool flag = PlayerPrefs.GetString("didTutorial", "nope") == "done";
		if (!flag)
		{
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add("didTutorial", flag);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
	}

	// Token: 0x06001BEC RID: 7148 RVA: 0x000921A2 File Offset: 0x000903A2
	public override bool GetMyTutorialCompletion()
	{
		return PlayerPrefs.GetString("didTutorial", "nope") == "done";
	}

	// Token: 0x06001BED RID: 7149 RVA: 0x00097088 File Offset: 0x00095288
	public override bool GetPlayerTutorialCompletion(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player == null)
		{
			return false;
		}
		Player playerRef = player.GetPlayerRef();
		if (playerRef == null)
		{
			return false;
		}
		object obj;
		if (playerRef.CustomProperties.TryGetValue("didTutorial", out obj))
		{
			bool flag;
			bool flag2;
			if (obj is bool)
			{
				flag = (bool)obj;
				flag2 = 1 == 0;
			}
			else
			{
				flag2 = true;
			}
			return flag2 || flag;
		}
		return false;
	}

	// Token: 0x06001BEE RID: 7150 RVA: 0x000970DC File Offset: 0x000952DC
	public override string GetPlayerPlatform(NetPlayer player)
	{
		if (player == null)
		{
			return "";
		}
		Player playerRef = player.GetPlayerRef();
		if (playerRef == null)
		{
			return "";
		}
		object obj;
		if (!playerRef.CustomProperties.TryGetValue("platform", out obj))
		{
			return "";
		}
		string text = obj as string;
		if (text == null)
		{
			return "";
		}
		return text;
	}

	// Token: 0x06001BEF RID: 7151 RVA: 0x00097130 File Offset: 0x00095330
	public override string GetPlayerMothershipId(NetPlayer player)
	{
		if (player == null)
		{
			return "";
		}
		Player playerRef = player.GetPlayerRef();
		if (playerRef == null)
		{
			return "";
		}
		object obj;
		if (!playerRef.CustomProperties.TryGetValue("mothershipId", out obj))
		{
			return "";
		}
		string text = obj as string;
		if (text == null)
		{
			return "";
		}
		return text;
	}

	// Token: 0x06001BF0 RID: 7152 RVA: 0x00097181 File Offset: 0x00095381
	public override string GetMyUserID()
	{
		return PhotonNetwork.LocalPlayer.UserId;
	}

	// Token: 0x06001BF1 RID: 7153 RVA: 0x00097190 File Offset: 0x00095390
	public override string GetUserID(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player != null)
		{
			return player.UserId;
		}
		return null;
	}

	// Token: 0x06001BF2 RID: 7154 RVA: 0x000971B0 File Offset: 0x000953B0
	public override string GetUserID(NetPlayer netPlayer)
	{
		Player playerRef = ((PunNetPlayer)netPlayer).PlayerRef;
		if (playerRef != null)
		{
			return playerRef.UserId;
		}
		return null;
	}

	// Token: 0x06001BF3 RID: 7155 RVA: 0x000971D4 File Offset: 0x000953D4
	public override int GlobalPlayerCount()
	{
		int num = 0;
		foreach (NetworkRegionInfo networkRegionInfo in this.regionData)
		{
			num += networkRegionInfo.playersInRegion;
		}
		return num;
	}

	// Token: 0x06001BF4 RID: 7156 RVA: 0x00097208 File Offset: 0x00095408
	public override bool IsObjectLocallyOwned(GameObject obj)
	{
		PhotonView photonView;
		return !this.IsOnline || !obj.TryGetComponent<PhotonView>(out photonView) || photonView.IsMine;
	}

	// Token: 0x06001BF5 RID: 7157 RVA: 0x00097234 File Offset: 0x00095434
	protected override void UpdateNetPlayerList()
	{
		if (!this.IsOnline)
		{
			bool flag = false;
			PunNetPlayer punNetPlayer = null;
			if (this.netPlayerCache.Count > 0)
			{
				for (int i = 0; i < this.netPlayerCache.Count; i++)
				{
					NetPlayer netPlayer = this.netPlayerCache[i];
					if (netPlayer.IsLocal)
					{
						punNetPlayer = (PunNetPlayer)netPlayer;
						flag = true;
					}
					else
					{
						this.playerPool.Return((PunNetPlayer)netPlayer);
					}
				}
				this.netPlayerCache.Clear();
			}
			if (!flag)
			{
				punNetPlayer = this.playerPool.Take();
				punNetPlayer.InitPlayer(PhotonNetwork.LocalPlayer);
			}
			this.netPlayerCache.Add(punNetPlayer);
		}
		else
		{
			Dictionary<int, Player>.ValueCollection values = PhotonNetwork.CurrentRoom.Players.Values;
			foreach (Player player in values)
			{
				bool flag2 = false;
				for (int j = 0; j < this.netPlayerCache.Count; j++)
				{
					if (player == ((PunNetPlayer)this.netPlayerCache[j]).PlayerRef)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					PunNetPlayer punNetPlayer2 = this.playerPool.Take();
					punNetPlayer2.InitPlayer(player);
					this.netPlayerCache.Add(punNetPlayer2);
				}
			}
			for (int k = 0; k < this.netPlayerCache.Count; k++)
			{
				PunNetPlayer punNetPlayer3 = (PunNetPlayer)this.netPlayerCache[k];
				bool flag3 = false;
				using (Dictionary<int, Player>.ValueCollection.Enumerator enumerator = values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == punNetPlayer3.PlayerRef)
						{
							flag3 = true;
							break;
						}
					}
				}
				if (!flag3)
				{
					this.playerPool.Return(punNetPlayer3);
					this.netPlayerCache.Remove(punNetPlayer3);
				}
			}
		}
		this.m_allNetPlayers = this.netPlayerCache.ToArray();
		this.m_otherNetPlayers = new NetPlayer[this.m_allNetPlayers.Length - 1];
		int num = 0;
		for (int l = 0; l < this.m_allNetPlayers.Length; l++)
		{
			NetPlayer netPlayer2 = this.m_allNetPlayers[l];
			if (netPlayer2.IsLocal)
			{
				num++;
			}
			else
			{
				int num2 = l - num;
				if (num2 == this.m_otherNetPlayers.Length)
				{
					break;
				}
				this.m_otherNetPlayers[num2] = netPlayer2;
			}
		}
	}

	// Token: 0x06001BF6 RID: 7158 RVA: 0x000974A0 File Offset: 0x000956A0
	public override bool IsObjectRoomObject(GameObject obj)
	{
		PhotonView component = obj.GetComponent<PhotonView>();
		if (component == null)
		{
			Debug.LogError("No photonview found on this Object, this shouldn't happen");
			return false;
		}
		return component.IsRoomView;
	}

	// Token: 0x06001BF7 RID: 7159 RVA: 0x000974CF File Offset: 0x000956CF
	public override bool ShouldUpdateObject(GameObject obj)
	{
		return this.IsObjectLocallyOwned(obj);
	}

	// Token: 0x06001BF8 RID: 7160 RVA: 0x000974CF File Offset: 0x000956CF
	public override bool ShouldWriteObjectData(GameObject obj)
	{
		return this.IsObjectLocallyOwned(obj);
	}

	// Token: 0x06001BF9 RID: 7161 RVA: 0x000974D8 File Offset: 0x000956D8
	public override int GetOwningPlayerID(GameObject obj)
	{
		PhotonView photonView;
		if (obj.TryGetComponent<PhotonView>(out photonView) && photonView.Owner != null)
		{
			return photonView.Owner.ActorNumber;
		}
		return -1;
	}

	// Token: 0x06001BFA RID: 7162 RVA: 0x00097504 File Offset: 0x00095704
	public override bool ShouldSpawnLocally(int playerID)
	{
		return this.LocalPlayerID == playerID || (playerID == -1 && PhotonNetwork.MasterClient.IsLocal);
	}

	// Token: 0x06001BFB RID: 7163 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsTotalAuthority()
	{
		return false;
	}

	// Token: 0x06001BFC RID: 7164 RVA: 0x00097521 File Offset: 0x00095721
	public void OnConnectedtoMaster()
	{
		if (this.internalState == NetworkSystemPUN.InternalState.ConnectingToMaster)
		{
			this.internalState = NetworkSystemPUN.InternalState.ConnectedToMaster;
		}
		base.UpdatePlayers();
	}

	// Token: 0x06001BFD RID: 7165 RVA: 0x00097539 File Offset: 0x00095739
	public void OnJoinedRoom()
	{
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Joining)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Joined;
		}
		else if (this.internalState == NetworkSystemPUN.InternalState.Searching_Creating)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Created;
		}
		this.AddVoice();
		base.UpdatePlayers();
		base.JoinedNetworkRoom();
	}

	// Token: 0x06001BFE RID: 7166 RVA: 0x00097574 File Offset: 0x00095774
	public void OnJoinRoomFailed(short returnCode, string message)
	{
		PersistLog.Log("OnJoinRoomFailed " + returnCode.ToString() + " " + message);
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Joining)
		{
			if (returnCode == 32758)
			{
				this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed_NotFound;
				return;
			}
			if (returnCode == 32765)
			{
				this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed_Full;
				return;
			}
			this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed_Other;
		}
	}

	// Token: 0x06001BFF RID: 7167 RVA: 0x000975D1 File Offset: 0x000957D1
	public void OnCreateRoomFailed(short returnCode, string message)
	{
		PersistLog.Log("OnCreateRoomFailed " + returnCode.ToString() + " " + message);
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Creating)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_CreateFailed;
		}
	}

	// Token: 0x06001C00 RID: 7168 RVA: 0x00097604 File Offset: 0x00095804
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.UpdatePlayers();
		NetPlayer player = base.GetPlayer(newPlayer);
		base.PlayerJoined(player);
	}

	// Token: 0x06001C01 RID: 7169 RVA: 0x00097628 File Offset: 0x00095828
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		NetPlayer player = base.GetPlayer(otherPlayer);
		base.UpdatePlayers();
		base.PlayerLeft(player);
	}

	// Token: 0x06001C02 RID: 7170 RVA: 0x0009764C File Offset: 0x0009584C
	public async void OnDisconnected(DisconnectCause cause)
	{
		if (!ApplicationQuittingState.IsQuitting)
		{
			PersistLog.Log("Disconnect callback, cause: " + cause.ToString());
			this.groupJoinOverrideGameMode = "";
			await base.RefreshNonce();
			if (this.internalState == NetworkSystemPUN.InternalState.Searching_Disconnecting)
			{
				this.internalState = NetworkSystemPUN.InternalState.Searching_Disconnected;
			}
			else if (this.internalState == NetworkSystemPUN.InternalState.PingGathering)
			{
				this.internalState = NetworkSystemPUN.InternalState.Internal_Disconnected;
			}
			else if (this.internalState == NetworkSystemPUN.InternalState.Internal_Disconnecting)
			{
				this.internalState = NetworkSystemPUN.InternalState.Internal_Disconnected;
			}
			else
			{
				base.UpdatePlayers();
				base.SinglePlayerStarted();
			}
		}
	}

	// Token: 0x06001C03 RID: 7171 RVA: 0x0009768B File Offset: 0x0009588B
	public void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitchedCallback(newMasterClient);
	}

	// Token: 0x06001C04 RID: 7172 RVA: 0x0009769C File Offset: 0x0009589C
	private ValueTuple<CancellationTokenSource, CancellationToken> GetCancellationToken()
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		CancellationToken token = cancellationTokenSource.Token;
		this._taskCancelTokens.Add(cancellationTokenSource);
		return new ValueTuple<CancellationTokenSource, CancellationToken>(cancellationTokenSource, token);
	}

	// Token: 0x06001C05 RID: 7173 RVA: 0x000976CC File Offset: 0x000958CC
	public void ResetSystem()
	{
		if (this.VoiceNetworkObject)
		{
			Object.Destroy(this.VoiceNetworkObject);
		}
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[this.lowestPingRegionIndex];
		this.currentRegionIndex = this.lowestPingRegionIndex;
		PhotonNetwork.Disconnect();
		this._taskCancelTokens.ForEach(delegate(CancellationTokenSource token)
		{
			token.Cancel();
			token.Dispose();
		});
		this._taskCancelTokens.Clear();
		this.internalState = NetworkSystemPUN.InternalState.Idle;
		base.netState = NetSystemState.Idle;
	}

	// Token: 0x06001C06 RID: 7174 RVA: 0x00097764 File Offset: 0x00095964
	private void UpdateZoneInfo(bool roomIsPublic, string zoneName = null)
	{
		AuthenticationValues authenticationValues = this.GetAuthenticationValues();
		Dictionary<string, object> dictionary = ((authenticationValues != null) ? authenticationValues.AuthPostData : null) as Dictionary<string, object>;
		if (dictionary != null)
		{
			dictionary["Zone"] = ((zoneName != null) ? zoneName : ((ZoneManagement.instance.activeZones.Count > 0) ? ZoneManagement.instance.activeZones.First<GTZone>().GetName<GTZone>() : ""));
			dictionary["SubZone"] = GTSubZone.none.GetName<GTSubZone>();
			dictionary["IsPublic"] = roomIsPublic;
			authenticationValues.SetAuthPostData(dictionary);
			this.SetAuthenticationValues(authenticationValues);
		}
	}

	// Token: 0x040025DB RID: 9691
	private NetworkRegionInfo[] regionData;

	// Token: 0x040025DC RID: 9692
	private Task<NetJoinResult> roomTask;

	// Token: 0x040025DD RID: 9693
	private ObjectPool<PunNetPlayer> playerPool;

	// Token: 0x040025DE RID: 9694
	private NetPlayer[] m_allNetPlayers = new NetPlayer[0];

	// Token: 0x040025DF RID: 9695
	private NetPlayer[] m_otherNetPlayers = new NetPlayer[0];

	// Token: 0x040025E0 RID: 9696
	private List<CancellationTokenSource> _taskCancelTokens = new List<CancellationTokenSource>();

	// Token: 0x040025E1 RID: 9697
	private PhotonVoiceNetwork punVoice;

	// Token: 0x040025E2 RID: 9698
	private GameObject VoiceNetworkObject;

	// Token: 0x040025E3 RID: 9699
	private NetworkSystemPUN.InternalState currentState;

	// Token: 0x040025E4 RID: 9700
	private bool firstRoomJoin;

	// Token: 0x02000473 RID: 1139
	private enum InternalState
	{
		// Token: 0x040025E6 RID: 9702
		AwaitingAuth,
		// Token: 0x040025E7 RID: 9703
		Authenticated,
		// Token: 0x040025E8 RID: 9704
		PingGathering,
		// Token: 0x040025E9 RID: 9705
		StateCheckFailed,
		// Token: 0x040025EA RID: 9706
		ConnectingToMaster,
		// Token: 0x040025EB RID: 9707
		ConnectedToMaster,
		// Token: 0x040025EC RID: 9708
		Idle,
		// Token: 0x040025ED RID: 9709
		Internal_Disconnecting,
		// Token: 0x040025EE RID: 9710
		Internal_Disconnected,
		// Token: 0x040025EF RID: 9711
		Searching_Connecting,
		// Token: 0x040025F0 RID: 9712
		Searching_Connected,
		// Token: 0x040025F1 RID: 9713
		Searching_Joining,
		// Token: 0x040025F2 RID: 9714
		Searching_Joined,
		// Token: 0x040025F3 RID: 9715
		Searching_JoinFailed_NotFound,
		// Token: 0x040025F4 RID: 9716
		Searching_JoinFailed_Full,
		// Token: 0x040025F5 RID: 9717
		Searching_JoinFailed_Other,
		// Token: 0x040025F6 RID: 9718
		Searching_Creating,
		// Token: 0x040025F7 RID: 9719
		Searching_Created,
		// Token: 0x040025F8 RID: 9720
		Searching_CreateFailed,
		// Token: 0x040025F9 RID: 9721
		Searching_Disconnecting,
		// Token: 0x040025FA RID: 9722
		Searching_Disconnected
	}
}
