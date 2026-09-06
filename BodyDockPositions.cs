using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200051E RID: 1310
public class BodyDockPositions : MonoBehaviour
{
	// Token: 0x17000388 RID: 904
	// (get) Token: 0x060020AB RID: 8363 RVA: 0x000AF5D3 File Offset: 0x000AD7D3
	// (set) Token: 0x060020AC RID: 8364 RVA: 0x000AF5DB File Offset: 0x000AD7DB
	public TransferrableObject[] allObjects
	{
		get
		{
			return this._allObjects;
		}
		set
		{
			this._allObjects = value;
		}
	}

	// Token: 0x060020AD RID: 8365 RVA: 0x000AF5E4 File Offset: 0x000AD7E4
	public void Awake()
	{
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x060020AE RID: 8366 RVA: 0x000AF61C File Offset: 0x000AD81C
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		if (object.Equals(this.myRig.creator, otherPlayer))
		{
			this.DeallocateSharableInstances();
		}
	}

	// Token: 0x060020AF RID: 8367 RVA: 0x000AF637 File Offset: 0x000AD837
	public void OnLeftRoom()
	{
		this.DeallocateSharableInstances();
	}

	// Token: 0x060020B0 RID: 8368 RVA: 0x000AF640 File Offset: 0x000AD840
	public WorldShareableItem AllocateSharableInstance(BodyDockPositions.DropPositions position, NetPlayer owner)
	{
		switch (position)
		{
		case BodyDockPositions.DropPositions.None:
		case BodyDockPositions.DropPositions.LeftArm:
		case BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.MaxDropPostions:
		case BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
			break;
		case BodyDockPositions.DropPositions.LeftBack:
			if (this.leftBackSharableItem == null)
			{
				this.leftBackSharableItem = ObjectPools.instance.Instantiate(this.SharableItemInstance, true).GetComponent<WorldShareableItem>();
				this.leftBackSharableItem.GetComponent<RequestableOwnershipGuard>().SetOwnership(owner, false, true);
				this.leftBackSharableItem.GetComponent<WorldShareableItem>().SetupSharableViewIDs(owner, 3);
			}
			return this.leftBackSharableItem;
		default:
			if (position == BodyDockPositions.DropPositions.RightBack)
			{
				if (this.rightBackShareableItem == null)
				{
					this.rightBackShareableItem = ObjectPools.instance.Instantiate(this.SharableItemInstance, true).GetComponent<WorldShareableItem>();
					this.rightBackShareableItem.GetComponent<RequestableOwnershipGuard>().SetOwnership(owner, false, true);
					this.rightBackShareableItem.GetComponent<WorldShareableItem>().SetupSharableViewIDs(owner, 4);
				}
				return this.rightBackShareableItem;
			}
			if (position != BodyDockPositions.DropPositions.All)
			{
			}
			break;
		}
		throw new ArgumentOutOfRangeException("position", position, null);
	}

	// Token: 0x060020B1 RID: 8369 RVA: 0x000AF73C File Offset: 0x000AD93C
	public void DeallocateSharableInstance(WorldShareableItem worldShareable)
	{
		if (worldShareable == null)
		{
			return;
		}
		if (worldShareable == this.leftBackSharableItem)
		{
			if (this.leftBackSharableItem == null)
			{
				return;
			}
			this.leftBackSharableItem.ResetViews();
			ObjectPools.instance.Destroy(this.leftBackSharableItem.gameObject);
			this.leftBackSharableItem = null;
		}
		if (worldShareable == this.rightBackShareableItem)
		{
			if (this.rightBackShareableItem == null)
			{
				return;
			}
			this.rightBackShareableItem.ResetViews();
			ObjectPools.instance.Destroy(this.rightBackShareableItem.gameObject);
			this.rightBackShareableItem = null;
		}
	}

