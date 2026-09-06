using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaNetworking;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020003E0 RID: 992
public class HowManyMonke : MonoBehaviour
{
	// Token: 0x1700024F RID: 591
	// (get) Token: 0x060017A9 RID: 6057 RVA: 0x00087CFF File Offset: 0x00085EFF
	public static float RecheckDelay
	{
		get
		{
			return Mathf.Max((float)HowManyMonke.recheckDelay / 1000f, 1f);
		}
	}

	// Token: 0x060017AA RID: 6058 RVA: 0x00087D18 File Offset: 0x00085F18
	public async void Start()
	{
		this.state = HowManyMonke.State.READY;
		await Task.Delay(1000);
		Debug.Log("[GT/HowManyMonke]  " + string.Format("Checking NetworkSystem.Instance: {0}", NetworkSystem.Instance));
		while (NetworkSystem.Instance == null)
		{
			await Task.Delay(1000);
			Debug.Log("[GT/HowManyMonke]  " + string.Format("Re-Checking NetworkSystem.Instance: {0}", NetworkSystem.Instance));
		}
		TaskAwaiter<int> taskAwaiter = this.FetchThisMany().GetAwaiter();
		TaskAwaiter<int> taskAwaiter2;
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<int>);
		}
		HowManyMonke.ThisMany = taskAwaiter.GetResult();
		if (HowManyMonke.OnCheck != null)
		{
			HowManyMonke.OnCheck(HowManyMonke.ThisMany);
		}
		Debug.Log("[GT/HowManyMonke]  " + string.Format("Fetch Complete: {0}", HowManyMonke.ThisMany));
		await this.FetchRecheckDelay();
		while (Application.isPlaying && HowManyMonke.recheckDelay > 0)
		{
			await Task.Delay(HowManyMonke.recheckDelay);
			if (HowManyMonke.OnCheck != null)
			{
				taskAwaiter = this.FetchThisMany().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<int>);
				}
				HowManyMonke.ThisMany = taskAwaiter.GetResult();
				HowManyMonke.OnCheck(HowManyMonke.ThisMany);
				await this.FetchRecheckDelay();
			}
		}
	}

	// Token: 0x060017AB RID: 6059 RVA: 0x00087D50 File Offset: 0x00085F50
	private async Task FetchRecheckDelay()
	{
		this.state = HowManyMonke.State.TD_LOOKUP;
		PlayFabTitleDataCache.Instance.GetTitleData(this.titleDataKey, new Action<string>(this.onTD), new Action<PlayFabError>(this.onTDError), false);
		while (this.state != HowManyMonke.State.READY)
		{
			await Task.Yield();
		}
	}

	// Token: 0x060017AC RID: 6060 RVA: 0x00087D93 File Offset: 0x00085F93
	private void onTDError(PlayFabError error)
	{
		this.state = HowManyMonke.State.READY;
		HowManyMonke.recheckDelay = 0;
	}

	// Token: 0x060017AD RID: 6061 RVA: 0x00087DA2 File Offset: 0x00085FA2
	private void onTD(string obj)
	{
		this.state = HowManyMonke.State.READY;
		if (int.TryParse(obj, out HowManyMonke.recheckDelay))
		{
			HowManyMonke.recheckDelay *= 1000;
			return;
		}
		HowManyMonke.recheckDelay = 0;
	}

	// Token: 0x060017AE RID: 6062 RVA: 0x00087DD0 File Offset: 0x00085FD0
	private async Task<int> FetchThisMany()
	{
		int num;
		if (HowManyMonke.recheckDelay < 0)
		{
			num = NetworkSystem.Instance.GlobalPlayerCount();
		}
		else
		{
			UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.ModerationApiBaseUrl + this.CCUEndpoint, "POST");
			request.downloadHandler = new DownloadHandlerBuffer();
			await request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				num = JsonConvert.DeserializeObject<HowManyMonke.CCUResponse>(request.downloadHandler.text).CCUTotal;
			}
			else
			{
				num = NetworkSystem.Instance.GlobalPlayerCount();
			}
		}
		return num;
	}

	// Token: 0x040022E1 RID: 8929
	private const string preLog = "[GT/HowManyMonke]  ";

	// Token: 0x040022E2 RID: 8930
	private const string preErr = "ERROR!!!  ";

	// Token: 0x040022E3 RID: 8931
	public static int ThisMany = 12549;

	// Token: 0x040022E4 RID: 8932
	public static Action<int> OnCheck;

	// Token: 0x040022E5 RID: 8933
	[SerializeField]
	private string titleDataKey;

	// Token: 0x040022E6 RID: 8934
	private HowManyMonke.State state;

	// Token: 0x040022E7 RID: 8935
	private static int recheckDelay;

	// Token: 0x040022E8 RID: 8936
	[SerializeField]
	private string CCUEndpoint;

	// Token: 0x020003E1 RID: 993
	private enum State
	{
		// Token: 0x040022EA RID: 8938
		READY,
		// Token: 0x040022EB RID: 8939
		TD_LOOKUP,
		// Token: 0x040022EC RID: 8940
		HMM_LOOKUP
	}

	// Token: 0x020003E2 RID: 994
	private class CCUResponse
	{
		// Token: 0x040022ED RID: 8941
		public int CCUTotal;

		// Token: 0x040022EE RID: 8942
		public string ErrorMessage;
	}
}
