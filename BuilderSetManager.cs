using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000660 RID: 1632
public class BuilderSetManager : MonoBehaviour
{
	// Token: 0x1700040F RID: 1039
	// (get) Token: 0x060028B6 RID: 10422 RVA: 0x000DC6EF File Offset: 0x000DA8EF
	internal List<BuilderPieceSet> StartPieceSets
	{
		get
		{
			return this._starterPieceSets;
		}
	}

	// Token: 0x17000410 RID: 1040
	// (get) Token: 0x060028B7 RID: 10423 RVA: 0x000DC6F7 File Offset: 0x000DA8F7
	// (set) Token: 0x060028B8 RID: 10424 RVA: 0x000DC6FE File Offset: 0x000DA8FE
	public static bool hasInstance { get; private set; }

	// Token: 0x060028B9 RID: 10425 RVA: 0x000DC708 File Offset: 0x000DA908
	public string GetStarterSetsConcat()
	{
		if (BuilderSetManager.concatStarterSets.Length > 0)
		{
			return BuilderSetManager.concatStarterSets;
		}
		BuilderSetManager.concatStarterSets = string.Empty;
		foreach (BuilderPieceSet builderPieceSet in this._starterPieceSets)
		{
			BuilderSetManager.concatStarterSets += builderPieceSet.playfabID;
		}
		return BuilderSetManager.concatStarterSets;
	}

	// Token: 0x060028BA RID: 10426 RVA: 0x000DC78C File Offset: 0x000DA98C
	public string GetAllSetsConcat()
	{
		if (BuilderSetManager.concatAllSets.Length > 0)
		{
			return BuilderSetManager.concatAllSets;
		}
		BuilderSetManager.concatAllSets = string.Empty;
		foreach (BuilderPieceSet builderPieceSet in this._allPieceSets)
		{
			BuilderSetManager.concatAllSets += builderPieceSet.playfabID;
		}
		return BuilderSetManager.concatAllSets;
	}

