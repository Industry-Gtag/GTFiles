using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020014A5 RID: 5285
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ElementReaderWriterInt32 : UnityLinkedListSurrogate<int, ElementReaderWriterInt32>
	{
		// Token: 0x17000CB2 RID: 3250
		// (get) Token: 0x06008410 RID: 33808 RVA: 0x002B20C9 File Offset: 0x002B02C9
		// (set) Token: 0x06008411 RID: 33809 RVA: 0x002B20D1 File Offset: 0x002B02D1
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

		// Token: 0x06008412 RID: 33810 RVA: 0x002B20DA File Offset: 0x002B02DA
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ElementReaderWriterInt32()
		{
		}

		// Token: 0x04009801 RID: 38913
		[WeaverGenerated]
		public int[] Data = Array.Empty<int>();
	}
}
