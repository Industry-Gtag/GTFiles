using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000C0B RID: 3083
public class LocalizationTextSyncer : MonoBehaviour
{
	// Token: 0x06004D3C RID: 19772 RVA: 0x0019BDB7 File Offset: 0x00199FB7
	private void Start()
	{
		this.OnLanguageChanged();
	}

	// Token: 0x06004D3D RID: 19773 RVA: 0x0019BDBF File Offset: 0x00199FBF
	private void OnEnable()
	{
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		if (LocalisationManager.Instance == null)
		{
			return;
		}
		this.OnLanguageChanged();
	}

	// Token: 0x06004D3E RID: 19774 RVA: 0x0019BDE6 File Offset: 0x00199FE6
	private void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x06004D3F RID: 19775 RVA: 0x0019BDE6 File Offset: 0x00199FE6
	private void OnDestroy()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x06004D40 RID: 19776 RVA: 0x0019BDFC File Offset: 0x00199FFC
	private void OnLanguageChanged()
	{
		LocalisationFontPair localisationFontPair;
		LocalisationManager.GetFontAssetForCurrentLocale(out localisationFontPair);
		LocalisationFontPair localisationFontPair2;
		bool flag = this.TryGetFontDataOverride(out localisationFontPair2);
		if (!flag && !LocalisationManager.GetFontAssetForCurrentLocale(out localisationFontPair2))
		{
			return;
		}
		foreach (LocalizationTextSyncer.TextCompSyncData textCompSyncData in this._textComponentsToSync)
		{
			if (!(textCompSyncData.textComponent == null))
			{
				LocalisationFontPair localisationFontPair3;
				if (textCompSyncData.overrideLanguageSettings && textCompSyncData.GetOverrideForLanguage(out localisationFontPair3))
				{
					localisationFontPair2 = localisationFontPair3;
				}
				if (localisationFontPair2.fontAsset != null)
				{
					textCompSyncData.textComponent.font = localisationFontPair2.fontAsset;
				}
				else
				{
					textCompSyncData.textComponent.font = localisationFontPair.fontAsset;
				}
				if (flag)
				{
					textCompSyncData.textComponent.characterSpacing = localisationFontPair2.charSpacing;
					textCompSyncData.textComponent.lineSpacing = localisationFontPair2.lineSpacing;
					if (localisationFontPair2.fontSize != 0f)
					{
						textCompSyncData.textComponent.fontSize = (textCompSyncData.textComponent.fontSizeMax = localisationFontPair2.fontSize);
					}
				}
			}
		}
	}

	// Token: 0x06004D41 RID: 19777 RVA: 0x0019BF24 File Offset: 0x0019A124
	private bool TryGetFontDataOverride(out LocalisationFontPair fontDataOverride)
	{
		fontDataOverride = default(LocalisationFontPair);
		for (int i = 0; i < this._universalFontOverrides.Count; i++)
		{
			if (this._universalFontOverrides[i].ContainsLocale(LocalisationManager.CurrentLanguage))
			{
				fontDataOverride = this._universalFontOverrides[i];
				return true;
			}
		}
		return false;
	}

	// Token: 0x0400609A RID: 24730
	[SerializeField]
	[Tooltip("List of all the Text Components - and optional overrides - that will be updated when langauge changes")]
	private List<LocalizationTextSyncer.TextCompSyncData> _textComponentsToSync = new List<LocalizationTextSyncer.TextCompSyncData>();

	// Token: 0x0400609B RID: 24731
	[SerializeField]
	[Tooltip("List of optional overrides that will be applied to ALL Text Components on this object")]
	private List<LocalisationFontPair> _universalFontOverrides = new List<LocalisationFontPair>();

	// Token: 0x02000C0C RID: 3084
	[Serializable]
	public struct TextCompSyncData
	{
		// Token: 0x06004D43 RID: 19779 RVA: 0x0019BF9C File Offset: 0x0019A19C
		public bool GetOverrideForLanguage(out LocalisationFontPair fontData)
		{
			fontData = default(LocalisationFontPair);
			for (int i = 0; i < this._fontOverrides.Count; i++)
			{
				if (this._fontOverrides[i].ContainsLocale(LocalisationManager.CurrentLanguage))
				{
					fontData = this._fontOverrides[i];
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400609C RID: 24732
		public TMP_Text textComponent;

		// Token: 0x0400609D RID: 24733
		public bool overrideLanguageSettings;

		// Token: 0x0400609E RID: 24734
		public List<LocalisationFontPair> _fontOverrides;
	}
}
