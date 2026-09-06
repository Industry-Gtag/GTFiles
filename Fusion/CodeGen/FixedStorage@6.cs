using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen
{
	// Token: 0x020014A2 RID: 5282
	[WeaverGenerated]
	[NetworkStructWeaved(6)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	internal struct FixedStorage@6 : INetworkStruct
	{
		// Token: 0x040097F9 RID: 38905
		[FixedBuffer(typeof(int), 6)]
		[WeaverGenerated]
		[FieldOffset(0)]
		public FixedStorage@6.<Data>e__FixedBuffer Data;

		// Token: 0x040097FA RID: 38906
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(4)]
		private int _1;

		// Token: 0x040097FB RID: 38907
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(8)]
		private int _2;

		// Token: 0x040097FC RID: 38908
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(12)]
		private int _3;

		// Token: 0x040097FD RID: 38909
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(16)]
		private int _4;

		// Token: 0x040097FE RID: 38910
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(20)]
		private int _5;

		// Token: 0x020014A3 RID: 5283
		[CompilerGenerated]
		[UnsafeValueType]
		[WeaverGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 24)]
		public struct <Data>e__FixedBuffer
		{
			// Token: 0x040097FF RID: 38911
			[WeaverGenerated]
			public int FixedElementField;
		}
	}
}
