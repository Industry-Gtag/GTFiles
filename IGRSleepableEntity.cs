using System;
using UnityEngine;

// Token: 0x02000840 RID: 2112
public interface IGRSleepableEntity
{
	// Token: 0x170004C2 RID: 1218
	// (get) Token: 0x06003648 RID: 13896
	Vector3 Position { get; }

	// Token: 0x170004C3 RID: 1219
	// (get) Token: 0x06003649 RID: 13897
	float WakeUpRadius { get; }

	// Token: 0x0600364A RID: 13898
	bool IsSleeping();

	// Token: 0x0600364B RID: 13899
	void WakeUp();

	// Token: 0x0600364C RID: 13900
	void Sleep();
}
