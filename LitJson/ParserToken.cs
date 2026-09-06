using System;

namespace LitJson
{
	// Token: 0x02000F01 RID: 3841
	internal enum ParserToken
	{
		// Token: 0x04006D3D RID: 27965
		None = 65536,
		// Token: 0x04006D3E RID: 27966
		Number,
		// Token: 0x04006D3F RID: 27967
		True,
		// Token: 0x04006D40 RID: 27968
		False,
		// Token: 0x04006D41 RID: 27969
		Null,
		// Token: 0x04006D42 RID: 27970
		CharSeq,
		// Token: 0x04006D43 RID: 27971
		Char,
		// Token: 0x04006D44 RID: 27972
		Text,
		// Token: 0x04006D45 RID: 27973
		Object,
		// Token: 0x04006D46 RID: 27974
		ObjectPrime,
		// Token: 0x04006D47 RID: 27975
		Pair,
		// Token: 0x04006D48 RID: 27976
		PairRest,
		// Token: 0x04006D49 RID: 27977
		Array,
		// Token: 0x04006D4A RID: 27978
		ArrayPrime,
		// Token: 0x04006D4B RID: 27979
		Value,
		// Token: 0x04006D4C RID: 27980
		ValueRest,
		// Token: 0x04006D4D RID: 27981
		String,
		// Token: 0x04006D4E RID: 27982
		End,
		// Token: 0x04006D4F RID: 27983
		Epsilon
	}
}
