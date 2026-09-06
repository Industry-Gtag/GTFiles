using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F56 RID: 3926
	public class BuilderAttachPoint : MonoBehaviour
	{
		// Token: 0x06006082 RID: 24706 RVA: 0x001E9AF5 File Offset: 0x001E7CF5
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x04006F1E RID: 28446
		public Transform center;
	}
}
