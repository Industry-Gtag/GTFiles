using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class CrittersStunBomb : CrittersToolThrowable
{
	// Token: 0x06000306 RID: 774 RVA: 0x00011E0C File Offset: 0x0001000C
	protected override void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
		if (CrittersManager.instance.LocalAuthority())
		{
			Vector3 position = base.transform.position;
			List<CrittersPawn> crittersPawns = CrittersManager.instance.crittersPawns;
			for (int i = 0; i < crittersPawns.Count; i++)
			{
				CrittersPawn crittersPawn = crittersPawns[i];
				if (crittersPawn.isActiveAndEnabled && Vector3.Distance(crittersPawn.transform.position, position) < this.radius)
				{
					crittersPawn.Stunned(this.stunDuration);
				}
			}
			CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StunExplosion, this.actorId, position, Quaternion.LookRotation(hitNormal));
		}
	}

	// Token: 0x06000307 RID: 775 RVA: 0x00011EA0 File Offset: 0x000100A0
	protected override void OnImpactCritter(CrittersPawn impactedCritter)
	{
		if (CrittersManager.instance.LocalAuthority())
		{
			impactedCritter.Stunned(this.stunDuration);
		}
	}

	// Token: 0x04000368 RID: 872
	[Header("Stun Bomb")]
	public float radius = 1f;

	// Token: 0x04000369 RID: 873
	public float stunDuration = 5f;
}
