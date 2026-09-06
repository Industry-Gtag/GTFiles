using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000CA9 RID: 3241
public class FriendBackendController : MonoBehaviour
{
	// Token: 0x1400008B RID: 139
	// (add) Token: 0x06004FF3 RID: 20467 RVA: 0x001A95AC File Offset: 0x001A77AC
	// (remove) Token: 0x06004FF4 RID: 20468 RVA: 0x001A95E4 File Offset: 0x001A77E4
	public event Action<bool> OnGetFriendsComplete;

	// Token: 0x1400008C RID: 140
	// (add) Token: 0x06004FF5 RID: 20469 RVA: 0x001A961C File Offset: 0x001A781C
	// (remove) Token: 0x06004FF6 RID: 20470 RVA: 0x001A9654 File Offset: 0x001A7854
	public event Action<bool> OnSetPrivacyStateComplete;

	// Token: 0x1400008D RID: 141
	// (add) Token: 0x06004FF7 RID: 20471 RVA: 0x001A968C File Offset: 0x001A788C
	// (remove) Token: 0x06004FF8 RID: 20472 RVA: 0x001A96C4 File Offset: 0x001A78C4
	public event Action<NetPlayer, bool> OnAddFriendComplete;

	// Token: 0x1400008E RID: 142
	// (add) Token: 0x06004FF9 RID: 20473 RVA: 0x001A96FC File Offset: 0x001A78FC
	// (remove) Token: 0x06004FFA RID: 20474 RVA: 0x001A9734 File Offset: 0x001A7934
	public event Action<FriendBackendController.Friend, bool> OnRemoveFriendComplete;

	// Token: 0x17000770 RID: 1904
	// (get) Token: 0x06004FFB RID: 20475 RVA: 0x001A9769 File Offset: 0x001A7969
	public List<FriendBackendController.Friend> FriendsList
	{
		get
		{
			return this.lastFriendsList;
		}
	}

	// Token: 0x17000771 RID: 1905
	// (get) Token: 0x06004FFC RID: 20476 RVA: 0x001A9771 File Offset: 0x001A7971
	public FriendBackendController.PrivacyState MyPrivacyState
	{
		get
		{
			return this.lastPrivacyState;
		}
	}

	// Token: 0x06004FFD RID: 20477 RVA: 0x001A9779 File Offset: 0x001A7979
	public void GetFriends()
	{
		if (!this.getFriendsInProgress)
		{
			this.getFriendsInProgress = true;
			this.GetFriendsInternal();
		}
	}

	// Token: 0x06004FFE RID: 20478 RVA: 0x001A9790 File Offset: 0x001A7990
	public void SetPrivacyState(FriendBackendController.PrivacyState state)
	{
		if (!this.setPrivacyStateInProgress)
		{
			this.setPrivacyStateInProgress = true;
			this.setPrivacyStateState = state;
			this.SetPrivacyStateInternal();
			return;
		}
		this.setPrivacyStateQueue.Enqueue(state);
	}

	// Token: 0x06004FFF RID: 20479 RVA: 0x001A97BC File Offset: 0x001A79BC
	public void AddFriend(NetPlayer target)
	{
		if (target == null)
		{
			return;
		}
		int hashCode = target.UserId.GetHashCode();
		if (!this.addFriendInProgress)
		{
			this.addFriendInProgress = true;
			this.addFriendTargetIdHash = hashCode;
			this.addFriendTargetPlayer = target;
			this.AddFriendInternal();
			return;
		}
		if (hashCode != this.addFriendTargetIdHash && !this.addFriendRequestQueue.Contains(new ValueTuple<int, NetPlayer>(hashCode, target)))
		{
			this.addFriendRequestQueue.Enqueue(new ValueTuple<int, NetPlayer>(hashCode, target));
		}
	}

	// Token: 0x06005000 RID: 20480 RVA: 0x001A982C File Offset: 0x001A7A2C
	public void RemoveFriend(FriendBackendController.Friend target)
	{
		if (target == null)
		{
			return;
		}
		int hashCode = target.Presence.FriendLinkId.GetHashCode();
		if (!this.removeFriendInProgress)
		{
			this.removeFriendInProgress = true;
			this.removeFriendTargetIdHash = hashCode;
			this.removeFriendTarget = target;
			this.RemoveFriendInternal();
			return;
		}
		if (hashCode != this.addFriendTargetIdHash && !this.removeFriendRequestQueue.Contains(new ValueTuple<int, FriendBackendController.Friend>(hashCode, target)))
		{
			this.removeFriendRequestQueue.Enqueue(new ValueTuple<int, FriendBackendController.Friend>(hashCode, target));
		}
	}

