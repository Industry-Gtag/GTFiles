using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen
{
	// Token: 0x02001480 RID: 5248
	[WeaverGenerated]
	[NetworkStructWeaved(3)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	internal struct FixedStorage@3 : INetworkStruct
	{
		// Token: 0x040094D4 RID: 38100
		[FixedBuffer(typeof(int), 3)]
		[WeaverGenerated]
		[FieldOffset(0)]
		public FixedStorage@3.<Data>e__FixedBuffer Data;

		// Token: 0x040094D5 RID: 38101
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(4)]
		private int _1;

		// Token: 0x040094D6 RID: 38102
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(8)]
		private int _2;

		// Token: 0x02001481 RID: 5249
		[CompilerGenerated]
		[UnsafeValueType]
		[WeaverGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 12)]
		public struct <Data>e__FixedBuffer
		{
			// Token: 0x040094D7 RID: 38103
			[WeaverGenerated]
			public int FixedElementField;
		}
	}
}
