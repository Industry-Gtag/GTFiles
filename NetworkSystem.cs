using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using GorillaNetworking;
using GorillaTag;
using Photon.Realtime;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;

// Token: 0x02000460 RID: 1120
public abstract class NetworkSystem : MonoBehaviour
{
	// Token: 0x170002C0 RID: 704
	// (get) Token: 0x06001AD6 RID: 6870 RVA: 0x00094C8B File Offset: 0x00092E8B
	// (set) Token: 0x06001AD7 RID: 6871 RVA: 0x00094C93 File Offset: 0x00092E93
	public bool groupJoinInProgress { get; protected set; }

	// Token: 0x170002C1 RID: 705
	// (get) Token: 0x06001AD8 RID: 6872 RVA: 0x00094C9C File Offset: 0x00092E9C
	// (set) Token: 0x06001AD9 RID: 6873 RVA: 0x00094CA4 File Offset: 0x00092EA4
	public NetSystemState netState
	{
		get
		{
			return this.testState;
		}
		protected set
		{
			Debug.Log("netstate set to:" + value.ToString());
			this.testState = value;
		}
	}

	// Token: 0x170002C2 RID: 706
	// (get) Token: 0x06001ADA RID: 6874 RVA: 0x00094CC9 File Offset: 0x00092EC9
	public IReadOnlyList<NetPlayer> NetPlayerCache
	{
		get
		{
			return this.netPlayerCache;
		}
	}

	// Token: 0x170002C3 RID: 707
	// (get) Token: 0x06001ADB RID: 6875 RVA: 0x00094CD1 File Offset: 0x00092ED1
	public NetPlayer LocalPlayer
	{
		get
		{
			return this.netPlayerCache.Find((NetPlayer p) => p.IsLocal);
		}
	}

	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x06001ADC RID: 6876 RVA: 0x00094CFD File Offset: 0x00092EFD
	public virtual bool IsMasterClient { get; }

	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x06001ADD RID: 6877 RVA: 0x00094D05 File Offset: 0x00092F05
	public virtual NetPlayer MasterClient
	{
		get
		{
			return this.netPlayerCache.Find((NetPlayer p) => p.IsMasterClient);
		}
	}

	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x06001ADE RID: 6878 RVA: 0x00094D31 File Offset: 0x00092F31
	public Recorder LocalRecorder
	{
		get
		{
			return this.localRecorder;
		}
	}

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x06001ADF RID: 6879 RVA: 0x00094D39 File Offset: 0x00092F39
	public Speaker LocalSpeaker
	{
		get
		{
			return this.localSpeaker;
		}
	}

	// Token: 0x06001AE0 RID: 6880 RVA: 0x00094D41 File Offset: 0x00092F41
	protected void JoinedNetworkRoom()
	{
		VRRigCache.Instance.OnJoinedRoom();
		DelegateListProcessor onJoinedRoomEvent = this.OnJoinedRoomEvent;
		if (onJoinedRoomEvent == null)
		{
			return;
		}
		onJoinedRoomEvent.InvokeSafe();
	}

	// Token: 0x06001AE1 RID: 6881 RVA: 0x00094D5D File Offset: 0x00092F5D
	internal void MultiplayerStarted()
	{
		DelegateListProcessor onMultiplayerStarted = this.OnMultiplayerStarted;
		if (onMultiplayerStarted == null)
		{
			return;
		}
		onMultiplayerStarted.InvokeSafe();
	}

	// Token: 0x06001AE2 RID: 6882 RVA: 0x00094D6F File Offset: 0x00092F6F
	internal void PreLeavingRoom()
	{
		DelegateListProcessor onPreLeavingRoom = this.OnPreLeavingRoom;
		if (onPreLeavingRoom == null)
		{
			return;
		}
		onPreLeavingRoom.InvokeSafe();
	}

