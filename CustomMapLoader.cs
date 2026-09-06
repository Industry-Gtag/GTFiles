using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using CosmeticRoom;
using CustomMapSupport;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion.Swimming;
using GorillaNetworking;
using GorillaNetworking.Store;
using GorillaTag.Gravity;
using GorillaTag.Rendering;
using GorillaTagScripts;
using GorillaTagScripts.CustomMapSupport;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using Modio;
using Modio.Mods;
using Newtonsoft.Json;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;

// Token: 0x02000A55 RID: 2645
public class CustomMapLoader : MonoBehaviour, IBuildValidation
{
	// Token: 0x060043DE RID: 17374 RVA: 0x001691C3 File Offset: 0x001673C3
	internal static void SetZoneDynamicLighting(bool enable)
	{
		if (enable && !CustomMapLoader.usingDynamicLighting)
		{
			GameLightingManager.instance.ZoneEnableCustomDynamicLighting(true);
			CustomMapLoader.usingDynamicLighting = true;
			return;
		}
		if (!enable && CustomMapLoader.usingDynamicLighting)
		{
			GameLightingManager.instance.ZoneEnableCustomDynamicLighting(false);
			CustomMapLoader.usingDynamicLighting = false;
		}
	}

	// Token: 0x060043DF RID: 17375 RVA: 0x00169200 File Offset: 0x00167400
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitOnLoad()
	{
		GTDev.Log<string>("CML::InitOnLoad", null);
		CustomMapLoader.instance = null;
		CustomMapLoader.hasInstance = false;
		CustomMapLoader.isLoading = false;
		CustomMapLoader.isUnloading = false;
		CustomMapLoader.runningAsyncLoad = false;
		CustomMapLoader.attemptedLoadID = 0L;
		CustomMapLoader.attemptedSceneToLoad = null;
		CustomMapLoader.shouldAbortMapLoading = false;
		CustomMapLoader.shouldAbortSceneLoad = false;
		CustomMapLoader.errorEncounteredDuringLoad = false;
		CustomMapLoader.unloadMapCallback = null;
		CustomMapLoader.cachedExceptionMessage = "";
		CustomMapLoader.mapBundle = null;
		CustomMapLoader.initialSceneNames = new List<string>();
		CustomMapLoader.initialSceneIndexes = new List<int>();
		CustomMapLoader.maxPlayersForMap = 20;
		CustomMapLoader.loadedMapModId = ModId.Null;
		CustomMapLoader.loadedMapModFileId = -1L;
		CustomMapLoader.loadedMapPackageInfo = null;
		CustomMapLoader.cachedLuauScript = null;
		CustomMapLoader.devModeEnabled = false;
		CustomMapLoader.disableHoldingHandsAllModes = false;
		CustomMapLoader.disableHoldingHandsCustomMode = false;
		CustomMapLoader.mapLoadProgressCallback = null;
		CustomMapLoader.mapLoadFinishedCallback = null;
		CustomMapLoader.zoneLoadingCoroutine = null;
		CustomMapLoader.sceneLoadedCallback = null;
		CustomMapLoader.sceneUnloadedCallback = null;
		CustomMapLoader.queuedLoadZoneRequests = new List<CustomMapLoader.LoadZoneRequest>();
		CustomMapLoader.assetBundleSceneFilePaths = null;
		CustomMapLoader.loadedSceneFilePaths = new List<string>();
		CustomMapLoader.loadedSceneNames = new List<string>();
		CustomMapLoader.loadedSceneIndexes = new List<int>();
		CustomMapLoader.leafGliderIndex = 0;
		CustomMapLoader.usingDynamicLighting = false;
		CustomMapLoader.totalObjectsInLoadingScene = 0;
		CustomMapLoader.objectsProcessedForLoadingScene = 0;
		CustomMapLoader.objectsProcessedThisFrame = 0;
		CustomMapLoader.initializePhaseTwoComponents = new List<Component>();
		CustomMapLoader.entitiesToCreate = new List<MapEntity>(Constants.aiAgentLimit);
		CustomMapLoader.lightmaps = null;
		CustomMapLoader.lightmapsToKeep = new List<Texture2D>();
		CustomMapLoader.placeholderReplacements = new List<GameObject>();
		CustomMapLoader.customMapATM = null;
		CustomMapLoader.storeCheckouts = new List<GameObject>();
		CustomMapLoader.storeDisplayStands = new List<GameObject>();
		CustomMapLoader.storeTryOnConsoles = new List<GameObject>();
		CustomMapLoader.storeTryOnAreas = new List<GameObject>();
	}

	// Token: 0x060043E0 RID: 17376 RVA: 0x00169382 File Offset: 0x00167582
	private void Awake()
	{
		if (CustomMapLoader.instance == null)
		{
			CustomMapLoader.instance = this;
			CustomMapLoader.hasInstance = true;
			return;
		}
		if (CustomMapLoader.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060043E1 RID: 17377 RVA: 0x001693BC File Offset: 0x001675BC
	private void Start()
	{
		byte[] array = new byte[]
		{
			Convert.ToByte(68),
			Convert.ToByte(111),
			Convert.ToByte(110),
			Convert.ToByte(116),
			Convert.ToByte(68),
			Convert.ToByte(101),
			Convert.ToByte(115),
			Convert.ToByte(116),
			Convert.ToByte(114),
			Convert.ToByte(111),
			Convert.ToByte(121),
			Convert.ToByte(79),
			Convert.ToByte(110),
			Convert.ToByte(76),
			Convert.ToByte(111),
			Convert.ToByte(97),
			Convert.ToByte(100)
		};
		this.dontDestroyOnLoadSceneName = Encoding.ASCII.GetString(array);
		if (this.publicJoinTrigger != null)
		{
			this.publicJoinTrigger.SetActive(false);
		}
	}

	// Token: 0x060043E2 RID: 17378 RVA: 0x001694AE File Offset: 0x001676AE
	public static void Initialize(Action<MapLoadStatus, int, string> onLoadProgress, Action<bool> onLoadFinished, Action<string> onSceneLoaded, Action<string> onSceneUnloaded)
	{
		CustomMapLoader.mapLoadProgressCallback = onLoadProgress;
		CustomMapLoader.mapLoadFinishedCallback = onLoadFinished;
		CustomMapLoader.sceneLoadedCallback = onSceneLoaded;
		CustomMapLoader.sceneUnloadedCallback = onSceneUnloaded;
	}

	// Token: 0x060043E3 RID: 17379 RVA: 0x001694C8 File Offset: 0x001676C8
	public static void LoadMap(long mapModId, string mapFilePath)
	{
		if (!CustomMapLoader.hasInstance)
		{
			return;
		}
		if (CustomMapLoader.isLoading)
		{
			return;
		}
		if (CustomMapLoader.isUnloading)
		{
			Action<bool> action = CustomMapLoader.mapLoadFinishedCallback;
			if (action == null)
			{
				return;
			}
			action(false);
			return;
		}
		else
		{
			if (!CustomMapLoader.IsMapLoaded(mapModId))
			{
				GorillaNetworkJoinTrigger.DisableTriggerJoins();
				CustomMapLoader.CanLoadEntities = false;
				CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadAssetBundle(mapModId, mapFilePath, new Action<bool, bool>(CustomMapLoader.OnAssetBundleLoaded)));
				return;
			}
			Action<bool> action2 = CustomMapLoader.mapLoadFinishedCallback;
			if (action2 == null)
			{
				return;
			}
			action2(true);
			return;
		}
	}

	// Token: 0x060043E4 RID: 17380 RVA: 0x00169546 File Offset: 0x00167746
	public static bool OpenDoorToMap()
	{
		if (!CustomMapLoader.hasInstance)
		{
			return false;
		}
		if (CustomMapLoader.instance.accessDoor != null)
		{
			CustomMapLoader.instance.accessDoor.OpenDoor();
			return true;
		}
		return false;
	}

	// Token: 0x060043E5 RID: 17381 RVA: 0x00169579 File Offset: 0x00167779
	private static IEnumerator LoadAssetBundle(long mapModID, string packageInfoFilePath, Action<bool, bool> OnLoadComplete)
	{
		CustomMapLoader.isLoading = true;
		CustomMapLoader.errorEncounteredDuringLoad = false;
		CustomMapLoader.attemptedLoadID = mapModID;
		CustomMapLoader.refreshReviveStations = false;
		CustomMapLoader.gravityZoneCount = 0;
		CustomMapLoader.sizeChangerCount = 0;
		CustomMapLoader.handHoldCount = 0;
		CustomMapLoader.mapperAssetCount = 0;
		CustomMapLoader.instance.ghostReactorManager.reactor.RefreshReviveStations(false);
		Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
		if (action != null)
		{
			action(MapLoadStatus.Loading, 1, "CACHING LIGHTMAP DATA");
		}
		CustomMapLoader.CacheLightmaps();
		Action<MapLoadStatus, int, string> action2 = CustomMapLoader.mapLoadProgressCallback;
		if (action2 != null)
		{
			action2(MapLoadStatus.Loading, 2, "LOADING PACKAGE INFO");
		}
		try
		{
			CustomMapLoader.loadedMapPackageInfo = CustomMapLoader.GetPackageInfo(packageInfoFilePath);
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("[CML.LoadAssetBundle] GetPackageInfo Exception: {0}", ex));
			Action<MapLoadStatus, int, string> action3 = CustomMapLoader.mapLoadProgressCallback;
			if (action3 != null)
			{
				action3(MapLoadStatus.Error, 0, ex.ToString());
			}
			OnLoadComplete(false, false);
			yield break;
		}
		if (CustomMapLoader.loadedMapPackageInfo == null)
		{
			Action<MapLoadStatus, int, string> action4 = CustomMapLoader.mapLoadProgressCallback;
			if (action4 != null)
			{
				action4(MapLoadStatus.Error, 0, "FAILED TO READ FILE AT " + packageInfoFilePath);
			}
			OnLoadComplete(false, false);
			yield break;
		}
		CustomMapLoader.LoadInitialSceneNames();
		Action<MapLoadStatus, int, string> action5 = CustomMapLoader.mapLoadProgressCallback;
		if (action5 != null)
		{
			action5(MapLoadStatus.Loading, 3, "PACKAGE INFO LOADED");
		}
		string text = Path.GetDirectoryName(packageInfoFilePath) + "/" + CustomMapLoader.loadedMapPackageInfo.pcFileName;
		Action<MapLoadStatus, int, string> action6 = CustomMapLoader.mapLoadProgressCallback;
		if (action6 != null)
		{
			action6(MapLoadStatus.Loading, 4, "LOADING MAP ASSET BUNDLE");
		}
		AssetBundleCreateRequest loadBundleRequest = AssetBundle.LoadFromFileAsync(text);
		yield return loadBundleRequest;
		CustomMapLoader.mapBundle = loadBundleRequest.assetBundle;
		if (CustomMapLoader.shouldAbortMapLoading || CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(-1);
			OnLoadComplete(false, true);
			yield break;
		}
		if (CustomMapLoader.mapBundle == null)
		{
			Action<MapLoadStatus, int, string> action7 = CustomMapLoader.mapLoadProgressCallback;
			if (action7 != null)
			{
				action7(MapLoadStatus.Error, 0, "CUSTOM MAP ASSET BUNDLE FAILED TO LOAD");
			}
			OnLoadComplete(false, false);
			yield break;
		}
		if (!CustomMapLoader.mapBundle.isStreamedSceneAssetBundle)
		{
			CustomMapLoader.mapBundle.Unload(true);
			Action<MapLoadStatus, int, string> action8 = CustomMapLoader.mapLoadProgressCallback;
			if (action8 != null)
			{
				action8(MapLoadStatus.Error, 0, "AssetBundle does not contain a Unity Scene file");
			}
			OnLoadComplete(false, false);
			yield break;
		}
		Action<MapLoadStatus, int, string> action9 = CustomMapLoader.mapLoadProgressCallback;
		if (action9 != null)
		{
			action9(MapLoadStatus.Loading, 10, "MAP ASSET BUNDLE LOADED");
		}
		CustomMapLoader.assetBundleSceneFilePaths = CustomMapLoader.mapBundle.GetAllScenePaths();
		if (CustomMapLoader.assetBundleSceneFilePaths.Length == 0)
		{
			CustomMapLoader.mapBundle.Unload(true);
			Action<MapLoadStatus, int, string> action10 = CustomMapLoader.mapLoadProgressCallback;
			if (action10 != null)
			{
				action10(MapLoadStatus.Error, 0, "AssetBundle does not contain a Unity Scene file");
			}
			OnLoadComplete(false, false);
			yield break;
		}
		foreach (string text2 in CustomMapLoader.assetBundleSceneFilePaths)
		{
			if (text2.Equals(CustomMapLoader.instance.dontDestroyOnLoadSceneName, StringComparison.OrdinalIgnoreCase))
			{
				CustomMapLoader.mapBundle.Unload(true);
				Action<MapLoadStatus, int, string> action11 = CustomMapLoader.mapLoadProgressCallback;
				if (action11 != null)
				{
					action11(MapLoadStatus.Error, 0, "Map name is " + text2 + " this is an invalid name");
				}
				OnLoadComplete(false, false);
				yield break;
			}
		}
		OnLoadComplete(true, false);
		yield break;
	}

