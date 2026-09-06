using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001492 RID: 5266
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterInt32 : UnityArraySurrogate<int, ElementReaderWriterInt32>
	{
		// Token: 0x17000CAB RID: 3243
		// (get) Token: 0x060083EF RID: 33775 RVA: 0x002B1EF2 File Offset: 0x002B00F2
		// (set) Token: 0x060083F0 RID: 33776 RVA: 0x002B1EFA File Offset: 0x002B00FA
		[WeaverGenerated]
		public override int[] DataProperty
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

		// Token: 0x060083F1 RID: 33777 RVA: 0x002B1F03 File Offset: 0x002B0103
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterInt32()
		{
		}

		// Token: 0x040095D5 RID: 38357
		[WeaverGenerated]
		public int[] Data = Array.Empty<int>();
	}
}
