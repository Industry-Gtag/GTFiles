using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CosmeticRoom;
using Cosmetics;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking.Store;
using GorillaTag;
using GorillaTag.Cosmetics;
using GorillaTag.CosmeticSystem;
using GorillaTagScripts;
using GorillaTagScripts.Subscription;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

namespace GorillaNetworking
{
	// Token: 0x020010A9 RID: 4265
	public class CosmeticsController : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
	{
		// Token: 0x17000A22 RID: 2594
		// (get) Token: 0x06006A4D RID: 27213 RVA: 0x00222761 File Offset: 0x00220961
		// (set) Token: 0x06006A4E RID: 27214 RVA: 0x00222769 File Offset: 0x00220969
		public CosmeticInfoV2[] v2_allCosmetics { get; private set; }

		// Token: 0x17000A23 RID: 2595
		// (get) Token: 0x06006A4F RID: 27215 RVA: 0x00222772 File Offset: 0x00220972
		// (set) Token: 0x06006A50 RID: 27216 RVA: 0x0022277A File Offset: 0x0022097A
		public bool v2_allCosmeticsInfoAssetRef_isLoaded { get; private set; }

		// Token: 0x17000A24 RID: 2596
		// (get) Token: 0x06006A51 RID: 27217 RVA: 0x00222783 File Offset: 0x00220983
		// (set) Token: 0x06006A52 RID: 27218 RVA: 0x0022278B File Offset: 0x0022098B
		public bool v2_isCosmeticPlayFabCatalogDataLoaded { get; private set; }

		// Token: 0x06006A53 RID: 27219 RVA: 0x00222794 File Offset: 0x00220994
		private void V2Awake()
		{
			this._allCosmetics = null;
			base.StartCoroutine(this.V2_allCosmeticsInfoAssetRefSO_LoadCoroutine());
		}

		// Token: 0x06006A54 RID: 27220 RVA: 0x002227AA File Offset: 0x002209AA
		private IEnumerator V2_allCosmeticsInfoAssetRefSO_LoadCoroutine()
		{
			while (!PlayFabAuthenticator.instance)
			{
				yield return new WaitForSecondsRealtime(1f);
			}
			float[] retryWaitTimes = new float[]
			{
				1f, 2f, 4f, 4f, 10f, 10f, 10f, 10f, 10f, 10f,
				10f, 10f, 10f, 10f, 30f
			};
			int retryCount = 0;
			AsyncOperationHandle<AllCosmeticsArraySO> newSysAllCosmeticsAsyncOp;
			for (;;)
			{
				Debug.Log(string.Format("Attempting to load runtime key \"{0}\" ", this.v2_allCosmeticsInfoAssetRef.RuntimeKey) + string.Format("(Attempt: {0})", retryCount + 1));
				newSysAllCosmeticsAsyncOp = this.v2_allCosmeticsInfoAssetRef.LoadAssetAsync();
				yield return newSysAllCosmeticsAsyncOp;
				if (ApplicationQuittingState.IsQuitting)
				{
					break;
				}
				if (!newSysAllCosmeticsAsyncOp.IsValid())
				{
					Debug.LogError("`newSysAllCosmeticsAsyncOp` (should never happen) became invalid some how.");
				}
				if (newSysAllCosmeticsAsyncOp.Status == AsyncOperationStatus.Succeeded)
				{
					goto Block_4;
				}
				Debug.LogError(string.Format("Failed to load \"{0}\". ", this.v2_allCosmeticsInfoAssetRef.RuntimeKey) + "Error: " + newSysAllCosmeticsAsyncOp.OperationException.Message);
				float num = retryWaitTimes[Mathf.Min(retryCount, retryWaitTimes.Length - 1)];
				yield return new WaitForSecondsRealtime(num);
				int num2 = retryCount;
				retryCount = num2 + 1;
				newSysAllCosmeticsAsyncOp = default(AsyncOperationHandle<AllCosmeticsArraySO>);
			}
			yield break;
			Block_4:
			this.V2_allCosmeticsInfoAssetRef_LoadSucceeded(newSysAllCosmeticsAsyncOp.Result);
			yield break;
		}

		// Token: 0x06006A55 RID: 27221 RVA: 0x002227BC File Offset: 0x002209BC
		private void V2_allCosmeticsInfoAssetRef_LoadSucceeded(AllCosmeticsArraySO allCosmeticsSO)
		{
			this.v2_allCosmetics = new CosmeticInfoV2[allCosmeticsSO.sturdyAssetRefs.Length];
			for (int i = 0; i < allCosmeticsSO.sturdyAssetRefs.Length; i++)
			{
				this.v2_allCosmetics[i] = allCosmeticsSO.sturdyAssetRefs[i].obj.info;
			}
			this._allCosmetics = new List<CosmeticsController.CosmeticItem>(allCosmeticsSO.sturdyAssetRefs.Length);
			for (int j = 0; j < this.v2_allCosmetics.Length; j++)
			{
				CosmeticInfoV2 cosmeticInfoV = this.v2_allCosmetics[j];
				string playFabID = cosmeticInfoV.playFabID;
				this._allCosmeticsDictV2[playFabID] = cosmeticInfoV;
				CosmeticsController.CosmeticItem cosmeticItem = default(CosmeticsController.CosmeticItem);
				cosmeticItem.itemName = playFabID;
				cosmeticItem.itemCategory = cosmeticInfoV.category;
				cosmeticItem.isHoldable = cosmeticInfoV.hasHoldableParts;
				cosmeticItem.displayName = playFabID;
				cosmeticItem.itemPicture = cosmeticInfoV.icon;
				cosmeticItem.overrideDisplayName = cosmeticInfoV.displayName;
				cosmeticItem.bothHandsHoldable = cosmeticInfoV.usesBothHandSlots;
				cosmeticItem.isNullItem = false;
				cosmeticItem.collectionParentLinks = cosmeticInfoV.collectionParentLinks;
				CosmeticCollectionSlotDefinition[] collectionSlots = cosmeticInfoV.collectionSlots;
				cosmeticItem.collectionSlotCount = ((collectionSlots != null) ? collectionSlots.Length : 0);
				cosmeticItem.collectionIsCycling = cosmeticInfoV.collectionIsCycling;
				cosmeticItem.collectionUsesIndexTargeting = cosmeticInfoV.collectionUsesIndexTargeting;
				cosmeticItem.appliedCosmeticPlayFabID = cosmeticInfoV.appliedCosmeticPlayFabID ?? string.Empty;
				CosmeticsController.CosmeticItem cosmeticItem2 = cosmeticItem;
				this._allCosmetics.Add(cosmeticItem2);
			}
			this.collectablesByParentID = new Dictionary<string, List<CosmeticsController.CosmeticItem>>();
			for (int k = 0; k < this._allCosmetics.Count; k++)
			{
				CosmeticCollectionParentLink[] collectionParentLinks = this._allCosmetics[k].collectionParentLinks;
				if (collectionParentLinks != null)
				{
					for (int l = 0; l < collectionParentLinks.Length; l++)
					{
						string parentPlayFabID = collectionParentLinks[l].parentPlayFabID;
						if (!string.IsNullOrEmpty(parentPlayFabID))
						{
							List<CosmeticsController.CosmeticItem> list;
							if (!this.collectablesByParentID.TryGetValue(parentPlayFabID, out list))
							{
								list = new List<CosmeticsController.CosmeticItem>();
								this.collectablesByParentID[parentPlayFabID] = list;
							}
							list.Add(this._allCosmetics[k]);
						}
					}
				}
			}
			this.v2_allCosmeticsInfoAssetRef_isLoaded = true;
			Action v2_allCosmeticsInfoAssetRef_OnPostLoad = this.V2_allCosmeticsInfoAssetRef_OnPostLoad;
			if (v2_allCosmeticsInfoAssetRef_OnPostLoad == null)
			{
				return;
			}
			v2_allCosmeticsInfoAssetRef_OnPostLoad();
		}

		// Token: 0x06006A56 RID: 27222 RVA: 0x002229EA File Offset: 0x00220BEA
		public bool TryGetCosmeticInfoV2(string playFabId, out CosmeticInfoV2 cosmeticInfo)
		{
			return this._allCosmeticsDictV2.TryGetValue(playFabId, out cosmeticInfo);
		}

		// Token: 0x06006A57 RID: 27223 RVA: 0x002229F9 File Offset: 0x00220BF9
		private void V2_ConformCosmeticItemV1DisplayName(ref CosmeticsController.CosmeticItem cosmetic)
		{
			if (cosmetic.itemName == cosmetic.displayName)
			{
				return;
			}
			cosmetic.overrideDisplayName = cosmetic.displayName;
			cosmetic.displayName = cosmetic.itemName;
		}

		// Token: 0x06006A58 RID: 27224 RVA: 0x00222A28 File Offset: 0x00220C28
		internal void InitializeCosmeticStands()
		{
			foreach (CosmeticStand cosmeticStand in this.cosmeticStands)
			{
				if (cosmeticStand != null)
				{
					cosmeticStand.InitializeCosmetic();
				}
			}
		}

		// Token: 0x17000A25 RID: 2597
		// (get) Token: 0x06006A59 RID: 27225 RVA: 0x00222A5D File Offset: 0x00220C5D
		// (set) Token: 0x06006A5A RID: 27226 RVA: 0x00222A64 File Offset: 0x00220C64
		public static bool hasInstance { get; private set; }

		// Token: 0x17000A26 RID: 2598
		// (get) Token: 0x06006A5B RID: 27227 RVA: 0x00222A6C File Offset: 0x00220C6C
		// (set) Token: 0x06006A5C RID: 27228 RVA: 0x00222A74 File Offset: 0x00220C74
		public string PurchaseLocation
		{
			get
			{
				return this.purchaseLocation;
			}
			set
			{
				this.purchaseLocation = value;
			}
		}

		// Token: 0x06006A5D RID: 27229 RVA: 0x00222A80 File Offset: 0x00220C80
		private string ConsumePurchaseLocation()
		{
			if (this.purchaseLocation.IsNullOrEmpty())
			{
				return GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone.ToString();
			}
			string text = this.purchaseLocation;
			this.purchaseLocation = null;
			return text;
		}

		// Token: 0x17000A27 RID: 2599
		// (get) Token: 0x06006A5E RID: 27230 RVA: 0x00222ACA File Offset: 0x00220CCA
		// (set) Token: 0x06006A5F RID: 27231 RVA: 0x00222AD2 File Offset: 0x00220CD2
		public List<CosmeticsController.CosmeticItem> allCosmetics
		{
			get
			{
				return this._allCosmetics;
			}
			set
			{
				this._allCosmetics = value;
			}
		}

		// Token: 0x17000A28 RID: 2600
		// (get) Token: 0x06006A60 RID: 27232 RVA: 0x00222ADB File Offset: 0x00220CDB
		// (set) Token: 0x06006A61 RID: 27233 RVA: 0x00222AE3 File Offset: 0x00220CE3
		public bool allCosmeticsDict_isInitialized { get; private set; }

		// Token: 0x17000A29 RID: 2601
		// (get) Token: 0x06006A62 RID: 27234 RVA: 0x00222AEC File Offset: 0x00220CEC
		public Dictionary<string, CosmeticsController.CosmeticItem> allCosmeticsDict
		{
			get
			{
				return this._allCosmeticsDict;
			}
		}

		// Token: 0x17000A2A RID: 2602
		// (get) Token: 0x06006A63 RID: 27235 RVA: 0x00222AF4 File Offset: 0x00220CF4
		// (set) Token: 0x06006A64 RID: 27236 RVA: 0x00222AFC File Offset: 0x00220CFC
		public bool allCosmeticsItemIDsfromDisplayNamesDict_isInitialized { get; private set; }

		// Token: 0x17000A2B RID: 2603
		// (get) Token: 0x06006A65 RID: 27237 RVA: 0x00222B05 File Offset: 0x00220D05
		public Dictionary<string, string> allCosmeticsItemIDsfromDisplayNamesDict
		{
			get
			{
				return this._allCosmeticsItemIDsfromDisplayNamesDict;
			}
		}

		// Token: 0x17000A2C RID: 2604
		// (get) Token: 0x06006A66 RID: 27238 RVA: 0x00222B0D File Offset: 0x00220D0D
		public CosmeticAnchorAntiIntersectOffsets defaultClipOffsets
		{
			get
			{
				return CosmeticAnchorAntiIntersectOffsets.Identity;
			}
		}

		// Token: 0x17000A2D RID: 2605
		// (get) Token: 0x06006A67 RID: 27239 RVA: 0x00222B14 File Offset: 0x00220D14
		// (set) Token: 0x06006A68 RID: 27240 RVA: 0x00222B1C File Offset: 0x00220D1C
		public bool isHidingCosmeticsFromRemotePlayers { get; private set; }

		// Token: 0x06006A69 RID: 27241 RVA: 0x00222B25 File Offset: 0x00220D25
		public void AddWardrobeInstance(WardrobeInstance instance)
		{
			this.wardrobes.Add(instance);
			this.UpdateWardrobeModelsAndButtons();
		}

		// Token: 0x06006A6A RID: 27242 RVA: 0x00222B39 File Offset: 0x00220D39
		public void RemoveWardrobeInstance(WardrobeInstance instance)
		{
			this.wardrobes.Remove(instance);
		}

		// Token: 0x06006A6B RID: 27243 RVA: 0x00222B48 File Offset: 0x00220D48
		public bool IsOwnedByPlayFabID(string playFabID)
		{
			return this.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => x.itemName == playFabID) >= 0;
		}

