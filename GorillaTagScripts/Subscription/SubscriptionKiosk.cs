using System;
using System.Collections.Generic;
using GorillaNetworking;
using Oculus.Platform;
using Oculus.Platform.Models;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace GorillaTagScripts.Subscription
{
	// Token: 0x02000FF8 RID: 4088
	public class SubscriptionKiosk : MonoBehaviour, ITouchScreenStation, IGorillaSliceableSimple
	{
		// Token: 0x170009B1 RID: 2481
		// (get) Token: 0x060065A4 RID: 26020 RVA: 0x0020BB45 File Offset: 0x00209D45
		// (set) Token: 0x060065A5 RID: 26021 RVA: 0x0020BB4C File Offset: 0x00209D4C
		public static bool ProcessingSubscriptionPurchase { get; set; }

		// Token: 0x170009B2 RID: 2482
		// (get) Token: 0x060065A6 RID: 26022 RVA: 0x0020BB54 File Offset: 0x00209D54
		public SIScreenRegion ScreenRegion { get; }

		// Token: 0x060065A7 RID: 26023 RVA: 0x0020BB5C File Offset: 0x00209D5C
		private void Awake()
		{
			this.toggleButtonContainers = new List<SITouchscreenButtonContainer>(base.GetComponentsInChildren<SITouchscreenButtonContainer>(true));
			for (int i = this.toggleButtonContainers.Count - 1; i >= 0; i--)
			{
				if (this.toggleButtonContainers[i].button.buttonMode != SITouchscreenButton.ButtonMode.Toggle)
				{
					this.toggleButtonContainers.RemoveAt(i);
				}
			}
			this.screensByState = new Dictionary<SubscriptionKiosk.ScreenState, GameObject>();
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SafeAccount, this.safeAccountScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.WaitingForScan, this.waitingForScanScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.Scanning, this.scanningScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SubscriptionStatusUnknown, this.subStatusUnknownScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.MainMenuSubscribed, this.mainMenuSubscribedScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.MainMenuUnsubscribed, this.mainMenuUnsubscribedScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SubscriptionData, this.subDataScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.PurchaseSubscription, this.purchaseSubScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SubscriptionPurchaseInProgress, this.purchaseProgressScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult, this.purchaseResultScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.FeatureToggles, this.featureTogglesScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SubscriptionSteamWarning, this.steamComingSoon);
		}

		// Token: 0x060065A8 RID: 26024 RVA: 0x0020BCA0 File Offset: 0x00209EA0
		private void OnEnable()
		{
			if (PlayFabAuthenticator.instance.GetSafety())
			{
				this.UpdateState(SubscriptionKiosk.ScreenState.SafeAccount);
				Object.Destroy(this);
				return;
			}
			this.UpdateState(SubscriptionKiosk.ScreenState.WaitingForScan);
			this.subsVideoPlayer.clip = this.defaultVideoClip;
			GorillaSlicerSimpleManager.RegisterSliceable(this);
			SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionData, new Action(this.LocalSubscriptionDataUpdated));
		}

		// Token: 0x060065A9 RID: 26025 RVA: 0x0020BD07 File Offset: 0x00209F07
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
			SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.LocalSubscriptionDataUpdated));
			Callback<MicroTxnAuthorizationResponse_t> steamMicroTransactionAuthorizationResponse = this._steamMicroTransactionAuthorizationResponse;
			if (steamMicroTransactionAuthorizationResponse == null)
			{
				return;
			}
			steamMicroTransactionAuthorizationResponse.Unregister();
		}

		// Token: 0x060065AA RID: 26026 RVA: 0x0020BD40 File Offset: 0x00209F40
		public void HandScanAborted()
		{
			if (this.currentState == SubscriptionKiosk.ScreenState.Scanning)
			{
				this.UpdateState(SubscriptionKiosk.ScreenState.WaitingForScan);
			}
		}

		// Token: 0x060065AB RID: 26027 RVA: 0x0020BD52 File Offset: 0x00209F52
		public void KioskAbandoned()
		{
			this.UpdateState(SubscriptionKiosk.ScreenState.WaitingForScan);
		}

		// Token: 0x060065AC RID: 26028 RVA: 0x0020BD5B File Offset: 0x00209F5B
		public void HandScanStarted()
		{
			if (this.currentState == SubscriptionKiosk.ScreenState.WaitingForScan)
			{
				this.UpdateState(SubscriptionKiosk.ScreenState.Scanning);
			}
		}

		// Token: 0x060065AD RID: 26029 RVA: 0x0020BD70 File Offset: 0x00209F70
		public void HandScanned()
		{
			if (PlayFabAuthenticator.instance.GetSafety())
			{
				return;
			}
			SubscriptionManager.SubscriptionStatus subscriptionStatus = SubscriptionManager.LocalSubscriptionStatus();
			if (subscriptionStatus == SubscriptionManager.SubscriptionStatus.Active)
			{
				this.UpdateState(SubscriptionKiosk.ScreenState.MainMenuSubscribed);
				return;
			}
			if (subscriptionStatus == SubscriptionManager.SubscriptionStatus.Inactive)
			{
				this.UpdateState(SubscriptionKiosk.ScreenState.MainMenuUnsubscribed);
				return;
			}
			this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionStatusUnknown);
		}

		// Token: 0x060065AE RID: 26030 RVA: 0x0020BDB0 File Offset: 0x00209FB0
		private void UpdateState(SubscriptionKiosk.ScreenState newState)
		{
			this.lastState = this.currentState;
			this.currentState = newState;
			if (this.lastState == this.currentState)
			{
				return;
			}
			this.ActivateScreen(this.currentState);
			switch (this.currentState)
			{
			case SubscriptionKiosk.ScreenState.WaitingForScan:
			case SubscriptionKiosk.ScreenState.Scanning:
			case SubscriptionKiosk.ScreenState.SubscriptionStatusUnknown:
			case SubscriptionKiosk.ScreenState.PurchaseSubscription:
			case SubscriptionKiosk.ScreenState.SubscriptionPurchaseInProgress:
			case SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult:
				break;
			case SubscriptionKiosk.ScreenState.MainMenuSubscribed:
				this.UpdateSubscribedMenu();
				return;
			case SubscriptionKiosk.ScreenState.MainMenuUnsubscribed:
				this.UpdateUnsubscribedMenu();
				return;
			case SubscriptionKiosk.ScreenState.SubscriptionData:
				this.UpdateSubscriptionData();
				return;
			case SubscriptionKiosk.ScreenState.FeatureToggles:
			{
				FeatureTogglesScreen component = this.screensByState[SubscriptionKiosk.ScreenState.FeatureToggles].GetComponent<FeatureTogglesScreen>();
				if (component != null)
				{
					component.enabled = true;
					component.MarkDirty();
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x060065AF RID: 26031 RVA: 0x0020BE5C File Offset: 0x0020A05C
		private void ActivateScreen(SubscriptionKiosk.ScreenState activeScreen)
		{
			foreach (KeyValuePair<SubscriptionKiosk.ScreenState, GameObject> keyValuePair in this.screensByState)
			{
				keyValuePair.Value.SetActive(keyValuePair.Key == activeScreen);
			}
		}

		// Token: 0x060065B0 RID: 26032 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void AddButton(SITouchscreenButton button, bool isPopupButton = false)
		{
		}

		// Token: 0x060065B1 RID: 26033 RVA: 0x0020BEC0 File Offset: 0x0020A0C0
		public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr)
		{
			if (actorNr != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return;
			}
			switch (this.currentState)
			{
			case SubscriptionKiosk.ScreenState.MainMenuSubscribed:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionData);
					return;
				}
				if (buttonType != SITouchscreenButton.SITouchscreenButtonType.PageSelect)
				{
					return;
				}
				this.UpdateState(SubscriptionKiosk.ScreenState.FeatureToggles);
				return;
			case SubscriptionKiosk.ScreenState.MainMenuUnsubscribed:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Subscribe)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionSteamWarning);
					this.subsVideoPlayer.clip = this.steamSubsVideoClip;
					this.subsVideoObservable.ObservableBehaviorRule = this.steamObservableRule;
					return;
				}
				break;
			case SubscriptionKiosk.ScreenState.SubscriptionData:
				if (buttonType != SITouchscreenButton.SITouchscreenButtonType.Cancel)
				{
					if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
					{
						this.HandScanned();
						return;
					}
					if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Subscribe)
					{
						this.UpdateState(SubscriptionKiosk.ScreenState.PurchaseSubscription);
						return;
					}
				}
				break;
			case SubscriptionKiosk.ScreenState.PurchaseSubscription:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Subscribe)
				{
					this.PurchaseSubscription((SubscriptionManager.SubscriptionTerm)data);
				}
				else if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
				{
					this.HandScanned();
				}
				this.subsVideoPlayer.clip = this.defaultVideoClip;
				this.subsVideoObservable.ObservableBehaviorRule = this.defaultObservableRule;
				return;
			case SubscriptionKiosk.ScreenState.SubscriptionPurchaseInProgress:
			case SubscriptionKiosk.ScreenState.FeatureToggles:
				break;
			case SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Confirm)
				{
					this.HandScanned();
				}
				break;
			case SubscriptionKiosk.ScreenState.SubscriptionSteamWarning:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Subscribe)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.PurchaseSubscription);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060065B2 RID: 26034 RVA: 0x0020BFCC File Offset: 0x0020A1CC
		public void TouchscreenToggleButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr, bool isToggledOn)
		{
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}

		// Token: 0x060065B3 RID: 26035 RVA: 0x0020BFE0 File Offset: 0x0020A1E0
		public void OnToggleFeaturesExitButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr)
		{
			if (actorNr != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return;
			}
			this.UpdateState(SubscriptionKiosk.ScreenState.MainMenuSubscribed);
		}

		// Token: 0x060065B4 RID: 26036 RVA: 0x0020BFFC File Offset: 0x0020A1FC
		private void UpdateToggleButtonState(int buttonData, bool state)
		{
			foreach (SITouchscreenButtonContainer sitouchscreenButtonContainer in this.toggleButtonContainers)
			{
				if (sitouchscreenButtonContainer.button.data == buttonData)
				{
					sitouchscreenButtonContainer.button.SetToggleState(state, true);
					break;
				}
			}
		}

		// Token: 0x060065B5 RID: 26037 RVA: 0x0020C068 File Offset: 0x0020A268
		private bool GetSubscriptionFeatureState(int buttonData)
		{
			switch (buttonData)
			{
			case 0:
				return SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.GoldenName);
			case 1:
				return SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.IOBT);
			case 2:
				return SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.HandTracking);
			default:
				Debug.Log(string.Format("Getting current state for subscription kiosk {0}", buttonData));
				return false;
			}
		}

		// Token: 0x060065B6 RID: 26038 RVA: 0x0020C0B4 File Offset: 0x0020A2B4
		public void UpdateGoldNameTag(bool state)
		{
			this.ToggleSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures.GoldenName, state);
			VRRig.LocalRig.OnSubscriptionData();
			if (GorillaScoreboardTotalUpdater.instance != null)
			{
				GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
			}
		}

		// Token: 0x060065B7 RID: 26039 RVA: 0x0020C0DF File Offset: 0x0020A2DF
		public void UpdateIOTBExperimentalFeature(bool state)
		{
			this.ToggleSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures.IOBT, state);
			if (GorillaIK.playerIK != null)
			{
				GorillaIK.playerIK.DelayedUpdateIK(state);
			}
		}

		// Token: 0x060065B8 RID: 26040 RVA: 0x0020C101 File Offset: 0x0020A301
		public void UpdateHandTrackingExperimentalFeature(bool state)
		{
			this.ToggleSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures.HandTracking, state);
		}

		// Token: 0x060065B9 RID: 26041 RVA: 0x0020C10B File Offset: 0x0020A30B
		private void ToggleSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures feature, bool state)
		{
			SubscriptionManager.SetSubscriptionSettingValue(feature, state ? 1 : 0);
		}

		// Token: 0x060065BA RID: 26042 RVA: 0x0020C11C File Offset: 0x0020A31C
		private void UpdateSubscribedMenu()
		{
			this.subMenuPlayerName.text = NetworkSystem.Instance.LocalPlayer.SanitizedNickName;
			this.subMenuDaysAccrued.text = SubscriptionManager.GetSubscriptionDetails().daysAccrued.ToString();
			foreach (SITouchscreenButtonContainer sitouchscreenButtonContainer in this.toggleButtonContainers)
			{
				this.UpdateToggleButtonState(sitouchscreenButtonContainer.data, this.GetSubscriptionFeatureState(sitouchscreenButtonContainer.data));
			}
		}

		// Token: 0x060065BB RID: 26043 RVA: 0x0020C1B8 File Offset: 0x0020A3B8
		private void UpdateUnsubscribedMenu()
		{
			this.unsubscribedMenuPlayerName.text = NetworkSystem.Instance.LocalPlayer.SanitizedNickName;
			this.mainMenuUnsubscribedQuestText.SetActive(false);
			this.mainMenuUnsubscribedSteamText.SetActive(true);
			this.subsVideoPlayer.clip = this.defaultVideoClip;
			this.subsVideoObservable.ObservableBehaviorRule = this.defaultObservableRule;
		}

		// Token: 0x060065BC RID: 26044 RVA: 0x0020C21C File Offset: 0x0020A41C
		private void UpdateSubscriptionData()
		{
			SubscriptionManager.SubscriptionDetails subscriptionDetails = SubscriptionManager.GetSubscriptionDetails();
			this.subDataPlayerName.text = NetworkSystem.Instance.LocalPlayer.SanitizedNickName;
			this.subDataDaysAccrued.text = subscriptionDetails.daysAccrued.ToString();
			this.subDataDaysRemaining.text = Mathf.RoundToInt((float)(subscriptionDetails.subscriptionActiveUntilDate - DateTime.UtcNow).TotalDays).ToString();
			this.subDataAutoRenew.text = (subscriptionDetails.autoRenew ? "ENABLED" : "DISABLED");
			this.subDataRenewDate.text = subscriptionDetails.subscriptionActiveUntilDate.ToString("MMM d, yyyy").ToUpper();
			this.subDataSubscriptionTerm.text = subscriptionDetails.autoRenewMonths.ToString() + " MONTH" + ((subscriptionDetails.autoRenewMonths > 1) ? "S" : "");
			if (this.subDataSubscribeButton.activeSelf == subscriptionDetails.autoRenew)
			{
				this.subDataSubscribeButton.SetActive(!subscriptionDetails.autoRenew);
			}
		}

		// Token: 0x060065BD RID: 26045 RVA: 0x0020C330 File Offset: 0x0020A530
		private void UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult result)
		{
			this.lastPurchase = result;
			string text = "";
			if (result == SubscriptionKiosk.PurchaseResult.Success)
			{
				text = "SUBSCRIPTION SUCCESSFUL! WELCOME TO THE FAN CLUB, YOU ARE NOW A VERY IMPORTANT MONKE (V.I.M.)!";
				LocalisationManager.TryGetKeyForCurrentLocale("SUBKIOSKPURCHASE_SUCCESS", out text, text);
			}
			else if (result == SubscriptionKiosk.PurchaseResult.Failure)
			{
				text = "PURCHASE FAILED! WE'RE NOT SURE WHAT HAPPENED, BUT PLEASE CHECK YOUR INFORMATION, OR TRY AGAIN LATER. IF IT LOOKED LIKE THE PURCHASE SHOULD HAVE SUCCEEDED, TRY RESTARTING THE GAME.";
				LocalisationManager.TryGetKeyForCurrentLocale("SUBKIOSKPURCHASE_FAIL", out text, text);
			}
			else if (result == SubscriptionKiosk.PurchaseResult.Cancel)
			{
				text = "PURCHASE CANCELED! WE'LL BE HERE IF YOU CHANGE YOUR MIND!";
				LocalisationManager.TryGetKeyForCurrentLocale("SUBKIOSKPURCHASE_CANCEL", out text, text);
			}
			this.purchaseResultText.text = text;
		}

		// Token: 0x060065BE RID: 26046 RVA: 0x0020C3A4 File Offset: 0x0020A5A4
		private void ProcessSteamCallback(MicroTxnAuthorizationResponse_t callBackResponse)
		{
			if (callBackResponse.m_bAuthorized == 0)
			{
				Debug.Log("The user did not authorize the steam subscription purchase");
				this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Cancel);
				this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult);
			}
			MothershipClientApiUnity.FinalizeSteamSubscriptionTransaction(this.steamOrderId, delegate(FinalizeSteamSubscriptionPurchaseResponse Response)
			{
				SubscriptionKiosk.ProcessingSubscriptionPurchase = false;
				this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Success);
				this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult);
				SubscriptionManager.InitializePersonalSubscriptionData();
			}, delegate(MothershipError Error, int Status)
			{
				SubscriptionKiosk.ProcessingSubscriptionPurchase = false;
				this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Failure);
				this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult);
				Debug.LogError("SubscriptionKiosk could not finalzie STEAM iap. Trace ID " + Error.TraceId + ", Error Code: " + Error.MothershipErrorCode);
			});
		}

		// Token: 0x060065BF RID: 26047 RVA: 0x0020C3F8 File Offset: 0x0020A5F8
		private void PurchaseSubscription(SubscriptionManager.SubscriptionTerm subTerm)
		{
			if (SteamManager.Initialized && this._steamMicroTransactionAuthorizationResponse == null)
			{
				this._steamMicroTransactionAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(new Callback<MicroTxnAuthorizationResponse_t>.DispatchDelegate(this.ProcessSteamCallback));
			}
			Debug.Log("Starting Steam Subscription Purchase");
			int num = 1;
			int num2 = 999;
			string text = "Month";
			switch (subTerm)
			{
			case SubscriptionManager.SubscriptionTerm.MONTHLY:
				num = 1;
				num2 = 999;
				text = "Month";
				break;
			case SubscriptionManager.SubscriptionTerm.QUARTERLY:
				num = 3;
				num2 = 2699;
				text = "Month";
				break;
			case SubscriptionManager.SubscriptionTerm.SEMIANNUAL:
				num = 6;
				num2 = 4999;
				text = "Month";
				break;
			case SubscriptionManager.SubscriptionTerm.ANNUAL:
				num = 1;
				num2 = 9499;
				text = "Year";
				break;
			}
			SubscriptionKiosk.ProcessingSubscriptionPurchase = true;
			MothershipClientApiUnity.InitSteamSubscriptionTransaction("40494", text, num, num2, delegate(InitSteamSubscriptionPurchaseResponse Response)
			{
				this.steamOrderId = Response.SteamOrderId;
			}, delegate(MothershipError Error, int Status)
			{
				this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Failure);
				this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult);
				Debug.LogError("SubscriptionKiosk could not start STEAM iap. Trace ID " + Error.TraceId + ", Error Code: " + Error.MothershipErrorCode);
				SubscriptionKiosk.ProcessingSubscriptionPurchase = false;
			});
			this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionPurchaseInProgress);
		}

		// Token: 0x060065C0 RID: 26048 RVA: 0x0020C4CC File Offset: 0x0020A6CC
		public void LaunchCheckoutFlowCallback(Message<Purchase> msg)
		{
			Debug.Log(string.Format("SubscriptionKiosk Purchase result: {0}   isError: {1}   Data: {2}", msg.Type, msg.IsError, msg.Data.ToString()));
			if (msg.IsError)
			{
				Error error = msg.GetError();
				if (error != null && error.Message != null && error.Message.Contains("cancel"))
				{
					this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Cancel);
					return;
				}
				this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Failure);
			}
			else
			{
				Purchase purchase = msg.GetPurchase();
				if (purchase != null && !string.IsNullOrEmpty(purchase.Sku))
				{
					this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Success);
				}
				else
				{
					this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Failure);
				}
			}
			SubscriptionManager.InitializePersonalSubscriptionData();
			this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult);
		}

		// Token: 0x060065C1 RID: 26049 RVA: 0x0020C57C File Offset: 0x0020A77C
		public void LocalSubscriptionDataUpdated()
		{
			SubscriptionManager.SubscriptionDetails subscriptionDetails = SubscriptionManager.LocalSubscriptionDetails();
			if (subscriptionDetails.active)
			{
				if (this.lastPurchase == SubscriptionKiosk.PurchaseResult.Failure)
				{
					this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Success);
				}
				if (this.currentState == SubscriptionKiosk.ScreenState.MainMenuUnsubscribed)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.MainMenuSubscribed);
				}
				if (this.currentState == SubscriptionKiosk.ScreenState.PurchaseSubscription && subscriptionDetails.autoRenew)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionData);
				}
			}
			this.subsVideoPlayer.clip = this.defaultVideoClip;
		}

		// Token: 0x060065C2 RID: 26050 RVA: 0x0020C5E0 File Offset: 0x0020A7E0
		private void UpdateSubsVideo()
		{
			if (SubscriptionManager.IsLocalSubscribed())
			{
				this.subsVideoObservable.ObservableBehaviorRule = this.defaultObservableRule;
				this.subsVideoPlayer.clip = this.defaultVideoClip;
				return;
			}
			this.subsVideoPlayer.clip = this.steamSubsVideoClip;
			this.subsVideoObservable.ObservableBehaviorRule = this.steamObservableRule;
		}

		// Token: 0x060065C3 RID: 26051 RVA: 0x0020C63C File Offset: 0x0020A83C
		public void SliceUpdate()
		{
			if (this.currentState == SubscriptionKiosk.ScreenState.SubscriptionStatusUnknown)
			{
				this.HandScanned();
			}
			if (VRRig.LocalRig == null)
			{
				return;
			}
			Vector3 vector = VRRig.LocalRig.transform.position - base.transform.position;
			float sqrMagnitude = vector.sqrMagnitude;
			float num = Vector3.Dot(base.transform.forward, vector.normalized);
			if (this.subsVideoPlayer.enabled)
			{
				if (num < 0f && sqrMagnitude >= this.videoViewableDist * this.videoViewableDist)
				{
					this.subsVideoPlayer.enabled = false;
					return;
				}
			}
			else if (num > 0f && sqrMagnitude < this.videoViewableDist * this.videoViewableDist)
			{
				this.subsVideoPlayer.enabled = true;
				this.subsVideoPlayer.Play();
			}
		}

		// Token: 0x060065C4 RID: 26052 RVA: 0x0020C707 File Offset: 0x0020A907
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(base.transform.position, this.videoViewableDist);
		}

		// Token: 0x060065C6 RID: 26054 RVA: 0x000066D3 File Offset: 0x000048D3
		GameObject ITouchScreenStation.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040074A5 RID: 29861
		private const string SUBSCRIPTION_KIOSK_PREFIX = "SUBKIOSK";

		// Token: 0x040074A6 RID: 29862
		private const string PURCHASE_SUCCESS_KEY = "SUBKIOSKPURCHASE_SUCCESS";

		// Token: 0x040074A7 RID: 29863
		private const string PURCHASE_CANCEL_KEY = "SUBKIOSKPURCHASE_CANCEL";

		// Token: 0x040074A8 RID: 29864
		private const string PURCHASE_FAIL_KEY = "SUBKIOSKPURCHASE_FAIL";

		// Token: 0x040074A9 RID: 29865
		private const string subSKU = "fan_club";

		// Token: 0x040074AA RID: 29866
		[Space]
		[SerializeField]
		private VideoPlayer subsVideoPlayer;

		// Token: 0x040074AB RID: 29867
		[SerializeField]
		private ObservableBehavior subsVideoObservable;

		// Token: 0x040074AC RID: 29868
		[SerializeField]
		private VideoClip defaultVideoClip;

		// Token: 0x040074AD RID: 29869
		[SerializeField]
		private VideoClip steamSubsVideoClip;

		// Token: 0x040074AE RID: 29870
		[SerializeField]
		private float videoViewableDist = 15f;

		// Token: 0x040074AF RID: 29871
		[SerializeField]
		private ObservableBehaviorRule defaultObservableRule;

		// Token: 0x040074B0 RID: 29872
		[SerializeField]
		private ObservableBehaviorRule steamObservableRule;

		// Token: 0x040074B1 RID: 29873
		[Space]
		[SerializeField]
		private GameObject steamComingSoon;

		// Token: 0x040074B2 RID: 29874
		[SerializeField]
		private GameObject safeAccountScreen;

		// Token: 0x040074B3 RID: 29875
		[SerializeField]
		private GameObject waitingForScanScreen;

		// Token: 0x040074B4 RID: 29876
		[SerializeField]
		private GameObject scanningScreen;

		// Token: 0x040074B5 RID: 29877
		[SerializeField]
		private GameObject subStatusUnknownScreen;

		// Token: 0x040074B6 RID: 29878
		[SerializeField]
		private GameObject mainMenuSubscribedScreen;

		// Token: 0x040074B7 RID: 29879
		[Space]
		[SerializeField]
		private GameObject mainMenuUnsubscribedScreen;

		// Token: 0x040074B8 RID: 29880
		[SerializeField]
		private GameObject mainMenuUnsubscribedQuestText;

		// Token: 0x040074B9 RID: 29881
		[SerializeField]
		private GameObject mainMenuUnsubscribedSteamText;

		// Token: 0x040074BA RID: 29882
		[Space]
		[SerializeField]
		private GameObject subDataScreen;

		// Token: 0x040074BB RID: 29883
		[SerializeField]
		private GameObject purchaseSubScreen;

		// Token: 0x040074BC RID: 29884
		[SerializeField]
		private GameObject purchaseProgressScreen;

		// Token: 0x040074BD RID: 29885
		[SerializeField]
		private GameObject purchaseResultScreen;

		// Token: 0x040074BE RID: 29886
		[SerializeField]
		private GameObject featureTogglesScreen;

		// Token: 0x040074C1 RID: 29889
		private List<SITouchscreenButtonContainer> toggleButtonContainers;

		// Token: 0x040074C2 RID: 29890
		private Dictionary<SubscriptionKiosk.ScreenState, GameObject> screensByState;

		// Token: 0x040074C3 RID: 29891
		private string steamOrderId = "";

		// Token: 0x040074C4 RID: 29892
		[SerializeField]
		private TextMeshPro subMenuPlayerName;

		// Token: 0x040074C5 RID: 29893
		[SerializeField]
		private TextMeshPro subMenuDaysAccrued;

		// Token: 0x040074C6 RID: 29894
		[SerializeField]
		private TextMeshPro unsubscribedMenuPlayerName;

		// Token: 0x040074C7 RID: 29895
		[SerializeField]
		private TextMeshPro subDataPlayerName;

		// Token: 0x040074C8 RID: 29896
		[SerializeField]
		private TextMeshPro subDataDaysAccrued;

		// Token: 0x040074C9 RID: 29897
		[SerializeField]
		private TextMeshPro subDataDaysRemaining;

		// Token: 0x040074CA RID: 29898
		[SerializeField]
		private TextMeshPro subDataAutoRenew;

		// Token: 0x040074CB RID: 29899
		[SerializeField]
		private TextMeshPro subDataRenewDate;

		// Token: 0x040074CC RID: 29900
		[SerializeField]
		private TextMeshPro subDataSubscriptionTerm;

		// Token: 0x040074CD RID: 29901
		[SerializeField]
		private GameObject subDataSubscribeButton;

		// Token: 0x040074CE RID: 29902
		[SerializeField]
		private TextMeshPro purchaseResultText;

		// Token: 0x040074CF RID: 29903
		private SubscriptionKiosk.ScreenState currentState = SubscriptionKiosk.ScreenState.WaitingForScan;

		// Token: 0x040074D0 RID: 29904
		private SubscriptionKiosk.ScreenState lastState;

		// Token: 0x040074D1 RID: 29905
		private SubscriptionKiosk.PurchaseResult lastPurchase;

		// Token: 0x040074D2 RID: 29906
		private Callback<MicroTxnAuthorizationResponse_t> _steamMicroTransactionAuthorizationResponse;

		// Token: 0x02000FF9 RID: 4089
		private enum ScreenState
		{
			// Token: 0x040074D4 RID: 29908
			SafeAccount,
			// Token: 0x040074D5 RID: 29909
			WaitingForScan,
			// Token: 0x040074D6 RID: 29910
			Scanning,
			// Token: 0x040074D7 RID: 29911
			SubscriptionStatusUnknown,
			// Token: 0x040074D8 RID: 29912
			MainMenuSubscribed,
			// Token: 0x040074D9 RID: 29913
			MainMenuUnsubscribed,
			// Token: 0x040074DA RID: 29914
			SubscriptionData,
			// Token: 0x040074DB RID: 29915
			PurchaseSubscription,
			// Token: 0x040074DC RID: 29916
			SubscriptionPurchaseInProgress,
			// Token: 0x040074DD RID: 29917
			SubscriptionPurchaseResult,
			// Token: 0x040074DE RID: 29918
			FeatureToggles,
			// Token: 0x040074DF RID: 29919
			SubscriptionSteamWarning,
			// Token: 0x040074E0 RID: 29920
			None
		}

		// Token: 0x02000FFA RID: 4090
		private enum PurchaseResult
		{
			// Token: 0x040074E2 RID: 29922
			Success,
			// Token: 0x040074E3 RID: 29923
			Failure,
			// Token: 0x040074E4 RID: 29924
			Cancel
		}
	}
}
