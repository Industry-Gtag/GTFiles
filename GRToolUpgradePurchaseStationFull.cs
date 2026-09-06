using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

// Token: 0x02000827 RID: 2087
public class GRToolUpgradePurchaseStationFull : MonoBehaviour, ITickSystemTick
{
	// Token: 0x170004BC RID: 1212
	// (get) Token: 0x06003584 RID: 13700 RVA: 0x00126608 File Offset: 0x00124808
	public int SelectedShelf
	{
		get
		{
			return this.selectedShelf;
		}
	}

	// Token: 0x170004BD RID: 1213
	// (get) Token: 0x06003585 RID: 13701 RVA: 0x00126610 File Offset: 0x00124810
	public int SelectedItem
	{
		get
		{
			return this.selectedItem;
		}
	}

	// Token: 0x170004BE RID: 1214
	// (get) Token: 0x06003586 RID: 13702 RVA: 0x00126618 File Offset: 0x00124818
	// (set) Token: 0x06003587 RID: 13703 RVA: 0x00126620 File Offset: 0x00124820
	public bool TickRunning { get; set; }

	// Token: 0x06003588 RID: 13704 RVA: 0x0001A297 File Offset: 0x00018497
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06003589 RID: 13705 RVA: 0x0001A29F File Offset: 0x0001849F
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x0600358A RID: 13706 RVA: 0x0012662C File Offset: 0x0012482C
	public void Init(GRToolProgressionManager progression, GhostReactor reactor)
	{
		this.reactor = reactor;
		this.grManager = reactor.grManager;
		this.toolProgressionManager = progression;
		this.toolProgressionManager.OnProgressionUpdated += this.ProgressionUpdated;
		this.nextVisibleShelfIndex = -1;
		this.prefabMagnetHeightOffset = this.ropeTop.position.y;
		this.frontBackShelfMovement = new GRSpringMovement(0.5f, 0.7f);
		this.raiseLowerShelfMovement = new GRSpringMovement(1f, 0.7f);
		this.magnetMovement = new GRSpringMovement(1f, 0.7f);
		ProgressionManager.Instance.OnGetShiftCredit += this.OnShiftCreditChanged;
		this.needsUIRefresh = true;
		this.InitPageSelectionWheel();
		this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
		this.SetActivePlayer(-1);
	}

	// Token: 0x0600358B RID: 13707 RVA: 0x001266F7 File Offset: 0x001248F7
	public void OnShiftCreditChanged(string targetMothershipId, int newShiftCredits)
	{
		this.needsUIRefresh = true;
	}

	// Token: 0x0600358C RID: 13708 RVA: 0x00126700 File Offset: 0x00124900
	public void HideOrShowTextBasedOnLocalPlayerDistance()
	{
		Vector3 position = GRPlayer.Get(VRRig.LocalRig).transform.position;
		Vector3 position2 = base.transform.position;
		float num = (this.currentlyShowingText ? 8f : 6f);
		bool flag = (position - position2).sqrMagnitude < num * num;
		if (flag != this.currentlyShowingText)
		{
			this.shelfSelectionText.enabled = flag;
			this.playerInfo.enabled = flag;
			this.itemDescription.enabled = flag;
			this.itemDescriptionName.enabled = flag;
			this.itemDescriptionAnnotation.enabled = flag;
			this.purchaseButtonText.enabled = flag;
			this.pageSelectionWheel.ShowText(flag);
			for (int i = 0; i < this.gameShelves.Count; i++)
			{
				if (!(this.gameShelves[i] == null))
				{
					foreach (GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot in this.gameShelves[i].gRPurchaseSlots)
					{
						if (grpurchaseSlot.Name != null)
						{
							grpurchaseSlot.Name.enabled = flag;
						}
						if (grpurchaseSlot.Price != null)
						{
							grpurchaseSlot.Price.enabled = flag;
						}
					}
				}
			}
		}
		this.currentlyShowingText = flag;
	}

	// Token: 0x0600358D RID: 13709 RVA: 0x00126878 File Offset: 0x00124A78
	public void Tick()
	{
		this.HideOrShowTextBasedOnLocalPlayerDistance();
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (this.toolProgressionManager == null)
		{
			return;
		}
		if (grplayer != null && (this.lastKnownLocalPlayerCredits != grplayer.ShiftCredits || this.lastKnownLocalPlayerJuice != this.toolProgressionManager.GetNumberOfResearchPoints()))
		{
			this.needsUIRefresh = true;
			this.lastKnownLocalPlayerCredits = grplayer.ShiftCredits;
			this.lastKnownLocalPlayerJuice = this.toolProgressionManager.GetNumberOfResearchPoints();
		}
		this.UpdateActivePlayer();
		this.UpdateSelectionLever();
		this.UpdateShelf();
		this.UpdateMagnet();
		if (this.disablePurchaseButton)
		{
			if (this.purchaseButtonPressed > 0f)
			{
				this.purchaseButtonPressed -= Time.deltaTime;
			}
			else
			{
				this.disablePurchaseButton = false;
			}
		}
		if (this.needsUIRefresh)
		{
			this.needsUIRefresh = false;
			this.UpdateShelfDisplayElements(this.currentVisibleShelfIndex);
			this.UpdateShelfDisplayElements(this.nextVisibleShelfIndex);
			this.UpdateShelfDisplayElements(this.selectedShelf);
			this.UpdatePlayerCurrencyUI();
			this.UpdatePurchaseButtonText();
		}
	}

	// Token: 0x0600358E RID: 13710 RVA: 0x0012697C File Offset: 0x00124B7C
	public void SetActivePlayer(int actorNum)
	{
		this.currentActivePlayerActorNumber = actorNum;
		this.needsUIRefresh = true;
		if (this.currentActivePlayerActorNumber == -1)
		{
			this.itemDescriptionName.text = "SWIPE FOR ACCESS";
			this.itemDescription.text = "Welcome to the Tool-o-matic v2 automated vending machine. Please swipe your ID card for access.";
			this.itemDescriptionAnnotation.text = "Remember: Compliance leads to success!";
			return;
		}
		if (this.IsValidShelfItemIndex(this.selectedShelf, this.selectedItem) && this.toolProgressionManager != null)
		{
			GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(this.gameShelves[this.selectedShelf].gRPurchaseSlots[this.selectedItem].PurchaseID);
			if (partMetadata != null)
			{
				this.itemDescriptionName.text = partMetadata.name;
				this.itemDescription.text = partMetadata.description;
				this.itemDescriptionAnnotation.text = partMetadata.annotation;
			}
			this.select1.SetButtonState(this.selectedItem == 0);
			this.select2.SetButtonState(this.selectedItem == 1);
			this.select3.SetButtonState(this.selectedItem == 2);
			this.select4.SetButtonState(this.selectedItem == 3);
		}
	}

