using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200023A RID: 570
public class MonkeVoteController : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x1700017F RID: 383
	// (get) Token: 0x06000F2E RID: 3886 RVA: 0x000532BE File Offset: 0x000514BE
	// (set) Token: 0x06000F2F RID: 3887 RVA: 0x000532C5 File Offset: 0x000514C5
	public static MonkeVoteController instance { get; private set; }

	// Token: 0x1400001C RID: 28
	// (add) Token: 0x06000F30 RID: 3888 RVA: 0x000532D0 File Offset: 0x000514D0
	// (remove) Token: 0x06000F31 RID: 3889 RVA: 0x00053308 File Offset: 0x00051508
	public event Action OnPollsUpdated;

	// Token: 0x1400001D RID: 29
	// (add) Token: 0x06000F32 RID: 3890 RVA: 0x00053340 File Offset: 0x00051540
	// (remove) Token: 0x06000F33 RID: 3891 RVA: 0x00053378 File Offset: 0x00051578
	public event Action OnVoteAccepted;

	// Token: 0x1400001E RID: 30
	// (add) Token: 0x06000F34 RID: 3892 RVA: 0x000533B0 File Offset: 0x000515B0
	// (remove) Token: 0x06000F35 RID: 3893 RVA: 0x000533E8 File Offset: 0x000515E8
	public event Action OnVoteFailed;

	// Token: 0x1400001F RID: 31
	// (add) Token: 0x06000F36 RID: 3894 RVA: 0x00053420 File Offset: 0x00051620
	// (remove) Token: 0x06000F37 RID: 3895 RVA: 0x00053458 File Offset: 0x00051658
	public event Action OnCurrentPollEnded;

	// Token: 0x06000F38 RID: 3896 RVA: 0x0005348D File Offset: 0x0005168D
	public void Awake()
	{
		if (MonkeVoteController.instance == null)
		{
			MonkeVoteController.instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06000F39 RID: 3897 RVA: 0x000534AC File Offset: 0x000516AC
	public void SliceUpdate()
	{
		if (this.isCurrentPollActive && !this.hasCurrentPollCompleted && this.currentPollCompletionTime < DateTime.UtcNow)
		{
			GTDev.Log<string>("Active vote poll completed.", null);
			this.hasCurrentPollCompleted = true;
			Action onCurrentPollEnded = this.OnCurrentPollEnded;
			if (onCurrentPollEnded == null)
			{
				return;
			}
			onCurrentPollEnded();
		}
	}

	// Token: 0x06000F3A RID: 3898 RVA: 0x00019260 File Offset: 0x00017460
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x00053500 File Offset: 0x00051700
	public async void RequestPolls()
	{
		if (!this.isFetchingPoll && (!this.hasPoll || (this.isCurrentPollActive && this.hasCurrentPollCompleted)))
		{
			this.isFetchingPoll = true;
			await this.WaitForSessionToken();
			this.FetchPolls();
		}
		else
		{
			Action onPollsUpdated = this.OnPollsUpdated;
			if (onPollsUpdated != null)
			{
				onPollsUpdated();
			}
		}
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x00053538 File Offset: 0x00051738
	private async Task WaitForSessionToken()
	{
		while (!PlayFabAuthenticator.instance || PlayFabAuthenticator.instance.GetPlayFabPlayerId().IsNullOrEmpty() || PlayFabAuthenticator.instance.GetPlayFabSessionTicket().IsNullOrEmpty() || PlayFabAuthenticator.instance.userID.IsNullOrEmpty())
		{
			await Task.Yield();
			await Task.Delay(1000);
		}
	}

	// Token: 0x06000F3E RID: 3902 RVA: 0x00053574 File Offset: 0x00051774
	private void FetchPolls()
	{
		base.StartCoroutine(this.DoFetchPolls(new MonkeVoteController.FetchPollsRequest
		{
			TitleId = PlayFabAuthenticatorSettings.TitleId,
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			IncludeInactive = this.includeInactive
		}, new Action<List<MonkeVoteController.FetchPollsResponse>>(this.OnFetchPollsResponse)));
	}

	// Token: 0x06000F3F RID: 3903 RVA: 0x000535DA File Offset: 0x000517DA
	private IEnumerator DoFetchPolls(MonkeVoteController.FetchPollsRequest data, Action<List<MonkeVoteController.FetchPollsResponse>> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.VotingApiBaseUrl + "/api/FetchPoll", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			List<MonkeVoteController.FetchPollsResponse> list = JsonConvert.DeserializeObject<List<MonkeVoteController.FetchPollsResponse>>(request.downloadHandler.text);
			callback(list);
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
			if (this.fetchPollsRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.fetchPollsRetryCount + 1));
				this.fetchPollsRetryCount++;
				yield return new WaitForSecondsRealtime((float)num);
				this.FetchPolls();
			}
			else
			{
				GTDev.LogError<string>("Maximum FetchPolls retries attempted. Please check your network connection.", null);
				this.fetchPollsRetryCount = 0;
				callback(null);
			}
		}
		yield break;
	}

	// Token: 0x06000F40 RID: 3904 RVA: 0x000535F8 File Offset: 0x000517F8
	private void OnFetchPollsResponse([CanBeNull] List<MonkeVoteController.FetchPollsResponse> response)
	{
		this.isFetchingPoll = false;
		this.hasPoll = false;
		this.lastPollData = null;
		this.currentPollData = null;
		this.isCurrentPollActive = false;
		this.hasCurrentPollCompleted = false;
		if (response != null)
		{
			DateTime minValue = DateTime.MinValue;
			using (List<MonkeVoteController.FetchPollsResponse>.Enumerator enumerator = response.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MonkeVoteController.FetchPollsResponse fetchPollsResponse = enumerator.Current;
					if (fetchPollsResponse.isActive)
					{
						this.hasPoll = true;
						this.currentPollData = fetchPollsResponse;
						if (this.currentPollData.EndTime > DateTime.UtcNow)
						{
							this.isCurrentPollActive = true;
							this.hasCurrentPollCompleted = false;
							this.currentPollCompletionTime = this.currentPollData.EndTime;
							this.currentPollCompletionTime = this.currentPollCompletionTime.AddMinutes(1.0);
						}
					}
					if (!fetchPollsResponse.isActive && fetchPollsResponse.EndTime > minValue && fetchPollsResponse.EndTime < DateTime.UtcNow)
					{
						this.lastPollData = fetchPollsResponse;
					}
				}
				goto IL_0106;
			}
		}
		GTDev.LogError<string>("Error: Could not fetch polls!", null);
		IL_0106:
		Action onPollsUpdated = this.OnPollsUpdated;
		if (onPollsUpdated == null)
		{
			return;
		}
		onPollsUpdated();
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x0005372C File Offset: 0x0005192C
	public void Vote(int pollId, int option, bool isPrediction)
	{
		if (!this.hasPoll)
		{
			return;
		}
		if (this.isSendingVote)
		{
			return;
		}
		this.isSendingVote = true;
		this.pollId = pollId;
		this.option = option;
		this.isPrediction = isPrediction;
		this.SendVote();
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x00053762 File Offset: 0x00051962
	private void SendVote()
	{
		this.GetNonceForVotingCallback(null);
	}

	// Token: 0x06000F43 RID: 3907 RVA: 0x0005376C File Offset: 0x0005196C
	private void GetNonceForVotingCallback([CanBeNull] Message<UserProof> message)
	{
		if (message != null)
		{
			UserProof data = message.Data;
			this.Nonce = ((data != null) ? data.Value : null);
		}
		base.StartCoroutine(this.DoVote(new MonkeVoteController.VoteRequest
		{
			PollId = this.pollId,
			TitleId = PlayFabAuthenticatorSettings.TitleId,
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			OculusId = PlayFabAuthenticator.instance.userID,
			UserPlatform = PlayFabAuthenticator.instance.platform.ToString(),
			UserNonce = this.Nonce,
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			OptionIndex = this.option,
			IsPrediction = this.isPrediction
		}, new Action<MonkeVoteController.VoteResponse>(this.OnVoteSuccess)));
	}

	// Token: 0x06000F44 RID: 3908 RVA: 0x0005383A File Offset: 0x00051A3A
	private IEnumerator DoVote(MonkeVoteController.VoteRequest data, Action<MonkeVoteController.VoteResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.VotingApiBaseUrl + "/api/Vote", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			MonkeVoteController.VoteResponse voteResponse = JsonConvert.DeserializeObject<MonkeVoteController.VoteResponse>(request.downloadHandler.text);
			callback(voteResponse);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.responseCode == 429L)
			{
				GTDev.LogWarning<string>("User already voted on this poll!", null);
				callback(null);
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (this.voteRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.voteRetryCount + 1));
				this.voteRetryCount++;
				yield return new WaitForSecondsRealtime((float)num);
				this.SendVote();
			}
			else
			{
				GTDev.LogError<string>("Maximum Vote retries attempted. Please check your network connection.", null);
				this.voteRetryCount = 0;
				callback(null);
			}
		}
		else
		{
			this.isSendingVote = false;
		}
		yield break;
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x00053857 File Offset: 0x00051A57
	private void OnVoteSuccess([CanBeNull] MonkeVoteController.VoteResponse response)
	{
		this.isSendingVote = false;
		if (response != null)
		{
			this.lastVoteData = response;
			Action onVoteAccepted = this.OnVoteAccepted;
			if (onVoteAccepted == null)
			{
				return;
			}
			onVoteAccepted();
			return;
		}
		else
		{
			Action onVoteFailed = this.OnVoteFailed;
			if (onVoteFailed == null)
			{
				return;
			}
			onVoteFailed();
			return;
		}
	}

	// Token: 0x06000F46 RID: 3910 RVA: 0x0005388B File Offset: 0x00051A8B
	public MonkeVoteController.FetchPollsResponse GetLastPollData()
	{
		return this.lastPollData;
	}

	// Token: 0x06000F47 RID: 3911 RVA: 0x00053893 File Offset: 0x00051A93
	public MonkeVoteController.FetchPollsResponse GetCurrentPollData()
	{
		return this.currentPollData;
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x0005389B File Offset: 0x00051A9B
	public MonkeVoteController.VoteResponse GetVoteData()
	{
		return this.lastVoteData;
	}

	// Token: 0x06000F49 RID: 3913 RVA: 0x000538A3 File Offset: 0x00051AA3
	public int GetLastVotePollId()
	{
		return this.pollId;
	}

	// Token: 0x06000F4A RID: 3914 RVA: 0x000538AB File Offset: 0x00051AAB
	public int GetLastVoteSelectedOption()
	{
		return this.option;
	}

	// Token: 0x06000F4B RID: 3915 RVA: 0x000538B3 File Offset: 0x00051AB3
	public bool GetLastVoteWasPrediction()
	{
		return this.isPrediction;
	}

	// Token: 0x06000F4C RID: 3916 RVA: 0x000538BB File Offset: 0x00051ABB
	public DateTime GetCurrentPollCompletionTime()
	{
		return this.currentPollCompletionTime;
	}

	// Token: 0x04001257 RID: 4695
	private string Nonce = "";

	// Token: 0x04001258 RID: 4696
	private bool includeInactive = true;

	// Token: 0x04001259 RID: 4697
	private int fetchPollsRetryCount;

	// Token: 0x0400125A RID: 4698
	private int maxRetriesOnFail = 3;

	// Token: 0x0400125B RID: 4699
	private int voteRetryCount;

	// Token: 0x04001260 RID: 4704
	private MonkeVoteController.FetchPollsResponse lastPollData;

	// Token: 0x04001261 RID: 4705
	private MonkeVoteController.FetchPollsResponse currentPollData;

	// Token: 0x04001262 RID: 4706
	private MonkeVoteController.VoteResponse lastVoteData;

	// Token: 0x04001263 RID: 4707
	private bool isFetchingPoll;

	// Token: 0x04001264 RID: 4708
	private bool hasPoll;

	// Token: 0x04001265 RID: 4709
	private bool isCurrentPollActive;

	// Token: 0x04001266 RID: 4710
	private bool hasCurrentPollCompleted;

	// Token: 0x04001267 RID: 4711
	private DateTime currentPollCompletionTime;

	// Token: 0x04001268 RID: 4712
	private bool isSendingVote;

	// Token: 0x04001269 RID: 4713
	private int pollId = -1;

	// Token: 0x0400126A RID: 4714
	private int option;

	// Token: 0x0400126B RID: 4715
	private bool isPrediction;

	// Token: 0x0200023B RID: 571
	[Serializable]
	private class FetchPollsRequest
	{
		// Token: 0x0400126C RID: 4716
		public string TitleId;

		// Token: 0x0400126D RID: 4717
		public string PlayFabId;

		// Token: 0x0400126E RID: 4718
		public string PlayFabTicket;

		// Token: 0x0400126F RID: 4719
		public bool IncludeInactive;
	}

	// Token: 0x0200023C RID: 572
	[Serializable]
	public class FetchPollsResponse
	{
		// Token: 0x04001270 RID: 4720
		public int PollId;

		// Token: 0x04001271 RID: 4721
		public string Question;

		// Token: 0x04001272 RID: 4722
		public List<string> VoteOptions;

		// Token: 0x04001273 RID: 4723
		public List<int> VoteCount;

		// Token: 0x04001274 RID: 4724
		public List<int> PredictionCount;

		// Token: 0x04001275 RID: 4725
		public DateTime StartTime;

		// Token: 0x04001276 RID: 4726
		public DateTime EndTime;

		// Token: 0x04001277 RID: 4727
		public bool isActive;
	}

	// Token: 0x0200023D RID: 573
	[Serializable]
	private class VoteRequest
	{
		// Token: 0x04001278 RID: 4728
		public int PollId;

		// Token: 0x04001279 RID: 4729
		public string TitleId;

		// Token: 0x0400127A RID: 4730
		public string PlayFabId;

		// Token: 0x0400127B RID: 4731
		public string OculusId;

		// Token: 0x0400127C RID: 4732
		public string UserNonce;

		// Token: 0x0400127D RID: 4733
		public string UserPlatform;

		// Token: 0x0400127E RID: 4734
		public int OptionIndex;

		// Token: 0x0400127F RID: 4735
		public bool IsPrediction;

		// Token: 0x04001280 RID: 4736
		public string PlayFabTicket;
	}

	// Token: 0x0200023E RID: 574
	[Serializable]
	public class VoteResponse
	{
		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000F51 RID: 3921 RVA: 0x000538EB File Offset: 0x00051AEB
		// (set) Token: 0x06000F52 RID: 3922 RVA: 0x000538F3 File Offset: 0x00051AF3
		public int PollId { get; set; }

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06000F53 RID: 3923 RVA: 0x000538FC File Offset: 0x00051AFC
		// (set) Token: 0x06000F54 RID: 3924 RVA: 0x00053904 File Offset: 0x00051B04
		public string TitleId { get; set; }

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06000F55 RID: 3925 RVA: 0x0005390D File Offset: 0x00051B0D
		// (set) Token: 0x06000F56 RID: 3926 RVA: 0x00053915 File Offset: 0x00051B15
		public List<string> VoteOptions { get; set; }

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000F57 RID: 3927 RVA: 0x0005391E File Offset: 0x00051B1E
		// (set) Token: 0x06000F58 RID: 3928 RVA: 0x00053926 File Offset: 0x00051B26
		public List<int> VoteCount { get; set; }

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000F59 RID: 3929 RVA: 0x0005392F File Offset: 0x00051B2F
		// (set) Token: 0x06000F5A RID: 3930 RVA: 0x00053937 File Offset: 0x00051B37
		public List<int> PredictionCount { get; set; }
	}
}
