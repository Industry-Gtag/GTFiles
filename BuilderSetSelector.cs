using System;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x0200066A RID: 1642
public class BuilderSetSelector : MonoBehaviour
{
	// Token: 0x060028F3 RID: 10483 RVA: 0x000DDC44 File Offset: 0x000DBE44
	private void Start()
	{
		this.zoneRenderers.Clear();
		foreach (GorillaPressableButton gorillaPressableButton in this.groupButtons)
		{
			this.zoneRenderers.Add(gorillaPressableButton.buttonRenderer);
			TMP_Text myTmpText = gorillaPressableButton.myTmpText;
			Renderer renderer = ((myTmpText != null) ? myTmpText.GetComponent<Renderer>() : null);
			if (renderer != null)
			{
				this.zoneRenderers.Add(renderer);
			}
		}
		this.zoneRenderers.Add(this.previousPageButton.buttonRenderer);
		this.zoneRenderers.Add(this.nextPageButton.buttonRenderer);
		TMP_Text myTmpText2 = this.previousPageButton.myTmpText;
		Renderer renderer2 = ((myTmpText2 != null) ? myTmpText2.GetComponent<Renderer>() : null);
		if (renderer2 != null)
		{
			this.zoneRenderers.Add(renderer2);
		}
		TMP_Text myTmpText3 = this.nextPageButton.myTmpText;
		renderer2 = ((myTmpText3 != null) ? myTmpText3.GetComponent<Renderer>() : null);
		if (renderer2 != null)
		{
			this.zoneRenderers.Add(renderer2);
		}
		foreach (Renderer renderer3 in this.zoneRenderers)
		{
			renderer3.enabled = false;
		}
		this.inBuilderZone = false;
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.OnZoneChanged();
	}

