using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GorillaNetworking;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace GorillaTagScripts.GhostReactor
{
	// Token: 0x0200101A RID: 4122
	public class GRKiosk : MonoBehaviour
	{
		// Token: 0x0600669B RID: 26267 RVA: 0x0020F43C File Offset: 0x0020D63C
		private async void Start()
		{
			if (string.IsNullOrEmpty(this.CosmeticNameForPurchase))
			{
				Debug.LogError("No cosmetic set for GRKiosk.");
				Object.Destroy(this);
			}
			else
			{
				while (!Application.isPlaying || CosmeticsController.instance == null || CosmeticsController.instance.allCosmetics == null)
				{
					await Task.Yield();
				}
				if (this._purchaseTextLoc != null)
				{
					this._purchaseTextLocStr = this._purchaseTextLoc.StringReference;
					this._itemNameVar = this._purchaseTextLocStr["item-name"] as StringVariable;
					this._itemCostVar = this._purchaseTextLocStr["item-cost"] as IntVariable;
					this._currencyBalanceVar = this._purchaseTextLocStr["currency-balance"] as IntVariable;
				}
				else
				{
					this._purchaseTextLocStr = new LocalizedString();
					this._itemNameVar = new StringVariable();
					this._itemCostVar = new IntVariable();
					this._currencyBalanceVar = new IntVariable();
				}
				if (this._purchaseParticles != null)
				{
					this._purchaseParticles.Stop();
				}
				CosmeticsController instance = CosmeticsController.instance;
				instance.OnGetCurrency = (Action)Delegate.Combine(instance.OnGetCurrency, new Action(this.OnGetCurrency));
				this.LeftPurchaseButton.onPressed += this.OnLeftPurchaseButtonPressed;
				this.RightPurchaseButton.onPressed += this.OnRightPurchaseButtonPressed;
				this._cosmeticForPurchase = CosmeticsController.instance.allCosmetics.Find(new Predicate<CosmeticsController.CosmeticItem>(this.MatchesCosmeticForPurchase));
				this._purchaseState = (this.PlayerOwnsItem() ? GRKiosk.PurchaseState.AlreadyOwned : GRKiosk.PurchaseState.AvailableForPurchase);
				this.ProcessPurchaseItemState(null, null);
			}
		}

		// Token: 0x0600669C RID: 26268 RVA: 0x0020F474 File Offset: 0x0020D674
		private void ProcessPurchaseItemState(GRKiosk.ButtonSide? button, HashSet<GRKiosk.PurchaseState> recentStates = null)
		{
			if (recentStates == null)
			{
				recentStates = new HashSet<GRKiosk.PurchaseState>();
			}
			recentStates.Add(this._purchaseState);
			switch (this._purchaseState)
			{
			case GRKiosk.PurchaseState.Initialize:
				throw new Exception("ProcessPurchaseItemState called in non-initialized GRKiosk!");
			case GRKiosk.PurchaseState.AlreadyOwned:
				this.ResetButtons();
				break;
			case GRKiosk.PurchaseState.AvailableForPurchase:
				this.SetAvailableForPurchaseDisplays(button);
				break;
			case GRKiosk.PurchaseState.CheckoutPressed:
				this.SetCheckoutConfirmationDisplays(button);
				break;
			case GRKiosk.PurchaseState.CheckoutConfirmation:
				this.ConfirmCheckout(button);
				break;
			}
			if (!recentStates.Contains(this._purchaseState))
			{
				this.ProcessPurchaseItemState(null, recentStates);
			}
			this.FormattedPurchaseText();
		}

		// Token: 0x0600669D RID: 26269 RVA: 0x0020F50B File Offset: 0x0020D70B
		private bool PlayerOwnsItem()
		{
			return CosmeticsController.instance.unlockedCosmetics.Any(new Func<CosmeticsController.CosmeticItem, bool>(this.MatchesCosmeticForPurchase));
		}

		// Token: 0x0600669E RID: 26270 RVA: 0x0020F52C File Offset: 0x0020D72C
		private void OnGetCurrency()
		{
			this.ProcessPurchaseItemState(null, null);
		}

		// Token: 0x0600669F RID: 26271 RVA: 0x0020F54C File Offset: 0x0020D74C
		private void ResetButtons()
		{
			this.LeftPurchaseButton.myTmpText.text = "-";
			this.RightPurchaseButton.myTmpText.text = "-";
			this.LeftPurchaseButton.buttonRenderer.material = this.LeftPurchaseButton.pressedMaterial;
			this.RightPurchaseButton.buttonRenderer.material = this.RightPurchaseButton.pressedMaterial;
		}

		// Token: 0x060066A0 RID: 26272 RVA: 0x0020F5BC File Offset: 0x0020D7BC
		private void SetAvailableForPurchaseDisplays(GRKiosk.ButtonSide? button)
		{
			if (this._cosmeticForPurchase.cost <= CosmeticsController.instance.currencyBalance)
			{
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CANCEL", out text, "NO!");
				string text2;
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CONFIRM", out text2, "YES!");
				this.LeftPurchaseButton.myTmpText.text = text;
				this.RightPurchaseButton.myTmpText.text = text2;
				this.LeftPurchaseButton.buttonRenderer.material = this.LeftPurchaseButton.unpressedMaterial;
				this.RightPurchaseButton.buttonRenderer.material = this.RightPurchaseButton.unpressedMaterial;
				GRKiosk.ButtonSide? buttonSide = button;
				GRKiosk.ButtonSide buttonSide2 = GRKiosk.ButtonSide.Right;
				if ((buttonSide.GetValueOrDefault() == buttonSide2) & (buttonSide != null))
				{
					this._purchaseState = GRKiosk.PurchaseState.CheckoutPressed;
					return;
				}
			}
			else
			{
				this.LeftPurchaseButton.myTmpText.text = "-";
				this.RightPurchaseButton.myTmpText.text = "-";
				this.LeftPurchaseButton.buttonRenderer.material = this.LeftPurchaseButton.pressedMaterial;
				this.RightPurchaseButton.buttonRenderer.material = this.RightPurchaseButton.pressedMaterial;
				this._purchaseState = GRKiosk.PurchaseState.AvailableForPurchase;
			}
		}

		// Token: 0x060066A1 RID: 26273 RVA: 0x0020F6E8 File Offset: 0x0020D8E8
		private void SetCheckoutConfirmationDisplays(GRKiosk.ButtonSide? button)
		{
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CANCEL", out text, "LET ME THINK ABOUT IT");
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CONFIRM", out text2, "YES! I NEED IT!");
			this.LeftPurchaseButton.myTmpText.text = text2;
			this.RightPurchaseButton.myTmpText.text = text;
			this.LeftPurchaseButton.buttonRenderer.material = this.LeftPurchaseButton.unpressedMaterial;
			this.RightPurchaseButton.buttonRenderer.material = this.RightPurchaseButton.unpressedMaterial;
			this._purchaseState = GRKiosk.PurchaseState.CheckoutConfirmation;
		}

		// Token: 0x060066A2 RID: 26274 RVA: 0x0020F778 File Offset: 0x0020D978
		private void ConfirmCheckout(GRKiosk.ButtonSide? button)
		{
			GRKiosk.ButtonSide? buttonSide = button;
			GRKiosk.ButtonSide buttonSide2 = GRKiosk.ButtonSide.Left;
			if ((buttonSide.GetValueOrDefault() == buttonSide2) & (buttonSide != null))
			{
				this.PurchaseItem();
				return;
			}
			buttonSide = button;
			buttonSide2 = GRKiosk.ButtonSide.Right;
			if ((buttonSide.GetValueOrDefault() == buttonSide2) & (buttonSide != null))
			{
				this._purchaseState = GRKiosk.PurchaseState.AvailableForPurchase;
			}
		}

		// Token: 0x060066A3 RID: 26275 RVA: 0x0020F7C4 File Offset: 0x0020D9C4
		private void PurchaseItem()
		{
			PurchaseItemRequest purchaseItemRequest = new PurchaseItemRequest();
			purchaseItemRequest.ItemId = this._cosmeticForPurchase.itemName;
			purchaseItemRequest.Price = this._cosmeticForPurchase.cost;
			purchaseItemRequest.VirtualCurrency = "SR";
			PlayFabClientAPI.PurchaseItem(purchaseItemRequest, delegate(PurchaseItemResult result)
			{
				this._purchaseState = ((result.Items.Count > 0) ? GRKiosk.PurchaseState.AlreadyOwned : GRKiosk.PurchaseState.AvailableForPurchase);
				if (this._purchaseParticles != null)
				{
					this._purchaseParticles.Play();
				}
				GorillaTagger.Instance.offlineVRRig.AddCosmetic(this._cosmeticForPurchase.itemName, 0);
				this.ProcessPurchaseItemState(null, null);
			}, delegate(PlayFabError error)
			{
				Debug.LogError(error.ToString());
			}, null, null);
		}

		// Token: 0x060066A4 RID: 26276 RVA: 0x0020F835 File Offset: 0x0020DA35
		private bool MatchesCosmeticForPurchase(CosmeticsController.CosmeticItem item)
		{
			return this.CosmeticNameForPurchase == item.displayName || this.CosmeticNameForPurchase == item.overrideDisplayName || this.CosmeticNameForPurchase == item.itemName;
		}

		// Token: 0x060066A5 RID: 26277 RVA: 0x0020F870 File Offset: 0x0020DA70
		private void OnLeftPurchaseButtonPressed(GorillaPressableButton button, bool isLeftHand)
		{
			this.ProcessPurchaseItemState(new GRKiosk.ButtonSide?(GRKiosk.ButtonSide.Left), null);
		}

		// Token: 0x060066A6 RID: 26278 RVA: 0x0020F87F File Offset: 0x0020DA7F
		private void OnRightPurchaseButtonPressed(GorillaPressableButton button, bool isLeftHand)
		{
			this.ProcessPurchaseItemState(new GRKiosk.ButtonSide?(GRKiosk.ButtonSide.Right), null);
		}

		// Token: 0x060066A7 RID: 26279 RVA: 0x0020F890 File Offset: 0x0020DA90
		private void FormattedPurchaseText()
		{
			if (this._itemNameVar == null || this._itemCostVar == null || this._currencyBalanceVar == null)
			{
				Debug.LogError("[LOCALIZATION::GRKIOSK] One of the dynamic variables is NULL and cannot update the [PurchaseText] screen");
				return;
			}
			this._itemNameVar.Value = this._cosmeticForPurchase.displayName.ToUpper();
			this._itemCostVar.Value = this._cosmeticForPurchase.cost;
			this._currencyBalanceVar.Value = CosmeticsController.instance.currencyBalance;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ITEM: ").Append(this._cosmeticForPurchase.overrideDisplayName.ToUpper());
			stringBuilder.Append("\nITEM COST: ").Append(this._cosmeticForPurchase.cost);
			stringBuilder.Append("\nYOU HAVE: ").Append(CosmeticsController.instance.currencyBalance);
			StringBuilder stringBuilder2 = stringBuilder.Append("\n");
			string text;
			switch (this._purchaseState)
			{
			case GRKiosk.PurchaseState.AlreadyOwned:
				text = "YOU ALREADY OWN THIS!";
				break;
			case GRKiosk.PurchaseState.AvailableForPurchase:
				text = "PURCHASE?";
				break;
			case GRKiosk.PurchaseState.CheckoutPressed:
				text = "CONFIRM PURCHASE?";
				break;
			case GRKiosk.PurchaseState.CheckoutConfirmation:
				text = "CONFIRMING PURCHASE...";
				break;
			default:
				text = "ERROR";
				break;
			}
			stringBuilder2.Append(text);
			this.PurchaseText.text = stringBuilder.ToString();
		}

		// Token: 0x0400757B RID: 30075
		[SerializeField]
		public string CosmeticNameForPurchase;

		// Token: 0x0400757C RID: 30076
		[SerializeField]
		public GorillaPressableButton LeftPurchaseButton;

		// Token: 0x0400757D RID: 30077
		[SerializeField]
		public GorillaPressableButton RightPurchaseButton;

		// Token: 0x0400757E RID: 30078
		[SerializeField]
		public TMP_Text PurchaseText;

		// Token: 0x0400757F RID: 30079
		private CosmeticsController.CosmeticItem _cosmeticForPurchase;

		// Token: 0x04007580 RID: 30080
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x04007581 RID: 30081
		[SerializeField]
		private AudioClip _purchaseAudioClip;

		// Token: 0x04007582 RID: 30082
		[SerializeField]
		private ParticleSystem _purchaseParticles;

		// Token: 0x04007583 RID: 30083
		[SerializeField]
		private LocalizedText _purchaseTextLoc;

		// Token: 0x04007584 RID: 30084
		private LocalizedString _purchaseTextLocStr;

		// Token: 0x04007585 RID: 30085
		private StringVariable _itemNameVar;

		// Token: 0x04007586 RID: 30086
		private IntVariable _itemCostVar;

		// Token: 0x04007587 RID: 30087
		private IntVariable _currencyBalanceVar;

		// Token: 0x04007588 RID: 30088
		private GRKiosk.PurchaseState _purchaseState;

		// Token: 0x0200101B RID: 4123
		private enum PurchaseState
		{
			// Token: 0x0400758A RID: 30090
			Initialize,
			// Token: 0x0400758B RID: 30091
			AlreadyOwned,
			// Token: 0x0400758C RID: 30092
			AvailableForPurchase,
			// Token: 0x0400758D RID: 30093
			CheckoutPressed,
			// Token: 0x0400758E RID: 30094
			CheckoutConfirmation
		}

		// Token: 0x0200101C RID: 4124
		private enum ButtonSide
		{
			// Token: 0x04007590 RID: 30096
			Left,
			// Token: 0x04007591 RID: 30097
			Right
		}
	}
}
