using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000CD7 RID: 3287
internal class OwnershipGaurd : MonoBehaviour
{
	// Token: 0x06005173 RID: 20851 RVA: 0x001AFA7B File Offset: 0x001ADC7B
	private void Start()
	{
		if (this.autoRegisterAll)
		{
			this.NetViews = base.GetComponents<PhotonView>();
		}
		if (this.NetViews == null)
		{
			return;
		}
		OwnershipGuardHandler.RegisterViews(this.NetViews);
	}

	// Token: 0x06005174 RID: 20852 RVA: 0x001AFAA5 File Offset: 0x001ADCA5
	private void OnDestroy()
	{
		if (this.NetViews == null)
		{
			return;
		}
		OwnershipGuardHandler.RemoveViews(this.NetViews);
	}

	// Token: 0x04006376 RID: 25462
	[SerializeField]
	private PhotonView[] NetViews;

	// Token: 0x04006377 RID: 25463
	[SerializeField]
	private bool autoRegisterAll = true;
}
