using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200082E RID: 2094
public class GRToolUpgradeStationDepositBox : MonoBehaviour
{
	// Token: 0x060035D5 RID: 13781 RVA: 0x00129750 File Offset: 0x00127950
	public void OnTriggerEnter(Collider other)
	{
		GRTool component = other.attachedRigidbody.GetComponent<GRTool>();
		if (component.IsNotNull() && component.gameEntity.IsNotNull() && component.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber && component.gameEntity.IsHeldByLocalPlayer())
		{
			Debug.LogError("Tool Deposited");
			this.upgradeStation.ToolInserted(component);
		}
	}

	// Token: 0x0400466F RID: 18031
	public GRToolUpgradeStation upgradeStation;
}
