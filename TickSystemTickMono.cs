using System;
using UnityEngine;

// Token: 0x02000D5D RID: 3421
internal abstract class TickSystemTickMono : MonoBehaviour, ITickSystemTick
{
	// Token: 0x17000816 RID: 2070
	// (get) Token: 0x060054A7 RID: 21671 RVA: 0x001BC7A7 File Offset: 0x001BA9A7
	// (set) Token: 0x060054A8 RID: 21672 RVA: 0x001BC7AF File Offset: 0x001BA9AF
	public bool TickRunning { get; set; }

	// Token: 0x060054A9 RID: 21673 RVA: 0x0001A297 File Offset: 0x00018497
	public virtual void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060054AA RID: 21674 RVA: 0x0001A29F File Offset: 0x0001849F
	public virtual void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x060054AB RID: 21675 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void Tick()
	{
	}
}
