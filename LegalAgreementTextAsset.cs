using System;
using UnityEngine;

// Token: 0x02000BFD RID: 3069
[CreateAssetMenu(fileName = "NewLegalAgreementAsset", menuName = "Gorilla Tag/Legal Agreement Asset")]
public class LegalAgreementTextAsset : ScriptableObject
{
	// Token: 0x04006042 RID: 24642
	public string title;

	// Token: 0x04006043 RID: 24643
	public string playFabKey;

	// Token: 0x04006044 RID: 24644
	public string latestVersionKey;

	// Token: 0x04006045 RID: 24645
	[TextArea(3, 5)]
	public string errorMessage;

	// Token: 0x04006046 RID: 24646
	public bool optional;

	// Token: 0x04006047 RID: 24647
	public LegalAgreementTextAsset.PostAcceptAction optInAction;

	// Token: 0x04006048 RID: 24648
	public string confirmString;

	// Token: 0x02000BFE RID: 3070
	public enum PostAcceptAction
	{
		// Token: 0x0400604A RID: 24650
		NONE
	}
}
