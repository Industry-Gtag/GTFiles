using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KID.Model;
using TMPro;
using UnityEngine;

// Token: 0x02000B53 RID: 2899
public class KIDAgeGate : MonoBehaviour
{
	// Token: 0x17000710 RID: 1808
	// (get) Token: 0x060049BB RID: 18875 RVA: 0x00189166 File Offset: 0x00187366
	public static int UserAge
	{
		get
		{
			return KIDAgeGate._ageValue;
		}
	}

	// Token: 0x17000711 RID: 1809
	// (get) Token: 0x060049BC RID: 18876 RVA: 0x0018916D File Offset: 0x0018736D
	// (set) Token: 0x060049BD RID: 18877 RVA: 0x00189174 File Offset: 0x00187374
	public static bool DisplayedScreen { get; private set; }

	// Token: 0x060049BE RID: 18878 RVA: 0x0018917C File Offset: 0x0018737C
	private void Awake()
	{
		if (KIDAgeGate._activeReference != null)
		{
			Debug.LogError("[KID::Age_Gate] Age Gate already exists, this is a duplicate, deleting the new one");
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		KIDAgeGate._activeReference = this;
	}

	// Token: 0x060049BF RID: 18879 RVA: 0x001891A8 File Offset: 0x001873A8
	private async void Start()
	{
	}

	// Token: 0x060049C0 RID: 18880 RVA: 0x001891D7 File Offset: 0x001873D7
	private void OnDestroy()
	{
		this.requestCancellationSource.Cancel();
	}

	// Token: 0x060049C1 RID: 18881 RVA: 0x001891E4 File Offset: 0x001873E4
	public static async Task BeginAgeGate()
	{
		if (KIDAgeGate._activeReference == null)
		{
			Debug.LogError("[KID::Age_Gate] Unable to start Age Gate. No active reference assigned. Has it initialised yet?");
			do
			{
				await Task.Yield();
			}
			while (KIDAgeGate._activeReference == null);
		}
		await KIDAgeGate._activeReference.StartAgeGate();
	}

	// Token: 0x060049C2 RID: 18882 RVA: 0x00189220 File Offset: 0x00187420
	private async Task StartAgeGate()
	{
		await this.InitialiseAgeGate();
	}

	// Token: 0x060049C3 RID: 18883 RVA: 0x00189264 File Offset: 0x00187464
	private async Task InitialiseAgeGate()
	{
		Debug.Log("[KID] Initialising Age-Gate");
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				KIDTelemetry.Open_MetricActionCustomTag,
				"kid_age_gate",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { { "screen", "age_gate" } }
		};
		TelemetryData telemetryData2 = telemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
		for (;;)
		{
			KIDAgeGate.DisplayedScreen = true;
			this._ageSlider.ControllerActive = true;
			PrivateUIRoom.AddUI(this._uiParent.transform);
			HandRayController.Instance.EnableHandRays();
			await this.ProcessAgeGate();
			this._ageSlider.ControllerActive = false;
			KIDAudioManager instance = KIDAudioManager.Instance;
			if (instance != null)
			{
				instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
			}
			PrivateUIRoom.RemoveUI(this._uiParent.transform);
			if (this.requestCancellationSource.IsCancellationRequested)
			{
				break;
			}
			AgeStatusType ageStatusType;
			if (KIDManager.TryGetAgeStatusTypeFromAge(KIDAgeGate.UserAge, out ageStatusType))
			{
				telemetryData = new TelemetryData
				{
					EventName = "kid_age_gate",
					CustomTags = new string[]
					{
						KIDTelemetry.Closed_MetricActionCustomTag,
						"kid_age_gate",
						KIDTelemetry.GameVersionCustomTag,
						KIDTelemetry.GameEnvironment
					},
					BodyData = new Dictionary<string, string> { 
					{
						"age_declared",
						ageStatusType.ToString()
					} }
				};
				telemetryData2 = telemetryData;
				GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
			}
			this._confirmationUIManager.Reset(KIDAgeGate._ageValue);
			PrivateUIRoom.AddUI(this._confirmationUI.transform);
			bool flag = await this.ProcessAgeGateConfirmation();
			telemetryData = new TelemetryData
			{
				EventName = "kid_age_gate_confirm",
				CustomTags = new string[]
				{
					"kid_age_gate",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment
				},
				BodyData = new Dictionary<string, string> { 
				{
					"button_pressed",
					flag ? "confirm" : "go_back"
				} }
			};
			telemetryData2 = telemetryData;
			GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
			KIDAudioManager instance2 = KIDAudioManager.Instance;
			if (instance2 != null)
			{
				instance2.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
			}
			PrivateUIRoom.RemoveUI(this._confirmationUI.transform);
			HandRayController.Instance.DisableHandRays();
			if (flag)
			{
				goto Block_6;
			}
		}
		return;
		Block_6:
		this.OnAgeGateCompleted();
		Debug.Log("[KID] Age Gate Complete");
	}

