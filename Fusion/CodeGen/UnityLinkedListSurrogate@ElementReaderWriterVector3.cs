using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x02001497 RID: 5271
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ElementReaderWriterVector3 : UnityLinkedListSurrogate<Vector3, ElementReaderWriterVector3>
	{
		// Token: 0x17000CAD RID: 3245
		// (get) Token: 0x060083FB RID: 33787 RVA: 0x002B1FA4 File Offset: 0x002B01A4
		// (set) Token: 0x060083FC RID: 33788 RVA: 0x002B1FAC File Offset: 0x002B01AC
		[WeaverGenerated]
		public override Vector3[] DataProperty
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

		// Token: 0x060083FD RID: 33789 RVA: 0x002B1FB5 File Offset: 0x002B01B5
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ElementReaderWriterVector3()
		{
		}

		// Token: 0x04009672 RID: 38514
		[WeaverGenerated]
		public Vector3[] Data = Array.Empty<Vector3>();
	}
}
