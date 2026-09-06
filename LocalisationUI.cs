using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x02000C06 RID: 3078
public class LocalisationUI : MonoBehaviour
{
	// Token: 0x17000765 RID: 1893
	// (get) Token: 0x06004D2D RID: 19757 RVA: 0x0019B9D1 File Offset: 0x00199BD1
	public static LocalisationUI Instance
	{
		get
		{
			return LocalisationUI._instance;
		}
	}

	// Token: 0x06004D2E RID: 19758 RVA: 0x0019B9D8 File Offset: 0x00199BD8
	private void Awake()
	{
		if (LocalisationUI._instance != null)
		{
			Object.DestroyImmediate(this);
			return;
		}
		LocalisationUI._instance = this;
	}

	// Token: 0x06004D2F RID: 19759 RVA: 0x0019B9F4 File Offset: 0x00199BF4
	private void Start()
	{
		this.ConstructLocalisationUI();
		this.CheckSelectedLanguage();
	}

	// Token: 0x06004D30 RID: 19760 RVA: 0x0019BA02 File Offset: 0x00199C02
	private void OnEnable()
	{
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		if (this._hasConstructedUI)
		{
			this.CheckSelectedLanguage();
		}
	}

	// Token: 0x06004D31 RID: 19761 RVA: 0x0019BA23 File Offset: 0x00199C23
	private void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x06004D32 RID: 19762 RVA: 0x0019BA38 File Offset: 0x00199C38
	public void OnLanguageButtonPressed(KIDUIButton objRef, int languageIndex)
	{
		if (objRef != this._activeButton)
		{
			KIDUIButton activeButton = this._activeButton;
			if (activeButton != null)
			{
				activeButton.SetBorderImage(this._inactiveSprite);
			}
			objRef.SetBorderImage(this._activeSprite);
			this._activeButton = objRef;
		}
		Locale locale;
		if (!LocalisationManager.TryGetLocaleBinding(languageIndex, out locale))
		{
			return;
		}
		LocalisationManager.Instance.OnLanguageButtonPressed(locale.Identifier.Code, false);
	}

	// Token: 0x06004D33 RID: 19763 RVA: 0x0019BAA1 File Offset: 0x00199CA1
	public void OnContinueButtonPressed()
	{
		HandRayController.Instance.DisableHandRays();
		PrivateUIRoom.RemoveUI(LocalisationUI.GetUITransform());
		LocalisationManager.OnSaveLanguage();
	}

	// Token: 0x06004D34 RID: 19764 RVA: 0x0019BABC File Offset: 0x00199CBC
	private void ConstructLocalisationUI()
	{
		using (Dictionary<int, Locale>.Enumerator enumerator = LocalisationManager.GetAllBindings().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, Locale> item = enumerator.Current;
				KIDUIButton newButton = Object.Instantiate<KIDUIButton>(this._languageButtonPrefab, this._languageButtonGridTransform);
				bool flag = LocalisationManager.CurrentLanguage.Identifier.Code.ToLower() != "ja";
				newButton.SetText(LocalisationManager.LocaleToFriendlyString(item.Value, flag).ToUpper());
				newButton.onClick.AddListener(delegate
				{
					this.OnLanguageButtonPressed(newButton, item.Key);
				});
				this._languageButtons.Add(newButton);
			}
		}
		this._hasConstructedUI = true;
	}

	// Token: 0x06004D35 RID: 19765 RVA: 0x0019BBB0 File Offset: 0x00199DB0
	private void CheckSelectedLanguage()
	{
		KIDUIButton kiduibutton = null;
		for (int i = 0; i < this._languageButtons.Count; i++)
		{
			bool flag = LocalisationManager.CurrentLanguage.Identifier.Code.ToLower() != "ja";
			if (!(this._languageButtons[i].GetText() != LocalisationManager.LocaleToFriendlyString(LocalisationManager.CurrentLanguage, flag).ToUpper()))
			{
				kiduibutton = this._languageButtons[i];
				break;
			}
		}
		if (kiduibutton == null)
		{
			return;
		}
		if (this._activeButton != null)
		{
			this._activeButton.SetBorderImage(this._inactiveSprite);
		}
		kiduibutton.SetBorderImage(this._activeSprite);
		this._activeButton = kiduibutton;
	}

	// Token: 0x06004D36 RID: 19766 RVA: 0x0019BC6C File Offset: 0x00199E6C
	private void OnLanguageChanged()
	{
		for (int i = 0; i < this._languageButtons.Count; i++)
		{
			bool flag = LocalisationManager.CurrentLanguage.Identifier.Code.ToLower() != "ja";
			this._languageButtons[i].SetText(LocalisationManager.LocaleDisplayNameToFriendlyString(this._languageButtons[i].GetText(), flag).ToUpper());
			if (!(LocalisationManager.CurrentLanguage.Identifier.Code == "ja"))
			{
				this._languageButtons[i].SetFont(this._defaultFont);
			}
			else
			{
				this._languageButtons[i].SetFont(this._japaneseFont);
			}
		}
	}

	// Token: 0x06004D37 RID: 19767 RVA: 0x0019BD34 File Offset: 0x00199F34
	public static Transform GetUITransform()
	{
		if (LocalisationUI.Instance == null)
		{
			return null;
		}
		if (LocalisationUI.Instance._uiTransform == null)
		{
			LocalisationUI.Instance._uiTransform = LocalisationUI.Instance.transform.GetChild(0);
		}
		return LocalisationUI.Instance._uiTransform;
	}

	// Token: 0x04006083 RID: 24707
	private static LocalisationUI _instance;

	// Token: 0x04006084 RID: 24708
	[Header("Text Components")]
	[SerializeField]
	private TMP_Text _titleTxt;

	// Token: 0x04006085 RID: 24709
	[SerializeField]
	private TMP_Text _confirmBtnTxt;

	// Token: 0x04006086 RID: 24710
	[Header("UI Setup")]
	[SerializeField]
	private KIDUIButton _languageButtonPrefab;

	// Token: 0x04006087 RID: 24711
	[SerializeField]
	private Transform _languageButtonGridTransform;

	// Token: 0x04006088 RID: 24712
	[SerializeField]
	private Sprite _activeSprite;

	// Token: 0x04006089 RID: 24713
	[SerializeField]
	private Sprite _inactiveSprite;

	// Token: 0x0400608A RID: 24714
	[SerializeField]
	private TMP_FontAsset _defaultFont;

	// Token: 0x0400608B RID: 24715
	[SerializeField]
	private TMP_FontAsset _japaneseFont;

	// Token: 0x0400608C RID: 24716
	private Transform _uiTransform;

	// Token: 0x0400608D RID: 24717
	private KIDUIButton _activeButton;

	// Token: 0x0400608E RID: 24718
	private List<KIDUIButton> _languageButtons = new List<KIDUIButton>();

	// Token: 0x0400608F RID: 24719
	private bool _hasConstructedUI;
}
