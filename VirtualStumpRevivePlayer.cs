using System;
using UnityEngine;

// Token: 0x02000AD1 RID: 2769
public class VirtualStumpRevivePlayer : MonoBehaviour
{
	// Token: 0x06004705 RID: 18181 RVA: 0x0017F744 File Offset: 0x0017D944
	private void OnTriggerEnter(Collider collider)
	{
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			VRRig component = attachedRigidbody.GetComponent<VRRig>();
			if (component != null)
			{
				GRPlayer component2 = component.GetComponent<GRPlayer>();
				if (component2 != null && (component2.State != GRPlayer.GRPlayerState.Alive || component2.Hp < component2.MaxHp))
				{
					if (!NetworkSystem.Instance.InRoom && component == VRRig.LocalRig)
					{
						this.defaultReviveStation.RevivePlayer(component2);
					}
					if (this.ghostReactorManager.IsAuthority())
					{
						this.ghostReactorManager.RequestPlayerRevive(this.defaultReviveStation, component2);
					}
				}
			}
		}
	}

	// Token: 0x04005999 RID: 22937
	[SerializeField]
	private GhostReactorManager ghostReactorManager;

	// Token: 0x0400599A RID: 22938
	[SerializeField]
	private GRReviveStation defaultReviveStation;
}