	// Token: 0x060028F4 RID: 10484 RVA: 0x000DDDB0 File Offset: 0x000DBFB0
	public void Setup(List<BuilderPieceSet.BuilderPieceCategory> categories)
	{
		List<BuilderPieceSet.BuilderDisplayGroup> liveDisplayGroups = BuilderSetManager.instance.GetLiveDisplayGroups();
		this.numLiveDisplayGroups = liveDisplayGroups.Count;
		this.includedGroups = new List<BuilderPieceSet.BuilderDisplayGroup>(liveDisplayGroups.Count);
		this._includedCategories = categories;
		foreach (BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup in liveDisplayGroups)
		{
			if (this.DoesDisplayGroupHaveIncludedCategories(builderDisplayGroup))
			{
				this.includedGroups.Add(builderDisplayGroup);
			}
		}
		BuilderSetManager.instance.OnOwnedSetsUpdated.AddListener(new UnityAction(this.RefreshUnlockedGroups));
		BuilderSetManager.instance.OnLiveSetsUpdated.AddListener(new UnityAction(this.RefreshUnlockedGroups));
		this.groupsPerPage = this.groupButtons.Length;
		this.totalPages = this.includedGroups.Count / this.groupsPerPage;
		if (this.includedGroups.Count % this.groupsPerPage > 0)
		{
			this.totalPages++;
		}
		this.previousPageButton.gameObject.SetActive(this.totalPages > 1);
		this.nextPageButton.gameObject.SetActive(this.totalPages > 1);
		this.previousPageButton.myTmpText.enabled = this.totalPages > 1;
		this.nextPageButton.myTmpText.enabled = this.totalPages > 1;
		this.pageIndex = 0;
		this.currentGroup = this.includedGroups[this.includedGroupIndex];
		this.previousPageButton.onPressButton.AddListener(new UnityAction(this.OnPreviousPageClicked));
		this.nextPageButton.onPressButton.AddListener(new UnityAction(this.OnNextPageClicked));
		GorillaPressableButton[] array = this.groupButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].onPressed += this.OnSetButtonPressed;
		}
		this.UpdateLabels();
	}

	// Token: 0x060028F5 RID: 10485 RVA: 0x000DDFB0 File Offset: 0x000DC1B0
	private void OnDestroy()
	{
		if (this.previousPageButton != null)
		{
			this.previousPageButton.onPressButton.RemoveListener(new UnityAction(this.OnPreviousPageClicked));
		}
		if (this.nextPageButton != null)
		{
			this.nextPageButton.onPressButton.RemoveListener(new UnityAction(this.OnNextPageClicked));
		}
		if (BuilderSetManager.instance != null)
		{
			BuilderSetManager.instance.OnOwnedSetsUpdated.RemoveListener(new UnityAction(this.RefreshUnlockedGroups));
			BuilderSetManager.instance.OnLiveSetsUpdated.RemoveListener(new UnityAction(this.RefreshUnlockedGroups));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.groupButtons)
		{
			if (!(gorillaPressableButton == null))
			{
				gorillaPressableButton.onPressed -= this.OnSetButtonPressed;
			}
		}
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x060028F6 RID: 10486 RVA: 0x000DE0C4 File Offset: 0x000DC2C4
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
		if (flag && !this.inBuilderZone)
		{
			using (List<Renderer>.Enumerator enumerator = this.zoneRenderers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Renderer renderer = enumerator.Current;
					renderer.enabled = true;
				}
				goto IL_008B;
			}
		}
		if (!flag && this.inBuilderZone)
		{
			foreach (Renderer renderer2 in this.zoneRenderers)
			{
				renderer2.enabled = false;
			}
		}
		IL_008B:
		this.inBuilderZone = flag;
	}

	// Token: 0x060028F7 RID: 10487 RVA: 0x000DE180 File Offset: 0x000DC380
	private void OnSetButtonPressed(GorillaPressableButton button, bool isLeft)
	{
		int num = 0;
		for (int i = 0; i < this.groupButtons.Length; i++)
		{
			if (button.Equals(this.groupButtons[i]))
			{
				num = i;
				break;
			}
		}
		int num2 = this.pageIndex * this.groupsPerPage + num;
		if (num2 < this.includedGroups.Count)
		{
			BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup = this.includedGroups[num2];
			if (this.currentGroup == null || builderDisplayGroup.displayName != this.currentGroup.displayName)
			{
				UnityEvent<int> onSelectedGroup = this.OnSelectedGroup;
				if (onSelectedGroup == null)
				{
					return;
				}
				onSelectedGroup.Invoke(builderDisplayGroup.GetDisplayGroupIdentifier());
			}
		}
	}

	// Token: 0x060028F8 RID: 10488 RVA: 0x000DE218 File Offset: 0x000DC418
	private void RefreshUnlockedGroups()
	{
		List<BuilderPieceSet.BuilderDisplayGroup> liveDisplayGroups = BuilderSetManager.instance.GetLiveDisplayGroups();
		if (liveDisplayGroups.Count != this.numLiveDisplayGroups)
		{
			string text = ((this.currentGroup != null) ? this.currentGroup.displayName : "");
			this.numLiveDisplayGroups = liveDisplayGroups.Count;
			this.includedGroups.EnsureCapacity(this.numLiveDisplayGroups);
			this.includedGroups.Clear();
			int num = 0;
			foreach (BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup in liveDisplayGroups)
			{
				if (this.DoesDisplayGroupHaveIncludedCategories(builderDisplayGroup))
				{
					if (builderDisplayGroup.displayName.Equals(text))
					{
						num = this.includedGroups.Count;
					}
					this.includedGroups.Add(builderDisplayGroup);
				}
			}
			if (this.includedGroups.Count < 1)
			{
				this.currentGroup = null;
			}
			else
			{
				this.includedGroupIndex = num;
				this.currentGroup = this.includedGroups[this.includedGroupIndex];
			}
			this.totalPages = this.includedGroups.Count / this.groupsPerPage;
			if (this.includedGroups.Count % this.groupsPerPage > 0)
			{
				this.totalPages++;
			}
			this.previousPageButton.gameObject.SetActive(this.totalPages > 1);
			this.nextPageButton.gameObject.SetActive(this.totalPages > 1);
			this.previousPageButton.myTmpText.enabled = this.totalPages > 1;
			this.nextPageButton.myTmpText.enabled = this.totalPages > 1;
		}
		this.UpdateLabels();
	}

	// Token: 0x060028F9 RID: 10489 RVA: 0x000DE3D0 File Offset: 0x000DC5D0
	private void OnPreviousPageClicked()
	{
		this.RefreshUnlockedGroups();
		int num = Mathf.Clamp(this.pageIndex - 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x060028FA RID: 10490 RVA: 0x000DE410 File Offset: 0x000DC610
	private void OnNextPageClicked()
	{
		this.RefreshUnlockedGroups();
		int num = Mathf.Clamp(this.pageIndex + 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x060028FB RID: 10491 RVA: 0x000DE450 File Offset: 0x000DC650
	public void SetSelection(int groupID)
	{
		if (BuilderSetManager.instance == null)
		{
			return;
		}
		BuilderPieceSet.BuilderDisplayGroup newGroup = BuilderSetManager.instance.GetDisplayGroupFromIndex(groupID);
		if (newGroup == null)
		{
			return;
		}
		this.currentGroup = newGroup;
		this.includedGroupIndex = this.includedGroups.FindIndex((BuilderPieceSet.BuilderDisplayGroup x) => x.displayName == newGroup.displayName);
		this.UpdateLabels();
	}

	// Token: 0x060028FC RID: 10492 RVA: 0x000DE4C0 File Offset: 0x000DC6C0
	private void UpdateLabels()
	{
		for (int i = 0; i < this.groupLabels.Length; i++)
		{
			int num = this.pageIndex * this.groupsPerPage + i;
			if (num < this.includedGroups.Count && this.includedGroups[num] != null)
			{
				if (!this.groupButtons[i].gameObject.activeSelf)
				{
					this.groupButtons[i].gameObject.SetActive(true);
					this.groupButtons[i].myTmpText.gameObject.SetActive(true);
				}
				if (this.groupButtons[i].myTmpText.text != this.includedGroups[num].displayName)
				{
					this.groupButtons[i].myTmpText.text = this.includedGroups[num].displayName;
				}
				if (BuilderSetManager.instance.IsPieceSetOwnedLocally(this.includedGroups[num].setID))
				{
					bool flag = this.currentGroup != null && this.includedGroups[num].displayName == this.currentGroup.displayName;
					if (flag != this.groupButtons[i].isOn || !this.groupButtons[i].enabled)
					{
						this.groupButtons[i].isOn = flag;
						this.groupButtons[i].buttonRenderer.material = (flag ? this.groupButtons[i].pressedMaterial : this.groupButtons[i].unpressedMaterial);
					}
					this.groupButtons[i].enabled = true;
				}
				else
				{
					if (this.groupButtons[i].enabled)
					{
						this.groupButtons[i].buttonRenderer.material = this.disabledMaterial;
					}
					this.groupButtons[i].enabled = false;
				}
			}
			else
			{
				if (this.groupButtons[i].gameObject.activeSelf)
				{
					this.groupButtons[i].gameObject.SetActive(false);
					this.groupButtons[i].myTmpText.gameObject.SetActive(false);
				}
				if (this.groupButtons[i].isOn || this.groupButtons[i].enabled)
				{
					this.groupButtons[i].isOn = false;
					this.groupButtons[i].enabled = false;
				}
			}
		}
		bool flag2 = this.pageIndex > 0 && this.totalPages > 1;
		bool flag3 = this.pageIndex < this.totalPages - 1 && this.totalPages > 1;
		if (this.previousPageButton.myTmpText.enabled != flag2)
		{
			this.previousPageButton.myTmpText.enabled = flag2;
		}
		if (this.nextPageButton.myTmpText.enabled != flag3)
		{
			this.nextPageButton.myTmpText.enabled = flag3;
		}
	}

	// Token: 0x060028FD RID: 10493 RVA: 0x000DE794 File Offset: 0x000DC994
	public bool DoesDisplayGroupHaveIncludedCategories(BuilderPieceSet.BuilderDisplayGroup set)
	{
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in set.pieceSubsets)
		{
			if (this._includedCategories.Contains(builderPieceSubset.pieceCategory))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060028FE RID: 10494 RVA: 0x000DE7FC File Offset: 0x000DC9FC
	public BuilderPieceSet.BuilderDisplayGroup GetSelectedGroup()
	{
		return this.currentGroup;
	}

	// Token: 0x060028FF RID: 10495 RVA: 0x000DE804 File Offset: 0x000DCA04
	public int GetDefaultGroupID()
	{
		if (this.includedGroups == null || this.includedGroups.Count < 1)
		{
			return -1;
		}
		BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup = this.includedGroups[0];
		if (!BuilderSetManager.instance.IsPieceSetOwnedLocally(builderDisplayGroup.setID))
		{
			foreach (BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup2 in this.includedGroups)
			{
				if (BuilderSetManager.instance.IsPieceSetOwnedLocally(builderDisplayGroup2.setID))
				{
					return builderDisplayGroup2.GetDisplayGroupIdentifier();
				}
			}
			Debug.LogWarning("No default group available for shelf");
			return -1;
		}
		return builderDisplayGroup.GetDisplayGroupIdentifier();
	}

	// Token: 0x04003558 RID: 13656
	private List<BuilderPieceSet.BuilderDisplayGroup> includedGroups;

	// Token: 0x04003559 RID: 13657
	private int numLiveDisplayGroups;

	// Token: 0x0400355A RID: 13658
	[SerializeField]
	private Material disabledMaterial;

	// Token: 0x0400355B RID: 13659
	[Header("UI")]
	[FormerlySerializedAs("setLabels")]
	[SerializeField]
	private Text[] groupLabels;

	// Token: 0x0400355C RID: 13660
	[Header("Buttons")]
	[FormerlySerializedAs("setButtons")]
	[SerializeField]
	private GorillaPressableButton[] groupButtons;

	// Token: 0x0400355D RID: 13661
	[SerializeField]
	private GorillaPressableButton previousPageButton;

	// Token: 0x0400355E RID: 13662
	[SerializeField]
	private GorillaPressableButton nextPageButton;

	// Token: 0x0400355F RID: 13663
	private List<BuilderPieceSet.BuilderPieceCategory> _includedCategories;

	// Token: 0x04003560 RID: 13664
	private int includedGroupIndex;

	// Token: 0x04003561 RID: 13665
	private BuilderPieceSet.BuilderDisplayGroup currentGroup;

	// Token: 0x04003562 RID: 13666
	private int pageIndex;

	// Token: 0x04003563 RID: 13667
	private int groupsPerPage = 3;

	// Token: 0x04003564 RID: 13668
	private int totalPages = 1;

	// Token: 0x04003565 RID: 13669
	private List<Renderer> zoneRenderers = new List<Renderer>(10);

	// Token: 0x04003566 RID: 13670
	private bool inBuilderZone;

	// Token: 0x04003567 RID: 13671
	[HideInInspector]
	public UnityEvent<int> OnSelectedGroup;
}
