using System;
using Steamworks;
using UnityEngine;

namespace GorillaTagScripts.Subscription;

public static class FanClubCheckout
{
	public enum Result
	{
		Success,
		Cancel,
		Failure
	}

	private static Action<Result> onComplete;

	private static Callback<MicroTxnAuthorizationResponse_t> steamAuthorizationCallback;

	private static string steamOrderId;

	public static bool InProgress { get; private set; }

	public static bool CanStart => !InProgress;

	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		InProgress = false;
		onComplete = null;
		steamAuthorizationCallback = null;
		steamOrderId = null;
	}

	public static bool Start(SubscriptionManager.SubscriptionTerm term, Action<Result> complete)
	{
		if (!CanStart)
		{
			return false;
		}
		InProgress = true;
		onComplete = complete;
		GetSteamTerm(term, out var frequencyUnit, out var frequency, out var priceInUSDCents);
		if (!SteamManager.Initialized)
		{
			Debug.LogError("FanClubCheckout :: Steam isn't initialized, so the purchase can't start");
			Finish(Result.Failure);
			return true;
		}
		if (steamAuthorizationCallback == null)
		{
			steamAuthorizationCallback = Callback<MicroTxnAuthorizationResponse_t>.Create(OnSteamAuthorization);
		}
		steamOrderId = null;
		SubscriptionKiosk.ProcessingSubscriptionPurchase = true;
		Debug.Log($"FanClubCheckout :: Starting Steam subscription purchase ({term})");
		MothershipClientApiUnity.InitSteamSubscriptionTransaction("40494", frequencyUnit, frequency, priceInUSDCents, delegate(InitSteamSubscriptionPurchaseResponse response)
		{
			steamOrderId = response.SteamOrderId;
		}, delegate(MothershipError error, int status)
		{
			Debug.LogError("FanClubCheckout :: Could not start the Steam purchase. Trace ID " + error.TraceId + ", Error Code: " + error.MothershipErrorCode);
			Finish(Result.Failure);
		});
		return true;
	}

	private static void GetSteamTerm(SubscriptionManager.SubscriptionTerm term, out string frequencyUnit, out int frequency, out int priceInUSDCents)
	{
		switch (term)
		{
		case SubscriptionManager.SubscriptionTerm.QUARTERLY:
			frequencyUnit = "Month";
			frequency = 3;
			priceInUSDCents = 2699;
			break;
		case SubscriptionManager.SubscriptionTerm.SEMIANNUAL:
			frequencyUnit = "Month";
			frequency = 6;
			priceInUSDCents = 4999;
			break;
		case SubscriptionManager.SubscriptionTerm.ANNUAL:
			frequencyUnit = "Year";
			frequency = 1;
			priceInUSDCents = 9499;
			break;
		default:
			frequencyUnit = "Month";
			frequency = 1;
			priceInUSDCents = 999;
			break;
		}
	}

	private static void OnSteamAuthorization(MicroTxnAuthorizationResponse_t response)
	{
		if (!InProgress || string.IsNullOrEmpty(steamOrderId))
		{
			return;
		}
		if (response.m_bAuthorized == 0)
		{
			Debug.Log("FanClubCheckout :: The player didn't authorize the Steam purchase");
			Finish(Result.Cancel);
			return;
		}
		MothershipClientApiUnity.FinalizeSteamSubscriptionTransaction(steamOrderId, delegate
		{
			Finish(Result.Success);
			SubscriptionManager.InitializePersonalSubscriptionData();
		}, delegate(MothershipError error, int status)
		{
			Debug.LogError("FanClubCheckout :: Could not finalize the Steam purchase. Trace ID " + error.TraceId + ", Error Code: " + error.MothershipErrorCode);
			Finish(Result.Failure);
		});
	}

	private static void Finish(Result result)
	{
		InProgress = false;
		SubscriptionKiosk.ProcessingSubscriptionPurchase = false;
		steamOrderId = null;
		Action<Result> action = onComplete;
		onComplete = null;
		action?.Invoke(result);
	}
}
