using System;
using UnityEngine;

// Token: 0x020006E2 RID: 1762
public struct GameHitData
{
	// Token: 0x040038E6 RID: 14566
	public GameEntityId hitEntityId;

	// Token: 0x040038E7 RID: 14567
	public GameEntityId hitByEntityId;

	// Token: 0x040038E8 RID: 14568
	public int hitTypeId;

	// Token: 0x040038E9 RID: 14569
	public Vector3 hitEntityPosition;

	// Token: 0x040038EA RID: 14570
	public Vector3 hitPosition;

	// Token: 0x040038EB RID: 14571
	public Vector3 hitImpulse;

	// Token: 0x040038EC RID: 14572
	public int hitAmount;

	// Token: 0x040038ED RID: 14573
	public int hittablePoint;
}