	// Token: 0x060043E6 RID: 17382 RVA: 0x00169598 File Offset: 0x00167798
	private static void LoadInitialSceneNames()
	{
		CustomMapLoader.initialSceneNames.Clear();
		if (CustomMapLoader.loadedMapPackageInfo != null)
		{
			if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion <= 2)
			{
				CustomMapLoader.initialSceneNames.Add(CustomMapLoader.loadedMapPackageInfo.initialScene);
				return;
			}
			if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion > 2)
			{
				CustomMapLoader.initialSceneNames.AddRange(CustomMapLoader.loadedMapPackageInfo.initialScenes);
			}
		}
	}

	// Token: 0x060043E7 RID: 17383 RVA: 0x001695FC File Offset: 0x001677FC
	private static void OnAssetBundleLoaded(bool loadSucceeded, bool loadAborted)
	{
		if (loadAborted)
		{
			return;
		}
		if (loadSucceeded)
		{
			CustomMapLoader.loadedMapModId = CustomMapLoader.attemptedLoadID;
			CustomMapLoader.loadedMapModFileId = 0L;
			if (CustomMapLoader.loadedMapModId != 999999999L)
			{
				ModIOManager.GetMod(new ModId(CustomMapLoader.loadedMapModId), false, delegate(Error error, Mod mod)
				{
					if (!error && mod != null && mod.File != null)
					{
						CustomMapLoader.loadedMapModFileId = mod.File.Id;
					}
				});
			}
			foreach (string text in CustomMapLoader.initialSceneNames)
			{
				int num = -1;
				if (text != string.Empty)
				{
					num = CustomMapLoader.GetSceneIndex(text);
				}
				if (num == -1)
				{
					GTDev.LogError<string>("[CustomMapLoader::OnAssetBundleLoaded] Encountered invalid initial scene, could not get scene index for: \"" + text + "\"", null);
				}
				else
				{
					CustomMapLoader.initialSceneIndexes.Add(num);
				}
			}
			if (CustomMapLoader.initialSceneIndexes.Count == 0)
			{
				if (CustomMapLoader.assetBundleSceneFilePaths.Length == 1)
				{
					GTDev.LogWarning<string>("[CustomMapLoader::OnAssetBundleLoaded] Asset Bundle only contains 1 Scene, but it isn't marked as an initial scene. Treating it as an initial scene...", null);
					CustomMapLoader.initialSceneIndexes.Add(0);
				}
				else if (CustomMapLoader.mapBundle != null)
				{
					string text2 = "";
					if (CustomMapLoader.assetBundleSceneFilePaths.Length == 0)
					{
						text2 = "MAP ASSET BUNDLE CONTAINS NO VALID SCENES.";
					}
					else if (CustomMapLoader.assetBundleSceneFilePaths.Length > 1)
					{
						text2 = "MAP ASSET BUNDLE CONTAINS MULTIPLE SCENES, BUT NONE ARE SET AS INITIAL SCENE.";
					}
					Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
					if (action != null)
					{
						action(MapLoadStatus.Error, 0, text2);
					}
					CustomMapLoader.OnInitialLoadComplete(false, true);
				}
			}
			CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadInitialScenesCoroutine(CustomMapLoader.initialSceneIndexes.ToArray()));
		}
	}

	// Token: 0x060043E8 RID: 17384 RVA: 0x00169788 File Offset: 0x00167988
	private static IEnumerator LoadInitialScenesCoroutine(int[] sceneIndexes)
	{
		CustomMapLoader.<>c__DisplayClass107_0 CS$<>8__locals1 = new CustomMapLoader.<>c__DisplayClass107_0();
		CS$<>8__locals1.sceneIndexes = sceneIndexes;
		if (!CustomMapLoader.loadedSceneIndexes.IsNullOrEmpty<int>())
		{
			GTDev.LogError<string>("[CustomMapLoader::LoadInitialScenesCoroutine] loadedSceneIndexes is not empty, LoadInitialScenes should not be called in this case!", null);
			yield break;
		}
		int progressAmountPerScene = 89 / CS$<>8__locals1.sceneIndexes.Length;
		GTDev.Log<string>(string.Format("[CustomMapLoader::LoadInitialScenesCoroutine] loading {0} scenes...", CS$<>8__locals1.sceneIndexes.Length), null);
		CS$<>8__locals1.i = 0;
		while (CS$<>8__locals1.i < CS$<>8__locals1.sceneIndexes.Length)
		{
			CustomMapLoader.<>c__DisplayClass107_1 CS$<>8__locals2 = new CustomMapLoader.<>c__DisplayClass107_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			int num = 10 + CS$<>8__locals2.CS$<>8__locals1.i * progressAmountPerScene;
			int num2 = num + progressAmountPerScene;
			CS$<>8__locals2.isLastScene = CS$<>8__locals2.CS$<>8__locals1.i == CS$<>8__locals2.CS$<>8__locals1.sceneIndexes.Length - 1;
			CS$<>8__locals2.stopLoading = false;
			CS$<>8__locals2.initialLoadAborted = false;
			yield return CustomMapLoader.LoadSceneFromAssetBundle(CS$<>8__locals2.CS$<>8__locals1.sceneIndexes[CS$<>8__locals2.CS$<>8__locals1.i], delegate(bool loadSucceeded, bool loadAborted, string loadedSceneName)
			{
				if (!loadSucceeded || loadAborted)
				{
					GTDev.Log<string>("[CustomMapLoader::LoadInitialScenesCoroutine] failed to load scene at index " + string.Format("\"{0}\", aborting initial load...", CS$<>8__locals2.CS$<>8__locals1.sceneIndexes[CS$<>8__locals2.CS$<>8__locals1.i]), null);
					CS$<>8__locals2.stopLoading = true;
					CS$<>8__locals2.initialLoadAborted = loadAborted;
					return;
				}
				if (CS$<>8__locals2.isLastScene)
				{
					CustomMapLoader.OnInitialLoadComplete(true, false);
				}
			}, true, num, num2);
			if (CS$<>8__locals2.stopLoading || CustomMapLoader.shouldAbortMapLoading)
			{
				CustomMapLoader.OnInitialLoadComplete(false, CS$<>8__locals2.initialLoadAborted);
				break;
			}
			CS$<>8__locals2 = null;
			int i = CS$<>8__locals1.i;
			CS$<>8__locals1.i = i + 1;
		}
		yield break;
	}

	// Token: 0x060043E9 RID: 17385 RVA: 0x00169798 File Offset: 0x00167998
	private static void OnInitialLoadComplete(bool loadSucceeded, bool loadAborted)
	{
		if (loadAborted || !loadSucceeded)
		{
			if (!loadAborted)
			{
				CustomMapLoader.instance.StartCoroutine(CustomMapLoader.AbortMapLoad());
				return;
			}
			Action<bool> action = CustomMapLoader.mapLoadFinishedCallback;
			if (action == null)
			{
				return;
			}
			action(false);
			return;
		}
		else
		{
			if (CustomMapLoader.loadedMapPackageInfo != null && CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion >= 3)
			{
				CustomMapLoader.maxPlayersForMap = (byte)Math.Clamp(CustomMapLoader.loadedMapPackageInfo.maxPlayers, 1, 20);
				if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion >= 5)
				{
					CustomMapModeSelector.SetAvailableGameModes(CustomMapLoader.loadedMapPackageInfo.availableGameModes, CustomMapLoader.loadedMapPackageInfo.defaultGameMode);
					if (RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
					{
						if (GameMode.ActiveGameMode.IsNull())
						{
							GameModeType gameModeType = (GameModeType)CustomMapLoader.loadedMapPackageInfo.defaultGameMode;
							GameMode.ChangeGameMode(gameModeType.ToString());
						}
						else if (GameMode.ActiveGameMode.GameType() != (GameModeType)CustomMapLoader.loadedMapPackageInfo.defaultGameMode)
						{
							GameModeType gameModeType = (GameModeType)CustomMapLoader.loadedMapPackageInfo.defaultGameMode;
							GameMode.ChangeGameMode(gameModeType.ToString());
						}
					}
				}
				else
				{
					List<int> list = new List<int>();
					foreach (GameModeType gameModeType2 in CustomMapLoader.instance.availableModesForOldMaps)
					{
						list.Add((int)gameModeType2);
					}
					GameModeType gameModeType3 = CustomMapLoader.instance.defaultGameModeForNonCustomOldMaps;
					if (!CustomMapLoader.loadedMapPackageInfo.customGamemodeScript.IsNullOrEmpty())
					{
						gameModeType3 = GameModeType.Custom;
						list.Add(7);
					}
					CustomMapModeSelector.SetAvailableGameModes(list.ToArray(), (int)gameModeType3);
					if (RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
					{
						if (GameMode.ActiveGameMode.IsNull())
						{
							GameMode.ChangeGameMode(gameModeType3.ToString());
						}
						else if (GameMode.ActiveGameMode.GameType() != gameModeType3)
						{
							GameMode.ChangeGameMode(gameModeType3.ToString());
						}
					}
				}
				CustomMapLoader.cachedLuauScript = CustomMapLoader.loadedMapPackageInfo.customGamemodeScript;
				CustomMapLoader.devModeEnabled = CustomMapLoader.loadedMapPackageInfo.devMode;
				CustomMapLoader.disableHoldingHandsAllModes = CustomMapLoader.loadedMapPackageInfo.disableHoldingHandsAllModes;
				CustomMapLoader.disableHoldingHandsCustomMode = CustomMapLoader.loadedMapPackageInfo.disableHoldingHandsCustomMode;
				Color color = new Color(CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_R, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_G, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_B, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_A);
				if (CustomMapLoader.loadedMapPackageInfo.useUberShaderDynamicLighting)
				{
					CustomMapLoader.SetZoneDynamicLighting(true);
					GameLightingManager.instance.SetAmbientLightDynamic(color);
				}
				VirtualStumpReturnWatch.SetWatchProperties(CustomMapLoader.loadedMapPackageInfo.GetReturnToVStumpWatchProps());
			}
			CustomMapLoader.isLoading = false;
			CustomMapLoader.CanLoadEntities = true;
			GorillaNetworkJoinTrigger.EnableTriggerJoins();
			Action<MapLoadStatus, int, string> action2 = CustomMapLoader.mapLoadProgressCallback;
			if (action2 != null)
			{
				action2(MapLoadStatus.Loading, 100, "LOAD COMPLETE");
			}
			if (CustomMapLoader.instance.publicJoinTrigger != null)
			{
				CustomMapLoader.instance.publicJoinTrigger.SetActive(true);
			}
			foreach (string text in CustomMapLoader.loadedSceneNames)
			{
				Action<string> action3 = CustomMapLoader.sceneLoadedCallback;
				if (action3 != null)
				{
					action3(text);
				}
			}
			Action<bool> action4 = CustomMapLoader.mapLoadFinishedCallback;
			if (action4 == null)
			{
				return;
			}
			action4(true);
			return;
		}
	}

	// Token: 0x060043EA RID: 17386 RVA: 0x00169AF8 File Offset: 0x00167CF8
	private static IEnumerator LoadScenesCoroutine(int[] sceneIndexes, Action<bool, bool, List<string>> loadCompleteCallback = null)
	{
		CustomMapLoader.<>c__DisplayClass109_0 CS$<>8__locals1 = new CustomMapLoader.<>c__DisplayClass109_0();
		CS$<>8__locals1.loadCompleteCallback = loadCompleteCallback;
		if (sceneIndexes.IsNullOrEmpty<int>())
		{
			Action<bool, bool, List<string>> loadCompleteCallback2 = CS$<>8__locals1.loadCompleteCallback;
			if (loadCompleteCallback2 != null)
			{
				loadCompleteCallback2(false, false, null);
			}
			yield break;
		}
		CustomMapLoader.isLoading = true;
		CS$<>8__locals1.successfullyLoadedSceneNames = new List<string>();
		CS$<>8__locals1.successfullyLoadedAllScenes = true;
		int num;
		for (int i = 0; i < sceneIndexes.Length; i = num + 1)
		{
			CustomMapLoader.<>c__DisplayClass109_1 CS$<>8__locals2 = new CustomMapLoader.<>c__DisplayClass109_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			if (CustomMapLoader.loadedSceneIndexes.Contains(sceneIndexes[i]))
			{
				GTDev.LogWarning<string>("[CustomMapLoader::LoadScenesCoroutine] Cannot load scene " + string.Format("{0}:\"{1}\" because it's already loaded!", sceneIndexes[i], CustomMapLoader.assetBundleSceneFilePaths[sceneIndexes[i]]), null);
			}
			else
			{
				CS$<>8__locals2.shouldAbortLoad = false;
				CS$<>8__locals2.isLastScene = i == sceneIndexes.Length - 1;
				yield return CustomMapLoader.LoadSceneFromAssetBundle(sceneIndexes[i], delegate(bool loadSucceeded, bool loadAborted, string loadedSceneName)
				{
					if (!loadSucceeded || loadAborted)
					{
						CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedAllScenes = false;
					}
					else
					{
						Action<string> action = CustomMapLoader.sceneLoadedCallback;
						if (action != null)
						{
							action(loadedSceneName);
						}
						CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedSceneNames.Add(loadedSceneName);
					}
					if (loadAborted)
					{
						CS$<>8__locals2.shouldAbortLoad = true;
						return;
					}
					if (CS$<>8__locals2.isLastScene)
					{
						Action<bool, bool, List<string>> loadCompleteCallback4 = CS$<>8__locals2.CS$<>8__locals1.loadCompleteCallback;
						if (loadCompleteCallback4 == null)
						{
							return;
						}
						loadCompleteCallback4(CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedAllScenes, false, CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedSceneNames);
					}
				}, false, 10, 90);
				if (CS$<>8__locals2.shouldAbortLoad)
				{
					CustomMapLoader.isLoading = false;
					Action<bool, bool, List<string>> loadCompleteCallback3 = CS$<>8__locals2.CS$<>8__locals1.loadCompleteCallback;
					if (loadCompleteCallback3 == null)
					{
						break;
					}
					loadCompleteCallback3(false, true, CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedSceneNames);
					break;
				}
				else
				{
					CS$<>8__locals2 = null;
				}
			}
			num = i;
		}
		CustomMapLoader.isLoading = false;
		yield break;
	}

	// Token: 0x060043EB RID: 17387 RVA: 0x00169B0E File Offset: 0x00167D0E
	private static IEnumerator LoadSceneFromAssetBundle(int sceneIndex, Action<bool, bool, string> OnLoadComplete, bool useProgressCallback = false, int startingProgress = 10, int endingProgress = 90)
	{
		int progressAmount = endingProgress - startingProgress;
		int currentProgress = startingProgress;
		CustomMapLoader.refreshReviveStations = false;
		LoadSceneParameters loadSceneParameters = new LoadSceneParameters
		{
			loadSceneMode = LoadSceneMode.Additive,
			localPhysicsMode = LocalPhysicsMode.None
		};
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			OnLoadComplete(false, true, "");
			yield break;
		}
		CustomMapLoader.runningAsyncLoad = true;
		if (useProgressCallback)
		{
			int num = startingProgress + Mathf.RoundToInt((float)progressAmount * 0.02f);
			Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
			if (action != null)
			{
				action(MapLoadStatus.Loading, num, "LOADING MAP SCENE");
			}
		}
		CustomMapLoader.attemptedSceneToLoad = CustomMapLoader.assetBundleSceneFilePaths[sceneIndex];
		string sceneName = CustomMapLoader.GetSceneNameFromFilePath(CustomMapLoader.attemptedSceneToLoad);
		ZoneManagement.AddSceneToForceStayLoaded(sceneName);
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(CustomMapLoader.attemptedSceneToLoad, loadSceneParameters);
		yield return asyncOperation;
		CustomMapLoader.runningAsyncLoad = false;
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			OnLoadComplete(false, true, "");
			yield break;
		}
		if (useProgressCallback)
		{
			currentProgress += Mathf.RoundToInt((float)progressAmount * 0.28f);
			Action<MapLoadStatus, int, string> action2 = CustomMapLoader.mapLoadProgressCallback;
			if (action2 != null)
			{
				action2(MapLoadStatus.Loading, currentProgress, "SANITIZING MAP");
			}
		}
		GameObject[] rootGameObjects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
		List<MapDescriptor> list = new List<MapDescriptor>();
		for (int i = 0; i < rootGameObjects.Length; i++)
		{
			MapDescriptor component = rootGameObjects[i].GetComponent<MapDescriptor>();
			if (component.IsNotNull())
			{
				list.Add(component);
			}
		}
		MapDescriptor mapDescriptor = null;
		bool flag = false;
		foreach (MapDescriptor mapDescriptor2 in list)
		{
			if (!mapDescriptor.IsNull())
			{
				flag = true;
				break;
			}
			mapDescriptor = mapDescriptor2;
		}
		if (flag)
		{
			GTDev.LogWarning<string>("[CustomMapLoader::LoadSceneFromAssetBundle] Found multiple MapDescriptor components in Scene \"" + sceneName + "\". Only the first one found will be used...", null);
		}
		if (mapDescriptor.IsNull())
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			if (useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action3 = CustomMapLoader.mapLoadProgressCallback;
				if (action3 != null)
				{
					action3(MapLoadStatus.Error, 0, "SCENE \"" + sceneName + "\" DOES NOT CONTAIN A MAP DESCRIPTOR ON ONE OF ITS ROOT GAME OBJECTS.");
				}
			}
			OnLoadComplete(false, false, "");
			yield break;
		}
		GameObject gameObject = mapDescriptor.gameObject;
		if (!CustomMapLoader.SanitizeObject(gameObject, gameObject))
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			if (useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action4 = CustomMapLoader.mapLoadProgressCallback;
				if (action4 != null)
				{
					action4(MapLoadStatus.Error, 0, "MAP DESCRIPTOR GAME OBJECT ON SCENE \"" + sceneName + "\" HAS UNAPPROVED COMPONENTS ON IT");
				}
			}
			OnLoadComplete(false, false, "");
			yield break;
		}
		if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion < 4)
		{
			foreach (TextMeshPro textMeshPro in gameObject.transform.GetComponentsInChildren<TextMeshPro>(true))
			{
				if (textMeshPro.font == null || textMeshPro.font.material == null)
				{
					textMeshPro.font = CustomMapLoader.instance.DefaultFont;
				}
			}
			foreach (TextMeshProUGUI textMeshProUGUI in gameObject.transform.GetComponentsInChildren<TextMeshProUGUI>(true))
			{
				if (textMeshProUGUI.font == null || textMeshProUGUI.font.material == null)
				{
					textMeshProUGUI.font = CustomMapLoader.instance.DefaultFont;
				}
			}
		}
		CustomMapLoader.totalObjectsInLoadingScene = 0;
		for (int l = 0; l < rootGameObjects.Length; l++)
		{
			CustomMapLoader.SanitizeObjectRecursive(rootGameObjects[l], gameObject);
		}
		CustomMapLoader.ResolveVirtualStumpColliderOverlaps(sceneName);
		if (useProgressCallback)
		{
			currentProgress += Mathf.RoundToInt((float)progressAmount * 0.2f);
			Action<MapLoadStatus, int, string> action5 = CustomMapLoader.mapLoadProgressCallback;
			if (action5 != null)
			{
				action5(MapLoadStatus.Loading, currentProgress, "MAP SCENE LOADED");
			}
		}
		CustomMapLoader.leafGliderIndex = 0;
		yield return CustomMapLoader.FinalizeSceneLoad(mapDescriptor, useProgressCallback, currentProgress, endingProgress);
		yield return null;
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			OnLoadComplete(false, true, "");
			if (CustomMapLoader.cachedExceptionMessage.Length > 0 && useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action6 = CustomMapLoader.mapLoadProgressCallback;
				if (action6 != null)
				{
					action6(MapLoadStatus.Error, 0, CustomMapLoader.cachedExceptionMessage);
				}
			}
			yield break;
		}
		if (CustomMapLoader.errorEncounteredDuringLoad)
		{
			OnLoadComplete(false, false, "");
			if (CustomMapLoader.cachedExceptionMessage.Length > 0 && useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action7 = CustomMapLoader.mapLoadProgressCallback;
				if (action7 != null)
				{
					action7(MapLoadStatus.Error, 0, CustomMapLoader.cachedExceptionMessage);
				}
			}
			yield break;
		}
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action8 = CustomMapLoader.mapLoadProgressCallback;
			if (action8 != null)
			{
				action8(MapLoadStatus.Loading, endingProgress, "FINALIZING MAP");
			}
		}
		CustomMapLoader.loadedSceneFilePaths.AddIfNew(CustomMapLoader.attemptedSceneToLoad);
		CustomMapLoader.loadedSceneNames.AddIfNew(sceneName);
		CustomMapLoader.loadedSceneIndexes.AddIfNew(sceneIndex);
		if (CustomMapLoader.refreshReviveStations)
		{
			CustomMapLoader.instance.ghostReactorManager.reactor.RefreshReviveStations(true);
		}
		OnLoadComplete(true, false, sceneName);
		yield break;
	}

	// Token: 0x060043EC RID: 17388 RVA: 0x00169B3C File Offset: 0x00167D3C
	private static void SanitizeObjectRecursive(GameObject rootObject, GameObject mapRoot)
	{
		if (!CustomMapLoader.SanitizeObject(rootObject, mapRoot))
		{
			return;
		}
		CustomMapLoader.totalObjectsInLoadingScene++;
		for (int i = 0; i < rootObject.transform.childCount; i++)
		{
			GameObject gameObject = rootObject.transform.GetChild(i).gameObject;
			if (gameObject.IsNotNull())
			{
				CustomMapLoader.SanitizeObjectRecursive(gameObject, mapRoot);
			}
		}
	}

	// Token: 0x060043ED RID: 17389 RVA: 0x00169B98 File Offset: 0x00167D98
	private static bool SanitizeObject(GameObject gameObject, GameObject mapRoot)
	{
		if (gameObject == null)
		{
			Debug.LogError("CustomMapLoader::SanitizeObject gameobject null");
			return false;
		}
		if (!CustomMapLoader.APPROVED_LAYERS.Contains(gameObject.layer))
		{
			gameObject.layer = 0;
		}
		foreach (Component component in gameObject.GetComponents<Component>())
		{
			if (component == null)
			{
				Debug.Log("CustomMapLoader::SanitizeObject null component has caused " + gameObject.name + " to be DELETED");
				Object.Destroy(gameObject);
				return false;
			}
			bool flag = true;
			foreach (Type type in CustomMapLoader.componentAllowlist)
			{
				if (component.GetType() == type)
				{
					if (type == typeof(Camera))
					{
						Camera camera = (Camera)component;
						if (camera.IsNotNull() && camera.targetTexture.IsNull())
						{
							break;
						}
					}
					flag = false;
					break;
				}
			}
			if (flag)
			{
				foreach (string text in CustomMapLoader.componentTypeStringAllowList)
				{
					if (component.GetType().ToString().Contains(text))
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				Debug.Log(string.Format("CustomMapLoader::SanitizeObject component type {0} has caused {1} to be DELETED", component.GetType(), gameObject.name));
				Object.Destroy(gameObject);
				return false;
			}
		}
		if (gameObject.transform.parent.IsNull() && gameObject.transform != mapRoot.transform)
		{
			gameObject.transform.SetParent(mapRoot.transform);
		}
		return true;
	}

	// Token: 0x060043EE RID: 17390 RVA: 0x00169D5C File Offset: 0x00167F5C
	private static void ResolveVirtualStumpColliderOverlaps(string sceneName)
	{
		Vector3 vector = new Vector3(5.15f, 0.72f, 5.15f);
		Vector3 vector2 = new Vector3(0f, 0.73f, 0f);
		float num = vector.x * 0.5f + 2f;
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		gameObject.transform.position = CustomMapLoader.instance.virtualStumpMesh.transform.position + vector2;
		gameObject.transform.localScale = vector;
		Collider[] array = Physics.OverlapSphere(gameObject.transform.position, num);
		if (array == null || array.Length == 0)
		{
			Object.Destroy(gameObject);
			return;
		}
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
		meshCollider.convex = true;
		foreach (Collider collider in array)
		{
			Vector3 vector3;
			float num2;
			if (!(collider == null) && !(collider.gameObject == gameObject) && !(collider.gameObject.scene.name != sceneName) && Physics.ComputePenetration(meshCollider, gameObject.transform.position, gameObject.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out vector3, out num2) && !collider.isTrigger)
			{
				Debug.Log("[CustomMapLoader::ResolveVirtualStumpColliderOverlaps] Gameobject " + collider.name + " has a collider overlapping with the virtual stump. Collider will be removed");
				Object.Destroy(collider);
			}
		}
		Object.Destroy(gameObject);
	}

	// Token: 0x060043EF RID: 17391 RVA: 0x00169EE3 File Offset: 0x001680E3
	private static IEnumerator FinalizeSceneLoad(MapDescriptor sceneDescriptor, bool useProgressCallback = false, int startingProgress = 50, int endingProgress = 90)
	{
		int num = endingProgress - startingProgress;
		int num2 = startingProgress;
		if (useProgressCallback)
		{
			num2 += Mathf.RoundToInt((float)num * 0.02f);
			Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
			if (action != null)
			{
				action(MapLoadStatus.Loading, num2, "PROCESSING ROOT MAP OBJECT");
			}
		}
		CustomMapLoader.objectsProcessedForLoadingScene = 0;
		CustomMapLoader.objectsProcessedThisFrame = 0;
		if (useProgressCallback)
		{
			num2 += Mathf.RoundToInt((float)num * 0.03f);
			Action<MapLoadStatus, int, string> action2 = CustomMapLoader.mapLoadProgressCallback;
			if (action2 != null)
			{
				action2(MapLoadStatus.Loading, num2, "PROCESSING CHILD OBJECTS");
			}
		}
		int processChildrenEndingProgress = endingProgress - Mathf.RoundToInt((float)num * 0.02f);
		CustomMapLoader.initializePhaseTwoComponents.Clear();
		CustomMapLoader.entitiesToCreate.Clear();
		CustomMapLoader.monkeGravityControllersToReplace.Clear();
		CustomMapLoader.replacedGravityZones.Clear();
		yield return CustomMapLoader.ProcessChildObjects(sceneDescriptor.gameObject, useProgressCallback, num2, processChildrenEndingProgress);
		if (CustomMapLoader.shouldAbortSceneLoad || CustomMapLoader.errorEncounteredDuringLoad)
		{
			yield break;
		}
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action3 = CustomMapLoader.mapLoadProgressCallback;
			if (action3 != null)
			{
				action3(MapLoadStatus.Loading, processChildrenEndingProgress, "PROCESSING COMPLETE");
			}
		}
		yield return null;
		CustomMapLoader.InitializeComponentsPhaseTwo();
		CustomMapLoader.placeholderReplacements.Clear();
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action4 = CustomMapLoader.mapLoadProgressCallback;
			if (action4 != null)
			{
				action4(MapLoadStatus.Loading, endingProgress, "PROCESSING COMPLETE");
			}
		}
		GorillaTelemetry.EnqueueTelemetryEvent("vstump_map_loaded", new Dictionary<string, object>(), null);
		if (CustomMapLoader.loadedMapPackageInfo != null && CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion < 3 && sceneDescriptor.IsInitialScene)
		{
			CustomMapLoader.maxPlayersForMap = (byte)Math.Clamp(sceneDescriptor.MaxPlayers, 1, 20);
			CustomMapLoader.cachedLuauScript = ((sceneDescriptor.CustomGamemode != null) ? sceneDescriptor.CustomGamemode.text : "");
			CustomMapLoader.devModeEnabled = sceneDescriptor.DevMode;
			CustomMapLoader.disableHoldingHandsAllModes = sceneDescriptor.DisableHoldingHandsAllGameModes;
			CustomMapLoader.disableHoldingHandsCustomMode = sceneDescriptor.DisableHoldingHandsCustomOnly;
			if (sceneDescriptor.UseUberShaderDynamicLighting)
			{
				CustomMapLoader.SetZoneDynamicLighting(true);
				GameLightingManager.instance.SetAmbientLightDynamic(sceneDescriptor.UberShaderAmbientDynamicLight);
			}
			List<int> list = new List<int>();
			foreach (GameModeType gameModeType in CustomMapLoader.instance.availableModesForOldMaps)
			{
				list.Add((int)gameModeType);
			}
			GameModeType gameModeType2 = CustomMapLoader.instance.defaultGameModeForNonCustomOldMaps;
			if (!CustomMapLoader.cachedLuauScript.IsNullOrEmpty())
			{
				gameModeType2 = GameModeType.Custom;
				list.Add(7);
			}
			CustomMapModeSelector.SetAvailableGameModes(list.ToArray(), (int)gameModeType2);
			if (RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
			{
				if (GameMode.ActiveGameMode.IsNull())
				{
					GameMode.ChangeGameMode(gameModeType2.ToString());
				}
				else if (GameMode.ActiveGameMode.GameType() != gameModeType2)
				{
					GameMode.ChangeGameMode(gameModeType2.ToString());
				}
			}
			VirtualStumpReturnWatch.SetWatchProperties(sceneDescriptor.GetReturnToVStumpWatchProps());
		}
		yield break;
	}

	// Token: 0x060043F0 RID: 17392 RVA: 0x00169F07 File Offset: 0x00168107
	private static IEnumerator ProcessChildObjects(GameObject parent, bool useProgressCallback = false, int startingProgress = 75, int endingProgress = 90)
	{
		if (parent == null || CustomMapLoader.placeholderReplacements.Contains(parent))
		{
			yield break;
		}
		int progressAmount = endingProgress - startingProgress;
		int num3;
		for (int i = 0; i < parent.transform.childCount; i = num3 + 1)
		{
			Transform child = parent.transform.GetChild(i);
			if (!(child == null))
			{
				GameObject gameObject = child.gameObject;
				if (!(gameObject == null) && !CustomMapLoader.placeholderReplacements.Contains(gameObject))
				{
					try
					{
						CustomMapLoader.InitializeComponentsPhaseOne(gameObject);
					}
					catch (Exception ex)
					{
						CustomMapLoader.errorEncounteredDuringLoad = true;
						CustomMapLoader.cachedExceptionMessage = ex.ToString();
						Debug.LogError("[CML.LoadMap] Exception: " + ex.ToString());
						yield break;
					}
					if (gameObject.transform.childCount > 0)
					{
						yield return CustomMapLoader.ProcessChildObjects(gameObject, useProgressCallback, startingProgress, endingProgress);
						if (CustomMapLoader.shouldAbortSceneLoad || CustomMapLoader.errorEncounteredDuringLoad)
						{
							yield break;
						}
					}
					if (CustomMapLoader.shouldAbortSceneLoad)
					{
						yield break;
					}
					CustomMapLoader.objectsProcessedForLoadingScene++;
					CustomMapLoader.objectsProcessedThisFrame++;
					if (CustomMapLoader.objectsProcessedThisFrame >= CustomMapLoader.numObjectsToProcessPerFrame)
					{
						CustomMapLoader.objectsProcessedThisFrame = 0;
						if (useProgressCallback)
						{
							float num = (float)CustomMapLoader.objectsProcessedForLoadingScene / (float)CustomMapLoader.totalObjectsInLoadingScene;
							int num2 = startingProgress + Mathf.FloorToInt((float)progressAmount * num);
							Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
							if (action != null)
							{
								action(MapLoadStatus.Loading, num2, "PROCESSING CHILD OBJECTS");
							}
						}
						yield return null;
					}
				}
			}
			num3 = i;
		}
		yield break;
	}

	// Token: 0x060043F1 RID: 17393 RVA: 0x00169F2B File Offset: 0x0016812B
	private static void InitializeComponentsPhaseOne(GameObject childGameObject)
	{
		CustomMapLoader.SetupCollisions(childGameObject);
		CustomMapLoader.ReplaceDataOnlyScripts(childGameObject);
		CustomMapLoader.ReplacePlaceholders(childGameObject);
		CustomMapLoader.SetupDynamicLight(childGameObject);
		CustomMapLoader.StoreMapEntity(childGameObject);
		CustomMapLoader.SetupReviveStation(childGameObject);
	}

	// Token: 0x060043F2 RID: 17394 RVA: 0x00169F54 File Offset: 0x00168154
	private static void InitializeComponentsPhaseTwo()
	{
		for (int i = 0; i < CustomMapLoader.initializePhaseTwoComponents.Count; i++)
		{
		}
		CustomMapLoader.initializePhaseTwoComponents.Clear();
		foreach (ValueTuple<MonkeGravityControllerSettings, BasicGravityZoneSettings> valueTuple in CustomMapLoader.monkeGravityControllersToReplace)
		{
			MonkeGravityControllerSettings item = valueTuple.Item1;
			BasicGravityZoneSettings item2 = valueTuple.Item2;
			if (!item.IsNull())
			{
				BasicGravityZone basicGravityZone = null;
				if (item2 != null)
				{
					CustomMapLoader.replacedGravityZones.TryGetValue(item2, out basicGravityZone);
				}
				item.gameObject.AddComponent<MonkeGravityController>().CopyProperties(item, basicGravityZone);
				Object.Destroy(item);
			}
		}
		CustomMapLoader.monkeGravityControllersToReplace.Clear();
		CustomMapLoader.replacedGravityZones.Clear();
		if (CustomMapLoader.entitiesToCreate.Count > 0)
		{
			for (int j = 0; j < CustomMapLoader.entitiesToCreate.Count; j++)
			{
				CustomMapLoader.entitiesToCreate[j].gameObject.SetActive(false);
			}
			CustomMapsGameManager.AddAgentsToCreate(CustomMapLoader.entitiesToCreate);
		}
	}

	// Token: 0x060043F3 RID: 17395 RVA: 0x0016A05C File Offset: 0x0016825C
	private static void SetupReviveStation(GameObject gameObject)
	{
		if (gameObject == null)
		{
			return;
		}
		CustomMapReviveStation component = gameObject.GetComponent<CustomMapReviveStation>();
		if (component == null)
		{
			return;
		}
		GameObject gameObject2 = Object.Instantiate<GameObject>(CustomMapLoader.instance.reviveStationPrefab, gameObject.transform.parent);
		if (gameObject2 == null)
		{
			return;
		}
		gameObject2.transform.position = gameObject.transform.position;
		gameObject2.transform.rotation = gameObject.transform.rotation;
		gameObject.transform.SetParent(gameObject2.transform);
		GRReviveStation component2 = gameObject2.GetComponent<GRReviveStation>();
		if (component2 == null)
		{
			return;
		}
		component2.audioSource = component.audioSource;
		if (!component.particleEffects.IsNullOrEmpty<ParticleSystem>())
		{
			component2.particleEffects = new ParticleSystem[component.particleEffects.Length];
			for (int i = 0; i < component.particleEffects.Length; i++)
			{
				component2.particleEffects[i] = component.particleEffects[i];
			}
		}
		component2.SetReviveCooldownSeconds(component.reviveCooldownSeconds);
		CustomMapLoader.refreshReviveStations = true;
	}

	// Token: 0x060043F4 RID: 17396 RVA: 0x0016A15C File Offset: 0x0016835C
	private static void SetupCollisions(GameObject gameObject)
	{
		if (gameObject == null || CustomMapLoader.placeholderReplacements.Contains(gameObject))
		{
			return;
		}
		Collider[] components = gameObject.GetComponents<Collider>();
		if (components == null)
		{
			return;
		}
		bool flag = true;
		foreach (Collider collider in components)
		{
			if (!(collider == null))
			{
				if (collider.isTrigger)
				{
					if (gameObject.layer != UnityLayer.GorillaInteractable.ToLayerIndex())
					{
						gameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
						break;
					}
				}
				else
				{
					if (gameObject.layer == UnityLayer.GorillaTrigger.ToLayerIndex())
					{
						collider.isTrigger = true;
					}
					flag = false;
					if (gameObject.GetComponent<GrabbableEntity>().IsNotNull())
					{
						gameObject.layer = UnityLayer.Default.ToLayerIndex();
						return;
					}
				}
			}
		}
		if (!flag)
		{
			SurfaceOverrideSettings component = gameObject.GetComponent<SurfaceOverrideSettings>();
			GorillaSurfaceOverride gorillaSurfaceOverride = gameObject.AddComponent<GorillaSurfaceOverride>();
			if (component == null)
			{
				gorillaSurfaceOverride.overrideIndex = 0;
				return;
			}
			gorillaSurfaceOverride.overrideIndex = (int)component.soundOverride;
			gorillaSurfaceOverride.extraVelMultiplier = component.extraVelMultiplier;
			gorillaSurfaceOverride.extraVelMaxMultiplier = component.extraVelMaxMultiplier;
			gorillaSurfaceOverride.slidePercentageOverride = component.slidePercentage;
			gorillaSurfaceOverride.disablePushBackEffect = component.disablePushBackEffect;
			Object.Destroy(component);
		}
	}

	// Token: 0x060043F5 RID: 17397 RVA: 0x0016A27C File Offset: 0x0016847C
	private static bool ValidateTeleporterDestination(Transform teleportTarget)
	{
		using (List<GameObject>.Enumerator enumerator = CustomMapLoader.storeCheckouts.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Vector3.Distance(enumerator.Current.transform.position, teleportTarget.position) < Constants.minTeleportDistFromStorePlaceholder)
				{
					return false;
				}
			}
		}
		using (List<GameObject>.Enumerator enumerator = CustomMapLoader.storeDisplayStands.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Vector3.Distance(enumerator.Current.transform.position, teleportTarget.position) < Constants.minTeleportDistFromStorePlaceholder)
				{
					return false;
				}
			}
		}
		return !CustomMapLoader.customMapATM.IsNotNull() || Vector3.Distance(CustomMapLoader.customMapATM.transform.position, teleportTarget.position) >= Constants.minTeleportDistFromStorePlaceholder;
	}

	// Token: 0x060043F6 RID: 17398 RVA: 0x0016A374 File Offset: 0x00168574
	private static bool ValidateStorePlaceholderPosition(GameObject storePlaceholder)
	{
		foreach (Component component in CustomMapLoader.teleporters)
		{
			if (!(component == null))
			{
				List<Transform> list = null;
				if (component.GetType() == typeof(CMSMapBoundary))
				{
					CMSMapBoundary cmsmapBoundary = (CMSMapBoundary)component;
					if (cmsmapBoundary != null)
					{
						list = cmsmapBoundary.TeleportPoints;
					}
				}
				else if (component.GetType() == typeof(CMSTeleporter))
				{
					CMSTeleporter cmsteleporter = (CMSTeleporter)component;
					if (cmsteleporter != null)
					{
						list = cmsteleporter.TeleportPoints;
					}
				}
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						Transform transform = list[i];
						if (Vector3.Distance(storePlaceholder.transform.position, transform.position) < Constants.minTeleportDistFromStorePlaceholder)
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	// Token: 0x060043F7 RID: 17399 RVA: 0x0016A480 File Offset: 0x00168680
	private static void ReplaceDataOnlyScripts(GameObject gameObject)
	{
		MapBoundarySettings[] components = gameObject.GetComponents<MapBoundarySettings>();
		if (components != null)
		{
			foreach (MapBoundarySettings mapBoundarySettings in components)
			{
				bool flag = false;
				for (int j = 0; j < mapBoundarySettings.TeleportPoints.Count; j++)
				{
					if (!mapBoundarySettings.TeleportPoints[j].IsNull() && !CustomMapLoader.ValidateTeleporterDestination(mapBoundarySettings.TeleportPoints[j]))
					{
						flag = true;
						Object.Destroy(mapBoundarySettings);
						break;
					}
				}
				if (!flag)
				{
					CMSMapBoundary cmsmapBoundary = gameObject.AddComponent<CMSMapBoundary>();
					if (cmsmapBoundary != null)
					{
						cmsmapBoundary.CopyTriggerSettings(mapBoundarySettings);
						CustomMapLoader.teleporters.Add(cmsmapBoundary);
					}
					Object.Destroy(mapBoundarySettings);
				}
			}
		}
		TagZoneSettings[] components2 = gameObject.GetComponents<TagZoneSettings>();
		if (components2 != null)
		{
			foreach (TagZoneSettings tagZoneSettings in components2)
			{
				CMSTagZone cmstagZone = gameObject.AddComponent<CMSTagZone>();
				if (cmstagZone != null)
				{
					cmstagZone.CopyTriggerSettings(tagZoneSettings);
				}
				Object.Destroy(tagZoneSettings);
			}
		}
		TeleporterSettings[] components3 = gameObject.GetComponents<TeleporterSettings>();
		if (components3 != null)
		{
			foreach (TeleporterSettings teleporterSettings in components3)
			{
				bool flag2 = false;
				for (int k = 0; k < teleporterSettings.TeleportPoints.Count; k++)
				{
					if (!teleporterSettings.TeleportPoints[k].IsNull() && !CustomMapLoader.ValidateTeleporterDestination(teleporterSettings.TeleportPoints[k]))
					{
						flag2 = true;
						Object.Destroy(teleporterSettings);
						break;
					}
				}
				if (!flag2)
				{
					CMSTeleporter cmsteleporter = gameObject.AddComponent<CMSTeleporter>();
					if (cmsteleporter != null)
					{
						cmsteleporter.CopyTriggerSettings(teleporterSettings);
					}
					Object.Destroy(teleporterSettings);
				}
			}
		}
		ObjectActivationTriggerSettings[] components4 = gameObject.GetComponents<ObjectActivationTriggerSettings>();
		if (components4 != null)
		{
			foreach (ObjectActivationTriggerSettings objectActivationTriggerSettings in components4)
			{
				CMSObjectActivationTrigger cmsobjectActivationTrigger = gameObject.AddComponent<CMSObjectActivationTrigger>();
				if (cmsobjectActivationTrigger != null)
				{
					cmsobjectActivationTrigger.CopyTriggerSettings(objectActivationTriggerSettings);
				}
				Object.Destroy(objectActivationTriggerSettings);
			}
		}
		LuauTriggerSettings[] components5 = gameObject.GetComponents<LuauTriggerSettings>();
		if (components5 != null)
		{
			foreach (LuauTriggerSettings luauTriggerSettings in components5)
			{
				CMSLuau cmsluau = gameObject.AddComponent<CMSLuau>();
				if (cmsluau != null)
				{
					cmsluau.CopyTriggerSettings(luauTriggerSettings);
				}
				Object.Destroy(luauTriggerSettings);
			}
		}
		PlayAnimationTriggerSettings[] components6 = gameObject.GetComponents<PlayAnimationTriggerSettings>();
		if (components6 != null)
		{
			foreach (PlayAnimationTriggerSettings playAnimationTriggerSettings in components6)
			{
				CMSPlayAnimationTrigger cmsplayAnimationTrigger = gameObject.AddComponent<CMSPlayAnimationTrigger>();
				if (cmsplayAnimationTrigger != null)
				{
					cmsplayAnimationTrigger.CopyTriggerSettings(playAnimationTriggerSettings);
				}
				Object.Destroy(playAnimationTriggerSettings);
			}
		}
		LoadZoneSettings[] components7 = gameObject.GetComponents<LoadZoneSettings>();
		if (components7 != null)
		{
			foreach (LoadZoneSettings loadZoneSettings in components7)
			{
				CMSLoadingZone cmsloadingZone = gameObject.AddComponent<CMSLoadingZone>();
				if (cmsloadingZone != null)
				{
					cmsloadingZone.SetupLoadingZone(loadZoneSettings, in CustomMapLoader.assetBundleSceneFilePaths);
				}
				Object.Destroy(loadZoneSettings);
			}
		}
		ZoneShaderTriggerSettings[] components8 = gameObject.GetComponents<ZoneShaderTriggerSettings>();
		if (components8 != null)
		{
			foreach (ZoneShaderTriggerSettings zoneShaderTriggerSettings in components8)
			{
				gameObject.AddComponent<CMSZoneShaderSettingsTrigger>().CopySettings(zoneShaderTriggerSettings);
				Object.Destroy(zoneShaderTriggerSettings);
			}
		}
		CMSZoneShaderSettings component = gameObject.GetComponent<CMSZoneShaderSettings>();
		if (component.IsNotNull())
		{
			ZoneShaderSettings zoneShaderSettings = gameObject.AddComponent<ZoneShaderSettings>();
			zoneShaderSettings.CopySettings(component, false);
			if (component.isDefaultValues)
			{
				CustomMapManager.SetDefaultZoneShaderSettings(zoneShaderSettings, component.GetProperties());
			}
			CustomMapManager.AddZoneShaderSettings(zoneShaderSettings);
			Object.Destroy(component);
		}
		HandHoldSettings component2 = gameObject.GetComponent<HandHoldSettings>();
		if (component2.IsNotNull())
		{
			gameObject.AddComponent<HandHold>().CopyProperties(component2);
			CustomMapLoader.handHoldCount++;
			Object.Destroy(component2);
		}
		CustomMapEjectButtonSettings component3 = gameObject.GetComponent<CustomMapEjectButtonSettings>();
		if (component3.IsNotNull())
		{
			CustomMapEjectButton customMapEjectButton = gameObject.AddComponent<CustomMapEjectButton>();
			customMapEjectButton.gameObject.layer = UnityLayer.GorillaInteractable.ToLayerIndex();
			customMapEjectButton.CopySettings(component3);
			Object.Destroy(component3);
		}
		MovingSurfaceSettings component4 = gameObject.GetComponent<MovingSurfaceSettings>();
		if (component4.IsNotNull())
		{
			MovingSurface movingSurface = gameObject.AddComponent<MovingSurface>();
			if (movingSurface.IsNotNull())
			{
				movingSurface.CopySettings(component4);
				Object.Destroy(component4);
			}
		}
		SurfaceMoverSettings component5 = gameObject.GetComponent<SurfaceMoverSettings>();
		if (component5.IsNotNull())
		{
			gameObject.AddComponent<SurfaceMover>().CopySettings(component5);
			Object.Destroy(component5);
		}
		SizeChangerSettings component6 = gameObject.GetComponent<SizeChangerSettings>();
		if (component6 != null)
		{
			gameObject.AddComponent<SizeChanger>().CopyProperties(component6);
			CustomMapLoader.sizeChangerCount++;
			Object.Destroy(component6);
		}
		MonkeGravityControllerSettings component7 = gameObject.GetComponent<MonkeGravityControllerSettings>();
		if (component7 != null)
		{
			CustomMapLoader.monkeGravityControllersToReplace.Add(new ValueTuple<MonkeGravityControllerSettings, BasicGravityZoneSettings>(component7, component7.alwaysInZone));
		}
		CustomMapLoader.ReplaceGravityDataOnlyScripts(gameObject);
	}

	// Token: 0x060043F8 RID: 17400 RVA: 0x0016A904 File Offset: 0x00168B04
	private static void ReplaceGravityDataOnlyScripts(GameObject gameObject)
	{
		BasicGravityZoneSettings component = gameObject.GetComponent<BasicGravityZoneSettings>();
		if (component == null)
		{
			return;
		}
		CustomMapLoader.gravityZoneCount++;
		ConsensusGravityZoneSettings consensusGravityZoneSettings = component as ConsensusGravityZoneSettings;
		if (consensusGravityZoneSettings != null)
		{
			ConsensusGravityZone consensusGravityZone = gameObject.AddComponent<ConsensusGravityZone>();
			consensusGravityZone.CopyProperties(consensusGravityZoneSettings);
			CustomMapLoader.replacedGravityZones.Add(consensusGravityZoneSettings, consensusGravityZone);
			Object.Destroy(consensusGravityZoneSettings);
			return;
		}
		TorusZoneSettings torusZoneSettings = component as TorusZoneSettings;
		if (torusZoneSettings != null)
		{
			TorusZone torusZone = gameObject.AddComponent<TorusZone>();
			torusZone.CopyProperties(torusZoneSettings);
			CustomMapLoader.replacedGravityZones.Add(torusZoneSettings, torusZone);
			Object.Destroy(torusZoneSettings);
			return;
		}
		PlanetZoneSettings planetZoneSettings = component as PlanetZoneSettings;
		if (planetZoneSettings == null)
		{
			BasicGravityZone basicGravityZone = gameObject.AddComponent<BasicGravityZone>();
			basicGravityZone.CopyProperties(component);
			CustomMapLoader.replacedGravityZones.Add(component, basicGravityZone);
			Object.Destroy(component);
			return;
		}
		CubicPlanetZoneSettings cubicPlanetZoneSettings = planetZoneSettings as CubicPlanetZoneSettings;
		if (cubicPlanetZoneSettings != null)
		{
			CubicPlanetZone cubicPlanetZone = gameObject.AddComponent<CubicPlanetZone>();
			cubicPlanetZone.CopyProperties(cubicPlanetZoneSettings);
			CustomMapLoader.replacedGravityZones.Add(cubicPlanetZoneSettings, cubicPlanetZone);
			Object.Destroy(cubicPlanetZoneSettings);
			return;
		}
		PlanetZone planetZone = gameObject.AddComponent<PlanetZone>();
		planetZone.CopyProperties(planetZoneSettings);
		CustomMapLoader.replacedGravityZones.Add(planetZoneSettings, planetZone);
		Object.Destroy(planetZoneSettings);
	}

	// Token: 0x060043F9 RID: 17401 RVA: 0x0016AA08 File Offset: 0x00168C08
	private static void ReplacePlaceholders(GameObject placeholderGameObject)
	{
		if (placeholderGameObject.IsNull())
		{
			return;
		}
		GTObjectPlaceholder component = placeholderGameObject.GetComponent<GTObjectPlaceholder>();
		if (component.IsNull())
		{
			return;
		}
		CustomMapLoader.mapperAssetCount++;
		switch (component.PlaceholderObject)
		{
		case GTObject.LeafGlider:
			if (CustomMapLoader.leafGliderIndex < CustomMapLoader.instance.leafGliders.Length)
			{
				CustomMapLoader.instance.leafGliders[CustomMapLoader.leafGliderIndex].enabled = true;
				CustomMapLoader.instance.leafGliders[CustomMapLoader.leafGliderIndex].CustomMapLoad(component.transform, component.maxDistanceBeforeRespawn);
				CustomMapLoader.instance.leafGliders[CustomMapLoader.leafGliderIndex].transform.GetChild(0).gameObject.SetActive(true);
				CustomMapLoader.leafGliderIndex++;
				return;
			}
			break;
		case GTObject.GliderWindVolume:
		{
			List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
			if (component.useDefaultPlaceholder || list.Count == 0)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(CustomMapLoader.instance.gliderWindVolume, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
				if (gameObject != null)
				{
					CustomMapLoader.placeholderReplacements.Add(gameObject);
					gameObject.transform.localScale = placeholderGameObject.transform.localScale;
					placeholderGameObject.transform.localScale = Vector3.one;
					gameObject.transform.SetParent(placeholderGameObject.transform);
					GliderWindVolume component2 = gameObject.GetComponent<GliderWindVolume>();
					if (component2 == null)
					{
						return;
					}
					component2.SetProperties(component.maxSpeed, component.maxAccel, component.SpeedVSAccelCurve, component.localWindDirection);
					return;
				}
			}
			else
			{
				placeholderGameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
				GliderWindVolume gliderWindVolume = placeholderGameObject.AddComponent<GliderWindVolume>();
				if (gliderWindVolume.IsNotNull())
				{
					gliderWindVolume.SetProperties(component.maxSpeed, component.maxAccel, component.SpeedVSAccelCurve, component.localWindDirection);
					return;
				}
			}
			break;
		}
		case GTObject.WaterVolume:
		{
			List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
			if (component.useDefaultPlaceholder || list.Count == 0)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(CustomMapLoader.instance.waterVolumePrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
				if (gameObject2 != null)
				{
					CustomMapLoader.placeholderReplacements.Add(gameObject2);
					gameObject2.layer = UnityLayer.Water.ToLayerIndex();
					gameObject2.transform.localScale = placeholderGameObject.transform.localScale;
					placeholderGameObject.transform.localScale = Vector3.one;
					gameObject2.transform.SetParent(placeholderGameObject.transform);
					MeshRenderer component3 = gameObject2.GetComponent<MeshRenderer>();
					if (component3.IsNull())
					{
						return;
					}
					if (!component.useWaterMesh)
					{
						component3.enabled = false;
						return;
					}
					component3.enabled = true;
					WaterSurfaceMaterialController component4 = gameObject2.GetComponent<WaterSurfaceMaterialController>();
					if (component4.IsNull())
					{
						return;
					}
					component4.ScrollX = component.scrollTextureX;
					component4.ScrollY = component.scrollTextureY;
					component4.Scale = component.scaleTexture;
					return;
				}
			}
			else
			{
				placeholderGameObject.layer = UnityLayer.Water.ToLayerIndex();
				WaterVolume waterVolume = placeholderGameObject.AddComponent<WaterVolume>();
				if (waterVolume.IsNotNull())
				{
					WaterParameters waterParameters = null;
					CMSZoneShaderSettings.EZoneLiquidType liquidType = component.liquidType;
					if (liquidType != CMSZoneShaderSettings.EZoneLiquidType.Water)
					{
						if (liquidType == CMSZoneShaderSettings.EZoneLiquidType.Lava)
						{
							waterParameters = CustomMapLoader.instance.defaultLavaParameters;
						}
					}
					else
					{
						waterParameters = CustomMapLoader.instance.defaultWaterParameters;
					}
					waterVolume.SetPropertiesFromPlaceholder(component.GetWaterVolumeProperties(), list, waterParameters);
					waterVolume.RefreshColliders();
					return;
				}
			}
			break;
		}
		case GTObject.ForceVolume:
		{
			List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
			if (component.useDefaultPlaceholder || list.Count == 0)
			{
				GameObject gameObject3 = Object.Instantiate<GameObject>(CustomMapLoader.instance.forceVolumePrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
				if (gameObject3.IsNotNull())
				{
					CustomMapLoader.placeholderReplacements.Add(gameObject3);
					gameObject3.transform.localScale = placeholderGameObject.transform.localScale;
					placeholderGameObject.transform.localScale = Vector3.one;
					gameObject3.transform.SetParent(placeholderGameObject.transform);
					ForceVolume component5 = gameObject3.GetComponent<ForceVolume>();
					if (component5.IsNull())
					{
						return;
					}
					component5.SetPropertiesFromPlaceholder(component.GetForceVolumeProperties(), null, null);
					return;
				}
			}
			else
			{
				ForceVolume forceVolume = placeholderGameObject.AddComponent<ForceVolume>();
				if (forceVolume.IsNotNull())
				{
					AudioSource audioSource = placeholderGameObject.GetComponent<AudioSource>();
					if (audioSource.IsNull())
					{
						audioSource = placeholderGameObject.AddComponent<AudioSource>();
						audioSource.spatialize = true;
						audioSource.playOnAwake = false;
						audioSource.priority = 128;
						audioSource.volume = 0.522f;
						audioSource.pitch = 1f;
						audioSource.panStereo = 0f;
						audioSource.spatialBlend = 1f;
						audioSource.reverbZoneMix = 1f;
						audioSource.dopplerLevel = 1f;
						audioSource.spread = 0f;
						audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
						audioSource.minDistance = 8.2f;
						audioSource.maxDistance = 43.94f;
						audioSource.enabled = true;
					}
					audioSource.outputAudioMixerGroup = CustomMapLoader.instance.masterAudioMixer;
					for (int i = list.Count - 1; i >= 0; i--)
					{
						if (i == 0)
						{
							list[i].isTrigger = true;
						}
						else
						{
							Object.Destroy(list[i]);
						}
					}
					placeholderGameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
					forceVolume.SetPropertiesFromPlaceholder(component.GetForceVolumeProperties(), audioSource, component.GetComponent<Collider>());
					return;
				}
				Debug.LogError("[CustomMapLoader::ReplacePlaceholders] Failed to add ForceVolume component to Placeholder!");
				return;
			}
			break;
		}
		case GTObject.ATM:
		{
			if (CustomMapLoader.customMapATM.IsNotNull())
			{
				Object.Destroy(component);
				return;
			}
			if (!CustomMapLoader.ValidateStorePlaceholderPosition(placeholderGameObject))
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject4 = CustomMapLoader.instance.atmPrefab;
			if (component.useCustomMesh)
			{
				gameObject4 = CustomMapLoader.instance.atmNoShellPrefab;
			}
			if (gameObject4.IsNull())
			{
				return;
			}
			GameObject gameObject5 = Object.Instantiate<GameObject>(gameObject4, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
			if (gameObject5.IsNotNull())
			{
				gameObject5.transform.SetParent(CustomMapLoader.instance.compositeTryOnArea.transform, true);
				gameObject5.transform.localScale = Vector3.one;
				ATM_UI componentInChildren = gameObject5.GetComponentInChildren<ATM_UI>();
				if (componentInChildren.IsNotNull() && ATM_Manager.instance.IsNotNull())
				{
					componentInChildren.SetCustomMapScene(placeholderGameObject.scene);
					CustomMapLoader.customMapATM = gameObject5;
					ATM_Manager.instance.AddATM(componentInChildren, null);
					if (!component.defaultCreatorCode.IsNullOrEmpty())
					{
						ATM_Manager.instance.SetTemporaryCreatorCode(component.defaultCreatorCode);
						return;
					}
				}
			}
			break;
		}
		case GTObject.HoverboardArea:
			if (component.AddComponent<HoverboardAreaTrigger>().IsNotNull())
			{
				component.gameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
				List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
				if (list.Count != 0)
				{
					for (int j = list.Count - 1; j >= 0; j--)
					{
						if (j == 0)
						{
							list[j].isTrigger = true;
						}
						else
						{
							Object.Destroy(list[j]);
						}
					}
					return;
				}
				BoxCollider boxCollider = component.AddComponent<BoxCollider>();
				if (boxCollider.IsNotNull())
				{
					boxCollider.isTrigger = true;
					return;
				}
			}
			break;
		case GTObject.HoverboardDispenser:
		{
			if (CustomMapLoader.instance.hoverboardDispenserPrefab.IsNull())
			{
				Debug.LogError("[CustomMapLoader::ReplacePlaceholders] hoverboardDispenserPrefab is NULL!");
				return;
			}
			GameObject gameObject6 = Object.Instantiate<GameObject>(CustomMapLoader.instance.hoverboardDispenserPrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
			if (gameObject6.IsNotNull())
			{
				CustomMapLoader.placeholderReplacements.Add(gameObject6);
				gameObject6.transform.SetParent(placeholderGameObject.transform);
				return;
			}
			break;
		}
		case GTObject.RopeSwing:
		{
			GameObject gameObject7 = Object.Instantiate<GameObject>(CustomMapLoader.instance.ropeSwingPrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
			if (gameObject7.IsNull())
			{
				return;
			}
			gameObject7.transform.SetParent(placeholderGameObject.transform);
			CustomMapsGorillaRopeSwing component6 = gameObject7.GetComponent<CustomMapsGorillaRopeSwing>();
			if (component6.IsNull())
			{
				Object.Destroy(gameObject7);
				return;
			}
			component.ropeLength = Math.Clamp(component.ropeLength, 3, 31);
			if (component.useDefaultPlaceholder)
			{
				component6.SetRopeLength(component.ropeLength);
			}
			else
			{
				component6.SetRopeProperties(component);
			}
			CustomMapLoader.placeholderReplacements.Add(gameObject7);
			return;
		}
		case GTObject.ZipLine:
		{
			GameObject gameObject8 = Object.Instantiate<GameObject>(CustomMapLoader.instance.ziplinePrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
			if (gameObject8.IsNull())
			{
				return;
			}
			gameObject8.transform.SetParent(placeholderGameObject.transform);
			CustomMapsGorillaZipline component7 = gameObject8.GetComponent<CustomMapsGorillaZipline>();
			if (component7.IsNull())
			{
				Object.Destroy(gameObject8);
				return;
			}
			if (component.useDefaultPlaceholder)
			{
				if (!component7.GenerateZipline(component.spline))
				{
					Object.Destroy(gameObject8);
					return;
				}
			}
			else
			{
				component7.Init(component);
			}
			CustomMapLoader.placeholderReplacements.Add(gameObject8);
			return;
		}
		case GTObject.Store_DisplayStand:
		{
			if (CustomMapLoader.instance.storeDisplayStandPrefab.IsNull())
			{
				return;
			}
			if (CustomMapLoader.storeDisplayStands.Count >= Constants.storeDisplayStandLimit)
			{
				Object.Destroy(component);
				return;
			}
			if (placeholderGameObject.transform.lossyScale != Vector3.one)
			{
				Object.Destroy(component);
				return;
			}
			if (!CustomMapLoader.ValidateStorePlaceholderPosition(placeholderGameObject))
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject9 = Object.Instantiate<GameObject>(CustomMapLoader.instance.storeDisplayStandPrefab, placeholderGameObject.transform);
			if (gameObject9.IsNull())
			{
				return;
			}
			gameObject9.transform.SetParent(CustomMapLoader.instance.compositeTryOnArea.transform, true);
			gameObject9.transform.localScale = Vector3.one;
			DynamicCosmeticStand component8 = gameObject9.GetComponent<DynamicCosmeticStand>();
			if (component8.IsNull())
			{
				Object.Destroy(gameObject9);
				return;
			}
			component8.InitializeForCustomMapCosmeticItem(component.CosmeticItem, placeholderGameObject.scene);
			CustomMapLoader.storeDisplayStands.Add(gameObject9);
			CustomMapLoader.placeholderReplacements.Add(gameObject9);
			return;
		}
		case GTObject.Store_TryOnArea:
		{
			if (CustomMapLoader.instance.storeTryOnAreaPrefab.IsNull() || CustomMapLoader.instance.compositeTryOnArea.IsNull())
			{
				return;
			}
			if (CustomMapLoader.storeTryOnAreas.Count >= Constants.storeTryOnAreaLimit)
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject10 = Object.Instantiate<GameObject>(CustomMapLoader.instance.storeTryOnAreaPrefab, placeholderGameObject.transform);
			gameObject10.transform.SetParent(CustomMapLoader.instance.compositeTryOnArea.transform);
			CMSTryOnArea component9 = gameObject10.GetComponent<CMSTryOnArea>();
			if (component9.IsNull() || component9.tryOnAreaCollider.IsNull())
			{
				Object.Destroy(gameObject10);
				return;
			}
			BoxCollider tryOnAreaCollider = component9.tryOnAreaCollider;
			Vector3 zero = Vector3.zero;
			zero.x = tryOnAreaCollider.size.x * tryOnAreaCollider.transform.lossyScale.x;
			zero.y = tryOnAreaCollider.size.y * tryOnAreaCollider.transform.lossyScale.y;
			zero.z = tryOnAreaCollider.size.z * tryOnAreaCollider.transform.lossyScale.z;
			float num = Math.Abs(zero.x * zero.y * zero.z);
			if (num > Constants.storeTryOnAreaVolumeLimit)
			{
				Debug.Log("[CustomMapLoader::ReplacePlaceholders] TryOnArea collider too large." + string.Format(" Volume:{0} | Limit: {1}", num, Constants.storeTryOnAreaVolumeLimit));
				Object.Destroy(gameObject10);
				return;
			}
			component9.InitializeForCustomMap(CustomMapLoader.instance.compositeTryOnArea, placeholderGameObject.scene);
			CustomMapLoader.storeTryOnAreas.Add(gameObject10);
			CustomMapLoader.placeholderReplacements.Add(gameObject10);
			break;
		}
		case GTObject.Store_Checkout:
		{
			if (CustomMapLoader.instance.storeCheckoutCounterPrefab.IsNull())
			{
				return;
			}
			if (CustomMapLoader.storeCheckouts.Count >= Constants.storeCheckoutCounterLimit)
			{
				Object.Destroy(component);
				return;
			}
			if (placeholderGameObject.transform.lossyScale != Vector3.one)
			{
				Object.Destroy(component);
				return;
			}
			if (!CustomMapLoader.ValidateStorePlaceholderPosition(placeholderGameObject))
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject11 = Object.Instantiate<GameObject>(CustomMapLoader.instance.storeCheckoutCounterPrefab, placeholderGameObject.transform);
			if (gameObject11.IsNull())
			{
				return;
			}
			gameObject11.transform.SetParent(CustomMapLoader.instance.compositeTryOnArea.transform);
			gameObject11.transform.localScale = Vector3.one;
			ItemCheckout componentInChildren2 = gameObject11.GetComponentInChildren<ItemCheckout>();
			if (componentInChildren2.IsNull())
			{
				Object.Destroy(gameObject11);
				return;
			}
			componentInChildren2.InitializeForCustomMap(CustomMapLoader.instance.compositeTryOnArea, placeholderGameObject.scene, component.useCustomMesh);
			CustomMapLoader.storeCheckouts.Add(gameObject11);
			CustomMapLoader.placeholderReplacements.Add(gameObject11);
			return;
		}
		case GTObject.Store_TryOnConsole:
		{
			if (CustomMapLoader.instance.storeTryOnConsolePrefab.IsNull())
			{
				return;
			}
			if (CustomMapLoader.storeTryOnConsoles.Count >= Constants.storeTryOnConsoleLimit)
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject12 = Object.Instantiate<GameObject>(CustomMapLoader.instance.storeTryOnConsolePrefab, placeholderGameObject.transform);
			if (gameObject12.IsNull())
			{
				return;
			}
			FittingRoom componentInChildren3 = gameObject12.GetComponentInChildren<FittingRoom>();
			if (componentInChildren3.IsNull())
			{
				Object.Destroy(gameObject12);
				return;
			}
			componentInChildren3.InitializeForCustomMap(component.useCustomMesh);
			CustomMapLoader.storeTryOnConsoles.Add(gameObject12);
			CustomMapLoader.placeholderReplacements.Add(gameObject12);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x060043FA RID: 17402 RVA: 0x0016B6F0 File Offset: 0x001698F0
	private static void SetupDynamicLight(GameObject dynamicLightGameObject)
	{
		if (dynamicLightGameObject.IsNull())
		{
			return;
		}
		UberShaderDynamicLight component = dynamicLightGameObject.GetComponent<UberShaderDynamicLight>();
		if (component.IsNull())
		{
			return;
		}
		if (component.dynamicLight.IsNull())
		{
			return;
		}
		GameObject gameObject = new GameObject(dynamicLightGameObject.name + "GameLight");
		GameLight gameLight = gameObject.AddComponent<GameLight>();
		gameLight.light = component.dynamicLight;
		GameLightingManager.instance.AddGameLight(gameLight, false);
		gameObject.transform.SetParent(dynamicLightGameObject.transform.parent);
		gameObject.transform.position = component.transform.position;
	}

	// Token: 0x060043FB RID: 17403 RVA: 0x0016B788 File Offset: 0x00169988
	private static void StoreMapEntity(GameObject entityGameObject)
	{
		if (entityGameObject.IsNull() || CustomMapsGameManager.instance.IsNull())
		{
			return;
		}
		MapEntity component = entityGameObject.GetComponent<MapEntity>();
		if (component.IsNull())
		{
			return;
		}
		if (component is AIAgent)
		{
			AIAgent aiagent = (AIAgent)component;
			if (!aiagent.IsNull())
			{
				string.Format(" | AgentID: {0}", aiagent.enemyTypeId);
			}
		}
		if (component.isTemplate)
		{
			return;
		}
		CustomMapLoader.entitiesToCreate.Add(component);
	}

	// Token: 0x060043FC RID: 17404 RVA: 0x0016B7FC File Offset: 0x001699FC
	private static void CacheLightmaps()
	{
		CustomMapLoader.lightmaps = new LightmapData[LightmapSettings.lightmaps.Length];
		if (CustomMapLoader.lightmapsToKeep.Count > 0)
		{
			CustomMapLoader.lightmapsToKeep.Clear();
		}
		CustomMapLoader.lightmapsToKeep = new List<Texture2D>(LightmapSettings.lightmaps.Length * 2);
		for (int i = 0; i < LightmapSettings.lightmaps.Length; i++)
		{
			CustomMapLoader.lightmaps[i] = LightmapSettings.lightmaps[i];
			if (LightmapSettings.lightmaps[i].lightmapColor != null)
			{
				CustomMapLoader.lightmapsToKeep.Add(LightmapSettings.lightmaps[i].lightmapColor);
			}
			if (LightmapSettings.lightmaps[i].lightmapDir != null)
			{
				CustomMapLoader.lightmapsToKeep.Add(LightmapSettings.lightmaps[i].lightmapDir);
			}
		}
	}

	// Token: 0x060043FD RID: 17405 RVA: 0x0016B8B8 File Offset: 0x00169AB8
	private static void LoadLightmaps(Texture2D[] colorMaps, Texture2D[] dirMaps)
	{
		if (colorMaps.Length == 0)
		{
			return;
		}
		CustomMapLoader.UnloadLightmaps();
		List<LightmapData> list = new List<LightmapData>(LightmapSettings.lightmaps);
		for (int i = 0; i < colorMaps.Length; i++)
		{
			bool flag = false;
			LightmapData lightmapData = new LightmapData();
			if (colorMaps[i] != null)
			{
				lightmapData.lightmapColor = colorMaps[i];
				flag = true;
				if (i < dirMaps.Length && dirMaps[i] != null)
				{
					lightmapData.lightmapDir = dirMaps[i];
				}
			}
			if (flag)
			{
				list.Add(lightmapData);
			}
		}
		LightmapSettings.lightmaps = list.ToArray();
	}

	// Token: 0x060043FE RID: 17406 RVA: 0x0016B938 File Offset: 0x00169B38
	public static void ResetToInitialZone(Action<string> onSceneLoaded, Action<string> onSceneUnloaded)
	{
		List<int> list = new List<int>(CustomMapLoader.initialSceneIndexes);
		List<int> list2 = new List<int>(CustomMapLoader.loadedSceneIndexes);
		foreach (int num in CustomMapLoader.loadedSceneIndexes)
		{
			if (CustomMapLoader.initialSceneIndexes.Contains(num))
			{
				list2.Remove(num);
				list.Remove(num);
			}
		}
		if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion <= 2 && CustomMapLoader.loadedSceneIndexes.Contains(CustomMapLoader.initialSceneIndexes[0]))
		{
			MapDescriptor[] array = Object.FindObjectsByType<MapDescriptor>(FindObjectsSortMode.None);
			bool flag = false;
			int i;
			for (i = 0; i < array.Length; i++)
			{
				if (array[i].IsInitialScene && array[i].UseUberShaderDynamicLighting)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				CustomMapLoader.SetZoneDynamicLighting(true);
				GameLightingManager.instance.SetAmbientLightDynamic(array[i].UberShaderAmbientDynamicLight);
			}
			else
			{
				CustomMapLoader.SetZoneDynamicLighting(false);
				GameLightingManager.instance.SetAmbientLightDynamic(Color.black);
			}
		}
		else if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion > 2)
		{
			if (CustomMapLoader.loadedMapPackageInfo.useUberShaderDynamicLighting)
			{
				Color color = new Color(CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_R, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_G, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_B, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_A);
				CustomMapLoader.SetZoneDynamicLighting(true);
				GameLightingManager.instance.SetAmbientLightDynamic(color);
			}
			else
			{
				CustomMapLoader.SetZoneDynamicLighting(false);
				GameLightingManager.instance.SetAmbientLightDynamic(Color.black);
			}
		}
		if (list.IsNullOrEmpty<int>() && list2.IsNullOrEmpty<int>())
		{
			return;
		}
		if (CustomMapLoader.zoneLoadingCoroutine != null)
		{
			CustomMapLoader.LoadZoneRequest loadZoneRequest = new CustomMapLoader.LoadZoneRequest
			{
				sceneIndexesToLoad = list.ToArray(),
				sceneIndexesToUnload = list2.ToArray(),
				onSceneLoadedCallback = onSceneLoaded,
				onSceneUnloadedCallback = onSceneUnloaded
			};
			CustomMapLoader.queuedLoadZoneRequests.Add(loadZoneRequest);
			return;
		}
		CustomMapLoader.sceneLoadedCallback = onSceneLoaded;
		CustomMapLoader.sceneUnloadedCallback = onSceneUnloaded;
		CustomMapLoader.zoneLoadingCoroutine = CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadZoneCoroutine(list.ToArray(), list2.ToArray()));
	}

	// Token: 0x060043FF RID: 17407 RVA: 0x0016BB58 File Offset: 0x00169D58
	public static void LoadZoneTriggered(int[] loadSceneIndexes, int[] unloadSceneIndexes, Action<string> onSceneLoaded, Action<string> onSceneUnloaded)
	{
		string text = "";
		for (int i = 0; i < loadSceneIndexes.Length; i++)
		{
			text += loadSceneIndexes[i].ToString();
			if (i != loadSceneIndexes.Length - 1)
			{
				text += ", ";
			}
		}
		string text2 = "";
		for (int j = 0; j < unloadSceneIndexes.Length; j++)
		{
			text2 += unloadSceneIndexes[j].ToString();
			if (j != unloadSceneIndexes.Length - 1)
			{
				text2 += ", ";
			}
		}
		if (CustomMapLoader.zoneLoadingCoroutine != null)
		{
			CustomMapLoader.LoadZoneRequest loadZoneRequest = new CustomMapLoader.LoadZoneRequest
			{
				sceneIndexesToLoad = loadSceneIndexes,
				sceneIndexesToUnload = unloadSceneIndexes,
				onSceneLoadedCallback = onSceneLoaded,
				onSceneUnloadedCallback = onSceneUnloaded
			};
			CustomMapLoader.queuedLoadZoneRequests.Add(loadZoneRequest);
			return;
		}
		CustomMapLoader.sceneLoadedCallback = onSceneLoaded;
		CustomMapLoader.sceneUnloadedCallback = onSceneUnloaded;
		CustomMapLoader.zoneLoadingCoroutine = CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadZoneCoroutine(loadSceneIndexes, unloadSceneIndexes));
	}

	// Token: 0x06004400 RID: 17408 RVA: 0x0016BC3F File Offset: 0x00169E3F
	private static IEnumerator LoadZoneCoroutine(int[] loadScenes, int[] unloadScenes)
	{
		if (!unloadScenes.IsNullOrEmpty<int>())
		{
			yield return CustomMapLoader.UnloadScenesCoroutine(unloadScenes);
		}
		if (!loadScenes.IsNullOrEmpty<int>())
		{
			yield return CustomMapLoader.LoadScenesCoroutine(loadScenes, delegate(bool successfullyLoadedAllScenes, bool loadAborted, List<string> successfullyLoadedSceneNames)
			{
				if (loadAborted)
				{
					CustomMapLoader.queuedLoadZoneRequests.Clear();
				}
			});
		}
		CustomMapLoader.zoneLoadingCoroutine = null;
		if (CustomMapLoader.queuedLoadZoneRequests.Count > 0)
		{
			CustomMapLoader.LoadZoneRequest loadZoneRequest = CustomMapLoader.queuedLoadZoneRequests[0];
			CustomMapLoader.queuedLoadZoneRequests.RemoveAt(0);
			CustomMapLoader.LoadZoneTriggered(loadZoneRequest.sceneIndexesToLoad, loadZoneRequest.sceneIndexesToUnload, loadZoneRequest.onSceneLoadedCallback, loadZoneRequest.onSceneUnloadedCallback);
		}
		yield break;
	}

	// Token: 0x06004401 RID: 17409 RVA: 0x0016BC55 File Offset: 0x00169E55
	public static void CloseDoorAndUnloadMap(Action unloadCompleted = null)
	{
		if (!CustomMapLoader.IsMapLoaded() && !CustomMapLoader.isLoading)
		{
			return;
		}
		if (unloadCompleted != null)
		{
			CustomMapLoader.unloadMapCallback = unloadCompleted;
		}
		if (CustomMapLoader.isLoading)
		{
			CustomMapLoader.RequestAbortMapLoad();
			return;
		}
		CustomMapLoader.instance.StartCoroutine(CustomMapLoader.CloseDoorAndUnloadMapCoroutine());
	}

	// Token: 0x06004402 RID: 17410 RVA: 0x0016BC8E File Offset: 0x00169E8E
	private static IEnumerator CloseDoorAndUnloadMapCoroutine()
	{
		if (!CustomMapLoader.IsMapLoaded())
		{
			yield break;
		}
		if (CustomMapLoader.instance.accessDoor != null)
		{
			CustomMapLoader.instance.accessDoor.CloseDoor();
		}
		if (CustomMapLoader.instance.publicJoinTrigger != null)
		{
			CustomMapLoader.instance.publicJoinTrigger.SetActive(false);
		}
		CustomMapLoader.shouldAbortMapLoading = true;
		if (CustomMapLoader.IsLoading())
		{
			yield break;
		}
		yield return CustomMapLoader.UnloadMapCoroutine();
		yield break;
	}

	// Token: 0x06004403 RID: 17411 RVA: 0x0016BC96 File Offset: 0x00169E96
	private static void RequestAbortMapLoad()
	{
		CustomMapLoader.shouldAbortSceneLoad = true;
		CustomMapLoader.shouldAbortMapLoading = true;
	}

	// Token: 0x06004404 RID: 17412 RVA: 0x0016BCA4 File Offset: 0x00169EA4
	private static IEnumerator AbortMapLoad()
	{
		GTDev.Log<string>("[CML.AbortMapLoad] Aborting map load...", null);
		CustomMapLoader.shouldAbortSceneLoad = true;
		CustomMapLoader.shouldAbortMapLoading = true;
		yield return CustomMapLoader.AbortSceneLoad(-1);
		Action<bool> action = CustomMapLoader.mapLoadFinishedCallback;
		if (action != null)
		{
			action(false);
		}
		yield break;
	}

	// Token: 0x06004405 RID: 17413 RVA: 0x0016BCAC File Offset: 0x00169EAC
	private static IEnumerator UnloadMapCoroutine()
	{
		GTDev.Log<string>("[CML.UnloadMap_Co] Unloading Custom Map...", null);
		if (CustomMapLoader.zoneLoadingCoroutine != null)
		{
			CustomMapLoader.queuedLoadZoneRequests.Clear();
			CustomMapLoader.instance.StopCoroutine(CustomMapLoader.zoneLoadingCoroutine);
			CustomMapLoader.zoneLoadingCoroutine = null;
		}
		CustomMapLoader.isUnloading = true;
		CustomMapLoader.CanLoadEntities = false;
		CustomMapTelemetry.OnMapUnloaded();
		CustomMapTelemetry.EndMapTracking();
		ZoneShaderSettings.ActivateDefaultSettings();
		CustomMapLoader.CleanupPlaceholders();
		CMSSerializer.ResetSyncedMapObjects();
		CustomMapsGameManager.ClearAgentsToCreate();
		CustomMapLoader.instance.ghostReactorManager.reactor.RefreshReviveStations(false);
		if (!CustomMapLoader.assetBundleSceneFilePaths.IsNullOrEmpty<string>())
		{
			int num;
			for (int sceneIndex = 0; sceneIndex < CustomMapLoader.assetBundleSceneFilePaths.Length; sceneIndex = num + 1)
			{
				yield return CustomMapLoader.UnloadSceneCoroutine(sceneIndex, null);
				num = sceneIndex;
			}
		}
		GorillaNetworkJoinTrigger.EnableTriggerJoins();
		LightmapSettings.lightmaps = CustomMapLoader.lightmaps;
		CustomMapLoader.UnloadLightmaps();
		yield return CustomMapLoader.ResetLightmaps();
		CustomMapLoader.SetZoneDynamicLighting(false);
		GameLightingManager.instance.SetAmbientLightDynamic(Color.black);
		if (CustomMapLoader.mapBundle != null)
		{
			CustomMapLoader.mapBundle.Unload(true);
		}
		CustomMapLoader.mapBundle = null;
		Resources.UnloadUnusedAssets();
		CustomMapLoader.cachedLuauScript = "";
		CustomMapLoader.devModeEnabled = false;
		CustomMapLoader.disableHoldingHandsAllModes = false;
		CustomMapLoader.disableHoldingHandsCustomMode = false;
		CustomMapLoader.queuedLoadZoneRequests.Clear();
		CustomMapLoader.assetBundleSceneFilePaths = new string[] { "" };
		CustomMapLoader.loadedMapPackageInfo = null;
		CustomMapLoader.loadedMapModId = 0L;
		CustomMapLoader.loadedSceneFilePaths.Clear();
		CustomMapLoader.loadedSceneNames.Clear();
		CustomMapLoader.loadedSceneIndexes.Clear();
		CustomMapLoader.initialSceneIndexes.Clear();
		CustomMapLoader.initialSceneNames.Clear();
		CustomMapLoader.maxPlayersForMap = 20;
		CustomMapLoader.gravityZoneCount = 0;
		CustomMapLoader.sizeChangerCount = 0;
		CustomMapLoader.handHoldCount = 0;
		CustomMapLoader.mapperAssetCount = 0;
		CustomMapModeSelector.ResetButtons();
		if (RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
		{
			if (GameMode.ActiveGameMode.IsNull())
			{
				GameMode.ChangeGameMode(GameModeType.Casual.ToString());
			}
			else if (GameMode.ActiveGameMode.GameType() != GameModeType.Casual)
			{
				GameMode.ChangeGameMode(GameModeType.Casual.ToString());
			}
		}
		CustomMapLoader.shouldAbortMapLoading = false;
		CustomMapLoader.shouldAbortSceneLoad = false;
		CustomMapLoader.isUnloading = false;
		if (CustomMapLoader.unloadMapCallback != null)
		{
			Action action = CustomMapLoader.unloadMapCallback;
			if (action != null)
			{
				action();
			}
			CustomMapLoader.unloadMapCallback = null;
		}
		yield break;
	}

	// Token: 0x06004406 RID: 17414 RVA: 0x0016BCB4 File Offset: 0x00169EB4
	private static IEnumerator AbortSceneLoad(int sceneIndex)
	{
		if (sceneIndex == -1)
		{
			CustomMapLoader.shouldAbortMapLoading = true;
		}
		CustomMapLoader.isLoading = false;
		if (CustomMapLoader.shouldAbortMapLoading)
		{
			yield return CustomMapLoader.UnloadMapCoroutine();
		}
		else
		{
			yield return CustomMapLoader.UnloadSceneCoroutine(sceneIndex, null);
		}
		CustomMapLoader.shouldAbortSceneLoad = false;
		yield break;
	}

	// Token: 0x06004407 RID: 17415 RVA: 0x0016BCC3 File Offset: 0x00169EC3
	private static IEnumerator UnloadScenesCoroutine(int[] sceneIndexes)
	{
		int num;
		for (int i = 0; i < sceneIndexes.Length; i = num + 1)
		{
			yield return CustomMapLoader.UnloadSceneCoroutine(sceneIndexes[i], null);
			num = i;
		}
		yield break;
	}

	// Token: 0x06004408 RID: 17416 RVA: 0x0016BCD2 File Offset: 0x00169ED2
	private static IEnumerator UnloadSceneCoroutine(int sceneIndex, Action OnUnloadComplete = null)
	{
		if (!CustomMapLoader.hasInstance)
		{
			yield break;
		}
		if (sceneIndex < 0 || sceneIndex >= CustomMapLoader.assetBundleSceneFilePaths.Length)
		{
			Debug.LogError(string.Format("[CustomMapLoader::UnloadSceneCoroutine] SceneIndex of {0} is invalid! ", sceneIndex) + string.Format("The currently loaded AssetBundle contains {0} scenes.", CustomMapLoader.assetBundleSceneFilePaths.Length));
			yield break;
		}
		while (CustomMapLoader.runningAsyncLoad)
		{
			yield return null;
		}
		UnloadSceneOptions unloadSceneOptions = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects;
		string scenePathWithExtension = CustomMapLoader.assetBundleSceneFilePaths[sceneIndex];
		string[] array = scenePathWithExtension.Split(".", StringSplitOptions.None);
		string text = "";
		string sceneName = "";
		if (!array.IsNullOrEmpty<string>())
		{
			text = array[0];
			if (text.Length > 0)
			{
				sceneName = Path.GetFileName(text);
			}
		}
		Scene sceneByName = SceneManager.GetSceneByName(text);
		if (sceneByName.IsValid())
		{
			CustomMapLoader.RemoveUnloadingStorePrefabs(sceneByName);
			for (int i = CustomMapLoader.teleporters.Count - 1; i >= 0; i--)
			{
				if (CustomMapLoader.teleporters[i].gameObject.scene == sceneByName)
				{
					CustomMapLoader.teleporters.RemoveAt(i);
				}
			}
			AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scenePathWithExtension, unloadSceneOptions);
			yield return asyncOperation;
			CustomMapLoader.loadedSceneFilePaths.Remove(scenePathWithExtension);
			CustomMapLoader.loadedSceneNames.Remove(sceneName);
			CustomMapLoader.loadedSceneIndexes.Remove(sceneIndex);
			ZoneManagement.RemoveSceneFromForceStayLoaded(sceneName);
			Action<string> action = CustomMapLoader.sceneUnloadedCallback;
			if (action != null)
			{
				action(sceneName);
			}
			if (OnUnloadComplete != null)
			{
				OnUnloadComplete();
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x06004409 RID: 17417 RVA: 0x0016BCE8 File Offset: 0x00169EE8
	private static void RemoveUnloadingStorePrefabs(Scene unloadingScene)
	{
		if (CustomMapLoader.customMapATM.IsNotNull())
		{
			ATM_UI componentInChildren = CustomMapLoader.customMapATM.GetComponentInChildren<ATM_UI>();
			if (componentInChildren.IsNotNull() && componentInChildren.IsFromCustomMapScene(unloadingScene) && ATM_Manager.instance.IsNotNull())
			{
				ATM_Manager.instance.RemoveATM(componentInChildren);
				ATM_Manager.instance.SetTemporaryCreatorCode(null);
			}
			Object.Destroy(CustomMapLoader.customMapATM);
			CustomMapLoader.customMapATM = null;
		}
		for (int i = CustomMapLoader.storeDisplayStands.Count - 1; i >= 0; i--)
		{
			if (CustomMapLoader.storeDisplayStands[i].IsNull())
			{
				CustomMapLoader.storeDisplayStands.RemoveAt(i);
			}
			else
			{
				DynamicCosmeticStand componentInChildren2 = CustomMapLoader.storeDisplayStands[i].GetComponentInChildren<DynamicCosmeticStand>();
				if (componentInChildren2.IsNotNull() && componentInChildren2.IsFromCustomMapScene(unloadingScene))
				{
					if (componentInChildren2.IsNotNull())
					{
						StoreController.instance.RemoveStandFromPlayFabIDDictionary(componentInChildren2);
					}
					Object.Destroy(CustomMapLoader.storeDisplayStands[i]);
					CustomMapLoader.storeDisplayStands.RemoveAt(i);
				}
			}
		}
		for (int i = CustomMapLoader.storeCheckouts.Count - 1; i >= 0; i--)
		{
			if (CustomMapLoader.storeCheckouts[i].IsNull())
			{
				CustomMapLoader.storeCheckouts.RemoveAt(i);
			}
			else
			{
				ItemCheckout componentInChildren3 = CustomMapLoader.storeCheckouts[i].GetComponentInChildren<ItemCheckout>();
				if (componentInChildren3.IsNotNull() && componentInChildren3.IsFromScene(unloadingScene))
				{
					componentInChildren3.RemoveFromCustomMap(CustomMapLoader.instance.compositeTryOnArea);
					CosmeticsController.instance.RemoveItemCheckout(componentInChildren3);
					Object.Destroy(CustomMapLoader.storeCheckouts[i]);
					CustomMapLoader.storeCheckouts.RemoveAt(i);
				}
			}
		}
		for (int i = CustomMapLoader.storeTryOnConsoles.Count - 1; i >= 0; i--)
		{
			if (CustomMapLoader.storeTryOnConsoles[i].IsNull())
			{
				CustomMapLoader.storeTryOnConsoles.RemoveAt(i);
			}
			else if (CustomMapLoader.storeTryOnConsoles[i].scene.Equals(unloadingScene))
			{
				FittingRoom componentInChildren4 = CustomMapLoader.storeTryOnConsoles[i].GetComponentInChildren<FittingRoom>();
				if (componentInChildren4.IsNotNull())
				{
					CosmeticsController.instance.RemoveFittingRoom(componentInChildren4);
				}
				CustomMapLoader.storeTryOnConsoles.RemoveAt(i);
			}
		}
		for (int i = CustomMapLoader.storeTryOnAreas.Count - 1; i >= 0; i--)
		{
			if (CustomMapLoader.storeTryOnAreas[i].IsNull())
			{
				CustomMapLoader.storeTryOnAreas.RemoveAt(i);
			}
			else
			{
				CMSTryOnArea component = CustomMapLoader.storeTryOnAreas[i].GetComponent<CMSTryOnArea>();
				if (component.IsNotNull() && component.IsFromScene(unloadingScene))
				{
					component.RemoveFromCustomMap(CustomMapLoader.instance.compositeTryOnArea);
					Object.Destroy(CustomMapLoader.storeTryOnAreas[i]);
					CustomMapLoader.storeTryOnAreas.RemoveAt(i);
				}
			}
		}
	}

	// Token: 0x0600440A RID: 17418 RVA: 0x0016BF94 File Offset: 0x0016A194
	private static void CleanupPlaceholders()
	{
		for (int i = 0; i < CustomMapLoader.instance.leafGliders.Length; i++)
		{
			CustomMapLoader.instance.leafGliders[i].CustomMapUnload();
			CustomMapLoader.instance.leafGliders[i].enabled = false;
			CustomMapLoader.instance.leafGliders[i].transform.GetChild(0).gameObject.SetActive(false);
		}
	}

	// Token: 0x0600440B RID: 17419 RVA: 0x0016C005 File Offset: 0x0016A205
	private static IEnumerator ResetLightmaps()
	{
		CustomMapLoader.instance.dayNightManager.RequestRepopulateLightmaps();
		LoadSceneParameters loadSceneParameters = new LoadSceneParameters
		{
			loadSceneMode = LoadSceneMode.Additive,
			localPhysicsMode = LocalPhysicsMode.None
		};
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(10, loadSceneParameters);
		yield return asyncOperation;
		asyncOperation = SceneManager.UnloadSceneAsync(10);
		yield return asyncOperation;
		yield break;
	}

	// Token: 0x0600440C RID: 17420 RVA: 0x0016C010 File Offset: 0x0016A210
	private static void UnloadLightmaps()
	{
		foreach (LightmapData lightmapData in LightmapSettings.lightmaps)
		{
			if (lightmapData.lightmapColor != null && !CustomMapLoader.lightmapsToKeep.Contains(lightmapData.lightmapColor))
			{
				Resources.UnloadAsset(lightmapData.lightmapColor);
			}
			if (lightmapData.lightmapDir != null && !CustomMapLoader.lightmapsToKeep.Contains(lightmapData.lightmapDir))
			{
				Resources.UnloadAsset(lightmapData.lightmapDir);
			}
		}
	}

	// Token: 0x0600440D RID: 17421 RVA: 0x0016C08C File Offset: 0x0016A28C
	private static int GetSceneIndex(string sceneName)
	{
		int num = -1;
		if (CustomMapLoader.assetBundleSceneFilePaths.Length == 1)
		{
			return 0;
		}
		for (int i = 0; i < CustomMapLoader.assetBundleSceneFilePaths.Length; i++)
		{
			string sceneNameFromFilePath = CustomMapLoader.GetSceneNameFromFilePath(CustomMapLoader.assetBundleSceneFilePaths[i]);
			if (sceneNameFromFilePath != null && sceneNameFromFilePath.Equals(sceneName))
			{
				num = i;
				break;
			}
		}
		return num;
	}

	// Token: 0x0600440E RID: 17422 RVA: 0x0016C0DB File Offset: 0x0016A2DB
	private static string GetSceneNameFromFilePath(string filePath)
	{
		string[] array = filePath.Split("/", StringSplitOptions.None);
		return array[array.Length - 1].Split(".", StringSplitOptions.None)[0];
	}

	// Token: 0x0600440F RID: 17423 RVA: 0x0016C0FC File Offset: 0x0016A2FC
	public static MapPackageInfo GetPackageInfo(string packageInfoFilePath)
	{
		MapPackageInfo mapPackageInfo;
		using (StreamReader streamReader = new StreamReader(File.OpenRead(packageInfoFilePath), Encoding.Default))
		{
			mapPackageInfo = JsonConvert.DeserializeObject<MapPackageInfo>(streamReader.ReadToEnd());
		}
		return mapPackageInfo;
	}

	// Token: 0x17000667 RID: 1639
	// (get) Token: 0x06004410 RID: 17424 RVA: 0x0016C144 File Offset: 0x0016A344
	public static ModId LoadedMapModId
	{
		get
		{
			return CustomMapLoader.loadedMapModId;
		}
	}

	// Token: 0x17000668 RID: 1640
	// (get) Token: 0x06004411 RID: 17425 RVA: 0x0016C14B File Offset: 0x0016A34B
	public static long LoadedMapModFileId
	{
		get
		{
			return CustomMapLoader.loadedMapModFileId;
		}
	}

	// Token: 0x17000669 RID: 1641
	// (get) Token: 0x06004412 RID: 17426 RVA: 0x0016C152 File Offset: 0x0016A352
	public static int LoadedMapSupportVersion
	{
		get
		{
			MapPackageInfo mapPackageInfo = CustomMapLoader.loadedMapPackageInfo;
			if (mapPackageInfo == null)
			{
				return 0;
			}
			return mapPackageInfo.customMapSupportVersion;
		}
	}

	// Token: 0x1700066A RID: 1642
	// (get) Token: 0x06004413 RID: 17427 RVA: 0x0016C164 File Offset: 0x0016A364
	public static int LoadedMapGravityZoneCount
	{
		get
		{
			return CustomMapLoader.gravityZoneCount;
		}
	}

	// Token: 0x1700066B RID: 1643
	// (get) Token: 0x06004414 RID: 17428 RVA: 0x0016C16B File Offset: 0x0016A36B
	public static int LoadedMapSizeChangerCount
	{
		get
		{
			return CustomMapLoader.sizeChangerCount;
		}
	}

	// Token: 0x1700066C RID: 1644
	// (get) Token: 0x06004415 RID: 17429 RVA: 0x0016C172 File Offset: 0x0016A372
	public static int LoadedMapHandHoldCount
	{
		get
		{
			return CustomMapLoader.handHoldCount;
		}
	}

	// Token: 0x1700066D RID: 1645
	// (get) Token: 0x06004416 RID: 17430 RVA: 0x0016C179 File Offset: 0x0016A379
	public static int LoadedMapMapperAssetCount
	{
		get
		{
			return CustomMapLoader.mapperAssetCount;
		}
	}

	// Token: 0x1700066E RID: 1646
	// (get) Token: 0x06004418 RID: 17432 RVA: 0x0016C188 File Offset: 0x0016A388
	// (set) Token: 0x06004417 RID: 17431 RVA: 0x0016C180 File Offset: 0x0016A380
	public static bool CanLoadEntities { get; private set; }

	// Token: 0x06004419 RID: 17433 RVA: 0x0016C18F File Offset: 0x0016A38F
	public static bool IsMapLoaded()
	{
		return CustomMapLoader.IsMapLoaded(ModId.Null);
	}

	// Token: 0x0600441A RID: 17434 RVA: 0x0016C19C File Offset: 0x0016A39C
	public static bool IsMapLoaded(ModId mapModId)
	{
		if (mapModId.IsValid())
		{
			return !CustomMapLoader.IsLoading() && CustomMapLoader.LoadedMapModId == mapModId;
		}
		return !CustomMapLoader.IsLoading() && CustomMapLoader.LoadedMapModId.IsValid();
	}

	// Token: 0x0600441B RID: 17435 RVA: 0x0016C1DD File Offset: 0x0016A3DD
	public static bool IsLoading()
	{
		return CustomMapLoader.isLoading;
	}

	// Token: 0x0600441C RID: 17436 RVA: 0x0016C1E4 File Offset: 0x0016A3E4
	public static long GetLoadingMapModId()
	{
		return CustomMapLoader.attemptedLoadID;
	}

	// Token: 0x0600441D RID: 17437 RVA: 0x0016C1EB File Offset: 0x0016A3EB
	public static byte GetRoomSizeForCurrentlyLoadedMap()
	{
		if (!CustomMapLoader.IsMapLoaded())
		{
			return 20;
		}
		return CustomMapLoader.maxPlayersForMap;
	}

	// Token: 0x0600441E RID: 17438 RVA: 0x0016C1FC File Offset: 0x0016A3FC
	public static bool IsCustomScene(string sceneName)
	{
		return CustomMapLoader.loadedSceneNames.Contains(sceneName);
	}

	// Token: 0x0600441F RID: 17439 RVA: 0x0016C209 File Offset: 0x0016A409
	public static string GetLuauGamemodeScript()
	{
		if (!CustomMapLoader.IsMapLoaded())
		{
			return "";
		}
		return CustomMapLoader.cachedLuauScript;
	}

	// Token: 0x06004420 RID: 17440 RVA: 0x0016C21D File Offset: 0x0016A41D
	public static bool IsDevModeEnabled()
	{
		return CustomMapLoader.IsMapLoaded() && CustomMapLoader.devModeEnabled;
	}

	// Token: 0x06004421 RID: 17441 RVA: 0x0016C22D File Offset: 0x0016A42D
	public static Transform GetCustomMapsDefaultSpawnLocation()
	{
		if (CustomMapLoader.hasInstance)
		{
			return CustomMapLoader.instance.CustomMapsDefaultSpawnLocation;
		}
		return null;
	}

	// Token: 0x06004422 RID: 17442 RVA: 0x0016C244 File Offset: 0x0016A444
	public static bool LoadedMapWantsHoldingHandsDisabled()
	{
		return CustomMapLoader.IsMapLoaded() && (CustomMapLoader.disableHoldingHandsAllModes || (CustomMapLoader.disableHoldingHandsCustomMode && GorillaGameManager.instance.IsNotNull() && GorillaGameManager.instance.GameType() == GameModeType.Custom));
	}

	// Token: 0x06004423 RID: 17443 RVA: 0x0016C27B File Offset: 0x0016A47B
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.defaultNexusGroupId == null)
		{
			Debug.LogError("You have to set defaultNexusGroupId in " + base.name + " or things will not work!");
			return false;
		}
		return true;
	}

	// Token: 0x040055D4 RID: 21972
	[SerializeField]
	private NexusGroupId defaultNexusGroupId;

	// Token: 0x040055D5 RID: 21973
	[OnEnterPlay_SetNull]
	private static volatile CustomMapLoader instance;

	// Token: 0x040055D6 RID: 21974
	[OnEnterPlay_Set(false)]
	private static bool hasInstance;

	// Token: 0x040055D7 RID: 21975
	public Transform CustomMapsDefaultSpawnLocation;

	// Token: 0x040055D8 RID: 21976
	public CustomMapAccessDoor accessDoor;

	// Token: 0x040055D9 RID: 21977
	[FormerlySerializedAs("networkTrigger")]
	public GameObject publicJoinTrigger;

	// Token: 0x040055DA RID: 21978
	[SerializeField]
	private BetterDayNightManager dayNightManager;

	// Token: 0x040055DB RID: 21979
	[SerializeField]
	private GhostReactorManager ghostReactorManager;

	// Token: 0x040055DC RID: 21980
	[SerializeField]
	private GameObject placeholderParent;

	// Token: 0x040055DD RID: 21981
	[SerializeField]
	private GliderHoldable[] leafGliders;

	// Token: 0x040055DE RID: 21982
	[SerializeField]
	private GameObject leafGlider;

	// Token: 0x040055DF RID: 21983
	[SerializeField]
	private GameObject gliderWindVolume;

	// Token: 0x040055E0 RID: 21984
	[FormerlySerializedAs("waterVolume")]
	[SerializeField]
	private GameObject waterVolumePrefab;

	// Token: 0x040055E1 RID: 21985
	[SerializeField]
	private WaterParameters defaultWaterParameters;

	// Token: 0x040055E2 RID: 21986
	[SerializeField]
	private WaterParameters defaultLavaParameters;

	// Token: 0x040055E3 RID: 21987
	[FormerlySerializedAs("forceVolume")]
	[SerializeField]
	private GameObject forceVolumePrefab;

	// Token: 0x040055E4 RID: 21988
	[SerializeField]
	private GameObject atmPrefab;

	// Token: 0x040055E5 RID: 21989
	[SerializeField]
	private GameObject atmNoShellPrefab;

	// Token: 0x040055E6 RID: 21990
	[SerializeField]
	private GameObject storeDisplayStandPrefab;

	// Token: 0x040055E7 RID: 21991
	[SerializeField]
	private GameObject storeCheckoutCounterPrefab;

	// Token: 0x040055E8 RID: 21992
	[SerializeField]
	private GameObject storeTryOnConsolePrefab;

	// Token: 0x040055E9 RID: 21993
	[SerializeField]
	private GameObject storeTryOnAreaPrefab;

	// Token: 0x040055EA RID: 21994
	[SerializeField]
	private GameObject hoverboardDispenserPrefab;

	// Token: 0x040055EB RID: 21995
	[SerializeField]
	private GameObject ropeSwingPrefab;

	// Token: 0x040055EC RID: 21996
	[SerializeField]
	private GameObject ziplinePrefab;

	// Token: 0x040055ED RID: 21997
	[SerializeField]
	private GameObject reviveStationPrefab;

	// Token: 0x040055EE RID: 21998
	[SerializeField]
	private GameObject zoneShaderSettingsTrigger;

	// Token: 0x040055EF RID: 21999
	[SerializeField]
	private AudioMixerGroup masterAudioMixer;

	// Token: 0x040055F0 RID: 22000
	[SerializeField]
	private ZoneShaderSettings customMapZoneShaderSettings;

	// Token: 0x040055F1 RID: 22001
	[SerializeField]
	private CompositeTriggerEvents compositeTryOnArea;

	// Token: 0x040055F2 RID: 22002
	[SerializeField]
	private GameObject virtualStumpMesh;

	// Token: 0x040055F3 RID: 22003
	[SerializeField]
	private List<GameModeType> availableModesForOldMaps = new List<GameModeType>
	{
		GameModeType.Infection,
		GameModeType.FreezeTag,
		GameModeType.Paintbrawl
	};

	// Token: 0x040055F4 RID: 22004
	[SerializeField]
	private GameModeType defaultGameModeForNonCustomOldMaps = GameModeType.Infection;

	// Token: 0x040055F5 RID: 22005
	public TMP_FontAsset DefaultFont;

	// Token: 0x040055F6 RID: 22006
	private static readonly int numObjectsToProcessPerFrame = 5;

	// Token: 0x040055F7 RID: 22007
	private static readonly List<int> APPROVED_LAYERS = new List<int>
	{
		0, 1, 2, 4, 5, 9, 11, 18, 20, 22,
		27, 30
	};

	// Token: 0x040055F8 RID: 22008
	private static bool isLoading;

	// Token: 0x040055F9 RID: 22009
	private static bool isUnloading;

	// Token: 0x040055FA RID: 22010
	private static bool runningAsyncLoad = false;

	// Token: 0x040055FB RID: 22011
	private static long attemptedLoadID = 0L;

	// Token: 0x040055FC RID: 22012
	private static string attemptedSceneToLoad;

	// Token: 0x040055FD RID: 22013
	private static bool shouldAbortMapLoading = false;

	// Token: 0x040055FE RID: 22014
	private static bool shouldAbortSceneLoad = false;

	// Token: 0x040055FF RID: 22015
	private static bool errorEncounteredDuringLoad = false;

	// Token: 0x04005600 RID: 22016
	private static Action unloadMapCallback;

	// Token: 0x04005601 RID: 22017
	private static string cachedExceptionMessage = "";

	// Token: 0x04005602 RID: 22018
	private static AssetBundle mapBundle;

	// Token: 0x04005603 RID: 22019
	private static List<string> initialSceneNames = new List<string>();

	// Token: 0x04005604 RID: 22020
	private static List<int> initialSceneIndexes = new List<int>();

	// Token: 0x04005605 RID: 22021
	private static byte maxPlayersForMap = 20;

	// Token: 0x04005606 RID: 22022
	private static ModId loadedMapModId;

	// Token: 0x04005607 RID: 22023
	private static long loadedMapModFileId;

	// Token: 0x04005608 RID: 22024
	private static MapPackageInfo loadedMapPackageInfo;

	// Token: 0x04005609 RID: 22025
	private static string cachedLuauScript;

	// Token: 0x0400560A RID: 22026
	private static bool devModeEnabled;

	// Token: 0x0400560B RID: 22027
	private static bool disableHoldingHandsAllModes;

	// Token: 0x0400560C RID: 22028
	private static bool disableHoldingHandsCustomMode;

	// Token: 0x0400560D RID: 22029
	private static Action<MapLoadStatus, int, string> mapLoadProgressCallback;

	// Token: 0x0400560E RID: 22030
	private static Action<bool> mapLoadFinishedCallback;

	// Token: 0x0400560F RID: 22031
	private static Coroutine zoneLoadingCoroutine;

	// Token: 0x04005610 RID: 22032
	private static Action<string> sceneLoadedCallback;

	// Token: 0x04005611 RID: 22033
	private static Action<string> sceneUnloadedCallback;

	// Token: 0x04005612 RID: 22034
	private static List<CustomMapLoader.LoadZoneRequest> queuedLoadZoneRequests = new List<CustomMapLoader.LoadZoneRequest>();

	// Token: 0x04005613 RID: 22035
	private static string[] assetBundleSceneFilePaths;

	// Token: 0x04005614 RID: 22036
	private static List<string> loadedSceneFilePaths = new List<string>();

	// Token: 0x04005615 RID: 22037
	private static List<string> loadedSceneNames = new List<string>();

	// Token: 0x04005616 RID: 22038
	private static List<int> loadedSceneIndexes = new List<int>();

	// Token: 0x04005617 RID: 22039
	private Coroutine loadScenesCoroutine;

	// Token: 0x04005618 RID: 22040
	private static int leafGliderIndex;

	// Token: 0x04005619 RID: 22041
	private static int gravityZoneCount;

	// Token: 0x0400561A RID: 22042
	private static int sizeChangerCount;

	// Token: 0x0400561B RID: 22043
	private static int handHoldCount;

	// Token: 0x0400561C RID: 22044
	private static int mapperAssetCount;

	// Token: 0x0400561D RID: 22045
	private static bool usingDynamicLighting = false;

	// Token: 0x0400561E RID: 22046
	private static bool refreshReviveStations = false;

	// Token: 0x0400561F RID: 22047
	private static int totalObjectsInLoadingScene = 0;

	// Token: 0x04005620 RID: 22048
	private static int objectsProcessedForLoadingScene = 0;

	// Token: 0x04005621 RID: 22049
	private static int objectsProcessedThisFrame = 0;

	// Token: 0x04005622 RID: 22050
	private static List<Component> initializePhaseTwoComponents = new List<Component>();

	// Token: 0x04005623 RID: 22051
	private static List<MapEntity> entitiesToCreate = new List<MapEntity>(Constants.aiAgentLimit);

	// Token: 0x04005624 RID: 22052
	[TupleElementNames(new string[] { "settings", "alwaysInZoneSettings" })]
	private static List<ValueTuple<MonkeGravityControllerSettings, BasicGravityZoneSettings>> monkeGravityControllersToReplace = new List<ValueTuple<MonkeGravityControllerSettings, BasicGravityZoneSettings>>();

	// Token: 0x04005625 RID: 22053
	private static Dictionary<BasicGravityZoneSettings, BasicGravityZone> replacedGravityZones = new Dictionary<BasicGravityZoneSettings, BasicGravityZone>();

	// Token: 0x04005626 RID: 22054
	private static LightmapData[] lightmaps;

	// Token: 0x04005627 RID: 22055
	private static List<Texture2D> lightmapsToKeep = new List<Texture2D>();

	// Token: 0x04005628 RID: 22056
	private static List<GameObject> placeholderReplacements = new List<GameObject>();

	// Token: 0x04005629 RID: 22057
	private static GameObject customMapATM = null;

	// Token: 0x0400562A RID: 22058
	private static List<GameObject> storeCheckouts = new List<GameObject>();

	// Token: 0x0400562B RID: 22059
	private static List<GameObject> storeDisplayStands = new List<GameObject>();

	// Token: 0x0400562C RID: 22060
	private static List<GameObject> storeTryOnConsoles = new List<GameObject>();

	// Token: 0x0400562D RID: 22061
	private static List<GameObject> storeTryOnAreas = new List<GameObject>();

	// Token: 0x0400562E RID: 22062
	private static List<Component> teleporters = new List<Component>();

	// Token: 0x0400562F RID: 22063
	private string dontDestroyOnLoadSceneName = "";

	// Token: 0x04005630 RID: 22064
	private static readonly List<Type> componentAllowlist = new List<Type>
	{
		typeof(MeshRenderer),
		typeof(Transform),
		typeof(MeshFilter),
		typeof(MeshRenderer),
		typeof(Collider),
		typeof(BoxCollider),
		typeof(SphereCollider),
		typeof(CapsuleCollider),
		typeof(MeshCollider),
		typeof(Light),
		typeof(ReflectionProbe),
		typeof(AudioSource),
		typeof(Animator),
		typeof(SkinnedMeshRenderer),
		typeof(TextMesh),
		typeof(ParticleSystem),
		typeof(ParticleSystemRenderer),
		typeof(RectTransform),
		typeof(SpriteRenderer),
		typeof(BillboardRenderer),
		typeof(Canvas),
		typeof(CanvasRenderer),
		typeof(CanvasScaler),
		typeof(GraphicRaycaster),
		typeof(Rigidbody),
		typeof(TrailRenderer),
		typeof(LineRenderer),
		typeof(LensFlareComponentSRP),
		typeof(Camera),
		typeof(UniversalAdditionalCameraData),
		typeof(NavMeshAgent),
		typeof(NavMesh),
		typeof(NavMeshObstacle),
		typeof(NavMeshLink),
		typeof(NavMeshModifierVolume),
		typeof(NavMeshModifier),
		typeof(NavMeshSurface),
		typeof(HingeJoint),
		typeof(ConstantForce),
		typeof(LODGroup),
		typeof(MapDescriptor),
		typeof(AccessDoorPlaceholder),
		typeof(MapOrientationPoint),
		typeof(SurfaceOverrideSettings),
		typeof(TeleporterSettings),
		typeof(TagZoneSettings),
		typeof(LuauTriggerSettings),
		typeof(MapBoundarySettings),
		typeof(ObjectActivationTriggerSettings),
		typeof(LoadZoneSettings),
		typeof(GTObjectPlaceholder),
		typeof(CMSZoneShaderSettings),
		typeof(ZoneShaderTriggerSettings),
		typeof(MultiPartFire),
		typeof(HandHoldSettings),
		typeof(CustomMapEjectButtonSettings),
		typeof(global::CustomMapSupport.BezierSpline),
		typeof(UberShaderDynamicLight),
		typeof(MapEntity),
		typeof(GrabbableEntity),
		typeof(AIAgent),
		typeof(AISpawnManager),
		typeof(AISpawnPoint),
		typeof(MapSpawnPoint),
		typeof(MapSpawnManager),
		typeof(RopeSwingSegment),
		typeof(ZiplineSegment),
		typeof(PlayAnimationTriggerSettings),
		typeof(SurfaceMoverSettings),
		typeof(MovingSurfaceSettings),
		typeof(CustomMapReviveStation),
		typeof(BasicGravityZoneSettings),
		typeof(ConsensusGravityZoneSettings),
		typeof(TorusZoneSettings),
		typeof(PlanetZoneSettings),
		typeof(CubicPlanetZoneSettings),
		typeof(SizeChangerSettings),
		typeof(MonkeGravityControllerSettings),
		typeof(ProBuilderMesh),
		typeof(TMP_Text),
		typeof(TextMeshPro),
		typeof(TextMeshProUGUI),
		typeof(UniversalAdditionalLightData),
		typeof(BakerySkyLight),
		typeof(BakeryDirectLight),
		typeof(BakeryPointLight),
		typeof(ftLightmapsStorage),
		typeof(BakeryAlwaysRender),
		typeof(BakeryLightMesh),
		typeof(BakeryLightmapGroupSelector),
		typeof(BakeryPackAsSingleSquare),
		typeof(BakerySector),
		typeof(BakeryVolume),
		typeof(BakeryLightmapGroup)
	};

	// Token: 0x04005631 RID: 22065
	private static readonly List<string> componentTypeStringAllowList = new List<string> { "UnityEngine.Halo" };

	// Token: 0x04005632 RID: 22066
	private static readonly Type[] badComponents = new Type[]
	{
		typeof(EventTrigger),
		typeof(UIBehaviour),
		typeof(GorillaPressableButton),
		typeof(GorillaPressableDelayButton),
		typeof(Camera),
		typeof(AudioListener),
		typeof(VideoPlayer)
	};

	// Token: 0x04005633 RID: 22067
	public const long LocalMapModId = 999999999L;

	// Token: 0x02000A56 RID: 2646
	private struct LoadZoneRequest
	{
		// Token: 0x04005635 RID: 22069
		public int[] sceneIndexesToLoad;

		// Token: 0x04005636 RID: 22070
		public int[] sceneIndexesToUnload;

		// Token: 0x04005637 RID: 22071
		public Action<string> onSceneLoadedCallback;

		// Token: 0x04005638 RID: 22072
		public Action<string> onSceneUnloadedCallback;
	}
}
