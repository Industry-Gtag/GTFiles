using System;
using UnityEngine;

// Token: 0x02000338 RID: 824
public class FlagCauldronColorer : MonoBehaviour
{
	// Token: 0x0400191E RID: 6430
	public FlagCauldronColorer.ColorMode mode;

	// Token: 0x0400191F RID: 6431
	public Transform colorPoint;

	// Token: 0x02000339 RID: 825
	public enum ColorMode
	{
		// Token: 0x04001921 RID: 6433
		None,
		// Token: 0x04001922 RID: 6434
		Red,
		// Token: 0x04001923 RID: 6435
		Green,
		// Token: 0x04001924 RID: 6436
		Blue,
		// Token: 0x04001925 RID: 6437
		Black,
		// Token: 0x04001926 RID: 6438
		Clear
	}
}
