using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GorillaTagScripts.VirtualStumpCustomMaps.ModIO;
using GT_CustomMapSupportRuntime;
using Modio;
using Modio.API;
using Modio.Authentication;
using Modio.Customizations;
using Modio.Errors;
using Modio.FileIO;
using Modio.Mods;
using Modio.Unity;
using Modio.Users;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

// Token: 0x02000A7D RID: 2685
public class ModIOManager : MonoBehaviour, ISteamCredentialProvider, IOculusCredentialProvider
{
	// Token: 0x06004526 RID: 17702 RVA: 0x00170EB4 File Offset: 0x0016F0B4
	private void Awake()
	{
		if (ModIOManager.instance == null)
		{
			ModIOManager.instance = this;
			ModIOManager.hasInstance = true;
			UGCPermissionManager.SubscribeToUGCEnabled(new Action(ModIOManager.OnUGCEnabled));
			UGCPermissionManager.SubscribeToUGCDisabled(new Action(ModIOManager.OnUGCDisabled));
			UGCPermissionManager.SubscribeToVirtualStumpEnabled(new Action(ModIOManager.OnMapAccessEnabled));
			ModioServices.Bind<IModioAuthService>().FromInstance(ModIOManager.accountLinkingAuthService, (ModioServicePriority)41, null);
			ModioServices.Bind<IModioAuthService>().FromInstance(ModIOManager.steamAuthService, ModioServicePriority.DeveloperOverride, null);
			long gameId = ModioServices.Resolve<ModioSettings>().GameId;
			ModIOManager.ModIODirectory = Path.Combine(ModioServices.Resolve<IModioRootPathProvider>().Path, "mod.io", gameId.ToString()) + Path.DirectorySeparatorChar.ToString();
			return;
		}
		if (ModIOManager.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06004527 RID: 17703 RVA: 0x00170F8F File Offset: 0x0016F18F
	private void Start()
	{
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
	}

	// Token: 0x06004528 RID: 17704 RVA: 0x00170FB4 File Offset: 0x0016F1B4
	private void OnDestroy()
	{
		if (ModIOManager.instance == this)
		{
			ModIOManager.instance = null;
			ModIOManager.hasInstance = false;
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(ModIOManager.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(ModIOManager.OnUGCDisabled));
			UGCPermissionManager.UnsubscribeFromVirtualStumpEnabled(new Action(ModIOManager.OnMapAccessEnabled));
		}
		NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
	}

	// Token: 0x06004529 RID: 17705 RVA: 0x00171032 File Offset: 0x0016F232
	private void Update()
	{
		bool flag = ModIOManager.hasInstance;
	}

	// Token: 0x0600452A RID: 17706 RVA: 0x00002C2D File Offset: 0x00000E2D
	private static void OnUGCEnabled()
	{
	}

	// Token: 0x0600452B RID: 17707 RVA: 0x00002C2D File Offset: 0x00000E2D
	private static void OnUGCDisabled()
	{
	}

	// Token: 0x0600452C RID: 17708 RVA: 0x0017103A File Offset: 0x0016F23A
	private static void OnMapAccessEnabled()
	{
		ModIOManager.PrefetchFeaturedMaps();
	}

	// Token: 0x0600452D RID: 17709 RVA: 0x00171044 File Offset: 0x0016F244
	private static async void PrefetchFeaturedMaps()
	{
		if (!ModIOManager.featuredMapsPrefetchStarted)
		{
			ModIOManager.featuredMapsPrefetchStarted = true;
			Error error = await ModIOManager.Initialize();
			if (error)
			{
				ModioLog error2 = ModioLog.Error;
				if (error2 != null)
				{
					error2.Log("[ModIOManager::PrefetchFeaturedMaps] Failed to initialize mod.io, skipping featured maps prefetch: " + error.GetMessage());
				}
				ModIOManager.featuredMapsPrefetchStarted = false;
			}
			else
			{
				ModSearchFilter modSearchFilter = new ModSearchFilter(0, 50);
				modSearchFilter.AddTag("Featured");
				ValueTuple<Error, ModioPage<Mod>> valueTuple = await ModIOManager.GetMods(modSearchFilter.GetModsFilter());
				Error item = valueTuple.Item1;
				ModioPage<Mod> modsPage = valueTuple.Item2;
				if (item || modsPage == null)
				{
					ModioLog error3 = ModioLog.Error;
					if (error3 != null)
					{
						error3.Log("[ModIOManager::PrefetchFeaturedMaps] Failed to retrieve featured maps: " + item.GetMessage());
					}
				}
				else
				{
					if (modsPage.TotalSearchResults > (long)modsPage.Data.Length)
					{
						ModioLog warning = ModioLog.Warning;
						if (warning != null)
						{
							warning.Log(string.Format("[ModIOManager::PrefetchFeaturedMaps] {0} featured maps ", modsPage.TotalSearchResults) + string.Format("found, only prefetching the first {0}.", modsPage.Data.Length));
						}
					}
					int downloadsQueued = 0;
					foreach (Mod featuredMod in modsPage.Data)
					{
						Mod mod = featuredMod;
						if (((mod != null) ? mod.File : null) != null && (featuredMod.File.State == ModFileState.None || featuredMod.File.State == ModFileState.Queued))
						{
							TaskAwaiter<bool> taskAwaiter = ModIOManager.DownloadMod(featuredMod.Id, null).GetAwaiter();
							if (!taskAwaiter.IsCompleted)
							{
								await taskAwaiter;
								TaskAwaiter<bool> taskAwaiter2;
								taskAwaiter = taskAwaiter2;
								taskAwaiter2 = default(TaskAwaiter<bool>);
							}
							if (taskAwaiter.GetResult())
							{
								downloadsQueued++;
								for (float waitedSeconds = 0f; waitedSeconds < 600f; waitedSeconds += 2f)
								{
									Modfile file = featuredMod.File;
									ModFileState modFileState = ((file != null) ? file.State : ModFileState.None);
									if (modFileState != ModFileState.None && modFileState != ModFileState.Queued && modFileState != ModFileState.Downloading && modFileState != ModFileState.Downloaded && modFileState != ModFileState.Installing)
									{
										break;
									}
									await Task.Delay(2000);
								}
								featuredMod = null;
							}
						}
					}
					Mod[] array = null;
					GTDev.Log<string>(string.Format("[ModIOManager::PrefetchFeaturedMaps] Queued {0} of {1} featured map downloads.", downloadsQueued, modsPage.Data.Length), null);
				}
			}
		}
	}

	// Token: 0x0600452E RID: 17710 RVA: 0x00171073 File Offset: 0x0016F273
	public static bool IsInitialized()
	{
		return ModIOManager.initialized;
	}

	// Token: 0x0600452F RID: 17711 RVA: 0x0017107C File Offset: 0x0016F27C
	public static bool IsFeaturedMap(Mod mod)
	{
		if (((mod != null) ? mod.Tags : null) == null)
		{
			return false;
		}
		foreach (ModTag modTag in mod.Tags)
		{
			if (modTag != null && string.Equals(modTag.ApiName, "Featured", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004530 RID: 17712 RVA: 0x001710CC File Offset: 0x0016F2CC
	[return: TupleElementNames(new string[] { "error", "featuredMaps" })]
	public static async Task<ValueTuple<Error, List<Mod>>> GetFeaturedMaps(bool forceRefresh = false)
	{
		ValueTuple<Error, List<Mod>> valueTuple;
		if (!forceRefresh && ModIOManager.featuredMapsRetrieved)
		{
			valueTuple = new ValueTuple<Error, List<Mod>>(Error.None, new List<Mod>(ModIOManager.retrievedFeaturedMaps));
		}
		else
		{
			Error error = await ModIOManager.Initialize();
			if (error)
			{
				valueTuple = new ValueTuple<Error, List<Mod>>(error, null);
			}
			else
			{
				ModSearchFilter modSearchFilter = new ModSearchFilter(0, 50);
				modSearchFilter.AddTag("Featured");
				ValueTuple<Error, ModioPage<Mod>> valueTuple2 = await ModIOManager.GetMods(modSearchFilter.GetModsFilter());
				Error item = valueTuple2.Item1;
				ModioPage<Mod> item2 = valueTuple2.Item2;
				if (item || item2 == null)
				{
					valueTuple = new ValueTuple<Error, List<Mod>>(item, null);
				}
				else
				{
					ModIOManager.retrievedFeaturedMaps.Clear();
					if (item2.Data != null)
					{
						ModIOManager.retrievedFeaturedMaps.AddRange(item2.Data);
					}
					ModIOManager.featuredMapsRetrieved = true;
					valueTuple = new ValueTuple<Error, List<Mod>>(Error.None, new List<Mod>(ModIOManager.retrievedFeaturedMaps));
				}
			}
		}
		return valueTuple;
	}

	// Token: 0x06004531 RID: 17713 RVA: 0x00171110 File Offset: 0x0016F310
	public static async Task<Error> Initialize()
	{
		Error error;
		if (UGCPermissionManager.HasNoMapAccess)
		{
			error = new Error(ErrorCode.UNKNOWN, "MOD.IO FUNCTIONALITY IS CURRENTLY DISABLED.");
		}
		else if (ModIOManager.initialized)
		{
			error = Error.None;
		}
		else
		{
			error = await ModIOManager.InitInternal();
		}
		return error;
	}

	// Token: 0x06004532 RID: 17714 RVA: 0x0017114C File Offset: 0x0016F34C
	private static async Task<Error> InitInternal()
	{
		Error error;
		if (UGCPermissionManager.HasNoMapAccess)
		{
			error = new Error(ErrorCode.UNKNOWN, "MOD.IO FUNCTIONALITY IS CURRENTLY DISABLED.");
		}
		else if (ModIOManager.initialized)
		{
			error = Error.None;
		}
		else
		{
			User.OnUserChanged -= ModIOManager.ModIOUserChanged;
			User.OnUserChanged += ModIOManager.ModIOUserChanged;
			User.OnUserSyncComplete -= ModIOManager.ModIOUserSyncComplete;
			User.OnUserSyncComplete += ModIOManager.ModIOUserSyncComplete;
			Error error2 = await ModioClient.Init();
			if (error2)
			{
				ModioLog error3 = ModioLog.Error;
				if (error3 != null)
				{
					error3.Log(string.Format("[ModIOManager::InitInternal] Error initializing mod.io: {0}", error2));
				}
				error = error2;
			}
			else
			{
				ModInstallationManagement.DownloadAndExtractAsSingleJob = false;
				ModIOManager.EnableModManagement();
				ModIOManager.initialized = true;
				await ModIOManager.GetFavoriteMods(false);
				error = Error.None;
			}
		}
		return error;
	}

	// Token: 0x06004533 RID: 17715 RVA: 0x00171188 File Offset: 0x0016F388
	private async Task<ValueTuple<Error, bool, bool>> HasAcceptedLatestTerms()
	{
		ValueTuple<Error, bool, bool> valueTuple;
		if (!ModIOManager.initialized)
		{
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log("[ModIOManager] HasAcceptedLatestTerms called before ModIO has been initialized!");
			}
			valueTuple = new ValueTuple<Error, bool, bool>(new Error(ErrorCode.NOT_INITIALIZED, "ModIOManager has not been initialized!"), false, false);
		}
		else
		{
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log("[ModIOManager::HasAcceptedLatestTerms] Retrieving terms of use from mod.io...");
			}
			ValueTuple<Error, Agreement> valueTuple2 = await Agreement.GetAgreement(AgreementType.TermsOfUse, false);
			Error item = valueTuple2.Item1;
			Agreement fullTermsOfUse = valueTuple2.Item2;
			if (item)
			{
				ModioLog error2 = ModioLog.Error;
				if (error2 != null)
				{
					error2.Log(string.Format("[ModIOManager::HasAcceptedLatestTerms] Failed to get Mod.io Terms of Use: {0} ", item));
				}
				valueTuple = new ValueTuple<Error, bool, bool>(item, false, false);
			}
			else
			{
				ValueTuple<Error, Agreement> valueTuple3 = await Agreement.GetAgreement(AgreementType.PrivacyPolicy, false);
				Error item2 = valueTuple3.Item1;
				Agreement item3 = valueTuple3.Item2;
				if (item2)
				{
					ModioLog error3 = ModioLog.Error;
					if (error3 != null)
					{
						error3.Log("[ModIOManager::HasAcceptedLatestTerms] Failed to get Mod.io Privacy Policy: " + string.Format("{0} ", item2));
					}
					valueTuple = new ValueTuple<Error, bool, bool>(item2, false, false);
				}
				else
				{
					ModioLog verbose2 = ModioLog.Verbose;
					if (verbose2 != null)
					{
						verbose2.Log("[ModIOManager::OnReceivedTermsOfUse] retrieved terms of use from mod.io, checking if already accepted...");
					}
					long num;
					long.TryParse(PlayerPrefs.GetString("modIOAcceptedTermsOfUseId"), out num);
					bool flag = fullTermsOfUse.Id == num;
					long num2;
					long.TryParse(PlayerPrefs.GetString("modIOAcceptedPrivacyPolicyId"), out num2);
					bool flag2 = item3.Id == num2;
					ModioLog verbose3 = ModioLog.Verbose;
					if (verbose3 != null)
					{
						verbose3.Log("[ModIOManager::OnReceivedTermsOfUse] Pre-Editor Skip: " + string.Format("Terms already accepted: {0} | ", flag) + string.Format("Privacy Policy already accepted: {0}", flag2));
					}
					ModioLog verbose4 = ModioLog.Verbose;
					if (verbose4 != null)
					{
						verbose4.Log("[ModIOManager::OnReceivedTermsOfUse] Post-Editor Skip: " + string.Format("Terms already accepted: {0} | ", flag) + string.Format("Privacy Policy already accepted: {0}", flag2));
					}
					valueTuple = new ValueTuple<Error, bool, bool>(Error.None, flag, flag2);
				}
			}
		}
		return valueTuple;
	}

	// Token: 0x06004534 RID: 17716 RVA: 0x001711C4 File Offset: 0x0016F3C4
	public static async Task<Error> ShowTermsOfUseAtGameLoad()
	{
		Error error;
		if (!ModIOManager.hasInstance)
		{
			error = new Error(ErrorCode.NOT_INITIALIZED, "ModIOManager has no instance!");
		}
		else if (UGCPermissionManager.HasNoMapAccess)
		{
			error = Error.None;
		}
		else
		{
			Error error2 = await ModIOManager.Initialize();
			if (error2)
			{
				error = error2;
			}
			else
			{
				ValueTuple<Error, bool, bool> valueTuple = await ModIOManager.instance.HasAcceptedLatestTerms();
				Error item = valueTuple.Item1;
				bool item2 = valueTuple.Item2;
				bool item3 = valueTuple.Item3;
				if (item)
				{
					error = item;
				}
				else if (item2 && item3)
				{
					error = Error.None;
				}
				else
				{
					error2 = await ModIOManager.instance.ShowModIOTermsOfUse();
					if (error2)
					{
						error = error2;
					}
					else
					{
						await ModIOManager.SaveAcceptedTermsIds();
						error = Error.None;
					}
				}
			}
		}
		return error;
	}

	// Token: 0x06004535 RID: 17717 RVA: 0x00171200 File Offset: 0x0016F400
	private static async Task SaveAcceptedTermsIds()
	{
		ValueTuple<Error, Agreement> valueTuple = await Agreement.GetAgreement(AgreementType.TermsOfUse, false);
		Error item = valueTuple.Item1;
		Agreement item2 = valueTuple.Item2;
		if (!item)
		{
			PlayerPrefs.SetString("modIOAcceptedTermsOfUseId", item2.Id.ToString());
		}
		ValueTuple<Error, Agreement> valueTuple2 = await Agreement.GetAgreement(AgreementType.PrivacyPolicy, false);
		Error item3 = valueTuple2.Item1;
		Agreement item4 = valueTuple2.Item2;
		if (!item3)
		{
			PlayerPrefs.SetString("modIOAcceptedPrivacyPolicyId", item4.Id.ToString());
		}
	}

	// Token: 0x06004536 RID: 17718 RVA: 0x0017123C File Offset: 0x0016F43C
	private async Task<Error> ShowModIOTermsOfUse()
	{
		Error error;
		if (!ModIOManager.initialized)
		{
			error = new Error(ErrorCode.NOT_INITIALIZED, "ModIOManager has not been initialized!");
		}
		else if (this.modIOTermsOfUsePrefab != null)
		{
			GameObject termsOfUseObject = Object.Instantiate<GameObject>(this.modIOTermsOfUsePrefab, base.transform);
			if (termsOfUseObject != null)
			{
				ModIOTermsOfUse_v2 component = termsOfUseObject.GetComponent<ModIOTermsOfUse_v2>();
				if (component != null)
				{
					CustomMapManager.DisableTeleportHUD();
					termsOfUseObject.SetActive(true);
					Error error2 = await component.ShowTerms();
					if (error2)
					{
						Object.Destroy(termsOfUseObject);
					}
					error = error2;
				}
				else
				{
					ModioLog error3 = ModioLog.Error;
					if (error3 != null)
					{
						error3.Log("[ModIOManager::ShowModIOTermsOfUse] TermsOfUsePrefab doesn't contain a ModIOTermsOfUse component!");
					}
					error = new Error(ErrorCode.NOT_INITIALIZED, "ModIOManager property 'ModIOTermsOfUsePrefab' object is missing the 'ModIOTermsOfUse_v2' script component.");
				}
			}
			else
			{
				ModioLog error4 = ModioLog.Error;
				if (error4 != null)
				{
					error4.Log("[ModIOManager::ShowModIOTermsOfUse] Failed to create termsOfUseObject!");
				}
				error = new Error(ErrorCode.UNKNOWN, "ModIOManager failed to instantiate the 'ModIOTermsOfUsePrefab'.");
			}
		}
		else
		{
			ModioLog error5 = ModioLog.Error;
			if (error5 != null)
			{
				error5.Log("[ModIOManager::ShowModIOTermsOfUse] ModIOTermsOfUsePrefab is not set!");
			}
			error = new Error(ErrorCode.UNKNOWN, "ModIOManager property 'ModIOTermsOfUsePrefab' is NULL!");
		}
		return error;
	}

	// Token: 0x06004537 RID: 17719 RVA: 0x00171280 File Offset: 0x0016F480
	private void OnModIOTermsOfUseAcknowledged(bool accepted)
	{
		if (accepted)
		{
			CustomMapManager.RequestEnableTeleportHUD(true);
			Action<ModIORequestResultAnd<bool>> action = ModIOManager.modIOTermsAcknowledgedCallback;
			if (action != null)
			{
				action(ModIORequestResultAnd<bool>.CreateSuccessResult(true));
			}
		}
		else
		{
			Action<ModIORequestResultAnd<bool>> action2 = ModIOManager.modIOTermsAcknowledgedCallback;
			if (action2 != null)
			{
				action2(ModIORequestResultAnd<bool>.CreateFailureResult("MOD.IO TERMS OF USE HAVE NOT BEEN ACCEPTED. YOU MUST ACCEPT THE MOD.IO TERMS OF USE TO LOGIN WITH YOUR PLATFORM CREDENTIALS OR YOU CAN LOGIN WITH AN EXISTING MOD.IO ACCOUNT BY PRESSING THE 'LINK MOD.IO ACCOUNT' BUTTON AND FOLLOWING THE INSTRUCTIONS."));
			}
		}
		ModIOManager.modIOTermsAcknowledgedCallback = null;
	}

	// Token: 0x06004538 RID: 17720 RVA: 0x001712CE File Offset: 0x0016F4CE
	private static void EnableModManagement()
	{
		if (!ModIOManager.modManagementEnabled)
		{
			ModInstallationManagement.ManagementEvents += ModIOManager.HandleModManagementEvent;
			ModInstallationManagement.Activate();
			ModIOManager.modManagementEnabled = true;
			ModioLog verbose = ModioLog.Verbose;
			if (verbose == null)
			{
				return;
			}
			verbose.Log("[ModIOManager::EnableModManagement] Mod Management enabled.");
		}
	}

	// Token: 0x06004539 RID: 17721 RVA: 0x00171307 File Offset: 0x0016F507
	private static void DisableModManagement()
	{
		if (ModIOManager.modManagementEnabled)
		{
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log("[ModIOManager::EnableModManagement] Mod Management disabled!");
			}
			ModInstallationManagement.ManagementEvents -= ModIOManager.HandleModManagementEvent;
			ModInstallationManagement.Deactivate(false);
			ModIOManager.modManagementEnabled = false;
		}
	}

	// Token: 0x0600453A RID: 17722 RVA: 0x00171344 File Offset: 0x0016F544
	private static void HandleModManagementEvent(Mod mod, Modfile modfile, ModInstallationManagement.OperationType jobType, ModInstallationManagement.OperationPhase jobPhase)
	{
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::HandleModManagementEvent] Mod " + mod.Id.ToString() + " | FileState: " + string.Format("{0} | JobType: {1} | JobPhase: {2}", modfile.State.ToString(), jobType, jobPhase));
		}
		try
		{
			if ((jobType == ModInstallationManagement.OperationType.Install || jobType == ModInstallationManagement.OperationType.Download) && jobPhase == ModInstallationManagement.OperationPhase.Completed && modfile.State == ModFileState.Installed)
			{
				ModIOManager.outdatedModCMSVersions.Remove(mod.Id);
				ModIOManager.IsModOutdated(mod);
			}
			if (jobPhase == ModInstallationManagement.OperationPhase.Started && (jobType == ModInstallationManagement.OperationType.Download || jobType == ModInstallationManagement.OperationType.Update || jobType == ModInstallationManagement.OperationType.Uninstall))
			{
				ModIOManager.outdatedModCMSVersions.Remove(mod.Id);
			}
		}
		catch (Exception ex)
		{
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log(string.Format("[ModIOManager::HandleModManagementEvent] Exception: {0}", ex));
			}
		}
		UnityEvent<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase> onModManagementEvent = ModIOManager.OnModManagementEvent;
		if (onModManagementEvent == null)
		{
			return;
		}
		onModManagementEvent.Invoke(mod, modfile, jobType, jobPhase);
	}

