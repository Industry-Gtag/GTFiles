using System;
using System.Collections;
using Modio.Mods;
using Modio.Users;
using Unity.Profiling;
using UnityEngine;

// Token: 0x02000A6A RID: 2666
public class CustomMapTelemetry : MonoBehaviour
{
	// Token: 0x1700068B RID: 1675
	// (get) Token: 0x06004484 RID: 17540 RVA: 0x0016E867 File Offset: 0x0016CA67
	public static bool IsActive
	{
		get
		{
			return CustomMapTelemetry.metricsCaptureStarted || CustomMapTelemetry.perfCaptureStarted;
		}
	}

	// Token: 0x06004485 RID: 17541 RVA: 0x0016E877 File Offset: 0x0016CA77
	private void Awake()
	{
		if (CustomMapTelemetry.instance == null)
		{
			CustomMapTelemetry.instance = this;
			return;
		}
		if (CustomMapTelemetry.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x1700068C RID: 1676
	// (get) Token: 0x06004486 RID: 17542 RVA: 0x0016E8AC File Offset: 0x0016CAAC
	public static string CurrentMapIdString
	{
		get
		{
			if (!CustomMapLoader.IsMapLoaded())
			{
				return string.Empty;
			}
			return CustomMapLoader.LoadedMapModId._id.ToString();
		}
	}

	// Token: 0x1700068D RID: 1677
	// (get) Token: 0x06004487 RID: 17543 RVA: 0x0016E8D8 File Offset: 0x0016CAD8
	public static string CurrentMapSourceString
	{
		get
		{
			return CustomMapTelemetry.currentMapSource.GetName<GTMapLoadSource>();
		}
	}

	// Token: 0x06004488 RID: 17544 RVA: 0x0016E8E4 File Offset: 0x0016CAE4
	public static void SetLoadingMapInfo(Mod mod, GTMapLoadSource source)
	{
		CustomMapTelemetry.currentMapMod = mod;
		CustomMapTelemetry.currentMapSource = source;
	}

	// Token: 0x06004489 RID: 17545 RVA: 0x0016E8F2 File Offset: 0x0016CAF2
	public static void ClearLoadingMapInfo()
	{
		if (CustomMapTelemetry.currentMapMod != null && CustomMapLoader.IsMapLoaded(CustomMapTelemetry.currentMapMod.Id))
		{
			return;
		}
		CustomMapTelemetry.currentMapMod = null;
		CustomMapTelemetry.currentMapSource = GTMapLoadSource.none;
	}

	// Token: 0x0600448A RID: 17546 RVA: 0x0016E91C File Offset: 0x0016CB1C
	public static void OnMapLoadCompleted()
	{
		if (CustomMapTelemetry.currentMapMod == null || !CustomMapLoader.IsMapLoaded())
		{
			return;
		}
		long id = CustomMapLoader.LoadedMapModId._id;
		if (id == 999999999L || CustomMapTelemetry.currentMapMod.Id._id != id)
		{
			return;
		}
		string[] array;
		if (CustomMapTelemetry.currentMapMod.Tags != null)
		{
			array = new string[CustomMapTelemetry.currentMapMod.Tags.Length];
			for (int i = 0; i < CustomMapTelemetry.currentMapMod.Tags.Length; i++)
			{
				string[] array2 = array;
				int num = i;
				ModTag modTag = CustomMapTelemetry.currentMapMod.Tags[i];
				array2[num] = ((modTag != null) ? modTag.ApiName : null) ?? "";
			}
		}
		else
		{
			array = Array.Empty<string>();
		}
		long num2 = id;
		string name = CustomMapTelemetry.currentMapMod.Name;
		UserProfile creator = CustomMapTelemetry.currentMapMod.Creator;
		long num3 = ((creator != null) ? creator.UserId : 0L);
		UserProfile creator2 = CustomMapTelemetry.currentMapMod.Creator;
		GorillaTelemetry.PostCustomMapRegistryEvent(num2, name, num3, ((creator2 != null) ? creator2.Username : null) ?? "", CustomMapTelemetry.currentMapMod.DateLive, CustomMapTelemetry.currentMapMod.DateUpdated, array, CustomMapLoader.LoadedMapSupportVersion, (int)CustomMapLoader.GetRoomSizeForCurrentlyLoadedMap(), !string.IsNullOrEmpty(CustomMapLoader.GetLuauGamemodeScript()), CustomMapLoader.LoadedMapGravityZoneCount, CustomMapLoader.LoadedMapSizeChangerCount, CustomMapLoader.LoadedMapHandHoldCount, CustomMapLoader.LoadedMapMapperAssetCount);
	}

	// Token: 0x0600448B RID: 17547 RVA: 0x0016EA46 File Offset: 0x0016CC46
	public static void OnMapUnloaded()
	{
		CustomMapTelemetry.OnPlayerLeftMap();
		CustomMapTelemetry.ClearLoadingMapInfo();
	}

	// Token: 0x0600448C RID: 17548 RVA: 0x0016EA54 File Offset: 0x0016CC54
	public static void OnPlayerEnteredMap()
	{
		if (CustomMapTelemetry.playerInMap || !CustomMapLoader.IsMapLoaded())
		{
			return;
		}
		long id = CustomMapLoader.LoadedMapModId._id;
		string name = CustomMapTelemetry.currentMapSource.GetName<GTMapLoadSource>();
		if (!GorillaTelemetry.PostCustomMapZoneEvent(GTZoneEventType.zone_enter, id, name))
		{
			return;
		}
		CustomMapTelemetry.playerInMap = true;
		CustomMapTelemetry.enteredMapId = id;
		CustomMapTelemetry.enteredMapSource = name;
	}

	// Token: 0x0600448D RID: 17549 RVA: 0x0016EAA3 File Offset: 0x0016CCA3
	public static void OnPlayerLeftMap()
	{
		if (!CustomMapTelemetry.playerInMap)
		{
			return;
		}
		CustomMapTelemetry.playerInMap = false;
		GorillaTelemetry.PostCustomMapZoneEvent(GTZoneEventType.zone_exit, CustomMapTelemetry.enteredMapId, CustomMapTelemetry.enteredMapSource);
	}

	// Token: 0x0600448E RID: 17550 RVA: 0x0016EAC4 File Offset: 0x0016CCC4
	private static void OnPlayerJoinedRoom(NetPlayer obj)
	{
		CustomMapTelemetry.runningPlayerCount++;
		CustomMapTelemetry.maxPlayersInMap = Math.Max(CustomMapTelemetry.runningPlayerCount, CustomMapTelemetry.maxPlayersInMap);
	}

	// Token: 0x0600448F RID: 17551 RVA: 0x0016EAE6 File Offset: 0x0016CCE6
	private static void OnPlayerLeftRoom(NetPlayer obj)
	{
		CustomMapTelemetry.runningPlayerCount--;
		CustomMapTelemetry.minPlayersInMap = Math.Min(CustomMapTelemetry.runningPlayerCount, CustomMapTelemetry.minPlayersInMap);
	}

	// Token: 0x06004490 RID: 17552 RVA: 0x0016EB08 File Offset: 0x0016CD08
	public static void StartMapTracking()
	{
		if (CustomMapTelemetry.metricsCaptureStarted || CustomMapTelemetry.perfCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.mapEnterTime = Time.realtimeSinceStartup;
		float value = Random.value;
		if (value <= 0.01f)
		{
			CustomMapTelemetry.StartMetricsCapture();
		}
		else if (value >= 0.99f)
		{
			CustomMapTelemetry.StartPerfCapture();
		}
		if (CustomMapTelemetry.metricsCaptureStarted || CustomMapTelemetry.perfCaptureStarted)
		{
			if (CustomMapLoader.IsMapLoaded())
			{
				CustomMapTelemetry.mapModId = CustomMapLoader.LoadedMapModId._id;
				bool flag = CustomMapTelemetry.currentMapMod != null && CustomMapTelemetry.currentMapMod.Id._id == CustomMapTelemetry.mapModId && !string.IsNullOrEmpty(CustomMapTelemetry.currentMapMod.Name);
				CustomMapTelemetry.mapName = (flag ? CustomMapTelemetry.currentMapMod.Name : CustomMapTelemetry.mapModId.ToString());
				string text;
				if (!flag)
				{
					text = "NULL";
				}
				else
				{
					UserProfile creator = CustomMapTelemetry.currentMapMod.Creator;
					text = ((creator != null) ? creator.Username : null) ?? "NULL";
				}
				CustomMapTelemetry.mapCreatorUsername = text;
				return;
			}
			CustomMapTelemetry.mapName = "NULL";
			CustomMapTelemetry.mapCreatorUsername = "NULL";
			CustomMapTelemetry.mapModId = 0L;
		}
	}

	// Token: 0x06004491 RID: 17553 RVA: 0x0016EC13 File Offset: 0x0016CE13
	public static void EndMapTracking()
	{
		CustomMapTelemetry.EndMetricsCapture();
		CustomMapTelemetry.EndPerfCapture();
		CustomMapTelemetry.mapName = "NULL";
		CustomMapTelemetry.mapCreatorUsername = "NULL";
		CustomMapTelemetry.mapEnterTime = -1f;
		CustomMapTelemetry.mapModId = 0L;
	}

	// Token: 0x06004492 RID: 17554 RVA: 0x0016EC44 File Offset: 0x0016CE44
	private static void StartMetricsCapture()
	{
		if (CustomMapTelemetry.metricsCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.metricsCaptureStarted = true;
		NetworkSystem.Instance.OnPlayerJoined -= CustomMapTelemetry.OnPlayerJoinedRoom;
		NetworkSystem.Instance.OnPlayerJoined += CustomMapTelemetry.OnPlayerJoinedRoom;
		NetworkSystem.Instance.OnPlayerLeft -= CustomMapTelemetry.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnPlayerLeft += CustomMapTelemetry.OnPlayerLeftRoom;
		CustomMapTelemetry.runningPlayerCount = NetworkSystem.Instance.RoomPlayerCount;
		CustomMapTelemetry.minPlayersInMap = CustomMapTelemetry.runningPlayerCount;
		CustomMapTelemetry.maxPlayersInMap = CustomMapTelemetry.runningPlayerCount;
	}

	// Token: 0x06004493 RID: 17555 RVA: 0x0016ED08 File Offset: 0x0016CF08
	private static void EndMetricsCapture()
	{
		if (!CustomMapTelemetry.metricsCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.metricsCaptureStarted = false;
		NetworkSystem.Instance.OnPlayerJoined -= CustomMapTelemetry.OnPlayerJoinedRoom;
		NetworkSystem.Instance.OnPlayerLeft -= CustomMapTelemetry.OnPlayerLeftRoom;
		CustomMapTelemetry.inPrivateRoom = NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate;
		int num = Mathf.RoundToInt(Time.realtimeSinceStartup - CustomMapTelemetry.mapEnterTime);
		if (num < 30)
		{
			return;
		}
		if (CustomMapTelemetry.mapName.Equals("NULL") || CustomMapTelemetry.mapModId == 0L)
		{
			Debug.LogError("[CustomMapTelemetry::EndMetricsCapture] mapName or mapModID is invalid, throwing out this capture data...");
			return;
		}
		GorillaTelemetry.PostCustomMapTracking(CustomMapTelemetry.mapName, CustomMapTelemetry.mapModId, CustomMapTelemetry.mapCreatorUsername, CustomMapTelemetry.minPlayersInMap, CustomMapTelemetry.maxPlayersInMap, num, CustomMapTelemetry.inPrivateRoom);
	}

	// Token: 0x06004494 RID: 17556 RVA: 0x0016EDE4 File Offset: 0x0016CFE4
	private static void StartPerfCapture()
	{
		if (CustomMapTelemetry.perfCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.perfCaptureStarted = true;
		if (CustomMapTelemetry.instance.perfCaptureCoroutine != null)
		{
			CustomMapTelemetry.EndPerfCapture();
		}
		CustomMapTelemetry.drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 1, ProfilerRecorderOptions.Default);
		CustomMapTelemetry.LowestFPS = int.MaxValue;
		CustomMapTelemetry.HighestFPS = int.MinValue;
		CustomMapTelemetry.totalFPS = 0;
		CustomMapTelemetry.totalDrawCalls = 0;
		CustomMapTelemetry.totalPlayerCount = 0;
		CustomMapTelemetry.frameCounter = 0;
		CustomMapTelemetry.instance.perfCaptureCoroutine = CustomMapTelemetry.instance.StartCoroutine(CustomMapTelemetry.instance.CaptureMapPerformance());
	}

	// Token: 0x06004495 RID: 17557 RVA: 0x0016EE7C File Offset: 0x0016D07C
	private static void EndPerfCapture()
	{
		if (!CustomMapTelemetry.perfCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.perfCaptureStarted = false;
		if (CustomMapTelemetry.instance.perfCaptureCoroutine != null)
		{
			CustomMapTelemetry.instance.StopAllCoroutines();
			CustomMapTelemetry.instance.perfCaptureCoroutine = null;
		}
		CustomMapTelemetry.drawCallsRecorder.Dispose();
		if (CustomMapTelemetry.frameCounter == 0)
		{
			return;
		}
		int num = Mathf.RoundToInt(Time.realtimeSinceStartup - CustomMapTelemetry.mapEnterTime);
		CustomMapTelemetry.AverageFPS = CustomMapTelemetry.totalFPS / CustomMapTelemetry.frameCounter;
		CustomMapTelemetry.AverageDrawCalls = CustomMapTelemetry.totalDrawCalls / CustomMapTelemetry.frameCounter;
		CustomMapTelemetry.AveragePlayerCount = CustomMapTelemetry.totalPlayerCount / CustomMapTelemetry.frameCounter;
		if (num < 30)
		{
			return;
		}
		if (CustomMapTelemetry.mapName.Equals("NULL") || CustomMapTelemetry.mapModId == 0L)
		{
			Debug.LogError("[CustomMapTelemetry::EndPerfCapture] mapName or mapModID is invalid, throwing out this capture data...");
			return;
		}
		GorillaTelemetry.PostCustomMapPerformance(CustomMapTelemetry.mapName, CustomMapTelemetry.mapModId, CustomMapTelemetry.LowestFPS, CustomMapTelemetry.LowestFPSDrawCalls, CustomMapTelemetry.LowestFPSPlayerCount, CustomMapTelemetry.AverageFPS, CustomMapTelemetry.AverageDrawCalls, CustomMapTelemetry.AveragePlayerCount, CustomMapTelemetry.HighestFPS, CustomMapTelemetry.HighestFPSDrawCalls, CustomMapTelemetry.HighestFPSPlayerCount, num);
	}

	// Token: 0x06004496 RID: 17558 RVA: 0x0016EF77 File Offset: 0x0016D177
	private IEnumerator CaptureMapPerformance()
	{
		for (;;)
		{
			int num = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
			int num2 = Mathf.RoundToInt((float)CustomMapTelemetry.drawCallsRecorder.LastValue);
			int roomPlayerCount = NetworkSystem.Instance.RoomPlayerCount;
			CustomMapTelemetry.totalFPS += num;
			CustomMapTelemetry.totalDrawCalls += num2;
			CustomMapTelemetry.totalPlayerCount += roomPlayerCount;
			if (num > CustomMapTelemetry.HighestFPS)
			{
				CustomMapTelemetry.HighestFPS = num;
				CustomMapTelemetry.HighestFPSDrawCalls = num2;
				CustomMapTelemetry.HighestFPSPlayerCount = roomPlayerCount;
			}
			if (num < CustomMapTelemetry.LowestFPS)
			{
				CustomMapTelemetry.LowestFPS = num;
				CustomMapTelemetry.LowestFPSDrawCalls = num2;
				CustomMapTelemetry.LowestFPSPlayerCount = roomPlayerCount;
			}
			CustomMapTelemetry.frameCounter++;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06004497 RID: 17559 RVA: 0x0016EF7F File Offset: 0x0016D17F
	private void OnDestroy()
	{
		if (this.perfCaptureCoroutine != null)
		{
			CustomMapTelemetry.EndMapTracking();
		}
	}

	// Token: 0x0400568E RID: 22158
	[OnEnterPlay_SetNull]
	private static volatile CustomMapTelemetry instance;

	// Token: 0x0400568F RID: 22159
	private static string mapName = "NULL";

	// Token: 0x04005690 RID: 22160
	private static long mapModId;

	// Token: 0x04005691 RID: 22161
	private static string mapCreatorUsername = "NULL";

	// Token: 0x04005692 RID: 22162
	private static Mod currentMapMod;

	// Token: 0x04005693 RID: 22163
	private static GTMapLoadSource currentMapSource = GTMapLoadSource.none;

	// Token: 0x04005694 RID: 22164
	private static bool playerInMap;

	// Token: 0x04005695 RID: 22165
	private static long enteredMapId;

	// Token: 0x04005696 RID: 22166
	private static string enteredMapSource;

	// Token: 0x04005697 RID: 22167
	private static bool metricsCaptureStarted;

	// Token: 0x04005698 RID: 22168
	private static float mapEnterTime;

	// Token: 0x04005699 RID: 22169
	private static int runningPlayerCount;

	// Token: 0x0400569A RID: 22170
	private static int minPlayersInMap;

	// Token: 0x0400569B RID: 22171
	private static int maxPlayersInMap;

	// Token: 0x0400569C RID: 22172
	private static bool inPrivateRoom;

	// Token: 0x0400569D RID: 22173
	private const int minimumPlaytimeForTracking = 30;

	// Token: 0x0400569E RID: 22174
	private static int LowestFPS = int.MaxValue;

	// Token: 0x0400569F RID: 22175
	private static int LowestFPSDrawCalls;

	// Token: 0x040056A0 RID: 22176
	private static int LowestFPSPlayerCount;

	// Token: 0x040056A1 RID: 22177
	private static int AverageFPS;

	// Token: 0x040056A2 RID: 22178
	private static int AverageDrawCalls;

	// Token: 0x040056A3 RID: 22179
	private static int AveragePlayerCount;

	// Token: 0x040056A4 RID: 22180
	private static int HighestFPS = int.MinValue;

	// Token: 0x040056A5 RID: 22181
	private static int HighestFPSDrawCalls;

	// Token: 0x040056A6 RID: 22182
	private static int HighestFPSPlayerCount;

	// Token: 0x040056A7 RID: 22183
	private static int totalFPS;

	// Token: 0x040056A8 RID: 22184
	private static int totalDrawCalls;

	// Token: 0x040056A9 RID: 22185
	private static int totalPlayerCount;

	// Token: 0x040056AA RID: 22186
	private static int frameCounter;

	// Token: 0x040056AB RID: 22187
	private Coroutine perfCaptureCoroutine;

	// Token: 0x040056AC RID: 22188
	private static ProfilerRecorder drawCallsRecorder;

	// Token: 0x040056AD RID: 22189
	private static bool perfCaptureStarted;
}
