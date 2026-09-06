using System;
using UnityEngine;

// Token: 0x020000C2 RID: 194
public class CosmeticCritterSpawnerButterflyNet : CosmeticCritterSpawnerTimed
{
	// Token: 0x060004C3 RID: 1219 RVA: 0x0001AACC File Offset: 0x00018CCC
	public override void SetRandomVariables(CosmeticCritter critter)
	{
		Vector3 vector = base.transform.position + Random.onUnitSphere * this.spawnRadius;
		(critter as CosmeticCritterButterfly).SetStartPos(vector);
	}

	// Token: 0x04000542 RID: 1346
	[Tooltip("Spawn a butterfly on the surface of a sphere with this radius, and with a center on this object.")]
	[SerializeField]
	private float spawnRadius = 1f;
}
