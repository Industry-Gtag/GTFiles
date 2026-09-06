using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaNetworking;
using GorillaTag;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000D45 RID: 3397
public static class RoomControls
{
	// Token: 0x170007FB RID: 2043
	// (get) Token: 0x06005427 RID: 21543 RVA: 0x001BBA7B File Offset: 0x001B9C7B
	public static bool RoomControlsEnabled
	{
		get
		{
			return RoomControls.roomControlsEnabled;
		}
	}

	// Token: 0x170007FC RID: 2044
	// (get) Token: 0x06005428 RID: 21544 RVA: 0x001BBA82 File Offset: 0x001B9C82
	public static IReadOnlyDictionary<string, long> BlockedPlayers
	{
		get
		{
			return RoomControls.blockedPlayers;
		}
	}

	// Token: 0x170007FD RID: 2045
	// (get) Token: 0x06005429 RID: 21545 RVA: 0x001BBA89 File Offset: 0x001B9C89
	public static IReadOnlyDictionary<string, long> MutedPlayers
	{
		get
		{
			return RoomControls.mutedPlayers;
		}
	}

	// Token: 0x0600542A RID: 21546 RVA: 0x001BBA90 File Offset: 0x001B9C90
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void SubscribeToRoomEvents()
	{
		RoomSystem.JoinedRoomEvent += delegate
		{
			if (!RoomSystem.WasRoomSubscription)
			{
				return;
			}
			PhotonNetwork.AddCallbackTarget(RoomControls.punCallbacks);
			RoomControls.ApplyRoomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
			RoomControls.OnRoomStateLoaded.InvokeSafe();
		};
		RoomSystem.LeftRoomEvent += delegate
		{
			PhotonNetwork.RemoveCallbackTarget(RoomControls.punCallbacks);
			RoomControls.roomControlsEnabled = false;
			RoomControls.blockedPlayers.Clear();
			RoomControls.mutedPlayers.Clear();
		};
	}

	// Token: 0x0600542B RID: 21547 RVA: 0x001BBAF9 File Offset: 0x001B9CF9
	[OnExitPlay_Run]
	private static void RemovePunCallbacks()
	{
		PhotonNetwork.RemoveCallbackTarget(RoomControls.punCallbacks);
	}

	// Token: 0x0600542C RID: 21548 RVA: 0x001BBB08 File Offset: 0x001B9D08
	private static void ApplyRoomProperties(Hashtable properties)
	{
		if (!RoomControls.IsRoomControlsTrusted())
		{
			return;
		}
		object obj;
		if (properties.TryGetValue("roomControlsEnabled", out obj))
		{
			RoomControls.roomControlsEnabled = obj is bool && (bool)obj;
		}
		object obj2;
		if (properties.TryGetValue("blockedUsers", out obj2))
		{
			RoomControls.ApplyRoomProperty(obj2 as Hashtable, RoomControls.blockedPlayers);
		}
		object obj3;
		if (properties.TryGetValue("mutedUsers", out obj3))
		{
			RoomControls.ApplyRoomProperty(obj3 as Hashtable, RoomControls.mutedPlayers);
		}
	}

	// Token: 0x0600542D RID: 21549 RVA: 0x001BBB80 File Offset: 0x001B9D80
	private static void ReconcileRoomProperties(Hashtable propertiesThatChanged)
	{
		if (!RoomControls.IsRoomControlsTrusted())
		{
			return;
		}
		object obj;
		if (propertiesThatChanged.TryGetValue("roomControlsEnabled", out obj))
		{
			bool flag = obj is bool && (bool)obj;
			if (flag != RoomControls.roomControlsEnabled)
			{
				RoomControls.roomControlsEnabled = flag;
				RoomControls.OnRoomControlsEnabledChanged.InvokeSafe(in flag);
			}
		}
		object obj2;
		if (propertiesThatChanged.TryGetValue("blockedUsers", out obj2))
		{
			RoomControls.ReconcileRoomProperty(obj2 as Hashtable, RoomControls.blockedPlayers, RoomControls.OnPlayerBlockChanged);
		}
		object obj3;
		if (propertiesThatChanged.TryGetValue("mutedUsers", out obj3))
		{
			RoomControls.ReconcileRoomProperty(obj3 as Hashtable, RoomControls.mutedPlayers, RoomControls.OnPlayerMuteChanged);
		}
	}

