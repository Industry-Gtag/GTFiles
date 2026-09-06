using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio;
using Modio.Errors;
using Modio.Mods;
using Modio.Users;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000AB9 RID: 2745
public class CustomMapsListScreen : CustomMapsTerminalScreen
{
	// Token: 0x1700069A RID: 1690
	// (get) Token: 0x06004650 RID: 18000 RVA: 0x0017ABC3 File Offset: 0x00178DC3
	public bool CommunityMapsOnly
	{
		get
		{
			return this.communityMapsOnly;
		}
	}

	// Token: 0x1700069B RID: 1691
	// (get) Token: 0x06004651 RID: 18001 RVA: 0x0017ABCB File Offset: 0x00178DCB
	public int CurrentModPage
	{
		get
		{
			return this.currentModPage;
		}
	}

	// Token: 0x1700069C RID: 1692
	// (get) Token: 0x06004652 RID: 18002 RVA: 0x0017ABD3 File Offset: 0x00178DD3
	public int ModsPerPage
	{
		get
		{
			return this.modsPerPage;
		}
	}

	// Token: 0x1700069D RID: 1693
	// (get) Token: 0x06004653 RID: 18003 RVA: 0x0017ABDB File Offset: 0x00178DDB
	// (set) Token: 0x06004654 RID: 18004 RVA: 0x0017ABE4 File Offset: 0x00178DE4
	public SortModsBy SortType
	{
		get
		{
			return this.sortType;
		}
		set
		{
			if (this.sortType != value)
			{
				this.currentAvailableModsRequestPage = 0;
			}
			this.sortType = value;
			switch (this.sortType)
			{
			case SortModsBy.Name:
				this.isAscendingOrder = true;
				return;
			case SortModsBy.Price:
				break;
			case SortModsBy.Rating:
				this.isAscendingOrder = false;
				return;
			case SortModsBy.Popular:
				this.isAscendingOrder = false;
				return;
			case SortModsBy.Downloads:
				this.isAscendingOrder = false;
				return;
			case SortModsBy.Subscribers:
				this.isAscendingOrder = false;
				return;
			case SortModsBy.DateSubmitted:
				this.isAscendingOrder = false;
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06004655 RID: 18005 RVA: 0x0017AC61 File Offset: 0x00178E61
	private void Awake()
	{
		this.subscribedBttnPosition = this.subscribedMapsButton.transform.position;
		this.searchBttnPosition = this.searchButton.transform.position;
	}

	// Token: 0x06004656 RID: 18006 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void Initialize()
	{
	}

	// Token: 0x06004657 RID: 18007 RVA: 0x0017AC90 File Offset: 0x00178E90
	public override void Show()
	{
		base.Show();
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOUserChanged.AddListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOCacheRefreshing.RemoveListener(new UnityAction(this.OnModCacheRefreshing));
		ModIOManager.OnModIOCacheRefreshing.AddListener(new UnityAction(this.OnModCacheRefreshing));
		ModIOManager.OnModIOCacheRefreshed.RemoveListener(new UnityAction(this.OnModCacheRefreshed));
		ModIOManager.OnModIOCacheRefreshed.AddListener(new UnityAction(this.OnModCacheRefreshed));
		if (this.featuredMods.IsNullOrEmpty<Mod>())
		{
			this.RetrieveFeaturedMods();
		}
		if (this.availableMods.IsNullOrEmpty<Mod>())
		{
			this.RetrieveAvailableMods();
		}
		this.RetrieveInstalledMods(false);
		this.RetrieveFavoriteMods(false);
		this.RetrieveSubscribedMods();
		this.RefreshScreenState();
	}

	// Token: 0x06004658 RID: 18008 RVA: 0x0017ADC4 File Offset: 0x00178FC4
	public override void Hide()
	{
		base.Hide();
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOCacheRefreshing.RemoveListener(new UnityAction(this.OnModCacheRefreshing));
		ModIOManager.OnModIOCacheRefreshed.RemoveListener(new UnityAction(this.OnModCacheRefreshed));
	}

	// Token: 0x06004659 RID: 18009 RVA: 0x0017AE45 File Offset: 0x00179045
	private void OnModIOLoggedIn()
	{
		if (CustomMapsTerminal.IsDriver)
		{
			this.subscribedMapsButton.gameObject.SetActive(true);
		}
		this.subscribedMods = null;
		this.filteredSubscribedMods.Clear();
		this.totalSubscribedMods = 0;
		this.RetrieveSubscribedMods();
	}

	// Token: 0x0600465A RID: 18010 RVA: 0x0017AE7F File Offset: 0x0017907F
	private void OnModIOLoggedOut()
	{
		this.subscribedMapsButton.gameObject.SetActive(false);
		this.subscribedMods = null;
		this.filteredSubscribedMods.Clear();
		this.totalSubscribedMods = 0;
	}

	// Token: 0x0600465B RID: 18011 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnModIOUserChanged(User user)
	{
	}

	// Token: 0x0600465C RID: 18012 RVA: 0x0017AEAB File Offset: 0x001790AB
	private void OnModCacheRefreshing()
	{
		this.RefreshScreenState();
	}

	// Token: 0x0600465D RID: 18013 RVA: 0x0017AEB3 File Offset: 0x001790B3
	private void OnModCacheRefreshed()
	{
		this.RetrieveFavoriteMods(false);
		this.RetrieveInstalledMods(false);
		if (ModIOManager.IsLoggedIn())
		{
			this.RetrieveSubscribedMods();
		}
	}

	// Token: 0x0600465E RID: 18014 RVA: 0x0017AED4 File Offset: 0x001790D4
	public override void PressButton(CustomMapKeyboardBinding buttonPressed)
	{
		if (Time.time < this.showTime + this.activationTime)
		{
			return;
		}
		GTDev.Log<string>("[CustomMapsListScreen::PressButton] Is Driver: " + CustomMapsTerminal.IsDriver.ToString() + ", Button Pressed: " + buttonPressed.ToString(), null);
		if (!CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.goback)
		{
			return;
		}
		if (this.loadingText.gameObject.activeSelf)
		{
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.option3)
		{
			ModIOManager.RefreshUserProfile(delegate(bool result)
			{
				if (result)
				{
					this.Refresh();
				}
			}, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.option4)
		{
			CustomMapsTerminal.ShowSearchScreen();
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.up)
		{
			this.currentModPage--;
			this.RefreshScreenState();
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.down)
		{
			this.currentModPage++;
			this.RefreshScreenState();
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.all)
		{
			bool flag = this.communityMapsOnly;
			this.communityMapsOnly = false;
			this.displayFeaturedMods = this.sortType == SortModsBy.Popular;
			if (flag)
			{
				this.RefreshModSearch();
			}
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.AvailableMods, flag);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.mustplay)
		{
			bool flag2 = !this.communityMapsOnly;
			this.communityMapsOnly = true;
			this.displayFeaturedMods = false;
			if (flag2)
			{
				this.RefreshModSearch();
			}
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.AvailableMods, flag2);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.sub)
		{
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.SubscribedMods, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.fav)
		{
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.FavoriteMods, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.inst)
		{
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.InstalledMods, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.sort)
		{
			this.SetSortType();
			this.RefreshModSearch();
			return;
		}
		if (CustomMapKeyboardBinding.one <= buttonPressed && buttonPressed <= CustomMapKeyboardBinding.nine && !this.customMapsGalleryView.IsNull())
		{
			this.customMapsGalleryView.ShowDetailsForEntry(buttonPressed - CustomMapKeyboardBinding.one);
		}
	}

	// Token: 0x0600465F RID: 18015 RVA: 0x0017B064 File Offset: 0x00179264
	private void SetSortType()
	{
		this.currentAvailableModsRequestPage = 0;
		this.sortTypeIndex++;
		if (this.sortTypeIndex >= 6)
		{
			this.sortTypeIndex = 0;
		}
		switch (this.sortTypeIndex)
		{
		case 0:
			this.SortType = SortModsBy.Popular;
			this.useMapName = true;
			this.displayFeaturedMods = !this.communityMapsOnly;
			return;
		case 1:
			this.SortType = SortModsBy.DateSubmitted;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		case 2:
			this.SortType = SortModsBy.Rating;
			this.useMapName = false;
			this.displayFeaturedMods = false;
			return;
		case 3:
			this.SortType = SortModsBy.Downloads;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		case 4:
			this.SortType = SortModsBy.Subscribers;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		case 5:
			this.SortType = SortModsBy.Name;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		default:
			this.sortTypeIndex = 0;
			this.SortType = SortModsBy.Popular;
			this.useMapName = true;
			this.displayFeaturedMods = !this.communityMapsOnly;
			return;
		}
	}

	// Token: 0x06004660 RID: 18016 RVA: 0x0017B170 File Offset: 0x00179370
	public void SwapListDisplay(CustomMapsListScreen.ListScreenState newState, bool force = false)
	{
		if (this.currentState == newState && !force)
		{
			return;
		}
		if (newState == CustomMapsListScreen.ListScreenState.SubscribedMods && !ModIOManager.IsLoggedIn())
		{
			return;
		}
		this.currentState = newState;
		this.currentModPage = 0;
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			this.allMapsButton.SetButtonActive(!this.communityMapsOnly);
			this.communityMapsButton.SetButtonActive(this.communityMapsOnly);
			this.favoriteMapsButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			break;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			this.allMapsButton.SetButtonActive(false);
			this.communityMapsButton.SetButtonActive(false);
			this.favoriteMapsButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(true);
			break;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			this.allMapsButton.SetButtonActive(false);
			this.communityMapsButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			this.favoriteMapsButton.SetButtonActive(true);
			break;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			this.allMapsButton.SetButtonActive(false);
			this.communityMapsButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(false);
			this.favoriteMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(true);
			break;
		}
		this.RefreshScreenState();
	}

	// Token: 0x06004661 RID: 18017 RVA: 0x0017B308 File Offset: 0x00179508
	public void RefreshModSearch()
	{
		if (this.loadingAvailableMods || this.loadingFavoriteMods || this.loadingInstalledMods || this.loadingSubscribedMods)
		{
			return;
		}
		this.currentModPage = 0;
		this.availableMods.Clear();
		this.filteredAvailableMods.Clear();
		this.currentAvailableModsRequestPage = 0;
		this.errorLoadingAvailableMods = false;
		this.totalAvailableMods = 0;
		this.RetrieveAvailableMods();
	}

	// Token: 0x06004662 RID: 18018 RVA: 0x0017B370 File Offset: 0x00179570
	public void Refresh()
	{
		if (this.loadingAvailableMods || this.loadingFavoriteMods || this.loadingFeaturedMods || this.loadingInstalledMods || this.loadingSubscribedMods)
		{
			return;
		}
		this.currentModPage = 0;
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			this.featuredMods.Clear();
			this.availableMods.Clear();
			this.filteredAvailableMods.Clear();
			this.currentAvailableModsRequestPage = 0;
			this.errorLoadingAvailableMods = false;
			this.totalAvailableMods = 0;
			this.RetrieveFeaturedMods();
			this.RetrieveAvailableMods();
			return;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			this.RetrieveInstalledMods(true);
			return;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			this.RetrieveFavoriteMods(true);
			return;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			this.RetrieveSubscribedMods();
			return;
		default:
			return;
		}
	}

