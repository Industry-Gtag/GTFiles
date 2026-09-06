using System;
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

// Token: 0x02000AB1 RID: 2737
public class CustomMapsDisplayScreen : CustomMapsTerminalScreen
{
	// Token: 0x17000698 RID: 1688
	// (get) Token: 0x0600461D RID: 17949 RVA: 0x0017972F File Offset: 0x0017792F
	// (set) Token: 0x0600461C RID: 17948 RVA: 0x00179726 File Offset: 0x00177926
	public Mod currentMapMod { get; private set; }

	// Token: 0x0600461E RID: 17950 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void Initialize()
	{
	}

	// Token: 0x0600461F RID: 17951 RVA: 0x00179738 File Offset: 0x00177938
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
		this.ResetToDefaultView();
	}

	// Token: 0x06004620 RID: 17952 RVA: 0x001798B4 File Offset: 0x00177AB4
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

	// Token: 0x06004621 RID: 17953 RVA: 0x00179977 File Offset: 0x00177B77
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

	// Token: 0x06004622 RID: 17954 RVA: 0x001799B0 File Offset: 0x00177BB0
	private void OnModIOLoggedOut()
	{
		if (this.currentMapMod.IsHidden())
		{
			this.UpdateMapDetails(true);
			return;
		}
		this.UpdateStatus(false);
	}

	// Token: 0x06004623 RID: 17955 RVA: 0x001799CF File Offset: 0x00177BCF
	private void OnModIOUserChanged(User user)
	{
		this.UpdateStatus(false);
	}

	// Token: 0x06004624 RID: 17956 RVA: 0x001799DC File Offset: 0x00177BDC
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

	// Token: 0x06004625 RID: 17957 RVA: 0x00179A80 File Offset: 0x00177C80
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

	// Token: 0x06004626 RID: 17958 RVA: 0x00179AD1 File Offset: 0x00177CD1
	public void SetModProfile(Mod mod)
	{
		if (mod.Id != ModId.Null)
		{
			this.pendingModId = 0L;
			this.currentMapMod = mod;
			this.hasModProfile = true;
			this.UpdateMapDetails(true);
		}
	}

	// Token: 0x06004627 RID: 17959 RVA: 0x00179B04 File Offset: 0x00177D04
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
			this.currentMapMod = null;
			this.ResetToDefaultView();
			this.RetrieveModFromModIO(id, true, null);
		}
	}

	// Token: 0x06004628 RID: 17960 RVA: 0x00179B58 File Offset: 0x00177D58
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

	// Token: 0x06004629 RID: 17961 RVA: 0x00179BB8 File Offset: 0x00177DB8
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
		this.mapScreenshotImage.gameObject.SetActive(false);
		this.loadRoomMapPromptText.gameObject.SetActive(false);
		this.hiddenRoomMapText.gameObject.SetActive(false);
		this.outdatedText.gameObject.SetActive(false);
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

	// Token: 0x0600462A RID: 17962 RVA: 0x00179D5C File Offset: 0x00177F5C
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
			this.modCreatorText.text = this.currentMapMod.Creator.Username;
			ModIOManager.GetModLogo(this.currentMapMod, new Action<Error, Texture2D>(this.OnGetModLogo));
		}
		this.UpdateStatus(false);
		if (refreshScreenState)
		{
			this.loadingText.gameObject.SetActive(false);
			this.loadingMapLabelText.gameObject.SetActive(false);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadRoomMapPromptText.gameObject.SetActive(false);
			this.hiddenRoomMapText.gameObject.SetActive(false);
			this.mapReadyText.gameObject.SetActive(false);
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

	// Token: 0x0600462B RID: 17963 RVA: 0x0017A050 File Offset: 0x00178250
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

	// Token: 0x0600462C RID: 17964 RVA: 0x0017A0CC File Offset: 0x001782CC
	private async Task UpdateStatus(bool errorEncountered = false)
	{
		if (base.isActiveAndEnabled && this.currentMapMod != null)
		{
			this.outdatedText.gameObject.SetActive(false);
			ModFileState modFileState = (errorEncountered ? ModFileState.FileOperationFailed : this.currentMapMod.File.State);
			if (modFileState > ModFileState.Queued && modFileState == ModFileState.Installed)
			{
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
			if (this.currentMapMod != null)
			{
				this.playerCountText.gameObject.SetActive(true);
				PlayerCountHelper.GetPlayerCount(this.currentMapMod, delegate(string count)
				{
					this.playerCountText.text = count;
				}, null);
			}
			else
			{
				this.playerCountText.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600462D RID: 17965 RVA: 0x0017A117 File Offset: 0x00178317
	public void OnMapLoadComplete(bool success)
	{
		if (success)
		{
			this.OnMapLoadComplete_UIUpdate();
		}
	}

	// Token: 0x0600462E RID: 17966 RVA: 0x0017A124 File Offset: 0x00178324
	private void OnMapLoadComplete_UIUpdate()
	{
		this.modDescriptionText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.loadingMapMessageText.gameObject.SetActive(false);
		this.loadRoomMapPromptText.gameObject.SetActive(false);
		this.hiddenRoomMapText.gameObject.SetActive(false);
		this.errorText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(true);
	}

	// Token: 0x0600462F RID: 17967 RVA: 0x0017A1A8 File Offset: 0x001783A8
	private void OnMapUnloaded()
	{
		this.mapLoadError = false;
		this.loadingMapMessageText.fontSize = 80f;
		this.UpdateMapDetails(true);
	}

	// Token: 0x06004630 RID: 17968 RVA: 0x0017A1C8 File Offset: 0x001783C8
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

	// Token: 0x06004631 RID: 17969 RVA: 0x0017A218 File Offset: 0x00178418
	private void OnRoomMapRetrieved(Error error, Mod mod)
	{
		this.OnProfileReceived(error, mod);
		if (!error)
		{
			this.ShowLoadRoomMapPrompt();
		}
	}

	// Token: 0x06004632 RID: 17970 RVA: 0x0017A230 File Offset: 0x00178430
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
		this.hiddenRoomMapText.gameObject.SetActive(false);
		this.loadRoomMapPromptText.gameObject.SetActive(false);
		if (this.IsCurrentModHidden())
		{
			this.hiddenRoomMapText.gameObject.SetActive(true);
			return;
		}
		this.loadRoomMapPromptText.gameObject.SetActive(true);
	}

	// Token: 0x06004633 RID: 17971 RVA: 0x0017A2EC File Offset: 0x001784EC
	public void OnMapLoadProgress(MapLoadStatus loadStatus, int progress, string message)
	{
		if (loadStatus != MapLoadStatus.None)
		{
			this.mapLoadError = false;
			this.loadingMapMessageText.fontSize = 80f;
			this.hiddenRoomMapText.gameObject.SetActive(false);
			this.loadRoomMapPromptText.gameObject.SetActive(false);
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
				this.loadingMapMessageText.fontSize = 60f;
			}
			else
			{
				this.loadingMapMessageText.fontSize = 80f;
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

	// Token: 0x06004634 RID: 17972 RVA: 0x0017A5BE File Offset: 0x001787BE
	public ModId GetModId()
	{
		Mod currentMapMod = this.currentMapMod;
		if (currentMapMod == null)
		{
			return ModId.Null;
		}
		return currentMapMod.Id;
	}

	// Token: 0x06004635 RID: 17973 RVA: 0x0017A5D5 File Offset: 0x001787D5
	public bool IsCurrentModHidden()
	{
		return this.hasModProfile && (this.currentMapMod.Creator == null || (!ModIOManager.IsLoggedIn() && this.currentMapMod.IsHidden()));
	}

	// Token: 0x04005870 RID: 22640
	[SerializeField]
	private SpriteRenderer mapScreenshotImage;

	// Token: 0x04005871 RID: 22641
	[SerializeField]
	private Sprite hiddenMapLogo;

	// Token: 0x04005872 RID: 22642
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x04005873 RID: 22643
	[SerializeField]
	private TMP_Text modNameText;

	// Token: 0x04005874 RID: 22644
	[SerializeField]
	private TMP_Text modCreatorLabelText;

	// Token: 0x04005875 RID: 22645
	[SerializeField]
	private TMP_Text modCreatorText;

	// Token: 0x04005876 RID: 22646
	[SerializeField]
	private TMP_Text modDescriptionText;

	// Token: 0x04005877 RID: 22647
	[SerializeField]
	private TMP_Text loadingMapLabelText;

	// Token: 0x04005878 RID: 22648
	[SerializeField]
	private TMP_Text loadingMapMessageText;

	// Token: 0x04005879 RID: 22649
	[SerializeField]
	private TMP_Text loadRoomMapPromptText;

	// Token: 0x0400587A RID: 22650
	[SerializeField]
	private TMP_Text hiddenRoomMapText;

	// Token: 0x0400587B RID: 22651
	[SerializeField]
	private TMP_Text mapReadyText;

	// Token: 0x0400587C RID: 22652
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x0400587D RID: 22653
	[SerializeField]
	private TMP_Text outdatedText;

	// Token: 0x0400587E RID: 22654
	[SerializeField]
	private TMP_Text playerCountText;

	// Token: 0x0400587F RID: 22655
	[SerializeField]
	private string mapAutoDownloadingString = "DOWNLOADING...";

	// Token: 0x04005880 RID: 22656
	[SerializeField]
	private string mapDownloadingProgressString = "DOWNLOADING:";

	// Token: 0x04005881 RID: 22657
	[SerializeField]
	private string mapInstallingString = "INSTALLING...";

	// Token: 0x04005882 RID: 22658
	[SerializeField]
	private string mapInstallingProgressString = "INSTALLING:";

	// Token: 0x04005883 RID: 22659
	[SerializeField]
	private string mapLoadingString = "LOADING:";

	// Token: 0x04005884 RID: 22660
	[SerializeField]
	private string mapUnloadingString = "UNLOADING...";

	// Token: 0x04005885 RID: 22661
	[SerializeField]
	private string mapLoadingErrorString = "ERROR:";

	// Token: 0x04005886 RID: 22662
	[SerializeField]
	private string mapLoadingErrorDriverString = "PRESS THE 'BACK' BUTTON TO TRY AGAIN";

	// Token: 0x04005887 RID: 22663
	[SerializeField]
	private string mapLoadingErrorNonDriverString = "LEAVE AND REJOIN THE VIRTUAL STUMP TO TRY AGAIN";

	// Token: 0x04005888 RID: 22664
	[SerializeField]
	private string mapLoadingErrorInvalidModFile = "INSTALL FAILED DUE TO INVALID MAP FILE";

	// Token: 0x04005889 RID: 22665
	[SerializeField]
	private string mapNotDownloadedString = "NOT DOWNLOADED";

	// Token: 0x0400588A RID: 22666
	[SerializeField]
	private string mapNeedsUpdateString = "NEEDS UPDATE";

	// Token: 0x0400588B RID: 22667
	[SerializeField]
	private string hiddenMapTitle = "HIDDEN MAP";

	// Token: 0x0400588C RID: 22668
	[SerializeField]
	private string hiddenMapDesc = "YOU DON'T CURRENTLY HAVE ACCESS TO THIS HIDDEN MAP.\nCHECK THAT YOU'RE LOGGED IN TO THE CORRECT MOD.IO ACCOUNT.";

	// Token: 0x0400588D RID: 22669
	private const float LOGO_WIDTH = 320f;

	// Token: 0x0400588E RID: 22670
	private const float LOGO_HEIGHT = 180f;

	// Token: 0x0400588F RID: 22671
	public long pendingModId;

	// Token: 0x04005891 RID: 22673
	private bool hasModProfile;

	// Token: 0x04005892 RID: 22674
	private bool mapLoadError;

	// Token: 0x04005893 RID: 22675
	private bool isFavorite;
}
