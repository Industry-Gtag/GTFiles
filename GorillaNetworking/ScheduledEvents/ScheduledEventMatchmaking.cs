using System;
using ExitGames.Client.Photon;
using UnityEngine;

namespace GorillaNetworking.ScheduledEvents
{
	// Token: 0x02001130 RID: 4400
	public static class ScheduledEventMatchmaking
	{
		// Token: 0x06006E67 RID: 28263 RVA: 0x002397A2 File Offset: 0x002379A2
		public static bool GracePeriodEnded(ScheduledEventInfo e, DateTime serverNow)
		{
			return !e.isActive || serverNow > e.scheduledStart + ScheduledEventManager.Instance.GracePeriod;
		}

		// Token: 0x06006E68 RID: 28264 RVA: 0x002397CC File Offset: 0x002379CC
		public static string ResolveCreateState(ScheduledEventInfo e, DateTime serverNow, bool creatorSeenRecently)
		{
			if (!e.isActive || ScheduledEventMatchmaking.GracePeriodEnded(e, serverNow))
			{
				return "regular";
			}
			DateTime dateTime = e.scheduledStart + ScheduledEventManager.Instance.GracePeriod;
			bool flag = serverNow > dateTime - TimeSpan.FromMinutes(5.0);
			if (!(serverNow < e.scheduledStart) && (creatorSeenRecently || flag))
			{
				return "post-event";
			}
			return "regular";
		}

		// Token: 0x06006E69 RID: 28265 RVA: 0x0023983F File Offset: 0x00237A3F
		public static string ResolveSearchState(ScheduledEventInfo e, DateTime serverNow, bool joinerSeenRecently)
		{
			if (!e.isActive || ScheduledEventMatchmaking.GracePeriodEnded(e, serverNow))
			{
				return "regular";
			}
			if (!joinerSeenRecently)
			{
				return "regular";
			}
			return "post-event";
		}

		// Token: 0x06006E6A RID: 28266 RVA: 0x00239868 File Offset: 0x00237A68
		public static bool HasSeenScheduledEventRecently(DateTime serverNow)
		{
			string @string = PlayerPrefs.GetString("lastSawScheduledEventTime", null);
			if (string.IsNullOrEmpty(@string))
			{
				return false;
			}
			long num;
			if (!long.TryParse(@string, out num))
			{
				return false;
			}
			DateTime dateTime = new DateTime(num, DateTimeKind.Utc);
			return (serverNow.ToUniversalTime() - dateTime).TotalHours < 12.0;
		}

		// Token: 0x06006E6B RID: 28267 RVA: 0x002398C0 File Offset: 0x00237AC0
		public static void MarkSeenScheduledEventNow(DateTime serverNow)
		{
			PlayerPrefs.SetString("lastSawScheduledEventTime", serverNow.ToUniversalTime().Ticks.ToString());
		}

		// Token: 0x06006E6C RID: 28268 RVA: 0x002398F0 File Offset: 0x00237AF0
		public static void ApplyScheduledEventStateToHashes(Hashtable createProps, out Hashtable searchFilter)
		{
			searchFilter = null;
		}

		// Token: 0x04007E96 RID: 32406
		public const string StateRegular = "regular";

		// Token: 0x04007E97 RID: 32407
		public const string StateInProgress = "event-in-progress";

		// Token: 0x04007E98 RID: 32408
		public const string StatePostEvent = "post-event";
	}
}
