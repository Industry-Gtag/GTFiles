using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000256 RID: 598
public class ProgressionController : MonoBehaviour
{
	// Token: 0x1400002F RID: 47
	// (add) Token: 0x06001018 RID: 4120 RVA: 0x00056B84 File Offset: 0x00054D84
	// (remove) Token: 0x06001019 RID: 4121 RVA: 0x00056BB8 File Offset: 0x00054DB8
	public static event Action OnQuestSelectionChanged;

	// Token: 0x14000030 RID: 48
	// (add) Token: 0x0600101A RID: 4122 RVA: 0x00056BEC File Offset: 0x00054DEC
	// (remove) Token: 0x0600101B RID: 4123 RVA: 0x00056C20 File Offset: 0x00054E20
	public static event Action OnProgressEvent;

	// Token: 0x17000194 RID: 404
	// (get) Token: 0x0600101C RID: 4124 RVA: 0x00056C53 File Offset: 0x00054E53
	// (set) Token: 0x0600101D RID: 4125 RVA: 0x00056C5A File Offset: 0x00054E5A
	public static int WeeklyCap { get; private set; } = 25;

	// Token: 0x17000195 RID: 405
	// (get) Token: 0x0600101E RID: 4126 RVA: 0x00056C62 File Offset: 0x00054E62
	public static int TotalPoints
	{
		get
		{
			return ProgressionController._gInstance.totalPointsRaw - ProgressionController._gInstance.unclaimedPoints;
		}
	}

