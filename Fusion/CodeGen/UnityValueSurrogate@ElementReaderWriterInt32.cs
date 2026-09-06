using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x0200148B RID: 5259
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ElementReaderWriterInt32 : UnityValueSurrogate<int, ElementReaderWriterInt32>
	{
		// Token: 0x17000CA8 RID: 3240
		// (get) Token: 0x060083E6 RID: 33766 RVA: 0x002B1E91 File Offset: 0x002B0091
		// (set) Token: 0x060083E7 RID: 33767 RVA: 0x002B1E99 File Offset: 0x002B0099
		[WeaverGenerated]
		public override int DataProperty
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

		// Token: 0x060083E8 RID: 33768 RVA: 0x002B1EA2 File Offset: 0x002B00A2
		[WeaverGenerated]
		public UnityValueSurrogate@ElementReaderWriterInt32()
		{
		}

		// Token: 0x040094F7 RID: 38135
		[WeaverGenerated]
		public int Data;
	}
}
