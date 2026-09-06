using System;
using UnityEngine;

// Token: 0x020003D6 RID: 982
public class GreyZoneActivator : MonoBehaviour
{
	// Token: 0x06001767 RID: 5991 RVA: 0x00087238 File Offset: 0x00085438
	private void OnEnable()
	{
		if (this.activateOnEnable)
		{
			this.Activate();
		}
	}

	// Token: 0x06001768 RID: 5992 RVA: 0x00087248 File Offset: 0x00085448
	private void OnDisable()
	{
		if (this.deactivateOnDisable)
		{
			this.Deactivate();
		}
	}

	// Token: 0x06001769 RID: 5993 RVA: 0x00087258 File Offset: 0x00085458
	public void Activate()
	{
		GreyZoneManager.Instance.LocalSimpleActivation(true, this.gMultiplier);
	}

	// Token: 0x0600176A RID: 5994 RVA: 0x0008726D File Offset: 0x0008546D
	public void ActivateWithG(float g)
	{
		GreyZoneManager.Instance.LocalSimpleActivation(true, g);
	}

	// Token: 0x0600176B RID: 5995 RVA: 0x0008727D File Offset: 0x0008547D
	public void Deactivate()
	{
		GreyZoneManager.Instance.LocalSimpleActivation(false, 1f);
	}

	// Token: 0x040022A7 RID: 8871
	[SerializeField]
	private bool activateOnEnable;

	// Token: 0x040022A8 RID: 8872
	[SerializeField]
	private bool deactivateOnDisable;

	// Token: 0x040022A9 RID: 8873
	[Range(-5f, 5f)]
	[SerializeField]
	private float gMultiplier = 1f;
}
