using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x0200147B RID: 5243
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ElementReaderWriterNetworkBool : UnityValueSurrogate<NetworkBool, ElementReaderWriterNetworkBool>
	{
		// Token: 0x17000CA2 RID: 3234
		// (get) Token: 0x060083C8 RID: 33736 RVA: 0x002B1CFD File Offset: 0x002AFEFD
		// (set) Token: 0x060083C9 RID: 33737 RVA: 0x002B1D05 File Offset: 0x002AFF05
		[WeaverGenerated]
		public override NetworkBool DataProperty
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

		// Token: 0x060083CA RID: 33738 RVA: 0x002B1D0E File Offset: 0x002AFF0E
		[WeaverGenerated]
		public UnityValueSurrogate@ElementReaderWriterNetworkBool()
		{
		}

		// Token: 0x040094AF RID: 38063
		[WeaverGenerated]
		public NetworkBool Data;
	}
}
