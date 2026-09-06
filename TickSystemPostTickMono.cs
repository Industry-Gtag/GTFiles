using System;
using UnityEngine;

// Token: 0x02000D5E RID: 3422
internal abstract class TickSystemPostTickMono : MonoBehaviour, ITickSystemPost
{
	// Token: 0x17000817 RID: 2071
	// (get) Token: 0x060054AD RID: 21677 RVA: 0x001BC7B8 File Offset: 0x001BA9B8
	// (set) Token: 0x060054AE RID: 21678 RVA: 0x001BC7C0 File Offset: 0x001BA9C0
	public bool PostTickRunning { get; set; }

	// Token: 0x060054AF RID: 21679 RVA: 0x001AF0B5 File Offset: 0x001AD2B5
	public virtual void OnEnable()
	{
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x060054B0 RID: 21680 RVA: 0x0015AB83 File Offset: 0x00158D83
	public virtual void OnDisable()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x060054B1 RID: 21681 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void PostTick()
	{
	}
}
