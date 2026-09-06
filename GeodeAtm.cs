using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oculus.Platform;
using Oculus.Platform.Models;
using Steamworks;
using TMPro;
using UnityEngine;

// Token: 0x02000571 RID: 1393
public class GeodeAtm : MonoBehaviour
{
	// Token: 0x06002363 RID: 9059 RVA: 0x000BE89C File Offset: 0x000BCA9C
	private string GetMetaSku(GeodeAtm.GeodePurchaseSize size)
	{
		string text = "";
		switch (size)
		{
		default:
			return text;
		}
	}

	// Token: 0x06002364 RID: 9060 RVA: 0x000BE8C4 File Offset: 0x000BCAC4
	private static string GetLocalizedText(string key, string fallback)
	{
		string text;
		if (!LocalisationManager.TryGetKeyForCurrentLocale(key, out text, fallback))
		{
			Debug.LogError("[LOCALIZATION::GEODE_ATM] Failed to get key for Geode ATM localization [" + key + "]");
		}
		return text;
	}

	// Token: 0x06002365 RID: 9061 RVA: 0x000BE8F4 File Offset: 0x000BCAF4
	private void UpdateStatusText(string text)
	{
		string text2;
		if (GeodeAtm.fetchedGeodes)
		{
			text2 = GeodeAtm.GetLocalizedText("GEODE_ATM_BALANCE", "Current Geodes Balance: {balance}").Replace("{balance}", GeodeAtm.geodes.ToString());
		}
		else
		{
			text2 = GeodeAtm.GetLocalizedText("GEODE_ATM_BALANCE_LOADING", "Loading Geodes Balance...");
		}
		text2 = text2 + "\n" + text;
		foreach (TextMeshPro textMeshPro in this.StatusTexts)
		{
			textMeshPro.text = text2;
		}
	}

	// Token: 0x06002366 RID: 9062 RVA: 0x000BE998 File Offset: 0x000BCB98
	private void ProcessSteamCallback(MicroTxnAuthorizationResponse_t callBackResponse)
	{
		if (callBackResponse.m_bAuthorized == 0)
		{
			Debug.Log("The user did not authorize the steam geodes purchase.");
			this.UpdateStatusText(GeodeAtm.GetLocalizedText("GEODE_ATM_PURCHASE_CANCELLED", "The purchase could not continue because it was cancelled. Please try again."));
			this.purchaseInFlight = false;
			GeodeAtm.fetchedGeodes = false;
			return;
		}
		if (this.steamOrderId.IsNullOrEmpty())
		{
			this.steamOrderId = callBackResponse.m_ulOrderID.ToString();
		}
		MothershipClientApiUnity.FinalizeSteamPurchase(callBackResponse.m_ulOrderID.ToString(), delegate(FinalizeSteamPurchaseResponse Response)
		{
			GeodeAtm.ProcessingGeodePurchase = false;
			GeodeAtm.fetchedGeodes = false;
			this.purchaseInFlight = false;
			this.RefreshGeodeBalance();
		}, delegate(MothershipError Error, int Status)
		{
			GeodeAtm.ProcessingGeodePurchase = false;
			GeodeAtm.fetchedGeodes = false;
			this.purchaseInFlight = false;
			Debug.LogError("Geodes ATM could not finalzie STEAM iap. Trace ID: " + Error.TraceId + ", Error Code: " + Error.MothershipErrorCode);
			this.UpdateStatusText(GeodeAtm.GetLocalizedText("GEODE_ATM_PURCHASE_FINALIZE_ERROR", "An unexpected error occurred while finalizing this purchase. Trace ID: {traceId}, Error Code: {errorCode}, Session ID: {sessionId} Refreshing current Geodes Balance...").Replace("{traceId}", Error.TraceId ?? "").Replace("{errorCode}", Error.MothershipErrorCode ?? "")
				.Replace("{sessionId}", MothershipClientApiUnity.SessionId ?? ""));
			this.RefreshGeodeBalance();
		});
	}

