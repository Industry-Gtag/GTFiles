using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001483 RID: 5251
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ElementReaderWriterSingle : UnityValueSurrogate<float, ElementReaderWriterSingle>
	{
		// Token: 0x17000CA5 RID: 3237
		// (get) Token: 0x060083D7 RID: 33751 RVA: 0x002B1DC6 File Offset: 0x002AFFC6
		// (set) Token: 0x060083D8 RID: 33752 RVA: 0x002B1DCE File Offset: 0x002AFFCE
		[WeaverGenerated]
		public override float DataProperty
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

		// Token: 0x060083D9 RID: 33753 RVA: 0x002B1DD7 File Offset: 0x002AFFD7
		[WeaverGenerated]
		public UnityValueSurrogate@ElementReaderWriterSingle()
		{
		}

		// Token: 0x040094D9 RID: 38105
		[WeaverGenerated]
		public float Data;
	}
}
