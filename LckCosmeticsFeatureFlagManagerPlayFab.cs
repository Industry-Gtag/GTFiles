using System;
using System.Threading.Tasks;
using GorillaNetworking;
using Liv.Lck;
using PlayFab;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000405 RID: 1029
[Preserve]
public class LckCosmeticsFeatureFlagManagerPlayFab : ILckCosmeticsFeatureFlagManager
{
	// Token: 0x06001868 RID: 6248 RVA: 0x0008AE50 File Offset: 0x00089050
	[Preserve]
	public LckCosmeticsFeatureFlagManagerPlayFab()
	{
	}

	// Token: 0x06001869 RID: 6249 RVA: 0x0008AE64 File Offset: 0x00089064
	public Task<bool> IsEnabledAsync()
	{
		if (this._initializationTask != null)
		{
			return this._initializationTask;
		}
		object @lock = this._lock;
		Task<bool> task2;
		lock (@lock)
		{
			Task<bool> task;
			if ((task = this._initializationTask) == null)
			{
				task2 = (this._initializationTask = this.GetEnabledStateWithRetryAsync());
				task = task2;
			}
			task2 = task;
		}
		return task2;
	}

	// Token: 0x0600186A RID: 6250 RVA: 0x0008AEC8 File Offset: 0x000890C8
	private async Task<bool> GetEnabledStateWithRetryAsync()
	{
		for (int i = 0; i < 2; i++)
		{
			if (!(PlayFabTitleDataCache.Instance == null))
			{
				TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
				PlayFabTitleDataCache.Instance.GetTitleData("EnableLckCosmetics", delegate(string data)
				{
					bool flag;
					if (bool.TryParse(data, out flag))
					{
						Debug.Log(string.Format("LCK: Feature flag '{0}' is set to '{1}'.", "EnableLckCosmetics", flag));
						tcs.TrySetResult(flag);
						return;
					}
					Debug.LogError("LCK: Failed to parse feature flag 'EnableLckCosmetics' from value '" + data + "'. Defaulting to 'true'.");
					tcs.TrySetResult(true);
				}, delegate(PlayFabError error)
				{
					Debug.LogError("LCK: Error fetching feature flag 'EnableLckCosmetics': " + error.ErrorMessage + ". Defaulting to 'true'.");
					tcs.TrySetResult(true);
				}, false);
				return await tcs.Task;
			}
			Debug.LogWarning("LCK: PlayFabTitleDataCache instance is not available. " + string.Format("Retrying feature flag check in {0} seconds... (Attempt {1}/{2})", 5, i + 1, 2));
			await Task.Delay(5000);
		}
		Debug.LogError(string.Format("LCK: {0} instance was not available after {1} attempts. ", "PlayFabTitleDataCache", 2) + "Cosmetics feature will be enabled by default as a fallback measure.");
		return true;
	}

	// Token: 0x0400239B RID: 9115
	private const string TitleDataKey = "EnableLckCosmetics";

	// Token: 0x0400239C RID: 9116
	private const int MaxRetries = 2;

	// Token: 0x0400239D RID: 9117
	private const int RetryDelayMilliseconds = 5000;

	// Token: 0x0400239E RID: 9118
	private Task<bool> _initializationTask;

	// Token: 0x0400239F RID: 9119
	private readonly object _lock = new object();
}
