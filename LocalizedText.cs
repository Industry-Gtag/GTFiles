using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Events;

// Token: 0x02000C0F RID: 3087
[DisallowMultipleComponent]
public class LocalizedText : LocalizeStringEvent
{
	// Token: 0x06004D46 RID: 19782 RVA: 0x0019C045 File Offset: 0x0019A245
	public bool HasFontOverrides()
	{
		return this._localisationFontsOverrides.Count > 0;
	}

	// Token: 0x17000767 RID: 1895
	// (get) Token: 0x06004D47 RID: 19783 RVA: 0x0019C055 File Offset: 0x0019A255
	private TextComponentLegacySupportStore TextComponent
	{
		get
		{
			if (!this._textComponent.IsValid)
			{
				this._textComponent = new TextComponentLegacySupportStore(base.transform);
			}
			return this._textComponent;
		}
	}

	// Token: 0x06004D48 RID: 19784 RVA: 0x0019C07C File Offset: 0x0019A27C
	private void Awake()
	{
		this._textComponent = new TextComponentLegacySupportStore(base.transform);
		base.OnUpdateString = new UnityEventString();
		base.OnUpdateString.AddListener(delegate(string val)
		{
			this.OnLocaleChanged(val);
		});
		if (!this.TextComponent.IsValid)
		{
			base.gameObject.AddComponent<TMP_Text>();
			this._textComponent = new TextComponentLegacySupportStore(base.transform);
		}
	}

	// Token: 0x06004D49 RID: 19785 RVA: 0x0019C0EC File Offset: 0x0019A2EC
	protected override async void UpdateString(string value)
	{
		if (LocalisationManager.ApplicationRunning && !LocalisationManager.IsReady)
		{
			await Task.Yield();
		}
		base.UpdateString(value);
	}

	// Token: 0x06004D4A RID: 19786 RVA: 0x0019C12C File Offset: 0x0019A32C
	private async void OnLocaleChanged(string newText)
	{
		if (LocalisationManager.ApplicationRunning && !LocalisationManager.IsReady)
		{
			await Task.Yield();
		}
		if (!ApplicationQuittingState.IsQuitting)
		{
			LocalisationFontPair localisationFontPair;
			if (this.GetLocalizedFonts(out localisationFontPair))
			{
				if (localisationFontPair.fontAsset == null)
				{
					LocalisationFontPair localisationFontPair2;
					if (LocalisationManager.GetFontAssetForCurrentLocale(out localisationFontPair2))
					{
						this.TextComponent.SetFont(localisationFontPair2.fontAsset, localisationFontPair2.legacyFontAsset);
					}
				}
				else
				{
					this.TextComponent.SetFont(localisationFontPair.fontAsset, localisationFontPair.legacyFontAsset);
				}
				if (localisationFontPair.fontSize != 0f && this.HasFontOverrides())
				{
					this.TextComponent.SetFontSize(localisationFontPair.fontSize);
				}
			}
			else
			{
				float time = Time.time;
			}
			if (this.HasFontOverrides())
			{
				this.TextComponent.SetCharSpacing(localisationFontPair.charSpacing);
			}
			this.TextComponent.SetText(newText);
		}
	}

	// Token: 0x06004D4B RID: 19787 RVA: 0x0019C16C File Offset: 0x0019A36C
	private bool GetLocalizedFonts(out LocalisationFontPair fontData)
	{
		fontData = default(LocalisationFontPair);
		if (!this.HasFontOverrides())
		{
			return LocalisationManager.GetFontAssetForCurrentLocale(out fontData);
		}
		for (int i = 0; i < this._localisationFontsOverrides.Count; i++)
		{
			if (this._localisationFontsOverrides[i].ContainsLocale(LocalisationManager.CurrentLanguage))
			{
				fontData = new LocalisationFontPair
				{
					fontAsset = this._localisationFontsOverrides[i].fontAsset,
					legacyFontAsset = this._localisationFontsOverrides[i].legacyFontAsset,
					charSpacing = this._localisationFontsOverrides[i].charSpacing
				};
				return true;
			}
		}
		return LocalisationManager.GetFontAssetForCurrentLocale(out fontData);
	}

	// Token: 0x040060A1 RID: 24737
	[SerializeField]
	private bool _isLocalized;

	// Token: 0x040060A2 RID: 24738
	[SerializeField]
	private bool _isNewKey;

	// Token: 0x040060A3 RID: 24739
	[SerializeField]
	private string _newKeyName;

	// Token: 0x040060A4 RID: 24740
	[SerializeField]
	private ELocale _previewLocale;

	// Token: 0x040060A5 RID: 24741
	[SerializeField]
	private List<LocalisationFontPair> _localisationFontsOverrides = new List<LocalisationFontPair>();

	// Token: 0x040060A6 RID: 24742
	private static List<ELocale> _cachedELocalesList = new List<ELocale>();

	// Token: 0x040060A7 RID: 24743
	private TextComponentLegacySupportStore _textComponent;
}
