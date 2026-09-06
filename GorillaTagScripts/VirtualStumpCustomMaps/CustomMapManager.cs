using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaNetworking;
using GorillaTag.Rendering;
using GorillaTagScripts.CustomMapSupport;
using GorillaTagScripts.UI.ModIO;
using GT_CustomMapSupportRuntime;
using Modio;
using Modio.Mods;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000FCF RID: 4047
	public class CustomMapManager : MonoBehaviour, IBuildValidation
	{
		// Token: 0x17000993 RID: 2451
		// (get) Token: 0x0600647E RID: 25726 RVA: 0x00204CFB File Offset: 0x00202EFB
		public static bool WaitingForRoomJoin
		{
			get
			{
				return CustomMapManager.waitingForRoomJoin;
			}
		}

		// Token: 0x17000994 RID: 2452
		// (get) Token: 0x0600647F RID: 25727 RVA: 0x00204D02 File Offset: 0x00202F02
		public static bool WaitingForDisconnect
		{
			get
			{
				return CustomMapManager.waitingForDisconnect;
			}
		}

		// Token: 0x17000995 RID: 2453
		// (get) Token: 0x06006480 RID: 25728 RVA: 0x00204D09 File Offset: 0x00202F09
		public static long LoadingMapId
		{
			get
			{
				return CustomMapManager.loadingMapId;
			}
		}

		// Token: 0x17000996 RID: 2454
		// (get) Token: 0x06006481 RID: 25729 RVA: 0x00204D15 File Offset: 0x00202F15
		public static long UnloadingMapId
		{
			get
			{
				return CustomMapManager.unloadingMapId;
			}
		}

		// Token: 0x17000997 RID: 2455
		// (get) Token: 0x06006482 RID: 25730 RVA: 0x00204D21 File Offset: 0x00202F21
		public static MapLoadStatus CurrentLoadStatus
		{
			get
			{
				return CustomMapManager.currentLoadStatus;
			}
		}

		// Token: 0x17000998 RID: 2456
		// (get) Token: 0x06006483 RID: 25731 RVA: 0x00204D28 File Offset: 0x00202F28
		public static int CurrentLoadProgress
		{
			get
			{
				return CustomMapManager.currentLoadProgress;
			}
		}

		// Token: 0x17000999 RID: 2457
		// (get) Token: 0x06006484 RID: 25732 RVA: 0x00204D2F File Offset: 0x00202F2F
		public static string CurrentLoadMessage
		{
			get
			{
				return CustomMapManager.currentLoadMessage;
			}
		}

		// Token: 0x06006485 RID: 25733 RVA: 0x00204D36 File Offset: 0x00202F36
		public bool BuildValidationCheck()
		{
			if (this.defaultTeleporter.IsNull())
			{
				Debug.LogError("CustomMapManager does not have its \"Default Teleporter\" property.");
				return false;
			}
			return true;
		}

		// Token: 0x06006486 RID: 25734 RVA: 0x00204D52 File Offset: 0x00202F52
		private void Awake()
		{
			if (CustomMapManager.instance == null)
			{
				CustomMapManager.instance = this;
				CustomMapManager.hasInstance = true;
				return;
			}
			if (CustomMapManager.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06006487 RID: 25735 RVA: 0x00204D8C File Offset: 0x00202F8C
		public void OnEnable()
		{
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.SubscribeToUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			UGCPermissionManager.SubscribeToUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			CMSSerializer.OnTriggerHistoryProcessedForScene.AddListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
			ModIOManager.OnModManagementEvent.AddListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
			Mod.RemoveChangeListener(ModChangeType.DownloadProgress | ModChangeType.FileState, new Action<Mod, ModChangeType>(CustomMapManager.HandleModFileProgress));
			Mod.AddChangeListener(ModChangeType.DownloadProgress | ModChangeType.FileState, new Action<Mod, ModChangeType>(CustomMapManager.HandleModFileProgress));
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnected;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnected;
		}

		// Token: 0x06006488 RID: 25736 RVA: 0x00204ED4 File Offset: 0x002030D4
		public void OnDisable()
		{
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
			Mod.RemoveChangeListener(ModChangeType.DownloadProgress | ModChangeType.FileState, new Action<Mod, ModChangeType>(CustomMapManager.HandleModFileProgress));
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnected;
		}

		// Token: 0x06006489 RID: 25737 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void OnUGCEnabled()
		{
		}

		// Token: 0x0600648A RID: 25738 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void OnUGCDisabled()
		{
		}

		// Token: 0x0600648B RID: 25739 RVA: 0x00204F80 File Offset: 0x00203180
		private void Start()
		{
			CustomMapLoader.Initialize(new Action<MapLoadStatus, int, string>(CustomMapManager.OnMapLoadProgress), new Action<bool>(CustomMapManager.OnMapLoadFinished), new Action<string>(CustomMapManager.OnSceneLoaded), new Action<string>(CustomMapManager.OnSceneUnloaded));
			for (int i = this.virtualStumpTeleportLocations.Count - 1; i >= 0; i--)
			{
				if (this.virtualStumpTeleportLocations[i] == null)
				{
					this.virtualStumpTeleportLocations.RemoveAt(i);
				}
			}
			if (this.defaultTeleporter.IsNull())
			{
				GTDev.LogError<string>("[CustomMapManager::Start] \"Default Teleporter\" property is invalid.", null);
			}
			this.virtualStumpToggleableRoot.SetActive(false);
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600648C RID: 25740 RVA: 0x0020502C File Offset: 0x0020322C
		private void OnDestroy()
		{
			if (CustomMapManager.instance == this)
			{
				CustomMapManager.instance = null;
				CustomMapManager.hasInstance = false;
			}
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
			Mod.RemoveChangeListener(ModChangeType.DownloadProgress | ModChangeType.FileState, new Action<Mod, ModChangeType>(CustomMapManager.HandleModFileProgress));
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnected;
		}

		// Token: 0x0600648D RID: 25741 RVA: 0x002050F3 File Offset: 0x002032F3
		public static void TrackMapDownload(ModId modId)
		{
			CustomMapManager.ResetModFileProgressTracking();
			CustomMapManager.trackedDownloadMapId = modId;
			CustomMapManager.BroadcastMapLoadProgress(MapLoadStatus.Downloading, 0, "WAITING FOR DOWNLOAD");
		}

		// Token: 0x0600648E RID: 25742 RVA: 0x0020510C File Offset: 0x0020330C
		public static void StopTrackingMapDownload()
		{
			CustomMapManager.trackedDownloadMapId = ModId.Null;
			if (!CustomMapManager.loadInProgress)
			{
				CustomMapManager.ResetModFileProgressTracking();
				CustomMapManager.BroadcastMapLoadProgress(MapLoadStatus.None, 0, "");
			}
		}

		// Token: 0x0600648F RID: 25743 RVA: 0x00205130 File Offset: 0x00203330
		private static bool IsPlayerWaitingOnMap(ModId modId)
		{
			return (CustomMapManager.loadInProgress && CustomMapManager.loadingMapId == modId) || (CustomMapManager.trackedDownloadMapId != ModId.Null && CustomMapManager.trackedDownloadMapId == modId);
		}

		// Token: 0x06006490 RID: 25744 RVA: 0x00205166 File Offset: 0x00203366
		private static void HandleModFileProgress(Mod mod, ModChangeType changeType)
		{
			if (mod == null || mod.File == null)
			{
				return;
			}
			if (!CustomMapManager.IsPlayerWaitingOnMap(mod.Id))
			{
				return;
			}
			CustomMapManager.BroadcastModFileState(mod);
		}

		// Token: 0x06006491 RID: 25745 RVA: 0x00205188 File Offset: 0x00203388
		private static void BroadcastModFileState(Mod mod)
		{
			if (((mod != null) ? mod.File : null) == null)
			{
				return;
			}
			switch (mod.File.State)
			{
			case ModFileState.None:
			case ModFileState.Queued:
				CustomMapManager.BroadcastModFileProgress(MapLoadStatus.Downloading, 0, "WAITING FOR DOWNLOAD");
				return;
			case ModFileState.Downloading:
				CustomMapManager.BroadcastModFileProgress(MapLoadStatus.Downloading, CustomMapManager.GetFileStatePercent(mod), "DOWNLOADING MAP FILES");
				return;
			case ModFileState.Downloaded:
				CustomMapManager.BroadcastModFileProgress(MapLoadStatus.Installing, 0, "WAITING TO INSTALL");
				return;
			case ModFileState.Installing:
			case ModFileState.Updating:
				CustomMapManager.BroadcastModFileProgress(MapLoadStatus.Installing, CustomMapManager.GetFileStatePercent(mod), "INSTALLING MAP FILES");
				return;
			case ModFileState.Installed:
				if (!CustomMapManager.loadInProgress)
				{
					CustomMapManager.trackedDownloadMapId = ModId.Null;
					CustomMapManager.BroadcastModFileProgress(MapLoadStatus.None, 0, "");
					return;
				}
				break;
			case ModFileState.Uninstalling:
				break;
			case ModFileState.FileOperationFailed:
				CustomMapManager.trackedDownloadMapId = ModId.Null;
				if (!CustomMapManager.loadInProgress)
				{
					CustomMapManager.BroadcastModFileProgress(MapLoadStatus.Error, 0, mod.File.FileStateErrorCause.GetMessage() ?? "MAP DOWNLOAD FAILED");
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006492 RID: 25746 RVA: 0x00205268 File Offset: 0x00203468
		private static int GetFileStatePercent(Mod mod)
		{
			return Mathf.Clamp(Mathf.RoundToInt(mod.File.FileStateProgress * 100f), 0, 100);
		}

		// Token: 0x06006493 RID: 25747 RVA: 0x00205288 File Offset: 0x00203488
		private static void ResetModFileProgressTracking()
		{
			CustomMapManager.lastBroadcastFileStatus = MapLoadStatus.None;
			CustomMapManager.lastBroadcastFilePercent = -1;
			CustomMapManager.trackedDownloadMapId = ModId.Null;
		}

		// Token: 0x06006494 RID: 25748 RVA: 0x002052A0 File Offset: 0x002034A0
		private static void BroadcastModFileProgress(MapLoadStatus status, int percent, string message)
		{
			if (status == CustomMapManager.lastBroadcastFileStatus && percent == CustomMapManager.lastBroadcastFilePercent)
			{
				return;
			}
			CustomMapManager.lastBroadcastFileStatus = status;
			CustomMapManager.lastBroadcastFilePercent = percent;
			CustomMapManager.BroadcastMapLoadProgress(status, percent, message);
		}

		// Token: 0x06006495 RID: 25749 RVA: 0x002052C8 File Offset: 0x002034C8
		private void HandleModManagementEvent(Mod mod, Modfile modfile, ModInstallationManagement.OperationType jobType, ModInstallationManagement.OperationPhase jobPhase)
		{
			if (CustomMapManager.waitingForModInstall && CustomMapManager.waitingForModInstallId == mod.Id)
			{
				if (CustomMapManager.abortModLoadIds.Contains(mod.Id))
				{
					CustomMapManager.abortModLoadIds.Remove(mod.Id);
					if (CustomMapManager.waitingForModInstallId.Equals(mod.Id))
					{
						CustomMapManager.waitingForModInstall = false;
						CustomMapManager.waitingForModDownload = false;
						CustomMapManager.waitingForModInstallId = ModId.Null;
					}
					return;
				}
				switch (modfile.State)
				{
				case ModFileState.Downloading:
				case ModFileState.Updating:
					CustomMapManager.waitingForModDownload = true;
					return;
				case ModFileState.Downloaded:
					CustomMapManager.waitingForModDownload = false;
					return;
				case ModFileState.Installing:
				case ModFileState.Uninstalling:
					break;
				case ModFileState.Installed:
					CustomMapManager.waitingForModDownload = false;
					this.LoadInstalledMap(mod);
					break;
				case ModFileState.FileOperationFailed:
					switch (jobType)
					{
					case ModInstallationManagement.OperationType.Download:
						Debug.LogError("[CustomMapManager::HandleModManagementEvent] Failed to download map with modID " + mod.Id.ToString() + ", error: " + modfile.FileStateErrorCause.GetMessage());
						CustomMapManager.HandleMapLoadFailed("FAILED TO DOWNLOAD MAP: " + modfile.FileStateErrorCause.GetMessage());
						CustomMapManager.waitingForModDownload = false;
						return;
					case ModInstallationManagement.OperationType.Install:
						Debug.LogError("[CustomMapManager::HandleModManagementEvent] Failed to install map with modID " + mod.Id.ToString() + ", error: " + modfile.FileStateErrorCause.GetMessage());
						CustomMapManager.HandleMapLoadFailed("FAILED TO INSTALL MAP: " + modfile.FileStateErrorCause.GetMessage());
						return;
					case ModInstallationManagement.OperationType.Update:
						Debug.LogError("[CustomMapManager::HandleModManagementEvent] Failed to update map with modID " + mod.Id.ToString() + ", error: " + modfile.FileStateErrorCause.GetMessage());
						CustomMapManager.HandleMapLoadFailed("FAILED TO UPDATE MAP: " + modfile.FileStateErrorCause.GetMessage());
						return;
					default:
						return;
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06006496 RID: 25750 RVA: 0x00205498 File Offset: 0x00203698
		internal static void TeleportToVirtualStump(VirtualStumpTeleporter fromTeleporter, Action<bool> callback)
		{
			if (UGCPermissionManager.HasNoMapAccess)
			{
				return;
			}
			if (!CustomMapManager.hasInstance || fromTeleporter == null)
			{
				if (callback != null)
				{
					callback(false);
				}
				return;
			}
			CustomMapManager.activateSkipTeleport = false;
			CustomMapManager.activateDeferZoneToNode = false;
			CustomMapManager.activateHasAutoLoadOverride = false;
			CustomMapManager.activateIsActive = false;
			CustomMapManager.instance.gameObject.SetActive(true);
			CustomMapManager.instance.StartCoroutine(CustomMapManager.Internal_TeleportToVirtualStump(fromTeleporter, callback));
		}

		// Token: 0x06006497 RID: 25751 RVA: 0x00205508 File Offset: 0x00203708
		public static async void Activate(VirtualStumpActivateMode mode, bool hasEntryTeleportNode = true)
		{
			if (!UGCPermissionManager.HasNoMapAccess)
			{
				if (CustomMapManager.hasInstance)
				{
					if (!CustomMapManager.activateIsActive)
					{
						CustomMapManager.activateIsActive = true;
						CustomMapManager.activateCurrentMode = mode;
						CustomMapManager.SetFeaturedMapObjectsHidden(CustomMapManager.IsInFeaturedMode());
						if (GorillaComputer.hasInstance)
						{
							GorillaComputer.instance.SetVStumpRoomModePrefix(CustomMapManager.GetActivateRoomModePrefix());
						}
						ModId autoLoadModId = ModId.Null;
						if (mode != VirtualStumpActivateMode.Custom)
						{
							int index = ((mode == VirtualStumpActivateMode.FeatureA) ? 0 : 1);
							ValueTuple<Error, List<Mod>> valueTuple = await ModIOManager.GetFeaturedMaps(false);
							Error item = valueTuple.Item1;
							List<Mod> item2 = valueTuple.Item2;
							if (item || item2 == null || index >= item2.Count)
							{
								GTDev.LogWarning<string>("[CustomMapManager::Activate] Could not resolve featured map index " + string.Format("{0} for {1}; opening the stump without an auto-load.", index, mode), null);
							}
							else
							{
								autoLoadModId = item2[index].Id;
							}
						}
						if (!CustomMapManager.hasInstance)
						{
							CustomMapManager.activateIsActive = false;
						}
						else if (CustomMapManager.instance.defaultTeleporter.IsNull())
						{
							GTDev.LogError<string>("[CustomMapManager::Activate] Default Teleporter is not set; cannot activate.", null);
							CustomMapManager.activateIsActive = false;
						}
						else
						{
							CustomMapManager.activateSkipTeleport = true;
							CustomMapManager.activateDeferZoneToNode = hasEntryTeleportNode;
							CustomMapManager.activateHasAutoLoadOverride = true;
							CustomMapManager.activateAutoLoadModIdOverride = autoLoadModId;
							CustomMapManager.instance.gameObject.SetActive(true);
							CustomMapManager.instance.StartCoroutine(CustomMapManager.Internal_TeleportToVirtualStump(CustomMapManager.instance.defaultTeleporter, null));
						}
					}
				}
			}
		}

		// Token: 0x06006498 RID: 25752 RVA: 0x00205547 File Offset: 0x00203747
		public static void Deactivate()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (!GorillaComputer.hasInstance || !GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				return;
			}
			CustomMapManager.activateIsActive = false;
			CustomMapManager.activateSkipTeleport = true;
			CustomMapManager.ExitVirtualStump(null);
		}

		// Token: 0x1700099A RID: 2458
		// (get) Token: 0x06006499 RID: 25753 RVA: 0x00205579 File Offset: 0x00203779
		public static VirtualStumpActivateMode CurrentActivateMode
		{
			get
			{
				return CustomMapManager.activateCurrentMode;
			}
		}

		// Token: 0x0600649A RID: 25754 RVA: 0x00205580 File Offset: 0x00203780
		public static bool IsInFeaturedMode()
		{
			return CustomMapManager.activateCurrentMode == VirtualStumpActivateMode.FeatureA || CustomMapManager.activateCurrentMode == VirtualStumpActivateMode.FeatureB;
		}

		// Token: 0x0600649B RID: 25755 RVA: 0x00205594 File Offset: 0x00203794
		public static bool IsFeaturedMapLocked()
		{
			return CustomMapManager.activateIsActive && CustomMapManager.IsInFeaturedMode() && CustomMapManager.activateAutoLoadModIdOverride != ModId.Null;
		}

		// Token: 0x1700099B RID: 2459
		// (get) Token: 0x0600649C RID: 25756 RVA: 0x002055B5 File Offset: 0x002037B5
		public static ModId FeaturedLockedMapId
		{
			get
			{
				return CustomMapManager.activateAutoLoadModIdOverride;
			}
		}

		// Token: 0x0600649D RID: 25757 RVA: 0x002055BC File Offset: 0x002037BC
		private static void SetFeaturedMapObjectsHidden(bool hidden)
		{
			if (!CustomMapManager.hasInstance || CustomMapManager.instance.featuredMapDisabledObjects == null)
			{
				return;
			}
			foreach (GameObject gameObject in CustomMapManager.instance.featuredMapDisabledObjects)
			{
				if (!(gameObject == null))
				{
					foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>(true))
					{
						if (renderer != null)
						{
							renderer.forceRenderingOff = hidden;
						}
					}
					foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>(true))
					{
						if (collider != null)
						{
							collider.enabled = !hidden;
						}
					}
				}
			}
		}

		// Token: 0x0600649E RID: 25758 RVA: 0x00205690 File Offset: 0x00203890
		public static void PrepareFeaturedMapReloadOnRoomChange()
		{
			CustomMapManager.pendingRoomChangeReloadModId = CustomMapManager.GetRoomMapId();
		}

		// Token: 0x0600649F RID: 25759 RVA: 0x0020569C File Offset: 0x0020389C
		public static void EnterVirtualStumpZone()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (VRRig.LocalRig.IsNotNull() && VRRig.LocalRig.zoneEntity.IsNotNull())
			{
				VRRig.LocalRig.zoneEntity.DisableZoneChanges();
			}
			ZoneManagement.SetActiveZone(GTZone.customMaps);
			foreach (GameObject gameObject in CustomMapManager.instance.rootObjectsToDeactivateAfterTeleport)
			{
				if (gameObject != null && gameObject.gameObject != null)
				{
					gameObject.gameObject.SetActive(false);
				}
			}
			if (CustomMapManager.instance.virtualStumpZoneShaderSettings.IsNotNull())
			{
				CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(false);
				return;
			}
			ZoneShaderSettings.ActivateDefaultSettings();
		}

		// Token: 0x060064A0 RID: 25760 RVA: 0x00205751 File Offset: 0x00203951
		public void OnEnteredVirtualStumpZone()
		{
			CustomMapManager.EnterVirtualStumpZone();
		}

		// Token: 0x060064A1 RID: 25761 RVA: 0x00205758 File Offset: 0x00203958
		private static string GetActivateRoomModePrefix()
		{
			VirtualStumpActivateMode virtualStumpActivateMode = CustomMapManager.activateCurrentMode;
			if (virtualStumpActivateMode == VirtualStumpActivateMode.FeatureA)
			{
				return "A";
			}
			if (virtualStumpActivateMode != VirtualStumpActivateMode.FeatureB)
			{
				return "C";
			}
			return "B";
		}

		// Token: 0x060064A2 RID: 25762 RVA: 0x00205786 File Offset: 0x00203986
		private static ModId GetEffectiveAutoLoadModId()
		{
			if (CustomMapManager.activateHasAutoLoadOverride)
			{
				return CustomMapManager.activateAutoLoadModIdOverride;
			}
			if (!CustomMapManager.lastUsedTeleporter.IsNotNull())
			{
				return ModId.Null;
			}
			return CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId();
		}

		// Token: 0x060064A3 RID: 25763 RVA: 0x002057B6 File Offset: 0x002039B6
		private static IEnumerator Internal_TeleportToVirtualStump(VirtualStumpTeleporter fromTeleporter, Action<bool> callback)
		{
			CustomMapManager.lastUsedTeleporter = fromTeleporter;
			CustomMapManager.preVStumpGamemode = GorillaComputer.instance.currentGameMode.Value;
			if (CustomMapManager.lastUsedTeleporter.GetAutoLoadGamemode() != GameModeType.None && CustomMapManager.lastUsedTeleporter.GetAutoLoadGamemode() != GameModeType.Count)
			{
				GorillaComputer.instance.SetGameModeWithoutButton(CustomMapManager.lastUsedTeleporter.GetAutoLoadGamemode().ToString());
			}
			GTDev.Log<string>("[CustomMapManager::TeleportToVirtualStump] Teleporting to Virtual Stump...", null);
			if (!CustomMapManager.activateSkipTeleport)
			{
				PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.CustomMap, "");
				GorillaTagger.Instance.overrideNotInFocus = true;
			}
			GreyZoneManager greyZoneManager = GreyZoneManager.Instance;
			if (greyZoneManager != null)
			{
				greyZoneManager.ForceStopGreyZone();
			}
			if (CustomMapManager.instance.virtualStumpTeleportLocations.Count > 0)
			{
				int num = Random.Range(0, CustomMapManager.instance.virtualStumpTeleportLocations.Count);
				Transform randTeleportTarget = CustomMapManager.instance.virtualStumpTeleportLocations[num];
				if (!CustomMapManager.activateSkipTeleport)
				{
					CustomMapManager.instance.EnableTeleportHUD(true);
					CustomMapManager.lastUsedTeleporter.PlayTeleportEffects(true, true, CustomMapManager.instance.localTeleportSFXSource, true);
				}
				yield return new WaitForSeconds(0.75f);
				CosmeticsController.instance.ClearCheckoutAndCart(false);
				CustomMapManager.instance.virtualStumpToggleableRoot.SetActive(true);
				if (!CustomMapManager.activateSkipTeleport)
				{
					GTPlayer.Instance.TeleportTo(randTeleportTarget, true, false);
				}
				GorillaComputer.instance.SetInVirtualStump(true);
				yield return null;
				if (!CustomMapManager.activateDeferZoneToNode)
				{
					CustomMapManager.EnterVirtualStumpZone();
				}
				CustomMapManager.instance.ghostReactorManager.reactor.EnableGhostReactorForVirtualStump();
				CustomMapManager.currentTeleportCallback = callback;
				CustomMapManager.pendingNewPrivateRoomName = "";
				CustomMapManager.preTeleportInPrivateRoom = false;
				if (NetworkSystem.Instance.InRoom)
				{
					if (NetworkSystem.Instance.SessionIsPrivate)
					{
						CustomMapManager.preTeleportInPrivateRoom = true;
						CustomMapManager.waitingForRoomJoin = true;
						CustomMapManager.pendingNewPrivateRoomName = CustomMapManager.GetActivateRoomModePrefix() + GorillaComputer.instance.VStumpRoomPrepend + NetworkSystem.Instance.RoomName;
					}
					GTDev.Log<string>("[CustomMapManager::TeleportToVirtualStump] Returning to singleplayer...", null);
					CustomMapManager.waitingForLoginDisconnect = true;
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
				else
				{
					GTDev.Log<string>("[CustomMapManager::TeleportToVirtualStump] Attempting auto-login to mod.io...", null);
					CustomMapManager.AttemptAutoLogin();
				}
				randTeleportTarget = null;
			}
			else
			{
				GTDev.Log<string>("[CustomMapManager::TeleportToVirtualStump] Not Teleporting, virtualStumpTeleportLocations is empty!", null);
				CustomMapManager.EndTeleport(false);
			}
			yield break;
		}

		// Token: 0x060064A4 RID: 25764 RVA: 0x002057CC File Offset: 0x002039CC
		private static void OnAutoLoginComplete(Error error)
		{
			GTDev.Log<string>(string.Format("[CustomMapManager::OnAutoLoginComplete] Error: {0}", error), null);
			if (!CustomMapManager.hasInstance)
			{
				Debug.LogError("[CustomMapManager::OnAutoLoginComplete] CustomMapManager not initialized!");
				return;
			}
			GTDev.Log<string>(string.Format("[CustomMapManager::OnAutoLoginComplete] Needs to rejoin private room: {0}", CustomMapManager.preTeleportInPrivateRoom), null);
			if (CustomMapManager.preTeleportInPrivateRoom)
			{
				if (NetworkSystem.Instance.netState != NetSystemState.Idle)
				{
					GTDev.Log<string>(string.Format("[CustomMapManager::OnAutoLoginComplete] Netstate not Idle, delaying join attempt. CurrentStatus: {0}", NetworkSystem.Instance.netState), null);
					CustomMapManager.delayedJoinCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedJoinVStumpPrivateRoom());
				}
				else
				{
					GTDev.Log<string>("[CustomMapManager::OnAutoLoginComplete] joining @ version of private room: " + CustomMapManager.pendingNewPrivateRoomName, null);
					PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
				}
			}
			GTDev.Log<string>(string.Format("[CustomMapManager::OnAutoLoginComplete] Waiting For D/C? {0}", CustomMapManager.waitingForDisconnect), null);
			if (!CustomMapManager.preTeleportInPrivateRoom && !CustomMapManager.waitingForDisconnect)
			{
				GTDev.Log<string>("[CustomMapManager::OnAutoLoginComplete] Ending teleport...", null);
				CustomMapManager.EndTeleport(true);
			}
			CustomMapManager.preTeleportInPrivateRoom = false;
		}

		// Token: 0x060064A5 RID: 25765 RVA: 0x002058D3 File Offset: 0x00203AD3
		private static IEnumerator DelayedJoinVStumpPrivateRoom()
		{
			GTDev.Log<string>("[CustomMapManager::DelayedJoinVStumpPrivateRoom] waiting for netstate to be Idle", null);
			while (NetworkSystem.Instance.netState != NetSystemState.Idle)
			{
				yield return null;
			}
			GTDev.Log<string>("[CustomMapManager::DelayedJoinVStumpPrivateRoom] joining @ version of private room: " + CustomMapManager.pendingNewPrivateRoomName, null);
			PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
			yield break;
		}

		// Token: 0x060064A6 RID: 25766 RVA: 0x002058DC File Offset: 0x00203ADC
		public static void ExitVirtualStump(Action<bool> callback)
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.lastUsedTeleporter.IsNull())
			{
				if (CustomMapManager.instance.defaultTeleporter.IsNull())
				{
					if (callback != null)
					{
						callback(false);
					}
				}
				else
				{
					CustomMapManager.lastUsedTeleporter = CustomMapManager.instance.defaultTeleporter;
				}
			}
			if (CustomMapManager.delayedJoinCoroutine != null)
			{
				CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedJoinCoroutine);
				CustomMapManager.delayedJoinCoroutine = null;
			}
			if (CustomMapManager.delayedTryAutoLoadCoroutine != null)
			{
				CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedTryAutoLoadCoroutine);
				CustomMapManager.delayedTryAutoLoadCoroutine = null;
			}
			CustomMapManager.instance.dayNightManager.RequestRepopulateLightmaps();
			if (!CustomMapManager.activateSkipTeleport)
			{
				PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.CustomMap, "");
				GorillaTagger.Instance.overrideNotInFocus = true;
			}
			if (!CustomMapManager.activateSkipTeleport)
			{
				CustomMapManager.instance.EnableTeleportHUD(false);
			}
			CustomMapManager.currentTeleportCallback = callback;
			CustomMapManager.exitVirtualStumpPending = true;
			if (!CustomMapManager.UnloadMap(false))
			{
				CustomMapManager.FinalizeExitVirtualStump();
			}
		}

		// Token: 0x060064A7 RID: 25767 RVA: 0x002059C4 File Offset: 0x00203BC4
		private static void FinalizeExitVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			GTPlayer.Instance.SetHoverActive(false);
			VRRig.LocalRig.hoverboardVisual.SetNotHeld();
			RoomSystem.ClearOverridenRoomSize();
			CosmeticsController.instance.ClearCheckoutAndCart(false);
			foreach (GameObject gameObject in CustomMapManager.instance.rootObjectsToDeactivateAfterTeleport)
			{
				if (gameObject != null)
				{
					gameObject.gameObject.SetActive(true);
				}
			}
			if (CustomMapManager.lastUsedTeleporter.GetReturnGamemode() != GameModeType.None && CustomMapManager.lastUsedTeleporter.GetReturnGamemode() != GameModeType.Count)
			{
				GorillaComputer.instance.SetGameModeWithoutButton(CustomMapManager.lastUsedTeleporter.GetReturnGamemode().ToString());
			}
			else if (CustomMapManager.preVStumpGamemode != "")
			{
				GorillaComputer.instance.SetGameModeWithoutButton(CustomMapManager.preVStumpGamemode);
				CustomMapManager.preVStumpGamemode = "";
			}
			if (VRRig.LocalRig.IsNotNull())
			{
				GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
				if (component != null && component.State == GRPlayer.GRPlayerState.Ghost)
				{
					CustomMapManager.instance.defaultReviveStation.RevivePlayer(component);
				}
			}
			if (!CustomMapManager.activateSkipTeleport)
			{
				ZoneManagement.SetActiveZone(CustomMapManager.lastUsedTeleporter.GetZone());
			}
			if (VRRig.LocalRig.IsNotNull() && VRRig.LocalRig.zoneEntity.IsNotNull())
			{
				VRRig.LocalRig.zoneEntity.EnableZoneChanges();
			}
			GorillaComputer.instance.SetInVirtualStump(false);
			CustomMapManager.activateIsActive = false;
			CustomMapManager.SetFeaturedMapObjectsHidden(false);
			if (!CustomMapManager.activateSkipTeleport)
			{
				GTPlayer.Instance.TeleportTo(CustomMapManager.lastUsedTeleporter.GetReturnTransform(), true, false);
			}
			CustomMapManager.instance.virtualStumpToggleableRoot.SetActive(false);
			ZoneShaderSettings.ActivateDefaultSettings();
			VRRig.LocalRig.EnableVStumpReturnWatch(false);
			GTPlayer.Instance.ForceHoverDisallowed();
			CustomMapManager.exitVirtualStumpPending = false;
			if (CustomMapManager.delayedEndTeleportCoroutine != null)
			{
				CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedEndTeleportCoroutine);
			}
			CustomMapManager.delayedEndTeleportCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedEndTeleport());
			if (CustomMapManager.preTeleportInPrivateRoom)
			{
				CustomMapManager.waitingForRoomJoin = true;
				CustomMapManager.pendingNewPrivateRoomName = GorillaComputer.instance.StripVStumpRoomPrefix(CustomMapManager.pendingNewPrivateRoomName);
				PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
				return;
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					CustomMapManager.waitingForRoomJoin = true;
					CustomMapManager.pendingNewPrivateRoomName = GorillaComputer.instance.StripVStumpRoomPrefix(NetworkSystem.Instance.RoomName);
					PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
					return;
				}
				if (CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger() != null)
				{
					CustomMapManager.waitingForRoomJoin = true;
					GorillaComputer.instance.allowedMapsToJoin = CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger().myCollider.myAllowedMapsToJoin;
					Debug.Log(string.Format("[CustomMapManager::FinalizeExit] allowedMaps: {0}", GorillaComputer.instance.allowedMapsToJoin));
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger(), JoinType.Solo, null, false);
					return;
				}
				NetworkSystem.Instance.ReturnToSinglePlayer();
				return;
			}
			else
			{
				if (CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger() != null)
				{
					GorillaComputer.instance.allowedMapsToJoin = CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger().myCollider.myAllowedMapsToJoin;
					Debug.Log(string.Format("[CustomMapManager::FinalizeExit] allowedMaps: {0}", GorillaComputer.instance.allowedMapsToJoin));
					CustomMapManager.waitingForRoomJoin = true;
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger(), JoinType.Solo, null, false);
					return;
				}
				CustomMapManager.EndTeleport(true);
				return;
			}
		}

		// Token: 0x060064A8 RID: 25768 RVA: 0x00205D48 File Offset: 0x00203F48
		private static void OnJoinSpecificRoomResult(NetJoinResult result)
		{
			GTDev.Log<string>("[CustomMapManager::OnJoinSpecificRoomResult] Result: " + result.ToString(), null);
			switch (result)
			{
			case NetJoinResult.Failed_Full:
				CustomMapManager.instance.OnJoinRoomFailed();
				return;
			case NetJoinResult.AlreadyInRoom:
				CustomMapManager.instance.OnJoinedRoom();
				return;
			case NetJoinResult.Failed_Other:
				GTDev.Log<string>("[CustomMapManager::OnJoinSpecificRoomResult] Joining " + CustomMapManager.pendingNewPrivateRoomName + " failed, marking for retry... ", null);
				CustomMapManager.waitingForDisconnect = true;
				CustomMapManager.shouldRetryJoin = true;
				return;
			default:
				return;
			}
		}

		// Token: 0x060064A9 RID: 25769 RVA: 0x00205DC8 File Offset: 0x00203FC8
		private static void OnJoinSpecificRoomResultFailureAllowed(NetJoinResult result)
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			GTDev.Log<string>("[CustomMapManager::OnJoinSpecificRoomResultFailureAllowed] Result: " + result.ToString(), null);
			switch (result)
			{
			case NetJoinResult.Success:
			case NetJoinResult.FallbackCreated:
				return;
			case NetJoinResult.Failed_Full:
			case NetJoinResult.Failed_Other:
				CustomMapManager.instance.OnJoinRoomFailed();
				return;
			case NetJoinResult.AlreadyInRoom:
				CustomMapManager.instance.OnJoinedRoom();
				return;
			default:
				return;
			}
		}

		// Token: 0x060064AA RID: 25770 RVA: 0x00205E30 File Offset: 0x00204030
		public static bool AreAllPlayersInVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return false;
			}
			foreach (VRRig vrrig in VRRigCache.ActiveRigs)
			{
				if (!CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(vrrig.creator.UserId))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060064AB RID: 25771 RVA: 0x00205EA8 File Offset: 0x002040A8
		public static bool IsRemotePlayerInVirtualStump(string playerID)
		{
			return CustomMapManager.hasInstance && !CustomMapManager.instance.virtualStumpPlayerDetector.IsNull() && CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(playerID);
		}

		// Token: 0x060064AC RID: 25772 RVA: 0x00205EE0 File Offset: 0x002040E0
		public static bool IsLocalPlayerInVirtualStump()
		{
			return CustomMapManager.hasInstance && !CustomMapManager.instance.virtualStumpPlayerDetector.IsNull() && !VRRig.LocalRig.IsNull() && CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(VRRig.LocalRig.creator.UserId);
		}

		// Token: 0x060064AD RID: 25773 RVA: 0x00205F40 File Offset: 0x00204140
		private void OnDisconnected()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (GorillaComputer.hasInstance)
			{
				GorillaComputer.instance.IsPlayerInVirtualStump();
			}
			CustomMapManager.ClearRoomMap();
			if (CustomMapManager.waitingForLoginDisconnect)
			{
				CustomMapManager.waitingForLoginDisconnect = false;
				GTDev.Log<string>("[CustomMapManager::OnDisconnected] Attempting auto-login to mod.io...", null);
				CustomMapManager.AttemptAutoLogin();
				return;
			}
			if (CustomMapManager.waitingForDisconnect)
			{
				CustomMapManager.waitingForDisconnect = false;
				if (CustomMapManager.shouldRetryJoin)
				{
					CustomMapManager.shouldRetryJoin = false;
					GTDev.Log<string>("[CustomMapManager::OnDisconnected] Joining " + CustomMapManager.pendingNewPrivateRoomName + " failed previously, retrying once... ", null);
					PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResultFailureAllowed));
					return;
				}
				GTDev.Log<string>("[CustomMapManager::OnDisconnected] Ending teleport...", null);
				CustomMapManager.EndTeleport(true);
			}
		}

		// Token: 0x060064AE RID: 25774 RVA: 0x00205FF4 File Offset: 0x002041F4
		private static async Task AttemptAutoLogin()
		{
			GTDev.Log<string>(string.Format("[CustomMapManager::AttemptAutoLogin] delayed end teleport coroutine == null : {0}", CustomMapManager.delayedJoinCoroutine == null), null);
			if (CustomMapManager.delayedEndTeleportCoroutine != null)
			{
				CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedEndTeleportCoroutine);
			}
			CustomMapManager.delayedEndTeleportCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedEndTeleport());
			Error error = await ModIOManager.Initialize();
			if (error)
			{
				CustomMapManager.OnAutoLoginComplete(error);
			}
			else
			{
				ModIOManager.IsAuthenticated(true);
				CustomMapManager.OnAutoLoginComplete(Error.None);
			}
		}

		// Token: 0x060064AF RID: 25775 RVA: 0x0020602F File Offset: 0x0020422F
		private void OnJoinRoomFailed()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.waitingForRoomJoin)
			{
				GTDev.Log<string>("[CustomMapManager::OnJoinRoomFailed] Currently waiting for room join, resetting state, ending teleport...", null);
				CustomMapManager.waitingForRoomJoin = false;
				CustomMapManager.EndTeleport(false);
			}
		}

		// Token: 0x060064B0 RID: 25776 RVA: 0x00206058 File Offset: 0x00204258
		private static void EndTeleport(bool teleportSuccessful)
		{
			if (CustomMapManager.hasInstance)
			{
				if (CustomMapManager.delayedEndTeleportCoroutine != null)
				{
					CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedEndTeleportCoroutine);
					CustomMapManager.delayedEndTeleportCoroutine = null;
				}
				if (CustomMapManager.delayedJoinCoroutine != null)
				{
					CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedJoinCoroutine);
					CustomMapManager.delayedJoinCoroutine = null;
				}
			}
			CustomMapManager.DisableTeleportHUD();
			GorillaTagger.Instance.overrideNotInFocus = false;
			PrivateUIRoom.StopForcedOverlay(PrivateUIRoom.OverlaySource.CustomMap);
			Action<bool> action = CustomMapManager.currentTeleportCallback;
			if (action != null)
			{
				action(teleportSuccessful);
			}
			CustomMapManager.currentTeleportCallback = null;
			if (CustomMapManager.hasInstance && !GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				GTDev.Log<string>("[CustomMapManager::EndTeleport] Player is not in VStump, disabling VStump_Lobby GameObject", null);
				CustomMapManager.instance.gameObject.SetActive(false);
			}
			if (teleportSuccessful && GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				CustomMapManager.TryAutoLoadMap();
			}
		}

		// Token: 0x060064B1 RID: 25777 RVA: 0x00206120 File Offset: 0x00204320
		private static void TryAutoLoadMap()
		{
			ModId effectiveAutoLoadModId = CustomMapManager.GetEffectiveAutoLoadModId();
			if (effectiveAutoLoadModId == ModId.Null)
			{
				return;
			}
			bool flag = false;
			if (CustomMapManager.waitingForRoomJoin)
			{
				GTDev.Log<string>("[CustomMapManager::TryAutoLoadMap] Still waiting for room join, delaying auto-load...", null);
				flag = true;
			}
			else if (NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.IsMasterClient && VirtualStumpSerializer.IsWaitingForRoomInit())
			{
				GTDev.Log<string>("[CustomMapManager::TryAutoLoadMap] Still waiting for room init, delaying auto-load...", null);
				flag = true;
			}
			if (flag)
			{
				CustomMapManager.delayedTryAutoLoadCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedTryAutoLoad());
				return;
			}
			GTDev.Log<string>("[CustomMapManager::TryAutoLoadMap] Attempting auto-load...", null);
			GTMapLoadSource autoLoadSource = CustomMapManager.GetAutoLoadSource();
			if (!NetworkSystem.Instance.InRoom || (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient))
			{
				CustomMapManager.SetRoomMap(effectiveAutoLoadModId);
				CustomMapManager.LoadMap(effectiveAutoLoadModId, autoLoadSource);
				return;
			}
			if (CustomMapManager.GetRoomMapId() == effectiveAutoLoadModId)
			{
				CustomMapManager.LoadMap(effectiveAutoLoadModId, autoLoadSource);
			}
		}

		// Token: 0x060064B2 RID: 25778 RVA: 0x002061FD File Offset: 0x002043FD
		private static GTMapLoadSource GetAutoLoadSource()
		{
			if (!CustomMapManager.activateHasAutoLoadOverride)
			{
				return GTMapLoadSource.teleporter;
			}
			return GTMapLoadSource.featured_hallway;
		}

		// Token: 0x060064B3 RID: 25779 RVA: 0x00206209 File Offset: 0x00204409
		private static IEnumerator DelayedEndTeleport()
		{
			yield return new WaitForSecondsRealtime(CustomMapManager.instance.maxPostTeleportRoomProcessingTime);
			GTDev.Log<string>("[CustomMapManager::DelayedEndTeleport] Timer expired, force ending teleport...", null);
			CustomMapManager.EndTeleport(false);
			yield break;
		}

		// Token: 0x060064B4 RID: 25780 RVA: 0x00206211 File Offset: 0x00204411
		private static IEnumerator DelayedTryAutoLoad()
		{
			while (CustomMapManager.waitingForRoomJoin || VirtualStumpSerializer.IsWaitingForRoomInit())
			{
				yield return new WaitForSeconds(0.1f);
			}
			GTDev.Log<string>("[CustomMapManager::DelayedTryAutoLoad] Room Init finished, attempting auto-load...", null);
			ModId effectiveAutoLoadModId = CustomMapManager.GetEffectiveAutoLoadModId();
			GTMapLoadSource autoLoadSource = CustomMapManager.GetAutoLoadSource();
			if (!NetworkSystem.Instance.InRoom || (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient))
			{
				CustomMapManager.SetRoomMap(effectiveAutoLoadModId);
				CustomMapManager.LoadMap(effectiveAutoLoadModId, autoLoadSource);
			}
			else if (CustomMapManager.GetRoomMapId() == effectiveAutoLoadModId)
			{
				CustomMapManager.LoadMap(effectiveAutoLoadModId, autoLoadSource);
			}
			yield break;
		}

		// Token: 0x060064B5 RID: 25781 RVA: 0x0020621C File Offset: 0x0020441C
		private void OnJoinedRoom()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.pendingRoomChangeReloadModId != ModId.Null)
			{
				ModId modId = CustomMapManager.pendingRoomChangeReloadModId;
				CustomMapManager.pendingRoomChangeReloadModId = ModId.Null;
				if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
				{
					CustomMapManager.SetRoomMap(modId);
					CustomMapManager.LoadMap(modId, GTMapLoadSource.room_reload);
				}
			}
			if (CustomMapManager.waitingForRoomJoin)
			{
				CustomMapManager.waitingForRoomJoin = false;
				GTDev.Log<string>("[CustomMapManager::OnJoinedRoom] Ending teleport...", null);
				CustomMapManager.EndTeleport(true);
				if (CustomMapManager.lastUsedTeleporter.IsNotNull())
				{
					CustomMapManager.lastUsedTeleporter.PlayTeleportEffects(true, false, null, true);
				}
			}
		}

		// Token: 0x060064B6 RID: 25782 RVA: 0x002062B8 File Offset: 0x002044B8
		public static bool UnloadMap(bool returnToSinglePlayerIfInPublic = true)
		{
			if (CustomMapManager.unloadInProgress)
			{
				return false;
			}
			if (!CustomMapLoader.IsMapLoaded() && !CustomMapLoader.IsLoading())
			{
				if (CustomMapManager.loadInProgress)
				{
					GTDev.Log<string>("[CustomMapManager::UnloadMap] Map load is currently in progress... aborting...", null);
					CustomMapManager.abortModLoadIds.AddIfNew(CustomMapManager.loadingMapId);
					bool flag = CustomMapManager.waitingForModDownload;
					CustomMapManager.loadInProgress = false;
					CustomMapManager.loadingMapId = ModId.Null;
					CustomMapManager.waitingForModDownload = false;
					CustomMapManager.waitingForModInstall = false;
					CustomMapManager.waitingForModInstallId = ModId.Null;
					CustomMapManager.ClearRoomMap();
				}
				else
				{
					CustomMapManager.ClearRoomMap();
				}
				return false;
			}
			CustomMapManager.unloadInProgress = true;
			CustomMapManager.unloadingMapId = new ModId(CustomMapLoader.IsMapLoaded() ? CustomMapLoader.LoadedMapModId : CustomMapLoader.GetLoadingMapModId());
			CustomMapManager.OnMapLoadProgress(MapLoadStatus.Unloading, 0, "");
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.waitingForModDownload = false;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			CustomMapManager.ClearRoomMap();
			CustomGameMode.LuaScript = "";
			if (CustomGameMode.gameScriptRunner != null)
			{
				CustomGameMode.StopScript();
			}
			CustomMapManager.customMapDefaultZoneShaderSettingsInitialized = false;
			CustomMapManager.customMapDefaultZoneShaderProperties = default(CMSZoneShaderSettings.CMSZoneShaderProperties);
			CustomMapManager.loadedCustomMapDefaultZoneShaderSettings = null;
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.CopySettings(CustomMapManager.instance.virtualStumpZoneShaderSettings, false);
				CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(false);
				CustomMapManager.allCustomMapZoneShaderSettings.Clear();
			}
			CustomMapLoader.CloseDoorAndUnloadMap(new Action(CustomMapManager.OnMapUnloadCompleted));
			if (returnToSinglePlayerIfInPublic && NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
			{
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			return true;
		}

		// Token: 0x060064B7 RID: 25783 RVA: 0x0020643C File Offset: 0x0020463C
		private static void OnMapUnloadCompleted()
		{
			CustomMapManager.unloadInProgress = false;
			CustomMapManager.currentLoadStatus = MapLoadStatus.None;
			CustomMapManager.currentLoadProgress = 0;
			CustomMapManager.currentLoadMessage = "";
			CustomMapManager.ResetModFileProgressTracking();
			CustomMapManager.OnMapUnloadComplete.Invoke();
			CustomMapManager.currentRoomMapModId = ModId.Null;
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(ModId.Null);
			if (CustomMapManager.exitVirtualStumpPending)
			{
				CustomMapManager.FinalizeExitVirtualStump();
			}
		}

		// Token: 0x060064B8 RID: 25784 RVA: 0x002064A0 File Offset: 0x002046A0
		public static async Task LoadMap(ModId modId, GTMapLoadSource loadSource = GTMapLoadSource.none)
		{
			if (CustomMapManager.hasInstance && !CustomMapManager.loadInProgress)
			{
				if (CustomMapManager.IsFeaturedMapLocked() && modId != CustomMapManager.FeaturedLockedMapId)
				{
					GTDev.LogWarning<string>(string.Format("[CustomMapManager::LoadMap] Blocked map change to {0} - Featured lobby ", modId) + string.Format("is locked to {0}.", CustomMapManager.FeaturedLockedMapId), null);
				}
				else
				{
					if (CustomMapManager.abortModLoadIds.Contains(modId))
					{
						CustomMapManager.abortModLoadIds.Remove(modId);
					}
					if (!CustomMapLoader.IsMapLoaded(modId))
					{
						CustomMapManager.loadInProgress = true;
						CustomMapManager.loadingMapId = modId;
						CustomMapManager.pendingMapLoadSource = loadSource;
						CustomMapManager.waitingForModDownload = false;
						CustomMapManager.waitingForModInstall = false;
						CustomMapManager.waitingForModInstallId = ModId.Null;
						CustomMapManager.ResetModFileProgressTracking();
						CustomMapManager.BroadcastMapLoadProgress(MapLoadStatus.Loading, 0, "PREPARING MAP");
						Error error = Error.None;
						ValueTuple<Error, Mod> valueTuple = await ModIOManager.GetMod(modId, false, null);
						error = valueTuple.Item1;
						Mod item = valueTuple.Item2;
						if (error)
						{
							Debug.LogError("[CustomMapManager::LoadMap] Failed to get details for Mod with modID " + modId.ToString() + ", error: " + error.GetMessage());
							CustomMapManager.HandleMapLoadFailed("FAILED TO GET MAP DETAILS: " + error.GetMessage());
						}
						else if (item.Creator == null)
						{
							CustomMapManager.loadInProgress = false;
							CustomMapManager.loadingMapId = ModId.Null;
						}
						else if (UGCPermissionManager.FeaturedMapsOnly && !ModIOManager.IsFeaturedMap(item))
						{
							GTDev.Log<string>("[CustomMapManager::LoadMap] Blocked loading non-featured map " + modId.ToString() + " ", null);
							CustomMapManager.HandleMapLoadFailed("THIS MAP IS NOT AVAILABLE FOR YOUR ACCOUNT");
						}
						else if (CustomMapManager.abortModLoadIds.Contains(modId))
						{
							GTDev.Log<string>("[CustomMapManager::LoadMap] Aborting load...", null);
							CustomMapManager.abortModLoadIds.Remove(modId);
						}
						else if (item.File != null)
						{
							switch (item.File.State)
							{
							case ModFileState.None:
							case ModFileState.Queued:
							{
								GTDev.Log<string>(string.Format("[CustomMapManager::LoadMap] Downloading mod {0}...", modId), null);
								CustomMapManager.waitingForModDownload = true;
								CustomMapManager.waitingForModInstall = true;
								CustomMapManager.waitingForModInstallId = item.Id;
								CustomMapManager.BroadcastMapLoadProgress(MapLoadStatus.Downloading, 0, "WAITING FOR DOWNLOAD");
								bool flag = await ModIOManager.DownloadMod(modId, null);
								if (CustomMapManager.abortModLoadIds.Contains(modId))
								{
									GTDev.Log<string>("[CustomMapManager::LoadMap] Aborting load...", null);
									CustomMapManager.abortModLoadIds.Remove(modId);
								}
								else if (!flag)
								{
									CustomMapManager.HandleMapLoadFailed("FAILED TO START MAP DOWNLOAD");
								}
								break;
							}
							case ModFileState.Downloading:
							case ModFileState.Updating:
								CustomMapManager.waitingForModDownload = true;
								CustomMapManager.waitingForModInstallId = modId;
								CustomMapManager.BroadcastModFileState(item);
								break;
							case ModFileState.Downloaded:
							case ModFileState.Installing:
								CustomMapManager.waitingForModInstall = true;
								CustomMapManager.waitingForModInstallId = modId;
								CustomMapManager.BroadcastModFileState(item);
								break;
							case ModFileState.Installed:
								CustomMapManager.instance.LoadInstalledMap(item);
								break;
							case ModFileState.Uninstalling:
							case ModFileState.FileOperationFailed:
								Debug.LogError("[CustomMapManager::LoadMap] Failed to load map with modID " + modId.ToString() + ", error: " + item.File.State.ToString());
								CustomMapManager.HandleMapLoadFailed("FAILED TO LOAD MAP: " + item.File.State.ToString());
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x060064B9 RID: 25785 RVA: 0x002064EC File Offset: 0x002046EC
		private async Task LoadInstalledMap(Mod installedMod)
		{
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			CustomMapTelemetry.SetLoadingMapInfo(installedMod, CustomMapManager.pendingMapLoadSource);
			if (installedMod.File.State != ModFileState.Installed)
			{
				Debug.LogError("[CustomMapManager::LoadInstalledMap] Requested map is not installed!");
				CustomMapManager.HandleMapLoadFailed("MAP IS NOT INSTALLED");
			}
			else
			{
				if (ModIOManager.ValidateInstalledMod(installedMod) && !string.IsNullOrEmpty(installedMod.File.InstallLocation))
				{
					try
					{
						FileInfo[] files = new DirectoryInfo(installedMod.File.InstallLocation).GetFiles("package.json");
						if (files.Length == 0)
						{
							Debug.LogError(string.Concat(new string[]
							{
								"[CustomMapManager::LoadInstalledMap] Directory (",
								installedMod.File.InstallLocation,
								") for mod ",
								installedMod.Name,
								" does not contain a package.json file!"
							}));
							CustomMapManager.HandleMapLoadFailed("COULD NOT FIND PACKAGE.JSON IN MAP FILES");
							return;
						}
						GTDev.Log<string>("[CustomMapManager::LoadInstalledMap] Loading map file: " + files[0].FullName, null);
						CustomMapLoader.LoadMap(installedMod.Id, files[0].FullName);
						goto IL_0240;
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Format("[CustomMapManager::LoadInstalledMap] Failed to load installed map: {0}", ex));
						CustomMapManager.HandleMapLoadFailed(string.Format("FAILED TO LOAD: {0}", ex));
						goto IL_0240;
					}
				}
				CustomMapManager.waitingForModDownload = true;
				CustomMapManager.waitingForModInstall = true;
				CustomMapManager.waitingForModInstallId = installedMod.Id;
				CustomMapManager.ResetModFileProgressTracking();
				CustomMapManager.BroadcastMapLoadProgress(MapLoadStatus.Downloading, 0, "WAITING FOR DOWNLOAD");
				bool flag = await ModIOManager.DownloadMod(installedMod.Id, null);
				if (CustomMapManager.abortModLoadIds.Contains(installedMod.Id))
				{
					GTDev.Log<string>("[CustomMapManager::LoadInstalledMap] Aborting load...", null);
					CustomMapManager.abortModLoadIds.Remove(installedMod.Id);
				}
				else if (!flag)
				{
					CustomMapManager.HandleMapLoadFailed("FAILED TO START MAP DOWNLOAD");
				}
				IL_0240:;
			}
		}

		// Token: 0x060064BA RID: 25786 RVA: 0x0020652F File Offset: 0x0020472F
		private static void OnMapLoadProgress(MapLoadStatus loadStatus, int progress, string message)
		{
			CustomMapManager.BroadcastMapLoadProgress(loadStatus, progress, message);
		}

		// Token: 0x060064BB RID: 25787 RVA: 0x00206539 File Offset: 0x00204739
		private static void BroadcastMapLoadProgress(MapLoadStatus loadStatus, int progress, string message)
		{
			CustomMapManager.currentLoadStatus = loadStatus;
			CustomMapManager.currentLoadProgress = progress;
			CustomMapManager.currentLoadMessage = message ?? "";
			CustomMapManager.OnMapLoadStatusChanged.Invoke(loadStatus, progress, message);
		}

		// Token: 0x060064BC RID: 25788 RVA: 0x00206564 File Offset: 0x00204764
		private static void OnMapLoadFinished(bool success)
		{
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.waitingForModDownload = false;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			CustomMapManager.currentLoadStatus = MapLoadStatus.None;
			CustomMapManager.currentLoadProgress = 0;
			CustomMapManager.currentLoadMessage = "";
			CustomMapManager.ResetModFileProgressTracking();
			if (success)
			{
				CustomMapTelemetry.OnMapLoadCompleted();
				CustomMapLoader.OpenDoorToMap();
				if (!CustomMapLoader.GetLuauGamemodeScript().IsNullOrEmpty())
				{
					CustomGameMode.LuaScript = CustomMapLoader.GetLuauGamemodeScript();
					if (CustomGameMode.LuaScript != "" && CustomGameMode.GameModeInitialized && CustomGameMode.gameScriptRunner == null)
					{
						CustomGameMode.LuaStart();
					}
				}
			}
			CustomMapManager.OnMapLoadComplete.Invoke(success);
		}

		// Token: 0x060064BD RID: 25789 RVA: 0x00206608 File Offset: 0x00204808
		private static void HandleMapLoadFailed(string message = null)
		{
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			CustomMapManager.pendingMapLoadSource = GTMapLoadSource.none;
			CustomMapTelemetry.ClearLoadingMapInfo();
			CustomMapManager.ResetModFileProgressTracking();
			CustomMapManager.BroadcastMapLoadProgress(MapLoadStatus.Error, 0, message ?? "UNKNOWN ERROR");
			CustomMapManager.OnMapLoadComplete.Invoke(false);
		}

		// Token: 0x060064BE RID: 25790 RVA: 0x00206661 File Offset: 0x00204861
		public static bool IsUnloading()
		{
			return CustomMapManager.unloadInProgress;
		}

		// Token: 0x060064BF RID: 25791 RVA: 0x00206668 File Offset: 0x00204868
		public static bool IsLoading()
		{
			return CustomMapManager.IsLoading(ModId.Null);
		}

		// Token: 0x060064C0 RID: 25792 RVA: 0x00206674 File Offset: 0x00204874
		public static bool IsLoading(ModId modId)
		{
			if (!modId.IsValid())
			{
				return CustomMapManager.loadInProgress || CustomMapLoader.IsLoading();
			}
			return CustomMapManager.loadInProgress && CustomMapManager.loadingMapId == modId;
		}

		// Token: 0x060064C1 RID: 25793 RVA: 0x002066A4 File Offset: 0x002048A4
		public static ModId GetRoomMapId()
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (CustomMapManager.currentRoomMapModId == ModId.Null && NetworkSystem.Instance.IsMasterClient && CustomMapLoader.IsMapLoaded())
				{
					CustomMapManager.currentRoomMapModId = new ModId(CustomMapLoader.LoadedMapModId);
				}
				return CustomMapManager.currentRoomMapModId;
			}
			if (CustomMapManager.IsLoading())
			{
				return CustomMapManager.loadingMapId;
			}
			if (CustomMapLoader.IsMapLoaded())
			{
				return new ModId(CustomMapLoader.LoadedMapModId);
			}
			return ModId.Null;
		}

		// Token: 0x060064C2 RID: 25794 RVA: 0x00206724 File Offset: 0x00204924
		public static void SetRoomMap(long modId)
		{
			if (!CustomMapManager.hasInstance || modId == CustomMapManager.currentRoomMapModId._id)
			{
				return;
			}
			if (CustomMapManager.IsFeaturedMapLocked() && modId != CustomMapManager.FeaturedLockedMapId._id)
			{
				GTDev.LogWarning<string>(string.Format("[CustomMapManager::SetRoomMap] Blocked room-map change to {0} - Featured ", modId) + string.Format("lobby is locked to {0}.", CustomMapManager.FeaturedLockedMapId), null);
				return;
			}
			CustomMapManager.currentRoomMapModId = new ModId(modId);
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(CustomMapManager.currentRoomMapModId);
		}

		// Token: 0x060064C3 RID: 25795 RVA: 0x002067AC File Offset: 0x002049AC
		public static void ClearRoomMap()
		{
			if (!CustomMapManager.hasInstance || CustomMapManager.currentRoomMapModId.Equals(ModId.Null))
			{
				return;
			}
			if (CustomMapManager.IsFeaturedMapLocked())
			{
				return;
			}
			CustomMapManager.currentRoomMapModId = ModId.Null;
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(ModId.Null);
		}

		// Token: 0x060064C4 RID: 25796 RVA: 0x00206804 File Offset: 0x00204A04
		public static bool CanLoadRoomMap()
		{
			return CustomMapManager.currentRoomMapModId != ModId.Null;
		}

		// Token: 0x060064C5 RID: 25797 RVA: 0x0020681A File Offset: 0x00204A1A
		public static void ApproveAndLoadRoomMap()
		{
			CustomMapManager.currentRoomMapApproved = true;
			CMSSerializer.ResetSyncedMapObjects();
			CustomMapManager.LoadMap(CustomMapManager.currentRoomMapModId, GTMapLoadSource.room_sync);
		}

		// Token: 0x060064C6 RID: 25798 RVA: 0x00206833 File Offset: 0x00204A33
		public static void RequestEnableTeleportHUD(bool enteringVirtualStump)
		{
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.EnableTeleportHUD(enteringVirtualStump);
			}
		}

		// Token: 0x060064C7 RID: 25799 RVA: 0x0020684C File Offset: 0x00204A4C
		private void EnableTeleportHUD(bool enteringVirtualStump)
		{
			if (CustomMapManager.teleportingHUD != null)
			{
				CustomMapManager.teleportingHUD.gameObject.SetActive(true);
				CustomMapManager.teleportingHUD.Initialize(enteringVirtualStump);
				return;
			}
			if (this.teleportingHUDPrefab != null)
			{
				Camera main = Camera.main;
				if (main != null)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.teleportingHUDPrefab, main.transform);
					if (gameObject != null)
					{
						CustomMapManager.teleportingHUD = gameObject.GetComponent<VirtualStumpTeleportingHUD>();
						if (CustomMapManager.teleportingHUD != null)
						{
							CustomMapManager.teleportingHUD.Initialize(enteringVirtualStump);
						}
					}
				}
			}
		}

		// Token: 0x060064C8 RID: 25800 RVA: 0x002068DD File Offset: 0x00204ADD
		public static void DisableTeleportHUD()
		{
			if (CustomMapManager.teleportingHUD != null)
			{
				CustomMapManager.teleportingHUD.gameObject.SetActive(false);
			}
		}

		// Token: 0x060064C9 RID: 25801 RVA: 0x002068FC File Offset: 0x00204AFC
		public static void LoadZoneTriggered(int[] scenesToLoad, int[] scenesToUnload)
		{
			CustomMapLoader.LoadZoneTriggered(scenesToLoad, scenesToUnload, new Action<string>(CustomMapManager.OnSceneLoaded), new Action<string>(CustomMapManager.OnSceneUnloaded));
		}

		// Token: 0x060064CA RID: 25802 RVA: 0x0020691D File Offset: 0x00204B1D
		private static void OnSceneLoaded(string sceneName)
		{
			CMSSerializer.ProcessSceneLoad(sceneName);
			CustomMapManager.ProcessZoneShaderSettings(sceneName);
		}

		// Token: 0x060064CB RID: 25803 RVA: 0x0020692C File Offset: 0x00204B2C
		private static void OnSceneUnloaded(string sceneName)
		{
			CMSSerializer.UnregisterTriggers(sceneName);
			for (int i = CustomMapManager.allCustomMapZoneShaderSettings.Count - 1; i >= 0; i--)
			{
				if (CustomMapManager.allCustomMapZoneShaderSettings[i].IsNull())
				{
					CustomMapManager.allCustomMapZoneShaderSettings.RemoveAt(i);
				}
			}
		}

		// Token: 0x060064CC RID: 25804 RVA: 0x00206974 File Offset: 0x00204B74
		private static void OnSceneTriggerHistoryProcessed(string sceneName)
		{
			CapsuleCollider bodyCollider = GTPlayer.Instance.bodyCollider;
			SphereCollider headCollider = GTPlayer.Instance.headCollider;
			Vector3 vector = bodyCollider.transform.TransformPoint(bodyCollider.center);
			float num = Mathf.Max(bodyCollider.height, bodyCollider.radius) * GTPlayer.Instance.scale;
			Collider[] array = new Collider[100];
			Physics.OverlapSphereNonAlloc(vector, num, array);
			foreach (Collider collider in array)
			{
				if (collider != null && collider.gameObject.scene.name.Equals(sceneName))
				{
					CMSTrigger[] components = collider.gameObject.GetComponents<CMSTrigger>();
					for (int j = 0; j < components.Length; j++)
					{
						if (components[j] != null)
						{
							components[j].OnTriggerEnter(bodyCollider);
							components[j].OnTriggerEnter(headCollider);
						}
					}
					CMSLoadingZone[] components2 = collider.gameObject.GetComponents<CMSLoadingZone>();
					for (int k = 0; k < components2.Length; k++)
					{
						if (components2[k] != null)
						{
							components2[k].OnTriggerEnter(bodyCollider);
						}
					}
					CMSZoneShaderSettingsTrigger[] components3 = collider.gameObject.GetComponents<CMSZoneShaderSettingsTrigger>();
					for (int l = 0; l < components3.Length; l++)
					{
						if (components3[l] != null)
						{
							components3[l].OnTriggerEnter(bodyCollider);
						}
					}
					HoverboardAreaTrigger[] components4 = collider.gameObject.GetComponents<HoverboardAreaTrigger>();
					for (int m = 0; m < components4.Length; m++)
					{
						if (components4[m] != null)
						{
							components4[m].OnTriggerEnter(headCollider);
						}
					}
					WaterVolume[] components5 = collider.gameObject.GetComponents<WaterVolume>();
					for (int n = 0; n < components5.Length; n++)
					{
						if (components5[n] != null)
						{
							components5[n].OnTriggerEnter(bodyCollider);
							components5[n].OnTriggerEnter(headCollider);
						}
					}
				}
			}
		}

		// Token: 0x060064CD RID: 25805 RVA: 0x00206B53 File Offset: 0x00204D53
		public static void SetDefaultZoneShaderSettings(ZoneShaderSettings defaultCustomMapShaderSettings, CMSZoneShaderSettings.CMSZoneShaderProperties defaultZoneShaderProperties)
		{
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.CopySettings(defaultCustomMapShaderSettings, true);
				CustomMapManager.loadedCustomMapDefaultZoneShaderSettings = defaultCustomMapShaderSettings;
				CustomMapManager.customMapDefaultZoneShaderProperties = defaultZoneShaderProperties;
				CustomMapManager.customMapDefaultZoneShaderSettingsInitialized = true;
			}
		}

		// Token: 0x060064CE RID: 25806 RVA: 0x00206B84 File Offset: 0x00204D84
		private static void ProcessZoneShaderSettings(string loadedSceneName)
		{
			if (CustomMapManager.hasInstance && CustomMapManager.customMapDefaultZoneShaderSettingsInitialized && CustomMapManager.customMapDefaultZoneShaderProperties.isInitialized)
			{
				for (int i = 0; i < CustomMapManager.allCustomMapZoneShaderSettings.Count; i++)
				{
					if (CustomMapManager.allCustomMapZoneShaderSettings[i].IsNotNull() && CustomMapManager.allCustomMapZoneShaderSettings[i] != CustomMapManager.loadedCustomMapDefaultZoneShaderSettings && CustomMapManager.allCustomMapZoneShaderSettings[i].gameObject.scene.name.Equals(loadedSceneName))
					{
						CustomMapManager.allCustomMapZoneShaderSettings[i].ReplaceDefaultValues(CustomMapManager.customMapDefaultZoneShaderProperties, true);
					}
				}
				return;
			}
			if (CustomMapManager.hasInstance && CustomMapManager.instance.virtualStumpZoneShaderSettings.IsNotNull())
			{
				for (int j = 0; j < CustomMapManager.allCustomMapZoneShaderSettings.Count; j++)
				{
					if (CustomMapManager.allCustomMapZoneShaderSettings[j].IsNotNull() && CustomMapManager.allCustomMapZoneShaderSettings[j].gameObject.scene.name.Equals(loadedSceneName))
					{
						CustomMapManager.allCustomMapZoneShaderSettings[j].ReplaceDefaultValues(CustomMapManager.instance.virtualStumpZoneShaderSettings, true);
					}
				}
			}
		}

		// Token: 0x060064CF RID: 25807 RVA: 0x00206CAE File Offset: 0x00204EAE
		public static void AddZoneShaderSettings(ZoneShaderSettings zoneShaderSettings)
		{
			CustomMapManager.allCustomMapZoneShaderSettings.AddIfNew(zoneShaderSettings);
		}

		// Token: 0x060064D0 RID: 25808 RVA: 0x00206CBB File Offset: 0x00204EBB
		public static void ActivateDefaultZoneShaderSettings()
		{
			if (CustomMapManager.hasInstance && CustomMapManager.customMapDefaultZoneShaderSettingsInitialized)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.BecomeActiveInstance(true);
				return;
			}
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(true);
			}
		}

		// Token: 0x060064D1 RID: 25809 RVA: 0x00206CF8 File Offset: 0x00204EF8
		public static void ReturnToVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (!GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				return;
			}
			if (CustomMapManager.instance.returnToVirtualStumpTeleportLocation.IsNotNull())
			{
				GTPlayer gtplayer = GTPlayer.Instance;
				if (gtplayer != null)
				{
					CustomMapLoader.ResetToInitialZone(new Action<string>(CustomMapManager.OnSceneLoaded), new Action<string>(CustomMapManager.OnSceneUnloaded));
					gtplayer.TeleportTo(CustomMapManager.instance.returnToVirtualStumpTeleportLocation, true, false);
				}
			}
		}

		// Token: 0x060064D2 RID: 25810 RVA: 0x00206D6F File Offset: 0x00204F6F
		public static bool WantsHoldingHandsDisabled()
		{
			if (GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				if (!CustomMapLoader.IsMapLoaded())
				{
					return true;
				}
				if (CustomMapLoader.LoadedMapWantsHoldingHandsDisabled())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04007345 RID: 29509
		[OnEnterPlay_SetNull]
		private static volatile CustomMapManager instance;

		// Token: 0x04007346 RID: 29510
		[OnEnterPlay_Set(false)]
		private static bool hasInstance = false;

		// Token: 0x04007347 RID: 29511
		[SerializeField]
		private GameObject virtualStumpToggleableRoot;

		// Token: 0x04007348 RID: 29512
		[SerializeField]
		private Transform returnToVirtualStumpTeleportLocation;

		// Token: 0x04007349 RID: 29513
		[SerializeField]
		private List<Transform> virtualStumpTeleportLocations;

		// Token: 0x0400734A RID: 29514
		[SerializeField]
		private GameObject[] rootObjectsToDeactivateAfterTeleport;

		// Token: 0x0400734B RID: 29515
		[Tooltip("Objects visually hidden (renderers only, so their behaviour keeps running) while in a Featured map (A/B), and shown again on exit / when in the Custom lobby.")]
		[SerializeField]
		private List<GameObject> featuredMapDisabledObjects;

		// Token: 0x0400734C RID: 29516
		[SerializeField]
		private GorillaFriendCollider virtualStumpPlayerDetector;

		// Token: 0x0400734D RID: 29517
		[SerializeField]
		private ZoneShaderSettings virtualStumpZoneShaderSettings;

		// Token: 0x0400734E RID: 29518
		[SerializeField]
		private BetterDayNightManager dayNightManager;

		// Token: 0x0400734F RID: 29519
		[SerializeField]
		private GhostReactorManager ghostReactorManager;

		// Token: 0x04007350 RID: 29520
		[SerializeField]
		private GRReviveStation defaultReviveStation;

		// Token: 0x04007351 RID: 29521
		[SerializeField]
		private ZoneShaderSettings customMapDefaultZoneShaderSettings;

		// Token: 0x04007352 RID: 29522
		[SerializeField]
		private GameObject teleportingHUDPrefab;

		// Token: 0x04007353 RID: 29523
		[SerializeField]
		private AudioSource localTeleportSFXSource;

		// Token: 0x04007354 RID: 29524
		[SerializeField]
		private VirtualStumpTeleporter defaultTeleporter;

		// Token: 0x04007355 RID: 29525
		[SerializeField]
		private float maxPostTeleportRoomProcessingTime = 15f;

		// Token: 0x04007356 RID: 29526
		private static VirtualStumpTeleporter lastUsedTeleporter;

		// Token: 0x04007357 RID: 29527
		private static string preVStumpGamemode = "";

		// Token: 0x04007358 RID: 29528
		private static bool activateSkipTeleport;

		// Token: 0x04007359 RID: 29529
		private static bool activateDeferZoneToNode;

		// Token: 0x0400735A RID: 29530
		private static bool activateHasAutoLoadOverride;

		// Token: 0x0400735B RID: 29531
		private static ModId activateAutoLoadModIdOverride = ModId.Null;

		// Token: 0x0400735C RID: 29532
		private static bool activateIsActive;

		// Token: 0x0400735D RID: 29533
		private static VirtualStumpActivateMode activateCurrentMode;

		// Token: 0x0400735E RID: 29534
		private static ModId pendingRoomChangeReloadModId = ModId.Null;

		// Token: 0x0400735F RID: 29535
		private static bool customMapDefaultZoneShaderSettingsInitialized;

		// Token: 0x04007360 RID: 29536
		private static ZoneShaderSettings loadedCustomMapDefaultZoneShaderSettings;

		// Token: 0x04007361 RID: 29537
		private static CMSZoneShaderSettings.CMSZoneShaderProperties customMapDefaultZoneShaderProperties;

		// Token: 0x04007362 RID: 29538
		private static readonly List<ZoneShaderSettings> allCustomMapZoneShaderSettings = new List<ZoneShaderSettings>();

		// Token: 0x04007363 RID: 29539
		private static bool loadInProgress = false;

		// Token: 0x04007364 RID: 29540
		private static ModId loadingMapId = ModId.Null;

		// Token: 0x04007365 RID: 29541
		private static GTMapLoadSource pendingMapLoadSource = GTMapLoadSource.none;

		// Token: 0x04007366 RID: 29542
		private static bool unloadInProgress = false;

		// Token: 0x04007367 RID: 29543
		private static ModId unloadingMapId = ModId.Null;

		// Token: 0x04007368 RID: 29544
		private static List<ModId> abortModLoadIds = new List<ModId>();

		// Token: 0x04007369 RID: 29545
		private static bool waitingForModDownload = false;

		// Token: 0x0400736A RID: 29546
		private static bool waitingForModInstall = false;

		// Token: 0x0400736B RID: 29547
		private static ModId waitingForModInstallId = ModId.Null;

		// Token: 0x0400736C RID: 29548
		private static MapLoadStatus currentLoadStatus = MapLoadStatus.None;

		// Token: 0x0400736D RID: 29549
		private static int currentLoadProgress = 0;

		// Token: 0x0400736E RID: 29550
		private static string currentLoadMessage = "";

		// Token: 0x0400736F RID: 29551
		private static MapLoadStatus lastBroadcastFileStatus = MapLoadStatus.None;

		// Token: 0x04007370 RID: 29552
		private static int lastBroadcastFilePercent = -1;

		// Token: 0x04007371 RID: 29553
		private static ModId trackedDownloadMapId = ModId.Null;

		// Token: 0x04007372 RID: 29554
		private static bool preTeleportInPrivateRoom = false;

		// Token: 0x04007373 RID: 29555
		private static string pendingNewPrivateRoomName = "";

		// Token: 0x04007374 RID: 29556
		private static Action<bool> currentTeleportCallback;

		// Token: 0x04007375 RID: 29557
		private static bool waitingForLoginDisconnect = false;

		// Token: 0x04007376 RID: 29558
		private static bool waitingForDisconnect = false;

		// Token: 0x04007377 RID: 29559
		private static bool waitingForRoomJoin = false;

		// Token: 0x04007378 RID: 29560
		private static bool shouldRetryJoin = false;

		// Token: 0x04007379 RID: 29561
		private static short pendingTeleportVFXIdx = -1;

		// Token: 0x0400737A RID: 29562
		private static bool exitVirtualStumpPending = false;

		// Token: 0x0400737B RID: 29563
		private static ModId currentRoomMapModId = ModId.Null;

		// Token: 0x0400737C RID: 29564
		private static bool currentRoomMapApproved = false;

		// Token: 0x0400737D RID: 29565
		private static VirtualStumpTeleportingHUD teleportingHUD;

		// Token: 0x0400737E RID: 29566
		private static Coroutine delayedEndTeleportCoroutine;

		// Token: 0x0400737F RID: 29567
		private static Coroutine delayedJoinCoroutine;

		// Token: 0x04007380 RID: 29568
		private static Coroutine delayedTryAutoLoadCoroutine;

		// Token: 0x04007381 RID: 29569
		public static UnityEvent<ModId> OnRoomMapChanged = new UnityEvent<ModId>();

		// Token: 0x04007382 RID: 29570
		public static UnityEvent<MapLoadStatus, int, string> OnMapLoadStatusChanged = new UnityEvent<MapLoadStatus, int, string>();

		// Token: 0x04007383 RID: 29571
		public static UnityEvent<bool> OnMapLoadComplete = new UnityEvent<bool>();

		// Token: 0x04007384 RID: 29572
		public static UnityEvent OnMapUnloadComplete = new UnityEvent();

		// Token: 0x04007385 RID: 29573
		private const ModChangeType ModFileProgressChanges = ModChangeType.DownloadProgress | ModChangeType.FileState;

		// Token: 0x04007386 RID: 29574
		private const string PreparingMessage = "PREPARING MAP";

		// Token: 0x04007387 RID: 29575
		private const string DownloadQueuedMessage = "WAITING FOR DOWNLOAD";

		// Token: 0x04007388 RID: 29576
		private const string DownloadingMessage = "DOWNLOADING MAP FILES";

		// Token: 0x04007389 RID: 29577
		private const string InstallQueuedMessage = "WAITING TO INSTALL";

		// Token: 0x0400738A RID: 29578
		private const string InstallingMessage = "INSTALLING MAP FILES";
	}
}
