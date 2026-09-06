using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cosmetics;
using GorillaExtensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	// Token: 0x02001139 RID: 4409
	public class BundleManager : MonoBehaviour
	{
		// Token: 0x06006E93 RID: 28307 RVA: 0x00239FE6 File Offset: 0x002381E6
		private IEnumerable GetStoreBundles()
		{
			List<StoreBundleData> list = new List<StoreBundleData>();
			list.Add(this.nullBundleData);
			list.AddRange(this._bundleScriptableObjects);
			return list;
		}

		// Token: 0x06006E94 RID: 28308 RVA: 0x0023A005 File Offset: 0x00238205
		public void Awake()
		{
			if (BundleManager.instance == null)
			{
				BundleManager.instance = this;
				return;
			}
			if (BundleManager.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}

		// Token: 0x06006E95 RID: 28309 RVA: 0x0023A03A File Offset: 0x0023823A
		private void Start()
		{
			this.GenerateBundleDictionaries();
			this.Initialize();
		}

		// Token: 0x06006E96 RID: 28310 RVA: 0x0023A048 File Offset: 0x00238248
		private void Initialize()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				storeBundle.InitializebundleStands();
			}
		}

		// Token: 0x06006E97 RID: 28311 RVA: 0x0023A098 File Offset: 0x00238298
		private void ValidateBundleData()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				storeBundle.ValidateBundleData();
			}
		}

		// Token: 0x06006E98 RID: 28312 RVA: 0x0023A0E8 File Offset: 0x002382E8
		private void SpawnBundleStands()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					if (bundleStand != null)
					{
						Object.DestroyImmediate(bundleStand.gameObject);
					}
				}
			}
			this._spawnedBundleStands.Clear();
			this.storeBundlesById.Clear();
			this.storeBundlesBySKU.Clear();
			this._storeBundles.Clear();
			this._bundleScriptableObjects.Clear();
			BundleStand[] array = Object.FindObjectsByType<BundleStand>(FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				Object.DestroyImmediate(array[i].gameObject);
			}
			for (int j = 0; j < this.BundleStands.Count; j++)
			{
				if (this.BundleStands[j].spawnLocation == null)
				{
					Debug.LogError("No spawn location set for Bundle Stand " + j.ToString());
				}
				else if (this.BundleStands[j].bundleStand == null)
				{
					Debug.LogError("No Bundle Stand set for Bundle Stand " + j.ToString());
				}
			}
			this.GenerateAllStoreBundleReferences();
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton1))
			{
				this.tryOnBundleButton1 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton2))
			{
				this.tryOnBundleButton2 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton3))
			{
				this.tryOnBundleButton3 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton4))
			{
				this.tryOnBundleButton4 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton5))
			{
				this.tryOnBundleButton4 = this.nullBundleData;
			}
		}

		// Token: 0x06006E99 RID: 28313 RVA: 0x0023A2FC File Offset: 0x002384FC
		public void ClearEverything()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					if (bundleStand != null)
					{
						Object.DestroyImmediate(bundleStand.gameObject);
					}
				}
			}
			this._spawnedBundleStands.Clear();
			this.storeBundlesById.Clear();
			this.storeBundlesBySKU.Clear();
			this._storeBundles.Clear();
			this._bundleScriptableObjects.Clear();
			this.tryOnBundleButton1 = this.nullBundleData;
			this.tryOnBundleButton2 = this.nullBundleData;
			this.tryOnBundleButton3 = this.nullBundleData;
			this.tryOnBundleButton4 = this.nullBundleData;
			this.tryOnBundleButton5 = this.nullBundleData;
			BundleStand[] array = Object.FindObjectsByType<BundleStand>(FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				Object.DestroyImmediate(array[i].gameObject);
			}
		}

		// Token: 0x06006E9A RID: 28314 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void GenerateAllStoreBundleReferences()
		{
		}

		// Token: 0x06006E9B RID: 28315 RVA: 0x0023A430 File Offset: 0x00238630
		private void AddNewBundleStand(BundleStand bundleStand)
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				if (storeBundle.playfabBundleID == bundleStand._bundleDataReference.playfabBundleID)
				{
					storeBundle.bundleStands.Add(bundleStand);
					return;
				}
			}
			StoreBundle storeBundle2 = new StoreBundle(bundleStand._bundleDataReference);
			storeBundle2.bundleStands.Add(bundleStand);
			this._storeBundles.Add(storeBundle2);
		}

		// Token: 0x06006E9C RID: 28316 RVA: 0x0023A4C8 File Offset: 0x002386C8
		public void GenerateBundleDictionaries()
		{
			this.storeBundlesById.Clear();
			this.storeBundlesBySKU.Clear();
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				this.storeBundlesById.Add(storeBundle.playfabBundleID, storeBundle);
				this.storeBundlesBySKU.Add(storeBundle.bundleSKU, storeBundle);
			}
		}

		// Token: 0x06006E9D RID: 28317 RVA: 0x0023A550 File Offset: 0x00238750
		public void BundlePurchaseButtonPressed(string playFabItemName, ICreatorCodeProvider ccp)
		{
			CosmeticsController.instance.PurchaseBundle(this.storeBundlesById[playFabItemName], ccp);
		}

		// Token: 0x06006E9E RID: 28318 RVA: 0x0023A56C File Offset: 0x0023876C
		public void FixBundles()
		{
			this._storeBundles.Clear();
			for (int i = this._spawnedBundleStands.Count - 1; i >= 0; i--)
			{
				if (this._spawnedBundleStands[i].bundleStand == null)
				{
					this._spawnedBundleStands.RemoveAt(i);
				}
			}
			BundleStand[] array = Object.FindObjectsByType<BundleStand>(FindObjectsSortMode.None);
			for (int j = 0; j < array.Length; j++)
			{
				BundleStand bundle = array[j];
				if (this._spawnedBundleStands.Any((SpawnedBundle x) => x.spawnLocationPath == bundle.transform.parent.gameObject.GetPath(3)))
				{
					SpawnedBundle spawnedBundle = this._spawnedBundleStands.First((SpawnedBundle x) => x.spawnLocationPath == bundle.transform.parent.gameObject.GetPath(3));
					if (spawnedBundle != null && spawnedBundle.bundleStand != bundle)
					{
						Object.DestroyImmediate(spawnedBundle.bundleStand.gameObject);
						spawnedBundle.bundleStand = bundle;
					}
				}
				else
				{
					this._spawnedBundleStands.Add(new SpawnedBundle
					{
						spawnLocationPath = bundle.transform.parent.gameObject.GetPath(3),
						bundleStand = bundle
					});
				}
			}
			this.GenerateAllStoreBundleReferences();
		}

		// Token: 0x06006E9F RID: 28319 RVA: 0x0023A698 File Offset: 0x00238898
		public StoreBundleData[] GetTryOnButtons()
		{
			return new StoreBundleData[] { this.tryOnBundleButton1, this.tryOnBundleButton2, this.tryOnBundleButton3, this.tryOnBundleButton4, this.tryOnBundleButton5 };
		}

		// Token: 0x06006EA0 RID: 28320 RVA: 0x0023A6D0 File Offset: 0x002388D0
		public void NotifyBundleOfErrorByPlayFabID(string ItemId)
		{
			StoreBundle storeBundle;
			if (this.storeBundlesById.TryGetValue(ItemId, out storeBundle))
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					bundleStand.ErrorHappened();
				}
			}
		}

		// Token: 0x06006EA1 RID: 28321 RVA: 0x0023A730 File Offset: 0x00238930
		public void NotifyBundleOfErrorBySKU(string ItemSKU)
		{
			StoreBundle storeBundle;
			if (this.storeBundlesBySKU.TryGetValue(ItemSKU, out storeBundle))
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					bundleStand.ErrorHappened();
				}
			}
		}

		// Token: 0x06006EA2 RID: 28322 RVA: 0x0023A790 File Offset: 0x00238990
		public void MarkBundleOwnedByPlayFabID(string ItemId)
		{
			if (this.storeBundlesById.ContainsKey(ItemId))
			{
				this.storeBundlesById[ItemId].isOwned = true;
				foreach (BundleStand bundleStand in this.storeBundlesById[ItemId].bundleStands)
				{
					bundleStand.NotifyAlreadyOwn();
				}
			}
		}

		// Token: 0x06006EA3 RID: 28323 RVA: 0x0023A80C File Offset: 0x00238A0C
		public void MarkBundleOwnedBySKU(string SKU)
		{
			if (this.storeBundlesBySKU.ContainsKey(SKU))
			{
				this.storeBundlesBySKU[SKU].isOwned = true;
				foreach (BundleStand bundleStand in this.storeBundlesBySKU[SKU].bundleStands)
				{
					bundleStand.NotifyAlreadyOwn();
				}
			}
		}

		// Token: 0x06006EA4 RID: 28324 RVA: 0x0023A888 File Offset: 0x00238A88
		public void CheckIfBundlesOwned()
		{
			foreach (StoreBundle storeBundle in this.storeBundlesById.Values)
			{
				if (storeBundle.isOwned)
				{
					foreach (BundleStand bundleStand in storeBundle.bundleStands)
					{
						bundleStand.NotifyAlreadyOwn();
					}
				}
			}
		}

		// Token: 0x06006EA5 RID: 28325 RVA: 0x0023A920 File Offset: 0x00238B20
		public void PressTryOnBundleButton(TryOnBundleButton pressedTryOnBundleButton, bool isLeftHand)
		{
			if (this._tryOnBundlesStand.IsNotNull())
			{
				TryOnBundlesStand tryOnBundlesStand = this._tryOnBundlesStand;
				if (tryOnBundlesStand == null)
				{
					return;
				}
				tryOnBundlesStand.PressTryOnBundleButton(pressedTryOnBundleButton, isLeftHand);
			}
		}

		// Token: 0x06006EA6 RID: 28326 RVA: 0x0023A941 File Offset: 0x00238B41
		public void PressPurchaseTryOnBundleButton()
		{
			TryOnBundlesStand tryOnBundlesStand = this._tryOnBundlesStand;
			if (tryOnBundlesStand == null)
			{
				return;
			}
			tryOnBundlesStand.PurchaseButtonPressed();
		}

		// Token: 0x06006EA7 RID: 28327 RVA: 0x0023A953 File Offset: 0x00238B53
		public void UpdateBundlePrice(string productSku, string productFormattedPrice)
		{
			if (this.storeBundlesBySKU.ContainsKey(productSku))
			{
				this.storeBundlesBySKU[productSku].TryUpdatePrice(productFormattedPrice);
			}
		}

		// Token: 0x06006EA8 RID: 28328 RVA: 0x0023A978 File Offset: 0x00238B78
		public void CheckForNoPriceBundlesAndDefaultPrice()
		{
			foreach (KeyValuePair<string, StoreBundle> keyValuePair in this.storeBundlesBySKU)
			{
				string text;
				StoreBundle storeBundle;
				keyValuePair.Deconstruct(out text, out storeBundle);
				StoreBundle storeBundle2 = storeBundle;
				if (!storeBundle2.HasPrice)
				{
					storeBundle2.TryUpdatePrice(null);
				}
			}
		}

		// Token: 0x04007EBB RID: 32443
		public static volatile BundleManager instance;

		// Token: 0x04007EBC RID: 32444
		[FormerlySerializedAs("_TryOnBundlesStand")]
		public TryOnBundlesStand _tryOnBundlesStand;

		// Token: 0x04007EBD RID: 32445
		[SerializeField]
		private StoreBundleData nullBundleData;

		// Token: 0x04007EBE RID: 32446
		private List<StoreBundleData> _bundleScriptableObjects = new List<StoreBundleData>();

		// Token: 0x04007EBF RID: 32447
		[SerializeField]
		private List<StoreBundle> _storeBundles = new List<StoreBundle>();

		// Token: 0x04007EC0 RID: 32448
		[FormerlySerializedAs("_SpawnedBundleStands")]
		[SerializeField]
		private List<SpawnedBundle> _spawnedBundleStands = new List<SpawnedBundle>();

		// Token: 0x04007EC1 RID: 32449
		public Dictionary<string, StoreBundle> storeBundlesById = new Dictionary<string, StoreBundle>();

		// Token: 0x04007EC2 RID: 32450
		public Dictionary<string, StoreBundle> storeBundlesBySKU = new Dictionary<string, StoreBundle>();

		// Token: 0x04007EC3 RID: 32451
		[Header("Enable Advanced Search window in your settings to easily see all bundle prefabs")]
		[SerializeField]
		private List<BundleManager.BundleStandSpawn> BundleStands = new List<BundleManager.BundleStandSpawn>();

		// Token: 0x04007EC4 RID: 32452
		[SerializeField]
		private StoreBundleData tryOnBundleButton1;

		// Token: 0x04007EC5 RID: 32453
		[SerializeField]
		private StoreBundleData tryOnBundleButton2;

		// Token: 0x04007EC6 RID: 32454
		[SerializeField]
		private StoreBundleData tryOnBundleButton3;

		// Token: 0x04007EC7 RID: 32455
		[SerializeField]
		private StoreBundleData tryOnBundleButton4;

		// Token: 0x04007EC8 RID: 32456
		[SerializeField]
		private StoreBundleData tryOnBundleButton5;

		// Token: 0x0200113A RID: 4410
		[Serializable]
		public class BundleStandSpawn
		{
			// Token: 0x06006EAA RID: 28330 RVA: 0x0023AA39 File Offset: 0x00238C39
			private static IEnumerable GetEndCapSpawnPoints()
			{
				return from x in Object.FindObjectsByType<EndCapSpawnPoint>(FindObjectsSortMode.None)
					select new ValueDropdownItem(string.Concat(new string[]
					{
						x.transform.parent.parent.name,
						"/",
						x.transform.parent.name,
						"/",
						x.name
					}), x);
			}

			// Token: 0x04007EC9 RID: 32457
			public EndCapSpawnPoint spawnLocation;

			// Token: 0x04007ECA RID: 32458
			public BundleStand bundleStand;
		}
	}
}
