using System;
using System.Collections;
using GorillaNetworking;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020005BF RID: 1471
public class DeepLinkHandler : MonoBehaviour
{
	// Token: 0x06002527 RID: 9511 RVA: 0x000C7182 File Offset: 0x000C5382
	public void Awake()
	{
		if (DeepLinkHandler.instance == null)
		{
			DeepLinkHandler.instance = this;
			return;
		}
		if (DeepLinkHandler.instance != this)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x06002528 RID: 9512 RVA: 0x000C71B4 File Offset: 0x000C53B4
	public static void Initialize(GameObject parent)
	{
		if (DeepLinkHandler.instance == null && parent != null)
		{
			parent.AddComponent<DeepLinkHandler>();
		}
		if (DeepLinkHandler.instance == null)
		{
			return;
		}
		DeepLinkHandler.instance.RefreshLaunchDetails();
		if (DeepLinkHandler.instance.cachedLaunchDetails != null && DeepLinkHandler.instance.cachedLaunchDetails.LaunchType == LaunchType.Deeplink)
		{
			DeepLinkHandler.instance.HandleDeepLink();
			return;
		}
		Object.Destroy(DeepLinkHandler.instance);
	}

	// Token: 0x06002529 RID: 9513 RVA: 0x000C7238 File Offset: 0x000C5438
	private void RefreshLaunchDetails()
	{
		if (global::UnityEngine.Application.platform != RuntimePlatform.Android)
		{
			GTDev.Log<string>("[DeepLinkHandler::RefreshLaunchDetails] Not on Android Platform!", null);
			return;
		}
		this.cachedLaunchDetails = ApplicationLifecycle.GetLaunchDetails();
		GTDev.Log<string>(string.Concat(new string[]
		{
			"[DeepLinkHandler::RefreshLaunchDetails] LaunchType: ",
			this.cachedLaunchDetails.LaunchType.ToString(),
			"\n[DeepLinkHandler::RefreshLaunchDetails] LaunchSource: ",
			this.cachedLaunchDetails.LaunchSource,
			"\n[DeepLinkHandler::RefreshLaunchDetails] DeepLinkMessage: ",
			this.cachedLaunchDetails.DeeplinkMessage
		}), null);
	}

	// Token: 0x0600252A RID: 9514 RVA: 0x000C72C3 File Offset: 0x000C54C3
	private static IEnumerator ProcessWebRequest(string url, string data, string contentType, Action<UnityWebRequest> callback)
	{
		UnityWebRequest request = UnityWebRequest.Post(url, data, contentType);
		yield return request.SendWebRequest();
		callback(request);
		yield break;
	}

	// Token: 0x0600252B RID: 9515 RVA: 0x000C72E8 File Offset: 0x000C54E8
	private void HandleDeepLink()
	{
		GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Handling deep link...", null);
		if (this.cachedLaunchDetails.LaunchSource.Contains("7221491444554579"))
		{
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] DeepLink received from Witchblood, processing...", null);
			string text = JsonUtility.ToJson(new DeepLinkHandler.CollabRequest
			{
				itemGUID = this.cachedLaunchDetails.DeeplinkMessage,
				launchSource = this.cachedLaunchDetails.LaunchSource,
				oculusUserID = PlayFabAuthenticator.instance.userID,
				playFabID = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				playFabSessionTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId
			});
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Web Request body: \n" + text, null);
			base.StartCoroutine(DeepLinkHandler.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeItem", text, "application/json", new Action<UnityWebRequest>(this.OnWitchbloodCollabResponse)));
			return;
		}
		if (this.cachedLaunchDetails.LaunchSource.Contains("1903584373052985"))
		{
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] DeepLink received from Racoon Lagoon, processing...", null);
			string text2 = JsonUtility.ToJson(new DeepLinkHandler.CollabRequest
			{
				itemGUID = this.cachedLaunchDetails.DeeplinkMessage,
				launchSource = this.cachedLaunchDetails.LaunchSource,
				oculusUserID = PlayFabAuthenticator.instance.userID,
				playFabID = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				playFabSessionTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId
			});
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Web Request body: \n" + text2, null);
			base.StartCoroutine(DeepLinkHandler.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeItem", text2, "application/json", new Action<UnityWebRequest>(this.OnRaccoonLagoonCollabResponse)));
			return;
		}
		GTDev.LogError<string>("[DeepLinkHandler::HandleDeepLink] App launched via DeepLink, but from an unknown app. App ID: " + this.cachedLaunchDetails.LaunchSource, null);
		Object.Destroy(this);
	}

	// Token: 0x0600252C RID: 9516 RVA: 0x000C74F4 File Offset: 0x000C56F4
	private void OnWitchbloodCollabResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != UnityWebRequest.Result.Success)
		{
			GTDev.LogError<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text, null);
			Object.Destroy(this);
			return;
		}
		if (completedRequest.downloadHandler.text.Contains("AlreadyRedeemed", StringComparison.OrdinalIgnoreCase))
		{
			GTDev.Log<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Item has already been redeemed!", null);
			Object.Destroy(this);
			return;
		}
		GTDev.Log<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Item successfully granted, processing external unlock...", null);
		base.StartCoroutine(this.CheckProcessExternalUnlock(this.WitchbloodCollabCosmeticID, true, true, true));
	}

	// Token: 0x0600252D RID: 9517 RVA: 0x000C7584 File Offset: 0x000C5784
	private void OnRaccoonLagoonCollabResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != UnityWebRequest.Result.Success)
		{
			GTDev.LogError<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text, null);
			Object.Destroy(this);
			return;
		}
		if (completedRequest.downloadHandler.text.Contains("AlreadyRedeemed", StringComparison.OrdinalIgnoreCase))
		{
			GTDev.Log<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Item has already been redeemed!", null);
			Object.Destroy(this);
			return;
		}
		GTDev.Log<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Item successfully granted, processing external unlock...", null);
		base.StartCoroutine(this.CheckProcessExternalUnlock(this.RaccoonLagoonCosmeticIDs, true, true, true));
	}

	// Token: 0x0600252E RID: 9518 RVA: 0x000C7612 File Offset: 0x000C5812
	private IEnumerator CheckProcessExternalUnlock(string[] itemIDs, bool autoEquip, bool isLeftHand, bool destroyOnFinish)
	{
		GTDev.Log<string>("[DeepLinkHandler::CheckProcessExternalUnlock] Cosmetics initialized, proceeding to process external unlock...", null);
		foreach (string text in itemIDs)
		{
			CosmeticsController.instance.ProcessExternalUnlock(text, autoEquip, isLeftHand);
		}
		if (destroyOnFinish)
		{
			Object.Destroy(this);
		}
		yield return null;
		yield break;
	}

	// Token: 0x040030AE RID: 12462
	public static volatile DeepLinkHandler instance;

	// Token: 0x040030AF RID: 12463
	private LaunchDetails cachedLaunchDetails;

	// Token: 0x040030B0 RID: 12464
	private const string WitchbloodAppID = "7221491444554579";

	// Token: 0x040030B1 RID: 12465
	private readonly string[] WitchbloodCollabCosmeticID = new string[] { "LMAKT." };

	// Token: 0x040030B2 RID: 12466
	private const string RaccoonLagoonAppID = "1903584373052985";

	// Token: 0x040030B3 RID: 12467
	private readonly string[] RaccoonLagoonCosmeticIDs = new string[] { "LMALI.", "LHAGS." };

	// Token: 0x040030B4 RID: 12468
	private const string HiddenPathCollabEndpoint = "/api/ConsumeItem";

	// Token: 0x020005C0 RID: 1472
	[Serializable]
	private class CollabRequest
	{
		// Token: 0x040030B5 RID: 12469
		public string itemGUID;

		// Token: 0x040030B6 RID: 12470
		public string launchSource;

		// Token: 0x040030B7 RID: 12471
		public string oculusUserID;

		// Token: 0x040030B8 RID: 12472
		public string playFabID;

		// Token: 0x040030B9 RID: 12473
		public string playFabSessionTicket;

		// Token: 0x040030BA RID: 12474
		public string mothershipId;

		// Token: 0x040030BB RID: 12475
		public string mothershipToken;

		// Token: 0x040030BC RID: 12476
		public string mothershipEnvId;
	}
}
