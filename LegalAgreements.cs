using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GorillaNetworking;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.CloudScriptModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000BF1 RID: 3057
[DefaultExecutionOrder(1)]
public class LegalAgreements : MonoBehaviour
{
	// Token: 0x1700075A RID: 1882
	// (get) Token: 0x06004CCA RID: 19658 RVA: 0x00199792 File Offset: 0x00197992
	// (set) Token: 0x06004CCB RID: 19659 RVA: 0x00199799 File Offset: 0x00197999
	public static LegalAgreements instance { get; private set; }

	// Token: 0x06004CCC RID: 19660 RVA: 0x001997A4 File Offset: 0x001979A4
	protected virtual void Awake()
	{
		if (LegalAgreements.instance != null)
		{
			Debug.LogError("Trying to set [LegalAgreements] instance but it is not null", this);
			base.gameObject.SetActive(false);
			return;
		}
		LegalAgreements.instance = this;
		this.stickHeldDuration = 0f;
		this.scrollSpeed = this._minScrollSpeed;
		base.enabled = false;
	}

	// Token: 0x06004CCD RID: 19661 RVA: 0x001997FC File Offset: 0x001979FC
	private void Update()
	{
		if (!this.legalAgreementsStarted)
		{
			return;
		}
		float num = Time.deltaTime * this.scrollSpeed;
		if (ControllerBehaviour.Instance.IsUpStick || ControllerBehaviour.Instance.IsDownStick)
		{
			if (ControllerBehaviour.Instance.IsDownStick)
			{
				num *= -1f;
			}
			this.scrollBar.value = Mathf.Clamp(this.scrollBar.value + num, 0f, 1f);
			if (this.scrollBar.value > 0f && this.scrollBar.value < 1f)
			{
				HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
			}
			this.stickHeldDuration += Time.deltaTime;
			this.scrollTime = Mathf.Clamp01(this.stickHeldDuration / this._scrollInterpTime);
			this.scrollSpeed = Mathf.Lerp(this._minScrollSpeed, this._maxScrollSpeed, this._scrollInterpCurve.Evaluate(this.scrollTime));
			this.scrollSpeed *= Mathf.Abs(ControllerBehaviour.Instance.StickYValue);
		}
		else
		{
			this.stickHeldDuration = 0f;
			this.scrollSpeed = this._minScrollSpeed;
		}
		if (this._scrollToBottomText)
		{
			if ((double)this.scrollBar.value < 0.001)
			{
				this._scrollToBottomText.gameObject.SetActive(false);
				this._pressAndHoldToConfirmButton.gameObject.SetActive(true);
				return;
			}
			this._scrollToBottomText.text = LegalAgreements.SCROLL_TO_END_MESSAGE;
			this._scrollToBottomText.gameObject.SetActive(true);
			this._pressAndHoldToConfirmButton.gameObject.SetActive(false);
		}
	}

