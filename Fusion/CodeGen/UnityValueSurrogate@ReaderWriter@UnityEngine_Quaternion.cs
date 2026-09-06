using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x0200148A RID: 5258
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion : UnityValueSurrogate<Quaternion, ReaderWriter@UnityEngine_Quaternion>
	{
		// Token: 0x17000CA7 RID: 3239
		// (get) Token: 0x060083E3 RID: 33763 RVA: 0x002B1E78 File Offset: 0x002B0078
		// (set) Token: 0x060083E4 RID: 33764 RVA: 0x002B1E80 File Offset: 0x002B0080
		[WeaverGenerated]
		public override Quaternion DataProperty
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

		// Token: 0x060083E5 RID: 33765 RVA: 0x002B1E89 File Offset: 0x002B0089
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion()
		{
		}

		// Token: 0x040094F6 RID: 38134
		[WeaverGenerated]
		public Quaternion Data;
	}
}
