using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameObjectScheduling;
using GorillaNetworking;
using GorillaTagScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

// Token: 0x02000634 RID: 1588
public class BuilderKiosk : MonoBehaviour
{
	// Token: 0x06002796 RID: 10134 RVA: 0x000D1794 File Offset: 0x000CF994
	private void Awake()
	{
		BuilderKiosk.nullItem = new BuilderSetManager.BuilderSetStoreItem
		{
			displayName = "NOTHING",
			playfabID = "NULL",
			isNullItem = true
		};
	}

	// Token: 0x06002797 RID: 10135 RVA: 0x000D17D0 File Offset: 0x000CF9D0
	private async void Start()
	{
		this._puchaseTextLocStr = this._puchaseTextLoc.StringReference;
		this._itemNameVar = this._puchaseTextLocStr["item-name"] as StringVariable;
		this._itemCostVar = this._puchaseTextLocStr["item-cost"] as IntVariable;
		this._currencyBalanceVar = this._puchaseTextLocStr["currency-balance"] as IntVariable;
		this._finalLineVar = this._puchaseTextLocStr["final-line-index"] as IntVariable;
		this.purchaseParticles.Stop();
		BuilderSetManager.instance.OnOwnedSetsUpdated.AddListener(new UnityAction(this.OnOwnedSetsUpdated));
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnGetCurrency = (Action)Delegate.Combine(instance.OnGetCurrency, new Action(this.OnUpdateCurrencyBalance));
		this.leftPurchaseButton.onPressed += this.PressLeftPurchaseItemButton;
		this.rightPurchaseButton.onPressed += this.PressRightPurchaseItemButton;
		BuilderTable builderTable;
		if (BuilderTable.TryGetBuilderTableForZone(GTZone.monkeBlocks, out builderTable))
		{
			builderTable.OnTableConfigurationUpdated.AddListener(new UnityAction(this.UpdateCountdown));
		}
		this.UpdateCountdown();
		this.availableItems.Clear();
		if (this.isMiniKiosk)
		{
			this.availableItems.Add(this.pieceSetForSale);
		}
		else
		{
			this.availableItems.AddRange(BuilderSetManager.instance.GetPermanentSetsForSale());
			this.availableItems.AddRange(BuilderSetManager.instance.GetSeasonalSetsForSale());
		}
		if (!this.isMiniKiosk)
		{
			this.SetupSetButtons();
		}
		await Task.Delay(TimeSpan.FromSeconds(5.0));
		Debug.Log("Task await complete.");
		if (this.availableItems.Count > 0 && BuilderSetManager.instance.pulledStoreItems)
		{
			this.hasInitFromPlayfab = true;
			if (this.pieceSetForSale != null)
			{
				this.itemToBuy = BuilderSetManager.instance.GetStoreItemFromSetID(this.pieceSetForSale.GetIntIdentifier());
				this.UpdateLabels();
				this.UpdateDiorama();
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.ProcessPurchaseItemState(null, false);
			}
			else
			{
				this.itemToBuy = BuilderKiosk.nullItem;
				this.UpdateLabels();
				this.UpdateDiorama();
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				this.ProcessPurchaseItemState(null, false);
			}
		}
		else
		{
			this.itemToBuy = BuilderKiosk.nullItem;
		}
	}

	// Token: 0x06002798 RID: 10136 RVA: 0x000D1808 File Offset: 0x000CFA08
	private void UpdateCountdown()
	{
		if (!this.useTitleCountDown)
		{
			return;
		}
		if (!string.IsNullOrEmpty(BuilderTable.nextUpdateOverride) && !BuilderTable.nextUpdateOverride.Equals(this.countdownOverride))
		{
			this.countdownOverride = BuilderTable.nextUpdateOverride;
			CountdownTextDate countdown = this.countdownText.Countdown;
			countdown.CountdownTo = this.countdownOverride;
			this.countdownText.Countdown = countdown;
		}
	}

