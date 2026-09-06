using System;
using UnityEngine;

// Token: 0x02000B8D RID: 2957
public static class KIDTelemetry
{
	// Token: 0x1700072A RID: 1834
	// (get) Token: 0x06004ABE RID: 19134 RVA: 0x00039037 File Offset: 0x00037237
	public static string GameVersionCustomTag
	{
		get
		{
			return "game_version_" + Application.version;
		}
	}

	// Token: 0x1700072B RID: 1835
	// (get) Token: 0x06004ABF RID: 19135 RVA: 0x0018F7AA File Offset: 0x0018D9AA
	public static string Open_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Open";
		}
	}

	// Token: 0x1700072C RID: 1836
	// (get) Token: 0x06004AC0 RID: 19136 RVA: 0x0018F7B1 File Offset: 0x0018D9B1
	public static string Updated_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Updated";
		}
	}

	// Token: 0x1700072D RID: 1837
	// (get) Token: 0x06004AC1 RID: 19137 RVA: 0x0018F7B8 File Offset: 0x0018D9B8
	public static string Closed_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Closed";
		}
	}

	// Token: 0x1700072E RID: 1838
	// (get) Token: 0x06004AC2 RID: 19138 RVA: 0x00039048 File Offset: 0x00037248
	public static string GameEnvironment
	{
		get
		{
			return "game_environment_live";
		}
	}

	// Token: 0x06004AC3 RID: 19139 RVA: 0x0018F7BF File Offset: 0x0018D9BF
	public static string GetPermissionManagedByBodyData(string permission)
	{
		return "permission_managedby_" + permission.Replace('-', '_');
	}

	// Token: 0x06004AC4 RID: 19140 RVA: 0x0018F7D5 File Offset: 0x0018D9D5
	public static string GetPermissionEnabledBodyData(string permission)
	{
		return "permission_eneabled_" + permission.Replace('-', '_');
	}

	// Token: 0x04005D7E RID: 23934
	public const string SCREEN_SHOWN_EVENT_NAME = "kid_screen_shown";

	// Token: 0x04005D7F RID: 23935
	public const string PHASE_TWO_IN_COHORT_EVENT_NAME = "kid_phase2_incohort";

	// Token: 0x04005D80 RID: 23936
	public const string PHASE_THREE_OPTIONAL_EVENT_NAME = "kid_phase3_optional";

	// Token: 0x04005D81 RID: 23937
	public const string AGE_GATE_EVENT_NAME = "kid_age_gate";

	// Token: 0x04005D82 RID: 23938
	public const string AGE_GATE_CONFIRM_EVENT_NAME = "kid_age_gate_confirm";

	// Token: 0x04005D83 RID: 23939
	public const string AGE_DISCREPENCY_EVENT_NAME = "kid_age_gate_discrepency";

	// Token: 0x04005D84 RID: 23940
	public const string GAME_SETTINGS_EVENT_NAME = "kid_game_settings";

	// Token: 0x04005D85 RID: 23941
	public const string EMAIL_CONFIRM_EVENT_NAME = "kid_email_confirm";

	// Token: 0x04005D86 RID: 23942
	public const string AGE_APPEAL_EVENT_NAME = "kid_age_appeal";

	// Token: 0x04005D87 RID: 23943
	public const string APPEAL_AGE_GATE_EVENT_NAME = "kid_age_appeal_age_gate";

	// Token: 0x04005D88 RID: 23944
	public const string APPEAL_ENTER_EMAIL_EVENT_NAME = "kid_age_appeal_enter_email";

	// Token: 0x04005D89 RID: 23945
	public const string APPEAL_CONFIRM_EMAIL_EVENT_NAME = "kid_age_appeal_confirm_email";

	// Token: 0x04005D8A RID: 23946
	private const string GAME_VERSION_CUSTOM_TAG_PREFIX = "game_version_";

	// Token: 0x04005D8B RID: 23947
	private const string METRIC_ACTION_CUSTOM_TAG_PREFIX = "metric_action_";

	// Token: 0x04005D8C RID: 23948
	public const string WARNING_SCREEN_CUSTOM_TAG = "kid_warning_screen";

	// Token: 0x04005D8D RID: 23949
	public const string PHASE_TWO = "kid_phase_2";

	// Token: 0x04005D8E RID: 23950
	public const string PHASE_THREE = "kid_phase_3";

	// Token: 0x04005D8F RID: 23951
	public const string PHASE_FOUR = "kid_phase_4";

	// Token: 0x04005D90 RID: 23952
	public const string AGE_GATE_CUSTOM_TAG = "kid_age_gate";

	// Token: 0x04005D91 RID: 23953
	public const string SETTINGS_CUSTOM_TAG = "kid_settings";

	// Token: 0x04005D92 RID: 23954
	public const string SETUP_CUSTOM_TAG = "kid_setup";

	// Token: 0x04005D93 RID: 23955
	public const string APPEAL_CUSTOM_TAG = "kid_age_appeal";

	// Token: 0x04005D94 RID: 23956
	public const string SCREEN_TYPE_BODY_DATA = "screen";

	// Token: 0x04005D95 RID: 23957
	public const string OPT_IN_CHOICE_BODY_DATA = "opt_in_choice";

	// Token: 0x04005D96 RID: 23958
	public const string BUTTON_PRESSED_BODY_DATA = "button_pressed";

	// Token: 0x04005D97 RID: 23959
	public const string MISMATCH_EXPECTED_BODY_DATA = "mismatch_expected";

	// Token: 0x04005D98 RID: 23960
	public const string MISMATCH_ACTUAL_BODY_DATA = "mismatch_actual";

	// Token: 0x04005D99 RID: 23961
	public const string AGE_DECLARED_BODY_DATA = "age_declared";

	// Token: 0x04005D9A RID: 23962
	public const string LEARN_MORE_URL_PRESSED_BODY_DATA = "learn_more_url_pressed";

	// Token: 0x04005D9B RID: 23963
	public const string SCREEN_SHOWN_REASON_BODY_DATA = "screen_shown_reason";

	// Token: 0x04005D9C RID: 23964
	public const string SUBMITTED_AGE_BODY_DATA = "submitted_age";

	// Token: 0x04005D9D RID: 23965
	public const string CORRECT_AGE_BODY_DATA = "correct_age";

	// Token: 0x04005D9E RID: 23966
	public const string APPEAL_EMAIL_TYPE_BODY_DATA = "email_type";

	// Token: 0x04005D9F RID: 23967
	public const string SHOWN_SETTINGS_SCREEN = "saw_game_settings";

	// Token: 0x04005DA0 RID: 23968
	public const string KID_STATUS_BODY_DATA = "kid_status";

	// Token: 0x04005DA1 RID: 23969
	private const string PERMISSION_MANAGED_BY_BODY_DATA = "permission_managedby_";

	// Token: 0x04005DA2 RID: 23970
	private const string PERMISSION_ENABLED_BODY_DATA = "permission_eneabled_";
}
