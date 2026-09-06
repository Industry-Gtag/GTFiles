using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription.AlarmClocks
{
	// Token: 0x02000FFD RID: 4093
	[DefaultExecutionOrder(10000)]
	public sealed class AlarmClockManager : MonoBehaviour
	{
		// Token: 0x170009B9 RID: 2489
		// (get) Token: 0x060065DE RID: 26078 RVA: 0x0020CAB3 File Offset: 0x0020ACB3
		// (set) Token: 0x060065DF RID: 26079 RVA: 0x0020CABA File Offset: 0x0020ACBA
		public static AlarmClockManager Instance { get; private set; }

		// Token: 0x170009BA RID: 2490
		// (get) Token: 0x060065E0 RID: 26080 RVA: 0x0020CAC2 File Offset: 0x0020ACC2
		// (set) Token: 0x060065E1 RID: 26081 RVA: 0x0020CACA File Offset: 0x0020ACCA
		public bool Initialized { get; private set; }

		// Token: 0x170009BB RID: 2491
		// (get) Token: 0x060065E2 RID: 26082 RVA: 0x0020CAD3 File Offset: 0x0020ACD3
		// (set) Token: 0x060065E3 RID: 26083 RVA: 0x0020CADB File Offset: 0x0020ACDB
		public string ActiveKey { get; private set; } = "";

		// Token: 0x060065E4 RID: 26084 RVA: 0x0020CAE4 File Offset: 0x0020ACE4
		private void Start()
		{
			if (AlarmClockManager.Instance != null)
			{
				Debug.LogError("Duplicate instance of singleton class AlarmClockManager.");
				Object.Destroy(this);
				return;
			}
			if (this._defaultSpawn == null)
			{
				Debug.LogError("No default spawn set in AlarmClockManager.");
				Object.Destroy(this);
				return;
			}
			AlarmClockManager.Instance = this;
			this.ActiveKey = PlayerPrefs.GetString("AlarmClock");
			if (!string.IsNullOrEmpty(this.ActiveKey))
			{
				AlarmClockManager.AlarmClockData clockData = this.GetClockData(this.ActiveKey);
				if (clockData != null && clockData.SpawnPoint != this._defaultSpawn)
				{
					this._activeClockData = clockData;
					this._teleportTarget = clockData.SpawnPoint;
					this.SendTelemetryEvent("wake_begin", this.ActiveKey);
					base.StartCoroutine(this.PerformWakeUpSequence());
					return;
				}
			}
			this.Initialized = true;
		}

		// Token: 0x060065E5 RID: 26085 RVA: 0x0020CBAD File Offset: 0x0020ADAD
		private IEnumerator PerformWakeUpSequence()
		{
			while (!GTPlayer.hasInstance)
			{
				yield return null;
			}
			GTPlayer.Instance.disableMovement = true;
			while (!GorillaTagger.hasInstance || !GorillaTagger.Instance.mainCamera)
			{
				yield return null;
			}
			PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.AlarmClock, this._loadingMessage);
			PersistLog.Log(string.Format("[AC][F{0}] Waiting for game systems", Time.frameCount));
			while (!AlarmClockManager.GameSystemsLoaded())
			{
				yield return null;
			}
			PersistLog.Log(string.Format("[AC][F{0}] Game systems loaded", Time.frameCount));
			if (!this._activeClockData.VIMOnly || (PlayFabAuthenticator.instance && SubscriptionManager.IsLocalSubscribed()))
			{
				this.RequestLoadZones();
				yield return null;
				bool isVStump = false;
				if (!this._activeClockData.SkipZoneReadyWait)
				{
					while (!this.AllZonesLoaded())
					{
						while (ZoneManagement.instance.AnyActiveLoadOps())
						{
							yield return null;
						}
						if (!this.AllZonesLoaded())
						{
							PersistLog.Log(string.Format("[AC][F{0}] Missing zones.  Requested:{1} Active: {2}", Time.frameCount, string.Join<GTZone>(", ", this._activeClockData.Zones), string.Join<GTZone>(", ", this.GetActiveZones())));
							this.RequestLoadZones();
							yield return null;
						}
					}
					PersistLog.Log(string.Format("[AC][F{0}] All zones loaded.", Time.frameCount));
				}
				else
				{
					isVStump = true;
				}
				foreach (XSceneRef xsceneRef in this._activeClockData.Objects)
				{
					GameObject gameObject;
					if (xsceneRef.TryResolve(out gameObject))
					{
						gameObject.SetActive(true);
					}
				}
				yield return null;
				GTPlayer.Instance.TeleportTo(this._teleportTarget, true, false);
				yield return null;
				int fixAttempts = 0;
				while ((GTPlayer.Instance.mainCamera.transform.position - this._teleportTarget.position).sqrMagnitude > this._wrongWarpTolerance * this._wrongWarpTolerance)
				{
					int i = fixAttempts + 1;
					fixAttempts = i;
					if (i > 10)
					{
						break;
					}
					PersistLog.Log(string.Format("[AC][F{0}] AlarmClockManager attempting wrong warp fix. (Off by {1:F2})", Time.frameCount, GTPlayer.Instance.mainCamera.transform.position - this._teleportTarget.position));
					GTPlayer.Instance.TeleportTo(this._teleportTarget, true, false);
					yield return null;
				}
				GTPlayer.Instance.disableMovement = false;
				PrivateUIRoom.StopForcedOverlay(PrivateUIRoom.OverlaySource.AlarmClock);
				if (isVStump)
				{
					UnityEvent onWakeUp = this.OnWakeUp;
					if (onWakeUp != null)
					{
						onWakeUp.Invoke();
					}
				}
				UnityEvent onSpawn = this._activeClockData.OnSpawn;
				if (onSpawn != null)
				{
					onSpawn.Invoke();
				}
				this.SendTelemetryEvent("wake_complete", this.ActiveKey);
			}
			else
			{
				if (PlayFabAuthenticator.instance == null)
				{
					PersistLog.Log(string.Format("[AC][F{0}] AlarmClockManager failed wake up because PlayFabAuthenticator was null.", Time.frameCount));
				}
				else if (PlayFabAuthenticator.instance.loginFailed)
				{
					PersistLog.Log(string.Format("[AC][F{0}] AlarmClockManager failed wake up because login failed.", Time.frameCount));
				}
				this.SendTelemetryEvent("wake_abort", this.ActiveKey);
				PersistLog.Log("VIM Only destination [" + this.ActiveKey + "] and no subscription.  Clearing clock data.");
				PlayerPrefs.SetString("AlarmClock", "");
				base.StartCoroutine(this.ClearUnsubPlayerData());
			}
			this.Initialized = true;
			GTPlayer.Instance.disableMovement = false;
			PrivateUIRoom.StopForcedOverlay(PrivateUIRoom.OverlaySource.AlarmClock);
			yield break;
		}

		// Token: 0x060065E6 RID: 26086 RVA: 0x0020CBBC File Offset: 0x0020ADBC
		public static void ToggleAlarmClock(AlarmClock clock)
		{
			AlarmClockManager.Instance.ToggleAlarmClockInternal(clock);
		}

		// Token: 0x060065E7 RID: 26087 RVA: 0x0020CBCC File Offset: 0x0020ADCC
		private void ToggleAlarmClockInternal(AlarmClock clock)
		{
			if (this.ActiveKey == clock.Key)
			{
				AlarmClock activeClock = this._activeClock;
				if (activeClock != null)
				{
					UnityEvent onDeactivate = activeClock.OnDeactivate;
					if (onDeactivate != null)
					{
						onDeactivate.Invoke();
					}
				}
				this._activeClock = null;
				this.ActiveKey = "";
				this.SendTelemetryEvent("unset", clock.Key);
			}
			else
			{
				AlarmClock activeClock2 = this._activeClock;
				if (activeClock2 != null)
				{
					UnityEvent onDeactivate2 = activeClock2.OnDeactivate;
					if (onDeactivate2 != null)
					{
						onDeactivate2.Invoke();
					}
				}
				this._activeClock = clock;
				AlarmClock activeClock3 = this._activeClock;
				if (activeClock3 != null)
				{
					UnityEvent onActivate = activeClock3.OnActivate;
					if (onActivate != null)
					{
						onActivate.Invoke();
					}
				}
				this.ActiveKey = clock.Key;
				this.SendTelemetryEvent("set", clock.Key);
			}
			PlayerPrefs.SetString("AlarmClock", this.ActiveKey);
			Debug.Log("Alarm clock data set to \"" + this.ActiveKey + "\".");
		}

		// Token: 0x060065E8 RID: 26088 RVA: 0x0020CCB3 File Offset: 0x0020AEB3
		private void OnDestroy()
		{
			if (AlarmClockManager.Instance == this)
			{
				AlarmClockManager.Instance = null;
			}
		}

		// Token: 0x060065E9 RID: 26089 RVA: 0x0020CCC8 File Offset: 0x0020AEC8
		private void SendTelemetryEvent(string eventType, string key)
		{
			if (this._telemetryDict == null)
			{
				this._telemetryDict = new Dictionary<string, object>();
			}
			this._telemetryDict["event_type"] = eventType;
			this._telemetryDict["clock_key"] = key;
			GorillaTelemetry.EnqueueTelemetryEvent("alarmclock_event", this._telemetryDict, null);
		}

		// Token: 0x060065EA RID: 26090 RVA: 0x0020CD1C File Offset: 0x0020AF1C
		[UsedImplicitly]
		private bool AllUniqueClockKeys()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (AlarmClockManager.AlarmClockData alarmClockData in this._clockData)
			{
				if (!hashSet.Add(alarmClockData.Key))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060065EB RID: 26091 RVA: 0x0020CD5C File Offset: 0x0020AF5C
		private static bool GameSystemsLoaded()
		{
			return (ZoneManagement.instance && ZoneManagement.instance.Initialized && CosmeticsController.instance && CosmeticsController.instance.v2_isCosmeticPlayFabCatalogDataLoaded && CosmeticsV2Spawner_Dirty.isPrepared && SubscriptionManager.LocalSubscriptionDataInitialized) || (PlayFabAuthenticator.instance != null && PlayFabAuthenticator.instance.loginFailed) || PlayFabAuthenticator.instance == null;
		}

		// Token: 0x060065EC RID: 26092 RVA: 0x0020CDD8 File Offset: 0x0020AFD8
		private bool AllZonesLoaded()
		{
			if (ZoneManagement.instance.AnyActiveLoadOps())
			{
				return false;
			}
			foreach (GTZone gtzone in this._activeClockData.Zones)
			{
				if (!ZoneManagement.instance.IsSceneLoaded(gtzone))
				{
					return false;
				}
				if (!ZoneManagement.IsZoneLoaded(gtzone))
				{
					PersistLog.Log(string.Format("[AC][F{0}] ZoneManagement reports Zone {1} is loaded but SceneManager says no.", Time.frameCount, gtzone));
					return false;
				}
			}
			return true;
		}

		// Token: 0x060065ED RID: 26093 RVA: 0x0020CE4C File Offset: 0x0020B04C
		private List<GTZone> GetActiveZones()
		{
			List<GTZone> list = new List<GTZone>();
			foreach (object obj in Enum.GetValues(typeof(GTZone)))
			{
				GTZone gtzone = (GTZone)obj;
				if (ZoneManagement.IsInZone(gtzone))
				{
					list.Add(gtzone);
				}
			}
			return list;
		}

		// Token: 0x060065EE RID: 26094 RVA: 0x0020CEC0 File Offset: 0x0020B0C0
		private void RequestLoadZones()
		{
			PersistLog.Log(string.Format("[AC][F{0}] Requesting zones: {1}", Time.frameCount, string.Join<GTZone>(", ", this._activeClockData.Zones)));
			ZoneManagement.SetActiveZones(this._activeClockData.Zones);
		}

		// Token: 0x060065EF RID: 26095 RVA: 0x0020CF00 File Offset: 0x0020B100
		private AlarmClockManager.AlarmClockData GetClockData(string key)
		{
			foreach (AlarmClockManager.AlarmClockData alarmClockData in this._clockData)
			{
				if (alarmClockData.Key == key)
				{
					return alarmClockData;
				}
			}
			return null;
		}

		// Token: 0x060065F0 RID: 26096 RVA: 0x0020CF37 File Offset: 0x0020B137
		public static bool IsVIMOnly(string key)
		{
			if (!AlarmClockManager.Instance)
			{
				return false;
			}
			AlarmClockManager.AlarmClockData clockData = AlarmClockManager.Instance.GetClockData(key);
			return clockData != null && clockData.VIMOnly;
		}

		// Token: 0x060065F1 RID: 26097 RVA: 0x0020CF5D File Offset: 0x0020B15D
		private void StartTracking()
		{
			base.StartCoroutine(this.DoTracking());
		}

		// Token: 0x060065F2 RID: 26098 RVA: 0x0020CF6C File Offset: 0x0020B16C
		private void StopTracking(float delay)
		{
			Debug.Log(string.Format("[AC][F{0}] STOP TRACKING", Time.frameCount));
			this._trackingEndTime = Time.time + delay;
		}

		// Token: 0x060065F3 RID: 26099 RVA: 0x0020CF94 File Offset: 0x0020B194
		private IEnumerator DoTracking()
		{
			this._trackingEndTime = float.PositiveInfinity;
			while (Time.time < this._trackingEndTime)
			{
				Debug.Log(string.Format("[AC][F{0}] Pos: {1} Off:[{2}] Distance: {3}[R{4}][C{5}]", new object[]
				{
					Time.frameCount,
					GTPlayer.Instance.transform.position,
					GTPlayer.Instance.LastPosition - this._teleportTarget.position,
					(GTPlayer.Instance.LastPosition - this._teleportTarget.position).magnitude,
					(GTPlayer.Instance.playerRigidBody.position - this._teleportTarget.position).magnitude,
					(GTPlayer.Instance.mainCamera.transform.position - this._teleportTarget.position).magnitude
				}));
				yield return null;
			}
			yield break;
		}

		// Token: 0x060065F4 RID: 26100 RVA: 0x0020CFA3 File Offset: 0x0020B1A3
		private IEnumerator ClearUnsubPlayerData()
		{
			PersistLog.Log("No subscription, warping home.");
			PlayerPrefs.SetString("AlarmClock", "");
			GTPlayer.Instance.TeleportTo(this._defaultSpawn, true, false);
			this.Initialized = true;
			yield return null;
			GTPlayer.Instance.disableMovement = false;
			PrivateUIRoom.StopForcedOverlay(PrivateUIRoom.OverlaySource.AlarmClock);
			yield break;
		}

		// Token: 0x040074F5 RID: 29941
		public const string SaveDataKey = "AlarmClock";

		// Token: 0x040074F8 RID: 29944
		[SerializeField]
		private string _loadingMessage = "";

		// Token: 0x040074F9 RID: 29945
		[SerializeField]
		private float _wrongWarpTolerance = 0.1f;

		// Token: 0x040074FA RID: 29946
		[SerializeField]
		private AlarmClockManager.AlarmClockData[] _clockData = new AlarmClockManager.AlarmClockData[0];

		// Token: 0x040074FB RID: 29947
		[SerializeField]
		private Transform _defaultSpawn;

		// Token: 0x040074FC RID: 29948
		public UnityEvent OnWakeUp;

		// Token: 0x040074FD RID: 29949
		private Transform _teleportTarget;

		// Token: 0x040074FF RID: 29951
		private AlarmClockManager.AlarmClockData _activeClockData;

		// Token: 0x04007500 RID: 29952
		[CanBeNull]
		private AlarmClock _activeClock;

		// Token: 0x04007501 RID: 29953
		private Dictionary<string, object> _telemetryDict;

		// Token: 0x04007502 RID: 29954
		private float _trackingEndTime;

		// Token: 0x02000FFE RID: 4094
		[Serializable]
		public class AlarmClockData
		{
			// Token: 0x04007503 RID: 29955
			public string Key;

			// Token: 0x04007504 RID: 29956
			public bool VIMOnly;

			// Token: 0x04007505 RID: 29957
			public GTZone[] Zones;

			// Token: 0x04007506 RID: 29958
			[Tooltip("Skip waiting for the zone scenes to report loaded. Use for zones with no real backing scene (e.g. customMaps / the virtual stump), which are activated by object toggling and would otherwise never satisfy the load check and hang the wake-up.")]
			public bool SkipZoneReadyWait;

			// Token: 0x04007507 RID: 29959
			public XSceneRef[] Objects;

			// Token: 0x04007508 RID: 29960
			public Transform SpawnPoint;

			// Token: 0x04007509 RID: 29961
			public UnityEvent OnSpawn;
		}
	}
}
