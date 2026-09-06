using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
using UnityEngine;

// Token: 0x02000BC7 RID: 3015
public class KIDUI_ConfirmScreen : MonoBehaviour
{
	// Token: 0x06004C05 RID: 19461 RVA: 0x0019523C File Offset: 0x0019343C
	private void Awake()
	{
		if (this._emailToConfirmTxt == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Email To Confirm Field is NULL", Array.Empty<object>());
			return;
		}
		if (this._setupScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Setup K-ID Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._mainScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Main Screen is NULL", Array.Empty<object>());
			return;
		}
		this._cancellationTokenSource = new CancellationTokenSource();
	}

	// Token: 0x06004C06 RID: 19462 RVA: 0x001952AE File Offset: 0x001934AE
	private void OnEnable()
	{
		this._confirmButton.interactable = true;
		this._backButton.interactable = true;
	}

	// Token: 0x06004C07 RID: 19463 RVA: 0x001952C8 File Offset: 0x001934C8
	public void OnEmailSubmitted(string emailAddress)
	{
		this._submittedEmailAddress = emailAddress;
		this._emailToConfirmTxt.text = this._submittedEmailAddress;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004C08 RID: 19464 RVA: 0x001952F0 File Offset: 0x001934F0
	public void OnConfirmPressed()
	{
		KIDUI_ConfirmScreen.<OnConfirmPressed>d__16 <OnConfirmPressed>d__;
		<OnConfirmPressed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnConfirmPressed>d__.<>4__this = this;
		<OnConfirmPressed>d__.<>1__state = -1;
		<OnConfirmPressed>d__.<>t__builder.Start<KIDUI_ConfirmScreen.<OnConfirmPressed>d__16>(ref <OnConfirmPressed>d__);
	}

	// Token: 0x06004C09 RID: 19465 RVA: 0x00195328 File Offset: 0x00193528
	public async void OnBackPressed()
	{
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_email_confirm",
			CustomTags = new string[]
			{
				"kid_setup",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { { "button_pressed", "go_back" } }
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		this._cancellationTokenSource.Cancel();
		await this._animatedEllipsis.StopAnimation();
		base.gameObject.SetActive(false);
		this._setupScreen.OnStartSetup();
	}

	// Token: 0x06004C0A RID: 19466 RVA: 0x0019535F File Offset: 0x0019355F
	public void NotifyOfResult(bool success)
	{
		this._hasCompletedSendEmailRequest = true;
		this._emailRequestResult = success;
	}

	// Token: 0x06004C0B RID: 19467 RVA: 0x00195370 File Offset: 0x00193570
	private async void ShowErrorScreen(string errorMessage)
	{
		Debug.LogErrorFormat("[KID::UI::Setup] K-ID Confirmation Failed - Failed to send email", Array.Empty<object>());
		this._cancellationTokenSource.Cancel();
		await this._animatedEllipsis.StopAnimation();
		base.gameObject.SetActive(false);
		this._errorScreen.ShowErrorScreen("Confirmation Error", this._submittedEmailAddress, errorMessage);
	}

	// Token: 0x06004C0C RID: 19468 RVA: 0x0018FAF0 File Offset: 0x0018DCF0
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005EF4 RID: 24308
	[SerializeField]
	private TMP_Text _emailToConfirmTxt;

	// Token: 0x04005EF5 RID: 24309
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x04005EF6 RID: 24310
	[SerializeField]
	private KIDUI_SetupScreen _setupScreen;

	// Token: 0x04005EF7 RID: 24311
	[SerializeField]
	private KIDUI_ErrorScreen _errorScreen;

	// Token: 0x04005EF8 RID: 24312
	[SerializeField]
	private KIDUI_EmailSuccess _successScreen;

	// Token: 0x04005EF9 RID: 24313
	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	// Token: 0x04005EFA RID: 24314
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x04005EFB RID: 24315
	[SerializeField]
	private KIDUIButton _backButton;

	// Token: 0x04005EFC RID: 24316
	[SerializeField]
	private int _minimumDelay = 1000;

	// Token: 0x04005EFD RID: 24317
	private string _submittedEmailAddress;

	// Token: 0x04005EFE RID: 24318
	private CancellationTokenSource _cancellationTokenSource;

	// Token: 0x04005EFF RID: 24319
	private bool _hasCompletedSendEmailRequest;

	// Token: 0x04005F00 RID: 24320
	private bool _emailRequestResult;
}