	// Token: 0x0600358F RID: 13711 RVA: 0x00126AB4 File Offset: 0x00124CB4
	public void UpdateActivePlayer()
	{
		if (!this.grManager.IsAuthority())
		{
			return;
		}
		if (this.currentActivePlayerActorNumber != -1)
		{
			GRPlayer grplayer = GRPlayer.Get(this.currentActivePlayerActorNumber);
			if (grplayer != null)
			{
				BoxCollider component = base.GetComponent<BoxCollider>();
				Vector3 position = grplayer.transform.position;
				Vector3 vector = component.transform.worldToLocalMatrix.MultiplyPoint(position) - component.center;
				Vector3 vector2 = component.size * 0.5f;
				if (Mathf.Abs(vector.x) > vector2.x || Mathf.Abs(vector.y) > vector2.y || Mathf.Abs(vector.z) > vector2.z)
				{
					this.grManager.SetActivePlayerAuthority(this, -1);
					return;
				}
			}
			else
			{
				this.currentActivePlayerActorNumber = -1;
			}
		}
	}

	// Token: 0x06003590 RID: 13712 RVA: 0x00126B8C File Offset: 0x00124D8C
	private void UpdateShelf()
	{
		switch (this.shelfMovementState)
		{
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle:
			if (this.currentVisibleShelfIndex != this.selectedShelf)
			{
				this.SetNextShelf(this.selectedShelf);
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward);
				return;
			}
			this.SetNextShelf(-1);
			return;
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward:
		{
			if (this.currentVisibleShelfIndex == this.selectedShelf)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfForward);
				return;
			}
			this.frontBackShelfMovement.target = 1f;
			this.frontBackShelfMovement.Update();
			float pos = this.frontBackShelfMovement.pos;
			this.gameShelves[this.currentVisibleShelfIndex].transform.position = Vector3.Lerp(this.shelfRootTransform.position, this.shelfBackTransform.position, pos);
			this.UpdateSoundsForMovement(this.frontBackShelfMovement);
			if (this.frontBackShelfMovement.IsAtTarget())
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward);
				return;
			}
			break;
		}
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfForward:
		{
			if (this.currentVisibleShelfIndex != this.selectedShelf)
			{
				this.SetNextShelf(this.selectedShelf);
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward);
				return;
			}
			this.frontBackShelfMovement.target = 0f;
			this.frontBackShelfMovement.Update();
			float pos2 = this.frontBackShelfMovement.pos;
			this.gameShelves[this.currentVisibleShelfIndex].transform.position = Vector3.Lerp(this.shelfRootTransform.position, this.shelfBackTransform.position, pos2);
			this.UpdateSoundsForMovement(this.frontBackShelfMovement);
			if (this.frontBackShelfMovement.IsAtTarget())
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
				return;
			}
			break;
		}
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward:
		{
			if (this.nextVisibleShelfIndex == -1)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
				return;
			}
			if (this.nextVisibleShelfIndex != this.selectedShelf && this.raiseLowerShelfMovement.pos <= 0.5f)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfDownward);
				return;
			}
			this.raiseLowerShelfMovement.target = 1f;
			this.raiseLowerShelfMovement.Update();
			float pos3 = this.raiseLowerShelfMovement.pos;
			this.gameShelves[this.nextVisibleShelfIndex].transform.position = Vector3.Lerp(this.shelfLowerTransform.position, this.shelfRootTransform.position, pos3);
			this.UpdateSoundsForMovement(this.raiseLowerShelfMovement);
			if (this.raiseLowerShelfMovement.IsAtTarget())
			{
				this.SetCurrentShelf(this.nextVisibleShelfIndex);
				if (this.nextVisibleShelfIndex == this.selectedShelf)
				{
					this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
					return;
				}
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward);
				return;
			}
			break;
		}
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfDownward:
			if (this.nextVisibleShelfIndex == -1)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
				return;
			}
			if (this.nextVisibleShelfIndex != this.selectedShelf)
			{
				this.raiseLowerShelfMovement.target = 0f;
				this.raiseLowerShelfMovement.Update();
				float pos4 = this.raiseLowerShelfMovement.pos;
				this.gameShelves[this.nextVisibleShelfIndex].transform.position = Vector3.Lerp(this.shelfLowerTransform.position, this.shelfRootTransform.position, pos4);
				this.UpdateSoundsForMovement(this.raiseLowerShelfMovement);
				if (this.raiseLowerShelfMovement.IsAtTarget())
				{
					this.SetNextShelf(this.selectedShelf);
					this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward);
					return;
				}
			}
			else
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06003591 RID: 13713 RVA: 0x00126EC0 File Offset: 0x001250C0
	private void UpdateSoundsForMovement(GRSpringMovement movement)
	{
		if (movement.IsAtTarget())
		{
			this.audioSourceLooping.volume = 0f;
			if (movement.HitTargetLastUpdate())
			{
				this.audioSourceClang.Play();
				return;
			}
		}
		else
		{
			this.audioSourceLooping.volume = Mathf.Clamp01(Math.Abs(movement.speed) * this.audioSourceLoopingVolume);
		}
	}

	// Token: 0x06003592 RID: 13714 RVA: 0x00126F1C File Offset: 0x0012511C
	public void SetCurrentShelf(int idx)
	{
		if (idx == -1)
		{
			return;
		}
		if (idx == this.currentVisibleShelfIndex)
		{
			return;
		}
		if (!this.IsValidShelfItemIndex(idx, 0))
		{
			return;
		}
		if (idx == this.nextVisibleShelfIndex)
		{
			this.SetNextShelf(-1);
		}
		this.UpdateShelfVisibility(this.currentVisibleShelfIndex, false);
		this.frontBackShelfMovement.Reset();
		this.gameShelves[idx].transform.position = this.shelfRootTransform.position;
		this.UpdateShelfVisibility(idx, true);
		this.currentVisibleShelfIndex = idx;
	}

	// Token: 0x06003593 RID: 13715 RVA: 0x00126F9C File Offset: 0x0012519C
	public void SetNextShelf(int idx)
	{
		if (idx == this.nextVisibleShelfIndex)
		{
			return;
		}
		if (idx == this.currentVisibleShelfIndex)
		{
			return;
		}
		if (this.nextVisibleShelfIndex != -1)
		{
			this.UpdateShelfVisibility(this.nextVisibleShelfIndex, false);
		}
		if (idx != -1)
		{
			this.raiseLowerShelfMovement.Reset();
			this.gameShelves[idx].transform.position = this.shelfLowerTransform.position;
			this.UpdateShelfVisibility(idx, true);
		}
		this.nextVisibleShelfIndex = idx;
	}

	// Token: 0x06003594 RID: 13716 RVA: 0x00127014 File Offset: 0x00125214
	public void ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState newState)
	{
		this.shelfMovementState = newState;
		switch (newState)
		{
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle:
			this.SetCurrentShelf(this.selectedShelf);
			this.SetNextShelf(-1);
			return;
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward:
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfForward:
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfDownward:
			this.audioSourceLooping.volume = 0f;
			this.audioSourceLooping.GTPlay();
			return;
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward:
			if (this.currentVisibleShelfIndex == this.selectedShelf)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfForward);
			}
			else
			{
				this.SetNextShelf(this.selectedShelf);
			}
			this.audioSourceLooping.volume = 0f;
			this.audioSourceLooping.GTPlay();
			return;
		default:
			return;
		}
	}

	// Token: 0x06003595 RID: 13717 RVA: 0x001270B1 File Offset: 0x001252B1
	public void UpdateShelfVisibility(int shelfID, bool isVisible)
	{
		if (!this.IsValidShelfItemIndex(shelfID, 0))
		{
			return;
		}
		this.gameShelves[shelfID].gameObject.SetActive(isVisible);
		if (isVisible)
		{
			this.UpdateShelfDisplayElements(shelfID);
		}
	}

	// Token: 0x06003596 RID: 13718 RVA: 0x001270E0 File Offset: 0x001252E0
	public void UpdateShelfDisplayElements(int shelfID)
	{
		if (!this.IsValidShelfItemIndex(shelfID, 0))
		{
			return;
		}
		GRToolUpgradePurchaseStationShelf grtoolUpgradePurchaseStationShelf = this.gameShelves[shelfID];
		for (int i = 0; i < grtoolUpgradePurchaseStationShelf.gRPurchaseSlots.Count; i++)
		{
			this.UpdateShelfItemDisplayElements(shelfID, i);
		}
	}

	// Token: 0x06003597 RID: 13719 RVA: 0x00127124 File Offset: 0x00125324
	public void UpdatePurchaseButtonText()
	{
		if (!this.IsValidShelfItemIndex(this.selectedShelf, this.selectedItem))
		{
			this.purchaseButtonText.text = "ERROR";
			return;
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[this.selectedShelf].gRPurchaseSlots[this.selectedItem];
		Color color = (grpurchaseSlot.canAfford ? this.colorPurchaseButtonCanAfford : this.colorCantBuy);
		string purchaseText = grpurchaseSlot.purchaseText;
		if (color != this.purchaseButtonText.color)
		{
			this.purchaseButtonText.color = color;
		}
		if (purchaseText != this.purchaseButtonText.text)
		{
			this.purchaseButtonText.text = purchaseText;
		}
	}

	// Token: 0x06003598 RID: 13720 RVA: 0x001271D4 File Offset: 0x001253D4
	public void UpdateShelfItemDisplayElements(int shelf, int slotID)
	{
		if (!this.IsValidShelfItemIndex(shelf, slotID))
		{
			return;
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[shelf].gRPurchaseSlots[slotID];
		if (this.toolProgressionManager)
		{
			GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(grpurchaseSlot.PurchaseID);
			if (partMetadata == null)
			{
				grpurchaseSlot.Name.text = "ERROR";
				return;
			}
			string text = "ERROR";
			string text2 = "";
			Color white = Color.white;
			bool flag = true;
			int num = 10000;
			int num2;
			this.toolProgressionManager.GetPlayerShiftCredit(out num2);
			int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
			grpurchaseSlot.canAfford = false;
			grpurchaseSlot.purchaseText = "LOCKED";
			bool flag2;
			if (this.toolProgressionManager.IsPartUnlocked(grpurchaseSlot.PurchaseID, out flag2))
			{
				if (flag2)
				{
					this.gameShelves[shelf].SetMaterialOverride(slotID, null);
					if (this.toolProgressionManager.GetShiftCreditCost(grpurchaseSlot.PurchaseID, out num))
					{
						text = string.Format("⑭ {0}", num);
					}
					bool flag3 = num2 >= num;
					grpurchaseSlot.Name.text = partMetadata.name;
					grpurchaseSlot.Name.color = ((slotID == this.selectedItem) ? this.colorSelectedItem : this.colorUnselectedItem);
					grpurchaseSlot.Price.text = text;
					grpurchaseSlot.Price.color = (flag3 ? this.colorCanBuyCredits : this.colorCantBuy);
					grpurchaseSlot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
					grpurchaseSlot.canAfford = flag3;
					if (flag3)
					{
						grpurchaseSlot.purchaseText = string.Format("BUY FOR\n⑭ {0}", num);
					}
					else
					{
						grpurchaseSlot.purchaseText = string.Format("NEED\n⑭ {0}", num);
					}
				}
				else
				{
					this.gameShelves[shelf].SetMaterialOverride(slotID, this.unresearchedItemMaterial);
					grpurchaseSlot.Name.text = partMetadata.name;
					grpurchaseSlot.Name.color = ((slotID == this.selectedItem) ? this.colorUnresearchedItem : this.colorUnselectedUnresearchedItem);
					flag = true;
					GRToolProgressionTree.EmployeeLevelRequirement employeeLevelRequirement;
					if (this.toolProgressionManager.GetPartUnlockEmployeeRequiredLevel(grpurchaseSlot.PurchaseID, out employeeLevelRequirement) && this.toolProgressionManager.GetCurrentEmployeeLevel() < employeeLevelRequirement)
					{
						this.toolProgressionManager.GetEmployeeLevelDisplayName(employeeLevelRequirement);
						text2 += string.Format("⑱ {0}\n", employeeLevelRequirement);
						flag = false;
					}
					this.cachedRequiredPartsList.Clear();
					if (this.toolProgressionManager.GetPartUnlockRequiredParentParts(grpurchaseSlot.PurchaseID, out this.cachedRequiredPartsList))
					{
						foreach (GRToolProgressionManager.ToolParts toolParts in this.cachedRequiredPartsList)
						{
							bool flag4 = false;
							GRToolProgressionManager.ToolProgressionMetaData partMetadata2 = this.toolProgressionManager.GetPartMetadata(toolParts);
							if (partMetadata2 == null)
							{
								text2 += "⑱ ERROR\n";
								flag = false;
							}
							else if (!this.toolProgressionManager.IsPartUnlocked(toolParts, out flag4) || !flag4)
							{
								text2 = text2 + "⑱ " + partMetadata2.name + "\n";
								flag = false;
							}
						}
					}
					if (!flag)
					{
						grpurchaseSlot.Price.text = text2;
						grpurchaseSlot.Price.color = this.colorCantBuy;
						grpurchaseSlot.Price.fontSize = ((text2.Length <= 8) ? 2.25f : 1.6f);
						grpurchaseSlot.canAfford = false;
						grpurchaseSlot.purchaseText = "LOCKED";
					}
					else
					{
						if (this.toolProgressionManager.GetPartUnlockJuiceCost(grpurchaseSlot.PurchaseID, out num))
						{
							text = string.Format("⑮ {0}", num);
						}
						bool flag3 = numberOfResearchPoints >= num;
						grpurchaseSlot.Price.text = text;
						grpurchaseSlot.Price.color = (flag3 ? this.colorCanBuyJuice : this.colorCantBuy);
						grpurchaseSlot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
						grpurchaseSlot.canAfford = flag3;
						if (flag3)
						{
							grpurchaseSlot.purchaseText = string.Format("RESEARCH\n⑮ {0}", num);
						}
						else
						{
							grpurchaseSlot.purchaseText = string.Format("NEED\n⑮ {0}", num);
						}
					}
				}
			}
			if (slotID != this.selectedItem)
			{
				this.gameShelves[shelf].SetBacklightStateAndMaterial(slotID, false, this.backlightLocked);
				return;
			}
			if (grpurchaseSlot.Price.color == this.colorCanBuyJuice)
			{
				this.gameShelves[shelf].SetBacklightStateAndMaterial(slotID, true, this.backlightResearch);
				return;
			}
			if (grpurchaseSlot.Price.color == this.colorCanBuyCredits)
			{
				this.gameShelves[shelf].SetBacklightStateAndMaterial(slotID, true, this.backlightPurchase);
				return;
			}
			this.gameShelves[shelf].SetBacklightStateAndMaterial(slotID, true, this.backlightLocked);
		}
	}

	// Token: 0x06003599 RID: 13721 RVA: 0x001276B4 File Offset: 0x001258B4
	public void UpdatePlayerCurrencyUI()
	{
		if (this.currentActivePlayerActorNumber == -1)
		{
			this.playerInfo.text = "AVAILABLE";
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		GRPlayer grplayer2 = GRPlayer.Get(this.currentActivePlayerActorNumber);
		if (grplayer2 == null)
		{
			this.currentActivePlayerActorNumber = -1;
			this.playerInfo.text = "AVAILABLE";
			return;
		}
		string text2;
		if (grplayer2 == grplayer)
		{
			int shiftCredits = grplayer2.ShiftCredits;
			int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
			NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentActivePlayerActorNumber);
			string text = ((player != null) ? player.SanitizedNickName : "RANDO MONKE");
			string employeeLevelDisplayName = this.toolProgressionManager.GetEmployeeLevelDisplayName(this.toolProgressionManager.GetCurrentEmployeeLevel());
			text2 = string.Format("<color=#c0c0c0>{0}\n{1}</color>\n\n<color=purple><size=2>⑮ {2}</size></color>\n<color=white><size=2>⑭ {3}</size></color>\n", new object[] { text, employeeLevelDisplayName, numberOfResearchPoints, shiftCredits });
		}
		else
		{
			NetPlayer player2 = NetworkSystem.Instance.GetPlayer(this.currentActivePlayerActorNumber);
			text2 = ((player2 != null) ? player2.SanitizedNickName : "RANDO MONKE") ?? "";
		}
		this.playerInfo.text = text2;
	}

	// Token: 0x0600359A RID: 13722 RVA: 0x001277E4 File Offset: 0x001259E4
	public bool CanLocalPlayerPurchaseItem(int shelf, int slotID)
	{
		if (!this.IsValidShelfItemIndex(shelf, slotID))
		{
			return false;
		}
		if (this.grManager && this.grManager.DebugIsToolStationHacked())
		{
			return true;
		}
		this.UpdateShelfItemDisplayElements(shelf, slotID);
		return this.gameShelves[shelf].gRPurchaseSlots[slotID].canAfford;
	}

	// Token: 0x0600359B RID: 13723 RVA: 0x00127840 File Offset: 0x00125A40
	public bool CheckActivePlayer()
	{
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (this.currentActivePlayerActorNumber == -1)
		{
			this.RequestActivePlayerToken();
			return false;
		}
		GRPlayer grplayer2 = GRPlayer.Get(this.currentActivePlayerActorNumber);
		if (grplayer2 == null)
		{
			this.currentActivePlayerActorNumber = -1;
		}
		return !(grplayer2 != grplayer);
	}

	// Token: 0x0600359C RID: 13724 RVA: 0x0012788F File Offset: 0x00125A8F
	public void SelectOption1()
	{
		this.OnLocalSelectionButtonPressed(0);
	}

	// Token: 0x0600359D RID: 13725 RVA: 0x00127898 File Offset: 0x00125A98
	public void SelectOption2()
	{
		this.OnLocalSelectionButtonPressed(1);
	}

	// Token: 0x0600359E RID: 13726 RVA: 0x001278A1 File Offset: 0x00125AA1
	public void SelectOption3()
	{
		this.OnLocalSelectionButtonPressed(2);
	}

	// Token: 0x0600359F RID: 13727 RVA: 0x001278AA File Offset: 0x00125AAA
	public void SelectOption4()
	{
		this.OnLocalSelectionButtonPressed(3);
	}

	// Token: 0x060035A0 RID: 13728 RVA: 0x001278B4 File Offset: 0x00125AB4
	public void OnLocalSelectionButtonPressed(int index)
	{
		if (!this.CheckActivePlayer())
		{
			if (index == 0 && this.selectedItem != 0)
			{
				this.select1.SetButtonState(false);
			}
			if (index == 1 && this.selectedItem != 1)
			{
				this.select2.SetButtonState(false);
			}
			if (index == 2 && this.selectedItem != 2)
			{
				this.select3.SetButtonState(false);
			}
			if (index == 3 && this.selectedItem != 3)
			{
				this.select4.SetButtonState(false);
			}
			return;
		}
		if (index != 0)
		{
			this.select1.SetButtonState(false);
		}
		if (index != 1)
		{
			this.select2.SetButtonState(false);
		}
		if (index != 2)
		{
			this.select3.SetButtonState(false);
		}
		if (index != 3)
		{
			this.select4.SetButtonState(false);
		}
		if (this.shelfMovementState == GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle)
		{
			this.SetSelectedShelfAndItem(this.selectedShelf, index, false);
		}
	}

	// Token: 0x060035A1 RID: 13729 RVA: 0x00127981 File Offset: 0x00125B81
	public void SelectPageDown()
	{
		this.OnLocalSelectionPageChange(1);
	}

	// Token: 0x060035A2 RID: 13730 RVA: 0x0012798A File Offset: 0x00125B8A
	public void SelectPageUp()
	{
		this.OnLocalSelectionPageChange(-1);
	}

	// Token: 0x060035A3 RID: 13731 RVA: 0x00127993 File Offset: 0x00125B93
	public void OnLocalSelectionPageChange(int delta)
	{
		if (!this.CheckActivePlayer())
		{
			return;
		}
		this.pageSelectionWheel.SetTargetShelf((this.pageSelectionWheel.targetPage + delta + this.gameShelves.Count) % this.gameShelves.Count);
	}

	// Token: 0x060035A4 RID: 13732 RVA: 0x001279CE File Offset: 0x00125BCE
	public void CardSwiped()
	{
		this.RequestActivePlayerToken();
	}

	// Token: 0x060035A5 RID: 13733 RVA: 0x001279D8 File Offset: 0x00125BD8
	public void PurchaseButtonPressed()
	{
		if (this.disablePurchaseButton)
		{
			return;
		}
		this.purchaseButtonPressed = this.purchaseButtonCooldown;
		this.disablePurchaseButton = true;
		if (!this.CheckActivePlayer())
		{
			return;
		}
		if (this.shelfMovementState == GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle && this.desiredMagnetEntityTypeId == this.currentMagnetEntityTypeId)
		{
			this.RequestPurchaseItem(this.selectedShelf, this.selectedItem);
		}
	}

	// Token: 0x060035A6 RID: 13734 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void DEBUGSetHackToolStation()
	{
	}

	// Token: 0x060035A7 RID: 13735 RVA: 0x00127A32 File Offset: 0x00125C32
	public void RequestActivePlayerToken()
	{
		if (this.lastRequestedActivePlayerTokenTime > Time.time || this.lastRequestedActivePlayerTokenTime + this.requestActivePlayerTokenThrottleTime < Time.time)
		{
			this.lastRequestedActivePlayerTokenTime = Time.time;
			this.grManager.RequestStationExclusivity(this);
		}
	}

	// Token: 0x060035A8 RID: 13736 RVA: 0x00127A6C File Offset: 0x00125C6C
	private void UpdateMagnet()
	{
		if (this.desiredMagnetEntityTypeId != this.currentMagnetEntityTypeId || this.currentMagnetEntityTypeId == -1 || this.currentMagnetEntity == null)
		{
			this.magnetMovement.SetHardStopAtTarget(true);
			this.magnetMovement.target = 0f;
			this.magnetMovement.Update();
			Vector3 position = this.ropeTop.transform.position;
			position.y = Mathf.Lerp(this.prefabMagnetHeightOffset, this.prefabMagnetHeightOffset - this.maxMagnetDistance, this.magnetMovement.pos);
			if (position.y != this.ropeTop.transform.position.y)
			{
				this.ropeTop.transform.position = position;
			}
			if (this.magnetMovement.IsAtTarget() && this.grManager.IsAuthority() && this.grManager.IsZoneActive())
			{
				if (this.currentMagnetEntity != null)
				{
					this.currentMagnetEntity.transform.parent = null;
					this.currentMagnetEntity.gameObject.SetActive(false);
					this.grManager.gameEntityManager.RequestDestroyItem(this.currentMagnetEntity.id);
					this.currentMagnetEntity = null;
					this.currentMagnetEntityTypeId = -1;
				}
				if (this.desiredMagnetEntityTypeId != -1)
				{
					GhostReactor.ToolEntityCreateData toolEntityCreateData = default(GhostReactor.ToolEntityCreateData);
					toolEntityCreateData.decayTime = 0f;
					toolEntityCreateData.stationIndex = this.grManager.GetIndexForToolUpgradeStationFull(this);
					this.grManager.gameEntityManager.RequestCreateItem(this.desiredMagnetEntityTypeId, this.ropeEnd.position, this.ropeEnd.rotation, toolEntityCreateData.Pack());
					this.currentMagnetEntityTypeId = this.desiredMagnetEntityTypeId;
					return;
				}
			}
		}
		else if (this.desiredMagnetEntityTypeId == this.currentMagnetEntityTypeId && this.currentMagnetEntity != null)
		{
			this.magnetMovement.SetHardStopAtTarget(false);
			this.magnetMovement.target = 1f;
			this.magnetMovement.Update();
			Vector3 position2 = this.ropeTop.transform.position;
			position2.y = Mathf.Lerp(this.prefabMagnetHeightOffset, this.prefabMagnetHeightOffset - this.maxMagnetDistance, this.magnetMovement.pos);
			if (this.ropeTop.transform.position.y != position2.y)
			{
				this.ropeTop.transform.position = position2;
			}
		}
	}

	// Token: 0x060035A9 RID: 13737 RVA: 0x00127CE0 File Offset: 0x00125EE0
	public void InitLinkedEntity(GameEntity entity)
	{
		if (this.currentMagnetEntity != null)
		{
			this.currentMagnetEntity.gameObject.SetActive(false);
		}
		entity.pickupable = false;
		Rigidbody component = entity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = true;
		}
		GRToolUpgradePurchaseStationMagnetPoint component2 = entity.GetComponent<GRToolUpgradePurchaseStationMagnetPoint>();
		GameDockable component3 = entity.GetComponent<GameDockable>();
		Transform transform = ((component2 != null) ? component2.magnetAttachTransform : ((component3 != null) ? component3.dockablePoint : entity.transform));
		GRToolUpgradePurchaseStationFull.AttachEntityToMagnet_DockGoesToLocation(this.magnet, entity.transform, transform, new Vector3(0f, -0.03f, 0f));
		float num = 0f;
		float num2 = 0f;
		bool flag = false;
		for (int i = 0; i < this.gameShelves.Count; i++)
		{
			for (int j = 0; j < this.gameShelves[i].gRPurchaseSlots.Count; j++)
			{
				GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[i].gRPurchaseSlots[j];
				if (grpurchaseSlot != null && !(grpurchaseSlot.ToolEntityPrefab == null) && grpurchaseSlot.ToolEntityPrefab.name != null && grpurchaseSlot.ToolEntityPrefab.name.GetStaticHash() == entity.typeId)
				{
					num = grpurchaseSlot.RopeYaw;
					num2 = grpurchaseSlot.RopePitch;
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		Quaternion quaternion = Quaternion.Euler(0f, 0f, 180f);
		quaternion = Quaternion.AngleAxis(num, Vector3.up) * quaternion;
		quaternion = Quaternion.AngleAxis(num2, Vector3.forward) * quaternion;
		this.magnet.localRotation = quaternion;
		this.magnet.localPosition = quaternion * new Vector3(0f, 0.055f, 0f);
		this.currentMagnetEntity = entity;
		this.currentMagnetEntityTypeId = entity.typeId;
	}

	// Token: 0x060035AA RID: 13738 RVA: 0x00127ED8 File Offset: 0x001260D8
	public void UpdateSelectionLever()
	{
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		GRPlayer grplayer2 = GRPlayer.Get(this.currentActivePlayerActorNumber);
		bool flag = ControllerInputPoller.GripFloat(XRNode.LeftHand) > 0.7f;
		bool flag2 = ControllerInputPoller.GripFloat(XRNode.RightHand) > 0.7f;
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		Transform handTransform = gamePlayer.GetHandTransform(0);
		Transform handTransform2 = gamePlayer.GetHandTransform(1);
		Vector3 position = this.pageSelectionHandle.transform.position;
		Vector3 vector = handTransform.position - position;
		Vector3 vector2 = handTransform2.position - position;
		float num = this.pageSelectionLever.transform.localRotation.eulerAngles.x;
		float num2 = 0.2f;
		float num3 = (this.bIsGrippingLeft ? 0.15f : 0.1f);
		float num4 = (this.bIsGrippingRight ? 0.15f : 0.1f);
		if (vector.sqrMagnitude > num3 * num3)
		{
			flag = false;
		}
		if (vector2.sqrMagnitude > num4 * num4)
		{
			flag2 = false;
		}
		if (!this.bGripLeftLastFrame && flag)
		{
			this.bIsGrippingLeft = true;
		}
		else if (this.bGripLeftLastFrame && flag)
		{
			Vector3 forward = this.pageSelectionHandle.transform.forward;
			float num5 = Vector3.Dot(vector, forward);
			num += num5 / num2 * 180f / 3.1415925f;
		}
		else
		{
			this.bIsGrippingLeft = false;
		}
		if (!this.bGripRightLastFrame && flag2)
		{
			this.bIsGrippingRight = true;
		}
		else if (this.bGripRightLastFrame && flag2)
		{
			Vector3 forward2 = this.pageSelectionHandle.transform.forward;
			float num6 = Vector3.Dot(vector2, forward2);
			num += num6 / num2 * 180f / 3.1415925f;
		}
		else
		{
			this.bIsGrippingRight = false;
		}
		if (!this.bIsGrippingLeft && !this.bIsGrippingRight && grplayer == grplayer2)
		{
			num = 30f + (num - 30f) * Mathf.Exp(-20f * Time.deltaTime);
		}
		num = Mathf.Clamp(num, 0f, 60f);
		if ((grplayer == grplayer2 || this.currentActivePlayerActorNumber == -1) && this.lastHandleAngle != num)
		{
			this.pageSelectionLever.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
			this.lastHandleAngle = num;
		}
		float num7 = 0f;
		if (this.bIsGrippingLeft || this.bIsGrippingRight)
		{
			num7 = (num - 30f) / 30f;
		}
		this.bGripLeftLastFrame = flag;
		this.bGripRightLastFrame = flag2;
		if (grplayer == grplayer2)
		{
			this.pageSelectionWheel.isBeingDrivenRemotely = false;
			this.pageSelectionWheel.SetRotationSpeed(num7);
			if (this.pageSelectionWheel.targetPage != this.selectedShelf)
			{
				this.SetSelectedShelfAndItem(this.pageSelectionWheel.targetPage, 0, false);
			}
			float num8 = 0.25f;
			this.timeSinceLastHandleBroadcast += Time.deltaTime;
			if (this.timeSinceLastHandleBroadcast > num8 && (Math.Abs(num - this.angleOfLastHandleBroadcast) > 0.02f || Math.Abs(this.pageSelectionWheel.currentAngle - this.selectionWheelAngleOfLastBroadcast) > 0.02f))
			{
				this.timeSinceLastHandleBroadcast = 0f;
				this.angleOfLastHandleBroadcast = num;
				this.selectionWheelAngleOfLastBroadcast = this.pageSelectionWheel.currentAngle;
				this.grManager.BroadcastHandleAndSelectionWheelPosition(this, (int)(num * this.quantMult), (int)(this.selectionWheelAngleOfLastBroadcast * this.quantMult));
				return;
			}
		}
		else if (this.bIsGrippingLeft || this.bIsGrippingRight)
		{
			this.CheckActivePlayer();
		}
	}

	// Token: 0x060035AB RID: 13739 RVA: 0x0012825C File Offset: 0x0012645C
	public static void AttachEntityToMagnet_DockGoesToLocation(Transform magnet, Transform entity, Transform dock, Vector3 magnetDockOffset)
	{
		if (magnet == null || entity == null || dock == null)
		{
			return;
		}
		if (!dock.IsChildOf(entity))
		{
			return;
		}
		Matrix4x4 matrix4x = entity.worldToLocalMatrix * dock.localToWorldMatrix;
		Vector3 vector = GRToolUpgradePurchaseStationFull.ExtractLossyScale(matrix4x);
		Vector3 vector2;
		Quaternion quaternion;
		Vector3 vector3;
		GRToolUpgradePurchaseStationFull.DecomposeTRS(Matrix4x4.TRS(magnetDockOffset, Quaternion.identity, vector) * matrix4x.inverse, out vector2, out quaternion, out vector3);
		entity.SetParent(magnet, false);
		entity.localPosition = vector2;
		entity.localRotation = quaternion;
		entity.localScale = vector3;
	}

	// Token: 0x060035AC RID: 13740 RVA: 0x001282EC File Offset: 0x001264EC
	public void SetHandleAndSelectionWheelPositionRemote(int handlePos, int wheelPos)
	{
		this.pageSelectionWheel.isBeingDrivenRemotely = true;
		float num = (float)handlePos / this.quantMult;
		num = Mathf.Clamp(num, 0f, 60f);
		this.pageSelectionLever.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
		this.pageSelectionWheel.SetTargetAngle((float)wheelPos / this.quantMult);
	}

	// Token: 0x060035AD RID: 13741 RVA: 0x001266F7 File Offset: 0x001248F7
	public void ProgressionUpdated()
	{
		this.needsUIRefresh = true;
	}

	// Token: 0x060035AE RID: 13742 RVA: 0x00128354 File Offset: 0x00126554
	public void SetSelectedShelfAndItem(int shelf, int item, bool fromNetworkRPC)
	{
		if (!this.IsValidShelfItemIndex(shelf, item))
		{
			return;
		}
		if (this.toolProgressionManager == null)
		{
			return;
		}
		GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(this.gameShelves[shelf].gRPurchaseSlots[item].PurchaseID);
		if (partMetadata != null)
		{
			this.itemDescriptionName.text = partMetadata.name;
			this.itemDescription.text = partMetadata.description;
			this.itemDescriptionAnnotation.text = partMetadata.annotation;
		}
		this.shelfSelectionText.text = this.gameShelves[shelf].ShelfName;
		if (this.gameShelves[shelf].gRPurchaseSlots[item].ToolEntityPrefab != null)
		{
			this.desiredMagnetEntityTypeId = this.gameShelves[shelf].gRPurchaseSlots[item].ToolEntityPrefab.name.GetStaticHash();
		}
		else
		{
			this.desiredMagnetEntityTypeId = -1;
		}
		bool flag = this.selectedShelf != shelf;
		bool flag2 = this.selectedItem != item;
		this.selectedShelf = shelf;
		this.selectedItem = item;
		this.needsUIRefresh = true;
		if (!fromNetworkRPC)
		{
			if (flag || flag2)
			{
				this.grManager.RequestNetworkShelfAndItemChange(this, this.selectedShelf, this.selectedItem);
				return;
			}
		}
		else
		{
			this.pageSelectionWheel.SetTargetShelf(this.selectedShelf);
			this.select1.SetButtonState(this.selectedItem == 0);
			this.select2.SetButtonState(this.selectedItem == 1);
			this.select3.SetButtonState(this.selectedItem == 2);
			this.select4.SetButtonState(this.selectedItem == 3);
		}
	}

	// Token: 0x060035AF RID: 13743 RVA: 0x00128500 File Offset: 0x00126700
	public void RequestPurchaseItem(int shelf, int item)
	{
		if (!this.IsValidShelfItemIndex(shelf, item))
		{
			return;
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[shelf].gRPurchaseSlots[item];
		if (!this.CanLocalPlayerPurchaseItem(shelf, item))
		{
			if (this.scanner != null)
			{
				UnityEvent onFailed = this.scanner.onFailed;
				if (onFailed == null)
				{
					return;
				}
				onFailed.Invoke();
			}
			return;
		}
		if (this.scanner != null)
		{
			UnityEvent onSucceeded = this.scanner.onSucceeded;
			if (onSucceeded != null)
			{
				onSucceeded.Invoke();
			}
		}
		bool flag;
		if (!this.grManager.DebugIsToolStationHacked() && (!this.toolProgressionManager.IsPartUnlocked(grpurchaseSlot.PurchaseID, out flag) || !flag))
		{
			this.toolProgressionManager.AttemptToUnlockPart(grpurchaseSlot.PurchaseID);
			return;
		}
		this.grManager.RequestPurchaseToolOrUpgrade(this, shelf, item);
	}

	// Token: 0x060035B0 RID: 13744 RVA: 0x001285C8 File Offset: 0x001267C8
	public ValueTuple<bool, bool> TryPurchaseAuthority(GRPlayer player, int shelf, int item)
	{
		if (this.currentActivePlayerActorNumber == -1)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		GRPlayer grplayer = GRPlayer.Get(this.currentActivePlayerActorNumber);
		if (grplayer == null)
		{
			this.currentActivePlayerActorNumber = -1;
			return new ValueTuple<bool, bool>(false, false);
		}
		if (player != grplayer)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		if (!this.grManager.IsAuthority())
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		if (!this.IsValidShelfItemIndex(shelf, item))
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		if (!this.toolProgressionManager)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[shelf].gRPurchaseSlots[item];
		this.toolProgressionManager.GetPartMetadata(grpurchaseSlot.PurchaseID);
		return new ValueTuple<bool, bool>(true, true);
	}

	// Token: 0x060035B1 RID: 13745 RVA: 0x00128688 File Offset: 0x00126888
	public void ToolPurchaseResponseLocal(GRPlayer player, int shelf, int item, bool success)
	{
		if (!this.IsValidShelfItemIndex(shelf, item))
		{
			return;
		}
		if (!this.toolProgressionManager)
		{
			return;
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[shelf].gRPurchaseSlots[item];
		GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(grpurchaseSlot.PurchaseID);
		if (partMetadata == null)
		{
			return;
		}
		if (success)
		{
			int shiftCreditCost = partMetadata.shiftCreditCost;
			if (player != null)
			{
				if (player == GRPlayer.Get(VRRig.LocalRig))
				{
					player.IncrementCoresSpentPlayer(shiftCreditCost);
					player.SendToolPurchasedTelemetry(partMetadata.name, item, shiftCreditCost, 0);
				}
				else
				{
					player.IncrementCoresSpentGroup(shiftCreditCost);
				}
				player.AddItemPurchased(partMetadata.name);
				player.SubtractShiftCredit(shiftCreditCost);
				player.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.SpentCredits, (float)shiftCreditCost);
				this.reactor.RefreshScoreboards();
			}
			if (this.currentMagnetEntity != null)
			{
				this.currentMagnetEntity.transform.parent = null;
				this.currentMagnetEntity.GetComponent<Rigidbody>().isKinematic = false;
				this.currentMagnetEntity.pickupable = true;
				this.currentMagnetEntity.createData = 0L;
				this.currentMagnetEntity = null;
				this.currentMagnetEntityTypeId = -1;
			}
			UnityEvent unityEvent = this.purchaseSucceded;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
			return;
		}
		else
		{
			UnityEvent unityEvent2 = this.purchaseFailed;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke();
			return;
		}
	}

	// Token: 0x060035B2 RID: 13746 RVA: 0x001287C4 File Offset: 0x001269C4
	public void InitPageSelectionWheel()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < this.gameShelves.Count; i++)
		{
			list.Add(this.gameShelves[i].ShelfName);
		}
		this.pageSelectionWheel.InitFromNameList(list);
	}

	// Token: 0x060035B3 RID: 13747 RVA: 0x00128810 File Offset: 0x00126A10
	public static Color ColorFromRGB32(int r, int g, int b)
	{
		return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
	}

	// Token: 0x060035B4 RID: 13748 RVA: 0x00128830 File Offset: 0x00126A30
	public bool IsValidShelfItemIndex(int shelf, int idx)
	{
		return shelf >= 0 && shelf < this.gameShelves.Count && this.gameShelves[shelf].gRPurchaseSlots != null && idx >= 0 && idx < this.gameShelves[shelf].gRPurchaseSlots.Count && this.gameShelves[shelf].gRPurchaseSlots[idx].PurchaseID > GRToolProgressionManager.ToolParts.None;
	}

	// Token: 0x060035B5 RID: 13749 RVA: 0x001288A0 File Offset: 0x00126AA0
	private static Vector3 ExtractLossyScale(Matrix4x4 m)
	{
		float magnitude = new Vector3(m.m00, m.m10, m.m20).magnitude;
		float magnitude2 = new Vector3(m.m01, m.m11, m.m21).magnitude;
		float magnitude3 = new Vector3(m.m02, m.m12, m.m22).magnitude;
		return new Vector3(magnitude, magnitude2, magnitude3);
	}

	// Token: 0x060035B6 RID: 13750 RVA: 0x00128914 File Offset: 0x00126B14
	private static void DecomposeTRS(Matrix4x4 m, out Vector3 pos, out Quaternion rot, out Vector3 scale)
	{
		pos = m.GetColumn(3);
		Vector3 vector = m.GetColumn(0);
		Vector3 vector2 = m.GetColumn(1);
		Vector3 vector3 = m.GetColumn(2);
		scale = new Vector3(vector.magnitude, vector2.magnitude, vector3.magnitude);
		vector / scale.x;
		Vector3 vector4 = vector2 / scale.y;
		Vector3 vector5 = vector3 / scale.z;
		rot = Quaternion.LookRotation(vector5, vector4);
	}

	// Token: 0x040045E5 RID: 17893
	private GhostReactor reactor;

	// Token: 0x040045E6 RID: 17894
	private GhostReactorManager grManager;

	// Token: 0x040045E7 RID: 17895
	public List<GRToolUpgradePurchaseStationShelf> gameShelves;

	// Token: 0x040045E8 RID: 17896
	[NonSerialized]
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x040045E9 RID: 17897
	private Color colorPurchaseButtonCanAfford = GRToolUpgradePurchaseStationFull.ColorFromRGB32(0, 0, 0);

	// Token: 0x040045EA RID: 17898
	private Color colorCanBuyCredits = GRToolUpgradePurchaseStationFull.ColorFromRGB32(140, 229, 37);

	// Token: 0x040045EB RID: 17899
	private Color colorCanBuyJuice = GRToolUpgradePurchaseStationFull.ColorFromRGB32(232, 65, 255);

	// Token: 0x040045EC RID: 17900
	private Color colorCantBuy = GRToolUpgradePurchaseStationFull.ColorFromRGB32(140, 38, 38);

	// Token: 0x040045ED RID: 17901
	private Color colorSelectedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(251, 240, 229);

	// Token: 0x040045EE RID: 17902
	private Color colorUnselectedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(147, 145, 140);

	// Token: 0x040045EF RID: 17903
	private Color colorUnresearchedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(230, 19, 17);

	// Token: 0x040045F0 RID: 17904
	private Color colorUnselectedUnresearchedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(133, 11, 10);

	// Token: 0x040045F1 RID: 17905
	private int selectedShelf;

	// Token: 0x040045F2 RID: 17906
	private int selectedItem;

	// Token: 0x040045F3 RID: 17907
	[NonSerialized]
	public int currentActivePlayerActorNumber = -1;

	// Token: 0x040045F4 RID: 17908
	private GRToolUpgradePurchaseStationFull.ShelfMovementState shelfMovementState;

	// Token: 0x040045F5 RID: 17909
	private int currentVisibleShelfIndex;

	// Token: 0x040045F6 RID: 17910
	private int nextVisibleShelfIndex;

	// Token: 0x040045F7 RID: 17911
	private GRSpringMovement frontBackShelfMovement;

	// Token: 0x040045F8 RID: 17912
	private GRSpringMovement raiseLowerShelfMovement;

	// Token: 0x040045F9 RID: 17913
	public Transform shelfRootTransform;

	// Token: 0x040045FA RID: 17914
	public Transform shelfBackTransform;

	// Token: 0x040045FB RID: 17915
	public Transform shelfLowerTransform;

	// Token: 0x040045FC RID: 17916
	public TMP_Text shelfSelectionText;

	// Token: 0x040045FD RID: 17917
	public TMP_Text playerInfo;

	// Token: 0x040045FE RID: 17918
	public TMP_Text itemDescription;

	// Token: 0x040045FF RID: 17919
	public TMP_Text itemDescriptionName;

	// Token: 0x04004600 RID: 17920
	public TMP_Text itemDescriptionAnnotation;

	// Token: 0x04004601 RID: 17921
	public TMP_Text purchaseButtonText;

	// Token: 0x04004602 RID: 17922
	public GorillaPhysicalButton select1;

	// Token: 0x04004603 RID: 17923
	public GorillaPhysicalButton select2;

	// Token: 0x04004604 RID: 17924
	public GorillaPhysicalButton select3;

	// Token: 0x04004605 RID: 17925
	public GorillaPhysicalButton select4;

	// Token: 0x04004606 RID: 17926
	public AudioSource audioSourceLooping;

	// Token: 0x04004607 RID: 17927
	public AudioSource audioSourceClang;

	// Token: 0x04004608 RID: 17928
	public float audioSourceLoopingVolume = 0.5f;

	// Token: 0x04004609 RID: 17929
	public Material unresearchedItemMaterial;

	// Token: 0x0400460A RID: 17930
	public AudioSource interactAudioSource;

	// Token: 0x0400460B RID: 17931
	public IDCardScanner scanner;

	// Token: 0x0400460C RID: 17932
	public UnityEvent purchaseSucceded;

	// Token: 0x0400460D RID: 17933
	public UnityEvent purchaseFailed;

	// Token: 0x0400460E RID: 17934
	public Material backlightPurchase;

	// Token: 0x0400460F RID: 17935
	public Material backlightResearch;

	// Token: 0x04004610 RID: 17936
	public Material backlightLocked;

	// Token: 0x04004611 RID: 17937
	private int lastKnownLocalPlayerCredits;

	// Token: 0x04004612 RID: 17938
	private int lastKnownLocalPlayerJuice;

	// Token: 0x04004613 RID: 17939
	private bool needsUIRefresh;

	// Token: 0x04004614 RID: 17940
	public Transform ropeTop;

	// Token: 0x04004615 RID: 17941
	public Transform ropeEnd;

	// Token: 0x04004616 RID: 17942
	public Transform magnet;

	// Token: 0x04004617 RID: 17943
	private GameEntity currentMagnetEntity;

	// Token: 0x04004618 RID: 17944
	private int currentMagnetEntityTypeId = -1;

	// Token: 0x04004619 RID: 17945
	private int desiredMagnetEntityTypeId = -1;

	// Token: 0x0400461A RID: 17946
	private float prefabMagnetHeightOffset;

	// Token: 0x0400461B RID: 17947
	public float maxMagnetDistance = 0.75f;

	// Token: 0x0400461C RID: 17948
	private GRSpringMovement magnetMovement;

	// Token: 0x0400461D RID: 17949
	public GRSelectionWheel pageSelectionWheel;

	// Token: 0x0400461E RID: 17950
	public GameObject pageSelectionHandle;

	// Token: 0x0400461F RID: 17951
	public GameObject pageSelectionLever;

	// Token: 0x04004620 RID: 17952
	public float playerQueueTimeLimit = 30f;

	// Token: 0x04004621 RID: 17953
	private bool disablePurchaseButton;

	// Token: 0x04004622 RID: 17954
	private float purchaseButtonCooldown = 2f;

	// Token: 0x04004623 RID: 17955
	private float purchaseButtonPressed;

	// Token: 0x04004624 RID: 17956
	private const int ShelfIndex_None = -1;

	// Token: 0x04004626 RID: 17958
	public bool currentlyShowingText = true;

	// Token: 0x04004627 RID: 17959
	private List<GRToolProgressionManager.ToolParts> cachedRequiredPartsList = new List<GRToolProgressionManager.ToolParts>(5);

	// Token: 0x04004628 RID: 17960
	private float lastRequestedActivePlayerTokenTime;

	// Token: 0x04004629 RID: 17961
	private float requestActivePlayerTokenThrottleTime = 0.25f;

	// Token: 0x0400462A RID: 17962
	private bool bIsGrippingLeft;

	// Token: 0x0400462B RID: 17963
	private bool bIsGrippingRight;

	// Token: 0x0400462C RID: 17964
	private bool bGripLeftLastFrame;

	// Token: 0x0400462D RID: 17965
	private bool bGripRightLastFrame;

	// Token: 0x0400462E RID: 17966
	private float maxHandleRange = 0.09f;

	// Token: 0x0400462F RID: 17967
	private float timeSinceLastHandleBroadcast;

	// Token: 0x04004630 RID: 17968
	private float angleOfLastHandleBroadcast;

	// Token: 0x04004631 RID: 17969
	private float selectionWheelAngleOfLastBroadcast;

	// Token: 0x04004632 RID: 17970
	private float quantMult = 100000f;

	// Token: 0x04004633 RID: 17971
	private float lastHandleAngle = -10000f;

	// Token: 0x02000828 RID: 2088
	public enum ShelfMovementState
	{
		// Token: 0x04004635 RID: 17973
		Idle,
		// Token: 0x04004636 RID: 17974
		MoveCurrentShelfBackward,
		// Token: 0x04004637 RID: 17975
		MoveCurrentShelfForward,
		// Token: 0x04004638 RID: 17976
		MoveNextShelfUpward,
		// Token: 0x04004639 RID: 17977
		MoveNextShelfDownward,
		// Token: 0x0400463A RID: 17978
		Count
	}
}
