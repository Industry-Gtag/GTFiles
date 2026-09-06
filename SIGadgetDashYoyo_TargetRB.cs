using System;
using UnityEngine;

// Token: 0x020000EC RID: 236
public class SIGadgetDashYoyo_TargetRB : MonoBehaviour
{
	// Token: 0x06000590 RID: 1424 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected void OnEnable()
	{
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x0001FF08 File Offset: 0x0001E108
	protected void OnTriggerEnter(Collider otherCollider)
	{
		if (base.isActiveAndEnabled && this.gadget.gameEntity.IsAuthority() && (this.gadget.gameEntity.heldByActorNumber != -1 || this.gadget.gameEntity.snappedByActorNumber != -1) && (otherCollider.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider) || otherCollider.gameObject.IsOnLayer(UnityLayer.GorillaSlingshotCollider)) && !ApplicationQuittingState.IsQuitting)
		{
			SuperInfectionGame superInfectionGame = GorillaGameManager.instance as SuperInfectionGame;
			if (superInfectionGame != null)
			{
				VRRig componentInParent = otherCollider.GetComponentInParent<VRRig>();
				if (componentInParent == null)
				{
					return;
				}
				NetPlayer creator = componentInParent.creator;
				if (creator == null)
				{
					return;
				}
				if (SuperInfectionManager.GetSIManagerForZone(this.gadget.gameEntity.manager.zone) == null)
				{
					return;
				}
				this.gadget.OnHitPlayer_Authority(superInfectionGame, creator);
				return;
			}
		}
	}

	// Token: 0x040006AA RID: 1706
	[SerializeField]
	private SIGadgetDashYoyo gadget;
}
