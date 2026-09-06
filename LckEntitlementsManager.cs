using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck;
using Liv.Lck.Core.Cosmetics;
using Liv.Lck.DependencyInjection;
using Photon.Pun;
using UnityEngine;

// Token: 0x020003F8 RID: 1016
public class LckEntitlementsManager : MonoBehaviour
{
	// Token: 0x1700025F RID: 607
	// (get) Token: 0x0600182F RID: 6191 RVA: 0x0008A1B8 File Offset: 0x000883B8
	// (set) Token: 0x06001830 RID: 6192 RVA: 0x0008A1BF File Offset: 0x000883BF
	public static bool LckEntitlementsEnabled { get; private set; }

	// Token: 0x17000260 RID: 608
	// (get) Token: 0x06001831 RID: 6193 RVA: 0x0008A1C7 File Offset: 0x000883C7
	// (set) Token: 0x06001832 RID: 6194 RVA: 0x0008A1CE File Offset: 0x000883CE
	public static LckEntitlementsManager Instance { get; private set; }

	// Token: 0x06001833 RID: 6195 RVA: 0x0008A1D6 File Offset: 0x000883D6
	private void Awake()
	{
		if (LckEntitlementsManager.Instance != null && LckEntitlementsManager.Instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		LckEntitlementsManager.Instance = this;
	}

	// Token: 0x06001834 RID: 6196 RVA: 0x0008A204 File Offset: 0x00088404
	private void OnEnable()
	{
		this.InitializeFeatureAsync();
		this._cleanupProcessedPlayersCoroutine = base.StartCoroutine(this.CleanupProcessedPlayersCoroutine());
		this._getEntitlementsBatchingCoroutine = base.StartCoroutine(this.ProcessBatchedRemotePlayersCoroutine());
	}

	// Token: 0x06001835 RID: 6197 RVA: 0x0008A231 File Offset: 0x00088431
	private void OnDisable()
	{
		if (this._cleanupProcessedPlayersCoroutine != null)
		{
			base.StopCoroutine(this._cleanupProcessedPlayersCoroutine);
			this._cleanupProcessedPlayersCoroutine = null;
		}
		if (this._getEntitlementsBatchingCoroutine != null)
		{
			base.StopCoroutine(this._getEntitlementsBatchingCoroutine);
			this._getEntitlementsBatchingCoroutine = null;
		}
	}

	// Token: 0x06001836 RID: 6198 RVA: 0x0008A26C File Offset: 0x0008846C
	private async Task InitializeFeatureAsync()
	{
		this._currentState = LckEntitlementsManager.FeatureState.Checking;
		bool flag = await this._featureFlagManager.IsEnabledAsync();
		if (!(this == null))
		{
			this._currentState = (flag ? LckEntitlementsManager.FeatureState.Enabled : LckEntitlementsManager.FeatureState.Disabled);
			LckEntitlementsManager.LckEntitlementsEnabled = flag;
		}
	}

	// Token: 0x06001837 RID: 6199 RVA: 0x0008A2AF File Offset: 0x000884AF
	public void OnLocalPlayerSpawned(string localUserId)
	{
		if (!this.ShouldProcessPlayer(localUserId))
		{
			return;
		}
		base.StartCoroutine(this.ProcessLocalPlayerSpawn(localUserId));
	}

	// Token: 0x06001838 RID: 6200 RVA: 0x0008A2CC File Offset: 0x000884CC
	public void OnRemotePlayerSpawned(string remoteUserId)
	{
		if (this._currentState == LckEntitlementsManager.FeatureState.Disabled)
		{
			return;
		}
		if (!this.ShouldProcessPlayer(remoteUserId))
		{
			return;
		}
		HashSet<string> remotePlayersToGetEntitlementsFor = this._remotePlayersToGetEntitlementsFor;
		lock (remotePlayersToGetEntitlementsFor)
		{
			this._remotePlayersToGetEntitlementsFor.Add(remoteUserId);
		}
	}

	// Token: 0x06001839 RID: 6201 RVA: 0x0008A328 File Offset: 0x00088528
	private IEnumerator ProcessLocalPlayerSpawn(string userId)
	{
		yield return new WaitUntil(() => this._currentState > LckEntitlementsManager.FeatureState.Checking);
		if (this._currentState == LckEntitlementsManager.FeatureState.Disabled)
		{
			yield break;
		}
		base.StartCoroutine(this.AnnouncePlayerPresenceForSession(userId));
		yield break;
	}

	// Token: 0x0600183A RID: 6202 RVA: 0x0008A340 File Offset: 0x00088540
	private bool ShouldProcessPlayer(string userId)
	{
		LckEntitlementsManager.PlayerProcessRecord playerProcessRecord;
		if (!this._processedPlayers.TryGetValue(userId, out playerProcessRecord))
		{
			playerProcessRecord = new LckEntitlementsManager.PlayerProcessRecord();
			this._processedPlayers[userId] = playerProcessRecord;
		}
		playerProcessRecord.LastSeenTimestamp = Time.time;
		if (Time.time < playerProcessRecord.TimeoutUntilTimestamp)
		{
			return false;
		}
		if (playerProcessRecord.AttemptCount > 3)
		{
			playerProcessRecord.AttemptCount = 0;
		}
		playerProcessRecord.AttemptCount++;
		if (playerProcessRecord.AttemptCount > 3)
		{
			playerProcessRecord.TimeoutUntilTimestamp = Time.time + 60f;
			return false;
		}
		return true;
	}

	// Token: 0x0600183B RID: 6203 RVA: 0x0008A3C5 File Offset: 0x000885C5
	private IEnumerator ProcessBatchedRemotePlayersCoroutine()
	{
		for (;;)
		{
			yield return new WaitForSeconds(15f);
			if (!this._isProcessingBatch)
			{
				HashSet<string> remotePlayersToGetEntitlementsFor = this._remotePlayersToGetEntitlementsFor;
				List<string> list;
				lock (remotePlayersToGetEntitlementsFor)
				{
					if (this._remotePlayersToGetEntitlementsFor.Count == 0)
					{
						continue;
					}
					list = this._remotePlayersToGetEntitlementsFor.ToList<string>();
					this._remotePlayersToGetEntitlementsFor.Clear();
				}
				if (list.Count > 0)
				{
					this._isProcessingBatch = true;
					this.GetCosmeticsForPlayersAsync(list, "ProcessBatchedRemotePlayers");
				}
			}
		}
		yield break;
	}

	// Token: 0x0600183C RID: 6204 RVA: 0x0008A3D4 File Offset: 0x000885D4
	private IEnumerator AnnouncePlayerPresenceForSession(string localPlayerId)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			Debug.LogError("LCK: Called AnnouncePlayerPresenceForSession() but no room was found. Player not announced.");
			yield break;
		}
		string sessionId = "DefaultSessionId";
		int num;
		for (int attempt = 1; attempt <= 2; attempt = num + 1)
		{
			LckEntitlementsManager.<>c__DisplayClass33_0 CS$<>8__locals1 = new LckEntitlementsManager.<>c__DisplayClass33_0();
			CS$<>8__locals1.announcementAsync = this._lckCosmeticsCoordinator.AnnouncePlayerPresenceForSessionAsync(localPlayerId, sessionId);
			yield return new WaitUntil(() => CS$<>8__locals1.announcementAsync.IsCompleted);
			if (!CS$<>8__locals1.announcementAsync.IsFaulted && CS$<>8__locals1.announcementAsync.Result.IsOk)
			{
				yield break;
			}
			string text = (CS$<>8__locals1.announcementAsync.IsFaulted ? CS$<>8__locals1.announcementAsync.Exception.ToString() : CS$<>8__locals1.announcementAsync.Result.Message.ToString());
			Debug.LogError(string.Format("LCK: Error setting session entitlement (Attempt {0}/{1}): {2}", attempt, 2, text));
			CS$<>8__locals1 = null;
			num = attempt;
		}
		Debug.LogError("LCK: All attempts to set session entitlement failed.");
		yield break;
	}

	// Token: 0x0600183D RID: 6205 RVA: 0x0008A3EC File Offset: 0x000885EC
	private async Task GetCosmeticsForPlayersAsync(List<string> userIdList, string methodNameForLogging)
	{
		LckEntitlementsManager.<>c__DisplayClass34_0 CS$<>8__locals1 = new LckEntitlementsManager.<>c__DisplayClass34_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.userIdList = userIdList;
		CS$<>8__locals1.methodNameForLogging = methodNameForLogging;
		try
		{
			if (CS$<>8__locals1.userIdList != null && CS$<>8__locals1.userIdList.Count != 0)
			{
				if (PhotonNetwork.CurrentRoom == null)
				{
					Debug.LogError("LCK: Called " + CS$<>8__locals1.methodNameForLogging + " but no room was found.");
				}
				else
				{
					CS$<>8__locals1.sessionId = "DefaultSessionId";
					await Task.Run(delegate
					{
						LckEntitlementsManager.<>c__DisplayClass34_0.<<GetCosmeticsForPlayersAsync>b__0>d <<GetCosmeticsForPlayersAsync>b__0>d;
						<<GetCosmeticsForPlayersAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<GetCosmeticsForPlayersAsync>b__0>d.<>4__this = CS$<>8__locals1;
						<<GetCosmeticsForPlayersAsync>b__0>d.<>1__state = -1;
						<<GetCosmeticsForPlayersAsync>b__0>d.<>t__builder.Start<LckEntitlementsManager.<>c__DisplayClass34_0.<<GetCosmeticsForPlayersAsync>b__0>d>(ref <<GetCosmeticsForPlayersAsync>b__0>d);
						return <<GetCosmeticsForPlayersAsync>b__0>d.<>t__builder.Task;
					});
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("LCK: An exception occurred in GetCosmeticsForPlayersAsync: {0}", ex));
		}
		finally
		{
			this._isProcessingBatch = false;
		}
	}

	// Token: 0x0600183E RID: 6206 RVA: 0x0008A43F File Offset: 0x0008863F
	private IEnumerator CleanupProcessedPlayersCoroutine()
	{
		List<string> playersToRemove = new List<string>();
		for (;;)
		{
			yield return new WaitForSeconds(60f);
			playersToRemove.Clear();
			float time = Time.time;
			foreach (KeyValuePair<string, LckEntitlementsManager.PlayerProcessRecord> keyValuePair in this._processedPlayers)
			{
				if (time > keyValuePair.Value.LastSeenTimestamp + 300f)
				{
					playersToRemove.Add(keyValuePair.Key);
				}
			}
			if (playersToRemove.Count > 0)
			{
				using (List<string>.Enumerator enumerator2 = playersToRemove.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						string text = enumerator2.Current;
						this._processedPlayers.Remove(text);
					}
					continue;
				}
				yield break;
			}
		}
	}

	// Token: 0x0400235D RID: 9053
	[InjectLck]
	private ILckCosmeticsCoordinator _lckCosmeticsCoordinator;

	// Token: 0x0400235E RID: 9054
	[InjectLck]
	private ILckCosmeticsFeatureFlagManager _featureFlagManager;

	// Token: 0x04002360 RID: 9056
	private const int MAX_API_CALL_ATTEMPTS = 2;

	// Token: 0x04002361 RID: 9057
	private const int MAX_CONSECUTIVE_ATTEMPTS = 3;

	// Token: 0x04002362 RID: 9058
	private const float ABUSE_TIMEOUT_MINUTES = 1f;

	// Token: 0x04002363 RID: 9059
	private const float BATCH_GET_ENTITLEMENTS_INTERVAL_SECONDS = 15f;

	// Token: 0x04002364 RID: 9060
	private const float STALE_PLAYER_TIMEOUT_MINUTES = 5f;

	// Token: 0x04002365 RID: 9061
	private const string DEFAULT_SESSION_ID = "DefaultSessionId";

	// Token: 0x04002366 RID: 9062
	private LckEntitlementsManager.FeatureState _currentState;

	// Token: 0x04002367 RID: 9063
	private readonly HashSet<string> _remotePlayersToGetEntitlementsFor = new HashSet<string>();

	// Token: 0x04002368 RID: 9064
	private Coroutine _getEntitlementsBatchingCoroutine;

	// Token: 0x04002369 RID: 9065
	private readonly Dictionary<string, LckEntitlementsManager.PlayerProcessRecord> _processedPlayers = new Dictionary<string, LckEntitlementsManager.PlayerProcessRecord>();

	// Token: 0x0400236A RID: 9066
	private Coroutine _cleanupProcessedPlayersCoroutine;

	// Token: 0x0400236B RID: 9067
	private bool _isProcessingBatch;

	// Token: 0x020003F9 RID: 1017
	private class PlayerProcessRecord
	{
		// Token: 0x0400236D RID: 9069
		public int AttemptCount;

		// Token: 0x0400236E RID: 9070
		public float TimeoutUntilTimestamp;

		// Token: 0x0400236F RID: 9071
		public float LastSeenTimestamp;
	}

	// Token: 0x020003FA RID: 1018
	private enum FeatureState
	{
		// Token: 0x04002371 RID: 9073
		Checking,
		// Token: 0x04002372 RID: 9074
		Enabled,
		// Token: 0x04002373 RID: 9075
		Disabled
	}
}
