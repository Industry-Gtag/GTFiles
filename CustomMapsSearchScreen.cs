using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaExtensions;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio;
using Modio.API;
using Modio.Mods;
using TMPro;
using UnityEngine;

// Token: 0x02000AC6 RID: 2758
public class CustomMapsSearchScreen : CustomMapsTerminalScreen
{
	// Token: 0x060046AF RID: 18095 RVA: 0x0017D45C File Offset: 0x0017B65C
	public override void Show()
	{
		base.Show();
		this.searchPhraseText.gameObject.SetActive(true);
		this.customMapsGalleryView.gameObject.SetActive(false);
		this.searchMessageText.gameObject.SetActive(false);
		this.leftPageButton.gameObject.SetActive(false);
		this.rightPageButton.gameObject.SetActive(false);
		this.searchedMods.Clear();
		this.filteredSearchedMods.Clear();
		this.displayedMods.Clear();
		this.searchPhraseText.text = this.defaultSearchString;
		this.searchPhrase = string.Empty;
		this.currentSearchModsRequestPage = 0;
		this.currentModPage = 0;
	}

	// Token: 0x060046B0 RID: 18096 RVA: 0x0017D50F File Offset: 0x0017B70F
	public override void Hide()
	{
		base.Hide();
		this.customMapsGalleryView.ShowTileText(false, true);
	}

	// Token: 0x060046B1 RID: 18097 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void Initialize()
	{
	}

	// Token: 0x060046B2 RID: 18098 RVA: 0x0017D524 File Offset: 0x0017B724
	public void ReturnFromDetailsScreen()
	{
		base.Show();
		this.customMapsGalleryView.ShowTileText(true, true);
	}

