using System;
using UnityEngine;

// Token: 0x020000DA RID: 218
public class SIBlasterRandomAngularVelocity : MonoBehaviour, SIGadgetProjectileModifier
{
	// Token: 0x06000520 RID: 1312 RVA: 0x0001CB4C File Offset: 0x0001AD4C
	public void ModifyProjectile(SIGadgetBlasterProjectile projectile)
	{
		projectile.rb.angularVelocity = new Vector3(Random.Range(-this.maxVel, this.maxVel), Random.Range(-this.maxVel, this.maxVel), Random.Range(-this.maxVel, this.maxVel));
	}

	// Token: 0x040005E6 RID: 1510
	public float maxVel;
}