	// Token: 0x060049C4 RID: 18884 RVA: 0x001892A8 File Offset: 0x001874A8
	private async Task ProcessAgeGate()
	{
		Debug.Log("[KID] Waiting for Age Confirmation");
		await this.WaitForAgeChoice();
	}

	// Token: 0x060049C5 RID: 18885 RVA: 0x001892EC File Offset: 0x001874EC
	private async Task<bool> ProcessAgeGateConfirmation()
	{
		while (this._confirmationUIManager.Result == KidAgeConfirmationResult.None)
		{
			if (this.requestCancellationSource.IsCancellationRequested)
			{
				return false;
			}
			await Task.Yield();
		}
		return this._confirmationUIManager.Result == KidAgeConfirmationResult.Confirm;
	}

	// Token: 0x060049C6 RID: 18886 RVA: 0x00189330 File Offset: 0x00187530
	private async Task WaitForAgeChoice()
	{
		KIDAgeGate._hasChosenAge = false;
		while (!this.requestCancellationSource.IsCancellationRequested)
		{
			await Task.Yield();
			if (KIDAgeGate._hasChosenAge)
			{
				KIDAgeGate._ageValue = this._ageSlider.CurrentAge;
				string ageString = this._ageSlider.GetAgeString();
				this._confirmationAgeText.text = "You entered " + ageString + "\n\nPlease be sure to enter your real age so we can customize your experience!";
				return;
			}
		}
	}

	// Token: 0x060049C7 RID: 18887 RVA: 0x00189373 File Offset: 0x00187573
	public static void OnConfirmAgePressed(int currentAge)
	{
		KIDAgeGate._hasChosenAge = true;
	}

	// Token: 0x060049C8 RID: 18888 RVA: 0x0018937B File Offset: 0x0018757B
	private void OnAgeGateCompleted()
	{
		this.FinaliseAgeGateAndContinue();
	}

