using System;
using UnityEngine;

// Token: 0x020004B4 RID: 1204
internal interface ITetheredObjectBehavior
{
	// Token: 0x06001D6C RID: 7532
	void DbgClear();

	// Token: 0x06001D6D RID: 7533
	void EnableDistanceConstraints(bool v, float playerScale);

	// Token: 0x06001D6E RID: 7534
	void EnableDynamics(bool enable, bool collider, bool kinematic);

	// Token: 0x06001D6F RID: 7535
	bool IsEnabled();

	// Token: 0x06001D70 RID: 7536
	void ReParent();

	// Token: 0x06001D71 RID: 7537
	bool ReturnStep();

	// Token: 0x06001D72 RID: 7538
	void TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership);
}
