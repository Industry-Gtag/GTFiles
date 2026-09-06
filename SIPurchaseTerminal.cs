using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x0200014E RID: 334
[DefaultExecutionOrder(500)]
public class SIPurchaseTerminal : MonoBehaviour, ITouchScreenStation
{
	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x060008C0 RID: 2240 RVA: 0x0002FEDC File Offset: 0x0002E0DC
	public SIScreenRegion ScreenRegion
	{
		get
		{
			return this.screenRegion;
		}
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x0002FEE4 File Offset: 0x0002E0E4
	private void OnEnable()
	{
		if (CosmeticsController.hasInstance)
		{
			this.DelayedOnEnable();
			return;
		}
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Combine(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.DelayedOnEnable));
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x0002FF14 File Offset: 0x0002E114
	private void DelayedOnEnable()
	{
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.DelayedOnEnable));
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnGetCurrency = (Action)Delegate.Combine(instance.OnGetCurrency, new Action(this.OnUpdateCurrencyBalance));
		this.OnUpdateCurrencyBalance();
		this.PopupBackgroundScreen.SetActive(false);
		this.ConfirmPurchasePopupScreen.SetActive(false);
		this.PendingPurchasePopupScreen.SetActive(false);
		this.PurchaseCompletePopupScreen.SetActive(false);
		this.InsufficientFundsPopupScreen.SetActive(false);
		this.UnableToCompletePurchasePopupScreen.SetActive(false);
		this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.PurchaseAmountSelection, true);
		this.purchaseSize = 1;
		this.UpdatePurchaseAmount();
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x0002FFCC File Offset: 0x0002E1CC
	private void OnDisable()
	{
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnGetCurrency = (Action)Delegate.Remove(instance.OnGetCurrency, new Action(this.OnUpdateCurrencyBalance));
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x0002FFF6 File Offset: 0x0002E1F6
	public void UpdateCurrentTechPoints()
	{
		this.PurchaseAmountCurrentTechPointsCount.text = SIPlayer.LocalPlayer.CurrentProgression.resourceArray[0].ToString();
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x0003001D File Offset: 0x0002E21D
	private void OnUpdateCurrencyBalance()
	{
		this.PurchaseAmountCurrentShinyRockCount.text = CosmeticsController.instance.currencyBalance.ToString().ToUpperInvariant();
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void AddButton(SITouchscreenButton button, bool isPopupButton = false)
	{
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x00030040 File Offset: 0x0002E240
	public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr)
	{
		switch (this.currentState)
		{
		case SIPurchaseTerminal.PurchaseTerminalState.PurchaseAmountSelection:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Purchase)
			{
				this.SelectPurchase();
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Next)
			{
				this.IncreasePurchase();
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.DecreasePurcahse();
				return;
			}
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.ConfirmPurchasePopup:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Confirm)
			{
				this.ConfirmPurchase();
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Cancel)
			{
				this.ReturnToBaseScreen();
				return;
			}
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.PendingPurchasePopup:
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.PurchaseCompletePopup:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Confirm)
			{
				this.ReturnToBaseScreen();
				return;
			}
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.InsufficientFundsPopup:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.ReturnToBaseScreen();
				return;
			}
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.UnableToCompletePurchasePopup:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.ReturnToBaseScreen();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void TouchscreenToggleButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr, bool isToggledOn)
	{
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x000300CB File Offset: 0x0002E2CB
	private void IncreasePurchase()
	{
		this.purchaseSize = Math.Min(this.purchaseSize + 1, this.maxPurchaseSize);
		this.UpdatePurchaseAmount();
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x000300EC File Offset: 0x0002E2EC
	private void DecreasePurcahse()
	{
		this.purchaseSize = Math.Max(this.purchaseSize - 1, this.minPurchaseSize);
		this.UpdatePurchaseAmount();
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x00030110 File Offset: 0x0002E310
	private void UpdatePurchaseAmount()
	{
		this.PurchaseAmountShinyRockCount.text = (this.purchaseSize * this.costPerTechPoint).ToString().ToUpperInvariant();
		this.PurchaseAmountTechPointCount.text = this.purchaseSize.ToString().ToUpperInvariant();
		this.ConfirmPurchaseShinyRockCount.text = (this.purchaseSize * this.costPerTechPoint).ToString().ToUpperInvariant();
		this.ConfirmPurchaseTechPointCount.text = this.purchaseSize.ToString().ToUpperInvariant();
		this.PurchasedTechPointCount.text = this.purchaseSize.ToString().ToUpperInvariant();
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x000301B8 File Offset: 0x0002E3B8
	private void SelectPurchase()
	{
		this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.ConfirmPurchasePopup, false);
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x000301C4 File Offset: 0x0002E3C4
	private void ConfirmPurchase()
	{
		int num = this.purchaseSize * this.costPerTechPoint;
		if (CosmeticsController.instance.currencyBalance < num)
		{
			this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.InsufficientFundsPopup, false);
			return;
		}
		this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.PendingPurchasePopup, false);
		ProgressionManager.Instance.PurchaseTechPoints(this.purchaseSize, delegate
		{
			SIProgression.Instance.SendPurchaseTechPointsData(this.purchaseSize);
			this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.PurchaseCompletePopup, false);
			ProgressionManager.Instance.RefreshUserInventory();
		}, delegate(string error)
		{
			Debug.LogError("[SIPurchaseTerminal] PurchaseTechPoints failed: " + error);
			this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.UnableToCompletePurchasePopup, false);
		});
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x00030227 File Offset: 0x0002E427
	private void ReturnToBaseScreen()
	{
		this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.PurchaseAmountSelection, false);
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x00030231 File Offset: 0x0002E431
	private void UpdateState(SIPurchaseTerminal.PurchaseTerminalState newState, bool forceUpdate = false)
	{
		if (!forceUpdate && this.currentState == newState)
		{
			return;
		}
		this.SetScreenVisibility(this.currentState, false);
		this.currentState = newState;
		this.SetScreenVisibility(this.currentState, true);
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x00030264 File Offset: 0x0002E464
	private void SetScreenVisibility(SIPurchaseTerminal.PurchaseTerminalState screenState, bool isEnabled)
	{
		switch (screenState)
		{
		case SIPurchaseTerminal.PurchaseTerminalState.ConfirmPurchasePopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.ConfirmPurchasePopupScreen.SetActive(isEnabled);
			return;
		case SIPurchaseTerminal.PurchaseTerminalState.PendingPurchasePopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.PendingPurchasePopupScreen.SetActive(isEnabled);
			return;
		case SIPurchaseTerminal.PurchaseTerminalState.PurchaseCompletePopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.PurchaseCompletePopupScreen.SetActive(isEnabled);
			return;
		case SIPurchaseTerminal.PurchaseTerminalState.InsufficientFundsPopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.InsufficientFundsPopupScreen.SetActive(isEnabled);
			return;
		case SIPurchaseTerminal.PurchaseTerminalState.UnableToCompletePurchasePopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.UnableToCompletePurchasePopupScreen.SetActive(isEnabled);
			return;
		default:
			return;
		}
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x000066D3 File Offset: 0x000048D3
	GameObject ITouchScreenStation.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04000AD7 RID: 2775
	private SIPurchaseTerminal.PurchaseTerminalState currentState;

	// Token: 0x04000AD8 RID: 2776
	[SerializeField]
	private SIScreenRegion screenRegion;

	// Token: 0x04000AD9 RID: 2777
	[SerializeField]
	private GameObject PopupBackgroundScreen;

	// Token: 0x04000ADA RID: 2778
	[SerializeField]
	private GameObject ConfirmPurchasePopupScreen;

	// Token: 0x04000ADB RID: 2779
	[SerializeField]
	private GameObject PurchaseCompletePopupScreen;

	// Token: 0x04000ADC RID: 2780
	[SerializeField]
	private GameObject PendingPurchasePopupScreen;

	// Token: 0x04000ADD RID: 2781
	[SerializeField]
	private GameObject InsufficientFundsPopupScreen;

	// Token: 0x04000ADE RID: 2782
	[SerializeField]
	private GameObject UnableToCompletePurchasePopupScreen;

	// Token: 0x04000ADF RID: 2783
	[SerializeField]
	private TextMeshProUGUI PurchaseAmountShinyRockCount;

	// Token: 0x04000AE0 RID: 2784
	[SerializeField]
	private TextMeshProUGUI PurchaseAmountTechPointCount;

	// Token: 0x04000AE1 RID: 2785
	[SerializeField]
	private TextMeshProUGUI PurchaseAmountCurrentShinyRockCount;

	// Token: 0x04000AE2 RID: 2786
	[SerializeField]
	private TextMeshProUGUI PurchaseAmountCurrentTechPointsCount;

	// Token: 0x04000AE3 RID: 2787
	[SerializeField]
	private TextMeshProUGUI ConfirmPurchaseShinyRockCount;

	// Token: 0x04000AE4 RID: 2788
	[SerializeField]
	private TextMeshProUGUI ConfirmPurchaseTechPointCount;

	// Token: 0x04000AE5 RID: 2789
	[SerializeField]
	private TextMeshProUGUI PurchasedTechPointCount;

	// Token: 0x04000AE6 RID: 2790
	[SerializeField]
	private int maxPurchaseSize = 10;

	// Token: 0x04000AE7 RID: 2791
	[SerializeField]
	private int minPurchaseSize = 1;

	// Token: 0x04000AE8 RID: 2792
	[SerializeField]
	private int costPerTechPoint = 100;

	// Token: 0x04000AE9 RID: 2793
	private int purchaseSize = 1;

	// Token: 0x0200014F RID: 335
	public enum PurchaseTerminalState
	{
		// Token: 0x04000AEB RID: 2795
		PurchaseAmountSelection,
		// Token: 0x04000AEC RID: 2796
		ConfirmPurchasePopup,
		// Token: 0x04000AED RID: 2797
		PendingPurchasePopup,
		// Token: 0x04000AEE RID: 2798
		PurchaseCompletePopup,
		// Token: 0x04000AEF RID: 2799
		InsufficientFundsPopup,
		// Token: 0x04000AF0 RID: 2800
		UnableToCompletePurchasePopup
	}
}
