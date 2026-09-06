using System;
using UnityEngine;

// Token: 0x02000AE0 RID: 2784
public class GreyZoneAreaEnable : MonoBehaviour
{
	// Token: 0x0600476C RID: 18284 RVA: 0x00181428 File Offset: 0x0017F628
	private void OnEnable()
	{
		GreyZoneManager instance = GreyZoneManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.RegisterArea(this);
	}

	// Token: 0x0600476D RID: 18285 RVA: 0x0018143C File Offset: 0x0017F63C
	private void OnDisable()
	{
		GreyZoneManager.Instance.UnRegisterArea(this);
	}
}
