using System;

namespace LitJson
{
	// Token: 0x02000EF9 RID: 3833
	public enum JsonToken
	{
		// Token: 0x04006CFA RID: 27898
		None,
		// Token: 0x04006CFB RID: 27899
		ObjectStart,
		// Token: 0x04006CFC RID: 27900
		PropertyName,
		// Token: 0x04006CFD RID: 27901
		ObjectEnd,
		// Token: 0x04006CFE RID: 27902
		ArrayStart,
		// Token: 0x04006CFF RID: 27903
		ArrayEnd,
		// Token: 0x04006D00 RID: 27904
		Int,
		// Token: 0x04006D01 RID: 27905
		Long,
		// Token: 0x04006D02 RID: 27906
		Double,
		// Token: 0x04006D03 RID: 27907
		String,
		// Token: 0x04006D04 RID: 27908
		Boolean,
		// Token: 0x04006D05 RID: 27909
		Null
	}
}
