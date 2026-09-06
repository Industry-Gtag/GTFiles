using System;
using DefaultNamespace;
using GorillaNetworking;
using GorillaNetworking.Store;
using UnityEngine;

namespace CosmeticRoom
{
	// Token: 0x0200108B RID: 4235
	public class EvolvingCosmeticKioskButtonSet : MonoBehaviour
	{
		// Token: 0x060069BC RID: 27068 RVA: 0x0022084E File Offset: 0x0021EA4E
		public void RegisterKiosk(EvolvingCosmeticKiosk kiosk)
		{
			if (this._kiosk != null)
			{
				throw new Exception("Attempted to double-register EvolvingCosmeticKiosk to a button.");
			}
			this._kiosk = kiosk;
		}

		// Token: 0x060069BD RID: 27069 RVA: 0x00220870 File Offset: 0x0021EA70
		public void Reset()
		{
			this._cosmeticStand.ClearCosmetics();
			this._playfabId = null;
			this._cosmetic = null;
		}

		// Token: 0x060069BE RID: 27070 RVA: 0x0022088B File Offset: 0x0021EA8B
		public void SetCosmetic(string playfabId, EvolvingCosmetic evolvingCosmetic)
		{
			this._cosmeticStand.SpawnItemOntoStand(playfabId);
			this._playfabId = playfabId;
			this._cosmetic = evolvingCosmetic;
		}

		// Token: 0x060069BF RID: 27071 RVA: 0x002208A7 File Offset: 0x0021EAA7
		public void GoForward()
		{
			if (this._cosmetic == null || !this._cosmetic.CanGoForward())
			{
				return;
			}
			this._cosmetic.GoForward();
			this.RefreshOnPlayer();
		}

		// Token: 0x060069C0 RID: 27072 RVA: 0x002208D6 File Offset: 0x0021EAD6
		public void GoBackward()
		{
			if (this._cosmetic == null || !this._cosmetic.CanGoBack())
			{
				return;
			}
			this._cosmetic.GoBack();
			this.RefreshOnPlayer();
		}

		// Token: 0x060069C1 RID: 27073 RVA: 0x00220908 File Offset: 0x0021EB08
		private void RefreshOnPlayer()
		{
			if (this._kiosk == null || this._playfabId == null || this._cosmetic == null)
			{
				return;
			}
			bool flag = false;
			CosmeticsController.CosmeticItem[] items = CosmeticsController.instance.currentWornSet.items;
			for (int i = 0; i < items.Length; i++)
			{
				if (!(items[i].itemName != this._playfabId))
				{
					CosmeticItemInstance cosmeticItemInstance = this._kiosk.VRRig.cosmeticsObjectRegistry.Cosmetic(this._playfabId);
					if (cosmeticItemInstance != null)
					{
						foreach (GameObject gameObject in cosmeticItemInstance.objects)
						{
							EvolvingCosmetic component = gameObject.GetComponent<EvolvingCosmetic>();
							if (component != null)
							{
								component.MatchStage(this._cosmetic);
								EvolvingCosmeticSaveData.Instance.SelectedIndices[component.PlayfabId] = component.SelectedObjectIndex;
								flag = true;
							}
						}
					}
				}
			}
			if (flag)
			{
				PlayerPrefs.SetString("EvolvingCosmeticSaveData", EvolvingCosmeticSaveData.Instance.Write());
			}
		}

		// Token: 0x04007974 RID: 31092
		[SerializeField]
		private DynamicCosmeticStand _cosmeticStand;

		// Token: 0x04007975 RID: 31093
		[SerializeField]
		private GorillaPressableButton _plusButton;

		// Token: 0x04007976 RID: 31094
		[SerializeField]
		private GorillaPressableButton _minusButton;

		// Token: 0x04007977 RID: 31095
		private EvolvingCosmeticKiosk _kiosk;

		// Token: 0x04007978 RID: 31096
		private EvolvingCosmetic _cosmetic;

		// Token: 0x04007979 RID: 31097
		private string _playfabId;
	}
}
