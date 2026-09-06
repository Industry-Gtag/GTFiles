using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001486 RID: 5254
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterNetworkBool : UnityArraySurrogate<NetworkBool, ElementReaderWriterNetworkBool>
	{
		// Token: 0x17000CA6 RID: 3238
		// (get) Token: 0x060083DA RID: 33754 RVA: 0x002B1DDF File Offset: 0x002AFFDF
		// (set) Token: 0x060083DB RID: 33755 RVA: 0x002B1DE7 File Offset: 0x002AFFE7
		[WeaverGenerated]
		public override NetworkBool[] DataProperty
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

		// Token: 0x060083DC RID: 33756 RVA: 0x002B1DF0 File Offset: 0x002AFFF0
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterNetworkBool()
		{
		}

		// Token: 0x040094EF RID: 38127
		[WeaverGenerated]
		public NetworkBool[] Data = Array.Empty<NetworkBool>();
	}
}
