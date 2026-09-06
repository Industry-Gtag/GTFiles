using System;
using UnityEngine;

// Token: 0x02000D54 RID: 3412
public abstract class MonoBehaviourPostTick : MonoBehaviour, ITickSystemPost
{
	// Token: 0x17000810 RID: 2064
	// (get) Token: 0x06005472 RID: 21618 RVA: 0x001BC356 File Offset: 0x001BA556
	// (set) Token: 0x06005473 RID: 21619 RVA: 0x001BC35E File Offset: 0x001BA55E
	public bool PostTickRunning { get; set; }

	// Token: 0x06005474 RID: 21620 RVA: 0x001AF0B5 File Offset: 0x001AD2B5
	public virtual void OnEnable()
	{
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06005475 RID: 21621 RVA: 0x0015AB83 File Offset: 0x00158D83
	public virtual void OnDisable()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x06005476 RID: 21622
	public abstract void PostTick();
}
