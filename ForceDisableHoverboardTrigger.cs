using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000A78 RID: 2680
public class ForceDisableHoverboardTrigger : MonoBehaviour
{
	// Token: 0x0600451B RID: 17691 RVA: 0x00170D41 File Offset: 0x0016EF41
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.AddHoverDisabler(this);
		}
	}

	// Token: 0x0600451C RID: 17692 RVA: 0x00170D60 File Offset: 0x0016EF60
	public void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			if (this.reEnableOnlyInVStump && !GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				GTPlayer.Instance.ForceHoverDisallowed();
				return;
			}
			GTPlayer.Instance.RemoveHoverDisabler(this);
		}
	}

	// Token: 0x0600451D RID: 17693 RVA: 0x00170DA0 File Offset: 0x0016EFA0
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			instance.RemoveHoverDisabler(this);
		}
	}

	// Token: 0x040056FC RID: 22268
	public bool reEnableOnlyInVStump = true;
}
