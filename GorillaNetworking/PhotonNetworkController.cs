using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking.ScheduledEvents;
using GorillaTagScripts;
using Photon.Pun;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02001107 RID: 4359
	public class PhotonNetworkController : MonoBehaviour
	{
		// Token: 0x17000A5E RID: 2654
		// (get) Token: 0x06006D34 RID: 27956 RVA: 0x002345FA File Offset: 0x002327FA
		// (set) Token: 0x06006D35 RID: 27957 RVA: 0x00234602 File Offset: 0x00232802
		public List<string> FriendIDList
		{
			get
			{
				return this.friendIDList;
			}
			set
			{
				this.friendIDList = value;
			}
		}

		// Token: 0x17000A5F RID: 2655
		// (get) Token: 0x06006D36 RID: 27958 RVA: 0x0023460B File Offset: 0x0023280B
		// (set) Token: 0x06006D37 RID: 27959 RVA: 0x00234613 File Offset: 0x00232813
		public string StartLevel
		{
			get
			{
				return this.startLevel;
			}
			set
			{
				this.startLevel = value;
			}
		}

		// Token: 0x17000A60 RID: 2656
		// (get) Token: 0x06006D38 RID: 27960 RVA: 0x0023461C File Offset: 0x0023281C
		// (set) Token: 0x06006D39 RID: 27961 RVA: 0x00234624 File Offset: 0x00232824
		public GTZone StartZone
		{
			get
			{
				return this.startZone;
			}
			set
			{
				this.startZone = value;
			}
		}

		// Token: 0x17000A61 RID: 2657
		// (get) Token: 0x06006D3A RID: 27962 RVA: 0x0023462D File Offset: 0x0023282D
		public GTZone CurrentRoomZone
		{
			get
			{
				if (!(this.currentJoinTrigger != null))
				{
					return GTZone.none;
				}
				return this.currentJoinTrigger.zone;
			}
		}

		// Token: 0x17000A62 RID: 2658
		// (get) Token: 0x06006D3B RID: 27963 RVA: 0x0023464B File Offset: 0x0023284B
		// (set) Token: 0x06006D3C RID: 27964 RVA: 0x00234653 File Offset: 0x00232853
		public GorillaGeoHideShowTrigger StartGeoTrigger
		{
			get
			{
				return this.startGeoTrigger;
			}
			set
			{
				this.startGeoTrigger = value;
			}
		}

		// Token: 0x06006D3D RID: 27965 RVA: 0x0023465C File Offset: 0x0023285C
		public void Awake()
		{
			if (PhotonNetworkController.Instance == null)
			{
				PhotonNetworkController.Instance = this;
			}
			else if (PhotonNetworkController.Instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			this.updatedName = false;
			this.playersInRegion = new int[this.serverRegions.Length];
			this.pingInRegion = new int[this.serverRegions.Length];
		}

		// Token: 0x06006D3E RID: 27966 RVA: 0x002346CC File Offset: 0x002328CC
		public void Start()
		{
			base.StartCoroutine(this.DisableOnStart());
			NetworkSystem.Instance.OnJoinedRoomEvent += this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnected;
			PhotonNetwork.NetworkingClient.LoadBalancingPeer.ReuseEventInstance = true;
		}

		// Token: 0x06006D3F RID: 27967 RVA: 0x00234738 File Offset: 0x00232938
		private IEnumerator DisableOnStart()
		{
			ZoneManagement.SetActiveZone(this.StartZone);
			yield break;
		}

		// Token: 0x06006D40 RID: 27968 RVA: 0x00234748 File Offset: 0x00232948
		public void FixedUpdate()
		{
			this.headRightHandDistance = (GTPlayer.Instance.headCollider.transform.position - GTPlayer.Instance.GetControllerTransform(false).position).magnitude;
			this.headLeftHandDistance = (GTPlayer.Instance.headCollider.transform.position - GTPlayer.Instance.GetControllerTransform(true).position).magnitude;
			this.headQuat = GTPlayer.Instance.headCollider.transform.rotation;
			if (!this.disableAFKKick && Quaternion.Angle(this.headQuat, this.lastHeadQuat) <= 0.01f && Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) < 0.001f && Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) < 0.001f && this.pauseTime + this.disconnectTime < Time.realtimeSinceStartup)
			{
				this.pauseTime = Time.realtimeSinceStartup;
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			else if (Quaternion.Angle(this.headQuat, this.lastHeadQuat) > 0.01f || Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) >= 0.001f || Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) >= 0.001f)
			{
				this.pauseTime = Time.realtimeSinceStartup;
			}
			this.lastHeadRightHandDistance = this.headRightHandDistance;
			this.lastHeadLeftHandDistance = this.headLeftHandDistance;
			this.lastHeadQuat = this.headQuat;
			if (this.deferredJoin && Time.realtimeSinceStartup >= this.partyJoinDeferredUntilTimestamp)
			{
				if ((this.partyJoinDeferredUntilTimestamp != 0f || NetworkSystem.Instance.netState == NetSystemState.Idle) && this.currentJoinTrigger != null)
				{
					this.deferredJoin = false;
					this.partyJoinDeferredUntilTimestamp = 0f;
					if (!(this.currentJoinTrigger == this.privateTrigger))
					{
						this.AttemptToJoinPublicRoom(this.currentJoinTrigger, this.currentJoinType, null, false);
						return;
					}
					if (this.customRoomID == this.roomToJoin || this.customRoomID == this.autoJoinRoom || this.customRoomID == this.LastRoomToJoin)
					{
						this.AttemptToAutoJoinSpecificRoom(this.customRoomID, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
						return;
					}
					this.AttemptToJoinSpecificRoom(this.customRoomID, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
					return;
				}
				else if (NetworkSystem.Instance.netState != NetSystemState.PingRecon && NetworkSystem.Instance.netState != NetSystemState.Initialization && NetworkSystem.Instance.netState != NetSystemState.Disconnecting)
				{
					this.deferredJoin = false;
					this.partyJoinDeferredUntilTimestamp = 0f;
				}
			}
		}

		// Token: 0x06006D41 RID: 27969 RVA: 0x00234A02 File Offset: 0x00232C02
		public void DeferJoining(float duration)
		{
			this.partyJoinDeferredUntilTimestamp = Mathf.Max(this.partyJoinDeferredUntilTimestamp, Time.realtimeSinceStartup + duration);
		}

		// Token: 0x06006D42 RID: 27970 RVA: 0x00234A1C File Offset: 0x00232C1C
		public void ClearDeferredJoin()
		{
			this.partyJoinDeferredUntilTimestamp = 0f;
			this.deferredJoin = false;
		}

		// Token: 0x06006D43 RID: 27971 RVA: 0x00234A30 File Offset: 0x00232C30
		public void AttemptToJoinPublicRoom(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType = JoinType.Solo, List<ValueTuple<string, string>> additionalCustomProperties = null, bool filterSubscribed = false)
		{
			this.AttemptToJoinPublicRoomAsync(triggeredTrigger, roomJoinType, additionalCustomProperties, filterSubscribed);
		}

		// Token: 0x06006D44 RID: 27972 RVA: 0x00234A40 File Offset: 0x00232C40
		private async void AttemptToJoinPublicRoomAsync(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType, List<ValueTuple<string, string>> additionalCustomProperties, bool filterSubscribed)
		{
			if ((!KIDManager.KidEnabledAndReady || KIDManager.CheckFeatureOptIn(EKIDFeatures.Multiplayer, null).Item2) && base.enabled)
			{
				if (NetworkSystem.Instance.netState != NetSystemState.Connecting && NetworkSystem.Instance.netState != NetSystemState.Disconnecting)
				{
					if (NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon || Time.realtimeSinceStartup < this.partyJoinDeferredUntilTimestamp)
					{
						this.currentJoinTrigger = triggeredTrigger;
						this.currentJoinType = roomJoinType;
						this.deferredJoin = true;
					}
					else
					{
						this.deferredJoin = false;
						string desiredGameMode = triggeredTrigger.GetFullDesiredGameModeString();
						if (NetworkSystem.Instance.InRoom)
						{
							if (NetworkSystem.Instance.SessionIsPrivate)
							{
								if (roomJoinType != JoinType.JoinWithNearby && roomJoinType != JoinType.ForceJoinWithParty)
								{
									return;
								}
							}
							else if (!filterSubscribed || (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.MaxPlayers > 10))
							{
								if (NetworkSystem.Instance.GameModeString.StartsWith(desiredGameMode))
								{
									return;
								}
								if (triggeredTrigger.SameZoneAsOverride())
								{
									return;
								}
							}
						}
						if (roomJoinType == JoinType.JoinWithParty || roomJoinType == JoinType.ForceJoinWithParty)
						{
							await this.SendPartyFollowCommands();
						}
						this.currentJoinTrigger = triggeredTrigger;
						this.currentJoinType = roomJoinType;
						if (PlayFabClientAPI.IsClientLoggedIn())
						{
							this.playFabAuthenticator.SetDisplayName(NetworkSystem.Instance.GetMyNickName());
						}
						RoomConfig roomConfig = RoomConfig.AnyPublicConfig();
						if (this.currentJoinType == JoinType.JoinWithNearby || this.currentJoinType == JoinType.JoinWithElevator)
						{
							roomConfig.SetFriendIDs(this.FriendIDList);
						}
						else if (this.currentJoinType == JoinType.JoinWithParty || this.currentJoinType == JoinType.ForceJoinWithParty)
						{
							roomConfig.SetFriendIDs(FriendshipGroupDetection.Instance.PartyMemberIDs.ToList<string>());
						}
						bool flag = filterSubscribed && SubscriptionManager.IsLocalSubscribed();
						Hashtable hashtable = new Hashtable
						{
							{ "gameMode", desiredGameMode },
							{ "platform", this.platformTag },
							{
								"queueName",
								GorillaComputer.instance.currentQueue
							},
							{
								"language",
								LocalisationManager.CurrentLanguage.ToString()
							},
							{
								"fan_club",
								flag ? "true" : "false"
							}
						};
						if (additionalCustomProperties != null)
						{
							foreach (ValueTuple<string, string> valueTuple in additionalCustomProperties)
							{
								hashtable.Add(valueTuple.Item1, valueTuple.Item2);
							}
						}
						Hashtable hashtable2;
						ScheduledEventMatchmaking.ApplyScheduledEventStateToHashes(hashtable, out hashtable2);
						roomConfig.CustomProps = hashtable;
						roomConfig.SearchFilter = hashtable2;
						roomConfig.MaxPlayers = this.currentJoinTrigger.GetRoomSize(flag);
						Debug.Log(string.Format("AttemptToJoinPublicRoom: MaxPlayers: {0}   FanClub: {1}", roomConfig.MaxPlayers, flag));
						await NetworkSystem.Instance.ConnectToRoom(null, roomConfig, -1);
					}
				}
			}
		}

		// Token: 0x06006D45 RID: 27973 RVA: 0x00234A98 File Offset: 0x00232C98
		public void AttemptToJoinRankedPublicRoom(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType = JoinType.Solo)
		{
			string text = RankedProgressionManager.Instance.GetRankedMatchmakingTier().ToString();
			string text2 = "PC";
			this.AttemptToJoinRankedPublicRoomAsync(triggeredTrigger, text, text2, roomJoinType);
		}

		// Token: 0x06006D46 RID: 27974 RVA: 0x00234AD4 File Offset: 0x00232CD4
		private async void AttemptToJoinRankedPublicRoomAsync(GorillaNetworkJoinTrigger triggeredTrigger, string mmrTier, string platform, JoinType roomJoinType)
		{
			if ((!KIDManager.KidEnabledAndReady || KIDManager.CheckFeatureOptIn(EKIDFeatures.Multiplayer, null).Item2) && base.enabled)
			{
				if (NetworkSystem.Instance.netState != NetSystemState.Connecting && NetworkSystem.Instance.netState != NetSystemState.Disconnecting)
				{
					if (NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon || Time.realtimeSinceStartup < this.partyJoinDeferredUntilTimestamp)
					{
						this.currentJoinTrigger = triggeredTrigger;
						this.currentJoinType = roomJoinType;
						this.deferredJoin = true;
					}
					else
					{
						this.deferredJoin = false;
						string fullDesiredGameModeString = triggeredTrigger.GetFullDesiredGameModeString();
						if (!NetworkSystem.Instance.InRoom)
						{
							this.currentJoinTrigger = triggeredTrigger;
							this.currentJoinType = roomJoinType;
							if (PlayFabClientAPI.IsClientLoggedIn())
							{
								this.playFabAuthenticator.SetDisplayName(NetworkSystem.Instance.GetMyNickName());
							}
							RoomConfig roomConfig = RoomConfig.AnyPublicConfig();
							Hashtable hashtable = new Hashtable
							{
								{ "gameMode", fullDesiredGameModeString },
								{ "mmrTier", mmrTier },
								{ "platform", platform }
							};
							Hashtable hashtable2;
							ScheduledEventMatchmaking.ApplyScheduledEventStateToHashes(hashtable, out hashtable2);
							roomConfig.CustomProps = hashtable;
							roomConfig.SearchFilter = hashtable2;
							roomConfig.MaxPlayers = this.currentJoinTrigger.GetRoomSize(false);
							await NetworkSystem.Instance.ConnectToRoom(null, roomConfig, -1);
						}
					}
				}
			}
		}

		// Token: 0x06006D47 RID: 27975 RVA: 0x00234B2C File Offset: 0x00232D2C
		private async Task SendPartyFollowCommands()
		{
			PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			RoomSystem.SendPartyFollowCommand(PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr);
			PhotonNetwork.SendAllOutgoingCommands();
			await Task.Delay(200);
		}

		// Token: 0x06006D48 RID: 27976 RVA: 0x00234B67 File Offset: 0x00232D67
		private void AttemptToAutoJoinRoomCallback(NetJoinResult obj)
		{
			this.LastRoomToJoin = this.roomToJoin;
			switch (obj)
			{
			case NetJoinResult.Success:
				return;
			case NetJoinResult.FallbackCreated:
				return;
			case NetJoinResult.Failed_Full:
				return;
			case NetJoinResult.AlreadyInRoom:
				return;
			default:
				return;
			}
		}

		// Token: 0x06006D49 RID: 27977 RVA: 0x00234B8F File Offset: 0x00232D8F
		public void AttemptToAutoJoinSpecificRoom(string roomID, JoinType roomJoinType)
		{
			this.roomToJoin = roomID;
			this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType, new Action<NetJoinResult>(this.AttemptToAutoJoinRoomCallback));
		}

		// Token: 0x06006D4A RID: 27978 RVA: 0x00234BAD File Offset: 0x00232DAD
		public void AttemptToJoinSpecificRoom(string roomID, JoinType roomJoinType)
		{
			this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType, null);
		}

		// Token: 0x06006D4B RID: 27979 RVA: 0x00234BB9 File Offset: 0x00232DB9
		public void AttemptToJoinSpecificRoomWithCallback(string roomID, JoinType roomJoinType, Action<NetJoinResult> callback)
		{
			this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType, callback);
		}

		// Token: 0x06006D4C RID: 27980 RVA: 0x00234BC8 File Offset: 0x00232DC8
		public async Task AttemptToJoinSpecificRoomAsync(string roomID, JoinType roomJoinType, Action<NetJoinResult> callback)
		{
			TaskAwaiter<bool> taskAwaiter = KIDManager.UseKID().GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult() || KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer))
			{
				if (NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon)
				{
					this.deferredJoin = true;
					this.customRoomID = roomID;
					this.currentJoinType = roomJoinType;
					this.currentJoinTrigger = this.privateTrigger;
				}
				else if (NetworkSystem.Instance.netState == NetSystemState.Idle || NetworkSystem.Instance.netState == NetSystemState.InGame)
				{
					this.customRoomID = roomID;
					this.currentJoinType = roomJoinType;
					this.currentJoinTrigger = this.privateTrigger;
					this.deferredJoin = false;
					if (this.currentJoinType == JoinType.JoinWithParty || this.currentJoinType == JoinType.ForceJoinWithParty)
					{
						await this.SendPartyFollowCommands();
					}
					string fullDesiredGameModeString = this.currentJoinTrigger.GetFullDesiredGameModeString();
					Hashtable hashtable = new Hashtable
					{
						{ "gameMode", fullDesiredGameModeString },
						{ "platform", this.platformTag },
						{
							"queueName",
							GorillaComputer.instance.currentQueue
						}
					};
					RoomConfig roomConfig = new RoomConfig();
					roomConfig.createIfMissing = true;
					roomConfig.isJoinable = true;
					roomConfig.isPublic = false;
					if (roomJoinType == JoinType.FriendStationPublic)
					{
						roomConfig.isPublic = true;
					}
					byte roomSizeForCreate = RoomSystem.GetRoomSizeForCreate(this.currentJoinTrigger.zone, Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true), roomConfig.isPublic, SubscriptionManager.IsLocalSubscribed());
					roomConfig.MaxPlayers = roomSizeForCreate;
					Debug.Log(string.Format("[AttemptToJoinSpecificRoomAsync] Room MaxPlayers = {0}", roomConfig.MaxPlayers));
					roomConfig.CustomProps = hashtable;
					if (PlayFabClientAPI.IsClientLoggedIn())
					{
						this.playFabAuthenticator.SetDisplayName(NetworkSystem.Instance.GetMyNickName());
					}
					Task<NetJoinResult> connectToRoomTask = NetworkSystem.Instance.ConnectToRoom(roomID, roomConfig, -1);
					if (callback != null)
					{
						await connectToRoomTask;
						Debug.Log("AttemptToJoinSpecificRoomAsync ConnectToRoom Result: " + connectToRoomTask.Result.ToString());
						callback(connectToRoomTask.Result);
					}
				}
			}
		}

		// Token: 0x06006D4D RID: 27981 RVA: 0x00234C24 File Offset: 0x00232E24
		private void DisconnectCleanup()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (GorillaParent.instance != null)
			{
				GorillaScoreboardSpawner[] componentsInChildren = GorillaParent.instance.GetComponentsInChildren<GorillaScoreboardSpawner>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].OnLeftRoom();
				}
			}
			this.attemptingToConnect = true;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.offlineVRRig)
			{
				if (skinnedMeshRenderer != null)
				{
					skinnedMeshRenderer.enabled = true;
				}
			}
			if (GorillaComputer.instance != null && !ApplicationQuittingState.IsQuitting)
			{
				this.UpdateTriggerScreens();
			}
			GTPlayer.Instance.maxJumpSpeed = 6.5f;
			GTPlayer.Instance.jumpMultiplier = 1.1f;
			MonkeAgent.instance.currentMasterClient = null;
			GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(false);
			this.initialGameMode = "";
		}

		// Token: 0x06006D4E RID: 27982 RVA: 0x00234D04 File Offset: 0x00232F04
		public void OnJoinedRoom()
		{
			if (NetworkSystem.Instance.GameModeString.IsNullOrEmpty())
			{
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			this.initialGameMode = NetworkSystem.Instance.GameModeString;
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				this.currentJoinTrigger = this.privateTrigger;
				PhotonNetworkController.Instance.UpdateTriggerScreens();
			}
			else if (this.currentJoinType != JoinType.FollowingParty)
			{
				bool flag = false;
				for (int i = 0; i < GorillaComputer.instance.allowedMapsToJoin.Length; i++)
				{
					if (NetworkSystem.Instance.GameModeString.StartsWith(GorillaComputer.instance.allowedMapsToJoin[i]))
					{
						flag = true;
						break;
					}
				}
				if (flag && GorillaComputer.instance.friendJoinCollider != null && !GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId))
				{
					GorillaNetworkJoinTrigger joinTriggerFromFullGameModeString = GorillaComputer.instance.GetJoinTriggerFromFullGameModeString(NetworkSystem.Instance.GameModeString);
					if (joinTriggerFromFullGameModeString.groupJoinRequiredZonesAB != default(GroupJoinZoneAB) && !joinTriggerFromFullGameModeString.groupJoinRequiredZonesAB.HasAnyFlag(VRRig.LocalRig.zoneEntity.currentNode.groupZoneAB))
					{
						Debug.Log(string.Format("NOT ALLOWED IN ROOM: Joined {0} room but physically in {1} zone", this.ParseZoneFromGameMode(NetworkSystem.Instance.GameModeString), VRRig.LocalRig.zoneEntity.currentNode.groupZoneAB));
						PersistLog.Log(string.Format("NOT ALLOWED IN ROOM - FAILED JOIN: [PlayerZone:{0} TriggerZone:{1}  Trigger:{2}]", VRRig.LocalRig.zoneEntity.currentNode.groupZoneAB, joinTriggerFromFullGameModeString.groupJoinRequiredZonesAB, joinTriggerFromFullGameModeString.name));
						flag = false;
					}
				}
				if (!flag)
				{
					GorillaComputer.instance.roomNotAllowed = true;
					NetworkSystem.Instance.ReturnToSinglePlayer();
					return;
				}
			}
			NetworkSystem.Instance.SetMyTutorialComplete();
			VRRigCache.Instance.InstantiateNetworkObject();
			if (NetworkSystem.Instance.IsMasterClient)
			{
				global::GorillaGameModes.GameMode.LoadGameModeFromProperty(this.initialGameMode);
			}
			GorillaComputer.instance.roomFull = false;
			GorillaComputer.instance.roomNotAllowed = false;
			if (this.currentJoinType == JoinType.JoinWithParty || this.currentJoinType == JoinType.JoinWithNearby || this.currentJoinType == JoinType.ForceJoinWithParty || this.currentJoinType == JoinType.JoinWithElevator)
			{
				this.keyToFollow = NetworkSystem.Instance.LocalPlayer.UserId + this.keyStr;
				NetworkSystem.Instance.BroadcastMyRoom(true, this.keyToFollow, this.shuffler);
			}
			MonkeAgent.instance.currentMasterClient = null;
			this.UpdateCurrentJoinTrigger();
			this.UpdateTriggerScreens();
			NetworkSystem.Instance.MultiplayerStarted();
		}

		// Token: 0x06006D4F RID: 27983 RVA: 0x00234FA3 File Offset: 0x002331A3
		public void RegisterJoinTrigger(GorillaNetworkJoinTrigger trigger)
		{
			this.allJoinTriggers.Add(trigger);
		}

		// Token: 0x06006D50 RID: 27984 RVA: 0x00234FB4 File Offset: 0x002331B4
		private void UpdateCurrentJoinTrigger()
		{
			GorillaNetworkJoinTrigger joinTriggerFromFullGameModeString = GorillaComputer.instance.GetJoinTriggerFromFullGameModeString(NetworkSystem.Instance.GameModeString);
			if (joinTriggerFromFullGameModeString != null)
			{
				this.currentJoinTrigger = joinTriggerFromFullGameModeString;
				return;
			}
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				if (this.currentJoinTrigger != this.privateTrigger)
				{
					Debug.LogError("IN a private game but private trigger isnt current");
					return;
				}
			}
			else
			{
				Debug.LogError("Not in private room and unabel tp update jointrigger.");
			}
		}

		// Token: 0x06006D51 RID: 27985 RVA: 0x00235020 File Offset: 0x00233220
		public void UpdateTriggerScreens()
		{
			foreach (GorillaNetworkJoinTrigger gorillaNetworkJoinTrigger in this.allJoinTriggers)
			{
				gorillaNetworkJoinTrigger.UpdateUI();
			}
		}

		// Token: 0x06006D52 RID: 27986 RVA: 0x00235070 File Offset: 0x00233270
		public void AttemptToFollowIntoPub(string userIDToFollow, int actorNumberToFollow, string newKeyStr, string shufflerStr, JoinType joinType)
		{
			this.friendToFollow = userIDToFollow;
			this.keyToFollow = userIDToFollow + newKeyStr;
			this.shuffler = shufflerStr;
			this.currentJoinType = joinType;
			this.ClearDeferredJoin();
			if (NetworkSystem.Instance.InRoom)
			{
				NetworkSystem.Instance.JoinFriendsRoom(this.friendToFollow, actorNumberToFollow, this.keyToFollow, this.shuffler);
			}
		}

		// Token: 0x06006D53 RID: 27987 RVA: 0x002350D1 File Offset: 0x002332D1
		public void OnDisconnected()
		{
			this.DisconnectCleanup();
		}

		// Token: 0x06006D54 RID: 27988 RVA: 0x002350D9 File Offset: 0x002332D9
		public void OnApplicationQuit()
		{
			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion != "dev";
			}
		}

		// Token: 0x06006D55 RID: 27989 RVA: 0x002350FC File Offset: 0x002332FC
		private string ReturnRoomName()
		{
			if (this.isPrivate)
			{
				return this.customRoomID;
			}
			return this.RandomRoomName();
		}

		// Token: 0x06006D56 RID: 27990 RVA: 0x00235114 File Offset: 0x00233314
		private string RandomRoomName()
		{
			string text = "";
			for (int i = 0; i < 4; i++)
			{
				text += "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Length), 1);
			}
			if (GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				return text;
			}
			return this.RandomRoomName();
		}

		// Token: 0x06006D57 RID: 27991 RVA: 0x0023516C File Offset: 0x0023336C
		private string GetRegionWithLowestPing()
		{
			int num = 10000;
			int num2 = 0;
			for (int i = 0; i < this.serverRegions.Length; i++)
			{
				Debug.Log("ping in region " + this.serverRegions[i] + " is " + this.pingInRegion[i].ToString());
				if (this.pingInRegion[i] < num && this.pingInRegion[i] > 0)
				{
					num = this.pingInRegion[i];
					num2 = i;
				}
			}
			return this.serverRegions[num2];
		}

		// Token: 0x06006D58 RID: 27992 RVA: 0x002351EC File Offset: 0x002333EC
		public int TotalUsers()
		{
			int num = 0;
			foreach (int num2 in this.playersInRegion)
			{
				num += num2;
			}
			return num;
		}

		// Token: 0x06006D59 RID: 27993 RVA: 0x0023521C File Offset: 0x0023341C
		public string CurrentState()
		{
			if (NetworkSystem.Instance == null)
			{
				Debug.Log("Null netsys!!!");
			}
			return NetworkSystem.Instance.netState.ToString();
		}

		// Token: 0x06006D5A RID: 27994 RVA: 0x00235258 File Offset: 0x00233458
		private void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				this.timeWhenApplicationPaused = new DateTime?(DateTime.Now);
				return;
			}
			if ((DateTime.Now - (this.timeWhenApplicationPaused ?? DateTime.Now)).TotalSeconds > (double)this.disconnectTime)
			{
				this.timeWhenApplicationPaused = null;
				NetworkSystem instance = NetworkSystem.Instance;
				if (instance != null)
				{
					instance.ReturnToSinglePlayer();
				}
			}
			if (NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom && NetworkSystem.Instance.netState == NetSystemState.InGame)
			{
				NetworkSystem instance2 = NetworkSystem.Instance;
				if (instance2 == null)
				{
					return;
				}
				instance2.ReturnToSinglePlayer();
			}
		}

		// Token: 0x06006D5B RID: 27995 RVA: 0x00235305 File Offset: 0x00233505
		private void OnApplicationFocus(bool focus)
		{
			if (!focus && NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom && NetworkSystem.Instance.netState == NetSystemState.InGame)
			{
				NetworkSystem instance = NetworkSystem.Instance;
				if (instance == null)
				{
					return;
				}
				instance.ReturnToSinglePlayer();
			}
		}

		// Token: 0x06006D5C RID: 27996 RVA: 0x00235340 File Offset: 0x00233540
		private GTZone ParseZoneFromGameMode(string gameMode)
		{
			if (string.IsNullOrEmpty(gameMode))
			{
				return GTZone.none;
			}
			foreach (object obj in Enum.GetValues(typeof(GTZone)))
			{
				GTZone gtzone = (GTZone)obj;
				if (gtzone != GTZone.none && gameMode.StartsWith(gtzone.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					return gtzone;
				}
			}
			return GTZone.none;
		}

		// Token: 0x04007D7A RID: 32122
		[OnEnterPlay_SetNull]
		public static volatile PhotonNetworkController Instance;

		// Token: 0x04007D7B RID: 32123
		public int incrementCounter;

		// Token: 0x04007D7C RID: 32124
		public PlayFabAuthenticator playFabAuthenticator;

		// Token: 0x04007D7D RID: 32125
		public string[] serverRegions;

		// Token: 0x04007D7E RID: 32126
		public bool isPrivate;

		// Token: 0x04007D7F RID: 32127
		public string customRoomID;

		// Token: 0x04007D80 RID: 32128
		public GameObject playerOffset;

		// Token: 0x04007D81 RID: 32129
		public SkinnedMeshRenderer[] offlineVRRig;

		// Token: 0x04007D82 RID: 32130
		public bool attemptingToConnect;

		// Token: 0x04007D83 RID: 32131
		private int currentRegionIndex;

		// Token: 0x04007D84 RID: 32132
		public string currentGameType;

		// Token: 0x04007D85 RID: 32133
		public bool roomCosmeticsInitialized;

		// Token: 0x04007D86 RID: 32134
		public GameObject photonVoiceObjectPrefab;

		// Token: 0x04007D87 RID: 32135
		public Dictionary<string, bool> playerCosmeticsLookup = new Dictionary<string, bool>();

		// Token: 0x04007D88 RID: 32136
		private float lastHeadRightHandDistance;

		// Token: 0x04007D89 RID: 32137
		private float lastHeadLeftHandDistance;

		// Token: 0x04007D8A RID: 32138
		private float pauseTime;

		// Token: 0x04007D8B RID: 32139
		private float disconnectTime = 120f;

		// Token: 0x04007D8C RID: 32140
		public bool disableAFKKick;

		// Token: 0x04007D8D RID: 32141
		private float headRightHandDistance;

		// Token: 0x04007D8E RID: 32142
		private float headLeftHandDistance;

		// Token: 0x04007D8F RID: 32143
		private Quaternion headQuat;

		// Token: 0x04007D90 RID: 32144
		private Quaternion lastHeadQuat;

		// Token: 0x04007D91 RID: 32145
		public GameObject[] disableOnStartup;

		// Token: 0x04007D92 RID: 32146
		public GameObject[] enableOnStartup;

		// Token: 0x04007D93 RID: 32147
		public bool updatedName;

		// Token: 0x04007D94 RID: 32148
		private int[] playersInRegion;

		// Token: 0x04007D95 RID: 32149
		private int[] pingInRegion;

		// Token: 0x04007D96 RID: 32150
		private List<string> friendIDList = new List<string>();

		// Token: 0x04007D97 RID: 32151
		private JoinType currentJoinType;

		// Token: 0x04007D98 RID: 32152
		private string friendToFollow;

		// Token: 0x04007D99 RID: 32153
		private string keyToFollow;

		// Token: 0x04007D9A RID: 32154
		public string shuffler;

		// Token: 0x04007D9B RID: 32155
		public string keyStr;

		// Token: 0x04007D9C RID: 32156
		private string platformTag = "OTHER";

		// Token: 0x04007D9D RID: 32157
		private string startLevel;

		// Token: 0x04007D9E RID: 32158
		[SerializeField]
		private GTZone startZone;

		// Token: 0x04007D9F RID: 32159
		private GorillaGeoHideShowTrigger startGeoTrigger;

		// Token: 0x04007DA0 RID: 32160
		public GorillaNetworkJoinTrigger privateTrigger;

		// Token: 0x04007DA1 RID: 32161
		internal string initialGameMode = "";

		// Token: 0x04007DA2 RID: 32162
		public GorillaNetworkJoinTrigger currentJoinTrigger;

		// Token: 0x04007DA3 RID: 32163
		public string autoJoinRoom;

		// Token: 0x04007DA4 RID: 32164
		public int autoJoinRoomCap = 18;

		// Token: 0x04007DA5 RID: 32165
		public string autoJoinGameMode;

		// Token: 0x04007DA6 RID: 32166
		private bool deferredJoin;

		// Token: 0x04007DA7 RID: 32167
		private float partyJoinDeferredUntilTimestamp;

		// Token: 0x04007DA8 RID: 32168
		private DateTime? timeWhenApplicationPaused;

		// Token: 0x04007DA9 RID: 32169
		[NetworkPrefab]
		[SerializeField]
		private NetworkObject testPlayerPrefab;

		// Token: 0x04007DAA RID: 32170
		private string roomToJoin = "";

		// Token: 0x04007DAB RID: 32171
		private int joinNextAttempt;

		// Token: 0x04007DAC RID: 32172
		private int maxNextAttempts = 10;

		// Token: 0x04007DAD RID: 32173
		private string LastRoomToJoin = "";

		// Token: 0x04007DAE RID: 32174
		private List<GorillaNetworkJoinTrigger> allJoinTriggers = new List<GorillaNetworkJoinTrigger>();
	}
}
