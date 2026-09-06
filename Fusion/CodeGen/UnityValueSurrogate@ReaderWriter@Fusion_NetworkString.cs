using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x0200147F RID: 5247
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@Fusion_NetworkString : UnityValueSurrogate<NetworkString<_32>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x17000CA3 RID: 3235
		// (get) Token: 0x060083D1 RID: 33745 RVA: 0x002B1D94 File Offset: 0x002AFF94
		// (set) Token: 0x060083D2 RID: 33746 RVA: 0x002B1D9C File Offset: 0x002AFF9C
		[WeaverGenerated]
		public override NetworkString<_32> DataProperty
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

		// Token: 0x060083D3 RID: 33747 RVA: 0x002B1DA5 File Offset: 0x002AFFA5
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x040094D3 RID: 38099
		[WeaverGenerated]
		public NetworkString<_32> Data;
	}
}
