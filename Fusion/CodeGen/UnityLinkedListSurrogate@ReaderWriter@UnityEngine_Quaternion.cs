using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x0200149A RID: 5274
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion : UnityLinkedListSurrogate<Quaternion, ReaderWriter@UnityEngine_Quaternion>
	{
		// Token: 0x17000CAE RID: 3246
		// (get) Token: 0x060083FE RID: 33790 RVA: 0x002B1FC8 File Offset: 0x002B01C8
		// (set) Token: 0x060083FF RID: 33791 RVA: 0x002B1FD0 File Offset: 0x002B01D0
		[WeaverGenerated]
		public override Quaternion[] DataProperty
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

		// Token: 0x06008400 RID: 33792 RVA: 0x002B1FD9 File Offset: 0x002B01D9
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion()
		{
		}

		// Token: 0x0400972B RID: 38699
		[WeaverGenerated]
		public Quaternion[] Data = Array.Empty<Quaternion>();
	}
}
