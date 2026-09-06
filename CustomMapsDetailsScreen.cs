using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio;
using Modio.Mods;
using Modio.Users;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000AAF RID: 2735
public class CustomMapsDetailsScreen : CustomMapsTerminalScreen
{
	// Token: 0x17000697 RID: 1687
	// (get) Token: 0x060045F0 RID: 17904 RVA: 0x001779FC File Offset: 0x00175BFC
	// (set) Token: 0x060045EF RID: 17903 RVA: 0x001779F3 File Offset: 0x00175BF3
	public Mod currentMapMod { get; private set; }

	// Token: 0x060045F1 RID: 17905 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void Initialize()
	{
	}

	// Token: 0x060045F2 RID: 17906 RVA: 0x00177A04 File Offset: 0x00175C04
	public override void Show()
	{
		base.Show();
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOUserChanged.AddListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
		ModIOManager.OnModManagementEvent.AddListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
		CustomMapManager.OnMapLoadStatusChanged.RemoveListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadStatusChanged.AddListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnRoomMapChanged.AddListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnMapUnloadComplete.RemoveListener(new UnityAction(this.OnMapUnloaded));
		CustomMapManager.OnMapUnloadComplete.AddListener(new UnityAction(this.OnMapUnloaded));
		if (!ModIOManager.IsLoggedIn())
		{
			this.subscriptionToggleButton.gameObject.SetActive(false);
		}
		this.deleteButton.gameObject.SetActive(false);
		this.ResetToDefaultView();
	}

	// Token: 0x060045F3 RID: 17907 RVA: 0x00177BA8 File Offset: 0x00175DA8
	public override void Hide()
	{
		base.Hide();
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
		CustomMapManager.OnMapLoadStatusChanged.RemoveListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnMapUnloadComplete.RemoveListener(new UnityAction(this.OnMapUnloaded));
	}

	// Token: 0x060045F4 RID: 17908 RVA: 0x00177C6C File Offset: 0x00175E6C
	private void OnModUpdated()
	{
		ModRating currentUserRating = this.currentMapMod.CurrentUserRating;
		this.rateUpButton.SetButtonActive(currentUserRating == ModRating.Positive);
		this.rateDownButton.SetButtonActive(currentUserRating == ModRating.Negative);
	}

	// Token: 0x060045F5 RID: 17909 RVA: 0x00177CA3 File Offset: 0x00175EA3
	private void OnModIOLoggedIn()
	{
		if (this.currentMapMod.Creator == null)
		{
			this.RefreshCurrentMapMod();
			return;
		}
		if (this.currentMapMod.IsHidden())
		{
			this.UpdateMapDetails(true);
			return;
		}
		this.UpdateStatus(false);
	}

	// Token: 0x060045F6 RID: 17910 RVA: 0x00177CDC File Offset: 0x00175EDC
	private void OnModIOLoggedOut()
	{
		if (this.currentMapMod.IsHidden())
		{
			this.UpdateMapDetails(true);
			return;
		}
		this.UpdateStatus(false);
	}

	// Token: 0x060045F7 RID: 17911 RVA: 0x00177CFB File Offset: 0x00175EFB
	private void OnModIOUserChanged(User user)
	{
		this.UpdateStatus(false);
	}