	// Token: 0x060046B3 RID: 18099 RVA: 0x0017D53C File Offset: 0x0017B73C
	public override void PressButton(CustomMapKeyboardBinding pressedButton)
	{
		if (Time.time < this.showTime + this.activationTime)
		{
			return;
		}
		if (!CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (CustomMapKeyboardBinding.tile1 <= pressedButton && pressedButton <= CustomMapKeyboardBinding.tile6 && !this.customMapsGalleryView.IsNull())
		{
			this.customMapsGalleryView.ShowDetailsForEntry(pressedButton - CustomMapKeyboardBinding.tile1);
		}
		if (pressedButton < CustomMapKeyboardBinding.up)
		{
			string text = this.searchPhrase;
			int num = (int)pressedButton;
			this.searchPhrase = text + num.ToString();
			this.RefreshSearchText();
			return;
		}
		if (pressedButton > CustomMapKeyboardBinding.option3 && pressedButton < CustomMapKeyboardBinding.at)
		{
			this.searchPhrase += pressedButton.ToString();
			this.RefreshSearchText();
			return;
		}
		if (pressedButton != CustomMapKeyboardBinding.delete)
		{
			if (pressedButton != CustomMapKeyboardBinding.enter)
			{
				switch (pressedButton)
				{
				case CustomMapKeyboardBinding.goback:
					if (this.loadingSearchMods)
					{
						return;
					}
					CustomMapsTerminal.ReturnFromSearchScreen();
					break;
				case CustomMapKeyboardBinding.left:
					this.currentModPage--;
					this.RefreshScreenState();
					break;
				case CustomMapKeyboardBinding.right:
					this.currentModPage++;
					this.RefreshScreenState();
					break;
				}
			}
			else
			{
				if (this.loadingSearchMods)
				{
					return;
				}
				this.searchedMods.Clear();
				this.filteredSearchedMods.Clear();
				this.currentSearchModsRequestPage = 0;
				this.searchMessageText.gameObject.SetActive(true);
				this.searchMessageText.text = this.searchingString;
				this.RetrieveMods();
			}
		}
		else if (!this.searchPhrase.IsNullOrEmpty())
		{
			this.searchPhrase = this.searchPhrase.Remove(this.searchPhrase.Length - 1);
		}
		this.RefreshSearchText();
	}

	// Token: 0x060046B4 RID: 18100 RVA: 0x0017D6C9 File Offset: 0x0017B8C9
	private void RefreshSearchText()
	{
		if (this.searchPhrase.IsNullOrEmpty())
		{
			this.searchPhraseText.text = this.defaultSearchString;
			return;
		}
		this.searchPhraseText.text = this.searchPhrase;
	}

	// Token: 0x060046B5 RID: 18101 RVA: 0x0017D6FC File Offset: 0x0017B8FC
	private async Task RetrieveMods()
	{
		if (!this.loadingSearchMods)
		{
			this.loadingSearchMods = true;
			int num = this.currentSearchModsRequestPage;
			this.currentSearchModsRequestPage = num + 1;
			ModSearchFilter modSearchFilter = new ModSearchFilter(num, this.numModsPerRequest);
			Filtering filtering = Filtering.Like;
			modSearchFilter.ClearSearchPhrases(filtering);
			if (!this.searchPhrase.IsNullOrEmpty())
			{
				modSearchFilter.AddSearchPhrase(this.searchPhrase, filtering);
			}
			if (UGCPermissionManager.FeaturedMapsOnly)
			{
				modSearchFilter.AddTag("Featured");
			}
			ValueTuple<Error, ModioPage<Mod>> valueTuple = await ModIOManager.GetMods(modSearchFilter.GetModsFilter());
			Error item = valueTuple.Item1;
			ModioPage<Mod> item2 = valueTuple.Item2;
			this.loadingSearchMods = false;
			if (item)
			{
				this.errorLoadingSearchMods = true;
				this.errorMessage = item.GetMessage();
				GTDev.LogError<string>("[CustomMapsListScreen::OnAvailableModsRetrieved] Failed to retrieve mods. Error: " + item.GetMessage(), null);
			}
			else
			{
				this.totalSearchMods = (int)item2.TotalSearchResults;
				this.searchedMods.AddRange(item2.Data);
				this.FilterSearchMods();
			}
			this.RefreshScreenState();
		}
	}

	// Token: 0x060046B6 RID: 18102 RVA: 0x0017D740 File Offset: 0x0017B940
	private void FilterSearchMods()
	{
		if (this.searchedMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		this.filteredSearchedMods.Clear();
		foreach (Mod mod in this.searchedMods)
		{
			ModId modId;
			if (ModIOManager.TryGetNewMapsModId(out modId) && mod.Id == modId)
			{
				this.totalSearchMods = Mathf.Max(0, this.totalSearchMods - 1);
			}
			else
			{
				this.filteredSearchedMods.Add(mod);
			}
		}
	}

	// Token: 0x060046B7 RID: 18103 RVA: 0x0017D7E0 File Offset: 0x0017B9E0
	private void RefreshScreenState()
	{
		this.searchMessageText.gameObject.SetActive(false);
		this.customMapsGalleryView.ResetGallery();
		this.customMapsGalleryView.gameObject.SetActive(false);
		this.displayedMods.Clear();
		if (this.errorLoadingSearchMods)
		{
			this.searchMessageText.gameObject.SetActive(true);
			this.searchMessageText.text = this.errorMessage;
			this.leftPageButton.SetActive(false);
			this.rightPageButton.SetActive(false);
			return;
		}
		if (this.filteredSearchedMods.IsNullOrEmpty<Mod>())
		{
			this.searchMessageText.gameObject.SetActive(true);
			this.searchMessageText.text = this.noMapsFoundString;
			this.leftPageButton.SetActive(false);
			this.rightPageButton.SetActive(false);
			return;
		}
		int num = 0;
		int num2 = this.modsPerPage - 1;
		if (!this.IsOnFirstPage())
		{
			num = this.currentModPage * this.modsPerPage;
			num2 = num + this.modsPerPage - 1;
			this.leftPageButton.gameObject.SetActive(true);
		}
		else
		{
			this.leftPageButton.gameObject.SetActive(false);
		}
		if (!this.IsOnLastPage())
		{
			this.rightPageButton.gameObject.SetActive(true);
		}
		else
		{
			this.rightPageButton.gameObject.SetActive(false);
		}
		if (this.filteredSearchedMods.Count <= num2 && this.totalSearchMods > this.searchedMods.Count)
		{
			this.RetrieveMods();
			return;
		}
		int num3 = num;
		while (num3 <= num2 && this.filteredSearchedMods.Count > num3)
		{
			this.displayedMods.Add(this.filteredSearchedMods[num3]);
			num3++;
		}
		this.customMapsGalleryView.gameObject.SetActive(true);
		string text;
		if (!this.customMapsGalleryView.DisplayGallery(this.displayedMods, true, out text))
		{
			this.searchMessageText.gameObject.SetActive(true);
			this.searchMessageText.text = text;
			this.customMapsGalleryView.gameObject.SetActive(false);
			this.leftPageButton.SetActive(false);
			this.rightPageButton.SetActive(false);
		}
	}

	// Token: 0x060046B8 RID: 18104 RVA: 0x0017D9F4 File Offset: 0x0017BBF4
	private int GetNumPages()
	{
		int num = this.totalSearchMods % this.modsPerPage;
		int num2 = this.totalSearchMods / this.modsPerPage;
		if (num > 0)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x060046B9 RID: 18105 RVA: 0x0017DA24 File Offset: 0x0017BC24
	private bool IsOnFirstPage()
	{
		return this.currentModPage == 0;
	}

	// Token: 0x060046BA RID: 18106 RVA: 0x0017DA30 File Offset: 0x0017BC30
	private bool IsOnLastPage()
	{
		long num = (long)this.GetNumPages();
		return (long)(this.currentModPage + 1) == num;
	}

	// Token: 0x0400593B RID: 22843
	[SerializeField]
	private TMP_Text searchPhraseText;

	// Token: 0x0400593C RID: 22844
	[SerializeField]
	private TMP_Text searchMessageText;

	// Token: 0x0400593D RID: 22845
	[SerializeField]
	private CustomMapsGalleryView customMapsGalleryView;

	// Token: 0x0400593E RID: 22846
	[SerializeField]
	private GameObject leftPageButton;

	// Token: 0x0400593F RID: 22847
	[SerializeField]
	private GameObject rightPageButton;

	// Token: 0x04005940 RID: 22848
	[SerializeField]
	private string defaultSearchString = "SEARCH PHRASE";

	// Token: 0x04005941 RID: 22849
	[SerializeField]
	private string noMapsFoundString = "NO RESULTS FOUND";

	// Token: 0x04005942 RID: 22850
	[SerializeField]
	private string searchingString = "SEARCHING";

	// Token: 0x04005943 RID: 22851
	[SerializeField]
	private int numModsPerRequest = 60;

	// Token: 0x04005944 RID: 22852
	[SerializeField]
	private int modsPerPage = 6;

	// Token: 0x04005945 RID: 22853
	private string searchPhrase = "";

	// Token: 0x04005946 RID: 22854
	private List<Mod> searchedMods = new List<Mod>();

	// Token: 0x04005947 RID: 22855
	private List<Mod> filteredSearchedMods = new List<Mod>();

	// Token: 0x04005948 RID: 22856
	private List<Mod> displayedMods = new List<Mod>();

	// Token: 0x04005949 RID: 22857
	private int currentSearchModsRequestPage;

	// Token: 0x0400594A RID: 22858
	private bool loadingSearchMods;

	// Token: 0x0400594B RID: 22859
	private bool errorLoadingSearchMods;

	// Token: 0x0400594C RID: 22860
	private int totalSearchMods;

	// Token: 0x0400594D RID: 22861
	private int currentModPage;

	// Token: 0x0400594E RID: 22862
	private string errorMessage = "";
}