	// Token: 0x06005001 RID: 20481 RVA: 0x001A98A1 File Offset: 0x001A7AA1
	private void Awake()
	{
		if (FriendBackendController.Instance == null)
		{
			FriendBackendController.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06005002 RID: 20482 RVA: 0x001A98C4 File Offset: 0x001A7AC4
	private void GetFriendsInternal()
	{
		base.StartCoroutine(this.SendGetFriendsRequest(new FriendBackendController.GetFriendsRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipId = ""
		}, new Action<FriendBackendController.GetFriendsResponse>(this.GetFriendsComplete)));
	}

	// Token: 0x06005003 RID: 20483 RVA: 0x001A991E File Offset: 0x001A7B1E
	private IEnumerator SendGetFriendsRequest(FriendBackendController.GetFriendsRequest data, Action<FriendBackendController.GetFriendsResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/GetFriendsV2", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			FriendBackendController.GetFriendsResponse getFriendsResponse = JsonConvert.DeserializeObject<FriendBackendController.GetFriendsResponse>(request.downloadHandler.text);
			callback(getFriendsResponse);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.getFriendsRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.getFriendsRetryCount + 1));
				this.getFriendsRetryCount++;
				yield return new WaitForSecondsRealtime((float)num);
				this.GetFriendsInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum GetFriends retries attempted. Please check your network connection.", null);
				this.getFriendsRetryCount = 0;
				callback(null);
			}
		}
		else
		{
			this.getFriendsInProgress = false;
		}
		yield break;
	}

	// Token: 0x06005004 RID: 20484 RVA: 0x001A993C File Offset: 0x001A7B3C
	private void GetFriendsComplete([CanBeNull] FriendBackendController.GetFriendsResponse response)
	{
		this.getFriendsInProgress = false;
		if (response != null)
		{
			this.lastGetFriendsResponse = response;
			if (this.lastGetFriendsResponse.Result != null)
			{
				this.lastPrivacyState = this.lastGetFriendsResponse.Result.MyPrivacyState;
				if (this.lastGetFriendsResponse.Result.Friends != null)
				{
					this.lastFriendsList.Clear();
					foreach (FriendBackendController.Friend friend in this.lastGetFriendsResponse.Result.Friends)
					{
						this.lastFriendsList.Add(friend);
					}
				}
			}
			Action<bool> onGetFriendsComplete = this.OnGetFriendsComplete;
			if (onGetFriendsComplete == null)
			{
				return;
			}
			onGetFriendsComplete(true);
			return;
		}
		else
		{
			Action<bool> onGetFriendsComplete2 = this.OnGetFriendsComplete;
			if (onGetFriendsComplete2 == null)
			{
				return;
			}
			onGetFriendsComplete2(false);
			return;
		}
	}

	// Token: 0x06005005 RID: 20485 RVA: 0x001A9A18 File Offset: 0x001A7C18
	public void CreateTestFriends()
	{
		Debug.Log("Adding test friends");
		for (int i = 0; i < 15; i++)
		{
			FriendBackendController.FriendPresence friendPresence = new FriendBackendController.FriendPresence();
			friendPresence.FriendLinkId = i.ToString();
			friendPresence.UserName = i.ToString();
			friendPresence.RoomId = i.ToString();
			friendPresence.Zone = "TreeHouse";
			friendPresence.Region = "Jungle";
			friendPresence.IsPublic = new bool?(true);
			FriendBackendController.Friend friend = new FriendBackendController.Friend();
			friend.Presence = friendPresence;
			friend.Created = DateTime.Now;
			this.FriendsList.Add(friend);
		}
	}

	// Token: 0x06005006 RID: 20486 RVA: 0x001A9AB0 File Offset: 0x001A7CB0
	private void SetPrivacyStateInternal()
	{
		base.StartCoroutine(this.SendSetPrivacyStateRequest(new FriendBackendController.SetPrivacyStateRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			PrivacyState = this.setPrivacyStateState.ToString()
		}, new Action<FriendBackendController.SetPrivacyStateResponse>(this.SetPrivacyStateComplete)));
	}

	// Token: 0x06005007 RID: 20487 RVA: 0x001A9B16 File Offset: 0x001A7D16
	private IEnumerator SendSetPrivacyStateRequest(FriendBackendController.SetPrivacyStateRequest data, Action<FriendBackendController.SetPrivacyStateResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/SetPrivacyState", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			FriendBackendController.SetPrivacyStateResponse setPrivacyStateResponse = JsonConvert.DeserializeObject<FriendBackendController.SetPrivacyStateResponse>(request.downloadHandler.text);
			callback(setPrivacyStateResponse);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.setPrivacyStateRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.setPrivacyStateRetryCount + 1));
				this.setPrivacyStateRetryCount++;
				yield return new WaitForSecondsRealtime((float)num);
				this.SetPrivacyStateInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum SetPrivacyState retries attempted. Please check your network connection.", null);
				this.setPrivacyStateRetryCount = 0;
				callback(null);
			}
		}
		else
		{
			this.setPrivacyStateInProgress = false;
		}
		yield break;
	}

	// Token: 0x06005008 RID: 20488 RVA: 0x001A9B34 File Offset: 0x001A7D34
	private void SetPrivacyStateComplete([CanBeNull] FriendBackendController.SetPrivacyStateResponse response)
	{
		this.setPrivacyStateInProgress = false;
		if (response != null)
		{
			this.lastPrivacyStateResponse = response;
			Action<bool> onSetPrivacyStateComplete = this.OnSetPrivacyStateComplete;
			if (onSetPrivacyStateComplete != null)
			{
				onSetPrivacyStateComplete(true);
			}
		}
		else
		{
			Action<bool> onSetPrivacyStateComplete2 = this.OnSetPrivacyStateComplete;
			if (onSetPrivacyStateComplete2 != null)
			{
				onSetPrivacyStateComplete2(false);
			}
		}
		if (this.setPrivacyStateQueue.Count > 0)
		{
			FriendBackendController.PrivacyState privacyState = this.setPrivacyStateQueue.Dequeue();
			this.SetPrivacyState(privacyState);
		}
	}

	// Token: 0x06005009 RID: 20489 RVA: 0x001A9B9C File Offset: 0x001A7D9C
	private void AddFriendInternal()
	{
		base.StartCoroutine(this.SendAddFriendRequest(new FriendBackendController.FriendRequestRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			MothershipId = "",
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipToken = "",
			MyFriendLinkId = NetworkSystem.Instance.LocalPlayer.UserId,
			FriendFriendLinkId = this.addFriendTargetPlayer.UserId
		}, new Action<bool>(this.AddFriendComplete)));
	}

	// Token: 0x0600500A RID: 20490 RVA: 0x001A9C27 File Offset: 0x001A7E27
	private IEnumerator SendAddFriendRequest(FriendBackendController.FriendRequestRequest data, Action<bool> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/RequestFriend", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			callback(true);
		}
		else
		{
			if (request.responseCode == 409L)
			{
				flag = false;
			}
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.addFriendRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.addFriendRetryCount + 1));
				this.addFriendRetryCount++;
				yield return new WaitForSecondsRealtime((float)num);
				this.AddFriendInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum AddFriend retries attempted. Please check your network connection.", null);
				this.addFriendRetryCount = 0;
				callback(false);
			}
		}
		else
		{
			this.addFriendInProgress = false;
		}
		yield break;
	}

	// Token: 0x0600500B RID: 20491 RVA: 0x001A9C44 File Offset: 0x001A7E44
	private void AddFriendComplete([CanBeNull] bool success)
	{
		if (success)
		{
			Action<NetPlayer, bool> onAddFriendComplete = this.OnAddFriendComplete;
			if (onAddFriendComplete != null)
			{
				onAddFriendComplete(this.addFriendTargetPlayer, true);
			}
		}
		else
		{
			Action<NetPlayer, bool> onAddFriendComplete2 = this.OnAddFriendComplete;
			if (onAddFriendComplete2 != null)
			{
				onAddFriendComplete2(this.addFriendTargetPlayer, false);
			}
		}
		this.addFriendInProgress = false;
		this.addFriendTargetIdHash = 0;
		this.addFriendTargetPlayer = null;
		if (this.addFriendRequestQueue.Count > 0)
		{
			ValueTuple<int, NetPlayer> valueTuple = this.addFriendRequestQueue.Dequeue();
			this.AddFriend(valueTuple.Item2);
		}
	}

	// Token: 0x0600500C RID: 20492 RVA: 0x001A9CC4 File Offset: 0x001A7EC4
	private void RemoveFriendInternal()
	{
		base.StartCoroutine(this.SendRemoveFriendRequest(new FriendBackendController.RemoveFriendRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			MothershipId = "",
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MyFriendLinkId = NetworkSystem.Instance.LocalPlayer.UserId,
			FriendFriendLinkId = this.removeFriendTarget.Presence.FriendLinkId
		}, new Action<bool>(this.RemoveFriendComplete)));
	}

	// Token: 0x0600500D RID: 20493 RVA: 0x001A9D49 File Offset: 0x001A7F49
	private IEnumerator SendRemoveFriendRequest(FriendBackendController.RemoveFriendRequest data, Action<bool> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/RemoveFriend", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			callback(true);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.removeFriendRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.removeFriendRetryCount + 1));
				this.removeFriendRetryCount++;
				yield return new WaitForSecondsRealtime((float)num);
				this.AddFriendInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum AddFriend retries attempted. Please check your network connection.", null);
				this.removeFriendRetryCount = 0;
				callback(false);
			}
		}
		else
		{
			this.removeFriendInProgress = false;
		}
		yield break;
	}

	// Token: 0x0600500E RID: 20494 RVA: 0x001A9D68 File Offset: 0x001A7F68
	private void RemoveFriendComplete([CanBeNull] bool success)
	{
		if (success)
		{
			Action<FriendBackendController.Friend, bool> onRemoveFriendComplete = this.OnRemoveFriendComplete;
			if (onRemoveFriendComplete != null)
			{
				onRemoveFriendComplete(this.removeFriendTarget, true);
			}
		}
		else
		{
			Action<FriendBackendController.Friend, bool> onRemoveFriendComplete2 = this.OnRemoveFriendComplete;
			if (onRemoveFriendComplete2 != null)
			{
				onRemoveFriendComplete2(this.removeFriendTarget, false);
			}
		}
		this.removeFriendInProgress = false;
		this.removeFriendTargetIdHash = 0;
		this.removeFriendTarget = null;
		if (this.removeFriendRequestQueue.Count > 0)
		{
			ValueTuple<int, FriendBackendController.Friend> valueTuple = this.removeFriendRequestQueue.Dequeue();
			this.RemoveFriend(valueTuple.Item2);
		}
	}

	// Token: 0x0600500F RID: 20495 RVA: 0x001A9DE8 File Offset: 0x001A7FE8
	private void LogNetPlayersInRoom()
	{
		Debug.Log("Local Player PlayfabId: " + PlayFabAuthenticator.instance.GetPlayFabPlayerId());
		int num = 0;
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			Debug.Log(string.Format("[{0}] Player: {1}, ActorNumber: {2}, UserID: {3}, IsMasterClient: {4}", new object[] { num, netPlayer.NickName, netPlayer.ActorNumber, netPlayer.UserId, netPlayer.IsMasterClient }));
			num++;
		}
	}

	// Token: 0x06005010 RID: 20496 RVA: 0x001A9E80 File Offset: 0x001A8080
	private void TestAddFriend()
	{
		this.OnAddFriendComplete -= this.TestAddFriendCompleteCallback;
		this.OnAddFriendComplete += this.TestAddFriendCompleteCallback;
		NetPlayer netPlayer = null;
		if (this.netPlayerIndexToAddFriend >= 0 && this.netPlayerIndexToAddFriend < NetworkSystem.Instance.AllNetPlayers.Length)
		{
			netPlayer = NetworkSystem.Instance.AllNetPlayers[this.netPlayerIndexToAddFriend];
		}
		this.AddFriend(netPlayer);
	}

	// Token: 0x06005011 RID: 20497 RVA: 0x001A9EE9 File Offset: 0x001A80E9
	private void TestAddFriendCompleteCallback(NetPlayer player, bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestAddFriendCompleteCallback returned with success = true");
			return;
		}
		Debug.Log("FriendBackend: TestAddFriendCompleteCallback returned with success = false");
	}

	// Token: 0x06005012 RID: 20498 RVA: 0x001A9F04 File Offset: 0x001A8104
	private void TestRemoveFriend()
	{
		this.OnRemoveFriendComplete -= this.TestRemoveFriendCompleteCallback;
		this.OnRemoveFriendComplete += this.TestRemoveFriendCompleteCallback;
		FriendBackendController.Friend friend = null;
		if (this.friendListIndexToRemoveFriend >= 0 && this.friendListIndexToRemoveFriend < this.FriendsList.Count)
		{
			friend = this.FriendsList[this.friendListIndexToRemoveFriend];
		}
		this.RemoveFriend(friend);
	}

	// Token: 0x06005013 RID: 20499 RVA: 0x001A9F6C File Offset: 0x001A816C
	private void TestRemoveFriendCompleteCallback(FriendBackendController.Friend friend, bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestRemoveFriendCompleteCallback returned with success = true");
			return;
		}
		Debug.Log("FriendBackend: TestRemoveFriendCompleteCallback returned with success = false");
	}

	// Token: 0x06005014 RID: 20500 RVA: 0x001A9F86 File Offset: 0x001A8186
	private void TestGetFriends()
	{
		this.OnGetFriendsComplete -= this.TestGetFriendsCompleteCallback;
		this.OnGetFriendsComplete += this.TestGetFriendsCompleteCallback;
		this.GetFriends();
	}

	// Token: 0x06005015 RID: 20501 RVA: 0x001A9FB4 File Offset: 0x001A81B4
	private void TestGetFriendsCompleteCallback(bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestGetFriendsCompleteCallback returned with success = true");
			if (this.FriendsList != null)
			{
				string text = string.Format("Friend Count: {0} Friends: \n", this.FriendsList.Count);
				for (int i = 0; i < this.FriendsList.Count; i++)
				{
					if (this.FriendsList[i] != null && this.FriendsList[i].Presence != null)
					{
						text = string.Concat(new string[]
						{
							text,
							this.FriendsList[i].Presence.UserName,
							", ",
							this.FriendsList[i].Presence.FriendLinkId,
							", ",
							this.FriendsList[i].Presence.RoomId,
							", ",
							this.FriendsList[i].Presence.Region,
							", ",
							this.FriendsList[i].Presence.Zone,
							"\n"
						});
					}
					else
					{
						text += "null friend\n";
					}
				}
				Debug.Log(text);
				return;
			}
		}
		else
		{
			Debug.Log("FriendBackend: TestGetFriendsCompleteCallback returned with success = false");
		}
	}

	// Token: 0x06005016 RID: 20502 RVA: 0x001AA111 File Offset: 0x001A8311
	private void TestSetPrivacyState()
	{
		this.OnSetPrivacyStateComplete -= this.TestSetPrivacyStateCompleteCallback;
		this.OnSetPrivacyStateComplete += this.TestSetPrivacyStateCompleteCallback;
		this.SetPrivacyState(this.privacyStateToSet);
	}

	// Token: 0x06005017 RID: 20503 RVA: 0x001AA144 File Offset: 0x001A8344
	private void TestSetPrivacyStateCompleteCallback(bool success)
	{
		if (success)
		{
			Debug.Log(string.Format("SetPrivacyState Success: Status: {0} Error: {1}", this.lastPrivacyStateResponse.StatusCode, this.lastPrivacyStateResponse.Error));
			return;
		}
		Debug.Log(string.Format("SetPrivacyState Failed: Status: {0} Error: {1}", this.lastPrivacyStateResponse.StatusCode, this.lastPrivacyStateResponse.Error));
	}

	// Token: 0x04006240 RID: 25152
	[OnEnterPlay_SetNull]
	public static volatile FriendBackendController Instance;

	// Token: 0x04006245 RID: 25157
	private int maxRetriesOnFail = 3;

	// Token: 0x04006246 RID: 25158
	private int getFriendsRetryCount;

	// Token: 0x04006247 RID: 25159
	private int setPrivacyStateRetryCount;

	// Token: 0x04006248 RID: 25160
	private int addFriendRetryCount;

	// Token: 0x04006249 RID: 25161
	private int removeFriendRetryCount;

	// Token: 0x0400624A RID: 25162
	private bool getFriendsInProgress;

	// Token: 0x0400624B RID: 25163
	private FriendBackendController.GetFriendsResponse lastGetFriendsResponse;

	// Token: 0x0400624C RID: 25164
	private List<FriendBackendController.Friend> lastFriendsList = new List<FriendBackendController.Friend>();

	// Token: 0x0400624D RID: 25165
	private bool setPrivacyStateInProgress;

	// Token: 0x0400624E RID: 25166
	private FriendBackendController.PrivacyState setPrivacyStateState;

	// Token: 0x0400624F RID: 25167
	private FriendBackendController.SetPrivacyStateResponse lastPrivacyStateResponse;

	// Token: 0x04006250 RID: 25168
	private Queue<FriendBackendController.PrivacyState> setPrivacyStateQueue = new Queue<FriendBackendController.PrivacyState>();

	// Token: 0x04006251 RID: 25169
	private FriendBackendController.PrivacyState lastPrivacyState;

	// Token: 0x04006252 RID: 25170
	private bool addFriendInProgress;

	// Token: 0x04006253 RID: 25171
	private int addFriendTargetIdHash;

	// Token: 0x04006254 RID: 25172
	private NetPlayer addFriendTargetPlayer;

	// Token: 0x04006255 RID: 25173
	private Queue<ValueTuple<int, NetPlayer>> addFriendRequestQueue = new Queue<ValueTuple<int, NetPlayer>>();

	// Token: 0x04006256 RID: 25174
	private bool removeFriendInProgress;

	// Token: 0x04006257 RID: 25175
	private int removeFriendTargetIdHash;

	// Token: 0x04006258 RID: 25176
	private FriendBackendController.Friend removeFriendTarget;

	// Token: 0x04006259 RID: 25177
	private Queue<ValueTuple<int, FriendBackendController.Friend>> removeFriendRequestQueue = new Queue<ValueTuple<int, FriendBackendController.Friend>>();

	// Token: 0x0400625A RID: 25178
	[SerializeField]
	private int netPlayerIndexToAddFriend;

	// Token: 0x0400625B RID: 25179
	[SerializeField]
	private int friendListIndexToRemoveFriend;

	// Token: 0x0400625C RID: 25180
	[SerializeField]
	private FriendBackendController.PrivacyState privacyStateToSet;

	// Token: 0x02000CAA RID: 3242
	public class Friend
	{
		// Token: 0x17000772 RID: 1906
		// (get) Token: 0x06005019 RID: 20505 RVA: 0x001AA1E4 File Offset: 0x001A83E4
		// (set) Token: 0x0600501A RID: 20506 RVA: 0x001AA1EC File Offset: 0x001A83EC
		public FriendBackendController.FriendPresence Presence { get; set; }

		// Token: 0x17000773 RID: 1907
		// (get) Token: 0x0600501B RID: 20507 RVA: 0x001AA1F5 File Offset: 0x001A83F5
		// (set) Token: 0x0600501C RID: 20508 RVA: 0x001AA1FD File Offset: 0x001A83FD
		public DateTime Created { get; set; }
	}

	// Token: 0x02000CAB RID: 3243
	public class FriendPresence
	{
		// Token: 0x17000774 RID: 1908
		// (get) Token: 0x0600501E RID: 20510 RVA: 0x001AA206 File Offset: 0x001A8406
		// (set) Token: 0x0600501F RID: 20511 RVA: 0x001AA20E File Offset: 0x001A840E
		public string FriendLinkId { get; set; }

		// Token: 0x17000775 RID: 1909
		// (get) Token: 0x06005020 RID: 20512 RVA: 0x001AA217 File Offset: 0x001A8417
		// (set) Token: 0x06005021 RID: 20513 RVA: 0x001AA21F File Offset: 0x001A841F
		public string UserName { get; set; }

		// Token: 0x17000776 RID: 1910
		// (get) Token: 0x06005022 RID: 20514 RVA: 0x001AA228 File Offset: 0x001A8428
		// (set) Token: 0x06005023 RID: 20515 RVA: 0x001AA230 File Offset: 0x001A8430
		public string RoomId { get; set; }

		// Token: 0x17000777 RID: 1911
		// (get) Token: 0x06005024 RID: 20516 RVA: 0x001AA239 File Offset: 0x001A8439
		// (set) Token: 0x06005025 RID: 20517 RVA: 0x001AA241 File Offset: 0x001A8441
		public string Zone { get; set; }

		// Token: 0x17000778 RID: 1912
		// (get) Token: 0x06005026 RID: 20518 RVA: 0x001AA24A File Offset: 0x001A844A
		// (set) Token: 0x06005027 RID: 20519 RVA: 0x001AA252 File Offset: 0x001A8452
		public string Region { get; set; }

		// Token: 0x17000779 RID: 1913
		// (get) Token: 0x06005028 RID: 20520 RVA: 0x001AA25B File Offset: 0x001A845B
		// (set) Token: 0x06005029 RID: 20521 RVA: 0x001AA263 File Offset: 0x001A8463
		public bool? IsPublic { get; set; }
	}

	// Token: 0x02000CAC RID: 3244
	public class FriendLink
	{
		// Token: 0x1700077A RID: 1914
		// (get) Token: 0x0600502B RID: 20523 RVA: 0x001AA26C File Offset: 0x001A846C
		// (set) Token: 0x0600502C RID: 20524 RVA: 0x001AA274 File Offset: 0x001A8474
		public string my_playfab_id { get; set; }

		// Token: 0x1700077B RID: 1915
		// (get) Token: 0x0600502D RID: 20525 RVA: 0x001AA27D File Offset: 0x001A847D
		// (set) Token: 0x0600502E RID: 20526 RVA: 0x001AA285 File Offset: 0x001A8485
		public string my_mothership_id { get; set; }

		// Token: 0x1700077C RID: 1916
		// (get) Token: 0x0600502F RID: 20527 RVA: 0x001AA28E File Offset: 0x001A848E
		// (set) Token: 0x06005030 RID: 20528 RVA: 0x001AA296 File Offset: 0x001A8496
		public string my_friendlink_id { get; set; }

		// Token: 0x1700077D RID: 1917
		// (get) Token: 0x06005031 RID: 20529 RVA: 0x001AA29F File Offset: 0x001A849F
		// (set) Token: 0x06005032 RID: 20530 RVA: 0x001AA2A7 File Offset: 0x001A84A7
		public string friend_playfab_id { get; set; }

		// Token: 0x1700077E RID: 1918
		// (get) Token: 0x06005033 RID: 20531 RVA: 0x001AA2B0 File Offset: 0x001A84B0
		// (set) Token: 0x06005034 RID: 20532 RVA: 0x001AA2B8 File Offset: 0x001A84B8
		public string friend_mothership_id { get; set; }

		// Token: 0x1700077F RID: 1919
		// (get) Token: 0x06005035 RID: 20533 RVA: 0x001AA2C1 File Offset: 0x001A84C1
		// (set) Token: 0x06005036 RID: 20534 RVA: 0x001AA2C9 File Offset: 0x001A84C9
		public string friend_friendlink_id { get; set; }

		// Token: 0x17000780 RID: 1920
		// (get) Token: 0x06005037 RID: 20535 RVA: 0x001AA2D2 File Offset: 0x001A84D2
		// (set) Token: 0x06005038 RID: 20536 RVA: 0x001AA2DA File Offset: 0x001A84DA
		public DateTime created { get; set; }
	}

	// Token: 0x02000CAD RID: 3245
	[NullableContext(2)]
	[Nullable(0)]
	public class FriendIdResponse
	{
		// Token: 0x17000781 RID: 1921
		// (get) Token: 0x0600503A RID: 20538 RVA: 0x001AA2E3 File Offset: 0x001A84E3
		// (set) Token: 0x0600503B RID: 20539 RVA: 0x001AA2EB File Offset: 0x001A84EB
		public string PlayFabId { get; set; }

		// Token: 0x17000782 RID: 1922
		// (get) Token: 0x0600503C RID: 20540 RVA: 0x001AA2F4 File Offset: 0x001A84F4
		// (set) Token: 0x0600503D RID: 20541 RVA: 0x001AA2FC File Offset: 0x001A84FC
		public string MothershipId { get; set; } = "";
	}

	// Token: 0x02000CAE RID: 3246
	public class FriendRequestRequest
	{
		// Token: 0x17000783 RID: 1923
		// (get) Token: 0x0600503F RID: 20543 RVA: 0x001AA318 File Offset: 0x001A8518
		// (set) Token: 0x06005040 RID: 20544 RVA: 0x001AA320 File Offset: 0x001A8520
		public string PlayFabId { get; set; }

		// Token: 0x17000784 RID: 1924
		// (get) Token: 0x06005041 RID: 20545 RVA: 0x001AA329 File Offset: 0x001A8529
		// (set) Token: 0x06005042 RID: 20546 RVA: 0x001AA331 File Offset: 0x001A8531
		public string MothershipId { get; set; } = "";

		// Token: 0x17000785 RID: 1925
		// (get) Token: 0x06005043 RID: 20547 RVA: 0x001AA33A File Offset: 0x001A853A
		// (set) Token: 0x06005044 RID: 20548 RVA: 0x001AA342 File Offset: 0x001A8542
		public string PlayFabTicket { get; set; }

		// Token: 0x17000786 RID: 1926
		// (get) Token: 0x06005045 RID: 20549 RVA: 0x001AA34B File Offset: 0x001A854B
		// (set) Token: 0x06005046 RID: 20550 RVA: 0x001AA353 File Offset: 0x001A8553
		public string MothershipToken { get; set; }

		// Token: 0x17000787 RID: 1927
		// (get) Token: 0x06005047 RID: 20551 RVA: 0x001AA35C File Offset: 0x001A855C
		// (set) Token: 0x06005048 RID: 20552 RVA: 0x001AA364 File Offset: 0x001A8564
		public string MyFriendLinkId { get; set; }

		// Token: 0x17000788 RID: 1928
		// (get) Token: 0x06005049 RID: 20553 RVA: 0x001AA36D File Offset: 0x001A856D
		// (set) Token: 0x0600504A RID: 20554 RVA: 0x001AA375 File Offset: 0x001A8575
		public string FriendFriendLinkId { get; set; }
	}

	// Token: 0x02000CAF RID: 3247
	public class GetFriendsRequest
	{
		// Token: 0x17000789 RID: 1929
		// (get) Token: 0x0600504C RID: 20556 RVA: 0x001AA391 File Offset: 0x001A8591
		// (set) Token: 0x0600504D RID: 20557 RVA: 0x001AA399 File Offset: 0x001A8599
		public string PlayFabId { get; set; }

		// Token: 0x1700078A RID: 1930
		// (get) Token: 0x0600504E RID: 20558 RVA: 0x001AA3A2 File Offset: 0x001A85A2
		// (set) Token: 0x0600504F RID: 20559 RVA: 0x001AA3AA File Offset: 0x001A85AA
		public string MothershipId { get; set; } = "";

		// Token: 0x1700078B RID: 1931
		// (get) Token: 0x06005050 RID: 20560 RVA: 0x001AA3B3 File Offset: 0x001A85B3
		// (set) Token: 0x06005051 RID: 20561 RVA: 0x001AA3BB File Offset: 0x001A85BB
		public string MothershipToken { get; set; }

		// Token: 0x1700078C RID: 1932
		// (get) Token: 0x06005052 RID: 20562 RVA: 0x001AA3C4 File Offset: 0x001A85C4
		// (set) Token: 0x06005053 RID: 20563 RVA: 0x001AA3CC File Offset: 0x001A85CC
		public string PlayFabTicket { get; set; }
	}

	// Token: 0x02000CB0 RID: 3248
	public class GetFriendsResponse
	{
		// Token: 0x1700078D RID: 1933
		// (get) Token: 0x06005055 RID: 20565 RVA: 0x001AA3E8 File Offset: 0x001A85E8
		// (set) Token: 0x06005056 RID: 20566 RVA: 0x001AA3F0 File Offset: 0x001A85F0
		[CanBeNull]
		public FriendBackendController.GetFriendsResult Result { get; set; }

		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x06005057 RID: 20567 RVA: 0x001AA3F9 File Offset: 0x001A85F9
		// (set) Token: 0x06005058 RID: 20568 RVA: 0x001AA401 File Offset: 0x001A8601
		public int StatusCode { get; set; }

		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x06005059 RID: 20569 RVA: 0x001AA40A File Offset: 0x001A860A
		// (set) Token: 0x0600505A RID: 20570 RVA: 0x001AA412 File Offset: 0x001A8612
		[Nullable(2)]
		public string Error
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}
	}

	// Token: 0x02000CB1 RID: 3249
	public class GetFriendsResult
	{
		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x0600505C RID: 20572 RVA: 0x001AA41B File Offset: 0x001A861B
		// (set) Token: 0x0600505D RID: 20573 RVA: 0x001AA423 File Offset: 0x001A8623
		public List<FriendBackendController.Friend> Friends { get; set; }

		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x0600505E RID: 20574 RVA: 0x001AA42C File Offset: 0x001A862C
		// (set) Token: 0x0600505F RID: 20575 RVA: 0x001AA434 File Offset: 0x001A8634
		public FriendBackendController.PrivacyState MyPrivacyState { get; set; }
	}

	// Token: 0x02000CB2 RID: 3250
	public class SetPrivacyStateRequest
	{
		// Token: 0x17000792 RID: 1938
		// (get) Token: 0x06005061 RID: 20577 RVA: 0x001AA43D File Offset: 0x001A863D
		// (set) Token: 0x06005062 RID: 20578 RVA: 0x001AA445 File Offset: 0x001A8645
		public string PlayFabId { get; set; }

		// Token: 0x17000793 RID: 1939
		// (get) Token: 0x06005063 RID: 20579 RVA: 0x001AA44E File Offset: 0x001A864E
		// (set) Token: 0x06005064 RID: 20580 RVA: 0x001AA456 File Offset: 0x001A8656
		public string PlayFabTicket { get; set; }

		// Token: 0x17000794 RID: 1940
		// (get) Token: 0x06005065 RID: 20581 RVA: 0x001AA45F File Offset: 0x001A865F
		// (set) Token: 0x06005066 RID: 20582 RVA: 0x001AA467 File Offset: 0x001A8667
		public string PrivacyState { get; set; }
	}

	// Token: 0x02000CB3 RID: 3251
	[NullableContext(2)]
	[Nullable(0)]
	public class SetPrivacyStateResponse
	{
		// Token: 0x17000795 RID: 1941
		// (get) Token: 0x06005068 RID: 20584 RVA: 0x001AA470 File Offset: 0x001A8670
		// (set) Token: 0x06005069 RID: 20585 RVA: 0x001AA478 File Offset: 0x001A8678
		public int StatusCode { get; set; }

		// Token: 0x17000796 RID: 1942
		// (get) Token: 0x0600506A RID: 20586 RVA: 0x001AA481 File Offset: 0x001A8681
		// (set) Token: 0x0600506B RID: 20587 RVA: 0x001AA489 File Offset: 0x001A8689
		public string Error { get; set; }
	}

	// Token: 0x02000CB4 RID: 3252
	public class RemoveFriendRequest
	{
		// Token: 0x17000797 RID: 1943
		// (get) Token: 0x0600506D RID: 20589 RVA: 0x001AA492 File Offset: 0x001A8692
		// (set) Token: 0x0600506E RID: 20590 RVA: 0x001AA49A File Offset: 0x001A869A
		public string PlayFabId { get; set; }

		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x0600506F RID: 20591 RVA: 0x001AA4A3 File Offset: 0x001A86A3
		// (set) Token: 0x06005070 RID: 20592 RVA: 0x001AA4AB File Offset: 0x001A86AB
		public string MothershipId { get; set; } = "";

		// Token: 0x17000799 RID: 1945
		// (get) Token: 0x06005071 RID: 20593 RVA: 0x001AA4B4 File Offset: 0x001A86B4
		// (set) Token: 0x06005072 RID: 20594 RVA: 0x001AA4BC File Offset: 0x001A86BC
		public string PlayFabTicket { get; set; }

		// Token: 0x1700079A RID: 1946
		// (get) Token: 0x06005073 RID: 20595 RVA: 0x001AA4C5 File Offset: 0x001A86C5
		// (set) Token: 0x06005074 RID: 20596 RVA: 0x001AA4CD File Offset: 0x001A86CD
		public string MothershipToken { get; set; }

		// Token: 0x1700079B RID: 1947
		// (get) Token: 0x06005075 RID: 20597 RVA: 0x001AA4D6 File Offset: 0x001A86D6
		// (set) Token: 0x06005076 RID: 20598 RVA: 0x001AA4DE File Offset: 0x001A86DE
		public string MyFriendLinkId { get; set; }

		// Token: 0x1700079C RID: 1948
		// (get) Token: 0x06005077 RID: 20599 RVA: 0x001AA4E7 File Offset: 0x001A86E7
		// (set) Token: 0x06005078 RID: 20600 RVA: 0x001AA4EF File Offset: 0x001A86EF
		public string FriendFriendLinkId { get; set; }
	}

	// Token: 0x02000CB5 RID: 3253
	public enum PendingRequestStatus
	{
		// Token: 0x04006289 RID: 25225
		I_REQUESTED,
		// Token: 0x0400628A RID: 25226
		THEY_REQUESTED,
		// Token: 0x0400628B RID: 25227
		CONFIRMED,
		// Token: 0x0400628C RID: 25228
		NOT_FOUND
	}

	// Token: 0x02000CB6 RID: 3254
	public enum PrivacyState
	{
		// Token: 0x0400628E RID: 25230
		VISIBLE,
		// Token: 0x0400628F RID: 25231
		PUBLIC_ONLY,
		// Token: 0x04006290 RID: 25232
		HIDDEN
	}
}