	// Token: 0x06004663 RID: 18019 RVA: 0x0017B428 File Offset: 0x00179628
	private void RetrieveFeaturedMods()
	{
		if (this.loadingFeaturedMods || this.featuredMods.Count > 0)
		{
			return;
		}
		this.loadingFeaturedMods = true;
		PlayFabTitleDataCache.Instance.GetTitleData(this.featuredModsPlayFabKey, new Action<string>(this.OnGetFeaturedModsTitleData), delegate(PlayFabError error)
		{
			this.loadingFeaturedMods = false;
			this.RefreshScreenState();
		}, false);
	}

	// Token: 0x06004664 RID: 18020 RVA: 0x0017B47C File Offset: 0x0017967C
	private async void OnGetFeaturedModsTitleData(string data)
	{
		if (data.IsNullOrEmpty())
		{
			this.RefreshScreenState();
		}
		else
		{
			this.featuredModIds.Clear();
			this.featuredMods.Clear();
			if (data[0] == '"' && data[data.Length - 1] == '"')
			{
				data = data.Substring(1, data.Length - 2);
			}
			string[] array = data.Split(',', StringSplitOptions.None);
			foreach (string text in array)
			{
				if (!text.IsNullOrEmpty())
				{
					long featuredModId;
					try
					{
						featuredModId = long.Parse(text);
					}
					catch (Exception)
					{
						goto IL_018F;
					}
					ValueTuple<Error, Mod> valueTuple = await ModIOManager.GetMod(new ModId(featuredModId), false, null);
					if (!valueTuple.Item1 && (!UGCPermissionManager.FeaturedMapsOnly || ModIOManager.IsFeaturedMap(valueTuple.Item2)))
					{
						this.featuredModIds.Add(featuredModId);
						this.featuredMods.Add(valueTuple.Item2);
					}
				}
				IL_018F:;
			}
			string[] array2 = null;
			this.totalFeaturedMods = this.featuredMods.Count;
			GTDev.Log<string>(string.Format("CustomMapsListScreen::OnGetFeaturedModsTitleData totalFeaturedMods {0}", this.totalFeaturedMods), null);
			this.FilterAvailableMods();
			this.loadingFeaturedMods = false;
			if (this.currentState == CustomMapsListScreen.ListScreenState.AvailableMods)
			{
				this.RefreshScreenState();
			}
		}
	}

