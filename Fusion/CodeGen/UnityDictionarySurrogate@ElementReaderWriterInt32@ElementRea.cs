using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020014A1 RID: 5281
	[WeaverGenerated]
	[Serializable]
	internal class UnityDictionarySurrogate@ElementReaderWriterInt32@ElementReaderWriterInt32 : UnityDictionarySurrogate<int, ElementReaderWriterInt32, int, ElementReaderWriterInt32>
	{
		// Token: 0x17000CB0 RID: 3248
		// (get) Token: 0x0600840A RID: 33802 RVA: 0x002B2081 File Offset: 0x002B0281
		// (set) Token: 0x0600840B RID: 33803 RVA: 0x002B2089 File Offset: 0x002B0289
		[WeaverGenerated]
		public override SerializableDictionary<int, int> DataProperty
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

		// Token: 0x0600840C RID: 33804 RVA: 0x002B2092 File Offset: 0x002B0292
		[WeaverGenerated]
		public UnityDictionarySurrogate@ElementReaderWriterInt32@ElementReaderWriterInt32()
		{
		}

		// Token: 0x040097F8 RID: 38904
		[WeaverGenerated]
		public SerializableDictionary<int, int> Data = SerializableDictionary.Create<int, int>();
	}
}
