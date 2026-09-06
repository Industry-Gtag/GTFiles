using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020014A7 RID: 5287
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterSingle : UnityArraySurrogate<float, ElementReaderWriterSingle>
	{
		// Token: 0x17000CB4 RID: 3252
		// (get) Token: 0x06008416 RID: 33814 RVA: 0x002B2111 File Offset: 0x002B0311
		// (set) Token: 0x06008417 RID: 33815 RVA: 0x002B2119 File Offset: 0x002B0319
		[WeaverGenerated]
		public override float[] DataProperty
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

		// Token: 0x06008418 RID: 33816 RVA: 0x002B2122 File Offset: 0x002B0322
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterSingle()
		{
		}

		// Token: 0x04009803 RID: 38915
		[WeaverGenerated]
		public float[] Data = Array.Empty<float>();
	}
}