	// Token: 0x0600542E RID: 21550 RVA: 0x001BBC18 File Offset: 0x001B9E18
	private static void ApplyRoomProperty(Hashtable source, Dictionary<string, long> destination)
	{
		destination.Clear();
		if (source == null)
		{
			return;
		}
		foreach (DictionaryEntry dictionaryEntry in source)
		{
			string text = dictionaryEntry.Key as string;
			if (text != null)
			{
				object value = dictionaryEntry.Value;
				if (value is long)
				{
					long num = (long)value;
					destination[text] = num;
				}
			}
		}
	}

	// Token: 0x0600542F RID: 21551 RVA: 0x001BBC9C File Offset: 0x001B9E9C
	private static void ReconcileRoomProperty(Hashtable source, Dictionary<string, long> destination, DelegateListProcessor<string, bool> onPropertyChanged)
	{
		HashSet<string> hashSet = new HashSet<string>(destination.Keys);
		RoomControls.ApplyRoomProperty(source, destination);
		foreach (string text in destination.Keys)
		{
			if (!hashSet.Remove(text))
			{
				bool flag = true;
				onPropertyChanged.InvokeSafe(in text, in flag);
			}
		}
		foreach (string text2 in hashSet)
		{
			bool flag = false;
			onPropertyChanged.InvokeSafe(in text2, in flag);
		}
	}

	// Token: 0x06005430 RID: 21552 RVA: 0x001BBD54 File Offset: 0x001B9F54
	private static bool IsRoomControlsTrusted()
	{
		GorillaServer instance = GorillaServer.Instance;
		return instance != null && instance.CheckRoomControlsEnabledForAnyone();
	}

	// Token: 0x06005431 RID: 21553 RVA: 0x001BBD68 File Offset: 0x001B9F68
	public static bool CanModerate()
	{
		string text;
		return RoomControls.CanModerate(out text);
	}

	// Token: 0x06005432 RID: 21554 RVA: 0x001BBD7C File Offset: 0x001B9F7C
	public static bool CanModerate(out string cannotReason)
	{
		if (!PhotonNetwork.InRoom)
		{
			cannotReason = "Not in a room";
			return false;
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			cannotReason = "The local player is not the master client";
			return false;
		}
		if (PhotonNetwork.CurrentRoom.IsVisible)
		{
			cannotReason = "The room is not a private room";
			return false;
		}
		if (!RoomSystem.WasRoomSubscription)
		{
			cannotReason = "The room was not a subscription room";
			return false;
		}
		if (!SubscriptionManager.IsLocalSubscribed())
		{
			cannotReason = "The local player is not a subscriber";
			return false;
		}
		if (!RoomControls.roomControlsEnabled)
		{
			cannotReason = "Room controls are disabled";
			return false;
		}
		cannotReason = null;
		return true;
	}

	// Token: 0x06005433 RID: 21555 RVA: 0x001BBDF2 File Offset: 0x001B9FF2
	public static void KickPlayer(int targetActorNumber)
	{
		if (!RoomControls.CanModerate())
		{
			return;
		}
		PhotonNetwork.RaiseEvent(100, new object[] { targetActorNumber, 0 }, RoomControls.RaiseToMasterClient, SendOptions.SendReliable);
	}

	// Token: 0x06005434 RID: 21556 RVA: 0x001BBE28 File Offset: 0x001BA028
	public static void KickAndBlockPlayer(int targetActorNumber, int howManySeconds = -1)
	{
		if (!RoomControls.CanModerate())
		{
			return;
		}
		object[] array2;
		if (howManySeconds >= 0)
		{
			object[] array = new object[2];
			array[0] = targetActorNumber;
			array2 = array;
			array[1] = howManySeconds;
		}
		else
		{
			(array2 = new object[1])[0] = targetActorNumber;
		}
		object[] array3 = array2;
		PhotonNetwork.RaiseEvent(100, array3, RoomControls.RaiseToMasterClient, SendOptions.SendReliable);
	}

	// Token: 0x06005435 RID: 21557 RVA: 0x001BBE7E File Offset: 0x001BA07E
	public static void UnblockPlayer(string targetUserId)
	{
		if (!RoomControls.CanModerate())
		{
			return;
		}
		PhotonNetwork.RaiseEvent(101, new object[] { targetUserId }, RoomControls.RaiseToMasterClient, SendOptions.SendReliable);
	}

