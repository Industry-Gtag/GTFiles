using System;
using UnityEngine;

// Token: 0x02000427 RID: 1063
public class MonkeyeProjectileTarget : MonoBehaviour
{
	// Token: 0x0600194B RID: 6475 RVA: 0x0008E8C9 File Offset: 0x0008CAC9
	private void Awake()
	{
		this.monkeyeAI = base.GetComponent<MonkeyeAI>();
		this.notifier = base.GetComponentInChildren<SlingshotProjectileHitNotifier>();
	}

	// Token: 0x0600194C RID: 6476 RVA: 0x0008E8E3 File Offset: 0x0008CAE3
	private void OnEnable()
	{
		if (this.notifier != null)
		{
			this.notifier.OnProjectileHit += this.Notifier_OnProjectileHit;
			this.notifier.OnPaperPlaneHit += this.Notifier_OnPaperPlaneHit;
		}
	}

	// Token: 0x0600194D RID: 6477 RVA: 0x0008E921 File Offset: 0x0008CB21
	private void OnDisable()
	{
		if (this.notifier != null)
		{
			this.notifier.OnProjectileHit -= this.Notifier_OnProjectileHit;
			this.notifier.OnPaperPlaneHit -= this.Notifier_OnPaperPlaneHit;
		}
	}

	// Token: 0x0600194E RID: 6478 RVA: 0x0008E95F File Offset: 0x0008CB5F
	private void Notifier_OnProjectileHit(SlingshotProjectile projectile, Collision collision)
	{
		this.monkeyeAI.SetSleep();
	}

	// Token: 0x0600194F RID: 6479 RVA: 0x0008E95F File Offset: 0x0008CB5F
	private void Notifier_OnPaperPlaneHit(PaperPlaneProjectile projectile, Collider collider)
	{
		this.monkeyeAI.SetSleep();
	}

	// Token: 0x0400245D RID: 9309
	private MonkeyeAI monkeyeAI;

	// Token: 0x0400245E RID: 9310
	private SlingshotProjectileHitNotifier notifier;
}
