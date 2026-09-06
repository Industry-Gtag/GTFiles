using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x0200148E RID: 5262
	[WeaverGenerated]
	[Serializable]
	internal class UnityDictionarySurrogate@ReaderWriter@Fusion_NetworkString`1<Fusion__32>@ReaderWriter@Fusion_NetworkString : UnityDictionarySurrogate<NetworkString<_32>, ReaderWriter@Fusion_NetworkString, NetworkString<_32>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x17000CA9 RID: 3241
		// (get) Token: 0x060083E9 RID: 33769 RVA: 0x002B1EAA File Offset: 0x002B00AA
		// (set) Token: 0x060083EA RID: 33770 RVA: 0x002B1EB2 File Offset: 0x002B00B2
		[WeaverGenerated]
		public override SerializableDictionary<NetworkString<_32>, NetworkString<_32>> DataProperty
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

		// Token: 0x060083EB RID: 33771 RVA: 0x002B1EBB File Offset: 0x002B00BB
		[WeaverGenerated]
		public UnityDictionarySurrogate@ReaderWriter@Fusion_NetworkString`1<Fusion__32>@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x040095C8 RID: 38344
		[WeaverGenerated]
		public SerializableDictionary<NetworkString<_32>, NetworkString<_32>> Data = SerializableDictionary.Create<NetworkString<_32>, NetworkString<_32>>();
	}
}