		// Token: 0x06006A6C RID: 27244 RVA: 0x00222B80 File Offset: 0x00220D80
		public int GetOwnedCollectableCount(string parentPlayFabID)
		{
			int num = 0;
			for (int i = 0; i < this.unlockedCosmetics.Count; i++)
			{
				if (this.unlockedCosmetics[i].IsCollectableOf(parentPlayFabID))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06006A6D RID: 27245 RVA: 0x00222BC4 File Offset: 0x00220DC4
		private int GetRemainingCollectableSlots(string parentPlayFabID)
		{
			CosmeticsController.CosmeticItem cosmeticItem;
			if (!this.allCosmeticsDict.TryGetValue(parentPlayFabID, out cosmeticItem))
			{
				return 0;
			}
			List<CosmeticsController.CosmeticItem> list;
			int num = (cosmeticItem.collectionIsCycling ? (this.collectablesByParentID.TryGetValue(parentPlayFabID, out list) ? list.Count : 0) : cosmeticItem.collectionSlotCount) - this.GetOwnedCollectableCount(parentPlayFabID);
			if (num <= 0)
			{
				return 0;
			}
			return num;
		}

		// Token: 0x06006A6E RID: 27246 RVA: 0x00222C1C File Offset: 0x00220E1C
		public bool CanPurchaseCollectable(string collectablePlayFabID)
		{
			CosmeticsController.CosmeticItem cosmeticItem;
			if (!this.allCosmeticsDict.TryGetValue(collectablePlayFabID, out cosmeticItem))
			{
				return false;
			}
			if (!cosmeticItem.IsCollectable)
			{
				return true;
			}
			CosmeticCollectionParentLink[] collectionParentLinks = cosmeticItem.collectionParentLinks;
			for (int i = 0; i < collectionParentLinks.Length; i++)
			{
				string parentPlayFabID = collectionParentLinks[i].parentPlayFabID;
				if (!string.IsNullOrEmpty(parentPlayFabID) && this.IsOwnedByPlayFabID(parentPlayFabID) && this.GetRemainingCollectableSlots(parentPlayFabID) > 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006A6F RID: 27247 RVA: 0x00222C88 File Offset: 0x00220E88
		public void BuildCanonicalCollectableOrder(string parentPlayFabID, List<CosmeticsController.CosmeticItem> result)
		{
			result.Clear();
			List<CosmeticsController.CosmeticItem> list;
			if (string.IsNullOrEmpty(parentPlayFabID) || !this.collectablesByParentID.TryGetValue(parentPlayFabID, out list))
			{
				return;
			}
			result.AddRange(list);
			CosmeticInfoV2 cosmeticInfoV;
			bool seriesOrder = this.TryGetCosmeticInfoV2(parentPlayFabID, out cosmeticInfoV) && cosmeticInfoV.collectionIsCycling && cosmeticInfoV.collectionUsesSeriesOrder;
			result.Sort(delegate(CosmeticsController.CosmeticItem a, CosmeticsController.CosmeticItem b)
			{
				if (seriesOrder)
				{
					int num = a.GetSeriesIndexForParent(parentPlayFabID);
					int num2 = b.GetSeriesIndexForParent(parentPlayFabID);
					if (num < 0)
					{
						num = int.MaxValue;
					}
					if (num2 < 0)
					{
						num2 = int.MaxValue;
					}
					if (num != num2)
					{
						return num.CompareTo(num2);
					}
				}
				return string.CompareOrdinal(a.itemName, b.itemName);
			});
		}

		// Token: 0x06006A70 RID: 27248 RVA: 0x00222D0C File Offset: 0x00220F0C
		public int GetCanonicalCollectableIndex(string parentPlayFabID, string itemName)
		{
			this.BuildCanonicalCollectableOrder(parentPlayFabID, CosmeticsController.scratchCanonicalIndexList);
			for (int i = 0; i < CosmeticsController.scratchCanonicalIndexList.Count; i++)
			{
				if (CosmeticsController.scratchCanonicalIndexList[i].itemName == itemName)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06006A71 RID: 27249 RVA: 0x00222D58 File Offset: 0x00220F58
		public static void PopulateCollectionDisplayOnRoot(GameObject rootObj, CosmeticsController.CosmeticItem parentItem, VRRig rig)
		{
			if (rootObj == null || parentItem.collectionSlotCount <= 0 || !CosmeticsController.hasInstance)
			{
				return;
			}
			CosmeticInfoV2 cosmeticInfoV;
			if (!CosmeticsController.instance.TryGetCosmeticInfoV2(parentItem.itemName, out cosmeticInfoV))
			{
				return;
			}
			if (cosmeticInfoV.collectionSlots == null || cosmeticInfoV.collectionSlots.Length == 0)
			{
				return;
			}
			CosmeticCollectionDisplay cosmeticCollectionDisplay;
			if (!rootObj.TryGetComponent<CosmeticCollectionDisplay>(out cosmeticCollectionDisplay))
			{
				cosmeticCollectionDisplay = rootObj.AddComponent<CosmeticCollectionDisplay>();
			}
			List<CosmeticsController.CosmeticItem> list = new List<CosmeticsController.CosmeticItem>();
			bool flag = false;
			if (rig.isLocal)
			{
				CosmeticsController.instance.BuildCanonicalCollectableOrder(parentItem.itemName, CosmeticsController.scratchCanonicalCollectables);
				CosmeticsController.CosmeticItem cosmeticItem = CosmeticsController.instance.tryOnCollectableItem;
				flag = !cosmeticItem.isNullItem && cosmeticItem.IsCollectableOf(parentItem.itemName) && VRRig.LocalRig != null && VRRig.LocalRig.inTryOnRoom;
				for (int i = 0; i < CosmeticsController.scratchCanonicalCollectables.Count; i++)
				{
					CosmeticsController.CosmeticItem cosmeticItem2 = CosmeticsController.scratchCanonicalCollectables[i];
					bool flag2 = CosmeticsController.instance.IsOwnedByPlayFabID(cosmeticItem2.itemName);
					bool flag3 = flag && cosmeticItem2.itemName == cosmeticItem.itemName;
					if (flag2 || flag3)
					{
						list.Add(cosmeticItem2);
					}
				}
				if (cosmeticCollectionDisplay.ContentMatches(list))
				{
					CosmeticCollectionDisplay.Register(rig, parentItem.itemName, cosmeticCollectionDisplay, rig.isLocal);
					return;
				}
			}
			else
			{
				CosmeticsController.instance.BuildCanonicalCollectableOrder(parentItem.itemName, list);
				if (cosmeticCollectionDisplay.ContentMatches(list))
				{
					CosmeticCollectionDisplay.Register(rig, parentItem.itemName, cosmeticCollectionDisplay, rig.isLocal);
					CosmeticsController.CollectionState collectionState;
					if (rig.remoteCycleStates.TryGetValue(parentItem.itemName, out collectionState))
					{
						cosmeticCollectionDisplay.SetVisibleMask(collectionState.visibleMask);
						cosmeticCollectionDisplay.SetActiveIndex(collectionState.activeIndex);
					}
					return;
				}
			}
			cosmeticCollectionDisplay.Populate(list, cosmeticInfoV, rootObj.transform);
			if (flag)
			{
				cosmeticCollectionDisplay.PersistLocalState();
			}
			CosmeticCollectionDisplay.Register(rig, parentItem.itemName, cosmeticCollectionDisplay, rig.isLocal);
			CosmeticsController.CollectionState collectionState3;
			if (rig.isLocal)
			{
				CosmeticsController.CollectionState collectionState2;
				if (CosmeticsController.instance.localCycleStates.TryGetValue(new ValueTuple<VRRig, string>(rig, parentItem.itemName), out collectionState2))
				{
					cosmeticCollectionDisplay.SetVisibleMask(collectionState2.visibleMask);
					cosmeticCollectionDisplay.SetActiveIndex(collectionState2.activeIndex);
					return;
				}
			}
			else if (rig.remoteCycleStates.TryGetValue(parentItem.itemName, out collectionState3))
			{
				cosmeticCollectionDisplay.SetVisibleMask(collectionState3.visibleMask);
				cosmeticCollectionDisplay.SetActiveIndex(collectionState3.activeIndex);
			}
		}

		// Token: 0x17000A2E RID: 2606
		// (get) Token: 0x06006A72 RID: 27250 RVA: 0x00222F9F File Offset: 0x0022119F
		public int CurrencyBalance
		{
			get
			{
				return this.currencyBalance;
			}
		}

		// Token: 0x17000A2F RID: 2607
		// (get) Token: 0x06006A73 RID: 27251 RVA: 0x00222FA7 File Offset: 0x002211A7
		public CosmeticSO EarlyAccessSupporterPackCosmeticSO
		{
			get
			{
				return this.m_earlyAccessSupporterPackCosmeticSO;
			}
		}

		// Token: 0x06006A74 RID: 27252 RVA: 0x00222FB0 File Offset: 0x002211B0
		public void Awake()
		{
			if (CosmeticsController.instance == null)
			{
				CosmeticsController.instance = this;
				CosmeticsController.hasInstance = true;
			}
			else if (CosmeticsController.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.V2Awake();
			if (base.gameObject.activeSelf)
			{
				this.catalog = "DLC";
				this.currencyName = "SR";
				this.nullItem = default(CosmeticsController.CosmeticItem);
				this.nullItem.itemName = "null";
				this.nullItem.displayName = "NOTHING";
				this.nullItem.itemPicture = Resources.Load<Sprite>("CosmeticNull_Icon");
				this.nullItem.itemPictureResourceString = "";
				this.nullItem.overrideDisplayName = "NOTHING";
				this.nullItem.meshAtlasResourceString = "";
				this.nullItem.meshResourceString = "";
				this.nullItem.materialResourceString = "";
				this.nullItem.isNullItem = true;
				this._allCosmeticsDict[this.nullItem.itemName] = this.nullItem;
				this._allCosmeticsItemIDsfromDisplayNamesDict[this.nullItem.displayName] = this.nullItem.itemName;
				this.tryOnCollectableItem = this.nullItem;
				for (int i = 0; i < 16; i++)
				{
					this.tryOnSet.items[i] = this.nullItem;
					this.tempUnlockedSet.items[i] = this.nullItem;
					this.activeMergedSet.items[i] = this.nullItem;
				}
				this.cosmeticsPages[0] = 0;
				this.cosmeticsPages[1] = 0;
				this.cosmeticsPages[2] = 0;
				this.cosmeticsPages[3] = 0;
				this.cosmeticsPages[4] = 0;
				this.cosmeticsPages[5] = 0;
				this.cosmeticsPages[6] = 0;
				this.cosmeticsPages[7] = 0;
				this.cosmeticsPages[8] = 0;
				this.cosmeticsPages[9] = 0;
				this.cosmeticsPages[10] = 0;
				this.itemLists[0] = this.unlockedHats;
				this.itemLists[1] = this.unlockedFaces;
				this.itemLists[2] = this.unlockedBadges;
				this.itemLists[3] = this.unlockedPaws;
				this.itemLists[4] = this.unlockedFurs;
				this.itemLists[5] = this.unlockedShirts;
				this.itemLists[6] = this.unlockedPants;
				this.itemLists[7] = this.unlockedArms;
				this.itemLists[8] = this.unlockedBacks;
				this.itemLists[9] = this.unlockedChests;
				this.itemLists[10] = this.unlockedTagFX;
				this.updateCosmeticsRetries = 0;
				this.maxUpdateCosmeticsRetries = 5;
				base.StartCoroutine(this.CheckCanGetDaily());
			}
			CreatorCodes.Initialize();
		}

		// Token: 0x06006A75 RID: 27253 RVA: 0x0022327C File Offset: 0x0022147C
		public void Start()
		{
			PlayFabTitleDataCache.Instance.GetTitleData("BundleData", delegate(string data)
			{
				this.bundleList.FromJson(data);
			}, delegate(PlayFabError e)
			{
				Debug.LogError(string.Format("Error getting bundle data: {0}", e));
			}, false);
			this.anchorOverrides = GorillaTagger.Instance.offlineVRRig.GetComponent<VRRigAnchorOverrides>();
		}

		// Token: 0x06006A76 RID: 27254 RVA: 0x002232D9 File Offset: 0x002214D9
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			if (SteamManager.Initialized && this._steamMicroTransactionAuthorizationResponse == null)
			{
				this._steamMicroTransactionAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(new Callback<MicroTxnAuthorizationResponse_t>.DispatchDelegate(this.ProcessSteamCallback));
			}
		}

		// Token: 0x06006A77 RID: 27255 RVA: 0x00223308 File Offset: 0x00221508
		public void OnDisable()
		{
			Callback<MicroTxnAuthorizationResponse_t> steamMicroTransactionAuthorizationResponse = this._steamMicroTransactionAuthorizationResponse;
			if (steamMicroTransactionAuthorizationResponse != null)
			{
				steamMicroTransactionAuthorizationResponse.Unregister();
			}
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06006A78 RID: 27256 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void SliceUpdate()
		{
		}

		// Token: 0x06006A79 RID: 27257 RVA: 0x00223324 File Offset: 0x00221524
		public static bool CompareCategoryToSavedCosmeticSlots(CosmeticsController.CosmeticCategory category, CosmeticsController.CosmeticSlots slot)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return slot == CosmeticsController.CosmeticSlots.Hat;
			case CosmeticsController.CosmeticCategory.Badge:
				return CosmeticsController.CosmeticSlots.Badge == slot;
			case CosmeticsController.CosmeticCategory.Face:
				return CosmeticsController.CosmeticSlots.Face == slot;
			case CosmeticsController.CosmeticCategory.Paw:
				return slot == CosmeticsController.CosmeticSlots.HandRight || slot == CosmeticsController.CosmeticSlots.HandLeft;
			case CosmeticsController.CosmeticCategory.Chest:
				return CosmeticsController.CosmeticSlots.Chest == slot;
			case CosmeticsController.CosmeticCategory.Fur:
				return CosmeticsController.CosmeticSlots.Fur == slot;
			case CosmeticsController.CosmeticCategory.Shirt:
				return CosmeticsController.CosmeticSlots.Shirt == slot;
			case CosmeticsController.CosmeticCategory.Back:
				return slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.BackRight;
			case CosmeticsController.CosmeticCategory.Arms:
				return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.ArmRight;
			case CosmeticsController.CosmeticCategory.Pants:
				return CosmeticsController.CosmeticSlots.Pants == slot;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return CosmeticsController.CosmeticSlots.TagEffect == slot;
			default:
				return false;
			}
		}

		// Token: 0x06006A7A RID: 27258 RVA: 0x002233B8 File Offset: 0x002215B8
		public static CosmeticsController.CosmeticSlots CategoryToNonTransferrableSlot(CosmeticsController.CosmeticCategory category)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return CosmeticsController.CosmeticSlots.Hat;
			case CosmeticsController.CosmeticCategory.Badge:
				return CosmeticsController.CosmeticSlots.Badge;
			case CosmeticsController.CosmeticCategory.Face:
				return CosmeticsController.CosmeticSlots.Face;
			case CosmeticsController.CosmeticCategory.Paw:
				return CosmeticsController.CosmeticSlots.HandRight;
			case CosmeticsController.CosmeticCategory.Chest:
				return CosmeticsController.CosmeticSlots.Chest;
			case CosmeticsController.CosmeticCategory.Fur:
				return CosmeticsController.CosmeticSlots.Fur;
			case CosmeticsController.CosmeticCategory.Shirt:
				return CosmeticsController.CosmeticSlots.Shirt;
			case CosmeticsController.CosmeticCategory.Back:
				return CosmeticsController.CosmeticSlots.Back;
			case CosmeticsController.CosmeticCategory.Arms:
				return CosmeticsController.CosmeticSlots.Arms;
			case CosmeticsController.CosmeticCategory.Pants:
				return CosmeticsController.CosmeticSlots.Pants;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return CosmeticsController.CosmeticSlots.TagEffect;
			default:
				return CosmeticsController.CosmeticSlots.Count;
			}
		}

		// Token: 0x06006A7B RID: 27259 RVA: 0x0022341A File Offset: 0x0022161A
		private CosmeticsController.CosmeticSlots DropPositionToCosmeticSlot(BodyDockPositions.DropPositions pos)
		{
			switch (pos)
			{
			case BodyDockPositions.DropPositions.LeftArm:
				return CosmeticsController.CosmeticSlots.ArmLeft;
			case BodyDockPositions.DropPositions.RightArm:
				return CosmeticsController.CosmeticSlots.ArmRight;
			case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
				break;
			case BodyDockPositions.DropPositions.Chest:
				return CosmeticsController.CosmeticSlots.Chest;
			default:
				if (pos == BodyDockPositions.DropPositions.LeftBack)
				{
					return CosmeticsController.CosmeticSlots.BackLeft;
				}
				if (pos == BodyDockPositions.DropPositions.RightBack)
				{
					return CosmeticsController.CosmeticSlots.BackRight;
				}
				break;
			}
			return CosmeticsController.CosmeticSlots.Count;
		}

		// Token: 0x06006A7C RID: 27260 RVA: 0x0022344C File Offset: 0x0022164C
		private static BodyDockPositions.DropPositions CosmeticSlotToDropPosition(CosmeticsController.CosmeticSlots slot)
		{
			switch (slot)
			{
			case CosmeticsController.CosmeticSlots.ArmLeft:
				return BodyDockPositions.DropPositions.LeftArm;
			case CosmeticsController.CosmeticSlots.ArmRight:
				return BodyDockPositions.DropPositions.RightArm;
			case CosmeticsController.CosmeticSlots.BackLeft:
				return BodyDockPositions.DropPositions.LeftBack;
			case CosmeticsController.CosmeticSlots.BackRight:
				return BodyDockPositions.DropPositions.RightBack;
			case CosmeticsController.CosmeticSlots.Chest:
				return BodyDockPositions.DropPositions.Chest;
			}
			return BodyDockPositions.DropPositions.None;
		}

		// Token: 0x06006A7D RID: 27261 RVA: 0x00223480 File Offset: 0x00221680
		public void AddItemCheckout(ItemCheckout newItemCheckout)
		{
			if (this.itemCheckouts.Contains(newItemCheckout))
			{
				return;
			}
			this.itemCheckouts.Add(newItemCheckout);
			this.UpdateShoppingCart();
			this.FormattedPurchaseText(this.finalLine, this.leftCheckoutPurchaseButtonString, this.rightCheckoutPurchaseButtonString, this.leftCheckoutPurchaseButtonOn, this.rightCheckoutPurchaseButtonOn);
			if (!this.itemToBuy.isNullItem)
			{
				this.RefreshItemToBuyPreview();
			}
		}

		// Token: 0x06006A7E RID: 27262 RVA: 0x002234E5 File Offset: 0x002216E5
		public void RemoveItemCheckout(ItemCheckout checkoutToRemove)
		{
			this.itemCheckouts.RemoveIfContains(checkoutToRemove);
		}

		// Token: 0x06006A7F RID: 27263 RVA: 0x002234F3 File Offset: 0x002216F3
		public void AddFittingRoom(FittingRoom newFittingRoom)
		{
			if (this.fittingRooms.Contains(newFittingRoom))
			{
				return;
			}
			this.fittingRooms.Add(newFittingRoom);
			this.UpdateShoppingCart();
		}

		// Token: 0x06006A80 RID: 27264 RVA: 0x00223516 File Offset: 0x00221716
		public void RemoveFittingRoom(FittingRoom fittingRoomToRemove)
		{
			this.fittingRooms.RemoveIfContains(fittingRoomToRemove);
		}

		// Token: 0x06006A81 RID: 27265 RVA: 0x00223524 File Offset: 0x00221724
		private void SaveItemPreference(CosmeticsController.CosmeticSlots slot, int slotIdx, CosmeticsController.CosmeticItem newItem)
		{
			PlayerPrefs.SetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), newItem.itemName);
			PlayerPrefs.Save();
		}

		// Token: 0x06006A82 RID: 27266 RVA: 0x0022353C File Offset: 0x0022173C
		public void SaveCurrentItemPreferences()
		{
			for (int i = 0; i < 16; i++)
			{
				CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots)i;
				CosmeticsController.CosmeticItem cosmeticItem = this.currentWornSet.items[i];
				if (cosmeticItem.itemName == "Slingshot")
				{
					cosmeticItem = this.nullItem;
				}
				this.SaveItemPreference(cosmeticSlots, i, cosmeticItem);
			}
		}

		// Token: 0x06006A83 RID: 27267 RVA: 0x0022358C File Offset: 0x0022178C
		private void ApplyCosmeticToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, int slotIdx, CosmeticsController.CosmeticSlots slot, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> appliedSlots)
		{
			CosmeticsController.CosmeticItem cosmeticItem = ((set.items[slotIdx].itemName == newItem.itemName) ? this.nullItem : newItem);
			set.items[slotIdx] = cosmeticItem;
			if (applyToPlayerPrefs)
			{
				this.SaveItemPreference(slot, slotIdx, cosmeticItem);
			}
			appliedSlots.Add(slot);
		}

		// Token: 0x06006A84 RID: 27268 RVA: 0x002235E5 File Offset: 0x002217E5
		public static void ClearTryOnCollectable()
		{
			if (!CosmeticsController.hasInstance)
			{
				return;
			}
			CosmeticsController.instance.tryOnCollectableItem = CosmeticsController.instance.nullItem;
		}