	// Token: 0x0600101F RID: 4127 RVA: 0x00056C79 File Offset: 0x00054E79
	public static void ReportQuestChanged(bool initialLoad)
	{
		ProgressionController._gInstance.OnQuestProgressChanged(initialLoad);
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x00056C86 File Offset: 0x00054E86
	public static void ReportQuestSelectionChanged()
	{
		ProgressionController._gInstance.LoadCompletedQuestQueue();
		Action onQuestSelectionChanged = ProgressionController.OnQuestSelectionChanged;
		if (onQuestSelectionChanged == null)
		{
			return;
		}
		onQuestSelectionChanged();
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x00056CA1 File Offset: 0x00054EA1
	public static void ReportQuestComplete(int questId, bool isDaily)
	{
		ProgressionController._gInstance.OnQuestComplete(questId, isDaily);
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x00056CAF File Offset: 0x00054EAF
	public static void RedeemProgress()
	{
		ProgressionController._gInstance.RequestProgressRedemption(new Action(ProgressionController._gInstance.OnProgressRedeemed));
	}

	// Token: 0x06001023 RID: 4131 RVA: 0x00056CCB File Offset: 0x00054ECB
	[return: TupleElementNames(new string[] { "weekly", "unclaimed", "total" })]
	public static ValueTuple<int, int, int> GetProgressionData()
	{
		return ProgressionController._gInstance.GetProgress();
	}

	// Token: 0x06001024 RID: 4132 RVA: 0x00056CD7 File Offset: 0x00054ED7
	public static void RequestProgressUpdate()
	{
		ProgressionController gInstance = ProgressionController._gInstance;
		if (gInstance == null)
		{
			return;
		}
		gInstance.ReportProgress();
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x00056CE8 File Offset: 0x00054EE8
	private void Awake()
	{
		if (ProgressionController._gInstance)
		{
			Debug.LogError("Duplicate ProgressionController detected. Destroying self.", base.gameObject);
			Object.Destroy(this);
			return;
		}
		ProgressionController._gInstance = this;
		this.unclaimedPoints = PlayerPrefs.GetInt("Claimed_Points_Key", 0);
		this.RequestStatus();
		this.LoadCompletedQuestQueue();
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x00056D3C File Offset: 0x00054F3C
	private async void RequestStatus()
	{
		if (this.<RequestStatus>g__ShouldFetchStatus|36_0())
		{
			this._isFetchingStatus = true;
			await this.WaitForSessionToken();
			this.FetchStatus();
		}
		else
		{
			Debug.LogError("RequestStatus triggered multiple times.  That's probably not good.");
		}
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x00056D74 File Offset: 0x00054F74
	private async Task WaitForSessionToken()
	{
		while (!PlayFabAuthenticator.instance || PlayFabAuthenticator.instance.GetPlayFabPlayerId().IsNullOrEmpty() || PlayFabAuthenticator.instance.GetPlayFabSessionTicket().IsNullOrEmpty())
		{
			await Task.Yield();
			await Task.Delay(1000);
		}
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x00056DB0 File Offset: 0x00054FB0
	private void FetchStatus()
	{
		base.StartCoroutine(this.DoFetchStatus(new ProgressionController.GetQuestsStatusRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipId = "",
			MothershipToken = ""
		}, new Action<ProgressionController.GetQuestStatusResponse>(this.OnFetchStatusResponse)));
	}

	// Token: 0x06001029 RID: 4137 RVA: 0x00056E15 File Offset: 0x00055015
	private IEnumerator DoFetchStatus(ProgressionController.GetQuestsStatusRequest data, Action<ProgressionController.GetQuestStatusResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl + "/api/GetQuestStatus", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionController.GetQuestStatusResponse getQuestStatusResponse = JsonConvert.DeserializeObject<ProgressionController.GetQuestStatusResponse>(request.downloadHandler.text);
			callback(getQuestStatusResponse);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (this._fetchStatusRetryCount < this._maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this._fetchStatusRetryCount + 1));
				this._fetchStatusRetryCount++;
				yield return new WaitForSecondsRealtime((float)num);
				this.FetchStatus();
			}
			else
			{
				GTDev.LogError<string>("Maximum FetchStatus retries attempted. Please check your network connection.", null);
				this._fetchStatusRetryCount = 0;
				callback(null);
			}
		}
		yield break;
	}

	// Token: 0x0600102A RID: 4138 RVA: 0x00056E34 File Offset: 0x00055034
	private void OnFetchStatusResponse([CanBeNull] ProgressionController.GetQuestStatusResponse response)
	{
		this._isFetchingStatus = false;
		this._statusReceived = false;
		if (response != null)
		{
			this.SetProgressionValues(response.result.GetWeeklyPoints(), this.unclaimedPoints, response.result.userPointsTotal);
			this.ReportProgress();
			return;
		}
		GTDev.LogError<string>("Error: Could not fetch status!", null);
	}

	// Token: 0x0600102B RID: 4139 RVA: 0x00056E86 File Offset: 0x00055086
	private void SendQuestCompleted(int questId)
	{
		if (this._isSendingQuestComplete)
		{
			return;
		}
		this._isSendingQuestComplete = true;
		this.StartSendQuestComplete(questId);
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x00056EA0 File Offset: 0x000550A0
	private void StartSendQuestComplete(int questId)
	{
		base.StartCoroutine(this.DoSendQuestComplete(new ProgressionController.SetQuestCompleteRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipId = "",
			MothershipToken = "",
			QuestId = questId,
			ClientVersion = MothershipClientApiUnity.DeploymentId
		}, new Action<ProgressionController.SetQuestCompleteResponse>(this.OnSendQuestCompleteSuccess)));
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x00056F17 File Offset: 0x00055117
	private IEnumerator DoSendQuestComplete(ProgressionController.SetQuestCompleteRequest data, Action<ProgressionController.SetQuestCompleteResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl + "/api/SetQuestComplete", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionController.SetQuestCompleteResponse setQuestCompleteResponse = JsonConvert.DeserializeObject<ProgressionController.SetQuestCompleteResponse>(request.downloadHandler.text);
			callback(setQuestCompleteResponse);
			this.ProcessQuestSubmittedSuccess();
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.responseCode == 403L)
			{
				GTDev.LogWarning<string>("User already reached the max number of completion points for this time period!", null);
				callback(null);
				this.ClearQuestQueue();
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (this._sendQuestCompleteRetryCount < this._maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this._sendQuestCompleteRetryCount + 1));
				this._sendQuestCompleteRetryCount++;
				yield return new WaitForSecondsRealtime((float)num);
				this.StartSendQuestComplete(data.QuestId);
			}
			else
			{
				GTDev.LogError<string>("Maximum SendQuestComplete retries attempted. Please check your network connection.", null);
				this._sendQuestCompleteRetryCount = 0;
				callback(null);
				this.ProcessQuestSubmittedFail();
			}
		}
		else
		{
			this._isSendingQuestComplete = false;
		}
		yield break;
	}

	// Token: 0x0600102E RID: 4142 RVA: 0x00056F34 File Offset: 0x00055134
	private void OnSendQuestCompleteSuccess([CanBeNull] ProgressionController.SetQuestCompleteResponse response)
	{
		this._isSendingQuestComplete = false;
		if (response != null)
		{
			this.UpdateProgressionValues(response.result.GetWeeklyPoints(), response.result.userPointsTotal);
			this.ReportProgress();
		}
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x00056F62 File Offset: 0x00055162
	private void OnQuestProgressChanged(bool initialLoad)
	{
		this.ReportProgress();
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x00056F6A File Offset: 0x0005516A
	private void OnQuestComplete(int questId, bool isDaily)
	{
		this.QueueQuestCompletion(questId, isDaily);
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x00056F74 File Offset: 0x00055174
	private void QueueQuestCompletion(int questId, bool isDaily)
	{
		if (isDaily)
		{
			this._queuedDailyCompletedQuests.Add(questId);
		}
		else
		{
			this._queuedWeeklyCompletedQuests.Add(questId);
		}
		this.SaveCompletedQuestQueue();
		this.SubmitNextQuestInQueue();
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x00056FA0 File Offset: 0x000551A0
	private void SubmitNextQuestInQueue()
	{
		if (this._currentlyProcessingQuest == -1 && this.AreCompletedQuestsQueued())
		{
			int num = -1;
			if (this._queuedWeeklyCompletedQuests.Count > 0)
			{
				num = this._queuedWeeklyCompletedQuests[0];
			}
			else if (this._queuedDailyCompletedQuests.Count > 0)
			{
				num = this._queuedDailyCompletedQuests[0];
			}
			this._currentlyProcessingQuest = num;
			this.SendQuestCompleted(num);
		}
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x00057006 File Offset: 0x00055206
	private void ClearQuestQueue()
	{
		this._currentlyProcessingQuest = -1;
		this._queuedDailyCompletedQuests.Clear();
		this._queuedWeeklyCompletedQuests.Clear();
		this.SaveCompletedQuestQueue();
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x0005702C File Offset: 0x0005522C
	private void ProcessQuestSubmittedSuccess()
	{
		if (this._currentlyProcessingQuest != -1)
		{
			if (this.AreCompletedQuestsQueued())
			{
				if (this._queuedWeeklyCompletedQuests.Remove(this._currentlyProcessingQuest))
				{
					this.SaveCompletedQuestQueue();
				}
				else if (this._queuedDailyCompletedQuests.Remove(this._currentlyProcessingQuest))
				{
					this.SaveCompletedQuestQueue();
				}
			}
			this._currentlyProcessingQuest = -1;
			this.SubmitNextQuestInQueue();
		}
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x0005708B File Offset: 0x0005528B
	private void ProcessQuestSubmittedFail()
	{
		this._currentlyProcessingQuest = -1;
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x00057094 File Offset: 0x00055294
	private bool AreCompletedQuestsQueued()
	{
		return this._queuedDailyCompletedQuests.Count > 0 || this._queuedWeeklyCompletedQuests.Count > 0;
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x000570B4 File Offset: 0x000552B4
	private void SaveCompletedQuestQueue()
	{
		int num = 0;
		for (int i = 0; i < this._queuedDailyCompletedQuests.Count; i++)
		{
			PlayerPrefs.SetInt(string.Format("{0}{1}", "Queued_Quest_Daily_ID_Key", num), this._queuedDailyCompletedQuests[i]);
			num++;
		}
		int dailyQuestSetID = this._questManager.dailyQuestSetID;
		PlayerPrefs.SetInt("Queued_Quest_Daily_SetID_Key", dailyQuestSetID);
		PlayerPrefs.SetInt("Queued_Quest_Daily_SaveCount_Key", num);
		int num2 = 0;
		for (int j = 0; j < this._queuedWeeklyCompletedQuests.Count; j++)
		{
			PlayerPrefs.SetInt(string.Format("{0}{1}", "Queued_Quest_Weekly_ID_Key", num2), this._queuedWeeklyCompletedQuests[j]);
			num2++;
		}
		int weeklyQuestSetID = this._questManager.weeklyQuestSetID;
		PlayerPrefs.SetInt("Queued_Quest_Weekly_SetID_Key", weeklyQuestSetID);
		PlayerPrefs.SetInt("Queued_Quest_Weekly_SaveCount_Key", num2);
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x00057194 File Offset: 0x00055394
	private void LoadCompletedQuestQueue()
	{
		this._queuedDailyCompletedQuests.Clear();
		int @int = PlayerPrefs.GetInt("Queued_Quest_Daily_SetID_Key", -1);
		int int2 = PlayerPrefs.GetInt("Queued_Quest_Daily_SaveCount_Key", -1);
		int dailyQuestSetID = this._questManager.dailyQuestSetID;
		if (@int == dailyQuestSetID)
		{
			for (int i = 0; i < int2; i++)
			{
				int int3 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Queued_Quest_Daily_ID_Key", i), -1);
				if (int3 != -1)
				{
					this._queuedDailyCompletedQuests.Add(int3);
				}
			}
		}
		this._queuedWeeklyCompletedQuests.Clear();
		int int4 = PlayerPrefs.GetInt("Queued_Quest_Weekly_SetID_Key", -1);
		int int5 = PlayerPrefs.GetInt("Queued_Quest_Weekly_SaveCount_Key", -1);
		int weeklyQuestSetID = this._questManager.weeklyQuestSetID;
		if (int4 == weeklyQuestSetID)
		{
			for (int j = 0; j < int5; j++)
			{
				int int6 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Queued_Quest_Weekly_ID_Key", j), -1);
				if (int6 != -1)
				{
					this._queuedWeeklyCompletedQuests.Add(int6);
				}
			}
		}
		this.SubmitNextQuestInQueue();
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x0005728C File Offset: 0x0005548C
	private async void RequestProgressRedemption(Action onComplete)
	{
		await Task.Yield();
		if (onComplete != null)
		{
			onComplete();
		}
	}

	// Token: 0x0600103A RID: 4154 RVA: 0x000572C3 File Offset: 0x000554C3
	private void OnProgressRedeemed()
	{
		this.unclaimedPoints = 0;
		PlayerPrefs.SetInt("Claimed_Points_Key", this.unclaimedPoints);
		this.ReportProgress();
	}

	// Token: 0x0600103B RID: 4155 RVA: 0x000572E4 File Offset: 0x000554E4
	private void AddPoints(int points)
	{
		if (this.weeklyPoints >= ProgressionController.WeeklyCap)
		{
			return;
		}
		int num = Mathf.Clamp(points, 0, ProgressionController.WeeklyCap - this.weeklyPoints);
		this.SetProgressionValues(this.weeklyPoints + num, this.unclaimedPoints + num, this.totalPointsRaw + num);
	}

	// Token: 0x0600103C RID: 4156 RVA: 0x00057334 File Offset: 0x00055534
	private void UpdateProgressionValues(int weekly, int totalRaw)
	{
		int num = totalRaw - this.totalPointsRaw;
		this.unclaimedPoints += num;
		this.SetProgressionValues(weekly, this.unclaimedPoints, totalRaw);
	}

	// Token: 0x0600103D RID: 4157 RVA: 0x00057366 File Offset: 0x00055566
	private void SetProgressionValues(int weekly, int unclaimed, int totalRaw)
	{
		this.weeklyPoints = weekly;
		this.unclaimedPoints = unclaimed;
		this.totalPointsRaw = totalRaw;
		this.ReportScoreChange();
		PlayerPrefs.SetInt("Claimed_Points_Key", unclaimed);
	}

	// Token: 0x0600103E RID: 4158 RVA: 0x00057390 File Offset: 0x00055590
	private async void ReportProgress()
	{
		try
		{
			if (!this._progressReportPending)
			{
				this._progressReportPending = true;
				await Task.Yield();
				this._progressReportPending = false;
				Action onProgressEvent = ProgressionController.OnProgressEvent;
				if (onProgressEvent != null)
				{
					onProgressEvent();
				}
				this.ReportScoreChange();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x0600103F RID: 4159 RVA: 0x000573C8 File Offset: 0x000555C8
	private void ReportScoreChange()
	{
		ValueTuple<int, int, int> valueTuple = new ValueTuple<int, int, int>(this.weeklyPoints, this.unclaimedPoints, this.totalPointsRaw);
		ValueTuple<int, int, int> lastProgressReport = this._lastProgressReport;
		ValueTuple<int, int, int> valueTuple2 = valueTuple;
		if (lastProgressReport.Item1 == valueTuple2.Item1 && lastProgressReport.Item2 == valueTuple2.Item2 && lastProgressReport.Item3 == valueTuple2.Item3)
		{
			return;
		}
		if (VRRig.LocalRig)
		{
			VRRig.LocalRig.SetQuestScore(ProgressionController.TotalPoints);
		}
		this._lastProgressReport = valueTuple;
	}

	// Token: 0x06001040 RID: 4160 RVA: 0x00057444 File Offset: 0x00055644
	[return: TupleElementNames(new string[] { "weekly", "unclaimed", "total" })]
	private ValueTuple<int, int, int> GetProgress()
	{
		return new ValueTuple<int, int, int>(this.weeklyPoints, this.unclaimedPoints, this.totalPointsRaw - this.unclaimedPoints);
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x00057499 File Offset: 0x00055699
	[CompilerGenerated]
	private bool <RequestStatus>g__ShouldFetchStatus|36_0()
	{
		return !this._isFetchingStatus && !this._statusReceived;
	}

	// Token: 0x04001344 RID: 4932
	private static ProgressionController _gInstance;

	// Token: 0x04001347 RID: 4935
	[SerializeField]
	private RotatingQuestsManager _questManager;

	// Token: 0x04001348 RID: 4936
	private int weeklyPoints;

	// Token: 0x04001349 RID: 4937
	private int totalPointsRaw;

	// Token: 0x0400134A RID: 4938
	private int unclaimedPoints;

	// Token: 0x0400134B RID: 4939
	private bool _progressReportPending;

	// Token: 0x0400134C RID: 4940
	[TupleElementNames(new string[] { "weeklyPoints", "unclaimedPoints", "totalPointsRaw" })]
	private ValueTuple<int, int, int> _lastProgressReport;

	// Token: 0x0400134D RID: 4941
	private bool _isFetchingStatus;

	// Token: 0x0400134E RID: 4942
	private bool _statusReceived;

	// Token: 0x0400134F RID: 4943
	private bool _isSendingQuestComplete;

	// Token: 0x04001350 RID: 4944
	private int _fetchStatusRetryCount;

	// Token: 0x04001351 RID: 4945
	private int _sendQuestCompleteRetryCount;

	// Token: 0x04001352 RID: 4946
	private int _maxRetriesOnFail = 3;

	// Token: 0x04001353 RID: 4947
	private List<int> _queuedDailyCompletedQuests = new List<int>();

	// Token: 0x04001354 RID: 4948
	private List<int> _queuedWeeklyCompletedQuests = new List<int>();

	// Token: 0x04001355 RID: 4949
	private int _currentlyProcessingQuest = -1;

	// Token: 0x04001356 RID: 4950
	private const string kUnclaimedPointKey = "Claimed_Points_Key";

	// Token: 0x04001358 RID: 4952
	private const string kQueuedDailyQuestSetIDKey = "Queued_Quest_Daily_SetID_Key";

	// Token: 0x04001359 RID: 4953
	private const string kQueuedDailyQuestSaveCountKey = "Queued_Quest_Daily_SaveCount_Key";

	// Token: 0x0400135A RID: 4954
	private const string kQueuedDailyQuestIDKey = "Queued_Quest_Daily_ID_Key";

	// Token: 0x0400135B RID: 4955
	private const string kQueuedWeeklyQuestSetIDKey = "Queued_Quest_Weekly_SetID_Key";

	// Token: 0x0400135C RID: 4956
	private const string kQueuedWeeklyQuestSaveCountKey = "Queued_Quest_Weekly_SaveCount_Key";

	// Token: 0x0400135D RID: 4957
	private const string kQueuedWeeklyQuestIDKey = "Queued_Quest_Weekly_ID_Key";

	// Token: 0x02000257 RID: 599
	[Serializable]
	private class GetQuestsStatusRequest
	{
		// Token: 0x0400135E RID: 4958
		public string PlayFabId;

		// Token: 0x0400135F RID: 4959
		public string PlayFabTicket;

		// Token: 0x04001360 RID: 4960
		public string MothershipId;

		// Token: 0x04001361 RID: 4961
		public string MothershipToken;
	}

	// Token: 0x02000258 RID: 600
	[Serializable]
	public class GetQuestStatusResponse
	{
		// Token: 0x04001362 RID: 4962
		public ProgressionController.UserQuestsStatus result;
	}

	// Token: 0x02000259 RID: 601
	public class UserQuestsStatus
	{
		// Token: 0x06001046 RID: 4166 RVA: 0x000574B0 File Offset: 0x000556B0
		public int GetWeeklyPoints()
		{
			int num = 0;
			if (this.dailyPoints != null)
			{
				foreach (KeyValuePair<string, int> keyValuePair in this.dailyPoints)
				{
					num += keyValuePair.Value;
				}
			}
			if (this.weeklyPoints != null)
			{
				foreach (KeyValuePair<int, int> keyValuePair2 in this.weeklyPoints)
				{
					num += keyValuePair2.Value;
				}
			}
			return Mathf.Min(num, ProgressionController.WeeklyCap);
		}

		// Token: 0x04001363 RID: 4963
		public Dictionary<string, int> dailyPoints;

		// Token: 0x04001364 RID: 4964
		public Dictionary<int, int> weeklyPoints;

		// Token: 0x04001365 RID: 4965
		public int userPointsTotal;
	}

	// Token: 0x0200025A RID: 602
	[Serializable]
	private class SetQuestCompleteRequest
	{
		// Token: 0x04001366 RID: 4966
		public string PlayFabId;

		// Token: 0x04001367 RID: 4967
		public string PlayFabTicket;

		// Token: 0x04001368 RID: 4968
		public string MothershipId;

		// Token: 0x04001369 RID: 4969
		public string MothershipToken;

		// Token: 0x0400136A RID: 4970
		public int QuestId;

		// Token: 0x0400136B RID: 4971
		public string ClientVersion;
	}

	// Token: 0x0200025B RID: 603
	[Serializable]
	public class SetQuestCompleteResponse
	{
		// Token: 0x0400136C RID: 4972
		public ProgressionController.UserQuestsStatus result;
	}
}
