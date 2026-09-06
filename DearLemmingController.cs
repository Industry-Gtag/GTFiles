using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000CA2 RID: 3234
public class DearLemmingController : MonoBehaviour
{
	// Token: 0x1700076D RID: 1901
	// (get) Token: 0x06004FD6 RID: 20438 RVA: 0x001A8DB6 File Offset: 0x001A6FB6
	// (set) Token: 0x06004FD7 RID: 20439 RVA: 0x001A8DBD File Offset: 0x001A6FBD
	public static DearLemmingController instance { get; private set; }

	// Token: 0x14000089 RID: 137
	// (add) Token: 0x06004FD8 RID: 20440 RVA: 0x001A8DC8 File Offset: 0x001A6FC8
	// (remove) Token: 0x06004FD9 RID: 20441 RVA: 0x001A8E00 File Offset: 0x001A7000
	public event Action<DearLemmingController.DearLemmingResponse> OnCheckComplete;

	// Token: 0x1400008A RID: 138
	// (add) Token: 0x06004FDA RID: 20442 RVA: 0x001A8E38 File Offset: 0x001A7038
	// (remove) Token: 0x06004FDB RID: 20443 RVA: 0x001A8E70 File Offset: 0x001A7070
	public event Action<DearLemmingController.DearLemmingResponse> OnSubmitComplete;

	// Token: 0x06004FDC RID: 20444 RVA: 0x001A8EA5 File Offset: 0x001A70A5
	public void Awake()
	{
		if (DearLemmingController.instance == null)
		{
			DearLemmingController.instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06004FDD RID: 20445 RVA: 0x001A8EC4 File Offset: 0x001A70C4
	public async void CheckCanSubmit()
	{
		if (!this.isChecking)
		{
			this.isChecking = true;
			await this.WaitForLogin();
			this.StartCheck();
		}
	}

	// Token: 0x06004FDE RID: 20446 RVA: 0x001A8EFC File Offset: 0x001A70FC
	public async void SubmitMessage(string messageText)
	{
		if (!this.isSubmitting)
		{
			this.isSubmitting = true;
			await this.WaitForLogin();
			this.StartSubmit(messageText);
		}
	}

	// Token: 0x06004FDF RID: 20447 RVA: 0x001A8F3B File Offset: 0x001A713B
	private void StartCheck()
	{
		base.StartCoroutine(this.DoRequest("/api/CheckDearLemming", null, true));
	}

	// Token: 0x06004FE0 RID: 20448 RVA: 0x001A8F51 File Offset: 0x001A7151
	private void StartSubmit(string messageText)
	{
		base.StartCoroutine(this.DoRequest("/api/SubmitDearLemming", messageText, false));
	}

	// Token: 0x06004FE1 RID: 20449 RVA: 0x001A8F67 File Offset: 0x001A7167
	private IEnumerator DoRequest(string endpoint, string messageText, bool isCheckRequest)
	{
		DearLemmingController.DearLemmingRequest dearLemmingRequest = new DearLemmingController.DearLemmingRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipToken = MothershipClientContext.Token,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MessageText = messageText
		};
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.VotingApiBaseUrl + endpoint, "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(dearLemmingRequest));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success || request.responseCode == 201L)
		{
			this.HandleResponse(request.downloadHandler.text, isCheckRequest);
		}
		else
		{
			long responseCode = request.responseCode;
			if ((responseCode < 500L || responseCode >= 600L) && request.result != UnityWebRequest.Result.ConnectionError)
			{
				this.HandleResponse(request.downloadHandler.text, isCheckRequest);
				yield break;
			}
			retry = true;
		}
		if (retry)
		{
			int num = (isCheckRequest ? this.checkRetryCount : this.submitRetryCount);
			if (num < this.maxRetriesOnFail)
			{
				int num2 = (int)Mathf.Pow(2f, (float)(num + 1));
				if (isCheckRequest)
				{
					this.checkRetryCount++;
				}
				else
				{
					this.submitRetryCount++;
				}
				yield return new WaitForSecondsRealtime((float)num2);
				if (isCheckRequest)
				{
					this.StartCheck();
				}
				else
				{
					this.StartSubmit(messageText);
				}
			}
			else
			{
				if (isCheckRequest)
				{
					this.checkRetryCount = 0;
				}
				else
				{
					this.submitRetryCount = 0;
				}
				this.HandleResponse(null, isCheckRequest);
			}
		}
		yield break;
	}

	// Token: 0x06004FE2 RID: 20450 RVA: 0x001A8F8C File Offset: 0x001A718C
	private void HandleResponse(string json, bool isCheckRequest)
	{
		DearLemmingController.DearLemmingResponse dearLemmingResponse = null;
		if (!string.IsNullOrEmpty(json))
		{
			try
			{
				dearLemmingResponse = JsonConvert.DeserializeObject<DearLemmingController.DearLemmingResponse>(json);
			}
			catch
			{
			}
		}
		if (isCheckRequest)
		{
			this.isChecking = false;
			Action<DearLemmingController.DearLemmingResponse> onCheckComplete = this.OnCheckComplete;
			if (onCheckComplete == null)
			{
				return;
			}
			onCheckComplete(dearLemmingResponse);
			return;
		}
		else
		{
			this.isSubmitting = false;
			Action<DearLemmingController.DearLemmingResponse> onSubmitComplete = this.OnSubmitComplete;
			if (onSubmitComplete == null)
			{
				return;
			}
			onSubmitComplete(dearLemmingResponse);
			return;
		}
	}

	// Token: 0x06004FE3 RID: 20451 RVA: 0x001A8FF4 File Offset: 0x001A71F4
	private async Task WaitForLogin()
	{
		while (!MothershipClientApiUnity.IsClientLoggedIn())
		{
			await Task.Yield();
			await Task.Delay(1000);
		}
	}

	// Token: 0x0400621A RID: 25114
	private int maxRetriesOnFail = 3;

	// Token: 0x0400621B RID: 25115
	private int checkRetryCount;

	// Token: 0x0400621C RID: 25116
	private int submitRetryCount;

	// Token: 0x0400621F RID: 25119
	private bool isChecking;

	// Token: 0x04006220 RID: 25120
	private bool isSubmitting;

	// Token: 0x02000CA3 RID: 3235
	[Serializable]
	private class DearLemmingRequest
	{
		// Token: 0x04006221 RID: 25121
		public string MothershipId;

		// Token: 0x04006222 RID: 25122
		public string MothershipToken;

		// Token: 0x04006223 RID: 25123
		public string MothershipTitleId;

		// Token: 0x04006224 RID: 25124
		public string MothershipEnvId;

		// Token: 0x04006225 RID: 25125
		public string MessageText;
	}

	// Token: 0x02000CA4 RID: 3236
	[Serializable]
	public class DearLemmingResponse
	{
		// Token: 0x04006226 RID: 25126
		public bool CanSubmit;

		// Token: 0x04006227 RID: 25127
		public DateTime? NextSubmitTimeUtc;

		// Token: 0x04006228 RID: 25128
		public double? SecondsUntilNextSubmit;

		// Token: 0x04006229 RID: 25129
		public string Error;

		// Token: 0x0400622A RID: 25130
		public int StatusCode;
	}
}
