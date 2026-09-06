using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020014A4 RID: 5284
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ElementReaderWriterByte : UnityLinkedListSurrogate<byte, ElementReaderWriterByte>
	{
		// Token: 0x17000CB1 RID: 3249
		// (get) Token: 0x0600840D RID: 33805 RVA: 0x002B20A5 File Offset: 0x002B02A5
		// (set) Token: 0x0600840E RID: 33806 RVA: 0x002B20AD File Offset: 0x002B02AD
		[WeaverGenerated]
		public override byte[] DataProperty
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

		// Token: 0x0600840F RID: 33807 RVA: 0x002B20B6 File Offset: 0x002B02B6
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ElementReaderWriterByte()
		{
		}

		// Token: 0x04009800 RID: 38912
		[WeaverGenerated]
		public byte[] Data = Array.Empty<byte>();
	}
}
