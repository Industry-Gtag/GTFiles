using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001491 RID: 5265
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterInt64 : UnityArraySurrogate<long, ElementReaderWriterInt64>
	{
		// Token: 0x17000CAA RID: 3242
		// (get) Token: 0x060083EC RID: 33772 RVA: 0x002B1ECE File Offset: 0x002B00CE
		// (set) Token: 0x060083ED RID: 33773 RVA: 0x002B1ED6 File Offset: 0x002B00D6
		[WeaverGenerated]
		public override long[] DataProperty
		{
			[WeaverGenerated]
			get
			{
				return this.Data;
			}
			[WeaverGenerated]
			set
			{
				this.Data = value;
			}
		}

		// Token: 0x060083EE RID: 33774 RVA: 0x002B1EDF File Offset: 0x002B00DF
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterInt64()
		{
		}

		// Token: 0x040095D4 RID: 38356
		[WeaverGenerated]
		public long[] Data = Array.Empty<long>();
	}
}
