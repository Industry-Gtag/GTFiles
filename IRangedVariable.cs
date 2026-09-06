using System;
using UnityEngine;

// Token: 0x0200090E RID: 2318
public interface IRangedVariable<T> : IVariable<T>, IVariable
{
	// Token: 0x17000589 RID: 1417
	// (get) Token: 0x06003CC3 RID: 15555
	// (set) Token: 0x06003CC4 RID: 15556
	T Min { get; set; }

	// Token: 0x1700058A RID: 1418
	// (get) Token: 0x06003CC5 RID: 15557
	// (set) Token: 0x06003CC6 RID: 15558
	T Max { get; set; }

	// Token: 0x1700058B RID: 1419
	// (get) Token: 0x06003CC7 RID: 15559
	T Range { get; }

	// Token: 0x1700058C RID: 1420
	// (get) Token: 0x06003CC8 RID: 15560
	AnimationCurve Curve { get; }
}
