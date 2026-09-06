using System;
using GorillaNetworking;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

// Token: 0x02000A41 RID: 2625
public class PlayFabTitleDataTextDisplay : MonoBehaviour, IBuildValidation
{
	// Token: 0x1700065F RID: 1631
	// (get) Token: 0x06004364 RID: 17252 RVA: 0x00166A8E File Offset: 0x00164C8E
	public string playFabKeyValue
	{
		get
		{
			return this.playfabKey;
		}
	}

	// Token: 0x06004365 RID: 17253 RVA: 0x00166A98 File Offset: 0x00164C98
	private void Start()
	{
		if (this.textBox != null)
		{
			this.textBox.color = this.defaultTextColor;
		}
		else
		{
			Debug.LogError("The TextBox is null on this PlayFabTitleDataTextDisplay component");
		}
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnNewTitleDataAdded));
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError), false);
		if (!this._hasRegisteredCallback)
		{
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		}
	}

	// Token: 0x06004366 RID: 17254 RVA: 0x00166B2D File Offset: 0x00164D2D
	private void OnEnable()
	{
		if (LocalisationManager.Instance == null)
		{
			return;
		}
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		this._hasRegisteredCallback = true;
	}

	// Token: 0x06004367 RID: 17255 RVA: 0x00166B55 File Offset: 0x00164D55
	private void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		this._hasRegisteredCallback = false;
	}

	// Token: 0x06004368 RID: 17256 RVA: 0x00166B70 File Offset: 0x00164D70
	private void OnPlayFabError(PlayFabError error)
	{
		if (this.textBox != null)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"PlayFabTitleDataTextDisplay: PlayFab error retrieving title data for key '",
				this.playfabKey,
				"' displayed '",
				this.fallbackText,
				"': ",
				error.GenerateErrorReport()
			}));
			if (this._fallbackLocalizedText == null || this._fallbackLocalizedText.IsEmpty)
			{
				this.textBox.text = this.fallbackText;
				return;
			}
			string text;
			if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(this._fallbackLocalizedText, out text, this.fallbackText, null))
			{
				Debug.LogError("[LOCALIZATION::PLAYFAB_TITLEDATA_TEXT_DISPLAY] Failed to get key for PlayFab Title Data Text [_fallbackLocalizedText]");
			}
			this.textBox.text = text;
		}
	}

	// Token: 0x06004369 RID: 17257 RVA: 0x00166C24 File Offset: 0x00164E24
	private void OnLanguageChanged()
	{
		if (string.IsNullOrEmpty(this._cachedText))
		{
			Debug.LogError("[LOCALIZATION::PLAY_FAB_TITLE_DATA_TEXT_DISPLAY] [_cachedText] is not set yet, is this being called before title data has been obtained?");
			return;
		}
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError), false);
	}

	// Token: 0x0600436A RID: 17258 RVA: 0x00166C74 File Offset: 0x00164E74
	private void OnTitleDataRequestComplete(string titleDataResult)
	{
		if (this.textBox != null)
		{
			this._cachedText = titleDataResult;
			string text = titleDataResult.Replace("\\r", "\r").Replace("\\n", "\n");
			if (text[0] == '"' && text[text.Length - 1] == '"')
			{
				text = text.Substring(1, text.Length - 2);
			}
			this.textBox.text = text;
		}
	}

	// Token: 0x0600436B RID: 17259 RVA: 0x00166CEF File Offset: 0x00164EEF
	private void OnNewTitleDataAdded(string key)
	{
		if (key == this.playfabKey && this.textBox != null)
		{
			this.textBox.color = this.newUpdateColor;
		}
	}

	// Token: 0x0600436C RID: 17260 RVA: 0x00166D1E File Offset: 0x00164F1E
	private void OnDestroy()
	{
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnNewTitleDataAdded));
	}

	// Token: 0x0600436D RID: 17261 RVA: 0x00166D3B File Offset: 0x00164F3B
	public bool BuildValidationCheck()
	{
		if (this.textBox == null)
		{
			Debug.LogError("text reference is null! sign text will be broken");
			return false;
		}
		return true;
	}

	// Token: 0x0600436E RID: 17262 RVA: 0x00166D58 File Offset: 0x00164F58
	public void ChangeTitleDataAtRuntime(string newTitleDataKey)
	{
		this.playfabKey = newTitleDataKey;
		if (this.textBox != null)
		{
			this.textBox.color = this.defaultTextColor;
		}
		else
		{
			Debug.LogError("The TextBox is null on this PlayFabTitleDataTextDisplay component");
		}
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnNewTitleDataAdded));
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError), false);
	}

	// Token: 0x04005548 RID: 21832
	[SerializeField]
	private TextMeshPro textBox;

	// Token: 0x04005549 RID: 21833
	[SerializeField]
	private Color newUpdateColor = Color.magenta;

	// Token: 0x0400554A RID: 21834
	[SerializeField]
	private Color defaultTextColor = Color.white;

	// Token: 0x0400554B RID: 21835
	[Tooltip("PlayFab Title Data key from where to pull display text")]
	[SerializeField]
	private string playfabKey;

	// Token: 0x0400554C RID: 21836
	[Tooltip("Text to display when error occurs during fetch")]
	[TextArea(3, 5)]
	[SerializeField]
	private string fallbackText;

	// Token: 0x0400554D RID: 21837
	[SerializeField]
	private LocalizedString _fallbackLocalizedText;

	// Token: 0x0400554E RID: 21838
	private bool _hasRegisteredCallback;

	// Token: 0x0400554F RID: 21839
	private string _cachedText = string.Empty;
}