	// Token: 0x0600453B RID: 17723 RVA: 0x00171440 File Offset: 0x0016F640
	public static async Task RefreshModCache()
	{
		if (ModIOManager.refreshingModCache)
		{
			ModIOManager.restartRefreshModCache = true;
		}
		else
		{
			ModIOManager.refreshingModCache = true;
			UnityEvent onModIOCacheRefreshing = ModIOManager.OnModIOCacheRefreshing;
			if (onModIOCacheRefreshing != null)
			{
				onModIOCacheRefreshing.Invoke();
			}
			ModIOManager.restartRefreshModCache = true;
			while (ModIOManager.restartRefreshModCache)
			{
				ModIOManager.restartRefreshModCache = false;
				await Mod.RefreshPotentiallyHiddenCachedMods();
			}
			ModIOManager.refreshingModCache = false;
			UnityEvent onModIOCacheRefreshed = ModIOManager.OnModIOCacheRefreshed;
			if (onModIOCacheRefreshed != null)
			{
				onModIOCacheRefreshed.Invoke();
			}
		}
	}

	// Token: 0x0600453C RID: 17724 RVA: 0x0017147B File Offset: 0x0016F67B
	public static bool IsRefreshing()
	{
		return ModIOManager.refreshingModCache;
	}

	// Token: 0x0600453D RID: 17725 RVA: 0x00171484 File Offset: 0x0016F684
	public static async Task<ValueTuple<bool, int>> IsModOutdated(ModId modId)
	{
		ValueTuple<bool, int> valueTuple;
		int num;
		if (!ModIOManager.hasInstance)
		{
			valueTuple = new ValueTuple<bool, int>(false, -1);
		}
		else if (ModIOManager.outdatedModCMSVersions.TryGetValue(modId, out num))
		{
			valueTuple = new ValueTuple<bool, int>(true, num);
		}
		else
		{
			ValueTuple<Error, Mod> valueTuple2 = await ModIOManager.GetMod(modId, false, null);
			Error item = valueTuple2.Item1;
			Mod item2 = valueTuple2.Item2;
			if (item)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(string.Format("[ModIOManager::IsModOutdated] Failed to retrieve mod: {0}", item));
				}
				valueTuple = new ValueTuple<bool, int>(false, -1);
			}
			else
			{
				valueTuple = ModIOManager.IsModOutdated(item2);
			}
		}
		return valueTuple;
	}

	// Token: 0x0600453E RID: 17726 RVA: 0x001714C8 File Offset: 0x0016F6C8
	public static ValueTuple<bool, int> IsModOutdated(Mod mod)
	{
		int num;
		if (ModIOManager.outdatedModCMSVersions.TryGetValue(mod.Id, out num))
		{
			return new ValueTuple<bool, int>(true, num);
		}
		if (mod.File != null)
		{
			if (mod.File.State == ModFileState.Installed)
			{
				ValueTuple<bool, int> valueTuple = ModIOManager.IsInstalledModOutdated(mod);
				bool item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				return new ValueTuple<bool, int>(item, item2);
			}
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log("[ModIOManager::IsModOutdated] Mod File for " + mod.Name + " is not installed. " + string.Format("State: {0}.", mod.File.State));
			}
		}
		else
		{
			ModioLog error2 = ModioLog.Error;
			if (error2 != null)
			{
				error2.Log("[ModIOManager::IsModOutdated] Mod File for " + mod.Name + " is null.");
			}
		}
		return new ValueTuple<bool, int>(false, -1);
	}

	// Token: 0x0600453F RID: 17727 RVA: 0x00171590 File Offset: 0x0016F790
	public static void SaveFavoriteMods()
	{
		if (!ModIOManager.initialized || !ModIOManager.modManagementEnabled)
		{
			return;
		}
		try
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(ModIOManager.ModIODirectory);
			if (!directoryInfo.Exists)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log("[ModIOManager::SaveFavoriteMods] ModIO Directory for GorillaTag does not exist!");
				}
			}
			else
			{
				long[] array = new long[ModIOManager.favoriteMods.Count];
				int num = 0;
				foreach (KeyValuePair<ModId, Mod> keyValuePair in ModIOManager.favoriteMods)
				{
					array[num++] = keyValuePair.Key;
				}
				string text = JsonConvert.SerializeObject(array);
				File.WriteAllText(Path.Join(directoryInfo.FullName, "favoriteMods.json"), text);
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06004540 RID: 17728 RVA: 0x00171678 File Offset: 0x0016F878
	[return: TupleElementNames(new string[] { "error", "favoriteMods" })]
	public static async Task<ValueTuple<Error, List<Mod>>> GetFavoriteMods(bool forceRefresh = false)
	{
		ValueTuple<Error, List<Mod>> valueTuple;
		if (!ModIOManager.initialized || !ModIOManager.modManagementEnabled)
		{
			valueTuple = new ValueTuple<Error, List<Mod>>(new Error(ErrorCode.NOT_INITIALIZED), null);
		}
		else
		{
			if (forceRefresh)
			{
				ModIOManager.favoriteModsLoaded = false;
				ModIOManager.favoriteMods.Clear();
			}
			if (ModIOManager.favoriteModsLoaded)
			{
				valueTuple = new ValueTuple<Error, List<Mod>>(Error.None, ModIOManager.favoriteMods.Values.ToList<Mod>());
			}
			else
			{
				while (ModInstallationManagement.IsRunning)
				{
					await Task.Yield();
				}
				try
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(ModIOManager.ModIODirectory);
					if (!directoryInfo.Exists)
					{
						GTDev.LogWarning<string>("ModIOManager::GetFavoriteMods Directory " + directoryInfo.ToString() + " does not exist", null);
						ModIOManager.favoriteModsLoaded = true;
						valueTuple = new ValueTuple<Error, List<Mod>>(new Error(ErrorCode.FILE_NOT_FOUND), ModIOManager.favoriteMods.Values.ToList<Mod>());
					}
					else
					{
						FileInfo[] files = directoryInfo.GetFiles("favoriteMods.json");
						if (files.Length == 0)
						{
							GTDev.LogWarning<string>("ModIOManager::GetFavoriteMods could not find file " + ModIOManager.ModIODirectory + "favoriteMods.json", null);
							ModIOManager.favoriteModsLoaded = true;
							valueTuple = new ValueTuple<Error, List<Mod>>(new Error(ErrorCode.FILE_NOT_FOUND), ModIOManager.favoriteMods.Values.ToList<Mod>());
						}
						else
						{
							ValueTuple<Error, ICollection<Mod>> valueTuple2 = await ModIOManager.GetMods(JsonConvert.DeserializeObject<long[]>(File.ReadAllText(files[0].FullName)), forceRefresh, null);
							Error item = valueTuple2.Item1;
							ICollection<Mod> item2 = valueTuple2.Item2;
							if (!item)
							{
								foreach (Mod mod in item2)
								{
									ModIOManager.favoriteMods[mod.Id] = mod;
								}
							}
							ModIOManager.favoriteModsLoaded = true;
							valueTuple = new ValueTuple<Error, List<Mod>>(item, ModIOManager.favoriteMods.Values.ToList<Mod>());
						}
					}
				}
				catch (Exception ex)
				{
					GTDev.LogError<Exception>(ex, null);
					ModIOManager.favoriteModsLoaded = true;
					valueTuple = new ValueTuple<Error, List<Mod>>(new Error(ErrorCode.READ_ERROR), ModIOManager.favoriteMods.Values.ToList<Mod>());
				}
			}
		}
		return valueTuple;
	}

	// Token: 0x06004541 RID: 17729 RVA: 0x001716BC File Offset: 0x0016F8BC
	public static async Task<Error> AddFavorite(ModId modId, Action<Error> callback = null)
	{
		Error error;
		if (ModIOManager.favoriteMods.ContainsKey(modId))
		{
			error = new Error(ErrorCode.UNKNOWN, "MOD ALREADY FAVORITED");
		}
		else
		{
			ValueTuple<Error, Mod> valueTuple = await ModIOManager.GetMod(modId, false, null);
			Error item = valueTuple.Item1;
			Mod item2 = valueTuple.Item2;
			if (!item)
			{
				ModIOManager.favoriteMods.Add(modId, item2);
				ModIOManager.SaveFavoriteMods();
			}
			if (callback != null)
			{
				callback(item);
			}
			error = item;
		}
		return error;
	}

	// Token: 0x06004542 RID: 17730 RVA: 0x00171707 File Offset: 0x0016F907
	public static Error RemoveFavorite(ModId modId)
	{
		if (!ModIOManager.favoriteMods.ContainsKey(modId))
		{
			return new Error(ErrorCode.UNKNOWN, "MOD NOT FAVORITED");
		}
		ModIOManager.favoriteMods.Remove(modId);
		ModIOManager.SaveFavoriteMods();
		return Error.None;
	}

	// Token: 0x06004543 RID: 17731 RVA: 0x0017173D File Offset: 0x0016F93D
	public static bool IsModFavorited(ModId modId)
	{
		return ModIOManager.favoriteMods.ContainsKey(modId);
	}

	// Token: 0x06004544 RID: 17732 RVA: 0x0017174C File Offset: 0x0016F94C
	[return: TupleElementNames(new string[] { "error", "installedMods" })]
	public static async Task<ValueTuple<Error, Mod[]>> GetInstalledMods(bool forceRefresh = false)
	{
		ValueTuple<Error, Mod[]> valueTuple;
		if (!ModIOManager.initialized || !ModIOManager.modManagementEnabled)
		{
			valueTuple = new ValueTuple<Error, Mod[]>(new Error(ErrorCode.NOT_INITIALIZED), null);
		}
		else
		{
			while (ModInstallationManagement.IsRunning)
			{
				await Task.Yield();
			}
			IEnumerable<Mod> enumerable = await ModInstallationManagement.GetAllInstalledMods(forceRefresh);
			List<Mod> list = new List<Mod>();
			foreach (Mod mod in enumerable)
			{
				if (mod.File.State == ModFileState.Installed)
				{
					list.AddIfNew(mod);
				}
				else if (mod.File.State == ModFileState.Queued && !mod.File.InstallLocation.IsNullOrEmpty())
				{
					list.AddIfNew(mod);
				}
			}
			valueTuple = new ValueTuple<Error, Mod[]>(Error.None, list.ToArray());
		}
		return valueTuple;
	}

	// Token: 0x06004545 RID: 17733 RVA: 0x0017178F File Offset: 0x0016F98F
	public static bool ValidateInstalledMod(Mod mod)
	{
		return ModIOManager.initialized && ModInstallationManagement.ValidateInstalledMod(mod);
	}

	// Token: 0x06004546 RID: 17734 RVA: 0x001717A0 File Offset: 0x0016F9A0
	private static ValueTuple<bool, int> IsInstalledModOutdated(Mod mod)
	{
		int num = -1;
		if (!ModIOManager.hasInstance)
		{
			return new ValueTuple<bool, int>(false, num);
		}
		if (mod.File == null || mod.File.State != ModFileState.Installed)
		{
			ModioLog message = ModioLog.Message;
			if (message != null)
			{
				message.Log("[ModIOManager::IsInstalledModOutdated] Mod " + mod.Id.ToString() + " is not currently installed.");
			}
			return new ValueTuple<bool, int>(false, num);
		}
		try
		{
			FileInfo[] files = new DirectoryInfo(mod.File.InstallLocation).GetFiles("package.json");
			if (files.Length == 0)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(string.Concat(new string[]
					{
						"[ModIOManager::IsInstalledModOutdated] Directory (",
						mod.File.InstallLocation,
						") for mod ",
						mod.Name,
						" does not contain a package.json file!"
					}));
				}
			}
			if (files.Length > 1)
			{
				ModioLog warning = ModioLog.Warning;
				if (warning != null)
				{
					warning.Log(string.Concat(new string[]
					{
						"[ModIOManager::IsInstalledModOutdated] Directory (",
						mod.File.InstallLocation,
						") for mod ",
						mod.Name,
						" contains more than one package.json file! Only the first one found will be used!"
					}));
				}
			}
			MapPackageInfo packageInfo = CustomMapLoader.GetPackageInfo(files[0].FullName);
			if (packageInfo.customMapSupportVersion != global::GT_CustomMapSupportRuntime.Constants.customMapSupportVersion)
			{
				ModIOManager.outdatedModCMSVersions.Add(mod.Id, packageInfo.customMapSupportVersion);
				return new ValueTuple<bool, int>(true, packageInfo.customMapSupportVersion);
			}
		}
		catch (Exception ex)
		{
			ModioLog error2 = ModioLog.Error;
			if (error2 != null)
			{
				error2.Log(string.Format("[ModIOManager::IsInstalledModOutdated] Exception while reading package.json: {0}", ex));
			}
			ModInstallationManagement.RefreshMod(mod);
			return new ValueTuple<bool, int>(false, num);
		}
		return new ValueTuple<bool, int>(false, num);
	}

	// Token: 0x06004547 RID: 17735 RVA: 0x00171958 File Offset: 0x0016FB58
	public static async Task RefreshUserProfile(Action<bool> callback = null, bool force = false)
	{
		if (!ModIOManager.hasInstance || !ModIOManager.IsLoggedIn())
		{
			if (callback != null)
			{
				callback(false);
			}
		}
		else if (ModIOManager.refreshing && callback != null)
		{
			ModIOManager.currentRefreshCallbacks.Add(callback);
		}
		else if (force || Mathf.Approximately(0f, ModIOManager.lastRefreshTime) || Time.realtimeSinceStartup - ModIOManager.lastRefreshTime >= 5f)
		{
			ModIOManager.currentRefreshCallbacks.Add(callback);
			ModIOManager.lastRefreshTime = Time.realtimeSinceStartup;
			ModIOManager.refreshing = true;
			if (User.Current.IsUpdating)
			{
				ModioLog verbose = ModioLog.Verbose;
				if (verbose != null)
				{
					verbose.Log("[ModIOManager::Refresh] Profile already updating, waiting for Sync to finish...");
				}
				while (User.Current.IsUpdating)
				{
					await Task.Yield();
				}
			}
			else
			{
				ModioLog verbose2 = ModioLog.Verbose;
				if (verbose2 != null)
				{
					verbose2.Log("[ModIOManager::Refresh] Syncing user profile...");
				}
				await User.Current.Sync();
			}
			ModIOManager.refreshing = false;
			foreach (Action<bool> action in ModIOManager.currentRefreshCallbacks)
			{
				if (action != null)
				{
					action(true);
				}
			}
			ModIOManager.currentRefreshCallbacks.Clear();
		}
		else if (callback != null)
		{
			callback(false);
		}
	}

	// Token: 0x06004548 RID: 17736 RVA: 0x001719A4 File Offset: 0x0016FBA4
	[return: TupleElementNames(new string[] { "error", "mods" })]
	public static async Task<ValueTuple<Error, ICollection<Mod>>> GetMods(ICollection<long> modIds, bool forceRefresh = false, Action<Error, ICollection<Mod>> callback = null)
	{
		ValueTuple<Error, ICollection<Mod>> valueTuple;
		if (!ModIOManager.hasInstance)
		{
			Error error = new Error(ErrorCode.NOT_INITIALIZED);
			if (callback != null)
			{
				callback(error, null);
			}
			valueTuple = new ValueTuple<Error, ICollection<Mod>>(error, null);
		}
		else
		{
			ValueTuple<Error, ICollection<Mod>> valueTuple2 = await Mod.GetMods(modIds, forceRefresh, null);
			Error error = valueTuple2.Item1;
			ICollection<Mod> item = valueTuple2.Item2;
			if (error)
			{
				ModioLog error2 = ModioLog.Error;
				if (error2 != null)
				{
					error2.Log("[ModIOManager::GetMod] Failed to get requested Mods. Error: " + error.GetMessage());
				}
				if (callback != null)
				{
					callback(error, null);
				}
				valueTuple = new ValueTuple<Error, ICollection<Mod>>(error, null);
			}
			else
			{
				if (callback != null)
				{
					callback(error, item);
				}
				valueTuple = new ValueTuple<Error, ICollection<Mod>>(Error.None, item);
			}
		}
		return valueTuple;
	}

	// Token: 0x06004549 RID: 17737 RVA: 0x001719F8 File Offset: 0x0016FBF8
	[return: TupleElementNames(new string[] { "error", "result" })]
	public static async Task<ValueTuple<Error, Mod>> GetMod(ModId modId, bool forceUpdate = false, Action<Error, Mod> callback = null)
	{
		ValueTuple<Error, Mod> valueTuple;
		if (!ModIOManager.hasInstance)
		{
			Error error = new Error(ErrorCode.NOT_INITIALIZED);
			if (callback != null)
			{
				callback(error, null);
			}
			valueTuple = new ValueTuple<Error, Mod>(error, null);
		}
		else
		{
			Mod retrievedMod = null;
			ValueTuple<Error, Mod> valueTuple2 = await Mod.GetMod(modId, forceUpdate, null, false);
			Error error = valueTuple2.Item1;
			retrievedMod = valueTuple2.Item2;
			if (error)
			{
				ModioLog error2 = ModioLog.Error;
				if (error2 != null)
				{
					error2.Log("[ModIOManager::GetMod] Failed to get Mod " + modId.ToString() + ". Error: " + error.GetMessage());
				}
				if (callback != null)
				{
					callback(error, retrievedMod);
				}
				valueTuple = new ValueTuple<Error, Mod>(error, retrievedMod);
			}
			else
			{
				if (forceUpdate)
				{
					if (ModIOManager.IsLoggedIn())
					{
						await ModIOManager.RefreshUserProfile(null, false);
					}
					else
					{
						ModInstallationManagement.RefreshMod(retrievedMod);
					}
				}
				if (callback != null)
				{
					callback(error, retrievedMod);
				}
				valueTuple = new ValueTuple<Error, Mod>(Error.None, retrievedMod);
			}
		}
		return valueTuple;
	}

	// Token: 0x0600454A RID: 17738 RVA: 0x00171A4C File Offset: 0x0016FC4C
	[return: TupleElementNames(new string[] { "error", "logo" })]
	public static async Task<ValueTuple<Error, Texture2D>> GetModLogo(Mod mod, Action<Error, Texture2D> callback)
	{
		ValueTuple<Error, Texture2D> valueTuple;
		if (mod == null || !mod.Id.IsValid())
		{
			valueTuple = new ValueTuple<Error, Texture2D>(new Error(ErrorCode.BAD_PARAMETER), null);
		}
		else if (!ModIOManager.hasInstance)
		{
			Error error = new Error(ErrorCode.NOT_INITIALIZED);
			if (callback != null)
			{
				callback(error, null);
			}
			valueTuple = new ValueTuple<Error, Texture2D>(error, null);
		}
		else if (mod.Logo == null)
		{
			Error error = new Error(ErrorCode.UNKNOWN, "Mod Logo is null!");
			if (callback != null)
			{
				callback(error, null);
			}
			valueTuple = new ValueTuple<Error, Texture2D>(error, null);
		}
		else
		{
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log("[ModIOManager::GetModLogo] Getting logo for Mod " + mod.Id.ToString() + "...");
			}
			ValueTuple<Error, Texture2D> valueTuple2 = await mod.Logo.DownloadAsTexture2D(Mod.LogoResolution.X320_Y180);
			Error error = valueTuple2.Item1;
			Texture2D item = valueTuple2.Item2;
			if (error)
			{
				ModioLog error2 = ModioLog.Error;
				if (error2 != null)
				{
					error2.Log("[ModIOManager::GetModLogo] Failed to download logo for Mod " + mod.Id.ToString() + ". Error: " + error.GetMessage());
				}
			}
			if (callback != null)
			{
				callback(error, item);
			}
			valueTuple = new ValueTuple<Error, Texture2D>(error, item);
		}
		return valueTuple;
	}

	// Token: 0x0600454B RID: 17739 RVA: 0x00171A98 File Offset: 0x0016FC98
	[return: TupleElementNames(new string[] { "error", "modsPage" })]
	public static async Task<ValueTuple<Error, ModioPage<Mod>>> GetMods(ModioAPI.Mods.GetModsFilter searchFilter)
	{
		ValueTuple<Error, ModioPage<Mod>> valueTuple;
		if (!ModIOManager.hasInstance)
		{
			Error error = new Error(ErrorCode.NOT_INITIALIZED);
			valueTuple = new ValueTuple<Error, ModioPage<Mod>>(error, null);
		}
		else
		{
			valueTuple = await Mod.GetMods(searchFilter, false);
		}
		return valueTuple;
	}

	// Token: 0x0600454C RID: 17740 RVA: 0x00171ADC File Offset: 0x0016FCDC
	private static void ModIOUserChanged(User currentUser)
	{
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::ModIOUserChanged] CurrentUser: " + ((currentUser == null) ? "NULL" : currentUser.Profile.Username));
		}
		UnityEvent<User> onModIOUserChanged = ModIOManager.OnModIOUserChanged;
		if (onModIOUserChanged == null)
		{
			return;
		}
		onModIOUserChanged.Invoke(currentUser);
	}

	// Token: 0x0600454D RID: 17741 RVA: 0x00171B28 File Offset: 0x0016FD28
	private static void ModIOUserSyncComplete()
	{
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::ModIOUserSyncComplete] Refreshing mod cache...");
		}
		ModIOManager.RefreshModCache();
	}

	// Token: 0x0600454E RID: 17742 RVA: 0x00171B45 File Offset: 0x0016FD45
	public static bool IsLoggedIn()
	{
		return User.Current != null && User.Current.IsAuthenticated;
	}

	// Token: 0x0600454F RID: 17743 RVA: 0x00171B5A File Offset: 0x0016FD5A
	public static bool IsLoggingIn()
	{
		return ModIOManager.loggingIn;
	}

	// Token: 0x06004550 RID: 17744 RVA: 0x00171B61 File Offset: 0x0016FD61
	public static bool IsLoggingOut()
	{
		return ModIOManager.loggingOut;
	}

	// Token: 0x06004551 RID: 17745 RVA: 0x00171B68 File Offset: 0x0016FD68
	public static string GetCurrentUsername()
	{
		if (!ModIOManager.IsLoggedIn())
		{
			return "";
		}
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::GetCurrentUsername] Username: " + User.Current.Profile.Username);
		}
		return User.Current.Profile.Username;
	}

	// Token: 0x06004552 RID: 17746 RVA: 0x00171BBC File Offset: 0x0016FDBC
	public static string GetCurrentUserId()
	{
		if (!ModIOManager.IsLoggedIn())
		{
			return "";
		}
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log(string.Format("[ModIOManager::GetCurrentUserId] User ID: {0}", User.Current.Profile.UserId));
		}
		return User.Current.Profile.UserId.ToString();
	}

	// Token: 0x06004553 RID: 17747 RVA: 0x00171C1B File Offset: 0x0016FE1B
	public static string GetCurrentAuthToken()
	{
		if (!ModIOManager.IsLoggedIn())
		{
			return "";
		}
		return User.Current.Token;
	}

	// Token: 0x06004554 RID: 17748 RVA: 0x00171C34 File Offset: 0x0016FE34
	public static bool IsAuthenticated(bool sendEvents = false)
	{
		if (!ModIOManager.hasInstance)
		{
			return false;
		}
		bool isAuthenticated = User.Current.IsAuthenticated;
		if (isAuthenticated)
		{
			ModIOManager.loggingIn = false;
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log("[ModIOManager::IsAuthenticated] User already authenticated...");
			}
			if (sendEvents)
			{
				UnityEvent onModIOLoggedIn = ModIOManager.OnModIOLoggedIn;
				if (onModIOLoggedIn != null)
				{
					onModIOLoggedIn.Invoke();
				}
			}
		}
		else
		{
			try
			{
				ModioLog verbose2 = ModioLog.Verbose;
				if (verbose2 != null)
				{
					verbose2.Log("[ModIOManager::IsAuthenticated] User not authenticated");
				}
				if (sendEvents)
				{
					UnityEvent onModIOLoggedOut = ModIOManager.OnModIOLoggedOut;
					if (onModIOLoggedOut != null)
					{
						onModIOLoggedOut.Invoke();
					}
				}
			}
			catch (Exception ex)
			{
				ModioLog verbose3 = ModioLog.Verbose;
				if (verbose3 != null)
				{
					verbose3.Log(string.Format("[ModIOManager::IsAuthenticated] error {0}", ex));
				}
			}
		}
		ModioLog verbose4 = ModioLog.Verbose;
		if (verbose4 != null)
		{
			verbose4.Log(string.Format("[ModIOManager::IsAuthenticated] returning {0}", isAuthenticated));
		}
		return isAuthenticated;
	}

	// Token: 0x06004555 RID: 17749 RVA: 0x00171D04 File Offset: 0x0016FF04
	public static void LogoutFromModIO()
	{
		if (!ModIOManager.hasInstance || ModIOManager.loggingIn || !ModIOManager.IsLoggedIn())
		{
			return;
		}
		ModIOManager.loggingOut = true;
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::LogoutFromModIO] Logging out of mod.io...");
		}
		ModIOManager.CancelExternalAuthentication();
		ModIOManager.loggingIn = false;
		User.DeleteUserData();
		ModioLog verbose2 = ModioLog.Verbose;
		if (verbose2 != null)
		{
			verbose2.Log("[ModIOManager::LogoutFromModIO] User data deleted...");
		}
		PlayerPrefs.SetInt("modIOLassSuccessfulAuthMethod", ModIOManager.ModIOAuthMethod.Invalid.GetIndex<ModIOManager.ModIOAuthMethod>());
		ModioLog verbose3 = ModioLog.Verbose;
		if (verbose3 != null)
		{
			verbose3.Log("[ModIOManager::LogoutFromModIO] User fully logged out.");
		}
		ModIOManager.loggingOut = false;
		UnityEvent onModIOLoggedOut = ModIOManager.OnModIOLoggedOut;
		if (onModIOLoggedOut != null)
		{
			onModIOLoggedOut.Invoke();
		}
		ModIOManager.RefreshModCache();
	}

	// Token: 0x06004556 RID: 17750 RVA: 0x00171DA8 File Offset: 0x0016FFA8
	public static void SetAccountLinkPrompter(IWssAuthPrompter prompter)
	{
		if (ModIOManager.accountLinkingAuthService != null)
		{
			ModIOManager.accountLinkingAuthService.SetPrompter(prompter);
		}
	}

	// Token: 0x06004557 RID: 17751 RVA: 0x00171DBC File Offset: 0x0016FFBC
	public static async Task<Error> RequestAccountLinkCode()
	{
		Error error;
		if (!ModIOManager.hasInstance)
		{
			error = new Error(ErrorCode.NOT_INITIALIZED);
		}
		else if (ModIOManager.loggingIn)
		{
			error = new Error(ErrorCode.USER_AUTHENTICATION_IN_PROGRESS);
		}
		else if (ModIOManager.IsLoggedIn())
		{
			error = new Error(ErrorCode.ALREADY_AUTHENTICATED);
		}
		else
		{
			ModIOManager.loggingIn = true;
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log("[ModIOManager::RequestAccountLinkCode] Requesting Link Code...");
			}
			Error error2 = await ModIOManager.accountLinkingAuthService.Authenticate(false, null);
			if (!error2)
			{
				ModioLog verbose2 = ModioLog.Verbose;
				if (verbose2 != null)
				{
					verbose2.Log("[ModIOManager::RequestAccountLinkCode] Account linked successfully!");
				}
				PlayerPrefs.SetInt("modIOLassSuccessfulAuthMethod", ModIOManager.ModIOAuthMethod.LinkedAccount.GetIndex<ModIOManager.ModIOAuthMethod>());
			}
			ModIOManager.OnAuthenticationComplete(error2);
			error = error2;
		}
		return error;
	}

	// Token: 0x06004558 RID: 17752 RVA: 0x00171DF7 File Offset: 0x0016FFF7
	public static void CancelExternalAuthentication()
	{
		if (!ModIOManager.hasInstance)
		{
			return;
		}
		if (ModIOManager.accountLinkingAuthService != null && ModIOManager.accountLinkingAuthService.InProgress())
		{
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log("[ModIOManager::CancelExternalAuthentication] Cancelling Mod.io Account Linking process...");
			}
			ModIOManager.accountLinkingAuthService.Cancel();
		}
	}

	// Token: 0x06004559 RID: 17753 RVA: 0x00171E34 File Offset: 0x00170034
	public static async Task<Error> RequestPlatformLogin()
	{
		Error error3;
		if (!ModIOManager.hasInstance)
		{
			ModioLog error2 = ModioLog.Error;
			if (error2 != null)
			{
				error2.Log("[ModIOManager::RequestPlatformLogin] has no instance");
			}
			error3 = new Error(ErrorCode.NOT_INITIALIZED, "ModIOManager has not been initialized!");
		}
		else if (ModIOManager.loggingIn)
		{
			ModioLog message = ModioLog.Message;
			if (message != null)
			{
				message.Log("[ModIOManager::RequestPlatformLogin] is already logging in");
			}
			error3 = new Error(ErrorCode.USER_AUTHENTICATION_IN_PROGRESS);
		}
		else
		{
			ModIOManager.loggingIn = true;
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log("[ModIOManager::RequestPlatformLogin] calling IsAuthenticated");
			}
			if (ModIOManager.IsAuthenticated(true))
			{
				ModioLog verbose2 = ModioLog.Verbose;
				if (verbose2 != null)
				{
					verbose2.Log("[ModIOManager::RequestPlatformLogin] User already authenticated!");
				}
				error3 = Error.None;
			}
			else
			{
				ModioLog verbose3 = ModioLog.Verbose;
				if (verbose3 != null)
				{
					verbose3.Log("[ModIOManager::RequestPlatformLogin] calling InitializePlatformLogin");
				}
				Error error = new Error(ErrorCode.NONE);
				try
				{
					Error error4 = await ModIOManager.instance.InitiatePlatformLogin();
					error = error4;
				}
				catch (Exception ex)
				{
					ModioLog error5 = ModioLog.Error;
					if (error5 != null)
					{
						error5.Log(string.Format("[ModIOManager::RequestPlatformLogin] exception initializing platform login {0}", ex));
					}
				}
				error3 = error;
			}
		}
		return error3;
	}

	// Token: 0x0600455A RID: 17754 RVA: 0x00171E70 File Offset: 0x00170070
	private async Task<Error> InitiatePlatformLogin()
	{
		UnityEvent onModIOLoginStarted = ModIOManager.OnModIOLoginStarted;
		if (onModIOLoginStarted != null)
		{
			onModIOLoginStarted.Invoke();
		}
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::InitiatePlatformLogin] Attempting to login using platform credentials...");
		}
		ValueTuple<Error, bool, bool> valueTuple = await this.HasAcceptedLatestTerms();
		Error error = valueTuple.Item1;
		bool item = valueTuple.Item2;
		bool item2 = valueTuple.Item3;
		Error error2;
		if (error)
		{
			ModIOManager.loggingIn = false;
			UnityEvent<string> onModIOLoginFailed = ModIOManager.OnModIOLoginFailed;
			if (onModIOLoginFailed != null)
			{
				onModIOLoginFailed.Invoke(string.Format("FAILED TO LOGIN TO MOD.IO:\nFAILED TO CHECK TERMS OF USE ACCEPTANCE STATUS: {0}", error));
			}
			error2 = error;
		}
		else
		{
			if (!item || !item2)
			{
				error = await this.ShowModIOTermsOfUse();
				if (error)
				{
					ModIOManager.OnAuthenticationComplete(error);
					return error;
				}
				await ModIOManager.SaveAcceptedTermsIds();
			}
			error = await this.ContinuePlatformLogin();
			error2 = error;
		}
		return error2;
	}

	// Token: 0x0600455B RID: 17755 RVA: 0x00171EB4 File Offset: 0x001700B4
	private async Task<Error> ContinuePlatformLogin()
	{
		Error error3;
		if (SteamManager.Initialized)
		{
			ModIOManager.steamAuthService.SetCredentialProvider(this);
			Error error = await ModIOManager.steamAuthService.Authenticate(true, null);
			if (error)
			{
				ModioLog error2 = ModioLog.Error;
				if (error2 != null)
				{
					error2.Log(string.Format("[ModIOManager::ContinuePlatformLogin] Failed to authenticate via Steam: {0}", error));
				}
				ModIOManager.OnAuthenticationComplete(error);
				error3 = error;
			}
			else
			{
				ModioLog verbose = ModioLog.Verbose;
				if (verbose != null)
				{
					verbose.Log("[ModIOManager::ContinuePlatformLogin] Successfully authenticated via Steam!");
				}
				PlayerPrefs.SetInt("modIOLassSuccessfulAuthMethod", ModIOManager.ModIOAuthMethod.Steam.GetIndex<ModIOManager.ModIOAuthMethod>());
				ModIOManager.OnAuthenticationComplete(Error.None);
				error3 = Error.None;
			}
		}
		else
		{
			ModioLog error4 = ModioLog.Error;
			if (error4 != null)
			{
				error4.Log("[ModIOManager::ContinuePlatformLogin] Steam enabled but not initialized...");
			}
			ModIOManager.OnAuthenticationComplete(new Error(ErrorCode.NOT_INITIALIZED, "STEAM IS ENABLED BUT NOT INITIALIZED."));
			error3 = new Error(ErrorCode.NOT_INITIALIZED, "Steam is enabled, but has not been initialized.");
		}
		return error3;
	}

	// Token: 0x0600455C RID: 17756 RVA: 0x00171EF8 File Offset: 0x001700F8
	public void RequestEncryptedAppTicket(Action<bool, string> callback)
	{
		if (this.requestEncryptedAppTicketCallback != null)
		{
			ModioLog warning = ModioLog.Warning;
			if (warning != null)
			{
				warning.Log("[ModIOManager::RequestEncryptedAppTicket] Callback already set, Encrypted App Ticket request already in progress!");
			}
			if (callback != null)
			{
				callback(false, "AN ENCRYPTED APP TICKET REQUEST IS ALREADY IN PROGRESS");
			}
			return;
		}
		this.requestEncryptedAppTicketCallback = callback;
		if (ModIOManager.requestEncryptedAppTicketResponse == null)
		{
			ModIOManager.requestEncryptedAppTicketResponse = CallResult<EncryptedAppTicketResponse_t>.Create(new CallResult<EncryptedAppTicketResponse_t>.APIDispatchDelegate(this.OnRequestEncryptedAppTicketFinished));
		}
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::RequestEncryptedAppTicket] Requesting Steam Encrypted App Ticket...");
		}
		SteamAPICall_t steamAPICall_t = SteamUser.RequestEncryptedAppTicket(null, 0);
		ModIOManager.requestEncryptedAppTicketResponse.Set(steamAPICall_t, null);
	}

	// Token: 0x0600455D RID: 17757 RVA: 0x00171F80 File Offset: 0x00170180
	private void OnRequestEncryptedAppTicketFinished(EncryptedAppTicketResponse_t response, bool bIOFailure)
	{
		if (bIOFailure)
		{
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log("Failed to retrieve EncryptedAppTicket due to a Steam API IO failure...");
			}
			Action<bool, string> action = this.requestEncryptedAppTicketCallback;
			if (action != null)
			{
				action(false, "FAILED TO RETRIEVE 'EncryptedAppTicket' DUE TO A STEAM API IO FAILURE.");
			}
			this.requestEncryptedAppTicketCallback = null;
			return;
		}
		EResult eResult = response.m_eResult;
		if (eResult <= EResult.k_EResultNoConnection)
		{
			if (eResult != EResult.k_EResultOK)
			{
				if (eResult == EResult.k_EResultNoConnection)
				{
					ModioLog error2 = ModioLog.Error;
					if (error2 != null)
					{
						error2.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] Not connected to steam.");
					}
					Action<bool, string> action2 = this.requestEncryptedAppTicketCallback;
					if (action2 != null)
					{
						action2(false, "NOT CONNECTED TO STEAM.");
					}
					this.requestEncryptedAppTicketCallback = null;
					return;
				}
			}
			else
			{
				if (!SteamUser.GetEncryptedAppTicket(ModIOManager.ticketBlob, ModIOManager.ticketBlob.Length, out ModIOManager.ticketSize))
				{
					ModioLog error3 = ModioLog.Error;
					if (error3 != null)
					{
						error3.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] Failed to retrieve " + string.Format("EncryptedAppTicket! Needed size: {0}", ModIOManager.ticketSize));
					}
					Action<bool, string> action3 = this.requestEncryptedAppTicketCallback;
					if (action3 != null)
					{
						action3(false, "FAILED TO RETRIEVE 'EncryptedAppTicket'.");
					}
					this.requestEncryptedAppTicketCallback = null;
					return;
				}
				Array.Resize<byte>(ref ModIOManager.ticketBlob, (int)ModIOManager.ticketSize);
				string text = Convert.ToBase64String(ModIOManager.ticketBlob);
				ModioLog verbose = ModioLog.Verbose;
				if (verbose != null)
				{
					verbose.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] Successfully retrieved Steam Encrypted App Ticket: " + text);
				}
				Action<bool, string> action4 = this.requestEncryptedAppTicketCallback;
				if (action4 != null)
				{
					action4(true, text);
				}
				this.requestEncryptedAppTicketCallback = null;
				return;
			}
		}
		else
		{
			if (eResult == EResult.k_EResultLimitExceeded)
			{
				ModioLog error4 = ModioLog.Error;
				if (error4 != null)
				{
					error4.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] Rate Limit exceeded, this function should not be called more than once per minute.");
				}
				Action<bool, string> action5 = this.requestEncryptedAppTicketCallback;
				if (action5 != null)
				{
					action5(false, "RATE LIMIT EXCEEDED, CAN ONLY REQUEST ONE 'EncryptedAppTicket' PER MINUTE.");
				}
				this.requestEncryptedAppTicketCallback = null;
				return;
			}
			if (eResult == EResult.k_EResultDuplicateRequest)
			{
				ModioLog error5 = ModioLog.Error;
				if (error5 != null)
				{
					error5.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] There is already a pending EncryptedAppTicket request.");
				}
				Action<bool, string> action6 = this.requestEncryptedAppTicketCallback;
				if (action6 != null)
				{
					action6(false, "THERE IS ALREADY AN 'EncryptedAppTicket' REQUEST IN PROGRESS.");
				}
				this.requestEncryptedAppTicketCallback = null;
				return;
			}
		}
		ModioLog error6 = ModioLog.Error;
		if (error6 != null)
		{
			error6.Log(string.Format("[ModIOManager::OnRequestEncryptedAppTicketFinished] Unknown Error: {0}", response.m_eResult));
		}
		Action<bool, string> action7 = this.requestEncryptedAppTicketCallback;
		if (action7 != null)
		{
			action7(false, string.Format("{0}", response.m_eResult));
		}
		this.requestEncryptedAppTicketCallback = null;
	}

	// Token: 0x0600455E RID: 17758 RVA: 0x00172198 File Offset: 0x00170398
	public async Task<ValueTuple<Error, string>> GetOculusUserId()
	{
		return new ValueTuple<Error, string>(Error.Unknown, "OCULUS is not enabled for this build");
	}

	// Token: 0x0600455F RID: 17759 RVA: 0x001721D4 File Offset: 0x001703D4
	public async Task<string> GetOculusAccessToken()
	{
		return "";
	}

	// Token: 0x06004560 RID: 17760 RVA: 0x00172210 File Offset: 0x00170410
	public async Task<string> GetOculusUserProof()
	{
		return "";
	}

	// Token: 0x06004561 RID: 17761 RVA: 0x00092236 File Offset: 0x00090436
	public string GetOculusDevice()
	{
		return "";
	}

	// Token: 0x06004562 RID: 17762 RVA: 0x0017224B File Offset: 0x0017044B
	private static void OnAuthenticationComplete(Error error)
	{
		ModIOManager.loggingIn = false;
		if (error)
		{
			UnityEvent<string> onModIOLoginFailed = ModIOManager.OnModIOLoginFailed;
			if (onModIOLoginFailed == null)
			{
				return;
			}
			onModIOLoginFailed.Invoke(string.Format("FAILED TO LOGIN TO MOD.IO: {0}", error));
			return;
		}
		else
		{
			UnityEvent onModIOLoggedIn = ModIOManager.OnModIOLoggedIn;
			if (onModIOLoggedIn == null)
			{
				return;
			}
			onModIOLoggedIn.Invoke();
			return;
		}
	}

	// Token: 0x06004563 RID: 17763 RVA: 0x00172288 File Offset: 0x00170488
	public static ModIOManager.ModIOAuthMethod GetLastAuthMethod()
	{
		int @int = PlayerPrefs.GetInt("modIOLassSuccessfulAuthMethod", -1);
		if (@int == -1)
		{
			return ModIOManager.ModIOAuthMethod.Invalid;
		}
		return (ModIOManager.ModIOAuthMethod)@int;
	}

	// Token: 0x06004564 RID: 17764 RVA: 0x001722A8 File Offset: 0x001704A8
	public static async Task<ValueTuple<Error, Mod[]>> GetSubscribedMods()
	{
		ValueTuple<Error, Mod[]> valueTuple;
		if (!ModIOManager.IsLoggedIn())
		{
			valueTuple = new ValueTuple<Error, Mod[]>(new Error(ErrorCode.USER_NOT_AUTHENTICATED), null);
		}
		else
		{
			if (User.Current.IsUpdating)
			{
				while (User.Current.IsUpdating)
				{
					await Task.Yield();
				}
			}
			valueTuple = new ValueTuple<Error, Mod[]>(Error.None, User.Current.ModRepository.GetSubscribed().ToArray<Mod>());
		}
		return valueTuple;
	}

	// Token: 0x06004565 RID: 17765 RVA: 0x001722E4 File Offset: 0x001704E4
	public static async Task<Error> SubscribeToMod(ModId modId, Action<Error> callback)
	{
		Error error2;
		if (!ModIOManager.IsLoggedIn())
		{
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log("[ModIOManager::SubscribeToMod] Called while not logged in!");
			}
			error2 = new Error(ErrorCode.USER_NOT_AUTHENTICATED);
		}
		else
		{
			if (User.Current.IsUpdating)
			{
				ModioLog verbose = ModioLog.Verbose;
				if (verbose != null)
				{
					verbose.Log("[ModIOManager::SubscribeToMod] User currently updating... waiting");
				}
				while (User.Current.IsUpdating)
				{
					await Task.Yield();
				}
			}
			if (User.Current.ModRepository.IsSubscribed(modId))
			{
				ModioLog message = ModioLog.Message;
				if (message != null)
				{
					message.Log(string.Format("[ModIOManager::SubscribeToMod] Already subscribed to Mod {0}", modId));
				}
				if (callback != null)
				{
					callback(Error.None);
				}
				error2 = Error.None;
			}
			else
			{
				ValueTuple<Error, Mod> valueTuple = await Mod.GetMod(modId, false, null, false);
				Error error3 = valueTuple.Item1;
				Mod item = valueTuple.Item2;
				if (error3)
				{
					ModioLog error4 = ModioLog.Error;
					if (error4 != null)
					{
						error4.Log(string.Format("[ModIOManager::SubscribeToMod] Failed to retrieve mod details for Mod {0}: ", modId) + error3.GetMessage());
					}
					if (callback != null)
					{
						callback(error3);
					}
					error2 = error3;
				}
				else
				{
					ModioLog verbose2 = ModioLog.Verbose;
					if (verbose2 != null)
					{
						verbose2.Log(string.Format("[ModIOManager::SubscribeToMod] Subscribing to mod with ID: {0}", modId));
					}
					error3 = await item.Subscribe(true);
					if (error3)
					{
						ModioLog error5 = ModioLog.Error;
						if (error5 != null)
						{
							error5.Log(string.Format("[ModIOManager::SubscribeToMod] Failed to subscribe to Mod {0}: ", modId) + error3.GetMessage());
						}
					}
					if (callback != null)
					{
						callback(error3);
					}
					error2 = error3;
				}
			}
		}
		return error2;
	}

	// Token: 0x06004566 RID: 17766 RVA: 0x00172330 File Offset: 0x00170530
	public static async Task<Error> UnsubscribeFromMod(ModId modId, Action<Error> callback)
	{
		Error error2;
		if (!ModIOManager.IsLoggedIn())
		{
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log("[ModIOManager::UnsubscribeFromMod] Called while not logged in!");
			}
			error2 = new Error(ErrorCode.USER_NOT_AUTHENTICATED);
		}
		else
		{
			if (User.Current.IsUpdating)
			{
				ModioLog verbose = ModioLog.Verbose;
				if (verbose != null)
				{
					verbose.Log("[ModIOManager::UnsubscribeFromMod] User currently updating... waiting");
				}
				while (User.Current.IsUpdating)
				{
					await Task.Yield();
				}
			}
			if (!User.Current.ModRepository.IsSubscribed(modId))
			{
				ModioLog message = ModioLog.Message;
				if (message != null)
				{
					message.Log(string.Format("[ModIOManager::UnsubscribeFromMod] Not currently subscribed to Mod {0}", modId));
				}
				if (callback != null)
				{
					callback(Error.None);
				}
				error2 = Error.None;
			}
			else
			{
				ValueTuple<Error, Mod> valueTuple = await Mod.GetMod(modId, false, null, false);
				Error error3 = valueTuple.Item1;
				Mod item = valueTuple.Item2;
				if (error3)
				{
					ModioLog error4 = ModioLog.Error;
					if (error4 != null)
					{
						error4.Log("[ModIOManager::UnsubscribeFromMod] Failed to retrieve mod details for Mod " + string.Format("{0}: {1}", modId, error3.GetMessage()));
					}
					if (callback != null)
					{
						callback(error3);
					}
					error2 = error3;
				}
				else
				{
					ModioLog verbose2 = ModioLog.Verbose;
					if (verbose2 != null)
					{
						verbose2.Log("[ModIOManager::UnsubscribeFromMod] Unsubscribing from Mod " + modId.ToString());
					}
					error3 = await item.Unsubscribe();
					if (error3)
					{
						ModioLog error5 = ModioLog.Error;
						if (error5 != null)
						{
							error5.Log(string.Format("[ModIOManager::UnsubscribeToMod] Failed to unsubscribe from Mod {0}: ", modId) + error3.GetMessage());
						}
					}
					if (callback != null)
					{
						callback(error3);
					}
					error2 = error3;
				}
			}
		}
		return error2;
	}

	// Token: 0x06004567 RID: 17767 RVA: 0x0017237C File Offset: 0x0017057C
	public static async Task<ValueTuple<bool, ModFileState>> GetSubscribedModStatus(ModId modId)
	{
		ValueTuple<bool, ModFileState> valueTuple;
		if (!ModIOManager.hasInstance)
		{
			valueTuple = new ValueTuple<bool, ModFileState>(false, ModFileState.None);
		}
		else if (!ModIOManager.IsLoggedIn())
		{
			valueTuple = new ValueTuple<bool, ModFileState>(false, ModFileState.None);
		}
		else
		{
			if (User.Current.IsUpdating)
			{
				while (User.Current.IsUpdating)
				{
					await Task.Yield();
				}
			}
			if (!User.Current.ModRepository.IsSubscribed(modId))
			{
				valueTuple = new ValueTuple<bool, ModFileState>(false, ModFileState.None);
			}
			else
			{
				ValueTuple<Error, Mod> valueTuple2 = await Mod.GetMod(modId, false, null, false);
				Error item = valueTuple2.Item1;
				Mod item2 = valueTuple2.Item2;
				if (item)
				{
					ModioLog error = ModioLog.Error;
					if (error != null)
					{
						error.Log("[ModIOManager::GetSubscribedModStatus] Failed to retrieve Mod " + modId.ToString() + "'s status: " + item.GetMessage());
					}
					valueTuple = new ValueTuple<bool, ModFileState>(true, ModFileState.None);
				}
				else
				{
					valueTuple = new ValueTuple<bool, ModFileState>(true, item2.File.State);
				}
			}
		}
		return valueTuple;
	}

	// Token: 0x06004568 RID: 17768 RVA: 0x001723C0 File Offset: 0x001705C0
	public static async Task<ValueTuple<bool, Mod>> GetSubscribedModProfile(ModId modId, Action<bool, Mod> callback = null)
	{
		ValueTuple<bool, Mod> valueTuple;
		if (!ModIOManager.hasInstance)
		{
			if (callback != null)
			{
				callback(false, null);
			}
			valueTuple = new ValueTuple<bool, Mod>(false, null);
		}
		else if (!ModIOManager.IsLoggedIn())
		{
			if (callback != null)
			{
				callback(false, null);
			}
			valueTuple = new ValueTuple<bool, Mod>(false, null);
		}
		else
		{
			if (User.Current.IsUpdating)
			{
				ModioLog verbose = ModioLog.Verbose;
				if (verbose != null)
				{
					verbose.Log("[ModIOManager::GetSubscribedModProfile] Subscriptions currently updating, waiting for Sync to finish...");
				}
				while (User.Current.IsUpdating)
				{
					await Task.Yield();
				}
			}
			ModioLog verbose2 = ModioLog.Verbose;
			if (verbose2 != null)
			{
				verbose2.Log("[ModIOManager::GetSubscribedModProfile] Checking Subscribed Mod list for Mod " + modId.ToString());
			}
			foreach (Mod mod in User.Current.ModRepository.GetSubscribed())
			{
				if (mod.Id.Equals(modId))
				{
					ModioLog verbose3 = ModioLog.Verbose;
					if (verbose3 != null)
					{
						verbose3.Log("[ModIOManager::GetSubscribedModProfile] Found Mod " + modId.ToString() + " in Subscribed Mod list.");
					}
					if (callback != null)
					{
						callback(true, mod);
					}
					return new ValueTuple<bool, Mod>(true, mod);
				}
			}
			ModioLog verbose4 = ModioLog.Verbose;
			if (verbose4 != null)
			{
				verbose4.Log("[ModIOManager::GetSubscribedModProfile] Mod " + modId.ToString() + " not present in Subscribed Mod list.");
			}
			if (callback != null)
			{
				callback(false, null);
			}
			valueTuple = new ValueTuple<bool, Mod>(false, null);
		}
		return valueTuple;
	}

	// Token: 0x06004569 RID: 17769 RVA: 0x0017240C File Offset: 0x0017060C
	public static async Task<ModFileState> GetModStatus(ModId modId)
	{
		ModFileState modFileState;
		if (!ModIOManager.hasInstance)
		{
			modFileState = ModFileState.None;
		}
		else
		{
			ValueTuple<Error, Mod> valueTuple = await Mod.GetMod(modId, false, null, false);
			Error item = valueTuple.Item1;
			Mod item2 = valueTuple.Item2;
			if (item)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log("[ModIOManager::GetModStatus] Failed to retrieve Mod " + modId.ToString() + "'s status: " + item.GetMessage());
				}
				modFileState = ModFileState.None;
			}
			else
			{
				modFileState = item2.File.State;
			}
		}
		return modFileState;
	}

	// Token: 0x0600456A RID: 17770 RVA: 0x00172450 File Offset: 0x00170650
	public static async Task<bool> DownloadMod(ModId modId, Action<bool> callback = null)
	{
		bool flag;
		if (!ModIOManager.hasInstance)
		{
			flag = false;
		}
		else
		{
			bool flag2 = await ModInstallationManagement.DownloadAndInstallMod(modId);
			if (callback != null)
			{
				callback(flag2);
			}
			flag = flag2;
		}
		return flag;
	}

	// Token: 0x0600456B RID: 17771 RVA: 0x0017249C File Offset: 0x0017069C
	private void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.RoomName.Contains(GorillaComputer.instance.VStumpRoomPrepend) && !GorillaComputer.instance.IsPlayerInVirtualStump() && !CustomMapManager.IsLocalPlayerInVirtualStump())
		{
			Debug.LogError("[ModIOManager::OnJoinedRoom] Player joined @ room while not in the VStump! Leaving the room...");
			NetworkSystem.Instance.ReturnToSinglePlayer();
		}
	}

	// Token: 0x0600456C RID: 17772 RVA: 0x001724F0 File Offset: 0x001706F0
	public static bool TryGetNewMapsModId(out ModId newMapsModId)
	{
		newMapsModId = ModId.Null;
		if (!ModIOManager.hasInstance)
		{
			return false;
		}
		newMapsModId = new ModId(ModIOManager.instance.newMapsModId);
		return true;
	}

	// Token: 0x0600456D RID: 17773 RVA: 0x0017251E File Offset: 0x0017071E
	public static IEnumerator AssociateMothershipAndModIOAccounts(AssociateMotherhsipAndModIOAccountsRequest data, Action<AssociateMotherhsipAndModIOAccountsResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/AssociatePlayFabAndModIO", "POST");
		string text = JsonUtility.ToJson(data);
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		request.timeout = 15;
		yield return request.SendWebRequest();
		if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
		{
			AssociateMotherhsipAndModIOAccountsResponse associateMotherhsipAndModIOAccountsResponse = JsonUtility.FromJson<AssociateMotherhsipAndModIOAccountsResponse>(request.downloadHandler.text);
			callback(associateMotherhsipAndModIOAccountsResponse);
		}
		else if (request.result == UnityWebRequest.Result.ProtocolError && request.responseCode != 400L)
		{
			retry = true;
			Debug.LogError(string.Format("HTTP {0} error: {1} message:{2}", request.responseCode, request.error, request.downloadHandler.text));
		}
		else if (request.result == UnityWebRequest.Result.ConnectionError)
		{
			retry = true;
			Debug.LogError("NETWORK ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
		}
		else
		{
			Debug.LogError("HTTP ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
			retry = true;
		}
		if (retry)
		{
			if (ModIOManager.currentAssociationRetries < ModIOManager.associationMaxRetries)
			{
				int num = (int)Mathf.Pow(2f, (float)(ModIOManager.currentAssociationRetries + 1));
				Debug.LogWarning(string.Format("Retrying Account Association... Retry attempt #{0}, waiting for {1} seconds", ModIOManager.currentAssociationRetries + 1, num));
				ModIOManager.currentAssociationRetries++;
				yield return new WaitForSecondsRealtime((float)num);
				ModIOManager.AssociateMothershipAndModIOAccounts(data, callback);
			}
			else
			{
				Debug.LogError("Maximum retries attempted. Please check your network connection.");
				callback(null);
			}
		}
		yield break;
	}

	// Token: 0x04005701 RID: 22273
	private const string MODIO_ACCEPTED_TERMS_KEY = "modIOAcceptedTermsHash";

	// Token: 0x04005702 RID: 22274
	private const string MODIO_ACCEPTED_TERMS_OF_USE_ID_KEY = "modIOAcceptedTermsOfUseId";

	// Token: 0x04005703 RID: 22275
	private const string MODIO_ACCEPTED_PRIVACY_POLICY_ID_KEY = "modIOAcceptedPrivacyPolicyId";

	// Token: 0x04005704 RID: 22276
	public const string FEATURED_MAP_TAG = "Featured";

	// Token: 0x04005705 RID: 22277
	private const int MAX_FEATURED_MAPS_TO_PREFETCH = 50;

	// Token: 0x04005706 RID: 22278
	private const float MAX_PREFETCH_WAIT_PER_MAP_SECONDS = 600f;

	// Token: 0x04005707 RID: 22279
	private const int PREFETCH_POLL_INTERVAL_MS = 2000;

	// Token: 0x04005708 RID: 22280
	private const string MODIO_LAST_AUTH_METHOD_KEY = "modIOLassSuccessfulAuthMethod";

	// Token: 0x04005709 RID: 22281
	private const string FAVORITES_FILE_NAME = "favoriteMods.json";

	// Token: 0x0400570A RID: 22282
	private const float REFRESH_RATE_LIMIT = 5f;

	// Token: 0x0400570B RID: 22283
	[OnEnterPlay_SetNull]
	private static volatile ModIOManager instance;

	// Token: 0x0400570C RID: 22284
	[OnEnterPlay_Set(false)]
	private static bool hasInstance;

	// Token: 0x0400570D RID: 22285
	private static string ModIODirectory;

	// Token: 0x0400570E RID: 22286
	private static ModioWssAuthService accountLinkingAuthService = new ModioWssAuthService();

	// Token: 0x0400570F RID: 22287
	private static bool initialized;

	// Token: 0x04005710 RID: 22288
	[OnEnterPlay_Set(false)]
	private static bool featuredMapsPrefetchStarted;

	// Token: 0x04005711 RID: 22289
	[OnEnterPlay_Set(false)]
	private static bool featuredMapsRetrieved;

	// Token: 0x04005712 RID: 22290
	private static readonly List<Mod> retrievedFeaturedMaps = new List<Mod>();

	// Token: 0x04005713 RID: 22291
	private static bool refreshing;

	// Token: 0x04005714 RID: 22292
	private static bool modManagementEnabled;

	// Token: 0x04005715 RID: 22293
	private static bool loggingIn;

	// Token: 0x04005716 RID: 22294
	private static bool loggingOut;

	// Token: 0x04005717 RID: 22295
	private static bool refreshingModCache;

	// Token: 0x04005718 RID: 22296
	private static bool favoriteModsLoaded;

	// Token: 0x04005719 RID: 22297
	private static bool restartRefreshModCache;

	// Token: 0x0400571A RID: 22298
	private static Coroutine refreshDisabledCoroutine;

	// Token: 0x0400571B RID: 22299
	private static float lastRefreshTime;

	// Token: 0x0400571C RID: 22300
	private static List<Action<bool>> currentRefreshCallbacks = new List<Action<bool>>();

	// Token: 0x0400571D RID: 22301
	private static Action<ModIORequestResultAnd<bool>> modIOTermsAcknowledgedCallback;

	// Token: 0x0400571E RID: 22302
	private static Dictionary<ModId, Mod> favoriteMods = new Dictionary<ModId, Mod>();

	// Token: 0x0400571F RID: 22303
	private static Dictionary<ModId, int> outdatedModCMSVersions = new Dictionary<ModId, int>();

	// Token: 0x04005720 RID: 22304
	private static byte[] ticketBlob = new byte[1024];

	// Token: 0x04005721 RID: 22305
	private static uint ticketSize;

	// Token: 0x04005722 RID: 22306
	protected static CallResult<EncryptedAppTicketResponse_t> requestEncryptedAppTicketResponse = null;

	// Token: 0x04005723 RID: 22307
	private Action<bool, string> requestEncryptedAppTicketCallback;

	// Token: 0x04005724 RID: 22308
	private static ModioSteamAuthService steamAuthService = new ModioSteamAuthService();

	// Token: 0x04005725 RID: 22309
	[SerializeField]
	private GameObject modIOTermsOfUsePrefab;

	// Token: 0x04005726 RID: 22310
	[SerializeField]
	private long newMapsModId;

	// Token: 0x04005727 RID: 22311
	public static UnityEvent OnModIOLoginStarted = new UnityEvent();

	// Token: 0x04005728 RID: 22312
	public static UnityEvent OnModIOLoggedIn = new UnityEvent();

	// Token: 0x04005729 RID: 22313
	public static UnityEvent<string> OnModIOLoginFailed = new UnityEvent<string>();

	// Token: 0x0400572A RID: 22314
	public static UnityEvent OnModIOLoggedOut = new UnityEvent();

	// Token: 0x0400572B RID: 22315
	public static UnityEvent<User> OnModIOUserChanged = new UnityEvent<User>();

	// Token: 0x0400572C RID: 22316
	public static UnityEvent<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase> OnModManagementEvent = new UnityEvent<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>();

	// Token: 0x0400572D RID: 22317
	public static UnityEvent OnModIOCacheRefreshing = new UnityEvent();

	// Token: 0x0400572E RID: 22318
	public static UnityEvent OnModIOCacheRefreshed = new UnityEvent();

	// Token: 0x0400572F RID: 22319
	private static int associationMaxRetries = 5;

	// Token: 0x04005730 RID: 22320
	private static int currentAssociationRetries = 0;

	// Token: 0x02000A7E RID: 2686
	public enum ModIOAuthMethod
	{
		// Token: 0x04005732 RID: 22322
		Invalid,
		// Token: 0x04005733 RID: 22323
		LinkedAccount,
		// Token: 0x04005734 RID: 22324
		Steam,
		// Token: 0x04005735 RID: 22325
		Oculus
	}
}
