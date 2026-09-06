using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x020012B7 RID: 4791
	[Serializable]
	public class EdMeshCombinedPrefabData
	{
		// Token: 0x0600784E RID: 30798 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void Clear()
		{
		}

		// Token: 0x0400886E RID: 34926
		public string path;

		// Token: 0x0400886F RID: 34927
		public List<Renderer> disabled = new List<Renderer>(512);

		// Token: 0x04008870 RID: 34928
		public List<GameObject> combined = new List<GameObject>(64);
	}
}
