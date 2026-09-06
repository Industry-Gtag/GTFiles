using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

namespace CosmeticRoom
{
	// Token: 0x0200108C RID: 4236
	public class FittingRoom : MonoBehaviour
	{
		// Token: 0x060069C3 RID: 27075 RVA: 0x00220A2C File Offset: 0x0021EC2C
		public void InitializeForCustomMap(bool useCustomConsoleMesh = true)
		{
			GameObject gameObject = this.consoleMesh;
			if (gameObject != null)
			{
				gameObject.SetActive(!useCustomConsoleMesh);
			}
			CosmeticsController.instance.AddFittingRoom(this);
		}

		// Token: 0x060069C4 RID: 27076 RVA: 0x00220A50 File Offset: 0x0021EC50
		private void OnEnable()
		{
			if (this.addOnEnable)
			{
				CosmeticsController.instance.AddFittingRoom(this);
			}
		}

		// Token: 0x060069C5 RID: 27077 RVA: 0x00220A67 File Offset: 0x0021EC67
		private void OnDisable()
		{
			if (this.addOnEnable)
			{
				CosmeticsController.instance.RemoveFittingRoom(this);
			}
		}

		// Token: 0x060069C6 RID: 27078 RVA: 0x00220A80 File Offset: 0x0021EC80
		public void UpdateFromCart(List<CosmeticsController.CosmeticItem> currentCart, CosmeticsController.CosmeticSet tryOnSet)
		{
			this.iterator = 0;
			while (this.iterator < this.fittingRoomButtons.Length)
			{
				if (this.iterator < currentCart.Count)
				{
					bool flag = CosmeticsController.instance.AnyMatch(tryOnSet, currentCart[this.iterator]) || (!CosmeticsController.instance.tryOnCollectableItem.isNullItem && currentCart[this.iterator].itemName == CosmeticsController.instance.tryOnCollectableItem.itemName);
					this.fittingRoomButtons[this.iterator].SetItem(currentCart[this.iterator], flag);
				}
				else
				{
					this.fittingRoomButtons[this.iterator].ClearItem();
				}
				this.iterator++;
			}
		}

		// Token: 0x0400797A RID: 31098
		public FittingRoomButton[] fittingRoomButtons;

		// Token: 0x0400797B RID: 31099
		public GameObject consoleMesh;

		// Token: 0x0400797C RID: 31100
		private int iterator;

		// Token: 0x0400797D RID: 31101
		public bool addOnEnable;
	}
}
