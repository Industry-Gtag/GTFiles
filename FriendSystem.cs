using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000CC2 RID: 3266
public class FriendSystem : MonoBehaviour
{
	// Token: 0x170007B6 RID: 1974
	// (get) Token: 0x0600510F RID: 20751 RVA: 0x001AE352 File Offset: 0x001AC552
	public FriendSystem.PlayerPrivacy LocalPlayerPrivacy
	{
		get
		{
			return this.localPlayerPrivacy;
		}
	}

	// Token: 0x1400008F RID: 143
	// (add) Token: 0x06005110 RID: 20752 RVA: 0x001AE35C File Offset: 0x001AC55C
	// (remove) Token: 0x06005111 RID: 20753 RVA: 0x001AE394 File Offset: 0x001AC594
	public event Action<List<FriendBackendController.Friend>> OnFriendListRefresh;

	// Token: 0x06005112 RID: 20754 RVA: 0x001AE3CC File Offset: 0x001AC5CC
	public void SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy privacyState)
	{
		this.localPlayerPrivacy = privacyState;
		FriendBackendController.PrivacyState privacyState2;
		switch (privacyState)
		{
		default:
			privacyState2 = FriendBackendController.PrivacyState.VISIBLE;
			break;
		case FriendSystem.PlayerPrivacy.PublicOnly:
			privacyState2 = FriendBackendController.PrivacyState.PUBLIC_ONLY;
			break;
		case FriendSystem.PlayerPrivacy.Hidden:
			privacyState2 = FriendBackendController.PrivacyState.HIDDEN;
			break;
		}
		FriendBackendController.Instance.SetPrivacyState(privacyState2);
	}

	// Token: 0x06005113 RID: 20755 RVA: 0x001AE409 File Offset: 0x001AC609
	public void RefreshFriendsList()
	{
		FriendBackendController.Instance.GetFriends();
	}

	// Token: 0x06005114 RID: 20756 RVA: 0x001AE418 File Offset: 0x001AC618
	public void SendFriendRequest(NetPlayer targetPlayer, GTZone stationZone, FriendSystem.FriendRequestCallback callback)
	{
		FriendSystem.FriendRequestData friendRequestData = new FriendSystem.FriendRequestData
		{
			completionCallback = callback,
			sendingPlayerId = NetworkSystem.Instance.LocalPlayer.UserId.GetHashCode(),
			targetPlayerId = targetPlayer.UserId.GetHashCode(),
			localTimeSent = Time.realtimeSinceStartup,
			zone = stationZone
		};
		this.pendingFriendRequests.Add(friendRequestData);
		FriendBackendController.Instance.AddFriend(targetPlayer);
	}

	// Token: 0x06005115 RID: 20757 RVA: 0x001AE494 File Offset: 0x001AC694
	public void RemoveFriend(FriendBackendController.Friend friend, FriendSystem.FriendRemovalCallback callback = null)
	{
		this.pendingFriendRemovals.Add(new FriendSystem.FriendRemovalData
		{
			completionCallback = callback,
			targetPlayerId = friend.Presence.FriendLinkId.GetHashCode(),
			localTimeSent = Time.realtimeSinceStartup
		});
		FriendBackendController.Instance.RemoveFriend(friend);
	}

	// Token: 0x06005116 RID: 20758 RVA: 0x001AE4F0 File Offset: 0x001AC6F0
	public bool HasPendingFriendRequest(GTZone zone, int senderId)
	{
		for (int i = 0; i < this.pendingFriendRequests.Count; i++)
		{
			if (this.pendingFriendRequests[i].zone == zone && this.pendingFriendRequests[i].sendingPlayerId == senderId)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06005117 RID: 20759 RVA: 0x001AE540 File Offset: 0x001AC740
	public bool CheckFriendshipWithPlayer(int targetActorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(targetActorNumber);
		if (player != null)
		{
			int hashCode = player.UserId.GetHashCode();
			List<FriendBackendController.Friend> friendsList = FriendBackendController.Instance.FriendsList;
			for (int i = 0; i < friendsList.Count; i++)
			{
				if (friendsList[i] != null && friendsList[i].Presence != null && friendsList[i].Presence.FriendLinkId.GetHashCode() == hashCode)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06005118 RID: 20760 RVA: 0x001AE5B9 File Offset: 0x001AC7B9
	private void Awake()
	{
		if (FriendSystem.Instance == null)
		{
			FriendSystem.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06005119 RID: 20761 RVA: 0x001AE5DC File Offset: 0x001AC7DC
	private void Start()
	{
		FriendBackendController.Instance.OnGetFriendsComplete += this.OnGetFriendsReturned;
		FriendBackendController.Instance.OnAddFriendComplete += this.OnAddFriendReturned;
		FriendBackendController.Instance.OnRemoveFriendComplete += this.OnRemoveFriendReturned;
	}

	// Token: 0x0600511A RID: 20762 RVA: 0x001AE634 File Offset: 0x001AC834
	private void OnDestroy()
	{
		if (FriendBackendController.Instance != null)
		{
			FriendBackendController.Instance.OnGetFriendsComplete -= this.OnGetFriendsReturned;
			FriendBackendController.Instance.OnAddFriendComplete -= this.OnAddFriendReturned;
			FriendBackendController.Instance.OnRemoveFriendComplete -= this.OnRemoveFriendReturned;
		}
	}

	// Token: 0x0600511B RID: 20763 RVA: 0x001AE698 File Offset: 0x001AC898
	private void OnGetFriendsReturned(bool succeeded)
	{
		if (succeeded)
		{
			this.lastFriendsListRefresh = Time.realtimeSinceStartup;
			switch (FriendBackendController.Instance.MyPrivacyState)
			{
			default:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.Visible;
				break;
			case FriendBackendController.PrivacyState.PUBLIC_ONLY:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.PublicOnly;
				break;
			case FriendBackendController.PrivacyState.HIDDEN:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.Hidden;
				break;
			}
			Action<List<FriendBackendController.Friend>> onFriendListRefresh = this.OnFriendListRefresh;
			if (onFriendListRefresh == null)
			{
				return;
			}
			onFriendListRefresh(FriendBackendController.Instance.FriendsList);
		}
	}

	// Token: 0x0600511C RID: 20764 RVA: 0x001AE708 File Offset: 0x001AC908
	private void OnAddFriendReturned(NetPlayer targetPlayer, bool succeeded)
	{
		int hashCode = targetPlayer.UserId.GetHashCode();
		this.indexesToRemove.Clear();
		for (int i = 0; i < this.pendingFriendRequests.Count; i++)
		{
			if (this.pendingFriendRequests[i].targetPlayerId == hashCode)
			{
				FriendSystem.FriendRequestCallback completionCallback = this.pendingFriendRequests[i].completionCallback;
				if (completionCallback != null)
				{
					completionCallback(this.pendingFriendRequests[i].zone, this.pendingFriendRequests[i].sendingPlayerId, this.pendingFriendRequests[i].targetPlayerId, succeeded);
				}
				this.indexesToRemove.Add(i);
			}
			else if (this.pendingFriendRequests[i].localTimeSent + this.friendRequestExpirationTime < Time.realtimeSinceStartup)
			{
				this.indexesToRemove.Add(i);
			}
		}
		for (int j = this.indexesToRemove.Count - 1; j >= 0; j--)
		{
			this.pendingFriendRequests.RemoveAt(this.indexesToRemove[j]);
		}
	}

	// Token: 0x0600511D RID: 20765 RVA: 0x001AE814 File Offset: 0x001ACA14
	private void OnRemoveFriendReturned(FriendBackendController.Friend friend, bool succeeded)
	{
		if (friend != null && friend.Presence != null)
		{
			int hashCode = friend.Presence.FriendLinkId.GetHashCode();
			this.indexesToRemove.Clear();
			for (int i = 0; i < this.pendingFriendRemovals.Count; i++)
			{
				if (this.pendingFriendRemovals[i].targetPlayerId == hashCode)
				{
					FriendSystem.FriendRemovalCallback completionCallback = this.pendingFriendRemovals[i].completionCallback;
					if (completionCallback != null)
					{
						completionCallback(hashCode, succeeded);
					}
					this.indexesToRemove.Add(i);
				}
				else if (this.pendingFriendRemovals[i].localTimeSent + this.friendRequestExpirationTime < Time.realtimeSinceStartup)
				{
					this.indexesToRemove.Add(i);
				}
			}
			for (int j = this.indexesToRemove.Count - 1; j >= 0; j--)
			{
				this.pendingFriendRemovals.RemoveAt(this.indexesToRemove[j]);
			}
		}
	}

	// Token: 0x0400630D RID: 25357
	[OnEnterPlay_SetNull]
	public static volatile FriendSystem Instance;

	// Token: 0x0400630E RID: 25358
	[SerializeField]
	private float friendRequestExpirationTime = 10f;

	// Token: 0x0400630F RID: 25359
	private FriendSystem.PlayerPrivacy localPlayerPrivacy;

	// Token: 0x04006310 RID: 25360
	private List<FriendSystem.FriendRequestData> pendingFriendRequests = new List<FriendSystem.FriendRequestData>();

	// Token: 0x04006311 RID: 25361
	private List<FriendSystem.FriendRemovalData> pendingFriendRemovals = new List<FriendSystem.FriendRemovalData>();

	// Token: 0x04006312 RID: 25362
	private List<int> indexesToRemove = new List<int>();

	// Token: 0x04006314 RID: 25364
	private float lastFriendsListRefresh;

	// Token: 0x02000CC3 RID: 3267
	// (Invoke) Token: 0x06005120 RID: 20768
	public delegate void FriendRequestCallback(GTZone zone, int localId, int friendId, bool success);

	// Token: 0x02000CC4 RID: 3268
	private struct FriendRequestData
	{
		// Token: 0x04006315 RID: 25365
		public GTZone zone;

		// Token: 0x04006316 RID: 25366
		public int sendingPlayerId;

		// Token: 0x04006317 RID: 25367
		public int targetPlayerId;

		// Token: 0x04006318 RID: 25368
		public float localTimeSent;

		// Token: 0x04006319 RID: 25369
		public FriendSystem.FriendRequestCallback completionCallback;
	}

	// Token: 0x02000CC5 RID: 3269
	// (Invoke) Token: 0x06005124 RID: 20772
	public delegate void FriendRemovalCallback(int friendId, bool success);

	// Token: 0x02000CC6 RID: 3270
	private struct FriendRemovalData
	{
		// Token: 0x0400631A RID: 25370
		public int targetPlayerId;

		// Token: 0x0400631B RID: 25371
		public float localTimeSent;

		// Token: 0x0400631C RID: 25372
		public FriendSystem.FriendRemovalCallback completionCallback;
	}

	// Token: 0x02000CC7 RID: 3271
	private enum FriendRequestStatus
	{
		// Token: 0x0400631E RID: 25374
		Pending,
		// Token: 0x0400631F RID: 25375
		Succeeded,
		// Token: 0x04006320 RID: 25376
		Failed
	}

	// Token: 0x02000CC8 RID: 3272
	public enum PlayerPrivacy
	{
		// Token: 0x04006322 RID: 25378
		Visible,
		// Token: 0x04006323 RID: 25379
		PublicOnly,
		// Token: 0x04006324 RID: 25380
		Hidden
	}
}
