using System;
using UnityEngine;

// Token: 0x02000D5B RID: 3419
internal abstract class TickSystemMono : MonoBehaviour, ITickSystem, ITickSystemPre, ITickSystemTick, ITickSystemPost
{
	// Token: 0x17000812 RID: 2066
	// (get) Token: 0x06005495 RID: 21653 RVA: 0x001BC743 File Offset: 0x001BA943
	// (set) Token: 0x06005496 RID: 21654 RVA: 0x001BC74B File Offset: 0x001BA94B
	public bool PreTickRunning { get; set; }

	// Token: 0x17000813 RID: 2067
	// (get) Token: 0x06005497 RID: 21655 RVA: 0x001BC754 File Offset: 0x001BA954
	// (set) Token: 0x06005498 RID: 21656 RVA: 0x001BC75C File Offset: 0x001BA95C
	public bool TickRunning { get; set; }

	// Token: 0x17000814 RID: 2068
	// (get) Token: 0x06005499 RID: 21657 RVA: 0x001BC765 File Offset: 0x001BA965
	// (set) Token: 0x0600549A RID: 21658 RVA: 0x001BC76D File Offset: 0x001BA96D
	public bool PostTickRunning { get; set; }

	// Token: 0x0600549B RID: 21659 RVA: 0x001BC776 File Offset: 0x001BA976
	public virtual void OnEnable()
	{
		TickSystem<object>.AddTickSystemCallBack(this);
	}

	// Token: 0x0600549C RID: 21660 RVA: 0x001BC77E File Offset: 0x001BA97E
	public virtual void OnDisable()
	{
		TickSystem<object>.RemoveTickSystemCallback(this);
	}

	// Token: 0x0600549D RID: 21661 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void PreTick()
	{
	}

	// Token: 0x0600549E RID: 21662 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void Tick()
	{
	}

	// Token: 0x0600549F RID: 21663 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void PostTick()
	{
	}
}