	// Token: 0x06002367 RID: 9063 RVA: 0x000BEA24 File Offset: 0x000BCC24
	private void OnMetaPurchaseComplete(Message<Purchase> msg)
	{
		this.purchaseInFlight = false;
		if (msg.IsError)
		{
			Error error = msg.GetError();
			Debug.Log(string.Format("Meta Geodes Failure: Error Code: {0}, HTTP Code: {1}, Message: {2}", (error != null) ? new int?(error.Code) : null, (error != null) ? new int?(error.HttpCode) : null, (error != null) ? error.Message : null));
			GeodeAtm.fetchedGeodes = false;
			this.UpdateStatusText(GeodeAtm.GetLocalizedText("GEODE_ATM_PURCHASE_META_ERROR", "This purchase could not continue because something went wrong processing the transaction with Meta. Was the transaction cancelled? Please try again."));
			this.RefreshGeodeBalance();
			return;
		}
		MothershipClientApiUnity.RefreshMetaIAP(delegate(MothershipRefreshIAPResponse Result)
		{
			GeodeAtm.fetchedGeodes = false;
			this.RefreshGeodeBalance();
		}, delegate(MothershipError Error, int StatusCode)
		{
			GeodeAtm.fetchedGeodes = false;
			this.RefreshGeodeBalance();
			Debug.Log(string.Concat(new string[]
			{
				"Something went wrong refreshing meta iap with Mothership ",
				Error.Message,
				" ",
				Error.MothershipErrorCode,
				" Trace ID: ",
				Error.TraceId,
				" Session ID: ",
				MothershipClientApiUnity.SessionId
			}));
		});
	}

	// Token: 0x06002368 RID: 9064 RVA: 0x000BEAE4 File Offset: 0x000BCCE4
	private async Task StartPurchase(GeodeAtm.GeodePurchaseSize size)
	{
		if (SteamManager.Initialized && this._steamMicroTransactionAuthorizationResponse == null)
		{
			this._steamMicroTransactionAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(new Callback<MicroTxnAuthorizationResponse_t>.DispatchDelegate(this.ProcessSteamCallback));
		}
		Debug.Log("Starting Steam Geodes Purchase");
		this.UpdateStatusText(GeodeAtm.GetLocalizedText("GEODE_ATM_PURCHASE_IN_PROGRESS", "Geodes Purchase in Progress"));
		GeodeAtm.ProcessingGeodePurchase = true;
		string text = "";
		int num = -1;
		string mothershipGeodeOfferDisplayIdLive = this.MothershipGeodeOfferDisplayIdLive;
		switch (size)
		{
		case GeodeAtm.GeodePurchaseSize.SMALL:
			text = this.MothershipSteamSmallGeodeOfferIdLive;
			num = this.MothershipSmallSteamGeodeOfferDisplayIndexLive;
			break;
		case GeodeAtm.GeodePurchaseSize.MEDIUM:
			text = this.MothershipSteamMediumGeodeOfferIdLive;
			num = this.MothershipMediumSteamGeodeOfferDisplayIndexLive;
			break;
		case GeodeAtm.GeodePurchaseSize.LARGE:
			text = this.MothershipSteamLargeGeodeOfferIdDev;
			num = this.MothershipLargeSteamGeodeOfferDisplayIndexLive;
			break;
		}
		MothershipClientApiUnity.InitSteamPurchase(mothershipGeodeOfferDisplayIdLive, text, num, delegate(InitSteamPurchaseResponse Response)
		{
			this.steamOrderId = Response.SteamOrderId;
		}, delegate(MothershipError Error, int StatusCode)
		{
			this.UpdateStatusText(GeodeAtm.GetLocalizedText("GEODE_ATM_PURCHASE_ERROR", "Something went wrong trying to purchase Geodes: {errorCode} Trace ID: {traceId} Session ID: {sessionId}").Replace("{errorCode}", Error.MothershipErrorCode ?? "").Replace("{traceId}", Error.TraceId ?? "")
				.Replace("{sessionId}", MothershipClientApiUnity.SessionId ?? ""));
			Debug.Log(string.Concat(new string[]
			{
				"Something went wrong trying to purchase Geodes on Stea, ",
				Error.Message,
				" ",
				Error.MothershipErrorCode,
				" Trace ID: ",
				Error.TraceId,
				" Session ID: ",
				MothershipClientApiUnity.SessionId
			}));
		});
	}

