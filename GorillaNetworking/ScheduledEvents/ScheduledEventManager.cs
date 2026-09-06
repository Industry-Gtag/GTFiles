using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking.ScheduledEvents
{
	// Token: 0x0200112B RID: 4395
	[RequireComponent(typeof(PhotonView))]
	public class ScheduledEventManager : MonoBehaviour, IGorillaSliceableSimple, IInRoomCallbacks, IPunObservable
	{
		// Token: 0x17000A80 RID: 2688
		// (get) Token: 0x06006E29 RID: 28201 RVA: 0x002387AF File Offset: 0x002369AF
		// (set) Token: 0x06006E2A RID: 28202 RVA: 0x002387B6 File Offset: 0x002369B6
		public static ScheduledEventManager Instance { get; private set; }

		// Token: 0x17000A81 RID: 2689
		// (get) Token: 0x06006E2B RID: 28203 RVA: 0x002387BE File Offset: 0x002369BE
		public TimeSpan GracePeriod
		{
			get
			{
				return this.gracePeriodDuration;
			}
		}

		// Token: 0x17000A82 RID: 2690
		// (get) Token: 0x06006E2C RID: 28204 RVA: 0x002387C6 File Offset: 0x002369C6
		public bool DataReady
		{
			get
			{
				return !this.fetchInFlight;
			}
		}

		// Token: 0x17000A83 RID: 2691
		// (get) Token: 0x06006E2D RID: 28205 RVA: 0x002387D1 File Offset: 0x002369D1
		public bool IsResolved
		{
			get
			{
				return this.startKind > ScheduledEventManager.StartKind.Unresolved;
			}
		}

		// Token: 0x17000A84 RID: 2692
		// (get) Token: 0x06006E2E RID: 28206 RVA: 0x002387DC File Offset: 0x002369DC
		public bool HasEvent
		{
			get
			{
				return this.startKind == ScheduledEventManager.StartKind.Scheduled;
			}
		}

		// Token: 0x17000A85 RID: 2693
		// (get) Token: 0x06006E2F RID: 28207 RVA: 0x002387E8 File Offset: 0x002369E8
		public double SecondsUntilEventStart
		{
			get
			{
				if (!this.HasEvent)
				{
					return 0.0;
				}
				return PhotonTimestamp.Now.SecondsUntil(this.scheduledStart);
			}
		}

		// Token: 0x17000A86 RID: 2694
		// (get) Token: 0x06006E30 RID: 28208 RVA: 0x0023881A File Offset: 0x00236A1A
		public ScheduledEventPhase CurrentPhase
		{
			get
			{
				return this.currentPhase;
			}
		}

		// Token: 0x17000A87 RID: 2695
		// (get) Token: 0x06006E31 RID: 28209 RVA: 0x00238822 File Offset: 0x00236A22
		public int EventSubphase
		{
			get
			{
				return this.eventSubphase;
			}
		}

		// Token: 0x17000A88 RID: 2696
		// (get) Token: 0x06006E32 RID: 28210 RVA: 0x0023882A File Offset: 0x00236A2A
		// (set) Token: 0x06006E33 RID: 28211 RVA: 0x00238832 File Offset: 0x00236A32
		public DateTime PreviousEventSubphaseStartTime { get; private set; }

		// Token: 0x17000A89 RID: 2697
		// (get) Token: 0x06006E34 RID: 28212 RVA: 0x0023883B File Offset: 0x00236A3B
		// (set) Token: 0x06006E35 RID: 28213 RVA: 0x00238843 File Offset: 0x00236A43
		public DateTime EventSubphaseStartTime { get; private set; }

		// Token: 0x140000C1 RID: 193
		// (add) Token: 0x06006E36 RID: 28214 RVA: 0x0023884C File Offset: 0x00236A4C
		// (remove) Token: 0x06006E37 RID: 28215 RVA: 0x00238884 File Offset: 0x00236A84
		public event Action OnChanged;

		// Token: 0x140000C2 RID: 194
		// (add) Token: 0x06006E38 RID: 28216 RVA: 0x002388BC File Offset: 0x00236ABC
		// (remove) Token: 0x06006E39 RID: 28217 RVA: 0x002388F4 File Offset: 0x00236AF4
		public event Action<ScheduledEventPhase> OnPhaseChanged;

		// Token: 0x140000C3 RID: 195
		// (add) Token: 0x06006E3A RID: 28218 RVA: 0x0023892C File Offset: 0x00236B2C
		// (remove) Token: 0x06006E3B RID: 28219 RVA: 0x00238964 File Offset: 0x00236B64
		public event Action<int> OnSubphaseChanged;

		// Token: 0x06006E3C RID: 28220 RVA: 0x0023899C File Offset: 0x00236B9C
		public void SetEventSubphase(int subphase)
		{
			if (subphase == this.eventSubphase)
			{
				return;
			}
			if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
			{
				return;
			}
			this.eventSubphase = subphase;
			this.PreviousEventSubphaseStartTime = this.EventSubphaseStartTime;
			this.EventSubphaseStartTime = DateTime.Now;
			Action<int> onSubphaseChanged = this.OnSubphaseChanged;
			if (onSubphaseChanged == null)
			{
				return;
			}
			onSubphaseChanged(subphase);
		}

		// Token: 0x06006E3D RID: 28221 RVA: 0x002389F1 File Offset: 0x00236BF1
		private void Awake()
		{
			if (ScheduledEventManager.Instance != null && ScheduledEventManager.Instance != this)
			{
				Object.Destroy(this);
				return;
			}
			ScheduledEventManager.Instance = this;
			this.currentPhase = ScheduledEventPhase.None;
		}

		// Token: 0x06006E3E RID: 28222 RVA: 0x00238A24 File Offset: 0x00236C24
		private async void Start()
		{
			if (this.useForcedEventTime)
			{
				this.ApplyForcedEventTime();
			}
			else if (!string.IsNullOrEmpty(this.titleDataKey))
			{
				await this.FetchReferenceDate();
			}
			PhotonNetwork.AddCallbackTarget(this);
			if (NetworkSystem.Instance != null)
			{
				NetworkSystem.Instance.OnMultiplayerStarted += this.OnMultiplayerStarted;
				NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnReturnedToSinglePlayer;
				if (NetworkSystem.Instance.InRoom)
				{
					this.OnMultiplayerStarted();
				}
			}
			if (GorillaComputer.instance != null)
			{
				GorillaComputer instance = GorillaComputer.instance;
				instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnServerTimeUpdated));
			}
		}

		// Token: 0x06006E3F RID: 28223 RVA: 0x00238A5C File Offset: 0x00236C5C
		private void OnDestroy()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
			if (NetworkSystem.Instance != null)
			{
				NetworkSystem.Instance.OnMultiplayerStarted -= this.OnMultiplayerStarted;
				NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnReturnedToSinglePlayer;
			}
			if (GorillaComputer.instance != null)
			{
				GorillaComputer instance = GorillaComputer.instance;
				instance.OnServerTimeUpdated = (Action)Delegate.Remove(instance.OnServerTimeUpdated, new Action(this.OnServerTimeUpdated));
			}
			if (ScheduledEventManager.Instance == this)
			{
				ScheduledEventManager.Instance = null;
			}
		}

		// Token: 0x06006E40 RID: 28224 RVA: 0x00238B08 File Offset: 0x00236D08
		private void OnEnable()
		{
			if (ScheduledEventManager.Instance == this)
			{
				GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			}
		}

		// Token: 0x06006E41 RID: 28225 RVA: 0x00019269 File Offset: 0x00017469
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06006E42 RID: 28226 RVA: 0x00238B1E File Offset: 0x00236D1E
		public void SliceUpdate()
		{
			this.RefreshPhase();
		}

		// Token: 0x06006E43 RID: 28227 RVA: 0x00238B26 File Offset: 0x00236D26
		public void Register(ScheduledEventControlledObject obj)
		{
			if (obj == null)
			{
				return;
			}
			this.registered.Add(obj);
			this.ApplyPhaseTo(obj);
		}

		// Token: 0x06006E44 RID: 28228 RVA: 0x00238B46 File Offset: 0x00236D46
		public void Unregister(ScheduledEventControlledObject obj)
		{
			this.registered.Remove(obj);
		}

		// Token: 0x06006E45 RID: 28229 RVA: 0x00238B58 File Offset: 0x00236D58
		private void ApplyPhaseTo(ScheduledEventControlledObject obj)
		{
			if (obj == null || obj.gameObject == null)
			{
				return;
			}
			bool flag = obj.MatchesPhase(this.currentPhase);
			if (obj.gameObject.activeSelf != flag)
			{
				obj.gameObject.SetActive(flag);
			}
		}

		// Token: 0x06006E46 RID: 28230 RVA: 0x00238BA4 File Offset: 0x00236DA4
		private void ApplyPhaseToAll()
		{
			foreach (ScheduledEventControlledObject scheduledEventControlledObject in this.registered)
			{
				this.ApplyPhaseTo(scheduledEventControlledObject);
			}
		}

		// Token: 0x06006E47 RID: 28231 RVA: 0x00238BF8 File Offset: 0x00236DF8
		private void RefreshPhase()
		{
			this.MaintainRoomStateAsMaster();
			ScheduledEventPhase scheduledEventPhase = this.ComputePhase();
			if (scheduledEventPhase == this.currentPhase)
			{
				return;
			}
			if (scheduledEventPhase == ScheduledEventPhase.During && this.currentPhase == ScheduledEventPhase.Before && NetworkSystem.Instance != null && NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient && this.lastKnownState == "regular")
			{
				this.SetRoomState("event-in-progress");
			}
			this.currentPhase = scheduledEventPhase;
			if (scheduledEventPhase == ScheduledEventPhase.During)
			{
				this.SetEventSubphase(0);
			}
			Action<ScheduledEventPhase> onPhaseChanged = this.OnPhaseChanged;
			if (onPhaseChanged != null)
			{
				onPhaseChanged(scheduledEventPhase);
			}
			this.ApplyPhaseToAll();
		}

		// Token: 0x06006E48 RID: 28232 RVA: 0x00238C94 File Offset: 0x00236E94
		private ScheduledEventPhase ComputePhase()
		{
			if (this.showEndedInRoom)
			{
				return ScheduledEventPhase.After;
			}
			if (this.lastKnownState == "event-in-progress")
			{
				return ScheduledEventPhase.During;
			}
			if (this.HasEvent)
			{
				if (!(PhotonTimestamp.Now >= this.scheduledStart))
				{
					return ScheduledEventPhase.Before;
				}
				return ScheduledEventPhase.During;
			}
			else
			{
				if (!(NetworkSystem.Instance != null) || !NetworkSystem.Instance.InRoom)
				{
					return this.ComputeOfflinePhase();
				}
				if (!this.useForcedEventTime && string.IsNullOrEmpty(this.titleDataKey))
				{
					return ScheduledEventPhase.NoEvent;
				}
				if (this.IsResolved)
				{
					return ScheduledEventPhase.After;
				}
				return ScheduledEventPhase.Before;
			}
		}

		// Token: 0x06006E49 RID: 28233 RVA: 0x00238D28 File Offset: 0x00236F28
		private ScheduledEventPhase ComputeOfflinePhase()
		{
			if (!this.useForcedEventTime && string.IsNullOrEmpty(this.titleDataKey))
			{
				return ScheduledEventPhase.NoEvent;
			}
			if (!this.scheduledStartKnown)
			{
				return ScheduledEventPhase.Before;
			}
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			ScheduledEventInfo current = this.GetCurrent(serverTime);
			bool flag = ScheduledEventMatchmaking.HasSeenScheduledEventRecently(serverTime);
			if (ScheduledEventMatchmaking.ResolveCreateState(current, serverTime, flag) == "post-event")
			{
				return ScheduledEventPhase.After;
			}
			if (!current.isActive)
			{
				return ScheduledEventPhase.After;
			}
			return ScheduledEventPhase.Before;
		}

		// Token: 0x06006E4A RID: 28234 RVA: 0x00238D99 File Offset: 0x00236F99
		private void OnServerTimeUpdated()
		{
			if (this.useForcedEventTime)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.titleDataKey) && !this.fetchInFlight)
			{
				this.FetchReferenceDate();
			}
		}

		// Token: 0x06006E4B RID: 28235 RVA: 0x00238DC0 File Offset: 0x00236FC0
		private void ApplyForcedEventTime()
		{
			if (string.IsNullOrEmpty(this.forceEventTime))
			{
				Debug.Log("ScheduledEventManager :: useForcedEventTime is true but forceEventTime is empty");
				return;
			}
			DateTime dateTime;
			if (!DateTime.TryParse(this.forceEventTime, out dateTime))
			{
				Debug.Log("ScheduledEventManager :: could not parse forceEventTime '" + this.forceEventTime + "'");
				return;
			}
			this.scheduledStartUtc = ((dateTime.Kind == DateTimeKind.Utc) ? dateTime : DateTime.SpecifyKind(dateTime, DateTimeKind.Local)).ToUniversalTime();
			this.scheduledStartKnown = true;
		}

		// Token: 0x06006E4C RID: 28236 RVA: 0x00238E38 File Offset: 0x00237038
		private async Task FetchReferenceDate()
		{
			this.fetchInFlight = true;
			while (PlayFabTitleDataCache.Instance == null)
			{
				await Task.Yield();
			}
			PlayFabTitleDataCache.Instance.GetTitleData(this.titleDataKey, new Action<string>(this.OnTitleData), new Action<PlayFabError>(this.OnTitleDataError), false);
			PlayFabTitleDataCache.Instance.GetTitleData(this.graceTitleDataKey, new Action<string>(this.OnGraceTitleData), new Action<PlayFabError>(this.OnGraceTitleDataError), false);
		}

		// Token: 0x06006E4D RID: 28237 RVA: 0x00238E7C File Offset: 0x0023707C
		private void OnTitleData(string raw)
		{
			this.fetchInFlight = false;
			DateTime dateTime;
			if (!DateTime.TryParse(raw, out dateTime))
			{
				Debug.Log("ScheduledEventManager :: could not parse title data '" + raw + "' for key " + this.titleDataKey);
				return;
			}
			this.scheduledStartUtc = ((dateTime.Kind == DateTimeKind.Unspecified) ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) : dateTime.ToUniversalTime());
			this.scheduledStartKnown = true;
		}

		// Token: 0x06006E4E RID: 28238 RVA: 0x00238EDC File Offset: 0x002370DC
		private void OnTitleDataError(PlayFabError error)
		{
			this.fetchInFlight = false;
			Debug.Log(string.Format("ScheduledEventManager :: title data fetch failed: {0}", error));
		}

		// Token: 0x06006E4F RID: 28239 RVA: 0x00238EF5 File Offset: 0x002370F5
		private void OnGraceTitleData(string raw)
		{
			this.fetchInFlight = false;
			if (!TimeSpan.TryParse(raw, out this.gracePeriodDuration))
			{
				Debug.Log("ScheduledEventManager :: could not parse time interval '" + raw + "' for key " + this.graceTitleDataKey);
				return;
			}
		}

		// Token: 0x06006E50 RID: 28240 RVA: 0x00238F28 File Offset: 0x00237128
		private void OnGraceTitleDataError(PlayFabError error)
		{
			this.fetchInFlight = false;
			Debug.Log(string.Format("ScheduledEventManager :: grace title data fetch failed: {0}", error));
			this.gracePeriodDuration = TimeSpan.FromMinutes(10.0);
		}

		// Token: 0x06006E51 RID: 28241 RVA: 0x00238F58 File Offset: 0x00237158
		public ScheduledEventInfo GetCurrent(DateTime serverNow)
		{
			if (!this.scheduledStartKnown)
			{
				return ScheduledEventInfo.None;
			}
			DateTime dateTime = this.scheduledStartUtc + this.gracePeriodDuration;
			if (serverNow >= dateTime)
			{
				return ScheduledEventInfo.None;
			}
			return new ScheduledEventInfo
			{
				isActive = true,
				scheduledStart = this.scheduledStartUtc
			};
		}

		// Token: 0x06006E52 RID: 28242 RVA: 0x00238FB4 File Offset: 0x002371B4
		private void OnMultiplayerStarted()
		{
			this.lastKnownState = this.ReadRoomState();
			this.showEndedInRoom = this.lastKnownState == "post-event";
			if (NetworkSystem.Instance.IsMasterClient && this.startKind == ScheduledEventManager.StartKind.Unresolved)
			{
				PhotonTimestamp? photonTimestamp = this.ComputeStartTime();
				if (photonTimestamp != null)
				{
					this.SetStartState(ScheduledEventManager.StartKind.Scheduled, photonTimestamp.Value);
				}
				else
				{
					this.SetStartState(ScheduledEventManager.StartKind.NoEvent, default(PhotonTimestamp));
				}
			}
			this.RefreshPhase();
		}

		// Token: 0x06006E53 RID: 28243 RVA: 0x00239030 File Offset: 0x00237230
		private void OnReturnedToSinglePlayer()
		{
			this.lastKnownState = null;
			this.showEndedInRoom = false;
			this.SetStartState(ScheduledEventManager.StartKind.Unresolved, default(PhotonTimestamp));
			this.RefreshPhase();
		}

		// Token: 0x06006E54 RID: 28244 RVA: 0x00239064 File Offset: 0x00237264
		public void OnShowEnded()
		{
			if (this.currentPhase == ScheduledEventPhase.During)
			{
				this.PreviousEventSubphaseStartTime = this.EventSubphaseStartTime;
				this.EventSubphaseStartTime = DateTime.Now;
			}
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			this.SetRoomState("post-event");
		}

		// Token: 0x06006E55 RID: 28245 RVA: 0x002390B8 File Offset: 0x002372B8
		public void DebugStartCountdown()
		{
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			this.showEndedInRoom = false;
			if (this.lastKnownState != "regular")
			{
				this.SetRoomState("regular");
			}
			this.SetStartState(ScheduledEventManager.StartKind.Scheduled, PhotonTimestamp.Now + 5.0);
		}

		// Token: 0x06006E56 RID: 28246 RVA: 0x0023911C File Offset: 0x0023731C
		private string ReadRoomState()
		{
			if (PhotonNetwork.CurrentRoom == null)
			{
				return null;
			}
			object obj;
			if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("scheduledEventState", out obj))
			{
				return obj as string;
			}
			return null;
		}

		// Token: 0x06006E57 RID: 28247 RVA: 0x00239154 File Offset: 0x00237354
		private void SetRoomState(string state)
		{
			if (PhotonNetwork.CurrentRoom == null)
			{
				return;
			}
			Hashtable hashtable = new Hashtable { { "scheduledEventState", state } };
			PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
		}

		// Token: 0x06006E58 RID: 28248 RVA: 0x0023918C File Offset: 0x0023738C
		void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			object obj;
			if (!propertiesThatChanged.TryGetValue("scheduledEventState", out obj))
			{
				return;
			}
			string text = this.lastKnownState;
			string text2 = obj as string;
			this.lastKnownState = text2;
			if (text == "event-in-progress" && text2 == "post-event")
			{
				ScheduledEventMatchmaking.MarkSeenScheduledEventNow(GorillaComputer.instance.GetServerTime());
			}
			if (text2 == "post-event")
			{
				this.showEndedInRoom = true;
			}
			else if (text2 == "event-in-progress")
			{
				this.showEndedInRoom = false;
			}
			this.RefreshPhase();
		}

		// Token: 0x06006E59 RID: 28249 RVA: 0x00002C2D File Offset: 0x00000E2D
		void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
		{
		}

		// Token: 0x06006E5A RID: 28250 RVA: 0x00002C2D File Offset: 0x00000E2D
		void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
		{
		}

		// Token: 0x06006E5B RID: 28251 RVA: 0x00002C2D File Offset: 0x00000E2D
		void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
		{
		}

		// Token: 0x06006E5C RID: 28252 RVA: 0x00002C2D File Offset: 0x00000E2D
		void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
		}

		// Token: 0x06006E5D RID: 28253 RVA: 0x00239218 File Offset: 0x00237418
		private void MaintainRoomStateAsMaster()
		{
			if (NetworkSystem.Instance == null || !NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (this.lastKnownState != "post-event")
			{
				return;
			}
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			if (ScheduledEventMatchmaking.GracePeriodEnded(this.GetCurrent(serverTime), serverTime))
			{
				this.SetRoomState("regular");
				this.lastKnownState = "regular";
			}
		}

		// Token: 0x06006E5E RID: 28254 RVA: 0x00239290 File Offset: 0x00237490
		private PhotonTimestamp? ComputeStartTime()
		{
			if (this.lastKnownState == "event-in-progress" || this.lastKnownState == "post-event")
			{
				return null;
			}
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			ScheduledEventInfo current = this.GetCurrent(serverTime);
			if (!current.isActive)
			{
				return null;
			}
			double totalSeconds = (current.scheduledStart - serverTime).TotalSeconds;
			double num = 300.0;
			double num2 = Math.Max(totalSeconds, num);
			return new PhotonTimestamp?(PhotonTimestamp.Now + num2);
		}

		// Token: 0x06006E5F RID: 28255 RVA: 0x0023932C File Offset: 0x0023752C
		void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				double num;
				switch (this.startKind)
				{
				case ScheduledEventManager.StartKind.Unresolved:
					num = double.NaN;
					break;
				case ScheduledEventManager.StartKind.NoEvent:
					num = -1.0;
					break;
				case ScheduledEventManager.StartKind.Scheduled:
					num = this.scheduledStart.Value;
					break;
				default:
					num = double.NaN;
					break;
				}
				double num2 = num;
				stream.SendNext(num2);
				stream.SendNext(this.eventSubphase);
				return;
			}
			double num3 = (double)stream.ReceiveNext();
			if (!double.IsFinite(num3))
			{
				this.SetStartState(ScheduledEventManager.StartKind.Unresolved, default(PhotonTimestamp));
			}
			else if (num3 < 0.0)
			{
				this.SetStartState(ScheduledEventManager.StartKind.NoEvent, default(PhotonTimestamp));
			}
			else
			{
				this.SetStartState(ScheduledEventManager.StartKind.Scheduled, new PhotonTimestamp(num3));
			}
			int num4 = (int)stream.ReceiveNext();
			if (num4 != this.eventSubphase)
			{
				this.eventSubphase = num4;
				Action<int> onSubphaseChanged = this.OnSubphaseChanged;
				if (onSubphaseChanged == null)
				{
					return;
				}
				onSubphaseChanged(this.eventSubphase);
			}
		}

		// Token: 0x06006E60 RID: 28256 RVA: 0x00239434 File Offset: 0x00237634
		private void SetStartState(ScheduledEventManager.StartKind kind, PhotonTimestamp ts)
		{
			if (kind == this.startKind)
			{
				if (kind != ScheduledEventManager.StartKind.Scheduled)
				{
					return;
				}
				if (ts.Value == this.scheduledStart.Value)
				{
					return;
				}
			}
			this.startKind = kind;
			this.scheduledStart = ts;
			Action onChanged = this.OnChanged;
			if (onChanged != null)
			{
				onChanged();
			}
			this.RefreshPhase();
		}

		// Token: 0x04007E71 RID: 32369
		public const int SCHEDULED_EVENT_MAX_DELAY_MINUTES = 5;

		// Token: 0x04007E72 RID: 32370
		public const int SCHEDULED_EVENT_SEEN_COOLDOWN_HOURS = 12;

		// Token: 0x04007E74 RID: 32372
		[Header("Schedule")]
		[SerializeField]
		[Tooltip("PlayFab Title Data key whose value parses as a DateTime (date + time of day). Empty = no event configured.")]
		private string titleDataKey;

		// Token: 0x04007E75 RID: 32373
		[SerializeField]
		[Tooltip("PlayFab Title Data key whose value parses as an ElapsedTime (e.g. \"00:10:00\").")]
		private string graceTitleDataKey;

		// Token: 0x04007E76 RID: 32374
		[SerializeField]
		[Tooltip("If true, ignore titleDataKey and use forceEventTime instead. For local testing without editing PlayFab Title Data.")]
		private bool useForcedEventTime;

		// Token: 0x04007E77 RID: 32375
		[SerializeField]
		[Tooltip("Event start time in LOCAL time (parsed via DateTime.Parse). Only used when useForcedEventTime is true. Example: 2026-04-21 14:30:00")]
		private string forceEventTime;

		// Token: 0x04007E78 RID: 32376
		private DateTime scheduledStartUtc = DateTime.MinValue;

		// Token: 0x04007E79 RID: 32377
		private TimeSpan gracePeriodDuration = TimeSpan.FromMinutes(10.0);

		// Token: 0x04007E7A RID: 32378
		private bool scheduledStartKnown;

		// Token: 0x04007E7B RID: 32379
		private bool fetchInFlight;

		// Token: 0x04007E7C RID: 32380
		private ScheduledEventManager.StartKind startKind;

		// Token: 0x04007E7D RID: 32381
		private PhotonTimestamp scheduledStart;

		// Token: 0x04007E7E RID: 32382
		private string lastKnownState;

		// Token: 0x04007E7F RID: 32383
		private bool showEndedInRoom;

		// Token: 0x04007E80 RID: 32384
		private ScheduledEventPhase currentPhase;

		// Token: 0x04007E81 RID: 32385
		private int eventSubphase = -1;

		// Token: 0x04007E82 RID: 32386
		private readonly HashSet<ScheduledEventControlledObject> registered = new HashSet<ScheduledEventControlledObject>();

		// Token: 0x0200112C RID: 4396
		private enum StartKind
		{
			// Token: 0x04007E89 RID: 32393
			Unresolved,
			// Token: 0x04007E8A RID: 32394
			NoEvent,
			// Token: 0x04007E8B RID: 32395
			Scheduled
		}
	}
}
