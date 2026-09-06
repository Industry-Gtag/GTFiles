using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

// Token: 0x02000BB8 RID: 3000
public class KIDUI_AgeAppealEmailScreen : MonoBehaviour
{
	// Token: 0x06004BC7 RID: 19399 RVA: 0x00193E2C File Offset: 0x0019202C
	public void ShowAgeAppealEmailScreen(bool receivedChallenge, int newAge)
	{
		this.newAgeToAppeal = newAge;
		base.gameObject.SetActive(true);
		this.hasChallenge = receivedChallenge;
		this._enterEmailText.text = (this.hasChallenge ? this.PARENT_EMAIL_DESCRIPTION : this.VERIFY_AGE_EMAIL_DESCRIPTION);
		if (this._parentPermissionNotice)
		{
			this._parentPermissionNotice.SetActive(this.hasChallenge);
		}
		this.OnInputChanged(this._emailText.text);
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_age_appeal_enter_email",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { 
			{
				"email_type",
				this.hasChallenge ? "under_dac" : "over_dac"
			} }
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
	}

	// Token: 0x06004BC8 RID: 19400 RVA: 0x00193F20 File Offset: 0x00192120
	public void OnInputChanged(string newVal)
	{
		bool flag = !string.IsNullOrEmpty(newVal);
		if (flag)
		{
			flag = Regex.IsMatch(newVal, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
		}
		this._confirmButton.interactable = flag;
	}

	// Token: 0x06004BC9 RID: 19401 RVA: 0x00193F54 File Offset: 0x00192154
	public void OnConfirmPressed()
	{
		if (string.IsNullOrEmpty(this._emailText.text))
		{
			Debug.LogError("[KID::UI::APPEAL_AGE_EMAIL] Age Appeal Email Text is empty");
			return;
		}
		this._confirmationScreen.ShowAgeAppealConfirmationScreen(this.hasChallenge, this.newAgeToAppeal, this._emailText.text);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06004BCA RID: 19402 RVA: 0x0018FAF0 File Offset: 0x0018DCF0
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005E97 RID: 24215
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x04005E98 RID: 24216
	[SerializeField]
	private KIDUI_AgeAppealEmailConfirmation _confirmationScreen;

	// Token: 0x04005E99 RID: 24217
	[SerializeField]
	private TMP_Text _enterEmailText;

	// Token: 0x04005E9A RID: 24218
	[SerializeField]
	private TMP_InputField _emailText;

	// Token: 0x04005E9B RID: 24219
	[SerializeField]
	private GameObject _parentPermissionNotice;

	// Token: 0x04005E9C RID: 24220
	private string PARENT_EMAIL_DESCRIPTION = "Enter your parent or guardian's email address below.";

	// Token: 0x04005E9D RID: 24221
	private string VERIFY_AGE_EMAIL_DESCRIPTION = "Enter your email address below";

	// Token: 0x04005E9E RID: 24222
	private bool hasChallenge = true;

	// Token: 0x04005E9F RID: 24223
	private int newAgeToAppeal;
}