	// Token: 0x060020B2 RID: 8370 RVA: 0x000AF7D0 File Offset: 0x000AD9D0
	public void DeallocateSharableInstances()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.rightBackShareableItem != null)
		{
			this.rightBackShareableItem.ResetViews();
			ObjectPools.instance.Destroy(this.rightBackShareableItem.gameObject);
		}
		if (this.leftBackSharableItem != null)
		{
			this.leftBackSharableItem.ResetViews();
			ObjectPools.instance.Destroy(this.leftBackSharableItem.gameObject);
		}
		this.leftBackSharableItem = null;
		this.rightBackShareableItem = null;
	}

	// Token: 0x060020B3 RID: 8371 RVA: 0x000AF843 File Offset: 0x000ADA43
	public static bool IsPositionLeft(BodyDockPositions.DropPositions pos)
	{
		return pos == BodyDockPositions.DropPositions.LeftArm || pos == BodyDockPositions.DropPositions.LeftBack;
	}

	// Token: 0x060020B4 RID: 8372 RVA: 0x000AF850 File Offset: 0x000ADA50
	public int DropZoneStorageUsed(BodyDockPositions.DropPositions dropPosition)
	{
		if (this.myRig == null)
		{
			Debug.Log("BodyDockPositions lost reference to VR Rig, resetting it now", this);
			this.myRig = base.GetComponent<VRRig>();
		}
		if (this.myRig == null)
		{
			Debug.Log("Unable to reset reference");
			return -1;
		}
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) >= 0 && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)] != null && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].gameObject.activeInHierarchy && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].storedZone == dropPosition)
			{
				return this.myRig.ActiveTransferrableObjectIndex(i);
			}
		}
		return -1;
	}

	// Token: 0x060020B5 RID: 8373 RVA: 0x000AF928 File Offset: 0x000ADB28
	public TransferrableObject ItemPositionInUse(BodyDockPositions.DropPositions dropPosition)
	{
		TransferrableObject.PositionState positionState = this.MapDropPositionToState(dropPosition);
		if (this.myRig == null)
		{
			Debug.Log("BodyDockPositions lost reference to VR Rig, resetting it now", this);
			this.myRig = base.GetComponent<VRRig>();
		}
		if (this.myRig == null)
		{
			Debug.Log("Unable to reset reference");
			return null;
		}
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) != -1 && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].gameObject.activeInHierarchy && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].currentState == positionState)
			{
				return this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)];
			}
		}
		return null;
	}

	// Token: 0x060020B6 RID: 8374 RVA: 0x000AF9F0 File Offset: 0x000ADBF0
	private int EnableTransferrableItem(int allItemsIndex, BodyDockPositions.DropPositions startingPosition, TransferrableObject.PositionState startingState)
	{
		if (allItemsIndex < 0 || allItemsIndex >= this.allObjects.Length)
		{
			return -1;
		}
		if (this.myRig != null && this.myRig.isOfflineVRRig)
		{
			for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.myRig.ActiveTransferrableObjectIndex(i) == allItemsIndex)
				{
					this.DisableTransferrableItem(allItemsIndex);
				}
			}
			for (int j = 0; j < this.myRig.ActiveTransferrableObjectIndexLength(); j++)
			{
				if (this.myRig.ActiveTransferrableObjectIndex(j) == -1)
				{
					string itemNameFromDisplayName = CosmeticsController.instance.GetItemNameFromDisplayName(this.allObjects[allItemsIndex].gameObject.name);
					if (this.myRig.IsItemAllowed(itemNameFromDisplayName))
					{
						this.myRig.SetActiveTransferrableObjectIndex(j, allItemsIndex);
						this.myRig.SetTransferrablePosStates(j, startingState);
						this.myRig.SetTransferrableItemStates(j, (TransferrableObject.ItemStates)0);
						this.myRig.SetTransferrableDockPosition(j, startingPosition);
						this.EnableTransferrableGameObject(allItemsIndex, startingPosition, startingState);
						return j;
					}
				}
			}
		}
		return -1;
	}

	// Token: 0x060020B7 RID: 8375 RVA: 0x000AFAF0 File Offset: 0x000ADCF0
	public BodyDockPositions.DropPositions ItemActive(int allItemsIndex)
	{
		if (!this.allObjects[allItemsIndex].gameObject.activeSelf)
		{
			return BodyDockPositions.DropPositions.None;
		}
		return this.allObjects[allItemsIndex].storedZone;
	}

	// Token: 0x060020B8 RID: 8376 RVA: 0x000AFB18 File Offset: 0x000ADD18
	public static BodyDockPositions.DropPositions OfflineItemActive(int allItemsIndex)
	{
		if (GorillaTagger.Instance == null || GorillaTagger.Instance.offlineVRRig == null)
		{
			return BodyDockPositions.DropPositions.None;
		}
		BodyDockPositions component = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>();
		if (component == null)
		{
			return BodyDockPositions.DropPositions.None;
		}
		if (component.allObjects[allItemsIndex] == null || !component.allObjects[allItemsIndex].gameObject.activeSelf)
		{
			return BodyDockPositions.DropPositions.None;
		}
		return component.allObjects[allItemsIndex].storedZone;
	}

	// Token: 0x060020B9 RID: 8377 RVA: 0x000AFB98 File Offset: 0x000ADD98
	public void DisableTransferrableItem(int index)
	{
		TransferrableObject transferrableObject = this.allObjects[index];
		if (transferrableObject.gameObject.activeSelf)
		{
			transferrableObject.gameObject.Disable();
			transferrableObject.storedZone = BodyDockPositions.DropPositions.None;
		}
		if (this.myRig.isOfflineVRRig)
		{
			for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.myRig.ActiveTransferrableObjectIndex(i) == index)
				{
					this.myRig.SetActiveTransferrableObjectIndex(i, -1);
				}
			}
		}
	}

	// Token: 0x060020BA RID: 8378 RVA: 0x000AFC0C File Offset: 0x000ADE0C
	public void DisableAllTransferableItems()
	{
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			int num = this.myRig.ActiveTransferrableObjectIndex(i);
			if (num >= 0 && num < this.allObjects.Length)
			{
				TransferrableObject transferrableObject = this.allObjects[num];
				if (transferrableObject != null)
				{
					GameObject gameObject = transferrableObject.gameObject;
					if (gameObject != null)
					{
						gameObject.Disable();
					}
					transferrableObject.storedZone = BodyDockPositions.DropPositions.None;
				}
				this.myRig.SetActiveTransferrableObjectIndex(i, -1);
				this.myRig.SetTransferrableItemStates(i, (TransferrableObject.ItemStates)0);
				this.myRig.SetTransferrablePosStates(i, TransferrableObject.PositionState.None);
			}
		}
		this.DeallocateSharableInstances();
	}

	// Token: 0x060020BB RID: 8379 RVA: 0x000AFCA2 File Offset: 0x000ADEA2
	private bool AllItemsIndexValid(int allItemsIndex)
	{
		return allItemsIndex != -1 && allItemsIndex < this.allObjects.Length;
	}

	// Token: 0x060020BC RID: 8380 RVA: 0x000AFCB5 File Offset: 0x000ADEB5
	public bool PositionAvailable(int allItemIndex, BodyDockPositions.DropPositions startPos)
	{
		return (this.allObjects[allItemIndex].dockPositions & startPos) > BodyDockPositions.DropPositions.None;
	}

	// Token: 0x060020BD RID: 8381 RVA: 0x000AFCCC File Offset: 0x000ADECC
	public BodyDockPositions.DropPositions FirstAvailablePosition(int allItemIndex)
	{
		for (int i = 0; i < 5; i++)
		{
			BodyDockPositions.DropPositions dropPositions = (BodyDockPositions.DropPositions)(1 << i);
			if ((this.allObjects[allItemIndex].dockPositions & dropPositions) != BodyDockPositions.DropPositions.None)
			{
				return dropPositions;
			}
		}
		return BodyDockPositions.DropPositions.None;
	}

	// Token: 0x060020BE RID: 8382 RVA: 0x000AFD00 File Offset: 0x000ADF00
	public int TransferrableItemDisable(int allItemsIndex)
	{
		if (BodyDockPositions.OfflineItemActive(allItemsIndex) != BodyDockPositions.DropPositions.None)
		{
			this.DisableTransferrableItem(allItemsIndex);
		}
		return 0;
	}

	// Token: 0x060020BF RID: 8383 RVA: 0x000AFD14 File Offset: 0x000ADF14
	public void TransferrableItemDisableAtPosition(BodyDockPositions.DropPositions dropPositions)
	{
		int num = this.DropZoneStorageUsed(dropPositions);
		if (num >= 0)
		{
			this.TransferrableItemDisable(num);
		}
	}

	// Token: 0x060020C0 RID: 8384 RVA: 0x000AFD38 File Offset: 0x000ADF38
	public void TransferrableItemEnableAtPosition(string itemName, BodyDockPositions.DropPositions dropPosition)
	{
		if (this.DropZoneStorageUsed(dropPosition) >= 0)
		{
			return;
		}
		List<int> list = this.TransferrableObjectIndexFromName(itemName);
		if (list.Count == 0)
		{
			return;
		}
		TransferrableObject.PositionState positionState = this.MapDropPositionToState(dropPosition);
		if (list.Count == 1)
		{
			this.EnableTransferrableItem(list[0], dropPosition, positionState);
			return;
		}
		int num = (BodyDockPositions.IsPositionLeft(dropPosition) ? list[0] : list[1]);
		this.EnableTransferrableItem(num, dropPosition, positionState);
	}

	// Token: 0x060020C1 RID: 8385 RVA: 0x000AFDA8 File Offset: 0x000ADFA8
	public bool TransferrableItemActive(string transferrableItemName)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int num in list)
		{
			if (this.TransferrableItemActive(num))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060020C2 RID: 8386 RVA: 0x000AFE14 File Offset: 0x000AE014
	public bool TransferrableItemActiveAtPos(string transferrableItemName, BodyDockPositions.DropPositions dropPosition)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int num in list)
		{
			BodyDockPositions.DropPositions dropPositions = this.TransferrableItemPosition(num);
			if (dropPositions != BodyDockPositions.DropPositions.None && dropPositions == dropPosition)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060020C3 RID: 8387 RVA: 0x000AFE88 File Offset: 0x000AE088
	public bool TransferrableItemActive(int allItemsIndex)
	{
		return this.ItemActive(allItemsIndex) > BodyDockPositions.DropPositions.None;
	}

	// Token: 0x060020C4 RID: 8388 RVA: 0x000AFE94 File Offset: 0x000AE094
	public TransferrableObject TransferrableItem(int allItemsIndex)
	{
		return this.allObjects[allItemsIndex];
	}

	// Token: 0x060020C5 RID: 8389 RVA: 0x000AFE9E File Offset: 0x000AE09E
	public BodyDockPositions.DropPositions TransferrableItemPosition(int allItemsIndex)
	{
		return this.ItemActive(allItemsIndex);
	}

	// Token: 0x060020C6 RID: 8390 RVA: 0x000AFEA8 File Offset: 0x000AE0A8
	public bool DisableTransferrableItem(string transferrableItemName)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int num in list)
		{
			this.DisableTransferrableItem(num);
		}
		return true;
	}

	// Token: 0x060020C7 RID: 8391 RVA: 0x000AFF0C File Offset: 0x000AE10C
	public BodyDockPositions.DropPositions OppositePosition(BodyDockPositions.DropPositions pos)
	{
		if (pos == BodyDockPositions.DropPositions.LeftArm)
		{
			return BodyDockPositions.DropPositions.RightArm;
		}
		if (pos == BodyDockPositions.DropPositions.RightArm)
		{
			return BodyDockPositions.DropPositions.LeftArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftBack)
		{
			return BodyDockPositions.DropPositions.RightBack;
		}
		if (pos == BodyDockPositions.DropPositions.RightBack)
		{
			return BodyDockPositions.DropPositions.LeftBack;
		}
		return pos;
	}

	// Token: 0x060020C8 RID: 8392 RVA: 0x000AFF2C File Offset: 0x000AE12C
	public BodyDockPositions.DockingResult ToggleWithHandedness(string transferrableItemName, bool isLeftHand, bool bothHands)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return new BodyDockPositions.DockingResult();
		}
		if (!this.AllItemsIndexValid(list[0]))
		{
			return new BodyDockPositions.DockingResult();
		}
		BodyDockPositions.DropPositions dropPositions;
		if (isLeftHand)
		{
			dropPositions = (((this.allObjects[list[0]].dockPositions & BodyDockPositions.DropPositions.RightArm) != BodyDockPositions.DropPositions.None) ? BodyDockPositions.DropPositions.RightArm : BodyDockPositions.DropPositions.LeftBack);
		}
		else
		{
			dropPositions = (((this.allObjects[list[0]].dockPositions & BodyDockPositions.DropPositions.LeftArm) != BodyDockPositions.DropPositions.None) ? BodyDockPositions.DropPositions.LeftArm : BodyDockPositions.DropPositions.RightBack);
		}
		return this.ToggleTransferrableItem(transferrableItemName, dropPositions, bothHands);
	}

	// Token: 0x060020C9 RID: 8393 RVA: 0x000AFFAC File Offset: 0x000AE1AC
	public BodyDockPositions.DockingResult ToggleTransferrableItem(string transferrableItemName, BodyDockPositions.DropPositions startingPos, bool bothHands)
	{
		BodyDockPositions.DockingResult dockingResult = new BodyDockPositions.DockingResult();
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return dockingResult;
		}
		if (bothHands && list.Count == 2)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int num = list[i];
				BodyDockPositions.DropPositions dropPositions = BodyDockPositions.OfflineItemActive(num);
				if (dropPositions != BodyDockPositions.DropPositions.None)
				{
					this.TransferrableItemDisable(num);
					dockingResult.positionsDisabled.Add(dropPositions);
				}
			}
			if (dockingResult.positionsDisabled.Count >= 1)
			{
				return dockingResult;
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			int num2 = list[j];
			BodyDockPositions.DropPositions dropPositions2 = startingPos;
			if (bothHands && j != 0)
			{
				dropPositions2 = this.OppositePosition(dropPositions2);
			}
			if (!this.PositionAvailable(num2, dropPositions2))
			{
				dropPositions2 = this.FirstAvailablePosition(num2);
				if (dropPositions2 == BodyDockPositions.DropPositions.None)
				{
					return dockingResult;
				}
			}
			if (BodyDockPositions.OfflineItemActive(num2) == dropPositions2)
			{
				this.TransferrableItemDisable(num2);
				dockingResult.positionsDisabled.Add(dropPositions2);
			}
			else
			{
				this.TransferrableItemDisableAtPosition(dropPositions2);
				dockingResult.dockedPosition.Add(dropPositions2);
				TransferrableObject.PositionState positionState = this.MapDropPositionToState(dropPositions2);
				if (this.TransferrableItemActive(num2))
				{
					BodyDockPositions.DropPositions dropPositions3 = this.TransferrableItemPosition(num2);
					dockingResult.positionsDisabled.Add(dropPositions3);
					this.MoveTransferableItem(num2, dropPositions2, positionState);
				}
				else
				{
					this.EnableTransferrableItem(num2, dropPositions2, positionState);
				}
			}
		}
		return dockingResult;
	}

	// Token: 0x060020CA RID: 8394 RVA: 0x000B00F3 File Offset: 0x000AE2F3
	private void MoveTransferableItem(int allItemsIndex, BodyDockPositions.DropPositions newPosition, TransferrableObject.PositionState newPositionState)
	{
		this.allObjects[allItemsIndex].storedZone = newPosition;
		this.allObjects[allItemsIndex].currentState = newPositionState;
		this.allObjects[allItemsIndex].ResetToDefaultState();
	}

	// Token: 0x060020CB RID: 8395 RVA: 0x000B0120 File Offset: 0x000AE320
	public void EnableTransferrableGameObject(int allItemsIndex, BodyDockPositions.DropPositions dropZone, TransferrableObject.PositionState startingPosition)
	{
		if (this.allObjects[allItemsIndex] == null)
		{
			return;
		}
		GameObject gameObject = this.allObjects[allItemsIndex].gameObject;
		TransferrableObject component = gameObject.GetComponent<TransferrableObject>();
		if ((component.dockPositions & dropZone) == BodyDockPositions.DropPositions.None || !component.ValidateState(startingPosition))
		{
			gameObject.Disable();
			return;
		}
		this.MoveTransferableItem(allItemsIndex, dropZone, startingPosition);
		gameObject.SetActive(true);
		ProjectileWeapon component2;
		if ((component2 = gameObject.GetComponent<ProjectileWeapon>()) != null)
		{
			component2.enabled = true;
		}
	}

	// Token: 0x060020CC RID: 8396 RVA: 0x000B0194 File Offset: 0x000AE394
	public void RefreshTransferrableItems()
	{
		if (!this.myRig)
		{
			this.myRig = base.GetComponentInParent<VRRig>(true);
			if (!this.myRig)
			{
				Debug.LogError("BodyDockPositions.RefreshTransferrableItems: (should never happen) myRig is null and could not be found on same GameObject or parents. Path: " + base.transform.GetPathQ(), this);
			}
		}
		this.objectsToEnable.Clear();
		this.objectsToDisable.Clear();
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			int num = this.myRig.ActiveTransferrableObjectIndex(i);
			if (num != -1)
			{
				if (num < 0 || num >= this.allObjects.Length)
				{
					Debug.LogError(string.Format("Transferrable object index {0} out of range, expected [0..{1})", num, this.allObjects.Length));
				}
				else
				{
					TransferrableObject transferrableObject = this.allObjects[num];
					string text = ((transferrableObject != null) ? transferrableObject.gameObject.name : null);
					string itemNameFromDisplayName = CosmeticsController.instance.GetItemNameFromDisplayName(text);
					if (this.myRig.IsItemAllowed(itemNameFromDisplayName))
					{
						int num2 = this.myRig.ActiveTransferrableObjectIndex(i);
						if (!(this.allObjects[num2] == null))
						{
							if (this.allObjects[num2].gameObject.activeSelf)
							{
								this.allObjects[num2].objectIndex = i;
							}
							else
							{
								this.objectsToEnable.Add(i);
							}
						}
					}
				}
			}
		}
		for (int j = 0; j < this.allObjects.Length; j++)
		{
			if (this.allObjects[j] != null && this.allObjects[j].gameObject.activeSelf)
			{
				bool flag = true;
				for (int k = 0; k < this.myRig.ActiveTransferrableObjectIndexLength(); k++)
				{
					if (this.myRig.ActiveTransferrableObjectIndex(k) == j && this.myRig.IsItemAllowed(CosmeticsController.instance.GetItemNameFromDisplayName(this.allObjects[this.myRig.ActiveTransferrableObjectIndex(k)].gameObject.name)))
					{
						flag = false;
					}
				}
				if (flag)
				{
					this.objectsToDisable.Add(j);
				}
			}
		}
		foreach (int num3 in this.objectsToDisable)
		{
			this.DisableTransferrableItem(num3);
		}
		foreach (int num4 in this.objectsToEnable)
		{
			int num5 = this.myRig.ActiveTransferrableObjectIndex(num4);
			this.EnableTransferrableGameObject(num5, this.myRig.TransferrableDockPosition(num4), this.myRig.TransferrablePosStates(num4));
		}
		this.UpdateHandState();
	}

	// Token: 0x060020CD RID: 8397 RVA: 0x000B0464 File Offset: 0x000AE664
	public int ReturnTransferrableItemIndex(int allItemsIndex)
	{
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) == allItemsIndex)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060020CE RID: 8398 RVA: 0x000B049C File Offset: 0x000AE69C
	public List<int> TransferrableObjectIndexFromName(string transObjectName)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.allObjects.Length; i++)
		{
			if (!(this.allObjects[i] == null) && this.allObjects[i].gameObject.name == transObjectName)
			{
				list.Add(i);
			}
		}
		return list;
	}

	// Token: 0x060020CF RID: 8399 RVA: 0x000B04F4 File Offset: 0x000AE6F4
	private TransferrableObject.PositionState MapDropPositionToState(BodyDockPositions.DropPositions pos)
	{
		if (pos == BodyDockPositions.DropPositions.RightArm)
		{
			return TransferrableObject.PositionState.OnRightArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftArm)
		{
			return TransferrableObject.PositionState.OnLeftArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftBack)
		{
			return TransferrableObject.PositionState.OnLeftShoulder;
		}
		if (pos == BodyDockPositions.DropPositions.RightBack)
		{
			return TransferrableObject.PositionState.OnRightShoulder;
		}
		return TransferrableObject.PositionState.OnChest;
	}

	// Token: 0x17000389 RID: 905
	// (get) Token: 0x060020D0 RID: 8400 RVA: 0x000B0513 File Offset: 0x000AE713
	internal int PreviousLeftHandThrowableIndex
	{
		get
		{
			return this.throwableDisabledIndex[0];
		}
	}

	// Token: 0x1700038A RID: 906
	// (get) Token: 0x060020D1 RID: 8401 RVA: 0x000B051D File Offset: 0x000AE71D
	internal int PreviousRightHandThrowableIndex
	{
		get
		{
			return this.throwableDisabledIndex[1];
		}
	}

	// Token: 0x1700038B RID: 907
	// (get) Token: 0x060020D2 RID: 8402 RVA: 0x000B0527 File Offset: 0x000AE727
	internal float PreviousLeftHandThrowableDisabledTime
	{
		get
		{
			return this.throwableDisabledTime[0];
		}
	}

	// Token: 0x1700038C RID: 908
	// (get) Token: 0x060020D3 RID: 8403 RVA: 0x000B0531 File Offset: 0x000AE731
	internal float PreviousRightHandThrowableDisabledTime
	{
		get
		{
			return this.throwableDisabledTime[1];
		}
	}

	// Token: 0x060020D4 RID: 8404 RVA: 0x000B053C File Offset: 0x000AE73C
	private void UpdateHandState()
	{
		for (int i = 0; i < 2; i++)
		{
			GameObject[] array = ((i == 0) ? this.leftHandThrowables : this.rightHandThrowables);
			int num = ((i == 0) ? this.myRig.LeftThrowableProjectileIndex : this.myRig.RightThrowableProjectileIndex);
			string text;
			if (num > -1 && CosmeticsV2Spawner_Dirty.GetPlayfabIdFromThrowableIndex(i == 0, num, out text))
			{
				this.myRig.cosmeticsObjectRegistry.Cosmetic(text);
			}
			for (int j = 0; j < array.Length; j++)
			{
				GameObject gameObject = array[j];
				if (!(gameObject == null))
				{
					bool activeSelf = gameObject.activeSelf;
					bool flag = gameObject.GetComponent<SnowballThrowable>().throwableMakerIndex == num;
					array[j].SetActive(flag);
					if (activeSelf && !flag)
					{
						this.throwableDisabledIndex[i] = j;
						this.throwableDisabledTime[i] = Time.time + 0.02f;
					}
				}
			}
		}
	}

	// Token: 0x060020D5 RID: 8405 RVA: 0x000B0619 File Offset: 0x000AE819
	internal GameObject GetLeftHandThrowable()
	{
		return this.GetLeftHandThrowable(this.myRig.LeftThrowableProjectileIndex);
	}

	// Token: 0x060020D6 RID: 8406 RVA: 0x000B062C File Offset: 0x000AE82C
	internal GameObject GetLeftHandThrowable(int throwableIndex)
	{
		if (throwableIndex < 0 || throwableIndex >= this.leftHandThrowables.Length)
		{
			throwableIndex = this.PreviousLeftHandThrowableIndex;
			if (throwableIndex < 0 || throwableIndex >= this.leftHandThrowables.Length || this.PreviousLeftHandThrowableDisabledTime < Time.time)
			{
				return null;
			}
		}
		return this.leftHandThrowables[throwableIndex];
	}

	// Token: 0x060020D7 RID: 8407 RVA: 0x000B066B File Offset: 0x000AE86B
	internal GameObject GetRightHandThrowable()
	{
		return this.GetRightHandThrowable(this.myRig.RightThrowableProjectileIndex);
	}

	// Token: 0x060020D8 RID: 8408 RVA: 0x000B067E File Offset: 0x000AE87E
	internal GameObject GetRightHandThrowable(int throwableIndex)
	{
		if (throwableIndex < 0 || throwableIndex >= this.rightHandThrowables.Length)
		{
			throwableIndex = this.PreviousRightHandThrowableIndex;
			if (throwableIndex < 0 || throwableIndex >= this.rightHandThrowables.Length || this.PreviousRightHandThrowableDisabledTime < Time.time)
			{
				return null;
			}
		}
		return this.rightHandThrowables[throwableIndex];
	}

	// Token: 0x04002B82 RID: 11138
	public VRRig myRig;

	// Token: 0x04002B83 RID: 11139
	public GameObject[] leftHandThrowables;

	// Token: 0x04002B84 RID: 11140
	public GameObject[] rightHandThrowables;

	// Token: 0x04002B85 RID: 11141
	[FormerlySerializedAs("allObjects")]
	public TransferrableObject[] _allObjects;

	// Token: 0x04002B86 RID: 11142
	private List<int> objectsToEnable = new List<int>();

	// Token: 0x04002B87 RID: 11143
	private List<int> objectsToDisable = new List<int>();

	// Token: 0x04002B88 RID: 11144
	public Transform leftHandTransform;

	// Token: 0x04002B89 RID: 11145
	public Transform rightHandTransform;

	// Token: 0x04002B8A RID: 11146
	public Transform chestTransform;

	// Token: 0x04002B8B RID: 11147
	public Transform leftArmTransform;

	// Token: 0x04002B8C RID: 11148
	public Transform rightArmTransform;

	// Token: 0x04002B8D RID: 11149
	public Transform leftBackTransform;

	// Token: 0x04002B8E RID: 11150
	public Transform rightBackTransform;

	// Token: 0x04002B8F RID: 11151
	public WorldShareableItem leftBackSharableItem;

	// Token: 0x04002B90 RID: 11152
	public WorldShareableItem rightBackShareableItem;

	// Token: 0x04002B91 RID: 11153
	public GameObject SharableItemInstance;

	// Token: 0x04002B92 RID: 11154
	private int[] throwableDisabledIndex = new int[] { -1, -1 };

	// Token: 0x04002B93 RID: 11155
	private float[] throwableDisabledTime = new float[2];

	// Token: 0x0200051F RID: 1311
	[Flags]
	public enum DropPositions
	{
		// Token: 0x04002B95 RID: 11157
		LeftArm = 1,
		// Token: 0x04002B96 RID: 11158
		RightArm = 2,
		// Token: 0x04002B97 RID: 11159
		Chest = 4,
		// Token: 0x04002B98 RID: 11160
		LeftBack = 8,
		// Token: 0x04002B99 RID: 11161
		RightBack = 16,
		// Token: 0x04002B9A RID: 11162
		MaxDropPostions = 5,
		// Token: 0x04002B9B RID: 11163
		All = 31,
		// Token: 0x04002B9C RID: 11164
		None = 0
	}

	// Token: 0x02000520 RID: 1312
	public class DockingResult
	{
		// Token: 0x060020DA RID: 8410 RVA: 0x000B06FB File Offset: 0x000AE8FB
		public DockingResult()
		{
			this.dockedPosition = new List<BodyDockPositions.DropPositions>(2);
			this.positionsDisabled = new List<BodyDockPositions.DropPositions>(2);
		}

		// Token: 0x04002B9D RID: 11165
		public List<BodyDockPositions.DropPositions> positionsDisabled;

		// Token: 0x04002B9E RID: 11166
		public List<BodyDockPositions.DropPositions> dockedPosition;
	}
}
