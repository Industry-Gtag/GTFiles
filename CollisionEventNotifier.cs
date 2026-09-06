using System;
using UnityEngine;

// Token: 0x02000D95 RID: 3477
public class CollisionEventNotifier : MonoBehaviour
{
	// Token: 0x14000099 RID: 153
	// (add) Token: 0x06005587 RID: 21895 RVA: 0x001BFAA4 File Offset: 0x001BDCA4
	// (remove) Token: 0x06005588 RID: 21896 RVA: 0x001BFADC File Offset: 0x001BDCDC
	public event CollisionEventNotifier.CollisionEvent CollisionEnterEvent;

	// Token: 0x1400009A RID: 154
	// (add) Token: 0x06005589 RID: 21897 RVA: 0x001BFB14 File Offset: 0x001BDD14
	// (remove) Token: 0x0600558A RID: 21898 RVA: 0x001BFB4C File Offset: 0x001BDD4C
	public event CollisionEventNotifier.CollisionEvent CollisionExitEvent;

	// Token: 0x0600558B RID: 21899 RVA: 0x001BFB81 File Offset: 0x001BDD81
	private void OnCollisionEnter(Collision collision)
	{
		CollisionEventNotifier.CollisionEvent collisionEnterEvent = this.CollisionEnterEvent;
		if (collisionEnterEvent == null)
		{
			return;
		}
		collisionEnterEvent(this, collision);
	}

	// Token: 0x0600558C RID: 21900 RVA: 0x001BFB95 File Offset: 0x001BDD95
	private void OnCollisionExit(Collision collision)
	{
		CollisionEventNotifier.CollisionEvent collisionExitEvent = this.CollisionExitEvent;
		if (collisionExitEvent == null)
		{
			return;
		}
		collisionExitEvent(this, collision);
	}

	// Token: 0x02000D96 RID: 3478
	// (Invoke) Token: 0x0600558F RID: 21903
	public delegate void CollisionEvent(CollisionEventNotifier notifier, Collision collision);
}
