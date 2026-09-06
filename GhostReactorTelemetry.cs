using System;
using UnityEngine;

// Token: 0x0200072C RID: 1836
public class GhostReactorTelemetry : MonoBehaviour
{
	// Token: 0x1700047E RID: 1150
	// (get) Token: 0x06002EAE RID: 11950 RVA: 0x00039037 File Offset: 0x00037237
	public static string GameVersionCustomTag
	{
		get
		{
			return "game_version_" + Application.version;
		}
	}

	// Token: 0x1700047F RID: 1151
	// (get) Token: 0x06002EAF RID: 11951 RVA: 0x00039048 File Offset: 0x00037248
	public static string GameEnvironment
	{
		get
		{
			return "game_environment_live";
		}
	}

	// Token: 0x04003B8C RID: 15244
	public const string SHIFT_START_EVENT_NAME = "ghost_game_start";

	// Token: 0x04003B8D RID: 15245
	public const string SHIFT_END_EVENT_NAME = "ghost_game_end";

	// Token: 0x04003B8E RID: 15246
	public const string FLOOR_START_EVENT_NAME = "ghost_floor_start";

	// Token: 0x04003B8F RID: 15247
	public const string FLOOR_END_EVENT_NAME = "ghost_floor_end";

	// Token: 0x04003B90 RID: 15248
	public const string TOOL_PURCHASED_EVENT_NAME = "ghost_tool_purchased";

	// Token: 0x04003B91 RID: 15249
	public const string RANK_UP_EVENT_NAME = "ghost_game_rank_up";

	// Token: 0x04003B92 RID: 15250
	public const string TOOL_UNLOCK_EVENT_NAME = "ghost_game_tool_unlock";

	// Token: 0x04003B93 RID: 15251
	public const string POD_UPGRADE_PURCHASED_EVENT_NAME = "ghost_pod_upgrade_purchased";

	// Token: 0x04003B94 RID: 15252
	public const string TOOL_UPGRADE_EVENT_NAME = "ghost_game_tool_upgrade";

	// Token: 0x04003B95 RID: 15253
	public const string CHAOS_SEED_START_EVENT_NAME = "ghost_chaos_seed_start";

	// Token: 0x04003B96 RID: 15254
	public const string CHAOS_JUICE_COLLECTED_EVENT_NAME = "ghost_chaos_juice_collected";

	// Token: 0x04003B97 RID: 15255
	public const string OVERDRIVE_PURCHASED_EVENT_NAME = "ghost_overdrive_purchased";

	// Token: 0x04003B98 RID: 15256
	public const string CREDITS_REFILL_PURCHASED_EVENT_NAME = "ghost_credits_refill_purchased";

	// Token: 0x04003B99 RID: 15257
	private const string GAME_VERSION_CUSTOM_TAG_PREFIX = "game_version_";

	// Token: 0x04003B9A RID: 15258
	private const string METRIC_ACTION_CUSTOM_TAG_PREFIX = "metric_action_";

	// Token: 0x04003B9B RID: 15259
	public const string GHOST_GAME_ID_BODY_DATA = "ghost_game_id";

	// Token: 0x04003B9C RID: 15260
	public const string EVENT_TIMESTAMP_BODY_DATA = "event_timestamp";

	// Token: 0x04003B9D RID: 15261
	public const string INITIAL_CORES_BALANCE_BODY_DATA = "initial_cores_balance";

	// Token: 0x04003B9E RID: 15262
	public const string FINAL_CORES_BALANCE_BODY_DATA = "final_cores_balance";

	// Token: 0x04003B9F RID: 15263
	public const string CORES_SPENT_WAITING_IN_BREAKROOM_BODY_DATA = "cores_spent_waiting_in_breakroom";

	// Token: 0x04003BA0 RID: 15264
	public const string CORES_COLLECTED_FROM_GHOSTS_BODY_DATA = "cores_collected_from_ghosts";

