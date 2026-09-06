using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020008B4 RID: 2228
public class GorillaTagCompetitiveServerApi : MonoBehaviour
{
	// Token: 0x06003A76 RID: 14966 RVA: 0x0013DA47 File Offset: 0x0013BC47
	private void Awake()
	{
		if (GorillaTagCompetitiveServerApi.Instance)
		{
			GTDev.LogError<string>("Duplicate GorillaTagCompetitiveServerApi detected. Destroying self.", base.gameObject, null);
			Object.Destroy(this);
			return;
		}
		GorillaTagCompetitiveServerApi.Instance = this;
	}

	// Token: 0x06003A77 RID: 14967 RVA: 0x0013DA74 File Offset: 0x0013BC74
	public void RequestGetRankInformation(List<string> playfabs, Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData> callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestGetRankInformation Client Not Logged into Mothership", null);
			return;
		}
		if (this.GetRankInformationInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestGetRankInformation already in progress", null);
			return;
		}
		this.GetRankInformationInProgress = true;
		string text = "PC";
		base.StartCoroutine(this.GetRankInformation(new GorillaTagCompetitiveServerApi.RankedModeProgressionRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = text,
			playfabIds = playfabs
		}, callback));
	}

	// Token: 0x06003A78 RID: 14968 RVA: 0x0013DAFD File Offset: 0x0013BCFD
	private IEnumerator GetRankInformation(GorillaTagCompetitiveServerApi.RankedModeProgressionRequestData data, Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/GetTier", "GET");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.OnCompleteGetRankInformation(request.downloadHandler.text, callback);
		}
		else if (request.result != UnityWebRequest.Result.ProtocolError)
		{
			retry = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_0136;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_0136;
			}
			bool flag = true;
			goto IL_0139;
			IL_0136:
			flag = false;
			IL_0139:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteGetRankInformation(null, callback);
			}
		}
		if (retry)
		{
			if (this.GetRankInformationRetryCount < this.MAX_SERVER_RETRIES)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.GetRankInformationRetryCount + 1)));
				this.GetRankInformationRetryCount++;
				yield return new WaitForSecondsRealtime(num);
				this.GetRankInformationInProgress = false;
				this.RequestGetRankInformation(data.playfabIds, callback);
			}
			else
			{
				this.GetRankInformationRetryCount = 0;
				this.OnCompleteGetRankInformation(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06003A79 RID: 14969 RVA: 0x0013DB1C File Offset: 0x0013BD1C
	private void OnCompleteGetRankInformation([CanBeNull] string response, Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData> callback)
	{
		this.GetRankInformationInProgress = false;
		this.GetRankInformationRetryCount = 0;
		if (response.IsNullOrEmpty())
		{
			return;
		}
		string text = "{ \"playerData\": " + response + " }";
		GorillaTagCompetitiveServerApi.RankedModeProgressionData rankedModeProgressionData;
		try
		{
			rankedModeProgressionData = JsonUtility.FromJson<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(text);
		}
		catch (ArgumentException ex)
		{
			Debug.LogException(ex);
			Debug.LogError("[GT/GorillaTagCompetitiveServerApi]  ERROR!!!  OnCompleteGetRankInformation: Encountered ArgumentException above while trying to parse json string:\n" + text);
			return;
		}
		catch (Exception ex2)
		{
			Debug.LogException(ex2);
			Debug.LogError("[GT/GorillaTagCompetitiveServerApi]  ERROR!!!  OnCompleteGetRankInformation: Encountered exception above while trying to parse json string:\n" + text);
			return;
		}
		if (callback != null)
		{
			callback(rankedModeProgressionData);
		}
	}

	// Token: 0x06003A7A RID: 14970 RVA: 0x0013DBB0 File Offset: 0x0013BDB0
	public void RequestCreateMatchId(Action<string> callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestCreateMatchId Client Not Logged into Mothership", null);
			return;
		}
		if (this.CreateMatchIdInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestCreateMatchId already in progress", null);
			return;
		}
		string text = "PC";
		this.CreateMatchIdInProgress = true;
		base.StartCoroutine(this.CreateMatchId(new GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = text
		}, callback));
	}

	// Token: 0x06003A7B RID: 14971 RVA: 0x0013DC32 File Offset: 0x0013BE32
	private IEnumerator CreateMatchId(GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed data, Action<string> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/CreateMatchId", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("CreateMatchId Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteCreateMatchId(request.downloadHandler.text, callback);
		}
		else if (request.result != UnityWebRequest.Result.ProtocolError)
		{
			retry = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_0156;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_0156;
			}
			bool flag = true;
			goto IL_0159;
			IL_0156:
			flag = false;
			IL_0159:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteCreateMatchId(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.CreateMatchIdRetryCount < this.MAX_SERVER_RETRIES)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.CreateMatchIdRetryCount + 1)));
				this.CreateMatchIdRetryCount++;
				yield return new WaitForSecondsRealtime(num);
				this.CreateMatchIdInProgress = false;
				this.RequestCreateMatchId(callback);
			}
			else
			{
				this.CreateMatchIdRetryCount = 0;
				this.OnCompleteCreateMatchId(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06003A7C RID: 14972 RVA: 0x0013DC4F File Offset: 0x0013BE4F
	private void OnCompleteCreateMatchId([CanBeNull] string response, Action<string> callback)
	{
		this.CreateMatchIdInProgress = false;
		this.CreateMatchIdRetryCount = 0;
		if (response.IsNullOrEmpty())
		{
			return;
		}
		if (callback != null)
		{
			callback(response);
		}
	}

	// Token: 0x06003A7D RID: 14973 RVA: 0x0013DC74 File Offset: 0x0013BE74
	public void RequestValidateMatchJoin(string matchId, Action<bool> callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestValidateMatchJoin Client Not Logged into Mothership", null);
			return;
		}
		if (this.ValidateMatchJoinInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestValidateMatchJoin already in progress", null);
			return;
		}
		string text = "PC";
		this.ValidateMatchJoinInProgress = true;
		base.StartCoroutine(this.ValidateMatchJoin(new GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = text,
			matchId = matchId
		}, callback));
	}

	// Token: 0x06003A7E RID: 14974 RVA: 0x0013DCFD File Offset: 0x0013BEFD
	private IEnumerator ValidateMatchJoin(GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId data, Action<bool> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/ValidateMatchJoin", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("ValidateMatchJoin Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteValidateMatchJoin(request.downloadHandler.text, callback);
		}
		else if (request.result != UnityWebRequest.Result.ProtocolError)
		{
			retry = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_0156;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_0156;
			}
			bool flag = true;
			goto IL_0159;
			IL_0156:
			flag = false;
			IL_0159:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteValidateMatchJoin(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.ValidateMatchJoinRetryCount < this.MAX_SERVER_RETRIES)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.ValidateMatchJoinRetryCount + 1)));
				this.ValidateMatchJoinRetryCount++;
				yield return new WaitForSecondsRealtime(num);
				this.ValidateMatchJoinInProgress = false;
				this.RequestValidateMatchJoin(data.matchId, callback);
			}
			else
			{
				this.ValidateMatchJoinRetryCount = 0;
				this.OnCompleteValidateMatchJoin(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06003A7F RID: 14975 RVA: 0x0013DD1C File Offset: 0x0013BF1C
	private void OnCompleteValidateMatchJoin([CanBeNull] string response, Action<bool> callback)
	{
		this.ValidateMatchJoinInProgress = false;
		this.ValidateMatchJoinRetryCount = 0;
		if (response.IsNullOrEmpty())
		{
			return;
		}
		GorillaTagCompetitiveServerApi.RankedModeValidateMatchJoinResponseData rankedModeValidateMatchJoinResponseData = JsonUtility.FromJson<GorillaTagCompetitiveServerApi.RankedModeValidateMatchJoinResponseData>(response);
		if (callback != null)
		{
			callback(rankedModeValidateMatchJoinResponseData.validJoin);
		}
	}

	// Token: 0x06003A80 RID: 14976 RVA: 0x0013DD58 File Offset: 0x0013BF58
	public void RequestSubmitMatchScores(string matchId, List<RankedMultiplayerScore.PlayerScore> finalScores)
	{
		List<GorillaTagCompetitiveServerApi.RankedModePlayerScore> list = new List<GorillaTagCompetitiveServerApi.RankedModePlayerScore>();
		foreach (RankedMultiplayerScore.PlayerScore playerScore in finalScores)
		{
			NetPlayer player = NetworkSystem.Instance.GetPlayer(playerScore.PlayerId);
			list.Add(new GorillaTagCompetitiveServerApi.RankedModePlayerScore
			{
				playfabId = player.UserId,
				gameScore = playerScore.GameScore
			});
		}
		this.RequestSubmitMatchScores(matchId, list);
	}

	// Token: 0x06003A81 RID: 14977 RVA: 0x0013DDE4 File Offset: 0x0013BFE4
	private void RequestSubmitMatchScores(string matchId, List<GorillaTagCompetitiveServerApi.RankedModePlayerScore> playerScores)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSubmitMatchScores Client Not Logged into Mothership", null);
			return;
		}
		if (this.SubmitMatchScoresInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSubmitMatchScores already in progress", null);
			return;
		}
		this.SubmitMatchScoresInProgress = true;
		base.StartCoroutine(this.SubmitMatchScores(new GorillaTagCompetitiveServerApi.RankedModeSubmitMatchScoresRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			matchId = matchId,
			playfabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			playerScores = playerScores
		}));
	}

	// Token: 0x06003A82 RID: 14978 RVA: 0x0013DE72 File Offset: 0x0013C072
	private IEnumerator SubmitMatchScores(GorillaTagCompetitiveServerApi.RankedModeSubmitMatchScoresRequestData data)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/SubmitMatchScores", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("SubmitMatchScores Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteSubmitMatchScores(request.downloadHandler.text);
		}
		else if (request.result != UnityWebRequest.Result.ProtocolError)
		{
			retry = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_0150;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_0150;
			}
			bool flag = true;
			goto IL_0153;
			IL_0150:
			flag = false;
			IL_0153:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteSubmitMatchScores(request.downloadHandler.text);
			}
		}
		if (retry)
		{
			if (this.SubmitMatchScoresRetryCount < this.MAX_SERVER_RETRIES)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.SubmitMatchScoresRetryCount + 1)));
				this.SubmitMatchScoresRetryCount++;
				yield return new WaitForSecondsRealtime(num);
				this.SubmitMatchScoresInProgress = false;
				this.RequestSubmitMatchScores(data.matchId, data.playerScores);
			}
			else
			{
				this.SubmitMatchScoresRetryCount = 0;
				this.OnCompleteSubmitMatchScores(null);
			}
		}
		yield break;
	}

	// Token: 0x06003A83 RID: 14979 RVA: 0x0013DE88 File Offset: 0x0013C088
	private void OnCompleteSubmitMatchScores([CanBeNull] string response)
	{
		this.SubmitMatchScoresInProgress = false;
		this.SubmitMatchScoresRetryCount = 0;
	}

	// Token: 0x06003A84 RID: 14980 RVA: 0x0013DE98 File Offset: 0x0013C098
	public void RequestSetEloValue(float desiredElo, Action callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSetEloValue Client Not Logged into Mothership", null);
			return;
		}
		if (this.SetEloValueInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSetEloValue already in progress", null);
			return;
		}
		string text = "PC";
		this.SetEloValueInProgress = true;
		base.StartCoroutine(this.SetEloValue(new GorillaTagCompetitiveServerApi.RankedModeSetEloValueRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = text,
			elo = desiredElo
		}, callback));
	}

	// Token: 0x06003A85 RID: 14981 RVA: 0x0013DF21 File Offset: 0x0013C121
	private IEnumerator SetEloValue(GorillaTagCompetitiveServerApi.RankedModeSetEloValueRequestData data, Action callback)
	{
		GTDev.LogWarning<string>("SetEloValue is for internal use only (Is Beta)", null);
		yield break;
	}

	// Token: 0x06003A86 RID: 14982 RVA: 0x0013DF29 File Offset: 0x0013C129
	private void OnCompleteSetEloValue([CanBeNull] string response, Action callback)
	{
		this.SetEloValueInProgress = false;
		this.SetEloValueRetryCount = 0;
		if (response != null && callback != null)
		{
			callback();
		}
	}

	// Token: 0x06003A87 RID: 14983 RVA: 0x0013DF48 File Offset: 0x0013C148
	public void RequestPingRoom(string matchId, Action callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestPingRoom Client Not Logged into Mothership", null);
			return;
		}
		if (this.SetEloValueInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestPingRoom already in progress", null);
			return;
		}
		string text = "PC";
		this.PingMatchInProgress = true;
		base.StartCoroutine(this.PingRoom(new GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = text,
			matchId = matchId
		}, callback));
	}

	// Token: 0x06003A88 RID: 14984 RVA: 0x0013DFD1 File Offset: 0x0013C1D1
	private IEnumerator PingRoom(GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId data, Action callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/PingRoom", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("PingRoom Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompletePingRoom(request.downloadHandler.text, callback);
		}
		else if (request.result != UnityWebRequest.Result.ProtocolError)
		{
			retry = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_0156;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_0156;
			}
			bool flag = true;
			goto IL_0159;
			IL_0156:
			flag = false;
			IL_0159:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompletePingRoom(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.PingMatchRetryCount < this.MAX_SERVER_RETRIES)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.PingMatchRetryCount + 1)));
				this.ValidateMatchJoinRetryCount++;
				yield return new WaitForSecondsRealtime(num);
				this.PingMatchInProgress = false;
				this.RequestPingRoom(data.matchId, callback);
			}
			else
			{
				this.PingMatchRetryCount = 0;
				this.OnCompletePingRoom(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06003A89 RID: 14985 RVA: 0x0013DFEE File Offset: 0x0013C1EE
	private void OnCompletePingRoom([CanBeNull] string response, Action callback)
	{
		GTDev.Log<string>("PingRoom complete", null);
		this.PingMatchInProgress = false;
		this.PingMatchRetryCount = 0;
		if (response != null && callback != null)
		{
			callback();
		}
	}

	// Token: 0x06003A8A RID: 14986 RVA: 0x0013E018 File Offset: 0x0013C218
	public void RequestUnlockCompetitiveQueue(bool unlocked, Action callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestUnlockCompetitiveQueue Client Not Logged into Mothership", null);
			return;
		}
		if (this.UnlockCompetitiveQueueInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestUnlockCompetitiveQueue already in progress", null);
			return;
		}
		string text = "PC";
		this.UnlockCompetitiveQueueInProgress = true;
		base.StartCoroutine(this.UnlockCompetitiveQueue(new GorillaTagCompetitiveServerApi.RankedModeUnlockCompetitiveQueueRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = text,
			unlocked = unlocked
		}, callback));
	}

	// Token: 0x06003A8B RID: 14987 RVA: 0x0013E0A1 File Offset: 0x0013C2A1
	private IEnumerator UnlockCompetitiveQueue(GorillaTagCompetitiveServerApi.RankedModeUnlockCompetitiveQueueRequestData data, Action callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/UnlockCompetitiveQueue", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			GTDev.Log<string>("UnlockCompetitiveQueue Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteUnlockCompetitiveQueue(request.downloadHandler.text, callback);
		}
		else if (request.result != UnityWebRequest.Result.ProtocolError)
		{
			retry = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_0156;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_0156;
			}
			bool flag = true;
			goto IL_0159;
			IL_0156:
			flag = false;
			IL_0159:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteUnlockCompetitiveQueue(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.UnlockCompetitiveQueueRetryCount < this.MAX_SERVER_RETRIES)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.UnlockCompetitiveQueueRetryCount + 1)));
				this.ValidateMatchJoinRetryCount++;
				yield return new WaitForSecondsRealtime(num);
				this.UnlockCompetitiveQueueInProgress = false;
				this.RequestUnlockCompetitiveQueue(data.unlocked, callback);
			}
			else
			{
				this.UnlockCompetitiveQueueRetryCount = 0;
				this.OnCompleteUnlockCompetitiveQueue(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06003A8C RID: 14988 RVA: 0x0013E0BE File Offset: 0x0013C2BE
	private void OnCompleteUnlockCompetitiveQueue([CanBeNull] string response, Action callback)
	{
		GTDev.Log<string>("UnlockCompetitiveQueue complete", null);
		this.UnlockCompetitiveQueueInProgress = false;
		this.UnlockCompetitiveQueueRetryCount = 0;
		if (response != null && callback != null)
		{
			callback();
		}
	}

	// Token: 0x04004A6C RID: 19052
	public static GorillaTagCompetitiveServerApi Instance;

	// Token: 0x04004A6D RID: 19053
	public int MAX_SERVER_RETRIES = 3;

	// Token: 0x04004A6E RID: 19054
	private bool GetRankInformationInProgress;

	// Token: 0x04004A6F RID: 19055
	private int GetRankInformationRetryCount;

	// Token: 0x04004A70 RID: 19056
	private bool CreateMatchIdInProgress;

	// Token: 0x04004A71 RID: 19057
	private int CreateMatchIdRetryCount;

	// Token: 0x04004A72 RID: 19058
	private bool ValidateMatchJoinInProgress;

	// Token: 0x04004A73 RID: 19059
	private int ValidateMatchJoinRetryCount;

	// Token: 0x04004A74 RID: 19060
	private bool SubmitMatchScoresInProgress;

	// Token: 0x04004A75 RID: 19061
	private int SubmitMatchScoresRetryCount;

	// Token: 0x04004A76 RID: 19062
	private bool SetEloValueInProgress;

	// Token: 0x04004A77 RID: 19063
	private int SetEloValueRetryCount;

	// Token: 0x04004A78 RID: 19064
	private bool PingMatchInProgress;

	// Token: 0x04004A79 RID: 19065
	private int PingMatchRetryCount;

	// Token: 0x04004A7A RID: 19066
	private bool UnlockCompetitiveQueueInProgress;

	// Token: 0x04004A7B RID: 19067
	private int UnlockCompetitiveQueueRetryCount;

	// Token: 0x020008B5 RID: 2229
	public enum EPlatformType
	{
		// Token: 0x04004A7D RID: 19069
		PC,
		// Token: 0x04004A7E RID: 19070
		Quest,
		// Token: 0x04004A7F RID: 19071
		NumPlatforms
	}

	// Token: 0x020008B6 RID: 2230
	[Serializable]
	public class RankedModeRequestDataBase
	{
		// Token: 0x04004A80 RID: 19072
		public string mothershipId;

		// Token: 0x04004A81 RID: 19073
		public string mothershipToken;

		// Token: 0x04004A82 RID: 19074
		public string mothershipEnvId;
	}

	// Token: 0x020008B7 RID: 2231
	[Serializable]
	public class RankedModeRequestDataPlatformed : GorillaTagCompetitiveServerApi.RankedModeRequestDataBase
	{
		// Token: 0x04004A83 RID: 19075
		public string platform;
	}

	// Token: 0x020008B8 RID: 2232
	[Serializable]
	public class RankedModeProgressionRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x04004A84 RID: 19076
		public List<string> playfabIds;
	}

	// Token: 0x020008B9 RID: 2233
	[Serializable]
	public class RankedModeProgressionPlatformData
	{
		// Token: 0x04004A85 RID: 19077
		public string platform;

		// Token: 0x04004A86 RID: 19078
		public float elo;

		// Token: 0x04004A87 RID: 19079
		public int majorTier;

		// Token: 0x04004A88 RID: 19080
		public int minorTier;

		// Token: 0x04004A89 RID: 19081
		public float rankProgress;
	}

	// Token: 0x020008BA RID: 2234
	[Serializable]
	public class RankedModePlayerProgressionData
	{
		// Token: 0x04004A8A RID: 19082
		public string playfabID;

		// Token: 0x04004A8B RID: 19083
		public GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData[] platformData = new GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData[2];
	}

	// Token: 0x020008BB RID: 2235
	[Serializable]
	public class RankedModeProgressionData
	{
		// Token: 0x04004A8C RID: 19084
		public List<GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData> playerData;
	}

	// Token: 0x020008BC RID: 2236
	[Serializable]
	public class RankedModeRequestDataWithMatchId : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x04004A8D RID: 19085
		public string matchId;
	}

	// Token: 0x020008BD RID: 2237
	[Serializable]
	public class RankedModeValidateMatchJoinResponseData
	{
		// Token: 0x04004A8E RID: 19086
		public bool validJoin;
	}

	// Token: 0x020008BE RID: 2238
	[Serializable]
	public class RankedModePlayerScore
	{
		// Token: 0x04004A8F RID: 19087
		public string playfabId;

		// Token: 0x04004A90 RID: 19088
		public float gameScore;
	}

	// Token: 0x020008BF RID: 2239
	[Serializable]
	public class RankedModeSubmitMatchScoresRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataBase
	{
		// Token: 0x04004A91 RID: 19089
		public string matchId;

		// Token: 0x04004A92 RID: 19090
		public string playfabId;

		// Token: 0x04004A93 RID: 19091
		public List<GorillaTagCompetitiveServerApi.RankedModePlayerScore> playerScores;
	}

	// Token: 0x020008C0 RID: 2240
	[Serializable]
	public class RankedModeSetEloValueRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x04004A94 RID: 19092
		public float elo;
	}

	// Token: 0x020008C1 RID: 2241
	[Serializable]
	public class RankedModeUnlockCompetitiveQueueRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x04004A95 RID: 19093
		public bool unlocked;
	}
}