	// Token: 0x060045F8 RID: 17912 RVA: 0x00177D08 File Offset: 0x00175F08
	private void HandleModManagementEvent(Mod mod, Modfile modfile, ModInstallationManagement.OperationType jobType, ModInstallationManagement.OperationPhase jobPhase)
	{
		if (base.isActiveAndEnabled && this.hasModProfile && this.GetModId() == mod.Id)
		{
			this.UpdateStatus(jobPhase == ModInstallationManagement.OperationPhase.Cancelled || jobPhase == ModInstallationManagement.OperationPhase.Failed);
			if (jobPhase == ModInstallationManagement.OperationPhase.Failed)
			{
				this.modDescriptionText.gameObject.SetActive(false);
				this.loadingMapLabelText.text = this.mapLoadingErrorString;
				this.loadingMapLabelText.gameObject.SetActive(true);
				this.loadingMapMessageText.text = this.mapLoadingErrorInvalidModFile;
				this.loadingMapMessageText.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060045F9 RID: 17913 RVA: 0x00177DAC File Offset: 0x00175FAC
	private void Update()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		string text;
		if (this.GetModId().IsValid() && ModInstallationManagement.CurrentOperationOnMod != null && ModInstallationManagement.CurrentOperationOnMod.Id == this.GetModId() && ModInstallationManagement.CurrentOperationOnMod.File.State != ModFileState.Installed && CustomMapsDetailsScreen.modStatusStrings.TryGetValue(ModInstallationManagement.CurrentOperationOnMod.File.State, out text))
		{
			float num = this.currentMapMod.File.FileStateProgress * 100f;
			this.modStatusText.text = text + string.Format(" {0}%", Mathf.RoundToInt(num));
		}
	}

	// Token: 0x060045FA RID: 17914 RVA: 0x00177E64 File Offset: 0x00176064
	public void RetrieveModFromModIO(long id, bool forceUpdate = false, Action<Error, Mod> callback = null)
	{
		if (this.hasModProfile && this.GetModId()._id == id)
		{
			this.UpdateMapDetails(true);
			return;
		}
		this.pendingModId = id;
		ModIOManager.GetMod(new ModId(id), forceUpdate, (callback != null) ? callback : new Action<Error, Mod>(this.OnProfileReceived));
	}

	// Token: 0x060045FB RID: 17915 RVA: 0x00177EB8 File Offset: 0x001760B8
	public void SetModProfile(Mod mod)
	{
		if (mod.Id != ModId.Null)
		{
			this.pendingModId = 0L;
			this.currentMapMod = mod;
			this.hasModProfile = true;
			this.currentMapMod.OnModUpdated += this.OnModUpdated;
			this.isFavorite = ModIOManager.IsModFavorited(mod.Id);
			this.favoriteToggleButton.SetButtonActive(this.isFavorite);
			PlayerCountHelper.GetPlayerCount(this.currentMapMod, delegate(string count)
			{
				this.playerCountText.text = count;
			}, null);
			this.UpdateMapDetails(true);
		}
	}

	// Token: 0x060045FC RID: 17916 RVA: 0x00177F48 File Offset: 0x00176148
	public override void PressButton(CustomMapKeyboardBinding buttonPressed)
	{
		if (Time.time < this.showTime + this.activationTime)
		{
			return;
		}
		GTDev.Log<string>("[CustomMapsDetailsScreen::PressButton] Is Driver: " + CustomMapsTerminal.IsDriver.ToString() + ", Button Pressed: " + buttonPressed.ToString(), null);
		if (!base.isActiveAndEnabled || !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.goback)
		{
			if (CustomMapManager.IsLoading())
			{
				return;
			}
			if (CustomMapManager.IsUnloading())
			{
				return;
			}
			if (this.mapLoadError)
			{
				this.mapLoadError = false;
				this.loadingMapMessageText.fontSize = 40f;
				CustomMapManager.ClearRoomMap();
				this.ResetToDefaultView();
				return;
			}
			if (CustomMapLoader.IsMapLoaded() || CustomMapManager.GetRoomMapId() != ModId.Null)
			{
				string text;
				if (!this.CanChangeMapState(false, out text))
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.errorText.text = text;
					this.errorText.gameObject.SetActive(true);
					return;
				}
				this.UnloadMap();
				return;
			}
			else
			{
				if (ModInstallationManagement.CurrentOperationOnMod != null && ModInstallationManagement.CurrentOperationOnMod.Id == this.GetModId())
				{
					GTDev.Log<string>("[CustomMapsDetailsScreen::PressButton] Attempted to go back while this mod is " + ModInstallationManagement.CurrentOperationOnMod.File.State.ToString() + ", ignoring...", null);
					return;
				}
				CustomMapsTerminal.ReturnFromDetailsScreen();
				this.hasModProfile = false;
				this.currentMapMod.OnModUpdated -= this.OnModUpdated;
				this.currentMapMod = null;
				return;
			}
		}
		else
		{
			if (!this.hasModProfile || this.mapLoadError)
			{
				bool flag = this.mapLoadError;
				return;
			}
			if (buttonPressed == CustomMapKeyboardBinding.option3)
			{
				this.RefreshCurrentMapMod();
				return;
			}
			if (buttonPressed == CustomMapKeyboardBinding.map)
			{
				if (this.currentMapMod == null || CustomMapLoader.IsMapLoaded() || CustomMapManager.IsLoading() || CustomMapManager.IsUnloading())
				{
					return;
				}
				this.errorText.gameObject.SetActive(false);
				this.errorText.text = "";
				this.loadingMapLabelText.gameObject.SetActive(false);
				this.loadingMapMessageText.gameObject.SetActive(false);
				this.modDescriptionText.gameObject.SetActive(true);
				ModIOManager.RefreshUserProfile(delegate(bool result)
				{
					if (this.currentMapMod.IsSubscribed)
					{
						ModIOManager.UnsubscribeFromMod(this.GetModId(), delegate(Error error)
						{
							if (!error)
							{
								this.UpdateMapDetails(false);
							}
						});
						return;
					}
					ModIOManager.SubscribeToMod(this.GetModId(), delegate(Error error)
					{
						if (!error)
						{
							this.UpdateMapDetails(false);
						}
					});
				}, false);
			}
			if (buttonPressed == CustomMapKeyboardBinding.enter && !CustomMapManager.IsLoading() && !CustomMapManager.IsUnloading() && !CustomMapLoader.IsMapLoaded() && this.currentMapMod != null && !this.IsCurrentModHidden())
			{
				if (this.currentMapMod.File.State == ModFileState.Installed)
				{
					string text2;
					if (!this.CanChangeMapState(true, out text2))
					{
						this.modDescriptionText.gameObject.SetActive(false);
						this.errorText.text = text2;
						this.errorText.gameObject.SetActive(true);
					}
					else
					{
						this.LoadMap();
					}
				}
				else
				{
					ModFileState modFileState = this.currentMapMod.File.State;
					if (modFileState == ModFileState.Queued || modFileState == ModFileState.None || modFileState == ModFileState.Downloaded || modFileState == ModFileState.FileOperationFailed)
					{
						CustomMapManager.TrackMapDownload(this.GetModId());
						ModIOManager.DownloadMod(this.GetModId(), delegate(bool modDownloadStarted)
						{
							if (modDownloadStarted)
							{
								this.UpdateStatus(false);
								return;
							}
							CustomMapManager.StopTrackingMapDownload();
						});
					}
					else
					{
						Debug.Log(string.Format("[CustomMapsDetailsScreen::PressButton] mod has status: {0}, ", this.currentMapMod.File.State) + "cannot start download or attempt to load map...");
					}
				}
			}
			if (buttonPressed == CustomMapKeyboardBinding.fav && this.currentMapMod != null)
			{
				if (this.isFavorite)
				{
					ModIOManager.RemoveFavorite(this.currentMapMod.Id);
					this.isFavorite = ModIOManager.IsModFavorited(this.currentMapMod.Id);
					this.favoriteToggleButton.SetButtonActive(this.isFavorite);
					if (this.IsCurrentModHidden())
					{
						this.favoriteToggleButton.gameObject.SetActive(false);
					}
				}
				else if (!this.IsCurrentModHidden())
				{
					ModIOManager.AddFavorite(this.currentMapMod.Id, delegate(Error error)
					{
						this.isFavorite = ModIOManager.IsModFavorited(this.currentMapMod.Id);
						this.favoriteToggleButton.SetButtonActive(this.isFavorite);
					});
				}
			}
			if (buttonPressed == CustomMapKeyboardBinding.delete)
			{
				if (CustomMapManager.IsLoading() || CustomMapManager.IsUnloading() || CustomMapLoader.IsMapLoaded())
				{
					return;
				}
				Mod currentMapMod = this.currentMapMod;
				bool flag2;
				if (currentMapMod != null)
				{
					Modfile file = currentMapMod.File;
					if (file != null)
					{
						ModFileState modFileState = file.State;
						if (modFileState == ModFileState.Queued || modFileState == ModFileState.Installed)
						{
							flag2 = true;
							goto IL_03FF;
						}
					}
				}
				flag2 = false;
				IL_03FF:
				if (flag2)
				{
					this.currentMapMod.UninstallOtherUserMod(true);
					this.UpdateStatus(false);
				}
			}
			if (buttonPressed == CustomMapKeyboardBinding.rateUp)
			{
				this.currentMapMod.RateMod((this.currentMapMod.CurrentUserRating == ModRating.Positive) ? ModRating.None : ModRating.Positive);
			}
			if (buttonPressed == CustomMapKeyboardBinding.rateDown)
			{
				this.currentMapMod.RateMod((this.currentMapMod.CurrentUserRating == ModRating.Negative) ? ModRating.None : ModRating.Negative);
			}
			return;
		}
	}

	// Token: 0x060045FD RID: 17917 RVA: 0x001783B4 File Offset: 0x001765B4
	private void RefreshCurrentMapMod()
	{
		if (CustomMapLoader.IsMapLoaded() || CustomMapManager.IsLoading() || CustomMapManager.IsUnloading())
		{
			return;
		}
		if (this.hasModProfile)
		{
			long id = this.GetModId()._id;
			this.hasModProfile = false;
			this.currentMapMod.OnModUpdated -= this.OnModUpdated;
			this.currentMapMod = null;
			this.ResetToDefaultView();
			this.RetrieveModFromModIO(id, true, null);
		}
	}

	// Token: 0x060045FE RID: 17918 RVA: 0x00178420 File Offset: 0x00176620
	private void OnProfileReceived(Error error, Mod mod)
	{
		if (error)
		{
			this.modDescriptionText.gameObject.SetActive(false);
			this.errorText.text = string.Format("FAILED TO RETRIEVE MOD DETAILS FOR MOD: {0}", this.GetModId());
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.SetModProfile(mod);
	}

	// Token: 0x060045FF RID: 17919 RVA: 0x00178480 File Offset: 0x00176680
	private void ResetToDefaultView()
	{
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.loadingMapMessageText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(false);
		this.errorText.gameObject.SetActive(false);
		this.modNameText.gameObject.SetActive(false);
		this.modCreatorLabelText.gameObject.SetActive(false);
		this.modCreatorText.gameObject.SetActive(false);
		this.modDescriptionText.gameObject.SetActive(false);
		this.modStatusText.gameObject.SetActive(false);
		this.modSubscriptionStatusText.gameObject.SetActive(false);
		this.mapScreenshotImage.gameObject.SetActive(false);
		this.hiddenRoomMapText.gameObject.SetActive(false);
		this.outdatedText.gameObject.SetActive(false);
		this.unloadPromptText.gameObject.SetActive(false);
		this.playerCountText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(true);
		if (CustomMapLoader.IsMapLoaded() || CustomMapManager.IsLoading() || CustomMapManager.IsUnloading())
		{
			ModId modId = new ModId(CustomMapLoader.IsMapLoaded() ? CustomMapLoader.LoadedMapModId : (CustomMapManager.IsLoading() ? CustomMapManager.LoadingMapId : CustomMapManager.UnloadingMapId));
			if (this.hasModProfile && this.GetModId() == modId)
			{
				this.UpdateMapDetails(true);
				return;
			}
			this.RetrieveModFromModIO(modId, false, delegate(Error error, Mod mod)
			{
				this.OnProfileReceived(error, mod);
			});
			return;
		}
		else
		{
			if (CustomMapManager.GetRoomMapId() != ModId.Null)
			{
				this.OnRoomMapChanged(CustomMapManager.GetRoomMapId());
				return;
			}
			if (this.hasModProfile)
			{
				this.UpdateMapDetails(true);
			}
			return;
		}
	}

	// Token: 0x06004600 RID: 17920 RVA: 0x00178644 File Offset: 0x00176844
	private void UpdateMapDetails(bool refreshScreenState = true)
	{
		if (!this.hasModProfile)
		{
			return;
		}
		if (this.IsCurrentModHidden())
		{
			this.modNameText.text = this.hiddenMapTitle;
			this.modDescriptionText.text = this.hiddenMapDesc;
			this.modCreatorLabelText.gameObject.SetActive(false);
			this.modCreatorText.text = "";
			this.mapScreenshotImage.sprite = this.hiddenMapLogo;
			this.mapScreenshotImage.gameObject.SetActive(true);
		}
		else
		{
			this.modNameText.text = this.currentMapMod.Name;
			this.modDescriptionText.text = this.currentMapMod.Description;
			this.modCreatorLabelText.gameObject.SetActive(true);
			this.modCreatorText.text = this.currentMapMod.Creator.Username;
			ModIOManager.GetModLogo(this.currentMapMod, new Action<Error, Texture2D>(this.OnGetModLogo));
		}
		this.UpdateStatus(false);
		if (refreshScreenState)
		{
			this.loadingText.gameObject.SetActive(false);
			this.loadingMapLabelText.gameObject.SetActive(false);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.hiddenRoomMapText.gameObject.SetActive(false);
			this.mapReadyText.gameObject.SetActive(false);
			this.unloadPromptText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(false);
			this.modNameText.gameObject.SetActive(true);
			this.modDescriptionText.gameObject.SetActive(true);
			if (!this.IsCurrentModHidden())
			{
				this.modCreatorLabelText.gameObject.SetActive(true);
				this.modCreatorText.gameObject.SetActive(true);
			}
			if (CustomMapLoader.IsMapLoaded())
			{
				ModId modId = new ModId(CustomMapLoader.LoadedMapModId);
				if (this.GetModId() == modId)
				{
					this.OnMapLoadComplete_UIUpdate();
					return;
				}
				this.RetrieveModFromModIO(modId, false, delegate(Error error, Mod mod)
				{
					this.OnProfileReceived(error, mod);
				});
				return;
			}
			else
			{
				if (CustomMapManager.IsLoading() && !this.mapLoadError)
				{
					this.modDescriptionText.gameObject.SetActive(false);
					if (!CustomMapManager.IsUnloading())
					{
						this.loadingMapLabelText.text = this.mapLoadingString + " 0%";
					}
					else
					{
						this.loadingMapLabelText.text = this.mapUnloadingString;
					}
					this.loadingMapLabelText.gameObject.SetActive(true);
					return;
				}
				if (CustomMapManager.IsUnloading())
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.loadingMapLabelText.text = this.mapUnloadingString;
					this.loadingMapLabelText.gameObject.SetActive(true);
					return;
				}
				if (CustomMapManager.GetRoomMapId() != ModId.Null)
				{
					this.ShowLoadRoomMapPrompt();
					return;
				}
				if (this.mapLoadError)
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.loadingMapLabelText.gameObject.SetActive(true);
					this.loadingMapMessageText.gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x06004601 RID: 17921 RVA: 0x00178948 File Offset: 0x00176B48
	private void OnGetModLogo(Error error, Texture2D modLogo)
	{
		if (error)
		{
			Debug.LogError(string.Format("[CustomMapsDetailsScreen::OnGetModLogo] Failed to retrieve logo for Mod {0}", this.GetModId()));
			return;
		}
		this.mapScreenshotImage.sprite = Sprite.Create(modLogo, new Rect(0f, 0f, 320f, 180f), new Vector2(0.5f, 0.5f));
		this.mapScreenshotImage.gameObject.SetActive(true);
	}

	// Token: 0x06004602 RID: 17922 RVA: 0x001789C4 File Offset: 0x00176BC4
	private async Task UpdateStatus(bool errorEncountered = false)
	{
		if (base.isActiveAndEnabled && this.currentMapMod != null)
		{
			this.outdatedText.gameObject.SetActive(false);
			this.deleteButton.gameObject.SetActive(false);
			this.subscriptionToggleButton.gameObject.SetActive(false);
			this.favoriteToggleButton.gameObject.SetActive(false);
			this.rateUpButton.gameObject.SetActive(false);
			this.rateDownButton.gameObject.SetActive(false);
			this.modSubscriptionStatusText.gameObject.SetActive(false);
			TMP_Text tmp_Text = this.modStatusLabelText;
			if (tmp_Text != null)
			{
				tmp_Text.gameObject.SetActive(false);
			}
			this.modStatusText.gameObject.SetActive(false);
			if (this.mapLoadError || CustomMapManager.IsUnloading() || CustomMapManager.IsLoading() || CustomMapLoader.IsMapLoaded() || CustomMapManager.GetRoomMapId() != ModId.Null || this.IsCurrentModHidden())
			{
				CustomMapsScreenButton customMapsScreenButton = this.loadButton;
				if (customMapsScreenButton != null)
				{
					customMapsScreenButton.gameObject.SetActive(false);
				}
				if (ModIOManager.IsModFavorited(this.currentMapMod.Id))
				{
					this.favoriteToggleButton.SetButtonActive(true);
					this.favoriteToggleButton.gameObject.SetActive(CustomMapsTerminal.IsDriver);
				}
				if (this.currentMapMod.File.State == ModFileState.Installed)
				{
					this.deleteButton.gameObject.SetActive(CustomMapsTerminal.IsDriver);
				}
			}
			else
			{
				CustomMapsScreenButton customMapsScreenButton2 = this.loadButton;
				if (customMapsScreenButton2 != null)
				{
					customMapsScreenButton2.gameObject.SetActive(true);
				}
				this.isFavorite = ModIOManager.IsModFavorited(this.currentMapMod.Id);
				this.favoriteToggleButton.SetButtonActive(this.isFavorite);
				this.favoriteToggleButton.gameObject.SetActive(CustomMapsTerminal.IsDriver);
				this.rateUpButton.gameObject.SetActive(CustomMapsTerminal.IsDriver);
				this.rateDownButton.gameObject.SetActive(CustomMapsTerminal.IsDriver);
				ModFileState modFileState = (errorEncountered ? ModFileState.FileOperationFailed : this.currentMapMod.File.State);
				this.modStatusText.text = CustomMapsDetailsScreen.modStatusStrings.GetValueOrDefault(modFileState, "STATUS STRING MISSING!");
				if (ModIOManager.IsLoggedIn())
				{
					this.modSubscriptionStatusText.text = (this.currentMapMod.IsSubscribed ? this.subscribedStatusString : this.unsubscribedStatusString);
					this.modSubscriptionStatusText.gameObject.SetActive(true);
					this.subscriptionToggleButton.SetButtonActive(this.currentMapMod.IsSubscribed);
					this.subscriptionToggleButton.SetButtonText(this.currentMapMod.IsSubscribed ? this.unsubscribeString : this.subscribeString);
					this.subscriptionToggleButton.gameObject.SetActive(CustomMapsTerminal.IsDriver);
				}
				if (modFileState != ModFileState.None)
				{
					if (modFileState != ModFileState.Queued)
					{
						if (modFileState == ModFileState.Installed)
						{
							CustomMapsScreenButton customMapsScreenButton3 = this.loadButton;
							if (customMapsScreenButton3 != null)
							{
								customMapsScreenButton3.SetButtonText(this.loadMapString);
							}
							this.deleteButton.gameObject.SetActive(CustomMapsTerminal.IsDriver);
							TaskAwaiter<ValueTuple<bool, int>> taskAwaiter = ModIOManager.IsModOutdated(this.GetModId()).GetAwaiter();
							if (!taskAwaiter.IsCompleted)
							{
								await taskAwaiter;
								TaskAwaiter<ValueTuple<bool, int>> taskAwaiter2;
								taskAwaiter = taskAwaiter2;
								taskAwaiter2 = default(TaskAwaiter<ValueTuple<bool, int>>);
							}
							bool item = taskAwaiter.GetResult().Item1;
							this.outdatedText.gameObject.SetActive(item);
						}
					}
					else
					{
						bool flag = ModInstallationManagement.DoesModNeedUpdate(this.currentMapMod);
						CustomMapsScreenButton customMapsScreenButton4 = this.loadButton;
						if (customMapsScreenButton4 != null)
						{
							customMapsScreenButton4.SetButtonText(flag ? this.updateMapString : this.downloadMapString);
						}
						this.modStatusText.text = (flag ? this.mapNeedsUpdateString : this.mapNotDownloadedString);
					}
				}
				else
				{
					CustomMapsScreenButton customMapsScreenButton5 = this.loadButton;
					if (customMapsScreenButton5 != null)
					{
						customMapsScreenButton5.SetButtonText(this.downloadMapString);
					}
				}
				TMP_Text tmp_Text2 = this.modStatusLabelText;
				if (tmp_Text2 != null)
				{
					tmp_Text2.gameObject.SetActive(true);
				}
				this.modStatusText.gameObject.SetActive(true);
				if (this.currentMapMod != null)
				{
					this.playerCountText.gameObject.SetActive(true);
					PlayerCountHelper.GetPlayerCount(this.currentMapMod, delegate(string count)
					{
						this.playerCountText.text = count;
					}, null);
				}
			}
		}
	}

	// Token: 0x06004603 RID: 17923 RVA: 0x00178A10 File Offset: 0x00176C10
	private bool CanChangeMapState(bool load, out string disallowedReason)
	{
		disallowedReason = "";
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			if (!CustomMapManager.AreAllPlayersInVirtualStump())
			{
				disallowedReason = "ALL PLAYERS IN THE ROOM MUST BE INSIDE THE VIRTUAL STUMP BEFORE " + (load ? "" : "UN") + "LOADING A MAP.";
				return false;
			}
			return true;
		}
		else
		{
			if (!CustomMapManager.IsLocalPlayerInVirtualStump())
			{
				disallowedReason = "YOU MUST BE INSIDE THE VIRTUAL STUMP TO " + (load ? "" : "UN") + "LOAD A MAP.";
				return false;
			}
			return true;
		}
	}

	// Token: 0x06004604 RID: 17924 RVA: 0x00178A94 File Offset: 0x00176C94
	private void LoadMap()
	{
		this.modDescriptionText.gameObject.SetActive(false);
		this.modStatusText.gameObject.SetActive(false);
		this.modSubscriptionStatusText.gameObject.SetActive(false);
		this.outdatedText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(true);
		if (NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
		{
			NetworkSystem.Instance.ReturnToSinglePlayer();
		}
		this.deleteButton.gameObject.SetActive(false);
		this.subscriptionToggleButton.gameObject.SetActive(false);
		GTMapLoadSource gtmapLoadSource = ((CustomMapsTerminal.PreviousScreen == CustomMapsTerminal.ScreenType.SearchMods) ? GTMapLoadSource.terminal_search : GTMapLoadSource.terminal_browse);
		this.networkObject.LoadMapSynced(this.GetModId(), gtmapLoadSource);
	}

	// Token: 0x06004605 RID: 17925 RVA: 0x00178B5F File Offset: 0x00176D5F
	private void UnloadMap()
	{
		this.networkObject.UnloadMapSynced();
	}

	// Token: 0x06004606 RID: 17926 RVA: 0x00178B6C File Offset: 0x00176D6C
	public void OnMapLoadComplete(bool success)
	{
		if (success)
		{
			this.OnMapLoadComplete_UIUpdate();
		}
	}

	// Token: 0x06004607 RID: 17927 RVA: 0x00178B78 File Offset: 0x00176D78
	private void OnMapLoadComplete_UIUpdate()
	{
		this.modDescriptionText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.loadingMapMessageText.gameObject.SetActive(false);
		this.hiddenRoomMapText.gameObject.SetActive(false);
		this.errorText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(true);
		this.unloadPromptText.gameObject.SetActive(true);
	}

	// Token: 0x06004608 RID: 17928 RVA: 0x00178BFC File Offset: 0x00176DFC
	private void OnMapUnloaded()
	{
		this.mapLoadError = false;
		this.loadingMapMessageText.fontSize = 40f;
		this.UpdateMapDetails(true);
	}

	// Token: 0x06004609 RID: 17929 RVA: 0x00178C1C File Offset: 0x00176E1C
	private void OnRoomMapChanged(ModId roomMapID)
	{
		if (roomMapID == ModId.Null)
		{
			this.UpdateMapDetails(true);
			return;
		}
		if (this.GetModId() != roomMapID)
		{
			this.RetrieveModFromModIO(roomMapID, false, new Action<Error, Mod>(this.OnRoomMapRetrieved));
			return;
		}
		this.ShowLoadRoomMapPrompt();
	}

	// Token: 0x0600460A RID: 17930 RVA: 0x00178C6C File Offset: 0x00176E6C
	private void OnRoomMapRetrieved(Error error, Mod mod)
	{
		this.OnProfileReceived(error, mod);
		if (!error)
		{
			this.ShowLoadRoomMapPrompt();
		}
	}

	// Token: 0x0600460B RID: 17931 RVA: 0x00178C84 File Offset: 0x00176E84
	private void ShowLoadRoomMapPrompt()
	{
		if (CustomMapManager.IsUnloading() || CustomMapManager.IsLoading() || CustomMapLoader.IsMapLoaded(this.GetModId()))
		{
			return;
		}
		this.modDescriptionText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(false);
		this.unloadPromptText.gameObject.SetActive(false);
		this.hiddenRoomMapText.gameObject.SetActive(false);
		if (this.IsCurrentModHidden())
		{
			this.hiddenRoomMapText.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600460C RID: 17932 RVA: 0x00178D2C File Offset: 0x00176F2C
	public void OnMapLoadProgress(MapLoadStatus loadStatus, int progress, string message)
	{
		if (loadStatus != MapLoadStatus.None)
		{
			this.mapLoadError = false;
			this.loadingMapMessageText.fontSize = 40f;
			this.hiddenRoomMapText.gameObject.SetActive(false);
			this.modDescriptionText.gameObject.SetActive(false);
		}
		switch (loadStatus)
		{
		case MapLoadStatus.None:
			if (!this.mapLoadError)
			{
				this.loadingMapLabelText.gameObject.SetActive(false);
				this.loadingMapMessageText.gameObject.SetActive(false);
				return;
			}
			break;
		case MapLoadStatus.Downloading:
			this.loadingMapLabelText.text = ((progress > 0) ? (this.mapDownloadingProgressString + " " + progress.ToString() + "%") : this.mapAutoDownloadingString);
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadingMapMessageText.text = "";
			return;
		case MapLoadStatus.Loading:
			this.loadingMapLabelText.text = this.mapLoadingString + " " + progress.ToString() + "%";
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.text = message;
			this.loadingMapMessageText.gameObject.SetActive(true);
			return;
		case MapLoadStatus.Unloading:
			this.mapReadyText.gameObject.SetActive(false);
			this.unloadPromptText.gameObject.SetActive(false);
			this.loadingMapLabelText.text = this.mapUnloadingString;
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadingMapMessageText.text = "";
			return;
		case MapLoadStatus.Error:
			this.mapLoadError = true;
			this.loadingMapLabelText.text = this.mapLoadingErrorString;
			this.loadingMapLabelText.gameObject.SetActive(true);
			if (CustomMapsTerminal.IsDriver)
			{
				this.loadingMapMessageText.text = message + "\n" + this.mapLoadingErrorDriverString;
			}
			else
			{
				this.loadingMapMessageText.text = message + "\n" + this.mapLoadingErrorNonDriverString;
			}
			if (this.loadingMapMessageText.text.Length > 150)
			{
				this.loadingMapMessageText.fontSize = 30f;
			}
			else
			{
				this.loadingMapMessageText.fontSize = 40f;
			}
			this.loadingMapMessageText.gameObject.SetActive(true);
			break;
		case MapLoadStatus.Installing:
			this.loadingMapLabelText.text = ((progress > 0) ? (this.mapInstallingProgressString + " " + progress.ToString() + "%") : this.mapInstallingString);
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadingMapMessageText.text = "";
			return;
		default:
			return;
		}
	}

	// Token: 0x0600460D RID: 17933 RVA: 0x00178FFE File Offset: 0x001771FE
	public ModId GetModId()
	{
		Mod currentMapMod = this.currentMapMod;
		if (currentMapMod == null)
		{
			return ModId.Null;
		}
		return currentMapMod.Id;
	}

	// Token: 0x0600460E RID: 17934 RVA: 0x00179015 File Offset: 0x00177215
	public bool IsCurrentModHidden()
	{
		return this.hasModProfile && (this.currentMapMod.Creator == null || (!ModIOManager.IsLoggedIn() && this.currentMapMod.IsHidden()));
	}

	// Token: 0x04005833 RID: 22579
	[SerializeField]
	private SpriteRenderer mapScreenshotImage;

	// Token: 0x04005834 RID: 22580
	[SerializeField]
	private Sprite hiddenMapLogo;

	// Token: 0x04005835 RID: 22581
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x04005836 RID: 22582
	[SerializeField]
	private TMP_Text modNameText;

	// Token: 0x04005837 RID: 22583
	[SerializeField]
	private TMP_Text modCreatorLabelText;

	// Token: 0x04005838 RID: 22584
	[SerializeField]
	private TMP_Text modCreatorText;

	// Token: 0x04005839 RID: 22585
	[SerializeField]
	private TMP_Text modDescriptionText;

	// Token: 0x0400583A RID: 22586
	[SerializeField]
	private TMP_Text modStatusText;

	// Token: 0x0400583B RID: 22587
	[SerializeField]
	private TMP_Text modStatusLabelText;

	// Token: 0x0400583C RID: 22588
	[SerializeField]
	private TMP_Text modSubscriptionStatusText;

	// Token: 0x0400583D RID: 22589
	[SerializeField]
	private TMP_Text loadingMapLabelText;

	// Token: 0x0400583E RID: 22590
	[SerializeField]
	private TMP_Text loadingMapMessageText;

	// Token: 0x0400583F RID: 22591
	[SerializeField]
	private TMP_Text hiddenRoomMapText;

	// Token: 0x04005840 RID: 22592
	[SerializeField]
	private TMP_Text mapReadyText;

	// Token: 0x04005841 RID: 22593
	[SerializeField]
	private TMP_Text unloadPromptText;

	// Token: 0x04005842 RID: 22594
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x04005843 RID: 22595
	[SerializeField]
	private TMP_Text outdatedText;

	// Token: 0x04005844 RID: 22596
	[SerializeField]
	private TMP_Text playerCountText;

	// Token: 0x04005845 RID: 22597
	[SerializeField]
	private CustomMapsScreenButton subscriptionToggleButton;

	// Token: 0x04005846 RID: 22598
	[SerializeField]
	private CustomMapsScreenButton favoriteToggleButton;

	// Token: 0x04005847 RID: 22599
	[SerializeField]
	private CustomMapsScreenButton rateUpButton;

	// Token: 0x04005848 RID: 22600
	[SerializeField]
	private CustomMapsScreenButton rateDownButton;

	// Token: 0x04005849 RID: 22601
	[SerializeField]
	private CustomMapsScreenButton loadButton;

	// Token: 0x0400584A RID: 22602
	[SerializeField]
	private CustomMapsScreenButton deleteButton;

	// Token: 0x0400584B RID: 22603
	[SerializeField]
	private string modAvailableString = "AVAILABLE";

	// Token: 0x0400584C RID: 22604
	[SerializeField]
	private string mapAutoDownloadingString = "DOWNLOADING...";

	// Token: 0x0400584D RID: 22605
	[SerializeField]
	private string mapDownloadingProgressString = "DOWNLOADING:";

	// Token: 0x0400584E RID: 22606
	[SerializeField]
	private string mapInstallingString = "INSTALLING...";

	// Token: 0x0400584F RID: 22607
	[SerializeField]
	private string mapInstallingProgressString = "INSTALLING:";

	// Token: 0x04005850 RID: 22608
	[SerializeField]
	private string mapDownloadQueuedString = "DOWNLOAD QUEUED";

	// Token: 0x04005851 RID: 22609
	[SerializeField]
	private string mapLoadingString = "LOADING:";

	// Token: 0x04005852 RID: 22610
	[SerializeField]
	private string mapUnloadingString = "UNLOADING...";

	// Token: 0x04005853 RID: 22611
	[SerializeField]
	private string mapLoadingErrorString = "ERROR:";

	// Token: 0x04005854 RID: 22612
	[SerializeField]
	private string mapLoadingErrorDriverString = "PRESS THE 'BACK' BUTTON TO TRY AGAIN";

	// Token: 0x04005855 RID: 22613
	[SerializeField]
	private string mapLoadingErrorNonDriverString = "LEAVE AND REJOIN THE VIRTUAL STUMP TO TRY AGAIN";

	// Token: 0x04005856 RID: 22614
	[SerializeField]
	private string mapLoadingErrorInvalidModFile = "INSTALL FAILED DUE TO INVALID MAP FILE";

	// Token: 0x04005857 RID: 22615
	[SerializeField]
	private VirtualStumpSerializer networkObject;

	// Token: 0x04005858 RID: 22616
	public static Dictionary<ModFileState, string> modStatusStrings = new Dictionary<ModFileState, string>
	{
		{
			ModFileState.Installed,
			"READY"
		},
		{
			ModFileState.Queued,
			"QUEUED"
		},
		{
			ModFileState.Downloading,
			"DOWNLOADING"
		},
		{
			ModFileState.Downloaded,
			"DOWNLOADED"
		},
		{
			ModFileState.Installing,
			"INSTALLING"
		},
		{
			ModFileState.Uninstalling,
			"UNINSTALLING"
		},
		{
			ModFileState.Updating,
			"UPDATING"
		},
		{
			ModFileState.FileOperationFailed,
			"ERROR"
		},
		{
			ModFileState.None,
			"AVAILABLE"
		}
	};

	// Token: 0x04005859 RID: 22617
	[SerializeField]
	private string mapNotDownloadedString = "NOT DOWNLOADED";

	// Token: 0x0400585A RID: 22618
	[SerializeField]
	private string mapNeedsUpdateString = "NEEDS UPDATE";

	// Token: 0x0400585B RID: 22619
	[SerializeField]
	private string subscribeString = "SUBSCRIBE";

	// Token: 0x0400585C RID: 22620
	[SerializeField]
	private string unsubscribeString = "UNSUBSCRIBE";

	// Token: 0x0400585D RID: 22621
	[SerializeField]
	private string subscribedStatusString = "SUBSCRIBED";

	// Token: 0x0400585E RID: 22622
	[SerializeField]
	private string unsubscribedStatusString = "NOT SUBSCRIBED";

	// Token: 0x0400585F RID: 22623
	[SerializeField]
	private string loadMapString = "LOAD";

	// Token: 0x04005860 RID: 22624
	[SerializeField]
	private string downloadMapString = "DOWNLOAD";

	// Token: 0x04005861 RID: 22625
	[SerializeField]
	private string updateMapString = "UPDATE";

	// Token: 0x04005862 RID: 22626
	[SerializeField]
	private string hiddenMapTitle = "HIDDEN MAP";

	// Token: 0x04005863 RID: 22627
	[SerializeField]
	private string hiddenMapDesc = "YOU DON'T CURRENTLY HAVE ACCESS TO THIS HIDDEN MAP.\nCHECK THAT YOU'RE LOGGED IN TO THE CORRECT MOD.IO ACCOUNT.";

	// Token: 0x04005864 RID: 22628
	private const float LOGO_WIDTH = 320f;

	// Token: 0x04005865 RID: 22629
	private const float LOGO_HEIGHT = 180f;

	// Token: 0x04005866 RID: 22630
	public long pendingModId;

	// Token: 0x04005868 RID: 22632
	private bool hasModProfile;

	// Token: 0x04005869 RID: 22633
	private bool mapLoadError;

	// Token: 0x0400586A RID: 22634
	private bool isFavorite;
}
