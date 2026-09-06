using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Realtime;

// Token: 0x0200046F RID: 1135
public class RoomConfig
{
	// Token: 0x170002E9 RID: 745
	// (get) Token: 0x06001B9A RID: 7066 RVA: 0x00095B0F File Offset: 0x00093D0F
	public Hashtable EffectiveSearchFilter
	{
		get
		{
			return this.SearchFilter ?? this.CustomProps;
		}
	}

	// Token: 0x170002EA RID: 746
	// (get) Token: 0x06001B9B RID: 7067 RVA: 0x00095B21 File Offset: 0x00093D21
	public bool IsJoiningWithFriends
	{
		get
		{
			return this.joinFriendIDs != null && this.joinFriendIDs.Length != 0;
		}
	}

	// Token: 0x06001B9C RID: 7068 RVA: 0x00095B38 File Offset: 0x00093D38
	public void SetFriendIDs(List<string> friendIDs)
	{
		for (int i = 0; i < friendIDs.Count; i++)
		{
			if (friendIDs[i] == NetworkSystem.Instance.GetMyNickName())
			{
				friendIDs.RemoveAt(i);
				i--;
			}
		}
		this.joinFriendIDs = new string[friendIDs.Count];
		for (int j = 0; j < friendIDs.Count; j++)
		{
			this.joinFriendIDs[j] = friendIDs[j];
		}
	}

	// Token: 0x06001B9D RID: 7069 RVA: 0x00095BAA File Offset: 0x00093DAA
	public void ClearExpectedUsers()
	{
		if (this.joinFriendIDs == null || this.joinFriendIDs.Length == 0)
		{
			return;
		}
		this.joinFriendIDs = new string[0];
	}

	// Token: 0x06001B9E RID: 7070 RVA: 0x00095BCC File Offset: 0x00093DCC
	public RoomOptions ToPUNOpts()
	{
		return new RoomOptions
		{
			IsVisible = this.isPublic,
			IsOpen = this.isJoinable,
			MaxPlayers = this.MaxPlayers,
			CustomRoomProperties = this.CustomProps,
			PublishUserId = true,
			CustomRoomPropertiesForLobby = this.AutoCustomLobbyProps()
		};
	}

	// Token: 0x06001B9F RID: 7071 RVA: 0x00095C21 File Offset: 0x00093E21
	public void SetFusionOpts(NetworkRunner runnerInst)
	{
		runnerInst.SessionInfo.IsVisible = this.isPublic;
		runnerInst.SessionInfo.IsOpen = this.isJoinable;
	}

	// Token: 0x06001BA0 RID: 7072 RVA: 0x00095C45 File Offset: 0x00093E45
	public static RoomConfig SPConfig()
	{
		return new RoomConfig
		{
			isPublic = false,
			isJoinable = false,
			MaxPlayers = 1
		};
	}

	// Token: 0x06001BA1 RID: 7073 RVA: 0x00095C61 File Offset: 0x00093E61
	public static RoomConfig AnyPublicConfig()
	{
		return new RoomConfig
		{
			isPublic = true,
			isJoinable = true,
			createIfMissing = true,
			MaxPlayers = 10
		};
	}

	// Token: 0x06001BA2 RID: 7074 RVA: 0x00095C88 File Offset: 0x00093E88
	private string[] AutoCustomLobbyProps()
	{
		string[] array = new string[this.CustomProps.Count];
		int num = 0;
		foreach (DictionaryEntry dictionaryEntry in this.CustomProps)
		{
			array[num] = (string)dictionaryEntry.Key;
			num++;
		}
		return array;
	}

	// Token: 0x040025C6 RID: 9670
	public const string Room_GameModePropKey = "gameMode";

	// Token: 0x040025C7 RID: 9671
	public const string Room_PlatformPropKey = "platform";

	// Token: 0x040025C8 RID: 9672
	public const string Room_ScheduledEventStatePropKey = "scheduledEventState";

	// Token: 0x040025C9 RID: 9673
	public bool isPublic;

	// Token: 0x040025CA RID: 9674
	public bool isJoinable;

	// Token: 0x040025CB RID: 9675
	public byte MaxPlayers;

	// Token: 0x040025CC RID: 9676
	public Hashtable CustomProps = new Hashtable();

	// Token: 0x040025CD RID: 9677
	public Hashtable SearchFilter;

	// Token: 0x040025CE RID: 9678
	public bool createIfMissing;

	// Token: 0x040025CF RID: 9679
	public string[] joinFriendIDs;
}
