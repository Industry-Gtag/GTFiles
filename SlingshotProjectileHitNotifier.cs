using System;
using GorillaTag.GuidedRefs;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x020004C2 RID: 1218
public class SlingshotProjectileHitNotifier : BaseGuidedRefTargetMono
{
	// Token: 0x1400003C RID: 60
	// (add) Token: 0x06001DC6 RID: 7622 RVA: 0x000A11C8 File Offset: 0x0009F3C8
	// (remove) Token: 0x06001DC7 RID: 7623 RVA: 0x000A1200 File Offset: 0x0009F400
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileHit;

	// Token: 0x1400003D RID: 61
	// (add) Token: 0x06001DC8 RID: 7624 RVA: 0x000A1238 File Offset: 0x0009F438
	// (remove) Token: 0x06001DC9 RID: 7625 RVA: 0x000A1270 File Offset: 0x0009F470
	public event SlingshotProjectileHitNotifier.PaperPlaneProjectileHitEvent OnPaperPlaneHit;

	// Token: 0x1400003E RID: 62
	// (add) Token: 0x06001DCA RID: 7626 RVA: 0x000A12A8 File Offset: 0x0009F4A8
	// (remove) Token: 0x06001DCB RID: 7627 RVA: 0x000A12E0 File Offset: 0x0009F4E0
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileCollisionStay;

	// Token: 0x1400003F RID: 63
	// (add) Token: 0x06001DCC RID: 7628 RVA: 0x000A1318 File Offset: 0x0009F518
	// (remove) Token: 0x06001DCD RID: 7629 RVA: 0x000A1350 File Offset: 0x0009F550
	public event SlingshotProjectileHitNotifier.ProjectileTriggerEvent OnProjectileTriggerEnter;

	// Token: 0x14000040 RID: 64
	// (add) Token: 0x06001DCE RID: 7630 RVA: 0x000A1388 File Offset: 0x0009F588
	// (remove) Token: 0x06001DCF RID: 7631 RVA: 0x000A13C0 File Offset: 0x0009F5C0
	public event SlingshotProjectileHitNotifier.ProjectileTriggerEvent OnProjectileTriggerExit;

	// Token: 0x06001DD0 RID: 7632 RVA: 0x000A13F5 File Offset: 0x0009F5F5
	public void InvokeHit(SlingshotProjectile projectile, Collision collision)
	{
		if (this.projectileType != "" && projectile.tag != this.projectileType)
		{
			return;
		}
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileHit = this.OnProjectileHit;
		if (onProjectileHit == null)
		{
			return;
		}
		onProjectileHit(projectile, collision);
	}

	// Token: 0x06001DD1 RID: 7633 RVA: 0x000A142F File Offset: 0x0009F62F
	public void InvokeHit(PaperPlaneProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.PaperPlaneProjectileHitEvent onPaperPlaneHit = this.OnPaperPlaneHit;
		if (onPaperPlaneHit == null)
		{
			return;
		}
		onPaperPlaneHit(projectile, collider);
	}

	// Token: 0x06001DD2 RID: 7634 RVA: 0x000A1443 File Offset: 0x0009F643
	public void InvokeCollisionStay(SlingshotProjectile projectile, Collision collision)
	{
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileCollisionStay = this.OnProjectileCollisionStay;
		if (onProjectileCollisionStay == null)
		{
			return;
		}
		onProjectileCollisionStay(projectile, collision);
	}

	// Token: 0x06001DD3 RID: 7635 RVA: 0x000A1457 File Offset: 0x0009F657
	public void InvokeTriggerEnter(SlingshotProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.ProjectileTriggerEvent onProjectileTriggerEnter = this.OnProjectileTriggerEnter;
		if (onProjectileTriggerEnter == null)
		{
			return;
		}
		onProjectileTriggerEnter(projectile, collider);
	}

	// Token: 0x06001DD4 RID: 7636 RVA: 0x000A146B File Offset: 0x0009F66B
	public void InvokeTriggerExit(SlingshotProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.ProjectileTriggerEvent onProjectileTriggerExit = this.OnProjectileTriggerExit;
		if (onProjectileTriggerExit == null)
		{
			return;
		}
		onProjectileTriggerExit(projectile, collider);
	}

	// Token: 0x06001DD5 RID: 7637 RVA: 0x000A147F File Offset: 0x0009F67F
	private new void OnDestroy()
	{
		this.OnProjectileHit = null;
		this.OnProjectileCollisionStay = null;
		this.OnProjectileTriggerEnter = null;
		this.OnProjectileTriggerExit = null;
	}

	// Token: 0x0400283B RID: 10299
	[TagField]
	[SerializeField]
	private string projectileType;

	// Token: 0x020004C3 RID: 1219
	// (Invoke) Token: 0x06001DD8 RID: 7640
	public delegate void ProjectileHitEvent(SlingshotProjectile projectile, Collision collision);

	// Token: 0x020004C4 RID: 1220
	// (Invoke) Token: 0x06001DDC RID: 7644
	public delegate void PaperPlaneProjectileHitEvent(PaperPlaneProjectile projectile, Collider collider);

	// Token: 0x020004C5 RID: 1221
	// (Invoke) Token: 0x06001DE0 RID: 7648
	public delegate void ProjectileTriggerEvent(SlingshotProjectile projectile, Collider collider);
}
