using System;
using GorillaLocomotion;
using GorillaTagScripts.VirtualStumpCustomMaps;
using UnityEngine;

// Token: 0x02000AD7 RID: 2775
public class VStumpDeactivateTrigger : MonoBehaviour
{
	// Token: 0x06004744 RID: 18244 RVA: 0x001808DF File Offset: 0x0017EADF
	public void OnTriggerEnter(Collider other)
	{
		if (!this.armed)
		{
			return;
		}
		if (other != GTPlayer.Instance.headCollider)
		{
			return;
		}
		this.armed = false;
		CustomMapManager.Deactivate();
	}

	// Token: 0x06004745 RID: 18245 RVA: 0x00180909 File Offset: 0x0017EB09
	public void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			this.armed = true;
		}
	}

	// Token: 0x040059C6 RID: 22982
	private bool armed = true;
}
