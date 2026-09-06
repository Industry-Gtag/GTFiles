using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000BA2 RID: 2978
public class KIDMessagingController : MonoBehaviour
{
	// Token: 0x1700073B RID: 1851
	// (get) Token: 0x06004B32 RID: 19250 RVA: 0x00191977 File Offset: 0x0018FB77
	private static string HasShownConfirmationScreenPlayerPref
	{
		get
		{
			return "hasShownKIDConfirmationScreen-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x06004B33 RID: 19251 RVA: 0x0019198F File Offset: 0x0018FB8F
	public void OnConfirmPressed()
	{
		this._closeMessageBox = true;
	}

	// Token: 0x06004B34 RID: 19252 RVA: 0x00191998 File Offset: 0x0018FB98
	private void Awake()
	{
		if (KIDMessagingController.instance != null)
		{
			Debug.LogError("[KID::MESSAGING_CONTROLLER] Trying to start a new [KIDMessagingController] but one already exists");
			Object.Destroy(this);
			return;
		}
		KIDMessagingController.instance = this;
	}

	// Token: 0x06004B35 RID: 19253 RVA: 0x001919BE File Offset: 0x0018FBBE
	private bool ShouldShowConfirmationScreen()
	{
		return !KIDManager.CurrentSession.IsDefault;
	}

	// Token: 0x06004B36 RID: 19254 RVA: 0x001919D0 File Offset: 0x0018FBD0
	private async Task StartKIDConfirmationScreenInternal(CancellationToken token)
	{
		if (this.messageBox == null)
		{
			Debug.LogError("[KID::MESSAGING_CONTROLLER] Trying to show confirmation screen but [messageBox] is null");
		}
		else
		{
			string text = await KIDMessagingController.GetSetupConfirmationMessage();
			if (string.IsNullOrEmpty(text))
			{
				text = "k-ID setup is now complete. Thanks and have fun in Gorilla World!";
			}
			string text2;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("KID_SETUP_CONFIRMATION_TITLE", out text2, "Thank you"))
			{
				Debug.LogError("[LOCALIZATION::KID_MESSAGING_CONTROLLER] Failed to get key for k-ID localization [KID_SETUP_CONFIRMATION_TITLE]");
			}
			this.messageBox.Header = text2;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("KID_SETUP_CONFIRMATION_BODY", out text2, text))
			{
				Debug.LogError("[LOCALIZATION::KID_MESSAGING_CONTROLLER] Failed to get key for k-ID localization [KID_SETUP_CONFIRMATION_BODY]");
			}
			this.messageBox.Body = text2;
			this.messageBox.LeftButton = string.Empty;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("KID_SETUP_CONFIRMATION_BUTTON", out text2, "Continue"))
			{
				Debug.LogError("[LOCALIZATION::KID_MESSAGING_CONTROLLER] Failed to get key for k-ID localization [KID_SETUP_CONFIRMATION_BUTTON]");
			}
			this.messageBox.RightButton = text2;
			this.messageBox.gameObject.SetActive(true);
			HandRayController.Instance.EnableHandRays();
			PrivateUIRoom.AddUI(base.transform);
			while (!token.IsCancellationRequested)
			{
				await Task.Yield();
				if (this._closeMessageBox)
				{
					PrivateUIRoom.RemoveUI(base.transform);
					HandRayController.Instance.DisableHandRays();
					this.messageBox.gameObject.SetActive(false);
					await KIDManager.TrySetHasConfirmedStatus();
					break;
				}
			}
		}
	}

	// Token: 0x06004B37 RID: 19255 RVA: 0x0018FAF0 File Offset: 0x0018DCF0
	public void OnDisable()
	{
		KIDAudioManager kidaudioManager = KIDAudioManager.Instance;
		if (kidaudioManager == null)
		{
			return;
		}
		kidaudioManager.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x06004B38 RID: 19256 RVA: 0x00191A1C File Offset: 0x0018FC1C
	public static async Task StartKIDConfirmationScreen(CancellationToken token)
	{
		KIDMessagingController kidmessagingController = KIDMessagingController.instance;
		if (kidmessagingController == null || kidmessagingController.ShouldShowConfirmationScreen())
		{
			await KIDMessagingController.instance.StartKIDConfirmationScreenInternal(token);
			TelemetryData telemetryData = new TelemetryData
			{
				EventName = "kid_screen_shown",
				CustomTags = new string[]
				{
					"kid_setup",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment
				},
				BodyData = new Dictionary<string, string>
				{
					{ "screen", "setup_complete" },
					{
						"saw_game_settings",
						KIDUI_MainScreen.ShownSettingsScreen.ToString().ToLower() ?? ""
					}
				}
			};
			GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		}
	}

	// Token: 0x06004B39 RID: 19257 RVA: 0x00191A60 File Offset: 0x0018FC60
	private static async Task<string> GetSetupConfirmationMessage()
	{
		int state = 0;
		string bodyText = string.Empty;
		PlayFabTitleDataCache.Instance.GetTitleData("KIDData", delegate(string res)
		{
			state = 1;
			bodyText = KIDMessagingController.GetConfirmMessageFromTitleDataJson(res);
		}, delegate(PlayFabError err)
		{
			state = -1;
			Debug.LogError("[KID_MANAGER] Something went wrong trying to get title data for key: [KIDData]. Error:\n" + err.ErrorMessage);
		}, false);
		do
		{
			await Task.Yield();
		}
		while (state == 0);
		return bodyText;
	}

	// Token: 0x06004B3A RID: 19258 RVA: 0x00191A9C File Offset: 0x0018FC9C
	private static string GetConfirmMessageFromTitleDataJson(string jsonTxt)
	{
		if (string.IsNullOrEmpty(jsonTxt))
		{
			Debug.LogError("[KID_MANAGER] Cannot get Confirmation Message. JSON is null or empty!");
			return null;
		}
		KIDMessagingTitleData kidmessagingTitleData = JsonConvert.DeserializeObject<KIDMessagingTitleData>(jsonTxt);
		if (kidmessagingTitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDMessagingTitleData]. Json: \n" + jsonTxt);
			return null;
		}
		if (string.IsNullOrEmpty(kidmessagingTitleData.KIDSetupConfirmation))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDMessagingTitleData] - [KIDSetupConfirmation] is null or empty. Json: \n" + jsonTxt);
			return null;
		}
		return kidmessagingTitleData.KIDSetupConfirmation;
	}

	// Token: 0x06004B3B RID: 19259 RVA: 0x00191B00 File Offset: 0x0018FD00
	public static void ShowConnectionErrorScreen()
	{
		if (KIDMessagingController.instance == null || KIDMessagingController.instance.messageBox == null)
		{
			Debug.LogError("[KID::MESSAGING_CONTROLLER] No message box");
			return;
		}
		KIDMessagingController.instance._closeMessageBox = false;
		KIDMessagingController.instance.messageBox.Header = "Connection Error";
		KIDMessagingController.instance.messageBox.Body = "Unable to connect to the internet. Please restart the game and try again.";
		KIDMessagingController.instance.messageBox.RightButton = "Quit";
		KIDMessagingController.instance.messageBox.ShowQuitButtonAsPrimary();
		KIDMessagingController.instance.messageBox.RightButtonCallback.RemoveAllListeners();
		KIDMessagingController.instance.messageBox.RightButtonCallback.AddListener(new UnityAction(Application.Quit));
		KIDMessagingController.instance.messageBox.gameObject.SetActive(true);
		HandRayController.Instance.EnableHandRays();
		PrivateUIRoom.AddUI(KIDMessagingController.instance.transform);
	}

	// Token: 0x04005E0C RID: 24076
	private const string SHOWN_CONFIRMATION_SCREEN_PREFIX = "hasShownKIDConfirmationScreen-";

	// Token: 0x04005E0D RID: 24077
	private const string CONFIRMATION_HEADER = "Thank you";

	// Token: 0x04005E0E RID: 24078
	private const string CONFIRMATION_BODY = "k-ID setup is now complete. Thanks and have fun in Gorilla World!";

	// Token: 0x04005E0F RID: 24079
	private const string CONFIRMATION_BUTTON = "Continue";

	// Token: 0x04005E10 RID: 24080
	private const string KID_SETUP_CONFIRMATION_TITLE_KEY = "KID_SETUP_CONFIRMATION_TITLE";

	// Token: 0x04005E11 RID: 24081
	private const string KID_SETUP_CONFIRMATION_BODY_KEY = "KID_SETUP_CONFIRMATION_BODY";

	// Token: 0x04005E12 RID: 24082
	private const string KID_SETUP_CONFIRMATION_BUTTON_KEY = "KID_SETUP_CONFIRMATION_BUTTON";

	// Token: 0x04005E13 RID: 24083
	private static KIDMessagingController instance;

	// Token: 0x04005E14 RID: 24084
	[SerializeField]
	private MessageBox messageBox;

	// Token: 0x04005E15 RID: 24085
	private const string CONNECTION_ERROR_HEADER = "Connection Error";

	// Token: 0x04005E16 RID: 24086
	private const string CONNECTION_ERROR_BODY = "Unable to connect to the internet. Please restart the game and try again.";

	// Token: 0x04005E17 RID: 24087
	private const string CONNECTION_ERROR_BUTTON = "Quit";

	// Token: 0x04005E18 RID: 24088
	private bool _closeMessageBox;
}
