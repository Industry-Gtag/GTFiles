using System;
using System.Collections;
using GorillaNetworking;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000517 RID: 1303
public class CodeRedemption : MonoBehaviour
{
	// Token: 0x0600208C RID: 8332 RVA: 0x000AED16 File Offset: 0x000ACF16
	public void Awake()
	{
		if (CodeRedemption.Instance == null)
		{
			CodeRedemption.Instance = this;
			return;
		}
		if (CodeRedemption.Instance != this)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x0600208D RID: 8333 RVA: 0x000AED48 File Offset: 0x000ACF48
	public void HandleCodeRedemption(string code)
	{
		string text = JsonConvert.SerializeObject(new CodeRedemption.CodeRedemptionRequest
		{
			itemGUID = code,
			playFabID = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			playFabSessionTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId
		});
		Debug.Log("[CodeRedemption] Web Request body: \n" + text);
		base.StartCoroutine(CodeRedemption.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeCodeItem", text, "application/json", new Action<UnityWebRequest>(this.OnCodeRedemptionResponse)));
	}

	// Token: 0x0600208E RID: 8334 RVA: 0x000AEDEC File Offset: 0x000ACFEC
	private void OnCodeRedemptionResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError("[CodeRedemption] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text);
			GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
			return;
		}
		string text = string.Empty;
		try
		{
			CodeRedemption.CodeRedemptionResponse codeRedemptionResponse = JsonConvert.DeserializeObject<CodeRedemption.CodeRedemptionResponse>(completedRequest.downloadHandler.text);
			if (codeRedemptionResponse.result.Contains("AlreadyRedeemed", StringComparison.OrdinalIgnoreCase))
			{
				Debug.Log("[CodeRedemption] Code has already been redeemed!");
				GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.AlreadyUsed;
				return;
			}
			if (codeRedemptionResponse.result.Contains("TooEarly", StringComparison.OrdinalIgnoreCase))
			{
				Debug.Log(string.Format("[CodeRedemption] Code is not redeemable until {0}!", codeRedemptionResponse.startTime));
				GorillaComputer.instance.RedemptionRestrictionTime = codeRedemptionResponse.startTime;
				GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.TooEarly;
				return;
			}
			if (codeRedemptionResponse.result.Contains("TooLate", StringComparison.OrdinalIgnoreCase))
			{
				Debug.Log(string.Format("[CodeRedemption] Code expired at {0}!", codeRedemptionResponse.endTime));
				GorillaComputer.instance.RedemptionRestrictionTime = codeRedemptionResponse.endTime;
				GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.TooLate;
				return;
			}
			if (codeRedemptionResponse.result.Contains("AlreadyGranted", StringComparison.OrdinalIgnoreCase))
			{
				Debug.Log("[CodeRedemption] Item has already been granted!");
				GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.AlreadyGranted;
				return;
			}
			text = codeRedemptionResponse.playFabItemName;
		}
		catch (Exception ex)
		{
			string text2 = "[CodeRedemption] Error parsing JSON response: ";
			Exception ex2 = ex;
			Debug.LogError(text2 + ((ex2 != null) ? ex2.ToString() : null));
			GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
			return;
		}
		Debug.Log("[CodeRedemption] Item successfully granted, processing external unlock...");
		GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Success;
		GorillaComputer.instance.RedemptionCode = "";
		base.StartCoroutine(this.CheckProcessExternalUnlock(new string[] { text }, true, true, true));
	}

	// Token: 0x0600208F RID: 8335 RVA: 0x000AEFDC File Offset: 0x000AD1DC
	private IEnumerator CheckProcessExternalUnlock(string[] itemIDs, bool autoEquip, bool isLeftHand, bool destroyOnFinish)
	{
		Debug.Log("[CodeRedemption] Checking if we can process external cosmetic unlock...");
		while (!CosmeticsController.instance.allCosmeticsDict_isInitialized)
		{
			yield return null;
		}
		Debug.Log("[CodeRedemption] Cosmetics initialized, proceeding to process external unlock...");
		foreach (string text in itemIDs)
		{
			CosmeticsController.instance.ProcessExternalUnlock(text, autoEquip, isLeftHand);
		}
		yield break;
	}

	// Token: 0x06002090 RID: 8336 RVA: 0x000AEFF9 File Offset: 0x000AD1F9
	private static IEnumerator ProcessWebRequest(string url, string data, string contentType, Action<UnityWebRequest> callback)
	{
		UnityWebRequest request = UnityWebRequest.Post(url, data, contentType);
		yield return request.SendWebRequest();
		callback(request);
		yield break;
	}

	// Token: 0x04002B60 RID: 11104
	public static volatile CodeRedemption Instance;

	// Token: 0x04002B61 RID: 11105
	private const string HiddenPathCollabEndpoint = "/api/ConsumeCodeItem";

	// Token: 0x02000518 RID: 1304
	[Serializable]
	private class CodeRedemptionRequest
	{
		// Token: 0x04002B62 RID: 11106
		public string itemGUID;

		// Token: 0x04002B63 RID: 11107
		public string playFabID;

		// Token: 0x04002B64 RID: 11108
		public string playFabSessionTicket;

		// Token: 0x04002B65 RID: 11109
		public string mothershipId;

		// Token: 0x04002B66 RID: 11110
		public string mothershipToken;

		// Token: 0x04002B67 RID: 11111
		public string mothershipEnvId;
	}

	// Token: 0x02000519 RID: 1305
	[Serializable]
	private class CodeRedemptionResponse
	{
		// Token: 0x04002B68 RID: 11112
		public string result;

		// Token: 0x04002B69 RID: 11113
		public string itemID;

		// Token: 0x04002B6A RID: 11114
		public string playFabItemName;

		// Token: 0x04002B6B RID: 11115
		public DateTimeOffset? startTime;

		// Token: 0x04002B6C RID: 11116
		public DateTimeOffset? endTime;
	}
}
