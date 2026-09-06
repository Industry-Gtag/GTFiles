using System;
using UnityEngine;

// Token: 0x02000D53 RID: 3411
public abstract class MonoBehaviourTick : MonoBehaviour, ITickSystemTick
{
	// Token: 0x1700080F RID: 2063
	// (get) Token: 0x0600546C RID: 21612 RVA: 0x001BC345 File Offset: 0x001BA545
	// (set) Token: 0x0600546D RID: 21613 RVA: 0x001BC34D File Offset: 0x001BA54D
	public bool TickRunning { get; set; }

	// Token: 0x0600546E RID: 21614 RVA: 0x0001A297 File Offset: 0x00018497
	public void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x0600546F RID: 21615 RVA: 0x0001A29F File Offset: 0x0001849F
	public void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06005470 RID: 21616
	public abstract void Tick();
}
