using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000BB9 RID: 3001
public class KIDUI_AgeAppealEmailConfirmation : MonoBehaviour
{
	// Token: 0x06004BCC RID: 19404 RVA: 0x00193FD1 File Offset: 0x001921D1
	private void OnEnable()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Combine(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x06004BCD RID: 19405 RVA: 0x00193FF3 File Offset: 0x001921F3
	private void OnDisable()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x06004BCE RID: 19406 RVA: 0x00194028 File Offset: 0x00192228
	public void ShowAgeAppealConfirmationScreen(bool hasChallenge, int newAge, string emailToConfirm)
	{
		this.hasChallenge = hasChallenge;
		this.newAgeToAppeal = newAge;
		this._confirmText.text = (this.hasChallenge ? this.CONFIRM_PARENT_EMAIL : this.CONFIRM_YOUR_EMAIL);
		this._emailText.text = emailToConfirm;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004BCF RID: 19407 RVA: 0x0019407C File Offset: 0x0019227C
	public void OnConfirmPressed()
	{
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_age_appeal_confirm_email",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"email_type",
					this.hasChallenge ? "under_dac" : "over_dac"
				},
				{ "button_pressed", "confirm" }
			}
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		if (this.hasChallenge)
		{
			this.StartAgeAppealChallengeEmail();
			return;
		}
		this.StartAgeAppealEmail();
	}

	// Token: 0x06004BD0 RID: 19408 RVA: 0x0019412C File Offset: 0x0019232C
	public void OnBackPressed()
	{
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_age_appeal_confirm_email",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"email_type",
					this.hasChallenge ? "under_dac" : "over_dac"
				},
				{ "button_pressed", "go_back" }
			}
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		base.gameObject.SetActive(false);
		this._ageAppealEmailScreen.ShowAgeAppealEmailScreen(this.hasChallenge, this.newAgeToAppeal);
	}

	// Token: 0x06004BD1 RID: 19409 RVA: 0x001941EC File Offset: 0x001923EC
	private void StartAgeAppealChallengeEmail()
	{
		KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealChallengeEmail>d__16 <StartAgeAppealChallengeEmail>d__;
		<StartAgeAppealChallengeEmail>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartAgeAppealChallengeEmail>d__.<>4__this = this;
		<StartAgeAppealChallengeEmail>d__.<>1__state = -1;
		<StartAgeAppealChallengeEmail>d__.<>t__builder.Start<KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealChallengeEmail>d__16>(ref <StartAgeAppealChallengeEmail>d__);
	}

	// Token: 0x06004BD2 RID: 19410 RVA: 0x00194224 File Offset: 0x00192424
	private async Task StartAgeAppealEmail()
	{
		TaskAwaiter<bool> taskAwaiter = KIDManager.TryAppealAge(this._emailText.text, this.newAgeToAppeal).GetAwaiter();
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			TaskAwaiter<bool> taskAwaiter2;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		if (!taskAwaiter.GetResult())
		{
			base.gameObject.SetActive(false);
			this._errorScreen.ShowAgeAppealEmailErrorScreen(this.hasChallenge, this.newAgeToAppeal, this._emailText.text);
		}
		else
		{
			Debug.Log("[KID::UI::APPEAL_AGE_EMAIL] Age appeal succesful for [" + this._emailText.text + "]. Proceeding tu Success screen");
			base.gameObject.SetActive(false);
			this._successScreen.ShowSuccessScreenAppeal(this._emailText.text);
		}
	}

	// Token: 0x06004BD3 RID: 19411 RVA: 0x00194268 File Offset: 0x00192468
	private void NotifyOfEmailResult(bool success)
	{
		if (this._successScreen == null)
		{
			Debug.LogError("[KID::AGE_APPEAL_EMAIL] _successScreen has not been set yet and is NULL. Cannot inform of result");
			return;
		}
		this._hasCompletedSendEmailRequest = true;
		if (success)
		{
			base.gameObject.SetActive(false);
			this._successScreen.ShowSuccessScreenAppeal(this._emailText.text);
			return;
		}
	}

	// Token: 0x06004BD4 RID: 19412 RVA: 0x001942BB File Offset: 0x001924BB
	private void ShowErrorScreen()
	{
		Debug.LogErrorFormat("[KID::UI::Setup] K-ID Confirmation Failed - Failed to send email", Array.Empty<object>());
		base.gameObject.SetActive(false);
		this._errorScreen.ShowAgeAppealEmailErrorScreen(this.hasChallenge, this.newAgeToAppeal, this._emailText.text);
	}

	// Token: 0x04005EA0 RID: 24224
	[SerializeField]
	private TMP_Text _confirmText;

	// Token: 0x04005EA1 RID: 24225
	[SerializeField]
	private TMP_Text _emailText;

	// Token: 0x04005EA2 RID: 24226
	private string CONFIRM_PARENT_EMAIL = "Please confirm your parent or guardian's email address.";

	// Token: 0x04005EA3 RID: 24227
	private string CONFIRM_YOUR_EMAIL = "Please confirm your email address.";

	// Token: 0x04005EA4 RID: 24228
	private bool hasChallenge = true;

	// Token: 0x04005EA5 RID: 24229
	private int newAgeToAppeal;

	// Token: 0x04005EA6 RID: 24230
	private bool _hasCompletedSendEmailRequest;

	// Token: 0x04005EA7 RID: 24231
	[SerializeField]
	private KIDUI_EmailSuccess _successScreen;

	// Token: 0x04005EA8 RID: 24232
	[SerializeField]
	private KIDUI_AgeAppealEmailError _errorScreen;

	// Token: 0x04005EA9 RID: 24233
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x04005EAA RID: 24234
	[SerializeField]
	private int _minimumDelay = 1000;
}
