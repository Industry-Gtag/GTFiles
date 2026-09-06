using System;
using System.Collections.Generic;
using KID.Model;
using UnityEngine;

// Token: 0x02000BB7 RID: 2999
public class KIDUI_AgeAppealController : MonoBehaviour
{
	// Token: 0x17000744 RID: 1860
	// (get) Token: 0x06004BBF RID: 19391 RVA: 0x00193C64 File Offset: 0x00191E64
	public static KIDUI_AgeAppealController Instance
	{
		get
		{
			return KIDUI_AgeAppealController._instance;
		}
	}

	// Token: 0x06004BC0 RID: 19392 RVA: 0x00193C6B File Offset: 0x00191E6B
	private void Awake()
	{
		KIDUI_AgeAppealController._instance = this;
		Debug.LogFormat("[KID::UI::AGEAPPEALCONTROLLER] Controller Initialised", Array.Empty<object>());
	}

	// Token: 0x06004BC1 RID: 19393 RVA: 0x00193C84 File Offset: 0x00191E84
	public void StartAgeAppealScreens(SessionStatus? sessionStatus)
	{
		Debug.LogFormat("[KID::UI::AGEAPPEALCONTROLLER] Showing k-ID Age Appeal Screens", Array.Empty<object>());
		HandRayController.Instance.EnableHandRays();
		PrivateUIRoom.AddUI(base.transform);
		this._firstAgeAppealScreen.ShowRestrictedAccessScreen(sessionStatus);
		AgeStatusType ageStatusType;
		if (KIDManager.TryGetAgeStatusTypeFromAge(KIDAgeGate.UserAge, out ageStatusType))
		{
			TelemetryData telemetryData = new TelemetryData
			{
				EventName = "kid_age_appeal",
				CustomTags = new string[]
				{
					"kid_age_appeal",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment
				},
				BodyData = new Dictionary<string, string> { 
				{
					"submitted_age",
					ageStatusType.ToString()
				} }
			};
			GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		}
	}

	// Token: 0x06004BC2 RID: 19394 RVA: 0x00193D45 File Offset: 0x00191F45
	public void CloseKIDScreens()
	{
		PrivateUIRoom.RemoveUI(base.transform);
		HandRayController.Instance.DisableHandRays();
		this._firstAgeAppealScreen.gameObject.SetActive(false);
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x06004BC3 RID: 19395 RVA: 0x00193D78 File Offset: 0x00191F78
	public void StartTooYoungToPlayScreen()
	{
		Debug.LogFormat("[KID::UI::AGEAPPEALCONTROLLER] Showing k-ID Too Young to Play Screen", Array.Empty<object>());
		HandRayController.Instance.EnableHandRays();
		PrivateUIRoom.AddUI(base.transform);
		this._tooYoungToPlayScreen.ShowTooYoungToPlayScreen();
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { { "screen", "blocked" } }
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
	}

	// Token: 0x06004BC4 RID: 19396 RVA: 0x00193E22 File Offset: 0x00192022
	public void OnQuitGamePressed()
	{
		Application.Quit();
	}

	// Token: 0x06004BC5 RID: 19397 RVA: 0x0018FAF0 File Offset: 0x0018DCF0
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005E94 RID: 24212
	private static KIDUI_AgeAppealController _instance;

	// Token: 0x04005E95 RID: 24213
	[SerializeField]
	private KIDUI_RestrictedAccessScreen _firstAgeAppealScreen;

	// Token: 0x04005E96 RID: 24214
	[SerializeField]
	private KIDUI_TooYoungToPlay _tooYoungToPlayScreen;
}