	// Token: 0x06002369 RID: 9065 RVA: 0x000BEB2F File Offset: 0x000BCD2F
	public void SmallPurchase()
	{
		if (!this.purchaseInFlight)
		{
			this.purchaseInFlight = true;
			this.StartPurchase(GeodeAtm.GeodePurchaseSize.SMALL);
		}
	}

	// Token: 0x0600236A RID: 9066 RVA: 0x000BEB48 File Offset: 0x000BCD48
	public void MediumPurchase()
	{
		if (!this.purchaseInFlight)
		{
			this.purchaseInFlight = true;
			this.StartPurchase(GeodeAtm.GeodePurchaseSize.MEDIUM);
		}
	}

	// Token: 0x0600236B RID: 9067 RVA: 0x000BEB61 File Offset: 0x000BCD61
	public void LargePurchase()
	{
		if (!this.purchaseInFlight)
		{
			this.purchaseInFlight = true;
			this.StartPurchase(GeodeAtm.GeodePurchaseSize.LARGE);
		}
	}

	// Token: 0x0600236C RID: 9068 RVA: 0x000BEB7A File Offset: 0x000BCD7A
	public void RefreshGeodeBalance()
	{
		if (GeodeAtm.fetchedGeodes)
		{
			this.UpdateStatusText("");
			return;
		}
		MothershipClientApiUnity.GetUserInventory(delegate(MothershipGetInventoryResponse Result)
		{
			GeodeAtm.geodes = 0;
			GeodeAtm.fetchedGeodes = true;
			foreach (KeyValuePair<string, MothershipPlayerInventorySummary> keyValuePair in Result.Results)
			{
				foreach (MothershipInventoryItemSummary mothershipInventoryItemSummary in keyValuePair.Value.entitlements)
				{
					if (mothershipInventoryItemSummary.in_game_id.Equals("geodes", StringComparison.OrdinalIgnoreCase))
					{
						GeodeAtm.geodes += mothershipInventoryItemSummary.quantity;
					}
				}
			}
			this.UpdateStatusText("");
		}, delegate(MothershipError Error, int StatusCode)
		{
			GeodeAtm.fetchedGeodes = false;
			this.UpdateStatusText(GeodeAtm.GetLocalizedText("GEODE_ATM_BALANCE_ERROR", "Something went wrong getting current Geodes balance: {errorCode} Trace ID: {traceId} Session ID: {sessionId}").Replace("{errorCode}", Error.MothershipErrorCode ?? "").Replace("{traceId}", Error.TraceId ?? "")
				.Replace("{sessionId}", MothershipClientApiUnity.SessionId ?? ""));
			Debug.Log(string.Concat(new string[]
			{
				"Something went wrong refreshing inventory for geodes ",
				Error.Message,
				" ",
				Error.MothershipErrorCode,
				" Trace ID: ",
				Error.TraceId,
				" Session ID: ",
				MothershipClientApiUnity.SessionId
			}));
		});
	}

	// Token: 0x04002E8F RID: 11919
	private const string GEODE_ATM_PREFIX = "GEODE_ATM_";

	// Token: 0x04002E90 RID: 11920
	private const string BALANCE_KEY = "GEODE_ATM_BALANCE";

	// Token: 0x04002E91 RID: 11921
	private const string BALANCE_LOADING_KEY = "GEODE_ATM_BALANCE_LOADING";

	// Token: 0x04002E92 RID: 11922
	private const string BALANCE_ERROR_KEY = "GEODE_ATM_BALANCE_ERROR";

	// Token: 0x04002E93 RID: 11923
	private const string PURCHASE_IN_PROGRESS_KEY = "GEODE_ATM_PURCHASE_IN_PROGRESS";

	// Token: 0x04002E94 RID: 11924
	private const string PURCHASE_CANCELLED_KEY = "GEODE_ATM_PURCHASE_CANCELLED";

	// Token: 0x04002E95 RID: 11925
	private const string PURCHASE_ERROR_KEY = "GEODE_ATM_PURCHASE_ERROR";

