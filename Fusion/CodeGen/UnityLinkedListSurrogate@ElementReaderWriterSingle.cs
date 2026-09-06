using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020014AA RID: 5290
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ElementReaderWriterSingle : UnityLinkedListSurrogate<float, ElementReaderWriterSingle>
	{
		// Token: 0x17000CB5 RID: 3253
		// (get) Token: 0x06008419 RID: 33817 RVA: 0x002B2135 File Offset: 0x002B0335
		// (set) Token: 0x0600841A RID: 33818 RVA: 0x002B213D File Offset: 0x002B033D
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

		// Token: 0x0600841B RID: 33819 RVA: 0x002B2146 File Offset: 0x002B0346
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ElementReaderWriterSingle()
		{
		}

		// Token: 0x04009817 RID: 38935
		[WeaverGenerated]
		public float[] Data = Array.Empty<float>();
	}
}
