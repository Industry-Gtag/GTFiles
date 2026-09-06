using System;
using UnityEngine;

// Token: 0x02000D5C RID: 3420
internal abstract class TickSystemPreTickMono : MonoBehaviour, ITickSystemPre
{
	// Token: 0x17000815 RID: 2069
	// (get) Token: 0x060054A1 RID: 21665 RVA: 0x001BC786 File Offset: 0x001BA986
	// (set) Token: 0x060054A2 RID: 21666 RVA: 0x001BC78E File Offset: 0x001BA98E
	public bool PreTickRunning { get; set; }

	// Token: 0x060054A3 RID: 21667 RVA: 0x001BC797 File Offset: 0x001BA997
	public virtual void OnEnable()
	{
		TickSystem<object>.AddPreTickCallback(this);
	}

	// Token: 0x060054A4 RID: 21668 RVA: 0x001BC79F File Offset: 0x001BA99F
	public void OnDisable()
	{
		TickSystem<object>.RemovePreTickCallback(this);
	}

	// Token: 0x060054A5 RID: 21669 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void PreTick()
	{
	}
}
