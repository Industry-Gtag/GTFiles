using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen
{
	// Token: 0x02001487 RID: 5255
	[WeaverGenerated]
	[NetworkStructWeaved(4)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	internal struct FixedStorage@4 : INetworkStruct
	{
		// Token: 0x040094F0 RID: 38128
		[FixedBuffer(typeof(int), 4)]
		[WeaverGenerated]
		[FieldOffset(0)]
		public FixedStorage@4.<Data>e__FixedBuffer Data;

		// Token: 0x040094F1 RID: 38129
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(4)]
		private int _1;

		// Token: 0x040094F2 RID: 38130
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(8)]
		private int _2;

		// Token: 0x040094F3 RID: 38131
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(12)]
		private int _3;

		// Token: 0x02001488 RID: 5256
		[CompilerGenerated]
		[UnsafeValueType]
		[WeaverGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <Data>e__FixedBuffer
		{
			// Token: 0x040094F4 RID: 38132
			[WeaverGenerated]
			public int FixedElementField;
		}
	}
}
