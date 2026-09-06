using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x0200149E RID: 5278
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@Fusion_NetworkString : UnityValueSurrogate<NetworkString<_128>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x17000CAF RID: 3247
		// (get) Token: 0x06008407 RID: 33799 RVA: 0x002B2068 File Offset: 0x002B0268
		// (set) Token: 0x06008408 RID: 33800 RVA: 0x002B2070 File Offset: 0x002B0270
		[WeaverGenerated]
		public override NetworkString<_128> DataProperty
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

		// Token: 0x06008409 RID: 33801 RVA: 0x002B2079 File Offset: 0x002B0279
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x040097AF RID: 38831
		[WeaverGenerated]
		public NetworkString<_128> Data;
	}
}