	// Token: 0x06004CCE RID: 19662 RVA: 0x001999B0 File Offset: 0x00197BB0
	public virtual async Task StartLegalAgreements()
	{
		if (!this.legalAgreementsStarted)
		{
			this.legalAgreementsStarted = true;
			while (!PlayFabClientAPI.IsClientLoggedIn())
			{
				if (PlayFabAuthenticator.instance && PlayFabAuthenticator.instance.loginFailed)
				{
					return;
				}
				await Task.Yield();
			}
			Dictionary<string, string> agreementResults = await this.GetAcceptedAgreements(this.legalAgreementScreens);
			foreach (LegalAgreementTextAsset screen in this.legalAgreementScreens)
			{
				string latestVersion = await this.GetTitleDataAsync(screen.latestVersionKey);
				if (!string.IsNullOrEmpty(latestVersion))
				{
					string empty = string.Empty;
					if (agreementResults == null || !agreementResults.TryGetValue(screen.playFabKey, out empty) || !(latestVersion == empty))
					{
						base.enabled = true;
						PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.KID, "");
						if (!screen.confirmString.IsNullOrEmpty())
						{
							this._pressAndHoldToConfirmButton.SetText(screen.confirmString);
						}
						PrivateUIRoom.AddUI(this.uiParent);
						HandRayController.Instance.EnableHandRays();
						TaskAwaiter<bool> taskAwaiter = this.UpdateText(screen, latestVersion).GetAwaiter();
						if (!taskAwaiter.IsCompleted)
						{
							await taskAwaiter;
							TaskAwaiter<bool> taskAwaiter2;
							taskAwaiter = taskAwaiter2;
							taskAwaiter2 = default(TaskAwaiter<bool>);
						}
						if (!taskAwaiter.GetResult())
						{
							for (;;)
							{
								await Task.Yield();
							}
						}
						else
						{
							await this.WaitForAcknowledgement();
							this.scrollBar.value = 1f;
							PrivateUIRoom.RemoveUI(this.uiParent);
							if (agreementResults == null)
							{
								agreementResults = new Dictionary<string, string>();
							}
							agreementResults.AddOrUpdate(screen.playFabKey, latestVersion);
							if (this.optIn)
							{
								LegalAgreementTextAsset.PostAcceptAction optInAction = screen.optInAction;
							}
							latestVersion = null;
							screen = null;
						}
					}
				}
			}
			LegalAgreementTextAsset[] array = null;
			base.enabled = false;
			await this.SubmitAcceptedAgreements(agreementResults);
		}
	}

	// Token: 0x06004CCF RID: 19663 RVA: 0x001999F3 File Offset: 0x00197BF3
	public void OnAccepted(int currentAge)
	{
		this._accepted = true;
	}

	// Token: 0x06004CD0 RID: 19664 RVA: 0x001999FC File Offset: 0x00197BFC
	protected async Task WaitForAcknowledgement()
	{
		this._accepted = false;
		while (!this._accepted)
		{
			await Task.Yield();
		}
		this._accepted = false;
	}

	// Token: 0x06004CD1 RID: 19665 RVA: 0x00199A40 File Offset: 0x00197C40
	private async Task<bool> UpdateText(LegalAgreementTextAsset asset, string version)
	{
		this.optional = asset.optional;
		this.tmpTitle.text = asset.title;
		bool flag = await this.UpdateTextFromPlayFabTitleData(asset.playFabKey, version, this.tmpBody);
		if (!flag)
		{
			this.tmpBody.text = asset.errorMessage + "\n\nPlease restart the game and try again.";
			this.scrollBar.value = 0f;
			this.scrollBar.size = 1f;
		}
		return flag;
	}

	// Token: 0x06004CD2 RID: 19666 RVA: 0x00199A94 File Offset: 0x00197C94
	public async Task<bool> UpdateTextFromPlayFabTitleData(string key, string version, TMP_Text target)
	{
		string text = key + "_" + version;
		this.state = 0;
		PlayFabTitleDataCache.Instance.GetTitleData(text, new Action<string>(this.OnTitleDataReceived), new Action<PlayFabError>(this.OnPlayFabError), false);
		while (this.state == 0)
		{
			await Task.Yield();
		}
		bool flag;
		if (this.state == 1)
		{
			string text2 = Regex.Unescape(this.cachedText);
			try
			{
				if (string.IsNullOrEmpty(text2))
				{
					Debug.LogError("[LOCALIZATION] TItle Data for Legal Agreements is NULL or Empty. Unable to deserialize or proceed.");
					return false;
				}
				text2 = JsonConvert.DeserializeObject<TitleDataLocalization>(text2).GetLocalizedText();
			}
			catch (Exception)
			{
				if (text2.StartsWith('{') && text2.EndsWith('}'))
				{
					Debug.LogError("[LOCALIZATION] TItle Data for Legal Agreements is likely in JSON format, but failed to deserialize into [TitleDataLocalization]");
					return false;
				}
			}
			target.text = text2;
			flag = true;
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	// Token: 0x06004CD3 RID: 19667 RVA: 0x00199AEF File Offset: 0x00197CEF
	private void OnPlayFabError(PlayFabError error)
	{
		this.state = -1;
	}

	// Token: 0x06004CD4 RID: 19668 RVA: 0x00199AF8 File Offset: 0x00197CF8
	private void OnTitleDataReceived(string obj)
	{
		this.cachedText = obj;
		this.state = 1;
	}

	// Token: 0x06004CD5 RID: 19669 RVA: 0x00199B08 File Offset: 0x00197D08
	private async Task<string> GetTitleDataAsync(string key)
	{
		int state = 0;
		string result = null;
		PlayFabTitleDataCache.Instance.GetTitleData(key, delegate(string res)
		{
			result = res;
			state = 1;
		}, delegate(PlayFabError err)
		{
			result = null;
			state = -1;
			Debug.LogError("[GT/LegalAgreements]  ERROR!!!  GetTitleDataAsync: Encountered error while getting title data: " + err.ErrorMessage);
		}, false);
		while (state == 0)
		{
			await Task.Yield();
		}
		return (state == 1) ? result : null;
	}

	// Token: 0x06004CD6 RID: 19670 RVA: 0x00199B4C File Offset: 0x00197D4C
	private async Task<Dictionary<string, string>> GetAcceptedAgreements(LegalAgreementTextAsset[] agreements)
	{
		int state = 0;
		Dictionary<string, string> returnValue = new Dictionary<string, string>();
		string[] array = agreements.Select((LegalAgreementTextAsset x) => x.playFabKey).ToArray<string>();
		GorillaServer.Instance.GetAcceptedAgreements(new GetAcceptedAgreementsRequest
		{
			AgreementKeys = array
		}, delegate(Dictionary<string, string> result)
		{
			state = 1;
			returnValue = result;
		}, delegate(PlayFabError error)
		{
			Debug.LogError(error.ErrorMessage);
			state = -1;
		});
		while (state == 0)
		{
			await Task.Yield();
		}
		return returnValue;
	}

	// Token: 0x06004CD7 RID: 19671 RVA: 0x00199B90 File Offset: 0x00197D90
	private async Task SubmitAcceptedAgreements(Dictionary<string, string> agreements)
	{
		int state = 0;
		GorillaServer.Instance.SubmitAcceptedAgreements(new SubmitAcceptedAgreementsRequest
		{
			Agreements = agreements
		}, delegate(ExecuteFunctionResult result)
		{
			state = 1;
		}, delegate(PlayFabError error)
		{
			state = -1;
		});
		while (state == 0)
		{
			await Task.Yield();
		}
	}

	// Token: 0x06004CD8 RID: 19672 RVA: 0x0018FAF0 File Offset: 0x0018DCF0
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005FF6 RID: 24566
	private static string SCROLL_TO_END_MESSAGE = "<b>Scroll to the bottom</b> to continue.";

	// Token: 0x04005FF7 RID: 24567
	[Header("Scroll Behavior")]
	[SerializeField]
	protected float _minScrollSpeed = 0.02f;

	// Token: 0x04005FF8 RID: 24568
	[SerializeField]
	private float _maxScrollSpeed = 3f;

	// Token: 0x04005FF9 RID: 24569
	[SerializeField]
	private float _scrollInterpTime = 3f;

	// Token: 0x04005FFA RID: 24570
	[SerializeField]
	private AnimationCurve _scrollInterpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04005FFC RID: 24572
	[SerializeField]
	protected Transform uiParent;

	// Token: 0x04005FFD RID: 24573
	[SerializeField]
	protected TMP_Text tmpBody;

	// Token: 0x04005FFE RID: 24574
	[SerializeField]
	protected TMP_Text tmpTitle;

	// Token: 0x04005FFF RID: 24575
	[SerializeField]
	protected Scrollbar scrollBar;

	// Token: 0x04006000 RID: 24576
	[SerializeField]
	private LegalAgreementTextAsset[] legalAgreementScreens;

	// Token: 0x04006001 RID: 24577
	[SerializeField]
	protected KIDUIButton _pressAndHoldToConfirmButton;

	// Token: 0x04006002 RID: 24578
	[SerializeField]
	private TMP_Text _scrollToBottomText;

	// Token: 0x04006003 RID: 24579
	[SerializeField]
	private float _stickVibrationStrength = 0.1f;

	// Token: 0x04006004 RID: 24580
	[SerializeField]
	private float _stickVibrationDuration = 0.05f;

	// Token: 0x04006005 RID: 24581
	protected float stickHeldDuration;

	// Token: 0x04006006 RID: 24582
	protected float scrollSpeed;

	// Token: 0x04006007 RID: 24583
	private float scrollTime;

	// Token: 0x04006008 RID: 24584
	protected bool legalAgreementsStarted;

	// Token: 0x04006009 RID: 24585
	protected bool _accepted;

	// Token: 0x0400600A RID: 24586
	private string cachedText;

	// Token: 0x0400600B RID: 24587
	private int state;

	// Token: 0x0400600C RID: 24588
	private bool optIn;

	// Token: 0x0400600D RID: 24589
	private bool optional;
}