		// Token: 0x06006A85 RID: 27269 RVA: 0x00223608 File Offset: 0x00221808
		private void PrivApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> appliedSlots)
		{
			if (newItem.isNullItem)
			{
				return;
			}
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Collectable)
			{
				if (set == this.tryOnSet)
				{
					this.tryOnCollectableItem = newItem;
				}
				return;
			}
			if (set == this.tryOnSet)
			{
				CosmeticsController.ClearTryOnCollectable();
			}
			VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(newItem.itemName);
			if (CosmeticsController.CosmeticSet.IsHoldable(newItem))
			{
				BodyDockPositions.DockingResult dockingResult = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>().ToggleWithHandedness(newItem.displayName, isLeftHand, newItem.bothHandsHoldable);
				foreach (BodyDockPositions.DropPositions dropPositions in dockingResult.positionsDisabled)
				{
					CosmeticsController.CosmeticSlots cosmeticSlots = this.DropPositionToCosmeticSlot(dropPositions);
					if (cosmeticSlots != CosmeticsController.CosmeticSlots.Count)
					{
						int num = (int)cosmeticSlots;
						set.items[num] = this.nullItem;
						if (applyToPlayerPrefs)
						{
							this.SaveItemPreference(cosmeticSlots, num, this.nullItem);
						}
					}
				}
				using (List<BodyDockPositions.DropPositions>.Enumerator enumerator = dockingResult.dockedPosition.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BodyDockPositions.DropPositions dropPositions2 = enumerator.Current;
						if (dropPositions2 != BodyDockPositions.DropPositions.None)
						{
							CosmeticsController.CosmeticSlots cosmeticSlots2 = this.DropPositionToCosmeticSlot(dropPositions2);
							int num2 = (int)cosmeticSlots2;
							set.items[num2] = newItem;
							if (applyToPlayerPrefs)
							{
								this.SaveItemPreference(cosmeticSlots2, num2, newItem);
							}
							appliedSlots.Add(cosmeticSlots2);
						}
					}
					return;
				}
			}
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Paw)
			{
				CosmeticsController.CosmeticSlots cosmeticSlots3 = (isLeftHand ? CosmeticsController.CosmeticSlots.HandLeft : CosmeticsController.CosmeticSlots.HandRight);
				int num3 = (int)cosmeticSlots3;
				this.ApplyCosmeticToSet(set, newItem, num3, cosmeticSlots3, applyToPlayerPrefs, appliedSlots);
				CosmeticsController.CosmeticSlots cosmeticSlots4 = CosmeticsController.CosmeticSet.OppositeSlot(cosmeticSlots3);
				int num4 = (int)cosmeticSlots4;
				if (newItem.bothHandsHoldable)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num4, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
					return;
				}
				if (set.items[num4].itemName == newItem.itemName)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num4, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
				}
				if (set.items[num4].bothHandsHoldable)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num4, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
					return;
				}
			}
			else
			{
				CosmeticsController.CosmeticSlots cosmeticSlots5 = CosmeticsController.CategoryToNonTransferrableSlot(newItem.itemCategory);
				if (cosmeticSlots5 == CosmeticsController.CosmeticSlots.Count)
				{
					return;
				}
				int num5 = (int)cosmeticSlots5;
				this.ApplyCosmeticToSet(set, newItem, num5, cosmeticSlots5, applyToPlayerPrefs, appliedSlots);
			}
		}

		// Token: 0x06006A86 RID: 27270 RVA: 0x00223850 File Offset: 0x00221A50
		public void ApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs)
		{
			this.ApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, CosmeticsController._g_default_outAppliedSlotsList_for_applyCosmeticItemToSet);
		}

		// Token: 0x06006A87 RID: 27271 RVA: 0x00223864 File Offset: 0x00221A64
		public void ApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> outAppliedSlotsList)
		{
			outAppliedSlotsList.Clear();
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Set)
			{
				bool flag = false;
				Dictionary<CosmeticsController.CosmeticItem, bool> dictionary = new Dictionary<CosmeticsController.CosmeticItem, bool>();
				foreach (string text in newItem.bundledItems)
				{
					CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(text);
					if (this.AnyMatch(set, itemFromDict))
					{
						flag = true;
						dictionary.Add(itemFromDict, true);
					}
					else
					{
						dictionary.Add(itemFromDict, false);
					}
				}
				using (Dictionary<CosmeticsController.CosmeticItem, bool>.Enumerator enumerator = dictionary.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<CosmeticsController.CosmeticItem, bool> keyValuePair = enumerator.Current;
						if (flag)
						{
							if (keyValuePair.Value)
							{
								this.PrivApplyCosmeticItemToSet(set, keyValuePair.Key, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
							}
						}
						else
						{
							this.PrivApplyCosmeticItemToSet(set, keyValuePair.Key, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
						}
					}
					return;
				}
			}
			this.PrivApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
		}

		// Token: 0x06006A88 RID: 27272 RVA: 0x00223950 File Offset: 0x00221B50
		public void RemoveCosmeticItemFromSet(CosmeticsController.CosmeticSet set, string itemName, bool applyToPlayerPrefs)
		{
			this.cachedSet.CopyItems(set);
			for (int i = 0; i < 16; i++)
			{
				if (set.items[i].displayName == itemName)
				{
					set.items[i] = this.nullItem;
					if (applyToPlayerPrefs)
					{
						this.SaveItemPreference((CosmeticsController.CosmeticSlots)i, i, this.nullItem);
					}
				}
			}
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			BodyDockPositions component = offlineVRRig.GetComponent<BodyDockPositions>();
			set.ActivateCosmetics(this.cachedSet, offlineVRRig, component, offlineVRRig.cosmeticsObjectRegistry);
		}

		// Token: 0x06006A89 RID: 27273 RVA: 0x002239D8 File Offset: 0x00221BD8
		private async void RepressButton(FittingRoomButton pressedButton, bool isLeftHand)
		{
			float timeEntered = Time.time;
			float maxTime = 1f;
			if (pressedButton.currentCosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Set)
			{
				CosmeticsController.CosmeticItem itemSet = pressedButton.currentCosmeticItem;
				bool flag = true;
				while (flag)
				{
					if (Time.time > timeEntered + maxTime)
					{
						return;
					}
					await Awaitable.EndOfFrameAsync(default(CancellationToken));
					flag = false;
					for (int i = 0; i < itemSet.bundledItems.Length; i++)
					{
						if (VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(itemSet.bundledItems[i]) == null)
						{
							flag = true;
						}
					}
				}
				itemSet = default(CosmeticsController.CosmeticItem);
			}
			else
			{
				while (VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(pressedButton.currentCosmeticItem.itemName) == null)
				{
					if (Time.time > timeEntered + maxTime)
					{
						return;
					}
					await Awaitable.EndOfFrameAsync(default(CancellationToken));
				}
			}
			this.PressFittingRoomButton(pressedButton, isLeftHand);
		}

		// Token: 0x06006A8A RID: 27274 RVA: 0x00223A20 File Offset: 0x00221C20
		public void PressFittingRoomButton(FittingRoomButton pressedFittingRoomButton, bool isLeftHand)
		{
			if (pressedFittingRoomButton.currentCosmeticItem.itemName == null || pressedFittingRoomButton.currentCosmeticItem.itemName == this.nullItem.itemName || pressedFittingRoomButton.currentCosmeticItem.itemName == "")
			{
				return;
			}
			if (pressedFittingRoomButton.currentCosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Set)
			{
				CosmeticsController.CosmeticItem currentCosmeticItem = pressedFittingRoomButton.currentCosmeticItem;
				bool flag = false;
				for (int i = 0; i < currentCosmeticItem.bundledItems.Length; i++)
				{
					if (VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(currentCosmeticItem.bundledItems[i]) == null)
					{
						flag = true;
					}
				}
				if (flag)
				{
					this.RepressButton(pressedFittingRoomButton, isLeftHand);
					return;
				}
			}
			else if (VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(pressedFittingRoomButton.currentCosmeticItem.itemName) == null)
			{
				this.RepressButton(pressedFittingRoomButton, isLeftHand);
				return;
			}
			TryOnBundlesStand tryOnBundlesStand = BundleManager.instance._tryOnBundlesStand;
			if (tryOnBundlesStand != null)
			{
				tryOnBundlesStand.ClearSelectedBundle();
			}
			this.ApplyCosmeticItemToSet(this.tryOnSet, pressedFittingRoomButton.currentCosmeticItem, isLeftHand, false);
			this.UpdateShoppingCart();
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x06006A8B RID: 27275 RVA: 0x00223B20 File Offset: 0x00221D20
		public CosmeticsController.EWearingCosmeticSet CheckIfCosmeticSetMatchesItemSet(CosmeticsController.CosmeticSet set, string itemName)
		{
			CosmeticsController.EWearingCosmeticSet ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.NotASet;
			CosmeticsController.CosmeticItem cosmeticItem = this.allCosmeticsDict[itemName];
			if (cosmeticItem.bundledItems.Length != 0)
			{
				foreach (string text in cosmeticItem.bundledItems)
				{
					if (this.AnyMatch(set, this.allCosmeticsDict[text]))
					{
						if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotASet)
						{
							ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Complete;
						}
						else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotWearing)
						{
							ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Partial;
						}
					}
					else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotASet)
					{
						ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.NotWearing;
					}
					else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.Complete)
					{
						ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Partial;
					}
				}
			}
			return ewearingCosmeticSet;
		}

		// Token: 0x06006A8C RID: 27276 RVA: 0x00223B94 File Offset: 0x00221D94
		public void PressCosmeticStandButton(CosmeticStand pressedStand)
		{
			this.searchIndex = this.currentCart.IndexOf(pressedStand.thisCosmeticItem);
			if (this.searchIndex != -1)
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_remove, pressedStand.thisCosmeticItem);
				this.currentCart.RemoveAt(this.searchIndex);
				pressedStand.isOn = false;
				for (int i = 0; i < 16; i++)
				{
					if (pressedStand.thisCosmeticItem.itemName == this.tryOnSet.items[i].itemName)
					{
						this.tryOnSet.items[i] = this.nullItem;
					}
				}
			}
			else
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_add, pressedStand.thisCosmeticItem);
				this.currentCart.Insert(0, pressedStand.thisCosmeticItem);
				pressedStand.isOn = true;
				if (this.currentCart.Count > this.numFittingRoomButtons)
				{
					foreach (CosmeticStand cosmeticStand in this.cosmeticStands)
					{
						if (!(cosmeticStand == null) && cosmeticStand.thisCosmeticItem.itemName == this.currentCart[this.numFittingRoomButtons].itemName)
						{
							cosmeticStand.isOn = false;
							cosmeticStand.UpdateColor();
							break;
						}
					}
					this.currentCart.RemoveAt(this.numFittingRoomButtons);
				}
			}
			pressedStand.UpdateColor();
			this.UpdateShoppingCart();
		}

		// Token: 0x06006A8D RID: 27277 RVA: 0x00223CF8 File Offset: 0x00221EF8
		public void PressWardrobeItemButton(CosmeticsController.CosmeticItem cosmeticItem, bool isLeftHand, bool isTempCosm)
		{
			if (cosmeticItem.isNullItem)
			{
				return;
			}
			CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(cosmeticItem.itemName);
			if (isTempCosm)
			{
				this.PressTemporaryWardrobeItemButton(itemFromDict, isLeftHand);
			}
			else
			{
				this.PressWardrobeItemButton(itemFromDict, isLeftHand);
			}
			this.UpdateWornCosmetics(true);
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated();
		}

		// Token: 0x06006A8E RID: 27278 RVA: 0x00223D48 File Offset: 0x00221F48
		private void PressWardrobeItemButton(CosmeticsController.CosmeticItem item, bool isLeftHand)
		{
			List<CosmeticsController.CosmeticSlots> list = CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Get();
			if (list.Capacity < 16)
			{
				list.Capacity = 16;
			}
			this.ApplyCosmeticItemToSet(this.currentWornSet, item, isLeftHand, true, list);
			foreach (CosmeticsController.CosmeticSlots cosmeticSlots in list)
			{
				this.tryOnSet.items[(int)cosmeticSlots] = this.nullItem;
			}
			CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Release(list);
			this.UpdateShoppingCart();
		}

		// Token: 0x06006A8F RID: 27279 RVA: 0x00223DDC File Offset: 0x00221FDC
		private void PressTemporaryWardrobeItemButton(CosmeticsController.CosmeticItem item, bool isLeftHand)
		{
			this.ApplyCosmeticItemToSet(this.tempUnlockedSet, item, isLeftHand, false);
		}

		// Token: 0x06006A90 RID: 27280 RVA: 0x00223DF0 File Offset: 0x00221FF0
		public void PressWardrobeFunctionButton(string function)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(function);
			if (num <= 2554875734U)
			{
				if (num <= 895779448U)
				{
					if (num != 292255708U)
					{
						if (num != 306900080U)
						{
							if (num == 895779448U)
							{
								if (function == "badge")
								{
									if (this.wardrobeType == 2)
									{
										return;
									}
									this.wardrobeType = 2;
								}
							}
						}
						else if (function == "left")
						{
							this.cosmeticsPages[this.wardrobeType] = this.cosmeticsPages[this.wardrobeType] - 1;
							if (this.cosmeticsPages[this.wardrobeType] < 0)
							{
								this.cosmeticsPages[this.wardrobeType] = (this.itemLists[this.wardrobeType].Count - 1) / 3;
							}
						}
					}
					else if (function == "face")
					{
						if (this.wardrobeType == 1)
						{
							return;
						}
						this.wardrobeType = 1;
					}
				}
				else if (num != 1538531746U)
				{
					if (num != 2028154341U)
					{
						if (num == 2554875734U)
						{
							if (function == "chest")
							{
								if (this.wardrobeType == 8)
								{
									return;
								}
								this.wardrobeType = 8;
							}
						}
					}
					else if (function == "right")
					{
						this.cosmeticsPages[this.wardrobeType] = this.cosmeticsPages[this.wardrobeType] + 1;
						if (this.cosmeticsPages[this.wardrobeType] > (this.itemLists[this.wardrobeType].Count - 1) / 3)
						{
							this.cosmeticsPages[this.wardrobeType] = 0;
						}
					}
				}
				else if (function == "back")
				{
					if (this.wardrobeType == 7)
					{
						return;
					}
					this.wardrobeType = 7;
				}
			}
			else if (num <= 3034286914U)
			{
				if (num != 2633735346U)
				{
					if (num != 2953262278U)
					{
						if (num == 3034286914U)
						{
							if (function == "fur")
							{
								if (this.wardrobeType == 4)
								{
									return;
								}
								this.wardrobeType = 4;
							}
						}
					}
					else if (function == "outfit")
					{
						if (this.wardrobeType == 5)
						{
							return;
						}
						this.wardrobeType = 5;
					}
				}
				else if (function == "arms")
				{
					if (this.wardrobeType == 6)
					{
						return;
					}
					this.wardrobeType = 6;
				}
			}
			else if (num <= 3300536096U)
			{
				if (num != 3081164502U)
				{
					if (num == 3300536096U)
					{
						if (function == "hand")
						{
							if (this.wardrobeType == 3)
							{
								return;
							}
							this.wardrobeType = 3;
						}
					}
				}
				else if (function == "tagEffect")
				{
					if (this.wardrobeType == 10)
					{
						return;
					}
					this.wardrobeType = 10;
				}
			}
			else if (num != 3568683773U)
			{
				if (num == 4072609730U)
				{
					if (function == "hat")
					{
						if (this.wardrobeType == 0)
						{
							return;
						}
						this.wardrobeType = 0;
					}
				}
			}
			else if (function == "reserved")
			{
				if (this.wardrobeType == 9)
				{
					return;
				}
				this.wardrobeType = 9;
			}
			this.UpdateWardrobeModelsAndButtons();
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated();
		}

		// Token: 0x06006A91 RID: 27281 RVA: 0x0022417C File Offset: 0x0022237C
		public void ClearCheckout(bool sendEvent)
		{
			if (sendEvent)
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_cancel, this.currentCart);
			}
			this.itemToBuy = this.nullItem;
			this.RefreshItemToBuyPreview();
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
			this.ProcessPurchaseItemState(null, false);
		}

		// Token: 0x06006A92 RID: 27282 RVA: 0x002241B8 File Offset: 0x002223B8
		public bool RemoveItemFromCart(CosmeticsController.CosmeticItem cosmeticItem)
		{
			this.searchIndex = this.currentCart.IndexOf(cosmeticItem);
			if (this.searchIndex != -1)
			{
				this.currentCart.RemoveAt(this.searchIndex);
				for (int i = 0; i < 16; i++)
				{
					if (cosmeticItem.itemName == this.tryOnSet.items[i].itemName)
					{
						this.tryOnSet.items[i] = this.nullItem;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06006A93 RID: 27283 RVA: 0x0022423B File Offset: 0x0022243B
		public void ClearCheckoutAndCart(bool sendEvent)
		{
			this.currentCart.Clear();
			this.tryOnSet.ClearSet(this.nullItem);
			CosmeticsController.ClearTryOnCollectable();
			this.ClearCheckout(sendEvent);
		}

		// Token: 0x06006A94 RID: 27284 RVA: 0x00224268 File Offset: 0x00222468
		public void PressCheckoutCartButton(CheckoutCartButton pressedCheckoutCartButton, bool isLeftHand)
		{
			if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Buying)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.tryOnSet.ClearSet(this.nullItem);
				CosmeticsController.ClearTryOnCollectable();
				if (this.itemToBuy.displayName == pressedCheckoutCartButton.currentCosmeticItem.displayName)
				{
					this.itemToBuy = this.nullItem;
					this.RefreshItemToBuyPreview();
				}
				else
				{
					this.itemToBuy = pressedCheckoutCartButton.currentCosmeticItem;
					this.checkoutCartButtonPressedWithLeft = isLeftHand;
					this.RefreshItemToBuyPreview();
				}
				this.ProcessPurchaseItemState(null, isLeftHand);
				this.UpdateShoppingCart();
			}
		}

		// Token: 0x06006A95 RID: 27285 RVA: 0x002242F4 File Offset: 0x002224F4
		private void RefreshItemToBuyPreview()
		{
			if (this.itemToBuy.bundledItems != null && this.itemToBuy.bundledItems.Length != 0)
			{
				List<string> list = new List<string>();
				foreach (string text in this.itemToBuy.bundledItems)
				{
					this.tempItem = this.GetItemFromDict(text);
					list.Add(this.tempItem.displayName);
				}
				this.iterator = 0;
				while (this.iterator < this.itemCheckouts.Count)
				{
					if (!this.itemCheckouts[this.iterator].IsNull())
					{
						this.itemCheckouts[this.iterator].checkoutHeadModel.SetCosmeticActiveArray(list.ToArray(), new bool[list.Count]);
					}
					this.iterator++;
				}
			}
			else
			{
				this.iterator = 0;
				while (this.iterator < this.itemCheckouts.Count)
				{
					if (!this.itemCheckouts[this.iterator].IsNull())
					{
						this.itemCheckouts[this.iterator].checkoutHeadModel.SetCosmeticActive(this.itemToBuy.displayName, false);
					}
					this.iterator++;
				}
			}
			this.ApplyCosmeticItemToSet(this.tryOnSet, this.itemToBuy, this.checkoutCartButtonPressedWithLeft, false);
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x06006A96 RID: 27286 RVA: 0x0022445D File Offset: 0x0022265D
		public void PressPurchaseItemButton(PurchaseItemButton pressedPurchaseItemButton, bool isLeftHand)
		{
			this.ProcessPurchaseItemState(pressedPurchaseItemButton.buttonSide, isLeftHand);
		}

		// Token: 0x06006A97 RID: 27287 RVA: 0x0022446C File Offset: 0x0022266C
		public async void PurchaseBundle(StoreBundle bundleToPurchase, ICreatorCodeProvider ccp)
		{
			if (bundleToPurchase.playfabBundleID != "NULL")
			{
				string code;
				NexusGroupId[] array;
				ccp.GetCreatorCode(out code, out array);
				ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Begin);
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				this.ProcessPurchaseItemState("left", false);
				this.buyingBundle = true;
				if (bundleToPurchase.nexusCreatorCode != null)
				{
					code = bundleToPurchase.nexusCreatorCode.Code;
					array = new NexusGroupId[] { bundleToPurchase.nexusCreatorCode.GroupId };
				}
				if (code.IsNullOrEmpty())
				{
					this.itemToPurchase = bundleToPurchase.playfabBundleID;
					this.SteamPurchase();
				}
				else
				{
					this.itemToPurchase = bundleToPurchase.playfabBundleID;
					NexusManager.MemberCode memberCode = await CreatorCodes.CheckValidationCoroutineJIT(ccp.TerminalId, code, array);
					if (memberCode != null)
					{
						if (this.buyingBundle)
						{
							this.SetValidatedCreatorCode(memberCode.memberCode, memberCode.groupId.Code, ccp.TerminalId);
							this.SteamPurchase();
						}
					}
					else
					{
						this.OnCreatorCodeFailure();
					}
				}
			}
		}

		// Token: 0x06006A98 RID: 27288 RVA: 0x002244B3 File Offset: 0x002226B3
		private void OnCreatorCodeFailure()
		{
			this.buyingBundle = false;
		}

		// Token: 0x06006A99 RID: 27289 RVA: 0x002244BC File Offset: 0x002226BC
		public void PressEarlyAccessButton()
		{
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
			this.ProcessPurchaseItemState("left", false);
			this.buyingBundle = true;
			this.itemToPurchase = this.BundlePlayfabItemName;
			ATM_Manager.instance.shinyRocksCost = (float)this.BundleShinyRocks;
			this.SteamPurchase();
		}

		// Token: 0x06006A9A RID: 27290 RVA: 0x00224508 File Offset: 0x00222708
		public void ProcessPurchaseItemState(string buttonSide, bool isLeftHand)
		{
			switch (this.currentPurchaseItemStage)
			{
			case CosmeticsController.PurchaseItemStages.Start:
				this.itemToBuy = this.nullItem;
				this.FormattedPurchaseText("SELECT AN ITEM FROM YOUR CART TO PURCHASE!", null, null, false, false);
				this.UpdateShoppingCart();
				return;
			case CosmeticsController.PurchaseItemStages.CheckoutButtonPressed:
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_start, this.currentCart);
				this.searchIndex = this.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => this.itemToBuy.itemName == x.itemName);
				if (this.searchIndex > -1)
				{
					this.FormattedPurchaseText("YOU ALREADY OWN THIS ITEM!", "-", "-", true, true);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemOwned;
					return;
				}
				if (this.itemToBuy.cost <= this.currencyBalance)
				{
					this.FormattedPurchaseText("DO YOU WANT TO BUY THIS ITEM?", "NO!", "YES!", false, false);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemSelected;
					return;
				}
				this.FormattedPurchaseText("INSUFFICIENT SHINY ROCKS FOR THIS ITEM!", "-", "-", true, true);
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				return;
			case CosmeticsController.PurchaseItemStages.ItemSelected:
				if (buttonSide == "right")
				{
					GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.item_select, this.itemToBuy);
					this.FormattedPurchaseText("ARE YOU REALLY SURE?", "YES! I NEED IT!", "LET ME THINK ABOUT IT", false, false);
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
					this.FormattedPurchaseText("PURCHASING ITEM...", "-", "-", true, true);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Buying;
					this.isLastHandTouchedLeft = isLeftHand;
					this.PurchaseItem();
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.ProcessPurchaseItemState(null, isLeftHand);
				return;
			case CosmeticsController.PurchaseItemStages.Success:
			{
				this.FormattedPurchaseText("SUCCESS! ENJOY YOUR NEW ITEM!", "-", "-", true, true);
				GorillaTagger.Instance.offlineVRRig.AddCosmetic(this.itemToBuy.itemName, 0);
				CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(this.itemToBuy.itemName);
				if (itemFromDict.bundledItems != null)
				{
					foreach (string text in itemFromDict.bundledItems)
					{
						GorillaTagger.Instance.offlineVRRig.AddCosmetic(text, 0);
					}
				}
				this.tryOnSet.ClearSet(this.nullItem);
				CosmeticsController.ClearTryOnCollectable();
				this.UpdateShoppingCart();
				this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true);
				this.UpdateShoppingCart();
				this.UpdateWornCosmetics();
				this.UpdateWardrobeModelsAndButtons();
				Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
				if (onCosmeticsUpdated == null)
				{
					return;
				}
				onCosmeticsUpdated();
				break;
			}
			case CosmeticsController.PurchaseItemStages.Failure:
				this.FormattedPurchaseText("ERROR IN PURCHASING ITEM! NO MONEY WAS SPENT. SELECT ANOTHER ITEM.", "-", "-", true, true);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006A9B RID: 27291 RVA: 0x0022478C File Offset: 0x0022298C
		public void FormattedPurchaseText(string finalLineVar, string leftPurchaseButtonText = null, string rightPurchaseButtonText = null, bool leftButtonOn = false, bool rightButtonOn = false)
		{
			this.finalLine = finalLineVar;
			if (leftPurchaseButtonText != null)
			{
				this.leftCheckoutPurchaseButtonString = leftPurchaseButtonText;
				this.leftCheckoutPurchaseButtonOn = leftButtonOn;
			}
			if (rightPurchaseButtonText != null)
			{
				this.rightCheckoutPurchaseButtonString = rightPurchaseButtonText;
				this.rightCheckoutPurchaseButtonOn = rightButtonOn;
			}
			string text = string.Concat(new string[]
			{
				"SELECTION: ",
				this.GetItemDisplayName(this.itemToBuy),
				"\nITEM COST: ",
				this.itemToBuy.cost.ToString(),
				"\nYOU HAVE: ",
				this.currencyBalance.ToString(),
				"\n\n",
				this.finalLine
			});
			this.iterator = 0;
			while (this.iterator < this.itemCheckouts.Count)
			{
				if (!this.itemCheckouts[this.iterator].IsNull())
				{
					this.itemCheckouts[this.iterator].UpdatePurchaseText(text, leftPurchaseButtonText, rightPurchaseButtonText, leftButtonOn, rightButtonOn);
				}
				this.iterator++;
			}
		}

		// Token: 0x06006A9C RID: 27292 RVA: 0x00224888 File Offset: 0x00222A88
		public void PurchaseItem()
		{
			PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
			{
				ItemId = this.itemToBuy.itemName,
				Price = this.itemToBuy.cost,
				VirtualCurrency = this.currencyName,
				CatalogVersion = this.catalog
			}, delegate(PurchaseItemResult result)
			{
				if (result.Items.Count > 0)
				{
					foreach (ItemInstance itemInstance in result.Items)
					{
						CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(this.itemToBuy.itemName);
						if (itemFromDict.itemCategory == CosmeticsController.CosmeticCategory.Set)
						{
							this.UnlockItem(itemInstance.ItemId, false);
							foreach (string text in itemFromDict.bundledItems)
							{
								this.UnlockItem(text, false);
							}
						}
						else
						{
							this.UnlockItem(itemInstance.ItemId, false);
						}
					}
					this.UpdateMyCosmetics();
					if (NetworkSystem.Instance.InRoom)
					{
						base.StartCoroutine(this.CheckIfMyCosmeticsUpdated(this.itemToBuy.itemName));
					}
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Success;
					this.currencyBalance -= this.itemToBuy.cost;
					this.UpdateShoppingCart();
					this.ProcessPurchaseItemState(null, this.isLastHandTouchedLeft);
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
				this.ProcessPurchaseItemState(null, false);
			}, delegate(PlayFabError error)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
				this.ProcessPurchaseItemState(null, false);
			}, null, null);
		}

		// Token: 0x06006A9D RID: 27293 RVA: 0x002248F4 File Offset: 0x00222AF4
		private void UnlockItem(string itemIdToUnlock, bool relock = false)
		{
			int num = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => itemIdToUnlock == x.itemName);
			if (num > -1)
			{
				this.ModifyUnlockList(this.unlockedCosmetics, num, relock);
				if (relock)
				{
					this.concatStringCosmeticsAllowed.Replace(this.allCosmetics[num].itemName, string.Empty);
				}
				else
				{
					this.concatStringCosmeticsAllowed += this.allCosmetics[num].itemName;
				}
				switch (this.allCosmetics[num].itemCategory)
				{
				case CosmeticsController.CosmeticCategory.Hat:
					this.ModifyUnlockList(this.unlockedHats, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Badge:
					this.ModifyUnlockList(this.unlockedBadges, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Face:
					this.ModifyUnlockList(this.unlockedFaces, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Paw:
					if (!this.allCosmetics[num].isThrowable)
					{
						this.ModifyUnlockList(this.unlockedPaws, num, relock);
						return;
					}
					this.ModifyUnlockList(this.unlockedThrowables, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Chest:
					this.ModifyUnlockList(this.unlockedChests, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Fur:
					this.ModifyUnlockList(this.unlockedFurs, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Shirt:
					this.ModifyUnlockList(this.unlockedShirts, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Back:
					this.ModifyUnlockList(this.unlockedBacks, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Arms:
					this.ModifyUnlockList(this.unlockedArms, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Pants:
					this.ModifyUnlockList(this.unlockedPants, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.TagEffect:
					this.ModifyUnlockList(this.unlockedTagFX, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Count:
				case CosmeticsController.CosmeticCategory.Collectable:
					break;
				case CosmeticsController.CosmeticCategory.Set:
					foreach (string text in this.allCosmetics[num].bundledItems)
					{
						this.UnlockItem(text, false);
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06006A9E RID: 27294 RVA: 0x00224AD4 File Offset: 0x00222CD4
		private void ModifyUnlockList(List<CosmeticsController.CosmeticItem> list, int index, bool relock)
		{
			if (!relock && !list.Contains(this.allCosmetics[index]))
			{
				list.Add(this.allCosmetics[index]);
				return;
			}
			if (relock && list.Contains(this.allCosmetics[index]))
			{
				list.Remove(this.allCosmetics[index]);
			}
		}

		// Token: 0x06006A9F RID: 27295 RVA: 0x00224B35 File Offset: 0x00222D35
		private IEnumerator CheckIfMyCosmeticsUpdated(string itemToBuyID)
		{
			Debug.Log("Cosmetic updated check!");
			yield return new WaitForSecondsRealtime(1f);
			this.foundCosmetic = false;
			this.attempts = 0;
			while (!this.foundCosmetic && this.attempts < 10 && NetworkSystem.Instance.InRoom)
			{
				PlayFabClientAPI.GetSharedGroupData(new global::PlayFab.ClientModels.GetSharedGroupDataRequest
				{
					Keys = new List<string> { "Inventory" },
					SharedGroupId = NetworkSystem.Instance.LocalPlayer.UserId + "Inventory"
				}, delegate(GetSharedGroupDataResult result)
				{
					this.attempts++;
					foreach (KeyValuePair<string, global::PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
					{
						if (keyValuePair.Value.Value.Contains(itemToBuyID))
						{
							PhotonNetwork.RaiseEvent(199, null, new RaiseEventOptions
							{
								Receivers = ReceiverGroup.Others
							}, SendOptions.SendReliable);
							this.foundCosmetic = true;
						}
					}
					if (this.foundCosmetic)
					{
						this.UpdateWornCosmetics(true);
					}
				}, delegate(PlayFabError error)
				{
					this.attempts++;
					this.ReauthOrBan(error);
				}, null, null);
				yield return new WaitForSecondsRealtime(1f);
			}
			Debug.Log("done!");
			yield break;
		}

		// Token: 0x06006AA0 RID: 27296 RVA: 0x00224B4C File Offset: 0x00222D4C
		public void UpdateWardrobeModelsAndButtons()
		{
			foreach (WardrobeInstance wardrobeInstance in this.wardrobes)
			{
				wardrobeInstance.wardrobeItemButtons[0].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3] : this.nullItem);
				wardrobeInstance.wardrobeItemButtons[1].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 + 1 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 1] : this.nullItem);
				wardrobeInstance.wardrobeItemButtons[2].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 + 2 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 2] : this.nullItem);
				this.iterator = 0;
				while (this.iterator < wardrobeInstance.wardrobeItemButtons.Length)
				{
					CosmeticsController.CosmeticItem currentCosmeticItem = wardrobeInstance.wardrobeItemButtons[this.iterator].currentCosmeticItem;
					wardrobeInstance.wardrobeItemButtons[this.iterator].isOn = !currentCosmeticItem.isNullItem && this.AnyMatch(this.currentWornSet, currentCosmeticItem);
					wardrobeInstance.wardrobeItemButtons[this.iterator].UpdateColor();
					this.iterator++;
				}
				wardrobeInstance.wardrobeItemButtons[0].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[0].currentCosmeticItem.displayName, false);
				wardrobeInstance.wardrobeItemButtons[1].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[1].currentCosmeticItem.displayName, false);
				wardrobeInstance.wardrobeItemButtons[2].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[2].currentCosmeticItem.displayName, false);
				wardrobeInstance.selfDoll.SetCosmeticActiveArray(this.currentWornSet.ToDisplayNameArray(), this.currentWornSet.ToOnRightSideArray());
			}
		}

		// Token: 0x06006AA1 RID: 27297 RVA: 0x00224DC4 File Offset: 0x00222FC4
		public int GetCategorySize(CosmeticsController.CosmeticCategory category)
		{
			int indexForCategory = this.GetIndexForCategory(category);
			if (indexForCategory != -1)
			{
				return this.itemLists[indexForCategory].Count;
			}
			return 0;
		}

		// Token: 0x06006AA2 RID: 27298 RVA: 0x00224DEC File Offset: 0x00222FEC
		public CosmeticsController.CosmeticItem GetCosmetic(int category, int cosmeticIndex)
		{
			if (cosmeticIndex >= this.itemLists[category].Count || cosmeticIndex < 0)
			{
				return this.nullItem;
			}
			return this.itemLists[category][cosmeticIndex];
		}

		// Token: 0x06006AA3 RID: 27299 RVA: 0x00224E17 File Offset: 0x00223017
		public CosmeticsController.CosmeticItem GetCosmetic(CosmeticsController.CosmeticCategory category, int cosmeticIndex)
		{
			return this.GetCosmetic(this.GetIndexForCategory(category), cosmeticIndex);
		}

		// Token: 0x06006AA4 RID: 27300 RVA: 0x00224E28 File Offset: 0x00223028
		private int GetIndexForCategory(CosmeticsController.CosmeticCategory category)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return 0;
			case CosmeticsController.CosmeticCategory.Badge:
				return 2;
			case CosmeticsController.CosmeticCategory.Face:
				return 1;
			case CosmeticsController.CosmeticCategory.Paw:
				return 3;
			case CosmeticsController.CosmeticCategory.Chest:
				return 9;
			case CosmeticsController.CosmeticCategory.Fur:
				return 4;
			case CosmeticsController.CosmeticCategory.Shirt:
				return 5;
			case CosmeticsController.CosmeticCategory.Back:
				return 8;
			case CosmeticsController.CosmeticCategory.Arms:
				return 7;
			case CosmeticsController.CosmeticCategory.Pants:
				return 6;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return 10;
			default:
				return 0;
			}
		}

		// Token: 0x06006AA5 RID: 27301 RVA: 0x00224E84 File Offset: 0x00223084
		public bool IsCosmeticEquipped(CosmeticsController.CosmeticItem cosmetic)
		{
			return this.AnyMatch(this.currentWornSet, cosmetic);
		}

		// Token: 0x06006AA6 RID: 27302 RVA: 0x00224E93 File Offset: 0x00223093
		public bool IsCosmeticEquipped(CosmeticsController.CosmeticItem cosmetic, bool tempSet)
		{
			if (!tempSet)
			{
				return this.IsCosmeticEquipped(cosmetic);
			}
			return this.IsTemporaryCosmeticEquipped(cosmetic);
		}

		// Token: 0x06006AA7 RID: 27303 RVA: 0x00224EA7 File Offset: 0x002230A7
		public bool IsTemporaryCosmeticEquipped(CosmeticsController.CosmeticItem cosmetic)
		{
			return this.AnyMatch(this.tempUnlockedSet, cosmetic);
		}

		// Token: 0x06006AA8 RID: 27304 RVA: 0x00224EB8 File Offset: 0x002230B8
		public CosmeticsController.CosmeticItem GetSlotItem(CosmeticsController.CosmeticSlots slot, bool checkOpposite = true, bool tempSet = false)
		{
			int num = (int)slot;
			if (checkOpposite)
			{
				num = (int)CosmeticsController.CosmeticSet.OppositeSlot(slot);
			}
			if (!tempSet)
			{
				return this.currentWornSet.items[num];
			}
			return this.tempUnlockedSet.items[num];
		}

		// Token: 0x06006AA9 RID: 27305 RVA: 0x00224EF7 File Offset: 0x002230F7
		public string[] GetCurrentlyWornCosmetics(bool tempSet = false)
		{
			if (!tempSet)
			{
				return this.currentWornSet.ToDisplayNameArray();
			}
			return this.tempUnlockedSet.ToDisplayNameArray();
		}

		// Token: 0x06006AAA RID: 27306 RVA: 0x00224F13 File Offset: 0x00223113
		public bool[] GetCurrentRightEquippedSided(bool tempSet = false)
		{
			if (!tempSet)
			{
				return this.currentWornSet.ToOnRightSideArray();
			}
			return this.tempUnlockedSet.ToOnRightSideArray();
		}

		// Token: 0x06006AAB RID: 27307 RVA: 0x00224F30 File Offset: 0x00223130
		public void UpdateShoppingCart()
		{
			this.iterator = 0;
			while (this.iterator < this.itemCheckouts.Count)
			{
				if (!this.itemCheckouts[this.iterator].IsNull())
				{
					this.itemCheckouts[this.iterator].UpdateFromCart(this.currentCart, this.itemToBuy);
				}
				this.iterator++;
			}
			this.iterator = 0;
			while (this.iterator < this.fittingRooms.Count)
			{
				if (!this.fittingRooms[this.iterator].IsNull())
				{
					this.fittingRooms[this.iterator].UpdateFromCart(this.currentCart, this.tryOnSet);
				}
				this.iterator++;
			}
			this.UpdateWardrobeModelsAndButtons();
		}

		// Token: 0x06006AAC RID: 27308 RVA: 0x0022500B File Offset: 0x0022320B
		public void UpdateWornCosmetics()
		{
			this.UpdateWornCosmetics(false, false);
		}

		// Token: 0x06006AAD RID: 27309 RVA: 0x00225015 File Offset: 0x00223215
		public void UpdateWornCosmetics(bool sync)
		{
			this.UpdateWornCosmetics(sync, false);
		}

		// Token: 0x06006AAE RID: 27310 RVA: 0x00225020 File Offset: 0x00223220
		public void UpdateWornCosmetics(bool sync, bool playfx)
		{
			VRRig localRig = VRRig.LocalRig;
			this.activeMergedSet.MergeInSets(this.currentWornSet, this.tempUnlockedSet, (string id) => PlayerCosmeticsSystem.LocalPlayerInTemporaryCosmeticSpace() || PlayerCosmeticsSystem.IsTemporaryCosmeticAllowed(localRig, id));
			GorillaTagger.Instance.offlineVRRig.LocalUpdateCosmeticsWithTryon(this.activeMergedSet, this.tryOnSet, playfx);
			if (sync && GorillaTagger.Instance.myVRRig != null)
			{
				if (this.isHidingCosmeticsFromRemotePlayers)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_HideAllCosmetics", RpcTarget.All, Array.Empty<object>());
					return;
				}
				int[] array = this.activeMergedSet.ToPackedIDArray();
				int[] array2 = this.tryOnSet.ToPackedIDArray();
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, new object[] { array, array2, playfx });
				CosmeticCollectionDisplay.GetDisplaysForRig(GorillaTagger.Instance.offlineVRRig, CosmeticsController.scratchDisplayList);
				if (CosmeticsController.scratchDisplayList.Count > 0)
				{
					int num = CosmeticsController.scratchDisplayList.Count * 3;
					if (CosmeticsController.cycleStatesArray.Length != num)
					{
						CosmeticsController.cycleStatesArray = new int[num];
					}
					for (int i = 0; i < CosmeticsController.scratchDisplayList.Count; i++)
					{
						CosmeticCollectionDisplay cosmeticCollectionDisplay = CosmeticsController.scratchDisplayList[i];
						string parentPlayFabID = cosmeticCollectionDisplay.ParentPlayFabID;
						CosmeticsController.cycleStatesArray[i * 3] = (int)(parentPlayFabID[0] - 'A' + '\u001a' * (parentPlayFabID[1] - 'A' + '\u001a' * (parentPlayFabID[2] - 'A' + '\u001a' * (parentPlayFabID[3] - 'A' + '\u001a' * (parentPlayFabID[4] - 'A')))));
						CosmeticsController.CosmeticItem? activeCollectable = cosmeticCollectionDisplay.ActiveCollectable;
						CosmeticsController.cycleStatesArray[i * 3 + 1] = ((activeCollectable != null) ? this.GetCanonicalCollectableIndex(parentPlayFabID, activeCollectable.Value.itemName) : cosmeticCollectionDisplay.ActiveIndex);
						CosmeticsController.cycleStatesArray[i * 3 + 2] = cosmeticCollectionDisplay.VisibleMask;
					}
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithCollectablesPacked", RpcTarget.Others, new object[] { CosmeticsController.cycleStatesArray });
				}
			}
		}

		// Token: 0x06006AAF RID: 27311 RVA: 0x0022523D File Offset: 0x0022343D
		public CosmeticsController.CosmeticItem GetItemFromDict(string itemID)
		{
			if (!this.allCosmeticsDict.TryGetValue(itemID, out this.cosmeticItemVar))
			{
				return this.nullItem;
			}
			return this.cosmeticItemVar;
		}

		// Token: 0x06006AB0 RID: 27312 RVA: 0x00225260 File Offset: 0x00223460
		public string GetItemNameFromDisplayName(string displayName)
		{
			if (displayName == "" || displayName == null)
			{
				return "null";
			}
			if (!this.allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(displayName, out this.returnString))
			{
				return "null";
			}
			return this.returnString;
		}

		// Token: 0x06006AB1 RID: 27313 RVA: 0x00225298 File Offset: 0x00223498
		public CosmeticSO GetCosmeticSOFromDisplayName(string displayName)
		{
			string itemNameFromDisplayName = this.GetItemNameFromDisplayName(displayName);
			if (itemNameFromDisplayName.Equals("null"))
			{
				return null;
			}
			AllCosmeticsArraySO allCosmeticsArraySO = this.v2_allCosmeticsInfoAssetRef.Asset as AllCosmeticsArraySO;
			if (allCosmeticsArraySO == null)
			{
				GTDev.LogWarning<string>("null AllCosmeticsArraySO", null);
				return null;
			}
			CosmeticSO cosmeticSO = allCosmeticsArraySO.SearchForCosmeticSO(itemNameFromDisplayName);
			if (cosmeticSO != null)
			{
				return cosmeticSO;
			}
			GTDev.Log<string>("Could not find cosmetic info for " + itemNameFromDisplayName, null);
			return null;
		}

		// Token: 0x06006AB2 RID: 27314 RVA: 0x00225308 File Offset: 0x00223508
		public CosmeticAnchorAntiIntersectOffsets GetClipOffsetsFromDisplayName(string displayName)
		{
			string itemNameFromDisplayName = this.GetItemNameFromDisplayName(displayName);
			if (itemNameFromDisplayName.Equals("null"))
			{
				return this.defaultClipOffsets;
			}
			AllCosmeticsArraySO allCosmeticsArraySO = this.v2_allCosmeticsInfoAssetRef.Asset as AllCosmeticsArraySO;
			if (allCosmeticsArraySO == null)
			{
				GTDev.LogWarning<string>("null AllCosmeticsArraySO", null);
				return this.defaultClipOffsets;
			}
			CosmeticSO cosmeticSO = allCosmeticsArraySO.SearchForCosmeticSO(itemNameFromDisplayName);
			if (cosmeticSO != null)
			{
				return cosmeticSO.info.anchorAntiIntersectOffsets;
			}
			GTDev.Log<string>("Could not find cosmetic info for " + itemNameFromDisplayName, null);
			return this.defaultClipOffsets;
		}

		// Token: 0x06006AB3 RID: 27315 RVA: 0x00225394 File Offset: 0x00223594
		public bool AnyMatch(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem item)
		{
			if (item.itemCategory != CosmeticsController.CosmeticCategory.Set)
			{
				return set.IsActive(item.displayName);
			}
			if (item.itemCategory == CosmeticsController.CosmeticCategory.Set && item.bundledItems != null)
			{
				if (item.bundledItems.Length == 1)
				{
					return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0]));
				}
				if (item.bundledItems.Length == 2)
				{
					return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[1]));
				}
				if (item.bundledItems.Length >= 3)
				{
					return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[1])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[2]));
				}
			}
			return false;
		}

		// Token: 0x06006AB4 RID: 27316 RVA: 0x00225480 File Offset: 0x00223680
		public void Initialize()
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			if (!this.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				this.V2_allCosmeticsInfoAssetRef_OnPostLoad = (Action)Delegate.Remove(this.V2_allCosmeticsInfoAssetRef_OnPostLoad, new Action(this.Initialize));
				this.V2_allCosmeticsInfoAssetRef_OnPostLoad = (Action)Delegate.Combine(this.V2_allCosmeticsInfoAssetRef_OnPostLoad, new Action(this.Initialize));
				return;
			}
			if (SubscriptionManager.LocalSubscriptionDataResolved)
			{
				this.GetCosmeticsPlayFabCatalogData();
				return;
			}
			SubscriptionManager.OnLocalSubscriptionDataResolved = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionDataResolved, new Action(this.Initialize));
			SubscriptionManager.OnLocalSubscriptionDataResolved = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionDataResolved, new Action(this.Initialize));
		}

		// Token: 0x06006AB5 RID: 27317 RVA: 0x00225536 File Offset: 0x00223736
		public void GetLastDailyLogin()
		{
			PlayFabClientAPI.GetUserReadOnlyData(new global::PlayFab.ClientModels.GetUserDataRequest(), delegate(GetUserDataResult result)
			{
				if (result.Data.TryGetValue("DailyLogin", out this.userDataRecord))
				{
					this.lastDailyLogin = this.userDataRecord.Value;
					return;
				}
				this.lastDailyLogin = "NONE";
				base.StartCoroutine(this.GetMyDaily());
			}, delegate(PlayFabError error)
			{
				Debug.Log("Got error getting read-only user data:");
				Debug.Log(error.GenerateErrorReport());
				this.lastDailyLogin = "FAILED";
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			}, null, null);
		}

		// Token: 0x06006AB6 RID: 27318 RVA: 0x0022555C File Offset: 0x0022375C
		private IEnumerator CheckCanGetDaily()
		{
			while (!KIDManager.InitialisationComplete)
			{
				yield return new WaitForSecondsRealtime(1f);
			}
			while (!PlayFabClientAPI.IsClientLoggedIn())
			{
				yield return new WaitForSecondsRealtime(1f);
			}
			for (;;)
			{
				if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
				{
					this.currentTime = new DateTime((GorillaComputer.instance.startupMillis + (long)(Time.realtimeSinceStartup * 1000f)) * 10000L);
					this.secondsUntilTomorrow = (int)(this.currentTime.AddDays(1.0).Date - this.currentTime).TotalSeconds;
					if (string.IsNullOrEmpty(this.lastDailyLogin))
					{
						this.GetLastDailyLogin();
					}
					else
					{
						string text = this.currentTime.ToString("o").Substring(0, 10);
						if (text == this.lastDailyLogin)
						{
							this.checkedDaily = true;
							this.gotMyDaily = true;
						}
						else if (text != this.lastDailyLogin)
						{
							this.checkedDaily = true;
							this.gotMyDaily = false;
							base.StartCoroutine(this.GetMyDaily());
						}
						else if (this.lastDailyLogin == "FAILED")
						{
							this.GetLastDailyLogin();
						}
					}
					this.secondsToWaitToCheckDaily = (this.checkedDaily ? 60f : 10f);
					this.UpdateCurrencyBoards();
					yield return new WaitForSecondsRealtime(this.secondsToWaitToCheckDaily);
				}
				else
				{
					yield return new WaitForSecondsRealtime(1f);
				}
			}
			yield break;
		}

		// Token: 0x06006AB7 RID: 27319 RVA: 0x0022556B File Offset: 0x0022376B
		private IEnumerator GetMyDaily()
		{
			yield return new WaitForSecondsRealtime(10f);
			GorillaServer.Instance.TryDistributeCurrency(delegate(ExecuteFunctionResult result)
			{
				this.GetCurrencyBalance();
				this.GetLastDailyLogin();
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			});
			yield break;
		}

		// Token: 0x06006AB8 RID: 27320 RVA: 0x0022557C File Offset: 0x0022377C
		public void GetCosmeticsPlayFabCatalogData()
		{
			if (!this.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				throw new Exception("Method `GetCosmeticsPlayFabCatalogData` was called before `v2_allCosmeticsInfoAssetRef` was loaded. Listen to callback `V2_allCosmeticsInfoAssetRef_OnPostLoad` or check `v2_allCosmeticsInfoAssetRef_isLoaded` before trying to get PlayFab catalog data.");
			}
			if (!SubscriptionManager.LocalSubscriptionDataResolved)
			{
				throw new Exception("Method `GetCosmeticsPlayFabCatalogData` was called before local subscription data was resolved. Listen to callback `OnLocalSubscriptionDataResolved` or check `LocalSubscriptionDataResolved` before trying to get PlayFab catalog data.");
			}
			if (this.catalogRequestInFlight)
			{
				this.catalogRequestRerunQueued = true;
				return;
			}
			this.catalogRequestInFlight = true;
			this.tryGetCatalogTwice = false;
			this.GetCosmeticsPlayFabCatalogDataInternal();
		}

		// Token: 0x06006AB9 RID: 27321 RVA: 0x002255D4 File Offset: 0x002237D4
		private void ReconcileBundleRewardsIfNeeded(List<ItemInstance> inventory)
		{
			if (inventory != null && this.bundleList.IsLoaded)
			{
				bool flag = false;
				foreach (ItemInstance itemInstance in inventory)
				{
					if (this.bundleList.HasMothershipRewards(itemInstance.ItemId) && (itemInstance.CustomData == null || !itemInstance.CustomData.ContainsKey("mshipRewardsGranted")))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return;
				}
			}
			if (GorillaServer.Instance == null)
			{
				Debug.LogWarning("ReconcileBundleRewards skipped: GorillaServer.Instance is null");
				return;
			}
			GorillaServer.Instance.ReconcileBundleRewards(delegate(string result)
			{
				Debug.Log("ReconcileBundleRewards success: " + result);
			}, delegate(string error)
			{
				Debug.LogWarning("ReconcileBundleRewards failed: " + error);
			});
		}

		// Token: 0x06006ABA RID: 27322 RVA: 0x002256C8 File Offset: 0x002238C8
		private void GetCosmeticsPlayFabCatalogDataInternal()
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				this.ReconcileBundleRewardsIfNeeded(result.Inventory);
				PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest
				{
					CatalogVersion = this.catalog
				}, delegate(GetCatalogItemsResult result2)
				{
					try
					{
						this.unlockedCosmetics.Clear();
						this.unlockedHats.Clear();
						this.unlockedBadges.Clear();
						this.unlockedFaces.Clear();
						this.unlockedPaws.Clear();
						this.unlockedFurs.Clear();
						this.unlockedShirts.Clear();
						this.unlockedPants.Clear();
						this.unlockedArms.Clear();
						this.unlockedBacks.Clear();
						this.unlockedChests.Clear();
						this.unlockedTagFX.Clear();
						this.unlockedThrowables.Clear();
						this.catalogItems = result2.Catalog;
						using (List<CatalogItem>.Enumerator enumerator = this.catalogItems.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								CatalogItem catalogItem = enumerator.Current;
								if (!BuilderSetManager.IsItemIDBuilderItem(catalogItem.ItemId))
								{
									this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => catalogItem.ItemId == x.itemName);
									if (this.searchIndex > -1)
									{
										this.tempStringArray = null;
										this.hasPrice = false;
										if (catalogItem.Bundle != null)
										{
											this.tempStringArray = catalogItem.Bundle.BundledItems.ToArray();
										}
										uint num;
										if (catalogItem.VirtualCurrencyPrices.TryGetValue(this.currencyName, out num))
										{
											this.hasPrice = true;
										}
										CosmeticsController.CosmeticItem cosmeticItem = this.allCosmetics[this.searchIndex];
										cosmeticItem.itemName = catalogItem.ItemId;
										cosmeticItem.displayName = catalogItem.DisplayName;
										cosmeticItem.cost = (int)num;
										cosmeticItem.bundledItems = this.tempStringArray;
										cosmeticItem.canTryOn = this.hasPrice;
										if (cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Paw)
										{
											CosmeticInfoV2 cosmeticInfoV = this.v2_allCosmetics[this.searchIndex];
											cosmeticItem.isThrowable = cosmeticInfoV.isThrowable && !cosmeticInfoV.hasWardrobeParts;
										}
										if (cosmeticItem.displayName == null)
										{
											string text = "null";
											if (this.allCosmetics[this.searchIndex].itemPicture)
											{
												text = this.allCosmetics[this.searchIndex].itemPicture.name;
											}
											string debugCosmeticSOName = this.v2_allCosmetics[this.searchIndex].debugCosmeticSOName;
											Debug.LogError(string.Concat(new string[]
											{
												string.Format("Cosmetic encountered with a null displayName at index {0}! ", this.searchIndex),
												"Setting displayName to id: \"",
												this.allCosmetics[this.searchIndex].itemName,
												"\". iconName=\"",
												text,
												"\".cosmeticSOName=\"",
												debugCosmeticSOName,
												"\". "
											}));
											cosmeticItem.displayName = cosmeticItem.itemName;
										}
										this.V2_ConformCosmeticItemV1DisplayName(ref cosmeticItem);
										this._allCosmetics[this.searchIndex] = cosmeticItem;
										this._allCosmeticsDict[cosmeticItem.itemName] = cosmeticItem;
										this._allCosmeticsItemIDsfromDisplayNamesDict[cosmeticItem.displayName] = cosmeticItem.itemName;
										this._allCosmeticsItemIDsfromDisplayNamesDict[cosmeticItem.overrideDisplayName] = cosmeticItem.itemName;
									}
								}
							}
						}
						for (int i = this._allCosmetics.Count - 1; i > -1; i--)
						{
							this.tempItem = this._allCosmetics[i];
							if (this.tempItem.itemCategory == CosmeticsController.CosmeticCategory.Set && this.tempItem.canTryOn)
							{
								string[] array = this.tempItem.bundledItems;
								for (int j = 0; j < array.Length; j++)
								{
									string setItemName2 = array[j];
									this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => setItemName2 == x.itemName);
									if (this.searchIndex > -1)
									{
										this.tempItem = this._allCosmetics[this.searchIndex];
										this.tempItem.canTryOn = true;
										this._allCosmetics[this.searchIndex] = this.tempItem;
										this._allCosmeticsDict[this._allCosmetics[this.searchIndex].itemName] = this.tempItem;
										this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this.tempItem.itemName;
									}
								}
							}
						}
						foreach (KeyValuePair<string, StoreBundle> keyValuePair in BundleManager.instance.storeBundlesById)
						{
							string text2;
							StoreBundle storeBundle;
							keyValuePair.Deconstruct(out text2, out storeBundle);
							string text3 = text2;
							StoreBundle bundleData = storeBundle;
							int num2 = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => bundleData.playfabBundleID == x.itemName);
							if (num2 > 0 && this._allCosmetics[num2].bundledItems != null)
							{
								string[] array = this._allCosmetics[num2].bundledItems;
								for (int j = 0; j < array.Length; j++)
								{
									string setItemName = array[j];
									this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => setItemName == x.itemName);
									if (this.searchIndex > -1)
									{
										this.tempItem = this._allCosmetics[this.searchIndex];
										this.tempItem.canTryOn = true;
										this._allCosmetics[this.searchIndex] = this.tempItem;
										this._allCosmeticsDict[this._allCosmetics[this.searchIndex].itemName] = this.tempItem;
										this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this.tempItem.itemName;
									}
								}
							}
							if (!bundleData.HasPrice)
							{
								num2 = this.catalogItems.FindIndex((CatalogItem ci) => ci.Bundle != null && ci.ItemId == bundleData.playfabBundleID);
								if (num2 > 0)
								{
									uint num3;
									if (this.catalogItems[num2].VirtualCurrencyPrices.TryGetValue("RM", out num3))
									{
										BundleManager.instance.storeBundlesById[text3].TryUpdatePrice(num3);
									}
									else
									{
										BundleManager.instance.storeBundlesById[text3].TryUpdatePrice(null);
									}
								}
							}
						}
						this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => "Slingshot" == x.itemName);
						if (this.searchIndex < 0)
						{
							throw new MissingReferenceException("CosmeticsController: Cannot find default slingshot! it is required for players that do not have another slingshot equipped and are playing Paintbrawl.");
						}
						this._allCosmeticsDict["Slingshot"] = this._allCosmetics[this.searchIndex];
						this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this._allCosmetics[this.searchIndex].itemName;
						this.allCosmeticsDict_isInitialized = true;
						this.allCosmeticsItemIDsfromDisplayNamesDict_isInitialized = true;
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						using (List<ItemInstance>.Enumerator enumerator3 = result.Inventory.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								ItemInstance item = enumerator3.Current;
								if (!BuilderSetManager.IsItemIDBuilderItem(item.ItemId))
								{
									if (item.ItemId == this.m_earlyAccessSupporterPackCosmeticSO.info.playFabID)
									{
										foreach (CosmeticSO cosmeticSO in this.m_earlyAccessSupporterPackCosmeticSO.info.setCosmetics)
										{
											CosmeticsController.CosmeticItem cosmeticItem2;
											if (this.allCosmeticsDict.TryGetValue(cosmeticSO.info.playFabID, out cosmeticItem2))
											{
												this.unlockedCosmetics.Add(cosmeticItem2);
											}
										}
									}
									BundleManager.instance.MarkBundleOwnedByPlayFabID(item.ItemId);
									if (!dictionary.ContainsKey(item.ItemId))
									{
										this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.ItemId == x.itemName);
										if (this.searchIndex > -1)
										{
											dictionary[item.ItemId] = item.ItemId;
											this.unlockedCosmetics.Add(this.allCosmetics[this.searchIndex]);
										}
									}
								}
							}
						}
						foreach (CosmeticsController.CosmeticItem cosmeticItem3 in this.unlockedCosmetics)
						{
							if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Hat && !this.unlockedHats.Contains(cosmeticItem3))
							{
								this.unlockedHats.Add(cosmeticItem3);
							}
							else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Face && !this.unlockedFaces.Contains(cosmeticItem3))
							{
								this.unlockedFaces.Add(cosmeticItem3);
							}
							else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Badge && !this.unlockedBadges.Contains(cosmeticItem3))
							{
								this.unlockedBadges.Add(cosmeticItem3);
							}
							else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Paw)
							{
								if (!cosmeticItem3.isThrowable && !this.unlockedPaws.Contains(cosmeticItem3))
								{
									this.unlockedPaws.Add(cosmeticItem3);
								}
								else if (cosmeticItem3.isThrowable && !this.unlockedThrowables.Contains(cosmeticItem3))
								{
									this.unlockedThrowables.Add(cosmeticItem3);
								}
							}
							else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Fur && !this.unlockedFurs.Contains(cosmeticItem3))
							{
								this.unlockedFurs.Add(cosmeticItem3);
							}
							else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Shirt && !this.unlockedShirts.Contains(cosmeticItem3))
							{
								this.unlockedShirts.Add(cosmeticItem3);
							}
							else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Arms && !this.unlockedArms.Contains(cosmeticItem3))
							{
								this.unlockedArms.Add(cosmeticItem3);
							}
							else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Back && !this.unlockedBacks.Contains(cosmeticItem3))
							{
								this.unlockedBacks.Add(cosmeticItem3);
							}
							else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Chest && !this.unlockedChests.Contains(cosmeticItem3))
							{
								this.unlockedChests.Add(cosmeticItem3);
							}
							else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Pants && !this.unlockedPants.Contains(cosmeticItem3))
							{
								this.unlockedPants.Add(cosmeticItem3);
							}
							else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.TagEffect && !this.unlockedTagFX.Contains(cosmeticItem3))
							{
								this.unlockedTagFX.Add(cosmeticItem3);
							}
							this.concatStringCosmeticsAllowed += cosmeticItem3.itemName;
						}
						BuilderSetManager.instance.OnGotInventoryItems(result, result2);
						this.currencyBalance = result.VirtualCurrency[this.currencyName];
						int num4;
						this.playedInBeta = result.VirtualCurrency.TryGetValue("TC", out num4) && num4 > 0;
						Action onGetCurrency = this.OnGetCurrency;
						if (onGetCurrency != null)
						{
							onGetCurrency();
						}
						BundleManager.instance.CheckIfBundlesOwned();
						StoreUpdater.instance.Initialize();
						this.currentWornSet.LoadFromPlayerPreferences(this);
						this.LoadSavedOutfits();
						if (!ATM_Manager.instance.alreadyBegan)
						{
							ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Begin);
							ATM_Manager.instance.alreadyBegan = true;
						}
						this.ProcessPurchaseItemState(null, false);
						this.UpdateShoppingCart();
						this.UpdateCurrencyBoards();
						this.ConfirmIndividualCosmeticsSharedGroup(result);
						Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
						if (onCosmeticsUpdated != null)
						{
							onCosmeticsUpdated();
						}
						this.v2_isCosmeticPlayFabCatalogDataLoaded = true;
						Action v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess = this.V2_OnGetCosmeticsPlayFabCatalogData_PostSuccess;
						if (v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess != null)
						{
							v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess();
						}
						CosmeticsV2Spawner_Dirty.PrepareLoadOpInfos();
					}
					finally
					{
						this.CompleteGetCosmeticsPlayFabCatalogData();
					}
				}, delegate(PlayFabError error)
				{
					if (error.Error == PlayFabErrorCode.NotAuthenticated)
					{
						PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					}
					else if (error.Error == PlayFabErrorCode.AccountBanned)
					{
						Application.Quit();
						NetworkSystem.Instance.ReturnToSinglePlayer();
						Object.DestroyImmediate(PhotonNetworkController.Instance);
						Object.DestroyImmediate(GTPlayer.Instance);
						GameObject[] array2 = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
						for (int k = 0; k < array2.Length; k++)
						{
							Object.Destroy(array2[k]);
						}
					}
					if (!this.tryGetCatalogTwice)
					{
						this.tryGetCatalogTwice = true;
						this.GetCosmeticsPlayFabCatalogDataInternal();
						return;
					}
					this.CompleteGetCosmeticsPlayFabCatalogData();
				}, null, null);
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				}
				else if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array3 = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
					for (int l = 0; l < array3.Length; l++)
					{
						Object.Destroy(array3[l]);
					}
				}
				if (!this.tryGetCatalogTwice)
				{
					this.tryGetCatalogTwice = true;
					this.GetCosmeticsPlayFabCatalogDataInternal();
					return;
				}
				this.CompleteGetCosmeticsPlayFabCatalogData();
			}, null, null);
		}

		// Token: 0x06006ABB RID: 27323 RVA: 0x002256EE File Offset: 0x002238EE
		private void CompleteGetCosmeticsPlayFabCatalogData()
		{
			this.catalogRequestInFlight = false;
			if (this.catalogRequestRerunQueued)
			{
				this.catalogRequestRerunQueued = false;
				this.GetCosmeticsPlayFabCatalogData();
			}
		}

		// Token: 0x06006ABC RID: 27324 RVA: 0x0022570C File Offset: 0x0022390C
		public void SteamPurchase()
		{
			if (string.IsNullOrEmpty(this.itemToPurchase))
			{
				Debug.Log("Unable to start steam purchase process. itemToPurchase is not set.");
				return;
			}
			Debug.Log(string.Format("attempting to purchase item through steam. Is this a bundle purchase: {0}", this.buyingBundle));
			PlayFabClientAPI.StartPurchase(this.GetStartPurchaseRequest(), new Action<StartPurchaseResult>(this.ProcessStartPurchaseResponse), new Action<PlayFabError>(this.ProcessSteamPurchaseError), null, null);
		}

		// Token: 0x06006ABD RID: 27325 RVA: 0x00225770 File Offset: 0x00223970
		private StartPurchaseRequest GetStartPurchaseRequest()
		{
			return new StartPurchaseRequest
			{
				CatalogVersion = this.catalog,
				Items = new List<ItemPurchaseRequest>
				{
					new ItemPurchaseRequest
					{
						ItemId = this.itemToPurchase,
						Quantity = 1U,
						Annotation = "Purchased via in-game store"
					}
				}
			};
		}

		// Token: 0x06006ABE RID: 27326 RVA: 0x002257C4 File Offset: 0x002239C4
		private void ProcessStartPurchaseResponse(StartPurchaseResult result)
		{
			Debug.Log("successfully started purchase. attempted to pay for purchase through steam");
			this.currentPurchaseID = result.OrderId;
			PlayFabClientAPI.PayForPurchase(CosmeticsController.GetPayForPurchaseRequest(this.currentPurchaseID), new Action<PayForPurchaseResult>(CosmeticsController.ProcessPayForPurchaseResult), new Action<PlayFabError>(this.ProcessSteamPurchaseError), null, null);
		}

		// Token: 0x06006ABF RID: 27327 RVA: 0x00225811 File Offset: 0x00223A11
		private static PayForPurchaseRequest GetPayForPurchaseRequest(string orderId)
		{
			return new PayForPurchaseRequest
			{
				OrderId = orderId,
				ProviderName = "Steam",
				Currency = "RM"
			};
		}

		// Token: 0x06006AC0 RID: 27328 RVA: 0x00225835 File Offset: 0x00223A35
		private static void ProcessPayForPurchaseResult(PayForPurchaseResult result)
		{
			Debug.Log("succeeded on sending request for paying with steam! waiting for response");
		}

		// Token: 0x06006AC1 RID: 27329 RVA: 0x00225844 File Offset: 0x00223A44
		private void ProcessSteamCallback(MicroTxnAuthorizationResponse_t callBackResponse)
		{
			if (SubscriptionKiosk.ProcessingSubscriptionPurchase || GeodeAtm.ProcessingGeodePurchase)
			{
				return;
			}
			Debug.Log("Steam has called back that the user has finished the payment interaction");
			if (callBackResponse.m_bAuthorized == 0)
			{
				Debug.Log("Steam has indicated that the payment was not authorised.");
			}
			if (this.buyingBundle)
			{
				PlayFabClientAPI.ConfirmPurchase(this.GetConfirmBundlePurchaseRequest(), delegate(ConfirmPurchaseResult _)
				{
					this.ProcessConfirmPurchaseSuccess();
				}, new Action<PlayFabError>(this.ProcessConfirmPurchaseError), null, null);
				return;
			}
			PlayFabClientAPI.ConfirmPurchase(this.GetConfirmATMPurchaseRequest(), delegate(ConfirmPurchaseResult _)
			{
				this.ProcessConfirmPurchaseSuccess();
			}, new Action<PlayFabError>(this.ProcessConfirmPurchaseError), null, null);
		}

		// Token: 0x06006AC2 RID: 27330 RVA: 0x002258D0 File Offset: 0x00223AD0
		private ConfirmPurchaseRequest GetConfirmBundlePurchaseRequest()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"PlayerName",
					GorillaComputer.instance.savedName
				},
				{
					"Location",
					this.ConsumePurchaseLocation()
				}
			};
			if (this.validatedCreatorCode != null)
			{
				dictionary.Add("NexusCreatorId", this.validatedCreatorCode.memberCode);
				dictionary.Add("NexusGroupId", this.validatedCreatorCode.groupId);
			}
			return new ConfirmPurchaseRequest
			{
				OrderId = this.currentPurchaseID,
				CustomTags = dictionary
			};
		}

		// Token: 0x06006AC3 RID: 27331 RVA: 0x00225958 File Offset: 0x00223B58
		private ConfirmPurchaseRequest GetConfirmATMPurchaseRequest()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"PlayerName",
					GorillaComputer.instance.savedName
				},
				{
					"Location",
					this.ConsumePurchaseLocation()
				}
			};
			if (this.validatedCreatorCode != null)
			{
				dictionary.Add("NexusCreatorId", this.validatedCreatorCode.memberCode);
				dictionary.Add("NexusGroupId", this.validatedCreatorCode.groupId);
			}
			return new ConfirmPurchaseRequest
			{
				OrderId = this.currentPurchaseID,
				CustomTags = dictionary
			};
		}

		// Token: 0x06006AC4 RID: 27332 RVA: 0x002259E0 File Offset: 0x00223BE0
		private void ProcessConfirmPurchaseSuccess()
		{
			if (this.buyingBundle)
			{
				if (this.validatedCreatorCode != null && CosmeticsController.PushTerminalMessage != null)
				{
					CosmeticsController.PushTerminalMessage(this.validatedCreatorCode.terminalId, "THIS PURCHASE SUPPORTED\n" + CreatorCodes.supportedMember.name + "!");
				}
				this.buyingBundle = false;
				this.UpdateMyCosmetics();
				base.StartCoroutine(this.CheckIfMyCosmeticsUpdated(this.BundlePlayfabItemName));
			}
			else
			{
				ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Success);
			}
			this.GetCurrencyBalance();
			this.UpdateCurrencyBoards();
			this.GetCosmeticsPlayFabCatalogData();
			GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
		}

		// Token: 0x06006AC5 RID: 27333 RVA: 0x00225A82 File Offset: 0x00223C82
		private void ProcessConfirmPurchaseError(PlayFabError error)
		{
			this.ProcessSteamPurchaseError(error);
			ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Failure);
			this.UpdateCurrencyBoards();
		}

		// Token: 0x06006AC6 RID: 27334 RVA: 0x00225AA0 File Offset: 0x00223CA0
		private void ProcessSteamPurchaseError(PlayFabError error)
		{
			PlayFabErrorCode error2 = error.Error;
			if (error2 <= PlayFabErrorCode.PurchaseInitializationFailure)
			{
				if (error2 <= PlayFabErrorCode.FailedByPaymentProvider)
				{
					if (error2 == PlayFabErrorCode.AccountBanned)
					{
						PhotonNetwork.Disconnect();
						Object.DestroyImmediate(PhotonNetworkController.Instance);
						Object.DestroyImmediate(GTPlayer.Instance);
						GameObject[] array = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
						for (int i = 0; i < array.Length; i++)
						{
							Object.Destroy(array[i]);
						}
						Application.Quit();
						goto IL_01A2;
					}
					if (error2 != PlayFabErrorCode.FailedByPaymentProvider)
					{
						goto IL_0192;
					}
					Debug.Log(string.Format("Attempted to pay for order, but has been Failed by Steam with error: {0}", error));
					goto IL_01A2;
				}
				else
				{
					if (error2 == PlayFabErrorCode.InsufficientFunds)
					{
						Debug.Log(string.Format("Attempting to do purchase through steam, steam has returned insufficient funds: {0}", error));
						goto IL_01A2;
					}
					if (error2 == PlayFabErrorCode.InvalidPaymentProvider)
					{
						Debug.Log(string.Format("Attempted to connect to steam as payment provider, but received error: {0}", error));
						goto IL_01A2;
					}
					if (error2 != PlayFabErrorCode.PurchaseInitializationFailure)
					{
						goto IL_0192;
					}
				}
			}
			else if (error2 <= PlayFabErrorCode.InvalidPurchaseTransactionStatus)
			{
				if (error2 == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					goto IL_01A2;
				}
				if (error2 == PlayFabErrorCode.PurchaseDoesNotExist)
				{
					Debug.Log(string.Format("Attempting to confirm purchase for order {0} but received error: {1}", this.currentPurchaseID, error));
					goto IL_01A2;
				}
				if (error2 != PlayFabErrorCode.InvalidPurchaseTransactionStatus)
				{
					goto IL_0192;
				}
			}
			else
			{
				if (error2 == PlayFabErrorCode.InternalServerError)
				{
					Debug.Log(string.Format("PlayFab threw an internal server error: {0}", error));
					goto IL_01A2;
				}
				if (error2 == PlayFabErrorCode.StoreNotFound)
				{
					Debug.Log(string.Format("Attempted to load {0} from {1} but received an error: {2}", this.itemToPurchase, this.catalog, error));
					goto IL_01A2;
				}
				if (error2 != PlayFabErrorCode.DuplicatePurchaseTransactionId)
				{
					goto IL_0192;
				}
			}
			Debug.Log(string.Format("Attempted to pay for order {0}, however received an error: {1}", this.currentPurchaseID, error));
			goto IL_01A2;
			IL_0192:
			Debug.Log(string.Format("Steam purchase flow returned error: {0}", error));
			IL_01A2:
			ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Failure);
		}

		// Token: 0x06006AC7 RID: 27335 RVA: 0x00225C5C File Offset: 0x00223E5C
		public void UpdateCurrencyBoards()
		{
			this.FormattedPurchaseText(this.finalLine, null, null, false, false);
			this.iterator = 0;
			while (this.iterator < this.currencyBoards.Count)
			{
				if (this.currencyBoards[this.iterator].IsNotNull())
				{
					this.currencyBoards[this.iterator].UpdateCurrencyBoard(this.checkedDaily, this.gotMyDaily, this.currencyBalance, this.secondsUntilTomorrow);
				}
				this.iterator++;
			}
		}

		// Token: 0x06006AC8 RID: 27336 RVA: 0x00225CE9 File Offset: 0x00223EE9
		public void AddCurrencyBoard(CurrencyBoard newCurrencyBoard)
		{
			if (this.currencyBoards.Contains(newCurrencyBoard))
			{
				return;
			}
			this.currencyBoards.Add(newCurrencyBoard);
			newCurrencyBoard.UpdateCurrencyBoard(this.checkedDaily, this.gotMyDaily, this.currencyBalance, this.secondsUntilTomorrow);
		}

		// Token: 0x06006AC9 RID: 27337 RVA: 0x00225D24 File Offset: 0x00223F24
		public void RemoveCurrencyBoard(CurrencyBoard currencyBoardToRemove)
		{
			this.currencyBoards.Remove(currencyBoardToRemove);
		}

		// Token: 0x06006ACA RID: 27338 RVA: 0x00225D33 File Offset: 0x00223F33
		public void GetCurrencyBalance()
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				this.currencyBalance = result.VirtualCurrency[this.currencyName];
				this.UpdateCurrencyBoards();
				Action onGetCurrency = this.OnGetCurrency;
				if (onGetCurrency == null)
				{
					return;
				}
				onGetCurrency();
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			}, null, null);
		}

		// Token: 0x06006ACB RID: 27339 RVA: 0x00225D6C File Offset: 0x00223F6C
		public string GetItemDisplayName(CosmeticsController.CosmeticItem item)
		{
			if (item.overrideDisplayName != null && item.overrideDisplayName != "")
			{
				return item.overrideDisplayName;
			}
			return item.displayName;
		}

		// Token: 0x06006ACC RID: 27340 RVA: 0x00225D95 File Offset: 0x00223F95
		public void UpdateMyCosmetics()
		{
			if (GorillaServer.Instance == null)
			{
				return;
			}
			GorillaServer.Instance.UpdateUserCosmetics();
		}

		// Token: 0x06006ACD RID: 27341 RVA: 0x00225DB4 File Offset: 0x00223FB4
		private void AlreadyOwnAllBundleButtons()
		{
			EarlyAccessButton[] array = this.earlyAccessButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AlreadyOwn();
			}
		}

		// Token: 0x06006ACE RID: 27342 RVA: 0x00225DDE File Offset: 0x00223FDE
		public void CheckCosmeticsSharedGroup()
		{
			this.updateCosmeticsRetries++;
			if (this.updateCosmeticsRetries < this.maxUpdateCosmeticsRetries)
			{
				base.StartCoroutine(this.WaitForNextCosmeticsAttempt());
			}
		}

		// Token: 0x06006ACF RID: 27343 RVA: 0x00225E09 File Offset: 0x00224009
		private IEnumerator WaitForNextCosmeticsAttempt()
		{
			int num = (int)Mathf.Pow(3f, (float)(this.updateCosmeticsRetries + 1));
			yield return new WaitForSecondsRealtime((float)num);
			this.ConfirmIndividualCosmeticsSharedGroup(this.latestInventory);
			yield break;
		}

		// Token: 0x06006AD0 RID: 27344 RVA: 0x00225E18 File Offset: 0x00224018
		private void ConfirmIndividualCosmeticsSharedGroup(GetUserInventoryResult inventory)
		{
			this.latestInventory = inventory;
			if (PhotonNetwork.LocalPlayer.UserId == null)
			{
				base.StartCoroutine(this.WaitForNextCosmeticsAttempt());
				return;
			}
			PlayFabClientAPI.GetSharedGroupData(new global::PlayFab.ClientModels.GetSharedGroupDataRequest
			{
				Keys = new List<string> { "Inventory" },
				SharedGroupId = PhotonNetwork.LocalPlayer.UserId + "Inventory"
			}, delegate(GetSharedGroupDataResult result)
			{
				bool flag = true;
				foreach (KeyValuePair<string, global::PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
				{
					foreach (ItemInstance itemInstance in inventory.Inventory)
					{
						if (itemInstance.CatalogVersion == CosmeticsController.instance.catalog && !keyValuePair.Value.Value.Contains(itemInstance.ItemId))
						{
							flag = false;
							break;
						}
					}
				}
				if (!flag || result.Data.Count == 0)
				{
					this.UpdateMyCosmetics();
					return;
				}
				this.updateCosmeticsRetries = 0;
			}, delegate(PlayFabError error)
			{
				this.ReauthOrBan(error);
				this.CheckCosmeticsSharedGroup();
			}, null, null);
		}

		// Token: 0x06006AD1 RID: 27345 RVA: 0x00225EB4 File Offset: 0x002240B4
		public void ReauthOrBan(PlayFabError error)
		{
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				return;
			}
			if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				Application.Quit();
				PhotonNetwork.Disconnect();
				Object.DestroyImmediate(PhotonNetworkController.Instance);
				Object.DestroyImmediate(GTPlayer.Instance);
				GameObject[] array = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
				for (int i = 0; i < array.Length; i++)
				{
					Object.Destroy(array[i]);
				}
			}
		}

		// Token: 0x06006AD2 RID: 27346 RVA: 0x00225F28 File Offset: 0x00224128
		public void ProcessExternalUnlock(string itemID, bool autoEquip, bool isLeftHand)
		{
			this.UnlockItem(itemID, false);
			GorillaTagger.Instance.offlineVRRig.AddCosmetic(itemID, 0);
			this.UpdateMyCosmetics();
			if (autoEquip)
			{
				CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(itemID);
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.external_item_claim, itemFromDict);
				List<CosmeticsController.CosmeticSlots> list = CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Get();
				if (list.Capacity < 16)
				{
					list.Capacity = 16;
				}
				this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true, list);
				foreach (CosmeticsController.CosmeticSlots cosmeticSlots in list)
				{
					this.tryOnSet.items[(int)cosmeticSlots] = this.nullItem;
				}
				CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Release(list);
				this.UpdateShoppingCart();
				this.UpdateWornCosmetics(true);
				Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
				if (onCosmeticsUpdated == null)
				{
					return;
				}
				onCosmeticsUpdated();
			}
		}

		// Token: 0x06006AD3 RID: 27347 RVA: 0x00226010 File Offset: 0x00224210
		public void AddTempUnlockToWardrobe(string cosmeticID)
		{
			int num = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => cosmeticID == x.itemName);
			if (num < 0)
			{
				return;
			}
			switch (this.allCosmetics[num].itemCategory)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				this.ModifyUnlockList(this.unlockedHats, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Badge:
				this.ModifyUnlockList(this.unlockedBadges, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Face:
				this.ModifyUnlockList(this.unlockedFaces, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Paw:
				if (!this.allCosmetics[num].isThrowable)
				{
					this.ModifyUnlockList(this.unlockedPaws, num, false);
					return;
				}
				this.ModifyUnlockList(this.unlockedThrowables, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Chest:
				this.ModifyUnlockList(this.unlockedChests, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Fur:
				this.ModifyUnlockList(this.unlockedFurs, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Shirt:
				this.ModifyUnlockList(this.unlockedShirts, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Back:
				this.ModifyUnlockList(this.unlockedBacks, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Arms:
				this.ModifyUnlockList(this.unlockedArms, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Pants:
				this.ModifyUnlockList(this.unlockedPants, num, false);
				return;
			case CosmeticsController.CosmeticCategory.TagEffect:
				this.ModifyUnlockList(this.unlockedTagFX, num, false);
				return;
			case CosmeticsController.CosmeticCategory.Count:
				break;
			case CosmeticsController.CosmeticCategory.Set:
				foreach (string text in this.allCosmetics[num].bundledItems)
				{
					this.AddTempUnlockToWardrobe(text);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006AD4 RID: 27348 RVA: 0x00226190 File Offset: 0x00224390
		public void RemoveTempUnlockFromWardrobe(string cosmeticID)
		{
			int num = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => cosmeticID == x.itemName);
			if (num < 0)
			{
				return;
			}
			switch (this.allCosmetics[num].itemCategory)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				this.ModifyUnlockList(this.unlockedHats, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Badge:
				this.ModifyUnlockList(this.unlockedBadges, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Face:
				this.ModifyUnlockList(this.unlockedFaces, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Paw:
				if (!this.allCosmetics[num].isThrowable)
				{
					this.ModifyUnlockList(this.unlockedPaws, num, true);
					return;
				}
				this.ModifyUnlockList(this.unlockedThrowables, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Chest:
				this.ModifyUnlockList(this.unlockedChests, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Fur:
				this.ModifyUnlockList(this.unlockedFurs, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Shirt:
				this.ModifyUnlockList(this.unlockedShirts, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Back:
				this.ModifyUnlockList(this.unlockedBacks, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Arms:
				this.ModifyUnlockList(this.unlockedArms, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Pants:
				this.ModifyUnlockList(this.unlockedPants, num, true);
				return;
			case CosmeticsController.CosmeticCategory.TagEffect:
				this.ModifyUnlockList(this.unlockedTagFX, num, true);
				return;
			case CosmeticsController.CosmeticCategory.Count:
				break;
			case CosmeticsController.CosmeticCategory.Set:
				foreach (string text in this.allCosmetics[num].bundledItems)
				{
					this.RemoveTempUnlockFromWardrobe(text);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006AD5 RID: 27349 RVA: 0x0022630F File Offset: 0x0022450F
		public bool BuildValidationCheck()
		{
			if (this.m_earlyAccessSupporterPackCosmeticSO == null)
			{
				Debug.LogError("m_earlyAccessSupporterPackCosmeticSO is empty, everything will break!");
				return false;
			}
			return true;
		}

		// Token: 0x06006AD6 RID: 27350 RVA: 0x0022632C File Offset: 0x0022452C
		public void SetHideCosmeticsFromRemotePlayers(bool hideCosmetics)
		{
			if (hideCosmetics == this.isHidingCosmeticsFromRemotePlayers)
			{
				return;
			}
			this.isHidingCosmeticsFromRemotePlayers = hideCosmetics;
			GorillaTagger.Instance.offlineVRRig.reliableState.SetIsDirty();
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x06006AD7 RID: 27351 RVA: 0x0022635C File Offset: 0x0022455C
		public bool ValidatePackedItems(int[] packed)
		{
			if (packed == null)
			{
				return false;
			}
			if (packed.Length == 0)
			{
				return true;
			}
			int num = 0;
			int num2 = packed[0];
			for (int i = 0; i < 16; i++)
			{
				if ((num2 & (1 << i)) != 0)
				{
					num++;
				}
			}
			return packed.Length == num + 1;
		}

		// Token: 0x06006AD8 RID: 27352 RVA: 0x002263A0 File Offset: 0x002245A0
		public static int[] PackCollectableItems(List<CosmeticsController.CosmeticItem> items)
		{
			if (items == null || items.Count == 0)
			{
				return Array.Empty<int>();
			}
			int[] array = new int[items.Count];
			for (int i = 0; i < items.Count; i++)
			{
				string itemName = items[i].itemName;
				array[i] = (int)(itemName[0] - 'A' + '\u001a' * (itemName[1] - 'A' + '\u001a' * (itemName[2] - 'A' + '\u001a' * (itemName[3] - 'A' + '\u001a' * (itemName[4] - 'A')))));
			}
			return array;
		}

		// Token: 0x06006AD9 RID: 27353 RVA: 0x00226430 File Offset: 0x00224630
		public CosmeticsController.CosmeticItem[] UnpackCollectableItems(int[] packed)
		{
			if (packed == null || packed.Length == 0)
			{
				return Array.Empty<CosmeticsController.CosmeticItem>();
			}
			char[] array = new char[] { '\0', '\0', '\0', '\0', '\0', '.' };
			CosmeticsController.CosmeticItem[] array2 = new CosmeticsController.CosmeticItem[packed.Length];
			for (int i = 0; i < packed.Length; i++)
			{
				int num = packed[i];
				array[0] = (char)(65 + num % 26);
				array[1] = (char)(65 + num / 26 % 26);
				array[2] = (char)(65 + num / 676 % 26);
				array[3] = (char)(65 + num / 17576 % 26);
				array[4] = (char)(65 + num / 456976 % 26);
				array2[i] = this.GetItemFromDict(new string(array));
			}
			return array2;
		}

		// Token: 0x06006ADA RID: 27354 RVA: 0x002264D1 File Offset: 0x002246D1
		public void SetValidatedCreatorCode(string memberCode, string groupCode, string terminalId)
		{
			this.validatedCreatorCode = new CosmeticsController.ValidatedCreatorCode();
			this.validatedCreatorCode.memberCode = memberCode;
			this.validatedCreatorCode.groupId = groupCode;
			this.validatedCreatorCode.terminalId = terminalId;
		}

		// Token: 0x17000A30 RID: 2608
		// (get) Token: 0x06006ADB RID: 27355 RVA: 0x00226502 File Offset: 0x00224702
		public static int SelectedOutfit
		{
			get
			{
				return CosmeticsController.selectedOutfit;
			}
		}

		// Token: 0x06006ADC RID: 27356 RVA: 0x00226509 File Offset: 0x00224709
		public static bool CanScrollOutfits()
		{
			return CosmeticsController.loadedSavedOutfits && !CosmeticsController.saveOutfitInProgress;
		}

		// Token: 0x06006ADD RID: 27357 RVA: 0x0022651C File Offset: 0x0022471C
		public void PressWardrobeScrollOutfit(bool forward)
		{
			int num = CosmeticsController.selectedOutfit;
			if (forward)
			{
				num = (num + 1) % CosmeticsController.maxOutfits;
			}
			else
			{
				num--;
				if (num < 0)
				{
					num = CosmeticsController.maxOutfits - 1;
				}
			}
			this.LoadSavedOutfit(num);
		}

		// Token: 0x06006ADE RID: 27358 RVA: 0x00226558 File Offset: 0x00224758
		public void LoadSavedOutfit(int newOutfitIndex)
		{
			if (!CosmeticsController.CanScrollOutfits() || newOutfitIndex == CosmeticsController.selectedOutfit || newOutfitIndex < 0 || newOutfitIndex >= CosmeticsController.maxOutfits)
			{
				return;
			}
			this.savedOutfits[CosmeticsController.selectedOutfit].CopyItems(this.currentWornSet);
			this.savedColors[CosmeticsController.selectedOutfit] = new Vector3(VRRig.LocalRig.playerColor.r, VRRig.LocalRig.playerColor.g, VRRig.LocalRig.playerColor.b);
			this.SaveOutfitsToMothership();
			CosmeticsController.selectedOutfit = newOutfitIndex;
			PlayerPrefs.SetInt(this.outfitSystemConfig.selectedOutfitPref, CosmeticsController.selectedOutfit);
			PlayerPrefs.Save();
			CosmeticsController.CosmeticSet cosmeticSet = this.savedOutfits[CosmeticsController.selectedOutfit];
			bool flag = true;
			for (int i = 0; i < 16; i++)
			{
				CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots)i;
				if ((cosmeticSlots != CosmeticsController.CosmeticSlots.ArmLeft && cosmeticSlots != CosmeticsController.CosmeticSlots.ArmRight) || flag)
				{
					this.ApplyNewItem(cosmeticSet, i);
				}
			}
			this.UpdateMonkeColor(this.savedColors[CosmeticsController.selectedOutfit], true);
			this.SaveCurrentItemPreferences();
			this.UpdateShoppingCart();
			this.UpdateWornCosmetics(true, true);
			this.UpdateWardrobeModelsAndButtons();
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated();
		}

		// Token: 0x06006ADF RID: 27359 RVA: 0x0022667C File Offset: 0x0022487C
		private void ApplyNewItem(CosmeticsController.CosmeticSet outfit, int i)
		{
			this.currentWornSet.items[i] = outfit.items[i];
			if (!outfit.items[i].isNullItem)
			{
				this.tryOnSet.items[i] = this.nullItem;
			}
		}

		// Token: 0x06006AE0 RID: 27360 RVA: 0x002266D0 File Offset: 0x002248D0
		private async void LoadSavedOutfits()
		{
			try
			{
				while (!SubscriptionManager.LocalSubscriptionDataResolved)
				{
					await Task.Yield();
				}
				CosmeticsController.maxOutfits = (SubscriptionManager.IsLocalSubscribed() ? this.outfitSystemConfig.subscriberMaxOutfits : this.outfitSystemConfig.nonSubscriberMaxOutfits);
				if (!CosmeticsController.loadedSavedOutfits && !CosmeticsController.loadOutfitsInProgress)
				{
					CosmeticsController.loadOutfitsInProgress = true;
					this.savedOutfits = new CosmeticsController.CosmeticSet[CosmeticsController.maxOutfits];
					this.savedColors = new Vector3[CosmeticsController.maxOutfits];
					if (!MothershipClientApiUnity.GetUserDataValue(this.outfitSystemConfig.mothershipKey, new Action<MothershipUserData>(this.GetSavedOutfitsSuccess), new Action<MothershipError, int>(this.GetSavedOutfitsFail), ""))
					{
						GTDev.LogError<string>("CosmeticsController LoadSavedOutfits GetUserDataValue failed", null);
						this.ClearOutfits();
						CosmeticsController.loadOutfitsInProgress = false;
						CosmeticsController.loadedSavedOutfits = true;
						Action onOutfitsUpdated = this.OnOutfitsUpdated;
						if (onOutfitsUpdated != null)
						{
							onOutfitsUpdated();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		// Token: 0x06006AE1 RID: 27361 RVA: 0x00226708 File Offset: 0x00224908
		private void GetSavedOutfitsSuccess(MothershipUserData response)
		{
			if (response != null && response.value != null && response.value.Length > 0)
			{
				try
				{
					byte[] array = Convert.FromBase64String(response.value);
					this.outfitStringMothership = Encoding.UTF8.GetString(array);
					this.StringToOutfits(this.outfitStringMothership);
					goto IL_006E;
				}
				catch (Exception ex)
				{
					GTDev.LogError<string>("CosmeticsController GetSavedOutfitsSuccess error decoding " + ex.Message, null);
					this.ClearOutfits();
					goto IL_006E;
				}
			}
			this.ClearOutfits();
			IL_006E:
			this.GetSavedOutfitsComplete();
		}

		// Token: 0x06006AE2 RID: 27362 RVA: 0x0022679C File Offset: 0x0022499C
		private void GetSavedOutfitsFail(MothershipError error, int status)
		{
			GTDev.LogError<string>(string.Format("CosmeticsController GetSavedOutfitsFail {0} {1}", status, error.Message), null);
			this.ClearOutfits();
			this.GetSavedOutfitsComplete();
		}

		// Token: 0x06006AE3 RID: 27363 RVA: 0x002267C8 File Offset: 0x002249C8
		private void GetSavedOutfitsComplete()
		{
			int num = PlayerPrefs.GetInt(this.outfitSystemConfig.selectedOutfitPref, 0);
			if (num < 0 || num >= CosmeticsController.maxOutfits)
			{
				num = 0;
			}
			else
			{
				CosmeticsController.CosmeticSet cosmeticSet = new CosmeticsController.CosmeticSet();
				cosmeticSet.LoadFromPlayerPreferences(this);
				if (cosmeticSet.HasAnyItems())
				{
					this.savedOutfits[num].CopyItems(cosmeticSet);
				}
				float @float = PlayerPrefs.GetFloat("redValue", 0f);
				float float2 = PlayerPrefs.GetFloat("greenValue", 0f);
				float float3 = PlayerPrefs.GetFloat("blueValue", 0f);
				if (@float > 0f || float2 > 0f || float3 > 0f)
				{
					this.savedColors[num] = new Vector3(@float, float2, float3);
				}
			}
			CosmeticsController.selectedOutfit = num;
			this.currentWornSet.CopyItems(this.savedOutfits[CosmeticsController.selectedOutfit]);
			this.UpdateMonkeColor(this.savedColors[CosmeticsController.selectedOutfit], true);
			CosmeticsController.loadedSavedOutfits = true;
			CosmeticsController.loadOutfitsInProgress = false;
			Action onOutfitsUpdated = this.OnOutfitsUpdated;
			if (onOutfitsUpdated == null)
			{
				return;
			}
			onOutfitsUpdated();
		}

		// Token: 0x06006AE4 RID: 27364 RVA: 0x002268CC File Offset: 0x00224ACC
		private void UpdateMonkeColor(Vector3 col, bool saveToPrefs)
		{
			float num = Mathf.Clamp(col.x, 0f, 1f);
			float num2 = Mathf.Clamp(col.y, 0f, 1f);
			float num3 = Mathf.Clamp(col.z, 0f, 1f);
			GorillaTagger.Instance.UpdateColor(num, num2, num3);
			GorillaComputer.instance.UpdateColor(num, num2, num3);
			if (CosmeticsController.OnPlayerColorSet != null)
			{
				CosmeticsController.OnPlayerColorSet(num, num2, num3);
			}
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { num, num2, num3 });
			}
			if (saveToPrefs)
			{
				PlayerPrefs.SetFloat("redValue", num);
				PlayerPrefs.SetFloat("greenValue", num2);
				PlayerPrefs.SetFloat("blueValue", num3);
				PlayerPrefs.Save();
			}
		}

		// Token: 0x06006AE5 RID: 27365 RVA: 0x002269B8 File Offset: 0x00224BB8
		private void SaveOutfitsToMothership()
		{
			if (!CosmeticsController.loadedSavedOutfits || CosmeticsController.saveOutfitInProgress)
			{
				return;
			}
			string mothershipKey = this.outfitSystemConfig.mothershipKey;
			this.outfitStringPendingSave = this.OutfitsToString();
			if (this.outfitStringPendingSave.Equals(this.outfitStringMothership))
			{
				return;
			}
			CosmeticsController.saveOutfitInProgress = true;
			if (!MothershipClientApiUnity.SetUserDataValue(mothershipKey, this.outfitStringPendingSave, new Action<SetUserDataResponse>(this.SaveOutfitsToMothershipSuccess), new Action<MothershipError, int>(this.SaveOutfitsToMothershipFail), ""))
			{
				GTDev.LogError<string>("CosmeticsController SaveOutfitToMothership SetUserDataValue failed", null);
				CosmeticsController.saveOutfitInProgress = false;
			}
		}

		// Token: 0x06006AE6 RID: 27366 RVA: 0x00226A42 File Offset: 0x00224C42
		private void SaveOutfitsToMothershipSuccess(SetUserDataResponse response)
		{
			this.outfitStringMothership = this.outfitStringPendingSave;
			CosmeticsController.saveOutfitInProgress = false;
			Action onOutfitsUpdated = this.OnOutfitsUpdated;
			if (onOutfitsUpdated != null)
			{
				onOutfitsUpdated();
			}
			response.Dispose();
		}

		// Token: 0x06006AE7 RID: 27367 RVA: 0x00226A6D File Offset: 0x00224C6D
		private void SaveOutfitsToMothershipFail(MothershipError error, int status)
		{
			GTDev.LogError<string>(string.Format("CosmeticsController SaveOutfitsToMothershipFail {0} ", status) + error.Message, null);
			CosmeticsController.saveOutfitInProgress = false;
		}

		// Token: 0x06006AE8 RID: 27368 RVA: 0x00226A98 File Offset: 0x00224C98
		private string OutfitsToString()
		{
			if (!CosmeticsController.loadedSavedOutfits)
			{
				return string.Empty;
			}
			CosmeticsController.outfitDataTemp = new CosmeticsController.OutfitData();
			this.sb.Clear();
			for (int i = 0; i < this.savedOutfits.Length; i++)
			{
				CosmeticsController.outfitDataTemp.Clear();
				CosmeticsController.CosmeticSet cosmeticSet = this.savedOutfits[i];
				for (int j = 0; j < cosmeticSet.items.Length; j++)
				{
					CosmeticsController.CosmeticItem cosmeticItem = cosmeticSet.items[j];
					string text = ((cosmeticItem.isNullItem || string.IsNullOrEmpty(cosmeticItem.displayName)) ? "null" : cosmeticItem.displayName);
					CosmeticsController.outfitDataTemp.itemIDs.Add(text);
				}
				if (VRRig.LocalRig != null)
				{
					CosmeticsController.outfitDataTemp.color = this.savedColors[i];
				}
				this.sb.Append(JsonUtility.ToJson(CosmeticsController.outfitDataTemp));
				if (i < this.savedOutfits.Length - 1)
				{
					this.sb.Append(this.outfitSystemConfig.outfitSeparator);
				}
			}
			return this.sb.ToString();
		}

		// Token: 0x06006AE9 RID: 27369 RVA: 0x00226BB4 File Offset: 0x00224DB4
		private void ClearOutfits()
		{
			for (int i = 0; i < this.savedOutfits.Length; i++)
			{
				this.savedOutfits[i] = new CosmeticsController.CosmeticSet();
				this.savedOutfits[i].ClearSet(this.nullItem);
				this.savedColors[i] = CosmeticsController.defaultColor;
			}
		}

		// Token: 0x06006AEA RID: 27370 RVA: 0x00226C08 File Offset: 0x00224E08
		private void StringToOutfits(string response)
		{
			if (response.IsNullOrEmpty())
			{
				this.ClearOutfits();
				return;
			}
			try
			{
				string[] array = response.Split(this.outfitSystemConfig.outfitSeparator, StringSplitOptions.None);
				for (int i = 0; i < CosmeticsController.maxOutfits; i++)
				{
					this.savedOutfits[i] = new CosmeticsController.CosmeticSet();
					if (i >= array.Length)
					{
						this.savedOutfits[i].ClearSet(this.nullItem);
						this.savedColors[i] = CosmeticsController.defaultColor;
					}
					else
					{
						string text = array[i];
						if (text.IsNullOrEmpty())
						{
							this.savedOutfits[i].ClearSet(this.nullItem);
							this.savedColors[i] = CosmeticsController.defaultColor;
						}
						else
						{
							Vector3 vector;
							this.savedOutfits[i].ParseSetFromString(this, text, out vector);
							this.savedColors[i] = vector;
						}
					}
				}
			}
			catch (Exception ex)
			{
				GTDev.LogError<string>("CosmeticsController StringToOutfit Error parsing " + ex.Message, null);
				this.ClearOutfits();
			}
		}

		// Token: 0x040079E3 RID: 31203
		[FormerlySerializedAs("v2AllCosmeticsInfoAssetRef")]
		[FormerlySerializedAs("newSysAllCosmeticsAssetRef")]
		[SerializeField]
		public GTAssetRef<AllCosmeticsArraySO> v2_allCosmeticsInfoAssetRef;

		// Token: 0x040079E5 RID: 31205
		private readonly Dictionary<string, CosmeticInfoV2> _allCosmeticsDictV2 = new Dictionary<string, CosmeticInfoV2>();

		// Token: 0x040079E6 RID: 31206
		public Action V2_allCosmeticsInfoAssetRef_OnPostLoad;

		// Token: 0x040079E9 RID: 31209
		public const int maximumTransferrableItems = 5;

		// Token: 0x040079EA RID: 31210
		[OnEnterPlay_SetNull]
		public static volatile CosmeticsController instance;

		// Token: 0x040079EC RID: 31212
		public static Action<string, string> PushTerminalMessage;

		// Token: 0x040079ED RID: 31213
		public Action V2_OnGetCosmeticsPlayFabCatalogData_PostSuccess;

		// Token: 0x040079EE RID: 31214
		public Action OnGetCurrency;

		// Token: 0x040079EF RID: 31215
		private string purchaseLocation;

		// Token: 0x040079F0 RID: 31216
		[FormerlySerializedAs("allCosmetics")]
		[SerializeField]
		private List<CosmeticsController.CosmeticItem> _allCosmetics;

		// Token: 0x040079F2 RID: 31218
		public Dictionary<string, CosmeticsController.CosmeticItem> _allCosmeticsDict = new Dictionary<string, CosmeticsController.CosmeticItem>(2048);

		// Token: 0x040079F4 RID: 31220
		public Dictionary<string, string> _allCosmeticsItemIDsfromDisplayNamesDict = new Dictionary<string, string>(2048);

		// Token: 0x040079F5 RID: 31221
		public CosmeticsController.CosmeticItem nullItem;

		// Token: 0x040079F6 RID: 31222
		public string catalog;

		// Token: 0x040079F7 RID: 31223
		private string[] tempStringArray;

		// Token: 0x040079F8 RID: 31224
		private CosmeticsController.CosmeticItem tempItem;

		// Token: 0x040079F9 RID: 31225
		private VRRigAnchorOverrides anchorOverrides;

		// Token: 0x040079FA RID: 31226
		public List<CatalogItem> catalogItems;

		// Token: 0x040079FB RID: 31227
		public bool tryGetCatalogTwice;

		// Token: 0x040079FC RID: 31228
		private bool catalogRequestInFlight;

		// Token: 0x040079FD RID: 31229
		private bool catalogRequestRerunQueued;

		// Token: 0x040079FE RID: 31230
		public CustomMapCosmeticsData customMapCosmeticsData;

		// Token: 0x040079FF RID: 31231
		[NonSerialized]
		public CosmeticsController.CosmeticSet tryOnSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04007A00 RID: 31232
		public int numFittingRoomButtons = 12;

		// Token: 0x04007A01 RID: 31233
		public List<FittingRoom> fittingRooms = new List<FittingRoom>();

		// Token: 0x04007A02 RID: 31234
		public CosmeticStand[] cosmeticStands;

		// Token: 0x04007A03 RID: 31235
		public List<CosmeticsController.CosmeticItem> currentCart = new List<CosmeticsController.CosmeticItem>();

		// Token: 0x04007A04 RID: 31236
		public CosmeticsController.PurchaseItemStages currentPurchaseItemStage;

		// Token: 0x04007A05 RID: 31237
		public List<ItemCheckout> itemCheckouts = new List<ItemCheckout>();

		// Token: 0x04007A06 RID: 31238
		public CosmeticsController.CosmeticItem itemToBuy;

		// Token: 0x04007A07 RID: 31239
		private bool foundCosmetic;

		// Token: 0x04007A08 RID: 31240
		private int attempts;

		// Token: 0x04007A09 RID: 31241
		private string finalLine;

		// Token: 0x04007A0A RID: 31242
		private string leftCheckoutPurchaseButtonString;

		// Token: 0x04007A0B RID: 31243
		private string rightCheckoutPurchaseButtonString;

		// Token: 0x04007A0C RID: 31244
		private bool leftCheckoutPurchaseButtonOn;

		// Token: 0x04007A0D RID: 31245
		private bool rightCheckoutPurchaseButtonOn;

		// Token: 0x04007A0E RID: 31246
		private bool isLastHandTouchedLeft;

		// Token: 0x04007A0F RID: 31247
		private CosmeticsController.CosmeticSet cachedSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04007A11 RID: 31249
		public readonly List<WardrobeInstance> wardrobes = new List<WardrobeInstance>();

		// Token: 0x04007A12 RID: 31250
		public List<CosmeticsController.CosmeticItem> unlockedCosmetics = new List<CosmeticsController.CosmeticItem>(2048);

		// Token: 0x04007A13 RID: 31251
		public List<CosmeticsController.CosmeticItem> unlockedHats = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007A14 RID: 31252
		public List<CosmeticsController.CosmeticItem> unlockedFaces = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007A15 RID: 31253
		public List<CosmeticsController.CosmeticItem> unlockedBadges = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007A16 RID: 31254
		public List<CosmeticsController.CosmeticItem> unlockedPaws = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007A17 RID: 31255
		public List<CosmeticsController.CosmeticItem> unlockedChests = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007A18 RID: 31256
		public List<CosmeticsController.CosmeticItem> unlockedFurs = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007A19 RID: 31257
		public List<CosmeticsController.CosmeticItem> unlockedShirts = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007A1A RID: 31258
		public List<CosmeticsController.CosmeticItem> unlockedPants = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007A1B RID: 31259
		public List<CosmeticsController.CosmeticItem> unlockedBacks = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007A1C RID: 31260
		public List<CosmeticsController.CosmeticItem> unlockedArms = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007A1D RID: 31261
		public List<CosmeticsController.CosmeticItem> unlockedTagFX = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007A1E RID: 31262
		public List<CosmeticsController.CosmeticItem> unlockedThrowables = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04007A1F RID: 31263
		public int[] cosmeticsPages = new int[11];

		// Token: 0x04007A20 RID: 31264
		private List<CosmeticsController.CosmeticItem>[] itemLists = new List<CosmeticsController.CosmeticItem>[11];

		// Token: 0x04007A21 RID: 31265
		private int wardrobeType;

		// Token: 0x04007A22 RID: 31266
		[NonSerialized]
		public CosmeticsController.CosmeticSet currentWornSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04007A23 RID: 31267
		[NonSerialized]
		public CosmeticsController.CosmeticSet tempUnlockedSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04007A24 RID: 31268
		[NonSerialized]
		public CosmeticsController.CosmeticSet activeMergedSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04007A25 RID: 31269
		[NonSerialized]
		public CosmeticsController.CosmeticItem tryOnCollectableItem;

		// Token: 0x04007A26 RID: 31270
		public string concatStringCosmeticsAllowed = "";

		// Token: 0x04007A27 RID: 31271
		public Action OnCosmeticsUpdated;

		// Token: 0x04007A28 RID: 31272
		[NonSerialized]
		public Dictionary<string, List<CosmeticsController.CosmeticItem>> collectablesByParentID = new Dictionary<string, List<CosmeticsController.CosmeticItem>>();

		// Token: 0x04007A29 RID: 31273
		[TupleElementNames(new string[] { "rig", "parentID" })]
		[NonSerialized]
		public Dictionary<ValueTuple<VRRig, string>, CosmeticsController.CollectionState> localCycleStates = new Dictionary<ValueTuple<VRRig, string>, CosmeticsController.CollectionState>();

		// Token: 0x04007A2A RID: 31274
		private static readonly List<CosmeticCollectionDisplay> scratchDisplayList = new List<CosmeticCollectionDisplay>();

		// Token: 0x04007A2B RID: 31275
		private static int[] cycleStatesArray = Array.Empty<int>();

		// Token: 0x04007A2C RID: 31276
		private static readonly List<CosmeticsController.CosmeticItem> scratchCanonicalCollectables = new List<CosmeticsController.CosmeticItem>();

		// Token: 0x04007A2D RID: 31277
		private static readonly List<CosmeticsController.CosmeticItem> scratchCanonicalIndexList = new List<CosmeticsController.CosmeticItem>();

		// Token: 0x04007A2E RID: 31278
		public int currencyBalance;

		// Token: 0x04007A2F RID: 31279
		public string currencyName;

		// Token: 0x04007A30 RID: 31280
		public List<CurrencyBoard> currencyBoards;

		// Token: 0x04007A31 RID: 31281
		public string itemToPurchase;

		// Token: 0x04007A32 RID: 31282
		public bool buyingBundle;

		// Token: 0x04007A33 RID: 31283
		public bool confirmedDidntPlayInBeta;

		// Token: 0x04007A34 RID: 31284
		public bool playedInBeta;

		// Token: 0x04007A35 RID: 31285
		public bool gotMyDaily;

		// Token: 0x04007A36 RID: 31286
		public bool checkedDaily;

		// Token: 0x04007A37 RID: 31287
		public string currentPurchaseID;

		// Token: 0x04007A38 RID: 31288
		public bool hasPrice;

		// Token: 0x04007A39 RID: 31289
		private int searchIndex;

		// Token: 0x04007A3A RID: 31290
		private int iterator;

		// Token: 0x04007A3B RID: 31291
		private CosmeticsController.CosmeticItem cosmeticItemVar;

		// Token: 0x04007A3C RID: 31292
		[SerializeField]
		private CosmeticSO m_earlyAccessSupporterPackCosmeticSO;

		// Token: 0x04007A3D RID: 31293
		public EarlyAccessButton[] earlyAccessButtons;

		// Token: 0x04007A3E RID: 31294
		private BundleList bundleList = new BundleList();

		// Token: 0x04007A3F RID: 31295
		private const string mothershipRewardsGrantedKey = "mshipRewardsGranted";

		// Token: 0x04007A40 RID: 31296
		public string BundleSkuName = "2024_i_lava_you_pack";

		// Token: 0x04007A41 RID: 31297
		public string BundlePlayfabItemName = "LSABG.";

		// Token: 0x04007A42 RID: 31298
		public int BundleShinyRocks = 10000;

		// Token: 0x04007A43 RID: 31299
		public DateTime currentTime;

		// Token: 0x04007A44 RID: 31300
		public string lastDailyLogin;

		// Token: 0x04007A45 RID: 31301
		public UserDataRecord userDataRecord;

		// Token: 0x04007A46 RID: 31302
		public int secondsUntilTomorrow;

		// Token: 0x04007A47 RID: 31303
		public float secondsToWaitToCheckDaily = 10f;

		// Token: 0x04007A48 RID: 31304
		private int updateCosmeticsRetries;

		// Token: 0x04007A49 RID: 31305
		private int maxUpdateCosmeticsRetries;

		// Token: 0x04007A4A RID: 31306
		private GetUserInventoryResult latestInventory;

		// Token: 0x04007A4B RID: 31307
		private string returnString;

		// Token: 0x04007A4C RID: 31308
		private bool checkoutCartButtonPressedWithLeft;

		// Token: 0x04007A4D RID: 31309
		private CosmeticsController.ValidatedCreatorCode validatedCreatorCode;

		// Token: 0x04007A4E RID: 31310
		private Callback<MicroTxnAuthorizationResponse_t> _steamMicroTransactionAuthorizationResponse;

		// Token: 0x04007A4F RID: 31311
		private static readonly List<CosmeticsController.CosmeticSlots> _g_default_outAppliedSlotsList_for_applyCosmeticItemToSet = new List<CosmeticsController.CosmeticSlots>(16);

		// Token: 0x04007A50 RID: 31312
		[SerializeField]
		private CosmeticOutfitSystemConfig outfitSystemConfig;

		// Token: 0x04007A51 RID: 31313
		private CosmeticsController.CosmeticSet[] savedOutfits;

		// Token: 0x04007A52 RID: 31314
		private Vector3[] savedColors;

		// Token: 0x04007A53 RID: 31315
		private static CosmeticsController.OutfitData outfitDataTemp;

		// Token: 0x04007A54 RID: 31316
		private string outfitStringMothership = string.Empty;

		// Token: 0x04007A55 RID: 31317
		private string outfitStringPendingSave = string.Empty;

		// Token: 0x04007A56 RID: 31318
		private static bool saveOutfitInProgress = false;

		// Token: 0x04007A57 RID: 31319
		private static bool loadOutfitsInProgress = false;

		// Token: 0x04007A58 RID: 31320
		private static bool loadedSavedOutfits = false;

		// Token: 0x04007A59 RID: 31321
		private static int selectedOutfit = 0;

		// Token: 0x04007A5A RID: 31322
		private static int maxOutfits = -1;

		// Token: 0x04007A5B RID: 31323
		private static readonly Vector3 defaultColor = new Vector3(0f, 0f, 0f);

		// Token: 0x04007A5C RID: 31324
		public Action OnOutfitsUpdated;

		// Token: 0x04007A5D RID: 31325
		public static Action<float, float, float> OnPlayerColorSet;

		// Token: 0x04007A5E RID: 31326
		private StringBuilder sb = new StringBuilder(256);

		// Token: 0x020010AA RID: 4266
		public enum PurchaseItemStages
		{
			// Token: 0x04007A60 RID: 31328
			Start,
			// Token: 0x04007A61 RID: 31329
			CheckoutButtonPressed,
			// Token: 0x04007A62 RID: 31330
			ItemSelected,
			// Token: 0x04007A63 RID: 31331
			ItemOwned,
			// Token: 0x04007A64 RID: 31332
			FinalPurchaseAcknowledgement,
			// Token: 0x04007A65 RID: 31333
			Buying,
			// Token: 0x04007A66 RID: 31334
			Success,
			// Token: 0x04007A67 RID: 31335
			Failure
		}

		// Token: 0x020010AB RID: 4267
		public enum CosmeticCategory
		{
			// Token: 0x04007A69 RID: 31337
			None,
			// Token: 0x04007A6A RID: 31338
			Hat,
			// Token: 0x04007A6B RID: 31339
			Badge,
			// Token: 0x04007A6C RID: 31340
			Face,
			// Token: 0x04007A6D RID: 31341
			Paw,
			// Token: 0x04007A6E RID: 31342
			Chest,
			// Token: 0x04007A6F RID: 31343
			Fur,
			// Token: 0x04007A70 RID: 31344
			Shirt,
			// Token: 0x04007A71 RID: 31345
			Back,
			// Token: 0x04007A72 RID: 31346
			Arms,
			// Token: 0x04007A73 RID: 31347
			Pants,
			// Token: 0x04007A74 RID: 31348
			TagEffect,
			// Token: 0x04007A75 RID: 31349
			Count,
			// Token: 0x04007A76 RID: 31350
			Set,
			// Token: 0x04007A77 RID: 31351
			Collectable
		}

		// Token: 0x020010AC RID: 4268
		public enum CosmeticSlots
		{
			// Token: 0x04007A79 RID: 31353
			Hat,
			// Token: 0x04007A7A RID: 31354
			Badge,
			// Token: 0x04007A7B RID: 31355
			Face,
			// Token: 0x04007A7C RID: 31356
			ArmLeft,
			// Token: 0x04007A7D RID: 31357
			ArmRight,
			// Token: 0x04007A7E RID: 31358
			BackLeft,
			// Token: 0x04007A7F RID: 31359
			BackRight,
			// Token: 0x04007A80 RID: 31360
			HandLeft,
			// Token: 0x04007A81 RID: 31361
			HandRight,
			// Token: 0x04007A82 RID: 31362
			Chest,
			// Token: 0x04007A83 RID: 31363
			Fur,
			// Token: 0x04007A84 RID: 31364
			Shirt,
			// Token: 0x04007A85 RID: 31365
			Pants,
			// Token: 0x04007A86 RID: 31366
			Back,
			// Token: 0x04007A87 RID: 31367
			Arms,
			// Token: 0x04007A88 RID: 31368
			TagEffect,
			// Token: 0x04007A89 RID: 31369
			Count
		}

		// Token: 0x020010AD RID: 4269
		[Serializable]
		public class CosmeticSet
		{
			// Token: 0x140000C0 RID: 192
			// (add) Token: 0x06006AFA RID: 27386 RVA: 0x002273BC File Offset: 0x002255BC
			// (remove) Token: 0x06006AFB RID: 27387 RVA: 0x002273F4 File Offset: 0x002255F4
			public event CosmeticsController.CosmeticSet.OnSetActivatedHandler onSetActivatedEvent;

			// Token: 0x06006AFC RID: 27388 RVA: 0x00227429 File Offset: 0x00225629
			protected void OnSetActivated(CosmeticsController.CosmeticSet prevSet, CosmeticsController.CosmeticSet currentSet, NetPlayer netPlayer)
			{
				if (this.onSetActivatedEvent != null)
				{
					this.onSetActivatedEvent(prevSet, currentSet, netPlayer);
				}
			}

			// Token: 0x17000A31 RID: 2609
			// (get) Token: 0x06006AFD RID: 27389 RVA: 0x00227444 File Offset: 0x00225644
			public static CosmeticsController.CosmeticSet EmptySet
			{
				get
				{
					if (CosmeticsController.CosmeticSet._emptySet == null)
					{
						string[] array = new string[16];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = "NOTHING";
						}
						CosmeticsController.CosmeticSet._emptySet = new CosmeticsController.CosmeticSet(array, CosmeticsController.instance);
					}
					return CosmeticsController.CosmeticSet._emptySet;
				}
			}

			// Token: 0x06006AFE RID: 27390 RVA: 0x0022748D File Offset: 0x0022568D
			public CosmeticSet()
			{
				this.items = new CosmeticsController.CosmeticItem[16];
			}

			// Token: 0x06006AFF RID: 27391 RVA: 0x002274B0 File Offset: 0x002256B0
			public CosmeticSet(string[] itemNames, CosmeticsController controller)
			{
				this.items = new CosmeticsController.CosmeticItem[16];
				for (int i = 0; i < itemNames.Length; i++)
				{
					string text = itemNames[i];
					string itemNameFromDisplayName = controller.GetItemNameFromDisplayName(text);
					this.items[i] = controller.GetItemFromDict(itemNameFromDisplayName);
				}
			}

			// Token: 0x06006B00 RID: 27392 RVA: 0x0022750C File Offset: 0x0022570C
			public CosmeticSet(int[] itemNamesPacked, CosmeticsController controller)
			{
				this.items = new CosmeticsController.CosmeticItem[16];
				int num = ((itemNamesPacked.Length != 0) ? itemNamesPacked[0] : 0);
				int num2 = 1;
				for (int i = 0; i < this.items.Length; i++)
				{
					if ((num & (1 << i)) != 0)
					{
						int num3 = itemNamesPacked[num2];
						if (num3 == -55)
						{
							this.items[i] = controller.GetItemFromDict("Slingshot");
						}
						else
						{
							CosmeticsController.CosmeticSet.nameScratchSpace[0] = (char)(65 + num3 % 26);
							CosmeticsController.CosmeticSet.nameScratchSpace[1] = (char)(65 + num3 / 26 % 26);
							CosmeticsController.CosmeticSet.nameScratchSpace[2] = (char)(65 + num3 / 676 % 26);
							CosmeticsController.CosmeticSet.nameScratchSpace[3] = (char)(65 + num3 / 17576 % 26);
							CosmeticsController.CosmeticSet.nameScratchSpace[4] = (char)(65 + num3 / 456976 % 26);
							CosmeticsController.CosmeticSet.nameScratchSpace[5] = '.';
							this.items[i] = controller.GetItemFromDict(new string(CosmeticsController.CosmeticSet.nameScratchSpace));
						}
						num2++;
					}
					else
					{
						this.items[i] = controller.GetItemFromDict("null");
					}
				}
			}

			// Token: 0x06006B01 RID: 27393 RVA: 0x00227634 File Offset: 0x00225834
			public void CopyItems(CosmeticsController.CosmeticSet other)
			{
				for (int i = 0; i < this.items.Length; i++)
				{
					this.items[i] = other.items[i];
				}
			}

			// Token: 0x06006B02 RID: 27394 RVA: 0x0022766C File Offset: 0x0022586C
			public void CopyItemsIntoEmpty(CosmeticsController.CosmeticSet other)
			{
				for (int i = 0; i < this.items.Length; i++)
				{
					if (this.items[i].isNullItem)
					{
						this.items[i] = other.items[i];
					}
				}
			}

			// Token: 0x06006B03 RID: 27395 RVA: 0x002276B8 File Offset: 0x002258B8
			public void MergeSets(CosmeticsController.CosmeticSet tryOn, CosmeticsController.CosmeticSet current)
			{
				for (int i = 0; i < 16; i++)
				{
					if (tryOn == null)
					{
						this.items[i] = current.items[i];
					}
					else
					{
						this.items[i] = (tryOn.items[i].isNullItem ? current.items[i] : tryOn.items[i]);
					}
				}
			}

			// Token: 0x06006B04 RID: 27396 RVA: 0x00227728 File Offset: 0x00225928
			public void MergeInSets(CosmeticsController.CosmeticSet playerPref, CosmeticsController.CosmeticSet tempOverrideSet, Predicate<string> predicate)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticItem[] array = tempOverrideSet.items;
					bool flag = predicate(tempOverrideSet.items[i].itemName);
					this.items[i] = (flag ? tempOverrideSet.items[i] : playerPref.items[i]);
				}
			}

			// Token: 0x06006B05 RID: 27397 RVA: 0x00227794 File Offset: 0x00225994
			public void ClearSet(CosmeticsController.CosmeticItem nullItem)
			{
				for (int i = 0; i < 16; i++)
				{
					this.items[i] = nullItem;
				}
			}

			// Token: 0x06006B06 RID: 27398 RVA: 0x002277BC File Offset: 0x002259BC
			public bool IsActive(string name)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].displayName == name)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06006B07 RID: 27399 RVA: 0x002277F4 File Offset: 0x002259F4
			public bool HasItemOfCategory(CosmeticsController.CosmeticCategory category)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].itemCategory == category)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06006B08 RID: 27400 RVA: 0x0022783C File Offset: 0x00225A3C
			public bool HasItem(string name)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].displayName == name)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06006B09 RID: 27401 RVA: 0x00227888 File Offset: 0x00225A88
			public bool HasAnyItems()
			{
				if (this.items == null || this.items.Length < 1)
				{
					return false;
				}
				for (int i = 0; i < this.items.Length; i++)
				{
					if (!this.items[i].isNullItem)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06006B0A RID: 27402 RVA: 0x002278D3 File Offset: 0x00225AD3
			public static bool IsSlotLeftHanded(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.HandLeft;
			}

			// Token: 0x06006B0B RID: 27403 RVA: 0x002278E3 File Offset: 0x00225AE3
			public static bool IsSlotRightHanded(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmRight || slot == CosmeticsController.CosmeticSlots.BackRight || slot == CosmeticsController.CosmeticSlots.HandRight;
			}

			// Token: 0x06006B0C RID: 27404 RVA: 0x002278F3 File Offset: 0x00225AF3
			public static bool IsHoldable(CosmeticsController.CosmeticItem item)
			{
				return item.isHoldable;
			}

			// Token: 0x06006B0D RID: 27405 RVA: 0x002278FC File Offset: 0x00225AFC
			public static CosmeticsController.CosmeticSlots OppositeSlot(CosmeticsController.CosmeticSlots slot)
			{
				switch (slot)
				{
				case CosmeticsController.CosmeticSlots.Hat:
					return CosmeticsController.CosmeticSlots.Hat;
				case CosmeticsController.CosmeticSlots.Badge:
					return CosmeticsController.CosmeticSlots.Badge;
				case CosmeticsController.CosmeticSlots.Face:
					return CosmeticsController.CosmeticSlots.Face;
				case CosmeticsController.CosmeticSlots.ArmLeft:
					return CosmeticsController.CosmeticSlots.ArmRight;
				case CosmeticsController.CosmeticSlots.ArmRight:
					return CosmeticsController.CosmeticSlots.ArmLeft;
				case CosmeticsController.CosmeticSlots.BackLeft:
					return CosmeticsController.CosmeticSlots.BackRight;
				case CosmeticsController.CosmeticSlots.BackRight:
					return CosmeticsController.CosmeticSlots.BackLeft;
				case CosmeticsController.CosmeticSlots.HandLeft:
					return CosmeticsController.CosmeticSlots.HandRight;
				case CosmeticsController.CosmeticSlots.HandRight:
					return CosmeticsController.CosmeticSlots.HandLeft;
				case CosmeticsController.CosmeticSlots.Chest:
					return CosmeticsController.CosmeticSlots.Chest;
				case CosmeticsController.CosmeticSlots.Fur:
					return CosmeticsController.CosmeticSlots.Fur;
				case CosmeticsController.CosmeticSlots.Shirt:
					return CosmeticsController.CosmeticSlots.Shirt;
				case CosmeticsController.CosmeticSlots.Pants:
					return CosmeticsController.CosmeticSlots.Pants;
				case CosmeticsController.CosmeticSlots.Back:
					return CosmeticsController.CosmeticSlots.Back;
				case CosmeticsController.CosmeticSlots.Arms:
					return CosmeticsController.CosmeticSlots.Arms;
				case CosmeticsController.CosmeticSlots.TagEffect:
					return CosmeticsController.CosmeticSlots.TagEffect;
				default:
					return CosmeticsController.CosmeticSlots.Count;
				}
			}

			// Token: 0x06006B0E RID: 27406 RVA: 0x0022797A File Offset: 0x00225B7A
			public static string SlotPlayerPreferenceName(CosmeticsController.CosmeticSlots slot)
			{
				return "slot_" + slot.ToString();
			}

			// Token: 0x06006B0F RID: 27407 RVA: 0x00227994 File Offset: 0x00225B94
			private void ActivateCosmetic(CosmeticsController.CosmeticSet prevSet, VRRig rig, int slotIndex, CosmeticItemRegistry cosmeticsObjectRegistry, BodyDockPositions bDock)
			{
				CosmeticsController.CosmeticItem cosmeticItem = prevSet.items[slotIndex];
				string itemNameFromDisplayName = CosmeticsController.instance.GetItemNameFromDisplayName(cosmeticItem.displayName);
				CosmeticsController.CosmeticItem cosmeticItem2 = this.items[slotIndex];
				string itemNameFromDisplayName2 = CosmeticsController.instance.GetItemNameFromDisplayName(cosmeticItem2.displayName);
				BodyDockPositions.DropPositions dropPositions = CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)slotIndex);
				if (cosmeticItem2.itemCategory != CosmeticsController.CosmeticCategory.None && !CosmeticsController.CompareCategoryToSavedCosmeticSlots(cosmeticItem2.itemCategory, (CosmeticsController.CosmeticSlots)slotIndex))
				{
					return;
				}
				if (cosmeticItem2.isHoldable && dropPositions == BodyDockPositions.DropPositions.None)
				{
					return;
				}
				CosmeticItemInstance cosmeticItemInstance = null;
				CosmeticItemInstance cosmeticItemInstance2 = null;
				try
				{
					if (itemNameFromDisplayName == itemNameFromDisplayName2)
					{
						if (!cosmeticItem2.isNullItem)
						{
							cosmeticItemInstance = cosmeticsObjectRegistry.Cosmetic(cosmeticItem2.displayName);
							if (cosmeticItemInstance != null)
							{
								if (!rig.IsItemAllowed(itemNameFromDisplayName2))
								{
									cosmeticItemInstance.DisableItem((CosmeticsController.CosmeticSlots)slotIndex);
								}
								else
								{
									if (cosmeticItem2.isHoldable)
									{
										bDock.TransferrableItemEnableAtPosition(cosmeticItem2.displayName, dropPositions);
									}
									cosmeticItemInstance.EnableItem((CosmeticsController.CosmeticSlots)slotIndex, rig);
									CosmeticsController.CosmeticSet.PopulateCollectionDisplay(cosmeticItemInstance, cosmeticItem2, rig);
								}
							}
						}
					}
					else
					{
						if (!cosmeticItem.isNullItem)
						{
							if (cosmeticItem.isHoldable)
							{
								bDock.TransferrableItemDisableAtPosition(dropPositions);
							}
							cosmeticItemInstance2 = cosmeticsObjectRegistry.Cosmetic(cosmeticItem.displayName);
							if (cosmeticItemInstance2 != null)
							{
								cosmeticItemInstance2.DisableItem((CosmeticsController.CosmeticSlots)slotIndex);
							}
						}
						if (!cosmeticItem2.isNullItem)
						{
							if (cosmeticItem2.isHoldable)
							{
								bDock.TransferrableItemEnableAtPosition(cosmeticItem2.displayName, dropPositions);
							}
							cosmeticItemInstance = cosmeticsObjectRegistry.Cosmetic(cosmeticItem2.displayName);
							if (rig.IsItemAllowed(itemNameFromDisplayName2) && cosmeticItemInstance != null)
							{
								cosmeticItemInstance.EnableItem((CosmeticsController.CosmeticSlots)slotIndex, rig);
								if (rig.isLocal && (slotIndex == 0 || slotIndex == 2))
								{
									PlayerPrefFlags.TouchIf(PlayerPrefFlags.Flag.SHOW_1P_COSMETICS, false);
								}
								CosmeticsController.CosmeticSet.PopulateCollectionDisplay(cosmeticItemInstance, cosmeticItem2, rig);
							}
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					try
					{
						if (cosmeticItemInstance2 != null && !cosmeticItem.isHoldable)
						{
							cosmeticItemInstance2.DisableItem((CosmeticsController.CosmeticSlots)slotIndex);
						}
						if (cosmeticItemInstance != null && !cosmeticItem2.isHoldable)
						{
							cosmeticItemInstance.DisableItem((CosmeticsController.CosmeticSlots)slotIndex);
						}
					}
					catch
					{
					}
				}
			}

			// Token: 0x06006B10 RID: 27408 RVA: 0x00227B8C File Offset: 0x00225D8C
			public void ActivateCosmetics(CosmeticsController.CosmeticSet prevSet, VRRig rig, BodyDockPositions bDock, CosmeticItemRegistry cosmeticsObjectRegistry)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					this.ActivateCosmetic(prevSet, rig, i, cosmeticsObjectRegistry, bDock);
				}
				this.OnSetActivated(prevSet, this, rig.creator);
			}

			// Token: 0x06006B11 RID: 27409 RVA: 0x00227BC4 File Offset: 0x00225DC4
			private static void PopulateCollectionDisplay(CosmeticItemInstance instance, CosmeticsController.CosmeticItem parentItem, VRRig rig)
			{
				if (parentItem.collectionSlotCount <= 0 || !CosmeticsController.hasInstance)
				{
					return;
				}
				GameObject gameObject = CosmeticsController.CosmeticSet.FindFirstPickupableVariantRoot(instance.objects) ?? CosmeticsController.CosmeticSet.FindFirstPickupableVariantRoot(instance.holdableObjects);
				if (gameObject == null)
				{
					CosmeticCollectionDisplay.GetAllForParent(rig, parentItem.itemName, CosmeticsController.scratchDisplayList);
					for (int i = 0; i < CosmeticsController.scratchDisplayList.Count; i++)
					{
						CosmeticCollectionDisplay cosmeticCollectionDisplay = CosmeticsController.scratchDisplayList[i];
						if (cosmeticCollectionDisplay != null && cosmeticCollectionDisplay.gameObject != null && cosmeticCollectionDisplay.GetComponent<PickupableVariant>() != null)
						{
							gameObject = cosmeticCollectionDisplay.gameObject;
							break;
						}
					}
				}
				if (gameObject == null)
				{
					gameObject = CosmeticsController.CosmeticSet.FindFirstNonNull(instance.objects) ?? CosmeticsController.CosmeticSet.FindFirstNonNull(instance.holdableObjects);
				}
				if (gameObject != null)
				{
					CosmeticCollectionDisplay.DestroyAllForParentExcept(rig, parentItem.itemName, gameObject);
					CosmeticsController.CosmeticSet.RemoveStaleDisplaysForParent(rig, parentItem.itemName, gameObject);
					CosmeticsController.PopulateCollectionDisplayOnRoot(gameObject, parentItem, rig);
				}
			}

			// Token: 0x06006B12 RID: 27410 RVA: 0x00227CB8 File Offset: 0x00225EB8
			private static void RemoveStaleDisplaysForParent(VRRig rig, string parentPlayFabID, GameObject host)
			{
				if (rig == null || string.IsNullOrEmpty(parentPlayFabID))
				{
					return;
				}
				foreach (CosmeticCollectionDisplay cosmeticCollectionDisplay in rig.GetComponentsInChildren<CosmeticCollectionDisplay>(true))
				{
					if (!(cosmeticCollectionDisplay == null) && !(cosmeticCollectionDisplay.gameObject == host) && !(cosmeticCollectionDisplay.ParentPlayFabID != parentPlayFabID))
					{
						Object.Destroy(cosmeticCollectionDisplay);
					}
				}
			}

			// Token: 0x06006B13 RID: 27411 RVA: 0x00227D1C File Offset: 0x00225F1C
			private static GameObject FindFirstPickupableVariantRoot(IList<GameObject> roots)
			{
				if (roots == null)
				{
					return null;
				}
				for (int i = 0; i < roots.Count; i++)
				{
					GameObject gameObject = roots[i];
					if (!(gameObject == null))
					{
						PickupableVariant componentInChildren = gameObject.GetComponentInChildren<PickupableVariant>(true);
						if (componentInChildren != null)
						{
							return componentInChildren.gameObject;
						}
					}
				}
				return null;
			}

			// Token: 0x06006B14 RID: 27412 RVA: 0x00227D6C File Offset: 0x00225F6C
			private static GameObject FindFirstNonNull(IList<GameObject> roots)
			{
				if (roots == null)
				{
					return null;
				}
				for (int i = 0; i < roots.Count; i++)
				{
					if (roots[i] != null)
					{
						return roots[i];
					}
				}
				return null;
			}

			// Token: 0x06006B15 RID: 27413 RVA: 0x00227DA8 File Offset: 0x00225FA8
			public void DeactivateAllCosmetcs(BodyDockPositions bDock, CosmeticsController.CosmeticItem nullItem, CosmeticItemRegistry cosmeticObjectRegistry)
			{
				bDock.DisableAllTransferableItems();
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticItem cosmeticItem = this.items[i];
					if (!cosmeticItem.isNullItem)
					{
						CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots)i;
						CosmeticItemInstance cosmeticItemInstance = cosmeticObjectRegistry.Cosmetic(cosmeticItem.displayName);
						if (cosmeticItemInstance != null)
						{
							cosmeticItemInstance.DisableItem(cosmeticSlots);
						}
						this.items[i] = nullItem;
					}
				}
			}

			// Token: 0x06006B16 RID: 27414 RVA: 0x00227E08 File Offset: 0x00226008
			public void LoadFromPlayerPreferences(CosmeticsController controller)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots)i;
					string @string = PlayerPrefs.GetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(cosmeticSlots), "NOTHING");
					if (@string == "null" || @string == "NOTHING")
					{
						this.items[i] = controller.nullItem;
					}
					else
					{
						CosmeticsController.CosmeticItem item = controller.GetItemFromDict(@string);
						if (item.isNullItem)
						{
							Debug.Log("LoadFromPlayerPreferences: Could not find item stored in player prefs: \"" + @string + "\"");
							this.items[i] = controller.nullItem;
						}
						else if (item.itemName == "Slingshot")
						{
							this.items[i] = controller.nullItem;
							PlayerPrefs.SetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(cosmeticSlots), "NOTHING");
						}
						else if (!CosmeticsController.CompareCategoryToSavedCosmeticSlots(item.itemCategory, cosmeticSlots))
						{
							this.items[i] = controller.nullItem;
						}
						else if (controller.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.itemName == x.itemName) >= 0)
						{
							this.items[i] = item;
						}
						else
						{
							this.items[i] = controller.nullItem;
						}
					}
				}
			}

			// Token: 0x06006B17 RID: 27415 RVA: 0x00227F64 File Offset: 0x00226164
			public void ParseSetFromString(CosmeticsController controller, string setString, out Vector3 color)
			{
				color = CosmeticsController.defaultColor;
				if (setString.IsNullOrEmpty())
				{
					this.ClearSet(controller.nullItem);
					GTDev.LogError<string>("CosmeticsController ParseSetFromString: null string", null);
					return;
				}
				int num = 16;
				CosmeticsController.OutfitData outfitData = new CosmeticsController.OutfitData();
				try
				{
					outfitData = JsonUtility.FromJson<CosmeticsController.OutfitData>(setString);
					color = outfitData.color;
				}
				catch (Exception)
				{
					char c = ',';
					if (controller.outfitSystemConfig != null)
					{
						c = controller.outfitSystemConfig.itemSeparator;
					}
					string[] array = setString.Split(c, num, StringSplitOptions.None);
					if (array == null || array.Length > num)
					{
						this.ClearSet(controller.nullItem);
						GTDev.LogError<string>(string.Format("CosmeticsController ParseSetFromString: wrong number of slots {0} {1}", array.Length, setString), null);
						return;
					}
					outfitData.Clear();
					outfitData.itemIDs = new List<string>(array);
				}
				try
				{
					for (int i = 0; i < num; i++)
					{
						CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots)i;
						string text = ((i < outfitData.itemIDs.Count) ? outfitData.itemIDs[i] : "null");
						if (text.IsNullOrEmpty() || text == "null" || text == "NOTHING")
						{
							this.items[i] = controller.nullItem;
						}
						else
						{
							CosmeticsController.CosmeticItem item = controller.GetItemFromDict(text);
							if (item.isNullItem)
							{
								GTDev.Log<string>("CosmeticsController ParseSetFromString: Could not find item stored in player prefs: \"" + text + "\"", null);
								this.items[i] = controller.nullItem;
							}
							else if (!CosmeticsController.CompareCategoryToSavedCosmeticSlots(item.itemCategory, cosmeticSlots))
							{
								this.items[i] = controller.nullItem;
							}
							else if (controller.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.itemName == x.itemName) >= 0)
							{
								this.items[i] = item;
							}
							else
							{
								this.items[i] = controller.nullItem;
							}
						}
					}
				}
				catch (Exception ex)
				{
					this.ClearSet(controller.nullItem);
					GTDev.LogError<string>("CosmeticsController: Issue parsing saved outfit string: " + ex.Message, null);
				}
			}

			// Token: 0x06006B18 RID: 27416 RVA: 0x002281C4 File Offset: 0x002263C4
			public string[] ToDisplayNameArray()
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					this.returnArray[i] = (string.IsNullOrEmpty(this.items[i].displayName) ? "null" : this.items[i].displayName);
				}
				return this.returnArray;
			}

			// Token: 0x06006B19 RID: 27417 RVA: 0x00228220 File Offset: 0x00226420
			public int[] ToPackedIDArray()
			{
				int num = 0;
				int num2 = 0;
				int num3 = 16;
				for (int i = 0; i < num3; i++)
				{
					if (!this.items[i].isNullItem && !string.IsNullOrEmpty(this.items[i].itemName) && (this.items[i].itemName.Length == 6 || this.items[i].itemName == "Slingshot"))
					{
						num |= 1 << i;
						num2++;
					}
				}
				if (num == 0)
				{
					return CosmeticsController.CosmeticSet.intArrays[0];
				}
				int[] array = CosmeticsController.CosmeticSet.intArrays[num2 + 1];
				array[0] = num;
				int num4 = 1;
				for (int j = 0; j < num3; j++)
				{
					if ((num & (1 << j)) != 0)
					{
						string itemName = this.items[j].itemName;
						if (itemName == "Slingshot")
						{
							array[num4] = -55;
						}
						else
						{
							array[num4] = (int)(itemName[0] - 'A' + '\u001a' * (itemName[1] - 'A' + '\u001a' * (itemName[2] - 'A' + '\u001a' * (itemName[3] - 'A' + '\u001a' * (itemName[4] - 'A')))));
						}
						num4++;
					}
				}
				return array;
			}

			// Token: 0x06006B1A RID: 27418 RVA: 0x00228370 File Offset: 0x00226570
			public string[] HoldableDisplayNames(bool leftHoldables)
			{
				int num = 16;
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].isHoldable && this.items[i].isHoldable && this.items[i].itemCategory != CosmeticsController.CosmeticCategory.Chest)
					{
						if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i)))
						{
							num2++;
						}
						else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i)))
						{
							num2++;
						}
					}
				}
				if (num2 == 0)
				{
					return null;
				}
				int num3 = 0;
				string[] array = new string[num2];
				for (int j = 0; j < num; j++)
				{
					if (this.items[j].isHoldable)
					{
						if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)j)))
						{
							array[num3] = this.items[j].displayName;
							num3++;
						}
						else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)j)))
						{
							array[num3] = this.items[j].displayName;
							num3++;
						}
					}
				}
				return array;
			}

			// Token: 0x06006B1B RID: 27419 RVA: 0x00228484 File Offset: 0x00226684
			public bool[] ToOnRightSideArray()
			{
				int num = 16;
				bool[] array = new bool[num];
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].isHoldable && this.items[i].itemCategory != CosmeticsController.CosmeticCategory.Chest)
					{
						array[i] = !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i));
					}
					else
					{
						array[i] = false;
					}
				}
				return array;
			}

			// Token: 0x04007A8A RID: 31370
			public const int k_fakePackedSlingshotID = -55;

			// Token: 0x04007A8B RID: 31371
			public CosmeticsController.CosmeticItem[] items;

			// Token: 0x04007A8D RID: 31373
			public string[] returnArray = new string[16];

			// Token: 0x04007A8E RID: 31374
			private static int[][] intArrays = new int[][]
			{
				new int[0],
				new int[1],
				new int[2],
				new int[3],
				new int[4],
				new int[5],
				new int[6],
				new int[7],
				new int[8],
				new int[9],
				new int[10],
				new int[11],
				new int[12],
				new int[13],
				new int[14],
				new int[15],
				new int[16],
				new int[17],
				new int[18],
				new int[19],
				new int[20],
				new int[21]
			};

			// Token: 0x04007A8F RID: 31375
			private static CosmeticsController.CosmeticSet _emptySet;

			// Token: 0x04007A90 RID: 31376
			private static char[] nameScratchSpace = new char[6];

			// Token: 0x020010AE RID: 4270
			// (Invoke) Token: 0x06006B1E RID: 27422
			public delegate void OnSetActivatedHandler(CosmeticsController.CosmeticSet prevSet, CosmeticsController.CosmeticSet currentSet, NetPlayer netPlayer);
		}

		// Token: 0x020010B1 RID: 4273
		[Serializable]
		public struct CollectionState
		{
			// Token: 0x04007A93 RID: 31379
			public int activeIndex;

			// Token: 0x04007A94 RID: 31380
			public int visibleMask;
		}

		// Token: 0x020010B2 RID: 4274
		[Serializable]
		public struct CosmeticItem
		{
			// Token: 0x17000A32 RID: 2610
			// (get) Token: 0x06006B25 RID: 27429 RVA: 0x0022861C File Offset: 0x0022681C
			public bool IsCollectable
			{
				get
				{
					CosmeticCollectionParentLink[] array = this.collectionParentLinks;
					return array != null && array.Length > 0;
				}
			}

			// Token: 0x06006B26 RID: 27430 RVA: 0x0022863C File Offset: 0x0022683C
			public bool IsCollectableOf(string parentPlayFabID)
			{
				if (this.collectionParentLinks == null)
				{
					return false;
				}
				for (int i = 0; i < this.collectionParentLinks.Length; i++)
				{
					if (this.collectionParentLinks[i].parentPlayFabID == parentPlayFabID)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06006B27 RID: 27431 RVA: 0x00228684 File Offset: 0x00226884
			public int GetTargetSlotIndexForParent(string parentPlayFabID)
			{
				if (this.collectionParentLinks != null)
				{
					for (int i = 0; i < this.collectionParentLinks.Length; i++)
					{
						if (this.collectionParentLinks[i].parentPlayFabID == parentPlayFabID)
						{
							return this.collectionParentLinks[i].targetSlotIndex;
						}
					}
				}
				return -1;
			}

			// Token: 0x06006B28 RID: 27432 RVA: 0x002286D8 File Offset: 0x002268D8
			public int GetSeriesIndexForParent(string parentPlayFabID)
			{
				if (this.collectionParentLinks != null)
				{
					for (int i = 0; i < this.collectionParentLinks.Length; i++)
					{
						if (this.collectionParentLinks[i].parentPlayFabID == parentPlayFabID)
						{
							return this.collectionParentLinks[i].seriesIndex;
						}
					}
				}
				return -1;
			}

			// Token: 0x04007A95 RID: 31381
			[Tooltip("Should match the spreadsheet item name.")]
			public string itemName;

			// Token: 0x04007A96 RID: 31382
			[Tooltip("Determines what wardrobe section the item will show up in.")]
			public CosmeticsController.CosmeticCategory itemCategory;

			// Token: 0x04007A97 RID: 31383
			[Tooltip("If this is a holdable item.")]
			public bool isHoldable;

			// Token: 0x04007A98 RID: 31384
			[Tooltip("If this is a throwable item and hidden on the wardrobe.")]
			public bool isThrowable;

			// Token: 0x04007A99 RID: 31385
			[Tooltip("Icon shown in the store menus & hunt watch.")]
			public Sprite itemPicture;

			// Token: 0x04007A9A RID: 31386
			public string displayName;

			// Token: 0x04007A9B RID: 31387
			public string itemPictureResourceString;

			// Token: 0x04007A9C RID: 31388
			[Tooltip("The name shown on the store checkout screen.")]
			public string overrideDisplayName;

			// Token: 0x04007A9D RID: 31389
			[DebugReadout]
			[NonSerialized]
			public int cost;

			// Token: 0x04007A9E RID: 31390
			[DebugReadout]
			[NonSerialized]
			public string[] bundledItems;

			// Token: 0x04007A9F RID: 31391
			[DebugReadout]
			[NonSerialized]
			public bool canTryOn;

			// Token: 0x04007AA0 RID: 31392
			[Tooltip("Set to true if the item takes up both left and right wearable hand slots at the same time. Used for things like mittens/gloves.")]
			public bool bothHandsHoldable;

			// Token: 0x04007AA1 RID: 31393
			public bool bLoadsFromResources;

			// Token: 0x04007AA2 RID: 31394
			public bool bUsesMeshAtlas;

			// Token: 0x04007AA3 RID: 31395
			public Vector3 rotationOffset;

			// Token: 0x04007AA4 RID: 31396
			public Vector3 positionOffset;

			// Token: 0x04007AA5 RID: 31397
			public string meshAtlasResourceString;

			// Token: 0x04007AA6 RID: 31398
			public string meshResourceString;

			// Token: 0x04007AA7 RID: 31399
			public string materialResourceString;

			// Token: 0x04007AA8 RID: 31400
			[HideInInspector]
			public bool isNullItem;

			// Token: 0x04007AA9 RID: 31401
			[NonSerialized]
			public CosmeticCollectionParentLink[] collectionParentLinks;

			// Token: 0x04007AAA RID: 31402
			[NonSerialized]
			public int collectionSlotCount;

			// Token: 0x04007AAB RID: 31403
			[NonSerialized]
			public bool collectionIsCycling;

			// Token: 0x04007AAC RID: 31404
			[NonSerialized]
			public bool collectionUsesIndexTargeting;

			// Token: 0x04007AAD RID: 31405
			[NonSerialized]
			public string appliedCosmeticPlayFabID;
		}

		// Token: 0x020010B3 RID: 4275
		[Serializable]
		public class IAPRequestBody
		{
			// Token: 0x04007AAE RID: 31406
			public string sku;

			// Token: 0x04007AAF RID: 31407
			public string mothershipId;

			// Token: 0x04007AB0 RID: 31408
			public string mothershipToken;

			// Token: 0x04007AB1 RID: 31409
			public string mothershipEnvId;

			// Token: 0x04007AB2 RID: 31410
			public string mothershipDeploymentId;

			// Token: 0x04007AB3 RID: 31411
			public Dictionary<string, string> customTags;
		}

		// Token: 0x020010B4 RID: 4276
		private class ValidatedCreatorCode
		{
			// Token: 0x17000A33 RID: 2611
			// (get) Token: 0x06006B2A RID: 27434 RVA: 0x0022872C File Offset: 0x0022692C
			// (set) Token: 0x06006B2B RID: 27435 RVA: 0x00228734 File Offset: 0x00226934
			public string terminalId { get; set; }

			// Token: 0x17000A34 RID: 2612
			// (get) Token: 0x06006B2C RID: 27436 RVA: 0x0022873D File Offset: 0x0022693D
			// (set) Token: 0x06006B2D RID: 27437 RVA: 0x00228745 File Offset: 0x00226945
			public string memberCode { get; set; }

			// Token: 0x17000A35 RID: 2613
			// (get) Token: 0x06006B2E RID: 27438 RVA: 0x0022874E File Offset: 0x0022694E
			// (set) Token: 0x06006B2F RID: 27439 RVA: 0x00228756 File Offset: 0x00226956
			public string groupId { get; set; }
		}

		// Token: 0x020010B5 RID: 4277
		public enum EWearingCosmeticSet
		{
			// Token: 0x04007AB8 RID: 31416
			NotASet,
			// Token: 0x04007AB9 RID: 31417
			NotWearing,
			// Token: 0x04007ABA RID: 31418
			Partial,
			// Token: 0x04007ABB RID: 31419
			Complete
		}

		// Token: 0x020010B6 RID: 4278
		public class OutfitData
		{
			// Token: 0x06006B31 RID: 27441 RVA: 0x0022875F File Offset: 0x0022695F
			public OutfitData()
			{
				this.version = 1;
				this.itemIDs = new List<string>(16);
				this.color = CosmeticsController.defaultColor;
			}

			// Token: 0x06006B32 RID: 27442 RVA: 0x00228786 File Offset: 0x00226986
			public void Clear()
			{
				this.itemIDs.Clear();
				this.color = CosmeticsController.defaultColor;
			}

			// Token: 0x04007ABC RID: 31420
			public const int OUTFIT_DATA_VERSION = 1;

			// Token: 0x04007ABD RID: 31421
			public int version;

			// Token: 0x04007ABE RID: 31422
			public List<string> itemIDs;

			// Token: 0x04007ABF RID: 31423
			public Vector3 color;
		}
	}
}
