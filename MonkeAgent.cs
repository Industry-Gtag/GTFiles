using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200094E RID: 2382
public class MonkeAgent : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x170005BB RID: 1467
	// (get) Token: 0x06003E7C RID: 15996 RVA: 0x0008FD25 File Offset: 0x0008DF25
	private NetworkRunner runner
	{
		get
		{
			return ((NetworkSystemFusion)NetworkSystem.Instance).runner;
		}
	}

	// Token: 0x170005BC RID: 1468
	// (get) Token: 0x06003E7D RID: 15997 RVA: 0x00151625 File Offset: 0x0014F825
	// (set) Token: 0x06003E7E RID: 15998 RVA: 0x0015162D File Offset: 0x0014F82D
	private bool sendReport
	{
		get
		{
			return this._sendReport;
		}
		set
		{
			if (!this._sendReport)
			{
				this._sendReport = true;
			}
		}
	}

	// Token: 0x170005BD RID: 1469
	// (get) Token: 0x06003E7F RID: 15999 RVA: 0x0015163E File Offset: 0x0014F83E
	// (set) Token: 0x06003E80 RID: 16000 RVA: 0x00151646 File Offset: 0x0014F846
	private string suspiciousPlayerId
	{
		get
		{
			return this._suspiciousPlayerId;
		}
		set
		{
			if (this._suspiciousPlayerId == "")
			{
				this._suspiciousPlayerId = value;
			}
		}
	}

	// Token: 0x170005BE RID: 1470
	// (get) Token: 0x06003E81 RID: 16001 RVA: 0x00151661 File Offset: 0x0014F861
	// (set) Token: 0x06003E82 RID: 16002 RVA: 0x00151669 File Offset: 0x0014F869
	private string suspiciousPlayerName
	{
		get
		{
			return this._suspiciousPlayerName;
		}
		set
		{
			if (this._suspiciousPlayerName == "")
			{
				this._suspiciousPlayerName = value;
			}
		}
	}

	// Token: 0x170005BF RID: 1471
	// (get) Token: 0x06003E83 RID: 16003 RVA: 0x00151684 File Offset: 0x0014F884
	// (set) Token: 0x06003E84 RID: 16004 RVA: 0x0015168C File Offset: 0x0014F88C
	private string suspiciousReason
	{
		get
		{
			return this._suspiciousReason;
		}
		set
		{
			if (this._suspiciousReason == "")
			{
				this._suspiciousReason = value;
			}
		}
	}

	// Token: 0x06003E85 RID: 16005 RVA: 0x00019260 File Offset: 0x00017460
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003E86 RID: 16006 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003E87 RID: 16007 RVA: 0x001516A7 File Offset: 0x0014F8A7
	public void SliceUpdate()
	{
		this.CheckReports();
	}

	// Token: 0x06003E88 RID: 16008 RVA: 0x001516B0 File Offset: 0x0014F8B0
	private void Start()
	{
		if (MonkeAgent.instance == null)
		{
			MonkeAgent.instance = this;
		}
		else if (MonkeAgent.instance != this)
		{
			Object.Destroy(this);
		}
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerEnteredRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		RoomSystem.JoinedRoomEvent += delegate
		{
			this.cachedPlayerList = NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0];
		};
		this.logErrorCount = 0;
		Application.logMessageReceived += this.LogErrorCount;
	}

	// Token: 0x06003E89 RID: 16009 RVA: 0x00151754 File Offset: 0x0014F954
	private void OnApplicationPause(bool paused)
	{
		if (paused || !RoomSystem.JoinedRoom)
		{
			return;
		}
		this.lastServerTimestamp = NetworkSystem.Instance.SimTick;
		this.RefreshRPCs();
	}

	// Token: 0x06003E8A RID: 16010 RVA: 0x00151778 File Offset: 0x0014F978
	public void LogErrorCount(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error)
		{
			this.logErrorCount++;
			this.stringIndex = logString.LastIndexOf("Sender is ");
			if (logString.Contains("RPC") && this.stringIndex >= 0)
			{
				this.playerID = logString.Substring(this.stringIndex + 10);
				this.tempPlayer = null;
				for (int i = 0; i < this.cachedPlayerList.Length; i++)
				{
					if (this.cachedPlayerList[i].UserId == this.playerID)
					{
						this.tempPlayer = this.cachedPlayerList[i];
						break;
					}
				}
				string text = "invalid RPC stuff";
				if (!this.IncrementRPCTracker(in this.tempPlayer, in text, in this.rpcErrorMax))
				{
					this.SendReport("invalid RPC stuff", this.tempPlayer.UserId, this.tempPlayer.NickName);
				}
				this.tempPlayer = null;
			}
			if (this.logErrorCount > this.logErrorMax)
			{
				Debug.unityLogger.logEnabled = false;
			}
		}
	}

	// Token: 0x06003E8B RID: 16011 RVA: 0x0015187C File Offset: 0x0014FA7C
	public void SendReport(string susReason, string susId, string susNick)
	{
		this.suspiciousReason = susReason;
		this.suspiciousPlayerId = susId;
		this.suspiciousPlayerName = susNick;
		this.sendReport = true;
	}

	// Token: 0x06003E8C RID: 16012 RVA: 0x0015189C File Offset: 0x0014FA9C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void DispatchReport()
	{
		if ((this.sendReport || this.testAssault) && this.suspiciousPlayerId != "" && this.reportedPlayers.IndexOf(this.suspiciousPlayerId) == -1)
		{
			if (this._suspiciousPlayerName.Length > 12)
			{
				this._suspiciousPlayerName = this._suspiciousPlayerName.Remove(12);
			}
			this.reportedPlayers.Add(this.suspiciousPlayerId);
			this.testAssault = false;
			WebFlags webFlags = new WebFlags(3);
			NetEventOptions netEventOptions = new NetEventOptions
			{
				TargetActors = MonkeAgent.targetActors,
				Reciever = NetEventOptions.RecieverTarget.master,
				Flags = webFlags
			};
			string[] array = new string[this.cachedPlayerList.Length];
			int num = 0;
			foreach (NetPlayer netPlayer in this.cachedPlayerList)
			{
				array[num] = netPlayer.UserId;
				num++;
			}
			object[] array3 = new object[]
			{
				NetworkSystem.Instance.RoomStringStripped(),
				array,
				NetworkSystem.Instance.MasterClient.UserId,
				this.suspiciousPlayerId,
				this.suspiciousPlayerName,
				this.suspiciousReason,
				NetworkSystemConfig.AppVersion
			};
			NetworkSystemRaiseEvent.RaiseEvent(8, array3, netEventOptions, true);
			if (this.ShouldDisconnectFromRoom())
			{
				base.StartCoroutine(this.QuitDelay(1f));
			}
		}
		this._sendReport = false;
		this._suspiciousPlayerId = "";
		this._suspiciousPlayerName = "";
		this._suspiciousReason = "";
	}

	// Token: 0x06003E8D RID: 16013 RVA: 0x00151A24 File Offset: 0x0014FC24
	private void CheckReports()
	{
		if (Time.time < this.lastCheck + this.reportCheckCooldown)
		{
			return;
		}
		this.lastCheck = Time.time;
		try
		{
			this.logErrorCount = 0;
			if (RoomSystem.JoinedRoom)
			{
				this.lastCheck = Time.time;
				this.lastServerTimestamp = NetworkSystem.Instance.SimTick;
				if (!PhotonNetwork.CurrentRoom.PublishUserId)
				{
					this.sendReport = true;
					this.suspiciousReason = "missing player ids";
					this.SetToRoomCreatorIfHere();
					this.CloseInvalidRoom();
				}
				if ((!RoomSystem.WasRoomSubscription && this.cachedPlayerList.Length > (int)RoomSystem.GetCurrentRoomExpectedSize()) || this.cachedPlayerList.Length > 20)
				{
					this.sendReport = true;
					this.suspiciousReason = "too many players";
					this.SetToRoomCreatorIfHere();
					this.CloseInvalidRoom();
				}
				if (this.currentMasterClient != NetworkSystem.Instance.MasterClient || this.LowestActorNumber() != NetworkSystem.Instance.MasterClient.ActorNumber)
				{
					foreach (NetPlayer netPlayer in this.cachedPlayerList)
					{
						if (this.currentMasterClient == netPlayer)
						{
							this.sendReport = true;
							this.suspiciousReason = "room host force changed";
							this.suspiciousPlayerId = NetworkSystem.Instance.MasterClient.UserId;
							this.suspiciousPlayerName = NetworkSystem.Instance.MasterClient.NickName;
						}
					}
					this.currentMasterClient = NetworkSystem.Instance.MasterClient;
				}
				this.RefreshRPCs();
				this.DispatchReport();
			}
		}
		catch
		{
		}
	}

	// Token: 0x06003E8E RID: 16014 RVA: 0x00151BAC File Offset: 0x0014FDAC
	private void RefreshRPCs()
	{
		foreach (Dictionary<string, MonkeAgent.RPCCallTracker> dictionary in this.userRPCCalls.Values)
		{
			foreach (MonkeAgent.RPCCallTracker rpccallTracker in dictionary.Values)
			{
				rpccallTracker.RPCCalls = 0;
			}
		}
	}

	// Token: 0x06003E8F RID: 16015 RVA: 0x00151C3C File Offset: 0x0014FE3C
	private int LowestActorNumber()
	{
		this.lowestActorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		foreach (NetPlayer netPlayer in this.cachedPlayerList)
		{
			if (netPlayer.ActorNumber < this.lowestActorNumber)
			{
				this.lowestActorNumber = netPlayer.ActorNumber;
			}
		}
		return this.lowestActorNumber;
	}

	// Token: 0x06003E90 RID: 16016 RVA: 0x00151C97 File Offset: 0x0014FE97
	public void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		this.cachedPlayerList = NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0];
	}

	// Token: 0x06003E91 RID: 16017 RVA: 0x00151CB4 File Offset: 0x0014FEB4
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.cachedPlayerList = NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0];
		Dictionary<string, MonkeAgent.RPCCallTracker> dictionary;
		if (this.userRPCCalls.TryGetValue(otherPlayer.UserId, out dictionary))
		{
			this.userRPCCalls.Remove(otherPlayer.UserId);
		}
	}

	// Token: 0x06003E92 RID: 16018 RVA: 0x00151D02 File Offset: 0x0014FF02
	public static void IncrementRPCCall(PhotonMessageInfo info, [CallerMemberName] string callingMethod = "")
	{
		MonkeAgent.IncrementRPCCall(new PhotonMessageInfoWrapped(info), callingMethod);
	}

	// Token: 0x06003E93 RID: 16019 RVA: 0x00151D10 File Offset: 0x0014FF10
	public static void IncrementRPCCall(PhotonMessageInfoWrapped infoWrapped, [CallerMemberName] string callingMethod = "")
	{
		MonkeAgent.instance.IncrementRPCCallLocal(infoWrapped, callingMethod);
	}

	// Token: 0x06003E94 RID: 16020 RVA: 0x00151D20 File Offset: 0x0014FF20
	private void IncrementRPCCallLocal(PhotonMessageInfoWrapped infoWrapped, string rpcFunction)
	{
		if (infoWrapped.sentTick < this.lastServerTimestamp)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(infoWrapped.senderID);
		if (player == null)
		{
			return;
		}
		string userId = player.UserId;
		if (!this.IncrementRPCTracker(in userId, in rpcFunction, in this.rpcCallLimit))
		{
			this.SendReport("too many rpc calls! " + rpcFunction, player.UserId, player.NickName);
			return;
		}
	}

	// Token: 0x06003E95 RID: 16021 RVA: 0x00151D88 File Offset: 0x0014FF88
	private bool IncrementRPCTracker(in NetPlayer sender, in string rpcFunction, in int callLimit)
	{
		string userId = sender.UserId;
		return this.IncrementRPCTracker(in userId, in rpcFunction, in callLimit);
	}

	// Token: 0x06003E96 RID: 16022 RVA: 0x00151DA8 File Offset: 0x0014FFA8
	private bool IncrementRPCTracker(in Player sender, in string rpcFunction, in int callLimit)
	{
		string userId = sender.UserId;
		return this.IncrementRPCTracker(in userId, in rpcFunction, in callLimit);
	}

	// Token: 0x06003E97 RID: 16023 RVA: 0x00151DC8 File Offset: 0x0014FFC8
	private bool IncrementRPCTracker(in string userId, in string rpcFunction, in int callLimit)
	{
		MonkeAgent.RPCCallTracker rpccallTracker = this.GetRPCCallTracker(userId, rpcFunction);
		if (rpccallTracker == null)
		{
			return true;
		}
		rpccallTracker.RPCCalls++;
		if (rpccallTracker.RPCCalls > rpccallTracker.RPCCallsMax)
		{
			rpccallTracker.RPCCallsMax = rpccallTracker.RPCCalls;
		}
		return rpccallTracker.RPCCalls <= callLimit;
	}

	// Token: 0x06003E98 RID: 16024 RVA: 0x00151E1C File Offset: 0x0015001C
	private MonkeAgent.RPCCallTracker GetRPCCallTracker(string userID, string rpcFunction)
	{
		if (userID == null)
		{
			return null;
		}
		MonkeAgent.RPCCallTracker rpccallTracker = null;
		Dictionary<string, MonkeAgent.RPCCallTracker> dictionary;
		if (!this.userRPCCalls.TryGetValue(userID, out dictionary))
		{
			rpccallTracker = new MonkeAgent.RPCCallTracker
			{
				RPCCalls = 0,
				RPCCallsMax = 0
			};
			Dictionary<string, MonkeAgent.RPCCallTracker> dictionary2 = new Dictionary<string, MonkeAgent.RPCCallTracker>();
			dictionary2.Add(rpcFunction, rpccallTracker);
			this.userRPCCalls.Add(userID, dictionary2);
		}
		else if (!dictionary.TryGetValue(rpcFunction, out rpccallTracker))
		{
			rpccallTracker = new MonkeAgent.RPCCallTracker
			{
				RPCCalls = 0,
				RPCCallsMax = 0
			};
			dictionary.Add(rpcFunction, rpccallTracker);
		}
		return rpccallTracker;
	}

	// Token: 0x06003E99 RID: 16025 RVA: 0x00151E99 File Offset: 0x00150099
	private IEnumerator QuitDelay(float time = 1f)
	{
		yield return new WaitForSeconds(1f);
		NetworkSystem.Instance.ReturnToSinglePlayer();
		yield break;
	}

	// Token: 0x06003E9A RID: 16026 RVA: 0x00151EA4 File Offset: 0x001500A4
	private void SetToRoomCreatorIfHere()
	{
		this.tempPlayer = PhotonNetwork.CurrentRoom.GetPlayer(1, false);
		if (this.tempPlayer != null)
		{
			this.suspiciousPlayerId = this.tempPlayer.UserId;
			this.suspiciousPlayerName = this.tempPlayer.NickName;
			return;
		}
		this.suspiciousPlayerId = "n/a";
		this.suspiciousPlayerName = "n/a";
	}

	// Token: 0x06003E9B RID: 16027 RVA: 0x00151F0C File Offset: 0x0015010C
	private bool ShouldDisconnectFromRoom()
	{
		return this._suspiciousReason.Contains("too many players") || this._suspiciousReason.Contains("invalid room name") || this._suspiciousReason.Contains("invalid game mode") || this._suspiciousReason.Contains("missing player ids");
	}

	// Token: 0x06003E9C RID: 16028 RVA: 0x00151F61 File Offset: 0x00150161
	private void CloseInvalidRoom()
	{
		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;
		PhotonNetwork.CurrentRoom.MaxPlayers = RoomSystem.GetCurrentRoomExpectedSize();
	}

	// Token: 0x04004EF0 RID: 20208
	[OnEnterPlay_SetNull]
	public static volatile MonkeAgent instance;

	// Token: 0x04004EF1 RID: 20209
	private bool _sendReport;

	// Token: 0x04004EF2 RID: 20210
	private string _suspiciousPlayerId = "";

	// Token: 0x04004EF3 RID: 20211
	private string _suspiciousPlayerName = "";

	// Token: 0x04004EF4 RID: 20212
	private string _suspiciousReason = "";

	// Token: 0x04004EF5 RID: 20213
	internal List<string> reportedPlayers = new List<string>();

	// Token: 0x04004EF6 RID: 20214
	public byte roomSize;

	// Token: 0x04004EF7 RID: 20215
	public float lastCheck;

	// Token: 0x04004EF8 RID: 20216
	public float userDecayTime = 15f;

	// Token: 0x04004EF9 RID: 20217
	public NetPlayer currentMasterClient;

	// Token: 0x04004EFA RID: 20218
	public bool testAssault;

	// Token: 0x04004EFB RID: 20219
	private const byte ReportAssault = 8;

	// Token: 0x04004EFC RID: 20220
	private int lowestActorNumber;

	// Token: 0x04004EFD RID: 20221
	private int calls;

	// Token: 0x04004EFE RID: 20222
	public int rpcCallLimit = 50;

	// Token: 0x04004EFF RID: 20223
	public int logErrorMax = 50;

	// Token: 0x04004F00 RID: 20224
	public int rpcErrorMax = 10;

	// Token: 0x04004F01 RID: 20225
	private object outObj;

	// Token: 0x04004F02 RID: 20226
	private NetPlayer tempPlayer;

	// Token: 0x04004F03 RID: 20227
	private int logErrorCount;

	// Token: 0x04004F04 RID: 20228
	private int stringIndex;

	// Token: 0x04004F05 RID: 20229
	private string playerID;

	// Token: 0x04004F06 RID: 20230
	private string playerNick;

	// Token: 0x04004F07 RID: 20231
	private int lastServerTimestamp;

	// Token: 0x04004F08 RID: 20232
	private const string InvalidRPC = "invalid RPC stuff";

	// Token: 0x04004F09 RID: 20233
	public NetPlayer[] cachedPlayerList;

	// Token: 0x04004F0A RID: 20234
	private float lastReportChecked;

	// Token: 0x04004F0B RID: 20235
	private float reportCheckCooldown = 1f;

	// Token: 0x04004F0C RID: 20236
	private static int[] targetActors = new int[] { -1 };

	// Token: 0x04004F0D RID: 20237
	private Dictionary<string, Dictionary<string, MonkeAgent.RPCCallTracker>> userRPCCalls = new Dictionary<string, Dictionary<string, MonkeAgent.RPCCallTracker>>();

	// Token: 0x04004F0E RID: 20238
	private Hashtable hashTable;

	// Token: 0x0200094F RID: 2383
	private class RPCCallTracker
	{
		// Token: 0x04004F0F RID: 20239
		public int RPCCalls;

		// Token: 0x04004F10 RID: 20240
		public int RPCCallsMax;
	}
}
