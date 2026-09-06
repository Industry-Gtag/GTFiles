using System;
using UnityEngine;

// Token: 0x020002CD RID: 717
[Serializable]
public class GestureNode
{
	// Token: 0x04001658 RID: 5720
	public bool track;

	// Token: 0x04001659 RID: 5721
	public GestureHandState state;

	// Token: 0x0400165A RID: 5722
	public GestureDigitFlexion flexion;

	// Token: 0x0400165B RID: 5723
	public GestureAlignment alignment;

	// Token: 0x0400165C RID: 5724
	[Space]
	public GestureNodeFlags flags;
}
