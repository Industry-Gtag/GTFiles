using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x0200114F RID: 4431
	public class StoreDepartment : MonoBehaviour
	{
		// Token: 0x06006F41 RID: 28481 RVA: 0x0023E0B4 File Offset: 0x0023C2B4
		private void FindAllDisplays()
		{
			this.Displays = base.GetComponentsInChildren<StoreDisplay>();
			for (int i = this.Displays.Length - 1; i >= 0; i--)
			{
				if (string.IsNullOrEmpty(this.Displays[i].displayName))
				{
					this.Displays[i] = this.Displays[this.Displays.Length - 1];
					Array.Resize<StoreDisplay>(ref this.Displays, this.Displays.Length - 1);
				}
			}
		}

		// Token: 0x04007F43 RID: 32579
		public StoreDisplay[] Displays;

		// Token: 0x04007F44 RID: 32580
		public string departmentName = "";
	}
}
