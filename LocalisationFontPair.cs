using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x02000C02 RID: 3074
[Serializable]
public struct LocalisationFontPair
{
	// Token: 0x06004D03 RID: 19715 RVA: 0x0019ACF0 File Offset: 0x00198EF0
	public bool ContainsLocale(Locale locale)
	{
		int count = this.locales.Count;
		for (int i = 0; i < this.locales.Count; i++)
		{
			if (!(this.locales[i] == null) && this.locales[i].Identifier.Code == locale.Identifier.Code)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04006062 RID: 24674
	public List<Locale> locales;

	// Token: 0x04006063 RID: 24675
	public TMP_FontAsset fontAsset;

	// Token: 0x04006064 RID: 24676
	public Font legacyFontAsset;

	// Token: 0x04006065 RID: 24677
	public float charSpacing;

	// Token: 0x04006066 RID: 24678
	public float lineSpacing;

	// Token: 0x04006067 RID: 24679
	public float fontSize;
}
