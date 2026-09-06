using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using JetBrains.Annotations;
using KID.Model;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x020008DA RID: 2266
public static class GorillaTelemetry
{
	// Token: 0x06003B5B RID: 15195 RVA: 0x00143AA0 File Offset: 0x00141CA0
	static GorillaTelemetry()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["User"] = null;
		dictionary["EventType"] = null;
		dictionary["ZoneId"] = null;
		dictionary["SubZoneId"] = null;
		GorillaTelemetry.gZoneEventArgs = dictionary;
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		dictionary2["User"] = null;
		dictionary2["EventType"] = null;
		dictionary2["ZoneId"] = null;
		dictionary2["SubZoneId"] = null;
		dictionary2["IsPrivateRoom"] = false;
		dictionary2["MapId"] = null;
		dictionary2["MapSource"] = null;
		GorillaTelemetry.gCustomMapZoneEventArgs = dictionary2;
		Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
		dictionary3["User"] = null;
		dictionary3["EventType"] = null;
		GorillaTelemetry.gNotifEventArgs = dictionary3;
		GorillaTelemetry.nextStayTimestamp = 0f;
		Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
		dictionary4["User"] = null;
		dictionary4["EventType"] = null;
		dictionary4["game_mode"] = null;
		GorillaTelemetry.gGameModeStartEventArgs = dictionary4;
		Dictionary<string, object> dictionary5 = new Dictionary<string, object>();
		dictionary5["User"] = null;
		dictionary5["EventType"] = null;
		dictionary5["Items"] = null;
		GorillaTelemetry.gShopEventArgs = dictionary5;
		GorillaTelemetry.gSingleItemParam = new CosmeticsController.CosmeticItem[1];
		GorillaTelemetry.gSingleItemBuilderParam = new BuilderSetManager.BuilderSetStoreItem[1];
		Dictionary<string, object> dictionary6 = new Dictionary<string, object>();
		dictionary6["User"] = null;
		dictionary6["EventType"] = null;
		dictionary6["AgeCategory"] = null;
		dictionary6["VoiceChatEnabled"] = null;
		dictionary6["CustomUsernameEnabled"] = null;
		dictionary6["JoinGroups"] = null;
		GorillaTelemetry.gKidEventArgs = dictionary6;
		Dictionary<string, object> dictionary7 = new Dictionary<string, object>();
		dictionary7["User"] = null;
		dictionary7["WamGameId"] = null;
		dictionary7["WamMachineId"] = null;
		GorillaTelemetry.gWamGameStartArgs = dictionary7;
		Dictionary<string, object> dictionary8 = new Dictionary<string, object>();
		dictionary8["User"] = null;
		dictionary8["WamGameId"] = null;
		dictionary8["WamMachineId"] = null;
		dictionary8["WamMLevelNumber"] = null;
		dictionary8["WamGoodMolesShown"] = null;
		dictionary8["WamHazardMolesShown"] = null;
		dictionary8["WamLevelMinScore"] = null;
		dictionary8["WamLevelScore"] = null;
		dictionary8["WamHazardMolesHit"] = null;
		dictionary8["WamGameState"] = null;
		GorillaTelemetry.gWamLevelEndArgs = dictionary8;
		Dictionary<string, object> dictionary9 = new Dictionary<string, object>();
		dictionary9["CustomMapName"] = null;
		dictionary9["CustomMapModId"] = null;
		dictionary9["LowestFPS"] = null;
		dictionary9["LowestFPSDrawCalls"] = null;
		dictionary9["LowestFPSPlayerCount"] = null;
		dictionary9["AverageFPS"] = null;
		dictionary9["AverageDrawCalls"] = null;
		dictionary9["AveragePlayerCount"] = null;
		dictionary9["HighestFPS"] = null;
		dictionary9["HighestFPSDrawCalls"] = null;
		dictionary9["HighestFPSPlayerCount"] = null;
		dictionary9["PlaytimeInSeconds"] = null;
		GorillaTelemetry.gCustomMapPerfArgs = dictionary9;
		Dictionary<string, object> dictionary10 = new Dictionary<string, object>();
		dictionary10["User"] = null;
		dictionary10["CustomMapName"] = null;
		dictionary10["CustomMapModId"] = null;
		dictionary10["CustomMapCreator"] = null;
		dictionary10["MinPlayerCount"] = null;
		dictionary10["MaxPlayerCount"] = null;
		dictionary10["PlaytimeOnMap"] = null;
		dictionary10["PrivateRoom"] = null;
		GorillaTelemetry.gCustomMapTrackingMetrics = dictionary10;
		Dictionary<string, object> dictionary11 = new Dictionary<string, object>();
		dictionary11["User"] = null;
		dictionary11["CustomMapName"] = null;
		dictionary11["CustomMapModId"] = null;
		dictionary11["CustomMapCreator"] = null;
		GorillaTelemetry.gCustomMapDownloadMetrics = dictionary11;
		Dictionary<string, object> dictionary12 = new Dictionary<string, object>();
		dictionary12["User"] = null;
		dictionary12["CustomMapModId"] = null;
		dictionary12["CustomMapName"] = null;
		dictionary12["CreatorId"] = null;
		dictionary12["CreatorUsername"] = null;
		dictionary12["MapDateLive"] = null;
		dictionary12["MapDateUpdated"] = null;
		dictionary12["MapTags"] = null;
		dictionary12["MapSupportVersion"] = null;
		dictionary12["MapMaxPlayers"] = null;
		dictionary12["MapHasCustomGameMode"] = null;
		dictionary12["MapGravityZoneCount"] = null;
		dictionary12["MapSizeChangerCount"] = null;
		dictionary12["MapHandHoldCount"] = null;
		dictionary12["MapMapperAssetCount"] = null;
		GorillaTelemetry.gCustomMapRegistryMetrics = dictionary12;
		GorillaTelemetry.gGhostReactorShiftStartArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_game_start",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "ghost_game_id", null },
				{ "event_timestamp", null },
				{ "initial_cores_balance", null },
				{ "number_of_players", null },
				{ "start_at_beginning", null },
				{ "seconds_into_shift_at_join", null },
				{ "floor_joined", null },
				{ "player_rank", null },
				{ "is_private_room", null }
			}
		};
		GorillaTelemetry.gGhostReactorShiftEndArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_game_end",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "ghost_game_id", null },
				{ "event_timestamp", null },
				{ "final_cores_balance", null },
				{ "total_cores_collected_by_player", null },
				{ "total_cores_collected_by_group", null },
				{ "total_cores_spent_by_player", null },
				{ "total_cores_spent_by_group", null },
				{ "gates_unlocked", null },
				{ "died", null },
				{ "items_purchased", null },
				{ "shift_cut_data", null },
				{ "play_duration", null },
				{ "started_late", null },
				{ "time_started", null },
				{ "reason", null },
				{ "max_number_in_game", null },
				{ "end_number_in_game", null },
				{ "items_picked_up", null },
				{ "revives", null },
				{ "num_shifts_played", null }
			}
		};
		GorillaTelemetry.gGhostReactorFloorStartArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_floor_start",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "ghost_game_id", null },
				{ "event_timestamp", null },
				{ "initial_cores_balance", null },
				{ "number_of_players", null },
				{ "start_at_beginning", null },
				{ "seconds_into_shift_at_join", null },
				{ "player_rank", null },
				{ "floor", null },
				{ "preset", null },
				{ "modifier", null },
				{ "is_private_room", null }
			}
		};
		GorillaTelemetry.gGhostReactorFloorEndArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_floor_end",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "ghost_game_id", null },
				{ "event_timestamp", null },
				{ "final_cores_balance", null },
				{ "total_cores_collected_by_player", null },
				{ "total_cores_collected_by_group", null },
				{ "total_cores_spent_by_player", null },
				{ "total_cores_spent_by_group", null },
				{ "gates_unlocked", null },
				{ "died", null },
				{ "items_purchased", null },
				{ "shift_cut_data", null },
				{ "play_duration", null },
				{ "started_late", null },
				{ "time_started", null },
				{ "reason", null },
				{ "max_number_in_game", null },
				{ "end_number_in_game", null },
				{ "items_picked_up", null },
				{ "revives", null },
				{ "floor", null },
				{ "preset", null },
				{ "modifier", null },
				{ "chaos_seeds_collected", null },
				{ "objectives_completed", null },
				{ "section", null },
				{ "xp_gained", null }
			}
		};
		GorillaTelemetry.gGhostReactorToolPurchasedArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_tool_purchased",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "ghost_game_id", null },
				{ "event_timestamp", null },
				{ "tool", null },
				{ "tool_level", null },
				{ "cores_spent", null },
				{ "shiny_rocks_spent", null },
				{ "floor", null },
				{ "preset", null }
			}
		};
		GorillaTelemetry.gGhostReactorRankUpArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_game_rank_up",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "ghost_game_id", null },
				{ "event_timestamp", null },
				{ "new_rank", null },
				{ "floor", null },
				{ "preset", null }
			}
		};
		GorillaTelemetry.gGhostReactorToolUnlockArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_game_tool_unlock",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "ghost_game_id", null },
				{ "event_timestamp", null },
				{ "tool", null }
			}
		};
		GorillaTelemetry.gGhostReactorPodUpgradePurchasedArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_pod_upgrade_purchased",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "ghost_game_id", null },
				{ "event_timestamp", null },
				{ "tool", null },
				{ "new_level", null },
				{ "shiny_rocks_spent", null },
				{ "juice_spent", null }
			}
		};
		GorillaTelemetry.gGhostReactorToolUpgradeArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_game_tool_upgrade",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "ghost_game_id", null },
				{ "event_timestamp", null },
				{ "type", null },
				{ "tool", null },
				{ "new_level", null },
				{ "juice_spent", null },
				{ "grift_spent", null },
				{ "cores_spent", null },
				{ "floor", null },
				{ "preset", null }
			}
		};
		GorillaTelemetry.gGhostReactorChaosSeedStartArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_chaos_seed_start",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "ghost_game_id", null },
				{ "event_timestamp", null },
				{ "unlock_time", null },
				{ "chaos_seeds_in_queue", null },
				{ "floor", null },
				{ "preset", null }
			}
		};
		GorillaTelemetry.gGhostReactorChaosJuiceCollectedArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_chaos_juice_collected",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "ghost_game_id", null },
				{ "event_timestamp", null },
				{ "juice_collected", null },
				{ "cores_processed_by_overdrive", null }
			}
		};
		GorillaTelemetry.gGhostReactorOverdrivePurchasedArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_overdrive_purchased",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "ghost_game_id", null },
				{ "event_timestamp", null },
				{ "shiny_rocks_used", null },
				{ "chaos_seeds_in_queue", null },
				{ "floor", null },
				{ "preset", null }
			}
		};
		GorillaTelemetry.gGhostReactorCreditsRefillPurchasedArgs = new GhostReactorTelemetryData
		{
			EventName = "ghost_credits_refill_purchased",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "ghost_game_id", null },
				{ "event_timestamp", null },
				{ "shiny_rocks_spent", null },
				{ "final_credits", null },
				{ "floor", null },
				{ "preset", null }
			}
		};
		GorillaTelemetry.gSuperInfectionArgs = new SuperInfectionTelemetryData
		{
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "event_timestamp", null },
				{ "total_play_time", null },
				{ "room_play_time", null },
				{ "session_play_time", null },
				{ "interval_play_time", null },
				{ "terminal_total_time", null },
				{ "terminal_interval_time", null },
				{ "time_holding_gadget_type_total", null },
				{ "time_holding_gadget_type_interval", null },
				{ "time_holding_own_gadgets_total", null },
				{ "time_holding_own_gadgets_interval", null },
				{ "time_holding_others_gadgets_total", null },
				{ "time_holding_others_gadgets_interval", null },
				{ "tags_holding_gadget_type_total", null },
				{ "tags_holding_gadget_type_interval", null },
				{ "tags_holding_own_gadgets_total", null },
				{ "tags_holding_own_gadgets_interval", null },
				{ "tags_holding_others_gadgets_total", null },
				{ "tags_holding_others_gadgets_interval", null },
				{ "resource_type_collected_total", null },
				{ "resource_type_collected_interval", null },
				{ "rounds_played_total", null },
				{ "rounds_played_interval", null },
				{ "unlocked_nodes", null },
				{ "player_count", null }
			}
		};
		GorillaTelemetry.gSuperInfectionPurchaseArgs = new SuperInfectionTelemetryData
		{
			EventName = "super_infection_purchase",
			CustomTags = new string[]
			{
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, object>
			{
				{ "event_timestamp", null },
				{ "total_play_time", null },
				{ "room_play_time", null },
				{ "session_play_time", null },
				{ "si_purchase_type", null },
				{ "si_shiny_rock_cost", null },
				{ "si_tech_points_purchased", null }
			}
		};
		GameObject gameObject = new GameObject("GorillaTelemetryBatcher");
		Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<GorillaTelemetry.BatchRunner>();
	}

	// Token: 0x06003B5C RID: 15196 RVA: 0x00144A1C File Offset: 0x00142C1C
	public static void EnqueueTelemetryEvent(string eventName, object content, [CanBeNull] string[] customTags = null)
	{
		if (content == null || string.IsNullOrWhiteSpace(eventName) || !GorillaServer.Instance.CheckIsMothershipTelemetryEnabled())
		{
			return;
		}
		if (GorillaTelemetry.telemetryEventsQueueMothership.Count > 100)
		{
			Debug.LogError("[Telemetry] Too many telemetry events!  Not enqueueing " + eventName + ": " + content.ToJson(true));
			return;
		}
		GorillaTelemetry.telemetryEventsQueueMothership.Enqueue(new MothershipAnalyticsEvent
		{
			event_name = eventName,
			event_timestamp = DateTime.UtcNow.ToString("O"),
			body = JsonConvert.SerializeObject(content),
			custom_tags = ((customTags != null && customTags.Length != 0) ? GorillaTelemetry.SerializeCustomTags(customTags) : string.Empty)
		});
	}

	// Token: 0x06003B5D RID: 15197 RVA: 0x00144AC4 File Offset: 0x00142CC4
	private static void FlushMothershipTelemetry()
	{
		int count = GorillaTelemetry.telemetryEventsQueueMothership.Count;
		if (count == 0)
		{
			return;
		}
		MothershipAnalyticsEvent[] array = ArrayPool<MothershipAnalyticsEvent>.Shared.Rent(count);
		try
		{
			int j;
			for (j = 0; j < count; j++)
			{
				MothershipAnalyticsEvent mothershipAnalyticsEvent;
				array[j] = (GorillaTelemetry.telemetryEventsQueueMothership.TryDequeue(out mothershipAnalyticsEvent) ? mothershipAnalyticsEvent : null);
			}
			if (j == 0)
			{
				ArrayPool<MothershipAnalyticsEvent>.Shared.Return(array, false);
			}
			else
			{
				MothershipWriteEventsRequest mothershipWriteEventsRequest = new MothershipWriteEventsRequest
				{
					title_id = MothershipClientApiUnity.TitleId,
					deployment_id = MothershipClientApiUnity.DeploymentId,
					env_id = MothershipClientApiUnity.EnvironmentId,
					events = new AnalyticsRequestVector(GorillaTelemetry.GetEventListForArrayMothership(array, j))
				};
				MothershipClientApiUnity.WriteEvents(MothershipClientContext.MothershipId, mothershipWriteEventsRequest, delegate(MothershipWriteEventsResponse resp)
				{
				}, delegate(MothershipError err, int i)
				{
				});
			}
		}
		finally
		{
			ArrayPool<MothershipAnalyticsEvent>.Shared.Return(array, false);
		}
	}

	// Token: 0x06003B5E RID: 15198 RVA: 0x00144BC4 File Offset: 0x00142DC4
	private static List<MothershipAnalyticsEvent> GetEventListForArrayMothership(MothershipAnalyticsEvent[] array, int count)
	{
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			if (array[i] != null)
			{
				num++;
			}
		}
		List<MothershipAnalyticsEvent> list;
		if (!GorillaTelemetry.gListPoolMothership.TryGetValue(num, out list))
		{
			list = new List<MothershipAnalyticsEvent>(num);
			GorillaTelemetry.gListPoolMothership.TryAdd(num, list);
		}
		else
		{
			list.Clear();
		}
		string code = LocalisationManager.CurrentLanguage.Identifier.Code;
		for (int j = 0; j < count; j++)
		{
			if (array[j] != null)
			{
				list.Add(array[j]);
			}
		}
		return list;
	}

	// Token: 0x06003B5F RID: 15199 RVA: 0x00144C47 File Offset: 0x00142E47
	private static bool IsConnected()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return false;
		}
		if (GorillaTelemetry.gPlayFabAuth == null)
		{
			GorillaTelemetry.gPlayFabAuth = PlayFabAuthenticator.instance;
		}
		return !(GorillaTelemetry.gPlayFabAuth == null);
	}

	// Token: 0x06003B60 RID: 15200 RVA: 0x00144C7A File Offset: 0x00142E7A
	private static bool IsConnectedToPlayfab()
	{
		if (GorillaTelemetry.gPlayFabAuth == null)
		{
			GorillaTelemetry.gPlayFabAuth = PlayFabAuthenticator.instance;
		}
		return !(GorillaTelemetry.gPlayFabAuth == null);
	}

	// Token: 0x06003B61 RID: 15201 RVA: 0x00144C7A File Offset: 0x00142E7A
	private static bool IsConnectedIgnoreRoom()
	{
		if (GorillaTelemetry.gPlayFabAuth == null)
		{
			GorillaTelemetry.gPlayFabAuth = PlayFabAuthenticator.instance;
		}
		return !(GorillaTelemetry.gPlayFabAuth == null);
	}

	// Token: 0x06003B62 RID: 15202 RVA: 0x00144C9F File Offset: 0x00142E9F
	private static string PlayFabUserId()
	{
		return GorillaTelemetry.gPlayFabAuth.GetPlayFabPlayerId();
	}

	// Token: 0x06003B63 RID: 15203 RVA: 0x00144CAC File Offset: 0x00142EAC
	private static string SerializeCustomTags(string[] customTags)
	{
		string text = string.Empty;
		if (customTags != null && customTags.Length != 0)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int i = 0; i < customTags.Length; i++)
			{
				dictionary.Add(string.Format("tag{0}", i + 1), customTags[i]);
			}
			text = JsonConvert.SerializeObject(dictionary);
		}
		return text;
	}

	// Token: 0x06003B64 RID: 15204 RVA: 0x00144D00 File Offset: 0x00142F00
	public static void EnqueueZoneEvent(ZoneDef zone, GTZoneEventType zoneEventType)
	{
		if (zoneEventType == GTZoneEventType.zone_stay && Time.realtimeSinceStartup < GorillaTelemetry.nextStayTimestamp)
		{
			return;
		}
		GorillaTelemetry.nextStayTimestamp = Time.realtimeSinceStartup + (float)zone.trackStayIntervalSec;
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		if (!GorillaServer.Instance.CheckIsTZE_Enabled())
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = zoneEventType.GetName<GTZoneEventType>();
		string name2 = zone.zoneId.GetName<GTZone>();
		string name3 = zone.subZoneId.GetName<GTSubZone>();
		bool sessionIsPrivate = NetworkSystem.Instance.SessionIsPrivate;
		bool flag = zone.zoneId == GTZone.customMaps;
		Dictionary<string, object> dictionary = (flag ? GorillaTelemetry.gCustomMapZoneEventArgs : GorillaTelemetry.gZoneEventArgs);
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["ZoneId"] = name2;
		dictionary["SubZoneId"] = name3;
		dictionary["IsPrivateRoom"] = sessionIsPrivate;
		if (flag)
		{
			dictionary["MapId"] = CustomMapTelemetry.CurrentMapIdString;
			dictionary["MapSource"] = CustomMapTelemetry.CurrentMapSourceString;
		}
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_zone_event", dictionary, null);
	}

	// Token: 0x06003B65 RID: 15205 RVA: 0x00144E0C File Offset: 0x0014300C
	public static bool PostCustomMapZoneEvent(GTZoneEventType zoneEventType, long mapId, string mapSource)
	{
		if (!GorillaTelemetry.IsConnectedToPlayfab())
		{
			return false;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		if (!GorillaServer.Instance.CheckIsTZE_Enabled())
		{
			return false;
		}
		bool sessionIsPrivate = NetworkSystem.Instance.SessionIsPrivate;
		Dictionary<string, object> dictionary = GorillaTelemetry.gCustomMapZoneEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = zoneEventType.GetName<GTZoneEventType>();
		dictionary["ZoneId"] = GTZone.customMaps.GetName<GTZone>();
		dictionary["SubZoneId"] = GTSubZone.none.GetName<GTSubZone>();
		dictionary["IsPrivateRoom"] = sessionIsPrivate;
		dictionary["MapId"] = mapId.ToString();
		dictionary["MapSource"] = mapSource;
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_zone_event", dictionary, null);
		return true;
	}

	// Token: 0x06003B66 RID: 15206 RVA: 0x00144ED0 File Offset: 0x001430D0
	public static void PostGameModeEvent(GTGameModeEventType gameModeEvent, GameModeType gameMode)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = gameModeEvent.GetName<GTGameModeEventType>();
		string name2 = gameMode.GetName<GameModeType>();
		Dictionary<string, object> dictionary = GorillaTelemetry.gGameModeStartEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["game_mode"] = name2;
		GorillaTelemetry.EnqueueTelemetryEvent("game_mode_played_event", dictionary, null);
	}

	// Token: 0x06003B67 RID: 15207 RVA: 0x00144F2F File Offset: 0x0014312F
	public static void PostShopEvent(VRRig playerRig, GTShopEventType shopEvent, CosmeticsController.CosmeticItem item)
	{
		GorillaTelemetry.gSingleItemParam[0] = item;
		GorillaTelemetry.PostShopEvent(playerRig, shopEvent, GorillaTelemetry.gSingleItemParam);
		GorillaTelemetry.gSingleItemParam[0] = default(CosmeticsController.CosmeticItem);
	}

	// Token: 0x06003B68 RID: 15208 RVA: 0x00144F5C File Offset: 0x0014315C
	private static string[] FetchItemArgs(IList<CosmeticsController.CosmeticItem> items)
	{
		int count = items.Count;
		if (count == 0)
		{
			return Array.Empty<string>();
		}
		HashSet<string> hashSet = new HashSet<string>(count);
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			CosmeticsController.CosmeticItem cosmeticItem = items[i];
			if (!cosmeticItem.isNullItem)
			{
				string itemName = cosmeticItem.itemName;
				if (!string.IsNullOrWhiteSpace(itemName) && !itemName.Contains("NOTHING", StringComparison.InvariantCultureIgnoreCase) && hashSet.Add(itemName))
				{
					num++;
				}
			}
		}
		string[] array = new string[num];
		hashSet.CopyTo(array);
		return array;
	}

	// Token: 0x06003B69 RID: 15209 RVA: 0x00144FE8 File Offset: 0x001431E8
	public static void PostShopEvent(VRRig playerRig, GTShopEventType shopEvent, IList<CosmeticsController.CosmeticItem> items)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		if (!playerRig.isLocal)
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = shopEvent.GetName<GTShopEventType>();
		string[] array = GorillaTelemetry.FetchItemArgs(items);
		Dictionary<string, object> dictionary = GorillaTelemetry.gShopEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["Items"] = array;
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_shop_event", dictionary, null);
	}

	// Token: 0x06003B6A RID: 15210 RVA: 0x00145050 File Offset: 0x00143250
	public static void PostBuilderKioskEvent(VRRig playerRig, GTShopEventType shopEvent, BuilderSetManager.BuilderSetStoreItem item)
	{
		GorillaTelemetry.gSingleItemBuilderParam[0] = item;
		GorillaTelemetry.PostBuilderKioskEvent(playerRig, shopEvent, GorillaTelemetry.gSingleItemBuilderParam);
		GorillaTelemetry.gSingleItemBuilderParam[0] = default(BuilderSetManager.BuilderSetStoreItem);
	}

	// Token: 0x06003B6B RID: 15211 RVA: 0x0014507C File Offset: 0x0014327C
	private static string[] BuilderItemsToStrings(IList<BuilderSetManager.BuilderSetStoreItem> items)
	{
		int count = items.Count;
		if (count == 0)
		{
			return Array.Empty<string>();
		}
		HashSet<string> hashSet = new HashSet<string>(count);
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem = items[i];
			if (!builderSetStoreItem.isNullItem)
			{
				string playfabID = builderSetStoreItem.playfabID;
				if (!string.IsNullOrWhiteSpace(playfabID) && !playfabID.Contains("NOTHING", StringComparison.InvariantCultureIgnoreCase) && hashSet.Add(playfabID))
				{
					num++;
				}
			}
		}
		string[] array = new string[num];
		hashSet.CopyTo(array);
		return array;
	}

	// Token: 0x06003B6C RID: 15212 RVA: 0x00145108 File Offset: 0x00143308
	public static void PostBuilderKioskEvent(VRRig playerRig, GTShopEventType shopEvent, IList<BuilderSetManager.BuilderSetStoreItem> items)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		if (!playerRig.isLocal)
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = shopEvent.GetName<GTShopEventType>();
		string[] array = GorillaTelemetry.BuilderItemsToStrings(items);
		Dictionary<string, object> dictionary = GorillaTelemetry.gShopEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["Items"] = array;
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_shop_event", dictionary, null);
	}

	// Token: 0x06003B6D RID: 15213 RVA: 0x00145170 File Offset: 0x00143370
	public static void PostKidEvent(bool joinGroupsEnabled, bool voiceChatEnabled, bool customUsernamesEnabled, AgeStatusType ageCategory, GTKidEventType kidEvent)
	{
		if ((double)Random.value < 0.1)
		{
			return;
		}
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = kidEvent.GetName<GTKidEventType>();
		string text2 = ((ageCategory == AgeStatusType.LEGALADULT) ? "Not_Managed_Account" : "Managed_Account");
		string text3 = joinGroupsEnabled.ToString().ToUpper();
		string text4 = voiceChatEnabled.ToString().ToUpper();
		string text5 = customUsernamesEnabled.ToString().ToUpper();
		Dictionary<string, object> dictionary = GorillaTelemetry.gKidEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["AgeCategory"] = text2;
		dictionary["VoiceChatEnabled"] = text4;
		dictionary["CustomUsernameEnabled"] = text5;
		dictionary["JoinGroups"] = text3;
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_kid_event", dictionary, null);
	}

	// Token: 0x06003B6E RID: 15214 RVA: 0x00145244 File Offset: 0x00143444
	public static void WamGameStart(string playerId, string gameId, string machineId)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gWamGameStartArgs["User"] = playerId;
		GorillaTelemetry.gWamGameStartArgs["WamGameId"] = gameId;
		GorillaTelemetry.gWamGameStartArgs["WamMachineId"] = machineId;
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_wam_gameStartEvent", GorillaTelemetry.gWamGameStartArgs, null);
	}

	// Token: 0x06003B6F RID: 15215 RVA: 0x0014529C File Offset: 0x0014349C
	public static void WamLevelEnd(string playerId, int gameId, string machineId, int currentLevelNumber, int levelGoodMolesShown, int levelHazardMolesShown, int levelMinScore, int currentScore, int levelHazardMolesHit, string currentGameResult)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gWamLevelEndArgs["User"] = playerId;
		GorillaTelemetry.gWamLevelEndArgs["WamGameId"] = gameId.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamMachineId"] = machineId;
		GorillaTelemetry.gWamLevelEndArgs["WamMLevelNumber"] = currentLevelNumber.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamGoodMolesShown"] = levelGoodMolesShown.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamHazardMolesShown"] = levelHazardMolesShown.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamLevelMinScore"] = levelMinScore.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamLevelScore"] = currentScore.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamHazardMolesHit"] = levelHazardMolesHit.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamGameState"] = currentGameResult;
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_wam_levelEndEvent", GorillaTelemetry.gWamLevelEndArgs, null);
	}

	// Token: 0x06003B70 RID: 15216 RVA: 0x0014538C File Offset: 0x0014358C
	public static void PostCustomMapPerformance(string mapName, long mapModId, int lowestFPS, int lowestDC, int lowestPC, int avgFPS, int avgDC, int avgPC, int highestFPS, int highestDC, int highestPC, int playtime)
	{
		if (!GorillaTelemetry.IsConnectedToPlayfab())
		{
			return;
		}
		Dictionary<string, object> dictionary = GorillaTelemetry.gCustomMapPerfArgs;
		dictionary["CustomMapName"] = mapName;
		dictionary["CustomMapModId"] = mapModId.ToString();
		dictionary["LowestFPS"] = lowestFPS.ToString();
		dictionary["LowestFPSDrawCalls"] = lowestDC.ToString();
		dictionary["LowestFPSPlayerCount"] = lowestPC.ToString();
		dictionary["AverageFPS"] = avgFPS.ToString();
		dictionary["AverageDrawCalls"] = avgDC.ToString();
		dictionary["AveragePlayerCount"] = avgPC.ToString();
		dictionary["HighestFPS"] = highestFPS.ToString();
		dictionary["HighestFPSDrawCalls"] = highestDC.ToString();
		dictionary["HighestFPSPlayerCount"] = highestPC.ToString();
		dictionary["PlaytimeInSeconds"] = playtime.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent("CustomMapPerformance", dictionary, null);
	}

	// Token: 0x06003B71 RID: 15217 RVA: 0x00145488 File Offset: 0x00143688
	public static void PostCustomMapTracking(string mapName, long mapModId, string mapCreatorUsername, int minPlayers, int maxPlayers, int playtime, bool privateRoom)
	{
		if (!GorillaTelemetry.IsConnectedToPlayfab())
		{
			return;
		}
		int num = playtime % 60;
		int num2 = (playtime - num) / 60;
		int num3 = num2 % 60;
		int num4 = (num2 - num3) / 60;
		string text = string.Format("{0}.{1}.{2}", num4, num3, num);
		Dictionary<string, object> dictionary = GorillaTelemetry.gCustomMapTrackingMetrics;
		dictionary["User"] = GorillaTelemetry.PlayFabUserId();
		dictionary["CustomMapName"] = mapName;
		dictionary["CustomMapModId"] = mapModId.ToString();
		dictionary["CustomMapCreator"] = mapCreatorUsername;
		dictionary["MinPlayerCount"] = minPlayers.ToString();
		dictionary["MaxPlayerCount"] = maxPlayers.ToString();
		dictionary["PlaytimeInSeconds"] = playtime.ToString();
		dictionary["PrivateRoom"] = privateRoom.ToString();
		dictionary["PlaytimeOnMap"] = text;
		GorillaTelemetry.EnqueueTelemetryEvent("CustomMapTracking", dictionary, null);
	}

	// Token: 0x06003B72 RID: 15218 RVA: 0x00002C2D File Offset: 0x00000E2D
	public static void PostCustomMapDownloadEvent(string mapName, long mapModId, string mapCreatorUsername)
	{
	}

	// Token: 0x06003B73 RID: 15219 RVA: 0x00145580 File Offset: 0x00143780
	public static void PostCustomMapRegistryEvent(long mapModId, string mapName, long creatorId, string creatorUsername, DateTime dateLive, DateTime dateUpdated, string[] tags, int mapSupportVersion, int maxPlayers, bool hasCustomGameMode, int gravityZoneCount, int sizeChangerCount, int handHoldCount, int mapperAssetCount)
	{
		if (!GorillaTelemetry.IsConnectedToPlayfab())
		{
			return;
		}
		Dictionary<string, object> dictionary = GorillaTelemetry.gCustomMapRegistryMetrics;
		dictionary["User"] = GorillaTelemetry.PlayFabUserId();
		dictionary["CustomMapModId"] = mapModId.ToString();
		dictionary["CustomMapName"] = mapName;
		dictionary["CreatorId"] = creatorId.ToString();
		dictionary["CreatorUsername"] = creatorUsername;
		dictionary["MapDateLive"] = dateLive.ToString("O");
		dictionary["MapDateUpdated"] = dateUpdated.ToString("O");
		dictionary["MapTags"] = tags ?? Array.Empty<string>();
		dictionary["MapSupportVersion"] = mapSupportVersion.ToString();
		dictionary["MapMaxPlayers"] = maxPlayers.ToString();
		dictionary["MapHasCustomGameMode"] = hasCustomGameMode.ToString();
		dictionary["MapGravityZoneCount"] = gravityZoneCount.ToString();
		dictionary["MapSizeChangerCount"] = sizeChangerCount.ToString();
		dictionary["MapHandHoldCount"] = handHoldCount.ToString();
		dictionary["MapMapperAssetCount"] = mapperAssetCount.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent("CustomMapRegistry", dictionary, null);
	}

	// Token: 0x06003B74 RID: 15220 RVA: 0x001456B8 File Offset: 0x001438B8
	public static void GhostReactorShiftStart(string gameId, int initialCores, float timeIntoShift, bool wasPlayerInAtStart, int numPlayers, int floorJoined, string playerRank)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorShiftStartArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["initial_cores_balance"] = initialCores.ToString();
		ghostReactorTelemetryData.BodyData["number_of_players"] = numPlayers.ToString();
		ghostReactorTelemetryData.BodyData["start_at_beginning"] = wasPlayerInAtStart.ToString();
		ghostReactorTelemetryData.BodyData["seconds_into_shift_at_join"] = timeIntoShift.ToString();
		ghostReactorTelemetryData.BodyData["floor_joined"] = floorJoined.ToString();
		ghostReactorTelemetryData.BodyData["player_rank"] = playerRank;
		ghostReactorTelemetryData.BodyData["is_private_room"] = NetworkSystem.Instance.SessionIsPrivate.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003B75 RID: 15221 RVA: 0x001457C0 File Offset: 0x001439C0
	public static void GhostReactorGameEnd(string gameId, int finalCores, int totalCoresCollectedByPlayer, int totalCoresCollectedByGroup, int totalCoresSpentByPlayer, int totalCoresSpentByGroup, int gatesUnlocked, int deaths, List<string> itemsPurchased, int shiftCut, bool isShiftActuallyEnding, float timeIntoShiftAtJoin, float playDuration, bool wasPlayerInAtStart, ZoneClearReason zoneClearReason, int maxNumberOfPlayersInShift, int endNumberOfPlayers, Dictionary<string, int> itemTypesHeldThisShift, int revives, int numShiftsPlayed)
	{
		if (!GorillaTelemetry.IsConnectedToPlayfab())
		{
			return;
		}
		string text = "shift_ended";
		if (!isShiftActuallyEnding)
		{
			if (zoneClearReason == ZoneClearReason.LeaveZone)
			{
				text = "left_zone";
			}
			else
			{
				text = "disconnect";
			}
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorShiftEndArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["final_cores_balance"] = finalCores.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_collected_by_player"] = totalCoresCollectedByPlayer.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_collected_by_group"] = totalCoresCollectedByGroup.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_spent_by_player"] = totalCoresSpentByPlayer.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_spent_by_group"] = totalCoresSpentByGroup.ToString();
		ghostReactorTelemetryData.BodyData["gates_unlocked"] = gatesUnlocked.ToString();
		ghostReactorTelemetryData.BodyData["died"] = deaths.ToString();
		ghostReactorTelemetryData.BodyData["items_purchased"] = itemsPurchased.ToJson(true);
		ghostReactorTelemetryData.BodyData["shift_cut_data"] = shiftCut.ToJson(true);
		ghostReactorTelemetryData.BodyData["play_duration"] = playDuration.ToString();
		ghostReactorTelemetryData.BodyData["started_late"] = (!wasPlayerInAtStart).ToString();
		ghostReactorTelemetryData.BodyData["time_started"] = timeIntoShiftAtJoin.ToString();
		ghostReactorTelemetryData.BodyData["reason"] = text;
		ghostReactorTelemetryData.BodyData["max_number_in_game"] = maxNumberOfPlayersInShift.ToString();
		ghostReactorTelemetryData.BodyData["end_number_in_game"] = endNumberOfPlayers.ToString();
		ghostReactorTelemetryData.BodyData["items_picked_up"] = itemTypesHeldThisShift.ToJson(true);
		ghostReactorTelemetryData.BodyData["revives"] = revives.ToString();
		ghostReactorTelemetryData.BodyData["num_shifts_played"] = numShiftsPlayed.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003B76 RID: 15222 RVA: 0x001459E0 File Offset: 0x00143BE0
	public static void GhostReactorFloorStart(string gameId, int initialCores, float timeIntoShift, bool wasPlayerInAtStart, int numPlayers, string playerRank, int floor, string preset, string modifier)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorFloorStartArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["initial_cores_balance"] = initialCores.ToString();
		ghostReactorTelemetryData.BodyData["number_of_players"] = numPlayers.ToString();
		ghostReactorTelemetryData.BodyData["start_at_beginning"] = wasPlayerInAtStart.ToString();
		ghostReactorTelemetryData.BodyData["seconds_into_shift_at_join"] = timeIntoShift.ToString();
		ghostReactorTelemetryData.BodyData["player_rank"] = playerRank;
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		ghostReactorTelemetryData.BodyData["modifier"] = modifier;
		ghostReactorTelemetryData.BodyData["is_private_room"] = NetworkSystem.Instance.SessionIsPrivate.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003B77 RID: 15223 RVA: 0x00145B0C File Offset: 0x00143D0C
	public static void GhostReactorFloorComplete(string gameId, int finalCores, int totalCoresCollectedByPlayer, int totalCoresCollectedByGroup, int totalCoresSpentByPlayer, int totalCoresSpentByGroup, int gatesUnlocked, int deaths, List<string> itemsPurchased, int shiftCut, bool isShiftActuallyEnding, float timeIntoShiftAtJoin, float playDuration, bool wasPlayerInAtStart, ZoneClearReason zoneClearReason, int maxNumberOfPlayersInShift, int endNumberOfPlayers, Dictionary<string, int> itemTypesHeldThisShift, int revives, int floor, string preset, string modifier, int chaosSeedsCollected, bool objectivesCompleted, string section, int xpGained)
	{
		if (!GorillaTelemetry.IsConnectedToPlayfab())
		{
			return;
		}
		string text = "shift_ended";
		if (!isShiftActuallyEnding)
		{
			if (zoneClearReason == ZoneClearReason.LeaveZone)
			{
				text = "left_zone";
			}
			else
			{
				text = "disconnect";
			}
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorFloorEndArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["final_cores_balance"] = finalCores.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_collected_by_player"] = totalCoresCollectedByPlayer.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_collected_by_group"] = totalCoresCollectedByGroup.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_spent_by_player"] = totalCoresSpentByPlayer.ToString();
		ghostReactorTelemetryData.BodyData["total_cores_spent_by_group"] = totalCoresSpentByGroup.ToString();
		ghostReactorTelemetryData.BodyData["gates_unlocked"] = gatesUnlocked.ToString();
		ghostReactorTelemetryData.BodyData["died"] = deaths.ToString();
		ghostReactorTelemetryData.BodyData["items_purchased"] = itemsPurchased.ToJson(true);
		ghostReactorTelemetryData.BodyData["shift_cut_data"] = shiftCut.ToJson(true);
		ghostReactorTelemetryData.BodyData["play_duration"] = playDuration.ToString();
		ghostReactorTelemetryData.BodyData["started_late"] = (!wasPlayerInAtStart).ToString();
		ghostReactorTelemetryData.BodyData["time_started"] = timeIntoShiftAtJoin.ToString();
		ghostReactorTelemetryData.BodyData["reason"] = text;
		ghostReactorTelemetryData.BodyData["max_number_in_game"] = maxNumberOfPlayersInShift.ToString();
		ghostReactorTelemetryData.BodyData["end_number_in_game"] = endNumberOfPlayers.ToString();
		ghostReactorTelemetryData.BodyData["items_picked_up"] = itemTypesHeldThisShift.ToJson(true);
		ghostReactorTelemetryData.BodyData["revives"] = revives.ToString();
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		ghostReactorTelemetryData.BodyData["modifier"] = modifier;
		ghostReactorTelemetryData.BodyData["chaos_seeds_collected"] = chaosSeedsCollected.ToString();
		ghostReactorTelemetryData.BodyData["objectives_completed"] = objectivesCompleted.ToString();
		ghostReactorTelemetryData.BodyData["section"] = section;
		ghostReactorTelemetryData.BodyData["xp_gained"] = xpGained.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003B78 RID: 15224 RVA: 0x00145DA8 File Offset: 0x00143FA8
	public static void GhostReactorToolPurchased(string gameId, string toolName, int toolLevel, int coresSpent, int shinyRocksSpent, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorToolPurchasedArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["tool"] = toolName;
		ghostReactorTelemetryData.BodyData["tool_level"] = toolLevel.ToString();
		ghostReactorTelemetryData.BodyData["cores_spent"] = coresSpent.ToString();
		ghostReactorTelemetryData.BodyData["shiny_rocks_spent"] = shinyRocksSpent.ToString();
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003B79 RID: 15225 RVA: 0x00145E88 File Offset: 0x00144088
	public static void GhostReactorRankUp(string gameId, string newRank, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorRankUpArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["new_rank"] = newRank;
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003B7A RID: 15226 RVA: 0x00145F24 File Offset: 0x00144124
	public static void GhostReactorToolUnlock(string gameId, string toolName)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorToolUnlockArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["tool"] = toolName;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003B7B RID: 15227 RVA: 0x00145F98 File Offset: 0x00144198
	public static void GhostReactorPodUpgradePurchased(string gameId, string toolName, int level, int shinyRocksSpent, int juiceSpent)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorPodUpgradePurchasedArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["tool"] = toolName;
		ghostReactorTelemetryData.BodyData["new_level"] = level.ToString();
		ghostReactorTelemetryData.BodyData["shiny_rocks_spent"] = shinyRocksSpent.ToString();
		ghostReactorTelemetryData.BodyData["juice_spent"] = juiceSpent.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003B7C RID: 15228 RVA: 0x00146050 File Offset: 0x00144250
	public static void GhostReactorToolUpgrade(string gameId, string upgradeType, string toolName, int newLevel, int juiceSpent, int griftSpent, int coresSpent, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorToolUpgradeArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["type"] = upgradeType;
		ghostReactorTelemetryData.BodyData["tool"] = toolName;
		ghostReactorTelemetryData.BodyData["new_level"] = newLevel.ToString();
		ghostReactorTelemetryData.BodyData["juice_spent"] = juiceSpent.ToString();
		ghostReactorTelemetryData.BodyData["grift_spent"] = griftSpent.ToString();
		ghostReactorTelemetryData.BodyData["cores_spent"] = coresSpent.ToString();
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003B7D RID: 15229 RVA: 0x00146158 File Offset: 0x00144358
	public static void GhostReactorChaosSeedStart(string gameId, string unlockTime, int chaosSeedsInQueue, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorChaosSeedStartArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["unlock_time"] = unlockTime;
		ghostReactorTelemetryData.BodyData["chaos_seeds_in_queue"] = chaosSeedsInQueue.ToString();
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003B7E RID: 15230 RVA: 0x0014620C File Offset: 0x0014440C
	public static void GhostReactorChaosJuiceCollected(string gameId, int juiceCollected, int coresProcessedByOverdrive)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorChaosJuiceCollectedArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["juice_collected"] = juiceCollected.ToString();
		ghostReactorTelemetryData.BodyData["cores_processed_by_overdrive"] = coresProcessedByOverdrive.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003B7F RID: 15231 RVA: 0x0014629C File Offset: 0x0014449C
	public static void GhostReactorOverdrivePurchased(string gameId, int shinyRocksUsed, int chaosSeedsInQueue, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorOverdrivePurchasedArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["shiny_rocks_used"] = shinyRocksUsed.ToString();
		ghostReactorTelemetryData.BodyData["chaos_seeds_in_queue"] = chaosSeedsInQueue.ToString();
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003B80 RID: 15232 RVA: 0x00146354 File Offset: 0x00144554
	public static void GhostReactorCreditsRefillPurchased(string gameId, int shinyRocksSpent, int finalCredits, int floor, string preset)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GhostReactorTelemetryData ghostReactorTelemetryData = GorillaTelemetry.gGhostReactorCreditsRefillPurchasedArgs;
		ghostReactorTelemetryData.BodyData["ghost_game_id"] = gameId;
		ghostReactorTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		ghostReactorTelemetryData.BodyData["shiny_rocks_spent"] = shinyRocksSpent.ToString();
		ghostReactorTelemetryData.BodyData["final_credits"] = finalCredits.ToString();
		ghostReactorTelemetryData.BodyData["floor"] = floor.ToString();
		ghostReactorTelemetryData.BodyData["preset"] = preset;
		GorillaTelemetry.EnqueueTelemetryEvent(ghostReactorTelemetryData.EventName, ghostReactorTelemetryData.BodyData, ghostReactorTelemetryData.CustomTags);
	}

	// Token: 0x06003B81 RID: 15233 RVA: 0x0014640C File Offset: 0x0014460C
	public unsafe static void SuperInfectionEvent(bool roomDisconnect, float totalPlayTime, float roomPlayTime, float sessionPlayTime, float intervalPlayTime, float terminalTotalTime, float terminalIntervalTime, Dictionary<SITechTreePageId, float> timeUsingGadgetsTotal, Dictionary<SITechTreePageId, float> timeUsingGadgetsInterval, float timeUsingOwnGadgetsTotal, float timeUsingOwnGadgetsInterval, float timeUsingOthersGadgetsTotal, float timeUsingOthersGadgetsInterval, Dictionary<SITechTreePageId, int> tagsUsingGadgetsTotal, Dictionary<SITechTreePageId, int> tagsUsingGadgetsInterval, int tagsHoldingOwnGadgetsTotal, int tagsHoldingOwnGadgetsInterval, int tagsHoldingOthersGadgetsTotal, int tagsHoldingOthersGadgetsInterval, Dictionary<SIResource.ResourceType, int> resourcesGatheredTotal, Dictionary<SIResource.ResourceType, int> resourcesGatheredInterval, int roundsPlayedTotal, int roundsPlayedInterval, bool[][] unlockedNodes, int numberOfPlayers)
	{
		if (!GorillaTelemetry.IsConnectedIgnoreRoom())
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < unlockedNodes.Length; i++)
		{
			num += unlockedNodes[i].Length;
		}
		int num2 = num;
		Span<char> span;
		checked
		{
			span = new Span<char>(stackalloc byte[unchecked((UIntPtr)num2) * 2], num2);
			num = 0;
		}
		for (int j = 0; j < unlockedNodes.Length; j++)
		{
			for (int k = 0; k < unlockedNodes[j].Length; k++)
			{
				*span[num] = (unlockedNodes[j][k] ? '1' : '0');
				num++;
			}
		}
		SuperInfectionTelemetryData superInfectionTelemetryData = GorillaTelemetry.gSuperInfectionArgs;
		superInfectionTelemetryData.EventName = (roomDisconnect ? "super_infection_room_left" : "super_infection_interval");
		Dictionary<string, object> dictionary = superInfectionTelemetryData.BodyData;
		if (dictionary["tags_holding_gadget_type_total"] == null)
		{
			dictionary["tags_holding_gadget_type_total"] = new Dictionary<string, object>();
		}
		dictionary = superInfectionTelemetryData.BodyData;
		if (dictionary["tags_holding_gadget_type_interval"] == null)
		{
			dictionary["tags_holding_gadget_type_interval"] = new Dictionary<string, object>();
		}
		Dictionary<string, object> dictionary2 = (Dictionary<string, object>)superInfectionTelemetryData.BodyData["tags_holding_gadget_type_total"];
		Dictionary<string, object> dictionary3 = (Dictionary<string, object>)superInfectionTelemetryData.BodyData["tags_holding_gadget_type_interval"];
		for (int l = 0; l < 11; l++)
		{
			SITechTreePageId sitechTreePageId = (SITechTreePageId)l;
			int num3;
			tagsUsingGadgetsTotal.TryGetValue(sitechTreePageId, out num3);
			int num4;
			tagsUsingGadgetsInterval.TryGetValue(sitechTreePageId, out num4);
			string text = sitechTreePageId.ToString();
			dictionary2[text] = num3.ToString();
			dictionary3[text] = num4.ToString();
		}
		dictionary = superInfectionTelemetryData.BodyData;
		if (dictionary["resource_type_collected_total"] == null)
		{
			dictionary["resource_type_collected_total"] = new Dictionary<string, object>();
		}
		dictionary = superInfectionTelemetryData.BodyData;
		if (dictionary["resource_type_collected_interval"] == null)
		{
			dictionary["resource_type_collected_interval"] = new Dictionary<string, object>();
		}
		Dictionary<string, object> dictionary4 = (Dictionary<string, object>)superInfectionTelemetryData.BodyData["resource_type_collected_total"];
		Dictionary<string, object> dictionary5 = (Dictionary<string, object>)superInfectionTelemetryData.BodyData["resource_type_collected_interval"];
		for (int m = 0; m < 6; m++)
		{
			SIResource.ResourceType resourceType = (SIResource.ResourceType)m;
			int num5;
			resourcesGatheredTotal.TryGetValue(resourceType, out num5);
			int num6;
			resourcesGatheredInterval.TryGetValue(resourceType, out num6);
			string text2 = resourceType.ToString();
			dictionary4[text2] = num5.ToString();
			dictionary5[text2] = num6.ToString();
		}
		superInfectionTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		superInfectionTelemetryData.BodyData["total_play_time"] = totalPlayTime.ToString();
		superInfectionTelemetryData.BodyData["room_play_time"] = roomPlayTime.ToString();
		superInfectionTelemetryData.BodyData["session_play_time"] = sessionPlayTime.ToString();
		superInfectionTelemetryData.BodyData["interval_play_time"] = intervalPlayTime.ToString();
		superInfectionTelemetryData.BodyData["terminal_total_time"] = terminalTotalTime.ToString();
		superInfectionTelemetryData.BodyData["terminal_interval_time"] = terminalIntervalTime.ToString();
		superInfectionTelemetryData.BodyData["time_holding_gadget_type_total"] = timeUsingGadgetsTotal;
		superInfectionTelemetryData.BodyData["time_holding_gadget_type_interval"] = timeUsingGadgetsInterval;
		superInfectionTelemetryData.BodyData["time_holding_own_gadgets_total"] = timeUsingOwnGadgetsTotal.ToString();
		superInfectionTelemetryData.BodyData["time_holding_own_gadgets_interval"] = timeUsingOwnGadgetsInterval.ToString();
		superInfectionTelemetryData.BodyData["time_holding_others_gadgets_total"] = timeUsingOthersGadgetsTotal.ToString();
		superInfectionTelemetryData.BodyData["time_holding_others_gadgets_interval"] = timeUsingOthersGadgetsInterval.ToString();
		superInfectionTelemetryData.BodyData["tags_holding_gadget_type_total"] = dictionary2;
		superInfectionTelemetryData.BodyData["tags_holding_gadget_type_interval"] = dictionary3;
		superInfectionTelemetryData.BodyData["tags_holding_own_gadgets_total"] = tagsHoldingOwnGadgetsTotal.ToString();
		superInfectionTelemetryData.BodyData["tags_holding_own_gadgets_interval"] = tagsHoldingOwnGadgetsInterval.ToString();
		superInfectionTelemetryData.BodyData["tags_holding_others_gadgets_total"] = tagsHoldingOthersGadgetsTotal.ToString();
		superInfectionTelemetryData.BodyData["tags_holding_others_gadgets_interval"] = tagsHoldingOthersGadgetsInterval.ToString();
		superInfectionTelemetryData.BodyData["resource_type_collected_total"] = dictionary4;
		superInfectionTelemetryData.BodyData["resource_type_collected_interval"] = dictionary5;
		superInfectionTelemetryData.BodyData["rounds_played_total"] = roundsPlayedTotal.ToString();
		superInfectionTelemetryData.BodyData["rounds_played_interval"] = roundsPlayedInterval.ToString();
		superInfectionTelemetryData.BodyData["unlocked_nodes"] = new string(span);
		superInfectionTelemetryData.BodyData["player_count"] = numberOfPlayers.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(superInfectionTelemetryData.EventName, superInfectionTelemetryData.BodyData, superInfectionTelemetryData.CustomTags);
	}

	// Token: 0x06003B82 RID: 15234 RVA: 0x001468B4 File Offset: 0x00144AB4
	public static void SuperInfectionEvent(string purchaseType, int shinyRockCost, int techPointsPurchased, float totalPlayTime, float roomPlayTime, float sessionPlayTime)
	{
		if (!GorillaTelemetry.IsConnectedIgnoreRoom())
		{
			return;
		}
		SuperInfectionTelemetryData superInfectionTelemetryData = GorillaTelemetry.gSuperInfectionPurchaseArgs;
		superInfectionTelemetryData.BodyData["event_timestamp"] = DateTime.Now.ToString();
		superInfectionTelemetryData.BodyData["total_play_time"] = totalPlayTime.ToString();
		superInfectionTelemetryData.BodyData["room_play_time"] = roomPlayTime.ToString();
		superInfectionTelemetryData.BodyData["session_play_time"] = sessionPlayTime.ToString();
		superInfectionTelemetryData.BodyData["si_purchase_type"] = purchaseType;
		superInfectionTelemetryData.BodyData["si_shiny_rock_cost"] = shinyRockCost.ToString();
		superInfectionTelemetryData.BodyData["si_tech_points_purchased"] = techPointsPurchased.ToString();
		GorillaTelemetry.EnqueueTelemetryEvent(superInfectionTelemetryData.EventName, superInfectionTelemetryData.BodyData, superInfectionTelemetryData.CustomTags);
	}

	// Token: 0x06003B83 RID: 15235 RVA: 0x00146988 File Offset: 0x00144B88
	public static void PostNotificationEvent(string notificationType)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		Dictionary<string, object> dictionary = GorillaTelemetry.gNotifEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = notificationType;
		GorillaTelemetry.EnqueueTelemetryEvent("telemetry_ggwp_event", dictionary, null);
	}

	// Token: 0x04004BC0 RID: 19392
	private static readonly float TELEMETRY_FLUSH_SEC = 10f;

	// Token: 0x04004BC1 RID: 19393
	private static readonly ConcurrentQueue<MothershipAnalyticsEvent> telemetryEventsQueueMothership = new ConcurrentQueue<MothershipAnalyticsEvent>();

	// Token: 0x04004BC2 RID: 19394
	private static readonly Dictionary<int, List<MothershipAnalyticsEvent>> gListPoolMothership = new Dictionary<int, List<MothershipAnalyticsEvent>>();

	// Token: 0x04004BC3 RID: 19395
	private static PlayFabAuthenticator gPlayFabAuth;

	// Token: 0x04004BC4 RID: 19396
	private static readonly Dictionary<string, object> gZoneEventArgs;

	// Token: 0x04004BC5 RID: 19397
	private static readonly Dictionary<string, object> gCustomMapZoneEventArgs;

	// Token: 0x04004BC6 RID: 19398
	private static readonly Dictionary<string, object> gNotifEventArgs;

	// Token: 0x04004BC7 RID: 19399
	public static float nextStayTimestamp;

	// Token: 0x04004BC8 RID: 19400
	private static readonly Dictionary<string, object> gGameModeStartEventArgs;

	// Token: 0x04004BC9 RID: 19401
	private static readonly Dictionary<string, object> gShopEventArgs;

	// Token: 0x04004BCA RID: 19402
	private static CosmeticsController.CosmeticItem[] gSingleItemParam;

	// Token: 0x04004BCB RID: 19403
	private static BuilderSetManager.BuilderSetStoreItem[] gSingleItemBuilderParam;

	// Token: 0x04004BCC RID: 19404
	private static Dictionary<string, object> gKidEventArgs;

	// Token: 0x04004BCD RID: 19405
	private static readonly Dictionary<string, object> gWamGameStartArgs;

	// Token: 0x04004BCE RID: 19406
	private static readonly Dictionary<string, object> gWamLevelEndArgs;

	// Token: 0x04004BCF RID: 19407
	private static Dictionary<string, object> gCustomMapPerfArgs;

	// Token: 0x04004BD0 RID: 19408
	private static Dictionary<string, object> gCustomMapTrackingMetrics;

	// Token: 0x04004BD1 RID: 19409
	private static Dictionary<string, object> gCustomMapDownloadMetrics;

	// Token: 0x04004BD2 RID: 19410
	private static Dictionary<string, object> gCustomMapRegistryMetrics;

	// Token: 0x04004BD3 RID: 19411
	private static readonly GhostReactorTelemetryData gGhostReactorShiftStartArgs;

	// Token: 0x04004BD4 RID: 19412
	private static readonly GhostReactorTelemetryData gGhostReactorShiftEndArgs;

	// Token: 0x04004BD5 RID: 19413
	private static readonly GhostReactorTelemetryData gGhostReactorFloorStartArgs;

	// Token: 0x04004BD6 RID: 19414
	private static readonly GhostReactorTelemetryData gGhostReactorFloorEndArgs;

	// Token: 0x04004BD7 RID: 19415
	private static readonly GhostReactorTelemetryData gGhostReactorToolPurchasedArgs;

	// Token: 0x04004BD8 RID: 19416
	private static readonly GhostReactorTelemetryData gGhostReactorRankUpArgs;

	// Token: 0x04004BD9 RID: 19417
	private static readonly GhostReactorTelemetryData gGhostReactorToolUnlockArgs;

	// Token: 0x04004BDA RID: 19418
	private static readonly GhostReactorTelemetryData gGhostReactorPodUpgradePurchasedArgs;

	// Token: 0x04004BDB RID: 19419
	private static readonly GhostReactorTelemetryData gGhostReactorToolUpgradeArgs;

	// Token: 0x04004BDC RID: 19420
	private static readonly GhostReactorTelemetryData gGhostReactorChaosSeedStartArgs;

	// Token: 0x04004BDD RID: 19421
	private static readonly GhostReactorTelemetryData gGhostReactorChaosJuiceCollectedArgs;

	// Token: 0x04004BDE RID: 19422
	private static readonly GhostReactorTelemetryData gGhostReactorOverdrivePurchasedArgs;

	// Token: 0x04004BDF RID: 19423
	private static readonly GhostReactorTelemetryData gGhostReactorCreditsRefillPurchasedArgs;

	// Token: 0x04004BE0 RID: 19424
	private static readonly SuperInfectionTelemetryData gSuperInfectionArgs;

	// Token: 0x04004BE1 RID: 19425
	private static readonly SuperInfectionTelemetryData gSuperInfectionPurchaseArgs;

	// Token: 0x020008DB RID: 2267
	public static class k
	{
		// Token: 0x04004BE2 RID: 19426
		public const string User = "User";

		// Token: 0x04004BE3 RID: 19427
		public const string ZoneId = "ZoneId";

		// Token: 0x04004BE4 RID: 19428
		public const string SubZoneId = "SubZoneId";

		// Token: 0x04004BE5 RID: 19429
		public const string EventType = "EventType";

		// Token: 0x04004BE6 RID: 19430
		public const string IsPrivateRoom = "IsPrivateRoom";

		// Token: 0x04004BE7 RID: 19431
		public const string Items = "Items";

		// Token: 0x04004BE8 RID: 19432
		public const string VoiceChatEnabled = "VoiceChatEnabled";

		// Token: 0x04004BE9 RID: 19433
		public const string JoinGroups = "JoinGroups";

		// Token: 0x04004BEA RID: 19434
		public const string CustomUsernameEnabled = "CustomUsernameEnabled";

		// Token: 0x04004BEB RID: 19435
		public const string AgeCategory = "AgeCategory";

		// Token: 0x04004BEC RID: 19436
		public const string telemetry_zone_event = "telemetry_zone_event";

		// Token: 0x04004BED RID: 19437
		public const string telemetry_shop_event = "telemetry_shop_event";

		// Token: 0x04004BEE RID: 19438
		public const string telemetry_kid_event = "telemetry_kid_event";

		// Token: 0x04004BEF RID: 19439
		public const string telemetry_ggwp_event = "telemetry_ggwp_event";

		// Token: 0x04004BF0 RID: 19440
		public const string NOTHING = "NOTHING";

		// Token: 0x04004BF1 RID: 19441
		public const string telemetry_wam_gameStartEvent = "telemetry_wam_gameStartEvent";

		// Token: 0x04004BF2 RID: 19442
		public const string telemetry_wam_levelEndEvent = "telemetry_wam_levelEndEvent";

		// Token: 0x04004BF3 RID: 19443
		public const string WamMachineId = "WamMachineId";

		// Token: 0x04004BF4 RID: 19444
		public const string WamGameId = "WamGameId";

		// Token: 0x04004BF5 RID: 19445
		public const string WamMLevelNumber = "WamMLevelNumber";

		// Token: 0x04004BF6 RID: 19446
		public const string WamGoodMolesShown = "WamGoodMolesShown";

		// Token: 0x04004BF7 RID: 19447
		public const string WamHazardMolesShown = "WamHazardMolesShown";

		// Token: 0x04004BF8 RID: 19448
		public const string WamLevelMinScore = "WamLevelMinScore";

		// Token: 0x04004BF9 RID: 19449
		public const string WamLevelScore = "WamLevelScore";

		// Token: 0x04004BFA RID: 19450
		public const string WamHazardMolesHit = "WamHazardMolesHit";

		// Token: 0x04004BFB RID: 19451
		public const string WamGameState = "WamGameState";

		// Token: 0x04004BFC RID: 19452
		public const string CustomMapName = "CustomMapName";

		// Token: 0x04004BFD RID: 19453
		public const string LowestFPS = "LowestFPS";

		// Token: 0x04004BFE RID: 19454
		public const string LowestFPSDrawCalls = "LowestFPSDrawCalls";

		// Token: 0x04004BFF RID: 19455
		public const string LowestFPSPlayerCount = "LowestFPSPlayerCount";

		// Token: 0x04004C00 RID: 19456
		public const string AverageFPS = "AverageFPS";

		// Token: 0x04004C01 RID: 19457
		public const string AverageDrawCalls = "AverageDrawCalls";

		// Token: 0x04004C02 RID: 19458
		public const string AveragePlayerCount = "AveragePlayerCount";

		// Token: 0x04004C03 RID: 19459
		public const string HighestFPS = "HighestFPS";

		// Token: 0x04004C04 RID: 19460
		public const string HighestFPSDrawCalls = "HighestFPSDrawCalls";

		// Token: 0x04004C05 RID: 19461
		public const string HighestFPSPlayerCount = "HighestFPSPlayerCount";

		// Token: 0x04004C06 RID: 19462
		public const string CustomMapCreator = "CustomMapCreator";

		// Token: 0x04004C07 RID: 19463
		public const string CustomMapModId = "CustomMapModId";

		// Token: 0x04004C08 RID: 19464
		public const string MinPlayerCount = "MinPlayerCount";

		// Token: 0x04004C09 RID: 19465
		public const string MaxPlayerCount = "MaxPlayerCount";

		// Token: 0x04004C0A RID: 19466
		public const string PlaytimeOnMap = "PlaytimeOnMap";

		// Token: 0x04004C0B RID: 19467
		public const string PlaytimeInSeconds = "PlaytimeInSeconds";

		// Token: 0x04004C0C RID: 19468
		public const string PrivateRoom = "PrivateRoom";

		// Token: 0x04004C0D RID: 19469
		public const string MapId = "MapId";

		// Token: 0x04004C0E RID: 19470
		public const string MapSource = "MapSource";

		// Token: 0x04004C0F RID: 19471
		public const string CustomMapRegistry = "CustomMapRegistry";

		// Token: 0x04004C10 RID: 19472
		public const string CreatorId = "CreatorId";

		// Token: 0x04004C11 RID: 19473
		public const string CreatorUsername = "CreatorUsername";

		// Token: 0x04004C12 RID: 19474
		public const string MapDateLive = "MapDateLive";

		// Token: 0x04004C13 RID: 19475
		public const string MapDateUpdated = "MapDateUpdated";

		// Token: 0x04004C14 RID: 19476
		public const string MapTags = "MapTags";

		// Token: 0x04004C15 RID: 19477
		public const string MapSupportVersion = "MapSupportVersion";

		// Token: 0x04004C16 RID: 19478
		public const string MapMaxPlayers = "MapMaxPlayers";

		// Token: 0x04004C17 RID: 19479
		public const string MapHasCustomGameMode = "MapHasCustomGameMode";

		// Token: 0x04004C18 RID: 19480
		public const string MapGravityZoneCount = "MapGravityZoneCount";

		// Token: 0x04004C19 RID: 19481
		public const string MapSizeChangerCount = "MapSizeChangerCount";

		// Token: 0x04004C1A RID: 19482
		public const string MapHandHoldCount = "MapHandHoldCount";

		// Token: 0x04004C1B RID: 19483
		public const string MapMapperAssetCount = "MapMapperAssetCount";

		// Token: 0x04004C1C RID: 19484
		public const string game_mode_played_event = "game_mode_played_event";

		// Token: 0x04004C1D RID: 19485
		public const string game_mode = "game_mode";
	}

	// Token: 0x020008DC RID: 2268
	private class BatchRunner : MonoBehaviour
	{
		// Token: 0x06003B84 RID: 15236 RVA: 0x001469CD File Offset: 0x00144BCD
		private IEnumerator Start()
		{
			for (;;)
			{
				float start = Time.realtimeSinceStartup;
				while (Time.realtimeSinceStartup < start + GorillaTelemetry.TELEMETRY_FLUSH_SEC)
				{
					yield return null;
				}
				GorillaTelemetry.FlushMothershipTelemetry();
			}
			yield break;
		}
	}
}
