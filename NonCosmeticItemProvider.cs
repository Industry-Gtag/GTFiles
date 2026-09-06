using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200048C RID: 1164
public class NonCosmeticItemProvider : MonoBehaviour
{
	// Token: 0x06001C5D RID: 7261 RVA: 0x00099AC4 File Offset: 0x00097CC4
	private void OnTriggerEnter(Collider other)
	{
		GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component != null)
		{
			GorillaGameManager.instance.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer).netView.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[] { true, component.isLeftHand });
		}
	}

	// Token: 0x04002676 RID: 9846
	public GTZone zone;

	// Token: 0x04002677 RID: 9847
	[Tooltip("only for honeycomb")]
	public bool useCondition;

	// Token: 0x04002678 RID: 9848
	public int conditionThreshold;

	// Token: 0x04002679 RID: 9849
	public NonCosmeticItemProvider.ItemType itemType;

	// Token: 0x0200048D RID: 1165
	public enum ItemType
	{
		// Token: 0x0400267B RID: 9851
		honeycomb
	}
}
