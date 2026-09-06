using System;
using UnityEngine;

// Token: 0x020000D9 RID: 217
public class SIBlasterPumpableProjectile : MonoBehaviour, SIGadgetProjectileModifier
{
	// Token: 0x0600051E RID: 1310 RVA: 0x0001CA74 File Offset: 0x0001AC74
	public void ModifyProjectile(SIGadgetBlasterProjectile projectile)
	{
		SIGadgetPumpBlaster component = projectile.parentBlaster.GetComponent<SIGadgetPumpBlaster>();
		if (component == null)
		{
			return;
		}
		this.pumpChargedAmount = Mathf.Min(this.maxPump, component.currentPumpChargeAmount);
		projectile.startingVelocity += this.pumpChargedAmount;
		if (this.strengthPerPumpCharge > 0f)
		{
			SIBlasterDirectHitProjectile component2 = projectile.GetComponent<SIBlasterDirectHitProjectile>();
			if (component2 != null)
			{
				component2.knockbackSpeed += this.strengthPerPumpCharge * this.pumpChargedAmount;
			}
			SIBlasterSplashProjectile component3 = projectile.GetComponent<SIBlasterSplashProjectile>();
			if (component3 != null)
			{
				component3.knockbackSpeed += this.strengthPerPumpCharge * this.pumpChargedAmount;
			}
			SIBlasterSprayProjectile component4 = projectile.GetComponent<SIBlasterSprayProjectile>();
			if (component4 != null)
			{
				component4.knockbackSpeed += this.strengthPerPumpCharge * this.pumpChargedAmount;
			}
		}
	}

	// Token: 0x040005E2 RID: 1506
	public float maxPump;

	// Token: 0x040005E3 RID: 1507
	public float pumpChargedAmount;

	// Token: 0x040005E4 RID: 1508
	public float velocityPerPumpCharge;

	// Token: 0x040005E5 RID: 1509
	public float strengthPerPumpCharge;
}
