using System;
using UnityEngine;

// Token: 0x02000C13 RID: 3091
[Serializable]
public class TitleDataLocalization
{
	// Token: 0x06004D60 RID: 19808 RVA: 0x0019C7D8 File Offset: 0x0019A9D8
	public string GetLocalizedText()
	{
		Debug.Log("TODO: JH - Review localization method");
		string code = LocalisationManager.CurrentLanguage.Identifier.Code;
		if (!(code == "en"))
		{
			if (code == "fr")
			{
				return this.French;
			}
			if (code == "es")
			{
				return this.Spanish;
			}
			if (code == "it")
			{
				return this.Italian;
			}
			if (code == "de")
			{
				return this.German;
			}
			if (code == "ja")
			{
				return this.Japanese;
			}
		}
		return this.English;
	}

	// Token: 0x040060B6 RID: 24758
	public string English;

	// Token: 0x040060B7 RID: 24759
	public string French;

	// Token: 0x040060B8 RID: 24760
	public string German;

	// Token: 0x040060B9 RID: 24761
	public string Spanish;

	// Token: 0x040060BA RID: 24762
	public string Italian;

	// Token: 0x040060BB RID: 24763
	public string Japanese;
}
