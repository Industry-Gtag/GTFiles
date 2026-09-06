using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking
{
	// Token: 0x020010F2 RID: 4338
	public class GorillaNetworkJoinTrigger : GorillaTriggerBox
	{
		// Token: 0x17000A58 RID: 2648
		// (get) Token: 0x06006CD0 RID: 27856 RVA: 0x00232D9C File Offset: 0x00230F9C
		public GroupJoinZoneAB groupJoinRequiredZonesAB
		{
			get
			{
				return new GroupJoinZoneAB
				{
					a = this.groupJoinRequiredZones,
					b = this.groupJoinRequiredZonesB
				};
			}
		}

		// Token: 0x06006CD1 RID: 27857 RVA: 0x00232DCC File Offset: 0x00230FCC
		private void Start()
		{
			if (this.primaryTriggerForMyZone == null)
			{
				this.primaryTriggerForMyZone = this;
			}
			if (this.primaryTriggerForMyZone == this)
			{
				GorillaComputer.instance.RegisterPrimaryJoinTrigger(this);
			}
			PhotonNetworkController.Instance.RegisterJoinTrigger(this);
			if (!this.didRegisterForCallbacks && this.ui != null)
			{
				this.didRegisterForCallbacks = true;
				FriendshipGroupDetection.Instance.AddGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
			}
		}

		// Token: 0x06006CD2 RID: 27858 RVA: 0x00232E4C File Offset: 0x0023104C
		public void RegisterUI(JoinTriggerUI ui)
		{
			this.ui = ui;
			if (!this.didRegisterForCallbacks && FriendshipGroupDetection.Instance != null)
			{
				this.didRegisterForCallbacks = true;
				FriendshipGroupDetection.Instance.AddGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
			}
			this.UpdateUI();
		}

		// Token: 0x06006CD3 RID: 27859 RVA: 0x00232E98 File Offset: 0x00231098
		public void UnregisterUI(JoinTriggerUI ui)
		{
			this.ui = null;
		}

		// Token: 0x06006CD4 RID: 27860 RVA: 0x00232EA1 File Offset: 0x002310A1
		private void OnDestroy()
		{
			if (this.didRegisterForCallbacks)
			{
				FriendshipGroupDetection.Instance.RemoveGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
			}
		}

		// Token: 0x06006CD5 RID: 27861 RVA: 0x00232EC1 File Offset: 0x002310C1
		private void OnGroupPositionsChanged(GroupJoinZoneAB groupZone)
		{
			this.UpdateUI();
		}

		// Token: 0x06006CD6 RID: 27862 RVA: 0x00232ECC File Offset: 0x002310CC
		public void UpdateUI()
		{
			if (this.ui == null || NetworkSystem.Instance == null)
			{
				return;
			}
			if (GorillaScoreboardTotalUpdater.instance.offlineTextErrorString != null)
			{
				this.ui.SetState(JoinTriggerVisualState.ConnectionError, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				this.ui.SetState(JoinTriggerVisualState.InPrivateRoom, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString == this.GetFullDesiredGameModeString())
			{
				this.ui.SetState(JoinTriggerVisualState.AlreadyInRoom, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				if (this.CanPartyJoin() && (!this.ui.HasFriendCollider || FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.ui.FriendJoinCollider, false)))
				{
					this.ui.SetState(JoinTriggerVisualState.LeaveRoomAndPartyJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
					return;
				}
				this.ui.SetState(JoinTriggerVisualState.AbandonPartyAndSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			else
			{
				if (!NetworkSystem.Instance.InRoom)
				{
					this.ui.SetState(JoinTriggerVisualState.NotConnectedSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
					return;
				}
				if (PhotonNetworkController.Instance.currentJoinTrigger == this.primaryTriggerForMyZone)
				{
					this.ui.SetState(JoinTriggerVisualState.ChangingGameModeSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
					return;
				}
				this.ui.SetState(JoinTriggerVisualState.LeaveRoomAndSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
		}

		// Token: 0x06006CD7 RID: 27863 RVA: 0x00233178 File Offset: 0x00231378
		private string GetActiveNetworkZone()
		{
			return PhotonNetworkController.Instance.currentJoinTrigger.networkZone.ToUpper();
		}

		// Token: 0x06006CD8 RID: 27864 RVA: 0x00233190 File Offset: 0x00231390
		private string GetDesiredNetworkZone()
		{
			return this.networkZone.ToUpper();
		}

		// Token: 0x06006CD9 RID: 27865 RVA: 0x0023319D File Offset: 0x0023139D
		public static string GetActiveGameType()
		{
			GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
			return ((activeGameMode != null) ? activeGameMode.GameModeName() : null) ?? "";
		}

		// Token: 0x06006CDA RID: 27866 RVA: 0x002331BC File Offset: 0x002313BC
		public GameModeType GetDesiredGameModeType()
		{
			GameModeType gameModeType;
			return GameMode.GameModeZoneMapping.VerifyModeForZone((this.zone != GTZone.none) ? this.zone : VRRig.LocalRig.zoneEntity.currentZone, Enum.TryParse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true, out gameModeType) ? gameModeType : GameModeType.Casual, NetworkSystem.Instance.SessionIsPrivate);
		}

		// Token: 0x06006CDB RID: 27867 RVA: 0x00233220 File Offset: 0x00231420
		public string GetDesiredGameType()
		{
			return this.GetDesiredGameModeType().ToString();
		}

		// Token: 0x06006CDC RID: 27868 RVA: 0x00233241 File Offset: 0x00231441
		public string GetDesiredGameTypeLocalized()
		{
			return GorillaGameManager.GameModeEnumToName(this.GetDesiredGameModeType());
		}

		// Token: 0x06006CDD RID: 27869 RVA: 0x0023324E File Offset: 0x0023144E
		public virtual string GetFullDesiredGameModeString()
		{
			return new GameModeString
			{
				zone = this.networkZone,
				queue = GorillaComputer.instance.currentQueue,
				gameType = this.GetDesiredGameType()
			}.ToString();
		}

		// Token: 0x06006CDE RID: 27870 RVA: 0x00233284 File Offset: 0x00231484
		public virtual bool SameZoneAsOverride()
		{
			return NetworkSystem.Instance.groupJoinOverrideGameMode.StartsWith(this.networkZone);
		}

		// Token: 0x06006CDF RID: 27871 RVA: 0x0023329B File Offset: 0x0023149B
		public virtual byte GetRoomSize(bool subscribed)
		{
			return RoomSystem.GetRoomSizeForCreate(this.zone, this.GetDesiredGameModeType(), false, subscribed);
		}

		// Token: 0x06006CE0 RID: 27872 RVA: 0x002332B0 File Offset: 0x002314B0
		public bool CanPartyJoin()
		{
			return this.CanPartyJoin(FriendshipGroupDetection.Instance.partyZone);
		}

		// Token: 0x06006CE1 RID: 27873 RVA: 0x002332C2 File Offset: 0x002314C2
		public bool CanPartyJoin(GroupJoinZoneAB zone)
		{
			return (this.groupJoinRequiredZonesAB & zone) == zone;
		}

		// Token: 0x06006CE2 RID: 27874 RVA: 0x002332D8 File Offset: 0x002314D8
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (this.isSubsOnly)
			{
				if (SubscriptionManager.IsLocalSubscribed())
				{
					this.SubsPublicJoin();
				}
				return;
			}
			if (GorillaNetworkJoinTrigger.triggerJoinsDisabled)
			{
				Debug.Log("GorillaNetworkJoinTrigger::OnBoxTriggered - blocking join call");
				return;
			}
			GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
			if (NetworkSystem.Instance.groupJoinInProgress)
			{
				return;
			}
			List<ValueTuple<string, string>> list = new List<ValueTuple<string, string>>();
			foreach (AdditionalCustomProperty additionalCustomProperty in this.additionalJoinCustomProperties)
			{
				list.Add(new ValueTuple<string, string>(additionalCustomProperty.key, additionalCustomProperty.value));
			}
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				if (this.ignoredIfInParty)
				{
					return;
				}
				if (NetworkSystem.Instance.netState == NetSystemState.Connecting || NetworkSystem.Instance.netState == NetSystemState.Disconnecting || NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon)
				{
					return;
				}
				if (NetworkSystem.Instance.InRoom)
				{
					if (NetworkSystem.Instance.GameModeString == this.GetFullDesiredGameModeString())
					{
						GTDev.Log<string>("JoinTrigger: Ignoring party join/leave because " + this.networkZone + " is already the game mode", null);
						return;
					}
					if (NetworkSystem.Instance.SessionIsPrivate)
					{
						GTDev.Log<string>("JoinTrigger: Ignoring party join/leave because we're in a private room", null);
						return;
					}
					if (this.SameZoneAsOverride())
					{
						GTDev.Log<string>("JoinTrigger: Ignoring party join/leave because we joined as a group, and this trigger matches the zone for the override, so there's no reason to attempt to leave", null);
						return;
					}
				}
				if (this.CanPartyJoin())
				{
					Debug.Log(string.Format("JoinTrigger: Attempting party join in 1 second! <{0}> accepts <{1}>", this.groupJoinRequiredZones, FriendshipGroupDetection.Instance.partyZone));
					PhotonNetworkController.Instance.DeferJoining(1f);
					FriendshipGroupDetection.Instance.SendAboutToGroupJoin();
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this, JoinType.JoinWithParty, list, false);
					return;
				}
				Debug.Log(string.Format("JoinTrigger: LeaveGroup: Leaving party and will solo join, wanted <{0}> but got <{1}>", this.groupJoinRequiredZones, FriendshipGroupDetection.Instance.partyZone));
				FriendshipGroupDetection.Instance.LeaveParty();
				PhotonNetworkController.Instance.DeferJoining(1f);
			}
			else
			{
				Debug.Log("JoinTrigger: Solo join (not in a group)");
				PhotonNetworkController.Instance.ClearDeferredJoin();
			}
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this, JoinType.Solo, list, false);
		}

		// Token: 0x06006CE3 RID: 27875 RVA: 0x002334F4 File Offset: 0x002316F4
		public void SubsPublicJoin()
		{
			if (GorillaNetworkJoinTrigger.triggerJoinsDisabled)
			{
				Debug.Log("GorillaNetworkJoinTrigger::SubsPublicJoin - blocking join call");
				return;
			}
			GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
			PhotonNetworkController.Instance.ClearDeferredJoin();
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this, JoinType.Solo, null, SubscriptionManager.IsLocalSubscribed());
		}

		// Token: 0x06006CE4 RID: 27876 RVA: 0x0023354A File Offset: 0x0023174A
		public static void DisableTriggerJoins()
		{
			Debug.Log("[GorillaNetworkJoinTrigger::DisableTriggerJoins] Disabling Trigger-based Room Joins...");
			GorillaNetworkJoinTrigger.triggerJoinsDisabled = true;
		}

		// Token: 0x06006CE5 RID: 27877 RVA: 0x0023355C File Offset: 0x0023175C
		public static void EnableTriggerJoins()
		{
			Debug.Log("[GorillaNetworkJoinTrigger::EnableTriggerJoins] Enabling Trigger-based Room Joins...");
			GorillaNetworkJoinTrigger.triggerJoinsDisabled = false;
		}

		// Token: 0x04007D26 RID: 32038
		public GameObject[] makeSureThisIsDisabled;

		// Token: 0x04007D27 RID: 32039
		public GameObject[] makeSureThisIsEnabled;

		// Token: 0x04007D28 RID: 32040
		public GTZone zone;

		// Token: 0x04007D29 RID: 32041
		public GroupJoinZoneA groupJoinRequiredZones;

		// Token: 0x04007D2A RID: 32042
		public GroupJoinZoneB groupJoinRequiredZonesB;

		// Token: 0x04007D2B RID: 32043
		[FormerlySerializedAs("gameModeName")]
		public string networkZone;

		// Token: 0x04007D2C RID: 32044
		public GorillaFriendCollider myCollider;

		// Token: 0x04007D2D RID: 32045
		public GorillaNetworkJoinTrigger primaryTriggerForMyZone;

		// Token: 0x04007D2E RID: 32046
		public bool ignoredIfInParty;

		// Token: 0x04007D2F RID: 32047
		public bool isSubsOnly;

		// Token: 0x04007D30 RID: 32048
		private JoinTriggerUI ui;

		// Token: 0x04007D31 RID: 32049
		private bool didRegisterForCallbacks;

		// Token: 0x04007D32 RID: 32050
		public AdditionalCustomProperty[] additionalJoinCustomProperties;

		// Token: 0x04007D33 RID: 32051
		private static bool triggerJoinsDisabled;
	}
}
