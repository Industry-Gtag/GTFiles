using System;
using System.Collections;
using System.Threading;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GorillaNetworking.Store
{
	// Token: 0x02001143 RID: 4419
	public class DynamicCosmeticStand : MonoBehaviour, iFlagForBaking
	{
		// Token: 0x06006EE0 RID: 28384 RVA: 0x0023B3E8 File Offset: 0x002395E8
		public virtual void SetForBaking()
		{
			this.GorillaHeadModel.SetActive(true);
			this.GorillaTorsoModel.SetActive(true);
			this.GorillaTorsoPostModel.SetActive(true);
			this.GorillaMannequinModel.SetActive(true);
			this.JeweleryBoxModel.SetActive(true);
			this.root.SetActive(true);
			this.DisplayHeadModel.gameObject.SetActive(false);
		}

		// Token: 0x06006EE1 RID: 28385 RVA: 0x0023B450 File Offset: 0x00239650
		public void OnEnable()
		{
			TMP_Text tmp_Text = this.addToCartTextTMP;
			if (tmp_Text != null)
			{
				tmp_Text.gameObject.SetActive(true);
			}
			TMP_Text tmp_Text2 = this.slotPriceTextTMP;
			if (tmp_Text2 != null)
			{
				tmp_Text2.gameObject.SetActive(true);
			}
			this.AddStandToStoreController();
			if (CosmeticsController.hasInstance)
			{
				CosmeticsController instance = CosmeticsController.instance;
				instance.OnCosmeticsUpdated = (Action)Delegate.Combine(instance.OnCosmeticsUpdated, new Action(this.RefreshPurchaseGate));
			}
		}

		// Token: 0x06006EE2 RID: 28386 RVA: 0x0023B4C0 File Offset: 0x002396C0
		public void OnDisable()
		{
			TMP_Text tmp_Text = this.addToCartTextTMP;
			if (tmp_Text != null)
			{
				tmp_Text.gameObject.SetActive(false);
			}
			TMP_Text tmp_Text2 = this.slotPriceTextTMP;
			if (tmp_Text2 != null)
			{
				tmp_Text2.gameObject.SetActive(false);
			}
			this.RemoveStandFromStoreController();
			if (CosmeticsController.hasInstance)
			{
				CosmeticsController instance = CosmeticsController.instance;
				instance.OnCosmeticsUpdated = (Action)Delegate.Remove(instance.OnCosmeticsUpdated, new Action(this.RefreshPurchaseGate));
			}
		}

		// Token: 0x06006EE3 RID: 28387 RVA: 0x0023B530 File Offset: 0x00239730
		public void AddStandToStoreController()
		{
			if (StoreController.instance != null && StoreController.instance.cosmeticsInitialized)
			{
				this._AddStandToStoreController();
				return;
			}
			base.StartCoroutine(this.ConnectToStoreController());
		}

		// Token: 0x06006EE4 RID: 28388 RVA: 0x0023B563 File Offset: 0x00239763
		private IEnumerator ConnectToStoreController()
		{
			int i = 0;
			while (i < 30 && !(StoreController.instance != null))
			{
				if (i == 29)
				{
					Object.Destroy(this);
					throw new Exception("Could not connect to store controller.");
				}
				yield return null;
				int num = i + 1;
				i = num;
			}
			if (!StoreController.instance.cosmeticsInitialized)
			{
				this.AsyncAddStandToStoreController();
				yield break;
			}
			while (Application.isPlaying && (!CosmeticsController.hasInstance || !CosmeticsController.instance.v2_allCosmeticsInfoAssetRef_isLoaded))
			{
				yield return null;
			}
			this._AddStandToStoreController();
			yield break;
		}

		// Token: 0x06006EE5 RID: 28389 RVA: 0x0023B574 File Offset: 0x00239774
		public async void AsyncAddStandToStoreController()
		{
			while (!StoreController.instance.cosmeticsInitialized)
			{
				await Awaitable.NextFrameAsync(default(CancellationToken));
			}
			this._AddStandToStoreController();
		}

		// Token: 0x06006EE6 RID: 28390 RVA: 0x0023B5AB File Offset: 0x002397AB
		public void _AddStandToStoreController()
		{
			StoreController.instance.AddStandToCosmeticStandsDictionary(this);
			StoreController.instance.AddStandToPlayfabIDDictionary(this);
			if (StoreController.instance.LoadFromTitleData)
			{
				StoreController.instance.InitializeStandFromTitleData(this);
				return;
			}
			this.InitializeCosmetic();
		}

		// Token: 0x06006EE7 RID: 28391 RVA: 0x0023B5E9 File Offset: 0x002397E9
		public void RemoveStandFromStoreController()
		{
			if (StoreController.instance == null || !StoreController.instance.cosmeticsInitialized)
			{
				return;
			}
			StoreController.instance.RemoveStandFromDynamicCosmeticStandsDictionary(this);
			StoreController.instance.RemoveStandFromPlayFabIDDictionary(this);
		}

		// Token: 0x06006EE8 RID: 28392 RVA: 0x0023B623 File Offset: 0x00239823
		public virtual void SetForGame()
		{
			this.DisplayHeadModel.gameObject.SetActive(true);
			this.SetStandType(this.DisplayHeadModel.bustType);
			this.parentDisplay = base.GetComponentInParent<StoreDisplay>();
			this.parentDepartment = base.GetComponentInParent<StoreDepartment>();
		}

		// Token: 0x17000A99 RID: 2713
		// (get) Token: 0x06006EE9 RID: 28393 RVA: 0x0023B65F File Offset: 0x0023985F
		// (set) Token: 0x06006EEA RID: 28394 RVA: 0x0023B667 File Offset: 0x00239867
		public string thisCosmeticName
		{
			get
			{
				return this._thisCosmeticName;
			}
			set
			{
				this._thisCosmeticName = value;
			}
		}

		// Token: 0x06006EEB RID: 28395 RVA: 0x0023B670 File Offset: 0x00239870
		public void InitializeCosmetic()
		{
			if (CosmeticsController.instance == null || CosmeticsController.instance.allCosmetics == null)
			{
				return;
			}
			this.AssignCosmeticItem();
			this.SetSlotPriceText();
			if (this.thisCosmeticItem.cost == 0)
			{
				base.StartCoroutine(this.SetStandPriceCoroutine());
			}
			this.RefreshPurchaseGate();
		}

		// Token: 0x06006EEC RID: 28396 RVA: 0x0023B6C7 File Offset: 0x002398C7
		private IEnumerator SetStandPriceCoroutine()
		{
			float startTime = Time.time;
			yield return new WaitForSeconds(Random.Range(1f, 5f));
			this.wait = new WaitForSeconds(2.5f);
			while (this.thisCosmeticItem.cost == 0)
			{
				yield return this.wait;
				if (Time.time - startTime > 60f)
				{
					yield break;
				}
				this.AssignCosmeticItem();
			}
			this.SetSlotPriceText();
			yield break;
		}

		// Token: 0x06006EED RID: 28397 RVA: 0x0023B6D6 File Offset: 0x002398D6
		private void AssignCosmeticItem()
		{
			this.thisCosmeticItem = CosmeticsController.instance.allCosmetics.Find((CosmeticsController.CosmeticItem x) => this.thisCosmeticName == x.displayName || this.thisCosmeticName == x.overrideDisplayName || this.thisCosmeticName == x.itemName);
		}

		// Token: 0x06006EEE RID: 28398 RVA: 0x0023B6FC File Offset: 0x002398FC
		private void SetSlotPriceText()
		{
			if (this.slotPriceTextTMP != null)
			{
				this.slotPriceTextTMP.text = this.thisCosmeticItem.itemCategory.ToString().ToUpper() + " " + this.thisCosmeticItem.cost.ToString();
			}
		}

		// Token: 0x06006EEF RID: 28399 RVA: 0x0023B758 File Offset: 0x00239958
		public void RefreshPurchaseGate()
		{
			if (this.thisCosmeticItem.itemCategory != CosmeticsController.CosmeticCategory.Collectable)
			{
				return;
			}
			CosmeticsController instance = CosmeticsController.instance;
			CosmeticCollectionParentLink[] collectionParentLinks = this.thisCosmeticItem.collectionParentLinks;
			string text = null;
			string text2 = null;
			if (collectionParentLinks != null)
			{
				for (int i = 0; i < collectionParentLinks.Length; i++)
				{
					string parentPlayFabID = collectionParentLinks[i].parentPlayFabID;
					if (!string.IsNullOrEmpty(parentPlayFabID))
					{
						if (text2 == null)
						{
							text2 = parentPlayFabID;
						}
						if (instance.IsOwnedByPlayFabID(parentPlayFabID))
						{
							text = parentPlayFabID;
							break;
						}
					}
				}
			}
			if (text == null)
			{
				this.AddToCartButton.gameObject.SetActive(false);
				CosmeticsController.CosmeticItem cosmeticItem;
				string text3 = ((text2 != null && instance.allCosmeticsDict.TryGetValue(text2, out cosmeticItem)) ? cosmeticItem.overrideDisplayName : text2);
				if (this.slotPriceTextTMP != null)
				{
					this.slotPriceTextTMP.text = "REQUIRES\n" + text3;
				}
				return;
			}
			if (!instance.CanPurchaseCollectable(this.thisCosmeticItem.itemName))
			{
				this.AddToCartButton.gameObject.SetActive(false);
				if (this.slotPriceTextTMP != null)
				{
					this.slotPriceTextTMP.text = "SLOTS FULL";
				}
				return;
			}
			this.AddToCartButton.gameObject.SetActive(true);
			if (this.slotPriceTextTMP != null)
			{
				this.slotPriceTextTMP.text = "ADD-ON   " + this.thisCosmeticItem.cost.ToString();
			}
		}

		// Token: 0x06006EF0 RID: 28400 RVA: 0x0023B8B4 File Offset: 0x00239AB4
		public void SpawnItemOntoStand(string PlayFabID)
		{
			this.ClearCosmetics();
			if (PlayFabID.IsNullOrEmpty())
			{
				GTDev.LogWarning<string>("ManuallyInitialize: PlayFabID is null or empty for " + this.StandName, null);
				return;
			}
			if (StoreController.instance.IsNotNull() && Application.isPlaying)
			{
				StoreController.instance.RemoveStandFromPlayFabIDDictionary(this);
			}
			this.thisCosmeticName = PlayFabID;
			if (this.thisCosmeticName.Length == 5)
			{
				this.thisCosmeticName += ".";
			}
			if (Application.isPlaying)
			{
				this.DisplayHeadModel.LoadCosmeticPartsV2(this.thisCosmeticName, false);
			}
			else
			{
				this.DisplayHeadModel.LoadCosmeticParts(StoreController.FindCosmeticInAllCosmeticsArraySO(this.thisCosmeticName), false);
			}
			if (StoreController.instance.IsNotNull() && Application.isPlaying)
			{
				StoreController.instance.AddStandToPlayfabIDDictionary(this);
			}
		}

		// Token: 0x06006EF1 RID: 28401 RVA: 0x0023B987 File Offset: 0x00239B87
		public void ClearCosmetics()
		{
			this.thisCosmeticName = "";
			this.DisplayHeadModel.ClearManuallySpawnedCosmeticParts();
			this.DisplayHeadModel.ClearCosmetics();
		}

		// Token: 0x06006EF2 RID: 28402 RVA: 0x0023B9AC File Offset: 0x00239BAC
		public void SetStandType(HeadModel_CosmeticStand.BustType newBustType)
		{
			this.DisplayHeadModel.SetStandType(newBustType);
			this.GorillaHeadModel.SetActive(false);
			this.GorillaTorsoModel.SetActive(false);
			this.GorillaTorsoPostModel.SetActive(false);
			this.GorillaMannequinModel.SetActive(false);
			this.GuitarStandModel.SetActive(false);
			this.JeweleryBoxModel.SetActive(false);
			this.AddToCartButton.gameObject.SetActive(true);
			Text text = this.slotPriceText;
			if (text != null)
			{
				text.gameObject.SetActive(true);
			}
			TMP_Text tmp_Text = this.slotPriceTextTMP;
			if (tmp_Text != null)
			{
				tmp_Text.gameObject.SetActive(true);
			}
			Text text2 = this.addToCartText;
			if (text2 != null)
			{
				text2.gameObject.SetActive(true);
			}
			TMP_Text tmp_Text2 = this.addToCartTextTMP;
			if (tmp_Text2 != null)
			{
				tmp_Text2.gameObject.SetActive(true);
			}
			switch (newBustType)
			{
			case HeadModel_CosmeticStand.BustType.Disabled:
			{
				this.ClearCosmetics();
				this.thisCosmeticName = "";
				this.AddToCartButton.gameObject.SetActive(false);
				Text text3 = this.slotPriceText;
				if (text3 != null)
				{
					text3.gameObject.SetActive(false);
				}
				TMP_Text tmp_Text3 = this.slotPriceTextTMP;
				if (tmp_Text3 != null)
				{
					tmp_Text3.gameObject.SetActive(false);
				}
				Text text4 = this.addToCartText;
				if (text4 != null)
				{
					text4.gameObject.SetActive(false);
				}
				TMP_Text tmp_Text4 = this.addToCartTextTMP;
				if (tmp_Text4 != null)
				{
					tmp_Text4.gameObject.SetActive(false);
				}
				this.DisplayHeadModel.transform.localPosition = Vector3.zero;
				this.DisplayHeadModel.transform.localRotation = Quaternion.identity;
				this.root.SetActive(false);
				break;
			}
			case HeadModel_CosmeticStand.BustType.GorillaHead:
				this.root.SetActive(true);
				this.GorillaHeadModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaHeadModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaHeadModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GorillaTorso:
				this.root.SetActive(true);
				this.GorillaTorsoModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaTorsoModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaTorsoModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GorillaTorsoPost:
				this.root.SetActive(true);
				this.GorillaTorsoPostModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaTorsoPostModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaTorsoPostModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GorillaMannequin:
				this.root.SetActive(true);
				this.GorillaMannequinModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaMannequinModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaMannequinModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GuitarStand:
				this.root.SetActive(true);
				this.GuitarStandModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GuitarStandMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GuitarStandMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.JewelryBox:
				this.root.SetActive(true);
				this.JeweleryBoxModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.JeweleryBoxMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.JeweleryBoxMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.Table:
				this.root.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.TableMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.TableMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.PinDisplay:
				this.root.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.PinDisplayMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.PinDisplayMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
				this.root.SetActive(true);
				break;
			default:
				this.root.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = Vector3.zero;
				this.DisplayHeadModel.transform.localRotation = Quaternion.identity;
				break;
			}
			this.SpawnItemOntoStand(this.thisCosmeticName);
		}

		// Token: 0x06006EF3 RID: 28403 RVA: 0x0023BE88 File Offset: 0x0023A088
		public void CopyChildsName()
		{
			foreach (DynamicCosmeticStand dynamicCosmeticStand in base.gameObject.GetComponentsInChildren<DynamicCosmeticStand>(true))
			{
				if (dynamicCosmeticStand != this)
				{
					this.StandName = dynamicCosmeticStand.StandName;
				}
			}
		}

		// Token: 0x06006EF4 RID: 28404 RVA: 0x0023BECC File Offset: 0x0023A0CC
		public void PressCosmeticStandButton()
		{
			if (!StoreController.instance.StandsByPlayfabID.ContainsKey(this.thisCosmeticName) || CosmeticsController.instance.GetCosmeticSOFromDisplayName(this.thisCosmeticName) == null)
			{
				return;
			}
			this.searchIndex = CosmeticsController.instance.currentCart.IndexOf(this.thisCosmeticItem);
			if (this.searchIndex != -1)
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_remove, this.thisCosmeticItem);
				CosmeticsController.instance.currentCart.RemoveAt(this.searchIndex);
				foreach (DynamicCosmeticStand dynamicCosmeticStand in StoreController.instance.StandsByPlayfabID[this.thisCosmeticItem.itemName])
				{
					dynamicCosmeticStand.AddToCartButton.isOn = false;
					dynamicCosmeticStand.AddToCartButton.UpdateColor();
				}
				for (int i = 0; i < 16; i++)
				{
					if (this.thisCosmeticItem.itemName == CosmeticsController.instance.tryOnSet.items[i].itemName)
					{
						CosmeticsController.instance.tryOnSet.items[i] = CosmeticsController.instance.nullItem;
					}
				}
			}
			else
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_add, this.thisCosmeticItem);
				CosmeticsController.instance.currentCart.Insert(0, this.thisCosmeticItem);
				foreach (DynamicCosmeticStand dynamicCosmeticStand2 in StoreController.instance.StandsByPlayfabID[this.thisCosmeticName])
				{
					dynamicCosmeticStand2.AddToCartButton.isOn = true;
					dynamicCosmeticStand2.AddToCartButton.UpdateColor();
				}
				if (CosmeticsController.instance.currentCart.Count > CosmeticsController.instance.numFittingRoomButtons)
				{
					foreach (DynamicCosmeticStand dynamicCosmeticStand3 in StoreController.instance.StandsByPlayfabID[CosmeticsController.instance.currentCart[CosmeticsController.instance.numFittingRoomButtons].itemName])
					{
						dynamicCosmeticStand3.AddToCartButton.isOn = false;
						dynamicCosmeticStand3.AddToCartButton.UpdateColor();
					}
					CosmeticsController.instance.currentCart.RemoveAt(CosmeticsController.instance.numFittingRoomButtons);
				}
			}
			CosmeticsController.instance.UpdateShoppingCart();
		}

		// Token: 0x06006EF5 RID: 28405 RVA: 0x0023C188 File Offset: 0x0023A388
		public void SetStandTypeString(string bustTypeString)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(bustTypeString);
			if (num <= 1590453963U)
			{
				if (num <= 1121133049U)
				{
					if (num != 214514339U)
					{
						if (num == 1121133049U)
						{
							if (bustTypeString == "GuitarStand")
							{
								this.SetStandType(HeadModel_CosmeticStand.BustType.GuitarStand);
								return;
							}
						}
					}
					else if (bustTypeString == "GorillaHead")
					{
						this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaHead);
						return;
					}
				}
				else if (num != 1364530810U)
				{
					if (num != 1520673798U)
					{
						if (num == 1590453963U)
						{
							if (bustTypeString == "GorillaMannequin")
							{
								this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaMannequin);
								return;
							}
						}
					}
					else if (bustTypeString == "JewelryBox")
					{
						this.SetStandType(HeadModel_CosmeticStand.BustType.JewelryBox);
						return;
					}
				}
				else if (bustTypeString == "PinDisplay")
				{
					this.SetStandType(HeadModel_CosmeticStand.BustType.PinDisplay);
					return;
				}
			}
			else if (num <= 2111326094U)
			{
				if (num != 1952506660U)
				{
					if (num == 2111326094U)
					{
						if (bustTypeString == "GorillaTorsoPost")
						{
							this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaTorsoPost);
							return;
						}
					}
				}
				else if (bustTypeString == "GorillaTorso")
				{
					this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaTorso);
					return;
				}
			}
			else if (num != 3217987877U)
			{
				if (num != 3607948159U)
				{
					if (num == 3845287012U)
					{
						if (bustTypeString == "TagEffectDisplay")
						{
							this.SetStandType(HeadModel_CosmeticStand.BustType.TagEffectDisplay);
							return;
						}
					}
				}
				else if (bustTypeString == "Table")
				{
					this.SetStandType(HeadModel_CosmeticStand.BustType.Table);
					return;
				}
			}
			else if (bustTypeString == "Disabled")
			{
				this.SetStandType(HeadModel_CosmeticStand.BustType.Disabled);
				return;
			}
			this.SetStandType(HeadModel_CosmeticStand.BustType.Table);
		}

		// Token: 0x06006EF6 RID: 28406 RVA: 0x0023C336 File Offset: 0x0023A536
		public void UpdateCosmeticsMountPositions()
		{
			this.DisplayHeadModel.UpdateCosmeticsMountPositions(StoreController.FindCosmeticInAllCosmeticsArraySO(this.thisCosmeticName));
		}

		// Token: 0x06006EF7 RID: 28407 RVA: 0x0023C350 File Offset: 0x0023A550
		public void InitializeForCustomMapCosmeticItem(GTObjectPlaceholder.ECustomMapCosmeticItem cosmeticItemSlot, Scene scene)
		{
			this.StandName = "CustomMapCosmeticItemStand-" + cosmeticItemSlot.ToString();
			this.customMapScene = scene;
			this.ClearCosmetics();
			CustomMapCosmeticItem customMapCosmeticItem;
			if (CosmeticsController.instance.customMapCosmeticsData.TryGetItem(cosmeticItemSlot, out customMapCosmeticItem))
			{
				this.thisCosmeticName = customMapCosmeticItem.playFabID;
				this.SetStandType(customMapCosmeticItem.bustType);
				this.InitializeCosmetic();
			}
		}

		// Token: 0x06006EF8 RID: 28408 RVA: 0x0023C3BB File Offset: 0x0023A5BB
		public bool IsFromCustomMapScene(Scene scene)
		{
			return this.customMapScene == scene;
		}

		// Token: 0x04007EF0 RID: 32496
		public HeadModel_CosmeticStand DisplayHeadModel;

		// Token: 0x04007EF1 RID: 32497
		public GorillaPressableButton AddToCartButton;

		// Token: 0x04007EF2 RID: 32498
		[HideInInspector]
		public Text slotPriceText;

		// Token: 0x04007EF3 RID: 32499
		[HideInInspector]
		public Text addToCartText;

		// Token: 0x04007EF4 RID: 32500
		public TMP_Text slotPriceTextTMP;

		// Token: 0x04007EF5 RID: 32501
		public TMP_Text addToCartTextTMP;

		// Token: 0x04007EF6 RID: 32502
		private CosmeticsController.CosmeticItem thisCosmeticItem;

		// Token: 0x04007EF7 RID: 32503
		[FormerlySerializedAs("StandID")]
		public string StandName;

		// Token: 0x04007EF8 RID: 32504
		public string _thisCosmeticName = "";

		// Token: 0x04007EF9 RID: 32505
		public GameObject GorillaHeadModel;

		// Token: 0x04007EFA RID: 32506
		public GameObject GorillaTorsoModel;

		// Token: 0x04007EFB RID: 32507
		public GameObject GorillaTorsoPostModel;

		// Token: 0x04007EFC RID: 32508
		public GameObject GorillaMannequinModel;

		// Token: 0x04007EFD RID: 32509
		public GameObject GuitarStandModel;

		// Token: 0x04007EFE RID: 32510
		public GameObject GuitarStandMount;

		// Token: 0x04007EFF RID: 32511
		public GameObject JeweleryBoxModel;

		// Token: 0x04007F00 RID: 32512
		public GameObject JeweleryBoxMount;

		// Token: 0x04007F01 RID: 32513
		public GameObject TableMount;

		// Token: 0x04007F02 RID: 32514
		[FormerlySerializedAs("PinDisplayMounnt")]
		[FormerlySerializedAs("PinDisplayMountn")]
		public GameObject PinDisplayMount;

		// Token: 0x04007F03 RID: 32515
		public GameObject root;

		// Token: 0x04007F04 RID: 32516
		public GameObject TagEffectDisplayMount;

		// Token: 0x04007F05 RID: 32517
		public GameObject TageEffectDisplayModel;

		// Token: 0x04007F06 RID: 32518
		private Scene customMapScene;

		// Token: 0x04007F07 RID: 32519
		[HideInInspector]
		public StoreDisplay parentDisplay;

		// Token: 0x04007F08 RID: 32520
		[HideInInspector]
		public StoreDepartment parentDepartment;

		// Token: 0x04007F09 RID: 32521
		private int searchIndex;

		// Token: 0x04007F0A RID: 32522
		private WaitForSeconds wait;
	}
}
