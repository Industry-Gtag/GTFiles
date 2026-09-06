using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

// Token: 0x02000BDD RID: 3037
public class KIDUI_SetupScreen : MonoBehaviour
{
	// Token: 0x06004C81 RID: 19585 RVA: 0x00198160 File Offset: 0x00196360
	private void Awake()
	{
		if (this._emailInputField == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Email Input Field is NULL", Array.Empty<object>());
			return;
		}
		if (this._confirmScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Confirm Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._mainScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Main Screen is NULL", Array.Empty<object>());
			return;
		}
	}

	// Token: 0x06004C82 RID: 19586 RVA: 0x001981C8 File Offset: 0x001963C8
	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString(KIDManager.GetEmailForUserPlayerPrefRef, "");
		this._emailInputField.text = @string;
		this._confirmButton.ResetButton();
		this.OnInputChanged(@string);
	}

	// Token: 0x06004C83 RID: 19587 RVA: 0x00198203 File Offset: 0x00196403
	private void OnDisable()
	{
		if (this._keyboard == null)
		{
			return;
		}
		this._keyboard.active = false;
	}

	// Token: 0x06004C84 RID: 19588 RVA: 0x0019821C File Offset: 0x0019641C
	public void OnStartSetup()
	{
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
			BodyData = new Dictionary<string, string> { { "screen", "enter_email" } }
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
	}

	// Token: 0x06004C85 RID: 19589 RVA: 0x001982A3 File Offset: 0x001964A3
	public void OnInputSelected()
	{
		Debug.LogFormat("[KID::UI::SETUP] Email Input Selected!", Array.Empty<object>());
	}

	// Token: 0x06004C86 RID: 19590 RVA: 0x001982B4 File Offset: 0x001964B4
	public void OnInputChanged(string newVal)
	{
		bool flag = !string.IsNullOrEmpty(newVal);
		if (flag)
		{
			flag = Regex.IsMatch(newVal, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
		}
		this._confirmButton.interactable = flag;
	}

	// Token: 0x06004C87 RID: 19591 RVA: 0x001982E6 File Offset: 0x001964E6
	public void OnSubmitEmailPressed()
	{
		PlayerPrefs.SetString(KIDManager.GetEmailForUserPlayerPrefRef, this._emailInputField.text);
		PlayerPrefs.Save();
		base.gameObject.SetActive(false);
		this._confirmScreen.OnEmailSubmitted(this._emailInputField.text);
	}

	// Token: 0x06004C88 RID: 19592 RVA: 0x00198324 File Offset: 0x00196524
	public void OnBackPressed()
	{
		PlayerPrefs.SetString(KIDManager.GetEmailForUserPlayerPrefRef, this._emailInputField.text);
		PlayerPrefs.Save();
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Previous);
	}

	// Token: 0x04005F91 RID: 24465
	[SerializeField]
	private TMP_InputField _emailInputField;

	// Token: 0x04005F92 RID: 24466
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x04005F93 RID: 24467
	[SerializeField]
	private KIDUI_ConfirmScreen _confirmScreen;

	// Token: 0x04005F94 RID: 24468
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x04005F95 RID: 24469
	[SerializeField]
	private TMP_Text _riftKeyboardMessage;

	// Token: 0x04005F96 RID: 24470
	private string _emailStr = string.Empty;

	// Token: 0x04005F97 RID: 24471
	private TouchScreenKeyboard _keyboard;
}
