using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CosmeticRoom
{
	// Token: 0x0200108D RID: 4237
	public class ItemCheckout : MonoBehaviour
	{
		// Token: 0x060069C8 RID: 27080 RVA: 0x00220B57 File Offset: 0x0021ED57
		private void OnEnable()
		{
			if (this.addOnEnable)
			{
				CosmeticsController.instance.AddItemCheckout(this);
			}
		}

		// Token: 0x060069C9 RID: 27081 RVA: 0x00220B6E File Offset: 0x0021ED6E
		private void OnDisable()
		{
			if (this.addOnEnable)
			{
				CosmeticsController.instance.RemoveItemCheckout(this);
			}
		}

		// Token: 0x060069CA RID: 27082 RVA: 0x00220B88 File Offset: 0x0021ED88
		public void InitializeForCustomMap(CompositeTriggerEvents customMapTryOnArea, Scene customMapScene, bool useCustomCounterMesh = true)
		{
			GameObject gameObject = this.checkoutCounterMesh;
			if (gameObject != null)
			{
				gameObject.SetActive(!useCustomCounterMesh);
			}
			GameObject gameObject2 = this.purchaseScreenMesh;
			if (gameObject2 != null)
			{
				gameObject2.SetActive(useCustomCounterMesh);
			}
			this.originalScene = customMapScene;
			customMapTryOnArea.AddCollider(this.checkoutTryOnArea);
			CosmeticsController.instance.AddItemCheckout(this);
		}

		// Token: 0x060069CB RID: 27083 RVA: 0x00220BDC File Offset: 0x0021EDDC
		public void RemoveFromCustomMap(CompositeTriggerEvents customMapTryOnArea)
		{
			if (customMapTryOnArea.IsNull())
			{
				return;
			}
			customMapTryOnArea.RemoveCollider(this.checkoutTryOnArea);
		}

		// Token: 0x060069CC RID: 27084 RVA: 0x00220BF4 File Offset: 0x0021EDF4
		public void UpdateFromCart(List<CosmeticsController.CosmeticItem> currentCart, CosmeticsController.CosmeticItem itemToBuy)
		{
			this.iterator = 0;
			while (this.iterator < this.checkoutCartButtons.Length)
			{
				if (this.iterator < currentCart.Count)
				{
					bool flag = currentCart[this.iterator].itemName == itemToBuy.itemName;
					this.checkoutCartButtons[this.iterator].SetItem(currentCart[this.iterator], flag);
				}
				else
				{
					this.checkoutCartButtons[this.iterator].ClearItem();
				}
				this.iterator++;
			}
		}

		// Token: 0x060069CD RID: 27085 RVA: 0x00220C88 File Offset: 0x0021EE88
		public void UpdatePurchaseText(string newText, string leftPurchaseButtonText, string rightPurchaseButtonText, bool leftButtonOn, bool rightButtonOn)
		{
			if (this.purchaseText.IsNotNull())
			{
				this.purchaseText.text = newText;
			}
			if (this.purchaseTextTMP.IsNotNull())
			{
				this.purchaseTextTMP.text = newText;
			}
			if (!leftPurchaseButtonText.IsNullOrEmpty())
			{
				this.leftPurchaseButton.SetText(leftPurchaseButtonText);
				this.leftPurchaseButton.buttonRenderer.material = (leftButtonOn ? this.leftPurchaseButton.pressedMaterial : this.leftPurchaseButton.unpressedMaterial);
			}
			if (!rightPurchaseButtonText.IsNullOrEmpty())
			{
				this.rightPurchaseButton.SetText(rightPurchaseButtonText);
				this.rightPurchaseButton.buttonRenderer.material = (rightButtonOn ? this.rightPurchaseButton.pressedMaterial : this.rightPurchaseButton.unpressedMaterial);
			}
		}

		// Token: 0x060069CE RID: 27086 RVA: 0x00220D47 File Offset: 0x0021EF47
		public bool IsFromScene(Scene unloadingScene)
		{
			return unloadingScene == this.originalScene;
		}

		// Token: 0x0400797E RID: 31102
		public CheckoutCartButton[] checkoutCartButtons;

		// Token: 0x0400797F RID: 31103
		public PurchaseItemButton leftPurchaseButton;

		// Token: 0x04007980 RID: 31104
		public PurchaseItemButton rightPurchaseButton;

		// Token: 0x04007981 RID: 31105
		[HideInInspector]
		public Text purchaseText;

		// Token: 0x04007982 RID: 31106
		public TMP_Text purchaseTextTMP;

		// Token: 0x04007983 RID: 31107
		public HeadModel checkoutHeadModel;

		// Token: 0x04007984 RID: 31108
		public Collider checkoutTryOnArea;

		// Token: 0x04007985 RID: 31109
		public GameObject checkoutCounterMesh;

		// Token: 0x04007986 RID: 31110
		public GameObject purchaseScreenMesh;

		// Token: 0x04007987 RID: 31111
		private Scene originalScene;

		// Token: 0x04007988 RID: 31112
		private int iterator;

		// Token: 0x04007989 RID: 31113
		public bool addOnEnable;
	}
}
