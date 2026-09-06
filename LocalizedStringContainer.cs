using System;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x02000C0E RID: 3086
[Serializable]
public struct LocalizedStringContainer
{
	// Token: 0x06004D45 RID: 19781 RVA: 0x0019BFF8 File Offset: 0x0019A1F8
	public string GetName()
	{
		string localizedString = this.StringReference.GetLocalizedString();
		string text = ((localizedString != null) ? localizedString.ToUpper() : null);
		if (string.IsNullOrEmpty(text) || text.ToLower().Contains("no translation found"))
		{
			return this.FallbackName;
		}
		return text;
	}

	// Token: 0x0400609F RID: 24735
	[SerializeField]
	private LocalizedString StringReference;

	// Token: 0x040060A0 RID: 24736
	[SerializeField]
	private string FallbackName;
}
