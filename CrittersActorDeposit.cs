using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200004B RID: 75
public class CrittersActorDeposit : MonoBehaviour
{
	// Token: 0x06000169 RID: 361 RVA: 0x00008F2C File Offset: 0x0000712C
	public void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody.IsNotNull())
		{
			CrittersActor component = other.attachedRigidbody.GetComponent<CrittersActor>();
			if (CrittersManager.instance.LocalAuthority() && component.IsNotNull() && this.CanDeposit(component) && this.IsAttachAvailable())
			{
				this.HandleDeposit(component);
			}
		}
	}

	// Token: 0x0600016A RID: 362 RVA: 0x00008F80 File Offset: 0x00007180
	protected virtual bool CanDeposit(CrittersActor depositActor)
	{
		if (depositActor.crittersActorType != this.actorType)
		{
			return false;
		}
		CrittersActor crittersActor;
		if (CrittersManager.instance.actorById.TryGetValue(depositActor.parentActorId, out crittersActor))
		{
			return crittersActor.crittersActorType == CrittersActor.CrittersActorType.Grabber;
		}
		return depositActor.parentActorId == -1;
	}

	// Token: 0x0600016B RID: 363 RVA: 0x00008FCC File Offset: 0x000071CC
	private bool IsAttachAvailable()
	{
		return this.allowMultiAttach || this.currentAttach == null;
	}

	// Token: 0x0600016C RID: 364 RVA: 0x00008FE4 File Offset: 0x000071E4
	protected virtual void HandleDeposit(CrittersActor depositedActor)
	{
		this.currentAttach = depositedActor;
		depositedActor.ReleasedEvent.AddListener(new UnityAction<CrittersActor>(this.HandleDetach));
		CrittersActor crittersActor = this.attachPoint;
		bool flag = this.snapOnAttach;
		bool flag2 = this.disableGrabOnAttach;
		depositedActor.GrabbedBy(crittersActor, flag, default(Quaternion), default(Vector3), flag2);
	}

	// Token: 0x0600016D RID: 365 RVA: 0x0000903C File Offset: 0x0000723C
	protected virtual void HandleDetach(CrittersActor detachingActor)
	{
		this.currentAttach = null;
	}

	// Token: 0x04000183 RID: 387
	public CrittersActor attachPoint;

	// Token: 0x04000184 RID: 388
	public CrittersActor.CrittersActorType actorType;

	// Token: 0x04000185 RID: 389
	public bool disableGrabOnAttach;

	// Token: 0x04000186 RID: 390
	public bool allowMultiAttach;

	// Token: 0x04000187 RID: 391
	public bool snapOnAttach;

	// Token: 0x04000188 RID: 392
	private CrittersActor currentAttach;
}