	// Token: 0x04003BA1 RID: 15265
	public const string CORES_COLLECTED_FROM_GATHERING_BODY_DATA = "cores_collected_from_gathering";

	// Token: 0x04003BA2 RID: 15266
	public const string CORES_SPENT_ON_ITEMS_BODY_DATA = "cores_spent_on_items";

	// Token: 0x04003BA3 RID: 15267
	public const string CORES_SPENT_ON_GATES_BODY_DATA = "cores_spent_on_gates";

	// Token: 0x04003BA4 RID: 15268
	public const string CORES_SPENT_ON_LEVELS_BODY_DATA = "cores_spent_on_levels";

	// Token: 0x04003BA5 RID: 15269
	public const string CORES_GIVEN_TO_OTHERS_BODY_DATA = "cores_given_to_others";

	// Token: 0x04003BA6 RID: 15270
	public const string CORES_RECEIVED_FROM_OTHERS_BODY_DATA = "cores_received_from_others";

	// Token: 0x04003BA7 RID: 15271
	public const string SHIFT_CUT_DATA = "shift_cut_data";

	// Token: 0x04003BA8 RID: 15272
	public const string GATES_UNLOCKED_BODY_DATA = "gates_unlocked";

	// Token: 0x04003BA9 RID: 15273
	public const string DIED_BODY_DATA = "died";

	// Token: 0x04003BAA RID: 15274
	public const string CAUGHT_IN_ANAMOLE_BODY_DATA = "caught_in_anamole";

	// Token: 0x04003BAB RID: 15275
	public const string ITEMS_PURCHASED_BODY_DATA = "items_purchased";

	// Token: 0x04003BAC RID: 15276
	public const string LEVELS_UNLOCKED_BODY_DATA = "levels_unlocked";

	// Token: 0x04003BAD RID: 15277
	public const string NUMBER_OF_PLAYERS_BODY_DATA = "number_of_players";

	// Token: 0x04003BAE RID: 15278
	public const string START_AT_BEGINNING_BODY_DATA = "start_at_beginning";

	// Token: 0x04003BAF RID: 15279
	public const string SECONDS_INTO_SHIFT_AT_JOIN_BODY_DATA = "seconds_into_shift_at_join";

	// Token: 0x04003BB0 RID: 15280
	public const string REASON_BODY_DATA = "reason";

	// Token: 0x04003BB1 RID: 15281
	public const string PLAY_DURATION_BODY_DATA = "play_duration";

	// Token: 0x04003BB2 RID: 15282
	public const string STARTED_LATE_BODY_DATA = "started_late";

	// Token: 0x04003BB3 RID: 15283
	public const string TIME_STARTED_BODY_DATA = "time_started";

	// Token: 0x04003BB4 RID: 15284
	public const string CORES_COLLECTED_BODY_DATA = "cores_collected";

	// Token: 0x04003BB5 RID: 15285
	public const string MAX_NUMBER_IN_GAME_BODY_DATA = "max_number_in_game";

	// Token: 0x04003BB6 RID: 15286
	public const string END_NUMBER_IN_GAME_BODY_DATA = "end_number_in_game";

	// Token: 0x04003BB7 RID: 15287
	public const string ITEMS_PICKED_UP_BODY_DATA = "items_picked_up";

	// Token: 0x04003BB8 RID: 15288
	public const string FLOOR_JOINED_BODY_DATA = "floor_joined";

	// Token: 0x04003BB9 RID: 15289
	public const string PLAYER_RANK_BODY_DATA = "player_rank";

	// Token: 0x04003BBA RID: 15290
	public const string TOTAL_CORES_COLLECTED_BY_PLAYER_BODY_DATA = "total_cores_collected_by_player";

	// Token: 0x04003BBB RID: 15291
	public const string TOTAL_CORES_COLLECTED_BY_GROUP_BODY_DATA = "total_cores_collected_by_group";

