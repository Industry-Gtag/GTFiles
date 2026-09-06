using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02001182 RID: 4482
	public class GameObjectOnDisableDispatcher : MonoBehaviour
	{
		// Token: 0x140000C5 RID: 197
		// (add) Token: 0x06007054 RID: 28756 RVA: 0x002437E0 File Offset: 0x002419E0
		// (remove) Token: 0x06007055 RID: 28757 RVA: 0x00243818 File Offset: 0x00241A18
		public event GameObjectOnDisableDispatcher.OnDisabledEvent OnDisabled;

		// Token: 0x06007056 RID: 28758 RVA: 0x0024384D File Offset: 0x00241A4D
		private void OnDisable()
		{
			if (this.OnDisabled != null)
			{
				this.OnDisabled(this);
			}
		}

		// Token: 0x02001183 RID: 4483
		// (Invoke) Token: 0x06007059 RID: 28761
		public delegate void OnDisabledEvent(GameObjectOnDisableDispatcher me);
	}
}
