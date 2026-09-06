using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	// Token: 0x02001141 RID: 4417
	[Serializable]
	public class StoreBundle
	{
		// Token: 0x17000A91 RID: 2705
		// (get) Token: 0x06006ECD RID: 28365 RVA: 0x0023AF11 File Offset: 0x00239111
		public string playfabBundleID
		{
			get
			{
				return this._storeBundleDataReference.playfabBundleID;
			}
		}

		// Token: 0x17000A92 RID: 2706
		// (get) Token: 0x06006ECE RID: 28366 RVA: 0x0023AF1E File Offset: 0x0023911E
		public string bundleSKU
		{
			get
			{
				return this._storeBundleDataReference.bundleSKU;
			}
		}

		// Token: 0x17000A93 RID: 2707
		// (get) Token: 0x06006ECF RID: 28367 RVA: 0x0023AF2B File Offset: 0x0023912B
		public Sprite bundleImage
		{
			get
			{
				return this._storeBundleDataReference.bundleImage;
			}
		}

		// Token: 0x17000A94 RID: 2708
		// (get) Token: 0x06006ED0 RID: 28368 RVA: 0x0023AF38 File Offset: 0x00239138
		public NexusCreatorCode nexusCreatorCode
		{
			get
			{
				return this._storeBundleDataReference.creatorCode;
			}
		}

		// Token: 0x17000A95 RID: 2709
		// (get) Token: 0x06006ED1 RID: 28369 RVA: 0x0023AF45 File Offset: 0x00239145
		public string price
		{
			get
			{
				return this._price;
			}
		}

		// Token: 0x17000A96 RID: 2710
		// (get) Token: 0x06006ED2 RID: 28370 RVA: 0x0023AF50 File Offset: 0x00239150
		public string bundleName
		{
			get
			{
				if (this._bundleName.IsNullOrEmpty())
				{
					int num = CosmeticsController.instance.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => this.playfabBundleID == x.itemName);
					if (num > -1)
					{
						if (!CosmeticsController.instance.allCosmetics[num].overrideDisplayName.IsNullOrEmpty())
						{
							this._bundleName = CosmeticsController.instance.allCosmetics[num].overrideDisplayName;
						}
						else
						{
							this._bundleName = CosmeticsController.instance.allCosmetics[num].displayName;
						}
					}
					else
					{
						this._bundleName = "NULL_BUNDLE_NAME";
					}
				}
				return this._bundleName;
			}
		}

		// Token: 0x17000A97 RID: 2711
		// (get) Token: 0x06006ED3 RID: 28371 RVA: 0x0023AFFC File Offset: 0x002391FC
		public bool HasPrice
		{
			get
			{
				return !string.IsNullOrEmpty(this.price) && this.price != StoreBundle.defaultPrice;
			}
		}

		// Token: 0x17000A98 RID: 2712
		// (get) Token: 0x06006ED4 RID: 28372 RVA: 0x0023B01D File Offset: 0x0023921D
		public string bundleDescriptionText
		{
			get
			{
				return this._storeBundleDataReference.bundleDescriptionText;
			}
		}

		// Token: 0x06006ED5 RID: 28373 RVA: 0x0023B02C File Offset: 0x0023922C
		public StoreBundle()
		{
			this.isOwned = false;
			this.bundleStands = new List<BundleStand>();
		}

		// Token: 0x06006ED6 RID: 28374 RVA: 0x0023B080 File Offset: 0x00239280
		public StoreBundle(StoreBundleData data)
		{
			this.isOwned = false;
			this.bundleStands = new List<BundleStand>();
			this._storeBundleDataReference = data;
		}

		// Token: 0x06006ED7 RID: 28375 RVA: 0x0023B0D8 File Offset: 0x002392D8
		public void InitializebundleStands()
		{
			foreach (BundleStand bundleStand in this.bundleStands)
			{
				bundleStand.UpdateDescriptionText(this.bundleDescriptionText);
				bundleStand.InitializeEventListeners();
			}
		}

		// Token: 0x06006ED8 RID: 28376 RVA: 0x0023B134 File Offset: 0x00239334
		public void TryUpdatePrice(uint bundlePrice)
		{
			this.TryUpdatePrice((bundlePrice / 100m).ToString());
		}

		// Token: 0x06006ED9 RID: 28377 RVA: 0x0023B164 File Offset: 0x00239364
		public void TryUpdatePrice(string bundlePrice = null)
		{
			if (!string.IsNullOrEmpty(bundlePrice))
			{
				decimal num;
				this._price = (decimal.TryParse(bundlePrice, out num) ? (StoreBundle.defaultCurrencySymbol + bundlePrice) : bundlePrice);
			}
			this.UpdatePurchaseButtonText();
		}

		// Token: 0x06006EDA RID: 28378 RVA: 0x0023B1A0 File Offset: 0x002393A0
		public void UpdatePurchaseButtonText()
		{
			this.purchaseButtonText = string.Format(this.purchaseButtonStringFormat, this.bundleName, this.price);
			foreach (BundleStand bundleStand in this.bundleStands)
			{
				bundleStand.UpdatePurchaseButtonText(this.purchaseButtonText);
			}
		}

		// Token: 0x06006EDB RID: 28379 RVA: 0x0023B214 File Offset: 0x00239414
		public void ValidateBundleData()
		{
			if (this._storeBundleDataReference == null)
			{
				Debug.LogError("StoreBundleData is null");
				foreach (BundleStand bundleStand in this.bundleStands)
				{
					if (bundleStand == null)
					{
						Debug.LogError("BundleStand is null");
					}
					else if (bundleStand._bundleDataReference != null)
					{
						this._storeBundleDataReference = bundleStand._bundleDataReference;
						Debug.LogError("BundleStand StoreBundleData is not equal to StoreBundle StoreBundleData");
					}
				}
			}
			if (this._storeBundleDataReference == null)
			{
				Debug.LogError("StoreBundleData is null");
				return;
			}
			if (this._storeBundleDataReference.playfabBundleID.IsNullOrEmpty())
			{
				Debug.LogError("playfabBundleID is null");
			}
			if (this._storeBundleDataReference.bundleSKU.IsNullOrEmpty())
			{
				Debug.LogError("bundleSKU is null");
			}
			if (this._storeBundleDataReference.bundleImage == null)
			{
				Debug.LogError("bundleImage is null");
			}
			if (this._storeBundleDataReference.bundleDescriptionText.IsNullOrEmpty())
			{
				Debug.LogError("bundleDescriptionText is null");
			}
		}

		// Token: 0x04007EE2 RID: 32482
		private static readonly string defaultPrice = "$--.--";

		// Token: 0x04007EE3 RID: 32483
		private static readonly string defaultCurrencySymbol = "$";

		// Token: 0x04007EE4 RID: 32484
		[NonSerialized]
		public string purchaseButtonStringFormat = "THE {0}\n{1}";

		// Token: 0x04007EE5 RID: 32485
		[SerializeField]
		public List<BundleStand> bundleStands;

		// Token: 0x04007EE6 RID: 32486
		public bool isOwned;

		// Token: 0x04007EE7 RID: 32487
		private string _price = StoreBundle.defaultPrice;

		// Token: 0x04007EE8 RID: 32488
		private string _bundleName = "";

		// Token: 0x04007EE9 RID: 32489
		public string purchaseButtonText = "";

		// Token: 0x04007EEA RID: 32490
		[FormerlySerializedAs("storeBundleDataReference")]
		[SerializeField]
		[ReadOnly]
		private StoreBundleData _storeBundleDataReference;
	}
}