	// Token: 0x04003BBC RID: 15292
	public const string TOTAL_CORES_SPENT_BY_PLAYER_BODY_DATA = "total_cores_spent_by_player";

	// Token: 0x04003BBD RID: 15293
	public const string TOTAL_CORES_SPENT_BY_GROUP_BODY_DATA = "total_cores_spent_by_group";

	// Token: 0x04003BBE RID: 15294
	public const string FLOOR_BODY_DATA = "floor";

	// Token: 0x04003BBF RID: 15295
	public const string PRESET_BODY_DATA = "preset";

	// Token: 0x04003BC0 RID: 15296
	public const string MODIFIER_BODY_DATA = "modifier";

	// Token: 0x04003BC1 RID: 15297
	public const string SECTION_BODY_DATA = "section";

	// Token: 0x04003BC2 RID: 15298
	public const string XP_GAINED_BODY_DATA = "xp_gained";

	// Token: 0x04003BC3 RID: 15299
	public const string CHAOS_SEEDS_COLLECTED_BODY_DATA = "chaos_seeds_collected";

	// Token: 0x04003BC4 RID: 15300
	public const string OBJECTIVES_COMPLETED_BODY_DATA = "objectives_completed";

	// Token: 0x04003BC5 RID: 15301
	public const string REVIVES_BODY_DATA = "revives";

	// Token: 0x04003BC6 RID: 15302
	public const string TOOL_BODY_DATA = "tool";

	// Token: 0x04003BC7 RID: 15303
	public const string TOOL_LEVEL_BODY_DATA = "tool_level";

	// Token: 0x04003BC8 RID: 15304
	public const string CORES_SPENT_BODY_DATA = "cores_spent";

	// Token: 0x04003BC9 RID: 15305
	public const string SHINY_ROCKS_SPENT_BODY_DATA = "shiny_rocks_spent";

	// Token: 0x04003BCA RID: 15306
	public const string NEW_RANK_BODY_DATA = "new_rank";

	// Token: 0x04003BCB RID: 15307
	public const string UPGRADE_BODY_DATA = "upgrade";

	// Token: 0x04003BCC RID: 15308
	public const string GRIFT_PRICE_BODY_DATA = "grift_price";

	// Token: 0x04003BCD RID: 15309
	public const string TYPE_BODY_DATA = "type";

	// Token: 0x04003BCE RID: 15310
	public const string NEW_LEVEL_BODY_DATA = "new_level";

	// Token: 0x04003BCF RID: 15311
	public const string JUICE_SPENT_BODY_DATA = "juice_spent";

	// Token: 0x04003BD0 RID: 15312
	public const string GRIFT_SPENT_BODY_DATA = "grift_spent";

	// Token: 0x04003BD1 RID: 15313
	public const string CHAOS_SEEDS_IN_QUEUE_BODY_DATA = "chaos_seeds_in_queue";

	// Token: 0x04003BD2 RID: 15314
	public const string UNLOCK_TIME_BODY_DATA = "unlock_time";

	// Token: 0x04003BD3 RID: 15315
	public const string SHINY_ROCKS_USED_BODY_DATA = "shiny_rocks_used";

	// Token: 0x04003BD4 RID: 15316
	public const string JUICE_COLLECTED_BODY_DATA = "juice_collected";

	// Token: 0x04003BD5 RID: 15317
	public const string CORES_PROCESSED_BY_OVERDRIVE_BODY_DATA = "cores_processed_by_overdrive";

	// Token: 0x04003BD6 RID: 15318
	public const string FINAL_CREDITS_BODY_DATA = "final_credits";

	// Token: 0x04003BD7 RID: 15319
	public const string IS_PRIVATE_ROOM_BODY_DATA = "is_private_room";

	// Token: 0x04003BD8 RID: 15320
	public const string NUM_SHIFTS_PLAYED_BODY_DATA = "num_shifts_played";
}
