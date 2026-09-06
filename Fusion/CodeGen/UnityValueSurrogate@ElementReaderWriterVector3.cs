using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x02001482 RID: 5250
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ElementReaderWriterVector3 : UnityValueSurrogate<Vector3, ElementReaderWriterVector3>
	{
		// Token: 0x17000CA4 RID: 3236
		// (get) Token: 0x060083D4 RID: 33748 RVA: 0x002B1DAD File Offset: 0x002AFFAD
		// (set) Token: 0x060083D5 RID: 33749 RVA: 0x002B1DB5 File Offset: 0x002AFFB5
		[WeaverGenerated]
		public override Vector3 DataProperty
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

		// Token: 0x060083D6 RID: 33750 RVA: 0x002B1DBE File Offset: 0x002AFFBE
		[WeaverGenerated]
		public UnityValueSurrogate@ElementReaderWriterVector3()
		{
		}

		// Token: 0x040094D8 RID: 38104
		[WeaverGenerated]
		public Vector3 Data;
	}
}