	// Token: 0x060028BB RID: 10427 RVA: 0x000DC810 File Offset: 0x000DAA10
	public void Awake()
	{
		if (BuilderSetManager.instance == null)
		{
			BuilderSetManager.instance = this;
			BuilderSetManager.hasInstance = true;
		}
		else if (BuilderSetManager.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		this.Init();
		if (this.monitor == null)
		{
			this.monitor = base.StartCoroutine(this.MonitorTime());
		}
	}

	// Token: 0x060028BC RID: 10428 RVA: 0x000DC878 File Offset: 0x000DAA78
	private void Init()
	{
		this.InitPieceDictionary();
		this.catalog = "DLC";
		this.currencyName = "SR";
		this.pulledStoreItems = false;
		BuilderSetManager._setIdToStoreItem = new Dictionary<int, BuilderSetManager.BuilderSetStoreItem>(this._allPieceSets.Count);
		BuilderSetManager._setIdToStoreItem.Clear();
		BuilderSetManager.pieceSetInfos = new List<BuilderSetManager.BuilderPieceSetInfo>(this._allPieceSets.Count * 45);
		BuilderSetManager.pieceSetInfoMap = new Dictionary<int, int>(this._allPieceSets.Count * 45);
		this.livePieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		this.scheduledPieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		this.displayGroups = new List<BuilderPieceSet.BuilderDisplayGroup>(this._allPieceSets.Count * 2);
		this.displayGroupMap = new Dictionary<int, int>(this._allPieceSets.Count * 2);
		this.liveDisplayGroups = new List<BuilderPieceSet.BuilderDisplayGroup>();
		Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
		foreach (BuilderPieceSet builderPieceSet in this._allPieceSets)
		{
			dictionary.Clear();
			int num = 0;
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem = new BuilderSetManager.BuilderSetStoreItem
			{
				displayName = builderPieceSet.SetName,
				playfabID = builderPieceSet.playfabID,
				setID = builderPieceSet.GetIntIdentifier(),
				cost = 0U,
				setRef = builderPieceSet,
				displayModel = builderPieceSet.displayModel,
				isNullItem = false
			};
			BuilderSetManager._setIdToStoreItem.TryAdd(builderPieceSet.GetIntIdentifier(), builderSetStoreItem);
			int num2 = -1;
			if (!string.IsNullOrEmpty(builderPieceSet.materialId))
			{
				num2 = builderPieceSet.materialId.GetHashCode();
			}
			for (int i = 0; i < builderPieceSet.subsets.Count; i++)
			{
				BuilderPieceSet.BuilderPieceSubset builderPieceSubset = builderPieceSet.subsets[i];
				if (!builderPieceSet.setName.Equals("HIDDEN"))
				{
					string text = builderPieceSet.subsets[i].GetShelfButtonName();
					if (text.IsNullOrEmpty())
					{
						text = builderPieceSet.setName;
					}
					text = text.ToUpper();
					int num3;
					if (dictionary.TryGetValue(text, out num3))
					{
						int num4;
						this.displayGroupMap.TryGetValue(num3, out num4);
						BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup = this.displayGroups[num4];
						builderDisplayGroup.pieceSubsets.Add(builderPieceSet.subsets[i]);
						this.displayGroups[num4] = builderDisplayGroup;
					}
					else
					{
						string groupUniqueID = this.GetGroupUniqueID(builderPieceSet.playfabID, num);
						num++;
						BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup2 = new BuilderPieceSet.BuilderDisplayGroup(text, builderPieceSet.materialId, builderPieceSet.GetIntIdentifier(), groupUniqueID);
						builderDisplayGroup2.pieceSubsets.Add(builderPieceSet.subsets[i]);
						dictionary.Add(text, builderDisplayGroup2.GetDisplayGroupIdentifier());
						this.displayGroupMap.Add(builderDisplayGroup2.GetDisplayGroupIdentifier(), this.displayGroups.Count);
						this.displayGroups.Add(builderDisplayGroup2);
						if (!builderPieceSet.isScheduled)
						{
							this.liveDisplayGroups.Add(builderDisplayGroup2);
						}
					}
				}
				for (int j = 0; j < builderPieceSubset.pieceInfos.Count; j++)
				{
					BuilderPiece piecePrefab = builderPieceSubset.pieceInfos[j].piecePrefab;
					piecePrefab == null;
					int staticHash = piecePrefab.name.GetStaticHash();
					int num5 = num2;
					if (piecePrefab.materialOptions == null)
					{
						num5 = -1;
						this.AddPieceToInfoMap(staticHash, num5, builderPieceSet.GetIntIdentifier());
					}
					else if (builderPieceSubset.pieceInfos[j].overrideSetMaterial)
					{
						if (builderPieceSubset.pieceInfos[j].pieceMaterialTypes.Length == 0)
						{
							Debug.LogErrorFormat("Material List for piece {0} in set {1} is empty", new object[] { piecePrefab.name, builderPieceSet.SetName });
						}
						foreach (string text2 in builderPieceSubset.pieceInfos[j].pieceMaterialTypes)
						{
							if (string.IsNullOrEmpty(text2))
							{
								Debug.LogErrorFormat("Material List Entry for piece {0} in set {1} is empty", new object[] { piecePrefab.name, builderPieceSet.SetName });
							}
							else
							{
								num5 = text2.GetHashCode();
								this.AddPieceToInfoMap(staticHash, num5, builderPieceSet.GetIntIdentifier());
							}
						}
					}
					else
					{
						Material material;
						int num6;
						piecePrefab.materialOptions.GetMaterialFromType(num2, out material, out num6);
						if (material == null)
						{
							num5 = -1;
						}
						this.AddPieceToInfoMap(staticHash, num5, builderPieceSet.GetIntIdentifier());
					}
				}
			}
			if (!builderPieceSet.isScheduled)
			{
				this.livePieceSets.Add(builderPieceSet);
			}
			else
			{
				this.scheduledPieceSets.Add(builderPieceSet);
			}
		}
		this._unlockedPieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		this._unlockedPieceSets.AddRange(this._starterPieceSets);
	}

	// Token: 0x060028BD RID: 10429 RVA: 0x000DCD5C File Offset: 0x000DAF5C
	private string GetGroupUniqueID(string setPlayfabID, int groupNumber)
	{
		return setPlayfabID.Trim('.') + ((char)(65 + groupNumber)).ToString();
	}

	// Token: 0x060028BE RID: 10430 RVA: 0x000DCD84 File Offset: 0x000DAF84
	public void InitPieceDictionary()
	{
		if (this.hasPieceDictionary)
		{
			return;
		}
		BuilderSetManager.pieceTypes = new List<int>(256);
		BuilderSetManager.pieceList = new List<BuilderPiece>(256);
		BuilderSetManager.pieceTypeToIndex = new Dictionary<int, int>(256);
		int num = 0;
		for (int i = 0; i < this._allPieceSets.Count; i++)
		{
			BuilderPieceSet builderPieceSet = this._allPieceSets[i];
			if (!(builderPieceSet == null))
			{
				for (int j = 0; j < builderPieceSet.subsets.Count; j++)
				{
					BuilderPieceSet.BuilderPieceSubset builderPieceSubset = builderPieceSet.subsets[j];
					if (!(builderPieceSet == null))
					{
						for (int k = 0; k < builderPieceSubset.pieceInfos.Count; k++)
						{
							BuilderPieceSet.PieceInfo pieceInfo = builderPieceSubset.pieceInfos[k];
							if (!(pieceInfo.piecePrefab == null))
							{
								int staticHash = pieceInfo.piecePrefab.name.GetStaticHash();
								if (!BuilderSetManager.pieceTypeToIndex.ContainsKey(staticHash))
								{
									BuilderSetManager.pieceList.Add(pieceInfo.piecePrefab);
									BuilderSetManager.pieceTypes.Add(staticHash);
									BuilderSetManager.pieceTypeToIndex.Add(staticHash, num);
									num++;
								}
							}
						}
					}
				}
			}
		}
		this.hasPieceDictionary = true;
	}

	// Token: 0x060028BF RID: 10431 RVA: 0x000DCEC8 File Offset: 0x000DB0C8
	public BuilderPiece GetPiecePrefab(int pieceType)
	{
		int num;
		if (BuilderSetManager.pieceTypeToIndex.TryGetValue(pieceType, out num))
		{
			return BuilderSetManager.pieceList[num];
		}
		Debug.LogErrorFormat("No Prefab found for type {0}", new object[] { pieceType });
		return null;
	}

	// Token: 0x060028C0 RID: 10432 RVA: 0x000DCF0A File Offset: 0x000DB10A
	private void OnEnable()
	{
		if (this.monitor == null && this.scheduledPieceSets.Count > 0)
		{
			this.monitor = base.StartCoroutine(this.MonitorTime());
		}
	}

	// Token: 0x060028C1 RID: 10433 RVA: 0x000DCF34 File Offset: 0x000DB134
	private void OnDisable()
	{
		if (this.monitor != null)
		{
			base.StopCoroutine(this.monitor);
		}
		this.monitor = null;
	}

	// Token: 0x060028C2 RID: 10434 RVA: 0x000DCF51 File Offset: 0x000DB151
	private IEnumerator MonitorTime()
	{
		while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
		{
			yield return null;
		}
		while (this.scheduledPieceSets.Count > 0)
		{
			bool flag = false;
			for (int i = this.scheduledPieceSets.Count - 1; i >= 0; i--)
			{
				BuilderPieceSet builderPieceSet = this.scheduledPieceSets[i];
				if (GorillaComputer.instance.GetServerTime() > builderPieceSet.GetScheduleDateTime())
				{
					flag = true;
					this.livePieceSets.Add(builderPieceSet);
					this.scheduledPieceSets.RemoveAt(i);
					int intIdentifier = builderPieceSet.GetIntIdentifier();
					foreach (BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup in this.displayGroups)
					{
						if (builderDisplayGroup != null && builderDisplayGroup.setID == intIdentifier && !this.liveDisplayGroups.Contains(builderDisplayGroup))
						{
							this.liveDisplayGroups.Add(builderDisplayGroup);
						}
					}
				}
			}
			if (flag)
			{
				this.OnLiveSetsUpdated.Invoke();
			}
			yield return new WaitForSecondsRealtime(60f);
		}
		this.monitor = null;
		yield break;
	}

	// Token: 0x060028C3 RID: 10435 RVA: 0x000DCF60 File Offset: 0x000DB160
	private void AddPieceToInfoMap(int pieceType, int pieceMaterial, int setID)
	{
		int num;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, pieceMaterial), out num))
		{
			BuilderSetManager.BuilderPieceSetInfo builderPieceSetInfo = BuilderSetManager.pieceSetInfos[num];
			if (!builderPieceSetInfo.setIds.Contains(setID))
			{
				builderPieceSetInfo.setIds.Add(setID);
			}
			BuilderSetManager.pieceSetInfos[num] = builderPieceSetInfo;
			return;
		}
		BuilderSetManager.BuilderPieceSetInfo builderPieceSetInfo2 = new BuilderSetManager.BuilderPieceSetInfo
		{
			pieceType = pieceType,
			materialType = pieceMaterial,
			setIds = new List<int> { setID }
		};
		BuilderSetManager.pieceSetInfoMap.Add(HashCode.Combine<int, int>(pieceType, pieceMaterial), BuilderSetManager.pieceSetInfos.Count);
		BuilderSetManager.pieceSetInfos.Add(builderPieceSetInfo2);
	}

	// Token: 0x060028C4 RID: 10436 RVA: 0x000DD008 File Offset: 0x000DB208
	public static bool IsItemIDBuilderItem(string playfabID)
	{
		return BuilderSetManager.instance.GetAllSetsConcat().Contains(playfabID);
	}

	// Token: 0x060028C5 RID: 10437 RVA: 0x000DD01C File Offset: 0x000DB21C
	public void OnGotInventoryItems(GetUserInventoryResult inventoryResult, GetCatalogItemsResult catalogResult)
	{
		CosmeticsController cosmeticsController = CosmeticsController.instance;
		cosmeticsController.concatStringCosmeticsAllowed += this.GetStarterSetsConcat();
		this._unlockedPieceSets.Clear();
		this._unlockedPieceSets.AddRange(this._starterPieceSets);
		foreach (CatalogItem catalogItem in catalogResult.Catalog)
		{
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem;
			if (BuilderSetManager.IsItemIDBuilderItem(catalogItem.ItemId) && BuilderSetManager._setIdToStoreItem.TryGetValue(catalogItem.ItemId.GetStaticHash(), out builderSetStoreItem))
			{
				bool flag = false;
				uint num = 0U;
				if (catalogItem.VirtualCurrencyPrices.TryGetValue(this.currencyName, out num))
				{
					flag = true;
				}
				builderSetStoreItem.playfabID = catalogItem.ItemId;
				builderSetStoreItem.cost = num;
				builderSetStoreItem.hasPrice = flag;
				BuilderSetManager._setIdToStoreItem[builderSetStoreItem.setRef.GetIntIdentifier()] = builderSetStoreItem;
			}
		}
		foreach (ItemInstance itemInstance in inventoryResult.Inventory)
		{
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem2;
			if (BuilderSetManager.IsItemIDBuilderItem(itemInstance.ItemId) && BuilderSetManager._setIdToStoreItem.TryGetValue(itemInstance.ItemId.GetStaticHash(), out builderSetStoreItem2))
			{
				this._unlockedPieceSets.Add(builderSetStoreItem2.setRef);
				CosmeticsController cosmeticsController2 = CosmeticsController.instance;
				cosmeticsController2.concatStringCosmeticsAllowed += itemInstance.ItemId;
			}
		}
		this.pulledStoreItems = true;
		UnityEvent onOwnedSetsUpdated = this.OnOwnedSetsUpdated;
		if (onOwnedSetsUpdated == null)
		{
			return;
		}
		onOwnedSetsUpdated.Invoke();
	}

	// Token: 0x060028C6 RID: 10438 RVA: 0x000DD1C8 File Offset: 0x000DB3C8
	public BuilderSetManager.BuilderSetStoreItem GetStoreItemFromSetID(int setID)
	{
		return BuilderSetManager._setIdToStoreItem.GetValueOrDefault(setID, BuilderKiosk.nullItem);
	}

	// Token: 0x060028C7 RID: 10439 RVA: 0x000DD1DC File Offset: 0x000DB3DC
	public BuilderPieceSet GetPieceSetFromID(int setID)
	{
		BuilderSetManager.BuilderSetStoreItem builderSetStoreItem;
		if (BuilderSetManager._setIdToStoreItem.TryGetValue(setID, out builderSetStoreItem))
		{
			return builderSetStoreItem.setRef;
		}
		return null;
	}

	// Token: 0x060028C8 RID: 10440 RVA: 0x000DD200 File Offset: 0x000DB400
	public BuilderPieceSet.BuilderDisplayGroup GetDisplayGroupFromIndex(int groupID)
	{
		int num;
		if (this.displayGroupMap.TryGetValue(groupID, out num))
		{
			return this.displayGroups[num];
		}
		return null;
	}

	// Token: 0x060028C9 RID: 10441 RVA: 0x000DD22B File Offset: 0x000DB42B
	public List<BuilderPieceSet> GetAllPieceSets()
	{
		return this._allPieceSets;
	}

	// Token: 0x060028CA RID: 10442 RVA: 0x000DD233 File Offset: 0x000DB433
	public List<BuilderPieceSet> GetLivePieceSets()
	{
		return this.livePieceSets;
	}

	// Token: 0x060028CB RID: 10443 RVA: 0x000DD23B File Offset: 0x000DB43B
	public List<BuilderPieceSet.BuilderDisplayGroup> GetLiveDisplayGroups()
	{
		return this.liveDisplayGroups;
	}

	// Token: 0x060028CC RID: 10444 RVA: 0x000DD243 File Offset: 0x000DB443
	public List<BuilderPieceSet> GetUnlockedPieceSets()
	{
		return this._unlockedPieceSets;
	}

	// Token: 0x060028CD RID: 10445 RVA: 0x000DD24B File Offset: 0x000DB44B
	public List<BuilderPieceSet> GetPermanentSetsForSale()
	{
		return this._setsAlwaysForSale;
	}

	// Token: 0x060028CE RID: 10446 RVA: 0x000DD253 File Offset: 0x000DB453
	public List<BuilderPieceSet> GetSeasonalSetsForSale()
	{
		return this._seasonalSetsForSale;
	}

	// Token: 0x060028CF RID: 10447 RVA: 0x000DD25C File Offset: 0x000DB45C
	public bool IsSetSeasonal(string playfabID)
	{
		return !this._seasonalSetsForSale.IsNullOrEmpty<BuilderPieceSet>() && this._seasonalSetsForSale.FindIndex((BuilderPieceSet x) => x.playfabID.Equals(playfabID)) >= 0;
	}

	// Token: 0x060028D0 RID: 10448 RVA: 0x000DD2A4 File Offset: 0x000DB4A4
	public bool DoesPlayerOwnDisplayGroup(Player player, int groupID)
	{
		if (player == null)
		{
			return false;
		}
		int num;
		if (!this.displayGroupMap.TryGetValue(groupID, out num))
		{
			return false;
		}
		if (num < 0 || num >= this.displayGroups.Count)
		{
			return false;
		}
		BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup = this.displayGroups[num];
		return builderDisplayGroup != null && this.DoesPlayerOwnPieceSet(player, builderDisplayGroup.setID);
	}

	// Token: 0x060028D1 RID: 10449 RVA: 0x000DD2FC File Offset: 0x000DB4FC
	public bool DoesPlayerOwnPieceSet(Player player, int setID)
	{
		BuilderPieceSet pieceSetFromID = this.GetPieceSetFromID(setID);
		if (pieceSetFromID == null)
		{
			return false;
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			bool flag = rigContainer.Rig.IsItemAllowed(pieceSetFromID.playfabID);
			Debug.LogFormat("BuilderSetManager: does player {0} own set {1} {2}", new object[] { player.ActorNumber, pieceSetFromID.SetName, flag });
			return flag;
		}
		Debug.LogFormat("BuilderSetManager: could not get rig for player {0}", new object[] { player.ActorNumber });
		return false;
	}

	// Token: 0x060028D2 RID: 10450 RVA: 0x000DD390 File Offset: 0x000DB590
	public bool DoesAnyPlayerInRoomOwnPieceSet(int setID)
	{
		BuilderPieceSet pieceSetFromID = this.GetPieceSetFromID(setID);
		if (pieceSetFromID == null)
		{
			return false;
		}
		if (this.GetStarterSetsConcat().Contains(pieceSetFromID.setName))
		{
			return true;
		}
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer) && rigContainer.Rig.IsItemAllowed(pieceSetFromID.playfabID))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060028D3 RID: 10451 RVA: 0x000DD430 File Offset: 0x000DB630
	public bool IsPieceOwnedByRoom(int pieceType, int materialType)
	{
		int num;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, materialType), out num))
		{
			foreach (int num2 in BuilderSetManager.pieceSetInfos[num].setIds)
			{
				if (this.DoesAnyPlayerInRoomOwnPieceSet(num2))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x060028D4 RID: 10452 RVA: 0x000DD4B0 File Offset: 0x000DB6B0
	public bool IsPieceOwnedLocally(int pieceType, int materialType)
	{
		int num;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, materialType), out num))
		{
			foreach (int num2 in BuilderSetManager.pieceSetInfos[num].setIds)
			{
				if (this.IsPieceSetOwnedLocally(num2))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x060028D5 RID: 10453 RVA: 0x000DD530 File Offset: 0x000DB730
	public bool IsPieceSetOwnedLocally(int setID)
	{
		return this._unlockedPieceSets.FindIndex((BuilderPieceSet x) => setID == x.GetIntIdentifier()) >= 0;
	}

	// Token: 0x060028D6 RID: 10454 RVA: 0x000DD568 File Offset: 0x000DB768
	public void UnlockSet(int setID)
	{
		int num = this._allPieceSets.FindIndex((BuilderPieceSet x) => setID == x.GetIntIdentifier());
		if (num >= 0 && !this._unlockedPieceSets.Contains(this._allPieceSets[num]))
		{
			this._unlockedPieceSets.Add(this._allPieceSets[num]);
		}
		UnityEvent onOwnedSetsUpdated = this.OnOwnedSetsUpdated;
		if (onOwnedSetsUpdated == null)
		{
			return;
		}
		onOwnedSetsUpdated.Invoke();
	}

	// Token: 0x060028D7 RID: 10455 RVA: 0x000DD5E0 File Offset: 0x000DB7E0
	public void TryPurchaseItem(int setID, Action<bool> resultCallback)
	{
		BuilderSetManager.BuilderSetStoreItem storeItem;
		if (!BuilderSetManager._setIdToStoreItem.TryGetValue(setID, out storeItem))
		{
			Debug.Log("BuilderSetManager: no store Item for set " + setID.ToString());
			Action<bool> resultCallback2 = resultCallback;
			if (resultCallback2 == null)
			{
				return;
			}
			resultCallback2(false);
			return;
		}
		else
		{
			if (!this.IsPieceSetOwnedLocally(setID))
			{
				PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
				{
					ItemId = storeItem.playfabID,
					Price = (int)storeItem.cost,
					VirtualCurrency = this.currencyName,
					CatalogVersion = this.catalog
				}, delegate(PurchaseItemResult result)
				{
					if (result.Items.Count > 0)
					{
						foreach (ItemInstance itemInstance in result.Items)
						{
							Debug.Log("BuilderSetManager: unlocking set " + itemInstance.ItemId);
							this.UnlockSet(itemInstance.ItemId.GetStaticHash());
						}
						CosmeticsController.instance.UpdateMyCosmetics();
						if (PhotonNetwork.InRoom)
						{
							this.StartCoroutine(this.CheckIfMyCosmeticsUpdated(storeItem.playfabID));
						}
						Action<bool> resultCallback4 = resultCallback;
						if (resultCallback4 == null)
						{
							return;
						}
						resultCallback4(true);
						return;
					}
					else
					{
						Debug.Log("BuilderSetManager: no items purchased ");
						Action<bool> resultCallback5 = resultCallback;
						if (resultCallback5 == null)
						{
							return;
						}
						resultCallback5(false);
						return;
					}
				}, delegate(PlayFabError error)
				{
					Debug.LogErrorFormat("BuilderSetManager: purchase {0} Error {1}", new object[] { setID, error.ErrorMessage });
					Action<bool> resultCallback6 = resultCallback;
					if (resultCallback6 == null)
					{
						return;
					}
					resultCallback6(false);
				}, null, null);
				return;
			}
			Debug.Log("BuilderSetManager: set already owned " + setID.ToString());
			Action<bool> resultCallback3 = resultCallback;
			if (resultCallback3 == null)
			{
				return;
			}
			resultCallback3(false);
			return;
		}
	}

	// Token: 0x060028D8 RID: 10456 RVA: 0x000DD6E4 File Offset: 0x000DB8E4
	private IEnumerator CheckIfMyCosmeticsUpdated(string itemToBuyID)
	{
		yield return new WaitForSecondsRealtime(1f);
		this.foundCosmetic = false;
		this.attempts = 0;
		while (!this.foundCosmetic && this.attempts < 10 && PhotonNetwork.InRoom)
		{
			PlayFabClientAPI.GetSharedGroupData(new global::PlayFab.ClientModels.GetSharedGroupDataRequest
			{
				Keys = new List<string> { "Inventory" },
				SharedGroupId = PhotonNetwork.LocalPlayer.UserId + "Inventory"
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
				bool flag = this.foundCosmetic;
			}, delegate(PlayFabError error)
			{
				this.attempts++;
				CosmeticsController.instance.ReauthOrBan(error);
			}, null, null);
			yield return new WaitForSecondsRealtime(1f);
		}
		Debug.Log("BuilderSetManager: done!");
		yield break;
	}

	// Token: 0x0400351A RID: 13594
	private const string preLog = "[GT/MonkeBlocks/BuilderSetManager]  ";

	// Token: 0x0400351B RID: 13595
	private const string preErr = "[GT/MonkeBlocks/BuilderSetManager]  ERROR!!!  ";

	// Token: 0x0400351C RID: 13596
	private const string preErrBeta = "[GT/MonkeBlocks/BuilderSetManager]  ERROR!!!  (beta only log)  ";

	// Token: 0x0400351D RID: 13597
	[SerializeField]
	private List<BuilderPieceSet> _allPieceSets;

	// Token: 0x0400351E RID: 13598
	[SerializeField]
	private List<BuilderPieceSet> _starterPieceSets;

	// Token: 0x0400351F RID: 13599
	[SerializeField]
	private List<BuilderPieceSet> _setsAlwaysForSale;

	// Token: 0x04003520 RID: 13600
	[SerializeField]
	private List<BuilderPieceSet> _seasonalSetsForSale;

	// Token: 0x04003521 RID: 13601
	private List<BuilderPieceSet> livePieceSets;

	// Token: 0x04003522 RID: 13602
	private List<BuilderPieceSet> scheduledPieceSets;

	// Token: 0x04003523 RID: 13603
	private List<BuilderPieceSet.BuilderDisplayGroup> liveDisplayGroups;

	// Token: 0x04003524 RID: 13604
	private Coroutine monitor;

	// Token: 0x04003525 RID: 13605
	private List<BuilderSetManager.BuilderSetStoreItem> _allStoreItems;

	// Token: 0x04003526 RID: 13606
	private List<BuilderPieceSet> _unlockedPieceSets;

	// Token: 0x04003527 RID: 13607
	private static Dictionary<int, BuilderSetManager.BuilderSetStoreItem> _setIdToStoreItem;

	// Token: 0x04003528 RID: 13608
	private static List<BuilderSetManager.BuilderPieceSetInfo> pieceSetInfos;

	// Token: 0x04003529 RID: 13609
	private static Dictionary<int, int> pieceSetInfoMap;

	// Token: 0x0400352A RID: 13610
	private List<BuilderPieceSet.BuilderDisplayGroup> displayGroups;

	// Token: 0x0400352B RID: 13611
	private Dictionary<int, int> displayGroupMap;

	// Token: 0x0400352C RID: 13612
	[OnEnterPlay_SetNull]
	public static volatile BuilderSetManager instance;

	// Token: 0x0400352E RID: 13614
	[HideInInspector]
	public string catalog;

	// Token: 0x0400352F RID: 13615
	[HideInInspector]
	public string currencyName;

	// Token: 0x04003530 RID: 13616
	private string[] tempStringArray;

	// Token: 0x04003531 RID: 13617
	[HideInInspector]
	public UnityEvent OnLiveSetsUpdated;

	// Token: 0x04003532 RID: 13618
	[HideInInspector]
	public UnityEvent OnOwnedSetsUpdated;

	// Token: 0x04003533 RID: 13619
	[HideInInspector]
	public bool pulledStoreItems;

	// Token: 0x04003534 RID: 13620
	[OnEnterPlay_Set("")]
	private static string concatStarterSets = string.Empty;

	// Token: 0x04003535 RID: 13621
	[OnEnterPlay_Set("")]
	private static string concatAllSets = string.Empty;

	// Token: 0x04003536 RID: 13622
	private bool foundCosmetic;

	// Token: 0x04003537 RID: 13623
	private int attempts;

	// Token: 0x04003538 RID: 13624
	private static List<int> pieceTypes;

	// Token: 0x04003539 RID: 13625
	[HideInInspector]
	public static List<BuilderPiece> pieceList;

	// Token: 0x0400353A RID: 13626
	private static Dictionary<int, int> pieceTypeToIndex;

	// Token: 0x0400353B RID: 13627
	private bool hasPieceDictionary;

	// Token: 0x02000661 RID: 1633
	[Serializable]
	public struct BuilderSetStoreItem
	{
		// Token: 0x0400353C RID: 13628
		public string displayName;

		// Token: 0x0400353D RID: 13629
		public string playfabID;

		// Token: 0x0400353E RID: 13630
		public int setID;

		// Token: 0x0400353F RID: 13631
		public uint cost;

		// Token: 0x04003540 RID: 13632
		public bool hasPrice;

		// Token: 0x04003541 RID: 13633
		public BuilderPieceSet setRef;

		// Token: 0x04003542 RID: 13634
		public GameObject displayModel;

		// Token: 0x04003543 RID: 13635
		[NonSerialized]
		public bool isNullItem;
	}

	// Token: 0x02000662 RID: 1634
	[Serializable]
	public struct BuilderPieceSetInfo
	{
		// Token: 0x04003544 RID: 13636
		public int pieceType;

		// Token: 0x04003545 RID: 13637
		public int materialType;

		// Token: 0x04003546 RID: 13638
		public List<int> setIds;
	}
}
