using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000BD0 RID: 3024
public class KIDUI_EmailSuccess : MonoBehaviour
{
	// Token: 0x06004C2E RID: 19502 RVA: 0x00196134 File Offset: 0x00194334
	public void ShowSuccessScreen(string email)
	{
		this._emailTxt.text = email;
		base.gameObject.SetActive(true);
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_setup",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { { "screen", "email_sent" } }
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
	}

	// Token: 0x06004C2F RID: 19503 RVA: 0x001961C8 File Offset: 0x001943C8
	public void ShowSuccessScreenAppeal(string email)
	{
		this._emailTxt.text = email;
		base.gameObject.SetActive(true);
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { { "screen", "age_appeal_email_sent" } }
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
	}

	// Token: 0x06004C30 RID: 19504 RVA: 0x0019625B File Offset: 0x0019445B
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Pending);
	}

	// Token: 0x06004C31 RID: 19505 RVA: 0x00193E22 File Offset: 0x00192022
	public void OnCloseGame()
	{
		Application.Quit();
	}

	// Token: 0x04005F2F RID: 24367
	[SerializeField]
	private TMP_Text _emailTxt;

	// Token: 0x04005F30 RID: 24368
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;
}
