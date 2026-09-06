using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005E6 RID: 1510
public class GorillaTriggerBoxGameFlag : GorillaTriggerBox
{
	// Token: 0x060025BF RID: 9663 RVA: 0x000C884D File Offset: 0x000C6A4D
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		PhotonView.Get(Object.FindAnyObjectByType<GorillaGameManager>()).RPC(this.functionName, RpcTarget.MasterClient, null);
	}

	// Token: 0x0400315A RID: 12634
	public string functionName;
}