	// Token: 0x06004665 RID: 18021 RVA: 0x0017B4BC File Offset: 0x001796BC
	private async void RetrieveAvailableMods()
	{
		if (!this.loadingAvailableMods)
		{
			this.loadingAvailableMods = true;
			int num = this.currentAvailableModsRequestPage;
			this.currentAvailableModsRequestPage = num + 1;
			ModSearchFilter modSearchFilter = new ModSearchFilter(num, this.numModsPerRequest);
			modSearchFilter.SortBy = this.sortType;
			if (this.communityMapsOnly)
			{
				modSearchFilter.AddTag(this.communityMapsTag);
			}
			if (UGCPermissionManager.FeaturedMapsOnly)
			{
				modSearchFilter.AddTag("Featured");
			}
			modSearchFilter.IsSortAscending = this.isAscendingOrder;
			ValueTuple<Error, ModioPage<Mod>> valueTuple = await ModIOManager.GetMods(modSearchFilter.GetModsFilter());
			Error item = valueTuple.Item1;
			ModioPage<Mod> item2 = valueTuple.Item2;
			if (item)
			{
				this.errorLoadingAvailableMods = true;
				this.loadingAvailableMods = false;
				GTDev.LogError<string>("[CustomMapsListScreen::OnAvailableModsRetrieved] Failed to retrieve mods. Error: " + item.GetMessage(), null);
			}
			else
			{
				this.totalAvailableMods = (int)item2.TotalSearchResults;
				this.availableMods.AddRange(item2.Data);
				this.FilterAvailableMods();
			}
			this.loadingAvailableMods = false;
			if (this.currentState == CustomMapsListScreen.ListScreenState.AvailableMods)
			{
				this.RefreshScreenState();
			}
		}
	}

