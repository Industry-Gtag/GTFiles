using System;
using System.Threading;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x0200057F RID: 1407
public class CosmeticWardrobe : MonoBehaviour
{
	// Token: 0x170003C6 RID: 966
	// (get) Token: 0x060023BD RID: 9149 RVA: 0x000C0377 File Offset: 0x000BE577
	// (set) Token: 0x060023BE RID: 9150 RVA: 0x000C037F File Offset: 0x000BE57F
	public bool UseTemporarySet
	{
		get
		{
			return this.m_useTemporarySet;
		}
		set
		{
			bool flag = value != this.m_useTemporarySet;
			this.m_useTemporarySet = value;
			if (flag)
			{
				this.HandleCosmeticsUpdated();
			}
		}
	}

	// Token: 0x060023BF RID: 9151 RVA: 0x000C039C File Offset: 0x000BE59C
	private void Start()
	{
		for (int i = 0; i < this.cosmeticCategoryButtons.Length; i++)
		{
			if (this.cosmeticCategoryButtons[i].category == CosmeticWardrobe.selectedCategory)
			{
				CosmeticWardrobe.selectedCategoryIndex = i;
				break;
			}
		}
		for (int j = 0; j < this.cosmeticCollectionDisplays.Length; j++)
		{
			this.cosmeticCollectionDisplays[j].displayHead.transform.localScale = this.startingHeadSize;
		}
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged += this.HandleLocalColorChanged;
			this.HandleLocalColorChanged(GorillaTagger.Instance.offlineVRRig.playerColor);
		}
		this.nextSelection.onPressed += this.HandlePressedNextSelection;
		this.prevSelection.onPressed += this.HandlePressedPrevSelection;
		for (int k = 0; k < this.cosmeticCollectionDisplays.Length; k++)
		{
			this.cosmeticCollectionDisplays[k].selectButton.onPressed += this.HandlePressedSelectCosmeticButton;
		}
		for (int l = 0; l < this.uniqueCosmeticButtons.Length; l++)
		{
			this.uniqueCosmeticButtons[l].onPressed += this.HandlePressedSelectCosmeticButtonUnique;
		}
		for (int m = 0; m < this.cosmeticCategoryButtons.Length; m++)
		{
			this.cosmeticCategoryButtons[m].button.onPressed += this.HandleChangeCategory;
			this.cosmeticCategoryButtons[m].slot1RemovedItem = CosmeticsController.instance.nullItem;
			this.cosmeticCategoryButtons[m].slot2RemovedItem = CosmeticsController.instance.nullItem;
		}
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnCosmeticsUpdated = (Action)Delegate.Combine(instance.OnCosmeticsUpdated, new Action(this.HandleCosmeticsUpdated));
		CosmeticsController instance2 = CosmeticsController.instance;
		instance2.OnOutfitsUpdated = (Action)Delegate.Combine(instance2.OnOutfitsUpdated, new Action(this.UpdateOutfitButtons));
		CosmeticWardrobe.OnWardrobeUpdateCategories = (Action)Delegate.Combine(CosmeticWardrobe.OnWardrobeUpdateCategories, new Action(this.UpdateCategoryButtons));
		CosmeticWardrobe.OnWardrobeUpdateDisplays = (Action)Delegate.Combine(CosmeticWardrobe.OnWardrobeUpdateDisplays, new Action(this.UpdateCosmeticDisplays));
		this.previousOutfit.onPressed += this.HandlePressedPrevOutfitButton;
		this.nextOutfit.onPressed += this.HandlePressedNextOutfitButton;
		this.HandleCosmeticsUpdated();
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x000C0610 File Offset: 0x000BE810
	private void OnDestroy()
	{
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged -= this.HandleLocalColorChanged;
		}
		this.nextSelection.onPressed -= this.HandlePressedNextSelection;
		this.prevSelection.onPressed -= this.HandlePressedPrevSelection;
		for (int i = 0; i < this.cosmeticCollectionDisplays.Length; i++)
		{
			this.cosmeticCollectionDisplays[i].selectButton.onPressed -= this.HandlePressedSelectCosmeticButton;
		}
		for (int j = 0; j < this.uniqueCosmeticButtons.Length; j++)
		{
			this.uniqueCosmeticButtons[j].onPressed -= this.HandlePressedSelectCosmeticButtonUnique;
		}
		for (int k = 0; k < this.cosmeticCategoryButtons.Length; k++)
		{
			this.cosmeticCategoryButtons[k].button.onPressed -= this.HandleChangeCategory;
		}
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnCosmeticsUpdated = (Action)Delegate.Remove(instance.OnCosmeticsUpdated, new Action(this.HandleCosmeticsUpdated));
		CosmeticsController instance2 = CosmeticsController.instance;
		instance2.OnOutfitsUpdated = (Action)Delegate.Remove(instance2.OnOutfitsUpdated, new Action(this.UpdateOutfitButtons));
		CosmeticWardrobe.OnWardrobeUpdateCategories = (Action)Delegate.Remove(CosmeticWardrobe.OnWardrobeUpdateCategories, new Action(this.UpdateCategoryButtons));
		CosmeticWardrobe.OnWardrobeUpdateDisplays = (Action)Delegate.Remove(CosmeticWardrobe.OnWardrobeUpdateDisplays, new Action(this.UpdateCosmeticDisplays));
		this.previousOutfit.onPressed -= this.HandlePressedPrevOutfitButton;
		this.nextOutfit.onPressed -= this.HandlePressedNextOutfitButton;
	}

	// Token: 0x060023C1 RID: 9153 RVA: 0x000C07D0 File Offset: 0x000BE9D0
	private void HandlePressedNextSelection(GorillaPressableButton button, bool isLeft)
	{
		CosmeticWardrobe.startingDisplayIndex += this.cosmeticCollectionDisplays.Length;
		if (CosmeticWardrobe.startingDisplayIndex >= CosmeticsController.instance.GetCategorySize(CosmeticWardrobe.selectedCategory))
		{
			CosmeticWardrobe.startingDisplayIndex = 0;
		}
		Action onWardrobeUpdateDisplays = CosmeticWardrobe.OnWardrobeUpdateDisplays;
		if (onWardrobeUpdateDisplays == null)
		{
			return;
		}
		onWardrobeUpdateDisplays();
	}

	// Token: 0x060023C2 RID: 9154 RVA: 0x000C0820 File Offset: 0x000BEA20
	private void HandlePressedPrevSelection(GorillaPressableButton button, bool isLeft)
	{
		CosmeticWardrobe.startingDisplayIndex -= this.cosmeticCollectionDisplays.Length;
		if (CosmeticWardrobe.startingDisplayIndex < 0)
		{
			int categorySize = CosmeticsController.instance.GetCategorySize(CosmeticWardrobe.selectedCategory);
			int num;
			if (categorySize % this.cosmeticCollectionDisplays.Length == 0)
			{
				num = categorySize - this.cosmeticCollectionDisplays.Length;
			}
			else
			{
				num = categorySize / this.cosmeticCollectionDisplays.Length;
				num *= this.cosmeticCollectionDisplays.Length;
			}
			CosmeticWardrobe.startingDisplayIndex = num;
		}
		Action onWardrobeUpdateDisplays = CosmeticWardrobe.OnWardrobeUpdateDisplays;
		if (onWardrobeUpdateDisplays == null)
		{
			return;
		}
		onWardrobeUpdateDisplays();
	}

	// Token: 0x060023C3 RID: 9155 RVA: 0x000C08A0 File Offset: 0x000BEAA0
	private async void RepressButton(GorillaPressableButton button, bool isLeft, string itemName)
	{
		float startTime = Time.time;
		float maxTime = 5f;
		while (VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(itemName) == null)
		{
			if (Time.time > startTime + maxTime)
			{
				return;
			}
			await Awaitable.NextFrameAsync(default(CancellationToken));
		}
		this.HandlePressedSelectCosmeticButton(button, isLeft);
	}

	// Token: 0x060023C4 RID: 9156 RVA: 0x000C08F0 File Offset: 0x000BEAF0
	private void HandlePressedSelectCosmeticButton(GorillaPressableButton button, bool isLeft)
	{
		for (int i = 0; i < this.cosmeticCollectionDisplays.Length; i++)
		{
			if (this.cosmeticCollectionDisplays[i].selectButton == button)
			{
				if (string.IsNullOrEmpty(this.cosmeticCollectionDisplays[i].currentCosmeticItem.itemName) || this.cosmeticCollectionDisplays[i].currentCosmeticItem.itemName == "NOTHING")
				{
					return;
				}
				if (VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(this.cosmeticCollectionDisplays[i].currentCosmeticItem.itemName) == null)
				{
					this.RepressButton(button, isLeft, this.cosmeticCollectionDisplays[i].currentCosmeticItem.itemName);
				}
				else
				{
					CosmeticsController.instance.PressWardrobeItemButton(this.cosmeticCollectionDisplays[i].currentCosmeticItem, isLeft, this.m_useTemporarySet);
					if (isLeft)
					{
						this.cosmeticCategoryButtons[CosmeticWardrobe.selectedCategoryIndex].slot2RemovedItem = CosmeticsController.instance.nullItem;
						return;
					}
					this.cosmeticCategoryButtons[CosmeticWardrobe.selectedCategoryIndex].slot1RemovedItem = CosmeticsController.instance.nullItem;
					return;
				}
			}
		}
	}

	// Token: 0x060023C5 RID: 9157 RVA: 0x000C0A04 File Offset: 0x000BEC04
	private async void RepressUniqueButton(GorillaPressableButton button, bool isLeft, string itemName)
	{
		float startTime = Time.time;
		float maxTime = 5f;
		while (VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(itemName) == null)
		{
			if (Time.time > startTime + maxTime)
			{
				return;
			}
			await Awaitable.NextFrameAsync(default(CancellationToken));
		}
		this.HandlePressedSelectCosmeticButtonUnique(button, isLeft);
	}

	// Token: 0x060023C6 RID: 9158 RVA: 0x000C0A54 File Offset: 0x000BEC54
	private void HandlePressedSelectCosmeticButtonUnique(GorillaPressableButton button, bool isLeft)
	{
		for (int i = 0; i < this.uniqueCosmeticButtons.Length; i++)
		{
			if (this.uniqueCosmeticButtons[i] == button)
			{
				if (string.IsNullOrEmpty(this.uniqueCosmeticButtons[i].SetCosmeticItemID) || this.uniqueCosmeticButtons[i].SetCosmeticItemID == "NOTHING")
				{
					return;
				}
				if (VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(this.uniqueCosmeticButtons[i].SetCosmeticItemID) == null)
				{
					Debug.LogWarning("Could not find cosmetic " + this.uniqueCosmeticButtons[i].SetCosmeticItemID + " in cosmetic registry!");
					this.RepressUniqueButton(button, isLeft, this.uniqueCosmeticButtons[i].SetCosmeticItemID);
				}
				else
				{
					CosmeticsController.CosmeticItem cosmeticItem;
					if (CosmeticsController.instance.allCosmeticsDict.TryGetValue(this.uniqueCosmeticButtons[i].SetCosmeticItemID, out cosmeticItem))
					{
						CosmeticsController.instance.PressWardrobeItemButton(cosmeticItem, isLeft, this.m_useTemporarySet);
						return;
					}
					Debug.LogWarning("Could not find cosmetic in dictionary named " + this.uniqueCosmeticButtons[i].SetCosmeticItemID + "!");
					return;
				}
			}
		}
	}

	// Token: 0x060023C7 RID: 9159 RVA: 0x000C0B6C File Offset: 0x000BED6C
	private void HandleChangeCategory(GorillaPressableButton button, bool isLeft)
	{
		for (int i = 0; i < this.cosmeticCategoryButtons.Length; i++)
		{
			CosmeticWardrobe.CosmeticWardrobeCategory cosmeticWardrobeCategory = this.cosmeticCategoryButtons[i];
			if (cosmeticWardrobeCategory.button == button)
			{
				if (CosmeticWardrobe.selectedCategory == cosmeticWardrobeCategory.category)
				{
					CosmeticsController.CosmeticItem cosmeticItem = CosmeticsController.instance.nullItem;
					if (cosmeticWardrobeCategory.slot1 != CosmeticsController.CosmeticSlots.Count)
					{
						cosmeticItem = CosmeticsController.instance.GetSlotItem(cosmeticWardrobeCategory.slot1, true, this.m_useTemporarySet);
					}
					CosmeticsController.CosmeticItem cosmeticItem2 = CosmeticsController.instance.nullItem;
					if (cosmeticWardrobeCategory.slot2 != CosmeticsController.CosmeticSlots.Count)
					{
						cosmeticItem2 = CosmeticsController.instance.GetSlotItem(cosmeticWardrobeCategory.slot2, true, this.m_useTemporarySet);
					}
					bool flag = CosmeticWardrobe.selectedCategory == CosmeticsController.CosmeticCategory.Arms;
					if (!cosmeticItem.isNullItem || !cosmeticItem2.isNullItem)
					{
						if (!cosmeticItem.isNullItem)
						{
							cosmeticWardrobeCategory.slot1RemovedItem = cosmeticItem;
							CosmeticsController.instance.PressWardrobeItemButton(cosmeticItem, flag, this.m_useTemporarySet);
						}
						if (!cosmeticItem2.isNullItem)
						{
							cosmeticWardrobeCategory.slot2RemovedItem = cosmeticItem2;
							CosmeticsController.instance.PressWardrobeItemButton(cosmeticItem2, !flag, this.m_useTemporarySet);
						}
						Action onWardrobeUpdateDisplays = CosmeticWardrobe.OnWardrobeUpdateDisplays;
						if (onWardrobeUpdateDisplays != null)
						{
							onWardrobeUpdateDisplays();
						}
						Action onWardrobeUpdateCategories = CosmeticWardrobe.OnWardrobeUpdateCategories;
						if (onWardrobeUpdateCategories == null)
						{
							return;
						}
						onWardrobeUpdateCategories();
						return;
					}
					else if (!cosmeticWardrobeCategory.slot1RemovedItem.isNullItem || !cosmeticWardrobeCategory.slot2RemovedItem.isNullItem)
					{
						if (!cosmeticWardrobeCategory.slot1RemovedItem.isNullItem)
						{
							CosmeticsController.instance.PressWardrobeItemButton(cosmeticWardrobeCategory.slot1RemovedItem, flag, this.m_useTemporarySet);
							cosmeticWardrobeCategory.slot1RemovedItem = CosmeticsController.instance.nullItem;
						}
						if (!cosmeticWardrobeCategory.slot2RemovedItem.isNullItem)
						{
							CosmeticsController.instance.PressWardrobeItemButton(cosmeticWardrobeCategory.slot2RemovedItem, !flag, this.m_useTemporarySet);
							cosmeticWardrobeCategory.slot2RemovedItem = CosmeticsController.instance.nullItem;
						}
						Action onWardrobeUpdateDisplays2 = CosmeticWardrobe.OnWardrobeUpdateDisplays;
						if (onWardrobeUpdateDisplays2 != null)
						{
							onWardrobeUpdateDisplays2();
						}
						Action onWardrobeUpdateCategories2 = CosmeticWardrobe.OnWardrobeUpdateCategories;
						if (onWardrobeUpdateCategories2 == null)
						{
							return;
						}
						onWardrobeUpdateCategories2();
						return;
					}
				}
				else
				{
					CosmeticWardrobe.selectedCategory = cosmeticWardrobeCategory.category;
					CosmeticWardrobe.selectedCategoryIndex = i;
					CosmeticWardrobe.startingDisplayIndex = 0;
					Action onWardrobeUpdateDisplays3 = CosmeticWardrobe.OnWardrobeUpdateDisplays;
					if (onWardrobeUpdateDisplays3 != null)
					{
						onWardrobeUpdateDisplays3();
					}
					Action onWardrobeUpdateCategories3 = CosmeticWardrobe.OnWardrobeUpdateCategories;
					if (onWardrobeUpdateCategories3 == null)
					{
						return;
					}
					onWardrobeUpdateCategories3();
				}
				return;
			}
		}
	}

	// Token: 0x060023C8 RID: 9160 RVA: 0x000C0D90 File Offset: 0x000BEF90
	private void HandleCosmeticsUpdated()
	{
		string[] currentlyWornCosmetics = CosmeticsController.instance.GetCurrentlyWornCosmetics(this.m_useTemporarySet);
		bool[] currentRightEquippedSided = CosmeticsController.instance.GetCurrentRightEquippedSided(this.m_useTemporarySet);
		this.currentEquippedDisplay.SetCosmeticActiveArray(currentlyWornCosmetics, currentRightEquippedSided);
		this.UpdateCategoryButtons();
		this.UpdateCosmeticDisplays();
		this.UpdateOutfitButtons();
	}

	// Token: 0x060023C9 RID: 9161 RVA: 0x000C0DE4 File Offset: 0x000BEFE4
	private void HandleLocalColorChanged(Color newColor)
	{
		MeshRenderer component = this.currentEquippedDisplay.GetComponent<MeshRenderer>();
		if (component != null)
		{
			component.material.color = newColor;
		}
	}

	// Token: 0x060023CA RID: 9162 RVA: 0x000C0E12 File Offset: 0x000BF012
	private void HandlePressedPrevOutfitButton(GorillaPressableButton button, bool isLeft)
	{
		CosmeticsController.instance.PressWardrobeScrollOutfit(false);
	}

	// Token: 0x060023CB RID: 9163 RVA: 0x000C0E21 File Offset: 0x000BF021
	private void HandlePressedNextOutfitButton(GorillaPressableButton button, bool isLeft)
	{
		CosmeticsController.instance.PressWardrobeScrollOutfit(true);
	}

	// Token: 0x060023CC RID: 9164 RVA: 0x000C0E30 File Offset: 0x000BF030
	private void UpdateCosmeticDisplays()
	{
		for (int i = 0; i < this.cosmeticCollectionDisplays.Length; i++)
		{
			CosmeticsController.CosmeticItem cosmetic = CosmeticsController.instance.GetCosmetic(CosmeticWardrobe.selectedCategory, CosmeticWardrobe.startingDisplayIndex + i);
			CosmeticWardrobe.CosmeticWardrobeSelection cosmeticWardrobeSelection = this.cosmeticCollectionDisplays[i];
			cosmeticWardrobeSelection.currentCosmeticItem = cosmetic;
			cosmeticWardrobeSelection.displayHead.SetCosmeticActive(cosmetic.displayName, false);
			cosmeticWardrobeSelection.selectButton.enabled = !cosmetic.isNullItem;
			cosmeticWardrobeSelection.selectButton.isOn = !cosmetic.isNullItem && CosmeticsController.instance.IsCosmeticEquipped(cosmetic, this.m_useTemporarySet);
			cosmeticWardrobeSelection.selectButton.UpdateColor();
		}
		int categorySize = CosmeticsController.instance.GetCategorySize(CosmeticWardrobe.selectedCategory);
		this.nextSelection.enabled = categorySize > this.cosmeticCollectionDisplays.Length;
		this.nextSelection.UpdateColor();
		this.prevSelection.enabled = categorySize > this.cosmeticCollectionDisplays.Length;
		this.prevSelection.UpdateColor();
	}

	// Token: 0x060023CD RID: 9165 RVA: 0x000C0F2C File Offset: 0x000BF12C
	private void UpdateCategoryButtons()
	{
		for (int i = 0; i < this.cosmeticCategoryButtons.Length; i++)
		{
			CosmeticWardrobe.CosmeticWardrobeCategory cosmeticWardrobeCategory = this.cosmeticCategoryButtons[i];
			if (cosmeticWardrobeCategory.slot1 != CosmeticsController.CosmeticSlots.Count)
			{
				CosmeticsController.CosmeticItem slotItem = CosmeticsController.instance.GetSlotItem(cosmeticWardrobeCategory.slot1, false, this.m_useTemporarySet);
				if (cosmeticWardrobeCategory.slot2 != CosmeticsController.CosmeticSlots.Count)
				{
					CosmeticsController.CosmeticItem slotItem2 = CosmeticsController.instance.GetSlotItem(cosmeticWardrobeCategory.slot2, false, this.m_useTemporarySet);
					if (slotItem.bothHandsHoldable)
					{
						cosmeticWardrobeCategory.button.SetIcon(slotItem.isNullItem ? null : slotItem.itemPicture);
					}
					else if (slotItem2.bothHandsHoldable)
					{
						cosmeticWardrobeCategory.button.SetIcon(slotItem2.isNullItem ? null : slotItem2.itemPicture);
					}
					else
					{
						cosmeticWardrobeCategory.button.SetDualIcon(slotItem.isNullItem ? null : slotItem.itemPicture, slotItem2.isNullItem ? null : slotItem2.itemPicture);
					}
				}
				else
				{
					cosmeticWardrobeCategory.button.SetIcon(slotItem.isNullItem ? null : slotItem.itemPicture);
				}
			}
			int categorySize = CosmeticsController.instance.GetCategorySize(cosmeticWardrobeCategory.category);
			cosmeticWardrobeCategory.button.enabled = categorySize > 0;
			cosmeticWardrobeCategory.button.isOn = CosmeticWardrobe.selectedCategory == cosmeticWardrobeCategory.category;
			cosmeticWardrobeCategory.button.UpdateColor();
		}
	}

	// Token: 0x060023CE RID: 9166 RVA: 0x000C108C File Offset: 0x000BF28C
	private void UpdateOutfitButtons()
	{
		bool flag = CosmeticsController.CanScrollOutfits();
		int num = CosmeticsController.SelectedOutfit + 1;
		this.nextOutfit.enabled = flag;
		this.previousOutfit.enabled = flag;
		this.nextOutfit.UpdateColor();
		this.previousOutfit.UpdateColor();
		this.outfitText.text = "Outfit #" + num.ToString();
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x000C10F4 File Offset: 0x000BF2F4
	public bool WardrobeButtonsInitialized()
	{
		for (int i = 0; i < this.cosmeticCategoryButtons.Length; i++)
		{
			if (!this.cosmeticCategoryButtons[i].button.Initialized)
			{
				return false;
			}
		}
		for (int i = 0; i < this.cosmeticCollectionDisplays.Length; i++)
		{
			if (!this.cosmeticCollectionDisplays[i].selectButton.Initialized)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04002EF3 RID: 12019
	[SerializeField]
	private CosmeticWardrobe.CosmeticWardrobeSelection[] cosmeticCollectionDisplays;

	// Token: 0x04002EF4 RID: 12020
	[SerializeField]
	private CosmeticButton[] uniqueCosmeticButtons;

	// Token: 0x04002EF5 RID: 12021
	[SerializeField]
	private CosmeticWardrobe.CosmeticWardrobeCategory[] cosmeticCategoryButtons;

	// Token: 0x04002EF6 RID: 12022
	[SerializeField]
	private HeadModel currentEquippedDisplay;

	// Token: 0x04002EF7 RID: 12023
	[SerializeField]
	private GorillaPressableButton nextSelection;

	// Token: 0x04002EF8 RID: 12024
	[SerializeField]
	private GorillaPressableButton prevSelection;

	// Token: 0x04002EF9 RID: 12025
	[SerializeField]
	private bool m_useTemporarySet;

	// Token: 0x04002EFA RID: 12026
	[SerializeField]
	private CosmeticButton previousOutfit;

	// Token: 0x04002EFB RID: 12027
	[SerializeField]
	private CosmeticButton nextOutfit;

	// Token: 0x04002EFC RID: 12028
	[SerializeField]
	private TMP_Text outfitText;

	// Token: 0x04002EFD RID: 12029
	private static int selectedCategoryIndex = 0;

	// Token: 0x04002EFE RID: 12030
	private static CosmeticsController.CosmeticCategory selectedCategory = CosmeticsController.CosmeticCategory.Hat;

	// Token: 0x04002EFF RID: 12031
	private static int startingDisplayIndex = 0;

	// Token: 0x04002F00 RID: 12032
	private static int selectedOutfitIndex = 0;

	// Token: 0x04002F01 RID: 12033
	private static Action OnWardrobeUpdateCategories;

	// Token: 0x04002F02 RID: 12034
	private static Action OnWardrobeUpdateDisplays;

	// Token: 0x04002F03 RID: 12035
	public Vector3 startingHeadSize = new Vector3(0.25f, 0.25f, 0.25f);

	// Token: 0x02000580 RID: 1408
	[Serializable]
	public class CosmeticWardrobeSelection
	{
		// Token: 0x04002F04 RID: 12036
		public HeadModel displayHead;

		// Token: 0x04002F05 RID: 12037
		public CosmeticButton selectButton;

		// Token: 0x04002F06 RID: 12038
		public CosmeticsController.CosmeticItem currentCosmeticItem;
	}

	// Token: 0x02000581 RID: 1409
	[Serializable]
	public class CosmeticWardrobeCategory
	{
		// Token: 0x04002F07 RID: 12039
		public CosmeticCategoryButton button;

		// Token: 0x04002F08 RID: 12040
		public CosmeticsController.CosmeticCategory category;

		// Token: 0x04002F09 RID: 12041
		public CosmeticsController.CosmeticSlots slot1 = CosmeticsController.CosmeticSlots.Count;

		// Token: 0x04002F0A RID: 12042
		public CosmeticsController.CosmeticSlots slot2 = CosmeticsController.CosmeticSlots.Count;

		// Token: 0x04002F0B RID: 12043
		public CosmeticsController.CosmeticItem slot1RemovedItem;

		// Token: 0x04002F0C RID: 12044
		public CosmeticsController.CosmeticItem slot2RemovedItem;
	}
}