	// Token: 0x06001AE3 RID: 6883 RVA: 0x00094D84 File Offset: 0x00092F84
	protected void SinglePlayerStarted()
	{
		try
		{
			DelegateListProcessor onReturnedToSinglePlayer = this.OnReturnedToSinglePlayer;
			if (onReturnedToSinglePlayer != null)
			{
				onReturnedToSinglePlayer.InvokeSafe();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		VRRigCache.Instance.OnLeftRoom();
	}

	// Token: 0x06001AE4 RID: 6884 RVA: 0x00094DC8 File Offset: 0x00092FC8
	protected void PlayerJoined(NetPlayer netPlayer)
	{
		if (this.IsOnline)
		{
			VRRigCache.Instance.OnPlayerEnteredRoom(netPlayer);
			DelegateListProcessor<NetPlayer> onPlayerJoined = this.OnPlayerJoined;
			if (onPlayerJoined == null)
			{
				return;
			}
			onPlayerJoined.InvokeSafe(in netPlayer);
		}
	}

	// Token: 0x06001AE5 RID: 6885 RVA: 0x00094DF0 File Offset: 0x00092FF0
	protected void PlayerLeft(NetPlayer netPlayer)
	{
		try
		{
			DelegateListProcessor<NetPlayer> onPlayerLeft = this.OnPlayerLeft;
			if (onPlayerLeft != null)
			{
				onPlayerLeft.InvokeSafe(in netPlayer);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		VRRigCache.Instance.OnPlayerLeftRoom(netPlayer);
	}

	// Token: 0x06001AE6 RID: 6886 RVA: 0x00094E34 File Offset: 0x00093034
	protected void OnMasterClientSwitchedCallback(NetPlayer nMaster)
	{
		DelegateListProcessor<NetPlayer> onMasterClientSwitchedEvent = this.OnMasterClientSwitchedEvent;
		if (onMasterClientSwitchedEvent == null)
		{
			return;
		}
		onMasterClientSwitchedEvent.InvokeSafe(in nMaster);
	}

	// Token: 0x14000039 RID: 57
	// (add) Token: 0x06001AE7 RID: 6887 RVA: 0x00094E48 File Offset: 0x00093048
	// (remove) Token: 0x06001AE8 RID: 6888 RVA: 0x00094E80 File Offset: 0x00093080
	public event Action<byte, object, int> OnRaiseEvent;

	// Token: 0x06001AE9 RID: 6889 RVA: 0x00094EB5 File Offset: 0x000930B5
	internal void RaiseEvent(byte eventCode, object data, int source)
	{
		Action<byte, object, int> onRaiseEvent = this.OnRaiseEvent;
		if (onRaiseEvent == null)
		{
			return;
		}
		onRaiseEvent(eventCode, data, source);
	}

	// Token: 0x1400003A RID: 58
	// (add) Token: 0x06001AEA RID: 6890 RVA: 0x00094ECC File Offset: 0x000930CC
	// (remove) Token: 0x06001AEB RID: 6891 RVA: 0x00094F04 File Offset: 0x00093104
	public event Action<Dictionary<string, object>> OnCustomAuthenticationResponse;

	// Token: 0x06001AEC RID: 6892 RVA: 0x00094F39 File Offset: 0x00093139
	internal void CustomAuthenticationResponse(Dictionary<string, object> response)
	{
		PersistLog.Log("Custom authentication succeeded");
		Action<Dictionary<string, object>> onCustomAuthenticationResponse = this.OnCustomAuthenticationResponse;
		if (onCustomAuthenticationResponse == null)
		{
			return;
		}
		onCustomAuthenticationResponse(response);
	}

	// Token: 0x06001AED RID: 6893 RVA: 0x00094F56 File Offset: 0x00093156
	internal void CustomAuthenticationFailed(string debugMessage)
	{
		PersistLog.Log("Custom authentication failed: " + debugMessage);
	}

	// Token: 0x06001AEE RID: 6894 RVA: 0x00094F68 File Offset: 0x00093168
	public virtual void Initialise()
	{
		Debug.Log("INITIALISING NETWORKSYSTEMS");
		if (NetworkSystem.Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		NetworkSystem.Instance = this;
		NetCrossoverUtils.Prewarm();
	}

	// Token: 0x06001AEF RID: 6895 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void Update()
	{
	}

	// Token: 0x06001AF0 RID: 6896 RVA: 0x00094F97 File Offset: 0x00093197
	public void RegisterSceneNetworkItem(GameObject item)
	{
		if (!this.SceneObjectsToAttach.Contains(item))
		{
			this.SceneObjectsToAttach.Add(item);
		}
	}

	// Token: 0x06001AF1 RID: 6897 RVA: 0x00094FB3 File Offset: 0x000931B3
	public virtual void AttachObjectInGame(GameObject item)
	{
		this.RegisterSceneNetworkItem(item);
	}

	// Token: 0x06001AF2 RID: 6898 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void DetatchSceneObjectInGame(GameObject item)
	{
	}

	// Token: 0x06001AF3 RID: 6899 RVA: 0x00094FBC File Offset: 0x000931BC
	public virtual AuthenticationValues GetAuthenticationValues()
	{
		Debug.LogWarning("NetworkSystem.GetAuthenticationValues should be overridden");
		return new AuthenticationValues();
	}

	// Token: 0x06001AF4 RID: 6900 RVA: 0x00094FCD File Offset: 0x000931CD
	public virtual void SetAuthenticationValues(AuthenticationValues authValues)
	{
		Debug.LogWarning("NetworkSystem.SetAuthenticationValues should be overridden");
	}

	// Token: 0x06001AF5 RID: 6901
	public abstract void FinishAuthenticating();

	// Token: 0x06001AF6 RID: 6902
	public abstract Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1);

	// Token: 0x06001AF7 RID: 6903
	public abstract Task JoinFriendsRoom(string userID, int actorID, string keyToFollow, string shufflerToFollow);

	// Token: 0x06001AF8 RID: 6904
	public abstract Task ReturnToSinglePlayer();

	// Token: 0x06001AF9 RID: 6905
	public abstract void JoinPubWithFriends();

	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x06001AFA RID: 6906 RVA: 0x00094FD9 File Offset: 0x000931D9
	public bool WrongVersion
	{
		get
		{
			return this.isWrongVersion;
		}
	}

	// Token: 0x06001AFB RID: 6907 RVA: 0x00094FE1 File Offset: 0x000931E1
	public void SetWrongVersion()
	{
		this.isWrongVersion = true;
	}

	// Token: 0x06001AFC RID: 6908 RVA: 0x00094FEA File Offset: 0x000931EA
	public GameObject NetInstantiate(GameObject prefab, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, Vector3.zero, Quaternion.identity, false);
	}

	// Token: 0x06001AFD RID: 6909 RVA: 0x00094FFE File Offset: 0x000931FE
	public GameObject NetInstantiate(GameObject prefab, Vector3 position, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, position, Quaternion.identity, false);
	}

	// Token: 0x06001AFE RID: 6910
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false);

	// Token: 0x06001AFF RID: 6911
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false);

