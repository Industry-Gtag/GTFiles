using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020014A6 RID: 5286
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterBoolean : UnityArraySurrogate<bool, ElementReaderWriterBoolean>
	{
		// Token: 0x17000CB3 RID: 3251
		// (get) Token: 0x06008413 RID: 33811 RVA: 0x002B20ED File Offset: 0x002B02ED
		// (set) Token: 0x06008414 RID: 33812 RVA: 0x002B20F5 File Offset: 0x002B02F5
		[WeaverGenerated]
		public override bool[] DataProperty
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

		// Token: 0x06008415 RID: 33813 RVA: 0x002B20FE File Offset: 0x002B02FE
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterBoolean()
		{
		}

		// Token: 0x04009802 RID: 38914
		[WeaverGenerated]
		public bool[] Data = Array.Empty<bool>();
	}
}
