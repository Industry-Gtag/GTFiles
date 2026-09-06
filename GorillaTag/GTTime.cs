using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020011EA RID: 4586
	public static class GTTime
	{
		// Token: 0x17000B4F RID: 2895
		// (get) Token: 0x06007493 RID: 29843 RVA: 0x0025E49C File Offset: 0x0025C69C
		// (set) Token: 0x06007494 RID: 29844 RVA: 0x0025E4A3 File Offset: 0x0025C6A3
		public static TimeZoneInfo timeZoneInfoLA { get; private set; }

		// Token: 0x06007495 RID: 29845 RVA: 0x0025E4AB File Offset: 0x0025C6AB
		static GTTime()
		{
			GTTime._Init();
		}

		// Token: 0x06007496 RID: 29846 RVA: 0x0025E4B4 File Offset: 0x0025C6B4
		[RuntimeInitializeOnLoadMethod]
		private static void _Init()
		{
			if (GTTime._isInitialized)
			{
				return;
			}
			try
			{
				GTTime.timeZoneInfoLA = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
			}
			catch
			{
				try
				{
					GTTime.timeZoneInfoLA = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
				}
				catch
				{
					TimeZoneInfo timeZoneInfo;
					if (GTTime._TryCreateCustomPST(out timeZoneInfo))
					{
						GTTime.timeZoneInfoLA = timeZoneInfo;
						Debug.Log("[GTTime]  _Init: Could not get US Pacific Time Zone, so using manual created Pacific time zone instead.");
					}
					else
					{
						Debug.LogError("[GTTime]  ERROR!!!  _Init: Could not get US Pacific Time Zone and manual Pacific time zone creation failed. Using UTC instead.");
						GTTime.timeZoneInfoLA = TimeZoneInfo.Utc;
					}
				}
			}
			finally
			{
				GTTime._isInitialized = true;
			}
		}

		// Token: 0x06007497 RID: 29847 RVA: 0x0025E550 File Offset: 0x0025C750
		private static bool _TryCreateCustomPST(out TimeZoneInfo out_tz)
		{
			TimeZoneInfo.AdjustmentRule[] array = new TimeZoneInfo.AdjustmentRule[] { TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2007, 1, 1), DateTime.MaxValue.Date, TimeSpan.FromHours(1.0), TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 3, 2, DayOfWeek.Sunday), TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 11, 1, DayOfWeek.Sunday)) };
			bool flag;
			try
			{
				out_tz = TimeZoneInfo.CreateCustomTimeZone("Custom/America_Los_Angeles", TimeSpan.FromHours(-8.0), "(UTC-08:00) Pacific Time (US & Canada)", "Pacific Standard Time", "Pacific Daylight Time", array, false);
				flag = true;
			}
			catch (Exception ex)
			{
				Debug.LogError("[GTTime]  ERROR!!!  _TryCreateCustomPST: Encountered exception: " + ex.Message);
				out_tz = null;
				flag = false;
			}
			return flag;
		}

		// Token: 0x17000B50 RID: 2896
		// (get) Token: 0x06007498 RID: 29848 RVA: 0x0025E614 File Offset: 0x0025C814
		// (set) Token: 0x06007499 RID: 29849 RVA: 0x0025E61B File Offset: 0x0025C81B
		public static bool usingServerTime { get; private set; }

		// Token: 0x0600749A RID: 29850 RVA: 0x0025E623 File Offset: 0x0025C823
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long GetServerStartupTimeAsMilliseconds()
		{
			return GorillaComputer.instance.startupMillis;
		}

		// Token: 0x0600749B RID: 29851 RVA: 0x0025E634 File Offset: 0x0025C834
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long GetDeviceStartupTimeAsMilliseconds()
		{
			return (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - Time.realtimeSinceStartupAsDouble * 1000.0);
		}

		// Token: 0x0600749C RID: 29852 RVA: 0x0025E66C File Offset: 0x0025C86C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetStartupTimeAsMilliseconds()
		{
			GTTime.usingServerTime = true;
			long num = 0L;
			if (GorillaComputer.hasInstance)
			{
				num = GTTime.GetServerStartupTimeAsMilliseconds();
			}
			if (num == 0L)
			{
				GTTime.usingServerTime = false;
				num = GTTime.GetDeviceStartupTimeAsMilliseconds();
			}
			return num;
		}

		// Token: 0x0600749D RID: 29853 RVA: 0x0025E69F File Offset: 0x0025C89F
		public static long TimeAsMilliseconds()
		{
			return GTTime.GetStartupTimeAsMilliseconds() + (long)(Time.realtimeSinceStartupAsDouble * 1000.0);
		}

		// Token: 0x0600749E RID: 29854 RVA: 0x0025E6B7 File Offset: 0x0025C8B7
		public static double TimeAsDouble()
		{
			return (double)GTTime.GetStartupTimeAsMilliseconds() / 1000.0 + Time.realtimeSinceStartupAsDouble;
		}

		// Token: 0x0600749F RID: 29855 RVA: 0x0025E6CF File Offset: 0x0025C8CF
		public static DateTime GetAAxiomDateTime()
		{
			return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GTTime.timeZoneInfoLA);
		}

		// Token: 0x060074A0 RID: 29856 RVA: 0x0025E6E0 File Offset: 0x0025C8E0
		public static string GetAAxiomDateTimeAsStringForDisplay()
		{
			return GTTime.GetAAxiomDateTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
		}

		// Token: 0x060074A1 RID: 29857 RVA: 0x0025E700 File Offset: 0x0025C900
		public static string GetAAxiomDateTimeAsStringForFilename()
		{
			return GTTime.GetAAxiomDateTime().ToString("yyyy-MM-dd_HH-mm-ss-fff");
		}

		// Token: 0x060074A2 RID: 29858 RVA: 0x0025E720 File Offset: 0x0025C920
		public static long GetAAxiomDateTimeAsHumanReadableLong()
		{
			return long.Parse(GTTime.GetAAxiomDateTime().ToString("yyyyMMddHHmmssfff00"));
		}

		// Token: 0x060074A3 RID: 29859 RVA: 0x0025E744 File Offset: 0x0025C944
		public static DateTime ConvertDateTimeHumanReadableLongToDateTime(long humanReadableLong)
		{
			return DateTime.ParseExact(humanReadableLong.ToString(), "yyyyMMddHHmmssfff'00'", CultureInfo.InvariantCulture);
		}

		// Token: 0x060074A4 RID: 29860 RVA: 0x0025E75C File Offset: 0x0025C95C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryUpdateTimeText(TMP_Text textComponent, TimeSpan timeSpan, char[] chars, int index, ref int ref_lastUpdateSeconds)
		{
			int num = (int)timeSpan.TotalSeconds;
			if (ref_lastUpdateSeconds == num)
			{
				return false;
			}
			ref_lastUpdateSeconds = num;
			int num2 = Mathf.Clamp((int)timeSpan.TotalHours, 0, 99);
			int minutes = timeSpan.Minutes;
			int seconds = timeSpan.Seconds;
			chars[index] = (char)(48 + num2 / 10);
			chars[index + 1] = (char)(48 + num2 % 10);
			chars[index + 3] = (char)(48 + minutes / 10);
			chars[index + 4] = (char)(48 + minutes % 10);
			chars[index + 6] = (char)(48 + seconds / 10);
			chars[index + 7] = (char)(48 + seconds % 10);
			textComponent.SetCharArray(chars);
			return true;
		}

		// Token: 0x0400845E RID: 33886
		private const string preLog = "[GTTime]  ";

		// Token: 0x0400845F RID: 33887
		private const string preErr = "[GTTime]  ERROR!!!  ";

		// Token: 0x04008460 RID: 33888
		private static bool _isInitialized;
	}
}