	// Token: 0x06001B00 RID: 6912
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject, byte group = 0, object[] data = null, NetworkRunner.OnBeforeSpawned callback = null);

	// Token: 0x06001B01 RID: 6913
	public abstract void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null);

	// Token: 0x06001B02 RID: 6914
	public abstract void NetDestroy(GameObject instance);

	// Token: 0x06001B03 RID: 6915
	public abstract void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true);

	// Token: 0x06001B04 RID: 6916
	public abstract void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true) where T : struct;

	// Token: 0x06001B05 RID: 6917
	public abstract void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true);

	// Token: 0x06001B06 RID: 6918
	public abstract void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod);

	// Token: 0x06001B07 RID: 6919
	public abstract void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args) where T : struct;

	// Token: 0x06001B08 RID: 6920
	public abstract void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message);

	// Token: 0x06001B09 RID: 6921 RVA: 0x00095010 File Offset: 0x00093210
	public static string GetRandomRoomName()
	{
		string text = "";
		for (int i = 0; i < 4; i++)
		{
			text += "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Length), 1);
		}
		if (GorillaComputer.instance.IsPlayerInVirtualStump())
		{
			text = GorillaComputer.instance.VStumpRoomFullPrepend + text;
		}
		if (GorillaComputer.instance.CheckAutoBanListForName(text))
		{
			return text;
		}
		return NetworkSystem.GetRandomRoomName();
	}

	// Token: 0x06001B0A RID: 6922
	public abstract string GetRandomWeightedRegion();

	// Token: 0x06001B0B RID: 6923 RVA: 0x00095088 File Offset: 0x00093288
	protected async Task RefreshNonce()
	{
		this.nonceRefreshed = false;
		PlayFabAuthenticator.instance.RefreshSteamAuthTicketForPhoton(new Action<string>(this.GetSteamAuthTicketSuccessCallback), new Action<EResult>(this.GetSteamAuthTicketFailureCallback));
		while (!this.nonceRefreshed)
		{
			await Task.Yield();
		}
	}

	// Token: 0x06001B0C RID: 6924 RVA: 0x000950CC File Offset: 0x000932CC
	private void GetSteamAuthTicketSuccessCallback(string ticket)
	{
		AuthenticationValues authenticationValues = this.GetAuthenticationValues();
		Dictionary<string, object> dictionary = ((authenticationValues != null) ? authenticationValues.AuthPostData : null) as Dictionary<string, object>;
		if (dictionary != null)
		{
			dictionary["Nonce"] = ticket;
			authenticationValues.SetAuthPostData(dictionary);
			this.SetAuthenticationValues(authenticationValues);
			this.nonceRefreshed = true;
		}
	}

	// Token: 0x06001B0D RID: 6925 RVA: 0x00095116 File Offset: 0x00093316
	private void GetSteamAuthTicketFailureCallback(EResult result)
	{
		base.StartCoroutine(this.ReGetNonce());
	}

	// Token: 0x06001B0E RID: 6926 RVA: 0x00095125 File Offset: 0x00093325
	private IEnumerator ReGetNonce()
	{
		yield return new WaitForSecondsRealtime(3f);
		PlayFabAuthenticator.instance.RefreshSteamAuthTicketForPhoton(new Action<string>(this.GetSteamAuthTicketSuccessCallback), new Action<EResult>(this.GetSteamAuthTicketFailureCallback));
		yield return null;
		yield break;
	}

	// Token: 0x06001B0F RID: 6927 RVA: 0x00095134 File Offset: 0x00093334
	public void BroadcastMyRoom(bool create, string key, string shuffler)
	{
		string text = NetworkSystem.ShuffleRoomName(NetworkSystem.Instance.RoomName, shuffler.Substring(2, 8), true) + "|" + NetworkSystem.ShuffleRoomName("ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(NetworkSystem.Instance.currentRegionIndex, 1), shuffler.Substring(0, 2), true);
		GorillaServer instance = GorillaServer.Instance;
		BroadcastMyRoomRequest broadcastMyRoomRequest = new BroadcastMyRoomRequest();
		broadcastMyRoomRequest.KeyToFollow = key;
		broadcastMyRoomRequest.RoomToJoin = text;
		broadcastMyRoomRequest.Set = create;
		instance.BroadcastMyRoom(broadcastMyRoomRequest, delegate(ExecuteFunctionResult result)
		{
		}, delegate(PlayFabError error)
		{
		});
	}

	// Token: 0x06001B10 RID: 6928 RVA: 0x000951EC File Offset: 0x000933EC
	public bool InstantCheckGroupData(string userID, string keyToFollow)
	{
		bool success = false;
		global::PlayFab.ClientModels.GetSharedGroupDataRequest getSharedGroupDataRequest = new global::PlayFab.ClientModels.GetSharedGroupDataRequest();
		getSharedGroupDataRequest.Keys = new List<string> { keyToFollow };
		getSharedGroupDataRequest.SharedGroupId = userID;
		PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, delegate(GetSharedGroupDataResult result)
		{
			if (result.Data.Count > 0)
			{
				success = true;
				return;
			}
		}, delegate(PlayFabError error)
		{
		}, null, null);
		return success;
	}

	// Token: 0x06001B11 RID: 6929 RVA: 0x0009525C File Offset: 0x0009345C
	public NetPlayer GetNetPlayerByID(int playerActorNumber)
	{
		return this.netPlayerCache.Find((NetPlayer a) => a.ActorNumber == playerActorNumber);
	}

	// Token: 0x06001B12 RID: 6930 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void NetRaiseEventReliable(byte eventCode, object data)
	{
	}

	// Token: 0x06001B13 RID: 6931 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void NetRaiseEventUnreliable(byte eventCode, object data)
	{
	}

	// Token: 0x06001B14 RID: 6932 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void NetRaiseEventReliable(byte eventCode, object data, NetEventOptions options)
	{
	}

	// Token: 0x06001B15 RID: 6933 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void NetRaiseEventUnreliable(byte eventCode, object data, NetEventOptions options)
	{
	}

	// Token: 0x06001B16 RID: 6934 RVA: 0x00095290 File Offset: 0x00093490
	public static string ShuffleRoomName(string room, string shuffle, bool encode)
	{
		NetworkSystem.shuffleStringBuilder.Clear();
		int num;
		if (!int.TryParse(shuffle, out num))
		{
			Debug.Log("Shuffle room failed");
			return "";
		}
		for (int i = 0; i < room.Length; i++)
		{
			int num2 = int.Parse(shuffle.Substring(i * 2 % (shuffle.Length - 1), 2));
			int num3 = NetworkSystem.mod("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".IndexOf(room[i]) + (encode ? num2 : (-num2)), "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".Length);
			NetworkSystem.shuffleStringBuilder.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"[num3]);
		}
		return NetworkSystem.shuffleStringBuilder.ToString();
	}

	// Token: 0x06001B17 RID: 6935 RVA: 0x0004CAF1 File Offset: 0x0004ACF1
	public static int mod(int x, int m)
	{
		return (x % m + m) % m;
	}

	// Token: 0x06001B18 RID: 6936
	public abstract Task AwaitSceneReady();

	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x06001B19 RID: 6937
	public abstract string CurrentPhotonBackend { get; }

	// Token: 0x06001B1A RID: 6938
	public abstract NetPlayer GetLocalPlayer();

	// Token: 0x06001B1B RID: 6939
	public abstract NetPlayer GetPlayer(int PlayerID);

	// Token: 0x06001B1C RID: 6940 RVA: 0x00095338 File Offset: 0x00093538
	public NetPlayer GetPlayer(Player punPlayer)
	{
		if (punPlayer == null)
		{
			return null;
		}
		NetPlayer netPlayer = this.FindPlayer(punPlayer);
		if (netPlayer == null)
		{
			this.UpdatePlayers();
			netPlayer = this.FindPlayer(punPlayer);
			if (netPlayer == null)
			{
				Debug.LogError(string.Format("There is no NetPlayer with this ID currently in game. Passed ID: {0} nickname {1}", punPlayer.ActorNumber, punPlayer.NickName));
				return null;
			}
		}
		return netPlayer;
	}

	// Token: 0x06001B1D RID: 6941 RVA: 0x0009538C File Offset: 0x0009358C
	private NetPlayer FindPlayer(Player punPlayer)
	{
		for (int i = 0; i < this.netPlayerCache.Count; i++)
		{
			if (this.netPlayerCache[i].GetPlayerRef() == punPlayer)
			{
				return this.netPlayerCache[i];
			}
		}
		return null;
	}

	// Token: 0x06001B1E RID: 6942 RVA: 0x00036275 File Offset: 0x00034475
	public NetPlayer GetPlayer(PlayerRef playerRef)
	{
		return null;
	}

	// Token: 0x06001B1F RID: 6943
	public abstract void SetMyNickName(string name);

	// Token: 0x06001B20 RID: 6944
	public abstract string GetMyNickName();

	// Token: 0x06001B21 RID: 6945
	public abstract string GetMyDefaultName();

	// Token: 0x06001B22 RID: 6946
	public abstract string GetNickName(int playerID);

	// Token: 0x06001B23 RID: 6947
	public abstract string GetNickName(NetPlayer player);

	// Token: 0x06001B24 RID: 6948
	public abstract string GetMyUserID();

	// Token: 0x06001B25 RID: 6949
	public abstract string GetUserID(int playerID);

	// Token: 0x06001B26 RID: 6950
	public abstract string GetUserID(NetPlayer player);

	// Token: 0x06001B27 RID: 6951
	public abstract void SetMyTutorialComplete();

	// Token: 0x06001B28 RID: 6952
	public abstract bool GetMyTutorialCompletion();

	// Token: 0x06001B29 RID: 6953
	public abstract bool GetPlayerTutorialCompletion(int playerID);

	// Token: 0x06001B2A RID: 6954 RVA: 0x000953D1 File Offset: 0x000935D1
	public string GetMyPlatform()
	{
		PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
		string text;
		if (instance == null)
		{
			text = null;
		}
		else
		{
			PlatformTagJoin platform = instance.platform;
			text = ((platform != null) ? platform.ToString() : null);
		}
		return text ?? "";
	}

	// Token: 0x06001B2B RID: 6955 RVA: 0x000953FB File Offset: 0x000935FB
	public string GetPlayerPlatform(int playerID)
	{
		return this.GetPlayerPlatform(this.GetPlayer(playerID));
	}

	// Token: 0x06001B2C RID: 6956 RVA: 0x0009540A File Offset: 0x0009360A
	public string GetPlayerMothershipId(int playerID)
	{
		return this.GetPlayerMothershipId(this.GetPlayer(playerID));
	}

	// Token: 0x06001B2D RID: 6957
	public abstract string GetPlayerPlatform(NetPlayer player);

	// Token: 0x06001B2E RID: 6958
	public abstract string GetPlayerMothershipId(NetPlayer player);

	// Token: 0x06001B2F RID: 6959 RVA: 0x00095419 File Offset: 0x00093619
	public void AddVoiceSettings(SO_NetworkVoiceSettings settings)
	{
		this.VoiceSettings = settings;
	}

	// Token: 0x06001B30 RID: 6960
	public abstract void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback);

	// Token: 0x170002CA RID: 714
	// (get) Token: 0x06001B31 RID: 6961
	public abstract VoiceConnection VoiceConnection { get; }

	// Token: 0x170002CB RID: 715
	// (get) Token: 0x06001B32 RID: 6962
	public abstract bool IsOnline { get; }

	// Token: 0x170002CC RID: 716
	// (get) Token: 0x06001B33 RID: 6963
	public abstract bool InRoom { get; }

	// Token: 0x170002CD RID: 717
	// (get) Token: 0x06001B34 RID: 6964
	public abstract string RoomName { get; }

	// Token: 0x06001B35 RID: 6965
	public abstract string RoomStringStripped();

	// Token: 0x06001B36 RID: 6966 RVA: 0x00095424 File Offset: 0x00093624
	public string RoomString()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", new object[]
		{
			this.RoomName,
			this.CurrentRoom.isPublic ? "visible" : "hidden",
			this.CurrentRoom.isJoinable ? "open" : "closed",
			this.CurrentRoom.MaxPlayers,
			this.RoomPlayerCount,
			this.CurrentRoom.CustomProps.ToStringFull()
		});
	}

	// Token: 0x170002CE RID: 718
	// (get) Token: 0x06001B37 RID: 6967
	public abstract string GameModeString { get; }

	// Token: 0x170002CF RID: 719
	// (get) Token: 0x06001B38 RID: 6968
	public abstract string CurrentRegion { get; }

	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x06001B39 RID: 6969
	public abstract bool SessionIsPrivate { get; }

	// Token: 0x170002D1 RID: 721
	// (get) Token: 0x06001B3A RID: 6970
	public abstract bool SessionIsSubscription { get; }

	// Token: 0x170002D2 RID: 722
	// (get) Token: 0x06001B3B RID: 6971
	public abstract int LocalPlayerID { get; }

	// Token: 0x170002D3 RID: 723
	// (get) Token: 0x06001B3C RID: 6972 RVA: 0x000954B6 File Offset: 0x000936B6
	public virtual NetPlayer[] AllNetPlayers
	{
		get
		{
			return this.netPlayerCache.ToArray();
		}
	}

	// Token: 0x170002D4 RID: 724
	// (get) Token: 0x06001B3D RID: 6973 RVA: 0x000954C3 File Offset: 0x000936C3
	public virtual NetPlayer[] PlayerListOthers
	{
		get
		{
			return this.netPlayerCache.FindAll((NetPlayer p) => !p.IsLocal).ToArray();
		}
	}

	// Token: 0x06001B3E RID: 6974
	protected abstract void UpdateNetPlayerList();

	// Token: 0x06001B3F RID: 6975 RVA: 0x000954F4 File Offset: 0x000936F4
	public void UpdatePlayers()
	{
		this.UpdateNetPlayerList();
	}

	// Token: 0x170002D5 RID: 725
	// (get) Token: 0x06001B40 RID: 6976
	public abstract double SimTime { get; }

	// Token: 0x170002D6 RID: 726
	// (get) Token: 0x06001B41 RID: 6977
	public abstract float SimDeltaTime { get; }

	// Token: 0x170002D7 RID: 727
	// (get) Token: 0x06001B42 RID: 6978
	public abstract int SimTick { get; }

	// Token: 0x170002D8 RID: 728
	// (get) Token: 0x06001B43 RID: 6979
	public abstract int TickRate { get; }

	// Token: 0x170002D9 RID: 729
	// (get) Token: 0x06001B44 RID: 6980
	public abstract int ServerTimestamp { get; }

	// Token: 0x170002DA RID: 730
	// (get) Token: 0x06001B45 RID: 6981
	public abstract int RoomPlayerCount { get; }

	// Token: 0x06001B46 RID: 6982
	public abstract int GlobalPlayerCount();

	// Token: 0x170002DB RID: 731
	// (get) Token: 0x06001B47 RID: 6983 RVA: 0x000954FC File Offset: 0x000936FC
	// (set) Token: 0x06001B48 RID: 6984 RVA: 0x00095504 File Offset: 0x00093704
	public RoomConfig CurrentRoom { get; protected set; }

	// Token: 0x06001B49 RID: 6985
	public abstract bool IsObjectLocallyOwned(GameObject obj);

	// Token: 0x06001B4A RID: 6986
	public abstract bool IsObjectRoomObject(GameObject obj);

	// Token: 0x06001B4B RID: 6987
	public abstract bool ShouldUpdateObject(GameObject obj);

	// Token: 0x06001B4C RID: 6988
	public abstract bool ShouldWriteObjectData(GameObject obj);

	// Token: 0x06001B4D RID: 6989
	public abstract int GetOwningPlayerID(GameObject obj);

	// Token: 0x06001B4E RID: 6990
	public abstract bool ShouldSpawnLocally(int playerID);

	// Token: 0x06001B4F RID: 6991
	public abstract bool IsTotalAuthority();

	// Token: 0x0400257C RID: 9596
	public static NetworkSystem Instance;

	// Token: 0x0400257D RID: 9597
	public NetworkSystemConfig config;

	// Token: 0x0400257E RID: 9598
	public bool changingSceneManually;

	// Token: 0x0400257F RID: 9599
	public string[] regionNames;

	// Token: 0x04002580 RID: 9600
	public int currentRegionIndex;

	// Token: 0x04002582 RID: 9602
	private bool nonceRefreshed;

	// Token: 0x04002583 RID: 9603
	protected bool isWrongVersion;

	// Token: 0x04002584 RID: 9604
	private NetSystemState testState;

	// Token: 0x04002585 RID: 9605
	protected List<NetPlayer> netPlayerCache = new List<NetPlayer>();

	// Token: 0x04002586 RID: 9606
	protected Recorder localRecorder;

	// Token: 0x04002587 RID: 9607
	protected Speaker localSpeaker;

	// Token: 0x04002589 RID: 9609
	public List<GameObject> SceneObjectsToAttach = new List<GameObject>();

	// Token: 0x0400258A RID: 9610
	protected SO_NetworkVoiceSettings VoiceSettings;

	// Token: 0x0400258B RID: 9611
	protected List<Action<RemoteVoiceLink>> remoteVoiceAddedCallbacks = new List<Action<RemoteVoiceLink>>();

	// Token: 0x0400258C RID: 9612
	public DelegateListProcessor OnJoinedRoomEvent = new DelegateListProcessor();

	// Token: 0x0400258D RID: 9613
	public DelegateListProcessor OnMultiplayerStarted = new DelegateListProcessor();

	// Token: 0x0400258E RID: 9614
	public DelegateListProcessor OnReturnedToSinglePlayer = new DelegateListProcessor();

	// Token: 0x0400258F RID: 9615
	public DelegateListProcessor OnPreLeavingRoom = new DelegateListProcessor();

	// Token: 0x04002590 RID: 9616
	public DelegateListProcessor<NetPlayer> OnPlayerJoined = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04002591 RID: 9617
	public DelegateListProcessor<NetPlayer> OnPlayerLeft = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04002592 RID: 9618
	internal DelegateListProcessor<NetPlayer> OnMasterClientSwitchedEvent = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04002595 RID: 9621
	protected static readonly byte[] EmptyArgs = new byte[0];

	// Token: 0x04002596 RID: 9622
	public const string roomCharacters = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";

	// Token: 0x04002597 RID: 9623
	public const string shuffleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

	// Token: 0x04002598 RID: 9624
	private static StringBuilder shuffleStringBuilder = new StringBuilder(4);

	// Token: 0x04002599 RID: 9625
	protected static StringBuilder reusableSB = new StringBuilder();

	// Token: 0x0400259A RID: 9626
	[NonSerialized]
	public string groupJoinOverrideGameMode = "";

	// Token: 0x02000461 RID: 1121
	// (Invoke) Token: 0x06001B53 RID: 6995
	public delegate void RPC(byte[] data);

	// Token: 0x02000462 RID: 1122
	// (Invoke) Token: 0x06001B57 RID: 6999
	public delegate void StringRPC(string message);

	// Token: 0x02000463 RID: 1123
	// (Invoke) Token: 0x06001B5B RID: 7003
	public delegate void StaticRPC(byte[] data);

	// Token: 0x02000464 RID: 1124
	// (Invoke) Token: 0x06001B5F RID: 7007
	public delegate void StaticRPCPlaceholder(byte[] args);
}
