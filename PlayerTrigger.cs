using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000CED RID: 3309
public abstract class PlayerTrigger : MonoBehaviour
{
	// Token: 0x060051E9 RID: 20969 RVA: 0x001B361B File Offset: 0x001B181B
	protected virtual void Awake()
	{
		this.triggerCollisionEvents.CompositeTriggerEnter += this.OnCompositeTriggerEnter;
		this.triggerCollisionEvents.CompositeTriggerExit += this.OnCompositeTriggerExit;
	}

	// Token: 0x060051EA RID: 20970 RVA: 0x001B364B File Offset: 0x001B184B
	private void OnCompositeTriggerEnter(Collider collider)
	{
		if (!this.isPlayerCollided && collider == GTPlayer.Instance.bodyCollider)
		{
			this.playerCollider = collider;
			this.PlayerEnter();
		}
	}

	// Token: 0x060051EB RID: 20971 RVA: 0x001B3674 File Offset: 0x001B1874
	private void OnCompositeTriggerExit(Collider collider)
	{
		if (this.isPlayerCollided && collider == this.playerCollider)
		{
			this.PlayerExit();
		}
	}

	// Token: 0x060051EC RID: 20972 RVA: 0x001B3692 File Offset: 0x001B1892
	protected virtual void PlayerEnter()
	{
		this.isPlayerCollided = true;
	}

	// Token: 0x060051ED RID: 20973 RVA: 0x001B369B File Offset: 0x001B189B
	protected virtual void PlayerExit()
	{
		this.playerCollider = null;
		this.isPlayerCollided = false;
	}

	// Token: 0x04006444 RID: 25668
	protected bool isPlayerCollided;

	// Token: 0x04006445 RID: 25669
	protected Collider playerCollider;

	// Token: 0x04006446 RID: 25670
	[SerializeField]
	private CompositeTriggerEvents triggerCollisionEvents;
}
