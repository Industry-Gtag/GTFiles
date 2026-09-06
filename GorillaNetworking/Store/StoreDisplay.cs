using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02001150 RID: 4432
	public class StoreDisplay : MonoBehaviour
	{
		// Token: 0x06006F43 RID: 28483 RVA: 0x0023E137 File Offset: 0x0023C337
		private void GetAllDynamicCosmeticStands()
		{
			this.Stands = base.GetComponentsInChildren<DynamicCosmeticStand>();
		}

		// Token: 0x06006F44 RID: 28484 RVA: 0x0023E148 File Offset: 0x0023C348
		private void SetDisplayNameForAllStands()
		{
			DynamicCosmeticStand[] componentsInChildren = base.GetComponentsInChildren<DynamicCosmeticStand>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].CopyChildsName();
			}
		}

		// Token: 0x04007F45 RID: 32581
		public string displayName = "";

		// Token: 0x04007F46 RID: 32582
		public DynamicCosmeticStand[] Stands;
	}
}