	// Token: 0x06002799 RID: 10137 RVA: 0x000D186C File Offset: 0x000CFA6C
	private void SetupSetButtons()
	{
		this.setsPerPage = this.setButtons.Length;
		this.totalPages = this.availableItems.Count / this.setsPerPage;
		if (this.availableItems.Count % this.setsPerPage > 0)
		{
			this.totalPages++;
		}
		this.previousPageButton.gameObject.SetActive(this.totalPages > 1);
		this.nextPageButton.gameObject.SetActive(this.totalPages > 1);
		this.previousPageButton.myTmpText.enabled = this.totalPages > 1;
		this.nextPageButton.myTmpText.enabled = this.totalPages > 1;
		this.previousPageButton.onPressButton.AddListener(new UnityAction(this.OnPreviousPageClicked));
		this.nextPageButton.onPressButton.AddListener(new UnityAction(this.OnNextPageClicked));
		GorillaPressableButton[] array = this.setButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].onPressed += this.OnSetButtonPressed;
		}
		this.UpdateLabels();
	}

	// Token: 0x0600279A RID: 10138 RVA: 0x000D1990 File Offset: 0x000CFB90
	private void OnDestroy()
	{
		if (this.leftPurchaseButton != null)
		{
			this.leftPurchaseButton.onPressed -= this.PressLeftPurchaseItemButton;
		}
		if (this.rightPurchaseButton != null)
		{
			this.rightPurchaseButton.onPressed -= this.PressRightPurchaseItemButton;
		}
		if (BuilderSetManager.instance != null)
		{
			BuilderSetManager.instance.OnOwnedSetsUpdated.RemoveListener(new UnityAction(this.OnOwnedSetsUpdated));
		}
		if (CosmeticsController.instance != null)
		{
			CosmeticsController instance = CosmeticsController.instance;
			instance.OnGetCurrency = (Action)Delegate.Remove(instance.OnGetCurrency, new Action(this.OnUpdateCurrencyBalance));
		}
		if (!this.isMiniKiosk)
		{
			foreach (GorillaPressableButton gorillaPressableButton in this.setButtons)
			{
				if (gorillaPressableButton != null)
				{
					gorillaPressableButton.onPressed -= this.OnSetButtonPressed;
				}
			}
			if (this.previousPageButton != null)
			{
				this.previousPageButton.onPressButton.RemoveListener(new UnityAction(this.OnPreviousPageClicked));
			}
			if (this.nextPageButton != null)
			{
				this.nextPageButton.onPressButton.RemoveListener(new UnityAction(this.OnNextPageClicked));
			}
		}
		if (this.currentDiorama != null)
		{
			Object.Destroy(this.currentDiorama);
			this.currentDiorama = null;
		}
		if (this.nextDiorama != null)
		{
			Object.Destroy(this.nextDiorama);
			this.nextDiorama = null;
		}
		BuilderTable builderTable;
		if (BuilderTable.TryGetBuilderTableForZone(GTZone.monkeBlocks, out builderTable))
		{
			builderTable.OnTableConfigurationUpdated.RemoveListener(new UnityAction(this.UpdateCountdown));
		}
	}

	// Token: 0x0600279B RID: 10139 RVA: 0x000D1B40 File Offset: 0x000CFD40
	private void OnOwnedSetsUpdated()
	{
		if (this.hasInitFromPlayfab || !BuilderSetManager.instance.pulledStoreItems)
		{
			if (this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.Start || this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.CheckoutButtonPressed)
			{
				this.ProcessPurchaseItemState(null, false);
			}
			return;
		}
		this.hasInitFromPlayfab = true;
		this.availableItems.Clear();
		if (this.isMiniKiosk)
		{
			this.availableItems.Add(this.pieceSetForSale);
		}
		else
		{
			this.availableItems.AddRange(BuilderSetManager.instance.GetPermanentSetsForSale());
			this.availableItems.AddRange(BuilderSetManager.instance.GetSeasonalSetsForSale());
		}
		if (this.pieceSetForSale != null)
		{
			this.itemToBuy = BuilderSetManager.instance.GetStoreItemFromSetID(this.pieceSetForSale.GetIntIdentifier());
			this.UpdateLabels();
			this.UpdateDiorama();
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
			this.ProcessPurchaseItemState(null, false);
			return;
		}
		this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
		this.ProcessPurchaseItemState(null, false);
	}

	// Token: 0x0600279C RID: 10140 RVA: 0x000D1C34 File Offset: 0x000CFE34
	private void OnSetButtonPressed(GorillaPressableButton button, bool isLeft)
	{
		if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Buying && !this.animating)
		{
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
			int num = 0;
			for (int i = 0; i < this.setButtons.Length; i++)
			{
				if (button.Equals(this.setButtons[i]))
				{
					num = i;
					break;
				}
			}
			int num2 = this.pageIndex * this.setsPerPage + num;
			if (num2 < this.availableItems.Count)
			{
				BuilderPieceSet builderPieceSet = this.availableItems[num2];
				if (builderPieceSet.SetName.Equals(this.itemToBuy.displayName))
				{
					this.itemToBuy = BuilderKiosk.nullItem;
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				}
				else
				{
					this.itemToBuy = BuilderSetManager.instance.GetStoreItemFromSetID(builderPieceSet.GetIntIdentifier());
					this.UpdateLabels();
					this.UpdateDiorama();
				}
				this.ProcessPurchaseItemState(null, isLeft);
			}
		}
	}

	// Token: 0x0600279D RID: 10141 RVA: 0x000D1D0C File Offset: 0x000CFF0C
	private void OnPreviousPageClicked()
	{
		int num = Mathf.Clamp(this.pageIndex - 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x0600279E RID: 10142 RVA: 0x000D1D48 File Offset: 0x000CFF48
	private void OnNextPageClicked()
	{
		int num = Mathf.Clamp(this.pageIndex + 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x0600279F RID: 10143 RVA: 0x000D1D84 File Offset: 0x000CFF84
	private void UpdateLabels()
	{
		if (this.isMiniKiosk)
		{
			return;
		}
		for (int i = 0; i < this.setButtons.Length; i++)
		{
			int num = this.pageIndex * this.setsPerPage + i;
			if (num < this.availableItems.Count && this.availableItems[num] != null)
			{
				if (!this.setButtons[i].gameObject.activeSelf)
				{
					this.setButtons[i].gameObject.SetActive(true);
					this.setButtons[i].myTmpText.gameObject.SetActive(true);
				}
				if (this.setButtons[i].myTmpText.text != this.availableItems[num].SetName.ToUpper())
				{
					this.setButtons[i].myTmpText.text = this.availableItems[num].SetName.ToUpper();
				}
				bool flag = !this.itemToBuy.isNullItem && this.availableItems[num].playfabID == this.itemToBuy.playfabID;
				if (flag != this.setButtons[i].isOn || !this.setButtons[i].enabled)
				{
					this.setButtons[i].isOn = flag;
					this.setButtons[i].buttonRenderer.material = (flag ? this.setButtons[i].pressedMaterial : this.setButtons[i].unpressedMaterial);
				}
				this.setButtons[i].enabled = true;
			}
			else
			{
				if (this.setButtons[i].gameObject.activeSelf)
				{
					this.setButtons[i].gameObject.SetActive(false);
					this.setButtons[i].myTmpText.gameObject.SetActive(false);
				}
				if (this.setButtons[i].isOn || this.setButtons[i].enabled)
				{
					this.setButtons[i].isOn = false;
					this.setButtons[i].enabled = false;
				}
			}
		}
		bool flag2 = this.pageIndex > 0 && this.totalPages > 1;
		bool flag3 = this.pageIndex < this.totalPages - 1 && this.totalPages > 1;
		if (this.previousPageButton.myTmpText.enabled != flag2)
		{
			this.previousPageButton.myTmpText.enabled = flag2;
		}
		if (this.nextPageButton.myTmpText.enabled != flag3)
		{
			this.nextPageButton.myTmpText.enabled = flag3;
		}
	}

	// Token: 0x060027A0 RID: 10144 RVA: 0x000D201C File Offset: 0x000D021C
	public void UpdateDiorama()
	{
		if (this.isMiniKiosk)
		{
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.itemToBuy.isNullItem)
		{
			this.countdownText.gameObject.SetActive(false);
		}
		else
		{
			this.countdownText.gameObject.SetActive(BuilderSetManager.instance.IsSetSeasonal(this.itemToBuy.playfabID));
		}
		if (this.animating)
		{
			base.StopCoroutine(this.PlaySwapAnimation());
			if (this.currentDiorama != null)
			{
				Object.Destroy(this.currentDiorama);
				this.currentDiorama = null;
			}
			this.currentDiorama = this.nextDiorama;
			this.nextDiorama = null;
		}
		this.animating = true;
		if (this.nextDiorama != null)
		{
			Object.Destroy(this.nextDiorama);
			this.nextDiorama = null;
		}
		if (!this.itemToBuy.isNullItem && this.itemToBuy.displayModel != null)
		{
			this.nextDiorama = Object.Instantiate<GameObject>(this.itemToBuy.displayModel, this.nextItemDisplayPos);
		}
		else
		{
			this.nextDiorama = Object.Instantiate<GameObject>(this.emptyDisplay, this.nextItemDisplayPos);
		}
		this.itemDisplayAnimation.Rewind();
		if (this.currentDiorama != null)
		{
			this.currentDiorama.transform.SetParent(this.itemDisplayPos, false);
		}
		base.StartCoroutine(this.PlaySwapAnimation());
	}

	// Token: 0x060027A1 RID: 10145 RVA: 0x000D2187 File Offset: 0x000D0387
	private IEnumerator PlaySwapAnimation()
	{
		this.itemDisplayAnimation.Play();
		yield return new WaitForSeconds(this.itemDisplayAnimation.clip.length);
		if (this.currentDiorama != null)
		{
			Object.Destroy(this.currentDiorama);
			this.currentDiorama = null;
		}
		this.currentDiorama = this.nextDiorama;
		this.nextDiorama = null;
		this.animating = false;
		yield break;
	}

	// Token: 0x060027A2 RID: 10146 RVA: 0x000D2196 File Offset: 0x000D0396
	public void PressLeftPurchaseItemButton(GorillaPressableButton pressedPurchaseItemButton, bool isLeftHand)
	{
		if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Start && !this.animating)
		{
			this.ProcessPurchaseItemState("left", isLeftHand);
		}
	}

	// Token: 0x060027A3 RID: 10147 RVA: 0x000D21B4 File Offset: 0x000D03B4
	public void PressRightPurchaseItemButton(GorillaPressableButton pressedPurchaseItemButton, bool isLeftHand)
	{
		if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Start && !this.animating)
		{
			this.ProcessPurchaseItemState("right", isLeftHand);
		}
	}

	// Token: 0x060027A4 RID: 10148 RVA: 0x000D21D2 File Offset: 0x000D03D2
	public void OnUpdateCurrencyBalance()
	{
		if (this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.Start || this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.CheckoutButtonPressed || this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.ItemOwned)
		{
			this.ProcessPurchaseItemState(null, false);
		}
	}

	// Token: 0x060027A5 RID: 10149 RVA: 0x000D21F6 File Offset: 0x000D03F6
	public void ClearCheckout()
	{
		GorillaTelemetry.PostBuilderKioskEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_cancel, this.itemToBuy);
		this.itemToBuy = BuilderKiosk.nullItem;
		this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
	}

	// Token: 0x060027A6 RID: 10150 RVA: 0x000D2220 File Offset: 0x000D0420
	public void ProcessPurchaseItemState(string buttonSide, bool isLeftHand)
	{
		switch (this.currentPurchaseItemStage)
		{
		case CosmeticsController.PurchaseItemStages.Start:
			this.itemToBuy = BuilderKiosk.nullItem;
			this.FormattedPurchaseText(0);
			this.leftPurchaseButton.myTmpText.text = "-";
			this.rightPurchaseButton.myTmpText.text = "-";
			this.UpdateLabels();
			this.UpdateDiorama();
			return;
		case CosmeticsController.PurchaseItemStages.CheckoutButtonPressed:
			if (this.availableItems.Count > 1)
			{
				GorillaTelemetry.PostBuilderKioskEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_start, this.itemToBuy);
			}
			if (BuilderSetManager.instance.IsPieceSetOwnedLocally(this.itemToBuy.setID))
			{
				this.FormattedPurchaseText(1);
				this.leftPurchaseButton.myTmpText.text = "-";
				this.rightPurchaseButton.myTmpText.text = "-";
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemOwned;
				return;
			}
			if ((ulong)this.itemToBuy.cost <= (ulong)((long)CosmeticsController.instance.currencyBalance))
			{
				this.FormattedPurchaseText(2);
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CANCEL", out text, "NO!");
				string text2;
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CONFIRM", out text2, "YES!");
				this.leftPurchaseButton.myTmpText.text = text;
				this.rightPurchaseButton.myTmpText.text = text2;
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.unpressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.unpressedMaterial;
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemSelected;
				return;
			}
			this.FormattedPurchaseText(3);
			this.leftPurchaseButton.myTmpText.text = "-";
			this.rightPurchaseButton.myTmpText.text = "-";
			this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
			this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
			if (!this.isMiniKiosk)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				return;
			}
			break;
		case CosmeticsController.PurchaseItemStages.ItemSelected:
			if (buttonSide == "right")
			{
				GorillaTelemetry.PostBuilderKioskEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.item_select, this.itemToBuy);
				this.FormattedPurchaseText(4);
				string text3;
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CANCEL", out text3, "LET ME THINK ABOUT IT");
				string text4;
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CONFIRM", out text4, "YES! I NEED IT!");
				this.leftPurchaseButton.myTmpText.text = text4;
				this.rightPurchaseButton.myTmpText.text = text3;
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.unpressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.unpressedMaterial;
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement;
				return;
			}
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
			this.ProcessPurchaseItemState(null, isLeftHand);
			return;
		case CosmeticsController.PurchaseItemStages.ItemOwned:
		case CosmeticsController.PurchaseItemStages.Buying:
			break;
		case CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement:
			if (buttonSide == "left")
			{
				this.FormattedPurchaseText(5);
				this.leftPurchaseButton.myTmpText.text = "-";
				this.rightPurchaseButton.myTmpText.text = "-";
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Buying;
				this.isLastHandTouchedLeft = isLeftHand;
				this.PurchaseItem();
				return;
			}
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
			this.ProcessPurchaseItemState(null, isLeftHand);
			return;
		case CosmeticsController.PurchaseItemStages.Success:
			this.FormattedPurchaseText(7);
			this.audioSource.GTPlayOneShot(this.purchaseSetAudioClip, 1f);
			this.purchaseParticles.Play();
			GorillaTagger.Instance.offlineVRRig.AddCosmetic(this.itemToBuy.playfabID, 0);
			this.leftPurchaseButton.myTmpText.text = "-";
			this.rightPurchaseButton.myTmpText.text = "-";
			this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
			this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
			break;
		case CosmeticsController.PurchaseItemStages.Failure:
			this.FormattedPurchaseText(6);
			this.leftPurchaseButton.myTmpText.text = "-";
			this.rightPurchaseButton.myTmpText.text = "-";
			this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
			this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
			return;
		default:
			return;
		}
	}

	// Token: 0x060027A7 RID: 10151 RVA: 0x000D26DC File Offset: 0x000D08DC
	public void FormattedPurchaseText(int finalLineVar)
	{
		if (this._itemNameVar == null || this._itemCostVar == null || this._currencyBalanceVar == null || this._finalLineVar == null)
		{
			Debug.LogError("[LOCALIZATION::BUILDER_KIOSK] One of the dynamic variables is NULL and cannot update the [purchaseText] screen");
			return;
		}
		this._itemNameVar.Value = this.itemToBuy.displayName.ToUpper();
		this._itemCostVar.Value = (int)this.itemToBuy.cost;
		this._currencyBalanceVar.Value = CosmeticsController.instance.currencyBalance;
		this._finalLineVar.Value = finalLineVar;
	}

	// Token: 0x060027A8 RID: 10152 RVA: 0x000D2768 File Offset: 0x000D0968
	public void PurchaseItem()
	{
		BuilderSetManager.instance.TryPurchaseItem(this.itemToBuy.setID, delegate(bool result)
		{
			if (result)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Success;
				CosmeticsController.instance.currencyBalance -= (int)this.itemToBuy.cost;
				this.ProcessPurchaseItemState(null, this.isLastHandTouchedLeft);
				return;
			}
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
			this.ProcessPurchaseItemState(null, false);
		});
	}

	// Token: 0x04003341 RID: 13121
	public const string MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CONFIRM_KEY = "MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CONFIRM";

	// Token: 0x04003342 RID: 13122
	public const string MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CANCEL_KEY = "MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CANCEL";

	// Token: 0x04003343 RID: 13123
	public const string MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CONFIRM_KEY = "MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CONFIRM";

	// Token: 0x04003344 RID: 13124
	public const string MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CANCEL_KEY = "MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CANCEL";

	// Token: 0x04003345 RID: 13125
	public BuilderPieceSet pieceSetForSale;

	// Token: 0x04003346 RID: 13126
	public GorillaPressableButton leftPurchaseButton;

	// Token: 0x04003347 RID: 13127
	public GorillaPressableButton rightPurchaseButton;

	// Token: 0x04003348 RID: 13128
	public TMP_Text purchaseText;

	// Token: 0x04003349 RID: 13129
	[SerializeField]
	private bool isMiniKiosk;

	// Token: 0x0400334A RID: 13130
	[SerializeField]
	private bool useTitleCountDown = true;

	// Token: 0x0400334B RID: 13131
	[Header("Buttons")]
	[SerializeField]
	private GorillaPressableButton[] setButtons;

	// Token: 0x0400334C RID: 13132
	[SerializeField]
	private GorillaPressableButton previousPageButton;

	// Token: 0x0400334D RID: 13133
	[SerializeField]
	private GorillaPressableButton nextPageButton;

	// Token: 0x0400334E RID: 13134
	private BuilderPieceSet currentSet;

	// Token: 0x0400334F RID: 13135
	private int pageIndex;

	// Token: 0x04003350 RID: 13136
	private int setsPerPage = 3;

	// Token: 0x04003351 RID: 13137
	private int totalPages = 1;

	// Token: 0x04003352 RID: 13138
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003353 RID: 13139
	[SerializeField]
	private AudioClip purchaseSetAudioClip;

	// Token: 0x04003354 RID: 13140
	[SerializeField]
	private ParticleSystem purchaseParticles;

	// Token: 0x04003355 RID: 13141
	[SerializeField]
	private GameObject emptyDisplay;

	// Token: 0x04003356 RID: 13142
	private List<BuilderPieceSet> availableItems = new List<BuilderPieceSet>(10);

	// Token: 0x04003357 RID: 13143
	internal CosmeticsController.PurchaseItemStages currentPurchaseItemStage;

	// Token: 0x04003358 RID: 13144
	private bool hasInitFromPlayfab;

	// Token: 0x04003359 RID: 13145
	internal BuilderSetManager.BuilderSetStoreItem itemToBuy;

	// Token: 0x0400335A RID: 13146
	public static BuilderSetManager.BuilderSetStoreItem nullItem;

	// Token: 0x0400335B RID: 13147
	private GameObject currentDiorama;

	// Token: 0x0400335C RID: 13148
	private GameObject nextDiorama;

	// Token: 0x0400335D RID: 13149
	private bool animating;

	// Token: 0x0400335E RID: 13150
	[SerializeField]
	private Transform itemDisplayPos;

	// Token: 0x0400335F RID: 13151
	[SerializeField]
	private Transform nextItemDisplayPos;

	// Token: 0x04003360 RID: 13152
	[SerializeField]
	private Animation itemDisplayAnimation;

	// Token: 0x04003361 RID: 13153
	[SerializeField]
	private CountdownText countdownText;

	// Token: 0x04003362 RID: 13154
	private string countdownOverride = string.Empty;

	// Token: 0x04003363 RID: 13155
	private bool isLastHandTouchedLeft;

	// Token: 0x04003364 RID: 13156
	private string finalLine;

	// Token: 0x04003365 RID: 13157
	[Header("Localization")]
	[SerializeField]
	private LocalizedText _puchaseTextLoc;

	// Token: 0x04003366 RID: 13158
	private LocalizedString _puchaseTextLocStr;

	// Token: 0x04003367 RID: 13159
	private StringVariable _itemNameVar;

	// Token: 0x04003368 RID: 13160
	private IntVariable _finalLineVar;

	// Token: 0x04003369 RID: 13161
	private IntVariable _itemCostVar;

	// Token: 0x0400336A RID: 13162
	private IntVariable _currencyBalanceVar;
}
