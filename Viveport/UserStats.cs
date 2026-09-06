using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000EB0 RID: 3760
	public class UserStats
	{
		// Token: 0x06005B3A RID: 23354 RVA: 0x001D9E1C File Offset: 0x001D801C
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			UserStats.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06005B3B RID: 23355 RVA: 0x001D9E2C File Offset: 0x001D802C
		public static int IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.IsReady_64(new StatusCallback(UserStats.IsReadyIl2cppCallback));
			}
			return UserStats.IsReady(new StatusCallback(UserStats.IsReadyIl2cppCallback));
		}

		// Token: 0x06005B3C RID: 23356 RVA: 0x001D9E99 File Offset: 0x001D8099
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadStatsIl2cppCallback(int errorCode)
		{
			UserStats.downloadStatsIl2cppCallback(errorCode);
		}

		// Token: 0x06005B3D RID: 23357 RVA: 0x001D9EA8 File Offset: 0x001D80A8
		public static int DownloadStats(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.downloadStatsIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.DownloadStats_64(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
			}
			return UserStats.DownloadStats(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
		}

		// Token: 0x06005B3E RID: 23358 RVA: 0x001D9F18 File Offset: 0x001D8118
		public static int GetStat(string name, int defaultValue)
		{
			int num = defaultValue;
			if (IntPtr.Size == 8)
			{
				UserStats.GetStat_64(name, ref num);
			}
			else
			{
				UserStats.GetStat(name, ref num);
			}
			return num;
		}

		// Token: 0x06005B3F RID: 23359 RVA: 0x001D9F44 File Offset: 0x001D8144
		public static float GetStat(string name, float defaultValue)
		{
			float num = defaultValue;
			if (IntPtr.Size == 8)
			{
				UserStats.GetStat_64(name, ref num);
			}
			else
			{
				UserStats.GetStat(name, ref num);
			}
			return num;
		}

		// Token: 0x06005B40 RID: 23360 RVA: 0x001D9F70 File Offset: 0x001D8170
		public static void SetStat(string name, int value)
		{
			if (IntPtr.Size == 8)
			{
				UserStats.SetStat_64(name, value);
				return;
			}
			UserStats.SetStat(name, value);
		}

		// Token: 0x06005B41 RID: 23361 RVA: 0x001D9F8B File Offset: 0x001D818B
		public static void SetStat(string name, float value)
		{
			if (IntPtr.Size == 8)
			{
				UserStats.SetStat_64(name, value);
				return;
			}
			UserStats.SetStat(name, value);
		}

		// Token: 0x06005B42 RID: 23362 RVA: 0x001D9FA6 File Offset: 0x001D81A6
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadStatsIl2cppCallback(int errorCode)
		{
			UserStats.uploadStatsIl2cppCallback(errorCode);
		}

		// Token: 0x06005B43 RID: 23363 RVA: 0x001D9FB4 File Offset: 0x001D81B4
		public static int UploadStats(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.uploadStatsIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.UploadStats_64(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
			}
			return UserStats.UploadStats(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
		}

		// Token: 0x06005B44 RID: 23364 RVA: 0x001DA024 File Offset: 0x001D8224
		public static bool GetAchievement(string pchName)
		{
			int num = 0;
			if (IntPtr.Size == 8)
			{
				UserStats.GetAchievement_64(pchName, ref num);
			}
			else
			{
				UserStats.GetAchievement(pchName, ref num);
			}
			return num == 1;
		}

		// Token: 0x06005B45 RID: 23365 RVA: 0x001DA054 File Offset: 0x001D8254
		public static int GetAchievementUnlockTime(string pchName)
		{
			int num = 0;
			if (IntPtr.Size == 8)
			{
				UserStats.GetAchievementUnlockTime_64(pchName, ref num);
			}
			else
			{
				UserStats.GetAchievementUnlockTime(pchName, ref num);
			}
			return num;
		}

		// Token: 0x06005B46 RID: 23366 RVA: 0x00092236 File Offset: 0x00090436
		public static string GetAchievementIcon(string pchName)
		{
			return "";
		}

		// Token: 0x06005B47 RID: 23367 RVA: 0x00092236 File Offset: 0x00090436
		public static string GetAchievementDisplayAttribute(string pchName, UserStats.AchievementDisplayAttribute attr)
		{
			return "";
		}

		// Token: 0x06005B48 RID: 23368 RVA: 0x00092236 File Offset: 0x00090436
		public static string GetAchievementDisplayAttribute(string pchName, UserStats.AchievementDisplayAttribute attr, Locale locale)
		{
			return "";
		}

		// Token: 0x06005B49 RID: 23369 RVA: 0x001DA080 File Offset: 0x001D8280
		public static int SetAchievement(string pchName)
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.SetAchievement_64(pchName);
			}
			return UserStats.SetAchievement(pchName);
		}

		// Token: 0x06005B4A RID: 23370 RVA: 0x001DA097 File Offset: 0x001D8297
		public static int ClearAchievement(string pchName)
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.ClearAchievement_64(pchName);
			}
			return UserStats.ClearAchievement(pchName);
		}

		// Token: 0x06005B4B RID: 23371 RVA: 0x001DA0AE File Offset: 0x001D82AE
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadLeaderboardScoresIl2cppCallback(int errorCode)
		{
			UserStats.downloadLeaderboardScoresIl2cppCallback(errorCode);
		}

		// Token: 0x06005B4C RID: 23372 RVA: 0x001DA0BC File Offset: 0x001D82BC
		public static int DownloadLeaderboardScores(StatusCallback callback, string pchLeaderboardName, UserStats.LeaderBoardRequestType eLeaderboardDataRequest, UserStats.LeaderBoardTimeRange eLeaderboardDataTimeRange, int nRangeStart, int nRangeEnd)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.downloadLeaderboardScoresIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.DownloadLeaderboardScores_64(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataRequest)eLeaderboardDataRequest, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nRangeStart, nRangeEnd);
			}
			return UserStats.DownloadLeaderboardScores(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataRequest)eLeaderboardDataRequest, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nRangeStart, nRangeEnd);
		}

		// Token: 0x06005B4D RID: 23373 RVA: 0x001DA137 File Offset: 0x001D8337
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadLeaderboardScoreIl2cppCallback(int errorCode)
		{
			UserStats.uploadLeaderboardScoreIl2cppCallback(errorCode);
		}

		// Token: 0x06005B4E RID: 23374 RVA: 0x001DA144 File Offset: 0x001D8344
		public static int UploadLeaderboardScore(StatusCallback callback, string pchLeaderboardName, int nScore)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.uploadLeaderboardScoreIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.UploadLeaderboardScore_64(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, nScore);
			}
			return UserStats.UploadLeaderboardScore(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, nScore);
		}

		// Token: 0x06005B4F RID: 23375 RVA: 0x001DA1B8 File Offset: 0x001D83B8
		public static Leaderboard GetLeaderboardScore(int index)
		{
			LeaderboardEntry_t leaderboardEntry_t;
			leaderboardEntry_t.m_nGlobalRank = 0;
			leaderboardEntry_t.m_nScore = 0;
			leaderboardEntry_t.m_pUserName = "";
			if (IntPtr.Size == 8)
			{
				UserStats.GetLeaderboardScore_64(index, ref leaderboardEntry_t);
			}
			else
			{
				UserStats.GetLeaderboardScore(index, ref leaderboardEntry_t);
			}
			return new Leaderboard
			{
				Rank = leaderboardEntry_t.m_nGlobalRank,
				Score = leaderboardEntry_t.m_nScore,
				UserName = leaderboardEntry_t.m_pUserName
			};
		}

		// Token: 0x06005B50 RID: 23376 RVA: 0x001DA226 File Offset: 0x001D8426
		public static int GetLeaderboardScoreCount()
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.GetLeaderboardScoreCount_64();
			}
			return UserStats.GetLeaderboardScoreCount();
		}

		// Token: 0x06005B51 RID: 23377 RVA: 0x001DA23B File Offset: 0x001D843B
		public static UserStats.LeaderBoardSortMethod GetLeaderboardSortMethod()
		{
			if (IntPtr.Size == 8)
			{
				return (UserStats.LeaderBoardSortMethod)UserStats.GetLeaderboardSortMethod_64();
			}
			return (UserStats.LeaderBoardSortMethod)UserStats.GetLeaderboardSortMethod();
		}

		// Token: 0x06005B52 RID: 23378 RVA: 0x001DA250 File Offset: 0x001D8450
		public static UserStats.LeaderBoardDiaplayType GetLeaderboardDisplayType()
		{
			if (IntPtr.Size == 8)
			{
				return (UserStats.LeaderBoardDiaplayType)UserStats.GetLeaderboardDisplayType_64();
			}
			return (UserStats.LeaderBoardDiaplayType)UserStats.GetLeaderboardDisplayType();
		}

		// Token: 0x04006C12 RID: 27666
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x04006C13 RID: 27667
		private static StatusCallback downloadStatsIl2cppCallback;

		// Token: 0x04006C14 RID: 27668
		private static StatusCallback uploadStatsIl2cppCallback;

		// Token: 0x04006C15 RID: 27669
		private static StatusCallback downloadLeaderboardScoresIl2cppCallback;

		// Token: 0x04006C16 RID: 27670
		private static StatusCallback uploadLeaderboardScoreIl2cppCallback;

		// Token: 0x02000EB1 RID: 3761
		public enum LeaderBoardRequestType
		{
			// Token: 0x04006C18 RID: 27672
			GlobalData,
			// Token: 0x04006C19 RID: 27673
			GlobalDataAroundUser,
			// Token: 0x04006C1A RID: 27674
			LocalData,
			// Token: 0x04006C1B RID: 27675
			LocalDataAroundUser
		}

		// Token: 0x02000EB2 RID: 3762
		public enum LeaderBoardTimeRange
		{
			// Token: 0x04006C1D RID: 27677
			AllTime,
			// Token: 0x04006C1E RID: 27678
			Daily,
			// Token: 0x04006C1F RID: 27679
			Weekly,
			// Token: 0x04006C20 RID: 27680
			Monthly
		}

		// Token: 0x02000EB3 RID: 3763
		public enum LeaderBoardSortMethod
		{
			// Token: 0x04006C22 RID: 27682
			None,
			// Token: 0x04006C23 RID: 27683
			Ascending,
			// Token: 0x04006C24 RID: 27684
			Descending
		}

		// Token: 0x02000EB4 RID: 3764
		public enum LeaderBoardDiaplayType
		{
			// Token: 0x04006C26 RID: 27686
			None,
			// Token: 0x04006C27 RID: 27687
			Numeric,
			// Token: 0x04006C28 RID: 27688
			TimeSeconds,
			// Token: 0x04006C29 RID: 27689
			TimeMilliSeconds
		}

		// Token: 0x02000EB5 RID: 3765
		public enum LeaderBoardScoreMethod
		{
			// Token: 0x04006C2B RID: 27691
			None,
			// Token: 0x04006C2C RID: 27692
			KeepBest,
			// Token: 0x04006C2D RID: 27693
			ForceUpdate
		}

		// Token: 0x02000EB6 RID: 3766
		public enum AchievementDisplayAttribute
		{
			// Token: 0x04006C2F RID: 27695
			Name,
			// Token: 0x04006C30 RID: 27696
			Desc,
			// Token: 0x04006C31 RID: 27697
			Hidden
		}
	}
}