	// Token: 0x04002E96 RID: 11926
	private const string PURCHASE_FINALIZE_ERROR_KEY = "GEODE_ATM_PURCHASE_FINALIZE_ERROR";

	// Token: 0x04002E97 RID: 11927
	private const string PURCHASE_META_ERROR_KEY = "GEODE_ATM_PURCHASE_META_ERROR";

	// Token: 0x04002E98 RID: 11928
	public static bool ProcessingGeodePurchase;

	// Token: 0x04002E99 RID: 11929
	[SerializeField]
	private List<TextMeshPro> StatusTexts;

	// Token: 0x04002E9A RID: 11930
	[Header("Global Storefront Info")]
	[SerializeField]
	private string MothershipGeodeOfferDisplayIdLive;

	// Token: 0x04002E9B RID: 11931
	[SerializeField]
	private string MothershipGeodeOfferDisplayIdDev;

	// Token: 0x04002E9C RID: 11932
	[Header("Small Geode Offer")]
	[SerializeField]
	private string QuestSmallSku;

	// Token: 0x04002E9D RID: 11933
	[SerializeField]
	private string RiftSmallSku;

	// Token: 0x04002E9E RID: 11934
	[SerializeField]
	private string MothershipSteamSmallGeodeOfferIdLive;

	// Token: 0x04002E9F RID: 11935
	[SerializeField]
	private string MothershipSteamSmallGeodeOfferIdDev;

	// Token: 0x04002EA0 RID: 11936
	[SerializeField]
	private int MothershipSmallSteamGeodeOfferDisplayIndexLive;

	// Token: 0x04002EA1 RID: 11937
	[SerializeField]
	private int MothershipSmallSteamGeodeOfferDisplayIndexDev;

	// Token: 0x04002EA2 RID: 11938
	[Header("Medium Geode Offer")]
	[SerializeField]
	private string QuestMediumSku;

	// Token: 0x04002EA3 RID: 11939
	[SerializeField]
	private string RiftMediumSku;

	// Token: 0x04002EA4 RID: 11940
	[SerializeField]
	private string MothershipSteamMediumGeodeOfferIdLive;

	// Token: 0x04002EA5 RID: 11941
	[SerializeField]
	private string MothershipSteamMediumGeodeOfferIdDev;

	// Token: 0x04002EA6 RID: 11942
	[SerializeField]
	private int MothershipMediumSteamGeodeOfferDisplayIndexLive;

	// Token: 0x04002EA7 RID: 11943
	[SerializeField]
	private int MothershipMediumSteamGeodeOfferDisplayIndexDev;

	// Token: 0x04002EA8 RID: 11944
	[Header("Large Geode Offer")]
	[SerializeField]
	private string QuestLargeSku;

	// Token: 0x04002EA9 RID: 11945
	[SerializeField]
	private string RiftLargeSku;

	// Token: 0x04002EAA RID: 11946
	[SerializeField]
	private string MothershipSteamLargeGeodeOfferIdLive;

	// Token: 0x04002EAB RID: 11947
	[SerializeField]
	private string MothershipSteamLargeGeodeOfferIdDev;

	// Token: 0x04002EAC RID: 11948
	[SerializeField]
	private int MothershipLargeSteamGeodeOfferDisplayIndexLive;

	// Token: 0x04002EAD RID: 11949
	[SerializeField]
	private int MothershipLargeSteamGeodeOfferDisplayIndexDev;

	// Token: 0x04002EAE RID: 11950
	private static bool fetchedGeodes;

	// Token: 0x04002EAF RID: 11951
	private static int geodes;

	// Token: 0x04002EB0 RID: 11952
	private string steamOrderId = "";

	// Token: 0x04002EB1 RID: 11953
	private Callback<MicroTxnAuthorizationResponse_t> _steamMicroTransactionAuthorizationResponse;

	// Token: 0x04002EB2 RID: 11954
	private bool purchaseInFlight;

	// Token: 0x02000572 RID: 1394
	private enum GeodePurchaseSize
	{
		// Token: 0x04002EB4 RID: 11956
		SMALL,
		// Token: 0x04002EB5 RID: 11957
		MEDIUM,
		// Token: 0x04002EB6 RID: 11958
		LARGE
	}
}
