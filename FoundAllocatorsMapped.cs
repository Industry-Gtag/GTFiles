using System;
using System.Collections.Generic;

// Token: 0x02000292 RID: 658
[Serializable]
public class FoundAllocatorsMapped
{
	// Token: 0x04001506 RID: 5382
	public string path;

	// Token: 0x04001507 RID: 5383
	public List<ViewsAndAllocator> allocators = new List<ViewsAndAllocator>();

	// Token: 0x04001508 RID: 5384
	public List<FoundAllocatorsMapped> subGroups = new List<FoundAllocatorsMapped>();
}