	// Token: 0x06005436 RID: 21558 RVA: 0x001BBEA4 File Offset: 0x001BA0A4
	public static void MutePlayer(int targetActorNumber, int howManySeconds = -1)
	{
		if (!RoomControls.CanModerate())
		{
			return;
		}
		object[] array2;
		if (howManySeconds >= 0)
		{
			object[] array = new object[2];
			array[0] = targetActorNumber;
			array2 = array;
			array[1] = howManySeconds;
		}
		else
		{
			(array2 = new object[1])[0] = targetActorNumber;
		}
		object[] array3 = array2;
		PhotonNetwork.RaiseEvent(102, array3, RoomControls.RaiseToMasterClient, SendOptions.SendReliable);
	}

	// Token: 0x06005437 RID: 21559 RVA: 0x001BBEFA File Offset: 0x001BA0FA
	public static void UnmutePlayer(string targetUserId)
	{
		if (!RoomControls.CanModerate())
		{
			return;
		}
		PhotonNetwork.RaiseEvent(103, new object[] { targetUserId }, RoomControls.RaiseToMasterClient, SendOptions.SendReliable);
	}

	// Token: 0x040065B9 RID: 26041
	private const byte BlockPlayerEventCode = 100;

	// Token: 0x040065BA RID: 26042
	private const byte UnblockPlayerEventCode = 101;

	// Token: 0x040065BB RID: 26043
	private const byte MutePlayerEventCode = 102;

	// Token: 0x040065BC RID: 26044
	private const byte UnmutePlayerEventCode = 103;

	// Token: 0x040065BD RID: 26045
	private const string RoomControlsEnabledRoomPropertyKey = "roomControlsEnabled";

	// Token: 0x040065BE RID: 26046
	private const string BlockedPlayersRoomPropertyKey = "blockedUsers";

	// Token: 0x040065BF RID: 26047
	private const string MutedPlayersRoomPropertyKey = "mutedUsers";

	// Token: 0x040065C0 RID: 26048
	[OnEnterPlay_Set(false)]
	private static bool roomControlsEnabled = false;

	// Token: 0x040065C1 RID: 26049
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor<bool> OnRoomControlsEnabledChanged = new DelegateListProcessor<bool>();

	// Token: 0x040065C2 RID: 26050
	[OnEnterPlay_Clear]
	private static readonly Dictionary<string, long> blockedPlayers = new Dictionary<string, long>();

	// Token: 0x040065C3 RID: 26051
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor<string, bool> OnPlayerBlockChanged = new DelegateListProcessor<string, bool>();

	// Token: 0x040065C4 RID: 26052
	[OnEnterPlay_Clear]
	private static readonly Dictionary<string, long> mutedPlayers = new Dictionary<string, long>();

	// Token: 0x040065C5 RID: 26053
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor<string, bool> OnPlayerMuteChanged = new DelegateListProcessor<string, bool>();

	// Token: 0x040065C6 RID: 26054
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor OnRoomStateLoaded = new DelegateListProcessor();

	// Token: 0x040065C7 RID: 26055
	private static readonly RoomControls.PunCallbacks punCallbacks = new RoomControls.PunCallbacks();

	// Token: 0x040065C8 RID: 26056
	private static readonly RaiseEventOptions RaiseToMasterClient = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.MasterClient
	};

	// Token: 0x02000D46 RID: 3398
	private class PunCallbacks : IInRoomCallbacks
	{
		// Token: 0x06005439 RID: 21561 RVA: 0x001BBF8A File Offset: 0x001BA18A
		void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			RoomControls.ReconcileRoomProperties(propertiesThatChanged);
		}

		// Token: 0x0600543A RID: 21562 RVA: 0x00002C2D File Offset: 0x00000E2D
		void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
		{
		}

		// Token: 0x0600543B RID: 21563 RVA: 0x00002C2D File Offset: 0x00000E2D
		void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
		{
		}

		// Token: 0x0600543C RID: 21564 RVA: 0x00002C2D File Offset: 0x00000E2D
		void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
		}

		// Token: 0x0600543D RID: 21565 RVA: 0x00002C2D File Offset: 0x00000E2D
		void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
		{
		}
	}
}