	// Token: 0x060049C9 RID: 18889 RVA: 0x00189383 File Offset: 0x00187583
	private void FinaliseAgeGateAndContinue()
	{
		if (this.requestCancellationSource.IsCancellationRequested)
		{
			return;
		}
		Debug.Log("[KID::AGE_GATE] Age gate completed");
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060049CA RID: 18890 RVA: 0x001893A8 File Offset: 0x001875A8
	private void QuitGame()
	{
		Debug.Log("[KID] QUIT PRESSED");
		Application.Quit();
	}

	// Token: 0x060049CB RID: 18891 RVA: 0x001893BC File Offset: 0x001875BC
	private async void AppealAge()
	{
		Debug.Log("[KID] APPEAL PRESSED");
		if (!KIDManager.InitialisationComplete)
		{
			Debug.LogError("[KID] [KIDManager] has not been Initialised yet. Unable to start appeals flow. Will wait until ready");
			do
			{
				await Task.Yield();
			}
			while (!KIDManager.InitialisationComplete);
		}
		if (KIDManager.InitialisationSuccessful)
		{
			string text = "VERIFY AGE";
			string text2 = "GETTING ONE TIME PASSCODE. PLEASE WAIT.\n\nGIVE IT TO A PARENT/GUARDIAN TO ENTER IT AT: k-id.com/code";
			string empty = string.Empty;
			this._pregameMessageReference.ShowMessage(text, text2, empty, new Action(this.RefreshChallengeStatus), 0.25f, 0f);
		}
		Debug.LogError("[KID::AGE_GATE] TODO: Refactor Age-Appeal flow");
	}

	// Token: 0x060049CC RID: 18892 RVA: 0x001893F4 File Offset: 0x001875F4
	private void AppealRejected()
	{
		Debug.Log("[KID] APPEAL REJECTED");
		string text = "UNDER AGE";
		string text2 = "Your VR platform requires a certain minimum age to play Gorilla Tag. Unfortunately, due to those age requirements, we cannot allow you to play Gorilla Tag at this time.\n\nIf you incorrectly submitted your age, please appeal.";
		string text3 = "Hold any face button to appeal";
		this._pregameMessageReference.ShowMessage(text, text2, text3, new Action(this.AppealAge), 0.25f, 0f);
	}

	// Token: 0x060049CD RID: 18893 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void RefreshChallengeStatus()
	{
	}

	// Token: 0x060049CE RID: 18894 RVA: 0x00189441 File Offset: 0x00187641
	public static void SetAgeGateConfig(GetRequirementsData response)
	{
		KIDAgeGate._ageGateConfig = response;
	}

	// Token: 0x060049CF RID: 18895 RVA: 0x0018944C File Offset: 0x0018764C
	public void OnWhyAgeGateButtonPressed()
	{
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_gate",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { { "screen", "why_age_gate" } }
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		this._uiParent.SetActive(false);
		PrivateUIRoom.AddUI(this._whyAgeGateScreen.transform);
		this._whyAgeGateScreen.SetActive(true);
	}

	// Token: 0x060049D0 RID: 18896 RVA: 0x001894EF File Offset: 0x001876EF
	public void OnWhyAgeGateButtonBackPressed()
	{
		this._uiParent.SetActive(true);
		PrivateUIRoom.RemoveUI(this._whyAgeGateScreen.transform);
		this._whyAgeGateScreen.SetActive(false);
	}

	// Token: 0x060049D1 RID: 18897 RVA: 0x0018951C File Offset: 0x0018771C
	public void OnLearnMoreAboutKIDPressed()
	{
		this._metrics_LearnMorePressed = true;
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_gate",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { { "screen", "learn_more_url" } }
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		Application.OpenURL("https://whyagegate.com/");
	}

	// Token: 0x04005C24 RID: 23588
	private const string LEARN_MORE_URL = "https://whyagegate.com/";

	// Token: 0x04005C25 RID: 23589
	private const string DEFAULT_AGE_VALUE_STRING = "SET AGE";

	// Token: 0x04005C26 RID: 23590
	private const int MINIMUM_PLATFORM_AGE = 13;

	// Token: 0x04005C27 RID: 23591
	[Header("Age Gate Settings")]
	[SerializeField]
	private PreGameMessage _pregameMessageReference;

	// Token: 0x04005C28 RID: 23592
	[SerializeField]
	private KIDUI_AgeDiscrepancyScreen _ageDiscrepancyScreen;

	// Token: 0x04005C29 RID: 23593
	[SerializeField]
	private GameObject _uiParent;

	// Token: 0x04005C2A RID: 23594
	[SerializeField]
	private AgeSliderWithProgressBar _ageSlider;

	// Token: 0x04005C2B RID: 23595
	[SerializeField]
	private GameObject _confirmationUI;

	// Token: 0x04005C2C RID: 23596
	[SerializeField]
	private KIDAgeGateConfirmation _confirmationUIManager;

	// Token: 0x04005C2D RID: 23597
	[SerializeField]
	private TMP_Text _confirmationAgeText;

	// Token: 0x04005C2E RID: 23598
	[SerializeField]
	private GameObject _whyAgeGateScreen;

	// Token: 0x04005C2F RID: 23599
	private const string strBlockAccessTitle = "UNDER AGE";

	// Token: 0x04005C30 RID: 23600
	private const string strBlockAccessMessage = "Your VR platform requires a certain minimum age to play Gorilla Tag. Unfortunately, due to those age requirements, we cannot allow you to play Gorilla Tag at this time.\n\nIf you incorrectly submitted your age, please appeal.";

	// Token: 0x04005C31 RID: 23601
	private const string strBlockAccessConfirm = "Hold any face button to appeal";

	// Token: 0x04005C32 RID: 23602
	private const string strVerifyAgeTitle = "VERIFY AGE";

	// Token: 0x04005C33 RID: 23603
	private const string strVerifyAgeMessage = "GETTING ONE TIME PASSCODE. PLEASE WAIT.\n\nGIVE IT TO A PARENT/GUARDIAN TO ENTER IT AT: k-id.com/code";

	// Token: 0x04005C34 RID: 23604
	private const string strDiscrepancyMessage = "You entered {0} for your age,\nbut your Meta account says you should be {1}. You could be logged into the wrong Meta account on this device.\n\nWe will use the lowest age ({2})\nif you Continue.";

	// Token: 0x04005C35 RID: 23605
	private static KIDAgeGate _activeReference;

	// Token: 0x04005C36 RID: 23606
	private static GetRequirementsData _ageGateConfig;

	// Token: 0x04005C37 RID: 23607
	private static int _ageValue;

	// Token: 0x04005C38 RID: 23608
	private CancellationTokenSource requestCancellationSource = new CancellationTokenSource();

	// Token: 0x04005C39 RID: 23609
	private static bool _hasChosenAge;

	// Token: 0x04005C3B RID: 23611
	private bool _metrics_LearnMorePressed;
}
