using System;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000FDA RID: 4058
	public class VirtualStumpAlarmWaker : MonoBehaviour
	{
		// Token: 0x06006503 RID: 25859 RVA: 0x00208556 File Offset: 0x00206756
		public void EnterStumpCustomMode()
		{
			CustomMapManager.Activate(VirtualStumpActivateMode.Custom, false);
		}
	}
}
