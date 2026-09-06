using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Timers;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Cosmetics;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using TagEffects;
using UnityEngine;
using Voxels;

// Token: 0x02000D39 RID: 3385
internal class RoomSystem : MonoBehaviour
{
	// Token: 0x060053BF RID: 21439 RVA: 0x001B8EE8 File Offset: 0x001B70E8
	internal static void DeserializeLaunchProjectile(object[] projectileData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "LaunchSlingshotProjectile");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return;
		}
		byte b = Convert.ToByte(projectileData[5]);
		byte b2 = Convert.ToByte(projectileData[6]);
		byte b3 = Convert.ToByte(projectileData[7]);
		byte b4 = Convert.ToByte(projectileData[8]);
		Color32 color = new Color32(b, b2, b3, b4);
		Vector3 vector = (Vector3)projectileData[0];
		Vector3 vector2 = (Vector3)projectileData[1];
		float num = 10000f;
		if ((in vector).IsValid(in num))
		{
			float num2 = 10000f;
			if ((in vector2).IsValid(in num2) && float.IsFinite((float)b) && float.IsFinite((float)b2) && float.IsFinite((float)b3) && float.IsFinite((float)b4))
			{
				RoomSystem.ProjectileSource projectileSource = (RoomSystem.ProjectileSource)Convert.ToInt32(projectileData[2]);
				int num3 = Convert.ToInt32(projectileData[3]);
				bool flag = Convert.ToBoolean(projectileData[4]);
				VRRig rig = rigContainer.Rig;
				if (rig.isOfflineVRRig || rig.IsPositionInRange(vector, 4f))
				{
					RoomSystem.launchProjectile.targetRig = rig;
					RoomSystem.launchProjectile.position = vector;
					RoomSystem.launchProjectile.velocity = vector2;
					RoomSystem.launchProjectile.overridecolour = flag;
					RoomSystem.launchProjectile.colour = color;
					RoomSystem.launchProjectile.projectileIndex = num3;
					RoomSystem.launchProjectile.projectileSource = projectileSource;
					RoomSystem.launchProjectile.messageInfo = info;
					FXSystem.PlayFXForRig(FXType.Projectile, RoomSystem.launchProjectile, info);
				}
				return;
			}
		}
		MonkeAgent.instance.SendReport("invalid projectile state", player.UserId, player.NickName);
	}

	// Token: 0x060053C0 RID: 21440 RVA: 0x001B9080 File Offset: 0x001B7280
	internal static void SendLaunchProjectile(Vector3 position, Vector3 velocity, RoomSystem.ProjectileSource projectileSource, int projectileCount, bool randomColour, byte r, byte g, byte b, byte a)
	{
		if (!RoomSystem.JoinedRoom)
		{
			return;
		}
		RoomSystem.projectileSendData[0] = position;
		RoomSystem.projectileSendData[1] = velocity;
		RoomSystem.projectileSendData[2] = projectileSource;
		RoomSystem.projectileSendData[3] = projectileCount;
		RoomSystem.projectileSendData[4] = randomColour;
		RoomSystem.projectileSendData[5] = r;
		RoomSystem.projectileSendData[6] = g;
		RoomSystem.projectileSendData[7] = b;
		RoomSystem.projectileSendData[8] = a;
		RoomSystem.SendEvent(0, RoomSystem.projectileSendData, in NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x060053C1 RID: 21441 RVA: 0x001B9120 File Offset: 0x001B7320
	internal static void ImpactEffect(VRRig targetRig, Vector3 position, float r, float g, float b, float a, int projectileCount, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		RoomSystem.impactEffect.targetRig = targetRig;
		RoomSystem.impactEffect.position = position;
		RoomSystem.impactEffect.colour = new Color(r, g, b, a);
		RoomSystem.impactEffect.projectileIndex = projectileCount;
		FXSystem.PlayFXForRig(FXType.Impact, RoomSystem.impactEffect, info);
	}

	// Token: 0x060053C2 RID: 21442 RVA: 0x001B9174 File Offset: 0x001B7374
	internal static void DeserializeImpactEffect(object[] impactData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "SpawnSlingshotPlayerImpactEffect");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || rigContainer.Rig.projectileWeapon.IsNull())
		{
			return;
		}
		float num = Convert.ToSingle(impactData[1]);
		float num2 = Convert.ToSingle(impactData[2]);
		float num3 = Convert.ToSingle(impactData[3]);
		float num4 = Convert.ToSingle(impactData[4]);
		Vector3 vector = (Vector3)impactData[0];
		float num5 = 10000f;
		if (!(in vector).IsValid(in num5) || !float.IsFinite(num) || !float.IsFinite(num2) || !float.IsFinite(num3) || !float.IsFinite(num4))
		{
			MonkeAgent.instance.SendReport("invalid impact state", player.UserId, player.NickName);
			return;
		}
		int num6 = Convert.ToInt32(impactData[5]);
		RoomSystem.ImpactEffect(rigContainer.Rig, vector, num, num2, num3, num4, num6, info);
	}

	// Token: 0x060053C3 RID: 21443 RVA: 0x001B9264 File Offset: 0x001B7464
	internal static void SendImpactEffect(Vector3 position, float r, float g, float b, float a, int projectileCount)
	{
		RoomSystem.ImpactEffect(VRRigCache.Instance.localRig.Rig, position, r, g, b, a, projectileCount, default(PhotonMessageInfoWrapped));
		if (RoomSystem.joinedRoom)
		{
			RoomSystem.impactSendData[0] = position;
			RoomSystem.impactSendData[1] = r;
			RoomSystem.impactSendData[2] = g;
			RoomSystem.impactSendData[3] = b;
			RoomSystem.impactSendData[4] = a;
			RoomSystem.impactSendData[5] = projectileCount;
			RoomSystem.SendEvent(1, RoomSystem.impactSendData, in NetworkSystemRaiseEvent.neoOthers, false);
		}
	}

	// Token: 0x060053C4 RID: 21444 RVA: 0x001B92FE File Offset: 0x001B74FE
	internal static void SendLavaSync(byte zone, byte state, double stateStartTime, float activationProgress, int voteCount, int[] votePlayerIds)
	{
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.PackLavaSyncData(zone, state, stateStartTime, activationProgress, voteCount, votePlayerIds);
		RoomSystem.SendEvent(12, RoomSystem.lavaSyncSendData, in NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x060053C5 RID: 21445 RVA: 0x001B9327 File Offset: 0x001B7527
	internal static void SendLavaSyncToPlayer(byte zone, byte state, double stateStartTime, float activationProgress, int voteCount, int[] votePlayerIds, NetPlayer target)
	{
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.PackLavaSyncData(zone, state, stateStartTime, activationProgress, voteCount, votePlayerIds);
		RoomSystem.SendEvent(12, RoomSystem.lavaSyncSendData, in target, false);
	}

	// Token: 0x060053C6 RID: 21446 RVA: 0x001B9350 File Offset: 0x001B7550
	private static void PackLavaSyncData(byte zone, byte state, double stateStartTime, float activationProgress, int voteCount, int[] votePlayerIds)
	{
		RoomSystem.lavaSyncSendData[0] = zone;
		RoomSystem.lavaSyncSendData[1] = state;
		RoomSystem.lavaSyncSendData[2] = stateStartTime;
		RoomSystem.lavaSyncSendData[3] = activationProgress;
		RoomSystem.lavaSyncSendData[4] = voteCount;
		for (int i = 0; i < 20; i++)
		{
			RoomSystem.lavaSyncSendData[5 + i] = votePlayerIds[i];
		}
	}

	// Token: 0x060053C7 RID: 21447 RVA: 0x001B93C0 File Offset: 0x001B75C0
	private unsafe static void DeserializeLavaSync(object[] data, PhotonMessageInfoWrapped info)
	{
		NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "DeserializeLavaSync");
		if (!RoomSystem.callbackInstance.roomSettings.LavaSyncLimiter.CheckCallServerTime(info.SentServerTime))
		{
			Debug.LogWarning(string.Format("[RoomSystem] LavaSync dropped by rate limiter: sender={0} sentTime={1:F3} photonTime={2:F3}", info.senderID, info.SentServerTime, PhotonNetwork.Time));
			return;
		}
		if (data != null && data.Length >= 25)
		{
			object obj = data[0];
			if (obj is byte)
			{
				byte b = (byte)obj;
				obj = data[1];
				if (obj is byte)
				{
					byte b2 = (byte)obj;
					obj = data[2];
					if (obj is double)
					{
						double num = (double)obj;
						obj = data[3];
						if (obj is float)
						{
							float num2 = (float)obj;
							obj = data[4];
							if (obj is int)
							{
								int num3 = (int)obj;
								for (int i = 0; i < 20; i++)
								{
									if (!(data[5 + i] is int))
									{
										return;
									}
								}
								if (b2 > 4)
								{
									return;
								}
								RoomSystem.LavaSyncEventData lavaSyncEventData;
								lavaSyncEventData.zone = b;
								lavaSyncEventData.state = b2;
								lavaSyncEventData.stateStartTime = num.GetFinite();
								lavaSyncEventData.activationProgress = num2.ClampSafe(0f, 2f);
								lavaSyncEventData.voteCount = Mathf.Clamp(num3, 0, 20);
								lavaSyncEventData.senderActorNumber = info.senderID;
								for (int j = 0; j < 20; j++)
								{
									*((ref lavaSyncEventData.votes.FixedElementField) + (IntPtr)j * 4) = (int)data[5 + j];
								}
								Action<RoomSystem.LavaSyncEventData> onLavaSyncReceived = RoomSystem.OnLavaSyncReceived;
								if (onLavaSyncReceived == null)
								{
									return;
								}
								onLavaSyncReceived(lavaSyncEventData);
								return;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060053C8 RID: 21448 RVA: 0x001B9569 File Offset: 0x001B7769
	internal static void SendMonkePointsRedeemed(int redeemedPointCount)
	{
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.monkePointsRedeemedSendData[0] = redeemedPointCount;
		RoomSystem.SendEvent(13, RoomSystem.monkePointsRedeemedSendData, in NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x060053C9 RID: 21449 RVA: 0x001B9594 File Offset: 0x001B7794
	private static void DeserializeMonkePointsRedeemed(object[] data, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "BroadcastRedeemQuestPoints");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (player == null)
		{
			return;
		}
		if (data != null && data.Length >= 1)
		{
			object obj = data[0];
			if (obj is int)
			{
				int num = (int)obj;
				num = Mathf.Clamp(num, 0, 50);
				Action<NetPlayer, int> onMonkePointsRedeemedReceived = RoomSystem.OnMonkePointsRedeemedReceived;
				if (onMonkePointsRedeemedReceived == null)
				{
					return;
				}
				onMonkePointsRedeemedReceived(player, num);
				return;
			}
		}
	}

	// Token: 0x060053CA RID: 21450 RVA: 0x001B95FC File Offset: 0x001B77FC
	private void Awake()
	{
		base.transform.SetParent(null, true);
		Object.DontDestroyOnLoad(this);
		RoomSystem.playerImpactEffectPrefab = this.roomSettings.PlayerImpactEffect;
		RoomSystem.callbackInstance = this;
		RoomSystem.disconnectTimer.Interval = (double)(this.roomSettings.PausedDCTimer * 1000);
		RoomSystem.playerEffectDictionary.Clear();
		foreach (RoomSystem.PlayerEffectConfig playerEffectConfig in this.roomSettings.PlayerEffects)
		{
			RoomSystem.playerEffectDictionary.Add(playerEffectConfig.type, playerEffectConfig);
		}
		this.roomSettings.ResyncNetworkTimeTimer.callback = new Action(PhotonNetwork.FetchServerTimestamp);
		RoomSystem.__roomSettings = this.roomSettings;
	}

	// Token: 0x060053CB RID: 21451 RVA: 0x001B96D4 File Offset: 0x001B78D4
	private void Start()
	{
		List<PhotonView> list = new List<PhotonView>(20);
		foreach (PhotonView photonView in PhotonNetwork.PhotonViewCollection)
		{
			if (photonView.IsRoomView)
			{
				list.Add(photonView);
			}
		}
		RoomSystem.sceneViews = list.ToArray();
		NetworkSystem.Instance.OnRaiseEvent += RoomSystem.OnEvent;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerEnteredRoom;
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnLeftRoom;
	}

	// Token: 0x060053CC RID: 21452 RVA: 0x001B97E0 File Offset: 0x001B79E0
	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			RoomSystem.disconnectTimer.Stop();
			return;
		}
		if (RoomSystem.JoinedRoom)
		{
			RoomSystem.disconnectTimer.Start();
		}
	}

	// Token: 0x060053CD RID: 21453 RVA: 0x001B9804 File Offset: 0x001B7A04
	private void OnJoinedRoom()
	{
		RoomSystem.joinedRoom = true;
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			RoomSystem.netPlayersInRoom.Add(netPlayer);
		}
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(RoomSystem.netPlayersInRoom);
		RoomSystem.roomGameMode = NetworkSystem.Instance.GameModeString;
		RoomSystem.WasRoomPrivate = NetworkSystem.Instance.SessionIsPrivate;
		RoomSystem.WasRoomSubscription = NetworkSystem.Instance.SessionIsSubscription;
		RoomSystem.IsVStumpRoom = GorillaComputer.instance.IsVStumpRoomName(NetworkSystem.Instance.RoomName);
		RoomSystem.InitialJoinTrigger = GorillaComputer.instance.GetJoinTriggerFromFullGameModeString(RoomSystem.roomGameMode);
		if (!RoomSystem.WasRoomPrivate)
		{
			RoomSystem.WasRoomSubscription = PhotonNetwork.CurrentRoom.Name.EndsWith(":GTFC");
		}
		bool wasRoomSubscription = RoomSystem.WasRoomSubscription;
		if (NetworkSystem.Instance.IsMasterClient)
		{
			for (int j = 0; j < this.prefabsToInstantiateByPath.Length; j++)
			{
				this.prefabsInstantiated.Add(NetworkSystem.Instance.NetInstantiate(this.prefabsToInstantiate[j], Vector3.zero, Quaternion.identity, true));
			}
		}
		try
		{
			RoomSystem.m_roomSizeOnJoin = PhotonNetwork.CurrentRoom.MaxPlayers;
			this.roomSettings.ExpectedUsersTimer.Start();
			this.roomSettings.ResyncNetworkTimeTimer.Start();
			DelegateListProcessor joinedRoomEvent = RoomSystem.JoinedRoomEvent;
			if (joinedRoomEvent != null)
			{
				joinedRoomEvent.InvokeSafe();
			}
			this.roomSettings.ResyncNetworkTimeTimer.OnTimedEvent();
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	// Token: 0x060053CE RID: 21454 RVA: 0x001B9984 File Offset: 0x001B7B84
	private void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		if (newPlayer.IsLocal)
		{
			return;
		}
		Debug.Log(string.Format("Player {0} entered room", (newPlayer != null) ? new int?(newPlayer.ActorNumber) : null));
		if (!RoomSystem.netPlayersInRoom.Contains(newPlayer))
		{
			RoomSystem.netPlayersInRoom.Add(newPlayer);
		}
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(newPlayer);
		try
		{
			DelegateListProcessor<NetPlayer> playerJoinedEvent = RoomSystem.PlayerJoinedEvent;
			if (playerJoinedEvent != null)
			{
				playerJoinedEvent.InvokeSafe(in newPlayer);
			}
			DelegateListProcessor playersChangedEvent = RoomSystem.PlayersChangedEvent;
			if (playersChangedEvent != null)
			{
				playersChangedEvent.InvokeSafe();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	// Token: 0x060053CF RID: 21455 RVA: 0x001B9A28 File Offset: 0x001B7C28
	private void OnLeftRoom()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		RoomSystem.joinedRoom = false;
		RoomSystem.netPlayersInRoom.Clear();
		RoomSystem.roomGameMode = "";
		PlayerCosmeticsSystem.StaticReset();
		int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		for (int i = 0; i < RoomSystem.sceneViews.Length; i++)
		{
			RoomSystem.sceneViews[i].ControllerActorNr = actorNumber;
			RoomSystem.sceneViews[i].OwnerActorNr = actorNumber;
		}
		this.roomSettings.StatusEffectLimiter.Reset();
		this.roomSettings.SoundEffectLimiter.Reset();
		this.roomSettings.SoundEffectOtherLimiter.Reset();
		this.roomSettings.PlayerEffectLimiter.Reset();
		this.roomSettings.LavaSyncLimiter.Reset();
		try
		{
			RoomSystem.m_roomSizeOnJoin = 0;
			this.roomSettings.ExpectedUsersTimer.Stop();
			this.roomSettings.ResyncNetworkTimeTimer.Stop();
			DelegateListProcessor leftRoomEvent = RoomSystem.LeftRoomEvent;
			if (leftRoomEvent != null)
			{
				leftRoomEvent.InvokeSafe();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
		finally
		{
			RoomSystem.WasRoomSubscription = false;
			RoomSystem.InitialJoinTrigger = null;
		}
		GC.Collect(0);
	}

	// Token: 0x060053D0 RID: 21456 RVA: 0x001B9B5C File Offset: 0x001B7D5C
	private void OnPlayerLeftRoom(NetPlayer netPlayer)
	{
		if (netPlayer == null)
		{
			Debug.LogError("Player that left doesn't have a reference somehow...");
		}
		RoomSystem.netPlayersInRoom.Remove(netPlayer);
		try
		{
			DelegateListProcessor<NetPlayer> playerLeftEvent = RoomSystem.PlayerLeftEvent;
			if (playerLeftEvent != null)
			{
				playerLeftEvent.InvokeSafe(in netPlayer);
			}
			DelegateListProcessor playersChangedEvent = RoomSystem.PlayersChangedEvent;
			if (playersChangedEvent != null)
			{
				playersChangedEvent.InvokeSafe();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	// Token: 0x170007EF RID: 2031
	// (get) Token: 0x060053D1 RID: 21457 RVA: 0x001B9BC4 File Offset: 0x001B7DC4
	// (set) Token: 0x060053D2 RID: 21458 RVA: 0x001B9BCB File Offset: 0x001B7DCB
	private static bool UseRoomSizeOverride { get; set; }

	// Token: 0x170007F0 RID: 2032
	// (get) Token: 0x060053D3 RID: 21459 RVA: 0x001B9BD3 File Offset: 0x001B7DD3
	// (set) Token: 0x060053D4 RID: 21460 RVA: 0x001B9BDA File Offset: 0x001B7DDA
	public static byte RoomSizeOverride { get; set; }

	// Token: 0x170007F1 RID: 2033
	// (get) Token: 0x060053D5 RID: 21461 RVA: 0x001B9BE2 File Offset: 0x001B7DE2
	// (set) Token: 0x060053D6 RID: 21462 RVA: 0x001B9BE9 File Offset: 0x001B7DE9
	public static byte RoomSizeReduction { get; set; }

	// Token: 0x170007F2 RID: 2034
	// (get) Token: 0x060053D7 RID: 21463 RVA: 0x001B9BF1 File Offset: 0x001B7DF1
	public static List<NetPlayer> PlayersInRoom
	{
		get
		{
			return RoomSystem.netPlayersInRoom;
		}
	}

	// Token: 0x170007F3 RID: 2035
	// (get) Token: 0x060053D8 RID: 21464 RVA: 0x001B9BF8 File Offset: 0x001B7DF8
	public static string RoomGameMode
	{
		get
		{
			return RoomSystem.roomGameMode;
		}
	}

	// Token: 0x170007F4 RID: 2036
	// (get) Token: 0x060053D9 RID: 21465 RVA: 0x001B9BFF File Offset: 0x001B7DFF
	public static bool JoinedRoom
	{
		get
		{
			return NetworkSystem.Instance.InRoom && RoomSystem.joinedRoom;
		}
	}

	// Token: 0x170007F5 RID: 2037
	// (get) Token: 0x060053DA RID: 21466 RVA: 0x001B9C14 File Offset: 0x001B7E14
	public static bool AmITheHost
	{
		get
		{
			return NetworkSystem.Instance.IsMasterClient || !NetworkSystem.Instance.InRoom;
		}
	}

	// Token: 0x170007F6 RID: 2038
	// (get) Token: 0x060053DB RID: 21467 RVA: 0x001B9C31 File Offset: 0x001B7E31
	// (set) Token: 0x060053DC RID: 21468 RVA: 0x001B9C38 File Offset: 0x001B7E38
	public static bool IsVStumpRoom { get; private set; }

	// Token: 0x170007F7 RID: 2039
	// (get) Token: 0x060053DD RID: 21469 RVA: 0x001B9C40 File Offset: 0x001B7E40
	// (set) Token: 0x060053DE RID: 21470 RVA: 0x001B9C47 File Offset: 0x001B7E47
	public static bool WasRoomPrivate { get; private set; }

	// Token: 0x170007F8 RID: 2040
	// (get) Token: 0x060053DF RID: 21471 RVA: 0x001B9C4F File Offset: 0x001B7E4F
	// (set) Token: 0x060053E0 RID: 21472 RVA: 0x001B9C56 File Offset: 0x001B7E56
	public static bool WasRoomSubscription { get; private set; }

	// Token: 0x170007F9 RID: 2041
	// (get) Token: 0x060053E1 RID: 21473 RVA: 0x001B9C5E File Offset: 0x001B7E5E
	// (set) Token: 0x060053E2 RID: 21474 RVA: 0x001B9C65 File Offset: 0x001B7E65
	public static GorillaNetworkJoinTrigger InitialJoinTrigger { get; private set; }

	// Token: 0x060053E3 RID: 21475 RVA: 0x001B9C70 File Offset: 0x001B7E70
	static RoomSystem()
	{
		RoomSystem.disconnectTimer.Elapsed += RoomSystem.TimerDC;
		RoomSystem.disconnectTimer.AutoReset = false;
		RoomSystem.StaticLoad();
	}

	// Token: 0x060053E4 RID: 21476 RVA: 0x001B9DE8 File Offset: 0x001B7FE8
	[OnEnterPlay_Run]
	private static void StaticLoad()
	{
		RoomSystem.netEventCallbacks[0] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeLaunchProjectile);
		RoomSystem.netEventCallbacks[1] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeImpactEffect);
		RoomSystem.netEventCallbacks[4] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForNearby);
		RoomSystem.netEventCallbacks[7] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForParty);
		RoomSystem.netEventCallbacks[10] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForElevator);
		RoomSystem.netEventCallbacks[11] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForShuttle);
		RoomSystem.netEventCallbacks[2] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeStatusEffect);
		RoomSystem.netEventCallbacks[3] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeSoundEffect);
		RoomSystem.netEventCallbacks[5] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeReportTouch);
		RoomSystem.netEventCallbacks[8] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializePlayerLaunched);
		RoomSystem.netEventCallbacks[6] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializePlayerEffect);
		RoomSystem.netEventCallbacks[9] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializePlayerHit);
		RoomSystem.netEventCallbacks[12] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeLavaSync);
		RoomSystem.netEventCallbacks[13] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeMonkePointsRedeemed);
		RoomSystem.soundEffectCallback = new Action<RoomSystem.SoundEffect, NetPlayer>(RoomSystem.OnPlaySoundEffect);
		RoomSystem.statusEffectCallback = new Action<RoomSystem.StatusEffects>(RoomSystem.OnStatusEffect);
		VoxelManager.RegisterNetEventCallbacks();
	}

	// Token: 0x060053E5 RID: 21477 RVA: 0x001B9F63 File Offset: 0x001B8163
	private static void TimerDC(object sender, ElapsedEventArgs args)
	{
		RoomSystem.disconnectTimer.Stop();
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		PhotonNetwork.Disconnect();
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x060053E6 RID: 21478 RVA: 0x001B9F81 File Offset: 0x001B8181
	public static byte GetMaxRoomSize()
	{
		return (byte)RoomSystem.__roomSettings.GetRoomCount(true, true);
	}

	// Token: 0x060053E7 RID: 21479 RVA: 0x001B9F90 File Offset: 0x001B8190
	public static byte GetCurrentRoomExpectedSize()
	{
		if (!RoomSystem.joinedRoom)
		{
			return 10;
		}
		if (RoomSystem.IsVStumpRoom)
		{
			if (RoomSystem.m_roomSizeOnJoin >= 10)
			{
				return 10;
			}
			return RoomSystem.m_roomSizeOnJoin;
		}
		else
		{
			NetPlayer lowestActorNumberPlayer = RoomSystem.GetLowestActorNumberPlayer();
			RigContainer rigContainer;
			if (lowestActorNumberPlayer == null || !VRRigCache.Instance.TryGetVrrig(lowestActorNumberPlayer, out rigContainer))
			{
				return 10;
			}
			bool active = SubscriptionManager.GetSubscriptionDetails(lowestActorNumberPlayer).active;
			byte b;
			if (RoomSystem.WasRoomPrivate)
			{
				Room currentRoom = PhotonNetwork.CurrentRoom;
				if (!rigContainer.Rig.InitializedCosmetics)
				{
					b = currentRoom.MaxPlayers;
					if (b >= 20)
					{
						return 20;
					}
					return b;
				}
				else
				{
					b = (byte)RoomSystem.__roomSettings.GetRoomCount(true, active);
					if (!active && PhotonNetwork.CurrentRoom.PlayerCount > 10)
					{
						b = PhotonNetwork.CurrentRoom.PlayerCount;
					}
				}
			}
			else
			{
				GorillaNetworkJoinTrigger initialJoinTrigger = RoomSystem.InitialJoinTrigger;
				GTZone gtzone = GTZone.none;
				if (initialJoinTrigger.IsNotNull())
				{
					gtzone = initialJoinTrigger.zone;
				}
				b = (byte)RoomSystem.__roomSettings.GetRoomCount(gtzone, GameMode.CurrentGameModeType, false, RoomSystem.WasRoomSubscription);
			}
			if (b >= 20)
			{
				return 20;
			}
			return b;
		}
	}

	// Token: 0x060053E8 RID: 21480 RVA: 0x001BA084 File Offset: 0x001B8284
	public static byte GetRoomSizeForCreate(GTZone zone, GameModeType mode, bool privateRoom, bool sub)
	{
		if (RoomSystem.UseRoomSizeOverride)
		{
			return RoomSystem.RoomSizeOverride;
		}
		return (byte)RoomSystem.__roomSettings.GetRoomCount(zone, mode, privateRoom, sub);
	}

	// Token: 0x060053E9 RID: 21481 RVA: 0x001BA0A2 File Offset: 0x001B82A2
	public static void OverrideRoomSize(byte size)
	{
		if (size < 1)
		{
			size = 1;
		}
		else if (size > 10)
		{
			size = 10;
		}
		if (size == 10)
		{
			RoomSystem.UseRoomSizeOverride = false;
		}
		else
		{
			RoomSystem.UseRoomSizeOverride = true;
		}
		RoomSystem.RoomSizeOverride = size;
	}

	// Token: 0x060053EA RID: 21482 RVA: 0x001BA0CF File Offset: 0x001B82CF
	public static byte GetOverridenRoomSize()
	{
		if (RoomSystem.UseRoomSizeOverride)
		{
			return RoomSystem.RoomSizeOverride;
		}
		return 10;
	}

	// Token: 0x060053EB RID: 21483 RVA: 0x001BA0E0 File Offset: 0x001B82E0
	public static void ClearOverridenRoomSize()
	{
		RoomSystem.UseRoomSizeOverride = false;
		RoomSystem.RoomSizeOverride = 10;
	}

	// Token: 0x060053EC RID: 21484 RVA: 0x001BA0EF File Offset: 0x001B82EF
	public static void MakeRoomMultiplayer(byte roomSize)
	{
		if (!RoomSystem.joinedRoom || RoomSystem.m_roomSizeOnJoin > 1)
		{
			return;
		}
		if (roomSize > 20)
		{
			roomSize = 20;
		}
		RoomSystem.m_roomSizeOnJoin = roomSize;
		PhotonNetwork.CurrentRoom.MaxPlayers = roomSize;
	}

	// Token: 0x060053ED RID: 21485 RVA: 0x001BA11C File Offset: 0x001B831C
	public static NetPlayer GetLowestActorNumberPlayer()
	{
		if (!RoomSystem.joinedRoom || RoomSystem.netPlayersInRoom.Count == 0)
		{
			return null;
		}
		NetPlayer netPlayer = RoomSystem.netPlayersInRoom[0];
		for (int i = 1; i < RoomSystem.netPlayersInRoom.Count; i++)
		{
			NetPlayer netPlayer2 = RoomSystem.netPlayersInRoom[i];
			if (netPlayer2.ActorNumber < netPlayer.ActorNumber)
			{
				netPlayer = netPlayer2;
			}
		}
		return netPlayer;
	}

	// Token: 0x060053EE RID: 21486 RVA: 0x001BA17C File Offset: 0x001B837C
	internal static void SendEvent(byte code, object[] evData, in NetPlayer target, bool reliable)
	{
		NetworkSystemRaiseEvent.neoTarget.TargetActors[0] = target.ActorNumber;
		RoomSystem.SendEvent(code, evData, in NetworkSystemRaiseEvent.neoTarget, reliable);
	}

	// Token: 0x060053EF RID: 21487 RVA: 0x001BA19E File Offset: 0x001B839E
	internal static void SendEvent(byte code, object[] evData, in NetEventOptions neo, bool reliable)
	{
		RoomSystem.sendEventData[0] = NetworkSystem.Instance.ServerTimestamp;
		RoomSystem.sendEventData[1] = code;
		RoomSystem.sendEventData[2] = evData;
		NetworkSystemRaiseEvent.RaiseEvent(3, RoomSystem.sendEventData, neo, reliable);
	}

	// Token: 0x060053F0 RID: 21488 RVA: 0x001BA1D9 File Offset: 0x001B83D9
	private static void OnEvent(EventData data)
	{
		RoomSystem.OnEvent(data.Code, data.CustomData, data.Sender);
	}

	// Token: 0x060053F1 RID: 21489 RVA: 0x001BA1F4 File Offset: 0x001B83F4
	private static void OnEvent(byte code, object data, int source)
	{
		NetPlayer netPlayer;
		if (code != 3 || !Utils.PlayerInRoom(source, out netPlayer))
		{
			return;
		}
		try
		{
			object[] array = (object[])data;
			int num = Convert.ToInt32(array[0]);
			byte b = Convert.ToByte(array[1]);
			object[] array2 = null;
			if (array.Length > 2)
			{
				object obj = array[2];
				array2 = ((obj == null) ? null : ((object[])obj));
			}
			PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(netPlayer.ActorNumber, num);
			Action<object[], PhotonMessageInfoWrapped> action;
			if (RoomSystem.netEventCallbacks.TryGetValue(b, out action))
			{
				action(array2, photonMessageInfoWrapped);
			}
		}
		catch
		{
		}
	}

	// Token: 0x060053F2 RID: 21490 RVA: 0x001BA288 File Offset: 0x001B8488
	internal static void SearchForNearby(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "JoinPubWithNearby");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 23, NetworkSystem.Instance.SimTime))
		{
			return;
		}
		string text = (string)shuffleData[0];
		string text2 = (string)shuffleData[1];
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
		if (!GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId))
		{
			MonkeAgent.instance.SendReport("possible kick attempt", player.UserId, player.NickName);
			return;
		}
		if (!flag || !RoomSystem.WasRoomPrivate)
		{
			return;
		}
		PhotonNetworkController.Instance.AttemptToFollowIntoPub(player.UserId, player.ActorNumber, text2, text, JoinType.FollowingNearby);
	}

	// Token: 0x060053F3 RID: 21491 RVA: 0x001BA368 File Offset: 0x001B8568
	internal static void SearchForParty(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "PARTY_JOIN");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 23, NetworkSystem.Instance.SimTime))
		{
			return;
		}
		string text = (string)shuffleData[0];
		string text2 = (string)shuffleData[1];
		if (!FriendshipGroupDetection.Instance.IsInMyGroup(info.Sender.UserId))
		{
			MonkeAgent.instance.SendReport("possible kick attempt", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (PlayFabAuthenticator.instance.GetSafety())
		{
			return;
		}
		PhotonNetworkController.Instance.AttemptToFollowIntoPub(info.Sender.UserId, info.Sender.ActorNumber, text2, text, JoinType.FollowingParty);
	}

	// Token: 0x060053F4 RID: 21492 RVA: 0x001BA438 File Offset: 0x001B8638
	internal static void SearchForElevator(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "JoinPubWithElevator");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 23, NetworkSystem.Instance.SimTime))
		{
			return;
		}
		string text = (string)shuffleData[0];
		string text2 = (string)shuffleData[1];
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
		if (GRElevatorManager.ValidElevatorNetworking(info.Sender.ActorNumber) && GRElevatorManager.ValidElevatorNetworking(NetworkSystem.Instance.LocalPlayer.ActorNumber))
		{
			if (!flag)
			{
				GRElevatorManager.JoinPublicRoom();
				return;
			}
			PhotonNetworkController.Instance.AttemptToFollowIntoPub(player.UserId, player.ActorNumber, text2, text, JoinType.JoinWithElevator);
		}
	}

	// Token: 0x060053F5 RID: 21493 RVA: 0x001BA4F8 File Offset: 0x001B86F8
	internal static void SearchForShuttle(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "JoinPubWithElevator");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 23, NetworkSystem.Instance.SimTime))
		{
			return;
		}
		string text = (string)shuffleData[0];
		string text2 = (string)shuffleData[1];
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
		bool flag2 = GRElevatorManager.ValidShuttleNetworking(info.Sender.ActorNumber);
		bool flag3 = GRElevatorManager.ValidShuttleNetworking(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		if (flag2 && flag3)
		{
			if (!flag)
			{
				GRElevatorManager.JoinPublicRoom();
				return;
			}
			PhotonNetworkController.Instance.AttemptToFollowIntoPub(player.UserId, player.ActorNumber, text2, text, JoinType.JoinWithElevator);
		}
	}

	// Token: 0x060053F6 RID: 21494 RVA: 0x001BA5BC File Offset: 0x001B87BC
	internal static void SendNearbyFollowCommand(GorillaFriendCollider friendCollider, string shuffler, string keyStr)
	{
		RoomSystem.groupJoinSendData[0] = shuffler;
		RoomSystem.groupJoinSendData[1] = keyStr;
		NetEventOptions netEventOptions = new NetEventOptions
		{
			TargetActors = new int[1]
		};
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (friendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) && netPlayer != NetworkSystem.Instance.LocalPlayer)
			{
				netEventOptions.TargetActors[0] = netPlayer.ActorNumber;
				RoomSystem.SendEvent(4, RoomSystem.groupJoinSendData, in netEventOptions, false);
			}
		}
	}

	// Token: 0x060053F7 RID: 21495 RVA: 0x001BA668 File Offset: 0x001B8868
	internal static void SendPartyFollowCommand(string shuffler, string keyStr)
	{
		RoomSystem.groupJoinSendData[0] = shuffler;
		RoomSystem.groupJoinSendData[1] = keyStr;
		NetEventOptions netEventOptions = new NetEventOptions
		{
			TargetActors = new int[1]
		};
		foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
		{
			VRRig rig = rigContainer.Rig;
			if (rig.IsLocalPartyMember && rig.creator != NetworkSystem.Instance.LocalPlayer)
			{
				netEventOptions.TargetActors[0] = rig.creator.ActorNumber;
				RoomSystem.SendEvent(7, RoomSystem.groupJoinSendData, in netEventOptions, false);
			}
		}
	}

	// Token: 0x060053F8 RID: 21496 RVA: 0x001BA710 File Offset: 0x001B8910
	internal static void SendElevatorFollowCommand(string shuffler, string keyStr, GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider targetFriendCollider)
	{
		RoomSystem.SendGroupJoinFollowCommand(10, shuffler, keyStr, sourceFriendCollider, targetFriendCollider);
	}

	// Token: 0x060053F9 RID: 21497 RVA: 0x001BA71D File Offset: 0x001B891D
	internal static void SendShuttleFollowCommand(string shuffler, string keyStr, GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider targetFriendCollider)
	{
		RoomSystem.SendGroupJoinFollowCommand(11, shuffler, keyStr, sourceFriendCollider, targetFriendCollider);
	}

	// Token: 0x060053FA RID: 21498 RVA: 0x001BA72C File Offset: 0x001B892C
	internal static void SendGroupJoinFollowCommand(byte eventType, string shuffler, string keyStr, GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider targetFriendCollider)
	{
		RoomSystem.groupJoinSendData[0] = shuffler;
		RoomSystem.groupJoinSendData[1] = keyStr;
		NetEventOptions netEventOptions = new NetEventOptions
		{
			TargetActors = new int[1]
		};
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (sourceFriendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) || (targetFriendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) && netPlayer != NetworkSystem.Instance.LocalPlayer))
			{
				netEventOptions.TargetActors[0] = netPlayer.ActorNumber;
				RoomSystem.SendEvent(eventType, RoomSystem.groupJoinSendData, in netEventOptions, false);
			}
		}
	}

	// Token: 0x060053FB RID: 21499 RVA: 0x001BA7EC File Offset: 0x001B89EC
	private static void DeserializeReportTouch(object[] data, PhotonMessageInfoWrapped info)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer netPlayer = (NetPlayer)data[0];
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		Action<NetPlayer, NetPlayer> action = RoomSystem.playerTouchedCallback;
		if (action == null)
		{
			return;
		}
		action(netPlayer, player);
	}

	// Token: 0x060053FC RID: 21500 RVA: 0x001BA834 File Offset: 0x001B8A34
	internal static void SendReportTouch(NetPlayer touchedNetPlayer)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			RoomSystem.reportTouchSendData[0] = touchedNetPlayer;
			RoomSystem.SendEvent(5, RoomSystem.reportTouchSendData, in NetworkSystemRaiseEvent.neoMaster, false);
			return;
		}
		Action<NetPlayer, NetPlayer> action = RoomSystem.playerTouchedCallback;
		if (action == null)
		{
			return;
		}
		action(touchedNetPlayer, NetworkSystem.Instance.LocalPlayer);
	}

	// Token: 0x060053FD RID: 21501 RVA: 0x001BA881 File Offset: 0x001B8A81
	internal static void LaunchPlayer(NetPlayer player, Vector3 velocity)
	{
		RoomSystem.reportTouchSendData[0] = velocity;
		RoomSystem.SendEvent(8, RoomSystem.reportTouchSendData, in player, false);
	}

	// Token: 0x060053FE RID: 21502 RVA: 0x001BA8A0 File Offset: 0x001B8AA0
	private static void DeserializePlayerLaunched(object[] data, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "DeserializePlayerLaunched");
		GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
		if (activeGameMode != null && activeGameMode.GameType() == GameModeType.Guardian && info.Sender == NetworkSystem.Instance.MasterClient)
		{
			object obj = data[0];
			if (obj is Vector3)
			{
				Vector3 vector = (Vector3)obj;
				float num = 10000f;
				if ((in vector).IsValid(in num) && vector.magnitude <= 20f && RoomSystem.playerLaunchedCallLimiter.CheckCallTime(Time.time))
				{
					GTPlayer.Instance.DoLaunch(vector);
					return;
				}
			}
		}
	}

	// Token: 0x060053FF RID: 21503 RVA: 0x001BA934 File Offset: 0x001B8B34
	internal static void HitPlayer(NetPlayer player, Vector3 direction, float strength)
	{
		RoomSystem.reportHitSendData[0] = direction;
		RoomSystem.reportHitSendData[1] = strength;
		RoomSystem.reportHitSendData[2] = player.ActorNumber;
		RoomSystem.SendEvent(9, RoomSystem.reportHitSendData, in NetworkSystemRaiseEvent.neoOthers, false);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			rigContainer.Rig.DisableHitWithKnockBack();
		}
	}

	// Token: 0x06005400 RID: 21504 RVA: 0x001BA99C File Offset: 0x001B8B9C
	private static void DeserializePlayerHit(object[] data, PhotonMessageInfoWrapped info)
	{
		object obj = data[0];
		if (obj is Vector3)
		{
			Vector3 vector = (Vector3)obj;
			obj = data[1];
			if (obj is float)
			{
				float num = (float)obj;
				obj = data[2];
				if (obj is int)
				{
					int num2 = (int)obj;
					float num3 = 10000f;
					RigContainer rigContainer;
					if ((in vector).IsValid(in num3) && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) && FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 20, info.SentServerTime))
					{
						float num4 = num.ClampSafe(0f, 10f);
						MonkeAgent.IncrementRPCCall(info, "DeserializePlayerHit");
						if (num2 == NetworkSystem.Instance.LocalPlayer.ActorNumber)
						{
							CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect;
							CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect2;
							if (GorillaTagger.Instance.offlineVRRig.TemporaryCosmeticEffects.TryGetValue(CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback, out cosmeticEffect))
							{
								if (!cosmeticEffect.IsGameModeAllowed())
								{
									return;
								}
								float num5 = (num4 * cosmeticEffect.knockbackStrength * cosmeticEffect.knockbackStrengthMultiplier).ClampSafe(cosmeticEffect.minKnockbackStrength, cosmeticEffect.maxKnockbackStrength);
								if (cosmeticEffect.applyScaleToKnockbackStrength)
								{
									num5 *= GTPlayer.Instance.scale;
								}
								GTPlayer.Instance.ApplyKnockback(vector.normalized, num5, cosmeticEffect.forceOffTheGround);
							}
							else if (GorillaTagger.Instance.offlineVRRig.TemporaryCosmeticEffects.TryGetValue(CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback, out cosmeticEffect2))
							{
								if (!cosmeticEffect2.IsGameModeAllowed())
								{
									return;
								}
								float num6 = (num4 * cosmeticEffect2.knockbackStrength * cosmeticEffect2.knockbackStrengthMultiplier).ClampSafe(cosmeticEffect2.minKnockbackStrength, cosmeticEffect2.maxKnockbackStrength);
								if (cosmeticEffect.applyScaleToKnockbackStrength)
								{
									num6 *= GTPlayer.Instance.scale;
								}
								GTPlayer.Instance.ApplyKnockback(vector.normalized, num6, cosmeticEffect2.forceOffTheGround);
							}
						}
						NetPlayer player = NetworkSystem.Instance.GetPlayer(num2);
						RigContainer rigContainer2;
						if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer2))
						{
							rigContainer2.Rig.DisableHitWithKnockBack();
						}
						return;
					}
				}
			}
		}
	}

	// Token: 0x06005401 RID: 21505 RVA: 0x001BAB88 File Offset: 0x001B8D88
	private static void SetSlowedTime()
	{
		if (GorillaTagger.Instance.currentStatus != GorillaTagger.StatusEffect.Slowed)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		}
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Slowed, GorillaTagger.Instance.slowCooldown);
		GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
	}

	// Token: 0x06005402 RID: 21506 RVA: 0x001BAC04 File Offset: 0x001B8E04
	private static void SetTaggedTime()
	{
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
	}

	// Token: 0x06005403 RID: 21507 RVA: 0x001BAC74 File Offset: 0x001B8E74
	private static void SetFrozenTime()
	{
		GorillaFreezeTagManager gorillaFreezeTagManager = GameMode.ActiveGameMode as GorillaFreezeTagManager;
		if (gorillaFreezeTagManager != null)
		{
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Slowed, gorillaFreezeTagManager.freezeDuration);
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
		}
	}

	// Token: 0x06005404 RID: 21508 RVA: 0x001BACED File Offset: 0x001B8EED
	private static void SetJoinedTaggedTime()
	{
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
	}

	// Token: 0x06005405 RID: 21509 RVA: 0x001BAD30 File Offset: 0x001B8F30
	private static void SetUntaggedTime()
	{
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.None, 0f);
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
	}

	// Token: 0x06005406 RID: 21510 RVA: 0x001BAD8B File Offset: 0x001B8F8B
	private static void OnStatusEffect(RoomSystem.StatusEffects status)
	{
		switch (status)
		{
		case RoomSystem.StatusEffects.TaggedTime:
			RoomSystem.SetTaggedTime();
			return;
		case RoomSystem.StatusEffects.JoinedTaggedTime:
			RoomSystem.SetJoinedTaggedTime();
			return;
		case RoomSystem.StatusEffects.SetSlowedTime:
			RoomSystem.SetSlowedTime();
			return;
		case RoomSystem.StatusEffects.UnTagged:
			RoomSystem.SetUntaggedTime();
			return;
		case RoomSystem.StatusEffects.FrozenTime:
			RoomSystem.SetFrozenTime();
			return;
		default:
			return;
		}
	}

	// Token: 0x06005407 RID: 21511 RVA: 0x001BADC8 File Offset: 0x001B8FC8
	private static void DeserializeStatusEffect(object[] data, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "DeserializeStatusEffect");
		if (!player.IsMasterClient)
		{
			MonkeAgent.instance.SendReport("invalid status", player.UserId, player.NickName);
			return;
		}
		if (!RoomSystem.callbackInstance.roomSettings.StatusEffectLimiter.CheckCallServerTime(info.SentServerTime))
		{
			return;
		}
		RoomSystem.StatusEffects statusEffects = (RoomSystem.StatusEffects)Convert.ToInt32(data[0]);
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action == null)
		{
			return;
		}
		action(statusEffects);
	}

	// Token: 0x06005408 RID: 21512 RVA: 0x001BAE4E File Offset: 0x001B904E
	internal static void SendStatusEffectAll(RoomSystem.StatusEffects status)
	{
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action != null)
		{
			action(status);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.statusSendData[0] = (int)status;
		RoomSystem.SendEvent(2, RoomSystem.statusSendData, in NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x06005409 RID: 21513 RVA: 0x001BAE87 File Offset: 0x001B9087
	internal static void SendStatusEffectToPlayer(RoomSystem.StatusEffects status, NetPlayer target)
	{
		if (!target.IsLocal)
		{
			RoomSystem.statusSendData[0] = (int)status;
			RoomSystem.SendEvent(2, RoomSystem.statusSendData, in target, false);
			return;
		}
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action == null)
		{
			return;
		}
		action(status);
	}

	// Token: 0x0600540A RID: 21514 RVA: 0x001BAEBD File Offset: 0x001B90BD
	internal static void PlaySoundEffect(int soundIndex, float soundVolume, bool stopCurrentAudio)
	{
		VRRigCache.Instance.localRig.Rig.PlayTagSoundLocal(soundIndex, soundVolume, stopCurrentAudio);
	}

	// Token: 0x0600540B RID: 21515 RVA: 0x001BAED8 File Offset: 0x001B90D8
	internal static void PlaySoundEffect(int soundIndex, float soundVolume, bool stopCurrentAudio, NetPlayer target)
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(target, out rigContainer))
		{
			rigContainer.Rig.PlayTagSoundLocal(soundIndex, soundVolume, stopCurrentAudio);
		}
	}

	// Token: 0x0600540C RID: 21516 RVA: 0x001BAF02 File Offset: 0x001B9102
	private static void OnPlaySoundEffect(RoomSystem.SoundEffect sound, NetPlayer target)
	{
		if (target.IsLocal)
		{
			RoomSystem.PlaySoundEffect(sound.id, sound.volume, sound.stopCurrentAudio);
			return;
		}
		RoomSystem.PlaySoundEffect(sound.id, sound.volume, sound.stopCurrentAudio, target);
	}

	// Token: 0x0600540D RID: 21517 RVA: 0x001BAF3C File Offset: 0x001B913C
	private static void DeserializeSoundEffect(object[] data, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		MonkeAgent.IncrementRPCCall(info, "DeserializeSoundEffect");
		if (!player.Equals(RoomSystem.GetLowestActorNumberPlayer()))
		{
			MonkeAgent.instance.SendReport("invalid sound effect", player.UserId, player.NickName);
			return;
		}
		RoomSystem.SoundEffect soundEffect;
		soundEffect.id = Convert.ToInt32(data[0]);
		soundEffect.volume = Convert.ToSingle(data[1]);
		soundEffect.stopCurrentAudio = Convert.ToBoolean(data[2]);
		if (!float.IsFinite(soundEffect.volume))
		{
			return;
		}
		NetPlayer netPlayer;
		if (data.Length > 3)
		{
			if (!RoomSystem.callbackInstance.roomSettings.SoundEffectOtherLimiter.CheckCallServerTime(info.SentServerTime))
			{
				return;
			}
			int num = Convert.ToInt32(data[3]);
			netPlayer = NetworkSystem.Instance.GetPlayer(num);
		}
		else
		{
			if (!RoomSystem.callbackInstance.roomSettings.SoundEffectLimiter.CheckCallServerTime(info.SentServerTime))
			{
				return;
			}
			netPlayer = NetworkSystem.Instance.LocalPlayer;
		}
		if (netPlayer != null)
		{
			RoomSystem.soundEffectCallback(soundEffect, netPlayer);
		}
	}

	// Token: 0x0600540E RID: 21518 RVA: 0x001BB040 File Offset: 0x001B9240
	internal static void SendSoundEffectAll(int soundIndex, float soundVolume, bool stopCurrentAudio = false)
	{
		RoomSystem.SendSoundEffectAll(new RoomSystem.SoundEffect(soundIndex, soundVolume, stopCurrentAudio));
	}

	// Token: 0x0600540F RID: 21519 RVA: 0x001BB050 File Offset: 0x001B9250
	internal static void SendSoundEffectAll(RoomSystem.SoundEffect sound)
	{
		Action<RoomSystem.SoundEffect, NetPlayer> action = RoomSystem.soundEffectCallback;
		if (action != null)
		{
			action(sound, NetworkSystem.Instance.LocalPlayer);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.soundSendData[0] = sound.id;
		RoomSystem.soundSendData[1] = sound.volume;
		RoomSystem.soundSendData[2] = sound.stopCurrentAudio;
		RoomSystem.SendEvent(3, RoomSystem.soundSendData, in NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x06005410 RID: 21520 RVA: 0x001BB0C7 File Offset: 0x001B92C7
	internal static void SendSoundEffectToPlayer(int soundIndex, float soundVolume, NetPlayer player, bool stopCurrentAudio = false)
	{
		RoomSystem.SendSoundEffectToPlayer(new RoomSystem.SoundEffect(soundIndex, soundVolume, stopCurrentAudio), player);
	}

	// Token: 0x06005411 RID: 21521 RVA: 0x001BB0D8 File Offset: 0x001B92D8
	internal static void SendSoundEffectToPlayer(RoomSystem.SoundEffect sound, NetPlayer player)
	{
		if (player.IsLocal)
		{
			Action<RoomSystem.SoundEffect, NetPlayer> action = RoomSystem.soundEffectCallback;
			if (action == null)
			{
				return;
			}
			action(sound, player);
			return;
		}
		else
		{
			if (!RoomSystem.joinedRoom)
			{
				return;
			}
			RoomSystem.soundSendData[0] = sound.id;
			RoomSystem.soundSendData[1] = sound.volume;
			RoomSystem.soundSendData[2] = sound.stopCurrentAudio;
			RoomSystem.SendEvent(3, RoomSystem.soundSendData, in player, false);
			return;
		}
	}

	// Token: 0x06005412 RID: 21522 RVA: 0x001BB14B File Offset: 0x001B934B
	internal static void SendSoundEffectOnOther(int soundIndex, float soundvolume, NetPlayer target, bool stopCurrentAudio = false)
	{
		RoomSystem.SendSoundEffectOnOther(new RoomSystem.SoundEffect(soundIndex, soundvolume, stopCurrentAudio), target);
	}

	// Token: 0x06005413 RID: 21523 RVA: 0x001BB15C File Offset: 0x001B935C
	internal static void SendSoundEffectOnOther(RoomSystem.SoundEffect sound, NetPlayer target)
	{
		Action<RoomSystem.SoundEffect, NetPlayer> action = RoomSystem.soundEffectCallback;
		if (action != null)
		{
			action(sound, target);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.sendSoundDataOther[0] = sound.id;
		RoomSystem.sendSoundDataOther[1] = sound.volume;
		RoomSystem.sendSoundDataOther[2] = sound.stopCurrentAudio;
		RoomSystem.sendSoundDataOther[3] = target.ActorNumber;
		RoomSystem.SendEvent(3, RoomSystem.sendSoundDataOther, in NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x06005414 RID: 21524 RVA: 0x001BB1DC File Offset: 0x001B93DC
	internal static void OnPlayerEffect(PlayerEffect effect, NetPlayer target)
	{
		if (target == null)
		{
			return;
		}
		RoomSystem.PlayerEffectConfig playerEffectConfig;
		RigContainer rigContainer;
		if (RoomSystem.playerEffectDictionary.TryGetValue(effect, out playerEffectConfig) && VRRigCache.Instance.TryGetVrrig(target, out rigContainer) && rigContainer != null && rigContainer.Rig != null && playerEffectConfig.tagEffectPack != null)
		{
			TagEffectsLibrary.PlayEffect(rigContainer.Rig.transform, false, rigContainer.Rig.scaleFactor, target.IsLocal ? TagEffectsLibrary.EffectType.FIRST_PERSON : TagEffectsLibrary.EffectType.THIRD_PERSON, playerEffectConfig.tagEffectPack, playerEffectConfig.tagEffectPack, rigContainer.Rig.transform.rotation);
		}
	}

	// Token: 0x06005415 RID: 21525 RVA: 0x001BB274 File Offset: 0x001B9474
	private static void DeserializePlayerEffect(object[] data, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "DeserializePlayerEffect");
		if (!RoomSystem.callbackInstance.roomSettings.PlayerEffectLimiter.CheckCallServerTime(info.SentServerTime))
		{
			return;
		}
		int num = Convert.ToInt32(data[0]);
		PlayerEffect playerEffect = (PlayerEffect)Convert.ToInt32(data[1]);
		NetPlayer player = NetworkSystem.Instance.GetPlayer(num);
		RoomSystem.OnPlayerEffect(playerEffect, player);
	}

	// Token: 0x06005416 RID: 21526 RVA: 0x001BB2D0 File Offset: 0x001B94D0
	internal static void SendPlayerEffect(PlayerEffect effect, NetPlayer target)
	{
		RoomSystem.OnPlayerEffect(effect, target);
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.playerEffectData[0] = target.ActorNumber;
		RoomSystem.playerEffectData[1] = effect;
		RoomSystem.SendEvent(6, RoomSystem.playerEffectData, in NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x04006545 RID: 25925
	private static RoomSystem.ImpactFxContainer impactEffect = new RoomSystem.ImpactFxContainer();

	// Token: 0x04006546 RID: 25926
	private static RoomSystem.LaunchProjectileContainer launchProjectile = new RoomSystem.LaunchProjectileContainer();

	// Token: 0x04006547 RID: 25927
	public static GameObject playerImpactEffectPrefab = null;

	// Token: 0x04006548 RID: 25928
	private static readonly object[] projectileSendData = new object[9];

	// Token: 0x04006549 RID: 25929
	private static readonly object[] impactSendData = new object[6];

	// Token: 0x0400654A RID: 25930
	private static readonly List<int> hashValues = new List<int>(2);

	// Token: 0x0400654B RID: 25931
	[OnExitPlay_SetNull]
	internal static Action<RoomSystem.LavaSyncEventData> OnLavaSyncReceived;

	// Token: 0x0400654C RID: 25932
	private const int lavaSyncHeaderSize = 5;

	// Token: 0x0400654D RID: 25933
	private const int lavaSyncTotalSize = 25;

	// Token: 0x0400654E RID: 25934
	private static readonly object[] lavaSyncSendData = new object[25];

	// Token: 0x0400654F RID: 25935
	[OnExitPlay_SetNull]
	internal static Action<NetPlayer, int> OnMonkePointsRedeemedReceived;

	// Token: 0x04006550 RID: 25936
	private const int monkePointsRedeemedMaxCount = 50;

	// Token: 0x04006551 RID: 25937
	private static readonly object[] monkePointsRedeemedSendData = new object[1];

	// Token: 0x04006552 RID: 25938
	[SerializeField]
	private RoomSystemSettings roomSettings;

	// Token: 0x04006553 RID: 25939
	[SerializeField]
	private string[] prefabsToInstantiateByPath;

	// Token: 0x04006554 RID: 25940
	[SerializeField]
	private GameObject[] prefabsToInstantiate;

	// Token: 0x04006555 RID: 25941
	private List<GameObject> prefabsInstantiated = new List<GameObject>();

	// Token: 0x04006556 RID: 25942
	public static Dictionary<PlayerEffect, RoomSystem.PlayerEffectConfig> playerEffectDictionary = new Dictionary<PlayerEffect, RoomSystem.PlayerEffectConfig>();

	// Token: 0x04006557 RID: 25943
	private static RoomSystemSettings __roomSettings;

	// Token: 0x04006558 RID: 25944
	[OnEnterPlay_SetNull]
	private static RoomSystem callbackInstance;

	// Token: 0x0400655B RID: 25947
	private static byte m_roomSizeOnJoin;

	// Token: 0x0400655D RID: 25949
	[OnEnterPlay_Clear]
	private static List<NetPlayer> netPlayersInRoom = new List<NetPlayer>(20);

	// Token: 0x0400655E RID: 25950
	[OnEnterPlay_Set("")]
	private static string roomGameMode = "";

	// Token: 0x0400655F RID: 25951
	[OnEnterPlay_Set(false)]
	private static bool joinedRoom = false;

	// Token: 0x04006564 RID: 25956
	[OnEnterPlay_SetNull]
	private static PhotonView[] sceneViews;

	// Token: 0x04006565 RID: 25957
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor LeftRoomEvent = new DelegateListProcessor();

	// Token: 0x04006566 RID: 25958
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor JoinedRoomEvent = new DelegateListProcessor();

	// Token: 0x04006567 RID: 25959
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor<NetPlayer> PlayerJoinedEvent = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04006568 RID: 25960
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor<NetPlayer> PlayerLeftEvent = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04006569 RID: 25961
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor PlayersChangedEvent = new DelegateListProcessor();

	// Token: 0x0400656A RID: 25962
	private static Timer disconnectTimer = new Timer();

	// Token: 0x0400656B RID: 25963
	[OnExitPlay_Clear]
	internal static readonly Dictionary<byte, Action<object[], PhotonMessageInfoWrapped>> netEventCallbacks = new Dictionary<byte, Action<object[], PhotonMessageInfoWrapped>>(20);

	// Token: 0x0400656C RID: 25964
	private static readonly object[] sendEventData = new object[3];

	// Token: 0x0400656D RID: 25965
	private static readonly object[] groupJoinSendData = new object[2];

	// Token: 0x0400656E RID: 25966
	private static readonly object[] reportTouchSendData = new object[1];

	// Token: 0x0400656F RID: 25967
	private static readonly object[] reportHitSendData = new object[3];

	// Token: 0x04006570 RID: 25968
	[OnExitPlay_SetNull]
	public static Action<NetPlayer, NetPlayer> playerTouchedCallback;

	// Token: 0x04006571 RID: 25969
	private static CallLimiter playerLaunchedCallLimiter = new CallLimiter(3, 15f, 0.5f);

	// Token: 0x04006572 RID: 25970
	private static CallLimiter hitPlayerCallLimiter = new CallLimiter(10, 2f, 0.5f);

	// Token: 0x04006573 RID: 25971
	private static object[] statusSendData = new object[1];

	// Token: 0x04006574 RID: 25972
	public static Action<RoomSystem.StatusEffects> statusEffectCallback;

	// Token: 0x04006575 RID: 25973
	private static object[] soundSendData = new object[3];

	// Token: 0x04006576 RID: 25974
	private static object[] sendSoundDataOther = new object[4];

	// Token: 0x04006577 RID: 25975
	public static Action<RoomSystem.SoundEffect, NetPlayer> soundEffectCallback;

	// Token: 0x04006578 RID: 25976
	private static object[] playerEffectData = new object[2];

	// Token: 0x02000D3A RID: 3386
	private class ImpactFxContainer : IFXContext
	{
		// Token: 0x170007FA RID: 2042
		// (get) Token: 0x06005418 RID: 21528 RVA: 0x001BB32F File Offset: 0x001B952F
		public FXSystemSettings settings
		{
			get
			{
				return this.targetRig.fxSettings;
			}
		}

		// Token: 0x06005419 RID: 21529 RVA: 0x001BB33C File Offset: 0x001B953C
		public virtual void OnPlayFX()
		{
			NetPlayer creator = this.targetRig.creator;
			ProjectileTracker.ProjectileInfo projectileInfo;
			if (this.targetRig.isOfflineVRRig)
			{
				projectileInfo = ProjectileTracker.GetLocalProjectile(this.projectileIndex);
			}
			else
			{
				ValueTuple<bool, ProjectileTracker.ProjectileInfo> andRemoveRemotePlayerProjectile = ProjectileTracker.GetAndRemoveRemotePlayerProjectile(creator, this.projectileIndex);
				if (!andRemoveRemotePlayerProjectile.Item1)
				{
					return;
				}
				projectileInfo = andRemoveRemotePlayerProjectile.Item2;
			}
			SlingshotProjectile projectileInstance = projectileInfo.projectileInstance;
			GameObject gameObject = (projectileInfo.hasImpactOverride ? projectileInstance.playerImpactEffectPrefab : RoomSystem.playerImpactEffectPrefab);
			GameObject gameObject2 = ObjectPools.instance.Instantiate(gameObject, this.position, true);
			gameObject2.transform.localScale = Vector3.one * this.targetRig.scaleFactor;
			GorillaColorizableBase gorillaColorizableBase;
			if (gameObject2.TryGetComponent<GorillaColorizableBase>(out gorillaColorizableBase))
			{
				gorillaColorizableBase.SetColor(this.colour);
			}
			SurfaceImpactFX component = gameObject2.GetComponent<SurfaceImpactFX>();
			if (component != null)
			{
				component.SetScale(projectileInstance.transform.localScale.x * projectileInstance.impactEffectScaleMultiplier);
			}
			SoundBankPlayer component2 = gameObject2.GetComponent<SoundBankPlayer>();
			if (component2 != null && !component2.playOnEnable)
			{
				component2.Play(projectileInstance.impactSoundVolumeOverride, projectileInstance.impactSoundPitchOverride);
			}
			if (projectileInstance.gameObject.activeSelf && projectileInstance.projectileOwner == creator)
			{
				projectileInstance.Deactivate();
			}
		}

		// Token: 0x04006579 RID: 25977
		public VRRig targetRig;

		// Token: 0x0400657A RID: 25978
		public Vector3 position;

		// Token: 0x0400657B RID: 25979
		public Color colour;

		// Token: 0x0400657C RID: 25980
		public int projectileIndex;
	}

	// Token: 0x02000D3B RID: 3387
	private class LaunchProjectileContainer : RoomSystem.ImpactFxContainer
	{
		// Token: 0x0600541B RID: 21531 RVA: 0x001BB470 File Offset: 0x001B9670
		public override void OnPlayFX()
		{
			GameObject gameObject = null;
			SlingshotProjectile slingshotProjectile = null;
			try
			{
				switch (this.projectileSource)
				{
				case RoomSystem.ProjectileSource.ProjectileWeapon:
					if (this.targetRig.projectileWeapon.IsNotNull() && this.targetRig.projectileWeapon.IsNotNull())
					{
						this.velocity = this.targetRig.ClampVelocityRelativeToPlayerSafe(this.velocity, 70f, 100f);
						SlingshotProjectile slingshotProjectile2 = this.targetRig.projectileWeapon.LaunchNetworkedProjectile(this.position, this.velocity, this.projectileSource, this.projectileIndex, this.targetRig.scaleFactor, this.overridecolour, this.colour, this.messageInfo);
						if (slingshotProjectile2.IsNotNull())
						{
							ProjectileTracker.AddRemotePlayerProjectile(this.messageInfo.Sender, slingshotProjectile2, this.projectileIndex, this.messageInfo.SentServerTime, this.velocity, this.position, this.targetRig.scaleFactor);
						}
					}
					return;
				case RoomSystem.ProjectileSource.LeftHand:
					this.tempThrowableGO = this.targetRig.myBodyDockPositions.GetLeftHandThrowable();
					break;
				case RoomSystem.ProjectileSource.RightHand:
					this.tempThrowableGO = this.targetRig.myBodyDockPositions.GetRightHandThrowable();
					break;
				default:
					return;
				}
				if (!this.tempThrowableGO.IsNull() && this.tempThrowableGO.TryGetComponent<SnowballThrowable>(out this.tempThrowableRef) && !(this.tempThrowableRef is GrowingSnowballThrowable))
				{
					this.velocity = this.targetRig.ClampVelocityRelativeToPlayerSafe(this.velocity, 50f, 100f);
					int projectileHash = this.tempThrowableRef.ProjectileHash;
					gameObject = ObjectPools.instance.Instantiate(projectileHash, true);
					slingshotProjectile = gameObject.GetComponent<SlingshotProjectile>();
					ProjectileTracker.AddRemotePlayerProjectile(this.targetRig.creator, slingshotProjectile, this.projectileIndex, this.messageInfo.SentServerTime, this.velocity, this.position, this.targetRig.scaleFactor);
					slingshotProjectile.Launch(this.position, this.velocity, this.messageInfo.Sender, false, false, this.projectileIndex, this.targetRig.scaleFactor, this.overridecolour, this.colour);
				}
			}
			catch
			{
				if (slingshotProjectile != null && slingshotProjectile)
				{
					slingshotProjectile.transform.position = Vector3.zero;
					slingshotProjectile.Deactivate();
				}
				else if (gameObject.IsNotNull())
				{
					ObjectPools.instance.Destroy(gameObject);
				}
			}
		}

		// Token: 0x0400657D RID: 25981
		public Vector3 velocity;

		// Token: 0x0400657E RID: 25982
		public RoomSystem.ProjectileSource projectileSource;

		// Token: 0x0400657F RID: 25983
		public bool overridecolour;

		// Token: 0x04006580 RID: 25984
		public PhotonMessageInfoWrapped messageInfo;

		// Token: 0x04006581 RID: 25985
		private GameObject tempThrowableGO;

		// Token: 0x04006582 RID: 25986
		private SnowballThrowable tempThrowableRef;
	}

	// Token: 0x02000D3C RID: 3388
	internal enum ProjectileSource
	{
		// Token: 0x04006584 RID: 25988
		ProjectileWeapon,
		// Token: 0x04006585 RID: 25989
		LeftHand,
		// Token: 0x04006586 RID: 25990
		RightHand
	}

	// Token: 0x02000D3D RID: 3389
	internal struct LavaSyncEventData
	{
		// Token: 0x04006587 RID: 25991
		public byte zone;

		// Token: 0x04006588 RID: 25992
		public byte state;

		// Token: 0x04006589 RID: 25993
		public double stateStartTime;

		// Token: 0x0400658A RID: 25994
		public float activationProgress;

		// Token: 0x0400658B RID: 25995
		public int voteCount;

		// Token: 0x0400658C RID: 25996
		public int senderActorNumber;

		// Token: 0x0400658D RID: 25997
		[FixedBuffer(typeof(int), 20)]
		public RoomSystem.LavaSyncEventData.<votes>e__FixedBuffer votes;

		// Token: 0x02000D3E RID: 3390
		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <votes>e__FixedBuffer
		{
			// Token: 0x0400658E RID: 25998
			public int FixedElementField;
		}
	}

	// Token: 0x02000D3F RID: 3391
	internal struct Events
	{
		// Token: 0x0400658F RID: 25999
		public const byte PROJECTILE = 0;

		// Token: 0x04006590 RID: 26000
		public const byte IMPACT = 1;

		// Token: 0x04006591 RID: 26001
		public const byte STATUS_EFFECT = 2;

		// Token: 0x04006592 RID: 26002
		public const byte SOUND_EFFECT = 3;

		// Token: 0x04006593 RID: 26003
		public const byte NEARBY_JOIN = 4;

		// Token: 0x04006594 RID: 26004
		public const byte PLAYER_TOUCHED = 5;

		// Token: 0x04006595 RID: 26005
		public const byte PLAYER_EFFECT = 6;

		// Token: 0x04006596 RID: 26006
		public const byte PARTY_JOIN = 7;

		// Token: 0x04006597 RID: 26007
		public const byte PLAYER_LAUNCHED = 8;

		// Token: 0x04006598 RID: 26008
		public const byte PLAYER_HIT = 9;

		// Token: 0x04006599 RID: 26009
		public const byte ELEVATOR_JOIN = 10;

		// Token: 0x0400659A RID: 26010
		public const byte SHUTTLE_JOIN = 11;

		// Token: 0x0400659B RID: 26011
		public const byte LAVA_SYNC = 12;

		// Token: 0x0400659C RID: 26012
		public const byte MONKE_BIZ_STATION__POINTS_REDEEMED = 13;

		// Token: 0x0400659D RID: 26013
		public const byte VOX_REQ_WORLD = 100;

		// Token: 0x0400659E RID: 26014
		public const byte VOX_REQ_OPERATION = 101;

		// Token: 0x0400659F RID: 26015
		public const byte VOX_REQ_MINE = 102;

		// Token: 0x040065A0 RID: 26016
		public const byte VOX_START_CHUNK = 103;

		// Token: 0x040065A1 RID: 26017
		public const byte VOX_CONTINUE_CHUNK = 104;

		// Token: 0x040065A2 RID: 26018
		public const byte VOX_SET_DENSITY = 105;

		// Token: 0x040065A3 RID: 26019
		public const byte VOX_MINE = 106;

		// Token: 0x040065A4 RID: 26020
		public const byte RPC = 255;
	}

	// Token: 0x02000D40 RID: 3392
	public enum StatusEffects
	{
		// Token: 0x040065A6 RID: 26022
		TaggedTime,
		// Token: 0x040065A7 RID: 26023
		JoinedTaggedTime,
		// Token: 0x040065A8 RID: 26024
		SetSlowedTime,
		// Token: 0x040065A9 RID: 26025
		UnTagged,
		// Token: 0x040065AA RID: 26026
		FrozenTime
	}

	// Token: 0x02000D41 RID: 3393
	public struct SoundEffect
	{
		// Token: 0x0600541D RID: 21533 RVA: 0x001BB6F8 File Offset: 0x001B98F8
		public SoundEffect(int soundID, float soundVolume, bool _stopCurrentAudio)
		{
			this.id = soundID;
			this.volume = soundVolume;
			this.volume = soundVolume;
			this.stopCurrentAudio = _stopCurrentAudio;
		}

		// Token: 0x040065AB RID: 26027
		public int id;

		// Token: 0x040065AC RID: 26028
		public float volume;

		// Token: 0x040065AD RID: 26029
		public bool stopCurrentAudio;
	}

	// Token: 0x02000D42 RID: 3394
	[Serializable]
	public struct PlayerEffectConfig
	{
		// Token: 0x040065AE RID: 26030
		public PlayerEffect type;

		// Token: 0x040065AF RID: 26031
		public TagEffectPack tagEffectPack;
	}
}