	// Token: 0x06004666 RID: 18022 RVA: 0x0017B4F4 File Offset: 0x001796F4
	private void FilterAvailableMods()
	{
		this.filteredAvailableMods.Clear();
		if (this.availableMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		this.totalAvailableMods = Mathf.Max(0, this.totalAvailableMods - 1);
		foreach (Mod mod in this.availableMods)
		{
			ModId modId;
			ModIOManager.TryGetNewMapsModId(out modId);
			if (!(mod.Id == modId) && (!this.displayFeaturedMods || this.featuredModIds.IsNullOrEmpty<long>() || !this.featuredModIds.Contains(mod.Id)))
			{
				this.filteredAvailableMods.Add(mod);
			}
		}
		if (this.displayFeaturedMods && !this.featuredMods.IsNullOrEmpty<Mod>())
		{
			this.filteredAvailableMods.InsertRange(0, this.featuredMods);
		}
	}

	// Token: 0x06004667 RID: 18023 RVA: 0x0017B5E4 File Offset: 0x001797E4
	private async Task RetrieveSubscribedMods()
	{
		if (ModIOManager.IsLoggedIn())
		{
			if (this.loadingSubscribedMods)
			{
				this.restartSubscribedModsRetrieval = true;
			}
			else
			{
				this.subscribedMods = null;
				this.filteredSubscribedMods.Clear();
				this.totalSubscribedMods = 0;
				this.errorLoadingSubscribedMods = false;
				this.loadingSubscribedMods = true;
				ValueTuple<Error, Mod[]> valueTuple = await ModIOManager.GetSubscribedMods();
				Error item = valueTuple.Item1;
				this.subscribedMods = valueTuple.Item2;
				if (this.restartSubscribedModsRetrieval)
				{
					this.restartSubscribedModsRetrieval = false;
					this.RetrieveSubscribedMods();
				}
				else if (item)
				{
					this.errorLoadingSubscribedMods = true;
					this.loadingSubscribedMods = false;
					Debug.LogError("[CustomMapsListScreen::RetrieveSubscribedMods] Failed to get subscribed mods. Error: " + item.GetMessage());
				}
				else
				{
					this.FilterSubscribedMods();
					this.totalSubscribedMods = this.filteredSubscribedMods.Count;
					this.loadingSubscribedMods = false;
					if (this.currentState == CustomMapsListScreen.ListScreenState.SubscribedMods)
					{
						this.RefreshScreenState();
					}
				}
			}
		}
	}

	// Token: 0x06004668 RID: 18024 RVA: 0x0017B628 File Offset: 0x00179828
	private void FilterSubscribedMods()
	{
		this.filteredSubscribedMods.Clear();
		if (this.subscribedMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		foreach (Mod mod in this.subscribedMods)
		{
			ModId modId;
			ModIOManager.TryGetNewMapsModId(out modId);
			if (!(mod.Id == modId))
			{
				this.filteredSubscribedMods.Add(mod);
			}
		}
	}

	// Token: 0x06004669 RID: 18025 RVA: 0x0017B68C File Offset: 0x0017988C
	private async Task RetrieveInstalledMods(bool forceRefresh = false)
	{
		if (this.loadingInstalledMods)
		{
			this.restartInstalledModsRetrieval = true;
			this.restartInstalledModsRetrievalForceRefresh = forceRefresh;
		}
		else
		{
			this.installedMods = null;
			this.filteredInstalledMods.Clear();
			this.totalInstalledMods = 0;
			this.errorLoadingInstalledMods = false;
			this.loadingInstalledMods = true;
			ValueTuple<Error, Mod[]> valueTuple = await ModIOManager.GetInstalledMods(forceRefresh);
			Error item = valueTuple.Item1;
			this.installedMods = valueTuple.Item2;
			if (this.restartInstalledModsRetrieval)
			{
				this.restartInstalledModsRetrieval = false;
				this.RetrieveInstalledMods(this.restartInstalledModsRetrievalForceRefresh);
				this.restartInstalledModsRetrievalForceRefresh = false;
			}
			else if (item)
			{
				this.errorLoadingInstalledMods = true;
				this.loadingInstalledMods = false;
				GTDev.LogError<string>("[CustomMapsListScreen::RetrieveInstalledMods] Failed to get Installed Mods. Error: " + item.GetMessage(), null);
			}
			else
			{
				this.FilterInstalledMods();
				this.totalInstalledMods = this.filteredInstalledMods.Count;
				this.loadingInstalledMods = false;
				if (this.currentState == CustomMapsListScreen.ListScreenState.InstalledMods)
				{
					this.RefreshScreenState();
				}
			}
		}
	}

	// Token: 0x0600466A RID: 18026 RVA: 0x0017B6D8 File Offset: 0x001798D8
	private void FilterInstalledMods()
	{
		this.filteredInstalledMods.Clear();
		if (this.installedMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		foreach (Mod mod in this.installedMods)
		{
			ModId modId;
			if (!ModIOManager.TryGetNewMapsModId(out modId) || !(mod.Id == modId))
			{
				this.filteredInstalledMods.Add(mod);
			}
		}
	}

	// Token: 0x0600466B RID: 18027 RVA: 0x0017B73C File Offset: 0x0017993C
	private async Task RetrieveFavoriteMods(bool forceRefresh = false)
	{
		if (this.loadingFavoriteMods)
		{
			this.restartFavoriteModsRetrieval = true;
			this.restartFavoriteModsRetrievalForceRefresh = forceRefresh;
		}
		else
		{
			this.favoriteMods.Clear();
			this.filteredFavoriteMods.Clear();
			this.totalFavoriteMods = 0;
			this.errorLoadingFavoriteMods = false;
			this.loadingFavoriteMods = true;
			ValueTuple<Error, List<Mod>> valueTuple = await ModIOManager.GetFavoriteMods(forceRefresh);
			Error item = valueTuple.Item1;
			this.favoriteMods = valueTuple.Item2;
			if (this.restartFavoriteModsRetrieval)
			{
				this.restartFavoriteModsRetrieval = false;
				this.RetrieveFavoriteMods(this.restartFavoriteModsRetrievalForceRefresh);
				this.restartFavoriteModsRetrievalForceRefresh = false;
			}
			else if (item)
			{
				if (item.Code != ErrorCode.FILE_NOT_FOUND)
				{
					this.errorLoadingFavoriteMods = true;
				}
				this.loadingFavoriteMods = false;
				GTDev.LogError<string>("[CustomMapsListScreen::RetrieveFavoriteMods] Failed to get Favorite mods. Error: " + item.GetMessage(), null);
			}
			else
			{
				this.FilterFavoriteMods();
				this.totalFavoriteMods = this.filteredFavoriteMods.Count;
				this.loadingFavoriteMods = false;
				if (this.currentState == CustomMapsListScreen.ListScreenState.FavoriteMods)
				{
					this.RefreshScreenState();
				}
			}
		}
	}

	// Token: 0x0600466C RID: 18028 RVA: 0x0017B788 File Offset: 0x00179988
	private void FilterFavoriteMods()
	{
		this.filteredFavoriteMods.Clear();
		if (this.favoriteMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		foreach (Mod mod in this.favoriteMods)
		{
			ModId modId;
			if (!ModIOManager.TryGetNewMapsModId(out modId) || !(mod.Id == modId))
			{
				this.filteredFavoriteMods.Add(mod);
			}
		}
	}

	// Token: 0x0600466D RID: 18029 RVA: 0x0017B810 File Offset: 0x00179A10
	public void GetDisplayedModList(out long[] modList)
	{
		if (this.displayedModProfiles.IsNullOrEmpty<Mod>())
		{
			modList = Array.Empty<long>();
			return;
		}
		modList = new long[this.displayedModProfiles.Count];
		for (int i = 0; i < this.displayedModProfiles.Count; i++)
		{
			modList[i] = this.displayedModProfiles[i].Id;
		}
	}

	// Token: 0x0600466E RID: 18030 RVA: 0x0017B874 File Offset: 0x00179A74
	private void RefreshScreenState()
	{
		this.displayedModProfiles.Clear();
		this.errorText.gameObject.SetActive(false);
		this.sortTypeText.gameObject.SetActive(false);
		this.modPageText.gameObject.SetActive(false);
		this.titleText.text = this.GetTitleForCurrentState();
		this.loadingText.gameObject.SetActive(true);
		if (CustomMapsTerminal.IsDriver && ModIOManager.IsLoggedIn())
		{
			this.subscribedMapsButton.gameObject.SetActive(true);
			this.subscribedMapsButton.transform.position = this.subscribedBttnPosition;
			this.searchButton.transform.position = this.searchBttnPosition;
		}
		else
		{
			this.subscribedMapsButton.gameObject.SetActive(false);
			this.subscribedMapsButton.transform.position = this.searchBttnPosition;
			this.searchButton.transform.position = this.subscribedBttnPosition;
		}
		if (this.currentState == CustomMapsListScreen.ListScreenState.AvailableMods)
		{
			this.RefreshScreenForAvailableMods();
			return;
		}
		this.sortByButton.SetActive(false);
		this.RefreshScreenForCurrentState();
	}

	// Token: 0x0600466F RID: 18031 RVA: 0x0017B98C File Offset: 0x00179B8C
	private void RefreshScreenForAvailableMods()
	{
		string text = ((this.sortType == SortModsBy.DateSubmitted) ? "NEWEST" : this.sortType.ToString().ToUpper());
		this.sortByButton.SetActive(true);
		this.sortTypeText.gameObject.SetActive(true);
		this.sortTypeText.text = text;
		this.customMapsGalleryView.ResetGallery();
		if (this.loadingAvailableMods)
		{
			return;
		}
		if (this.errorLoadingAvailableMods)
		{
			this.errorText.text = this.failedToRetrieveModsString;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.UpdatePageCount(this.totalAvailableMods);
		int num = 0;
		int num2 = this.modsPerPage - 1;
		if (!this.IsOnFirstPage())
		{
			num = this.currentModPage * this.modsPerPage;
			num2 = num + this.modsPerPage - 1;
			this.pageUpButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageUpButton.gameObject.SetActive(false);
		}
		if (!this.IsOnLastPage())
		{
			this.pageDownButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageDownButton.gameObject.SetActive(false);
		}
		if (this.filteredAvailableMods.Count <= num2 && this.totalAvailableMods > this.availableMods.Count)
		{
			this.displayedModProfiles.Clear();
			this.RetrieveAvailableMods();
			return;
		}
		int num3 = num;
		while (num3 <= num2 && this.filteredAvailableMods.Count > num3)
		{
			this.displayedModProfiles.Add(this.filteredAvailableMods[num3]);
			num3++;
		}
		string text2;
		if (!this.customMapsGalleryView.DisplayGallery(this.displayedModProfiles, this.useMapName, out text2))
		{
			this.errorText.text = text2;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		if (this.displayFeaturedMods && !this.featuredModIds.IsNullOrEmpty<long>())
		{
			for (int i = 0; i < this.displayedModProfiles.Count; i++)
			{
				if (this.featuredModIds.Contains(this.displayedModProfiles[i].Id))
				{
					this.customMapsGalleryView.HighlightTileAtIndex(num + i);
				}
			}
		}
		this.loadingText.gameObject.SetActive(false);
	}

	// Token: 0x06004670 RID: 18032 RVA: 0x0017BBE8 File Offset: 0x00179DE8
	private void RefreshScreenForCurrentState()
	{
		this.customMapsGalleryView.ResetGallery();
		if (this.GetLoadingStatusForCurrentState())
		{
			return;
		}
		if (this.HasModLoadingErrorForCurrentState())
		{
			this.modPageText.gameObject.SetActive(false);
			if (CustomMapsTerminal.IsDriver)
			{
				this.currentModPage = -1;
			}
			this.errorText.text = this.failedToRetrieveModsString;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.UpdatePageCount(this.GetTotalModsForCurrentState());
		if (!this.IsOnFirstPage())
		{
			this.pageUpButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageUpButton.gameObject.SetActive(false);
		}
		if (!this.IsOnLastPage())
		{
			this.pageDownButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageDownButton.gameObject.SetActive(false);
		}
		List<Mod> modListForCurrentState = this.GetModListForCurrentState();
		if (modListForCurrentState != null)
		{
			if (this.currentState == CustomMapsListScreen.ListScreenState.CustomModList)
			{
				this.displayedModProfiles.AddRange(modListForCurrentState);
			}
			else
			{
				int num = this.currentModPage * this.modsPerPage;
				int num2 = num;
				while (num2 < num + this.modsPerPage && modListForCurrentState.Count > num2)
				{
					this.displayedModProfiles.Add(modListForCurrentState[num2]);
					num2++;
				}
			}
		}
		string text;
		if (!this.customMapsGalleryView.DisplayGallery(this.displayedModProfiles, true, out text))
		{
			this.errorText.text = text;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.loadingText.gameObject.SetActive(false);
	}

	// Token: 0x06004671 RID: 18033 RVA: 0x0017BD7C File Offset: 0x00179F7C
	private bool GetLoadingStatusForCurrentState()
	{
		if (ModIOManager.IsRefreshing())
		{
			return true;
		}
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.loadingAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.loadingInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.loadingFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.loadingSubscribedMods;
		default:
			return false;
		}
	}

	// Token: 0x06004672 RID: 18034 RVA: 0x0017BDD0 File Offset: 0x00179FD0
	private bool HasModLoadingErrorForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.errorLoadingAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.errorLoadingInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.errorLoadingFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.errorLoadingSubscribedMods;
		default:
			return false;
		}
	}

	// Token: 0x06004673 RID: 18035 RVA: 0x0017BE1C File Offset: 0x0017A01C
	private List<Mod> GetModListForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.filteredAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.filteredInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.filteredFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.filteredSubscribedMods;
		default:
			return null;
		}
	}

	// Token: 0x06004674 RID: 18036 RVA: 0x0017BE68 File Offset: 0x0017A068
	private int GetTotalModsForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.totalAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.totalInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.totalFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.totalSubscribedMods;
		default:
			return 0;
		}
	}

	// Token: 0x06004675 RID: 18037 RVA: 0x0017BEB4 File Offset: 0x0017A0B4
	private string GetTitleForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			if (this.communityMapsOnly)
			{
				return this.communityModsTitle;
			}
			return this.browseModsTitle;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.installedModsTitle;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.favoriteModsTitle;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.subscribedModsTitle;
		default:
			return "";
		}
	}

	// Token: 0x06004676 RID: 18038 RVA: 0x0017BF10 File Offset: 0x0017A110
	private void UpdatePageCount(int totalMods)
	{
		this.totalModCount = totalMods;
		this.modPageText.gameObject.SetActive(false);
		if (this.totalModCount != 0)
		{
			int numPages = this.GetNumPages();
			if (numPages > 1)
			{
				this.modPageText.text = string.Format("{0} / {1}", this.currentModPage + 1, numPages);
				this.modPageText.gameObject.SetActive(true);
			}
			return;
		}
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			this.errorText.text = this.noModsAvailableString;
			return;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			this.errorText.text = this.noInstalledModsString;
			return;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			this.errorText.text = this.noFavoriteModsString;
			return;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			this.errorText.text = this.noSubscribedModsString;
			return;
		case CustomMapsListScreen.ListScreenState.CustomModList:
			this.errorText.text = this.noModsFoundGenericString;
			return;
		default:
			return;
		}
	}

	// Token: 0x06004677 RID: 18039 RVA: 0x0017C000 File Offset: 0x0017A200
	public int GetNumPages()
	{
		int num = this.totalModCount % this.modsPerPage;
		int num2 = this.totalModCount / this.modsPerPage;
		if (num > 0)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x06004678 RID: 18040 RVA: 0x0017C030 File Offset: 0x0017A230
	private bool IsOnFirstPage()
	{
		return this.currentModPage == 0;
	}

	// Token: 0x06004679 RID: 18041 RVA: 0x0017C03C File Offset: 0x0017A23C
	private bool IsOnLastPage()
	{
		long num = (long)this.GetNumPages();
		return (long)(this.currentModPage + 1) == num;
	}

	// Token: 0x0600467A RID: 18042 RVA: 0x0017C060 File Offset: 0x0017A260
	public void RefreshDriverNickname(string driverNickname)
	{
		if (this.currentState == CustomMapsListScreen.ListScreenState.CustomModList)
		{
			this.titleText.text = driverNickname;
		}
	}

	// Token: 0x040058A5 RID: 22693
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x040058A6 RID: 22694
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x040058A7 RID: 22695
	[SerializeField]
	private TMP_Text modPageText;

	// Token: 0x040058A8 RID: 22696
	[SerializeField]
	private TMP_Text titleText;

	// Token: 0x040058A9 RID: 22697
	[SerializeField]
	private TMP_Text sortTypeText;

	// Token: 0x040058AA RID: 22698
	[SerializeField]
	private GameObject sortByButton;

	// Token: 0x040058AB RID: 22699
	[SerializeField]
	private CustomMapsScreenButton allMapsButton;

	// Token: 0x040058AC RID: 22700
	[FormerlySerializedAs("officialMapsButton")]
	[SerializeField]
	private CustomMapsScreenButton communityMapsButton;

	// Token: 0x040058AD RID: 22701
	[SerializeField]
	private CustomMapsScreenButton favoriteMapsButton;

	// Token: 0x040058AE RID: 22702
	[SerializeField]
	private CustomMapsScreenButton installedMapsButton;

	// Token: 0x040058AF RID: 22703
	[SerializeField]
	private CustomMapsScreenButton subscribedMapsButton;

	// Token: 0x040058B0 RID: 22704
	[SerializeField]
	private CustomMapsScreenButton searchButton;

	// Token: 0x040058B1 RID: 22705
	[SerializeField]
	private CustomMapsScreenButton pageUpButton;

	// Token: 0x040058B2 RID: 22706
	[SerializeField]
	private CustomMapsScreenButton pageDownButton;

	// Token: 0x040058B3 RID: 22707
	[SerializeField]
	private CustomMapsGalleryView customMapsGalleryView;

	// Token: 0x040058B4 RID: 22708
	[SerializeField]
	private string browseModsTitle = "AVAILABLE MODS";

	// Token: 0x040058B5 RID: 22709
	[FormerlySerializedAs("officialModsTitle")]
	[SerializeField]
	private string communityModsTitle = "COMMUNITY MODS";

	// Token: 0x040058B6 RID: 22710
	[SerializeField]
	private string installedModsTitle = "INSTALLED MODS";

	// Token: 0x040058B7 RID: 22711
	[SerializeField]
	private string favoriteModsTitle = "FAVORITE MODS";

	// Token: 0x040058B8 RID: 22712
	[SerializeField]
	private string subscribedModsTitle = "SUBSCRIBED MODS";

	// Token: 0x040058B9 RID: 22713
	[SerializeField]
	private string noModsAvailableString = "NO MODS AVAILABLE";

	// Token: 0x040058BA RID: 22714
	[SerializeField]
	private string noModsFoundGenericString = "NO MODS FOUND";

	// Token: 0x040058BB RID: 22715
	[SerializeField]
	private string noSubscribedModsString = "NOT SUBSCRIBED TO ANY MODS";

	// Token: 0x040058BC RID: 22716
	[SerializeField]
	private string noInstalledModsString = "NO MODS INSTALLED";

	// Token: 0x040058BD RID: 22717
	[SerializeField]
	private string noFavoriteModsString = "NO FAVORITE MODS FOUND";

	// Token: 0x040058BE RID: 22718
	[SerializeField]
	private string failedToRetrieveModsString = "FAILED TO RETRIEVE MODS FROM MOD.IO \nPRESS THE 'REFRESH' BUTTON TO RETRY";

	// Token: 0x040058BF RID: 22719
	[SerializeField]
	private int modsPerPage = 12;

	// Token: 0x040058C0 RID: 22720
	[SerializeField]
	private int numModsPerRequest = 24;

	// Token: 0x040058C1 RID: 22721
	[SerializeField]
	private int maxModListItemLength = 25;

	// Token: 0x040058C2 RID: 22722
	[SerializeField]
	private string communityMapsTag = "Community";

	// Token: 0x040058C3 RID: 22723
	[SerializeField]
	private string featuredModsPlayFabKey = "VStumpFeaturedMaps";

	// Token: 0x040058C4 RID: 22724
	private bool loadingFeaturedMods;

	// Token: 0x040058C5 RID: 22725
	private bool displayFeaturedMods = true;

	// Token: 0x040058C6 RID: 22726
	private int totalFeaturedMods;

	// Token: 0x040058C7 RID: 22727
	private List<long> featuredModIds = new List<long>();

	// Token: 0x040058C8 RID: 22728
	private List<Mod> featuredMods = new List<Mod>();

	// Token: 0x040058C9 RID: 22729
	private int currentAvailableModsRequestPage;

	// Token: 0x040058CA RID: 22730
	private bool loadingAvailableMods;

	// Token: 0x040058CB RID: 22731
	private int totalAvailableMods;

	// Token: 0x040058CC RID: 22732
	private bool errorLoadingAvailableMods;

	// Token: 0x040058CD RID: 22733
	private List<Mod> availableMods = new List<Mod>();

	// Token: 0x040058CE RID: 22734
	private List<Mod> filteredAvailableMods = new List<Mod>();

	// Token: 0x040058CF RID: 22735
	private bool loadingInstalledMods;

	// Token: 0x040058D0 RID: 22736
	private bool errorLoadingInstalledMods;

	// Token: 0x040058D1 RID: 22737
	private int totalInstalledMods;

	// Token: 0x040058D2 RID: 22738
	private Mod[] installedMods;

	// Token: 0x040058D3 RID: 22739
	private List<Mod> filteredInstalledMods = new List<Mod>();

	// Token: 0x040058D4 RID: 22740
	private bool loadingFavoriteMods;

	// Token: 0x040058D5 RID: 22741
	private bool errorLoadingFavoriteMods;

	// Token: 0x040058D6 RID: 22742
	private int totalFavoriteMods;

	// Token: 0x040058D7 RID: 22743
	private List<Mod> favoriteMods = new List<Mod>();

	// Token: 0x040058D8 RID: 22744
	private List<Mod> filteredFavoriteMods = new List<Mod>();

	// Token: 0x040058D9 RID: 22745
	private bool loadingSubscribedMods;

	// Token: 0x040058DA RID: 22746
	private bool errorLoadingSubscribedMods;

	// Token: 0x040058DB RID: 22747
	private int totalSubscribedMods;

	// Token: 0x040058DC RID: 22748
	private Mod[] subscribedMods;

	// Token: 0x040058DD RID: 22749
	private List<Mod> filteredSubscribedMods = new List<Mod>();

	// Token: 0x040058DE RID: 22750
	private int currentModPage;

	// Token: 0x040058DF RID: 22751
	private int totalModCount;

	// Token: 0x040058E0 RID: 22752
	private List<Mod> displayedModProfiles = new List<Mod>();

	// Token: 0x040058E1 RID: 22753
	private int sortTypeIndex;

	// Token: 0x040058E2 RID: 22754
	private SortModsBy sortType = SortModsBy.Popular;

	// Token: 0x040058E3 RID: 22755
	private const int MAX_SORT_TYPES = 6;

	// Token: 0x040058E4 RID: 22756
	private List<string> searchTags = new List<string>();

	// Token: 0x040058E5 RID: 22757
	private bool isAscendingOrder;

	// Token: 0x040058E6 RID: 22758
	private bool communityMapsOnly;

	// Token: 0x040058E7 RID: 22759
	private bool useMapName = true;

	// Token: 0x040058E8 RID: 22760
	private Vector3 subscribedBttnPosition;

	// Token: 0x040058E9 RID: 22761
	private Vector3 searchBttnPosition;

	// Token: 0x040058EA RID: 22762
	private bool restartCustomModListRetrieval;

	// Token: 0x040058EB RID: 22763
	private bool restartCustomModListRetrievalForceRefresh;

	// Token: 0x040058EC RID: 22764
	private bool restartInstalledModsRetrieval;

	// Token: 0x040058ED RID: 22765
	private bool restartInstalledModsRetrievalForceRefresh;

	// Token: 0x040058EE RID: 22766
	private bool restartFavoriteModsRetrieval;

	// Token: 0x040058EF RID: 22767
	private bool restartFavoriteModsRetrievalForceRefresh;

	// Token: 0x040058F0 RID: 22768
	private bool restartSubscribedModsRetrieval;

	// Token: 0x040058F1 RID: 22769
	public CustomMapsListScreen.ListScreenState currentState;

	// Token: 0x02000ABA RID: 2746
	public enum ListScreenState
	{
		// Token: 0x040058F3 RID: 22771
		AvailableMods,
		// Token: 0x040058F4 RID: 22772
		InstalledMods,
		// Token: 0x040058F5 RID: 22773
		FavoriteMods,
		// Token: 0x040058F6 RID: 22774
		SubscribedMods,
		// Token: 0x040058F7 RID: 22775
		CustomModList
	}
}
