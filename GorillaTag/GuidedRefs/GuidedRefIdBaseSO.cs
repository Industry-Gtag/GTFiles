using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001265 RID: 4709
	public abstract class GuidedRefIdBaseSO : ScriptableObject, IGuidedRefObject
	{
		// Token: 0x0600772E RID: 30510 RVA: 0x00002C2D File Offset: 0x00000E2D
		public virtual void GuidedRefInitialize()
		{
		}

		// Token: 0x06007730 RID: 30512 RVA: 0x00019405 File Offset: 0x00017605
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}
	}
}
